﻿using Abp.MultiTenancy;
using MahjongBuddy.Users;

namespace MahjongBuddy.MultiTenancy
{
    public class Tenant : AbpTenant<Tenant, User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}