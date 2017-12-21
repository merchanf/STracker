using System.IO;
using Travelport.Adapter.Source.Uapi;

namespace Mistaker
{
    class Saver
    {
        public static void Save(Transaction[] transactions)
        {
            foreach (var transaction in transactions)
            {
                var fileName = transaction.CorrelationId;
                if (!string.IsNullOrEmpty(fileName))
                {
                    string folder;
                    if (!string.IsNullOrEmpty(transaction.RecordLocator) && !transaction.RecordLocator.Equals("ERROR!"))
                    {
                        folder = Config.BookingsPath;
                        fileName += $"[{transaction.RecordLocator}]";
                    }
                    else
                    {
                        folder = Config.ErrorsPath;
                    }
                    Directory.CreateDirectory(folder);
                    var path = Path.Combine(folder, fileName + ".tst");
                    File.WriteAllText(path, Decoder.Compress(Serializer.SerializeObject(transaction)));
                }
            }
        }
    }
}
