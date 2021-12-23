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
namespace Info
{
    public class Server
    {
        public int Client_ID;
        private string ipAddr;
        private int port;
        public IPEndPoint ipPoint;
        public Socket socket;
        public Socket socketclient;
        public List<Socket> ClientsSocket;
        public List<Client> clients;
        public List<Task> tasks;
        public SqlConnection sqlConnection;
        public byte[] data;
        public Server()
        {
            ClientsSocket = new List<Socket>();
            data = new byte[256];
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
            }
        }
       

        public string GetMsg()
        {       
            int bytes = 0;
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                bytes = ClientsSocket.Last().Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socketclient.Available > 0);

            return stringBuilder.ToString();
        }
        private void Connection_SQL(string connectionstr)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
           
        }
    }
}
