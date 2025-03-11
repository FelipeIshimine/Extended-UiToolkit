using System;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit
{
    public static class ButtonExtensions
    {
        public static IDisposable AddListener(this Button button, Action callback)
        {
            void Unregister()
            {
                button.clicked -= callback;
            }
            button.clicked += callback;
            return new Releaser(Unregister);
        }
    }

    public class Releaser : IDisposable
    {
        private Action _callback;

        public Releaser(Action callback)
        {
            _callback = callback;
        }

        public void Dispose()
        {
            _callback.Invoke();
            _callback = null;
        }
    }
}