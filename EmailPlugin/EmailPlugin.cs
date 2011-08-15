using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;
using InterComm.Helpers;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace EmailPlugin
{
    [StatelessChatPluginAttribute("use this command to email a file or send a message to someone")]
    public class EmailPlugin: IStatelessChatInterface
    {
        private enum EmailType
        {
            Message,
            File
        }

        private string lastError = "null";

        public string GetCommandResult(string cmdArgs)
        {
            string returnString = "null";
            string[] splitCommand = cmdArgs.Split(new char[]{' '});

            switch (splitCommand[0])
            {
                case "message":
                    {
                        if ( ValidateCommand(splitCommand) == false)
                        {
                            returnString = "Invalid command! see 'help' for usage";
                            break;
                        }

                        string message = string.Join(" ", splitCommand, 2, splitCommand.Length - 2);
                        if (SendMessage(splitCommand[1], message, EmailType.Message) == false)
                        {
                            returnString = "Error sending message to " + splitCommand[1];
                        }
                        else
                        {
                            returnString = "Message sent successfully!";
                        }
                        break;
                    }
                case "file":
                    {
                        if (ValidateCommand(splitCommand) == false)
                        {
                            returnString = "Invalid command! see 'help' for usage";
                            break;
                        }

                        string filePath = string.Join(" ", splitCommand, 2, splitCommand.Length - 2);
                        if (File.Exists(filePath) == false)
                        {
                            returnString = "Error! Invalid filename provided: " + filePath;
                            break;
                        }

                        if (SendMessage(splitCommand[1], filePath, EmailType.File) == false)
                        {
                            returnString = "Error sending file to " + splitCommand[1];
                        }
                        else
                        {
                            returnString = "File sent successfully!";
                        }
                        
                        break;
                    }
                case "help":
                default:
                    {
                        returnString = "Usage: " + GetCommandTrigger() + " message|file recepient message_string|file_path";
                        break;
                    }
            }
            return returnString;
        }
        public string GetCommandTrigger()
        {
            return "email";
        }

        //resuing a class here, is there a better way to do this? todo(4)
        private bool ValidateCommand(string[] args)
        {
            if (args.Length < 3)
            {
                return false;
            }

            EmailAddress recepient = new EmailAddress(args[1]);
            if (!recepient.Valid)
            {
                return false; 
            }
            return true;


        }

        private bool SendMessage(string to, string param, EmailType messageType)
        {
            bool result = false;

            string username = "null";
            string password = "null";
            try
            {

                foreach (string line in File.ReadAllLines("Plugins\\EmailPluginFiles\\credentials.ini"))
                {
                    if (line.StartsWith("username=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string[] values = line.Split(new char[] { '=' });
                        username = values[1];
                    }
                    if (line.StartsWith("password=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string[] values = line.Split(new char[] { '=' });
                        password = values[1];
                    }
                }

                NetworkCredential credentials = new NetworkCredential(username, password);
                MailMessage mailMessage = new MailMessage(username, to);

                if (messageType == EmailType.Message)
                {
                    mailMessage.Subject = "Message from " + username;
                    mailMessage.Body = param;
                }
                else if (messageType == EmailType.File)
                {
                    mailMessage.Subject = "File from " + username;
                    FileInfo fileInfo = new FileInfo(param);
                    mailMessage.Body = "File Name = " + fileInfo.FullName + "\n"
                                        + "File Size = " + (fileInfo.Length / 1024) + " KB\n";
                    
                    Attachment file = new Attachment(param);
                    mailMessage.Attachments.Add(file);
                }
                else
                {
                    throw new ArgumentException("invalid args");
                }

                SmtpClient mailClient = new SmtpClient("imap.gmail.com", 587);
                mailClient.UseDefaultCredentials = false;
                mailClient.EnableSsl = true;
                mailClient.Credentials = credentials;

                //this is a workaround for the certificate errors with gmail! dono why i dont have the real certs...
                ServicePointManager.ServerCertificateValidationCallback = 
                    new RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);

                mailClient.Send(mailMessage);
                result = true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                result = false;
            }

            return result;
        }

        /*
         * certificate errors! dammit!
         */
        public static bool RemoteServerCertificateValidationCallback
            (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;// sslPolicyErrors == SslPolicyErrors.None;
        }

    }
}
