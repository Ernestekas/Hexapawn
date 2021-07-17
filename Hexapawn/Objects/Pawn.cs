using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Hexapawn.Objects
{
    class Pawn
    {
        public Pawn()
        {
            MovesRewards = new List<int>();
        }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool YourTurn { get; set; }
        public Panel Possition { get; set; }
        public PictureBox PawnPic { get; set; }
        public bool Selected { get; set; }
        public bool DeselectOthers { get; set; }
        public List<Pawn> WhiteSet { get; set; }
        public List<Pawn> BlackSet { get; set; }
        public GameTable Board { get; set; }
        public List<Panel> PossibleMoves
        {
            get
            {
                List<Panel> output = new List<Panel>();

                char possitionColumn = Possition.Name[0];
                int possitionRow = int.Parse(Possition.Name[1].ToString());
                List<char> allColumns = Board.AllColumns();

                // Check if this pawn can move forward.
                foreach (Panel c in Board.Table)
                {
                    if(c.Name == (possitionColumn.ToString() + (possitionRow + 1).ToString()) 
                        && c.Controls.Count < 1 && Color == "white")
                    {
                        // White pawn can move here. Add destination cell to output list.
                        output.Add(c);
                    }
                    else if(c.Name == (possitionColumn.ToString() + (possitionRow - 1).ToString())
                        && c.Controls.Count < 1 && Color == "black")
                    {
                        // Black pawn can move here. Add destination cell to output list.
                        output.Add(c);
                    }
                }

                // Check if this pawn can move diagonaly.
                for (int i = 0; i < allColumns.Count; i++)
                {
                    if(Color == "white")
                    {
                        // Check if can kill to the right and selected pawn isn't on the most right column.
                        if (possitionColumn == allColumns[i] && possitionColumn
                            != allColumns[allColumns.Count - 1])
                        {
                            foreach (Panel c in Board.Table)
                            {
                                // Check diagonal to the right cell.
                                if (c.Name == (allColumns[i + 1].ToString() + (possitionRow + 1).ToString()))
                                {
                                    // Check if this cell contains black pawn.
                                    foreach (Pawn p in BlackSet)
                                    {
                                        if (p.Possition.Name == c.Name)
                                        {
                                            // White pawn can move here. Add to output list.
                                            output.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                        // Check if pawn can kill to the left and selected pawn isn't on the most left column.
                        if(possitionColumn == allColumns[i] && possitionColumn
                            != allColumns[0])
                        {
                            foreach(Panel c in Board.Table)
                            {
                                // Check diagonal to the left cell.
                                if(c.Name == (allColumns[i - 1].ToString() + (possitionRow + 1).ToString()))
                                {
                                    // Check if this cell contains black pawn.
                                    foreach(Pawn p in BlackSet)
                                    {
                                        if(p.Possition.Name == c.Name)
                                        {
                                            // White pawn can move here. Add to output list.
                                            output.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if(Color == "black")
                    {
                        // Check if black pawn can kill to the right.
                        if(possitionColumn == allColumns[i] && possitionColumn != allColumns[allColumns.Count - 1])
                        {
                            foreach(Panel c in Board.Table)
                            {
                                // Check diagonal to the right cell.
                                if(c.Name == (allColumns[i + 1].ToString() + (possitionRow - 1).ToString()))
                                {
                                    // Check if this cell contains white pawn.
                                    foreach(Pawn p in WhiteSet)
                                    {
                                        if(p.Possition.Name == c.Name)
                                        {
                                            // Black pawn can move here. Add to output list.
                                            output.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                        // Check if black pawn can kill to the left.
                        if(possitionColumn == allColumns[i] && possitionColumn != allColumns[0])
                        {
                            foreach(Panel c in Board.Table)
                            {
                                // Check diagonal to the left cell.
                                if(c.Name == (allColumns[i - 1].ToString() + (possitionRow - 1).ToString()))
                                {
                                    // Check if this cell contains white Pawn.
                                    foreach(Pawn p in WhiteSet)
                                    {
                                        if(p.Possition.Name == c.Name)
                                        {
                                            // Black pawn can move here. Add t ooutput list.
                                            output.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                return output;
            }
        }
        public List<int> MovesRewards { get; set; }
        public List<int> MovesRewardsIntervals { get; set; }
        public void CreatePawn()
        {
            Image picDefault = Hexapawn.Properties.Resources.Pawn_White;
            Image picGreen = Hexapawn.Properties.Resources.Pawn_White_GBorder;
            Selected = false;

            YourTurn = true;

            // Set images for pawn.
            if (Color == "black")
            {
                picDefault = Hexapawn.Properties.Resources.Pawn_Black;
                picGreen = Hexapawn.Properties.Resources.Pawn_Black_GB;
                YourTurn = false;
            }

            PawnPic = new PictureBox()
            {
                Name = Name,
                Size = new Size(46, 46),
                Location = new Point(2, 2),
                Image = picDefault
            };
            PawnPic.MouseEnter += new EventHandler(Pawn_MouseEnter);
            PawnPic.MouseLeave += new EventHandler(Pawn_MouseLeave);
            PawnPic.MouseClick += new MouseEventHandler(Pawn_MouseClick);
            Possition.Controls.Add(PawnPic);
        }
        public void MovePawn(Panel moveTo)
        {
            moveTo.Controls.Add(PawnPic);
            if(Color == "white")
            {
                PawnPic.Image = Hexapawn.Properties.Resources.Pawn_White;
            }
            Possition = moveTo;
            Selected = false;
        }
        public List<Pawn> KillBlackPawn(Panel moveTo)
        {
            // Remove pawn from table.
            Pawn pawnToRemove = new Pawn();
            foreach(Pawn p in BlackSet)
            {
                if(p.Possition.Name == moveTo.Name)
                {
                    pawnToRemove = p;
                }
            }
            BlackSet.Remove(pawnToRemove);
            moveTo.Controls.Remove(PawnPic);
            return BlackSet;
        }
        public List<Pawn> KillWhitePawn(Panel moveTo)
        {
            Pawn pawnToRemove = new Pawn();
            foreach(Pawn p in WhiteSet)
            {
                if(p.Possition.Name == moveTo.Name)
                {
                    pawnToRemove = p;
                }
            }
            WhiteSet.Remove(pawnToRemove);
            moveTo.Controls.Remove(pawnToRemove.PawnPic);
            return WhiteSet;
        }
        private void Pawn_MouseEnter(object sender, EventArgs e)
        {
            // Highlight player figures:
            // 1. Players turn.

            if(YourTurn && Color == "white")
            {
                // Image defaultPic = Hexapawn.Properties.Resources.Pawn_White;
                Image greenPic= Hexapawn.Properties.Resources.Pawn_White_GBorder;
                

                if (YourTurn && !Selected && Color == "white")
                {
                    PawnPic.Image = greenPic;
                }
            }
        }
        private void Pawn_MouseLeave(object sender, EventArgs e)
        {
            if (Color == "white" && YourTurn && !Selected)
            {
                Image defaultPic = Hexapawn.Properties.Resources.Pawn_White;
                // Image greenPic = Hexapawn.Properties.Resources.Pawn_White_GBorder;
                PawnPic.Image = defaultPic;
            }
        }
        private void Pawn_MouseClick(object sender, MouseEventArgs e)
        {
            // Select this one.
            if(YourTurn && Color == "white" && !Selected)
            {
                Selected = true;
                DeselectOthers = true;
                PawnPic.Image = Hexapawn.Properties.Resources.Pawn_White_GBorder;
                foreach(Pawn p in WhiteSet)
                {
                    if(p.Selected && !p.DeselectOthers)
                    {
                        p.PawnPic.Image = Hexapawn.Properties.Resources.Pawn_White;
                        p.Selected = false;
                        p.DeselectOthers = false;
                    }
                }
                DeselectOthers = false;
            }
            else if(YourTurn && Color == "white" && Selected)
            {
                Selected = false;
                PawnPic.Image = Hexapawn.Properties.Resources.Pawn_White;
            }
        }
        private void KillBlackPawn_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if it's a player's turn.
            if(!YourTurn)
            {
                // Check if white pawn is selected.
                foreach(Pawn p in WhiteSet)
                {
                    if(p.Selected)
                    {
                        List<char> allColumns = new List<char>();
                        char selectedColumn = p.Possition.Name[0];
                        char destinationColumn = Possition.Name[0];
                        int selectedRow = int.Parse(p.Possition.Name[1].ToString());
                        int destinationRow = int.Parse(Possition.Name[1].ToString());
                        
                        foreach(Panel column in Board.ABC1)
                        {
                            allColumns.Add(column.Name[0]);
                        }

                        // Check if selected pawn moves to a row above it.
                        if((selectedRow + 1) == destinationRow)
                        {
                            for (int i = 0; i < allColumns.Count; i++)
                            {
                                if (selectedColumn == allColumns[i])
                                {
                                    // Check if destination is diagonaly to the right from selected column.
                                    if (i < allColumns.Count - 1)
                                    {
                                        if (destinationColumn == allColumns[i + 1])
                                        {
                                            // Delete black pawn and move white pawn.
                                            //KillBlackPawn();
                                            p.MovePawn(Possition);
                                        }
                                    }
                                    // Check if destination is diagonaly to the left from selected column.
                                    else if (i > 0)
                                    {
                                        if (destinationColumn == allColumns[i - 1])
                                        {
                                            // Delete black pawn and move white pawn.
                                            //KillBlackPawn();
                                            p.MovePawn(Possition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
