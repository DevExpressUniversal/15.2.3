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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
#if !SL
using System.Drawing.Drawing2D;
#else
using System.Windows.Media;
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Drawing2D;
using Brush = System.Windows.Media.Brush;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region GraphicsDocumentLayoutExporterWhitespaceStrategy (abstract class)
	public abstract class GraphicsDocumentLayoutExporterWhitespaceStrategy {
		readonly GraphicsDocumentLayoutExporter exporter;
		protected GraphicsDocumentLayoutExporterWhitespaceStrategy(GraphicsDocumentLayoutExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		protected internal GraphicsDocumentLayoutExporter Exporter { get { return exporter; } }
		public abstract void ExportSpaceBox(Box box);
		public abstract void ExportParagraphMarkBox(ParagraphMarkBox box);
		public abstract void ExportSectionMarkBox(SectionMarkBox box);
		public abstract void ExportColumnBreakBox(ColumnBreakBox box);
		public abstract void ExportPageBreakBox(PageBreakBox box);
		public abstract void ExportTabSpaceBox(TabSpaceBox box);
		public abstract void ExportLineBreakBox(LineBreakBox box);
		public abstract void ExportSeparatorBox(SeparatorBox box);
		public abstract void ExportNumberingListBoxSeparator(NumberingListBoxWithSeparator numberingListBoxWithSeparator);
	}
	#endregion
	#region GraphicsDocumentLayoutExporterShowWhitespaceStrategy
	public class GraphicsDocumentLayoutExporterShowWhitespaceStrategy : GraphicsDocumentLayoutExporterWhitespaceStrategy {
		public GraphicsDocumentLayoutExporterShowWhitespaceStrategy(GraphicsDocumentLayoutExporter exporter)
			: base(exporter) {
		}
		public override void ExportSpaceBox(Box spaceBox) {
			Rectangle textBounds = Exporter.GetDrawingBounds(spaceBox.Bounds);
			if (Exporter.IsValidBounds(textBounds)) {
				string text = Exporter.GetBoxText(spaceBox);
				text = new String(Characters.MiddleDot, text.Length);
				Exporter.ExportSpaceBoxCore(spaceBox, textBounds, text);
			}
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			Exporter.ExportParagraphMarkBoxCore(box);
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
			Exporter.ExportSectionMarkBoxCore(box);
		}
		public override void ExportColumnBreakBox(ColumnBreakBox box) {
			Exporter.ExportColumnBreakBoxCore(box);
		}
		public override void ExportPageBreakBox(PageBreakBox box) {
			Exporter.ExportPageBreakBoxCore(box);
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			Exporter.Adapter.ExportTabSpaceBoxCore(Exporter, box);
		}
		public override void ExportLineBreakBox(LineBreakBox box) {
			Exporter.Adapter.ExportLineBreakBoxCore(Exporter, box);
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
			Exporter.Adapter.ExportSeparatorBoxCore(Exporter, box);
		}
		public override void ExportNumberingListBoxSeparator(NumberingListBoxWithSeparator numberingListBoxWithSeparator) {
			TabSpaceBox tabSpaceBox = numberingListBoxWithSeparator.SeparatorBox as TabSpaceBox;
			if (tabSpaceBox != null && !tabSpaceBox.Bounds.IsEmpty) 
				ExportTabSpaceBox(tabSpaceBox);
		}
	}
	#endregion
	#region GraphicsDocumentLayoutExporterHideWhitespaceStrategy
	public class GraphicsDocumentLayoutExporterHideWhitespaceStrategy : GraphicsDocumentLayoutExporterShowWhitespaceStrategy {
		public GraphicsDocumentLayoutExporterHideWhitespaceStrategy(GraphicsDocumentLayoutExporter exporter)
			: base(exporter) {
		}
		FormattingMarkVisibilityOptions FormattingMarkVisibility { get { return Exporter.DocumentModel.FormattingMarkVisibilityOptions; } }
		public override void ExportSpaceBox(Box box) {
			if (FormattingMarkVisibility.Space != RichEditFormattingMarkVisibility.Visible)
				return;
			base.ExportSpaceBox(box);
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			if (FormattingMarkVisibility.ParagraphMark != RichEditFormattingMarkVisibility.Visible)
				return;
			base.ExportParagraphMarkBox(box);
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
		}
		public override void ExportColumnBreakBox(ColumnBreakBox box) {
		}
		public override void ExportPageBreakBox(PageBreakBox box) {
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			if (FormattingMarkVisibility.TabCharacter != RichEditFormattingMarkVisibility.Visible)
				return;
			base.ExportTabSpaceBox(box);
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
			if (FormattingMarkVisibility.Separator != RichEditFormattingMarkVisibility.Visible)
				return;
			base.ExportSeparatorBox(box);
		}
		public override void ExportLineBreakBox(LineBreakBox box) {
		}
		public override void ExportNumberingListBoxSeparator(NumberingListBoxWithSeparator numberingListBoxWithSeparator) {
			base.ExportNumberingListBoxSeparator(numberingListBoxWithSeparator);
		}
	}
	#endregion
	#region MarkBoxExporterBase (abstract class)
	public abstract class MarkBoxExporterBase {
		#region Fields
		const int paddingsWidth = 60;
		const int spacingWidth = 6;
		readonly GraphicsDocumentLayoutExporter exporter;
		#endregion
		protected MarkBoxExporterBase(GraphicsDocumentLayoutExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		#region Properties
		public GraphicsDocumentLayoutExporter Exporter { get { return exporter; } }
		public DocumentModel DocumentModel { get { return Exporter.DocumentModel; } }
		public PieceTable PieceTable { get { return Exporter.PieceTable; } }
		public abstract string Text { get; }
		#endregion
		public void Export(Box box, Rectangle boxBounds) {
			int fontIndex = DocumentModel.FontCache.CalcFontIndex("Arial", 16, false, false, CharacterFormattingScript.Normal, false, false);
			FontInfo fontInfo = DocumentModel.FontCache[fontIndex];
			Size textSize = DocumentModel.FontCache.Measurer.MeasureString(Text, fontInfo);
			Rectangle textBounds = boxBounds;
			textBounds.X += (textBounds.Width - textSize.Width) / 2;
			textBounds.Width = textSize.Width;
			textBounds.Y += (textBounds.Height - textSize.Height) / 2;
			Color foreColor = Exporter.GetActualColor(box.GetActualForeColor(PieceTable, Exporter.TextColors, exporter.GetBackColor(textBounds)));
			if (textBounds.Width + paddingsWidth < boxBounds.Width) {
				Exporter.Painter.DrawString(Text, fontInfo, foreColor, textBounds);
				Rectangle lineBounds = boxBounds;
				lineBounds.Y += lineBounds.Height / 2;
				lineBounds.Height = 1;
				Rectangle leftLineBounds = lineBounds;
				leftLineBounds.Width = textBounds.Left - lineBounds.Left - spacingWidth;
				DrawLine(leftLineBounds, foreColor);
				Rectangle rightLineBounds = lineBounds;
				rightLineBounds.X = textBounds.Right + spacingWidth;
				rightLineBounds.Width = boxBounds.Right - textBounds.Right - spacingWidth;
				DrawLine(rightLineBounds, foreColor);
			}
			else {
				Rectangle lineBounds = boxBounds;
				lineBounds.Y += lineBounds.Height / 2;
				lineBounds.Height = 1;
				DrawLine(lineBounds, foreColor);
			}
		}
		protected internal abstract void DrawLine(Rectangle lineBounds, Color foreColor);
	}
	#endregion
	#region ColumnBreakMarkBoxExporter
	public class ColumnBreakMarkBoxExporter : MarkBoxExporterBase {
		public ColumnBreakMarkBoxExporter(GraphicsDocumentLayoutExporter exporter)
			: base(exporter) {
		}
		public override string Text { get { return "Column Break"; } }
		protected internal override void DrawLine(Rectangle lineBounds, Color foreColor) {
			Exporter.DrawPatternLine(lineBounds, foreColor, Exporter.HorizontalLinePainter.ColumnBreakPattern);
		}
	}
	#endregion
	#region PageBreakMarkBoxExporter
	public class PageBreakMarkBoxExporter : MarkBoxExporterBase {
		public PageBreakMarkBoxExporter(GraphicsDocumentLayoutExporter exporter)
			: base(exporter) {
		}
		public override string Text { get { return "Page Break"; } }
		protected internal override void DrawLine(Rectangle lineBounds, Color foreColor) {
			Exporter.DrawPatternLine(lineBounds, foreColor, Exporter.HorizontalLinePainter.PageBreakPattern);
		}
	}
	#endregion
	#region SectionBreakMarkBoxExporter
	public class SectionBreakMarkBoxExporter : MarkBoxExporterBase {
		public SectionBreakMarkBoxExporter(GraphicsDocumentLayoutExporter exporter)
			: base(exporter) {
		}
		public override string Text { get { return "Section Break"; } }
		protected internal override void DrawLine(Rectangle lineBounds, Color foreColor) {
			lineBounds.Offset(0, -3);
			Exporter.DrawPatternLine(lineBounds, foreColor, Exporter.HorizontalLinePainter.SectionStartPattern);
			lineBounds.Offset(0, 6);
			Exporter.DrawPatternLine(lineBounds, foreColor, Exporter.HorizontalLinePainter.SectionStartPattern);
		}
	}
	#endregion
	public abstract class GraphicsDocumentLayoutExporterAdapter {
		public abstract void ExportLineBoxCore<T>(GraphicsDocumentLayoutExporter exporter, IPatternLinePainter<T> linePainter, UnderlineBox lineBox, PatternLine<T> line, Color lineColor) where T : struct;
		public abstract void ExportInlinePictureBox(GraphicsDocumentLayoutExporter exporter, InlinePictureBox box);
		public abstract void ExportTabSpaceBoxCore(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box);
		public abstract void ExportSeparatorBoxCore(GraphicsDocumentLayoutExporter exporter, SeparatorBox box);
		public abstract void ExportLineBreakBoxCore(GraphicsDocumentLayoutExporter exporter, LineBreakBox box);
		public abstract void ExportTabLeader(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box);
		public abstract void ExportFloatingObjectPicture(GraphicsDocumentLayoutExporter exporter, FloatingObjectBox box, PictureFloatingObjectContent pictureContent);
	}
	#region GraphicsDocumentLayoutExporter
	public class GraphicsDocumentLayoutExporter : BoundedDocumentLayoutExporter {
		#region Fields
		readonly Painter painter;
		readonly GraphicsDocumentLayoutExporterAdapter adapter;
		GraphicsDocumentLayoutExporterWhitespaceStrategy whitespaceStrategy;
		bool hidePartiallyVisibleRow;
		bool drawInactivePieceTableWithDifferentColor = true;
		#endregion
		public GraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: this(documentModel, painter, adapter, bounds, false, textColors) {
		}
		public GraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, bool hidePartiallyVisibleRow, TextColors textColors)
			: base(documentModel, bounds, painter, textColors) {
			Guard.ArgumentNotNull(painter, "painter");
			Guard.ArgumentNotNull(adapter, "adapter");
			this.painter = painter;
			this.adapter = adapter;
			this.hidePartiallyVisibleRow = hidePartiallyVisibleRow;
			this.ShowWhitespace = false;
		}
		public override Painter Painter { get { return painter; } }
		protected internal bool HidePartiallyVisibleRow { get { return hidePartiallyVisibleRow; } set { hidePartiallyVisibleRow = value; } }
		public override bool ShowWhitespace {
			get { return base.ShowWhitespace; }
			set {
				base.ShowWhitespace = value;
				if (value)
					this.whitespaceStrategy = new GraphicsDocumentLayoutExporterShowWhitespaceStrategy(this);
				else
					this.whitespaceStrategy = new GraphicsDocumentLayoutExporterHideWhitespaceStrategy(this);
			}
		}
		public GraphicsDocumentLayoutExporterAdapter Adapter { get { return adapter; } }
		public bool DrawInactivePieceTableWithDifferentColor { get { return drawInactivePieceTableWithDifferentColor; } set { drawInactivePieceTableWithDifferentColor = value; } }
		protected internal override RichEditPatternLinePainter CreateHorizontalLinePainter(IPatternLinePaintingSupport linePaintingSupport) {
			return new RichEditHorizontalPatternLinePainter(linePaintingSupport, DocumentModel.LayoutUnitConverter);
		}
		protected internal override RichEditPatternLinePainter CreateVerticalLinePainter(IPatternLinePaintingSupport linePaintingSupport) {
			return new RichEditVerticalPatternLinePainter(linePaintingSupport, DocumentModel.LayoutUnitConverter);
		}
		protected internal virtual Color GetActualColor(Color color) {
			if (!ShouldGrayContent())
				return color;
			else
				return DXColor.Blend(DXColor.FromArgb(128, color.R, color.G, color.B), DXColor.White);
		}
		protected internal virtual PieceTable GetEffectivePieceTable(PieceTable pieceTable) {
			if (pieceTable.IsTextBox) {
				TextBoxContentType contentType = (TextBoxContentType)pieceTable.ContentType;
				if (contentType.AnchorRun == null)
					return pieceTable;
				return contentType.AnchorRun.PieceTable;
			}
			else if (pieceTable.IsComment) {
				CommentContentType commentContent = pieceTable.ContentType as CommentContentType;
				return commentContent.ReferenceComment.PieceTable;
			}
			return pieceTable;
		}
		protected internal virtual bool ShouldGrayContent() {
			PieceTable pieceTable = GetEffectivePieceTable(PieceTable);
			PieceTable activePieceTable = GetEffectivePieceTable(DocumentModel.ActivePieceTable);
			return !Object.ReferenceEquals(pieceTable, activePieceTable) && DrawInactivePieceTableWithDifferentColor;
		}
		#region IDocumentLayoutExporter implementation
		public override void ExportRow(Row row) {
			if (ShouldExportRow(row))
				base.ExportRow(row);
		}
		protected override bool ShouldExportRowBackground(Row row) {
			if (row.InnerHighlightAreas != null && row.InnerHighlightAreas.Count > 0)
				return true;
			if (row.InnerFieldHighlightAreas != null && row.InnerFieldHighlightAreas.Count > 0)
				return true;
			if (row.InnerRangePermissionHighlightAreas != null && row.InnerRangePermissionHighlightAreas.Count > 0)
				return true;
			if (row.InnerCommentHighlightAreas != null && row.InnerCommentHighlightAreas.Count > 0)
				return true;
			return false;
		}
		protected internal override bool ShouldExportRow(Row row) {
			if (hidePartiallyVisibleRow) {
				Rectangle rowBounds = CalcRowContentBounds(row);
				if (GetDrawingBounds(rowBounds).Bottom > Bounds.Bottom)
					return false;
			}
			if (!VisibleBounds.IsEmpty) {
				Rectangle rowBounds = CalcRowContentBounds(row);
				Rectangle drawingBounds = GetDrawingBounds(rowBounds);
				if (drawingBounds.Bottom < VisibleBounds.Top || drawingBounds.Top > VisibleBounds.Bottom)
					return false;
			}
			return true;
		}
		public override bool IsTableRowVisible(TableRowViewInfoBase row) {
			if (VisibleBounds.IsEmpty)
				return true;
			if (IsRowContainsVerticalMergingCell(row.Row)) 
				return true;
			ITableCellVerticalAnchor topAnchor = row.TopAnchor;
			ITableCellVerticalAnchor bottomAnchor = row.BottomAnchor;
			int maxTop = Math.Max(topAnchor.VerticalPosition, VisibleBounds.Top);
			int minBottom = Math.Min(bottomAnchor.VerticalPosition + bottomAnchor.BottomTextIndent, VisibleBounds.Bottom);
			return maxTop <= minBottom;
		}
		protected internal virtual bool IsRowContainsVerticalMergingCell(TableRow row) {
			TableCellCollection modelCells = row.Cells;
			int modelCellCount = modelCells.Count;
			for (int i = 0; i < modelCellCount; i++) {
				if (modelCells[i].VerticalMerging == MergingState.Restart)
					return true;
			}
			return false;
		}
		public override bool IsAnchorVisible(ITableCellVerticalAnchor anchor) {
			if (VisibleBounds.IsEmpty)
				return true;
			if (anchor.VerticalPosition + anchor.BottomTextIndent < VisibleBounds.Top)
				return false;
			if (anchor.VerticalPosition > VisibleBounds.Bottom)
				return false;
			return true;
		}
		protected override void ExportBackground() {
			ExportFieldsHighlighting();
			base.ExportBackground();
		}
		protected virtual void ExportFieldsHighlighting() {
			ExportHighlighting(CurrentRow.InnerFieldHighlightAreas);
			ExportHighlighting(CurrentRow.InnerRangePermissionHighlightAreas);
			ExportHighlighting(CurrentRow.InnerCommentHighlightAreas);
		}
		public override void ExportHighlightArea(HighlightArea area) {
			Painter.FillRectangle(GetActualColor(area.Color), Rectangle.Round(GetDrawingBounds(area.Bounds)));
		}
		public override void ExportTextBox(TextBox box) {
			ExportTextBoxCore(box);
		}
		public override void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
			ExportTextBoxCore(box);
		}
		public override void ExportNumberingListBox(NumberingListBox box) {
			ExportNumberingListBoxCore(box);
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
			TextRunBase run = box.GetRun(PieceTable.DocumentModel.MainPieceTable);
			if (!DXColor.IsTransparentOrEmpty(run.BackColor))
				ExportHighlightArea(new HighlightArea(run.BackColor, box.Bounds));
			ExportTextBoxCoreNoCheckBoundsValidity(box);
		}
		public override void ExportSpaceBox(Box box) {
			whitespaceStrategy.ExportSpaceBox(box);
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
			whitespaceStrategy.ExportSeparatorBox(box);
		}
		public override void ExportInlinePictureBox(InlinePictureBox box) {
			adapter.ExportInlinePictureBox(this, box);
		}
		protected internal override bool BeforeExportRotatedContent(FloatingObjectBox box) {
			Rectangle bounds = GetDrawingBounds(box.Bounds);
			Point center = RectangleUtils.CenterPoint(bounds);
			bool transformApplied = Painter.TryPushRotationTransform(center, DocumentModel.GetBoxEffectiveRotationAngleInDegrees(box));
			Painter.PushSmoothingMode(transformApplied);
			return transformApplied;
		}
		protected internal override void AfterExportRotatedContent(bool transformApplied) {
			Painter.PopSmoothingMode();
			if (transformApplied)
				Painter.PopTransform();
		}
		protected internal override bool ExportRotatedContent(FloatingObjectBox box) {
			bool transformApplied = BeforeExportRotatedContent(box);
			try {
				return base.ExportRotatedContent(box);
			}
			finally {
				AfterExportRotatedContent(transformApplied);
			}
		}
		public override void ExportFloatingObjectShape(FloatingObjectBox box, Shape shape) {
			Rectangle contentBounds = GetDrawingBounds(box.ContentBounds);
			if (!IsValidBounds(contentBounds))
				return;
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			TextRunBase currentRun = box.GetRun(PieceTable);
			Color fillColor = run.Shape.FillColor;
			if (!DXColor.IsTransparentOrEmpty(fillColor))
				Painter.FillRectangle(GetActualColor(fillColor), contentBounds);
			int penWidth = GetShapeOutlinePenWidth(run, box);
			if (penWidth >= 0) {
				Color outlineColor = GetActualColor(run.Shape.OutlineColor);
				using (Pen pen = new Pen(outlineColor, penWidth)) {
					Rectangle shapeBounds = GetDrawingBounds(box.Bounds);
					pen.Alignment = PenAlignment.Inset;
					Painter.DrawRectangle(pen, shapeBounds);
				}
			}
		}
		public override void ExportParagraphFrameShape(ParagraphFrameBox box, Shape shape) {
			Rectangle contentBounds = box.ContentBounds;
			Rectangle actualBounds = box.ActualSizeBounds;
			Paragraph firstParagraph = box.GetParagraph();
			ParagraphProperties boxParagraphProperties = firstParagraph.ParagraphProperties;
			if (box.FrameProperties == null || box.DocumentLayout == null) {
				Color backColor = firstParagraph.BackColor;
				SetBackColor(backColor, contentBounds);
				FillRectangle(backColor, contentBounds);
				DrawParagraphBordersWithCorners(contentBounds, boxParagraphProperties);
				return;
			}
			RowCollection rows = box.DocumentLayout.Pages.Last.Areas.Last.Columns.Last.Rows;
			bool isContainsTable = box.DocumentLayout.Pages.Last.Areas.Last.Columns.Last.Tables.Count > 0;
			DrawParagraphBackground(actualBounds, rows);
			if (isContainsTable)
				DrawParagraphBordersWithoutTableBounds(actualBounds, boxParagraphProperties, rows);
			else
				DrawParagraphBordersWithCorners(actualBounds, boxParagraphProperties);
		}
		private void FillRectangle(Color fillColor, Rectangle actualContentBounds) {
			if (!DXColor.IsTransparentOrEmpty(fillColor))
				Painter.FillRectangle(GetActualColor(fillColor), actualContentBounds);
		}
		private void DrawParagraphBordersWithCorners(Rectangle actualContentBounds, ParagraphProperties boxParagraphProperties) {
			DrawParagraphBorders(actualContentBounds, boxParagraphProperties);
			DrawParagraphBordersCorners(actualContentBounds, boxParagraphProperties);
		}
		private static int GetActualBoxHeight(Rectangle actualBounds, RowCollection rows, Row currentRow, Rectangle rowBounds) {
			if (currentRow == rows.First && currentRow == rows.Last)
				return actualBounds.Height;
			if (currentRow == rows.First)
				return rowBounds.Height + rowBounds.Y - actualBounds.Y;
			if (currentRow == rows.Last)
				return actualBounds.Bottom - rowBounds.Y;
			return rowBounds.Height;
		}
		private void DrawParagraphBackground(Rectangle actualBounds, RowCollection rows) {
			for (int index = 0; index < rows.Count; index++) {
				Row currentRow = rows[index];
				if (!currentRow.Paragraph.IsInCell()) {
					Rectangle rowBounds = currentRow.Bounds;
					int y = currentRow != rows.First ? rowBounds.Y : actualBounds.Y;
					int height = GetActualBoxHeight(actualBounds, rows, currentRow, rowBounds);
					Rectangle actualParagraphBounds = new Rectangle(actualBounds.X, y, actualBounds.Width, height);
					FillRectangle(currentRow.Paragraph.BackColor, actualParagraphBounds);
				}
			}
		}
		private void DrawParagraphBordersWithoutTableBounds(Rectangle actualBounds, ParagraphProperties boxParagraphProperties, RowCollection rows) {
			for (int index = 0; index < rows.Count; index++) {
				Row currentRow = rows[index];
				if (!currentRow.Paragraph.IsInCell()) {
					Rectangle rowBounds = currentRow.Bounds;
					int y = currentRow != rows.First ? rowBounds.Y : actualBounds.Y;
					int height = 0;
					do {
						height += GetActualBoxHeight(actualBounds, rows, rows[index], rows[index].Bounds);
						if (index < rows.Count - 1)
							index++;
					}
					while (index < rows.Count - 1 && !rows[index].Paragraph.IsInCell());
					Rectangle actualParagraphBounds = new Rectangle(actualBounds.X, y, actualBounds.Width, height);
					DrawParagraphBordersWithCorners(actualParagraphBounds, boxParagraphProperties);
				}
			}
		}
		private void DrawParagraphBorders(Rectangle contentBounds, ParagraphProperties paragraphProperties) {
			DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
			BorderInfo leftBorder = paragraphProperties.LeftBorder;
			BorderInfo rightBorder = paragraphProperties.RightBorder;
			BorderInfo topBorder = paragraphProperties.TopBorder;
			BorderInfo bottomBorder = paragraphProperties.BottomBorder;
			TableCellVerticalBorderViewInfo borderViewInfo = new TableCellVerticalBorderViewInfo(null, rightBorder, 0, 0, converter);
			Rectangle rightBorderBounds = new Rectangle(contentBounds.Right, contentBounds.Y, 0, contentBounds.Height);
			ExportTableBorder(borderViewInfo, rightBorderBounds);
			borderViewInfo = new TableCellVerticalBorderViewInfo(null, leftBorder, 0, 0, converter);
			Rectangle leftBorderBounds = new Rectangle(contentBounds.Left, contentBounds.Y, 0, contentBounds.Height);
			ExportTableBorder(borderViewInfo, leftBorderBounds);
			SingleLineCornerViewInfo leftCorner = new SingleLineCornerViewInfo(leftBorder, rightBorder, topBorder, bottomBorder, 0.0f, 0.0f, CornerViewInfoType.OuterVerticalStart);
			SingleLineCornerViewInfo rightCorner = new SingleLineCornerViewInfo(leftBorder, rightBorder, topBorder, bottomBorder, 0.0f, 0.0f, CornerViewInfoType.OuterVerticalEnd);
			ParagraphHorizontalBorderViewInfo horizontalBorderViewInfo = new ParagraphHorizontalBorderViewInfo(topBorder, converter, leftCorner, rightCorner);
			Rectangle topBorderBounds = new Rectangle(contentBounds.X, contentBounds.Top, contentBounds.Width, 0);
			ExportTableBorder(horizontalBorderViewInfo, topBorderBounds);
			Rectangle bottomBorderBounds = new Rectangle(contentBounds.X, contentBounds.Bottom, contentBounds.Width, 0);
			horizontalBorderViewInfo = new ParagraphHorizontalBorderViewInfo(bottomBorder, converter, leftCorner, rightCorner);
			ExportTableBorder(horizontalBorderViewInfo, bottomBorderBounds);
		}
		private void DrawParagraphBordersCorners(Rectangle contentBounds, ParagraphProperties paragraphProperties) {
			DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
			BorderInfo leftBorder = paragraphProperties.LeftBorder;
			BorderInfo rightBorder = paragraphProperties.RightBorder;
			BorderInfo topBorder = paragraphProperties.TopBorder;
			BorderInfo bottomBorder = paragraphProperties.BottomBorder;
			CornerViewInfoBase topLeftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, null, null, topBorder, leftBorder, 0);
			ExportTableBorderCorner(topLeftCorner, contentBounds.Left, contentBounds.Top);
			CornerViewInfoBase topRightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, topBorder, null, null, rightBorder, 0);
			ExportTableBorderCorner(topRightCorner, contentBounds.Right, contentBounds.Top);
			CornerViewInfoBase bottomLeftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, null, leftBorder, bottomBorder, null, 0);
			ExportTableBorderCorner(bottomLeftCorner, contentBounds.Left, contentBounds.Bottom);
			CornerViewInfoBase bottomRightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, bottomBorder, rightBorder, null, null, 0);
			ExportTableBorderCorner(bottomRightCorner, contentBounds.Right, contentBounds.Bottom);
		}
		public override void ExportFloatingObjectPicture(FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
			adapter.ExportFloatingObjectPicture(this, box, pictureContent);
		}
		public override void ExportFloatingObjectTextBox(FloatingObjectBox box, TextBoxFloatingObjectContent textBoxContent, DocumentLayout textBoxDocumentLayout) {
			ExportCompositeObject(GetDrawingBounds(box.ContentBounds), textBoxContent.TextBox.PieceTable, (oldClipRect, clipRect) => { textBoxDocumentLayout.Pages.First.ExportTo(this); });
		}
		public override void ExportParagraphFrameTextBox(ParagraphFrameBox box, DocumentLayout textBoxDocumentLayout) {
			ExportCompositeObject(GetDrawingBounds(box.ContentBounds), this.PieceTable, (oldClipRect, clipRect) => { this.ExportParagraphFramePage(textBoxDocumentLayout.Pages.First, oldClipRect, clipRect != RectangleF.Empty); });
		}
		internal override bool BeginExportCompositeObject(Rectangle bounds, PieceTable pieceTable, ref RectangleF oldClipRect, ref RectangleF clipRect, ref PieceTable oldPieceTable) {
			if (!IsValidBounds(bounds)) {
				return false;
			}
			oldClipRect = GetClipBounds();
			clipRect = IntersectClipBounds(oldClipRect, bounds);
			if (clipRect == RectangleF.Empty)
				return false;
			ApplyClipBounds(clipRect);
			oldPieceTable = this.PieceTable;
			this.PieceTable = pieceTable;
			return true;
		}
		internal override void EndExportCompositeObject(RectangleF oldClipRect, PieceTable oldPieceTable) {
			this.PieceTable = oldPieceTable;
			RestoreClipBounds(oldClipRect);
		}
		protected override void ApplyClipBoundsCore(RectangleF bounds) {
			Painter.ApplyClipBounds(bounds);
		}
		protected override void RestoreClipBoundsCore(RectangleF bounds) {
			Painter.RestoreClipBounds(bounds);
		}
		private void ExportCompositeObject(Rectangle bounds, PieceTable pieceTable, Action<RectangleF, RectangleF> action) {
			RectangleF oldClipRect = RectangleF.Empty;
			RectangleF clipRect = RectangleF.Empty;
			PieceTable oldPieceTable = null;
			if (!BeginExportCompositeObject(bounds, pieceTable, ref oldClipRect, ref clipRect, ref oldPieceTable))
				return;
			try {
				action(oldClipRect, clipRect);
			}
			finally {
				EndExportCompositeObject(oldClipRect, oldPieceTable);
			}
		}
		public override void ExportComment(CommentViewInfo commentViewInfo) {
			ExportCompositeObject(commentViewInfo.ContentBounds, commentViewInfo.Comment.Content.PieceTable, (oldClipRect, clipRect) => { commentViewInfo.CommentDocumentLayout.Pages.First.ExportTo(this); });
		}
		protected internal virtual void ExportTextBoxCore(Box box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				string text = GetBoxText(box);
				ExportTextBoxCore(box, textBounds, text);
			}
		}
		protected internal virtual void ExportTextBoxCoreNoCheckBoundsValidity(Box box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			string text = GetBoxText(box);
			ExportTextBoxCore(box, textBounds, text);
		}
		protected internal virtual void ExportNumberingListBoxCore(Box box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				string text = GetBoxText(box);
				ExportTextBoxCore(box, textBounds, text);
				NumberingListBoxWithSeparator numberingListBoxWithSeparator = box as NumberingListBoxWithSeparator;
				if (numberingListBoxWithSeparator != null)
					whitespaceStrategy.ExportNumberingListBoxSeparator(numberingListBoxWithSeparator);
			}
		}
		protected internal virtual void ExportTextBoxCore(Box box, Rectangle textBounds, string text) {
			FontInfo fontInfo = box.GetFontInfo(PieceTable);
			Color foreColor = GetActualForeColor(box, textBounds);
			Painter.DrawString(text, fontInfo, foreColor, textBounds);
		}		
		protected internal Color GetActualForeColor(Box box, Rectangle bounds) {
			Color backColor = GetActualBackColor(bounds);
			return GetActualColor(box.GetActualForeColor(PieceTable, TextColors, backColor));
		}
		protected internal virtual void ExportSpaceBoxCore(Box box, Rectangle textBounds, string text) {
			FontInfo fontInfo = box.GetFontInfo(PieceTable);
			Color foreColor = GetActualColor(box.GetActualForeColor(PieceTable, TextColors, GetBackColor(textBounds)));
			Painter.DrawSpacesString(text, fontInfo, foreColor, textBounds);
		}
		public override void ExportHyphenBox(HyphenBox box) {
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			whitespaceStrategy.ExportTabSpaceBox(box);
			TabLeaderType leaderType = box.TabInfo.Leader;
			if (leaderType != TabLeaderType.None)
				adapter.ExportTabLeader(this, box);
		}
		protected internal virtual void ExportTabLeaderAsCharacterSequence(TabSpaceBox box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (!IsValidBounds(textBounds))
				return;
			ExportTextBoxCore(box, textBounds, GetTabLeaderText(box, textBounds));
		}
		protected internal virtual void ExportTabLeaderAsUnderline(TabSpaceBox box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (!IsValidBounds(textBounds))
				return;
			Underline underline = DocumentModel.UnderlineRepository.GetPatternLineByType(GetTabLeaderUnderlineType(box));
			UnderlineCalculator underlineCalculator = new UnderlineCalculator(PieceTable);
			UnderlineBox underlineBox = underlineCalculator.CreateTabLeaderUnderlineBox(box, textBounds, CurrentRow);
			if (ShouldCenterTabLeaderLineVertically(box))
				underlineCalculator.CenterTabLeaderUnderlineBoxVertically(underlineBox, textBounds);
			Color color = GetActualColor(box.GetActualUnderlineColor(PieceTable, TextColors, GetBackColor(textBounds)));
			adapter.ExportLineBoxCore<UnderlineType>(this, this, underlineBox, underline, color);
		}
		public override void ExportLineBreakBox(LineBreakBox box) {
			whitespaceStrategy.ExportLineBreakBox(box);
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			whitespaceStrategy.ExportParagraphMarkBox(box);
		}
		protected internal virtual void ExportParagraphMarkBoxCore(ParagraphMarkBox box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				string text = GetBoxText(box);
				TextRunBase run = box.GetRun(PieceTable);
				TableCell cell = run.Paragraph.GetCell();
				if (cell != null && run.Paragraph.Index == cell.EndParagraphIndex)
					text = new String(Characters.CurrencySign, text.Length);
				else
					text = new String(Characters.PilcrowSign, text.Length);
				ExportTextBoxCore(box, textBounds, text);
			}
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
			whitespaceStrategy.ExportSectionMarkBox(box);
		}
		protected internal virtual void ExportSectionMarkBoxCore(SectionMarkBox box) {
			SectionBreakMarkBoxExporter exporter = new SectionBreakMarkBoxExporter(this);
			ExportMarkBoxCore(box, exporter);
		}
		public override void ExportColumnBreakBox(ColumnBreakBox box) {
			whitespaceStrategy.ExportColumnBreakBox(box);
		}
		protected internal virtual void ExportColumnBreakBoxCore(ColumnBreakBox box) {
			ColumnBreakMarkBoxExporter exporter = new ColumnBreakMarkBoxExporter(this);
			ExportMarkBoxCore(box, exporter);
		}
		public override void ExportPageBreakBox(PageBreakBox box) {
			whitespaceStrategy.ExportPageBreakBox(box);
		}
		protected internal virtual void ExportPageBreakBoxCore(PageBreakBox box) {
			PageBreakMarkBoxExporter exporter = new PageBreakMarkBoxExporter(this);
			ExportMarkBoxCore(box, exporter);
		}
		protected internal virtual void ExportMarkBoxCore(Box box, MarkBoxExporterBase exporter) {
			Rectangle bounds = GetDrawingBounds(box.Bounds);
			if (!IsValidBounds(bounds))
				return;
			Rectangle boxBounds = new Rectangle(bounds.Left, CurrentRow.Bounds.Top, bounds.Width, CurrentRow.Height);
			exporter.Export(box, boxBounds);
		}
		public override void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds) {
			GraphicsDocumentLayoutExporterTableBorder tableBorderExporter = new GraphicsDocumentLayoutExporterTableBorder(DocumentModel, Painter, HorizontalLinePainter, VerticalLinePainter, cellBounds);
			BorderInfo borderBase = border.Border;
			DocumentModelUnitToLayoutUnitConverter converter = border.Converter;
			tableBorderExporter.ExportTableBorder(borderBase, GetDrawingBounds(cellBounds), converter, (ITableBorderViewInfoBase)border);
		}
		public override void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) {
			GraphicsDocumentLayoutExporterTableBorder tableBorderExporter = new GraphicsDocumentLayoutExporterTableBorder(DocumentModel, Painter, HorizontalLinePainter, VerticalLinePainter);
			tableBorderExporter.ExportTableBorderCorner(corner, x, y);
		}
		public override void ExportTableCell(TableCellViewInfo cell) {
			Rectangle rect = cell.GetBackgroundBounds();
			Color color = cell.Cell.GetActualBackgroundColor();
			if (!Object.Equals(color, DXColor.Empty))
				Painter.FillRectangle(color, GetDrawingBounds(rect));
			int cellInnerTablesCount = cell.InnerTables.Count;
			if (cellInnerTablesCount > 0)
				for (int i = 0; i < cellInnerTablesCount; i++) {
					cell.InnerTables[i].ExportBackground(this);
				}
		}
		public override void ExportTableRow(TableRowViewInfoBase row) {
			RectangleF oldClip = Painter.ClipBounds;
			try {
				Rectangle drawingBounds = GetDrawingBounds(row.GetBounds());
				ExcludeCellBounds(row.Cells, drawingBounds);
				Painter.FillRectangle(GetActualBackgroundColor(row), drawingBounds);
			}
			finally {
				Painter.ClipBounds = oldClip;
				Painter.ResetCellBoundsClip();
			}
		}
		void ExcludeCellBounds(TableCellViewInfoCollection cells, Rectangle rowBounds) {
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				Rectangle currentCellBounds = cells[i].GetBackgroundBounds();
				Painter.ExcludeCellBounds(currentCellBounds, rowBounds);
			}
		}
		Color GetActualBackgroundColor(TableRowViewInfoBase row) {
			TableProperties tablePropertiesException = row.Row.TablePropertiesException;
			if (tablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseBackgroundColor))
				return tablePropertiesException.BackgroundColor;
			return row.TableViewInfo.Table.BackgroundColor;
		}
		protected virtual TableCornerPainter GetBorderPainter(CornerViewInfoBase corner) {
			if (corner is NoneLineCornerViewInfo)
				return null;
			return new TableCornerPainter();
		}
		public override void ExportUnderlineBox(Row row, UnderlineBox underlineBox) {
			Box box = GetCharacterLineBoxByIndex(row, underlineBox.StartAnchorIndex);
			Color color = GetActualColor(box.GetActualUnderlineColor(PieceTable, TextColors, GetBackColor(GetDrawingBounds(underlineBox.UnderlineBounds))));
			Underline underline = DocumentModel.UnderlineRepository.GetPatternLineByType(box.GetFontUnderlineType(PieceTable));
			adapter.ExportLineBoxCore<UnderlineType>(this, this, underlineBox, underline, color);
		}
		public override void ExportStrikeoutBox(Row row, UnderlineBox StrikeoutBox) {
			Box box = GetCharacterLineBoxByIndex(row, StrikeoutBox.StartAnchorIndex);
			Color color = GetActualColor(box.GetActualStrikeoutColor(PieceTable, TextColors, GetBackColor(GetDrawingBounds(StrikeoutBox.UnderlineBounds))));
			Strikeout strikeout = DocumentModel.StrikeoutRepository.GetPatternLineByType(box.GetFontStrikeoutType(PieceTable));
			adapter.ExportLineBoxCore<StrikeoutType>(this, this, StrikeoutBox, strikeout, color);
		}
		public override void ExportErrorBox(ErrorBox errorBox) {
		}
		#endregion
		protected internal override RectangleF GetClipBounds() {
			return Painter.ClipBounds;
		}
		protected internal override void SetClipBounds(RectangleF clipBounds) {
			Painter.ClipBounds = clipBounds;
		}
		protected internal override RectangleF BeginHeaderFooterExport(RectangleF clipBounds) {
			return Painter.ApplyClipBounds(clipBounds);
		}
		protected internal override void EndHeaderFooterExport(RectangleF oldClipBounds) {
			Painter.RestoreClipBounds(oldClipBounds);
		}
		protected internal override void BeginExportTableCellContent(RectangleF clipBounds) {
			ApplyClipBounds(clipBounds);
		}
		protected internal override void EndExportTableContent(RectangleF oldClipBounds) {
			Painter.RestoreClipBounds(oldClipBounds);
		}
	}
	#endregion
	#region ScreenOptimizedGraphicsDocumentLayoutExporter
	public class ScreenOptimizedGraphicsDocumentLayoutExporter : GraphicsDocumentLayoutExporter {
		readonly int pixel;
		public ScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(documentModel, painter, adapter, bounds, textColors) {
			this.pixel = (int)Math.Ceiling((double)documentModel.LayoutUnitConverter.PixelsToLayoutUnits(2, painter.DpiY));
		}
		protected internal override void FinishExport() {
			Painter.FinishPaint();
		}
		protected internal virtual Brush CreatePlaceholderBrush(Color foreColor) {
#if !DXPORTABLE
#if !SL
			return new HatchBrush(HatchStyle.Percent50, DXColor.Transparent, foreColor);
#else
			return new SolidColorBrush(foreColor);
#endif
#else
			return new SolidBrush(foreColor);
#endif
		}
		protected internal virtual bool ShouldOptimizeBox(Box box) {
			return box.Bounds.Height < MinReadableTextHeight;
		}
		protected override void ExportRowStrikeoutBoxes() {
			if (!ShouldOptimizeBox(CurrentRow))
				base.ExportRowStrikeoutBoxes();
		}
		protected override void ExportRowUnderlineBoxes() {
			if (!ShouldOptimizeBox(CurrentRow))
				base.ExportRowUnderlineBoxes();
		}
		protected override void ExportRowErrorBoxes() {
			if (!ShouldOptimizeBox(CurrentRow))
				base.ExportRowErrorBoxes();
		}
		public override void ExportHyphenBox(HyphenBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportHyphenBox(box);
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			if (!ShouldOptimizeBox(box))
				base.ExportParagraphMarkBox(box);
		}
		public override void ExportSpaceBox(Box box) {
			if (!ShouldOptimizeBox(box))
				base.ExportSpaceBox(box);
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportSeparatorBox(box);
		}
		public override void ExportTextBox(TextBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportTextBox(box);
		}
		public override void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportTextBox(box);
		}
		public override void ExportNumberingListBox(NumberingListBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportNumberingListBox(box);
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
			if (ShouldOptimizeBox(box))
				ExportBoxOptimized(box);
			else
				base.ExportLineNumberBox(box);
		}
		protected internal virtual void ExportBoxOptimized(Box box) {
			Rectangle actualBounds = Rectangle.Inflate(box.Bounds, 0, -pixel);
#if !SL
			using (Brush brush = CreatePlaceholderBrush(GetActualColor(box.GetActualForeColor(PieceTable, TextColors, GetBackColor(actualBounds))))) {
#else
			Brush brush = CreatePlaceholderBrush(box.GetActualForeColor(PieceTable, TextColors, CurrentBackColor));
#endif
				if (actualBounds.Height <= 0)
					actualBounds.Height = pixel;
				Painter.FillRectangle(brush, actualBounds);
#if !SL
			}
#endif
		}
	}
#endregion
	public class ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers : ScreenOptimizedGraphicsDocumentLayoutExporter {
		public ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(documentModel, painter, adapter, bounds, textColors) {
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
		}
	}
	public class DraftScreenOptimizedGraphicsDocumentLayoutExporter : ScreenOptimizedGraphicsDocumentLayoutExporter {
		public DraftScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(documentModel, painter, adapter, bounds, textColors) {
		}
		protected internal override bool ShouldExportComments(Page page) {
			return false;
		}
		protected internal override bool ShouldExportComment(CommentViewInfo commentViewInfo) {
			return false;
		}
	}
	public class DraftScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers : ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers {
		public DraftScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(documentModel, painter, adapter, bounds, textColors) {
		}
		protected internal override bool ShouldExportComments(Page page) {
			return false;
		}
		protected internal override bool ShouldExportComment(CommentViewInfo commentViewInfo) {
			return false;
		}
	}
#region ITableBorderViewInfoBase
	public interface ITableBorderViewInfoBase {
		BorderInfo Border { get; }
		BorderTypes BorderType { get; }
		CornerViewInfoBase StartCorner { get; }
		CornerViewInfoBase EndCorner { get; }
		bool HasStartCorner { get; }
		bool HasEndCorner { get; }
	}
#endregion
#region GraphicsDocumentLayoutExporterTableBorder
	public class GraphicsDocumentLayoutExporterTableBorder {
#region Fields
		readonly Painter painter;
		readonly RichEditPatternLinePainter horizontalLinePainter;
		readonly RichEditPatternLinePainter verticalLinePainter;
		readonly DocumentModel documentModel;
		readonly Rectangle bounds;
#endregion
		public GraphicsDocumentLayoutExporterTableBorder(DocumentModel documentModel, Painter painter, RichEditPatternLinePainter horizontalLinePainter, RichEditPatternLinePainter verticalLinePainter, Rectangle bounds) {
			Guard.ArgumentNotNull(painter, "painter");
			Guard.ArgumentNotNull(horizontalLinePainter, "horizontalLinePainter");
			Guard.ArgumentNotNull(verticalLinePainter, "verticalLinePainter");
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(bounds, "bounds");
			this.painter = painter;
			this.horizontalLinePainter = horizontalLinePainter;
			this.verticalLinePainter = verticalLinePainter;
			this.documentModel = documentModel;
			this.bounds = bounds;
		}
		public GraphicsDocumentLayoutExporterTableBorder(DocumentModel documentModel, Painter painter, RichEditPatternLinePainter horizontalLinePainter, RichEditPatternLinePainter verticalLinePainter) {
			Guard.ArgumentNotNull(painter, "painter");
			Guard.ArgumentNotNull(horizontalLinePainter, "horizontalLinePainter");
			Guard.ArgumentNotNull(verticalLinePainter, "verticalLinePainter");
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.painter = painter;
			this.horizontalLinePainter = horizontalLinePainter;
			this.verticalLinePainter = verticalLinePainter;
			this.documentModel = documentModel;
		}
		public Painter Painter { get { return painter; } }
		protected virtual Point Offset { get { return bounds.Location; } }
		protected internal RichEditPatternLinePainter HorizontalLinePainter { get { return horizontalLinePainter; } }
		protected internal RichEditPatternLinePainter VerticalLinePainter { get { return verticalLinePainter; } }
		protected internal virtual TableBorderPainter GetBorderPainter(BorderInfo border, DocumentModelUnitToLayoutUnitConverter converter) {
			BorderLineStyle borderStyle = border.Style;
			if (borderStyle == BorderLineStyle.None || borderStyle == BorderLineStyle.Nil || borderStyle == BorderLineStyle.Disabled)
				return null;
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			int actualWidth = borderCalculator.GetActualWidth(border);
			float width = converter.ToLayoutUnits((float)actualWidth);
			float[] compoundArray = borderCalculator.GetDrawingCompoundArray(border);
			GraphicsPainterWrapper painterWrapper = new GraphicsPainterWrapper(Painter, HorizontalLinePainter, VerticalLinePainter);
			if (compoundArray.Length == 4)
				return new DoubleBorderPainter(painterWrapper, compoundArray, width);
			else if (compoundArray.Length == 6)
				return new TripleBorderPainter(painterWrapper, compoundArray, width);
			else
				return new SingleBorderPainter(painterWrapper, width, GetTableBorderLine(border.Style, documentModel));
		}
		public virtual void ExportTableBorder(BorderInfo border, Rectangle drawingBounds, DocumentModelUnitToLayoutUnitConverter converter, ITableBorderViewInfoBase viewInfo) {
			TableBorderPainter borderPainter = GetBorderPainter(border, converter);
			if (borderPainter != null) {
				borderPainter.DrawBorder(viewInfo, drawingBounds);
			}
		}
		protected virtual Underline GetTableBorderLine(BorderLineStyle borderLineStyle, DocumentModel documentModel) {
			if (borderLineStyle == BorderLineStyle.Single)
				return null;
			BorderLineRepository repository = documentModel.BorderLineRepository;
			return repository.GetCharacterLineByType(borderLineStyle);
		}
		public virtual void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) {
			TableCornerPainter borderPainter = GetBorderPainter(corner);
			if (borderPainter != null) {
				Point location = new Point(x, y);
				location.Offset(Offset);
				borderPainter.DrawCorner(new GraphicsPainterWrapper(Painter, HorizontalLinePainter, VerticalLinePainter), location.X, location.Y, corner);
			}
		}
		protected virtual TableCornerPainter GetBorderPainter(CornerViewInfoBase corner) {
			if (corner is NoneLineCornerViewInfo)
				return null;
			return new TableCornerPainter();
		}
	}
#endregion
}
