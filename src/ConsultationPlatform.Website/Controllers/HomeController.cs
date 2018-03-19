using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GZCNegotiation.Services.EtgInfoLib;
using CstInfoLib;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace Consultation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEtgInfoService etgService;
        private readonly string url = "http://gzcdc.udesk.cn/im_client/?";
        private readonly string key = "2aec788b6a6191ce0ffd5a0f13e950a7";

        private readonly ICstInfoService cstService;
        private IHostingEnvironment hostingEnv;

        public HomeController(IEtgInfoService etgService, ICstInfoService cstService, IHostingEnvironment hostingEnv)
        {
            this.etgService = etgService;
            this.cstService = cstService;
            this.hostingEnv = hostingEnv;
        }

        [Filters.AuthenticationFilter]
        public IActionResult Index()
        {
            return View();
        }

        [Filters.AuthenticationFilter]
        public IActionResult UploadDocument()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Filters.AuthenticationFilter]
        public IActionResult CustomService()
        {
            GZCNegotiation.Services.EtgInfoLib.Data.UserInfo userInfo;
            GZCNegotiation.Services.EtgInfoLib.Data.CompanyInfo companyInfo;
            userInfo = etgService.GetCurrentUser();
            companyInfo = etgService.GetCompanyInfo();
            ViewData["Url"] = url + GetCustomServiceParam(userInfo, companyInfo);
            return View();
        }

        [Filters.AuthenticationFilter]
        public async Task<IActionResult> ConsultationList()
        {
            ViewData["cstInfo"] = await GetConsultationInfo();
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            return View("UploadDocument");
        }

        [HttpPost]
        public IActionResult UploadFileEx()
        {
            FileResponse res = new FileResponse();
            try
            {
                IFormFile file = Request.Form.Files[0];
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                filename = hostingEnv.WebRootPath + $@"/{filename}";
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                    res.status = "上传成功";
                    res.size = file.Length;
                }
            }
            catch(Exception ex)
            {
                res.status = "上传失败";
                res.error = ex.ToString();
            }
            return Json(res);
        }
        
        [HttpGet]
        public async Task<IActionResult> AcceptInvite(string consultationId)
        {
            CstInfoLib.Data.PostStatus postResult = await cstService.PostConsultationStatusAsync(consultationId, "接受");
            if (postResult != null)
                ViewData["cstInfo"] = await GetConsultationInfo();
            return View("ConsultationList");
        }

        [HttpGet]
        public async Task<IActionResult> RejectInvite(string consultationId)
        {
            CstInfoLib.Data.PostStatus postResult = await cstService.PostConsultationStatusAsync(consultationId, "拒绝");
            if (postResult != null)
                ViewData["cstInfo"] = await GetConsultationInfo();
            return View("ConsultationList");
        }

        private async Task<CstInfoLib.Data.ConsultationInfo> GetConsultationInfo()
        {
            GZCNegotiation.Services.EtgInfoLib.Data.UserInfo userInfo;
            userInfo = etgService.GetCurrentUser();
            return await cstService.GetConsultationInfoAsync(userInfo.CompanyId);
        }

        private string GetCustomServiceParam(GZCNegotiation.Services.EtgInfoLib.Data.UserInfo userInfo, GZCNegotiation.Services.EtgInfoLib.Data.CompanyInfo companyInfo)
        {

            string param = string.Empty;
            string sign_str = string.Empty;
            string nonce = Guid.NewGuid().ToString("N");
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timestamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            param += "c_name=" + userInfo.Name;
            param += "&c_email=" + userInfo.Email;
            param += "&c_phone=" + userInfo.Mobile;
            param += "&c_org=" + companyInfo.Name;
            param += "&c_cf_TextField_7179=" + companyInfo.Id;
            param += "&c_cf_TextField_7180=" + companyInfo.SocialCreditCode;
            param += "&nonce=" + nonce;
            sign_str += "nonce=" + nonce;
            sign_str += "&timestamp=" + timestamp;
            sign_str += "&web_token=" + userInfo.Email;
            sign_str += "&" + key;
            param += "&signature=" + GetSHA1(sign_str);
            param += "&timestamp=" + timestamp;
            param += "&web_token=" + userInfo.Email;
            param += "&group_id=30750";
            return param;
        }

        private string GetSHA1(string strSource)
        {
            string strResult = "";

            //Create
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] bytResult = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strSource));
            for (int i = 0; i < bytResult.Length; i++)
            {
                strResult = strResult + bytResult[i].ToString("X2");
            }
            return strResult;
        }

        public class FileResponse
        {
            public string status { get; set; }
            public string error { get; set; }
            public long size { get; set; }
        }
    }
}
