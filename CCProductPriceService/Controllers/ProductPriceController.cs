using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CCProductPriceService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductPriceController : CCApiControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductPriceController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("ProductPrice")]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceRepository repo = _serviceProvider.GetService<IProductPriceRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetAllProductPricesAsync().ConfigureAwait(false));
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceRepository repo = _serviceProvider.GetService<IProductPriceRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetProductPriceByIdAsync(id).ConfigureAwait(false));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(ProductPrice productPrice)
        {
            try
            {
                Guid? priceId = null;
                UserClaim userClaim = GetUserClaim();

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                priceId = await _serviceProvider.GetService<IProductPriceRepository>().AddProductPriceAsync(productPrice, userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{priceId}"), null);

            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        //[HttpPatch]
        //[Route("{id}")]
        //public async Task<IActionResult> Update(Guid id, JsonPatchDocument jsonPatch)
        //{

        //}

        //[HttpPatch]
        //[Route("{id}")]
        //public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        //{

        //}

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceRepository repo = _serviceProvider.GetService<IProductPriceRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await _serviceProvider.GetService<IProductPriceRepository>().DeleteProductPriceAsync(id).ConfigureAwait(false));
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
    }
}
