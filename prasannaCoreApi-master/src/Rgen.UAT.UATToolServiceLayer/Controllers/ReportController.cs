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
    public class ReportController : Controller
    {
        private clsDbContext _context;
        private string _returnParameter = "@Ret_Parameter";

        public ReportController(clsDbContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return "value";
        }

        //GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // Get DropDown data for Report Page
        [HttpGet, Route("GetTestPassReportPage")]
        public List<GrpPortfolioProjectTP> GetTestPassReportPage()
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
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetGroupPortfolioProjectTestPassReportPage";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    List<GrpPortfolioProjectTP> groupPortfolioProjectTestPassResult = new List<GrpPortfolioProjectTP>();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;

                            List<TestPassTesterRolesNew> _tester = new List<TestPassTesterRolesNew>();
                            string _projectLeadEmail = string.Empty;
                            string _projectLeadName = string.Empty;
                            
                            #region Project Lead Conversion
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
                            int testPassDescription = dataReader.GetOrdinal("testpassdescription");
                            int groupId = dataReader.GetOrdinal("groupId");
                            int groupName = dataReader.GetOrdinal("groupName");
                            int portfolioId = dataReader.GetOrdinal("portfolioId");
                            int portfolioName = dataReader.GetOrdinal("portfolioName");
                            int projectName = dataReader.GetOrdinal("projectName");
                            int projectVersion = dataReader.GetOrdinal("projectVersion");
                            int projectStartDate = dataReader.GetOrdinal("projectStartDate");
                            int projectEndDate = dataReader.GetOrdinal("projectEndDate");
                            int projectDesc = dataReader.GetOrdinal("projectDesc");

                            string _projectId = (dataReader.IsDBNull(projectId)) == true ? "" : Convert.ToString(dataReader["projectId"]);

                            var project = groupPortfolioProjectTestPassResult.Where(prj => prj.projectId == _projectId).ToList();
                            if (project.Count == 0)
                            {
                                List<GrpPortfolioProjectTP.TestPass> _testPass = new List<GrpPortfolioProjectTP.TestPass>();
                                if (!(dataReader.IsDBNull(testPassId) || dataReader.IsDBNull(testPassName)))
                                {
                                    string _testPassId = Convert.ToString(dataReader["testPassId"]);
                                    string _testPassName = Convert.ToString(dataReader["testPassName"]);
                                    string _testPassDescription = Convert.ToString(dataReader["testpassdescription"]);
                                    string _TestManager = Convert.ToString(dataReader["TestManager"]);

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
                                    string _testPassDescription = Convert.ToString(dataReader["testpassdescription"]);
                                    string _TestManager = Convert.ToString(dataReader["TestManager"]);

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

        //Get distinct Testpass and TestStep Details for Report Page
        [HttpGet, Route("GetReportData/{projectId}")]
        public List<Reports> GetReportData(int projectId)
        {
            try
            {
                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
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

                List<Reports> listReport = new List<Reports>();
                List<ProjectUser> listMgr = new List<ProjectUser>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetReport";
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
                        Reports Report = null;
                        List<ReportUniqueObject> lstUnique = new List<ReportUniqueObject>();
                        List<ReportTesterRoleStatus> listRptTesterRoleStatus = null;

                        int tpid = dr.GetOrdinal("TestPass_ID");
                        int tsid = dr.GetOrdinal("TestStep_ID");
                        int uid = dr.GetOrdinal("User_Id");
                        int testPassName = dr.GetOrdinal("TestPass_Name");
                        int description = dr.GetOrdinal("TestPass_Description");
                        int TestPassEndDate = dr.GetOrdinal("TestPassEndDate");
                        int testMgrName = dr.GetOrdinal("TestManager");
                        int testCaseId = dr.GetOrdinal("TestCase_ID");
                        int testCaseName = dr.GetOrdinal("TestCase_Name");
                        int testCaseDesc = dr.GetOrdinal("TestCase_Description");
                        int testStepName = dr.GetOrdinal("TestStep_ActionName");
                        int expectedResult = dr.GetOrdinal("Expected_Result");
                        int testerName = dr.GetOrdinal("Tester");
                        int teststepPlanId = dr.GetOrdinal("TestStepPlan_Id");
                        int roleId = dr.GetOrdinal("Role_Id");
                        int roleName = dr.GetOrdinal("Role_Name");
                        int actualResult = dr.GetOrdinal("Actual_Result");
                        int status = dr.GetOrdinal("TestStepStatus");

                        while (dr.Read())
                        {
                            Report = new Reports();
                            lstUnique.Add(new ReportUniqueObject
                            {
                                TPID = tpid,
                                TSID = tsid,
                                UID = uid
                            });

                            Report.testPassId = (dr.IsDBNull(tpid)) == true ? "" : Convert.ToString(dr["TestPass_ID"]).Trim();
                            Report.testPassName = (dr.IsDBNull(testPassName)) == true ? "" : Convert.ToString(dr["TestPass_Name"]).Trim();
                            Report.description = (dr.IsDBNull(description)) == true ? "" : Convert.ToString(dr["TestPass_Description"]).Trim();
                            Report.dueDate = (dr.IsDBNull(TestPassEndDate)) == true ? "" : Convert.ToString(dr["TestPassEndDate"]).Trim();
                            Report.testMgrName = (dr.IsDBNull(testMgrName)) == true ? "" : Convert.ToString(dr["TestManager"]).Trim();
                            Report.testCaseId = (dr.IsDBNull(testCaseId)) == true ? "" : Convert.ToString(dr["TestCase_ID"]).Trim();
                            Report.testCaseName = (dr.IsDBNull(testCaseName)) == true ? "" : Convert.ToString(dr["TestCase_Name"]).Trim();
                            Report.testCaseDesc = (dr.IsDBNull(testCaseDesc)) == true ? "" : Convert.ToString(dr["TestCase_Description"]).Trim();
                            Report.testStepId = (dr.IsDBNull(tsid)) == true ? "" : Convert.ToString(dr["TestStep_ID"]).Trim();
                            Report.testStepName = (dr.IsDBNull(testStepName)) == true ? "" : Convert.ToString(dr["TestStep_ActionName"]).Trim();
                            Report.expectedResult = (dr.IsDBNull(expectedResult)) == true ? "" : Convert.ToString(dr["Expected_Result"]).Trim();
                            Report.testerName = (dr.IsDBNull(testerName)) == true ? "" : Convert.ToString(dr["Tester"]).Trim();
                            Report.testerId = (dr.IsDBNull(uid)) == true ? "" : Convert.ToString(dr["User_Id"]).Trim();

                            listRptTesterRoleStatus = new List<ReportTesterRoleStatus>();

                            listRptTesterRoleStatus.Add(new ReportTesterRoleStatus()
                            {

                                teststepPlanId = (dr.IsDBNull(teststepPlanId) == true ? "" : Convert.ToString(dr["TestStepPlan_Id"])),
                                roleId = (dr.IsDBNull(roleId) == true ? "" : Convert.ToString(dr["Role_Id"])),
                                roleName = (dr.IsDBNull(roleName) == true ? "" : Convert.ToString(dr["Role_Name"])),
                                actualResult = (dr.IsDBNull(actualResult) == true ? "" : Convert.ToString(dr["Actual_Result"])),
                                status = (dr.IsDBNull(status) == true ? "" : Convert.ToString(dr["TestStepStatus"])),
                            });

                            Report.listRptTesterRoleStatus = listRptTesterRoleStatus;
                            listReport.Add(Report);
                        }
                    }
                    return listReport;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
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
                    cmd.CommandText = "UAT.spGetReportToExcel";
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
                    cmd.CommandText = "UAT.spGetActResReportToExcelActRes";
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
