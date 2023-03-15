using CCProductPoolService.Data;
using System.ComponentModel.DataAnnotations;

namespace CCProductPoolService.Dtos
{
    public class ProductPoolBase
    {
        [Required]
        public int? Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ParentProductPool { get; set; }

        [Required]
        public Guid? SystemSettingsId { get; set; }

        public ProductPoolBase() { }

        public ProductPoolBase(InternalProductPool productPool)
        {
            Key = productPool.ProductPoolKey;
            Name = productPool.Name;
            Description = productPool.Description;
            ParentProductPool = productPool.ParentProductPoolId;
            SystemSettingsId = productPool.SystemSettingsId;
        }
    }
}
