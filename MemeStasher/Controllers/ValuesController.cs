using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace MemeStasher.Controllers
{
    [Route("api/[controller]")]
    public class MemesController : Controller
    {        
        SHA256 hasher = SHA256.Create();

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

//         [HttpGet("{sha}")]
//         public ActionResult Get(string sha)
//         {
//             if (!IsHashValid(sha)) {
//                 return BadRequest();
//             }

// //            return File()
//         }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private char[] validHashChars = Enumerable.Range((short)'a', (short)'z' - (short)'a').Select(i => (char)i)
                                            .Concat(
                                        Enumerable.Range(0, 10).Select(i => i.ToString()[0]))
                                            .ToArray();
        private bool IsHashValid(string sha) {
            return sha.All(x => validHashChars.Contains(x));
        }
    }
}
