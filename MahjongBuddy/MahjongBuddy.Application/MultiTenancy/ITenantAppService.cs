using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MahjongBuddy.MultiTenancy.Dto;

namespace MahjongBuddy.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        ListResultOutput<TenantListDto> GetTenants();

        Task CreateTenant(CreateTenantInput input);
    }
}
