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
	public class VariableFilter : ElementFilterBase
	{
		public VariableFilter()	{}
		public override bool Apply(IElement element)
		{
			if (element == null)
				return false;
			return element.ElementType == LanguageElementType.Variable ||
				element.ElementType == LanguageElementType.InitializedVariable ||
				element is IBaseVariable;								
		}
		public override bool SkipChildren(IElement element)
		{
			return false;
		}
		public override IElementCollection Apply(IElementCollection elements)
		{
			return ElementCollector.CollectElements(elements, this);
		}
	}
	public class LocalNameFilter : IElementFilter
	{
		SourceRange _Range;
		public LocalNameFilter(SourceRange range)
		{
			_Range = range;
		}
		public bool Apply(IElement element)
		{
			if (element == null)
				return false;
			if (!_Range.Contains(element.FirstRange))
				return false;
			if (element is BaseVariable)
				return true;
			if (element.ElementType != LanguageElementType.ElementReferenceExpression &&
		element.ElementType != LanguageElementType.MethodReferenceExpression &&
		element.ElementType != LanguageElementType.TypedElementReferenceExpression)
				return false;
			IReferenceExpression lReference = (IReferenceExpression)element;
			return lReference.Source == null;
		}
		public bool SkipChildren(IElement element)
		{
			return false;
		}
	}
}
