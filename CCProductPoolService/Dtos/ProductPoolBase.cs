using CCProductPoolService.Data;
using System.ComponentModel.DataAnnotations;

namespace CCProductPoolService.Dtos
{
    public class ProductPoolBase
    {
       // public Guid Id { get; set; }
        [Required]
        public int? Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ParentProductPool { get; set; }

        [Required]
        public Guid? SystemSettingsId { get; set; }

        public ProductPoolBase() { }

        public ProductPoolBase(InternalProductPool internalProductPool)
        {
            if (internalProductPool != null)
            {                
                Key = internalProductPool.ProductPoolKey;
                Name = internalProductPool.Name;
                Description = internalProductPool.Description;
                ParentProductPool = internalProductPool.ParentProductPoolId;
                SystemSettingsId = internalProductPool.SystemSettingsId;
            }
            
        }
    }
}
