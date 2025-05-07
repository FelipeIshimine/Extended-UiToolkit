using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static async Awaitable WaitForPress(this VisualElement element,CancellationToken token)
    {
        AwaitableCompletionSource completionSource = new AwaitableCompletionSource();
        element.RegisterCallbackOnce<PointerDownEvent>(_=> completionSource.TrySetResult());
        try
        {
	        do
	        {
		        await Awaitable.NextFrameAsync(token);
	        } while (!completionSource.Awaitable.IsCompleted);
        }
        finally
        {
            completionSource.TrySetCanceled();
        }
    }
    
    public static void LateFocus(this VisualElement element) => element.schedule.Execute(_=>element.Focus());
}
