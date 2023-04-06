using CCApiLibrary.Models;

namespace CCProductService.DTOs
{
    public class Taxes
    {
        public string CategoryPoolId { get; set; }

        List<MultilanguageText> CatPoolNames { get; set; }

        List<ProductCategory> ProductCategories { get; set; }

        public int PoolType { get; set; }
    }
}
