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
        public static List<AgencyCompareM> GetAgencyM(MySqlConnection conn, string dtHotelDate1, string dtHotelDate2, string dtHotelDate3, string dtHotelDate4,ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<AgencyCompareM>();
                CultureInfo cur = new CultureInfo("en-US");
                string previousyear = dtHotelDate1;
                string previousyear2 = dtHotelDate2;
                string presentyear = dtHotelDate3;
                string presentyear2 = dtHotelDate4;
                Debug.WriteLine("Previous year: " + previousyear + "-" + previousyear2 + " Present year: " + presentyear + "-" + presentyear2);
                string[] oldID; string[] newID; string[] oldSUM; string[] newSUM; string[] MergeID; string[] MergeOldSUM; string[] MergeNewSUM;
                string[] oldName; string[] newName; string[] MergeName;
                int oldCount, newCount, superCount, finalCount = 0;
                ////// Get Previous Year //////
                string sqlstring = "SELECT Trans_AgencyID, AgencyName, SUM(RoomNight) AS SumRoomNight FROM ( SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=3 AND C.Trans_Date>='"+previousyear+ "' AND C.Trans_Date<'" + previousyear2 + "' GROUP BY A.Trans_AgencyID UNION SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=4 AND D.Trans_Date>='" + previousyear + "' AND D.Trans_Date<'" + previousyear2 + "' GROUP BY A.Trans_AgencyID ) AS A GROUP BY Trans_AgencyID ORDER BY SumRoomNight DESC, AgencyName";
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                oldID = new string[dt.Rows.Count];
                oldSUM = new string[dt.Rows.Count];
                oldName = new string[dt.Rows.Count];
                oldCount = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    oldID[i] = dt.Rows[i]["Trans_AgencyID"].ToString();
                    oldName[i] = dt.Rows[i]["AgencyName"].ToString();
                }
                Debug.WriteLine("Finish Get Previous Year");
                ////// Get Present Year //////
                string sqlstring2 = "SELECT Trans_AgencyID, AgencyName, SUM(RoomNight) AS SumRoomNight FROM ( SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=3 AND C.Trans_Date>='" + presentyear + "' AND C.Trans_Date<'" + presentyear2 + "' GROUP BY A.Trans_AgencyID UNION SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=4 AND D.Trans_Date>='" + presentyear + "' AND D.Trans_Date<'" + presentyear2 + "' GROUP BY A.Trans_AgencyID ) AS A GROUP BY Trans_AgencyID ORDER BY SumRoomNight DESC, AgencyName";
                DataTable dt2 = DBHelper.QueryListData(conn, sqlstring2);
                newID = new string[dt2.Rows.Count];
                newSUM = new string[dt2.Rows.Count];
                newName = new string[dt2.Rows.Count];
                newCount = dt2.Rows.Count;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    newID[i] = dt2.Rows[i]["Trans_AgencyID"].ToString();
                    newName[i] = dt2.Rows[i]["AgencyName"].ToString();
                }
                Debug.WriteLine("Finish Get Present Year");
                ///// Get Transaction ID previous Year //////
                string[] oldTransactionID = new string [oldCount];
                for(int i = 0; i < oldCount; i++)
                {
                    string sqlTransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='"+previousyear+ "' AND C.Trans_Date<'" + previousyear2 + "' AND A.Trans_TypeID=3 AND A.Trans_AgencyID="+oldID[i]+ " GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + previousyear + "' AND D.Trans_Date<'" + previousyear2 + "' AND A.Trans_TypeID=4 AND A.Trans_AgencyID="+oldID[i]+" GROUP BY A.TransactionID;";
                    DataTable dtID = DBHelper.QueryListData(conn, sqlTransID);
                    for (int j = 0;j< dtID.Rows.Count; j++)
                    {
                        if (j == 0)
                        {
                            oldTransactionID[i] = dtID.Rows[j]["TransactionID"].ToString();
                        }else
                        {
                            oldTransactionID[i] += "," + dtID.Rows[j]["TransactionID"].ToString();
                        }
                    }
                }
                Debug.WriteLine("Finish Get Transaction ID previous Year");
                Debug.WriteLine(oldTransactionID[0]);

                ///// Get Transaction ID Present Year //////
                string[] newTransactionID = new string[newCount];
                for (int i = 0; i < newCount; i++)
                {
                    string sqlTransID2 = "SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + presentyear + "' AND C.Trans_Date<'" + presentyear2 + "' AND A.Trans_TypeID=3 AND A.Trans_AgencyID=" + newID[i] + " GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + presentyear + "' AND D.Trans_Date<'" + presentyear2 + "' AND A.Trans_TypeID=4 AND A.Trans_AgencyID=" + newID[i] + " GROUP BY A.TransactionID;";
                    DataTable dtID2 = DBHelper.QueryListData(conn, sqlTransID2);
                    for (int j = 0; j < dtID2.Rows.Count; j++)
                    {
                        if (j == 0)
                        {
                            newTransactionID[i] = dtID2.Rows[j]["TransactionID"].ToString();
                        }
                        else
                        {
                            newTransactionID[i] += "," + dtID2.Rows[j]["TransactionID"].ToString(); 
                        }
                    }
                }
                Debug.WriteLine("Finish Get Transaction ID present Year");
                Debug.WriteLine(newTransactionID[0]);

                ////// Get Previous year sum //////
                for (int i = 0; i < oldCount; i++)
                {
                    string sqlSum = " SELECT SUM(SumPrice) AS TotalPrice FROM ( SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioDetail A WHERE A.TransactionID IN (" + oldTransactionID[i] + ") AND FolioDetailDate>='" + previousyear + "' AND FolioDetailDate<'" + previousyear2 + "' AND FolioItemID=1001 AND A.FolioStatusID<90 UNION SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioGroupDetail A WHERE A.TransactionID IN ("+oldTransactionID[i]+") AND FolioDetailDate>='" + previousyear + "' AND FolioDetailDate<'" + previousyear2 + "' AND FolioItemID=1001 AND A.FolioStatusID<90 ) AS A;";
                    DataTable dtSum = DBHelper.QueryListData(conn, sqlSum);
                    oldSUM[i] = dtSum.Rows[0]["TotalPrice"].ToString();
                }
                Debug.WriteLine("Finish Get Previous year sum");

                ////// Get Present year sum //////
                for (int i = 0; i < newCount; i++)
                {
                    string sqlSum2 = " SELECT SUM(SumPrice) AS TotalPrice FROM ( SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioDetail A WHERE A.TransactionID IN (" + newTransactionID[i] + ") AND FolioDetailDate>='" + presentyear + "' AND FolioDetailDate<'" + presentyear2 + "' AND FolioItemID=1001 AND A.FolioStatusID<90 UNION SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioGroupDetail A WHERE A.TransactionID IN (" + newTransactionID[i] + ") AND FolioDetailDate>='" + presentyear + "' AND FolioDetailDate<'" + presentyear2 + "' AND FolioItemID=1001 AND A.FolioStatusID<90 ) AS A;";
                    DataTable dtSum2 = DBHelper.QueryListData(conn, sqlSum2);
                    newSUM[i] = dtSum2.Rows[0]["TotalPrice"].ToString();
                }
                Debug.WriteLine("Finish Get present year sum");
                ////// MERGE YEARS //////
                if (oldCount > newCount)
                {
                    superCount = oldCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                } if (newCount > oldCount)
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }
                else
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }

                //oldCount > NewCount
                if (oldCount > newCount)
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = newSUM[j];
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = "0";
                            }
                        }
                    }
                }////END oldCount > NewCount

                //newCount > oldCount or newCount == oldCount
                else
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = oldSUM[j];
                                MergeNewSUM[i] = newSUM[i];
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = "0";
                                MergeNewSUM[i] = newSUM[i];
                            }
                        }
                    }
                } //// END newCount > oldCount or newCount == oldCount

                //Recheck
                if (oldCount > newCount)
                {
                    int offset = 0;
                    for (int i = 0; i < newCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[oldCount + offset] = newID[i];
                                MergeName[oldCount + offset] = newName[i];
                                MergeNewSUM[oldCount + offset] = newSUM[i];
                                MergeOldSUM[oldCount + offset] = "0";
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                //if (newCount > oldCount) and newCount == oldCount
                else
                {
                    int offset = 0;
                    for (int i = 0; i < oldCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[newCount + offset] = oldID[i];
                                MergeName[newCount + offset] = oldName[i];
                                MergeNewSUM[newCount + offset] = "0";
                                MergeOldSUM[newCount + offset] = oldSUM[i];
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                for (int i = 0; i < finalCount; i++)
                {
                    var xTrans = new AgencyCompareM();
                    xTrans.id = MergeID[i];
                    xTrans.name = MergeName[i];
                    xTrans.previousSum = MergeOldSUM[i];
                    xTrans.presentSum = MergeNewSUM[i];
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
        public static List<AgencyCompareM> GetAgencyY(MySqlConnection conn, string dtHotelDate1, string dtHotelDate2, string dtHotelDate3, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<AgencyCompareM>();
                CultureInfo cur = new CultureInfo("en-US");
                string previousyear = dtHotelDate1;
                string presentyear = dtHotelDate2;
                string nextyear = dtHotelDate3;
                //Debug.WriteLine("Previous year: " + previousyear + "-" + previousyear2 + " Present year: " + presentyear + "-" + presentyear2);
                string[] oldID; string[] newID; string[] oldSUM; string[] newSUM; string[] MergeID; string[] MergeOldSUM; string[] MergeNewSUM;
                string[] oldName; string[] newName; string[] MergeName;
                int oldCount, newCount, superCount, finalCount = 0;
                ////// Get Previous Year //////
                string sqlstring = "SELECT Trans_AgencyID, AgencyName, SUM(RoomNight) AS SumRoomNight FROM ( SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=3 AND C.Trans_Date>='" + previousyear + "' AND C.Trans_Date<'" + presentyear + "' GROUP BY A.Trans_AgencyID UNION SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=4 AND D.Trans_Date>='" + previousyear + "' AND D.Trans_Date<'" + presentyear + "' GROUP BY A.Trans_AgencyID ) AS A GROUP BY Trans_AgencyID ORDER BY SumRoomNight DESC, AgencyName";
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                oldID = new string[dt.Rows.Count];
                oldSUM = new string[dt.Rows.Count];
                oldName = new string[dt.Rows.Count];
                oldCount = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    oldID[i] = dt.Rows[i]["Trans_AgencyID"].ToString();
                    oldName[i] = dt.Rows[i]["AgencyName"].ToString();
                }
                Debug.WriteLine("Finish Get Previous Year");
                ////// Get Present Year //////
                string sqlstring2 = "SELECT Trans_AgencyID, AgencyName, SUM(RoomNight) AS SumRoomNight FROM ( SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=3 AND C.Trans_Date>='" + presentyear + "' AND C.Trans_Date<'" + nextyear + "' GROUP BY A.Trans_AgencyID UNION SELECT A.Trans_AgencyID, B.AgencyName, COUNT(Trans_Date) AS RoomNight FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND A.Trans_TypeID=4 AND D.Trans_Date>='" + presentyear + "' AND D.Trans_Date<'" + nextyear + "' GROUP BY A.Trans_AgencyID ) AS A GROUP BY Trans_AgencyID ORDER BY SumRoomNight DESC, AgencyName";
                DataTable dt2 = DBHelper.QueryListData(conn, sqlstring2);
                newID = new string[dt2.Rows.Count];
                newSUM = new string[dt2.Rows.Count];
                newName = new string[dt2.Rows.Count];
                newCount = dt2.Rows.Count;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    newID[i] = dt2.Rows[i]["Trans_AgencyID"].ToString();
                    newName[i] = dt2.Rows[i]["AgencyName"].ToString();
                }
                Debug.WriteLine("Finish Get Present Year");
                ///// Get Transaction ID previous Year //////
                string[] oldTransactionID = new string[oldCount];
                for (int i = 0; i < oldCount; i++)
                {
                    string sqlTransID = "SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + previousyear + "' AND C.Trans_Date<'" + presentyear + "' AND A.Trans_TypeID=3 AND A.Trans_AgencyID=" + oldID[i] + " GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + previousyear + "' AND D.Trans_Date<'" + presentyear + "' AND A.Trans_TypeID=4 AND A.Trans_AgencyID=" + oldID[i] + " GROUP BY A.TransactionID;";
                    DataTable dtID = DBHelper.QueryListData(conn, sqlTransID);
                    for (int j = 0; j < dtID.Rows.Count; j++)
                    {
                        if (j == 0)
                        {
                            oldTransactionID[i] = dtID.Rows[j]["TransactionID"].ToString();
                        }
                        else
                        {
                            oldTransactionID[i] += "," + dtID.Rows[j]["TransactionID"].ToString();
                        }
                    }
                }
                Debug.WriteLine("Finish Get Transaction ID previous Year");
                Debug.WriteLine(oldTransactionID[0]);

                ///// Get Transaction ID Present Year //////
                string[] newTransactionID = new string[newCount];
                for (int i = 0; i < newCount; i++)
                {
                    string sqlTransID2 = "SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransRoomInfo C ON A.TransactionID=C.TransactionID WHERE A.Trans_StatusID<90 AND C.Trans_Date>='" + presentyear + "' AND C.Trans_Date<'" + nextyear + "' AND A.Trans_TypeID=3 AND A.Trans_AgencyID=" + newID[i] + " GROUP BY A.TransactionID UNION SELECT A.TransactionID FROM HotelTransaction A JOIN AgencyInfo B ON A.Trans_AgencyID=B.AgencyID JOIN HotelTransGroupInfo C ON A.TransactionID=C.TransactionID JOIN HotelTransGroupDetail D ON C.TransactionID=D.TransactionID AND C.Trans_GroupID=D.Trans_GroupID WHERE A.Trans_StatusID<90 AND D.Trans_Date>='" + presentyear + "' AND D.Trans_Date<'" + nextyear + "' AND A.Trans_TypeID=4 AND A.Trans_AgencyID=" + newID[i] + " GROUP BY A.TransactionID;";
                    DataTable dtID2 = DBHelper.QueryListData(conn, sqlTransID2);
                    for (int j = 0; j < dtID2.Rows.Count; j++)
                    {
                        if (j == 0)
                        {
                            newTransactionID[i] = dtID2.Rows[j]["TransactionID"].ToString();
                        }
                        else
                        {
                            newTransactionID[i] += "," + dtID2.Rows[j]["TransactionID"].ToString();
                        }
                    }
                }
                Debug.WriteLine("Finish Get Transaction ID present Year");
                Debug.WriteLine(newTransactionID[0]);

                ////// Get Previous year sum //////
                for (int i = 0; i < oldCount; i++)
                {
                    string sqlSum = " SELECT SUM(SumPrice) AS TotalPrice FROM ( SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioDetail A WHERE A.TransactionID IN (" + oldTransactionID[i] + ") AND FolioDetailDate>='" + previousyear + "' AND FolioDetailDate<'" + presentyear + "' AND FolioItemID=1001 AND A.FolioStatusID<90 UNION SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioGroupDetail A WHERE A.TransactionID IN (" + oldTransactionID[i] + ") AND FolioDetailDate>='" + previousyear + "' AND FolioDetailDate<'" + presentyear + "' AND FolioItemID=1001 AND A.FolioStatusID<90 ) AS A;";
                    DataTable dtSum = DBHelper.QueryListData(conn, sqlSum);
                    oldSUM[i] = dtSum.Rows[0]["TotalPrice"].ToString();
                }
                Debug.WriteLine("Finish Get Previous year sum");

                ////// Get Present year sum //////
                for (int i = 0; i < newCount; i++)
                {
                    string sqlSum2 = " SELECT SUM(SumPrice) AS TotalPrice FROM ( SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioDetail A WHERE A.TransactionID IN (" + newTransactionID[i] + ") AND FolioDetailDate>='" + presentyear + "' AND FolioDetailDate<'" + nextyear + "' AND FolioItemID=1001 AND A.FolioStatusID<90 UNION SELECT SUM(TotalItemPrice+TotalItemVat) AS SumPrice FROM HotelFolioGroupDetail A WHERE A.TransactionID IN (" + newTransactionID[i] + ") AND FolioDetailDate>='" + presentyear + "' AND FolioDetailDate<'" + nextyear + "' AND FolioItemID=1001 AND A.FolioStatusID<90 ) AS A;";
                    DataTable dtSum2 = DBHelper.QueryListData(conn, sqlSum2);
                    newSUM[i] = dtSum2.Rows[0]["TotalPrice"].ToString();
                }
                Debug.WriteLine("Finish Get present year sum");
                ////// MERGE YEARS //////
                if (oldCount > newCount)
                {
                    superCount = oldCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }
                if (newCount > oldCount)
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }
                else
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }

                //oldCount > NewCount
                if (oldCount > newCount)
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = newSUM[j];
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = "0";
                            }
                        }
                    }
                }////END oldCount > NewCount

                //newCount > oldCount or newCount == oldCount
                else
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = oldSUM[j];
                                MergeNewSUM[i] = newSUM[i];
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = "0";
                                MergeNewSUM[i] = newSUM[i];
                            }
                        }
                    }
                } //// END newCount > oldCount or newCount == oldCount

                //Recheck
                if (oldCount > newCount)
                {
                    int offset = 0;
                    for (int i = 0; i < newCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[oldCount + offset] = newID[i];
                                MergeName[oldCount + offset] = newName[i];
                                MergeNewSUM[oldCount + offset] = newSUM[i];
                                MergeOldSUM[oldCount + offset] = "0";
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                //if (newCount > oldCount) and newCount == oldCount
                else
                {
                    int offset = 0;
                    for (int i = 0; i < oldCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[newCount + offset] = oldID[i];
                                MergeName[newCount + offset] = oldName[i];
                                MergeNewSUM[newCount + offset] = "0";
                                MergeOldSUM[newCount + offset] = oldSUM[i];
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                for (int i = 0; i < finalCount; i++)
                {
                    var xTrans = new AgencyCompareM();
                    xTrans.id = MergeID[i];
                    xTrans.name = MergeName[i];
                    xTrans.previousSum = MergeOldSUM[i];
                    xTrans.presentSum = MergeNewSUM[i];
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
        public static List<FolioCompare> ComparefolioYear(MySqlConnection conn, string dtHotelDate1, string dtHotelDate2, string dtHotelDate3, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<FolioCompare>();

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string previousDate = dtHotelDate1;
                string presentDate = dtHotelDate2;
                string nextDate = dtHotelDate3;
                string[] oldID; string[] newID; string[] oldSUM; string[] newSUM; string[] MergeID; string[] MergeOldSUM; string[] MergeNewSUM;
                string[] oldName; string[] newName; string[] MergeName;
                int oldCount, newCount, superCount, finalCount = 0;
                ////// Get Previous Year //////
                string sqlstring = "SELECT HotelItemServiceName, FolioItemID, SUM(SumTotalPrice) AS TotalPrice FROM( SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + previousDate + "' AND A.FolioDetailDate< '" + presentDate + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID UNION ALL SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + previousDate + "' AND A.FolioDetailDate< '" + presentDate + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID) AS A Group by FolioItemID;";
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                oldID = new string[dt.Rows.Count];
                oldSUM = new string[dt.Rows.Count];
                oldName = new string[dt.Rows.Count];
                oldCount = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    oldID[i] = dt.Rows[i]["FolioItemID"].ToString();
                    oldSUM[i] = dt.Rows[i]["TotalPrice"].ToString();
                    oldName[i] = dt.Rows[i]["HotelItemServiceName"].ToString();
                }

                ////// Get Present Year //////
                string sqlstring2 = "SELECT HotelItemServiceName, FolioItemID, SUM(SumTotalPrice) AS TotalPrice FROM( SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + presentDate + "' AND A.FolioDetailDate< '" + nextDate + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID UNION ALL SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + presentDate + "' AND A.FolioDetailDate< '" + nextDate + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID) AS A Group by FolioItemID;";
                DataTable dt2 = DBHelper.QueryListData(conn, sqlstring2);
                newID = new string[dt2.Rows.Count];
                newSUM = new string[dt2.Rows.Count];
                newName = new string[dt2.Rows.Count];
                newCount = dt2.Rows.Count;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    newID[i] = dt2.Rows[i]["FolioItemID"].ToString();
                    newSUM[i] = dt2.Rows[i]["TotalPrice"].ToString();
                    newName[i] = dt2.Rows[i]["HotelItemServiceName"].ToString();
                }
                ////// MERGE YEARS //////
                if (oldCount > newCount)
                {
                    superCount = oldCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                } if (newCount > oldCount)
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }
                else
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }

                //oldCount > NewCount
                if (oldCount > newCount)
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = newSUM[j];
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = "0";
                            }
                        }
                    }
                }////END oldCount > NewCount

                //newCount > oldCount or newCount == oldCount
                else
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = oldSUM[j];
                                MergeNewSUM[i] = newSUM[i];
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = "0";
                                MergeNewSUM[i] = newSUM[i];
                            }
                        }
                    }
                } //// END newCount > oldCount or newCount == oldCount

                //Recheck
                if (oldCount > newCount)
                {
                    int offset = 0;
                    for (int i = 0; i < newCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[oldCount + offset] = newID[i];
                                MergeName[oldCount + offset] = newName[i];
                                MergeNewSUM[oldCount + offset] = newSUM[i];
                                MergeOldSUM[oldCount + offset] = "0";
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                //if (newCount > oldCount) and newCount == oldCount
                else
                {
                    int offset = 0;
                    for (int i = 0; i < oldCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[newCount + offset] = oldID[i];
                                MergeName[newCount + offset] = oldName[i];
                                MergeNewSUM[newCount + offset] = "0";
                                MergeOldSUM[newCount + offset] = oldSUM[i];
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                for (int i = 0; i < finalCount; i++)
                {
                    var xTrans = new FolioCompare();
                    xTrans.id = MergeID[i];
                    xTrans.name = MergeName[i];
                    xTrans.previousSum = MergeOldSUM[i];
                    xTrans.presentSum = MergeNewSUM[i];
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
        public static List<FolioCompare> ComparefolioMonth(MySqlConnection conn, string dtHotelDate1, string dtHotelDate2, string dtHotelDate3, string dtHotelDate4,ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<FolioCompare>();

                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string previousyear = dtHotelDate1;
                string previousyear2 = dtHotelDate2;
                string presentyear = dtHotelDate3;
                string presentyear2 = dtHotelDate4;
                Debug.WriteLine("Previous year: " + previousyear + "-" + previousyear2 + " Present year: " + presentyear + "-" + presentyear2);
                string[] oldID; string[] newID; string[] oldSUM; string[] newSUM; string[] MergeID; string[] MergeOldSUM; string[] MergeNewSUM;
                string[] oldName; string[] newName; string[] MergeName;
                int oldCount, newCount, superCount, finalCount = 0;
                ////// Get Previous Year //////
                string sqlstring = "SELECT HotelItemServiceName, FolioItemID, SUM(SumTotalPrice) AS TotalPrice FROM( SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + previousyear + "' AND A.FolioDetailDate< '" + previousyear2 + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID UNION ALL SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + previousyear + "' AND A.FolioDetailDate< '" + previousyear2 + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID) AS A Group by FolioItemID;";
                DataTable dt = DBHelper.QueryListData(conn, sqlstring);
                oldID = new string[dt.Rows.Count];
                oldSUM = new string[dt.Rows.Count];
                oldName = new string[dt.Rows.Count];
                oldCount = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    oldID[i] = dt.Rows[i]["FolioItemID"].ToString();
                    oldSUM[i] = dt.Rows[i]["TotalPrice"].ToString();
                    oldName[i] = dt.Rows[i]["HotelItemServiceName"].ToString();
                }

                ////// Get Present Year //////
                string sqlstring2 = "SELECT HotelItemServiceName, FolioItemID, SUM(SumTotalPrice) AS TotalPrice FROM( SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + presentyear + "' AND A.FolioDetailDate< '" + presentyear2 + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID UNION ALL SELECT FolioDetailDate, FolioItemID, HotelItemServiceName, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS SumTotalPrice, SUM(TotalItemPrice) AS TotalPrice, SUM(TotalItemVat+TotalServiceChargeVat) AS TotalVat, SUM(TotalServiceCharge) AS TotalSrvCharge FROM HotelFolioGroupDetail A JOIN HotelItemService B ON A.FolioItemID=B.HotelItemServiceID WHERE A.FolioStatusID<>99 AND B.ItemSign=1 AND A.FolioDetailDate>= '" + presentyear + "' AND A.FolioDetailDate< '" + presentyear2 + "' AND B.HotelItemServiceID>1000 GROUP BY A.FolioItemID) AS A Group by FolioItemID;";
                DataTable dt2 = DBHelper.QueryListData(conn, sqlstring2);
                newID = new string[dt2.Rows.Count];
                newSUM = new string[dt2.Rows.Count];
                newName = new string[dt2.Rows.Count];
                newCount = dt2.Rows.Count;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    newID[i] = dt2.Rows[i]["FolioItemID"].ToString();
                    newSUM[i] = dt2.Rows[i]["TotalPrice"].ToString();
                    newName[i] = dt2.Rows[i]["HotelItemServiceName"].ToString();
                }
                ////// MERGE YEARS //////
                if (oldCount > newCount)
                {
                    superCount = oldCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                } if (newCount > oldCount)
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }
                else
                {
                    superCount = newCount;
                    finalCount = superCount;
                    MergeID = new string[oldCount + newCount];
                    MergeName = new string[oldCount + newCount];
                    MergeNewSUM = new string[oldCount + newCount];
                    MergeOldSUM = new string[oldCount + newCount];
                }

                //oldCount > NewCount
                if (oldCount > newCount)
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = newSUM[j];
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[i] = oldID[i];
                                MergeName[i] = oldName[i];
                                MergeOldSUM[i] = oldSUM[i];
                                MergeNewSUM[i] = "0";
                            }
                        }
                    }
                }////END oldCount > NewCount

                //newCount > oldCount or newCount == oldCount
                else
                {
                    for (int i = 0; i < superCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = oldSUM[j];
                                MergeNewSUM[i] = newSUM[i];
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[i] = newID[i];
                                MergeName[i] = newName[i];
                                MergeOldSUM[i] = "0";
                                MergeNewSUM[i] = newSUM[i];
                            }
                        }
                    }
                } //// END newCount > oldCount or newCount == oldCount

                //Recheck
                if (oldCount > newCount)
                {
                    int offset = 0;
                    for (int i = 0; i < newCount; i++)
                    {
                        for (int j = 0; j < oldCount; j++)
                        {
                            if (newID[i] == oldID[j])
                            {
                                break;
                            }
                            if (j == oldCount - 1)
                            {
                                MergeID[oldCount + offset] = newID[i];
                                MergeName[oldCount + offset] = newName[i];
                                MergeNewSUM[oldCount + offset] = newSUM[i];
                                MergeOldSUM[oldCount + offset] = "0";
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                //if (newCount > oldCount) and newCount == oldCount
                else
                {
                    int offset = 0;
                    for (int i = 0; i < oldCount; i++)
                    {
                        for (int j = 0; j < newCount; j++)
                        {
                            if (oldID[i] == newID[j])
                            {
                                break;
                            }
                            if (j == newCount - 1)
                            {
                                MergeID[newCount + offset] = oldID[i];
                                MergeName[newCount + offset] = oldName[i];
                                MergeNewSUM[newCount + offset] = "0";
                                MergeOldSUM[newCount + offset] = oldSUM[i];
                                offset++;
                                finalCount++;
                            }
                        }
                    }
                }

                for (int i = 0; i < finalCount; i++)
                {
                    var xTrans = new FolioCompare();
                    xTrans.id = MergeID[i];
                    xTrans.name = MergeName[i];
                    xTrans.previousSum = MergeOldSUM[i];
                    xTrans.presentSum = MergeNewSUM[i];
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
        public static List<CompareBusiness> GetBusiness(MySqlConnection conn, string dtHotelDate, ref string szErrMsg)
        {
            string[] id;
            string[] source;
            double[] total;
            try
            {
                string BusinessType = "SELECT GuestChannelID, GuestChannelName FROM GuestChannel ORDER BY Ordering;";
                DataTable dtFIT = DBHelper.QueryListData(conn, BusinessType);
                id = new string[dtFIT.Rows.Count];
                source = new string[dtFIT.Rows.Count];
                total = new double[dtFIT.Rows.Count];
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
                    string SQL = "SELECT GuestChannelID, SUM(TotalPrice) as Totalprice FROM ( SELECT MONTH(A.Trans_Date) AS AtMonth, B.GuestChannelID, ROUND(SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))),2) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_TypeID<>10 AND B.GuestChannelID=" + id[i] + " AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Trans_Date) AS AtMonth, C.GuestChannelID, ROUND(SUM(A.Trans_RoomPrice + IF(Trans_AbfInc=0, Trans_AbfPrice,0)+Trans_ExtraBedPrice+IF(Trans_ExtraBedAbf=0, 0, (IF(Trans_AbfInc=0, Trans_AbfPrice,0)))),2) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID WHERE C.GuestChannelID=" + id[i] + " AND YEAR(A.Trans_Date)='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, B.GuestChannelID, ROUND(SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))),2) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE YEAR(A.Rsvn_Date)='" + selectYear + "' AND B.GuestChannelID=" + id[i] + " AND B.Rsvn_StatusID IN (1,2) AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, B.GuestChannelID UNION ALL SELECT MONTH(A.Rsvn_Date) AS AtMonth, C.GuestChannelID, ROUND(SUM(A.Rsvn_RoomPrice + IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)+Rsvn_ExtraBedPrice+IF(Rsvn_ExtraBedAbf=0, 0, (IF(Rsvn_AbfInc=0, Rsvn_AbfPrice,0)))),2) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.GuestChannelID=" + id[i] + " AND YEAR(A.Rsvn_Date)='" + selectYear + "' AND C.Rsvn_StatusID IN (1,2) AND B.Rsvn_ArrivalDate>='" + selectYear + "' GROUP BY AtMonth, C.GuestChannelID ) AS A";
                    DataTable dtFIT2 = DBHelper.QueryListData(conn, SQL);
                    if (dtFIT2.Rows[0]["Totalprice"].ToString() != "")
                        total[i] = double.Parse(dtFIT2.Rows[0]["Totalprice"].ToString());
                    else
                        total[i] = 0;
                    Debug.WriteLine(total[i]);
                }

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new CompareBusiness();
                    xTrans.SourceID = id[i];
                    xTrans.BusinessSource = source[i];
                    xTrans.sum = total[i].ToString();
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
