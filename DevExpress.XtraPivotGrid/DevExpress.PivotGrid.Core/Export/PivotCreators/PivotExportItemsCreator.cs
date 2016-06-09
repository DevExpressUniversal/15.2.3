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
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Export {
	public abstract class PivotExportItemsCreator<T> : IDisposable where T : IPivotExportItem<T> {
		static bool IsDataItem(PivotFieldValueItem item) {
			return item.IsLastFieldLevel && item.ItemType == PivotFieldValueItemType.DataCell;
		}
		List<T> items;
		PivotGridData pivotData;
		PivotVisualItemsBase visualItems;
		IDataAwareExportOptions exportOptions;
		readonly bool allowGrouping;
		readonly int firstAreaMeasure;
		readonly int lastAreaLevel;
		readonly int areaCount;
		long itemsCount;
		int position;
		int exportIndex;
		int handle = -1;
		protected abstract bool IsColumnCreator { get; }
		protected PivotVisualItemsBase VisualItems { get { return visualItems; } }
		protected int FirstAreaMeasure { get { return firstAreaMeasure; } }
		protected PivotExportItemsCreator(PivotGridData pivotData, IDataAwareExportOptions exportOptions, int firstAreaMeasure, int lastAreaLevel, int areaCount) {
			this.pivotData = pivotData;
			this.visualItems = pivotData.VisualItems;
			this.exportOptions = exportOptions;
			this.allowGrouping = exportOptions.AllowGrouping == DefaultBoolean.True;
			this.firstAreaMeasure = firstAreaMeasure;
			this.lastAreaLevel = lastAreaLevel;
			this.areaCount = areaCount;
		}
		FormatInfo GetFormatInfo(PivotFieldValueItem valueItem) {
			PivotFieldItemBase dataField = valueItem.DataField;
			PivotGridFieldBase pivotGridField = pivotData.GetField(dataField);
			FormatInfo formatInfo = null;
			bool isTotal = false;
			bool isGrandTotal = false;
			PivotGridCustomTotalBase customTotal = null;
			switch(valueItem.ItemType) {
				case PivotFieldValueItemType.CustomTotalCell:
					customTotal = valueItem.CustomTotal;
					formatInfo = customTotal.CellFormat;
					break;
				case PivotFieldValueItemType.TotalCell:
					isTotal = true;
					if(pivotGridField != null)
						formatInfo = pivotGridField.TotalCellFormat;
					break;
				case PivotFieldValueItemType.GrandTotalCell:
					isGrandTotal = true;
					if(pivotGridField != null)
						formatInfo = pivotGridField.GrandTotalCellFormat;
					break;
			}
			if(formatInfo == null || formatInfo.IsEmpty)
				formatInfo = PivotGridCellItem.GetFormatInfo(() => null, dataField, customTotal, isTotal, isGrandTotal);
			return formatInfo;
		}
		Type GetActualDataType(PivotFieldValueItem valueItem) {
			PivotFieldItemBase dataField = valueItem.DataField;
			if(dataField != null)
				return dataField.DataType;
			return null;
		}
		protected FormatSettings CreateFormatSettings(PivotFieldValueItem valueItem) {
			FormatSettings settings = new FormatSettings();
			FormatInfo formatInfo = GetFormatInfo(valueItem);
			if(formatInfo != null) {
				settings.FormatType = formatInfo.FormatType;
				settings.FormatString = formatInfo.FormatString;
			}
			settings.ActualDataType = GetActualDataType(valueItem);
			return settings;
		}
		protected FormatSettings CreateGeneralFormatSettings() {
			return new FormatSettings();
		}
		bool CheckCollapse(PivotFieldValueItem valueItem) {
			switch(exportOptions.GroupState) {
				case GroupState.CollapseAll:
					return true;
				case GroupState.ExpandAll:
					return false;
				default:
					return valueItem.IsCollapsed;
			}
		}
		void CreateExportItems() {
			items = new List<T>();
			for(int i = 0; i < firstAreaMeasure; i++)
				items.Add(CreateAreaExportItem(i));
			position = items.Count;
			itemsCount = items.Count;
			exportIndex = 0;
			const int level = 0;
			for(int i = 0; i < areaCount; i++) {
				if(allowGrouping) {
					PivotFieldValueItem item = visualItems.GetUnpagedItem(IsColumnCreator, i, 0);
					FillExportItem(items, item, level, CheckCollapse(item));
					i = item.MaxLastLevelIndex;
				}
				else {
					PivotFieldValueItem item = visualItems.GetUnpagedItem(IsColumnCreator, i, lastAreaLevel);
					items.Add(CreateSingleExportItem(item, level));
				}
			}
		}
		void FillExportItem(IList<T> items, PivotFieldValueItem item, int level, bool collapsed) {
			int childsCount = item.UnpagedCellCount;
			if(childsCount > 0) {
				PivotFieldValueItem firstChild = item.GetUnpagedCell(0);
				IList<T> itemCollection = items;
				if(!IsDataItem(firstChild)) {
					T groupItem = CreateGroupExportItem(item, handle--, level++, collapsed);
					itemsCount++;
					itemCollection = groupItem.Items;
					items.Add(groupItem);
				}
				for(int i = 0; i < childsCount; i++) {
					PivotFieldValueItem childItem = i == 0 ? firstChild : item.GetUnpagedCell(i);
					FillExportItem(itemCollection, childItem, level, CheckCollapse(childItem));
				}
			}
			else
				items.Add(CreateSingleExportItem(item, level));
		}
		T CreateSingleExportItem(PivotFieldValueItem item, int level) {
			itemsCount++;
			return CreateSingleExportItem(item, position++, exportIndex++, level);
		}
		protected abstract T CreateAreaExportItem(int index);
		protected abstract T CreateSingleExportItem(PivotFieldValueItem item, int logicalPosition, int exportIndex, int level);
		protected abstract T CreateGroupExportItem(PivotFieldValueItem item, int logicalPosition, int level, bool collapsed);
		public IList<T> GetExportItems() {
			if(items == null)
				CreateExportItems();
			return items;
		}
		public long GetItemsCount() {
			if(items == null)
				CreateExportItems();
			return itemsCount;
		}
		public void Dispose() {
			items.Clear();
			items = null;
			pivotData = null;
			visualItems = null;
			exportOptions = null;
		}
	}
}
