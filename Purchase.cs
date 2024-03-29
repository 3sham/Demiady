﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demiady
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Purchase
    {
        public int Pur_ID { get; set; }
        [Display(Name = "التاريخ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "من فضلك ادخل التاريخ")]
        public System.DateTime Date { get; set; }
        [Display(Name = "سعر الواحدة")]
        [Required(ErrorMessage = "من فضلك ادخل السعر")]
        [Range(1, Int32.MaxValue, ErrorMessage = "القيمة يجب ان تكون اكبر من ال 0 ")]
        public int Prod_Price { get; set; }
        [Display(Name = "العدد")]
        [Required(ErrorMessage = "من فضلك ادخل العدد")]
        [Range(1, Int32.MaxValue, ErrorMessage = "القيمة يجب ان تكون اكبر من ال 0 ")]
        public int Prod_count { get; set; }
        public Nullable<int> Sup_ID { get; set; }
        public Nullable<int> Prod_ID { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
