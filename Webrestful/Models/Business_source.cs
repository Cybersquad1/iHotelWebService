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

                for (int i = 0; i < dtFIT.Rows.Count; i++)
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
        public static List<NoDefine> GetEachSource(MySqlConnection conn, string dtHotelDate, string id,ref string szErrMsg)
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
                string SQL = "SELECT AtMonth, SUM(SumRoom) FROM (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransRoomInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_TypeID<>10 AND B.GuestChannelID="+id+" AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID="+id+"  AND B.Rsvn_StatusID IN (1,2)  AND B.Rsvn_ArrivalDate>='" + selectYear + "'  GROUP BY AtMonth, B.GuestChannelID  UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.TransactionID) AS SumRoom  FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID  WHERE C.GuestChannelID="+id+" AND YEAR(A.Trans_Date)='" + selectYear + "'  GROUP BY AtMonth, C.GuestChannelID  UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  COUNT(A.Rsvn_TransID) AS SumRoom  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID="+id+" AND  YEAR(A.Rsvn_Date)='" + selectYear + "'  AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  )AS A GROUP BY AtMonth";
                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET NO DEFINE
                if (dtFIT.Rows.Count == 0)
                {
                    var xTrans = new NoDefine();
                    xTrans.month = null;
                    xTrans.sum = null;
                    NoDefineList.Add(xTrans);
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
        public static List<NoDefine> GetEachCharge(MySqlConnection conn, string dtHotelDate, string id, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");

                var GovernmentList = new List<NoDefine>();

                string selectYear = dtHotelDate;
                string SQL = "Select Atmonth, Sum(TotalPrice) From (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID, SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_TypeID<>10 AND B.GuestChannelID="+id+" AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))) AS TotalPrice  FROM RsvnRoomInfo A  JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID="+id+" AND B.Rsvn_StatusID IN (1,2) AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID, SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID WHERE C.GuestChannelID="+id+" AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))) AS TotalPrice  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID="+id+" AND  YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  ) As A Group by Atmonth";
                DataTable dtFIT = DBHelper.QueryListData(conn, SQL);
                //GET Government
                if (dtFIT.Rows.Count == 0)
                {
                    var xTrans = new NoDefine();
                    xTrans.month = null;
                    xTrans.sum = null;
                    GovernmentList.Add(xTrans);
                }
                else
                {
                    for (int j = 0; j < dtFIT.Rows.Count; j++)
                    {
                        float sum = 0;
                        var xTrans = new NoDefine();
                        xTrans.month = dtFIT.Rows[j]["AtMonth"].ToString();
                        sum = float.Parse(dtFIT.Rows[j]["SUM(TotalPrice)"].ToString());
                        xTrans.sum = sum.ToString("0.");
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

    }
}