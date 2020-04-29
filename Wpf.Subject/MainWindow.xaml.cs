using System;
using System.Reactive.Linq;
using System.Windows;

namespace Wpf.Subject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            Observable.FromEventPattern(
                h => Application.Current.Activated += h,
                h => Application.Current.Activated -= h).Subscribe(x => Console.WriteLine("app activated"));
        }
    }
}
