using System.Threading.Tasks;
using Abp.Application.Services;
using MahjongBuddy.Users.Dto;

namespace MahjongBuddy.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(ProhibitPermissionInput input);

        Task RemoveFromRole(long userId, string roleName);
    }
}