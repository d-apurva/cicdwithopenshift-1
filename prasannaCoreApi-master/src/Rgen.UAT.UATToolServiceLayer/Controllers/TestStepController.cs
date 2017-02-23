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

/*Adding to BitBucket */

namespace Rgen.UAT.UATToolServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class TestStepController : Controller
    {
        private string _schemaNameParameterName = "@SchemaName";
        private string _returnParameter = "@Ret_Parameter";
        private string _statementTypeParameterName = "@StatementType";
        private string _statusText = "Success";
        private string _errorText = "ErrorDetails";

        private clsDbContext _context;
        public TestStepController(clsDbContext context)
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

        [HttpGet, Route("GetTestStepsByTestPassID/{testPassId}")]
        public JsonResult GetTestStepsByTestPassID(string testPassId)
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

                string _clientSchemaName = SchemaName;

                if (string.IsNullOrEmpty(_clientSchemaName))
                    return null;

                if (string.IsNullOrEmpty(testPassId))
                {
                    Dictionary<string, string> oError = new Dictionary<string, string>();
                    oError.Add(this._errorText, "Test Pass ID is required");
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
                        if (dataReader != null && dataReader.HasRows && dataReader.FieldCount > 0)
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
                        else
                        {
                            Dictionary<string, string> oError = new Dictionary<string, string>();
                            oError.Add(this._errorText, "No data Found");
                            return Json(oError);
                        }
                    }
                    return Json(retObject);
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
        }

        [HttpPost, Route("InsertUpdateTestStep")]
        public JsonResult InsertUpdateTestStep([FromBody]TestStep_IU testStep)
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

            Dictionary<string, string> _result = new Dictionary<string, string>();

            List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();

            if (string.IsNullOrEmpty(testStep.testCaseId))

            {
                _result.Add(this._errorText, " Test Case ID is required");
                return Json(_result);
            }

            try
            {
                int _res = 0;
                string _roles = "";
                for (int i = 0; i < testStep.roleArray.Count; i++)
                {
                    _roles += testStep.roleArray[i] + ",";
                }
                if (_roles != "")
                {
                    _roles = _roles.Remove(_roles.LastIndexOf(","));
                }
                 int _seq = 0;
                int.TryParse(testStep.testStepSequence, out _seq);
                //List<TestStep> dt = generateTestStepTable(testStep);
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spTestStep_Core";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestStepActionName", SqlDbType.VarChar) { Value = testStep.testStepName });
                    cmd.Parameters.Add(new SqlParameter("@TestCaseId", SqlDbType.Int) { Value = Convert.ToInt32(testStep.testCaseId) });
                    cmd.Parameters.Add(new SqlParameter("@ExpectedResult", SqlDbType.VarChar) { Value = testStep.expectedResult });
                    cmd.Parameters.Add(new SqlParameter("@TestStepSequence", SqlDbType.Int) { Value = Convert.ToInt32(_seq) });
                    cmd.Parameters.Add(new SqlParameter("@RoleArray", SqlDbType.VarChar) { Value = _roles });
                    cmd.Parameters.Add(new SqlParameter("@ER_Attachment_URL", SqlDbType.VarChar) { Value = testStep.erAttachmentURL });
                    cmd.Parameters.Add(new SqlParameter("@ER_Attachment_Name", SqlDbType.VarChar) { Value = testStep.erAttachmentName });
                    cmd.Parameters.Add(new SqlParameter("@Expected_Result_Image", SqlDbType.VarChar) { Value = testStep.expResultImage });
                    if (testStep.action.ToLower() == "edit")
                    {
                        cmd.Parameters.Add(new SqlParameter("@TestStepId", SqlDbType.Int) { Value = Convert.ToInt32(testStep.testStepId) });
                    }
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = (testStep.action.ToLower() == "add") ? "Insert" : "Update" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    cmd.Parameters.Add(new SqlParameter("@UserCId", SqlDbType.Int) { Value = Convert.ToInt32(testStep.UserCid) });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    //var resp = _context.Database.ExecuteN();
                    //return Json(ReturnVal.Value);


                    _res = cmd.ExecuteNonQuery();
                    var OutResult = outparam.Value;
                    if (_res > 0)
                        _result.Add(this._statusText, "Done");
                    else
                        _result.Add(this._errorText, OutResult.ToString());
                }

            }
            catch (Exception ex)
            {
                _result.Add(this._errorText, ex.Message);

            }
            return Json(_result);
        }

        //private List<TestStep> generateTestStepTable(TestStep testStep)
        //{
        //    List<TestStep> dtTestStep = new List<TestStep>();

        //    #region Add Columns
        //    /*
        //     dtTestStep.Columns.Add("RowID", typeof(int));
        //     dtTestStep.Columns["RowID"].AutoIncrement = true;
        //     dtTestStep.Columns["RowID"].AutoIncrementSeed = 1;
        //     dtTestStep.Columns["RowID"].AutoIncrementStep = 1;

        //     dtTestStep.Columns.Add("TestStep_ID", typeof(int));
        //     dtTestStep.Columns["TestStep_ID"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("TestCase_ID", typeof(int));
        //     dtTestStep.Columns["TestCase_ID"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("TestStep_ActionName", typeof(string));
        //     dtTestStep.Columns["TestStep_ActionName"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("DisplayTestStep_ID", typeof(string));
        //     dtTestStep.Columns["DisplayTestStep_ID"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("Expected_Result", typeof(string));
        //     dtTestStep.Columns["Expected_Result"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("TestStep_Sequence", typeof(int));
        //     dtTestStep.Columns["TestStep_Sequence"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("ER_Attachment_URL", typeof(string));
        //     dtTestStep.Columns["ER_Attachment_URL"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("ER_Attachment_Name", typeof(string));
        //     dtTestStep.Columns["ER_Attachment_Name"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("Expected_Result_Image", typeof(string));
        //     dtTestStep.Columns["Expected_Result_Image"].AllowDBNull = true;

        //     dtTestStep.Columns.Add("Role_ID", typeof(int));
        //     dtTestStep.Columns["Role_ID"].AllowDBNull = true;*/


        //    #endregion

        //    if (testStep != null)
        //    {
        //        if (testStep.roleArray != null)
        //        {

        //            for (int i = 0; i <= testStep.roleArray.Count - 1; i++)
        //            {
        //                List<string> oLst = new List<string>();
        //                oLst.Add(testStep.roleArray[i]);
        //                //DataRow dr = dtTestStep.NewRow();
        //                dtTestStep.Add(new TestStep
        //                {
        //                    testStepId = (testStep.testStepId == "") ? null : Convert.ToString(testStep.testStepId),
        //                    testCaseId = (testStep.testCaseId == "") ? null : Convert.ToString(testStep.testCaseId),
        //                    testStepName = (testStep.testStepName == "") ? null : Convert.ToString(testStep.testStepName),/*dr["TestStep_ActionName"]*/
        //                    expectedResult = (testStep.expectedResult == "") ? null : Convert.ToString(testStep.expectedResult),
        //                    testStepSequence = (testStep.testStepSequence == "") ? null : Convert.ToString(testStep.testStepSequence),
        //                    erAttachmentURL = (testStep.erAttachmentURL == "") ? null : Convert.ToString(testStep.erAttachmentURL),
        //                    erAttachmentName = (testStep.erAttachmentName == "") ? null : Convert.ToString(testStep.erAttachmentName),
        //                    expectedResultImage = (testStep.expectedResultImage == "") ? null : Convert.ToString(testStep.expectedResultImage),
        //                    roleArray = oLst,

        //                });
        //            }
        //        }
        //    }

        //    return dtTestStep;
        //}

        [HttpPost, Route("TestStepSequencing/{testStepIdSequence}")]
        public JsonResult TestStepSequencing(string testStepIdSequence)
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

            Dictionary<string, string> _result = new Dictionary<string, string>();


            //testCaseIdSequence = "2025~111#2026~2#2027~33#2028~48#2029~5#2030~6546";
            if (string.IsNullOrEmpty(testStepIdSequence))
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, "testStepIdSequence is required");
                return Json(oError);
            }

            try
            {

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spTestStepSeq";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SequencingString", SqlDbType.VarChar) { Value = testStepIdSequence });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Sequence" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    //var resp = _context.Database.ExecuteN();
                    //return Json(ReturnVal.Value);
                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutValue = outparam.Value;

                    //if (_res > 0)
                    //    _result.Add(this._statusText, "DONE");
                    //else
                    //    _result.Add(this._errorText, OutValue.ToString());



                    if (!string.IsNullOrEmpty(OutValue.ToString()) && OutValue.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        Dictionary<string, string> oError = new Dictionary<string, string>();
                        oError.Add(this._errorText, OutValue.ToString());
                        return Json(oError);
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }
            return Json(_result);
        }

        [HttpDelete, Route("DeleteTestStep/{testStepId}")]
        public JsonResult DeleteTestStep(string testStepId)
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

            Dictionary<string, string> _result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(testStepId))
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, "TestStepId is required!");
                return Json(oError);
            }

            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "UAT.spTestStep";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TestStepId", SqlDbType.VarChar) { Value = testStepId });
                    cmd.Parameters.Add(new SqlParameter(this._statementTypeParameterName, SqlDbType.VarChar) { Value = "Delete" });
                    cmd.Parameters.Add(new SqlParameter(this._schemaNameParameterName, SqlDbType.NVarChar, 10) { Value = _clientSchemaName });
                    SqlParameter outparam = new SqlParameter(this._returnParameter, SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outparam);

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    //var resp = _context.Database.ExecuteN();
                    //return Json(ReturnVal.Value);
                    int _res = 0;
                    _res = cmd.ExecuteNonQuery();
                    var OutValue = outparam.Value;



                    if (_res > 0)
                        _result.Add(this._statusText, "DONE");
                    else
                        _result.Add(this._errorText, OutValue.ToString());



                    if (!string.IsNullOrEmpty(OutValue.ToString()) && OutValue.ToString().ToLower() == "success")
                    {
                        _result.Add(this._statusText, "Done");
                    }
                    else
                    {
                        Dictionary<string, string> oError = new Dictionary<string, string>();
                        oError.Add(this._errorText, OutValue.ToString());
                        return Json(oError);
                    }
                }
            }

            catch (Exception ex)
            {
                Dictionary<string, string> oError = new Dictionary<string, string>();
                oError.Add(this._errorText, ex.Message);
                return Json(oError);
            }

            return Json(_result);
        }
       
    }
}


