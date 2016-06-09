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
	public class ExecutorBenchmarks {
		static void Init(int[] values, int count) {
			int curr = 0;
			for(int i = 0; i < values.Length; i++) {
				values[i] = curr;
				curr++;
				if(curr >= count)
					curr = 0;
			}
		}
		static DataVector<T> CreateVector<T>(params object[] values) {
			DataVector<T> vector = new DataVector<T>(values.Length);
			vector.Count = values.Length;
			for(int i = 0; i < vector.Count; i++) {
				object value = values[i];
				if(value == null)
					vector.SpecialData[i] = SpecialDataValue.Null;
				else
					if(value is SpecialDataValue)
						vector.SpecialData[i] = (SpecialDataValue)value;
					else
						vector.Data[i] = (T)Convert.ChangeType(value, typeof(T));
			}
			return vector;
		}
		public void TestScan(params Scan[] scans) {
			ExecutorContext queueContext = new ExecutorContext();
			ScanExecutor<int>[] executors = scans.Select(s => new ScanExecutor<int>(s, queueContext)).ToArray();
			int cnt = 0;
			bool eof = false;
			while(!eof) {
				eof = true;
				for(int i = 0; i < executors.Length; i++)
					eof &= executors[i].ProcessVector() == ExecutorProcessResult.EndOfFlow;
				cnt++;
			}
			for(int i = 0; i < executors.Length; i++)
				executors[i].FinalizeExecutor();
		}
		public void TestSurrogateScan(params SurrogateScan[] scans) {
			ExecutorContext queueContext = new ExecutorContext();
			ScanExecutor<int>[] executors = scans.Select(s => new ScanExecutor<int>(s, queueContext)).ToArray();
			int cnt = 0;
			bool eof = false;
			while(!eof) {
				eof = true;
				for(int i = 0; i < executors.Length; i++)
					eof &= executors[i].ProcessVector() == ExecutorProcessResult.EndOfFlow;
				cnt++;
			}
			for(int i = 0; i < executors.Length; i++)
				executors[i].FinalizeExecutor();
		}
		public void CommonGroupBenchmark(int rowsCount, SingleFlowOperation[] operations, DataVectorBase[] vectors) {
			int vectorSize = DataProcessingOptions.DataVectorSize;
			int iterationsCount = rowsCount / DataProcessingOptions.DataVectorSize;
			Group group = new Group(operations);
			ExecutorContext queueContext = new ExecutorContext();
			for(int i = 0; i < operations.Length; i++)
				queueContext.AddExecutorResult(operations[i], new SingleFlowExecutorResult(vectors[i]));
			GroupExecutor executor = new GroupExecutor(group, queueContext);
			for(int i = 0; i < iterationsCount; i++)
				executor.ProcessVector();
			executor.FinalizeExecutor();
		}
		public void Group2IntBenchmark(int rowsCount) {
			int vectorSize = DataProcessingOptions.DataVectorSize;
			int iterationsCount = rowsCount / vectorSize;
			ConstScan scan1 = new ConstScan(typeof(int), 1, 2);
			ConstScan scan2 = new ConstScan(typeof(int), 1, 3);
			Group group = new Group(scan1, scan2);
			int[] values1 = new int[vectorSize];
			int[] values2 = new int[vectorSize];
			Init(values1, 1000);
			Init(values2, 1000);
			ExecutorContext queueContext = new ExecutorContext();
			queueContext.AddExecutorResult(scan1, new SingleFlowExecutorResult(CreateVector<int>(values1.Cast<object>().ToArray())));
			queueContext.AddExecutorResult(scan2, new SingleFlowExecutorResult(CreateVector<int>(values2.Cast<object>().ToArray())));
			GroupExecutor executor = new GroupExecutor(group, queueContext);
			for(int i = 0; i < iterationsCount; i++)
				executor.ProcessVector();
			executor.FinalizeExecutor();
		}
		public void TestAgg<T>(object[] values, SummaryType summType, int rowsCount) {
			int vectorSize = DataProcessingOptions.DataVectorSize;
			int iterationsCount = rowsCount / vectorSize;
			ConstScan scan = new ConstScan(typeof(T), 1, 2);
			GrandTotalAggregate agg = new GrandTotalAggregate(scan, summType);
			object[] vector = new object[vectorSize];
			for(int i = 0; i < vector.Length; i++)
				vector[i] = values[i % values.Length];
			ExecutorContext queueContext = new ExecutorContext();
			queueContext.AddExecutorResult(scan, new SingleFlowExecutorResult(CreateVector<T>(vector)));
			AggExecutor executor = new GrandTotalAggregateExecutor(agg, queueContext);
			for(int i = 0; i < iterationsCount; i++)
				executor.ProcessVector();
			executor.FinalizeExecutor();
		}
		public void TestJoin(int rowsCount) {
			int vectorSize = DataProcessingOptions.DataVectorSize;
			int iterationsCount = rowsCount / vectorSize;
			ConstScan scan1 = new ConstScan(typeof(int), 1, 2);
			ConstScan scan2 = new ConstScan(typeof(int), 1, 3);
			ConstScan scanData = new ConstScan(typeof(int), 1, 4);
			Buffer buf1 = new Buffer(scan1);
			Buffer buf2 = new Buffer(scan2);
			Buffer bufData = new Buffer(scanData);
			Group group = new Group(scan1, scan2);
			Join join = new Join(new SingleFlowOperation[] { scan1, scan2 }, new SingleBlockOperation[] { buf1, buf2 }, new SingleBlockOperation[] { bufData });
			int[] values1 = new int[vectorSize];
			int[] values2 = new int[vectorSize];
			Init(values1, vectorSize);
			Init(values2, vectorSize);
			ExecutorContext queueContext = new ExecutorContext();
			queueContext.AddExecutorResult(scan1, new SingleFlowExecutorResult(CreateVector<int>(values1.Cast<object>().ToArray())));
			queueContext.AddExecutorResult(scan2, new SingleFlowExecutorResult(CreateVector<int>(values2.Cast<object>().ToArray())));
			queueContext.AddExecutorResult(buf1, new SingleFlowExecutorResult(CreateVector<int>(values1.Cast<object>().ToArray())));
			queueContext.AddExecutorResult(buf2, new SingleFlowExecutorResult(CreateVector<int>(values2.Cast<object>().ToArray())));
			queueContext.AddExecutorResult(bufData, new SingleFlowExecutorResult(CreateVector<int>(values2.Cast<object>().ToArray())));
			JoinExecutor executor = new JoinExecutor(join, queueContext);
			for (int i = 0; i < iterationsCount; i++)
				executor.ProcessVector();
			executor.FinalizeExecutor();
		}
		public void TestProject(int rowsCount, Project project, DataVectorBase[] vectors) {
			int vectorSize = DataProcessingOptions.DataVectorSize;
			int iterationsCount = rowsCount / vectorSize;
			ExecutorContext queueContext = new ExecutorContext();
			var operands = project.Operands.ToArray();
			for(int i = 0; i < operands.Length; i++)
				queueContext.AddExecutorResult(operands[i], new SingleFlowExecutorResult(vectors[i]));
			ProjectExecutor executor = new ProjectExecutor(project, queueContext);
			for(int i = 0; i < iterationsCount; i++)
				executor.ProcessVector();
			executor.FinalizeExecutor();
		}
	}
}
