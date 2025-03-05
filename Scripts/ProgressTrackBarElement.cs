using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ProgressTrackBarElement : VisualElement
{
	internal VisualElement progressFill;
	internal VisualElement tracker;
    
    [UxmlAttribute,Range(0,1f)] internal float progress = 0f;

    public ProgressTrackBarElement()
    {
	    
        // Main progress container
        AddToClassList("progress-container");
        
        style.overflow = Overflow.Hidden;

        // Progress fill
        progressFill = new VisualElement(){name="ProgressFill"};
        progressFill.AddToClassList("progress-fill");

        hierarchy.Add(progressFill);

        // Progress tracker
        tracker = new VisualElement(){name="Tracker"};
        tracker.AddToClassList("progress-tracker");

        hierarchy.Add(tracker);

        // Set default progress
        SetProgress(0);
    }

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);
        progressFill.style.width = Length.Percent(progress * 100);

        // Move tracker along the progress bar
        tracker.style.left = Length.Percent(progress * 100);
        tracker.style.top = Length.Percent(-50); // Keeps tracker aligned vertically
    }

    public void SetTrackerContent(VisualElement content)
    {
        tracker.Clear();
        tracker.Add(content);
    }
}