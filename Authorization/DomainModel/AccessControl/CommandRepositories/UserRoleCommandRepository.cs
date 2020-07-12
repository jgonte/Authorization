using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class UserRoleCommandRepository : EntityCommandRepository<UserRole>
    {
        protected override Command CreateInsertCommand(UserRole entity, IAuthenticatedUser user, string selector)
        {
            if (user != null)
            {
                entity.CreatedBy = (int)user.Id;
            }

            var command = Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUserRole_Insert]")
                .Parameters(
                    p => p.Name("createdBy").Value(entity.CreatedBy)
                )
                .OnBeforeCommandExecuted(cmd =>
                {
                    var dependencies = Dependencies();

                    if (!dependencies.Any())
                    {
                        cmd.Parameters(
                            p => p.Name("roleId").Value(entity.Id.RoleId),
                            p => p.Name("userId").Value(entity.Id.UserId)
                        );
                    }
                    else
                    {
                        switch (selector)
                        {
                            case "Roles":
                                {
                                    var user_ = (User)dependencies.ElementAt(0).Entity;

                                    cmd.Parameters(
                                        p => p.Name("roleId").Value(entity.Id.RoleId),
                                        p => p.Name("userId").Value(user_.Id)
                                    );
                                }
                                break;

                            case "Users":
                                {
                                    var role = (Role)dependencies.ElementAt(0).Entity;

                                    cmd.Parameters(
                                        p => p.Name("roleId").Value(role.Id),
                                        p => p.Name("userId").Value(entity.Id.UserId)
                                    );
                                }
                                break;

                            default:
                                {
                                    var role = (Role)dependencies.Single(d => d.Selector == "Roles").Entity;

                                    var user_ = (User)dependencies.Single(d => d.Selector == "Users").Entity;

                                    entity.Id = new UserRoleId
                                    {
                                        RoleId = role.Id,
                                        UserId = user_.Id
                                    };

                                    cmd.Parameters(
                                        p => p.Name("roleId").Value(role.Id),
                                        p => p.Name("userId").Value(user_.Id)
                                    );
                                }
                                break;
                        }
                    }
                });

            return command;
        }

        protected override void HandleInsert(Command command)
        {
            ((NonQueryCommand)command).Execute();
        }

        protected async override Task HandleInsertAsync(Command command)
        {
            await ((NonQueryCommand)command).ExecuteAsync();
        }

        protected override Command CreateUpdateCommand(UserRole entity, IAuthenticatedUser user, string selector)
        {
            if (user != null)
            {
                entity.UpdatedBy = (int)user.Id;
            }

            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUserRole_Update]")
                .Parameters(
                    p => p.Name("roleId").Value(entity.Id.RoleId),
                    p => p.Name("userId").Value(entity.Id.UserId),
                    p => p.Name("updatedBy").Value(entity.UpdatedBy)
                );
        }

        protected override bool HandleUpdate(Command command)
        {
            var result = ((NonQueryCommand)command).Execute();

            return result.AffectedRows > 0;
        }

        protected async override Task<bool> HandleUpdateAsync(Command command)
        {
            var result = await ((NonQueryCommand)command).ExecuteAsync();

            return result.AffectedRows > 0;
        }

        protected override Command CreateDeleteCommand(UserRole entity, IAuthenticatedUser user, string selector)
        {
            return Command
                .NonQuery()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pUserRole_Delete]")
                .Parameters(
                    p => p.Name("roleId").Value(entity.Id.RoleId),
                    p => p.Name("userId").Value(entity.Id.UserId)
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