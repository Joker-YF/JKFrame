//-----------------------------------------------------------------------
// <copyright file="OdinAddressableReflectionValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR
#if SIRENIX_INTERNAL
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Modules.Addressables.Editor.Internal;

[assembly: RegisterValidator(typeof(OdinAddressableReflectionValidator))]

namespace Sirenix.OdinInspector.Modules.Addressables.Editor.Internal
{
	public class OdinAddressableReflectionValidator : GlobalValidator
	{
		public override IEnumerable RunValidation(ValidationResult result)
		{
			OdinAddressableReflection.EnsureConstructed();

			FieldInfo[] fields = typeof(OdinAddressableReflection).GetFields(BindingFlags.Static | BindingFlags.Public);

			for (var i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsLiteral)
				{
					continue;
				}

				if (fields[i].GetValue(null) != null)
				{
					continue;
				}

				result.AddError($"[Odin Addressable Module]: {fields[i].Name} was not found.");
			}

			return null;
		}
	}
}
#endif
#endif