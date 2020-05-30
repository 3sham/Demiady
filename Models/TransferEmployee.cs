using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demiady.Models
{
    public class TransferEmployee
    {
        public IEnumerable<Employee> employees { get; set; }
        public IEnumerable<TransferEmp> transferEmps { get; set; }
    }
}