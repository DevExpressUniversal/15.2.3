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
	public class UnaryDecrement : UnaryOperatorExpression, IUnaryDecrementExpression
	{
		const int INT_PreMaintainanceComplexity = 4;
		const int INT_PostMaintainanceComplexity = 3;		
		#region UnaryDecrement()
		public UnaryDecrement()			
		{
		}
		#endregion
		#region UnaryDecrement(Token token, Expression expression, bool isPostDecrement)
		public UnaryDecrement(Token token, Expression expression, bool isPostDecrement)
			: base(token, expression, isPostDecrement)
		{
		}
		#endregion
		#region UnaryDecrement(Expression expression, bool isPostDecrement)
	public UnaryDecrement(Expression expression, bool isPostDecrement)
	  : base(expression, isPostDecrement)
	{
	}
		#endregion
		#region CanBeStatement
		public override bool CanBeStatement
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.UnaryDecrement;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is UnaryDecrement)
			{
				UnaryDecrement lOperation = expression as UnaryDecrement;
				if (Expression == null)
					return false;
				return Expression.IsIdenticalTo(lOperation.Expression) && 
					IsPostOperator == lOperation.IsPostOperator;
			}
			return false;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			UnaryDecrement lClone = new UnaryDecrement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
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
				return LanguageElementType.UnaryDecrement;
			}
		}
	}
}
