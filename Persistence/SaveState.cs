using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public struct SaveState
    {
        public TimeSpan Time;
        public FieldSize FieldSize;
        public PlayerState PlayerA;
        public PlayerState PlayerB;

        public string ToFileString()
        {
            return Time.Seconds + Environment.NewLine +
                FieldSize + Environment.NewLine +
                PlayerA.ToFileString() + Environment.NewLine +
                PlayerB.ToFileString();
        }

        public static SaveState Parse(string str)
        {
            var state = new SaveState();
            var data = str.Split(Environment.NewLine);
            state.Time = TimeSpan.FromSeconds(double.Parse(data[0]));
            state.FieldSize = Enum.Parse<FieldSize>(data[1]);
            state.PlayerA = PlayerState.Parse(line: data[2], name: "Piros");
            state.PlayerB = PlayerState.Parse(line: data[3], name: "Kék");

            return state;
        }
    }

    public struct PlayerState
    {
        public string Name;
        public Point Location;
        public List<Point> Trail;
        public Direction Direction;

        public string ToFileString()
        {
            var result = $"{Direction} {Location.X} {Location.Y}";
            foreach(var point in Trail)
            {
                result += $" {point.X} {point.Y}";
            }
            return result;
        }

        public static PlayerState Parse(string line, string name)
        {
            var state = new PlayerState();
            state.Name = name;
            var data = line.Split(" ");
            state.Direction = Enum.Parse<Direction>(data[0]);
            state.Location.X = int.Parse(data[1]);
            state.Location.Y = int.Parse(data[2]);

            state.Trail = new List<Point>();
            for(int i = 4; i < data.Length; i+=2)
            {
                var point = new Point();
                point.X = int.Parse(data[i - 1]);
                point.Y = int.Parse(data[i]);
                state.Trail.Add(point);
            }

            return state;
        }
    }
}
