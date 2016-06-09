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
	public enum InExpressionType
	{
		InExpression,
		LetExpression
	}	
	#region InExpression
	public class InExpression: QueryExpressionBase, IInExpression
	{
		QueryIdent _QueryIdent;
		Expression _Expression;
		InExpressionType _InExpressionType = InExpressionType.InExpression;
		public InExpression()
		{
		}
		public InExpression(QueryIdent queryIdent, Expression exp)
		{
			_QueryIdent = queryIdent;
			_Expression = exp;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is InExpression))
				return;
			InExpression lSource = source as InExpression;
			if (lSource._Expression != null)
			{
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
			if (lSource._QueryIdent != null)
			{
				_QueryIdent = ParserUtils.GetCloneFromNodes(this, lSource, lSource._QueryIdent) as QueryIdent;
				if (_QueryIdent == null)
					_QueryIdent = lSource._QueryIdent.Clone(options) as QueryIdent;
			}
			_InExpressionType = lSource.InExpressionType;
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_QueryIdent == oldElement)
				_QueryIdent = (QueryIdent)newElement;
			else if (_Expression == oldElement)
				_Expression = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return null;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			InExpression lClone = new InExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public QueryIdent QueryIdent
		{
			get
			{
				return _QueryIdent;
			}
			set
			{
				if (value == null)
					return;
				ReplaceDetailNode(_QueryIdent, value);
				_QueryIdent = value;
			}
		}
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				if (value == null)
					return;
				ReplaceDetailNode(_Expression, value);
				_Expression = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.InExpression;
			}
		}
		public override string ToString()
		{
			if (_InExpressionType == InExpressionType.LetExpression)
				return "LetExpression";
			return "InExpression";
		}
		public InExpressionType InExpressionType
		{
			get
			{
				return _InExpressionType;
			}
			set
			{
				_InExpressionType = value;
			}
		}
		IExpression IInExpression.Expression
		{
			get
			{
				return _Expression;
			}
		}
		IQueryIdent IInExpression.QueryIdent
		{
			get
			{
				return _QueryIdent;
			}
		}
	}
	#endregion
}
