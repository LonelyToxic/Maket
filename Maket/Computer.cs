using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maket.Models
{
    public class Computer
    {
        public int ComputerID { get; set; }
        public int ComputerNumber { get; set; }
        public string Status { get; set; }
        public string Processor { get; set; }
        public string GraphicsCard { get; set; }
        public int RAM { get; set; }
        public string Motherboard { get; set; }
    }
}
