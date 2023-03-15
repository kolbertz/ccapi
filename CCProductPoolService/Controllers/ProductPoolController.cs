using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Models;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CCProductPoolService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductPoolController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductPoolController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    return Ok(await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolsAsync());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            UserClaim userClaim = null;

            try
            {
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    ProductPool productPoolDto = await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolByIdAsync(id);
                    if (productPoolDto == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(productPoolDto);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Adds a new ProductPool")]
        public async Task<IActionResult> Post(ProductPoolBase productPoolDto)
        {
            try
            {

                Guid? poolId = null;
                UserClaim userClaim = null;

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    poolId = await _serviceProvider.GetService<IProductPoolRepository>().AddProductPoolAsync(productPoolDto, userClaim);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{poolId}"), null);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Updates a Product (using EF Core)")]
        public async Task<IActionResult> Put(Guid id, ProductPool productPoolDto)
        {
            try
            {
                if (id != productPoolDto.Id)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    await _serviceProvider.GetService<IProductPoolRepository>().UpdateProductPoolAsync(productPoolDto, userClaim);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a ProductPool not using Microsoft.AspNetCore.JsonPatch. See https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 (using EF Core)")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument productPoolPatch)
        {
            try
            {
                ProductPool productPoolDto = null;
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    productPoolDto = await _serviceProvider.GetService<IProductPoolRepository>().PatchProductPoolAsync(id, productPoolPatch, userClaim).ConfigureAwait(false);
                    if (productPoolDto != null)
                    {
                        return Ok(productPoolDto);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>())
                {
                    productPoolRepository.Init(userClaim.TenantDatabase);
                    if (await productPoolRepository.DeleteProductPoolAsync(id).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
