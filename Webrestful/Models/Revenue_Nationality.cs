using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Revenue_Nationality
    {
        public static List<Nation> GetNationNight(MySqlConnection conn, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<Nation>();
                string Nation = "SELECT GuestNationalityID, NationalityName, SUM(RoomNight) AS SumRoomNight FROM (SELECT B.GuestNationalityID, N.NationalityName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN Nationality N ON B.GuestNationalityID=N.NationalityID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_date >= '"+szDate+"'  and C.Trans_date<'"+szDate2+"' GROUP BY B.GuestNationalityID UNION ALL SELECT B.GuestNationalityID, N.NationalityName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN Nationality N ON B.GuestNationalityID=N.NationalityID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_date >= '"+szDate+"' and D.Trans_date<'"+szDate2+"' GROUP BY B.GuestNationalityID) AS A GROUP BY GuestNationalityID ORDER BY SumRoomNight DESC, NationalityName;";
                DataTable dtFIT = DBHelper.QueryListData(conn, Nation);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new Nation();
                    xTrans.id = dtFIT.Rows[i]["GuestNationalityID"].ToString();
                    xTrans.name = dtFIT.Rows[i]["NationalityName"].ToString();
                    xTrans.roomNight = dtFIT.Rows[i]["SumRoomNight"].ToString();
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
        public static List<TransID> GetTransID(MySqlConnection conn, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<TransID>();
                string TransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + szDate + "' AND C.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + szDate + "' AND D.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID";
                DataTable dtFIT = DBHelper.QueryListData(conn, TransID);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new TransID();
                    xTrans.id = dtFIT.Rows[i]["TransactionID"].ToString();
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
        public static List<NationObject> GetRR(MySqlConnection conn, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                string id = "";
                string TransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + szDate + "' AND C.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + szDate + "' AND D.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID";
                DataTable dtFIT2 = DBHelper.QueryListData(conn, TransID);
                for (int i = 0; i < dtFIT2.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        id = "(" + dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    } if (i == (dtFIT2.Rows.Count - 1))
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ")";
                    }
                    else
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    }
                }
                Debug.WriteLine(id);
                var xAryTransCheckIn = new List<NationObject>();
                string RR = "SELECT SUM(SumPrice) AS SUM, GuestNationalityID FROM (SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN GuestInfo C ON C.GuestID=B.GuestID WHERE A.TransactionID IN "+id+" AND FolioDetailDate>='"+szDate+"' AND FolioDetailDate<'"+szDate2+"' AND FolioItemID=1001 AND A.FolioStatusID<>99 AND A.FolioStatusID<90 Group by C.GuestNationalityID UNION ALL SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN GuestInfo C ON C.GuestID=B.GuestID WHERE A.TransactionID IN"+id+" AND FolioDetailDate>='"+szDate+"' AND FolioDetailDate<'"+szDate2+"' AND FolioItemID=1001 AND A.FolioStatusID<>99 AND A.FolioStatusID<90 Group by C.GuestNationalityID) AS A GROUP BY GuestNationalityID";
                DataTable dtFIT = DBHelper.QueryListData(conn, RR);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new NationObject();
                    xTrans.sum = dtFIT.Rows[i]["SUM"].ToString();
                    xTrans.id = dtFIT.Rows[i]["GuestNationalityID"].ToString();
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
        public static List<NationObject> GetABF(MySqlConnection conn, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                string id = "";
                string TransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + szDate + "' AND C.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + szDate + "' AND D.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID";
                DataTable dtFIT2 = DBHelper.QueryListData(conn, TransID);
                for (int i = 0; i < dtFIT2.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        id = "(" + dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    } if (i == (dtFIT2.Rows.Count - 1))
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ")";
                    }
                    else
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    }
                }
                var xAryTransCheckIn = new List<NationObject>();
                string ABF = "SELECT SUM(SumPrice) AS SUM, GuestNationalityID FROM (SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN GuestInfo C ON C.GuestID=B.GuestID WHERE A.TransactionID IN " + id + " AND FolioDetailDate>='" + szDate + "' AND FolioDetailDate<'" + szDate2 + "' AND FolioItemID=1002 AND A.FolioStatusID<>99 AND A.FolioStatusID<90 Group by C.GuestNationalityID UNION ALL SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN GuestInfo C ON C.GuestID=B.GuestID WHERE A.TransactionID IN" + id + " AND FolioDetailDate>='" + szDate + "' AND FolioDetailDate<'" + szDate2 + "' AND FolioItemID=1002 AND A.FolioStatusID<>99 AND A.FolioStatusID<90 Group by C.GuestNationalityID) AS A GROUP BY GuestNationalityID";
                DataTable dtFIT = DBHelper.QueryListData(conn, ABF);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new NationObject();
                    xTrans.sum = dtFIT.Rows[i]["SUM"].ToString();
                    xTrans.id = dtFIT.Rows[i]["GuestNationalityID"].ToString();
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
        public static List<NationObject> GetOther(MySqlConnection conn, string szDate, string szDate2, ref string szErrMsg)
        {
            try
            {
                string id = "";
                string TransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + szDate + "' AND C.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN GuestInfo B ON A.GuestID=B.GuestID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + szDate + "' AND D.Trans_Date<'" + szDate2 + "' AND B.GuestNationalityID>=0 GROUP BY A.TransactionID";
                DataTable dtFIT2 = DBHelper.QueryListData(conn, TransID);
                for (int i = 0; i < dtFIT2.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        id = "(" + dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    } if (i == (dtFIT2.Rows.Count - 1))
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ")";
                    }
                    else
                    {
                        id += dtFIT2.Rows[i]["TransactionID"].ToString() + ",";
                    }
                }
                var xAryTransCheckIn = new List<NationObject>();
                string ABF = "SELECT SUM(SumPrice) as SUM, GuestNationalityID FROM (SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID JOIN HotelTransaction D ON A.TransactionID=D.TransactionID JOIN GuestInfo C ON C.GuestID=D.GuestID WHERE A.TransactionID IN "+id+" AND FolioDetailDate>='"+szDate+"' and FolioDetailDate<'"+szDate2+"' AND A.FolioStatusID<90 AND HotelItemServiceID NOT IN(1001,1002) AND ItemSign=1 GROUP BY C.GuestNationalityID UNION ALL SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice, C.GuestNationalityID FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID JOIN HotelTransaction D ON A.TransactionID=D.TransactionID JOIN GuestInfo C ON C.GuestID=D.GuestID WHERE A.TransactionID IN "+id+" AND FolioDetailDate>='"+szDate+"' and FolioDetailDate<'"+szDate2+"' AND A.FolioStatusID<90 AND HotelItemServiceID NOT IN(1001,1002) AND ItemSign=1 GROUP BY C.GuestNationalityID) AS A GROUP BY GuestNationalityID";
                DataTable dtFIT = DBHelper.QueryListData(conn, ABF);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new NationObject();
                    xTrans.sum = dtFIT.Rows[i]["SUM"].ToString();
                    xTrans.id = dtFIT.Rows[i]["GuestNationalityID"].ToString();
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