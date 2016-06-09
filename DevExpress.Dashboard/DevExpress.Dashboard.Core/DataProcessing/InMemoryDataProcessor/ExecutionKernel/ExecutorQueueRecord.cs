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
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class QueueResultEntry {
		public CustomOperation Operation { get; private set; }
		public ExecutorResultBase Result { get; private set; }
		public QueueResultEntry(CustomOperation operation, ExecutorResultBase result) {
			this.Operation = operation;
			this.Result = result;
		}
	}
	class ExecutorQueueRecord {
		enum ExecutorState { WaitInput, ReadyForExecute, WaitReaders }
		ExecutorState state;
		ExecutorBase executor;
		List<ExecutorQueueRecord> prevRecords;
		List<ExecutorQueueRecord> nextRecords;
		int nextRecordExecutedCount = 0;
		int prevRecordExecutedCount = 0;
		public CustomOperation Operation { get { return executor.CustomOperation; } }
		public QueueResultEntry ResultEntry { get { return new QueueResultEntry(Operation, executor.Result); } }
		public ExecutorQueueRecord(ExecutorBase executor, List<ExecutorQueueRecord> prevRecords) {
			this.prevRecords = prevRecords;
			this.nextRecords = new List<ExecutorQueueRecord>();
			this.executor = executor;
			this.state = prevRecords.Count == 0 ? ExecutorState.ReadyForExecute : ExecutorState.WaitInput;
			foreach(ExecutorQueueRecord record in prevRecords)
				record.RegisterNextRecord(this);
		}
		public bool Execute() {
			if(state == ExecutorState.ReadyForExecute) {
				ExecutorProcessResult result = executor.ProcessVector();
				NotifyPrevRecords();
				if(result == ExecutorProcessResult.ResultNotReady) {
					state = ExecutorState.WaitInput;
				} else {
					state = ExecutorState.WaitReaders;
					NotifyNextRecords();
				}
				TryMoveState();
			}
			return executor.IsFinished;
		}
		public void FinalizeExecutor() {
			executor.FinalizeExecutor();
		}
		void NotifyNextRecords() {
			foreach(ExecutorQueueRecord record in nextRecords)
				record.PrevRecordExecuted();
		}
		void NotifyPrevRecords() {
			foreach(ExecutorQueueRecord record in prevRecords)
				record.NextRecordExecuted();
		}
		void RegisterNextRecord(ExecutorQueueRecord record) {
			nextRecords.Add(record);
		}
		void NextRecordExecuted() {
			if(nextRecordExecutedCount >= nextRecords.Count)
				throw new Exception("Executors queue is invalid.");
			nextRecordExecutedCount++;
			TryMoveState();
		}
		void PrevRecordExecuted() {
			if(prevRecordExecutedCount >= prevRecords.Count)
				throw new Exception("Executors queue is invalid.");
			prevRecordExecutedCount++;
			TryMoveState();
		}
		void TryMoveState() {
			bool isStateInvalid = true;
			while(isStateInvalid)
				switch(state) {
					case ExecutorState.WaitInput:
						isStateInvalid = prevRecords.Count == prevRecordExecutedCount;
						if(isStateInvalid) {
							state = ExecutorState.ReadyForExecute;
							prevRecordExecutedCount = 0;
						}
						break;
					case ExecutorState.ReadyForExecute:
						isStateInvalid = false;
						break;
					case ExecutorState.WaitReaders:
						isStateInvalid = nextRecords.Count == nextRecordExecutedCount;
						if(isStateInvalid) {
							state = ExecutorState.WaitInput;
							nextRecordExecutedCount = 0;
						}
						break;
				}
		}
	}
}
