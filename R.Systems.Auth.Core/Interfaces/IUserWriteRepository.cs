using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Interfaces
{
    public interface IUserWriteRepository
    {
        Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes);
    }
}
