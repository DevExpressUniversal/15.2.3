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
using System.ComponentModel;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum BinaryOperatorType : byte
	{
		None,
		Add,
		Assign,
	BitwiseAnd,
		BitwiseOr,
		BooleanAnd,
		BooleanOr,
		ExclusiveOr,
		Divide,
		GreaterThan,
		GreaterThanOrEqual,
		IdentityEquality,
		IdentityInequality,
		LessThan,
		LessThanOrEqual,
		Modulus,
		Multiply,
		Subtract,
		ShiftLeft,
		ShiftRight,
		UnsignedShiftRight,
		ValueEquality,
		Like,
		IntegerDivision,
		Exponentiation,
		StrictEquality,
		StrictInequality,
		In,
		Concatenation,
	PipeLeft,
	PipeRight,
	ComposeLeft,
	ComposeRight,
	Append,
	Cons
	}
	public class BinaryOperatorExpression : OperatorExpression, IBinaryOperatorExpression
	{
		const int INT_MaintainanceComplexity = 5;
		#region private fields...
		BinaryOperatorType _Operator;
		Expression _LeftSide;
		Expression _RightSide;
		#endregion
		#region BinaryOperatorExpression
		public BinaryOperatorExpression()
		{
		}
		#endregion
		#region BinaryOperatorExpression
		public BinaryOperatorExpression(Expression left, Token token, Expression right)
			: base (token)
		{
			SetLeftSide(left);
			SetRightSide(right);
		}
		#endregion
		#region BinaryOperatorExpression
		public BinaryOperatorExpression(Expression left, Expression right)
		{
			SetLeftSide(left);
			SetRightSide(right);
		}
		#endregion
		#region BinaryOperatorExpression
		public BinaryOperatorExpression(Expression left, string operatorText, Expression right)
			: base(operatorText)
		{
			SetLeftSide(left);
			SetRightSide(right);
		}
		#endregion
		#region BinaryOperatorExpression
		public BinaryOperatorExpression(Expression left, BinaryOperatorType operatorType, Expression right)
		{
			BinaryOperator = operatorType;
			SetLeftSide(left);
			SetRightSide(right);
		}
		#endregion
		#region SetLeftSide(Expression left)
		protected void SetLeftSide(Expression left)
		{
	  if (_LeftSide != null)
		RemoveDetailNode(_LeftSide);
	  _LeftSide = left;
	  if (_LeftSide != null)
		AddDetailNode(_LeftSide);
		}
		#endregion
		#region SetRightSide(Expression right)
		protected void SetRightSide(Expression right)
		{
	  if (_RightSide != null)
		RemoveDetailNode(_RightSide);
	  _RightSide = right;
	  if (_RightSide != null)
		AddDetailNode(_RightSide);
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_LeftSide == oldElement)
				_LeftSide = newElement as Expression;
			else if (_RightSide == oldElement)
				_RightSide = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is BinaryOperatorExpression))
				return;
			BinaryOperatorExpression lSource = (BinaryOperatorExpression)source;
			_Operator = lSource._Operator;
			if (lSource._LeftSide != null)
			{
				_LeftSide = ParserUtils.GetCloneFromNodes(this, lSource, lSource._LeftSide) as Expression;
				if (_LeftSide == null)
					_LeftSide = lSource._LeftSide.Clone(options) as Expression;
			}
			if (lSource._RightSide != null)
			{
				_RightSide = ParserUtils.GetCloneFromNodes(this, lSource, lSource._RightSide) as Expression;
				if (_RightSide == null)
					_RightSide = lSource._RightSide.Clone(options) as Expression;
			}
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			if (LeftSide != null)
				lResult.AppendFormat("{0} ", LeftSide.ToString());
			if (OperatorText != String.Empty)
				lResult.AppendFormat("{0} ", OperatorText);
			if (RightSide != null)
				lResult.AppendFormat("{0}", RightSide.ToString());
			return lResult.ToString();
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.BinaryOperator;
		}
		#endregion
		#region GetChildNodeDescription
		public override string GetDetailNodeDescription(int index)
		{
			if (index == 0)
				return "Left Side";
			else if (index == 1)
				return "Right Side";
			else
				return base.GetDetailNodeDescription(index);
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is BinaryOperatorExpression)
			{
				BinaryOperatorExpression lOperation = expression as BinaryOperatorExpression;
				if (LeftSide == null)
					return false;
				if (RightSide == null)
					return false;
				return LeftSide.IsIdenticalTo(lOperation.LeftSide) && 
					OperatorText == lOperation.OperatorText &&
					RightSide.IsIdenticalTo(lOperation.RightSide);
			}
			return false;
		}
		#endregion
		#region Resolve
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_LeftSide = null;
			_RightSide = null;
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_LeftSide = null;
			_RightSide = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			BinaryOperatorExpression lClone = new BinaryOperatorExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	#region GetOverloadableOperatorType
	public static OperatorType GetOverloadableOperatorType(BinaryOperatorType binary)
	{
	  switch (binary)
	  {
		case BinaryOperatorType.None:
		  return OperatorType.None;
		case BinaryOperatorType.Add:
		  return OperatorType.Addition;
		case BinaryOperatorType.Subtract:
		  return OperatorType.Subtraction;
		case BinaryOperatorType.Multiply:
		  return OperatorType.Multiply;
		case BinaryOperatorType.Divide:
		  return OperatorType.Division;
		case BinaryOperatorType.Modulus:
		  return OperatorType.Modulus;
		case BinaryOperatorType.BitwiseAnd:
		  return OperatorType.BitwiseAnd;
		case BinaryOperatorType.BitwiseOr:
		  return OperatorType.BitwiseOr;
		case BinaryOperatorType.Exponentiation:
		  return OperatorType.Exponent;
		case BinaryOperatorType.BooleanAnd:
		  return OperatorType.LogicalAnd;
		case BinaryOperatorType.BooleanOr:
		  return OperatorType.LogicalOr;
		case BinaryOperatorType.ExclusiveOr:
		  return OperatorType.ExclusiveOr;
		case BinaryOperatorType.GreaterThan:
		  return OperatorType.GreaterThan;
		case BinaryOperatorType.GreaterThanOrEqual:
		  return OperatorType.GreaterThanOrEqual;
		case BinaryOperatorType.IdentityEquality:
		  return OperatorType.Equality;
		case BinaryOperatorType.IdentityInequality:
		  return OperatorType.Inequality;
		case BinaryOperatorType.LessThan:
		  return OperatorType.LessThan;
		case BinaryOperatorType.LessThanOrEqual:
		  return OperatorType.LessThanOrEqual;
		case BinaryOperatorType.ShiftLeft:
		  return OperatorType.LeftShift;
		case BinaryOperatorType.ShiftRight:
		  return OperatorType.SignedRightShift;
		case BinaryOperatorType.UnsignedShiftRight:
		  return OperatorType.RightShift;
		case BinaryOperatorType.ValueEquality:
		  return OperatorType.Equality;
		case BinaryOperatorType.IntegerDivision:
		  return OperatorType.IntegerDivision;
		case BinaryOperatorType.StrictEquality:
		  return OperatorType.Equality;
		case BinaryOperatorType.StrictInequality:
		  return OperatorType.Inequality;
		case BinaryOperatorType.Like:
		  return OperatorType.Like;
		case BinaryOperatorType.Concatenation:
		  return OperatorType.Concatenate;
	  }
	  return OperatorType.None;
	}
	#endregion
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
				return LanguageElementType.BinaryOperatorExpression;
			}
		}
		#region LeftSide
		public Expression LeftSide
		{
			get
			{
				return _LeftSide;
			}
			set
			{
				SetLeftSide(value);
			}
		}
		#endregion
		public BinaryOperatorType BinaryOperator
		{
			get
			{
				return _Operator;
			}
			set
			{
				_Operator = value;
			}
		}
		[Obsolete("Use BinaryOperator instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BinaryOperatorType Operator
		{
			get
			{
				return _Operator;
			}
			set
			{
				_Operator = value;
			}
		}
		#region RightSide
		public Expression RightSide
		{
			get
			{
				return _RightSide;
			}
			set
			{
				SetRightSide(value);
			}
		}
		#endregion
		#region IBinaryOperatorExpression Members
		IExpression IBinaryOperatorExpression.LeftSide
		{
			get
			{
				return LeftSide;
			}
		}
		IExpression IBinaryOperatorExpression.RightSide
		{
			get
			{
				return RightSide;
			}
		}
		#endregion
	}
}
