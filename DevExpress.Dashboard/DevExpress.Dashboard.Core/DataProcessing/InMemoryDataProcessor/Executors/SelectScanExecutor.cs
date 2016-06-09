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
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class SelectScanExecutor<T> :  ScanExecutorBase<T> {
		DataVector<bool> filter;
		DataVector<T> currentData;
		DataVector<T> previousData;
		int previousDataCount;
		new SelectScanBase Operation { get { return (SelectScanBase)base.Operation; } }
		public SelectScanExecutor(SelectScanBase operation, IExecutorQueueContext context) : base(operation, context) {
			currentData = new DataVector<T>(Vector.MaxCount);
			previousData = new DataVector<T>(context.VectorSize);
			previousDataCount = 0;
			filter = (DataVector<bool>)((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.FilterFlow)).ResultVector;
		}
		void FilterDataVector(DataVector<T> currentData) {
			int filteredDataCount = 0;
			for (int i = 0; i < currentData.Count; i++)
				if (filter.Data[i]) {
					currentData.Data[filteredDataCount] = currentData.Data[i];
					currentData.SpecialData[filteredDataCount] = currentData.SpecialData[i];
					filteredDataCount++;
				}
			currentData.Count = filteredDataCount;
		}
		int AppendPreviousData(DataVector<T> currentData) {
			int countToCopy = Math.Min(currentData.Count, Context.VectorSize - previousDataCount);
			previousData.CopyFrom(previousDataCount, currentData, 0, countToCopy);
			previousDataCount += countToCopy;
			return countToCopy;
		}
		void FillResultVector() {
			for (int i = 0; i < previousDataCount; i++) {
				Vector.Data[i] = previousData.Data[i];
				Vector.SpecialData[i] = previousData.SpecialData[i];
			}
			Vector.Count = previousDataCount;
		}
		protected override ExecutorProcessResult Process() {
			if (!InputFlow.IsEnded) {
				if (Operation is SelectScan)
					InputFlow.NextMaterialized(currentData);
				else if (Operation is SurrogateSelectScan)
					InputFlow.NextSubstitutes((IDataVector<int>)currentData);
				if (filter.Count != currentData.Count)
					throw new ArgumentException("Filter length should be equal data column length");
				FilterDataVector(currentData);
				int countToCopy = AppendPreviousData(currentData);
				FillResultVector();
				if (previousDataCount == Context.VectorSize || InputFlow.IsEnded) {
					previousDataCount = 0;
					if (countToCopy < currentData.Count) {
						previousData.CopyFrom(previousDataCount, currentData, countToCopy, currentData.Count - countToCopy);
						previousDataCount = currentData.Count - countToCopy;
					}
				}
			}
			else {
				FillResultVector();
				return ExecutorProcessResult.EndOfFlow;
			}
			if (Vector.Count < Context.VectorSize)				
				return InputFlow.IsEnded ? ExecutorProcessResult.EndOfFlow : ExecutorProcessResult.ResultNotReady;
			else
				return ExecutorProcessResult.ResultReady;
		}
	}
}
