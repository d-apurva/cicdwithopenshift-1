using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DashboardController : Controller
    {
        private clsDbContext _context;
        private string _returnParameter = "@Ret_Parameter";

        public DashboardController(clsDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public string Get()
        {
            return "value";
        }
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet, Route("GetDashboardData")]
        public JsonResult GetDashboardData()
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
                    return Json("Invalid Url");

                }

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spDashboardReport";
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
        [HttpGet, Route("GetUserProjectsWithSecurity")]
        public JsonResult GetUserProjectsWithSecurity()
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
                    cmd.CommandText = "UAT.spGetProjectsWithSecurityForUser";
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

        [HttpGet, Route("GetDropdownDataForDetailAnalysis")]
        public JsonResult GetDropdownDataForDetailAnalysis()
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
                    cmd.CommandText = "UAT.spDetailAnalysisDropDown";
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

                            for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                            {
                                var Name = dataReader.GetName(iFiled);
                                var Value = dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled];
                                List<clsTestterList> oTester = new List<clsTestterList>();
                                string valueCheck = Convert.ToString(Value);
                                if (!string.IsNullOrEmpty(valueCheck))
                                {
                                    #region ' XMl Parsing '

                                    if (valueCheck.Contains('<'))
                                    {
                                        if (Convert.ToString(Name) == "TesterDetailsRowNew")
                                        {
                                            Name = "lstTesterList";
                                            using (XmlReader reader = XmlReader.Create(new StringReader("<users>" + Value + "</users>")))
                                            {
                                                XmlDocument xml = new XmlDocument();
                                                xml.Load(reader);
                                                XmlNodeList companyList = xml.GetElementsByTagName("user");
                                                foreach (XmlNode node in companyList)
                                                {
                                                    XmlElement companyElement = (XmlElement)node;

                                                    oTester.Add(new clsTestterList
                                                    {
                                                        testerId = (companyElement.GetElementsByTagName("TUserId") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TUserId")[0].InnerText) : "",
                                                        testerName = (companyElement.GetElementsByTagName("TUserName") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TUserName")[0].InnerText) : "",
                                                        roleId = (companyElement.GetElementsByTagName("TRoleId") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TRoleId")[0].InnerText) : "",
                                                        roleName = (companyElement.GetElementsByTagName("TRoleName") != null) ? Convert.ToString(companyElement.GetElementsByTagName("TRoleName")[0].InnerText) : ""
                                                    });
                                                }
                                            }
                                            lst.lstTesterList = oTester;

                                        }
                                    }

                                    #endregion
                                }


                                dataRow.Add(Name, Name == "lstTesterList" ? lst.lstTesterList : Value);
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

        [HttpGet, Route("GetDetailAnalysisData")]
        public JsonResult GetDetailAnalysisData(string ProjectId, string TestPassId = null, string TesterSPUserId = null, string RoleId = null)
        {
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
                List<DetailAnalysis> listDetailAnalysis = new List<DetailAnalysis>();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetDetailAnalysis";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.VarChar, 500) { Value = ProjectId });
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = TestPassId });
                    cmd.Parameters.Add(new SqlParameter("@TesterSPUserId", SqlDbType.VarChar, 500) { Value = TesterSPUserId });
                    cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.VarChar, 500) { Value = RoleId });
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

                                var value = dataReader.GetName(iFiled);
                                var name = dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled];

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

        [HttpGet, Route("GetUsers")]
        public JsonResult GetUsers()
        {
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
                List<DetailAnalysis> listDetailAnalysis = new List<DetailAnalysis>();
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.SP_GetUsers";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });


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

                                var value = dataReader.GetName(iFiled);
                                var name = dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled];

                                dataRow.Add(value, Convert.ToString(name));
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

        [HttpGet, Route("ExportTestersParticipation/{projectId}")]
        public JsonResult ExportTestersParticipation(string projectId)
        {
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];

                string retValue = "";
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
                    cmd.CommandText = "UAT.spExportTestersParticipation";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.VarChar, 500) { Value = projectId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });


                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {

                        retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);

                        while (dr.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            for (var iFiled = 0; iFiled < dr.FieldCount; iFiled++)
                            {

                                dataRow.Add(dr.GetName(iFiled), dr.IsDBNull(iFiled) ? null : dr[iFiled]);
                            }
                            retObject.Add((ExpandoObject)dataRow);
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

        [HttpGet, Route("ExportDetailAnalysis/{projectId}/{testPassId}")]
        public JsonResult ExportDetailAnalysis(string projectId, string testPassid)
        {
            try
            {
                string AppUrl = HttpContext.Request.Headers["appurl"];

                #region 'Initialize Variables'
                //List<EntityDynamicExcel> listExcelInfo = new List<EntityDynamicExcel>();
                //List<RowData> listRowData = new List<RowData>();
                //List<CellData> listData = new List<CellData>();
                //CellData objCell = new CellData();
                //List<RowData> listRowData_Sheet2 = new List<RowData>();

                //List<Excelexport> _exl1 = new List<Excelexport>();
                //List<Excelexport2> _exl2 = new List<Excelexport2>();
                #endregion

                int _rowindex = 1;
                string retValue = "";
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
                    cmd.CommandText = "UAT.spExportDetailAnalysis_test";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.VarChar, 500) { Value = projectId });
                    cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar, 500) { Value = testPassid });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });


                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();
                    List<Export_DA> listExport_DA = new List<Export_DA>();
                    List<Export_DA_replica> listExport_DA_replica = new List<Export_DA_replica>();
                    using (var dr = cmd.ExecuteReader())
                    {

                        retValue = Convert.ToString(cmd.Parameters["@Ret_Parameter"].Value);

                        while (dr.Read())
                        {

                            listExport_DA.Add(new Export_DA
                            {
                                projectId = Convert.ToInt32(dr["projectId"]),
                                projectName = Convert.ToString(dr["projectName"]),
                                Project_Version = Convert.ToString(dr["Project_Version"]),
                                TestPass_ID = Convert.ToInt32(dr["TestPass_ID"]),
                                TestPass_Name = Convert.ToString(dr["TestPass_Name"]),
                                testcaseName = Convert.ToString(dr["testcaseName"]),
                                TestCaseID = Convert.ToInt32(dr["TestCaseID"]),
                                RowID = Convert.ToInt32(dr["RowID"]),
                                userid = Convert.ToInt32(dr["userid"]),
                                TesterId = Convert.ToInt32(dr["TesterId"]),
                                NC = Convert.ToInt32(dr["NC"]),
                                Fail = Convert.ToInt32(dr["Fail"]),
                                Pass = Convert.ToInt32(dr["Pass"]),
                                TcStatusByTester = Convert.ToString(dr["TcStatusByTester"]),
                                MarkDelete = Convert.ToInt32(dr["MarkDelete"])
                            });

                        }
                    }

                    if (listExport_DA != null || listExport_DA.Count != 0)
                    {
                        /************************************************************************************************/

                        int TCCount = 0;
                        //retObject = retObject.AsEnumerable().Where(z => z.Field<string>("TcStatusByTester").ToUpper().Trim() != "NULL").OrderBy(Z => Z.Field<int>("userid")).OrderBy(Z => Z.Field<int>("TestCaseID")).CopyToDataTable();

                        int iTCId, iUId, iOldTCId, iOldUId, iRowId, iOldRowId = 0;
                        string sTcStatus, sOldTcStatus = "";

                        for (int j = 0; j < listExport_DA.Count; j++)
                        {
                            iTCId = listExport_DA[j].TestCaseID;
                            iUId = listExport_DA[j].userid;
                            sTcStatus = listExport_DA[j].TcStatusByTester.ToString().ToUpper().Trim();
                            iRowId = listExport_DA[j].RowID;

                            if (j > 0)
                            {
                                iOldTCId = listExport_DA[j - 1].TestCaseID;
                                iOldUId = listExport_DA[j - 1].userid;
                                sOldTcStatus = listExport_DA[j - 1].TcStatusByTester.ToString().ToUpper().Trim();
                                iOldRowId = listExport_DA[j - 1].RowID;

                                if (iOldTCId == iTCId && iOldUId == iUId)
                                {
                                    if (sTcStatus != sOldTcStatus)
                                    {
                                        if (sOldTcStatus == "PASS" && (sTcStatus == "FAIL" || sTcStatus == "NOTCOMPLETED"))
                                        {

                                            listExport_DA[j - 1].MarkDelete = 1;
                                            continue;
                                        }

                                        if (sOldTcStatus == "NOTCOMPLETED" && sTcStatus == "FAIL")
                                        {
                                            listExport_DA[j - 1].MarkDelete = 1;
                                            continue;
                                        }

                                        if (sTcStatus == "PASS" && (sOldTcStatus == "FAIL" || sOldTcStatus == "NOTCOMPLETED"))
                                        {
                                            listExport_DA[j].MarkDelete = 1;
                                            continue;
                                        }

                                        if (sTcStatus == "NOTCOMPLETED" && sOldTcStatus == "FAIL")
                                        {
                                            listExport_DA[j].MarkDelete = 1;
                                            continue;
                                        }
                                    }
                                    else if (sTcStatus == sOldTcStatus)
                                    {
                                        listExport_DA[j - 1].MarkDelete = 1;
                                        continue;
                                    }
                                }
                            }
                        }

                        /***********************************/
                        //DataRow dr = null;
                        int tcid = 0;
                        int old_tcid = 0;
                        int old_testCaseId = 0;
                        int passTC = 0;
                        int FailTC = 0;
                        int NCTC = 0;
                        List<int> lstTestCaseId = new List<int>();
                        List<int> lstUserId = new List<int>();

                        int flagUserPass = 0;/*to mark weather same user of testpass has been counted for this tc status earlier*/
                        int flagUserFail = 0;/*to mark weather same user of testpass has been counted for this tc status earlier*/
                        int flagUserNC = 0;/*to mark weather same user of testpass has been counted for this tc status earlier*/
                        int old_userId = 0;/*to clear flagUserPass,flagUserFail,flagUserNC if user changes*/
                                           // dtData = dtData.AsEnumerable().Where(z => z.Field<string>("TcStatusByTester").ToUpper().Trim() != "NULL").OrderBy(Z => Z.Field<int>("userid")).OrderBy(Z => Z.Field<int>("TestCaseID")).CopyToDataTable();
                                           //dtData = dtData.AsEnumerable().Where(z => z.Field<string>("TcStatusByTester").ToUpper().Trim() != "NULL").CopyToDataTable();

                        int cntTab = listExport_DA.Count - 1;

                        for (int i = 0; i < listExport_DA.Count; i++)
                        {
                            tcid = Convert.ToInt32(listExport_DA[i].TestCaseID.ToString());
                            if (!lstTestCaseId.Contains(tcid))
                            {
                                lstTestCaseId.Add(tcid);/*counting unique testcases*/
                                TCCount++;
                                old_tcid = tcid;
                            }
                            if (listExport_DA[i].TestCaseID.ToString() == old_tcid.ToString())
                            {
                                int userId = Convert.ToInt32(listExport_DA[i].userid);
                                if (old_userId != userId)
                                {
                                    flagUserPass = 0;
                                    flagUserFail = 0;
                                    flagUserNC = 0;
                                }

                                if (!lstUserId.Contains(userId))
                                {

                                    lstUserId.Add(userId);/*counting unique users*/

                                    flagUserPass = 0;
                                    flagUserFail = 0;
                                    flagUserNC = 0;
                                }
                                if (listExport_DA[i].TcStatusByTester.ToString().ToUpper().Trim() == "PASS")
                                {
                                    if (flagUserPass == 0)
                                    {
                                        passTC++;
                                        flagUserPass++; /*to mark weather same user of testpass has been counted for this tc status earlier*/
                                    }

                                }
                                else if (listExport_DA[i].TcStatusByTester.ToString().ToUpper().Trim() == "FAIL")
                                {
                                    if (flagUserFail == 0)
                                    {
                                        FailTC++;
                                        flagUserFail++;/*to mark weather same user of testpass has been counted for this tc status earlier*/
                                    }

                                }
                                else if (listExport_DA[i].TcStatusByTester.ToString().ToUpper().Trim() == "NOTCOMPLETED")
                                {
                                    if (flagUserNC == 0)
                                    {
                                        NCTC++;
                                        flagUserNC++;/*to mark weather same user of testpass has been counted for this tc status earlier*/
                                    }

                                }

                                if (i == cntTab || listExport_DA[i + 1].TestCaseID.ToString().Trim() != listExport_DA[i].TestCaseID.ToString().Trim())
                                {
                                    listExport_DA_replica.Add(new Export_DA_replica
                                    {
                                        project = Convert.ToString(listExport_DA[i].projectName),
                                        Version = Convert.ToString(listExport_DA[i].Project_Version),
                                        TestPass = Convert.ToString(listExport_DA[i].TestPass_Name),
                                        TestCase = Convert.ToString(listExport_DA[i].testcaseName),
                                        TesterOnTc = lstUserId.Count,
                                        TesterPassTC = passTC,
                                        TesterFailTC = FailTC,
                                        TesterNCTC = NCTC
                                    });

                                    passTC = 0;
                                    FailTC = 0;
                                    NCTC = 0;
                                    flagUserPass = 0;
                                    flagUserFail = 0;
                                    flagUserNC = 0;
                                    lstUserId.RemoveRange(0, lstUserId.Count);
                                }

                                old_userId = userId;
                                old_testCaseId = tcid;
                            }
                        }
                        /************************************************************************************************/
                    }
                    var a = listExport_DA;
                    var b = listExport_DA_replica;
                    return Json(listExport_DA_replica);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //done
        [HttpGet, Route("ExportTestingTemplate/{spUserId}/{testPassId}/{roleId}")]
        public JsonResult ExportTestingTemplate(string spUserId, string testPassId, string roleId)
        {
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string retValue = "";
            string SchemaName = "";
            List<OfflineTemplate> list_OfflineTemplate = new List<OfflineTemplate>();

            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }

            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spExportTestingTemplate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@testPassId", SqlDbType.VarChar, 500) { Value = testPassId });
                    cmd.Parameters.Add(new SqlParameter("@testerSpUserId", SqlDbType.VarChar, 500) { Value = spUserId });
                    cmd.Parameters.Add(new SqlParameter("@roleId", SqlDbType.VarChar, 500) { Value = roleId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    var retObject = new List<dynamic>();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list_OfflineTemplate.Add(new OfflineTemplate
                            {
                                Project_Name = Convert.ToString(dr["Project_Name"]),
                                Project_Version = Convert.ToString(dr["Project_Version"]),
                                TestPass_Name = Convert.ToString(dr["TestPass_Name"]),
                                TestManager = Convert.ToString(dr["TestManager"]),
                                TestCase_Name = Convert.ToString(dr["TestCase_Name"]),
                                TestCase_Sequence = Convert.ToString(dr["TestCase_Sequence"]),
                                TestCase_ID = Convert.ToString(dr["TestCase_ID"]),
                                TestCase_Description = Convert.ToString(dr["TestCase_Description"]),
                                ETT = Convert.ToString(dr["ETT"]),
                                Expected_Result = Convert.ToString(dr["Expected_Result"]),
                                TestStep_ActionName = Convert.ToString(dr["TestStep_ActionName"]),
                                TestStep_Sequence = Convert.ToString(dr["TestStep_Sequence"]),
                                Actual_Result = Convert.ToString(dr["Actual_Result"]),
                                TestStep_Id = Convert.ToString(dr["TestStep_Id"]),
                                TestStepPlan_Id = Convert.ToString(dr["TestStepPlan_Id"]),
                                Role_Name = Convert.ToString(dr["Role_Name"]),
                                Status = Convert.ToString(dr["Status"]),
                                Tester_Name = Convert.ToString(dr["Tester_Name"]),
                                TestStepStatus = Convert.ToString(dr["TestStepStatus"])
                            });
                        }
                        return Json(list_OfflineTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        [HttpGet, Route("GetFile_ExpResult")]
        public FileContentResult GetFile_ExpResult(int id, string Url)
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
                    cmd.CommandText = "UAT.sp_getExpResult";
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

        [HttpGet, Route("GetFile_ActResult")]
        public FileContentResult GetFile_ActResult(int id, string Url)
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
                    cmd.CommandText = "UAT.sp_getActualResult";
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

                            //filename = dataReader["FileName"].ToString();
                            filename = "abc.png";
                            //contentType = dataReader["ContentType"].ToString();
                            contentType = "image/png";
                            //fileContent = (byte[])dataReader["FileData"];
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

        [HttpPost, Route("ImportTestingTemplate")]
        public JsonResult ImportTestingTemplate([FromBody]BulkImport_testing BulkImportArrayList)
        {
            tsDataCollection_c tsDataTable = new tsDataCollection_c();
            string outval = null;
            try
            {
                string SchemaName = "";

                string AppUrl = HttpContext.Request.Headers["appurl"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }

                var desc = "";
                var value = "";

                for (var ik = 0; ik < BulkImportArrayList.TestStep_Listobj.Count; ik++)
                {

                    int status_val = 0;
                    value =BulkImportArrayList.TestStep_Listobj[ik].s_status;
                    if (value == "Not Completed")
                    {
                        status_val = 1;
                    }
                    else if (value == "Pass")
                    {
                        status_val = 2;
                    }
                    else if (value == "Fail")
                    {
                        status_val = 3;                       
                    }

                    tsDataTable.Add(new clsTestStepIdTableDataTable_c
                    {
                        Actual_Result = BulkImportArrayList.TestStep_Listobj[ik].s_actualResult,
                        Status = status_val,
                        TestStepPlanID = Convert.ToInt32(BulkImportArrayList.TestStep_Listobj[ik].s_planId)
                    });
                }
                
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spImportTestingTemplate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@TestingStatus", SqlDbType.Structured) { Value = tsDataTable });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    int i = cmd.ExecuteNonQuery();                 
                    outval = (string)cmd.Parameters["@Ret_Parameter"].Value;
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(outval);
        }
    }
}






