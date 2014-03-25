using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLib
{
    public class IoTSignalElement
    {
        public string Kind { get; set; }
        public string Name { get; set; }

        public IoTSignalElement()
        {
            Kind = "Signal";
            this.Name = "MyUnitTestLight";
        }
    }
}
