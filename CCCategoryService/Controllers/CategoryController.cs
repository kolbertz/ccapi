using CCCategoryService.Data;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using CCCategoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;

namespace CCCategoryService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public CategoryController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get a list with "<see cref="CategoryDto"/>" items (using Dapper)
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get a list with Category items (using Dapper)")]
        public async Task<IActionResult> Get(int? skip, int? take)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            
            return Ok(userClaim);
        }

        /// <summary>
        /// Gets a "<see cref="CategoryDto"/>" by Id (using Dapper)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a Category by Id (using Dapper)")]
        public async Task<IActionResult> Get(Guid id)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            { 
                userClaim = new UserClaim(HttpContext.User.Claims); 
            }

            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                categoryRepository.Init(userClaim.TenantDatabase);
                CategoryDto category = await categoryRepository.GetCategoryById(id, userClaim).ConfigureAwait(false);
            }
            return Ok();
        }
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