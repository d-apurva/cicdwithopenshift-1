using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class TestingPgController : Controller
    {
        clsDbContext _context;

        #region 'Constructor'
        public TestingPgController(clsDbContext context)
        {

            _context = context;
        }
        #endregion


        #region 'GET'
        [HttpGet, Route("getProjectTestPass/{testPassId}&{roleId}")]
        public List<listProjectTestPass> getProjectTestPass(string testPassId, string roleId)
        {
            List<listProjectTestPass> ProjectTestPass = new List<listProjectTestPass>();
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];
                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                string SchemaName = "";

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
                    cmd.CommandText = "UAT.spTestingPage";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "SelectPTP" });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();



                    using (var dr = cmd.ExecuteReader())
                    {
                        listProjectTestPass projectDet = new listProjectTestPass();
                        List<listActualAliasUrls> urls = new List<listActualAliasUrls>();
                        List<listTestPassNames> tpNames = new List<listTestPassNames>();

                        while (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                projectDet.projectName = (dr["Project_Name"] == null) ? "" : Convert.ToString(dr["Project_Name"]);
                                projectDet.projectEndDate = (dr["PrjEndDate"] == null) ? "" : Convert.ToString(dr["PrjEndDate"]);
                                projectDet.testPassName = (dr["TestPass_Name"] == null) ? "" : Convert.ToString(dr["TestPass_Name"]);
                                projectDet.testPassManager = (dr["User_Name"] == null) ? "" : Convert.ToString(dr["User_Name"]);
                                projectDet.managerEmail = (dr["User_EmailId"] == null) ? "" : Convert.ToString(dr["User_EmailId"]);
                                projectDet.testPassStartDate = (dr["TPStartDate"] == null) ? "" : Convert.ToString(dr["TPStartDate"]);
                                projectDet.testPassEndDate = (dr["TPEndDate"] == null) ? "" : Convert.ToString(dr["TPEndDate"]);
                                projectDet.testingType = (dr["Testing_Type"] == null) ? "" : Convert.ToString(dr["Testing_Type"]);
                                projectDet.feedbackRType = (dr["FeedBack_Type"] == null) ? "" : Convert.ToString(dr["FeedBack_Type"]);

                                projectDet.testerArea = (dr["Area_Name"] == null) ? "" : Convert.ToString(dr["Area_Name"]);
                                projectDet.projectStatus = (dr["Project_Status"] == null) ? "" : Convert.ToString(dr["Project_Status"]);
                                projectDet.testPassStatus = (dr["TPStatus"] == null) ? "" : Convert.ToString(dr["TPStatus"]);
                                projectDet.testerRole = (dr["Role_Name"] == null) ? "" : Convert.ToString(dr["Role_Name"]);

                                urls.Add(new listActualAliasUrls()
                                {
                                    aliasUrl = (dr["Alias_Url"] == null) ? "" : Convert.ToString(dr["Alias_Url"]),
                                    actualUrl = (dr["Actual_Url"] == null) ? "" : Convert.ToString(dr["Actual_Url"]),
                                });

                            }
                            dr.NextResult();
                            while (dr.Read())
                            {
                                tpNames.Add(new listTestPassNames()
                                {
                                    testPassName = (dr["TestPass_Name"] == null) ? "" : Convert.ToString(dr["TestPass_Name"]),
                                });
                            }

                            projectDet.listActualAliasUrls = urls;
                            projectDet.listTestPassNames = tpNames;
                            ProjectTestPass.Add(projectDet);
                        }

                    }
                    return ProjectTestPass;
                }
            }
            catch (Exception)
            {

                return ProjectTestPass;
            }
        }

        [HttpGet, Route("GetTestCasesTestSteps/{testPassId}&{roleId}")]
        public List<TestingTestCases> GetTestCasesTestSteps(string testPassId, string roleId)
        {
            List<TestingTestCases> TestCasesTestSteps = new List<TestingTestCases>();
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];
                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                string SchemaName = "";

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
                    cmd.CommandText = "UAT.spTestingPage";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "SelectTCTS" });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();



                    using (var dr = cmd.ExecuteReader())
                    {

                        List<int> TCIds = new List<int>();
                        TestingTestCases TestCases = null;
                        List<listTestStep> TestSteps = null;
                        int count = 0;
                        int dr_Row = 0;

                        List<clsTestPg> lst = new List<clsTestPg>();
                        while (dr.Read())
                        {
                            lst.Add(new clsTestPg()
                            {
                                Actual_Result = Convert.ToString(dr["Actual_Result"]),
                                AR_Attachment1_URL = Convert.ToString(dr["AR_Attachment1_URL"]),
                                AR_Attachment2_URL = Convert.ToString(dr["AR_Attachment2_URL"]),
                                AR_Attachment3_URL = Convert.ToString(dr["AR_Attachment3_URL"]),
                                ER_Attachment_URL = Convert.ToString(dr["ER_Attachment_URL"]),
                                ETT = Convert.ToString(dr["ETT"]),
                                Expected_Result = Convert.ToString(dr["Expected_Result"]),
                                Status = Convert.ToString(dr["Status"]),
                                TestCase_Description = Convert.ToString(dr["TestCase_Description"]),
                                TestCase_ID = Convert.ToString(dr["TestCase_ID"]),
                                TestCase_Name = Convert.ToString(dr["TestCase_Name"]),
                                TestCase_Sequence = Convert.ToString(dr["TestCase_Sequence"]),
                                TestStepPlan_Id = Convert.ToString(dr["TestStepPlan_Id"]),
                                TestStep_ActionName = Convert.ToString(dr["TestStep_ActionName"]),
                                TestStep_Sequence = Convert.ToString(dr["TestStep_Sequence"]),
                            });
                        }

                        if (lst.Count > 0)
                        {
                            dr_Row = lst.Count;
                            for (int i = 0; i < dr_Row; i++)
                            {
                                count++;

                                int tcid = Convert.ToInt32(lst[i].TestCase_ID);
                                if (!TCIds.Contains(tcid))
                                {
                                    TCIds.Add(tcid);
                                    TestCases = new TestingTestCases();
                                    TestCases.testCaseId = lst[i].TestCase_ID;
                                    TestCases.testCaseName = lst[i].TestCase_Name;
                                    TestCases.testCaseDescription = lst[i].TestCase_Description;
                                    TestCases.testCaseSeq = lst[i].TestCase_Sequence;
                                    TestCases.testCaseETT = lst[i].ETT;

                                    TestSteps = new List<listTestStep>();
                                    TestSteps.Add(new listTestStep()
                                    {
                                        testStepPlanId = lst[i].TestStepPlan_Id,
                                        testStepName = lst[i].TestStep_ActionName,
                                        expResult = lst[i].Expected_Result,
                                        actResult = lst[i].Actual_Result,
                                        status = lst[i].Status,
                                        testStepSeq = lst[i].TestStep_Sequence,

                                        expAttachment = lst[i].ER_Attachment_URL,
                                        actAttachment1 = lst[i].AR_Attachment1_URL,
                                        actAttachment2 = lst[i].AR_Attachment2_URL,
                                        actAttachment3 = lst[i].AR_Attachment3_URL,
                                    });
                                    if (dr_Row == count)
                                    {
                                        TestCases.listTestStep = TestSteps;
                                        TestCasesTestSteps.Add(TestCases);
                                    }
                                    else if (Convert.ToInt32(lst[count].TestCase_ID) != tcid)
                                    {
                                        TestCases.listTestStep = TestSteps;
                                        TestCasesTestSteps.Add(TestCases);
                                    }
                                }
                                else
                                {
                                    TestSteps.Add(new listTestStep()
                                    {
                                        testStepPlanId = lst[i].TestStepPlan_Id,
                                        testStepName = lst[i].TestStep_ActionName,
                                        expResult = lst[i].Expected_Result,
                                        actResult = lst[i].Actual_Result,
                                        status = lst[i].Status,
                                        testStepSeq = lst[i].TestStep_Sequence,

                                        expAttachment = lst[i].ER_Attachment_URL,
                                        actAttachment1 = lst[i].AR_Attachment1_URL,
                                        actAttachment2 = lst[i].AR_Attachment2_URL,
                                        actAttachment3 = lst[i].AR_Attachment3_URL,
                                    });

                                    if (dr_Row == count)
                                    {
                                        TestCases.listTestStep = TestSteps;
                                        TestCasesTestSteps.Add(TestCases);
                                    }
                                    else if (Convert.ToInt32(lst[count].TestCase_ID) != tcid)
                                    {
                                        TestCases.listTestStep = TestSteps;
                                        TestCasesTestSteps.Add(TestCases);
                                    }
                                }

                            }


                        }
                        return TestCasesTestSteps;


                    }

                }
            }
            catch (Exception ex)
            {

                return TestCasesTestSteps;
            }
        }

        [HttpGet, Route("getFeedbackRating/{testPassId}&{roleId}&{testCaseId}")]
        public List<TestingFR> getFeedbackRating(string testPassId, string roleId, string testCaseId)
        {
            List<TestingFR> FeedbackRating = new List<TestingFR>();
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            string SchemaName = "";

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
                cmd.CommandText = "UAT.spTestingPage";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testPassId });
                cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "SelectFR" });

                if (!string.IsNullOrEmpty(testCaseId) && testCaseId != "0")///* Test Case Level Feedback Rating*/
                {
                    cmd.Parameters.Add(new SqlParameter("@TestCaseId", SqlDbType.VarChar, 500) { Value = testCaseId });
                    cmd.Parameters.Add(new SqlParameter("@FeedbackType", SqlDbType.VarChar, 500) { Value = "1" });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@FeedbackType", SqlDbType.VarChar, 500) { Value = "0" });
                }


                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();



                using (var dr = cmd.ExecuteReader())
                {
                  
                    while (dr.Read())
                    {
                        FeedbackRating.Add(new TestingFR()
                        {
                            feedbackId = (dr["FeedBack_Id"] == null) ? "" : Convert.ToString(dr["FeedBack_Id"]),
                            rating = (dr["Rating"] == null) ? "" : Convert.ToString(dr["Rating"]),
                            feedback = (dr["FeedBack"] == null) ? "" : Convert.ToString(dr["FeedBack"])
                        });
                    }
                  
                }
                return FeedbackRating;
            }
        }
        #endregion


        #region 'POST'
        [HttpPost, Route("AddActAttachment")]
        public Dictionary<string, string> AddActAttachment([FromBody] actualAttachment oActResult)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            #region CheckingNullValues

            if (string.IsNullOrEmpty(oActResult.testStepPlanId))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "teststepPlanId is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oActResult.actAttachmentName))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "Actual Attachment Name is required");
                return _result;
            }
            /*
            else if (string.IsNullOrEmpty(oActResult.actAttachmentUrl))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "Actual Attachment URL is required");
                return _result;
            }
            */
            else if (string.IsNullOrEmpty(oActResult.actAttachmentIndex))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "Actual Attachment index is required");
                return _result;
            }
            #endregion
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
                    _result.Add(clsGlobalVariables._errorText, "Invalid Appurl");
                    return _result;

                }
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    string statementType = string.Empty;


                    cmd.CommandText = "UAT.spTestingPage";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlanId", SqlDbType.VarChar, 500) { Value = oActResult.testStepPlanId });
                    cmd.Parameters.Add(new SqlParameter("@ActAttachmentName", SqlDbType.VarChar, 500) { Value = oActResult.actAttachmentName });
                    cmd.Parameters.Add(new SqlParameter("@ActUrl", SqlDbType.VarChar, 500) { Value = oActResult.actAttachmentUrl });
                    cmd.Parameters.Add(new SqlParameter("@ActAttachmentIndex", SqlDbType.VarChar, 500) { Value = oActResult.actAttachmentIndex });
                    cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._statementTypeParameterName, SqlDbType.VarChar, 500) { Value = "ActUPDATE" });
                    cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._schemaNameParameterName, SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add("Value", _retValue);
                            _result.Add(clsGlobalVariables._statusText, "Done");
                        }
                        else
                            _result.Add(clsGlobalVariables._statusText, "Done");
                    }



                }


            }
            catch (Exception ex)
            {

                _result.Add(clsGlobalVariables._errorText, ex.Message);
            }
            return _result;
        }

        [HttpPost, Route("UpdateTesting")]
        public Dictionary<string, string> UpdateTesting([FromBody]  TestingTSPlan oTSPlan)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            #region CheckingNullValues

            if (string.IsNullOrEmpty(oTSPlan.testStepPlanId))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "teststepPlanId is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oTSPlan.status))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "status is required");
                return _result;
            }
            #endregion
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
                    _result.Add(clsGlobalVariables._errorText, "Invalid Appurl");
                    return _result;

                }
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    string statementType = string.Empty;


                    cmd.CommandText = "UAT.spTestingPage";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlanId", SqlDbType.VarChar, 500) { Value = oTSPlan.testStepPlanId });
                    cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 500) { Value = oTSPlan.status });
                    cmd.Parameters.Add(new SqlParameter("@ActualResult", SqlDbType.NVarChar, -1) { Value = oTSPlan.actResult });
                    cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._statementTypeParameterName, SqlDbType.VarChar, 500) { Value = "UPDATET" });
                    cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._schemaNameParameterName, SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add("Value", _retValue);
                            _result.Add(clsGlobalVariables._statusText, "Done");
                        }
                        else
                            _result.Add(clsGlobalVariables._statusText, "Done");
                    }



                }


            }
            catch (Exception ex)
            {

                _result.Add(clsGlobalVariables._errorText, ex.Message);
            }
            return _result;
        }

        [HttpPost, Route("InsertUpdateFR")]
        public Dictionary<string, string> InsertUpdateFR([FromBody] clsTestingFR oFeedback)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            #region CheckingNullValues

            if (string.IsNullOrEmpty(oFeedback.rating))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "Rating is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oFeedback.testPassId) && string.IsNullOrEmpty(oFeedback.feedbackId))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "testPassId is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oFeedback.userId) && string.IsNullOrEmpty(oFeedback.feedbackId))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "SPUserId is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oFeedback.roleId) && string.IsNullOrEmpty(oFeedback.feedbackId))
            {
                //send service level Exception as service response
                _result.Add(clsGlobalVariables._errorText, "roleId is required");
                return _result;
            }
            #endregion
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
                    _result.Add(clsGlobalVariables._errorText, "Invalid Appurl");
                    return _result;

                }
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    string statementType = string.Empty;


                    cmd.CommandText = "UAT.spTestingPage";
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (string.IsNullOrEmpty(oFeedback.feedbackId))
                    {
                        cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = oFeedback.testPassId });
                        cmd.Parameters.Add(new SqlParameter("@TestCaseId", SqlDbType.VarChar, 500) { Value = oFeedback.testCaseId });
                        cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.VarChar, 500) { Value = oFeedback.userId });
                        cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.VarChar, 500) { Value = oFeedback.roleId });
                        cmd.Parameters.Add(new SqlParameter("@Rating", SqlDbType.VarChar, 500) { Value = oFeedback.rating });
                        cmd.Parameters.Add(new SqlParameter("@Feedback", SqlDbType.VarChar, 500) { Value = oFeedback.feedback });
                        cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._statementTypeParameterName, SqlDbType.VarChar, 500) { Value = "InsertFR" });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@FeedbackId", SqlDbType.VarChar, 500) { Value = oFeedback.feedbackId });
                        cmd.Parameters.Add(new SqlParameter("@Rating", SqlDbType.VarChar, 500) { Value = oFeedback.rating });
                        cmd.Parameters.Add(new SqlParameter("@Feedback", SqlDbType.VarChar, 500) { Value = oFeedback.feedback });
                        cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._statementTypeParameterName, SqlDbType.VarChar, 500) { Value = "UpdateFR" });
                    }



                    cmd.Parameters.Add(new SqlParameter(clsGlobalVariables._schemaNameParameterName, SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add("ID", _retValue);
                            _result.Add(clsGlobalVariables._statusText, "Done");
                        }
                        else
                            _result.Add(clsGlobalVariables._statusText, "Done");
                    }



                }


            }
            catch (Exception ex)
            {

                _result.Add(clsGlobalVariables._errorText, ex.Message);
            }
            return _result;
        }
        #endregion

        #region 'Attachment'
        public bool inFile(IFormFile file)
        {
            return (file != null && file.Length > 0) ? true : false;
        }
        [HttpGet, Route("CheckFileCount/{id}")]
        public string CheckFileCount(int id)
        {
            string _count = "0";
            try
            {
                string SchemaName = "";

                string AppUrl = HttpContext.Request.Headers["appurl"];

                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {

                    return "Invalid URL";
                }


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.CheckFileCount";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@childID", SqlDbType.NVarChar, 500) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@schemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            _count = Convert.ToString(dataReader[0]);
                        }


                    }

                }
            }
            catch (Exception ex)
            {

                _count = ex.Message;
            }
            return _count;
        }

        [HttpPost, Route("DeleteAtachmentonExceeds")]
        public string DeleteAtachmentonExceeds([FromBody] clsAttachment childid)
        {
            string _result = "";
            try
            {
                string SchemaName = "";

                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    return "Invalid URL";

                }


                if (childid._childId == "" && childid == null)
                {
                    return _result = "Error";
                }
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "UAT.DeleteAtachmentIfExceeds";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.NVarChar, 500) { Value = childid._childId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@outval", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = cmd.ExecuteNonQuery();
                    bool outval = (bool)cmd.Parameters["@outval"].Value;

                    if (i != 0 && outval != false)
                    {
                        _result = "Done";
                    }
                }
            }
            catch (Exception ex)
            {

                _result = ex.Message;
            }
            return _result;
        }

        [HttpGet, Route("GetAttachmentAfterUpload/{ChildId}")]
        public JsonResult GetAttachmentAfterUpload(string childid)
        {
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    return Json("Invalid Appurl");

                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.GetAttachmentAfterUpload";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.NVarChar, 500) { Value = childid });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                var dataRow = new ExpandoObject() as IDictionary<string, object>;

                                for (var iFiled = 0; iFiled < dr.FieldCount; iFiled++)
                                {

                                    var value = dr.GetName(iFiled);
                                    var name = dr.IsDBNull(iFiled) ? null : dr[iFiled];

                                    dataRow.Add(value, Convert.ToString(name));
                                }


                                retObject.Add((ExpandoObject)dataRow);
                            }



                        }



                    }

                    return Json(retObject);


                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        [HttpPost, Route("UploadAttachment")]
        public IActionResult UploadAttachment()
        {

            clsAttachment _at = null;
            string _msg = "";
            try
            {
                string SchemaName = "";


                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    _msg = "Invalid Url";

                }


                var files = HttpContext.Request.Form.Files;
                var _data = HttpContext.Request.Form;
                foreach (var file in files)
                {
                    _at = new clsAttachment();
                    _at._childId = Convert.ToString(_data["childid"]);
                    _at.AttachmentName = Convert.ToString(_data["AttachmentName"]);
                    _at.ActualResult = Convert.ToString(_data["AcutualResult"]);

                    _at.Description = Convert.ToString(_data["Description"]);
                    _at.FileName = Convert.ToString(_data["FileName"]);
                    _at.ProjectID = Convert.ToInt32(_data["ProjectID"]);
                    _at.ResultType = Convert.ToString(_data["ResultType"]);
                    _at.TestCaseID = Convert.ToInt32(_data["TestCaseID"]);
                    _at.TestPassID = Convert.ToInt32(_data["TestPassID"]);
                    _at.TestStepID = Convert.ToString(_data["TestStepID"]);

                }


                if (inFile(Request.Form.Files[0]))
                {


                    string fileType = Request.Form.Files[0].ContentType;
                    Stream file_strm = Request.Form.Files[0].OpenReadStream();
                    string file_Name = Path.GetFileName(Request.Form.Files[0].FileName);
                    int fileSize = Convert.ToInt32(Request.Form.Files[0].Length);
                    byte[] fileRecord = new byte[fileSize];
                    file_strm.Read(fileRecord, 0, fileSize);

                    _at.ContentType = fileType;
                    _at.FileName = file_Name;
                    using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                    {



                        cmd.CommandText = "UAT.AddAttachment";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TestStepID", SqlDbType.VarChar, 500) { Value = _at.TestStepID });
                        cmd.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.VarChar, 500) { Value = _at._childId });
                        cmd.Parameters.Add(new SqlParameter("@AttachmentName", SqlDbType.VarChar, 500) { Value = _at.AttachmentName });
                        cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 500) { Value = _at.Description });
                        cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 500) { Value = _at.FileName });
                        cmd.Parameters.Add(new SqlParameter("@ResultType", SqlDbType.VarChar, 500) { Value = _at.ResultType });
                        cmd.Parameters.Add(new SqlParameter("@ActualResult", SqlDbType.NVarChar, -1) { Value = _at.ActualResult });
                        cmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.Int) { Value = Convert.ToInt32(_at.ProjectID) });
                        cmd.Parameters.Add(new SqlParameter("@TestPassID", SqlDbType.Int) { Value = Convert.ToInt32(_at.TestPassID) });
                        cmd.Parameters.Add(new SqlParameter("@TestCaseID", SqlDbType.Int) { Value = Convert.ToInt32(_at.TestCaseID) });
                        cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar, 500) { Value = fileType });
                        cmd.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary, 5000000) { Value = fileRecord });
                        cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                        cmd.Parameters.Add(new SqlParameter("@outval", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();

                        int i = cmd.ExecuteNonQuery();
                        string outval = Convert.ToString(cmd.Parameters["@outval"].Value);

                        if (i != 0 && outval != "" && outval != "0")
                        {
                            _msg = ("Success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _msg = (ex.Message);
            }
            return Json(_msg);
        }

        [HttpGet, Route("GetAttachmentFile")]
        public FileContentResult GetAttachmentFile(int id,string _Appurl)
        {
             var filename = "";
             var contentType = "";
            byte[] fileContent = null;
            try
            {
                string SchemaName = "";

                string AppUrl = _Appurl;

                

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {


                }

               

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.GetAttachmentFile";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@attachmentId", SqlDbType.NVarChar, 500) { Value = id });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["imagename"].ToString();
                            contentType = dataReader["ContentType"].ToString();
                            fileContent = (byte[])dataReader["FileData"];
                            Response.ContentType = dataReader["ContentType"].ToString();
                            Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["imagename"].ToString());
                            Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                        }

                       
                    }

                }
            }
            catch (Exception ex)
            {

              
            }
        return File(fileContent, contentType, filename);

        }

       

        [HttpGet, Route("GetExpectedAttachmentIDbyTCID/{TCID}")]
        public List<clsAttachmetnIDs> GetExpectedAttachmentIDbyTCID(string TCID)
        {
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    return null;

                }
                List<clsAttachmetnIDs> _ids = new List<clsAttachmetnIDs>();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.GetIdforExpectedAttachment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TCId", SqlDbType.NVarChar, 500) { Value = TCID });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            _ids.Add(new clsAttachmetnIDs() { Ids = Convert.ToString(dr[0]) });
                        }


                        return _ids;




                    }




                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        [HttpGet, Route("GetAttachmentIDbyChildID/{childID}")]
        public List<clsAttachmetnIDs> GetAttachmentIDbyChildID(string childID)
        {
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];
                string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                string SchemaName = "";
                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {
                    return null;

                }
                List<clsAttachmetnIDs> _ids = new List<clsAttachmetnIDs>();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.GetAttachmentIDbyChildID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChildId", SqlDbType.NVarChar, 500) { Value = childID });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            _ids.Add(new clsAttachmetnIDs() { Ids = Convert.ToString(dr[0]) });
                        }


                        return _ids;




                    }




                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        #endregion
    }
    #region 'Attachment Class'
    public class clsAttachment
    {
        public string _childId { get; set; }
        public string TestStepID { get; set; }
        public string AttachmentName { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string ResultType { get; set; }
        public int ProjectID { get; set; }
        public int TestPassID { get; set; }
        public int TestCaseID { get; set; }
        public string ActualResult { get; set; }
        public string ContentType { get; set; }
        public string FileData { get; set; }
    }
    public class clsAttachmetnIDs
    {
        public string Ids { get; set; }
    }
    #endregion
}
