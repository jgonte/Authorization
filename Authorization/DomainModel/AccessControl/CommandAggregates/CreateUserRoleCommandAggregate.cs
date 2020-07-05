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

        public CreateUserRoleCommandAggregate(UserRoleInputDto userRole, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(userRole, dependencies);
        }

        public override void Initialize(IInputDataTransferObject userRole, EntityDependency[] dependencies)
        {
            Initialize((UserRoleInputDto)userRole, dependencies);
        }

        private void Initialize(UserRoleInputDto userRole, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<UserRole>(() => new UserRoleCommandRepository());

            RootEntity = new UserRole
            {
                Id = new UserRoleId
                {
                    UserId = userRole.UserId,
                    RolesId = userRole.RolesId
                }
            };

            Enqueue(new InsertEntityCommandOperation<UserRole>(RootEntity, dependencies));
        }

    }
}