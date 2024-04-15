using UnityEngine;

namespace F2F
{
	public class AutoSOEditorConfig : ScriptableObject
	{
		[SerializeField] private string path = _defaultPath;

		private const string _defaultPath = "Assets/Resources/F2F/AutoSO";


		public string Path => path;

	}
}