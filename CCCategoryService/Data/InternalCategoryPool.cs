
using CCCategoryService.Dtos;

namespace CCCategoryService.Data;

public class InternalCategoryPool
{
    public Guid Id { get; set; }

    public string Name { get; set; }    

    public string Description { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTimeOffset LastUpdatedDate { get; set; }

    public Guid LastUpdatedUser { get; set; }

    public Guid? ParentCategoryPoolId { get; set; }

    public int PoolType { get; set; }

    public Guid SystemSettingsId { get; set; }

    public InternalCategoryPool() { }

    public InternalCategoryPool(CategoryPoolBase categoryPoolBase)
    {
        Name = categoryPoolBase.Names?.Select(x => x.Text).FirstOrDefault();
        Description = categoryPoolBase.Descriptions?.Select(x => x.Text).FirstOrDefault();
        ParentCategoryPoolId = categoryPoolBase.ParentCategoryPool;
        PoolType = categoryPoolBase.Type.Value;
        SystemSettingsId = categoryPoolBase.SystemSettingsId.Value;
    }

    public InternalCategoryPool(CategoryPool categoryPool)
    {
        Id = categoryPool.Id;
        Name = categoryPool.Names?.Select(x => x.Text).FirstOrDefault();
        Description = categoryPool.Descriptions?.Select(x => x.Text).FirstOrDefault();
        ParentCategoryPoolId = categoryPool.ParentCategoryPool;
        PoolType = categoryPool.Type.Value;
        SystemSettingsId = categoryPool.SystemSettingsId.Value;
    }

}
