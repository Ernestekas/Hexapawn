using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hexapawn.Objects;
using Hexapawn.Objects.Database;

namespace Hexapawn
{
    public partial class Form1 : Form
    {
        GameTable board = new GameTable();
        Sets gamePieces = new Sets();
        DB_DataTable AllData = new DB_DataTable();
        
        public Form1()
        {
            InitializeComponent();
            board.ABC1 = new List<Panel>() { A1, B1, C1 };
            board.ABC2 = new List<Panel>() { A2, B2, C2 };
            board.ABC3 = new List<Panel>() { A3, B3, C3 };
            board.A123 = new List<Panel>() { A1, A2, A3 };
            board.B123 = new List<Panel>() { B1, B2, B3 };
            board.C123 = new List<Panel>() { C1, C2, C3 };
            this.Icon = Hexapawn.Properties.Resources.Hexapawn_Ic;

            AllData.DataContainer = Database;
            AllData.GameTable = board;
            
            foreach(Panel c in board.Table)
            {
                c.MouseClick += new MouseEventHandler(Panel_MouseClick);
            }
            LastWinTALbl.Text = "0";
            TurnLbl.Text = "0";
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            foreach(Panel c in board.Table)
            {
                c.Controls.Clear();
            }
            CreatePawns(board.ABC1, "white");
            CreatePawns(board.ABC3, "black");
            board.Turn = 1;
            TurnLbl.Text = board.Turn.ToString();
            LBpawnMoveLbl.Text = "n/a";
            AllData.LastMoveID = "";
            AllData.LastButOneMoveId = "";
            DebugLbl.Text = "";
        }

        private void CreatePawns(List<Panel> line, string pawnColor)
        {
            List<Pawn> set = new List<Pawn>();

            foreach(Panel cell in line)
            {
                Pawn pawn = new Pawn()
                {
                    Name = cell.Name,
                    Color = pawnColor,
                    YourTurn = true,
                    Possition = cell,
                    Board = board
                };

                pawn.CreatePawn();
                set.Add(pawn);
            }
            if(pawnColor == "white")
            {
                foreach (Pawn p in set)
                {
                    gamePieces.White = p.WhiteSet = set;
                }
            }
            else if(pawnColor == "black")
            {
                foreach(Pawn p in set)
                {
                    gamePieces.Black = p.BlackSet = set;
                    p.WhiteSet = gamePieces.White;
                    p.PawnPic.MouseClick += new MouseEventHandler(BlackPawnKill_MouseClick);
                }
            }
            foreach(Pawn p in gamePieces.White)
            {
                p.BlackSet = gamePieces.Black;
            }
        }

        /// <summary>
        /// Peaceful move EventHandler without killing enemy pawns.
        /// </summary>
        private void Panel_MouseClick(object sender, MouseEventArgs e)
        {
            // Moves selected piece.
            Panel cp = (Panel)sender;
            
            if(int.Parse(TurnLbl.Text) % 2 != 0)
            {
                
                bool ok = false;
                foreach(Pawn p in gamePieces.White)
                {
                    if (p.Selected)
                    {
                        ok = true;
                    }
                }
                if(ok)
                {
                    bool end = false;
                    // Check selected piece possible moves.
                    foreach (Pawn p in gamePieces.White)
                    {
                        if(p.Selected)
                        {
                            List<char> allColumns = new List<char>();
                            char selectedColumn = p.Possition.Name[0];
                            char destinationColumn = cp.Name[0];
                            int selectedRow = int.Parse(p.Possition.Name[1].ToString());
                            int destinationRow = int.Parse(cp.Name[1].ToString());
                            

                            // Get all columns names.
                            foreach(Panel c in board.ABC1)
                            {
                                allColumns.Add(c.Name[0]);
                            }
                            // Check if it can move forward.
                            if(selectedColumn == destinationColumn && (selectedRow + 1) == destinationRow
                                && cp.Controls.Count < 1)
                            {
                                // Change selected pawn possition on screen and in parameter.
                                p.MovePawn(cp);
                                end = true;
                                DebugLbl.Text += "(" + p.Name + "-" + cp.Name + ") ";
                            }
                        }
                    }
                    if(end)
                    {
                        EndTurn();
                    }
                }
            }
        }

        private void BlackPawnKill_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox pawnToKill = (PictureBox)sender;
            bool possibleKillMove = false;

            // Get name of a target pawn possition.
            List<Pawn> getPawns = gamePieces.Black.Where(x => x.PawnPic.Name == pawnToKill.Name).ToList();
            List<Pawn> swPawns = gamePieces.White.Where(x => x.Selected == true).ToList();

            Pawn targetPawn = getPawns.Single();
            if(swPawns.Any())
            {
                Pawn swPawn = swPawns.Single();
                foreach (Panel pMove in swPawn.PossibleMoves)
                {
                    if (pMove.Name == targetPawn.Possition.Name)
                    {
                        possibleKillMove = true;
                    }
                }


                // Check if it is player's turn.
                if (board.Turn % 2 != 0 && possibleKillMove)
                {
                    // Check if white pawn is selected.
                    foreach (Pawn p in gamePieces.White)
                    {
                        if (p.Selected)
                        {
                            // Destination possition. Define.
                            char destinationColumn;
                            int destinationRow;
                            // Selected possition.
                            char selectedColumn = p.Possition.Name[0];
                            int selectedRow = int.Parse(p.Possition.Name[1].ToString());
                            // Destination possition (Black pawn click).
                            foreach (Pawn bp in gamePieces.Black)
                            {
                                if (bp.PawnPic.Name == pawnToKill.Name)
                                {
                                    List<char> allColumns = new List<char>();
                                    destinationColumn = bp.Possition.Name[0];
                                    destinationRow = int.Parse(bp.Possition.Name[1].ToString());

                                    foreach (Panel c in board.ABC1)
                                    {
                                        allColumns.Add(c.Name[0]);
                                    }

                                    // Check if selected pawn moves to a row above.
                                    if (destinationRow == selectedRow + 1)
                                    {
                                        for (int i = 0; i < allColumns.Count; i++)
                                        {
                                            if (selectedColumn == allColumns[i])
                                            {
                                                // Check if destination column is on the right from selected col.
                                                if (i < allColumns.Count - 1)
                                                {
                                                    if (destinationColumn == allColumns[i + 1])
                                                    {
                                                        p.MovePawn(bp.Possition);
                                                        DebugLbl.Text += "(" + p.Name +"-" + bp.Possition.Name + ") ";
                                                    }
                                                }
                                                // Check if destination column is on the left from selected column.
                                                if (i > 0)
                                                {
                                                    if (destinationColumn == allColumns[i - 1])
                                                    {
                                                        p.MovePawn(bp.Possition);
                                                        DebugLbl.Text += "(" + p.Name +"-" + bp.Possition.Name + ") ";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // It is players turn yet. Compare which white pawn is in the same possition as black
                    // And remove black pawn.
                    Pawn blackPawn = new Pawn();
                    foreach (Pawn wp in gamePieces.White)
                    {
                        foreach (Pawn bp in gamePieces.Black)
                        {
                            if (wp.Possition.Name == bp.Possition.Name)
                            {
                                blackPawn = bp;
                            }
                        }
                    }
                    // Remove pawn from board.
                    //gamePieces.Black.Clear();
                    gamePieces.Black = blackPawn.KillBlackPawn(blackPawn.Possition);
                    EndTurn();
                }
            }
        }

        private void EndTurn(bool surrender = false)
        {
            bool win = false;
            board.Turn += 1;

            // New turn is opponent's turn. So last turn was players turn.
            if(board.Turn % 2 == 0)
            {
                // End turn for player. Start turn for AI.
                foreach (Pawn p in gamePieces.White)
                {
                    p.YourTurn = false;
                }

                foreach (Pawn p in gamePieces.Black)
                {
                    p.YourTurn = true;
                }

                // Check victory for white player.
                win = CheckVictory(true);

                if(!win)
                {
                    // Start turn for AI.
                    AITurn();
                }
                else
                {
                    // White pawns player wins. Decrease black pawn last move reward by one.
                    WVLbl.Text = (int.Parse(WVLbl.Text) + 1).ToString();
                    LastWinTALbl.Text = "0";
                    ManageRewards_1(false);
                    MessageBox.Show("White WINS!");
                }
            }
            else
            {
                win = CheckVictory(false);
                if(surrender)
                {
                    // AI surrender. White player wins.
                    WVLbl.Text = (int.Parse(WVLbl.Text) + 1).ToString();
                    LastWinTALbl.Text = "0";
                    ManageRewards_1(false, true);
                    MessageBox.Show("White WINS!");
                }
                else
                {
                    if (win)
                    {
                        // Black pawns player wins. Increase last moves reward by one.
                        BVLbl.Text = (int.Parse(BVLbl.Text) + 1).ToString();
                        LastWinTALbl.Text = (int.Parse(LastWinTALbl.Text) + 1).ToString();
                        ManageRewards_1(true);
                        MessageBox.Show("Black WINS!");
                    }
                    else
                    {
                        // End turn for AI.
                        foreach (Pawn p in gamePieces.White)
                        {
                            p.YourTurn = true;
                        }
                        foreach (Pawn p in gamePieces.Black)
                        {
                            p.YourTurn = false;
                        }
                    }
                }
            }
            
            TurnLbl.Text = board.Turn.ToString();
        }

        private void AITurn()
        {
            DB_Turn selectedTurn = new DB_Turn();
            DataGridView selectedVariant = new DataGridView();
            Pawn selectedPawn = new Pawn();
            Panel selectedMove = new Panel();
            List<int> allIntervals = new List<int>();
            List<int> allrewards = new List<int>();
            bool turnExists = false;
            bool variantExist = false;
            bool surrender = false;
            int rnd = 0;
            
            // Pickup data.
            if(AllData.Turns.Any())
            {
                if(AllData.Turns.Where(x => x.Turn == board.Turn) != null && AllData.Turns.Where(x => x.Turn == board.Turn).Any())
                {
                    turnExists = true;
                    selectedTurn = AllData.Turns.Where(x => x.Turn == board.Turn).Single();
                }
            }
            
            if(!turnExists)
            {
                // Create turn.
                AllData.CurrentTurn = board.Turn;
                AllData.CreateTurn(gamePieces);
            }

            selectedTurn = AllData.Turns.Where(x => x.Turn == board.Turn).Single();

            // Check variants.
            variantExist = selectedTurn.CompareVariantWithTable(gamePieces);
            if (variantExist)
            {
                selectedVariant = selectedTurn.SelectedVariant;
            }
            else
            {
                selectedTurn.CreateNewVariant(gamePieces, Database.Controls.OfType<Panel>().ToList());
                selectedTurn.CompareVariantWithTable(gamePieces);
                selectedVariant = selectedTurn.SelectedVariant;
            }
            
            // Write rewards to black pawns and calculate reward intervals.
            foreach (Pawn p in gamePieces.Black)
            {
                for(int rowIndex = 0; rowIndex < selectedVariant.RowCount - 1; rowIndex++)
                {
                    if(p.Name == selectedVariant.Rows[rowIndex].Cells[0].Value.ToString().Substring(1)
                        && selectedVariant.Rows[rowIndex].Cells[1].Value.ToString() == p.Possition.Name)
                    {
                        p.MovesRewards.Clear();
                        for (int i = 0; i < p.PossibleMoves.Count; i++)
                        {
                            if(selectedVariant.Rows[rowIndex].Cells[5 + i].Value != null)
                            {
                                p.MovesRewards.Add(int.Parse(selectedVariant.Rows[rowIndex].Cells[5 + i].Value.ToString()));
                            }
                        }
                    }
                }
            }

            gamePieces.CreateRewardIntervals();
            
            // Choose a move.
            foreach(Pawn p in gamePieces.Black)
            {
                allIntervals.AddRange(p.MovesRewardsIntervals);
                allrewards.AddRange(p.MovesRewards);
            }

            if(allrewards.Count == allrewards.Where(x => x == 0).Count())
            {
                surrender = true;
            }

            rnd = GenerateRandomNumber(1, allIntervals.Max());
            foreach (Pawn p in gamePieces.Black)
            {
                for (int i = 0; i < p.MovesRewardsIntervals.Count; i++)
                {
                    if (rnd > p.MovesRewardsIntervals[i] - p.MovesRewards[i] && rnd <= p.MovesRewardsIntervals[i])
                    {
                        selectedPawn = p;
                        selectedMove = p.PossibleMoves[i];
                    }
                }
            }
            selectedPawn.MovePawn(selectedMove);
            if(selectedMove.Controls.OfType<PictureBox>().Count() > 1)
            {
                selectedPawn.KillWhitePawn(selectedMove);
            }
            LBpawnMoveLbl.Text = selectedPawn.Name + "-" + selectedMove.Name;
            if(AllData.LastMoveID == "")
            {
                AllData.LastMoveID = selectedTurn.TurnName + "-" + selectedVariant.Name + "-" + selectedPawn.Name + "-" + selectedMove.Name;
            }
            else
            {
                AllData.LastButOneMoveId = AllData.LastMoveID;
                AllData.LastMoveID = selectedTurn.TurnName + "-" + selectedVariant.Name + "-" + selectedPawn.Name + "-" + selectedMove.Name;
            }
            DebugLbl.Text += "(" + selectedPawn.Name + "-" + selectedMove.Name + ") ";
            EndTurn(surrender);
        }
        private int GenerateRandomNumber(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max + 1);
        }

        private bool CheckVictory(bool checkWhiteWin)
        {
            List<bool> opponentCanMove = new List<bool>();
            List<Pawn> victoryPawnsCheck = new List<Pawn>();
            List<Pawn> opponentPawns = new List<Pawn>();
            List<Panel> endLine = new List<Panel>();

            bool victory = false;
            
            if(checkWhiteWin)
            {
                victoryPawnsCheck = gamePieces.White;
                opponentPawns = gamePieces.Black;
                endLine = board.ABC3;
            }
            else
            {
                victoryPawnsCheck = gamePieces.Black;
                opponentPawns = gamePieces.White;
                endLine = board.ABC1;
            }

            // Check if reached at least one pawn reached end line.
            foreach(Pawn p in victoryPawnsCheck)
            {
                foreach(Panel endCell in endLine)
                {
                    if(p.Possition.Name == endCell.Name)
                    {
                        victory = true;
                    }
                }
            }

            // Check if opponent can't make a move on his turn.
            foreach(Pawn op in opponentPawns)
            {
                if(op.PossibleMoves.Any())
                {
                    opponentCanMove.Add(true);
                }
            }
  
            if (!opponentCanMove.Contains(true))
            {
                victory = true;
            }

            // Check if there is no opponents pawns.
            if (!opponentPawns.Any())
            {
                victory = true;
            }

            return victory;
        }
        private void ManageRewards_1(bool increaseReward, bool surrender = false)
        {
            List<bool> bpZero = new List<bool>();
            List<string> lboMoveID = new List<string>();
            List<int> allVariantRewards = new List<int>();
            List<int> allVariantRewardsFiltered = new List<int>();
            DataGridView lboSelectedVariant = new DataGridView();
            DataGridViewCell rewardCell = null;
            List<string> lastMoveID = AllData.LastMoveID.Split('-').ToList();
            
            
            if(AllData.LastButOneMoveId.Count() > 0)
            {
                lboMoveID = AllData.LastButOneMoveId.Split('-').ToList();
                lboSelectedVariant = AllData.Turns
                    .Where(x => x.TurnName == lboMoveID[0])
                    .Single().Variants.Where(y => y.Variant.Name == lboMoveID[1]).Single().Variant;
            }
            if(surrender)
            {
                lastMoveID = lboMoveID;

            }
            // Get variant.
            DataGridView selectedVariant = AllData.Turns
                .Where(x => x.TurnName == lastMoveID[0])
                .Single().Variants.Where(y => y.Variant.Name == lastMoveID[1]).Single().Variant;

            // Get cell of a reward to change.
            foreach (DataGridViewRow row in selectedVariant.Rows)
            {
                if(row.Cells[0].Value != null)
                {
                    if(row.Cells[0].Value.ToString() == "b" + lastMoveID[2])
                    {
                        for(int columnIndex = 2; columnIndex < 5; columnIndex++)
                        {
                            if(row.Cells[columnIndex].Value != null && row.Cells[columnIndex + 3].Value != null)
                            {
                                if(row.Cells[columnIndex].Value.ToString() == lastMoveID[3])
                                {
                                    rewardCell = row.Cells[columnIndex + 3];
                                }
                            }
                            else if(row.Cells[columnIndex].Value != null && row.Cells[columnIndex + 3].Value == null)
                            {
                                MessageBox.Show("Error. Possible move: " + row.Cells[columnIndex].Value + " Reward: " + row.Cells[columnIndex + 3].Value);
                            }
                        }
                    }
                }
            }
            if(increaseReward)
            {
                rewardCell.Value = (int.Parse(rewardCell.Value.ToString()) + 1).ToString();
            }
            else
            {
                rewardCell.Value = (int.Parse(rewardCell.Value.ToString()) - 1).ToString();
            }

            // If all rewards in this variant is 0 change last but one variant selected turn reward value.
            foreach(DataGridViewRow row in selectedVariant.Rows)
            {
                if(row.Cells[0].Value != null)
                {
                    for(int i = 5; i < 8; i++)
                    {
                        if(row.Cells[i].Value != null)
                        {
                            allVariantRewards.Add(int.Parse(row.Cells[i].Value.ToString()));
                        }
                    }
                }
            }
            allVariantRewardsFiltered = allVariantRewards.Where(x => x == 0).ToList();
            if (allVariantRewards.Count == allVariantRewardsFiltered.Count)
            {
                // All variant rewards equals to zero. Change last but one selected move reward.
                // Get cell to change.
                foreach (DataGridViewRow row in lboSelectedVariant.Rows)
                {
                    // Check if this row isn't empty.
                    if(row.Cells[0].Value != null)
                    {
                        // Check Last but one move id and select that row which matches pawn from ID.
                        if(row.Cells[0].Value.ToString() == "b" + lboMoveID[2])
                        {
                            // Cycle possible moves.
                            for(int i = 2; i < 5; i++)
                            {
                                if(row.Cells[i].Value != null)
                                {
                                    // Cycle through moves.
                                    if(row.Cells[i].Value.ToString() == lboMoveID[3])
                                    {
                                        // Move from table matches ID move. Change reward.
                                        row.Cells[i + 3].Value = (int.Parse(row.Cells[i + 3].Value.ToString()) - 1).ToString();
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
