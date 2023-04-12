using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CCCategoryService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryPoolController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public CategoryPoolController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        [SwaggerOperation("Get a list with CategoryPool items (using Dapper)")]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())                
                {
                    categoryPoolRepository.Init(userClaim.TenantDatabase);
                    return Ok(await _serviceProvider.GetService<ICategoryPoolRepository>().GetCategoryPoolsAsync());
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a CategoryPool by Id (using Dapper)")]
        public async Task<IActionResult> Get(Guid id)
        {
            UserClaim userClaim = null;

            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())
            {                
                categoryPoolRepository.Init(userClaim.TenantDatabase);
                CategoryPoolBase categoryPoolDto = await categoryPoolRepository.GetCategoryPoolByIdAsync(id, userClaim).ConfigureAwait(false);
                if (categoryPoolDto != null)
                {
                    return Ok(categoryPoolDto);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpGet]
        [Route("{id}/moreInfo")]
        [SwaggerOperation("Gets a CategoryPool by Id with more Info")]
        public async Task<IActionResult> GetMore(Guid id)
        {
            UserClaim userClaim = null;

            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())
            {
                categoryPoolRepository.Init(userClaim.TenantDatabase);
                CategoryPoolWithCategoryList categoryPoolDto = await categoryPoolRepository.GetCategoryPoolAsyncMoreInfo(id, userClaim).ConfigureAwait(false);
                if (categoryPoolDto != null)
                {
                    return Ok(categoryPoolDto);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Adds a new CategoryPool ")]
        public async Task<IActionResult> Post(CategoryPoolBase categoryPoolDto)
        {
            try
            {
                Guid? categorypoolId = null;
                UserClaim userClaim = null;

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())
                {
                    categoryPoolRepository.Init(userClaim.TenantDatabase);
                    categorypoolId = await categoryPoolRepository.AddCategoryPoolAsync(categoryPoolDto, userClaim);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{categorypoolId}"), null);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Updates a CategoryPool)")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Put(Guid id, CategoryPool categoryPoolDto)
        {
            try
            {
                if (id != categoryPoolDto.Id)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)

                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())
                {
                    categoryPoolRepository.Init(userClaim.TenantDatabase);
                    if (await categoryPoolRepository.UpdateCategoryPoolAsync(categoryPoolDto, userClaim).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a CategoryPool not using Microsoft.AspNetCore.JsonPatch. See https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 ")]

        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument categoryPoolPatch)
        {
            try
            {
                CategoryPoolBase categoryPoolDto = null;
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                categoryPoolDto = await _serviceProvider.GetService<ICategoryPoolRepository>().PatchCategoryPoolAsync(id, categoryPoolPatch, userClaim).ConfigureAwait(false);
                if (categoryPoolDto != null)
                {
                    return Ok(categoryPoolDto);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        [SwaggerOperation("Deletes a CategoryPool ")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>())

                {
                    categoryPoolRepository.Init(userClaim.TenantDatabase);
                    if (await categoryPoolRepository.DeleteCategoryPoolAsync(id).ConfigureAwait(false) > 0)

                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
    }
}