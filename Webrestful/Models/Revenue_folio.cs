using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Revenue_folio
    {
        public static List<Setrevenue_folio> revenuefoliotoday(MySqlConnection conn, DateTime dtHotelDate1, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<Setrevenue_folio>();

            string format = "yyyy-MM-dd";
            CultureInfo cur = new CultureInfo("en-US");
            string startdate = dtHotelDate1.ToString(format, cur);

            string sqlstring = "SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, " +
                "SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, " +
                "SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge " +
                "FROM HotelFolioDetail A " +
                "JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID " +
                "WHERE A.FolioStatusID<>99 AND B.ItemSign=1 " +
                "AND A.FolioDetailDate= '" + startdate + "' " +
                "AND B.HotelItemServiceID>1000 " +
                "GROUP BY A.FolioItemID " +
                "UNION ALL " +
                "SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, " +
                "SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, " +
                 "SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge " +
                "FROM HotelFolioGroupDetail A " +
                "JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID " +
                "WHERE A.FolioStatusID<>99 AND B.ItemSign=1 " +
                "AND A.FolioDetailDate= '" + startdate + "' " +
                "AND B.HotelItemServiceID>1000 " +
                "GROUP BY A.FolioItemID ";
            string matcur = ("N");
            decimal SRevenue = 0;
            decimal SVat = 0;
            decimal SService = 0;
            decimal STotal = 0;
            DataTable dt = DBHelper.QueryListData(conn, sqlstring);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var xTrans = new Setrevenue_folio();
                xTrans.Item = dt.Rows[i]["HotelItemServiceName"].ToString();
                decimal Revenue = (dt.Rows[i]["TotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalPrice"].ToString());
                SRevenue += Revenue;
                xTrans.Revenue = Revenue.ToString(matcur);
                decimal Vat = (dt.Rows[i]["TotalVat"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalVat"].ToString());
                SVat += Vat;
                xTrans.Vat = Vat.ToString(matcur);
                decimal Service = (dt.Rows[i]["TotalSrvCharge"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalSrvCharge"].ToString());
                SService += Service;
                xTrans.Service = Service.ToString(matcur);
                decimal Total = (dt.Rows[i]["SumTotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalPrice"].ToString());
                STotal += Total;
                xTrans.Total = Total.ToString(matcur);

                xAryTransCheckIn.Add(xTrans);
            }

            xAryTransCheckIn.Add(new Setrevenue_folio { SumRevenue = SRevenue.ToString(matcur),
                SumTotal = STotal.ToString(matcur),
                SumVat = SVat.ToString(matcur),
                SumService = SService.ToString(matcur) });

            return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }

        public static List<Setrevenue_folio> revenuefoliomonth(MySqlConnection conn, DateTime dtHotelDate1, DateTime dtHotelDate2, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<Setrevenue_folio>();

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string startdate = dtHotelDate1.ToString(format, cur);
                string enddate = dtHotelDate2.ToString(format, cur);

                string sqlstring = "SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, " +
                "SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, " +
                "SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge " +
                "FROM HotelFolioDetail A " +
                "JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID " +
                "WHERE A.FolioStatusID<>99 AND B.ItemSign=1 " +
                "AND A.FolioDetailDate>= '" + startdate + "' AND A.FolioDetailDate<= '" + enddate + "' " +
                "AND B.HotelItemServiceID>1000 " +
                "GROUP BY A.FolioItemID " +
                "UNION ALL " +
                "SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, " +
                "SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, " +
                 "SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge " +
                "FROM HotelFolioGroupDetail A " +
                "JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID " +
                "WHERE A.FolioStatusID<>99 AND B.ItemSign=1 " +
                "AND A.FolioDetailDate>= '" + startdate + "' AND A.FolioDetailDate<= '" + enddate + "' " +
                "AND B.HotelItemServiceID>1000 " +
                "GROUP BY A.FolioItemID ";

                string matcur = ("N");
                decimal SRevenue = 0;
                decimal SVat = 0;
                decimal SService = 0;
                decimal STotal = 0;
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var xTrans = new Setrevenue_folio();
                    xTrans.Item = dt.Rows[i]["HotelItemServiceName"].ToString();
                    decimal Revenue = (dt.Rows[i]["TotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalPrice"].ToString());
                    SRevenue += Revenue;
                    xTrans.Revenue = Revenue.ToString(matcur);
                    decimal Vat = (dt.Rows[i]["TotalVat"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalVat"].ToString());
                    SVat += Vat;
                    xTrans.Vat = Vat.ToString(matcur);
                    decimal Service = (dt.Rows[i]["TotalSrvCharge"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalSrvCharge"].ToString());
                    SService += Service;
                    xTrans.Service = Service.ToString(matcur);
                    decimal Total = (dt.Rows[i]["SumTotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalPrice"].ToString());
                    STotal += Total;
                    xTrans.Total = Total.ToString(matcur);

                    xAryTransCheckIn.Add(xTrans);
                }

                xAryTransCheckIn.Add(new Setrevenue_folio
                {
                    SumRevenue = SRevenue.ToString(matcur),
                    SumTotal = STotal.ToString(matcur),
                    SumVat = SVat.ToString(matcur),
                    SumService = SService.ToString(matcur)
                });

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