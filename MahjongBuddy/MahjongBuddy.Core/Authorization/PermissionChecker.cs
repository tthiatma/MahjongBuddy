using Abp.Authorization;
using MahjongBuddy.Authorization.Roles;
using MahjongBuddy.MultiTenancy;
using MahjongBuddy.Users;

namespace MahjongBuddy.Authorization
{
    public class PermissionChecker : PermissionChecker<Tenant, Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
