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
using DevExpress.Utils;
using FieldItemPair = DevExpress.Spreadsheet.FieldItemPair;
namespace DevExpress.XtraSpreadsheet.Model {
	#region GetPivotDataCalculator
	public static class GetPivotDataCalculator {
		public static VariantValue GetPivotData(PivotTable pivotTable, int dataFieldIndex, params FieldItemPair[] fieldItemPairIndices) {
			if (dataFieldIndex < 0 || dataFieldIndex >= pivotTable.DataFields.Count)
				return VariantValue.ErrorReference;
			WorkbookDataContext context = pivotTable.Worksheet.DataContext;
			Dictionary<int, AllowedItems> keyMap = new Dictionary<int, AllowedItems>(fieldItemPairIndices.Length);
			for (int i = 0; i < fieldItemPairIndices.Length; ++i) {
				FieldItemPair pair = fieldItemPairIndices[i];
				int fieldIndex = pair.FieldIndex;
				if (fieldIndex < 0 || fieldIndex >= pivotTable.Fields.Count)
					return VariantValue.ErrorReference;
				int itemIndex = pair.ItemIndex;
				PivotField field = pivotTable.Fields[fieldIndex];
				if (itemIndex < 0 || itemIndex >= field.Items.Count || field.Items[itemIndex].ItemType != PivotFieldItemType.Data)
					return VariantValue.ErrorReference;
				AddToKeyMap(keyMap, fieldIndex, itemIndex);
			}
			return FindValueOnWorksheet(pivotTable, keyMap, dataFieldIndex);
		}
		public static VariantValue GetPivotData(PivotTable pivotTable, VariantValue dataFieldName, params VariantValue[] fieldItemPairCaptions) {
			Dictionary<string, int> pivotFieldNames = new Dictionary<string, int>(pivotTable.Fields.Count, StringExtensions.ComparerInvariantCultureIgnoreCase);
			for (int i = 0; i < pivotTable.Fields.Count; ++i)
				pivotFieldNames.Add(pivotTable.GetFieldCaption(i), i);
			int dataFieldIndex;
			VariantValue error = TryGetDataFieldIndex(dataFieldName, pivotTable, pivotFieldNames, out dataFieldIndex);
			if (error.IsError)
				return error;
			Dictionary<int, AllowedItems> keyMap;
			error = TryGetKeyMap(fieldItemPairCaptions, pivotTable, pivotFieldNames, out keyMap);
			if (error.IsError)
				return error;
			return FindValueOnWorksheet(pivotTable, keyMap, dataFieldIndex);
		}
		static VariantValue TryGetDataFieldIndex(VariantValue dataFieldValue, PivotTable pivotTable, Dictionary<string, int> pivotFieldNames, out int dataFieldIndex) {
			dataFieldIndex = -1;
			if (dataFieldValue.IsError)
				return dataFieldValue;
			PivotDataFieldCollection dataFields = pivotTable.DataFields;
			if (dataFieldValue.IsCellRange) {
			}
			string stringValue = dataFieldValue.ToText(pivotTable.Worksheet.Workbook.DataContext).InlineTextValue;
			if (string.IsNullOrEmpty(stringValue))
				return VariantValue.ErrorReference;
			int fieldIndex;
			if (pivotFieldNames.TryGetValue(stringValue, out fieldIndex)) {
				for (int j = dataFields.Count - 1; j >= 0; --j) {
					PivotDataField dataField = dataFields[j];
					if (dataField.FieldIndex == fieldIndex) {
						dataFieldIndex = j;
						if (dataField.Subtotal == PivotDataConsolidateFunction.Sum)
							return VariantValue.Empty;
					}
				}
				if (dataFieldIndex >= 0)
					return VariantValue.Empty;
			}
			else
				for (int i = 0; i < dataFields.Count; ++i)
					if (StringExtensions.CompareInvariantCultureIgnoreCase(stringValue, dataFields[i].Name) == 0) {
						dataFieldIndex = i;
						return VariantValue.Empty;
					}
			return VariantValue.ErrorReference;
		}
		static VariantValue TryGetKeyMap(VariantValue[] arguments, PivotTable pivotTable, Dictionary<string, int> pivotFieldNames, out Dictionary<int, AllowedItems> keyMap) {
			if (arguments.Length % 2 != 0) {
				keyMap = null;
				return VariantValue.ErrorReference;
			}
			WorkbookDataContext context = pivotTable.Worksheet.DataContext;
			keyMap = new Dictionary<int, AllowedItems>(arguments.Length / 2);
			for (int i = 0; i < arguments.Length; i += 2) {
				VariantValue fieldValue = arguments[i];
				if (fieldValue.IsError)
					return fieldValue;
				VariantValue itemValue = arguments[i + 1];
				int fieldIndex;
				string fieldName = fieldValue.ToText(context).InlineTextValue;
				if (!pivotFieldNames.TryGetValue(fieldName, out fieldIndex))
					return VariantValue.ErrorReference;
				int itemIndex = -1;
				string itemString = VariantValueToText.ToString(itemValue, context);
				IPivotCacheField cacheField = pivotTable.Cache.CacheFields[fieldIndex];
				for (int j = 0; j < cacheField.SharedItems.Count; ++j) {
					VariantValue cacheItemValue = cacheField.SharedItems[pivotTable.Fields[fieldIndex].Items[j].ItemIndex].ToVariantValue(cacheField, context);
					if (StringExtensions.CompareInvariantCultureIgnoreCase(itemString, VariantValueToText.ToString(cacheItemValue, context)) == 0) {
						itemIndex = j;
						break;
					}
				}
				if (itemIndex < 0)
					return VariantValue.ErrorReference;
				AddToKeyMap(keyMap, fieldIndex, itemIndex);
			}
			return VariantValue.Empty;
		}
		static void AddToKeyMap(Dictionary<int, AllowedItems> keyMap, int fieldIndex, int itemIndex) {
			AllowedItems allowedItems;
			if (!keyMap.TryGetValue(fieldIndex, out allowedItems)) {
				allowedItems = new AllowedItems();
				keyMap.Add(fieldIndex, allowedItems);
			}
			allowedItems.Add(itemIndex);
		}
		static VariantValue FindValueOnWorksheet(PivotTable pivotTable, Dictionary<int, AllowedItems> keyMap, int dataFieldIndex) {
			int rowItemIndex = FindLayoutItemIndex(pivotTable, pivotTable.RowFields, pivotTable.RowItems, keyMap, dataFieldIndex, false);
			if (rowItemIndex < 0)
				return VariantValue.ErrorReference;
			int columnItemIndex = FindLayoutItemIndex(pivotTable, pivotTable.ColumnFields, pivotTable.ColumnItems, keyMap, dataFieldIndex, true);
			if (columnItemIndex < 0)
				return VariantValue.ErrorReference;
			if (keyMap.Count > 0)
				return VariantValue.ErrorReference;
			ICellBase cell = pivotTable.Range.TryGetCellRelative(columnItemIndex + pivotTable.Location.FirstDataColumn, rowItemIndex + pivotTable.Location.FirstDataRow);
			if (cell == null)
				return VariantValue.Empty;
			return cell.Value;
		}
		static int FindLayoutItemIndex(PivotTable pivotTable, PivotTableColumnRowFieldIndices references, PivotLayoutItems layoutItems, Dictionary<int, AllowedItems> keyMap, int dataFieldIndex, bool column) {
			if (references.KeyIndicesCount == 0)
				return references.HasValuesField ? dataFieldIndex : 0;
			int oldCount = keyMap.Count;
			List<AllowedItems> itemIndices = GetItemIndices(references, dataFieldIndex, keyMap);
			if (oldCount == keyMap.Count) { 
				int itemIndex = references.HasValuesField ? layoutItems.Count - pivotTable.DataFields.Count + dataFieldIndex : layoutItems.Count - 1;
				return layoutItems[itemIndex].Type == PivotFieldItemType.Grand ? itemIndex : -1;
			}
			int prevEqualsCount = 0;
			int firstResultIndex = -1;
			int lastResultIndex = int.MaxValue;
			PivotFieldItemType lastType = PivotFieldItemType.Blank;
			for (int layoutItemIndex = 0; layoutItemIndex < layoutItems.Count; ++layoutItemIndex) {
				IPivotLayoutItem item = layoutItems[layoutItemIndex];
				if ((item.Type & (PivotFieldItemType.Data | PivotFieldItemType.DefaultValue)) == 0)
					continue;
				if (KeyMismatch(item, itemIndices, ref prevEqualsCount))
					continue;
				if (lastResultIndex < layoutItemIndex - 1)
					return -1;
				if (firstResultIndex < 0)
					firstResultIndex = layoutItemIndex;
				lastResultIndex = layoutItemIndex;
				lastType = item.Type;
			}
			if (firstResultIndex >= 0) {
				if (firstResultIndex == lastResultIndex || lastType == PivotFieldItemType.DefaultValue)
					return lastResultIndex;
				if (SubtotalOnTop(pivotTable, references, itemIndices.Count - 1, column))
					return firstResultIndex;
			}
			return -1;
		}
		static bool KeyMismatch(IPivotLayoutItem item, List<AllowedItems> itemIndices, ref int prevEqualsCount) {
			if (prevEqualsCount < item.RepeatedItemsCount)
				return true;
			prevEqualsCount = item.RepeatedItemsCount;
			for (int i = item.RepeatedItemsCount; i < itemIndices.Count; ++i) {
				int itemIndexPos = i - item.RepeatedItemsCount;
				if (item.PivotFieldItemIndices.Length <= itemIndexPos)
					return true;
				int actualItemIndex = item.PivotFieldItemIndices[itemIndexPos];
				AllowedItems allowedItems = itemIndices[i];
				if (allowedItems.Count > 0 && !allowedItems.Contains(actualItemIndex))
					return true;
				++prevEqualsCount;
			}
			return false;
		}
		static bool SubtotalOnTop(PivotTable pivotTable, PivotTableColumnRowFieldIndices references, int fieldReferenceIndex, bool column) {
			if (!column) {
				int fieldIndex = references[fieldReferenceIndex].FieldIndex;
				PivotField field = pivotTable.Fields[fieldIndex];
				if (field.Outline && field.SubtotalTop) {
					int subtotalCount = 0;
					for (int i = field.Items.Count - 1; i >= 0; --i) {
						PivotItem item = field.Items[i];
						if (item.ItemType == PivotFieldItemType.Data)
							break;
						++subtotalCount;
					}
					if (subtotalCount == 1)
						return true;
				}
			}
			return false;
		}
		static List<AllowedItems> GetItemIndices(PivotTableColumnRowFieldIndices references, int dataFieldIndex, Dictionary<int, AllowedItems> keyMap) {
			int lastItemIndex = -1;
			List<AllowedItems> itemIndices = new List<AllowedItems>(references.Count);
			for (int i = 0; i < references.Count; ++i) {
				int fieldIndex = references[i].FieldIndex;
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex) {
					lastItemIndex = itemIndices.Count;
					itemIndices.Add(new AllowedItems() { dataFieldIndex });
				}
				else {
					AllowedItems allowedItems;
					if (keyMap.TryGetValue(fieldIndex, out allowedItems)) {
						lastItemIndex = itemIndices.Count;
						itemIndices.Add(allowedItems);
						keyMap.Remove(fieldIndex);
					}
					else
						itemIndices.Add(AllowedItems.Empty);
				}
			}
			++lastItemIndex;
			itemIndices.RemoveRange(lastItemIndex, itemIndices.Count - lastItemIndex);
			return itemIndices;
		}
		class AllowedItems : List<int> {
			public static AllowedItems Empty = new AllowedItems();
		}
	}
	#endregion
	#region FunctionGetPivotData
	public class FunctionGetPivotData : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "GETPIVOTDATA"; } }
		public override int Code { get { return 0x0166; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue pivotValue = arguments[1];
			if (pivotValue.IsError)
				return pivotValue;
			if (!pivotValue.IsCellRange)
				return VariantValue.ErrorReference;
			Worksheet sheet = pivotValue.CellRangeValue.Worksheet as Worksheet;
			if (sheet == null)
				sheet = context.CurrentWorksheet as Worksheet;
			if (sheet == null)
				return VariantValue.ErrorReference;
			List<PivotTable> pivotTables = sheet.PivotTables.GetItems(pivotValue.CellRangeValue, true);
			if (pivotTables.Count == 0)
				return VariantValue.ErrorReference;
			PivotTable pivotTable = pivotTables[pivotTables.Count - 1];
			VariantValue dataField = arguments[0];
			List<VariantValue> values = arguments as List<VariantValue>;
			values.RemoveRange(0, 2);
			return GetPivotDataCalculator.GetPivotData(pivotTable, dataField, values.ToArray());
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
}
