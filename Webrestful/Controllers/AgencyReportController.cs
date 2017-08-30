using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webrestful.Models;

namespace Webrestful.Controllers
{
    public class AgencyReportController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAgencyName(string szHotelDB, string szServer,string szDeviceCode)
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
            var xListTransInfo = AgencyReport.GetAgencyName(conn, ref szErrMsg);
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
        [HttpGet]
        public HttpResponseMessage GetAgencyNight(string szHotelDB, string szServer,string szDate, string szDate2, string mode, string id, string szDeviceCode)
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
            var xListTransInfo = AgencyReport.GetAgencyNight(conn, szDate, szDate2, mode, id, ref szErrMsg);
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
       
        [HttpGet]
        public HttpResponseMessage GetAgencyCharge(string szHotelDB, string szServer,string szDate, string szDate2, string mode, string id, string szDeviceCode)
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
            var xListTransInfo = AgencyReport.GetAgencyCharge(conn, szDate, szDate2, mode, id, ref szErrMsg);
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
       
    }
}
