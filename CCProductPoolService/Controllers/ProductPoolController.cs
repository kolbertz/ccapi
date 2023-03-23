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
        [ProducesResponseType(200, Type = typeof(ProductPool))]
        [ProducesResponseType(204)]
        [SwaggerOperation("Returns a list of productPools")]
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
                    return Ok(await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolsAsync(userClaim));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductPool))]
        [ProducesResponseType(404)]
        [SwaggerOperation("Returns a ProductPool by the given ID")]
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
                    ProductPool productPoolDto = await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolByIdAsync(id, userClaim);
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
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidateModelAttribute))]

        [SwaggerOperation("Adds a ProductPool")]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [SwaggerOperation("Updates a ProductPool via put request")]
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
                    if (await _serviceProvider.GetService<IProductPoolRepository>().UpdateProductPoolAsync(productPoolDto, userClaim).ConfigureAwait(false) > 0)
                    { return NoContent(); }
                    return NotFound();
                   
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Updates the given properties of a ProductPool via patch request")]
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

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Deletes the ProductPool with the given ID")]
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
