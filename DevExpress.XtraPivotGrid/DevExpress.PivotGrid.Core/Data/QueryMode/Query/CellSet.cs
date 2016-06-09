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
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.QueryMode {
	public class CellSet<TColumn> : ICalculationSource<LevelRecord, int>, ISortContext<TColumn, LevelRecord, int> where TColumn : QueryColumn {
		internal class IndexedTable : Dictionary<LevelRecord, Dictionary<LevelRecord, MeasuresStorage>> {
			Dictionary<IQueryMetadataColumn, int> indexes;
			Dictionary<int, IQueryMetadataColumn> cindexes;
			public IndexedTable(Dictionary<IQueryMetadataColumn, int> indexes) {
				this.indexes = indexes;
				cindexes = new Dictionary<int, IQueryMetadataColumn>();
				foreach(KeyValuePair<IQueryMetadataColumn, int> pair in indexes)
					cindexes.Add(pair.Value, pair.Key);
			}
			public object GetMeasureValue(LevelRecord columnIndex, LevelRecord rowIndex, int dataIndex) {
				Dictionary<LevelRecord, MeasuresStorage> dic = null;
				if(!this.TryGetValue(columnIndex, out dic))
					return null;
				MeasuresStorage val = null;
				if(!dic.TryGetValue(rowIndex, out val))
					return null;
				return val.GetValue(cindexes[dataIndex]);
			}
			public void SetMeasureValues(LevelRecord rowIndex, LevelRecord columnIndex, MeasuresStorage row, int capacity) {
				Dictionary<LevelRecord, MeasuresStorage> dic = null;
				if(!this.TryGetValue(columnIndex, out dic)) {
					dic = new Dictionary<LevelRecord, MeasuresStorage>(capacity);
					this.Add(columnIndex, dic);
				}
				dic[rowIndex] = row;
			}
		}
		readonly IndexedTable rows;
		readonly TuplesIndexedTreeCache<TColumn> rowIndexes;
		readonly TuplesIndexedTreeCache<TColumn> columnIndexes;
		readonly List<TColumn> dataArea;
		readonly IDataSourceHelpersOwner<TColumn> owner;
		readonly IQueryContext<TColumn> context;
		public TuplesIndexedTreeCache<TColumn> RowIndexes { get { return rowIndexes; } }
		public TuplesIndexedTreeCache<TColumn> ColumnIndexes { get { return columnIndexes; } }
		public bool IsDataEmpty {
			get { return rows.Count == 0 && columnIndexes.Root.Count == 0 && rowIndexes.Root.Count == 0; }
		}
		public CellSet(IQueryContext<TColumn> context, IDataSourceHelpersOwner<TColumn> owner) {
			Dictionary<IQueryMetadataColumn, int> indexes = new Dictionary<IQueryMetadataColumn, int>();
			dataArea = context.Areas.ServerSideDataArea;
			for(int i = 0; i < dataArea.Count; i++)
				indexes.Add(dataArea[i].Metadata, i);
			rows = new IndexedTable(indexes);
			rowIndexes = CreateTree(context.Areas.RowArea);
			columnIndexes = CreateTree(context.Areas.ColumnArea);
			this.owner = owner;
			this.context = context;
		}
		protected TuplesIndexedTreeCache<TColumn> CreateTree(List<TColumn> allArea) {
			return new TuplesIndexedTreeCache<TColumn>(allArea);
		}
		internal IndexedTable GetValues() {
			return rows;
		}
		internal object GetMeasureValue(LevelRecord columnIndex, LevelRecord rowIndex, int dataIndex) {
			return rows.GetMeasureValue(columnIndex, rowIndex, dataIndex);
		}
		internal void SetRowValue(LevelRecord rowIndex, LevelRecord columnIndex, MeasuresStorage row, int columnCapacity) {
			rows.SetMeasureValues(rowIndex, columnIndex, row, columnCapacity);
		}
		LevelRecord ISortContext<TColumn, LevelRecord, int>.GetSortByObject(List<QueryMember> members, bool isColumn) {
			return (isColumn ? ColumnIndexes : RowIndexes).GetMembersIndex(members);
		}
		int ISortContext<TColumn, LevelRecord, int>.GetData(TColumn column) {
			return dataArea.IndexOf(column);
		}
		bool ISortContext<TColumn, LevelRecord, int>.IsValidData(int data) {
			return data >= 0;
		}
		List<TColumn> ISortContext<TColumn, LevelRecord, int>.GetDataArea() {
			return dataArea;
		}
		Func<IQueryMemberProvider, IQueryMemberProvider, int?> ISortContext<TColumn, LevelRecord, int>.GetCustomFieldSort(TColumn column) {
			return owner.GetCustomFieldSort(new CustomSortHelper<TColumn, LevelRecord, int>(this, context.Areas, owner), column);
		}
		Func<object, string> ISortContext<TColumn, LevelRecord, int>.GetCustomFieldText(TColumn column) {
			return owner.GetCustomFieldText(column);
		}
		object ICalculationSource<LevelRecord, int>.GetValue(LevelRecord column, LevelRecord row, int data) {
			return GetMeasureValue(column, row, data);
		}
		ICalculationSource<LevelRecord, int> ICalculationContext<LevelRecord, int>.GetValueProvider() {
			return this;
		}
		int ICalculationContext<LevelRecord, int>.GetData(int index) {
			return index;
		}
		IEnumerable<LevelRecord> ICalculationContext<LevelRecord, int>.EnumerateFullLevel(bool isColumn, int level) {
			throw new NotImplementedException(); 
		}
		object ICalculationContext<LevelRecord, int>.GetValue(LevelRecord record) {
			throw new NotImplementedException(); 
		}
		object ICalculationContext<LevelRecord, int>.GetDisplayValue(LevelRecord record) {
			throw new NotImplementedException(); 
		}
	}
}
