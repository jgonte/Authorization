using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class GetUserByNormalizedEmailQueryAggregate : QueryAggregate<User, UserOutputDto>
    {
        public GetUserByNormalizedEmailQueryAggregate() : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()))
        {
            var context = (DomainFramework.DataAccess.RepositoryContext)RepositoryContext;

            UserQueryRepository.Register(context);

            User_UserLogins_QueryRepository.Register(context);
        }

        public UserOutputDto Get(string email)
        {
            var repository = (UserQueryRepository)RepositoryContext.GetQueryRepository(typeof(User));

            RootEntity = repository.GetUserByNormalizedEmail(email);

            if (RootEntity == null)
            {
                return null;
            }

            LoadLinks(null);

            PopulateDto();

            return OutputDto;
        }

        public async Task<UserOutputDto> GetAsync(string email)
        {
            var repository = (UserQueryRepository)RepositoryContext.GetQueryRepository(typeof(User));

            RootEntity = await repository.GetUserByNormalizedEmailAsync(email);

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
        }

    }
}