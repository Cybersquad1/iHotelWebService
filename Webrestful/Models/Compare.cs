using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Compare
    {
        public static List<AgencyCompare> GetAgency(MySqlConnection conn, string mode, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                string[] id;
                string[] name;
                float[] TotalPrice;
                int[] rowCount;
                var xAryTransCheckIn = new List<AgencyCompare>();
                string AgencyName = "SELECT * FROM AgencyInfo ORDER BY AgencyName";
                DataTable dtFIT = DBHelper.QueryListData(conn, AgencyName);
                id = new string[dtFIT.Rows.Count];
                name = new string[dtFIT.Rows.Count];
                rowCount = new int[dtFIT.Rows.Count];
                TotalPrice = new float[dtFIT.Rows.Count];
                Debug.WriteLine("Enter Agency NAME AND ID");
                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    id[i] = dtFIT.Rows[i]["AgencyID"].ToString();
                    name[i] = dtFIT.Rows[i]["AgencyName"].ToString(); ;
                   
                }
                Debug.WriteLine("Finish get Agency NAME AND ID");
                Debug.WriteLine("Enter AtDay and TotalPrice");
                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    string AgencyCharge = "SELECT "+mode+"(FolioDetailDate) AS AtDay, SUM(TotalPrice) AS TotalPrice FROM(SELECT FolioDetailDate, B.Trans_AgencyID, SUM(TotalItemPrice + TotalItemVat + TotalServiceCharge + TotalServiceChargeVat) AS TotalPrice FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID = B.TransactionID WHERE A.FolioStatusID <> 99 AND FolioItemID IN(1001, 2001) AND B.Trans_AgencyID = "+id[i]+" AND A.FolioDetailDate >= '"+szDate+"' AND A.FolioDetailDate < '"+szDate2+"' GROUP BY FolioDetailDate, B.Trans_AgencyID UNION ALL SELECT FolioDetailDate, B.Trans_AgencyID, SUM(TotalItemPrice + TotalItemVat + TotalServiceCharge + TotalServiceChargeVat) AS TotalPrice FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID = B.TransactionID WHERE A.FolioStatusID <> 99 AND FolioItemID IN(1001, 2001) AND B.Trans_AgencyID = "+id[i]+" AND A.FolioDetailDate >= '"+szDate+"' AND A.FolioDetailDate < '"+szDate2+"' GROUP BY FolioDetailDate, B.Trans_AgencyID) AS A GROUP BY "+mode+"(FolioDetailDate)";
                    DataTable dtFIT2 = DBHelper.QueryListData(conn, AgencyCharge);
                    rowCount[i] = dtFIT2.Rows.Count;
                    Debug.WriteLine("Getting "+name[i] +" id:" + id[i]);
                    Debug.WriteLine(rowCount[i] + "," +dtFIT2.Rows.Count);
                    for (int j = 0; j < dtFIT2.Rows.Count; j++)
                    {
                        TotalPrice[i] += float.Parse(dtFIT2.Rows[j]["TotalPrice"].ToString());
                    }
                }
                Debug.WriteLine("Finish get AtDay and TotalPrice");
                //for (int j = 0; j < dtFIT.Rows.Count; j++)
                //{
                //    Debug.WriteLine("Pushing " + name[j] + " id:" + id[j]);
                //    for (int z = 0; z < rowCount[j]; z++)
                //    {
                //        Debug.WriteLine("AtDay:" + AtDay[j, z] + "TotalPrice:" + TotalPrice[j, z]);
                //    }
                //}
                for (int j = 0; j < dtFIT.Rows.Count; j++)
                {
                    var xTrans = new AgencyCompare();
                    //xTrans.total = new TotalPrices();
                    //xTrans.total.Price = new List<TotalPrice>();
                    //var xxx = new List<TotalPrice>();
                    Debug.WriteLine("Start xTrans.ID");
                    xTrans.id = id[j];
                    Debug.WriteLine("End xTrans.ID");
                    Debug.WriteLine("Start xTrans.name");
                    xTrans.name = name[j];
                    Debug.WriteLine("End xTrans.name");
                    xTrans.total = TotalPrice[j].ToString("N2");
                    //if (rowCount[j] > 0)
                    //{
                    //    Debug.WriteLine("ROWCOUNT > 0");
                    //    for (int z = 0; z < rowCount[j]; z++)
                    //    {
                    //        Debug.WriteLine("Start xTrans2");
                    //        //var xTrans2 = new TotalPrice();
                    //        Debug.WriteLine("Start xTrans2.date");
                    //        Debug.WriteLine("End xTrans2.date");
                    //        Debug.WriteLine("Start xTrans2.price");
                    //        xTrans2.price = TotalPrice[j];
                    //        Debug.WriteLine("End xTrans2.price");
                    //        //xxx.Add(xTrans2);
                    //        Debug.WriteLine("xTrans.total.Price.Add(xTrans2)");
                    //    }
                    //    xTrans.total.Price.AddRange(xxx);
                    //}
                    //else
                    //{
                    //    //
                    //}
                    //xTrans.date = AtDay[j];
                    //xTrans.total = TotalPrice[j];
                    Debug.WriteLine("xAryTransCheckIn.Add(xTrans)");
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
        public static List<Setrevenue_folio> Comparefoliomonth(MySqlConnection conn, DateTime dtHotelDate1, DateTime dtHotelDate2, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<Setrevenue_folio>();

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string startdate = dtHotelDate1.ToString(format, cur);
                string enddate = dtHotelDate2.ToString(format, cur);

                string sqlstring = "SELECT HotelItemServiceName, FolioItemID, SUM(SumTotalPrice) AS TotalPrice FROM( SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '"+startdate+"' AND A.FolioDetailDate< '"+enddate+"' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID UNION ALL SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '"+startdate+"' AND A.FolioDetailDate< '"+enddate+"' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID) AS A Group by FolioItemID;";
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
                    xTrans.Vat = dt.Rows[i]["FolioItemID"].ToString();
                    xTrans.Total = dt.Rows[i]["TotalPrice"].ToString();
                    //xTrans.Item = dt.Rows[i]["HotelItemServiceName"].ToString();
                    //decimal Revenue = (dt.Rows[i]["TotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalPrice"].ToString());
                    //SRevenue += Revenue;
                    //xTrans.Revenue = Revenue.ToString(matcur);
                    //decimal Vat = (dt.Rows[i]["TotalVat"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalVat"].ToString());
                    //SVat += Vat;
                    //xTrans.Vat = Vat.ToString(matcur);
                    //decimal Service = (dt.Rows[i]["TotalSrvCharge"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["TotalSrvCharge"].ToString());
                    //SService += Service;
                    //xTrans.Service = Service.ToString(matcur);
                    //decimal Total = (dt.Rows[i]["SumTotalPrice"] == DBNull.Value) ? 0 : decimal.Parse(dt.Rows[i]["SumTotalPrice"].ToString());
                    //STotal += Total;
                    //xTrans.Total = Total.ToString(matcur);

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
        public static List<CompareBusiness> GetBusiness(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            string[] id;
            string[] source;
            float[] total;
            try
            {
                string BusinessType = "SELECT GuestChannelID, GuestChannelName FROM GuestChannel ORDER BY Ordering;";
                DataTable dtFIT = DBHelper.QueryListData(conn, BusinessType);
                id = new string[dtFIT.Rows.Count];
                source = new string[dtFIT.Rows.Count];
                total = new float[dtFIT.Rows.Count];
                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    source[i] = dtFIT.Rows[i]["GuestChannelName"].ToString();
                    id[i] = dtFIT.Rows[i]["GuestChannelID"].ToString();
                }

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");

                var GovernmentList = new List<CompareBusiness>();

                string selectYear = dtHotelDate;
                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    string SQL = "Select Atmonth, Sum(TotalPrice) From (SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID, SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=" + id[i] + " AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID,  SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))) AS TotalPrice  FROM RsvnRoomInfo A  JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=" + id[i] + " AND B.Rsvn_StatusID IN (1,2) AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID, SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID WHERE C.GuestChannelID=" + id[i] + " AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID,  SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))) AS TotalPrice  FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID  WHERE C.GuestChannelID=" + id[i] + " AND  YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID  ) As A Group by Atmonth";
                    DataTable dtFIT2 = DBHelper.QueryListData(conn, SQL);
                    for (int j = 0; j < dtFIT2.Rows.Count; j++)
                    {
                        total[i] += float.Parse(dtFIT2.Rows[j]["Sum(TotalPrice)"].ToString());
                    }
                }

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new CompareBusiness();
                    xTrans.SourceID = id[i];
                    xTrans.BusinessSource = source[i];
                    xTrans.sum = total[i].ToString("0.");
                    GovernmentList.Add(xTrans);
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
