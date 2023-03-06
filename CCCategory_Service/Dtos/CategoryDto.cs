using CCCategoryService.Data;
using CCCategoryService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace CCCategoryService.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryKey { get; set; }

        public List<MultilanguageText> Description { get; set; }

        [Required]
        public Guid CategoryPoolId { get; set; }

        public CategoryDto() { }

        public CategoryDto(Category category)
        {
            if (category != null)
            {
                Id = category.Id;
                CategoryPoolId = category.CategoryPoolId;
                CategoryKey = category.CategoryKey;
                Description = new List<MultilanguageText>();              
                
            }
        }

       
    }
}
