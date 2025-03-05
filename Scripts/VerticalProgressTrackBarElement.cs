using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class VerticalProgressTrackBarElement : VisualElement
{
	private VisualElement progressFill;
	private VisualElement tracker;

	[UxmlAttribute,Range(0,1f)] private float Progress { get; set; } = .5f;
	[UxmlAttribute] private TrackerSide Side { get; set; }
	[UxmlAttribute] private Color FillColor { get; set; } = Color.gray;
	[UxmlAttribute] private Color BgColor { get; set; } = Color.green;

	public VerticalProgressTrackBarElement()
	{
		// Main progress container
		AddToClassList("progress-container");
		AddToClassList("vertical"); // Apply vertical-specific styles
        
		// Progress fill
		progressFill = new VisualElement() { name="ProgressFill" };
		progressFill.AddToClassList("progress-fill");
		progressFill.AddToClassList("vertical"); // Vertical-specific class

		hierarchy.Add(progressFill);

		// Progress tracker
		tracker = new VisualElement() { name="ProgressTracker" };
		tracker.AddToClassList("progress-tracker");
		tracker.AddToClassList("vertical"); // Vertical-specific class

		hierarchy.Add(tracker);

		// Set default progress
		SetProgress(Progress);
		
		this.RegisterCallbackOnce<AttachToPanelEvent>(_ => Init());


	}
	
	private void Init()
	{
		SetProgress(Progress);
	
		progressFill.style.unityBackgroundImageTintColor = FillColor;
		this.style.unityBackgroundImageTintColor = BgColor;
		
		tracker.RemoveFromClassList("left");
		tracker.RemoveFromClassList("right");

		switch (Side)
		{
			case TrackerSide.Center:
				break;
			case TrackerSide.Left:
				tracker.AddToClassList("left");
				break;
			case TrackerSide.Right:
				tracker.AddToClassList("right");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		/*if (!Application.isPlaying && Application.isEditor)
		{
			this.schedule.Execute(Init).Every(100);
		}*/
		
	}

	public void SetProgress(float value)
	{
		Progress = Mathf.Clamp01(value);
		progressFill.style.height = Length.Percent(Progress * 100); // Fill grows upwards

		// Move tracker along Y-axis
		tracker.style.top = Length.Percent(100 - (Progress * 100)); // Inverted because 100% = bottom
	}

	public void SetTrackerContent(VisualElement content)
	{
		tracker.Clear();
		tracker.Add(content);
	}
	
}

[System.Serializable]
public enum TrackerSide
{
	Center,
	Left,
	Right
}