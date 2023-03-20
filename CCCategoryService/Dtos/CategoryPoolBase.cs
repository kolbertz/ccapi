using CCApiLibrary.Models;
using CCCategoryService.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace CCCategoryService.Dtos
{
    public class CategoryPool : CategoryPoolBase
    {
        public Guid Id { get; set; }
        public CategoryPool() { }

        public CategoryPool(InternalCategoryPool internalPool, string description)
        {
            Id = internalPool.Id;
            Names = new List<MultilanguageText>
            {
                new MultilanguageText("de-DE", internalPool.Name)
            };
            Descriptions = new List<MultilanguageText>
            {
                new MultilanguageText ("de-DE", description)
            };
            ParentCategoryPool = internalPool.ParentCategoryPoolId;
            Type = internalPool.PoolType;
            this.SystemSettingsId = SystemSettingsId;
        }

        public CategoryPool(InternalCategoryPool internalPool)
        {
            Id = internalPool.Id;
            Names = new List<MultilanguageText>
            {
                new MultilanguageText("de-DE", internalPool.Name)
            };
            Descriptions = new List<MultilanguageText>
            {
                new MultilanguageText ("de-DE", internalPool.Description)
            };
            ParentCategoryPool = internalPool.ParentCategoryPoolId;
            Type = internalPool.PoolType;
            this.SystemSettingsId = SystemSettingsId;
        }
    }


    public class CategoryPoolBase
    {
        [Required]
        public int? Type { get; set; }

        [Required]
        public List<MultilanguageText> Names { get; set; }

        public List<MultilanguageText> Descriptions { get; set; }

        public Guid? ParentCategoryPool { get; set; }

        [Required]
        public Guid? SystemSettingsId { get; set; }

        public CategoryPoolBase() { }
    }
}
