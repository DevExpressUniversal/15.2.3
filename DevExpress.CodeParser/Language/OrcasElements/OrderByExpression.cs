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
using System.Text;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region OrderByExpression
	public class OrderByExpression : QueryExpressionBase, IOrderByExpression
	{
		ExpressionCollection _Orderings;
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is OrderByExpression))
				return;
			OrderByExpression lSource = (OrderByExpression)source;
			if (lSource._Orderings != null)
			{
				_Orderings = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Orderings, lSource._Orderings);
				if (_Orderings.Count == 0 && lSource._Orderings.Count > 0)
					_Orderings = lSource._Orderings.DeepClone(options) as ExpressionCollection;
			}			
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Orderings != null && _Orderings.Contains(oldElement))
				_Orderings.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
	public void AddOrderings(IEnumerable<IElement> orderings)
	{
	  if (orderings == null)
		return;
	  foreach (OrderingExpression ordering in orderings)
		AddOrdering(ordering);
	}
		public void AddOrdering(OrderingExpression ordering)
		{
			if (ordering == null)
				return;
			Orderings.Add(ordering);
			AddDetailNode(ordering);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			OrderByExpression lClone = new OrderByExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public ExpressionCollection Orderings
		{
			get
			{
				if (_Orderings == null)
					_Orderings = new ExpressionCollection();
				return _Orderings;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.OrderByExpression;
			}
		}
		#region ToString
		public override string ToString()
		{						
			return "OrderByExpression";
		}
		#endregion
		IExpressionCollection IOrderByExpression.Orderings
		{
			get
			{
				return Orderings;
			}
		}
	}
	#endregion
}
