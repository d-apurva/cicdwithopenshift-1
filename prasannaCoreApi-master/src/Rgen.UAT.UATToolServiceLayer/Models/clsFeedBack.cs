using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsFeedBack
    {
        public string feedbackId { get; set; }
        public string testpassId { get; set; }
        public string testpassName { get; set; }
        public string testcaseId { get; set; }
        public string testcaseName { get; set; }
        public string userId { get; set; }
        public string testerName { get; set; }
        public List<FeedbackTesterDetail> listTesterDetail { get; set; }
    }
   
    public class FeedbackTesterDetail
    {
        public string roleId { get; set; }
        public string roleName { get; set; }
        public string testStepId { get; set; }
        public string testStepName { get; set; }
        public string expectedResult { get; set; }
        public string testplanId { get; set; }
        public string status { get; set; }
        public string actualResult { get; set; }
        public string tsFeedback { get; set; }
        public string fBType { get; set; }
        public string tpTcRating { get; set; }
        public string tpTcFeedback { get; set; }
        public string feedbackRatingId { get; set; }
    }
    public class FeedbackUnique
    {
        public int TPassId { get; set; }
        public int TCaseId { get; set; }
        public int spUserId { get; set; }
        public int RoleId { get; set; }
    }
    public class TestingFR
    {
        public string feedbackId { get; set; }
        public string testPassId { get; set; }
        public string testCaseId { get; set; }
        public string userId { get; set; }
        public string roleId { get; set; }
        public string rating { get; set; }
        public string feedback { get; set; }
    }

    public class FeedBackTestSteps
    {
        public string feedback { get; set; }
        public string testStepPlanId { get; set; }
    }
}

