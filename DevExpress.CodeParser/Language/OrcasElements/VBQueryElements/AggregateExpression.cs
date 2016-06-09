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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class AggregateExpression: FromExpression, IIntoContainingElement
	{
		ExpressionCollection _QueryOperators = null;
		LanguageElementCollection _IntoElements = null;
		public AggregateExpression()
		{
		}
	public void AddIntoElements(IEnumerable<LanguageElement> elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddIntoElement(element);
	}
		public void AddIntoElement(LanguageElement element)
		{
			if (element == null)
				return;
			if (_IntoElements == null)
				_IntoElements = new LanguageElementCollection();
			_IntoElements.Add(element);
			AddDetailNode(element);
		}
	public void AddQueryOperators(IEnumerable<Expression> queryOperators)
	{
	  if (queryOperators == null)
		return;
	  foreach (Expression queryOperator in queryOperators)
		AddQueryOperator(queryOperator);
	}
		public void AddQueryOperator(Expression exp)
		{
			if (exp == null || !(exp is QueryExpressionBase))
				return;
			if (_QueryOperators == null)
				_QueryOperators = new ExpressionCollection();
			_QueryOperators.Add(exp);
	  AddDetailNode(exp);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AggregateExpression lClone = new AggregateExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AggregateExpression))
				return;
			AggregateExpression lSource = (AggregateExpression)source;
			if (lSource._QueryOperators != null)
			{
				_QueryOperators = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _QueryOperators, lSource._QueryOperators);
				if (_QueryOperators.Count == 0 && lSource._QueryOperators.Count > 0)
					_QueryOperators = lSource._QueryOperators.DeepClone(options) as ExpressionCollection;
			}
	  if (lSource._IntoElements != null)
	  {
		_IntoElements = new LanguageElementCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _IntoElements, lSource._IntoElements);
		if (_IntoElements.Count == 0 && lSource._IntoElements.Count > 0)
		  _IntoElements = lSource._IntoElements.DeepClone(options) as LanguageElementCollection;
	  }
		}
		#endregion
		public override string ToString()
		{
			return "AggregateExpression";
		}
	public LanguageElementCollection IntoElements
	{
	  get
	  {
		return _IntoElements;
	  }
	}
		public ExpressionCollection QueryOperators
		{
			get
			{
				return _QueryOperators;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AggregateExpression;
			}
		}
	}
}
