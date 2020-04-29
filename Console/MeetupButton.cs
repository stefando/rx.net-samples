using System;

namespace Console
{
    public delegate void ButtonClickHandler(DateTime when, MeetupButton origin);

    public class MeetupButton
    {
        private ButtonClickHandler _onClickHandler;

        public event ButtonClickHandler Click
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

        public void DoClick()
        {
            _onClickHandler?.Invoke(DateTime.Now, this);
        }
    }
}