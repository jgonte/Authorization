using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authorization.AccessControl
{
    public class AddUserLoginCommandAggregate : CommandAggregate<User>
    {
        public AddUserLoginCommandAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
        }

        public AddUserLoginCommandAggregate(AddUserLoginsInputDto user, EntityDependency[] dependencies = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            Initialize(user, dependencies);
        }

        public override void Initialize(IInputDataTransferObject user, EntityDependency[] dependencies)
        {
            Initialize((AddUserLoginsInputDto)user, dependencies);
        }

        private void Initialize(AddUserLoginsInputDto user, EntityDependency[] dependencies)
        {
            RegisterCommandRepositoryFactory<User>(() => new UserCommandRepository());

            RegisterCommandRepositoryFactory<User_UserLogins_CommandRepository.RepositoryKey>(() => new User_UserLogins_CommandRepository());

            RootEntity = new User
            {
                Id = user.UserId
            };

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