using CCProductPoolService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPoolApiTest.Interface
{
    internal interface IPopulateDbControllerTest
    {
        Task PopulateDatabaseWithList(IDbContextTransaction transaction, DbContext ctx);
        Task<Guid> PopulateDatabaseWithSingleEntity(IDbContextTransaction transaction, DbContext ctx);
        Task DePopulateDatabase(IDbContextTransaction transaction, DbContext ctx);
    }
}
