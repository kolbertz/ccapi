using CCProductService.Data;
using System.Data.SqlTypes;

namespace CCProductService.DTOs
{
    public class ProductStandardPrice : ProductDto
    {
        public decimal Standardprice { get; set; }

        public ProductStandardPrice(Product product) : base(product) { }
        

    }
}
