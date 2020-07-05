using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class GetUserByUserLoginQueryAggregate : QueryAggregate<User, UserOutputDto>
    {
        public GetAllLinkedAggregateQueryCollectionOperation<int?, Role, RoleOutputDto> GetAllRolesLinkedAggregateQueryOperation { get; set; }

        public GetUserByUserLoginQueryAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            var context = (DomainFramework.DataAccess.RepositoryContext)RepositoryContext;

            UserQueryRepository.Register(context);

            RoleQueryRepository.Register(context);

            User_UserLogins_QueryRepository.Register(context);

            GetAllRolesLinkedAggregateQueryOperation = new GetAllLinkedAggregateQueryCollectionOperation<int?, Role, RoleOutputDto>
            {
                GetAllLinkedEntities = (repository, entity, user) => ((RoleQueryRepository)repository).GetAllRolesForUser(RootEntity.Id).ToList(),
                GetAllLinkedEntitiesAsync = async (repository, entity, user) =>
                {
                    var entities = await ((RoleQueryRepository)repository).GetAllRolesForUserAsync(RootEntity.Id);

                    return entities.ToList();
                },
                CreateLinkedQueryAggregate = entity =>
                {
                    if (entity is Role)
                    {
                        return new GetRoleByIdQueryAggregate();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            };

            QueryOperations.Enqueue(GetAllRolesLinkedAggregateQueryOperation);
        }

        public UserOutputDto Get(string provider, string userKey)
        {
            var repository = (UserQueryRepository)RepositoryContext.GetQueryRepository(typeof(User));

            RootEntity = repository.GetUserByUserLogin(provider, userKey);

            if (RootEntity == null)
            {
                return null;
            }

            LoadLinks(null);

            PopulateDto();

            return OutputDto;
        }

        public async Task<UserOutputDto> GetAsync(string provider, string userKey)
        {
            var repository = (UserQueryRepository)RepositoryContext.GetQueryRepository(typeof(User));

            RootEntity = await repository.GetUserByUserLoginAsync(provider, userKey);

            if (RootEntity == null)
            {
                return null;
            }

            await LoadLinksAsync(null);

            PopulateDto();

            return OutputDto;
        }

        public override void PopulateDto()
        {
            OutputDto.UserId = RootEntity.Id.Value;

            OutputDto.Email = RootEntity.Email;

            OutputDto.Roles = GetAllRolesLinkedAggregateQueryOperation.OutputDtos;
        }

    }
}