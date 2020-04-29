using System;

namespace Console.HotCold
{

    public class Button
    {
        private EventHandler<string> _onClickHandler;

        public event EventHandler<string> Click
        {
            add
            {
                _onClickHandler += value;
            }
            remove
            {
                _onClickHandler -= value;
            }
        }

        public void DoClick(string s)
        {
            _onClickHandler?.Invoke(this, s);
        }
    }
}