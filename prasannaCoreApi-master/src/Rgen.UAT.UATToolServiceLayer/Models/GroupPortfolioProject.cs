#region Copyright ©2016, RGen Software Solutions (I) Pvt. Ltd. - All Rights Reserved
/* --------------------------------------------------------------------------------- *
*                      RGen Software Solutions (I) Pvt. Ltd.                         *
*                      Copyright ©2016 - All Rights reserved                         *
*                                                                                    *
*                                                                                    *
*        Copyright © 2016 by RGen Software Solutions | www.rgensoft.com/             *
*        All rights reserved. No part of this publication may be reproduced,         *
*        stored in a retrieval system or transmitted, in any form or by any          *
*        means, photocopying, recording or otherwise, without prior written          *
*        consent of  RGen Software Solutions (I) Pvt. Ltd.                           *
*                                                                                    *
*                                                                                    *
* ---------------------------------------------------------------------------------  */
#endregion Copyright ©2016, RGen Software Solutions (I) Pvt. Ltd. - All Rights Reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class GroupPortfolioProject
    {
        public string groupId { get; set; }
        public string groupName { get; set; }
        public string portfolioId { get; set; }
        public string portfolioName { get; set; }
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string projectVersion { get; set; }
        public string projectStartDate { get; set; }
        public string projectEndDate { get; set; }
        public string projectDescription { get; set; }
    }

    /// <summary>
    /// Model for filling dropdownbox of Project, testpass, Testcase & role
    /// </summary>
    public class GroupPortfolioProjectTestPass : GroupPortfolioProject
    {
        #region Test Pass Defination

        public class TestPass
        {
            public string testpassId { get; set; }
            public string testpassName { get; set; }
            public string testCaseList { get; set; }
            public string ListRoles { get; set; }
            public string testingType { get; set; }

        }

        #endregion

        #region Role Defination

        public class Role
        {
            public string roleId { get; set; }
            public string roleName { get; set; }
            public string roleDetails { get; set; }
            public string isTestersAssigned { get; set; }
        }

        #endregion

        #region Test Case Defination

        public class TestCase
        {
            public string testCaseId { get; set; }
            public string testCaseName { get; set; }
        }

        #endregion

        public List<GroupPortfolioProjectTestPass.TestPass> testPassList { get; set; }
        public string roleList { get; set; }
        public string stakeholderList { get; set; }
        public string leadEmailId { get; set; }

    }

    public class StakeholderList
    {
        public string stakeHolderEmail { get; set; }
    }
}
