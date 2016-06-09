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
using System.Windows;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class BestFitResult {
		public BestFitResult(){ }
		public List<int> NewSizes { get; set; }
		public List<PivotFieldValueItem> ValueItems { get; set; }
	}
	public class BestFitter {
		PivotGridWpfData data;
		BestWidthCalculator.BestFitCacheHelper CacheHelper { get { return Data.BestWidthCalculator.CacheHelper; } }
		public BestFitter(PivotGridWpfData data) {
			this.data = data;
		}
		protected PivotGridWpfData Data { get { return data; } }
		protected PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		public void BestFitColumn(int columnIndex) {
			Data.RaiseInvalidOperationOnLockedUpdateException();
			BestFitColumn column = new BestFitColumn(Data, columnIndex);
			int newWidth = CalcBestWidth(column);
			if(newWidth > 0) {
				column.SetSize(newWidth);
				UpdateScrollbars();
			}
		}
		public void BestFitRow(int rowIndex) {
			Data.RaiseInvalidOperationOnLockedUpdateException();
			BestFitRow row = new BestFitRow(Data, rowIndex);
			int newHeight = CalcBestHeight(row);
			if(newHeight > 0) {
				row.SetSize(newHeight);
				UpdateScrollbars();
			}
		}
		public void BestFit(bool isTreeRowTotalsLocation, bool fitWidth, bool fitHeight) {
			CacheHelper.BeginBestFit(fitHeight, fitHeight);
			BestFit(FieldArea.RowArea, isTreeRowTotalsLocation, fitWidth, fitHeight);
			BestFit(FieldArea.ColumnArea, isTreeRowTotalsLocation, fitWidth, fitHeight);
			CacheHelper.EndBestFit();
		}
		public void BestFit(FieldArea area, bool isTreeRowTotalsLocation, bool fitWidth, bool fitHeight) {
			if(!area.IsColumnOrRow()) return;
			Data.RaiseInvalidOperationOnLockedUpdateException();
			if(area == FieldArea.RowArea) {
				if(isTreeRowTotalsLocation) {
					BestFit(VisualItems.RowTreeField, fitWidth, false);
					if(Data.DataField.Area == FieldArea.RowArea && Data.DataField.InternalField.Visible)
						BestFit(Data.DataField, fitWidth, false);
				} else
					foreach(PivotGridField field in Data.GetFieldsByArea(area, true))
						BestFit(field, fitWidth, false);
				if(fitHeight)
					BestFitRowAreaHeight();
				return;
			} else
				BestFitColumns(fitWidth, fitHeight);
		}
		void BestFitRowAreaHeight() {
			if(Data.PivotGrid.BestFitMode == BestFitMode.VisibleRows) {
				int start = Data.PivotGrid.PivotGridScroller.Cells.Top;
				double ah = Data.PivotGrid.PivotGridScroller.Cells.ActualHeight;
				while(ah > 0 && ah < data.VisualItems.RowCount) {
					BestFitRow(start);
					ah -= Data.VisualItems.GetHeightDifference(start, start + 1, false);
					start++;
				}
			} else {
				for(int i = 0; i < Data.VisualItems.RowCount; i ++)
					BestFitRow(i);
			}
		}
		public void BestFit(PivotGridField field, bool fitWidth, bool fitHeight) {
			Data.RaiseInvalidOperationOnLockedUpdateException();
			if(!field.Visible && field.InternalField.IsDataField)
				BestFitDataField(fitWidth, fitHeight);
			if(!field.Visible || (!fitWidth && !fitHeight)) return;
			switch(field.Area) {
				case FieldArea.RowArea:
					BestFitRowField(field, fitWidth, fitHeight);
					break;
				case FieldArea.ColumnArea:
					BestFitColumnField(field, fitWidth, fitHeight);
					break;
				case FieldArea.DataArea:
					switch(Data.DataField.Area) {
						case FieldArea.RowArea:
							BestFitRowField(field, fitWidth, fitHeight);
							break;
						case FieldArea.ColumnArea:
							BestFitColumnField(field, fitWidth, fitHeight);
							break;
					}
					break;
			}
		}
		void BestFitDataField(bool fitWidth, bool fitHeight) {
			if(!fitHeight || fitWidth) return;
			BestFitColumnField(Data.DataField, fitWidth, true);
		}
		void BestFitRowField(PivotGridField field, bool fitWidth, bool fitHeight) {
			RowFieldBestFitColumn column = new RowFieldBestFitColumn(Data, field);
			if(fitWidth) {
				int newWidth = CalcBestWidth(column);
				if(newWidth > 0) {
					column.SetSize(newWidth);
					UpdateScrollbars();
				}
			}
			if(fitHeight) {
				int oldWidth = Data.PivotGrid.PivotGridScroller.ViewportHeight;
				int oldLeft = Data.PivotGrid.PivotGridScroller.VerticalOffset;
				BestFitResult result = CalcBestHeights(column);
				if(column.SetSizes(result, false))
					UpdateScrollbars();
				if(column.AppliedBestFitMode == BestFitMode.VisibleRows && fitHeight)
					BestFitShowedRows(oldWidth, oldLeft, result);
			}			
		}
		void BestFitColumnField(PivotGridField field, bool fitWidth, bool fitHeight) {
			ColumnFieldBestFitColumn column = new ColumnFieldBestFitColumn(Data, field);
			if(fitHeight) {
				int newHeight = CalcBestHeight(column);
				if(newHeight > 0)
					column.SetSize(newHeight);
			}
			if(fitWidth) {
				if(column.SetSizes(CalcBestWidths(column), true))
					UpdateScrollbars();
			}
		}
		void BestFitColumns(bool fitWidth, bool fitHeight) {
			int oldWidth = Data.PivotGrid.PivotGridScroller.ViewportWidth;
			int oldLeft = Data.PivotGrid.PivotGridScroller.HorizontalOffset;
			BestFitLevelFieldValues curColumn = null, prevColumn = null;
			BestFitResult sizesCurLevel = null, sizesPrevLevel = null;
			int count = VisualItems.GetLevelCount(true);
			for(int i = 0; i < count; i++) {
				curColumn = new BestFitLevelFieldValues(Data, i);
				if(fitWidth) {
					sizesCurLevel = CalcBestWidths(curColumn);
					if(prevColumn != null) {
						prevColumn.UpdateSizes(sizesPrevLevel, sizesCurLevel);
					}
					sizesPrevLevel = sizesCurLevel;
					prevColumn = curColumn;
				}
				if(fitHeight) {
					int newHeight = CalcBestHeight(curColumn);
					if(newHeight > 0)
						curColumn.SetSize(newHeight); ;
				}
			}
			if(!fitWidth)
				return;
			SetColumnSizesCore(curColumn, sizesCurLevel);
			if(curColumn.AppliedBestFitMode == BestFitMode.VisibleRows)
				BestFitShowedColumns(oldWidth, oldLeft, sizesCurLevel);
		}
		void BestFitShowedColumns(int oldWidth, int oldLeft, BestFitResult sizesCurLevel) {
			double actualWidth = Data.PivotGrid.PivotGridScroller.Cells.ActualWidth;
			foreach(PivotFieldValueItem item in sizesCurLevel.ValueItems)
				actualWidth -= VisualItems.GetItemWidth(item.MinLastLevelIndex, true);
			int startItem = oldWidth + oldLeft;
			while(actualWidth > 0 && startItem < VisualItems.ColumnCount) {
				BestFitColumn(startItem);
				actualWidth -= VisualItems.GetItemWidth(startItem, true);
				startItem++;
			}
			startItem = oldLeft - 1;
			while(actualWidth > 0 && startItem >= 0) {
				BestFitColumn(startItem);
				actualWidth -= VisualItems.GetItemWidth(startItem, true);
				startItem--;
			}
		}
		void BestFitShowedRows(int oldWidth, int oldTop, BestFitResult sizesCurLevel) {
			double actualHeight = Data.PivotGrid.PivotGridScroller.Cells.ActualHeight;
			foreach(PivotFieldValueItem item in sizesCurLevel.ValueItems)
				actualHeight -= VisualItems.GetItemHeight(item.MinLastLevelIndex, false);
			int startItem = oldWidth + oldTop;
			while(actualHeight > 0 && startItem < VisualItems.RowCount) {
				BestFitRow(startItem);
				actualHeight -= VisualItems.GetItemHeight(startItem, false);
				startItem++;
			}
			startItem = oldTop - 1;
			while(actualHeight > 0 && startItem >= 0) {
				BestFitRow(startItem);
				actualHeight -= VisualItems.GetItemHeight(startItem, false);
				startItem--;
			}
		}
		int CalcBestWidth(BestFitColumn column) {
			return (int)Data.BestWidthCalculator.CalcColumnBestFitWidth(column);
		}
		BestFitResult CalcBestWidths(IBestFitColumn column) {
			return Data.BestWidthCalculator.CalcColumnBestFitWidths(column);
		}
		List<int> CalcDataBestWidths(BestFitResult bestFitResult) {
			return Data.BestWidthCalculator.CalcDataBestFitWidths(bestFitResult);
		}
		int CalcBestHeight(BestFitColumn column) {
			return (int)Data.BestHeightCalculator.CalcColumnBestFitWidth(column);
		}
		BestFitResult CalcBestHeights(IBestFitColumn column) {
			return Data.BestHeightCalculator.CalcColumnBestFitWidths(column);
		}
		void SetColumnSizesCore(BestFitLevelFieldValues curColumn, BestFitResult bestFitResult) {
			if(curColumn == null) return;
			curColumn.UpdateMaxSizes(bestFitResult.NewSizes, CalcDataBestWidths(bestFitResult));
			if(curColumn.SetSizes(bestFitResult, false))
				UpdateScrollbars();
		}
		void UpdateScrollbars() {
			Data.PivotGrid.UpdateScrollbars();
		}
	}
}
