using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestWebApi.Controllers
{
    public class DocumentController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 1)
            {
                var postedFile = httpRequest.Files[0];
                string token = Guid.NewGuid().ToString();
                postedFile.SaveAs(
                    HttpContext.Current.Server.MapPath(string.Format("~/App_Data/Documents/{0}_{1}", token,
                        new FileInfo(postedFile.FileName).Name)));

                result = Request.CreateResponse(HttpStatusCode.OK, token);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage result = null;

            DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Data/Documents"));
            var files = directory.GetFiles(string.Format("{0}_*", id));
            var localFilePath = HttpContext.Current.Server.MapPath("~/" + id);

            if (String.IsNullOrEmpty(id))
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else if (!files.Any())
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read));
                //result.Content.Headers.ContentLocation = new Uri(files[0].FullName);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = files[0].Name.Replace(string.Format("{0}_", id), string.Empty);
            }

            return result;
        }
    }
}
