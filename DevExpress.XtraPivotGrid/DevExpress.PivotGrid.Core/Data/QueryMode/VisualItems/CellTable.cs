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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.DataCalculation;
using PivotCellValue = DevExpress.XtraPivotGrid.Data.PivotCellValue;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class CellTable<TColumn> : DictionaryContainer<GroupInfo, GroupInfoColumn>,  ICalculationSource<GroupInfo, IQueryMetadataColumn> where TColumn : QueryColumn {
		ICellTableOwner<TColumn> owner;
		EvaluatorContextDescriptor descriptor;		
		Dictionary<string, IQueryMetadataColumn> measureMap = new Dictionary<string, IQueryMetadataColumn>();
		public Dictionary<string, IQueryMetadataColumn> MeasureMap { get { return measureMap; } }
		public EvaluatorContextDescriptor Descriptor { get { return descriptor; } }
		protected ICellTableOwner<TColumn> Owner { get { return owner; } }
		internal CellTable(ICellTableOwner<TColumn> owner) {
			this.owner = owner;
			descriptor = new QueryEvaluatorContextDescriptor(this);
		}
		public GroupInfoColumn GetOrCreate(GroupInfo key, int capacity) {
			if(key == null)
				return null;
			GroupInfoColumn result = GetDictionaryValue(key);
			if(result != null) {
				if(result.Count * 7 < capacity) {
					GroupInfoColumn old = result; 
					result = new GroupInfoColumn(old.Count + capacity);
					foreach(KeyValuePair<GroupInfo, MeasuresStorage> pair in old)
						result[pair.Key] = pair.Value;
					SetDictionaryValue(key, result);
				}
				return result;
			}
			result = new GroupInfoColumn(capacity);
			AddDictionaryValue(key, result);
			return result;
		}
		public GroupInfoColumn this[GroupInfo key] {
			get {
				GroupInfoColumn result = GetDictionaryValue(key);
				if(result != null)
					return result;
				result = new GroupInfoColumn();
				AddDictionaryValue(key, result);
				return result;
			}
			set { SetDictionaryValue(key, value); }
		}
		protected int MeasureCount {
			get {
				return Owner.GetUniqueMeasureColumns().Count;
			}
		}
		public void RemoveRows(List<GroupInfo> removedChildren) {
			foreach(GroupInfoColumn column in Values)
				column.RemoveItems(removedChildren);
		}
		public void ReadData(CellSet<TColumn> set) {
			int measureCount = Owner.GetUniqueMeasureColumns().Count;
			if(measureCount == 0)
				return;
			EnsureCapcity(set.GetValues().Count);
			GroupInfo key = null;
			foreach(KeyValuePair<LevelRecord, Dictionary<LevelRecord, MeasuresStorage>> pair1 in set.GetValues()) {
				GroupInfoColumn columnStorage = GetOrCreate(pair1.Key.GroupInfo, pair1.Value.Count);
				if(columnStorage != null)
					foreach(KeyValuePair<LevelRecord, MeasuresStorage> pair2 in pair1.Value) {
						key = pair2.Key.GroupInfo;
						if(key != null)
							columnStorage[key] = pair2.Value;
					}
			}
		}
		class QueryEvaluatorContextDescriptor : EvaluatorContextDescriptor {
			CellTable<TColumn> cells;
			public QueryEvaluatorContextDescriptor(CellTable<TColumn> cells) {
				this.cells = cells;
			}
			public override IEnumerable GetCollectionContexts(object source, string collectionName) {
				throw new NotImplementedException();
			}
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
				throw new NotImplementedException();
			}
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
				return cells.GetValue((MeasuresStorage)source, propertyPath.PropertyPath);
			}
		}
		public void AddUnboundFieldToMap(string name, IQueryMetadataColumn column) {
			if(!measureMap.ContainsKey(name))
				measureMap.Add(name, column);
		}
		object GetValue(MeasuresStorage storage, string measure) {
			IQueryMetadataColumn measureColumn = null;
			if(!measureMap.TryGetValue(measure, out measureColumn))
				return null;
			object value = storage.GetValue(measureColumn);
			if(value != null)
				return value;
			return GetValueCore(measureColumn, storage);
		}
		 public object GetValue(GroupInfo columnGroup, GroupInfo rowGroup, IQueryMetadataColumn measure) {
			GroupInfoColumn column;
			if(!TryGetValue(columnGroup, out column))
				return null;
			if(column == null)
				return null;
			MeasuresStorage storage = column[rowGroup];
			if(storage == null)
				return null;
			object value = storage.GetValue(measure);
			if(value != null)
				return value;
			return GetValueCore(measure, storage);
		}
		public PivotCellValue GetPivotCellValue(GroupInfo columnGroup, GroupInfo rowGroup, IQueryMetadataColumn measure) {
			GroupInfoColumn column;
			if(!TryGetValue(columnGroup, out column))
				return null;
			if(column == null)
				return null;
			MeasuresStorage storage = column[rowGroup];
			if(storage == null)
				return null;
			PivotCellValue value = storage.GetPivotCellValue(measure);
			if(value != null)
				return value;
			return GetPivotCellValueCore(measure, storage);
		}
		protected object GetValueCore(IQueryMetadataColumn measure, MeasuresStorage storage) {
			IUnboundSummaryLevelMetadataColumn unboundColumn = measure as IUnboundSummaryLevelMetadataColumn;
			if(unboundColumn == null)
				return null;
			else
				return unboundColumn.EvaluateValue(storage);
		}
		protected PivotCellValue GetPivotCellValueCore(IQueryMetadataColumn measure, MeasuresStorage storage) {
			IUnboundSummaryLevelMetadataColumn unboundColumn = measure as IUnboundSummaryLevelMetadataColumn;
			if(unboundColumn == null)
				return null;
			else
				return unboundColumn.EvaluatePivotCellValue(storage);
		}
		public virtual void SaveToStream(TypedBinaryWriter writer, List<GroupInfo> columnGroupIndexes,
			Dictionary<GroupInfo, int> rowGroupIndexes, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			MeasureStorageKeepHelperBase helper = CreateSerializeHelper(true);
			helper.SaveToStream(writer, columnIndexes);
			GroupInfoColumn gic;
			int nullCount = 0;
			for(int i = 0; i < columnGroupIndexes.Count; i++) {
				if(TryGetValue(columnGroupIndexes[i], out gic) && gic.Count > 0) {
					if(nullCount != 0)
						writer.Write(-(nullCount - 1));
					gic.SaveToStream(helper, writer, rowGroupIndexes, columnIndexes);
					nullCount = 0;
				} else
					nullCount++;
			}
			if(nullCount != 0)
				writer.Write(-(nullCount - 1));
		}
		protected abstract MeasureStorageKeepHelperBase CreateSerializeHelper(bool save);
		public void LoadFromStream(TypedBinaryReader reader, List<GroupInfo> columnGroups, List<GroupInfo> rowGroups, List<IQueryMetadataColumn> columnIndexes) {
			Clear();
			MeasureStorageKeepHelperBase helper = CreateSerializeHelper(false);
			helper.ReadFromStream(reader, columnIndexes);
			for(int i = 0; i < columnGroups.Count; i++) {
				int count = reader.ReadInt32();
				if(count > 0) {
					GroupInfo group = columnGroups[i];
					GroupInfoColumn giColumn = new GroupInfoColumn();
					giColumn.LoadFromStream(count, helper, reader, rowGroups, columnIndexes);
					AddDictionaryValue(group, giColumn);
				} else {
					i = i - count;
				}
			}
		}
		object ICalculationSource<GroupInfo, IQueryMetadataColumn>.GetValue(GroupInfo column, GroupInfo row, IQueryMetadataColumn data) {
			return GetValue(column, row, data);
		}
	}
}
