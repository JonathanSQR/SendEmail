using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace SendEmail
{
    class Program
    {
        static string Host;
        static string BodyFilePath;
        static string Body;
        static string Subject;
        static string Recipient;
        static string Sender;
        static string AttachmentPath;
        static bool UseSSL = true;
        static bool DebugInfo = false;

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No Arguements supplied. Type /help for more info.");
                Console.ReadLine();
                Environment.Exit(10);
            }
            else
            {
                for(int i = 0; i<args.Length; i++)
                {
                    //Console.WriteLine("|" + args[i] + "|");
                    try
                    {
                        switch (args[i].ToLower())
                        {
                            //Help Info
                            case "/help":
                                Console.WriteLine("  -h     : Host/Server name or IP.");
                                Console.WriteLine("  -s     : Subject.");
                                Console.WriteLine("  -b     : File path to htm/html file containing Email body.");
                                Console.WriteLine("  -t     : To Email address. (Recipient)");
                                Console.WriteLine("  -f     : From Email address. (Sender)");
                                Console.WriteLine("  -a     : File path to additinal attachment.");
                                Console.WriteLine("  -ssl   : Use SSL encryption.");
                                Console.WriteLine("  -d     : Show Debug Messages.");
                                Environment.Exit(0);
                                break;
                            
                            //Process Normal Parameters
                            case "-h": Host = args[i + 1]; break;
                            case "-s": Subject = args[i + 1]; break;
                            case "-b": BodyFilePath = args[i + 1]; break;
                            case "-t": Recipient = args[i + 1]; break;
                            case "-f": Sender = args[i + 1]; break;
                            case "-a": AttachmentPath = args[i + 1]; break;
                            case "-ssl": UseSSL = true; break;
                            case "-d": DebugInfo = true; break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Incorrect Parameter Specification. Type /help for more info.");
                        if (DebugInfo) Console.ReadLine();
                        Environment.Exit(10);
                    }
                }
            }
            //Display basic parameters(Debug)
            if (DebugInfo)
            {
                Console.WriteLine("Startup Settings used:");
                Console.WriteLine("Host = " + Host);
                Console.WriteLine("Subject = " + Subject);
                Console.WriteLine("Body File Path = " + (BodyFilePath==null?"None": BodyFilePath));
                Console.WriteLine("Recipient = " + Recipient);
                Console.WriteLine("Sender = " + Sender);
                Console.WriteLine("Attachment Path = " + (AttachmentPath == null ? "None" : AttachmentPath));
                Console.WriteLine("Use SSL = " + (UseSSL ? "True" : "False"));
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
            }

            SendEmail();

            if (DebugInfo) Console.ReadLine();
            Environment.Exit(0);
        }

        private static void ReadFileForBody()
        {
            Body = File.ReadAllText(BodyFilePath);
        }

        private static void SendEmail()
        {
            //Setup client
            SmtpClient client = new SmtpClient(Host);
            client.UseDefaultCredentials = true;

            //Enable SSL if required
            if (UseSSL)
            {
                client.EnableSsl = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            try
            {
                if (BodyFilePath != null)
                { 
                        Body = File.ReadAllText(BodyFilePath);
                    if (Body == null || Body == "") throw new FileLoadException();
                }
                if (DebugInfo) Console.WriteLine("Body File loaded." + "\n" + Body.Substring(0,10));

                MailMessage message = new MailMessage(Sender, Recipient, Subject, Body);
                message.IsBodyHtml = true;

                if (DebugInfo) Console.WriteLine("Sending Email...");
                client.Send(message);

                if (DebugInfo) Console.WriteLine("Sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
                if (DebugInfo) Console.ReadLine();
                Environment.Exit(10);
            }
        }
    }
}
