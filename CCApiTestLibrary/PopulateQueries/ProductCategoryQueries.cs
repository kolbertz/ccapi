using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public class ProductCategoryQueries
    {
        public static string PopulateSingleCategory(Guid productId, Guid categoryId)
        {
            return $"INSERT INTO ProductCategory(ProductId, CategoryId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{productId}','{categoryId}')";
        }

        //public static string PopulateProductCategoryStringsForSingleCategory(Guid categoryId, string name)
        //{
        //    return $"INSERT INTO CategoryString(Culture, CategoryName, CategoryId) " +
        //        $"VALUES('de-DE', '{name}', '{categoryId}'), " +
        //        $"('en-GB', '{name}', '{categoryId}'), " +
        //        $"('fr-FR', '{name}', '{categoryId}')";
        //}

        public static string DeleteProductCategories()
        {
            return "DELETE FROM ProductCategory";
        }

      
    }
}
