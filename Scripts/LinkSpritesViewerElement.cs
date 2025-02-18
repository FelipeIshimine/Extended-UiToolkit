using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit
{
	[UxmlElement]
	public partial class LinkSpritesViewerElement : VisualElement
	{
		[UxmlAttribute]
		public BackgroundSizeType imageSizeType { get; set; } = BackgroundSizeType.Contain;

		public List<SpriteInfo> sprites { get; set; }= new();
	
		private int currentIndex = 0;
		private Button centerButton;
		private Image buttonImage;
		private Button nextButton;
		private Button prevButton;
    
		[UxmlAttribute]
		public bool timerEnabled { get; set; }= false;
    
		[UxmlAttribute]
		public float timerDuration { get; set; }= 0f; // 0 means no timer
		private float currentTimer = 0f;
    
		[UxmlAttribute]
		public bool looping { get; set; }= true; // Option for looping

		public LinkSpritesViewerElement()
		{
			this.style.flexDirection = FlexDirection.Row;
			this.RegisterCallbackOnce<AttachToPanelEvent>(_=>Init());
		}

		private void Init()
		{
			
			style.alignContent = Align.Stretch;
			style.alignSelf = Align.Stretch;
			style.justifyContent = Justify.SpaceBetween;
   
			this.schedule.Execute(Update).Every(100);
	    
			prevButton = new Button(PrevImage)
			{
				text = "\u25c0",
				style = { alignSelf = Align.Center}
			};
			this.Add(prevButton);

			// Initialize the image element
			centerButton = new Button()
			{
				text = string.Empty,
				style =
				{
					alignSelf = Align.Stretch,
					flexGrow = 1,
				}
			};

			buttonImage = new Image
			{
				style=
				{
					flexGrow = 1,
					backgroundSize = new StyleBackgroundSize(new BackgroundSize(imageSizeType)),
					alignSelf = Align.Stretch,
				}
			};
			buttonImage.pickingMode = PickingMode.Ignore;
			centerButton.Add(buttonImage);
			centerButton.style.marginBottom =
				centerButton.style.marginLeft =
					centerButton.style.marginRight =
						centerButton.style.marginTop = 4;
			centerButton.clicked += ()=> OnClick(buttonImage);
			this.Add(centerButton);

			// Initialize buttons
			nextButton = new Button(NextImage)
			{
				text = "\u25b6",
				style = { alignSelf = Align.Center}
			};
        
			this.Add(nextButton);
	    
			UpdateImage();
		}

		public LinkSpritesViewerElement(List<SpriteInfo> sprites, float timerDuration = 0f, bool looping = true) : this()
		{
			this.sprites = sprites;
			this.timerDuration = timerDuration;
			this.looping = looping;
			timerEnabled = timerDuration > 0f;
		}

		// Update the image display based on the current index
		private void UpdateImage()
		{
			if (sprites.Count > 0)
			{
				SpriteInfo info = sprites[currentIndex];
				buttonImage.userData = info;
				buttonImage.style.backgroundImage = new StyleBackground(info.sprite);
			}
		}

		private void OnClick(Image image)
		{
			var info = (SpriteInfo)image.userData;
			if (!string.IsNullOrEmpty(info.link))
			{
				Application.OpenURL(info.link);
			}
		}

		// Handle Next button press
		private void NextImage()
		{
			currentIndex++;
			if (currentIndex >= sprites.Count)
			{
				currentIndex = looping ? 0 : sprites.Count - 1;
			}
			UpdateImage();
			ResetTimer();
		}

		// Handle Prev button press
		private void PrevImage()
		{
			currentIndex--;
			if (currentIndex < 0)
			{
				currentIndex = looping ? sprites.Count - 1 : 0;
			}
			UpdateImage();
			ResetTimer();
		}

		// Reset the timer when a button is pressed
		private void ResetTimer()
		{
			if (timerEnabled)
			{
				currentTimer = timerDuration;
			}
		}

		// Update the timer every frame (optional)
		public void Update(TimerState timerState)
		{
			if (timerEnabled)
			{
				currentTimer -= timerState.deltaTime / 1000f;
				if (currentTimer <= 0)
				{
					NextImage(); // Automatically go to the next image when the timer runs out
				}
			}
		}

		// Start the timer when this element is initialized
		public void StartTimer()
		{
			currentTimer = timerDuration;
		}

		// Method to stop the timer
		public void StopTimer()
		{
			currentTimer = 0;
		}

		public void SetSprites(List<SpriteInfo> list)
		{
			currentIndex = 0;
			sprites.Clear();
			sprites.AddRange(list);
			UpdateImage();
		}
	}

	[System.Serializable]
	public class SpriteInfo
	{
		public Sprite sprite;
		public string link;
	}
	
}