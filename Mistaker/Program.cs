using System;

namespace Mistaker
{
    public class Program
    {
        static void Main()
        {
            while (true)
            {
                try
                {
                    string[] encodedEmails = {};
                    int tempReceivedEmails;
                    while (encodedEmails.Length == 0)
                        encodedEmails = EmailHandler.CheckEmail(out tempReceivedEmails);

                    var transactions = Decoder.Decode(encodedEmails);

                    Saver.Save(transactions);

                    EoAvailClosedError[] eoAvailClosedErrors;
                    var eobjects = Mapper.Map(transactions, out eoAvailClosedErrors);
                    ElasticController.AddElasticEoErrors(eobjects);
                    if (eobjects.Length > 0)
                        Console.Write($"\nSuccesfully added {eobjects.Length} generalerror objects.");
                    ElasticController.AddElasticEoAvailClosedErrors(eoAvailClosedErrors);
                    if (eoAvailClosedErrors.Length > 0)
                        Console.Write($"\nSuccesfully added {eoAvailClosedErrors.Length} availclosed objects");
                    
                }
                catch (Exception exception)
                {
                    Console.WriteLine("\n-- Exception thrown -----------------------------------------------------------");
                    Console.WriteLine(exception);
                    Console.WriteLine("-------------------------------------------------------------------------------");
                }
            }
        }
    }
}
