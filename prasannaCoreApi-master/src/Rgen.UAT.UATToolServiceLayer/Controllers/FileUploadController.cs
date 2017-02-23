using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class FileUploadController : Controller
    {
        private clsDbContext _context;

        IHostingEnvironment _env;
        public FileUploadController(IHostingEnvironment envm, clsDbContext context)
        {
            _context = context;
            _env = envm;
        }
        [HttpGet]
        public string Get()
        {
            return "hello file";
        }
        [HttpPost, Route("UploadFile")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public IActionResult UploadFile()
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            bool isUpload = false;

            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                isUpload = false;

            }

            /*test Code 1*/
            byte[] byt;
            using (var reader = new StreamReader(HttpContext.Request.Form.Files["file"].OpenReadStream()))
            {
                var fileContent = reader.ReadToEnd();

                BinaryReader br = new BinaryReader(HttpContext.Request.Form.Files["file"].OpenReadStream());
                byt = br.ReadBytes((Int32)fileContent.Length);
            }
            /*end*/




            //var stream = HttpContext.Request.Form.Files["file"];
            ///*Test Code*/
            //var uploads = Path.Combine(_env.WebRootPath, "upload");
            //using (var fileStream = new FileStream(Path.Combine(uploads, stream.FileName), FileMode.Create))
            //{

            //    stream.CopyTo(fileStream);
            //}
            ///*END*/

            //var name = stream.FileName;
            //int Length = Convert.ToInt32(HttpContext.Request.ContentLength);
            //byte[] bytes = new byte[Length];
            //stream.OpenReadStream().Read(bytes, 0, Length);

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "TestUploadFile";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@imagename", SqlDbType.VarChar, 500) { Value = HttpContext.Request.Form.Files["file"].FileName });
                cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar, 500) { Value = HttpContext.Request.Form.Files["file"].ContentType });

                cmd.Parameters.Add(new SqlParameter("@imagedata", SqlDbType.VarBinary, 5000000) { Value = byt });
                cmd.Parameters.Add(new SqlParameter("@outval", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                int i = cmd.ExecuteNonQuery();
                bool outval = (bool)cmd.Parameters["@outval"].Value;

                if (i != 0 && outval != false)
                {
                    isUpload = true;
                }
            }
            return Json(isUpload);

        }

        [HttpGet, Route("GetUploadFilesfromDB")]
        
        public FileResult GetUploadFilesfromDB()
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {


            }
            var filename = "";
            var contentType = "";

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "GetfilefromDB";
                cmd.CommandType = CommandType.StoredProcedure;

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        filename = dataReader["imagename"].ToString();
                        contentType= dataReader["ContentType"].ToString();
                        byte[] bytes = (byte[])dataReader["imagedata"];
                        Response.ContentType = dataReader["ContentType"].ToString();
                        Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["imagename"].ToString());
                        Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
            }
            return File(filename, contentType, filename);
        }



    }
}