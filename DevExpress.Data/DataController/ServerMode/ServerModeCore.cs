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
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Linq;
using System.Threading;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#if SL
using ApplicationException = System.Exception;
using DevExpress.Xpf.Collections;
#else
#endif
namespace DevExpress.Data {
	public interface IListServer : IList {
		void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo);
		List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup);
		object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut);
		List<object> GetTotalSummary();
		object GetRowKey(int index);
		int GetRowIndexByKey(object key);
		int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop);
		int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp);
		IList GetAllFilteredAndSortedRows();	
		bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken);
		event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		void Refresh();
	}
	public class ServerModeOrderDescriptor {
		public readonly CriteriaOperator SortExpression;
		public readonly CriteriaOperator AuxExpression;
		public readonly bool IsDesc;
		public ServerModeOrderDescriptor(CriteriaOperator sortExpression, bool isDesc)
			: this(sortExpression, isDesc, null) {
		}
		public ServerModeOrderDescriptor(CriteriaOperator sortExpression, bool isDesc, CriteriaOperator auxExpression) {
			this.SortExpression = sortExpression;
			this.IsDesc = isDesc;
			this.AuxExpression = auxExpression;
		}
		public override string ToString() {
			return string.Format("{0} {1}", SortExpression, IsDesc ? "desc" : "asc");
		}
		public static string ToString(ServerModeOrderDescriptor[] descrs) {
			List<string> strs = new List<string>(descrs.Length);
			foreach(ServerModeOrderDescriptor d in descrs) {
				strs.Add(d.ToString());
			}
			return string.Join(", ", strs.ToArray());
		}
	}
	public class ServerModeSummaryDescriptor {
		public readonly CriteriaOperator SummaryExpression;
		public readonly Aggregate SummaryType;
		public ServerModeSummaryDescriptor(CriteriaOperator expression, Aggregate type) {
			this.SummaryExpression = expression;
			this.SummaryType = type;
		}
	}
	public class ServerModeInconsistencyDetectedEventArgs : HandledEventArgs {
		Exception _Exception;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Exception Message {
			get { return _Exception; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ServerModeInconsistencyDetectedEventArgs(Exception message) {
			this._Exception = message;
		}
		public ServerModeInconsistencyDetectedEventArgs() : this(null) { }
	}
	public class ServerModeExceptionThrownEventArgs : EventArgs {
		Exception _Exception;
		public Exception Exception {
			get { return _Exception; }
		}
		public ServerModeExceptionThrownEventArgs(Exception exception) {
			this._Exception = exception;
		}
	}
}
namespace DevExpress.Data.Helpers {
	public interface IListServerHints {
		void HintGridIsPaged(int pageSize);
		void HintMaxVisibleRowsInGrid(int rowsInGrid);
	}
	public class ServerModeEtcValue {
		object value;
		public object Value {
			get { return value; }
		}
		bool isDesc;
		public bool IsDesc {
			get { return isDesc; }
		}
		public ServerModeEtcValue(object value, bool isDesc) {
			this.value = value;
			this.isDesc = isDesc;
		}
		public override string ToString() {
			return "etc.";
		}
	}
	public static class ServerModeCommonParameters {
		static int? complexGroupingOperationTimeout;
		public static int ComplexGroupingOperationTimeout {
			get { return complexGroupingOperationTimeout ?? 300000; }
			set { complexGroupingOperationTimeout = value; }
		}
		public static TimeSpan ComplexGroupingOperationTimeoutTimeSpan {
			get { return new TimeSpan((long)ComplexGroupingOperationTimeout * 10000L); }
		}
		static int? complexGroupingOperationEtcTimeout;
		public static int ComplexGroupingOperationEtcTimeout {
			get { return complexGroupingOperationEtcTimeout ?? 20000; }
			set { complexGroupingOperationEtcTimeout = value; }
		}
		public static TimeSpan ComplexGroupingOperationEtcTimeoutTimeSpan {
			get { return new TimeSpan((long)ComplexGroupingOperationEtcTimeout * 10000L); }
		}
		static bool? useEtcTimeoutForGroupingOperation;
		public static bool UseEtcTimeoutForGroupingOperation {
			get { return useEtcTimeoutForGroupingOperation ?? true; }
			set { useEtcTimeoutForGroupingOperation = value; }
		}
		public static BinaryOperator FixServerModeEtcValue(BinaryOperator theOperator) {
			BinaryOperatorType operatorType = theOperator.OperatorType;
			CriteriaOperator leftOperand = theOperator.LeftOperand;
			CriteriaOperator rightOperand = theOperator.RightOperand;
			if(leftOperand is OperandValue && ((OperandValue)leftOperand).Value is ServerModeEtcValue) {
				ServerModeEtcValue leftValue = ((ServerModeEtcValue)(((OperandValue)leftOperand).Value));
				if(!(rightOperand is OperandValue && ((OperandValue)rightOperand).Value is ServerModeEtcValue)) {
					if(operatorType == BinaryOperatorType.Equal) {
						operatorType = leftValue.IsDesc ? BinaryOperatorType.GreaterOrEqual : BinaryOperatorType.LessOrEqual;
					}
				} else {
					throw new InvalidOperationException();
				}
				leftOperand = new OperandValue(leftValue.Value);
				return new BinaryOperator(leftOperand, rightOperand, operatorType);
			} else if(rightOperand is OperandValue && ((OperandValue)rightOperand).Value is ServerModeEtcValue) {
				ServerModeEtcValue rightValue = ((ServerModeEtcValue)(((OperandValue)rightOperand).Value));
				if(!(leftOperand is OperandValue && ((OperandValue)leftOperand).Value is ServerModeEtcValue)) {
					if(operatorType == BinaryOperatorType.Equal) {
						operatorType = rightValue.IsDesc ? BinaryOperatorType.LessOrEqual : BinaryOperatorType.GreaterOrEqual;
					}
				} else {
					throw new InvalidOperationException();
				}
				rightOperand = new OperandValue(rightValue.Value);
				return new BinaryOperator(leftOperand, rightOperand, operatorType);
			}
			return theOperator;
		}
	}
	public class InconsistencyDetectedException : ApplicationException {
		public InconsistencyDetectedException() : base() { }
		public InconsistencyDetectedException(string message) : base(message) { }
	}
	public class ServerModeGroupInfoData {
		public readonly object GroupValue;
		public readonly int ChildDataRowCount;
		public readonly IList Summary;
		public ServerModeGroupInfoData(object groupValue, int childDataRowCount, IList summary) {
			this.GroupValue = groupValue;
			this.ChildDataRowCount = childDataRowCount;
			this.Summary = summary;
		}
	}
	public abstract class ServerModeCache {
		public readonly CriteriaOperator[] KeysCriteria;
		public readonly ServerModeOrderDescriptor[] SortDescription;
		public readonly int GroupByCount;
		public readonly ServerModeSummaryDescriptor[] SummaryInfo, TotalSummaryInfo;
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		public readonly Dictionary<object, object> SomethingCache = new Dictionary<object, object>();
		protected ServerModeCache(CriteriaOperator[] keyCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary) {
			this.KeysCriteria = keyCriteria;
			this.SortDescription = sortInfo;
			this.GroupByCount = groupCount;
			this.SummaryInfo = summary;
			this.TotalSummaryInfo = totalSummary;
		}
		bool _landed = false;
		protected virtual bool IsLanded { get { return _landed; } }
		bool _DeathProof;
		public void CanResetCache() {
			if(_DeathProof)
				throw new InvalidOperationException("Can't drop server mode cache while code within that cache is still being executed. Please use BeginInvoke/Post methods to perform this operation.");
			WithReentryProtection(() => default(object));
		}
		void PerformDeathProofAction(Action action) {
			bool saved = _DeathProof;
			_DeathProof = true;
			try {
				action();
			} finally {
				_DeathProof = saved;
			}
		}
		protected virtual void Fatal(Exception e) {
			if(IsLanded)
				return;
			_landed = true;
			if(ExceptionThrown != null)
				PerformDeathProofAction(() => ExceptionThrown(this, new ServerModeExceptionThrownEventArgs(e)));
		}
		protected void RaiseInconsistencyDetected(string format, params object[] args) {
			string message = string.Format(format, args);
			RaiseInconsistencyDetected(message);
		}
		protected void RaiseInconsistencyDetected(string message) {
			if(IsLanded)
				return;
			_landed = true;
			Exception msg = new InconsistencyDetectedException(message);
			try {
				throw msg;
			} catch {
			}
			if(InconsistencyDetected != null)
				PerformDeathProofAction(() => InconsistencyDetected(this, new ServerModeInconsistencyDetectedEventArgs(msg)));
			GC.KeepAlive(message);
		}
		protected void RaiseInconsistencyDetected(IEnumerable<string> messages) {
			RaiseInconsistencyDetected(string.Join("; ", messages));
		}
		protected internal class ServerModeGroupInfo : ListSourceGroupInfo {
			List<object> _Summary = new List<object>();
			public List<ListSourceGroupInfo> ChildrenGroups;
			public override List<object> Summary {
				get {
					return _Summary;
				}
			}
			public readonly ServerModeGroupInfo Parent;
			internal readonly object OwnershipMarker;
			public int TopRecordIndex;
			public ServerModeGroupInfo(ServerModeGroupInfo parent, object ownershipMarker) {
				this.Parent = parent;
				this.OwnershipMarker = ownershipMarker;
			}
		}
		ServerModeGroupInfo _topGroupInfo;
		protected ServerModeGroupInfo TotalGroupInfo {
			get {
				if(_topGroupInfo == null)
					_topGroupInfo = CreateTopGroupInfo();
				return _topGroupInfo;
			}
		}
		public abstract object Indexer(int index);
		public abstract int GetRowIndexByKey(object key);
		public abstract int IndexOf(object value);
		public abstract bool Contains(object value);
		public int Count() {
			return TotalGroupInfo.ChildDataRowCount;
		}
		public abstract object GetRowKey(int index);
		public List<object> GetTotalSummary() {
			return TotalGroupInfo.Summary;
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			if(parentGroup == null)
				parentGroup = TotalGroupInfo;
			ServerModeGroupInfo myGroupInfo = (ServerModeGroupInfo)parentGroup;
			FillChildrenIfNeeded(myGroupInfo);
			return myGroupInfo.ChildrenGroups;
		}
		void FillChildrenIfNeeded(ServerModeGroupInfo myGroupInfo) {
			if(myGroupInfo.ChildrenGroups != null)
				return;
			if(!ReferenceEquals(myGroupInfo.OwnershipMarker, this))
				throw new InvalidOperationException("Group info from the different cache!");
			myGroupInfo.ChildrenGroups = new List<ListSourceGroupInfo>();
			if(IsLanded || myGroupInfo.Level + 1 >= GroupByCount || myGroupInfo.ChildDataRowCount == 0) {
				return;
			}
			CriteriaOperator groupWhere = GetGroupWhere(myGroupInfo);
			ServerModeOrderDescriptor ordDs = SortDescription[myGroupInfo.Level + 1];
			bool isDesc = ordDs.IsDesc;
			CriteriaOperator groupCriterion;
			CriteriaOperator orderCriterion;
			ServerModeSummaryDescriptor[] fullSummaryInfo;
			int? additionalAggregatePosition;
			if(ReferenceEquals(ordDs.AuxExpression, null) || Equals(ordDs.AuxExpression, ordDs.SortExpression)) {
				groupCriterion = ordDs.SortExpression;
				orderCriterion = null;
				fullSummaryInfo = SummaryInfo;
				additionalAggregatePosition = null;
			} else {
				additionalAggregatePosition = SummaryInfo.Length;
				fullSummaryInfo = SummaryInfo.Concat(new[] { new ServerModeSummaryDescriptor(ordDs.SortExpression, Aggregate.Max) }).ToArray();
				groupCriterion = ordDs.AuxExpression;
				orderCriterion = new AggregateOperand(null, ordDs.SortExpression, Aggregate.Max, null);
			}
			ServerModeGroupInfoData[] children;
			try {
				children = WithReentryProtection(
					() => PrepareChildren(groupWhere, groupCriterion, orderCriterion, isDesc, fullSummaryInfo)
					);
			} catch(Exception e) {
				Fatal(e);
				return;
			}
			int currentTop = myGroupInfo.TopRecordIndex;
			foreach(ServerModeGroupInfoData grpSrc in children) {
				ServerModeGroupInfo nextGroup = new ServerModeGroupInfo(myGroupInfo, this);
				nextGroup.Level = myGroupInfo.Level + 1;
				if(additionalAggregatePosition.HasValue) {
					nextGroup.AuxValue = grpSrc.GroupValue;
					nextGroup.GroupValue = grpSrc.Summary[additionalAggregatePosition.Value];
				} else {
					nextGroup.GroupValue = grpSrc.GroupValue;
					nextGroup.AuxValue = null;
				}
				nextGroup.ChildDataRowCount = grpSrc.ChildDataRowCount;
				nextGroup.TopRecordIndex = currentTop;
				currentTop += nextGroup.ChildDataRowCount;
				for(int i = 0; i < SummaryInfo.Length; ++i) {
					nextGroup.Summary.Add(grpSrc.Summary[i]);
				}
				myGroupInfo.ChildrenGroups.Add(nextGroup);
			}
			if(myGroupInfo.ChildDataRowCount < 0) {
			} else if(currentTop - myGroupInfo.TopRecordIndex != myGroupInfo.ChildDataRowCount) {
				RaiseInconsistencyDetected("The total row count of nested groups ({1}) does not equal to the row count in a parent group ({0}).", myGroupInfo.ChildDataRowCount, currentTop - myGroupInfo.TopRecordIndex);
			}
		}
		protected abstract ServerModeGroupInfoData[] PrepareChildren(CriteriaOperator groupWhere, CriteriaOperator groupByCriterion, CriteriaOperator orderByCriterion, bool isDesc, ServerModeSummaryDescriptor[] summaries);
		protected abstract ServerModeGroupInfoData PrepareTopGroupInfo(ServerModeSummaryDescriptor[] summaries);
		protected virtual int PrepareTopGroupCount() {
			return PrepareTopGroupInfo(new ServerModeSummaryDescriptor[0]).ChildDataRowCount;
		}
		ServerModeGroupInfoData PrepareTopGroupInfoWithTrick(ServerModeSummaryDescriptor[] summaries) {
			if(IsNothingButCount(summaries)) {
				int topGroupCount = PrepareTopGroupCount();
				List<object> resSummary = new List<object>(summaries.Length);
				for(int i = 0; i < summaries.Length; ++i) {
					resSummary.Add(topGroupCount);
				}
				return new ServerModeGroupInfoData(null, topGroupCount, resSummary.ToArray());
			} else {
				return PrepareTopGroupInfo(summaries);
			}
		}
#if DEBUG
		bool creatingTopGroupInfo;
#endif
		ServerModeGroupInfo CreateTopGroupInfo() {
#if DEBUG
			if(creatingTopGroupInfo)
				throw new InvalidOperationException("Internal error -- should be called once per cache");
			creatingTopGroupInfo = true;
#endif
			ServerModeGroupInfo rv = new ServerModeGroupInfo(null, this);
			rv.GroupValue = null;
			rv.Level = -1;
			rv.ChildrenGroups = null;
			rv.TopRecordIndex = 0;
			rv.ChildDataRowCount = 0;
			if(IsLanded)
				return rv;
			ServerModeGroupInfoData topGroupInfoData;
			if(CanTrickCreateTopGroupFromNextGroups()) {
				rv.ChildDataRowCount = -1;  
				List<ListSourceGroupInfo> children = GetGroupInfo(rv);
				int totalCount = 0;
				foreach(ListSourceGroupInfo gr in children) {
					totalCount += gr.ChildDataRowCount;
				}
				List<object> summaries = new List<object>(TotalSummaryInfo.Length);
				for(int i = 0; i < TotalSummaryInfo.Length; ++i)
					summaries.Add(totalCount);
				topGroupInfoData = new ServerModeGroupInfoData(null, totalCount, summaries.ToArray());
			} else {
				try {
					topGroupInfoData = WithReentryProtection(
						() => PrepareTopGroupInfoWithTrick(TotalSummaryInfo)
						);
				} catch(Exception e) {
					Fatal(e);
					return rv;
				}
			}
			System.Diagnostics.Debug.Assert(topGroupInfoData.GroupValue == null);
			rv.ChildDataRowCount = topGroupInfoData.ChildDataRowCount;
			for(int i = 0; i < TotalSummaryInfo.Length; ++i) {
				rv.Summary.Add(topGroupInfoData.Summary[i]);
			}
			return rv;
		}
		bool CanTrickCreateTopGroupFromNextGroups() {
			return this.GroupByCount > 0 && IsNothingButCount(TotalSummaryInfo);
		}
		public static bool IsNothingButCount(ServerModeSummaryDescriptor[] summaries) {
			foreach(ServerModeSummaryDescriptor sd in summaries) {
				if(sd.SummaryType == Aggregate.Count)
					continue;
				return false;
			}
			return true;
		}
		protected CriteriaOperator GetGroupWhere(ServerModeGroupInfo myGroupInfo) {
			CriteriaOperator rv = null;
			for(ServerModeGroupInfo currentGroup = myGroupInfo; currentGroup.Level >= 0; currentGroup = currentGroup.Parent) {
				var descriptor = SortDescription[currentGroup.Level];
				CriteriaOperator expression;
				object value;
				if(ReferenceEquals(descriptor.AuxExpression, null) || Equals(descriptor.AuxExpression, descriptor.SortExpression)) {
					expression = descriptor.SortExpression;
					value = currentGroup.GroupValue;
				} else {
					expression = descriptor.AuxExpression;
					value = currentGroup.AuxValue;
				}
				CriteriaOperator criterion;
				if(value == null)
					criterion = expression.IsNull();
				else
					criterion = expression == new OperandValue(value);
				rv = rv & criterion;
			}
			return rv;
		}
		protected T WithReentryProtection<T>(Func<T> action) {
			var watch = new ReentrancyAndThreadsWatch(this);
			try {
				return action();
			} catch {
				watch.ExceptionAlreadyThrown();
				throw;
			} finally {
				watch.Exit();
			}
		}
		ReentrancyAndThreadsWatch _ReenterancyUndThreadsWatch;
		class ReentrancyAndThreadsWatch {
			readonly ServerModeCache Watched;
			bool dontThrow;
			public void ExceptionAlreadyThrown() {
				dontThrow = true;
			}
			void Throw() {
				if(dontThrow)
					return;
				dontThrow = true;
				throw new InvalidOperationException("Reentry or race condition detected.");
			}
			public ReentrancyAndThreadsWatch(ServerModeCache watched) {
				this.Watched = watched;
				var prev = Interlocked.Exchange(ref this.Watched._ReenterancyUndThreadsWatch, this);
				if(prev != null)
					Throw();
			}
			public void Exit() {
				var prev = Interlocked.Exchange(ref this.Watched._ReenterancyUndThreadsWatch, null);
				if(prev != this)
					Throw();
			}
		}
		internal void FillFromOldCacheWhateverMakesSence(ServerModeCache oldCache) {
			if(oldCache == null)
				return;
			if(oldCache.IsLanded)
				return;
			ServerModeGroupInfo oldTopGroupInfo = oldCache._topGroupInfo;
			if(oldTopGroupInfo == null)
				return;
			if(oldTopGroupInfo.ChildDataRowCount == 0)  
				return;
			if(!ServerModeCore.AreEquals(this.TotalSummaryInfo, oldCache.TotalSummaryInfo))
				return;
			ServerModeGroupInfo newTopGroupInfo = new ServerModeGroupInfo(null, this);
			newTopGroupInfo.GroupValue = oldTopGroupInfo.GroupValue;
			newTopGroupInfo.AuxValue = oldTopGroupInfo.AuxValue;
			newTopGroupInfo.Level = oldTopGroupInfo.Level;
			newTopGroupInfo.ChildrenGroups = null;
			newTopGroupInfo.TopRecordIndex = oldTopGroupInfo.TopRecordIndex;
			newTopGroupInfo.ChildDataRowCount = oldTopGroupInfo.ChildDataRowCount;
			newTopGroupInfo.Summary.AddRange(oldTopGroupInfo.Summary);
			int maxCloneDepth = Math.Min(this.GroupByCount, oldCache.GroupByCount);
			for(int i = 0; i < maxCloneDepth; ++i) {
				if(!Equals(this.SortDescription[i].SortExpression, oldCache.SortDescription[i].SortExpression)) {
					maxCloneDepth = i;
					break;
				}
			}
			if(maxCloneDepth > 0 && ServerModeCore.AreEquals(this.SummaryInfo, oldCache.SummaryInfo)) {
				CloneFromOldCache(newTopGroupInfo, oldTopGroupInfo, this.SortDescription, oldCache.SortDescription, 0, maxCloneDepth);
			}
			if(this._topGroupInfo != null)
				throw new InvalidOperationException("internal error, _topGroupInfo should be null at that point");
			if(newTopGroupInfo.ChildrenGroups == null && CanTrickCreateTopGroupFromNextGroups())
				return;
			this._topGroupInfo = newTopGroupInfo;
		}
		static void CloneFromOldCache(ServerModeGroupInfo newGroupInfo, ServerModeGroupInfo oldGroupInfo, ServerModeOrderDescriptor[] newDescriptors, ServerModeOrderDescriptor[] oldDescriptors, int currentDepth, int maxCloneDepth) {
			List<ListSourceGroupInfo> protoGroups = oldGroupInfo.ChildrenGroups;
			if(protoGroups == null)
				return;
			if(currentDepth >= maxCloneDepth)
				return;
			if(newDescriptors[currentDepth].IsDesc != oldDescriptors[currentDepth].IsDesc) {
				protoGroups = new List<ListSourceGroupInfo>(protoGroups);
				protoGroups.Reverse();
			}
			List<ListSourceGroupInfo> newGroups = new List<ListSourceGroupInfo>(protoGroups.Count);
			int currentTop = newGroupInfo.TopRecordIndex;
			foreach(ServerModeGroupInfo currentPrototype in protoGroups) {
				ServerModeGroupInfo clonedGroupInfo = new ServerModeGroupInfo(newGroupInfo, newGroupInfo.OwnershipMarker);
				clonedGroupInfo.ChildDataRowCount = currentPrototype.ChildDataRowCount;
				clonedGroupInfo.ChildrenGroups = null;
				clonedGroupInfo.GroupValue = currentPrototype.GroupValue;
				clonedGroupInfo.AuxValue = currentPrototype.AuxValue;
				clonedGroupInfo.Level = currentPrototype.Level;
				clonedGroupInfo.Summary.AddRange(currentPrototype.Summary);
				clonedGroupInfo.TopRecordIndex = currentTop;
				currentTop += clonedGroupInfo.ChildDataRowCount;
				newGroups.Add(clonedGroupInfo);
				CloneFromOldCache(clonedGroupInfo, currentPrototype, newDescriptors, oldDescriptors, currentDepth + 1, maxCloneDepth);
			}
			if(newGroupInfo.ChildrenGroups != null)
				throw new InvalidOperationException("internal error, newGroupInfo.ChildrenGroups should be null at that point");
			newGroupInfo.ChildrenGroups = newGroups;
		}
		public abstract bool PrefetchRows(IEnumerable<ListSourceGroupInfo> groupsToPrefetch, CancellationToken cancellationToken);
		public abstract object FindFirstKeyByCriteriaOperator(CriteriaOperator filterCriteria, bool isReversed);
	}
	public abstract class ServerModeKeyedCache: ServerModeCache, IListServerHints {
#if !SL
		public static readonly BooleanSwitch ExplainSkipTake = new BooleanSwitch("ServerModeKeyedCacheExplainSkipTake", "Explain Skip/Take keys decision by ServerModeKeyedCache");
#endif
		IByIntDictionary _RowsByIndex;
		IByIntDictionary _KeysByIndex;
		protected IByIntDictionary RowsByIndex {
			get {
				if(_RowsByIndex == null)
					_RowsByIndex = ByIntDictionary.CreateForType(ResolveRowType());
				return _RowsByIndex;
			}
		}
		protected IByIntDictionary KeysByIndex {
			get {
				if(_KeysByIndex == null) {
					Type keyType = KeysCriteria.Length == 1 ? ResolveKeyType(KeysCriteria[0]) : typeof(ServerModeCompoundKey);
					_KeysByIndex = ByIntDictionary.CreateForType(keyType);
				}
				return _KeysByIndex;
			}
		}
		protected ServerModeKeyedCache(CriteriaOperator[] keyCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary)
			: base(keyCriteria, sortInfo, groupCount, summary, totalSummary) {
		}
		protected abstract Type ResolveKeyType(CriteriaOperator singleKeyToResolve);
		protected abstract Type ResolveRowType();
		public override bool Contains(object value) {
			return IndexOf(value) >= 0;
		}
		public override int IndexOf(object value) {
			if(value == null || value is DBNull)
				return -1;
			int rv;
			if(RowsByIndex.TryGetKeyByValue(value, out rv, 0, int.MaxValue))
				return rv;
			object key;
			try {
				key = GetKeyFromRow(value);
			} catch {
				key = null;
			}
			return GetRowIndexByKey(key);
		}
		public bool KeyEquals(object a, object b) { return KeysComparer.Equals(a, b); }
		public virtual IEqualityComparer<object> KeysComparer { get { return EqualityComparer<object>.Default; } }
		protected virtual Func<object, object> GetKeyComponentFromRowGetter(CriteriaOperator keyComponent) {
			return row => EvaluateOnInstance(row, keyComponent);
		}
		Func<object, object> keyFromRowGetter;
		protected object GetKeyFromRow(object row) {
			if(keyFromRowGetter == null) {
				var subGetters = KeysCriteria.Select(keyComponent => GetKeyComponentFromRowGetter(keyComponent)).ToArray();
				if(subGetters.Length == 1)
					keyFromRowGetter = subGetters[0];
				else
					keyFromRowGetter = o => new ServerModeCompoundKey(subGetters.Select(g => g(o)).ToArray());
			}
			return keyFromRowGetter(row);
		}
		protected virtual int MaxInSize { get { return int.MaxValue; } }
		public override object Indexer(int index) {
			object row;
			if(RowsByIndex.TryGetValue(index, out row))
				return row;
			if(IsLanded)
				return null;
			object key = GetRowKey(index);
			if(key == null)
				return null;
			if(RowsByIndex.TryGetValue(index, out row)) 
				return row;
			return FetchInIndexerCore(index);
		}
		object FetchInIndexerCore(int index) {
			if(IsLanded)
				return null;
			IList<string> inconsistencies = new List<string>();
			IList<object> keysToFetch;
			IDictionary<object, int> indicesByKeys;
			if(_hintPageSize > 0 && this.GroupByCount == 0)
				FillKeysToFetchListWeb(index, inconsistencies, out keysToFetch, out indicesByKeys);
			else
				FillKeysToFetchList(index, inconsistencies, out keysToFetch, out indicesByKeys);
			if(inconsistencies.Count > 0 && _hintPageSize <= 0) {
				RaiseInconsistencyDetected(inconsistencies);
				return null;
			}
			object[] rows;
			try {
				rows = WithReentryProtection(
					() => FetchRowsByKeys(keysToFetch.ToArray())
					);
			} catch(Exception e) {
				Fatal(e);
				return null;
			}
			if(IsLanded)
				return null;
			if(rows.Length != keysToFetch.Count) {
				inconsistencies.Add(string.Format("The number of returned rows ({1}) does not equal the number of row keys in the query ({0})", keysToFetch.Count, rows.Length));
			}
			object rv = null;
			foreach(object fetchedRow in rows) {
				object fetchedKey = GetKeyFromRow(fetchedRow);
				int rowIndex;
				if(!indicesByKeys.TryGetValue(fetchedKey, out rowIndex)) {
					inconsistencies.Add(string.Format("Row with key '{0}' fetched which was not queried. May be internal error or unsupported key design", fetchedKey));
				} else {
					if(RowsByIndex.ContainsKey(rowIndex)) {
						inconsistencies.Add(string.Format("Row at index {0} with key '{1}' fetched twice", rowIndex, fetchedKey));
					} else {
						RowsByIndex.Add(rowIndex, fetchedRow);
						if(rowIndex == index) {
							if(rv != null)
								throw new InvalidOperationException("internal error (double rv)");
							rv = fetchedRow;
						}
					}
				}
			}
			if(rv == null && inconsistencies.Count == 0)
				throw new InvalidOperationException("internal error (rv not found)");
			if(inconsistencies.Count > 0 && !IsLanded) {
				RaiseInconsistencyDetected(inconsistencies);
			}
			return rv;
		}
		void FillKeysToFetchList(int index, IList<string> inconsistencies, out IList<object> keysToFetch, out IDictionary<object, int> indicesByKeys) {
			int inSize = Math.Min(MagicNumberFetchRowsInSize, MaxInSize);
			keysToFetch = new List<object>(inSize);
			indicesByKeys = new Dictionary<object, int>(keysToFetch.Count, KeysComparer);
			int splitLen = inSize == 1 ? 1 : inSize * 2 / 3;
			bool fillFromSplit = true;
			for(int i = index; i < index + splitLen; ++i) {
				if(RowsByIndex.ContainsKey(i)) {
					fillFromSplit = false;
					break;
				}
				object fillKey;
				if(!KeysByIndex.TryGetValue(i, out fillKey)) {
					fillFromSplit = false;
					break;
				}
				int oldIndex;
				if(indicesByKeys.TryGetValue(fillKey, out oldIndex)) {
					inconsistencies.Add(string.Format("Key '{0}' found twice at indices {1} and {2}", fillKey, oldIndex, i));
				} else {
					keysToFetch.Add(fillKey);
					indicesByKeys.Add(fillKey, i);
				}
			}
			System.Diagnostics.Debug.Assert(keysToFetch.Count > 0);
			for(int i = index - 1; i >= 0 && keysToFetch.Count < inSize; --i) {
				if(RowsByIndex.ContainsKey(i))
					break;
				object fillKey;
				if(!KeysByIndex.TryGetValue(i, out fillKey))
					break;
				int oldIndex;
				if(indicesByKeys.TryGetValue(fillKey, out oldIndex)) {
					inconsistencies.Add(string.Format("Key '{0}' found twice at indices {1} and {2}", fillKey, oldIndex, i));
				} else {
					keysToFetch.Insert(0, fillKey);
					indicesByKeys.Add(fillKey, i);
				}
			}
			if(fillFromSplit) {
				for(int i = index + splitLen; keysToFetch.Count < inSize; ++i) {
					if(RowsByIndex.ContainsKey(i))
						break;
					object fillKey;
					if(!KeysByIndex.TryGetValue(i, out fillKey))
						break;
					int oldIndex;
					if(indicesByKeys.TryGetValue(fillKey, out oldIndex)) {
						inconsistencies.Add(string.Format("Key '{0}' found twice at indices {1} and {2}", fillKey, oldIndex, i));
					} else {
						keysToFetch.Add(fillKey);
						indicesByKeys.Add(fillKey, i);
					}
				}
			}
		}
		void FillKeysToFetchListWeb(int index, IList<string> inconsistencies, out IList<object> keysToFetch, out IDictionary<object, int> indicesByKeys) {
			keysToFetch = new List<object>(_hintPageSize);
			indicesByKeys = new Dictionary<object, int>(_hintPageSize, KeysComparer);
			int pageStart = index / _hintPageSize * _hintPageSize;
			int nextPageStart = pageStart + _hintPageSize;
			for(int i = pageStart; i < nextPageStart; ++i) {
				if(RowsByIndex.ContainsKey(i))
					continue;
				object fillKey;
				if(!KeysByIndex.TryGetValue(i, out fillKey))
					continue;
				int oldIndex;
				if(indicesByKeys.TryGetValue(fillKey, out oldIndex)) {
					inconsistencies.Add(string.Format("Key '{0}' found twice at indices {1} and {2}", fillKey, oldIndex, i));
				} else {
					keysToFetch.Add(fillKey);
					indicesByKeys.Add(fillKey, i);
				}
			}
			System.Diagnostics.Debug.Assert(keysToFetch.Count > 0);
		}
		protected virtual object[] FetchRowsByKeys(object[] keys) {
			CriteriaOperator inOp = GetFetchRowsByKeysCondition(keys);
			var rows = FetchRows(inOp, new ServerModeOrderDescriptor[0], 0);
			return rows;
		}
		public static int DefaultMagicNumberFetchRowsInSize = 55;
		protected virtual int MagicNumberFetchRowsInSize { get { return DefaultMagicNumberFetchRowsInSize; } }
		public static int DefaultMagicNumberFetchRowsAllThreshold = 256;
		protected virtual int MagicNumberFetchRowsAllThreshold { get { return DefaultMagicNumberFetchRowsAllThreshold; } }
		public static int DefaultMagicNumberFetchKeysAllThreshold = 2048;
		protected virtual int MagicNumberFetchKeysAllThreshold { get { return DefaultMagicNumberFetchKeysAllThreshold; } }
		public static int DefaultMagicNumberFetchRowsTopThreshold = 100;
		protected virtual int MagicNumberFetchRowsTopThreshold { get { return DefaultMagicNumberFetchRowsTopThreshold; } }
		public static int DefaultMagicNumberFetchRowsTop = 128;
		protected virtual int MagicNumberFetchRowsTop { get { return DefaultMagicNumberFetchRowsTop; } }
		protected virtual int MagicNumberFetchKeysTopPenaltyGap { get { return 100; } }
		public static int DefaultMagicNumberFetchKeysModulo = 50;
		protected virtual int MagicNumberFetchKeysModulo { get { return DefaultMagicNumberFetchKeysModulo; } }
		protected virtual int MagicNumberTakeKeysBase { get { return 256; } }
		public static int DefaultMagicNumberScanKeysBase = 256;
		protected virtual int MagicNumberScanKeysBase { get { return DefaultMagicNumberScanKeysBase; } }
		public static int DefaultMagicNumberTakeKeysUpperLimitAfterSkip = int.MaxValue;
		protected virtual int MagicNumberTakeKeysUpperLimitAfterSkip { get { return DefaultMagicNumberTakeKeysUpperLimitAfterSkip; } }
		protected virtual double MagicNumberAllowedSlowerThenBase { get { return 1.6; } }
		protected virtual double MagicNumberAllowedSlowerThenBaseVariance { get { return .5; } }
		public static double DefaultMagicNumberTakeIncreaseStepMultiplier = 4.0;
		protected virtual double MagicNumberTakeIncreaseStepMultiplier { get { return DefaultMagicNumberTakeIncreaseStepMultiplier; } }
		public static bool WebPagingPrefetchNeighbourPage = true;
		public static bool? FetchRowsIsGood;
		bool? ActualFetchRowsIsGood;
		protected bool IsFetchRowsGoodIdeaForSure() {
			if(!ActualFetchRowsIsGood.HasValue)
				ActualFetchRowsIsGood = FetchRowsIsGood ?? DecideIsFetchRowsGoodIdeaForSure();
			return ActualFetchRowsIsGood.Value;
		}
		public CriteriaOperator IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria;
		protected virtual bool DecideIsFetchRowsGoodIdeaForSure() {
			return DecideIsFetchRowsGoodIdea(this.SortDescription, IsFetchRowsGoodIdeaForSureHint_FullestPossibleCriteria);
		}
		static bool DecideIsFetchRowsGoodIdea(ServerModeOrderDescriptor[] order, CriteriaOperator criteria) {
			if(order.Length > 2)
				return false;
			IDictionary<OperandProperty, object> columns = new Dictionary<OperandProperty, object>(order.Length);
			foreach(ServerModeOrderDescriptor d in order) {
				OperandProperty prop = CriteriaColumnAffinityResolver.GetAffinityColumn(d.SortExpression);
				if(string.IsNullOrEmpty(prop.PropertyName))
					return false;
				if(!columns.ContainsKey(prop))
					columns.Add(prop, null);
			}
			ICollection<OperandProperty> affi = CriteriaColumnAffinityResolver.SplitByColumns(criteria).Keys;
			if(affi.Count > 2)
				return false;
			foreach(OperandProperty filterProp in affi) {
				if(string.IsNullOrEmpty(filterProp.PropertyName))
					return false;
				if(!columns.ContainsKey(filterProp)) {
					if(columns.Count >= 2)
						return false;
					columns.Add(filterProp, null);
				}
			}
			return columns.Keys.All(op => op.PropertyName.IndexOf('.') < 0);
		}
		static readonly ServerModeServerAndChannelModel SafeModel = new ServerModeServerAndChannelModel(0.0, 1.0, 0.0);
		public override object GetRowKey(int index) {
			if(index < 0 || index >= Count())
				return null;
			object key;
			if(KeysByIndex.TryGetValue(index, out key))
				return key;
			if(IsLanded)
				return null;
			ServerModeGroupInfo gi = GetGroupForKeysFetchingAround(index);
			if(IsLanded)
				return null;
			if(MagicNumberFetchRowsAllThreshold > 0 && (_hintPageSize <= 0 || this.GroupByCount > 0) && gi.ChildDataRowCount <= Math.Max(MagicNumberFetchRowsAllThreshold, _hintMaxVisibleRows * 3) && IsFetchRowsGoodIdeaForSure()) {
				FetchRowsAll(gi);
				if(!KeysByIndex.TryGetValue(index, out key)) {
					if(IsLanded)
						return null;
					throw new InvalidOperationException("internal error (key absent after successful FetchRowsAll)");
				}
				return key;
			}
			int posInGroup = index - gi.TopRecordIndex;
			int posFromBottom = gi.ChildDataRowCount - posInGroup - 1;
			bool isFromBottom = posInGroup > posFromBottom;
			int fetchPosition = isFromBottom ? posFromBottom : posInGroup;
			int fetchCount = fetchPosition + 1;
			if(MagicNumberFetchRowsTopThreshold > 0 && fetchCount <= Math.Max(MagicNumberFetchRowsTopThreshold, _hintMaxVisibleRows * 2) && IsFetchRowsGoodIdeaForSure()) {
				int rowsToFetch;
				if(_hintPageSize > 0 && this.GroupByCount == 0) {
					if(isFromBottom) {
						int adjustment = _hintPageSize - gi.ChildDataRowCount % _hintPageSize;
						rowsToFetch = ((posFromBottom + adjustment) / _hintPageSize + 1) * _hintPageSize - adjustment;
					} else
						rowsToFetch = (posInGroup / _hintPageSize + 1) * _hintPageSize;
					if(WebPagingPrefetchNeighbourPage)
						rowsToFetch += _hintPageSize;
				} else
					rowsToFetch = Math.Max(MagicNumberFetchRowsTop, _hintMaxVisibleRows * 3);
				if(rowsToFetch >= gi.ChildDataRowCount)
					FetchRowsAll(gi);
				else
					FetchRowsTop(gi, isFromBottom, rowsToFetch);
				if(!KeysByIndex.TryGetValue(index, out key)) {
					if(IsLanded)
						return null;
					throw new InvalidOperationException("internal error (key absent after successful FetchRowsTop/FetchRowsAll)");
				}
				return key;
			}
			if(IsLanded)
				return null;
			SkipTakeParamsTake pureTakeParams = UseTakeEnforcer != false ? CalculateTakeParams(gi, isFromBottom, fetchCount) : null;
			SkipTakeParamsSkip skipParams = UseTakeEnforcer != true ? CalculateSkipTakeParams(gi, index) : null;
			if(skipParams != null) {
				if(skipParams.Skip == 0 || pureTakeParams == null) {
					DoFetchKeys(gi, skipParams.IsFromBottom, skipParams.Skip, skipParams.Take);
				} else {
					DoFetchKeysSkipWithTakeBackup(gi, skipParams.IsFromBottom, skipParams.Skip, skipParams.Take, pureTakeParams.IsFromBottom, pureTakeParams.Take);
				}
			} else if(pureTakeParams != null) {
				DoFetchKeys(gi, pureTakeParams.IsFromBottom, 0, pureTakeParams.Take);
			} else {
				throw new InvalidOperationException("internal error -- neither skip nor take were chosen");
			}
			if(!KeysByIndex.TryGetValue(index, out key)) {
				if(IsLanded)
					return null;
				throw new InvalidOperationException("internal error (key absent after successful DoFetchKeys*)");
			}
			return key;
		}
		class SkipTakeParamsTake {
			public int Take;
			public bool IsFromBottom;
		}
		class SkipTakeParamsSkip {
			public int Skip;
			public int Take;
			public bool IsFromBottom;
		}
		SkipTakeParamsSkip CalculateSkipTakeParams(ServerModeGroupInfo gi, int index) {
			int posInGroup = index - gi.TopRecordIndex;
			int minPosInGroupToFetch = Math.Max(KeysByIndex.GetFirstFilledIndex(index, true) - gi.TopRecordIndex + 1, 0);
			int maxPosInGroupToFetch = Math.Min(KeysByIndex.GetFirstFilledIndex(index, false) - gi.TopRecordIndex, gi.ChildDataRowCount) - 1;
			if(_hintPageSize > 0 && this.GroupByCount == 0) {
				int pageStart = posInGroup / _hintPageSize * _hintPageSize;
				int minIndexToFetch = Math.Max(pageStart - (WebPagingPrefetchNeighbourPage ? _hintPageSize : 0), minPosInGroupToFetch);
				int maxIndexToFetch = Math.Min(pageStart + (WebPagingPrefetchNeighbourPage ? 2 : 1) * _hintPageSize - 1, maxPosInGroupToFetch);
				int skipFromTop = minIndexToFetch;
				int skipFromBottom = gi.ChildDataRowCount - maxIndexToFetch - 1;
				int take = maxIndexToFetch - minIndexToFetch + 1;
				if(skipFromTop <= skipFromBottom)
					return new SkipTakeParamsSkip() { IsFromBottom = false, Skip = skipFromTop, Take = take };
				else
					return new SkipTakeParamsSkip() { IsFromBottom = true, Skip = skipFromBottom, Take = take };
			}
			ServerModeServerAndChannelModel model = regressor.Resolve();
#if !SL
			bool goodModel;
			if(model == null) {
				model = SafeModel;
				goodModel = false;
			} else {
				goodModel = true;
				if(ExplainSkipTake.Enabled) {
					Trace.WriteLine(string.Format("ExplainSkipTake: model: timeToFetch = {0}", model));
				}
			}
#else
				if(model == null) {
					model = SafeModel;
				}
#endif
			int maxAllowedTake = (int)Math.Min(MagicNumberTakeKeysUpperLimitAfterSkip, Math.Max(regressor.GetMaxObservableTake() ?? 0, 1024) * 4L);
			ServerModeOptimalFetchParam optimalParam = new ServerModeOptimalFetchParam(model, posInGroup, minPosInGroupToFetch, maxPosInGroupToFetch, gi.ChildDataRowCount, 128, maxAllowedTake, 1.66, 1.33, 1.15);
			ServerModeOptimalFetchResult optimalResult = ServerModeOptimalFetchHelper.CalculateOptimalFetchResult(optimalParam);
			ServerModeOptimalFetchParam minParam = new ServerModeOptimalFetchParam(SafeModel, posInGroup, minPosInGroupToFetch, maxPosInGroupToFetch, gi.ChildDataRowCount, 64, maxAllowedTake, 1, 1, 1);
			ServerModeOptimalFetchResult minResult = ServerModeOptimalFetchHelper.CalculateOptimalFetchResult(minParam);
			ServerModeOptimalFetchResult choosenResult = MinimiseSingleRequestTransferSizeInsteadOfOverallOptimisation ? minResult : optimalResult;
			bool rvIsFromBottom = choosenResult.IsFromEnd;
			int draftSkip = choosenResult.Skip;
			int skipPages = draftSkip / MagicNumberFetchKeysModulo;
			int rvSkip = skipPages * MagicNumberFetchKeysModulo;
			int draftScan = choosenResult.Skip + choosenResult.Take;
			int scanPages = draftScan / MagicNumberFetchKeysModulo;
			int realScan = scanPages * MagicNumberFetchKeysModulo + 1;
			if(realScan < draftScan)
				realScan += MagicNumberFetchKeysModulo;
			if(realScan > gi.ChildDataRowCount)
				realScan = gi.ChildDataRowCount;
			int rvTake = realScan == gi.ChildDataRowCount ? 0 : realScan - rvSkip;
#if !SL
			if(goodModel && ExplainSkipTake.Enabled) {
				double optimalTime = model.ConstantPart + model.TakeCoeff * optimalResult.Take + model.ScanCoeff * (optimalResult.Skip + optimalResult.Take);
				Trace.WriteLine(string.Format("ExplainSkipTake:  expected {0} with skip {1} take {2}", TimeSpan.FromSeconds(optimalTime), optimalResult.Skip, optimalResult.Take));
				double minTime = model.ConstantPart + model.TakeCoeff * minResult.Take + model.ScanCoeff * (minResult.Skip + minResult.Take);
				Trace.WriteLine(string.Format("ExplainSkipTake:  expected {0} with skip {1} take {2}", TimeSpan.FromSeconds(minTime), minResult.Skip, minResult.Take));
				double choosenTime = model.ConstantPart + model.TakeCoeff * (realScan - rvSkip) + model.ScanCoeff * realScan;
				Trace.WriteLine(string.Format("ExplainSkipTake:  expected {0} with skip {1} take {2}", TimeSpan.FromSeconds(choosenTime), rvSkip, rvTake));
			}
#endif
			return new SkipTakeParamsSkip() { IsFromBottom = rvIsFromBottom, Skip = rvSkip, Take = rvTake };
		}
		SkipTakeParamsTake CalculateTakeParams(ServerModeGroupInfo gi, bool isFromBottom, int fetchCount) {
			int fetchCountWithPenalty = fetchCount + MagicNumberFetchKeysTopPenaltyGap;
			int take = MagicNumberTakeKeysBase;
			while(fetchCountWithPenalty > take)
				take *= 2;
			if(take > gi.ChildDataRowCount / 2) {
				take = gi.ChildDataRowCount;
			}
			if(take < fetchCount)
				throw new InvalidOperationException(string.Format("internal error (pureTake({0}) < fetchCount({1})) (gi.ChildDataRowCount = {2})", take, fetchCount, gi.ChildDataRowCount));
			if(take > gi.ChildDataRowCount)
				throw new InvalidOperationException(string.Format("internal error (pureTake({0}) > gi.ChildDataRowCount({1}))", take, gi.ChildDataRowCount));
			return new SkipTakeParamsTake() { Take = take, IsFromBottom = take == gi.ChildDataRowCount ? false : isFromBottom };
		}
		public sealed override object FindFirstKeyByCriteriaOperator(CriteriaOperator filterCriteria, bool isReversed) {
			var keys = FetchKeys(filterCriteria, GetOrder(isReversed), 0, 1);
			if(keys.Length == 0)
				return null;
			else
				return keys[0];
		}
		protected abstract object[] FetchKeys(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take);
		private void FillKeys(ServerModeGroupInfo gi, bool isFromBottom, int skip, int take, object[] keys) {
			IList<string> inconsistencies = new List<string>();
			int firstRecord = GetGlobalIndex(gi, skip, isFromBottom);
			int validateCount = (take > 0) ? take : gi.ChildDataRowCount - skip;
			if(keys.Length != validateCount) {
				inconsistencies.Add(string.Format("Unexpected number of returned keys: {1}. Expected: {0}", validateCount, keys.Length));
			}
			int currentRow = firstRecord;
			int increment = isFromBottom ? -1 : 1;
			foreach(object fetchedKey in keys) {
				object oldKey;
				if(KeysByIndex.TryGetValue(currentRow, out oldKey)) {
					if(!KeyEquals(oldKey, fetchedKey)) {
						if(inconsistencies.Count < 4)
							inconsistencies.Add(string.Format("Key '{2}' fetched at index {0} does not match previously fetched key '{1}' for the same index", currentRow, oldKey, fetchedKey));
					}
				} else {
					KeysByIndex.Add(currentRow, fetchedKey);
				}
				currentRow += increment;
			}
			if(inconsistencies.Count > 0 && !IsLanded)
				RaiseInconsistencyDetected(inconsistencies);
		}
		void DoFetchKeys(ServerModeGroupInfo gi, bool isFromBottom, int skip, int take) {
			if(IsLanded)
				return;
			object[] keys;
			try {
				int patchedTake = take;
				if(take == 0 || skip + take == gi.ChildDataRowCount) {
					patchedTake = gi.ChildDataRowCount - skip + 1;
				}
				keys = WithReentryProtection(
					() => FetchKeysTimed(GetGroupWhere(gi), GetOrder(isFromBottom), skip, patchedTake)
					);
			} catch(Exception e) {
				Fatal(e);
				return;
			}
			FillKeys(gi, isFromBottom, skip, take, keys);
		}
		void DoFetchKeysSkipWithTakeBackup(ServerModeGroupInfo gi, bool skipIsFromBottom, int skipSkip, int skipTake, bool pureTakeIsFromBottom, int pureTake) {
			if(IsLanded)
				return;
			object[] keys;
			try {
				int patchedTake = skipTake;
				if(skipTake == 0 || skipSkip + skipTake == gi.ChildDataRowCount) {
					patchedTake = gi.ChildDataRowCount - skipSkip + 1;
				}
				keys = WithReentryProtection(
					() => FetchKeysTimed(GetGroupWhere(gi), GetOrder(skipIsFromBottom), skipSkip, patchedTake)
					);
				UseTakeEnforcer = false;
			} catch {
#if !SL
				Trace.WriteLineIf(ExplainSkipTake.Enabled, "ExplainSkipTake: exception caught on first skip; fallback to pure take...");
#endif
				UseTakeEnforcer = true;
				DoFetchKeys(gi, pureTakeIsFromBottom, 0, pureTake);
				return;
			}
			FillKeys(gi, skipIsFromBottom, skipSkip, skipTake, keys);
		}
		object[] FetchKeysTimed(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
			object[] rv = FetchKeys(where, order, skip, take);
			stopwatch.Stop();
			int actualFetch = rv.Length;
			int actualScan = actualFetch + skip;
			regressor.RegisterSample(actualFetch, actualScan, stopwatch.Elapsed.TotalSeconds);
#if ! SL
			if(ExplainSkipTake.Enabled) {
				Trace.WriteLine(string.Format("ExplainSkipTake:  actually {0} to fetch {1} keys; request: skip {2} take {3} group where ({4}) order by ({5})", stopwatch.Elapsed, actualFetch, skip, take, where, ServerModeOrderDescriptor.ToString(order)));
			}
#endif
			return rv;
		}
		bool? UseTakeEnforcer = ForceTake;
		public static bool MinimiseSingleRequestTransferSizeInsteadOfOverallOptimisation;
		[Obsolete("Not needed anymore. If you want to force Take, use ForceTake instead. If you want to chop server responses to smallest possible chunks use MinimiseSingleRequestTransferSizeInsteadOfOverallOptimisation instead")]
		public static bool? ForceSkip {
			get {
				if(ForceTake.HasValue)
					return !ForceTake.Value;
				else
					return null;
			}
			set {
				if(value.HasValue)
					ForceTake = !value.Value;
				else
					ForceTake = null;
			}
		}
		public static bool? ForceTake;
		readonly ServerModeServerAndChannelModelBuilder regressor = new ServerModeServerAndChannelModelBuilder();
		static int GetGlobalIndex(ServerModeGroupInfo gi, int pos, bool isReversed) {
			if(isReversed) {
				return gi.TopRecordIndex + gi.ChildDataRowCount - pos - 1;
			} else {
				return gi.TopRecordIndex + pos;
			}
		}
		ServerModeGroupInfo GetGroupForKeysFetchingAround(int index) {
			ServerModeGroupInfo gi = TotalGroupInfo;
			for(; ; ) {
				if(gi.Level + 1 >= GroupByCount)
					break;
				ServerModeGroupInfo ngi = null;
				foreach(ServerModeGroupInfo g in GetGroupInfo(gi)) {
					if(g.TopRecordIndex + g.ChildDataRowCount <= index)
						continue;
					ngi = g;
					break;
				}
				if(IsLanded)
					break;
				if(ngi == null) {
					RaiseInconsistencyDetected("Can't find an appropriate group for row {0}", index);
					break;
				}
				gi = ngi;
			}
			return gi;
		}
		void FetchRowsAll(ServerModeGroupInfo gi) {
			CriteriaOperator where = GetGroupWhere(gi);
			FetchRows(where, gi.ChildDataRowCount + 1, gi.ChildDataRowCount, gi.TopRecordIndex, false);
		}
		ServerModeOrderDescriptor[] GetOrder(bool isReversed) {
			return GetOrder(SortDescription, isReversed);
		}
		static ServerModeOrderDescriptor[] GetOrder(ServerModeOrderDescriptor[] src, bool isReversed) {
			if(isReversed) {
				List<ServerModeOrderDescriptor> rv = new List<ServerModeOrderDescriptor>(src.Length);
				foreach(ServerModeOrderDescriptor descr in src) {
					rv.Add(new ServerModeOrderDescriptor(descr.SortExpression, !descr.IsDesc, descr.AuxExpression));
				}
				return rv.ToArray();
			} else {
				return src;
			}
		}
		void FetchRowsTop(ServerModeGroupInfo gi, bool isFromBottom, int top) {
			CriteriaOperator where = GetGroupWhere(gi);
			int firstRecord = isFromBottom ? gi.TopRecordIndex + gi.ChildDataRowCount - 1 : gi.TopRecordIndex;
			FetchRows(where, top, top, firstRecord, isFromBottom);
		}
		void FetchRows(CriteriaOperator where, int take, int validateCount, int firstRecord, bool isFromBottom) {
			if(IsLanded)
				return;
			object[] rows;
			try {
				rows = WithReentryProtection(
					() => FetchRows(where, GetOrder(isFromBottom), take)
					);
			} catch(Exception e) {
				Fatal(e);
				return;
			}
			IList<string> inconsistencies = new List<string>();
			if(rows.Length != validateCount) {
				inconsistencies.Add(string.Format("Unexpected number of rows returned: {1}. Expected: {0}", validateCount, rows.Length));
			}
			int currentRow = firstRecord;
			int increment = isFromBottom ? -1 : 1;
			foreach(object fetchedRow in rows) {
				object fetchedKey = GetKeyFromRow(fetchedRow);
				object oldKey;
				if(KeysByIndex.TryGetValue(currentRow, out oldKey)) {
					if(!KeyEquals(oldKey, fetchedKey)) {
						if(inconsistencies.Count < 4) {
							inconsistencies.Add(string.Format("Key '{2}' of the row fetched at index {0} does not match previously fetched key '{1}' for the same index", currentRow, oldKey, fetchedKey));
						}
					}
				} else {
					KeysByIndex.Add(currentRow, fetchedKey);
					if(!RowsByIndex.ContainsKey(currentRow))
						RowsByIndex.Add(currentRow, fetchedRow);
				}
				currentRow += increment;
			}
			if(inconsistencies.Count > 0 && !IsLanded)
				RaiseInconsistencyDetected(inconsistencies);
		}
		protected abstract object[] FetchRows(CriteriaOperator where, ServerModeOrderDescriptor[] order, int take);
		public override int GetRowIndexByKey(object key) {
			if(key == null || key is DBNull)
				return -1;
			int index;
			if(KeysByIndex.TryGetKeyByValue(key, out index, 0, int.MaxValue))
				return index;
			if(IsLanded)
				return -1;
			object row;
			try {
				object[] rowarr = WithReentryProtection(
					() => FetchRowsByKeys(new object[] { key })
					);
				if(rowarr.Length == 0)
					return -1;
				row = rowarr[0];
			} catch {
				return -1;
			}
			if(IsLanded)
				return -1;
			if(row == null)
				return -1;
			ServerModeGroupInfo cgi = this.TotalGroupInfo;
			for(; ; ) {
				int groupLevel = cgi.Level + 1;
				if(groupLevel >= GroupByCount)
					break;
				object groupVal = EvaluateOnInstance(row, SortDescription[groupLevel].SortExpression);
				ServerModeGroupInfo ngi = null;
				List<ListSourceGroupInfo> subgroups = GetGroupInfo(cgi);
				if(IsLanded)
					return -1;
				foreach(ServerModeGroupInfo g in subgroups) {
					if(KeyEquals(groupVal, g.GroupValue)) {
						ngi = g;
						break;
					}
				}
				if(ngi == null) {
					RaiseInconsistencyDetected("Can't find appropriate group for row with key '{0}'", key);
					return -1;
				}
				cgi = ngi;
			}
			bool allKeysFetched = true;
			for(int i = 0; i < cgi.ChildDataRowCount; ++i) {
				if(!KeysByIndex.ContainsKey(i + cgi.TopRecordIndex)) {
					allKeysFetched = false;
					break;
				}
			}
			if(allKeysFetched) {
				if(KeysByIndex.TryGetKeyByValue(key, out index, cgi.TopRecordIndex, cgi.TopRecordIndex + cgi.ChildDataRowCount))
					return index;
				RaiseInconsistencyDetected("Can't find key '{0}' in the completely fetched group", key);
				return -1;
			}
			if(cgi.ChildDataRowCount <= MagicNumberFetchKeysAllThreshold) {
				if(cgi.ChildDataRowCount <= MagicNumberFetchRowsAllThreshold && IsFetchRowsGoodIdeaForSure()) {
					FetchRowsAll(cgi);
				} else {
					DoFetchKeys(cgi, false, 0, 0);
				}
				if(KeysByIndex.TryGetKeyByValue(key, out index, cgi.TopRecordIndex, cgi.TopRecordIndex + cgi.ChildDataRowCount))
					return index;
				if(IsLanded)
					return -1;
				RaiseInconsistencyDetected("Can't find key '{0}' in the newly fetched group", key);
				return -1;
			}
			CriteriaOperator whereBefore = null;
			for(int i = this.SortDescription.Length - 1; i >= 0; --i) {
				ServerModeOrderDescriptor od = this.SortDescription[i];
				OperandValue ov = new OperandValue(EvaluateOnInstance(row, od.SortExpression));
				CriteriaOperator strong = MakeStrongClause(od, ov);
				if(ReferenceEquals(whereBefore, null) || ReferenceEquals(whereBefore, FalseMarker)) {
					whereBefore = strong;
				} else {
					if(ReferenceEquals(strong, FalseMarker))
						whereBefore = MakeEqClause(od, ov) & whereBefore;
					else
						whereBefore = strong | (MakeEqClause(od, ov) & whereBefore);
				}
			}
			CriteriaOperator groupWhere = GetGroupWhere(cgi);
			if(IsLanded)
				return -1;
			int cnt;
			if(ReferenceEquals(whereBefore, FalseMarker)) {
				cnt = 0;
			} else {
				try {
					cnt = GetCount(GroupOperator.And(groupWhere, whereBefore));
				} catch {
					return -1;
				}
			}
			int guessedIndex = cnt + cgi.TopRecordIndex;
			object keyAtGuessedIndex = GetRowKey(guessedIndex);
			if(KeyEquals(key, keyAtGuessedIndex))
				return guessedIndex;
			if(KeysByIndex.TryGetKeyByValue(key, out index, cgi.TopRecordIndex, cgi.TopRecordIndex + cgi.ChildDataRowCount))
				return index;
			return -1;
		}
		protected abstract int GetCount(CriteriaOperator criteriaOperator);
		protected override int PrepareTopGroupCount() {
			return GetCount(null);
		}
		protected abstract object EvaluateOnInstance(object row, CriteriaOperator criteriaOperator);
		static CriteriaOperator MakeEqClause(ServerModeOrderDescriptor od, OperandValue ov) {
			if(ov.Value == null) {
				return od.SortExpression.IsNull();
			} else {
				return od.SortExpression == ov;
			}
		}
		static readonly CriteriaOperator FalseMarker = new OperandValue(false);
		static CriteriaOperator MakeStrongClause(ServerModeOrderDescriptor od, OperandValue ov) {
			if(ov.Value == null) {
				if(od.IsDesc)
					return od.SortExpression.IsNotNull();
				else
					return FalseMarker;
			}
			if(ov.Value is bool) {
				bool val = (bool)ov.Value;
				if(od.IsDesc) {
					if(val == false)
						return od.SortExpression == new OperandValue(true);
				} else {
					if(val == true)
						return od.SortExpression == new OperandValue(false);
				}
				return FalseMarker;
			}
			if(od.IsDesc) {
				return od.SortExpression > ov;
			} else {
				return od.SortExpression < ov | od.SortExpression.IsNull();
			}
		}
		protected CriteriaOperator GetFetchRowsByKeysCondition(object[] keys) {
			CriteriaOperator inOp;
			if(this.KeysCriteria.Length == 1) {
				if(keys.Length == 1) {
					inOp = KeysCriteria[0] == new OperandValue(keys[0]);
				} else {
					inOp = new InOperator(KeysCriteria[0], keys.Select(key => new OperandValue(key)).ToArray());
				}
			} else {
				List<CriteriaOperator> ops = new List<CriteriaOperator>(keys.Length);
				foreach(object _key in keys) {
					ServerModeCompoundKey key = _key as ServerModeCompoundKey;
					if(key == null || key.SubKeys.Length != KeysCriteria.Length)
						continue;
					List<CriteriaOperator> subCriteria = new List<CriteriaOperator>(KeysCriteria.Length);
					for(int i = 0; i < KeysCriteria.Length; ++i) {
						object subKey = key.SubKeys[i];
						if(subKey == null)
							subCriteria.Add(KeysCriteria[i].IsNull());
						else
							subCriteria.Add(KeysCriteria[i] == new OperandValue(subKey));
					}
					ops.Add(GroupOperator.And(subCriteria));
				}
				inOp = GroupOperator.Or(ops);
			}
			return inOp;
		}
		int _hintPageSize;
		void IListServerHints.HintGridIsPaged(int pageSize) {
			_hintPageSize = pageSize;
		}
		int _hintMaxVisibleRows;
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			_hintMaxVisibleRows = rowsInGrid;
		}
		public override bool PrefetchRows(IEnumerable<ListSourceGroupInfo> groupsToPrefetch, CancellationToken cancellationToken) {
			if(IsLanded)
				return false;
			if(groupsToPrefetch == null)
				groupsToPrefetch = new ListSourceGroupInfo[] { TotalGroupInfo };
			var flattenedGroups = FlattenGroups(groupsToPrefetch, cancellationToken).ToArray();
			if(IsLanded || cancellationToken.IsCancellationRequested)
				return false;
			foreach(ServerModeGroupInfo gri in flattenedGroups) {
				if(cancellationToken.IsCancellationRequested)
					return false;
				PrefetchRows(gri);
				if(IsLanded)
					return false;
			}
			return true;
		}
		IEnumerable<ServerModeGroupInfo> FlattenGroups(IEnumerable<ListSourceGroupInfo> groupsToPrefetch, CancellationToken cancellationToken) {
			foreach(ServerModeGroupInfo sgi in groupsToPrefetch) {
				if(cancellationToken.IsCancellationRequested)
					yield break;
				if(sgi.Level + 1 >= GroupByCount) {
					yield return sgi;
				} else {
					foreach(ServerModeGroupInfo n in FlattenGroups(GetGroupInfo(sgi), cancellationToken)) {
						yield return n;
					}
				}
			}
		}
		void PrefetchRows(ServerModeGroupInfo gri) {
			if(IsLanded)
				return;
			int firstIndex = gri.TopRecordIndex;
			int lastIndex = gri.TopRecordIndex + gri.ChildDataRowCount - 1;
			int firstNotFetched = -1;
			for(int i = firstIndex; i <= lastIndex; ++i) {
				if(!RowsByIndex.ContainsKey(i)) {
					firstNotFetched = i;
					break;
				}
			}
			if(firstNotFetched < 0)
				return;
			int lastNotFetched = -1;
			for(int i = lastIndex; i >= firstIndex; --i) {
				if(!RowsByIndex.ContainsKey(i)) {
					lastNotFetched = i;
					break;
				}
			}
			System.Diagnostics.Debug.Assert(lastNotFetched >= firstNotFetched);
			if(lastNotFetched - firstNotFetched <= MagicNumberFetchRowsInSize * 3) {
				bool allKeysFetched = true;
				for(int i = firstNotFetched; i <= lastNotFetched; ++i) {
					if(!KeysByIndex.ContainsKey(i)) {
						allKeysFetched = false;
						break;
					}
				}
				if(allKeysFetched) {
					for(int i = firstNotFetched; i <= lastNotFetched && !IsLanded; ++i) {
						GC.KeepAlive(Indexer(i));
					}
					return;
				}
			}
			int fetchedFromTop = firstNotFetched - firstIndex;
			int fetchedFromBottom = lastIndex - lastNotFetched;
			if((fetchedFromTop + fetchedFromBottom) * 4 < gri.ChildDataRowCount) {
				FetchRowsAll(gri);
			} else if(fetchedFromTop <= fetchedFromBottom) {
				FetchRowsTop(gri, false, gri.ChildDataRowCount - fetchedFromBottom + 1);
			} else {
				FetchRowsTop(gri, true, gri.ChildDataRowCount - fetchedFromTop + 1);
			}
		}
	}
	public abstract class ServerModeKeyedCacheExtendable : ServerModeKeyedCache {
		protected virtual CriteriaOperator ExternalCriteria { get { return null; } }
		protected ServerModeKeyedCacheExtendable(CriteriaOperator[] keysCriteria, ServerModeOrderDescriptor[] sortInfo, int groupCount, ServerModeSummaryDescriptor[] summary, ServerModeSummaryDescriptor[] totalSummary)
			: base(keysCriteria, sortInfo, groupCount, summary, totalSummary) {
		}
		public event EventHandler<CustomGetCountEventArgs> CustomGetCount;
		public event EventHandler<CustomPrepareChildrenEventArgs> CustomPrepareChildren;
		public event EventHandler<CustomPrepareTopGroupInfoEventArgs> CustomPrepareTopGroupInfo;
		public event EventHandler<CustomFetchKeysEventArgs> CustomFetchKeys;
		protected sealed override int GetCount(CriteriaOperator criteriaOperator) {
			if(CustomGetCount != null) {
				CriteriaOperator fullWhere = ReferenceEquals(ExternalCriteria, null) ? criteriaOperator : ExternalCriteria & criteriaOperator;
				CustomGetCountEventArgs args = new CustomGetCountEventArgs(fullWhere);
				CustomGetCount(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			return GetCountInternal(criteriaOperator);
		}
		int? fetchRowsSize;
		protected override int MagicNumberTakeKeysUpperLimitAfterSkip {
			get {
				if(fetchRowsSize.HasValue) return fetchRowsSize.Value;
				return base.MagicNumberTakeKeysUpperLimitAfterSkip;
			}
		}
		readonly Dictionary<object, object> keyObjectCache = new Dictionary<object, object>(1024);
		int GetCurrentTake(int take, int position) {
			return !fetchRowsSize.HasValue ? take : take == 0 ? fetchRowsSize.Value : Math.Min(take - position, fetchRowsSize.Value);
		}
		bool ProcessException(int take) {
			if(!fetchRowsSize.HasValue) {
				fetchRowsSize = take == 0 ? (MagicNumberTakeKeysUpperLimitAfterSkip < 8192 ? MagicNumberTakeKeysUpperLimitAfterSkip : 8192) : take;
			}
			if(fetchRowsSize <= 1) {
				fetchRowsSize = null;
				return true;
			}
			fetchRowsSize = fetchRowsSize / 2;
			return false;
		}
		static bool NeedBreak(int currentTake, int currentCounter) {
			return currentTake == 0 || currentCounter < currentTake;
		}
		protected sealed override object[] FetchKeys(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			if(CustomFetchKeys != null) {
				CriteriaOperator fullWhere = ReferenceEquals(ExternalCriteria, null) ? where : ExternalCriteria & where;
				CustomFetchKeysEventArgs args = new CustomFetchKeysEventArgs(fullWhere, order, skip, take);
				CustomFetchKeys(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			object source = FetchPrepare(where, order);
			List<object> rows = new List<object>(take);
			List<object> keys = new List<object>(take);
			int position = 0;
			while((position < take) || (take == 0)) {
				int currentSkip = skip + position;
				int currentTake = GetCurrentTake(take, position);
				IEnumerable keysCore;
				IEnumerable rowsCore;
				try {
					FetchKeysCore(source, currentSkip, currentTake, out keysCore, out rowsCore);
				} catch(Exception) {
					if(ProcessException(take)) throw;
					continue;
				}
				if(rowsCore != null) {
					foreach(object row in rowsCore) {
						rows.Add(row);
					}
				}
				int currentCounter = 0;
				foreach(object key in keysCore) {
					keys.Add(key);
					currentCounter++;
				}
				position += currentTake;
				if(NeedBreak(currentTake, currentCounter)) {
					break;
				}
			}
			if(rows.Count == keys.Count) {
				for(int i = 0; i < rows.Count; i++) {
					keyObjectCache[keys[i]] = rows[i];
				}
			}
			return keys.ToArray();
		}
		protected sealed override object[] FetchRows(CriteriaOperator where, ServerModeOrderDescriptor[] order, int take) {
			object source = FetchPrepare(where, order);
			List<object> rows = new List<object>(take);
			int position = 0;
			while((position < take) || (take == 0)) {
				int currentTake = GetCurrentTake(take, position);
				IEnumerable rowsCore;
				try {
					rowsCore = FetchRowsCore(source, position, currentTake);
				} catch(Exception) {
					if(ProcessException(take)) throw;
					continue;
				}
				int currentCounter = 0;
				if(rowsCore != null) {
					foreach(object row in rowsCore) {
						rows.Add(row);
						currentCounter++;
					}
				}
				position += currentTake;
				if(NeedBreak(currentTake, currentCounter)) {
					break;
				}
			}
			return rows.ToArray();
		}
		int maxInSize = 128;
		protected override int MaxInSize {
			get { return maxInSize; }
		}
		protected sealed override object[] FetchRowsByKeys(object[] keys) {
			List<int> cacheIndex = null;
			List<object> cacheResult = null;
			List<int> queryIndex = null;
			List<object> queryResult = null;
			List<object> operands = null;
			for(int i = 0; i < keys.Length; i++) {
				object key = keys[i];
				object row;
				if(keyObjectCache.TryGetValue(key, out row)) {
					if(cacheIndex == null) {
						cacheIndex = new List<int>(keys.Length);
						cacheResult = new List<object>(keys.Length);
					}
					keyObjectCache.Remove(key);
					cacheIndex.Add(i);
					cacheResult.Add(row);
				} else {
					if(queryIndex == null) {
						queryIndex = new List<int>(keys.Length);
						queryResult = new List<object>(keys.Length);
						operands = new List<object>(keys.Length);
					}
					queryIndex.Add(i);
					operands.Add(key);
				}
			}
			if(operands != null && operands.Count > 0) {
				int position = 0;
				while (true) {
					try {
						while(operands.Count - position > 0) {
							int nextRequestSize = Math.Min(operands.Count - position, maxInSize);
							IEnumerable currentRows = FetchRowsByKeysCore(operands.GetRange(position, nextRequestSize).ToArray());
							if(currentRows != null) {
								IEnumerable<object> objectRows = currentRows as IEnumerable<object>;
								if(objectRows != null) {
									queryResult.AddRange(objectRows);
								} else {
									foreach(object row in currentRows) {
										queryResult.Add(row);
									}
								}
							}
							position += nextRequestSize;
						}
						break;
					} catch(Exception) {
						if(maxInSize == 1) throw;
						maxInSize = maxInSize / 2;
					}
				}
				if(cacheResult != null && cacheResult.Count > 0) {
					object[] result = new object[keys.Length];
					for(int i = 0; i < cacheResult.Count; i++) {
						result[cacheIndex[i]] = cacheResult[i];
					}
					for(int i = 0; i < queryResult.Count; i++) {
						result[queryIndex[i]] = queryResult[i];
					}
					return result;
				}
				return queryResult.ToArray();
			}
			return cacheResult == null ? new object[0] : cacheResult.ToArray();
		}
		protected sealed override ServerModeGroupInfoData[] PrepareChildren(CriteriaOperator groupWhere, CriteriaOperator groupByCriterion, CriteriaOperator orderByCriterion, bool isDesc, ServerModeSummaryDescriptor[] summaries) {
			ServerModeOrderDescriptor groupByDescriptor = new ServerModeOrderDescriptor(groupByCriterion, isDesc);
			if(CustomPrepareChildren != null) {
				CriteriaOperator fullWhere = ReferenceEquals(ExternalCriteria, null) ? groupWhere : ExternalCriteria & groupWhere;
				CustomPrepareChildrenEventArgs args = new CustomPrepareChildrenEventArgs(fullWhere, groupByDescriptor, summaries);
				CustomPrepareChildren(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			return PrepareChildrenInternal(groupWhere, groupByDescriptor, summaries);
		}
		protected sealed override ServerModeGroupInfoData PrepareTopGroupInfo(ServerModeSummaryDescriptor[] summaries) {
			if(CustomPrepareTopGroupInfo != null) {
				CustomPrepareTopGroupInfoEventArgs args = new CustomPrepareTopGroupInfoEventArgs(ExternalCriteria, summaries);
				CustomPrepareTopGroupInfo(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			return PrepareTopGroupInfoInternal(summaries);
		}
		protected abstract object FetchPrepare(CriteriaOperator where, ServerModeOrderDescriptor[] order);
		protected abstract void FetchKeysCore(object source, int skip, int take, out IEnumerable keys, out IEnumerable rows);
		protected abstract IEnumerable FetchRowsCore(object source, int skip, int take);
		protected abstract IEnumerable FetchRowsByKeysCore(object[] keys);
		protected abstract int GetCountInternal(CriteriaOperator criteriaOperator);
		protected abstract ServerModeGroupInfoData[] PrepareChildrenInternal(CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries);
		protected abstract ServerModeGroupInfoData PrepareTopGroupInfoInternal(ServerModeSummaryDescriptor[] summaries);
	}
	public class CustomGetCountEventArgs : EventArgs {
		public bool Handled { get; set; }
		public int Result { get; set; }
		readonly CriteriaOperator where;
		public CriteriaOperator Where {
			get { return where; }
		}
		public CustomGetCountEventArgs(CriteriaOperator where) {
			this.where = where;
		}
	}
	public class CustomPrepareChildrenEventArgs : EventArgs {
		public bool Handled { get; set; }
		public ServerModeGroupInfoData[] Result { get; set; }
		CriteriaOperator groupWhere;
		ServerModeOrderDescriptor groupByDescriptor;
		ServerModeSummaryDescriptor[] summaries;
		public CriteriaOperator GroupWhere {
			get { return groupWhere; }
		}
		public ServerModeOrderDescriptor GroupByDescriptor {
			get { return groupByDescriptor; }
		}
		public ServerModeSummaryDescriptor[] Summaries {
			get { return summaries; }
		}
		public CustomPrepareChildrenEventArgs(CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries) {
			this.groupWhere = groupWhere;
			this.groupByDescriptor = groupByDescriptor;
			this.summaries = summaries;
		}
	}
	public class CustomPrepareTopGroupInfoEventArgs : EventArgs {
		public bool Handled { get; set; }
		public ServerModeGroupInfoData Result { get; set; }
		CriteriaOperator where;
		public CriteriaOperator Where {
			get { return where; }
		}
		ServerModeSummaryDescriptor[] summaries;
		public ServerModeSummaryDescriptor[] Summaries {
			get { return summaries; }
		}
		public CustomPrepareTopGroupInfoEventArgs(CriteriaOperator where, ServerModeSummaryDescriptor[] summaries) {
			this.where = where;
			this.summaries = summaries;
		}
	}
	public class CustomFetchKeysEventArgs : EventArgs {
		public bool Handled { get; set; }
		public object[] Result { get; set; }
		CriteriaOperator where;
		ServerModeOrderDescriptor[] order;
		int skip;
		int take;
		public CriteriaOperator Where {
			get { return where; }
		}
		public ServerModeOrderDescriptor[] Order {
			get { return order; }
		}
		public int Skip {
			get { return skip; }
		}
		public int Take {
			get { return take; }
		}
		public CustomFetchKeysEventArgs(CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			this.where = where;
			this.order = order;
			this.skip = skip;
			this.take = take;
		}
	}
	public class CustomGetUniqueValuesEventArgs : EventArgs {
		public bool Handled { get; set; }
		public object[] Result { get; set; }
		CriteriaOperator expression;
		int maxCount;
		CriteriaOperator where;
		public CriteriaOperator Expression {
			get { return expression; }
		}
		public int MaxCount {
			get { return maxCount; }
		}
		public CriteriaOperator Where {
			get { return where; }
		}
		public CustomGetUniqueValuesEventArgs(CriteriaOperator expression, int maxCount, CriteriaOperator where) {
			this.expression = expression;
			this.maxCount = maxCount;
			this.where = where;
		}
	}
#if !DXPORTABLE
	public class ServerModeCoreExtender {
		public event EventHandler<CustomGetCountEventArgs> CustomGetCount;
		public event EventHandler<CustomPrepareChildrenEventArgs> CustomPrepareChildren;
		public event EventHandler<CustomPrepareTopGroupInfoEventArgs> CustomPrepareTopGroupInfo;
		public event EventHandler<CustomFetchKeysEventArgs> CustomFetchKeys;
		public event EventHandler<CustomGetUniqueValuesEventArgs> CustomGetUniqueValues;
		readonly static ICriteriaToExpressionConverter Converter = new CriteriaToExpressionConverter();
		public static ServerModeGroupInfoData PrepareTopGroupInfo(IQueryable q, CriteriaOperator where, ServerModeSummaryDescriptor[] summaries) {
			return PrepareTopGroupInfo(q, Converter, where, summaries);
		}
		public static ServerModeGroupInfoData PrepareTopGroupInfo(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator where, ServerModeSummaryDescriptor[] summaries) {
			return DevExpress.Data.Linq.Helpers.LinqServerModeCache.PrepareTopGroupInfoStatic(q, converter, where, summaries);
		}
		public static ServerModeGroupInfoData[] PrepareChildren(IQueryable q, CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries) {
			return PrepareChildren(q, Converter, groupWhere, groupByDescriptor, summaries);
		}
		public static ServerModeGroupInfoData[] PrepareChildren(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator groupWhere, ServerModeOrderDescriptor groupByDescriptor, ServerModeSummaryDescriptor[] summaries) {
			return DevExpress.Data.Linq.Helpers.LinqServerModeCache.PrepareChildrenStatic(q, converter, groupWhere, groupByDescriptor, summaries);
		}
		public static object[] FetchKeys(IQueryable q, CriteriaOperator[] keysCriteria, CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			return FetchKeys(q, Converter, keysCriteria, where, order, skip, take);
		}
		public static object[] FetchKeys(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator[] keysCriteria, CriteriaOperator where, ServerModeOrderDescriptor[] order, int skip, int take) {
			return DevExpress.Data.Linq.Helpers.LinqServerModeCache.FetchKeysStatic(q, converter, keysCriteria, where, order, skip, take);
		}
		public static int GetCount(IQueryable q, CriteriaOperator where) {
			return GetCount(q, Converter, where);
		}
		public static int GetCount(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator where) {
			return DevExpress.Data.Linq.Helpers.LinqServerModeCache.GetCountStatic(q, converter, where);
		}
		public static object[] GetUniqueValues(IQueryable q, CriteriaOperator expression, int maxCount, CriteriaOperator where) {
			return GetUniqueValues(q, Converter, expression, maxCount, where);
		}
		public static object[] GetUniqueValues(IQueryable q, ICriteriaToExpressionConverter converter, CriteriaOperator expression, int maxCount, CriteriaOperator where) {
			return DevExpress.Data.Linq.Helpers.LinqServerModeCore.GetUniqueValuesStatic(q, converter, expression, maxCount, where);
		}
		protected virtual void OnCustomPrepareTopGroupInfo(object sender, CustomPrepareTopGroupInfoEventArgs e) {
			if(CustomPrepareTopGroupInfo == null) return;
			CustomPrepareTopGroupInfo(sender, e);
		}
		void OnCustomPrepareChildren(object sender, CustomPrepareChildrenEventArgs e) {
			if(CustomPrepareChildren == null) return;
			CustomPrepareChildren(sender, e);
		}
		void OnCustomGetCount(object sender, CustomGetCountEventArgs e) {
			if(CustomGetCount == null) return;
			CustomGetCount(sender, e);
		}
		void OnCustomFetchKeys(object sender, CustomFetchKeysEventArgs e) {
			if(CustomFetchKeys == null) return;
			CustomFetchKeys(sender, e);
		}
		void OnCustomGetUniqueValues(object sender, CustomGetUniqueValuesEventArgs e) {
			if(CustomGetUniqueValues == null) return;
			CustomGetUniqueValues(sender, e);
		}
		internal void Subscribe(ServerModeKeyedCacheExtendable serverModeCache) {
			serverModeCache.CustomFetchKeys += OnCustomFetchKeys;
			serverModeCache.CustomGetCount += OnCustomGetCount;
			serverModeCache.CustomPrepareChildren += OnCustomPrepareChildren;
			serverModeCache.CustomPrepareTopGroupInfo += OnCustomPrepareTopGroupInfo;
		}
		internal void Subscribe(ServerModeCoreExtendable serverModeCore) {
			serverModeCore.CustomGetUniqueValues += OnCustomGetUniqueValues;
		}
	}
#endif
#if !DXPORTABLE
	public abstract class ServerModeCoreExtendable : ServerModeCore {
		public event EventHandler<CustomGetUniqueValuesEventArgs> CustomGetUniqueValues;
		readonly protected ServerModeCoreExtender Extender;
		protected ServerModeCoreExtendable(CriteriaOperator[] key, ServerModeCoreExtender extender)
			: base(key) {
			Extender = extender;
			Extender.Subscribe(this);
		}
		protected sealed override ServerModeCache CreateCacheCore() {
			ServerModeKeyedCacheExtendable serverModeCache = CreateCacheCoreExtendable();
			if(Extender != null) {
				Extender.Subscribe(serverModeCache);
			}
			return serverModeCache;
		}
		protected sealed override object[] GetUniqueValues(CriteriaOperator expression, int maxCount, CriteriaOperator filter) {
			if(CustomGetUniqueValues != null) {
				CustomGetUniqueValuesEventArgs args = new CustomGetUniqueValuesEventArgs(expression, maxCount, filter);
				CustomGetUniqueValues(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			return GetUniqueValuesInternal(expression, maxCount, filter);
		}
		protected abstract object[] GetUniqueValuesInternal(CriteriaOperator expression, int maxCount, CriteriaOperator filter);
		protected abstract ServerModeKeyedCacheExtendable CreateCacheCoreExtendable();
	}
#endif
	public abstract class ServerModeCore : IListServer, IListServerHints, IDXCloneable {
		protected readonly CriteriaOperator[] KeysCriteria;
		protected CriteriaOperator FilterClause;
		protected ServerModeOrderDescriptor[] SortInfo;
		protected int GroupCount = 0;
		protected ServerModeSummaryDescriptor[] SummaryInfo = new ServerModeSummaryDescriptor[0];
		protected ServerModeSummaryDescriptor[] TotalSummaryInfo = new ServerModeSummaryDescriptor[0];
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		public string DefaultSorting;
		public static bool? DefaultForceCaseInsensitiveForAnySource;
		public bool ForceCaseInsensitiveForAnySource = DefaultForceCaseInsensitiveForAnySource ?? false;
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected abstract ServerModeCore DXCloneCreate();
		protected virtual ServerModeCore DXClone() {
			ServerModeCore clone = DXCloneCreate();
			clone.useToLower = this.useToLower;
			clone.FilterClause = this.FilterClause;
			clone.SortInfo = this.SortInfo;
			clone.GroupCount = this.GroupCount;
			clone.SummaryInfo = this.SummaryInfo;
			clone.TotalSummaryInfo = this.TotalSummaryInfo;
			clone.InconsistencyDetected = this.InconsistencyDetected;
			clone.ExceptionThrown = this.ExceptionThrown;
			clone.DefaultSorting = this.DefaultSorting;
			clone.ForceCaseInsensitiveForAnySource = this.ForceCaseInsensitiveForAnySource;
			return clone;
		}
		protected ServerModeCore(CriteriaOperator[] key) {
			this.KeysCriteria = key;
			this.SortInfo = this.KeysCriteria.Select(c => new ServerModeOrderDescriptor(c, false)).ToArray();
		}
		ServerModeCache _cache;
		protected ServerModeCache Cache {
			get {
				if(_cache == null) {
					_cache = CreateCache();
					ApplyHints();
				}
				return _cache;
			}
		}
		ServerModeCache CreateCache() {
			ServerModeCache rv = CreateCacheCore();
			rv.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(_cache_InconsistencyDetected);
			rv.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(_cache_ExceptionThrown);
			return rv;
		}
		void _cache_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			RaiseExceptionThrown(e);
		}
		protected virtual void RaiseExceptionThrown(ServerModeExceptionThrownEventArgs e) {
			if(ExceptionThrown == null)
				return;
			ExceptionThrown(this, e);
		}
		void _cache_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			RaiseInconsistencyDetected(e);
		}
		protected virtual void RaiseInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			if(InconsistencyDetected == null)
				return;
			InconsistencyDetected(this, e);
		}
		protected virtual void SoftReset() {
			if(_cache != null) {
				_cache.CanResetCache();
				_cache = null;
			}
		}
		public virtual void Refresh() {
			SoftReset();
		}
		protected abstract ServerModeCache CreateCacheCore();
		public virtual CriteriaOperator ExtractExpression(CriteriaOperator d) {
			if(ForceCaseInsensitiveForAnySource)
				return StringsTolowerCloningHelper.Process(d);
			else
				return CriteriaOperator.Clone(d);
		}
		ServerModeOrderDescriptor[] Convert(ICollection<ServerModeOrderDescriptor> original, int groupCount) {
			if(original == null)
				original = new ServerModeOrderDescriptor[0];
			List<ServerModeOrderDescriptor> rv = new List<ServerModeOrderDescriptor>();
			Dictionary<CriteriaOperator, ServerModeOrderDescriptor> uniquer = new Dictionary<CriteriaOperator, ServerModeOrderDescriptor>();
			foreach(ServerModeOrderDescriptor descr in original) {
				ServerModeOrderDescriptor converted = new ServerModeOrderDescriptor(ExtractExpression(descr.SortExpression), descr.IsDesc, ExtractExpression(descr.AuxExpression));
				rv.Add(converted);
				uniquer[converted.SortExpression] = converted;
			}
			if(rv.Count == groupCount) {
				ServerModeOrderDescriptor[] additionalSortings = GetSortingDescriptors(DefaultSorting);
				foreach(ServerModeOrderDescriptor additionalSorting in additionalSortings) {
					if(uniquer.ContainsKey(additionalSorting.SortExpression))
						continue;
					rv.Add(additionalSorting);
					uniquer.Add(additionalSorting.SortExpression, additionalSorting);
				}
			}
			foreach(CriteriaOperator subKey in KeysCriteria) {
				if(!uniquer.ContainsKey(subKey)) {
					bool keyDirection = rv.Count == 0 ? false : rv[rv.Count - 1].IsDesc;
					ServerModeOrderDescriptor keyDescriptor = new ServerModeOrderDescriptor(subKey, keyDirection);
					rv.Add(keyDescriptor);
				}
			}
			return rv.ToArray();
		}
		public static ServerModeOrderDescriptor[] GetSortingDescriptors(string sortingsString) {
			if(string.IsNullOrEmpty(sortingsString))
				return new ServerModeOrderDescriptor[0];
			CriteriaOperator[] additionalSortingsSrc;
			try {
				OperandValue[] dummy;
				additionalSortingsSrc = DevExpress.Data.Filtering.Helpers.CriteriaParser.ParseList(sortingsString, out dummy, true);
			} catch {
				return new ServerModeOrderDescriptor[0];
			}
			List<ServerModeOrderDescriptor> additionalSortings = new List<ServerModeOrderDescriptor>();
			foreach(CriteriaOperator op in additionalSortingsSrc) {
				additionalSortings.Add(ExtractSorting(op));
			}
			return additionalSortings.ToArray();
		}
		internal const string OrderDescToken = "DevExpress;MAGIK const;ORDER DESCENDING";
		static ServerModeOrderDescriptor ExtractSorting(CriteriaOperator op) {
			FunctionOperator fop = op as FunctionOperator;
			if(!ReferenceEquals(fop, null)) {
				if(fop.OperatorType == FunctionOperatorType.Custom) {
					OperandValue ov = fop.Operands[0] as OperandValue;
					if(!ReferenceEquals(ov, null)) {
						string name = ov.Value as string;
						if(name == OrderDescToken) {
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
		public static bool AreEquals(ServerModeOrderDescriptor[] a, ServerModeOrderDescriptor[] b) {
			if(a.Length != b.Length)
				return false;
			for(int i = 0; i < a.Length; ++i) {
				ServerModeOrderDescriptor ai = a[i];
				ServerModeOrderDescriptor bi = b[i];
				if(ai.IsDesc != bi.IsDesc)
					return false;
				if(!Equals(ai.SortExpression, bi.SortExpression))
					return false;
			}
			return true;
		}
		public static bool AreEquals(ServerModeSummaryDescriptor[] a, ServerModeSummaryDescriptor[] b) {
			if(a.Length != b.Length)
				return false;
			for(int i = 0; i < a.Length; ++i) {
				ServerModeSummaryDescriptor ai = a[i];
				ServerModeSummaryDescriptor bi = b[i];
				if(ai.SummaryType != bi.SummaryType)
					return false;
				if(!Equals(ai.SummaryExpression, bi.SummaryExpression))
					return false;
			}
			return true;
		}
		public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			CriteriaOperator convertedCriteria = ExtractExpression(filterCriteria);
			ServerModeOrderDescriptor[] convertedSortInfo = Convert(sortInfo, groupCount);
			ServerModeSummaryDescriptor[] convertedSummary = Convert(summaryInfo);
			ServerModeSummaryDescriptor[] convertedTotalSummary = Convert(totalSummaryInfo);
			bool sameFilter = Equals(FilterClause, convertedCriteria);
			if(this.GroupCount == groupCount
				&& sameFilter
				&& AreEquals(this.SortInfo, convertedSortInfo)
				&& AreEquals(this.SummaryInfo, convertedSummary)
				&& AreEquals(this.TotalSummaryInfo, convertedTotalSummary))
				return;
			if(!sameFilter)
				this.FilterClause = convertedCriteria;
			this.SortInfo = convertedSortInfo;
			this.GroupCount = groupCount;
			this.SummaryInfo = convertedSummary;
			this.TotalSummaryInfo = convertedTotalSummary;
			RefreshCacheOnApply(sameFilter);
		}
		void RefreshCacheOnApply(bool sameFilter) {
			if(_cache == null)
				return;
			if(!sameFilter) {
				SoftReset();
				return;
			}
			ServerModeCache oldCache = _cache;
			SoftReset();
			ServerModeCache newCache = CreateCache();
			newCache.FillFromOldCacheWhateverMakesSence(oldCache);
			if(_cache != null)
				throw new InvalidOperationException("internal error, _cache should be null at that point");
			_cache = newCache;
			ApplyHints();
		}
		public int Count {
			get { return Cache.Count(); }
		}
		public bool Contains(object value) {
			return Cache.Contains(value);
		}
		public int IndexOf(object value) {
			return Cache.IndexOf(value);
		}
		public int GetRowIndexByKey(object key) {
			return Cache.GetRowIndexByKey(key);
		}
		public object GetRowKey(int index) {
			return Cache.GetRowKey(index);
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Cache.GetGroupInfo(parentGroup);
		}
		public List<object> GetTotalSummary() {
			return Cache.GetTotalSummary();
		}
		public object this[int index] {
			get {
				return Cache.Indexer(index);
			}
			set {
				throw new NotSupportedException();
			}
		}
		protected object FindFirstKeyByCriteriaOperator(CriteriaOperator filterCriteria, bool isReversed) {
			return Cache.FindFirstKeyByCriteriaOperator(filterCriteria, isReversed);
		}
		protected abstract object EvaluateOnInstance(object row, CriteriaOperator criteriaOperator);
		protected abstract bool EvaluateOnInstanceLogical(object startRow, CriteriaOperator filter);
		static readonly CriteriaOperator FalseMarker = new OperandValue(false);
		CriteriaOperator LimitSearchByRow(object startRow, bool searchUp, bool ignoreStartRow) {
			CriteriaOperator rv = null;
			for(int i = this.SortInfo.Length - 1; i >= 0; --i) {
				ServerModeOrderDescriptor ord = this.SortInfo[i];
				object val = EvaluateOnInstance(startRow, ord.SortExpression);
				bool isDesc = searchUp ? !ord.IsDesc : ord.IsDesc;
				CriteriaOperator rowExpr = ord.SortExpression;
				if(!ReferenceEquals(val, null)) {
					if(ReferenceEquals(rv, null)) {
						if(ignoreStartRow) {
							if(isDesc)
								rv = rowExpr < new OperandValue(val) | rowExpr.IsNull();
							else
								rv = rowExpr > new OperandValue(val);
						} else {
							if(isDesc)
								rv = rowExpr <= new OperandValue(val) | rowExpr.IsNull();
							else
								rv = rowExpr >= new OperandValue(val);
						}
					} else {
						if(!ReferenceEquals(FalseMarker, rv)) {
							if(isDesc)
								rv = rowExpr < new OperandValue(val) | rowExpr.IsNull() | rowExpr == new OperandValue(val) & rv;
							else
								rv = rowExpr > new OperandValue(val) | rowExpr == new OperandValue(val) & rv;
						} else {
							if(isDesc)
								rv = rowExpr < new OperandValue(val) | rowExpr.IsNull();
							else
								rv = rowExpr > new OperandValue(val);
						}
					}
				} else {
					if(ReferenceEquals(rv, null)) {
						if(ignoreStartRow) {
							if(isDesc)
								rv = FalseMarker;
							else
								rv = rowExpr.IsNotNull();
						} else {
							if(isDesc)
								rv = rowExpr.IsNull();
							else
								rv = null;
						}
					} else {
						if(!ReferenceEquals(FalseMarker, rv)) {
							if(isDesc)
								rv = rowExpr.IsNull() & rv;
							else
								rv = rowExpr.IsNotNull() | rowExpr.IsNull() & rv;
						} else {
							if(isDesc)
								rv = FalseMarker;
							else
								rv = rowExpr.IsNotNull();
						}
					}
				}
			}
			return rv;
		}
		public int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			int rv = FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow);
			if(rv < 0 && allowLoop && startIndex >= 0)
				rv = FindIncremental(expression, value, -1, searchUp, ignoreStartRow);
			return rv;
		}
		bool? useToLower;
		int FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow) {
			try {
				if(value == null)
					return -1;
				CriteriaOperator columnExpression = ExtractExpression(expression);
				CriteriaOperator filter = FilterClause;
				string lowerValue = value.ToLower();
				if(startIndex >= 0) {
					object startRow = this[startIndex];
					if(startRow == null)
						return -1;
					if(!ignoreStartRow) {
						object currentValue = EvaluateOnInstance(startRow, columnExpression);
						if(currentValue != null) {
							if(currentValue.ToString().ToLower().StartsWith(lowerValue))
								return startIndex;
						}
					}
					CriteriaOperator limit = LimitSearchByRow(startRow, searchUp, ignoreStartRow);
					if(ReferenceEquals(FalseMarker, limit))
						return -1;
					filter &= limit;
				}
				bool insensitiveOverride = lowerValue == lowerValue.ToUpper();
				object simpleKey = null;
				if(insensitiveOverride || useToLower != true) {
					CriteriaOperator simpleExpression = filter & (new FunctionOperator(FunctionOperatorType.StartsWith, columnExpression, lowerValue));
					simpleKey = FindFirstKeyByCriteriaOperator(simpleExpression, searchUp);
					int simpleIndex = GetRowIndexByKey(simpleKey);
					if(insensitiveOverride || useToLower == false) {
						return simpleIndex;
					}
					if(simpleIndex >= 0) {
						object sensitiveRow = this[simpleIndex];
						if(sensitiveRow != null) {
							object sensitiveValue = EvaluateOnInstance(sensitiveRow, columnExpression);
							if(sensitiveValue != null) {
								if(sensitiveValue.ToString().StartsWithInvariantCultureIgnoreCase(lowerValue) && !sensitiveValue.ToString().StartsWithInvariantCulture(lowerValue)) {
									useToLower = false;
									return simpleIndex;
								}
							}
							CriteriaOperator limit = LimitSearchByRow(sensitiveRow, !searchUp, false);
							if(ReferenceEquals(FalseMarker, limit))
								return -1;
							filter &= limit;
						}
					}
				}
				CriteriaOperator lowerExpr = filter & (new FunctionOperator(FunctionOperatorType.StartsWith, new FunctionOperator(FunctionOperatorType.Lower, columnExpression), lowerValue));
				object lowerKey = FindFirstKeyByCriteriaOperator(lowerExpr, searchUp);
				if(!useToLower.HasValue && !Equals(simpleKey, lowerKey))
					useToLower = true;
				return ValidateIncrementalSearchRowIndex(GetRowIndexByKey(lowerKey), startIndex, searchUp);
			} catch {
				return -1;
			}
		}
		static int ValidateIncrementalSearchRowIndex(int rowFound, int startRow, bool searchUp) {
			if(startRow < 0)
				return rowFound;
			if(rowFound < 0)
				return rowFound;
			if(rowFound <= startRow && searchUp)
				return rowFound;
			if(rowFound >= startRow && !searchUp)
				return rowFound;
			return -1;
		}
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			try {
				CriteriaOperator filter = FilterClause;
				CriteriaOperator columnExpression = ExtractExpression(expression);
				if(this.KeysCriteria.Length == 1 && Equals(this.KeysCriteria[0], columnExpression))
					return GetRowIndexByKey(value);
				CriteriaOperator columnFilter;
				if(value == null || value is DBNull)
					columnFilter = columnExpression.IsNull();
				else
					columnFilter = columnExpression == new OperandValue(value);
				if(startIndex >= 0) {
					object startRow = this[startIndex];
					if(EvaluateOnInstanceLogical(startRow, columnFilter))
						return startIndex;
					CriteriaOperator limit = LimitSearchByRow(startRow, searchUp, false);
					if(ReferenceEquals(FalseMarker, limit))
						return -1;
					filter &= limit;
				}
				object simpleKey = null;
				CriteriaOperator simpleExpression = filter & columnFilter;
				simpleKey = FindFirstKeyByCriteriaOperator(simpleExpression, searchUp);
				return ValidateIncrementalSearchRowIndex(GetRowIndexByKey(simpleKey), startIndex, searchUp);
			} catch {
				return -1;
			}
		}
		class GetUniqueColumnValuesCacheKey {
			public GetUniqueColumnValuesCacheKey(CriteriaOperator columnExpression, int maxCount, bool includeFilteredOut) {
				this.ColumnExpression = columnExpression;
				this.MaxCount = maxCount;
				this.IncludeFilteredOut = includeFilteredOut;
			}
			public readonly CriteriaOperator ColumnExpression;
			public readonly int MaxCount;
			public readonly bool IncludeFilteredOut;
			public override int GetHashCode() {
				return ReferenceEquals(null, ColumnExpression) ? 0 : ColumnExpression.GetHashCode();
			}
			public override bool Equals(object obj) {
				GetUniqueColumnValuesCacheKey another = obj as GetUniqueColumnValuesCacheKey;
				if(another == null)
					return false;
				return this.IncludeFilteredOut == another.IncludeFilteredOut && this.MaxCount == another.MaxCount && Equals(this.ColumnExpression, another.ColumnExpression);
			}
		}
		public object[] GetUniqueColumnValues(CriteriaOperator columnExpression, int maxCount, bool includeFilteredOut) {
			GetUniqueColumnValuesCacheKey cacheKey = new GetUniqueColumnValuesCacheKey(columnExpression, maxCount, includeFilteredOut);
			object[] cachedResult;
			object cachedValue;
			if(Cache.SomethingCache.TryGetValue(cacheKey, out cachedValue)) {
				cachedResult = (object[])cachedValue;
			} else {
				CriteriaOperator expression = ExtractExpression(columnExpression);
				CriteriaOperator filter = includeFilteredOut ? null : FilterClause;
				cachedResult = GetUniqueValues(expression, maxCount, filter);
				Cache.SomethingCache.Add(cacheKey, cachedResult);
			}
			return (object[])cachedResult.Clone();
		}
		protected abstract object[] GetUniqueValues(CriteriaOperator expression, int maxCount, CriteriaOperator filter);
		public abstract IList GetAllFilteredAndSortedRows();
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Cache.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public bool IsFixedSize {
			get { return true; }
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public IEnumerator GetEnumerator() {
			throw new NotSupportedException("Specified method is not supported in server mode. Make certain that the bound control supports this mode.");
		}
		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		int _hintGridIsPagedPageSize;
		void IListServerHints.HintGridIsPaged(int pageSize) {
			_hintGridIsPagedPageSize = pageSize;
			ApplyHints();
		}
		int _hintMaxVisibleRowsInGrid;
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			_hintMaxVisibleRowsInGrid = rowsInGrid;
			ApplyHints();
		}
		void ApplyHints() {
			IListServerHints hints = _cache as IListServerHints;
			if(hints == null)
				return;
			if(_hintGridIsPagedPageSize > 0)
				hints.HintGridIsPaged(_hintGridIsPagedPageSize);
			if(_hintMaxVisibleRowsInGrid > 0)
				hints.HintMaxVisibleRowsInGrid(_hintMaxVisibleRowsInGrid);
		}
	}
	public sealed class ServerModeCompoundKey {
		public readonly object[] SubKeys;
		public ServerModeCompoundKey(object[] subKeys) {
			this.SubKeys = subKeys;
		}
		public override int GetHashCode() {
			int hash = 0;
			foreach(object o in SubKeys) {
				if(o == null)
					continue;
				hash ^= o.GetHashCode();
			}
			return hash;
		}
		public override bool Equals(object obj) {
			ServerModeCompoundKey another = obj as ServerModeCompoundKey;
			if(another == null)
				return false;
			if(this.SubKeys.Length != another.SubKeys.Length)
				return false;
			for(int i = 0; i < SubKeys.Length; ++i) {
				if(!Equals(this.SubKeys[i], another.SubKeys[i]))
					return false;
			}
			return true;
		}
	}
	public class StringsTolowerCloningHelper: IClientCriteriaVisitor<CriteriaOperator> {
		static readonly StringsTolowerCloningHelper Instance = new StringsTolowerCloningHelper();
		StringsTolowerCloningHelper() { }
		public CriteriaOperator Visit(AggregateOperand theOperand) {
			return new AggregateOperand((OperandProperty)Process(theOperand.CollectionProperty), Process(theOperand.AggregatedExpression), theOperand.AggregateType, Process(theOperand.Condition));
		}
		public CriteriaOperator Visit(OperandProperty theOperand) {
			return CriteriaOperator.Clone(theOperand);
		}
		public CriteriaOperator Visit(JoinOperand theOperand) {
			return new JoinOperand(theOperand.JoinTypeName, Process(theOperand.Condition), theOperand.AggregateType, Process(theOperand.AggregatedExpression));
		}
		public CriteriaOperator Visit(BetweenOperator theOperator) {
			return new BetweenOperator(Process(theOperator.TestExpression), Process(theOperator.BeginExpression), Process(theOperator.EndExpression));
		}
		public CriteriaOperator Visit(BinaryOperator theOperator) {
			CriteriaOperator left = Process(theOperator.LeftOperand);
			CriteriaOperator right = Process(theOperator.RightOperand);
#pragma warning disable 618
			if(theOperator.OperatorType == BinaryOperatorType.Like) {
#pragma warning restore 618
				if(!ReferenceEquals(left, null)) {
					FunctionOperator fl = left as FunctionOperator;
					if(ReferenceEquals(fl, null) || fl.OperatorType != FunctionOperatorType.Lower)
						left = new FunctionOperator(FunctionOperatorType.Lower, left);
				}
				if(!ReferenceEquals(right, null)) {
					FunctionOperator fr = right as FunctionOperator;
					if(ReferenceEquals(fr, null) || fr.OperatorType != FunctionOperatorType.Lower)
						right = new FunctionOperator(FunctionOperatorType.Lower, right);
				}
				return DevExpress.Data.Filtering.Helpers.LikeCustomFunction.Create(left, right);
			} else {
				return new BinaryOperator(left, right, theOperator.OperatorType);
			}
		}
		public CriteriaOperator Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		public CriteriaOperator Visit(InOperator theOperator) {
			return new InOperator(Process(theOperator.LeftOperand), theOperator.Operands.Select(o => Process(o)));
		}
		public CriteriaOperator Visit(GroupOperator theOperator) {
			return GroupOperator.Combine(theOperator.OperatorType, theOperator.Operands.Select(o => Process(o)));
		}
		public CriteriaOperator Visit(OperandValue theOperand) {
			return CriteriaOperator.Clone(theOperand);
		}
		public CriteriaOperator Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					List<CriteriaOperator> ops = new List<CriteriaOperator>(theOperator.Operands.Count);
					foreach(CriteriaOperator op in theOperator.Operands) {
						CriteriaOperator processed = Process(op);
						if(!ReferenceEquals(processed, null)) {
							FunctionOperator fp = processed as FunctionOperator;
							if(ReferenceEquals(fp, null) || fp.OperatorType != FunctionOperatorType.Lower)
								processed = new FunctionOperator(FunctionOperatorType.Lower, processed);
						}
						ops.Add(processed);
					}
					return new FunctionOperator(theOperator.OperatorType, ops);
				case FunctionOperatorType.Custom:
					if(DevExpress.Data.Filtering.Helpers.LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator))
						return Process(DevExpress.Data.Filtering.Helpers.LikeCustomFunction.Convert(theOperator));
					break;
			}
			return new FunctionOperator(theOperator.OperatorType, theOperator.Operands.Select(o => Process(o)));
		}
		public static CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			else
				return op.Accept(Instance);
		}
	}
	public static class InconsistentHelper {
		public delegate void MyMethodInvoker();
		class PostState {
			public bool ShouldFailWithException;
			public MyMethodInvoker RefreshMethod;
			public MyMethodInvoker FailMethod;
		}
		public static void PostpronedInconsistent(MyMethodInvoker refreshMethod, MyMethodInvoker failMethod) {
			if(refreshMethod == null)
				return;
			SynchronizationContext context = SynchronizationContext.Current;
			if(IsGoodContext(context)) {
				PostState state = new PostState();
				state.ShouldFailWithException = true;
				state.RefreshMethod = refreshMethod;
				state.FailMethod = failMethod;
				context.Post(DoPostpronedReload, state);
				state.ShouldFailWithException = false;
			} else {
				FailUnderAspOrAnotherNonPostEnvironment(failMethod);
			}
		}
		static void DoPostpronedReload(object state) {
			PostState postedState = (PostState)state;
			if(postedState.ShouldFailWithException)
				FailUnderAspOrAnotherNonPostEnvironment(postedState.FailMethod);
			else
				postedState.RefreshMethod();
		}
		static void FailUnderAspOrAnotherNonPostEnvironment(MyMethodInvoker failMethod) {
			if(failMethod == null) {
				try {
					throw new InvalidOperationException("Failed to post refresh command");
				} catch { }
				return;
			}
			failMethod();
		}
		static  bool IsGoodContext(SynchronizationContext context) {
			if(context == null)
				return false;
			return true;
		}
	}
}
