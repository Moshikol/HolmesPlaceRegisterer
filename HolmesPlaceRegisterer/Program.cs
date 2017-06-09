using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace HolmesPlaceRegisterer
{
    class Program
    {
        static void Main(string[] args)
        {
            string globres = "";
            string usrID = "10534167";
            Exception EX2 = new Exception();
            try
            {
                // var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.holmesplace.co.il/MobileWebSite/Pages/Spinning.aspx/RegisterToSpinningClass");
                //httpWebRequest.ContentType = "application/json";
                //httpWebRequest.Method = "POST";

                //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                //{
                //    double d = ConvertToUnixTimestamp(DateTime.Now);
                //    // 10078 = sigal 10079 = dudi
                //    string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'72253fd0d48d4800a9372c09c6140113', 'lessonId':'10078', 'date': {1}, 'time':'191500', 'seatId':23}}", usrID,d ).ToString();

                //    streamWriter.Write(json);
                //    streamWriter.Flush();
                //    streamWriter.Close();
                //}

                //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();
                //    globres = result.ToString();
                //    EmailSend(usrID, result.ToString(), "You Just Registerd Successfully", EX2);
                //}
            }
            catch (Exception EX)
            {
                EmailSend(usrID, globres, "There Was An Exception :" + EX.Message, EX);
            }

        }

        public static string SendWebReq(string Method, string json, string contentType, string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = Method;
            try
            {

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    double d = ConvertToUnixTimestamp(DateTime.Now);
                    // 10078 = sigal 10079 = dudi
                    //  string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'72253fd0d48d4800a9372c09c6140113', 'lessonId':'10078', 'date': {1}, 'time':'191500', 'seatId':23}}", usrID, d).ToString();

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    EmailSend("", result.ToString(), "You Just Registerd Successfully", null);
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }



        }
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);


        }
        public static void EmailSend(string users, string res, string Subj, Exception ex)
        {

            MailAddress to = new MailAddress("moshiko.lev@gmail.com");


            MailAddress from = new MailAddress("Registerer@Holmes.com");

            MailMessage mail = new MailMessage(from, to);


            mail.Subject = Subj.ToString();


            mail.Body = "the users that was registered" + users;
            mail.Body += Environment.NewLine + "The Respond of the Server:" + Environment.NewLine + res;
            mail.Body += "StackTrace:" + Environment.NewLine + ex.StackTrace;
            mail.Body += "Data:" + Environment.NewLine + ex.Data;
            mail.Body += "InnerException:" + Environment.NewLine + ex.InnerException;

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
