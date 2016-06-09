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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class JoinExecutor : ExecutorBase<Join> {
		DataVectorBase criteria1ResultVector;
		DataVectorBase criteria2ResultVector;
		List<DataVectorBase> dataFlow = new List<DataVectorBase>();
		DataVectorBase[] resultFlow;
		List<JoinGroupedColumnBase> criteriaColumns;
		Dictionary<JoinDataTuple, int> criteria2Dictionary;
		public JoinExecutor(Join operation, IExecutorQueueContext context) : base(operation, context) {
			AssertInputVectors();
			PrepareSearchData();
			PrepareResultFlow();
			this.Result = new MultiFlowExecutorResult(resultFlow);
		}
		void AssertInputVectors() { 
			if (Operation.Criteria2.Length != Operation.Criteria1.Length)
				throw new ArgumentException("Join criteria counts are different");
			for (int i = 0; i < Operation.Criteria1.Length; i++)
				if (Operation.Criteria1[i].OperationType != Operation.Criteria2[i].OperationType)
					throw new ArgumentException("Join criteria types are different");
		}
		void PrepareSearchData() {
			criteriaColumns = new List<JoinGroupedColumnBase>();
			if (Operation.Criteria1.Length > 0)
				criteria1ResultVector = ((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Criteria1[0])).ResultVector;
			if (Operation.Criteria2.Length > 0)
				criteria2ResultVector = ((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Criteria2[0])).ResultVector;
			for (int i = 0; i < Operation.Criteria1.Length; i++) {
				DataVectorBase criteria1Result = ((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Criteria1[i])).ResultVector;
				DataVectorBase criteria2Result = ((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.Criteria2[i])).ResultVector;
				JoinGroupedColumnBase column = NewColumn(criteria1Result, criteria2Result);
				criteriaColumns.Add(column);
			}
			JoinDataTupleEqualityComparer comparer = new JoinDataTupleEqualityComparer(criteriaColumns.ToArray());
			criteria2Dictionary = new Dictionary<JoinDataTuple, int>(comparer);
			for (int i = 0; i < criteria2ResultVector.Count; i++)
				criteria2Dictionary.Add(new JoinDataTuple(i), i);			
		}
		void PrepareResultFlow() {
			resultFlow = new DataVectorBase[Operation.DataFlow.Length];
			for (int j = 0; j < Operation.DataFlow.Length; j++) {
				DataVectorBase dataVector = ((SingleFlowExecutorResult)Context.GetExecutorResult(Operation.DataFlow[j])).ResultVector;
				dataFlow.Add(dataVector);
				resultFlow[j] = NewResultVector(Operation.DataFlow[j].OperationType, criteria1ResultVector.MaxCount);				
			}
		}
		DataVectorBase NewResultVector(Type type, int length) {
			return GenericActivator.New<DataVectorBase>(typeof(DataVector<>), type, length);
		}
		JoinGroupedColumnBase NewColumn(DataVectorBase vectorCriteria1, DataVectorBase vectorCriteria2) {
			return GenericActivator.New<JoinGroupedColumnBase>(typeof(JoinGroupedColumn<>), vectorCriteria1.DataType, vectorCriteria1, vectorCriteria2);
		}
		protected override ExecutorProcessResult Process() {
			for (int j = 0; j < dataFlow.Count; j++) {
				resultFlow[j].Count = 0;
				if (criteria1ResultVector.Count > resultFlow[j].MaxCount)
					resultFlow[j].Extend(criteria1ResultVector.Count);
			}
			for (int i = 0; i < criteria1ResultVector.Count; i++) {
				int findRowIndex;
				if (criteria2Dictionary.TryGetValue(new JoinDataTuple(i, true), out findRowIndex)) {
					for (int j = 0; j < dataFlow.Count; j++) {
						resultFlow[j].CopyValueFrom(i, dataFlow[j], findRowIndex);
						resultFlow[j].Count++;
					}
				}
				else
					throw new ArgumentException("Join criteria not found");
			}
			return ExecutorProcessResult.ResultReady;
		}
	}
	abstract class JoinGroupedColumnBase {
		public abstract int ValueHashCode(int index);
		public abstract int TestValueHashCode(int index);
		public abstract bool ValueEquals(int index1, int index2);
		public abstract bool TestValueEquals(int index1, int index2);
	}
	class JoinGroupedColumn<T> : JoinGroupedColumnBase {
		EqualityComparer<T> comparer;
		DataVector<T> criteria1Vector;
		DataVector<T> criteria2Vector;
		public JoinGroupedColumn(DataVector<T> criteria1Vector, DataVector<T> criteria2Vector) {
			this.criteria1Vector = criteria1Vector;
			this.criteria2Vector = criteria2Vector;
			this.comparer = EqualityComparerFactory.Get<T>();
		}
		public override int ValueHashCode(int index) {
			SpecialDataValue specialValue = criteria2Vector.SpecialData[index];
			if (specialValue == SpecialDataValue.None)
				return comparer.GetHashCode(criteria2Vector.Data[index]);
			else
				return specialValue.GetHashCode();
		}
		public override int TestValueHashCode(int index) {
			SpecialDataValue specialValue = criteria1Vector.SpecialData[index];
			if (specialValue == SpecialDataValue.None)
				return comparer.GetHashCode(criteria1Vector.Data[index]);
			else
				return specialValue.GetHashCode();
		}
		public override bool ValueEquals(int index1, int index2) {
			SpecialDataValue spec1 = criteria2Vector.SpecialData[index1];
			SpecialDataValue spec2 = criteria2Vector.SpecialData[index2];
			if (spec1 == SpecialDataValue.None && spec2 == SpecialDataValue.None)
				return comparer.Equals(criteria2Vector.Data[index1], criteria2Vector.Data[index2]);
			else
				return spec1 == spec2;
		}
		public override bool TestValueEquals(int index1, int index2) {
			SpecialDataValue spec1 = criteria2Vector.SpecialData[index1];
			SpecialDataValue spec2 = criteria1Vector.SpecialData[index2];
			if (spec1 == SpecialDataValue.None && spec2 == SpecialDataValue.None)
				return comparer.Equals(criteria2Vector.Data[index1], criteria1Vector.Data[index2]);
			else
				return spec1 == spec2;
		}
	}
	class JoinDataTupleEqualityComparer : IEqualityComparer<JoinDataTuple> {
		int columnsCount;
		JoinGroupedColumnBase[] columns;
		public JoinDataTupleEqualityComparer(JoinGroupedColumnBase[] columns) {
			this.columns = columns;
			this.columnsCount = columns.Length;
		}
		public bool Equals(JoinDataTuple x, JoinDataTuple y) {
			for (int i = 0; i < columnsCount; i++) {
				bool equals = y.IsTestData ? columns[i].TestValueEquals(x.Index, y.Index) : columns[i].ValueEquals(x.Index, y.Index);
				if (!equals)
					return false;
			}
			return true;
		}
		public int GetHashCode(JoinDataTuple tuple) {
			unchecked {
				int FNV_prime_32 = 16777619;
				int FNV_offset_basis_32 = (int)2166136261;
				int hash = FNV_offset_basis_32;
				for (int i = 0; i < columnsCount; i++) {
					int item = tuple.IsTestData ? columns[i].TestValueHashCode(tuple.Index) : columns[i].ValueHashCode(tuple.Index);
					hash = (hash ^ item) * FNV_prime_32;  
				}
				return hash;
			}
		}
	}
	struct JoinDataTuple {
		int index;
		bool isTestData;
		public int Index { get { return index; } }
		public bool IsTestData { get { return isTestData; } }
		public JoinDataTuple(int index, bool isTestData = false) {
			this.index = index;
			this.isTestData = isTestData;
		}
		public override int GetHashCode() {
			throw new NotSupportedException();
		}
		public override bool Equals(object obj) {
			throw new NotSupportedException();
		}
	}
}
