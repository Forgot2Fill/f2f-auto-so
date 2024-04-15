using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace F2F
{
	public struct SOKey
	{
		private static readonly Regex _invalidRegex = new Regex(@"\W+?");
		private const string _defaultKey = "default";

		private readonly bool _isCustom;
		private readonly string _key;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator string(SOKey key) => key.ToString();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SOKey(string key) => new SOKey(key);
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SOKey(string key)
		{
			var isValidKey = IsValidKey(key);
			
			_isCustom = isValidKey && key != _defaultKey;
			_key = isValidKey ? key : _defaultKey;
		}
		
		public override string ToString() => !_isCustom ? _defaultKey : _key;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsDefault() => !_isCustom;

		public static readonly SOKey Default = new SOKey();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValidKey(string key) => !string.IsNullOrEmpty(key) && !_invalidRegex.IsMatch(key);
	}
}