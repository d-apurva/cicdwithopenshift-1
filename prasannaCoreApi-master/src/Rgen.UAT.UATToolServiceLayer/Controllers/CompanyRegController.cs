using System;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class CompanyRegController : Controller
    {
        clsDbContext _context;
        public CompanyRegController(clsDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        [HttpPost, Route("SaveCompanyDetails")]
        public string SaveCompanyDetails([FromBody] clsCompanyReg _reg)
        {

            string _result = "";
            try
            {
               // string AppUrl = HttpContext.Request.Headers["appurl"];
                string SchemaName = "";
                /*if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {

                    _result = "Invalid Appurl";

                }*/
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "[UAT].[CompanyDetails]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@sClient_Name", SqlDbType.VarChar, 500) { Value = _reg.sClient_Name });
                    cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 500) { Value = _reg.FirstName});
                    cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 500) { Value = _reg.LastName});
                    cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 500) { Value = _reg.Password });
                    cmd.Parameters.Add(new SqlParameter("@sClient_Address", SqlDbType.VarChar, 500) { Value = _reg.sClient_Address });
                    cmd.Parameters.Add(new SqlParameter("@sClient_State", SqlDbType.VarChar, 500) { Value = _reg.sClient_State });
                    cmd.Parameters.Add(new SqlParameter("@sClient_Country", SqlDbType.VarChar, 500) { Value = _reg.sClient_Country });
                    cmd.Parameters.Add(new SqlParameter("@sClient_ContactNo", SqlDbType.VarChar, 500) { Value = _reg.sClient_ContactNo });
                    cmd.Parameters.Add(new SqlParameter("@sClient_Url", SqlDbType.VarChar, 500) { Value = _reg.sClient_Url });
                    cmd.Parameters.Add(new SqlParameter("@sClient_AdminEmailID", SqlDbType.VarChar, 500) { Value = _reg.sClient_AdminEmailID });
                    cmd.Parameters.Add(new SqlParameter("@sClientType", SqlDbType.VarChar, 500) { Value = _reg.sClientType });
                    cmd.Parameters.Add(new SqlParameter("@Mobile_Number", SqlDbType.VarChar, 500) { Value = _reg.Mobile_Number });
                    cmd.Parameters.Add(new SqlParameter("@sClientApp_url", SqlDbType.VarChar, 500) { Value = _reg.sClientApp_url });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Insert" });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result != 0)
                    {
                        if (!string.IsNullOrEmpty(_retValue))
                        {
                            _result = cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString();

                        }
                        else
                        {
                            _result = cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString();
                        }
                    }



                }
            }
            catch (Exception ex)
            {

                _result = ex.Message;
            }
            return _result;
        }

        [HttpGet, Route("FillDropdown")]
        public List<clsDropdown> FillDropdown(string Type , string _Param )
        {
            List<clsDropdown> _cls = new List<clsDropdown>();
            string SpName = "";
            try
            {
                if (string.IsNullOrEmpty(Type))
                {
                   return null;
                }
                else
                {
                    SpName = Type.ToLower() == "c" ? "UAT.SP_GetallCountries" : "UAT.SP_GetStatesbyCountry";
                }

                if (string.IsNullOrEmpty(_Param) && Type == "s")
                {
                     return null;
                }
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = SpName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (Type.ToLower() != "c")
                    {
                        cmd.Parameters.Add(new SqlParameter("@country_id", SqlDbType.Int) { Value = _Param });
                    }
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            _cls.Add(new clsDropdown()
                            {
                                id=Convert.ToString(dr[0]),
                                Value = Convert.ToString(dr[1]),
                            });
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            return _cls;
        }
    }
}
