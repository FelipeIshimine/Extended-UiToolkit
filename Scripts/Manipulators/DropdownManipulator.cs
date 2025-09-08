using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Manipulators
{
	public class DropdownManipulator : Manipulator
	{
		public event Action<int> OnSelection;
		private DropdownField dropdown;
		private readonly string[] options;

		public DropdownManipulator(Action<int> onSelection, IEnumerable<string> options)
		{
			this.options = options.ToArray();
			OnSelection = onSelection;
		}

		private void Selection(ChangeEvent<string> evt)
		{
			var index = dropdown.choices.IndexOf(evt.newValue);
			if (index >= 0)
			{
				OnSelection?.Invoke(index);
			}
		}

		protected override void RegisterCallbacksOnTarget()
		{
			this.dropdown = (DropdownField)target;
			dropdown.choices = new List<string>(this.options);
			dropdown.value = this.options[0];
			dropdown.RegisterValueChangedCallback(Selection);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			dropdown.UnregisterValueChangedCallback(Selection);
		}

		public void SetValue(int index) => dropdown.value = this.options[index];
	}
}