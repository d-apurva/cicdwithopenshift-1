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
//Created  by Rupal to perform Test Case Operations

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class TestCaseController : Controller
    {
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public TestCaseController(clsDbContext context)
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


        [HttpGet, Route("GetTestCaseDetailForTestPassId_old/{testPassId}")]
        public List<TestCase> GetTestCaseDetailForTestPassId_old(string testPassId)
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
                    return listTestCaseDetails;
                }


                if (string.IsNullOrEmpty(testPassId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "Test Pass Id is required!");
                    return listTestCaseDetails;
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

                    var retObject = new List<dynamic>();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                            {
                                dataRow.Add(dataReader.GetName(iFiled), dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled]);
                            }

                            retObject.Add((ExpandoObject)dataRow);
                        }
                    }
                    return listTestCaseDetails;
                }

            }
            catch (Exception ex)
            {

                return null;
            }

            // return listTestCaseDetails;


            return listTestCaseDetails;
        }

        [HttpGet, Route("GetTestCaseDetailForTestPassIdWithTesterFlag/{testPassId}")]
        public JsonResult GetTestCaseDetailForTestPassIdWithTesterFlag(string testPassId)
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
                    return Json("Invalid Url");
                }



                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.SpTestCase";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Select" }); ;

                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int retValPos = cmd.ExecuteNonQuery();
                    string retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    using (var dr = cmd.ExecuteReader())
                    {


                        if (dr == null || dr.FieldCount == 0)
                        {

                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add("ERROR", "Incorrect Test Pass Id !");
                            return Json(oError);


                        }
                        else
                        {
                            int TestCase_IdOrdinal = dr.GetOrdinal("TestCase_Id");
                            int TestPass_IdOrdinal = dr.GetOrdinal("TestPass_Id");
                            int DisplayTestCase_IdOrdinal = dr.GetOrdinal("DisplayTestCase_Id");
                            int TestCase_NameOrdinal = dr.GetOrdinal("TestCase_Name");
                            int TestCase_DescriptionOrdinal = dr.GetOrdinal("TestCase_Description");
                            int Testcase_SequenceOrdinal = dr.GetOrdinal("Testcase_Sequence");
                            int ETTOrdinal = dr.GetOrdinal("ETT");


                            string flagTester = "";
                            if (retValue == "SUCCESS")
                            {
                                flagTester = "y";
                            }
                            else if (retValue == "No Tester(s) Assigned!")
                            {
                                flagTester = "n";
                            }

                            while (dr.Read())
                            {
                                listTestCaseDetails.Add(new TestCase()
                                {
                                    testCaseDesp = (dr.IsDBNull(TestCase_DescriptionOrdinal)) == true ? "" : Convert.ToString(dr[TestCase_DescriptionOrdinal]),
                                    testCaseDisplayId = (dr.IsDBNull(DisplayTestCase_IdOrdinal)) == true ? "" : Convert.ToString(dr[DisplayTestCase_IdOrdinal]),
                                    testCaseETT = (dr.IsDBNull(ETTOrdinal)) == true ? "" : Convert.ToString(dr[ETTOrdinal]),
                                    testcaseflagTester = flagTester,
                                    testCaseId = (dr.IsDBNull(TestCase_IdOrdinal)) == true ? "" : Convert.ToString(dr[TestCase_IdOrdinal]),
                                    testCaseName = (dr.IsDBNull(TestCase_NameOrdinal)) == true ? "" : Convert.ToString(dr[TestCase_NameOrdinal]),
                                    testCaseSeq = (dr.IsDBNull(Testcase_SequenceOrdinal)) == true ? "" : Convert.ToString(dr[Testcase_SequenceOrdinal]),
                                    testPassId = (dr.IsDBNull(TestPass_IdOrdinal)) == true ? "" : Convert.ToString(dr[TestPass_IdOrdinal])
                                });
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(listTestCaseDetails);
        }


        [HttpPost, Route("InsertUpdateTestCase")]
        public Dictionary<string, string> InsertUpdateTestCase([FromBody]TestCase oTestCase)

        {
            Dictionary<string, string> _result = new Dictionary<string, string>();


            try
            {


                if (string.IsNullOrEmpty(oTestCase.testCaseName))
                {

                    //send service level Exception as service response
                    _result.Add(this._errorText, "testCaseName is required");
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
                if (string.IsNullOrEmpty(oTestCase.testCaseId))
                    statementType = "Insert";
                else
                    statementType = "Update";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.SpTestCase";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestCaseId", SqlDbType.Int) { Value = oTestCase.testCaseId });
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = oTestCase.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@TestCaseName", SqlDbType.VarChar, 500) { Value = oTestCase.testCaseName });
                    cmd.Parameters.Add(new SqlParameter("@DisplayID", SqlDbType.VarChar, 500) { Value = oTestCase.testCaseDisplayId });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 500) { Value = oTestCase.testCaseDesp });
                    cmd.Parameters.Add(new SqlParameter("@Sequence", SqlDbType.Int) { Value = oTestCase.testCaseSeq });
                    cmd.Parameters.Add(new SqlParameter("@Ett", SqlDbType.NVarChar, 500) { Value = (oTestCase.testCaseETT == "") ? null : oTestCase.testCaseETT });
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
                        if (ReturnParamValue.ToLower() == "success")
                        {/*update case*/
                            _result.Add(this._statusText, "Done");
                            _result.Add("ID", oTestCase.testCaseId);
                        }
                        else
                        {
                            string[] arr = ReturnParamValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length == 1)
                            {/*insert*/
                                _result.Add(this._statusText, "Done");
                                _result.Add("ID", ReturnParamValue);
                            }
                            else
                            {
                                _result.Add(this._errorText, (ReturnParamValue));
                                return _result;
                            }



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

        [HttpDelete, Route("DeleteTestCase")]
        public Dictionary<string, string> DeleteTestCase([FromBody]TestCase tstcs)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();

            try
            {
                List<TestCase> listProjectUsers = new List<TestCase>();
                if (string.IsNullOrEmpty(tstcs.testCaseId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "Test case Id is required!");
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


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "[UAT].[SpTestCase]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestCaseId", SqlDbType.Int) { Value = tstcs.testCaseId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = new clsUtility().GetLoggedInUserSPUserId() == "" ? null : new clsUtility().GetLoggedInUserSPUserId() });
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

        [HttpDelete, Route("BulkDeleteTestCase")]
        public Dictionary<string, string> BulkDeleteTestCase([FromBody]TestCase tstcs1)
        {

            Dictionary<string, string> _result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(tstcs1.testPassId))
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add("ERROR", "Test pass Id is required!");
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
                string statementType = string.Empty;
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())

                {

                    cmd.CommandText = "[UAT].[SpTestCase]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = tstcs1.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "BulkDelete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = new clsUtility().GetLoggedInUserSPUserId() == "" ? null : new clsUtility().GetLoggedInUserSPUserId() });
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


                    }
                }
            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);

            }

            return _result;
        }

        [HttpPost, Route("TestCaseSequencing")]
        public Dictionary<string, string> TestCaseSequencing(string testCaseIdSequence)
        {

            Dictionary<string, string> _result = new Dictionary<string, string>();
            try
            {
                if (string.IsNullOrEmpty(testCaseIdSequence))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "testCaseIdSequence is required!");
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
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())

                {

                    cmd.CommandText = "[UAT].[spTestCaseSeq]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SequencingString", SqlDbType.Int) { Value = testCaseIdSequence });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Sequence" });
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
                    }
                }



            }

            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);

            }

            return _result;

        }



        [HttpPost, Route("CopyTestCase")]
        public List<TesterRoleNames> CopyTestCase([FromBody]TestCaseCopy otestcasecopy)
        {

            List<TesterRoleNames> TesterRoleNames = new List<TesterRoleNames>();
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
                    return null;
                }

                if (string.IsNullOrEmpty(otestcasecopy.testPassId) || string.IsNullOrEmpty(otestcasecopy.testCaseId) || string.IsNullOrEmpty(otestcasecopy.destTestPassId))
                {

                    TesterRoleNames.Add(new TesterRoleNames()
                    {
                        errorText = this._errorText,
                        errorValue = "Test pass Id, test case Id,testPassIdDestination  is required!",
                    });
                    return TesterRoleNames;
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "[uat].[sptestcasecopy]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@oldtestpassid", SqlDbType.Int) { Value = otestcasecopy.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@oldtestcaseid", SqlDbType.Int) { Value = otestcasecopy.testCaseId });
                    cmd.Parameters.Add(new SqlParameter("@newtestpassid", SqlDbType.Int) { Value = otestcasecopy.destTestPassId });

                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    #region 'Old Code'
                    //int retValPos = cmd.ExecuteNonQuery();
                    //if (retValPos != 0)
                    //{
                    //    string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);

                    //    using (var dr = cmd.ExecuteReader())
                    //    {
                    //        int user_name = dr.GetOrdinal("user_name");
                    //        int role_name = dr.GetOrdinal("role_name");
                    //        while (dr.Read())
                    //        {
                    //            user_name = Convert.ToInt32(dr["user_name"]);
                    //            role_name = Convert.ToInt32(dr["role_name"]);
                    //            TesterRoleNames.Add(new TesterRoleNames()
                    //            {
                    //                errorText = "added",
                    //                errorValue = "yes",
                    //                testerName = (dr.IsDBNull(user_name)) == true ? "" : Convert.ToString(dr["user_name"]),
                    //                roleName = (dr.IsDBNull(role_name)) == true ? "" : Convert.ToString(dr["role_name"]),

                    //            });
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(ReturnParamValue) && ReturnParamValue.ToLower() == "success")
                    //    {

                    //        TesterRoleNames.Add(new TesterRoleNames()
                    //        {
                    //            errorText = this._statusText,
                    //            errorValue = "Done",
                    //        });

                    //    }

                    //    else if (ReturnParamValue.ToLower() == "test case name already exists")
                    //    {
                    //        TesterRoleNames.Add(new TesterRoleNames()
                    //        {
                    //            errorText = this._statusText,
                    //            errorValue = ReturnParamValue,
                    //        });
                    //    }

                    //    else
                    //    {
                    //        List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                    //        _outParameter.Add(cmd.Parameters["@Ret_Parameter"]);
                    //        TesterRoleNames.Add(new TesterRoleNames()
                    //        {
                    //            errorText = this._errorText,
                    //            errorValue = _outParameter.ToString(),
                    //        });

                    //    }
                    //} 
                    #endregion

                    using (var dr = cmd.ExecuteReader())
                    {

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                TesterRoleNames.Add(new TesterRoleNames()
                                {
                                    errorText = "added",
                                    errorValue = "yes",
                                    testerName = dr["user_name"] == null ? "" : Convert.ToString(dr["user_name"]),
                                    roleName = dr["role_name"] == null ? "" : Convert.ToString(dr["role_name"]),

                                });
                            }

                        }

                    }
                    int retValPos = 1; // cmd.ExecuteNonQuery();
                    if (retValPos > 0)
                    {
                        string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                        if (!string.IsNullOrEmpty(ReturnParamValue) && ReturnParamValue.ToLower() == "success")
                        {

                            TesterRoleNames.Add(new TesterRoleNames()
                            {
                                errorText = this._statusText,
                                errorValue = "Done",
                            });

                        }

                        else if (ReturnParamValue.ToLower() == "test case name already exists")
                        {
                            TesterRoleNames.Add(new TesterRoleNames()
                            {
                                errorText = this._statusText,
                                errorValue = ReturnParamValue,
                            });
                        }

                        else
                        {
                            List<System.Data.Common.DbParameter> _outParameter = new List<System.Data.Common.DbParameter>();
                            _outParameter.Add(cmd.Parameters["@Ret_Parameter"]);
                            TesterRoleNames.Add(new TesterRoleNames()
                            {
                                errorText = this._errorText,
                                errorValue = _outParameter.ToString(),
                            });

                        }

                    }
                    

                }
            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);

            }

            return TesterRoleNames;




        }









    }

}







