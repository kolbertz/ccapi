using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public static class ProductQueries
    {
        public static string PopulateSingleProduct()
        {
            return "INSERT INTO Product(ProductKey, IsBlocked, Balance, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, ProductPoolId, ProductType) " +
                "OUTPUT Inserted.Id " +
                "VALUES(1, false, false, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', 0)";
        }

        public static string PopulateProductList()
        {
            return "INSERT INTO Product(ProductKey, IsBlocked, Balance, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, ProductPoolId, ProductType) " +
                "VALUES(1, false, false, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', 0), " +
                      "(2, false, false, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', 0);";
        }
    }
}
