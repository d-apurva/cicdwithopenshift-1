using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsDetailAnalysis
    {
    }
    public class DetailAnalysis
    {
       
        public string projectId { get; set; }

       
        public string projectName { get; set; }

       
        public string projectVersion { get; set; }

       
        public string versionLead { get; set; }

       
        public string testpassId { get; set; }

       
        public string testpassName { get; set; }

       
        public string testManager { get; set; }

       
        public string testPassDescription { get; set; }

       
        public string tpStartDate { get; set; }

       
        public string tpEndDate { get; set; }


       
        public string testerId { get; set; }

       
        public string testerName { get; set; }

       
        public string roleId { get; set; }

       
        public string roleName { get; set; }

       
        public string testCaseId { get; set; }

       
        public string testCaseName { get; set; }

        

       
        public string totalTestCaseCount { get; set; }

       
        public string totalTestStepCount { get; set; }

       
        public string totalTesterCount { get; set; }


       
        public string testCasePassCount { get; set; }

       
        public string testCaseFailcount { get; set; }

       
        public string testCaseNtCompletedCount { get; set; }

       
        public string testStepPassCount { get; set; }

       
        public string testStepFailcount { get; set; }

       
        public string testStepNtCompletedCount { get; set; }

       
        public List<TesterDataForDetailAnalysis> listTesterData { get; set; }
    }

    public class TesterDataForDetailAnalysis
    {
       
        public string testerId { get; set; }

       
        public string testerName { get; set; }

       
        public string testerArea { get; set; }

       
        public string testerPassCount { get; set; }

       
        public string testerFailcount { get; set; }

       
        public string testerNtCompletedCount { get; set; }

    }

    public class DropdownDataForDetailAnalysis
    {
       
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string projectVersion { get; set; }
        public string versionLead { get; set; }
        
        public string testpassId { get; set; }
        public string testpassName { get; set; }
        public string testManager { get; set; }
        public string tpEndDate { get; set; }
        
        public string testerId { get; set; }
        public string testerName { get; set; }
        public string roleId { get; set; }
        public string roleName { get; set; }
        public List<clsTestterList> lstTesterList { get; set; }
    }

    public class Export_DA
    {       
        public int projectId { get; set; }
        public string projectName { get; set; }
        public string Project_Version { get; set; }
        public int TestPass_ID { get; set; }
        public string TestPass_Name { get; set; }
        public int RowID { get; set; }
        public string testcaseName { get; set; }
        public int TestCaseID { get; set; }
        public int userid { get; set; }
        public int TesterId { get; set; }
        public int NC { get; set; }
        public int Fail { get; set; }
        public int Pass { get; set; }
        public string TcStatusByTester { get; set; }
        public int MarkDelete { get; set; }
    }

    public class Export_DA_replica
    {      
        public string project { get; set; }
        public string Version { get; set; }    
        public string TestPass { get; set; }    
        public string TestCase { get; set; }
        public int TesterOnTc { get; set; }
        public int TesterPassTC { get; set; }
        public int TesterFailTC { get; set; }
        public int TesterNCTC { get; set; }      
    }

    public class OfflineTemplate
    {      
        public string Project_Name { get; set; }
        public string Project_Version { get; set; }
        public string TestPass_Name { get; set; }
        public string TestManager { get; set; }
        public string TestCase_Name { get; set; }
        public string TestCase_Sequence { get; set; }
        public string TestCase_ID { get; set; }
        public string TestCase_Description { get; set; }
        public string ETT { get; set; }
        public string Expected_Result { get; set; }
        public string TestStep_ActionName { get; set; }
        public string TestStep_Sequence { get; set; }
        public string Actual_Result { get; set; }
        public string TestStep_Id { get; set; }
        public string TestStepPlan_Id { get; set; }
        public string Role_Name { get; set; }
        public string Status { get; set; }
        public string Tester_Name { get; set; }
        public string TestStepStatus { get; set; }
    }

    public class BulkImport_testing
    {
        public List<Import_TestStep> TestStep_Listobj { get; set; }
    }

    public class Import_TestStep
    {
        public string s_status { get; set; }
        public string s_actualResult { get; set; }
        public string s_planId { get; set; }
    }
    public class clsTestStepIdTableDataTable_c
    {
        public string Actual_Result { get; set; }
        public int Status { get; set; }
        public int TestStepPlanID { get; set; }
    }

    public class tsDataCollection_c : List<clsTestStepIdTableDataTable_c>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("Actual_Result", SqlDbType.VarChar,500),
                new SqlMetaData("Status", SqlDbType.Int),
                new SqlMetaData("TestStepPlanID", SqlDbType.Int)
                );

            foreach (clsTestStepIdTableDataTable_c data in this)
            {
                ret.SetString(0, data.Actual_Result);
                ret.SetInt32(1, data.Status);
                ret.SetInt32(2, data.TestStepPlanID);
                yield return ret;
            }
        }
    }
}
