using System.Collections.Generic;
using UnityEngine;

namespace F2F
{
	[System.Serializable]
	internal class SOGroupInfo
	{
		public int ID;
		public ScriptableObject Default;
		public List<SOInfo> Elements = new List<SOInfo>();
	}
}