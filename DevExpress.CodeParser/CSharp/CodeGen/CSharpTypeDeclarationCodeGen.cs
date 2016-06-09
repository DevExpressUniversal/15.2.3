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
	public class CSharpTypeDeclarationCodeGen : TypeDeclarationCodeGenBase 
	{
		public CSharpTypeDeclarationCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
	# region  GenerateCode
	public override void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  if (CodeGen == null)
		throw new ArgumentNullException("CodeGen is Null");
	  PushCodeWriter();
	  SetCodeWriter(writer);
	  if (calculateIndent)
		CalculateIndent(languageElement);
	  CodeGen.GenerateElement(languageElement);
	  if (calculateIndent)
		ResetIndent();
	  PopCodeWriter();
	}
	#endregion
	FormattingTokenType GetDeclarationType(TypeDeclaration type)
	{
	  if (type == null)
		return FormattingTokenType.None;
	  LanguageElementType elementType = type.ElementType;
	  switch (elementType)
	  {
		case LanguageElementType.Class:
		  return FormattingTokenType.Class;
		case LanguageElementType.Interface:
		  return FormattingTokenType.Interface;
		case LanguageElementType.Struct:
		  return FormattingTokenType.Struct;
		case LanguageElementType.Enum:
		  return FormattingTokenType.Enum;
	  }
	  return FormattingTokenType.None;
	}
	void GenerateUnderlyingType(Enumeration enumeration)
	{
	  string underlyingType = enumeration.UnderlyingType;
	  if (string.IsNullOrEmpty(underlyingType))
		return;
	  Write(FormattingTokenType.Colon);
	  Write(underlyingType);
	}
	void GenerateAncestorTypes(TypeDeclaration type)
	{
	  if (type == null)
		return;
	  if (type.ElementType == LanguageElementType.Enum)
	  {
		GenerateUnderlyingType((Enumeration)type);
		return;
	  }
	  if (type.PrimaryAncestorType == null && type.SecondaryAncestorTypes.Count == 0)
		return;
	  Write(FormattingTokenType.Colon);
	  LanguageElementCollection coll = new LanguageElementCollection();
	  if (type.PrimaryAncestorType != null)
		coll.Add(type.PrimaryAncestorType);
	  coll.AddRange(type.SecondaryAncestorTypes);
	  GenerateElementCollection(coll, FormattingTokenType.Comma);
	}
	void GenerateType(TypeDeclaration type)
	{
	  GenerateType(type, FormattingTokenType.None);
	}
	void GenerateType(TypeDeclaration type, FormattingTokenType delimiter)
	{
	  MemberVisibility typeVisibility = type.Visibility;
	  if (typeVisibility == MemberVisibility.Illegal && type.IsDefaultVisibility)
		typeVisibility = type.Parent is TypeDeclaration ? MemberVisibility.Private : MemberVisibility.Internal;
	  CodeGenHelper.GenerateVisibility(this);
	  CodeGenHelper.GenerateAccessSpecifiers(this);
	  Write(GetDeclarationType(type));
	  Write(FormattingTokenType.Ident);
	  if (type.IsGeneric)
		CodeGen.GenerateElement(type.GenericModifier);
	  GenerateAncestorTypes(type);
	  if (type.IsGeneric)
		CodeGen.GenerateElement(type.GenericModifier);
	  if (type.GenerateBlock)
	  {
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(type.Nodes, delimiter);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	  if (type.HasEndingSemicolon)
		Write(FormattingTokenType.Semicolon);
	}
		protected override void GenerateModuleDeclaration(Module type)
		{
		}
		protected override void GenerateClassDeclaration(Class type) 
		{
			GenerateType(type);
		}
		protected override void GenerateStructDeclaration(Struct type) 
		{
			GenerateType(type);
		}
		protected override void GenerateInterfaceDeclaration(Interface type) 
		{
			GenerateType(type);
		}
		protected override void GenerateEnumDeclaration(Enumeration type) 
		{
			GenerateType(type, FormattingTokenType.Comma);
		}
		protected override void GenerateUnionDeclaration(Union type) 
		{
		}
		protected override void GenerateManagedClassDeclaration(ManagedClass type) 
		{
		}
		protected override void GenerateManagedStructDeclaration(ManagedStruct type) 
		{
		}
		protected override void GenerateInterfaceClassDeclaration(InterfaceClass type) 
		{
		}
		protected override void GenerateInterfaceStructDeclaration(InterfaceStruct type) 
		{
		}
		protected override void GenerateValueClassDeclaration(ValueClass type) 
		{
		}
		protected override void GenerateValueStructDeclaration(ValueStruct type) 
		{
		}
	protected override void GenerateMembers(NodeList nodeList)
	{
	  GenerateElementCollection(nodeList);
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Colon:
		  if (Context is TypeDeclaration)
		  {
			if (Options.WrappingAlignment.WrapBeforeColonForAncestorsList)
			  result.AddNewLine();
			else if (Options.Spacing.BeforeColonForAncestorsListInTypeDeclaration)
			  result.AddWhiteSpace();
		  }
		  break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Class:
		case FormattingTokenType.Struct:
		case FormattingTokenType.Interface:
		case FormattingTokenType.Enum:
		case FormattingTokenType.Partial:
		case FormattingTokenType.Abstract:
		case FormattingTokenType.Override:
		case FormattingTokenType.ReadOnly:
		case FormattingTokenType.Sealed:
		case FormattingTokenType.Static:
		case FormattingTokenType.Unsafe:
		case FormattingTokenType.Virtual:
		case FormattingTokenType.Volatile:
		case FormattingTokenType.Out:
		case FormattingTokenType.In:
		case FormattingTokenType.New:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Colon:
		  if (Options.Spacing.AfterColonForAncestorsListInTypeDeclaration)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Comma:
		  if (ContextMatch(LanguageElementType.Enum))
			result.AddNewLine();
		  break;
	  }
	  return result;
	}
  }
}
