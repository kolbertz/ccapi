using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiLibrary.Models
{
    public class CCApiControllerBase : ControllerBase
    {
        protected UserClaim GetUserClaim()
        {
            if (HttpContext.User.Claims != null)
            {
                return new UserClaim(HttpContext.User.Claims);
            }
            return null;
        }
    }
}
