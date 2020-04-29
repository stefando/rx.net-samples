using System;

namespace Console
{
    public class DummyObserver<TValue> : IObserver<TValue>
    {
        private readonly string _observableName;

        public DummyObserver(string observableName)
        {
            _observableName = observableName;
        }

        public void OnNext(TValue value)
        {
            System.Console.WriteLine("{0} next: {1}", _observableName, value);
        }

        public void OnError(Exception error)
        {
            System.Console.WriteLine("{0} error: {1}", _observableName, error);
        }

        public void OnCompleted()
        {
            System.Console.WriteLine("{0} complete", _observableName);
        }
    }
}