using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit
{
	[UxmlElement]
    public partial class AutocompleteTextField : VisualElement
    {
        private TextField textField;
        public TextField TextField => textField;
        private ScrollView suggestionsContainer;
     
        [UxmlAttribute]
        private List<string> suggestionsList = new List<string>();
        private List<string> currentSuggestions = new List<string>();
      
        [UxmlAttribute]
        private int maxSuggestionsToShow = 5;
        private int selectedSuggestionIndex = -1;

        private bool hasFocus;

        public AutocompleteTextField()
        {
	        this.style.flexGrow = 1;
            // Create the text field
            textField = new TextField();
            textField.RegisterValueChangedCallback(OnTextChanged);
            textField.isDelayed = false;
            Add(textField);

            // Create the scrollable container for the suggestions
            suggestionsContainer = new ScrollView()
            {
                name = "Suggestions",
                mode = ScrollViewMode.Vertical,
                style =
                {
                    flexGrow = 1,
                    backgroundColor = new Color(.14f,.14f,.14f,.95f)
                }
            };
            suggestionsContainer.style.borderBottomLeftRadius =
                suggestionsContainer.style.borderBottomRightRadius =
                    suggestionsContainer.style.borderTopLeftRadius =
                        suggestionsContainer.style.borderTopRightRadius = 4;
            suggestionsContainer.style.paddingBottom = 1;
            suggestionsContainer.style.display = DisplayStyle.None; // Hidden by default
            Add(suggestionsContainer);

            // Handle keyboard navigation
            this.RegisterCallback<KeyDownEvent>(OnKeyDown,TrickleDown.TrickleDown);

            // Focus events
            this.RegisterCallback<FocusOutEvent>(OnFocusOut);
            this.RegisterCallback<FocusInEvent>(OnFocusIn);
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
	        hasFocus = false;
            // Hide the suggestions after a short delay to register click events
            schedule.Execute(DisableSuggestions).ExecuteLater(100);
        }

        private void OnFocusIn(FocusInEvent evt)
        {
	        hasFocus = true;
            suggestionsContainer.style.display = DisplayStyle.None;
            UpdateSuggestions(textField.value);
        }

        // Set the available suggestions list
        public void SetSuggestions(IEnumerable<string> suggestions)
        {
            suggestionsList.Clear();
            suggestionsList.AddRange(suggestions);
            suggestionsList.Sort();
        }

        // Text changed event handler
        private void OnTextChanged(ChangeEvent<string> evt)
        {
	        if (!hasFocus)
	        {
		        return;
	        }
            string searchText = evt.newValue.ToLower();
            UpdateSuggestions(searchText);
        }

        // Update suggestion list based on input
        private void UpdateSuggestions(string searchText)
        {
            suggestionsContainer.Clear();
            currentSuggestions.Clear();
            selectedSuggestionIndex = -1;

            if (!string.IsNullOrEmpty(searchText))
            {
                currentSuggestions.AddRange(suggestionsList
                                         .Where(s => s.ToLower().Contains(searchText))
                                         .Take(maxSuggestionsToShow));
            }
            else
            {
                currentSuggestions.AddRange(suggestionsList.Take(maxSuggestionsToShow));
            }

            if (currentSuggestions.Count > 0)
            {
                Add(suggestionsContainer);
                suggestionsContainer.style.display = DisplayStyle.Flex;

                var root = FindRootVisualContainer();
                
                root.Add(suggestionsContainer);

                var textInput = textField.Q<VisualElement>("unity-text-input");
                
                // Convert world position to local UI position
                Vector2 localPosition = root.WorldToLocal(new Vector2(textInput.worldBound.x, textInput.worldBound.yMax));
                
                // Apply absolute positioning
                suggestionsContainer.style.position = Position.Absolute;
                suggestionsContainer.style.left = localPosition.x;
                suggestionsContainer.style.top = localPosition.y;
                suggestionsContainer.style.width = textInput.worldBound.width;
            
                    
                foreach (var suggestion in currentSuggestions)
                {
                    var label = new Label(HighlightMatch(suggestion, searchText));
                    label.RegisterCallback<ClickEvent>(_ =>
                    {
                        OnSuggestionClicked(suggestion);
                    });
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    
                    label.style.marginLeft = label.style.marginRight = 6;
                    label.style.paddingTop = 2;
                    
                    suggestionsContainer.Add(label);
                }
            }
            else
            {
                DisableSuggestions();
            }
        }

        private VisualElement FindRootVisualContainer()
        {
            VisualElement current = this;
            while (current.viewDataKey != "rootVisualContainer")
            {
                current = current.parent;
            }
            return current;
        }

        private void DisableSuggestions()
        {
            //this.Add(suggestionsContainer);
            suggestionsContainer.style.display = DisplayStyle.None;
        }

        // Highlight the matching part of the suggestion
        private string HighlightMatch(string suggestion, string searchText)
        {
            int matchIndex = suggestion.ToLower().IndexOf(searchText);
            if (matchIndex >= 0)
            {
                string beforeMatch = suggestion.Substring(0, matchIndex);
                string matchText = suggestion.Substring(matchIndex, searchText.Length);
                string afterMatch = suggestion.Substring(matchIndex + searchText.Length);
                return $"{beforeMatch}<b>{matchText}</b>{afterMatch}";
            }
            return suggestion;
        }

        private void OnSuggestionClicked(string suggestion)
        {
            textField.value = suggestion;
            DisableSuggestions();

        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (suggestionsContainer.style.display == DisplayStyle.None)
                return;

            if (evt.keyCode == KeyCode.DownArrow)
            {
                NavigateSuggestions(1);
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.UpArrow)
            {
                NavigateSuggestions(-1);
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                if (selectedSuggestionIndex >= 0 && selectedSuggestionIndex < currentSuggestions.Count)
                {
                    SetSelectedSuggestion(selectedSuggestionIndex);
                    evt.StopImmediatePropagation();
                }
            }
        }

        private void NavigateSuggestions(int direction)
        {
            if (currentSuggestions.Count == 0) return;

            selectedSuggestionIndex += direction;

            if (selectedSuggestionIndex >= currentSuggestions.Count)
            {
                selectedSuggestionIndex = 0;
            }
            else if (selectedSuggestionIndex < 0)
            {
                selectedSuggestionIndex = currentSuggestions.Count - 1;
            }

            HighlightSuggestion(selectedSuggestionIndex);
        }

        private void HighlightSuggestion(int index)
        {
            for (int i = 0; i < suggestionsContainer.childCount; i++)
            {
                var child = suggestionsContainer[i];
                if (i == index)
                {
                    child.style.backgroundColor = new StyleColor(HighlightColor); // Highlight
                }
                else
                {
                    child.style.backgroundColor = new StyleColor(Color.clear);
                }
            }
        }

        private static readonly Color HighlightColor = new Color(0,.6f,1f,.4f);

        private void SetSelectedSuggestion(int index)
        {
            textField.value = currentSuggestions[index];
            DisableSuggestions();
            textField.schedule.Execute(textField.SelectAll).ExecuteLater(1);
        }
    }
}

