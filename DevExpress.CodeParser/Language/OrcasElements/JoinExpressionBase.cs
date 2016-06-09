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
	public abstract class JoinExpressionBase: QueryExpressionBase
	{
		InExpression _InExpression;
		ExpressionCollection _EqualsExpressions = null;
		ExpressionCollection _JoinExpressions = null;		
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is JoinExpressionBase))
				return;
			JoinExpressionBase lSource = (JoinExpressionBase)source;
			if (lSource._EqualsExpressions != null)
			{
				_EqualsExpressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _EqualsExpressions, lSource._EqualsExpressions);
				if (_EqualsExpressions.Count == 0 && lSource._EqualsExpressions.Count > 0)
					_EqualsExpressions = lSource._EqualsExpressions.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._JoinExpressions != null)
			{
				_JoinExpressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _JoinExpressions, lSource._JoinExpressions);
				if (_JoinExpressions.Count == 0 && lSource._JoinExpressions.Count > 0)
					_EqualsExpressions = lSource._JoinExpressions.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._InExpression != null)
			{
				_InExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._InExpression) as InExpression;
				if (_InExpression == null)
					_InExpression = lSource._InExpression.Clone(options) as InExpression;
			}
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_EqualsExpressions != null && _EqualsExpressions.Contains(oldElement as Expression))
				_EqualsExpressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				if (_JoinExpressions != null && _JoinExpressions.Contains(oldElement as Expression))
					_JoinExpressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				if (_InExpression != null && _InExpression == oldElement)
					_InExpression = newElement as InExpression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region SetInExpression
		public void SetInExpression(InExpression expression)
		{
			if (expression == null)
				return;
			ReplaceDetailNode(_InExpression, expression);
			_InExpression = expression;
		}
		#endregion
	public void AddEqualsExpressions(IEnumerable<Expression> equalsExpressions)
	{
	  if (equalsExpressions == null)
		return;
	  foreach (EqualsExpression equalsExpression in equalsExpressions)
		AddEqualsExpression(equalsExpression);
	}
		#region AddEqualsExpression
		public void AddEqualsExpression(EqualsExpression exp)
		{
			if (exp == null)
				return;
			EqualsExpressions.Add(exp);
			AddDetailNode(exp);
		}
		#endregion
	public void AddJoinExpressions(IEnumerable<Expression> joinExpressions)
	{
	  if (joinExpressions == null)
		return;
	  foreach (JoinExpressionBase joinExpression in joinExpressions)
		AddJoinExpression(joinExpression);
	}
	#region AddJoinExpression
		public void AddJoinExpression(JoinExpressionBase exp)
		{
			if (exp == null)
				return;
			JoinExpressions.Add(exp);
			AddDetailNode(exp);
		}
		#endregion
		#region EqualsExpressions
		public ExpressionCollection EqualsExpressions
		{
			get
			{
				if (_EqualsExpressions == null)
					_EqualsExpressions = new ExpressionCollection();
				return _EqualsExpressions;
			}
		}
		#endregion
		#region JoinExpressions
		public ExpressionCollection JoinExpressions
		{
			get
			{
				if (_JoinExpressions == null)
					_JoinExpressions = new ExpressionCollection();
				return _JoinExpressions;
			}
		}
		#endregion
		#region InExpression
		public InExpression InExpression
		{
			get
			{
				return _InExpression;
			}
		}
		#endregion
	}
}
