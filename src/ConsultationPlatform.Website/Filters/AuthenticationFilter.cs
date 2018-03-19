using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GZCNegotiation.Services.EtgInfoLib;

namespace Consultation.Filters
{
    public class AuthenticationFilterAttribute : TypeFilterAttribute
    {
        public AuthenticationFilterAttribute() : base(typeof(AuthenticationFilterImpl))
        {
        }
    }

    public class AuthenticationFilterImpl : IAsyncActionFilter
    {
        private readonly IEtgInfoService etgService;

        public AuthenticationFilterImpl(IEtgInfoService etgService)
        {
            this.etgService = etgService;
        }

        /// <summary>
        /// ETG身份认证
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
                if(!string.IsNullOrEmpty(errorDetal))
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
            await next();
        }
    }
}