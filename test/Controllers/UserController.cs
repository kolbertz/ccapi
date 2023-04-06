using CCApiLibrary.Models;
using CCUserService.DTOs;
using CCUserService.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CCUserService.Controllers
{
    public class UserController : ControllerBase
    {
        private IServiceProvider _serviceProvider;

        public UserController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get a list with "<see cref="User"/>"  (using Dapper)
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(204)]
        [SwaggerOperation("Get a list with Users")]
        public async Task<IActionResult> Get(int? skip, int? take)
        {
            try
            {
                UserClaim userClaim = null;
                if (HttpContext.User.Claims != null)
                {
                    userClaim = new UserClaim(HttpContext.User.Claims);
                }

                using (IUserRepository userRepository = _serviceProvider.GetService<IUserRepository>())
                {
                    IEnumerable<User> usersList = null;
                    userRepository.Init(userClaim.TenantDatabase);
                    usersList = await userRepository.GetAllUsers(take, skip, userClaim).ConfigureAwait(false);
                    return Ok(usersList);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
