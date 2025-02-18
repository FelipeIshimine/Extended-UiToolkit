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
                mode = ScrollViewMode.Vertical
            };
            suggestionsContainer.style.display = DisplayStyle.None; // Hidden by default
            Add(suggestionsContainer);

            // Handle keyboard navigation
            textField.RegisterCallback<KeyDownEvent>(OnKeyDown);

            // Focus events
            this.RegisterCallback<FocusOutEvent>(OnFocusOut);
            this.RegisterCallback<FocusInEvent>(OnFocusIn);
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
	        hasFocus = false;
            // Hide the suggestions after a short delay to register click events
            schedule.Execute(() => suggestionsContainer.style.display = DisplayStyle.None).ExecuteLater(100);
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
                suggestionsContainer.style.display = DisplayStyle.Flex;

                foreach (var suggestion in currentSuggestions)
                {
                    var label = new Label(HighlightMatch(suggestion, searchText));
                    label.RegisterCallback<ClickEvent>(evt => OnSuggestionClicked(suggestion));
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    suggestionsContainer.Add(label);
                }
            }
            else
            {
                suggestionsContainer.style.display = DisplayStyle.None;
            }
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
            suggestionsContainer.style.display = DisplayStyle.None;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (suggestionsContainer.style.display == DisplayStyle.None)
                return;

            if (evt.keyCode == KeyCode.DownArrow)
            {
                NavigateSuggestions(1);
            }
            else if (evt.keyCode == KeyCode.UpArrow)
            {
                NavigateSuggestions(-1);
            }
            else if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                if (selectedSuggestionIndex >= 0 && selectedSuggestionIndex < currentSuggestions.Count)
                {
                    SetSelectedSuggestion(selectedSuggestionIndex);
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
                    child.style.backgroundColor = new StyleColor(Color.gray); // Highlight
                }
                else
                {
                    child.style.backgroundColor = new StyleColor(Color.clear);
                }
            }
        }

        private void SetSelectedSuggestion(int index)
        {
            textField.value = currentSuggestions[index];
            suggestionsContainer.style.display = DisplayStyle.None;
        }
      
    }
}
