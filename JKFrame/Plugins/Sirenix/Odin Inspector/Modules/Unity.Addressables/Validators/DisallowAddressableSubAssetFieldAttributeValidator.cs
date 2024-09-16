//-----------------------------------------------------------------------
// <copyright file="DisallowAddressableSubAssetFieldAttributeValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Modules.Addressables.Editor;
using UnityEngine.AddressableAssets;

[assembly: RegisterValidator(typeof(DisallowAddressableSubAssetFieldAttributeValidator))]

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	/// <summary>
	/// Validator for the DisallowAddressableSubAssetFieldAttribute.
	/// </summary>
	public class DisallowAddressableSubAssetFieldAttributeValidator : AttributeValidator<DisallowAddressableSubAssetFieldAttribute, AssetReference>
    {
        protected override void Validate(ValidationResult result)
        {
            if (this.Value != null && string.IsNullOrEmpty(this.Value.SubObjectName) == false)
            {
                result.AddError("Sub-asset references is not allowed on this field.")
                    .WithFix("Remove Sub-Asset", () => this.Value.SubObjectName = null, true);
            }
        }
    }

}

#endif