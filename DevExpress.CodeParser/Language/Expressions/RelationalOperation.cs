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
	public enum RelationalOperator : byte
	{
		None,
		Equality,
		Inequality,
		LessThan,
		GreaterThan,
		LessOrEqual,
		GreaterOrEqual,
		Like,
		StrictEquality,
		StrictInequality
	}
	public class RelationalOperation : BinaryOperatorExpression, IRelationalOperationExpression
	{
		const int INT_MaintainanceComplexity = 5;
		protected RelationalOperation()
		{
		}
		#region RelationalOperation
		public RelationalOperation(Expression left, Token token, Expression right)
			: base (left, token, right)
		{
			SetRelationalOperator(RelationalOperator.None);
		}
		#endregion
		#region RelationalOperation(Expression left, Token token, Expression right, RelationalOperator relationalOperator, SourceRange range)
		public RelationalOperation(Expression left, Token token, Expression right, RelationalOperator relationalOperator, SourceRange range)
			: base (left, token, right)
		{
			SetRange(range);
			SetRelationalOperator(relationalOperator);
		}
		#endregion
		#region RelationalOperation
		public RelationalOperation(Expression left, RelationalOperator op, Expression right)
		{
			SetLeftSide(left);
			SetRelationalOperator(op);
			SetRightSide(right);
		}
		#endregion
		bool CheckSymetric(RelationalOperation op)
		{
			return LeftSide.IsIdenticalTo(op.LeftSide) && 
				RightSide.IsIdenticalTo(op.RightSide);
		}
		bool CheckAsymetric(RelationalOperation op)
		{
			return LeftSide.IsIdenticalTo(op.RightSide) && 
				RightSide.IsIdenticalTo(op.LeftSide);
		}
		bool CheckEquality(RelationalOperation op)
		{
			return op.RelationalOperator == RelationalOperator.Equality &&
				 (CheckSymetric(op) || CheckAsymetric(op));
		}
		bool CheckInequality(RelationalOperation op)
		{
			return op.RelationalOperator == RelationalOperator.Inequality &&
				(CheckSymetric(op) || CheckAsymetric(op));
		}
		bool CheckLessThan(RelationalOperation op)
		{
			return (op.RelationalOperator == RelationalOperator.LessThan && CheckSymetric(op)) ||
				(op.RelationalOperator == RelationalOperator.GreaterThan && CheckAsymetric(op));
		}
		bool CheckGreaterThan(RelationalOperation op)
		{
			return (op.RelationalOperator == RelationalOperator.GreaterThan && CheckSymetric(op)) ||
				(op.RelationalOperator == RelationalOperator.LessThan && CheckAsymetric(op));
		}
		bool CheckLessOrEqual(RelationalOperation op)
		{
			return (op.RelationalOperator == RelationalOperator.LessOrEqual && CheckSymetric(op)) ||
				(op.RelationalOperator == RelationalOperator.GreaterOrEqual && CheckAsymetric(op));
		}
		bool CheckGreaterOrEqual(RelationalOperation op)
		{
			return (op.RelationalOperator == RelationalOperator.GreaterOrEqual && CheckSymetric(op)) ||
				(op.RelationalOperator == RelationalOperator.LessOrEqual && CheckAsymetric(op));
		}
		protected virtual void SetRelationalOperator(RelationalOperator op)
		{
			BinaryOperator = GetBinaryOperatorType(op);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.RelationalOperation;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is RelationalOperation)
			{
				RelationalOperation lOperation = expression as RelationalOperation;
				if (LeftSide == null)
					return false;
				if (RightSide == null)
					return false;
				switch (RelationalOperator)
				{
					case RelationalOperator.Equality:
						return CheckEquality(lOperation);
					case RelationalOperator.Inequality:
						return CheckInequality(lOperation);
					case RelationalOperator.LessThan:
						return CheckLessThan(lOperation);
					case RelationalOperator.GreaterThan:
						return CheckGreaterThan(lOperation);
					case RelationalOperator.LessOrEqual:
						return CheckLessOrEqual(lOperation);
					case RelationalOperator.GreaterOrEqual:
						return CheckGreaterOrEqual(lOperation);
				}
			}
			return false;
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		public static RelationalOperator InvertRelationalOperator(RelationalOperator op)
		{
			switch (op)
			{
				case RelationalOperator.Inequality:
					return RelationalOperator.Equality;
				case RelationalOperator.Equality:
					return RelationalOperator.Inequality;
				case RelationalOperator.GreaterOrEqual:
					return RelationalOperator.LessThan;
				case RelationalOperator.GreaterThan:
					return RelationalOperator.LessOrEqual;
				case RelationalOperator.LessOrEqual:
					return RelationalOperator.GreaterThan;
				case RelationalOperator.LessThan:
					return RelationalOperator.GreaterOrEqual;
				case RelationalOperator.StrictEquality:
					return RelationalOperator.StrictInequality;
				case RelationalOperator.StrictInequality:
					return RelationalOperator.StrictEquality;
				default:
					return RelationalOperator.None;
			}
		}
		public static BinaryOperatorType GetBinaryOperatorType(RelationalOperator op)
		{
			switch (op)
			{
				case RelationalOperator.Equality:
					return BinaryOperatorType.ValueEquality;
				case RelationalOperator.Inequality:
					return BinaryOperatorType.IdentityInequality;
				case RelationalOperator.LessThan:
					return BinaryOperatorType.LessThan;
				case RelationalOperator.LessOrEqual:
					return BinaryOperatorType.LessThanOrEqual;
				case RelationalOperator.GreaterThan:
					return BinaryOperatorType.GreaterThan;
				case RelationalOperator.GreaterOrEqual:
					return BinaryOperatorType.GreaterThanOrEqual;
				case RelationalOperator.Like:
					return BinaryOperatorType.Like;
				case RelationalOperator.StrictEquality:
					return BinaryOperatorType.StrictEquality;
				case RelationalOperator.StrictInequality:
					return BinaryOperatorType.StrictInequality;
			}
			return BinaryOperatorType.None;
		}
		public static RelationalOperator GetRelationalOperatorType(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.ValueEquality:
					return RelationalOperator.Equality;
				case BinaryOperatorType.IdentityInequality:
					return RelationalOperator.Inequality;
				case BinaryOperatorType.LessThan:
					return RelationalOperator.LessThan;
				case BinaryOperatorType.LessThanOrEqual:
					return RelationalOperator.LessOrEqual;
				case BinaryOperatorType.GreaterThan:
					return RelationalOperator.GreaterThan;
				case BinaryOperatorType.GreaterThanOrEqual:
					return RelationalOperator.GreaterOrEqual;
				case BinaryOperatorType.Like:
					return RelationalOperator.Like;
				case BinaryOperatorType.StrictEquality:
					return RelationalOperator.StrictEquality;
				case BinaryOperatorType.StrictInequality:
					return RelationalOperator.StrictInequality;
			}
			return RelationalOperator.None;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			RelationalOperation lClone = new RelationalOperation();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				if (RelationalOperator == RelationalOperator.Inequality ||
					RelationalOperator == RelationalOperator.StrictInequality)
					return INT_MaintainanceComplexity + 1;
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.RelationalOperation;
			}
		}
		#region RelationalOperator
		public RelationalOperator RelationalOperator
		{
			get
			{
				return GetRelationalOperatorType(BinaryOperator);
			}
			set
			{
				SetRelationalOperator(value);
			}
		}
		#endregion
		#region IRelationalOperationExpression Members
		RelationalOperator IRelationalOperationExpression.RelationalOperator
		{
			get
			{
				return RelationalOperator;
			}
		}
		#endregion
	}
}
