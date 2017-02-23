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
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {


        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public LoginController(clsDbContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }



        [HttpGet, Route("UserLogin")]
        public Dictionary<string, string>UserLogin(string Email, string Password)

        {
            Dictionary<string, string> _result = new Dictionary<string, string>();


            try
            {


                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "Mandatory Fields data is required");
                    return _result;
                }


                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    _result.Add(this._errorText, "Invalid Appurl");
                    return _result;

                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "[UAT].[SpUserRegistration]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 500) { Value = "" });
                    cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 500) { Value = ""});
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 500) { Value = Email });
                    
                    cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 500) { Value = Password });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 500) { Value = "Select" });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                   using (var dr=cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            _result.Add(_errorText, "Invalid Id or Passoword..!!");
                        }
                        while (dr.Read())
                        {
                            _result.Add("ID", Convert.ToString(dr["UserId"]));
                        }


                    }
                }

            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

        }



        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
