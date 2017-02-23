using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsUserDataTable
    {
        public int spUser_Id { get; set; }
        public string spUser_name { get; set; }
        public string spUser_Alias { get; set; }
        public string spUser_EmailId { get; set; }
        public int spUser_desgnID { get; set; }
        
    }


    public class MyTesterType
    {
        public int RowID { get; set; }
        public int Tester_ID { get; set; }
        public int TestPass_ID { get; set; }
        public int Area_ID { get; set; }
        public int Role_ID { get; set; }
        public int User_ID { get; set; }
    }

    public class clsTesterByUserIdTestPassId
    {
        public int Tester_ID { get; set; }
        public int TestPass_ID { get; set; }
        public int Area_ID { get; set; }
        public int Role_ID { get; set; }
        public int User_ID { get; set; }
    }

    public class myDataCollection : List<clsUserDataTable>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                new SqlMetaData("spUser_Id", SqlDbType.Int),
                new SqlMetaData("spUser_name", SqlDbType.VarChar, 500),
                new SqlMetaData("spUser_Alias", SqlDbType.VarChar, 500),
                new SqlMetaData("spUser_EmailId", SqlDbType.VarChar, 500),
                new SqlMetaData("spUser_desgnID", SqlDbType.Int)
                );

            foreach (clsUserDataTable data in this)
            {
                ret.SetInt32(0, data.spUser_Id);
                ret.SetString(1, data.spUser_name);
                ret.SetString(2, data.spUser_Alias);
                ret.SetString(3, data.spUser_EmailId);
                ret.SetInt32(4, data.spUser_desgnID);
                yield return ret;
            }
        }
    }



    public class myTesterTypeCollection:List<MyTesterType>,IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
                
                new SqlMetaData("RowID",  SqlDbType.Int),
                new SqlMetaData("Tester_ID", SqlDbType.Int),
                new SqlMetaData("TestPass_ID",SqlDbType.Int),
                new SqlMetaData("Area_ID", SqlDbType.Int),
                new SqlMetaData("Role_ID", SqlDbType.Int),
                new SqlMetaData("User_ID", SqlDbType.Int)
                );

            foreach (MyTesterType data in this)
            {
                ret.SetInt32(0, data.RowID);
                ret.SetInt32(1, data.Tester_ID);
                ret.SetInt32(2, data.TestPass_ID);
                ret.SetInt32(3, data.Area_ID);
                ret.SetInt32(4, data.Role_ID);
                ret.SetInt32(5, data.User_ID);
                yield return ret;
            }
        }

    }

    public class getTesterByUserIdTestPassId : List<clsTesterByUserIdTestPassId>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(
             
                new SqlMetaData("Tester_ID", SqlDbType.Int),
                new SqlMetaData("TestPass_ID", SqlDbType.Int),
                new SqlMetaData("Area_ID", SqlDbType.Int),
                new SqlMetaData("Role_ID", SqlDbType.Int),
                new SqlMetaData("User_ID", SqlDbType.Int)
                );

            foreach (clsTesterByUserIdTestPassId data in this)
            {
              
                ret.SetInt32(0, data.Tester_ID);
                ret.SetInt32(1, data.TestPass_ID);
                ret.SetInt32(2, data.Area_ID);
                ret.SetInt32(3, data.Role_ID);
                ret.SetInt32(4, data.User_ID);
                yield return ret;
            }
        }

    }

}
