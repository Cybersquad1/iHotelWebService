using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Business_source
    {
        public static List<Source> GetSource(MySqlConnection conn, ref string szErrMsg)
        {
            try
            {

                var xAryTransCheckIn = new List<Source>();
                string BusinessType = "SELECT GuestChannelID, GuestChannelName FROM GuestChannel ORDER BY Ordering;";
                DataTable dtFIT = DBHelper.QueryListData(conn, BusinessType);

                for (int i = 0; i < 15; i++)
                {
                    var xTrans = new Source();
                    xTrans.BusinessSource = dtFIT.Rows[i]["GuestChannelName"].ToString();
                    xTrans.SourceID = dtFIT.Rows[i]["GuestChannelID"].ToString();
                    xAryTransCheckIn.Add(xTrans);
                }
                return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<NoDefine> GetNoDefine(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var NoDefineList = new List<NoDefine>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=0 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=0  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=0 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=0 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";
                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET NO DEFINE
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new NoDefine();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        NoDefineList.Add(xTrans);
                    }
                }
                return NoDefineList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<WalkIn> GetWalkIn(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var WalkInList = new List<WalkIn>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=1 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=1  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=1 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=1 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";
                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET WALK IN
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new WalkIn();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        WalkInList.Add(xTrans);
                    }
                }
                return WalkInList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<Reservation> GetReservation(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var ReservationList = new List<Reservation>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=2 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=2  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=2 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=2 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Reservation
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new Reservation();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        ReservationList.Add(xTrans);
                    }
                }
                return ReservationList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<AgencyReservation> GetAgencyReservation(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var AgencyReservationList = new List<AgencyReservation>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=3 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=3  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=3 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=3 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET AgencyReservation
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new AgencyReservation();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        AgencyReservationList.Add(xTrans);
                    }
                }
                return AgencyReservationList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<History> GetHistory(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var HistoryList = new List<History>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=4 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=4  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=4 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=4 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET History
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new History();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        HistoryList.Add(xTrans);
                    }
                }
                return HistoryList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<Government> GetGovernment(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var GovernmentList = new List<Government>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=5 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=5  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=5 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=5 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Government
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new Government();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        GovernmentList.Add(xTrans);
                    }
                }
                return GovernmentList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<WebBooking> GetWebBooking(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var WebBookingList = new List<WebBooking>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=6 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=6  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=6 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=6 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET WebBooking
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new WebBooking();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        WebBookingList.Add(xTrans);
                    }
                }
                return WebBookingList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<Association> GetAssociation(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var AssociationList = new List<Association>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=7 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=7  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=7 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=7 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Association
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new Association();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        AssociationList.Add(xTrans);
                    }
                }
                return AssociationList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<Complimentary> GetComplimentary(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var ComplimentaryList = new List<Complimentary>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=8 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=8  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=8 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=8 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Complimentary
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new Complimentary();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        ComplimentaryList.Add(xTrans);
                    }
                }
                return ComplimentaryList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<Corporation> GetCorporation(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var CorporationList = new List<Corporation>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=9 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=9  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=9 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=9 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Corporation
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new Corporation();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        CorporationList.Add(xTrans);
                    }
                }
                return CorporationList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<DayUse> GetDayUse(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var DayUseList = new List<DayUse>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=10 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=10  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=10 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=10 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET DayUse
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new DayUse();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        DayUseList.Add(xTrans);
                    }
                }
                return DayUseList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<EMB> GetEMB(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var EMBList = new List<EMB>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=11 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=11  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=11 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=11 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET EMB
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new EMB();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        EMBList.Add(xTrans);
                    }
                }
                return EMBList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<HouseUse> GetHouseUse(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var HouseUseList = new List<HouseUse>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=12 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=12  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=12 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=12 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET HouseUse
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new HouseUse();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        HouseUseList.Add(xTrans);
                    }
                }
                return HouseUseList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<GuideTaxi> GetGuideTaxi(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var GuideTaxiList = new List<GuideTaxi>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=13 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=13  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=13 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=13 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET GuideTaxi
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new GuideTaxi();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        GuideTaxiList.Add(xTrans);
                    }
                }
                return GuideTaxiList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<VIP> GetVIP(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                //string enddate = dtHotelDate.ToString(format, cur);

                //int month = dtHotelDate.Month;
                //int year = dtHotelDate.Year;

                var VIPList = new List<VIP>();

                string selectYear = dtHotelDate;
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=14 AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=14  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID=14 AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=14 AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";

                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET VIP
                if (dtFIT.Rows.Count == 0)
                {
                    //
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        var xTrans = new VIP();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        xTrans.sum = dtFIT.Rows[j]["SUM(SumRoom)"].ToString();
                        VIPList.Add(xTrans);
                    }
                }
                return VIPList;


            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }

    }
}