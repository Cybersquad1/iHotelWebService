using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Diagnostics;
namespace Webrestful.Models
{
    public class HistoryandForcast
    {
        public static List<HisandFor> GetHistorys(MySqlConnection conn, DateTime dtHotelDate, DateTime dtHotelDate2, int vat, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string startdate = dtHotelDate.ToString(format, cur);
                string enddate = dtHotelDate2.ToString(format, cur);

                int month = dtHotelDate.Month;
                int year = dtHotelDate.Year;
                int days = System.DateTime.DaysInMonth(year, month);

                var xAryTransCheckIn = new List<HisandFor>();

             
                DataTable dtTotalOccPax = new DataTable();
                DataTable dtTotalRevenue = new DataTable();
                DataTable dtTotalOooRoom = new DataTable();
         
                dtTotalOccPax = GetTotalOccPaxHistory(conn, startdate, enddate, days, year, month);
                dtTotalRevenue = GetTotalRevenueHistory(conn, startdate, enddate, days, year, month, vat);
              

                int iTotalRooms = GetTotalRooms(conn);
                int iGrandTotalOcc = 0;
                decimal fGrandTotalABF = 0, fGrandTotalRoomCharge = 0;
                int iGrandTotalPax = 0;

                for (int i = 0; i < days; i++)
                {
                    DateTime dtDate = new DateTime(year, month, i+1);
                    string szDate = dtDate.ToString("dd");
                    int iTotalOcc = int.Parse(dtTotalOccPax.Rows[i]["TotalOcc"].ToString());
                    decimal fOccPercent = (decimal)(iTotalOcc) / iTotalRooms;
                    decimal fOccPercent2 = fOccPercent * 100;
                    decimal fTotalRoomCharge = decimal.Parse(dtTotalRevenue.Rows[i]["TotalRoomCharge"].ToString());
                    decimal fAvgRate = (iTotalOcc == 0) ? 0 : fTotalRoomCharge / (decimal)iTotalOcc;
                    decimal fTotalAbf = decimal.Parse(dtTotalRevenue.Rows[i]["TotalABF"].ToString());
                    int iTotalPax = int.Parse(dtTotalOccPax.Rows[i]["TotalPax"].ToString());

                    iGrandTotalOcc += iTotalOcc;
                    fGrandTotalRoomCharge += fTotalRoomCharge;
                    fGrandTotalABF += fTotalAbf;
                    iGrandTotalPax += iTotalPax;

                    var xTrans = new HisandFor();
                    xTrans.Date = szDate;
                    xTrans.Pax = Convert.ToString(iTotalPax);
                    xTrans.Totalocc = Convert.ToString(iTotalOcc);
                    xTrans.ABF = Convert.ToString(fTotalAbf.ToString("N"));
                    xTrans.Avg = Convert.ToString(fAvgRate.ToString("N"));
                    xTrans.PerOcc = Convert.ToString(fOccPercent2.ToString("0.##")+"%");
                    xTrans.RawOcc = Convert.ToString(fOccPercent2.ToString());
                    xTrans.Roomchg = Convert.ToString(fTotalRoomCharge.ToString("N"));
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

        public static List<HisandFor> GetForcasts(MySqlConnection conn, DateTime dtHotelDate, DateTime dtHotelDate2, int vat, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string startdate = dtHotelDate.ToString(format, cur);
                string enddate = dtHotelDate2.ToString(format, cur);

                int month = dtHotelDate.Month;
                int year = dtHotelDate.Year;
                int days = System.DateTime.DaysInMonth(year, month);

                var xAryTransCheckIn = new List<HisandFor>();

                DataTable dtArrivalDeparture = new DataTable();
                DataTable dtTotalOccPax = new DataTable();
                DataTable dtTotalRevenue = new DataTable();
                DataTable dtTotalOooRoom = new DataTable();

              
                dtTotalOccPax = GetTotalOccPaxForecast(conn, startdate, enddate, days, year, month);
                dtTotalRevenue = GetTotalRevenueForecast(conn, startdate, enddate, days, year, month, vat);

                int iTotalRooms = GetTotalRooms(conn);

                int iGrandTotalOcc = 0;
                decimal fGrandTotalABF = 0, fGrandTotalRoomCharge = 0;
                int iGrandTotalPax = 0;

                for (int i = 0; i < days; i++)
                {
                    DateTime dtDate = new DateTime(year,month, i + 1);
                    string szDate = dtDate.ToString("dd");
                    int iTotalOcc = int.Parse(dtTotalOccPax.Rows[i]["TotalOcc"].ToString());
                    decimal fOccPercent = (decimal)(iTotalOcc) / iTotalRooms;
                    decimal fOccPercent2 = fOccPercent * 100;
                    decimal fTotalRoomCharge = decimal.Parse(dtTotalRevenue.Rows[i]["TotalRoomCharge"].ToString());
                    decimal fAvgRate = (iTotalOcc == 0) ? 0 : fTotalRoomCharge / (decimal)iTotalOcc;     
                    decimal fTotalAbf = decimal.Parse(dtTotalRevenue.Rows[i]["TotalABF"].ToString());
                    int iTotalPax = int.Parse(dtTotalOccPax.Rows[i]["TotalPax"].ToString());

                    iGrandTotalOcc += iTotalOcc;
                    fGrandTotalRoomCharge += fTotalRoomCharge;
                    fGrandTotalABF += fTotalAbf;
                    iGrandTotalPax += iTotalPax;

                    var xTrans = new HisandFor();
                    xTrans.Date = szDate;
                    xTrans.Pax = Convert.ToString(iTotalPax);
                    xTrans.Totalocc = Convert.ToString(iTotalOcc);
                    xTrans.ABF = Convert.ToString(fTotalAbf.ToString("N"));
                    xTrans.Avg = Convert.ToString(fAvgRate.ToString("N"));
                    xTrans.PerOcc = Convert.ToString(fOccPercent2.ToString("0.##") + "%");
                    xTrans.RawOcc = Convert.ToString(fOccPercent2.ToString());
                    xTrans.Roomchg = Convert.ToString(fTotalRoomCharge.ToString("N"));
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


        protected static DataTable GetTotalOccPaxForecast(MySqlConnection conn, string startdate, string enddate, int days, int year, int month)
        {
            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("Date", typeof(DateTime));
            dtRes.Columns.Add("TotalOcc", typeof(int));
            dtRes.Columns.Add("TotalPax", typeof(int));

            string szFmt = "SELECT A.Rsvn_Date, COUNT(DISTINCT A.Rsvn_TransID) AS CountTrans, " +
                           "SUM(Rsvn_PaxAdult+Rsvn_PaxChild) AS SumGuest " +
                           "FROM RsvnRoomInfo A " +
                           "JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID " +
                           "WHERE B.Rsvn_StatusID<90 AND B.Rsvn_TypeID IN (1,3) " +
                            "AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "'" +
                           "GROUP BY A.Rsvn_Date";
 
            DataTable dtFIT = DBHelper.QueryListData(conn, szFmt);

            string szFmt2 = "SELECT C.Rsvn_Date, COUNT(A.Rsvn_TransID) AS CountTrans, " +
                    "SUM(A.Rsvn_PaxAdult+A.Rsvn_PaxChild) AS SumGuest " +
                    "FROM RsvnGroupInfo A " +
                    "JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID " +
                    "JOIN RsvnGroupDetail C ON A.Rsvn_TransID=C.Rsvn_TransID  AND A.Rsvn_GroupID=C.Rsvn_GroupID " +
                    "WHERE B.Rsvn_StatusID<90 AND B.Rsvn_TypeID IN (2,4) " +
                     "AND C.Rsvn_Date>='" + startdate + "' AND C.Rsvn_Date<'" + enddate + "'" +
                    "GROUP BY C.Rsvn_Date";

            DataTable dtGroup = DBHelper.QueryListData(conn, szFmt2);

            for (int i = 1; i <= days; i++)
            {
                int iTotalOcc = 0, iTotalPax = 0;
                DateTime dtDate = new DateTime(year, month, i);
                DataRow[] row = dtFIT.Select("Rsvn_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                {
                    iTotalOcc = int.Parse(row[0]["CountTrans"].ToString());
                    iTotalPax = int.Parse(row[0]["SumGuest"].ToString());
                }
                row = dtGroup.Select("Rsvn_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                {
                    iTotalOcc += int.Parse(row[0]["CountTrans"].ToString());
                    iTotalPax += int.Parse(row[0]["SumGuest"].ToString());
                }

                DataRow dr = dtRes.NewRow();
                dr["Date"] = dtDate;
                dr["TotalOcc"] = iTotalOcc;
                dr["TotalPax"] = iTotalPax;
                dtRes.Rows.Add(dr);
            }

            return dtRes;
        }
        protected static DataTable GetTotalRevenueForecast(MySqlConnection conn, string startdate, string enddate, int days, int year, int month, int vat) {
            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("Date", typeof(DateTime));
            dtRes.Columns.Add("TotalRoomCharge", typeof(decimal));
            dtRes.Columns.Add("TotalABF", typeof(decimal));

            string szFmt = "";
            if (vat == 0)
                szFmt = "SELECT DATE_FORMAT(Date, '%Y-%m-%d') as Date, (SUM(RoomPrice)*100)/107 AS SumRoomPrice, (SUM(AbfPrice)*100)/107 AS SumAbfPrice FROM ( SELECT A.Rsvn_Date AS Date, IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "' UNION ALL SELECT A.Rsvn_Date AS Date, IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON A.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "' UNION ALL SELECT A.Trans_Date AS Date, IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_StatusID<90 AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "' UNION ALL SELECT A.Trans_Date AS Date, IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON A.TransactionID=C.TransactionID WHERE C.Trans_StatusID<90 AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "' ) AS A GROUP BY Date";
            else
                szFmt = "SELECT DATE_FORMAT(Date, '%Y-%m-%d') as Date, SUM(RoomPrice) AS SumRoomPrice, SUM(AbfPrice) AS SumAbfPrice FROM ( SELECT A.Rsvn_Date AS Date, IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "' UNION ALL SELECT A.Rsvn_Date AS Date, IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON A.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "' UNION ALL SELECT A.Trans_Date AS Date, IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_StatusID<90 AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "' UNION ALL SELECT A.Trans_Date AS Date, IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON A.TransactionID=C.TransactionID WHERE C.Trans_StatusID<90 AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "' )AS A GROUP BY Date";
            DataTable dtRc = DBHelper.QueryListData(conn, szFmt);
            for(int i = 1; i <= days; i++)
            {
                decimal fTotalRC = 0, fTotalABF = 0;
                Debug.WriteLine("Year:"+year+" Month: "+month+" Days: "+days);
                DateTime dtDate = new DateTime(year, month, i);
                Debug.WriteLine(dtDate);
                DataRow[] row = dtRc.Select("Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                {
                    fTotalRC = decimal.Parse(row[0][1].ToString());
                    fTotalABF = decimal.Parse(row[0][2].ToString());
                }
                DataRow dr = dtRes.NewRow();
                dr["Date"] = dtDate;
                dr["TotalRoomCharge"] = fTotalRC;
                dr["TotalABF"] = fTotalABF;
                dtRes.Rows.Add(dr);
            }
            return dtRes;
        }
        //protected static DataTable GetTotalRevenueForecast(MySqlConnection conn, string startdate, string enddate, int days, int year, int month)
        //{
        //    DataTable dtRes = new DataTable();
        //    dtRes.Columns.Add("Date", typeof(DateTime));
        //    dtRes.Columns.Add("TotalRoomCharge", typeof(decimal));
        //    dtRes.Columns.Add("TotalABF", typeof(decimal));

        //    int iVatType = 0, iServiceChargeType = 0;
        //    decimal fVatRate = 0, fServiceChargeRate = 0;
        //    GetVatTypeServiceChargeType(conn ,ref iVatType, ref iServiceChargeType, ref fVatRate, ref fServiceChargeRate);

        //    string szFmtRsvnFIT = "SELECT Rsvn_Date, SUM(RoomPrice) AS SumRoomPrice, SUM(AbfPrice) AS SumAbfPrice FROM (" +
        //                          "SELECT A.Rsvn_Date, " +
        //                          "IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, " +
        //                          "Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice " +
        //                          "FROM RsvnRoomInfo A " +
        //                          "JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID " +
        //                          "WHERE B.Rsvn_StatusID<10 " +
        //                           "AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "'" +
        //                          "ORDER BY A.Rsvn_Date) AS A " +
        //                          "GROUP BY Rsvn_Date";

        //    string szFmtRsvnGroup =
        //        "SELECT Rsvn_Date, SUM(RoomPrice) AS SumRoomPrice, SUM(AbfPrice) AS SumAbfPrice FROM (" +
        //        "SELECT A.Rsvn_Date, " +
        //        "IF (A.Rsvn_AbfInc = 1, Rsvn_RoomPrice-(Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild)), Rsvn_RoomPrice) AS RoomPrice, " +
        //        "Rsvn_AbfPrice*(B.Rsvn_PaxAdult+B.Rsvn_PaxChild) AS AbfPrice " +
        //        "FROM RsvnGroupDetail A " +
        //        "JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID " +
        //        "JOIN RsvnTransaction C ON A.Rsvn_TransID=C.Rsvn_TransID " +
        //        "WHERE C.Rsvn_StatusID<10 " +
        //         "AND A.Rsvn_Date>='" + startdate + "' AND A.Rsvn_Date<'" + enddate + "'" +
        //        "ORDER BY A.Rsvn_Date) AS A " +
        //        "GROUP BY Rsvn_Date";

        //    string szFmtTransFIT =
        //        "SELECT Trans_Date, SUM(RoomPrice) AS SumRoomPrice, SUM(AbfPrice) AS SumAbfPrice FROM (" +
        //        "SELECT A.Trans_Date, " +
        //        "IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, " +
        //        "Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice " +
        //        "FROM HotelTransRoomInfo A " +
        //        "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
        //        "WHERE B.Trans_StatusID<90 " +
        //         "AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "'" +
        //        "ORDER BY A.Trans_Date) AS A " +
        //        "GROUP BY Trans_Date";

        //    string szFmtTransGroup =
        //        "SELECT Trans_Date, SUM(RoomPrice) AS SumRoomPrice, SUM(AbfPrice) AS SumAbfPrice FROM (" +
        //        "SELECT A.Trans_Date, " +
        //        "IF (A.Trans_AbfInc = 1, Trans_RoomPrice-(Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild)), Trans_RoomPrice) AS RoomPrice, " +
        //        "Trans_AbfPrice*(B.Trans_PaxAdult+B.Trans_PaxChild) AS AbfPrice " +
        //        "FROM HotelTransGroupDetail A " +
        //        "JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID " +
        //        "JOIN HotelTransaction C ON A.TransactionID=C.TransactionID " +
        //        "WHERE C.Trans_StatusID<90 " +
        //         "AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "'" +
        //        "ORDER BY A.Trans_Date) AS A " +
        //        "GROUP BY Trans_Date ";

        //   DataTable dtRsvnFit = DBHelper.QueryListData(conn, szFmtRsvnFIT);

        //   DataTable dtRsvnGroup = DBHelper.QueryListData(conn, szFmtRsvnGroup);

        //  DataTable dtTransFit = DBHelper.QueryListData(conn, szFmtTransFIT);

        //  DataTable dtTransGroup = DBHelper.QueryListData(conn, szFmtTransGroup);

        //    for (int i = 1; i <= days; i++)
        //    {
        //        decimal fTotalRC = 0, fTotalABF = 0;
        //        DateTime dtDate = new DateTime(year, month, i);
        //        DataRow[] row = dtRsvnFit.Select("Rsvn_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
        //        if (row.Length > 0)
        //        {
        //            fTotalRC = decimal.Parse(row[0][1].ToString());
        //            fTotalABF = decimal.Parse(row[0][2].ToString());
        //        }

        //        row = dtRsvnGroup.Select("Rsvn_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
        //        if (row.Length > 0)
        //        {
        //            fTotalRC += decimal.Parse(row[0][1].ToString());
        //            fTotalABF += decimal.Parse(row[0][2].ToString());
        //        }

        //        row = dtTransFit.Select("Trans_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
        //        if (row.Length > 0)
        //        {
        //            fTotalRC += decimal.Parse(row[0][1].ToString());
        //            fTotalABF += decimal.Parse(row[0][2].ToString());
        //        }

        //        row = dtTransGroup.Select("Trans_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
        //        if (row.Length > 0)
        //        {
        //            fTotalRC += decimal.Parse(row[0][1].ToString());
        //            fTotalABF += decimal.Parse(row[0][2].ToString());
        //        }

        //        DataRow dr = dtRes.NewRow();
        //        dr["Date"] = dtDate;
        //        dr["TotalRoomCharge"] = fTotalRC;
        //        dr["TotalABF"] = fTotalABF;
        //        dtRes.Rows.Add(dr);
        //    }

        //    return dtRes;
        //}
        protected static int GetTotalRooms(MySqlConnection conn)
        {
            string szQuery = "SELECT COUNT(*) FROM RoomNo WHERE RoomCategoryID=0 AND Deleted=0 AND Activate=1";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows[0][0] == DBNull.Value)
                return 0;
            else
                return int.Parse(dt.Rows[0][0].ToString());
        }

        protected static void GetVatTypeServiceChargeType(MySqlConnection conn,ref int iVatType, ref int iServiceChargeType, ref decimal fVatRate, ref decimal fServiceChargeRate)
        {
            string szQuery = "SELECT * FROM HotelItemService WHERE HotelItemServiceID=1001";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            iVatType = int.Parse(dt.Rows[0]["VatType"].ToString());
            iServiceChargeType = int.Parse(dt.Rows[0]["ServiceChargeType"].ToString());

            szQuery = "SELECT * FROM CompanyProfile";
            dt = DBHelper.QueryListData(conn, szQuery);
            fVatRate = decimal.Parse(dt.Rows[0]["CompanyVatRate"].ToString());
            fServiceChargeRate = decimal.Parse(dt.Rows[0]["CompanyServiceCharge"].ToString());
        }
        protected static DataTable GetTotalOccPaxHistory(MySqlConnection conn, string startdate, string enddate, int days, int year, int month)
        {
            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("Date", typeof(DateTime));
            dtRes.Columns.Add("TotalOcc", typeof(int));
            dtRes.Columns.Add("TotalPax", typeof(int));


            string szQuery = "SELECT A.Trans_Date, COUNT(DISTINCT A.TransactionID) AS CountTrans, " +
                       "SUM(Trans_PaxAdult+Trans_PaxChild) AS SumGuest " +
                       "FROM HotelTransRoomInfo A " +
                       "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                       "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (1,3) " +
                       "AND A.Trans_Date>='" + startdate + "' AND A.Trans_Date<'" + enddate + "'" +
                       "GROUP BY A.Trans_Date";

            DataTable dtFIT = DBHelper.QueryListData(conn, szQuery);

            string szQuery2 = "SELECT C.Trans_Date, COUNT(A.TransactionID) AS CountTrans, " +
                    "SUM(A.Trans_PaxAdult+A.Trans_PaxChild) AS SumGuest " +
                    "FROM HotelTransGroupInfo A " +
                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                    "JOIN HotelTransGroupDetail C ON A.TransactionID=C.TransactionID  AND A.Trans_GroupID=C.Trans_GroupID " +
                    "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (2,4) " +
                      "AND C.Trans_Date>='" + startdate + "' AND C.Trans_Date<'" + enddate + "'" +
                    "GROUP BY C.Trans_Date";

            DataTable dtGroup = DBHelper.QueryListData(conn, szQuery2);

            for (int i = 1; i <= days; i++)
            {

                int iTotalOcc = 0, iTotalPax = 0;
                DateTime dtDate = new DateTime(year, month, i);
                DataRow[] row = dtFIT.Select("Trans_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                {
                    iTotalOcc = int.Parse(row[0]["CountTrans"].ToString());
                    iTotalPax = int.Parse(row[0]["SumGuest"].ToString());
                }
                row = dtGroup.Select("Trans_Date=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                {
                    iTotalOcc += int.Parse(row[0]["CountTrans"].ToString());
                    iTotalPax += int.Parse(row[0]["SumGuest"].ToString());
                }

                DataRow dr = dtRes.NewRow();
                dr["Date"] = dtDate;
                dr["TotalOcc"] = iTotalOcc;
                dr["TotalPax"] = iTotalPax;
                dtRes.Rows.Add(dr);
            }
            return dtRes;
        }

        protected static DataTable GetTotalRevenueHistory(MySqlConnection conn, string startdate, string enddate, int days, int year, int month, int vat)
        {
            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("Date", typeof(DateTime));
            dtRes.Columns.Add("TotalRoomCharge", typeof(decimal));
            dtRes.Columns.Add("TotalABF", typeof(decimal));


            //string szFmtFIT = "";
            //if (vat == 0)
            //    szFmtFIT = "SELECT A.FolioDetailDate, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
            //                "FROM HotelFolioDetail A " +
            //                "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            //                "WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) " +
            //                 "AND A.FolioDetailDate>='" + startdate + "' AND A.FolioDetailDate<'" + enddate + "'" +
            //                "GROUP BY A.FolioDetailDate";
            //else
            //    szFmtFIT = "SELECT A.FolioDetailDate, SUM(TotalItemPrice) AS TotalPrice " +
            //                "FROM HotelFolioDetail A " +
            //                "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            //                "WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) " +
            //                 "AND A.FolioDetailDate>='" + startdate + "' AND A.FolioDetailDate<'" + enddate + "'" +
            //                "GROUP BY A.FolioDetailDate";

            //string szFmtGroup = "";
            //if (vat == 1)
            //    szFmtGroup = "SELECT A.FolioDetailDate, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
            //                "FROM HotelFolioGroupDetail A " +
            //                "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            //                "WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) " +
            //                "AND A.FolioDetailDate>='" + startdate + "' AND A.FolioDetailDate<'" + enddate + "'" +
            //                "GROUP BY A.FolioDetailDate";
            //else
            //    szFmtGroup = "SELECT A.FolioDetailDate, SUM(TotalItemPrice) AS TotalPrice " +
            //                    "FROM HotelFolioGroupDetail A " +
            //                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            //                    "WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) " +
            //                     "AND A.FolioDetailDate>='" + startdate + "' AND A.FolioDetailDate<'" + enddate + "'" +
            //                    "GROUP BY A.FolioDetailDate";
            string szFmt = "";
            if (vat == 0)
                szFmt = "SELECT FolioDate, SUM(TotalPrice) AS TotalPrice From(SELECT A.FolioDetailDate AS FolioDate, SUM(TotalItemPrice) AS TotalPrice FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) AND (A.FolioDetailDate)>='" + startdate + "' AND (A.FolioDetailDate)<'" + enddate + "' GROUP BY FolioDate UNION ALL SELECT A.FolioDetailDate AS FolioDate, SUM(TotalItemPrice) AS TotalPrice FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) AND (A.FolioDetailDate)>='" + startdate + "' AND (A.FolioDetailDate)<'" + enddate + "' GROUP BY FolioDate) AS A GROUP BY FolioDate";
            else
                szFmt = "SELECT FolioDate, SUM(TotalPrice) AS TotalPrice From(SELECT A.FolioDetailDate AS FolioDate, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) AND (A.FolioDetailDate)>='" + startdate + "' AND (A.FolioDetailDate)<'" + enddate + "' GROUP BY A.FolioDetailDate UNION ALL SELECT A.FolioDetailDate AS FolioDate, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN ({0},{1}) AND (A.FolioDetailDate)>='" + startdate + "' AND (A.FolioDetailDate)<'" + enddate + "' GROUP BY A.FolioDetailDate) AS A GROUP BY FolioDate";

            //string szQuery = string.Format(szFmtFIT, 1001, 2001);
            //DataTable dtFitRC = DBHelper.QueryListData(conn, szQuery);

            //szQuery = string.Format(szFmtGroup, 1001, 2001);
            //DataTable dtGroupRC = DBHelper.QueryListData(conn, szQuery);

            //szQuery = string.Format(szFmtFIT, 1002, 2002);
            //DataTable dtFitABF = DBHelper.QueryListData(conn, szQuery);

            //szQuery = string.Format(szFmtGroup, 1002, 2002);
            //DataTable dtGroupABF = DBHelper.QueryListData(conn, szQuery);
            ///// NEW /////
            string szQuery = string.Format(szFmt, 1001, 2001);
            DataTable dtRc = DBHelper.QueryListData(conn, szQuery);
            szQuery = string.Format(szFmt, 1002, 2002);
            DataTable dtABF = DBHelper.QueryListData(conn, szQuery);
            for (int i = 1; i <= days; i++)
            {
                decimal fTotalRC = 0, fTotalABF = 0;
                DateTime dtDate = new DateTime(year, month, i);
                //DataRow[] row = dtFitRC.Select("FolioDetailDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                //if (row.Length > 0)
                //    fTotalRC = decimal.Parse(row[0][1].ToString());

                //row = dtGroupRC.Select("FolioDetailDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                //if (row.Length > 0)
                //    fTotalRC += decimal.Parse(row[0][1].ToString());

                //row = dtFitABF.Select("FolioDetailDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                //if (row.Length > 0)
                //    fTotalABF = decimal.Parse(row[0][1].ToString());

                //row = dtGroupABF.Select("FolioDetailDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                //if (row.Length > 0)
                //    fTotalABF += decimal.Parse(row[0][1].ToString());

                DataRow[] row = dtRc.Select("FolioDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                    fTotalRC = decimal.Parse(row[0][1].ToString());

                row = dtABF.Select("FolioDate=#" + dtDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "#");
                if (row.Length > 0)
                    fTotalABF = decimal.Parse(row[0][1].ToString());

                DataRow dr = dtRes.NewRow();
                dr["Date"] = dtDate;
                dr["TotalRoomCharge"] = fTotalRC;
                dr["TotalABF"] = fTotalABF;
                dtRes.Rows.Add(dr);
            }
            return dtRes;
        }
    }
            
}