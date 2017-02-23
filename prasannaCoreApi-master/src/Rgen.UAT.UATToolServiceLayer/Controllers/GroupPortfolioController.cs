#region Copyright ©2016, RGen Software Solutions (I) Pvt. Ltd. - All Rights Reserved
/* --------------------------------------------------------------------------------- *
*                      RGen Software Solutions (I) Pvt. Ltd.                         *
*                      Copyright ©2016 - All Rights reserved                         *
*                                                                                    *
*                                                                                    *
*        Copyright © 2016 by RGen Software Solutions | www.rgensoft.com/             *
*        All rights reserved. No part of this publication may be reproduced,         *
*        stored in a retrieval system or transmitted, in any form or by any          *
*        means, photocopying, recording or otherwise, without prior written          *
*        consent of  RGen Software Solutions (I) Pvt. Ltd.                           *
*                                                                                    *
*                                                                                    *
* ---------------------------------------------------------------------------------  */
#endregion Copyright ©2016, RGen Software Solutions (I) Pvt. Ltd. - All Rights Reserved

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
using System.IO;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class GroupPortfolioController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";


        private clsDbContext _context;
        public GroupPortfolioController(clsDbContext context)
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

        [HttpGet, Route("GetDropdownDataTestManagement/{spUserID}/{tabName}")]
        public JsonResult GetDropdownDataTestManagement(string spUserID, string tabName)
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

            if (string.IsNullOrEmpty(spUserID) || string.IsNullOrEmpty(tabName))
            {
                //ExceptionHelper.TraceServiceLevelException("spUserId and tabName is required!");
                return null;
            }

            tabName = tabName.ToLower();

            List<GroupPortfolioProjectTestPass> groupPortfolioProjectTestPassResult = new List<GroupPortfolioProjectTestPass>();


            try
            {

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetGroupPortfolioProjectTestPassTestManagement";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TabName", SqlDbType.VarChar) { Value = tabName });
                    cmd.Parameters.Add(new SqlParameter("@SPUserId", SqlDbType.Int) { Value = spUserID });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 4000) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        int ProjectLeadOrdinal = dr.GetOrdinal("projectLead");
                        int projectRoleCollectionOrdinal = -1;
                        try { projectRoleCollectionOrdinal = dr.GetOrdinal("projectRoleCollection"); } catch (Exception e1) { }
                        int projectStakeholderCollectionOrdinal = -1;
                        try { projectStakeholderCollectionOrdinal = dr.GetOrdinal("projectStakeholderCollection"); } catch (Exception e1) { }
                        int testCaseCollectionOrdinal = -1;
                        try { testCaseCollectionOrdinal = dr.GetOrdinal("testCaseCollection"); } catch (Exception e1) { }
                        int projectIdOrdinal = dr.GetOrdinal("projectId");
                        int testPassIdOrdinal = -1;
                        try { testPassIdOrdinal = dr.GetOrdinal("testPassId"); } catch (Exception e1) { }
                        int testPassNameOrdinal = -1;
                        try { testPassNameOrdinal = dr.GetOrdinal("testPassName"); } catch (Exception e1) { }
                        int testingTypeOrdinal = -1;
                        try { testingTypeOrdinal = dr.GetOrdinal("testingType"); } catch (Exception e1) { }
                        int groupIdOrdinal = dr.GetOrdinal("groupId");
                        int groupNameOrdinal = dr.GetOrdinal("groupName");
                        int portfolioIdOrdinal = dr.GetOrdinal("portfolioId");
                        int portfolioNameOrdinal = dr.GetOrdinal("portfolioName");
                        int projectNameOrdinal = dr.GetOrdinal("projectName");
                        int projectVersionOrdinal = dr.GetOrdinal("projectVersion");
                        int projectStartDateOrdinal = dr.GetOrdinal("projectStartDate");
                        int projectEndDateOrdinal = dr.GetOrdinal("projectEndDate");


                        while (dr.Read())
                        {
                            #region Project Lead Conversion

                            string _projectLeadEmail = (dr.IsDBNull(ProjectLeadOrdinal)) == true ? "" : Convert.ToString(dr[ProjectLeadOrdinal]);


                            #endregion

                            #region Role Conversion

                            string _role = null;

                            if (tabName == "tester")
                            {
                                if (projectRoleCollectionOrdinal >= 0)
                                    _role = (dr.IsDBNull(projectRoleCollectionOrdinal)) == true ? "" : Convert.ToString(dr[projectRoleCollectionOrdinal]);


                            }
                            else if (tabName == "teststep")
                            {
                                if (projectRoleCollectionOrdinal >= 0)
                                    _role = (dr.IsDBNull(projectRoleCollectionOrdinal)) == true ? "" : Convert.ToString(dr[projectRoleCollectionOrdinal]);

                            }

                            #endregion

                            #region Stakeholder Conversion

                            string _stakeholder = null;

                            if (tabName == "testpass")
                            {

                                if (projectStakeholderCollectionOrdinal >= 0)
                                    _stakeholder = (dr.IsDBNull(projectStakeholderCollectionOrdinal)) == true ? "" : Convert.ToString(dr[projectStakeholderCollectionOrdinal]);


                            }

                            #endregion

                            #region Test Case Conversion

                            string _testCase = null;

                            if (tabName == "teststep")
                            {
                                if (testCaseCollectionOrdinal >= 0)
                                    _testCase = (dr.IsDBNull(testCaseCollectionOrdinal)) == true ? "" : Convert.ToString(dr[testCaseCollectionOrdinal]);
                            }

                            #endregion

                            string _projectId = (dr.IsDBNull(projectIdOrdinal)) == true ? "" : Convert.ToString(dr[projectIdOrdinal]);

                            var project = groupPortfolioProjectTestPassResult.Where(prj => prj.projectId == _projectId).ToList();

                            if (project.Count == 0)
                            {
                                #region Test Pass Processing

                                List<GroupPortfolioProjectTestPass.TestPass> _testPass = null;
                                if (tabName == "teststep")
                                {
                                    _testPass = new List<GroupPortfolioProjectTestPass.TestPass>();
                                    List<GroupPortfolioProjectTestPass.Role> _lstRole = new List<GroupPortfolioProjectTestPass.Role>();



                                    if (!(dr.IsDBNull(testPassIdOrdinal) || dr.IsDBNull(testPassNameOrdinal)))
                                        _testPass.Add(new GroupPortfolioProjectTestPass.TestPass()
                                        {


                                            testpassId = (dr.IsDBNull(testPassIdOrdinal)) == true ? "" : Convert.ToString(dr[testPassIdOrdinal]),
                                            testpassName = (dr.IsDBNull(testPassNameOrdinal)) == true ? "" : Convert.ToString(dr[testPassNameOrdinal]),
                                            testingType = (dr.IsDBNull(testingTypeOrdinal)) == true ? "" : Convert.ToString(dr[testingTypeOrdinal]),
                                            testCaseList = _testCase,
                                            ListRoles = _role
                                        });
                                }
                                else if (tabName != "testpass")
                                {
                                    _testPass = new List<GroupPortfolioProjectTestPass.TestPass>();

                                    if (!(dr.IsDBNull(testPassIdOrdinal) || dr.IsDBNull(testPassNameOrdinal)))
                                        _testPass.Add(new GroupPortfolioProjectTestPass.TestPass()
                                        {
                                            testpassId = (dr.IsDBNull(testPassIdOrdinal)) == true ? "" : Convert.ToString(dr[testPassIdOrdinal]),
                                            testpassName = (dr.IsDBNull(testPassNameOrdinal)) == true ? "" : Convert.ToString(dr[testPassNameOrdinal]),

                                            testCaseList = _testCase,
                                        });
                                }

                                #endregion

                                groupPortfolioProjectTestPassResult.Add(new GroupPortfolioProjectTestPass()
                                {
                                    groupId = (dr.IsDBNull(groupIdOrdinal)) == true ? "" : Convert.ToString(dr[groupIdOrdinal]),
                                    groupName = (dr.IsDBNull(groupNameOrdinal)) == true ? "" : Convert.ToString(dr[groupNameOrdinal]),

                                    portfolioId = (dr.IsDBNull(portfolioIdOrdinal)) == true ? "" : Convert.ToString(dr[portfolioIdOrdinal]),
                                    portfolioName = (dr.IsDBNull(portfolioNameOrdinal)) == true ? "" : Convert.ToString(dr[portfolioNameOrdinal]),

                                    projectId = _projectId,
                                    projectName = (dr.IsDBNull(projectNameOrdinal)) == true ? "" : Convert.ToString(dr[projectNameOrdinal]),
                                    projectVersion = (dr.IsDBNull(projectVersionOrdinal)) == true ? "" : Convert.ToString(dr[projectVersionOrdinal]),
                                    projectStartDate = (dr.IsDBNull(projectStartDateOrdinal)) == true ? "" : Convert.ToString(dr[projectStartDateOrdinal]),
                                    projectEndDate = (dr.IsDBNull(projectEndDateOrdinal)) == true ? "" : Convert.ToString(dr[projectEndDateOrdinal]),

                                    leadEmailId = _projectLeadEmail,

                                    roleList = _role,
                                    stakeholderList = _stakeholder,
                                    testPassList = _testPass,
                                });
                            }
                            else
                            {
                                #region Test Pass Processing

                                if (tabName == "teststep")
                                {
                                    if (!(dr.IsDBNull(testPassIdOrdinal) || dr.IsDBNull(testPassNameOrdinal)))
                                        project.First().testPassList.Add(new GroupPortfolioProjectTestPass.TestPass()
                                        {
                                            testpassId = (dr.IsDBNull(testPassIdOrdinal)) == true ? "" : Convert.ToString(dr[testPassIdOrdinal]),
                                            testpassName = (dr.IsDBNull(testPassNameOrdinal)) == true ? "" : Convert.ToString(dr[testPassNameOrdinal]),
                                            testingType = (dr.IsDBNull(testingTypeOrdinal)) == true ? "" : Convert.ToString(dr[testingTypeOrdinal]),
                                            testCaseList = _testCase,
                                            ListRoles = _role
                                        });
                                }
                                else if (tabName != "testpass")
                                {
                                    if (!(dr.IsDBNull(testPassIdOrdinal) || dr.IsDBNull(testPassNameOrdinal)))
                                        project.First().testPassList.Add(new GroupPortfolioProjectTestPass.TestPass()
                                        {
                                            testpassId = (dr.IsDBNull(testPassIdOrdinal)) == true ? "" : Convert.ToString(dr[testPassIdOrdinal]),
                                            testpassName = (dr.IsDBNull(testPassNameOrdinal)) == true ? "" : Convert.ToString(dr[testPassNameOrdinal]),

                                            testCaseList = _testCase,

                                        });
                                }

                                #endregion
                            }
                        }
                    }
                    return Json(groupPortfolioProjectTestPassResult);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
