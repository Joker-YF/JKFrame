//-----------------------------------------------------------------------
// <copyright file="MissingAddressableGroupReferenceValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && ODIN_VALIDATOR_3_1

using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEditor.AddressableAssets.Settings;
using System.Collections;
using Sirenix.OdinInspector.Modules.Addressables.Editor;

[assembly: RegisterValidator(typeof(MissingAddressableGroupReferenceValidator))]

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	public class MissingAddressableGroupReferenceValidator : GlobalValidator
	{
		public override IEnumerable RunValidation(ValidationResult result)
		{
			var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

			if (addressableAssetSettings == null) yield break;

			var missingGroupIndices = new List<int>();

			for (var i = 0; i < addressableAssetSettings.groups.Count; i++)
			{
				var group = addressableAssetSettings.groups[i];

				if (group == null)
				{
					missingGroupIndices.Add(i);
				}
			}

			if (missingGroupIndices.Count > 0)
			{
				result.Add(ValidatorSeverity.Error, "Addressable groups contains missing references").WithFix("Delete missing reference", () =>
				{
					for (var i = missingGroupIndices.Count - 1; i >= 0; i--)
					{
						addressableAssetSettings.groups.RemoveAt(missingGroupIndices[i]);
						addressableAssetSettings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, null, true, true);
					}
				});
			}
		}
	}
}

#endif