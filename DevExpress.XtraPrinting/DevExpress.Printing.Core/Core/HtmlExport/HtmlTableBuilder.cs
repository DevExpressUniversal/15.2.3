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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Export.Web {
	public abstract class HtmlBuilderBase : IHtmlExportProvider {
		#region  static
		protected static bool NeedClipping(BrickViewData data) {
			return data.OriginalBounds != data.Bounds;
		}
		#endregion
		#region inner classes
		protected class Table : ArrayList {
			public Table(int rowCount) {
				for(int i = 0; i < rowCount; i++) {
					Add(new ArrayList());
				}
			}
			public new ArrayList this[int rowIndex] {
				get { return (ArrayList)base[rowIndex]; }
			}
			public int RowCount {
				get { return Count; }
			}
		}
		public class HtmlCellLayout {
			public PaddingInfo Borders;
			public PaddingInfo Padding;
			public HtmlCellLayout() {
			}
			public HtmlCellLayout(BrickStyle style) {
				if(style != null) {
					Borders.Left = GetBorderWidth(style, BorderSide.Left);
					Borders.Right = GetBorderWidth(style, BorderSide.Right);
					Borders.Top = GetBorderWidth(style, BorderSide.Top);
					Borders.Bottom = GetBorderWidth(style, BorderSide.Bottom);
				}
			}
			static int GetBorderWidth(BrickStyle style, BorderSide side) {
				return (int)((style.Sides & side) != 0 ? style.BorderWidth : 0);
			}
		}
		abstract class LayoutCalculator {
			HtmlCellLayout calcLayout;
			public LayoutCalculator(HtmlCellLayout calcLayout) {
				this.calcLayout = calcLayout;
			}
			public void Calculate(int boundsNear, int boundsFar, int baseBoundsNear, int baseBoundsFar, HtmlCellLayout baselayout) {
				int nearBorder = baseBoundsNear + GetNearValue(baselayout.Borders);
				int farBorder = baseBoundsFar - GetFarValue(baselayout.Borders);
				int nearPadding = nearBorder + GetNearValue(baselayout.Padding);
				int farPadding = farBorder - GetFarValue(baselayout.Padding);
				int bordersNear = CalculateElement(baseBoundsNear, nearBorder, boundsNear, boundsFar);
				int bordersFar = CalculateElement(farBorder, baseBoundsFar, boundsNear, boundsFar);
				SetBorders(calcLayout, bordersNear, bordersFar);
				int paddingNear = CalculateElement(nearBorder, nearPadding, boundsNear, boundsFar);
				int paddingFar = CalculateElement(farPadding, farBorder, boundsNear, boundsFar);
				SetPadding(calcLayout, paddingNear, paddingFar);
			}
			protected abstract int GetNearValue(PaddingInfo padding);
			protected abstract int GetFarValue(PaddingInfo padding);
			protected abstract void SetPadding(HtmlCellLayout layout, int nearValue, int farValue);
			protected abstract void SetBorders(HtmlCellLayout layout, int nearValue, int farValue);
		}
		class LayoutCalculatorHoriz : LayoutCalculator {
			public LayoutCalculatorHoriz(HtmlCellLayout areaLayout) : base(areaLayout) {
			}
			protected override int GetNearValue(PaddingInfo padding) {
				return padding.Left;
			}
			protected override int GetFarValue(PaddingInfo padding) {
				return padding.Right;
			}
			protected override void SetPadding(HtmlCellLayout layout, int nearValue, int farValue) {
				layout.Padding.Left = nearValue;
				layout.Padding.Right = farValue;
			}
			protected override void SetBorders(HtmlCellLayout layout, int nearValue, int farValue) {
				layout.Borders.Left = nearValue;
				layout.Borders.Right = farValue;
			}
		}
		class LayoutCalculatorVert : LayoutCalculator {
			public LayoutCalculatorVert(HtmlCellLayout areaLayout) : base(areaLayout) {
			}
			protected override int GetNearValue(PaddingInfo padding) {
				return padding.Top;
			}
			protected override int GetFarValue(PaddingInfo padding) {
				return padding.Bottom;
			}
			protected override void SetPadding(HtmlCellLayout layout, int nearValue, int farValue) {
				layout.Padding.Top = nearValue;
				layout.Padding.Bottom = farValue;
			}
			protected override void SetBorders(HtmlCellLayout layout, int nearValue, int farValue) {
				layout.Borders.Top = nearValue;
				layout.Borders.Bottom = farValue;
			}
		}
		#endregion
		protected string emptyCellContent;
		protected HtmlExportContext fHtmlExportContext;
		protected BrickViewData fCurrentData;
		protected DXHtmlContainerControl fCurrentCell;
		protected Rectangle fCurrentCellBounds;
		INavigationService navigationService;
		INavigationService NavigationService {
			get {
				if(navigationService == null) {
					navigationService = fHtmlExportContext.PrintingSystem.GetService<INavigationService>();
					if(navigationService == null) 
						navigationService = new WebNavigationService();
				}
				return navigationService;
			}
		}
		ExportContext ITableExportProvider.ExportContext { get { return fHtmlExportContext; } }
		HtmlExportContext IHtmlExportProvider.HtmlExportContext { get { return fHtmlExportContext; } }
		DXHtmlContainerControl IHtmlExportProvider.CurrentCell { get { return fCurrentCell; } }
		Rectangle IHtmlExportProvider.CurrentCellBounds { get { return fCurrentCellBounds; } }
		BrickViewData ITableExportProvider.CurrentData { get { return fCurrentData; } }
		#region ITableExportProvider
		void ITableExportProvider.SetCellShape(Color lineColor, XtraReports.UI.LineDirection lineDirection, System.Drawing.Drawing2D.DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink) {
		}
		void ITableExportProvider.SetCellImage(Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imgSize, PaddingInfo padding, string hyperLink) {
			fHtmlExportContext.HtmlCellImageContentCreator.CreateContent(fCurrentCell, image, imageSrc, sizeMode, align, bounds, imgSize, padding);
		}
		void ITableExportProvider.SetCellText(object text, string hyperLink) {
			string s = text as string;
			if(!string.IsNullOrEmpty(s)) {
				HtmlCellTextContentCreator creator = new HtmlCellTextContentCreator(fCurrentCell);
				creator.CreateContent(s, fCurrentData.Style, fCurrentData.OriginalBoundsF, fHtmlExportContext.Measurer);
			}
		}
		#endregion
		protected virtual void SetNavigationUrlCore(VisualBrick brick) {
			string script = NavigationService.GetMouseDownScript(fHtmlExportContext, brick);
			if(!string.IsNullOrEmpty(script)) {
				fHtmlExportContext.RegisterNavigationScript();
				fCurrentCell.Style.Add("cursor", "pointer");
				fCurrentCell.Attributes.Add("onmousedown", script);
			}
		}
		#region IHtmlExportProvider
		void IHtmlExportProvider.SetNavigationUrl(VisualBrick brick) {
			SetNavigationUrlCore(brick);
		}
		void IHtmlExportProvider.SetAnchor(string anchorName) {
			VisualBrickExporter.SetHtmlAnchor(fCurrentCell, anchorName, fHtmlExportContext);
		}
		void IHtmlExportProvider.RaiseHtmlItemCreated(VisualBrick brick) {
			brick.BrickOwner.RaiseHtmlItemCreated(brick, fHtmlExportContext.ScriptContainer, fCurrentCell);
		}
		#endregion
		protected DXHtmlLiteralControl CreateEmptyCellControl() {
			return new DXHtmlLiteralControl(emptyCellContent);
		}
		protected static int CalculateElement(int x0, int x1, int nearValue, int farValue) {
			return Math.Max(Math.Min(x1, farValue) - Math.Max(x0, nearValue), 0);
		}
		protected static HtmlCellLayout CalculateCellLayout(HtmlCellLayout baselayout, RectangleF bounds, RectangleF originalBounds) {
			HtmlCellLayout areaLayout = new HtmlCellLayout();
			new LayoutCalculatorHoriz(areaLayout).Calculate((int)bounds.Left, (int)bounds.Right, (int)originalBounds.Left, (int)originalBounds.Right, baselayout);
			new LayoutCalculatorVert(areaLayout).Calculate((int)bounds.Top, (int)bounds.Bottom, (int)originalBounds.Top, (int)originalBounds.Bottom, baselayout);
			return areaLayout;
		}
		protected Rectangle CalculateCellRect(Rectangle bounds, HtmlCellLayout areaLayout) {
			Rectangle result = bounds;
			if(areaLayout != null) {
				result = RectHelper.DeflateRect(result, areaLayout.Padding.Left, areaLayout.Padding.Top, areaLayout.Padding.Right, areaLayout.Padding.Bottom);
				result = RectHelper.DeflateRect(result, areaLayout.Borders.Left, areaLayout.Borders.Top, areaLayout.Borders.Right, areaLayout.Borders.Bottom);
			}
			return result;
		}
		protected string RegisterHtmlClassName(BrickStyle style, PaddingInfo borders, PaddingInfo padding) {
			if(style == null)
				return String.Empty;
			string htmlStyle = PSHtmlStyleRender.GetHtmlStyle(style.Font, style.ForeColor, style.BackColor, style.BorderColor, borders, padding, style.BorderDashStyle);
			return fHtmlExportContext.ScriptContainer.RegisterCssClass(htmlStyle);
		}
		protected void ClippingControl(BrickViewData data, Rectangle fCurrentCellBounds, DXHtmlContainerControl control, HtmlCellLayout baselayout) {
			Rectangle originalBounds = CalculateCellRect(data.OriginalBounds, baselayout);
			Point offset = new Point(originalBounds.X - fCurrentCellBounds.X, originalBounds.Y - fCurrentCellBounds.Y);
			ClipControl clipControl = HtmlHelper.SetClip(control, offset, fCurrentCellBounds.Size, originalBounds.Size);
			if(data.Style != null && fHtmlExportContext.CopyStyleWhenClipping) {
				BrickStyle contentStyle = (BrickStyle)data.Style.Clone();
				contentStyle.BorderWidth = 0;
				contentStyle.Sides = BorderSide.None;
				contentStyle.Padding = PaddingInfo.Empty;
				clipControl.InnerControlCSSClass = RegisterHtmlClassName(contentStyle, PSHtmlStyleRender.GetBorders(contentStyle), PaddingInfo.Empty);
			}
		}
		protected virtual void FillCellStyle(BrickViewData data, DXHtmlContainerControl control, HtmlCellLayout areaLayout) {
			if(data.Style != null) {
				control.Attributes["class"] = RegisterHtmlClassName(data.Style, areaLayout.Borders, areaLayout.Padding);
			}
		}
		protected void FillCellContent(BrickViewData data, DXHtmlContainerControl control) {
			ITableCell tableCell = data.TableCell;
			HtmlCellLayout baselayout = new HtmlCellLayout(data.Style);
			baselayout.Padding = tableCell.ShouldApplyPadding ? AdjustPadding(data.Style.Padding, data.Style.TextAlignment) : PaddingInfo.Empty;
			HtmlCellLayout areaLayout = NeedClipping(data) ?
				CalculateCellLayout(baselayout, data.Bounds, data.OriginalBounds) :
				baselayout;
			FillCellStyle(data, control, areaLayout);
			fCurrentCellBounds = CalculateCellRect(data.Bounds, areaLayout);
			HtmlHelper.SetStyleSize(control.Style, fCurrentCellBounds.Size);
			fCurrentCell = control;
			fCurrentData = data;
			((BrickExporter)fHtmlExportContext.PrintingSystem.ExportersFactory.GetExporter(tableCell)).FillHtmlTableCell(this);
			if(NeedClipping(data)) {
				ClippingControl(data, fCurrentCellBounds, control, baselayout);
			}
		}
		static PaddingInfo AdjustPadding(PaddingInfo padding, TextAlignment textAlignment) {
			if(textAlignment.HasAnyFlag(TextAlignment.TopLeft, TextAlignment.MiddleLeft, TextAlignment.BottomLeft))
				padding.Right = 0;
			if(textAlignment.HasAnyFlag(TextAlignment.TopCenter, TextAlignment.TopJustify, TextAlignment.TopLeft, TextAlignment.TopRight))
				padding.Bottom = 0;
			padding.Dpi = GraphicsDpi.DeviceIndependentPixel;
			return padding;
		}
		protected virtual DXHtmlTable Build(HtmlExportContext htmlExportContext) {
			this.fHtmlExportContext = htmlExportContext;
			string emptyStyleName = htmlExportContext.ScriptContainer.RegisterCssClass("height:0px;width:0px;overflow:hidden;font-size:0px;line-height:0px;");
			emptyCellContent = string.Format(@"<!--[if lte IE 7]><div class=""{0}""></div><![endif]-->", emptyStyleName);
			DXHtmlTable table = CreateHtmlTable();
			if(htmlExportContext.MainExportMode == HtmlExportMode.SingleFile) {
				htmlExportContext.ProgressReflector.InitializeRange(CountObjects); 
			}
			SetupSpans(table);
			if(fHtmlExportContext.CancelPending) return null;
			return table;
		}
		protected abstract int CountObjects { get; }
		public abstract DXHtmlTable BuildTable(LayoutControlCollection layoutControls, bool correctImportBrickBounds, HtmlExportContext htmlExportContext);
		protected abstract DXHtmlTable CreateHtmlTable();
		protected abstract void SetupSpans(DXHtmlTable table);
#if DEBUGTEST
		internal static int Test_CalculateElement(int x0, int x1, int nearValue, int farValue) {
			return CalculateElement(x0, x1, nearValue, farValue);
		}
		internal static HtmlCellLayout Test_CalculateCellLayout(BrickStyle style, RectangleF area, RectangleF baseBounds) {
			HtmlCellLayout baseLayout = new HtmlCellLayout(style) { Padding = new PaddingInfo(style.Padding, GraphicsDpi.DeviceIndependentPixel) };
			return CalculateCellLayout(baseLayout, area, baseBounds);
		}
#endif
	}
	public class HtmlTableBuilder : HtmlBuilderBase {
		MegaTable megaTable;
		protected override int CountObjects {
			get { return megaTable.Objects.Length;}
		}
		public override DXHtmlTable BuildTable(LayoutControlCollection layoutControls, bool correctImportBrickBounds, HtmlExportContext htmlExportContext) {
			MegaTable megaTable = new MegaTable(layoutControls, true, correctImportBrickBounds);
			if(megaTable.IsEmpty)
				return null;
			this.megaTable = megaTable;
			return Build(htmlExportContext);
		}
		protected override DXHtmlTable Build(HtmlExportContext htmlExportContext) {
			DXHtmlTable table = base.Build(htmlExportContext);
			RemoveSpannedCells(table);
			InitFirstRowWidths(table);
			InitFirstColumnHeights(table);
			return table;
		}
		protected override void SetupSpans(DXHtmlTable table) {
			foreach(ObjectInfo objectInfo in megaTable.Objects) {
				if(fHtmlExportContext.CancelPending) break;
				System.Diagnostics.Debug.Assert(objectInfo.ColIndex >= 0 && objectInfo.RowIndex >= 0);
				BrickViewData data = (BrickViewData)objectInfo.Object;
				DXHtmlTableCell cell = table.Rows[objectInfo.RowIndex].Cells[objectInfo.ColIndex];
				FillCellContent(data, cell);
				if(cell.Controls.Count == 0)
					cell.Controls.Add(CreateEmptyCellControl());
				if(objectInfo.ColSpan > 1)
					cell.ColSpan = objectInfo.ColSpan;
				if(objectInfo.RowSpan > 1)
					cell.RowSpan = objectInfo.RowSpan;
				if(this.fHtmlExportContext.MainExportMode == HtmlExportMode.SingleFile)
					fHtmlExportContext.ProgressReflector.RangeValue++;
			}
		}
		private void RemoveSpannedCells(DXHtmlTable table) {
			Table cellTable = new Table(table.Rows.Count);
			for(int i = 0; i < table.Rows.Count; i++) {
				for(int j = 0; j < table.Rows[i].Cells.Count; j++) {
					if(table.Rows[i].Cells[j].ColSpan > 1 || table.Rows[i].Cells[j].RowSpan > 1)
						AddSpannedCells(cellTable, table, i, j);
				}
			}
			for(int i = 0; i < cellTable.RowCount; i++)
				foreach(DXHtmlTableCell cell in cellTable[i])
					table.Rows[i].Cells.Remove(cell);
		}
		private void AddSpannedCells(Table cellTable, DXHtmlTable table, int row, int column) {
			System.Diagnostics.Debug.Assert(cellTable.RowCount == table.Rows.Count);
			DXHtmlTableCell cell = table.Rows[row].Cells[column];
			int colSpan = cell.ColSpan > 0 ? cell.ColSpan : 1;
			int rowSpan = cell.RowSpan > 0 ? cell.RowSpan : 1;
			System.Diagnostics.Debug.Assert(row + rowSpan <= table.Rows.Count);
			System.Diagnostics.Debug.Assert(column + colSpan <= table.Rows[0].Cells.Count);
			for(int i = row; i < row + rowSpan; i++) {
				for(int j = column; j < column + colSpan; j++) {
					if(i == row && j == column)
						continue;
					cell = table.Rows[i].Cells[j];
					if(cell.ColSpan > 1 || cell.RowSpan > 1 || !IsEmptyCell(cell))
						continue;
					cellTable[i].Add(cell);
				}
			}
		}
		bool IsEmptyCell(DXHtmlTableCell cell) {
			if(cell.Controls.Count == 0)
				return true;
			return cell.Controls.Count == 1 && cell.Controls[0] is DXHtmlLiteralControl && ((DXHtmlLiteralControl)cell.Controls[0]).Text == emptyCellContent;
		}
		private DXHtmlTableRow CreateHtmlTableRow() {
			DXHtmlTableRow htmlTableRow = new DXHtmlTableRow();
			htmlTableRow.Style.Add(DXHtmlTextWriterStyle.VerticalAlign, "top");
			return htmlTableRow;
		}
		protected override DXHtmlTable CreateHtmlTable() {
			DXHtmlTable table = new DXHtmlTable();
			table.Style["border-width"] = "0px";
			table.Style["empty-cells"] = "show";
			table.Style["width"] = HtmlConvert.ToHtml(megaTable.Width);
			table.Style["height"] = HtmlConvert.ToHtml(megaTable.Height);
			table.Style.Add(DXHtmlTextWriterStyle.Position, "relative");
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Border = 0;
			for(int i = 0; i < megaTable.RowCount; i++) {
				if(this.fHtmlExportContext.CancelPending) break;
				DXHtmlTableRow htmlTableRow = CreateHtmlTableRow();
				table.Rows.Add(htmlTableRow);
				for(int j = 0; j < megaTable.ColumnCount; j++) {
					table.Rows[i].Cells.Add(new DXHtmlTableCell());
				}
			}
			return table;
		}
		void InitFirstRowWidths(DXHtmlTable table) {
			if(megaTable.ZeroRowNeeded) {
				InsertFirstZeroRow(table);
			}
			DXHtmlTableRow row = table.Rows[0];
			System.Diagnostics.Debug.Assert(megaTable.ColumnCount == row.Cells.Count);
			for(int i = 0; i < row.Cells.Count; i++) {
				HtmlHelper.SetStyleWidth(row.Cells[i].Style, megaTable.ColWidths[i]);
			}
		}
		void InitFirstColumnHeights(DXHtmlTable table) {
			System.Diagnostics.Debug.Assert(megaTable.RowHeights.Count == table.Rows.Count);
			if(megaTable.ZeroColumnNeeded) {
				InsertFirstZeroColumn(table);
			}
			for(int i = 0; i < table.Rows.Count; i++) {
				HtmlHelper.SetStyleHeight(table.Rows[i].Cells[0].Style, megaTable.RowHeights[i]);
			}
		}
		void InsertFirstZeroColumn(DXHtmlTable table) {
			for(int i = 0; i < table.Rows.Count; i++) {
				DXHtmlTableCell cell = new DXHtmlTableCell();
				HtmlHelper.SetStyleWidth(cell.Style, 0);
				table.Rows[i].Cells.Insert(0, cell);
			}
			megaTable.ColWidths.Insert(0, 0);
		}
		void InsertFirstZeroRow(DXHtmlTable table) {
			DXHtmlTableRow row = new DXHtmlTableRow();
			for(int i = 0; i < megaTable.ColumnCount; i++) {
				DXHtmlTableCell cell = new DXHtmlTableCell();
				HtmlHelper.SetStyleHeight(cell.Style, 0);
				row.Cells.Add(cell);
			}
			table.Rows.Insert(0, row);
			megaTable.RowHeights.Insert(0, 0);
		}
	}
	public class HtmlDivBuilder : HtmlBuilderBase {
		LayoutControlCollection layoutControls;
		protected override int CountObjects {
			get { return layoutControls.Count; }
		}
		public override DXHtmlTable BuildTable(LayoutControlCollection layoutControls, bool correctImportBrickBounds, HtmlExportContext htmlExportContext) {
			this.layoutControls = layoutControls;
			return Build(htmlExportContext);
		}
		protected override void SetupSpans(DXHtmlTable table) { 
			DXHtmlTableCell cell = table.Rows[0].Cells[0];
			Rectangle resultTableBounds = Rectangle.Empty;
			foreach(ILayoutControl item in layoutControls) {
				if(fHtmlExportContext.CancelPending) break;
				resultTableBounds = Rectangle.Union(resultTableBounds, item.Bounds);
				BrickViewData data = (BrickViewData)item;	
				DXHtmlDivision divControl = new DXHtmlDivision();
				divControl.Style.Add(DXHtmlTextWriterStyle.MarginTop, HtmlConvert.ToHtml(data.Bounds.Top));
				divControl.Style.Add(DXHtmlTextWriterStyle.MarginLeft, HtmlConvert.ToHtml(data.Bounds.Left));
				divControl.Style.Add(DXHtmlTextWriterStyle.Position, "absolute");
				FillCellContent(data, divControl);			
				cell.Controls.Add(divControl);
				if(this.fHtmlExportContext.MainExportMode == HtmlExportMode.SingleFile)
					fHtmlExportContext.ProgressReflector.RangeValue++;
			}
			if(resultTableBounds.X < 0) cell.Style["padding-left"] = HtmlConvert.ToHtml(-resultTableBounds.X);
			if(resultTableBounds.Y < 0) cell.Style["padding-top"] = HtmlConvert.ToHtml(-resultTableBounds.Y);
			table.Style["width"] = HtmlConvert.ToHtml(resultTableBounds.Width);
			table.Style["height"] = HtmlConvert.ToHtml(resultTableBounds.Height);
		}
		protected override DXHtmlTable CreateHtmlTable() {
			DXHtmlTable table = new DXHtmlTable();
			table.Style["border-width"] = "0px";
			table.Style["empty-cells"] = "show";
			table.Style.Add(DXHtmlTextWriterStyle.Position, "relative");
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Border = 0;
			table.Rows.Add(new DXHtmlTableRow());
			table.Rows[0].Cells.Add(new DXHtmlTableCell());
			table.Rows[0].Cells[0].Style.Add(DXHtmlTextWriterStyle.Position, "absolute");
			return table;
		}
	}
}
