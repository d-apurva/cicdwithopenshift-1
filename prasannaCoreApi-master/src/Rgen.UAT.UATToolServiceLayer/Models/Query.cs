using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class Query
    {
        
    }

    public class Q_collectionForStatus
    {
        public string st_TestStepId { get; set; }
    }

    public class Q_Project
    {
        public string Project_Id { get; set; }
        public string Project_Name { get; set; }
        public string Project_Version { get; set; }
    }

    public class Q_TestPass
    {
        public string TestPass_ID { get; set; }
        public string TestPass_Name { get; set; }
    }
    public class Q_Role
    {
        public string Role_Id { get; set; }
        public string Role_Name { get; set; }
    }


    public class QueryResponseTPandRoleDD
    {
        public List<QueryTestPassDropDown> lstTpDD { get; set; }
        public List<QueryRoleDropDown> lstRoleDD { get; set; }
    }

    public class QueryTestPassDropDown
    {
        public string sTestPassId { get; set; }
        public string sTestPassName { get; set; }
    }

    public class QueryRoleDropDown
    {
        public string sRoleId { get; set; }
        public string sRoleName { get; set; }

    }

    /******************Added from v2.0***************************************/

     
    public class QueryReq
    {
         
        public string sCriteria { get; set; }
         
        public string sOperator { get; set; }
         
        public string sInputValue { get; set; }
    }

     
    public class QueryObject
    {
         
        public string sCriteria { get; set; }
         
        public string sInputValue { get; set; }
         
        public string sClause { get; set; }

    }

     
    public class QueryProject
    {
         
        public string sProject_Id { get; set; }
         
        public string sProject_Name { get; set; }
         
        public string sProject_Version { get; set; }
         
        public string sProject_Status { get; set; }
         
        public string sVersion_Lead { get; set; }
         
        public string sType { get; set; }

    }



     
    public class QueryTP
    {
         
        public string sTestPassId { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sTestMgrId { get; set; }
         
        public string stestpassStatus { get; set; }
         
        public string sTestManagerName { get; set; }
         
        public string sType { get; set; }
         
        public string sStartDate { get; set; }
         
        public string sEndDate { get; set; }
         
        public string sTcCount { get; set; }
         
        public string sTPDescription { get; set; }
         
        public string sTesterCount { get; set; }
    }

     
    public class QueryUITP
    {

         
        public string sTestPassId { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sTestMgrId { get; set; }
         
        public string stestpassStatus { get; set; }
         
        public string sTestManagerName { get; set; }
         
        public string sType { get; set; }
         
        public string sStartDate { get; set; }
         
        public string sEndDate { get; set; }
         
        public string sTcCount { get; set; }
         
        public string sTPDescription { get; set; }
         
        public string sTesterCount { get; set; }
    }

     
    public class QueryTS
    {
         
        public string sTestStepId { get; set; }
         
        public string sTestStepName { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTestStepStatus { get; set; }
         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sType { get; set; }
         
        public string sTesterId { get; set; }

         
        public string sExpected_Result { get; set; }
         
        public string sActual_Result { get; set; }
         
        public string sAR_Attachment1_Name { get; set; }
         
        public string sAR_Attachment1_URL { get; set; }
         
        public string sAR_Attachment2_Name { get; set; }
         
        public string sAR_Attachment2_URL { get; set; }
         
        public string sAR_Attachment3_Name { get; set; }
         
        public string sAR_Attachment3_URL { get; set; }
    }

     
    public class QueryUITS
    {

         
        public string sTestStepId { get; set; }
         
        public string sTestStepName { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTestStepStatus { get; set; }
         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sTesterId { get; set; }
         
        public string sType { get; set; }

         
        public string sExpected_Result { get; set; }
         
        public string sActual_Result { get; set; }
         
        public string sAR_Attachment1_Name { get; set; }
         
        public string sAR_Attachment1_URL { get; set; }
         
        public string sAR_Attachment2_Name { get; set; }
         
        public string sAR_Attachment2_URL { get; set; }
         
        public string sAR_Attachment3_Name { get; set; }
         
        public string sAR_Attachment3_URL { get; set; }
    }

     
    public class QueryTC
    {
         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sTCStatus { get; set; }
         
        public string sType { get; set; }
         
        public string sRoleId { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTesterId { get; set; }

        public  string sTestStepStatusID { get; set; }

        public string sRoleid_N { get; set; }
    }

     
    public class QueryUITC
    {

         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sTCStatus { get; set; }
         
        public string sType { get; set; }
         
        public string sRoleId { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTesterId { get; set; }

    }

     
    public class QueryProjectDropDown
    {
         
        public string ProjectId { get; set; }
         
        public string ProjectName { get; set; }
         
        public string ProjectVersion { get; set; }
    }



     
    public class QueryRoleTester
    {
         
        public string sRoleId { get; set; }

         
        public string sRoleName { get; set; }

         
        public string sTpId { get; set; }

         
        public string sTpName { get; set; }

         
        public string sStartDate { get; set; }

         
        public string sEndDate { get; set; }

         
        public string sTcCount { get; set; }

         
        public string sTesterID { get; set; }

         
        public string sTesterUserID { get; set; }

         
        public string sTesterName { get; set; }

         
        public string sStatus { get; set; }

         
        public string sType { get; set; }


    }


     
    public class QueryRoleTestStep
    {
         
        public string sCriteria { get; set; }

         
        public string sRoleId { get; set; }

         
        public string sRoleName { get; set; }

         
        public string sTestStepName { get; set; }

         
        public string sTestStepID { get; set; }

         
        public string sTestPassName { get; set; }

         
        public string sTestPassID { get; set; }

         
        public string sTestCaseName { get; set; }

         
        public string sTestCaseID { get; set; }

         
        public string sTesterID { get; set; }

         
        public string sTester { get; set; }

         
        public string sStatus { get; set; }

         
        public string sType { get; set; }

         
        public string sUserId { get; set; }

         
        public string sExpected_Result { get; set; }
         
        public string sActual_Result { get; set; }
         
        public string sAR_Attachment1_Name { get; set; }
         
        public string sAR_Attachment1_URL { get; set; }
         
        public string sAR_Attachment2_Name { get; set; }
         
        public string sAR_Attachment2_URL { get; set; }
         
        public string sAR_Attachment3_Name { get; set; }
         
        public string sAR_Attachment3_URL { get; set; }
    }
    
    
    public class QueryResponse
    {
         
        public List<QueryProject> lstProject { get; set; }

         
        public List<QueryTP> lstTestPass { get; set; }

         
        public List<QueryTS> lstTestStep { get; set; }

         
        public List<QueryTC> lstTestCase { get; set; }

         
        public List<QueryUITC> lstTestCaseUI { get; set; }

         
        public List<QueryUITS> lstTestStepUI { get; set; }

         
        public List<QueryUITP> lstTestPassUI { get; set; }

         
        public List<QueryRoleTester> lstRoleTester { get; set; }

         
        public List<QueryRoleTestStep> lstRoleTS { get; set; }

         
        public List<QueryAssignedToTester> lstAssignedToTester { get; set; }

         
        public List<QueryAssignedToTestMgr> lstAssignedToTestMgr { get; set; }

         
        public List<QueryTCByStatus> lstTcByStatus { get; set; }

         
        public List<QueryTPByStatus> lstTPByStatus { get; set; }

         
        public List<QueryProjectByStatus> lstPrjByStatus { get; set; }

         
        public List<QueryTSByStatus> lstTSStatus { get; set; }

         
        public List<QueryTSByID> lstTSByID { get; set; }

         
        public List<QueryTCByID> lstTCByID { get; set; }

         
        public List<QueryProjectByID> lstPrjByID { get; set; }

         
        public List<QueryTestPassByID> lstTPByID { get; set; }

         
        public List<QueryAssignedPrjLead> lstAssignPrjLead { get; set; }
    }


     
    public class QueryAssignedToTester
    {

         
        public string sTpId { get; set; }

         
        public string sTpName { get; set; }

         
        public string sRoleId { get; set; }

         
        public string sRoleName { get; set; }

         
        public string sTesterID { get; set; }

         
        public string sTesterName { get; set; }

         
        public string sStatusTester { get; set; }
         
        public string sArea_Name { get; set; }

         
        public string sType { get; set; }
    }

     
    public class QueryAssignedToTestMgr
    {

         
        public string sTpId { get; set; }

         
        public string sTpName { get; set; }

         
        public string sTestMgrID { get; set; }

         
        public string sTestMgrName { get; set; }

         
        public string sStatusTestMgr { get; set; }

         
        public string sType { get; set; }

         
        public string sStartDate { get; set; }
         
        public string sEndDate { get; set; }
         
        public string sTcCount { get; set; }
         
        public string sTesterId { get; set; }
         
        public string sTpDescription { get; set; }
         
        public string sTesterCount { get; set; }
    }

     
    public class QueryTPByStatus
    {
         
        public string sTestPassId { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sTestMgrId { get; set; }
         
        public string stestpassStatus { get; set; }
         
        public string sTestManagerName { get; set; }
         
        public string sType { get; set; }
         
        public string sStartDate { get; set; }
         
        public string sEndDate { get; set; }
         
        public string sTcCount { get; set; }
         
        public string sTPDescription { get; set; }
         
        public string sTesterCount { get; set; }
    }

     
    public class QueryProjectByStatus
    {
         
        public string sProject_Id { get; set; }
         
        public string sProject_Name { get; set; }
         
        public string sProject_Version { get; set; }
         
        public string sProject_Status { get; set; }
         
        public string sVersion_Lead { get; set; }
         
        public string sType { get; set; }

    }

     
    public class QueryTSByStatus
    {

         
        public string sTestStepId { get; set; }
         
        public string sTestStepName { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTestStepStatus { get; set; }
         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sType { get; set; }
         
        public string sTesterId { get; set; }

         
        public string sExpected_Result { get; set; }
         
        public string sActual_Result { get; set; }
         
        public string sAR_Attachment1_Name { get; set; }
         
        public string sAR_Attachment1_URL { get; set; }
         
        public string sAR_Attachment2_Name { get; set; }
         
        public string sAR_Attachment2_URL { get; set; }
         
        public string sAR_Attachment3_Name { get; set; }
         
        public string sAR_Attachment3_URL { get; set; }
    }

     
    public class QueryTCByStatus
    {

         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sTCStatus { get; set; }
         
        public string sType { get; set; }
         
        public string sRoleId { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTesterId { get; set; }


    }

     
    public class QueryTSByID
    {

         
        public string sTestStepId { get; set; }
         
        public string sTestStepName { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTestStepStatus { get; set; }
         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sType { get; set; }
         
        public string sTesterId { get; set; }

         
        public string sExpected_Result { get; set; }
         
        public string sActual_Result { get; set; }
         
        public string sAR_Attachment1_Name { get; set; }
         
        public string sAR_Attachment1_URL { get; set; }
         
        public string sAR_Attachment2_Name { get; set; }
         
        public string sAR_Attachment2_URL { get; set; }
         
        public string sAR_Attachment3_Name { get; set; }
         
        public string sAR_Attachment3_URL { get; set; }
    }

     
    public class QueryTCByID
    {

         
        public string sTestPassId { get; set; }
         
        public string sTestCaseId { get; set; }
         
        public string sTestCaseName { get; set; }
         
        public string sTestPassName { get; set; }
         
        public string sUserName { get; set; }
         
        public string sTCStatus { get; set; }
         
        public string sType { get; set; }
         
        public string sRoleId { get; set; }
         
        public string sRoleName { get; set; }
         
        public string sTesterId { get; set; }
    }

     
    public class QueryProjectByID
    {

         
        public string sProjID { get; set; }

         
        public string sProjName { get; set; }

         
        public string sProjVersion { get; set; }

         
        public string sProjStatus { get; set; }

         
        public string sProjleadID { get; set; }

         
        public string sProjleadName { get; set; }

         
        public string sType { get; set; }
    }

     
    public class QueryTestPassByID
    {

         
        public string sTPID { get; set; }

         
        public string sTPName { get; set; }

         
        public string sStart_Date { get; set; }

         
        public string sEnd_Date { get; set; }

         
        public string sTestMgr_ID { get; set; }

         
        public string sTestMgr_Name { get; set; }

         
        public string sTpstatus { get; set; }

         
        public string sTcCount { get; set; }

         
        public string sType { get; set; }
         
        public string sTPDescription { get; set; }
         
        public string sTesterCount { get; set; }
    }

     
    public class QueryAssignedPrjLead
    {
         
        public string sProjID { get; set; }

         
        public string sProjName { get; set; }

         
        public string sProjVersion { get; set; }

         
        public string sProjStatus { get; set; }

         
        public string sProjleadName { get; set; }

         
        public string sType { get; set; }
    }

     
    public class QueryResponseForAND
    {
         
        public List<QueryProject> lstProject { get; set; }

         
        public List<QueryTP> lstTestPass { get; set; }

         
        public List<QueryTS> lstTestStep { get; set; }

         
        public List<QueryTC> lstTestCase { get; set; }

         
        public List<QueryUITC> lstTestCaseUI { get; set; }

         
        public List<QueryUITS> lstTestStepUI { get; set; }

         
        public List<QueryUITP> lstTestPassUI { get; set; }

         
        public List<QueryRoleTester> lstRoleTester { get; set; }

         
        public List<QueryRoleTestStep> lstRoleTS { get; set; }

         
        public List<QueryAssignedToTester> lstAssignedToTester { get; set; }

         
        public List<QueryAssignedToTestMgr> lstAssignedToTestMgr { get; set; }

         
        public List<QueryTCByStatus> lstTcByStatus { get; set; }

         
        public List<QueryTPByStatus> lstTPByStatus { get; set; }

         
        public List<QueryProjectByStatus> lstPrjByStatus { get; set; }

         
        public List<QueryTSByStatus> lstTSStatus { get; set; }

         
        public List<QueryTSByID> lstTSByID { get; set; }

         
        public List<QueryTCByID> lstTCByID { get; set; }

         
        public List<QueryProjectByID> lstPrjByID { get; set; }

         
        public List<QueryTestPassByID> lstTPByID { get; set; }

         
        public List<QueryAssignedPrjLead> lstAssignPrjLead { get; set; }
    }
}
