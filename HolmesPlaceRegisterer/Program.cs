using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HolmesPlaceRegisterer
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.holmesplace.co.il/MobileWebSite/Pages/Spinning.aspx/RegisterToSpinningClass");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{'companyId':200, 'branchId':210, 'userId':10534167,'token':'72253fd0d48d4800a9372c09c6140113', 'lessonId':'10079', 'date': 1494979200, 'time':'203000', 'seatId':2}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

        }
    }
}
