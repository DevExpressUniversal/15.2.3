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
	class SelectExecutor<T> : ExecutorBase<Select> {
		DataVector<T> vector;
		DataVector<bool> filter;
		DataVector<T> currentData;
		DataVector<T> currentDataCopy;
		DataVector<T> previousData;
		int previousDataCount;
		public SelectExecutor(Select operation, IExecutorQueueContext context) : base(operation, context) {
			this.vector = new DataVector<T>(context.VectorSize);
			this.currentData = (DataVector<T>)((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.ValuesFlow)).ResultVector;
			this.currentDataCopy = new DataVector<T>(0);
			previousData = new DataVector<T>(context.VectorSize);
			previousDataCount = 0;
			filter = (DataVector<bool>)((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.FilterFlow)).ResultVector;
			Result = new SingleFlowExecutorResult(vector);
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
				vector.Data[i] = previousData.Data[i];
				vector.SpecialData[i] = previousData.SpecialData[i];
			}
			vector.Count = previousDataCount;
		}
		protected override ExecutorProcessResult Process() {
			if (currentData.Count > 0) {
				currentDataCopy.CopyFrom(0, currentData, 0, currentData.Count);
				if (filter.Count != currentDataCopy.Count)
					throw new ArgumentException("Filter length should be equal data column length");
				FilterDataVector(currentDataCopy);
				int countToCopy = AppendPreviousData(currentDataCopy);
				FillResultVector();
				if (previousDataCount == Context.VectorSize) {
					previousDataCount = 0;
					if (countToCopy < currentDataCopy.Count) {
						previousData.CopyFrom(previousDataCount, currentDataCopy, countToCopy, currentDataCopy.Count - countToCopy);
						previousDataCount = currentDataCopy.Count - countToCopy;
					}
				}
			}
			else {
				FillResultVector();
				return ExecutorProcessResult.EndOfFlow;
			}
			if (vector.Count < Context.VectorSize)
				return ExecutorProcessResult.ResultNotReady;
			else
				return ExecutorProcessResult.ResultReady;
		}
	}
}
