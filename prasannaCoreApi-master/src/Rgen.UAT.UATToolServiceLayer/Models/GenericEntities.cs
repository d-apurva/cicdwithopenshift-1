using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    [DataContract]
    public class Area
    {
        [DataMember]
        public string areaId { get; set; }

        [DataMember]
        public string areaName { get; set; }
    }

    [DataContract]
    public class Role
    {
        [DataMember]
        public string roleId { get; set; }

        [DataMember]
        public string roleName { get; set; }

        [DataMember]
        public string roleDetails { get; set; }

        [DataMember]
        public string projectId { get; set; }

        [DataMember]
        public string isTestingInprogress { get; set; }

        [DataMember]
        public string isTestersAssigned { get; set; }

        [DataMember]
        public string isTestStepsAssigned { get; set; }
    }

    //[DataContract]
    //public class GroupPortfolio
    //{
    //    [DataMember]
    //    public string groupId { get; set; }

    //    [DataMember]
    //    public string groupName { get; set; }

    //    [DataMember]
    //    public List<Portfolio> listPortfolio { get; set; }
    //}

    //[DataContract]
    //public class Portfolio
    //{
    //    [DataMember]
    //    public string portfolioId { get; set; }

    //    [DataMember]
    //    public string portfolioName { get; set; }
    //}
}
