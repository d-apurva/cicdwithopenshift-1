using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rgen.UAT.UATToolServiceLayer.Models
{

    [DataContract]
    public class StakeholderDashboardProject
    {
        [DataMember]
        public string groupId { get; set; }
        [DataMember]
        public string groupName { get; set; }
        [DataMember]
        public string portfolioId { get; set; }
        [DataMember]
        public string portfolio { get; set; }
        [DataMember]
        public string projectId { get; set; }
        [DataMember]
        public string projectName { get; set; }
        [DataMember]
        public string projectVersion { get; set; }

        [DataMember]
        public string projectLeadEmail { get; set; }
        [DataMember]
        public string projectLeadAlias { get; set; }
        [DataMember]
        public string projectLeadName { get; set; }
        [DataMember]
        public string projectLeadSpUserId { get; set; }
        /* [DataMember]
         public string leastSecurityId { get; set; }*/

        [DataMember]
        public string testpassId { get; set; }
        [DataMember]
        public string testpassName { get; set; }
        [DataMember]
        public string tpStartDate { get; set; }
        [DataMember]
        public string tpEndDate { get; set; }
        [DataMember]
        public string tpStatus { get; set; }

        [DataMember]
        public string testManagerAlias { get; set; }
        [DataMember]
        public string testManagerEmail { get; set; }
        [DataMember]
        public string testManagerSpUserId { get; set; }
        [DataMember]
        public string testManagerName { get; set; }
        [DataMember]
        public string testerId { get; set; }

        [DataMember]
        public string passCount { get; set; }
        [DataMember]
        public string failcount { get; set; }
        [DataMember]
        public string ntCompletedCount { get; set; }
        [DataMember]
        public string pendingCount { get; set; }

        [DataMember]
        public string projectStartDate { get; set; }
        [DataMember]
        public string projectEndDate { get; set; }
        [DataMember]
        public string projectStakeholders { get; set; }
    }

    public class clsStakeholderDbTSDetails
    {
        public string testerId { get; set; }

        public string testerName { get; set; }

        public string roleId { get; set; }

        public string roleName { get; set; }

        public string teststepId { get; set; }

        public string teststepName { get; set; }

        public string Modified { get; set; }

        public string Created { get; set; }

        public string expectedResult { get; set; }

        public string actualResult { get; set; }

        public string status { get; set; }
    }

    [DataContract]
    public class lstStakeholderDetails
    {
        [DataMember]
        public string teststepId { get; set; }
        [DataMember]
        public string teststepName { get; set; }
        [DataMember]
        public string Modified { get; set; }
        [DataMember]
        public string expectedResult { get; set; }
        [DataMember]
        public string actualResult { get; set; }
        [DataMember]
        public string status { get; set; }
    }

   
    [DataContract]
    public class lstStakeholderDbTSDetails
    {
        [DataMember]
        public string teststepId { get; set; }
        [DataMember]
        public string teststepName { get; set; }
        [DataMember]
        public string Modified { get; set; }
        [DataMember]
        public string expectedResult { get; set; }
        [DataMember]
        public string actualResult { get; set; }
        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class StakeholderDbTSDetails
    {
        [DataMember]
        public string teststepId { get; set; }
        [DataMember]
        public string teststepName { get; set; }
        [DataMember]
        public string Modified { get; set; }
        [DataMember]
        public string Created { get; set; }
        [DataMember]
        public string expectedResult { get; set; }
        [DataMember]
        public string actualResult { get; set; }
        [DataMember]
        public string status { get; set; }
        //[DataMember]
        //public string tcId { get; set; }
        //[DataMember]
        //public string trID { get; set; }
        //[DataMember]
        //public string tpID { get; set; }
        //[DataMember]
        //public string rID { get; set; }

        public List<clsStakeholderDbTSDetails> lstStakeholderDbTSDetails { get; set; }
    }

    [DataContract]
    public class StakeholderDbTestStep
    {
        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string projectName { get; set; }

        [DataMember]
        public string testpassId { get; set; }

        [DataMember]
        public string testpassName { get; set; }

        [DataMember]
        public string testcaseId { get; set; }

        [DataMember]
        public string testcaseName { get; set; }

        [DataMember]
        public string testerId { get; set; }

        [DataMember]
        public string testerName { get; set; }

        [DataMember]
        public string roleId { get; set; }

        [DataMember]
        public string roleName { get; set; }

        [DataMember]
        public List<StakeholderDbTSDetails> lstStakeholderDbTSDetails { get; set; }
    }

    [DataContract]
    public class StkDBUniqueObject
    {
        [DataMember]
        public int ProjectID { get; set; }
        [DataMember]
        public int TPID { get; set; }
        [DataMember]
        public int TCID { get; set; }
        [DataMember]
        public int TesterID { get; set; }
        [DataMember]
        public int RoleID { get; set; }
    }

}
