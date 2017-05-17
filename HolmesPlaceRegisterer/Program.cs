using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HolmesPlaceRegisterer
{
    class Program
    {
        static void Main(string[] args)
        {
            string globres = "";
            string usrID = "10534167";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.holmesplace.co.il/MobileWebSite/Pages/Spinning.aspx/RegisterToSpinningClass");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    double d = ConvertToUnixTimestamp(DateTime.Now);

                    string json = string.Format("{'companyId':200, 'branchId':210, 'userId':{1},'token':'72253fd0d48d4800a9372c09c6140113', 'lessonId':'10079', 'date': {0}, 'time':'203000', 'seatId':2}", d, usrID);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    globres = result.ToString();
                    EmailSend(usrID, result.ToString(), "You Just Registerd Successfully");
                }
            }
            catch (Exception EX)
            {

                EmailSend(usrID, globres, "There Was An Exception :" + Environment.NewLine + EX.Message);
            }

        }
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);


        }
        public static void EmailSend(string users, string res, string Subj)
        {

            MailAddress to = new MailAddress("moshiko.lev@gmail.com");


            MailAddress from = new MailAddress("Registerer@Holmes.com");

            MailMessage mail = new MailMessage(from, to);


            mail.Subject = Subj;


            mail.Body = "the users that was registered" + users;
            mail.Body += Environment.NewLine + "The Respond of the Server:" + Environment.NewLine + res;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(
                "EmailSender1402@gmail.com", "Aa@123456");
            smtp.EnableSsl = true;
            Console.WriteLine("Sending email...");
            smtp.Send(mail);
        }
    }
}
