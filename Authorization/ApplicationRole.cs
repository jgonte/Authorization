using Authorization.AccessControl;
using System.Threading.Tasks;

namespace Authorization
{
    public static class ApplicationRole
    {
        public static async Task<int> Create(string roleName)
        {
            var roleAggregate = new CreateRoleCommandAggregate(new CreateRoleInputDto
            {
                Name = roleName
            });

            await roleAggregate.SaveAsync();

            return roleAggregate.RootEntity.Id.Value;
        }
    }
}
