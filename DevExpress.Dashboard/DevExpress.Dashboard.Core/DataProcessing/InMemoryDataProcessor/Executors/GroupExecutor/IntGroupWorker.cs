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
	class IntGroupWorker : GroupWorkerBase {
		int groupCount;
		DataVector<int> groupIndexes;
		DataVector<int>[] inputVectors;
		DataVector<int>[] resultVectors;
		DataVectorBase firstInputVector;
		Dictionary<DataTuple, int> groups;
		public override DataVectorBase[] GroupedData { get { return resultVectors; } }
		public override DataVector<int> GroupIndexes { get { return groupIndexes; } }
		public override int GroupCount { get { return groupCount; } }
		public IntGroupWorker(DataVectorBase[] inputVectors) {
			this.groupCount = 0;
			this.firstInputVector = inputVectors[0];
			this.inputVectors = inputVectors.Cast<DataVector<int>>().ToArray();
			this.groupIndexes = new DataVector<int>(DataProcessingOptions.DefaultGroupIndexesSize);
			this.resultVectors = inputVectors.Select(c => new DataVector<int>(DataProcessingOptions.DefaultGroupStorageCapacity)).ToArray();
			this.groups = new Dictionary<DataTuple, int>(new DataTupleIntVectorEqualityComparer(this.resultVectors));
		}
		public override void Process() {
			ProcessDirect();
		}
		public void ProcessDirect() {
			for(int j = 0; j < resultVectors.Length; j++) {
				int required = resultVectors[j].Count + inputVectors[j].Count;
				if(resultVectors[j].MaxCount < required)
					resultVectors[j].Extend(required);
			}
			for(int i = 0; i < firstInputVector.Count; i++) {
				int currentGroupIndex;
				int testValueIndex = groupCount;
				CopyTestValue(i, testValueIndex);
				DataTuple testTuple = new DataTuple(testValueIndex);
				if(!groups.TryGetValue(testTuple, out currentGroupIndex)) {
					currentGroupIndex = groupCount;
					groups.Add(testTuple, currentGroupIndex);
					groupCount++;
				}
				if(groupIndexes.Count == groupIndexes.MaxCount)
					groupIndexes.Extend();
				groupIndexes.Data[groupIndexes.Count] = currentGroupIndex;
				groupIndexes.Count++;
			}
			for(int j = 0; j < inputVectors.Length; j++)
				resultVectors[j].Count = groupCount;
		}
		void CopyTestValue(int inputPosition, int resultPosition) {
			for(int j = 0; j < resultVectors.Length; j++) {
				DataVector<int> inputVector = inputVectors[j];
				DataVector<int> resultVector = resultVectors[j];
				resultVector.Data[resultPosition] = inputVector.Data[inputPosition];
				resultVector.SpecialData[resultPosition] = inputVector.SpecialData[inputPosition];
			}
		}
	}
	class DataTupleIntVectorEqualityComparer : IEqualityComparer<DataTuple> {
		DataVector<int>[] columns;
		public DataTupleIntVectorEqualityComparer(DataVector<int>[] columns) {
			this.columns = columns;
		}
		public bool Equals(DataTuple x, DataTuple y) {
			bool eq = true;
			int dataIndex1 = x.Index;
			int dataIndex2 = y.Index;
			for(int i = 0; i < columns.Length; i++) {
				DataVector<int> vector = columns[i];
				SpecialDataValue spec1 = vector.SpecialData[dataIndex1];
				SpecialDataValue spec2 = vector.SpecialData[dataIndex2];
				if(spec1 == SpecialDataValue.None && spec2 == SpecialDataValue.None)
					eq &= vector.Data[dataIndex1] == vector.Data[dataIndex2];
				else
					eq &= spec1 == spec2;
				if(!eq)
					return false;
			}
			return true;
		}
		public int GetHashCode(DataTuple tuple) {
			unchecked {
				int item;
				int dataIndex = tuple.Index;
				int FNV_prime_32 = 16777619;
				int FNV_offset_basis_32 = (int)2166136261;
				int hash = FNV_offset_basis_32;
				for(int i = 0; i < columns.Length; i++) {
					DataVector<int> vector = columns[i];
					SpecialDataValue specialValue = vector.SpecialData[dataIndex];
					if(specialValue == SpecialDataValue.None)
						item = vector.Data[dataIndex];
					else
						item = specialValue.GetHashCode();
					hash = (hash ^ item) * FNV_prime_32;  
				}
				return hash;
			}
		}
	}
}
