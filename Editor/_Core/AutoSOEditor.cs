using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace F2F
{
	public static class AutoSOEditor
	{
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered<TSO>() 
			where TSO : ScriptableObject => Config.IsRegistered(Config.FindGroupID(typeof(TSO)));

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(Type soType) => Config.IsRegistered(Config.FindGroupID(soType));

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(SOGroupID groupID) => Config.IsRegistered(groupID);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<TSO>(SOKey key) 
			where TSO : ScriptableObject => Config.Has(Config.FindGroupID(typeof(TSO)), key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(Type soType, SOKey key) => Config.Has(Config.FindGroupID(soType), key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(SOGroupID groupID, SOKey key) => Config.Has(groupID, key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(ScriptableObject so, out SOKey key) => Config.Has(so, out key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TSO Get<TSO>(SOKey key)
			where TSO : ScriptableObject => (TSO)Config.Get(Config.FindGroupID(typeof(TSO)), key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Get(Type soType, SOKey key) => Config.Get(Config.FindGroupID(soType), key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Get(SOGroupID groupID, SOKey key) => Config.Get(groupID, key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TSO> GetAll<TSO>()
			where TSO : ScriptableObject => Config.GetAll(Config.FindGroupID(typeof(TSO))).Cast<TSO>().ToList();

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ScriptableObject> GetAll(Type soType) => Config.GetAll(Config.FindGroupID(soType));
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ScriptableObject> GetAll(SOGroupID groupID) => Config.GetAll(groupID);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<SOKey> GetKeys<TSO>() 
			where TSO : ScriptableObject => Config.GetKeys(Config.FindGroupID(typeof(TSO)));

		//TODO: summary
		public static IReadOnlyList<SOKey> GetKeys(Type soType) => Config.GetKeys(Config.FindGroupID(soType));
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<SOKey> GetKeys(SOGroupID groupID) => Config.GetKeys(groupID);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SOGroupID Register<TSO>() 
			where TSO : ScriptableObject => AutoSOEditorInternal.Register(typeof(TSO));
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SOGroupID Register(Type soType) => AutoSOEditorInternal.Register(soType);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TSO Create<TSO>(SOKey key) 
			where TSO : ScriptableObject => (TSO)AutoSOEditorInternal.Create(Config.FindGroupID(typeof(TSO)), key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Create(Type soType, SOKey key) =>
			AutoSOEditorInternal.Create(Config.FindGroupID(soType), key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Create(SOGroupID groupID, SOKey key) =>
			AutoSOEditorInternal.Create(groupID, key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove<TSO>(SOKey key) 
			where TSO : ScriptableObject => AutoSOEditorInternal.Remove(Config.FindGroupID(typeof(TSO)), key);

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove(Type soType, SOKey key) =>
			AutoSOEditorInternal.Remove(Config.FindGroupID(soType), key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove(SOGroupID groupID, SOKey key) => AutoSOEditorInternal.Remove(groupID, key);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveAll<TSO>() 
			where TSO : ScriptableObject => AutoSOEditorInternal.Remove(Config.FindGroupID(typeof(TSO)));
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveAll(Type soType) => AutoSOEditorInternal.Remove(Config.FindGroupID(soType));

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RemoveAll(SOGroupID groupID) => AutoSOEditorInternal.Remove(groupID);
		
		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckoutFast()
		{
			AutoSOEditorFixer.CheckoutConfigNullReferences();
			AutoSOEditorFixer.CheckoutConfigNames();
		}

		//TODO: summary
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckoutFull()
		{
			AutoSOEditorFixer.CheckoutConfigNullReferences();
			AutoSOEditorFixer.CheckoutConfigNames();
			AutoSOEditorFixer.CheckoutConfigPaths();
			AutoSOEditorFixer.CheckoutUnhandledObjects();
			
		}
		
		private static AutoSOConfig Config
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AutoSOEditorInternal.Config;
		}

		[InitializeOnLoadMethod]
		private static void Init() => CheckoutFast();
	}
}