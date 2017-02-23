using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    /// <summary>
    /// Author: Sheetal Dhiran
    /// Created Date: 10/11/2014
    /// Description: DataContract for TestPass of Users.
    /// </summary>

    [DataContract]
    public class TestPass 
    {

        //public class ProjectUser
        //{
        //    [DataMember]
        //    public string userName { get; set; }
        //    [DataMember]
        //    public string alias { get; set; }
        //    [DataMember]
        //    public string email { get; set; }
        //    [DataMember]
        //    public string spUserId { get; set; }
        //    [DataMember]
        //    public string securityId { get; set; }
        //}




        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string testPassId { get; set; }

        [DataMember]
        public string testPassName { get; set; }

        [DataMember]
        public string testPassDesp { get; set; }

        [DataMember]
        public string tpStartDate { get; set; }

        [DataMember]
        public string tpEndDate { get; set; }

        [DataMember]
        public string tpStatus { get; set; }

        [DataMember]
        public string totalTestCaseCount { get; set; }

        [DataMember]
        public string testPassDisplayId { get; set; }

        [DataMember]
        public List<ProjectUser> listTestMgr { get; set; }
        
    }

    /// <summary>
    /// To get list of all Testers with its Role,Passs,Fail,NCCount for each testpass
    /// Used for SendEmail and SendTestingStatus on TestPass Page
    /// </summary>
    [DataContract]
    public class TestPassTesterRolePFNCount
    {
        [DataMember]
        public string testPassId { get; set; }
        [DataMember]
        public string testPassStatus { get; set; }

        [DataMember]
        public List<TesterRolePFNCount> listTesterRolePFNCount { get; set; }
    }


    [DataContract]
    public class TesterRolePFNCount
    {
        [DataMember]
        public string testerName { get; set; }

        [DataMember]
        public string testerSpUserId { get; set; }

        [DataMember]
        public string testerEmail { get; set; }

        [DataMember]
        public string testerRoleId { get; set; }

        [DataMember]
        public string testerRoleName { get; set; }

        [DataMember]
        public string passCount { get; set; }

        [DataMember]
        public string failCount { get; set; }

        [DataMember]
        public string NCCount { get; set; }

        [DataMember]
        public string TCCount { get; set; }

        [DataMember]
        public string TestingTime { get; set; }

        [DataMember]
        public string FeedbackAvailable { get; set; }

    }

    [DataContract]
    public class TpDropDowns
    {
        [DataMember]
        public string groupId { get; set; }

        [DataMember]
        public string groupName { get; set; }

        [DataMember]
        public string portfolioId { get; set; }

        [DataMember]
        public string portfolioName { get; set; }

        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string projectName { get; set; }

        [DataMember]
        public string projectVersion { get; set; }
    }


    public class clsTestPassTesterRolePFNCountList
    {

        public string Status { get; set; }
        public string CurrentETT { get; set; }
        public string User_Name { get; set; }
        public string User_EmailId { get; set; }
        public string Role_Name { get; set; }
        public string passSteps { get; set; }
        public string failSteps { get; set; }
        public string ncSteps { get; set; }
        public string Feedback { get; set; }
    }

}
