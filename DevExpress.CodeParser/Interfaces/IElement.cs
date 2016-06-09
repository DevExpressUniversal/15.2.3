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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface IElement
	{
		void Accept(IElementVisitor visitor);
		IElement GetParent(LanguageElementType type, params LanguageElementType[] types);
		IElement GetParentStatementOrVariable();
		IElement FindChildByName(string name);
		bool IsParentedBy(IElement element);
		IElement Clone();
		IElement Clone(ElementCloneOptions options);
		IElement GetDeclaration();
		IElement GetDeclaration(bool restore);
		LanguageElement ToLanguageElement();
		IElementCollection FindAllReferences();
		IElementCollection FindAllReferences(IElement scope);
		string Name { get; }
		IProjectElement Project { get; }
		ISolutionElement Solution { get; }
		IAssemblyModel AssemblyModel { get; }
		IElement Parent { get; }
		ITypeElement ParentType { get; }
		IEventElement ParentEvent { get; }
		IMethodElement ParentMethod { get; }
		IElement ParentMethodOrAccessor { get; }
		IElement ParentMethodOrPropertyOrEvent { get; }
		IMemberElement ParentMember { get; }
		IPropertyElement ParentProperty { get; }
		INamespaceElement ParentNamespace { get; }
		IElementCollection Children { get; }
		IEnumerable<IElement> AllChildren { get; }
		IElementCollection CodeChildren { get; }
		ISourceFileCollection Files { get; }
		ISourceFile FirstFile { get; }
		ITextRangeCollection NameRanges { get; }
		TextRange FirstNameRange { get; }
		ITextRangeCollection Ranges { get; }
		TextRange FirstRange { get; }
		LanguageElementType ElementType { get; }
		string FullName { get; }		
		string RootNamespaceFullName { get; }
		IElement NextSibling { get; }
		IElement PreviousSibling { get; }
		bool CompletesPrevious { get; }
		int ImageIndex { get; }
		bool InReferencedAssembly { get; }
		bool IsMember { get; }
		bool IsNestedType { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool IsFakeNode { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool IsTypeDeclaration { get; }
	}
}
