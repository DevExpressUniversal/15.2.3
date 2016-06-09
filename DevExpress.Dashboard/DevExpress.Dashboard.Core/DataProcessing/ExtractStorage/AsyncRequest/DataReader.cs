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
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class DataReader : IThreadObject {
		Thread selfThread;
		bool isAlive = true;
		QueueWithWaiting<ReadTask> inQueue;
		QueueWithWaiting<ReadTask> outQueue;
		string path;
		internal string Path { get { return path; } }
		public QueueWithWaiting<ReadTask> InQueue { get { return inQueue; } }
		public QueueWithWaiting<ReadTask> OutQueue { get { return outQueue; } }
		protected DataReader(string path) {
			this.path = path;
			inQueue = new QueueWithWaiting<ReadTask>(100);
			outQueue = new QueueWithWaiting<ReadTask>(100);
			selfThread = new Thread(new ThreadStart(Loop));
		}
		internal void Loop() {
			while (isAlive) {
				ReadTask task;
				if (!InQueue.WaitAndPull(out task))
					continue;
				ReadFromFile(task.Offset, task.LengthInBytes, task.Buffer);
				task.IsReady = true;
				OutQueue.Push(task);
			}
		}
		internal abstract void ReadFromFile(int offsetInFile, int count, ByteBuffer circularBuffer);
		#region IThreadObject
		void IThreadObject.Start() {
			selfThread.Start();
		}
		void IThreadObject.Stop() {
			inQueue.Stop();
			isAlive = false;
			InternalStop();
		}
		bool IThreadObject.Join(int millisecondsTimeout) {
			return selfThread.Join(millisecondsTimeout);
		}
		#endregion IThreadObject
		protected virtual void InternalStop() { }
	}
	public class FileReader : DataReader {
		public FileReader(string path) : base(path) { }
		internal override void ReadFromFile(int offsetInFile, int count, ByteBuffer circularBuffer) {
			using (FileStream fs = new FileStream(this.Path, FileMode.OpenOrCreate)) {
				circularBuffer.Push(fs, offsetInFile, count);
			}
		}
	}
	public class MemoryReader : DataReader {
		MemoryMappedFile mmf;
		public MemoryReader(string path)
			: base(path) {
			mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
		}
		public MemoryReader(MemoryMappedFile file)
			: base("") {
			mmf = file;
		}
		protected override void InternalStop() {
			if (mmf != null)
				mmf.Dispose();
		}
		internal override void ReadFromFile(int offsetInFile, int count, ByteBuffer circularBuffer) {
			using (MemoryMappedViewStream stream = mmf.CreateViewStream(offsetInFile, count)) {
				circularBuffer.Push(stream, offsetInFile, count);
			}
		}
	}
	public class ReadTask {
		public bool IsReady { get; set; }
		public string ColumnName { get; set; }
		public int Offset { get; set; }
		public int LengthInBytes { get; set; }
		public int StartIndex { get; set; }
		public int RecordsCount { get; set; }
		public ByteBuffer Buffer { get; set; }
	}
	public class ReadTaskComparer : IComparer<ReadTask> {
		public int Compare(ReadTask x, ReadTask y) {
			if (x.Offset == y.Offset)
				return 0;
			return x.Offset < y.Offset ? 1 : -1;
		}
	}
}
