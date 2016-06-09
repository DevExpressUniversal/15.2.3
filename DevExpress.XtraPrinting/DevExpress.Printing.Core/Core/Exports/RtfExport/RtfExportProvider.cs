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

using DevExpress.Printing.Exports.RtfExport;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
namespace DevExpress.XtraPrinting.Export.Rtf {
	public class RtfExportProvider : RtfDocumentProviderBase, IRtfExportProvider {
		#region inner classes
		class RtfLayoutControlComparer : IComparer<ILayoutControl> {
			int IComparer<ILayoutControl>.Compare(ILayoutControl xControl, ILayoutControl yControl) {
				return Compare(xControl.Top, yControl.Top);
			}
			int Compare(int y1, int y2) {
				if(y1 > y2) {
					return 1;
				} else if(y1 < y2) {
					return -1;
				} else
					return 0;
			}
		}
		#endregion
		readonly bool emptyFirstPageHeaderFooter, exportPageBreaks;
		float footerY = 0;
		int topMarginOffset = 0;
		RtfChunkData currentChunkData, headerChunkData, footerChunkData;
		List<RtfChunkData> mainChunkDataList;
		System.Drawing.Printing.Margins margins;
		Rectangle pageBounds;
		ObjectInfo currentInfo;
		public override BrickViewData CurrentData { get { return (BrickViewData)currentInfo.Object; } }
		int CurrentRowIndex { get { return currentInfo.RowIndex; } }
		int CurrentColIndex { get { return currentInfo.ColIndex; } }
		int CurrentColSpan { get { return currentInfo.ColSpan; } }
		int CurrentRowSpan { get { return currentInfo.RowSpan; } }
		public RtfExportProvider(Stream stream, Document document, RtfExportOptions options)
			: base(stream, new RtfExportContext(document.PrintingSystem, new RtfExportHelper())) {
			emptyFirstPageHeaderFooter = options.EmptyFirstPageHeaderFooter;
			exportPageBreaks = options.ExportPageBreaks;
		}
		public override void SetCellStyle() {
			if(CurrentStyle.BackColor != Color.Transparent) {
				currentChunkData.SetBackColor();
			}
			currentChunkData.SetBorders(CurrentColIndex, CurrentRowIndex);
			currentChunkData.SetForeColor();
			currentChunkData.SetFontString(GetFontString());
			currentChunkData.SetPadding();
			SetDirection();
			currentChunkData.SetCellGAlign();
			currentChunkData.SetCellVAlign();
		}
		protected override void OverrideFontSizeToPreventDissapearBottomBorder() {
		}
		int GetTopMarginContent(LayoutControlCollection layoutControls) {
			LayoutControlCollection topMarginLayoutControls = new LayoutControlCollection();
			for(int i = 0; i < layoutControls.Count; i++) {
				BrickViewData viewData = (BrickViewData)layoutControls[i];
				if(viewData.TableCell.Modifier == BrickModifier.MarginalHeader) {
					topMarginLayoutControls.Add(viewData);
				} else { 
					break; 
				}
			}
			if(topMarginLayoutControls.Count > 0) {
				topMarginOffset = GraphicsUnitConverter.Convert(margins.Top, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.DeviceIndependentPixel);
				MegaTable headerMegaTable = new MegaTable(topMarginLayoutControls, true, RtfExportContext.PrintingSystem.Document.CorrectImportBrickBounds);
				headerChunkData = new TableRtfChunkData(headerMegaTable.ColWidths, headerMegaTable.RowHeights) { CanAutoHeight = false };
				currentChunkData = headerChunkData;
				GetHeaderFooterContent(headerChunkData, headerMegaTable);
			}
			return topMarginLayoutControls.Count;
		}
		int GetBottomMarginContent(LayoutControlCollection layoutControls, float bottomMarginOffset) {
			LayoutControlCollection bottomMarginLayoutControls = new LayoutControlCollection();
			for(int i = layoutControls.Count - 1; i >= 0; i--) {
				BrickViewData viewData = (BrickViewData)layoutControls[i];
				if(viewData.TableCell.Modifier == BrickModifier.MarginalFooter) {
					bottomMarginLayoutControls.Add(viewData);
				} else {
					break;
				}
			}
			if(bottomMarginLayoutControls.Count > 0) {
				BrickViewData firstBottomControl = bottomMarginLayoutControls[bottomMarginLayoutControls.Count - 1] as BrickViewData;
				BrickViewData lastBottomControl = bottomMarginLayoutControls[0] as BrickViewData;
				float bottomContentHeight = lastBottomControl.Top + lastBottomControl.Height - firstBottomControl.Top;
				footerY = ConvertToTwips(margins.Bottom - bottomContentHeight, GraphicsDpi.HundredthsOfAnInch) - ConvertToTwips(bottomMarginOffset, GraphicsDpi.Document);
				MegaTable footerMegaTable = new MegaTable(bottomMarginLayoutControls, true, RtfExportContext.PrintingSystem.Document.CorrectImportBrickBounds);
				List<int> rowHeights = footerMegaTable.RowHeights;
				rowHeights[0] = (int)GraphicsUnitConverter.Convert(bottomMarginOffset, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
				footerChunkData = new TableRtfChunkData(footerMegaTable.ColWidths, rowHeights) { CanAutoHeight = false };
				currentChunkData = footerChunkData;
				GetHeaderFooterContent(footerChunkData, footerMegaTable);
			}
			return bottomMarginLayoutControls.Count;
		}
		void GetHeaderFooterContent(RtfChunkData chunkData, MegaTable megaTable) {
			chunkData.FillTemplate();
			for(int i = 0; i < megaTable.Objects.Length; i++) {
				currentInfo = megaTable.Objects[i];
				UpdateCurrentChunkInfo();
				SetCurrentCell();
			}
		}
		protected override void GetContent() {
			mainChunkDataList = new List<RtfChunkData>();
			if(PrintingSettings.UseNewSingleFileRtfExport) {
				RtfExportContext.ProgressReflector.SetProgressRanges(new float[] { 1, 4 });
				RtfLayoutBuilder builder = new RtfLayoutBuilder(RtfExportContext.PrintingSystem.Document, RtfExportContext);
				LayoutControlCollection layoutControls = builder.BuildLayoutControls();
				margins = builder.PageMargins;
				pageBounds = builder.PageBounds;
				if(layoutControls.Count == 0)
					return;
				MegaTable megaTable;
				List<int> rowHeights;
				try {
					RtfExportContext.ProgressReflector.InitializeRange(layoutControls.Count);
					layoutControls.Sort(new RtfLayoutControlComparer());
					int topMarginControlsCount = GetTopMarginContent(layoutControls);
					int bottomMarginControlsCount = GetBottomMarginContent(layoutControls, builder.BottomMarginOffset);
					layoutControls = new LayoutControlCollection(layoutControls.GetRange(topMarginControlsCount, layoutControls.Count - topMarginControlsCount - bottomMarginControlsCount));
					if(layoutControls.Count == 0)
						return;
					IEnumerator mcEnumerator = builder.MultiColumnInfo.GetEnumerator();
					MultiColumnInfo currentMultiColumn = GetNextMultiColumnValue(mcEnumerator);
					bool insideMultiColumn = false;
					IEnumerator pbEnumerator = builder.PageBreakPositions.GetEnumerator();
					int? currentPageBreak = exportPageBreaks ? GetNextPageBreakValue(null, pbEnumerator, 0, 0) : null;
					int offsetY = topMarginOffset;
					LayoutControlDivider divider = new LayoutControlDivider(layoutControls);
					foreach(LayoutControlCollection layoutControlsPart in divider.Parts) {
						megaTable = new MegaTable(layoutControlsPart, true, RtfExportContext.PrintingSystem.Document.CorrectImportBrickBounds);
						rowHeights = megaTable.RowHeights;
						rowHeights[0] -= offsetY;
						RtfChunkData chunkData;
						RtfMultiColumn rtfMultiColumn = RtfMultiColumn.None;
						if(currentMultiColumn != null) {
							if(currentMultiColumn.Start + topMarginOffset >= offsetY && currentMultiColumn.Start + topMarginOffset < megaTable.Height) {
								rtfMultiColumn = RtfMultiColumn.Start;
								insideMultiColumn = true;
							} else if(currentMultiColumn.End != 0 && currentMultiColumn.End + topMarginOffset >= offsetY && currentMultiColumn.End + topMarginOffset < megaTable.Height) {
								rtfMultiColumn = RtfMultiColumn.End;
								insideMultiColumn = false;
							}
						}
						if(!insideMultiColumn && CanExportAsPlain(megaTable.Objects)) {
							int usefulPageWidth = ConvertToTwips(pageBounds.Width - margins.Left - margins.Right, GraphicsDpi.HundredthsOfAnInch);
							chunkData = new PlainRtfChunkData(megaTable.ColWidths, rowHeights) { UsefulPageWidth = usefulPageWidth };
						} else {
							chunkData = new TableRtfChunkData(megaTable.ColWidths, rowHeights);
						}
						currentChunkData = chunkData;
						if(currentPageBreak.HasValue && currentPageBreak.Value + topMarginOffset >= offsetY && currentPageBreak.Value + topMarginOffset < megaTable.Height) {
							currentChunkData.PageBreakLocation = currentPageBreak.Value - offsetY;
							currentPageBreak = GetNextPageBreakValue(currentPageBreak, pbEnumerator, topMarginOffset, megaTable.Height);
						}
						if(rtfMultiColumn == RtfMultiColumn.Start) {
							currentChunkData.MultiColumn = RtfMultiColumn.Start;
							currentChunkData.ColumnCount = currentMultiColumn.ColumnCount;
						} else if(rtfMultiColumn == RtfMultiColumn.End) {
							currentChunkData.MultiColumn = RtfMultiColumn.End;
							currentMultiColumn = GetNextMultiColumnValue(mcEnumerator);
						}
						currentChunkData.FillTemplate();
						mainChunkDataList.Add(currentChunkData);
						offsetY = megaTable.Height;
						for(int i = 0; i < megaTable.Objects.Length; i++) {
							currentInfo = megaTable.Objects[i];
							UpdateCurrentChunkInfo();
							SetCurrentCell();
							RtfExportContext.ProgressReflector.RangeValue++;
						}
					}
				} finally {
					RtfExportContext.ProgressReflector.MaximizeRange();
				}
			} else {
				RtfExportContext.ProgressReflector.SetProgressRanges(new float[] { 1, 4 });
				RtfLayoutBuilder builder = new RtfLayoutBuilder(RtfExportContext.PrintingSystem.Document, RtfExportContext);
				LayoutControlCollection layoutControls = builder.BuildLayoutControls();
				MegaTable megaTable;
				margins = builder.PageMargins;
				pageBounds = builder.PageBounds;
				List<int> rowHeights;
				megaTable = new MegaTable(layoutControls, true, RtfExportContext.PrintingSystem.Document.CorrectImportBrickBounds);
				rowHeights = megaTable.RowHeights;
				RtfChunkData mainChunkData = new TableRtfChunkData(megaTable.ColWidths, rowHeights);
				mainChunkData.FillTemplate();
				mainChunkDataList.Add(mainChunkData);
				currentChunkData = mainChunkData;
				try {
					RtfExportContext.ProgressReflector.InitializeRange(megaTable.Objects.Length);
					for(int i = 0; i < megaTable.Objects.Length; i++) {
						currentInfo = megaTable.Objects[i];
						UpdateCurrentChunkInfo();
						SetCurrentCell();
						RtfExportContext.ProgressReflector.RangeValue++;
					}
				} finally {
					RtfExportContext.ProgressReflector.MaximizeRange();
				}
			}
		}
		private void UpdateCurrentChunkInfo() {
			currentChunkData.UpdateDataIndexes(CurrentColIndex, CurrentRowIndex, CurrentColSpan, CurrentRowSpan);
			if(CurrentStyle != null) {
				int backColorIndex = CurrentStyle.BackColor == Color.Transparent ? 0 :
					rtfExportHelper.GetColorIndex(DXColor.Blend(CurrentStyle.BackColor, Color.White));
				int borderColorIndex = CurrentStyle.Sides == BorderSide.None ? 0 : 
					rtfExportHelper.GetColorIndex(DXColor.Blend(CurrentStyle.BorderColor, Color.White));
				int foreColorIndex = rtfExportHelper.GetColorIndex(DXColor.Blend(CurrentStyle.ForeColor, Color.White));
				currentChunkData.UpdateColors(backColorIndex, borderColorIndex, foreColorIndex);
				currentChunkData.UpdateStyle(CurrentStyle);
			}
		}
		int? GetNextPageBreakValue(int? predValue, IEnumerator pbEnumerator, int offset, int height) {
			int? result = null;
			if(pbEnumerator.MoveNext())
				result = Convert.ToInt32(GraphicsUnitConverter.DocToDip((float)pbEnumerator.Current));
			if((predValue != null && predValue == result) || (result + offset < height))
				return GetNextPageBreakValue(result, pbEnumerator, offset, height);
			return result;
		}
		MultiColumnInfo GetNextMultiColumnValue(IEnumerator mcEnumerator) {
			MultiColumnInfo result = null;
			if(mcEnumerator.MoveNext()) {
				result = (MultiColumnInfo)mcEnumerator.Current;
				result.Start = Convert.ToInt32(GraphicsUnitConverter.DocToDip(result.Start));
				result.End = Convert.ToInt32(GraphicsUnitConverter.DocToDip(result.End));
			}
			return result;
		}
		bool CanExportAsPlain(ObjectInfo[] objectInfos) {
			if(objectInfos.Length != 1)
				return false;
			BrickViewData brickViewData = (BrickViewData)objectInfos[0].Object;
			if(brickViewData.TableCell is ImageBrick && (brickViewData.TableCell as ImageBrick).BackColor == Color.Transparent)
				return true;
			TextBrick textBrick = brickViewData.TableCell as TextBrick;
			if(textBrick != null && GraphicsConvertHelper.ToVertStringAlignment(textBrick.Style.TextAlignment) == StringAlignment.Near &&
				textBrick.Sides == BorderSide.None && textBrick.BackColor == Color.Transparent)
				return true;
			return false;
		}
		protected override void WritePageNumberingInfo() {
			if(!string.IsNullOrEmpty(PageNumberingInfo))
				writer.WriteLine(PageNumberingInfo);
		}
		protected override void WriteDocumentHeaderFooter() {
			System.Drawing.Printing.Margins minMargins = RtfExportContext.PrintingSystem.PageSettings.MinMargins;
			if(emptyFirstPageHeaderFooter)
				writer.WriteLine(RtfTags.SpecialFirstPageHeaderFooter);
			if(headerChunkData != null) {
				writer.WriteLine(String.Format(RtfTags.HeaderY, ConvertToTwips(minMargins.Top, GraphicsDpi.HundredthsOfAnInch)));
				writer.Write("{");
				writer.Write(RtfTags.Header);
				writer.Write(headerChunkData.GetResultContent().ToString());
				writer.WriteLine("}");
			}
			if(footerChunkData != null) {
				writer.WriteLine(String.Format(RtfTags.FooterY, footerY + ConvertToTwips(minMargins.Bottom, GraphicsDpi.HundredthsOfAnInch)));
				writer.Write("{");
				writer.Write(RtfTags.Footer);
				writer.Write(footerChunkData.GetResultContent().ToString());
				writer.WriteLine("}");
			}
		}
		protected override void SetCellUnion() {
			currentChunkData.SetCellUnion();
		}
		protected override void SetFrameText(string text) {
			SetContent(text);
			if(PrintingSettings.UseNewSingleFileRtfExport && !string.IsNullOrEmpty(text)) {
				currentChunkData.SetAutoHeightRows(CurrentRowIndex, CurrentRowSpan);
			}
		}
		protected override void SetContent(string content) {
			currentChunkData.SetContent(content);
		}
		protected override void SetImageContent(string content) {
			SetContent(content);
			if(PrintingSettings.UseNewSingleFileRtfExport) {
				currentChunkData.SetAutoHeightRows(CurrentRowIndex, CurrentRowSpan);
			}
		}
		protected override void WriteContent() {
			foreach(RtfChunkData data in mainChunkDataList) {
				writer.Write(data.GetResultContent().ToString());
			}
			writer.Write("}" + RtfTags.SectionEnd);
		}
		protected override void WriteHeader() {
			base.WriteHeader();
			WritePageBounds();
			WriteMargins();
			if(PrintingSettings.UseNewSingleFileRtfExport) {
				WritePageNumberingInfo();
				WriteDocumentHeaderFooter();
			}
			writer.WriteLine(RtfTags.NoGrowAutoFit);
			writer.WriteLine("{");
		}
		protected override void WritePageBounds() {
			writer.WriteLine(RtfTags.SectionDefault);
			if(RtfExportContext.PrintingSystem.PageSettings.Landscape)
				writer.WriteLine(RtfTags.PageLandscape);
			int width = ConvertToTwips(pageBounds.Width, GraphicsDpi.HundredthsOfAnInch);
			int height = ConvertToTwips(pageBounds.Height, GraphicsDpi.HundredthsOfAnInch);
			writer.WriteLine(String.Format(RtfTags.PageSize, width, height));
		}
		protected override void WriteMargins() {
			int marginLeft = ConvertToTwips(margins.Left, GraphicsDpi.HundredthsOfAnInch);
			int marginRight = ConvertToTwips(margins.Right, GraphicsDpi.HundredthsOfAnInch);
			int marginTop = ConvertToTwips(margins.Top, GraphicsDpi.HundredthsOfAnInch);
			int marginBottom = ConvertToTwips(margins.Bottom, GraphicsDpi.HundredthsOfAnInch);
			writer.WriteLine(String.Format(RtfTags.Margins, marginLeft, marginRight, marginTop, marginBottom));
		}
	}
}
