using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class UserQueryRepository : EntityQueryRepository<User, int?>
    {
        public override (int, IEnumerable<User>) Get(CollectionQueryParameters queryParameters)
        {
            var result = Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_Get]")
                .QueryParameters(queryParameters)
                .Parameters(p => p.Name("count").Size(20).Output())
                .Execute();

            var count = (string)result.GetParameter("count").Value;

            return (int.Parse(count), result.Data);
        }

        public async override Task<(int, IEnumerable<User>)> GetAsync(CollectionQueryParameters queryParameters)
        {
            var result = await Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_Get]")
                .QueryParameters(queryParameters)
                .Parameters(p => p.Name("count").Size(20).Output())
                .ExecuteAsync();

            var count = (string)result.GetParameter("count").Value;

            return (int.Parse(count), result.Data);
        }

        public User GetUserByNormalizedEmail(string email)
        {
            var result = Query<User>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetByNormalizedEmail]")
                .Parameters(
                    p => p.Name("email").Value(email)
                )
                .Execute();

            return result.Data;
        }

        public async Task<User> GetUserByNormalizedEmailAsync(string email)
        {
            var result = await Query<User>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetByNormalizedEmail]")
                .Parameters(
                    p => p.Name("email").Value(email)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public User GetUserByUserLogin(string provider, string userKey)
        {
            var result = Query<User>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetByUserLogin]")
                .Parameters(
                    p => p.Name("provider").Value(provider),
                    p => p.Name("userKey").Value(userKey)
                )
                .Execute();

            return result.Data;
        }

        public async Task<User> GetUserByUserLoginAsync(string provider, string userKey)
        {
            var result = await Query<User>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetByUserLogin]")
                .Parameters(
                    p => p.Name("provider").Value(provider),
                    p => p.Name("userKey").Value(userKey)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public IEnumerable<User> GetAllUsersForRole(int? roleId)
        {
            var result = Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetAllUsers]")
                .Parameters(
                    p => p.Name("roleId").Value(roleId.Value)
                )
                .Execute();

            return result.Data;
        }

        public async Task<IEnumerable<User>> GetAllUsersForRoleAsync(int? roleId)
        {
            var result = await Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetAllUsers]")
                .Parameters(
                    p => p.Name("roleId").Value(roleId.Value)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public (int, IEnumerable<User>) GetUsersForRole(int? roleId, CollectionQueryParameters queryParameters)
        {
            var result = Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetUsers]")
                .QueryParameters(queryParameters)
                .Parameters(p => p.Name("count").Size(20).Output())
                .Parameters(
                    p => p.Name("roleId").Value(roleId.Value)
                )
                .Execute();

            var count = (string)result.GetParameter("count").Value;

            return (int.Parse(count), result.Data);
        }

        public async Task<(int, IEnumerable<User>)> GetUsersForRoleAsync(int? roleId, CollectionQueryParameters queryParameters)
        {
            var result = await Query<User>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetUsers]")
                .QueryParameters(queryParameters)
                .Parameters(p => p.Name("count").Size(20).Output())
                .Parameters(
                    p => p.Name("roleId").Value(roleId.Value)
                )
                .ExecuteAsync();

            var count = (string)result.GetParameter("count").Value;

            return (int.Parse(count), result.Data);
        }

        public static void Register(DomainFramework.DataAccess.RepositoryContext context)
        {
            context.RegisterQueryRepository<User>(new UserQueryRepository());
        }

    }
}