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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	abstract class TopNExecutorBase<T> : ExecutorBase<TopNBase> {
		int inputValueIndex = 0;
		protected int TopNCount { get; set; }
		protected DataVector<T> InputFlow { get; set; }
		protected DataVector<bool> ResultFlow { get; set; }
		protected BinaryHeap<T> BinaryHeap { get; set; }
		public TopNExecutorBase(TopNBase operation, IExecutorQueueContext context) : base(operation, context) {}
		protected override ExecutorProcessResult Process() {
			for (int i = 0; i < InputFlow.Count; i++) {
				if (InputFlow.SpecialData[i] == SpecialDataValue.None || InputFlow.SpecialData[i] == SpecialDataValue.Null)
					BinaryHeap.Process(InputFlow.Data[i], inputValueIndex++);
			}
			return ExecutorProcessResult.ResultReady;
		}
		public override void FinalizeExecutor() {
			base.FinalizeExecutor();
			if (inputValueIndex > ResultFlow.MaxCount)
				ResultFlow.Extend(inputValueIndex);
			for (int i = 0; i < BinaryHeap.Indexes.Length; i++)
				ResultFlow.Data[BinaryHeap.Indexes[i]] = true;
			ResultFlow.Count = inputValueIndex;
		}
	}
	class TopNExecutor<T> : TopNExecutorBase<T> {
		public TopNExecutor(TopN operation, IExecutorQueueContext context)
			: base(operation, context) {
			InputFlow = (DataVector<T>)((SingleFlowExecutorResult)Context.GetExecutorResult(operation.InputFlow)).ResultVector;
			ResultFlow = new DataVector<bool>(InputFlow.Count);
			TopNCount = operation.TopNCount;
			BinaryHeap = new BinaryHeap<T>(TopNCount, operation.Mode, ComparerFactory.Get<T>());
			this.Result = new SingleFlowExecutorResult(ResultFlow);
		}
	}
	class TopNPercentExecutor<T> : TopNExecutorBase<T> {
		public TopNPercentExecutor(TopNPercent operation, IExecutorQueueContext context)
			: base(operation, context) {
			InputFlow = (DataVector<T>)((SingleFlowExecutorResult)Context.GetExecutorResult(operation.InputFlow)).ResultVector;
			ResultFlow = new DataVector<bool>(InputFlow.Count);
			TopNCount = (int)(operation.Percent * InputFlow.Count);
			BinaryHeap = new BinaryHeap<T>(TopNCount, operation.Mode, ComparerFactory.Get<T>());
			this.Result = new SingleFlowExecutorResult(ResultFlow);
		}
	}
	class BinaryHeap<T> {
		#region inner classes
		class TopValueComparer<K> : IComparer<K> {
			Comparer<K> baseComparer;
			public TopValueComparer(Comparer<K> baseComparer) {
				this.baseComparer = baseComparer;
			}
			public int Compare(K x, K y) {
				return baseComparer.Compare(x, y);
			}
		}
		class BottomValueComparer<K> : IComparer<K> {
			Comparer<K> baseComparer;
			public BottomValueComparer(Comparer<K> baseComparer) {
				this.baseComparer = baseComparer;
			}
			public int Compare(K x, K y) {
				return baseComparer.Compare(y, x);
			}
		}
		#endregion
		List<int> indexes;
		List<T> data;
		int count;
		IComparer<T> comparer;		
		public int[] Indexes { get { return indexes.ToArray(); } }
		public BinaryHeap(int count, TopNMode mode, Comparer<T> baseComparer) {
			indexes = new List<int>(count);
			data = new List<T>(count);
			this.count = count;
			comparer = mode == TopNMode.Top ? (IComparer<T>)new TopValueComparer<T>(baseComparer) : new BottomValueComparer<T>(baseComparer);			
		}
		void Swap(int index1, int index2) {
			T buf = data[index1];
			data[index1] = data[index2];
			data[index2] = buf;
			int bufIndex = indexes[index1];
			indexes[index1] = indexes[index2];
			indexes[index2] = bufIndex;
		}
		public void Process(T inputValue, int index) {
			if (count > 0)
				if (data.Count < count) {
					int position = data.BinarySearch(inputValue, comparer);
					if (position < 0)
						position = ~position;
					data.Insert(position, inputValue);
					indexes.Insert(position, index);
				}
				else if (comparer.Compare(inputValue, data[0]) > 0) {
					data[0] = inputValue;
					indexes[0] = index;
					int currentIndex = 0;
					while (true) {
						int nextIndex = currentIndex * 2 + 2;
						nextIndex = nextIndex < count && comparer.Compare(data[nextIndex - 1], data[nextIndex]) > 0 ? nextIndex : nextIndex - 1;
						if (nextIndex < count && comparer.Compare(data[currentIndex], data[nextIndex]) > 0) {
							Swap(currentIndex, nextIndex);
							currentIndex = nextIndex;
						}
						else
							break;
					}
				}
		}
	}	
}
