using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
	public static async Awaitable WaitForPressDown(this VisualElement element, CancellationToken token)
	{
		AwaitableCompletionSource completionSource = new AwaitableCompletionSource();
		element.RegisterCallbackOnce<PointerDownEvent>(_ => completionSource.TrySetResult());
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

	public static void LateFocus(this VisualElement element) => element.schedule.Execute(_ => element.Focus());

	/*public static Awaitable WaitForTransition(this VisualElement element,
	                                          float fallbackDuration = 0.5f,
	                                          CancellationToken cancellationToken = default)
	{
		var completionSource = new AwaitableCompletionSource();

		// Fallback in case transitionend is unreliable
		var fallbackTimer = element.schedule.Execute(() =>
		{
			if (!completionSource.Awaitable.IsCompleted)
			{
				completionSource.TrySetResult();
			}
		}).StartingIn((long)(fallbackDuration * 1000));

		// Experimental event for transitionend (not officially supported in all builds)
		EventCallback<TransitionEndEvent> callback = null;
		callback = (e) =>
		{
			if (e.target == element)
			{
				element.UnregisterCallback(callback);
				fallbackTimer.Pause();
				if (!completionSource.IsCompleted)
					completionSource.Complete();
			}
		};

		element.RegisterCallback(callback);

		// Cancellation support
		if (cancellationToken != default)
		{
			cancellationToken.Register(() =>
			{
				element.UnregisterCallback(callback);
				fallbackTimer.Pause();
				if (!completionSource.IsCompleted)
					completionSource.Complete(); // or CompleteExceptionally, based on your needs
			});
		}

		return completionSource;
	}*/

}
