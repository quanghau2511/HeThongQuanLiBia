using System.Collections.Generic;

namespace HeThongQuanLiBia.Models
{
    public class DashboardViewModel
    {
        public decimal TotalRevenueToday { get; set; }
        public int ActiveTables { get; set; }
        public int TotalInvoicesToday { get; set; }
        public int TotalCustomersToday { get; set; }
        public List<ChartPoint> RevenueChart { get; set; } = new List<ChartPoint>();
    }

    public class ChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
