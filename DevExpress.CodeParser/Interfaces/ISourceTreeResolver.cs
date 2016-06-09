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
using System.Collections;
using System.Collections.Specialized;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface ISourceTreeResolver
	{
		void BeginResolve();
		void EndResolve();
		bool IsTypeReference(IElement type, IElement start);
		bool IsDeclarationOverriden(IElement declaration, IElement target);
		ArrayList ResolveAncestorInterfaces(ITypeElement type);
		IElement ResolveTypeAncestor(ITypeElement type);
		ITypeElement[] ResolveBaseTypes(ITypeElement type);
		ITypeElement[] ResolveAllBaseTypes(ITypeElement type);
		ArrayList ResolveList(IElement element, StringCollection simpleNames);
		ArrayList Resolve(IElement start, string fullName);
		ArrayList Resolve(IElement start, string fullName, bool isNamespace);
		IElement ResolveFirst(IElement start, string fullName);
		IElement ResolveFirst(IElement start, string fullName, bool isNamespace);
		IElement ResolveType(IElement start, string typeName);
	IElementCollection ResolveTypes(IElement start, string typeName, bool breakOnFirstFound);
		IElement FindLocalDeclaration(IElement element);
		IElement FindLocalDeclaration(IElement start, string name);
		IElement FindFieldDeclaration(IElement element);
		IElementCollection FindConstructors(IObjectCreationExpression element, IExpressionCollection arguments);
		IElementCollection FindConstructors(IConstructorInitializerElement element, IExpressionCollection arguments);
	IElementCollection FindConstructors(IMethodReferenceExpression reference, IExpressionCollection arguments);
	IElementCollection FindConstructors(ITypeElement type, IExpressionCollection arguments);
	IElementCollection FindConstructors(IElement start, ITypeElement type, IExpressionCollection arguments);
		IElement FindFirstDeclaration(IExpression expression);
		IElement FindFirstElementDeclaration(IElement element);
		IElementCollection FindElementDeclarations(IElement element);
		IElementCollection FindMethodDeclarations(IExpression element, IExpressionCollection arguments, bool searchOnlyInCurrentClass, bool filterByArguments);
		IElementCollection FilterExplicitDeclarationsIfNeeded(IElementCollection declarations, IExpression active);
		IElement FindMatchingMember(IElementCollection members, IMemberElement member);
		IElementCollection FindMatchingMembers(IElementCollection members, IMemberElement member, bool ignoreInterfaceName);
		[EditorBrowsable(EditorBrowsableState.Never)]
		IElementCollection FindMatchingMembers(IElementCollection members, IMemberElement member, bool ignoreInterfaceName, bool checkExplicitMembers);
		ITypeElement FindParentType(IElement element);
		IMethodPrototypeElement FindPrototype(IMethodElement method);
	IMethodPrototypeElement[] FindPrototypes(IMethodElement method);
	IFieldElement[] FindPrototypes(IBaseVariable variable);
	[EditorBrowsable(EditorBrowsableState.Never)]
	IElement FindAccessorPrototype(IMethodElement method);
		IFieldElement FindPrototype(IBaseVariable variable);
		IElement FindAttributeDeclaration(IElement element);
		[EditorBrowsable(EditorBrowsableState.Never)]
		IMemberElement GetBaseMember(IMemberElement target);
		IElement Resolve(IElementReferenceExpression expression);
		IElement Resolve(IAddressOfExpression expression);
	IElement Resolve(IAwaitExpression expression);
		IElement Resolve(IArrayCreateExpression expression);
		IElement Resolve(IArgumentDirectionExpression expression);
		IElement Resolve(IAssignmentExpression expression);
		IElement Resolve(ICheckedExpression expression);
		IElement Resolve(IConditionalExpression expression);
		IElement Resolve(IUncheckedExpression expression);
		IElement Resolve(ILogicalInversionExpression expression);
		IElement Resolve(ILogicalOperationExpression expression);
		IElement Resolve(IRelationalOperationExpression expression);
		IElement Resolve(IParenthesizedExpression expression);
		IElement Resolve(IObjectCreationExpression expression);
		IElement Resolve(ITypeCastExpression expression);
		IElement Resolve(IConditionalTypeCast expression);
		IElement Resolve(ITypeCheckExpression expression);
		IElement Resolve(ITypeReferenceExpression expression);
		IElement Resolve(IElement start, PrimitiveType type);
		IElement Resolve(IPrimitiveExpression expression);
		IElement Resolve(IThisReferenceExpression expression);
		IElement Resolve(IBaseReferenceExpression expression);
		IElement Resolve(IIndexerExpression expression);
		IElement Resolve(ITypeOfIsExpression expression);
		IElement Resolve(ISizeOfExpression expression);
		IElement Resolve(IUnaryOperatorExpression expression);
		IElement Resolve(IBinaryOperatorExpression expression);
		IElement Resolve(ITypeOfExpression expression);
		IElement Resolve(IMethodReferenceExpression expression);
		IElement Resolve(IMethodCallExpression expression);
		IElement Resolve(IIsNotExpression expression);
		IElement Resolve(IMemberAccessExpression expression);
		IElement Resolve(IQueryExpression expression);
		IElement Resolve(IDefaultValueExpression expression);
		IElement Resolve(IArrayInitializerExpression expression);
		IElement Resolve(IAnonymousMethodExpression expression);
		IElement Resolve(ILambdaExpression expression);
		IElement Resolve(IMemberInitializerExpression expression);
		IElement Resolve(IXmlNode expression);
		IElement Resolve(IXmlExpression expression);
	IElement Resolve(IXmlElementReferenceExpression expression);
	IElement Resolve(IXmlAttributeReferenceExpression expression);
		IElement Resolve(IGetXmlNamespaceOperator expression);
	IElement Resolve(IEmptyArgumentExpression expression);
	IElement Resolve(IMarkupExtensionExpression expression);
		IElement ResolveElementType(IElement element);
		IElement ResolveType(ITypeReferenceExpression typeRef);
		IElement FindTypeOrNamespace(IElement element);
		bool IsImplicitVariableReference(IElement reference);
		bool IsAttributeMemberAssignment(IExpression expression);
	IElement ResolveNullable(ITypeReferenceExpression typeRef);
		IElement ResolveExpression(IExpression expression);
	IElement ResolveExpression(IExpression expression, bool resolveMethodGroup);
		IElement ResolveMethodGroup(IExpression expression);
	ITypeElement ResolveCoreType(IElement start, string typeName);
	ITypeElement ResolveSystemObject(IElement start);
	ITypeElement ResolveSystemString(IElement start);
	ITypeElement ResolveSystemArray(IElement start);
	ITypeElement ResolveSystemValueType(IElement start);
	ITypeElement ResolveSystemEnumType(IElement start);
	ITypeElement ResolveSystemDelegateType(IElement start);
	ITypeElement ResolveSystemMultiCastDelegateType(IElement start);
	ITypeElement ResolveSystemNullable(IElement start);
	ITypeElement ResolveSystemGenericIEnumerable(IElement start);
	IElement GetDeclaration(IElement element);
	IResolvingCache GetResolvingCache();
		void SetActiveProject(IProjectElement project);
	ISourceTreeResolverOptions Options { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		IElementCollection FindAllElementDeclarations(IElement element);
  }
}
