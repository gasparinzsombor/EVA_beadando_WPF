using Persistence;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Model
{
    public class MotorDuelModel
    {
        private bool _gameStarted = false;

        public Player PlayerA { get; private set; }

        public Player PlayerB { get; private set; }

        public FieldSize FieldSize { get; private set; }

        public bool GameIsRunning { get; private set; }

        public bool IsGameOver { get; private set; }

        public TimeSpan Time { get => _gameStarted ? DateTime.Now - _startTime + _alreadyElapsedTime : _alreadyElapsedTime; }

        private DateTime _startTime;

        private TimeSpan _alreadyElapsedTime;

        private readonly IDataAccess _dataAccess;

        public MotorDuelModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _alreadyElapsedTime = new TimeSpan(0);
            FieldSize = FieldSize.Size24x24;
            GameIsRunning = false;
        }

        public void NewGame(FieldSize fieldSize)
        {
            FieldSize = fieldSize;
            NewGame();
        }

        public void NewGame()
        {
            IsGameOver = false;
            GameIsRunning = false;
            _alreadyElapsedTime = new TimeSpan(0);
            _gameStarted = false;
            int n = (int)FieldSize;
            PlayerA = new Player(
                name: "Piros",
                location: new Point(n / 4, n / 2),
                direction: Direction.Down
                );


            PlayerB = new Player(
                name: "Kék",
                location: new Point(n - n / 4, n / 2),
                direction: Direction.Up
                );

            Load?.Invoke(this, null);
        }

        public void GameAdvanced()
        {
            _gameStarted = true;
            try
            {
                PlayerA.MoveForward(this);
                PlayerB.MoveForward(this);
            }
            catch (PlayerCollidedException e)
            {
                ToggleGameIsRunning();
                IsGameOver = true;
                GameOver?.Invoke(this, new GameOverEventArgs(e.Player.Equals(PlayerA) ? PlayerB : PlayerA));
            }
        }

        public void ToggleGameIsRunning()
        {
            GameIsRunning = !GameIsRunning;

            if (GameIsRunning)
            {
                _startTime = DateTime.Now;
            }
            else
            {
                _alreadyElapsedTime += DateTime.Now - _startTime;
            }
            GameIsRunningChange?.Invoke(this, null);
        }

        public async Task OpenFileAsync(string filename)
        {

            var state = await _dataAccess.LoadAsync(filename);
            FieldSize = state.FieldSize;
            _alreadyElapsedTime = state.Time;
            PlayerA = Player.FromPlayerState(state.PlayerA);
            PlayerB = Player.FromPlayerState(state.PlayerB);
            IsGameOver = false;
            _gameStarted = false;
            Load?.Invoke(this, null);


        }

        public async Task SaveAsync(string filename)
        {
            await _dataAccess.SaveAsync(filename, ToSaveState());
        }

        private SaveState ToSaveState()
        {
            var state = new SaveState();
            state.FieldSize = FieldSize;
            state.Time = _alreadyElapsedTime;
            state.PlayerA = PlayerA.ToPlayerState();
            state.PlayerB = PlayerB.ToPlayerState();
            return state;
        }

        public event EventHandler Load;

        public event EventHandler<GameOverEventArgs> GameOver;

        public event EventHandler GameIsRunningChange;
    }
}
