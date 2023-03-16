using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public class CategoryPoolQueries
    {
        public static string PopulateSingleCategoryPool(string name, int type)
        {
            return $"INSERT INTO CategoryPool(Name, Description, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, PoolType, SystemSettingsId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{name}', 'Beschreibungstext', GetDate(), '{StaticTestGuids.UserId}', GetDate(), '{StaticTestGuids.UserId}', {type}, '{StaticTestGuids.SystemSettingsId}')";
        }

        public static string DeleteCategoryPools()
        {
            return "DELETE FROM CategoryPool";
        }
    }
}
