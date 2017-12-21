using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Mistaker
{
    class Config
    {
        private static Configuration Configuration { get; set; }

        public static string GetKey(string key)
        {
            if (Configuration == null)
            {
                Configuration = new Configuration
                {
                    AppSettings = new AppSettings
                    {
                        Add = new List<Add>()
                    }
                };
                Refresh();
            }
            return (from add in Configuration.AppSettings.Add where add.Key.Equals(key) select add.Value).FirstOrDefault();
        }

        public static void Refresh()
        {
            try
            {
                var configFileString = File.ReadAllText("app.config");
                Configuration = Serializer.DeserializeObject(typeof(Configuration), configFileString) as Configuration;
            }
            catch (Exception)
            {
                Console.WriteLine("\nError al leer el archivo de configuración");
                throw;
            }
        }

        //Email Settings
        public static readonly string Host = GetKey("EmailHost");
        public static readonly int Port = int.Parse(GetKey("EmailPort"));
        public static readonly bool Ssl = bool.Parse(GetKey("EmailSsl"));
        public static readonly string Username = GetKey("EmailUsername");
        public static readonly string Password = GetKey("EmailPassword");
        public static readonly int MaxHandledEmailsPerLoop = int.Parse(GetKey("MaxHandledEmailsPerLoop"));

        //Path settings
        public static readonly string BasePath = GetKey("BasePath");
        public static readonly string BookingsPath = BasePath + @"\Bookings";
        public static readonly string ErrorsPath = BasePath + @"\Errors";

        //Kibana
        public static readonly string DefaultIndex = GetKey("DefaultIndex");
        public static readonly string AvailClosedIndex = GetKey("AvailClosedIndex");
    }
    [XmlRoot(ElementName = "add")]
    public class Add
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "appSettings")]
    public class AppSettings
    {
        [XmlElement(ElementName = "add")]
        public List<Add> Add { get; set; }
    }

    [XmlRoot(ElementName = "configuration")]
    public class Configuration
    {
        [XmlElement(ElementName = "appSettings")]
        public AppSettings AppSettings { get; set; }
    }

}
