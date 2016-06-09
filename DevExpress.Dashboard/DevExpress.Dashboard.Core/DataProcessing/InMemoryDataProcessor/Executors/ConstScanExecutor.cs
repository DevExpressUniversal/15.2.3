﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	class ConstScanExecutor<T> : ExecutorBase<ConstScan> {
		T value;
		DataVector<T> vector;
		int restCount;
		bool initialized;
		public ConstScanExecutor(ConstScan operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.value = (T)operation.Value;
			this.vector = new DataVector<T>(context.VectorSize);
			this.restCount = operation.Count;
			Result = new SingleFlowExecutorResult(vector);
		}
		protected override ExecutorProcessResult Process() {
			T[] data = vector.Data;
			if(!initialized) {
				for(int i = 0; i < vector.Data.Length; i++)
					data[i] = value;
				initialized = true;
			}
			vector.Count = Math.Min(vector.Data.Length, restCount);
			restCount -= vector.Count;
			if(restCount <= 0)
				return ExecutorProcessResult.EndOfFlow;
			else
				return ExecutorProcessResult.ResultReady;
		}
	}
}
