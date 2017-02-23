using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class Configuration
    {
    }

    public class SummarySetting
    {
        public string Field { get; set; }
        public string ActualCount { get; set; }
    }

    public class Environment_Tab
    {
        public string actualUrl { get; set; }
        public string aliasUrl { get; set; }
        public string envID { get; set; }
        public string projectId { get; set; }
        public string testPassId { get; set; }
    }

    public class GeneralSetting
    {
        public string testPassId { get; set; }
        public string testingType { get; set; }//0,1,2
        public string feedbackType { get; set; }//0,1,2
        public string testStepUnique { get; set; }//Y,N
        public string testingPage { get; set; }//0,1,2,3
    }

    public class UserSetting
    {
        public string userSettingId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string testPassId { get; set; }
        public string projectId { get; set; }
        public string envId { get; set; }
        public string actualUrl { get; set; }
        public string aliasUrl { get; set; }
    }

    public class ConfigurationDocuments
    {
        public string configurationDocId { get; set; }
        public string projectId { get; set; }
        public string attachmentName { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileDescription { get; set; }
    }

    public class ProcessDetail
    {
        public string projectId { get; set; }
        public string processDetailId { get; set; }
        public string asIs { get; set; }
        public string toBe { get; set; }
        public string asIsDescription { get; set; }
        public string toBeDescription { get; set; }
    }

    public class UserSettingReqObj
    {
        public string testPassId { get; set; }
        public string projectId { get; set; }
        public string UserSettingString { get; set; }
        public string userId { get; set; }
    }

    public class ConfigurationAttachment
    {
        public string AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string projectId { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

}
