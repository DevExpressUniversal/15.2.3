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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum UnaryOperatorType : byte
	{
		None,
		Decrement,
		Increment,
		UnaryNegation,
		UnaryPlus,
		LogicalNot,
		AddressOf,
		OnesComplement,
		PointerDereference,
		TrackingReference
	}
	public class UnaryOperatorExpression : OperatorExpression, IUnaryOperatorExpression
	{
		const int INT_MaintainanceComplexity = 2;
		UnaryOperatorType _Operator;
		Expression _Expression;
		bool _IsPostOperator;
		public UnaryOperatorExpression()
		{
			_IsPostOperator = false;
			_Expression = null;
		}
		public UnaryOperatorExpression(Token token, Expression expression)
			: this(token, expression, false)
		{
		}
		public UnaryOperatorExpression(Expression expression)
			: this(expression, false)
		{
		}
		public UnaryOperatorExpression(Token token, Expression expression, bool isPostOperator)
			: base (token)
		{
			_IsPostOperator = isPostOperator;
			SetExpression(expression);
		}
		public UnaryOperatorExpression(Expression expression, bool isPostOperator)
		{
			_IsPostOperator = isPostOperator;
			SetExpression(expression);
		}
		public UnaryOperatorExpression(string op, Expression expression, bool isPostOperator)
			: base(op)
		{
			_IsPostOperator = isPostOperator;
			SetExpression(expression);
		}
		public UnaryOperatorExpression(UnaryOperatorType op, Expression expression, bool isPostOperator)
	  : this (expression, false)
		{
			_IsPostOperator = isPostOperator;
			_Operator = op;
			SetExpression(expression);
		}
		protected void SetExpression(Expression expression)
		{
	  if (_Expression != null)
		RemoveDetailNode(_Expression);
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression);
		}
		[Obsolete("Use SetExpression instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void SetUnaryExpression(Expression expression)
		{
	  SetExpression(expression);
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is UnaryOperatorExpression))
				return;
			UnaryOperatorExpression lSource = (UnaryOperatorExpression)source;
			_IsPostOperator = lSource._IsPostOperator;
			_Operator = lSource._Operator;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
		}
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			string lExpression = String.Empty;
			if (Expression != null)
				lExpression = Expression.ToString();
			if (IsPostOperator)
				lResult.AppendFormat("{0}{1}", lExpression, OperatorText);
			else
				lResult.AppendFormat("{0}{1}", OperatorText, lExpression);
			return lResult.ToString();
		}
		public override int GetImageIndex()
		{
			return ImageIndex.UnaryOperator;
		}
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is UnaryOperatorExpression)
			{
				UnaryOperatorExpression lOperation = expression as UnaryOperatorExpression;
				if (Expression == null)
					return false;
				return Expression.IsIdenticalTo(lOperation.Expression) && 
					OperatorText == lOperation.OperatorText &&
					IsPostOperator == lOperation.IsPostOperator;
			}
			return false;
		}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_Expression = null;
			base.OwnedReferencesTransfered();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Expression = null;
			base.CleanUpOwnedReferences();
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			UnaryOperatorExpression lClone = new UnaryOperatorExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	#region GetOverloadableOperatorType
	public static OperatorType GetOverloadableOperatorType(UnaryOperatorType unary)
	{
	  switch (unary)
	  {
		case UnaryOperatorType.None:
		  return OperatorType.None;
		case UnaryOperatorType.UnaryPlus:
		  return OperatorType.UnaryPlus;
		case UnaryOperatorType.UnaryNegation:
		  return OperatorType.UnaryNegation;
		case UnaryOperatorType.LogicalNot:
		  return OperatorType.LogicalNot;
		case UnaryOperatorType.Increment:
		  return OperatorType.Increment;
		case UnaryOperatorType.Decrement:
		  return OperatorType.Decrement;
		case UnaryOperatorType.OnesComplement:
		  return OperatorType.OnesComplement;
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
				return LanguageElementType.UnaryOperatorExpression;
			}
		}
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				SetExpression(value);
			}
		}
		[Obsolete("Use Expression instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Expression UnaryExpression
		{
			get
			{
				return Expression;
			}
			set
			{
				Expression = value;
			}
		}
		public bool IsPostOperator
		{
			get
			{
				return _IsPostOperator;
			}
			set
			{
				if (_IsPostOperator == value)
					return;
				_IsPostOperator = value;
			}
		}
		public UnaryOperatorType UnaryOperator
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
		[Obsolete("Use UnaryOperator instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public UnaryOperatorType Operator
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
		#region IUnaryOperatorExpression Members
		IExpression IUnaryOperatorExpression.UnaryExpression
		{
			get
			{
				return Expression;
			}
		}
		IExpression IUnaryOperatorExpression.Expression
		{
			get
			{
				return Expression;
			}
		}
		bool IUnaryOperatorExpression.IsPostOperator
		{
			get
			{
				return IsPostOperator;
			}
		}
		UnaryOperatorType IUnaryOperatorExpression.UnaryOperator
		{
			get
			{
				return UnaryOperator;
			}
		}
		#endregion
	}
}
