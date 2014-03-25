using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace IoTLib
{
    [DataContract]
    public class IoTMessage
    {
        [DataMember(Order=10)]
        public string Version { get; set; }

        [DataMember(Order = 20)]
        public string Variation { get; set; }

        [DataMember(Order = 30)]
        public Guid Transaction { get; set; }

        [DataMember(Order = 40)]
        public string Route { get; set; }

        [DataMember(Order = 50)]
        public string Via { get; set; }

        [DataMember(Order = 60)]
        public IoTSignal Signal { get; set; }

        [DataMember(Order = 70)]
        public IoTMessageData Data { get; set; }

        public IoTMessage()
        {
            this.Version = "0.3";
            this.Variation = "JSON";
            this.Transaction = Guid.NewGuid();
            this.Route = "Discover";
            this.Via = "192.168.1.11:5183";
            this.Signal = new IoTSignal();

            this.Data = new IoTMessageData();

        }

        public string ToJsonString()
        {
            //string json = new JavaScriptSerializer().Serialize(this);
            //return json;

            string json;

            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(this.GetType());

            ser.WriteObject(stream1, this);

            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            json = sr.ReadToEnd();

            return json;
        }
    }
}
