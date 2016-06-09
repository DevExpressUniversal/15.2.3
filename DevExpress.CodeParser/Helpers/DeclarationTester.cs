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
	public class DeclarationTester
	{
		private DeclarationTester(){}
		static bool IsMethodReference(IElement element)
		{
			return
				element.ElementType == LanguageElementType.MethodReferenceExpression ||
				element.ElementType == LanguageElementType.PointerMethodReference ||
				element.ElementType == LanguageElementType.QualifiedMethodReference;
		}
		static bool IsElementReference(ILocalDeclaration declaration, IElement element)
		{
			if (!IsElementReference(element))
				return false;
			if (declaration.IsImplicit && declaration.IsReturnedValue)
			{
				IExpression expression = (IExpression)element;
				if (expression.IsStatement || IsMethodReference(element))
					return false;
			}
			return true;
		}
		static bool IsSizeOfExpresion(IElement element)
		{
			return element != null && element.Parent != null && element.Parent.ElementType == LanguageElementType.SizeOfExpression;
		}
		static bool IsHtmlReference(IElement element)
		{
			return element != null && element is HtmlAttribute;
		}
		static bool IsKeywordReference(IElement declaration, IElement element)
		{
			if (element == null)
				return false;
	  if (declaration != null && String.Compare(declaration.Name, element.Name) == 0)
		return false;
			IReferenceExpression referenceExpr = element as IReferenceExpression;
			if (referenceExpr == null)
				return false;
			if (referenceExpr.Source != null)
				return false;
			return StructuralParserServicesHolder.IsKeyword(referenceExpr.Name) && StructuralParserServicesHolder.IsKeywordElement(referenceExpr);
		}
	public static bool IsSuitableForLocalReference(ILocalDeclaration declaration, IElement element)
		{
			if (element == null || !IsElementReference(declaration, element))
				return false;
			if (IsKeywordReference(declaration, element))
				return false;
	  if(IsArgumentNameSpecifier(element))
		return false;
			IWithSource withSource = element as IWithSource;
			if (withSource == null)
				return false;
	  if (withSource.Source != null)
	  {
		IReferenceExpression reference = withSource as IReferenceExpression;
		if (reference != null && reference.IsMulOperator)
		  return true;
		if (declaration is IQueryIdent)
		  return true;
		return false;
	  }
			return true;
		}
	private static bool IsArgumentNameSpecifier(IElement element)
	{
	  if (element == null)
		return false;
	  IAttributeVariableInitializer parent = element.Parent as IAttributeVariableInitializer;
	  if (parent == null)
		return false;
	  IExpression leftSide = parent.LeftSide;
	  return leftSide != null && leftSide.Equals(element);
	}
		public static bool IsSuitableForFieldReference(IElement element)
		{
			if (element == null)
				return false;
			if (!IsElementReference(element))
				return false;
			if (IsKeywordReference(null, element))
				return false;
			return true;
		}
		public static bool IsSuitableForTypeDefReference(IElement element)
		{
			if (element == null)
				return false;
			return IsTypeReference(element) || IsElementReference(element);
		}
		public static bool IsElementReference(IElement element)
		{
			if (element == null)
				return false;
			LanguageElementType elementType = element.ElementType;
			return 
				elementType == LanguageElementType.ElementReferenceExpression ||
				elementType == LanguageElementType.PointerElementReference ||
				elementType == LanguageElementType.QualifiedNestedReference ||
				elementType == LanguageElementType.CppQualifiedElementReference ||
				elementType == LanguageElementType.MemberInitializerExpression ||
				elementType == LanguageElementType.TypedElementReferenceExpression ||
				IsMethodReference(element) ||
				IsSizeOfExpresion(element);
		}
		public static bool IsTypeReference(IElement element)
		{
			if (element == null)
				return false;
			LanguageElementType elementType = element.ElementType;
			return 
				elementType == LanguageElementType.TypeReferenceExpression ||
				elementType == LanguageElementType.ElementReferenceExpression ||
				elementType == LanguageElementType.PointerElementReference ||
				elementType == LanguageElementType.QualifiedNestedReference ||
				elementType == LanguageElementType.CppQualifiedElementReference ||
				elementType == LanguageElementType.QualifiedTypeReferenceExpression ||
				elementType == LanguageElementType.QualifiedNestedTypeReference ||
				elementType == LanguageElementType.Attribute ||
				elementType == LanguageElementType.TypedElementReferenceExpression;
		}
		public static bool IsReference(IElement element)
		{
			if (element == null)
				return false;
			return IsTypeReference(element) ||
				IsElementReference(element) ||
				IsHtmlReference(element);				
		}
		public static bool IsConstructor(IElement element)
		{
			if (element == null)
				return false;
			if (!(element is IMethodElement))
				return false;
			IMethodElement lMethod = (IMethodElement)element;
			return lMethod.IsConstructor;
		}
		public static bool IsDestructor(IElement element)
		{
			if (element == null)
				return false;
			if (!(element is IMethodElement))
				return false;
			IMethodElement lMethod = (IMethodElement)element;
			return lMethod.IsDestructor;
		}
		public static bool DeclarationIsValid(IElement element)
		{			
			return element != null && (
				element.ElementType == LanguageElementType.Variable ||
				element.ElementType == LanguageElementType.Volatile ||
				element.ElementType == LanguageElementType.InitializedVariable ||
				element.ElementType == LanguageElementType.InitializedVolatile ||
		element.ElementType == LanguageElementType.ImplicitVariable ||
				element.ElementType == LanguageElementType.LambdaImplicitlyTypedParam ||
		element.ElementType == LanguageElementType.QueryIdent ||
				element.ElementType == LanguageElementType.Const ||
				element.ElementType == LanguageElementType.ConstVolatile ||
				element.ElementType == LanguageElementType.Parameter ||
				element.ElementType == LanguageElementType.Method ||
		element.ElementType == LanguageElementType.MethodPrototype ||
				element.ElementType == LanguageElementType.Property ||				
				element.ElementType == LanguageElementType.Event ||
				element.ElementType == LanguageElementType.Class ||
				element.ElementType == LanguageElementType.ManagedClass ||
				element.ElementType == LanguageElementType.ManagedStruct ||
				element.ElementType == LanguageElementType.ValueClass ||
				element.ElementType == LanguageElementType.ValueStruct ||
				element.ElementType == LanguageElementType.Union ||				
				element.ElementType == LanguageElementType.Struct ||
				element.ElementType == LanguageElementType.Interface ||
				element.ElementType == LanguageElementType.InterfaceClass ||
				element.ElementType == LanguageElementType.InterfaceStruct ||
				element.ElementType == LanguageElementType.Module ||
				element.ElementType == LanguageElementType.Enum ||
				element.ElementType == LanguageElementType.EnumElement ||
				element.ElementType == LanguageElementType.Delegate ||
				element.ElementType == LanguageElementType.TypeParameter ||
		element.ElementType == LanguageElementType.TypenameTypeParameter ||
				element.ElementType == LanguageElementType.Namespace ||
		element.ElementType == LanguageElementType.BitFieldConst ||
		element.ElementType == LanguageElementType.BitFieldVariable ||
		element.ElementType == LanguageElementType.BitFieldVolatile 
		);
		}
	}
}
