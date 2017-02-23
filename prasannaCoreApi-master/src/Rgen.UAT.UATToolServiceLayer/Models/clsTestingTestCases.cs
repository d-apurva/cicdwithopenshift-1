using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsTestingTestCases
    {
    }
   
    public class TestingTestCases
    {
        
        public string testCaseId { get; set; }

        
        public string testCaseName { get; set; }

        
        public string testCaseDescription { get; set; }

        
        public string testCaseSeq { get; set; }

        
        public string testCaseETT { get; set; }

        
        public List<listTestStep> listTestStep { get; set; }
    }
   
    public class listTestStep
    {
        
        public string testStepPlanId { get; set; }

        
        public string testStepName { get; set; }

        
        public string expResult { get; set; }

        
        public string actResult { get; set; }

        
        public string status { get; set; }

        
        public string testStepSeq { get; set; }

        
        public string expAttachment { get; set; }

        
        public string actAttachment1 { get; set; }

        
        public string actAttachment2 { get; set; }

        
        public string actAttachment3 { get; set; }
    }

    public class clsTestPg
    {
        public string TestCase_Name { get; set; }
        public string TestCase_Sequence { get; set; }
        public string TestCase_ID { get; set; }
        public string TestCase_Description { get; set; }
        public string ETT { get; set; }
        public string Expected_Result { get; set; }
        public string TestStep_ActionName { get; set; }
        public string TestStep_Sequence { get; set; }
        public string Status { get; set; }
        public string Actual_Result { get; set; }
        public string TestStepPlan_Id { get; set; }
        public string ER_Attachment_URL { get; set; }
        public string AR_Attachment1_URL { get; set; }
        public string AR_Attachment2_URL { get; set; }
        public string AR_Attachment3_URL { get; set; }
    }

    public class clsTestSD
    {
        public string status { get; set; }
        public string projectId { get; set; }
        public string actualResult { get; set; }
        public string expectedResult { get; set; }
        public string testerId { get; set; }
        public string testCaseId { get; set; }
        public string roleId { get; set; }
        public string testerName { get; set; }
        public string testCaseName { get; set; }
        public string roleName { get; set; }
        public string projectName { get; set; }
        public string testStepId { get; set; }
        public string testStepName { get; set; }
        public string modified { get; set; }
        public string created { get; set; }
        public string testPassName { get; set; }
        public string testPassId { get; set; }
    }

    public class clsTestFeedBack
    {
        public string testpassId { get; set; }
        public string testcaseId { get; set; }
        public string Role_ID { get; set; }
        public string userId { get; set; }
        public string testpassName { get; set; }
        public string testcaseName { get; set; }
        public string testerName { get; set; }
        public string Role_Name { get; set; }
        public string Feedback_Type { get; set; }
        public string Feedback { get; set; }
        public string Rating { get; set; }
        public string RatingFeedId { get; set; }
        public string TestStep_ID { get; set; }
        public string TestStep_ActionName { get; set; }
        public string TestStepPlan_Id { get; set; }
        public string Expected_Result { get; set; }
        public string Actual_Result { get; set; }
        public string TesterStatusText { get; set; }
        public string TspFeedback { get; set; }
    }
}
