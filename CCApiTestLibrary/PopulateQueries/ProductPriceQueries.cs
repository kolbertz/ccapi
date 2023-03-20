using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public static class ProductPriceQueries
    {
        public static string PopulateSingleProductPriceList(string name, int key, int priority)
        {
            return $"INSERT INTO ProductPriceList([Name], [Key], Priority, SystemSettingsId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{name}', {key}, {priority}, '{StaticTestGuids.SystemSettingsId}')";
        }

        public static string DeleteProductPriceLists()
        {
            return "DELETE FROM ProductPriceList";
        }
    }
}
