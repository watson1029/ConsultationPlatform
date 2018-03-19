using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Consultation.Controllers
{
    [Route("[controller]")]
    public class tokenController : Controller
    {
        [Route("[action]/{requestToken}/{appId}")]
        [HttpGet]
        public string getAccessToken(string requestToken, string appId)
        {
            if (requestToken == "test")
                return @"{""access_token"":""abc""}";
            else
                return @"{}";
        }
    }
}
