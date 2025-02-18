using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit
{
	[UxmlElement]
	public partial class CarouselElement : VisualElement
	{
		[UxmlAttribute]
		private int currentIndex = 0;
		
		[UxmlAttribute]
		private List<Texture2D> images = new List<Texture2D>();
		
		private VisualElement container;
		private Button prevButton;
		private Button nextButton;

		public CarouselElement()
		{
			this.style.flexDirection = FlexDirection.Row;
			container = new VisualElement
			{
				name = "CarouselContainer", 
			};
			prevButton = new Button(() => ScrollTo(currentIndex - 1)) { text = "<" };
			nextButton = new Button(() => ScrollTo(currentIndex + 1)) { text = ">" };

			prevButton.AddToClassList("carousel-button");
			nextButton.AddToClassList("carousel-button");
			container.AddToClassList("carousel-container");

			Add(prevButton);
			Add(container);
			Add(nextButton);
			
			this.RegisterCallbackOnce<AttachToPanelEvent>(_=> PopulateCarousel());
		}

		public CarouselElement(List<Texture2D> textures) : this()
		{
			images = textures;
			PopulateCarousel();
		}
		
		public void SetImages(List<Texture2D> textures)
		{
			images = textures;
			PopulateCarousel();
		}

		private void PopulateCarousel()
		{
			container.Clear();
			foreach (var texture in images)
			{
				VisualElement item = new VisualElement();
				item.style.backgroundImage = new StyleBackground(texture);
				item.AddToClassList("carousel-item");
				container.Add(item);
			}
		}

		private void ScrollTo(int index)
		{
			if (index < 0 || index >= images.Count) return;
			currentIndex = index;
			float newOffset = -currentIndex * 100; // Adjust based on item width
			container.style.translate = new StyleTranslate(new Translate(newOffset, 0, 0));
		}
		
	}
}