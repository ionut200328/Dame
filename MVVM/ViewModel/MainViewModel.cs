using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dame.Services;
using Dame.Core;
using System.Windows.Input;
using System.Runtime.InteropServices;
using Dame.MVVM.Model;

namespace Dame.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
        private INavigationService _navigationService;
        private ICommand _aboutCommand;
        private ICommand _statisticsCommand;

        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        public ICommand AboutCommand
        {
            get => _aboutCommand;
            set
            {
                _aboutCommand = value;
                OnPropertyChanged();
            }
        }
       
        public ICommand StatisticsCommand
        {
            get => _statisticsCommand;
            set
            {
                _statisticsCommand = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand NavigateToGameCommand { get; set; }     
        public MainViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            //NavigationService.OnNavigated += NavigationService_OnNavigated;
            NavigateToGameCommand = new RelayCommand(execute: o => { NavigationService.NavigateTo<GameViewModel>(o); }, canExecute: o => true);
            AboutCommand = new RelayCommand(execute: o => { About(); }, canExecute: o => true);
            StatisticsCommand = new RelayCommand(execute: o => { Stats(); }, canExecute: o => true);
           
        }

        // AboutCommand pops with a message box
        public void About()
        {
            System.Windows.MessageBox.Show("Jocul de dame este un joc de societate care se joacă pe o tablă de 64 de pătrate, de 8x8, numită tablă de dame. \n" +
                "Fiecare jucător își așează piesele pe cele 12 pătrate de pe rândul din fața sa, de pe partea de jos a tablei. \n" +
                "Jocul începe cu jucătorul care are piesele negre. \n" +
                "Jucătorii mută pe rând câte o piesă, deplasându-se pe diagonalele tablei, pe pătratele libere. \n" +
                "O piesă care ajunge pe ultimul rând al tablei devine damă și se marchează prin adăugarea unei piese de aceeași culoare deasupra ei. \n" +
                "O damă se poate deplasa pe orizontală și pe verticală, pe câte pătrate libere dorește. \n" +
                "Scopul jocului este de a lua toate piesele adversarului sau de a bloca toate piesele adversarului, astfel încât acesta să nu mai poată face nicio mutare. \n" +
                "Jocul se termină atunci când unul dintre jucători nu mai poate face nicio mutare. \n\n\n" +
                "Developed by: Chițu Ioan\n" +
                "Contact: ioan.chitu@student.unitbv.ro\n" +
                "Grupa: 10LF321\n",
                "Despre jocul de dame",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
        public void Stats()
        {
            StatisticsService statistics = new StatisticsService();
            statistics.LoadStatistics();
            int blackW = statistics.BlackWins;
            int whiteW = statistics.WhiteWins;
            System.Windows.MessageBox.Show("Statistici joc:\n" +
                "Jucatorul cu piesele negre a castigat de " + blackW + " ori\n"
                +"Jucatorul cu piesele albe a castigat de " + whiteW + " ori\n",
                "Statistici joc",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
    }
}
