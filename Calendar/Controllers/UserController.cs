using Calendar.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext context;
        public UserController(ApplicationContext context)
        {
            this.context = context;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] )
        { 
        
        }

    }
}
