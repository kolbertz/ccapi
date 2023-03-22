using CCProductPoolService.Dtos;
using Dapper.Contrib.Extensions;

namespace CCProductPoolService.Data;

[Table("ProductPool")]
public partial class InternalProductPool
{
    [Key]
    public Guid Id { get; set; }

    public int ProductPoolKey { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTimeOffset LastUpdatedDate { get; set; }

    public Guid LastUpdatedUser { get; set; }

    public Guid? ParentProductPoolId { get; set; }

    public Guid SystemSettingsId { get; set; }

    public InternalProductPool(ProductPool productPoolDto)
    {
        MergeProductPool(productPoolDto);
    }

    public InternalProductPool(ProductPoolBase productPool) 
    {        
        ProductPoolKey = productPool.Key.Value;
        Name = productPool.Name;
        Description = productPool.Description;
        ParentProductPoolId = productPool.ParentProductPool;
        SystemSettingsId = productPool.SystemSettingsId.Value;
    }

    public void MergeProductPool(ProductPool productPoolDto)
    {
        Id = productPoolDto.Id;
        ProductPoolKey = productPoolDto.Key.Value;
        Name = productPoolDto.Name;
        Description = productPoolDto.Description;
        ParentProductPoolId = productPoolDto.ParentProductPool;
        SystemSettingsId = productPoolDto.SystemSettingsId.Value;
    }
}
