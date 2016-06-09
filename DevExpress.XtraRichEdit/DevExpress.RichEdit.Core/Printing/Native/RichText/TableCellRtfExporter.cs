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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraRichEdit.Export;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraPrinting.Native.RichText {
	#region TableCellRtfExporter
	public class TableCellRtfExporter : RtfContentExporter {
		public TableCellRtfExporter(DocumentModel documentModel, RtfDocumentExporterOptions options, DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper rtfExportHelper)
			: base(documentModel, options, rtfExportHelper) {
		}
		protected override RtfParagraphPropertiesExporter CreateParagraphPropertiesExporter() {
			return new TableCellParagraphPropertiesExporter(DocumentModel, RtfExportHelper, RtfBuilder);
		}
	}
	#endregion
	public class TableCellParagraphPropertiesExporter : RtfParagraphPropertiesExporter {
		public TableCellParagraphPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		public override void ExportParagraphProperties(Paragraph paragraph, int tableNestingLevel) {
			RtfBuilder.WriteCommand(RtfTags.SuggestToTable);
			base.ExportParagraphProperties(paragraph, tableNestingLevel);
		}
	}
	#region TableFrameRtfExporter
	public class TableFrameRtfExporter : RtfContentExporter {
		public bool ExportAsNestedTable { get; set; }
		public RtfPageExportProvider.PrintingParagraphAppearance ParagraphAppearance { get; set; }
		public TableFrameRtfExporter(DocumentModel documentModel, RtfDocumentExporterOptions options, DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper rtfExportHelper)
			: base(documentModel, options, rtfExportHelper) {
				ParagraphAppearance = new RtfPageExportProvider.PrintingParagraphAppearance();
		}
		protected internal override void StartNewParagraph(Paragraph paragraph, int tableNestingLevel) {
			if (ExportAsNestedTable && tableNestingLevel == 0)
				tableNestingLevel = 1; 
			SetParagraphStyle();
			if(paragraph.IsInList())
				WriteAlternativeAndPlainText(paragraph);
			StartNewInnerParagraph(paragraph, tableNestingLevel);
		}
		void SetParagraphStyle() {
			RtfBuilder.WriteCommand(RtfExportSR.ResetParagraphProperties);
			SetBounds();
			SetBorders();
			SetBackground();
		}
		void SetBounds() {
			if(ParagraphAppearance.Bounds != Rectangle.Empty) {
				RtfBuilder.WriteCommand(RtfTags.TextUnderneath);
				RtfBuilder.WriteCommand(String.Format(RtfTags.ObjectBounds, ParagraphAppearance.Bounds.Left, ParagraphAppearance.Bounds.Top, ParagraphAppearance.Bounds.Width, ParagraphAppearance.Bounds.Height));
				RtfBuilder.WriteCommand(RtfTags.RelativeFrameToPage);
			}
		}
		void SetBackground() {
			string test = String.Format(RtfTags.BackgroundPatternColor, ParagraphAppearance.BackgroundColorIndex);
			RtfBuilder.WriteCommand(test);
		}
		void SetBorders() {
			BorderSide sides = ParagraphAppearance.BorderSides;
			if((sides & BorderSide.Top) != 0) {
				RtfBuilder.WriteCommand(RtfTags.TopBorder);
				SetBorderStyle();
			}
			if((sides & BorderSide.Bottom) != 0) {
				RtfBuilder.WriteCommand(RtfTags.BottomBorder);
				SetBorderStyle();
			} 
			if((sides & BorderSide.Left) != 0) {
				RtfBuilder.WriteCommand(RtfTags.LeftBorder);
				SetBorderStyle();
			} 
			if((sides & BorderSide.Right) != 0) {
				RtfBuilder.WriteCommand(RtfTags.RightBorder);
				SetBorderStyle();
			}   
		}
		void SetBorderStyle() {
			int borderWidth = ParagraphAppearance.BorderWidth;
			const int maxSingleBorderWidth = 75;
			if(borderWidth <= maxSingleBorderWidth)
				RtfBuilder.WriteCommand(RtfTags.SingleBorderWidth);
			else
				RtfBuilder.WriteCommand(RtfTags.DoubleBorderWidth);
			RtfBuilder.WriteCommand(String.Format(RtfTags.BorderWidth, borderWidth));
			RtfBuilder.WriteCommand(String.Format(RtfTags.BorderColor, ParagraphAppearance.BorderColorIndex));
		}
		protected override bool SuppressExportLastParagraph(Paragraph paragraph) {
			if (ExportAsNestedTable)
				return true;
			return base.SuppressExportLastParagraph(paragraph);
		}
		protected override ParagraphIndex ExportRtfTable(Table table) {
			RtfBuilder.OpenGroup();
			RtfTableExporter exporter = new RtfTableExporter(this);
			exporter.ExportAsNestedTable = ExportAsNestedTable;
			ParagraphIndex paragraphIndex = exporter.Export(table);
			RtfBuilder.CloseGroup();
			return paragraphIndex;
		}
		protected internal override RtfNumberingListExporter CreateNumberingListExporter(RtfContentExporter exporter) {
			return new XRRtfNumberingListExporter(exporter);
		}
		protected override RtfParagraphPropertiesExporter CreateParagraphPropertiesExporter() {
			return new XRRtfParagraphPropertiesExporter(DocumentModel, RtfExportHelper, RtfBuilder);
		}
		protected override RtfStyleExporter CreateStyleExporter() {
			return new XRRtfStyleExporter(PieceTable.DocumentModel, CreateRtfBuilder(), RtfExportHelper, Options);
		}
	}
	class XRRtfStyleExporter : RtfStyleExporter {
		public XRRtfStyleExporter(DocumentModel documentModel, RtfBuilder rtfBuilder, IRtfExportHelper rtfExportHelper, RtfDocumentExporterOptions options) : 
			base(documentModel, rtfBuilder, rtfExportHelper, options) {
		}
		protected override int GetListId(NumberingListIndex index) {
			int id;
			if(!((DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper)RtfExportHelper).ListCollectionIndex.TryGetValue(DocumentModel.NumberingLists[index], out id))
				id = base.GetListId(index);
			return id;
		}
	}
	class XRRtfParagraphPropertiesExporter : RtfParagraphPropertiesExporter {
		public XRRtfParagraphPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		protected override void WriteParagraphListIndex(NumberingListIndex index) {
			int id;
			if(!((DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper)RtfExportHelper).ListCollectionIndex.TryGetValue(DocumentModel.NumberingLists[index], out id))
				id = DocumentModel.NumberingLists[index].Id;
			RtfBuilder.WriteCommand(RtfExportSR.ListIndex, id);
		}
	}
	class XRRtfNumberingListExporter : RtfNumberingListExporter {
		public XRRtfNumberingListExporter(RtfContentExporter rtfExporter):base(rtfExporter) {
		}
		protected internal override void WriteListOverrideId(NumberingList numberingList) {
			int index = RtfExporter.RtfExportHelper.ListOverrideCollection.Count;
			RtfBuilder.WriteCommand(RtfExportSR.ListIndex, index + 1);
			((DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper)RtfExporter.RtfExportHelper).ListCollectionIndex.Add(numberingList, index + 1);
		}
	}
	#endregion
}
