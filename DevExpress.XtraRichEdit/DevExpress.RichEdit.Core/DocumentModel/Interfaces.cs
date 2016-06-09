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
using System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Collections.Generic;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region IObjectMeasurer
	public interface IObjectMeasurer {
		bool TryAdjustEndPositionToFit(BoxInfo boxInfo, string text, FontInfo fontInfo, int maxWidth);
		void MeasureText(BoxInfo boxInfo, string text, FontInfo fontInfo);
		Size MeasureInlinePicture(InlinePictureRun pic);
		Size MeasureFloatingObject(FloatingObjectAnchorRun run);
		void BeginTextMeasure();
		void EndTextMeasure();
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model {
	#region IDocumentModelExporter
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDocumentModelExporter {
		void Export(TextRun run);
		void Export(InlineObjectRun run);
		void Export(InlinePictureRun run);
		void Export(ParagraphRun run);
		void Export(SectionRun run);
		void Export(FieldCodeStartRun run);
		void Export(FieldCodeEndRun run);
		void Export(FieldResultEndRun run);
		void Export(FootNoteRun run);
		void Export(EndNoteRun run);
		void Export(FloatingObjectAnchorRun run);
		void Export(CustomRun run);
		void Export(SeparatorTextRun run);
		void Export(DataContainerRun run);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region IDocumentLayoutExporter
	public interface IDocumentLayoutExporter {
		void ExportPage(Page page);
		void ExportParagraphFramePage(Page page, RectangleF pageClipBounds, bool exportContent);
		void ExportPageArea(PageArea pageArea);
		void ExportColumn(Column column);
		void ExportRow(Row row);
		void ExportTextBox(DevExpress.XtraRichEdit.Layout.TextBox box);
		void ExportSpecialTextBox(SpecialTextBox box);
		void ExportLayoutDependentTextBox(LayoutDependentTextBox box);
		void ExportHyphenBox(HyphenBox box);
		void ExportInlinePictureBox(InlinePictureBox box);
		void ExportSpaceBox(Box box);
		void ExportTabSpaceBox(TabSpaceBox box);
		void ExportNumberingListBox(NumberingListBox box);
		void ExportLineBreakBox(LineBreakBox box);
		void ExportPageBreakBox(PageBreakBox box);
		void ExportColumnBreakBox(ColumnBreakBox box);
		void ExportParagraphMarkBox(ParagraphMarkBox box);
		void ExportSectionMarkBox(SectionMarkBox box);
		void ExportLineNumberBox(LineNumberBox box);
		void ExportUnderlineBox(Row row, UnderlineBox underlineBox);
		void ExportStrikeoutBox(Row row, UnderlineBox strikeoutBox);
		void ExportErrorBox(ErrorBox errorBox);
		void ExportBookmarkStartBox(VisitableDocumentIntervalBox box);
		void ExportBookmarkEndBox(VisitableDocumentIntervalBox box);
		void ExportCommentStartBox(VisitableDocumentIntervalBox box);
		void ExportCommentEndBox(VisitableDocumentIntervalBox box);
		void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds);
		void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y);
		void ExportTableCell(TableCellViewInfo cell);
		void ExportTableRow(TableRowViewInfoBase row);
		void ExportCustomMarkBox(CustomMarkBox box);
		void ExportCustomRunBox(CustomRunBox box);
		void ExportSeparatorBox(SeparatorBox box);
		void ExportFloatingObjectBox(FloatingObjectBox box);
		void ExportParagraphFrameBox(ParagraphFrameBox box);
		void ExportDataContainerRunBox(DataContainerRunBox box);
		bool IsAnchorVisible(ITableCellVerticalAnchor anchor);
		bool IsTableRowVisible(TableRowViewInfoBase row);
	}
	#endregion
	#region ICustomMarkExporter
	public interface ICustomMarkExporter {
		void ExportCustomMarkBox(CustomMark customMark, Rectangle bounds);
		CustomMarkVisualInfoCollection CustomMarkVisualInfoCollection { get; }
	}
	#endregion
}
