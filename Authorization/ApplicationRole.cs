using Authorization.AccessControl;
using System.Threading.Tasks;

namespace Authorization
{
    public static class ApplicationRole
    {
        public static async Task<int> Create(string roleName)
        {
            var aggregate = new CreateRoleCommandAggregate(new CreateRoleInputDto
            {
                Name = roleName
            });

            await aggregate.SaveAsync();

            return aggregate.RootEntity.Id;
        }
    }
}
