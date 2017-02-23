using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsTriage
    {

    }
  
    public class Triage
    {

      
        /// TestPassId
        public string testPassId { get; set; }
       
        /// Test Pass Name
        public string testPassName { get; set; }
       
        /// Test Pass Description
        public string description { get; set; }
      
        /// Test Manager Name
        public string testMgrId { get; set; }
      
        /// Test Manager Name
        public string testMgrName { get; set; }
     
        /// Test Pass Due Date
        public string endDate { get; set; }
     
        //public string passedTestSteps { get; set; }
       
        public string failedTestSteps { get; set; }
       
        //public string notCompletedTestSteps { get; set; }
     
        /// Tester ID
        public string testerSpuserId { get; set; }
   
        /// Tester Name
        public string testerName { get; set; }
      
        /// list of testcases of test pass
        public List<TriageTestSteps> listTriageTestSteps { get; set; }
       
        public string testCaseId { get; set; }
      
        public string testCaseName { get; set; }
        public string testCaseDesc { get; set; }
    }

   
    public class TriageTestSteps
    {
       
        /// TriageId
        public string triageId { get; set; }
     
        //public string expectedResult { get; set; }
        
        /// Actual Result of test step
        public string actualResult { get; set; }

      
        /// Tester Role Id
        public string roleId { get; set; }
       
        /// Tester Role Name
        public string roleName { get; set; }
        
        /// bug
        public string bug { get; set; }
       
        /// vstfBug
        public string vstfBug { get; set; }
       
        /// triageDone
        public string triageDetails { get; set; }
       
        /// testPassId for bottom table
        public string tpId { get; set; }
       
        /// testPassId for bottom table
        public string tcId { get; set; }
       
        /// testStepId for bottom table
        public string tsId { get; set; }
       
        public string uId { get; set; }
        
        public string teststepPlanId { get; set; }
       
        public List<Attachment> lstAttachment { get; set; }
  
        public string vstfBugTitle { get; set; }
        
        public string vstfBugLink { get; set; }
        
        public string owner { get; set; }
        
        public string testingDate { get; set; }
       
        public string priority { get; set; }
        
        public string severity { get; set; }
        
        public string dateClosed { get; set; }
        
        public string Triagestatus { get; set; }

      
        /// Expected Result of test step
        public string expectedResult { get; set; }

        /// test step id
        public string testStepId { get; set; }
        
        /// Test Step Name
        public string testStepName { get; set; }
    }

   
    /// object values to request triage data
    public class TriageReqObj
    {
        /* [DataMember]
         public string spUserId { get; set; }*/
      
        public string projectId { get; set; }
       
        public string testpassId { get; set; }
       
        public string testerId { get; set; }

        public string roleId { get; set; }
    }

   
    public class LstAttachmentTriage
    {
        public string AR_Attachement1 { get; set; }
        
        public string AR_Attachement2 { get; set; }
        
        public string AR_Attachement3 { get; set; }
    }
}
