using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [HttpGet]
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
                using (IProductPriceRepository productPriceRepository = _serviceProvider.GetService<IProductPriceRepository>())
                {
                    productPriceRepository.Init(userClaim.TenantDatabase);
                    priceId = await _serviceProvider.GetService<IProductPriceRepository>().AddProductPriceAsync(productPrice, userClaim);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{priceId}"), null);
                }               
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        //[HttpPost]
        //[Route("{id}")]
        //[SwaggerOperation("Adds a new ProductPrice")]
        //public async Task<IActionResult> Post([ModelBinder] ProductPriceBase productDto)
        //{
        //    UserClaim userClaim = null;
        //    Guid? newproductId = null;
        //    if (HttpContext.User.Claims != null)
        //    {
        //        userClaim = new UserClaim(HttpContext.User.Claims);
        //    }

        //    using (IProductPriceRepository productPriceRepository = _serviceProvider.GetService<IProductPriceRepository>())

        //    {
        //        productPriceRepository.Init(userClaim.TenantDatabase);
        //        newproductId = await productPriceRepository.AddProductPriceAsync(productDto, userClaim).ConfigureAwait(false);
        //        return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{newproductId}"), null);
        //    }
        //}

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        {
            ProductPriceBase dto;
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductPriceRepository productPriceRepository = _serviceProvider.GetService<IProductPriceRepository>())
            {
                productPriceRepository.Init(userClaim.TenantDatabase);
                dto = await productPriceRepository.PatchProductPriceAsync(id, jsonPatch, userClaim).ConfigureAwait(false);
                return Ok(dto);
            }
            return null;
        }

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
