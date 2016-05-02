using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using MahjongBuddy.EntityFramework;

namespace MahjongBuddy
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(MahjongBuddyCoreModule))]
    public class MahjongBuddyDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
