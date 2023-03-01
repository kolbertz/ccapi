using CCApiLibrary.CustomAttributes;
using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http.Controllers;

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
                IProductPoolRepository productPoolRepository = _serviceProvider.GetService<IProductPoolRepository>();
                productPoolRepository.Init(userClaim.TenantDatabase);
                return Ok(await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolsAsync());
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

            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            ProductPoolDto productPoolDto = await _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolByIdAsync(id);
            if 
                (productPoolDto == null) 
            {
                return NotFound(); 
            }
            else
            {
                return Ok(productPoolDto);
            }
            

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidateModelAttribute))]

        public async Task<IActionResult> Post(ProductPoolDto productPoolDto)
        {
            try
            {

                Guid? poolId = null;
                UserClaim userClaim = null;

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                poolId = await _serviceProvider.GetService<IProductPoolRepository>().AddProductPoolAsync(productPoolDto, userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{poolId}"), null);
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
        public async Task<IActionResult> Put(Guid id, ProductPoolDto productPoolDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (id != productPoolDto.Id)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                await _serviceProvider.GetService<IProductPoolRepository>().UpdateProductPoolAsync(productPoolDto, userClaim);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument productPoolPatch)
        {
            try
            {
                ProductPoolDto productPoolDto = null;
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
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
                if (await _serviceProvider.GetService<IProductPoolRepository>().DeleteProductPoolAsync(id).ConfigureAwait(false) > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        //[HttpPost]
        //[ServiceFilter (typeof(ValidateModelAttribute))]
        //public IActionResult Post(ProductPoolDto productPoolDto)
        //{
        //    return Created(productPoolDto);
        //}
    }
}
