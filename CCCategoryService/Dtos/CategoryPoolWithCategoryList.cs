using CCApiLibrary.Models;
using System.Xml.Linq;

namespace CCCategoryService.Dtos
{
    public class CategoryPoolWithCategoryList
    {
        public Guid CategoryPoolId { get; set; }
        public string CategoryPoolName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryNames { get; set; }
              
    }

}
