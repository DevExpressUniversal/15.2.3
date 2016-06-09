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
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class SyncDataProxyViewCache : DataProxyViewCache {
		readonly IListServerDataView dataView;
		SyncListWrapper Wrapper { get { return dataView.Wrapper; } }
		readonly Dictionary<int, DataProxy> view;
		public SyncDataProxyViewCache(IListServerDataView dataView)
			: base(dataView.DataAccessor) {
			this.dataView = dataView;
			view = new Dictionary<int, DataProxy>();
		}
		public override DataProxy this[int index] {
			get {
				if (!IndexInRange(index))
					return null;
				if (!view.ContainsKey(index))
					SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index), index));
				return view[index];
			}
			set { throw new NotImplementedException(); }
		}
		bool IndexInRange(int index) {
			return index >= 0 && index < Count;
		}
		void SetProxy(DataProxy proxy) {
			int index = proxy.f_visibleIndex;
			view[index] = proxy;
			proxy.f_RowKey = DataAccessor.HasValueMember ? dataView.GetValueFromProxy(proxy) : Wrapper.GetLoadedRowKey(index);
		}
		public override int Count {
			get { return Wrapper.Count; }
		}
		public override int FindIndexByText(CriteriaOperator criteriaOperand, CriteriaOperator compareOperator, string text, bool isCaseSensitive, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			int localIndex = FindIndexByTextLocal(compareOperator, isCaseSensitive, view.Values, startItemIndex, searchNext, ignoreStartIndex);
			if (localIndex > -1)
				return localIndex;
			return Wrapper.FindRowByText(criteriaOperand, text, startItemIndex, searchNext, ignoreStartIndex);
		}
		public override int FindIndexByValue(CriteriaOperator compareOperator, object value) {
			return Wrapper.FindRowByValue(compareOperator, value);
		}
		public override void Add(int index, DataProxy proxy) {
			SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index), index));
		}
		public override void Replace(int index, DataProxy item) {
			SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index), index));
		}
		public override void Remove(int index) {
			if (view.ContainsKey(index))
				view.Remove(index);
		}
		public override IEnumerator<DataProxy> GetEnumerator() {
			throw new NotImplementedException();
		}
		public override void Reset() {
			view.Clear();
		}
#if DEBUGTEST
		public IDictionary<int, DataProxy> View { get { return view; } }
#endif
	}
}
