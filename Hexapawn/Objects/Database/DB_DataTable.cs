using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hexapawn.Objects.Database;
using System.Drawing;

namespace Hexapawn.Objects.Database
{
    class DB_DataTable
    {
        public DB_DataTable()
        {
            Turns = new List<DB_Turn>();
        }
        public Panel DataContainer { get; set; }
        public List<DB_Turn> Turns { get; set; }
        public int CurrentTurn { get; set; }
        public GameTable GameTable { get; set; }
        public string LastMoveID { get; set; }
        public string LastButOneMoveId { get; set; }
        public void CreateTurn(Sets sets)
        {
            List<Panel> allTurns = new List<Panel>();
            DB_Turn newTurn = new DB_Turn()
            {
                Turn = CurrentTurn
            };

            //newTurn.DefinePanel(DataContainer.Controls.OfType<Panel>().ToList());
            newTurn.DefinePanel_1(DataContainer);
            Turns.Add(newTurn);
            DataContainer.Controls.Add(newTurn.TurnContainer);

            // Create new variant for new turn.
            foreach(DB_Turn t in Turns)
            {
                allTurns.Add(t.TurnContainer);
            }
            newTurn.CreateNewVariant(sets, allTurns);
        }

    }
}
