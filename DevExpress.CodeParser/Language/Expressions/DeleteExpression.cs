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
	public class DeleteExpression : Expression, IDeleteExpression
	{
		Expression _Expression = null;
		protected DeleteExpression()
		{
		}
		public DeleteExpression(Expression expression)
		{
			Name = "delete";
	  SetExpression(expression);
		}
	void SetExpression(Expression expression)
	{
	  if (_Expression != null)
		RemoveDetailNode(_Expression);
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression);
	}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return null;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is DeleteExpression))
				return;
			DeleteExpression sourceElement = (DeleteExpression)source;
			if (sourceElement._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, sourceElement, sourceElement._Expression) as Expression;
				if (_Expression == null)
					_Expression = sourceElement._Expression.Clone(options) as Expression;
			}			
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			DeleteExpression expression = new DeleteExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.DeleteExpression; }
		}
		public Expression Expression
		{
	  get { return _Expression; }
	  set { SetExpression(value); }
		}
		IExpression IDeleteExpression.Expression
		{
	  get { return Expression; }
		}
	}
}
