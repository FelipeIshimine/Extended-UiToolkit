using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Extension methods for awaiting UI Toolkit CSS transitions as Unity Awaitables.
///
/// Design notes:
///   TransitionEndEvent is marked experimental/unreliable in Unity UIToolkit —
///   using it as the primary resolution mechanism causes stalls when it doesn't fire.
///   Instead we read maxDuration from resolvedStyle and await WaitForSecondsAsync,
///   which is deterministic and runs on Unity's timer independent of element lifecycle.
///
///   DetachFromPanelEvent is kept as an early-exit safety valve: if the element is
///   destroyed before the timer expires we resolve immediately rather than hanging.
/// </summary>
public static class StyleTransitionAwaiter
{
    // -----------------------------------------------------------------------
    // Core wait — call AFTER you've already mutated the style/class
    // -----------------------------------------------------------------------

    /// <summary>
    /// Waits for all CSS transitions on the element to complete.
    /// Resolves immediately if no transitions are defined (zero-duration fast path).
    /// Resolves early if the element is detached before the timer expires.
    /// </summary>
    public static async Awaitable WaitForTransitionAsync(
        this VisualElement element, CancellationToken ct = default)
    {
        // One frame so the style system resolves transition durations
        await Awaitable.NextFrameAsync(ct);

        float maxDuration = 0f;
        foreach (var dur in element.resolvedStyle.transitionDuration)
            if (dur.value > maxDuration) maxDuration = dur.value;

        // No transitions defined — done
        if (maxDuration <= 0f) return;

        var acs = new AwaitableCompletionSource();

        // Safety valve: resolve immediately if element is removed from the hierarchy
        EventCallback<DetachFromPanelEvent> onDetach = null;
        onDetach = _ =>
        {
            element.UnregisterCallback(onDetach);
            acs.TrySetResult();
        };
        element.RegisterCallback(onDetach);

        // Primary path: wait the actual transition duration (reliable, timer-based)
        // Run concurrently as async void — safe here because it only calls TrySetResult
        // which is idempotent and cannot throw
        WaitThenComplete(acs, maxDuration, ct);

        await acs.Awaitable;
        element.UnregisterCallback(onDetach);
    }

    private static async void WaitThenComplete(
        AwaitableCompletionSource acs, float seconds, CancellationToken ct)
    {
        try { await Awaitable.WaitForSecondsAsync(seconds, ct); }
        catch { /* cancellation — detach already resolved or caller cancelled */ }
        acs.TrySetResult();
    }

    // -----------------------------------------------------------------------
    // Apply + wait convenience methods
    // -----------------------------------------------------------------------

    /// <summary>Adds a USS class then awaits its transitions.</summary>
    public static async Awaitable AddToClassListAndWaitAsync(
        this VisualElement element, string className, CancellationToken ct = default)
    {
        element.AddToClassList(className);
        await element.WaitForTransitionAsync(ct);
    }

    /// <summary>Removes a USS class then awaits its transitions.</summary>
    public static async Awaitable RemoveFromClassListAndWaitAsync(
        this VisualElement element, string className, CancellationToken ct = default)
    {
        element.RemoveFromClassList(className);
        await element.WaitForTransitionAsync(ct);
    }

    /// <summary>Toggles a USS class then awaits its transitions.</summary>
    public static async Awaitable ToggleInClassListAndWaitAsync(
        this VisualElement element, string className, CancellationToken ct = default)
    {
        element.ToggleInClassList(className);
        await element.WaitForTransitionAsync(ct);
    }
}