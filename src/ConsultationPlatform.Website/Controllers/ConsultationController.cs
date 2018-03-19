using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Consultation.Controllers
{
    [Route("ConsultationAPI/[controller]")]
    public class ConsultationController : Controller
    {
        [Route("[action]/{customCode}")]
        [HttpGet]
        public string getById(string customCode)
        {
            if (customCode == "4401986999")
                return @"{
                            ""Consultation"":[
                                { ""CONSULTATION_ID"":""1234567890"" , ""ENTRY_ID"":""514120160416636421,514120160416636421,514120160416636421"" , ""TRADE_CO"":""4401986999"" , ""TRADE_NAME"":""广州海通"" , ""MOBILE"":""15820200921"" , ""EMAIL"":""fatherpeanut@163.com"" , ""BEGINDATE"":""2016-10-28 15:00:00.000"" , ""ENDDATE"":""2016-10-28 16:00:00.000"" , ""MESSAGE"":""开发测试，您好，您申报的报关单号：514120160416636421有异，请于2016-10-29 05:46至2016-10-29 05:46 期间到易通关：gzcustoms.gov.cn平台上进行网上磋商，如有疑问，请联系15920426026；管理员。。。。。。"" , ""STATUS"":""等待确认"" },
                                { ""CONSULTATION_ID"":""1234567899"" , ""ENTRY_ID"":""1111111111,2222222222,3333333333"" , ""TRADE_CO"":""4401986999"" , ""TRADE_NAME"":""广州海通"" , ""MOBILE"":""15820200921"" , ""EMAIL"":""fatherpeanut@163.com"" , ""BEGINDATE"":""2016-10-28 15:00:00.000"" , ""ENDDATE"":""2016-10-28 16:00:00.000"" , ""MESSAGE"":""磋商内容"" , ""STATUS"":""接受"" }
                        ]}";
            else
                return @"{}";
        }

        [Route("[action]/{consultationId}/{status}")]
        [HttpGet]
        public string postStatus(string consultationId, string status)
        {
            return @"{
                        ""Status"":""OK"",
                        ""Msg"":""""
                    }";
            //return @"{
            //            ""Status"":""ERROR"",
            //            ""Msg"":""测试错误""
            //        }";
        }
    }
}
