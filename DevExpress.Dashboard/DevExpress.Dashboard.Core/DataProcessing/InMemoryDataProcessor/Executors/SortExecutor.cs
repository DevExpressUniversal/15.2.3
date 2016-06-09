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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class SortExecutor : ExecutorBase<Sort> {
		readonly BufferStorageBase[] bufferStorages;
		readonly SortContainerBase[] activeSortContainer;
		public SortExecutor(Sort operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.bufferStorages = operation.Values.Select(d => BufferStorageBase.New(d.OperationType, context.GetSingleFlowExecutorResult(d))).ToArray();
			this.activeSortContainer = operation.SortBy.Select((bufferIndex, i) => SortContainerBase.New(bufferStorages[bufferIndex].Buffer, operation.SortOrder[i])).ToArray();
			this.Result = new MultiFlowExecutorResult(bufferStorages.Select(c => c.Buffer).ToArray());
		}
		protected override ExecutorProcessResult Process() {
			for(int i = 0; i < bufferStorages.Length; i++)
				bufferStorages[i].Process();
			return ExecutorProcessResult.ResultReady;
		}
		public override void FinalizeExecutor() {
			Sort();
		}
		void Sort() {
			int count = bufferStorages[0].Buffer.Count;
			int[] indexes = new int[count];
			for(int i = 0; i < indexes.Length; i++)
				indexes[i] = i;
			Array.Sort(indexes, CompareFunc);
			for(int i = 0; i < bufferStorages.Length; i++)
				bufferStorages[i].ReorderData(indexes);
		}
		int CompareFunc(int x, int y) {
			int result = 0;
			for(int i = 0; i < activeSortContainer.Length; i++) {
				result = activeSortContainer[i].Compare(x, y);
				if(result != 0)
					return result;
			}
			return result;
		}
	}
	abstract class SortContainerBase {
		public abstract int Compare(int index1, int index2);
		public static SortContainerBase New(DataVectorBase buffer, SortOrder sortOrder) {
			return GenericActivator.New<SortContainerBase>(typeof(SortContainer<>), buffer.DataType, buffer, sortOrder);
		}
	}
	class SortContainer<T> : SortContainerBase {
		readonly Comparer<T> comparer;
		readonly DataVector<T> buffer;
		readonly int orderMultiplier;
		public SortContainer(DataVectorBase buffer, SortOrder sortOrder) {
			this.buffer = (DataVector<T>)buffer;
			this.comparer = ComparerFactory.Get<T>();
			this.orderMultiplier = sortOrder == SortOrder.Ascending ? 1 : -1;
		}
		public override int Compare(int index1, int index2) {
			return CompareAsc(index1, index2) * orderMultiplier;
		}
		int CompareAsc(int index1, int index2) {
			SpecialDataValue spec1 = buffer.SpecialData[index1];
			SpecialDataValue spec2 = buffer.SpecialData[index2];
			if(spec1 == SpecialDataValue.None && spec2 == SpecialDataValue.None)
				return comparer.Compare(buffer.Data[index1], buffer.Data[index2]);
			return GetSpecialValueWeight(spec1) - GetSpecialValueWeight(spec2);
		}
		int GetSpecialValueWeight(SpecialDataValue special) {
			if(special == SpecialDataValue.Error)
				return 1;
			if(special == SpecialDataValue.Null)
				return 2;
			if(special == SpecialDataValue.None)
				return 3;
			if(special == SpecialDataValue.Others)
				return 4;
			throw new NotSupportedException();
		}
	}
}
