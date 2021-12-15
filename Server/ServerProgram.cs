using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
            Console.WriteLine("Server started");
            Task.Factory.StartNew(() => server.Connects());
            while (true)
            {

            }
        }
    }
}
