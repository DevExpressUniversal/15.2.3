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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ComplexExpression : Expression, IComplexExpression
	{
		ExpressionCollection _Expressions = null;
		public ComplexExpression()
		{
		}
	void SetExpressions(ExpressionCollection expressions)
	{
	  if (_Expressions != null)
		RemoveDetailNodes(_Expressions);
	  _Expressions = expressions;
	  if (_Expressions != null)
		AddDetailNodes(_Expressions);
	}
		public override string ToString()
		{
			if (Expressions == null)
				return String.Empty;
			StringBuilder builder = new StringBuilder();
			int count = Expressions.Count;
			for (int i = 0; i < count; i++)
				if (i > 0)
					builder.AppendFormat(",{0}", Expressions[i].ToString());
				else
					builder.Append(Expressions[i].ToString());
			return builder.ToString();
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
			if (!(source is ComplexExpression))
				return;
			ComplexExpression sourceElement = (ComplexExpression)source;
			if (sourceElement._Expressions != null)
			{
				_Expressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, sourceElement.DetailNodes, _Expressions, sourceElement._Expressions);
				if (_Expressions.Count == 0 && sourceElement._Expressions.Count > 0)
					_Expressions = sourceElement._Expressions.DeepClone(options) as ExpressionCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expressions != null && _Expressions.Contains(oldElement as Expression))
				_Expressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ComplexExpression expression = new ComplexExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
	public void AddExpression(Expression expression)
	{
	  if (expression == null)
		return;
	  Expressions.Add(expression);
	  AddDetailNode(expression);
	}
	public void AddExpressions(IEnumerable<Expression> expressions)
	{
	  if (expressions == null)
		return;
	  foreach (Expression exp in expressions)
		AddExpression(exp);
	}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ComplexExpression; }
		}
		public ExpressionCollection Expressions
		{
			get
			{
				if (_Expressions == null)
					_Expressions = new ExpressionCollection();
				return _Expressions;
			}
	  set { SetExpressions(value); }
		}
		#region IComplexExpression Members
		IExpressionCollection IComplexExpression.Expressions
		{
	  get { return _Expressions; }
		}
		#endregion		
	}
}
