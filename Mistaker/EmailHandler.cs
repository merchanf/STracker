using System.Collections.Generic;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace Mistaker
{
    class EmailHandler
    {
        private static readonly string Host = Config.Host;
        private static readonly int Port = Config.Port;
        private static readonly bool Ssl = Config.Ssl;
        private static readonly string Username = Config.Username;
        private static readonly string Password = Config.Password;
        private static readonly int MaxHandledEmailsPerLoop = Config.MaxHandledEmailsPerLoop;

        /// <summary>
        /// Reads the email inbox, then adds every message to a list to finally
        /// delete the message and send the email.
        /// </summary>
        /// <returns>string array with all the email bodies</returns>
        public static string[] CheckEmail(out int receivedEmails)
        {
            var messageList = new List<string>();
            using (ImapClient client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Host, Port, Ssl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(Username, Password);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                for (var i = 0; i < inbox.Count && i < MaxHandledEmailsPerLoop; i++)
                {
                    var message = inbox.GetMessage(i);
                    messageList.Add(message.TextBody);
                    var uids = inbox.Search(SearchQuery.HeaderContains("Message-Id", message.MessageId));
                    inbox.AddFlags(uids, MessageFlags.Deleted, true);
                    inbox.Expunge();
                }
            }

            receivedEmails = messageList.Count;
            return messageList.ToArray();
        }
    }
}
