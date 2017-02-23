using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class QueryController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";

        private clsDbContext _context;

        public QueryController(clsDbContext context)
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

        [HttpGet, Route("GetQueryProjectDropDown")]
        public JsonResult GetQueryProjectDropDown()
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

            List<Q_Project> _project = new List<Q_Project>();

            int _res = 0;
            try
            {
                /******************standard*********************/
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spQueryProject";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 10) { Value = "Select" });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    _res = cmd.ExecuteNonQuery();

                    var OutResult = outparam.Value;
                   
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows != false)
                        {
                            while (dataReader.Read())
                            {
                                _project.Add(new Q_Project
                                {
                                    Project_Id = dataReader["Project_Id"].ToString(),
                                    Project_Name = dataReader["Project_Name"].ToString(),
                                    Project_Version = dataReader["Project_Version"].ToString()
                                });
                            }
                        }
                    }
                }
            }         
            catch
            {

            }
            return Json(_project);
        }

        [HttpGet, Route("GetQueryTestPassDropDown/{projectID}")]
        public JsonResult GetQueryTestPassDropDown(string projectID)
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

            List<Q_TestPass> _testpass = new List<Q_TestPass>();
            List<Q_Role> _Role = new List<Q_Role>();
        
            int _res = 0;
            try
            {
                /******************standard*********************/
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spQueryTessPass&Role_Core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 10) { Value = "TestPass" });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.NVarChar, 10) { Value = projectID });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    _res = cmd.ExecuteNonQuery();


                    List<QueryTestPassDropDown> lstTpDD = null;
                    List<QueryRoleDropDown> lstRoleDD = null;

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows != false)
                        {
                            while (dataReader.Read())
                            {
                                _testpass.Add(new Q_TestPass
                                {
                                    TestPass_ID = dataReader["TestPass_ID"].ToString(),
                                    TestPass_Name = dataReader["TestPass_Name"].ToString()
                                });
                            }
                        }
                    }
                }

            }
            catch
            {

            }
            return Json(_testpass);

        }

        [HttpGet, Route("GetQueryRoleDropDown/{projectID}")]
        public JsonResult GetQueryRoleDropDown(string projectID)
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

            List<Q_Role> _Role = new List<Q_Role>();

            int _res = 0;
            try
            {
                /******************standard*********************/
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spQueryTessPass&Role_Core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 10) { Value = "Role" });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.NVarChar, 10) { Value = projectID });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    _res = cmd.ExecuteNonQuery();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows != false)
                        {
                            while (dataReader.Read())
                            {
                                _Role.Add(new Q_Role
                                {
                                    Role_Id = dataReader["Role_Id"].ToString(),
                                    Role_Name = dataReader["Role_Name"].ToString()
                                });
                            }
                        }
                    }
                }

            }
            catch
            {

            }
            return Json(_Role);

        }

        [HttpGet, Route("GetQueryDetailsForAND/{sRequest}/{projectId}")]
        public JsonResult GetQueryDetailsForAND(string sRequest, string projectId)
        {
            string conditionalStatement = "";
            string lastCriteriaName = "";
            string lastCriteriaValue = "";

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
            int _res = 0;
            Dictionary<string, string> _result = new Dictionary<string, string>();

            List<QueryReq> listQueryReq = new List<QueryReq>();
            List<QueryTP> lstQueryTP = null;
            List<QueryTS> lstQueryTS = null;
            List<QueryProject> lstQueryProject = null;
            List<QueryTC> lstQueryTC = null;
            List<QueryUITC> lstQueryUITC = null;
            List<QueryUITP> lstQueryUITP = null;
            List<QueryUITS> lstQueryUITS = null;
            List<QueryRoleTester> lstRoleTester = null;
            List<QueryRoleTestStep> lstRoleTS = null;
            List<QueryAssignedToTester> lstAssignTester = null;
            List<QueryAssignedToTestMgr> lstAssignTestMgr = null;
            List<QueryTSByStatus> lstQTSStatus = null;
            List<QueryTPByStatus> lstQTPStatus = null;
            List<QueryTCByStatus> lstQTCStatus = null;
            List<QueryProjectByStatus> lstQPrjStatus = null;
            List<QueryTCByID> lstTCID = null;
            List<QueryTSByID> lstTSID = null;
            List<QueryProjectByID> lstProjectByID = null;
            List<QueryTestPassByID> lstTestPassByID = null;
            List<QueryAssignedPrjLead> lstAssignPrjLead = null;
            QueryResponseForAND oQueryResponse = new QueryResponseForAND();

            try
            {
                string conStatement = GetConditionalStatement(sRequest);

                if (!string.IsNullOrEmpty(conStatement))
                {
                    conditionalStatement = conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(0);
                    if (conditionalStatement == " " && (conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries)).Length != 3)
                        conditionalStatement = null;
                    else if ((conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries)).Length == 3)
                    {
                        if (conditionalStatement == " ")
                            conditionalStatement = null;
                        lastCriteriaName = ((conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1)) == "" ? "" : conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1));
                        lastCriteriaValue = ((conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(2)) == "" ? "" : conStatement.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(2));
                    }
                }

                //var str = conditionalStatement;
                //int startIndex = str.IndexOf('"');
                //var final=conditionalStatement.Substring(1, conditionalStatement.Length);


                //List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetANDQueryDetails_Core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });

                    /**/
                    if (projectId != null)
                        cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.NVarChar, 10) { Value = Convert.ToInt16(projectId) });
                    else
                        cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.NVarChar, 10) { Value = DBNull.Value });

                    /**/
                    if (conditionalStatement != null)
                        cmd.Parameters.Add(new SqlParameter("@ConditionalStalement", SqlDbType.NVarChar, 4000) { Value = conditionalStatement.Trim() });
                    else
                        cmd.Parameters.Add(new SqlParameter("@ConditionalStalement", SqlDbType.NVarChar, 4000) { Value = DBNull.Value });

                    /**/
                    if (lastCriteriaName != null)
                        cmd.Parameters.Add(new SqlParameter("@LastCriteriaName", SqlDbType.NVarChar, 4000) { Value = lastCriteriaName.Trim() });
                    else
                        cmd.Parameters.Add(new SqlParameter("@LastCriteriaName", SqlDbType.NVarChar, 4000) { Value = DBNull.Value });

                    /**/
                    if (lastCriteriaValue != null)
                        cmd.Parameters.Add(new SqlParameter("@LastCriteriaValue", SqlDbType.NVarChar, 4000) { Value = lastCriteriaValue });
                    else
                        cmd.Parameters.Add(new SqlParameter("@LastCriteriaValue", SqlDbType.NVarChar, 4000) { Value = DBNull.Value });

                    /**/
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    _res = cmd.ExecuteNonQuery();
                    List<Q_collectionForStatus> lst_collectionForStatus = new List<Q_collectionForStatus>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows != false)
                        {
                            //while (dr.Read())
                            //{
                                
                            //    string tsid_s = null;
                            //    var flag1 = dr["flag"].ToString();
                            //    if(flag1=="TestStep")
                            //        tsid_s = (dr["TestStep_ID"] == null) ? null : Convert.ToString(dr["TestStep_ID"]);
                            //    lst_collectionForStatus.Add(new Q_collectionForStatus
                            //    {
                            //        st_TestStepId = tsid_s
                            //    });
                            //}

                                while (dr.Read())
                            {
                                var flag = dr["flag"].ToString();

                                switch (flag)
                                {
                                    case "TestPass":
                                        lstQueryTP = new List<QueryTP>();
                                        if (oQueryResponse.lstTestPass != null)
                                        {
                                            lstQueryTP = oQueryResponse.lstTestPass;
                                        }


                                        string tpid = dr["TestPass_ID"].ToString();//== un ? "" : Convert.ToString(dr["TestPass_ID"]);
                                        if (tpid != "")
                                        {
                                            if (lstQueryTP.Any(z => z.sTestPassId == tpid) == false)
                                            {
                                                lstQueryTP.Add(new QueryTP
                                                {
                                                    sTestManagerName = (dr["TestManagerName"] == null) ? "-" : Convert.ToString(dr["TestManagerName"]),//sTestManagerName = (dr["TestManagerName")) == null) ? "-" : Convert.ToString(dr["TestManagerName"]),
                                                    sTPDescription = (dr["TestPass_Description"] == null) ? "-" : Convert.ToString(dr["TestPass_Description"]),
                                                    sTesterCount = (dr["TesterCount"] == null) ? "0" : Convert.ToString(dr["TesterCount"]),
                                                    sTestPassId = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    stestpassStatus = (dr["TPStatus"] == null) ? "-" : Convert.ToString(dr["TPStatus"]),
                                                    sStartDate = (dr["Start_Date"] == null) ? "-" : Convert.ToString(dr["Start_Date"]),
                                                    sEndDate = (dr["End_Date"] == null) ? "-" : Convert.ToString(dr["End_Date"]),
                                                    sTcCount = (dr["TcCount"] == null) ? "0" : Convert.ToString(dr["TcCount"]),
                                                    sType = "TestPass_Type"
                                                });
                                            }
                                        }
                                        oQueryResponse.lstTestPass = lstQueryTP;
                                        break;
                                    /*end of test pass*/
                                    /*start of test case*/
                                    case "TestStep":
                                        lstQueryTS = new List<QueryTS>();
                                        if (oQueryResponse.lstTestStep != null)
                                        {
                                            lstQueryTS = oQueryResponse.lstTestStep;
                                        }

                                        string status1 = "-";//(dr["Status"] == null) ? "-" : GetStatusForTSandTC(dtTS, Convert.ToString(dr["TestStep_ID"]), "");
                                        string roleName2 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string tsid2 = (dr["TestStep_ID"] == null) ? "" : Convert.ToString(dr["TestStep_ID"]);
                                        string testerid2 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        if (roleName2 != "" && tsid2 != "" && testerid2 != "")
                                        {
                                            if (lstQueryTS.Any(z => z.sTestStepId == tsid2 && z.sTesterId == testerid2 && z.sRoleName == roleName2) == false)
                                            {
                                                lstQueryTS.Add(new QueryTS
                                                {
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sTestStepId = (dr["TestStep_ID"] == null) ? "-" : Convert.ToString(dr["TestStep_ID"]),
                                                    sTestStepName = (dr["TestStep_ActionName"] == null) ? "-" : Convert.ToString(dr["TestStep_ActionName"]),
                                                    sTestStepStatus = status1,
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sExpected_Result = (dr["Expected_Result"] == null) ? "-" : Convert.ToString(dr["Expected_Result"]),
                                                    sActual_Result = (dr["Actual_Result"] == null) ? "-" : Convert.ToString(dr["Actual_Result"]),
                                                    sAR_Attachment1_Name = (dr["AR_Attachment1_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_Name"]),
                                                    sAR_Attachment1_URL = (dr["AR_Attachment1_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_URL"]),
                                                    sAR_Attachment2_Name = (dr["AR_Attachment2_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_Name"]),
                                                    sAR_Attachment2_URL = (dr["AR_Attachment2_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_URL"]),
                                                    sAR_Attachment3_Name = (dr["AR_Attachment3_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_Name"]),
                                                    sAR_Attachment3_URL = (dr["AR_Attachment3_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_URL"]),
                                                    sType = "TestStep_Type"
                                                });
                                            }
                                        }

                                        oQueryResponse.lstTestStep = lstQueryTS;
                                        break;

                                    case "TestCase":
                                        lstQueryTC = new List<QueryTC>();
                                        if (oQueryResponse.lstTestCase != null)
                                        {
                                            lstQueryTC = oQueryResponse.lstTestCase;
                                        }
                                        //done
                                        string tcid3 = (dr["TestCase_ID"] == null) ? "" : Convert.ToString(dr["TestCase_ID"]);
                                        string testerid3 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        string roleName3 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string roleId3 = (dr["Role_ID"] == null) ? null : Convert.ToString(dr["Role_ID"]);

                                        if (roleName3 != "" && tcid3 != "" && testerid3 != "")
                                        {
                                            if (lstQueryTC.Any(z => z.sTestCaseId == tcid3 && z.sTesterId == testerid3 && z.sRoleName == roleName3) == false)
                                            {
                                                lstQueryTC.Add(new QueryTC
                                                {
                                                    sTCStatus = "",//(dr["Status"] == null) ? "" : GetStatusForTestcase(dtTC, Convert.ToString(dr["TestCase_ID"]),Convert.ToString(dr["User_Id"]), Convert.ToString(dr["Role_Id"]],
                                                    sTestCaseId = (dr["TestCase_ID"] == null) ? "-" : Convert.ToString(dr["TestCase_ID"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sType = "TestCase_Type",
                                                    sTestStepStatusID= (dr["TestStepStatusID"] == null) ?null : Convert.ToString(dr["TestStepStatusID"]),
                                                    sRoleId= roleId3

                                                });
                                            }
                                        }
                                        oQueryResponse.lstTestCase = lstQueryTC;
                                        break;

                                    /*end of test case*/
                                    /*********start role step*********************************/
                                    case "Role TestStep":

                                        lstRoleTS = new List<QueryRoleTestStep>();
                                        if (oQueryResponse.lstRoleTS != null)
                                        {
                                            lstRoleTS = oQueryResponse.lstRoleTS;
                                        }

                                        string status4 = "-";// (dr["Status")) == null) ? "-" : GetStatusForTSandTC(dtR2, Convert.ToString(dr["TestStep_ID"]), "");
                                        string roleName4 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string tsid4 = (dr["TestStep_ID"] == null) ? "" : Convert.ToString(dr["TestStep_ID"]);
                                        string userid4 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        if (roleName4 != "" && tsid4 != "" && userid4 != "")
                                        {
                                            if (lstRoleTS.Any(z => z.sTestStepID == tsid4 && z.sTesterID == userid4 && z.sRoleName == roleName4) == false)
                                            {
                                                lstRoleTS.Add(new QueryRoleTestStep
                                                {
                                                    sTestStepID = (dr["TestStep_ID"] == null) ? "-" : Convert.ToString(dr["TestStep_ID"]),
                                                    sTestStepName = (dr["TestStep_ActionName"] == null) ? "-" : Convert.ToString(dr["TestStep_ActionName"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sStatus = status4,
                                                    sTesterID = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sUserId = (dr["User_ID"] == null) ? "-" : Convert.ToString(dr["User_ID"]),
                                                    sTester = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sExpected_Result = (dr["Expected_Result"] == null) ? "-" : Convert.ToString(dr["Expected_Result"]),
                                                    sActual_Result = (dr["Actual_Result"] == null) ? "-" : Convert.ToString(dr["Actual_Result"]),
                                                    sAR_Attachment1_Name = (dr["AR_Attachment1_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_Name"]),
                                                    sAR_Attachment1_URL = (dr["AR_Attachment1_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_URL"]),
                                                    sAR_Attachment2_Name = (dr["AR_Attachment2_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_Name"]),
                                                    sAR_Attachment2_URL = (dr["AR_Attachment2_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_URL"]),
                                                    sAR_Attachment3_Name = (dr["AR_Attachment3_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_Name"]),
                                                    sAR_Attachment3_URL = (dr["AR_Attachment3_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_URL"]),
                                                    sType = "RoleTester_Type"
                                                });
                                            }
                                        }


                                        oQueryResponse.lstRoleTS = lstRoleTS;
                                        break;

                                    /***********end role step *****************************/
                                    /*******************************************************/
                                    //Status TestCase_TestStep
                                    case "Status TestCase_TestStep":
                                        //******test step*****/
                                        lstQTSStatus = new List<QueryTSByStatus>();
                                        if (oQueryResponse.lstTSStatus != null)
                                        {
                                            lstQTSStatus = oQueryResponse.lstTSStatus;
                                        }

                                        string status = "-"; //(dr["Status"] == null) ? "-" : GetStatusForTSandTC(dtTSS, Convert.ToString(dr["TestStep_ID"]), "");
                                        string roleName = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string tsid = (dr["TestStep_ID"] == null) ? "" : Convert.ToString(dr["TestStep_ID"]);
                                        string testerid = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        if (roleName != "" && tsid != "" && testerid != "")
                                        {
                                            if (lstQTSStatus.Any(z => z.sTestStepId == tsid && z.sTesterId == testerid && z.sRoleName == roleName) == false)
                                            {
                                                lstQTSStatus.Add(new QueryTSByStatus
                                                {
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sTestStepId = (dr["TestStep_ID"] == null) ? "-" : Convert.ToString(dr["TestStep_ID"]),
                                                    sTestStepName = (dr["TestStep_ActionName"] == null) ? "-" : Convert.ToString(dr["TestStep_ActionName"]),
                                                    sTestStepStatus = status,
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sExpected_Result = (dr["Expected_Result"] == null) ? "-" : Convert.ToString(dr["Expected_Result"]),
                                                    sActual_Result = (dr["Actual_Result"] == null) ? "-" : Convert.ToString(dr["Actual_Result"]),
                                                    sAR_Attachment1_Name = (dr["AR_Attachment1_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_Name"]),
                                                    sAR_Attachment1_URL = (dr["AR_Attachment1_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_URL"]),
                                                    sAR_Attachment2_Name = (dr["AR_Attachment2_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_Name"]),
                                                    sAR_Attachment2_URL = (dr["AR_Attachment2_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_URL"]),
                                                    sAR_Attachment3_Name = (dr["AR_Attachment3_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_Name"]),
                                                    sAR_Attachment3_URL = (dr["AR_Attachment3_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_URL"]),
                                                    sType = "StatusTestStep_Type"
                                                });
                                            }
                                        }

                                        oQueryResponse.lstTSStatus = lstQTSStatus;
                                        //break;
                                        /******test step*****/
                                        /******test case*****/
                                        //case "Status TestCase":
                                        lstQTCStatus = new List<QueryTCByStatus>();
                                        if (oQueryResponse.lstTcByStatus != null)
                                        {
                                            lstQTCStatus = oQueryResponse.lstTcByStatus;
                                        }
                                        //rupal
                                        //string status = (dr["Status"] == null) ? "-" : GetStatusForTSandTC(dtTCS, "", Convert.ToString(dr["TestCase_ID"]];
                                        string roleName1 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string tcid = (dr["TestCase_ID"] == null) ? "" : Convert.ToString(dr["TestCase_ID"]);
                                        string testerid1 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        if (roleName1 != "" && tcid != "" && testerid1 != "")
                                        {
                                            if (lstQTCStatus.Any(z => z.sTestCaseId == tcid && z.sTesterId == testerid1 && z.sRoleName == roleName1) == false)
                                            {
                                                lstQTCStatus.Add(new QueryTCByStatus
                                                {
                                                    sTCStatus = "",//(dr["Status"]== null) ? "" : GetStatusForTestcase(dtTCS, Convert.ToString(dr["TestCase_ID"]),Convert.ToString(dr["User_Id"]), Convert.ToString(dr["Role_Id"]],
                                                    sTestCaseId = (dr["TestCase_ID"] == null) ? "-" : Convert.ToString(dr["TestCase_ID"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sType = "StatusTestCase_Type"
                                                });
                                            }
                                        }
                                        oQueryResponse.lstTcByStatus = lstQTCStatus;
                                        break;

                                    /******test case*****/
                                    /************ status project *************/
                                    case "Status Project_TestPass":

                                        lstQTPStatus = new List<QueryTPByStatus>();
                                        if (oQueryResponse.lstTPByStatus != null)
                                        {
                                            lstQTPStatus = oQueryResponse.lstTPByStatus;
                                        }

                                        string tpid5 = (dr["TestPass_ID"] == null) ? "" : Convert.ToString(dr["TestPass_ID"]);
                                        if (tpid5 != "")
                                        {
                                            if (lstQTPStatus.Any(z => z.sTestPassId == tpid5) == false)
                                            {
                                                lstQTPStatus.Add(new QueryTPByStatus
                                                {
                                                    sTPDescription = (dr["TestPass_Description"] == null) ? "-" : Convert.ToString(dr["TestPass_Description"]),
                                                    sTesterCount = (dr["TesterCount"] == null) ? "0" : Convert.ToString(dr["TesterCount"]),
                                                    sEndDate = (dr["End_Date"] == null) ? "-" : Convert.ToString(dr["End_Date"]),
                                                    sStartDate = (dr["Start_Date"] == null) ? "-" : Convert.ToString(dr["Start_Date"]),
                                                    sTcCount = (dr["TcCount"] == null) ? "0" : Convert.ToString(dr["TcCount"]),
                                                    sTestManagerName = (dr["TestManagerName"] == null) ? "-" : Convert.ToString(dr["TestManagerName"]),
                                                    sTestPassId = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    stestpassStatus = (dr["TPStatus"] == null) ? "-" : Convert.ToString(dr["TPStatus"]),
                                                    sType = "StatusTP_Type"
                                                });
                                            }

                                        }
                                        oQueryResponse.lstTPByStatus = lstQTPStatus;
                                        // break;
                                        lstQPrjStatus = new List<QueryProjectByStatus>();
                                        if (oQueryResponse.lstPrjByStatus != null)
                                        {
                                            lstQPrjStatus = oQueryResponse.lstPrjByStatus;
                                        }

                                        lstQPrjStatus.Add(new QueryProjectByStatus
                                        {
                                            sProject_Id = (dr["Project_Id"] == null) ? "-" : Convert.ToString(dr["Project_Id"]),
                                            sProject_Name = (dr["Project_Name"] == null) ? "-" : Convert.ToString(dr["Project_Name"]),
                                            sProject_Status = (dr["ProjStatus"] == null) ? "-" : Convert.ToString(dr["ProjStatus"]),
                                            sProject_Version = (dr["Project_Version"] == null) ? "-" : Convert.ToString(dr["Project_Version"]),
                                            sVersion_Lead = (dr["Version_Lead"] == null) ? "-" : Convert.ToString(dr["Version_Lead"]),
                                            sType = "StatusPrj_Type"
                                        });


                                        oQueryResponse.lstPrjByStatus = lstQPrjStatus;
                                        break;
                                    /**************** status project ******************/

                                    /***************** UI All*************************/
                                    case "UI TestCase":
                                        lstQueryUITC = new List<QueryUITC>();
                                        if (oQueryResponse.lstTestCaseUI != null)
                                        {
                                            lstQueryUITC = oQueryResponse.lstTestCaseUI;
                                        }

                                        string tcid6 = (dr["TestCase_ID"] == null) ? "" : Convert.ToString(dr["TestCase_ID"]);
                                        string testerid6 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                        string roleName6 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        if (roleName6 != "" && tcid6 != "" && testerid6 != "")
                                        {
                                            if (lstQueryUITC.Any(z => z.sTestCaseId == tcid6 && z.sTesterId == testerid6 && z.sRoleName == roleName6) == false)
                                            {
                                                lstQueryUITC.Add(new QueryUITC
                                                {
                                                    sTCStatus ="",// (dr["Status"] == null) ? "" : GetStatusForTestcase(dtTCUI, Convert.ToString(dr["TestCase_ID"]),Convert.ToString(dr["User_Id"]), Convert.ToString(dr["Role_Id"]],
                                                    sTestCaseId = (dr["TestCase_ID"] == null) ? "-" : Convert.ToString(dr["TestCase_ID"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sType = "UITestCase_Type"
                                                });
                                            }
                                        }
                                        oQueryResponse.lstTestCaseUI = lstQueryUITC;
                                        break;

                                    case "UI TestStep":
                                        lstQueryUITS = new List<QueryUITS>();
                                        if (oQueryResponse.lstTestStepUI != null)
                                        {
                                            lstQueryUITS = oQueryResponse.lstTestStepUI;
                                        }

                                        string status7 = "-";// (dr["Status"] == null) ? "-" : GetStatusForTSandTC(dtTSUI, Convert.ToString(dr["TestStep_ID"]), "");
                                        string roleName7 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                        string tsid7 = (dr["TestStep_ID"] == null) ? "" : Convert.ToString(dr["TestStep_ID"]);
                                        string testerid7 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);

                                        if (roleName7 != "" && tsid7 != "" && testerid7 != "")
                                        {
                                            if (lstQueryUITS.Any(z => z.sTestStepId == tsid7 && z.sTesterId == testerid7 && z.sRoleName == roleName7) == false)
                                            {
                                                lstQueryUITS.Add(new QueryUITS
                                                {
                                                    sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                    sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sTestStepId = (dr["TestStep_ID"] == null) ? "-" : Convert.ToString(dr["TestStep_ID"]),
                                                    sTestStepName = (dr["TestStep_ActionName"] == null) ? "-" : Convert.ToString(dr["TestStep_ActionName"]),
                                                    sTestStepStatus = status7,
                                                    sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                    sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                    sExpected_Result = (dr["Expected_Result"] == null) ? "-" : Convert.ToString(dr["Expected_Result"]),
                                                    sActual_Result = (dr["Actual_Result"] == null) ? "-" : Convert.ToString(dr["Actual_Result"]),
                                                    sAR_Attachment1_Name = (dr["AR_Attachment1_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_Name"]),
                                                    sAR_Attachment1_URL = (dr["AR_Attachment1_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_URL"]),
                                                    sAR_Attachment2_Name = (dr["AR_Attachment2_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_Name"]),
                                                    sAR_Attachment2_URL = (dr["AR_Attachment2_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_URL"]),
                                                    sAR_Attachment3_Name = (dr["AR_Attachment3_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_Name"]),
                                                    sAR_Attachment3_URL = (dr["AR_Attachment3_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_URL"]),
                                                    sType = "UITestStep_Type"
                                                });
                                            }
                                        }
                                        oQueryResponse.lstTestStepUI = lstQueryUITS;
                                        break;

                                        case "UI TestPass":
                                        lstQueryUITP = new List<QueryUITP>();
                                        if (oQueryResponse.lstTestPassUI != null)
                                        {
                                            lstQueryUITP = oQueryResponse.lstTestPassUI;
                                        }

                                        string tpid8 = (dr["TestPass_ID"] == null) ? "" : Convert.ToString(dr["TestPass_ID"]);
                                        if (tpid8 != "")
                                        {
                                            if (lstQueryUITP.Any(z => z.sTestPassId == tpid8) == false)
                                            {
                                                lstQueryUITP.Add(new QueryUITP
                                                {
                                                    sTPDescription = (dr["TestPass_Description"] == null) ? "-" : Convert.ToString(dr["TestPass_Description"]),
                                                    sTesterCount = (dr["TesterCount"] == null) ? "0" : Convert.ToString(dr["TesterCount"]),
                                                    sTestManagerName = (dr["TestManagerName"] == null) ? "-" : Convert.ToString(dr["TestManagerName"]),
                                                    sTestPassId = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                    sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    stestpassStatus = (dr["TPStatus"] == null) ? "-" : Convert.ToString(dr["TPStatus"]),
                                                    sStartDate = (dr["Start_Date"] == null) ? "-" : Convert.ToString(dr["Start_Date"]),
                                                    sEndDate = (dr["End_Date"] == null) ? "-" : Convert.ToString(dr["End_Date"]),
                                                    sTcCount = (dr["TcCount"] == null) ? "0" : Convert.ToString(dr["TcCount"]),
                                                    sType = "UITestPass_Type"
                                                });
                                            }
                                        }
                                        oQueryResponse.lstTestPassUI = lstQueryUITP;
                                        break;

                                    /***************** UI All*************************/

                                    /***************** assigned to*********************/
                                    //case "AssignedTo Lead":
                                    case "AssignedTo All":
                                        lstAssignPrjLead = new List<QueryAssignedPrjLead>();
                                        if (oQueryResponse.lstAssignPrjLead != null)
                                        {
                                            lstAssignPrjLead = oQueryResponse.lstAssignPrjLead;
                                        }
                                        
                                            string pid9 = (dr["Project_Id"] == null) ? "-" : Convert.ToString(dr["Project_Id"]);
                                            if (lstAssignPrjLead.Any(z => z.sProjID == pid9) == false)
                                            {
                                                lstAssignPrjLead.Add(new QueryAssignedPrjLead
                                                {
                                                    sProjID = (dr["Project_Id"] == null) ? "-" : Convert.ToString(dr["Project_Id"]),
                                                    sProjName = (dr["Project_Name"] == null) ? "-" : Convert.ToString(dr["Project_Name"]),
                                                    sProjStatus = (dr["ProjStatus"] == null) ? "-" : Convert.ToString(dr["ProjStatus"]),
                                                    sProjVersion = (dr["Project_Version"] == null) ? "-" : Convert.ToString(dr["Project_Version"]),
                                                    sProjleadName = (dr["Version_Lead"] == null) ? "-" : Convert.ToString(dr["Version_Lead"]),
                                                    sType = "AssignPrjLead_Type"
                                                });
                                            }
                                        
                                        oQueryResponse.lstAssignPrjLead = lstAssignPrjLead;
                                       // break;

                                    //case "AssignedTo Tester":
                                        lstAssignTester = new List<QueryAssignedToTester>();
                                        if (oQueryResponse.lstAssignedToTester != null)
                                        {
                                            lstAssignTester = oQueryResponse.lstAssignedToTester;
                                        }
                                        
                                            string tpid11 = (dr["TestPass_ID"] == null) ? "" : Convert.ToString(dr["TestPass_ID"]);
                                            string testerid11 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                            string roleName11 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                            if (tpid11 != "" && testerid11 != "" && roleName11 != "")
                                            {
                                                if (lstAssignTester.Any(z => z.sTpId == tpid11 && z.sTesterID == testerid11 && z.sRoleName == roleName11) == false)
                                                {
                                                    lstAssignTester.Add(new QueryAssignedToTester
                                                    {
                                                        sArea_Name = (dr["Area_Name"] == null) ? "N/A" : Convert.ToString(dr["Area_Name"]),
                                                        sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                        sTesterID = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                        sTpName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                        sTesterName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                        sTpId = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                        sStatusTester = "-",//(dr["Status"] == null) ? "In Active" : GetStatusForTpAssigntoTester(dtAT,
                                                                //Convert.ToString(dr["TestPass_ID"]), Convert.ToString(dr["User_Id"]), Convert.ToString(dr["Role_ID"]],
                                                        sType = "AssignedToTester"
                                                    });
                                                }
                                            
                                        }

                                        oQueryResponse.lstAssignedToTester = lstAssignTester;
                                       // break;

                                   // case "AssignedTo TestManager":
                                        lstAssignTestMgr = new List<QueryAssignedToTestMgr>();
                                        if (oQueryResponse.lstAssignedToTestMgr != null)
                                        {
                                            lstAssignTestMgr = oQueryResponse.lstAssignedToTestMgr;
                                        }
                                       
                                            string tpid22 = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]);
                                            if (lstAssignTestMgr.Any(z => z.sTpId == tpid22 /*&& z.sTesterId == testerid*/) == false)
                                            {
                                                lstAssignTestMgr.Add(new QueryAssignedToTestMgr
                                                {
                                                    sTpDescription = (dr["TestPass_Description"] == null) ? "-" : Convert.ToString(dr["TestPass_Description"]),
                                                    sTesterCount = (dr["TesterCount"] == null) ? "0" : Convert.ToString(dr["TesterCount"]),
                                                    sTestMgrID = (dr["TestMgr_ID"] == null) ? "-" : Convert.ToString(dr["TestMgr_ID"]),
                                                    sTestMgrName = (dr["TestManagerName"] == null) ? "-" : Convert.ToString(dr["TestManagerName"]),
                                                    sTpName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                    sTpId = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                    sStatusTestMgr = (dr["TPStatus"] == null) ? "-" : Convert.ToString(dr["TPStatus"]),
                                                    sEndDate = (dr["End_Date"] == null) ? "-" : Convert.ToString(dr["End_Date"]),
                                                    sStartDate = (dr["Start_Date"] == null) ? "-" : Convert.ToString(dr["Start_Date"]),
                                                    sTcCount = (dr["TcCount"] == null) ? "0" : Convert.ToString(dr["TcCount"]),
                                                    sType = "AssignedToTestMgr"
                                                });
                                            }                                      
                                        oQueryResponse.lstAssignedToTestMgr = lstAssignTestMgr;
                                        break;

                                    /***************** assigned to ********************/
                                    /************* ID *****************/
                                    case "ID Project":
                                        lstProjectByID = new List<QueryProjectByID>();
                                        if (oQueryResponse.lstPrjByID != null)
                                        {
                                            lstProjectByID = oQueryResponse.lstPrjByID;
                                        }
                                        string tpid333 = (dr["Project_Id"] == null) ? "-" : Convert.ToString(dr["Project_Id"]);
                                        if (lstProjectByID.Any(z => z.sProjID == tpid333) == false)
                                        {
                                            lstProjectByID.Add(new QueryProjectByID
                                            {
                                                sProjID = (dr["Project_Id"] == null) ? "-" : Convert.ToString(dr["Project_Id"]),
                                                sProjName = (dr["Project_Name"] == null) ? "-" : Convert.ToString(dr["Project_Name"]),
                                                sProjStatus = (dr["ProjStatus"] == null) ? "-" : Convert.ToString(dr["ProjStatus"]),
                                                sProjleadName = (dr["Version_Lead"] == null) ? "-" : Convert.ToString(dr["Version_Lead"]),
                                                sProjVersion = (dr["Project_Version"] == null) ? "-" : Convert.ToString(dr["Project_Version"]),
                                                sType = "IDProj_Type"

                                            });
                                        }                                          
                                        oQueryResponse.lstPrjByID = lstProjectByID;
                                       break;

                                    case "ID TestPass":
                                        lstTestPassByID = new List<QueryTestPassByID>();
                                        if (oQueryResponse.lstTPByID != null)
                                        {
                                            lstTestPassByID = oQueryResponse.lstTPByID;
                                        }
                                        
                                            string tpid33 = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]);
                                            if (tpid33 != null)
                                            {
                                                if (lstTestPassByID.Any(z => z.sTPID == tpid33) == false)
                                                {
                                                    lstTestPassByID.Add(new QueryTestPassByID
                                                    {
                                                        sTPDescription = (dr["TestPass_Description"] == null) ? "-" : Convert.ToString(dr["TestPass_Description"]),
                                                        sTesterCount = (dr["TesterCount"] == null) ? "0" : Convert.ToString(dr["TesterCount"]),
                                                        sTPID = (dr["TestPass_ID"] == null) ? "-" : Convert.ToString(dr["TestPass_ID"]),
                                                        sTPName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                        sStart_Date = (dr["Start_Date"] == null) ? "-" : Convert.ToString(dr["Start_Date"]),
                                                        sEnd_Date = (dr["End_Date"] == null) ? "-" : Convert.ToString(dr["End_Date"]),
                                                        sTestMgr_Name = (dr["TestManagerName"] == null) ? "-" : Convert.ToString(dr["TestManagerName"]),
                                                        sTpstatus = (dr["TPStatus"] == null) ? "-" : Convert.ToString(dr["TPStatus"]),
                                                        sTcCount = (dr["TcCount"] == null) ? "0" : Convert.ToString(dr["TcCount"]),
                                                        sType = "IDTP_Type"
                                                    });
                                                }
                                            }

                                        
                                        oQueryResponse.lstTPByID = lstTestPassByID;
                                        break;

                                    case "ID TestCase":

                                        lstTCID = new List<QueryTCByID>();
                                        if (oQueryResponse.lstTCByID != null)
                                        {
                                            lstTCID = oQueryResponse.lstTCByID;
                                        }
                                        
                                            string roleName44 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                            string tcid44 = (dr["TestCase_ID"] == null) ? "" : Convert.ToString(dr["TestCase_ID"]);
                                            string testerid44 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                            if (roleName44 != "" && tcid44 != "" && testerid44 != "")
                                            {
                                                if (lstTCID.Any(z => z.sTestCaseId == tcid44 && z.sTesterId == testerid44 && z.sRoleName == roleName44) == false)
                                                {
                                                    lstTCID.Add(new QueryTCByID
                                                    {
                                                        sTCStatus ="",// (dr["Status"] == null) ? "" : GetStatusForTestcase(dtTCID, Convert.ToString(dr["TestCase_ID"]),
                                                        sTestCaseId = (dr["TestCase_ID"] == null) ? "-" : Convert.ToString(dr["TestCase_ID"]),
                                                        sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                        sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                        sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                        sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                        sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                        sType = "IDTC_Type"
                                                    });
                                                }                                           
                                        }
                                        oQueryResponse.lstTCByID = lstTCID;
                                        break;

                                    case "ID TestStep":

                                        lstTSID = new List<QueryTSByID>();
                                        if (oQueryResponse.lstTSByID != null)
                                        {
                                            lstTSID = oQueryResponse.lstTSByID;
                                        }

                                        string status111 = "-";// (dr["Status"] == null) ? "-" : GetStatusForTSandTC(dtTSID, Convert.ToString(dr["TestStep_ID"]), "");
                                            string roleName111 = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);
                                            string tsid111 = (dr["TestStep_ID"] == null) ? "" : Convert.ToString(dr["TestStep_ID"]);
                                            string testerid111 = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]);
                                            if (roleName111 != "" && tsid111 != "" && testerid111 != "")
                                            {
                                                if (lstTSID.Any(z => z.sTestStepId == tsid111 && z.sTesterId == testerid111 && z.sRoleName == roleName111) == false)
                                                {
                                                    lstTSID.Add(new QueryTSByID
                                                    {
                                                        sTestStepStatus = status111,
                                                        sTestCaseName = (dr["TestCase_Name"] == null) ? "-" : Convert.ToString(dr["TestCase_Name"]),
                                                        sTestPassName = (dr["TestPass_Name"] == null) ? "-" : Convert.ToString(dr["TestPass_Name"]),
                                                        sUserName = (dr["User_Name"] == null) ? "-" : Convert.ToString(dr["User_Name"]),
                                                        sRoleName = (dr["Role_Name"] == null) ? "-" : Convert.ToString(dr["Role_Name"]),
                                                        sTestStepId = (dr["TestStep_ID"] == null) ? "-" : Convert.ToString(dr["TestStep_ID"]),
                                                        sTestStepName = (dr["TestStep_ActionName"] == null) ? "-" : Convert.ToString(dr["TestStep_ActionName"]),
                                                        sTesterId = (dr["User_Id"] == null) ? "-" : Convert.ToString(dr["User_Id"]),
                                                        sExpected_Result = (dr["Expected_Result"] == null) ? "-" : Convert.ToString(dr["Expected_Result"]),
                                                        sActual_Result = (dr["Actual_Result"] == null) ? "-" : Convert.ToString(dr["Actual_Result"]),
                                                        sAR_Attachment1_Name = (dr["AR_Attachment1_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_Name"]),
                                                        sAR_Attachment1_URL = (dr["AR_Attachment1_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment1_URL"]),
                                                        sAR_Attachment2_Name = (dr["AR_Attachment2_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_Name"]),
                                                        sAR_Attachment2_URL = (dr["AR_Attachment2_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment2_URL"]),
                                                        sAR_Attachment3_Name = (dr["AR_Attachment3_Name"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_Name"]),
                                                        sAR_Attachment3_URL = (dr["AR_Attachment3_URL"] == null) ? "-" : Convert.ToString(dr["AR_Attachment3_URL"]),
                                                        sType = "IDTS_Type"
                                                    });
                                                }                                          
                                        }
                                        oQueryResponse.lstTSByID = lstTSID;
                                        break;
                                        /************* ID ****************/
                                        /************************************************/
                                }
                            }
                        }
                    }

                }
                return Json(oQueryResponse);
                //oQueryResponse = GetStatus_core(oQueryResponse);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
                return Json(oQueryResponse);
        }
       



        private string GetConditionalStatement(string sRequest)
        {
            string retResult = "";
            string criteria = "";
            string lastcriteriaName = "";
            string lastcriteriaValue = "";
            string temp = "";


            if (!string.IsNullOrEmpty(sRequest))
            {
                string[] reqArr = sRequest.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);

                if (reqArr != null)
                {
                    foreach (var item in reqArr)
                    {
                        temp = item;
                        if (Convert.ToString(item.Split('~')[1]).Trim() != string.Empty && Convert.ToString(item.Split('~')[0]).Trim() != "UATItems")
                        {
                            criteria = item.Split('~')[0].Trim().ToLower().ToString();

                            switch (criteria)
                            {
                                case "testpass":
                                    //retResult += "TestPass_ID=" + item.Split('~')[1].Trim().ToLower() + " and ";
                                    //retResult += "TestPass_ID=" + item.Split('~')[1].Trim().ToLower() + Convert.ToString(item.Split('~')[2]).Trim();
                                    retResult += "TestPass_Name ='" + item.Split('~')[1].Trim().ToLower().ToString() + "' and ";
                                    break;

                                case "testcase":
                                    retResult += "TestCase_Name like '%" + item.Split('~')[1].Trim().ToLower().ToString() + "%' and ";
                                    break;

                                case "teststep":
                                    retResult += "TestStep_ActionName like '%" + item.Split('~')[1].Trim().ToLower().ToString() + "%' and ";
                                    break;

                                case "id":
                                    retResult += " " + item.Split('~')[1].Trim().ToLower() + " " + " " + "in(Project_Id,TestPass_ID,TestCase_ID,TestStep_ID) and ";
                                    break;

                                case "assignedto":
                                    //Version_Lead like ''%m%'' or TestManagerName like ''%m%'' or User_Name like ''%m%''
                                    retResult += "(Version_Lead like '%" + item.Split('~')[1].Trim().ToLower().ToString() +
                                                 "%' or TestManagerName like '%" + item.Split('~')[1].Trim().ToLower().ToString() +
                                                 "%' or User_Name like '%" + item.Split('~')[1].Trim().ToLower().ToString() + "%') and ";
                                    break;

                                case "status":
                                    /* retResult += "(ProjStatus = '" + item.Split('~')[1].Trim().ToLower() +
                                                  "' or TPStatus = '" + item.Split('~')[1].Trim().ToLower() +
                                                  "' or Status = '" + item.Split('~')[1].Trim().ToLower() + "') and ";*/
                                    retResult += " '" + item.Split('~')[1].Trim().ToLower().ToString() + "' " + " " + "in(ProjStatus,TPStatus,Status) and ";
                                    break;

                                case "statustp":
                                    retResult += " '" + item.Split('~')[1].Trim().ToLower().ToString() + "' " + " " + "in(TPStatus) and ";
                                    break;

                                case "statustc":
                                case "statustsrole":
                                    retResult += " '" + item.Split('~')[1].Trim().ToLower().ToString() + "' " + " " + "in(Status) and ";
                                    break;

                                /* case "StatusTSRole":
                                     retResult += " '" + item.Split('~')[1].Trim().ToLower().ToString() + "' " + " " + "in(Status) and ";
                                     break; */

                                case "roles":
                                    retResult += "Role_Name ='" + item.Split('~')[1].Trim().ToLower().ToString() + "' and ";
                                    break;

                            }
                        }
                    }

                    /* lastcriteriaName = reqArr[reqArr.Length - 1].Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(0);
                     lastcriteriaValue = reqArr[reqArr.Length - 1].Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1);*/

                    lastcriteriaName = Convert.ToString(temp.Split('~')[0]).Trim();
                    lastcriteriaValue = Convert.ToString(temp.Split('~')[1]).Trim();

                    if (!string.IsNullOrEmpty(retResult))
                        retResult = retResult.Remove(retResult.Length - 5).ToString() + "$" + lastcriteriaName + "$" + lastcriteriaValue;
                    else
                        retResult = " " + "$" + lastcriteriaName + "$" + lastcriteriaValue;
                }
                return retResult;
            }
            else
            {
                return null;
            }

        }

        //done
        private QueryResponseForAND GetStatus_core(QueryResponseForAND oQueryResponse)
        {
            if (oQueryResponse.lstTestCase != null && oQueryResponse.lstTestCase.Count > 0)
            {
                var lstTestCase = oQueryResponse.lstTestCase;
                lstTestCase = lstTestCase.OrderBy(z => z.sTestCaseId).OrderBy(z => z.sTesterId).OrderBy(z => z.sRoleId).ToList();

                string _tcId = lstTestCase[0].sTestCaseId, _tsrId = lstTestCase[0].sTesterId, _roleId = lstTestCase[0].sRoleId, _statId = lstTestCase[0].sTestStepStatusID;

                int _NC = 0;
                int _PASS = 0;
                int _Fail = 0;
                int _Pending = 0;

                for (int i = 0; i < lstTestCase.Count; i++)
                {
                    if (i==0)
                    {
                        _NC = lstTestCase.Where(z => z.sTestStepStatusID == "1" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                        _PASS = lstTestCase.Where(z => z.sTestStepStatusID == "2" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                        _Fail = lstTestCase.Where(z => z.sTestStepStatusID == "3" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                        _Pending = lstTestCase.Where(z => z.sTestStepStatusID == "4" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();

                        string status = "";

                        if (_Fail != 0)
                        {
                            status = "Fail";
                        }
                        else if (_NC != 0 || _Pending != 0)
                        {
                            status = "Not Completed";
                        }
                        else if (_PASS != 0)
                        {
                            status = "Pass";
                        }
                        lstTestCase.Where(z => z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).ToList().ForEach(z => z.sTCStatus = status);
                    }
                    else
                    {
                        if (!(_tcId == lstTestCase[i].sTestCaseId && _tsrId == lstTestCase[i].sTesterId && _roleId == lstTestCase[i].sRoleId))
                        {
                            _NC = lstTestCase.Where(z => z.sTestStepStatusID == "1" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                            _PASS = lstTestCase.Where(z => z.sTestStepStatusID == "2" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                            _Fail = lstTestCase.Where(z => z.sTestStepStatusID == "3" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();
                            _Pending = lstTestCase.Where(z => z.sTestStepStatusID == "4" && z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).Count();

                            string status = "";

                            if (_Fail != 0)
                            {
                                status = "Fail";
                            }
                            else if (_NC != 0 || _Pending != 0)
                            {
                                status = "Not Completed";
                            }
                            else if (_PASS != 0)
                            {
                                status = "Pass";
                            }
                            lstTestCase.Where(z => z.sTestCaseId == lstTestCase[i].sTestCaseId && z.sTesterId == lstTestCase[i].sTesterId && z.sRoleId == lstTestCase[i].sRoleId).ToList().ForEach(z => z.sTCStatus = status);
                        }
                    }
                }
                oQueryResponse.lstTestCase = lstTestCase;
            }
            return oQueryResponse;
        }


        //private string GetStatusForTestcase(string testCaseId, string testerId, string roleId)
        //{

        //    string status = "";
        //    if (dTable != null && !string.IsNullOrEmpty(testCaseId) && !string.IsNullOrEmpty(testerId) && !string.IsNullOrEmpty(roleId))
        //    {
        //        DataRow[] drNC = dTable.Select("TestStepStatusID = '1' and TestCase_ID='" + testCaseId + "' and User_Id='" + testerId + "' and Role_ID='" + roleId + "'");
        //        DataRow[] drPass = dTable.Select("TestStepStatusID = '2' and TestCase_ID='" + testCaseId + "' and User_Id='" + testerId + "' and Role_ID='" + roleId + "'");
        //        DataRow[] drFail = dTable.Select("TestStepStatusID = '3' and TestCase_ID='" + testCaseId + "' and User_Id='" + testerId + "' and Role_ID='" + roleId + "'");
        //        DataRow[] drPending = dTable.Select("TestStepStatusID = '4' and TestCase_ID='" + testCaseId + "' and User_Id='" + testerId + "' and Role_ID='" + roleId + "'");
        //        if (drPass.Count() != 0)
        //        {
        //            status = "Pass";
        //        }
        //        if (drNC.Count() != 0 || drPending.Count() != 0)
        //        {
        //            status = "Not Completed";
        //        }
        //        if (drFail.Count() != 0)
        //        {
        //            status = "Fail";
        //        }
        //        return status;
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}
    }
}
