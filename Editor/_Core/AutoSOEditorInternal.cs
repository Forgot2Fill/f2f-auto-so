using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace F2F
{
	internal static class AutoSOEditorInternal
	{
		public static SOGroupID Register(Type soType)
		{
			if (!AutoSOInternal.IsValid(soType))
			{
				return SOGroupID.Invalid;
			}
			
			var groups = Config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.Default == null)
				{
					continue;
				}

				if (groupInfo.Default.GetType() == soType)
				{
					return groupInfo.ID;
				}
			}
			
			var newGroupID = GetNewGroupID();
			
			groups.Add(new SOGroupInfo
			{
				ID = newGroupID,
				Default = GetOrCreateSO(soType, SOKey.Default)
			});

			return newGroupID;
		}
		
		public static ScriptableObject Create(SOGroupID groupID, SOKey key)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return null;
			}
			
			AutoSOEditorFixer.CheckoutConfigNullReferences();
			
			var groups = Config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID != groupID ||
				    groupInfo.Default == null)
				{
					continue;
				}

				if (key.IsDefault())
				{
					return groupInfo.Default;
				}

				var soType = groupInfo.Default.GetType();

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.Key == key)
					{
						if (soInfo.SO == null)
						{
							soInfo.SO = GetOrCreateSO(soType, key);
							EditorUtility.SetDirty(Config);
						}
						
						return soInfo.SO;
					}
				}

				var so = GetOrCreateSO(soType, key);
				
				elements.Add(new SOInfo
				{
					Key = key,
					SO = so
				});
				EditorUtility.SetDirty(Config);

				return so;
			}
			
			return null;
		}

		public static bool Remove(SOGroupID groupID, SOKey key)
		{
			if (groupID == SOGroupID.Invalid || 
			    key == SOKey.Default)
			{
				return false;
			}
			
			AutoSOEditorFixer.CheckoutConfigNullReferences();

			var groups = Config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID != groupID)
				{
					continue;
				}

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.Key == key)
					{
						if (soInfo.SO != null)
						{
							AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(soInfo.SO));
						}

						elements.RemoveAt(iElement);
						EditorUtility.SetDirty(Config);

						return true;
					}
				}

				return false;
			}
			
			return false;
		}

		public static bool Remove(SOGroupID groupID)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return false;
			}
			
			AutoSOEditorFixer.CheckoutConfigNullReferences();
			
			var groups = Config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID != groupID)
				{
					continue;
				}

				if (groupInfo.Default != null)
				{
					AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(groupInfo.Default));
				}

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.SO != null)
					{
						AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(soInfo.SO));
					}
				}

				groups.RemoveAt(iGroup);
				EditorUtility.SetDirty(Config);

				return true;
			}

			return false;
		}
		
		public static int GetNewGroupID()
		{
			var groups = Config.Groups;
			var nGroups = groups.Count;

			var groupID = 1;
			
			while (groupID <= nGroups)
			{
				var hasGroupID = false;
				
				for (var iGroup = 0; iGroup < nGroups; iGroup++)
				{
					var groupInfo = groups[iGroup];
					if (groupInfo.ID == groupID)
					{
						hasGroupID = true;
						break;
					}
				}

				if (!hasGroupID)
				{
					return groupID;
				}

				++groupID;
			}

			return groupID;
		}

		public static ScriptableObject GetOrCreateSO(Type soType, SOKey key)
		{
			var soPath = GetSOPath(soType, key);
			var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);

			if (so != null &&
			    so.GetType() != soType)
			{
				AutoSOEditorFixer.CheckoutUnhandledObject(so);
				so = null;
			}
			
			if (so == null)
			{
				var soFolderPath = $"{EditorConfig.Path}/{soType.FullName}";
				var soFolders = soFolderPath.Split('/', '\\');
				var fullSoFolderPath = Path.Combine(Environment.CurrentDirectory, Path.Combine(soFolders));
				
				if (!Directory.Exists(fullSoFolderPath))
				{
					Directory.CreateDirectory(fullSoFolderPath);
					AssetDatabase.Refresh();
				}
				
				so = ScriptableObject.CreateInstance(soType);
				AssetDatabase.CreateAsset(so, soPath);
				so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);
			}

			return so;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetSOPath(Type soType, SOKey key)
		{
			return $"{EditorConfig.Path}/{soType.FullName}/{key.ToString()}.asset";
		}
		
		private static AutoSOConfig _config;
		
		public static AutoSOConfig Config
		{
			get
			{
				if (_config != null)
				{
					return _config;
				}

				_config = AssetDatabase.LoadAssetAtPath<AutoSOConfig>(ConfigPath);

				if (_config != null)
				{
					return _config;
				}

				var fullFolderPath = Path.Combine(Environment.CurrentDirectory, Path.Combine(ConfigFolderPath));
				
				if (!Directory.Exists(fullFolderPath))
				{
					Directory.CreateDirectory(fullFolderPath);
					AssetDatabase.Refresh();
				}

				_config = ScriptableObject.CreateInstance<AutoSOConfig>();
				AssetDatabase.CreateAsset(_config, ConfigPath);
				AssetDatabase.Refresh();
				_config = AssetDatabase.LoadAssetAtPath<AutoSOConfig>(ConfigPath);
				
				return _config;
			}
		}


		private static AutoSOEditorConfig _editorConfig;

		public static AutoSOEditorConfig EditorConfig
		{
			get
			{
				if (_editorConfig != null)
				{
					return _editorConfig;
				}

				_editorConfig = AssetDatabase.LoadAssetAtPath<AutoSOEditorConfig>(EditorConfigPath);

				if (_editorConfig != null)
				{
					return _editorConfig;
				}

				var fullFolderPath = Path.Combine(Environment.CurrentDirectory, Path.Combine(EditorConfigFolderPath));
				
				if (!Directory.Exists(fullFolderPath))
				{
					Directory.CreateDirectory(fullFolderPath);
					AssetDatabase.Refresh();
				}

				_editorConfig = ScriptableObject.CreateInstance<AutoSOEditorConfig>();
				AssetDatabase.CreateAsset(_editorConfig, EditorConfigPath);
				AssetDatabase.Refresh();
				_editorConfig = AssetDatabase.LoadAssetAtPath<AutoSOEditorConfig>(EditorConfigPath);
				
				return _editorConfig;
			}
		}
		
		public const string ConfigPath = "Assets/Resources/F2F/Config/auto_so.asset";
		public static readonly string[] ConfigFolderPath = new[] { "Assets", "Resources", "F2F", "Config" };

		public const string EditorConfigPath = "Assets/Resources/F2f/Config/Editor/auto_so.asset";
		public static readonly string[] EditorConfigFolderPath = new[] { "Assets", "Resources", "F2F", "Config", "Editor" };
	}
}