#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
namespace DevExpress.PivotGrid.Internal.ThinClientDataSource {
	public class PivotGridThinClientData {
		readonly Dictionary<PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>, Dictionary<int, ThinClientValueItem>> data = new Dictionary<PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>, Dictionary<int, ThinClientValueItem>>();
		readonly IList<ThinClientFieldValueItem> columnFieldValues;
		readonly IList<ThinClientFieldValueItem> rowFieldValues;
		public IList<ThinClientFieldValueItem> ColumnFieldValues { get { return columnFieldValues; } }
		public IList<ThinClientFieldValueItem> RowFieldValues { get { return rowFieldValues; } }
		internal Dictionary<PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>, Dictionary<int, ThinClientValueItem>> Data { get { return data; } }
		public PivotGridThinClientData(IList<ThinClientFieldValueItem> columnFieldValues, IList<ThinClientFieldValueItem> rowFieldValues) {
			this.columnFieldValues = columnFieldValues;
			this.rowFieldValues = rowFieldValues;
		}
		public void AddCell(ThinClientFieldValueItem column, ThinClientFieldValueItem row, int dataIndex, ThinClientValueItem cellValue) {
			PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem> key = new PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>(column, row);
			Dictionary<int, ThinClientValueItem> dataIndexHash;
			if(!data.TryGetValue(key, out dataIndexHash))
				dataIndexHash = new Dictionary<int, ThinClientValueItem>();
			dataIndexHash[dataIndex] = cellValue;
			data[key] = dataIndexHash;
		}
		public bool TryGetCell(ThinClientFieldValueItem column, ThinClientFieldValueItem row, int dataIndex, out ThinClientValueItem cellValue) {
			PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem> key = new PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>(column, row);
			Dictionary<int, ThinClientValueItem> dataIndexHash;
			if(data.TryGetValue(key, out dataIndexHash)) {
				ThinClientValueItem outCellValue;
				if(dataIndexHash.TryGetValue(dataIndex, out outCellValue)) {
					cellValue = outCellValue;
					return true;
				}
			}
			cellValue = null;
			return false;
		}
	}
	public class ThinClientFieldValueItem  {
		public ThinClientValueItem Value { get; private set; }
		public string TotalDisplayText { get; private set; }
		public IList<ThinClientFieldValueItem> Children { get; set; }
		public ThinClientFieldValueItem(ThinClientValueItem value) {
			Value = value;
		}
		public ThinClientFieldValueItem(ThinClientValueItem value, string totalDisplayText) {
			Value = value;
			TotalDisplayText = totalDisplayText;
		}
	}
	public class FieldValueItem {
		public ThinClientValueItem Value { get; private set; }
		public string TotalDisplayText { get; private set; }
		public int Level { get; private set; }
		public IList<FieldValueItem> Children { get; set; }
		public FieldValueItem(ThinClientValueItem value, string totalDisplayText, int level) {
			Value = value;
			TotalDisplayText = totalDisplayText;
			Level = level;
		}
	}
	public class ThinClientValueItem {
		public object Value { get; private set; }
		public string DisplayText { get; private set; }
		public object Tag { get; set; }
		public ThinClientValueItem(object value) {
			Value = value;
		}
		public ThinClientValueItem(object value, string displayText) {
			Value = value;
			DisplayText = displayText;
		}
	}
	public class PairReferenceKey<T1, T2> {
		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		public PairReferenceKey(T1 first, T2 second) {
			First = first;
			Second = second;
		}
		public override bool Equals(object obj) {
			PairReferenceKey<T1, T2> key = obj as PairReferenceKey<T1, T2>;
			return key != null && Object.ReferenceEquals(key.First, First) && Object.ReferenceEquals(key.Second, Second);
		}
		public override int GetHashCode() {
			int columnHash = First != null ? First.GetHashCode() : 0;
			int rowHash = Second != null ? Second.GetHashCode() : 0;
			return columnHash ^ rowHash;
		}
	}
}
