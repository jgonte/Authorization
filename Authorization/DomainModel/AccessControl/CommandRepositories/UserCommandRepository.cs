using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class UserCommandRepository : EntityCommandRepository<User>
    {
        protected override Command CreateInsertCommand(User entity, IAuthenticatedUser user, string selector)
        {
            var command = Query<User>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_Insert]")
                .Parameters(
                    p => p.Name("email").Value(entity.Email),
                    p => p.Name("normalizedEmail").Value(entity.NormalizedEmail),
                    p => p.Name("userLogins_Provider").Value(entity.UserLogins?.Provider),
                    p => p.Name("userLogins_UserKey").Value(entity.UserLogins?.UserKey)
                )
                .Instance(entity)
                .MapProperties(
                    p => p.Name("Id").Index(0)
                );

            return command;
        }

        protected override void HandleInsert(Command command)
        {
            ((SingleQuery<User>)command).Execute();
        }

        protected async override Task HandleInsertAsync(Command command)
        {
            await ((SingleQuery<User>)command).ExecuteAsync();
        }

        protected override Command CreateDeleteCommand(User entity, IAuthenticatedUser user, string selector)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_Delete]")
                .Parameters(
                    p => p.Name("userId").Value(entity.Id)
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

        protected override Command CreateDeleteLinksCommand(User entity, IAuthenticatedUser user, string selector)
        {
            switch (selector)
            {
                case "UnlinkRolesFromUser": return Command
                    .NonQuery()
                    .Connection(AuthorizationConnectionClass.GetConnectionName())
                    .StoredProcedure("[AccessControl].[pUser_UnlinkRoles]")
                    .Parameters(
                        p => p.Name("userId").Value(entity.Id)
                    );

                case "DeleteUserLoginsFromUser": return Command
                    .NonQuery()
                    .Connection(AuthorizationConnectionClass.GetConnectionName())
                    .StoredProcedure("[AccessControl].[pUser_DeleteUserLogins]")
                    .Parameters(
                        p => p.Name("userId").Value(entity.Id)
                    );

                default: throw new InvalidOperationException();
            }
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