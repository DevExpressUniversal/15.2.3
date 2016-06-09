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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace DevExpress.Xpo.Helpers {
	public class XpoLinqQuery<T> : IOrderedQueryable<T> {
		Expression queryExpression;
		XpoLinqQueryProvider queryProvider;
		public XpoLinqQuery(XpoLinqQueryProvider queryProvider) {
			if (queryProvider == null) {
				throw new ArgumentNullException("provider");
			}
			this.queryProvider = queryProvider;
			this.queryExpression = Expression.Constant(this);
		}
		public XpoLinqQuery(XpoLinqQueryProvider queryProvider, Expression queryExpression) {
			if (queryProvider == null) {
				throw new ArgumentNullException("provider");
			}
			if (queryExpression == null) {
				throw new ArgumentNullException("expression");
			}
			if (!queryExpression.Type.IsGenericType) {
				throw new ArgumentOutOfRangeException("expression");
			}
			this.queryProvider = queryProvider;
			this.queryExpression = queryExpression;
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			object result = this.queryProvider.Execute(this.queryExpression);			
			return ((IEnumerable<T>)result).GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)this.queryProvider.Execute(this.queryExpression)).GetEnumerator();
		}
		#endregion
		#region IQueryable Members
		public Type ElementType {
			get { return typeof(T); }
		}
		public Expression Expression {
			get { return this.queryExpression; }
		}
		public IQueryProvider Provider {
			get { return this.queryProvider; }
		}
		#endregion
	}
}
