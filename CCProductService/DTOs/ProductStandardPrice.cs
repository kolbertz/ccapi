using CCProductService.Data;
using System.Data.SqlTypes;

namespace CCProductService.DTOs
{
    public class ProductStandardPrice : ProductBase
    {
        public decimal Standardprice { get; set; }

        public ProductStandardPrice(InternalProduct product) : base(product) 
        
        {
           Standardprice = product.Standardprice;
        }
        

    }
}
