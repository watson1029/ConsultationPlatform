using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Consultation.Controllers
{
    [Route("[controller]")]
    public class userController : Controller
    {
        [Route("[action]/{accessToken}")]
        [HttpGet]
        public string getUser(string accessToken)
        {
            if(accessToken == "abc")
                return @"{
                            ""_id"": ""567e0ba5a29bd3c57083677c"",
                            ""email"": ""zouweilun@163.com"",
                            ""mobile"": ""15820200921"",
                            ""displayName"":""邹巍伦"",
                            ""companyId"":""4401986999""
                        }";
            else
                return @"{}";
        }

        [Route("[action]/{accessToken}")]
        [HttpGet]
        public string getCompany(string accessToken)
        {
            if (accessToken == "abc")
                return @"{
                            ""TRADE_CO"":""443096143D"",
                            ""FULL_NAME"":""广州森晓贸易有限公司"",
                            ""SOCIAL_CREDIT_CODE"":""341281196802266552"",
                        }";
            else
                return @"{}";
        }
    }
}
