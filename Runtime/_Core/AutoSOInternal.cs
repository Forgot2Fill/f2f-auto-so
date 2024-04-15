using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace F2F
{
	internal static class AutoSOInternal
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SOGroupID ToGroupID(Type soType)
		{
			var soTypeHash = soType.GetHashCode();
			return _soGroupIDs.TryGetValue(soTypeHash, out var soGroupID) ? soGroupID : SOGroupID.Invalid;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(Type soType) => _soGroupIDs.ContainsKey(soType.GetHashCode());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(SOGroupID groupID) => _soGroups.ContainsKey(groupID);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(Type soType, SOKey key)
		{
			return _soGroupIDs.TryGetValue(soType.GetHashCode(), out var soGroupID) &&
			       _soGroups.TryGetValue(soGroupID, out var soGroupData) &&
			       (key.IsDefault() || soGroupData.Elements.ContainsKey(key));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(SOGroupID groupID, SOKey key)
		{
			return _soGroups.TryGetValue(groupID, out var soGroupData) &&
			       (key.IsDefault() || soGroupData.Elements.ContainsKey(key));
		}

		public static ScriptableObject Get(Type soType, SOKey key)
		{
			var soTypeHash = soType.GetHashCode();
			if (!_soGroupIDs.TryGetValue(soTypeHash, out var soGroupID))
			{
				return null;
			}
			
			if (!_soGroups.TryGetValue(soGroupID, out var soGroupData))
			{
				return null;
			}

			if (key.IsDefault())
			{
				return soGroupData.Default;
			}

			return soGroupData.Elements.TryGetValue(key, out var so) ? so : soGroupData.Default;
		}

		public static ScriptableObject Get(SOGroupID groupID, SOKey key)
		{
			if (!_soGroups.TryGetValue(groupID, out var soGroupData))
			{
				return null;
			}

			if (key.IsDefault())
			{
				return soGroupData.Default;
			}

			return soGroupData.Elements.TryGetValue(key, out var so) ? so : soGroupData.Default;
		}

		public static IReadOnlyList<TSO> GetAll<TSO>() where TSO : ScriptableObject
		{
			var soType = typeof(TSO);
			var soTypeHash = soType.GetHashCode();

			if (!_soGroupIDs.TryGetValue(soTypeHash, out var soGroupID) ||
			    !_soGroups.TryGetValue(soGroupID, out var soGroupData))
			{
				return Array.Empty<TSO>();
			}

			var nSO = soGroupData.Elements.Count + 1;
			var soArray = new TSO[nSO];

			var iSO = 0;
			soArray[iSO++] = (TSO)soGroupData.Default;

			foreach (var so in soGroupData.Elements.Values)
			{
				soArray[iSO++] = (TSO)so;
			}

			return soArray;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ScriptableObject> GetAll(Type soType)
		{
			return _soGroupIDs.TryGetValue(soType.GetHashCode(), out var soGroupID)
				? GetAll(soGroupID)
				: Array.Empty<ScriptableObject>();
		}

		public static IReadOnlyList<ScriptableObject> GetAll(SOGroupID groupID)
		{
			if (!_soGroups.TryGetValue(groupID, out var soGroupData))
			{
				return Array.Empty<ScriptableObject>();
			}

			var nSO = soGroupData.Elements.Count + 1;
			var soArray = new ScriptableObject[nSO];

			var iSO = 0;
			soArray[iSO++] = soGroupData.Default;

			foreach (var so in soGroupData.Elements.Values)
			{
				soArray[iSO++] = so;
			}

			return soArray;
		}
		
		
		private static IReadOnlyDictionary<int, int> _soGroupIDs;
		private static IReadOnlyDictionary<int, SOGroupData> _soGroups;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			var soGroupIDs = new Dictionary<int, int>();
			var soGroups = new Dictionary<int, SOGroupData>();

			_soGroupIDs = soGroupIDs;
			_soGroups = soGroups;

			var config = LoadConfig();
			if (config == null)
			{
				return;
			}
			
			var nGroups = config.Groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = config.Groups[iGroup];

				if (groupInfo == null || groupInfo.Default == null)
				{
					continue;
				}

				var soType = groupInfo.Default.GetType();

				if (!IsValid(soType))
				{
					continue;
				}
				
				var soGroupID = groupInfo.ID;
				if (soGroups.ContainsKey(soGroupID))
				{
					continue;
				}
				
				var soTypeHash = soType.GetHashCode();
				if (soGroupIDs.ContainsKey(soTypeHash))
				{
					continue;
				}
				
				var elements = new Dictionary<string, ScriptableObject>();

				if (groupInfo.Elements != null)
				{
					var nElements = groupInfo.Elements.Count;
					for (var iElement = 0; iElement < nElements; iElement++)
					{
						var elementInfo = groupInfo.Elements[iElement];

						if (elementInfo == null ||
						    !SOKey.IsValidKey(elementInfo.Key) ||
						    elementInfo.SO == null || elementInfo.SO.GetType() != soType ||
						    elements.ContainsKey(elementInfo.Key))
						{
							continue;
						}

						elements.Add(elementInfo.Key, elementInfo.SO);
					}
				}
				
				var soGroupData = new SOGroupData(soType, groupInfo.Default, elements);
				soGroups.Add(soGroupID, soGroupData);

				soGroupIDs.Add(soTypeHash, soGroupID);
			}
			
			Debug.Log("[F2F.AutoSO] Init complete successfully");
		}

		//TODO: Move from here
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(Type soType) => soType != typeof(ScriptableObject) && !soType.IsAbstract &&
		                                             typeof(ScriptableObject).IsAssignableFrom(soType);

		private const string _configPath = "F2F/Config/auto_so.asset";
		private static AutoSOConfig LoadConfig() => Resources.Load<AutoSOConfig>(_configPath);
	}
}