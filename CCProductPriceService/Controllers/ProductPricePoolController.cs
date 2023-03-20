using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(200, Type = typeof(ProductPriceList))]
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
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPricePoolRepository repo = _serviceProvider.GetService<IProductPricePoolRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetPricePoolById(id, userClaim).ConfigureAwait(false));
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
        [ProducesResponseType(StatusCodes.Status201Created)]
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
                poolId = await _serviceProvider.GetService<IProductPricePoolRepository>().AddPricePoolAsync(pricePoolBase,userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{poolId}"), null);
            }
            catch (Exception ex)
            {

                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("{id}")]
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
                await productPricePoolRepository.UpdatePricePool(pricePool,userClaim).ConfigureAwait(false);
                return NoContent();
            }
        }



        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a ProductPricePool")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        {
            ProductPricePool dto;
            UserClaim userClaim= null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductPricePoolRepository productPricePoolRepository = _serviceProvider.GetService<IProductPricePoolRepository>()) 
            {
                productPricePoolRepository.Init(userClaim.TenantDatabase);
                dto = await productPricePoolRepository.PatchPricePoolAsync(id, jsonPatch, userClaim).ConfigureAwait(false);
                return Ok(dto);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
                    return Ok(await _serviceProvider.GetService<IProductPricePoolRepository>().DeletePricePool(id).ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
