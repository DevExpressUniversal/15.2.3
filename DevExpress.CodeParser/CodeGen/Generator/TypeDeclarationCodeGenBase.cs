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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif 
{
	public abstract class TypeDeclarationCodeGenBase : LanguageElementCodeGenBase 
	{
		public TypeDeclarationCodeGenBase(CodeGen codeGen) : base(codeGen) 
		{
		}
		protected virtual bool GenerateTypeDeclaration(TypeDeclaration typeDeclaration) 
		{
			if (typeDeclaration == null)
				return false;
			switch (typeDeclaration.ElementType) 
			{
				case LanguageElementType.Module:
					GenerateModuleDeclaration(typeDeclaration as Module);
					return true;
				case LanguageElementType.Class:
					GenerateClassDeclaration(typeDeclaration as Class);
					return true;
				case LanguageElementType.Struct:
					GenerateStructDeclaration(typeDeclaration as Struct);
					return true;
				case LanguageElementType.ManagedClass:
					GenerateManagedClassDeclaration(typeDeclaration as ManagedClass);
					return true;
				case LanguageElementType.ManagedStruct:
					GenerateManagedStructDeclaration(typeDeclaration as ManagedStruct);
					return true;
				case LanguageElementType.ValueClass:
					GenerateValueClassDeclaration(typeDeclaration as ValueClass);
					return true;
				case LanguageElementType.ValueStruct:
					GenerateValueStructDeclaration(typeDeclaration as ValueStruct);
					return true;
				case LanguageElementType.Interface:
					GenerateInterfaceDeclaration(typeDeclaration as Interface);
					return true;
				case LanguageElementType.InterfaceClass:
					GenerateInterfaceClassDeclaration(typeDeclaration as InterfaceClass);
					return true;
				case LanguageElementType.InterfaceStruct:
					GenerateInterfaceStructDeclaration(typeDeclaration as InterfaceStruct);
					return true;
				case LanguageElementType.Enum:
					GenerateEnumDeclaration(typeDeclaration as Enumeration);
					return true;
				case LanguageElementType.Union:
					GenerateUnionDeclaration(typeDeclaration as Union);
					return true;
			}
			return false;
		}
	protected virtual void GenerateMembers(NodeList nodeList)
	{
	  GenerateElementCollection(nodeList, string.Empty, true);
	}
		protected abstract void GenerateModuleDeclaration(Module type);
		protected abstract void GenerateClassDeclaration(Class type);
		protected abstract void GenerateStructDeclaration(Struct type);
		protected abstract void GenerateManagedClassDeclaration(ManagedClass type);
		protected abstract void GenerateManagedStructDeclaration(ManagedStruct type);
		protected abstract void GenerateInterfaceClassDeclaration(InterfaceClass type);
		protected abstract void GenerateInterfaceStructDeclaration(InterfaceStruct type);
		protected abstract void GenerateValueClassDeclaration(ValueClass type);
		protected abstract void GenerateValueStructDeclaration(ValueStruct type);
		protected abstract void GenerateInterfaceDeclaration(Interface type);
		protected abstract void GenerateEnumDeclaration(Enumeration type);
		protected abstract void GenerateUnionDeclaration(Union type);
	public override void GenerateElement(LanguageElement languageElement)
	{
	  if (languageElement == null)
		return;
	  GenerateTypeDeclaration(languageElement as TypeDeclaration);
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  TypeDeclaration type = element as TypeDeclaration;
	  if (type == null)
		return false;
	  LanguageElement codeSibling = element.NextSibling;
	  if (codeSibling == null)
		return false;
	  if (Options.BlankLines.AfterTypeDeclarations)
		CodeGen.AddNewLine(2);
	  else
		CodeGen.AddNewLineIfNeeded();
	  return true;
	}
	}
}
