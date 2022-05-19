using Microsoft.Win32;
using Model;
using ViewModel;
using Persistence;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using View;

namespace MotorDuelWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainViewModel _viewModel;
        private MainWindow _window;
        private MotorDuelModel _model;
        private DispatcherTimer _timer;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new MotorDuelModel(new FileDataAccess());
            _model.GameIsRunningChange += Model_GameIsRunningChange;
            _model.GameOver += Model_GameOver;

            _window = new MainWindow();
            _viewModel = new MainViewModel(_model);
            _viewModel.OnOpen += ViewModel_OnOpen;
            _viewModel.OnSave += ViewModel_OnSave;


            _window.DataContext = _viewModel;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;


            _window.Show();
        }

        private async void ViewModel_OnSave(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "Tron Motor Dual files|*.tmd|All files|*.*",
                FilterIndex = 2,
                DefaultExt = "tmd"
            };

            if(dialog.ShowDialog() == true)
            {
                await _model.SaveAsync(dialog.FileName);
            }
        }

        private async void ViewModel_OnOpen(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Tron Motor Dual files|*.tmd|All files|*.*",
                FilterIndex = 2,
                DefaultExt = "tmd"
            };

            if (dialog.ShowDialog() == true)
            {
                await _model.OpenFileAsync(dialog.FileName);
            }
        }

        private void Model_GameOver(object sender, GameOverEventArgs e)
        {
            _timer.Stop();
            MessageBox.Show(
                messageBoxText: $"A nyertes: {e.Winner.Name}",
                caption: "",
                icon: MessageBoxImage.Information,
                button: MessageBoxButton.OK
            );
        }

        private void Model_GameIsRunningChange(object sender, EventArgs e)
        {
            if(_model.GameIsRunning)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _model.GameAdvanced();
            _viewModel.OnPropertyChanged(nameof(_viewModel.ElapsedSeconds));
        }
    }
}