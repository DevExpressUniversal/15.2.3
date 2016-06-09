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
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class ColumnProcessorBase {
		public abstract Type DataType { get; }
		public abstract int UniqueCount { get; }
		public abstract void FinishProcessing(IStorageColumn storageColumn);
		public abstract void WriteUniqueData(IStorageColumn storageColumn);
		public abstract void SaveProcessingData(IStorageColumn storageColumn);
		public abstract void SaveIndexesSortedByValues(IStorageColumn tempStorageColumn, IStorageColumn indexesStorageColumn, bool isCompressed);
		public string ColumnName { get; protected set; }
	}
	public class ColumnProcessor<T> : ColumnProcessorBase {
		const int NullValueIndex = 0;
		Dictionary<T, int> mapValueToSurrogate;
		Dictionary<int, int> updatedIndexes;
		Comparer<T> comparer = null;
		int[] indexMapping;
		int currentRowIndex = 0;
		int currentIndexMapping = 0;
		int[] indexes;
		bool isNullWriteUniqueValues = false;
		protected Dictionary<int, int> UpdatedIndexes { get { return updatedIndexes; } }
		public override int UniqueCount { get { return mapValueToSurrogate.Count + 1; } }
		public override Type DataType { get { return typeof(T); } }
		public ColumnProcessor(string columnName, int rowCount)
			: this(columnName, rowCount, EqualityComparerFactory.Get<T>(), ComparerFactory.Get<T>()) { }
		ColumnProcessor(string columnName, int rowCount, EqualityComparer<T> eqComparer, Comparer<T> comparer) {
			this.ColumnName = columnName;
			this.indexMapping = new int[rowCount];
			this.mapValueToSurrogate = new Dictionary<T, int>(eqComparer);
			this.updatedIndexes = new Dictionary<int, int>();
			this.comparer = comparer;
		}
		protected virtual void SortProcessingData() {
			List<KeyValuePair<T, int>> uniqueSortedValues = mapValueToSurrogate.ToList();
			uniqueSortedValues.Sort((x, y) => comparer.Compare(x.Key, y.Key));
			mapValueToSurrogate.Clear();
			int count = 1;
			foreach(KeyValuePair<T, int> pair in uniqueSortedValues) {
				mapValueToSurrogate.Add(pair.Key, pair.Value);
				updatedIndexes.Add(pair.Value, count);
				count++;
			}
			updatedIndexes.Add(NullValueIndex, NullValueIndex);
		}
		protected virtual int ProcessCore(T dataItem, bool isNullValue) {
			int dataItemIndex = NullValueIndex;
			if(!isNullValue && !mapValueToSurrogate.TryGetValue(dataItem, out dataItemIndex)) {
				dataItemIndex = mapValueToSurrogate.Count + 1;
				mapValueToSurrogate.Add(dataItem, dataItemIndex);
			}
			return dataItemIndex;
		}
		public void Process(T dataItem, bool isNullValue) {
			indexMapping[currentIndexMapping++] = ProcessCore(dataItem, isNullValue || dataItem == null);
		}
		public override void SaveProcessingData(IStorageColumn storageColumn) {
			storageColumn.WriteCompressedValues(currentRowIndex, currentRowIndex + currentIndexMapping - 1, DataConvertor.Convert(indexMapping));
			currentRowIndex += currentIndexMapping;
			currentIndexMapping = 0;
		}
		public override void FinishProcessing(IStorageColumn storageColumn) {
			if(currentIndexMapping > 0) {
				int[] lastIndexes = new int[currentIndexMapping];
				Array.Copy(indexMapping, lastIndexes, currentIndexMapping);
				storageColumn.WriteCompressedValues(currentRowIndex, currentRowIndex + currentIndexMapping - 1, DataConvertor.Convert(lastIndexes));
			}
			SortProcessingData();
		}
		public override void WriteUniqueData(IStorageColumn storageColumn) {
			List<T> mapUniqueData = mapValueToSurrogate.Keys.ToList();
			if(!isNullWriteUniqueValues) {
				if(mapUniqueData.Count > 0)
					mapUniqueData.Insert(0, mapUniqueData[0]);
				else
					mapUniqueData.Insert(0, default(T));
				isNullWriteUniqueValues = true;
			}
			storageColumn.WriteUniqueValues(mapUniqueData);
			mapValueToSurrogate.Clear();
		}
		public override void SaveIndexesSortedByValues(IStorageColumn tempStorageColumn, IStorageColumn indexesStorageColumn, bool shouldCompress) {
			int startIndex = 0;
			int endIndex = 0;
			for(int j = 0; j < tempStorageColumn.CompressedBlocksCount; j++) {
				byte[] data = tempStorageColumn.ReadCompressedValues(j, out startIndex, out endIndex);
				indexes = DataConvertor.Convert(data);
				int[] newIndexes = new int[indexes.Length];
				for(int i = 0; i < indexes.Length; i++)
					newIndexes[i] = updatedIndexes[indexes[i]];
				byte[] compressionData = DataConvertor.Convert(newIndexes);
				if(shouldCompress)
					compressionData = DataCompression.Compression(newIndexes, updatedIndexes.Count, endIndex - startIndex + 1);
				indexesStorageColumn.WriteCompressedValues(startIndex, endIndex, compressionData);
			}
		}
	}
}
