using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AdvancedFillBarElement : VisualElement
{
	private readonly VisualElement lostFill;
	private readonly VisualElement currentFill;
	private readonly VisualElement gainFill;
	private readonly VisualElement overlay;

	private readonly List<VisualElement> subdivisionElements = new List<VisualElement>();

	private float current = 50;
	private float gain = 25;
	private float lost = 25;

	private bool useSubdivisions = true;
	private float subdivisionsSeparation = 50;

	public bool UseSubdivisions
	{
		get => useSubdivisions;
		set
		{
			useSubdivisions = value; 
			for (int i = 0; i < subdivisionElements.Count; i++)
			{
				subdivisionElements[i].style.display = useSubdivisions? DisplayStyle.Flex: DisplayStyle.None;
			}          
		}
	}

	public float SubdivisionsSeparation
	{
		get => subdivisionsSeparation;
		set
		{
			subdivisionsSeparation = Mathf.Max(value,1);
            RefreshSubDivisions();
		}
	}

	public float Current
	{
		get => current;
		set
		{
			current = value;
			
			currentFill.style.width = new StyleLength(Length.Percent(value));
            
			RefreshLostFill();
			RefreshGainFill();
			
		}
	}

	public float Gain
	{
		get => gain;
		set
		{
			gain = value;
			RefreshGainFill();
		}
	}

	public float Lost
	{
		get => lost;
		set
		{
			lost = value;
			RefreshLostFill();
		}
	}

	public AdvancedFillBarElement()
	{
		this.style.overflow = Overflow.Hidden;
		VisualElement bg = new VisualElement()
		{
			name = "bg",
			style = {position = Position.Absolute, left = 0, right = 0, bottom = 0, top = 0}
		};
		bg.AddToClassList("advanced-bar-bg");
		Add(bg);

		lostFill = new VisualElement()
		{
			name = "lost-fill",
			style = { position = Position.Absolute, left = 0, top = 0, bottom = 0}
		};
		lostFill.AddToClassList("advanced-bar-lost-fill");
		Add(lostFill);

		currentFill = new VisualElement()
		{
			name = "current-fill",
			style = { position = Position.Absolute, left = 0, top = 0, bottom = 0}
		};
		currentFill.AddToClassList("advanced-bar-current-fill");
		Add(currentFill);
		
		gainFill = new VisualElement()
		{
			name = "gain-fill",
			style = { position = Position.Absolute, top = 0, bottom = 0}
		};
		gainFill.AddToClassList("advanced-bar-gain-fill");
		Add(gainFill);
		
		overlay = new VisualElement()
		{
			name = "Overlay",
			style = { position = Position.Absolute, top = 0, bottom = 0, left = 0, right = 0}
		};
		overlay.AddToClassList("advanced-bar-overlay");
		Add(overlay);

		Current = 5f;
		Lost = 25f;
		Gain = 25f;
		SubdivisionsSeparation = 50;
		UseSubdivisions = true;
	}

	private void RefreshGainFill()
	{
		gainFill.style.left = new StyleLength(Length.Percent(current-gain));
		gainFill.style.width = new StyleLength(Length.Percent(gain));
	}

	private void RefreshLostFill()
	{
		lostFill.style.width = new StyleLength(Length.Percent(current + lost));
	}

	private void RefreshSubDivisions()
	{
		if (!useSubdivisions || subdivisionsSeparation == 0)
		{
			for (int i = subdivisionElements.Count - 1; i >= 0; i--)
			{
				subdivisionElements[i].RemoveFromHierarchy();
			}
			subdivisionElements.Clear();
			return;
		}
		float count = (100f / subdivisionsSeparation) - .01f;

		int ceilCount = (int)Mathf.Ceil(count);
		if (ceilCount != subdivisionElements.Count)
		{
			//Remove Excess
			for (int i = subdivisionElements.Count; i > ceilCount; i--)
			{
				subdivisionElements[^1].RemoveFromHierarchy();
				subdivisionElements.RemoveAt(subdivisionElements.Count-1);
			}

			for (int i = subdivisionElements.Count; i < ceilCount; i++)
			{
				VisualElement division = new VisualElement()
				{
					name = $"division {i}"
				};
				division.AddToClassList("advanced-bar-subdivision");
				subdivisionElements.Add(division);
				this.Add(division);
			}
		}

		float offset = subdivisionsSeparation;
		for (int i = 0; i < subdivisionElements.Count; i++)
		{
			var subdivision = subdivisionElements[i];
			float width = subdivision.style.width.value.value;
			subdivision.style.position = Position.Absolute;
			subdivision.style.left = new StyleLength(Length.Percent(offset - width/2f));
			subdivision.style.right = new StyleLength(Length.Percent(offset + width/2f));
			subdivision.style.top = subdivision.style.bottom = 0;
			subdivision.style.display = useSubdivisions ? DisplayStyle.Flex : DisplayStyle.None;
			offset += subdivisionsSeparation;
		}
		
		Remove(overlay);
		Add(overlay);
	}

	public new class UxmlFactory : UxmlFactory<AdvancedFillBarElement,UxmlTraits> {}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private readonly UxmlFloatAttributeDescription current = new UxmlFloatAttributeDescription { name = "current" };
		private readonly UxmlFloatAttributeDescription lost = new UxmlFloatAttributeDescription { name = "lost" };
		private readonly UxmlFloatAttributeDescription gain = new UxmlFloatAttributeDescription { name = "gain" };

		private readonly UxmlBoolAttributeDescription useSubdivision = new UxmlBoolAttributeDescription { name = "useSubdivisions" };
		private readonly UxmlFloatAttributeDescription subdivisionSeparation = new UxmlFloatAttributeDescription { name = "subdivisionsSeparation" };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			
			var healthBarElement = (AdvancedFillBarElement) ve;
			healthBarElement.Current = current.GetValueFromBag(bag,cc);
			healthBarElement.Lost = lost.GetValueFromBag(bag,cc);
			healthBarElement.Gain = gain.GetValueFromBag(bag,cc);
			healthBarElement.UseSubdivisions = useSubdivision.GetValueFromBag(bag,cc);
			healthBarElement.SubdivisionsSeparation = subdivisionSeparation.GetValueFromBag(bag,cc);
		}
	}
}