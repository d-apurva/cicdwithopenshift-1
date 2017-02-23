using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class TestPassTesterRolesNew
    {
        #region Role Defination

        [DataContract]
        public class Role
        {
            [DataMember]
            public string roleId { get; set; }

            [DataMember]
            public string roleName { get; set; }

            [DataMember]
            public string isTestingInprogress { get; set; }

            [DataMember]
            public string isTestStepsAssigned { get; set; }
        }

        #endregion

        [DataMember]
        public string testPassId { get; set; }

        /// <summary>
        /// Tester ID
        /// </summary>
        [DataMember]
        public string spUserId { get; set; }

        [DataMember]
        public string oldTesterspUserId { get; set; }

        /// <summary>
        /// Tester Name
        /// </summary>
        [DataMember]
        public string testerName { get; set; }

        [DataMember]
        public string testerAlias { get; set; }

        [DataMember]
        public string testerEmail { get; set; }

        [DataMember]
        public string areaId { get; set; }

        [DataMember]
        public string areaName { get; set; }

        [DataMember]
        public List<TestPassTesterRolesNew.Role> roleList { get; set; }

        [DataMember]
        public List<string> roleArray { get; set; }

        [DataMember]
        public string action { get; set; }

        [DataMember]
        public string testerID { get; set; }

        //[DataMember]
        //public DataTable TesterTypeTable { get; set; }
        public class TesterRole
        {
            public List<TestPassTesterRolesNew> lstTesterRole { get; set; }
        }
    }
    
}

