using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class RoleCommandRepository : EntityCommandRepository<Role>
    {
        protected override Command CreateInsertCommand(Role entity, IAuthenticatedUser user, string selector)
        {
            var command = Query<Role>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_Insert]")
                .Parameters(
                    p => p.Name("name").Value(entity.Name)
                )
                .Instance(entity)
                .MapProperties(
                    p => p.Name("Id").Index(0)
                );

            return command;
        }

        protected override void HandleInsert(Command command)
        {
            ((SingleQuery<Role>)command).Execute();
        }

        protected async override Task HandleInsertAsync(Command command)
        {
            await ((SingleQuery<Role>)command).ExecuteAsync();
        }

        protected override Command CreateDeleteCommand(Role entity, IAuthenticatedUser user, string selector)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_Delete]")
                .Parameters(
                    p => p.Name("roleId").Value(entity.Id)
                );
        }

        protected override bool HandleDelete(Command command)
        {
            var result = ((NonQueryCommand)command).Execute();

            return result.AffectedRows > 0;
        }

        protected async override Task<bool> HandleDeleteAsync(Command command)
        {
            var result = await ((NonQueryCommand)command).ExecuteAsync();

            return result.AffectedRows > 0;
        }

        protected override Command CreateDeleteLinksCommand(Role entity, IAuthenticatedUser user, string selector)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pRole_UnlinkUsers]")
                .Parameters(
                    p => p.Name("roleId").Value(entity.Id)
                );
        }

        protected override bool HandleDeleteLinks(Command command)
        {
            var result = ((NonQueryCommand)command).Execute();

            return result.AffectedRows > 0;
        }

        protected async override Task<bool> HandleDeleteLinksAsync(Command command)
        {
            var result = await ((NonQueryCommand)command).ExecuteAsync();

            return result.AffectedRows > 0;
        }

    }
}