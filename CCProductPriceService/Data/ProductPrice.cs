

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCProductPriceService.Data
{
    [Table("ProductPrice")]
    public partial class ProductPrice
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductPricePoolId { get; set; }

        public Guid ProductPriceListId { get; set; }

        public int ManualPrice { get; set; }

        public ProductPrice() { }
    }

}
