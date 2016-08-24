using System.Xml.Serialization;

namespace Ada.Framework.Development.Log4Me.Writers.RollingFileWrite
{
    public class RollingTag
    {
        [XmlAttribute]
        public string By { get; set; }

        [XmlAttribute]
        public double Value { get; set; }

        [XmlAttribute]
        public string Unit { get; set; }
    }
}
