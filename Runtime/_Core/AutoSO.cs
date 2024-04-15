using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

[assembly: InternalsVisibleTo("F2F.AutoSO.Editor")]

namespace F2F
{
	public static class AutoSO
	{
		/// <summary>
		/// Convert <c>TSO</c> to <c>SOGroupID</c>
		/// </summary>
		/// <typeparam name="TSO">non-abstract <c>ScriptableObject</c> type</typeparam>
		/// <returns><c>ScriptableObject</c> group ID</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SOGroupID ToGroupID<TSO>() where TSO : ScriptableObject
		{
			AssetEditorCall();
			return AutoSOInternal.ToGroupID(typeof(TSO));
		}

		/// <summary>
		/// Convert <c>soType</c> to <c>SOGroupID</c>
		/// </summary>
		/// <param name="soType">non-abstract <c>ScriptableObject</c> type</param>
		/// <returns><c>ScriptableObject</c> group ID</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SOGroupID ToGroupID(Type soType)
		{
			AssetEditorCall();
			return AutoSOInternal.ToGroupID(soType);
		}
		
		/// <summary>
		/// <para>Check is <c>ScriptableObject</c> of <c>TSO</c> registered in AutoSO system</para>
		/// <para>If not, you cannot get any <c>ScriptableObject</c> of this type</para>
		/// </summary>
		/// <typeparam name="TSO">non-abstract <c>ScriptableObject</c> type</typeparam>
		/// <returns>
		/// <para>True if <c>TSO</c> is registered</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered<TSO>() where TSO : ScriptableObject
		{
			AssetEditorCall();
			return AutoSOInternal.IsRegistered(typeof(TSO));
		}

		/// <summary>
		/// <para>Check is <c>ScriptableObject</c> of <c>soType</c> registered in AutoSO system</para>
		/// <para>If not, you cannot get any <c>ScriptableObject</c> of this type</para>
		/// </summary>
		/// <param name="soType">non-abstract <c>ScriptableObject</c> type</param>
		/// <returns>
		/// <para>True if <c>soType</c> is registered</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(Type soType)
		{
			AssetEditorCall();
			return AutoSOInternal.IsRegistered(soType);
		}
		
		/// <summary>
		/// <para>Check is <c>groupID</c> registered in AutoSO system</para>
		/// <para>If not, you cannot get any <c>ScriptableObject</c> of this group ID</para>
		/// </summary>
		/// <param name="groupID"><c>ScriptableObject</c> group ID</param>
		/// <returns>
		/// <para>True if <c>groupID</c> is registered</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegistered(SOGroupID groupID)
		{
			AssetEditorCall();
			return AutoSOInternal.IsRegistered(groupID);
		}

		/// <summary>
		/// Check is AutoSO system has <c>ScriptableObject</c> instance of <c>TSO</c> with key <c>SOKey</c>.
		/// </summary>
		/// <param name="key">key of <c>ScriptableObject</c> instance</param>
		/// <typeparam name="TSO">non-abstract <c>ScriptableObject</c> type</typeparam>
		/// <returns>True if has <c>ScriptableObject</c> instance of <c>TSO</c> with <c>key</c></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<TSO>(SOKey key) where TSO : ScriptableObject
		{
			AssetEditorCall();
			return AutoSOInternal.Has(typeof(TSO), key);
		}

		/// <summary>
		/// Check is AutoSO system has <c>ScriptableObject</c> instance of <c>soType</c> with key <c>SOKey</c>.
		/// </summary>
		/// <param name="soType">non-abstract <c>ScriptableObject</c> type</param>
		/// <param name="key">key of <c>ScriptableObject</c> instance</param>
		/// <returns>True if has <c>ScriptableObject</c> instance of <c>soType</c> with <c>key</c></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has(Type soType, SOKey key)
		{
			AssetEditorCall();
			return AutoSOInternal.Has(soType, key);
		}

		/// <summary>
		/// Check is AutoSO system has <c>ScriptableObject</c> instance inside group with <c>groupID</c> with key <c>SOKey</c>.
		/// </summary>
		/// <param name="groupID"><c>ScriptableObject</c> group ID</param>
		/// <param name="key">key of <c>ScriptableObject</c> instance</param>
		/// <returns>True if has <c>ScriptableObject</c> instance of <c>soType</c> with <c>key</c></returns>
		public static bool Has(SOGroupID groupID, SOKey key)
		{
			AssetEditorCall();
			return AutoSOInternal.Has(groupID, key);
		}

		/// <summary>
		/// Get <c>ScriptableObject</c> instance of type <c>TSO</c> by key <c>SOKey</c>
		/// </summary>
		/// <param name="key">Key of <c>ScriptableObject</c> instance</param>
		/// <typeparam name="TSO">non-abstract <c>ScriptableObject</c> type</typeparam>
		/// <returns>
		/// <para><c>ScriptableObject</c> instance of type <c>TSO</c></para>
		/// <para><b>default</b> instance, if AutoSO system doesn't have key</para>
		/// <para><b><c>null</c></b>, if <c>TSO</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TSO Get<TSO>(SOKey key) where TSO : ScriptableObject
		{
			AssetEditorCall();
			return (TSO)AutoSOInternal.Get(typeof(TSO), key);
		}

		/// <summary>
		/// Get <c>ScriptableObject</c> instance of <c>soType</c> by key <c>SOKey</c>
		/// </summary>
		/// <param name="soType">non-abstract <c>ScriptableObject</c> type</param>
		/// <param name="key">Key of <c>ScriptableObject</c> instance</param>
		/// <returns>
		/// <para><c>ScriptableObject</c> instance of <c>soType</c></para>
		/// <para><b>default</b> instance, if AutoSO system doesn't have key</para>
		/// <para><b><c>null</c></b>, if <c>soType</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Get(Type soType, SOKey key)
		{
			AssetEditorCall();
			return AutoSOInternal.Get(soType, key);
		}

		/// <summary>
		/// Get <c>ScriptableObject</c> instance inside group with <c>groupID</c> by key <c>SOKey</c>
		/// </summary>
		/// <param name="groupID"><c>ScriptableObject</c> group ID</param>
		/// <param name="key">Key of <c>ScriptableObject</c> instance</param>
		/// <returns>
		/// <para><c>ScriptableObject</c> instance of <c>soType</c></para>
		/// <para><b>default</b> instance, if AutoSO system doesn't have key</para>
		/// <para><b><c>null</c></b>, if <c>groupID</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptableObject Get(SOGroupID groupID, SOKey key)
		{
			AssetEditorCall();
			return AutoSOInternal.Get(groupID, key);
		}

		/// <summary>
		/// <para>Get all <c>ScriptableObject</c> instances of <c>TSO</c></para>
		/// </summary>
		/// <typeparam name="TSO">non-abstract <c>ScriptableObject</c> type</typeparam>
		/// <returns>
		/// <para>Read-only list of <c>ScriptableObject</c> instances of <c>TSO</c></para>
		/// <para>Empty list, if <c>TSO</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TSO> GetAll<TSO>()
			where TSO : ScriptableObject
		{
			AssetEditorCall();
			return AutoSOInternal.GetAll<TSO>();
		}
		
		/// <summary>
		/// <para>Get all <c>ScriptableObject</c> instances of <c>soType</c></para>
		/// </summary>
		/// <param name="soType">non-abstract <c>ScriptableObject</c> type</param>
		/// <returns>
		/// <para>Read-only list of <c>ScriptableObject</c> instances</para>
		/// <para>Empty list, if <c>soType</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ScriptableObject> GetAll(Type soType)
		{
			AssetEditorCall();
			return AutoSOInternal.GetAll(soType);
		}
		
		/// <summary>
		/// <para>Get all <c>ScriptableObject</c> instances inside group with <c>groupID</c></para>
		/// </summary>
		/// <param name="groupID"><c>ScriptableObject</c> group ID</param>
		/// <returns>
		/// <para>Read-only list of <c>ScriptableObject</c> instances</para>
		/// <para>Empty list, if <c>groupID</c> isn't registered in AutoSO system</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<ScriptableObject> GetAll(SOGroupID groupID)
		{
			AssetEditorCall();
			return AutoSOInternal.GetAll(groupID);
		}
		
		#region ASSERT
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AssetEditorCall()
		{
#if UNITY_EDITOR
			Assert.IsTrue(Application.isPlaying, "[F2F.AutoSO] Call some runtime method from editor");
#endif
		}
		#endregion
	}
}