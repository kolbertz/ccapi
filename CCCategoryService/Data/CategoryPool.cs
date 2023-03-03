using CCCategoryService.Dtos;

namespace CCCategoryService.Data
{
    public partial class CategoryPool
    {
        public Guid Id { get; set; }

        public string Name { get; set; }    

        public string Description { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid CreatedUser { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid LastUpdatedUser { get; set; }

        public Guid ParentCategoryPoolId { get; set; }

        public int PoolType { get; set; }

        public Guid SystemSettingsId { get; set; }

        //public CategoryPool(CategoryPoolDto categoryPoolDto)
        //{
        //    MergeCategoryPool(categoryPoolDto);
        //}

        //public void MergeProductPool(CategoryPoolDto productPoolDto)
        //{
        //    Id = categoryPoolDto.Id;
            
        //    Name = categoryPoolDto.Name;
        //    Description = categoryPoolDto.Description;
        //    ParentPoolId = categoryPoolDto.ParentProductPool;
        //    SystemSettingsId = categoryPoolDto.SystemSettingsId;
        //}
    }
}
