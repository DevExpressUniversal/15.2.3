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
	public class UnaryIncrement : UnaryOperatorExpression, IUnaryIncrementExpression
	{
		const int INT_PreMaintainanceComplexity = 4;
		const int INT_PostMaintainanceComplexity = 3;
		public UnaryIncrement()
		{
		}
		public UnaryIncrement(Token token, Expression expression, bool isPostIncrement)
			: base(token, expression, isPostIncrement)
		{
		}
		public UnaryIncrement(Expression expression, bool isPostIncrement)
			: base(expression, isPostIncrement)
		{
		}
		public override int GetImageIndex()
		{
			return ImageIndex.UnaryIncrement;
		}
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is UnaryIncrement)
			{
				UnaryIncrement lOperation = expression as UnaryIncrement;
				if (Expression == null)
					return false;
				return Expression.IsIdenticalTo(lOperation.Expression) && 
					IsPostOperator == lOperation.IsPostOperator;
			}
			return false;
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			UnaryIncrement lClone = new UnaryIncrement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return IsPostOperator ? INT_PostMaintainanceComplexity : INT_PreMaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.UnaryIncrement;
			}
		}
		public override bool CanBeStatement
		{
			get
			{
				return true;
			}
		}
	}
}
