using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Summary_Folio
    {
        public static List<SetFolio> GetSummaryfolio(MySqlConnection conn, DateTime dtHotelDate1, DateTime dtHotelDate2, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<SetFolio>();

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string startdate = dtHotelDate1.ToString(format, cur);

                string enddate = dtHotelDate2.ToString(format, cur);

                string sqlstring = "SELECT FolioDetailDate, HotelItemDeptName, HotelItemServiceName, " +
                       "FolioItemID, VatType, ItemSign, " +
                       "SUM(TotalItemPrice) AS SumTotalItemPrice, SUM(TotalItemVat) AS SumTotalItemVat, " +
                       "SUM(TotalServiceCharge) AS SumTotalServiceCharge, SUM(TotalServiceChargeVat) AS SumTotalServiceChargeVat, " +
                       "TransactionID, Trans_GroupID, HotelItemGroupID, HotelItemServiceID FROM (" +
                       "SELECT A.FolioDetailDate, C.HotelItemDeptName, B.HotelItemServiceName, " +
                       "A.FolioItemID, A.VatType, B.ItemSign, " +
                       "A.TotalItemPrice, A.TotalItemVat, A.TotalServiceCharge, A.TotalServiceChargeVat, " +
                       "A.TransactionID, A.FolioDetailDate>='" + startdate + "'AND A.FolioDetailDate< '" + enddate + "' AS Trans_GroupID, C.HotelItemGroupID, B.HotelItemServiceID " +
                       "FROM HotelFolioDetail A " +
                       "JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID " +
                       "JOIN HotelItemDept C ON B.HotelItemDeptID=C.HotelItemDeptID " +
                       "WHERE A.FolioDetailDate>= '" + startdate + "' AND A.FolioDetailDate< '" + enddate + "' AND A.FolioStatusID<>99 " +
                       "UNION ALL " +
                       "SELECT M.FolioDetailDate, O.HotelItemDeptName, N.HotelItemServiceName, " +
                       "M.FolioItemID, M.VatType, N.ItemSign, " +
                       "M.TotalItemPrice, M.TotalItemVat, M.TotalServiceCharge, M.TotalServiceChargeVat, " +
                       "M.TransactionID, M.Trans_GroupID, O.HotelItemGroupID, N.HotelItemServiceID " +
                       "FROM HotelFolioGroupDetail M " +
                       "JOIN HotelItemService N ON M.FolioItemID=N.HotelItemServiceID " +
                       "JOIN HotelItemDept O ON N.HotelItemDeptID=O.HotelItemDeptID " +
                       "WHERE M.FolioDetailDate>= '" + startdate + "' AND M.FolioDetailDate< '" + enddate + "' AND M.FolioStatusID<>99 " +
                       ") AS A " +
                       "GROUP BY FolioDetailDate,FolioItemID ORDER BY FolioDetailDate ";
                string matcur = ("N");
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var xTrans = new SetFolio();
                    DateTime dateeiei = (DateTime)dt.Rows[i]["FolioDetailDate"];
                    xTrans.FolioDate = dateeiei.ToString("yyyy-MM-dd");
                    if(int.Parse(dt.Rows[i]["VatType"].ToString()) == 1)
                    {
                        xTrans.Vattype = "I";
                    }
                    else if (int.Parse(dt.Rows[i]["VatType"].ToString()) == 2)
                    {
                        xTrans.Vattype = "E";
                    }
                    xTrans.Item = dt.Rows[i]["HotelItemServiceName"].ToString();
                    decimal Vat = (dt.Rows[i]["SumTotalItemVat"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalItemVat"].ToString());
                    xTrans.Vat = Vat.ToString(matcur);
                    decimal Itemprice = (dt.Rows[i]["SumTotalItemPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalItemPrice"].ToString());
                    xTrans.Itemprice = Itemprice.ToString(matcur);
                    decimal SC = (dt.Rows[i]["SumTotalServiceCharge"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalServiceCharge"].ToString());
                    xTrans.SC = SC.ToString(matcur);
                    decimal SC_Vat = (dt.Rows[i]["SumTotalServiceChargeVat"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalServiceChargeVat"].ToString());
                    xTrans.SC_Vat = SC_Vat.ToString(matcur);
                    decimal Total = decimal.Parse(dt.Rows[i]["SumTotalItemPrice"].ToString()) +
                        decimal.Parse(dt.Rows[i]["SumTotalItemVat"].ToString()) +
                        decimal.Parse(dt.Rows[i]["SumTotalServiceCharge"].ToString()) +
                        decimal.Parse(dt.Rows[i]["SumTotalServiceChargeVat"].ToString());
                    Total *= int.Parse(dt.Rows[i]["ItemSign"].ToString());

                    xTrans.Total = Total.ToString(matcur);
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