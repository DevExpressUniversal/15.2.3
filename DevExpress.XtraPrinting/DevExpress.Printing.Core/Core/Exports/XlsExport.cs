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
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraExport;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Export.XLS {
	public abstract class XlsExportProviderBase : IXlsExportProvider, IDisposable {
		XlsExportContext exportContext;
		protected ObjectInfo currentObjectInfo;
		protected IExportXlsProvider exportXlsProvider;
		public XlsExportProviderBase(Stream stream, XlsExportContext exportContext) {
			exportXlsProvider = CreateExportXlsProvider(stream, exportContext);
			this.exportContext = exportContext;
		}
		public virtual void Dispose() {
			if(exportXlsProvider != null) {
				exportXlsProvider.Dispose();
				exportXlsProvider = null;
			}
		}
		#region properties
		ExportContext ITableExportProvider.ExportContext { get { return exportContext; } }
		public XlsExportContext XlsExportContext { get { return exportContext; } }
		protected int CurrentRowIndex { get { return currentObjectInfo.RowIndex; } }
		protected int CurrentColIndex { get { return currentObjectInfo.ColIndex; } }
		protected int CurrentRowSpan { get { return currentObjectInfo.RowSpan; } }
		protected int CurrentColSpan { get { return currentObjectInfo.ColSpan; } }
		public BrickViewData CurrentData { get { return (BrickViewData)currentObjectInfo.Object; } }
		#endregion
		void IXlsExportProvider.SetCellData(object data) {
			exportXlsProvider.SetCellData(CurrentColIndex, CurrentRowIndex, data);
		}
		#region ITableExportProvider
		void ITableExportProvider.SetCellText(object textValue, string hyperLink) {
			TextExportMode mode = TextBrickExporter.ToTextExportMode(CurrentData.TableCell.XlsExportNativeFormat, this.exportContext.XlsExportOptions.TextExportMode);
			((IXlsExportProvider)this).SetCellData(HotkeyPrefixHelper.PreprocessHotkeyPrefixesInObject(textValue, CurrentData, mode));
		}
		void ITableExportProvider.SetCellImage(Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imageSize, PaddingInfo padding, string hyperlink) {
			if (image != null)
				exportXlsProvider.SetCellImage(
					new TwoCellAnchorInfo(
						new Point(CurrentColIndex, CurrentRowIndex),
						GetStartCellOffset(CurrentData),
						new Point(CurrentColIndex + CurrentColSpan, CurrentRowIndex + CurrentRowSpan),
						GetEndCellOffset(CurrentData)), 
					image,
					GetInnerSize(CurrentData),
					padding,
					hyperlink);
		}
		void ITableExportProvider.SetCellShape(Color lineColor, DevExpress.XtraReports.UI.LineDirection lineDirection, DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink) {
			PointF startCellOffset = GetStartCellOffset(CurrentData);
			PointF endCellOffset = GetEndCellOffset(CurrentData);
			exportXlsProvider.SetCellShape(
					new TwoCellAnchorInfo(
						new Point(CurrentColIndex, CurrentRowIndex),
						new PointF(startCellOffset.X + padding.Left, startCellOffset.Y + padding.Top),
						new Point(CurrentColIndex + CurrentColSpan, CurrentRowIndex + CurrentRowSpan),
						new PointF(endCellOffset.X + padding.Right, endCellOffset.Y + padding.Bottom)),
					lineColor,
					lineDirection,
					lineStyle,
					lineWidth,
					hyperlink);
		}
		#endregion
		ExportCacheCellStyle GetXlsStyle(BrickViewData brickViewData) {
			System.Diagnostics.Debug.Assert(brickViewData.Style != null);
			BrickStyle brickStyle = brickViewData.Style;
			ExportCacheCellStyle style = new ExportCacheCellStyle();
			if(!exportContext.RawDataMode) {
				style.BrushStyle_ = brickStyle.BackColor == Color.Transparent ? BrushStyle.Clear : BrushStyle.Solid;
				style.TextColor = DXColor.Blend(brickStyle.ForeColor, Color.White);
				style.BkColor = DXColor.Blend(brickStyle.BackColor, Color.White);
			}
			style.WrapText = brickStyle.StringFormat.WordWrap;
			style.RightToLeft = brickStyle.StringFormat.RightToLeft;
			style.TextFont = brickStyle.Font;
			style.TextAlignment = GraphicsConvertHelper.ToHorzStringAlignment(brickStyle.TextAlignment);
			style.LineAlignment = GraphicsConvertHelper.ToVertStringAlignment(brickStyle.TextAlignment);
			TextExportMode textExportMode = TextBrickExporter.ToTextExportMode(brickViewData.TableCell.XlsExportNativeFormat, exportContext.XlsExportOptions.TextExportMode);
			if(textExportMode == TextExportMode.Value) {
				style.FormatString = brickViewData.TableCell.FormatString;
				style.XlsxFormatString = brickViewData.TableCell.XlsxFormatString;
				style.PreparedCellType = (short)GetPreparedCellType(brickViewData.TableCell.TextValue);
			} else
				style.PreparedCellType = (short)XlsConsts.RealFormat;
			if(!exportContext.RawDataMode) {
				ExportCacheCellBorderStyle borderStyle = CreateBorderStyle(brickStyle);
				if((BorderSide.Left & brickStyle.Sides) != 0)
					style.LeftBorder = borderStyle;
				if((BorderSide.Right & brickStyle.Sides) != 0)
					style.RightBorder = borderStyle;
				if((BorderSide.Top & brickStyle.Sides) != 0)
					style.TopBorder = borderStyle;
				if((BorderSide.Bottom & brickStyle.Sides) != 0)
					style.BottomBorder = borderStyle;
			}
			return style;
		}
#if DEBUGTEST
		internal ITableCell Test_GetXlsTableCell(BrickViewData brickViewData) {
			return brickViewData.TableCell;
		}
#endif
		ushort GetPreparedCellType(object value) {
			if(value is DateTime || value is TimeSpan)
				return XlsConsts.DateTimeFormat;
			if(value is string)
				return ((string)value).Length <= 256 ? XlsConsts.RealFormat : XlsConsts.GeneralFormat;
			return XlsConsts.GeneralFormat;
		}
		ExportCacheCellBorderStyle CreateBorderStyle(BrickStyle style) {
			ExportCacheCellBorderStyle borderStyle = new ExportCacheCellBorderStyle();
			borderStyle.Width = Convert.ToInt32(style.BorderWidth);
			borderStyle.BorderDashStyle = style.BorderDashStyle;
			borderStyle.Color_ = GetActualColor(style.BorderColor, style.BackColor, Color.White);
			return borderStyle;
		}
		static Color GetActualColor(params Color[] colors) {
			for(int i = 0; i < colors.Length - 1; i++)
				if(!colors[i].IsEmpty) return colors[i];
			return colors[colors.Length - 1];
		}
		protected virtual PointF GetStartCellOffset(BrickViewData data) {
			BrickStyle style = data.Style;
			PointF result = new PointF();
			result.X = style != null && (style.Sides & BorderSide.Left) > 0 && data.Width > 0 ? (style.BorderWidth / 2f) / data.Width : 0;
			result.Y = style != null && (style.Sides & BorderSide.Top) > 0 && data.Height > 0 ? (style.BorderWidth / 2f) / data.Height : 0;
			return result;
		}
		protected virtual PointF GetEndCellOffset(BrickViewData data) {
			BrickStyle style = data.Style;
			PointF result = new PointF();
			result.X = style != null && (style.Sides & BorderSide.Right) > 0 && data.Width > 0 ? (style.BorderWidth / 2f) / data.Width : 0;
			result.Y = style != null && (style.Sides & BorderSide.Bottom) > 0 && data.Height > 0 ? (style.BorderWidth / 2f) / data.Height : 0;
			return result;
		}
		protected virtual SizeF GetInnerSize(BrickViewData data) {
			return SizeF.Empty;
		}
		protected abstract IExportXlsProvider CreateExportXlsProvider(Stream stream, XlsExportContext exportContext);
		protected MegaTable CreateMegaTable(LayoutControlCollection layoutControls, bool correctImportBrickBounds) {
			MegaTable megaTable = new MegaTable(layoutControls, !XlsExportContext.RawDataMode, correctImportBrickBounds);
			ValidateMegaTableCore(megaTable, exportContext);
			SetRange(megaTable.ColWidths, megaTable.RowHeights,  exportContext.XlsExportOptions.ShowGridLines);
			ReadonlyPageData pageData = exportContext.XlsExportOptions.IsMultiplePaged ? exportContext.DrawingPage.PageData : exportContext.PrintingSystem.PageSettings.Data;
			MarginsF marginsF = GraphicsUnitConverter.Convert(pageData.MarginsF, GraphicsDpi.Document, GraphicsDpi.Inch);
			exportXlsProvider.SetPageSettings(marginsF, pageData.PaperKind, pageData.Landscape);
			exportXlsProvider.SetAdditionalSettings(exportContext.XlsExportOptions.FitToPrintedPageWidth, false, exportContext.XlsExportOptions.RightToLeftDocument);
			return megaTable;
		}
		protected virtual void SetCurrentSheetTable(MegaTable megaTable) {
			for(int i = 0; i < megaTable.Objects.Length; i++) {
				if(XlsExportContext.CancelPending) break;
				SetCurrentObjectInfo(megaTable.Objects[i]);
				exportContext.ProgressReflector.RangeValue++;
			}
		}
		protected void SetCurrentObjectInfo(ObjectInfo objectInfo) {
			BrickViewData data = (BrickViewData)objectInfo.Object;
			currentObjectInfo = objectInfo;
			((BrickExporter)this.exportContext.PrintingSystem.ExportersFactory.GetExporter(data.TableCell)).FillXlsTableCell(this);
			SetCellStyle(objectInfo.ColIndex, objectInfo.RowIndex, data);
			SetCellUnion(objectInfo.ColIndex, objectInfo.RowIndex, objectInfo.ColSpan, objectInfo.RowSpan);
			if(exportContext.XlsExportOptions.ExportHyperlinks)
				 SetCellUrlAndAnchor(objectInfo.ColIndex, objectInfo.RowIndex, data.TableCell);
		}
		protected abstract void ValidateMegaTableCore(MegaTable megaTable, XlsExportContext exportContext);
		protected void CreateDocument(MegaTable megaTable) {
			SetCurrentSheetTable(megaTable);
			Commit();
		}
		public virtual void CreateDocument(Document document) {
			LayoutControlCollection layoutControls = new XlsLayoutBuilder(document, XlsExportContext).BuildLayoutControls();
			bool correctImportBrickBounds = document.CorrectImportBrickBounds;
			MegaTable megaTable = CreateMegaTable(layoutControls, correctImportBrickBounds);
			try {
				exportContext.ProgressReflector.InitializeRange(megaTable.Objects.Length);
				CreateDocument(megaTable);
			} finally {
				exportContext.ProgressReflector.MaximizeRange();
			}
		}
		public void SetRange(List<int> colWidths, List<int> rowHeights,  bool showGridLines) {
			exportXlsProvider.SetRange(colWidths.Count, rowHeights.Count, showGridLines);
			for(int i = 0; i < rowHeights.Count; i++) {
				exportXlsProvider.SetRowHeight(i, rowHeights[i]);
			}
			for(int i = 0; i < colWidths.Count; i++) {
				exportXlsProvider.SetColumnWidth(i, colWidths[i]);
			}
		}
		public void SetCellStyle(int columnIndex, int rowIndex, BrickViewData brickViewData) {
			if(brickViewData.Style != null)
				exportXlsProvider.SetCellStyle(columnIndex, rowIndex, GetXlsStyle(brickViewData));
		}
		public void SetCellUrlAndAnchor(int columnIndex, int rowIndex, ITableCell tableCell) {
			exportXlsProvider.SetCellUrlAndAnchor(columnIndex, rowIndex, tableCell);
		}
		public void SetCellUnion(int columnIndex, int rowIndex, int colSpan, int rowSpan) {
			exportXlsProvider.SetCellUnion(columnIndex, rowIndex, colSpan, rowSpan);
		}
		public void Commit() {
			exportXlsProvider.Commit();
		}
	}
	public class XlsExportProvider : XlsExportProviderBase {
		const int maxRowCount = 65536;
		const Int16 maxColumnCount = 256;
		public XlsExportProvider(Stream stream, XlsExportContext exportContext) : base(stream, exportContext) {
		}
		protected override void ValidateMegaTableCore(MegaTable megaTable, XlsExportContext exportContext) {
			ValidateMegaTable(megaTable, exportContext);
		}
		protected virtual string GetFinalSheetName(XlsExportContext exportContext) {
			return exportContext.XlsExportOptions.SheetName;
		}
		protected override IExportXlsProvider CreateExportXlsProvider(Stream stream, XlsExportContext exportContext) {
			return new ExportXlsProviderInternal(stream, GetFinalSheetName(exportContext), ((XlsExportOptions)exportContext.XlsExportOptions).WorkbookColorPaletteCompliance);
		}
		internal static void ValidateMegaTable(MegaTable megaTable, XlsExportContext exportContext) {
			System.Diagnostics.Debug.Assert(exportContext.XlsExportOptions is XlsExportOptions);
			XlsExportOptions options = (XlsExportOptions)exportContext.XlsExportOptions;
			if(megaTable.RowCount > maxRowCount && !options.Suppress65536RowsWarning) {
				throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsMoreThanMaxRows));
			}
			if(megaTable.ColumnCount > maxColumnCount && !options.Suppress256ColumnsWarning) {
				throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsMoreThanMaxColumns));
			}
		}
	}
	public class XlsFilePageExportProvider : XlsExportProvider {
		public XlsFilePageExportProvider(Stream stream, XlsExportContext exportContext)
			: base(stream, exportContext) {
		}
		protected override string GetFinalSheetName(XlsExportContext exportContext) {
			return exportContext.XlsExportOptions.SheetName + exportContext.XlsExportOptions.PageRange;
		}
		public override void CreateDocument(Document document) {
			XlsPageLayoutBuilder layoutBuilder = new XlsPageLayoutBuilder(document.Pages[int.Parse(XlsExportContext.XlsExportOptions.PageRange) - 1], XlsExportContext);
			LayoutControlCollection layoutControls = layoutBuilder.BuildLayoutControls();
			bool correctImportBrickBounds = document.CorrectImportBrickBounds;
			CreateDocument(CreateMegaTable(layoutControls, correctImportBrickBounds));
			layoutBuilder.Dispose();
		}
		protected override void SetCurrentSheetTable(MegaTable megaTable) {
			for(int i = 0; i < megaTable.Objects.Length; i++) {
				if(XlsExportContext.CancelPending) break;
				SetCurrentObjectInfo(megaTable.Objects[i]);
			}
		}
	}
	public class XlsxExportProvider : XlsExportProviderBase {		
		const int maxRowCount = 1048576;
		const Int16 maxColumnCount = 16384;
		public XlsxExportProvider(Stream stream, XlsExportContext exportContext)
			: base(stream, exportContext) {
		}
		public override void CreateDocument(Document document) {
			CreateSheet(XlsExportContext.XlsExportOptions.SheetName);
			base.CreateDocument(document);
		}
		protected void CreateSheet(string sheetName) {
			((ExportXlsxProvider)exportXlsProvider).CreateSheet(sheetName);
		}
		protected override void ValidateMegaTableCore(MegaTable megaTable, XlsExportContext exportContext) {
			if(megaTable.RowCount > maxRowCount) {
				throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsxMoreThanMaxRows));
			}
			if(megaTable.ColumnCount > maxColumnCount) {
				throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsxMoreThanMaxColumns));
			}
		}
		protected override IExportXlsProvider CreateExportXlsProvider(Stream stream, XlsExportContext exportContext) {
			return new ExportXlsxProvider(stream, exportContext.XlsExportOptions.SheetName);
		}
		protected override PointF GetStartCellOffset(BrickViewData data) {
			return new PointF(GetBorderSideWidth(data, BorderSide.Left),GetBorderSideWidth(data, BorderSide.Top)) ;
		}
		protected override PointF GetEndCellOffset(BrickViewData data) {
			return new PointF(GetBorderSideWidth(data, BorderSide.Right), GetBorderSideWidth(data, BorderSide.Bottom));
		}
		protected override SizeF GetInnerSize(BrickViewData data) {
			float left = GetBorderSideWidth(data, BorderSide.Left);
			float top = GetBorderSideWidth(data, BorderSide.Top);
			float right = GetBorderSideWidth(data, BorderSide.Right);
			float bottom = GetBorderSideWidth(data, BorderSide.Bottom);
			float width = Math.Max(0, data.BoundsF.Width - left - right);
			float height = Math.Max(0, data.BoundsF.Height - top - bottom);
			return new SizeF(width, height);
		}
		float GetBorderSideWidth(BrickViewData data, BorderSide side) {
			BrickStyle style = data.Style;
			return style != null &&
				(style.Sides & side) > 0 &&
				data.Width > 0 &&
				style.BorderWidth > 0 ?
				BorderSideNode.GetBorderWidth(Convert.ToInt32(style.BorderWidth), style.BorderDashStyle) : 0;
		}
	}
	public class XlsxPageExportProvider : XlsxExportProvider {
		public XlsxPageExportProvider(Stream stream, XlsExportContext exportContext)
			: base(stream, exportContext) {
		}
		public override void CreateDocument(Document document) {
			int[] pageIndices = ExportOptionsHelper.GetPageIndices(XlsExportContext.XlsExportOptions, document.PageCount);
			if(pageIndices.Length > 1) {
				XlsExportContext.ProgressReflector.SetProgressRanges(new float[] { 1 });
				XlsExportContext.ProgressReflector.InitializeRange(pageIndices.Length);
			}
			try {
				foreach(int index in pageIndices) {
					CreateSheet(XlsExportContext.XlsExportOptions.SheetName + (index + 1).ToString());
					XlsPageLayoutBuilder layoutBuilder = new XlsPageLayoutBuilder(document.Pages[index], XlsExportContext);
					MegaTable megaTable = CreateMegaTable(layoutBuilder.BuildLayoutControls(), document.CorrectImportBrickBounds);
					try {
						SetCurrentSheetTable(megaTable);
					} finally {
						XlsExportContext.ProgressReflector.RangeValue++;
						layoutBuilder.Dispose();
					}
				}
				UpdateSheetNames();
				Commit();
			} finally {
				if(pageIndices.Length > 1)
					XlsExportContext.ProgressReflector.MaximizeRange();
			}
		}
		protected override void SetCurrentSheetTable(MegaTable megaTable) {
			for(int i = 0; i < megaTable.Objects.Length; i++) {
				if(XlsExportContext.CancelPending) break;
				SetCurrentObjectInfo(megaTable.Objects[i]);
			}
		}
		void UpdateSheetNames() {
			if(exportXlsProvider is ExportXlsxProvider) { 
				SheetNode[] sheetNodes = ((ExportXlsxProvider)exportXlsProvider).WorkbookDocument.SheetsNode.Cast<SheetNode>().ToArray();
				string[] sheetNames = sheetNodes.Select<SheetNode, string>(x => x.SheetName).ToArray();
				((ITableExportProvider)this).ExportContext.PrintingSystem.OnXlsxDocumentCreated(this, new XlsxDocumentCreatedEventArgs(sheetNames));
				for(int index = 0; index < sheetNodes.Length; index++)
					sheetNodes[index].SheetName = sheetNames[index];
			}
		}
	}
}
