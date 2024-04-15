using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F2F
{
	internal static class AutoSOEditorFixer
	{
		public static void CheckoutConfigNullReferences()
		{
			var config = AutoSOEditorInternal.Config;
			var groups = config.Groups;

			if (groups == null)
			{ 
				config.Groups = new List<SOGroupInfo>();
			}
			else
			{
				var nGroups = groups.Count;
				for (var iGroup = 0; iGroup < nGroups; iGroup++)
				{
					var groupInfo = groups[iGroup];

					if (groupInfo == null)
					{
						groups.RemoveAt(iGroup);
						--iGroup;
					
						continue;
					}

					if (groupInfo.HasNullReferences())
					{
						if (!TryFixGroupNullReferences(groupInfo))
						{
							groups.RemoveAt(iGroup);
							--iGroup;
						}
					}
				}
			}

			EditorUtility.SetDirty(config);
		}

		public static void CheckoutConfigNames()
		{
			var config = AutoSOEditorInternal.Config;

			foreach (var groupInfo in config.Groups)
			{
				TryFixSOName(groupInfo.Default, SOKey.Default);

				foreach (var soInfo in groupInfo.Elements)
				{
					TryFixSOName(soInfo.SO, soInfo.Key);
				}
			}
		}
		
		public static void CheckoutConfigPaths()
		{
			var config = AutoSOEditorInternal.Config;

			foreach (var groupInfo in config.Groups)
			{
				TryFixSOPath(groupInfo.Default, SOKey.Default);

				foreach (var soInfo in groupInfo.Elements)
				{
					TryFixSOPath(soInfo.SO, soInfo.Key);
				}
			}
		}
		
		public static void CheckoutUnhandledObjects()
		{
			var editorConfig = AutoSOEditorInternal.EditorConfig;
			
			var allFolderAssetGUIDs = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { editorConfig.Path });

			foreach (var assetGUID in allFolderAssetGUIDs)
			{
				var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetGUID);
				if (so == null)
				{
					continue;
				}

				CheckoutUnhandledObject(so);
			}
		}
		
		public static void CheckoutUnhandledObject(ScriptableObject so)
		{
			if (so == null || AutoSOEditor.Has(so, out _))
			{
				return;
			}
			
			var config = AutoSOEditorInternal.Config;
			var soType = so.GetType();

			if (!AutoSOInternal.IsValid(soType) ||
			    !SOKey.IsValidKey(so.name))
			{
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(so));
				return;
			}
			
			if (!AutoSOEditor.IsRegistered(soType) &&
			    so.name == SOKey.Default)
			{
				config.Groups.Add(new SOGroupInfo
				{
					ID = AutoSOEditorInternal.GetNewGroupID(),
					Default = so
				});
				
				EditorUtility.SetDirty(config);
				return;
			}

			var groupID = AutoSOEditor.Register(soType);
			if (groupID == SOGroupID.Invalid)
			{
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(so));
				return;
			}
			
			if (AutoSOEditor.Has(groupID, so.name))
			{
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(so));
				return;
			}

			if (config.TryFindGroupInfo(groupID, out var groupInfo))
			{
				groupInfo.Elements.Add(new SOInfo
				{
					SO = so,
					Key = so.name
				});
				
				EditorUtility.SetDirty(config);
			}
		}
		
		private static bool TryFixGroupNullReferences(SOGroupInfo groupInfo)
		{
			if (!groupInfo.TryGetType(out var soType))
			{
				return false;
			}

			if (groupInfo.Default == null)
			{
				groupInfo.Default = AutoSOEditorInternal.GetOrCreateSO(soType, SOKey.Default);
			}

			var nElements = groupInfo.Elements.Count;
			for (var iElement = nElements - 1; iElement >= 0; iElement--)
			{
				var elementInfo = groupInfo.Elements[iElement];
				
				if (elementInfo == null || elementInfo.SO == null)
				{
					groupInfo.Elements.RemoveAt(iElement);
				}
			}
			
			return true;
		}

		private static bool TryFixSOName(Object so, SOKey key)
		{
			if (so.name == key.ToString())
			{
				return false;
			}

			var soPath = AssetDatabase.GetAssetPath(so);
			if (string.IsNullOrEmpty(soPath))
			{
				return false;
			}
			
			AssetDatabase.RenameAsset(soPath, key);
			return true;
		}
		
		private static bool TryFixSOPath(Object so, SOKey key)
		{
			var soTargetPath = AutoSOEditorInternal.GetSOPath(so.GetType(), key);
			var soPath = AssetDatabase.GetAssetPath(so);

			if (string.IsNullOrEmpty(soPath))
			{
				AssetDatabase.CreateAsset(so, soTargetPath);
				return true;
			}

			if (AssetDatabase.AssetPathToGUID(soTargetPath) != 
			    AssetDatabase.AssetPathToGUID(soPath))
			{
				AssetDatabase.MoveAsset(soPath, soTargetPath);
				return true;
			}

			return false;
		}
	}
}