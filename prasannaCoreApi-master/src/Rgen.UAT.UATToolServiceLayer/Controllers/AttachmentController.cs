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
using System.Net.Http;

//comment
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class AttachmentController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";

        private clsDbContext _context;

        List<data_coll> listdata_coll = new List<data_coll>();

        public AttachmentController(clsDbContext context)
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

        [HttpGet, Route("GetTestCaseDetailForTestPassIdWithTesterFlag/{testPassId}")]
        public JsonResult GetTestCaseDetailForTestPassIdWithTesterFlag(string testPassId)
        {
            List<TestCase_New> listTestCaseDetails = new List<TestCase_New>();
            int _res = 0;
            string status = string.Empty;
            string statementType = string.Empty;

            /**************************************************************************************************/
            /**************************************************************************************************/
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
            Dictionary<string, string> oError = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(testPassId))
            {

                oError.Add("ERROR", "Test Pass ID is required");
                return Json(oError);
            }

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.SpTestCase";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 10) { Value = "Select" });
                SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outparam);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                _res = cmd.ExecuteNonQuery();

                var retObject = new List<dynamic>();

                var OutResult = outparam.Value;

                string flagTester = "";
                string retValue = "";

                retValue = OutResult.ToString();

                if (retValue == "SUCCESS")
                {
                    flagTester = "y";
                }
                else if (retValue == "No Tester(s) Assigned!")
                {
                    flagTester = "n";
                }

                using (var dataReader = cmd.ExecuteReader())
                {
                    int TestCase_IdOrdinal = dataReader.GetOrdinal("TestCase_Id");
                    int TestPass_IdOrdinal = dataReader.GetOrdinal("TestPass_Id");
                    int DisplayTestCase_IdOrdinal = dataReader.GetOrdinal("DisplayTestCase_Id");
                    int TestCase_NameOrdinal = dataReader.GetOrdinal("TestCase_Name");
                    int TestCase_DescriptionOrdinal = dataReader.GetOrdinal("TestCase_Description");
                    int Testcase_SequenceOrdinal = dataReader.GetOrdinal("Testcase_Sequence");
                    int ETTOrdinal = dataReader.GetOrdinal("ETT");

                    if (dataReader.HasRows != false)
                    {
                        while (dataReader.Read())
                        {
                            listTestCaseDetails.Add(new TestCase_New
                            {
                                TestCase_Id = (dataReader.IsDBNull(TestCase_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_IdOrdinal]),
                                TestPass_Id = (dataReader.IsDBNull(TestPass_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[TestPass_IdOrdinal]),
                                DisplayTestCase_Id = (dataReader.IsDBNull(DisplayTestCase_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[DisplayTestCase_IdOrdinal]),
                                TestCase_Name = (dataReader.IsDBNull(TestCase_NameOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_NameOrdinal]),
                                TestCase_Description = (dataReader.IsDBNull(TestCase_DescriptionOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_DescriptionOrdinal]),
                                Testcase_Sequence = (dataReader.IsDBNull(Testcase_SequenceOrdinal)) == true ? "" : Convert.ToString(dataReader[Testcase_SequenceOrdinal]),
                                testcaseETT = (dataReader.IsDBNull(ETTOrdinal)) == true ? "" : Convert.ToString(dataReader[ETTOrdinal]),
                                testcaseflagTester = flagTester
                            });
                        }
                    }
                    else
                    {
                        listTestCaseDetails.Add(new TestCase_New
                        {
                            TestCase_Id = "",
                            TestPass_Id = "",
                            DisplayTestCase_Id = "",
                            TestCase_Name = "",
                            TestCase_Description = "",
                            Testcase_Sequence = "",
                            testcaseETT = "",
                            testcaseflagTester = flagTester
                        });
                    }

                }
                return Json(listTestCaseDetails);

            }
        }

        [HttpGet, Route("GetTestCaseDetailForTestPassId/{testPassId}")]
        public JsonResult GetTestCaseDetailForTestPassId(string testPassId)
        {
            List<TestCase_New> listTestCaseDetails = new List<TestCase_New>();
            int _res = 0;
            string status = string.Empty;
            string statementType = string.Empty;

            /**************************************************************************************************/
            /**************************************************************************************************/
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
            Dictionary<string, string> oError = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(testPassId))
            {

                oError.Add("ERROR", "Test Pass ID is required");
                return Json(oError);
            }

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.SpTestCase";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 10) { Value = "Select" });
                SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outparam);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                _res = cmd.ExecuteNonQuery();

                var retObject = new List<dynamic>();

                var OutResult = outparam.Value;

                string flagTester = "";

                using (var dataReader = cmd.ExecuteReader())
                {
                    int TestCase_IdOrdinal = dataReader.GetOrdinal("TestCase_Id");
                    int TestPass_IdOrdinal = dataReader.GetOrdinal("TestPass_Id");
                    int DisplayTestCase_IdOrdinal = dataReader.GetOrdinal("DisplayTestCase_Id");
                    int TestCase_NameOrdinal = dataReader.GetOrdinal("TestCase_Name");
                    int TestCase_DescriptionOrdinal = dataReader.GetOrdinal("TestCase_Description");
                    int Testcase_SequenceOrdinal = dataReader.GetOrdinal("Testcase_Sequence");
                    int ETTOrdinal = dataReader.GetOrdinal("ETT");

                    if (dataReader.HasRows != false)
                    {
                        while (dataReader.Read())
                        {
                            listTestCaseDetails.Add(new TestCase_New
                            {
                                TestCase_Id = (dataReader.IsDBNull(TestCase_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_IdOrdinal]),
                                TestPass_Id = (dataReader.IsDBNull(TestPass_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[TestPass_IdOrdinal]),
                                DisplayTestCase_Id = (dataReader.IsDBNull(DisplayTestCase_IdOrdinal)) == true ? "" : Convert.ToString(dataReader[DisplayTestCase_IdOrdinal]),
                                TestCase_Name = (dataReader.IsDBNull(TestCase_NameOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_NameOrdinal]),
                                TestCase_Description = (dataReader.IsDBNull(TestCase_DescriptionOrdinal)) == true ? "" : Convert.ToString(dataReader[TestCase_DescriptionOrdinal]),
                                Testcase_Sequence = (dataReader.IsDBNull(Testcase_SequenceOrdinal)) == true ? "" : Convert.ToString(dataReader[Testcase_SequenceOrdinal]),
                                testcaseETT = (dataReader.IsDBNull(ETTOrdinal)) == true ? "" : Convert.ToString(dataReader[ETTOrdinal]),
                                testcaseflagTester = flagTester
                            });
                        }
                    }
                    else
                    {
                        //no data found
                    }

                }
            }
            return Json(listTestCaseDetails);
        }

        [HttpGet, Route("GetTestStepsByTestPassID/{testPassId}")]
        public JsonResult GetTestStepsByTestPassID(string testPassId)
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

            if (string.IsNullOrEmpty(testPassId))
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add("ERROR", "Test Pass ID is required");
                return Json(oError);
            }

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.spGetTestStepsByTestPassIds";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                cmd.Parameters.Add(new SqlParameter(this._returnParameter, SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

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

        [HttpPost, Route("senddata")]
        public JsonResult senddata([FromBody]Attachment_dat attachment)
        {

            /********************/
            string AppUrl = HttpContext.Request.Headers["appurl"];
            Dictionary<string, string> _result = new Dictionary<string, string>();
            string SchemaName = "";
            int outval;
            bool isUpload = false;
            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                isUpload = false;
            }

            string _clientSchemaName = SchemaName;

            if (string.IsNullOrEmpty(_clientSchemaName))
                return null;



            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.spUpdateFileDataInAttachment_Core";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TestStepID", SqlDbType.VarChar, 500) { Value = attachment.TestStepID });
                cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int) { Value = attachment.AttachmentId });
                cmd.Parameters.Add(new SqlParameter("@AttachmentName", SqlDbType.VarChar, 500) { Value = attachment.AttachmentName });
                cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 500) { Value = attachment.Description });
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                cmd.Parameters.Add(new SqlParameter("@ResultType", SqlDbType.VarChar, 500) { Value = attachment.ResultType });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = attachment.StatementType });
                cmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.VarChar, 500) { Value = attachment.ProjectID });
                cmd.Parameters.Add(new SqlParameter("@TestPassID", SqlDbType.VarChar, 500) { Value = attachment.TestPassID });
                cmd.Parameters.Add(new SqlParameter("@TestCaseID", SqlDbType.VarChar, 500) { Value = attachment.TestCaseID });

                SqlParameter outparam = new SqlParameter("@outval", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outparam);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                int _res = cmd.ExecuteNonQuery();
                // bool outval = (bool)cmd.Parameters["@outval"].Value;
                outval = Convert.ToInt32(cmd.Parameters["@outval"].Value);
                if (_res != 0)
                {
                    isUpload = true;
                }
            }
            return Json(outval);
        }

        [HttpPost, Route("UploadFile")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public IActionResult UploadFile()
        {
            //string AppUrl = HttpContext.Request.Headers["appurl"];
            string AppUrl = HttpContext.Request.Headers["appurl"];
            string SchemaName = "";
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];
            bool isUpload = false;

            if (!string.IsNullOrEmpty(AppUrl))
            {
                SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
            }
            else
            {
                isUpload = false;

            }

            /*test Code 1*/
            byte[] byt;
            byte[] fileRecord;
            using (var reader = new StreamReader(HttpContext.Request.Form.Files["file"].OpenReadStream()))
            {
                var fileContent = reader.ReadToEnd();

                ////BinaryReader br = new BinaryReader(HttpContext.Request.Form.Files["file"].OpenReadStream());
                ////byt = br.ReadBytes((Int32)fileContent.Length);

                Stream file_strm = Request.Form.Files["file"].OpenReadStream();
                int fileSize = Convert.ToInt32(Request.Form.Files["file"].Length);
                fileRecord = new byte[fileSize];
                file_strm.Read(fileRecord, 0, fileSize);


            }
            /*end*/

            int new_AttachmentId = Convert.ToInt32(HttpContext.Request.Form["attaid"]);
            string new_AttachmentName = HttpContext.Request.Form["fileName"];

            //var stream = HttpContext.Request.Form.Files["file"];
            ///*Test Code*/
            //var uploads = Path.Combine(_env.WebRootPath, "upload");
            //using (var fileStream = new FileStream(Path.Combine(uploads, stream.FileName), FileMode.Create))
            //{

            //    stream.CopyTo(fileStream);
            //}
            ///*END*/

            //var name = stream.FileName;
            //int Length = Convert.ToInt32(HttpContext.Request.ContentLength);
            //byte[] bytes = new byte[Length];
            //stream.OpenReadStream().Read(bytes, 0, Length);

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.spUploadFile";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 500) { Value = new_AttachmentName });
                cmd.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.VarChar, 500) { Value = HttpContext.Request.Form.Files["file"].ContentType });
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                cmd.Parameters.Add(new SqlParameter("@AttachmentID", SqlDbType.Int) { Value = new_AttachmentId });
                cmd.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary, 5000000) { Value = fileRecord });//byt
                cmd.Parameters.Add(new SqlParameter("@outval", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                int i = cmd.ExecuteNonQuery();
                bool outval = (bool)cmd.Parameters["@outval"].Value;

                if (i != 0 && outval != false)
                {
                    isUpload = true;
                }
            }
            return Json(isUpload);

        }

        [HttpPost, Route("InsertUpdateAttachment")]
        public JsonResult InsertUpdateAttachment([FromBody]Attachment_append attachment)
        {
            //string testStepId = null, string testStepPlanId = null, string fileIndex = null


            string AppUrl = HttpContext.Request.Headers["appurl"];
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            Dictionary<string, string> _result = new Dictionary<string, string>();

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


            List<ProjectUser> listProjectUsers = new List<ProjectUser>();
            string fileType = string.Empty;
            //attachment.testStepId = null;
            //attachment.TestStepPlanID = 0;
            //attachment.FileIndex = 0;

            //if (string.IsNullOrEmpty(attachment.testStepId) && string.IsNullOrEmpty(attachment.TestStepPlanID))
            //{
            //    //send service level Exception as service response
            //    _result.Add(this._errorText, "testStepId/testStepPlanId is required for Expected/Actual File");
            //    return Json(_result);
            //}
            //else if (string.IsNullOrEmpty(attachment.fileName) && !(attachment.isDelete))
            //{
            //    _result.Add(this._errorText, "fileName is required");

            //    return Json(_result);
            //}
            //else if (string.IsNullOrEmpty(attachment.fileUrl) && !(attachment.isDelete))
            //{
            //    _result.Add(this._errorText,"fileUrl is required");
            //    return Json(_result);
            //}
            //else if (string.IsNullOrEmpty(attachment.fileType))
            //{
            //    _result.Add(this._errorText, "fileType is required");
            //    return Json(_result);
            //}

            List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                /************************************/

                tsDataCollection tsDataTable = new tsDataCollection();
                if (!string.IsNullOrEmpty(attachment.testStepId))
                {
                    int ik = 0;
                    if (attachment.testStepId.IndexOf(',') != -1)// if (attachment.testStepId.Length > 1)
                    {
                        string[] testStepIds = attachment.testStepId.Split(',');

                        foreach (string tsId in testStepIds)
                        {
                            tsDataTable.Add(new clsTestStepIdTableDataTable
                            {
                                RowID = Convert.ToInt32(ik + 1),
                                TestStep_ID = Convert.ToInt32(testStepIds[ik])//Convert.ToInt32(testStepIds)

                            });
                            ik++;
                        }

                    }
                    else
                    {

                        string testStepIds = attachment.testStepId;


                        tsDataTable.Add(new clsTestStepIdTableDataTable
                        {
                            RowID = Convert.ToInt32(1),
                            TestStep_ID = Convert.ToInt32(testStepIds)//Convert.ToInt32(testStepIds)
                        });

                    }
                }

                /*********************************/
                cmd.CommandText = "UAT.spAttachmentInsUpd_Core_1";
                cmd.CommandType = CommandType.StoredProcedure;

                //  if (!string.IsNullOrEmpty(attachment.TestStepPlanID))
                cmd.Parameters.Add(new SqlParameter("@TestStepPlanID", SqlDbType.Int) { Value = attachment.TestStepPlanID });

                // if (!string.IsNullOrEmpty(attachment.FileIndex))
                cmd.Parameters.Add(new SqlParameter("@FileIndex", SqlDbType.Int) { Value = attachment.FileIndex });

                cmd.Parameters.Add(new SqlParameter("@attID", SqlDbType.Int) { Value = attachment.attID });
                cmd.Parameters.Add(new SqlParameter("@isDelete", SqlDbType.VarChar) { Value = attachment.isDelete });
                cmd.Parameters.Add(new SqlParameter("@FileType", SqlDbType.VarChar, 500) { Value = attachment.fileType.Trim() });
                cmd.Parameters.Add(new SqlParameter("@FileUrl", SqlDbType.VarChar, 500) { Value = attachment.fileUrl });
                cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 500) { Value = attachment.fileName });
                cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 500) { Value = attachment.Description });
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 500) { Value = SchemaName });
                cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = _SpUserId });
                cmd.Parameters.Add(new SqlParameter("@TestStepIDs", SqlDbType.Structured) { Value = tsDataTable });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.VarChar, 500) { Value = attachment.StatementType });
                cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                int i = cmd.ExecuteNonQuery();
                //bool outval = (bool)cmd.Parameters["@Ret_Parameter"].Value;
                string outval = (string)cmd.Parameters["@Ret_Parameter"].Value;

                //if (i != 0 && outval != false)
                //{
                //    _result.Add("Status", parameter[parameter.Count - 1].Value.ToString());
                //}

            }

            return Json(_result);
        }

        [HttpGet, Route("GetDescAttachment/{attachID}")]
        public JsonResult GetDescAttachment(string attachID)
        {
            //[spGetAttachmentDesc]
            try
            {
                string SchemaName = "";

                string AppUrl = HttpContext.Request.Headers["appurl"];

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
               
                var desc = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetAttachmentDesc";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = attachID });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            desc = dataReader["Description"] == null? null: dataReader["Description"].ToString();
                        }
                        return Json(desc);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }          
        }

        [HttpGet, Route("GetAllAttachment/{testPassId}")]
        public JsonResult GetAllAttachment(string testPassId)
        {
            List<AttachmentData> listAttachmentData = new List<AttachmentData>();
          

            string AppUrl = HttpContext.Request.Headers["appurl"];
            string _SpUserId = HttpContext.Request.Headers["LoggedInUserSPUserId"];

            Dictionary<string, string> _result = new Dictionary<string, string>();

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

            List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();

            /***************************************************************************************/
            using (var cmdTP = _context.Database.GetDbConnection().CreateCommand())
            {
                cmdTP.CommandText = "UAT.spGetAttachment_ForTP";
                cmdTP.CommandType = CommandType.StoredProcedure;

                cmdTP.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                cmdTP.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                cmdTP.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmdTP.Connection.State != ConnectionState.Open)
                    cmdTP.Connection.Open();
                try
                {
                    using (var drr = cmdTP.ExecuteReader())
                    {
                        if (drr.HasRows != false)
                        {
                            while (drr.Read())
                            {
                                listdata_coll.Add(new data_coll()
                                {
                                    col_attachmentId=Convert.ToInt32(drr["AttachmentId"]),
                                    col_testStepId= drr["TestStepID"].ToString(),
                                    col_testCaseId = Convert.ToInt32(drr["TestCaseID"])                                
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var R = ex;
                }
            }
                        /***************************************************************************************/


                        /*******************************/
                        using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.spGetAttachment_core1";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@TestPassId", SqlDbType.VarChar) { Value = testPassId });
                cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                var retObject = new List<dynamic>();

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr == null || dr.FieldCount == 0 || dr.HasRows == false)
                    {
                        Dictionary<string, string> oError = new Dictionary<string, string>();
                        oError.Add(this._errorText, "No Data Found");
                        return Json(oError);
                    }
                    else
                    {
                        int _ER_Attachment_Name = dr.GetOrdinal("ER_Attachment_Name");
                        int _ER_Attachment_URL = dr.GetOrdinal("ER_Attachment_URL");
                        int _TestCase_ID = dr.GetOrdinal("TestCase_ID");
                        int _TestStep_ID = dr.GetOrdinal("TestStep_ID");
                        //int _a_TestStep_ID = dr.GetOrdinal("TestStepID");
                        int _TestStep_ActionName = dr.GetOrdinal("TestStep_ActionName");
                        int _testerSpUserId = dr.GetOrdinal("testerSpUserId");
                        int _TesterName = dr.GetOrdinal("TesterName");
                        int _Role_ID = dr.GetOrdinal("Role_ID");
                        int _Role_Name = dr.GetOrdinal("Role_Name");
                        int _TestStepPlan_Id = dr.GetOrdinal("TestStepPlan_Id");
                        int _AR_Attachment1_Name = dr.GetOrdinal("AR_Attachment1_Name");
                        int _AR_Attachment1_URL = dr.GetOrdinal("AR_Attachment1_URL");
                        int _AR_Attachment2_Name = dr.GetOrdinal("AR_Attachment2_Name");
                        int _AR_Attachment2_URL = dr.GetOrdinal("AR_Attachment2_URL");
                        int _AR_Attachment3_Name = dr.GetOrdinal("AR_Attachment3_Name");
                        int _AR_Attachment3_URL = dr.GetOrdinal("AR_Attachment3_URL");
                        int _AttachmentID = dr.GetOrdinal("AttachmentId");
                        //int _Description = dr.GetOrdinal("Description");
                        //   lstEnv = new List<AttachmentData>();

                        while (dr.Read())
                        {
                            List<Attachment> listExpectedAttach = new List<Attachment>();
                            List<Attachment> listActualAttach = new List<Attachment>();
                            if (dr.IsDBNull(_ER_Attachment_Name) != true)
                            {
                                listExpectedAttach.Add(new Attachment()
                                {
                                    fileName = Convert.ToString(dr["ER_Attachment_Name"]),
                                    AttachmentId= getAttaId((dr.IsDBNull(_TestStep_ID)) == true ? "" : Convert.ToString(dr[_TestStep_ID]))
                                });
                            }

                            if (listExpectedAttach.Count > 0 || listActualAttach.Count > 0)
                            {
                                listAttachmentData.Add(new AttachmentData()
                                {
                                    testCaseId = (dr.IsDBNull(_TestCase_ID)) == true ? "" : Convert.ToString(dr[_TestCase_ID]),
                                    testStepId =(dr.IsDBNull(_TestStep_ID)) == true ? "" : Convert.ToString(dr[_TestStep_ID]),
                                    testStepName = (dr.IsDBNull(_TestStep_ActionName)) == true ? "" : Convert.ToString(dr[_TestStep_ActionName]),
                                    testerSpUserId = (dr.IsDBNull(_testerSpUserId)) == true ? "" : Convert.ToString(dr[_testerSpUserId]),
                                    testerName = (dr.IsDBNull(_TesterName)) == true ? "" : Convert.ToString(dr[_TesterName]),
                                    roleId = (dr.IsDBNull(_Role_ID)) == true ? "" : Convert.ToString(dr[_Role_ID]),
                                    roleName = (dr.IsDBNull(_Role_Name)) == true ? "" : Convert.ToString(dr[_Role_Name]),
                                    testStepPlanId = (dr.IsDBNull(_TestStepPlan_Id)) == true ? "" : Convert.ToString(dr[_TestStepPlan_Id]),
                                    AttachmentID = getAttaId((dr.IsDBNull(_TestStep_ID)) == true ? "" : Convert.ToString(dr[_TestStep_ID])), //(dr.IsDBNull(_AttachmentID)) == true ? "" : Convert.ToString(dr[_AttachmentID]),
                                    listExpectedAttach = listExpectedAttach,
                                    listActualAttach = listActualAttach,
                                    //Description= (dr.IsDBNull(_Description)) == true ? "" : Convert.ToString(dr[_Description]),
                                });
                            }
                        }
                    }
                    return Json(listAttachmentData);
                }
            }
        }

        public string getAttaId(string testStepId)
        {
            //don
            var f_attachmentID = "";

            for (var t = 0; t < listdata_coll.Count; t++)
            {
                if (listdata_coll[t].col_testStepId.IndexOf('|') != -1)
                {
                    string[] testStepIds = listdata_coll[t].col_testStepId.Split('|');
                    for (var h = 0; h < testStepIds.Length; h++)
                    {
                        if(testStepIds[h]== testStepId)
                        {
                            f_attachmentID = listdata_coll[t].col_attachmentId.ToString();
                        }
                    }
                }
            }
            return f_attachmentID;
        }

        [HttpGet, Route("GetFileToPreview")]
        //public byte[] GetFileToDownload(int id)
        public JsonResult GetFileToPreview(int id)
        {

            byte[] fileContent = new byte[0];
            string base64String = "";
            Attachment_view obj = new Attachment_view();

            try
            {
                string SchemaName = "";

                string AppUrl = HttpContext.Request.Headers["appurl"]; 

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {

                }

                var filename = "";
                var contentType = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetAttachmentToDownload";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = id });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["FileName"].ToString();
                            contentType = dataReader["ContentType"].ToString();
                            fileContent = (byte[])dataReader["FileData"];

                            base64String = Convert.ToBase64String(fileContent, 0, fileContent.Length);
                            Response.ContentType = contentType;
                            Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["FileName"].ToString());
                            //Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                            Response.WriteAsync("data:" + contentType + ";base64," + base64String);
                        }
                        obj.fileData = "data:" + contentType + ";base64," + base64String;
                        return Json(obj);

                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }
            //return Json(base64String);
            return Json(obj);
        }

        [HttpGet, Route("GetFileToDownload")]
        //public byte[] GetFileToDownload(int id)
        public FileContentResult GetFileToDownload(int id, string Url)
        {

            byte[] fileContent = new byte[0];
            string base64String = "";
            Attachment_view obj = new Attachment_view();

            try
            {
                string SchemaName = "";

                string AppUrl = Url; 

                if (!string.IsNullOrEmpty(AppUrl))
                {
                    SchemaName = new clsUatClient(_context).GetClientSchema(AppUrl);
                }
                else
                {

                }

                var filename = "";
                var contentType = "";

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spGetAttachmentToDownload";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 500) { Value = SchemaName });
                    cmd.Parameters.Add(new SqlParameter("@AttachmentId", SqlDbType.Int, 15) { Value = id });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            filename = dataReader["FileName"].ToString();
                            contentType = dataReader["ContentType"].ToString();
                            fileContent = (byte[])dataReader["FileData"];

                            base64String = Convert.ToBase64String(fileContent, 0, fileContent.Length);
                            Response.ContentType = contentType;
                            Response.Headers.Add("content-disposition", "inline;filename=" + dataReader["FileName"].ToString());//fordownload directly//Response.Headers.Add("content-disposition", "attachment;filename=" + dataReader["FileName"].ToString());

                            Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
                            // Response.WriteAsync("data:" + contentType + ";base64," + base64String);
                        }
                        //obj.fileData = "data:" + contentType + ";base64," + base64String;
                        //return Json(obj);
                        return File(fileContent, contentType, filename);
                    }

                }
            }
            catch (Exception ex)
            {

                return null;
            }
            //return Json(base64String);
            // return fileContent;
        }
    }
}

