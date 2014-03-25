using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTLib
{
    [DataContract]
    public class IoTSignal
    {
        [DataMember(Order=10)]
        public string Name { get; set; }

        [DataMember(Order = 20)]
        public Guid Identity { get; set; }

        [DataMember(Order = 30)]
        public Uri Location { get; set; }

        [DataMember(Order = 40)]
        public List<IoTSignalElement> Elements { get; set; }

        public IoTSignal()
        {
            this.Name = "UnitTestLight";
            this.Identity = new Guid("93674756-D529-4A98-B1D3-9E0C07655248");
            this.Location = new Uri("coap://113.128.153.72"); //Uri("coap://localhostIPAddress");
            
            this.Elements = new List<IoTSignalElement>();
            this.Elements.Add(new IoTSignalElement());
        }
    }
}
