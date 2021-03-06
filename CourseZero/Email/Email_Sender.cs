﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CourseZero.Email
{
    public class Email_Sender
    {
        static SmtpClient smtpclient;
        static string email_source = "";
        public Email_Sender()
        {
            try
            {
                string[] strs = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/email_settings.cfg");
                email_source = strs[2];
                smtpclient = new SmtpClient(strs[0], int.Parse(strs[1]));
                smtpclient.EnableSsl = true;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new System.Net.NetworkCredential(strs[2], strs[3]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when setting up smtp client!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
        public async static Task<bool> Send_Verification_Email(string target_email, string username, string verifying_hash_encoded)
        {
            string mail_title = "Verify your email for CourseZero";
            string mail_msg = "Hello "+ username +",\r\n\r\nFollow this link to verify your email address.\r\n\r\nhttps://localhost:5001/api/Register/Verify_Email/"+username +"/"+ verifying_hash_encoded + "\r\n\r\nIf you didn't ask to verify this address, you can ignore this email.\r\n\r\nThanks,\r\n\r\nCourseZero Team";
            return await Send_Mail(target_email, mail_title, mail_msg);
        }
        public async static Task<bool> Send_Password_Change_Email(string target_email, int userid, string username, string password, string verifying_hash_encoded)
        {
            string mail_title = "Your new password at CourseZero";
            string mail_msg = "Hello " + username + ",\r\n\r\nYou have requested to reset the password of your account.\r\n\r\nYour new password: "+password+"\r\n\r\nFollow this link to activate your new password.\r\n\r\nhttps://localhost:5001/api/ForgotPassword/Activate/" + userid + "/" + verifying_hash_encoded + "\r\n\r\nIf you didn't ask to reset your password, you can ignore this email.\r\n\r\nThanks,\r\n\r\nCourseZero Team";
            return await Send_Mail(target_email, mail_title, mail_msg);
        }
        private async static Task<bool> Send_Mail(string target_email, string mail_title, string mail_msg)
        {
            MailMessage message = new MailMessage(email_source, target_email);
            message.Subject = mail_title;
            message.Body = mail_msg;
            message.Priority = MailPriority.High;
            try
            {
                await smtpclient.SendMailAsync(message);
            }
            catch
            {
                return false;
            }
            message.Dispose();
            return true;
        }
    }
}
