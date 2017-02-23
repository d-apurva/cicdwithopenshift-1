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

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class ProjectMgmtController : Controller
    {
        clsDbContext _context;
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        public ProjectMgmtController(clsDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("GetProjectDetails")]
        public List<clsProjectDetails> GetProjectDetails()
        {
            List<clsProjectDetails> listProjectDetail = new List<clsProjectDetails>();
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
                List<DetailAnalysis> listDetailAnalysis = new List<DetailAnalysis>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spProjectSelect";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar, 500) { Value = SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {

                            List<ProjectUser> listProjectUsers = new List<ProjectUser>();

                            #region 'New Iteration'
                            string lead = Convert.ToString(dr["Project_Lead"]);
                            string stakeholders = Convert.ToString(dr["Project_SH"]);
                            //lead
                            if (!string.IsNullOrEmpty(lead))
                            {
                                string[] leadDetail = lead.Split(',');
                                listProjectUsers.Add(new ProjectUser()
                                {
                                    spUserId = !string.IsNullOrEmpty(leadDetail[0]) ? Convert.ToString(leadDetail[0]) : string.Empty,
                                    userName = !string.IsNullOrEmpty(leadDetail[1]) ? Convert.ToString(leadDetail[1]) : string.Empty,
                                    alias = !string.IsNullOrEmpty(leadDetail[2]) ? Convert.ToString(leadDetail[2]) : string.Empty,
                                    email = !string.IsNullOrEmpty(leadDetail[3]) ? Convert.ToString(leadDetail[3]) : string.Empty,
                                    securityId = !string.IsNullOrEmpty(leadDetail[4]) ? Convert.ToString(leadDetail[4]) : string.Empty,
                                });
                            }
                            //multiple stakeholder
                            if (!string.IsNullOrEmpty(stakeholders))
                            {
                                string[] SH = stakeholders.Split('~');
                                if (SH != null && SH.Length > 0)
                                {
                                    foreach (string SHData in SH)
                                    {
                                        if (!string.IsNullOrEmpty(SHData))
                                        {
                                            string[] SHDetail = SHData.Split(',');

                                            listProjectUsers.Add(new ProjectUser()
                                            {
                                                spUserId = !string.IsNullOrEmpty(SHDetail[0]) ? Convert.ToString(SHDetail[0]) : string.Empty,
                                                userName = !string.IsNullOrEmpty(SHDetail[1]) ? Convert.ToString(SHDetail[1]) : string.Empty,
                                                alias = !string.IsNullOrEmpty(SHDetail[2]) ? Convert.ToString(SHDetail[2]) : string.Empty,
                                                email = !string.IsNullOrEmpty(SHDetail[3]) ? Convert.ToString(SHDetail[3]) : string.Empty,
                                                securityId = !string.IsNullOrEmpty(SHDetail[4]) ? Convert.ToString(SHDetail[4]) : string.Empty,
                                            });
                                        }
                                    }
                                }
                            }





                            listProjectDetail.Add(new clsProjectDetails()
                            {
                                groupId = Convert.ToString(dr["Group_Id"]),
                                portfolioId = Convert.ToString(dr["Portfolio_Id"]),
                                projectId = Convert.ToString(dr["Project_Id"]),
                                projectName = Convert.ToString(dr["Project_Name"]),
                                projectVersion = Convert.ToString(dr["Project_Version"]),
                                projectStatus = Convert.ToString(dr["Project_Status"]),
                                startDate = Convert.ToString(dr["Start_Date"]) == "" ? "" : Convert.ToDateTime(Convert.ToString(dr["Start_Date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                endDate = Convert.ToString(dr["End_Date"]) == "" ? "" : Convert.ToDateTime(Convert.ToString(dr["End_Date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                description = Convert.ToString(dr["Project_Description"]),
                                aliasAppUrl = Convert.ToString(dr["AppAlias_Url"]),
                                appUrl = Convert.ToString(dr["App_Url"]),
                                projectUrl = Convert.ToString(dr["Project_Url"]),
                                projectAliasUrl = Convert.ToString(dr["ProjectAlias_Url"]),
                                testpass_Count = Convert.ToString(dr["tp_Count"]),
                                testpass_MinDate = Convert.ToString(dr["tp_MinDate"]) == "" ? "" : Convert.ToDateTime(Convert.ToString(dr["tp_MinDate"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                testpass_MaxDate = Convert.ToString(dr["tp_MaxDate"]) == "" ? "" : Convert.ToDateTime(Convert.ToString(dr["tp_MaxDate"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                listProjectUsers = listProjectUsers
                            });
                            #endregion

                        }

                    }


                    return listProjectDetail.ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }



        }

        [HttpGet, Route("GetGrpPortProjects")]
        public JsonResult GetGrpPortProjects()
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetGroupPortfolio";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
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

        [HttpPost, Route("InsertUpdateProjectDetails")]
        public Dictionary<string, string> InsertUpdateProjectDetails([FromBody]clsProjectDetails prjDetail)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            if (prjDetail.listProjectUsers.Count == 0)
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "ProjectLead is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(prjDetail.groupId) || string.IsNullOrEmpty(prjDetail.portfolioId) || string.IsNullOrEmpty(prjDetail.projectName) || string.IsNullOrEmpty(prjDetail.projectVersion) || string.IsNullOrEmpty(prjDetail.startDate) || string.IsNullOrEmpty(prjDetail.endDate))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Mandatory Fields data is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    myDataCollection UserDataTable = new myDataCollection();
                    foreach (ProjectUser ss in prjDetail.listProjectUsers)
                    {
                        UserDataTable.Add(new clsUserDataTable
                        {
                            spUser_Id = Convert.ToInt16(ss.spUserId),
                            spUser_name = ss.userName,
                            spUser_Alias = ss.alias,
                            spUser_EmailId = ss.email,
                            spUser_desgnID = Convert.ToInt16(ss.securityId)
                        });
                    }

                    string statementType = string.Empty;
                    if (string.IsNullOrEmpty(prjDetail.projectId))
                        statementType = "Insert";
                    else
                        statementType = "Update";

                    cmd.CommandText = "UAT.spProjectInsUpd";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectName", SqlDbType.VarChar, 500) { Value = prjDetail.projectName });
                    cmd.Parameters.Add(new SqlParameter("@ProjectStatus", SqlDbType.VarChar, 500) { Value = prjDetail.projectStatus });
                    cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.VarChar, 500) { Value = (!string.IsNullOrEmpty(prjDetail.startDate)) ? prjDetail.startDate : Convert.ToString(DBNull.Value) });
                    cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.VarChar, 500) { Value = (!string.IsNullOrEmpty(prjDetail.endDate)) ? prjDetail.endDate : Convert.ToString(DBNull.Value) });
                    cmd.Parameters.Add(new SqlParameter("@ProjectDescription", SqlDbType.VarChar, 500) { Value = prjDetail.description });
                    cmd.Parameters.Add(new SqlParameter("@ProjectVersion", SqlDbType.VarChar, 500) { Value = prjDetail.projectVersion });
                    cmd.Parameters.Add(new SqlParameter("@GroupId", SqlDbType.VarChar, 500) { Value = prjDetail.groupId });
                    cmd.Parameters.Add(new SqlParameter("@PortFolioId", SqlDbType.VarChar, 500) { Value = prjDetail.portfolioId });
                    cmd.Parameters.Add(new SqlParameter("@AppAliasUrl", SqlDbType.VarChar, 500) { Value = prjDetail.aliasAppUrl });
                    cmd.Parameters.Add(new SqlParameter("@AppUrl", SqlDbType.VarChar, 500) { Value = prjDetail.appUrl });
                    cmd.Parameters.Add(new SqlParameter("@ProjectUrl", SqlDbType.VarChar, 500) { Value = prjDetail.projectUrl });
                    cmd.Parameters.Add(new SqlParameter("@ProjectAliasUrl", SqlDbType.VarChar, 500) { Value = prjDetail.projectAliasUrl });
                    cmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.VarChar, 500) { Value = prjDetail.projectId });
                    cmd.Parameters.Add(new SqlParameter("@spUsertable", SqlDbType.Structured) { Value = UserDataTable });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = statementType });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = SpUserId }); //new UATClient().GetLoggedInUserSPUserId();
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
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
                            _result.Add(this._statusText, "Done");
                        }
                        else
                            _result.Add(this._statusText, "Done");
                    }



                }


            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }
            return _result;
        }

        [HttpDelete, Route("DeleteProject")]
        public Dictionary<string, string> DeleteProject([FromBody]clsdata cls)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            if (string.IsNullOrEmpty(cls.projectId))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "projectId is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "UAT.spProjectDelete";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.VarChar, 500) { Value = cls.projectId });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = SpUserId}); //new UATClient().GetLoggedInUserSPUserId();
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add(this._errorText, "Error");
                        }
                        else
                        {
                            _result.Add("DBStatus", _retValue);
                            _result.Add(this._statusText, "Done");
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

        [HttpPost, Route("ValidateProjectName")]
        public Dictionary<string, string> ValidateProjectName([FromBody]ValidateProjectNameParam parameter)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            //Check for GroupName 
            string projectName = ""; string versionName = "";
            if (parameter.parameter != "" && parameter.parameter.IndexOf('/') >= 0 && parameter!=null)
            {
                projectName = parameter.parameter.Split('/')[0];
                versionName = parameter.parameter.Split('/')[1];
            }
            if (string.IsNullOrEmpty(projectName))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Project Name is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(versionName))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Version Name is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.SpProjectCheck";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectName", SqlDbType.VarChar, 500) { Value = projectName });
                    cmd.Parameters.Add(new SqlParameter("@ProjectVersion", SqlDbType.VarChar, 500) { Value = versionName });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0 || result==-1)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add("Error", cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString());

                        }
                        else
                            _result.Add(this._statusText, _retValue);
                    }



                }


            }
            catch (Exception ex)
            {

                _result.Add(this._errorText, ex.Message);
            }

            return _result;
        }

        [HttpPost, Route("InsertGroup")]
        public Dictionary<string, string> InsertGroup([FromBody]clsgroupName groupName)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            //Check for GroupName 
            if (string.IsNullOrEmpty(groupName.groupName))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Group Name is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.spGroupNames";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 500) { Value = groupName.groupName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Insert" });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add(this._errorText, cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString());

                        }
                        else
                        {
                            _result.Add("Value", _retValue);
                            _result.Add(this._statusText, "Done");
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

        [HttpPost, Route("InsertPortfolio")]
        public Dictionary<string, string> InsertPortfolio([FromBody]clsprocess cls)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            //Check for GroupName 
            //Check for GroupName 
            if (string.IsNullOrEmpty(cls.groupID))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "groupID is required");
                return _result;
            }
            else if (string.IsNullOrEmpty(cls.portfolioName))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "portfolioName is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.spPortFolio";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GroupId", SqlDbType.VarChar, 500) { Value = cls.groupID });
                    cmd.Parameters.Add(new SqlParameter("@PortfolioName", SqlDbType.VarChar, 500) { Value = cls.portfolioName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Insert" });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add(this._errorText, cmd.Parameters[cmd.Parameters.Count - 1].Value.ToString());

                        }
                        else
                        {
                            _result.Add("Value", _retValue);
                            _result.Add(this._statusText, "Done");
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

        [HttpDelete, Route("DeleteGroup")]
        public Dictionary<string, string> DeleteGroup([FromBody]clsgroupId groupId)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();

            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            //Check for GroupName 
            if (string.IsNullOrEmpty(groupId.groupId))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "groupId is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "UAT.spGroupNames";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GroupId", SqlDbType.VarChar, 500) { Value = groupId.groupId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = SpUserId }); //new UATClient().GetLoggedInUserSPUserId();
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add(this._errorText, "Error");
                        }
                        else
                        {
                            _result.Add("DBStatus", _retValue);
                            _result.Add(this._statusText, "Done");
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

        [HttpDelete, Route("DeletePortfolio")]
        public Dictionary<string, string> DeletePortfolio([FromBody]clsportfolioID portfolioID)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();
            List<clsProjectDetails> listProjectUsers = new List<clsProjectDetails>();
            string SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            //Check for GroupName 
            if (string.IsNullOrEmpty(portfolioID.portfolioID))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "portfolioID is required");
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
                List<GroupPortfolio> listGroup = new List<GroupPortfolio>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "UAT.spPortFolio";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@portFolioID", SqlDbType.VarChar, 500) { Value = portfolioID.portfolioID });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.VarChar, 500) { Value = SpUserId }); //new UATClient().GetLoggedInUserSPUserId();
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (result > 0)
                    {
                        if (string.IsNullOrEmpty(_retValue))
                        {
                            _result.Add(this._errorText, "Error");
                        }
                        else
                        {
                            _result.Add("DBStatus", _retValue);
                            _result.Add(this._statusText, "Done");
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


    }
}
