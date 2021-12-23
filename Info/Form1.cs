using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Info
{
    public partial class Form1 : Form
    {
        static Server server = new Server();
        public Form1()
        {
            server.socket.Bind(server.ipPoint);
            server.socket.Listen(10);

            Task.Factory.StartNew(() => Connect());
            InitializeComponent();
        }
        public void Connect()
        {
            while (true)
            {
                server.socketclient = server.socket.Accept();
                server.ClientsSocket.Add(server.socketclient);
                Task.Factory.StartNew(() =>
                {
                    Invoke((MethodInvoker)(() => AddIPAddress()));
                });
            }
        }
        public void AddIPAddress()
        {
            var msg = server.GetMsg();
            List<string> ips = new List<string>();
            for (int i = 0; i < this.listBox1.Items.Count; i++)
            {
                ips.Add(this.listBox1.Items[i].ToString());
            }
            if (!ips.All(item => item.Contains(msg)) || ips.Count == 0)
            {
                this.listBox1.Invoke((MethodInvoker)(() => this.listBox1.Items.Add(msg)));
            }
            GetHistory();
        }
        public void GetHistory()
        {
            var msg = server.GetMsg();
            if (!Directory.Exists("Google"))
                Directory.CreateDirectory("Google");

            File.WriteAllText($@"Google\{this.listBox1.Items[this.listBox1.Items.Count - 1]}", msg);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox) != null)
            {
                if (File.Exists($@"Google\{(sender as ListBox)?.SelectedItem.ToString()}"))
                {
                    SQLiteDataAdapter adapter = JsonSerializer.Deserialize<SQLiteDataAdapter>(File.ReadAllText($@"Google\{(sender as ListBox)?.SelectedItem.ToString()}"));
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset);
                    dataGridView1.DataSource = dataset.Tables[0];
                }
            }
        }
    }
}
