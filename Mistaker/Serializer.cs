using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Mistaker
{
    public static class Serializer
    {
        #region Class private data

        public static Hashtable Serializers;

        //***************************************
        /// <summary>
        /// SOAP url
        /// </summary>
        private const string UrlSoap = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// SOAP namespace
        /// </summary>
        private static readonly string NsSoap = $"xmlns:soap=\"{UrlSoap}\"";

        /// <summary>
        /// XSI namespace
        /// </summary>
        private const string NsXsi = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";

        /// <summary>
        /// XSD namespace
        /// </summary>
        private const string NsXsd = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";

        #endregion

        #region Serialize an object

        //***************************************
        /// <summary>
        /// Serialize an object into XML
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="asSoap">If should wrap in a soap wrapper</param>
        /// <returns>Returns the serialized object as an XML string</returns>
        /// 
        public static string SerializeObject(object obj, bool asSoap = false)
        {
            if (Serializers == null)
                Serializers = new Hashtable();

            string str = null;
            if (obj != null)
            {
                // Get the object type
                var t = obj.GetType();

                // Get the default namespace from the attributes
                var ns = GetDefaultNamespace(t);

                var key = !string.IsNullOrEmpty(ns)
                    ? t.FullName + ns
                    : t.FullName;

                var ser = (XmlSerializer)Serializers[key];
                if (ser == null)
                {
                    ser = string.IsNullOrWhiteSpace(ns)
                    ? new XmlSerializer(t)
                    : new XmlSerializer(t, ns);
                    // Cache the serializer.
                    Serializers[key] = ser;
                }

                // Serialize the object
                using (var sw = new StringWriter())
                {
                    ser.Serialize(sw, obj);
                    sw.Flush();
                    str = sw.ToString();
                }

                // Strip the serializer namespaces
                str = XDocument.Parse(str).ToString().Replace(NsXsd, string.Empty).Replace(NsXsi, string.Empty);

                if (asSoap)
                {
                    // Wrap in a soap wrapper
                    str = XDocument.Parse($"<soap:Envelope {NsSoap}><soap:Body>{str}</soap:Body></soap:Envelope>").ToString();
                }
            }
            return str;
        }

        #endregion

        #region Deserialize an object

        //***************************************
        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <param name="t">The type of object to return</param>
        /// <param name="xml">The xml data to deserialize</param>
        /// <param name="error">Returns the soap error if any, else null</param>
        /// <param name="asSoap">If the xml has a soap wrapper</param>
        /// <returns>Returns the object if successful, else null</returns>
        /// 
        public static object DeserializeObject(Type t, string xml, bool asSoap = false)
        {
            if (Serializers == null)
                Serializers = new Hashtable();

            if (!string.IsNullOrWhiteSpace(xml))
            {
                // Get the default namespace from the attributes
                var ns = GetDefaultNamespace(t);

                // If need to remove the soap wrapper
                if (asSoap)
                {
                    var doc = XDocument.Parse(xml);
                    XNamespace soap = UrlSoap;
                    var envelope = doc.Element(soap + "Envelope");
                    var body = envelope?.Element(soap + "Body");
                    if (body != null)
                    {
                        xml = body.FirstNode.ToString();
                    }
                }

                var key = !string.IsNullOrEmpty(ns)
                    ? t.FullName + ns
                    : t.FullName;

                var ser = (XmlSerializer)Serializers[key];
                if (ser == null)
                {
                    ser = string.IsNullOrWhiteSpace(ns)
                    ? new XmlSerializer(t)
                    : new XmlSerializer(t, ns);
                    // Cache the serializer.
                    Serializers[key] = ser;
                }

                // Deserialize the object
                object obj;
                using (var sr = new StringReader(xml))
                {
                    obj = ser.Deserialize(sr);
                }
                return obj;
            }
            return null;
        }

        #endregion

        #region Check for soap fault

        //***************************************
        /// <summary>
        /// Check if we have a soap fault response
        /// </summary>
        /// <param name="xml">The xml to parse</param>
        /// <returns>Returns the error string if any, else null if ok</returns>
        /// 
        public static string IsSoapFault(string xml)
        {
            string error = null;
            var doc = XDocument.Parse(xml);
            XNamespace soap = UrlSoap;
            var envelope = doc.Element(soap + "Envelope");
            var body = envelope?.Element(soap + "Body");
            var fault = body?.Element(soap + "Fault");
            if (fault != null)
            {
                var faultcode = fault.Element("faultcode");
                var faultstring = fault.Element("faultstring");
                error = $"Code: {faultcode?.Value ?? string.Empty}, Message: {faultstring?.Value ?? string.Empty}";
            }
            return error;
        }

        #endregion

        #region Get object namespace from attributes

        //***************************************
        /// <summary>
        /// Parse an objects attributes to find the default xml namespace
        /// </summary>
        /// <param name="t">The type to parse</param>
        /// <returns>Returns the default namespace if found, else null</returns>
        /// 
        private static string GetDefaultNamespace(MemberInfo t)
        {
            var attributes = t.GetCustomAttributesData();
            if (attributes != null)
            {
                // Loop through the custom attributes
                return (
                    from attribute in attributes
                    where attribute.Constructor?.DeclaringType != null
                    where attribute.Constructor.DeclaringType.Name.Equals("xmltypeattribute", StringComparison.OrdinalIgnoreCase)
                    select attribute.NamedArguments into args
                    where args != null
                    from arg in args
                    where arg.MemberInfo.Name.Equals("namespace", StringComparison.OrdinalIgnoreCase)
                    select arg.TypedValue.Value as string).FirstOrDefault();
            }
            return null;
        }

        #endregion
    }
}
