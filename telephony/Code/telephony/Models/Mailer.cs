using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Revoco.Models
{
    public class Mailer
    {

        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public Int32 SmtpPort { get; set; }
        public bool IsBodyHtml { get; set; }
        public string[] Attachments { get; set; }
        public bool IsSendMail { get; set; }
        public bool IsMailSent { get; set; }
        public string ErrorMessage { get; set; }
        public string[] CC { get; set; }
        public bool EnableSSL { get; set; }
        public Dictionary<string, string> NameValuePairAttachment { get; set; }

        /// <summary>
        /// Sends the register mail.
        /// </summary>
        /// <param name="invoicePDFFile">The invoice PDF file.</param>
        /// <param name="invoiceRequestFileDictionary">The invoice request file dictionary.</param>
        /// <returns></returns>
        public bool SendRegisterMail(string invoicePDFFile, Dictionary<string, string> invoiceRequestFileDictionary)
        {
           
            return SendMail();
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <returns></returns>
        public bool SendMail()
        {

            try
            {
                if (this.To == "admin@admin.com")
                    return true;

                MailMessage mailMessage = new MailMessage(this.From, this.To, this.Subject, this.Body);

                //From Email Address and From Display Name
                mailMessage.From = new MailAddress(this.From, this.FromName);

                mailMessage.IsBodyHtml = this.IsBodyHtml;

                // Add attachments
                if (this.Attachments != null)
                {
                    foreach (string file in Attachments)
                    {
                        if (File.Exists(file))
                        {
                            // Add the file attachment to this e-mail message.
                            mailMessage.Attachments.Add(new Attachment(file, MediaTypeNames.Application.Octet));
                        }
                    }
                }

                // Add Name Value Paid Attachment
                if (this.NameValuePairAttachment != null)
                {
                    foreach (KeyValuePair<string, string> keyValue in this.NameValuePairAttachment)
                    {
                        if (File.Exists(keyValue.Key))
                        {
                            // Add the file attachment to this e-mail message.
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(keyValue.Key);
                            attachment.Name = keyValue.Value;  // set name here
                            //attachment.ContentType = MediaTypeNames.Application.Octet;
                            mailMessage.Attachments.Add(attachment);
                        }
                    }
                }

                // Set SMTP client properties
                SmtpClient smtpClient = new SmtpClient(this.SmtpServer, this.SmtpPort);
                //   SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                if (this.SmtpUserName.Trim().Length > 0)
                {
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(this.SmtpUserName, this.SmtpPassword);
                    smtpClient.Credentials = credentials;

                    smtpClient.EnableSsl = true;

                }


                //Check IsSendMail flag true...
                this.IsSendMail = true;
                if (this.IsSendMail)
                {
                    // Send mail
                    smtpClient.Send(mailMessage);

                    // Set Staus
                    this.IsMailSent = true;
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;

                this.ErrorMessage = this.ErrorMessage + "**********To: " + this.To + "*********From: " + this.From;

                this.IsMailSent = false;
            }
            return this.IsMailSent;
        }

        /// <summary>
        /// Sends the forgot password link.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool SendForgotPasswordLink(string username, string id)
        {

           

            return SendMail();
        }



        /// <summary>
        /// Sends the register mail to user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public bool SendRegisterMailToUser(string username)
        {

          

            return SendMail();
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}