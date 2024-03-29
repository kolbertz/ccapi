﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.PopulateQueries
{
    public static class ProductQueries
    {
        public static string PopulateSingleProduct(int key, Guid poolId, int productType = 0)
        {
            return "INSERT INTO Product(ProductKey, IsBlocked, Balance, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, ProductPoolId, ProductType) " +
                "OUTPUT Inserted.Id " +
                $"VALUES({key}, 0, 0, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', '{poolId}', {productType})";
        }

        public static string PopulateProductStringsForSingleProduct(Guid productId, string text)
        {
            return $"INSERT INTO ProductString(Language, ShortName, LongName, Description, ProductId) " +
                $"VALUES('de-DE', '{text}', '{text} Lang', '{text} Beschreibung', '{productId}'), " +
                $"('en-GB', '{text}', '{text} Lang', '{text} Beschreibung', '{productId}'), " +
                $"('fr-FR', '{text}', '{text} Lang', '{text} Beschreibung', '{productId}')";
        }

        public static string DeleteProducts()
        {
            return "DELETE FROM Product";
        }

        public static string DeleteProductStrings()
        {
            return "DELETE FROM ProductString";
        }

        public static string SetProductBarcode(Guid productId, string barcode)
        {
            return $"INSERT INTO ProductBarcode(Barcode, ProductId, Refund) VALUES('{barcode}', '{productId}', 0)";
        }

        public static string DeleteProductBarcode()
        {
            return "DELETE FROM ProductBarcode";
        }
    }
}
