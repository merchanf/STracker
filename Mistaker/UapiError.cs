using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mistaker
{
    [XmlRoot(ElementName = "AirSegment", Namespace = "http://www.travelport.com/schema/air_v39_0")]
    public class AirSegment
    {
        [XmlAttribute(AttributeName = "Key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "Group")]
        public string Group { get; set; }
        [XmlAttribute(AttributeName = "Carrier")]
        public string Carrier { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "Origin")]
        public string Origin { get; set; }
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; }
        [XmlAttribute(AttributeName = "DepartureTime")]
        public string DepartureTime { get; set; }
        [XmlAttribute(AttributeName = "ArrivalTime")]
        public string ArrivalTime { get; set; }
        [XmlAttribute(AttributeName = "ClassOfService")]
        public string ClassOfService { get; set; }
        [XmlAttribute(AttributeName = "ChangeOfPlane")]
        public string ChangeOfPlane { get; set; }
        [XmlAttribute(AttributeName = "OptionalServicesIndicator")]
        public string OptionalServicesIndicator { get; set; }
    }

    [XmlRoot(ElementName = "AirSegmentError", Namespace = "http://www.travelport.com/schema/air_v39_0")]
    public class AirSegmentError
    {
        [XmlElement(ElementName = "AirSegment", Namespace = "http://www.travelport.com/schema/air_v39_0")]
        public AirSegment AirSegment { get; set; }
        [XmlElement(ElementName = "ErrorMessage", Namespace = "http://www.travelport.com/schema/air_v39_0")]
        public string ErrorMessage { get; set; }
    }

    [XmlRoot(ElementName = "AvailabilityErrorInfo", Namespace = "http://www.travelport.com/schema/air_v39_0")]
    public class AvailabilityErrorInfo
    {
        [XmlElement(ElementName = "Code", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Service", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string Service { get; set; }
        [XmlElement(ElementName = "Type", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string Type { get; set; }
        [XmlElement(ElementName = "Description", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string Description { get; set; }
        [XmlElement(ElementName = "TransactionId", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string TransactionId { get; set; }
        [XmlElement(ElementName = "TraceId", Namespace = "http://www.travelport.com/schema/common_v39_0")]
        public string TraceId { get; set; }
        [XmlElement(ElementName = "AirSegmentError", Namespace = "http://www.travelport.com/schema/air_v39_0")]
        public List<AirSegmentError> AirSegmentError { get; set; }
        [XmlAttribute(AttributeName = "air", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Air { get; set; }
        [XmlAttribute(AttributeName = "common_v39_0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Common_v39_0 { get; set; }
    }
}
