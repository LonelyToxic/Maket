using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maket.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string UserType { get; set; }
        public int? UserId { get; set; } // Изменено с ClientId на UserId
    }
}

