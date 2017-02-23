using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class ClsTesterTypeDataTable
    {

        public int RowID { get; set; }
        public int Tester_ID { get; set; }
        public int TestPass_ID { get; set; }
        public int Area_ID { get; set; }
        public int Role_ID { get; set; }
        public int User_ID { get; set; }
    }

    public class TesterTypeCollection : List<ClsTesterTypeDataTable>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord ret = new SqlDataRecord(


               
                new SqlMetaData("RowID", SqlDbType.Int),
                new SqlMetaData("Tester_ID", SqlDbType.Int),
                new SqlMetaData("TestPass_ID", SqlDbType.Int),
                new SqlMetaData("Area_ID", SqlDbType.Int),
                new SqlMetaData("Role_ID", SqlDbType.Int),
                new SqlMetaData("User_ID", SqlDbType.Int)

           );
            foreach (ClsTesterTypeDataTable data in this)
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



}
