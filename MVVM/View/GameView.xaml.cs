using Dame.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dame.MVVM.View
{
    public partial class GameView : Page
    {
        public GameView()
        {
            InitializeComponent();
            //var gameStateService = new GameStateService(); 
            //var viewModel = new GameViewModel(gameStateService);
            //DataContext = viewModel;
            //viewModel.SetParameter(null);
        }

    }
}
