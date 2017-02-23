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
using System.Data.Common;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class TesterController : Controller
    {

        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public TesterController(clsDbContext context)
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
                            //   continue;
                            areaResult.Add(new Area()
                            {
                                areaId = (dr["areaId"] == null) ? "" : Convert.ToString(dr["areaId"]),
                                areaName = (dr["areaName"] == null) ? "" : Convert.ToString(dr["areaName"]),
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                return null;
            }
            return areaResult;
        }
        #endregion


        #region Role Operations

        //[HttpGet, Route("GetTestCaseDetailForTestPassId/{testPassId}")]
        //public JsonResult GetTestCaseDetailForTestPassId(string testPassId)

        //{
        //    try
        //    {
        //        string AppUrl = HttpContext.Request.Headers["appurl"];
        //        string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
        //        string SchemaName = "";
        //        if (!string.IsNullOrEmpty(AppUrl))
        //        {
        //            SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
        //        }
        //        else
        //        {
        //            return Json("Invalid Appurl");

        //        }

        //        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
        //        {
        //            cmd.CommandText = "UAT.spTester";
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });
        //            cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = SpUserId });
        //            cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Check" });
        //            SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
        //            cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
        //            cmd.Parameters.Add(outparam);
        //            if (cmd.Connection.State != ConnectionState.Open)
        //                cmd.Connection.Open();
        //            var retObject = new List<dynamic>();

        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                if (dr.HasRows)
        //                {

        //                    while (dr.Read())
        //                    {
        //                        var dataRow = new ExpandoObject() as IDictionary<string, object>;

        //                        for (var iFiled = 0; iFiled < dr.FieldCount; iFiled++)
        //                        {

        //                            var value = dr.GetName(iFiled);
        //                            var name = dr.IsDBNull(iFiled) ? null : dr[iFiled];

        //                            dataRow.Add(value, Convert.ToString(name));
        //                        }


        //                        retObject.Add((ExpandoObject)dataRow);
        //                    }



        //                }



        //            }

        //            return Json(retObject);


        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }
        //}

        [HttpGet, Route("GetTestCaseDetailForTestPassId/{testPassId}")]
        public List<TestCase> GetTestCaseDetailForTestPassId(string testPassId)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<TestCase> listTestCaseDetails = new List<TestCase>();
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
                    return null;
                }


                if (string.IsNullOrEmpty(testPassId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "Test Pass Id is required!");
                    return null;
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.SpTestCase";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Select" });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (_retValue.ToLower() == "success")
                    {
                        var retObject = new List<dynamic>();

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                //var dataRow = new ExpandoObject() as IDictionary<string, object>;
                                //for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                                //{
                                //    dataRow.Add(dataReader.GetName(iFiled), dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled]);
                                //}
                                listTestCaseDetails.Add(new TestCase
                                {
                                    testCaseId = (dr["TestCase_Id"] == null) ? "" : Convert.ToString(dr["TestCase_Id"]),
                                    testPassId = (dr["TestPass_Id"] == null) ? "" : Convert.ToString(dr["TestPass_Id"]),
                                    testCaseDisplayId = (dr["DisplayTestCase_Id"] == null) ? "" : Convert.ToString(dr["DisplayTestCase_Id"]),
                                    testCaseName = (dr["TestCase_Name"] == null) ? "" : Convert.ToString(dr["TestCase_Name"]),
                                    testCaseDesp = (dr["TestCase_Description"] == null) ? "" : Convert.ToString(dr["TestCase_Description"]),
                                    testCaseSeq = (dr["Testcase_Sequence"] == null) ? "" : Convert.ToString(dr["Testcase_Sequence"]),
                                    testCaseETT = (dr["ETT"] == null) ? "" : Convert.ToString(dr["ETT"])
                                });
                                // retObject.Add((ExpandoObject)dataRow);
                            }
                        }
                    }
                    else
                    {
                        _result.Add(this._statusText, "No Data Found");
                        return listTestCaseDetails;

                    }
                        return listTestCaseDetails;
                    }

                }
            
    
            catch (Exception ex)
            {
                _result.Add(this._errorText, ex.Message);

            }

            return listTestCaseDetails;
            //  return null;
        }

        [HttpPost, Route("InsertUpdateRole")]
        public Dictionary<string, string> InsertUpdateRole([FromBody]Role role)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();


            try
            {
                if (string.IsNullOrEmpty(role.roleName))
                {
                    _result.Add(this._errorText, ("Role Name is required!"));
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
                if (string.IsNullOrEmpty(role.roleId))
                    statementType = "Insert";
                else
                    statementType = "Update";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.spRole";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = role.projectId });
                    cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = role.roleId });
                    cmd.Parameters.Add(new SqlParameter("@RoleName", SqlDbType.VarChar, 500) { Value = role.roleName });
                    cmd.Parameters.Add(new SqlParameter("@RoleDetail", SqlDbType.VarChar, 500) { Value = role.roleDetails });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 500) { Value = statementType });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (!string.IsNullOrEmpty(ReturnParamValue) || ReturnParamValue.ToLower() == "success")
                    {
                        if (string.IsNullOrEmpty(role.roleId))
                        {
                            _result.Add("roleId", cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString());
                            _result.Add(this._statusText, "Done");
                        }
                        else
                            _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                        foreach (System.Data.Common.DbParameter outP in _outParameter)
                            if (outP.Direction == ParameterDirection.Output)
                                _outParameter.Add(outP);

                        _result.Add(this._errorText, (ReturnParamValue));

                    }

                }


            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

        }


        [HttpDelete, Route("DeleteRole")]
        //public Dictionary<string, string> DeleteRole(string roleId)
        public Dictionary<string, string> DeleteRole([FromBody]Role r1)


        {
            Dictionary<string, string> _result = new Dictionary<string, string>();

            try
            {
                List<TestCase> listProjectUsers = new List<TestCase>();
                if (string.IsNullOrEmpty(r1.roleId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "Test case Id is required!");
                    return _result;

                }

                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
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


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "[UAT].[spRole]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = r1.roleId });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    if (retValPos != 0)
                    {
                        string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                        if (!string.IsNullOrEmpty(ReturnParamValue) && ReturnParamValue.ToLower() == "success")
                        {
                            _result.Add(this._statusText, "Done");
                        }
                        else
                        {
                            //List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                            //_outParameter.Add(cmd.Parameters["@Ret_Parameter"]);
                            //_result.Add(this._errorText, _outParameter.ToString());
                            _result.Add(this._statusText, "role cannot be deleted! testers are assigned with selected roleid.");
                        }
                    }

                    else
                    {
                        _result.Add(this._statusText, " Sorry!Something went wrong, please try again!");


                    }
                }
            }

            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

        }

        #endregion


        #region Tester Operations



        [HttpPost, Route("InsertUpdateTester")]
        public Dictionary<string, string> InsertUpdateTester([FromBody]TestPassTesterRoles tester)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            try
            {

                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (string.IsNullOrEmpty(tester.oldTesterspUserId))
                    tester.oldTesterspUserId = tester.spUserId;

                if (string.IsNullOrEmpty(tester.spUserId))
                {
                    _result.Add(this._errorText, "spUserId is required");
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
                TesterTypeCollection TesterTypeDataTable = new TesterTypeCollection();
                TesterTypeDataTable = generateTesterTable(tester);
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    string statementType = string.Empty;
                    if (tester.action.ToLower() == "add")
                        statementType = "Insert";
                    else
                        statementType = "Update";


                    cmd.CommandText = "UAT.spTester";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TesterTypeCollection", SqlDbType.Structured) { Value = TesterTypeDataTable });
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = Convert.ToInt32(tester.testPassId) });
                    cmd.Parameters.Add(new SqlParameter("@UserName", SqlDbType.VarChar, 500) { Value = tester.testerName });
                    cmd.Parameters.Add(new SqlParameter("@UserAlias", SqlDbType.VarChar, 500) { Value = tester.testerAlias });
                    cmd.Parameters.Add(new SqlParameter("@UserEmailId", SqlDbType.VarChar, 500) { Value = tester.testerEmail });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = tester.spUserId });
                    cmd.Parameters.Add(new SqlParameter("@OldSPUserId", SqlDbType.Int) { Value = Convert.ToInt32(tester.oldTesterspUserId) });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = Convert.ToInt32(SpUserId) });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 500) { Value = statementType });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    if (retValPos != 0)
                    {

                        string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                        if (!string.IsNullOrEmpty(ReturnParamValue) && ReturnParamValue.ToLower() == "success")
                        {
                            //_result.Add("testerId", parameter[parameter.Count - 3].Value.ToString());
                            _result.Add(this._statusText, "Done");
                        }
                        else
                        {
                            _result.Add(this._statusText, "Sorry! Something went wrong, please try again!");
                        }

                    }
                    else
                    {
                        List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                        foreach (System.Data.Common.DbParameter outP in _outParameter)
                            if (outP.Direction == ParameterDirection.Output)
                                _outParameter.Add(outP);

                        _result.Add(this._errorText, "Sorry! Something went wrong, please try again!");


                    }
                }
            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

        }

        //if (!string.IsNullOrEmpty(ReturnParamValue) || ReturnParamValue.ToLower() == "success")
        //{
        //    if (string.IsNullOrEmpty(tester.testPassId))
        //    {
        //        _result.Add("roleId", cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString());
        //        _result.Add(this._statusText, "Done");
        //    }
        //    else
        //        _result.Add(this._statusText, "Done");
        //}

        //    else
        // //{
        //    List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
        //    foreach (System.Data.Common.DbParameter outP in _outParameter)
        //        if (outP.Direction == ParameterDirection.Output)
        //            _outParameter.Add(outP);

        //    _result.Add(this._errorText, "Error ");


        //}


        // }

        //  }



        //[HttpGet, Route("GetTestersByTestPassID/{testPassId}")]
        //public List<TestPassTesterRoles> GetTestersByTestPassID(string testPassId)
        //{
        //    {
        //        try
        //        {
        //            string AppUrl = HttpContext.Request.Headers["appurl"];
        //            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
        //            string SchemaName = "";
        //            List<TestPassTesterRoles> testersResult = new List<TestPassTesterRoles>();
        //            if (!string.IsNullOrEmpty(AppUrl))
        //            {
        //                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
        //            }
        //            else
        //            {
        //                return null;

        //            }

        //            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
        //            {
        //                cmd.CommandText = "UAT.spGetTestersByTestPassIds";
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });


        //                SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
        //                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
        //                cmd.Parameters.Add(outparam);
        //                if (cmd.Connection.State != ConnectionState.Open)
        //                    cmd.Connection.Open();
        //                var retObject = new List<dynamic>();

        //                using (var dr = cmd.ExecuteReader())
        //                {
        //                    if (dr.HasRows)
        //                    {

        //                        while (dr.Read())
        //                        {

        //                            string _testPassId = string.Empty, _spUserId = string.Empty, _areaId = string.Empty, _isTestingInProgress = string.Empty;
        //                            string _isTestStepAssigned = string.Empty;
        //                            _testPassId = (dr["testPassId"] == null) ? "" : Convert.ToString(dr["testPassId"]);
        //                            _spUserId = (dr["spUserId"] == null) ? "" : Convert.ToString(dr["spUserId"]);
        //                            _areaId = (dr["areaId"] == null) ? "" : Convert.ToString(dr["areaId"]);

        //                            _isTestingInProgress = int.Parse(dr["totalPassFailCount"].ToString()) == 0 ? "0" : "1";
        //                            _isTestStepAssigned = int.Parse(dr["totalAllStatusCount"].ToString()) == 0 ? "0" : "1";//:SD

        //                            var tester = testersResult.Where(t => t.testPassId == _testPassId && t.spUserId == _spUserId && t.areaId == _areaId).ToList();

        //                            List<TestPassTesterRoles.Role> _role = new List<TestPassTesterRoles.Role>();

        //                            if (tester.Count == 0)
        //                            {
        //                                _role.Add(new TestPassTesterRoles.Role()
        //                                {
        //                                    roleId = (dr["roleId"] == null) ? "" : Convert.ToString(dr["roleId"]),
        //                                    roleName = (dr["roleName"] == null) ? "" : Convert.ToString(dr["roleName"]),

        //                                    isTestingInprogress = _isTestingInProgress,
        //                                    isTestStepsAssigned = _isTestStepAssigned
        //                                });

        //                                testersResult.Add(new TestPassTesterRoles()
        //                                {
        //                                    testPassId = _testPassId,
        //                                    testerID = (dr["testerID"] == null) ? "" : Convert.ToString(dr["testerID"]),

        //                                    spUserId = _spUserId,

        //                                    testerName = (dr["testerName"] == null) ? "" : Convert.ToString(dr["testerName"]),
        //                                    testerAlias = (dr["testerAlias"] == null) ? "" : Convert.ToString(dr["testerAlias"]),
        //                                    testerEmail = (dr["testerEmail"] == null) ? "" : Convert.ToString(dr["testerEmail"]),

        //                                    areaId = _areaId,
        //                                    areaName = (dr["areaName"] == null) ? "" : Convert.ToString(dr["areaName"]),

        //                                    roleList = _role,

        //                                });
        //                            }
        //                            else
        //                            {
        //                                tester.First().roleList.Add(new TestPassTesterRoles.Role()
        //                                {
        //                                    roleId = (dr["roleId"] == null) ? "" : Convert.ToString(dr["roleId"]),
        //                                    roleName = (dr["roleName"] == null) ? "" : Convert.ToString(dr["roleName"]),

        //                                    isTestingInprogress = _isTestingInProgress,
        //                                    isTestStepsAssigned = _isTestStepAssigned//:SD
        //                                });
        //                            }

        //                        }



        //                    }



        //                }

        //                return testersResult;


        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            return null;
        //        }

        //    }
        //}



        [HttpGet, Route("GetTestersByTestPassID/{testPassId}")]
        public List<TestPassTesterRoles> GetTestersByTestPassID(string testPassId)
        {
            {
                try
                {
                    string AppUrl = HttpContext.Request.Headers["appurl"];
                    string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                    string SchemaName = "";
                    List<TestPassTesterRoles> testersResult = new List<TestPassTesterRoles>();
                    if (!string.IsNullOrEmpty(AppUrl))
                    {
                        SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                    }
                    else
                    {
                        return null;

                    }

                    using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "UAT.spGetTestersByTestPassIds";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });


                        SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                        cmd.Parameters.Add(outparam);
                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();
                        var retObject = new List<dynamic>();

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {

                                while (dr.Read())
                                {

                                    string _testPassId = string.Empty, _spUserId = string.Empty, _areaId = string.Empty, _isTestingInProgress = string.Empty;
                                    string _isTestStepAssigned = string.Empty;
                                    _testPassId = (dr["testPassId"] == null) ? "" : Convert.ToString(dr["testPassId"]);
                                    _spUserId = (dr["spUserId"] == null) ? "" : Convert.ToString(dr["spUserId"]);
                                    _areaId = (dr["areaId"] == null) ? "" : Convert.ToString(dr["areaId"]);

                                    _isTestingInProgress = int.Parse(dr["totalPassFailCount"].ToString()) == 0 ? "0" : "1";
                                    _isTestStepAssigned = int.Parse(dr["totalAllStatusCount"].ToString()) == 0 ? "0" : "1";//:SD

                                    var tester = testersResult.Where(t => t.testPassId == _testPassId && t.spUserId == _spUserId && t.areaId == _areaId).ToList();

                                    List<TestPassTesterRoles.Role> _role = new List<TestPassTesterRoles.Role>();

                                    if (tester.Count == 0)
                                    {
                                        _role.Add(new TestPassTesterRoles.Role()
                                        {
                                            roleId = (dr["roleId"] == null) ? "" : Convert.ToString(dr["roleId"]),
                                            roleName = (dr["roleName"] == null) ? "" : Convert.ToString(dr["roleName"]),

                                            isTestingInprogress = _isTestingInProgress,
                                            isTestStepsAssigned = _isTestStepAssigned
                                        });

                                        testersResult.Add(new TestPassTesterRoles()
                                        {
                                            testPassId = _testPassId,
                                            testerID = (dr["testerID"] == null) ? "" : Convert.ToString(dr["testerID"]),

                                            spUserId = _spUserId,

                                            testerName = (dr["testerName"] == null) ? "" : Convert.ToString(dr["testerName"]),
                                            testerAlias = (dr["testerAlias"] == null) ? "" : Convert.ToString(dr["testerAlias"]),
                                            testerEmail = (dr["testerEmail"] == null) ? "" : Convert.ToString(dr["testerEmail"]),

                                            areaId = _areaId,
                                            areaName = (dr["areaName"] == null) ? "" : Convert.ToString(dr["areaName"]),

                                            roleList = _role,

                                        });
                                    }
                                    else
                                    {
                                        tester.First().roleList.Add(new TestPassTesterRoles.Role()
                                        {
                                            roleId = (dr["roleId"] == null) ? "" : Convert.ToString(dr["roleId"]),
                                            roleName = (dr["roleName"] == null) ? "" : Convert.ToString(dr["roleName"]),

                                            isTestingInprogress = _isTestingInProgress,
                                            isTestStepsAssigned = _isTestStepAssigned//:SD
                                        });
                                    }

                                }



                            }



                        }

                        return testersResult;


                    }
                }
                catch (Exception ex)
                {

                    return null;
                }

            }
        }



        [HttpDelete, Route("DeleteTester")]
        public Dictionary<string, string> DeleteTester([FromBody]ClsTesterParmcs tstpr)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string _SpUserID = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            if (string.IsNullOrEmpty(tstpr.testPassId) || string.IsNullOrEmpty(tstpr.testPassId))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Tester SP User Id and Test Pass Id is required!");
                return _result;
            }

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
                    _result.Add(this._errorText, "Invalid Appurl");
                    return _result;

                }
                List<ClsTesterTypeDataTable> listTesterType = new List<ClsTesterTypeDataTable>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "UAT.spTester";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = tstpr.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.VarChar, 500) { Value = tstpr.spUserID }); //new UATClient().GetLoggedInUserSPUserId();            
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = _SpUserID });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    if (retValPos != 0)
                    {
                        string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                        if (!string.IsNullOrEmpty(ReturnParamValue) && ReturnParamValue.ToLower() == "success")
                        {
                            _result.Add(this._statusText, "Done");
                        }
                        else
                        {
                            List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                            _outParameter.Add(cmd.Parameters["@Ret_Parameter"]);
                            _result.Add(this._errorText, _outParameter.ToString());
                        }
                    }

                    else
                    {
                        List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                        foreach (System.Data.Common.DbParameter outP in _outParameter)
                            if (outP.Direction == ParameterDirection.Output)
                                _outParameter.Add(outP);
                        _result.Add(this._errorText, _outParameter.ToString());

                        //  _result.Add(this._errorText, E);
                    }
                }
            }

            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;

        }



        public List<TesterType> getTesterByUserIdTestPassId(string oldTesterspUserId, string testPassId)
        {
            List<TesterType> _type = new List<TesterType>();
            try
            {

                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SchemaName = "";
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
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
                    cmd.CommandText = "UAT.spTester";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = testPassId });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.VarChar, 500) { Value = SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Check" });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                _type.Add(new TesterType()
                                {
                                    Area_ID = Convert.ToInt32((string.IsNullOrEmpty(Convert.ToString(dr["Area_ID"])))?"0":Convert.ToString(dr["Area_ID"])),
                                    Role_ID = Convert.ToInt32(dr["Role_ID"]),
                                    Tester_ID = Convert.ToInt32(dr["Tester_ID"]),
                                    TestPass_ID = Convert.ToInt32(dr["TestPass_ID"]),
                                    User_ID = Convert.ToInt32(dr["User_ID"]),

                                });
                            }
                            

                        }
                        //if (_type.Count > 0)
                        //{
                        //    int rowid = 1;
                        //    foreach (var item in _type)
                        //    {

                        //        item.RowID = rowid;
                        //        rowid++;
                        //    }
                        //}


                        return _type;
                       // return _type;

                    }
                }

            }
            catch (Exception ex)
            {

                return null;
            }

        }


        public string GetSingleValue(string TableName, string SelectVal, string WhereClause, string OrderBy)
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            string ReturnVal = "";
            if (string.IsNullOrEmpty(TableName))
                return null;
            if (string.IsNullOrEmpty(SelectVal))
                return null;
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
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
                cmd.CommandText = "UAT.spGetSingleValue";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@TableName", SqlDbType.VarChar, 500) { Value = TableName });
                cmd.Parameters.Add(new SqlParameter("@WhereCondition", SqlDbType.VarChar, 500) { Value = WhereClause });
                cmd.Parameters.Add(new SqlParameter("@SelectValue", SqlDbType.VarChar, 500) { Value = SelectVal });
                cmd.Parameters.Add(new SqlParameter("@OrderBy", SqlDbType.VarChar, 500) { Value = OrderBy });
                SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                cmd.Parameters.Add(outparam);
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        ReturnVal = Convert.ToString(dr[0]);

                    }
                    return ReturnVal;

                }
            }
        }


        public TesterTypeCollection generateTesterTable(TestPassTesterRoles tester)
        {
            TesterTypeCollection TesterTypeDataTable = new TesterTypeCollection();
            List<TesterType> _lst1 = new List<TesterType>();
            try
            {
                if (tester != null)
                {
                    if (tester.roleArray != null)
                    {
                        if (tester.action.ToLower() == "edit")
                            _lst1 = getTesterByUserIdTestPassId(tester.oldTesterspUserId, tester.testPassId);
                        foreach (var item in _lst1)
                        {
                            string _roleId = item.Role_ID.ToString();
                            if (tester.roleArray.IndexOf(_roleId) == -1)
                                continue;

                            ClsTesterTypeDataTable dr = new ClsTesterTypeDataTable();
                            dr.Tester_ID = item.Tester_ID;
                            dr.TestPass_ID = (tester.testPassId == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.testPassId);
                            dr.Area_ID = (tester.areaId == "") ? Convert.ToInt32(0) : Convert.ToInt32(tester.areaId);
                            dr.Role_ID = item.Role_ID;
                            if (tester.action.ToLower() == "edit")
                                dr.User_ID = (tester.oldTesterspUserId == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.oldTesterspUserId);
                            else
                                dr.User_ID = (tester.spUserId == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.spUserId);
                            TesterTypeDataTable.Add(dr);
                            /*old record added to datatable*/
                        }


                        int flagTesterId = 0;// counter for rowid
                        for (int i = 0; i <= tester.roleArray.Count - 1; i++)
                        {
                            if (tester.action.ToLower() == "edit")
                            {/*checking if new tester role exists for old tester*/
                                List<TesterType> _dr1 = new List<TesterType>();
                                //_dr1= _lst1.Select( "Role_ID = " + tester.roleArray[i]);
                                _dr1 = (from x in _lst1 where x.Role_ID == Convert.ToInt32(tester.roleArray[i]) select x).ToList();

                                if (tester.spUserId.Trim() == tester.oldTesterspUserId.Trim())
                                {
                                    if (_dr1.Count > 0)//a new record is not inserted if same record is changed
                                        continue;
                                }
                            }

                            ClsTesterTypeDataTable dr = new ClsTesterTypeDataTable();

                            dr.Tester_ID = (tester.testerID == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.testerID);
                            if (string.IsNullOrEmpty(tester.testerID) && tester.spUserId.Trim() != tester.oldTesterspUserId.Trim())
                            {/*block to execute when tester has changed*/

                                string whClause = "t.TestPass_ID =" + tester.testPassId + " and t.User_ID =" + tester.oldTesterspUserId + " and t.role_id =" + tester.roleArray[i];
                                try
                                {
                                    //getting testerId of old tester in case of tester update so that old tester gets replaced with new tester details, role is considered here  
                                    string resultVal = GetSingleValue("Testers t", "t.Tester_ID", whClause, null);
                                    if (string.IsNullOrEmpty(resultVal))
                                    {
                                        resultVal = "0";

                                        if (tester.action.ToLower() == "edit")
                                        {
                                            List<TesterType> _dr2 = new List<TesterType>();
                                            _dr2 = (from x in _lst1 where x.Role_ID == Convert.ToInt32(tester.roleArray[i]) select x).ToList();
                                            //DataRow[] dataRowTester = dtEditTester.Select("Role_ID = " + tester.roleArray[i]);

                                            if (_dr2.Count == 0)//a new record is not inserted if same record is changed
                                            {
                                                // role is not considered here, tester id is fetched by rowid from database
                                                whClause = "TestPass_ID = " + tester.testPassId + " and User_ID = " + tester.oldTesterspUserId + ")as temp where row=" + (flagTesterId + 1);
                                                resultVal = GetSingleValue("Testers", "Tester_ID from(select Tester_ID ,row_number() over(order by tester_id) as 'row'", whClause, "");

                                                flagTesterId++;// counter for rowid

                                            }

                                        }

                                        dr.Tester_ID = Convert.ToInt32(resultVal);//can be repetative, which is set to '0' in code block below
                                    }
                                    else
                                    {
                                        dr.Tester_ID = Convert.ToInt32(resultVal);
                                    }

                                }
                                catch (Exception e)
                                {
                                }
                            }

                            dr.TestPass_ID = (tester.testPassId == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.testPassId);
                            int Area_ID = 0;
                            int.TryParse(tester.areaId, out Area_ID);
                            dr.Area_ID = (tester.areaId == "") ? Convert.ToInt32(0) : Convert.ToInt32(Area_ID);

                            dr.Role_ID = Convert.ToInt32(tester.roleArray[i]);
                            dr.User_ID = (tester.spUserId == "") ? Convert.ToInt32(DBNull.Value) : Convert.ToInt32(tester.spUserId);

                            TesterTypeDataTable.Add(dr);
                        }


                    }

                    // r
                    if (tester.action.ToLower() == "edit")
                    {

                        if (tester.spUserId.Trim() != tester.oldTesterspUserId.Trim())
                        {

                            //dtTester = dtTester.AsEnumerable().OrderBy(z => z.Field<int>("Tester_Id")).OrderBy(z => z.Field<int>("Role_Id")).OrderBy(z => z.Field<int>("User_Id")).CopyToDataTable();

                            Dictionary<int, int> oldData = new Dictionary<int, int>();
                            // IEnumerable<DataRow> rows = from row in TesterTypeDataTable.AsQueryable() select row;

                            foreach (var row in TesterTypeDataTable)
                            {/*getting old testers' data for further use in dictionary*/
                                if (row.User_ID.ToString().Trim() == tester.oldTesterspUserId.Trim())
                                {
                                    oldData.Add(row.Tester_ID, row.Role_ID);
                                }
                            }

                            foreach (var row in TesterTypeDataTable)
                            {
                                if (row.User_ID.ToString().Trim() == tester.spUserId.Trim())/*processing datatable for new user only*/
                                {
                                    foreach (KeyValuePair<int, int> pair in oldData)
                                    {
                                        int oldval = pair.Value;
                                        int oldKey = pair.Key;
                                        if (row.Tester_ID == oldKey)
                                        {
                                            if (row.Role_ID != oldval)
                                            {
                                                /*if new users testerID + roleId combination is not same as old tester's combination then set new users tester id as '0' to treat it as new record to be inserted*/
                                                /*if roleId + testerId combination is matched then no change in datatable here, data is updated in sp*/
                                                row.Tester_ID = 0;
                                            }
                                        }
                                    }
                                }
                            }
                            //dtTester = dtTester.AsEnumerable().OrderBy(z => z.Field<int>("RowID")).CopyToDataTable(); /*to bring dttester in original sorting as received*/
                        }

                    }

                }
            }
            catch (Exception)
            {

                throw;
            }


            if (TesterTypeDataTable.Count > 0)
            {
                int rowid = 1;
                foreach (var item in TesterTypeDataTable)
                {

                    item.RowID=rowid;
                    rowid++;
                }
            }


            return TesterTypeDataTable;
        }





        #endregion






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
