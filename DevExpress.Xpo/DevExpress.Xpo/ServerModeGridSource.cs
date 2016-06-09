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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;
using System.ComponentModel;
using System.Linq;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpo.Helpers {
	using DevExpress.Xpo;
	using DevExpress.Data.Filtering.Helpers;
	using System.Collections.Specialized;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Xpo.Metadata.Helpers;
	using DevExpress.Data.Helpers;
	public class XpoServerModeCache: ServerModeKeyedCache {
		public readonly Session Session;
		public readonly XPClassInfo ClassInfo;
		public readonly CriteriaOperator ExternalCriteria;
		public XpoServerModeCache(Session session, XPClassInfo classInfo, CriteriaOperator externalCriteria, CriteriaOperator[] keyCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary)
			: base(keyCriteria, sortInfo, groupCount, summary, totalSummary) {
			this.Session = session;
			this.ClassInfo = classInfo;
			this.ExternalCriteria = externalCriteria;
		}
		protected override object EvaluateOnInstance(object row, CriteriaOperator criteriaOperator) {
			return new ExpressionEvaluator(ClassInfo.GetEvaluatorContextDescriptor(), criteriaOperator, Session.CaseSensitive, Session.Dictionary.CustomFunctionOperators).Evaluate(row);
		}
		public static SortingCollection OrderDescriptorsToSortingCollection(ServerModeOrderDescriptor[] order) {
			List<SortProperty> result = new List<SortProperty>(order.Length);
			foreach(ServerModeOrderDescriptor ord in order) {
				SortProperty sp = new SortProperty(ord.SortExpression, ord.IsDesc ? SortingDirection.Descending : SortingDirection.Ascending);
				result.Add(sp);
			}
			return new SortingCollection(result.ToArray());
		}
		protected override object[] FetchKeys(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			props.AddRange(this.KeysCriteria);
			CriteriaOperator filter = ExternalCriteria & where;
			SortingCollection sorting = OrderDescriptorsToSortingCollection(order);
			IList<object[]> keys = Session.SelectData(ClassInfo, props, filter, false, skip, take, sorting);
			if(this.KeysCriteria.Length == 1)
				return keys.Select(ks => ks[0]).ToArray();
			else
				return keys.Select(ks => new ServerModeCompoundKey(ks)).ToArray();
		}
		protected override object[] FetchRows(CriteriaOperator where, ServerModeOrderDescriptor[] order, int take) {
			CriteriaOperator filter = ExternalCriteria & where;
			SortingCollection sorting = OrderDescriptorsToSortingCollection(order);
			ICollection objects = Session.GetObjects(ClassInfo, filter, sorting, take, false, false);
			return ListHelper.FromCollection(objects).ToArray();
		}
		protected override object[] FetchRowsByKeys(object[] keys) {
			if(this.KeysCriteria.Length == 1 && ClassInfo.OptimisticLockField == null && ReferenceEquals(ExternalCriteria, null))
				return Session.GetObjectsByKey(ClassInfo, keys, false).OfType<object>().ToArray();
			else
				return base.FetchRowsByKeys(keys);
		}
		protected override int GetCount(CriteriaOperator criteriaOperator) {
			return ((int?)Session.Evaluate(ClassInfo, new AggregateOperand(null, null, Aggregate.Count, null), ExternalCriteria & criteriaOperator)) ?? 0;
		}
		protected override Func<object, object> GetKeyComponentFromRowGetter(CriteriaOperator keyComponent) {
			var op = keyComponent as OperandProperty;
			if(!ReferenceEquals(op, null)) {
				var mi = ClassInfo.GetMember(op.PropertyName);
				if(mi.ReferenceType == null)
					return row => mi.GetValue(row);
				else
					return row => this.Session.GetKeyValue(mi.GetValue(row));
			}
			return base.GetKeyComponentFromRowGetter(keyComponent);
		}
		Type ExtractKeyPropertyType(XPMemberInfo mi) {
			if(mi.SubMembers.Count > 0)
				throw new InvalidOperationException("mi.SubMembers.Count > 0");
			else if(mi.ReferenceType != null)
				return ExtractKeyPropertyType(mi.ReferenceType.KeyProperty);
			else
				return mi.StorageType;
		}
		protected override Type ResolveKeyType(CriteriaOperator singleKeyCriterion) {
			var op = singleKeyCriterion as OperandProperty;
			if(ReferenceEquals(op, null))
				throw new ArgumentException("singleKeyCriterion");
			string propertyName = op.PropertyName;
			var prop = ClassInfo.GetMember(propertyName);
			return ExtractKeyPropertyType(prop);
		}
		protected override Type ResolveRowType() {
			return typeof(object);
		}
		public static XPClassInfo ObjectExpressionType(XPClassInfo ci, CriteriaOperator expression) {
			OperandProperty prop = expression as OperandProperty;
			if(ReferenceEquals(prop, null))
				return null;
			string path = prop.PropertyName;
			if(string.IsNullOrEmpty(path))
				return null;
			MemberInfoCollection mic = ci.ParsePersistentPath(path);
			return mic[mic.Count - 1].ReferenceType;
		}
		protected override ServerModeGroupInfoData[] PrepareChildren(CriteriaOperator groupWhere, CriteriaOperator groupByCriterion, CriteriaOperator orderByCriterion, bool isDesc, ServerModeSummaryDescriptor[] summaries) {
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			props.Add(groupByCriterion);
			props.Add(new AggregateOperand(null, null, Aggregate.Count, null));
			foreach(ServerModeSummaryDescriptor d in summaries) {
				props.Add(ConvertToAggregate(d));
			}
			CriteriaOperatorCollection groupBy = new CriteriaOperatorCollection();
			groupBy.Add(groupByCriterion);
			SortingCollection sorting = new SortingCollection(new SortProperty(orderByCriterion ?? groupByCriterion, isDesc ? SortingDirection.Descending : SortingDirection.Ascending));
			List<object[]> selected = Session.SelectData(ClassInfo, props, ExternalCriteria & groupWhere, groupBy, null, false, 0, 0, sorting);
			List<object> values = new List<object>(selected.Count);
			foreach(object[] row in selected) {
				values.Add(row[0]);
			}
			XPClassInfo groupByObjectType = ObjectExpressionType(ClassInfo, groupByCriterion);
			if(groupByObjectType != null) {
				values = ListHelper.FromCollection(Session.GetObjectsByKey(groupByObjectType, values, false));
			}
			List<ServerModeGroupInfoData> rv = new List<ServerModeGroupInfoData>(selected.Count);
			for(int i = 0; i < selected.Count; ++i) {
				List<object> rowStub = new List<object>(selected[i]);
				object val = values[i];
				int count = (int)rowStub[1];
				rowStub.RemoveRange(0, 2);
				rv.Add(new ServerModeGroupInfoData(val, count, rowStub.ToArray()));
			}
			return rv.ToArray();
		}
		protected override ServerModeGroupInfoData PrepareTopGroupInfo(ServerModeSummaryDescriptor[] summaries) {
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			props.Add(new AggregateOperand(null, null, Aggregate.Count, null));
			foreach(ServerModeSummaryDescriptor d in summaries) {
				props.Add(ConvertToAggregate(d));
			}
			IList<object[]> selected = Session.SelectData(ClassInfo, props, ExternalCriteria, false, 0, 0, null);
			if(selected.Count == 0) {
				return new ServerModeGroupInfoData(null, 0, new object[summaries.Length]);
			} else if(selected.Count == 1) {
				List<object> rv = new List<object>(selected[0]);
				int count = ((int?)rv[0]) ?? 0;
				rv.RemoveAt(0);
				return new ServerModeGroupInfoData(null, count, rv.ToArray());
			} else {
				throw new InvalidOperationException(Res.GetString(Res.ServerModeGridSource_WrongTopLevelAggregate));
			}
		}
		static CriteriaOperator ConvertToAggregate(ServerModeSummaryDescriptor d) {
			CriteriaOperator op;
			switch(d.SummaryType) {
				case Aggregate.Count:
					op = new AggregateOperand(null, null, Aggregate.Count, null);
					break;
				case Aggregate.Avg:
				case Aggregate.Max:
				case Aggregate.Min:
				case Aggregate.Sum:
					op = new AggregateOperand(null, d.SummaryExpression, d.SummaryType, null);
					break;
				default:
					throw new NotSupportedException(Res.GetString(Res.ServerModeGridSource_SummaryItemTypeNotSupported, d.SummaryType.ToString()));
			}
			return op;
		}
	}
	public interface IXpoServerModeGridDataSource: 
		IListServerHints, IDXCloneable,
		IFilteredXtraBindingList, ITypedList, 
		IListServer, IXPClassInfoAndSessionProvider, IColumnsServerActions {
		void SetFixedCriteria(CriteriaOperator fixedCriteria);
	}
	public class XpoServerModeCore: ServerModeCore, IXpoServerModeGridDataSource {
		static CriteriaOperator[] GetKeyCriteria(XPClassInfo ci) {
			var key = ci.KeyProperty;
			switch(key.SubMembers.Count){
				case 0:
					return new CriteriaOperator[] { new OperandProperty(key.Name) };
				case 1:
					return new CriteriaOperator[] { new OperandProperty(((XPMemberInfo)key.SubMembers[0]).Name) };
				default:
					return key.SubMembers.Cast<XPMemberInfo>().Where(mi => mi.IsPersistent).Select(mi => new OperandProperty(mi.Name)).ToArray<CriteriaOperator>();
			}
		}
		public XpoServerModeCore(Session initialSession, XPClassInfo initialClassInfo, CriteriaOperator initialFixedCriteria, string displayableProps, string defaultSorting)
			: base(GetKeyCriteria(initialClassInfo)) {
			this._Session = initialSession;
			this._ClassInfo = initialClassInfo;
			this._FixedCriteria = ExpandFilter(initialSession, initialClassInfo, initialFixedCriteria);
			this._DisplayableProperties = displayableProps;
			this.DefaultSorting = defaultSorting;
		}
		protected override ServerModeCore DXClone() {
			return base.DXClone();
		}
		protected override ServerModeCore DXCloneCreate() {
			return new XpoServerModeCore(this.Session, this.ClassInfo, this.FixedCriteria, this.DisplayableProperties, this.DefaultSorting);
		}
		protected override ServerModeCache CreateCacheCore() {
			XpoServerModeCache rv = new XpoServerModeCache(Session, ClassInfo, FixedCriteria & this.FilterClause, this.KeysCriteria, this.SortInfo, this.GroupCount, this.SummaryInfo, this.TotalSummaryInfo);
			rv.IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria = this.FixedCriteria & this.FilterClause;
			return rv;
		}
		CriteriaOperator _FixedCriteria;
		public virtual void SetFixedCriteria(CriteriaOperator op) {
			if(ReferenceEquals(_FixedCriteria, op))
				return;
			_FixedCriteria = ExpandFilter(Session, ClassInfo, op);
			Refresh();
		}
		public CriteriaOperator FixedCriteria {
			get { return _FixedCriteria; }
		}
		CriteriaOperator IFilteredDataSource.Filter {
			get { return FixedCriteria; }
			set { SetFixedCriteria(value); }
		}
		protected CriteriaOperator ExpandFilter(IPersistentValueExtractor session, XPClassInfo ci, CriteriaOperator op) {
			ExpandedCriteriaHolder h = PersistentCriterionExpander.Expand(session, ci, op);
			if(h.RequiresPostProcessing)
				throw new ArgumentException(Res.GetString(Res.PersistentAliasExpander_NonPersistentCriteria, CriteriaOperator.ToString(op), h.PostProcessingCause));
			return ExtractExpression(h.ExpandedCriteria);
		}
		Session _Session;
		public Session Session {
			get { return _Session; }
		}
		XPClassInfo _ClassInfo;
		public XPClassInfo ClassInfo {
			get { return _ClassInfo; }
		}
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { }
		object IBindingList.AddNew() {
			throw new NotSupportedException();
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
			throw new NotSupportedException();
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public event ListChangedEventHandler ListChanged;
		bool IBindingList.IsSorted {
			get {
				throw new NotSupportedException();
			}
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) { }
		void IBindingList.RemoveSort() {
			throw new NotSupportedException();
		}
		ListSortDirection IBindingList.SortDirection {
			get {
				throw new NotSupportedException();
			}
		}
		PropertyDescriptor IBindingList.SortProperty {
			get {
				throw new NotSupportedException();
			}
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
		public XPDictionary Dictionary {
			get {
				if(this.Session == null)
					return null;
				return Session.Dictionary;
			}
		}
		public IObjectLayer ObjectLayer {
			get {
				if(this.Session == null)
					return null;
				return Session.ObjectLayer;
			}
		}
		public IDataLayer DataLayer {
			get {
				if(this.Session == null)
					return null;
				return Session.DataLayer;
			}
		}
		public static bool IColumnsServerActionsAllowAction(XPClassInfo ci, string fieldName) {
			if(ci == null)
				return true;
			XPMemberInfo memberInfo = ci.FindMember(fieldName);
			if(memberInfo == null)
				return true;
			return memberInfo.IsPersistent || memberInfo.IsAliased;
		}
		bool IColumnsServerActions.AllowAction(string fieldName, ColumnServerActionType action) {
			return IColumnsServerActionsAllowAction(ClassInfo, fieldName);
		}
		static readonly OperandProperty ThisCriterion = new OperandProperty("This");
		public override CriteriaOperator ExtractExpression(CriteriaOperator input) {
			if(Equals(ThisCriterion, input))
				throw new ArgumentException(Res.GetString(Res.PersistentAliasExpander_NonPersistentCriteriaThisValueMember, CriteriaOperator.ToString(input), CriteriaOperator.ToString(ThisCriterion)));
			CriteriaOperator op = base.ExtractExpression(input);
			ExpandedCriteriaHolder expressionExpanded = PersistentCriterionExpander.Expand(ClassInfo, Session, op);
			if(expressionExpanded.RequiresPostProcessing)
				throw new ArgumentException(Res.GetString(Res.PersistentAliasExpander_NonPersistentCriteria, CriteriaOperator.ToString(op), expressionExpanded.PostProcessingCause));
			return expressionExpanded.ExpandedCriteria;
		}
		string _DisplayableProperties = null;
		public string DisplayableProperties {
			get {
				if(_DisplayableProperties == null)
					_DisplayableProperties = string.Empty;
				return _DisplayableProperties;
			}
		}
		class ItemProperties : DevExpress.Xpo.Helpers.ClassMetadataHelper.ItemProperties {
			public ItemProperties(XpoServerModeCore context) : base(context) { }
			public override string GetDisplayableProperties() {
				return ((XpoServerModeCore)Context).DisplayableProperties;
			}
		}
		ItemProperties itemProperties;
		public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(itemProperties == null)
				itemProperties = new ItemProperties(this);
			return ClassMetadataHelper.GetItemProperties(itemProperties, listAccessors);
		}
		public virtual string GetListName(PropertyDescriptor[] listAccessors) {
			return ClassMetadataHelper.GetListName(listAccessors);
		}
		protected override object EvaluateOnInstance(object row, CriteriaOperator criteriaOperator) {
			return new ExpressionEvaluator(ClassInfo.GetEvaluatorContextDescriptor(), criteriaOperator, Session.CaseSensitive, Session.Dictionary.CustomFunctionOperators).Evaluate(row);
		}
		protected override bool EvaluateOnInstanceLogical(object row, CriteriaOperator criteriaOperator) {
			return new ExpressionEvaluator(ClassInfo.GetEvaluatorContextDescriptor(), criteriaOperator, Session.CaseSensitive, Session.Dictionary.CustomFunctionOperators).Fit(row);
		}
		protected override object[] GetUniqueValues(CriteriaOperator expression, int maxCount, CriteriaOperator filter) {
			int top;
			if(maxCount > 0)
				top = maxCount;
			else
				top = 0;
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			props.Add(expression);
			SortingCollection sorting = new SortingCollection(new SortProperty(expression, SortingDirection.Ascending));
			IList<object[]> selected = Session.SelectData(ClassInfo, props, FixedCriteria & filter, props, null, false, 0, top, sorting);
			List<object> rv = new List<object>(selected.Count);
			foreach(object[] row in selected) {
				rv.Add(row[0]);
			}
			XPClassInfo objectExpressionType = XpoServerModeCache.ObjectExpressionType(ClassInfo, expression);
			if(objectExpressionType != null) {
				rv = ListHelper.FromCollection(Session.GetObjectsByKey(objectExpressionType, rv, false));
			}
			return rv.ToArray();
		}
		public override void Refresh() {
			base.Refresh();
			if(ListChanged != null)
				ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public override IList GetAllFilteredAndSortedRows() {
			return ListHelper.FromCollection(Session.GetObjects(ClassInfo, FixedCriteria & FilterClause, XpoServerModeCache.OrderDescriptorsToSortingCollection(SortInfo), 0, 0, false, false));
		}
	}
	public abstract class XpoServerCollectionWrapperBase : IXpoServerModeGridDataSource {
		public readonly IXpoServerModeGridDataSource Nested;
		protected XpoServerCollectionWrapperBase(IXpoServerModeGridDataSource nested) {
			this.Nested = nested;
		}
		public virtual XPClassInfo ClassInfo {
			get { return Nested.ClassInfo; }
		}
		public XPDictionary Dictionary {
			get { return Nested.Dictionary; }
		}
		public Session Session {
			get { return Nested.Session; }
		}
		public IObjectLayer ObjectLayer {
			get { return Nested.ObjectLayer; }
		}
		public IDataLayer DataLayer {
			get { return Nested.DataLayer; }
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected { add { Nested.InconsistencyDetected += value; } remove { Nested.InconsistencyDetected -= value; } }
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown { add { Nested.ExceptionThrown += value; } remove { Nested.ExceptionThrown -= value; } }
		public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return Nested.GetItemProperties(listAccessors);
		}
		public virtual string GetListName(PropertyDescriptor[] listAccessors) {
			return Nested.GetListName(listAccessors);
		}
		public virtual void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			Nested.Apply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo);
		}
		public virtual int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return Nested.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		public virtual int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Nested.LocateByValue(expression, value, startIndex, searchUp);
		}
		public virtual List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Nested.GetGroupInfo(parentGroup);
		}
		public virtual int GetRowIndexByKey(object key) {
			return Nested.GetRowIndexByKey(key);
		}
		public virtual object GetRowKey(int index) {
			return Nested.GetRowKey(index);
		}
		public virtual List<object> GetTotalSummary() {
			return Nested.GetTotalSummary();
		}
		public virtual object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return Nested.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
		}
		public virtual int Add(object value) {
			return Nested.Add(value);
		}
		public virtual void Clear() {
			Nested.Clear();
		}
		public virtual bool Contains(object value) {
			return Nested.Contains(value);
		}
		public virtual int IndexOf(object value) {
			return Nested.IndexOf(value);
		}
		public virtual void Insert(int index, object value) {
			Nested.Insert(index, value);
		}
		public virtual bool IsFixedSize {
			get { return Nested.IsFixedSize; }
		}
		public virtual bool IsReadOnly {
			get { return Nested.IsReadOnly; }
		}
		public virtual void Remove(object value) {
			Nested.Remove(value);
		}
		public virtual void RemoveAt(int index) {
			Nested.RemoveAt(index);
		}
		public virtual object this[int index] {
			get {
				return Nested[index];
			}
			set {
				Nested[index] = value;
			}
		}
		public virtual void CopyTo(Array array, int index) {
			Nested.CopyTo(array, index);
		}
		public virtual int Count {
			get { return Nested.Count; }
		}
		public virtual bool IsSynchronized {
			get { return Nested.IsSynchronized; }
		}
		public object SyncRoot {
			get { return Nested.SyncRoot; }
		}
		public virtual IEnumerator GetEnumerator() {
			return Nested.GetEnumerator();
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
			Nested.AddIndex(property);
		}
		public virtual object AddNew() {
			return Nested.AddNew();
		}
		public virtual bool AllowEdit {
			get { return Nested.AllowEdit; }
		}
		public virtual bool AllowNew {
			get { return Nested.AllowNew; }
		}
		public virtual bool AllowRemove {
			get { return Nested.AllowRemove; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			Nested.ApplySort(property, direction);
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			return Nested.Find(property, key);
		}
		bool IBindingList.IsSorted {
			get { return Nested.IsSorted; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			Nested.RemoveIndex(property);
		}
		void IBindingList.RemoveSort() {
			Nested.RemoveSort();
		}
		ListSortDirection IBindingList.SortDirection {
			get { return Nested.SortDirection; }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { return Nested.SortProperty; }
		}
		public virtual bool SupportsChangeNotification {
			get { return Nested.SupportsChangeNotification; }
		}
		bool IBindingList.SupportsSearching {
			get { return Nested.SupportsSearching; }
		}
		bool IBindingList.SupportsSorting {
			get { return Nested.SupportsSorting; }
		}
		CriteriaOperator IFilteredDataSource.Filter {
			get { return Nested.Filter; }
			set { Nested.Filter = value; }
		}
		public virtual event ListChangedEventHandler ListChanged {
			add { Nested.ListChanged += value; }
			remove { Nested.ListChanged -= value; }
		}
		public virtual void Refresh() {
			Nested.Refresh();
		}
		public virtual void SetFixedCriteria(CriteriaOperator fixedCriteria) {
			Nested.SetFixedCriteria(fixedCriteria);
		}
		public virtual bool AllowAction(string fieldName, ColumnServerActionType action) {
			return Nested.AllowAction(fieldName, action);
		}
		public virtual IList GetAllFilteredAndSortedRows() {
			return Nested.GetAllFilteredAndSortedRows();
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, System.Threading.CancellationToken cancellationToken) {
			return Nested.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		void IListServerHints.HintGridIsPaged(int pageSize) {
			IListServerHints n = Nested as IListServerHints;
			if(n == null)
				return;
			n.HintGridIsPaged(pageSize);
		}
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			IListServerHints n = Nested as IListServerHints;
			if(n == null)
				return;
			n.HintMaxVisibleRowsInGrid(rowsInGrid);
		}
		public abstract object DXClone();
	}
	public class XpoServerCollectionChangeTracker : XpoServerCollectionWrapperBase {
		public XpoServerCollectionChangeTracker(IXpoServerModeGridDataSource nested) : base(nested) {
		}
		public override object DXClone() {
			return new XpoServerCollectionChangeTracker((IXpoServerModeGridDataSource)Nested.DXClone());
		}
		void OnNestedSessionCommiting(object sender, SessionManipulationEventArgs e) {
			Session.BeforeCommitNestedUnitOfWork -= new SessionManipulationEventHandler(OnNestedSessionCommited);
			Session.BeforeCommitNestedUnitOfWork += new SessionManipulationEventHandler(OnNestedSessionCommited);
			Session.ObjectLoaded -= new ObjectManipulationEventHandler(OnObjectLoaded);
			Session.ObjectLoaded += new ObjectManipulationEventHandler(OnObjectLoaded);
		}
		void OnNestedSessionCommited(object sender, SessionManipulationEventArgs e) {
			Session.BeforeCommitNestedUnitOfWork -= new SessionManipulationEventHandler(OnNestedSessionCommited);
			Session.ObjectLoaded -= new ObjectManipulationEventHandler(OnObjectLoaded);
		}
		void OnNestedListChanged(object sender, ListChangedEventArgs e) {
			RaiseChanged(e);
		}
		void OnObjectChanged(object sender, ObjectChangeEventArgs e) {
			if(e.Reason != ObjectChangeReason.PropertyChanged)
				return;
			NotifyObjectChangedIfNeeded(sender);
		}
		void OnObjectLoaded(object sender, ObjectManipulationEventArgs e) {
			NotifyObjectChangedIfNeeded(e.Object);
		}
		void NotifyObjectChangedIfNeeded(object obj) {
			XPClassInfo ci = Dictionary.QueryClassInfo(obj);
			if(ci == null)
				return;
			if(!ci.IsAssignableTo(ClassInfo))
				return;
			int index = Nested.IndexOf(obj);
			if(index >= 0) {
				RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}
		event ListChangedEventHandler _ListChanged;
		public override event ListChangedEventHandler ListChanged {
			add {
				_ListChanged += value;
				EventsSubscribeUnSubscribe();
			}
			remove {
				_ListChanged -= value;
				EventsSubscribeUnSubscribe();
			}
		}
		void EventsSubscribeUnSubscribe() {
			Nested.ListChanged -= new ListChangedEventHandler(OnNestedListChanged);
			Session.ObjectChanged -= new ObjectChangeEventHandler(OnObjectChanged);
			Session.BeforeCommitNestedUnitOfWork -= new SessionManipulationEventHandler(OnNestedSessionCommiting);
			if(_ListChanged != null) {
				Nested.ListChanged += new ListChangedEventHandler(OnNestedListChanged);
				Session.ObjectChanged += new ObjectChangeEventHandler(OnObjectChanged);
				Session.BeforeCommitNestedUnitOfWork += new SessionManipulationEventHandler(OnNestedSessionCommiting);
			}
		}
		protected virtual void RaiseChanged(ListChangedEventArgs e) {
			if(_ListChanged == null)
				return;
			_ListChanged(this, e);
		}
		public override bool SupportsChangeNotification {
			get {
				return true;
			}
		}
	}
	public class XpoServerCollectionFlagger : XpoServerCollectionWrapperBase {
		readonly bool allowEdit;
		readonly bool allowAddNew;
		readonly bool allowRemove;
		public XpoServerCollectionFlagger(IXpoServerModeGridDataSource nested, bool allowEdit, bool allowAddNew, bool allowRemove) : base(nested) {
			this.allowEdit = allowEdit;
			this.allowAddNew = allowAddNew;
			this.allowRemove = allowRemove;
		}
		public override object DXClone() {
			return new XpoServerCollectionFlagger((IXpoServerModeGridDataSource)Nested.DXClone(), allowEdit, allowAddNew, allowRemove);
		}
		public override bool AllowEdit {
			get {
				return allowEdit;
			}
		}
		public override bool AllowNew {
			get {
				return base.AllowNew && allowAddNew;
			}
		}
		public override bool AllowRemove {
			get {
				return base.AllowRemove && allowRemove;
			}
		}
		public override bool IsReadOnly {
			get {
				return !(AllowEdit || AllowNew || AllowRemove);
			}
		}
	}
	public class XpoServerCollectionAdderRemover : XpoServerCollectionWrapperBase {
		readonly bool deleteOnRemove;
		List<object> addedItems = new List<object>();
		ObjectSet addedItemsDictionary = new ObjectSet();
		ObjectSet removedItemsDictionary = new ObjectSet();
		int currentGroupDepth = 0;
		bool CollectionInModifiedState { get { return addedItemsDictionary.Count != 0 || removedItemsDictionary.Count != 0; } }
		bool AddingRemovingAllowed { get { return currentGroupDepth == 0; } }
		public XpoServerCollectionAdderRemover(IXpoServerModeGridDataSource nested, bool deleteOnRemove)
			: base(nested) {
			this.deleteOnRemove = deleteOnRemove;
			Nested.ListChanged += new ListChangedEventHandler(OnNestedListChanged);
		}
		public override object DXClone() {
			if(CollectionInModifiedState)
				throw new InvalidOperationException("Can't clone modified collection");
			return new XpoServerCollectionAdderRemover((IXpoServerModeGridDataSource)Nested.DXClone(), deleteOnRemove);
		}
		public override int Add(object value) {
			int index = IndexOf(value);
			if(index >= 0)
				return index;
			if(!AddingRemovingAllowed)
				throw new InvalidOperationException(Res.GetString(Res.ServerModeGridSource_GroupAndAddOrRemoveIsNotAllwed));
			if(removedItemsDictionary.Contains(value)) {
				removedItemsDictionary.Remove(value);
			} else {
				addedItemsDictionary.Add(value);
				addedItems.Add(value);
			}
			index = IndexOf(value);
			RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			return index;
		}
		public override void Remove(object value) {
			RemoveAt(IndexOf(value));
		}
		public override void RemoveAt(int index) {
			System.Diagnostics.Debug.Assert(addedItemsDictionary.Count == addedItems.Count);
			if(index < 0 || index >= Count)
				return;
			if(!AddingRemovingAllowed)
				throw new InvalidOperationException(Res.GetString(Res.ServerModeGridSource_GroupAndAddOrRemoveIsNotAllwed));
			object obj = this[index];
			if(addedItemsDictionary.Contains(obj)) {
				addedItemsDictionary.Remove(obj);
				addedItems.Remove(obj);
			} else {
				removedItemsDictionary.Add(obj);
			}
			RaiseChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
			if(this.deleteOnRemove)
				Session.Delete(obj);
		}
		public override int Count {
			get {
				return base.Count + addedItemsDictionary.Count - removedItemsDictionary.Count;
			}
		}
		public override void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			if(groupCount > 0 && CollectionInModifiedState)
				throw new InvalidOperationException(Res.GetString(Res.ServerModeGridSource_GroupAndAddOrRemoveIsNotAllwed));
			currentGroupDepth = groupCount;
			base.Apply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo);
			ValidateLists();
		}
		public override bool Contains(object value) {
			if(addedItemsDictionary.Contains(value))
				return true;
			if(removedItemsDictionary.Contains(value))
				return false;
			return base.Contains(value);
		}
		public override void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public override IEnumerator GetEnumerator() {
			throw new NotSupportedException();
		}
		public override List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return base.GetGroupInfo(parentGroup);
		}
		public override int GetRowIndexByKey(object key) {
			if(!CollectionInModifiedState)
				return base.GetRowIndexByKey(key);
			if(key == null)
				return -1;
			XPClassInfo ci = Dictionary.QueryClassInfo(key);
			object obj;
			if(ci != null && ci.IsAssignableTo(ClassInfo)) {
				obj = key;
			} else {
				try {
					obj = Session.GetObjectByKey(ClassInfo, key);
				} catch {
					obj = null;
				}
			}
			return IndexOf(obj);
		}
		public override object GetRowKey(int index) {
			object obj = this[index];
			if(obj == null)
				return null;
			if(Session.IsNewObject(obj))
				return null;
			return Session.GetKeyValue(obj);
		}
		public override List<object> GetTotalSummary() {
			return base.GetTotalSummary();
		}
		public override int IndexOf(object value) {
			if(value == null)
				return -1;
			if(addedItemsDictionary.Contains(value)) {
				for(int i = 0; ; ++i) {
					if(ReferenceEquals(addedItems[i], value))
						return base.Count - removedItemsDictionary.Count + i;
				}
			} else if(removedItemsDictionary.Contains(value)) {
				return -1;
			} else {
				return IndexFromBase(base.IndexOf(value));
			}
		}
		protected int IndexFromBase(int baseIndex) {
			if(baseIndex < 0)
				return -1;
			int outerIndex = baseIndex;
			foreach(object obj in removedItemsDictionary) {
				int removedObjIndex = base.IndexOf(obj);
				if(removedObjIndex >= 0 && removedObjIndex < baseIndex)
					--outerIndex;
			}
			return outerIndex;
		}
		protected int IndexToBase(int outerIndex) {
			if(removedItemsDictionary.Count == 0) {
				return outerIndex;
			} else {
				int[] map = new int[removedItemsDictionary.Count];
				int mapIndex = 0;
				foreach(object obj in removedItemsDictionary) {
					int objIndex = base.IndexOf(obj);
					map[mapIndex++] = objIndex;
				}
				Array.Sort<int>(map);
				int baseIndex = outerIndex;
				foreach(int deletedObjIndex in map) {
					if(deletedObjIndex > baseIndex)
						break;
					baseIndex++;
				}
				return baseIndex;
			}
		}
		public override object this[int index] {
			get {
				System.Diagnostics.Debug.Assert(addedItemsDictionary.Count == addedItems.Count);
				int addedIndex = index - base.Count + removedItemsDictionary.Count;
				if(addedIndex >= 0) {
					return addedItems[addedIndex];
				} else {
					return base[IndexToBase(index)];
				}
			}
			set {
				throw new NotSupportedException();
			}
		}
		public override bool IsReadOnly {
			get {
				return true;
			}
		}
		public override bool IsFixedSize {
			get {
				return !AddingRemovingAllowed;
			}
		}
		public override void Insert(int index, object value) {
			Add(value);
		}
		public override bool AllowRemove {
			get {
				return AddingRemovingAllowed;
			}
		}
		public override bool AllowNew {
			get {
				return AddingRemovingAllowed;
			}
		}
		public override void Refresh() {
			this.addedItems.Clear();
			this.addedItemsDictionary.Clear();
			this.removedItemsDictionary.Clear();
			base.Refresh();
		}
		void ValidateLists() {
			for(int i = addedItems.Count - 1; i >= 0; --i) {
				object obj = addedItems[i];
				if(base.Contains(obj)) {
					addedItemsDictionary.Remove(obj);
					addedItems.RemoveAt(i);
				}
			}
			List<object> removedItemsList = new List<object>(removedItemsDictionary.Count);
			foreach (object obj in removedItemsDictionary) {
				removedItemsList.Add(obj);
			}
			foreach (object obj in removedItemsList) {
				if(!base.Contains(obj)) {
					removedItemsDictionary.Remove(obj);
				}
			}
		}
		void OnNestedListChanged(object sender, ListChangedEventArgs e) {
			ValidateLists();
			RaiseChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		event ListChangedEventHandler _ListChanged;
		public override event ListChangedEventHandler ListChanged {
			add {
				_ListChanged += value;
			}
			remove {
				_ListChanged -= value;
			}
		}
		protected virtual void RaiseChanged(ListChangedEventArgs e) {
			if(_ListChanged == null)
				return;
			_ListChanged(this, e);
		}
		public override bool SupportsChangeNotification {
			get {
				return true;
			}
		}
		protected virtual object CreateAddNewInstance() {
			return ClassInfo.CreateNewObject(Session);
		}
		public override object AddNew() {
			object obj = CreateAddNewInstance();
			this.Add(obj);
			SubscribeNewlyAdded(obj);
			return obj;
		}
		private void SubscribeNewlyAdded(object obj) {
			IXPObjectWithChangedEvent newlyAdded = obj as IXPObjectWithChangedEvent;
			if(newlyAdded != null)
				newlyAdded.Changed += new ObjectChangeEventHandler(NewAddedObjectChangedHandler);
		}
		private void UnsubscribeNewlyAdded(object obj) {
			IXPObjectWithChangedEvent newlyAdded = obj as IXPObjectWithChangedEvent;
			if(newlyAdded != null)
				newlyAdded.Changed -= new ObjectChangeEventHandler(NewAddedObjectChangedHandler);
		}
		void NewAddedObjectChangedHandler(object sender, ObjectChangeEventArgs e) {
			switch(e.Reason){
				case ObjectChangeReason.EndEdit:
					UnsubscribeNewlyAdded(sender);
					break;
				case ObjectChangeReason.CancelEdit:
					UnsubscribeNewlyAdded(sender);
					Remove(sender);
					break;
			}
		}
		public override IList GetAllFilteredAndSortedRows() {
			List<object> lst = new List<object>(ListHelper.FromCollection(base.GetAllFilteredAndSortedRows()));
			foreach(object o in addedItemsDictionary)
				lst.Remove(o);
			foreach(object o in removedItemsDictionary)
				lst.Remove(o);
			lst.AddRange(addedItems);
			return lst;
		}
	}
}
namespace DevExpress.Xpo {
	using DevExpress.Xpo.Helpers;
	using DevExpress.Xpo.DB.Helpers;
	using System.Drawing;
	using DevExpress.Data.Helpers;
	using System.ComponentModel;
#if SL
	using DevExpress.Xpf.ComponentModel;
#endif
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
#if !SL
	[Designer("DevExpress.Xpo.Design.SessionOwnerDesigner, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
	[Description("Serves as a data source for data-aware controls in server mode (working with large datasets).")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "XPServerCollectionSource")]
	public class XPServerCollectionSource: Component,
#if !SL
 ISupportInitializeNotification,
#endif
 IListSource, IXPClassInfoAndSessionProvider, IColumnsServerActions,
	IDXCloneable {
		Session _Session;
		string displayableProperties;
		XPClassInfo _ClassInfo;
		Type _Type;
		string _DefaultSorting;
		CriteriaOperator _FixedFilter;
		IList _List;
		IList List {
			get {
				if(_List == null) {
					_List = CreateList();
				}
				return _List;
			}
		}
#if DEBUG
		internal IXpoServerModeGridDataSource ServerList {
			get { return List as IXpoServerModeGridDataSource; }
		}
#endif
		bool _TrackChanges = false;
		bool _AllowEdit = false;
		bool _AllowRemove = false;
		bool _DeleteObjectOnRemove = false;
		bool _AllowNew = false;
		public XPServerCollectionSource() {
		}
		public XPServerCollectionSource(IContainer container)
			: this() {
			container.Add(this);
		}
		public XPServerCollectionSource(Session session, XPClassInfo objectClassInfo, CriteriaOperator fixedFilterCriteria)
			: this() {
			this._Session = session;
			this._ClassInfo = objectClassInfo;
			this._FixedFilter = fixedFilterCriteria;
		}
		public XPServerCollectionSource(Session session, XPClassInfo objectClassInfo) : this(session, objectClassInfo, null) { }
		public XPServerCollectionSource(Session session, Type objectType, CriteriaOperator fixedFilterCriteria) : this(session, session.GetClassInfo(objectType), fixedFilterCriteria) { }
		public XPServerCollectionSource(Session session, Type objectType) : this(session, session.GetClassInfo(objectType)) { }
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceTrackChanges"),
#endif
 DefaultValue(false)]
		public bool TrackChanges {
			get { return this._TrackChanges; }
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "TrackChanges"));
				}
				if(TrackChanges == value)
					return;
				this._TrackChanges = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceAllowEdit"),
#endif
 DefaultValue(false)]
		public bool AllowEdit {
			get { return this._AllowEdit; }
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "AllowEdit"));
				}
				if(AllowEdit == value)
					return;
				this._AllowEdit = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceAllowRemove"),
#endif
 DefaultValue(false)]
		public bool AllowRemove {
			get { return this._AllowRemove; }
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "AllowRemove"));
				}
				if(AllowRemove == value)
					return;
				this._AllowRemove = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceDeleteObjectOnRemove"),
#endif
 DefaultValue(false)]
		public bool DeleteObjectOnRemove {
			get { return this._DeleteObjectOnRemove; }
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "DeleteObjectOnRemove"));
				}
				if(DeleteObjectOnRemove == value)
					return;
				this._DeleteObjectOnRemove = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceAllowNew"),
#endif
 DefaultValue(false)]
		public bool AllowNew {
			get { return this._AllowNew; }
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "AllowNew"));
				}
				if(AllowNew == value)
					return;
				this._AllowNew = value;
			}
		}
		bool? _isDesignMode;
		protected bool IsDesignMode {
			get {
				return DevExpress.Data.Helpers.IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			}
		}
		private IList CreateList() {
#if !SL
			if(IsDesignMode) {
				XPCollection cll = new XPCollection();
				cll.Site = this.Site;
				return cll;
			} else
#endif
				if(IsInitialized()) {
					IXpoServerModeGridDataSource result = EquipServerModeCore(CreateServerModeCore());
					result.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(result_InconsistencyDetected);
					result.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(result_ExceptionThrown);
					return result;
				} else {
					return new object[0];
				}
		}
		protected virtual IXpoServerModeGridDataSource EquipServerModeCore(IXpoServerModeGridDataSource result) {
			if(AllowRemove || AllowNew)
				result = new XpoServerCollectionAdderRemover(result, DeleteObjectOnRemove);
			if(AllowEdit || AllowRemove || AllowNew)
				result = new XpoServerCollectionFlagger(result, AllowEdit, AllowNew, AllowRemove);
			if(TrackChanges)
				result = new XpoServerCollectionChangeTracker(result);
			return result;
		}
		protected virtual IXpoServerModeGridDataSource CreateServerModeCore() {
			return new XpoServerModeCore(Session, ObjectClassInfo, FixedFilterCriteria, DisplayableProperties, DefaultSorting);
		}
		void KillList() {
			_List = null;
		}
#if !SL
		bool ShouldSerializeSession() { return !(Session is DefaultSession); }
#if !SL
	[DevExpressXpoLocalizedDescription("XPServerCollectionSourceSession")]
#endif
		[TypeConverter("DevExpress.Xpo.Design.SessionReferenceConverter, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
		[RefreshProperties(RefreshProperties.All)]
		public Session Session {
			get {
#if !SL
				if(IsDesignMode)
					return ((XPBaseCollection)List).Session;
#endif
				if(_Session == null) {
					_Session = DoResolveSession();
				}
				return _Session;
			}
			set {
#if !SL
				if(IsDesignMode) {
					((XPBaseCollection)List).Session = value;
					return;
				}
#endif
				if(_Session == value)
					return;
				if(!this.IsInit)
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "Session"));
				_Session = value;
				if(_ClassInfo != null) {
					_Type = _ClassInfo.ClassType;
					_ClassInfo = null;
				}
				KillList();
			}
		}
		Session DoResolveSession() {
#if !CF && !SL
			if(IsDesignMode) {
				return new DevExpress.Xpo.Helpers.DefaultSession(this.Site);
			}
#endif
			ResolveSessionEventArgs args = new ResolveSessionEventArgs();
			OnResolveSession(args);
			if(args.Session != null) {
				Session resolved = args.Session.Session;
				if(resolved != null) {
					return resolved;
				}
			}
			return XpoDefault.GetSession();
		}
		protected virtual void OnResolveSession(ResolveSessionEventArgs args) {
			if(_ResolveSession != null)
				_ResolveSession(this, args);
		}
		event ResolveSessionEventHandler _ResolveSession;
		public event ResolveSessionEventHandler ResolveSession {
			add {
				_ResolveSession += value;
			}
			remove {
				_ResolveSession -= value;
			}
		}
		bool ShouldSerializeDisplayableProperties() {
			return DisplayableProperties != StringListHelper.DelimitedText(ClassMetadataHelper.GetDefaultDisplayableProperties(ObjectClassInfo), ";");
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPServerCollectionSourceDisplayableProperties")]
#endif
#if !SL
		[Editor("DevExpress.Xpo.Design.DisplayablePropertiesEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
		public string DisplayableProperties {
			get {
#if !SL
				if(IsDesignMode)
					return ((XPBaseCollection)List).DisplayableProperties;
#endif
				if(String.IsNullOrEmpty(displayableProperties))
					displayableProperties = StringListHelper.DelimitedText(ClassMetadataHelper.GetDefaultDisplayableProperties(ObjectClassInfo), ";");
				return displayableProperties;
			}
			set {
#if !SL
				if(IsDesignMode) {
					((XPBaseCollection)List).DisplayableProperties = value;
					return;
				}
#endif
				if(displayableProperties != value) {
					displayableProperties = value;
				}
			}
		}
		bool ShouldSerializeDefaultSorting() {
			return !string.IsNullOrEmpty(DefaultSorting);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPServerCollectionSourceDefaultSorting")]
#endif
		public string DefaultSorting {
			get {
				return _DefaultSorting;
			}
			set {
				if(_List is IXpoServerModeGridDataSource) {
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "DefaultSorting"));
				}
				this._DefaultSorting = value;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(null)]
		public Type ObjectType {
			get {
				return ObjectClassInfo != null ? ObjectClassInfo.ClassType : null;
			}
			set {
#if !SL
				if(IsDesignMode) {
					((XPCollection)List).ObjectType = value;
					return;
				}
#endif
				if(!this.IsInit)
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "ObjectType"));
				_ClassInfo = null;
				_Type = value;
				KillList();
			}
		}
#if !CF && !SL
		XPDictionary designDictionary;
		internal XPDictionary DesignDictionary {
			get {
				if(designDictionary == null) {
					if(!IsDesignMode)
						throw new InvalidOperationException();
					designDictionary = new DesignTimeReflection(Site);
				}
				return designDictionary;
			}
		}
#endif
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPServerCollectionSourceObjectClassInfo"),
#endif
 DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !SL
		[TypeConverter("DevExpress.Xpo.Design.ObjectClassInfoTypeConverter, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
		[MergableProperty(false)]
#endif
		public XPClassInfo ObjectClassInfo {
			get {
#if !SL
				if(IsDesignMode)
					return ((XPBaseCollection)List).GetObjectClassInfo();
#endif
				if(_ClassInfo == null && _Type != null && Session != null) {
					_ClassInfo =
#if !CF && !SL
 IsDesignMode ? DesignDictionary.GetClassInfo(_Type) :
#endif
 Session.GetClassInfo(_Type);
				}
				return _ClassInfo;
			}
			set {
				if(ObjectClassInfo == value)
					return;
#if !SL
				if(IsDesignMode) {
					XPCollection designHelper = (XPCollection)List;
					designHelper.ObjectType = null;
					designHelper.ObjectClassInfo = value;
					FixedFilterCriteria = null;
					return;
				}
#endif
				if(!this.IsInit)
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "ObjectClassInfo"));
				_ClassInfo = value;
				KillList();
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPServerCollectionSourceFixedFilterCriteria")]
#endif
#if !SL
		[Editor("DevExpress.Xpo.Design.XPCollectionCriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
		[TypeConverter("DevExpress.Xpo.Design.CriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FixedFilterCriteria {
			get { return _FixedFilter; }
			set {
				if(ReferenceEquals(FixedFilterCriteria, value))
					return;
				_FixedFilter = value;
#if !SL
				if(!IsDesignMode) {
#endif
					IXpoServerModeGridDataSource ds = List as IXpoServerModeGridDataSource;
					if(ds != null)
						ds.SetFixedCriteria(FixedFilterCriteria);
					else
						KillList();
#if !SL
				}
#endif
			}
		}
		[Browsable(false)]
		[DefaultValue("")]
		public string FixedFilterString {
			get {
				return CriteriaOperator.ToString(FixedFilterCriteria);
			}
			set {
				FixedFilterCriteria = CriteriaOperator.Parse(value);
			}
		}
		protected bool IsInit = false;
#if !SL
		event EventHandler Initialized;
		event EventHandler ISupportInitializeNotification.Initialized {
			add { Initialized += value; }
			remove { Initialized -= value; }
		}
		bool ISupportInitializeNotification.IsInitialized {
			get {
				return IsInitialized();
			}
		}
#endif
		protected bool IsInitialized() {
			if(IsInit)
				return false;
			return ObjectClassInfo != null;
		}
#if !SL
		void ISupportInitialize.BeginInit() {
			if(IsDesignMode)
				return;
			if(IsInit)
				throw new InvalidOperationException();
			KillList();
			IsInit = true;
		}
		void ISupportInitialize.EndInit() {
			if(IsDesignMode)
				return;
			if(!IsInit)
				throw new InvalidOperationException();
			IsInit = false;
			KillList();
			if(Initialized != null && IsInitialized())
				Initialized(this, EventArgs.Empty);
		}
#endif
		#region ISessionProvider Members
		Session ISessionProvider.Session {
			get { return Session; }
		}
		#endregion
		#region IDataLayerProvider Members
		IObjectLayer IObjectLayerProvider.ObjectLayer {
			get {
				if(Session == null)
					return null;
				return Session.ObjectLayer;
			}
		}
		IDataLayer IDataLayerProvider.DataLayer {
			get {
				if(Session == null)
					return null;
				return Session.DataLayer;
			}
		}
		#endregion
		#region IXPDictionaryProvider Members
		XPDictionary DevExpress.Xpo.Metadata.Helpers.IXPDictionaryProvider.Dictionary {
			get {
				if(Session == null)
					return null;
				return
#if !CF && !SL
 IsDesignMode ? DesignDictionary :
#endif
 Session.Dictionary;
			}
		}
		#endregion
		#region IListSource Members
#if !SL
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
#endif
		IList IListSource.GetList() {
			return List;
		}
		#endregion
		public void Reload() {
			IXpoServerModeGridDataSource src = List as IXpoServerModeGridDataSource;
			if(src != null)
				src.Refresh();
		}
		protected virtual void OnServerExceptionThrown(ServerExceptionThrownEventArgs e) {
			if(_ServerExceptionThrown != null)
				_ServerExceptionThrown(this, e);
		}
		event ServerExceptionThrownEventHandler _ServerExceptionThrown;
		public event ServerExceptionThrownEventHandler ServerExceptionThrown {
			add {
				_ServerExceptionThrown += value;
			}
			remove {
				_ServerExceptionThrown -= value;
			}
		}
		void result_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			FatalException(e.Exception);
		}
		protected virtual void FatalException(Exception e) {
			ServerExceptionThrownEventArgs args = new ServerExceptionThrownEventArgs(e);
			OnServerExceptionThrown(args);
			if(args.Action == ServerExceptionThrownAction.Rethrow)
				throw e;
		}
		void result_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			Inconsistent(e);
			if(e.Handled)
				return;
			e.Handled = true;
			InconsistentHelper.PostpronedInconsistent(() => Reload(), null);
		}
		protected virtual void Inconsistent(ServerModeInconsistencyDetectedEventArgs e) {
		}
		XPClassInfo IXPClassInfoProvider.ClassInfo {
			get { return ObjectClassInfo; }
		}
		bool IColumnsServerActions.AllowAction(string fieldName, ColumnServerActionType action) {
			return XpoServerModeCore.IColumnsServerActionsAllowAction(ObjectClassInfo, fieldName);
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual object DXClone() {
			XPServerCollectionSource clone = DXCloneCreate();
			clone._AllowEdit = this._AllowEdit;
			clone._AllowNew = this._AllowNew;
			clone._AllowRemove = this._AllowRemove;
			clone._ClassInfo = this._ClassInfo;
			clone._DefaultSorting = this._DefaultSorting;
			clone._DeleteObjectOnRemove = this._DeleteObjectOnRemove;
			clone._FixedFilter = this._FixedFilter;
			clone._ServerExceptionThrown = this._ServerExceptionThrown;
			clone._Session = this._Session;
			clone._TrackChanges = this._TrackChanges;
			clone._Type = this._Type;
			clone.displayableProperties = this.displayableProperties;
			clone._ResolveSession = this._ResolveSession;
			return clone;
		}
		protected virtual XPServerCollectionSource DXCloneCreate() {
			return new XPServerCollectionSource();
		}
	}
	public delegate void ServerExceptionThrownEventHandler(object sender, ServerExceptionThrownEventArgs e);
	public enum ServerExceptionThrownAction { Skip, Rethrow }
	public class ServerExceptionThrownEventArgs: EventArgs {
		Exception _Exception;
		ServerExceptionThrownAction _Action;
		public ServerExceptionThrownEventArgs(Exception exception, ServerExceptionThrownAction action) {
			this._Exception = exception;
			this._Action = action;
		}
		public ServerExceptionThrownEventArgs(Exception exception) : this(exception, ServerExceptionThrownAction.Skip) { }
		public ServerExceptionThrownAction Action {
			get { return _Action; }
			set { _Action = value; }
		}
		public Exception Exception {
			get { return _Exception; }
		}
	}
}
