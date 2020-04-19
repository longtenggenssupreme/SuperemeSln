using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAspNetCore.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public string Get() {
            return "Get";
        }

        [HttpPut]
        public string Put()
        {
            return "Put";
        }

        [HttpPost]
        public string Post(IFormFile file)
        {
            return "Post file";
        }

        [HttpDelete]
        public string Delete()
        {
            return "";
        }
    }
}
