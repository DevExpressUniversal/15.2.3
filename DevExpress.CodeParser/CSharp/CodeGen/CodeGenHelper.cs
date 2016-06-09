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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public sealed class CodeGenHelper
	{
	struct AccessSpecifierInfo : IComparable<AccessSpecifierInfo>
	{
	  public SourcePoint Point;
	  public FormattingTokenType KeyWord;
	  int _Index;
	  public AccessSpecifierInfo(SourceRange range, FormattingTokenType keyWord, int index)
	  {
		Point = range.Start;
		KeyWord = keyWord;
		_Index = index;
	  }
	  public int CompareTo(AccessSpecifierInfo other)
	  {
		int result = Point.CompareTo(other.Point);
		if (result == 0)
		  return _Index.CompareTo(other._Index);
		return result;
	  }
	}
		private CodeGenHelper()	{}
		static bool HasTypeParameterConstraints(TypeParameterCollection typeParams)
		{
			if (typeParams == null)
				return false;
			int count = typeParams.Count;
			for (int i = 0; i < count; i++)
			{
				TypeParameter param = typeParams[i];
				if (param == null)
					continue;
				if (HasTypeParameterConstraints(param))
					return true;
			}
			return false;
		}		
	public static void GenerateAccessSpecifiers(LanguageElementCodeGenBase leGen) 
		{
	  AccessSpecifiedElement accessElement = leGen.Context as AccessSpecifiedElement;
	  if (accessElement == null)
		return;
	  AccessSpecifiers access = accessElement.AccessSpecifiers;
			if (access == null)
				return;
	  List<AccessSpecifierInfo> infos = new List<AccessSpecifierInfo>();
	  LanguageElement context = leGen.Context;
	  if (access.IsAbstract)
		infos.Add(new AccessSpecifierInfo(access.AbstractRange, FormattingTokenType.Abstract, 1));
	  if (access.IsExtern)
		infos.Add(new AccessSpecifierInfo(access.ExternRange, FormattingTokenType.Extern, 2));
	  if (access.IsNew)
		infos.Add(new AccessSpecifierInfo(access.NewRange, FormattingTokenType.New, 3));
	  if (access.IsUnsafe)
		infos.Add(new AccessSpecifierInfo(access.UnsafeRange, FormattingTokenType.Unsafe, 4));
	  if (access.IsStatic)
		infos.Add(new AccessSpecifierInfo(access.StaticRange, FormattingTokenType.Static, 5));
	  if (access.IsReadOnly && (context == null || context.ElementType != LanguageElementType.Property))
		infos.Add(new AccessSpecifierInfo(access.ReadOnlyRange, FormattingTokenType.ReadOnly, 6));
	  if (access.IsSealed)
		infos.Add(new AccessSpecifierInfo(access.SealedRange, FormattingTokenType.Sealed, 7));
	  if (access.IsOverride)
		infos.Add(new AccessSpecifierInfo(access.OverrideRange, FormattingTokenType.Override, 8));
	  if (access.IsVirtual)
		infos.Add(new AccessSpecifierInfo(access.VirtualRange, FormattingTokenType.Virtual, 9));
	  if (access.IsVolatile)
		infos.Add(new AccessSpecifierInfo(access.VolatileRange, FormattingTokenType.Volatile, 10));
	  if (access.IsPartial)
		infos.Add(new AccessSpecifierInfo(access.PartialRange, FormattingTokenType.Partial, 11));
	  if (access.IsAsynchronous)
		infos.Add(new AccessSpecifierInfo(access.AsynchronousRange, FormattingTokenType.Async, 12));
	  infos.Sort();
	  foreach (AccessSpecifierInfo info in infos)
		leGen.Write(info.KeyWord);
		}
	public static void GenerateVisibility(LanguageElementCodeGenBase leGen)
	{
	  AccessSpecifiedElement member = leGen.Context as AccessSpecifiedElement;
	  if (member == null)
		return;
	  GenerateVisibility(leGen, member.Visibility);
	}
	public static void GenerateVisibility(LanguageElementCodeGenBase leGen, MemberVisibility visibility)
	{
	  AccessSpecifiedElement member = leGen.Context as AccessSpecifiedElement;
	  if (member != null && member.IsDefaultVisibility)
		return;
	  switch (visibility)
	  {
		case MemberVisibility.Public:
		  leGen.Write(FormattingTokenType.Public);
		  return;
		case MemberVisibility.Private:
		  leGen.Write(FormattingTokenType.Private);
		  return;
		case MemberVisibility.Protected:
		  leGen.Write(FormattingTokenType.Protected);
		  return;
		case MemberVisibility.Internal:
		  leGen.Write(FormattingTokenType.Internal);
		  return;
		case MemberVisibility.ProtectedInternal:
		  leGen.Write(FormattingTokenType.Protected);
		  leGen.Write(FormattingTokenType.Internal);
		  return;
	  }
	}
		public static bool HasTypeParameterConstraints(GenericModifier modifier)
		{
			if (modifier == null)
				return false;
			return HasTypeParameterConstraints(modifier.TypeParameters);
		}
		public static bool HasTypeParameterConstraints(TypeParameter param)
		{
			if (param == null)
				return false;
			return param.Constraints != null && param.Constraints.Count > 0;
		}
		public static void GenerateExpressionListForMethodCall(ExpressionCollection expressions, string delimiter, CodeGen codeGen)
		{
			if (expressions == null || expressions.Count == 0 || codeGen == null)
				return;
			for (int i = 0; i < expressions.Count; i++)
			{
				Expression exp = expressions[i] as Expression;
				if (exp == null)
					continue;
		if (!string.IsNullOrEmpty(delimiter) && i != 0)
		  codeGen.Code.Write(delimiter);
				codeGen.GenerateElement(exp);
			}
		}
	const string STR_Public = "public";
	const string STR_Private = "private";
	const string STR_Protected = "protected";
	const string STR_Internal = "internal";
	const string STR_ProtectedInternal = "protected internal";
	public static string GetVisibility(MemberVisibility visibility)
	{
	  switch (visibility)
	  {
		case MemberVisibility.Public:
		  return STR_Public;
		case MemberVisibility.Private:
		  return STR_Private;
		case MemberVisibility.Protected:
		  return STR_Protected;
		case MemberVisibility.Internal:
		  return STR_Internal;
		case MemberVisibility.ProtectedInternal:
		  return STR_ProtectedInternal;
	  }
	  return string.Empty;
	}
	}
}
