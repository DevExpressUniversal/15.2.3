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
	class BufferExecutor<T> : ExecutorBase<Buffer> {
		readonly BufferStorageBase bufferStorage;
		public BufferExecutor(Buffer operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.bufferStorage = BufferStorageBase.New(typeof(T), (SingleFlowExecutorResult)context.GetExecutorResult(operation.Argument));
			this.Result = new SingleFlowExecutorResult(bufferStorage.Buffer);
		}
		protected override ExecutorProcessResult Process() {
			bufferStorage.Process();
			return ExecutorProcessResult.ResultReady;
		}
	}
	abstract class BufferStorageBase {
		public abstract DataVectorBase Buffer { get; }
		public abstract void Process();
		public abstract void ReorderData(int[] indexes);
		public static BufferStorageBase New(Type bufferDataType, SingleFlowExecutorResult input) {
			return GenericActivator.New<BufferStorageBase>(typeof(BufferStorage<>), bufferDataType, input);
		}
	}
	class BufferStorage<T> : BufferStorageBase {
		readonly DataVector<T> inputVector;
		readonly DataVector<T> bufferVector;
		public override DataVectorBase Buffer { get { return bufferVector; } }
		public BufferStorage(SingleFlowExecutorResult input) {
			this.inputVector = (DataVector<T>)input.ResultVector;
			this.bufferVector = new DataVector<T>(DataProcessingOptions.DefaultBufferSize);
		}
		public override void Process() {
			bufferVector.CopyFrom(inputVector);
		}
		public override void ReorderData(int[] indexes) {
			DataVector<T> temp = (DataVector<T>)DataVectorBase.New(typeof(T), bufferVector.Count);
			T[] data = bufferVector.Data;
			SpecialDataValue[] specialData = bufferVector.SpecialData;
			temp.CopyFrom(bufferVector);
			for(int i = 0; i < indexes.Length; i++) {
				data[i] = temp.Data[indexes[i]];
				specialData[i] = temp.SpecialData[indexes[i]];
			}
		}
	}
}
