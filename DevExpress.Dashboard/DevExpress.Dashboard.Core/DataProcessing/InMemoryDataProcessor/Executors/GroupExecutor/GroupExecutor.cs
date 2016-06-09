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
	class GroupExecutorResult : MultiFlowExecutorResult {
		public DataVector<int> GroupIndexes { get; private set; }
		public int GroupCount { get; set; }
		public GroupExecutorResult(DataVectorBase[] groupedData, DataVector<int> groupIndexes)
			: base(groupedData) {
			this.GroupIndexes = groupIndexes;
		}
	}
	class GroupExecutor : ExecutorBase<Group> {
		GroupWorkerBase worker;
		GroupExecutorResult groupResult;
		public GroupExecutor(Group operation, IExecutorQueueContext context)
			: base(operation, context) {
			DataVectorBase[] inputVectors = operation.Dimensions.Select(d => context.GetSingleFlowExecutorResultVector(d)).ToArray();
			bool isIntTypedInput = inputVectors.All(v => v.DataType == typeof(int));
			if(isIntTypedInput)
				this.worker = new IntGroupWorker(inputVectors);
			else
				this.worker = new AnyTypeGroupWorker(inputVectors);
			this.groupResult = new GroupExecutorResult(worker.GroupedData, worker.GroupIndexes);
			this.Result = groupResult;
		}
		protected override ExecutorProcessResult Process() {
			worker.Process();
			groupResult.GroupCount = worker.GroupCount;
			return ExecutorProcessResult.ResultReady;
		}
	}
}
