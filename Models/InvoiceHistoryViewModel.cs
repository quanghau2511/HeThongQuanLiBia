using System;
using System.Collections.Generic;

namespace HeThongQuanLiBia.Models
{
    public class InvoiceHistoryViewModel
    {
        public DateTime? SearchDate { get; set; }
        public string? TableName { get; set; }
        public int? InvoiceId { get; set; }
        public List<HoaDon> Invoices { get; set; } = new List<HoaDon>();
    }
}
