using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    //public class Project
    //{
    //}

    public class ProjectDetails
    {
        [DataMember]
        public string groupId { get; set; }

        [DataMember]
        public string portfolioId { get; set; }

        [DataMember]
        public string projectName { get; set; }

        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string startDate { get; set; }

        [DataMember]
        public string endDate { get; set; }

        [DataMember]
        public string projectStatus { get; set; }

        [DataMember]
        public string projectVersion { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string appUrl { get; set; }
        [DataMember]
        public string aliasAppUrl { get; set; }
        [DataMember]
        public string projectUrl { get; set; }
        [DataMember]
        public string projectAliasUrl { get; set; }

        [DataMember]
        public string projectDisplayID { get; set; }

        [DataMember]
        public string testpass_Count { get; set; }

        [DataMember]
        public string testpass_MinDate { get; set; }

        [DataMember]
        public string testpass_MaxDate { get; set; }


        [DataMember]
        public List<ProjectUser> listProjectUsers { get; set; }
    }


    [DataContract]
    public class ProjectUser
    {
        [DataMember]
        public string userName { get; set; }
        [DataMember]
        public string alias { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string spUserId { get; set; }
        [DataMember]
        public string securityId { get; set; }
    }
}
