using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public dynamic GetLoginUser()
        {
            string val = "";
            HttpContext.Request.Cookies.TryGetValue("wms-user", out string value);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic cookie = Newtonsoft.Json.JsonConvert.DeserializeObject(value);
                val = cookie.userName;
            }
            return val;
        }
    }
}
