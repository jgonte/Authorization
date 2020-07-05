using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServerScriptRunner;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // Create the test database
            var script = File.ReadAllText(@"C:\tmp\Dev\Projects\Applications\Authorization.Solution\Authorization\DomainModel\Sql\CreateTestDatabase.sql");

            ScriptRunner.Run(ConnectionManager.GetConnection("Master").ConnectionString, script);
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        [TestMethod]
        public async Task Create_User_And_Grant_Roles_Test()
        {
            // Create principal
            var principal = new System.Security.Claims.ClaimsPrincipal();

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("email", "user1@mail.com"),
                new System.Security.Claims.Claim("aud", "Authorization"),
                new System.Security.Claims.Claim("iss", "http://localhost:5000"),
                new System.Security.Claims.Claim("sub", "5BE86359-073C-434B-AD2D-A3931111FFFF")
            });

            principal.AddIdentity(claimsIdentity);

            // Create user
            var appUser = new ApplicationUser();

            var user =  await appUser.GetOrCreate(principal);

            Assert.AreEqual(1, user.UserId);

            Assert.AreEqual("user1@mail.com", user.Email);

            // No roles added yet
            Assert.AreEqual(0, user.Roles.Count());

            var roleId = await ApplicationRole.Create("admin");

            // Associate the user with the role
            await appUser.GrantRoles(user.UserId, new int[] { roleId });

            user = await appUser.GetOrCreate(principal);

            Assert.AreEqual(1, user.UserId);

            Assert.AreEqual("user1@mail.com", user.Email);

            // No roles added yet
            Assert.AreEqual(1, user.Roles.Count());

            var role = user.Roles.First();

            Assert.AreEqual("admin", role.Name);

        }

        [TestMethod]
        public async Task Create_User_Retrieve_By_Email_Add_Another_User_Login_And_Grant_Roles_Test()
        {
            // Create principal
            var principal = new System.Security.Claims.ClaimsPrincipal();

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("email", "user2@mail.com"),
                new System.Security.Claims.Claim("aud", "Authorization"),
                new System.Security.Claims.Claim("iss", "http://localhost:5000"),
                new System.Security.Claims.Claim("sub", "ABC86359-073C-434B-AD2D-A3931111AAAA")
            });

            principal.AddIdentity(claimsIdentity);

            // Create user
            var appUser = new ApplicationUser();

            var user = await appUser.GetOrCreate(principal);

            Assert.AreEqual(2, user.UserId);

            Assert.AreEqual("user2@mail.com", user.Email);

            // No roles added yet
            Assert.AreEqual(0, user.Roles.Count());

            var roleId = await ApplicationRole.Create("admin");

            // Associate the user with the role
            await appUser.GrantRoles(user.UserId, new int[] { roleId });

            user = await appUser.GetOrCreate(principal);

            Assert.AreEqual(2, user.UserId);

            Assert.AreEqual("user2@mail.com", user.Email);

            Assert.AreEqual(1, user.Roles.Count());

            var role = user.Roles.First();

            Assert.AreEqual("admin", role.Name);

            // Verify the user has the user login set
            Assert.AreEqual(1, user.UserLogins.Count());

            var userLogin = user.UserLogins.Single();

            Assert.AreEqual("http://localhost:5000", userLogin.Provider);

            Assert.AreEqual("ABC86359-073C-434B-AD2D-A3931111AAAA", userLogin.UserKey);

            // Create another principal for the same user
            principal = new System.Security.Claims.ClaimsPrincipal();

            claimsIdentity = new System.Security.Claims.ClaimsIdentity(new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("email", "user2@mail.com"), // Same email
                new System.Security.Claims.Claim("aud", "Authorization"),
                new System.Security.Claims.Claim("iss", "http://localhost:5001"), // Different provider
                new System.Security.Claims.Claim("sub", "ABC86359-073C-434B-AD2D-A39311110000") // Different user key
            });

            principal.AddIdentity(claimsIdentity);

            user = await appUser.GetOrCreate(principal);

            Assert.AreEqual(2, user.UserId);

            Assert.AreEqual("user2@mail.com", user.Email);

            Assert.AreEqual(1, user.Roles.Count());

            role = user.Roles.First();

            Assert.AreEqual("admin", role.Name);

            // Verify the user kept the first user login added
            Assert.AreEqual(2, user.UserLogins.Count());

            userLogin = user.UserLogins.First();

            Assert.AreEqual("http://localhost:5000", userLogin.Provider);

            Assert.AreEqual("ABC86359-073C-434B-AD2D-A3931111AAAA", userLogin.UserKey);

            // Verify the user had the second user login added

            userLogin = user.UserLogins.Last();

            Assert.AreEqual("http://localhost:5001", userLogin.Provider);

            Assert.AreEqual("ABC86359-073C-434B-AD2D-A39311110000", userLogin.UserKey);

            // Create two roles and replace the admin one with those two roles
            int role1Id = await ApplicationRole.Create("role1");

            int role2Id = await ApplicationRole.Create("role2");

            await appUser.GrantRoles(user.UserId, new int[] { role1Id, role2Id });

            // Verify the user has the "admin" role replaced by "role1" and "role2"
            user = await appUser.GetOrCreate(principal);

            Assert.AreEqual(2, user.UserId);

            Assert.AreEqual("user2@mail.com", user.Email);

            Assert.AreEqual(2, user.Roles.Count());

            role = user.Roles.First();

            Assert.AreEqual("role1", role.Name);

            role = user.Roles.Last();

            Assert.AreEqual("role2", role.Name);

            // Verify the user kept the first user login added
            Assert.AreEqual(2, user.UserLogins.Count());

            userLogin = user.UserLogins.First();

            Assert.AreEqual("http://localhost:5000", userLogin.Provider);

            Assert.AreEqual("ABC86359-073C-434B-AD2D-A3931111AAAA", userLogin.UserKey);

            // Verify the user had the second user login added

            userLogin = user.UserLogins.Last();

            Assert.AreEqual("http://localhost:5001", userLogin.Provider);

            Assert.AreEqual("ABC86359-073C-434B-AD2D-A39311110000", userLogin.UserKey);

        }
    }
}
