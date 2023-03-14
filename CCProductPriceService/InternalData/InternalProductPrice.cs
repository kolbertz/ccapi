using CCProductPriceService.DTOs;

namespace CCProductPriceService.InternalData
{
    public partial class InternalProductPrice
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPricePoolId { get; set; }
        public Guid ProductPriceListId { get; set; }
        public int ManualPrice { get; set; }
    
        public InternalProductPrice(ProductPriceBase productPrice) 
        {
            MergeProductPrice(productPrice);
        }

        public void MergeProductPrice(ProductPriceBase productPrice)
        {
            Id= productPrice.Id;
            ProductId= productPrice.ProductId;
            ProductPricePoolId= productPrice.ProductPricePoolId;
            ProductPriceListId= productPrice.ProductPriceListId;
            ManualPrice= productPrice.ManualPrice;
        }
    
    }
}
