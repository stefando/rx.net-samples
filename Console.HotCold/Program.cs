using System;
using System.Reactive.Linq;
using System.Threading;

namespace Console.HotCold
{
    class Program
    {
        static void Main(string[] args)
        {
            // cold
            var rangeObservable = Observable.Range(0, 10);
            rangeObservable.Subscribe(x => System.Console.WriteLine("rangeSub1: {0}", x));
            rangeObservable.Subscribe(x => System.Console.WriteLine("rangeSub2: {0}", x));

            System.Console.WriteLine();

            // also cold
            var intervalObservable = Observable.Interval(TimeSpan.FromMilliseconds(100));
            var sub1 = intervalObservable.Subscribe(x => System.Console.WriteLine("intervalSub1: {0}", x));
            Thread.Sleep(1000);
            var sub2 = intervalObservable.Subscribe(x => System.Console.WriteLine("intervalSub2: {0}", x));
            Thread.Sleep(1000);

            sub1.Dispose();
            sub2.Dispose();
            System.Console.WriteLine();

            // hot
            var button = new Button();
            var clickObservable = Observable.FromEventPattern<string>(h => button.Click += h, h => button.Click -= h);
            clickObservable.Subscribe(x => System.Console.WriteLine("buttonSub1: {0}", x.EventArgs));
            button.DoClick("c1");
            button.DoClick("c2");
            clickObservable.Subscribe(x => System.Console.WriteLine("buttonSub2: {0}", x.EventArgs));
            button.DoClick("c3");

            System.Console.WriteLine();

            //// codl -> hot
            var intervalObservablePublished = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Do(x => System.Console.WriteLine("intervalObservablePublished emitting: {0}", x))
                .Publish(); // Publish() makes ConnectableObservable
            intervalObservablePublished.Connect(); // this actually subscribes to source, can be moved
            //Thread.Sleep(1000);
            intervalObservablePublished.Subscribe(x => System.Console.WriteLine("intervalObservableHot1: {0}", x));
            Thread.Sleep(1000);
            intervalObservablePublished.Subscribe(x => System.Console.WriteLine("intervalObservableHot2: {0}", x));
            Thread.Sleep(1000);


            // cold -> hot 2
            //var intervalObservablePublished = Observable.Interval(TimeSpan.FromMilliseconds(100))
            //    .Do(x => System.Console.WriteLine("intervalObservablePublished emitting: {0}", x))
            //    .Publish()
            //    .RefCount();
            //Thread.Sleep(1000);
            //intervalObservablePublished.Subscribe(x => System.Console.WriteLine("intervalObservableHot1: {0}", x));
            //Thread.Sleep(1000);
            //intervalObservablePublished.Subscribe(x => System.Console.WriteLine("intervalObservableHot2: {0}", x));
            //Thread.Sleep(1000);
        }
    }
}
