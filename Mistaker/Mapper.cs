using System.Collections.Generic;
using System.IO;
using System.Linq;
using Travelport.Adapter.Source.Uapi;

namespace Mistaker
{
    class Mapper
    {
        private static string GeneralServiceAirError = "General air service Error.";

        public static EoError[] Map(Transaction[] transactions, out EoAvailClosedError[] availClosedErrors)
        {
            var errorsList = new List<EoError>();
            var availClosedErrorList = new List<EoAvailClosedError>();
            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction?.Error?.Message))
                {
                    if (transaction.Error.Message.Equals(GeneralServiceAirError))
                        availClosedErrorList.AddRange(MapAvailClosedErrors(transaction));

                    errorsList.Add(new EoError
                    {
                        Error = transaction.Error.Message,
                        CorporateId = transaction.CorporateId,
                        CorrelationId = transaction.CorrelationId,
                        Flow = transaction.Error.Flow.ToString(),
                        Pcc = transaction.Pcc,
                        FileName = Path.Combine(Config.ErrorsPath, transaction.CorrelationId + ".xml")
                    });
                }
            }
            availClosedErrors = availClosedErrorList.ToArray();
            return errorsList.ToArray();
        }

        public static EoAvailClosedError[] MapAvailClosedErrors(Transaction transaction)
        {
            var errorsList = new List<EoAvailClosedError>();
            var availabilityErrorInfo = Serializer.DeserializeObject(typeof(AvailabilityErrorInfo), transaction.Error.Detail) as AvailabilityErrorInfo;
            if (availabilityErrorInfo != null)
                errorsList.AddRange(availabilityErrorInfo.AirSegmentError.Select(segmentError => new EoAvailClosedError
                {
                    Error = segmentError.ErrorMessage,
                    CorporateId = transaction.CorporateId,
                    CorrelationId = transaction.CorrelationId,
                    Pcc = transaction.Pcc,
                    FileName = Path.Combine(Config.ErrorsPath, transaction.CorrelationId + ".xml"),
                    Carrier = segmentError.AirSegment.Carrier,
                    Class = segmentError.AirSegment.ClassOfService,
                    Destination = segmentError.AirSegment.Destination,
                    FlightNumber = segmentError.AirSegment.FlightNumber,
                    Origin = segmentError.AirSegment.Origin
                }));
            return errorsList.ToArray();
        }
    }   
}
