using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsTester
    {


        [DataContract]
        public class Tester
        {
            /// <summary>
            /// Tester ID
            /// </summary>
            [DataMember]
            public string testerId { get; set; }

            /// <summary>
            /// Tester Name
            /// </summary>
            [DataMember]
            public string testerName { get; set; }

            [DataMember]
            public string areaId { get; set; }

            [DataMember]
            public string areaName { get; set; }

            
            [DataMember]
            public List<Role> roles { get; set; }

        }


        [DataMember]
        public string group { get; set; }

        [DataMember]
        public string portfolio { get; set; }


        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string projectName { get; set; }

        [DataMember]
        public string projectVersion { get; set; }

        [DataMember]
        public List<Role> roles { get; set; }


        [DataMember]
        public string testpassId { get; set; }

        [DataMember]
        public string testpassName { get; set; }



        [DataMember]
        public List<Tester> tester { get; set; }
    }
}
