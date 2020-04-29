using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Wpf.Utils;

namespace Wpf.Subject
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Subject<object> _clickSubject = new Subject<object>();

        public MainWindowViewModel()
        {
            _clickSubject.AsObservable() // if we want to expose it outside, its nice to hide subject methods
                .Subscribe(
                    value => TextBox += "click\n",
                    error => TextBox += "error\n",
                    () => TextBox += "completed\n");
        }

        public ICommand ClickCommand => new Command(x => _clickSubject.OnNext(x));

        public ICommand ErrorCommand => new Command(_ => _clickSubject.OnError(new Exception("fail")));

        public ICommand CompleteCommand => new Command(_ => _clickSubject.OnCompleted());

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