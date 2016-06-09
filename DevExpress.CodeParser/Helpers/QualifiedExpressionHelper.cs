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
  public class QualifiedExpressionHelper
  {
	static bool IsSimpleReference(IElement element)
	{
	  if (element.ElementType == LanguageElementType.ElementReferenceExpression ||
		element.ElementType == LanguageElementType.QualifiedNestedReference)
		return true;
	  if (element.ElementType == LanguageElementType.TypeReferenceExpression ||
		element.ElementType == LanguageElementType.QualifiedTypeReferenceExpression ||
		element.ElementType == LanguageElementType.QualifiedNestedTypeReference)
	  {
		ITypeReferenceExpression lTypeRef = element as ITypeReferenceExpression;
		return lTypeRef != null && !(lTypeRef.IsArrayType || lTypeRef.IsPointerType || lTypeRef.IsGeneric);
	  }
	  return false;
	}
	#region GetLevel
	public static int GetLevel(IHasQualifier element)
	{
	  if (element == null)
		return 1;
	  if (element.Qualifier == null)
		return 1;
	  return element.Qualifier.Level + 1;
	}
	#endregion
	#region GetByLevel(LanguageElement element, int level)
	public static LanguageElement GetByLevel(LanguageElement element, int level)
	{
	  if (element == null || level < 0)
		return null;
	  if (!(element is IHasQualifier))
	  {
		if (level == 1)
		  return element;
		return null;
	  }
	  IHasQualifier lElement = element as IHasQualifier;
	  int lLevel = GetLevel(lElement);
	  LanguageElement lResult = null;
	  if (level > lLevel || level < 1)
		return null;
	  if (lLevel == level)
		lResult = element;
	  else
		lResult = GetByLevel(lElement.Qualifier, level);
	  return lResult;
	}
	#endregion
	#region GetStartElement
	public static LanguageElement GetStartElement(LanguageElement element)
	{
	  return GetByLevel(element, 1);
	}
	#endregion
	#region GetEndElement
	public static LanguageElement GetEndElement(LanguageElement element)
	{
	  return GetEndElement(element, false );
	}
	#endregion
	#region GetEndElement
	private static LanguageElement GetEndElement(LanguageElement element, bool expressionOnly)
	{
	  if (element == null && element is IHasQualifier)
		return null;
	  LanguageElement lStart = element;
	  LanguageElement lParent = null;
	  bool lDone = false;
	  while (!lDone && lStart != null)
	  {
		if (lStart.Parent is IHasQualifier)
		{
		  lParent = lStart.Parent;
		  if (expressionOnly && !(lParent is Expression))
			return lStart;
		  if (((IHasQualifier)lParent).Qualifier == lStart)
		  {
			lStart = lParent;
			continue;
		  }
		}
		lDone = true;
	  }
	  return lStart;
	}
	#endregion
	#region GetEndExpression
	public static LanguageElement GetEndExpression(LanguageElement element)
	{
	  return GetEndElement(element, true );
	}
	#endregion
	public static bool HasCleanReferences(IElement element)
	{
	  if (element == null || !(element is IWithSource))
		return false;
	  IWithSource lHasSource = (IWithSource)element;
	  if (lHasSource.Source == null)
		return IsSimpleReference(element);
	  return IsSimpleReference(element) && HasCleanReferences(lHasSource.Source);
	}
  }
  [Obsolete("Use QualifiedExpressionHelper instead.")]
  public class QuailifiedExpressionHelper : QualifiedExpressionHelper
  {
  }  
}
