using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Wpf.Utils;

namespace Wpf.MultiClick
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Subject<object> _clickSubject = new Subject<object>();

        public MainWindowViewModel()
        {
            _clickSubject
                .Do(x => TextBox += "click\n") // for debugging only
                .Buffer(_clickSubject.Throttle(TimeSpan.FromMilliseconds(250)))
                .Select(x => x.Count)
                .Where(x => x >= 2)
                .Subscribe(x => TextBox += "multi-click: " + x + "\n");
        }

        public ICommand ClickCommand => new Command(x => _clickSubject.OnNext(x));

        private string _textBox;
        public string TextBox
        {
            private set
            {
                _textBox = value;
                OnPropertyChanged();
            }
            get => _textBox;
        }
    }
}