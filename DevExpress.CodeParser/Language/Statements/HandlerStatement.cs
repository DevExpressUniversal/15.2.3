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
	public abstract class HandlerStatement : Statement, IHandlerStatement
	{
		Expression _Expression;
		Expression _AddressExpression;
		#region HandlerStatement
		public HandlerStatement()
		{
		}
		public HandlerStatement(Expression expression, Expression address)
		{
			SetExpression(expression);
			SetAddressExpression(address);
		}
		#endregion
		protected void SetExpression(Expression expression)
		{
			ReplaceNode(_Expression, expression);
			_Expression = expression;
		}
		protected void SetAddressExpression(Expression address)
		{
			ReplaceNode(_AddressExpression, address);
			_AddressExpression = address;
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				SetExpression(newElement as Expression);
			else if (_AddressExpression == oldElement)
				SetAddressExpression(newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is HandlerStatement))
				return;
			HandlerStatement lSource = (HandlerStatement)source;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
			if (lSource._AddressExpression != null)
			{				
				_AddressExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._AddressExpression) as Expression;
				if (_AddressExpression == null)
					_AddressExpression = lSource._AddressExpression.Clone(options) as Expression;
			}			
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			_Expression = null;
			_AddressExpression = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Event;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "HandlerStatement";
		}
		#endregion
		#region Expression
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
		#endregion
		#region AddressExpression
		public Expression AddressExpression
		{
			get
			{
				return _AddressExpression;
			}
			set
			{
				SetAddressExpression(value);
			}
		}
		#endregion
		#region IHandlerStatement Members
		IExpression IHandlerStatement.Expression
		{
			get
			{
				return Expression;
			}
		}
		IExpression IHandlerStatement.AddressExpression
		{
			get
			{
				return AddressExpression;
			}
		}
		#endregion
	}
}
