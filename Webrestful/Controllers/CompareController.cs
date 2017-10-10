using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webrestful.Models;

namespace Webrestful.Controllers
{
    public class CompareController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Getrevenuefolioyear(string szHotelDB, string szServer, string szDate1, string szDate2, string szDate3,string szDeviceCode)
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
            var xListTransInfo = Compare.ComparefolioYear(conn,szDate1, szDate2,szDate3, ref szErrMsg);
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
        public HttpResponseMessage Getrevenuefoliomonth(string szHotelDB, string szServer, string szDate1, string szDate2,string szDate3, string szDate4, string szDeviceCode)
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
            var xListTransInfo = Compare.ComparefolioMonth(conn, szDate1,szDate2,szDate3,szDate4, ref szErrMsg);
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
        public HttpResponseMessage GetAgencymonth(string szHotelDB, string szServer, string szDate1, string szDate2, string szDate3, string szDate4, string szDeviceCode)
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
            var xListTransInfo = Compare.GetAgencyM(conn, szDate1, szDate2, szDate3, szDate4, ref szErrMsg);
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
        public HttpResponseMessage GetAgencyyear(string szHotelDB, string szServer, string szDate1, string szDate2, string szDate3, string szDeviceCode)
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
            var xListTransInfo = Compare.GetAgencyY(conn, szDate1, szDate2, szDate3, ref szErrMsg);
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
        public HttpResponseMessage GetBusiness(string szHotelDB, string szServer, string szDate1, string szDeviceCode)
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
            var xListTransInfo = Compare.GetBusiness(conn, szDate1, ref szErrMsg);
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
        public HttpResponseMessage GetAgency(string szHotelDB, string szServer, string mode, string szDate, string szDate2, string szDeviceCode)
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
            var xListTransInfo = Compare.GetAgency(conn, mode, szDate, szDate2, ref szErrMsg);
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
