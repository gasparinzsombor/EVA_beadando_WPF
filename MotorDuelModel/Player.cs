using Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Player
    {
        private Point _location;

        public string Name { get; private set; }
        public Point Location { get => _location; private set { PreviousLocation = Location; _location = value; } }

        public Point PreviousLocation { get; private set; }
        public List<Point> Trail { get; private set; }
        public Direction Direction { get; private set; }

        public string Color { get; private set; }

        public string TrailColor { get; private set; }

        public Player(string name, Point location, Direction direction, List<Point> trail = null)
        {
            Name = name;
            Location = location;
            Direction = direction;
            Trail = trail ?? new List<Point>();
            Color = ColorPool[Counter];
            TrailColor = TrailColorPool[Counter];
            Counter++;
        }

        public void MoveForward(MotorDuelModel model)
        {
            Trail.Add(Location);
            switch (Direction)
            {
                case Direction.Up:
                    Location = new Point(Location.X, Location.Y - 1);
                    break;
                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + 1);
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - 1, Location.Y);
                    break;
                default:
                    Location = new Point(Location.X + 1, Location.Y);
                    break;

            }

            if (this.HasCollidedWithSomething(model))
            {
                throw new PlayerCollidedException(this);
            }
            else
            {
                PlayerMove?.Invoke(this, null);
            }
        }

        public void TurnLeft()
        {
            switch (Direction)
            {
                case Direction.Up:
                    Direction = Direction.Left;
                    break;
                case Direction.Down:
                    Direction = Direction.Right;
                    break;
                case Direction.Left:
                    Direction = Direction.Down;
                    break;
                case Direction.Right:
                    Direction = Direction.Up;
                    break;
                default:
                    break;

            }

            PlayerTurn?.Invoke(this, null);
        }

        public void TurnRight()
        {
            switch (Direction)
            {
                case Direction.Up:
                    Direction = Direction.Right;
                    break;
                case Direction.Down:
                    Direction = Direction.Left;
                    break;
                case Direction.Left:
                    Direction = Direction.Up;
                    break;
                case Direction.Right:
                    Direction = Direction.Down;
                    break;
                default:
                    break;
            }

            PlayerTurn?.Invoke(this, null);
        }

        private bool HasCollidedWithSomething(MotorDuelModel model)
        {
            var n = (int)(model.FieldSize);

            return (model.PlayerA.Location.Equals(Location) && !this.Equals(model.PlayerA))
                || (model.PlayerB.Location.Equals(Location) && !this.Equals(model.PlayerB))
                || model.PlayerA.Trail.Contains(Location)
                || model.PlayerB.Trail.Contains(Location)
                || Location.X < 0 || n <= Location.X
                || Location.Y < 0 || n <= Location.Y;
        }

        public static Player FromPlayerState(PlayerState state)
        {
            return new Player(
                name: state.Name,
                direction: state.Direction,
                location: state.Location,
                trail: state.Trail);
        }

        public PlayerState ToPlayerState()
        {
            var state = new PlayerState();
            state.Direction = Direction;
            state.Location = Location;
            state.Name = Name;
            state.Trail = Trail;
            return state;
        }

        public event EventHandler PlayerMove;

        public event EventHandler PlayerTurn;

        private static string[] ColorPool = new string[] { "Red", "Blue" };
        private static string[] TrailColorPool = new string[] { "Orange", "DeepSkyBlue" };
        private static int Counter
        {
            get => _counter;
            set => _counter = value == 2 ? 0 : value;
        }
        private static int _counter = 0;

    }
}
