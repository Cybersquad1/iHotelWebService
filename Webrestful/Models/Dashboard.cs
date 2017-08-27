using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Dashboard
    {
        public static List<Setdashboard> Getmydashboard(MySqlConnection conn, DateTime dtHotelDate1, ref string szErrMsg)
        {
            var xAryTransCheckIn = new List<Setdashboard>();

            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate1.ToString(format, cur);

            string szQuery = "SELECT * FROM RoomType WHERE Deleted=0 AND RoomTypeCategoryID=0 ORDER BY Ordering";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);

            int iTotalRoomQty = 0, iTotalRoomDep = 0, iTotalRoomOcc = 0, iTotalRoomRsvn = 0, iTotalRoomOoo = 0, iTotalRoomStock = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int iRoomTypeID = int.Parse(dt.Rows[i]["RoomTypeID"].ToString());
                string szRoomTypeName = dt.Rows[i]["RoomTypeName"].ToString();
                int iRoomQty = 0;
                int iRoomDep = 0;
                int iRoomOcc = 0;
                int iRoomRsvn = 0;
                int iRoomOoo = 0;
                int iRoomStock = 0;

                GetSumRoomFromRoomType(conn, iRoomTypeID, startdate, ref iRoomQty, ref iRoomDep, ref iRoomOcc, ref iRoomRsvn, ref iRoomOoo, ref iRoomStock);

                iTotalRoomQty += iRoomQty;
                iTotalRoomDep += iRoomDep;
                iTotalRoomOcc += iRoomOcc;
                iTotalRoomRsvn += iRoomRsvn;
                iTotalRoomOoo += iRoomOoo;
                iTotalRoomStock += iRoomStock;
            }
            var xTrans = new Setdashboard();
            xTrans.departure = iTotalRoomDep.ToString();
            xTrans.ooo = iTotalRoomOoo.ToString();
            xTrans.inhouse = iTotalRoomOcc.ToString();
            xTrans.arrival = iTotalRoomRsvn.ToString();
            xTrans.avalible = iTotalRoomStock.ToString();

            xAryTransCheckIn.Add(xTrans);


            return xAryTransCheckIn;
        }

        static void GetSumRoomFromRoomType(MySqlConnection conn, int iRoomTypeID, string startdate, ref int iRoomQty, ref int iRoomDep, ref int iRoomOcc, ref int iRoomRsvn,
                                                ref int iRoomOoo, ref int iRoomStock)
        {
            string szQuery = "SELECT * FROM RoomNo WHERE Deleted=0 AND Activate=1 AND RoomTypeID=" + iRoomTypeID;
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            iRoomQty = dt.Rows.Count;
            iRoomOcc = iRoomRsvn = iRoomOoo = iRoomStock = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int iStatusID = int.Parse(dt.Rows[i]["RoomStatusID"].ToString());
                int iOutOfOrder = int.Parse(dt.Rows[i]["MaintenanceStatusID"].ToString());
                if (iStatusID == 2)         // Eoom Occupied
                    iRoomOcc++;
                else if (iStatusID == 3)    // Room Departure
                    iRoomDep++;

                if (iOutOfOrder == 5)    // Room Out of order=5 (Maintenance=4 not calc)
                {
                    iRoomOoo++;
                    //if (iRoomOcc > 0)
                    //    iRoomOcc--;
                }
            }

            //--- Get Room Rsvn
            string szQuery2 = "SELECT * FROM RsvnTransaction A " +
                           "JOIN RsvnRoomInfo B ON A.Rsvn_TransID=B.Rsvn_TransID " +
                           "WHERE A.RSVN_ArrivalDate = '" + startdate + "' " +
                           "AND A.RSVN_StatusID IN (1,2) " +
                           "AND A.Rsvn_RoomTypeID = " + iRoomTypeID + " " +
                           "GROUP BY A.Rsvn_TransID";
            dt = DBHelper.QueryListData(conn, szQuery2);
            int iTotalRsvnFIT = dt.Rows.Count;

            string szQuery3 = "SELECT * FROM RsvnGroupInfo X " +
                    "JOIN RsvnGroupInfo Y ON X.Rsvn_TransID=Y.Rsvn_TransID " +
                    "WHERE X.RSVN_ArrivalDate = '" + startdate + "' " +
                    "AND X.Rsvn_CheckInStatus=0 " +
                    "AND X.Rsvn_RoomTypeID = " + iRoomTypeID + " " +
                    "GROUP BY X.Rsvn_TransID, X.RSVN_GroupID";

            dt = DBHelper.QueryListData(conn, szQuery3);
            int iTotalRsvnGroup = dt.Rows.Count;

            iRoomRsvn = iTotalRsvnFIT + iTotalRsvnGroup;
            //--- Calc RoomStock
            iRoomStock = iRoomQty - iRoomOcc - iRoomRsvn - iRoomOoo;
        }


        public static List<setabf> Getdashboardabfs(MySqlConnection conn, DateTime dtHotelDate1, ref string szErrMsg)
        {
            var xAryTransCheckIn = new List<setabf>();
            DateTime dateabf = dtHotelDate1.AddDays(-1);
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dateabf.ToString(format, cur);

            string szFmt = "SELECT * FROM (" +
                       "SELECT RoomName, RoomTypeName, E.GuestName, A.Trans_ArrivalDate, A.Trans_DepartureDate, G.NationalityName, " +
                       "A.Trans_PaxAdult, A.Trans_PaxChild, B.Trans_AbfPrice,  B.Trans_ExtraBedPrice, B.Trans_ExtraBedAbf, " +
                       "B.Trans_Date, A.TransactionID, D.Ordering, C.RoomTypeID, 1 AS GuestType, F.AgencyName " +
                       "FROM HotelTransaction A " +
                       "JOIN HotelTransRoomInfo B ON A.TransactionID=B.TransactionID " +
                       "JOIN RoomType C ON A.Trans_RoomTypeID=C.RoomTypeID " +
                       "JOIN RoomNo D ON B.Trans_RoomID=D.RoomNoID AND D.RoomTypeID=C.RoomTypeID " +
                       "JOIN GuestInfo E ON A.GuestID=E.GuestID " +
                       "LEFT JOIN AgencyInfo F ON A.Trans_AgencyID=F.AgencyID " +
                       "JOIN Nationality G ON E.GuestNationalityID=G.NationalityID " +
                       "WHERE A.Trans_StatusID=1 AND (B.Trans_AbfPrice>0 OR B.Trans_AbfInc=1) AND B.Trans_Date='"+ startdate + "' " +
                       "UNION " +
                       "SELECT RoomName, RoomTypeName, T.GuestName, T.Trans_ArrivalDate, T.Trans_DepartureDate, Z.NationalityName, " +
                       "T.Trans_PaxAdult, T.Trans_PaxChild, U.Trans_AbfPrice, U.Trans_ExtraBedPrice, 1, " +
                       "U.Trans_Date, T.TransactionID, W.Ordering, V.RoomTypeID, 2 AS GuestType, Y.AgencyName " +
                       "FROM HotelTransGroupInfo T " +
                       "JOIN HotelTransaction S ON S.TransactionID=T.TransactionID " +
                       "JOIN HotelTransGroupDetail U ON T.TransactionID=U.TransactionID AND T.Trans_GroupID=U.Trans_GroupID " +
                       "JOIN RoomType V ON T.Trans_RoomTypeID=V.RoomTypeID " +
                       "JOIN RoomNo W ON U.Trans_RoomID=W.RoomNoID AND W.RoomTypeID=V.RoomTypeID " +
                       "LEFT JOIN AgencyInfo Y ON S.Trans_AgencyID=Y.AgencyID " +
                       "JOIN GuestInfo R ON S.GuestID=R.GuestID " +
                       "JOIN Nationality Z ON R.GuestNationalityID=Z.NationalityID " +
                       "WHERE T.Trans_CheckOutStatus=0 AND S.Trans_StatusID=1 AND (U.Trans_AbfPrice>0 OR U.Trans_AbfInc=1) AND U.Trans_Date='"+ startdate + "' " +
                       ") AS A " +
                       "ORDER BY RoomTypeID ";  // GuestType, Trans_Date, TransactionID, Ordering

            DataTable dt = DBHelper.QueryListData(conn, szFmt);
            int iTotalPax = 0;
            DateTime dtCurDate = DateTime.MinValue;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dtMealDate = ((DateTime)dt.Rows[i]["Trans_Date"]);
                string szMealDate = "";
                if (dtMealDate != dtCurDate)
                {
                    dtCurDate = dtMealDate;
                    szMealDate = dtCurDate.ToString("ddd dd/MM/yyyy");
                }


                decimal fExtraBedPrice = decimal.Parse(dt.Rows[i]["Trans_ExtraBedPrice"].ToString());
                int iAbfExtraBed = (fExtraBedPrice > 0) ? int.Parse(dt.Rows[i]["Trans_ExtraBedABF"].ToString()) : 0;
                int iPax = int.Parse(dt.Rows[i]["Trans_PaxAdult"].ToString()) + int.Parse(dt.Rows[i]["Trans_PaxChild"].ToString()) + iAbfExtraBed;
                iTotalPax += iPax;
            }
            var xTrans = new setabf();
            xTrans.abf = iTotalPax.ToString();
            xAryTransCheckIn.Add(xTrans);

            return xAryTransCheckIn;
        }
    }
}