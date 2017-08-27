using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Staticroom
    {
        public static List<Setstaticroom> Getstaticroom(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            //--- Arrival
            int iRoomArrival = 0, iRoomDeparture = 0, iRoomRsvn = 0;
            int iGuestArrival = 0, iGuestDeparture = 0, iGuestRsvn = 0;
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<Setstaticroom>();

            string szQuery = "SELECT COUNT(DISTINCT A.TransactionID), SUM(Trans_PaxAdult+Trans_PaxChild) " +
                           "FROM HotelTransRoomInfo A " +
                           "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                           "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (1,3) " +
                           "AND A.Trans_Date=B.Trans_ArrivalDate " +
                           "AND B.Trans_ArrivalDate= '" + startdate + "' ";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomArrival = int.Parse(dt.Rows[0][0].ToString());
                iGuestArrival = (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }

            szQuery = "SELECT COUNT(A.TransactionID), SUM(A.Trans_PaxAdult+A.Trans_PaxChild) " +
                    "FROM HotelTransGroupInfo A " +
                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                    "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (2,4) " +
                    "AND A.Trans_ArrivalDate= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomArrival += int.Parse(dt.Rows[0][0].ToString());
                iGuestArrival += (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }

            var xTrans = new Setstaticroom();
            xTrans.Roomqty = iRoomArrival.ToString();
            xTrans.Guest = iGuestArrival.ToString();
            xAryTransCheckIn.Add(xTrans);

            //--- Reservation
            szQuery = "SELECT COUNT(Rsvn_TransID), SUM(Rsvn_PaxAdult+Rsvn_PaxChild) " +
                    "FROM RsvnTransaction " +
                    "WHERE Rsvn_StatusID<90 AND Rsvn_TypeID IN (1,3) AND Rsvn_ArrivalDate= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomRsvn = int.Parse(dt.Rows[0][0].ToString());
                iGuestRsvn = (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }

            szQuery = "SELECT COUNT(A.Rsvn_TransID), SUM(A.Rsvn_PaxAdult+A.Rsvn_PaxChild) " +
                    "FROM RsvnGroupInfo A " +
                    "JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID " +
                    "WHERE B.Rsvn_StatusID<90 AND B.Rsvn_TypeID IN (2,4) AND A.Rsvn_ArrivalDate= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomRsvn += int.Parse(dt.Rows[0][0].ToString());
                iGuestRsvn += (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }
            var xTrans3 = new Setstaticroom();
            xTrans3.Roomqty = iRoomRsvn.ToString();
            xTrans3.Guest = iGuestRsvn.ToString();
            xAryTransCheckIn.Add(xTrans3);


            //--- Departure
            szQuery = "SELECT COUNT(DISTINCT A.TransactionID), SUM(Trans_PaxAdult+Trans_PaxChild) " +
                    "FROM HotelTransRoomInfo A " +
                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                    "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (1,3) " +
                    "AND B.Trans_DepartureDate= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomDeparture = int.Parse(dt.Rows[0][0].ToString());
                iGuestDeparture = (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }

            szQuery = "SELECT COUNT(A.TransactionID), SUM(A.Trans_PaxAdult+A.Trans_PaxChild) " +
                    "FROM HotelTransGroupInfo A " +
                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                    "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (2,4) AND A.Trans_DepartureDate= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomDeparture += int.Parse(dt.Rows[0][0].ToString());
                iGuestDeparture += (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
            }
            var xTrans2 = new Setstaticroom();
            xTrans2.Roomqty = iRoomDeparture.ToString();
            xTrans2.Guest = iGuestDeparture.ToString();
            xAryTransCheckIn.Add(xTrans2);


            return xAryTransCheckIn;
        }

        public static List<Setcurrentroom> Getcurrentroom(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<Setcurrentroom>();

            int iTotalRooms = 0;

            string szQ = "SELECT COUNT(*) FROM RoomNo WHERE RoomCategoryID=0 AND Deleted=0 AND Activate=1";
            DataTable dts = DBHelper.QueryListData(conn, szQ);
            if (dts.Rows[0][0] == DBNull.Value)
            {
                iTotalRooms = 0;
            }
            else
            {
                iTotalRooms = int.Parse(dts.Rows[0][0].ToString());
            }


            string szQuery = "SELECT B.Trans_RoomTypeID, C.RoomTypeName, COUNT(DISTINCT A.TransactionID) AS CountTrans, SUM(Trans_PaxAdult+Trans_PaxChild) AS SumGuest " +
            "FROM HotelTransRoomInfo A " +
            "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            "JOIN RoomType C ON B.Trans_RoomTypeID=C.RoomTypeID " +
            "WHERE A.Trans_Date= '" + startdate + "' AND B.Trans_StatusID<90 " +
            "GROUP BY B.Trans_RoomTypeID " +
            "UNION " +
            "SELECT A.Trans_RoomTypeID, D.RoomTypeName, COUNT(A.TransactionID) AS CountTrans, SUM(A.Trans_PaxAdult+A.Trans_PaxChild) AS SumGuest " +
            "FROM HotelTransGroupInfo A " +
            "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
            "JOIN HotelTransGroupDetail C ON A.TransactionID=C.TransactionID  AND A.Trans_GroupID=C.Trans_GroupID " +
            "JOIN RoomType D ON A.Trans_RoomTypeID=D.RoomTypeID " +
            "WHERE C.Trans_Date= '" + startdate + "' AND B.Trans_StatusID<90 " +
            "GROUP BY A.Trans_RoomTypeID";

            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            DataView dw = dt.DefaultView;
            DataTable dtRoomTypeID = dw.ToTable(true, "Trans_RoomTypeID");
            for (int i = 0; i < dtRoomTypeID.Rows.Count; i++)
            {
                var xTrans = new Setcurrentroom();

                int iRoomTypeID = int.Parse(dtRoomTypeID.Rows[i][0].ToString());
                DataRow[] row = dt.Select("Trans_RoomTypeID=" + iRoomTypeID);
                decimal iTotalRoom = 0, iTotalGuest = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    if (int.Parse(dt.Rows[k][0].ToString()) == int.Parse(row[0][0].ToString()))
                    {
                        iTotalRoom += int.Parse(dt.Rows[k][2].ToString());
                        iTotalGuest += int.Parse(dt.Rows[k][3].ToString());
                    }
                }
                decimal fPercentRoom = (iTotalRoom * 100) / iTotalRooms;

                xTrans.Item = row[0]["RoomTypeName"].ToString();
                xTrans.Roomqty = iTotalRoom.ToString();
                xTrans.Guest = iTotalGuest.ToString();
                xTrans.Occ = fPercentRoom.ToString("#,0.00") + "%";
                xAryTransCheckIn.Add(xTrans);
            }
            return xAryTransCheckIn;
        }

        public static List<Setinhouse> GetinHouses(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<Setinhouse>();

            int iRoomInHouseFIT = 0, iGuestInHouseFIT = 0;
            int iRoomInHouseGroup = 0, iGuestInHouseGroup = 0;
            decimal fOccInHouseFIT = 0, fOccInHouseGroup = 0;
            int iTotalRooms = 0;

            string szQ = "SELECT COUNT(*) FROM RoomNo WHERE RoomCategoryID=0 AND Deleted=0 AND Activate=1";
            DataTable dts = DBHelper.QueryListData(conn, szQ);

            if (dts.Rows[0][0] == DBNull.Value)
            {
                iTotalRooms = 0;
            }
            else
            {
                iTotalRooms = int.Parse(dts.Rows[0][0].ToString());
            }

            string szQuery = "SELECT COUNT(DISTINCT A.TransactionID) AS CountTrans, " +
                       "SUM(Trans_PaxAdult+Trans_PaxChild) AS SumGuest " +
                       "FROM HotelTransRoomInfo A " +
                       "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                       "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (1,3) AND A.Trans_Date= '" + startdate + "'";

            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomInHouseFIT = int.Parse(dt.Rows[0][0].ToString());
                iGuestInHouseFIT = (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
                fOccInHouseFIT = (decimal)(iRoomInHouseFIT * 100) / iTotalRooms;
            }

            szQuery = "SELECT COUNT(A.TransactionID) AS CountTrans, " +
                    "SUM(A.Trans_PaxAdult+A.Trans_PaxChild) AS SumGuest " +
                    "FROM HotelTransGroupInfo A " +
                    "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                    "JOIN HotelTransGroupDetail C ON A.TransactionID=C.TransactionID  AND A.Trans_GroupID=C.Trans_GroupID " +
                    "WHERE B.Trans_StatusID<90 AND B.Trans_TypeID IN (2,4) AND C.Trans_Date= '" + startdate + "'";
            dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows.Count > 0)
            {
                iRoomInHouseGroup = int.Parse(dt.Rows[0][0].ToString());
                iGuestInHouseGroup = (dt.Rows[0][1] == DBNull.Value) ? 0 : int.Parse(dt.Rows[0][1].ToString());
                fOccInHouseGroup = (decimal)(iRoomInHouseGroup * 100) / iTotalRooms;
            }
            var xTrans = new Setinhouse();
            xTrans.Roomqty = iRoomInHouseFIT.ToString();
            xTrans.Guest = iGuestInHouseFIT.ToString();
            xTrans.Occ = fOccInHouseFIT.ToString("#,0.00") + "%";
            xAryTransCheckIn.Add(xTrans);

            var xTrans2 = new Setinhouse();
            xTrans2.Roomqty = iRoomInHouseGroup.ToString();
            xTrans2.Guest = iGuestInHouseGroup.ToString();
            xTrans2.Occ = fOccInHouseGroup.ToString("#,0.00") + "%";
            xAryTransCheckIn.Add(xTrans2);

            int iTotalOcc = iRoomInHouseFIT + iRoomInHouseGroup, iTotalVacant = iTotalRooms - iRoomInHouseFIT - iRoomInHouseGroup;
            decimal fPercentOcc = (decimal)(iTotalOcc * 100) / iTotalRooms;
            decimal fPercentVacant = 100 - fPercentOcc;

            var xTransoc = new Setinhouse();
            xTransoc.Roomqty = iTotalOcc.ToString();
            xTransoc.Occ = fPercentOcc.ToString("#,0.00") + "%";
            xAryTransCheckIn.Add(xTransoc);

            var xTransva = new Setinhouse();
            xTransva.Roomqty = iTotalVacant.ToString();
            xTransva.Occ = fPercentVacant.ToString("#,0.00") + "%";
            xAryTransCheckIn.Add(xTransva);

            return xAryTransCheckIn;
        }

        public static List<Setinhouse> GetCurRoomBusinessSource(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<Setinhouse>();

            int iTotalRooms = 0;

            string szQ = "SELECT COUNT(*) FROM RoomNo WHERE RoomCategoryID=0 AND Deleted=0 AND Activate=1";
            DataTable dts = DBHelper.QueryListData(conn, szQ);

            if (dts.Rows[0][0] == DBNull.Value)
            {
                iTotalRooms = 0;
            }
            else
            {
                iTotalRooms = int.Parse(dts.Rows[0][0].ToString());
            }

            string szQuery = "SELECT B.GuestChannelID, C.GuestChannelName, " +
                       "COUNT(DISTINCT A.TransactionID) AS CountTrans, SUM(Trans_PaxAdult+Trans_PaxChild) AS SumGuest " +
                       "FROM HotelTransRoomInfo A " +
                       "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                       "JOIN GuestChannel C ON B.GuestChannelID=C.GuestChannelID " +
                       "WHERE B.Trans_StatusID<90 AND A.Trans_Date= '" + startdate + "' " +
                       "GROUP BY B.GuestChannelID " +
                       "UNION " +
                       "SELECT B.GuestChannelID, D.GuestChannelName, " +
                       "COUNT(A.TransactionID) AS CountTrans, SUM(A.Trans_PaxAdult+A.Trans_PaxChild) AS SumGuest " +
                       "FROM HotelTransGroupInfo A " +
                       "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                       "JOIN HotelTransGroupDetail C ON A.TransactionID=C.TransactionID  AND A.Trans_GroupID=C.Trans_GroupID " +
                       "JOIN GuestChannel D ON B.GuestChannelID=D.GuestChannelID " +
                       "WHERE B.Trans_StatusID<90 AND C.Trans_Date= '" + startdate + "' " +
                       "GROUP BY B.GuestChannelID";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            DataView dw = dt.DefaultView;
            DataTable dtChannelID = dw.ToTable(true, "GuestChannelID");
            for (int i = 0; i < dtChannelID.Rows.Count; i++)
            {
                int iChannelID = int.Parse(dtChannelID.Rows[i][0].ToString());
                DataRow[] row = dt.Select("GuestChannelID=" + iChannelID);
                decimal iTotalRoom = 0, iTotalGuest = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    if (int.Parse(dt.Rows[k][0].ToString()) == int.Parse(row[0][0].ToString()))
                    {
                        iTotalRoom += int.Parse(dt.Rows[k][2].ToString());
                        iTotalGuest += int.Parse(dt.Rows[k][3].ToString());
                    }
                }
                decimal fPercentRoom = (iTotalRoom * 100) / iTotalRooms;
                string name = dt.Rows[i]["GuestChannelName"].ToString();

                var xTransva = new Setinhouse();
                xTransva.Guestchanel = name.ToString();
                xTransva.Roomqty = iTotalRoom.ToString();
                xTransva.Guest = iTotalGuest.ToString();
                xTransva.Occ = fPercentRoom.ToString("#,0.00") + "%";
                xAryTransCheckIn.Add(xTransva);
            }

            return xAryTransCheckIn;
        }

        public static List<SetRevenuestatic> GetCurRevenues(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<SetRevenuestatic>();

            decimal fAmountABF = 0, fAmountRoomCharge = 0;
            int iTotalRooms = 0;

            string szQ = "SELECT COUNT(*) FROM RoomNo WHERE RoomCategoryID=0 AND Deleted=0 AND Activate=1";
            DataTable dts = DBHelper.QueryListData(conn, szQ);

            if (dts.Rows[0][0] == DBNull.Value)
            {
                iTotalRooms = 0;
            }
            else
            {
                iTotalRooms = int.Parse(dts.Rows[0][0].ToString());
            }

            string szQuery = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                          "FROM HotelFolioDetail A " +
                          "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                          "WHERE A.FolioStatusID<>99 AND FolioItemID=1001 " +
                          "AND A.FolioDetailDate= '" + startdate + "'";
            DataTable dtFit1 = DBHelper.QueryListData(conn, szQuery);

            string szQuery2 = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                "FROM HotelFolioGroupDetail A " +
                "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                "WHERE A.FolioStatusID<>99 AND FolioItemID=1001 " +
                "AND A.FolioDetailDate= '" + startdate + "'";

            DataTable dtGroup = DBHelper.QueryListData(conn, szQuery2);

            if (dtFit1.Rows.Count >= 0)
            {
                fAmountRoomCharge = ((dtFit1.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtFit1.Rows[0][0].ToString()));
            }
            if (dtGroup.Rows.Count >= 0)
            {
                fAmountRoomCharge += ((dtGroup.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtGroup.Rows[0][0].ToString()));
            }

            //--- ABF
            string szQuery3 = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                          "FROM HotelFolioDetail A " +
                          "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                          "WHERE A.FolioStatusID<>99 AND FolioItemID=1002 " +
                          "AND A.FolioDetailDate= '" + startdate + "'";
            DataTable dtFIT2 = DBHelper.QueryListData(conn, szQuery3);

            string szQuery4 = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                "FROM HotelFolioGroupDetail A " +
                "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                "WHERE A.FolioStatusID<>99 AND FolioItemID=1002 " +
                "AND A.FolioDetailDate= '" + startdate + "'";

            DataTable dtGroup2 = DBHelper.QueryListData(conn, szQuery4);

            if (dtFIT2.Rows.Count > 0)
            {
                fAmountABF = ((dtFIT2.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtFIT2.Rows[0][0].ToString()));
            }
            if (dtGroup2.Rows.Count > 0)
            {
                fAmountABF += ((dtGroup2.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtGroup2.Rows[0][0].ToString()));
            }

            decimal fAvgRateRoomCharge = fAmountRoomCharge / (decimal)iTotalRooms;
            decimal fAvgRateABF = fAmountABF / (decimal)iTotalRooms;


            var xTransva = new SetRevenuestatic();
            xTransva.Avg = fAvgRateRoomCharge.ToString("N");
            xTransva.Revenue = fAmountRoomCharge.ToString("N");
            xAryTransCheckIn.Add(xTransva);

            var xTransvb = new SetRevenuestatic();
            xTransvb.Avg = fAvgRateABF.ToString("N");
            xTransvb.Revenue = fAmountABF.ToString("N");
            xAryTransCheckIn.Add(xTransvb);

            return xAryTransCheckIn;
        }

        public static List<Setpayments> GetCurPayments(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate.ToString(format, cur);
            var xAryTransCheckIn = new List<Setpayments>();
            
            decimal fCash = GetTotalPaymentAmount(conn, startdate,1);
            if (fCash >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment Cash";
                xTran.payment = fCash.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fCreditCard = GetTotalPaymentAmount(conn, startdate, 11);
            if (fCreditCard >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment Credit Card";
                xTran.payment = fCreditCard.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fCheque = GetTotalPaymentAmount(conn, startdate, 31);
            if (fCheque >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment Cheque";
                xTran.payment = fCheque.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fBankTransfer = GetTotalPaymentAmount(conn, startdate, 41);
            if (fBankTransfer >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment Bank Transfer";
                xTran.payment = fBankTransfer.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fCityLedger = GetTotalPaymentAmount(conn, startdate, 51);
            if (fCityLedger >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment City Ledger";
                xTran.payment = fCityLedger.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fDepositCash = GetTotalPaymentAmount(conn, startdate, 201);
            if (fDepositCash >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Deposit Cash";
                xTran.payment = fDepositCash.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fDepositCreditCard = GetTotalPaymentAmount(conn, startdate, 202);
            if (fDepositCreditCard >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Deposit Credit Card";
                xTran.payment = fDepositCreditCard.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fDepositCheque = GetTotalPaymentAmount(conn, startdate, 203);
            if (fDepositCheque >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Deposit Cheque";
                xTran.payment = fDepositCheque.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fDepositBankTransfer = GetTotalPaymentAmount(conn, startdate, 204);
            if (fDepositBankTransfer >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Payment Bank Transger";
                xTran.payment = fDepositBankTransfer.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            decimal fDepositCityLedger = GetTotalPaymentAmount(conn, startdate, 205);
            if (fDepositCityLedger >= 0)
            {
                var xTran = new Setpayments();
                xTran.Item = "Deposit City Ledger";
                xTran.payment = fDepositCityLedger.ToString("N");
                xAryTransCheckIn.Add(xTran);
            }

            return xAryTransCheckIn;
        }
        static decimal GetTotalPaymentAmount(MySqlConnection conn, string dtHotelDate,int iItemID)
        {
            string szFmtFIT = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                              "FROM HotelFolioDetail A " +
                              "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                              "WHERE A.FolioStatusID<>99 AND FolioItemID="+ iItemID + " " +
                              "AND A.FolioDetailDate='"+ dtHotelDate + "'";

            string szFmtGroup = "SELECT SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice " +
                               "FROM HotelFolioGroupDetail A " +
                               "JOIN HotelTransaction B ON A.TransactionID=B.TransactionID " +
                               "WHERE A.FolioStatusID<>99 AND FolioItemID=" + iItemID + " " +
                               "AND A.FolioDetailDate='" + dtHotelDate + "'";

            DataTable dtFIT = DBHelper.QueryListData(conn, szFmtFIT);

            DataTable dtGroup = DBHelper.QueryListData(conn, szFmtGroup);

            decimal fPayment = 0;
            if (dtFIT.Rows.Count > 0)
                fPayment = ((dtFIT.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtFIT.Rows[0][0].ToString()));
            if (dtGroup.Rows.Count > 0)
                fPayment += ((dtGroup.Rows[0][0] == DBNull.Value) ? 0 : decimal.Parse(dtGroup.Rows[0][0].ToString()));

            return fPayment;
        }
    }
}
