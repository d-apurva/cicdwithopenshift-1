using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    internal class clsUatClient
    {
        #region Private Member Functions
        clsDbContext _context;

        public clsUatClient(clsDbContext cd)
        {
            _context = cd;
        }

        private List<dynamic> _GetSchemaName(string appURL)
        {
           // Uri uri = new Uri(appURL);
           // string requested = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
            List<dynamic> clientInfo = new List<dynamic>();
           // appURL = appURL.Substring(0, appURL.LastIndexOf('/'));
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "UAT.[spGetSchemaByClientAppURL]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ClientAppURL", SqlDbType.VarChar, 500) { Value = appURL });
                cmd.Parameters.Add(new SqlParameter("@Ret_Parameter", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();



                try
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            for (var iFiled = 0; iFiled < dataReader.FieldCount; iFiled++)
                            {
                                dataRow.Add(dataReader.GetName(iFiled), dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled]);
                                clientInfo.Add(Convert.ToString(dataReader.IsDBNull(iFiled) ? null : dataReader[iFiled]));//Schema Name

                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    return null;
                }
                if (clientInfo == null)
                {
                    return null;
                }
                else
                    return clientInfo;
            }




        }

        private string _GetAppURL(string requestURL)
        {
            string appURL = string.Empty;

            string[] url = requestURL.Split('/');
            foreach (string s in url)
            {
                if (s.ToLower().Contains(".aspx"))
                    break;

                if (s.ToLower() == "sitepages")
                    break;

                appURL += s.ToLower() + "/";
            }

            return appURL.Substring(0, appURL.Length - 1);
        }

        #endregion

        #region Internal Member Functions

        internal string GetClientSchema(string frontEndUrl)
        { 
            List<string> listClientInfo = new List<string>();

            string schemaName = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(frontEndUrl))
                {
                    var listdata = _GetSchemaName(_GetAppURL(frontEndUrl));
                    schemaName = listdata[0];
                    string clientInfo = listdata[1] + "|" + listdata[2] + "~";
                }

                if (string.IsNullOrEmpty(schemaName))
                { }
            }
            catch (Exception ex) { }
            return schemaName;
        }
        
        internal string GetClientInfo(string requestURL)
        {
            List<string> listClientInfo = new List<string>();

            string clientInfo = string.Empty;

            try
            {

                var q = _GetSchemaName(_GetAppURL(requestURL));

                clientInfo = q[1] + "|" + q[2];

            }
            catch (Exception) { }

            return clientInfo;
        }

      

        #endregion
    }
}
