namespace Mistaker
{
    class EoError
    {
        public string CorrelationId { get; set; }
        public string FileName { get; set; }
        public string Pcc { get; set; }
        public string CorporateId { get; set; }
        public string Flow { get; set; }
        public string Error { get; set; }
    }
    class EoAvailClosedError
    {
        public string CorrelationId { get; set; }
        public string FileName { get; set; }
        public string Pcc { get; set; }
        public string CorporateId { get; set; }
        public string Carrier { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FlightNumber { get; set; }
        public string Class { get; set; }
        public string Error { get; set; }
    }
}
