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

    public partial class Expens
    {
        public int Ex_ID { get; set; }
        [Display(Name = "التاريخ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "من فضلك ادخل التاريخ")]
        public System.DateTime Date { get; set; }
        [Display(Name = "اسم المصروف")]
        [Required(ErrorMessage = "من فضلك ادخل اسم المصروف")]
        public string Name { get; set; }
        [Display(Name = "قيمة المصروف")]
        [Required(ErrorMessage = "من فضلك ادخل القيمة")]
        [Range(1, Int32.MaxValue, ErrorMessage = "القيمة يجب ان تكون اكبر من ال 0 ")]
        public int Value { get; set; }
    }
}
