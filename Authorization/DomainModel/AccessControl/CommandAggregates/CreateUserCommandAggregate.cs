using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authorization.AccessControl
{
    public class CreateUserCommandAggregate : CommandAggregate<User>
    {
        public CreateUserCommandAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
        }

        public CreateUserCommandAggregate(CreateUserInputDto user, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(user, dependencies);
        }

        public override void Initialize(IInputDataTransferObject user, EntityDependency[] dependencies)
        {
            Initialize((CreateUserInputDto)user, dependencies);
        }

        private void Initialize(CreateUserInputDto user, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<User>(() => new UserCommandRepository());

            RegisterCommandRepositoryFactory<User_UserLogins_CommandRepository.RepositoryKey>(() => new User_UserLogins_CommandRepository());

            RootEntity = new User
            {
                Email = user.Email,
                NormalizedEmail = user.Email.ToUpperInvariant()
            };

            Enqueue(new InsertEntityCommandOperation<User>(RootEntity, dependencies));

            foreach (var dto in user.UserLogins)
            {
                var userLoginValueObject = new UserLogin
                {
                    Provider = dto.Provider,
                    UserKey = dto.UserKey
                };

                Enqueue(new AddLinkedValueObjectCommandOperation<User, UserLogin, User_UserLogins_CommandRepository.RepositoryKey>(RootEntity, userLoginValueObject));
            }
        }

    }
}