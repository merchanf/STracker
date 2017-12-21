using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Travelport.Adapter.Source.Uapi;

namespace Mistaker
{
    class Decoder
    {
        public static Transaction[] Decode(string[] emailBodies)
        {
            var trxList = new List<Transaction>();
            foreach (var body in emailBodies)
            {
                var b = body
                    .Replace("_______________________________________________________________________", "")
                    .Replace("*** This is an EXTERNAL email. Exercise caution. DO NOT open attachments or click links from unknown senders or in unexpected email.", "")
                    .Replace("This message has been analyzed for malware by Cloud Email Security.", "")
                    .Replace("To report this as spam please attach this email to an email addressed to asa@websense.com.", "")
                    .Trim();
                b = Decompress(b);
                var trx = Serializer.DeserializeObject(typeof(Transaction), b) as Transaction;
                trxList.Add(trx);
            }
            return trxList.ToArray();
        }

        public static string Compress(string text)
        {
            byte[] compressed;
            var buffer = Encoding.UTF8.GetBytes(text);
            using (var ms = new MemoryStream())
            {
                using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }
                ms.Position = 0;
                compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
            }

            var gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        private static string Decompress(string compressedText)
        {
            var gzBuffer = Convert.FromBase64String(compressedText);
            using (var ms = new MemoryStream())
            {
                var msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                var buffer = new byte[msgLength];

                ms.Position = 0;
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
