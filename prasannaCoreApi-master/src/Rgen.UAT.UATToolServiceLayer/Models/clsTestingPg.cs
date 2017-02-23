using System.Collections.Generic;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsTestingPg
    {
    }

    public class listProjectTestPass
    {

        public string projectName { get; set; }


        public string projectEndDate { get; set; }


        public string testPassName { get; set; }


        public string testPassManager { get; set; }


        public string managerEmail { get; set; }


        public string testPassEndDate { get; set; }


        public string testPassStartDate { get; set; }


        public string testingType { get; set; }


        public string feedbackRType { get; set; }


        public string testerArea { get; set; }


        public string projectStatus { get; set; }


        public string testPassStatus { get; set; }


        public string testerRole { get; set; }


        public List<listActualAliasUrls> listActualAliasUrls { get; set; }


        public List<listTestPassNames> listTestPassNames { get; set; }
    }

    public class listTestPassNames
    {

        public string testPassName { get; set; }
    }

    public class listActualAliasUrls
    {

        public string aliasUrl { get; set; }


        public string actualUrl { get; set; }
    }

   
    public class actualAttachment
    {
        
        public string testStepPlanId { get; set; }

        
        public string actAttachmentName { get; set; }

        
        public string actAttachmentUrl { get; set; }

        
        public string actAttachmentIndex { get; set; }
    }
  
    public class TestingTSPlan
    {
       
        public string testStepPlanId { get; set; }

       
        public string status { get; set; }

       
        public string actResult { get; set; }
    }
   
    public class clsTestingFR
    {
       
        public string feedbackId { get; set; }

       
        public string testPassId { get; set; }

       
        public string testCaseId { get; set; }

       
        public string userId { get; set; }

       
        public string roleId { get; set; }

       
        public string rating { get; set; }

       
        public string feedback { get; set; }
    }
}
