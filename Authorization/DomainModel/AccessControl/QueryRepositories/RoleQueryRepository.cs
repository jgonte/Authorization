using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class RoleQueryRepository : EntityQueryRepository<Role, int>
    {
        public override Role GetById(int roleId)
        {
            var result = Query<Role>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetById]")
                .Parameters(
                    p => p.Name("roleId").Value(roleId)
                )
                .Execute();

            return result.Data;
        }

        public async override Task<Role> GetByIdAsync(int roleId)
        {
            var result = await Query<Role>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_GetById]")
                .Parameters(
                    p => p.Name("roleId").Value(roleId)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public IEnumerable<Role> GetAllRolesForUser(int userId)
        {
            var result = Query<Role>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetAllRoles]")
                .Parameters(
                    p => p.Name("userId").Value(userId)
                )
                .Execute();

            return result.Data;
        }

        public async Task<IEnumerable<Role>> GetAllRolesForUserAsync(int userId)
        {
            var result = await Query<Role>
                .Collection()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_GetAllRoles]")
                .Parameters(
                    p => p.Name("userId").Value(userId)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public static void Register(DomainFramework.DataAccess.RepositoryContext context)
        {
            context.RegisterQueryRepository<Role>(new RoleQueryRepository());
        }

    }
}