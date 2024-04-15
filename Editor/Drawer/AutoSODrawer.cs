using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace F2F
{
	[CustomPropertyDrawer(typeof(AutoSOAttribute))]
	public class AutoSODrawer : PropertyDrawer
	{
		private VisualElement _root;

		private VisualElement _errorRoot;
		private VisualElement _successRoot;
		private VisualElement _createRoot;
		private VisualElement _soRoot;
        
		private DropdownField _keyDropdown;
		private TextField _keyField;

		private Editor _soEditor;
		private VisualElement _soElement;
		
		private ObjectField _defaultField;

		private SerializedProperty _property;
		
		private SOGroupID _groupID;
		private SOKey _key;
		
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			_root = new VisualElement();
			_groupID = AutoSOEditor.Register(fieldInfo.FieldType);
			_property = property;
			
			if (AutoSOEditor.Has((ScriptableObject)property.objectReferenceValue, out var key))
			{
				_key = key;
			}
			else
			{
				_key = SOKey.Default;
				property.objectReferenceValue = AutoSOEditor.Get(fieldInfo.FieldType, _key);
				property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}

			CreateVisualElements();
			
			RefreshDropdownList();
			Refresh();
			
			return _root;
		}

		private void CreateVisualElements()
		{
			_errorRoot = new VisualElement();
			
			_successRoot = new VisualElement();
			
			var foldout = new Foldout();

			_keyDropdown = new DropdownField { value = _key };
			_keyDropdown.RegisterValueChangedCallback(OnKeyDropdownChanged);

			_keyDropdown.style.width = new StyleLength(new Length(60f, LengthUnit.Percent));

			foldout.hierarchy[0].Add(_keyDropdown);
			
			foldout.text = preferredLabel;

			var box = new Box();
			foldout.Add(box);

			_keyField = new TextField { value = _key };
			_keyField.RegisterValueChangedCallback(OnKeyFieldChanged);
			
			box.Add(_keyField);
			
			box.Add(new VisualElement
			{
				style = { height = new StyleLength(15) }
			});

			_soRoot = new Box();
			
			box.Add(_soRoot);
			
			_successRoot.Add(foldout);

			_createRoot = new Button(OnCreateBtnClicked)
			{
				text = "Create"
			};
		}

		private void OnKeyFieldChanged(ChangeEvent<string> e)
		{
			_key = e.newValue;
			Refresh();
		}

		private void OnKeyDropdownChanged(ChangeEvent<string> e)
		{
			_keyField.value = e.newValue;
			_key = e.newValue;
			Refresh();
		}

		private void OnCreateBtnClicked()
		{
			AutoSOEditor.Create(_groupID, _key);
			
			RefreshDropdownList();
			Refresh();
		}
		
		private void Refresh()
		{
			_root.Clear();
			
			if (_groupID == SOGroupID.Invalid)
			{
				_root.Add(_errorRoot);
				return;
			}

			_root.Add(_successRoot);
			
			_soRoot.Clear();
			
			if (AutoSOEditor.Has(_groupID, _key))
			{
				_keyDropdown.value = _key;
				
				var so = AutoSOEditor.Get(_groupID, _key);

				if (_property.objectReferenceValue != so)
				{
					_property.objectReferenceValue = so;
					_property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				}
				
				Editor.CreateCachedEditor(_property.objectReferenceValue, null, ref _soEditor);
				if (_soEditor != null)
				{
					var soInspector = new InspectorElement(_soEditor);
					soInspector.Children().Last().RemoveAt(0);
				
					_soRoot.Add(soInspector.Children().Last());
				}
			}
			else
			{
				_soRoot.Add(_createRoot);
			}
		}

		private void RefreshDropdownList()
		{
			var keys = AutoSOEditor.GetKeys(_groupID);

			_keyDropdown.choices = keys.Select(key => key.ToString()).ToList();
		}
	}
}