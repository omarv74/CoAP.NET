using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoTLib
{
    [DataContract]
    public class IoTMessageData
    {
        [DataMember(Name="p-characteristics")]
        public List<string> PCharacteristics { get; set; }

        public IoTMessageData()
        {
            this.PCharacteristics = new List<string>() { "/device/light" };
        }

    }
}
