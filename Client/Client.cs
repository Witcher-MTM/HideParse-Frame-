using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client_
{
    public class Client
    {
        public int ID;
        public string ipAddr;
        public int port;
        public IPEndPoint iPEndPoint;
        public Socket socket;
        private string Ip;
        public bool isConnect { get; set; }
        public List<string> PathesGoogle { get; set; }
        public List<string> PathesOpera { get; set; }
        public bool FileFind { get; set; }
        public Client()
        {
            PathesGoogle = new List<string>();
            PathesOpera = new List<string>();
            isConnect = false;
            this.ID++;
            this.ipAddr = "127.0.0.1";
            this.port = 8000;
            this.iPEndPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            FileFind = false;
        }
        public Client(Socket socket)
        {
            this.socket = socket;
        }
        public void Connect()
        {
            socket.Connect(iPEndPoint);
            this.Ip = socket.RemoteEndPoint.ToString();
            PathesGoogle.Add(@"\Google\Chrome\User Data\System Profile\History");
            PathesGoogle.Add(@"\Google\Chrome\User Data\Default\History");
            PathesOpera.Add(@"C:\Users\student\AppData\Roaming\Opera Software\Opera Stable\History");
        }

        
        public StringBuilder GetInfo()
        {
            byte[] data = new byte[1024];
            int bytes = 0;
            string json = String.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                do
                {
                    bytes = socket.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                } while (socket.Available > 0);
            }
            catch (Exception)
            {

            }

            return stringBuilder;
        }
       
    }
}
