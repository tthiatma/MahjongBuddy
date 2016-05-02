using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using MahjongBuddy.Authorization.Roles;
using MahjongBuddy.Editions;
using MahjongBuddy.Users;

namespace MahjongBuddy.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager
            )
        {
        }
    }
}