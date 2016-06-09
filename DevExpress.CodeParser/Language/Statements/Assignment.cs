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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	using AssignmentOperatorTypeEnum = AssignmentOperatorType;
	public class Assignment : Statement, IAssignmentStatement
	{
		private const int INT_MaintainanceComplexity = 1;
		Expression _LeftSide;
		SourceTextRange _Operator;
		AssignmentOperatorType _AssignmentOperatorType;
		Expression _Expression;
		#region Assignment
		public Assignment()
		{
			_AssignmentOperatorType = AssignmentOperatorTypeEnum.Assignment;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Assignment))
				return;			
			Assignment lSource = (Assignment)source;
			_LeftSide = (Expression)GetLanguageElementLink(lSource._LeftSide);
			_Expression = (Expression)GetLanguageElementLink(lSource._Expression);
			_Operator = new SourceTextRange();
			if (lSource._Operator != null)
			{
				_Operator.Text = lSource._Operator.Text;
				_Operator.Range = lSource._Operator.Range.Clone();
			}
	  _AssignmentOperatorType = lSource.AssignmentOperator;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetLeftSide(Expression left)
		{
			if (left == null)
				return;
			ReplaceDetailNode(_LeftSide, left);
			_LeftSide = left;
			SetStart(left.Range);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetExpression(Expression exp)
		{
			if (exp == null)
				return;
			ReplaceDetailNode(_Expression, exp);
			_Expression = exp;
			SetEnd(exp.Range);
		}
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (_LeftSide != null)
				lResult += _LeftSide.ToString();
			if (_Operator != null)
				lResult += _Operator.Text;
			if (_Expression != null)
				lResult += _Expression.ToString();
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Assignment;
		}
		#endregion
		#region FromAssignmentExpression
		public static Assignment FromAssignmentExpression(AssignmentExpression expression)
		{
			if (expression == null)
				return null;
			Assignment lAssignment = new Assignment();
	  lAssignment.SourceExpression = expression;
			expression.TransferCommentsTo(lAssignment);
			lAssignment.LeftSide = expression.LeftSide;
			lAssignment.AddDetailNode(expression.LeftSide);
			SourceTextRange lOperator = new SourceTextRange(expression.OperatorText, expression.OperatorRange);
			lAssignment.Operator = lOperator;
			lAssignment.AssignmentOperator = expression.AssignmentOperator;
			lAssignment.Expression = expression.RightSide;
			lAssignment.AddDetailNode(expression.RightSide);
			lAssignment.SetRange(expression.Range);
			lAssignment.MacroCall = expression.MacroCall;
			return lAssignment;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Assignment lClone = new Assignment();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{						
			if (_LeftSide == oldElement)
				_LeftSide = newElement as Expression;	
			else if (_Expression == oldElement)
				_Expression = newElement as Expression;	
			else
				base.ReplaceOwnedReference(oldElement, newElement);
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
				return LanguageElementType.Assignment;
			}
		}
		#region LeftSide
		[Description("Left side of the assignment statement.")]
		[Category("Details")]
		public Expression LeftSide
		{
			get
			{
				return _LeftSide;
			}
			set
			{
				if (_LeftSide == value)
					return;
		SetLeftSide(value);
			}
		}
		#endregion
		#region Operator
		[Description("Gets assignment operator.")]
		[Category("Details")]
		public SourceTextRange Operator
		{
			get
			{
				return _Operator;
			}
			set
			{
				if (_Operator == value)
					return;
				_Operator = value;
			}
		}
		#endregion
		#region AssignmentOperator
		public AssignmentOperatorType AssignmentOperator
		{
			get
			{
				return _AssignmentOperatorType;
			}
			set
			{
				if (_AssignmentOperatorType == value)
					return;
				_AssignmentOperatorType = value;
			}
		}
		#endregion
		#region AssignmentOperatorType
		[Obsolete("Use AssignmentOperator instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AssignmentOperatorType AssignmentOperatorType
		{
			get
			{
				return AssignmentOperator;
			}
			set
			{
				AssignmentOperator = value;
			}
		}
		#endregion
		#region Expression
		[Description("Right side of the assignment statement.")]
		[Category("Details")]
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				if (_Expression == value)
					return;
		SetExpression(value);
			}
		}
		#endregion
		#region IAssignmentStatement Members
		IExpression IAssignmentStatement.LeftSide
		{
			get
			{
				return LeftSide;
			}
		}
		IExpression IAssignmentStatement.Expression
		{
			get
			{
				return Expression;
			}
		}
		AssignmentOperatorType IAssignmentStatement.AssignmentOperator
		{
			get
			{
				return AssignmentOperator;
			}
		}
		#endregion
	}
}
