//-----------------------------------------------------------------------
// <copyright file="CheckDuplicateBundleDependenciesValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && ODIN_VALIDATOR_3_1

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor.AddressableAssets.Settings;
using System.Collections;
using System;
using Sirenix.OdinValidator.Editor;
using Sirenix.OdinInspector.Modules.Addressables.Editor;

[assembly: RegisterValidationRule(typeof(CheckDuplicateBundleDependenciesValidator),
	Description = "This validator detects potential duplicate asset dependencies in an addressable group, without the need for a build. " +
		"For instance, imagine two prefabs in separate groups, both referencing the same material. Each group would then include the material " +
	"and all its associated dependencies. " +
		"To address this, the material should be marked as Addressable, either with one of the prefabs or in a distinct group.\n\n" +
		"<b>Fixes: </b>Executing the fix will make the dependency addressable and move it to the specified group.\n\n" +
		"<b>Exceptions: </b>It's important to note that duplicate assets aren't inherently problematic. For example, if certain assets are " +
	"never accessed by the same user group, such as region-specific assets, these duplications might be desired or at least inconsequential. " +
	"As every project is unique, decisions concerning duplicate asset dependencies should be considered on a case-by-case basis.")]

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	public class CheckDuplicateBundleDependenciesValidator : GlobalValidator
	{
		private static Dictionary<GUID, List<string>> dependencyGroupMap = new Dictionary<GUID, List<string>>();

		[Tooltip("The severity of the validation result.")]
		public ValidatorSeverity ValidatorSeverity = ValidatorSeverity.Warning;

		[Tooltip("Assets to ignore when validating.")]
		[LabelText("Ignored GUIDs"), CustomValueDrawer(nameof(DrawGUIDEntry))]
		public List<string> IgnoredGUIDs = new List<string>();

		public override IEnumerable RunValidation(ValidationResult result)
		{
			dependencyGroupMap.Clear();

			var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

			if (addressableAssetSettings == null) yield break;

			foreach (var addressableAssetGroup in addressableAssetSettings.groups)
			{
				if (addressableAssetGroup == null) continue;

				foreach (var addressableAssetEntry in addressableAssetGroup.entries)
				{
					var dependencyAssetPaths = AssetDatabase.GetDependencies(addressableAssetEntry.AssetPath)
						.Where(assetPath => !assetPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
											!assetPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

					foreach (var dependencyAssetPath in dependencyAssetPaths)
					{
						var dependencyGUID = new GUID(AssetDatabase.AssetPathToGUID(dependencyAssetPath));

						if (this.IgnoredGUIDs.Contains(dependencyGUID.ToString())) continue;

						var dependencyAddressableAssetEntry = addressableAssetSettings.FindAssetEntry(dependencyGUID.ToString());

						var isAddressable = dependencyAddressableAssetEntry != null;
						if (isAddressable) continue;

						if (!dependencyGroupMap.ContainsKey(dependencyGUID))
						{
							dependencyGroupMap.Add(dependencyGUID, new List<string>());
						}

						if (!dependencyGroupMap[dependencyGUID].Contains(addressableAssetGroup.Name))
						{
							dependencyGroupMap[dependencyGUID].Add(addressableAssetGroup.Name);
						}
					}
				}
			}

			foreach (var kvp in dependencyGroupMap)
			{
				var dependencyGUID = kvp.Key;
				var groups = kvp.Value;

				if (groups.Count > 1)
				{
					var assetPath = AssetDatabase.GUIDToAssetPath(dependencyGUID.ToString());
					var message = $"{assetPath} is duplicated in these groups: {string.Join(", ", groups)}";

					result.Add(this.ValidatorSeverity, message).WithFix<FixArgs>(args =>
					{
						if (args.FixChoice == FixChoice.Ignore)
						{
							var sourceType = args.IgnoreForEveryone ? ConfigSourceType.Project : ConfigSourceType.Local;
							var data = RuleConfig.Instance.GetRuleData<CheckDuplicateBundleDependenciesValidator>(sourceType);
							data.IgnoredGUIDs.Add(dependencyGUID.ToString());
							RuleConfig.Instance.SetAndSaveRuleData(data, sourceType);
							return;
						}

						var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
						AddressableAssetGroup group;

						if (args.Group == "Create New Group")
						{
							if (args.GroupName.IsNullOrWhitespace()) return;

							group = addressableAssetSettings.FindGroup(args.GroupName);

							if (group == null)
							{
								group = addressableAssetSettings.CreateGroup(args.GroupName, false, false, false, null);
							}
						}
						else
						{
							group = addressableAssetSettings.FindGroup(args.Group);

							if (group == null)
							{
								group = addressableAssetSettings.CreateGroup(args.Group, false, false, false, null);
							}
						}

						OdinAddressableUtility.MakeAddressable(obj, group);
					}, false).WithModifyRuleDataContextClick<CheckDuplicateBundleDependenciesValidator>("Ignore", data =>
					{
						data.IgnoredGUIDs.Add(dependencyGUID.ToString());
					}).SetSelectionObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(dependencyGUID.ToString())));
				}
			}
		}

		private string DrawGUIDEntry(string guid)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			EditorGUILayout.TextArea(assetPath, SirenixGUIStyles.MultiLineLabel);
			EditorGUILayout.TextField(guid);
			return guid;
		}

		private enum FixChoice
		{
			AddToGroup,
			Ignore,
		}

		private class FixArgs
		{
			[EnumToggleButtons, HideLabel]
			public FixChoice FixChoice;

			[PropertySpace(10)]
			[ValueDropdown("Groups")]
			//[Title("Group To Add To", TitleAlignment = TitleAlignments.Centered)]
			[ShowIf(nameof(FixChoice), FixChoice.AddToGroup, Animate = false)]
			public string Group = "Duplicate Asset Isolation";

			[ValidateInput(nameof(ValidateGroupName), "The group name cannot be empty")]
			[ShowIf(nameof(ShowNewGroupName), Animate = false)]
			public string GroupName;

			[LabelWidth(120f)]
			[PropertySpace(10)]
			[ShowIf("FixChoice", FixChoice.Ignore, Animate = false)]
			public bool IgnoreForEveryone = true;

			[OnInspectorGUI]
			[PropertySpace(10)]
			[DetailedInfoBox("Note that duplicate assets may not always be an issue", "Note that duplicate assets may not always be an issue. If assets will never be requested by the same set of users (for example, region-specific assets), then duplicate dependencies may be desired, or at least inconsequential. Each Project is unique, so fixing duplicate asset dependencies should be evaluated on a case by case basis")]
			private void Dummy() { }

			private bool ShowNewGroupName => this.FixChoice != FixChoice.Ignore && this.Group == "Create New Group";

			private bool ValidateGroupName() => !this.GroupName.IsNullOrWhitespace();

			private IEnumerable<string> Groups()
			{
				var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

				return addressableAssetSettings == null
					? Enumerable.Empty<string>()
					: addressableAssetSettings.groups
						.Where(group => group != null && group.Name != "Built In Data")
						.Select(group => group.Name)
						.Append("Duplicate Asset Isolation")
						.Prepend("Create New Group");
			}
		}
	}
}

#endif