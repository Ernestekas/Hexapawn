using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Hexapawn.Objects.Database
{
    class DB_Turn
    {
        public DB_Turn()
        {
            Variants = new List<DB_Variant>();
        }
        public int Turn { get; set; }
        public string TurnName
        {
            get
            {
                return "Turn" + Turn;
            }
        }
        public List<DB_Variant> Variants { get; set; }
        public Panel TurnContainer { get; set; }
        private bool Maximize { get; set; }
        public DataGridView SelectedVariant { get; set; }
        /// <summary>
        /// Creates Panel in Data table for specific turn.
        /// </summary>
        /// <param name="existingTurns"></param>
        public void DefinePanel(List<Panel> existingTurns)
        {
            Panel output = new Panel()
            {
                Name = TurnName,
                Size = new Size(600, 36),
                Location = new Point(3, 3),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button expRet = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                Image = Hexapawn.Properties.Resources.Expand,
                Size = new Size(30, 30),
                Location = new Point(3, 3)
            };

            Label turn = new Label()
            {
                AutoSize = false,
                Text = TurnName,
                Size = new Size(92, 30),
                Location = new Point(39, 3),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };

            if(existingTurns.Any())
            {
                output.Location = new Point(output.Location.X, existingTurns.Last().Location.Y + existingTurns.Last().Size.Height + 3);
            }
            Maximize = true;
            expRet.Click += (s, e) => ExpRetBtn_Click(s, e, existingTurns);
            output.Controls.Add(expRet);
            output.Controls.Add(turn);
            TurnContainer = output;
        }
        public void DefinePanel_1(Panel dataContainer)
        {
            List<Panel> existingTurns = dataContainer.Controls.OfType<Panel>().ToList();
            Panel output = new Panel()
            {
                Name = TurnName,
                Size = new Size(600, 36),
                Location = new Point(3, 3),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button expRet = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                Image = Hexapawn.Properties.Resources.Expand,
                Size = new Size(30, 30),
                Location = new Point(3, 3)
            };

            Label turn = new Label()
            {
                AutoSize = false,
                Text = TurnName,
                Size = new Size(92, 30),
                Location = new Point(39, 3),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };

            if (existingTurns.Any())
            {
                output.Location = new Point(output.Location.X, existingTurns.Last().Location.Y + existingTurns.Last().Size.Height + 3);
            }
            Maximize = true;
            expRet.Click += (s, e) => ExpRetBtn_Click_1(s, e, dataContainer);
            output.Controls.Add(expRet);
            output.Controls.Add(turn);
            TurnContainer = output;
        }
        /// <summary>
        /// Creates new variant for this turn and adds it to Variants.
        /// </summary>
        /// <param name="sets"></param>
        public void CreateNewVariant(Sets sets, List<Panel> allTurns)
        {
            DB_Variant newVariant = new DB_Variant();
            string newVariantID = "V" + Variants.Count.ToString();
            List<Pawn> gamePieces = sets.All();
            
            newVariant.DefineVariant(sets, newVariantID);

            // Add new variant.
            if(Variants.Any())
            {
                newVariant.Variant.Location = new Point(newVariant.Variant.Location.X, Variants.Last().Variant.Location.Y
                    + Variants.Last().Variant.Size.Height + 3);
            }
            Variants.Add(newVariant);
            TurnContainer.Controls.Add(newVariant.Variant);
            // Resize Turn container.
            if (!Maximize)
            {
                bool relocateNextTurn = false;
                TurnContainer.Size = new Size(TurnContainer.Width, Variants.Last().Variant.Location.Y
                + Variants.Last().Variant.Size.Height + 3);
                for (int i = 0; i < allTurns.Count; i++)
                {
                    if (relocateNextTurn)
                    {
                        allTurns[i].Location = new Point(allTurns[i].Location.X
                            , allTurns[i].Location.Y + allTurns[i].Size.Height + 3);
                    }
                    if (allTurns[i].Name == TurnName)
                    {
                        relocateNextTurn = true;
                    }
                }
            }
        }
        /// <summary>
        /// Compares each variant pawn possitions with possitions in data.
        /// </summary>
        /// <param name="setToComapreWith"></param>
        /// <returns>Checks if variant of this situation exists and return bool. If true, sets SelectedVariant DataGridView table.</returns>
        public bool CompareVariantWithTable(Sets setToComapreWith)
        {
            // Checks if any variant matches set positions. Returns true if matches.
            bool output = false;
            List<Pawn> gamePieces = setToComapreWith.All();
            for(int variantIndex = 0; variantIndex < Variants.Count; variantIndex++)
            {
                DB_Variant variant = Variants[variantIndex];
                List<bool> match = new List<bool>();
                for(int pawnIndex = 0; pawnIndex < gamePieces.Count; pawnIndex++)
                {
                    Pawn p = gamePieces[pawnIndex];
                    bool pawnExistsInTable = false;
                    for (int rowIndex = 0; rowIndex < variant.Variant.Rows.Count - 1; rowIndex++)
                    {
                        DataGridViewRow row = variant.Variant.Rows[rowIndex];
                        
                        if(p.Possition.Name == row.Cells[1].Value.ToString() && ("w" + p.Name == row.Cells[0].Value.ToString() 
                            || "b" + p.Name == row.Cells[0].Value.ToString()))
                        {
                            // This pawn matches. Move to next pawn.
                            match.Add(true);
                            rowIndex = variant.Variant.Rows.Count;
                            pawnExistsInTable = true;
                        }
                        else if(("w" + p.Name == row.Cells[0].Value.ToString() || "b" + p.Name == row.Cells[0].Value.ToString()) 
                            && p.Possition.Name != row.Cells[1].Value.ToString())
                        {
                            // There is no matched possitions for this pawn. 
                            // This variant doesn't match situation on table. Move to next variant.
                            match.Add(false);
                            pawnIndex = gamePieces.Count;
                            rowIndex = variant.Variant.Rows.Count;
                        }
                        else if(rowIndex == variant.Variant.Rows.Count - 2 && !pawnExistsInTable)
                        {
                            // This part is needed as there is one situation on table where program choses one variant where pawn and its possition doesn't
                            // match with table. Program doesn't find that pawn and choses wrong variant. So this part checks if checked pawn exist
                            // in a table and if it doesn't exist, thet it do the same thing as another else if.
                            match.Add(false);
                            pawnIndex = gamePieces.Count;
                            rowIndex = variant.Variant.Rows.Count;
                        }
                    }
                }
                if (!match.Contains(false))
                {
                    output = true;
                    SelectedVariant = variant.Variant;
                    variantIndex = Variants.Count;
                }
            }
            return output;
        }
        public void ExpRetBtn_Click(object sender, EventArgs e, List<Panel> ExistingTurns)
        {
            Button btn = (Button)sender;
            DataGridView last = new DataGridView();
            bool turnFound = false;

            if(Maximize)
            {
                int resizeBy = 0;
                int relocateBy = 0;
                if(Variants.Any())
                {
                    last = Variants.Last().Variant;
                    resizeBy = last.Location.Y + last.Size.Height + 3;
                }
                // Resize this turn.
                btn.Image = Hexapawn.Properties.Resources.Retract;
                Maximize = false;
                TurnContainer.Size = new Size(TurnContainer.Width, resizeBy);

                // Relocate other turns.
                for(int turnIndex = 0; turnIndex < ExistingTurns.Count; turnIndex++)
                {
                    if(!turnFound)
                    {
                        if(TurnName == ExistingTurns[turnIndex].Name)
                        {
                            turnFound = true;
                            relocateBy = ExistingTurns[turnIndex].Height;
                        }
                    }
                    else
                    {
                        ExistingTurns[turnIndex].Location = new Point(ExistingTurns[turnIndex].Location.X
                            , ExistingTurns[turnIndex].Location.Y + relocateBy);
                    }
                }
            }
            else
            {
                // Resize this turn.
                btn.Image = Hexapawn.Properties.Resources.Expand;
                Maximize = true;
                TurnContainer.Size = new Size(TurnContainer.Width, 36);
            }
        }
        public void ExpRetBtn_Click_1(object sender, EventArgs e, Panel dataContainer)
        {
            Button btn = (Button)sender;
            DataGridView last = new DataGridView();
            bool turnFound = false;
            List<Panel> existingTurns = dataContainer.Controls.OfType<Panel>().ToList();
            int relocateBy = 0;

            if (Maximize)
            {
                int resizeBy = 0;
                if (Variants.Any())
                {
                    last = Variants.Last().Variant;
                    resizeBy = last.Location.Y + last.Size.Height + 3;
                }
                // Resize this turn.
                btn.Image = Hexapawn.Properties.Resources.Retract;
                Maximize = false;
                TurnContainer.Size = new Size(TurnContainer.Width, resizeBy);

                // Relocate other turns.
                for (int turnIndex = 0; turnIndex < existingTurns.Count; turnIndex++)
                {
                    if (!turnFound)
                    {
                        if (TurnName == existingTurns[turnIndex].Name)
                        {
                            turnFound = true;
                            relocateBy = existingTurns[turnIndex].Height - 36;
                        }
                    }
                    else
                    {
                        existingTurns[turnIndex].Location = new Point(existingTurns[turnIndex].Location.X
                            , existingTurns[turnIndex].Location.Y + relocateBy);
                    }
                }
            }
            else
            {
                // Resize this turn.
                btn.Image = Hexapawn.Properties.Resources.Expand;
                Maximize = true;
                relocateBy = TurnContainer.Size.Height - 36;
                TurnContainer.Size = new Size(TurnContainer.Width, 36);
                // Relocate other turns.
                for(int turnIndex = 0; turnIndex < existingTurns.Count; turnIndex++)
                {
                    if(!turnFound)
                    {
                        if (TurnName == existingTurns[turnIndex].Name)
                        {
                            turnFound = true;
                        }
                    }
                    else
                    {
                        existingTurns[turnIndex].Location = new Point(existingTurns[turnIndex].Location.X
                            , existingTurns[turnIndex].Location.Y - relocateBy);
                    }
                }
            }
        }
    }
}
