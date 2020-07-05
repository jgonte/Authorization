using ApplicationLogs;
using Microsoft.Extensions.Configuration;
using NetCoreHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{

    public class ApplicationUser
    {
        /// <summary>
        /// Retrieves an existing user or creates one if it does not exist
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public virtual async Task<UserOutputDto> GetOrCreate(ClaimsPrincipal principal)
        {
            var claimsIdentity = await GetClaimsIdentityByAudience(principal);

            if (claimsIdentity == null) // Do not add a user with no claims identity for the audience of the application
            {
                return null;
            }

            // Email is required
            var emailClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == "email");

            if (emailClaim == null)
            {
                await ApplicationLogger.LogSecurity(message: $"No 'email' claim found for user");

                return null;
            }

            var email = emailClaim.Value;

            // Provider is required
            var providerClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == "iss");

            if (providerClaim == null)
            {
                await ApplicationLogger.LogSecurity(message: $"No 'iss' claim found for user with email: {email}");

                return null;
            }

            var provider = providerClaim.Value;

            // User key is required
            var userKeyClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == "sub");

            if (userKeyClaim == null)
            {
                await ApplicationLogger.LogSecurity(message: $"No 'sub' claim found for user with email: {email}");

                return null;
            }

            var userKey = userKeyClaim.Value;

            var user = await Get(email, provider, userKey);

            if (user == null)
            {
                await Create(email, provider, userKey);
            }

            return await Get(email, provider, userKey);
        }

        public async Task GrantRoles(int userId, int[] roleIds)
        {
            var userRoleAggregate = new ReplaceUserRolesCommandAggregate(new ReplaceUserRolesInputDto
            {
                Id = userId,
                Roles = roleIds.Select(roleId => new CreateUserRoleInputDto
                {
                    RoleId = roleId
                })
                .ToList()
            });

            await userRoleAggregate.SaveAsync();
        }

        protected virtual async Task<UserOutputDto> Get(string email, string provider, string userKey)
        {
            var getUserByUserLoginQueryAggregate = new GetUserByUserLoginQueryAggregate();

            var user = await getUserByUserLoginQueryAggregate.GetAsync(provider, userKey);

            if (user == null)
            {
                var getUserByNormalizedEmailQueryAggregate = new GetUserByNormalizedEmailQueryAggregate();

                user = await getUserByNormalizedEmailQueryAggregate.GetAsync(email);

                if (user != null) // User not found by login id but found by email
                {
                    // Add the login to the user
                    var userLogin = new AddUserLoginsInputDto
                    {
                        Id = user.Id,
                        UserLogins = new List<AddUserLoginInputDto>
                        {
                            new AddUserLoginInputDto
                            {
                                Provider = provider,
                                UserKey = userKey
                            }
                        }
                    };

                    var addUserLoginCommandAggregate = new AddUserLoginCommandAggregate(userLogin);

                    await addUserLoginCommandAggregate.SaveAsync();
                }
            }

            return user;
        }

        protected virtual async Task Create(string email, string provider, string userKey)
        {
            var userAggregate = new CreateUserCommandAggregate(new CreateUserInputDto
            {
                Email = email,
                UserLogins = new List<CreateUserLoginInputDto>
                {
                    new CreateUserLoginInputDto
                    {
                        Provider = provider,
                        UserKey = userKey
                    }
                }
            });

            await userAggregate.SaveAsync();

            var userId = userAggregate.RootEntity.Id.Value;
        }

        private static async Task<ClaimsIdentity> GetClaimsIdentityByAudience(ClaimsPrincipal principal)
        {
            var section = (ConfigurationSection)ConfigurationHelper.GetConfiguration().GetSection("Audience");

            var audience = section.Value;

            if (audience == null)
            {
                throw new ArgumentNullException("audience");
            }

            // Verify this is the intended audience of the token
            var claimsIdentity = principal.Identities.FirstOrDefault(
                i => i.Claims.Any(c => c.Type == "aud" && c.Value == audience)
            );

            if (claimsIdentity == null)
            {
                await ApplicationLogger
                    .LogSecurity(
                        message: $"No OAuth claims found for user: {principal.Identity?.Name ?? "Unknown"} and audience : {audience}"
                    );
            }

            return claimsIdentity;
        }
    }

}