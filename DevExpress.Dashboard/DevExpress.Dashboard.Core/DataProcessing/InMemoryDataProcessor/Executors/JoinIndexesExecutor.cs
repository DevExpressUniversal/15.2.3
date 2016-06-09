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

namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class JoinIndexesExecutor : ExecutorBase<JoinIndexes> {
		DataVector<int> group1IndexesFlow;
		DataVector<int> group2IndexesFlow;
		DataVector<int> resultFlow;
		public JoinIndexesExecutor(JoinIndexes operation, IExecutorQueueContext context) : base(operation, context) {
			this.group1IndexesFlow = (DataVector<int>)((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Group1Indexes)).ResultVector;
			this.group2IndexesFlow = (DataVector<int>)((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Group2Indexes)).ResultVector;
			resultFlow = new DataVector<int>(group1IndexesFlow.MaxCount);
			this.Result = new SingleFlowExecutorResult(resultFlow);
		}
		protected override ExecutorProcessResult Process() {			
			for (int i = 0; i < group1IndexesFlow.Count; i++)
				resultFlow.Data[i] = group2IndexesFlow.Data[group1IndexesFlow.Data[i]];
			resultFlow.Count = group1IndexesFlow.Count;
			return ExecutorProcessResult.ResultReady;
		}
	}
}
