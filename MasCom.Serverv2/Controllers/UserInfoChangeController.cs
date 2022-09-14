using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MasCom.Serverv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoChangeController : ControllerBase
    {
        // GET: api/<FileServerController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FileServerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FileServerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FileServerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FileServerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
