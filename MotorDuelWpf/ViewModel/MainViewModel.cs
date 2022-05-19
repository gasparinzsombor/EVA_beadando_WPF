using Microsoft.Win32;
using Model;
using ViewModel;
using Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ColorConverter = System.Windows.Media.ColorConverter;
using Point = System.Drawing.Point;
using System.Windows.Media.Imaging;

namespace ViewModel
{
    class MainViewModel : ViewModelBase
    {

        #region Fields

        private int _fieldSize = 24;
        private string _commandMessage;
        private readonly MotorDuelModel _model;
        private bool _gameIsNotRunning = true;

        #endregion

        #region Properties

        public int FieldSize
        {
            get => _fieldSize;
            set { _fieldSize = value; OnPropertyChanged(); }
        }

        public string CommandMessage
        {
            get => _commandMessage;
            set { _commandMessage = value; OnPropertyChanged(); }
        }

        public bool GameIsNotRunning
        {
            get => _gameIsNotRunning;
            set { _gameIsNotRunning = value; OnPropertyChanged(); }
        }

        public bool GameIsNotOver
        {
            get => !_model.IsGameOver;
        }

        public int ElapsedSeconds
        {
            get => _model.Time.Seconds;
        }

        public DelegateCommand EnterKeyCommand { get; }

        public DelegateCommand LeftKeyCommand { get; }

        public DelegateCommand RightKeyCommand { get; }

        public DelegateCommand AKeyCommand { get; }

        public DelegateCommand DKeyCommand { get; }

        public DelegateCommand SaveMenuCommand { get; }

        public DelegateCommand OpenMenuCommand { get; }

        public DelegateCommand NewGameCommand { get; }

        public ObservableCollection<PanelViewModel> Panels { get; } = new ObservableCollection<PanelViewModel>();

        #endregion

        #region Contructors

        public MainViewModel(MotorDuelModel model)
        {
            _model = model;
            _model.Load += Model_Load;
            _model.GameIsRunningChange += Model_GameIsRunningChange;
            _model.GameOver += Model_GameOver;
            _model.NewGame();

            

            EnterKeyCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => _model.ToggleGameIsRunning());
            LeftKeyCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => _model.PlayerB.TurnLeft());
            RightKeyCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => _model.PlayerB.TurnRight());
            AKeyCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => _model.PlayerA.TurnLeft());
            DKeyCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => _model.PlayerA.TurnRight());
            SaveMenuCommand = new DelegateCommand(_ => OnSave?.Invoke(this, null));
            OpenMenuCommand = new DelegateCommand(_ => OnOpen?.Invoke(this, null));
            NewGameCommand = new DelegateCommand(s => NewGame(int.Parse(s.ToString())));
        }

        #endregion

        #region ModelEventHandlers


        private void Model_GameIsRunningChange(object sender, EventArgs e)
        {
            GameIsNotRunning = !_model.GameIsRunning;
            CommandMessage = _model.GameIsRunning ? "Nyomj Entert a megállításhoz!" : "Nyomj Entert az indításoz!";
        }

        private void Model_GameOver(object sender, GameOverEventArgs e)
        {
            CommandMessage = "";
            OnPropertyChanged(nameof(GameIsNotOver));
        }

        private void Model_Load(object sender, EventArgs e)
        {
            CommandMessage = "Nyomj Entert az inításhoz!";
            GameIsNotRunning = true;
            FieldSize = (int)_model.FieldSize;
            Panels.Clear();
            for (int i = 0; i < FieldSize * FieldSize; i++)
            {
                var panel = new PanelViewModel(FieldSize, i);
                Panels.Add(panel);
            }

            SetupPlayerPanels(_model.PlayerA);
            SetupPlayerPanels(_model.PlayerB);

            _model.PlayerA.PlayerMove += Player_PlayerMove;
            _model.PlayerA.PlayerTurn += Player_PlayerTurn;
            _model.PlayerB.PlayerMove += Player_PlayerMove;
            _model.PlayerB.PlayerTurn += Player_PlayerTurn;
        }

        private void Player_PlayerMove(object sender, EventArgs e)
        {
            var player = sender as Player;
            var index = PointToIndex(player.Location);
            Panels[index].Fill = PanelViewModel.ImageOf(player);
            Panels[index].Rotation = (int)player.Direction;

            var lastIndex = PointToIndex(player.PreviousLocation);
            Panels[lastIndex].Fill = PanelViewModel.TrailBrushOf(player);
        }

        private void Player_PlayerTurn(object sender, EventArgs e)
        {
            var player = sender as Player;
            var index = PointToIndex(player.Location);
            Panels[index].Rotation = (int)player.Direction;
        }

        #endregion

        #region PrivateMethods

        private int PointToIndex(Point point)
        {
            return (point.Y * FieldSize) + point.X;
        }

        private void NewGame(int size)
        {
            _model.NewGame((FieldSize)size);
        }

        private void SetupPlayerPanels(Player player)
        {
            int index = PointToIndex(player.Location);
            Panels[index].Fill = PanelViewModel.ImageOf(player);
            Panels[index].Rotation = (int)player.Direction;

            foreach(var point in player.Trail)
            {
                index = PointToIndex(point);
                Panels[index].Fill = PanelViewModel.TrailBrushOf(player);
            }
        }

        #endregion

        #region Events

        public event EventHandler OnSave;
        public event EventHandler OnOpen;

        #endregion
    }
}
