using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace InternetProgramming.Controllers
{
    [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
    public class DownloadController : ApiController
    {
        [HttpGet]
        [Route("api/DownloadFile")]
        [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
        public HttpResponseMessage DownloadFile(string fileName)
        {
            try
            {
                string file = @"D:/ФИСТ ИСЭбд/Интернет программирование/" + fileName; //Hard Coding for testing only. 

                if (!string.IsNullOrEmpty(file))
                {

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(file, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv"); 
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = file;
                    return response;
                    //return ResponseMessage(response); 
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Download/GetFiles")]
        [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
        public IEnumerable<string> GetFiles()
        {
            return new System.IO.DirectoryInfo("D:/ФИСТ ИСЭбд/Интернет программирование/").GetFiles().Select(x => x.Name);
        }
    }
    [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
    public class UploadController : ApiController
    {
        [HttpPost]
        [Route("api/Upload")]
        [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
        public IHttpActionResult UploadFiles()
        {
            int i = 0;
            int cntSuccess = 0;
            var uploadedFileNames = new List<string>();
            string result = string.Empty;

            HttpResponseMessage response = new HttpResponseMessage();

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[i];
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
                    try
                    {
                        postedFile.SaveAs(filePath);
                        uploadedFileNames.Add(httpRequest.Files[i].FileName);
                        cntSuccess++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    i++;
                }
            }

            result = cntSuccess.ToString() + " files uploaded succesfully.<br/>";

            result += "<ul>";

            foreach (var f in uploadedFileNames)
            {
                result += "<li>" + f + "</li>";
            }

            result += "</ul>";

            return Json(result);
        }
    }
}

