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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;
using Microsoft.SqlServer.Server;
using System.Runtime.Serialization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{

    public class Import_TestPass
    {

        public int Project_Id { get; set; }
        public string TestPass_Name { get; set; }
        public string TestPass_Description { get; set; }
        public string TestMgrEmail { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
    }

    public class TestPass_DataCol : List<Import_TestPass>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("Project_Id", SqlDbType.Int),
                new SqlMetaData("TestPass_Name", SqlDbType.VarChar, 500),
                new SqlMetaData("TestPass_Description", SqlDbType.NVarChar, 4000),
                new SqlMetaData("TestMgrEmail", SqlDbType.NVarChar, 4000),
                new SqlMetaData("Start_Date", SqlDbType.NVarChar, 500),
                new SqlMetaData("End_Date", SqlDbType.NVarChar, 500)
                );

            foreach (Import_TestPass data in this)
            {
                ret.SetInt32(0, data.Project_Id);
                ret.SetString(1, string.IsNullOrEmpty(data.TestPass_Name) ? "" : data.TestPass_Name);
                ret.SetString(2, string.IsNullOrEmpty(data.TestPass_Description) ? "" : data.TestPass_Description);
                ret.SetString(3, string.IsNullOrEmpty(data.TestMgrEmail) ? "" : data.TestMgrEmail);
                ret.SetString(4, data.Start_Date);
                ret.SetString(5, data.End_Date);
                yield return ret;
            }
        }
    }

    public class Import_Tester
    {
        public int Project_Id { get; set; }
        public string TestPass_Name { get; set; }
        public string TesterEmail { get; set; }
        public string RoleName { get; set; }
        public string Area { get; set; }
    }

    public class Tester_DataCol : List<Import_Tester>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("Project_Id", SqlDbType.Int),
                new SqlMetaData("TestPass_Name", SqlDbType.VarChar, 500),
                new SqlMetaData("TesterEmail", SqlDbType.VarChar, 200),
                new SqlMetaData("RoleName", SqlDbType.VarChar, 500),
                new SqlMetaData("Area", SqlDbType.VarChar, 500)
                );

            foreach (Import_Tester data in this)
            {
                ret.SetInt32(0, data.Project_Id);
                ret.SetString(1, string.IsNullOrEmpty(data.TestPass_Name) ? "" : data.TestPass_Name);
                ret.SetString(2, string.IsNullOrEmpty(data.TesterEmail) ? "" : data.TesterEmail);
                ret.SetString(3, string.IsNullOrEmpty(data.RoleName) ? "" : data.RoleName);
                ret.SetString(4, string.IsNullOrEmpty(data.Area) ? "" : data.Area);
                yield return ret;
            }
        }
    }

    public class Import_TestCase
    {
        public int Project_Id { get; set; }
        public string TestPass_Name { get; set; }
        public string TestCase_Name { get; set; }
        public string TestCase_Description { get; set; }
        public int ETT { get; set; }
    }

    public class TestCase_DataCol : List<Import_TestCase>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("TestPass_Name", SqlDbType.VarChar, 500),
                new SqlMetaData("TestCase_Name", SqlDbType.NVarChar, 4000),
                new SqlMetaData("TestCase_Description", SqlDbType.VarChar, 4000),
                new SqlMetaData("ETT", SqlDbType.Int),
                new SqlMetaData("Project_Id", SqlDbType.Int)
                );

            foreach (Import_TestCase data in this)
            {
                ret.SetString(0, string.IsNullOrEmpty(data.TestPass_Name) ? "" : data.TestPass_Name);
                ret.SetString(1, string.IsNullOrEmpty(data.TestCase_Name) ? "" : data.TestCase_Name);
                ret.SetString(2, string.IsNullOrEmpty(data.TestCase_Description) ? "" : data.TestCase_Description);
                ret.SetInt32(3, data.ETT);
                ret.SetInt32(4, data.Project_Id);
                yield return ret;
            }
        }
    }

    public class Import_TestStep
    {
        public int Project_Id { get; set; }
        public string TestPass_Name { get; set; }
        public string TestCase_Name { get; set; }
        public string TestStep_Name { get; set; }
        public string Role { get; set; }
        public string Expected_Result { get; set; }
        public string Expected_Result_Image { get; set; }
    }

    public class TestStep_DataCol : List<Import_TestStep>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("TestPass_Name", SqlDbType.NVarChar, 4000),
                new SqlMetaData("TestCase_Name", SqlDbType.NVarChar, 4000),
                new SqlMetaData("TestStep_Name", SqlDbType.NVarChar, 4000),
                new SqlMetaData("Role", SqlDbType.NVarChar, 4000),
                new SqlMetaData("Expected_Result", SqlDbType.NVarChar, -1),
                new SqlMetaData("Project_Id", SqlDbType.Int),
                new SqlMetaData("Expected_Result_Image", SqlDbType.NVarChar, 4000)
              );

            foreach (Import_TestStep data in this)
            {
                ret.SetString(0, string.IsNullOrEmpty(data.TestPass_Name) ? "" : data.TestPass_Name);
                ret.SetString(1, string.IsNullOrEmpty(data.TestCase_Name)? "": data.TestCase_Name);
                ret.SetString(2, string.IsNullOrEmpty(data.TestStep_Name)? "" : data.TestStep_Name);
                ret.SetString(3, string.IsNullOrEmpty(data.Role)? "" :data.Role);
                ret.SetString(4, string.IsNullOrEmpty(data.Expected_Result)? "" : data.Expected_Result);
                ret.SetInt32(5,  data.Project_Id );
                ret.SetString(6, string.IsNullOrEmpty(data.Expected_Result_Image) ? "" : data.Expected_Result_Image);
                yield return ret;
            }
        }
    }

    public class BulkImport
    {
        public List<Import_TestPass> TestPass_Listobj { get; set; }
        public List<Import_Tester> Tester_Listobj { get; set; }
        public List<Import_TestCase> TestCase_Listobj { get; set; }
        public List<Import_TestStep> TestStep_Listobj { get; set; }
    }

    [Route("api/[controller]")]
    public class BulkDownloadImportTemplateController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";

        private clsDbContext _context;
        private IHostingEnvironment _env;

        public BulkDownloadImportTemplateController(clsDbContext context)
        {
            _context = context;

        }

        //public BulkDownloadImportTemplateController(IHostingEnvironment env)
        //{
        //    _env = env;
        //}
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
              

        [HttpGet, Route("DownloadBulkImportTemplate/{projectId}")]
        public JsonResult DownloadBulkImportTemplate(string projectId)
        {
            var retObject = new List<dynamic>();
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

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spDownloadBulkTemplate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId });
                    cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 20) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 20) { Value = "Download" });
                    cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

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
                }



            }
            catch (Exception ex)
            {

            }
            return Json(retObject);
        }

        [HttpPost, Route("ImportBulkImportTemplate")]
        public JsonResult ImportBulkImportTemplate([FromBody]BulkImport BulkImportArrayList)
        {
            int projectId = 0;
            var retObject = new List<dynamic>();
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
                //  return Json("Invalid Url");
            }
            string _clientSchemaName = SchemaName;

            if (string.IsNullOrEmpty(_clientSchemaName)) { }
            // return null;


            /*
            #region To delete the Project data before importing new values

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.spDownloadBulkTemplate";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId });
                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 20) { Value = _clientSchemaName });
                cmd.Parameters.Add(new SqlParameter("@StatementType", SqlDbType.NVarChar, 20) { Value = "Delete" });
                cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                int i = cmd.ExecuteNonQuery();
                var ou= cmd.Parameters["@Ret_Parameter"];
                //using (var dataReader = cmd.ExecuteReader())
                //{

                //}
            }

            #endregion
        */
            if (BulkImportArrayList != null)
            {
                #region "Convert List to Data Table"()
                TestPass_DataCol testPassDataTable = new TestPass_DataCol();
                if (BulkImportArrayList.TestPass_Listobj.Count == 0)
                {
                    _result.Add(this._errorText, "Test Pass Details is required");
                    //  return _result;
                }

                foreach (Import_TestPass importTestPass in BulkImportArrayList.TestPass_Listobj)
                {
                        projectId = importTestPass.Project_Id;
                        testPassDataTable.Add(new Import_TestPass
                        {
                            Project_Id = importTestPass.Project_Id,
                            TestPass_Name = string.IsNullOrEmpty(importTestPass.TestPass_Name) ? "" : importTestPass.TestPass_Name,
                            TestPass_Description = string.IsNullOrEmpty(importTestPass.TestPass_Description) ? "" : importTestPass.TestPass_Description,
                            TestMgrEmail = string.IsNullOrEmpty(importTestPass.TestMgrEmail) ? "" : importTestPass.TestMgrEmail,
                            Start_Date = importTestPass.Start_Date,
                            End_Date = importTestPass.End_Date
                        });              
                }

                Tester_DataCol testerDataTable = new Tester_DataCol();
                if (BulkImportArrayList.Tester_Listobj.Count == 0)
                {
                    _result.Add(this._errorText, "Tester Details is required");
                }

                foreach (Import_Tester importTester in BulkImportArrayList.Tester_Listobj)
                {                   
                        testerDataTable.Add(new Import_Tester
                        {
                            Project_Id = importTester.Project_Id,
                            TestPass_Name = string.IsNullOrEmpty(importTester.TestPass_Name) ? "" : importTester.TestPass_Name,
                            TesterEmail =  string.IsNullOrEmpty(importTester.TesterEmail) ? "": importTester.TesterEmail,
                            RoleName = string.IsNullOrEmpty(importTester.RoleName) ? "" : importTester.RoleName,
                            Area = string.IsNullOrEmpty(importTester.Area) ? "" : importTester.Area
                        });
                }

                TestCase_DataCol testCaseDataTable = new TestCase_DataCol();
                if (BulkImportArrayList.TestCase_Listobj.Count == 0)
                {
                    _result.Add(this._errorText, "Test Case Details is required");
                }

                foreach (Import_TestCase importTestCase in BulkImportArrayList.TestCase_Listobj)
                {                    
                        testCaseDataTable.Add(new Import_TestCase
                        {
                            Project_Id = importTestCase.Project_Id,
                            TestPass_Name = string.IsNullOrEmpty(importTestCase.TestPass_Name) ? "": importTestCase.TestPass_Name,
                            TestCase_Name = string.IsNullOrEmpty(importTestCase.TestCase_Name) ? "" : importTestCase.TestCase_Name,
                            TestCase_Description = string.IsNullOrEmpty(importTestCase.TestCase_Description) ? "": importTestCase.TestCase_Description,
                            ETT = importTestCase.ETT
                        });
                   
                }

                TestStep_DataCol testStepDataTable = new TestStep_DataCol();
                if (BulkImportArrayList.TestStep_Listobj.Count == 0)
                {
                    _result.Add(this._errorText, "Test Step Details is required");
                }

                foreach (Import_TestStep importTestStep in BulkImportArrayList.TestStep_Listobj)
                {                  
                        testStepDataTable.Add(new Import_TestStep
                        {
                            Project_Id = importTestStep.Project_Id,
                            TestPass_Name = string.IsNullOrEmpty(importTestStep.TestPass_Name) ? "": importTestStep.TestPass_Name,
                            TestCase_Name = string.IsNullOrEmpty(importTestStep.TestCase_Name) ? "" : importTestStep.TestCase_Name,
                            TestStep_Name = string.IsNullOrEmpty(importTestStep.TestStep_Name) ? "" : importTestStep.TestStep_Name,
                            Role = string.IsNullOrEmpty(importTestStep.Role) ? "" : importTestStep.Role,
                            Expected_Result = string.IsNullOrEmpty(importTestStep.Expected_Result) ? "" : importTestStep.Expected_Result,                           
                            Expected_Result_Image = string.IsNullOrEmpty(importTestStep.Expected_Result_Image) ? "" : importTestStep.Expected_Result_Image
                        });                   
                }

                #endregion
                                

                #region "Save all data to Database"
                if (projectId != 0)
                {
                    using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "UAT.spUploadBulkTemplate";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId });
                        cmd.Parameters.Add(new SqlParameter("@TestPassTable", SqlDbType.Structured) { Value = testPassDataTable });
                        cmd.Parameters.Add(new SqlParameter("@TesterTable", SqlDbType.Structured) { Value = testerDataTable });
                        cmd.Parameters.Add(new SqlParameter("@TestCaseTable", SqlDbType.Structured) { Value = testCaseDataTable });
                        cmd.Parameters.Add(new SqlParameter("@TestStepTable", SqlDbType.Structured) { Value = testStepDataTable });
                        cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.NVarChar, 20) { Value = _clientSchemaName });
                        cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();
                        // int i = cmd.ExecuteNonQuery();
                        // var outval = cmd.Parameters["@Ret_Parameter"];

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
                    }
                }
                #endregion "End of Save Database"

            }

            return Json(retObject);
        }



    }
}

