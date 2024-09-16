//-----------------------------------------------------------------------
// <copyright file="AssetReferenceValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector.Modules.Addressables.Editor;

#if ODIN_VALIDATOR_3_1
[assembly: RegisterValidationRule(typeof(AssetReferenceValidator), Description =
	"This validator provides robust integrity checks for your asset references within Unity. " +
	"It validates whether an asset reference has been assigned, and if it's missing, raises an error. " +
	"It further checks the existence of the main asset at the assigned path, ensuring it hasn't been " +
	"inadvertently deleted or moved. The validator also verifies if the assigned asset is addressable " +
	"and, if not, offers a fix to make it addressable. Moreover, it ensures the asset adheres to " +
	"specific label restrictions set through the AssetReferenceUILabelRestriction attribute. " +
	"Lastly, it performs checks on any sub-object linked to the asset, making sure it hasn't gone missing. " +
	"This comprehensive validation system prevents hard-to-spot bugs and errors, " +
	"fostering a more robust and efficient development workflow.")]
#else
[assembly: RegisterValidator(typeof(AssetReferenceValidator))]
#endif

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	public class AssetReferenceValidator : ValueValidator<AssetReference>
    {
        [Tooltip("If true and the AssetReference is not marked with the Optional attribute, " +
            "the validator will display an error message if the AssetReference is not set. " +
            "If false, the validator will only display an error message if the AssetReference is set, " +
            "but the assigned asset does not exist.")]
        [ToggleLeft]
        public bool RequiredByDefault;

        private bool required;
        private bool optional;
        private string requiredMessage;

        private List<AssetReferenceUIRestriction> restrictions;

        protected override void Initialize()
        {
            var requiredAttr = this.Property.GetAttribute<RequiredAttribute>();

            this.requiredMessage = requiredAttr?.ErrorMessage ?? $"<b>{this.Property.NiceName}</b> is required.";

            if (this.RequiredByDefault)
            {
                this.required = true;
                this.optional = this.Property.GetAttribute<OptionalAttribute>() != null;
            }
            else
            {
                this.required = requiredAttr != null;
                this.optional = false;
            }

            this.restrictions = new List<AssetReferenceUIRestriction>();
            foreach (var attr in this.Property.Attributes)
            {
                if (attr is AssetReferenceUIRestriction r)
                {
                    this.restrictions.Add(r);
                }
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

            var assetReference = this.Value;
            var assetReferenceHasBeenAssigned = !string.IsNullOrEmpty(assetReference?.AssetGUID);

            // No item has been assigned.
            if (!assetReferenceHasBeenAssigned)
            {
                if (optional == false && required) // Optional == false & required? Nice.
                {
                    result.AddError(this.requiredMessage).EnableRichText();
                }

                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);

            // The item has been assigned, but is now missing.
            if (mainAsset == null)
            {
                result.AddError($"The previously assigned main asset with path <b>'{assetPath}'</b> is missing. GUID <b>'{assetReference.AssetGUID}'</b>");
                return;
            }

            var addressableAssetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(assetReference.AssetGUID, true);
            var isAddressable = addressableAssetEntry != null;

            // Somehow an item sneaked through all of unity's validation measures and ended up not being addressable
            // while still ending up in the asset reference object field.
            if (!isAddressable)
            {
                result.AddError("Assigned item is not addressable.")
                    .WithFix<MakeAddressableFixArgs>("Make Addressable", args => OdinAddressableUtility.MakeAddressable(mainAsset, args.Group));
            }
            // Check the assigned item against any and all label restrictions.
            else
            {
                if (OdinAddressableUtility.ValidateAssetReferenceRestrictions(restrictions, mainAsset, out var failedRestriction) == false)
                {
                    if (failedRestriction is AssetReferenceUILabelRestriction labelRestriction)
                    {
                        result.AddError($"Asset reference is restricted to items with these specific labels <b>'{string.Join(", ", labelRestriction.m_AllowedLabels)}'</b>. The currently assigned item has none of them.")
                            .WithFix<AddLabelsFixArgs>("Add Labels", args => SetLabels(mainAsset, args.AssetLabels));
                    }
                    else
                    {
                        result.AddError("Restriction failed: " + failedRestriction.ToString());
                    }
                }
            }

            // The assigned item had a sub object, but it's missing.
            if (!string.IsNullOrEmpty(assetReference.SubObjectName))
            {
                var subObjects = OdinAddressableUtility.EnumerateAllActualAndVirtualSubAssets(mainAsset, assetPath);

                var hasMissingSubObject = true;

                foreach (var subObject in subObjects)
                {
                    if (subObject.name == assetReference.SubObjectName)
                    {
                        hasMissingSubObject = false;
                        break;
                    }
                }

                if (hasMissingSubObject)
                {
                    result.AddError($"The previously assigned sub asset with name <b>'{assetReference.SubObjectName}'</b> is missing.").EnableRichText();
                }
            }

            if (assetReference.ValidateAsset(mainAsset) || assetReference.ValidateAsset(assetPath))
                return;

            if (assetReference is AssetReferenceSprite && assetReference.editorAsset is Sprite)
                return;

            result.AddError($"{assetReference.GetType().GetNiceFullName()}.ValidateAsset failed to validate assigned asset.");
        }

        private static void SetLabels(UnityEngine.Object obj, List<AssetLabel> assetLabels)
        {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) return;

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            var entry = settings.FindAssetEntry(guid, false);

            foreach (var assetLabel in assetLabels.Where(a => a.Toggled))
            {
                entry.SetLabel(assetLabel.Label, true, false, false);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.LabelAdded, entry, false, true);
        }

        private class MakeAddressableFixArgs
        {
            [ValueDropdown(nameof(GetGroups))]
            [OnInspectorInit(nameof(SelectDefault))]
            public AddressableAssetGroup Group;

            private void SelectDefault()
            {
                this.Group = AddressableAssetSettingsDefaultObject.SettingsExists
                    ? AddressableAssetSettingsDefaultObject.Settings.DefaultGroup
                    : null;
            }

            private static IEnumerable<ValueDropdownItem> GetGroups()
            {
                return !AddressableAssetSettingsDefaultObject.SettingsExists
                    ? Enumerable.Empty<ValueDropdownItem>()
                    : AddressableAssetSettingsDefaultObject.Settings.groups
                        .Where(group => !group.ReadOnly)
                        .Select(group => new ValueDropdownItem(group.Name, group));
            }

            [Button(SdfIconType.ListNested), PropertySpace(8f)]
            private void OpenAddressablesGroups() => OdinAddressableUtility.OpenGroupsWindow();
        }

        private class AddLabelsFixArgs
        {
            [HideIf("@true")]
            public List<AssetLabel> AssetLabels
            {
                get
                {
                    if (!AddressableAssetSettingsDefaultObject.SettingsExists) return this.assetLabels;

                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    var labels = settings
                        .GetLabels()
                        .Select(l => new AssetLabel { Label = l, Toggled = false })
                        .ToList();

                    foreach (var assetLabel in this.assetLabels)
                    {
                        var label = labels.FirstOrDefault(l => l.Label == assetLabel.Label);

                        if (label != null)
                        {
                            label.Toggled = assetLabel.Toggled;
                        }
                    }

                    this.assetLabels = labels;
                    return this.assetLabels;
                }
            }

            private List<AssetLabel> assetLabels = new List<AssetLabel>();

            [OnInspectorGUI]
            private void Draw()
            {
                var togglesRect = EditorGUILayout.GetControlRect(false, Mathf.CeilToInt(this.AssetLabels.Count / 2f) * 20f);

                for (var i = 0; i < this.AssetLabels.Count; i++)
                {
                    var assetLabel = this.AssetLabels[i];
                    var toggleRect = togglesRect.SplitGrid(togglesRect.width / 2f, 20, i);
                    assetLabel.Toggled = GUI.Toggle(toggleRect, assetLabel.Toggled, assetLabel.Label);
                }

                if (!AddressableAssetSettingsDefaultObject.SettingsExists) return;

                GUILayout.Space(8f);

                var buttonsRect = EditorGUILayout.GetControlRect(false, 20f);

                if (SirenixEditorGUI.SDFIconButton(buttonsRect, "Open Addressables Labels", SdfIconType.TagsFill))
                {
                    OdinAddressableUtility.OpenLabelsWindow();
                }
            }
        }

        private class AssetLabel
        {
            public bool Toggled;
            public string Label;
        }
    }

}

#endif