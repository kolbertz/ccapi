using CCProductService.Data;

namespace CCProductService.Interface
{
    public interface IClaimsRepository
    {
        Task GetProfileId(UserClaim userClaim);
        Task GetProductPoolIds(UserClaim userClaim);
    }
}
