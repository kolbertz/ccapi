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

    public InternalProductPool()
    {

    }
    public InternalProductPool(ProductPool productPoolDto)
    {
        MergeProductPool(productPoolDto);
    }

    public InternalProductPool(ProductPoolBase productPool)
    {
        ProductPoolKey = productPool.Key.Value;
        if (productPool.Names != null && productPool.Names.Count > 0)
        {
            Name = productPool.Names.First().Text;
        }
        if (productPool.Descriptions != null && productPool.Descriptions.Count > 0)
        {
            Description = productPool.Descriptions.First().Text;
        }
        ParentProductPoolId = productPool.ParentProductPool;
        SystemSettingsId = productPool.SystemSettingsId.Value;
    }

    public void MergeProductPool(ProductPool productPoolDto)
    {
        Id = productPoolDto.Id;
        ProductPoolKey = productPoolDto.Key.Value;

        if (productPoolDto.Names != null && productPoolDto.Names.Count > 0)
        {
            Name = productPoolDto.Names.First().Text;
        }
        if (productPoolDto.Descriptions != null && productPoolDto.Descriptions.Count > 0)
        {
            Description = productPoolDto.Descriptions.First().Text;
        }
        ParentProductPoolId = productPoolDto.ParentProductPool;
        SystemSettingsId = productPoolDto.SystemSettingsId.Value;
    }
}
