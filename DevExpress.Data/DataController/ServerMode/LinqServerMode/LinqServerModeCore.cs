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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using System.Linq.Expressions;
using DevExpress.Data.Helpers;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#else
using System.Windows.Forms;
#endif
using System.Threading;
namespace DevExpress.Data.Linq.Helpers {
	public class LinqServerModeCache: ServerModeKeyedCache {
		public readonly IQueryable Q;
		public LinqServerModeCache(IQueryable q, CriteriaOperator[] keyCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary) : base(keyCriteria, sortInfo, groupCount, summary, totalSummary) {
			this.Q = q;
		}
		public static bool StaticKeyEquals(object a, object b) {
			return Equals(a, b);
		}
		protected override object EvaluateOnInstance(object row, CriteriaOperator subj) {
			return CriteriaToQueryableExtender.EvaluateOnInstance(Converter, Q.ElementType, row, subj);
		}
		ICriteriaToExpressionConverter converter;
		protected virtual ICriteriaToExpressionConverter Converter {
			get {
				if(converter == null) {
					converter = new CriteriaToExpressionConverter();
				}
				return converter;
			}
		}
		internal static ServerModeGroupInfoData PrepareTopGroupInfoStatic(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator where, ServerModeSummaryDescriptor[] summaries) {
			var grouped = ReferenceEquals(where, null) ? q.MakeGroupBy(converter, new OperandValue(0)) : q.AppendWhere(converter, where).MakeGroupBy(converter, new OperandValue(0));
			var summary = grouped.DoSelectSummary(converter, q.ElementType, summaries);
			foreach(IList row in summary) {
				int childDataRowCount = (int)row[1];
				object[] totals = new object[summaries.Length];
				for(int i = 0; i < summaries.Length; ++i) {
					totals[i] = row[i + 2];
				}
				return new ServerModeGroupInfoData(null, childDataRowCount, totals);
			}
			return new ServerModeGroupInfoData(null, 0, new object[summaries.Length]);
		}
		protected override ServerModeGroupInfoData PrepareTopGroupInfo(ServerModeSummaryDescriptor[] summaries) {
			return PrepareTopGroupInfoStatic(Q, Converter, null, summaries);
		}
		internal static ServerModeGroupInfoData[] PrepareChildrenStatic(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries) {
			IQueryable qq = q.AppendWhere(converter, groupWhere);
			var gr = qq.MakeGroupBy(converter, groupByDescriptor.SortExpression);
			var ord = gr.MakeOrderBy(converter, new ServerModeOrderDescriptor(new OperandProperty("Key"), groupByDescriptor.IsDesc));
			var sum = ord.DoSelectSummary(converter, q.ElementType, summaries);
			List<ServerModeGroupInfoData> rv = new List<ServerModeGroupInfoData>();
			foreach(IList row in sum) {
				object value = row[0];
				int cnt = (int)row[1];
				object[] summary = new object[summaries.Length];
				for(int i = 0; i < summaries.Length; ++i) {
					summary[i] = row[i + 2];
				}
				rv.Add(new ServerModeGroupInfoData(value, cnt, summary));
			}
			return rv.ToArray();
		}
		protected override ServerModeGroupInfoData[] PrepareChildren(CriteriaOperator groupWhere, CriteriaOperator groupByCriterion, CriteriaOperator orderByCriterion, bool isDesc, ServerModeSummaryDescriptor[] summaries) {
			ServerModeOrderDescriptor groupByDescriptor = new ServerModeOrderDescriptor(groupByCriterion, isDesc);
			return PrepareChildrenStatic(Q, Converter, groupWhere, groupByDescriptor, summaries);
		}
		protected override object[] FetchRows(CriteriaOperator where, ServerModeOrderDescriptor[] order, int take) {
			var qq = Q.AppendWhere(Converter, where).MakeOrderBy(Converter, order);
			if(take > 0)
				qq = qq.Take(take);
			List<object> rv = new List<object>(take);
			foreach(object obj in qq) {
				rv.Add(obj);
			}
			return rv.ToArray();
		}
		internal static object[] FetchKeysStatic(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator[] keysCriteria, CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			var qq = q.AppendWhere(converter, where).MakeOrderBy(converter, order);
			if(skip > 0)
				qq = qq.Skip(skip);
			if(take > 0)
				qq = qq.Take(take);
			var res = qq.DoSelectSeveral(converter, keysCriteria);
			if(keysCriteria.Length == 1)
				return res.Select(oa => oa[0]).ToArray();
			else
				return res.Select(oa => new ServerModeCompoundKey(oa)).ToArray();
		}
		protected override object[] FetchKeys(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			return FetchKeysStatic(Q, Converter, KeysCriteria, where, order, skip, take);
		}
		protected override Type ResolveKeyType(CriteriaOperator singleKeyToResolve) {
			return Q.MakeSelect(Converter, singleKeyToResolve).ElementType;
		}
		protected override Type ResolveRowType() {
			return Q.ElementType;
		}
		internal static int GetCountStatic(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator criteriaOperator) {
			return q.AppendWhere(converter, criteriaOperator).Count();
		}
		protected override int GetCount(CriteriaOperator criteriaOperator) {
			return GetCountStatic(Q, Converter, criteriaOperator);
		}
	}
	public class LinqServerModeCore: ServerModeCore {
		protected readonly IQueryable Q;
		protected override ServerModeCore DXCloneCreate() {
			return new LinqServerModeCore(Q, this.KeysCriteria);
		}
		ICriteriaToExpressionConverter converter;
		protected virtual ICriteriaToExpressionConverter Converter {
			get {
				if(converter == null) {
					converter = new CriteriaToExpressionConverter();
				}
				return converter;
			}
		}
		protected override ServerModeCache CreateCacheCore() {
			IQueryable filtered;
			try {
				filtered = Q.AppendWhere(Converter, FilterClause);
			} catch(Exception e) {
				RaiseExceptionThrown(new ServerModeExceptionThrownEventArgs(e));
				filtered = Array.CreateInstance(Q.ElementType, 0).AsQueryable();
			}
			LinqServerModeCache rv = new LinqServerModeCache(filtered, KeysCriteria, SortInfo, GroupCount, SummaryInfo, TotalSummaryInfo);
			rv.IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria = FilterClause;
			return rv;
		}
		public LinqServerModeCore(IQueryable queryable, CriteriaOperator[] keys)
			: base(keys) {
			this.Q = queryable;
		}
		internal static object[] GetUniqueValuesStatic(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator expression, int maxCount, CriteriaOperator filter) {
			try {
				var select = q
					.AppendWhere(converter, filter)
					.MakeGroupBy(converter, expression)
					.MakeOrderBy(converter, new ServerModeOrderDescriptor(new OperandProperty("Key"), false))
					.MakeSelect(converter, new OperandProperty("Key"));
				if(maxCount > 0)
					select = select.Take(maxCount);
				List<object> rv = new List<object>();
				foreach(object o in select) {
					if(o == null)
						continue;
					rv.Add(o);
				}
				return rv.ToArray();
			} catch {
				return new object[0];
			}
		}
		protected override object[] GetUniqueValues(CriteriaOperator expression, int maxCount, CriteriaOperator filter) {
			return GetUniqueValuesStatic(Q, Converter, expression, maxCount, filter);
		}
		public static string GuessKeyExpression(Type objectType) {
			try {
				var props = objectType.GetProperties();
#if !SL //TODO SL
				var pk = new List<string>();
				foreach(var prop in props) {
					bool keyFound = false;
					foreach(System.Data.Objects.DataClasses.EdmScalarPropertyAttribute attr in prop.GetCustomAttributes(typeof(System.Data.Objects.DataClasses.EdmScalarPropertyAttribute), true)) {
						if(attr.EntityKeyProperty) {
							pk.Add(new OperandProperty(prop.Name).ToString());
							keyFound = true;
							break;
						}
					}
					if(keyFound) continue;
					foreach(System.Data.Linq.Mapping.ColumnAttribute attr in prop.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true)) {
						if(attr.IsPrimaryKey) {
							pk.Add(new OperandProperty(prop.Name).ToString());
							break;
						}
					}
				}
				if(pk.Count > 0) {
					return string.Join(", ", pk.ToArray());
				}
#endif
				foreach(var prop in props) {
					switch(prop.Name.ToLowerInvariant()) {
						case "oid":
						case "key":
						case "id":
							return prop.Name;
					}
				}
			} catch { }
			return string.Empty;
		}
		protected override object EvaluateOnInstance(object row, CriteriaOperator criteriaOperator) {
			return CriteriaToQueryableExtender.EvaluateOnInstance(Converter, Q.ElementType, row, criteriaOperator);
		}
		protected override bool EvaluateOnInstanceLogical(object row, CriteriaOperator criteriaOperator) {
			object evaluated = CriteriaToQueryableExtender.EvaluateOnInstance(Converter, Q.ElementType, row, criteriaOperator);
			bool rv =((bool?)evaluated) ?? false;
			return rv;
		}
		public override IList GetAllFilteredAndSortedRows() {
			IQueryable qq = Q.AppendWhere(Converter, FilterClause).MakeOrderBy(Converter, SortInfo).MakeSelectThis();
			List<object> rv = new List<object>();
			foreach(object o in qq) {
				rv.Add(o);
			}
			return rv;
		}
	}
	public interface ILinqServerModeFrontEndOwner {
		Type ElementType { get; }
		IQueryable QueryableSource { get; }
		bool IsReadyForTakeOff();
		string KeyExpression { get; }
		string DefaultSorting { get; }
	}
	public class LinqServerModeFrontEnd : IListServer, IListServerHints, IBindingList, ITypedList, IDXCloneable {
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
		protected virtual LinqServerModeFrontEnd DXClone() {
			LinqServerModeFrontEnd clone = CreateDXClone();
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
		protected virtual LinqServerModeFrontEnd CreateDXClone() {
			return new LinqServerModeFrontEnd(this.Owner);
		}
		protected IListServer Wrapper {
			get {
				if(_Wrapper == null) {
					_Wrapper = CreateWrapper();
					_Wrapper.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_Wrapper_InconsistencyDetected);
					_Wrapper.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_Wrapper_ExceptionThrown);
					_Wrapper.Apply(_Successful_FilterCriteria, _Successful_sortInfo, _Successful_groupCount, _Successful_summaryInfo, _Successful_totalSummaryInfo);
				}
				return _Wrapper;
			}
		}
		void _Wrapper_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			if(InconsistencyDetected == null)
				return;
			InconsistencyDetected(this, e);
		}
		void _Wrapper_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			if(ExceptionThrown == null)
				return;
			ExceptionThrown(this, e);
		}
		private IListServer CreateWrapper() {
			if(_IsReadyForTakeOff)
				return CreateRuntimeWrapper();
			else
				return new LinqServerModeDesignTimeWrapper();
		}
		protected virtual IListServer CreateRuntimeWrapper() {
			LinqServerModeCore core = new LinqServerModeCore(_DataSource, CriteriaOperator.ParseList(_KeyExpression));
			core.DefaultSorting = _DefaultSorting;
			return core;
		}
		protected void KillWrapper() {
			_Wrapper = null;
		}
		public LinqServerModeFrontEnd(ILinqServerModeFrontEndOwner owner) {
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
	public class LinqServerModeDesignTimeWrapper : IListServer {
		#region IListServer Members
		public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
		}
		public void Refresh() { }
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected { add { } remove { } }
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown { add { } remove { } }
		public object FindKeyByBeginWith(PropertyDescriptor column, string value) {
			return null;
		}
		public object FindKeyByValue(PropertyDescriptor column, object value) {
			return null;
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return -1;
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return -1;
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return new List<ListSourceGroupInfo>();
		}
		public int GetRowIndexByKey(object key) {
			return -1;
		}
		public object GetRowKey(int index) {
			return null;
		}
		public List<object> GetTotalSummary() {
			return new List<object>();
		}
		public object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return new object[0];
		}
		public virtual IList GetAllFilteredAndSortedRows() {
			return new object[0];
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return true;
		}
		#endregion
		#region IList Members
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public bool Contains(object value) {
			return false;
		}
		public int IndexOf(object value) {
			return -1;
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public bool IsFixedSize {
			get { return true; }
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public object this[int index] {
			get {
				throw new ArgumentOutOfRangeException("index");
			}
			set {
				throw new ArgumentOutOfRangeException("index");
			}
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
		}
		public int Count {
			get { return 0; }
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
			return new object[0].GetEnumerator();
		}
		#endregion
	}
}
