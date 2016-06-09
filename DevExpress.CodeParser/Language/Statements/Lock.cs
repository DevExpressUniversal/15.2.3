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
	public class Lock: ParentToSingleStatement, ILockStatement
	{
		private const int INT_MaintainanceComplexity = 3;		
	Expression _Expression;
		#region Lock
		public Lock()
	{
	}
		#endregion
	public Lock(Expression expression)
	{
	  SetExpression(expression);
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
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Lock))
				return;
			Lock lSource = (Lock)source;
			if (lSource.Expression != null)
			{				
				Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource.Expression) as Expression;
				if (Expression == null)
					Expression = lSource.Expression.Clone(options) as Expression;
			}			
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Lock;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Lock lClone = new Lock();
			lClone.CloneDataFrom(this, options);
			return lClone;
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
				return LanguageElementType.Lock;
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
		IExpression ILockStatement.Expression
		{
			get 
			{
				return Expression;
			}
		}
	}
}
