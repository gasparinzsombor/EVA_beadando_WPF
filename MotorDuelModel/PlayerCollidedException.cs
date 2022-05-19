using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PlayerCollidedException : Exception
    {
        public Player Player { get; private set; }

        public PlayerCollidedException(Player player) => Player = player;
    }
}
