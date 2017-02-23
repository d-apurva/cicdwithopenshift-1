using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rgen.UAT.UATToolServiceLayer.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using System.Data.Common;
namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public  class clsUtility :Controller
    {
      

        public  string GetLoggedInUserSPUserId()
        {
            string spUserId = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Request.Headers["LoggedInUserSPUserId"]))
                {
                    string customHeader = HttpContext.Request.Headers["LoggedInUserSPUserId"];

                    if (!string.IsNullOrEmpty(customHeader))
                        spUserId = customHeader;
                }

                if (string.IsNullOrEmpty(spUserId))
                    return "Invalid Client Session!!!";
            }
            catch (Exception) { }

            return spUserId;
        }
    }
}
