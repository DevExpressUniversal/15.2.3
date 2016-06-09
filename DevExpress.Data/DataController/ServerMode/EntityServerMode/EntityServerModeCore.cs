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
using System.Linq;
using System.Text;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Linq {
	public class EntityServerModeCache : LinqServerModeCache {
		public EntityServerModeCache(IQueryable q, CriteriaOperator[] keysCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary) :
			base(q, keysCriteria, sortInfo, groupCount, summary, totalSummary) {
		}
		protected override ICriteriaToExpressionConverter Converter {
			get { return new CriteriaToEFExpressionConverter(); }
		}
	}
	public class EntityServerModeCore : LinqServerModeCore {
		public EntityServerModeCore(IQueryable queryable, CriteriaOperator[] keys)
			: base(queryable, keys) {			
		}
		protected override ServerModeCore DXCloneCreate() {
			return new EntityServerModeCore(Q, this.KeysCriteria);
		}
		protected override ServerModeCache CreateCacheCore() {
			IQueryable filtered;
			try {
				filtered = Q.AppendWhere(Converter, FilterClause);
			} catch (Exception e) {
				RaiseExceptionThrown(new ServerModeExceptionThrownEventArgs(e));
				filtered = Array.CreateInstance(Q.ElementType, 0).AsQueryable();
			}
			EntityServerModeCache rv = new EntityServerModeCache(filtered, KeysCriteria, SortInfo, GroupCount, SummaryInfo, TotalSummaryInfo);
			rv.IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria = FilterClause;
			return rv;
		}
		protected override ICriteriaToExpressionConverter Converter {
			get { return new CriteriaToEFExpressionConverter(); }		
		}
	}
	public class EntityServerModeFrontEnd : IListServer, IListServerHints, IBindingList, ITypedList, IDXCloneable {
		public readonly ILinqServerModeFrontEndOwner Owner;
		IListServer _Wrapper;
		Type _Type;
		string _KeyExpression;
		IQueryable _DataSource;
		bool _IsReadyForTakeOff;
		string _DefaultSorting;
		#region reload store
		CriteriaOperator _Successful_FilterCriteria;
		ICollection<ServerModeOrderDescriptor> _Successful_sortInfo;
		int _Successful_groupCount;
		ICollection<ServerModeSummaryDescriptor> _Successful_summaryInfo;
		ICollection<ServerModeSummaryDescriptor> _Successful_totalSummaryInfo;
		#endregion
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual EntityServerModeFrontEnd DXClone() {
			EntityServerModeFrontEnd clone = CreateDXClone();
			clone._Type = this._Type;
			clone._KeyExpression = this._KeyExpression;
			clone._DataSource = this._DataSource;
			clone._IsReadyForTakeOff = this._IsReadyForTakeOff;
			clone._DefaultSorting = this._DefaultSorting;
			clone._Successful_FilterCriteria = this._Successful_FilterCriteria;
			clone._Successful_sortInfo = this._Successful_sortInfo;
			clone._Successful_groupCount = this._Successful_groupCount;
			clone._Successful_summaryInfo = this._Successful_summaryInfo;
			clone._Successful_totalSummaryInfo = this._Successful_totalSummaryInfo;
			return clone;
		}
		protected virtual EntityServerModeFrontEnd CreateDXClone() {
			return new EntityServerModeFrontEnd(this.Owner);
		}
		protected IListServer Wrapper {
			get {
				if (_Wrapper == null) {
					_Wrapper = CreateWrapper();
					_Wrapper.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_Wrapper_InconsistencyDetected);
					_Wrapper.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_Wrapper_ExceptionThrown);
					_Wrapper.Apply(_Successful_FilterCriteria, _Successful_sortInfo, _Successful_groupCount, _Successful_summaryInfo, _Successful_totalSummaryInfo);
				}
				return _Wrapper;
			}
		}
		void _Wrapper_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			if (InconsistencyDetected == null)
				return;
			InconsistencyDetected(this, e);
		}
		void _Wrapper_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			if (ExceptionThrown == null)
				return;
			ExceptionThrown(this, e);
		}
		private IListServer CreateWrapper() {
			if (_IsReadyForTakeOff)
				return CreateRuntimeWrapper();
			else
				return new LinqServerModeDesignTimeWrapper();
		}
		protected virtual IListServer CreateRuntimeWrapper() {
			EntityServerModeCore core = new EntityServerModeCore(_DataSource, CriteriaOperator.ParseList(_KeyExpression));
			core.DefaultSorting = _DefaultSorting;
			return core;
		}
		protected void KillWrapper() {
			_Wrapper = null;
		}
		public EntityServerModeFrontEnd(ILinqServerModeFrontEndOwner owner) {
			this.Owner = owner;
			CatchUp();
		}
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) {
		}
		object IBindingList.AddNew() {
			if(_IsReadyForTakeOff)
			throw new NotSupportedException();
			else
				return new object();
		}
		bool IBindingList.AllowEdit {
			get { return false; }
		}
		bool IBindingList.AllowNew {
			get { return false; }
		}
		bool IBindingList.AllowRemove {
			get { return false; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		bool IBindingList.IsSorted {
			get { return false; }
		}
		public event ListChangedEventHandler ListChanged {
			add { _ListChanged += value; }
			remove { _ListChanged -= value; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
		}
		void IBindingList.RemoveSort() {
		}
		ListSortDirection IBindingList.SortDirection {
			get { throw new NotSupportedException(); }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { throw new NotSupportedException(); }
		}
		bool IBindingList.SupportsChangeNotification {
			get { return true; } 
		}
		bool IBindingList.SupportsSearching {
			get { return false; }
		}
		bool IBindingList.SupportsSorting {
			get { return false; }
		}
		#endregion
		event ListChangedEventHandler _ListChanged;
		protected virtual void OnListChanged(ListChangedEventArgs e) {
			if(_ListChanged != null)
				_ListChanged(this, e);
		}
		#region IListServer Members
		public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			Wrapper.Apply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo);
			_Successful_FilterCriteria = filterCriteria;
			_Successful_sortInfo = sortInfo;
			_Successful_groupCount = groupCount;
			_Successful_summaryInfo = summaryInfo;
			_Successful_totalSummaryInfo = totalSummaryInfo;
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return Wrapper.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Wrapper.LocateByValue(expression, value, startIndex, searchUp);
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Wrapper.GetGroupInfo(parentGroup);
		}
		public int GetRowIndexByKey(object key) {
			return Wrapper.GetRowIndexByKey(key);
		}
		public object GetRowKey(int index) {
			return Wrapper.GetRowKey(index);
		}
		public List<object> GetTotalSummary() {
			return Wrapper.GetTotalSummary();
		}
		public object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return Wrapper.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
		}
		public virtual IList GetAllFilteredAndSortedRows() {
			return Wrapper.GetAllFilteredAndSortedRows();
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Wrapper.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		#endregion
		#region IList Members
		public int Add(object value) {
			return Wrapper.Add(value);
		}
		public void Clear() {
			Wrapper.Clear();
		}
		public bool Contains(object value) {
			return Wrapper.Contains(value);
		}
		public int IndexOf(object value) {
			return Wrapper.IndexOf(value);
		}
		public void Insert(int index, object value) {
			Wrapper.Insert(index, value);
		}
		public bool IsFixedSize {
			get { return Wrapper.IsFixedSize; }
		}
		public bool IsReadOnly {
			get { return Wrapper.IsReadOnly; }
		}
		public void Remove(object value) {
			Wrapper.Remove(value);
		}
		public void RemoveAt(int index) {
			Wrapper.RemoveAt(index);
		}
		public object this[int index] {
			get {
				return Wrapper[index];
			}
			set {
				Wrapper[index] = value; ;
			}
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			Wrapper.CopyTo(array, index);
		}
		public int Count {
			get { return Wrapper.Count; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return Wrapper.GetEnumerator();
		}
		#endregion
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ListBindingHelper.GetListItemProperties(_Type, listAccessors);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return ListBindingHelper.GetListName(_Type, listAccessors);
		}
		#endregion
		public void CatchUp() {
			bool reset = false, pdreset = false, takeoff = false;
			Type t = Owner.ElementType;
			if(t == null)
				t = typeof(object);
			if(_Type != t) {
				_Type = t;
				pdreset = true;
			}
			if(_DataSource != Owner.QueryableSource) {
				_DataSource = Owner.QueryableSource;
				reset = true;
			}
			if(_KeyExpression != Owner.KeyExpression) {
				_KeyExpression = Owner.KeyExpression;
				reset = true;
			}
			if(_DefaultSorting != Owner.DefaultSorting) {
				_DefaultSorting = Owner.DefaultSorting;
				reset = true;
			}
			if(_IsReadyForTakeOff != Owner.IsReadyForTakeOff()) {
				_IsReadyForTakeOff = Owner.IsReadyForTakeOff();
				takeoff = true;
			}
			if(reset || takeoff)
				KillWrapper();
			if(pdreset)
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
			if(reset)
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public void Refresh() {
			Wrapper.Refresh();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		void IListServerHints.HintGridIsPaged(int pageSize) {
			IListServerHints n = Wrapper as IListServerHints;
			if(n == null)
				return;
			n.HintGridIsPaged(pageSize);
		}
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			IListServerHints n = Wrapper as IListServerHints;
			if(n == null)
				return;
			n.HintMaxVisibleRowsInGrid(rowsInGrid);
		}		
	}
}
