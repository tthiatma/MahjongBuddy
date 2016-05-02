using System.Data.Common;
using Abp.Zero.EntityFramework;
using MahjongBuddy.Authorization.Roles;
using MahjongBuddy.MultiTenancy;
using MahjongBuddy.Users;

namespace MahjongBuddy.EntityFramework
{
    public class MahjongBuddyDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public MahjongBuddyDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in MahjongBuddyDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of MahjongBuddyDbContext since ABP automatically handles it.
         */
        public MahjongBuddyDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public MahjongBuddyDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
