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
	static class ExecutorFactory {
		public static ExecutorBase Create(CustomOperation operation, IExecutorQueueContext context) {
			if(Is<SelectScanBase>(operation))
				return New((SelectScanBase)operation, context);
			if(Is<ScanBase>(operation))
				return New((ScanBase)operation, context);
			if(Is<ConstScan>(operation))
				return New((ConstScan)operation, context);
			if(Is<Buffer>(operation))
				return New((Buffer)operation, context);
			if(Is<Project>(operation))
				return New((Project)operation, context);
			if(Is<ProjectDateTime>(operation))
				return New((ProjectDateTime)operation, context);
			if(Is<Select>(operation))
				return New((Select)operation, context);
			if(Is<Extract>(operation))
				return New((Extract)operation, context);
			if(Is<BlockExtract>(operation))
				return New((BlockExtract)operation, context);
			if(Is<Group>(operation))
				return New((Group)operation, context);
			if(Is<MultiScanBuffer>(operation))
				return New((MultiScanBuffer)operation, context);
			if(Is<ScanBuffer>(operation))
				return New((ScanBuffer)operation, context);
			if(Is<GroupAggregate>(operation))
				return New((GroupAggregate)operation, context);
			if(Is<GrandTotalAggregate>(operation))
				return New((GrandTotalAggregate)operation, context);
			if(Is<Materialize>(operation))
				return New((Materialize)operation, context);
			if(Is<Sort>(operation))
				return New((Sort)operation, context);
			if(Is<Join>(operation))
				return New((Join)operation, context);
			if(Is<TopN>(operation))
				return New((TopN)operation, context);
			if(Is<TopNPercent>(operation))
				return New((TopNPercent)operation, context);
			if(Is<ProjectOthers>(operation))
				return New((ProjectOthers)operation, context);
			if(Is<ExtractIndexes>(operation))
				return New((ExtractIndexes)operation, context);
			if(Is<JoinIndexes>(operation))
				return New((JoinIndexes)operation, context);
			if(Is<ConvertType>(operation))
				return New((ConvertType)operation, context);
			throw new NotSupportedException();
		}
		static bool Is<T>(CustomOperation operation) { 
			return operation is T;
		}
		static ExecutorBase New(ConstScan operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(ConstScanExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(ScanBase operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(ScanExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(SelectScanBase operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(SelectScanExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(Extract operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(ExtractExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(BlockExtract operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(BlockExtractExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(Buffer operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(BufferExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(Group operation, IExecutorQueueContext context) {
			return new GroupExecutor(operation, context);
		}
		static ExecutorBase New(Project operation, IExecutorQueueContext context) {
			return new ProjectExecutor(operation, context);
		}
		static ExecutorBase New(ProjectDateTime operation, IExecutorQueueContext context) {
			return new ProjectDateTimeExecutor(operation, context);
		}
		static ExecutorBase New(Select operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(SelectExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(MultiScanBuffer operation, IExecutorQueueContext context) {
			return new MultiScanBufferExecutor(operation, context);
		}
		static ExecutorBase New(ScanBuffer operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(ScanBufferExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(GrandTotalAggregate operation, IExecutorQueueContext context) {
			return new GrandTotalAggregateExecutor(operation, context);
		}
		static ExecutorBase New(GroupAggregate operation, IExecutorQueueContext context) {
			return new GroupAggregateExecutor(operation, context);
		}
		static ExecutorBase New(Materialize operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(MaterializeExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(Sort operation, IExecutorQueueContext context) {
			return new SortExecutor(operation, context);
		}
		static ExecutorBase New(Join operation, IExecutorQueueContext context) {
			return new JoinExecutor(operation, context);
		}
		static ExecutorBase New(TopN operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(TopNExecutor<>), operation.InputFlow.OperationType, operation, context);
		}
		static ExecutorBase New(TopNPercent operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(TopNPercentExecutor<>), operation.InputFlow.OperationType, operation, context);
		}
		static ExecutorBase New(ProjectOthers operation, IExecutorQueueContext context) {
			return GenericActivator.New<ExecutorBase>(typeof(ProjectOthersExecutor<>), operation.OperationType, operation, context);
		}
		static ExecutorBase New(ExtractIndexes operation, IExecutorQueueContext context) {
			return new ExtractIndexesExecutor(operation, context);
		}
		static ExecutorBase New(JoinIndexes operation, IExecutorQueueContext context) {
			return new JoinIndexesExecutor(operation, context);
		}
		static ExecutorBase New(ConvertType operation, IExecutorQueueContext context) {
			return new ConvertTypeExecutor(operation, context);
		}
	}
}
