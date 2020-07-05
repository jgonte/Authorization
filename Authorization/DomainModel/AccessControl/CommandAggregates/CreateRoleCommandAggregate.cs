using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authorization.AccessControl
{
    public class CreateRoleCommandAggregate : CommandAggregate<Role>
    {
        public CreateRoleCommandAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
        }

        public CreateRoleCommandAggregate(RoleInputDto role, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(role, dependencies);
        }

        public override void Initialize(IInputDataTransferObject role, EntityDependency[] dependencies)
        {
            Initialize((RoleInputDto)role, dependencies);
        }

        private void Initialize(RoleInputDto role, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<Role>(() => new RoleCommandRepository());

            RootEntity = new Role
            {
                Name = role.Name
            };

            Enqueue(new InsertEntityCommandOperation<Role>(RootEntity, dependencies));
        }

    }
}