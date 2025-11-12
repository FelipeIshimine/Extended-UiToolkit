using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public static class StyleTransitionAwaiter
{
    public static async Awaitable RemoveFromClassListAsync(this VisualElement element, string className, CancellationToken ct = default)
    {
        // Apply the class
        element.RemoveFromClassList(className);
        await WaitForTransition(element, ct);
    }
    
    public static async Awaitable AddToClassListAsync(this VisualElement element, string className, CancellationToken ct = default)
    {
        // Apply the class
        element.AddToClassList(className);
        await WaitForTransition(element, ct);
    }
    
    public static async Awaitable ToggleClassAsync(this VisualElement element, string className, CancellationToken ct = default)
    {
        // Apply the class
        element.ToggleInClassList(className);
        await WaitForTransition(element, ct);
    }

    private static async Awaitable WaitForTransition(VisualElement element, CancellationToken ct)
    {
        // Wait a frame to let the style system resolve
        await Awaitable.NextFrameAsync(ct);

        // Read transition durations after class has been applied
        var resolvedDurations = element.resolvedStyle.transitionDuration;
        float maxDuration = 0f;

        foreach (var dur in resolvedDurations)
            maxDuration = Mathf.Max(maxDuration, dur.value); // value is in seconds

        // If no transition, just yield one more frame
        while (maxDuration > 0)
        {
	        maxDuration -= Time.unscaledDeltaTime;
            await Awaitable.NextFrameAsync(ct);
        }
    }
}