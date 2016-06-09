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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface ITypeElement : IMemberElement, IGenericElement
	{
		ITypeReferenceExpression PrimaryAncestor { get; }
		ITypeReferenceExpressionCollection SecondaryAncestors { get; }
		IMemberElementCollection Members { get; }
	bool IsTypeParameter{ get; }
	ITypeElement[] GetAllDescendants();
	ITypeElement[] GetAllDescendants(IElement scope);
	ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver);
	ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, IElement scope);
		ITypeElement GetBaseType();
		ITypeElement GetBaseType(ISourceTreeResolver resolver);
		ITypeElement[] GetBaseTypes();
		ITypeElement[] GetBaseTypes(ISourceTreeResolver resolver);
		ITypeElement[] GetDescendants();
		ITypeElement[] GetDescendants(IElement scope);
		ITypeElement[] GetDescendants(ISourceTreeResolver resolver);
		ITypeElement[] GetDescendants(ISourceTreeResolver resolver, IElement scope);
		bool Is(string fullTypeName);
		bool Is(ITypeElement parentElement);
		bool Is(Type type);
		bool Is(ISourceTreeResolver resolver, string fullTypeName);
		bool Is(ISourceTreeResolver resolver, ITypeElement parentElement);
		bool Is(ISourceTreeResolver resolver, Type type);
		bool DescendsFrom(string fullTypeName);
		bool DescendsFrom(ITypeElement parentElement);
		bool DescendsFrom(Type type);
		bool DescendsFrom(ISourceTreeResolver resolver, string fullTypeName);
		bool DescendsFrom(ISourceTreeResolver resolver, ITypeElement parentElement);
		bool DescendsFrom(ISourceTreeResolver resolver, Type type);
		IMemberElement FindMember(string name);
		IMemberElement FindMember(string name, bool searchInAncestors);
		IMemberElement FindMember(string name, IElementFilter filter, bool searchInAncestors);
		IMemberElementCollection FindMembers(string name);
	IMemberElementCollection FindMembers(ISourceTreeResolver resolver, string name);
		IMemberElementCollection FindMembers(string name, bool searchInAncestors);
		IMemberElementCollection FindMembers(string name, IElementFilter filter, bool searchInAncestors);
	}
  public interface ITypeElementModifier : IMemberElementModifier, IGenericElementModifier
  {
	void SetPrimaryAncestor(ITypeReferenceExpression type);
	void AddSecondaryAncestor(ITypeReferenceExpression type);
	void AddMember(IMemberElement member);
	void RemoveMember(IMemberElement member);
  }
}
