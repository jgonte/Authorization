using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class GetRoleByIdQueryAggregate : GetByIdQueryAggregate<Role, int, RoleOutputDto>
    {
        public GetRoleByIdQueryAggregate() : this(null)
        {
        }

        public GetRoleByIdQueryAggregate(HashSet<(string, IEntity)> processedEntities = null) : base(new DomainFramework.DataAccess.RepositoryContext(AuthorizationConnectionClass.GetConnectionName()), processedEntities)
        {
            var context = (DomainFramework.DataAccess.RepositoryContext)RepositoryContext;

            RoleQueryRepository.Register(context);
        }

        public override void PopulateDto()
        {
            OutputDto.RoleId = RootEntity.Id;

            OutputDto.Name = RootEntity.Name;
        }

    }
}