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
using System.IO;
using System.Linq;
using DevExpress.Export.Xl;
using DevExpress.Printing.Native;
using DevExpress.Utils;
using DevExpress.XtraExport;
using DevExpress.XtraExport.Implementation;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.XLS {
	public class NewExcelExportProvider : IXlsExportProvider {
		public enum ExportFileType {
			Xlsx,
			Xls
		}
		#region inner classes
		public class CellFiller {
			#region inner classes
			struct Сoords : IComparable<Сoords> {
				public readonly int Col;
				public readonly int Row;
				public Сoords(int col, int row) { Col = col; Row = row; }
				public int CompareTo(Сoords y) {
					return this.Row != y.Row ? this.Row - y.Row : this.Col - y.Col;
				}
				public static bool operator <(Сoords x, Сoords y) {
					return x.Row == y.Row ? x.Col < y.Col : x.Row < y.Row;
				}
				public static bool operator >(Сoords x, Сoords y) {
					return x.Row == y.Row ? x.Col > y.Col : x.Row > y.Row;
				}
				public static bool Equals(Сoords x, Сoords y) {
					return x.Row == y.Row && x.Col == y.Col;
				}
			}
			struct FormattingSpan {
				public XlCellFormatting formatting;
				public int colSpan;
				public FormattingSpan(XlCellFormatting formatting, int colSpan) {
					this.formatting = formatting;
					this.colSpan = colSpan;
				}
			}
			#endregion
			NewExcelExportProvider provider;
			IXlExport export;
			IList<int> rowHeights;
			static Сoords maxCoord = new Сoords(int.MaxValue, int.MaxValue);
			Сoords nextObjectCoord;
			Сoords nextFormattingCoord;
			Сoords lastCellCoord;
			Сoords nextCellCoord;
			IXlRow currentRow;
			SortedDictionary<Сoords, FormattingSpan> styleRange = new SortedDictionary<Сoords, FormattingSpan>();
			public CellFiller(NewExcelExportProvider provider, IXlExport export, IList<int> rowHeights) {
				this.provider = provider;
				this.export = export;
				this.rowHeights = rowHeights;
			}
			public void FillCells(ObjectInfo[] mtObjects) {
				int objectIndex = 0;
				ObjectInfo nextObject = mtObjects[objectIndex];
				KeyValuePair<Сoords, FormattingSpan> nextFormatting = new KeyValuePair<Сoords, FormattingSpan>();
				nextObjectCoord = new Сoords(nextObject.ColIndex, nextObject.RowIndex);
				nextFormattingCoord = maxCoord;
				lastCellCoord = new Сoords(0, 0);
				nextCellCoord = nextObjectCoord;
				currentRow = export.BeginRow();
				currentRow.HeightInPixels = rowHeights[currentRow.RowIndex];
				do {
					MoveToFillingCell();
					if(nextCellCoord.Equals(nextObjectCoord)) {
						FillCellWithData(nextObject);
						objectIndex++;
						lastCellCoord = new Сoords(nextObject.ColIndex + nextObject.ColSpan, nextObject.RowIndex);
						nextObject = objectIndex < mtObjects.Length ? mtObjects[objectIndex] : null;
						nextObjectCoord = nextObject != null ? new Сoords(nextObject.ColIndex, nextObject.RowIndex) : maxCoord;
					} else {
						FormattingSpan span = nextFormatting.Value;
						FillCellsWithStyle(span.colSpan, span.formatting);
						lastCellCoord = new Сoords(nextCellCoord.Col + span.colSpan, nextCellCoord.Row);
						styleRange.Remove(nextFormatting.Key);
					}
					if(styleRange.Count > 0) {
						nextFormatting = styleRange.First();
						nextFormattingCoord = nextFormatting.Key;
					} else
						nextFormattingCoord = maxCoord;
					nextCellCoord = nextObjectCoord < nextFormattingCoord ? nextObjectCoord : nextFormattingCoord;
				} while(!nextCellCoord.Equals(maxCoord));
				export.EndRow();
			}
			void MoveToFillingCell() {
				int rowDiff = nextCellCoord.Row - lastCellCoord.Row;
				if(rowDiff > 0) {
					lastCellCoord = new Сoords(0, nextCellCoord.Row);
					AdvanceRow(rowDiff);
				}
				int cellsToSkip = nextCellCoord.Col - lastCellCoord.Col;
				if(cellsToSkip > 0)
					currentRow.SkipCells(cellsToSkip);
			}
			void AdvanceRow(int rowCount) {
				for(int i = 0; i < rowCount; i++) {
					export.EndRow();
					currentRow = export.BeginRow();
					currentRow.HeightInPixels = rowHeights[currentRow.RowIndex];
				}
			}
			internal virtual void SetCurrentObjectInfo(ObjectInfo obj, IXlCell cell) {
				provider.SetObjectInfo(obj, cell);
			}
			void FillCellWithData(ObjectInfo obj) {
				IXlCell cell = export.BeginCell();
				SetCurrentObjectInfo(obj, cell);
				export.EndCell();
				for(int i = 1; i < obj.RowSpan; i++)
					styleRange.Add(new Сoords(obj.ColIndex, obj.RowIndex + i), new FormattingSpan(cell.Formatting, obj.ColSpan));
				if(obj.ColSpan > 1)
					FillCellsWithStyle(obj.ColSpan - 1, cell.Formatting);
			}
			void FillCellsWithStyle(int count, XlCellFormatting formatting) {
				for(int i = 0; i < count; i++) {
					IXlCell cell = export.BeginCell();
					cell.Formatting = formatting;
					export.EndCell();
				}
			}
		}
		static class XlCellFormattingCreator {
			public static XlCellFormatting CreateFormatting(BrickStyle style, ITableCell tableCell, bool rawDataMode, TextExportMode textExportMode) {
				XlCellFormatting formatting = new XlCellFormatting();
				if(style != null) {
					formatting.Font = CreateFont(style.Font);
					formatting.Alignment = new XlCellAlignment()
					{
						HorizontalAlignment = ToHorisontalAlignment(style.StringFormat.Alignment),
						VerticalAlignment = ToVerticalAlignment(style.StringFormat.LineAlignment),
						WrapText = style.StringFormat.WordWrap,
						ReadingOrder = style.StringFormat.RightToLeft ? XlReadingOrder.RightToLeft : XlReadingOrder.Context
					};
					if(!rawDataMode)
						SetColorDetails(style, ref formatting);
				}
				if(textExportMode == TextExportMode.Value) {
					formatting.IsDateTimeFormatString = tableCell.TextValue is DateTime || tableCell.TextValue is TimeSpan;
					formatting.NumberFormat = GetNumberFormat(tableCell.XlsxFormatString, tableCell.FormatString, formatting.IsDateTimeFormatString, tableCell.TextValue);
				} else formatting.NumberFormat = XlNumberFormat.Text;
				return formatting;
			}
			static XlFont CreateFont(Font styleFont) {
				XlFont font = XlFont.CustomFont(styleFont.Name, styleFont.SizeInPoints);
				font.Bold = styleFont.Bold;
				font.Italic = styleFont.Italic;
				font.StrikeThrough = styleFont.Strikeout;
				font.Underline = styleFont.Underline ? XlUnderlineType.Single : XlUnderlineType.None;
				return font;
			}
			static void SetColorDetails(BrickStyle style, ref XlCellFormatting formatting) {
				formatting.Font.Color = XlColor.FromArgb(DXColor.Blend(style.ForeColor, Color.White).ToArgb());
				if(!style.IsTransparent || style.BackColor.A > 0)
					formatting.Fill = new XlFill()
					{
						ForeColor = DXColor.Blend(style.BackColor, Color.White),
						PatternType = style.BackColor == Color.Transparent ? XlPatternType.None : XlPatternType.Solid
					};
				if(style.Sides != BorderSide.None)
					formatting.Border = CreateBorder(style);
			}
			static XlNumberFormat GetNumberFormat(string xlsxFormatString, string formatString, bool isDateTimeFormatString, object value) {
				if(xlsxFormatString != null)
					return (XlNumberFormat)xlsxFormatString;
				if(formatString != null) {
					DevExpress.Export.Xl.XlExportNumberFormatConverter converter = new DevExpress.Export.Xl.XlExportNumberFormatConverter();
					ExcelNumberFormat excelFormat = converter.Convert(formatString, isDateTimeFormatString, System.Windows.Forms.Application.CurrentCulture);
					if(excelFormat != null)
						return (XlNumberFormat)excelFormat.FormatString;
				}
				if(isDateTimeFormatString)
					return XlNumberFormat.ShortDateTime;
				if(value is string)
					return ((string)value).Length <= 256 ? XlNumberFormat.Text : XlNumberFormat.General;
				return XlNumberFormat.General;
			}
			static XlHorizontalAlignment ToHorisontalAlignment(StringAlignment stringAlignment) {
				switch(stringAlignment) {
					case StringAlignment.Far: return XlHorizontalAlignment.Right;
					case StringAlignment.Center: return XlHorizontalAlignment.Center;
					default: return XlHorizontalAlignment.Left;
				}
			}
			static XlVerticalAlignment ToVerticalAlignment(StringAlignment stringAlignment) {
				switch(stringAlignment) {
					case StringAlignment.Far: return XlVerticalAlignment.Bottom;
					case StringAlignment.Center: return XlVerticalAlignment.Center;
					default: return XlVerticalAlignment.Top;
				}
			}
			static XlBorder CreateBorder(BrickStyle style) {
				XlBorder border = new XlBorder();
				ExportCacheCellBorderStyle borderStyle = new ExportCacheCellBorderStyle();
				borderStyle.Width = Convert.ToInt32(style.BorderWidth);
				borderStyle.BorderDashStyle = style.BorderDashStyle;
				borderStyle.Color_ = GetActualColor(style.BorderColor, style.BackColor, Color.White);
				XlBorderLineStyle lineStyle = (XlBorderLineStyle)ExcelHelper.ConvertBorderStyle(borderStyle.Width, borderStyle.BorderDashStyle);
				if((BorderSide.Left & style.Sides) != 0) {
					border.LeftColor = borderStyle.Color_;
					border.LeftLineStyle = lineStyle;
				}
				if((BorderSide.Right & style.Sides) != 0) {
					border.RightColor = borderStyle.Color_;
					border.RightLineStyle = lineStyle;
				}
				if((BorderSide.Top & style.Sides) != 0) {
					border.TopColor = borderStyle.Color_;
					border.TopLineStyle = lineStyle;
				}
				if((BorderSide.Bottom & style.Sides) != 0) {
					border.BottomColor = borderStyle.Color_;
					border.BottomLineStyle = lineStyle;
				}
				return border;
			}
			static Color GetActualColor(params Color[] colors) {
				for(int i = 0; i < colors.Length - 1; i++)
					if(!colors[i].IsEmpty) return colors[i];
				return colors[colors.Length - 1];
			}
		}
		#endregion
		#region Fields & properties
		const int maxXlsRowCount = 65536;
		const Int16 maxXlsColumnCount = 256;
		const int maxXlsxRowCount = 1048576;
		const Int16 maxXlsxColumnCount = 16384;
		IXlExport export;
		IXlSheet sheet;
		IXlShapeContainer container;
		IXlCell cell;
		List<int> colWidths;
		List<int> rowHeights;
		XlsExportContext excelExportContext;
		ProgressMaster progressMaster;
		ObjectInfo currentObjectInfo;   
		Dictionary<MultiKey, XlCellFormatting> styleCache = new Dictionary<MultiKey, XlCellFormatting>(); 
		bool exportCrossReferences = false;
		List<XlHyperlink> crossReferencesCache = new List<XlHyperlink>();
		Dictionary<string, int[]> anchorCache = new Dictionary<string, int[]>();
		int CurrentRowIndex { get { return currentObjectInfo.RowIndex; } }
		int CurrentColIndex { get { return currentObjectInfo.ColIndex; } }
		int CurrentRowSpan { get { return currentObjectInfo.RowSpan; } }
		int CurrentColSpan { get { return currentObjectInfo.ColSpan; } }
		public BrickViewData CurrentData { get { return (BrickViewData)currentObjectInfo.Object; } }
		public XlsExportContext XlsExportContext { get { return excelExportContext; } }
		ExportContext ITableExportProvider.ExportContext { get { return excelExportContext; } }
		#endregion
		public NewExcelExportProvider(Stream stream, XlsExportContext exportContext, ExportFileType exportFileType, ProgressMaster progressMaster) {
			this.excelExportContext = exportContext;
			if(progressMaster != null)
				this.progressMaster = progressMaster;
			if(exportFileType == ExportFileType.Xls)
				export = new DevExpress.XtraExport.Xls.XlsDataAwareExporter();
			else
				export = new DevExpress.XtraExport.Xlsx.XlsxDataAwareExporter();
			export.BeginExport(stream);
		}
		static TextExportMode ToTextExportMode(DevExpress.Utils.DefaultBoolean value, TextExportMode defaultTextExportMode) {
			return value == DevExpress.Utils.DefaultBoolean.True ? TextExportMode.Value :
				value == DevExpress.Utils.DefaultBoolean.False ? TextExportMode.Text :
				defaultTextExportMode;
		}
		static XlOutlineDashing GetXlOutlineDashing(DashStyle lineStyle) {
			switch(lineStyle){
				case DashStyle.Solid: return XlOutlineDashing.Solid;
				case DashStyle.Dash:return XlOutlineDashing.SystemDash;
				case DashStyle.Dot: return XlOutlineDashing.SystemDot;
				case DashStyle.DashDot: return XlOutlineDashing.SystemDashDot;
				case DashStyle.DashDotDot: return XlOutlineDashing.SystemDashDotDot;
				default: return XlOutlineDashing.Solid;
			}
		}
		#region ITableExportProvider
		void ITableExportProvider.SetCellText(object textValue, string hyperLink) {
			TextExportMode mode = ToTextExportMode(CurrentData.TableCell.XlsExportNativeFormat, this.excelExportContext.XlsExportOptions.TextExportMode);
			((IXlsExportProvider)this).SetCellData(HotkeyPrefixHelper.PreprocessHotkeyPrefixesInObject(textValue, CurrentData, mode));
		}
		void ITableExportProvider.SetCellImage(Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imageSize, PaddingInfo padding, string hyperLink) {
			IXlPicture picture = export.BeginPicture();
			picture.Image = image;
			if(excelExportContext.XlsExportOptions.ExportHyperlinks)
				picture.HyperlinkClick.TargetUri = hyperLink;
			picture.AnchorType = XlAnchorType.TwoCell;
			picture.AnchorBehavior = XlAnchorType.OneCell;
			int startColOffset = padding.Left;
			int startRowOffset = padding.Top;
			int endColIndex = CurrentColIndex + CurrentColSpan - 1;
			int endColOffset = colWidths[endColIndex] - padding.Right;
			int endRowIndex = CurrentRowIndex + CurrentRowSpan - 1;
			int endRowOffset = rowHeights[endRowIndex] - padding.Bottom;
			if(CurrentData.Style != null) {
				int borderWidth = (int)Math.Ceiling((double)ExcelHelper.GetBorderWidth(Convert.ToInt32(CurrentData.Style.BorderWidth), CurrentData.Style.BorderDashStyle) / 2);
				if((CurrentData.Style.Sides & BorderSide.Left) != 0)
					startColOffset += borderWidth;
				if((CurrentData.Style.Sides & BorderSide.Top) != 0)
					startRowOffset += borderWidth;
				if((CurrentData.Style.Sides & BorderSide.Right) != 0)
					endColOffset -= borderWidth;
				if((CurrentData.Style.Sides & BorderSide.Bottom) != 0)
					endRowOffset -= borderWidth;
			}
			picture.TopLeft = new XlAnchorPoint(
				CurrentColIndex,
				CurrentRowIndex,
				startColOffset,
				startRowOffset,
				colWidths[CurrentColIndex],
				rowHeights[CurrentRowIndex]);
			picture.BottomRight = new XlAnchorPoint(
				endColIndex,
				endRowIndex,
				endColOffset,
				endRowOffset,
				colWidths[endColIndex],
				rowHeights[endRowIndex]);
			export.EndPicture();
		}
		void ITableExportProvider.SetCellShape(Color lineColor, DevExpress.XtraReports.UI.LineDirection lineDirection, DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink) {
			XlShape shape = XlShape.Line(XlColor.FromArgb(lineColor.ToArgb()), GetXlOutlineDashing(lineStyle));
			shape.Outline.Width = GraphicsUnitConverter.Convert(lineWidth, GraphicsUnit.Pixel, GraphicsUnit.Point);
			if(lineDirection == XtraReports.UI.LineDirection.Vertical || lineDirection == XtraReports.UI.LineDirection.Slant)
				shape.Frame.FlipVertical = true;
			int startColOffset = padding.Left;
			int startRowOffset = padding.Top;
			int endColIndex = CurrentColIndex + CurrentColSpan - 1;
			int endColOffset = colWidths[endColIndex] - padding.Right;
			int endRowIndex = CurrentRowIndex + CurrentRowSpan - 1;
			int endRowOffset = rowHeights[endRowIndex] - padding.Bottom;
			if(lineDirection == XtraReports.UI.LineDirection.Horizontal) {
				int rectHeight = -padding.Top - padding.Bottom;
				for(int i = CurrentRowIndex; i <= endRowIndex; i++)
					rectHeight += rowHeights[i];
				int additionalOffset = rectHeight / 2;
				startRowOffset += additionalOffset;
				endRowOffset -= additionalOffset;
			} else if(lineDirection == XtraReports.UI.LineDirection.Vertical) {
				int rectWidth = -padding.Left - padding.Right;
				for(int i = CurrentColIndex; i <= endColIndex; i++)
					rectWidth += colWidths[i];
				int additionalOffset = rectWidth / 2;
				startColOffset += additionalOffset;
				endColOffset -= additionalOffset;
			}
			XlAnchorPoint topLeft = CalculateAnchorPoint(new XlAnchorPoint(
				CurrentColIndex,
				CurrentRowIndex,
				startColOffset,
				startRowOffset,
				colWidths[CurrentColIndex],
				rowHeights[CurrentRowIndex]),
				colWidths,
				rowHeights);
			XlAnchorPoint bottomRight = CalculateAnchorPoint(new XlAnchorPoint(
				endColIndex,
				endRowIndex,
				endColOffset,
				endRowOffset,
				colWidths[endColIndex],
				rowHeights[endRowIndex]),
				colWidths,
				rowHeights);
			shape.SetTwoCellAnchor(topLeft, bottomRight, XlAnchorType.TwoCell);
			container.Shapes.Add(shape);
		}
		static XlAnchorPoint CalculateAnchorPoint(XlAnchorPoint source, List<int> colWidths, List<int> rowHeights) {
			int column = source.Column;
			int row = source.Row;
			int columnOffsetInPixels = source.ColumnOffsetInPixels;
			int rowOffsetInPixels = source.RowOffsetInPixels;
			int cellWidth = colWidths[column];
			int cellHeight = rowHeights[row];
			while(columnOffsetInPixels < 0 && column > 0) {
				column--;
				cellWidth = colWidths[column];
				columnOffsetInPixels += cellWidth;
			}
			while(columnOffsetInPixels > cellWidth) {
				columnOffsetInPixels -= cellWidth;
				column++;
				cellWidth = colWidths[column];
			}
			while(rowOffsetInPixels < 0 && row > 0) {
				row--;
				cellHeight = rowHeights[row];
				rowOffsetInPixels += cellHeight;
			}
			while(rowOffsetInPixels > cellHeight) {
				rowOffsetInPixels -= cellHeight;
				row++;
				cellHeight = rowHeights[row];
			}
			return new XlAnchorPoint(column, row, columnOffsetInPixels, rowOffsetInPixels, cellWidth, cellHeight);
		}
		#endregion
		void IXlsExportProvider.SetCellData(object data) {
			XlVariantValue value = XlVariantValue.FromObject(data);
			if(!value.IsEmpty) {
				if(XlsExportContext.XlsExportOptions.RawDataMode && value.IsBoolean)
					value = value.ToText();
				cell.Value = value;
			}
		}
		void SetDocumentProperties() {
			XlDocumentOptions documentOptions = excelExportContext.XlsExportOptions.DocumentOptions;
			export.DocumentProperties.Application = documentOptions.Application;
			export.DocumentProperties.Author = documentOptions.Author;
			export.DocumentProperties.Category = documentOptions.Category;
			export.DocumentProperties.Company = documentOptions.Company;
			export.DocumentProperties.Description = documentOptions.Comments;
			export.DocumentProperties.Keywords = documentOptions.Tags;
			export.DocumentProperties.Subject = documentOptions.Subject;
			export.DocumentProperties.Title = documentOptions.Title;
		}
		public void CreateDocument(Document document) {
			SetDocumentProperties();
			if(document.PageCount > 0) {
				if(excelExportContext.XlsExportOptions.IsMultiplePaged) {
					int[] pageIndices = PageRangeParser.GetIndices(XlsExportContext.XlsExportOptions.PageRange, document.PageCount);
					if(pageIndices.Length > 1) {
						progressMaster.InitializeRangeByPages(pageIndices.Length);
						foreach(int index in pageIndices) {
							CreateSheet(document, index);
							progressMaster.PageExported();
						}
						progressMaster.AllPagesExported();
					} else {
						int index = XlsExportContext.XlsExportOptions.PageRange.IsEmpty() ? 0 : int.Parse(XlsExportContext.XlsExportOptions.PageRange) - 1;
						CreateSheet(document, index);
					}
				} else {
					exportCrossReferences = true;
					CreateSheet(document, CreateLayoutBuilder(document), excelExportContext.XlsExportOptions.SheetName);			 
				}
			}
			export.EndExport();
		}
		void CreateSheet(Document document, int pageIndex) {
			using(XlsPageLayoutBuilder layoutBuilder = (XlsPageLayoutBuilder)CreateLayoutBuilder(document, pageIndex))
				CreateSheet(document, layoutBuilder, GetFinalSheetName(pageIndex, excelExportContext.XlsExportOptions.SheetName + (pageIndex + 1).ToString()));
		} 
		ILayoutBuilder CreateLayoutBuilder(Document document) {
			return new XlsLayoutBuilder(document, XlsExportContext);
		}
		ILayoutBuilder CreateLayoutBuilder(Document document, int index) {
			return new XlsPageLayoutBuilder(document.Pages[index], XlsExportContext);
		}
		string GetFinalSheetName(int index, string sheetName) {
			XlSheetCreatedEventArgs sheetCreatedEventArgs = new XlSheetCreatedEventArgs(index, sheetName);
			excelExportContext.PrintingSystem.OnXlSheetCreated(this, sheetCreatedEventArgs);
			return sheetCreatedEventArgs.SheetName;
		}
		void CreateSheet(Document document, ILayoutBuilder layoutBuilder, string sheetName) {
			sheet = export.BeginSheet();
			container = sheet as IXlShapeContainer;
			bool correctImportBrickBounds = document.CorrectImportBrickBounds;
			ReadonlyPageData pageData = excelExportContext.XlsExportOptions.IsMultiplePaged ? excelExportContext.DrawingPage.PageData : excelExportContext.PrintingSystem.PageSettings.Data;
			MarginsF marginsF = GraphicsUnitConverter.Convert(pageData.MarginsF, GraphicsDpi.Document, GraphicsDpi.Inch);
			SetPageSettings(marginsF, pageData.PaperKind, pageData.Landscape, sheetName);
			MegaTable megaTable = new MegaTable(layoutBuilder.BuildLayoutControls(), !XlsExportContext.RawDataMode, correctImportBrickBounds);
			ValidateMegaTable(XlsExportContext.XlsExportOptions, megaTable);
			if(megaTable.Objects.Length > 0)
				try {
					progressMaster.InitializeRangeByObjects(megaTable.Objects.Length);
					SetCurrentSheetTable(megaTable);
					if(exportCrossReferences)
						SetCrossReferences();
				} finally {
					progressMaster.AllObjectsExported();
					export.EndSheet();
				}
		}
		void ValidateMegaTable(XlsExportOptionsBase exportOptions, MegaTable megaTable) {
			if(exportOptions is XlsExportOptions) {
				if(megaTable.RowCount > maxXlsRowCount && !((XlsExportOptions)exportOptions).Suppress65536RowsWarning)
					throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsMoreThanMaxRows));
				if(megaTable.ColumnCount > maxXlsColumnCount && !((XlsExportOptions)exportOptions).Suppress256ColumnsWarning)
					throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsMoreThanMaxColumns));
			} else {
				if(megaTable.RowCount > maxXlsxRowCount)
					throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsxMoreThanMaxRows));
				if(megaTable.ColumnCount > maxXlsxColumnCount)
					throw new Exception(PreviewLocalizer.GetString(PreviewStringId.Msg_XlsxMoreThanMaxColumns));
			}
		}
		public void SetPageSettings(MarginsF margins, System.Drawing.Printing.PaperKind paperKind, bool landscape, string sheetName) {
			XlPageMargins pageMargins = new XlPageMargins();
			pageMargins.Bottom = margins.Bottom;
			pageMargins.Left = margins.Left;
			pageMargins.Right = margins.Right;
			pageMargins.Top = margins.Top;
			sheet.PageMargins = pageMargins;
			sheet.PageSetup = new XlPageSetup();
			sheet.PageSetup.PaperKind = paperKind;
			sheet.PageSetup.PageOrientation = landscape ? XlPageOrientation.Landscape : XlPageOrientation.Portrait;
			sheet.PageSetup.FitToPage = excelExportContext.XlsExportOptions.FitToPrintedPageWidth;
			if(sheet.PageSetup.FitToPage) {
				sheet.PageSetup.FitToWidth = 1;
				sheet.PageSetup.FitToHeight = 0;
			}
			sheet.ViewOptions.ShowGridLines = excelExportContext.XlsExportOptions.ShowGridLines;
			sheet.ViewOptions.RightToLeft = excelExportContext.XlsExportOptions.RightToLeftDocument == DefaultBoolean.True;
			sheet.PrintOptions = new XlPrintOptions();
			sheet.PrintOptions.GridLines = excelExportContext.XlsExportOptions.ShowGridLines;
			sheet.Name = sheetName;		   
		}
		void SetCurrentSheetTable(MegaTable megaTable) {
			ObjectInfo[] mtObjects = megaTable.Objects;
			colWidths = megaTable.ColWidths;
			rowHeights = megaTable.RowHeights;
			Array.Sort(mtObjects, (x, y) => { int r = x.RowIndex.CompareTo(y.RowIndex); return r != 0 ? r : x.ColIndex.CompareTo(y.ColIndex); });
			for(int columnIndex = 0; columnIndex < megaTable.ColumnCount; columnIndex++) {
				IXlColumn xlColumn = export.BeginColumn();
				xlColumn.WidthInPixels = megaTable.ColWidths[columnIndex];
				export.EndColumn();
			}
			CellFiller cellFiller = new CellFiller(this, export, megaTable.RowHeights);
			cellFiller.FillCells(mtObjects);
		}
		public void SetObjectInfo(ObjectInfo objectInfo, IXlCell cell) {
			this.cell = cell;
			currentObjectInfo = objectInfo;
			BrickViewData data = (BrickViewData)objectInfo.Object;
			if(!(data.TableCell is EmptyBrick)) {
				((BrickExporter)this.excelExportContext.PrintingSystem.ExportersFactory.GetExporter(data.TableCell)).FillXlsTableCell(this);
				SetCellStyle(data.Style, data.TableCell, objectInfo);
			}
			if(objectInfo.ColSpan > 1 || objectInfo.RowSpan > 1)
				SetCellUnion(objectInfo.ColIndex, objectInfo.RowIndex, objectInfo.ColSpan, objectInfo.RowSpan);
			if(excelExportContext.XlsExportOptions.ExportHyperlinks)
				SetCellUrlAndAnchor(data.TableCell);
			progressMaster.ObjectExported();
		}
		public void SetCellStyle(BrickStyle style, ITableCell tableCell, ObjectInfo objectInfo) {
			XlCellFormatting formatting;
			MultiKey key = new MultiKey(style, tableCell.FormatString, tableCell.TextValue is DateTime);
			if(!styleCache.TryGetValue(key, out formatting)) {
				TextExportMode textExportMode = ToTextExportMode(tableCell.XlsExportNativeFormat, excelExportContext.XlsExportOptions.TextExportMode);
				formatting = XlCellFormattingCreator.CreateFormatting(style, tableCell, excelExportContext.RawDataMode, textExportMode);
				styleCache.Add(key, formatting);
			}
			cell.Formatting = formatting;
		}
		public void SetCellUnion(int col, int row, int width, int height) {
			sheet.MergedCells.Add(new XlCellRange(new XlCellPosition(col, row), new XlCellPosition(col + width - 1, row + height - 1)), false);
		}
		public void SetCellUrlAndAnchor(ITableCell tableCell) {
			if(!string.IsNullOrEmpty(tableCell.Url)) {
				if(HasUrlScheme(tableCell.Url))
					sheet.Hyperlinks.Add(new XlHyperlink() { TargetUri = tableCell.Url, Reference = new XlCellRange(cell.Position) });
				else
					if(exportCrossReferences)
						crossReferencesCache.Add(new XlHyperlink() { TargetUri = tableCell.Url, Reference = new XlCellRange(cell.Position) });
			}
			SetAnchor(cell.ColumnIndex, cell.RowIndex, tableCell);
		}
		void SetCrossReferences() {
			foreach(XlHyperlink crossReference in crossReferencesCache)
				foreach(var anchor in anchorCache)
					if(crossReference.TargetUri == anchor.Key) {
						string destCell = IntToLetter(anchor.Value[0]) + (anchor.Value[1] + 1).ToString();
						crossReference.TargetUri = '#' + destCell;
						sheet.Hyperlinks.Add(crossReference);
						break;
					}
		}
		static bool HasUrlScheme(string url) {
			return PrintingSettings.AllowCustomUrlScheme
				? System.Text.RegularExpressions.Regex.IsMatch(url, @"^[a-zA-Z]([a-zA-Z0-9+\-\.])*:")
				: url.StartsWith("http") || url.StartsWith("mailto") || url.StartsWith("file:///");
		}
		static string IntToLetter(int intCol) {
			int intFirstLetter = ((intCol) / 676) + 64;
			int intSecondLetter = ((intCol % 676) / 26) + 64;
			int intThirdLetter = (intCol % 26) + 65;
			if(intSecondLetter == 64 && intFirstLetter > 64) {
				intSecondLetter = 90;
				intFirstLetter--;
			}
			char firstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
			char secondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
			char thirdLetter = (char)intThirdLetter;
			return string.Concat(firstLetter, secondLetter, thirdLetter).Trim();
		}
		void SetAnchor(int col, int row, ITableCell tableCell) {
			AnchorCell anchorCell = tableCell as AnchorCell;
			if(anchorCell != null && anchorCell.InnerCell != null) {
				this.SetAnchor(col, row, anchorCell.InnerCell);
			} else {
				string anchorName = tableCell is VisualBrick ? ((VisualBrick)tableCell).AnchorName : anchorCell != null ? anchorCell.AnchorName : string.Empty;
				if(!string.IsNullOrEmpty(anchorName) && exportCrossReferences && !anchorCache.ContainsKey(anchorName))
					anchorCache.Add(anchorName, new int[] { col, row });
			}
		}
#if DEBUGTEST
		public static XlVariantValue Test_SetCellData(IXlsExportProvider provider, object data) {
			((NewExcelExportProvider)provider).cell = new XlCell();
			provider.SetCellData(data);
			return ((NewExcelExportProvider)provider).cell.Value;
		}
#endif
	}
	public class ProgressMaster {
		enum ProgressMode {
			None,
			ByObjects,
			ByPages,
		}
		ProgressMode progressMode;
		ProgressReflector progressReflector;
		public ProgressMaster(ProgressReflector progressReflector, XlsExportMode exportMode) {
			this.progressReflector = progressReflector;
			this.progressMode = GetProgressMode(exportMode);
		}
		public ProgressMaster(ProgressReflector progressReflector, XlsxExportMode exportMode) {
			this.progressReflector = progressReflector;
			this.progressMode = GetProgressMode(exportMode);
		}
		static ProgressMode GetProgressMode(XlsExportMode exportMode) {
			switch(exportMode) {
				case XlsExportMode.SingleFile:
					return ProgressMode.ByObjects;
				default:
					return ProgressMode.None;
			}
		}
		static ProgressMode GetProgressMode(XlsxExportMode exportMode) {
			switch(exportMode) {
				case XlsxExportMode.SingleFile:
					return ProgressMode.ByObjects;
				case XlsxExportMode.SingleFilePageByPage:
					return ProgressMode.ByPages;
				default:
					return ProgressMode.None;
			}
		}
		public void InitializeRangeByObjects(int count){
			if(progressMode == ProgressMode.ByObjects)
				progressReflector.InitializeRange(count);
		}
		public void InitializeRangeByPages(int count) {
			if(progressMode == ProgressMode.ByPages)
				progressReflector.InitializeRange(count);
		}
		public void ObjectExported() {
			if(progressMode == ProgressMode.ByObjects)
				progressReflector.RangeValue++;
		}
		public void PageExported() {
			if(progressMode == ProgressMode.ByPages)
				progressReflector.RangeValue++;
		}
		public void AllObjectsExported() {
			if(progressMode == ProgressMode.ByObjects)
				progressReflector.MaximizeRange();
		}
		public void AllPagesExported() {
			if(progressMode == ProgressMode.ByPages)
				progressReflector.MaximizeRange();
		}
	} 
}
