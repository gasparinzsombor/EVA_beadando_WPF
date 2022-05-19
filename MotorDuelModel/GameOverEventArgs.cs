using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class GameOverEventArgs
    {
        public Player Winner { get; private set; }

        public GameOverEventArgs(Player winner)
        {
            Winner = winner;
        }
    }
}
