using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class UserRegistrationController : Controller
    {
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public UserRegistrationController(clsDbContext context)
        {
            _context = context;
        }



        #region Area Operations

        [HttpGet, Route("GetArea")]
        public List<Area> GetArea()
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();

            List<Area> areaResult = new List<Area>();
            try
            {

                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    // return Json("Invalid Url");
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spRegion";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Select" });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    string retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {

                            //if (dr["areaId"] != null || dr["areaName"] != null)
                            //    continue;

                            areaResult.Add(new Area()
                            {
                                areaId = (dr["areaId"] == null) ? "" : Convert.ToString(dr["areaId"]),
                                areaName = (dr["areaName"] == null) ? "" : Convert.ToString(dr["areaName"]),
                            });


                        }


                        //int areaIdOrdinal = dr.GetOrdinal("areaId");
                        //int areaNameOrdinal = dr.GetOrdinal("areaName");


                        //if (dr.IsDBNull(areaIdOrdinal) || dr.IsDBNull(areaNameOrdinal))


                        //    areaResult.Add(new Area()
                        //    {
                        //        areaId = (dr.IsDBNull(areaIdOrdinal)) == true ? "" : Convert.ToString(dr["areaId"]),
                        //        areaName = (dr.IsDBNull(areaNameOrdinal)) == true ? "" : Convert.ToString(dr["areaName"]),
                        //    });

                    }
                }

            }
            catch (Exception ex)
            {

                return null;
            }
            // return _result;
            return areaResult;
        }
        #endregion


        [HttpPost, Route("InsertRegisterUser")]
        public Dictionary<string, string> InsertRegisterUser([FromBody]ClsUserRegistration Reg)

        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
           


            try
            {


                if (string.IsNullOrEmpty(Reg.Email))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "Please provide Email");
                    return _result;
                }

                if (string.IsNullOrEmpty(Reg.ConfirmPassword) || string.IsNullOrEmpty(Reg.Password) || string.IsNullOrEmpty(Reg.FirstName) || string.IsNullOrEmpty(Reg.LastName))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "Mandatory Fields data is required");
                    return _result;
                }
              


                if ((Reg.ConfirmPassword) != (Reg.Password))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "Passwords do not match.");
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


                string statementType = string.Empty;
                if (string.IsNullOrEmpty(Reg.UserId))
                    statementType = "Insert";
                else
                    statementType = "Update";




                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "[UAT].[SpUserRegistration]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 500) { Value = Reg.FirstName});
                    cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 500) { Value = Reg.LastName });
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 500) { Value = Reg.Email });
                    cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 500) { Value = Reg.Password});          
                    cmd.Parameters.Add(new SqlParameter("@DOB", SqlDbType.VarChar, 500) { Value = (Reg.DOB == "") ? null : Reg.DOB });
                    cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.VarChar, 500) { Value = Reg.Gender  });
                    cmd.Parameters.Add(new SqlParameter("@MobileNo", SqlDbType.VarChar, 500) { Value = Reg.MobileNo});
                    cmd.Parameters.Add(new SqlParameter("@Country", SqlDbType.NVarChar, 500) { Value = (Reg.Country=="")? null: Reg.Country});
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 500) { Value = statementType });         
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (!string.IsNullOrEmpty(ReturnParamValue) || ReturnParamValue.ToLower() == "Email Already Exist..!!")
                    {
                            string[] arr = ReturnParamValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length == 1)
                            {/*insert*/
                                _result.Add(this._statusText, "Done");
                                _result.Add("Email", Reg.Email);
                            }
                            else
                            {

                            _result.Add(this._errorText, "Sorry! This Email is already registered.");
                            return _result;
                           
                             }

                     

                        
                    }
                    else
                    {
                        _result.Add(this._errorText, "Sorry! Something went wrong, please try again!");
                        return _result;
                    }
                        
                }

            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

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
