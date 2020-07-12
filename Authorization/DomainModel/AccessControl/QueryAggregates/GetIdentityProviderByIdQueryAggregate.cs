using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class GetIdentityProviderByIdQueryAggregate : GetByIdQueryAggregate<IdentityProvider, int, IdentityProviderOutputDto>
    {
        public GetIdentityProviderByIdQueryAggregate() : this(null)
        {
        }

        public GetIdentityProviderByIdQueryAggregate(HashSet<(string, IEntity)> processedEntities = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()), processedEntities)
        {
            var context = (DomainFramework.DataAccess.RepositoryContext)RepositoryContext;

            IdentityProviderQueryRepository.Register(context);
        }

        public override void PopulateDto()
        {
            OutputDto.IdentityProviderId = RootEntity.Id;

            OutputDto.Name = RootEntity.Name;

            OutputDto.Uri = RootEntity.Uri;
        }

    }
}