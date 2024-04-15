using System.Runtime.CompilerServices;
using UnityEngine;

namespace F2F
{
	[System.Serializable]
	public struct SOGroupID
	{
		[SerializeField] internal int _id;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(SOGroupID v) => v._id;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SOGroupID(int v) => new SOGroupID(v);


		private SOGroupID(int id) => _id = id;


		public static readonly SOGroupID Invalid = new SOGroupID();
	}
}