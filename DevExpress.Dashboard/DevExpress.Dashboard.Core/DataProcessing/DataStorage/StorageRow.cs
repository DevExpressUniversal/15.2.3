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
namespace DevExpress.DashboardCommon.DataProcessing {
	public struct StorageRow : IEnumerable<KeyValuePair<StorageColumn, StorageValue>> {
		#region internal
		public struct StorageRowColumnValueEnumerator : IEnumerator<KeyValuePair<StorageColumn, StorageValue>> {
			StorageRow row;
			int boundPosition;
			Dictionary<StorageColumn, StorageValue>.Enumerator unboundEnumerator;
			public KeyValuePair<StorageColumn, StorageValue> Current {
				get {
					if(row.IsBound) {
						StorageColumn column = row.rowOwner.KeyColumns[boundPosition];
						return new KeyValuePair<StorageColumn, StorageValue>(column, row[column]);
					} else
						return unboundEnumerator.Current;
				}
			}
			object IEnumerator.Current { get { return this.Current; } }
			public StorageRowColumnValueEnumerator(StorageRow row) {
				this.row = row;
				this.boundPosition = -1;
				if(row.IsBound)
					this.unboundEnumerator = default(Dictionary<StorageColumn, StorageValue>.Enumerator);
				else
					this.unboundEnumerator = row.UnboundList.GetEnumerator();
			}
			public void Dispose() {
				if(!row.IsBound)
					unboundEnumerator.Dispose();
			}
			public bool MoveNext() {
				if(row.IsBound) {
					boundPosition++;
					return boundPosition < row.rowOwner.KeyColumns.Length;
				} else
					return unboundEnumerator.MoveNext();
			}
			public void Reset() {
				if(row.IsBound)
					boundPosition = -1;
				else
					((IEnumerator)unboundEnumerator).Reset();
			}
		}
		#endregion
		IRowOwner rowOwner;
		CompositeKey compositeKey;
		Dictionary<int, object> measureColumnsValueList;
		Dictionary<StorageColumn, StorageValue> unboundList;
		Dictionary<int, object> MeasureColumnsValueList {
			get {
				if(measureColumnsValueList == null)
					measureColumnsValueList = rowOwner.ExtractValues(compositeKey);
				return measureColumnsValueList;
			}
		}
		Dictionary<StorageColumn, StorageValue> UnboundList {
			get {
				if(unboundList == null)
					unboundList = new Dictionary<StorageColumn, StorageValue>();
				return unboundList;
			}
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043")]
#endif
		public StorageValue this[StorageColumn column] {
			get { return GetStorageValue(column); }
			set { SetStorageValue(column, value); }
		}
		public override string ToString() {
			string type = (IsBound ? "(bound)" : "(unbound)");
			string unboundData = unboundList != null ? unboundList.Values.Select(v => v.ToString()).Aggregate((s1, s2) => s1 + "," + s2) : "";
			return type + " " + unboundData;
		}
		StorageValue GetStorageValue(StorageColumn column) {
			if(IsBound)
				return CreateColumnValue(column);
			else
				return UnboundList[column];
		}
		void SetStorageValue(StorageColumn column, StorageValue value) {
			if(IsBound) {
				if(column.IsKey)
					throw new InvalidOperationException("Can't change key value in bound row.");
				else {
					StorageValue boundValue = this[column];
					boundValue.MaterializedValue = value.MaterializedValue;
				}
			} else {
				if(UnboundList.ContainsKey(column))
					UnboundList[column] = value;
				else
					UnboundList.Add(column, value);
			}
		}
		#region for internal DataStorage use
		internal bool IsBound { get { return rowOwner != null; } }
		internal Dictionary<StorageColumn, StorageValue> GetUnboundValues() {
			if(IsBound)
				return null;
			else
				return unboundList;
		}
		#endregion
		StorageValue CreateColumnValue(StorageColumn column) {
			if(column.IsKey)
				return StorageValue.CreateKeyValue(column, compositeKey.Keys[rowOwner.GetColumnIndex(column)]);
			else
				return StorageValue.CreateMeasureValue(MeasureColumnsValueList, rowOwner.GetColumnIndex(column));
		}
		public static StorageRow CreateBound(CompositeKey compositeKey, IRowOwner rowOwner) {
			StorageRow row = new StorageRow();
			row.compositeKey = compositeKey;
			row.rowOwner = rowOwner;
			row.measureColumnsValueList = null;
			return row;
		}
		#region implementation IEnumerable<KeyValuePair<StorageColumn, StorageValue>>
		public StorageRowColumnValueEnumerator GetEnumerator() {
			return new StorageRowColumnValueEnumerator(this);
		}
		IEnumerator<KeyValuePair<StorageColumn, StorageValue>> IEnumerable<KeyValuePair<StorageColumn, StorageValue>>.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
}
