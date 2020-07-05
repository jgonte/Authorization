using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class IdentityProviderCommandRepository : EntityCommandRepository<IdentityProvider>
    {
        protected override Command CreateInsertCommand(IdentityProvider entity, IAuthenticatedUser user, string selector)
        {
            if (user != null)
            {
                entity.CreatedBy = (int)user.Id;
            }

            var command = Query<IdentityProvider>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pIdentityProvider_Insert]")
                .Parameters(
                    p => p.Name("name").Value(entity.Name),
                    p => p.Name("uri").Value(entity.Uri),
                    p => p.Name("createdBy").Value(entity.CreatedBy)
                )
                .Instance(entity)
                .MapProperties(
                    p => p.Name("Id").Index(0)
                );

            return command;
        }

        protected override void HandleInsert(Command command)
        {
            ((SingleQuery<IdentityProvider>)command).Execute();
        }

        protected async override Task HandleInsertAsync(Command command)
        {
            await ((SingleQuery<IdentityProvider>)command).ExecuteAsync();
        }

        protected override Command CreateDeleteCommand(IdentityProvider entity, IAuthenticatedUser user, string selector)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pIdentityProvider_Delete]")
                .Parameters(
                    p => p.Name("identityProviderId").Value(entity.Id)
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

    }
}