using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace Rgen.UAT.UATToolServiceLayer.Models
{
    
        [DataContract]
        public class TestCase
        {

            [DataMember]
            public string testPassId { get; set; }

            [DataMember]
            public string testCaseId { get; set; }

            [DataMember]
            public string testCaseName { get; set; }

            [DataMember]
            public string testCaseDesp { get; set; }

            [DataMember]
            public string testCaseSeq { get; set; }

            [DataMember]
            public string testCaseETT { get; set; }

            [DataMember]
            public string testCaseDisplayId { get; set; }

            [DataMember]
            public string testcaseflagTester { get; set; }

        }

        [DataContract]
        public class TestCaseCopy
        {
            [DataMember]
            public string testPassId { get; set; }

            [DataMember]
            public string testCaseId { get; set; }

            [DataMember]
            public string destTestPassId { get; set; }
        }

        [DataContract]
        public class TesterRoleNames
        {
            [DataMember]
            public string errorText { get; set; }

            [DataMember]
            public string errorValue { get; set; }

            [DataMember]
            public string testerName { get; set; }

            [DataMember]
            public string roleName { get; set; }
        }

    }