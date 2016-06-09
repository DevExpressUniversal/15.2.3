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
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class LocalDataProxyViewCache : DataProxyViewCache {
		ChunkList2<DataProxy> view;
		public override DataProxy this[int index] {
			get { return view[index]; }
			set { view[index] = value; }
		}
		public override int Count {
			get { return view.Count; }
		}
		public LocalDataProxyViewCache(DataAccessor accessor, IEnumerable<DataProxy> listSource)
			: base(accessor) {
			view = new ChunkList2<DataProxy>(listSource, "Tag");
		}
		public override int FindIndexByText(CriteriaOperator compareOperand, CriteriaOperator compareOperator, string text, bool isCaseSensitive, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			return FindIndexByTextLocal(compareOperator, isCaseSensitive, view, startItemIndex, searchNext, ignoreStartIndex);
		}
		public override int FindIndexByValue(CriteriaOperator compareOperator, object value) {
			throw new NotImplementedException();
		}
		public override void Add(int index, DataProxy item) {
			view.Insert(index, item);
		}
		public override void Replace(int index, DataProxy item) {
			view[index] = item;
		}
		public override void Remove(int index) {
			view.RemoveAt(index);
		}
		public override IEnumerator<DataProxy> GetEnumerator() {
			return view.GetEnumerator();
		}
		public void ApplySortGroupFilter(Func<IList<DataProxy>, ChunkList2<DataProxy>> performSortGroupFilter) {
			view = performSortGroupFilter(view);
		}
		public override void Reset() {
		}
		public int BinarySearch(DataProxy proxy, int startIndex, int count, IComparer<object> comparer) {
			return view.BinarySearch(proxy, startIndex, count, comparer);
		}
		protected override int FindIndexByItem(DataProxy item) {
			int index = base.FindIndexByItem(item);
			if (index >= 0 && index < Count) {
				var testItem = view[index];
				if (ReferenceEquals(testItem, item))
					return index;
			}
			index = view.IndexOf(item);
			item.f_visibleIndex = index;
			return index;
		}
		protected override bool SkipInternal(int startItemIndex, bool searchNext, bool ignoreStartIndex, DataProxy target) {
			int i = 0;
			foreach (var item in view)
				item.f_visibleIndex = i++;
			return base.SkipInternal(startItemIndex, searchNext, ignoreStartIndex, target);
		}
	}
}
