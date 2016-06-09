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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Export.PlainText {
	#region PlainTextExporter
	public class PlainTextExporter : DocumentModelExporter, IPlainTextExporter {
		readonly StringBuilder sb;
		readonly PlainTextDocumentExporterOptions options;
		public PlainTextExporter(DocumentModel documentModel, PlainTextDocumentExporterOptions options)
			: base(documentModel) {
			this.sb = new StringBuilder();
			this.options = options;
		}
		public StringBuilder Content { get { return sb; } }
		protected internal override bool ShouldExportHiddenText { get { return options.ExportHiddenText; } }
		public new string Export() {
			return ExportSaveMemory().ToString();
		}
		public StringBuilder ExportSaveMemory() {
			sb.Length = 0;
			base.Export();
			sb.Length = CalculateActualContentLength();
			return sb;
		}
		protected internal override void ExportDocument() {
			base.ExportDocument();
			ExportFootEndNotes(FootNoteExportInfos, options.ActualFootNoteSeparator);
			ExportFootEndNotes(EndNoteExportInfos, options.ActualEndNoteSeparator);
		}
		void ExportFootEndNotes(List<FootNoteExportInfo> notes, string separator) {
			int count = notes.Count;
			if (count <= 0)
				return;
			sb.Append(separator);
			sb.Append("\r\n");
			for (int i = 0; i < count; i++)
				PerformExportPieceTable(notes[i].Note, ExportPieceTable);
		}
		internal int CalculateActualContentLength() {
			int length = sb.Length;
			if (length <= 0)
				return 0;
			if (length == 1) {
				char lastChar = sb[length - 1];
				if (lastChar == '\r' || lastChar == '\n')
					return 0; 
				else
					return 1;
			}
			else {
				string trail = sb.ToString(length - 2, 2);
				if (trail == "\r\n" || trail == "\n\r")
					return length - 2; 
				if (trail[1] == '\r' || trail[1] == '\n')
					return length - 1; 
				else
					return length;
			}
		}
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
		}
		protected internal override void ExportTextRun(TextRun run) {
			string text = run.GetPlainText(PieceTable.TextBuffer);
			if (run.AllCaps)
				text = text.ToUpper();
			string[] lines = text.Split(Characters.LineBreak);
			int linesCount = lines.Length;
			for (int i = 0; i < linesCount - 1; i++) {
				sb.Append(lines[i]); 
				sb.Append("\r\n");
			}
			sb.Append(lines[linesCount - 1]);
		}
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			if (options.ExportBulletsAndNumbering && paragraph.IsInList() && paragraph.ShouldExportNumbering())
				sb.Append(GetNumberingListText(paragraph));
			return base.ExportParagraph(paragraph);
		}
		protected internal override void ExportParagraphRun(ParagraphRun run) {
			sb.Append("\r\n");
		}
		protected internal override void ExportSectionRun(SectionRun run) {
			sb.Append("\r\n");
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run);
			if (ShouldExportHiddenText && !String.IsNullOrEmpty(options.FieldCodeStartMarker))
				sb.Append(options.FieldCodeStartMarker);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			base.ExportFieldCodeEndRun(run);
			if (ShouldExportHiddenText && !String.IsNullOrEmpty(options.FieldCodeEndMarker))
				sb.Append(options.FieldCodeEndMarker);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			base.ExportFieldResultEndRun(run);
			if (ShouldExportHiddenText && !String.IsNullOrEmpty(options.FieldResultEndMarker))
				sb.Append(options.FieldResultEndMarker);
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			if (PieceTable.IsMain) {
				base.ExportFootNoteRun(run);
				FootNoteExportInfo info = CreateFootNoteExportInfo(run);
				info.Id = FootNoteExportInfos.Count;
				FootNoteExportInfos.Add(info);
				ExportFootNoteRunReference(info, options.ActualFootNoteNumberStringFormat);
			}
			else {
				FootNoteExportInfo info = FindFootNoteExportInfoByNote(FootNoteExportInfos, PieceTable);
				if (info != null)
					ExportFootNoteRunReference(info, options.ActualFootNoteNumberStringFormat);
			}
		}
		protected internal virtual void ExportFootNoteRunReference(FootNoteExportInfo info, string format) {
			string text = String.Format(format, info.NumberText, info.Number, info.Id);
			sb.Append(text);
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			if (PieceTable.IsMain) {
				base.ExportEndNoteRun(run);
				FootNoteExportInfo info = CreateFootNoteExportInfo(run);
				info.Id = EndNoteExportInfos.Count;
				EndNoteExportInfos.Add(info);
				ExportFootNoteRunReference(info, options.ActualEndNoteNumberStringFormat);
			}
			else {
				FootNoteExportInfo info = FindFootNoteExportInfoByNote(EndNoteExportInfos, PieceTable);
				if (info != null)
					ExportFootNoteRunReference(info, options.ActualEndNoteNumberStringFormat);
			}
		}
		public string ExportPieceTableContent(PieceTable pieceTable) {
			Content.Clear();
			PerformExportPieceTable(pieceTable, ExportPieceTable);
			return Content.ToString();
		}
	}
	#endregion
}
