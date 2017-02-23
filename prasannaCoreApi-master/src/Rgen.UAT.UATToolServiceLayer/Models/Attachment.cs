using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
   
    public class data_coll
    {
        public int col_attachmentId { get; set; }
        public string col_testStepId { get; set; }
        public int col_testCaseId { get; set; }
    }


    public class Attachment
    {
        public string fileName { get; set; }
        public string AttachmentId { get; set; }
        public string filePath { get; set; }
        
        public string fileIndex { get; set; }
    }

    
    public class AttachmentDetail
    {
       
        public string testCaseId { get; set; }
        
        public string testStepId { get; set; }
        
        public string testStepName { get; set; }
        
        public string testerSpUserId { get; set; }
        
        public string testerName { get; set; }
        
        public string roleId { get; set; }
        
        public string roleName { get; set; }
        
        public string testStepPlanId { get; set; }
        
        public List<Attachment> listExpectedAttach { get; set; }
        
        public List<Attachment> listActualAttach { get; set; }

    }

    public class Attachment_dat
    {

        public string TestStepID { get; set; }
        public string AttachmentName { get; set; }
        public string Description { get; set; }
        public string ResultType { get; set; }
        public string ProjectID { get; set; }
        public string TestPassID { get; set; }
        public string TestCaseID { get; set; }
        public string ActualResult { get; set; }
        public string StatementType { get; set; }
        public int AttachmentId { get; set; }
    }

    public class Attachment_append
    {
        public int attID { get; set; }
        public string testPassId { get; set; }
        public string fileName { get; set; }
        public string fileUrl { get; set; }
        public string Description { get; set; }
        public int FileIndex { get; set; }
        public string testStepId { get; set; }
        public string fileType { get; set; }
        public string isDelete { get; set; }
        public int TestStepPlanID { get; set; }
        public string SchemaName { get; set; }
        public string StatementType { get; set; }
    }

    public class clsTestStepIdTableDataTable
    {
        public int RowID { get; set; }
        public int TestStep_ID { get; set; }
    }

    public class tsDataCollection : List<clsTestStepIdTableDataTable>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("RowID", SqlDbType.Int),
                new SqlMetaData("TestStep_ID", SqlDbType.Int)
                );

            foreach (clsTestStepIdTableDataTable data in this)
            {
                ret.SetInt32(0, data.RowID);
                ret.SetInt32(1, data.TestStep_ID);
                yield return ret;
            }
        }
    }
    public class AttachmentData
    {
        public string testCaseId { get; set; }

        public string testStepId { get; set; }

        public string testStepName { get; set; }

        public string testerSpUserId { get; set; }

        public string testerName { get; set; }

        public string roleId { get; set; }

        public string roleName { get; set; }

        public string Description { get; set; }

        public List<Attachment> listExpectedAttach { get; set; }

        public List<Attachment> listActualAttach { get; set; }

        public string testStepPlanId { get; set; }
        public string AttachmentID { get; set; }
    }
    public class Attachment_view
    {
        public string fileData { get; set; }
    }
}
