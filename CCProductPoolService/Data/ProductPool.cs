using CCProductPoolService.Dtos;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace CCProductPoolService.Data;

[Table("ProductPool")]
public partial class ProductPool
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

    public virtual ICollection<ProductPool> InverseParentProductPool { get; } = new List<ProductPool>();

    public virtual ProductPool ParentProductPool { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();

    public ProductPool(ProductPoolDto productPoolDto)
    {
        MergeProductPool(productPoolDto);
    }

    public ProductPool() { }

    public void MergeProductPool(ProductPoolDto productPoolDto)
    {
        Id = productPoolDto.Id;
        ProductPoolKey = productPoolDto.Key;
        Name = productPoolDto.Name;
        Description = productPoolDto.Description;
        ParentProductPoolId = productPoolDto.ParentProductPool;
        SystemSettingsId = productPoolDto.SystemSettingsId;
    }
}
