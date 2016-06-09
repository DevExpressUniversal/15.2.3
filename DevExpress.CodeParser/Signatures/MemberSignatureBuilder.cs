#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class MemberSignatureBuilder
	{
		#region private constants...
		const char CHAR_AccentChar = '`';
		const char CHAR_Comma = ',';
		const char CHAR_Space = ' ';
		const string STR_TypeSeparator = " : ";
		const char CHAR_OpenParen = '(';
		const char CHAR_CloseParen = ')';
		const char CHAR_OpenSquareBracket = '[';
		const char CHAR_CloseSquareBracket = ']';
		const char CHAR_LessThan = '<';
		const char CHAR_GreaterThan = '>';
		const string STR_ConstructorName = ".ctor";
	const string STR_DelegateName = "delegate";
		const string STR_DestructorName = "Finalize";
		const string STR_DoubleColons = "::";
		const string STR_Reference = "&";
		const string STR_Void = "void";
		const string CHAR_Dot = ".";
		#endregion
	#region ArrayIsNullOrEmpty
	static bool ArrayIsNullOrEmpty(ICollection collection)
	{
	  return collection == null || collection.Count == 0;
	}
	#endregion
		#region MethodHasNoReturnType
		static bool MethodHasNoReturnType(IMethodElement method)
		{
			return method != null && 
				(method.ElementType == LanguageElementType.PropertyAccessorSet ||
				method.ElementType == LanguageElementType.EventAdd ||
				method.ElementType == LanguageElementType.EventRemove ||
				method.ElementType == LanguageElementType.EventRaise);
		}
		#endregion
	#region AppendType(StringBuilder builder, IHasType hasType)
	static void AppendType(StringBuilder builder, IHasType hasType)
	{
	  AppendType(builder, hasType, true);
	}
	#endregion
	#region AppendType(StringBuilder builder, IHasType hasType)
	static void AppendType(StringBuilder builder, IHasType hasType, bool addVoid)
	{
	  if (builder == null || hasType == null)
		return;
	  ITypeReferenceExpression type = hasType.Type;
	  if (type == null || MethodHasNoReturnType(hasType as IMethodElement))
	  {
		if (addVoid)
		  builder.Append(STR_Void);
	  }
	  else
		builder.Append(ExpressionSignatureBuilder.GetSignature(type));
	}
	#endregion
		#region AppendAccessorParameters
		static void AppendAccessorParameters(StringBuilder builder, IMemberElement member, IMethodElement accessor)	
		{
			IWithParameters withParameters = member as IWithParameters;
			switch (accessor.ElementType)
			{
				case LanguageElementType.PropertyAccessorGet:
					if (withParameters != null)
						AppendParameters(builder, withParameters.Parameters);
					break;
				case LanguageElementType.PropertyAccessorSet:
					AppendType(builder, member as IHasType);
					if (withParameters != null && !ArrayIsNullOrEmpty(withParameters.Parameters))
					{
						builder.Append(CHAR_Comma);
						builder.Append(CHAR_Space);
						AppendParameters(builder, withParameters.Parameters);
					}		  
					break;
				case LanguageElementType.EventAdd:		
				case LanguageElementType.EventRemove:		  
				case LanguageElementType.EventRaise:		  
					AppendType(builder, member as IHasType);
					break;
			}
		}
		#endregion
		#region AppendParameters(StringBuilder builder, IParameterElementCollection parameters)
	static void AppendParameters(StringBuilder builder, IParameterElementCollection parameters)
	{
	  AppendParameters(builder, parameters, true);
	}
	#endregion
	private static bool IsActivatedGenericParameter(IParameterElement parameter)
	{
	  if (parameter == null || parameter.Type == null)
		return false;
	  ITypeElement parentType = parameter.ParentType;
	  if (parentType == null || parentType.TypeParameters == null || parentType.TypeParameters.Count == 0)
		return false;
	  for (int i = 0; i < parentType.TypeParameters.Count; i++)
	  {
		ITypeParameter typeParameter = parentType.TypeParameters[i];
		if (typeParameter == null || !typeParameter.IsActivated || typeParameter.ActivatedType == null)
		  continue;
		if (parameter.Type.Name == typeParameter.ActivatedType.Name)
		  return true;
	  } 
	  return false;
	}
	#region AppendParameters(StringBuilder builder, IParameterElementCollection parameters, bool addSpaceSplitter)
	static void AppendParameters(StringBuilder builder, IParameterElementCollection parameters, bool addSpaceSplitter)
	{
	  if (builder == null || ArrayIsNullOrEmpty(parameters))
		return;
	  int count = parameters.Count;
	  int genericParameterIndex = 0;
	  for (int i = 0; i < count; i++)
	  {
		IParameterElement parameter = parameters[i];
		if (IsActivatedGenericParameter(parameter))
		{
		  builder.Append(String.Format("`{0}", genericParameterIndex));
		  genericParameterIndex++;
		}
		else
		  AppendType(builder, parameter);
		if (parameter.Direction == ArgumentDirection.Out || parameter.Direction == ArgumentDirection.Ref)
		  builder.Append(STR_Reference);
		if (i < count - 1)
		{
		  builder.Append(CHAR_Comma);
		  if (addSpaceSplitter)
			builder.Append(CHAR_Space);
		}
	  }
	}
	#endregion
	#region AppendParameters(StringBuilder builder, IMethodElement method)
	static void AppendParameters(StringBuilder builder, IMethodElement method)
	{
	  AppendParameters(builder, method, true);
	}
	#endregion
	#region AppendParameters(StringBuilder builder, IMethodElement method, bool addSpaceSplitter)
	static void AppendParameters(StringBuilder builder, IMethodElement method, bool addSpaceSplitter)
	{
	  IMemberElement parentMember = method.Parent as IMemberElement;
	  if (IsAccessor(method) && CanContainAccessor(parentMember))
		AppendAccessorParameters(builder, parentMember, method);
	  else
		AppendParameters(builder, method.Parameters, addSpaceSplitter);
	}
	#endregion
	#region AppendParameters(StringBuilder builder, IDelegateElement del)
	static void AppendParameters(StringBuilder builder, IDelegateElement del)
	{ 
	  AppendParameters(builder, del.Parameters);
	}
	#endregion
	#region AppendParentType
	private static void AppendParentType(StringBuilder builder, IMemberElement member)
	{
	  ITypeElement parentType = member.ParentType;
	  if (parentType == null)
		return;
	  const string DOT = ".";
	  string parentTypeFullName = parentType.FullName;
	  builder.Append(parentTypeFullName + DOT);
	}
	#endregion
	#region BuildSignaturePart(string baseName, ITypeParameterCollection typeParameters, bool buildGenerics)
	static string BuildSignaturePart(string baseName, ITypeParameterCollection typeParameters, bool buildGenerics)
		{
			int count = 0;
			if (typeParameters != null)
				count = typeParameters.Count;
	  if (count == 0)
		return baseName;
	  if (!buildGenerics)
			  return BuildSignaturePart(baseName, count);
	  StringBuilder builder = new StringBuilder();
	  builder.Append(baseName);
	  builder.Append(CHAR_LessThan);
	  if (typeParameters != null)
	  {
		for (int i = 0; i < count; i++)
		{
		  builder.Append(typeParameters[i].Name);
		  if (i < count - 1)
			builder.Append(", ");
		}
	  }
	  builder.Append(CHAR_GreaterThan);
	  return builder.ToString();
		}
		#endregion
		#region BuildSignaturePart(string baseName, int paramsCount)
		static string BuildSignaturePart(string baseName, int paramsCount)
		{
			if (paramsCount == 0)
				return baseName;
			return String.Format("{0}{1}{2}", baseName, CHAR_AccentChar, paramsCount);
		}
		#endregion
		#region BuildSignaturePart(IMemberElement member, bool buildGenerics, ref string name, ref int nameIndex)
		static string BuildSignaturePart(IMemberElement member, bool buildGenerics, ref string name, ref int nameIndex)
		{
			if (member == null)
				return String.Empty;
			name = member.Name;
			nameIndex = 0;
			if (member is IMethodElement)
			{
				IMethodElement method = (IMethodElement)member;
				if (method.IsConstructor)
					name = STR_ConstructorName;
				else if (method.IsDestructor)
					name = STR_DestructorName;
			}
			string nameWithQualifier = name;
			if (member.NameQualifier != null && member.NameQualifier is IReferenceExpression)
			{
				IReferenceExpression refExp = (IReferenceExpression)member.NameQualifier;					
				nameWithQualifier = refExp.FullSignature + STR_DoubleColons + name;
				nameIndex += refExp.FullSignature.Length + STR_DoubleColons.Length;
			}
			if (member is IGenericElement)
				name = BuildSignaturePart(name, ((IGenericElement)member).TypeParameters, buildGenerics);
			return name;
		}
		#endregion		
		#region CanContainAccessor
		static bool CanContainAccessor(IMemberElement member)
		{
			return member != null && member is IEventElement || member is IPropertyElement;
		}
		#endregion
		#region IsAccessor
		static bool IsAccessor(IMethodElement method)
		{
			return method != null && 
				(method.ElementType == LanguageElementType.PropertyAccessorGet || 
				method.ElementType == LanguageElementType.PropertyAccessorSet ||
				method.ElementType == LanguageElementType.EventAdd ||
				method.ElementType == LanguageElementType.EventRemove ||
				method.ElementType == LanguageElementType.EventRaise);
		}
		#endregion
		#region GetTypeSignature
	static string GetTypeSignature(ITypeElement type, bool buildGenerics, ref string name, ref int nameIndex)
	{
	  if (type is IDelegateElement)
		return GetDelegateSignature((IDelegateElement)type, buildGenerics, ref name, ref nameIndex);
	  return BuildSignaturePart(type, buildGenerics, ref name, ref nameIndex);
	}
	#endregion
	#region GetDelegateSignature
	static string GetDelegateSignature(IDelegateElement delegateElement, bool buildGenerics, ref string name, ref int nameIndex)
	{
	  StringBuilder builder = new StringBuilder();
	  builder.Append(BuildSignaturePart(delegateElement, buildGenerics, ref name, ref nameIndex));			
	  builder.Append(STR_TypeSeparator);
	  AppendType(builder, delegateElement);
	  return builder.ToString();
	}
	#endregion
	#region GetMethodSignature
	static string GetMethodSignature(IMethodElement method, bool buildGenerics, ref string name, ref int nameIndex)
	{
	  StringBuilder builder = new StringBuilder();
	  builder.Append(BuildSignaturePart(method, buildGenerics, ref name, ref nameIndex));
	  builder.Append(CHAR_OpenParen);
	  AppendParameters(builder, method);
	  builder.Append(CHAR_CloseParen);
			if (!method.IsConstructor && !method.IsDestructor)
			{
				builder.Append(STR_TypeSeparator);
				AppendType(builder, method);
			}
	  return builder.ToString();
	}
	#endregion
	#region GetPropertySignature
	static string GetPropertySignature(IPropertyElement property, ref string name, ref int nameIndex)
	{
			name = property.Name;
			nameIndex = 0;
	  StringBuilder builder = new StringBuilder();
			builder.Append(property.Name);
			if (!ArrayIsNullOrEmpty(property.Parameters))
		builder.Append(CHAR_OpenSquareBracket);
			AppendParameters(builder, property.Parameters);
			if (!ArrayIsNullOrEmpty(property.Parameters))
		builder.Append(CHAR_CloseSquareBracket);	  
			builder.Append(STR_TypeSeparator);
	  AppendType(builder, property);
	  return builder.ToString();
	}
	#endregion
	#region GetEventSignature
	static string GetEventSignature(IEventElement eventElement, ref string name, ref int nameIndex)
	{
			name = eventElement.Name;
			nameIndex = 0;
	  StringBuilder builder = new StringBuilder();
	  builder.Append(eventElement.Name);	  
	  builder.Append(STR_TypeSeparator);
	  AppendType(builder, eventElement);
	  return builder.ToString();
	}
	#endregion
		#region GetSignature
		public static string GetSignature(IMemberElement member)
		{
			string name; 
			int nameIndex;
			return GetSignature(member, false, out name, out nameIndex);
		}
		#endregion
	#region GetSignature
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetSignature(IMemberElement member, bool buildGenerics, out string name, out int nameIndex)
	{
			name = String.Empty;
			nameIndex = -1;
	  if (member == null)
		return String.Empty;
			name = member.Name;
			nameIndex = 0;
	  if (member is ITypeElement)
		return GetTypeSignature((ITypeElement)member, buildGenerics, ref name, ref nameIndex);
			if (member is IMethodElement)
		return GetMethodSignature((IMethodElement)member, buildGenerics, ref name, ref nameIndex);
			if (member is IPropertyElement)
		return GetPropertySignature((IPropertyElement)member, ref name, ref nameIndex);
			if (member is IEventElement)
		return GetEventSignature((IEventElement)member, ref name, ref nameIndex);
	  return member.Name;
	}
	#endregion
	internal static string GetSignatureForXmlDocumentation(IMemberElement member, bool buildGenerics, out string name, out int nameIndex)
	{
	  name = String.Empty;
	  nameIndex = -1;
	  if (member == null)
		return String.Empty;
	  name = member.Name;
	  nameIndex = 0;
	  StringBuilder builder = new StringBuilder();
	  if (member is IMethodElement)
	  {
		IMethodElement method = member as IMethodElement;
		builder.Append("M:");
		AppendParentType(builder, member);
		builder.Append(BuildSignaturePart(method, buildGenerics, ref name, ref nameIndex));
		if (!ArrayIsNullOrEmpty(method.Parameters))
		{
		  builder.Append(CHAR_OpenParen);
		  AppendParameters(builder, method, false);
		  builder.Append(CHAR_CloseParen);
		}
	  }
	  else if (member is IPropertyElement)
	  {
		IPropertyElement property = member as IPropertyElement;
		builder.Append("P:");
		AppendParentType(builder, member);
		builder.Append(property.Name);
		if (!ArrayIsNullOrEmpty(property.Parameters))
		  builder.Append(CHAR_OpenSquareBracket);
		AppendParameters(builder, property.Parameters, false);
		if (!ArrayIsNullOrEmpty(property.Parameters))
		  builder.Append(CHAR_CloseSquareBracket);
	  }
	  else if (member is IFieldElement)
	  {
		builder.Append("F:");
		AppendParentType(builder, member);
		builder.Append(name);
	  }
	  else if (member is IEventElement)
	  {
		builder.Append("E:");
		AppendParentType(builder, member);
		builder.Append(name);
	  }
	  return builder.ToString();
	}
  }
}
