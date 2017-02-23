using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsTestterList
    {
      
            public string testerId { get; set; }
           
            public string testerName { get; set; }
         
            public string roleId { get; set; }
         
            public string roleName { get; set; }

        

    }

    public class TesterList
    {
        public List<clsTestterList> lstTesterList { get; set; }
    }
}
