using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace MahjongBuddy
{
    [DependsOn(typeof(MahjongBuddyCoreModule), typeof(AbpAutoMapperModule))]
    public class MahjongBuddyApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
