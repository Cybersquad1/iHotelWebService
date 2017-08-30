using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webrestful.Models;

namespace Webrestful.Controllers
{
    public class Revenue_NationalityController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetNationality(string szHotelDB, string szServer,string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Revenue_Nationality.GetNationNight(conn, szDate, szDate2, ref szErrMsg);
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
        public HttpResponseMessage GetTransID(string szHotelDB, string szServer,string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Revenue_Nationality.GetTransID(conn, szDate, szDate2, ref szErrMsg);
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
       public HttpResponseMessage GetRR(string szHotelDB, string szServer, string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Revenue_Nationality.GetRR(conn, szDate, szDate2, ref szErrMsg);
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
        public HttpResponseMessage GetOther(string szHotelDB, string szServer, string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Revenue_Nationality.GetOther(conn, szDate, szDate2, ref szErrMsg);
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
        public HttpResponseMessage GetABF(string szHotelDB,string szServer, string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Revenue_Nationality.GetABF(conn, szDate, szDate2, ref szErrMsg);
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