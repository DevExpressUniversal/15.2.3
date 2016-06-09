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
	#region WhereExpression
	public class WhereExpression : QueryExpressionBase, IWhereExpression
	{
		Expression _WhereClause;
	public WhereExpression()
	{
	}
	public WhereExpression(Expression whereClause)
	{
	  SetWhereClause(whereClause);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is WhereExpression))
				return;
			WhereExpression lSource = (WhereExpression)source;
			if (lSource._WhereClause != null)
			{
				_WhereClause = ParserUtils.GetCloneFromNodes(this, lSource, lSource._WhereClause) as Expression;
				if (_WhereClause == null)
					_WhereClause = lSource._WhereClause.Clone(options) as Expression;
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_WhereClause == oldElement)
				_WhereClause = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public void SetWhereClause(Expression whereClause)
		{
			if (whereClause == null)
				return;
			ReplaceDetailNode(_WhereClause, whereClause);
			_WhereClause = whereClause;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			WhereExpression lClone = new WhereExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public Expression WhereClause
		{
			get
			{
				return _WhereClause;
			}
		}
		public override string ToString()
		{
			return "WhereExpression";
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.WhereExpression;
			}
		}
		IExpression IWhereExpression.WhereClause
		{
			get
			{
				return WhereClause;
			}
		}
	}
	#endregion
}
