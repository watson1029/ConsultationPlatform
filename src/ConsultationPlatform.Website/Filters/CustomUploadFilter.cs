using GZCNegotiation.Services.EtgInfoLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Consultation.Filters
{
    public class CustomUploadFilterAttribute : TypeFilterAttribute
    {
        public CustomUploadFilterAttribute() : base(typeof(CustomUploadFilterImpl))
        {
        }
    }

    public class CustomUploadFilterImpl : IAsyncActionFilter
    {
        private readonly IEtgInfoService etgService;
        private readonly string key = "f6fc360b4b2ac206e002f23282a29077";
        private readonly string postUrl = "http://gzcdc.udesk.cn/api/v2/customer_import.json";

        public CustomUploadFilterImpl(IEtgInfoService etgService)
        {
            this.etgService = etgService;
        }
        /// <summary>
        /// 上传客户数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string token = context.HttpContext.Request.Query["token"];
            GZCNegotiation.Services.EtgInfoLib.Data.UserInfo userInfo;
            GZCNegotiation.Services.EtgInfoLib.Data.CompanyInfo companyInfo;
            string errorType = string.Empty;
            string errorDetal = string.Empty;
            //带Token参数则从远程API获取用户信息和企业信息
            if (!string.IsNullOrEmpty(token))
            {
                errorDetal = await etgService.RegisterUserAsync(token);
                if (!string.IsNullOrEmpty(errorDetal))
                {
                    errorType = "RequestToken无效，请重新登陆。";
                    context.Result = new RedirectToActionResult("Authentication", "Error", new { errorType, errorDetal });
                    return;
                }
            }
            //从Session中读取User信息
            try
            {
                userInfo = etgService.GetCurrentUser();
            }
            catch (Exception ex)
            {
                errorType = "Session已过期，请重新登陆。";
                errorDetal = ex.ToString();
                context.Result = new RedirectToActionResult("Authentication", "Error", new { errorType, errorDetal });
                return;
            }
            //从Session中读取Company信息
            try
            {
                companyInfo = etgService.GetCompanyInfo();
            }
            catch (Exception ex)
            {
                errorType = "CompanyInfo获取失败，请先实名认证。";
                errorDetal = ex.ToString();
                context.Result = new RedirectToActionResult("Authentication", "Error", new { errorType, errorDetal });
                return;
            }

            //获取API返回结果
            var result = await GetUdeskCustomerImport(userInfo, companyInfo);
            
            await next();
        }

        /// <summary>
        /// 调用Udesk添加/更新客户API
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="companyInfo"></param>
        /// <returns></returns>
        private async Task<string> GetUdeskCustomerImport(GZCNegotiation.Services.EtgInfoLib.Data.UserInfo userInfo, GZCNegotiation.Services.EtgInfoLib.Data.CompanyInfo companyInfo)
        {
            //生成json数据
            Models.ImportMsg import = new Models.ImportMsg();
            import.sign = Md5(key);
            import.user.email = userInfo.Email;
            import.user.nick_name = userInfo.Name;
            import.user.cellphone = userInfo.Mobile;
            import.user.org_name = companyInfo.Name;
            import.user.customer_field.TextField_7179 = companyInfo.Id;
            import.user.customer_field.TextField_7180 = companyInfo.SocialCreditCode;
            string data = JsonConvert.SerializeObject(import);
            //POST数据至远程API
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(data, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(postUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        private string Md5(string value)
        {
            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
            var result = new StringBuilder();
            foreach (byte t in bytes)
            {
                result.Append(t.ToString("x2"));
            }
            return result.ToString();
        }
    }
}
