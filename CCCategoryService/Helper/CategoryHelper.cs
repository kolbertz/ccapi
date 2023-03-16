using CCApiLibrary.Models;
using CCCategoryService.Data;
using CCCategoryService.Dtos;
using InternalCategory = CCCategoryService.Data.InternalCategory;

namespace CCCategoryService.Helper
{
    public class CategoryHelper
    {
        public static void ParseCategoryToDto(InternalCategory category, CategoryBase categoryBase)
        {
            if (category != null)
            {
                if (categoryBase == null)
                {
                    categoryBase = new CategoryBase();
                }
                //categoryBase.CategoryId = category.Id;
                categoryBase.CategoryPoolId = category.CategoryPoolId;
                categoryBase.CategoryKey = category.CategoryKey;
                categoryBase.CategoryNames = category.CategoryStrings?.Select(x => new MultilanguageText { Culture = x.Culture, Text = x.CategoryName }).ToList();
                categoryBase.Comments = category.CategoryStrings?.Select(x => new MultilanguageText { Culture = x.Culture, Text = x.Comment }).ToList();
                categoryBase.Descriptions = category.CategoryStrings?.Select(x => new MultilanguageText { Culture = x.Culture, Text = x.Description }).ToList();
               
            }

        }

        public static void ParseDtoToCategory(CategoryBase CategoryDto, InternalCategory category)
        {
            if (CategoryDto != null)
            {
                if (category == null)
                {
                    category = new InternalCategory();
                }
                
                category.CategoryPoolId = CategoryDto.CategoryPoolId.Value;
                category.CategoryKey = CategoryDto.CategoryKey.Value;



                List<string> cultures = CategoryDto.CategoryNames.Select(sn => sn.Culture).ToList();
                cultures.AddRange(CategoryDto.Comments.Where(ln => !cultures.Contains(ln.Culture)).Select(ln => ln.Culture));
                cultures.AddRange(CategoryDto.Descriptions.Where(ld => !cultures.Contains(ld.Culture)).Select(ld => ld.Culture));
                if (cultures != null && cultures.Count > 0)
                {
                    category.CategoryStrings.Clear();
                    foreach (string culture in cultures)
                    {
                        category.CategoryStrings.Add(new InternalCategoryString
                        {
                            CategoryId = category.Id,
                            Culture = culture,
                            CategoryName = CategoryDto.CategoryNames.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            Comment = CategoryDto.Comments.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            Description = CategoryDto.Descriptions.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault()
                        });
                    }
                }
            }
        }

        public static void ParsewithId(CategoryBase categoryBase) 
        
        { 
                   
        
        }
    }
}
