using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Arrival_Departure
    {
        public static List<SetArrival_Departure> GetArrival(MySqlConnection conn, DateTime dtHotelDate,  ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<SetArrival_Departure>();
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string date = dtHotelDate.ToString(format, cur);

                string sqlstring = "SELECT * FROM (" +
                       "SELECT A.Rsvn_ArrivalDate, A.Rsvn_DepartureDate, D.RoomName, C.RoomTypeName, " +
                       "E.GuestName, F.AgencyName, 1 AS GuestType, A.Rsvn_Remark, C.RoomTypeID, " +
                       "A.Rsvn_PaxAdult, A.Rsvn_PaxChild, B.Rsvn_ExtraBedPrice, A.AgencyRefNo, " +
                       "B.Rsvn_AbfInc, B.Rsvn_RoomPrice, B.Rsvn_AbfPrice " +
                       "FROM RsvnTransaction A " +
                       "JOIN RsvnRoomInfo B ON A.Rsvn_TransID=B.Rsvn_TransID " +
                       "LEFT JOIN RoomType C ON A.Rsvn_RoomTypeID=C.RoomTypeID " +
                       "LEFT JOIN RoomNo D ON B.Rsvn_RoomID=D.RoomNoID " +
                       "JOIN GuestInfo E ON A.GuestID=E.GuestID " +
                       "LEFT JOIN AgencyInfo F ON A.Rsvn_AgencyID=F.AgencyID " +
                       "WHERE (A.Rsvn_StatusID=1 || A.Rsvn_StatusID=2) AND A.Rsvn_ArrivalDate = '" + date + "' " +
                       "GROUP BY A.Rsvn_TransID " +
                       "UNION " +
                       "SELECT R.Rsvn_ArrivalDate, R.Rsvn_DepartureDate, V.RoomName, U.RoomTypeName, " +
                       "S.GuestName, X.AgencyName, 2 AS GuuestType, R.Rsvn_Remark, U.RoomTypeID, " +
                       "S.Rsvn_PaxAdult, S.Rsvn_PaxChild, T.Rsvn_ExtraBedPrice, R.AgencyRefNo, " +
                       "T.Rsvn_AbfInc, T.Rsvn_RoomPrice, T.Rsvn_AbfPrice " +
                       "FROM RsvnTransaction R " +
                       "JOIN RsvnGroupInfo S ON R.Rsvn_TransID=S.Rsvn_TransID " +
                       "JOIN RsvnGroupDetail T ON T.Rsvn_TransID=S.Rsvn_TransID AND T.Rsvn_GroupID=S.Rsvn_GroupID " +
                       "LEFT JOIN RoomType U ON S.Rsvn_RoomTypeID=U.RoomTypeID " +
                       "LEFT JOIN RoomNo V ON T.Rsvn_RoomID=V.RoomNoID " +
                       "JOIN GuestInfo W ON R.GuestID=W.GuestID " +
                       "LEFT JOIN AgencyInfo X ON R.Rsvn_AgencyID=X.AgencyID " +
                       "WHERE R.Rsvn_StatusID<>99 AND R.Rsvn_ArrivalDate = '" + date + "' " +
                       ") AS A " +
                       "ORDER BY AgencyName, RoomName";
                string matcur = ("N");
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var xTrans = new SetArrival_Departure();
                    xTrans.AgencyName = dt.Rows[i]["AgencyName"].ToString();
                    xTrans.Roomno = dt.Rows[i]["RoomName"].ToString();
                    xTrans.Roomtype = dt.Rows[i]["RoomTypeName"].ToString();
                    decimal ABF = (dt.Rows[i]["Rsvn_AbfPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["Rsvn_AbfPrice"].ToString());
                    string paxa = dt.Rows[i]["Rsvn_PaxAdult"].ToString();
                    string paxc = dt.Rows[i]["Rsvn_PaxChild"].ToString();
                    int totalPax = int.Parse(paxa) + int.Parse(paxc);
                    decimal totalABF = ABF * totalPax;
                    xTrans.Pax = paxa + "/" + paxc;
                    xTrans.ABF = (ABF * totalPax).ToString(matcur);
                    //xTrans.ABF  = ABF.ToString(matcur);
                    if (int.Parse(dt.Rows[i]["Rsvn_AbfInc"].ToString()) == 1)
                    {
                        decimal R_Rate = (decimal.Parse(dt.Rows[i]["Rsvn_RoomPrice"].ToString())) - totalABF;
                        xTrans.R_Rate = R_Rate.ToString(matcur);
                    }
                    else
                    {
                        decimal R_Rate = (dt.Rows[i]["Rsvn_RoomPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["Rsvn_RoomPrice"].ToString());
                        xTrans.R_Rate = R_Rate.ToString(matcur);
                    }             
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

        public static List<SetArrival_Departure> GetDeparture(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<SetArrival_Departure>();
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string date = dtHotelDate.ToString(format, cur);

                string sqlstring = "SELECT * FROM (" +
                       "SELECT A.Trans_ArrivalDate, A.Trans_DepartureDate, D.RoomName, C.RoomTypeName, " +
                       "E.GuestName, F.AgencyName, 1 AS GuestType, A.Trans_Remark, C.RoomTypeID, " +
                       "A.Trans_PaxAdult, A.Trans_PaxChild, B.Trans_ExtraBedPrice, A.AgencyRefNo, " +
                       "B.Trans_AbfInc, B.Trans_RoomPrice, B.Trans_AbfPrice " +
                       "FROM HotelTransaction A " +
                       "JOIN HotelTransRoomInfo B ON A.TransactionID=B.TransactionID " +
                       "LEFT JOIN RoomType C ON A.Trans_RoomTypeID=C.RoomTypeID " +
                       "LEFT JOIN RoomNo D ON B.Trans_RoomID=D.RoomNoID " +
                       "JOIN GuestInfo E ON A.GuestID=E.GuestID " +
                       "LEFT JOIN AgencyInfo F ON A.Trans_AgencyID=F.AgencyID " +
                       "WHERE A.Trans_StatusID=1 AND A.Trans_DepartureDate = '" + date + "' " +
                       "GROUP BY A.TransactionID " +
                       "UNION " +
                       "SELECT R.Trans_ArrivalDate, R.Trans_DepartureDate, V.RoomName, U.RoomTypeName, " +
                       "S.GuestName, X.AgencyName, 2 AS GuuestType, R.Trans_Remark, U.RoomTypeID, " +
                       "S.Trans_PaxAdult, S.Trans_PaxChild, T.Trans_ExtraBedPrice, R.AgencyRefNo, " +
                       "T.Trans_AbfInc, T.Trans_RoomPrice, T.Trans_AbfPrice " +
                       "FROM HotelTransaction R " +
                       "JOIN HotelTransGroupInfo S ON R.TransactionID=S.TransactionID " +
                       "JOIN HotelTransGroupDetail T ON T.TransactionID=S.TransactionID AND T.Trans_GroupID=S.Trans_GroupID " +
                       "LEFT JOIN RoomType U ON S.Trans_RoomTypeID=U.RoomTypeID " +
                       "LEFT JOIN RoomNo V ON T.Trans_RoomID=V.RoomNoID " +
                       "JOIN GuestInfo W ON R.GuestID=W.GuestID " +
                       "LEFT JOIN AgencyInfo X ON R.Trans_AgencyID=X.AgencyID " +
                       "WHERE S.Trans_CheckOutStatus=0 AND R.Trans_StatusID=1 AND R.Trans_DepartureDate = '" + date + "' " +
                       ") AS A " +
                       "ORDER BY AgencyName, RoomName";

                string matcur = ("N");
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var xTrans = new SetArrival_Departure();
                    xTrans.AgencyName = dt.Rows[i]["AgencyName"].ToString();
                    xTrans.Roomno = dt.Rows[i]["RoomName"].ToString();
                    xTrans.Roomtype = dt.Rows[i]["RoomTypeName"].ToString();
                    decimal ABF = (dt.Rows[i]["Trans_AbfPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["Trans_AbfPrice"].ToString());
                    xTrans.ABF = ABF.ToString(matcur);
                    string paxa = dt.Rows[i]["Trans_PaxAdult"].ToString();
                    string paxc = dt.Rows[i]["Trans_PaxChild"].ToString();
                    xTrans.Pax = paxa + "/" + paxc;

                    if (int.Parse(dt.Rows[i]["Trans_AbfInc"].ToString()) == 1)
                    {
                        decimal R_Rate = (decimal.Parse(dt.Rows[i]["Trans_RoomPrice"].ToString())) - (decimal.Parse(dt.Rows[i]["Trans_AbfPrice"].ToString()));
                        xTrans.R_Rate = R_Rate.ToString(matcur);
                    }
                    else
                    {
                        decimal R_Rate = (dt.Rows[i]["Trans_RoomPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["Trans_RoomPrice"].ToString());
                        xTrans.R_Rate = R_Rate.ToString(matcur);
                    }
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
    }
}