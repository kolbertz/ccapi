using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public class CategoryQueries
    {
        public static string PopulateSingleCategory(int key, Guid categoryPoolId)
        {
            return $"INSERT INTO Category(CategoryKey, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, CategoryPoolId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES({key}, GetDate(), '{StaticTestGuids.UserId}', GetDate(), '{StaticTestGuids.UserId}', '{categoryPoolId}')";
        }

        public static string PopulateCategoryStringsForSingleCategory(Guid categoryId, string name)
        {
            return $"INSERT INTO CategoryString(Culture, CategoryName, CategoryId) " +
                $"VALUES('de-DE', '{name}', '{categoryId}'), " +
                $"('en-GB', '{name}', '{categoryId}'), " +
                $"('fr-FR', '{name}', '{categoryId}')";
        }

        public static string DeleteCategories()
        {
            return "DELETE FROM Category";
        }

        public static string DeleteCategoryStrings()
        {
            return "DELETE FROM CategoryString";
        }
    }
}
