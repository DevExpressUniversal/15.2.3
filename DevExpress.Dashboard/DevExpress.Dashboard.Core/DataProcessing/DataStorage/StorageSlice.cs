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
using System.Text;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class StorageSlice : IEnumerable<StorageRow>, IRowOwner {
		#region internal types
		public struct StorageRowEnumerator : IEnumerator<StorageRow> {
			StorageSlice slice;
			Dictionary<CompositeKey, Dictionary<int, object>>.KeyCollection.Enumerator enumerator;
			public StorageRow Current { get { return this.slice.GetboundRow(enumerator.Current); } }
			object IEnumerator.Current { get { return this.Current; } }
			public StorageRowEnumerator(StorageSlice slice) {
				this.slice = slice;
				this.enumerator = slice.dto.Data.Keys.GetEnumerator();
			}
			public void Dispose() {
				enumerator.Dispose();
			}
			public bool MoveNext() {
				return enumerator.MoveNext();
			}
			public void Reset() {
				((IEnumerator)enumerator).Reset();
			}
		}
		#endregion
		ISliceOwner sliceOwner;
		SliceDTO dto;
		StorageColumn[] keyColumns;
		Dictionary<StorageColumn, int> keyColumnIndexes;
		public StorageSlice(IEnumerable<StorageColumn> keyColumns, SliceDTO dto, ISliceOwner sliceOwner) {
			this.keyColumns = keyColumns.ToArray();
			this.dto = dto;
			this.sliceOwner = sliceOwner;
			this.keyColumnIndexes = new Dictionary<StorageColumn, int>();
			for(int i = 0; i < dto.KeyIds.Length; i++)
				this.keyColumnIndexes.Add(keyColumns.Single(c => c.Name == dto.KeyIds[i]), i);
		}
		public IEnumerable<StorageColumn> KeyColumns { get { return keyColumns; } }
		public IEnumerable<StorageColumn> MeasureColumns { get { return dto.ValueIds.Keys.Select(name => sliceOwner.GetColumn(name)).ToList(); } }
		public StorageRow? FindRow(StorageRow unboundRow) {
			if(unboundRow.IsBound)
				return unboundRow;
			Dictionary<StorageColumn, StorageValue> unboundValues = unboundRow.GetUnboundValues(); 
			int[] keyValues = new int[keyColumns.Length];
			if(unboundValues != null)
				foreach(KeyValuePair<StorageColumn, StorageValue> pair in unboundValues) {
					StorageColumn column = pair.Key;
					StorageValue value = pair.Value;
					if(!column.IsKey)
						continue;
					int columnIndex = keyColumnIndexes[column];
					if(value.ValueBindType == StorageValue.BindType.KeyValue)
						keyValues[columnIndex] = value.KeyBindData.EncodedValue;
					else
						keyValues[columnIndex] = column.Encode(value.MaterializedValue);
				}
			CompositeKey key = new CompositeKey(keyValues);
			if(dto.Data.ContainsKey(key))
				return GetboundRow(key);
			else
				return null;
		}
		public StorageRow AddRow(StorageRow unboundRow) {
			if(unboundRow.IsBound)
				throw new InvalidOperationException("This row already bound");
			Dictionary<StorageColumn, StorageValue> unbound = unboundRow.GetUnboundValues();
			int[] encodedValues = new int[keyColumns.Length];
			for(int i = 0; i < keyColumns.Length; i++) {
				StorageColumn column = keyColumns[i];
				encodedValues[i] = column.Encode(unbound[column].MaterializedValue);
			}
			CompositeKey rowKey = new CompositeKey(encodedValues);
			if(!dto.Data.ContainsKey(rowKey))
				dto.Data.Add(rowKey, new Dictionary<int, object>());
			StorageRow boundRow = GetboundRow(rowKey);
			if(unbound != null)
				foreach(var pair in unbound) 
					if(!pair.Key.IsKey)
						boundRow[pair.Key] = pair.Value;
			return boundRow;
		}
		public void RemoveMeasure(StorageColumn column) {
			DXContract.Requires(!column.IsKey);
			int index = dto.ValueIds[column.Name];
			dto.ValueIds.Remove(column.Name);
			foreach(var measureValues in dto.Data.Values)
				measureValues.Remove(index);
		}
		public override string ToString() {
			return keyColumns.Select(c => c.ToString()).Aggregate((s1, s2) => s1 + ", " + s2);
		}
		public StorageRowEnumerator GetEnumerator() {
			return new StorageRowEnumerator(this);
		}
		IEnumerator<StorageRow> IEnumerable<StorageRow>.GetEnumerator() {
			return new StorageRowEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new StorageRowEnumerator(this);
		}
		StorageRow GetboundRow(CompositeKey key) {
			return StorageRow.CreateBound(key, this);
		}
		#region IRowOwner implementation
		StorageColumn[] IRowOwner.KeyColumns { get { return keyColumns; } }
		Dictionary<int, object> IRowOwner.ExtractValues(CompositeKey key) {
			return dto.Data[key];
		}
		int IRowOwner.GetColumnIndex(StorageColumn column) {
			if(column.IsKey)
				return keyColumnIndexes[column];
			else {
				int key;
				if(!dto.ValueIds.TryGetValue(column.Name, out key)) {
					key = dto.ValueIds.Keys.Count;
					dto.ValueIds[column.Name] = key;
				}
				return key;
			}
		}
		#endregion
	}
}
