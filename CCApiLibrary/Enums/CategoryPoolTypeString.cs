using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CCApiLibrary.Enums
{
    public class CategoryPoolTypeString
    {
        public string GetCategoryPoolTypes(int index)
        {
            switch (index)
            {
                case 1:
                    return "Categories";
                case 2:
                    return "Tags";
                case 3:
                    return "Taxes";
                case 4:
                    return "Allergens";
                case 5:
                    return "Additives";
                default:
                    return "";
            }
        }

        public string ConvertMethod(CategoryPoolType index)
        {
            
            switch (index)
            {
                case CategoryPoolType.PoolTypeCategory:
                    return CategoryPoolType.PoolTypeCategory.ToString();
                case CategoryPoolType.PoolTypeTags:
                    return CategoryPoolType.PoolTypeTags.ToString();
                case CategoryPoolType.PoolTypeTax:
                    return CategoryPoolType.PoolTypeTax.ToString();
                case CategoryPoolType.PoolTypeMenuPlan:
                    return CategoryPoolType.PoolTypeMenuPlan.ToString();               
                default:
                    return "";
            }
        }
    }
}

