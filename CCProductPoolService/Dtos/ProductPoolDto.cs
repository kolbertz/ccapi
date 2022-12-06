using CCProductPoolService.Data;

namespace CCProductPoolService.Dtos
{
    public class ProductPoolDto
    {
        public Guid Id { get; set; }
        public int Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid? ParentProductPool { get; set; }

        public Guid SystemSettingsId { get; set; }

        public ProductPoolDto() { }

        public ProductPoolDto(ProductPool productPool) {
            Id = productPool.Id;
            Key = productPool.ProductPoolKey;
            Name = productPool.Name;
            Description = productPool.Description;
            ParentProductPool = productPool.ParentProductPoolId;
            SystemSettingsId = productPool.SystemSettingsId;
        }
    }
}
