using System;
using System.Collections.Generic;
using UnityEngine;

namespace F2F
{
	internal static class AutoSOConfigExtensions
	{
		public static bool IsRegistered(this AutoSOConfig config, SOGroupID groupID)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return false;
			}
			
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID == groupID)
				{
					return true;
				}
			}

			return false;
		}
		
		public static bool Has(this AutoSOConfig config, SOGroupID groupID, SOKey key)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return false;
			}
			
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID != groupID ||
				    groupInfo.Elements == null ||
				    groupInfo.Default == null)
				{
					continue;
				}

				if (key.IsDefault())
				{
					return true;
				}
				
				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.SO != null &&
					    soInfo.Key == key)
					{
						return true;
					}
				}
			}

			return false;
		}

		public static bool Has(this AutoSOConfig config, ScriptableObject so, out SOKey key)
		{
			if (so == null)
			{
				key = SOKey.Default;
				return false;
			}
			
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.Default == null)
				{
					continue;
				}

				if (groupInfo.Default == so)
				{
					key = SOKey.Default;
					return true;
				}

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];

					if (soInfo.SO == so)
					{
						key = soInfo.Key;
						return true;
					}
				}
			}

			key = SOKey.Default;
			return false;
		}

		public static ScriptableObject Get(this AutoSOConfig config, SOGroupID groupID, SOKey key)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return null;
			}
			
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];
				if (groupInfo.ID != groupID)
				{
					continue;
				}

				if (key.IsDefault())
				{
					return groupInfo.Default;
				}

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.Key == key)
					{
						return soInfo.SO;
					}
				}
			}
			
			return null;
		}
		
		public static IReadOnlyList<ScriptableObject> GetAll(this AutoSOConfig config, SOGroupID groupID)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return Array.Empty<ScriptableObject>();
			}
			
			var list = new List<ScriptableObject>();
			
			var groups = config.Groups;
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
					list.Add(groupInfo.Default);
				}

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.SO != null)
					{
						list.Add(soInfo.SO);
					}
				}
			}

			return list;
		}

		public static IReadOnlyList<SOKey> GetKeys(this AutoSOConfig config, SOGroupID groupID)
		{
			if (groupID == SOGroupID.Invalid)
			{
				return Array.Empty<SOKey>();
			}
			
			var list = new List<SOKey>();
			
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.ID != groupID ||
				    groupInfo.Default == null)
				{
					continue;
				}
				
				list.Add(SOKey.Default);

				var elements = groupInfo.Elements;
				var nElements = elements.Count;
				for (var iElement = 0; iElement < nElements; iElement++)
				{
					var soInfo = elements[iElement];
					if (soInfo.SO != null)
					{
						list.Add(soInfo.Key);
					}
				}
			}

			return list;
		}
		
		public static SOGroupID FindGroupID(this AutoSOConfig config, Type soType)
		{
			var groups = config.Groups;
			var nGroups = groups.Count;
			for (var iGroup = 0; iGroup < nGroups; iGroup++)
			{
				var groupInfo = groups[iGroup];

				if (groupInfo.Default == null ||
				    groupInfo.Default.GetType() != soType)
				{
					continue;
				}

				return groupInfo.ID;
			}
			
			return SOGroupID.Invalid;
		}
		
		public static bool TryFindGroupInfo(this AutoSOConfig config, SOGroupID groupID, out SOGroupInfo info)
		{
			foreach (var groupInfo in config.Groups)
			{
				if (groupInfo.ID == groupID)
				{
					info = groupInfo;
					return true;
				}
			}

			info = null;
			return false;
		}
	}
}