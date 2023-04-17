using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Models;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CCCategoryService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
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

            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                IEnumerable<Category> categorysList = null;
                categoryRepository.Init(userClaim.TenantDatabase);
                categorysList = await categoryRepository.GetAllCategorys(take, skip, userClaim).ConfigureAwait(false);
                return Ok(categorysList);
            }

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
                Category category = await categoryRepository.GetCategoryById(id, userClaim).ConfigureAwait(false);
                if (category != null)
                {
                    return Ok(category);
                }
                else
                {
                    return NotFound();
                }
            }
            
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Adds a new Category (using EF Core)")]
        public async Task<IActionResult> Post([ModelBinder] CategoryBase categoryDto)
        {
            UserClaim userClaim = null;
            Guid? newCategoryId = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                categoryRepository.Init(userClaim.TenantDatabase);
                newCategoryId = await categoryRepository.AddCategoryAsync(categoryDto, userClaim).ConfigureAwait(false);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{newCategoryId}"), null);
            }

        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Updates a Category (using EF Core)")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Put(Guid id, [ModelBinder] Category categoryDto)

        {
            if (id != categoryDto.Id)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                categoryRepository.Init(userClaim.TenantDatabase);
                if (await categoryRepository.UpdateCategoryAsync(categoryDto, userClaim).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a Category not using Microsoft.AspNetCore.JsonPatch. See https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 ")]
        public async Task<IActionResult> Patch(Guid id)
        {
            CategoryBase dto;
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                categoryRepository.Init(userClaim.TenantDatabase);
                dto = await categoryRepository.PatchCategoryAsync(id, userClaim).ConfigureAwait(false);
                return Ok(dto);

            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        [SwaggerOperation("Deletes a Category ")]
        public async Task<IActionResult> Delete(Guid id)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);

            }
            using (ICategoryRepository categoryRepository = _serviceProvider.GetService<ICategoryRepository>())
            {
                categoryRepository.Init(userClaim.TenantDatabase);

                if (await categoryRepository.DeleteCategoryAsync(id, userClaim).ConfigureAwait(false)>0)
                {
                    return NoContent();
                }
                else { return NotFound(); }
            }
        }
    }
}