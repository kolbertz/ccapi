﻿using CCApiLibrary.Models;
using CCCategoryService.Data;
using System.ComponentModel.DataAnnotations;

namespace CCCategoryService.Dtos
{
    public class Category : CategoryBase
    {
        public Guid Id { get; set; }

        public Category() { }
        
        public Category(InternalCategory intcategory)
            :base(intcategory)
        {            
            Id = intcategory.Id;
        }

        
    }

    public class CategoryBase
    {        
        [Required]
        public List<MultilanguageText> CategoryNames { get; set; }

        [Required]
        public int? CategoryKey { get; set; }

        public List<MultilanguageText> Descriptions { get; set; }

        public List<MultilanguageText> Comments { get; set; }

        [Required]
        public Guid? CategoryPoolId { get; set; }


        public CategoryBase()  
        {
            CategoryNames = new List<MultilanguageText>();
            Descriptions = new List<MultilanguageText>();
            Comments = new List<MultilanguageText>();           
        }
        public CategoryBase(InternalCategory internalCategory) :base()
        {
            if (internalCategory != null)
            {
                CategoryPoolId= internalCategory.CategoryPoolId;
                CategoryKey= internalCategory.CategoryKey;
               
            }          
        }

        public CategoryBase(Category category)
            : base()
        {
            if (category != null)
            {
                CategoryPoolId = category.CategoryPoolId;
                CategoryKey = category.CategoryKey;
            }
        }
        public void SetMultilanguageText(InternalCategoryString categoryString)
        {            
            CategoryNames.Add(new MultilanguageText { Culture = categoryString.Culture, Text = categoryString.CategoryName });
            Descriptions.Add(new MultilanguageText { Culture = categoryString.Culture, Text = categoryString.Description });
            Comments.Add(new MultilanguageText { Culture = categoryString.Culture, Text = categoryString.Comment });
        }

    }
}
