using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class User_UserLogins_CommandRepository : LinkedValueObjectCommandRepository<UserLogin>
    {
        protected override Command CreateInsertCommand(UserLogin valueObject, IAuthenticatedUser user)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_AddUserLogins]")
                .Parameters(
                    p => p.Name("provider").Value(valueObject.Provider),
                    p => p.Name("userKey").Value(valueObject.UserKey)
                )
                .OnBeforeCommandExecuted(cmd =>
                {
                    var dependencies = Dependencies();

                    var entity = (User)dependencies.Single().Entity;

                    cmd.Parameters(
                        p => p.Name("userId").Value(entity.Id)
                    );
                });
        }

        protected override void HandleInsert(Command command)
        {
            ((NonQueryCommand)command).Execute();
        }

        protected async override Task HandleInsertAsync(Command command)
        {
            await ((NonQueryCommand)command).ExecuteAsync();
        }

        protected override Command CreateDeleteLinksCommand(IAuthenticatedUser user)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUser_DeleteUserLogins]")
                .OnBeforeCommandExecuted(cmd =>
                {
                    var dependencies = Dependencies();

                    var entity = (User)dependencies.Single().Entity;

                    cmd.Parameters(
                        p => p.Name("userId").Value(entity.Id)
                    );
                });
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

        public static void RegisterFactory(DomainFramework.DataAccess.RepositoryContext context)
        {
            context.RegisterCommandRepositoryFactory<RepositoryKey>(() => new User_UserLogins_CommandRepository());
        }

        public class RepositoryKey
        {
        }
    }
}