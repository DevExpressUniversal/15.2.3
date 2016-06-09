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
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class ProjectOthersExecutor<T> : ExecutorBase<ProjectOthers> {
		DataVector<T> inputFlow;
		DataVector<bool> filterFlow;
		DataVector<T> resultFlow;
		public ProjectOthersExecutor(ProjectOthers operation, IExecutorQueueContext context)
			: base(operation, context) {
			inputFlow = (DataVector<T>)((SingleFlowExecutorResult)Context.GetExecutorResult(operation.InputFlow)).ResultVector;
			filterFlow = (DataVector<bool>)((SingleFlowExecutorResult)Context.GetExecutorResult(operation.OthersFlow)).ResultVector;
			AssertInputVectors();
			resultFlow = new DataVector<T>(context.VectorSize);
			this.Result = new SingleFlowExecutorResult(resultFlow);
		}
		void AssertInputVectors() {
			if (inputFlow.Count != filterFlow.Count)
				throw new ArgumentException("Input flow and filter flow counts are different");
		}
		protected override ExecutorProcessResult Process() {
			resultFlow.Count = 0;
			resultFlow.CopyFrom(inputFlow);
			for (int i = 0; i < filterFlow.Count; i++)
				if (filterFlow.Data[i] == Operation.OthersValue)
					resultFlow.SpecialData[i] = SpecialDataValue.Others;
			return ExecutorProcessResult.ResultReady;
		}
	}
}
