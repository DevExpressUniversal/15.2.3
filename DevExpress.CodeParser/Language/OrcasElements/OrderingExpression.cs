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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum OrderingType 
	{
		Default,
		Ascending,
		Descending
	}
	#region OrderingExpression
	public class OrderingExpression : QueryExpressionBase, IOrderingExpression
	{
		OrderingType _Order = OrderingType.Default;
		Expression _Ordering;
	public OrderingExpression()
	{
	}
	public OrderingExpression(OrderingType order, Expression ordering)
	{
	  _Order = order;
	  SetOrdering(ordering);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is OrderingExpression))
				return;
			OrderingExpression lSource = (OrderingExpression)source;
			if (lSource._Ordering != null)
			{
				_Ordering = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Ordering) as Expression;
				if (_Ordering == null)
					_Ordering = lSource._Ordering.Clone(options) as Expression;
			}
			_Order = lSource.Order;			
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Ordering == oldElement)
				_Ordering = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public void SetOrdering(Expression ordering)
		{
			if (ordering == null)
				return;
			ReplaceDetailNode(_Ordering, ordering);
			_Ordering = ordering;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			OrderingExpression lClone = new OrderingExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public Expression Ordering
		{
			get
			{
				return _Ordering;
			}
		}
		public OrderingType Order
		{
			get
			{
				return _Order;
			}
			set
			{
				_Order = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.OrderingExpression;
			}
		}
		public override string ToString()
		{
			if (_Ordering != null)
				return _Ordering.ToString();
			return String.Empty;
		}
		IExpression IOrderingExpression.Ordering
		{
			get
			{
				return Ordering;
			}
		}
	}
	#endregion
}
