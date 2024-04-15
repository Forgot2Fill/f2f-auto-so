using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace F2F
{
	[CustomPropertyDrawer(typeof(SOGroupID))]
	public class SOGroupIDDrawer : PropertyDrawer
	{
		private readonly List<KeyValuePair<int, string>> _pairs = new List<KeyValuePair<int, string>>();
		
		private DropdownField _dropdownField = new DropdownField();

		private SerializedProperty _property;
		private SOGroupID _value = new SOGroupID();
		
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			_property = property;
			_value = (SOGroupID)property.boxedValue;
			
			_dropdownField = new DropdownField(preferredLabel);
			_dropdownField.RegisterValueChangedCallback(OnDropdownChanged);
			
			RefreshDropdownList();
			
			_dropdownField.value = ValueToString(_value);
			
			return _dropdownField;
		}

		private void OnDropdownChanged(ChangeEvent<string> v)
		{
			_value = StringToValue(v.newValue);
			_property.boxedValue = _value;
			_property.serializedObject.ApplyModifiedProperties();
		}

		private void RefreshDropdownList()
		{
			_pairs.Clear();
			_pairs.Add(new KeyValuePair<int, string>(SOGroupID.Invalid, "INVALID"));
			
			var config = AutoSOEditorInternal.Config;

			var nGroups = config.Groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = config.Groups[iGroup];

				if (groupInfo.Default == null)
				{
					continue;
				}
				
				_pairs.Add(new KeyValuePair<int, string>(groupInfo.ID, groupInfo.Default.GetType().FullName));
			}

			_dropdownField.choices = _pairs.Select(v => v.Value).ToList();
		}

		private string ValueToString(SOGroupID v)
		{
			foreach (var pair in _pairs)
			{
				if (pair.Key == v)
				{
					return pair.Value;
				}
			}

			return "INVALID";
		}

		private SOGroupID StringToValue(string s)
		{
			foreach (var pair in _pairs)
			{
				if (pair.Value == s)
				{
					return pair.Key;
				}
			}

			return SOGroupID.Invalid;
		}
	}
}