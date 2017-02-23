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
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]


    public class TestPassController : Controller
    {
        private string _errorText = "ErrorDetails";
        private string _statusText = "Success";
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";

        private clsDbContext _context;
        public TestPassController(clsDbContext context)
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




        [HttpGet, Route("GetTestPassDetailForProjectId/{projectId}/{userId}")]
        public JsonResult GetTestPassDetailForProjectId(int projectId, string userId)
        //[HttpGet, Route("GetTestPassDetailForProjectId/{projectId}")]
        //public JsonResult GetTestPassDetailForProjectId(int projectId)
        {
            try
            {
                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
                // string userid = Convert.ToInt32("userId").ToString();
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


                //  string listTestMgr;

                List<TestPass> listTP = new List<TestPass>();
                List<ProjectUser> listMgr = new List<ProjectUser>();
                //string SchemaName = new clsUatClient(_context).GetClientSchema(Appurl);

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.SpTPSelect";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId });
                    //   cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = _SpUserId });
                    cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });

                    var statementType = "S2";
                    SqlParameter sttype = new SqlParameter("@StatementType", SqlDbType.NVarChar, 10);
                    sttype.Value = statementType;
                    cmd.Parameters.Add(sttype);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();



                    using (var dr = cmd.ExecuteReader())

                    {
                        if (dr.HasRows)
                        {
                            int userNameOrdinal = dr.GetOrdinal("User_Name");
                            int aliasOrdinal = dr.GetOrdinal("User_Alias");
                            int emailOrdinal = dr.GetOrdinal("User_EmailId");
                            //  int securityIdOrdinal = "";
                            int spUserIdOrdinal = dr.GetOrdinal("User_SpUserID");

                            // for listTP
                            int testPassIdOrdinal = dr.GetOrdinal("TestPass_Id");
                            int projectIdOrdinal = dr.GetOrdinal("Project_Id");
                            int testPassNameOrdinal = dr.GetOrdinal("TestPass_Name");
                            int testPassDespOrdinal = dr.GetOrdinal("TestPass_Description");
                            int tpEndDateOrdinal = dr.GetOrdinal("End_date");
                            int tpStartDateOrdinal = dr.GetOrdinal("Start_date");
                            int tpStatusOrdinal = dr.GetOrdinal("Status");
                            int totalTestCaseCountOrdinal = dr.GetOrdinal("tcCount");



                            while (dr.Read())
                            {
                                listMgr = new List<ProjectUser>();
                                listMgr.Add(new ProjectUser
                                {

                                    userName = (dr.IsDBNull(userNameOrdinal)) == true ? "" : Convert.ToString(dr[userNameOrdinal]),
                                    alias = (dr.IsDBNull(aliasOrdinal)) == true ? "" : Convert.ToString(dr[aliasOrdinal]),
                                    email = (dr.IsDBNull(emailOrdinal)) == true ? "" : Convert.ToString(dr[emailOrdinal]),
                                    securityId = "",
                                    spUserId = (dr.IsDBNull(spUserIdOrdinal)) == true ? "" : Convert.ToString(dr[spUserIdOrdinal]),

                                });
                                listTP.Add(new TestPass
                                {

                                    testPassId = (dr.IsDBNull(testPassIdOrdinal) == true ? "" : Convert.ToString(dr["TestPass_Id"])),
                                    projectId = (dr.IsDBNull(projectIdOrdinal) == true ? "" : Convert.ToString(dr["Project_Id"])),
                                    testPassName = (dr.IsDBNull(testPassNameOrdinal) == true ? "" : Convert.ToString(dr["TestPass_Name"])),
                                    testPassDesp = (dr.IsDBNull(testPassDespOrdinal) == true ? "" : Convert.ToString(dr["TestPass_Description"])),
                                    tpEndDate = Convert.ToDateTime(Convert.ToString(dr["End_date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                    tpStartDate = Convert.ToDateTime(Convert.ToString(dr["Start_date"])).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                    tpStatus = (dr.IsDBNull(tpStatusOrdinal)) == true ? "" : ReplaceStatus(Convert.ToString(dr["Status"])),
                                    totalTestCaseCount = (dr.IsDBNull(totalTestCaseCountOrdinal)) == true ? "" : Convert.ToString(dr["tcCount"]),
                                    listTestMgr = listMgr
                                });

                                //retObject.Add(dr);
                            }
                        }

                    }
                    return Json(listTP);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }


        private string GetShortStatus(string Status)
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(Status))
            {
                retVal = string.Empty;
            }
            else if (Status.ToUpper() == "ACTIVE")
            {
                retVal = "A";
            }
            else if (Status.ToUpper() == "ON HOLD")
            {
                retVal = "H";
            }
            else if (Status.ToUpper() == "COMPLETE")
            {
                retVal = "C";
            }
            else
            {
                retVal = Status;
            }

            return retVal;
        }

        private string ReplaceStatus(string Status)
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(Status))
            {
                retVal = string.Empty;
            }
            else if (Status.ToUpper() == "A")
            {
                retVal = "Active";
            }
            else if (Status.ToUpper() == "H")
            {
                retVal = "On Hold";
            }
            else if (Status.ToUpper() == "C")
            {
                retVal = "Completed";
            }
            else
            {
                retVal = Status;
            }

            return retVal;
        }





        [HttpGet, Route("GetAllTesterRolePFNCountForTestPassId/{testpassid}")]
        public TestPassTesterRolePFNCount GetAllTesterRolePFNCountForTestPassId(string testpassid)
        {
            try
            {
                Dictionary<string, string> _result = new Dictionary<string, string>();
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
                string status = "";
                string statementtype = "";

                if (string.IsNullOrEmpty(testpassid))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "testpassid is required");
                    // return _result;
                }

                List<TesterRolePFNCount> listTesterRolePFNCount = new List<TesterRolePFNCount>();
                TestPassTesterRolePFNCount objMain = null;

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spTesterRolewise";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = testpassid });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Select" });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int result = cmd.ExecuteNonQuery();
                    string _retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                    if (_retValue.ToLower() == "success")
                    {
                        var retObject = new List<dynamic>();
                        using (var dr = cmd.ExecuteReader())
                        {
                            int count = 0;
                            int tcCount = 0;
                            int testingTime = 0;
                            string testPassStatus = "";
                            int roleid = 0;
                            int userid = 0;
                            while (dr.Read())
                            {
                                //if (dr.HasRows)
                                //{
                                int Status = dr.GetOrdinal("Status");
                                int CurrentETT = dr.GetOrdinal("CurrentETT");
                                int User_Name = dr.GetOrdinal("User_Name");
                                int User_EmailId = dr.GetOrdinal("User_EmailId");
                                int Role_Name = dr.GetOrdinal("Role_Name");
                                int passSteps = dr.GetOrdinal("passSteps");
                                int failSteps = dr.GetOrdinal("failSteps");
                                int ncSteps = dr.GetOrdinal("ncSteps");
                                int Feedback = dr.GetOrdinal("Feedback");

                                //listttt
                                List<clsTestPassTesterRolePFNCountList> lst = new List<clsTestPassTesterRolePFNCountList>();
                                lst.Add(new clsTestPassTesterRolePFNCountList()
                                {

                                    Status = Convert.ToString(dr[Status]),
                                    CurrentETT = Convert.ToString(dr[CurrentETT]),
                                    User_Name = Convert.ToString(dr[User_Name]),
                                    User_EmailId = Convert.ToString(dr[User_EmailId]),
                                    Role_Name = Convert.ToString(dr[Role_Name]),
                                    passSteps = Convert.ToString(dr[passSteps]),
                                    failSteps = Convert.ToString(dr[failSteps]),
                                    ncSteps = Convert.ToString(dr[ncSteps]),
                                    Feedback = Convert.ToString(dr[Feedback]),


                                });

                                count++;
                                testPassStatus = (dr.IsDBNull(Status)) == true ? "" : Convert.ToString(dr["Status"]);
                                roleid = Convert.ToInt32(dr["Role_ID"]);
                                userid = Convert.ToInt32(dr["User_Id"]);
                                tcCount++;
                                if (!(dr.IsDBNull(dr.GetOrdinal("CurrentETT"))))
                                    testingTime = testingTime + Convert.ToInt32(dr["CurrentETT"]);
                                if (lst.Count == count)
                                // if (dr.FieldCount == count)
                                {
                                    listTesterRolePFNCount.Add(new TesterRolePFNCount
                                    {
                                        testerSpUserId = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]),
                                        testerName = (dr.IsDBNull(User_Name)) == true ? "" : Convert.ToString(dr["User_Name"]),
                                        testerEmail = (dr.IsDBNull(User_EmailId)) == true ? "" : Convert.ToString(dr["User_EmailId"]),
                                        testerRoleId = (dr["Role_ID"] == null) ? "" : Convert.ToString(dr["Role_ID"]),
                                        testerRoleName = (dr.IsDBNull(Role_Name)) == true ? "" : Convert.ToString(dr["Role_Name"]),
                                        passCount = (dr.IsDBNull(passSteps)) == true ? "" : Convert.ToString(dr["passSteps"]),
                                        failCount = (dr.IsDBNull(failSteps)) == true ? "" : Convert.ToString(dr["failSteps"]),
                                        NCCount = (dr.IsDBNull(ncSteps)) == true ? "" : Convert.ToString(dr["ncSteps"]),
                                        TCCount = Convert.ToString(tcCount),
                                        TestingTime = Convert.ToString(testingTime),
                                        FeedbackAvailable = (dr.IsDBNull(Feedback)) == true ? "" : Convert.ToString(dr["Feedback"])
                                    });
                                }

                                else if (Convert.ToInt32(dr["Role_ID"]) != roleid || Convert.ToInt32(dr["User_Id"]) != userid)

                                {
                                    listTesterRolePFNCount.Add(new TesterRolePFNCount
                                    {
                                        testerSpUserId = (dr["User_Id"] == null) ? "" : Convert.ToString(dr["User_Id"]),
                                        testerName = (dr.IsDBNull(User_Name)) == true ? "" : Convert.ToString(dr["User_Name"]),
                                        testerEmail = (dr.IsDBNull(User_EmailId)) == true ? "" : Convert.ToString(dr["User_EmailId"]),
                                        testerRoleId = (dr["Role_ID"] == null) ? "" : Convert.ToString(dr["Role_ID"]),
                                        testerRoleName = (dr.IsDBNull(Role_Name)) == true ? "" : Convert.ToString(dr["Role_Name"]),
                                        passCount = (dr.IsDBNull(passSteps)) == true ? "" : Convert.ToString(dr["passSteps"]),
                                        failCount = (dr.IsDBNull(failSteps)) == true ? "" : Convert.ToString(dr["failSteps"]),
                                        NCCount = (dr.IsDBNull(ncSteps)) == true ? "" : Convert.ToString(dr["ncSteps"]),
                                        TCCount = Convert.ToString(tcCount),
                                        TestingTime = Convert.ToString(testingTime),
                                        FeedbackAvailable = (dr.IsDBNull(Feedback)) == true ? "" : Convert.ToString(dr["Feedback"])
                                    });
                                    tcCount = 0;
                                    testingTime = 0;
                                }


                            }
                            objMain = new TestPassTesterRolePFNCount();
                            objMain.testPassId = testpassid;
                            objMain.testPassStatus = testPassStatus;
                            objMain.listTesterRolePFNCount = listTesterRolePFNCount;
                            // }
                        }
                    }
                    else
                    {
                        objMain = new TestPassTesterRolePFNCount();
                        string ReturnParamValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);
                        if (ReturnParamValue.ToLower() == "tester present")
                        {
                            objMain.testPassStatus = "tester present";
                        }
                        else
                            objMain.testPassStatus = "tester not present";
                        //ExceptionHelper.TraceDBLevelException("No Data Found");
                        //return objMain;
                    }
                    return objMain;
                }
            }

            catch (Exception ex)
            {

                return null;
            }


            //   return objMain;


        }












        /// <summary>
        /// new InsertUpdateTestPass using  Dictionary
        /// </summary>
        /// <param name="testpassId"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertUpdateTestPass")]
        public Dictionary<string, string> InsertUpdateTestPass([FromBody]TestPass testpass)
        {
            Dictionary<string, string> _result = new Dictionary<string, string>();



            List<ProjectUser> listMgr = new List<ProjectUser>();

            if (string.IsNullOrEmpty(testpass.testPassName) || string.IsNullOrEmpty(testpass.tpStartDate) || string.IsNullOrEmpty(testpass.tpEndDate))
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "Mandatory Fields data is required");
                return _result;
            }
            else if (testpass.listTestMgr.Count > 0)
            {
                if (string.IsNullOrEmpty(testpass.listTestMgr[0].alias) || string.IsNullOrEmpty(testpass.listTestMgr[0].email) || string.IsNullOrEmpty(testpass.listTestMgr[0].userName) || string.IsNullOrEmpty(testpass.listTestMgr[0].spUserId))
                {
                    //send service level Exception as service response
                    _result.Add(this._errorText, "TestManager complete details required");
                    return _result;
                }
            }
            else if (testpass.listTestMgr.Count == 0)
            {
                //send service level Exception as service response
                _result.Add(this._errorText, "TestManager detail required");
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
                if (string.IsNullOrEmpty(testpass.testPassId))
                    statementType = "Insert";
                else
                    statementType = "Update";


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.SpTestPass";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testpass.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = testpass.projectId });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrId", SqlDbType.Int) { Value = testpass.listTestMgr[0].spUserId });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrName", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].userName });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrAlias", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].alias });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrEmailId", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].email });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrSecurityId", SqlDbType.Int) { Value = testpass.listTestMgr[0].securityId });
                    cmd.Parameters.Add(new SqlParameter("@TestPassName", SqlDbType.NVarChar, 500) { Value = testpass.testPassName });
                    cmd.Parameters.Add(new SqlParameter("@DisplayID", SqlDbType.NVarChar, 500) { Value = testpass.testPassDisplayId });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = testpass.testPassDesp });
                    cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.VarChar, 500) { Value = testpass.tpStartDate });
                    cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.VarChar, 500) { Value = testpass.tpEndDate });
                    cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 500) { Value = testpass.tpStatus });
                    if (!string.IsNullOrEmpty(testpass.tpStatus))
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = GetShortStatus(testpass.tpStatus);
                    else
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = statementType });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    //  int i = cmd.ExecuteNonQuery();

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


        [HttpDelete, Route("DeleteTestPass")]
        public JsonResult DeleteTestPass([FromBody] ClsTestPassParm testpassId)
        {

            if (string.IsNullOrEmpty(testpassId.testPassId))
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add("ERROR", "testpassId is required");
                return Json(oError);
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
                    return Json("Invalid Url");

                }


                string statementType = string.Empty;

                string SuccessMsg = "";
                //string SchemaName = new clsUatClient(_context).GetClientSchema(Appurl);
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "[UAT].[SpTestPass]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testpassId.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = new clsUtility().GetLoggedInUserSPUserId() == "" ? null : new clsUtility().GetLoggedInUserSPUserId() });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    SqlParameter outRet_Parameter = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outRet_Parameter);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = cmd.ExecuteNonQuery();

                    var outparam1 = outRet_Parameter.Value;
                    if (i == -1)
                    {

                        if (outparam1 != null && Convert.ToString(outparam1) == "SUCCESS")
                        {

                            SuccessMsg = "Done";
                        }
                    }
                    return Json(SuccessMsg);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }








        #region 'UploadAttachment'

        public bool inFile(IFormFile file)
        {
            return (file != null && file.Length > 0) ? true : false;
        }
        [HttpPost, Route("UploadAttachmentForMail")]
        public string UploadAttachmentForMail()
        {

            byte[] fileRecord = null;
            string outval = "";
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
                if (inFile(Request.Form.Files[0]))
                {


                    string fileType = Request.Form.Files[0].ContentType;
                    Stream file_strm = Request.Form.Files[0].OpenReadStream();
                    string file_Name = Path.GetFileName(Request.Form.Files[0].FileName);
                    int fileSize = Convert.ToInt32(Request.Form.Files[0].Length);
                    fileRecord = new byte[fileSize];
                    file_strm.Read(fileRecord, 0, fileSize);
                }




                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {



                    cmd.CommandText = "[UAT].[spAddTpAttachment]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 500) { Value = "Att.png" });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentImg", SqlDbType.VarBinary, 500000000) { Value = fileRecord });

                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "InsertData" });

                    cmd.Parameters.Add(new SqlParameter("@outval", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = cmd.ExecuteNonQuery();
                    outval = Convert.ToString(cmd.Parameters["@outval"].Value);


                }

            }
            catch (Exception ex)
            {

                _msg = (ex.Message);
            }
            return outval;

        }


        [HttpGet, Route("GetAttachmentFile")]
        public byte[] GetAttachmentFile(string id, string appurl)
        {

            byte[] fileContent = new byte[0];
            try
            {
                string SchemaName = "";

                string AppUrl = appurl;

                string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {


                }

                var cid = "";
                var filename = "";


                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "[UAT].[spAddTpAttachment]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "GetAttach" });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int) { Value = id });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["FileName"].ToString();
                            cid = "myimagecid";
                            fileContent = (byte[])dataReader["AttachmentImg"];
                            Response.ContentType = "image/png";
                            Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["FileName"].ToString());
                            Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                        }

                        // return File(fileContent, contentType, filename);

                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }
            return fileContent;

        }


    

        [HttpGet, Route("GetAttachmentFile_Core2")]
        public JsonResult GetAttachmentFile_Core2(string id, string appurl)
        {

            byte[] fileContent = new byte[0];
            try
            {
                string SchemaName = "";

                string _msg = "";


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
                var filename = "";
                var contentType = "";
                string base64String = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "[UAT].[spAddTpAttachment]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = "GetAttach" });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int) { Value = id });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["FileName"].ToString();

                            fileContent = (byte[])dataReader["AttachmentImg"];

                            base64String = Convert.ToBase64String(fileContent, 0, fileContent.Length);
                            Response.Headers["content-Id"] = "A";
                            Response.ContentType = "multipart/related";
                            contentType = "image/png";
                            Response.Headers.Add("content-disposition", "inline;filename=" + dataReader["FileName"].ToString());
                            Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                        }

                        // obj.fileData = "data:" + contentType + ";base64," + base64String;
                        return Json("data:" + contentType + ";base64," + base64String);

                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }
            // return fileContent;

        }

        #endregion


















        // POST api/TestPass
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        //POST: /TestPass
        [HttpPost, Route("InsertUpdateTestPass_1")]
        public JsonResult InsertUpdateTestPass_1([FromBody]TestPass testpass)
        {

            if (string.IsNullOrEmpty(testpass.testPassName) || string.IsNullOrEmpty(testpass.tpStartDate) || string.IsNullOrEmpty(testpass.tpEndDate))
            {

                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add("ERROR", "Mandatory fields are required");
                return Json(oError);
            }
            else if (testpass.listTestMgr.Count > 0)
            {
                if (string.IsNullOrEmpty(testpass.listTestMgr[0].alias) || string.IsNullOrEmpty(testpass.listTestMgr[0].email) || string.IsNullOrEmpty(testpass.listTestMgr[0].userName) || string.IsNullOrEmpty(testpass.listTestMgr[0].spUserId))
                {

                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add("ERROR", "TestManager complete details required");
                    return Json(oError);
                }
            }
            else if (testpass.listTestMgr.Count == 0)
            {

                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add("ERROR", "TestManager detail required");
                return Json(oError);
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
                    return Json("Invalid Url");

                }


                string statementType = string.Empty;
                if (string.IsNullOrEmpty(testpass.testPassId))
                    statementType = "Insert";
                else
                    statementType = "Update";



                string SuccessMsg = "";
                //  string SchemaName = new clsUatClient(_context).GetClientSchema(Appurl);
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "UAT.SpTestPass";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.Int) { Value = testpass.testPassId });
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int, 500) { Value = testpass.projectId });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrId", SqlDbType.Int, 500) { Value = testpass.listTestMgr[0].spUserId });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrName", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].userName });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrAlias", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].alias });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrEmailId", SqlDbType.VarChar, 500) { Value = testpass.listTestMgr[0].email });
                    cmd.Parameters.Add(new SqlParameter("@TestMgrSecurityId", SqlDbType.Int, 500) { Value = testpass.listTestMgr[0].securityId });
                    cmd.Parameters.Add(new SqlParameter("@TestPassName", SqlDbType.NVarChar, 500) { Value = testpass.testPassName });
                    cmd.Parameters.Add(new SqlParameter("@DisplayID", SqlDbType.NVarChar, 500) { Value = testpass.testPassDisplayId });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = testpass.testPassDesp });
                    cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date, 500) { Value = testpass.tpStartDate });
                    cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date, 500) { Value = testpass.tpEndDate });
                    cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 500) { Value = testpass.tpStatus });
                    if (!string.IsNullOrEmpty(testpass.tpStatus))
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = GetShortStatus(testpass.tpStatus);
                    else
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = statementType });
                    SqlParameter outparam = new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(outparam);
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    int i = cmd.ExecuteNonQuery();
                    var outparam1 = outparam.Value;
                    if (i == -1)
                    {
                        if (Convert.ToString(outparam1) != "" && outparam1 != null)
                        {
                            //SuccessMsg = "Insert Successfully..!!";
                            SuccessMsg = "Done";
                        }
                    }
                    return Json(SuccessMsg);
                }





            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }



        [HttpGet, Route("GetFileToDownload")]
        //public byte[] GetFileToDownload(int id)
        public FileContentResult GetFileToDownload(string baseStr, string Url)
        {

            byte[] fileContent = new byte[0];
            var filename = "";
            var contentType = "";

            try
            {
                //baseStr="iVBORw0KGgoAAAANSUhEUgAABFQAAALKCAIAAACTI5IOAAAAAXNSR0IArs4c6QAAqIVJREFUeF7t3Q2cFWXd / 3HW0kxTxFREQJFAy3xAQBFFrGRFJEsLVNRIMzEs + svtA9xmiA95g1hwa6nRA8ZtooKpGSGBmYgiCIhiaoKKiiKigrvirusu / L / 4w8txztOcc + bMmXPmM6997evsnGuuh / c1O2d + c10zp2bz5s2twlg2bdrU0tLS3Nzc1NTU2NjY0NCwcePG + vr6urq62trampqaggtZv359wdu6DS + 66KL58 + cHyad79 + 433nijUp500klvvPGGXrg1QTbPmWbBggXnn3++S3bYYYdNnjxZfw4cOPD1119365944glvVt53v / 3tb48dO9a9q7e + 9a1vnXfeeVpz6KGHerdSQ3r37q01L7744ne / +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";
                //baseStr = baseStr.Replace(' ', '+');
                byte[] imageBytes = Convert.FromBase64String(baseStr/*.file*/);
                //MemoryStream ms = new MemoryStream(imageBytes, 0,imageBytes.Length);

                //filename = dataReader["FileName"].ToString();
                filename = "abc.png";
                //contentType = dataReader["ContentType"].ToString();
                contentType = "image/png";
                // fileContent = (byte[])dataReader["FileData"];
                fileContent = imageBytes;

                Response.ContentType = contentType;
                //Response.Headers.Add("content-disposition", "inline;filename=" + dataReader["FileName"].ToString());
                Response.Headers.Add("content-disposition", "inline;filename=" + filename);
                Response.Body.WriteAsync(fileContent, 0, fileContent.Length);

                return File(fileContent, contentType, filename);
            }
            catch (Exception ex)
            {
                return null;
            }

        }




        //Rupal

        [HttpGet, Route("GetFileToDownloadActResult")]
        public FileContentResult GetFileToDownloadActResult(int id, string Url)
        {
            byte[] fileContent = new byte[0];
            try
            {
                string SchemaName = "";
                string AppUrl = Url;
             

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }

                var filename = "";
                var contentType = "";
                string base64String = "";
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.SpGetImageToSendEmail";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int) { Value = id });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                           
                            filename = "Att.png";
                            contentType = "image/png";
                            fileContent = (byte[])dataReader["AttachmentImg"];

                            base64String = Convert.ToBase64String(fileContent, 0, fileContent.Length);
                            Response.ContentType = contentType;
                            Response.Headers.Add("content-disposition", "Attachment;filename=" + filename);
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
        //Rupal

    }
}


