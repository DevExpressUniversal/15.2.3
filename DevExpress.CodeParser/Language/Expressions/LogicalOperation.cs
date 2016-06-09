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
	public enum LogicalOperator : byte
	{
		None,
		And,							
		Or,								
		ShortCircuitAnd,	
		ShortCircuitOr,		
		ExclusiveOr,			
	}
	public class LogicalOperation : BinaryOperatorExpression, ILogicalOperationExpression
	{
		const int INT_MaintainanceComplexity = 5;
		#region LogicalOperation()
		protected LogicalOperation()			
		{
		}
		#endregion
		#region LogicalOperation(Expression left, Token token, Expression right)
		public LogicalOperation(Expression left, Token token, Expression right)
			: base (left, token, right)
		{
		}
		#endregion
		#region LogicalOperation(Expression left, Token token, Expression right, LogicalOperator op, SourceRange range)
		public LogicalOperation(Expression left, Token token, Expression right, LogicalOperator op, SourceRange range)
			: this (left, token, right)
		{
			SetLogicalOperator(op);
			SetRange(range);
		}
		#endregion
		#region LogicalOperation(Expression left, LogicalOperator op, Expression right)
		public LogicalOperation(Expression left, LogicalOperator op, Expression right)
		{
			SetLeftSide(left);
			SetLogicalOperator(op);
			SetRightSide(right);
		}
		#endregion
		#region EvaluateExpression
		protected override object EvaluateExpression()
		{
			if (LeftSide == null)
				return null;
			if (RightSide == null)
				return null;
			bool lLeftValue = LeftSide.EvaluateAsBool();
			bool lRightValue = RightSide.EvaluateAsBool();
			switch (LogicalOperator)
			{
				case LogicalOperator.And:
					return lLeftValue & lRightValue;
				case LogicalOperator.Or:
					return lLeftValue | lRightValue;
				case LogicalOperator.ExclusiveOr:
					return lLeftValue ^ lRightValue;
				case LogicalOperator.ShortCircuitAnd:
					return lLeftValue && lRightValue;
				case LogicalOperator.ShortCircuitOr:
					return lLeftValue || lRightValue;
			}
			return null;
		}
		#endregion
		protected virtual void SetLogicalOperator(LogicalOperator op)
		{
			BinaryOperator = GetBinaryOperatorType(op);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.LogicalOperation;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is LogicalOperation)
			{
				LogicalOperation lOperation = expression as LogicalOperation;
				if (LeftSide == null)
					return false;
				if (RightSide == null)
					return false;
				return LeftSide.IsIdenticalTo(lOperation.LeftSide) && 
							 LogicalOperator == lOperation.LogicalOperator &&
							 RightSide.IsIdenticalTo(lOperation.RightSide);
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
		bool HasCondition(LanguageElement element)
		{
			if (element == null)
				return false;
			return element.ElementType == LanguageElementType.If ||
				element.ElementType == LanguageElementType.For ||
				element.ElementType == LanguageElementType.ElseIf ||
				element.ElementType == LanguageElementType.While ||
				element.ElementType == LanguageElementType.Do ||
				element.ElementType == LanguageElementType.Switch;
		}
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			int thisCC = 0;
			LanguageElement parentMember = GetParent(LanguageElementType.Method, LanguageElementType.Property, LanguageElementType.Event);
			LanguageElement parentStatement = GetParentStatementOrVariable();
			if (HasCondition(parentStatement))
				parentStatement = null;
			if (parentMember != null && parentStatement == null)
			{
				thisCC = 1;
			}
			return thisCC + GetChildCyclomaticComplexity();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			LogicalOperation lClone = new LogicalOperation();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public static BinaryOperatorType GetBinaryOperatorType(LogicalOperator op)
		{
			switch (op)
			{
				case LogicalOperator.And:
					return BinaryOperatorType.BitwiseAnd;
				case LogicalOperator.ExclusiveOr:
					return BinaryOperatorType.ExclusiveOr;
				case LogicalOperator.Or:
					return BinaryOperatorType.BitwiseOr;
				case LogicalOperator.ShortCircuitAnd:
					return BinaryOperatorType.BooleanAnd;					
				case LogicalOperator.ShortCircuitOr:
					return BinaryOperatorType.BooleanOr;					
			}
			return BinaryOperatorType.None;
		}
		public static LogicalOperator GetLogicalOperatorType(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.BooleanAnd:
					return LogicalOperator.ShortCircuitAnd;
				case BinaryOperatorType.ExclusiveOr:
					return LogicalOperator.ExclusiveOr;
				case BinaryOperatorType.BooleanOr:
					return LogicalOperator.ShortCircuitOr;
				case BinaryOperatorType.BitwiseAnd:
					return LogicalOperator.And;
				case BinaryOperatorType.BitwiseOr:
					return LogicalOperator.Or;					
			}
			return LogicalOperator.None;
		}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.LogicalOperation;
			}
		}
		#region LogicalOperator
		public LogicalOperator LogicalOperator
		{
			get
			{
				return GetLogicalOperatorType(BinaryOperator);
			}
			set
			{
				SetLogicalOperator(value);
			}
		}
		#endregion
		#region ILogicalOperationExpression Members
		LogicalOperator ILogicalOperationExpression.LogicalOperator
		{
			get
			{
				return LogicalOperator;
			}
		}
		#endregion		
	}
}
