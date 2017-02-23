using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class TestStep
    {
        public class Role
        {
            public string roleId { get; set; }
            public string roleName { get; set; }
            public string isTestingStarted { get; set; }
            public string isTesterAssigned { get; set; }
        }
        public string testPassId { get; set; }
        public string testCaseId { get; set; }
        public string testStepId { get; set; }
        public string testStepName { get; set; }
        public string expectedResult { get; set; }
        public string testStepSequence { get; set; }
        public string erAttachmentURL { get; set; }
        public string erAttachmentName { get; set; }
        public List<Role> roleList { get; set; }
        public List<string> roleArray { get; set; }
        public string action { get; set; }
        public string isTestingStarted { get; set; }
        public string expectedResultImage { get; set; }
    }

    public class TesterType
    {
        public int RowID { get; set; }
        public int Tester_ID { get; set; }
        public int TestPass_ID { get; set; }
        public int Area_ID { get; set; }
        public int Role_ID { get; set; }
        public int User_ID { get; set; }
    }
    

    public class TestPassTesterRoles
    {
        #region Role Defination
        public class Role
        {
            public string roleId { get; set; }
            public string roleName { get; set; }
            public string projectId { get; set; }
            public string isTestingInprogress { get; set; }
            public string isTestStepsAssigned { get; set; }
        }

        #endregion

        public string testPassId { get; set; }
        public string spUserId { get; set; }
        public string oldTesterspUserId { get; set; }
        public string testerName { get; set; }
        public string testerAlias { get; set; }
        public string testerEmail { get; set; }
        public string areaId { get; set; }
        public string areaName { get; set; }
        public List<TestPassTesterRoles.Role> roleList { get; set; }
        public List<string> roleArray { get; set; }
        public string action { get; set; }
        public string testerID { get; set; }

        public List<ClsTesterTypeDataTable> listTesterType { get; set; }
    }

    public class TestStep_IU
    {
        public string testCaseId { get; set; }
        public string testStepName { get; set; }
        public string expectedResult { get; set; }
        public string testStepSequence { get; set; }
        public List<string> roleArray { get; set; }
        public string action { get; set; }
        public string erAttachmentURL { get; set; }
        public string erAttachmentName { get; set; }
        public string expResultImage { get; set; }
        public string testStepId { get; set; }
        public string UserCid { get; set; }
    }
}
