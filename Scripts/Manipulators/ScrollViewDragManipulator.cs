using UnityEngine;
using UnityEngine.UIElements;

namespace Manipulators
{
    public class ScrollViewDragManipulator : PointerManipulator
    {
        private Vector2 _startPos;
        private Vector2 _startScroll;
        private bool _active;

        public ScrollViewDragManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCancelEvent>(OnPointerUp);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerUp);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (target is ScrollView scrollView)
            {
                _active = true;
                _startPos = evt.position;
                _startScroll = scrollView.scrollOffset;
                target.CapturePointer(evt.pointerId);
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_active && target is ScrollView scrollView)
            {
                Vector2 delta = (Vector2)evt.position - _startPos;
                Vector2 newScroll = _startScroll - delta;

                // Manually calculate the scroll limits
                float maxX = Mathf.Max(0, scrollView.contentContainer.resolvedStyle.width - scrollView.resolvedStyle.width);
                float maxY = Mathf.Max(0, scrollView.contentContainer.resolvedStyle.height - scrollView.resolvedStyle.height);

                // Clamp scrollOffset to valid bounds
                newScroll.x = Mathf.Clamp(newScroll.x, 0, maxX);
                newScroll.y = Mathf.Clamp(newScroll.y, 0, maxY);

                scrollView.scrollOffset = newScroll;
            }
        }

        private void OnPointerUp(IPointerEvent evt)
        {
            if (_active)
            {
                _active = false;
                target.ReleasePointer(evt.pointerId);
            }
        }
    }
}