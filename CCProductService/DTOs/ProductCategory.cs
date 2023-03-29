using CCApiLibrary.Models;

namespace CCProductService.DTOs
{
    public class ProductCategory
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid CategoryPoolId { get; set; }
        public IEnumerable<MultilanguageText> CategoryNames { get; set; }
    }
}
