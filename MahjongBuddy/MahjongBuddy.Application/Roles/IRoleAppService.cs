using System.Threading.Tasks;
using Abp.Application.Services;
using MahjongBuddy.Roles.Dto;

namespace MahjongBuddy.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
    }
}
