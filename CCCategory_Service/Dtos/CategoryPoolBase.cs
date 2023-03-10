using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CCCategoryService.Dtos
{
    public class CategoryPool : CategoryPoolBase
    {
        public Guid Id { get; set; }
        public CategoryPool() { }
    }


    public class CategoryPoolBase
    {
        //public Guid Id { get; set; }

        [Required]
        public int Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ParentProductPool { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        public CategoryPoolBase() { }

        public CategoryPoolBase(CategoryPoolBase categoryPool) {
            //Id = categoryPool.Id;
            Key = categoryPool.Key;
            Name = categoryPool.Name;
            Description = categoryPool.Description;
            ParentProductPool = categoryPool.ParentProductPool;
            SystemSettingsId = categoryPool.SystemSettingsId;
        }
    }
}
