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
	public class Throw: Abort, IThrowStatement
	{
		Expression _Expression;
	public Throw()
	{
	}
	protected void SetExpression(Expression expression)
	{
	  Expression oldExpression = _Expression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Throw))
				return;
			Throw lSource = (Throw)source;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}			
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Throw;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "throw " + Exception;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Throw lClone = new Throw();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Throw;
			}
		}
		#region Expression
		[Description("The expression thrown.")]
		[Category("Details")]
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				if (_Expression != value)
		  SetExpression(value);
			}
		}
		#endregion
		#region Exception
		[Description("The exception thrown.")]
		[Category("Details")]
		public string Exception
		{
			get
			{
				return _Expression == null ? String.Empty : _Expression.ToString();
			}
		}
		#endregion
		IExpression IThrowStatement.Expression
		{
			get
			{
				return Expression;
			}
		}
	}
}
