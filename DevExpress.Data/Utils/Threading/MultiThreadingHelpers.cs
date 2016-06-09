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
using System.Globalization;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Security;
namespace DevExpress.Data.Helpers {
#if DXRESTRICTED
	public static class StaSafeHelper {
		public static bool? DontTouchStackTrace;
		public static T Invoke<T>(Func<T> worker) {
			return worker();
		}
	}
#else
	public static class StaSafeHelper {
		static bool IsStaCurrentThread {
			get { return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA; }
		}
		class STASafeDataStoreExecutionState<T> {
			public volatile bool Ready;
			public Exception Error;
			public T Result;
		}
		public static bool? DontTouchStackTrace;
		public static T Invoke<T>(Func<T> worker) {
			if(!IsStaCurrentThread)
				return worker();
			STASafeDataStoreExecutionState<T> executionState = new STASafeDataStoreExecutionState<T>();
			TinyThreadPool.Enqueue(() => {
				try {
					executionState.Result = worker();
				} catch(Exception ex) {
					executionState.Error = ex;
				}
				executionState.Ready = true;
			});
			for(int sleepZeroCounter = 0; !executionState.Ready && sleepZeroCounter < 1000; ++sleepZeroCounter)
				Thread.Sleep(0);
			while(!executionState.Ready)
				Thread.Sleep(1);
			if(executionState.Error == null)
				return executionState.Result;
			if(DontTouchStackTrace == true)
				throw new System.Reflection.TargetInvocationException(executionState.Error);
			else
				throw executionState.Error;
		}
		public static void Invoke(Action worker) {
			Invoke(() => {
				worker();
				return (object)null;
			});
		}
		static class TinyThreadPool {   
			static readonly ConcurrentQueue<Action> Q = new ConcurrentQueue<Action>();
			static readonly AutoResetEvent SomethingInQ = new AutoResetEvent(false);
			static int spareThreads;
			static int theThreadRun;
			static void TheThread() {
				try {
					do {
						Core();
					} while(SomethingInQ.WaitOne(30000));
				} finally {
					Interlocked.Decrement(ref spareThreads);
					Interlocked.Decrement(ref theThreadRun);
				}
				if(!Q.IsEmpty)
					TryRunTheThread();
			}
			static void TryRunTheThread() {
				if(Interlocked.CompareExchange(ref theThreadRun, 1, 0) != 0)
					return;
				Interlocked.Increment(ref spareThreads);
				new Thread(TheThread) { IsBackground = true }.Start();
			}
			static void PooledThread(object useless) {
				try {
					Core();
				} finally {
					Interlocked.Decrement(ref spareThreads);
				}
			}
			static void Core() {
				Action action;
				while(Q.TryDequeue(out action)) {
					action();
					Interlocked.Increment(ref spareThreads);
				}
			}
			static void RunThreadPoolThread() {
				Interlocked.Increment(ref spareThreads);
				ThreadPool.QueueUserWorkItem(PooledThread);
			}
			public static void Enqueue(Action action) {
				Interlocked.Decrement(ref spareThreads);
				Q.Enqueue(action);
				SomethingInQ.Set();
				TryRunTheThread();
				if(Thread.VolatileRead(ref spareThreads) < 0) {
					RunThreadPoolThread();
				}
			}
		}
	}
#endif
	public class AvailableDegreeOfParallelismCalculator {
		static Lazy<int> maxDoP = new Lazy<int>(() => CalculateMaxDoP());
		static int CalculateMaxDoP() {
			int processors = Environment.ProcessorCount;
			int maybe_MAX_DOP = 64;
			for(; ; ) {
				if(processors <= maybe_MAX_DOP)
					return processors;
				int nextMaxDopToTest = checked(maybe_MAX_DOP * 2);
				try {
					ParallelEnumerable.Empty<object>().WithDegreeOfParallelism(nextMaxDopToTest);
				} catch(ArgumentOutOfRangeException) {
					return maybe_MAX_DOP;
				}
				maybe_MAX_DOP = nextMaxDopToTest;
			}
		}
		public static int GetMaxDoP() {
			return maxDoP.Value;
		}
		public static int? CoresToUseHint;
		static Lazy<int> realCores = new Lazy<int>(() => CoresToUseHint ?? GuessRealCoresStaSafe());
		static int GuessRealCoresStaSafe() {
			return StaSafeHelper.Invoke(() => GuessRealCores());
		}
		[SecuritySafeCritical]
		static int GuessRealCores() {
#if !DXPORTABLE
			try {
				if(AppDomain.CurrentDomain.IsFullyTrusted) {
					var data = new System.Management.ManagementObjectSearcher("select NumberOfCores from Win32_Processor").Get();
					int cores = 0;
					foreach(var processor in data) {
						cores += Convert.ToInt32(processor["NumberOfCores"]);
					}
					if(cores > 0)
						return cores;
				}
			} catch { }
#endif
			var envprocessors = Environment.ProcessorCount;
			if(envprocessors > 6)
				return envprocessors / 2;
			else
				return envprocessors;
		}
		public static int GetRealCoresToUse() {
			return realCores.Value;
		}
		public readonly int MaxDegreeToProbe;
		public int Result;
		int Waiting;
		int Alive;
		readonly ManualResetEvent Event = new ManualResetEvent(false);
		void Spawner(object useless) {
			if(Event.WaitOne(0)) {
				Event.Dispose();
			} else {
				Alive = MaxDegreeToProbe;
				for(int i = 0; i < MaxDegreeToProbe; ++i)
					ThreadPool.QueueUserWorkItem(Probe);
			}
		}
		void Probe(object useless) {
			int myNumber = Interlocked.Increment(ref Waiting);
			if(myNumber != 1) {
				Event.WaitOne();
			} else {
				if(!Event.WaitOne(60000)) {
					try {
						throw new Exception("!Event.WaitOne(60000) (may be orphaned AvailableDegreeOfParallelismCalculator?)");
					} catch { }
					Event.WaitOne();
				}
			}
			var probesLeft = Interlocked.Decrement(ref Alive);
			if(probesLeft == 0)
				Event.Dispose();
		}
		AvailableDegreeOfParallelismCalculator(int maxDegreeToProbe) {
			if(maxDegreeToProbe <= 1)
				throw new InvalidOperationException(string.Format("maxDegreeToProbe({0}) <= 1", maxDegreeToProbe));
			MaxDegreeToProbe = maxDegreeToProbe;
			ThreadPool.QueueUserWorkItem(Spawner);
		}
		public static AvailableDegreeOfParallelismCalculator Start(int maxDegreeToProbe) {
			return new AvailableDegreeOfParallelismCalculator(maxDegreeToProbe);
		}
		public void Finish() {
#if DXRESTRICTED
			Result = Waiting;
#else
			Result = Thread.VolatileRead(ref Waiting);
#endif
			Event.Set();
		}
	}
}
