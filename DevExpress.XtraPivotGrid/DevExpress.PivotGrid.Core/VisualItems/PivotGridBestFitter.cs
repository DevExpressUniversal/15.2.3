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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraPivotGrid.Data {
	public interface IPivotFieldValueItem {
		PivotFieldValueItem Item { get; }
		int GetBestWidth();
	}
	public interface IFieldValueAreaBestFitProvider {
		void BeginBestFitCalculcations();
		void EndBestFitCalculcations();
		int ChildCount { get; }
		bool IsColumn { get; }
		IPivotFieldValueItem this[int index] { get; }
	}
	public interface ICellsBestFitProvider {
		bool BestFitConsiderCustomAppearance { get; }
		int VertLinesWidth { get; }
		void BeginBestFitCalculcations();
		void EndBestFitCalculcations();
		int GetCellStringWidth(PivotFieldItemBase field, string text);
		int GetTotalCellStringWidth(PivotFieldItemBase field, string text);
		int GetGrandTotalCellStringWidth(PivotFieldItemBase field, string text);
		int GetItemTextWidth(PivotFieldItemBase field, PivotGridCellItem item, Rectangle? bounds);
	}
	public abstract class PivotGridBestFitterBase {
		readonly PivotGridData data;
		CellSizeProvider sizeProvider;
		int bestFitCount = 0;
		Dictionary<PivotFieldItemBase, int> dataFieldWidths = new Dictionary<PivotFieldItemBase, int>();
		public CellSizeProvider CellSizeProvider { get { return sizeProvider; } }
		PivotFieldItemCollection FieldItems { get { return data.FieldItems; } }
		PivotVisualItemsBase VisualItems { get { return data.VisualItems; } }
		int ColumnCount { get { return VisualItems.GetLastLevelItemCount(true); } }
		int RowCount { get { return VisualItems.GetItemsCreator(false).LastLevelUnpagedItemCount; } }
		protected abstract IFieldValueAreaBestFitProvider RowAreaFields { get; }
		protected abstract IFieldValueAreaBestFitProvider ColumnAreaFields { get; }
		protected internal abstract ICellsBestFitProvider CellsArea { get; }
		protected PivotGridData Data { get { return data; } }
		public PivotGridBestFitterBase(PivotGridData data) {
			this.data = data;
		}
		public void SetSizeProvider(CellSizeProvider sizeProvider) {
			this.sizeProvider = sizeProvider;
		}
		public virtual void BeginBestFit() {
			data.RaiseInvalidOperationOnLockedUpdateException();
			data.VisualItems.EnsureIsCalculated();
			bestFitCount++;
		}
		public void EndBestFit() {
			bestFitCount--;
			if(bestFitCount == 0) {
				if(this.dataFieldWidths == null)
					return;
				this.dataFieldWidths.Clear();
			}
		}
		protected int GetDataFieldWidth(PivotFieldItemBase field) {
			int width;
			if(!dataFieldWidths.TryGetValue(field, out width)) {
				width = GetDataFieldBestWidthCore(field);
				dataFieldWidths.Add(field, width);
			}
			return width;
		}
		public virtual void BestFitRowField(PivotFieldItemBase field) {
			BeginBestFit();
			int headerWidth = 0, valueWidth = 0;
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.FieldHeader))
				headerWidth = CellSizeProvider.CalculateHeaderWidth(field);
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.FieldValue))
				valueWidth = GetFieldValuesBestWidth(RowAreaFields, field);
			EndBestFit();
			if(headerWidth == valueWidth && valueWidth == 0)
				return;
			new PivotGridFieldUISetWidthAction(field, data).SetWidth(Math.Max(headerWidth, valueWidth));
		}
		public virtual void BestFitDataField(PivotFieldItemBase field) {
			BeginBestFit();
			int rowValueWidth = 0, columnValueWidth = 0, cellWidth = 0;
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.FieldValue)) {
				rowValueWidth = GetFieldValuesBestWidth(RowAreaFields, field);
				columnValueWidth = GetFieldValuesBestWidth(ColumnAreaFields, field);
			}
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.Cell))
				cellWidth = GetDataFieldWidth(field);
			EndBestFit();
			if(FieldItems.DataFieldCount == 1 && FieldItems.RowFieldCount == 0) {
				if(rowValueWidth == cellWidth && cellWidth == 0)
					return;
				new PivotGridFieldUISetWidthAction(field, data).SetWidth(Math.Max(rowValueWidth, cellWidth));
			} else {
				if(columnValueWidth == cellWidth && cellWidth == 0)
					return;
				new PivotGridFieldUISetWidthAction(field, data).SetWidth(Math.Max(columnValueWidth, cellWidth));
			}
		}
		public virtual void BestFitColumnField(PivotFieldItemBase field) {
			BeginBestFit();
			bool isDataFieldLastInArea = data.OptionsDataField.Area == PivotDataArea.ColumnArea && data.OptionsDataField.AreaIndex == FieldItems.GetFieldCountByArea(PivotArea.ColumnArea);
			List<PivotFieldItemBase> fields = GetFieldsWithValues(FieldItems.GetFieldItemsByArea(PivotArea.DataArea, false));
			if(fields.Count > 1 && isDataFieldLastInArea) {
				EndBestFit();
				return;
			}
			int width = 0;
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.FieldValue))
				width = GetFieldValuesBestWidth(ColumnAreaFields, field);
			if(data.OptionsBehavior.IsBestFitEnabled(PivotGridBestFitMode.Cell)) {
				foreach(PivotFieldItemBase dataField in fields) {
					if(dataField.Options.ShowValues)
						width = Math.Max(width, GetDataFieldWidth(dataField));
				}
			}
			if(width != 0) {
				new PivotGridFieldUISetWidthAction(field, data).SetWidth(width);
				if(ColumnAreaFieldsHasSmallChildCells(field)) {
					PivotFieldItemBase childField = FieldItems.GetFieldItemByArea(field.Area, field.AreaIndex + 1);
					if(childField != null)
						BestFit(childField);
				}
			}
			EndBestFit();
		}
		public void BestFit() {
			BeginBestFit();
			BestFitColumnArea();
			BestFitRowArea();
			EndBestFit();
		}
		public void BestFitColumnArea() {
			BeginBestFit();
			if(FieldItems.DataFieldItem.Visible && FieldItems.DataFieldItem.AreaIndex == 0) {
				BestFitCore(PivotArea.DataArea);
				BestFitCore(PivotArea.ColumnArea);
			} else {
				BestFitCore(PivotArea.ColumnArea);
				BestFitCore(PivotArea.DataArea);
			}
			EndBestFit();
		}
		public void BestFitRowArea() {
			BeginBestFit();
			BestFitCore(PivotArea.RowArea);
			if(data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
				BestFit(FieldItems.RowTreeFieldItem);
			EndBestFit();
		}
		public void BestFitDataHeaders(bool considerRowArea) {
			List<PivotFieldItemBase> dataFields = FieldItems.GetFieldItemsByArea(PivotArea.DataArea, true);
			if(dataFields.Count == 0) return;
			int dataFieldsWidth = 0;
			foreach(PivotFieldItemBase field in dataFields) {
				dataFieldsWidth += CellSizeProvider.CalculateHeaderWidth(field);
			}
			BeginBestFit();
			if(data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree) {
				PivotFieldItemBase rowTreeField = FieldItems.RowTreeFieldItem;
				if(!considerRowArea || dataFieldsWidth > rowTreeField.Width)
					new PivotGridFieldUISetWidthAction(rowTreeField, data).SetWidth(dataFieldsWidth - data.OptionsView.RowTreeOffset + CellSizeProvider.HeaderWidthOffset);
			} else {
				List<PivotFieldItemBase> rowFields = FieldItems.GetFieldItemsByArea(PivotArea.RowArea, true);
				int rowFieldsWidth = 0, rowFieldsCount = rowFields.Count;
				foreach(PivotFieldItemBase field in rowFields) {
					rowFieldsWidth += field.Width;
				}
				if(rowFieldsCount > 0 && (!considerRowArea || dataFieldsWidth > rowFieldsWidth)) {
					int increment = (dataFieldsWidth - rowFieldsWidth) / rowFieldsCount;
					foreach(PivotFieldItemBase field in rowFields) {
						int newWidth = field.Width + increment + CellSizeProvider.CalculateHeaderWidthOffset(field);
						new PivotGridFieldUISetWidthAction(field, data).SetWidth(newWidth);
					}
				}
			}
			EndBestFit();
		}
		public void BestFit(PivotFieldItemBase field) {
			if(field == null || !field.Visible || field.Area == PivotArea.FilterArea)
				return;
			BeginBestFit();
			switch(field.Area) {
				case PivotArea.RowArea:
					BestFitRowField(field);
					break;
				case PivotArea.DataArea:
					BestFitDataField(field);
					break;
				case PivotArea.ColumnArea:
					BestFitColumnField(field);
					break;
			}
			EndBestFit();
		}
		void BestFitCore(PivotArea area) {
			List<PivotFieldItemBase> fields = FieldItems.GetFieldItemsByArea(area, true);
			foreach(PivotFieldItemBase field in fields) {
				BestFit(field);
			}
		}
		bool ColumnAreaFieldsHasSmallChildCells(PivotFieldItemBase field) {
			for(int i = 0; i < data.VisualItems.GetItemCount(true); i++) {
				PivotFieldValueItem item = data.VisualItems.GetItem(true, i);
				if(field.Equals(item.ResizingField) && sizeProvider.GetWidthDifference(true, item.StartLevel, item.EndLevel + 1) < field.Width)
					return true;
			}
			return false;
		}
		List<PivotFieldItemBase> GetFieldsWithValues(List<PivotFieldItemBase> fields) {
			List<PivotFieldItemBase> fieldsWithValues = new List<PivotFieldItemBase>();
			foreach(PivotFieldItemBase field in fields)
				if(field.Options.ShowValues || field.Options.ShowCustomTotals || field.Options.ShowGrandTotal)
					fieldsWithValues.Add(field);
			return fieldsWithValues;
		}
		#region field value
		protected int GetFieldValuesBestWidth(IFieldValueAreaBestFitProvider itemsProvider, PivotFieldItemBase field) {
			itemsProvider.BeginBestFitCalculcations();
			int bestWidth = 0;
			try {
				int childCount = itemsProvider.ChildCount;
				if(itemsProvider.IsColumn) {
					for(int i = 0; i < childCount; i++) {
						IPivotFieldValueItem item = itemsProvider[i];
						PivotFieldValueItem fieldValue = item.Item;
						if(field.Equals(fieldValue.ResizingField) || childCount == 1) {
							bestWidth = Math.Max(bestWidth, item.GetBestWidth());
							PivotFieldValueItem parentItem = VisualItems.GetItemsCreator(true).GetParentUnpagedItem(fieldValue);
							if(parentItem != null && parentItem.ResizingField != null && parentItem.CellCount > 0)
								bestWidth = Math.Max(bestWidth, parentItem.ResizingField.Width / parentItem.CellCount);
						}
					}
				} else {
					int lCount = VisualItems.GetItemsCreator(false).GetUnpagedItems().LevelCount;
					for(int i = 0; i < childCount; i++) {
						IPivotFieldValueItem item = itemsProvider[i];
						PivotFieldValueItem fieldValue = item.Item;
						if(!field.Equals(fieldValue.ResizingField) && lCount != 1)
							continue;
						int cellBestWidth = item.GetBestWidth();
						if(fieldValue.IsCollapsed && !fieldValue.IsRowTree)
							cellBestWidth -= GetRowAreaLowerFieldsWidth(field.AreaIndex);
						else if(fieldValue.IsRowTree)
							cellBestWidth = cellBestWidth + VisualItems.GetRowTreeWidthItemDiff(fieldValue);
						bestWidth = Math.Max(bestWidth, cellBestWidth);
					}
				}
			} finally {
				itemsProvider.EndBestFitCalculcations();
			}
			return bestWidth > 0 ? bestWidth : 0;
		}
		int GetRowAreaLowerFieldsWidth(int startAreaIndex) {
			int res = 0;
			for(int i = startAreaIndex + 1; i < FieldItems.GetFieldCountByArea(PivotArea.RowArea); i++) {
				res += FieldItems.GetFieldItemByArea(PivotArea.RowArea, i).Width;
			}
			return res;
		}
		#endregion
		#region cell
		public int GetDataFieldBestWidthCore(PivotFieldItemBase field) {
			if(field.Area != PivotArea.DataArea)
				return 0;
			int width = 0;
			CellsArea.BeginBestFitCalculcations();
			try {
				if(!CellsArea.BestFitConsiderCustomAppearance) {
					string text = GetBiggestText(field);
					width = GetBestWidthCore(field, text);
				} else
					width = GetBiggestCellWidth(field);
			} finally {
				CellsArea.EndBestFitCalculcations();
			}
			return width + CellSizeProvider.LeftCellPadding + CellSizeProvider.RightCellPadding + CellsArea.VertLinesWidth + 1;
		}
		int GetBestWidthCore(PivotFieldItemBase field, string maxText) {
			int cellWidth = CellsArea.GetCellStringWidth(field, maxText),
				totalWidth = CellsArea.GetTotalCellStringWidth(field, maxText),
				grandTotalWidth = CellsArea.GetGrandTotalCellStringWidth(field, maxText);
			return Math.Max(Math.Max(cellWidth, totalWidth), grandTotalWidth);
		}
		int GetBiggestCellWidth(PivotFieldItemBase field) {
			int biggestWidth = 0;
			string text = string.Empty;
			for(int i = 0; i < RowCount; i++) {
				for(int j = 0; j < ColumnCount; j++) {
					PivotGridCellItem item = CreatCellItem(i, j);
					if(!item.DataField.Equals(field))
						continue;
					text = item.Text ?? string.Empty;
					biggestWidth = Math.Max(CellsArea.GetItemTextWidth(field, item, null), biggestWidth);
				}
			}
			return biggestWidth;
		}
		protected virtual PivotGridCellItem CreatCellItem(int rowIndex, int columnIndex) {
			return VisualItems.CreateUnpagedCellItem(columnIndex, rowIndex);
		}
		string GetBiggestText(PivotFieldItemBase field) {
			string biggestText = string.Empty;
			for(int i = 0; i < RowCount; i++) {
				for(int j = 0; j < ColumnCount; j++) {
					PivotGridCellItem item = VisualItems.CreateUnpagedCellItem(j, i);
					if(!item.DataField.Equals(field))
						continue;
					string text = item.Text;
					if(!string.IsNullOrEmpty(text) && text.Length > biggestText.Length)
						biggestText = text;
				}
			}
			return biggestText;
		}
		#endregion
		#region header
		#endregion
	}
}
