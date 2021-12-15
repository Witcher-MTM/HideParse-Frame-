using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_
{
    public class HistoryItem
    {
        public List<string> URL { get; set; }

        public List<string> Title { get; set; }

        public DateTime VisitedTime { get; set; }
        public string UserIP { get; set; }
        public HistoryItem()
        {
            URL = new List<string>();
            Title = new List<string>();
        }
    }
}
