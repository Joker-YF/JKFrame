//-----------------------------------------------------------------------
// <copyright file="AssetLabelReferenceValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AddressableAssets;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector.Modules.Addressables.Editor;

#if ODIN_VALIDATOR_3_1
[assembly: RegisterValidationRule(typeof(AssetLabelReferenceValidator), Description =
	"This validator ensures that AssetLabelReferences marked with the Required attribute display an error " +
	"message if they are not set. It can also be configured to require that all AssetLabelReferences be set " +
	"by default; the Optional attribute can then be used to exclude specific AssetLabelReferences from " +
	"validation.")]
#else
[assembly: RegisterValidator(typeof(AssetLabelReferenceValidator))]
#endif

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	/// <summary>
	/// Validator for AssetLabelReference values.
	/// </summary>
	public class AssetLabelReferenceValidator : ValueValidator<AssetLabelReference>
    {
        [Tooltip("If enabled, the validator will display an error message if the AssetLabelReference is not set. " +
            "If disabled, the validator will only display an error message if the AssetLabelReference is set, but the " +
            "assigned label does not exist.")]
        [ToggleLeft]
        public bool RequiredByDefault;

        private bool required;
        private bool optional;
        private string requiredMessage;

        protected override void Initialize()
        {
            var requiredAttr = this.Property.GetAttribute<RequiredAttribute>();

            this.requiredMessage = requiredAttr?.ErrorMessage ?? $"<b>{this.Property.NiceName}</b> is required.";

            if (this.RequiredByDefault)
            {
                required = true;
                optional = Property.GetAttribute<OptionalAttribute>() != null;
            }
            else
            {
                required = requiredAttr != null;
                optional = false;
            }
        }

        protected override void Validate(ValidationResult result)
        {
            // If the Addressables settings have not been created, nothing else is really valid.
            if (AddressableAssetSettingsDefaultObject.SettingsExists == false)
            {
                result.AddError("Addressables Settings have not been created.")
                    .WithButton("Open Settings Window", () => OdinAddressableUtility.OpenGroupsWindow());
                return;
            }

            var value = Value?.labelString;

            if (string.IsNullOrEmpty(value))
            {
                if (optional == false && required) // Optional == false & required? Nice.
                {
                    result.AddError(requiredMessage).EnableRichText();
                }
            }
            else
            {
                var labels = AddressableAssetSettingsDefaultObject.Settings.GetLabels();

                if (labels.Contains(value) == false)
                {
                    result.AddError($"Label <i>{value}</i> has not been created as a label.")
                        .WithButton("Open Label Settings", () => OdinAddressableUtility.OpenLabelsWindow());
                }
            }
        }
    }

}

#endif