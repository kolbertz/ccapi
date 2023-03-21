using CCApiLibrary.Models;

namespace CCProductService.DTOs
{
    public class ProductCategory
    {
        public Guid CategoryId { get; set; }
        public IEnumerable<MultilanguageText> CategoryNames { get; set; }
    }
}
