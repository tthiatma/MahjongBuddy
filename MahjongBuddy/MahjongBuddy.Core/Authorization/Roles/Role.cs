using Abp.Authorization.Roles;
using MahjongBuddy.MultiTenancy;
using MahjongBuddy.Users;

namespace MahjongBuddy.Authorization.Roles
{
    public class Role : AbpRole<Tenant, User>
    {

    }
}