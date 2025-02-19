using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit
{
	[UxmlElement]
	public partial class LinkSpritesViewerElement : VisualElement
	{
		public const string NAME = "SritesViewer";
			
		[UxmlAttribute]
		public BackgroundSizeType imageSizeType { get; set; } = BackgroundSizeType.Contain;

		private List<SpriteInfo> sprites { get; }= new();
	
		private int currentIndex = 0;
		private Image buttonImage;
		
		private Button centerButton;
		private Button nextButton;
		private Button prevButton;
    
		[UxmlAttribute]
		public bool timerEnabled { get; set; }= false;
    
		[UxmlAttribute]
		public float timerDuration { get; set; }= 0f; // 0 means no timer
		private float currentTimer = 0f;
		private VisualElement dotsContainer;

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
				name = $"{NAME}Prev",
				text = "\u25c0",
				style = { alignSelf = Align.Center}
			};
			this.Add(prevButton);

			VisualElement centerContainer = new VisualElement()
			{
				style =
				{
					flexGrow = 1,
				}
			};
			
			// Initialize the image element
			centerButton = new Button()
			{
				name = $"{NAME}Center",
				text = string.Empty,
				style =
				{
					alignSelf = Align.Stretch,
					flexGrow = 1,
				}
			};

			buttonImage = new Image
			{
				name = $"{NAME}Image",
				style =
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
			
			centerContainer.Add(centerButton);

			CreateImagesDots();
			
			centerContainer.Add(CreateImagesDots());
			
			this.Add(centerContainer);

			// Initialize buttons
			nextButton = new Button(NextImage)
			{
				name = $"{NAME}Next",
				text = "\u25b6",
				style = { alignSelf = Align.Center}
			};
        
			this.Add(nextButton);
	    
			SetIndex(0);
		}

		private VisualElement CreateImagesDots()
		{
			dotsContainer = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row,
					position = Position.Absolute,
					alignSelf = Align.Stretch,
					justifyContent = Justify.SpaceEvenly,
					alignContent = Align.Stretch,
					height = 32,
					left = 0,
					right = 0,
					bottom = 0,
					translate = new StyleTranslate(new Translate(Length.None(), Length.Percent(100)))
				}
			};
			return dotsContainer;
		}

		public LinkSpritesViewerElement(List<SpriteInfo> sprites, float timerDuration = 0f) : this()
		{
			this.sprites = sprites;
			this.timerDuration = timerDuration;
			timerEnabled = timerDuration > 0f;
		}

		// Update the image display based on the current index
		private void SetIndex(int nextIndex)
		{
			if (currentIndex != -1)
			{
				if (dotsContainer.childCount > currentIndex)
				{
					dotsContainer.ElementAt(currentIndex).SetEnabled(false);
				}
			}
			
			currentIndex = nextIndex;

			if (sprites.Count > 0)
			{
				SpriteInfo info = sprites[currentIndex];
				buttonImage.userData = info;
				buttonImage.style.backgroundImage = new StyleBackground(info.sprite);
				dotsContainer.ElementAt(currentIndex).SetEnabled(true);
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
			if (sprites.Count == 0)
			{
				return;
			}
			SetIndex((currentIndex + 1) % sprites.Count);
			ResetTimer();
		}

		// Handle Prev button press
		private void PrevImage()
		{
			if (sprites.Count == 0)
			{
				return;
			}
			SetIndex((currentIndex - 1 + sprites.Count) % sprites.Count);
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

			RefreshDots();
			SetIndex(0);
		}

		private void RefreshDots()
		{
			dotsContainer.Clear();

			for (int i = 0; i < sprites.Count; i++)
			{
				var dot = new VisualElement()
				{
					style=
					{
						flexGrow = 1,
						maxHeight = 24,
						maxWidth = 24,
					}
				};

				dot.style.borderBottomLeftRadius =
					dot.style.borderBottomRightRadius =
						dot.style.borderTopLeftRadius =
							dot.style.borderTopRightRadius = 24;
				dot.style.backgroundColor = Color.white;
				
				dot.SetEnabled(false);
				dotsContainer.Add(dot);
			}
		}
	}

	[System.Serializable]
	public class SpriteInfo
	{
		public Sprite sprite;
		public string link;
	}
	
}