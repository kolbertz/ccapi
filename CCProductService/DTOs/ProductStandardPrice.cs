using CCProductService.Data;

namespace CCProductService.DTOs
{
    public class ProductStandardPrice : Product
    {
        public decimal Standardprice { get; set; }

        public ProductStandardPrice(InternalProduct product) : base(product) 
        
        {
           Standardprice = product.Standardprice;
        }
        

    }
}
