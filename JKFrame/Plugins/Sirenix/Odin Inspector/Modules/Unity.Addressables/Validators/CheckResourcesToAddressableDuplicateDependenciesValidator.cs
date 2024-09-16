//-----------------------------------------------------------------------
// <copyright file="CheckResourcesToAddressableDuplicateDependenciesValidator.cs" company="Sirenix ApS">
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
using Sirenix.Utilities.Editor;
using System.Collections;
using System;
using System.IO;
using Sirenix.OdinValidator.Editor;
using Sirenix.OdinInspector.Modules.Addressables.Editor;

[assembly: RegisterValidationRule(typeof(CheckResourcesToAddressableDuplicateDependenciesValidator),
	Description = "This validator identifies dependencies that are duplicated in both addressable groups and the \"Resources\" folder.\n\n" +
				"These duplications mean that data will be included in both the application build and the addressables build.\n\n" +
				"You can decide to simply ignore these duplicated dependencies if this behavior is desired, or use the provided fix " +
				"to move the asset outside of the \"Resources\" folder.")]

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
	public class CheckResourcesToAddressableDuplicateDependenciesValidator : GlobalValidator
	{
		[Tooltip("The severity of the validation result.")]
		public ValidatorSeverity ValidatorSeverity = ValidatorSeverity.Warning;

		[Tooltip("Assets to ignore when validating.")]
		[LabelText("Ignored GUIDs"), CustomValueDrawer(nameof(DrawGUIDEntry))]
		public List<string> IgnoredGUIDs = new List<string>();

		public override IEnumerable RunValidation(ValidationResult result)
		{
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
						if (!IsInsideResourcesFolder(dependencyAssetPath)) continue;

						result.Add(this.ValidatorSeverity, $"{dependencyAssetPath} is duplicated in addressable data and resource folders.")
							.WithFix<FixArgs>(args =>
							{
								if (args.FixChoice == FixChoice.Ignore)
								{
									var sourceType = args.IgnoreForEveryone ? ConfigSourceType.Project : ConfigSourceType.Local;
									var data = RuleConfig.Instance.GetRuleData<CheckResourcesToAddressableDuplicateDependenciesValidator>(sourceType);
									data.IgnoredGUIDs.Add(dependencyGUID.ToString());
									RuleConfig.Instance.SetAndSaveRuleData(data, sourceType);
									return;
								}

								if (!ValidNewFolder(args.NewFolder, out _)) return;

								if (!AssetDatabase.IsValidFolder(args.NewFolder))
								{
									Directory.CreateDirectory(new DirectoryInfo(args.NewFolder).FullName);
									AssetDatabase.Refresh();
								}

								var newPath = $"{args.NewFolder}/{Path.GetFileName(dependencyAssetPath)}";
								AssetDatabase.MoveAsset(dependencyAssetPath, newPath);
							}, false).WithModifyRuleDataContextClick<CheckResourcesToAddressableDuplicateDependenciesValidator>("Ignore", data =>
							{
								data.IgnoredGUIDs.Add(dependencyGUID.ToString());
							}).SetSelectionObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(dependencyGUID.ToString())));

						yield break;
					}
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

		private static bool IsInsideResourcesFolder(string path)
		{
			var pathElements = path.Split('/');

			foreach (var pathElement in pathElements)
			{
				if (pathElement.Equals("Resources", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		private static bool ValidNewFolder(string path, out string message)
		{
			if (IsInsideResourcesFolder(path))
			{
				message = "The asset cannot be moved into a 'Resources' folder";
				return false;
			}

			if (!path.StartsWith("Assets/"))
			{
				message = "The asset must be inside the 'Assets' folder";
				return false;
			}

			message = "The folder is valid";
			return true;
		}

		private enum FixChoice
		{
			MoveAsset,
			Ignore,
		}
		private class FixArgs
		{
			[HideLabel]
			[EnumToggleButtons]
			public FixChoice FixChoice;

			[FolderPath]
			[PropertySpace(10)]
			[ValidateInput(nameof(ValidateFolderPath))]
			[ShowIf("FixChoice", FixChoice.MoveAsset, Animate = false)]
			public string NewFolder = "Assets/Resources_moved";

			[LabelWidth(120f)]
			[PropertySpace(10)]
			[ShowIf("FixChoice", FixChoice.Ignore, Animate = false)]
			public bool IgnoreForEveryone = true;

			private bool ValidateFolderPath(string path, ref string message)
			{
				return ValidNewFolder(path, out message);
			}
		}
	}
}

#endif