using CCCategoryService.Data;
using CCCategoryService.Dtos;
using CCCategoryService.Interface;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CCCategoryService.Controllers
{
    public class CategoryPoolController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public CategoryPoolController(IServiceProvider serviceProvider)
        { 
            _serviceProvider= serviceProvider;
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                ICategoryPoolRepository categoryPoolRepository = _serviceProvider.GetService<ICategoryPoolRepository>();
                categoryPoolRepository.Init(userClaim.TenantDatabase);
                return Ok(await _serviceProvider.GetService<ICategoryPoolRepository>().GetCategoryPoolsAsync());
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        public async Task <IActionResult> Get(Guid id)
        { 
            UserClaim userClaim= null;

            if (HttpContext.User.Claims != null)
            {
                userClaim = new UserClaim(HttpContext.User.Claims);
            }
            CategoryPoolDto categoryPoolDto = await _serviceProvider.GetService<ICategoryPoolRepository>().GetCategoryPoolByIdAsync(id);
            if (categoryPoolDto == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(categoryPoolDto);
            }
        }
        public async Task<IActionResult> Post(CategoryPoolDto categoryPoolDto)
        {
            try
            {
                Guid? categorypoolId = null;
                UserClaim userClaim = null;

                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                categorypoolId = await _serviceProvider.GetService<ICategoryPoolRepository>().AddCategoryPoolAsync(categoryPoolDto, userClaim);
                return Created(new Uri($"{HttpContext.Request.GetEncodedUrl()}/{categorypoolId}"), null);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        public async Task<IActionResult>  Put(Guid id, CategoryPoolDto categoryPoolDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (id != categoryPoolDto.Id)
                {
                    return BadRequest("The id inside the body do not match the query parameter");
                }
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)

                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                await _serviceProvider.GetService<ICategoryPoolRepository>().UpdateCategoryPoolAsync(categoryPoolDto, userClaim);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument categoryPoolPatch)
        {
            try
            {
                CategoryPoolDto categoryPoolDto = null;
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                categoryPoolDto = await _serviceProvider.GetService<ICategoryPoolRepository>().PatchCategoryPoolAsync(id, categoryPoolPatch,userClaim).ConfigureAwait(false);
                if (categoryPoolDto != null) 
                { 
                    return Ok(categoryPoolDto);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }
                if (await _serviceProvider.GetService<ICategoryPoolRepository>().DeleteCategoryPoolAsync(id).ConfigureAwait(false)>0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
    }
}