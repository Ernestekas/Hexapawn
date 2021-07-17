using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Hexapawn.Objects.Database
{
    class DB_Variant
    {
        public DataGridView Variant { get; set; }
        
        public void DefineVariant(Sets sets, string variantID)
        {
            List<string> columnHeaders = new List<string>() { "ID", "Position", "Move1", "Move2", "Move3", "Reward1", "Reward2", "Reward3" };
            List<Pawn> gamePieces = sets.All();
            DataGridView newVariant = new DataGridView()
            {
                Size = new Size(594, 180),
                Location = new Point(3, 39),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = false, // must be true, but for debug purposes it is set to false.
                Name = variantID
            };

            // Set columns headers.
            for(int i = 0; i < columnHeaders.Count; i++)
            {
                newVariant.Columns.Add(columnHeaders[i], columnHeaders[i]);
                // newVariant.Columns[i].HeaderText = columnHeaders[i];
            }

            // Add data of each pawn.
            foreach(Pawn p in gamePieces)
            {
                p.MovesRewards.Clear(); // Testing. Sometimes pawn already has rewards. So clear it. Question: where rewards comes from?
                int r = newVariant.Rows.Add();
                newVariant.Rows[r].Cells[0].Value = p.Color[0].ToString() + p.Name;
                newVariant.Rows[r].Cells[1].Value = p.Possition.Name;
                if(p.Color == "black")
                {
                    for (int cell = 2; cell < p.PossibleMoves.Count + 2; cell++)
                    {
                        p.MovesRewards.Add(1);
                        newVariant.Rows[r].Cells[cell].Value = p.PossibleMoves[cell - 2].Name;
                        newVariant.Rows[r].Cells[cell + 3].Value = p.MovesRewards[cell - 2];
                    }
                }
            }
            Variant = newVariant;
        }
    }
}
