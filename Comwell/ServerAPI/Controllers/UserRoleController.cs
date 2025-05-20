using Microsoft.AspNetCore.Mvc;
using Core.Models;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = Enum.GetNames(typeof(UserRole));
            return Ok(roles);
        }
    }
}