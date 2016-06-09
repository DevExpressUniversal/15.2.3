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
using System.Collections.Generic;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	abstract class CustomComparerBase<TColumn, TObject> where TColumn : QueryColumn {
		protected readonly Func<IQueryMemberProvider, IQueryMemberProvider, int?> sortMethod;
		protected readonly TColumn column;
		protected readonly IComparer<TObject> comparer;
		public CustomComparerBase(Func<IQueryMemberProvider, IQueryMemberProvider, int?> sortMethod, TColumn column, IComparer<TObject> basic) {
			this.sortMethod = sortMethod;
			this.column = column;
			this.comparer = basic;
		}
	}
	class CustomComparer<TColumn, TObject> : CustomComparerBase<TColumn, TObject>, IComparer<TObject>
		where TObject : IQueryMemberProvider
		where TColumn : QueryColumn {
		public CustomComparer(Func<IQueryMemberProvider, IQueryMemberProvider, int?> sortMethod, TColumn column, IComparer<TObject> comparer)
			: base(sortMethod, column, comparer) {
		}
		int IComparer<TObject>.Compare(TObject x, TObject y) {
			int? value = sortMethod(x, y);
			if(!value.HasValue || value.Value == 0)
				return comparer.Compare(x, y);
			return value.Value;
		}
	}
	class CustomComparer<TColumn> : CustomComparerBase<TColumn, QueryMember>, IComparer<QueryMember> where TColumn : QueryColumn {
		class QueryMemberWrapper : IQueryMemberProvider {
			public QueryMember Member { get; set; }
		}
		QueryMemberWrapper a = new QueryMemberWrapper(), b = new QueryMemberWrapper();
		public CustomComparer(Func<IQueryMemberProvider, IQueryMemberProvider, int?> sortMethod, TColumn column, IComparer<QueryMember> comparer)
			: base(sortMethod, column, comparer) {
		}
		int IComparer<QueryMember>.Compare(QueryMember x, QueryMember y) {
			a.Member = x;
			b.Member = y;
			int? value = sortMethod(a, b);
			if(!value.HasValue || value.Value == 0)
				return comparer.Compare(x, y);
			return value.Value;
		}
	}
}
