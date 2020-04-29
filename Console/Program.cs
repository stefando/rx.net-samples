using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var neverObservable = Observable.Never<int>(); // infinite
            neverObservable.Subscribe(new DummyObserver<int>(nameof(neverObservable)));

            var emptyObservable = Observable.Empty<int>();
            emptyObservable.Subscribe(new DummyObserver<int>(nameof(emptyObservable)));

            var throwObservable = Observable.Throw<int>(new Exception("fail"));
            throwObservable.Subscribe(new DummyObserver<int>(nameof(throwObservable)));

            var singleValueObservable = Observable.Return<string>("hello");
            singleValueObservable.Subscribe(new DummyObserver<string>(nameof(singleValueObservable)));

            var rangeObservable = Observable.Range(10, 15);
            rangeObservable.Subscribe(new DummyObserver<int>(nameof(rangeObservable)));

            var actionObservable = Observable.Start(() => 42);
            actionObservable.Subscribe(new DummyObserver<int>(nameof(actionObservable)));

            var intervalObservable = Observable.Interval(TimeSpan.FromMilliseconds(100));
            var subscription = intervalObservable.Subscribe(new DummyObserver<long>(nameof(intervalObservable)));

            var taskObservable = Task.Factory.StartNew(() => "task done").ToObservable();
            taskObservable.Subscribe(new DummyObserver<string>(nameof(taskObservable)));

            var asyncObservable1 = Observable.FromAsync(() => AsyncMethod(500)); // fires provided lambda on subscription
            asyncObservable1.Subscribe(new DummyObserver<string>(nameof(asyncObservable1)));

            var asyncObservable2 = AsyncMethod(500).ToObservable(); // fires SomeMethodAsync() immediately (even if not subscribed, but it persists the result)
            asyncObservable2.Subscribe(new DummyObserver<string>(nameof(asyncObservable2)));

            // usually overload with lambdas is used instead o full-blown observer class implementation
            new[] {"🍕", "🍪", "🍔", "🌭", "🍟"}.ToObservable().Subscribe(
                value => System.Console.WriteLine("next: {0}", value), //optional
                error => System.Console.WriteLine("error: {0}", error), // optional
                () => System.Console.WriteLine("complete")); // optional

            Thread.Sleep(2000);
            subscription.Dispose(); // unsubscribe

            // observable from events

            // event pattern = handler delegate in form of:   void Handler(object sender, EventArgs e);
            var fsw = new FileSystemWatcher(@"C:\Temp", "*.*") { EnableRaisingEvents = true };
            var fileCreatedObservable = Observable.FromEventPattern<FileSystemEventArgs>(fsw, nameof(fsw.Created));
            fileCreatedObservable.Subscribe(e => System.Console.WriteLine("{0} was created.", e.EventArgs.FullPath));

            // custom event handler delegate
            var button = new MeetupButton();
            var clickObservable = Observable.FromEvent<ButtonClickHandler, DateTime>(extraction =>
                {
                    ButtonClickHandler clickHandler = (dateTime, sender) =>
                    {
                        extraction(dateTime); // body of registered handler
                    };
                    return clickHandler;
                },
                h => button.Click += h,
                h => button.Click -= h);
            clickObservable.Subscribe(x => System.Console.WriteLine("meetup button clicked at {0}", x));
            button.DoClick();
            Thread.Sleep(1000);
            button.DoClick();
        }

        private static async Task<string> AsyncMethod(int delay)
        {
            // fake async work
            await Task.Delay(delay);
            return "async result";
        }
    }
}
