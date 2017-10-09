using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Webrestful.Models;

namespace Webrestful.Controllers
{
    public class HotelAuthenController : ApiController
    {
        //[HttpGet]
        //public HttpResponseMessage CheckAuthenLicense(string szHotelLicense, string szHotelPassword, string szDeviceCode)
        //{
        //    var result = new Response();
        //    int iHotelID = 0;
        //    string szHotelDB = HotelAuthenAPI.CheckHotelLicense(szHotelLicense, szHotelPassword, ref iHotelID);
        //    if (szHotelDB == null)
        //    {
        //        //--- Fail, no do any thing in mobile until connect database.
        //        result.status = -1;
        //        result.dataResult = "Fail Connect Database";
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        //    }
        //    else
        //    {
        //        result.status = 0;
        //        result.dataResult = szHotelDB;    // This parameter will be first parameter of all methods.
        //        result.dataExtra = iHotelID;
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, result);
        //}
        [HttpGet]
        public HttpResponseMessage CheckUserRole(string szHotelDB, string szIPServer, string szUserLogin, string szPassword, string szDeviceCode)
        {
            var result = new Response();
            var conn = DBHelper.ConnectDatabase(szHotelDB, szIPServer);
            if (conn == null)
            {
                result.status = -1;
                result.dataResult = "Fail Connect Database";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }

            string szErrMsg = "";
            var xListTransInfo = HotelAuthenAPI.UserRole(conn, szUserLogin, szPassword, ref szErrMsg);
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
        public HttpResponseMessage VerifyUserNamePassword(string szHotelDB, string szIPServer, string szUserLogin, string szPassword, string szDeviceCode)
        {
            var result = new Response();
            var conn = DBHelper.ConnectDatabase(szHotelDB, szIPServer);
            if (conn == null)
            {
                result.status = -1;
                result.dataResult = "Fail Connect Database";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }

            //--- Check UserName & Password and permission
            string szUserFullName = "";
            int iUserID = HotelAuthenAPI.VerifyUserNamePassword(conn, szUserLogin, szPassword, ref szUserFullName);
            if (iUserID == 0)
            {
                result.status = 1;
                result.dataResult = "UserLogin or Password is not correct";
            }
            else
            {
                result.status = 0;
                result.dataResult = iUserID;
                result.dataExtra = szUserFullName;
            }

            DBHelper.CloseConnection(conn);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetCurrentHotelDate(string szHotelDB, string szServer,string szDeviceCode)
        {
            var result = new Response();
            var conn = DBHelper.ConnectDatabase(szHotelDB, szServer);
            if (conn == null)
            {
                result.status = -1;
                result.dataResult = "Fail Connect Database";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }

            //--- Check UserName & Password and permission
            DateTime dtHotelDate = HotelAuthenAPI.GetCurrentHotelDate(conn);
            result.status = 0;
            result.dataResult = dtHotelDate.ToString("yyyy-MM-dd");

            DBHelper.CloseConnection(conn);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        // DELETE: api/HotelAuthen/5
        [HttpGet]
        public HttpResponseMessage Getdate(string szHotelDB,string szServer)
        {
            var result = new Response();
            var conn = DBHelper.ConnectDatabase(szHotelDB,szServer);
            if (conn == null)
            {
                result.status = -1;
                result.dataResult = "Fail Connect Database";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }

            string szErrMsg = "";
            var xListTransInfo = Date.CheckDate(conn, ref szErrMsg);
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
