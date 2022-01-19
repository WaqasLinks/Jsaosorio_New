using jsaosorio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace LeaveON.Models
{
  public static class SendEmail
  {


    /// <summary>
    /// Function will send email.
    /// </summary>
    /// <param name="senderEmail">sender email</param>
    /// <param name="senderPassword">sender password</param>
    /// <param name="receiver">receiver as LMS.Models.Employee Type is required</param>
    /// <param name="MessageType">"LeaveRequest" or "LeaveAccept"</param>
    /// <returns>return type is void</returns>
    /// <remarks>Text put here will not display in a Visual Studio summary box.  
    /// It is meant to add in further detail for anyone who might read this  
    /// code in the future </remarks>
    public static void SendEmailUsingSMTP(List<String> RecipientsEmails, String Template)
    {

      MailMessage mail = new MailMessage();

      SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
      //smtpServer.UseDefaultCredentials = false;

      smtpServer.Credentials = new System.Net.NetworkCredential(Constants.LeavON_Email, Constants.LeavON_Password);
      //smtpServer.Host = "smtp.gmail.com"; not neccesry now. as mention above
      smtpServer.Port = 587; // Gmail works on this port
      smtpServer.EnableSsl = true;

      try
      {

        mail.From = new MailAddress(Constants.LeavON_Email);
        //mail.From = new MailAddress(sender.Email);
        foreach (String email in RecipientsEmails)
        {
          mail.To.Add(new MailAddress(email));
        }

        mail.Subject = "New [Lead magnet type]) [Lead magnet name]";
        mail.Body = Template;
        mail.IsBodyHtml = true;
        smtpServer.Send(mail);
      }
      catch (Exception ex)
      {

        switch (ex.HResult)
        {
          case -2146233088://sender email is wrong
                           //return quitely                  
            break;
          default:
            //return quitely
            break;

        }
      }
    }


  }
}

