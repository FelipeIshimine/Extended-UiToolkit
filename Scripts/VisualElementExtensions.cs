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
            await completionSource.Awaitable;
        }
        finally
        {
            completionSource.TrySetCanceled();
        }
    }
    
}