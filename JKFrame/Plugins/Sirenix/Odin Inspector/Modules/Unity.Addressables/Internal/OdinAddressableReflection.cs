//-----------------------------------------------------------------------
// <copyright file="OdinAddressableReflection.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor.AddressableAssets.Settings;

namespace Sirenix.OdinInspector.Modules.Addressables.Editor.Internal
{
	internal static class OdinAddressableReflection
	{
		public static FieldInfo AddressableAssetEntry_mGUID_Field;

		static OdinAddressableReflection()
		{
			AddressableAssetEntry_mGUID_Field = typeof(AddressableAssetEntry).GetField("m_GUID", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		internal static void EnsureConstructed() { }
	}
}
#endif