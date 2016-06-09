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
	public interface IExpressionInverter
	{
		#region Invert
		Expression Invert(Expression expression);
		#endregion
	}
	public abstract class ExpressionInverterBase : IExpressionInverter
	{
		protected virtual Expression InvertIsExpression(Is expression)
		{
			return DefaultInvert(expression);
		}
		protected virtual Expression InvertIsNotExpression(IsNot expression)
		{
			return DefaultInvert(expression);
		}
		protected virtual Expression InvertRelationalOperation(RelationalOperation expression)
		{
			RelationalOperator lOperator = RelationalOperation.InvertRelationalOperator(expression.RelationalOperator);
			if (lOperator == RelationalOperator.None)
				return new LogicalInversion(expression);
			return new RelationalOperation(expression.LeftSide, lOperator, expression.RightSide);
		}		
		protected virtual Expression InvertExpression(Expression expression)
		{
			if (expression is Is)
				return InvertIsExpression((Is)expression);
			if (expression is IsNot)
				return InvertIsNotExpression((IsNot)expression);
			if (expression is RelationalOperation)
				return InvertRelationalOperation((RelationalOperation)expression);
			return DefaultInvert(expression);
		}
		#region Invert
		public virtual Expression Invert(Expression expression)
		{
			if (expression == null)
				return expression;
			return InvertExpression(expression);
		}
		#endregion
		public static Expression DefaultInvert(Expression expression)
		{
			Expression lExpression = expression;
			if (lExpression is LogicalInversion)
			{
				Expression lUnaryExpression = ((LogicalInversion)lExpression).Expression;
				if (lUnaryExpression is ParenthesizedExpression && 
					!lUnaryExpression.NeedsInvertParens)
					lUnaryExpression = ((ParenthesizedExpression)lUnaryExpression).Expression;
				return lUnaryExpression;
			}
			if (lExpression.NeedsInvertParens)
				lExpression = new ParenthesizedExpression(lExpression);
			return new LogicalInversion(lExpression);
		}
	}
}
