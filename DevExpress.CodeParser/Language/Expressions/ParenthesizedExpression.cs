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
	public class ParenthesizedExpression : Expression, IParenthesizedExpression
	{
		const int INT_MaintainanceComplexity = 2;
		Expression _Exp;
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		protected ParenthesizedExpression()
		{			
		}
		public ParenthesizedExpression(Expression expression)
		{
			SetExpression(expression);
		}		
		#region SetExpression
		protected void SetExpression(Expression expression)
		{
			if (expression == null)
				return;
			_Exp = expression;
			AddNode(expression);
		}
		#endregion
		#region EvaluateExpression
		protected override object EvaluateExpression()
		{
			if (Expression == null)
				return null;
			return Expression.Evaluate();
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Exp == oldElement)
				_Exp = (Expression)newElement;
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
			if (!(source is ParenthesizedExpression))
				return;
			ParenthesizedExpression lSource = (ParenthesizedExpression)source;
			if (lSource._Exp != null)
			{				
				_Exp = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exp) as Expression;
				if (_Exp == null)
					_Exp = lSource._Exp.Clone(options) as TypeReferenceExpression;
			}			
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return _Exp == null ? "()" : String.Format("({0})", _Exp.ToString());
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (Expression == null)
				return false;
			if (expression is ParenthesizedExpression)
			{
				ParenthesizedExpression lParenExpression = expression as ParenthesizedExpression;
				return Expression.IsIdenticalTo(lParenExpression.Expression);
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
			_Exp = null;
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Exp = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ParenthesizedExpression lClone = new ParenthesizedExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region NeedsInvertParens
		public override bool NeedsInvertParens
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ParenthesizedExpression;
			}
		}
		#endregion
		#region Expression
		public Expression Expression
		{
			get
			{
				return _Exp;
			}
			set
			{
				if (_Exp == value)
					return;
				_Exp = value;
			}
		}
		#endregion
		#region IParenthesizedExpression Members
		IExpression IParenthesizedExpression.Expression
		{
			get
			{
				return Expression;
			}
		}
		#endregion
	}
}
