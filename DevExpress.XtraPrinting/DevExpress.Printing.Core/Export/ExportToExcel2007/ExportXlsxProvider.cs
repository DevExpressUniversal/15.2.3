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
using System.Text;
using DevExpress.XtraExport;
using DevExpress.Utils.Zip;
using System.IO;
using System.Globalization;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.HtmlExport.Native;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraReports.UI;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraExport {
	public class ExportXlsxProvider : ExportExcelProvider, IExportXlsProvider {
		XlsxPackage package;
		string sheetName;
		internal WorkbookDocument WorkbookDocument { get { return package.WorkbookDocument; } }
		public ExportXlsxProvider(string fileName)
			: base(fileName) {
			package = new XlsxPackage(fileName);
		}
		public ExportXlsxProvider(Stream stream, string sheetName)
			: base(stream, sheetName, WorkbookColorPaletteCompliance.ReducePaletteForExactColors) {
			package = new XlsxPackage(stream);
		}
		public ExportXlsxProvider(Stream stream)
			: this(stream, string.Empty) {
		}
		public void CreateSheet(string sheetName) {
			package.CreateSheet(sheetName);
		}
		protected override ExportExcelProvider CreateInstance(string fileName) {
			return new ExportXlsxProvider(fileName);
		}
		protected override ExportExcelProvider CreateInstance(Stream stream) {
			return new ExportXlsxProvider(stream);
		}
		protected override ExportStyleManagerBase CreateExportStyleManager(string fileName, Stream stream) {
			return new ExportXlsxStyleManager(fileName, stream);
		}
		protected override void InitializeSheetName(string sheetName, string defaultSheetName) {
			this.sheetName = string.IsNullOrEmpty(sheetName) ? defaultSheetName : sheetName;
		}
		string AdjustImagePath(string path) { 
			return path.Replace(@"/xl/",@"../");
		}
		void SetStartEndForAnchorCell(TwoCellAnchorNodeBase twoCellAnchorNode, TwoCellAnchorInfo correctAnchorCell) {
			twoCellAnchorNode.FromNode.Col = correctAnchorCell.StartCell.X;
			twoCellAnchorNode.FromNode.Row = correctAnchorCell.StartCell.Y;
			twoCellAnchorNode.FromNode.ColOff = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.StartCellOffset.X, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			twoCellAnchorNode.FromNode.RowOff = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.StartCellOffset.Y, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			twoCellAnchorNode.ToNode.Col = correctAnchorCell.EndCell.X;
			twoCellAnchorNode.ToNode.Row = correctAnchorCell.EndCell.Y;
			twoCellAnchorNode.ToNode.ColOff = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.X, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			twoCellAnchorNode.ToNode.RowOff = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.Y, GraphicsDpi.Pixel, GraphicsDpi.EMU));
		}
		TwoCellAnchorInfo GetCorrectAnchorCell(TwoCellAnchorInfo twoCellAnchorInfo) {
			Tuple<int, float, int, float> x = TwoCellAnchorHelper.GetCorrectCellPart(twoCellAnchorInfo.StartCell.X, twoCellAnchorInfo.StartCellOffset.X, twoCellAnchorInfo.EndCell.X, twoCellAnchorInfo.EndCellOffset.X,
				i => package.WorksheetDocument.WorksheetNode.Cols.GetColNodeWithCache(i).WidthInPixels);
			Tuple<int, float, int, float> y = TwoCellAnchorHelper.GetCorrectCellPart(twoCellAnchorInfo.StartCell.Y, twoCellAnchorInfo.StartCellOffset.Y, twoCellAnchorInfo.EndCell.Y, twoCellAnchorInfo.EndCellOffset.Y,
				i => package.WorksheetDocument.WorksheetNode.SheetDataNode.GetRowNodeWithCache(i).HtInPixels);
			return new TwoCellAnchorInfo(new Point(x.Item1, y.Item1), new PointF(x.Item2, y.Item2), new Point(x.Item3, y.Item3), new PointF(x.Item4, y.Item4));
		}
		#region IExportXlsProvider Members
		void IExportXlsProvider.SetCellShape(TwoCellAnchorInfo twoCellAnchorInfo, Color lineColor, LineDirection lineDirection, DashStyle lineStyle, float lineWidth, string hyperLink) {
			string hyperLinkId = string.Empty;
			if(!string.IsNullOrEmpty(hyperLink) && ExportXlsProviderInternal.HasUrlScheme(hyperLink)) {
				hyperLinkId = package.DrawingRelsDocument.RelationshipsNode.GetRelationshipNodeWithCache(hyperLink, XlsxHelper.HyperlinkTargetType, true).Id;
			}
			TwoCellAnchorLineShapeNode twoCellAnchorNode = package.DrawingDocument.WsDrNode.AppendTwoCellAnchorLineShapeNode();
			twoCellAnchorNode.ShapeNode.NvCxnSpPrNode.Id = package.DrawingDocument.WsDrNode.ChildNodes.Count;
			twoCellAnchorNode.ShapeNode.NvCxnSpPrNode.HyperLinkId = hyperLinkId;
			twoCellAnchorNode.ShapeNode.LnNode.Color = XlsxHelper.ColorToRGBString(lineColor);
			twoCellAnchorNode.ShapeNode.LnNode.Style = XlsxHelper.LineStyleToString(lineStyle);
			twoCellAnchorNode.ShapeNode.LnNode.Width = (int)Math.Round(GraphicsUnitConverter.Convert(lineWidth, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			TwoCellAnchorInfo correctAnchorCell = GetCorrectAnchorCell(twoCellAnchorInfo);
			switch(lineDirection) {
				case LineDirection.Slant:
					twoCellAnchorNode.ShapeNode.SpPrNode.IsVerticalFlip = true;
					break;
				case LineDirection.Horizontal:
					Tuple<int, float> hMiddlePoint = TwoCellAnchorHelper.GetCorrectMiddlePoint(correctAnchorCell.StartCell.Y, correctAnchorCell.StartCellOffset.Y, 
						correctAnchorCell.EndCell.Y, correctAnchorCell.EndCellOffset.Y, i => package.WorksheetDocument.WorksheetNode.SheetDataNode.GetRowNodeWithCache(i).HtInPixels);
					correctAnchorCell = new TwoCellAnchorInfo(new Point(correctAnchorCell.StartCell.X, hMiddlePoint.Item1), new PointF(correctAnchorCell.StartCellOffset.X, hMiddlePoint.Item2),
															  new Point(correctAnchorCell.EndCell.X, hMiddlePoint.Item1), new PointF(correctAnchorCell.EndCellOffset.X, hMiddlePoint.Item2));
					break;
				case LineDirection.Vertical:
					Tuple<int, float> vMiddlePoint = TwoCellAnchorHelper.GetCorrectMiddlePoint(correctAnchorCell.StartCell.X, correctAnchorCell.StartCellOffset.X, 
						correctAnchorCell.EndCell.X, correctAnchorCell.EndCellOffset.X, i => package.WorksheetDocument.WorksheetNode.Cols.GetColNodeWithCache(i).WidthInPixels);
					correctAnchorCell = new TwoCellAnchorInfo(new Point(vMiddlePoint.Item1, correctAnchorCell.StartCell.Y), new PointF(vMiddlePoint.Item2, correctAnchorCell.StartCellOffset.Y),
															  new Point(vMiddlePoint.Item1, correctAnchorCell.EndCell.Y), new PointF(vMiddlePoint.Item2, correctAnchorCell.EndCellOffset.Y));
					break;
				case LineDirection.BackSlant:
				default:
					break;
			}
			SetStartEndForAnchorCell(twoCellAnchorNode, correctAnchorCell);
			twoCellAnchorNode.ShapeNode.SpPrNode.Cx = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.X, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			twoCellAnchorNode.ShapeNode.SpPrNode.Cy = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.Y, GraphicsDpi.Pixel, GraphicsDpi.EMU));
		}
		void IExportXlsProvider.SetCellImage(TwoCellAnchorInfo twoCellAnchorInfo, Image image, SizeF innerSize, PaddingInfo padding, string hyperLink) {
			string path = AdjustImagePath(XlsxHelper.Separator + package.AppendImage(image));
			string id = package.DrawingRelsDocument.RelationshipsNode.GetRelationshipNodeWithCache(path, XlsxHelper.ImageTargetType, false).Id;
			string hyperLinkId = string.Empty;
			if(!string.IsNullOrEmpty(hyperLink) && ExportXlsProviderInternal.HasUrlScheme(hyperLink)) {
				hyperLinkId = package.DrawingRelsDocument.RelationshipsNode.GetRelationshipNodeWithCache(hyperLink, XlsxHelper.HyperlinkTargetType, true).Id;
			}
			TwoCellAnchorPictureNode twoCellAnchorNode = package.DrawingDocument.WsDrNode.AppendTwoCellAnchorNode();
			twoCellAnchorNode.PicNode.NvPicPrNode.Id = package.DrawingDocument.WsDrNode.ChildNodes.Count;
			twoCellAnchorNode.PicNode.NvPicPrNode.HyperLinkId = hyperLinkId;
			twoCellAnchorNode.PicNode.BlipFillNode.Id = id;
			twoCellAnchorNode.PicNode.BlipFillNode.SrcRectNode.L = (decimal)(padding.Left) / image.Size.Width * 100;
			twoCellAnchorNode.PicNode.BlipFillNode.SrcRectNode.R = (decimal)(padding.Right) / image.Size.Width * 100;
			twoCellAnchorNode.PicNode.BlipFillNode.SrcRectNode.T = (decimal)(padding.Top) / image.Size.Height * 100;
			twoCellAnchorNode.PicNode.BlipFillNode.SrcRectNode.B = (decimal)(padding.Bottom) / image.Size.Height * 100;
			TwoCellAnchorInfo correctAnchorCell = GetCorrectAnchorCell(twoCellAnchorInfo);
			SetStartEndForAnchorCell(twoCellAnchorNode, correctAnchorCell);
			twoCellAnchorNode.PicNode.SpPrNode.Cx = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.X, GraphicsDpi.Pixel, GraphicsDpi.EMU));
			twoCellAnchorNode.PicNode.SpPrNode.Cy = (int)Math.Round(GraphicsUnitConverter.Convert(correctAnchorCell.EndCellOffset.Y, GraphicsDpi.Pixel, GraphicsDpi.EMU));
		}
		void IExportXlsProvider.SetCellUrlAndAnchor(int col, int row, ITableCell tableCell) {
			string url = tableCell.Url;
			string anchorName = tableCell is VisualBrick ? (tableCell as VisualBrick).AnchorName : tableCell is AnchorCell ? (tableCell as AnchorCell).AnchorName : string.Empty;
			if(!string.IsNullOrEmpty(url)) {
				if(ExportXlsProviderInternal.HasUrlScheme(url)) {
					HyperlinkNode hyperlinkNode = package.WorksheetDocument.WorksheetNode.HyperlinksNode.AppendHyperlinkNode();
					hyperlinkNode.Col = col + 1;
					hyperlinkNode.Row = row + 1;
					hyperlinkNode.RId = package.WorksheetRelsDocument.RelationshipsNode.GetRelationshipNodeWithCache(DXHttpUtility.UrlEncodeSpaces(url), XlsxHelper.HyperlinkTargetType, true).Id;
				} else {
					FileHyperlinkNode fileHyperlinkNode = package.GetFileHyperlinkByText(url);
					fileHyperlinkNode.Col = col + 1;
					fileHyperlinkNode.Row = row + 1;
				}
			}
			if(!string.IsNullOrEmpty(anchorName)) {
				List<FileHyperlinkNode> fileHyperlinkNodes = package.GetFileHyperlinksByText(anchorName);
				foreach(FileHyperlinkNode fileHyperlinkNode in fileHyperlinkNodes) {
					fileHyperlinkNode.DestCol = col + 1;
					fileHyperlinkNode.DestRow = row + 1;
				}
			}
			if(tableCell is AnchorCell && (tableCell as AnchorCell).InnerCell != null) {
				(this as IExportXlsProvider).SetCellUrlAndAnchor(col, row, (tableCell as AnchorCell).InnerCell);   
			}
		}
		void IExportXlsProvider.SetGroupIndex(int row, int index) {
		}
		void IExportXlsProvider.SetAdditionalSettings(bool fitToPageWidth, bool fitToPageHeight, DevExpress.Utils.DefaultBoolean rightToLeft) {
			package.WorksheetDocument.WorksheetNode.SheetPropertyNode.PageSetUpPr.FitToPage = fitToPageWidth;
			package.WorksheetDocument.WorksheetNode.PageSetupNode.CountFitToPagesWide = 1;
			package.WorksheetDocument.WorksheetNode.PageSetupNode.CountFitToPagesLong = 0;
			package.WorksheetDocument.WorksheetNode.SheetViewsNode.SheetViewNode.RightToLeft = rightToLeft;
		}
		#endregion
		#region IExportProvider Members
		void IExportProvider.Commit() {
			for(int i = 0; i < StyleCache.Count; i++) {
				ExportCacheCellStyle style = StyleCache[i];
				XfNode xfNode = package.StyleSheetDocument.StyleSheetNode.CellXfsNode.AppendXfNode();
				xfNode.AlignmentNode.VerticalAlignment = style.LineAlignment;
				xfNode.AlignmentNode.HorizontalAlignment = style.TextAlignment;
				xfNode.AlignmentNode.WrapText = style.WrapText;
				xfNode.AlignmentNode.ShrinkToFit = style.WrapText;
				xfNode.AlignmentNode.ReadingOrder = style.RightToLeft ? AlignmentNode.ReadingDirection.RightToLeft : AlignmentNode.ReadingDirection.Context;
				xfNode.FontId = package.StyleSheetDocument.StyleSheetNode.FontsNode.AppendFontNode(style);
				xfNode.BorderId = package.StyleSheetDocument.StyleSheetNode.BordersNode.AppendBorderNode(style);
				xfNode.FillId = package.StyleSheetDocument.StyleSheetNode.FillsNode.AppendFillNode(style);
				xfNode.NumFmtId = GetFormatId(style);
			}
			package.StyleSheetDocument.StyleSheetNode.NumFmtsNode.AddStandardFormats(); 
			package.CreateXlsxFile();
		}
		protected override int RegisterFormat(string formatString) {
			return package.StyleSheetDocument.StyleSheetNode.NumFmtsNode.AppendNumFmtNode(formatString);
		}
		int IExportProvider.RegisterStyle(ExportCacheCellStyle style) {
			return StyleCache.RegisterStyle(style);
		}
		void IExportProvider.SetDefaultStyle(ExportCacheCellStyle style) {
			StyleCache.DefaultStyle = style;
		}
		void IExportProvider.SetStyle(ExportCacheCellStyle style) {
			int index = ((IExportProvider)this).RegisterStyle(style);
			((IExportProvider)this).SetStyle(index);
		}
		void IExportProvider.SetStyle(int styleIndex) {
			foreach(XlsxRow rowNode in package.WorksheetDocument.WorksheetNode.SheetDataNode.Rows) {
				foreach(XlsxCell cNode in rowNode.Cells)
					cNode.StyleIndex = styleIndex;
			}
		}
		void IExportProvider.SetCellStyle(int col, int row, int styleIndex) {
			XlsxCell cNode = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(col + 1, row + 1);
			if(cNode == null)
				return;
			cNode.StyleIndex = styleIndex;
		}
		void IExportProvider.SetCellStyle(int col, int row, ExportCacheCellStyle style) {
			int index = ((IExportProvider)this).RegisterStyle(style);
			((IExportProvider)this).SetCellStyle(col, row, index);
		}
		void IExportProvider.SetCellStyle(int col, int row, int exampleCol, int exampleRow) {
			XlsxCell examleCNode = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(exampleCol + 1, exampleRow + 1);
			if(examleCNode == null)
				return;
			XlsxCell cNode = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(col + 1, row + 1);
			if(cNode == null)
				return;
			cNode.StyleIndex = examleCNode.StyleIndex;
		}
		void IExportProvider.SetCellUnion(int col, int row, int width, int height) {
			if(width == 1 && height == 1)
				return;
			for(int i = col; i < col + width; i++) {
				for(int j = row; j < row + height; j++)
					if(i != col || j != row)
						((IExportProvider)this).SetCellStyle(i, j, col, row);
			}
			MergeCellNode mergeCellNode = package.WorksheetDocument.WorksheetNode.MergeCellsNode.AppendMergeCellNode();
			mergeCellNode.Col1 = col + 1;
			mergeCellNode.Row1 = row + 1;
			mergeCellNode.Col2 = col + width;
			mergeCellNode.Row2 = row + height;
		}
		void IExportProvider.SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex) {
			((IExportProvider)this).SetCellUnion(col, row, width, height);
			((IExportProvider)this).SetCellStyle(col, row, styleIndex);
		}
		void IExportProvider.SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style) {
			((IExportProvider)this).SetCellUnion(col, row, width, height);
			((IExportProvider)this).SetCellStyle(col, row, style);
		}
		void IExportProvider.SetRange(int width, int height, bool isVisible) {
			if(package.WorksheetDocument == null)
				package.CreateSheet(this.sheetName);
			package.WorksheetDocument.WorksheetNode.DimensionNode.Col1 = 1;
			package.WorksheetDocument.WorksheetNode.DimensionNode.Row1 = 1;
			package.WorksheetDocument.WorksheetNode.DimensionNode.Col2 = width;
			package.WorksheetDocument.WorksheetNode.DimensionNode.Row2 = height;
			package.WorksheetDocument.WorksheetNode.SheetViewsNode.SheetViewNode.ShowGridLines = isVisible;
		}
		void IExportProvider.SetPageSettings(MarginsF margins, PaperKind paperKind, bool landscape) {
			if(package.WorksheetDocument == null)
				package.CreateSheet(this.sheetName);
			package.WorksheetDocument.WorksheetNode.PageMarginsNode.Margins = margins;
			package.WorksheetDocument.WorksheetNode.PageSetupNode.IsLandscape = landscape;
			package.WorksheetDocument.WorksheetNode.PageSetupNode.PaperSize = (int)paperKind; 
		}
		void IExportProvider.SetColumnWidth(int col, int width) {
			ColNode colNode = package.WorksheetDocument.WorksheetNode.Cols.GetColNodeWithCache(col + 1);
			colNode.WidthInPixels = width;
		}
		void IExportProvider.SetRowHeight(int row, int height) {
			XlsxRow rowNode = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetRowNodeWithCache(row + 1);
			rowNode.HtInPixels = height;
			rowNode.Spans1 = 1;
			rowNode.Spans2 = package.WorksheetDocument.WorksheetNode.DimensionNode.Col2;
		}
		void IExportProvider.SetCellData(int col, int row, object data) {
			if(data is string || data is Enum)
				((IExportProvider)this).SetCellString(col, row, Convert.ToString(data));
			else {
				XlsxCell cell = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(col + 1, row + 1);
				cell.IsSharedString = false;
				cell.ObjectValue = data;
			}
		}
		void IExportProvider.SetCellString(int col, int row, string str) {
				XlsxCell cell = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(col + 1, row + 1);
				cell.IsSharedString = !string.IsNullOrEmpty(str);
				if(cell.IsSharedString) {
					int index = package.SharedStringsDocument.GetStringIndex(str);
					cell.StringIndex = index;
				}
		}
		ExportCacheCellStyle IExportProvider.GetStyle(int styleIndex) {
			return StyleCache[styleIndex];
		}
		ExportCacheCellStyle IExportProvider.GetCellStyle(int col, int row) {
			XlsxCell cNode = package.WorksheetDocument.WorksheetNode.SheetDataNode.GetCNodeWithCache(col + 1, row + 1);
			if(cNode == null || cNode.StyleIndex == -1)
				return StyleCache.DefaultStyle;
			return ((IExportProvider)this).GetStyle(cNode.StyleIndex);
		}
		ExportCacheCellStyle IExportProvider.GetDefaultStyle() {
			return StyleCache.DefaultStyle;
		}
		int IExportProvider.GetColumnWidth(int col) {
			return package.WorksheetDocument.WorksheetNode.Cols.GetColNodeWithCache(col + 1).WidthInPixels;
		}
		int IExportProvider.GetRowHeight(int row) {
			return package.WorksheetDocument.WorksheetNode.SheetDataNode.GetRowNodeWithCache(row + 1).HtInPixels;
		}
		IExportProvider IExportProvider.Clone(string fileName, System.IO.Stream stream) {
			return (IExportProvider)CloneCore(fileName, stream);
		}
		bool IExportProvider.IsStreamMode {
			get { return base.IsStreamMode; }
		}
		Stream IExportProvider.Stream {
			get { return base.Stream; }
		}
		event ProviderProgressEventHandler IExportProvider.ProviderProgress {
			add {  }
			remove {  }
		}
		#endregion
	}
}
