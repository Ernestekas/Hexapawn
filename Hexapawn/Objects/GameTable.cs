using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hexapawn.Objects
{
    class GameTable
    {
        public List<Panel> ABC1 { get; set; }
        public List<Panel> ABC2 { get; set; }
        public List<Panel> ABC3 { get; set; }
        public List<Panel> A123 { get; set; }
        public List<Panel> B123 { get; set; }
        public List<Panel> C123 { get; set; }
        public List<char> AllColumns()
        {
            List<char> output = new List<char>();
                foreach(Panel c in ABC1)
                {
                    output.Add(c.Name[0]);
                }
                return output;
        }
        public int Turn { get; set; }
        public List<Panel> Table
        {
            get
            {
                List<Panel> temp = new List<Panel>();
                temp.AddRange(ABC1);
                temp.AddRange(ABC2);
                temp.AddRange(ABC3);
                return temp;
            }
        }

    }
}
