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
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Linq;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.Data.Async {
	public interface IAsyncListServer {
		CommandGetTotals GetTotals(params DictionaryEntry[] tags);
		CommandGetRow GetRow(int index, params DictionaryEntry[] tags);
		CommandGetGroupInfo GetGroupInfo(ListSourceGroupInfo parentGroup, params DictionaryEntry[] tags);
		CommandGetRowIndexByKey GetRowIndexByKey(object key, params DictionaryEntry[] tags);
		CommandGetUniqueColumnValues GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut, params DictionaryEntry[] tags);
		CommandFindIncremental FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop, params DictionaryEntry[] tags);
		CommandLocateByValue LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp, params DictionaryEntry[] tags);
		CommandApply Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, params DictionaryEntry[] tags);
		CommandRefresh Refresh(params DictionaryEntry[] tags);
		CommandGetAllFilteredAndSortedRows GetAllFilteredAndSortedRows(params DictionaryEntry[] tags);
		CommandPrefetchRows PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, params DictionaryEntry[] tags);
		void Cancel(Command command);
		void Cancel<T>() where T: Command;
		void WeakCancel<T>() where T: Command;
		void SetReceiver(IAsyncResultReceiver receiver);
		T PullNext<T>() where T: Command;
		bool WaitFor(Command command);
	}
	public interface IAsyncCommandVisitor {
		void Canceled(Command command);
		void Visit(CommandGetTotals command);
		void Visit(CommandGetRow command);
		void Visit(CommandApply command);
		void Visit(CommandRefresh command);
		void Visit(CommandGetRowIndexByKey command);
		void Visit(CommandGetGroupInfo command);
		void Visit(CommandGetUniqueColumnValues command);
		void Visit(CommandFindIncremental command);
		void Visit(CommandLocateByValue command);
		void Visit(CommandGetAllFilteredAndSortedRows command);
		void Visit(CommandPrefetchRows command);
	}
	public interface IAsyncResultReceiver: IAsyncCommandVisitor {
		void Notification(NotificationInconsistencyDetected notification);
		void Notification(NotificationExceptionThrown exception);
		void BusyChanged(bool busy);
		void Refreshing(CommandRefresh refreshCommand);
		void PropertyDescriptorsRenewed();
	}
	abstract public class Command {
		public abstract void Accept(IAsyncCommandVisitor visitor);
		volatile bool _canceled = false;
		readonly Dictionary<object, object> _tags;
		public bool IsCanceled { get { return _canceled; } }
		public virtual void Cancel() { _canceled = true; }
		public void Cancel(Exception exception) {
			_exception = exception;
			Cancel();
		}
		Exception _exception;
		public Exception Exception { get { return _exception; } }
		protected Command(DictionaryEntry[] tags) {
			if(tags != null && tags.Length > 0) {
				_tags = new Dictionary<object, object>(tags.Length);
				foreach(var tag in tags)
					_tags.Add(tag.Key, tag.Value);
			}
		}
		public bool TryGetTag<T>(object token, out T tag) {
			if(_tags == null) {
				tag = default(T);
				return false;
			}
			object otag;
			if(_tags.TryGetValue(token, out otag)) {
				tag = (T)otag;
				return true;
			} else {
				tag = default(T);
				return false;
			}
		}
		bool _IsResultDispatched;
		public bool IsResultDispatched { get { return _IsResultDispatched; } }
		public void MarkResultDispatched() { _IsResultDispatched = true; }
	}
	public class CommandGetTotals: Command {
		public int Count;
		public List<object> TotalSummary;
		public CommandGetTotals(params DictionaryEntry[] tags)
			: base(tags) {
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandRefresh: Command {
		public CommandRefresh(params DictionaryEntry[] tags)
			: base(tags) {
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandGetGroupInfo: Command {
		public ListSourceGroupInfo ParentGroup;
		public List<ListSourceGroupInfo> ChildrenGroups;
		public CommandGetGroupInfo(ListSourceGroupInfo parentGroup, params DictionaryEntry[] tags)
			: base(tags) {
			ParentGroup = parentGroup;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandGetUniqueColumnValues: Command {
		public CriteriaOperator Expression;
		public int MaxCount;
		public bool IncludeFilteredOut;
		public object Values;
		public CommandGetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut, params DictionaryEntry[] tags)
			: base(tags) {
			Expression = expression;
			MaxCount = maxCount;
			IncludeFilteredOut = includeFilteredOut;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandGetAllFilteredAndSortedRows: Command {
		public IList RowsInfo;
		public IList Rows;
		public CommandGetAllFilteredAndSortedRows(params DictionaryEntry[] tags) : base(tags) { }
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandPrefetchRows: Command {
		public ListSourceGroupInfo[] GroupsToPrefetch;
		public bool Successful;
		public CommandPrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, params DictionaryEntry[] tags)
			: base(tags) {
			this.GroupsToPrefetch = groupsToPrefetch;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
		internal CancellationTokenSource CancellationTokenSource;
		public override void Cancel() {
			base.Cancel();
			lock(this) {
				var cancellationTokenSource = this.CancellationTokenSource;
				if(cancellationTokenSource != null)
					cancellationTokenSource.Cancel();
			}
		}
	}
	public class CommandFindIncremental: Command {
		public CriteriaOperator Expression;
		public string Value;
		public int StartIndex;
		public bool SearchUp;
		public bool IgnoreStartRow;
		public bool AllowLoop;
		public int RowIndex;
		public CommandFindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop, params DictionaryEntry[] tags)
			: base(tags) {
			Expression = expression;
			Value = value;
			StartIndex = startIndex;
			SearchUp = searchUp;
			IgnoreStartRow = ignoreStartRow;
			AllowLoop = allowLoop;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandLocateByValue: Command {
		public CriteriaOperator Expression;
		public object Value;
		public int StartIndex;
		public bool SearchUp;
		public int RowIndex;
		public CommandLocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp, params DictionaryEntry[] tags)
			: base(tags) {
			Expression = expression;
			Value = value;
			StartIndex = startIndex;
			SearchUp = searchUp;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandGetRow: Command {
		public readonly int Index;
		public object RowKey;
		public object RowInfo;
		public object Row;
		public CommandGetRow(int index, params DictionaryEntry[] tags)
			: base(tags) {
			Index = index;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandGetRowIndexByKey: Command {
		public object Key;
		public int Index;
		public List<CommandGetGroupInfo> Groups;
		public CommandGetRowIndexByKey(object key, params DictionaryEntry[] tags)
			: base(tags) {
			Key = key;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class CommandApply: Command {
		public CriteriaOperator FilterCriteria;
		public ICollection<ServerModeOrderDescriptor> SortInfo;
		public int GroupCount;
		public ICollection<ServerModeSummaryDescriptor> GroupSummaryInfo;
		public ICollection<ServerModeSummaryDescriptor> TotalSummaryInfo;
		public CommandApply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, params DictionaryEntry[] tags)
			: base(tags) {
			FilterCriteria = filterCriteria;
			SortInfo = sortInfo;
			GroupCount = groupCount;
			GroupSummaryInfo = summaryInfo;
			TotalSummaryInfo = totalSummaryInfo;
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class NotificationInconsistencyDetected: Command {
		public bool Handled;
		Exception notification;
		public NotificationInconsistencyDetected(Exception notificationMessage, params DictionaryEntry[] tags)
			: base(tags) {
			notification = notificationMessage;
		}
		public Exception Notification {
			get { return notification; }
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			((IAsyncResultReceiver)visitor).Notification(this);
		}
	}
	public class NotificationExceptionThrown: Command {
		Exception exception;
		public NotificationExceptionThrown(Exception exceptionMessage, params DictionaryEntry[] tags)
			: base(tags) {
			exception = exceptionMessage;
		}
		public Exception Notification {
			get { return exception; }
		}
		public override void Accept(IAsyncCommandVisitor visitor) {
			((IAsyncResultReceiver)visitor).Notification(this);
		}
	}
}
namespace DevExpress.Data.Async.Helpers {
	using Compatibility.System.ComponentModel;
	using DevExpress.Data.Async;
	using DevExpress.Data.Helpers;
	public class AsyncListServerCore: IAsyncListServer, IListServerHints, IDisposable, ITypedList, IDXCloneable {
		bool isDisposed;
		public readonly SynchronizationContext SynchronizationContext;
		public EventHandler<ListServerGetOrFreeEventArgs> ListServerGet, ListServerFree;
		public EventHandler<GetTypeInfoEventArgs> GetTypeInfo;
		public EventHandler<GetWorkerThreadRowInfoEventArgs> GetWorkerThreadRowInfo;
		public EventHandler<GetPropertyDescriptorsEventArgs> GetPropertyDescriptors;
		public EventHandler<GetUIThreadRowEventArgs> GetUIThreadRow;
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual AsyncListServerCore DXClone() {
			AsyncListServerCore clone = CreateDXClone();
			clone.isDisposed = this.isDisposed;
			clone.ListServerGet = this.ListServerGet;
			clone.ListServerFree = this.ListServerFree;
			clone.GetTypeInfo = this.GetTypeInfo;
			clone.GetWorkerThreadRowInfo = this.GetWorkerThreadRowInfo;
			clone.GetPropertyDescriptors = this.GetPropertyDescriptors;
			clone.GetUIThreadRow = this.GetUIThreadRow;
			return clone;
		}
		protected virtual AsyncListServerCore CreateDXClone() {
			return new AsyncListServerCore(SynchronizationContext);
		}
		IAsyncResultReceiver ResultsReceiver;
		CommandQueue _Worker;
		CommandQueue Worker {
			get {
				if(isDisposed)
					throw new ObjectDisposedException(this.GetType().FullName);
				if(ResultsReceiver == null)
					throw new InvalidOperationException("SetReceiver should be called before performing any operation on the IAsyncListServer interface. This component should not be used by the controls that do not support Instant Feedback(tm) binding mode.");
				if(_Worker == null) {
					_Worker = CreateCommandQueue(SynchronizationContext, somethingInTheOutputQueueCallback, ListServerGet, ListServerFree, GetTypeInfo, GetWorkerThreadRowInfo);
				}
				return _Worker;
			}
		}
		protected virtual CommandQueue CreateCommandQueue(SynchronizationContext context, SendOrPostCallback somethingInTheOutputQueueCallback, EventHandler<ListServerGetOrFreeEventArgs> listServerGet, EventHandler<ListServerGetOrFreeEventArgs> listServerFree, EventHandler<GetTypeInfoEventArgs> getTypeInfo, EventHandler<GetWorkerThreadRowInfoEventArgs> getWorkerThreadRowInfo) {
			return new CommandQueue(context, somethingInTheOutputQueueCallback, listServerGet, listServerFree, getTypeInfo, getWorkerThreadRowInfo);
		}
		public void SetReceiver(IAsyncResultReceiver receiver) {
			if(this.ResultsReceiver == receiver)
				return;
			if(this.ResultsReceiver != null && receiver != null)
				throw new InvalidOperationException("Multiple receivers set. Instant Feedback source should not be shared between different controls.");
			if(receiver == null)
				ShutDown();
			else
				this.ResultsReceiver = receiver;
		}
		void ShutDown() {
			if(_Worker != null) {
				lock(Worker.SyncRoot) {
					Worker.CancelAll();
					Worker.AskForTermination();
				}
				Worker.MessageWaiter.Set();
				ResultsReceiver = null;
				_Worker = null;
				this.isBusy = false;
			}
		}
		public AsyncListServerCore(SynchronizationContext context) {
			this.SynchronizationContext = context ?? SynchronizationContext.Current;
			if(this.SynchronizationContext == null)
				throw new ArgumentNullException("SynchronizationContext.Current");
		}
		public AsyncListServerCore() : this(null) { }
		public AsyncListServerCore(SynchronizationContext context, EventHandler<ListServerGetOrFreeEventArgs> listServerGet) : this(context) {
			this.ListServerGet = listServerGet;
		}
		public AsyncListServerCore(SynchronizationContext context, IAsyncResultReceiver resultsReceiver, EventHandler<ListServerGetOrFreeEventArgs> listServerCreation)
			: this(context, listServerCreation) {
			SetReceiver(resultsReceiver);
		}
		void somethingInTheOutputQueueCallback(object arg) {
			if(_Worker == null)
				return;
			CheckDescriptorsRenew();
			DispatchOutputQueue();
		}
		void CheckDescriptorsRenew() {
			if(Worker.PropertyDescriptorsNeedReset) {
				if(DevExpress.Data.Utils.Helpers.WaitOne(Worker.TypeInfoObtained, 0)) {
					Worker.PropertyDescriptorsNeedReset = false;
					Worker.PropertyDescriptors = null;
					ResultsReceiver.PropertyDescriptorsRenewed();
				}
			}
		}
		void DispatchOutputQueue() {
			for(; ; ) {
				Command result;
				lock(Worker.SyncRoot) {
					if(Worker.CountOutputCommand == 0) {
						Worker.AskForPosts();
						break;
					}
					result = Worker.OutputDequeue();
				}
				DoBeforeDispatch(result);
				try {
					if(result.IsCanceled)
						ResultsReceiver.Canceled(result);
					else
						result.Accept(ResultsReceiver);
				} catch { }
				DoAfterDispatch(result);
			}
			bool isSomethingInQueue;
			lock(Worker.SyncRoot) {
				isSomethingInQueue = Worker.IsSomethingInQueue;
			}
			ProcessNewBusy(isSomethingInQueue);
		}
		bool IsDoAfterDispatchRequired(Command nextCommand) {
			return nextCommand is NotificationInconsistencyDetected;
		}
		private void DoAfterDispatch(Command result) {
			if(!result.IsCanceled) {
				NotificationInconsistencyDetected inconsistent = result as NotificationInconsistencyDetected;
				if(inconsistent != null) {
					if(!inconsistent.Handled) {
						inconsistent.Handled = true;
						Refresh();
					}
				}
			}
		}
		class GetAllFilteredAndSortedRowsResult: List<object>, ITypedList {
			readonly ITypedList TypedListInfoSource;
			public GetAllFilteredAndSortedRowsResult(int count, ITypedList typedListInfoSource)
				: base(count) {
				this.TypedListInfoSource = typedListInfoSource;
			}
			PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
				return this.TypedListInfoSource.GetItemProperties(listAccessors);
			}
			string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
				return this.TypedListInfoSource.GetListName(listAccessors);
			}
		}
		void DoBeforeDispatch(Command result) {
			CommandGetRow rowCommand = result as CommandGetRow;
			if(rowCommand != null) {
				rowCommand.Row = ExtractUIRowFromRowInfo(rowCommand.RowInfo);
			}
			CommandGetAllFilteredAndSortedRows allRowsCommand = result as CommandGetAllFilteredAndSortedRows;
			if(allRowsCommand != null && allRowsCommand.RowsInfo != null) {
				List<object> rows = new GetAllFilteredAndSortedRowsResult(allRowsCommand.RowsInfo.Count, this);
				foreach(object rowInfo in allRowsCommand.RowsInfo) {
					rows.Add(ExtractUIRowFromRowInfo(rowInfo));
				}
				allRowsCommand.Rows = rows;
			}
			result.MarkResultDispatched();
		}
		object ExtractUIRowFromRowInfo(object rowInfo) {
			if(GetUIThreadRow == null)
				return rowInfo;
			else {
				GetUIThreadRowEventArgs args = new GetUIThreadRowEventArgs(Worker.TypeInfo, Worker.PropertyDescriptors, rowInfo);
				try {
					GetUIThreadRow(this, args);
				} catch { }
				return args.UIThreadRow;
			}
		}
		public T PullNext<T>() where T: Command {
			T rv;
			lock(Worker.SyncRoot) {
				Command nextCommand = Worker.PeekOutput();
				if(!(nextCommand is T))
					return null;
				if(nextCommand.IsCanceled)
					return null;
				if(IsDoAfterDispatchRequired(nextCommand))
					return null;
				rv = (T)Worker.OutputDequeue();
			}
			DoBeforeDispatch(rv);
			return rv;
		}
		public bool WaitFor(Command command) {
			DispatchOutputQueue();
			if(command == null)
				return true;
			else
				return command.IsResultDispatched;
		}
		bool isBusy;
		void ProcessNewBusy(bool newBusy) {
			if(isBusy == newBusy)
				return;
			isBusy = newBusy;
			ResultsReceiver.BusyChanged(isBusy);
		}
		public virtual void Dispose() {
			if(isDisposed)
				return;
			SetReceiver(null);
			isDisposed = true;
		}
		T PostCommand<T>(T command) where T:Command {
			lock(Worker.SyncRoot) {
				Worker.InputEnqueue(command);
			}
			Worker.MessageWaiter.Set();
			ProcessNewBusy(true);
			return command;
		}
		public CommandGetTotals GetTotals(params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetTotals(tags));
		}
		public CommandGetRow GetRow(int index, params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetRow(index, tags));
		}
		public CommandGetGroupInfo GetGroupInfo(ListSourceGroupInfo parentGroup, params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetGroupInfo(parentGroup, tags));
		}
		public CommandGetRowIndexByKey GetRowIndexByKey(object key, params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetRowIndexByKey(key, tags));
		}
		public CommandGetUniqueColumnValues GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut, params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetUniqueColumnValues(expression, maxCount, includeFilteredOut, tags));
		}
		public CommandFindIncremental FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop, params DictionaryEntry[] tags) {
			return PostCommand(new CommandFindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop, tags));
		}
		public CommandLocateByValue LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp, params DictionaryEntry[] tags) {
			return PostCommand(new CommandLocateByValue(expression, value, startIndex, searchUp, tags));
		}
		public CommandApply Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, params DictionaryEntry[] tags) {
			lock(Worker.SyncRoot) {
				Worker.CancelAllButRefresh();
			}
			return PostCommand(new CommandApply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo, tags));
		}
		public CommandRefresh Refresh(params DictionaryEntry[] tags) {
			lock(Worker.SyncRoot) {
				Worker.CancelAllButApply();
			}
			CommandRefresh command = PostCommand(new CommandRefresh(tags));
			ResultsReceiver.Refreshing(command);
			return command;
		}
		public CommandGetAllFilteredAndSortedRows GetAllFilteredAndSortedRows(params DictionaryEntry[] tags) {
			return PostCommand(new CommandGetAllFilteredAndSortedRows(tags));
		}
		public CommandPrefetchRows PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, params DictionaryEntry[] tags) {
			return PostCommand(new CommandPrefetchRows(groupsToPrefetch, tags));
		}
		public void Cancel(Command command) {
			lock(Worker.SyncRoot) {
				command.Cancel();
			}
		}
		public void Cancel<T>() where T: Command {
			lock(Worker.SyncRoot) {
				Worker.Cancel<T>();
			}
		}
		public void WeakCancel<T>() where T: Command {
			lock(Worker.SyncRoot) {
				Worker.WeakCancel<T>();
			}
		}
#if DEBUG
		public void CatchUpDescriptorsIfReady() {
			CheckDescriptorsRenew();
		}
		public void PumpAll() {
			for(; ; ) {
				DispatchOutputQueue();
				lock(Worker.SyncRoot) {
					if(!Worker.IsSomethingInQueue)
						return;
				}
				Thread.Sleep(1);
			}
		}
#endif
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(Worker.PropertyDescriptors == null) {
				if(DevExpress.Data.Utils.Helpers.WaitOne(Worker.TypeInfoObtained, 100)) {
					if(GetPropertyDescriptors != null) {
						try {
							GetPropertyDescriptorsEventArgs args = new GetPropertyDescriptorsEventArgs(Worker.TypeInfo);
							GetPropertyDescriptors(this, args);
							Worker.PropertyDescriptors = args.PropertyDescriptors;
						} catch { }
					}
					if(Worker.PropertyDescriptors == null) {
						if(Worker.TypeInfo != null)
							Worker.PropertyDescriptors = Worker.TypeInfo as PropertyDescriptorCollection;
						if(Worker.PropertyDescriptors == null)
							Worker.PropertyDescriptors = PropertyDescriptorCollection.Empty;
					}
				} else {
					Worker.PropertyDescriptorsNeedReset = true;
					Worker.PropertyDescriptors = PropertyDescriptorCollection.Empty;
				}
			}
			return Worker.PropertyDescriptors;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		#endregion
		void DevExpress.Data.Helpers.IListServerHints.HintGridIsPaged(int pageSize) {
		}
		void DevExpress.Data.Helpers.IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
		}
	}
	public class ListServerGetOrFreeEventArgs: EventArgs {
		public object ListServerSource;
		public object Tag;
	}
	public class GetTypeInfoEventArgs: EventArgs {
		public readonly object ListServerSource;
		public readonly object Tag;
		public object TypeInfo;
		public GetTypeInfoEventArgs(object listServerSource, object tag) {
			this.ListServerSource = listServerSource;
			this.Tag = tag;
		}
	}
	public class GetPropertyDescriptorsEventArgs: EventArgs {
		public readonly object TypeInfo;
		public PropertyDescriptorCollection PropertyDescriptors;
		public GetPropertyDescriptorsEventArgs(object typeInfo) {
			this.TypeInfo = typeInfo;
		}
	}
	public class GetWorkerThreadRowInfoEventArgs: EventArgs {
		public readonly object TypeInfo;
		public readonly object WorkerThreadRow;
		public object RowInfo;
		public GetWorkerThreadRowInfoEventArgs(object typeInfo, object workerThreadRow) {
			this.TypeInfo = typeInfo;
			this.WorkerThreadRow = workerThreadRow;
		}
	}
	public class GetUIThreadRowEventArgs: EventArgs {
		public readonly object TypeInfo;
		public readonly PropertyDescriptorCollection PropertyDescriptors;
		public readonly object RowInfo;
		public object UIThreadRow;
		public GetUIThreadRowEventArgs(object typeInfo, PropertyDescriptorCollection propertyDescriptors, object rowInfo) {
			this.TypeInfo = typeInfo;
			this.PropertyDescriptors = propertyDescriptors;
			this.RowInfo = rowInfo;
		}
	}
	public class CommandQueue: IAsyncCommandVisitor {
		public readonly ManualResetEvent MessageWaiter = new ManualResetEvent(false);
		readonly Queue<Command> inputQueueNormalPriority = new Queue<Command>();
		readonly Queue<Command> inputQueueLoweredPriority = new Queue<Command>();
		readonly Queue<Command> outQueue = new Queue<Command>();
		protected IListServer ListServer;
		public object TypeInfo;
		public PropertyDescriptorCollection PropertyDescriptors;
		public ManualResetEvent TypeInfoObtained = new ManualResetEvent(false);
		public bool PropertyDescriptorsNeedReset;
		Command CurrentCommand;
		volatile bool skipPosts;
		readonly SynchronizationContext SynchronizationContext;
		readonly SendOrPostCallback SomethingInTheOutputQueueCallback;
		readonly EventHandler<ListServerGetOrFreeEventArgs> ListServerGet, ListServerFree;
		readonly EventHandler<GetTypeInfoEventArgs> GetTypeInfo;
		readonly EventHandler<GetWorkerThreadRowInfoEventArgs> GetWorkerThreadRowInfo;
		public CommandQueue(SynchronizationContext context, SendOrPostCallback somethingInTheOutputQueueCallback, EventHandler<ListServerGetOrFreeEventArgs> listServerGet, EventHandler<ListServerGetOrFreeEventArgs> listServerFree, EventHandler<GetTypeInfoEventArgs> getTypeInfo, EventHandler<GetWorkerThreadRowInfoEventArgs> getWorkerThreadRowInfo) {
			this.SynchronizationContext = context;
			this.SomethingInTheOutputQueueCallback = somethingInTheOutputQueueCallback;
			this.ListServerGet = listServerGet;
			this.ListServerFree = listServerFree;
			this.GetTypeInfo = getTypeInfo;
			this.GetWorkerThreadRowInfo = getWorkerThreadRowInfo;
			Thread t = new Thread(Run);
			t.IsBackground = true;
#if !SL && !DXRESTRICTED
			t.Priority = ThreadPriority.BelowNormal;
#endif
			t.Start();
		}
		public object SyncRoot { get { return this; } }
		public void AskForPosts() {
			skipPosts = false;
		}
		public void InputEnqueue(Command command) {
			if(IsLowPriority(command))
				inputQueueLoweredPriority.Enqueue(command);
			else
				inputQueueNormalPriority.Enqueue(command);
		}
		static readonly object LowPriorityToken = new object();
		static readonly object LowPriorityTag = (object)true;
		public static DictionaryEntry GetLowPriorityTag() {
			return new DictionaryEntry(LowPriorityToken, LowPriorityTag);
		}
		static bool IsLowPriority(Command command) {
			if(command is CommandApply)
				return false;
			if(command is CommandRefresh)
				return false;
			bool lowPriority;
			if(command.TryGetTag<bool>(LowPriorityToken, out lowPriority))
				return lowPriority;
			else
				return false;
		}
		void EstablishNormalPriority() {
			foreach(Command command in inputQueueLoweredPriority)
				inputQueueNormalPriority.Enqueue(command);
			inputQueueLoweredPriority.Clear();
		}
		IEnumerable<Command> GetAllCommands() {
			foreach(Command command in inputQueueNormalPriority) 
				yield return command;
			foreach(Command command in inputQueueLoweredPriority)
				yield return command;
			foreach(Command command in outQueue)
				yield return command;
			if(CurrentCommand != null)
				yield return CurrentCommand;
		}
		void InputDequeue() {
			if(CurrentCommand != null)
				throw new InvalidOperationException("CurrentCommand already fetched");
			if(inputQueueNormalPriority.Count > 0)
				CurrentCommand = inputQueueNormalPriority.Dequeue();
			else if(inputQueueLoweredPriority.Count > 0)
				CurrentCommand = inputQueueLoweredPriority.Dequeue();
			else
				throw new InvalidOperationException();
		}
		void OutputEnqueue() {
			if(CurrentCommand == null)
				throw new InvalidOperationException("Nothing to enqueue (CurrentCommand == null)");
			outQueue.Enqueue(CurrentCommand);
			CurrentCommand = null;
		}
		public Command OutputDequeue() {
			if(CountOutputCommand != 0)
				return outQueue.Dequeue();
			else
				throw new InvalidOperationException();
		}
		public Command PeekOutput() {
			if(CountOutputCommand > 0)
				return outQueue.Peek();
			else
				return null;
		}
		int CountInputCommand {
			get { return inputQueueNormalPriority.Count + inputQueueLoweredPriority.Count; }
		}
		public int CountOutputCommand {
			get { return outQueue.Count; }
		}
		public bool IsSomethingInQueue {
			get {
				return CountInputCommand > 0 || CountOutputCommand > 0 || CurrentCommand != null;
			}
		}
		public void CancelAll() {
			EstablishNormalPriority();
			foreach(Command command in GetAllCommands())				
				command.Cancel();
		}
		public void CancelAllButApply() {
			EstablishNormalPriority();
			foreach(Command command in GetAllCommands())
				if(!(command is CommandApply))
					command.Cancel();
		}
		public void CancelAllButRefresh() {
			EstablishNormalPriority();
			foreach(Command command in GetAllCommands())
				if(!(command is CommandRefresh))
					command.Cancel();
		}
		void OutputEnqueueNotification(Command notification) {
			outQueue.Enqueue(notification);
		}
		public void Cancel<T>() where T: Command {
			foreach(Command command in GetAllCommands())
				if(command is T)
					command.Cancel();
		}
		public void WeakCancel<T>() where T: Command {
			foreach(Command command in inputQueueNormalPriority)
				if(command is T)
					command.Cancel();
			foreach(Command command in inputQueueLoweredPriority)
				if(command is T)
					command.Cancel();
		}
		void Run() {
			ListServerGetOrFreeEventArgs args = new ListServerGetOrFreeEventArgs();
			if(ListServerGet != null) {
				try {
					ListServerGet(this, args);
				} catch { }
			}
			try {
				ListServer = ExtractListServer(args);
			} catch { }
			if(ListServer != null) {
				ListServer.InconsistencyDetected += new EventHandler<ServerModeInconsistencyDetectedEventArgs>(ListServer_InconsistencyDetected);
				ListServer.ExceptionThrown += new EventHandler<ServerModeExceptionThrownEventArgs>(ListServer_ExceptionThrown);
			}
			if(GetTypeInfo != null) {
				try {
					GetTypeInfoEventArgs typeInfoArgs = new GetTypeInfoEventArgs(args.ListServerSource, args.Tag);
					GetTypeInfo(this, typeInfoArgs);
					this.TypeInfo = typeInfoArgs.TypeInfo;
				} catch { }
			}
#if !DXPORTABLE
			if(this.TypeInfo == null) {
				if(args.ListServerSource != null)
					try {
						this.TypeInfo = ListBindingHelper.GetListItemProperties(args.ListServerSource);
					} catch { }
			}
#endif
			TypeInfoObtained.Set();
			AskUIToDispatchOutputQueue();
			DoLoop();
			if(ListServer != null) {
				ListServer.InconsistencyDetected -= new EventHandler<ServerModeInconsistencyDetectedEventArgs>(ListServer_InconsistencyDetected);
				ListServer.ExceptionThrown -= new EventHandler<ServerModeExceptionThrownEventArgs>(ListServer_ExceptionThrown);
			}
			if(ListServerFree != null) {
				try {
					ListServerFree(this, args);
				} catch { }
			}
		}
		static IListServer ExtractListServer(ListServerGetOrFreeEventArgs args) {
			object src = args.ListServerSource;
			IListSource probableSource = src as IListSource;
			if(probableSource != null) {
				src = probableSource.GetList();
			}
			return src as IListServer;
		}
		volatile bool _Terminate;
		public void AskForTermination() { _Terminate = true; }
		class BagOfTricks {
			public List<ListSourceGroupInfo> LastExpandedGroups;
			public int? LastIndex;
		}
		BagOfTricks _bagOfTricks;
		BagOfTricks Tricks {
			get {
				if(_bagOfTricks == null)
					_bagOfTricks = new BagOfTricks();
				return _bagOfTricks;
			}
		}
		void ClearTricksCache() {
			_bagOfTricks = null;
		}
		bool HasTricks { get { return _bagOfTricks != null; } }
		void DoTricks() {
			if(!HasTricks)
				return;
			if(DevExpress.Data.Utils.Helpers.WaitOne(MessageWaiter, 50))
				return;
			if(Tricks.LastExpandedGroups != null) {
				IEnumerable<ListSourceGroupInfo> toIterate;
				if(Tricks.LastExpandedGroups.Count <= 64)
					toIterate = Tricks.LastExpandedGroups;
				else
					toIterate = Tricks.LastExpandedGroups.Take(32).Concat(Tricks.LastExpandedGroups.AsEnumerable().Reverse().Take(8));
				foreach(ListSourceGroupInfo gr in toIterate) {
					if(DevExpress.Data.Utils.Helpers.WaitOne(MessageWaiter, 0))
						return;
					try {
						ListServer.GetGroupInfo(gr);
					} catch {
						break;
					}
				}
				Tricks.LastExpandedGroups = null;
			}
			if(Tricks.LastIndex.HasValue) {
				if(DevExpress.Data.Utils.Helpers.WaitOne(MessageWaiter, 0))
					return;
				try {
					int range = 16;
					int minIndex = Tricks.LastIndex.Value - range;
					if(minIndex >= 0)
						ListServer.GetRowKey(minIndex);
					if(DevExpress.Data.Utils.Helpers.WaitOne(MessageWaiter, 0))
						return;
					int maxIndex = Tricks.LastIndex.Value + range;
					if(maxIndex < ListServer.Count)
						ListServer.GetRowKey(maxIndex);
				} catch {
				} finally {
					Tricks.LastIndex = null;
				}
			}
			ClearTricksCache();
		}
		void DoLoop() {
			for(; ; ) {
				DoTricks();
				MessageWaiter.WaitOne();
				MessageWaiter.Reset();
				for(; ; ) {
					if(_Terminate)
						return;
					lock(SyncRoot) {
						if(CountInputCommand == 0)
							break;
						InputDequeue();
					}
					Command current = CurrentCommand;
					if(!current.IsCanceled) {
						try {
							if(ListServer == null)
								throw new ArgumentNullException("this.ListServer");
							current.Accept(this);
						} catch(Exception e) {
							current.Cancel(e);
						}
					}
					lock(SyncRoot) {
						OutputEnqueue();
					}
					AskUIToDispatchOutputQueue();
				}
			}
		}
		void AskUIToDispatchOutputQueue() {
			if(!skipPosts) {
				skipPosts = true;
				SynchronizationContext.Post(SomethingInTheOutputQueueCallback, null);
			}
		}
		void ListServer_ExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			lock(SyncRoot) {
				OutputEnqueueNotification(new NotificationExceptionThrown(e.Exception));
			}
		}
		void ListServer_InconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			lock(SyncRoot) {
				OutputEnqueueNotification(new NotificationInconsistencyDetected(e.Message));
			}
			TrottleInconsistency();
		}
		public static bool? ForceTrottleInconsistency;
		readonly List<DateTime> inconsistenciesWindow = new List<DateTime>();
		void TrottleInconsistency() {
			if(ForceTrottleInconsistency == false)
				return;
			DateTime utcNow = DateTime.UtcNow;
			if(inconsistenciesWindow.Count > 0 && inconsistenciesWindow[0] > utcNow)
				inconsistenciesWindow.Clear();
			while(inconsistenciesWindow.Count > 0 && utcNow.Subtract(inconsistenciesWindow[inconsistenciesWindow.Count - 1]).TotalSeconds > 120) {
				inconsistenciesWindow.RemoveAt(inconsistenciesWindow.Count - 1);
			}
			inconsistenciesWindow.Insert(0, utcNow);
			if(inconsistenciesWindow.Count > 4) {
				try {
					throw new InvalidOperationException("REALLY inconsistent source");
				} catch { }
				Thread.Sleep(16000);
			} else if(inconsistenciesWindow.Count > 2 && utcNow.Subtract(inconsistenciesWindow[2]).TotalSeconds < 8) {
				Thread.Sleep(4000);
			} else if(inconsistenciesWindow.Count > 1 && utcNow.Subtract(inconsistenciesWindow[1]).TotalSeconds < 2) {
				Thread.Sleep(1000);
			} else {
			}
		}
		#region IAsyncCommandVisitor Members
		void IAsyncCommandVisitor.Canceled(Command canceled) {
		}
		void IAsyncCommandVisitor.Visit(CommandGetTotals result) {
			result.TotalSummary = ListServer.GetTotalSummary();
			result.Count = ListServer.Count;
		}
		void IAsyncCommandVisitor.Visit(CommandGetRow result) {
			result.RowInfo = GetRowInfoFromRow(ListServer[result.Index]);
			result.RowKey = ListServer.GetRowKey(result.Index);
			Tricks.LastIndex = result.Index;
		}
		object GetRowInfoFromRow(object row) {
			if(GetWorkerThreadRowInfo == null)
				return row;
			else {
				GetWorkerThreadRowInfoEventArgs args = new GetWorkerThreadRowInfoEventArgs(this.TypeInfo, row);
				try {
					GetWorkerThreadRowInfo(this, args);
				} catch { }
				return args.RowInfo;
			}
		}
		void IAsyncCommandVisitor.Visit(CommandApply result) {
			ClearTricksCache();
			ListServer.Apply(result.FilterCriteria, result.SortInfo, result.GroupCount, result.GroupSummaryInfo, result.TotalSummaryInfo);
		}
		void IAsyncCommandVisitor.Visit(CommandRefresh result) {
			ClearTricksCache();
			Visit(result);
		}
		protected virtual void Visit(CommandRefresh result) {
			ListServer.Refresh();
		}
		void IAsyncCommandVisitor.Visit(CommandGetRowIndexByKey result) {
			result.Index = ListServer.GetRowIndexByKey(result.Key);
			if(result.Index >= 0) {
				List<CommandGetGroupInfo> groups = new List<CommandGetGroupInfo>();
				ListSourceGroupInfo currentGroup = null;
				int currentTop = 0;
				for(; ; ) {
					CommandGetGroupInfo cgi = new CommandGetGroupInfo(currentGroup);
					cgi.ChildrenGroups = ListServer.GetGroupInfo(currentGroup);
					if(cgi.ChildrenGroups == null || cgi.ChildrenGroups.Count == 0)
						break;
					groups.Add(cgi);
					bool found = false;
					foreach(ListSourceGroupInfo g in cgi.ChildrenGroups) {
						int nextTop = currentTop + g.ChildDataRowCount;
						if(result.Index >= currentTop && result.Index < nextTop) {
							currentGroup = g;
							found = true;
							break;
						} else {
							currentTop = nextTop;
						}
					}
					if(!found)
						break;
				}
				result.Groups = groups;
			}
		}
		void IAsyncCommandVisitor.Visit(CommandGetGroupInfo result) {
			result.ChildrenGroups = ListServer.GetGroupInfo(result.ParentGroup);
			Tricks.LastExpandedGroups = result.ChildrenGroups;
		}
		void IAsyncCommandVisitor.Visit(CommandGetUniqueColumnValues result) {
			result.Values = ListServer.GetUniqueColumnValues(result.Expression, result.MaxCount, result.IncludeFilteredOut);
		}
		void IAsyncCommandVisitor.Visit(CommandFindIncremental result) {
			result.RowIndex = ListServer.FindIncremental(result.Expression, result.Value, result.StartIndex, result.SearchUp, result.IgnoreStartRow, result.AllowLoop);
		}
		void IAsyncCommandVisitor.Visit(CommandLocateByValue result) {
			result.RowIndex = ListServer.LocateByValue(result.Expression, result.Value, result.StartIndex, result.SearchUp);
		}
		void IAsyncCommandVisitor.Visit(CommandGetAllFilteredAndSortedRows command) {
			IList rows = ListServer.GetAllFilteredAndSortedRows();
			List<object> rowsInfo = new List<object>(rows.Count);
			foreach(object row in rows) {
				rowsInfo.Add(GetRowInfoFromRow(row));
			}
			command.RowsInfo = rowsInfo.ToArray();
		}
		void IAsyncCommandVisitor.Visit(CommandPrefetchRows command) {
			CancellationToken token;
			lock(this) {
				command.CancellationTokenSource = new CancellationTokenSource();
				token = command.CancellationTokenSource.Token;
			}
			try {
				command.Successful = ListServer.PrefetchRows(command.GroupsToPrefetch, token);
			} finally {
				lock(this) {
					command.CancellationTokenSource.Dispose();
					command.CancellationTokenSource = null;
				}
			}
		}
		#endregion
	}
	public class AsyncListServer2DatacontrollerProxy: IAsyncListServer, IListServerHints, IDisposable, ITypedList, IList, IDXCloneable {
		protected readonly IAsyncListServer Nested;
		public AsyncListServer2DatacontrollerProxy(IAsyncListServer nested) {
			this.Nested = nested;
		}
		public virtual void Dispose() {
			((IDisposable)Nested).Dispose();
		}
		public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ((ITypedList)Nested).GetItemProperties(listAccessors);
		}
		public virtual string GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		public virtual int Add(object value) {
			throw new NotSupportedException();
		}
		public virtual void Clear() {
		}
		public virtual bool Contains(object value) {
			return false;
		}
		public virtual int IndexOf(object value) {
			return -1;
		}
		public virtual void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public virtual bool IsFixedSize {
			get { return true; }
		}
		public virtual bool IsReadOnly {
			get { return true; }
		}
		public virtual void Remove(object value) {
			throw new NotSupportedException();
		}
		public virtual void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public virtual object this[int index] {
			get {
				return null;
			}
			set {
			}
		}
		public virtual void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public virtual int Count {
			get { return 0; }
		}
		public virtual bool IsSynchronized {
			get { return false; }
		}
		public virtual object SyncRoot {
			get { return this; }
		}
		public virtual IEnumerator GetEnumerator() {
			throw new NotSupportedException();
		}
		public virtual CommandGetTotals GetTotals(params DictionaryEntry[] tags) {
			return Nested.GetTotals(tags);
		}
		public virtual CommandGetRow GetRow(int index, params DictionaryEntry[] tags) {
			return Nested.GetRow(index, tags);
		}
		public virtual CommandGetGroupInfo GetGroupInfo(ListSourceGroupInfo parentGroup, params DictionaryEntry[] tags) {
			return Nested.GetGroupInfo(parentGroup, tags);
		}
		public virtual CommandGetRowIndexByKey GetRowIndexByKey(object key, params DictionaryEntry[] tags) {
			return Nested.GetRowIndexByKey(key, tags);
		}
		public virtual CommandGetUniqueColumnValues GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut, params DictionaryEntry[] tags) {
			return Nested.GetUniqueColumnValues(expression, maxCount, includeFilteredOut, tags);
		}
		public virtual CommandFindIncremental FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop, params DictionaryEntry[] tags) {
			return Nested.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop, tags);
		}
		public virtual CommandLocateByValue LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp, params DictionaryEntry[] tags) {
			return Nested.LocateByValue(expression, value, startIndex, searchUp, tags);
		}
		public virtual CommandApply Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, params DictionaryEntry[] tags) {
			return Nested.Apply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo, tags);
		}
		public virtual CommandRefresh Refresh(params DictionaryEntry[] tags) {
			return Nested.Refresh(tags);
		}
		public virtual CommandGetAllFilteredAndSortedRows GetAllFilteredAndSortedRows(params DictionaryEntry[] tags) {
			return Nested.GetAllFilteredAndSortedRows(tags);
		}
		public virtual CommandPrefetchRows PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, params DictionaryEntry[] tags) {
			return Nested.PrefetchRows(groupsToPrefetch, tags);
		}
		public virtual void Cancel(Command command) {
			Nested.Cancel(command);
		}
		public virtual void Cancel<T>() where T: Command {
			Nested.Cancel<T>();
		}
		public virtual void WeakCancel<T>() where T: Command {
			Nested.WeakCancel<T>();
		}
		public virtual void SetReceiver(IAsyncResultReceiver receiver) {
			Nested.SetReceiver(receiver);
		}
		public virtual T PullNext<T>() where T: Command {
			return Nested.PullNext<T>();
		}
		public virtual bool WaitFor(Command waitForCommand) {
			return Nested.WaitFor(waitForCommand);
		}
		void DevExpress.Data.Helpers.IListServerHints.HintGridIsPaged(int pageSize) {
		}
		void DevExpress.Data.Helpers.IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
		}
		object IDXCloneable.DXClone() {
			return DXClone();
		}
		protected virtual AsyncListServer2DatacontrollerProxy DXClone() {
			IDXCloneable nestedCloneable = Nested as IDXCloneable;
			if(nestedCloneable == null)
				throw new InvalidOperationException(Nested.GetType().FullName + " does not implement IDXCloneable");
			IAsyncListServer clone = (IAsyncListServer)nestedCloneable.DXClone();
			return new AsyncListServer2DatacontrollerProxy(clone);
		}
	}
	public class ReadonlyThreadSafeProxyForObjectFromAnotherThread {
		public readonly object[] Content;
		public readonly object OriginalRow;
		public ReadonlyThreadSafeProxyForObjectFromAnotherThread(object original, object[] content) {
			this.OriginalRow = original;
			this.Content = content;
		}
		public static object ExtractOriginalRow(object uiThreadRow) {
			ReadonlyThreadSafeProxyForObjectFromAnotherThread wrapper = uiThreadRow as ReadonlyThreadSafeProxyForObjectFromAnotherThread;
			if(wrapper != null)
				return wrapper.OriginalRow;
			else
				return uiThreadRow;
		}
	}
	public class ReadonlyThreadSafeProxyForObjectFromAnotherThreadPropertyDescriptor: PropertyDescriptor {
		readonly Type Type;
		readonly int Index;
		readonly string displayName;
		public ReadonlyThreadSafeProxyForObjectFromAnotherThreadPropertyDescriptor(PropertyDescriptor proto, int index)
			: base(proto.Name, proto.Attributes.Cast<Attribute>().ToArray()) {
			this.Type = proto.PropertyType;
			this.displayName = proto.DisplayName;
			this.Index = index;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(ReadonlyThreadSafeProxyForObjectFromAnotherThread); }
		}
		public override object GetValue(object component) {
			ReadonlyThreadSafeProxyForObjectFromAnotherThread obj = component as ReadonlyThreadSafeProxyForObjectFromAnotherThread;
			if(obj == null)
				return null;
			if(obj.Content == null)
				return null;
			return obj.Content[Index];
		}
		public override bool IsReadOnly {
			get { return true; }
		}
		public override Type PropertyType {
			get { return Type; }
		}
		public override string DisplayName {
			get { return displayName; }
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			throw new NotSupportedException();
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
}
