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
using System.Linq.Expressions;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Data.Helpers;
#if !SL
using System.Windows.Forms;
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
#else
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
using System.Threading;
namespace DevExpress.Data.PLinq.Helpers {
	public class PLinqServerModeCore : IListServer, IDXCloneable {
		public string DefaultSorting;
		public static bool? DefaultForceCaseInsensitiveForAnySource;
		public bool ForceCaseInsensitiveForAnySource = DefaultForceCaseInsensitiveForAnySource ?? true;
		CriteriaOperator _filterCriteria;
		ServerModeOrderDescriptor[] _sortInfo = new ServerModeOrderDescriptor[0];
		int _groupCount;
		ServerModeSummaryDescriptor[] _groupSummaryInfo = new ServerModeSummaryDescriptor[0];
		ServerModeSummaryDescriptor[] _totalSummaryInfo = new ServerModeSummaryDescriptor[0];
		IEnumerable source;
		IList _filtered;
		IList _sorted;
		int? degreeOfParallelism;
		Type ListType;
		PLinqListSourceGroupInfo _RootGroup;
		public static ICriteriaToExpressionConverter PLinqCriteriaToExpressionConverter {
			get { return new CriteriaToExpressionConverterForObjects(); }
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual PLinqServerModeCore DXClone() {
			PLinqServerModeCore clone = CreateDXClone();
			clone._filterCriteria = this._filterCriteria;
			clone._sortInfo = this._sortInfo;
			clone._groupCount = this._groupCount;
			clone._groupSummaryInfo = this._groupSummaryInfo;
			clone._totalSummaryInfo = this._totalSummaryInfo;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone.DefaultSorting = this.DefaultSorting;
			clone.ForceCaseInsensitiveForAnySource = this.ForceCaseInsensitiveForAnySource;
			return clone;
		}
		protected virtual PLinqServerModeCore CreateDXClone() {
			return new PLinqServerModeCore(source, degreeOfParallelism);
		}
		protected IList Filtered {
			get {
				if(_filtered == null)
					_filtered = DoFilter();
				return _filtered;
			}
		}
		protected IList Sorted {
			get {
				if(_sorted == null)
					_sorted = DoSort();
				return _sorted;
			}
		}
		protected PLinqListSourceGroupInfo RootGroup {
			get {
				try {
					if(_RootGroup == null)
						_RootGroup = CreateRootGroup();
					return _RootGroup;
				} catch(Exception e) {
					if(ExceptionThrown != null)
						ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
					return null;
				}
			}
		}
		private PLinqListSourceGroupInfo CreateRootGroup() {
			try {
				return new PLinqListSourceGroupInfo(_sortInfo, _groupCount, Filtered, -1, _totalSummaryInfo, _groupSummaryInfo, 0, null, ListType, degreeOfParallelism);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return null;
			}
		}
		protected class PLinqListSourceGroupInfo : ListSourceGroupInfo {
			ServerModeOrderDescriptor[] sortInfo;
			int groupCount;
			IEnumerable rows;
			ServerModeSummaryDescriptor[] totalSummaryInfo;
			ServerModeSummaryDescriptor[] groupSummaryInfo;
			int topRowIndex;
			List<object> _Summary;
			List<ListSourceGroupInfo> _Children;
			Type sourceType;
			int? degreeOfParallelism;
			public PLinqListSourceGroupInfo(ServerModeOrderDescriptor[] sortInfo, int groupCount, IEnumerable rows, int level, ServerModeSummaryDescriptor[] totalSummaryInfo,
											ServerModeSummaryDescriptor[] groupSummaryInfo, int topRowIndex, object groupValue, Type sourseType, int? degreeOfParallelism) {
				this.sortInfo = sortInfo;
				this.groupCount = groupCount;
				this.rows = rows;
				this.Level = level;
				this.totalSummaryInfo = totalSummaryInfo;
				this.groupSummaryInfo = groupSummaryInfo;
				this.topRowIndex = topRowIndex;
				this.GroupValue = groupValue;
				this.sourceType = sourseType;
				this.degreeOfParallelism = degreeOfParallelism;
				this.ChildDataRowCount = CalcRowCount();
			}
			int CalcRowCount() {
				ParallelQuery pQuery = rows.AsParallel(sourceType, degreeOfParallelism);
				return pQuery.CallTotalSummary(sourceType);
			}
			public override List<object> Summary {
				get {
					if(_Summary == null)
						_Summary = CreateSummary(rows, Level < 0 ? totalSummaryInfo : groupSummaryInfo);
					return _Summary;
				}
			}
			private List<object> CreateSummary(IEnumerable rows, ServerModeSummaryDescriptor[] serverModeSummaryDescriptor) {
				List<object> totalSummary = new List<object>();
				ParallelQuery pQuery = rows.AsParallel(sourceType, degreeOfParallelism);
				foreach(ServerModeSummaryDescriptor TSummary in serverModeSummaryDescriptor) {
					if(TSummary.SummaryType == Aggregate.Count)
						totalSummary.Add(this.ChildDataRowCount);
					else						
						totalSummary.Add(pQuery.CallTotalSummary(sourceType, TSummary));				   
				}
				return totalSummary;
			}
			public List<ListSourceGroupInfo> GetChildren() {
				if(_Children == null)
					_Children = CreateChildren();
				return _Children;
			}
			private List<ListSourceGroupInfo> CreateChildren() {
				List<ListSourceGroupInfo> listGroupInfo = new List<ListSourceGroupInfo>();				
				if(Level < groupCount - 1) {
					ParallelQuery pQuery = rows.AsParallel(sourceType, degreeOfParallelism);
					int nextLevel = this.Level + 1;
					ServerModeOrderDescriptor currentDescriptor = sortInfo[nextLevel];
					ICriteriaToExpressionConverter converter = PLinqServerModeCore.PLinqCriteriaToExpressionConverter;
					Expression expr = converter.Convert(Expression.Parameter(sourceType), currentDescriptor.SortExpression);
					Type iGroupingType = typeof(IGrouping<,>).MakeGenericType(expr.Type, sourceType);
					var groupedQuery = pQuery.MakeGroupBy(currentDescriptor.SortExpression, sourceType)
											 .MakeOrderBy(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, iGroupingType, new ServerModeOrderDescriptor(new OperandProperty("Key"), currentDescriptor.IsDesc));
					Delegate del = GetDelegate(iGroupingType);
					var children = groupedQuery.MakeSelect(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, del, iGroupingType, typeof(GroupingResult));
					foreach(GroupingResult child in children) {
						listGroupInfo.Add(new PLinqListSourceGroupInfo(sortInfo, groupCount, child.Rows, nextLevel, totalSummaryInfo, groupSummaryInfo, topRowIndex, child.Key, sourceType, degreeOfParallelism));
						topRowIndex += child.Count;
					}
				}
				return listGroupInfo;
			}
			private Delegate GetDelegate(Type iGroupingType) {
				ParameterExpression paramTypeSource = Expression.Parameter(iGroupingType, "group");
				Expression exp = Expression.MemberInit(Expression.New(typeof(GroupingResult)),
													   new List<MemberBinding>() { Expression.Bind(typeof(GroupingResult).GetMember("Key")[0], Expression.Convert(Expression.Property(paramTypeSource, "Key"), typeof(object))),
																				   Expression.Bind(typeof(GroupingResult).GetMember("Rows")[0], paramTypeSource),
																				   Expression.Bind(typeof(GroupingResult).GetMember("Count")[0], Expression.Call(typeof(Enumerable), "Count", new Type[] {sourceType}, paramTypeSource))});
				var lambda = Expression.Lambda(exp, paramTypeSource);
				Delegate del = lambda.Compile();
				return del;
			}
		}
		protected List<object> TotalSummary {
			get {
				try {
					return RootGroup.Summary;
				} catch(Exception e) {
					if(ExceptionThrown != null)
						ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
					return new List<object>();
				}
			}
		}
		IList DoFilter() {
			try {
				if(ReferenceEquals(_filterCriteria, null)) {
					return source.ToList(ListType);
				} else {
					ParallelQuery pQuery = source.AsParallel(ListType, degreeOfParallelism);
					pQuery = pQuery.ApplyWhere(ListType, _filterCriteria);
					return pQuery.ToList(ListType);
				}
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return Array.CreateInstance(ListType, 0);
			}
		} 
		IList DoSort() {
			try{
				if(_sortInfo.Length == 0) {
					return Filtered;
				} else {
					ParallelQuery pQuery = Filtered.AsParallel(ListType, degreeOfParallelism);
					pQuery = pQuery.MakeOrderBy(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, ListType, _sortInfo);
					return pQuery.ToList(ListType);
				}
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return Array.CreateInstance(ListType, 0);
			}
		}
		public PLinqServerModeCore(IEnumerable source, int? degreeOfParallelism = null) {
			this.source = source;
			this.degreeOfParallelism = degreeOfParallelism;
			try {
				this.ListType = ExtractGenericEnumerableType(source);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
			}
		}
		public virtual CriteriaOperator ExtractExpression(CriteriaOperator d) {
			if(ForceCaseInsensitiveForAnySource)
				return StringsTolowerCloningHelper.Process(d);
			else
				return CriteriaOperator.Clone(d);
		}
		public static Type ExtractGenericEnumerableType(IEnumerable enumerable) {
			if(enumerable == null)
				throw new ArgumentNullException("enumerable");
			foreach(var iface in enumerable.GetType().GetInterfaces()) {
				if(iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					return iface.GetGenericArguments()[0];
			}
			throw new ArgumentException("'enumerable' does not implement IEnumerable<>");
		}
		protected virtual void RaiseInconsistencyDetected(){
			if(InconsistencyDetected != null)
				InconsistencyDetected(this, new ServerModeInconsistencyDetectedEventArgs());
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			try {
				CriteriaOperator convertedCriteria = ExtractExpression(filterCriteria);
				if(sortInfo == null)
					sortInfo = new ServerModeOrderDescriptor[0];
				if(groupSummaryInfo == null)
					groupSummaryInfo = new ServerModeSummaryDescriptor[0];
				if(totalSummaryInfo == null)
					totalSummaryInfo = new ServerModeSummaryDescriptor[0];
				ServerModeOrderDescriptor[] newSortInfo = Convert(sortInfo, groupCount);
				ServerModeSummaryDescriptor[] newGroupSummary = Convert(groupSummaryInfo);
				ServerModeSummaryDescriptor[] newTotalSummary = Convert(totalSummaryInfo);
				if(!Equals(_filterCriteria, convertedCriteria)) {
					_filterCriteria = convertedCriteria;
					_filtered = null;
				} else if(LinqServerModeCore.AreEquals(_sortInfo, newSortInfo) &&
					(_groupCount == groupCount) &&
					LinqServerModeCore.AreEquals(_groupSummaryInfo, newGroupSummary) &&
					LinqServerModeCore.AreEquals(_totalSummaryInfo, newTotalSummary)) {
					return;
				}
				_sortInfo = newSortInfo;
				_groupCount = groupCount;
				_groupSummaryInfo = newGroupSummary;
				_totalSummaryInfo = newTotalSummary;
				_RootGroup = null;
				_sorted = null;
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
			}
		}
		ServerModeOrderDescriptor[] Convert(ICollection<ServerModeOrderDescriptor> original, int groupCount) {
			List<ServerModeOrderDescriptor> rv = new List<ServerModeOrderDescriptor>();
			Dictionary<CriteriaOperator, ServerModeOrderDescriptor> uniquer = new Dictionary<CriteriaOperator, ServerModeOrderDescriptor>();
			foreach (ServerModeOrderDescriptor descr in original) {
				ServerModeOrderDescriptor converted = new ServerModeOrderDescriptor(ExtractExpression(descr.SortExpression), descr.IsDesc, ExtractExpression(descr.AuxExpression));
				rv.Add(converted);
				uniquer[converted.SortExpression] = converted;
			}
			if (rv.Count == groupCount) {
				ServerModeOrderDescriptor[] additionalSortings = GetSortingDescriptors(DefaultSorting);
				foreach (ServerModeOrderDescriptor additionalSorting in additionalSortings) {
					if (uniquer.ContainsKey(additionalSorting.SortExpression))
						continue;
					rv.Add(additionalSorting);
					uniquer.Add(additionalSorting.SortExpression, additionalSorting);
				}
			}			
			return rv.ToArray();
		}
		public static ServerModeOrderDescriptor[] GetSortingDescriptors(string sortingsString) {
			if (string.IsNullOrEmpty(sortingsString))
				return new ServerModeOrderDescriptor[0];
			CriteriaOperator[] additionalSortingsSrc;
			try {
				OperandValue[] dummy;
				additionalSortingsSrc = DevExpress.Data.Filtering.Helpers.CriteriaParser.ParseList(sortingsString, out dummy, true);
			} catch {
				return new ServerModeOrderDescriptor[0];
			}
			List<ServerModeOrderDescriptor> additionalSortings = new List<ServerModeOrderDescriptor>();
			foreach (CriteriaOperator op in additionalSortingsSrc) {
				additionalSortings.Add(ExtractSorting(op));
			}
			return additionalSortings.ToArray();
		}
		internal const string OrderDescToken = "DevExpress;MAGIK const;ORDER DESCENDING";
		static ServerModeOrderDescriptor ExtractSorting(CriteriaOperator op) {
			FunctionOperator fop = op as FunctionOperator;
			if (!ReferenceEquals(fop, null)) {
				if (fop.OperatorType == FunctionOperatorType.Custom) {
					OperandValue ov = fop.Operands[0] as OperandValue;
					if (!ReferenceEquals(ov, null)) {
						string name = ov.Value as string;
						if (name == OrderDescToken) {
							return new ServerModeOrderDescriptor(fop.Operands[1], true);
						}
					}
				}
			}
			return new ServerModeOrderDescriptor(op, false);
		}
		ServerModeSummaryDescriptor[] Convert(ICollection<ServerModeSummaryDescriptor> original) {
			if(original == null)
				original = new ServerModeSummaryDescriptor[0];
			List<ServerModeSummaryDescriptor> rv = new List<ServerModeSummaryDescriptor>(original.Count + 1);
			foreach(ServerModeSummaryDescriptor o in original) {
				rv.Add(new ServerModeSummaryDescriptor(ExtractExpression(o.SummaryExpression), o.SummaryType));
			}
			return rv.ToArray();
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			try {
				if(value == null)
					return -1;
				IEnumerable newSource = searchUp ? Sorted.MakeReverse(ListType) : Sorted;
				IQueryable query = newSource.AsQueryable();
				var convertedExpression = new FunctionOperator(FunctionOperatorType.ToStr, expression);
				var equalsExpression = ExtractExpression(new BinaryOperator(convertedExpression, new OperandValue(value), BinaryOperatorType.Equal));
				var startWithExpression = ExtractExpression(new FunctionOperator(FunctionOperatorType.Iif, expression.IsNull(), new OperandValue(false), new FunctionOperator(FunctionOperatorType.StartsWith, convertedExpression, new OperandValue(value))));
				var select = query.MakeSelect(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, GroupOperator.Or(equalsExpression, startWithExpression));
				bool found = false;
				int correctionStartIndex = ignoreStartRow ? 1 : 0;
				int index = -1;
				foreach(var result in select) {
					index++;
					if((index >= (startIndex + correctionStartIndex)) && result != null && (bool)result) {
							found = true;
							break;
					}
				}
				if(allowLoop & !found) {
					index = -1;
					foreach(var result in select) {
						index++;
						if((index < startIndex) && result != null && (bool)result) {
							found = true;
							break;
						}
					}
				}
				return found ? index : -1;
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return -1;
			}
		}
		IList IListServer.GetAllFilteredAndSortedRows() {
			return Sorted;
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Sorted.Count >= 0;
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			try {
				PLinqListSourceGroupInfo realParentGroup = (PLinqListSourceGroupInfo)parentGroup ?? RootGroup;
				return realParentGroup.GetChildren();
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return new List<ListSourceGroupInfo>();
			}
		}
		public int GetRowIndexByKey(object key) {
			try {
				return Sorted.IndexOf(key);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return -1;
			}
		}
		public object GetRowKey(int index) {
			try {
				if(index < 0 || index >= Count)
					return null;
				object value = this[index];
				return value;
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return null;
			}
		}
		public List<object> GetTotalSummary() {
			return TotalSummary;
		}
		public object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			CriteriaOperator filter = includeFilteredOut ? null : _filterCriteria;
			ParallelQuery pQuery = source.AsParallel(ListType, degreeOfParallelism);
			ICriteriaToExpressionConverter converter = PLinqServerModeCore.PLinqCriteriaToExpressionConverter;
			CriteriaOperator convertedExpression = ExtractExpression(expression);
			Expression expr = converter.Convert(Expression.Parameter(ListType), convertedExpression);
			Type iGroupingType = typeof(IGrouping<,>).MakeGenericType(expr.Type, ListType);
			try {			   
				ParallelQuery select = pQuery.ApplyWhere(ListType, filter)
											 .MakeGroupBy(convertedExpression, ListType)
											 .MakeOrderBy(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, iGroupingType, new ServerModeOrderDescriptor(new OperandProperty("Key"), false))
											 .MakeSelect(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, new OperandProperty("Key"), iGroupingType);
				if(maxCount > 0)
					select = select.Take(expr.Type, maxCount);
				return select.ToArray(expr.Type);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return new object[0];
			}
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			try {
				CriteriaOperator columnFilter;
				CriteriaOperator columnExpression = ExtractExpression(expression);
				int index = -1;
				bool find = false;
				if(value == null)
					columnFilter = columnExpression.IsNull();
				else
					columnFilter = columnExpression == new OperandValue(value);
				IQueryable pQuery = Sorted.AsQueryable();
				var select = pQuery.MakeSelect(PLinqServerModeCore.PLinqCriteriaToExpressionConverter, columnExpression);				
				foreach(var row in select) {
					index++;
					if(index >= startIndex)
						if(object.Equals(row, value)) {
							find = true;
							break;
						}
				}
				if(!find)
					return -1;
				if(searchUp)
					index = (Sorted.Count - 1) - index;
				return index;
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return -1;
			}
		}
		public void Refresh() {
			_filtered = null;
			_sorted = null;
			_RootGroup = null;
		}
		public int Add(object value) {
			throw new NotImplementedException();
		}
		public void Clear() {
			throw new NotImplementedException();
		}
		public bool Contains(object value) {
			try {
				ParallelQuery pQuery = Sorted.AsParallel(ListType, degreeOfParallelism);
				return PLinqCallerCore.Contains(ListType, pQuery, value);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return false;
			}
		}
		public int IndexOf(object value) {
			try {
				return Sorted.IndexOf(value);
			} catch(Exception e) {
				if(ExceptionThrown != null)
					ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
				return -1;
			}
		}
		public void Insert(int index, object value) {
			throw new NotImplementedException();
		}
		public bool IsFixedSize {
			get { return true; }
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public void Remove(object value) {
			throw new NotImplementedException();
		}
		public void RemoveAt(int index) {
			throw new NotImplementedException();
		}
		public object this[int index] {
			get {
				try {
					if((Sorted.Count == 0) | (Sorted.Count < (index + 1)))
						return null;
					else
						return Sorted[index];
				} catch(Exception e) {
					if(ExceptionThrown != null)
						ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e));
					return null;
				}
			}
			set {
				throw new NotSupportedException();
			}
		}
		public void CopyTo(Array array, int index) {
			throw new NotImplementedException();
		}
		public int Count {
			get {
				return Filtered.Count;
			}
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public IEnumerator GetEnumerator() {
			throw new NotSupportedException("Specified method is not supported in server mode. Make certain that the bound control supports this mode.");
		}
	}
}
 namespace DevExpress.Data.PLinq.Helpers{
	 abstract class PLinqCallerCore {
		 class ReaderWriterLockSlim {
			 object SyncRoot = new object();
			 public void EnterReadLock() {
				 System.Threading.Monitor.Enter(SyncRoot);
			 }
			 public void ExitReadLock() {
				 System.Threading.Monitor.Exit(SyncRoot);
			 }
			 public void EnterWriteLock() {
				 System.Threading.Monitor.Enter(SyncRoot);
			 }
			 public void ExitWriteLock() {
				 System.Threading.Monitor.Exit(SyncRoot);
			 }
		 }
		 static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
		 static Dictionary<Tuple<Type, Type>, PLinqCallerCore> cache = new Dictionary<Tuple<Type, Type>, PLinqCallerCore>();
		 static PLinqCallerCore GetCore(Type sourceType) {
			 return GetCore(sourceType, typeof(string));
		 }
		 static PLinqCallerCore GetCore(Type sourceType, Type type2) {
			 Tuple<Type, Type> types = new Tuple<Type, Type>(sourceType, type2);
			 cacheLock.EnterReadLock();
			 try {
				 PLinqCallerCore rv;
				 if(cache.TryGetValue(types, out rv))
					 return rv;
			 } finally {
				 cacheLock.ExitReadLock();
			 }
			 cacheLock.EnterWriteLock();
			 try {
				 PLinqCallerCore rv;
				 if(cache.TryGetValue(types, out rv))
					 return rv;
				 Type realType = typeof(PLinqCaller<,>).MakeGenericType(sourceType, type2);
				 rv = (PLinqCallerCore)Activator.CreateInstance(realType);
				 cache.Add(types, rv);
				 return rv;
			 } finally {
				 cacheLock.ExitWriteLock();
			 }
		 }
		 protected abstract ParallelQuery AsParallel(IEnumerable source, int? degreeOfParallelism);
		 protected abstract IList ToList(IEnumerable source);
		 protected abstract IList ToList(ParallelQuery source);
		 protected abstract int Count(ParallelQuery source);
		 protected abstract IEnumerable MakeReverse(IEnumerable source);
		 protected abstract object[] ToArray(ParallelQuery source);
		 protected abstract ParallelQuery Take(ParallelQuery source, int count);
		 protected abstract bool Contains(ParallelQuery source, object value);
		 protected abstract ParallelQuery Where(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery GroupBy(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery Select(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery OrderBy(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery OrderByDescending(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery ThenBy(ParallelQuery source, Delegate func);
		 protected abstract ParallelQuery ThenByDescending(ParallelQuery source, Delegate func);
		 public static ParallelQuery AsParallel(Type sourceType, int? degreeOfParallelism, IEnumerable source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.AsParallel(source, degreeOfParallelism);
		 }
		 public static ParallelQuery Take(Type sourceType, ParallelQuery source, int count) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.Take(source, count);
		 }
		 public static bool Contains(Type sourceType, ParallelQuery source, object value) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.Contains(source, value);
		 }
		 public static ParallelQuery Where(Type sourceType, ParallelQuery source, Delegate func) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.Where(source, func);
		 }
		 public static ParallelQuery GroupBy(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.GroupBy(source, func);
		 }
		 public static ParallelQuery Select(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.Select(source, func);
		 }
		 public static ParallelQuery OrderBy(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.OrderBy(source, func);
		 }
		 public static ParallelQuery OrderByDescending(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.OrderByDescending(source, func);
		 }
		 public static ParallelQuery ThenBy(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.ThenBy(source, func);
		 }
		 public static ParallelQuery ThenByDescending(Type sourceType, ParallelQuery source, Delegate func, Type resultType) {
			 PLinqCallerCore core = GetCore(sourceType, resultType);
			 return core.ThenByDescending(source, func);
		 }
		 public static IList ToList(Type sourceType, IEnumerable source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.ToList(source);
		 }
		 public static IList ToList(Type sourceType, ParallelQuery source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.ToList(source);
		 }
		 public static int Count(Type sourceType, ParallelQuery source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.Count(source);
		 }
		 public static IEnumerable MakeReverse(Type sourceType, IEnumerable source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.MakeReverse(source);
		 }
		 public static object[] ToArray(Type sourceType, ParallelQuery source) {
			 PLinqCallerCore core = GetCore(sourceType);
			 return core.ToArray(source);
		 }
		 class PLinqCaller<TSource, TResult> : PLinqCallerCore {
			 protected override ParallelQuery AsParallel(IEnumerable source, int? degreeOfParallelism) {
				 var result = ((IEnumerable<TSource>)source).AsParallel<TSource>();
#if !SL
				 if(degreeOfParallelism.HasValue)
					 result = result.WithDegreeOfParallelism<TSource>(degreeOfParallelism.Value);
#endif
				 return result;
			 }
			 protected override IList ToList(IEnumerable source) {
				 return ((IEnumerable<TSource>)source).ToList<TSource>();
			 }
			 protected override IList ToList(ParallelQuery source) {
				 return ((ParallelQuery<TSource>)source).ToList<TSource>();
			 }
			 protected override int Count(ParallelQuery source) {
				 return ((ParallelQuery<TSource>)source).Count<TSource>();
			 }
			 protected override ParallelQuery Take(ParallelQuery source, int count) {
				 return ((ParallelQuery<TSource>)source).Take<TSource>(count);
			 }
			 protected override bool Contains(ParallelQuery source, object value) {
				 return ((ParallelQuery<TSource>)source).Contains<TSource>((TSource)value);
			 }
			 protected override ParallelQuery Where(ParallelQuery source, Delegate func) {
				 return ((ParallelQuery<TSource>)source).Where<TSource>((Func<TSource,bool>)func);
			 }
			 protected override ParallelQuery GroupBy(ParallelQuery source, Delegate func) {
				 return ((ParallelQuery<TSource>)source).GroupBy((Func<TSource, TResult>)func);
			 }
			 protected override ParallelQuery Select(ParallelQuery source, Delegate func) {
				 return ((ParallelQuery<TSource>)source).Select((Func<TSource, TResult>)func);
			 }
			 protected override ParallelQuery OrderBy(ParallelQuery source, Delegate func) {
				 return ((ParallelQuery<TSource>)source).OrderBy((Func<TSource, TResult>)func);
			 }
			 protected override ParallelQuery OrderByDescending(ParallelQuery source, Delegate func) {
				 return ((ParallelQuery<TSource>)source).OrderByDescending((Func<TSource, TResult>)func);
			 }
			 protected override ParallelQuery ThenBy(ParallelQuery source, Delegate func) {
				 return ((OrderedParallelQuery<TSource>)source).ThenBy((Func<TSource, TResult>)func);
			 }
			 protected override ParallelQuery ThenByDescending(ParallelQuery source, Delegate func) {
				 return ((OrderedParallelQuery<TSource>)source).ThenByDescending((Func<TSource, TResult>)func);
			 }
			 protected override object[] ToArray(ParallelQuery source) {
				 return DevExpress.Utils.ArrayHelper.ConvertAll(((ParallelQuery<TSource>)source).ToArray<TSource>(), new Converter<TSource, object>(ToObject));
			 }
			 static object ToObject(TSource obj) {
				 return (object)obj;
			 }
			 protected override IEnumerable MakeReverse(IEnumerable source) {
				 return ((IEnumerable<TSource>)source).Reverse<TSource>();
			 }
		 }
	}
	static class PLinqHelpres {
		public static ParallelQuery AsParallel(this IEnumerable enumerable, Type sourceType, int? degreeOfParallelism) {
			return PLinqCallerCore.AsParallel(sourceType, degreeOfParallelism, enumerable);
		}
		public static IList ToList(this IEnumerable source, Type sourceType) {
			return PLinqCallerCore.ToList(sourceType, source);
		}
		public static IEnumerable MakeReverse(this IEnumerable source, Type sourceType) {
			return PLinqCallerCore.MakeReverse(sourceType, source);
		}
		public static IList ToList(this ParallelQuery source, Type sourceType) {
			return PLinqCallerCore.ToList(sourceType, source);
		}
		public static object[] ToArray(this ParallelQuery source, Type sourceType) {
			return PLinqCallerCore.ToArray(sourceType, source);
		}
		public static ParallelQuery ApplyWhere(this ParallelQuery source, Type sourceType, CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null))
				return source;
			var lambda = GetLambda(filterCriteria, sourceType);
			Delegate funcFiltering = lambda.Compile();
			return PLinqCallerCore.Where(sourceType, source, funcFiltering);
		}
		public static ParallelQuery Take(this ParallelQuery source, Type sourceType, int count) {
			return PLinqCallerCore.Take(sourceType, source, count);
		}
		static ParallelQuery Lambda(ParallelQuery source, Delegate predicate, ParameterExpression sourceParam, ParameterExpression predicateParam, MethodCallExpression methodCall) {
			var del = Expression.Lambda(methodCall, sourceParam, predicateParam).Compile();
			var typedDelegate = (Func<ParallelQuery, Delegate, ParallelQuery>)del;
			var rv = typedDelegate(source, predicate);
			return rv;
		}
		public static ParallelQuery MakeOrderBy(this ParallelQuery source, ICriteriaToExpressionConverter converter, Type sourceType, params ServerModeOrderDescriptor[] orders) {
			ParameterExpression thisParameter = Expression.Parameter(sourceType, "this");
			ParameterExpression predicateParam = Expression.Parameter(typeof(Delegate), "predicate");
			ParallelQuery result = source;
			for(int i = 0; i < orders.Length; i++) {
				ServerModeOrderDescriptor od = orders[i];
				Expression sortExpression = converter.Convert(thisParameter, od.SortExpression);
				var typedPredicate = Expression.Convert(predicateParam, typeof(Func<,>).MakeGenericType(sourceType, sortExpression.Type));
				Delegate funcSorting = Expression.Lambda(sortExpression, thisParameter).Compile();
				if(i == 0) {
					if(od.IsDesc)
						result = PLinqCallerCore.OrderByDescending(sourceType, result, funcSorting, sortExpression.Type);
					else
						result = PLinqCallerCore.OrderBy(sourceType, result, funcSorting, sortExpression.Type);
				} else {
					if(od.IsDesc)
						result = PLinqCallerCore.ThenByDescending(sourceType, result, funcSorting, sortExpression.Type);
					else
						result = PLinqCallerCore.ThenBy(sourceType, result, funcSorting, sortExpression.Type);
				}
			}
			return result;
		}
		public static object CallTotalSummary(this ParallelQuery source, Type sourceType, ServerModeSummaryDescriptor summary) {
			try {
				var lambda = GetLambda(summary.SummaryExpression, sourceType);
				Type returnType = lambda.ReturnType;
				Delegate funcTotalSummary = lambda.Compile();
				ParameterExpression sourceParam = Expression.Parameter(typeof(ParallelQuery), "src");
				ParameterExpression predicateParam = Expression.Parameter(typeof(Delegate), "predicate");
				var typedSource = Expression.Convert(sourceParam, typeof(ParallelQuery<>).MakeGenericType(sourceType));
				var typedPredicate = Expression.Convert(predicateParam, typeof(Func<,>).MakeGenericType(sourceType, returnType));
				string methodName = "";
				switch(summary.SummaryType) {
					case Aggregate.Count:
						methodName = "Count";
						break;
					case Aggregate.Avg:
						methodName = "Average";
						break;
					case Aggregate.Max:
						methodName = "Max";
						break;
					case Aggregate.Min:
						methodName = "Min";
						break;
					case Aggregate.Sum:
						methodName = "Sum";
						break;
					default:
						throw new NotSupportedException(summary.SummaryType.ToString());
				}
				var methodCall = Expression.Call(typeof(ParallelEnumerable), methodName, new Type[] { sourceType }, typedSource, typedPredicate);
				var convertedResult = Expression.Convert(methodCall, typeof(object));
				var del = Expression.Lambda(convertedResult, sourceParam, predicateParam).Compile();
				var typedDelegate = (Func<ParallelQuery, Delegate, object>)del;
				var rv = typedDelegate(source, funcTotalSummary);
				return rv;
			} catch {
				return null;
			}
		}
		public static int CallTotalSummary(this ParallelQuery source, Type sourceType) {
			return PLinqCallerCore.Count(sourceType, source);
		}
		public static ParallelQuery MakeGroupBy(this ParallelQuery source, CriteriaOperator groupCriteria, Type sourceType) {
			var lambda = GetLambda(groupCriteria, sourceType);
			Type resultType = lambda.ReturnType;
			Delegate funcFiltering = lambda.Compile();
			return PLinqCallerCore.GroupBy(sourceType, source, funcFiltering, resultType);
		}
		static LambdaExpression GetLambda(CriteriaOperator criteriaOperator, Type sourceType) {
			ParameterExpression paramTypeSource = Expression.Parameter(sourceType, "X");
			ICriteriaToExpressionConverter converter = PLinqServerModeCore.PLinqCriteriaToExpressionConverter;
			Expression groupExpression = converter.Convert(paramTypeSource, criteriaOperator);
			var lambda = Expression.Lambda(groupExpression, paramTypeSource);
			return lambda;
		}
		public static ParallelQuery MakeSelect(this ParallelQuery source, ICriteriaToExpressionConverter converter, CriteriaOperator selectCriteria, Type sourceType) {
			var lambda = GetLambda(selectCriteria, sourceType);
			Type resultType = lambda.ReturnType;
			Delegate funcFiltering = lambda.Compile();
			return PLinqCallerCore.Select(sourceType, source, funcFiltering, resultType);		   
		}
		public static ParallelQuery MakeSelect(this ParallelQuery source, ICriteriaToExpressionConverter converter, Delegate funcSelect, Type sourceType, Type resultType) {
			return PLinqCallerCore.Select(sourceType, source, funcSelect, resultType);
		}
	}
	public class GroupingResult {
		public object Key;
		public IEnumerable Rows;
		public int Count;
	}
	public class PLinqListServerDatacontrollerProxy: IListServer, ITypedList {
		protected readonly IListServer Nested;
		public PLinqListServerDatacontrollerProxy(IListServer nested) {
			Nested = nested;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ((ITypedList)Nested).GetItemProperties(listAccessors);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			Nested.Apply(filterCriteria, sortInfo, groupCount, groupSummaryInfo, totalSummaryInfo);
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown {
			add {
				Nested.ExceptionThrown += value;
			}
			remove {
				Nested.ExceptionThrown -= value;
			}
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return Nested.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		public IList GetAllFilteredAndSortedRows() {
			return Nested.GetAllFilteredAndSortedRows();
		}
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Nested.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Nested.GetGroupInfo(parentGroup);
		}
		public int GetRowIndexByKey(object key) {
			return Nested.GetRowIndexByKey(key);
		}
		public object GetRowKey(int index) {
			return Nested.GetRowKey(index);
		}
		public List<object> GetTotalSummary() {
			return Nested.GetTotalSummary();
		}
		public object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return Nested.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected {
			add {
				Nested.InconsistencyDetected += value;
			}
			remove {
				Nested.InconsistencyDetected -= value;
			}
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Nested.LocateByValue(expression, value, startIndex, searchUp);
		}
		public void Refresh() {
			Nested.Refresh();
		}
		public int Add(object value) {
			return Nested.Add(value);
		}
		public void Clear() {
			Nested.Clear();
		}
		public bool Contains(object value) {
			return Nested.Contains(value);
		}
		public int IndexOf(object value) {
			return Nested.IndexOf(value);
		}
		public void Insert(int index, object value) {
			Nested.Insert(index, value);
		}
		public bool IsFixedSize {
			get {
				return Nested.IsFixedSize;
			}
		}
		public bool IsReadOnly {
			get {
				return Nested.IsReadOnly;
			}
		}
		public void Remove(object value) {
			Nested.Remove(value);
		}
		public void RemoveAt(int index) {
			Nested.RemoveAt(index);
		}
		public object this[int index] {
			get {
				return Nested[index];
			}
			set {
				Nested[index] = value;
			}
		}
		public void CopyTo(Array array, int index) {
			Nested.CopyTo(array, index);
		}
		public int Count {
			get {
				return Nested.Count;
			}
		}
		public bool IsSynchronized {
			get { 
				return Nested.IsSynchronized;
			}
		}
		public object SyncRoot {
			get {
				return Nested.SyncRoot; 
			}
		}
		public IEnumerator GetEnumerator() {
			return Nested.GetEnumerator();
		}
	}
	public interface IPLinqServerModeFrontEndOwner {
		Type ElementType { get; }
		IEnumerable Source { get; }
		bool IsReadyForTakeOff();
		string DefaultSorting { get; }
		int? DegreeOfParallelism { get; }
	}
	public class PLinqServerModeFrontEnd : IListServer, IListServerHints, IBindingList, ITypedList, IDXCloneable {
		public readonly IPLinqServerModeFrontEndOwner Owner;
		IListServer _Wrapper;
		Type _Type;
		IEnumerable _DataSource;
		int? _DegreeOfParallelism;
		bool _IsReadyForTakeOff;
		string _DefaultSorting;
		#region reload store
		CriteriaOperator _Successful_FilterCriteria;
		ICollection<ServerModeOrderDescriptor> _Successful_sortInfo;
		int _Successful_groupCount;
		ICollection<ServerModeSummaryDescriptor> _Successful_summaryInfo;
		ICollection<ServerModeSummaryDescriptor> _Successful_totalSummaryInfo;
		#endregion
		protected IListServer Wrapper {
			get {
				if(_Wrapper == null) {
					_Wrapper = CreateWrapper();
					_Wrapper.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_Wrapper_InconsistencyDetected);
					_Wrapper.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_Wrapper_ExceptionThrown);
					_Wrapper.Apply(StringsTolowerCloningHelper.Process(_Successful_FilterCriteria), _Successful_sortInfo, _Successful_groupCount, _Successful_summaryInfo, _Successful_totalSummaryInfo);
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
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual PLinqServerModeFrontEnd DXClone() {
			PLinqServerModeFrontEnd clone = CreateDXClone();
			clone._Type = this._Type;
			clone._DataSource = this._DataSource;
			clone._IsReadyForTakeOff = this._IsReadyForTakeOff;
			clone._DefaultSorting = this._DefaultSorting;
			clone._DegreeOfParallelism = this._DegreeOfParallelism;
			clone._Successful_FilterCriteria = this._Successful_FilterCriteria;
			clone._Successful_sortInfo = this._Successful_sortInfo;
			clone._Successful_groupCount = this._Successful_groupCount;
			clone._Successful_summaryInfo = this._Successful_summaryInfo;
			clone._Successful_totalSummaryInfo = this._Successful_totalSummaryInfo;
			return clone;
		}
		protected virtual PLinqServerModeFrontEnd CreateDXClone() {
			return new PLinqServerModeFrontEnd(this.Owner);
		}
		private IListServer CreateWrapper() {
			if(_IsReadyForTakeOff)
				return CreateRuntimeWrapper();
			else
				return new PLinqServerModeDesignTimeWrapper();
		}
		protected virtual IListServer CreateRuntimeWrapper() {
			PLinqServerModeCore core = new PLinqServerModeCore(_DataSource, _DegreeOfParallelism);
			core.DefaultSorting = _DefaultSorting;
			return core;
		}
		protected void KillWrapper() {
			_Wrapper = null;
		}
		public PLinqServerModeFrontEnd(IPLinqServerModeFrontEndOwner owner) {
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
			Wrapper.Apply(StringsTolowerCloningHelper.Process(filterCriteria), sortInfo, groupCount, summaryInfo, totalSummaryInfo);
			_Successful_FilterCriteria = filterCriteria;
			_Successful_sortInfo = sortInfo;
			_Successful_groupCount = groupCount;
			_Successful_summaryInfo = summaryInfo;
			_Successful_totalSummaryInfo = totalSummaryInfo;
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return Wrapper.FindIncremental(StringsTolowerCloningHelper.Process(expression), value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Wrapper.LocateByValue(StringsTolowerCloningHelper.Process(expression), value, startIndex, searchUp);
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
			return Wrapper.GetUniqueColumnValues(StringsTolowerCloningHelper.Process(expression), maxCount, includeFilteredOut);
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
				Wrapper[index] = value;
				;
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
			if(_DataSource != Owner.Source) {
				_DataSource = Owner.Source;
				reset = true;
			}
			if(_DefaultSorting != Owner.DefaultSorting) {
				_DefaultSorting = Owner.DefaultSorting;
				reset = true;
			}
			if(_DegreeOfParallelism != Owner.DegreeOfParallelism) {
				_DegreeOfParallelism = Owner.DegreeOfParallelism;
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
	public class PLinqServerModeDesignTimeWrapper : IListServer {
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
