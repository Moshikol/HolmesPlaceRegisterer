﻿using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace HolmesPlaceRegisterer
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Vars
            string globres = "", Token = "";
            string usrID = "10534167";
            string LoginReq = "";
            Exception EX2 = new Exception();
            Stream res;
            #endregion

            try
            {

                #region Login
                //Login To The System
                using (var streamReader = new StreamReader(@"D:\Docs\ReqText.txt"))
                {
                    LoginReq = streamReader.ReadToEnd();
                    res = SendWebReq("POST", LoginReq, "text/xml;charset=utf-8", "http://api.holmesplace.co.il/WebServices/LoginService.asmx", true);
                    Token = GetTokenFromReq(res);
                }
                #endregion
                #region Register
                //Register To The lesson
                // 10078 = sigal 10079 = dudi
                //  string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10072', 'date': {1}, 'time':'194500', 'seatId':22}}", usrID, d, Token).ToString();
                //  dudi Class string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10079', 'date': {1}, 'time':'203000', 'seatId':19}}", usrID, d, Token).ToString();

                double d = ConvertToUnixTimestamp(DateTime.Now);
                string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10078', 'date': {1}, 'time':'191500', 'seatId':22}}", usrID, d, Token).ToString();
               //Dudi string json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10079', 'date': {1}, 'time':'203000', 'seatId':22}}", usrID, d, Token).ToString();
                int[] arrSeatNum = { 10, 22, 21, 20, 19, 24, 25, 26, 27, 28, 29, 30 };
                int i = 1, ErrorCount = 0;
                bool StopTrying = false;
                #region LogDateGetting
                string LogDate = DateTime.Now.ToLongTimeString();
                LogDate = LogDate.Replace(':', '_');
                LogDate = LogDate + "_" + DateTime.Now.ToShortDateString();
                LogDate = LogDate.Replace('/', '-');
                #endregion

                using (StreamWriter LogWriter = new StreamWriter(@"d:\Logs\" + LogDate + ".txt"))
                {
                    while ((!(globres.Contains("קיים רישום כבר לשיעור הזה"))) && (!(StopTrying))) //While you did not catch a seat 
                    {

                        if (globres.Contains("true"))
                        {// it means that Succsefully Registerd
                            string seat = arrSeatNum[i].ToString();
                            LogWriter.WriteLine("Date: " + DateTime.Now.ToLongTimeString() + "|| " + "the Respond is " + globres + "||" + " You Succsefully Registerd To Seat Number:  " + seat);
                            EmailSend(usrID, globres, " You Succsefully Registerd To Seat Number: " + seat+"  ", EX2);
                            StopTrying = true;
                        }
                        if (globres.Contains("מצטערים, ברגע זה נתפס המושב על ידי חבר מועדון אחר"))
                        {// if this number of seat is taken move to another
                            if (i <= arrSeatNum.Length - 1)
                            {
                                LogWriter.WriteLine("||" + "This Seat is Taken: " + arrSeatNum[i].ToString() + "  Moving To Next Seat:  " + arrSeatNum[i + 1].ToString());
                                i++;

                            }

                            else
                                StopTrying = true;


                          // Dudi json = String.Format("{ 'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10079', 'date': { 1}, 'time':'203000', 'seatId':{3}}", usrID, d, Token, arrSeatNum[i].ToString()).ToString();
                               json = String.Format("{{'companyId':200, 'branchId':210, 'userId':{0},'token':'{2}', 'lessonId':'10078', 'date': {1}, 'time':'191500', 'seatId':{3}}", usrID, d, Token, arrSeatNum[i].ToString()).ToString();
                        }

                        if (globres.Contains("ארעה שגיאה"))// safety Check
                        {
                            ErrorCount++;
                            Thread.Sleep(3000);
                            if (ErrorCount > 10)
                                StopTrying = true;
                        }

                        Thread.Sleep(200);
                        res = SendWebReq("POST", json, "application/json", "http://api.holmesplace.co.il/MobileWebSite/Pages/Spinning.aspx/RegisterToSpinningClass", false);
                        #region GetResponseString
                        using (StreamReader streamReader = new StreamReader(res))
                        {
                            string result = streamReader.ReadToEnd();
                            globres = result.ToString();
                            LogWriter.WriteLine("Date: " + DateTime.Now.ToLongTimeString() + "|| " + "the Respond is " + globres);
                            LogWriter.WriteLine();

                        }
                        #endregion
                    }
                }

                Process.Start(@"Notepad.exe", @"d:\Logs\" + LogDate + ".txt");
                #endregion

                Debug.Print(globres);
                Console.WriteLine(res.ToString());
            }
            catch (Exception EX)
            {
                EmailSend(usrID, globres, "There Was An Exception :" + EX.Message.ToString(), EX);
            }

        }

        public static Stream SendWebReq(string Method, string ReqContent, string contentType, string url, bool Islogin)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = Method;
            httpWebRequest.Host = "api.holmesplace.co.il";
            httpWebRequest.ContentLength = ReqContent.Length;

            if (Islogin)
            {// if login all of this headers need to be added for a valid Request
                httpWebRequest.UserAgent = "ksoap2-android/2.6.0+";
                httpWebRequest.Headers.Add("USER_NAME", "sysuser!@#$");
                httpWebRequest.Headers.Add("PASSWORD", "sysPassword!@#$");
                httpWebRequest.Headers.Add("SYSTEM", "MOBILE");
                httpWebRequest.ContentLength = ReqContent.Length;
            }

            try
            {

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {


                    streamWriter.Write(ReqContent);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                Stream result = httpResponse.GetResponseStream();

                //   EmailSend("", result.ToString(), "You Just Registerd Successfully", null);
                // Console.WriteLine("The Responed of the Server is : " + result.ToString());
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // return ex.Message;
                return null;
            }



        }
        public static string GetTokenFromReq(Stream Req)
        {
            string Token = "";
            XmlDocument xd = new XmlDocument();
            xd.Load(Req);
            XmlNodeList RelNodes = xd.GetElementsByTagName("Token");
            Token = RelNodes.Item(0).InnerText;




            return Token;
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


            mail.Subject = Subj.ToString() + "The Respond of the Server:  " + res;
            mail.SubjectEncoding = Encoding.UTF8;

            mail.Body = " the users that was registered: " + users;
            mail.Body += Environment.NewLine + " The Respond of the Server: " + Environment.NewLine + res + Environment.NewLine;
            mail.Body += " StackTrace: " + Environment.NewLine + ex.StackTrace + Environment.NewLine;
            mail.Body += " Data: " + Environment.NewLine + ex.Data;
            mail.Body += " InnerException: " + Environment.NewLine + ex.InnerException;
            mail.BodyEncoding = Encoding.UTF8;

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
