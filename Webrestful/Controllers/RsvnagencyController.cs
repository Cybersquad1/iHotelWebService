using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webrestful.Models;

namespace Webrestful.Controllers
{
    public class RsvnagencyController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Getrsvnagency(string szHotelDB, string szServer,string szDate1, string szDate2, string szDate3, string szDeviceCode)
        {
            var result = new Response();
            var conn = DBHelper.ConnectDatabase(szHotelDB, szServer);
            if (conn == null)
            {
                result.status = -1;
                result.dataResult = "Fail Connect Database";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }

            string szErrMsg = "";
            var xListTransInfo = Rsvnagency.GetAgency(conn, Convert.ToDateTime(szDate1), Convert.ToDateTime(szDate2), Convert.ToDateTime(szDate3), ref szErrMsg);
            if (xListTransInfo == null)
            {
                result.status = 1;
                result.dataResult = szErrMsg;
            }
            else
            {
                result.status = 0;
                result.dataResult = xListTransInfo;
            }

            DBHelper.CloseConnection(conn);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // DELETE: api/Rsvnagency/5
        public void Delete(int id)
        {
        }
    }
}
