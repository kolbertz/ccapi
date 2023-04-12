using CCApiLibrary.Models;
using Newtonsoft.Json;

namespace CCProductService.DTOs
{
    public class Allergens
    {
        public string CategoryPoolId { get; set; }

        List<MultilanguageText> CatPoolNames { get; set; }

        List<ProductCategory> ProductCategories { get; set; }

        private int PoolType {get; set;}
    }
}
