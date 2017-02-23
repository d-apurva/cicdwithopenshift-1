using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class UserInfoController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";

        private clsDbContext _context;

        public UserInfoController(clsDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("GetUserInfo/{spuserid}")]
        public JsonResult GetUserInfo(string spuserid)
        {
           
            List<UserInfo> userInfoObj = new List<UserInfo>();
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            string SchemaName = "";
            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                return Json("Invalid Url");
            }

        
      
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.sp_GetUserInfo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SpUserId", SqlDbType.VarChar) { Value = spuserid });
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 10) { Value = SchemaName });
                //cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                var retObject = new List<dynamic>();
                var n_fname ="";
                var n_lname="";
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["FirstName"] != null)
                            n_fname = Convert.ToString(dataReader["FirstName"]);
                        if (dataReader["LastName"] != null)
                            n_lname = Convert.ToString(dataReader["LastName"]);

                        userInfoObj.Add(new UserInfo()
                        {
                            FirstName = n_fname + " " + n_lname//,
                        });
                    }
                   
                }
                return Json(userInfoObj);
            }


            ///return Json(userInfoObj);
        }
    }
}
