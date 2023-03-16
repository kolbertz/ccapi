using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CCProductPriceService.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductPriceListController : CCApiControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductPriceListController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ProductPriceList))]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetAllProductPriceLists().ConfigureAwait(false));
                    
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
                using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetProductPriceListById(id).ConfigureAwait(false));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(ProductPriceList priceListBase)
        {
            try
            {
                Guid? listId = null;
                UserClaim userClaim = GetUserClaim();

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                listId = await _serviceProvider.GetService<IProductPriceListRepository>().AddProductPriceListAsync(priceListBase, userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{listId}"), null);
            }
            catch (Exception)
            {

                return StatusCode(500);
            }


        }


        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        {
            ProductPriceList dto;
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }

            using (IProductPriceListRepository productPriceListRepository = _serviceProvider.GetService<IProductPriceListRepository>()) 
            {
                productPriceListRepository.Init(userClaim.TenantDatabase);
                dto = await productPriceListRepository.PatchProductPriceList(id, jsonPatch, userClaim).ConfigureAwait(false);
                return Ok(dto);
            }           
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await _serviceProvider.GetService<IProductPriceListRepository>().DeletePriceListAsync(id).ConfigureAwait(false));
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }

        }
    }
}
