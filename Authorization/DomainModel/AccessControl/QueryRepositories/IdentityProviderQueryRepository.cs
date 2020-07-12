using DataAccess;
using DomainFramework.Core;
using DomainFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.AccessControl
{
    public class IdentityProviderQueryRepository : EntityQueryRepository<IdentityProvider, int>
    {
        public override IdentityProvider GetById(int identityProviderId)
        {
            var result = Query<IdentityProvider>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pIdentityProvider_GetById]")
                .Parameters(
                    p => p.Name("identityProviderId").Value(identityProviderId)
                )
                .Execute();

            return result.Data;
        }

        public async override Task<IdentityProvider> GetByIdAsync(int identityProviderId)
        {
            var result = await Query<IdentityProvider>
                .Single()
                .Connection(AuthorizationConnectionClass.GetConnectionName())
                .StoredProcedure("[AccessControl].[pIdentityProvider_GetById]")
                .Parameters(
                    p => p.Name("identityProviderId").Value(identityProviderId)
                )
                .ExecuteAsync();

            return result.Data;
        }

        public static void Register(DomainFramework.DataAccess.RepositoryContext context)
        {
            context.RegisterQueryRepository<IdentityProvider>(new IdentityProviderQueryRepository());
        }

    }
}