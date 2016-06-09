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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Office.Internal;
using System.Linq;
using System.IO;
using DevExpress.Utils;
using DevExpress.Xpf.Charts;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
using StringFormat = System.Drawing.StringFormat;
using StringFormatFlags = System.Drawing.StringFormatFlags;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region CellLayout
	class CellLayout {
		public CellLayout(int index, Rect rect) {
			Index = index;
			ArrangeRect = rect;
		}
		public int Index { get; private set; }
		public Rect ArrangeRect { get; set; }
	}
	#endregion
	#region WorksheetPaintPanel
	public class WorksheetPaintPanel : Panel {
		#region Fields
		WorksheetControl owner;
		SpreadsheetControl spreadsheetControl;
		#endregion
		public WorksheetPaintPanel() {
			Loaded += OnLoaded;
		}
		#region Properties
		public WorksheetControl Owner { get { return owner; } }
		public SpreadsheetControl SpreadsheetControl { get { return spreadsheetControl; } }
		public DocumentLayout LayoutInfo { get { return Owner != null ? Owner.LayoutInfo : null; } }
		protected internal SpreadsheetPropertiesProvider SpreadsheetProvider { get { return GetValue(SpreadsheetViewControl.SpreadsheetProviderProperty) as SpreadsheetPropertiesProvider; } }
		#endregion
		void OnLoaded(object sender, RoutedEventArgs e) {
			owner = LayoutHelper.FindParentObject<WorksheetControl>(this);
			spreadsheetControl = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
			InvalidateMeasure();
		}
	}
	#endregion
	#region WorksheetPanel
	public class WorksheetPanel : WorksheetPaintPanel {
		#region Fields
		const int maxColumn = 1000000;
		bool isMeasureInProcess;
		List<int> freeIndexes;
		Dictionary<int, CellLayout> cachedCells;
		ICellErrorTextProvider errorTextProvider;
		#endregion
		public WorksheetPanel() {
			this.cachedCells = new Dictionary<int, CellLayout>();
			this.freeIndexes = new List<int>();
			this.errorTextProvider = new DefaultCellErrorTextProvider();
		}
		#region Properties
		internal bool IsMeasureInProcess { get { return isMeasureInProcess; } }
		Brush Foreground { get { return Owner.Foreground; } }
		Dictionary<int, StringFormat> StringFormats { get { return StringFormatProvider.StringFormats; } }
		#endregion
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (int key in cachedCells.Keys) {
				CellLayout cell = cachedCells[key];
				CellPresenter cellContainer = Children[cell.Index] as CellPresenter;
				cellContainer.Arrange(cell.ArrangeRect);
			}
			HideFreeCells();
			return finalSize;
		}
		void HideFreeCells() {
			List<int> free = cachedCells.Values.Select(c => c.Index).ToList();
			for (int i = 0; i < Children.Count; i++)
				if (!free.Contains(i))
					Children[i].Arrange(new Rect(0, 0, 0, 0));
		}
		protected override Size MeasureOverride(Size finalSize) {
			try {
				isMeasureInProcess = true;
				finalSize = base.MeasureOverride(finalSize);
				if (LayoutInfo == null || Owner == null)
					return finalSize;
				var pages = LayoutInfo.Pages;
				if (pages == null || pages.Count == 0)
					return finalSize;
				ClearDiagonalBorders();
				UpdateFreeIndexes(LayoutInfo);
				List<int> visibleCells = new List<int>();
				for (int i = 0; i < pages.Count; i++) {
					LayoutPage page = pages[i];
					MeasurePage(page, visibleCells, i);
				}
				ValidateCache(visibleCells);
				return finalSize;
			}
			finally {
				isMeasureInProcess = false;
			}
		}
		void UpdateFreeIndexes(DocumentLayout layout) {
			CellPosition topLeft = layout.DocumentModel.ActiveSheet.ActiveView.TopLeftCell;
			CellPosition bottomRight = layout.VisibleRange.BottomRight;
			List<int> keys = new List<int>(cachedCells.Keys);
			foreach (int key in keys) {
				int colIndex = key % maxColumn;
				int rowIndex = key / maxColumn;
				if ((colIndex < topLeft.Column || colIndex > bottomRight.Column) ||
					(rowIndex < topLeft.Row || rowIndex > bottomRight.Row)) {
					freeIndexes.Add(cachedCells[key].Index);
					cachedCells.Remove(key);
				}
			}
		}
		void MeasurePage(LayoutPage page, List<int> visibleCells, int pageIndex) {
			for (int i = 0; i < page.Boxes.Count; i++) {
				MeasureCellTextBox(page, page.Boxes[i], visibleCells, pageIndex);
			}
			for (int i = 0; i < page.ComplexBoxes.Count; i++) {
				MeasureCellTextBox(page, page.ComplexBoxes[i], visibleCells, pageIndex);
			}
		}
		void MeasureCellTextBox(LayoutPage page, CellTextBoxBase box, List<int> visibleCells, int pageIndex) {
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			if (IsInplaceEditorOpenInThisCell(cell.Position))
				return;
			XlBorderLineStyle downLineStyle = cell.ActualBorder.DiagonalDownLineStyle;
			XlBorderLineStyle upLineStyle = cell.ActualBorder.DiagonalUpLineStyle;
			bool shouldMeasureDiagonalBorders = ShouldMeasureDiagonalBorders(downLineStyle, upLineStyle);
			bool shouldMeasureCell = ShouldMeasureCell(cell);
			if (!shouldMeasureDiagonalBorders && !shouldMeasureCell)
				return;
			if (shouldMeasureDiagonalBorders) {
				Rect bounds = box.GetFillBounds(page).ToRect();
				Color borderColor = CellTextBoxBase.GetDiagonalBorderColor(cell.ActualBorder, page.DocumentLayout.DocumentModel).ToWpfColor();
				MeasureDiagonalBorders(bounds, downLineStyle, upLineStyle, borderColor);
			}
			if (shouldMeasureCell)
				MeasureCell(page, box, cell, visibleCells, pageIndex);
		}
		bool IsInplaceEditorOpenInThisCell(CellPosition position) {
			return !Owner.ValidateInplaceEditorPosition(position);
		}
		bool ShouldMeasureCell(ICell cell) {
			return (cell.HasFormula && cell.Worksheet.ActiveView.ShowFormulas) || !String.IsNullOrEmpty(cell.Text);
		}
		void ClearDiagonalBorders() {
			Owner.DiagonalBorderControl.Clear();
		}
		bool ShouldMeasureDiagonalBorders(XlBorderLineStyle downLineStyle, XlBorderLineStyle upLineStyle) {
			return Owner.DiagonalBorderControl.ShouldAddBorder(downLineStyle, upLineStyle);
		}
		void MeasureDiagonalBorders(Rect bounds, XlBorderLineStyle downLineStyle, XlBorderLineStyle upLineStyle, Color color) {
			WorksheetDiagonalBorderControl borderControl = Owner.DiagonalBorderControl;
			borderControl.AddBorderInfo(bounds, downLineStyle, upLineStyle, color);
		}
		void MeasureCell(LayoutPage page, CellTextBoxBase box, ICell cell, List<int> visibleCells, int pageIndex) {
			Rect rect = box.GetBounds(page).ToRect();
			Rect clipRect = box.GetClipBounds(page).ToRect();
			rect.Intersect(clipRect);
			int cellIndex = CalcCellIndex(cell.ColumnIndex, cell.RowIndex, pageIndex);
			if (!cachedCells.Keys.Contains(cellIndex))
				CacheCell(page, box, cell, rect, clipRect, cellIndex);
			else {
				CellLayout cellLayout = cachedCells[cellIndex];
				cellLayout.ArrangeRect = rect;
				CellPresenter cellContainer = Children[cellLayout.Index] as CellPresenter;
				CellData cellData = cellContainer.Content as CellData;
				if (cellData != null)
					cellData.Clip = GetClipGeometry(page.Bounds.ToRect(), rect);
			}
			visibleCells.Add(cellIndex);
		}
		int CalcCellIndex(int colIndex, int rowIndex, int pageIndex) {
			return rowIndex * maxColumn + colIndex + pageIndex;
		}
		void CacheCell(LayoutPage page, CellTextBoxBase box, ICell cell, Rect cellBounds, Rect clipBounds, int cacheIndex) {
			CellForegroundDisplayFormat displayFormat = box.CalculateForegroundDisplayFormat(page, errorTextProvider, false, false);
			string text = displayFormat.Text;
			if (String.IsNullOrEmpty(text))
				return;
			TextSettings textInfo = CalculateTextInfo(displayFormat, cellBounds, clipBounds);
			if (textInfo == null)
				return;
			CellData data = new CellData(Owner, cell.Position, textInfo, GetClipGeometry(page.Bounds.ToRect(), cellBounds));
			DataTemplate cellTemplate = Owner.CellTemplateSelector != null ? Owner.CellTemplateSelector.SelectTemplate(data, SpreadsheetControl) : null;
			int childIndex = 0;
			CellPresenter cellContainer = GetCellContainer(cacheIndex, ref childIndex);
			cellContainer.Content = data;
			if (cellTemplate != null)
				cellContainer.ContentTemplate = cellTemplate;
			else {
				IConditionalFormattingPainters condFmts = cell.Worksheet.ConditionalFormattings.GetPainters(cell, cell.ConditionalFormattingStoppedAtPriority);
				ConditionalFormattingSettings cfSettings = new ConditionalFormattingSettings();
				DataTemplate cfTemplate = Owner.GetConditionalFormattingTemplate(cell, condFmts, displayFormat.Bounds, ref cfSettings);
				cellContainer.ContentTemplate = cfTemplate == null ? Owner.CellTemplate : cfTemplate;
				data.ConditionalFormattingSettings = cfSettings;
				if (!condFmts.ShowValues)
					data.TextSettings.Text = String.Empty;
			}
			cellContainer.ShowCellToolTip = CanShowToolTip(displayFormat.Bounds.Width, cellBounds.Width, text, cell.Value.Type);
			cellContainer.Measure(cellBounds.Size);
			cachedCells.Add(cacheIndex, new CellLayout(childIndex, cellBounds));
		}
		TextSettings CalculateTextInfo(CellForegroundDisplayFormat displayFormat, Rect cellRect, Rect clipRect) {
			ICell cell = displayFormat.Cell;
			StringFormat stringFormat = GetStringFormat(cell);
			if (stringFormat == null)
				return null;
			TextSettings result = TextInfoCalculator.PrepareTextInfo(displayFormat, stringFormat, Foreground);
			Rect textRect = displayFormat.Bounds.ToRect();
			textRect.Intersect(clipRect);
			double xDelta = Math.Abs(textRect.X - cellRect.X);
			result.TextBounds = new Rect(xDelta, textRect.Y, textRect.Width, textRect.Height);
			return result;
		}
		StringFormat GetStringFormat(ICell cell) {
			int stringFormatKey = StringFormatProvider.CalculateAlignmentHash(cell.ActualHorizontalAlignment, cell.ActualAlignment.Vertical, cell.ActualAlignment.WrapText);
			StringFormat result;
			if (StringFormats.TryGetValue(stringFormatKey, out result))
				return result;
			return null;
		}
		RectangleGeometry GetClipGeometry(Rect pageRect, Rect cellRect) {
			Rect clipRect = Rect.Intersect(pageRect, cellRect);
			if (clipRect == Rect.Empty) 
				return new RectangleGeometry(Rect.Empty);
			double x = clipRect.X - cellRect.X;
			double y = clipRect.Y - cellRect.Y;
			Rect result = new Rect(x, y, clipRect.Width, clipRect.Height); 
			return new RectangleGeometry(result);
		}
		CellPresenter GetCellContainer(int cellIndex, ref int index) {
			if (cachedCells.Keys.Contains(cellIndex)) {
				index = cachedCells[cellIndex].Index;
				return Children[index] as CellPresenter;
			}
			if (freeIndexes.Count != 0) {
				index = freeIndexes[0];
				freeIndexes.RemoveAt(0);
			}
			else {
				CellPresenter control = new CellPresenter() { FlowDirection = System.Windows.FlowDirection.LeftToRight };
				Children.Add(control);
				index = Children.Count - 1;
			}
			CellPresenter presenter = Children[index] as CellPresenter;
			presenter.CellToolTip = Owner.GetCellToolTip();
			return presenter;
		}
		bool CanShowToolTip(double textWidth, double cellWidth, string text, VariantValueType valueType) {
			if (SpreadsheetProvider.ShowCellToolTipMode == ShowCellToolTipMode.Auto)
				return textWidth >= cellWidth || IsPlaceHolder(text, valueType);
			return SpreadsheetProvider.ShowCellToolTipMode == ShowCellToolTipMode.Always;
		}
		bool IsPlaceHolder(string text, VariantValueType valueType) {
			if (String.IsNullOrEmpty(text))
				return false;
			if (valueType != VariantValueType.Numeric && valueType != VariantValueType.Boolean)
				return false;
			return text[0] == '#';
		}
		void ValidateCache(List<int> visibleCells) {
			List<int> free = cachedCells.Keys.Except(visibleCells).ToList();
			foreach (int key in free) {
				int index = cachedCells[key].Index;
				CellPresenter presenter = Children[index] as CellPresenter;
				presenter.Arrange(new Rect(0, 0, 0, 0));
				InvalidateCellPresenter(presenter);
				freeIndexes.Add(index);
				cachedCells.Remove(key);
			}
		}
		internal void ClearCache(bool deleteChildren = false) {
			cachedCells.Clear();
			if (deleteChildren) {
				freeIndexes.Clear();
				Children.Clear();
			}
			else {
				freeIndexes = new List<int>();
				for (int i = 0; i < Children.Count; i++) {
					freeIndexes.Add(i);
					CellPresenter presenter = Children[i] as CellPresenter;
					InvalidateCellPresenter(presenter);
				}
			}
		}
		void InvalidateCellPresenter(CellPresenter presenter) {
			presenter.Content = null;
			presenter.ContentTemplate = null;
		}
	}
	#region StringFormatProvider
	static class StringFormatProvider {
		public static Dictionary<int, StringFormat> StringFormats { get { return stringFormats; } }
		static Dictionary<int, StringFormat> stringFormats = PopulateStringFormats();
		static Dictionary<int, StringFormat> PopulateStringFormats() {
			Dictionary<int, StringFormat> resultStringFormats = new Dictionary<int, StringFormat>();
			for (int wrapText = 0; wrapText <= 1; wrapText++) {
				for (XlHorizontalAlignment i = XlHorizontalAlignment.General; i <= XlHorizontalAlignment.Distributed; i++) {
					for (XlVerticalAlignment j = XlVerticalAlignment.Top; j <= XlVerticalAlignment.Distributed; j++) {
						int hash = CalculateAlignmentHash(i, j, wrapText != 0);
						StringFormat stringFormat = CreateStringFormat(i, j, wrapText != 0);
						resultStringFormats.Add(hash, stringFormat);
					}
				}
			}
			return resultStringFormats;
		}
		static StringFormat CreateStringFormat(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip;
			result.FormatFlags &= ~StringFormatFlags.LineLimit;
			result.Alignment = ConvertHorizontalAlignment(horizontalAlignment);
			result.LineAlignment = ConvertVerticalAlignment(verticalAlignment);
			if (!wrapText)
				result.FormatFlags |= StringFormatFlags.NoWrap;
			return result;
		}
		public static int CalculateAlignmentHash(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			return ((int)horizontalAlignment) << 3 | (int)verticalAlignment | ((wrapText ? 0 : 1) << 6);
		}
		static System.Drawing.StringAlignment ConvertHorizontalAlignment(XlHorizontalAlignment horizontalAlignment) {
			switch (horizontalAlignment) {
				default:
				case XlHorizontalAlignment.General:
				case XlHorizontalAlignment.Fill:
				case XlHorizontalAlignment.Distributed:
				case XlHorizontalAlignment.Justify:
				case XlHorizontalAlignment.Left:
					return System.Drawing.StringAlignment.Near;
				case XlHorizontalAlignment.CenterContinuous:
				case XlHorizontalAlignment.Center:
					return System.Drawing.StringAlignment.Center;
				case XlHorizontalAlignment.Right:
					return System.Drawing.StringAlignment.Far;
			}
		}
		static System.Drawing.StringAlignment ConvertVerticalAlignment(XlVerticalAlignment verticalAlignment) {
			switch (verticalAlignment) {
				default:
				case XlVerticalAlignment.Justify:
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Bottom:
					return System.Drawing.StringAlignment.Far;
				case XlVerticalAlignment.Center:
					return System.Drawing.StringAlignment.Center;
				case XlVerticalAlignment.Top:
					return System.Drawing.StringAlignment.Near;
			}
		}
	}
	#endregion
	#region FloatingObjectsContainer
	public class FloatingObjectsContainer : Panel, IDrawingBoxVisitor {
		public static readonly DependencyProperty LayoutInfoProperty;
		static FloatingObjectsContainer() {
			LayoutInfoProperty = DependencyProperty.Register("LayoutInfo", typeof(DocumentLayout), typeof(FloatingObjectsContainer));
		}
		public FloatingObjectsContainer() {
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
			this.Loaded += WorksheetPanelLoaded;
		}
		public DocumentLayout LayoutInfo {
			get { return (DocumentLayout)GetValue(LayoutInfoProperty); }
			set { SetValue(LayoutInfoProperty, value); }
		}
		WorksheetControl Owner { get; set; }
		double ScaleFactor { get { return Owner != null ? Owner.ScaleFactor : 1.0; } }
		void WorksheetPanelLoaded(object sender, RoutedEventArgs e) {
			Owner = LayoutHelper.FindParentObject<WorksheetControl>(this);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (LayoutInfo == null) return finalSize;
			var pages = LayoutInfo.Pages;
			if (pages == null || pages.Count == 0) return finalSize;
			List<int> visibleDrawingObjects = new List<int>();
			foreach (DevExpress.XtraSpreadsheet.Layout.Page page in pages) {
				visibleDrawingObjects.AddRange(ArrangeObjects(page));
			}
			HideDrawingObjects(visibleDrawingObjects);
			return finalSize;
		}
		private void HideDrawingObjects(List<int> visibleDrawingObjects) {
			HideUnVisibleDrawingObjects(visibleDrawingObjects, images);
			HideUnVisibleDrawingObjects(visibleDrawingObjects, charts);
			HideFreeObjectContainer();
		}
		private void HideFreeObjectContainer() {
			foreach (int key in images.Keys) {
				if (!measuredItems.Contains(key)) images[key].Arrange(new Rect(0, 0, 0, 0));
			}
			foreach (int key in charts.Keys) {
				if (!measuredItems.Contains(key)) charts[key].Arrange(new Rect(0, 0, 0, 0));
			}
		}
		private List<int> ArrangeObjects(XtraSpreadsheet.Layout.Page page) {
			List<int> visibleDrawingObjects = new List<int>();
			foreach (DrawingBox drawingBox in page.DrawingBoxes) {
				visibleDrawingObjects.Add(drawingBox.DrawingIndex);
				drawingBox.Visit(this);
			}
			return visibleDrawingObjects;
		}
		private void HideUnVisibleDrawingObjects<TValue>(List<int> visibleDrawingObjects, IDictionary<int, TValue> drawingObjects) {
			foreach (int key in drawingObjects.Keys) {
				if (!visibleDrawingObjects.Contains(key)) (drawingObjects[key] as UIElement).Arrange(new Rect(0, 0, 0, 0));
			}
		}
		List<int> measuredItems = new List<int>();
		Dictionary<int, Image> images = new Dictionary<int, Image>();
		Dictionary<int, ContentPresenter> charts = new Dictionary<int, ContentPresenter>();
#if DEBUGTEST
		internal Dictionary<int, Image> Images { get { return images; } }
		internal Dictionary<int, ContentPresenter> Charts { get { return charts; } }
#endif
		private void ArrangeDrawingBox(DrawingBox box) {
			Rect bounds = box.Bounds.ToRect();
			int objectIndex = box.DrawingIndex;
			UIElement container = GetContainer(box);
			if (container == null) return;
			if (!measuredItems.Contains(objectIndex)) {
				MeasureContainer(box, container);
				measuredItems.Add(box.DrawingIndex);
			}
			float angleInDegrees = LayoutInfo.DocumentModel.GetPictureRotationAngleInDegrees(objectIndex);
			ApplyRotateTransform(container, angleInDegrees, CalcCenterPoint(bounds));
			container.Arrange(bounds);
		}
		private void MeasureContainer(DrawingBox box, UIElement container) {
			if (box is PictureBox)
				MeasurePictureBox(box as PictureBox, container);
			if (box is ChartBox)
				MeasureChartBox(box as ChartBox, container);
		}
		private void MeasureChartBox(ChartBox chartBox, UIElement container) {
			Chart chart = chartBox.Chart;
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null)
				return;
			Rect bounds = chartBox.Bounds.ToRect();
			ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, bounds.Width, bounds.Height);
			chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, container));
			container.SetValue(Panel.ZIndexProperty, chart.ZOrder);
			container.Measure(bounds.Size);
		}
		private void MeasurePictureBox(PictureBox box, UIElement container) {
			Rect bounds = box.Bounds.ToRect();
			System.Drawing.Image image = GetImage(box);
			double scaleFactor = ScaleFactor;
			Size targetSize = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
			((Image)container).Source = image.ToImageSource(targetSize);
			container.SetValue(Panel.ZIndexProperty, box.Picture.ZOrder);
			container.Measure(bounds.Size);
		}
		private UIElement GetContainer(DrawingBox box) {
			if (box is PictureBox)
				return GetImageContainer(box.DrawingIndex);
			if (box is ChartBox)
				return GetChartContainer(box.DrawingIndex);
			return null;
		}
		private UIElement GetContainer(ChartBox box) {
			return GetChartContainer(box.DrawingIndex);
		}
		private Point CalcCenterPoint(Rect bounds) {
			double x = bounds.Width / 2;
			double y = bounds.Height / 2;
			return new Point(x, y);
		}
		private void ApplyRotateTransform(UIElement child, float angleInDegrees, Point center) {
			RotateTransform rotate = new RotateTransform(angleInDegrees) { CenterX = center.X, CenterY = center.Y };
			child.RenderTransform = rotate;
		}
		private Image GetImageContainer(int index) {
			if (images.ContainsKey(index)) return images[index];
			else {
				Image im = new Image();
				im.Stretch = Stretch.Fill;
				Children.Add(im);
				images.Add(index, im);
				return im;
			}
		}
		private ContentPresenter GetChartContainer(int index) {
			if (charts.ContainsKey(index)) return charts[index];
			else {
				ContentPresenter chartContainer = new ContentPresenter();
				Children.Add(chartContainer);
				charts.Add(index, chartContainer);
				return chartContainer;
			}
		}
		System.Drawing.Image GetImage(PictureBox box) {
#if !SL
			return box.NativeImage; ;
#else
			byte[] brickImageBytes = DevExpress.Data.Printing.Native.DxDibImageConverter.Encode(box.NativeImage, false);
			return Image.FromStream(new System.IO.MemoryStream(brickImageBytes));
#endif
		}
		internal void ClearCache(bool deleteChildren) {
			if (deleteChildren) {
				images.Clear();
				charts.Clear();
				Children.Clear();
			}
			if (measuredItems != null) measuredItems.Clear();
		}
		#region IDrawingBoxVisitor Members
		void IDrawingBoxVisitor.Visit(ChartBox box) {
			ArrangeDrawingBox(box);
		}
		void IDrawingBoxVisitor.Visit(PictureBox box) {
			ArrangeDrawingBox(box);
		}
		void IDrawingBoxVisitor.Visit(ShapeBox box) {
		}
		#endregion
	}
	#endregion
	#endregion
}
