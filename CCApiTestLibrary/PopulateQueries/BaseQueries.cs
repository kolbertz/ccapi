using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public static class BaseQueries
    {
        public static string PopulateSystemSettingsQuery(Guid systemSettingsId)
        {
            return "INSERT INTO SystemSettings (Id, InternalName, [Name], DistributorId, IsBlocked, IsHosted, SystemType, AddressName1, " +
                "AddressStreet, AddressPostalCode, AddressCity, DefaultTimeZone, MaxCustomCurrencyExchangeRateDiff, MinPriceUnit, NoDeleteRange, " +
                "CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, [Key]) " +
                $"VALUES('{systemSettingsId}', 'TestSystem', 'TestSystem', 0, 0, 0, 0, 'Test', 'Test', 'Test', 'Test', 'Test', 0, 0, 0, GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', 0);";
        }

        public static string DeleteSystemSettingsQuery()
        {
            return "DELETE FROM SystemSettings";
        }
    }
}
