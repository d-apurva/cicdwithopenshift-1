using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    
    public class GroupPortfolio
    {
      
        public string groupId { get; set; }

       
        public string groupName { get; set; }

      
        public List<Portfolio> listPortfolio { get; set; }
    }

  
    public class Portfolio
    {
      
        public string portfolioId { get; set; }

      
        public string portfolioName { get; set; }
    }
}
