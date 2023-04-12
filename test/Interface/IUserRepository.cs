using CCApiLibrary.Models;
using CCUserService.DTOs;

namespace CCUserService.Interface
{
    public interface IUserRepository : IDisposable
    {
        void Init(string database);
        Task<IEnumerable<User>> GetAllUsers(int? take, int? skip, UserClaim userClaim);
    }
}
