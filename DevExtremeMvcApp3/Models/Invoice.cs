//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DevExtremeMvcApp3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public System.DateTimeOffset InvoiceDate { get; set; }
        public System.DateTimeOffset InvoiceDueDate { get; set; }
        public string InvoiceName { get; set; }
        public int InvoiceTypeId { get; set; }
        public int ShipmentId { get; set; }
    }
}
