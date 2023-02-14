using CCProductPoolService.Data;
using CCProductPoolService.Interface;
using System.Data;

namespace ProductPoolApiTest.Interface
{
    internal interface IPopulateDbControllerTest
    {
        Task PopulateDatabaseWithList(IApplicationDbConnection ctx);
        Task<Guid> PopulateDatabaseWithSingleEntity(IApplicationDbConnection ctx);
        Task DePopulateDatabase(IApplicationDbConnection ctx);
    }
}
