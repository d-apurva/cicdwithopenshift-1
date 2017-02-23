
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
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class TriageController : Controller
    {
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;

        public TriageController(clsDbContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return "value";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // Get DropDown data for Traige Page
        [HttpGet, Route("GetTestPassTriagePage")]
        public List<GrpPortfolioProjectTP> GetTestPassTriagePage()
        {
            List<GrpPortfolioProjectTP> groupPortfolioProjectTestPassResult = new List<GrpPortfolioProjectTP>();

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
                    cmd.CommandText = "UAT.spGetGroupPortfolioProjectTestPassTriagePage_core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;

                            List<TestPassTesterRolesNew> _tester = new List<TestPassTesterRolesNew>();

                            #region Project Lead Conversion

                            string _projectLeadEmail = string.Empty;
                            string _projectLeadName = string.Empty;
                            if (dataReader["projectLead"] != null)
                            {
                                // Name = "lstprojectLead";
                                using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + dataReader["projectLead"] + "</users>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList TesterList = xml.GetElementsByTagName("user");
                                    foreach (XmlNode node in TesterList)
                                    {
                                        if (node["userEmail"] != null)
                                            _projectLeadEmail = node["userEmail"].InnerText;
                                         if (node["userName"] != null)
                                            _projectLeadName = node["userName"].InnerText;

                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region Tester Conversion
                            if (dataReader["testersCollection"] != null)
                            {
                                // Name = "lstTesterRole";
                                using (XmlReader reader = XmlReader.Create(new StringReader("<testers>" + dataReader["testersCollection"] + "</testers>")))
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(reader);
                                    XmlNodeList TesterList = xml.GetElementsByTagName("tester");
                                    foreach (XmlNode node in TesterList)
                                    {
                                        XmlElement companyElement = (XmlElement)node;
                                        string _spUserId = node["spUserId"].InnerText;
                                        if (!string.IsNullOrEmpty(_spUserId))
                                        {
                                            var testerCheck = _tester.Where(t => t.spUserId == _spUserId).ToList();
                                            if (testerCheck.Count == 0)
                                            {
                                                //Add Tester
                                                List<TestPassTesterRolesNew.Role> r = new List<TestPassTesterRolesNew.Role>();
                                                if (node["roleId"] != null && node["roleName"] != null)
                                                {
                                                    r.Add(new TestPassTesterRolesNew.Role()
                                                    {
                                                        roleId = node["roleId"].InnerText,
                                                        roleName = node["roleName"].InnerText
                                                    });
                                                }
                                                if (node["testerID"] != null && node["spUserId"] != null)
                                                {
                                                    _tester.Add(new TestPassTesterRolesNew()
                                                    {
                                                        spUserId = node["spUserId"].InnerText,
                                                        testerName = node["testerName"].InnerText,
                                                        roleList = r
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testerCheck.First().roleList.Add(new TestPassTesterRolesNew.Role()
                                                {
                                                    roleId = node["roleId"].InnerText,
                                                    roleName = node["roleName"].InnerText,
                                                });
                                            }
                                        }
                                    }
                                }

                            }
                            #endregion

                            int projectId = dataReader.GetOrdinal("projectId");
                            int testPassId = dataReader.GetOrdinal("testPassId");
                            int testPassName = dataReader.GetOrdinal("testPassName");
                            int groupId = dataReader.GetOrdinal("groupId");
                            int groupName = dataReader.GetOrdinal("groupName");
                            int portfolioId = dataReader.GetOrdinal("portfolioId");
                            int portfolioName = dataReader.GetOrdinal("portfolioName");
                            int projectName = dataReader.GetOrdinal("projectName");
                            int projectVersion = dataReader.GetOrdinal("projectVersion");
                            int projectStartDate = dataReader.GetOrdinal("projectStartDate");
                            int projectEndDate = dataReader.GetOrdinal("projectEndDate");
                            int projectDesc = dataReader.GetOrdinal("projectDesc");
                            int testPassDescription = dataReader.GetOrdinal("TestPass_Description");
                            int TestManager = dataReader.GetOrdinal("TestManager");

                            string _projectId = (dataReader.IsDBNull(projectId)) == true ? "" : Convert.ToString(dataReader["projectId"]);

                            var project = groupPortfolioProjectTestPassResult.Where(prj => prj.projectId == _projectId).ToList();
                            if (project.Count == 0)
                            {
                                List<GrpPortfolioProjectTP.TestPass> _testPass = new List<GrpPortfolioProjectTP.TestPass>();
                                if (!(dataReader.IsDBNull(testPassId) || dataReader.IsDBNull(testPassName)))
                                {
                                    string _testPassId = Convert.ToString(dataReader["testPassId"]);
                                    string _testPassName = Convert.ToString(dataReader["testPassName"]);
                                    string _TestManager = Convert.ToString(dataReader["TestManager"]);
                                    string _testPassDescription = Convert.ToString(dataReader["TestPass_Description"]);


                                    if (_testPass.Where(obj => obj.testpassId == _testPassId && obj.testpassName == _testPassName).Count() == 0)
                                        _testPass.Add(new GrpPortfolioProjectTP.TestPass()
                                        {
                                            testpassId = _testPassId,
                                            testpassName = _testPassName,
                                            testPassDescription = _testPassDescription,
                                            TestManager = _TestManager,
                                            tester = _tester
                                        });
                                }

                                groupPortfolioProjectTestPassResult.Add(new GrpPortfolioProjectTP()
                                {
                                    groupId = (dataReader.IsDBNull(groupId)) == true ? "" : Convert.ToString(dataReader["groupId"]),
                                    groupName = (dataReader.IsDBNull(groupName)) == true ? "" : Convert.ToString(dataReader["groupName"]),

                                    portfolioId = (dataReader.IsDBNull(portfolioId)) == true ? "" : Convert.ToString(dataReader["portfolioId"]),
                                    portfolioName = (dataReader.IsDBNull(portfolioName)) == true ? "" : Convert.ToString(dataReader["portfolioName"]),

                                    projectId = _projectId,
                                    projectName = (dataReader.IsDBNull(projectName)) == true ? "" : Convert.ToString(dataReader["projectName"]),
                                    projectVersion = (dataReader.IsDBNull(projectVersion)) == true ? "" : Convert.ToString(dataReader["projectVersion"]),
                                    projectStartDate = (dataReader.IsDBNull(projectStartDate)) == true ? "" : Convert.ToString(dataReader["projectStartDate"]),
                                    projectEndDate = (dataReader.IsDBNull(projectEndDate)) == true ? "" : Convert.ToString(dataReader["projectEndDate"]),
                                    projectDescription = (dataReader.IsDBNull(projectDesc)) == true ? "" : Convert.ToString(dataReader["projectDesc"]),
                                    leadEmailId = _projectLeadEmail,
                                     ProjectleadName = _projectLeadName,
                                    testPassList = _testPass,
                                });
                            }
                            else
                            {
                                if (!(dataReader.IsDBNull(testPassId) || dataReader.IsDBNull(testPassName)))
                                {
                                    string _testPassId = Convert.ToString(dataReader["testPassId"]);
                                    string _testPassName = Convert.ToString(dataReader["testPassName"]);
                                    string _TestManager = Convert.ToString(dataReader["TestManager"]);
                                    string _testPassDescription = Convert.ToString(dataReader["TestPass_Description"]);

                                    if (project.First().testPassList.Where(obj => obj.testpassId == _testPassId && obj.testpassName == _testPassName).Count() == 0)
                                        project.First().testPassList.Add(new GrpPortfolioProjectTP.TestPass()
                                        {
                                            testpassId = _testPassId,
                                            testpassName = _testPassName,
                                            testPassDescription = _testPassDescription,
                                            TestManager = _TestManager,
                                            tester = _tester,
                                        });
                                }
                            }
                        }
                        return groupPortfolioProjectTestPassResult;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Get distinct Testpass and TestStep Details for Traige Page
        [HttpGet, Route("GetTriageDetails/{projectId}")]
        public List<Triage> GetTriageDetails(int projectId)
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
                List<Triage> listTriage = new List<Triage>();
                List<Triage> newLst = null;
                List<Attachment> oAttach = new List<Attachment>();

                if (string.IsNullOrEmpty(Convert.ToString(projectId)))
                {
                    return null;
                }
                else if (string.IsNullOrEmpty(Convert.ToString(_SpUserId)))
                {
                    return null;
                }
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetTriage_core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId });
                    cmd.Parameters.Add(new SqlParameter("@LoginSpUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        List<TriageTestSteps> _lstTriageTestSteps = null;
                        _lstTriageTestSteps = new List<TriageTestSteps>();

                        int tpid = dr.GetOrdinal("TestPass_ID");
                        int tsid = dr.GetOrdinal("TestStep_ID");
                        int uid = dr.GetOrdinal("User_Id");
                        int triageId = dr.GetOrdinal("Triage_Id");
                        int bug = dr.GetOrdinal("Bug");
                        int vstfBug = dr.GetOrdinal("VSTFBug_Id");
                        int triageDetails = dr.GetOrdinal("Triage_Details");
                        int dateClosed = dr.GetOrdinal("Closed_Date");
                        int owner = dr.GetOrdinal("Owner");
                        int testStepName = dr.GetOrdinal("TestStep_ActionName");
                        int expectedResult = dr.GetOrdinal("Expected_Result");
                        int priority = dr.GetOrdinal("Priority");
                        int teststepPlanId = dr.GetOrdinal("TestStepPlan_Id");
                        int severity = dr.GetOrdinal("Severity");
                        int testingDate = dr.GetOrdinal("Test_Date");
                        int Triagestatus = dr.GetOrdinal("BugStatus");
                        int vstfBugLink = dr.GetOrdinal("VSTFBug_Url");
                        int vstfBugTitle = dr.GetOrdinal("VSTFBug_Title");
                        int roleId = dr.GetOrdinal("Role_Id");
                        int roleName = dr.GetOrdinal("Role_Name");
                        int actualResult = dr.GetOrdinal("Actual_Result");
                        int TestStepStatus = dr.GetOrdinal("tspStatus");
                        int AttachmentName = dr.GetOrdinal("AttachmentName");
                        int testCaseId = dr.GetOrdinal("TestCase_ID");
                        int testCaseName = dr.GetOrdinal("TestCase_Name");
                        int testCaseDesc = dr.GetOrdinal("TestCase_Description");

                        while (dr.Read())
                        {
                            string tspStatus = (dr.IsDBNull(TestStepStatus)) == true ? "" : Convert.ToString(dr["tspStatus"]).Trim();
                            if (tspStatus == "3" || string.IsNullOrEmpty(tspStatus))
                            {
                                oAttach = new List<Attachment>();
                                if ((dr.IsDBNull(AttachmentName)) != true && !string.IsNullOrEmpty(Convert.ToString(dr["AttachmentName"]).Trim()))
                                {
                                    string TempAttachment = dr["AttachmentName"].ToString();
                                    string[] resultAttachment = TempAttachment.Split('#');
                                    string AttachmentDetails = "";
                                    foreach (string att in resultAttachment)
                                    {
                                        if (att.Trim() != "")
                                            AttachmentDetails = att;
                                        if (AttachmentDetails != "")
                                        {
                                            string[] AttachmentArr = AttachmentDetails.Split('~');

                                                string fileName = AttachmentArr[0];
                                                string AttachmentId = AttachmentArr[1];
                                                string filePath = AttachmentArr[2];
                                               

                                                oAttach.Add(new Attachment()
                                                {
                                                    fileName = Convert.ToString(fileName).Trim(),
                                                    filePath = Convert.ToString(filePath).Trim(),
                                                    AttachmentId = Convert.ToString(AttachmentId).Trim()
                                                    
                                                });
                                        }
                                    }
                                    
                                }
                               
                                _lstTriageTestSteps.Add(new TriageTestSteps
                                {
                                    tpId = (dr.IsDBNull(tpid)) == true ? "" : Convert.ToString(dr["TestPass_ID"]).Trim(),
                                    uId = (dr.IsDBNull(uid)) == true ? "" : Convert.ToString(dr["User_Id"]).Trim(),
                                    roleId = (dr.IsDBNull(roleId)) == true ? "" : Convert.ToString(dr["Role_Id"]).Trim(),
                                    roleName = (dr.IsDBNull(roleName)) == true ? "" : Convert.ToString(dr["Role_Name"]).Trim(),
                                    actualResult = (dr.IsDBNull(actualResult)) == true ? "" : Convert.ToString(dr["Actual_Result"]).Trim(),
                                    teststepPlanId = (dr.IsDBNull(teststepPlanId)) == true ? "" : Convert.ToString(dr["TestStepPlan_Id"]).Trim(),
                                    triageId = (dr.IsDBNull(triageId)) == true ? "" : Convert.ToString(dr["Triage_Id"]).Trim(),
                                    bug = (dr.IsDBNull(bug)) == true ? "" : Convert.ToString(dr["Bug"]).Trim(),
                                    expectedResult = (dr.IsDBNull(expectedResult)) == true ? "" : Convert.ToString(dr["Expected_Result"]),
                                    vstfBug = (dr.IsDBNull(vstfBug)) == true ? "" : Convert.ToString(dr["VSTFBug_Id"]).Trim(),
                                    triageDetails = (dr.IsDBNull(triageDetails)) == true ? "" : Convert.ToString(dr["Triage_Details"]).Trim(),
                                    dateClosed = (dr.IsDBNull(dateClosed)) == true ? "" : Convert.ToDateTime(Convert.ToString(dr["Closed_Date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                    lstAttachment = oAttach,
                                    owner = (dr.IsDBNull(owner)) == true ? "" : Convert.ToString(dr["Owner"]).Trim(),
                                    priority = (dr.IsDBNull(priority)) == true ? "" : Convert.ToString(dr["Priority"]).Trim(),
                                    severity = (dr.IsDBNull(severity)) == true ? "" : Convert.ToString(dr["Severity"]).Trim(),
                                    testingDate = (dr.IsDBNull(testingDate)) == true ? "" : Convert.ToDateTime(Convert.ToString(dr["Test_Date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                    Triagestatus = (dr.IsDBNull(Triagestatus)) == true ? "" : Convert.ToString(dr["BugStatus"]).Trim(),
                                    vstfBugLink = (dr.IsDBNull(vstfBugLink)) == true ? "" : Convert.ToString(dr["VSTFBug_Url"]).Trim(),
                                    vstfBugTitle = (dr.IsDBNull(vstfBugTitle)) == true ? "" : Convert.ToString(dr["VSTFBug_Title"]).Trim(),
                                    testStepId = (dr.IsDBNull(tsid)) == true ? "" : Convert.ToString(dr["TestStep_Id"]).Trim(),
                                    testStepName = (dr.IsDBNull(testStepName)) == true ? "" : Convert.ToString(dr["TestStep_ActionName"]).Trim(),
                                });
                            }

                            int testPassName = dr.GetOrdinal("TestPass_Name");
                            int description = dr.GetOrdinal("TestPass_Description");
                            int TestPassEndDate = dr.GetOrdinal("TestPassEndDate");
                            int testMgrName = dr.GetOrdinal("TestManager");
                            int testerName = dr.GetOrdinal("Tester");
                            int testMgrId = dr.GetOrdinal("TestMgr_ID");

                            listTriage.Add(new Triage
                            {
                                testPassId = (dr.IsDBNull(tpid)) == true ? "" : Convert.ToString(dr["TestPass_ID"]).Trim(),
                                testPassName = (dr.IsDBNull(testPassName)) == true ? "" : Convert.ToString(dr["TestPass_Name"]).Trim(),
                                description = (dr.IsDBNull(description)) == true ? "" : Convert.ToString(dr["TestPass_Description"]).Trim(),
                                testMgrId = (dr.IsDBNull(testMgrId)) == true ? "" : Convert.ToString(dr["TestMgr_ID"]).Trim(),
                                testMgrName = (dr.IsDBNull(testMgrName)) == true ? "" : Convert.ToString(dr["TestManager"]).Trim(),
                                endDate = (dr.IsDBNull(TestPassEndDate)) == true ? "" : Convert.ToString(dr["TestPassEndDate"]).Trim(),
                                failedTestSteps = "",
                                testerSpuserId = (dr.IsDBNull(uid)) == true ? "" : Convert.ToString(dr["User_Id"]).Trim(),
                                testerName = (dr.IsDBNull(testerName)) == true ? "" : Convert.ToString(dr["Tester"]).Trim(),
                                testCaseId = (dr.IsDBNull(testCaseId)) == true ? "" : Convert.ToString(dr["TestCase_ID"]).Trim(),
                                testCaseName = (dr.IsDBNull(testCaseName)) == true ? "" : Convert.ToString(dr["TestCase_Name"]).Trim(),
                                testCaseDesc = (dr.IsDBNull(testCaseDesc)) == true ? "" : Convert.ToString(dr["TestCase_Description"]).Trim(),
                                listTriageTestSteps = new List<TriageTestSteps>()
                            });
                        }

                        //var groupTriage = listTriage.GroupBy(z => z.testPassId).ToList();
                        newLst = new List<Triage>();
                        if (listTriage != null && listTriage.Count > 0)
                        {
                            /*sorting array*/
                            listTriage = listTriage.OrderBy(y => y.testPassId).OrderBy(y => y.testerSpuserId).ToList();

                            /*getting distinct testpass id*/
                            string ID = listTriage[0].testPassId;
                            string UID = listTriage[0].testerSpuserId;

                            for (int i = 0; i < listTriage.Count; i++)
                            {
                                if (i == 0)
                                {
                                    newLst.Add(listTriage[i]);
                                }
                                else
                                {
                                    if (ID != listTriage[i].testPassId || UID != listTriage[i].testerSpuserId)
                                    {
                                        newLst.Add(listTriage[i]);
                                        ID = listTriage[i].testPassId;
                                        UID = listTriage[i].testerSpuserId;
                                    }
                                }
                            }
                            /*getting distinct testpass id end*/

                            /*sorting*/
                            _lstTriageTestSteps = _lstTriageTestSteps.OrderBy(z => z.tpId).OrderBy(z => z.uId).ToList();

                            /*adding teststep array to list*/
                            for (int i = 0; i < newLst.Count; i++)
                            {
                                for (int j = 0; j < _lstTriageTestSteps.Count; j++)
                                {
                                    if (_lstTriageTestSteps[j].tpId == newLst[i].testPassId && _lstTriageTestSteps[j].uId == newLst[i].testerSpuserId)
                                    {
                                        newLst[i].listTriageTestSteps.Add(new TriageTestSteps
                                        {
                                            tpId = _lstTriageTestSteps[j].tpId,
                                            uId = _lstTriageTestSteps[j].uId,
                                            testStepId = _lstTriageTestSteps[j].testStepId,
                                            testStepName = _lstTriageTestSteps[j].testStepName,
                                            roleId = _lstTriageTestSteps[j].roleId,
                                            roleName = _lstTriageTestSteps[j].roleName,
                                            actualResult = _lstTriageTestSteps[j].actualResult,
                                            teststepPlanId = _lstTriageTestSteps[j].teststepPlanId,
                                            expectedResult = _lstTriageTestSteps[j].expectedResult,

                                            triageId = _lstTriageTestSteps[j].triageId,
                                            bug = _lstTriageTestSteps[j].bug,
                                            vstfBug = _lstTriageTestSteps[j].vstfBug,
                                            triageDetails = _lstTriageTestSteps[j].triageDetails,

                                            dateClosed = _lstTriageTestSteps[j].dateClosed,
                                            lstAttachment = _lstTriageTestSteps[j].lstAttachment,
                                            owner = _lstTriageTestSteps[j].owner,
                                            priority = _lstTriageTestSteps[j].priority,
                                            severity = _lstTriageTestSteps[j].severity,

                                            testingDate = _lstTriageTestSteps[j].testingDate,
                                            Triagestatus = _lstTriageTestSteps[j].Triagestatus,
                                            vstfBugLink = _lstTriageTestSteps[j].vstfBugLink,
                                            vstfBugTitle = _lstTriageTestSteps[j].vstfBugTitle
                                        });
                                    }
                                }
                            }
                        }
                        newLst = newLst.OrderBy(z => Convert.ToInt32(z.testPassId)).ToList();
                    }
                    return newLst;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Insert Triage Details in Triage Table 
        [HttpPost, Route("InsertUpdateTriage")]
        public Dictionary<string, string> InsertUpdateTriage([FromBody]TriageTestSteps oTriage)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(oTriage.teststepPlanId))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "teststepPlanId is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oTriage.bug))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "bug is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(oTriage.vstfBugTitle))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "vstfBugTitle is required");
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
                if (string.IsNullOrEmpty(oTriage.triageId))
                    statementType = "Insert";
                else
                    statementType = "Update";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spTriageInsUpd";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Triage_Id", SqlDbType.Int) { Value = oTriage.triageId });
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlan_Id", SqlDbType.Int) { Value = oTriage.teststepPlanId });
                    cmd.Parameters.Add(new SqlParameter("@Bug", SqlDbType.VarChar) { Value = oTriage.bug });
                    cmd.Parameters.Add(new SqlParameter("@Triage_Details", SqlDbType.VarChar, 500) { Value = oTriage.triageDetails });
                    cmd.Parameters.Add(new SqlParameter("@Owner", SqlDbType.VarChar, 500) { Value = oTriage.owner });
                    cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 500) { Value = oTriage.Triagestatus });
                    cmd.Parameters.Add(new SqlParameter("@Closed_Date", SqlDbType.VarChar, 500) { Value = (oTriage.dateClosed == "") ? null : oTriage.dateClosed });

                    cmd.Parameters.Add(new SqlParameter("@VSTFBug_Id", SqlDbType.VarChar, 500) { Value = oTriage.vstfBug });
                    cmd.Parameters.Add(new SqlParameter("@Priority", SqlDbType.VarChar, 500) { Value = oTriage.priority });
                    cmd.Parameters.Add(new SqlParameter("@Severity", SqlDbType.VarChar, 500) { Value = oTriage.severity });
                    cmd.Parameters.Add(new SqlParameter("@VSTFBug_Link", SqlDbType.VarChar, 500) { Value = oTriage.vstfBugLink });
                    cmd.Parameters.Add(new SqlParameter("@VSTFBug_Title", SqlDbType.VarChar, 500) { Value = oTriage.vstfBugTitle });

                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 500) { Value = statementType });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result == -1)
                    {
                        if (!string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add("Value", _retValue);
                            _result.Add(this._statusText, "Done");
                        }
                        else
                            _result.Add(this._statusText, "Please Insert Again");
                    }
                }
            }
            catch (Exception ex)
            {
                _result.Add(this._errorText, ex.Message);
            }
            return _result;
        }

        [HttpGet, Route("GetFileToDownloadExpResult")]
        public FileContentResult GetFileToDownloadExpResult(int id, string Url)
        {
            byte[] fileContent = new byte[0];
            try
            {
                string SchemaName = "";
                string AppUrl = Url;
                string matches;
                string expResultimg;

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }

                var filename = "";
                var contentType = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetTriageToExcel";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlan_Id", SqlDbType.Int, 15) { Value = id });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string baseStr = dataReader["Expected_Result"].ToString();
                            matches = baseStr.Split(',')[1];
                            expResultimg = matches.Split('"')[0];
                            byte[] imageBytes = Convert.FromBase64String(expResultimg);


                            filename = "abc.png";
                            contentType = "image/png";
                            fileContent = imageBytes;

                            Response.ContentType = contentType;
                            Response.Headers.Add("content-disposition", "inline;filename=" + filename);
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

        [HttpGet, Route("GetFileToDownloadActResult")]
        public FileContentResult GetFileToDownloadActResult(int id, string Url)
        {
            byte[] fileContent = new byte[0];
            try
            {
                string SchemaName = "";
                string AppUrl = Url;
                string matches;
                string expResultimg;

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }

                var filename = "";
                var contentType = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetActResTriageToExcelActRes";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@TestStepPlan_Id", SqlDbType.Int, 15) { Value = id });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string baseStr = dataReader["Actual_Result"].ToString();
                            matches = baseStr.Split(',')[1];
                            expResultimg = matches.Split('"')[0];
                            byte[] imageBytes = Convert.FromBase64String(expResultimg);


                            filename = "abc.png";
                            contentType = "image/png";
                            fileContent = imageBytes;

                            Response.ContentType = contentType;
                            Response.Headers.Add("content-disposition", "inline;filename=" + filename);
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
    }
}

