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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class StakeholderDashboardController : Controller
    {
        private clsDbContext _context;
        public StakeholderDashboardController(clsDbContext context)
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

        // Get Project Details for Stakeholder Dashboard Page
        [HttpGet, Route("GetStakeholderProjectDetails")]
        public JsonResult GetStakeholderProjectDetails()
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
                    return Json("Invalid Url");
                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spStakeholderDashboardReportProjectDetails";
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
                            for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                            {
                                dataRow.Add(dataReader.GetName(iFiled), dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled]);
                            }
                            retObject.Add((ExpandoObject)dataRow);
                        }
                    }
                    return Json(retObject);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        // Get TestStep Details for Stakeholder Dashboard Page
        [HttpGet, Route("GetStakeholderTestStep")]
        public List<StakeholderDbTestStep> GetStakeholderTestStep()
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
                    cmd.CommandText = "UAT.spStakeholderDashboardTestStepDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    List<StakeholderDbTestStep> stakeholderDbTestStepResult = new List<StakeholderDbTestStep>();

                    var retObject = new List<dynamic>();
                    List<StkDBUniqueObject> lstStkDbUnique = new List<StkDBUniqueObject>();
                    StakeholderDbTestStep objTS = null;
                    List<StakeholderDbTSDetails> lstDetails = null;
                    List<clsTestSD> lst = new List<clsTestSD>();


                    using (var dr = cmd.ExecuteReader())
                    {
                        int count = 0;
                        int dr_Row = 0;

                        while (dr.Read())
                        {
                            lst.Add(new clsTestSD()
                            {
                                projectId = Convert.ToString(dr["projectId"]),
                                testPassId = Convert.ToString(dr["testPassId"]),
                                testPassName = Convert.ToString(dr["testPassName"]),
                                created = Convert.ToString(dr["created"]),
                                modified = Convert.ToString(dr["modified"]),
                                testStepName = Convert.ToString(dr["testStepName"]),
                                testStepId = Convert.ToString(dr["testStepId"]),
                                projectName = Convert.ToString(dr["projectName"]),
                                roleName = Convert.ToString(dr["roleName"]),
                                testerName = Convert.ToString(dr["testerName"]),
                                testCaseName = Convert.ToString(dr["testCaseName"]),
                                testCaseId = Convert.ToString(dr["testCaseId"]),
                                roleId = Convert.ToString(dr["roleId"]),
                                testerId = Convert.ToString(dr["testerId"]),
                                expectedResult = Convert.ToString(dr["expectedResult"]),
                                actualResult = Convert.ToString(dr["actualResult"]),
                                status = Convert.ToString(dr["status"]),
                            });
                        }

                        if (lst.Count > 0)
                        {
                            dr_Row = lst.Count;
                            for (int i = 0; i < dr_Row; i++)
                            {
                                count++;
                                
                                int _projectId = Convert.ToInt32(lst[i].projectId);
                                int _testPassId = Convert.ToInt32(lst[i].testPassId);
                                int _testCaseId = Convert.ToInt32(lst[i].testCaseId);
                                int _testerId = Convert.ToInt32(lst[i].testerId);
                                int _roleId = Convert.ToInt32(lst[i].roleId);

                                if (lstStkDbUnique.Any(z => z.ProjectID == _projectId && z.TPID == _testPassId && z.TCID == _testCaseId && z.TesterID == _testerId && z.RoleID == _roleId) == false)
                                {
                                    lstStkDbUnique.Add(new StkDBUniqueObject()
                                    {
                                        ProjectID = _projectId,
                                        TPID = _testPassId,
                                        TCID = _testCaseId,
                                        TesterID = _testerId,
                                        RoleID = _roleId
                                    });
                                    objTS = new StakeholderDbTestStep();
                                    objTS.projectId = lst[i].projectId;
                                    objTS.projectName = lst[i].projectName;
                                    objTS.testpassId = lst[i].testPassId;
                                    objTS.testpassName = lst[i].testPassName;
                                    objTS.testcaseId = lst[i].testCaseId;
                                    objTS.testcaseName = lst[i].testCaseName;
                                    objTS.testerId = lst[i].testerId;
                                    objTS.testerName = lst[i].testerName;
                                    objTS.roleId = lst[i].roleId;
                                    objTS.roleName = lst[i].roleName;
                                    objTS.lstStakeholderDbTSDetails = null;
                                    lstDetails = new List<StakeholderDbTSDetails>();

                                    lstDetails.Add(new StakeholderDbTSDetails()
                                    {
                                        teststepId = lst[i].testStepId,
                                        teststepName = lst[i].testStepName,
                                        Modified = lst[i].modified,
                                        Created = lst[i].created,
                                        expectedResult = lst[i].expectedResult,
                                        actualResult = lst[i].actualResult,
                                        status = lst[i].status
                                    });

                                    if (dr_Row == count)
                                    {
                                        objTS.lstStakeholderDbTSDetails = lstDetails;
                                        stakeholderDbTestStepResult.Add(objTS);
                                    }
                                   else if (lst[count].projectId != _projectId.ToString() || lst[count].testPassId != _testPassId.ToString() || lst[count].testCaseId != _testCaseId.ToString() || lst[count].testerId != _testerId.ToString() || lst[count].roleId != _roleId.ToString())
                                    {
                                        objTS.lstStakeholderDbTSDetails = lstDetails;
                                        stakeholderDbTestStepResult.Add(objTS);
                                    }
                                }

                                else
                                {
                                    lstDetails.Add(new StakeholderDbTSDetails()
                                    {
                                        teststepId = lst[i].testStepId,
                                        teststepName = lst[i].testStepName,
                                        Modified = lst[i].modified,
                                        Created = lst[i].created,
                                        expectedResult = lst[i].expectedResult,
                                        actualResult = lst[i].actualResult,
                                        status = lst[i].status
                                    });

                                    if (dr_Row == count)
                                    {
                                        objTS.lstStakeholderDbTSDetails = lstDetails;
                                        stakeholderDbTestStepResult.Add(objTS);
                                    }
                                    else if ((Convert.ToString(lst[count].projectId) != _projectId.ToString() || Convert.ToString(lst[count].testPassId) != _testPassId.ToString() || Convert.ToString(lst[count].testCaseId) != _testCaseId.ToString() || Convert.ToString(lst[count].testerId) != _testerId.ToString() || Convert.ToString(lst[count].roleId) != _roleId.ToString()))
                                    {
                                        objTS.lstStakeholderDbTSDetails = lstDetails;
                                        stakeholderDbTestStepResult.Add(objTS);
                                    }

                                }


                            }
                        }



                        return stakeholderDbTestStepResult;
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




