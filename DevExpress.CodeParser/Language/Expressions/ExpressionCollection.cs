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
using System.Collections;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ExpressionCollection : ExpressionCollectionBase, IExpressionCollection, IEnumerable<Expression>
	{
		protected override NodeList CreateInstance()
		{
			return new ExpressionCollection();			
		}
		#region Add
		public int Add(Expression expression)
		{
			return base.Add(expression);
		}
		#endregion
		#region AddRange
		public void AddRange(ExpressionCollection collection)
		{
			base.AddRange(collection);
		}
		#endregion
		#region IndexOf
		public int IndexOf(Expression expression)
		{
			return base.IndexOf(expression);
		}
		#endregion
		#region Insert
		public void Insert(int index, Expression expression)
		{
			base.Insert(index, expression);
		}
		#endregion
		#region Remove
		public void Remove(Expression expression)
		{
			base.Remove(expression);
		}
		#endregion
		#region Find
		public Expression Find(Expression expression)
		{
			return (Expression) base.Find(expression);
		}
		#endregion
		#region Contains
		public bool Contains(Expression expression)
		{
			return (Find(expression) != null);
		}
		#endregion
		#region this[int aIndex]
		public new Expression this[int index] 
		{
			get
			{
				return (Expression) base[index];
			}
			set
			{
				base[index] = value;
			}
		}
		#endregion
		#region IExpressionCollection Members
		int IExpressionCollection.IndexOf(IExpression e)
		{
			return IndexOf(e);
		}
		IExpression IExpressionCollection.this[int index]
		{
			get
			{				
				return this[index] as IExpression;
			}
		}
		#endregion
	IEnumerator<Expression> IEnumerable<Expression>.GetEnumerator()
	{
	  IEnumerator enumerator = base.GetEnumerator();
	  while (enumerator.MoveNext())
	  {
		Expression expression = enumerator.Current as Expression;
		if (expression != null)
		  yield return expression;
	  }
	}
  }
}
