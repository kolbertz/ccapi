using CCCategoryService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CCCategoryService.Controllers
{
    public class CategoryController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public CategoryController(IServiceProvider serviceProvider)
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