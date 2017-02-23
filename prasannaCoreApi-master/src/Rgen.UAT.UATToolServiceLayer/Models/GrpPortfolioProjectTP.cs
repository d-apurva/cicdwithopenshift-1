using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rgen.UAT.UATToolServiceLayer.Models
{

    public class GrpPortfolioProjectTP : GroupPortfolioProject
    {
        // internal static object groupPortfolioProjectTestPassResult;
        #region Test Pass Defination


        public class TestPass
        {

            public string testpassId { get; set; }
            public string testpassName { get; set; }
            public string testPassDescription { get; set; }
            public string TestManager { get; set; }

            public List<TestPassTesterRolesNew> tester { get; set; }

        }

        #endregion

        //public string groupName { get; set; }
        //public string portfolioId { get; set; }
        //public string portfolioName { get; set; }
        //public string projectId { get; set; }
        //public string projectName { get; set; }
        //public string projectVersion { get; set; }
        //public string projectStartDate { get; set; }
        //public string projectEndDate { get; set; }
        //public string projectDescription { get; set; }



        public List<TestPass> testPassList { get; set; }


        public List<Role> roleList { get; set; }


        public string leadEmailId { get; set; }
        public string ProjectleadName { get; set; }

        //public List<GrpPortfolioProjectTP> lstGrpPortfolioProjectTP { get; set; }
        //public List<GrpPortfolioProjectTP> lstGrpPortfolioProjectTPNew { get; set; }
    }



}
