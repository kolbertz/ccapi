using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CCProductPriceService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductPricePoolController : CCApiControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductPricePoolController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        
        [SwaggerOperation("Get a list with ProductPrice Pool items (using Dapper)")]
        [ProducesResponseType(200, Type = typeof(ProductPricePoolBase))]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPricePoolRepository repo = _serviceProvider.GetService<IProductPricePoolRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetAllPricePools(userClaim).ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a ProductPricePool by Id (using Dapper)")]
        [ProducesResponseType(200, Type = typeof(ProductPricePool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPricePoolRepository repo = _serviceProvider.GetService<IProductPricePoolRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    ProductPricePool productPricePool = await repo.GetPricePoolById(id, userClaim).ConfigureAwait(false);
                    if (productPricePool != null)
                    {
                        return Ok(productPricePool);
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [SwaggerOperation("Adds a new ProductPricePool")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Post(ProductPricePoolBase pricePoolBase)
        {
            try
            {
                Guid? poolId = null;
                UserClaim userClaim = null;

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPricePoolRepository productPricePoolRepository = _serviceProvider.GetService<IProductPricePoolRepository>())
                {
                    productPricePoolRepository.Init(userClaim.TenantDatabase);
                    poolId = await _serviceProvider.GetService<IProductPricePoolRepository>().AddPricePoolAsync(pricePoolBase, userClaim);
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
        [SwaggerOperation("Updates a ProductPrice")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Put(Guid id, [ModelBinder] ProductPricePool pricePool)
        {
            if (id != pricePool.Id)
            {
                return BadRequest("The id inside the body do not match the query parameter");
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            using (IProductPricePoolRepository productPricePoolRepository = _serviceProvider.GetService<IProductPricePoolRepository>())
            {
                productPricePoolRepository.Init(userClaim.TenantDatabase);
                if (await productPricePoolRepository.UpdatePricePool(pricePool, userClaim).ConfigureAwait(false) > 0) 
                {
                    return NoContent();
                }
                return NotFound();
                
            }
        }



        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a ProductPricePool")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        {
            ProductPricePool dto;
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductPricePoolRepository productPricePoolRepository = _serviceProvider.GetService<IProductPricePoolRepository>())
            {
                productPricePoolRepository.Init(userClaim.TenantDatabase);
                dto = await productPricePoolRepository.PatchPricePoolAsync(id, jsonPatch, userClaim).ConfigureAwait(false);
                if(dto != null)
                {
                    return Ok(dto);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        [SwaggerOperation("Delete a ProductPricePool")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPricePoolRepository repo = _serviceProvider.GetService<IProductPricePoolRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    if (await _serviceProvider.GetService<IProductPricePoolRepository>().DeletePricePool(id).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception) { return StatusCode(500); }


        }
    }
}
