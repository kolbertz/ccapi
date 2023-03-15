using CCProductPoolService.Data;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CCProductPoolService.Dtos
{
    public class ProductPool : ProductPoolBase
    {
        public Guid Id { get; set; }

        public ProductPool() :base() { }

        public ProductPool(InternalProductPool productPool) 
            :base(productPool)
        {
            Id = productPool.Id;
        }
    }
}
