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
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Data.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Helpers {
	public static class ItemsMovesSwapHelper {
		static void SwapMoves(ref int? from1, ref int? to1, ref int? from2, ref int? to2, bool skipFirstConversion) {
			if(to1.HasValue && from2 == to1)
				throw new InvalidOperationException("from2 == to1");
			int From1 = from1 ?? int.MaxValue;
			int From2 = from2 ?? int.MaxValue;
			int To1 = to1 ?? int.MaxValue;
			int To2 = to2 ?? int.MaxValue;
			int? newFrom1, newTo1;
			if(!from2.HasValue)
				newFrom1 = null;
			else if(From2 <= From1 && From2 > To1)
				newFrom1 = From2 - 1;
			else if(From2 >= From1 && From2 < To1)
				newFrom1 = From2 + 1;
			else
				newFrom1 = From2;
			if(!to2.HasValue)
				newTo1 = null;
			else if(To2 <= From1 && To2 > To1 || To2 == To1 && From1 > To1 && From2 < To2)
				newTo1 = To2 - 1;
			else if(To2 >= From1 && To2 < To1 || To2 == To1 && From1 < To1 && From2 > To2)
				newTo1 = To2 + 1;
			else
				newTo1 = To2;
			if(!skipFirstConversion) {
				int? newFrom2, newTo2;
				int _NewFrom1 = newFrom1 ?? int.MaxValue;
				int _NewTo1 = newTo1 ?? int.MaxValue;
				if(!from1.HasValue)
					newFrom2 = null;
				else if(From1 < _NewFrom1 && From1 >= _NewTo1)
					newFrom2 = From1 + 1;
				else if(From1 > _NewFrom1 && From1 <= _NewTo1)
					newFrom2 = From1 - 1;
				else
					newFrom2 = From1;
				if(!to1.HasValue)
					newTo2 = null;
				else if(To1 < From2 && To1 >= To2)
					newTo2 = To1 + 1;
				else if(To1 > From2 && To1 <= To2)
					newTo2 = To1 - 1;
				else
					newTo2 = To1;
				from1 = newFrom2;
				to1 = newTo2;
			}
			from2 = newFrom1;
			to2 = newTo1;
		}
		public static void SwapMoves(ref int? from1, ref int? to1, ref int? from2, ref int? to2) {
			SwapMoves(ref from1, ref to1, ref from2, ref to2, false);
		}
		public static void SwapMovesJustSecond(int? from1, int? to1, ref int? from2, ref int? to2) {
			SwapMoves(ref from1, ref to1, ref from2, ref to2, true);
		}
	}
	public abstract class RealTimeEventBase {
		readonly DateTime _Created = DateTime.Now;
		volatile RealTimeEventBase _Before;
		public RealTimeEventBase Before {
			get { return _Before; }
			set { _Before = value; }
		}
		public RealTimeEventBase After;
		public int Locks;
		public virtual void PostProcess(IRealTimeListChangeProcessor realTimeSourceCore) {
			realTimeSourceCore.NotifyLastProcessedCommandCreationTime(_Created);
		}
		public abstract void Push(RealTimeEventsQueue queue);
	}
	public class RealTimeRowEvent : RealTimeEventBase {
		public int? From;
		public int? To;
		public RealTimeProxyForObject FieldsChangeData;
		public RealTimeRowEvent(int? From, int? To, RealTimeProxyForObject value) {
			this.From = From;
			this.To = To;
			this.FieldsChangeData = value;
		}
		public override void PostProcess(IRealTimeListChangeProcessor realTimeSourceCore) {
			if(!From.HasValue && !To.HasValue)
				throw new InvalidOperationException("Internal error: From and To is null.");
			ListChangedEventArgs eventArg;
			if(!From.HasValue) {
				if(To.Value < 0 || To.Value > realTimeSourceCore.Cache.Count)
					throw new InvalidOperationException("Internal error: Add index.");
				if(FieldsChangeData == null)
					throw new InvalidOperationException("Internal error: Add value.");
				realTimeSourceCore.Cache.Insert(To.Value, FieldsChangeData);
				eventArg = RTListChangedEventArgs.Create(ListChangedType.ItemAdded, To.Value, -1, null);
			} else if(!To.HasValue) {
				if(From.Value < 0 || From.Value >= realTimeSourceCore.Cache.Count)
					throw new InvalidOperationException("Internal error: Delete index.");
				realTimeSourceCore.Cache.RemoveAt(From.Value);
				eventArg = RTListChangedEventArgs.Create(ListChangedType.ItemDeleted, From.Value, -1, null);
			} else if(To == From) {
				if(To.Value < 0 || To.Value >= realTimeSourceCore.Cache.Count)
					throw new InvalidOperationException("Internal error: Change index.");
				if(FieldsChangeData == null)
					throw new InvalidOperationException("Internal error: Change value.");
				realTimeSourceCore.Cache[To.Value].Assign(FieldsChangeData);
				eventArg = RTListChangedEventArgs.Create(ListChangedType.ItemChanged, To.Value, -1, FieldsChangeData.GetChangedPropertyDescriptor());
			} else {
				if(To.Value < 0 || To.Value >= realTimeSourceCore.Cache.Count || From.Value < 0 || From.Value >= realTimeSourceCore.Cache.Count)
					throw new InvalidOperationException("Internal error: Move index.");
				if(FieldsChangeData == null)
					throw new InvalidOperationException("Internal error: Move value.");
				realTimeSourceCore.Cache.RemoveAt(From.Value);
				realTimeSourceCore.Cache.Insert(To.Value, FieldsChangeData);
				eventArg = RTListChangedEventArgs.Create(ListChangedType.ItemMoved, To.Value, From.Value, null);
			}
			if(realTimeSourceCore.ListChanged != null && !realTimeSourceCore.IsCatchUp) {
				realTimeSourceCore.ListChanged(realTimeSourceCore, eventArg);
			}
			base.PostProcess(realTimeSourceCore);
		}
		public override void Push(RealTimeEventsQueue queue) {
			queue.PushEvent(this);
		}
	}
	public class RealTimeResetEvent : RealTimeEventBase {
		public RealTimeProxyForObject[] AllRowsData;
		public RealTimeResetEvent(RealTimeProxyForObject[] allValues) {
			this.AllRowsData = allValues;
		}
		public override void PostProcess(IRealTimeListChangeProcessor realTimeSourceCore) {
			realTimeSourceCore.Cache = new List<RealTimeProxyForObject>();
			realTimeSourceCore.Cache.AddRange(AllRowsData);
			realTimeSourceCore.IsCatchUp = false;
			if(realTimeSourceCore.ListChanged != null) {
				realTimeSourceCore.ListChanged(realTimeSourceCore, RTListChangedEventArgs.Create(ListChangedType.PropertyDescriptorChanged, -1));
			}
			base.PostProcess(realTimeSourceCore);
		}
		public override void Push(RealTimeEventsQueue queue) {
			queue.PushEvent(this);
		}
	}
	public class RealTimePropertyDescriptorsChangedEvent : RealTimeResetEvent {
		public RealTimePropertyDescriptorCollection properties;
		public RealTimePropertyDescriptorsChangedEvent(RealTimePropertyDescriptorCollection properties, RealTimeProxyForObject[] allValues)
			: base(allValues) {
			this.properties = properties;
		}
		public override void PostProcess(IRealTimeListChangeProcessor realTimeSourceCore) {
			realTimeSourceCore.PropertyDescriptorsCollection = properties;
			base.PostProcess(realTimeSourceCore);
		}
	}
	public class RealTimeEventsQueue {
		static void SureLock(ref int location, int lockedValue) {
			for(int i = 0; i < 1024; ++i) {
				if(Interlocked.CompareExchange(ref location, lockedValue, 0) == 0)
					return;
				Thread.Sleep(0);
			}
			while(Interlocked.CompareExchange(ref location, lockedValue, 0) != 0)
				Thread.Sleep(1);
		}
		static void UnLock(ref int location, int lockedValue) {
			int oldValue = Interlocked.Exchange(ref location, 0);
			System.Diagnostics.Debug.Assert(oldValue == lockedValue);
		}
		int Orderer4InQueueLock, InQueueLock, BufferLock, UiLock;
		RealTimeEventBase First, Last;
		Queue<RealTimeRowEvent> Buffer = new Queue<RealTimeRowEvent>();
		int BufferedProcessing;
		readonly Action SomethingInTheQueueAction;
		public RealTimeEventsQueue(Action _SomethingInTheQueueAction) {
			this.SomethingInTheQueueAction = _SomethingInTheQueueAction;
		}
		public RealTimeEventBase PullEvent() {
			for(; ; ) {
				RealTimeEventBase ev = PullEventCore();
				if(ev != null && IsEmptyEvent(ev))
					continue;
				return ev;
			}
		}
		public bool IsSomethingToPull() {
			if(Interlocked.CompareExchange(ref UiLock, 1, 0) != 0)
				return true;
			try {
				return First != null;
			} finally {
				UnLock(ref UiLock, 1);
			}
		}
		public bool IsEmpty() {
			if(Interlocked.CompareExchange(ref InQueueLock, 1, 0) != 0)
				return false;
			try {
				if(Last != null)
					return false;
				if(Interlocked.CompareExchange(ref BufferLock, 1, 0) != 0)
					return false;
				try {
					return Buffer.Count == 0;
				} finally {
					UnLock(ref BufferLock, 1);
				}
			} finally {
				UnLock(ref InQueueLock, 1);
			}
		}
		RealTimeEventBase PullEventCore() {
			if(Interlocked.CompareExchange(ref UiLock, 2, 0) != 0)
				return null;
			try {
				RealTimeEventBase rv = First;
				Thread.MemoryBarrier();
				if(rv == null)
					return null;
				if(Interlocked.CompareExchange(ref rv.Locks, 42, 0) != 0)
					return null;
				rv.Before = null;
				First = rv.After;
				Interlocked.CompareExchange(ref Last, null, rv);
				return rv;
			} finally {
				UnLock(ref UiLock, 2);
			}
		}
		public IEnumerable<RealTimeEventBase> PullAllEvents() {
			RealTimeEventBase current;
			Queue<RealTimeRowEvent> buff;
			SureLock(ref InQueueLock, 333);
			try {
				SureLock(ref BufferLock, 333);
				try {
					SureLock(ref UiLock, 333);
					try {
						current = Interlocked.Exchange(ref First, null);
						Last = null;
						buff = Buffer;
						Buffer = new Queue<RealTimeRowEvent>();
					} finally {
						UnLock(ref UiLock, 333);
					}
				} finally {
					UnLock(ref BufferLock, 333);
				}
			} finally {
				UnLock(ref InQueueLock, 333);
			}
			List<RealTimeEventBase> rv = new List<RealTimeEventBase>();
			for(; current != null; current = current.After) {
				if(IsEmptyEvent(current))
					continue;
				rv.Add(current);
			}
			rv.AddRange(buff.Cast<RealTimeEventBase>());
			return rv;
		}
		static bool IsEmptyEvent(RealTimeEventBase ev) {
			RealTimeRowEvent rowEvent = ev as RealTimeRowEvent;
			if(rowEvent == null)
				return false;
			return !rowEvent.From.HasValue && !rowEvent.To.HasValue;
		}
		public void PushEvent(RealTimePropertyDescriptorsChangedEvent pdcEvent) {
			SureLock(ref InQueueLock, 1);
			try {
				SureLock(ref BufferLock, 1);
				try {
					SureLock(ref UiLock, 1);
					try {
						pdcEvent.Before = null;
						pdcEvent.After = null;
						First = pdcEvent;
						Last = pdcEvent;
					} finally {
						UnLock(ref UiLock, 1);
					}
					Buffer.Clear();
				} finally {
					UnLock(ref BufferLock, 1);
				}
			} finally {
				UnLock(ref InQueueLock, 1);
			}
			SomethingReadyForPull();
		}
		public void PushEvent(RealTimeResetEvent resetEvent) {
			SureLock(ref InQueueLock, 1);
			try {
				SureLock(ref BufferLock, 1);
				try {
					SureLock(ref UiLock, 1);
					try {
						RealTimePropertyDescriptorsChangedEvent pdcEvent = First as RealTimePropertyDescriptorsChangedEvent;
						if(pdcEvent != null) {
							pdcEvent.AllRowsData = resetEvent.AllRowsData;
							pdcEvent.Before = null;
							pdcEvent.After = null;
							First = pdcEvent;
							Last = pdcEvent;
						} else {
							resetEvent.Before = null;
							resetEvent.After = null;
							First = resetEvent;
							Last = resetEvent;
						}
					} finally {
						UnLock(ref UiLock, 1);
					}
					Buffer.Clear();
				} finally {
					UnLock(ref BufferLock, 1);
				}
			} finally {
				UnLock(ref InQueueLock, 1);
			}
			SomethingReadyForPull();
		}
		public void PushEvent(RealTimeRowEvent rowEvent) {
			bool processHere;
			SureLock(ref BufferLock, 1);
			try {
				Buffer.Enqueue(rowEvent);
				processHere = Buffer.Count >= 16384 || this.Last == null;
			} finally {
				UnLock(ref BufferLock, 1);
			}
			if(processHere)
				ProcessBufferStep();
			else
				StartBufferProcessingIfNeeded();
		}
		void StartBufferProcessingIfNeeded() {
			if(Interlocked.CompareExchange(ref BufferedProcessing, 1, 0) != 0)
				return;
			ThreadPool.QueueUserWorkItem(x => _BufferProcessing());
		}
		void _BufferProcessing() {
			for(; ; ) {
				while(ProcessBufferStep())
					;
				SureLock(ref BufferLock, 5);
				try {
					if(Buffer.Count == 0) {
						Interlocked.Exchange(ref BufferedProcessing, 0);
						return;
					}
				} finally {
					UnLock(ref BufferLock, 5);
				}
			}
		}
		bool ProcessBufferStep() {
			SureLock(ref Orderer4InQueueLock, 1);
			SureLock(ref InQueueLock, 1);
			UnLock(ref Orderer4InQueueLock, 1);
			try {
				RealTimeRowEvent rowEvent;
				SureLock(ref BufferLock, 1);
				try {
					if(Buffer.Count == 0)
						return false;
					rowEvent = Buffer.Dequeue();
				} finally {
					UnLock(ref BufferLock, 1);
				}
				if(ProcessRowInQueueIfExists(rowEvent)) {
					;
				} else {
					PushEventCore(rowEvent);
				}
			} finally {
				UnLock(ref InQueueLock, 1);
			}
			SomethingReadyForPull();
			return true;
		}
		void PushEventCore(RealTimeRowEvent rowEvent) {
			for(; ; ) {
				RealTimeEventBase previous = this.Last;
				Thread.MemoryBarrier();
				if(previous == null) {
					rowEvent.Before = null;
					rowEvent.After = null;
					this.Last = rowEvent;
					this.First = rowEvent;
					return;
				} else {
					if(Interlocked.CompareExchange(ref previous.Locks, 1, 0) != 0)
						continue;
					try {
						rowEvent.Before = previous;
						rowEvent.After = null;
						previous.After = rowEvent;
						this.Last = rowEvent;
					} finally {
						Interlocked.Exchange(ref previous.Locks, 0);
					}
					return;
				}
			}
		}
		bool ProcessRowInQueueIfExists(RealTimeRowEvent rowEvent) {
			RealTimeRowEvent found = FindRowInQueue(rowEvent.From, rowEvent.To);
			if(found == null)
				return false;
			if(Interlocked.CompareExchange(ref found.Locks, 123456, 0) == 0) {
				try {
					int? currentFrom = rowEvent.From;
					int? currentTo = rowEvent.To;
					for(RealTimeEventBase current = Last; ; current = current.Before) {
						RealTimeRowEvent row = current as RealTimeRowEvent;
						if(found == row) {
							if(found.To != currentFrom)
								throw new InvalidOperationException("Internal error: found.To != currentFrom");
							if(!found.To.HasValue)
								throw new Exception();
							found.To = currentTo;
							found.FieldsChangeData.Assign(rowEvent.FieldsChangeData);
							return true;
						}
						if(row == null)
							continue;
						ItemsMovesSwapHelper.SwapMoves(ref row.From, ref row.To, ref currentFrom, ref currentTo);
					}
				} finally {
					Interlocked.Exchange(ref found.Locks, 0);
				}
			}
			return false;
		}
		RealTimeRowEvent FindRowInQueue(int? from, int? to) {
			if(!from.HasValue)
				return null;
			int? currentFrom = from;
			int? currentTo = to;
			for(RealTimeEventBase current = Last; ; current = current.Before) {
				if(current == null)
					return null;
				if(current.Locks != 0)
					return null;
				RealTimeRowEvent row = current as RealTimeRowEvent;
				if(row == null)
					continue;
				if(row.To == currentFrom)
					return row;
				ItemsMovesSwapHelper.SwapMovesJustSecond(row.From, row.To, ref currentFrom, ref currentTo);
			}
		}
		void SomethingReadyForPull() {
			if(SomethingInTheQueueAction != null)
				SomethingInTheQueueAction();
		}
	}
	public delegate void ListChangedCoreDelegate(object sender, RealTimeEventBase command);
	public class RealTimeQueue : IDisposable {
		enum QueueState { Stop, PrepareToWork, Work }
		readonly SynchronizationContext syncContext;
		readonly RealTimeEventsQueue outputQuery;
		readonly ListChangedCoreDelegate listChangedCore;
		IBindingList source;
		RealTimePropertyDescriptorCollection sourcePropertyDescriptorCollection;
		readonly bool useWeakEventHandler;
		bool needReset;
		bool isDisposed;
		QueueState state = QueueState.Stop;
		readonly object syncObject = new object();
		ListChangedWeakEventHandler<RealTimeQueue> listChangedHandler;
		ListChangedWeakEventHandler<RealTimeQueue> ListChangedHandler {
			get {
				if(listChangedHandler == null) {
					listChangedHandler = new ListChangedWeakEventHandler<RealTimeQueue>(this, (owner, o, e) => owner.SourceListChanged(o, e));
				}
				return listChangedHandler;
			}
		}
		string displayableProperties;
		public string DisplayableProperties {
			get { return displayableProperties; }
			set {
				if(displayableProperties == value) return;
				displayableProperties = value;
				PushPropertyDescriptorChanged();
			}
		}
		internal RealTimeQueue(SynchronizationContext syncContext, IBindingList source, RealTimePropertyDescriptorCollection propertyDescriptorCollection, ListChangedCoreDelegate listChanged, bool useWeakEventHandler, string displayableProperties) {
			if(syncContext == null)
				throw new ArgumentNullException("syncContext");
			this.syncContext = syncContext;
			this.source = source;
			this.listChangedCore = listChanged;
			this.sourcePropertyDescriptorCollection = propertyDescriptorCollection;
			this.useWeakEventHandler = useWeakEventHandler;
			this.needReset = false;
			this.outputQuery = new RealTimeEventsQueue(somethingInTheQueue);
			if(this.source != null) {
				if(this.useWeakEventHandler)
					this.source.ListChanged += ListChangedHandler.Handler;
				else
					this.source.ListChanged += SourceListChanged;
			}
			this.state = QueueState.PrepareToWork;
			SourceListChanged(source, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
			this.state = QueueState.Work;
		}
		int posted = 0;
		void somethingInTheQueue() {
			if(syncContext != null) {
				if(Interlocked.CompareExchange(ref posted, 1, 0) == 0) {
					syncContext.Post((o) => SomethingInTheQueueUI()
					, null);
				} else {
				}
			}
		}
		void SomethingInTheQueueUI() {
			if(isDisposed)
				return;
			Stopwatch watch = Stopwatch.StartNew();
			bool reTrottle = false;
			try {
				for(; ; ) {
					RealTimeEventBase outputCommand = outputQuery.PullEvent();
					if(outputCommand == null)
						break;
					reTrottle = true;
					if(listChangedCore != null)
						listChangedCore(this, outputCommand);
					if(watch.ElapsedMilliseconds > 100) {   
						break;
					}
				}
			} finally {
				watch.Stop();
			}
			if(!reTrottle) {
				int oldPosted = Interlocked.CompareExchange(ref posted, 0, 1);
				System.Diagnostics.Debug.Assert(oldPosted == 1);
				if(outputQuery.IsSomethingToPull()) {
					if(Interlocked.CompareExchange(ref posted, 1, 0) == 0) {
						reTrottle = true;
					}
				}
			}
			if(reTrottle) {
				long processingTime = watch.ElapsedMilliseconds;
				int trottleTime = (int)Math.Min(Math.Max(processingTime / 2, 10), 500);
				RealTimeSourceTrottler.Trottle(syncContext, trottleTime,
					() => RealTimeSourceTrottler.Trottle(syncContext, trottleTime,
						() => RealTimeSourceTrottler.Trottle(syncContext, trottleTime,
						SomethingInTheQueueUI
						)
						)
					);
			}
		}
		internal bool IsQueueEmpty {
			get {
				if(this.outputQuery == null)
					return true;
				return outputQuery.IsEmpty();
			}
		}
		void SourceListChanged(object sender, ListChangedEventArgs e) {
			lock(syncObject) {
				if(this.source == null)
					return;
				if(state == QueueState.Stop || state == QueueState.PrepareToWork && e.ListChangedType != ListChangedType.PropertyDescriptorChanged)
					return;
				lock(this.source.SyncRoot) {
					ListChangedType changeType = e.ListChangedType;
					if(this.sourcePropertyDescriptorCollection.Count == 0)
						changeType = ListChangedType.PropertyDescriptorChanged;
					if(this.needReset)
						changeType = ListChangedType.PropertyDescriptorChanged;
					RealTimeProxyForObject value;
					switch(changeType) {
						case ListChangedType.Reset:
							RealTimeProxyForObject[] allValues = GetAllRows(this.sourcePropertyDescriptorCollection);
							if(allValues != null) {
								EnqueueOutputItem(new RealTimeResetEvent(allValues));
							}
							break;
						case ListChangedType.ItemAdded:
							value = new RealTimeProxyForObject(source[e.NewIndex], this.sourcePropertyDescriptorCollection);
							EnqueueOutputItem(new RealTimeRowEvent(null, e.NewIndex, value));
							break;
						case ListChangedType.ItemDeleted:
							EnqueueOutputItem(new RealTimeRowEvent(e.NewIndex, null, null));
							break;
						case ListChangedType.ItemMoved:
							value = new RealTimeProxyForObject(source[e.NewIndex], this.sourcePropertyDescriptorCollection);
							EnqueueOutputItem(new RealTimeRowEvent(e.OldIndex, e.NewIndex, value));
							break;
						case ListChangedType.ItemChanged:
							if(e.PropertyDescriptor == null)
								value = new RealTimeProxyForObject(source[e.NewIndex], this.sourcePropertyDescriptorCollection);
							else
								value = new RealTimeProxyForObject(source[e.NewIndex], this.sourcePropertyDescriptorCollection, e.PropertyDescriptor);
							EnqueueOutputItem(new RealTimeRowEvent(e.NewIndex, e.NewIndex, value));
							break;
						case ListChangedType.PropertyDescriptorAdded:
						case ListChangedType.PropertyDescriptorDeleted:
						case ListChangedType.PropertyDescriptorChanged:
							PushPropertyDescriptorChanged();
							break;
						default:
							throw new InvalidOperationException("ListChangedType");
					}
				}
			}
		}
		private void PushPropertyDescriptorChanged() {
			RealTimeProxyForObject[] allValues;
			this.sourcePropertyDescriptorCollection = RealTimePropertyDescriptorCollection.CreatePropertyDescriptorCollection(this.source, displayableProperties);
			allValues = GetAllRows(this.sourcePropertyDescriptorCollection);
			if(allValues != null) {
				EnqueueOutputItem(new RealTimePropertyDescriptorsChangedEvent(this.sourcePropertyDescriptorCollection, allValues));
			}
		}
		RealTimeProxyForObject[] GetAllRows(RealTimePropertyDescriptorCollection pdc) {
			List<RealTimeProxyForObject> components = new List<RealTimeProxyForObject>();
			try {
				foreach(var item in this.source) {
					components.Add(new RealTimeProxyForObject(item, pdc));
				}
			} catch {
				this.needReset = true;
				return null;
				;
			}
			return components.ToArray();
		}
		void EnqueueOutputItem(RealTimeEventBase command) {
			command.Push(outputQuery);
			if(needReset && ((command is RealTimeResetEvent) || (command is RealTimePropertyDescriptorsChangedEvent))) {
				needReset = false;
			}
		}
		public virtual void Dispose() {
			lock(syncObject) {
				if(this.useWeakEventHandler)
					this.source.ListChanged -= ListChangedHandler.Handler;
				else
					this.source.ListChanged -= SourceListChanged;
				this.source = null;
				this.isDisposed = true;
				this.state = QueueState.Stop;
			}
		}
	}
	public class RealTimeSourceTrottler {
		public static void Trottle(SynchronizationContext context, int trottleTime, Action action) {
			new RealTimeSourceTrottler(context, trottleTime, action);
		}
		readonly SynchronizationContext Context;
		readonly Action Action;
		readonly Timer Timer;
		int Disposed;
		RealTimeSourceTrottler(SynchronizationContext context, int trottleTime, Action action) {
			this.Context = context;
			this.Action = action;
			System.Windows.Forms.Application.Idle += OnIdle;
			System.Windows.Interop.ComponentDispatcher.ThreadIdle += OnIdle;
			Timer = new Timer(x => Do(), null, trottleTime, Timeout.Infinite);
		}
		void OnIdle(object sender, EventArgs e) {
			Do();
		}
		void Do() {
			if(Disposed != 0)
				return;
			Context.Post((d) => DoCore(), null);
		}
		void DoCore() {
			if(Interlocked.CompareExchange(ref Disposed, 1, 0) != 0)
				return;
			System.Windows.Forms.Application.Idle -= OnIdle;
			System.Windows.Interop.ComponentDispatcher.ThreadIdle -= OnIdle;
			Timer.Dispose();
			Action();
		}
	}
}
