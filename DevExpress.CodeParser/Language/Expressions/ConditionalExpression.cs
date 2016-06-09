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
	public class ConditionalExpression : Expression, IConditionalExpression
	{
		const int INT_MaintainanceComplexity = 6;
		Expression _Condition;
		Expression _TrueExpression;
		Expression _FalseExpression;
		bool _IsNeededResultsCasting = false;
		#region ConditionalExpression()
		protected ConditionalExpression()
		{
		}
		#endregion
		#region ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		public ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		{
			SetCondition(condition);
			SetTrueExpression(trueExpression);
			SetFalseExpression(falseExpression);
		}
		#endregion
		protected void SetCondition(Expression condition)
		{
	  if (_Condition != null)
		RemoveDetailNode(_Condition);
	  _Condition = condition;
	  if (_Condition != null)
		AddDetailNode(_Condition);
		}
		#region SetTrueExpression(Expression trueExpression)
		protected void SetTrueExpression(Expression trueExpression)
		{
	  if (_TrueExpression != null)
		RemoveDetailNode(_TrueExpression);
	  _TrueExpression = trueExpression;
	  if (_TrueExpression != null)
		AddDetailNode(_TrueExpression);
		}
		#endregion
		#region SetFalseExpression(Expression falseExpression)
		protected void SetFalseExpression(Expression falseExpression)
		{
	  if (_FalseExpression != null)
		RemoveDetailNode(_FalseExpression);
	  _FalseExpression = falseExpression;
	  if (_FalseExpression != null)
		AddDetailNode(_FalseExpression);
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Condition == oldElement)
				_Condition = (Expression)newElement;
			else if (_TrueExpression == oldElement)
				_TrueExpression = (Expression)newElement;
			else if (_FalseExpression == oldElement)
				_FalseExpression = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ConditionalExpression))
				return;
			ConditionalExpression lSource = (ConditionalExpression)source;
			if (lSource._Condition != null)
			{				
				_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as Expression;
				if (_Condition == null)
					_Condition = lSource._Condition.Clone(options) as Expression;
			}
			if (lSource._TrueExpression != null)
			{				
				_TrueExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._TrueExpression) as Expression;
				if (_TrueExpression == null)
					_TrueExpression = lSource._TrueExpression.Clone(options) as Expression;
			}
			if (lSource._FalseExpression != null)
			{				
				_FalseExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._FalseExpression) as Expression;
				if (_FalseExpression == null)
					_FalseExpression = lSource._FalseExpression.Clone(options) as Expression;
			}
			_IsNeededResultsCasting = lSource._IsNeededResultsCasting;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (_Condition != null)
				lResult += _Condition.ToString() + " ";
			lResult += "? ";
			if (_TrueExpression != null)
				lResult += _TrueExpression.ToString() + " ";
			lResult += ": ";
			if (_FalseExpression != null)
				lResult += _FalseExpression.ToString();
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ConditionalExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ConditionalExpression)
			{
				ConditionalExpression lExpression = (ConditionalExpression)expression;
				if (Condition == null || !Condition.IsIdenticalTo(lExpression.Condition))
					return false;
				if (TrueExpression == null || !TrueExpression.IsIdenticalTo(lExpression.TrueExpression))
					return false;
				if (FalseExpression == null || !FalseExpression.IsIdenticalTo(lExpression.FalseExpression))
					return false;
				return true;
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
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			return 1 + GetChildCyclomaticComplexity();
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_Condition = null;
			_TrueExpression = null;
			_FalseExpression = null;
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Condition = null;
			_TrueExpression = null;
			_FalseExpression = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ConditionalExpression lClone = new ConditionalExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
	  get { return INT_MaintainanceComplexity; }
		}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ConditionalExpression; }
		}
		#region Condition
		public Expression Condition
		{
	  get { return _Condition; }
	  set { SetCondition(value); }
		}
		#endregion
		#region TrueExpression
		public Expression TrueExpression
		{
	  get { return _TrueExpression; }
	  set { SetTrueExpression(value); }
		}
		#endregion
		#region FalseExpression
		public Expression FalseExpression
		{
	  get { return _FalseExpression; }
	  set { SetFalseExpression(value); }
		}
		#endregion
		#region IConditionalExpression Members
		IExpression IConditionalExpression.Condition
		{
	  get { return Condition; }
		}
		IExpression IConditionalExpression.TrueExpression
		{
	  get { return TrueExpression; }
		}
		IExpression IConditionalExpression.FalseExpression
		{
	  get { return FalseExpression; }
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsNeededResultsCasting
		{
	  get { return _IsNeededResultsCasting; }
	  set { _IsNeededResultsCasting = value; }
		}
	}
}
