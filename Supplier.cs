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

    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Purchases = new HashSet<Purchase>();
            this.Transfers = new HashSet<Transfer>();
        }

        public int Sup_ID { get; set; }
        [Display(Name = "اسم المورد")]
        [Required(ErrorMessage = "من فضلك ادخل اسم المورد")]
        public string Sup_Name { get; set; }
        [Display(Name = "عنوانه")]
        public string Sup_Address { get; set; }
        [Display(Name = "رقم التليفون")]
        public string Sup_Phone { get; set; }
        [Display(Name = "حسابه")]
        [Required(ErrorMessage = "من فضلك ادخل قيمة الحساب")]
        [Range(0, Int32.MaxValue, ErrorMessage = "القيمة يجب ان تكون اكبر من ال 0 ")]
        public int Account { get; set; }
        [Display(Name = "واصل له ")]
        [Required(ErrorMessage = "من فضلك ادخل القيمة المدفوعة")]
        [Range(0, Int32.MaxValue, ErrorMessage = "القيمة يجب ان تكون اكبر من ال 0 ")]
        public int Wasel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase> Purchases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
