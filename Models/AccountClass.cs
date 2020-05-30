using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Demiady.Models;
namespace Demiady.Models
{
    public class AccountClass
    {
        public IEnumerable<Purchase> purchase { get; set; }
        public IEnumerable<Sale> sale { get; set; }
        public IEnumerable<Transfer> transfer { get; set; }
        public IEnumerable<Account> account { get; set; }
        public IEnumerable<Expens> expens { get; set; }
    }
}