using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hexapawn.Objects
{
    class Sets
    {
        public List<Pawn> White { get; set; }
        public List<Pawn> Black { get; set; }
        public GameTable GameTable { get; set; }
        public List<Pawn> All()
        {
            List<Pawn> output = new List<Pawn>();
            output.AddRange(White);
            output.AddRange(Black);
            return output;
        }
        public int CreateRewardIntervals()
        {
            int nextIntervalValue = 0;
            foreach(Pawn black in Black)
            {
                black.MovesRewardsIntervals = new List<int>();
                foreach(int reward in black.MovesRewards)
                {
                    nextIntervalValue += reward;
                    black.MovesRewardsIntervals.Add(nextIntervalValue);
                }
            }
            return nextIntervalValue;
        }
    }
}
