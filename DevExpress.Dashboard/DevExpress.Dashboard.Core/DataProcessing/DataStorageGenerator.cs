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

using DevExpress.DashboardCommon.Native;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class DataStorageGeneratorResult {
		public DataStorage Storage { get; set; }
		public string[][] SortOrderSlices { get; set; }
	}
	public class DataStorageGenerator {
		#region internal types
		class SliceInfo {
			public PivotGridFieldBase[] Fields { get; private set; }
			public StorageColumn[] Columns { get; private set; }
			public StorageSlice Slice { get; private set; }
			public SliceInfo(IEnumerable<PivotGridFieldBase> fields, IEnumerable<StorageColumn> columns, StorageSlice slice) {
				this.Fields = fields.ToArray();
				this.Columns = columns.ToArray();
				this.Slice = slice;
			}
		}
		struct CrossSliceKey : IEquatable<CrossSliceKey> {
			PivotGridFieldBase ColField;
			PivotGridFieldBase RowField;
			public CrossSliceKey(PivotGridFieldBase colField, PivotGridFieldBase rowField) {
				ColField = colField;
				RowField = rowField;
			}
			public bool Equals(CrossSliceKey other) {
				return ColField == other.ColField && RowField == other.RowField;
			}
			public override int GetHashCode() {
				return HashcodeHelper.GetCompositeHashCode(ColField, RowField);
			}
		}
		class CrossSliceInfo {
			public StorageSlice Slice { get; set; }
			public CrossSliceInfo(StorageSlice slice) {
				this.Slice = slice;
			}
		}
		struct DimensionMemberValue {
			public object Value { get; set; }
			public string UniqueName { get; set; }
			public string DisplayText { get; set; }
			public int VisibleIndex { get; set; }
			public object GetMainValue(bool isOlap) {
				return isOlap ? UniqueName : Value;
			}
		}
		#endregion
		public const string DisplayTextStorageColumnName = "_DisplayText_{4873F9E9-65B2-4307-BB25-BFD09F6A2E54}";
		public const string ValueStorageColumnName = "_Value_{E5597004-313E-4F79-B02E-DEA46EEB1BFE}";
		public static bool IsSpecialColumn(StorageColumn column) {
			string name = column.Name;
			return name == DisplayTextStorageColumnName || name == ValueStorageColumnName;
		}
		readonly DataStorage storage;
		readonly IPivotGridDataSource dataSource;
		readonly bool isOlap;
		readonly List<PivotGridFieldBase> colAxisFields;
		readonly List<PivotGridFieldBase> rowAxisFields;
		readonly List<PivotGridFieldBase> dataFields;
		readonly List<SliceDescription> requiredSlices;
		readonly Dictionary<PivotGridFieldBase, StorageColumn> storageColumns;
		readonly Dictionary<PivotGridFieldBase, SliceInfo> colAxisSliceInfos;
		readonly Dictionary<PivotGridFieldBase, SliceInfo> rowAxisSliceInfos;
		readonly Dictionary<PivotGridFieldBase, SliceInfo> valueMetaDataSliceInfos;
		readonly Dictionary<CrossSliceKey, CrossSliceInfo> crossAxisSliceInfos;
		readonly Dictionary<int, PivotGridFieldBase> colLevelToField;
		readonly Dictionary<int, PivotGridFieldBase> rowLevelToField;
		readonly StorageColumn displayTextStorageColumn;
		readonly StorageColumn valueStorageColumn;
		DataStorageGenerator(PivotGridData data, IEnumerable<PivotGridFieldBase> dataFields, IEnumerable<SliceDescription> slices) {
			storage = DataStorage.CreateEmpty();
			dataSource = data.PivotDataSource;
			isOlap = dataSource is IPivotOLAPDataSource;
			colAxisFields = data.GetFieldsByArea(PivotArea.ColumnArea, false);
			rowAxisFields = data.GetFieldsByArea(PivotArea.RowArea, false);
			this.dataFields = dataFields.ToList();
			requiredSlices = slices == null ? null : slices.ToList();
			var allFields = colAxisFields.Concat(rowAxisFields).ToList();
			storageColumns = allFields.Concat(dataFields)
				.ToDictionary(f => f, f => storage.CreateColumn(f.Name, !dataFields.Contains(f)));
			colAxisSliceInfos = colAxisFields.ToDictionary(currentField => currentField, currentField => CreateSliceInfo(colAxisFields, currentField));
			rowAxisSliceInfos = rowAxisFields.ToDictionary(currentField => currentField, currentField => CreateSliceInfo(rowAxisFields, currentField));
			if(isOlap) {
				valueMetaDataSliceInfos = allFields.ToDictionary(f => f, f => {
					StorageColumn[] columns = new[] { storage.GetColumn(f.Name) };
					return new SliceInfo(new[] { f }, columns, storage.GetSlice(columns));
				});
				displayTextStorageColumn = storage.CreateColumn(DisplayTextStorageColumnName, false);
				valueStorageColumn = storage.CreateColumn(ValueStorageColumnName, false);
			}
			colLevelToField = colAxisFields.Select((f, i) => i).ToDictionary(i => i, i => colAxisFields[i]);
			rowLevelToField = rowAxisFields.Select((f, i) => i).ToDictionary(i => i, i => rowAxisFields[i]);
			crossAxisSliceInfos = new Dictionary<CrossSliceKey, CrossSliceInfo>();
			for(int i = 0; i < colAxisFields.Count; i++)
				for(int j = 0; j < rowAxisFields.Count; j++)
					crossAxisSliceInfos.Add(new CrossSliceKey(colAxisFields[i], rowAxisFields[j]), CreateCrossSliceInfo(i, j));
			FillStorage();
		}
		SliceInfo CreateSliceInfo(List<PivotGridFieldBase> axisFields, PivotGridFieldBase currentField) {
			var sliceFields = axisFields.Take(axisFields.IndexOf(currentField) + 1).ToList();
			var columns = sliceFields.Select(f => storage.GetColumn(f.Name)).ToList();
			StorageSlice slice = IsSliceRequired(sliceFields) ? storage.GetSlice(columns) : null;
			return new SliceInfo(sliceFields, columns, slice);
		}
		CrossSliceInfo CreateCrossSliceInfo(int colLevel, int rowLevel) {
			var colFields = colAxisFields.Take(colLevel + 1).ToList();
			var rowFields = rowAxisFields.Take(rowLevel + 1).ToList();
			var signature = rowFields.Concat(colFields).ToList();
			StorageColumn[] columns = signature.Select(f => storage.GetColumn(f.Name)).ToArray();
			StorageSlice slice = IsSliceRequired(signature) ? storage.GetSlice(columns) : null;
			return new CrossSliceInfo(slice);
		}
		bool IsSliceRequired(IEnumerable<PivotGridFieldBase> sliceFields) {
			IEnumerable<string> names = sliceFields.Select(f => f.Name).ToList();
			return requiredSlices == null ? true : requiredSlices.Any(sliceD => sliceD.IsSignatureEqual(names));
		}
		void FillStorage() {
			List<DimensionMemberValue[]> colAxisValues = GetAxisValues(dataSource, true);
			List<DimensionMemberValue[]> rowAxisValues = GetAxisValues(dataSource, false);
			if(isOlap) {
				WriteValueMetadataSlice(colAxisValues, colAxisFields);
				WriteValueMetadataSlice(rowAxisValues, rowAxisFields);
			}
			WriteAxisSlicesToStorage(true, colAxisValues);
			WriteAxisSlicesToStorage(false, rowAxisValues);
			WriteCrossingSlicesToStorage(colAxisValues, rowAxisValues);
			if(requiredSlices == null || requiredSlices.Any(sliceDescr => sliceDescr.IsSignatureEqual(new string[0]))) {
				StorageSlice gtSlice = storage.GetSlice(new StorageColumn[0]);
				StorageRow row = AddMeasureValuesToRow(-1, -1, new StorageRow());
				gtSlice.AddRow(row);
			}
		}
		void WriteValueMetadataSlice(List<DimensionMemberValue[]> axisValues, List<PivotGridFieldBase> axisFields) {
			foreach(DimensionMemberValue[] values in axisValues) {
				int level = values.Length - 1;
				PivotGridFieldBase field = axisFields[level];
				DimensionMemberValue value = values[level];
				SliceInfo sliceInfo = valueMetaDataSliceInfos[field];
				StorageRow row = new StorageRow();
				row[sliceInfo.Columns[0]] = StorageValue.CreateUnbound(isOlap ? value.UniqueName : value.Value);
				row[displayTextStorageColumn] = StorageValue.CreateUnbound(value.DisplayText);
				row[valueStorageColumn] = StorageValue.CreateUnbound(value.Value);
				sliceInfo.Slice.AddRow(row);
			}
		}
		bool IsHierarchyRagged(List<DimensionMemberValue[]> axisValues, int index, int maxLevel) {
			int level1 = axisValues[index].Length - 1;
			int level2 = index + 1 < axisValues.Count ? axisValues[index + 1].Length - 1 : 0;
			return (level1 != maxLevel) && (level1 >= level2);
		}
		SliceInfo GetSliceInfo(bool isColumn, List<DimensionMemberValue[]> axisValues, int index) {
			int maxLevel = (isColumn ? colAxisFields.Count : rowAxisFields.Count) - 1;
			Dictionary<PivotGridFieldBase, SliceInfo> axisSliceInfos = isColumn ? colAxisSliceInfos : rowAxisSliceInfos;
			Dictionary<int, PivotGridFieldBase> levelToField = isColumn ? colLevelToField : rowLevelToField;
			DimensionMemberValue[] values = axisValues[index];
			SliceInfo sliceInfo = null;
			bool isRagged = IsHierarchyRagged(axisValues, index, maxLevel);
			for(int level = values.Length - 1; level <= maxLevel; level++) {
				sliceInfo = axisSliceInfos[levelToField[level]];
				bool findInNextLevel = sliceInfo.Slice == null && isRagged;
				if(!findInNextLevel)
					break;
			}
			return sliceInfo;
		}
		void WriteAxisSlicesToStorage(bool isColumn, List<DimensionMemberValue[]> axisValues) {
			for(int i = 0; i < axisValues.Count; i++) {
				DimensionMemberValue[] values = axisValues[i];
				SliceInfo sliceInfo = GetSliceInfo(isColumn, axisValues, i);
				if(sliceInfo.Slice == null)
					continue; 
				StorageRow row = new StorageRow();
				row = WriteKeyValuesToRow(values, sliceInfo.Columns, row);
				int index = values[values.Length - 1].VisibleIndex;
				int colIndex = isColumn ? index : -1;
				int rowIndex = isColumn ? -1 : index;
				row = AddMeasureValuesToRow(colIndex, rowIndex, row);
				sliceInfo.Slice.AddRow(row);
			}
		}
		void WriteCrossingSlicesToStorage(List<DimensionMemberValue[]> colAxisValues, List<DimensionMemberValue[]> rowAxisValues) {
			object[] measureValues = new object[dataFields.Count];
			for(int i = 0; i < colAxisValues.Count; i++) {
				for(int j = 0; j < rowAxisValues.Count; j++) {
					DimensionMemberValue[] colValues = colAxisValues[i];
					DimensionMemberValue[] rowValues = rowAxisValues[j];
					int colVisibleIndex = colValues[colValues.Length - 1].VisibleIndex;
					int rowVisibleIndex = rowValues[rowValues.Length - 1].VisibleIndex;
					for(int index = 0; index < dataFields.Count; index++)
						measureValues[index] = GetValue(colVisibleIndex, rowVisibleIndex, dataFields[index]);
					bool needWriteToStorage = false;
					for(int index = 0; index < measureValues.Length; index++)
						needWriteToStorage = needWriteToStorage || measureValues[index] != null;
					if(needWriteToStorage) {
						StorageRow row = new StorageRow();
						int colValuesLevel = colValues.Length - 1;
						int rowValuesLevel = rowValues.Length - 1;
						SliceInfo colSliceInfo = GetSliceInfo(true, colAxisValues, i);
						SliceInfo rowSliceInfo = GetSliceInfo(false, rowAxisValues, j);
						CrossSliceInfo sliceInfo = GetCrossingSliceInfo(colSliceInfo, rowSliceInfo);
						if(sliceInfo.Slice == null)
							continue; 
						row = WriteKeyValuesToRow(colValues, colSliceInfo.Columns, row);
						row = WriteKeyValuesToRow(rowValues, rowSliceInfo.Columns, row);
						for(int index = 0; index < dataFields.Count; index++)
							row[storageColumns[dataFields[index]]] = StorageValue.CreateUnbound(measureValues[index]);
						sliceInfo.Slice.AddRow(row);
					}
				}
			}
		}
		StorageRow AddMeasureValuesToRow(int colVisibleIndex, int rowVisibleIndex, StorageRow row) {
			foreach(PivotGridFieldBase dataField in dataFields)
				row[storageColumns[dataField]] = StorageValue.CreateUnbound(GetValue(colVisibleIndex, rowVisibleIndex, dataField));
			return row;
		}
		StorageRow WriteKeyValuesToRow(DimensionMemberValue[] values, StorageColumn[] columns, StorageRow row) {
			for(int i = 0; i < columns.Length; i++)
				row[columns[i]] = StorageValue.CreateUnbound(i < values.Length ? values[i].GetMainValue(isOlap) : DashboardSpecialValues.OlapNullValue);
			return row;
		}
		CrossSliceInfo GetCrossingSliceInfo(SliceInfo colSliceInfo, SliceInfo rowSliceInfo) {
			PivotGridFieldBase colField = colSliceInfo.Fields[colSliceInfo.Fields.Length - 1];
			PivotGridFieldBase rowField = rowSliceInfo.Fields[rowSliceInfo.Fields.Length - 1];
			return crossAxisSliceInfos[new CrossSliceKey(colField, rowField)];
		}
		List<DimensionMemberValue[]> GetAxisValues(IPivotGridDataSource ds, bool isColumn) {
			List<DimensionMemberValue[]> result = new List<DimensionMemberValue[]>();
			int count = ds.GetCellCount(isColumn);
			for(int index = 0; index < count; index++) {
				int level = ds.GetObjectLevel(isColumn, index);
				int valuesCount = level + 1;
				var values = new DimensionMemberValue[valuesCount];
				if(result.Count > 0) {
					var lastValues = result.Last();
					for(int i = 0; i < Math.Min(lastValues.Length, valuesCount - 1); i++)
						values[i] = lastValues[i];
				}
				values[values.Length - 1] = GetDimensionMemberValue(isColumn, index);
				result.Add(values);
			}
			return result;
		}
		DimensionMemberValue GetDimensionMemberValue(bool isColumn, int visibleIndex) {
			int level = dataSource.GetObjectLevel(isColumn, visibleIndex);
			DimensionMemberValue memberValue = new DimensionMemberValue();
			memberValue.VisibleIndex = visibleIndex;
			IPivotOLAPDataSource olapDs = dataSource as IPivotOLAPDataSource;
			if(olapDs != null) {
				IOLAPMember member = olapDs.GetMember(isColumn, visibleIndex);
				memberValue.UniqueName = member.UniqueName;
				memberValue.DisplayText = member.Caption;
				memberValue.Value = member.Value;
			} else
				memberValue.Value = dataSource.GetFieldValue(isColumn, visibleIndex, level);
			memberValue.Value = DashboardSpecialValuesInternal.ToSpecialUniqueValue(memberValue.Value);
			if(olapDs != null) {
				memberValue.DisplayText = DashboardSpecialValuesInternal.ToSpecialDisplayText(memberValue.Value, memberValue.DisplayText);
				memberValue.UniqueName = DashboardSpecialValuesInternal.ToSpecialUniqueName(memberValue.Value, memberValue.UniqueName);
			}
			return memberValue;
		}
		object GetValue(int colIndex, int rowIndex, PivotGridFieldBase dataField) {
			PivotCellValue pivotCellValue = dataSource.GetCellValue(colIndex, rowIndex, dataField.AreaIndex, dataField.SummaryType);
			object value = null;
			if(pivotCellValue == null)
				value = null;
			else if(pivotCellValue.Value is PivotErrorValue)
				value = DashboardSpecialValues.ErrorValue;
			else
				value = pivotCellValue.Value;
			return value;
		}
		DataStorage GetDataStorage() {
			return storage;
		}
		string[][] GetSortOrderSlices() {
			HashSet<string> colFieldNames = new HashSet<string>(colAxisFields.Select(f => f.Name));
			HashSet<string> rowFieldNames = new HashSet<string>(rowAxisFields.Select(f => f.Name));
			return requiredSlices == null ?
				new string[][] { } :
				requiredSlices.Where(s => colFieldNames.ContainsAny(s) ^ rowFieldNames.ContainsAny(s)).Select(s => s.ToArray()).ToArray();
		}
		public static DataStorageGeneratorResult Generate(PivotGridData data, IEnumerable<PivotGridFieldBase> dataFields, IEnumerable<SliceDescription> slices) {
			DataStorageGenerator generator = new DataStorageGenerator(data, dataFields, slices);
			return new DataStorageGeneratorResult() { Storage = generator.GetDataStorage(), SortOrderSlices = generator.GetSortOrderSlices() };
		}
	}
	public class SliceDescription : IEnumerable<string> {
		HashSet<string> dimensionNames;
		public SliceDescription(IEnumerable<string> dimensionNames) {
			this.dimensionNames = new HashSet<string>(dimensionNames);
		}
		public SliceDescription(IEnumerable<DimensionModel> dimensions)
			: this(dimensions.Select(d => d.Name)) {
		}
		internal bool IsSignatureEqual(IEnumerable<string> sliceColumnNames) {
			return dimensionNames.SetEquals(sliceColumnNames);
		}
		public IEnumerator<string> GetEnumerator() {
			return dimensionNames.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return dimensionNames.GetEnumerator();
		}
	}
}
