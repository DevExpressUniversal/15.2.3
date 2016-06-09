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
	public class UncheckedExpression : Expression, IUncheckedExpression
	{
		const int INT_MaintainanceComplexity = 3;
		#region private fields...
		Expression _Exp;
		#endregion
		#region UncheckedExpression
		protected UncheckedExpression()
		{			
		}
		#endregion
		#region UncheckedExpression
		public UncheckedExpression(Expression expression)
		{
	  SetExpression(expression);
		}
		#endregion
	void SetExpression(Expression expression)
	{
	  if (_Exp != null)
		RemoveDetailNode(_Exp);
	  _Exp = expression;
	  if (_Exp != null)
		AddDetailNode(_Exp);
	}
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
			if (!(source is UncheckedExpression))
				return;
			UncheckedExpression lSource = (UncheckedExpression)source;
			if (lSource._Exp != null)
			{				
				_Exp = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exp) as Expression;
				if (_Exp == null)
					_Exp = lSource._Exp.Clone(options) as Expression;
			}		
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			if (_Exp != null)
				return "unchecked(" + _Exp.ToString() + ")";
			return base.ToString();
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.UncheckedExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is UncheckedExpression)
			{
				UncheckedExpression lExpression = (UncheckedExpression)expression;
				if (Expression == null)
					return false;
				return Expression.IsIdenticalTo(lExpression.Expression);
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
			UncheckedExpression lClone = new UncheckedExpression();
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
	  get { return LanguageElementType.UncheckedExpression; }
		}
		#region Expression
		public Expression Expression
		{
	  get { return _Exp; }
	  set { SetExpression(value); }
		}
		#endregion
		#region IUncheckedExpression Members
		IExpression IUncheckedExpression.Expression
		{
	  get { return Expression; }
		}
		#endregion
	}
}
