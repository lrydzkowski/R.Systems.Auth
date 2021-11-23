using System.Threading.Tasks;

namespace R.Systems.Auth.SharedKernel.Interfaces
{
    public interface IGenericWriteRepository<T> where T : class
    {
        public Task<bool> EditAsync(T entity);

        public Task<bool> DeleteAsync(T entity);
    }
}
