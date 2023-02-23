using CCProductPoolService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ProductPoolApiTest.ProductPool
{
    public class ProductPoolStart : IDisposable
    {
        public ProductPoolStart()
        {
            var options = new DbContextOptionsBuilder<AramarkDbProduction20210816Context>()
                .UseSqlServer("Server=tcp:kolbertz.database.windows.net,1433;Initial Catalog=CCServiceApiTestDatabase;Persist Security Info=False;User ID=cc_user;Password=!1cc#2§44ef!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            var dbContext = new AramarkDbProduction20210816Context(options.Options);
            dbContext.Database.Migrate();
        }

        public void Dispose()
        {

        }
    }
}
