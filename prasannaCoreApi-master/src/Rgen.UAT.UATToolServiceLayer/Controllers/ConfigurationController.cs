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
using System.IO;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";


        private clsDbContext _context;
        public ConfigurationController(clsDbContext context)
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

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet, Route("GetSummaryReport")]
        public JsonResult GetSummaryReport()
        {
            List<SummarySetting> lstSummary = null;

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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "SUMMARY" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {
                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int iField = dr.GetOrdinal("Field");
                            int iActualCount = dr.GetOrdinal("ActualCount");
                            lstSummary = new List<SummarySetting>();

                            while (dr.Read())
                            {
                                lstSummary.Add(new SummarySetting()
                                {
                                    Field = (dr.IsDBNull(iField)) == true ? "" : Convert.ToString(dr[iField]),
                                    ActualCount = (dr.IsDBNull(iActualCount)) == true ? "" : Convert.ToString(dr[iActualCount])
                                });
                            }
                        }
                    }
                    return Json(lstSummary);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }

        [HttpGet, Route("GetEnvironment/{projectId}/{testPassId}")]
        public JsonResult GetEnvironment(string projectId, string testPassId)
        {

            List<Environment_Tab> lstEnv = null;
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                if (string.IsNullOrEmpty(projectId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }

                if (string.IsNullOrEmpty(testPassId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spEnvironment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = Convert.ToInt32(testPassId) });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "SELECT" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {

                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int _Env_ID = dr.GetOrdinal("Env_ID");
                            int _Actual_Url = dr.GetOrdinal("Actual_Url");
                            int _Alias_Url = dr.GetOrdinal("Alias_Url");
                            int _Project_Id = dr.GetOrdinal("Project_Id");
                            int _TestPass_Id = dr.GetOrdinal("TestPass_Id");

                            lstEnv = new List<Environment_Tab>();

                            while (dr.Read())
                            {
                                lstEnv.Add(new Environment_Tab()
                                {
                                    envID = (dr.IsDBNull(_Env_ID)) == true ? "" : Convert.ToString(dr[_Env_ID]),
                                    actualUrl = (dr.IsDBNull(_Actual_Url)) == true ? "" : Convert.ToString(dr[_Actual_Url]),
                                    aliasUrl = (dr.IsDBNull(_Alias_Url)) == true ? "" : Convert.ToString(dr[_Alias_Url]),
                                    projectId = (dr.IsDBNull(_Project_Id)) == true ? "" : Convert.ToString(dr[_Project_Id]),
                                    testPassId = (dr.IsDBNull(_TestPass_Id)) == true ? "" : Convert.ToString(dr[_TestPass_Id])
                                });
                            }
                        }
                    }
                    return Json(lstEnv);
                }

            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }


        [HttpGet, Route("GetGeneralSetting/{testPassId}")]
        public JsonResult GetGeneralSetting(string testPassId)
        {
            GeneralSetting oGeneralSetting = new GeneralSetting();
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                if (string.IsNullOrEmpty(testPassId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = Convert.ToInt32(testPassId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "GSSELECT" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {
                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int _TestPass_ID = dr.GetOrdinal("TestPass_ID");
                            int _Testing_Type = dr.GetOrdinal("Testing_Type");
                            int _FeedBack_Type = dr.GetOrdinal("FeedBack_Type");
                            int _TestStepUnique_Name = dr.GetOrdinal("TestStepUnique_Name");

                            int _counter = 0;

                            //lstEnv = new List<Environment_Tab>();

                            while (dr.Read())
                            {
                                if (_counter == 0)
                                {
                                    oGeneralSetting.testPassId = (dr.IsDBNull(_TestPass_ID)) == true ? "" : Convert.ToString(dr[_TestPass_ID]).Trim();
                                    oGeneralSetting.testingType = (dr.IsDBNull(_Testing_Type)) == true ? "" : Convert.ToString(dr[_Testing_Type]).Trim();
                                    oGeneralSetting.feedbackType = (dr.IsDBNull(_FeedBack_Type)) == true ? "" : Convert.ToString(dr[_FeedBack_Type]).Trim();
                                    oGeneralSetting.testStepUnique = (dr.IsDBNull(_TestStepUnique_Name)) == true ? "" : Convert.ToString(dr[_TestStepUnique_Name]).Trim();
                                }
                                else
                                {
                                    break;
                                }
                                _counter++;
                            }
                        }
                    }
                    return Json(oGeneralSetting);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }

        [HttpGet, Route("GetUserSetting/{projectId}/{testPassId}")]
        public JsonResult GetUserSetting(string projectId, string testPassId)
        {

            List<UserSetting> lstUS = null;
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                Dictionary<string, string> oError = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }

                if (string.IsNullOrEmpty(testPassId))
                {
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spUserSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = Convert.ToInt32(testPassId) });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "SELECT" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int _User_ID = dr.GetOrdinal("User_ID");
                            int _User_Name = dr.GetOrdinal("User_Name");
                            int _UserSetting_ID = dr.GetOrdinal("UserSetting_ID");
                            int _Env_ID = dr.GetOrdinal("Env_ID");
                            int _Actual_Url = dr.GetOrdinal("Actual_Url");
                            int _Alias_Url = dr.GetOrdinal("Alias_Url");
                            int _Project_Id = dr.GetOrdinal("Project_Id");
                            int _TestPass_Id = dr.GetOrdinal("TestPass_Id");


                            lstUS = new List<UserSetting>();

                            while (dr.Read())
                            {
                                lstUS.Add(new UserSetting()
                                {
                                    actualUrl = (dr.IsDBNull(_Actual_Url)) == true ? "" : Convert.ToString(dr[_Actual_Url]).Trim(),
                                    aliasUrl = (dr.IsDBNull(_Alias_Url)) == true ? "" : Convert.ToString(dr[_Alias_Url]).Trim(),
                                    envId = (dr.IsDBNull(_Env_ID)) == true ? "" : Convert.ToString(dr[_Env_ID]).Trim(),
                                    projectId = (dr.IsDBNull(_Project_Id)) == true ? "" : Convert.ToString(dr[_Project_Id]).Trim(),
                                    testPassId = (dr.IsDBNull(_TestPass_Id)) == true ? "" : Convert.ToString(dr[_TestPass_Id]).Trim(),
                                    userId = (dr.IsDBNull(_User_ID)) == true ? "" : Convert.ToString(dr[_User_ID]).Trim(),
                                    userName = (dr.IsDBNull(_User_Name)) == true ? "" : Convert.ToString(dr[_User_Name]).Trim(),
                                    userSettingId = (dr.IsDBNull(_UserSetting_ID)) == true ? "" : Convert.ToString(dr[_UserSetting_ID]).Trim()

                                });
                            }
                        }
                    }
                    return Json(lstUS);
                }

            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, "No Data Found");
                return Json(oError);
            }
        }

        [HttpGet, Route("GetConfigurationDocuments/{projectId}")]
        public JsonResult GetConfigurationDocuments(string projectId)
        {
            List<ConfigurationDocuments> lstCD = null;
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;


                if (string.IsNullOrEmpty(projectId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationDocuments";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Select" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {
                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int _ConfigurationDocument_ID = dr.GetOrdinal("ConfigurationDocument_ID");
                            int _Project_Id = dr.GetOrdinal("Project_Id");
                            int _AttachmentName = dr.GetOrdinal("AttachmentName");
                            int _FileDescription = dr.GetOrdinal("FileDescription");
                            int _FileName = dr.GetOrdinal("FileName");
                            int _FilePath = dr.GetOrdinal("FilePath");


                            lstCD = new List<ConfigurationDocuments>();

                            while (dr.Read())
                            {
                                lstCD.Add(new ConfigurationDocuments()
                                {
                                    attachmentName = (dr.IsDBNull(_AttachmentName)) == true ? "" : Convert.ToString(dr[_AttachmentName]).Trim(),
                                    configurationDocId = (dr.IsDBNull(_ConfigurationDocument_ID)) == true ? "" : Convert.ToString(dr[_ConfigurationDocument_ID]).Trim(),
                                    fileDescription = (dr.IsDBNull(_FileDescription)) == true ? "" : Convert.ToString(dr[_FileDescription]).Trim(),
                                    projectId = (dr.IsDBNull(_Project_Id)) == true ? "" : Convert.ToString(dr[_Project_Id]).Trim(),
                                    filePath = (dr.IsDBNull(_FilePath)) == true ? "" : Convert.ToString(dr[_FilePath]).Trim(),
                                    fileName = (dr.IsDBNull(_FileName)) == true ? "" : Convert.ToString(dr[_FileName]).Trim(),
                                });
                            }
                        }
                    }
                    return Json(lstCD);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }


        [HttpGet, Route("GetProcessDetail/{projectId}")]
        public JsonResult GetProcessDetail(string projectId)
        {
            List<ProcessDetail> lstPD = null;
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;



                if (string.IsNullOrEmpty(projectId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spProcessDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Select" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                        {
                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No Data Found");
                            return Json(oError);
                        }
                        else
                        {
                            int _AsIs = dr.GetOrdinal("AsIs");
                            int _Project_Id = dr.GetOrdinal("Project_Id");
                            int _ToBe = dr.GetOrdinal("ToBe");
                            int _AsIsDescription = dr.GetOrdinal("AsIsDescription");
                            int _ToBeDescription = dr.GetOrdinal("ToBeDescription");
                            int _ProcessDetail_ID = dr.GetOrdinal("ProcessDetail_ID");


                            lstPD = new List<ProcessDetail>();

                            while (dr.Read())
                            {
                                lstPD.Add(new ProcessDetail()
                                {
                                    asIs = (dr.IsDBNull(_AsIs)) == true ? "" : Convert.ToString(dr[_AsIs]).Trim(),
                                    asIsDescription = (dr.IsDBNull(_AsIsDescription)) == true ? "" : Convert.ToString(dr[_AsIsDescription]).Trim(),
                                    processDetailId = (dr.IsDBNull(_ProcessDetail_ID)) == true ? "" : Convert.ToString(dr[_ProcessDetail_ID]).Trim(),
                                    projectId = (dr.IsDBNull(_Project_Id)) == true ? "" : Convert.ToString(dr[_Project_Id]).Trim(),
                                    toBe = (dr.IsDBNull(_ToBe)) == true ? "" : Convert.ToString(dr[_ToBe]).Trim(),
                                    toBeDescription = (dr.IsDBNull(_ToBeDescription)) == true ? "" : Convert.ToString(dr[_ToBeDescription]).Trim(),
                                });
                            }
                        }
                    }
                    return Json(lstPD);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }


        [HttpPost, Route("UpdateGSConfig")]
        public JsonResult UpdateGSConfig([FromBody]GeneralSetting oGeneralSetting)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                Dictionary<string, string> oError = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(oGeneralSetting.testPassId))
                {
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }
                if (!string.IsNullOrEmpty(oGeneralSetting.feedbackType) && !"1,2,0".Contains(oGeneralSetting.feedbackType.ToUpper().Trim()))
                {
                    oError.Add(this._errorText, "feedback type can be NULL or have values 0/1/2 !");
                    return Json(oError);
                }
                if (!string.IsNullOrEmpty(oGeneralSetting.testingType) && !"1,2,0".Contains(oGeneralSetting.testingType))
                {
                    oError.Add(this._errorText, "testingType can be NULL or have values 0/1/2 !");
                    return Json(oError);
                }
                if (!string.IsNullOrEmpty(oGeneralSetting.testStepUnique) && !"Y,N".Contains(oGeneralSetting.testStepUnique))
                {
                    oError.Add(this._errorText, "testStepUnique can be NULL or have values Y,N !");
                    return Json(oError);
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = oGeneralSetting.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@Feedback", SqlDbType.Int) { Value = Convert.ToInt32(oGeneralSetting.feedbackType) });
                    cmd.Parameters.Add(new SqlParameter("@TestingType", SqlDbType.VarChar) { Value = oGeneralSetting.testingType });
                    cmd.Parameters.Add(new SqlParameter("@TSNameConstraint", SqlDbType.VarChar) { Value = oGeneralSetting.testStepUnique });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "GSUPDATE" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    //var resp = _context.Database.ExecuteN();
                    //return Json(ReturnVal.Value);

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;
                    //if (_res > 0)
                    //    _result.Add(this._statusText, "Done");
                    //else
                    //    _result.Add(this._errorText, OutResult.ToString());


                    if (outparam.Value.ToString().ToLower() == "success" || outparam.Value.ToString().ToLower() == "testing has begun")
                    {//update case
                        if (outparam.Value.ToString().ToLower() == "testing has begun")
                            _result.Add(this._statusText, "Testing has begun");
                        else
                            _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, outparam.Value.ToString());
                    }


                }

            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }


        [HttpPost, Route("InsertUserSetting")]
        public JsonResult InsertUserSetting([FromBody]UserSettingReqObj oUserSettingReqObj)
        {
            /*
             Insert : ENVID~USERID#ENVID~USERID#ENVID~USERID#ENVID~USERID#ENVID~USERID 
             Update : ENVID~USERID^userSettingId#ENVID~USERID^userSettingId#ENVID~USERID^userSettingId#ENVID~USERID^userSettingId#ENVID~USERID^userSettingId
             */
            Dictionary<string, string> _result = new Dictionary<string, string>();

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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;
                Dictionary<string, string> oError = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;
                if (string.IsNullOrEmpty(oUserSettingReqObj.projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oUserSettingReqObj.testPassId))
                {
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oUserSettingReqObj.UserSettingString))
                {
                    oError.Add(this._errorText, "UserSettingString is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spUserSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = oUserSettingReqObj.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(oUserSettingReqObj.projectId) });

                    if (oUserSettingReqObj.UserSettingString[oUserSettingReqObj.UserSettingString.Length - 1] == '#')
                    {
                        oUserSettingReqObj.UserSettingString = oUserSettingReqObj.UserSettingString.Substring(0, oUserSettingReqObj.UserSettingString.Length - 1);
                    }
                    cmd.Parameters.Add(new SqlParameter("@SettingString", SqlDbType.VarChar) { Value = oUserSettingReqObj.UserSettingString });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = (oUserSettingReqObj.UserSettingString.Contains('^') == false) ? "INSERT" : "UPDATE" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;

                    string[] arr = OutResult.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 1)
                    {//insert
                        _result.Add(this._statusText, "Done");
                        _result.Add("ID", OutResult.ToString());
                    }
                    else
                        _result.Add(this._errorText, OutResult.ToString());

                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpPost, Route("UpdateUserSetting")]
        public JsonResult UpdateUserSetting([FromBody]UserSettingReqObj oUserSettingReqObj)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                Dictionary<string, string> oError = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(oUserSettingReqObj.projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oUserSettingReqObj.testPassId))
                {
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oUserSettingReqObj.userId))
                {
                    oError.Add(this._errorText, "UserID is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oUserSettingReqObj.UserSettingString))
                {
                    oError.Add(this._errorText, "UserSettingString is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spUserSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = oUserSettingReqObj.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@User_Id", SqlDbType.Int) { Value = oUserSettingReqObj.userId });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(oUserSettingReqObj.projectId) });

                    if (oUserSettingReqObj.UserSettingString[oUserSettingReqObj.UserSettingString.Length - 1] == ',')
                    {
                        oUserSettingReqObj.UserSettingString = oUserSettingReqObj.UserSettingString.Substring(0, oUserSettingReqObj.UserSettingString.Length - 1);
                    }
                    cmd.Parameters.Add(new SqlParameter("@SettingString", SqlDbType.VarChar) { Value = oUserSettingReqObj.UserSettingString });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "UPDATE" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;


                    if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        string[] arr = OutResult.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 1)
                        {//insert
                            _result.Add(this._statusText, "Done");
                            _result.Add("ID", OutResult.ToString());
                        }
                        else
                            _result.Add(this._errorText, OutResult.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpDelete, Route("DeleteUserSetting")]
        public JsonResult DeleteUserSetting([FromBody]UserSettingReqObj oUserSettingReqObj)
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                return Json("Invalid Url");
            }

            string _clientSchemaName = SchemaName;

            if (string.IsNullOrEmpty(_clientSchemaName))
                return null;

            Dictionary<string, string> _result = new Dictionary<string, string>();

            Dictionary<string, string> oError = new Dictionary<string, string>();
            //validate mandatory fields
            if (string.IsNullOrEmpty(oUserSettingReqObj.testPassId) || string.IsNullOrEmpty(oUserSettingReqObj.userId))
            {
                oError.Add(this._errorText, "testPassId and userId is required");
                return Json(oError);
            }
            try
            {

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spUserSetting";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = oUserSettingReqObj.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@User_Id", SqlDbType.Int) { Value = oUserSettingReqObj.userId });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar) { Value = new clsUtility().GetLoggedInUserSPUserId() });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;


                    if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, OutResult.ToString());
                    }
                }
                return Json(_result);
            }
            catch (Exception ex)
            {
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }


        [HttpDelete, Route("DeleteEnvironment/{envID}")]
        public JsonResult DeleteEnvironment(string envID)
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                return Json("Invalid Url");
            }

            string _clientSchemaName = SchemaName;

            if (string.IsNullOrEmpty(_clientSchemaName))
                return null;

            Dictionary<string, string> _result = new Dictionary<string, string>();

            Dictionary<string, string> oError = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(envID))
            {
                oError.Add(this._errorText, "Enviornment Id is required");
                return Json(oError);
            }
            try
            {

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spEnvironment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@EnvironmentId", SqlDbType.Int) { Value = envID });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar) { Value = new clsUtility().GetLoggedInUserSPUserId() });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;


                    if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, OutResult.ToString());
                    }
                }
                return Json(_result);

            }
            catch (Exception ex)
            {
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpDelete, Route("DeleteProcessDetail/{processDetailId}")]
        public JsonResult DeleteProcessDetail(string processDetailId)
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                return Json("Invalid Url");
            }

            string _clientSchemaName = SchemaName;

            if (string.IsNullOrEmpty(_clientSchemaName))
                return null;

            Dictionary<string, string> _result = new Dictionary<string, string>();

            Dictionary<string, string> oError = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(processDetailId))
            {
                oError.Add(this._errorText, "processDetailId is required");
                return Json(oError);
            }
            try
            {

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spProcessDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProcessDetailId", SqlDbType.Int) { Value = Convert.ToInt32(processDetailId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar) { Value = new clsUtility().GetLoggedInUserSPUserId() });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;


                    if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, OutResult.ToString());
                    }
                }
                return Json(_result);

            }
            catch (Exception ex)
            {
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpPost, Route("InsertUpdateEnvironment")]
        public JsonResult InsertUpdateEnvironment([FromBody]Environment_Tab OEnvironment_Tab)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();

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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;


                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;
                Dictionary<string, string> oError = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(OEnvironment_Tab.projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(OEnvironment_Tab.testPassId))
                {
                    oError.Add(this._errorText, "TestPass Id is required!");
                    return Json(oError);
                }
                else if (string.IsNullOrEmpty(OEnvironment_Tab.actualUrl))
                {
                    oError.Add(this._errorText, "Actual URL is required!");
                    return Json(oError);
                }
                else if (string.IsNullOrEmpty(OEnvironment_Tab.aliasUrl))
                {
                    oError.Add(this._errorText, "Alias URL is required!");
                    return Json(oError);
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spEnvironment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = OEnvironment_Tab.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = OEnvironment_Tab.projectId });
                    cmd.Parameters.Add(new SqlParameter("@ActualURL", SqlDbType.NVarChar) { Value = OEnvironment_Tab.actualUrl });
                    cmd.Parameters.Add(new SqlParameter("@AliasURL", SqlDbType.NVarChar) { Value = OEnvironment_Tab.aliasUrl });
                    cmd.Parameters.Add(new SqlParameter("@EnvironmentId", SqlDbType.Int) { Value = OEnvironment_Tab.envID });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = (string.IsNullOrEmpty(OEnvironment_Tab.envID) ? "Insert" : "Update") });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar) { Value = new clsUtility().GetLoggedInUserSPUserId() });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;

                    if (string.IsNullOrEmpty(OEnvironment_Tab.envID))
                    {
                        int result = 0;
                        int.TryParse(OutResult.ToString(), out result);

                        if (result > 0)
                        {
                            _result.Add(this._statusText, "Done");
                            _result.Add("ID", OutResult.ToString());
                        }
                    }
                    else if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                        
                    }
                    else
                    {
                        _result.Add(this._errorText, OutResult.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpPost, Route("InsertUpdateProcessDetail")]
        public JsonResult InsertUpdateProcessDetail([FromBody]ProcessDetail oProcessDetail)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
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
                    return Json("Invalid Url");
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                Dictionary<string, string> oError = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(oProcessDetail.projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oProcessDetail.asIs))
                {
                    oError.Add(this._errorText, "AsIs is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(oProcessDetail.toBe))
                {
                    oError.Add(this._errorText, "ToBe is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spProcessDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = oProcessDetail.projectId });
                    cmd.Parameters.Add(new SqlParameter("@ProcessDetailId", SqlDbType.Int) { Value = oProcessDetail.processDetailId });
                    cmd.Parameters.Add(new SqlParameter("@AsIs", SqlDbType.VarChar) { Value = oProcessDetail.asIs });
                    cmd.Parameters.Add(new SqlParameter("@ToBe", SqlDbType.VarChar) { Value = oProcessDetail.toBe });
                    cmd.Parameters.Add(new SqlParameter("@AsIsDesc", SqlDbType.VarChar) { Value = oProcessDetail.asIsDescription });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = (string.IsNullOrEmpty(oProcessDetail.processDetailId) ? "Insert" : "Update") });
                    cmd.Parameters.Add(new SqlParameter("@ToBeDesc", SqlDbType.VarChar) { Value = oProcessDetail.toBeDescription });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;

                    if (OutResult.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        string[] arr = OutResult.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 1)
                        {//insert
                            _result.Add(this._statusText, "Done");
                            _result.Add("ID", OutResult.ToString());
                        }
                        else
                            _result.Add(this._errorText, OutResult.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }


        [HttpPost, Route("UploadConfigFileData")]
        public JsonResult UploadConfigFileData([FromBody]ConfigurationAttachment attachment)
        {

            /********************/
            string AppUrl = HttpContext.Request.Headers["appurl"];
            Dictionary<string, string> _result = new Dictionary<string, string>();
            Dictionary<string, string> oError = new Dictionary<string, string>();

            try
            {
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    oError.Add(this._errorText, "Invalid Url");
                    return Json(oError);
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                {
                    oError.Add(this._errorText, "Schema not found");
                    return Json(oError);
                }

                if (string.IsNullOrEmpty(attachment.projectId))
                {
                    oError.Add(this._errorText, "Project Id is required!");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(attachment.AttachmentName))
                {
                    oError.Add(this._errorText, "Attachment Name is required!");
                    return Json(oError);
                }

                int _res = 0;
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    string _IUFlag = (string.IsNullOrEmpty(attachment.AttachmentId)) ? "InsertData" : "UpdateData";

                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AttachmentName", SqlDbType.VarChar) { Value = attachment.AttachmentName });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = attachment.Description });
                    cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar) { Value = attachment.FileName });
                    cmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.Int) { Value = Convert.ToInt32(attachment.projectId) });
                    //cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar) { Value = attachment.ContentType });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.NVarChar, 10) { Value = _IUFlag });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = SchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;
                    int i_result = 0; int.TryParse(OutResult.ToString(), out i_result);
                    if (i_result > 0)
                    {
                        _result.Add(this._statusText, "Done");
                        _result.Add("ID", i_result.ToString());
                    }
                    else
                        _result.Add(this._errorText, OutResult.ToString());
                }
            }
            catch (Exception ex)
            {
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        //actual file upload
        [HttpPost, Route("UploadConfigurationFile")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public IActionResult UploadConfigurationFile()
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            bool isUpload = false;
            try
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
                    isUpload = false;

                }

                int new_AttachmentId = Convert.ToInt32(HttpContext.Request.Form["attaid"]);

                byte[] byt;
                using (var reader = new StreamReader(HttpContext.Request.Form.Files["file"].OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();

                    BinaryReader br = new BinaryReader(HttpContext.Request.Form.Files["file"].OpenReadStream());
                    byt = br.ReadBytes((Int32)fileContent.Length);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = new_AttachmentId });
                    cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar, 500) { Value = HttpContext.Request.Form.Files["file"].ContentType });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.NVarChar, 10) { Value = "InsertFile" });
                    cmd.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary, 5000000) { Value = byt });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = SchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = cmd.ExecuteNonQuery();
                    string outval = outparam.Value.ToString();

                    if (outval.ToUpper().Trim() == "SUCCESS")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, "Upload failed");
                    }

                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }

            return Json(isUpload);

        }

        [HttpPost, Route("UploadConfigurationFileAsync")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public async Task<IActionResult> UploadConfigurationFileAsync()
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            bool isUpload = false;
            try
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
                    isUpload = false;

                }

                int new_AttachmentId = Convert.ToInt32(HttpContext.Request.Form["attaid"]);

                byte[] byt;
                using (var reader = new StreamReader(HttpContext.Request.Form.Files["file"].OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();

                    BinaryReader br = new BinaryReader(HttpContext.Request.Form.Files["file"].OpenReadStream());
                    byt = br.ReadBytes((Int32)fileContent.Length);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = new_AttachmentId });
                    cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar, 500) { Value = HttpContext.Request.Form.Files["file"].ContentType });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.NVarChar, 10) { Value = "InsertFile" });
                    cmd.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary, 5000000) { Value = byt });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = SchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = await (cmd.ExecuteNonQueryAsync());
                    string outval = outparam.Value.ToString();

                    if (outval.ToUpper().Trim() == "SUCCESS")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        _result.Add(this._errorText, "Upload failed");
                    }

                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }

            return Json(isUpload);

        }

        [HttpGet, Route("GetConfigurationAttachment")]
        public FileContentResult GetConfigurationAttachment(int id, string Url)
        {

            byte[] fileContent = null;
            try
            {
                string SchemaName = "";
                
                string AppUrl = Url;

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
                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = id });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.NVarChar, 10) { Value = "GetAttach" });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["FileName"].ToString();
                            contentType = dataReader["contenttype"].ToString();
                            fileContent = (byte[])dataReader["FileData"];
                            Response.ContentType = dataReader["ContentType"].ToString();
                            Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["FileName"].ToString());
                            Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                        }

                        return File(fileContent, contentType, filename);
                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        [HttpGet, Route("GetConfigurationAttachmentDetails/{projectId}")]
        public JsonResult GetConfigurationAttachmentDetails(int projectId)
        {
            List<ConfigurationAttachment> lstConfigurationAttachment = new List<ConfigurationAttachment>();
            //byte[] fileContent = null;
            try
            {
                string SchemaName = "";

                string AppUrl =  HttpContext.Request.Headers["appurl"];
                
                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {

                }
               
                if (string.IsNullOrEmpty(Convert.ToString(projectId)))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Project Id is needed");
                    return Json(oError);
                }
                if (string.IsNullOrEmpty(SchemaName))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Schema not found");
                    return Json(oError);
                }

                var filename = "";
                var contentType = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 10) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int, 15) { Value = projectId });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.NVarChar, 10) { Value = "Select" });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            lstConfigurationAttachment.Add(new ConfigurationAttachment {
                                 AttachmentId = dataReader["AttachmentId"].ToString(),
                                 AttachmentName = dataReader["AttachmentName"].ToString(),
                                 Description = dataReader["Description"].ToString(),
                                 FileName = dataReader["FileName"].ToString(),
                                 projectId = dataReader["ProjectID"].ToString(),
                                 ContentType = ""
                            });
                        }
                        return Json(lstConfigurationAttachment);
                    }

                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }

        [HttpDelete,Route("DeleteConfigAttachment/{AttId}/{projectId}")]
        public JsonResult DeleteConfigAttachment(string AttId, string projectId)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
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
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Invalid Url");
                    return Json(oError);
                }

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

               

                if (string.IsNullOrEmpty(AttId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "AttachmentId is required!");
                    return Json(oError);
                }

                if (string.IsNullOrEmpty(projectId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "projectId is required!");
                    return Json(oError);
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spConfigurationAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int) { Value = Convert.ToInt32(AttId)});
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "DelSingle" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    //var resp = _context.Database.ExecuteN();
                    //return Json(ReturnVal.Value);
                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutValue = outparam.Value;



                    //if (Convert.ToInt32(OutValue.ToString()) > 0)
                    //    _result.Add(this._statusText, "DONE");
                    //else
                    //    _result.Add(this._errorText, OutValue.ToString());

                    if (!string.IsNullOrEmpty(OutValue.ToString()) && OutValue.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                        return Json(_result);
                    }
                    else
                    {
                        Dictionary<string, string> oError = new Dictionary<string, string>();
                        oError.Add(this._errorText, OutValue.ToString());
                        return Json(oError);
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }
    }
}
