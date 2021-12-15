using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client_;
namespace Server
{
    public class Server
    {
        public int Client_ID;
        private string ipAddr;
        private int port;
        private IPEndPoint ipPoint;
        public Socket socket;
        public Socket socketclient;
        public List<Client> clients;
        public List<Task> tasks;
        public SqlConnection sqlConnection;
        public Server()
        {
            sqlConnection = new SqlConnection();
            this.Client_ID = -1;
            this.ipAddr = "127.0.0.1";
            this.port = 8000;
            this.ipPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.clients = new List<Client>();
            this.tasks = new List<Task>();

        }
        public void StartServer()
        {
            try
            {
                Connection_SQL($@"Data Source=SQL5105.site4now.net;Initial Catalog=db_a7d3c0_opendi;UID=db_a7d3c0_opendi_admin;Password=qwerty20039");
                this.socket.Bind(ipPoint);
                this.socket.Listen(10);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Start", ex.Message);

            }


        }
        public void Connects()
        {
          
            while (true)
            {
                this.socketclient = this.socket.Accept();
                clients.Add(new Client(socketclient));
                this.Client_ID++;
                clients[clients.Count - 1].ID = this.Client_ID;
              

                tasks.Add(new Task(() => GetInfo()));

                Console.WriteLine("New player has been conected");
                Console.WriteLine($"Clients - {clients.Count}\n{clients.Last().socket.RemoteEndPoint.ToString()}");
                tasks.Last().Start();
                Console.WriteLine("New task for new player started");
            }
        }
        public void GetInfo()
        {
            int user = 0;
            string json = String.Empty;
            user = clients.Count - 1;

            while (clients[user].socket.Connected)
            {
                try
                {
                    if (clients[user].socket.Connected)
                    {
                        json = GetInfo(user).ToString();
                      
                    }
                }
                catch (Exception)
                {
                }
            }
            Console.WriteLine($"User {clients[user].socket.RemoteEndPoint.ToString()} disconnect");
            clients.RemoveAt(user);
           

            if (user > clients.Count)
            {
                user--;
            }
        }

        public StringBuilder GetInfo(int user)
        {
            StringBuilder builder = new StringBuilder();
            HistoryItem item = new HistoryItem();
            int bytes = 0;
            byte[] data = new byte[1024];
            if (clients.Count != 0)
            {
                do
                {
                    try
                    {
                        if (clients[user].socket.Connected)
                        {
                            bytes = clients[user].socket.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }

                    }
                    catch (Exception)
                    {
                    }

                } while (clients[user].socket.Available > 0);

                if (builder.ToString() != "")
                {
                    try
                    {
                        item = JsonSerializer.Deserialize<HistoryItem>(builder.ToString());
                        using (SqlCommand command = new SqlCommand($@"INSERT INTO [UserHistory]VALUES('{item.UserIP}','{item.URL}','{item.Title}','{item.VisitedTime}')", sqlConnection))
                        {
                           command.ExecuteNonQuery();
                        }
                        Console.WriteLine("Added!");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return builder;
        }
        private void Connection_SQL(string connectionstr)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
            Console.WriteLine("Connected SQL");
        }
    }
}
