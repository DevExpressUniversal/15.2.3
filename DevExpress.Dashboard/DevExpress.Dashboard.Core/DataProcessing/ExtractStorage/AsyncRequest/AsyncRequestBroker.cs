#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class VirtualColumn {
		int blockIndex = 0;
		ByteBufferPool bufferPool;
		protected ByteBufferPool BufferPool { get { return bufferPool; } }
		protected IStorageColumn Column { get; set; }
		public string Name { get { return Column.Name; } }
		protected VirtualColumn(IStorageColumn column, ByteBufferPool bufferPool) {
			this.Column = column;
			this.bufferPool = bufferPool;
		}
		internal abstract bool ContainsData(int index, int count);
		internal abstract void CopyData(ITask request);
		public abstract void Process(ReadTask complete);
		public List<ReadTask> TryFill(ITask request) {
			List<ReadTask> readTasks = new List<ReadTask>();
			if(request.ValuesKind == TaskValuesKind.Unique) {
				CopyData(request);
				request.IsReady = true;
				return null;
			}
			if(ContainsData(request.StartIndex, request.RecordCount) || blockIndex == Column.CompressedBlocksCount) {
				CopyData(request);
				request.IsReady = true;
				return null;
			}
			int count = 0;
			while(count < request.RecordCount * 2 && count < Column.Records) {
				int start;
				int end;
				int offset;
				int length;
				if(!Column.GetCompressedBlockMetadata(blockIndex, out start, out end, out offset, out length))
					break;
				blockIndex++;
				count += end - start + 1;
				ByteBuffer buffer = GetBuffer();
				readTasks.Add(new ReadTask() { ColumnName = Column.Name, StartIndex = start, RecordsCount = end - start + 1, LengthInBytes = length, Offset = offset, Buffer = buffer });
			}
			return readTasks;
		}
		ByteBuffer GetBuffer() {
			return bufferPool.GetAvailableObject();
		}
		internal virtual void Reset() {
			blockIndex = 0;
		}
	}
	public class TypedVirtualColumn<T> : VirtualColumn {
		int start;
		int end;
		CircularBuffer<Int32> substitutes;
		CircularBuffer<T> materialized;
		CircularBuffer<T> uniqueValues;
		T[] dictionary;
		List<ReadTask> complitedTasks;
		public TypedVirtualColumn(IStorageColumn column, ByteBufferPool bufferPool) :
			base(column, bufferPool) {
			substitutes = new CircularBuffer<int>(1024 * 1024);
			materialized = new CircularBuffer<T>(1024 * 1024);
			complitedTasks = new List<ReadTask>();
			dictionary = column.ReadColumnUniqueValues<T>(0, column.UniqueCount);
			uniqueValues = new CircularBuffer<T>(dictionary.Length);
			uniqueValues.Push(dictionary, 0, dictionary.Length);
		}
		public override void Process(ReadTask complete) {
			List<ReadTask> toRemove = new List<ReadTask>();
			complitedTasks.Add(complete);
			complitedTasks.Sort(new ReadTaskComparer());
			foreach(ReadTask task in complitedTasks) {
				if(task.StartIndex != end)
					break;
				int[] temp = DataCompression.Decompression(task.Buffer, this.Column.UniqueCount, task.RecordsCount);
				if(temp.Length != task.RecordsCount)
					throw new Exception("error when decompression of data");
				substitutes.Push(temp, 0, temp.Length);
				this.end = task.StartIndex + task.RecordsCount;
				toRemove.Add(task);
				task.Buffer.Clean();
				BufferPool.ReturnObjectToPool(task.Buffer);
			}
			complitedTasks.RemoveAll(x => { return toRemove.Contains(x); });
		}
		internal override bool ContainsData(int index, int count) {
			return this.start <= index && index + count <= this.end;
		}
		internal override void CopyData(ITask request) {
			int count;
			switch(request.ValuesKind) {
				case TaskValuesKind.Substitutes:
					count = request.MoveToBuffer<Int32>(substitutes);
					this.start += count;
					break;
				case TaskValuesKind.Materialized:
					count = Math.Min(request.RecordCount, substitutes.Count);
					for(int i = 0; i < count; i++) {
						int index = substitutes.PullOne();
						materialized.PushOne(dictionary[index]);
					}
					this.start += count;
					request.MoveToBuffer<T>(materialized);
					break;
				case TaskValuesKind.Unique:
					request.CopyToBuffer<T>(uniqueValues);
					break;
			}
			if(start == end && Column.Records == end && complitedTasks.Count == 0)
				Reset();
		}
		internal override void Reset() {
			base.Reset();
			substitutes.Clean();
			end = 0;
			start = 0;
		}
	}
	public class AsyncRequestBroker : IRequestBroker, IThreadObject {
		object dataStreamLocker;
		EventWaitHandle brokerResponseEvent;
		EventWaitHandle clientsCheckEndedEvent;
		DataReader dataWorker;
		Thread selfThread;
		bool isAlive = true;
		QueueWithWaiting<ITask> inQueue;
		Dictionary<string, VirtualColumn> columns;
		Dictionary<string, ITask> inProcessing;
		ByteBufferPool byteBufferPool;
		CommonWaiter waiter;
		int clientCount;
		int counter = 0;
		public AsyncRequestBroker(string path, bool inMemory) {
			dataStreamLocker = new object();
			brokerResponseEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
			clientsCheckEndedEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
			inQueue = new QueueWithWaiting<ITask>(100);
			columns = new Dictionary<string, VirtualColumn>();
			inProcessing = new Dictionary<string, ITask>();
			if (inMemory)
				dataWorker = new MemoryReader(path);
			else
				dataWorker = new FileReader(path);
			selfThread = new Thread(new ThreadStart(Loop));
			byteBufferPool = new ByteBufferPool(1024 * 1024 * 10);
			waiter = CommonWaiter.Create(inQueue, dataWorker.OutQueue);
		}
		void FillOrRead(ITask currentTask, VirtualColumn column) {
			inProcessing.Add(column.Name, currentTask);
			List<ReadTask> readTasks = column.TryFill(currentTask);
			if (readTasks != null)
				dataWorker.InQueue.Push(readTasks);
			else
				WaitAllClients(column.Name);
		}
		void ProcessCompletedReadTask() {
			if (dataWorker.OutQueue.Count == 0)
				return;
			ReadTask complete;
			if (!dataWorker.OutQueue.WaitAndPull(out complete))
				return;
			ITask currentTask = inProcessing[complete.ColumnName];
			VirtualColumn column = columns[currentTask.ColumnName];
			column.Process(complete);
			List<ReadTask> readTasks = column.TryFill(currentTask);
			if (readTasks == null)
				WaitAllClients(complete.ColumnName);
		}
		ITask GetIncomingTask() {
			ITask task = null;
			if (inQueue.Count > 0)
				inQueue.WaitAndPull(out task);
			return task;
		}
		bool NotAllClientsYet() {
			return counter < inProcessing.Count + inQueue.Count;
		}
		void WaitAllClients(string columnNameByCompletedTask) {
			counter = 0;
#if !DXPORTABLE // TODO dnxcore
			while (NotAllClientsYet())
				WaitHandle.SignalAndWait(brokerResponseEvent, clientsCheckEndedEvent);
#endif
			inProcessing.Remove(columnNameByCompletedTask);
		}
		bool NeedWait() {
			return dataWorker.OutQueue.Count == 0 && inQueue.Count == 0;
		}
		VirtualColumn CreateVirtualColumn(IStorageColumn column) {
			if (column.Type == typeof(Int32))
				return new TypedVirtualColumn<Int32>(column, byteBufferPool);
			if (column.Type == typeof(string))
				return new TypedVirtualColumn<string>(column, byteBufferPool);
			if (column.Type == typeof(decimal))
				return new TypedVirtualColumn<decimal>(column, byteBufferPool);
			throw new NotSupportedException();
		}
		void Loop() {
			while (isAlive) {
				if (NeedWait())
					waiter.WaitAny();
				ITask currentTask = GetIncomingTask();
				if (currentTask != null) {
					VirtualColumn column = columns[currentTask.ColumnName];
					FillOrRead(currentTask, column);
				}
				ProcessCompletedReadTask();
			}
		}
		internal void AddDataStream(IStorageColumn column) {
			lock (dataStreamLocker) {
				if (!columns.ContainsKey(column.Name))
					columns.Add(column.Name, CreateVirtualColumn(column));
			}
		}
		#region IRequestBroker
		void IRequestBroker.SendRequest(ITask task) {
			inQueue.Push(task);
		}
		void IRequestBroker.ClientCheck() {
			counter++;
			if (counter > inProcessing.Count + inQueue.Count)
				throw new Exception("unknown clients");
		}
		TaskLocker IRequestBroker.CreateTaskLocker() {
			clientCount++;
			return new TaskLocker(brokerResponseEvent, clientsCheckEndedEvent, this);
		}
		void IRequestBroker.DestroyTaskLocker(TaskLocker locker) {
			clientCount--;
			locker.Destroy(this);
		}
		#endregion
		#region IThreadObject
		void IThreadObject.Start() {
			((IThreadObject)dataWorker).Start();
			selfThread.Start();
		}
		void IThreadObject.Stop() {
			inQueue.Stop();
			((IThreadObject)dataWorker).Stop();
			isAlive = false;
		}
		bool IThreadObject.Join(int millisecondsTimeout) {
			return selfThread.Join(millisecondsTimeout);
		}
		#endregion IThreadObject
	}
	public class TaskLocker {
		readonly IRequestBroker broker;
		readonly EventWaitHandle brokerResponseEvent;
		readonly EventWaitHandle clientsCheckEndedEvent;
		readonly EventWaitHandle destroyEvent;
		readonly EventWaitHandle[] waitAny;
		bool isAlive = true;
		public TaskLocker(EventWaitHandle brokerResponseEvent, EventWaitHandle clientsCheckEndedEvent, IRequestBroker broker) {
			this.brokerResponseEvent = brokerResponseEvent;
			this.clientsCheckEndedEvent = clientsCheckEndedEvent;
			this.destroyEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
			waitAny = new EventWaitHandle[] { this.brokerResponseEvent, this.destroyEvent };
			this.broker = broker;
		}
		public void WaitUntilReady(ITask task) {
			if (!isAlive)
				return;
			broker.SendRequest(task);
			while (isAlive) {
				EventWaitHandle.WaitAny(waitAny);
				if (task.IsReady || !isAlive)
					break;
				lock (broker)
					broker.ClientCheck();
				clientsCheckEndedEvent.Set();
			}
			lock (broker)
				broker.ClientCheck();
			clientsCheckEndedEvent.Set();
		}
		public void Destroy(IRequestBroker destrer) {
			if (destrer != broker)
				throw new Exception("error when trying to destroy locker");
			isAlive = false;
			this.destroyEvent.Set();
		}
	}
	public enum TaskValuesKind { Substitutes, Materialized, Unique }
	public class TaskForBroker<T> : ITask {
		readonly string column;
		readonly IDataVector<T> vector;
		bool isReady;
		TaskValuesKind valuesKind;
		int startIndex;
		public TaskForBroker(int startIndex, string column, IDataVector<T> vector, TaskValuesKind valuesKind) {
			this.startIndex = startIndex;
			this.column = column;
			this.vector = vector;
			this.valuesKind = valuesKind;
		}
		#region IStorage
		TaskValuesKind ITask.ValuesKind { get { return valuesKind; } }
		bool ITask.IsReady { get { return isReady; } set { isReady = value; } }
		string ITask.ColumnName { get { return column; } }
		int ITask.MoveToBuffer<IN>(CircularBuffer<IN> input) {
			if (typeof(IN) != typeof(T))
				throw new Exception("wrong type of requested data, column : " + column);
			int count = input.Pull(vector.Data as IN[], vector.Data.Length);
			vector.Count = count;
			return count;
		}
		int ITask.CopyToBuffer<IN>(CircularBuffer<IN> input) {
			if (typeof(IN) != typeof(T))
				throw new Exception("wrong type of requested data, column : " + column);
			int count = input.Copy(vector.Data as IN[], vector.Data.Length);
			vector.Count = count;
			return count;
		}
		int ITask.RecordCount { get { return vector.Data.Length; } }
		int ITask.StartIndex { get { return startIndex; } }
		#endregion IStorage
	}
}
