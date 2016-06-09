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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VBTypeDeclarationCodeGen : TypeDeclarationCodeGenBase 
	{
		public VBTypeDeclarationCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
	void GenerateAncestorTypes(TypeDeclaration type)
	{
	  if (type == null)
		return;
	  if (type.PrimaryAncestorType != null)
	  {
		Write(FormattingTokenType.Inherits);
		CodeGen.GenerateElement(type.PrimaryAncestorType);
		if (!(type is Interface))
		  CodeGen.AddNewLineIfNeeded();
	  }
	  if (type.SecondaryAncestorTypes.Count == 0) {
		  if (type.PrimaryAncestorType != null)
			  CodeGen.AddNewLineIfNeeded();
		  return;
	  }
	  if (type is Interface)
		Write(FormattingTokenType.Comma);
	  else
		Write(FormattingTokenType.Implements);
	  GenerateElementCollection(type.SecondaryAncestorTypes, FormattingTokenType.Comma);
	  CodeGen.AddNewLineIfNeeded();
	}
		void GenerateTypeDeclaration(TypeDeclaration type, FormattingTokenType keyword)
		{
			GenerateTypeDeclaration(type, keyword, true, true);
		}
	bool WriteComment(CodeElement element)
	{
	  if (element == null)
		return false;
	  NodeList details = element.DetailNodes;
	  if (details == null)
		return false; ;
	  int count = details.Count;
	  if (count == 0)
		return false;
	  bool wasComment = false;
	  for (int i = 0; i < count; i++)
	  {
		Comment com = details[i] as Comment;
		if (com == null)
		  break;
		if (!wasComment)
		  CodeGen.AddWSIfNeeded();
		wasComment = true;
		CodeGen.GenerateElement(com);
	  }
	  return wasComment;
	}
	void GenerateTypeDeclaration(TypeDeclaration type, FormattingTokenType keyword, bool isGeneric, bool hasAncestors)
	{
	  ((VBMemberCodeGen)CodeGen.MemberGen).GenerateMemberVisibility(type);
	  if (type.IsPartial)
		Write(FormattingTokenType.Partial);
	  ((VBMemberCodeGen)CodeGen.MemberGen).GenerateAccessSpecifiers(type.AccessSpecifiers, true);
	  Write(keyword);
	  CodeGen.AddWSIfNeeded();
	  Write(FormattingTokenType.Ident);
	  WriteComment(type);
	  if (isGeneric && type.IsGeneric)
		CodeGen.GenerateElement(type.GenericModifier);
	  CodeGen.AddNewLineIfNeeded();
	  using (GetIndent())
	  {
		if (hasAncestors)
		  GenerateAncestorTypes(type);
		if (type.GenerateBlock)
		  GenerateElementCollection(type.Nodes, FormattingTokenType.None, true);
	  }
	  if(type.GenerateBlock)
	  {
		CodeGen.AddNewLineIfNeeded();
		Write(FormattingTokenType.End);
		CodeGen.AddWSIfNeeded();
		Write(keyword);
	  }
	}
		protected override void GenerateModuleDeclaration(Module type)
		{
			GenerateTypeDeclaration(type, FormattingTokenType.Module, false, false);
		}
		protected override void GenerateClassDeclaration(Class type) 
		{
			GenerateTypeDeclaration(type, FormattingTokenType.Class);
		}
		protected override void GenerateStructDeclaration(Struct type) 
		{
			GenerateTypeDeclaration(type, FormattingTokenType.Structure);
		}
		protected override void GenerateInterfaceDeclaration(Interface type) 
		{
			GenerateTypeDeclaration(type, FormattingTokenType.Interface);
		}
		protected override void GenerateEnumDeclaration(Enumeration type) 
		{
			GenerateTypeDeclaration(type, FormattingTokenType.Enum, false, false);
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
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.MustInherit:
		case FormattingTokenType.Inherits:
		case FormattingTokenType.Implements:
		case FormattingTokenType.NotInheritable:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  return result;
	}
	}
}
