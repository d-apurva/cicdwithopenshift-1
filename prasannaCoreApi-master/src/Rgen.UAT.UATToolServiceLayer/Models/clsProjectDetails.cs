using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsProjectDetails
    {
        public string groupId { get; set; }


        public string portfolioId { get; set; }


        public string projectName { get; set; }


        public string projectId { get; set; }


        public string startDate { get; set; }


        public string endDate { get; set; }


        public string projectStatus { get; set; }


        public string projectVersion { get; set; }


        public string description { get; set; }


        public string appUrl { get; set; }

        public string aliasAppUrl { get; set; }

        public string projectUrl { get; set; }

        public string projectAliasUrl { get; set; }


        public string projectDisplayID { get; set; }


        public string testpass_Count { get; set; }


        public string testpass_MinDate { get; set; }


        public string testpass_MaxDate { get; set; }



        public List<ProjectUser> listProjectUsers { get; set; }
    }
   
}
