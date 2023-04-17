using CCApiLibrary.CustomAttributes;
using CCApiLibrary.Models;
using CCProductPriceService.DTOs;
using CCProductPriceService.Interfaces;
using CCProductPriceService.Repositories;
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
    public class ProductPriceListController : CCApiControllerBase
    {
        private IServiceProvider _serviceProvider;

        public ProductPriceListController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ProductPriceList))]
        [SwaggerOperation("Get a list with ProductPriceList items (using Dapper)")]
        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    return Ok(await repo.GetAllProductPriceLists(userClaim).ConfigureAwait(false));

                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a ProductPriceList by Id (using Dapper)")]
        public async Task<IActionResult> Get(Guid id)
        {
            UserClaim userClaim = GetUserClaim();
            using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
            {
                repo.Init(userClaim.TenantDatabase);
                 ProductPriceList productPriceList = await repo.GetProductPriceListById(id, userClaim).ConfigureAwait(false);
                if (productPriceList != null)
                {
                    return Ok(productPriceList);
                }
                else
                {
                    return NotFound();
                }
            }
            


        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Adds a new ProductPriceList")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Post(ProductPriceListBase priceList)
        {
            try
            {
                Guid? listId = null;
                UserClaim userClaim = GetUserClaim();

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                using (IProductPriceListRepository productPriceListRepository = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    productPriceListRepository.Init(userClaim.TenantDatabase);
                    listId = await _serviceProvider.GetService<IProductPriceListRepository>().AddProductPriceListAsync(priceList, userClaim);
                    return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{listId}"), null);
                }
                
            }
            catch (Exception)
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
        [SwaggerOperation("Updates a ProductPriceList")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Put(Guid id, [ModelBinder] ProductPriceList productDto)
        {
            try
            {
                if (id != productDto.Id)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IProductPriceListRepository productPriceListRepository = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    productPriceListRepository.Init(userClaim.TenantDatabase);
                    if (await productPriceListRepository.UpdateProductPriceListAsync(productDto, userClaim).ConfigureAwait(false) > 0)
                    {
                        return NoContent();
                    }
                    return NotFound();
                }
            }
            catch(Exception) 
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [SwaggerOperation("Patch a ProductPriceList")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument jsonPatch)
        {
            try 
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
                    if (dto != null)
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

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("{id}")]
        [SwaggerOperation("Delete a ProductPriceList")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = GetUserClaim();
                using (IProductPriceListRepository repo = _serviceProvider.GetService<IProductPriceListRepository>())
                {
                    repo.Init(userClaim.TenantDatabase);
                    if (await _serviceProvider.GetService<IProductPriceListRepository>().DeletePriceListAsync(id).ConfigureAwait(false) > 0)
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
