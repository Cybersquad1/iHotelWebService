using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class HisandFor
    {
        public string Date { get; set; }
        public string ABF { get; set; }
        public string Totalocc { get; set; }
        public string PerOcc { get; set; }
        public string Roomchg { get; set; }
        public string Avg { get; set; }
        public string Pax { get; set; }
    }
    public class Setstaticroom
    {
        public string Roomqty { get; set; }
        public string Guest { get; set; }
    }
    public class Setdashboard
    {
        public string avalible { get; set; }
        public string arrival { get; set; }
        public string departure { get; set; }
        public string ooo { get; set; }
        public string inhouse { get; set; }

    }
    public class setabf
    {
        public string abf { get; set; }
    }
    public class Setinhouse
    {
        public string Guestchanel { get; set; }
        public string Roomqty { get; set; }
        public string Guest { get; set; }
        public string Occ { get; set; }
    }
    public class Setpayments
    {
        public string Item { get; set; }
        public string payment { get; set; }
    }

    public class SetRevenuestatic
    {
        public string Avg { get; set; }
        public string Revenue { get; set; }
    }
    public class Setcurrentroom
    {
        public string Item { get; set; }
        public string Roomqty { get; set; }
        public string Guest { get; set; }
        public string Occ { get; set; }
    }
    public class SetFolio
    {
        public string FolioDate { get; set; }
        public string Item { get; set; }
        public string Vat { get; set; }
        public string SC { get; set; }
        public string SC_Vat { get; set; }
        public string Vattype { get; set; }
        public string Itemprice { get; set; }
        public string Total { get; set; }
    }
    public class SetArrival_Departure
    {
        public string AgencyName { get; set; }
        public string Roomno { get; set; }
        public string Roomtype { get; set; }
        public string R_Rate { get; set; }
        public string ABF { get; set; }
        public string Pax { get; set; }
    }
    public class SetRsvn_agency
    {
        public string AgencyName { get; set; }
        public int Roomnight { get; set; }
        public string Roomrev { get; set; }
        public string Roomavg { get; set; }
        public string Perroomnight { get; set; }
        public string Perroomrev { get; set; }
        public int Sumroomnight { get; set; }
        public string Sumroomrev { get; set; }
        public string Sumroomavg { get; set; }
    }
    public class Setrevenue_folio
    {
        public string Item { get; set; }
        public string Revenue { get; set; }
        public string Service { get; set; }
        public string Vat { get; set; }
        public string Total { get; set; }

        public string SumRevenue { get; set; }
        public string SumTotal { get; set; }
        public string SumVat { get; set; }
        public string SumService { get; set; }
    }
    public class SetRoomForecast
    {
        public string RoomDate { get; set; }
        public string Available { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string RoomUsed { get; set; }
        public string Occ { get; set; }
        public string Abf { get; set; }
        public string tRevenue { get; set; }
        public string AvgRate { get; set; }
    }
    public class SetRoomTypeForecast
    {
        public string EachRoom { get; set; }
        public string BeachBung { get; set; }
        public string GardenBung { get; set; }
        public string SupSea { get; set; }
        public string SupPool { get; set; }
        public string SupGarden { get; set; }
        public string SupBung { get; set; }
        public string OOO { get; set; }
    }
    public class Source
    {
        public string BusinessSource { get; set; }
        public string SourceID { get; set; }
    }

    public class NoDefine
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class WalkIn
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class Reservation
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class AgencyReservation
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class History
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class Government
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class WebBooking
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class Association
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class Complimentary
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class Corporation
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class DayUse
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class EMB
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class HouseUse
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class GuideTaxi
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
    public class VIP
    {
        public string month { get; set; }
        public string sum { get; set; }
    }
}