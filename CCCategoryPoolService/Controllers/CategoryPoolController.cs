using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CCCategoryPoolService.Controllers
{
    public class CategoryPoolController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public CategoryPoolController(IServiceProvider serviceProvider)
        { 
            _serviceProvider= serviceProvider;
        }

        public async Task Get()
        { }

        public async Task Get(Guid id)
        { }
        public async Task Post(Guid id)
        { }
        public async Task Put(Guid id)
        { }
        public async Task Patch(Guid id)
        { }
        public async Task Delete(Guid id)
        { }
    }
}