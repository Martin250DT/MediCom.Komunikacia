using MediCom.Komunikacia.ViewModels;
using System.Windows;

namespace MediCom.Komunikacia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
