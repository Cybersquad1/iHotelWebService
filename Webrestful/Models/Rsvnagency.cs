using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Rsvnagency
    {
        public static List<SetRsvn_agency> GetAgency(MySqlConnection conn, DateTime dtHotelDate, DateTime dtHotelDate2, DateTime dtHotelDate3, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<SetRsvn_agency>();
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            string enddate = dtHotelDate2.ToString(format, cur);
            string datetime = dtHotelDate3.ToString(format, cur);

            string sqlstring = "SELECT Trans_AgencyID, AgencyName, SUM(RoomNight) AS SumRoomNight FROM ( " +
                   "SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight " +
                   "FROM HotelTransaction A " +
                   "JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID " +
                   "JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID " +
                   "WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=3 AND C.Trans_Date>='" + startdate + "' AND C.Trans_Date< '" + enddate + "' AND C.Trans_Date<'" + datetime + "' " +
                   "GROUP BY A.Trans_AgencyID " +
                   "UNION " +
                   "SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight " +
                   "FROM HotelTransaction A " +
                   "JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID " +
                   "JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID " +
                   "JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID " +
                   "WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=4 AND D.Trans_Date>='" + startdate + "' AND D.Trans_Date< '" + enddate + "' AND D.Trans_Date<'" + datetime + "' " +
                   "GROUP BY A.Trans_AgencyID " +
                   ") AS A " +
                   "GROUP BY Trans_AgencyID " +
                   "ORDER BY SumRoomNight DESC, AgencyName";
            string matcur = ("N");
            int iSumRoomNight = 0;
            decimal fSumRoomRevenue = 0;
            DataTable dt = DBHelper.QueryListData(conn, sqlstring);
            decimal[] arr = new decimal[dt.Rows.Count];
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var xTrans = new SetRsvn_agency();
                int iAgencyID = int.Parse(dt.Rows[i]["Trans_AgencyID"].ToString());
                xTrans.AgencyName = dt.Rows[i]["AgencyName"].ToString();
                int iRoomNight = int.Parse(dt.Rows[i]["SumRoomNight"].ToString());
                xTrans.Roomnight = iRoomNight;
                var fRoomRevenue = CalcRoomRevenue(conn, iAgencyID, startdate, enddate);
                xTrans.Roomrev =  fRoomRevenue.ToString(matcur);
                arr[i] = fRoomRevenue;
               decimal fRoomAvg = (iRoomNight == 0) ? 0 : fRoomRevenue / iRoomNight;
               xTrans.Roomavg = fRoomAvg.ToString(matcur);

                iSumRoomNight += iRoomNight;
                fSumRoomRevenue += fRoomRevenue;

                xAryTransCheckIn.Add(xTrans);
            }

            var xTrans1 = new SetRsvn_agency();
            decimal fSumRoomAvg = (iSumRoomNight == 0) ? 0 : fSumRoomRevenue / iSumRoomNight;

            xTrans1.Sumroomavg = fSumRoomAvg.ToString(matcur);
            xTrans1.Sumroomnight = iSumRoomNight;
            xTrans1.Sumroomrev = fSumRoomRevenue.ToString(matcur);
            
            xAryTransCheckIn.Add(xTrans1);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var xTrans2 = new SetRsvn_agency();
                int iRoomNight = int.Parse(dt.Rows[i]["SumRoomNight"].ToString());
                decimal fPercentRoomNight = (iSumRoomNight == 0) ? 0 : (decimal)(iRoomNight * 100) / iSumRoomNight;

                decimal fRoomRevenue2 = arr[i];
                decimal fPercentRoomRev = (fSumRoomRevenue == 0) ? 0 : (decimal)(fRoomRevenue2 * 100) / fSumRoomRevenue;

                xTrans2.Perroomnight =  Convert.ToString(fPercentRoomNight.ToString(matcur));
                xTrans2.Perroomrev = Convert.ToString(fPercentRoomRev.ToString(matcur));
               xAryTransCheckIn.Add(xTrans2);
            }
           

            return xAryTransCheckIn;
        }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
}
        static decimal CalcRoomRevenue(MySqlConnection conn, int iAgencyID, string startdate, string enddate)
        {
            string szFmt = "";
            szFmt = "SELECT A.TransactionID " +
                       "FROM HotelTransaction A " +
                       "JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID " +
                       "JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID " +
                       "WHERE A.Trans_StatusID<90 AND {1} AND A.Trans_TypeID=3 AND A.Trans_AgencyID={0} " +
                       "GROUP BY A.TransactionID " +
                       "UNION " +
                       "SELECT A.TransactionID " +
                       "FROM HotelTransaction A " +
                       "JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID " +
                       "JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID " +
                       "JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID " +
                       "WHERE A.Trans_StatusID<90 AND {2} AND A.Trans_TypeID=4 AND A.Trans_AgencyID={0} " +
                       "GROUP BY A.TransactionID";
            string szDate1 = "C.Trans_Date>='" + startdate + "' AND C.Trans_Date<'" + enddate + "'";
            string szDate2 = "D.Trans_Date>='" + startdate + "' AND D.Trans_Date<'" + enddate + "'";
            
            string szQuery = string.Format(szFmt, iAgencyID, szDate1, szDate2);
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            string szAryTransID = "(";
            for (int i = 0; i < dt.Rows.Count; i++)
                szAryTransID += dt.Rows[i][0] + ",";
            szAryTransID = szAryTransID.Remove(szAryTransID.Length - 1) + ")";

            szFmt = "SELECT SUM(SumPrice) AS TotalPrice FROM ( " +
                    "SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice " +
                    "FROM HotelFolioDetail A " +
                    "WHERE A.TransactionID IN {0} " +
                    "AND {1} AND FolioItemID={2} AND A.FolioStatusID<90 " +
                    "UNION " +
                    "SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice " +
                    "FROM HotelFolioGroupDetail A " +
                    "WHERE A.TransactionID IN {0} " +
                    "AND {1} AND FolioItemID={2} AND A.FolioStatusID<90 " +
                    ") AS A";
            string szDate = "FolioDetailDate>='" + startdate + "' AND FolioDetailDate<'" + enddate + "'";
            szQuery = string.Format(szFmt, szAryTransID, szDate, 1001);
            dt = DBHelper.QueryListData(conn, szQuery);
            decimal x = (dt.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[0][0].ToString());
            return x;
        }


    }
}