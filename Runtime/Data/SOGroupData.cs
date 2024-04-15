using System;
using System.Collections.Generic;
using UnityEngine;

namespace F2F
{
	public class SOGroupData
	{
		public readonly Type Type;
		public readonly ScriptableObject Default;
		public readonly IReadOnlyDictionary<string, ScriptableObject> Elements;

		public SOGroupData(Type type, ScriptableObject defaultSO,
			IReadOnlyDictionary<string, ScriptableObject> elements)
		{
			Type = type;
			Default = defaultSO;
			Elements = elements;
		}
	}
}