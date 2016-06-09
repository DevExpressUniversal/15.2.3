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
using System.Diagnostics;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class BestWidthCalculator : BestFitCalculatorBase {
		PivotGridWpfData data;
		BestFitCacheHelper helper;
		WeakReference cellsDecorator, columnValuesDecorator, rowValuesDecorator, rowAreaDecorator;
		public BestWidthCalculator(PivotGridWpfData data, BestFitCacheHelper helper) {
			this.data = data;
			this.helper = helper;
		}
		protected PivotGridWpfData Data { get { return data; } }
		protected PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		protected PivotGridControl PivotGrid { get { return Data.PivotGrid; } }
		public BestFitCacheHelper CacheHelper { get { return helper; } }
		protected override BestFitCalculatorBase.RowsBestFitCalculatorBase CreateBestFitCalculator(System.Collections.Generic.IEnumerable<int> rows) {
			return new RowsBestFitCalculator(this, rows, helper);
		}
		protected override FrameworkElement CreateBestFitControl(IBestFitColumn column) {
			return new CellElement() { Template = PivotGrid.PivotGridScroller.Cells.ItemTemplate };
		}
		protected override object[] GetUniqueValues(IBestFitColumn column) {
			return ((BestFitColumn)column).GetUniqueValues();
		}
		public virtual bool IsFitWidth { get { return true; } }
		public BestFitDecorator CellsDecorator {
			get { return cellsDecorator != null ? (BestFitDecorator)cellsDecorator.Target : null; }
			set { cellsDecorator = new WeakReference(value); }
		}
		public BestFitDecorator ColumnValuesDecorator {
			get { return columnValuesDecorator != null ? (BestFitDecorator)columnValuesDecorator.Target : null; }
			set { columnValuesDecorator = new WeakReference(value); }
		}
		public BestFitDecorator RowValuesDecorator {
			get { return rowValuesDecorator != null ? (BestFitDecorator)rowValuesDecorator.Target : null; }
			set { rowValuesDecorator = new WeakReference(value); }
		}
		public BestFitDecorator RowAreaDecorator {
			get { return rowAreaDecorator != null ? (BestFitDecorator)rowAreaDecorator.Target : null; }
			set { rowAreaDecorator = new WeakReference(value); }
		}
		protected override int GetRowCount(IBestFitColumn column) {
			return ((BestFitColumn)column).RowCount;
		}
		protected override int VisibleRowCount {
			get {
				throw new NotImplementedException("BestFitCalculator.VisibleRowCount");
			}
		}
		protected override BestFitCalculatorBase.CalcBestFitDelegate GetCalcBestFitDelegate(BestFitMode bestFitMode, IBestFitColumn column) {
			if(bestFitMode != BestFitMode.VisibleRows)
				return base.GetCalcBestFitDelegate(bestFitMode, column);
			return CreateBestFitCalculator(CalcBestFitVisibleRows(column)).CalcRowsBestFit;
		}
		private IEnumerable<int> CalcBestFitVisibleRows(IBestFitColumn column) {
			PivotGridScroller scroller = PivotGrid.PivotGridScroller;
			if(scroller == null)
				return new RowsRange(0, 0);
			if(((BestFitColumn)column).ResizingField.Area == FieldArea.ColumnArea || column is BestFitData)
				return new RowsRange(scroller.VerticalOffset, scroller.ViewportHeight);
			else
				return new RowsRange(scroller.HorizontalOffset, scroller.ViewportWidth);
		}
		protected override void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn icolumn, object cellValue) {
			BestFitColumn column = (BestFitColumn)icolumn;
			PivotGridField field = column.ResizingField;
			if(field.GetInternalField().IsRowTreeField) 
				field = Data.DataField;
			((ScrollableAreaCell)bestFitControl).ValueItem = column.GetCellsAreaItem((int)(cellValue));
		}
		protected override void SetBestFitElement(FrameworkElement bestFitElement) {
			CellsDecorator.Child = bestFitElement;
		}
		BestFitDecorator GetBestFitDecorator(BestFitColumnDecorator decorator) {
			return decorator == BestFitColumnDecorator.ColumnDecorator ? ColumnValuesDecorator : RowValuesDecorator;
		}
		protected void SetFieldValueBestFitElement(BestFitColumn column, FrameworkElement element) {
			GetBestFitDecorator(column.Decorator).Child = element;
		}
		protected void SetHeaderBestFitElement(FrameworkElement element) {
			RowAreaDecorator.Child = element;
		}
		public BestFitResult CalcColumnBestFitWidths(IBestFitColumn column) {
			try {
				return CalcColumnBestFitWidthsCore(column);
			} finally {
				SetBestFitElement(null);
			}
		}
		BestFitResult CalcColumnBestFitWidthsCore(IBestFitColumn column) {
			if(!ShouldCalcBestFitArea(column, FieldBestFitArea.FieldValue))
				return null;
			double result = 0.0;
			BestFitResult bestFitResult = new BestFitResult() { NewSizes = new List<int>() };
			CalcFieldValueBestFit((BestFitColumn)column, ref result, bestFitResult);
			return bestFitResult;
		}
		public List<int> CalcDataBestFitWidths(BestFitResult bestFitResult) {
			try {
				return CalcDataBestFitWidthsCore(bestFitResult);
			} finally {
				SetBestFitElement(null);
			}
		}
		List<int> CalcDataBestFitWidthsCore(BestFitResult bestFitResult) {
			if(VisualItems.ColumnCount <= 0)
				return null;
			List<int> widths = new List<int>();
			BestFitData dataColumn = new BestFitData(Data, bestFitResult);
			do {
				double width = 0.0;
				CalcDataBestFit(dataColumn, ref width);
				widths.Add((int)Math.Ceiling(IncWidth(width)));
			} while(dataColumn.MoveNext());
			return widths;
		}
		protected override double CalcColumnBestFitWidthCore(IBestFitColumn column) {
			double result = 0;
			BestFitColumn bestFitColumn = (BestFitColumn)column;
			if(ShouldCalcBestFitArea(bestFitColumn, FieldBestFitArea.FieldValue))
				CalcFieldValueBestFit(bestFitColumn, ref result, null);
			if(ShouldCalcBestFitArea(bestFitColumn, FieldBestFitArea.Cell))
				CalcDataBestFit(bestFitColumn, ref result);
			if(ShouldCalcBestFitArea(bestFitColumn, FieldBestFitArea.FieldHeader))
				CalcHeaderBestFit(bestFitColumn, ref result);
			return result != 0 ? IncWidth(result) : result;
		}
		void CalcFieldValueBestFit(BestFitColumn column, ref double result, BestFitResult bestFitResult) {
			try {
				ScrollableAreaCell cell = new FieldValueCellElement();
				ScrollableAreaPresenter.SetItemTemplate(cell, column.IsColumn ? PivotGrid.PivotGridScroller.ColumnValues.ItemTemplate : PivotGrid.PivotGridScroller.RowValues.ItemTemplate);
				SetFieldValueBestFitElement(column, cell);
				List<PivotFieldValueItem> valueItems = GetValueItems(column, GetBestFitMode(column));
				if(bestFitResult != null)
					bestFitResult.ValueItems = valueItems;
				int bestFitMaxRowCount = GetBestFitMaxRowCount(column);
				int rowCount = valueItems.Count;
				if(bestFitMaxRowCount != -1)
					rowCount = Math.Min(bestFitMaxRowCount, rowCount);
				Dictionary<PivotFieldValueItem, Size> itemSizeCache = helper.itemSizeCache;
				Dictionary<string, int> textSizes = null;
				bool needCalculateAllItems = bestFitResult != null && column.AppliedBestFitMode == BestFitMode.DistinctValues && column.IsColumn;
				if(needCalculateAllItems)
					textSizes = new Dictionary<string,int>();
				for(int i = 0; i < rowCount; i++) {
					double width = 0.0;
					Size size;
					PivotFieldValueItem fvItem = valueItems[i];
					if(itemSizeCache == null || !itemSizeCache.TryGetValue(fvItem, out size)) {
						FieldValueItem item = new FieldValueItem(fvItem, VisualItems);
						cell.ValueItem = item;
						PivotGrid.PivotGridScroller.UpdateCellBorders(cell, column.IsColumn);
						UpdateBestFitResult(cell, ref width);
						size = new Size(
							 Math.Min(10000, (column.IsRowTree ? Math.Max(0, width + VisualItems.GetRowTreeWidthItemDiff(item.Item)) : cell.DesiredSize.Width)),
							 Math.Min(10000, cell.DesiredSize.Height));
						if(itemSizeCache != null)
							itemSizeCache.Add(fvItem, size);
					}
					width = IsFitWidth ? size.Width : size.Height;
					result = Math.Max(width, result);
					if(bestFitResult != null) {
						int resultWidth = (int)Math.Ceiling(IncWidth(width));
						bestFitResult.NewSizes.Add(resultWidth);
						if(textSizes != null)
							textSizes.Add(fvItem.DisplayText, resultWidth);
					}
				}
				if(needCalculateAllItems) {
					List<int> NewSizes = bestFitResult.NewSizes;
					List<PivotFieldValueItem> ValueItems = bestFitResult.ValueItems;
					bestFitResult.ValueItems.Clear();
					bestFitResult.NewSizes.Clear();
					bestFitResult.ValueItems.AddRange(column.ValueItems);
					foreach(PivotFieldValueItem item in bestFitResult.ValueItems)
						bestFitResult.NewSizes.Add(textSizes[item.DisplayText]);
				}
			} finally {
				SetFieldValueBestFitElement(column, null);
			}
		}
		double IncWidth(double width) {
			return width + 1.0;
		}
		protected internal List<PivotFieldValueItem> GetValueItems(BestFitColumn column, BestFitMode mode) {
			column.AppliedBestFitMode = mode;
			List<PivotFieldValueItem> valueItems;
			switch(mode) {
				case BestFitMode.Default:
				case BestFitMode.Smart:
					if(column.ValueItems.Count < SmartModeRowCountThreshold)
						return GetValueItems(column, BestFitMode.AllRows);
					else
						return GetValueItems(column, BestFitMode.DistinctValues);
				case BestFitMode.AllRows:
					valueItems = column.ValueItems;
					return valueItems;
				case BestFitMode.DistinctValues:
					valueItems = new List<PivotFieldValueItem>();
					valueItems.AddRange(column.ValueItems);
					valueItems.Sort(ComparePivotFieldValueItemByDisplayText);
					for(int i = valueItems.Count - 1; i >= 1; i--) {
						if(valueItems[i - 1].DisplayText == valueItems[i].DisplayText)
							valueItems.Remove(valueItems[i]);
					}
					return valueItems;
				case BestFitMode.VisibleRows:
					if(column.ValueItems.Count == 1)
						return column.ValueItems;
					PivotGridScroller scroller = PivotGrid.PivotGridScroller;
					List<PivotFieldValueItem> columnValueItems = column.ValueItems;
					valueItems = new List<PivotFieldValueItem>();
					if(column.ResizingField.Area == FieldArea.RowArea) {
						for(int i = 0; i < columnValueItems.Count; i++) {
							PivotFieldValueItem item = columnValueItems[i];
							if(item.MinLastLevelIndex <= scroller.VerticalOffset + scroller.ViewportHeight
									&& item.MaxLastLevelIndex >= scroller.VerticalOffset) {
								valueItems.Add(item);
							}
						}
					} else {
						for(int i = 0; i < columnValueItems.Count; i++) {
							PivotFieldValueItem item = columnValueItems[i];
							if(item.MinLastLevelIndex <= scroller.HorizontalOffset + scroller.ViewportWidth
									&& item.MaxLastLevelIndex >= scroller.HorizontalOffset) {
								valueItems.Add(item);
							}
						}
					}
					return valueItems;
				default:
					throw new ArgumentException("BestFitMode");
			}
		}
		static int ComparePivotFieldValueItemByDisplayText(PivotFieldValueItem x, PivotFieldValueItem y) {
			return Comparer<string>.Default.Compare(x.DisplayText, y.DisplayText);
		}
		void CalcHeaderBestFit(BestFitColumn bestFitColumn, ref double result) {
			if(bestFitColumn.IsColumn && !PivotGrid.ShowColumnHeaders || !bestFitColumn.IsColumn && !PivotGrid.ShowRowHeaders)
				return;
			try {
				DragFieldHeader header = new DragFieldHeader();
				header.Bind(bestFitColumn.ResizingField, 0, bestFitColumn.ResizingField.Area.ToFieldListArea());
				SetHeaderBestFitElement(header);
				UpdateBestFitResult(header, ref result, -header.BestFitWCorrection);
			} finally {
				SetHeaderBestFitElement(null);
			}
		}
		protected FieldBestFitArea GetBestFitArea(IBestFitColumn column) {
			BestFitColumn bestFitColumn = (BestFitColumn)column;
			return bestFitColumn.BestFitArea == FieldBestFitArea.None ? PivotGrid.BestFitArea & ~bestFitColumn.DenyBestFitArea : bestFitColumn.BestFitArea;
		}
		protected override BestFitMode GetBestFitMode(IBestFitColumn column) {
			return column.BestFitMode == BestFitMode.Default ? PivotGrid.BestFitMode : column.BestFitMode;
		}
		protected override int GetBestFitMaxRowCount(IBestFitColumn column) {
			return column.BestFitMaxRowCount == -1 ? PivotGrid.BestFitMaxRowCount : column.BestFitMaxRowCount;
		}
		bool ShouldCalcBestFitArea(IBestFitColumn column, FieldBestFitArea testArea) {
			FieldBestFitArea bestFitArea = GetBestFitArea(column);
			return (bestFitArea & testArea) > 0;
		}
		public class BestFitCacheHelper {
			int bestFitCount = 0;
			internal Dictionary<int, Dictionary<int, Size>> cellSizeCache = null;
			internal Dictionary<PivotFieldValueItem, Size> itemSizeCache = null;
			public BestFitCacheHelper() {
			}
			public void BeginBestFit(bool fitWidth, bool fitHeight) {
				bestFitCount++;
				if(bestFitCount != 1)
					return;
				if(fitHeight && fitWidth) {
					cellSizeCache = new Dictionary<int, Dictionary<int, Size>>();
					itemSizeCache = new Dictionary<PivotFieldValueItem, Size>();
				}
			}
			public void EndBestFit() {
				bestFitCount--;
				if(bestFitCount != 0)
					return;
				cellSizeCache = null;
				itemSizeCache = null;
			}
			public double GetCellSize(FrameworkElement bestFitControl, BestWidthCalculator calculator, IBestFitColumn column, int rowHandle) {
				int columnIndex, rowIndex;
				((BestFitColumn)column).GetCellIndex(rowHandle, out columnIndex, out rowIndex);
				Dictionary<int, Size> dic = null;
				if(cellSizeCache != null && !cellSizeCache.TryGetValue(columnIndex, out dic)) {
					dic = new Dictionary<int, Size>();
					cellSizeCache[columnIndex] = dic;
				}
				Size desired;
				if(dic == null || !dic.TryGetValue(rowIndex, out desired)) {
					CellsAreaItem cellItem = new CellsAreaItem(calculator.VisualItems, columnIndex, rowIndex);
					ScrollableAreaCell cell = (ScrollableAreaCell)bestFitControl;
					cell.ValueItem = cellItem;
					if(calculator.PivotGrid != null)
						calculator.PivotGrid.PivotGridScroller.UpdateCellBorders(cell);
					DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.DoLockedAction(bestFitControl.UpdateLayout);
					desired = new Size(Math.Min(10000, bestFitControl.DesiredSize.Width), Math.Min(10000, bestFitControl.DesiredSize.Height));
					if(dic != null)
						dic[rowIndex] = desired;
				}
				return calculator.IsFitWidth ? desired.Width : desired.Height;
			}
		}
		#region public class RowsBestFitCalculator
		protected class RowsBestFitCalculator : BestFitCalculatorBase.RowsBestFitCalculatorBase {
			BestWidthCalculator calculator;
			BestFitCacheHelper helper;
			public RowsBestFitCalculator(BestWidthCalculator calculator, IEnumerable<int> rows, BestFitCacheHelper helper)
				: base(calculator, rows) {
				this.calculator = calculator;
				this.helper = helper;
			}
			protected PivotVisualItems VisualItems { get { return ((BestWidthCalculator)Owner).VisualItems; } }
			public override void CalcRowsBestFit(FrameworkElement bestFitControl, IBestFitColumn column, ref double result) {
				foreach(int rowHandle in Rows) {
					if(!IsValidRowHandle(rowHandle))
						continue;
					result = Math.Max(result, helper.GetCellSize(bestFitControl, calculator, column, rowHandle));
				}
			}
			protected override void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, int rowHandle) {
				int columnIndex, rowIndex;
				((BestFitColumn)column).GetCellIndex(rowHandle, out columnIndex, out rowIndex);
				CellsAreaItem cellItem = new CellsAreaItem(VisualItems, columnIndex, rowIndex);
				ScrollableAreaCell cell = (ScrollableAreaCell)bestFitControl;
				cell.ValueItem = cellItem;
				if(calculator.PivotGrid != null)
					calculator.PivotGrid.PivotGridScroller.UpdateCellBorders(cell);
			}
		}
		#endregion
		[Obsolete]
		public class FakeScrollabelItem : ScrollableAreaItemBase {
			PivotCellValue value;
			PivotGridField field;
			public FakeScrollabelItem(PivotCellValue value, PivotGridField field) {
				this.value = value;
				this.field = field;
			}
			#region IScrollableAreaItem Members
			public override int MinLevel {
				get { return 0; }
			}
			public override int MaxLevel {
				get { return 0; }
			}
			public override int MinIndex {
				get { return 0; }
			}
			public override int MaxIndex {
				get { return 0; }
			}
			public override object Value {
				get { return value.Value; }
			}
			public override string DisplayText {
				get { return value.DisplayText; }
			}
			#endregion
			public HorizontalAlignment HorizontalContentAlignment { get { return HorizontalAlignment.Left; } }
			public bool IsTotalAppearance { get { return false; } }
			public override PivotGridField Field { get { return field; } }
			protected override System.Windows.Media.Brush GetActualBackgroundBrush() { return null; }
			protected override System.Windows.Media.Brush GetActualForegroundBrush() { return null; }
			protected internal override void UpdateAppearance() { }
		}
	}
	public class BestHeightCalculator : BestWidthCalculator {
		public BestHeightCalculator(PivotGridWpfData data, BestFitCacheHelper helper)
			: base(data, helper) {
		}
		protected override double GetDesiredSize(FrameworkElement bestFitElement) {
			return bestFitElement.DesiredSize.Height;
		}
		public override bool IsFitWidth {
			get { return false; }
		}
	}
}
