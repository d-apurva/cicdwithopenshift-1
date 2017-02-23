
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

/* For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860 */

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class FeedbackController : Controller
    {
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public FeedbackController(clsDbContext context)
        {
            _context = context;
        }

        /* GET: api/values*/
        [HttpGet]
        public string Get()
        {
            return "value";
        }

        /* GET api/values/5*/
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /*Get TestPass/TestCase level feedback, rating value along with Feedback_Type configured feedback type in TestPass Table
         Also get Feedback of Tester from TestStepPlan Table for a TestStep with a particular role of Tester*/
         [HttpGet, Route("GetFeedback/{projectId}/{type}")]
        public List<clsFeedBack> GetFeedback(int projectId, string type)
        {
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
                string status = string.Empty;
                string statementType = string.Empty;

                List<clsFeedBack> listFB = new List<clsFeedBack>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetFeedBack";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = Convert.ToInt32(projectId) });
                    cmd.Parameters.Add(new SqlParameter("@LoginSpUserId", SqlDbType.Int) { Value = Convert.ToInt32(_SpUserId) });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        List<clsTestFeedBack> lst = new List<clsTestFeedBack>();
                        int count = 0;
                        int dr_Row = 0;

                        while (dr.Read())
                        {
                            lst.Add(new clsTestFeedBack()
                            {
                                testpassId = Convert.ToString(dr["TestPass_Id"]),
                                testcaseId = Convert.ToString(dr["TestCase_Id"]),
                                Role_ID = Convert.ToString(dr["Role_ID"]),
                                userId = Convert.ToString(dr["spUser_Id"]),
                                testpassName = Convert.ToString(dr["TestPass_Name"]),
                                testcaseName = Convert.ToString(dr["TestCase_Name"]),
                                testerName = Convert.ToString(dr["Tester_Name"]),
                                Role_Name = Convert.ToString(dr["Role_Name"]),
                                Feedback_Type = Convert.ToString(dr["Feedback_Type"]),
                                Feedback = Convert.ToString(dr["Feedback"]),
                                Rating = Convert.ToString(dr["Rating"]),
                                RatingFeedId = Convert.ToString(dr["RatingFeedId"]),
                                TestStep_ID = Convert.ToString(dr["TestStep_ID"]),
                                TestStep_ActionName = Convert.ToString(dr["TestStep_ActionName"]),
                                Expected_Result = Convert.ToString(dr["Expected_Result"]),
                                TestStepPlan_Id = Convert.ToString(dr["TestStepPlan_Id"]),
                                Actual_Result = Convert.ToString(dr["Actual_Result"]),
                                TesterStatusText = Convert.ToString(dr["TesterStatusText"]),
                                TspFeedback = Convert.ToString(dr["TspFeedback"]),
                            });
                        }
                                clsFeedBack fbObject = null;
                                List<FeedbackUnique> lstUnique = new List<FeedbackUnique>();
                                List<FeedbackTesterDetail> listTesterDetail = null;

                        if (lst.Count > 0)
                        {
                            dr_Row = lst.Count;
                            for (int i = 0; i < dr_Row; i++)
                            {
                                lst = lst.OrderBy(z => z.testpassId).OrderBy(z => z.testcaseId).OrderBy(z => z.Role_ID).OrderBy(z => z.userId).ToList();

                               
                                count++;
                                int tpid = Convert.ToInt32(lst[i].testpassId);
                                int tcid = Convert.ToInt32(lst[i].testcaseId);
                                int rid = Convert.ToInt32(lst[i].Role_ID);
                                int uid = Convert.ToInt32(lst[i].userId);

                                if (lstUnique.Any(z => z.TPassId == tpid && z.TCaseId == tcid && z.RoleId == rid && z.spUserId == uid) == false)
                                {
                                    lstUnique.Add(new FeedbackUnique()
                                    {
                                        TPassId = tpid,
                                        TCaseId = tcid,
                                        RoleId = rid,
                                        spUserId = uid
                                    });

                                    fbObject = new clsFeedBack();

                                    fbObject.testpassId = lst[i].testpassId;
                                    fbObject.testpassName = lst[i].testpassName;
                                    fbObject.testcaseId = lst[i].testcaseId;
                                    fbObject.testcaseName = lst[i].testcaseName;
                                    fbObject.userId = lst[i].userId;
                                    fbObject.testerName = lst[i].testerName;

                                    listTesterDetail = new List<FeedbackTesterDetail>();

                                    if (type == "0")/*testpass*/
                                    {
                                        listTesterDetail.Add(new FeedbackTesterDetail()
                                        {
                                            roleId = lst[i].Role_ID,
                                            roleName = lst[i].Role_Name,
                                            fBType = lst[i].Feedback_Type,
                                            tpTcFeedback = lst[i].Feedback,
                                            tpTcRating = lst[i].Rating,
                                            feedbackRatingId = lst[i].RatingFeedId
                                        });
                                    }
                                    else if (type == "1")/*teststep*/
                                    {
                                        listTesterDetail.Add(new FeedbackTesterDetail()
                                        {
                                            testStepId = lst[i].TestStep_ID,
                                            testStepName = lst[i].TestStep_ActionName,
                                            expectedResult = lst[i].Expected_Result,
                                            testplanId = lst[i].TestStepPlan_Id,
                                            actualResult = lst[i].Actual_Result,
                                            roleId = lst[i].Role_ID,
                                            roleName = lst[i].Role_Name,
                                            status = lst[i].TesterStatusText,
                                            tsFeedback = lst[i].TspFeedback
                                        });
                                    }
                                    if (dr_Row == count)
                                    {
                                        fbObject.listTesterDetail = listTesterDetail;
                                        listFB.Add(fbObject);
                                    }
                                    else if (Convert.ToInt32(lst[count].testpassId) != tpid || Convert.ToInt32(lst[count].testcaseId) != tcid || Convert.ToInt32(lst[count].Role_ID) != rid || Convert.ToInt32(lst[count].userId) != uid)
                                    {
                                        fbObject.listTesterDetail = listTesterDetail;
                                        listFB.Add(fbObject);
                                    }
                                }
                                else
                                {
                                    if (type == "0")
                                    {
                                        listTesterDetail.Add(new FeedbackTesterDetail()
                                        {
                                            roleId = lst[i].Role_ID,
                                            roleName = lst[i].Role_Name,
                                            fBType = lst[i].Feedback_Type,
                                            tpTcFeedback = lst[i].Feedback,
                                            tpTcRating = lst[i].Rating,
                                            feedbackRatingId = lst[i].RatingFeedId
                                        });

                                    }
                                    else if (type == "1")
                                    {
                                        listTesterDetail.Add(new FeedbackTesterDetail()
                                        {
                                            testStepId = lst[i].TestStep_ID,
                                            testStepName = lst[i].TestStep_ActionName,
                                            expectedResult = lst[i].Expected_Result,
                                            testplanId = lst[i].TestStepPlan_Id,
                                            actualResult = lst[i].Actual_Result,
                                            roleId = lst[i].Role_ID,
                                            roleName = lst[i].Role_Name,
                                            status = lst[i].TesterStatusText,
                                            tsFeedback = lst[i].TspFeedback
                                        });
                                    }
                                    if (dr_Row == count)
                                    {
                                        fbObject.listTesterDetail = listTesterDetail;
                                        listFB.Add(fbObject);
                                    }
                                    else if (Convert.ToInt32(lst[count].testpassId) != tpid || Convert.ToInt32(lst[count].testcaseId) != tcid || Convert.ToInt32(lst[count].Role_ID) != rid || Convert.ToInt32(lst[count].userId) != uid)
                                    {
                                        fbObject.listTesterDetail = listTesterDetail;
                                        listFB.Add(fbObject);
                                    }
                                }
                            }
                        }

                        return listFB;
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*Insert Feedback in TestStepPlan Table from Feedback Page*/
        [HttpPost, Route("InsertUpdateFeedBack")]
        public Dictionary<string, string> InsertUpdateFeedBack([FromBody]FeedBackTestSteps oFeedback)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
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
                    _result.Add(this._errorText, "Invalid Appurl");
                    return _result;
                }
                if (string.IsNullOrEmpty(oFeedback.feedback))
                {
                    _result.Add(this._errorText, "Feedback is required");
                    return _result;
                }
                else if (string.IsNullOrEmpty(oFeedback.testStepPlanId))
                {
                    _result.Add(this._errorText, "TestStepPlanId is required");
                    return _result;
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spFeedbackInsUpd";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Feedback", SqlDbType.VarChar, 8000) { Value = oFeedback.feedback });
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlanId", SqlDbType.Int) { Value = oFeedback.testStepPlanId });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = new clsUtility().GetLoggedInUserSPUserId() == "" ? null : new clsUtility().GetLoggedInUserSPUserId() });
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


        [HttpDelete, Route("DeleteFeedback")]
        public Dictionary<string, string> DeleteFeedback(string feedbackId)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            try
            {
                List<TestCase> listProjectUsers = new List<TestCase>();
                if (string.IsNullOrEmpty(feedbackId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "Feedback Id is required!");
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
                    cmd.CommandText = "[UAT].[spfeedbackRating]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@feedbackId", SqlDbType.Int) { Value = feedbackId });
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

        /* Get Data for Detailed Analysis Page*/
        [HttpGet, Route("GetDropdownDataForDetailAnalysis_Portfolio")]
        public List<DropdownDataForDetailAnalysis> GetDropdownDataForDetailAnalysis_Portfolio()
        {
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
                    return null;
                }
                List<DropdownDataForDetailAnalysis> detailedAnalysisResult = new List<DropdownDataForDetailAnalysis>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spDetailAnalysisDropDown_Portfolio";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    TesterList lst = new TesterList();
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            List<clsTestterList> oTester = new List<clsTestterList>();

                            #region ' XMl Parsing '

                            #region Project Lead Conversion

                            string _versionLeadName = string.Empty;
                            if (dataReader["projectLead"] != null)
                            {
                                using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + dataReader["projectLead"] + "</users>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList TesterList = xml.GetElementsByTagName("user");
                                    foreach (XmlNode node in TesterList)
                                    {
                                        if (node["userName"] != null)
                                            _versionLeadName = node["userName"].InnerText;
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region Test Manager Conversion

                            string _testManagerName = string.Empty;
                            if (dataReader["testManagerDetails"] != null)
                            {
                                using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + dataReader["testManagerDetails"] + "</users>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList xmlNodeList = xml.GetElementsByTagName("user");
                                    foreach (XmlNode node in xmlNodeList)
                                    {
                                        if (node["userName"] != null)
                                            _testManagerName = node["userName"].InnerText;

                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region Tester Conversion

                            string _testerSPID = string.Empty, _testerName = string.Empty;
                            if (dataReader["rowTesterDetails"] != null)
                            {
                                using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + dataReader["rowTesterDetails"] + "</users>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList xmlNodeList = xml.GetElementsByTagName("user");
                                    foreach (XmlNode node in xmlNodeList)
                                    {
                                        if (node["spUserId"] != null)
                                            _testerSPID = node["spUserId"].InnerText;

                                        if (node["userName"] != null)
                                            _testerName = node["userName"].InnerText;

                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region Tester Conversion New
                            string _tSPID = string.Empty, _tName = string.Empty;
                            string _tRoleID = string.Empty, _tRoleName = string.Empty;
                            if (dataReader["TesterDetailsRowNew"] != null)
                            {
                                using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + dataReader["TesterDetailsRowNew"] + "</users>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList xmlNodeList = xml.GetElementsByTagName("user");
                                    foreach (XmlNode node in xmlNodeList)
                                    {
                                        XmlElement companyElement = (XmlElement)node;

                                        if (node["TUserId"] != null)
                                            _tSPID = node["TUserId"].InnerText;

                                        if (node["TUserName"] != null)
                                            _tName = node["TUserName"].InnerText;

                                        if (node["TRoleId"] != null)
                                            _tRoleID = node["TRoleId"].InnerText;

                                        if (node["TRoleName"] != null)
                                            _tRoleName = node["TRoleName"].InnerText;

                                        oTester.Add(new clsTestterList
                                        {
                                            testerId = (companyElement.GetElementsByTagName("TUserId") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TUserId")[0].InnerText) : "",
                                            testerName = (companyElement.GetElementsByTagName("TUserName") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TUserName")[0].InnerText) : "",
                                            roleId = (companyElement.GetElementsByTagName("TRoleId") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TRoleId")[0].InnerText) : "",
                                            roleName = (companyElement.GetElementsByTagName("TRoleName") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TRoleName")[0].InnerText) : ""
                                        });
                                    }
                                }
                            }
                            #endregion

                            #endregion

                            int projectId = dataReader.GetOrdinal("projectId");
                            int testPassId = dataReader.GetOrdinal("testPassId");
                            int testPassName = dataReader.GetOrdinal("testPassName");
                            int tpEndDate = dataReader.GetOrdinal("tpEndDate");
                            int roleId = dataReader.GetOrdinal("roleId");
                            int roleName = dataReader.GetOrdinal("roleName");
                            int projectName = dataReader.GetOrdinal("projectName");
                            int projectVersion = dataReader.GetOrdinal("projectVersion");

                            detailedAnalysisResult.Add(new DropdownDataForDetailAnalysis()
                            {
                                projectId = (dataReader.IsDBNull(projectId)) == true ? "" : Convert.ToString(dataReader["projectId"]),
                                projectName = (dataReader.IsDBNull(projectName)) == true ? "" : Convert.ToString(dataReader["projectName"]),
                                projectVersion = (dataReader.IsDBNull(projectVersion)) == true ? "" : Convert.ToString(dataReader["projectVersion"]),
                                versionLead = _versionLeadName,

                                testpassId = (dataReader.IsDBNull(testPassId)) == true ? "" : Convert.ToString(dataReader["testPassId"]),
                                testpassName = (dataReader.IsDBNull(testPassName)) == true ? "" : Convert.ToString(dataReader["testPassName"]),
                                testManager = _testManagerName,
                                tpEndDate = (dataReader.IsDBNull(tpEndDate)) == true ? "" : Convert.ToString(dataReader["tpEndDate"]),

                                testerId = _testerSPID,
                                testerName = _testerName,
                                lstTesterList = oTester,
                                roleId = (dataReader.IsDBNull(roleId)) == true ? "" : Convert.ToString(dataReader["roleId"]),
                                roleName = (dataReader.IsDBNull(roleName)) == true ? "" : Convert.ToString(dataReader["roleName"]),
                            });
                        }
                        return detailedAnalysisResult;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
