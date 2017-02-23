using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class fileData
    {
        public string file { get; set; }
    }

    public class Reports
    {
       
        public string testPassId { get; set; }
      
        public string testPassName { get; set; }
       
        public string description { get; set; }
      
        public string testMgrName { get; set; }
       
        public string dueDate { get; set; }
       
        public string passedTestSteps { get; set; }
       
        public string failedTestSteps { get; set; }
       
        public string notCompletedTestSteps { get; set; }
       
        public string testCaseId { get; set; }

      
        public string testCaseName { get; set; }


        public string testCaseDesc { get; set; }

       
        public string testStepName { get; set; }

      
        public string testStepId { get; set; }

     
        public string expectedResult { get; set; }

       
        public string testerId { get; set; }

        public string testerName { get; set; }

       
        public List<ReportTesterRoleStatus> listRptTesterRoleStatus { get; set; }
    }

  
    public class ReportTesterRoleStatus
    {
       
        public string teststepPlanId { get; set; }

       
        public string roleId { get; set; }

      
        public string roleName { get; set; }

     
        public string status { get; set; }

        
        public string actualResult { get; set; }
    }


    
    public class ReportUniqueObject
    {
      
        public int TPID { get; set; }
       
        public int TSID { get; set; }
      
        public int UID { get; set; }
    }
}
