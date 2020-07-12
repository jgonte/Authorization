using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authorization.AccessControl
{
    public class CreateUserRoleCommandAggregate : CommandAggregate<UserRole>
    {
        public CreateUserRoleCommandAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
        }

        public CreateUserRoleCommandAggregate(CreateUserRoleInputDto userRole, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(userRole, dependencies);
        }

        public override void Initialize(IInputDataTransferObject userRole, EntityDependency[] dependencies)
        {
            Initialize((CreateUserRoleInputDto)userRole, dependencies);
        }

        private void Initialize(CreateUserRoleInputDto userRole, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<UserRole>(() => new UserRoleCommandRepository());

            RootEntity = new UserRole
            {
                Id = new UserRoleId
                {
                    RoleId = userRole.RoleId,
                    UserId = userRole.UserId
                }
            };

            Enqueue(new InsertEntityCommandOperation<UserRole>(RootEntity, dependencies));
        }

    }
}