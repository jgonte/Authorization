using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authorization.AccessControl
{
    public class ReplaceUserRolesCommandAggregate : CommandAggregate<User>
    {
        public ReplaceUserRolesCommandAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
        }

        public ReplaceUserRolesCommandAggregate(ReplaceUserRolesInputDto userRole, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(userRole, dependencies);
        }

        public override void Initialize(IInputDataTransferObject userRole, EntityDependency[] dependencies)
        {
            Initialize((ReplaceUserRolesInputDto)userRole, dependencies);
        }

        private void Initialize(ReplaceUserRolesInputDto userRole, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<User>(() => new UserCommandRepository());

            RootEntity = new User
            {
                Id = userRole.UserId
            };

            Enqueue(new DeleteLinksCommandOperation<User>(RootEntity, "UnlinkRolesFromUser"));

            if (userRole.Roles?.Any() == true)
            {
                foreach (var dto in userRole.Roles)
                {
                    ILinkedAggregateCommandOperation operation;

                    if (dto is RoleInputDto)
                    {
                        operation = new AddLinkedAggregateCommandOperation<User, CreateRoleCommandAggregate, RoleInputDto>(
                            RootEntity,
                            (RoleInputDto)dto
                        );

                        Enqueue(operation);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    Enqueue(new AddLinkedAggregateCommandOperation<User, CreateUserRoleCommandAggregate, UserRoleInputDto>(
                        RootEntity,
                        dto.UserRole,
                        new EntityDependency[]
                        {
                            new EntityDependency
                            {
                                Entity = RootEntity,
                                Selector = "Users"
                            },
                            new EntityDependency
                            {
                                Entity = operation.CommandAggregate.RootEntity,
                                Selector = "Roles"
                            }
                        }
                    ));
                }
            }
        }

    }
}