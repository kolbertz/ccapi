using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

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
        public Task<IReadOnlyList<ProductPoolDto>> Get()
        {
            return _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolsAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ProductPoolDto> Get(Guid id)
        {
            return _serviceProvider.GetService<IProductPoolRepository>().GetProductPoolByIdAsync(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(ProductPoolDto productPoolDto)
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(Guid id, ProductPoolDto productPoolDto)
        {
            if (id != productPoolDto.Id)
            {
                return BadRequest();
            }
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            await _serviceProvider.GetService<IProductPoolRepository>().UpdateProductPoolAsync(productPoolDto, userClaim);
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument productPoolPatch)
        {
            ProductPoolDto productPoolDto = null;
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            productPoolDto = await _serviceProvider.GetService<IProductPoolRepository>().PatchProductPoolAsync(id, productPoolPatch, userClaim).ConfigureAwait(false);
            return Ok(productPoolDto);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            UserClaim userClaim = null;
            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            await _serviceProvider.GetService<IProductPoolRepository>().DeleteProductPoolAsync(id).ConfigureAwait(false);
            return Ok();
        }
    }
}
