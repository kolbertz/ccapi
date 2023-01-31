using CCAuthServer.OauthRequest;
using CCAuthServer.Services.CodeService;
using CCAuthServer.Services;
using Microsoft.AspNetCore.Mvc;
using CCAuthServer.Context;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Web.Helpers;

namespace CCAuthServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizeResultService _authorizeResultService;
        private readonly ICodeStoreService _codeStoreService;
        IUserRepository _userRepository;

        public HomeController(IHttpContextAccessor httpContextAccessor, IAuthorizeResultService authorizeResultService,
            ICodeStoreService codeStoreService, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizeResultService = authorizeResultService;
            _codeStoreService = codeStoreService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetSystems(string username)
        {
            var tenants = await _userRepository.GetUserSystems(username);
            return Json(tenants);
        }

        [HttpPost]
        public async Task<IActionResult> Login(OpenIdConnectLoginRequest loginRequest)
        {
            // here I have to check if the username and passowrd is correct
            // and I will show you how to integrate the ASP.NET Core Identity
            // With our framework
            UserData userData = await _userRepository.GetUserData(loginRequest.UserName, loginRequest.Password);
            string pwdValue = loginRequest.Password + (userData.OldSaltKey ?? "mb3XdW5fN0ztctuJKbUv7XhD16") + "ePK2kOIZTDMePvPY0Yxb" + userData.Id.ToString();
            string encodePwd = string.IsNullOrEmpty(loginRequest.Password) ? string.Empty : Crypto.SHA256(pwdValue);

            if (string.CompareOrdinal(userData.Password, encodePwd) == 0)
            {
                var result = _codeStoreService.UpdatedClientDataByCode(loginRequest);
                if (result != null)
                {

                    loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
                    return Redirect(loginRequest.RedirectUri);
                }
            }
            return RedirectToAction("Error", new { error = "invalid_request" });
        }

        public IActionResult Authorize(AuthorizationRequest authorizationRequest)
        {
            var result = _authorizeResultService.AuthorizeRequest(_httpContextAccessor, authorizationRequest);

            if (result.HasError)
                return RedirectToAction("Error", new { error = result.Error });

            var loginModel = new OpenIdConnectLoginRequest
            {
                RedirectUri = result.RedirectUri,
                Code = result.Code,
                RequestedScopes = result.RequestedScopes,
                Nonce = result.Nonce
            };


            return View("Login", loginModel);
        }

        public JsonResult Token()
        {
            var result = _authorizeResultService.GenerateToken(_httpContextAccessor);

            if (result.HasError)
                return Json("0");

            return Json(result);
        }

        public IActionResult Error(string error)
        {
            return View(error);
        }
    }
}
