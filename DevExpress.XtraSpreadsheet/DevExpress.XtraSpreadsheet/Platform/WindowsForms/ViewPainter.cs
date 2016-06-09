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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Drawing {
	public delegate void DrawAtPageDelegate(GraphicsCache cache);
	#region SpreadsheetViewPainter (abstract class)
	public abstract class SpreadsheetViewPainter : IDisposable, IPatternLinePaintingSupport, IDrawingBoxVisitor {
		#region BindingImages
#if (!SL)
		public static Bitmap CreateBitmapFromResources(string name) {
			System.IO.Stream stream = System.Reflection.Assembly.GetAssembly(typeof(DocumentLayout)).GetManifestResourceStream(name);
			return (Bitmap)DevExpress.Data.Utils.ImageTool.ImageFromStream(stream);
		}
#else
		public static Image CreateBitmapFromResources(string name) {
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			if (stream == null) return null;
			Image image = new Image();
			BitmapImage b = new BitmapImage();
			b.SetSource(stream);
			image.Source = b;
			return image;
		}
#endif
		static readonly List<Image> bindingImages = CreateImages();
		static Image BindingCellFieldImage { get { return bindingImages[0]; } }
		static Image BindingCellFieldPictureImage { get { return bindingImages[1]; } }
		static List<Image> CreateImages() {
			List<Image> result = new List<Image>();
			result.Add(CreateBitmapFromResources("DevExpress.XtraSpreadsheet.Images.IconSets.MailMergeBinding.png"));
			result.Add(CreateBitmapFromResources("DevExpress.XtraSpreadsheet.Images.IconSets.ImageBinding.png"));
			return result;
		}
		#endregion
		#region CreateBrushHatchStyleTable
		static readonly Dictionary<XlPatternType, HatchStyle> brushHatchStyles = CreateBrushHatchStyleTable();
		static Dictionary<XlPatternType, HatchStyle> CreateBrushHatchStyleTable() {
			Dictionary<XlPatternType, HatchStyle> result = new Dictionary<XlPatternType, HatchStyle>();
			result.Add(XlPatternType.None, HatchStyle.Percent05); 
			result.Add(XlPatternType.Solid, HatchStyle.Percent90); 
			result.Add(XlPatternType.MediumGray, HatchStyle.Percent50);
			result.Add(XlPatternType.DarkGray, HatchStyle.Percent70);
			result.Add(XlPatternType.LightGray, HatchStyle.Percent25);
			result.Add(XlPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(XlPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(XlPatternType.DarkDown, HatchStyle.DarkDownwardDiagonal);
			result.Add(XlPatternType.DarkUp, HatchStyle.DarkUpwardDiagonal);
			result.Add(XlPatternType.DarkGrid, HatchStyle.SmallCheckerBoard);
			result.Add(XlPatternType.DarkTrellis, HatchStyle.Trellis);
			result.Add(XlPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(XlPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(XlPatternType.LightDown, HatchStyle.LightDownwardDiagonal);
			result.Add(XlPatternType.LightUp, HatchStyle.LightUpwardDiagonal);
			result.Add(XlPatternType.LightGrid, HatchStyle.SmallGrid);
			result.Add(XlPatternType.LightTrellis, HatchStyle.Percent30);
			result.Add(XlPatternType.Gray125, HatchStyle.Percent20);
			result.Add(XlPatternType.Gray0625, HatchStyle.Percent10);
			return result;
		}
		#endregion
		#region Fields
		const int dataValidationInvalidCircleWidthInPixels = 2;
		const int dataValidationMessageBoundsWidthInPixels = 1;
		static Color dataValidationInvalidCircleColor = Color.Red;
		static Color dataValidationMessageForeColor = Color.FromArgb(93, 93, 93);
		static Color dataValidationMessageBackgroundColor = Color.FromArgb(255, 255, 225);
		readonly SpreadsheetView view;
		readonly SpreadsheetControl control;
		readonly ICellErrorTextProvider errorTextProvider = new DefaultCellErrorTextProvider();
		GraphicsCache cache;
		Painter painter;
		SpreadsheetVerticalPatternLinePainter verticalPainter;
		SpreadsheetHorizontalPatternLinePainter horizontalPainter;
		SpreadsheetHeaderPainter headerPainter;
		FormulaRangesPainter formulaRangesPainter;
		FrozenPanesSeparatorPainter frozenPanesSeparatorPainter;
		SpreadsheetCommentPainter commentPainter;
		SpreadsheetIndicatorPainter indicatorPainter;
		SpreadsheetGroupItemsPainter groupPainter;
		bool forcePaintEmptyCellBackground;
		bool shouldExpandBounds;
		int minReadableTextHeight;
		#endregion
		protected SpreadsheetViewPainter(SpreadsheetView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.control = (SpreadsheetControl)view.Control;
			Initialize();
		}
		#region Properties
		public SpreadsheetView View { get { return view; } }
		protected internal SpreadsheetControl Control { get { return control; } }
		protected internal DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		public virtual UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		protected internal DocumentLayout DocumentLayout { get { return Control.InnerControl.DesignDocumentLayout; } }
		protected internal SpreadsheetHeaderPainter HeaderPainter { get { return headerPainter; } }
		protected internal SpreadsheetGroupItemsPainter GroupPainter { get { return groupPainter; } }
		protected internal SpreadsheetCommentPainter CommentPainter { get { return commentPainter; } }
		protected internal SpreadsheetIndicatorPainter IndicatorPainter { get { return indicatorPainter; } }
		protected GraphicsCache Cache { get { return cache; } }
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal virtual void Initialize() {
			this.verticalPainter = new SpreadsheetVerticalPatternLinePainter(this, DocumentModel.LayoutUnitConverter);
			this.horizontalPainter = new SpreadsheetHorizontalPatternLinePainter(this, DocumentModel.LayoutUnitConverter);
			this.headerPainter = CreateHeaderPainter();
			this.formulaRangesPainter = new FormulaRangesPainter(DocumentModel);
			this.frozenPanesSeparatorPainter = CreateFrozenPanesSeparatorPainter();
			this.commentPainter = CreateCommentPainter();
			this.indicatorPainter = new SpreadsheetIndicatorPainter(Control);
			this.groupPainter = CreateGroupPainter();
			DocumentModel.SkinGridlineColor = GetDefaultGridlineColor();
			DocumentModel.SkinForeColor = GetDefaultForeColor();
			DocumentModel.SkinBackColor = GetDefaultBackColor();
		}
		public void Draw(GraphicsCache cache) {
			this.cache = cache;
			this.minReadableTextHeight = (int)Math.Round(Control.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(6, cache.Graphics.DpiY) / View.ZoomFactor);
			this.shouldExpandBounds = (View.ZoomFactor == 1);
			using (Painter painter = view.Control.InnerControl.MeasurementAndDrawingStrategy.CreateDocumentPainter(new GraphicsCacheDrawingSurface(cache))) {
				this.painter = painter;
				PreprocessHeaderAppearance();
				IList<Page> pages = DocumentLayout.Pages;
				if (pages.Count > 0) {
					int count = pages.Count;
					for (int i = 0; i < count; i++) {
						DrawPage(pages[i]);
					}
				}
				else
					cache.FillRectangle(cache.GetSolidBrush(GetDefaultBackColor()), Control.ViewBounds);
				DrawGroupPage(DocumentLayout.GroupItemsPage);
				DrawHeaderPage(DocumentLayout.HeaderPage);
				DrawFrozenPanesSeparator();
			}
		}
		void PreprocessHeaderAppearance() {
			AppearanceDefault appearanceDefault = new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.ControlDarkDark, SystemColors.ActiveBorder, HorzAlignment.Center, VertAlignment.Center);
			SkinElement element = SkinPaintHelper.GetSkinElement(LookAndFeel, GridSkins.SkinHeader);
			if (element != null)
				element.ApplyForeColorAndFont(appearanceDefault);
			AppearanceObject appearance = new AppearanceObject(appearanceDefault);
			appearance.DrawString(this.cache, " ", view.Control.ViewBounds);
		}
		protected internal virtual void DrawAtPageCore(GraphicsCache cache, Page page, DrawAtPageDelegate draw) {
			DrawAtPageCore(cache, page.ClientBounds, draw);
		}
		protected internal virtual void DrawAtPageCore(GraphicsCache cache, Rectangle bounds, DrawAtPageDelegate draw) {
			Rectangle correctedBounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width, bounds.Height);
			Graphics gr = cache.Graphics;
			Matrix oldTransform = gr.Transform.Clone();
			try {
				gr.ResetTransform();
				gr.TranslateTransform(correctedBounds.X, correctedBounds.Y);
				gr.MultiplyTransform(oldTransform);
				float zoomFactor = View.ZoomFactor;
				gr.ScaleTransform(zoomFactor, zoomFactor);
				using (HdcZoomModifier zoomModifier = new HdcZoomModifier(gr, zoomFactor)) {
					Point origin = new Point(correctedBounds.X, correctedBounds.Y);
					using (HdcOriginModifier originModifier = new HdcOriginModifier(gr, origin, zoomFactor, HdcOriginModifier.Mode.Combine)) {
						draw(cache);
					}
				}
			}
			finally {
				gr.Transform = oldTransform.Clone();
			}
		}
		void DrawPage(Page page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawPageClipped(page);
			};
			DrawAtPageCore(cache, page, draw);
		}
		const int lastCellPixelPadding = 4;
		int GetClipPaddingWidth(PageAlignment pageAlign) {
			if (pageAlign == PageAlignment.BottomLeft || pageAlign == PageAlignment.Left || pageAlign == PageAlignment.TopLeft)
				return 0;
			return DocumentLayout.UnitConverter.PixelsToLayoutUnits(lastCellPixelPadding, DocumentModel.DpiX);
		}
		int GetClipPaddingHeight(PageAlignment pageAlign) {
			if (pageAlign == PageAlignment.TopRight || pageAlign == PageAlignment.Top || pageAlign == PageAlignment.TopLeft)
				return 0;
			return DocumentLayout.UnitConverter.PixelsToLayoutUnits(lastCellPixelPadding, DocumentModel.DpiY);
		}
		void DrawPageClipped(Page page) {
			using (GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {
				try {
					Rectangle clipBounds = page.Bounds;
					clipBounds.Width += GetClipPaddingWidth(page.DesignAlignment);
					clipBounds.Height += GetClipPaddingHeight(page.DesignAlignment);
					painter.ClipBounds = clipBounds;
					DrawPageContent(page);
				}
				finally {
					cache.ClipInfo.RestoreClip(clipState);
				}
			}
		}
		void DrawPageContent(Page page) {
			Point savedOrigin = cache.Graphics.RenderingOrigin;
			cache.Graphics.RenderingOrigin = new Point(1 - page.Bounds.X, 1 - page.Bounds.Y);
			try {
				DrawDefaultCellBackground(page);
				DrawCellsBackground(page);
			}
			finally {
				cache.Graphics.RenderingOrigin = savedOrigin;
			}
			DrawPageBorders(page, page.VerticalGridBorders);
			DrawPageBorders(page, page.HorizontalGridBorders);
			DrawPageBorders(page, page.VerticalBorders);
			DrawPageBorders(page, page.HorizontalBorders);
			DrawIndicators(page);
			DrawCellsContent(page);
			DrawPivotTableItems(page);
			DrawSelection(page);
			DrawCutCopyRange(page);
			DrawAutoFilters(page);
			DrawFormulaRanges(page);
			DrawObjectBoxes(page);
			DrawPictureSelection(page);
			DrawParameterSelection(page);
			DrawPrintRange(page);
			DrawMailMergeRanges(page);
			DrawDataValidation(page);
			DrawComments(page);
		}
		void DrawMailMergeRanges(Page page) {
			if (!page.Sheet.Workbook.ShowMailMergeRanges)
				return;
			MailMergeOptions options = new MailMergeOptions(page.Sheet.Workbook);
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(Cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			RangeMailMergeLayoutItem headerItem = View.SelectionLayout.MailMergeHeaderItem;
			if (headerItem != null) {
				headerItem.Update(page);
				headerItem.Draw(selectionPainter);
			}
			RangeMailMergeLayoutItem footerItem = View.SelectionLayout.MailMergeFooterItem;
			if (footerItem != null) {
				footerItem.Update(page);
				footerItem.Draw(selectionPainter);
			}
			RangeMailMergeLayoutItem detailItem = View.SelectionLayout.MailMergeDetailItem;
			if (detailItem != null) {
				detailItem.Update(page);
				detailItem.Draw(selectionPainter);
			}
			if (options.DetailLevels.Count > 0) {
				foreach (RangeMailMergeLayoutItem layoutItem in View.SelectionLayout.MailMergeDetailLevelList) {
					layoutItem.Update(page);
					layoutItem.Draw(selectionPainter);
				}
			}
			if (options.HasGroup) {
				foreach (RangeMailMergeLayoutItem layoutItem in View.SelectionLayout.MailMergeGroupHeadersList) {
					layoutItem.Update(page);
					layoutItem.Draw(selectionPainter);
				}
				foreach (RangeMailMergeLayoutItem layoutItem in View.SelectionLayout.MailMergeGroupFootersList) {
					layoutItem.Update(page);
					layoutItem.Draw(selectionPainter);
				}
			}
		}
		void DrawObjectBoxes(Page page) {
			foreach (DrawingBox drawingBox in page.DrawingBoxes)
				drawingBox.Visit(this);
		}
		#region IDrawingBoxVisitor Members
		void IDrawingBoxVisitor.Visit(ChartBox value) {
			DrawChartBox(value);
		}
		void IDrawingBoxVisitor.Visit(PictureBox value) {
			DrawPictureBox(value);
		}
		void IDrawingBoxVisitor.Visit(ShapeBox value) {
#if DEBUG
			DrawShapeBox(value);
#endif
		}
		#endregion
		void DrawChartBox(ChartBox box) {
			Chart chart = box.Chart;
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null || chart.Controller.ChartModel == null) {
				DrawChartPlaceHolder(box);
				return;
			}
			if (chart.Is3DChart) {
				OfficeImage image = chart.GetCachedImage(box.Bounds.Size);
				if (image != null)
					painter.DrawImage(image, box.Bounds);
				else
					DrawChartPlaceHolder(box);
			}
			else {
				ChartsModel.ModelRect rect = new ChartsModel.ModelRect(box.Bounds.Left, box.Bounds.Top, box.Bounds.Width, box.Bounds.Height);
				chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, cache.Graphics));
			}
		}
		void DrawChartPlaceHolder(ChartBox box) {
			painter.FillRectangle(Brushes.White, box.Bounds);
			painter.DrawRectangle(Pens.Red, box.Bounds);
		}
		void DrawPictureBox(PictureBox box) {
			Rectangle bounds = box.Bounds;
			Point center = RectangleUtils.CenterPoint(bounds);
			bool transformApplied = painter.TryPushRotationTransform(center, DocumentModel.GetBoxRotationAngleInDegrees(box));
			painter.PushSmoothingMode(transformApplied);
			try {
				DrawPictureBoxCore(box, bounds);
			}
			finally {
				painter.PopSmoothingMode();
				if (transformApplied)
					painter.PopTransform();
			}
		}
		void DrawPictureBoxCore(PictureBox box, Rectangle bounds) {
			bool highQualityScaling = control.Options.View.Pictures.HighQualityScalingAllowed;
			InterpolationMode savedInterpolationMode = cache.Graphics.InterpolationMode;
			if (highQualityScaling)
				cache.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			try {
				Rectangle clipBounds = box.ClipBounds;
				GraphicsUnit graphicsUnit = GraphicsUnit.Pixel;
				Image image = GetImage(box);
				Rectangle originalImageBounds = Rectangle.Round(image.GetBounds(ref graphicsUnit));
				clipBounds.X += originalImageBounds.X;
				clipBounds.Y += originalImageBounds.Y;
				if (box.ColorMatrixElements == null) {
					cache.Graphics.DrawImage(image, bounds, clipBounds.X, clipBounds.Y, clipBounds.Width, clipBounds.Height, GraphicsUnit.Pixel);
				}
				else {
					ColorMatrix colorMatrix = new ColorMatrix(box.ColorMatrixElements);
					ImageAttributes imageAttributes = new ImageAttributes();
					imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					cache.Graphics.DrawImage(image, bounds, clipBounds.X, clipBounds.Y, clipBounds.Width, clipBounds.Height, GraphicsUnit.Pixel, imageAttributes);
				}
				cache.Graphics.DrawRectangle(box.BorderPen, bounds);
			}
			finally {
				if (highQualityScaling)
					cache.Graphics.InterpolationMode = savedInterpolationMode;
			}
		}
		Image GetImage(PictureBox box) {
#if !SL
			return box.NativeImage;
#else
			byte[] brickImageBytes = DevExpress.Data.Printing.Native.DxDibImageConverter.Encode(box.NativeImage, false);
			return Image.FromStream(new System.IO.MemoryStream(brickImageBytes));
#endif
		}
		void DrawShapeBox(ShapeBox value) {
			IShapeRenderService service = value.Shape.DocumentModel.GetService<IShapeRenderService>();
			if (service == null)
				return;
			Point center = RectangleUtils.CenterPoint(value.Bounds);
			bool transformApplied = painter.TryPushRotationTransform(center, DocumentModel.GetBoxRotationAngleInDegrees(value));
			SmoothingMode oldSmoothingMode = cache.Graphics.SmoothingMode;
			cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				service.RenderShape(value.Shape, cache.Graphics, value.Bounds);
			}
			finally {
				cache.Graphics.SmoothingMode = oldSmoothingMode;
				if (transformApplied)
					painter.PopTransform();
			}
		}
		void DrawCellsBackground(Page page) {
			IList<SingleCellTextBox> singleBoxes = page.Boxes;
			int count = singleBoxes.Count;
			for (int i = 0; i < count; i++)
				DrawCellBackground(page, singleBoxes[i]);
			IList<ComplexCellTextBox> complexBoxes = page.ComplexBoxes;
			count = complexBoxes.Count;
			for (int i = 0; i < count; i++)
				DrawCellBackground(page, complexBoxes[i]);
		}
		void DrawDefaultCellBackground(Page page) {
			forcePaintEmptyCellBackground = false;
			IActualFillInfo fill = DocumentModel.StyleSheet.CellStyles.Normal.ActualFill;
			if (fill.FillType == ModelFillType.Pattern)
				DrawDefaultCellBackgroundFromPatternFill(page.Bounds, fill);
			else
				DrawDefaultCellBackgroundFromGradientFill(page, fill.GradientFill);
		}
		void DrawDefaultCellBackgroundFromPatternFill(Rectangle pageBounds, IActualFillInfo fill) {
			if (pageBounds == Rectangle.Empty)
				return;
			Color backColor = Cell.GetBackgroundColor(fill);
			forcePaintEmptyCellBackground = DXColor.IsTransparentOrEmpty(backColor);
			CellBackgroundDisplayFormat displayFormat = new CellBackgroundDisplayFormat();
			displayFormat.BackColor = backColor;
			displayFormat.Bounds = pageBounds;
			displayFormat.PatternType = fill.PatternType;
			displayFormat.ForeColor = fill.ForeColor;
			DrawDefaultCellBackgroundFromPatternFillCore(displayFormat);
		}
		void DrawDefaultCellBackgroundFromGradientFill(Page page, IActualGradientFillInfo gradientFill) {
			forcePaintEmptyCellBackground = true;
			CellBackgroundDisplayFormat displayFormat = new CellBackgroundDisplayFormat();
			displayFormat.GradientFill = gradientFill;
			int gridRowsCount = page.GridRows.Count;
			int gridColumnsCount = page.GridColumns.Count;
			for (int i = 0; i < gridColumnsCount; i++) {
				PageGridItem gridColumn = page.GridColumns[i];
				for (int j = 0; j < gridRowsCount; j++) {
					PageGridItem gridRow = page.GridRows[j];
					displayFormat.Bounds = new Rectangle(gridColumn.Near, gridRow.Near, gridColumn.Extent, gridRow.Extent);
					DrawCellBackgroundFromBrush(displayFormat);
				}
			}
		}
		void DrawCellBackground(Page page, ICellTextBox box) {
			CellBackgroundDisplayFormat displayFormat = box.CalculateBackgroundDisplayFormat(page, page.DocumentLayout.DocumentModel);
			Action defaultDraw = () => { DrawCellBackgroundCore(displayFormat); };
			if (RaiseCustomDrawCellBackground(displayFormat, page, box, defaultDraw))
				return;
			defaultDraw();
		}
		void DrawCellBackgroundCore(CellBackgroundDisplayFormat displayFormat) {
			Rectangle fillBounds = displayFormat.Bounds;
			if (fillBounds == Rectangle.Empty)
				return;
			if (displayFormat.GradientFill != null)
				DrawCellBackgroundFromBrush(displayFormat);
			else
				DrawDefaultCellBackgroundFromPatternFillCore(displayFormat);
		}
		void DrawDefaultCellBackgroundFromPatternFillCore(CellBackgroundDisplayFormat displayFormat) {
			Rectangle fillBounds = displayFormat.Bounds;
			fillBounds = ExpandBounds(fillBounds);
			DrawCellBackgroundFromColor(displayFormat.BackColor, fillBounds);
			if (displayFormat.ShouldUseForeColor) {
				using (Brush brush = CreatePatternBrush(displayFormat.PatternType, displayFormat.ForeColor)) {
					cache.FillRectangle(brush, fillBounds);
				}
			}
		}
		Brush CreatePatternBrush(XlPatternType patternType, Color foreColor) {
			HatchStyle hatchStyle;
			if (!brushHatchStyles.TryGetValue(patternType, out hatchStyle))
				return new SolidBrush(foreColor);
			return new HatchBrush(hatchStyle, foreColor, Color.Transparent);
		}
		Rectangle ExpandBounds(Rectangle bounds) {
			if (shouldExpandBounds)
				return new Rectangle(bounds.Left, bounds.Top, bounds.Width + 1, bounds.Height + 1);
			else
				return bounds;
		}
		void FillRectangleDefaultBackColor(Rectangle fillBounds) {
			if (forcePaintEmptyCellBackground)
				cache.FillRectangle(DocumentModel.SkinBackColor, fillBounds);
		}
		void DrawCellBackgroundFromBrush(CellBackgroundDisplayFormat displayFormat) {
			Rectangle fillBounds = ExpandBounds(displayFormat.Bounds);
			Brush brush = displayFormat.CreateGradientBrush();
			if (brush == Brushes.Transparent)
				FillRectangleDefaultBackColor(fillBounds);
			else {
				using (brush) {
					cache.FillRectangle(brush, fillBounds);
				}
			}
		}
		void DrawCellBackgroundFromColor(Color backColor, Rectangle fillBounds) {
			if (DXColor.IsTransparentOrEmpty(backColor))
				FillRectangleDefaultBackColor(fillBounds);
			else
				cache.FillRectangle(backColor, fillBounds);
		}
		bool RaiseCustomDrawCellBackground(CellBackgroundDisplayFormat displayFormat, Page page, ICellTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawCellBackground)
				return false;
			CustomDrawCellBackgroundEventArgs args = new CustomDrawCellBackgroundEventArgs(displayFormat, control, page, box, this.cache, defaultDraw);
			control.RaiseCustomDrawCellBackground(args);
			return args.Handled;
		}
		bool RaiseCustomDrawCellContent(CellForegroundDisplayFormat displayFormat, Page page, ICellTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawCell)
				return false;
			cache.ClipInfo.SetClip(originalClipBounds);
			CustomDrawCellEventArgs args = new CustomDrawCellEventArgs(displayFormat, control, page, box, this.cache, defaultDraw);
			control.RaiseCustomDrawCell(args);
			return args.Handled;
		}
		void DrawPageBorders(Page page, List<PageBorderCollection> borders) {
			int count = borders.Count;
			for (int i = 0; i < count; i++)
				DrawBorders(page, borders[i]);
		}
		void DrawBorders(Page page, PageBorderCollection borders) {
			List<BorderLineBox> boxes = borders.Boxes;
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				DrawBorder(boxes[i], borders.GetBounds(page, boxes[i]), page);
		}
		void DrawBorder(BorderLineBox box, Rectangle boxBounds, Page page) {
			ColorModelInfo borderColorInfo = DocumentModel.Cache.ColorModelInfoCache[box.ColorIndex];
			Color borderColor = borderColorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
			if (DXColor.IsEmpty(borderColor))
				borderColor = DXColor.Black;
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(box.LineStyle);
			if (boxBounds.Width < boxBounds.Height)
				borderLine.Draw(verticalPainter, boxBounds, borderColor);
			else
				borderLine.Draw(horizontalPainter, boxBounds, borderColor);
		}
		void DrawSelection(Page page) {
			if (!Control.ShouldDrawSelection())
				return;
			PageSelectionLayoutItem selection = View.SelectionLayout.GetPageSelection(page);
			if (selection != null) {
				PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
				selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
				selection.Update(page);
				selection.Draw(selectionPainter);
			}
		}
		void DrawCutCopyRange(Page page) {
			if (!Control.Enabled)
				return;
			var copiedRange = View.SelectionLayout.GetCutCopyRange(page);
			if (copiedRange == null || copiedRange.Count <= 0)
				return;
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			copiedRange.Update(page);
			copiedRange.InflateBounds(-1 * selectionPainter.OnePixel);
			copiedRange.Draw(selectionPainter);
		}
		void DrawAutoFilters(Page page) {
			View.AutoFilterLayout.Update(page);
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			selectionPainter.DrawHotZones(View.AutoFilterLayout.HotZones);
		}
		void DrawPrintRange(Page page) {
			if (!Control.ShouldDrawSelection())
				return;
			PrintRangeSelectionLayoutItem item = View.SelectionLayout.GetPrintRangeSelection();
			if (item != null) {
				PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
				selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
				item.Update(page);
				item.Draw(selectionPainter);
			}
		}
		void DrawComments(Page page) {
			View.CommentLayout.Update(page);
			foreach (CommentBox box in page.CommentBoxes) {
				DrawComment(box);
			}
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			DrawCommentHotZones(View.CommentLayout.Items, selectionPainter);
		}
		void DrawComment(CommentBox box) {
			CommentPainter.Draw(Cache, box);
		}
		void DrawCommentHotZones(CommentLayoutItemCollection items, PageSpreadsheetSelectionPainter hotZonePainter) {
			foreach (CommentLayoutItem item in items) {
				hotZonePainter.DrawHotZones(item.ResizeHotZones);
			}
		}
		void DrawIndicators(Page page) {
			foreach (IndicatorBox box in page.IndicatorBoxes) {
				DrawIndicator(box);
			}
		}
		void DrawIndicator(IndicatorBox box) {
			indicatorPainter.Draw(cache, box);
		}
		void DrawDataValidation(Page page) {
			View.DataValidationLayout.Update(page);
			DrawDataValidationHotZone(View.DataValidationLayout.HotZone);
			DrawDataValidationInvalidCircle(View.DataValidationLayout.InvalidDataRectangles);
			DrawDataValidationMessage(View.DataValidationLayout.MessageLayout);
		}
		void DrawDataValidationHotZone(DataValidationHotZone hotZone) {
			if (hotZone == null)
				return;
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			selectionPainter.DrawHotZone(hotZone);
		}
		void DrawDataValidationInvalidCircle(List<Rectangle> rectangles) {
			if (rectangles.Count == 0)
				return;
			Graphics graphics = cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				int circleWidth = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(dataValidationInvalidCircleWidthInPixels, DocumentModel.Dpi);
				using (Pen pen = new Pen(dataValidationInvalidCircleColor, circleWidth)) {
					foreach (Rectangle rect in rectangles) {
						graphics.DrawEllipse(pen, rect);
					}
				}
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
		void DrawDataValidationMessage(DataValidationMessageLayout messageLayout) {
			if (messageLayout == null || messageLayout.BoundsMessage.IsEmpty)
				return;
			Graphics graphics = cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				using (Pen pen = new Pen(dataValidationMessageForeColor, dataValidationMessageBoundsWidthInPixels)) {
					graphics.FillRectangle(new SolidBrush(dataValidationMessageBackgroundColor), messageLayout.BoundsMessage);
					graphics.DrawRectangle(pen, messageLayout.BoundsMessage);
					DevExpress.Utils.Text.TextUtils.DrawString(graphics, messageLayout.Title, messageLayout.TitleFontInfo.Font, dataValidationMessageForeColor, messageLayout.BoundsTitle, Rectangle.Empty, StringFormat.GenericTypographic);
					DevExpress.Utils.Text.TextUtils.DrawString(graphics, messageLayout.Text, messageLayout.TextFontInfo.Font, dataValidationMessageForeColor, messageLayout.BoundsText, Rectangle.Empty, StringFormat.GenericTypographic);
				}
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
		void DrawParameterSelection(Page page) {
			if (!Control.Enabled)
				return;
			if (Control.InnerControl.DocumentModel.ShowReferenceSelection) {
				PageDashSelectionLayoutItem selection = View.SelectionLayout.GetPageDashSelection(page);
				if (selection != null) {
					PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
					selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
					selection.Update(page);
					selection.Draw(selectionPainter);
				}
			}
			else {
			}
		}
		void DrawPictureSelection(Page page) {
			if (!Control.Enabled)
				return;
			PictureSelectionLayoutItemCollection selections = View.SelectionLayout.GetPictureSelection(page);
			int count = selections.Count;
			if (count == 0)
				return;
			PictureSpreadsheetSelectionPainter selectionPainter = new PictureSpreadsheetSelectionPainter(cache);
			for (int i = 0; i < count; i++) {
				ISelectionLayoutItem item = selections[i];
				item.Update(page);
				item.Draw(selectionPainter);
			}
		}
		Rectangle originalClipBounds;
		void DrawCellsContent(Page page) {
			using (GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {
				this.originalClipBounds = Rectangle.Round(cache.Graphics.ClipBounds);
				try {
					DrawCellsContentCore(page);
				}
				finally {
					cache.ClipInfo.RestoreClip(clipState);
				}
			}
		}
		void DrawCellsContentCore(Page page) {
			IList<SingleCellTextBox> singleBoxes = page.Boxes;
			int count = singleBoxes.Count;
			for (int i = 0; i < count; i++)
				DrawCellTextBox(page, singleBoxes[i]);
			IList<ComplexCellTextBox> complexBoxes = page.ComplexBoxes;
			count = complexBoxes.Count;
			for (int i = 0; i < count; i++)
				DrawCellTextBox(page, complexBoxes[i]);
		}
		bool ShouldDrawCellContent(Page page, ICellTextBox box) {
			if (control.InnerControl.IsInplaceEditorActive) {
				ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
				if (control.InnerControl.InplaceEditor.CellPosition.Equals(cell.Position))
					return false;
			}
			return true;
		}
		void DrawCellTextBox(Page page, ICellTextBox box) {
			if (!ShouldDrawCellContent(page, box))
				return;
			CellForegroundDisplayFormat displayFormat = box.CalculateForegroundDisplayFormat(page, errorTextProvider, control.HasCustomDrawCell);
			Action defaultDraw = () => { DrawCellTextBoxCore(page, box, displayFormat); };
			if (RaiseCustomDrawCellContent(displayFormat, page, box, defaultDraw))
				return;
			defaultDraw();
		}
		void DrawCellTextBoxCore(Page page, ICellTextBox box, CellForegroundDisplayFormat displayFormat) {
			Rectangle clipBounds = Rectangle.Intersect(box.GetClipBounds(page), originalClipBounds);
			System.Diagnostics.Debug.Assert(!clipBounds.IsEmpty);
			cache.ClipInfo.SetClip(clipBounds);
			DrawCellTextBoxText(page, box, displayFormat);
			DrawDiagonalDownBorder(page, box, displayFormat);
			DrawDiagonalUpBorder(page, box, displayFormat);
			DrawBindingSymbol(page, box);
		}
		void DrawDiagonalDownBorder(Page page, ICellTextBox box, CellForegroundDisplayFormat displayFormat) {
			if (displayFormat.DiagonalDownBorderLineStyle == XlBorderLineStyle.None || DXColor.IsTransparentOrEmpty(displayFormat.DiagonalBorderColor))
				return;
			Rectangle bounds = box.GetFillBounds(page);
			PointF from = bounds.Location;
			PointF to = new PointF(bounds.Right, bounds.Bottom);
			DrawDiagonalBorderCore(from, to, displayFormat.DiagonalDownBorderLineStyle, displayFormat.DiagonalBorderColor);
		}
		void DrawDiagonalUpBorder(Page page, ICellTextBox box, CellForegroundDisplayFormat displayFormat) {
			if (displayFormat.DiagonalUpBorderLineStyle == XlBorderLineStyle.None || DXColor.IsTransparentOrEmpty(displayFormat.DiagonalBorderColor))
				return;
			Rectangle bounds = box.GetFillBounds(page);
			PointF from = new PointF(bounds.Left, bounds.Bottom);
			PointF to = new PointF(bounds.Right, bounds.Top);
			DrawDiagonalBorderCore(from, to, displayFormat.DiagonalUpBorderLineStyle, displayFormat.DiagonalBorderColor);
		}
		void DrawDiagonalBorderCore(PointF from, PointF to, XlBorderLineStyle style, Color color) {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(style);
			Graphics graphics = Cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				borderLine.Draw(horizontalPainter, from, to, color, this.DocumentLayout.LineThicknessTable[style]);
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
		void DrawCellTextBoxText(Page page, ICellTextBox box, CellForegroundDisplayFormat displayFormat) {
			ICell cell = displayFormat.Cell;
			IConditionalFormattingPainters condFmts; 
			if (cell.ConditionalFormattingStoppedAtPriority > 0) {
				condFmts = cell.Worksheet.ConditionalFormattings.GetPainters(cell, cell.ConditionalFormattingStoppedAtPriority);
				ConditionalFormattingPainter condFmtPainter = new ConditionalFormattingPainter(cell, cache, displayFormat.Bounds);
				condFmtPainter.Visit(condFmts.DataBar);
				condFmtPainter.Visit(condFmts.IconSet);
			}
			else
				condFmts = ConditionalFormattingDummyPainters.GetInstance();
			if (String.IsNullOrEmpty(displayFormat.Text))
				return;
			if (displayFormat.Bounds.Height < minReadableTextHeight) {
				DrawTextOptimized(displayFormat);
				return;
			}
			int stringFormatKey = CalculateAlignmentHash(cell.ActualHorizontalAlignment, cell.ActualAlignment.Vertical, cell.ActualAlignment.WrapText);
			StringFormat stringFormat;
			if (!stringFormats.TryGetValue(stringFormatKey, out stringFormat))
				return;
			if (condFmts.ShowValues) {
				if ((stringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0) {
					DevExpress.Utils.Text.TextUtils.DrawString(cache.Graphics, displayFormat.Text, displayFormat.FontInfo.Font, displayFormat.ForeColor, displayFormat.Bounds, Rectangle.Empty, stringFormat, null, DocumentModel.WordBreakProvider);
				}
				else
					painter.DrawString(displayFormat.Text, displayFormat.FontInfo, displayFormat.ForeColor, displayFormat.Bounds, stringFormat);
			}
		}
		void DrawTextOptimized(CellForegroundDisplayFormat displayFormat) {
			using (Brush brush = CreatePlaceholderBrush(displayFormat.ForeColor)) {
				Rectangle actualBounds = Rectangle.Inflate(displayFormat.Bounds, 0, -1);
				if (actualBounds.Height <= 0)
					actualBounds.Height = 1;
				cache.Graphics.FillRectangle(brush, actualBounds);
			}
		}
		protected internal virtual Brush CreatePlaceholderBrush(Color foreColor) {
			return new HatchBrush(HatchStyle.Percent50, DXColor.Transparent, foreColor);
		}
		void DrawBindingSymbol(Page page, ICellTextBox box) {
			if (!page.Sheet.Workbook.ShowMailMergeRanges)
				return;
			if (box.HasBindingPicture)
				DrawBindingSymbolCore(page, box, BindingCellFieldPictureImage);
			else
				if (box.HasBindingField)
					DrawBindingSymbolCore(page, box, BindingCellFieldImage);
		}
		void DrawBindingSymbolCore(Page page, ICellTextBox box, Image image) {
			Rectangle bounds = box.GetFillBounds(page);
			Size size = image.Size;
			Point location = new Point(bounds.Right - size.Width - 1, bounds.Top);
			painter.DrawImage(OfficeImage.CreateImage(image), new Rectangle(location, size));
		}
		FormulaReferencedRanges GetReferencedRanges(Page page) {
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			foreach (int index in page.Sheet.Selection.SelectedDrawingIndexes) {
				Chart chart = page.Sheet.DrawingObjects[index] as Chart;
				if (chart != null)
					result.AddRange(chart.GetAllReferencedRanges(page.Sheet));
			}
			if (result.Count > 0)
				return result;
			if (!control.InnerControl.IsInplaceEditorActive)
				return null;
			InnerCellInplaceEditor editor = control.InnerControl.InplaceEditor;
			return editor.ReferencedRanges;
		}
		void DrawPivotTableItems(Page page) {
			View.PivotTableLayout.Update(page);
			PageSpreadsheetSelectionPainter selectionPainter = CreatePageSelectionPainter(cache);
			selectionPainter.Initialize(DocumentModel.LayoutUnitConverter);
			selectionPainter.DrawHotZones(View.PivotTableLayout.HotZones);
		}
		void DrawFormulaRanges(Page page) {
			if (!Control.Enabled)
				return;
			FormulaReferencedRanges referencedRanges = GetReferencedRanges(page);
			if (referencedRanges == null || referencedRanges.Count <= 0)
				return;
			formulaRangesPainter.Draw(cache, page, referencedRanges);
		}
		void DrawHeaderPage(HeaderPage page) {
			if (page == null || !page.IsValid())
				return;
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawHeaderPageClipped(page);
			};
			DrawHeaderPageCore(cache, page, draw);
		}
		void DrawGroupPage(GroupItemsPage page) {
			if (page == null || !page.IsValid())
				return;
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawGroupPageClipped(page);
			};
			DrawGroupPageCore(cache, page, draw);
		}
		protected internal virtual void DrawHeaderPageCore(GraphicsCache cache, HeaderPage page, DrawAtPageDelegate draw) {
			Rectangle pageViewInfoClientBounds = page.Bounds;
			Graphics gr = cache.Graphics;
			Matrix oldTransform = gr.Transform.Clone();
			try {
				gr.ResetTransform();
				gr.TranslateTransform(pageViewInfoClientBounds.X, pageViewInfoClientBounds.Y);
				gr.MultiplyTransform(oldTransform);
				float zoomFactor = View.ZoomFactor;
				gr.ScaleTransform(zoomFactor, zoomFactor);
				using (HdcZoomModifier zoomModifier = new HdcZoomModifier(gr, zoomFactor)) {
					Point origin = pageViewInfoClientBounds.Location;
					using (HdcOriginModifier originModifier = new HdcOriginModifier(gr, origin, zoomFactor, HdcOriginModifier.Mode.Combine)) {
						draw(cache);
					}
				}
			}
			finally {
				gr.Transform = oldTransform.Clone();
			}
		}
		protected internal virtual void DrawGroupPageCore(GraphicsCache cache, GroupItemsPage page, DrawAtPageDelegate draw) {
			Rectangle pageViewInfoClientBounds = page.Bounds;
			Graphics gr = cache.Graphics;
			Matrix oldTransform = gr.Transform.Clone();
			try {
				gr.ResetTransform();
				gr.TranslateTransform(pageViewInfoClientBounds.X, pageViewInfoClientBounds.Y);
				gr.MultiplyTransform(oldTransform);
				float zoomFactor = View.ZoomFactor;
				gr.ScaleTransform(zoomFactor, zoomFactor);
				using (HdcZoomModifier zoomModifier = new HdcZoomModifier(gr, zoomFactor)) {
					Point origin = pageViewInfoClientBounds.Location;
					using (HdcOriginModifier originModifier = new HdcOriginModifier(gr, origin, zoomFactor, HdcOriginModifier.Mode.Combine)) {
						draw(cache);
					}
				}
			}
			finally {
				gr.Transform = oldTransform.Clone();
			}
		}
		void DrawHeaderPageClipped(HeaderPage page) {
			using (GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {
				try {
					cache.ClipInfo.SetClip(page.Bounds);
					DrawHeaderPageContent(page);
				}
				finally {
					cache.ClipInfo.RestoreClip(clipState);
				}
			}
		}
		void DrawGroupPageClipped(GroupItemsPage page) {
			using (GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {
				try {
					cache.ClipInfo.SetClip(page.Bounds);
					DrawGroupPageContent(page);
				}
				finally {
					cache.ClipInfo.RestoreClip(clipState);
				}
			}
		}
		void DrawHeaderPageContent(HeaderPage page) {
			headerPainter.Draw(cache, page, DocumentModel.LayoutUnitConverter);
		}
		void DrawGroupPageContent(GroupItemsPage page) {
			groupPainter.Draw(cache, page, DocumentModel.LayoutUnitConverter);
		}
		void DrawFrozenPanesSeparator() {
			if (DocumentLayout.Pages.Count <= 1)
				return;
			DrawFrozenPanesSeparatorCore();
		}
		void DrawFrozenPanesSeparatorCore() {
			Rectangle clientBounds = Control.ClientBounds;
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				frozenPanesSeparatorPainter.Draw(cache, clientBounds, DocumentLayout.Pages[0], View.ZoomFactor);
			};
			DrawAtPageCore(cache, clientBounds, draw);
		}
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
		static int CalculateAlignmentHash(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			return ((int)horizontalAlignment) << 3 | (int)verticalAlignment | ((wrapText ? 0 : 1) << 6);
		}
		static StringFormat CreateStringFormat(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip;
			result.FormatFlags &= ~StringFormatFlags.LineLimit;
			result.Alignment = ConvertHorizontalAlignment(horizontalAlignment);
			result.LineAlignment = ConvertVerticalAlignment(verticalAlignment);
			if (!wrapText)
				result.FormatFlags |= StringFormatFlags.NoWrap;
			else
				result.FormatFlags &= ~StringFormatFlags.NoWrap;
			return result;
		}
		static StringAlignment ConvertHorizontalAlignment(XlHorizontalAlignment horizontalAlignment) {
			switch (horizontalAlignment) {
				default:
				case XlHorizontalAlignment.General:
				case XlHorizontalAlignment.Fill:
				case XlHorizontalAlignment.Distributed:
				case XlHorizontalAlignment.Justify:
				case XlHorizontalAlignment.Left:
					return StringAlignment.Near;
				case XlHorizontalAlignment.CenterContinuous:
				case XlHorizontalAlignment.Center:
					return StringAlignment.Center;
				case XlHorizontalAlignment.Right:
					return StringAlignment.Far;
			}
		}
		static StringAlignment ConvertVerticalAlignment(XlVerticalAlignment verticalAlignment) {
			switch (verticalAlignment) {
				default:
				case XlVerticalAlignment.Justify:
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Bottom:
					return StringAlignment.Far;
				case XlVerticalAlignment.Center:
					return StringAlignment.Center;
				case XlVerticalAlignment.Top:
					return StringAlignment.Near;
			}
		}
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			cache.Graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public void DrawLines(Pen pen, PointF[] points) {
			cache.Graphics.DrawLines(pen, points);
		}
		public Brush GetBrush(Color color) {
			return cache.GetSolidBrush(color);
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
		public virtual void DrawReversibleHorizontalLineAtPage(GraphicsCache cache, int y, Rectangle bounds) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				bounds.Y = y;
				bounds.Height = 0;
				DrawReversibleHorizontalLine(c, bounds);
			};
			DrawAtPageCore(cache, bounds, draw);
		}
		public virtual void DrawReversibleVerticalLineAtPage(GraphicsCache cache, int x, Rectangle bounds) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				bounds.X = x;
				bounds.Width = 0;
				DrawReversibleVerticalLine(c, bounds);
			};
			DrawAtPageCore(cache, bounds, draw);
		}
		public virtual void DrawReversibleFrameAtPage(GraphicsCache cache, Rectangle bounds) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawReversibleFrame(c, bounds);
			};
			DrawAtPageCore(cache, Control.ClientBounds, draw);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleHorizontalLine(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleHorizontalLine(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleHorizontalLine(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleHorizontalLineCore);
		}
		protected internal virtual void DrawReversibleHorizontalLineCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Top);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleVerticalLine(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleVerticalLine(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleVerticalLine(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleVerticalLineCore);
		}
		protected internal virtual void DrawReversibleVerticalLineCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Left, bounds.Bottom);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleFrame(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleFrame(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleFrame(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleFrameCore);
		}
		protected internal virtual void DrawReversibleFrameCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Bottom);
			Win32.LineTo(hdc, bounds.Left, bounds.Bottom);
			Win32.LineTo(hdc, bounds.Left, bounds.Top);
		}
		protected internal void DrawReversible(IntPtr hdc, Rectangle bounds, DrawReversibleDelegate draw) {
			Win32.BinaryRasterOperation oldRop2 = Win32.SetROP2(hdc, Win32.BinaryRasterOperation.R2_NOTXORPEN);
			try {
				IntPtr pen = Win32.CreatePen(Win32.PenStyle.PS_DOT, 0, 0x0);
				IntPtr oldPen = Win32.SelectObject(hdc, pen);
				try {
					IntPtr brush = Win32.GetStockObject(Win32.StockObject.NULL_BRUSH);
					IntPtr oldBrush = Win32.SelectObject(hdc, brush);
					try {
						draw(hdc, bounds);
					}
					finally {
						Win32.SelectObject(hdc, oldBrush);
					}
				}
				finally {
					Win32.SelectObject(hdc, oldPen);
					Win32.DeleteObject(pen);
				}
			}
			finally {
				Win32.SetROP2(hdc, oldRop2);
			}
		}
		protected internal delegate void DrawReversibleDelegate(IntPtr hdc, Rectangle bounds);
		protected internal abstract Color GetDefaultGridlineColor();
		protected internal abstract Color GetDefaultForeColor();
		protected internal abstract Color GetDefaultBackColor();
		protected internal abstract PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache);
		protected internal abstract SpreadsheetHeaderPainter CreateHeaderPainter();
		protected internal abstract FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter();
		protected internal abstract SpreadsheetCommentPainter CreateCommentPainter();
		protected internal abstract SpreadsheetGroupItemsPainter CreateGroupPainter();
	}
	#endregion
	#region NormalViewPainter (abstract class)
	public abstract class NormalViewPainter : SpreadsheetViewPainter {
		protected NormalViewPainter(NormalView view)
			: base(view) {
		}
		protected internal override Color GetDefaultGridlineColor() {
			return Color.Empty;
		}
		protected internal override Color GetDefaultForeColor() {
			if (Control.Enabled)
				return SystemColors.WindowText;
			else
				return SystemColors.GrayText;
		}
		protected internal override Color GetDefaultBackColor() {
			if (Control.Enabled)
				return SystemColors.Window;
			else
				return SystemColors.Control;
		}
	}
	#endregion
	#region NormalViewFlatPainter
	public class NormalViewFlatPainter : NormalViewPainter {
		public NormalViewFlatPainter(NormalView view)
			: base(view) {
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionFlatPainter(cache);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderFlatPainter painter = new SpreadsheetHeaderFlatPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorFlatPainter painter = new FrozenPanesSeparatorFlatPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentFlatPainter(Control);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsFlatPainter(Control);
		}
	}
	#endregion
	#region NormalViewUltraFlatPainter
	public class NormalViewUltraFlatPainter : NormalViewPainter {
		public NormalViewUltraFlatPainter(NormalView view)
			: base(view) {
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionUltraFlatPainter(cache);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderUltraFlatPainter painter = new SpreadsheetHeaderUltraFlatPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorUltraFlatPainter painter = new FrozenPanesSeparatorUltraFlatPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentUltraFlatPainter(Control);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsUltraFlatPainter(Control);
		}
	}
	#endregion
	#region NormalViewStyle3DPainter
	public class NormalViewStyle3DPainter : NormalViewPainter {
		public NormalViewStyle3DPainter(NormalView view)
			: base(view) {
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionStyle3DPainter(cache);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderStyle3DPainter painter = new SpreadsheetHeaderStyle3DPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorStyle3DPainter painter = new FrozenPanesSeparatorStyle3DPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentStyle3DPainter(Control);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsStyle3DPainter(Control);
		}
	}
	#endregion
	#region NormalViewOffice2003Painter
	public class NormalViewOffice2003Painter : NormalViewPainter {
		public NormalViewOffice2003Painter(NormalView view)
			: base(view) {
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionOffice2003Painter(cache);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderOffice2003Painter painter = new SpreadsheetHeaderOffice2003Painter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorOffice2003Painter painter = new FrozenPanesSeparatorOffice2003Painter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentOffice2003Painter(Control);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsOffice2003Painter(Control);
		}
	}
	#endregion
	#region NormalViewWindowsXPPainter
	public class NormalViewWindowsXPPainter : NormalViewPainter {
		public NormalViewWindowsXPPainter(NormalView view)
			: base(view) {
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionWindowsXPPainter(cache);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderWindowsXPPainter painter = new SpreadsheetHeaderWindowsXPPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorWindowsXPPainter painter = new FrozenPanesSeparatorWindowsXPPainter(Control);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentWindowsXPPainter(Control);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsWindowsXPPainter(Control);
		}
	}
	#endregion
	#region NormalViewSkinPainter
	public class NormalViewSkinPainter : NormalViewPainter {
		public NormalViewSkinPainter(NormalView view)
			: base(view) {
		}
		protected internal override Color GetDefaultGridlineColor() {
			return GridSkins.GetSkin(LookAndFeel)[GridSkins.SkinGridLine].Color.GetBackColor();
		}
		protected internal override Color GetDefaultForeColor() {
			if (Control.Enabled)
				return GridSkins.GetSkin(LookAndFeel)[GridSkins.SkinGridLine].Color.Owner.GetSystemColor(SystemColors.WindowText);
			else
				return CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.DisabledText];
		}
		protected internal override Color GetDefaultBackColor() {
			if (Control.Enabled)
				return GridSkins.GetSkin(LookAndFeel)[GridSkins.SkinGridLine].Color.Owner.GetSystemColor(SystemColors.Window);
			else
				return CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.DisabledControl];
		}
		protected internal override PageSpreadsheetSelectionPainter CreatePageSelectionPainter(GraphicsCache cache) {
			return new PageSpreadsheetSelectionSkinPainter(cache, LookAndFeel);
		}
		protected internal override SpreadsheetHeaderPainter CreateHeaderPainter() {
			SpreadsheetHeaderSkinPainter painter = new SpreadsheetHeaderSkinPainter(Control, LookAndFeel);
			painter.Initialize();
			return painter;
		}
		protected internal override FrozenPanesSeparatorPainter CreateFrozenPanesSeparatorPainter() {
			FrozenPanesSeparatorSkinPainter painter = new FrozenPanesSeparatorSkinPainter(Control, LookAndFeel);
			painter.Initialize();
			return painter;
		}
		protected internal override SpreadsheetCommentPainter CreateCommentPainter() {
			return new SpreadsheetCommentSkinPainter(Control, LookAndFeel);
		}
		protected internal override SpreadsheetGroupItemsPainter CreateGroupPainter() {
			return new SpreadsheetGroupItemsSkinPainter(Control, LookAndFeel);
		}
	}
	#endregion
	#region SpreadsheetViewPainterFactory (abstract class)
	public abstract class SpreadsheetViewPainterFactory : ISpreadsheetViewVisitor {
		SpreadsheetViewPainter painter;
		protected SpreadsheetViewPainter Painter { get { return painter; } set { painter = value; } }
		public SpreadsheetViewPainter CreatePainter(SpreadsheetView view) {
			Painter = null;
			view.Visit(this);
			return Painter;
		}
		public abstract void Visit(NormalView view);
	}
	#endregion
	#region SpreadsheetViewPainterFlatFactory
	public class SpreadsheetViewPainterFlatFactory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewFlatPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewPainterUltraFlatFactory
	public class SpreadsheetViewPainterUltraFlatFactory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewUltraFlatPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewPainterStyle3DFactory
	public class SpreadsheetViewPainterStyle3DFactory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewStyle3DPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewPainterOffice2003Factory
	public class SpreadsheetViewPainterOffice2003Factory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewOffice2003Painter(view);
		}
	}
	#endregion
	#region SpreadsheetViewPainterWindowsXPFactory
	public class SpreadsheetViewPainterWindowsXPFactory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewWindowsXPPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewPainterSkinFactory
	public class SpreadsheetViewPainterSkinFactory : SpreadsheetViewPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewSkinPainter(view);
		}
	}
	#endregion
}
