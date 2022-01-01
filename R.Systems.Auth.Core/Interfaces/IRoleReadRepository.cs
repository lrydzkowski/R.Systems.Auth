using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Interfaces;

public interface IRoleReadRepository
{
    Task<bool> RoleExistsAsync(long roleId);
}
