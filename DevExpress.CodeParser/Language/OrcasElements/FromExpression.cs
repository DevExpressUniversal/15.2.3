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
	#region FromExpression
	public class FromExpression: QueryExpressionBase, IFromExpression
	{
		ExpressionCollection _InExpressions;
		public  FromExpression()
		{			
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is FromExpression))
				return;
			FromExpression lSource = source as FromExpression;
			if (lSource._InExpressions != null)
			{
				_InExpressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _InExpressions, lSource._InExpressions);
				if (_InExpressions.Count == 0 && lSource._InExpressions.Count > 0)
					_InExpressions = lSource._InExpressions.DeepClone(options) as ExpressionCollection;
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_InExpressions != null && _InExpressions.Contains(oldElement as Expression))
				_InExpressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			FromExpression lClone = new FromExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
	public void AddInExpressions(IEnumerable<Expression> inExpressions)
	{
	  if (inExpressions == null)
		return;
	  foreach (Expression inExpression in inExpressions)
		AddInExpression(inExpression as InExpression);
	}
	public void AddInExpressions(IEnumerable<InExpression> inExpressions)
	{
	  if (inExpressions == null)
		return;
	  foreach (InExpression inExpression in inExpressions)
		AddInExpression(inExpression);
	}
		#region AddInExpression
		public void AddInExpression(InExpression exp)
		{
			if (exp == null)
				return;
			InExpressions.Add(exp);
			AddDetailNode(exp);
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "FromExpression";
		}
		#endregion
		public ExpressionCollection InExpressions
		{
			get
			{
				if (_InExpressions == null)
					_InExpressions = new ExpressionCollection();
				return _InExpressions;
			}			
		}		
		#region ElementType
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.FromExpression;
			}
		}
		#endregion
		IExpressionCollection IFromExpression.InExpressions
		{
			get
			{
				return InExpressions;
			}
		}
	}
	#endregion	
}
