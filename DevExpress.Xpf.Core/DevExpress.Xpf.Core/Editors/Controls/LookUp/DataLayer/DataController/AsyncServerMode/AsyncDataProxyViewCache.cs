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
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncDataProxyViewCache : DataProxyViewCache {
		readonly IAsyncListServerDataView dataView;
		AsyncListWrapper Wrapper { get { return dataView.Wrapper; } }
		readonly Dictionary<int, DataProxy> view;
		readonly DataProxy tempProxy;
		public DataProxy TempProxy { get { return tempProxy; } }
		public AsyncDataProxyViewCache(IAsyncListServerDataView dataView)
			: base(dataView.DataAccessor) {
			this.dataView = dataView;
			view = new Dictionary<int, DataProxy>();
			DataAccessor dataAccessor = dataView.DataAccessor;
			tempProxy = dataAccessor.CreateProxy(null, -1);
		}
		public override DataProxy this[int index] {
			get {
				if (Wrapper.IsRowLoaded(index)) {
					if (!view.ContainsKey(index)) {
						SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index, null), index));
					}
					return view[index];
				}
				Wrapper.GetRow(index, LoadRowCompleted);
				return tempProxy;
			}
			set { throw new NotImplementedException(); }
		}
		void SetProxy(DataProxy proxy) {
			int index = proxy.f_visibleIndex;
			view[index] = proxy;
			proxy.f_RowKey = DataAccessor.HasValueMember ? dataView.GetValueFromProxy(proxy) : Wrapper.GetLoadedRowKey(index);
		}
		void LoadRowCompleted(object arguments) {
			CommandGetRow command = (CommandGetRow)arguments;
			int index = command.Index;
			if (index < 0)
				return;
			LoadRowCompletedInternal(command.Index);
		}
		void LoadRowCompletedInternal(int listSourceIndex) {
			dataView.NotifyLoaded(listSourceIndex);
		}
		void FindRowByValueCompleted(object arguments) {
			CommandLocateByValue command = (CommandLocateByValue)arguments;
			int rowIndex = command.RowIndex;
			if (command.RowIndex < 0)
				return;
			var row = Wrapper.GetRow(rowIndex, LoadRowCompleted);
			if (AsyncServerModeDataController.IsNoValue(row))
				return;
			dataView.NotifyLoaded(rowIndex);
		}
		public override int Count {
			get { return Wrapper.Count; }
		}
		public override int FindIndexByText(CriteriaOperator criteriaOperand, CriteriaOperator compareOperator, string text, bool isCaseSensitive, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			int localIndex = FindIndexByTextLocal(compareOperator, isCaseSensitive, view.Values, startItemIndex, searchNext, ignoreStartIndex);
			if (localIndex > -1)
				return localIndex;
			var completer = new FindRowByTextCompleter(dataView, text, startItemIndex, searchNext, ignoreStartIndex);
			Wrapper.FindRowByText(criteriaOperand, text, startItemIndex, searchNext, ignoreStartIndex, completer.Completed);
			return Data.DataController.OperationInProgress;
		}
		public override int FindIndexByValue(CriteriaOperator compareOperator, object value) {
			Wrapper.FindRowByValue(compareOperator, value, FindRowByValueCompleted);
			return Data.DataController.OperationInProgress;
		}
		public virtual int FindIndexByValue(CriteriaOperator compareOperator, object value, OperationCompleted completed) {
			Wrapper.FindRowByValue(compareOperator, value, completed);
			return Data.DataController.OperationInProgress;
		}
		public override void Add(int index, DataProxy proxy) {
			if (Wrapper.IsRowLoaded(index))
				SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index, null), index));
			else
				SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index, LoadRowCompleted), index));
		}
		public override void Replace(int index, DataProxy item) {
			if (Wrapper.IsRowLoaded(index))
				SetProxy(DataAccessor.CreateProxy(Wrapper.GetRow(index, null), index));
			else {
				var proxy = DataAccessor.CreateProxy(null, index);
				proxy.f_component = Wrapper.GetRow(index, LoadRowCompleted);
				SetProxy(proxy);
			}
		}
		public override void Remove(int index) {
			if (view.ContainsKey(index))
				view.Remove(index);
		}
		public override IEnumerator<DataProxy> GetEnumerator() {
			throw new NotImplementedException();
		}
		public bool IsRowLoaded(int listSourceIndex) {
			return Wrapper.IsRowLoaded(listSourceIndex);
		}
		public void FetchItem(int controllerIndex, OperationCompleted action = null) {
			Wrapper.GetRow(controllerIndex, action);
		}
		public void CancelItem(int listSourceIndex) {
			Wrapper.CancelRow(listSourceIndex);
		}
		public void FetchCount() {
			Wrapper.Invalidate();
		}
		public override void Reset() {
			Wrapper.Invalidate();
			view.Clear();
		}
	}
}
