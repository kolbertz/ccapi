namespace CCProductService.DTOs
{
    public class ProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public IEnumerable<MultilanguageText> CategoryNames { get; set; }
    }
}
