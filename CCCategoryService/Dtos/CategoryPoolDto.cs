using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CCCategoryService.Dtos
{
    public class CategoryPoolDto
    {
        public Guid Id { get; set; }

        [Required]
        public int Key { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ParentProductPool { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        public CategoryPoolDto() { }

        public CategoryPoolDto(CategoryPoolDto categoryPool) {
            //Id = productPool.Id;
            //Key = productPool.ProductPoolKey;
            //Name = productPool.Name;
            //Description = productPool.Description;
            //ParentProductPool = productPool.ParentProductPoolId;
            //SystemSettingsId = productPool.SystemSettingsId;
        }
    }
}
