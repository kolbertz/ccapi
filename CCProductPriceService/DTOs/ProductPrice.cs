using CCApiLibrary.Models;
using CCProductPriceService.InternalData;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.DTOs
{
    public class ProductPrice : ProductPriceBase
    {

        public ProductPrice(ProductPriceBase productPriceBase) { }

        public ProductPrice(InternalProductPrice internalProductPrice)
        {
            if (internalProductPrice != null)
            {

            }
        }



    }

    public class ProductPriceBase
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public Guid ProductPricePoolId { get; set; }

        public Guid ProductPriceListId { get; set; }
       
        public int ManualPrice { get; set; }

        public ProductPriceBase()
        {
           
        }
    }
}

