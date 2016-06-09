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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.Office.Export.Html;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if !SL
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using SysImage = System.Windows.Controls.Image;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlExporterTextProperties
	public class HtmlExporterTextProperties {
		readonly int fontCacheIndex;
		readonly Color foreColor;
		readonly Color backColor;
		readonly bool underline;
		readonly bool strikeout;
		readonly bool allCaps;
		readonly CharacterFormattingScript script;
		readonly int doubleFontSize;
		public HtmlExporterTextProperties(int fontCacheIndex, Color foreColor, Color backColor, bool underline, bool strikeout, CharacterFormattingScript script, bool allCaps, int doubleFontSize) {
			this.fontCacheIndex = fontCacheIndex;
			this.foreColor = foreColor;
			this.backColor = backColor;
			this.underline = underline;
			this.strikeout = strikeout;
			this.allCaps = allCaps;
			this.script = script;
			this.doubleFontSize = doubleFontSize;
		}
		public int FontCacheIndex { get { return fontCacheIndex; } }
		public Color ForeColor { get { return foreColor; } }
		public Color BackColor { get { return backColor; } }
		public bool Underline { get { return underline; } }
		public bool Strikeout { get { return strikeout; } }
		public bool AllCaps { get { return allCaps; } }
		public CharacterFormattingScript Script { get { return script; } }
		public int DoubleFontSize { get { return doubleFontSize; } }
		public override bool Equals(object obj) {
			HtmlExporterTextProperties other = obj as HtmlExporterTextProperties;
			if (Object.ReferenceEquals(other, null))
				return false;
			return FontCacheIndex == other.FontCacheIndex &&
				ForeColor == other.ForeColor &&
				BackColor == other.BackColor &&
				Underline == other.Underline &&
				Strikeout == other.Strikeout &&
				AllCaps == other.AllCaps &&
				DoubleFontSize == other.DoubleFontSize;
		}
		public override int GetHashCode() {
			return FontCacheIndex ^ ForeColor.GetHashCode() ^ BackColor.GetHashCode() ^ (int)(allCaps ? 0x80000000 : 0) ^ (underline ? 0x40000000 : 0) ^ (strikeout ? 0x20000000 : 0) ^ Script.GetHashCode() ^ DoubleFontSize.GetHashCode();
		}
	}
	#endregion
	#region HtmlContentExporter
	public class HtmlContentExporter : DocumentModelExporter {
		#region Fields
		readonly HtmlExportHelper exportHelper;
		readonly Stack<DXWebControlBase> controlStack;
		readonly HtmlDocumentExporterOptions options;
		HtmlExporterTextProperties lastExportedTextProperties;
		HtmlExporterTextProperties lastExportedHyperLinkTextProperties;
		DXWebControlBase lastCreatedTextControl;
		int listControlNestingLevel;
		Stack<int> listLeftIndents;
		int currentListLeftIndent;
		int parentListLeftIndent;
		bool isHyperlinkContentExporting;
		HyperlinkInfo currentHyperlinkInfo;
		readonly Dictionary<Bookmark, DXHtmlGenericControl> exportedBookmarks = new Dictionary<Bookmark, DXHtmlGenericControl>();
		bool keepLeadingWhitespace;
		#endregion
		public HtmlContentExporter(DocumentModel documentModel, IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlNumberingListExportFormat numberingListExportFormat, bool isExportInlineStyle)
			: this(documentModel, scriptContainer, imageRepository, CreateOptions(numberingListExportFormat, isExportInlineStyle)) {
		}
		static HtmlDocumentExporterOptions CreateOptions(HtmlNumberingListExportFormat numberingListExportFormat, bool isExportInlineStyle) {
			HtmlDocumentExporterOptions result = new HtmlDocumentExporterOptions();
			result.HtmlNumberingListExportFormat = numberingListExportFormat;
			if (isExportInlineStyle)
				result.CssPropertiesExportType = CssPropertiesExportType.Inline;
			else
				result.CssPropertiesExportType = CssPropertiesExportType.Style;
			return result;
		}
		public HtmlContentExporter(DocumentModel documentModel, IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(scriptContainer, "scriptContainer");
			Guard.ArgumentNotNull(imageRepository, "imageRepository");
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.controlStack = new Stack<DXWebControlBase>();
			this.listLeftIndents = new Stack<int>();
			this.exportHelper = CreateHtmlExportHelper(scriptContainer, imageRepository, options);
		}
		internal virtual HtmlExportHelper ExportHelper { get { return exportHelper; } }
		protected internal virtual HtmlExportHelper CreateHtmlExportHelper(IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlDocumentExporterOptions options) {
			return new HtmlExportHelper(DocumentModel, scriptContainer, imageRepository, options);
		}
		#region Properties
		protected internal DXWebControlBase CurrentParent { get { return controlStack.Peek(); } }
		protected Stack<DXWebControlBase> ControlStack { get { return controlStack; } }
		#endregion
		protected override VisitableDocumentIntervalBasedObjectBoundaryIterator CreateVisitableDocumentIntervalBoundaryIterator() {
			return new HtmlVisitableDocumentIntervalBasedObjectBoundaryIterator(PieceTable);
		}
		protected internal virtual void PushControl(DXWebControlBase parent) {
			controlStack.Push(parent);
		}
		protected internal virtual DXWebControlBase PopControl() {
			return controlStack.Pop();
		}
		public void Export(DXWebControlBase parent) {
			Guard.ArgumentNotNull(parent, "parent");
			PushControl(parent);
			try {
				Export();
			}
			finally {
				PopControl();
			}
		}
		public override void Export() {
			Debug.Assert(controlStack.Count > 0);
			base.Export();
		}
		protected internal override void ExportDocument() {
			base.ExportDocument();
			keepLeadingWhitespace = true;
			ExportFootEndNotes(FootNoteExportInfos);
			ExportFootEndNotes(EndNoteExportInfos);
		}
		protected virtual void ExportFootEndNotes(List<FootNoteExportInfo> notes) {
			int count = notes.Count;
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++)
				PerformExportPieceTable(notes[i].Note, ExportPieceTable);
		}
		protected internal virtual void AddControlToChild(DXWebControlBase parent, DXWebControlBase control) {
			parent.Controls.Add(control);
		}
		protected virtual void ExportCore(HtmlExporterTextProperties textProperties, string text) {
			CharacterFormattingScript script = textProperties.Script;
			DXWebControlCollection parentControls = CurrentParent.Controls;
			DXHtmlGenericControl peek = CurrentParent as DXHtmlGenericControl;
			bool forceExportCharacterProperties = !Object.ReferenceEquals(peek, null) && peek.TagKey == DXHtmlTextWriterTag.Li;
			if (script == CharacterFormattingScript.Normal) {
				int parentControlsCount = parentControls.Count;
				DXWebControlBase prevControl = parentControlsCount > 0 ? parentControls[parentControlsCount - 1] : null;
				if (prevControl != null && Object.ReferenceEquals(lastCreatedTextControl, prevControl) && textProperties.Equals(lastExportedTextProperties)) {
					exportHelper.AppendText(prevControl, text);
				}
				else {
					DXHtmlControl textControl = (DXHtmlControl)exportHelper.CreateTextControl(text, textProperties);
#if (DEBUG)
					Debug.Assert(Object.ReferenceEquals(CurrentParent.Controls, parentControls));
#endif
					AddControlToChild(CurrentParent, textControl);
					if (CurrentParent as DXHtmlAnchor == null || !textProperties.Equals(lastExportedHyperLinkTextProperties))
						exportHelper.SetTextProperties(textControl, textProperties, forceExportCharacterProperties);
					lastCreatedTextControl = textControl;
					lastExportedTextProperties = textProperties;
				}
			}
			else {
				DXHtmlTextWriterTag tag = new DXHtmlTextWriterTag();
				if (script == CharacterFormattingScript.Subscript)
					tag = DXHtmlTextWriterTag.Sub;
				if (script == CharacterFormattingScript.Superscript)
					tag = DXHtmlTextWriterTag.Sup;
				DXHtmlGenericControl control = new DXHtmlGenericControl(tag);
				exportHelper.AppendText(control, text);
				DXHtmlGenericControl style = new DXHtmlGenericControl(DXHtmlTextWriterTag.Span);
				exportHelper.SetTextProperties(style, textProperties, forceExportCharacterProperties);
#if (DEBUG)
				Debug.Assert(Object.ReferenceEquals(CurrentParent.Controls, parentControls));
#endif
				AddControlToChild(style, control);
				AddControlToChild(CurrentParent, style);
				lastCreatedTextControl = null;
				lastExportedTextProperties = null;
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
		protected internal override ParagraphIndex ExportTable(TableInfo tableInfo) {
			Paragraph firstParagraph = tableInfo.Table.PieceTable.Paragraphs[tableInfo.Table.Rows[0].Cells[0].StartParagraphIndex];
			if (!firstParagraph.IsInList() || !firstParagraph.ShouldExportNumbering())
				if (CurrentParent is NumberingListWebControl)
					CloseListsWithSmallerLevelIndex(firstParagraph);
			DXWebControlBase table = exportHelper.CreateTableControl(tableInfo.Table);
			try {
				AddControlToChild(CurrentParent, table);
				PushControl(table);
				return base.ExportTable(tableInfo);
			}
			finally {
				PopControl();
			}
		}
		protected internal override void ExportRow(TableRow row, TableInfo tableInfo) {
			DXWebControlBase tableRow = exportHelper.CreateTableRowControl(row);
			try {
				AddControlToChild(CurrentParent, tableRow);
				PushControl(tableRow);
				base.ExportRow(row, tableInfo);
			}
			finally {
				PopControl();
			}
		}
		protected internal override void ExportCell(TableCell cell, TableInfo tableInfo) {
			VerticalMergeCellProperties mergedCellProperties = tableInfo.GetMergedCellProperties(cell);
			int rowSpan = mergedCellProperties.RowSpan;
			if (rowSpan == 0)
				return;
			DXWebControlBase tableCell = exportHelper.CreateTableCellControl(cell, mergedCellProperties);
			try {
				AddControlToChild(CurrentParent, tableCell);
				PushControl(tableCell);
				base.ExportCell(cell, tableInfo);
			}
			finally {
				while (PopControl() != tableCell) ;
			}
		}
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			keepLeadingWhitespace = true;
			if (paragraph.IsInList() && paragraph.ShouldExportNumbering()) {
				ParagraphIndex paragraphIndex = ExportParagraphInList(paragraph);
				if (paragraph.IsLast)
					CloseListsWithSmallerLevelIndex(paragraph);
				return paragraphIndex;
			}
			else {
				if (CurrentParent is NumberingListWebControl) 
					CloseListsWithSmallerLevelIndex(paragraph);
				DXWebControlBase paragraphControl = CreateParagraphControl(paragraph);
				this.currentListLeftIndent = paragraph.LeftIndent;
				try {
					return ExportParagraphCore(paragraph, paragraphControl);
				}
				finally {
					this.currentListLeftIndent = 0;
				}
			}
		}
		protected internal virtual void ClosePreviousNumberingList() {
			Guard.ArgumentNotNull(CurrentParent is NumberingListWebControl, "CurrentParent1 is NumberingListWebControl");
			PopControl();
			DecreaseListControlNestingLevel();
			DXHtmlGenericControl peek = CurrentParent as DXHtmlGenericControl;
			if(!object.ReferenceEquals(peek, null) && peek.TagKey == DXHtmlTextWriterTag.Li)
				PopControl();
		}
		protected internal virtual ParagraphIndex ExportParagraphInList(Paragraph paragraph) {
			int[] counters = this.PieceTableNumberingListCounters.CalculateNextCounters(paragraph);
			if (options.HtmlNumberingListExportFormat == HtmlNumberingListExportFormat.PlainTextFormat) {
				DXWebControlBase paragraphControl = CreateParagraphControl(paragraph);
				this.currentListLeftIndent = paragraph.LeftIndent;
				try {
					return ExportParagraphInListCore(paragraph, paragraphControl, counters);
				}
				finally {
					this.currentListLeftIndent = 0;
				}
			}
			DXWebControlBase paragraphInListControl = CreateParagraphInListControl(paragraph, counters);
			return ExportParagraphCore(paragraph, paragraphInListControl);
		}
		void ExportParagraphNumeration(Paragraph paragraph, int[] counters) {
			IListLevel level = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()].Levels[paragraph.GetListLevelIndex()];
			int fontCacheIndex = paragraph.GetNumerationFontCacheIndex();
			string text = paragraph.GetNumberingListText(counters) + level.ListLevelProperties.Separator;
			MergedCharacterProperties properties = paragraph.GetNumerationCharacterProperties();
			CharacterFormattingInfo info = properties.Info;
			ExportTextCore(fontCacheIndex, text, info.ForeColor, info.BackColor, info.FontUnderlineType, info.FontStrikeoutType, info.Script, info.AllCaps, info.DoubleFontSize);
		}
		protected internal virtual ParagraphIndex ExportParagraphInListCore(Paragraph paragraph, DXWebControlBase paragraphControl, int[] counters) {
			AddControlToChild(CurrentParent, paragraphControl);
			PushControl(paragraphControl);
			ExportParagraphNumeration(paragraph, counters);
			if (isHyperlinkContentExporting)
				StartHyperlinkExport(currentHyperlinkInfo);
			return ExportParagraphContent(paragraph);
		}
		protected ParagraphIndex ExportParagraphCore(Paragraph paragraph, DXWebControlBase paragraphControl) {
			AddControlToChild(CurrentParent, paragraphControl);
			PushControl(paragraphControl);
			if (isHyperlinkContentExporting)
				StartHyperlinkExport(currentHyperlinkInfo);
			return ExportParagraphContent(paragraph);
		}
		ParagraphIndex ExportParagraphContent(Paragraph paragraph) {
			int controlsCountBefore = ControlStack.Count;
			try {
				if (IsEmptyParagraph(paragraph)) {
					TryToExportBookmarks(paragraph.FirstRunIndex, 0);
					ExportEmptyParagraph(PieceTable.Runs[paragraph.FirstRunIndex]);
					TryToExportBookmarks(paragraph.LastRunIndex, 0);
					return paragraph.Index;
				}
				else
					return base.ExportParagraph(paragraph);
			}
			finally {
				if (isHyperlinkContentExporting)
					PopControl();
				while(ControlStack.Count >= controlsCountBefore)
					PopControl();
			}
		}
		bool IsEmptyParagraph(Paragraph paragraph) {
			int runCount = paragraph.LastRunIndex - paragraph.FirstRunIndex;
			if (runCount == 0 || (runCount == 1 && paragraph.PieceTable.Runs[paragraph.FirstRunIndex].GetPlainText(PieceTable.TextBuffer) == " "))
				return true;
			return false;
		}
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			DXHtmlGenericControl control = new DXHtmlGenericControl(DXHtmlTextWriterTag.A);
			if(options.UseHtml5)
				control.Attributes.Add("id", bookmark.Name);
			else
				control.Attributes.Add("name", bookmark.Name);
			AddControlToChild(CurrentParent, control);
			PushControl(control);
			exportedBookmarks[bookmark] = control;
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			DXHtmlGenericControl control;
			if (!exportedBookmarks.TryGetValue(bookmark, out control))
				return;
			exportedBookmarks.Remove(bookmark);
			if (controlStack.Count > 0 && control != controlStack.Peek())
				return;
			PopControl();
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			base.ExportFieldCodeStartRun(run); 
			if (GetHyperlinkInfo(run) == null)
				return;
			FinishHyperlinkExport();
			ExportTextRunCore(run, String.Empty);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			base.ExportFieldCodeEndRun(run); 
			HyperlinkInfo hyperlinkInfo = GetHyperlinkInfo(run);
			if (hyperlinkInfo == null)
				return;
			Color foreColor = run.ForeColor;
			Color backColor = run.BackColor;
			if (foreColor == DXColor.Empty)
				foreColor = DXColor.Black;
			if (backColor == DXColor.Empty)
				backColor = DXColor.Transparent;
			bool underline = run.FontUnderlineType != UnderlineType.None;
			if (options.UnderlineTocHyperlinks) {
				Field parentTocField = PieceTable.FindFieldByRunIndex(run.GetRunIndex()).Parent;
				if (parentTocField != null && PieceTable.IsTocField(parentTocField))
					underline = true;
			}
			bool strikeout = run.FontStrikeoutType != StrikeoutType.None;
			StartHyperlinkExport(hyperlinkInfo, CreateHtmlExporterTextProperties(run.FontCacheIndex, foreColor, backColor, underline, strikeout, run.Script, run.AllCaps, run.DoubleFontSize));
		}
		protected virtual HtmlExporterTextProperties CreateHtmlExporterTextProperties(int fontCacheIndex, Color foreColor, Color backColor, bool underline, bool strikeout, CharacterFormattingScript script, bool allCaps, int doubleFontSize) {
			return new HtmlExporterTextProperties(fontCacheIndex, foreColor, backColor, underline, strikeout, script, allCaps, doubleFontSize);
		}
		protected virtual void StartHyperlinkExport(HyperlinkInfo hyperlinkInfo) {
			DXWebControlBase control = CreateHyperlinkControl(hyperlinkInfo);
			StartHyperlinkExportCore(control, hyperlinkInfo);
		}
		protected virtual void StartHyperlinkExport(HyperlinkInfo hyperlinkInfo, HtmlExporterTextProperties textProperties) {
			DXWebControlBase control = CreateHyperlinkControl(hyperlinkInfo, textProperties);
			StartHyperlinkExportCore(control, hyperlinkInfo);
		}
		void StartHyperlinkExportCore(DXWebControlBase control, HyperlinkInfo hyperlinkInfo) {
			int childrenCount = CurrentParent.Controls.Count;
			DXWebControlBase lastChild = childrenCount > 0 ? CurrentParent.Controls[childrenCount - 1] : null;
			if (lastChild != null && Object.ReferenceEquals(lastCreatedTextControl, lastChild))
				AddControlToChild(this.lastCreatedTextControl, control);
			else
				AddControlToChild(CurrentParent, control);
			PushControl(control);
			this.isHyperlinkContentExporting = true;
			currentHyperlinkInfo = hyperlinkInfo;
		}
		HyperlinkInfo GetHyperlinkInfo(TextRun run) {
			RunIndex runIndex = run.GetRunIndex();
			Field field = PieceTable.FindFieldByRunIndex(runIndex);
			Debug.Assert(field != null);
			HyperlinkInfo hyperlinkInfo = null;
			if (PieceTable.HyperlinkInfos.TryGetHyperlinkInfo(field.Index, out hyperlinkInfo))
				return hyperlinkInfo;
			return null;
		}
		protected virtual DXHtmlAnchor CreateHyperlinkControl(HyperlinkInfo hyperlinkInfo) {
			DXHtmlAnchor control = CreateHyperlinkControlCore(hyperlinkInfo);
			return control;
		}
		protected virtual DXHtmlAnchor CreateHyperlinkControl(HyperlinkInfo hyperlinkInfo, HtmlExporterTextProperties textProperties) {
			DXHtmlAnchor control = CreateHyperlinkControlCore(hyperlinkInfo);			
			exportHelper.SetTextProperties(control, textProperties, false);
			lastExportedHyperLinkTextProperties = textProperties;
			return control;
		}
		DXHtmlAnchor CreateHyperlinkControlCore(HyperlinkInfo hyperlinkInfo) {
			DXHtmlAnchor control = new DXHtmlAnchor();
			if (!String.IsNullOrEmpty(hyperlinkInfo.NavigateUri)) {
				if (String.IsNullOrEmpty(hyperlinkInfo.Anchor))
					control.HRef = HyperlinkUriHelper.ConvertToUrl(hyperlinkInfo.NavigateUri);
				else
					control.HRef = HyperlinkUriHelper.ConvertToUrl(hyperlinkInfo.NavigateUri + "#" + hyperlinkInfo.Anchor);
			}
			else if (!String.IsNullOrEmpty(hyperlinkInfo.Anchor))
				control.HRef = "#" + hyperlinkInfo.Anchor;
			if (!String.IsNullOrEmpty(hyperlinkInfo.Target))
				control.Target = hyperlinkInfo.Target;
			if (!String.IsNullOrEmpty(hyperlinkInfo.ToolTip))
				control.Title = hyperlinkInfo.ToolTip;
			return control;
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			if (GetHyperlinkInfo(run) == null)
				return;
			FinishHyperlinkExport();
		}
		protected virtual void FinishHyperlinkExport() {
			if (controlStack.Count > 0 && isHyperlinkContentExporting) {
				PopControl();
				this.isHyperlinkContentExporting = false;
				this.currentHyperlinkInfo = null;
			}
		}
		protected DXWebControlBase CreateParagraphControl(Paragraph paragraph) {
			return exportHelper.CreateParagraphControl(paragraph, options.IgnoreParagraphOutlineLevel, GetListLevelNestingLevel(), parentListLeftIndent);
		}
		DXWebControlBase CreateParagraphInListControl(Paragraph paragraph, int[] counters) {
			NumberingList numberingList = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()];
			EnsureNumberingListControl(paragraph, numberingList, counters);
			return CreateListLevelControl(paragraph, numberingList, exportHelper.GetMostNestedCounter(counters));
		}
		protected internal virtual void DecreaseListControlNestingLevel() {
			listControlNestingLevel--;
			currentListLeftIndent = listLeftIndents.Pop();
			parentListLeftIndent = listLeftIndents.Count > 0 ? listLeftIndents.Peek() + DocumentModel.UnitConverter.DocumentsToModelUnits(150) : 0;
		}
		protected internal virtual void IncreaseListControlNestingLevel() {
			listControlNestingLevel++;
			listLeftIndents.Push(currentListLeftIndent);
			parentListLeftIndent = currentListLeftIndent + DocumentModel.UnitConverter.DocumentsToModelUnits(150);
		}
		protected internal virtual int GetListLevelNestingLevel() {
			return listControlNestingLevel;
		}
		protected internal virtual void EnsureNumberingListControl(Paragraph paragraph, NumberingList numberingList, int[] counters) {
			NumberingListWebControl numberingListWebControl = CurrentParent as NumberingListWebControl;
			if (numberingListWebControl == null) {
				if (IsRootFirstParagraphInListHasNonZeroLevel(paragraph)) {
					CreateSeveralParentNumberingListToMakeNewNumberingListNested(paragraph, numberingList, counters);
				}
				CreateNumberingListControl(paragraph, numberingList, exportHelper.GetMostNestedCounter(counters));
				return;
			}
			if (numberingListWebControl.AbstractListIndex != numberingList.AbstractNumberingListIndex) {
				ClosePreviousNumberingList();
				if (paragraph.GetListLevelIndex() < numberingListWebControl.CurrentLevelIndex)
					CloseListsWithSmallerLevelIndex(paragraph);
				if (IsRootFirstParagraphInListHasNonZeroLevel(paragraph)) {
					CreateSeveralParentNumberingListToMakeNewNumberingListNested(paragraph, numberingList, counters);
				}
				CreateNumberingListControl(paragraph, numberingList, exportHelper.GetMostNestedCounter(counters));
				return;
			}
			if (paragraph.GetListLevelIndex() > numberingListWebControl.CurrentLevelIndex)
				CreateNumberingListControl(paragraph, numberingList, exportHelper.GetMostNestedCounter(counters));
			if (paragraph.GetListLevelIndex() < numberingListWebControl.CurrentLevelIndex)
				CloseListsWithSmallerLevelIndex(paragraph);
		}
		bool IsRootFirstParagraphInListHasNonZeroLevel(Paragraph paragraph) {
			return CurrentParent as NumberingListWebControl == null && paragraph.GetListLevelIndex() != 0;
		}
		void CreateSeveralParentNumberingListToMakeNewNumberingListNested(Paragraph paragraph, NumberingList numberingList, int[] counters) {
			int count = paragraph.GetListLevelIndex();
			for (int i = 0; i < count; i++)
				CreateNumberingListControl(paragraph, numberingList, counters[Math.Min(i, counters.Length - 1)]);
		}
		void CloseListsWithSmallerLevelIndex(Paragraph paragraph) {
			NumberingListWebControl numberingListWebControl = CurrentParent as NumberingListWebControl;
			if (numberingListWebControl == null)
				return;
			do {
				ClosePreviousNumberingList();
				numberingListWebControl = CurrentParent as NumberingListWebControl;
				if (numberingListWebControl == null)
					return;
			}
			while (paragraph.GetListLevelIndex() < numberingListWebControl.CurrentLevelIndex);
		}
		protected internal virtual void CreateNumberingListControl(Paragraph paragraph, NumberingList numberingList, int counter) {
			NumberingListWebControl parent = CurrentParent as NumberingListWebControl;
			if(!object.ReferenceEquals(parent, null) && parent.Controls.Count > 0) {
				DXHtmlGenericControl lastItem = parent.Controls[parent.Controls.Count - 1] as DXHtmlGenericControl;
				if(!object.ReferenceEquals(lastItem, null) && lastItem.TagKey == DXHtmlTextWriterTag.Li)
					PushControl(lastItem);
			}
			DXWebControlBase numberingListControl = CreateNumberingListControlCore(paragraph, numberingList, counter);
			AddControlToChild(CurrentParent, numberingListControl);
			PushControl(numberingListControl);
			IncreaseListControlNestingLevel();
		}
		DXWebControlBase CreateNumberingListControlCore(Paragraph paragraph, NumberingList numberingList, int counter) {
			return exportHelper.CreateNumberingListControl(paragraph, numberingList, counter);
		}
		public DXWebControlBase CreateListLevelControl(Paragraph paragraph, NumberingList numberingList, int counter) {
			currentListLeftIndent = paragraph.LeftIndent;
			return exportHelper.CreateListLevelControl(paragraph, numberingList, GetListLevelNestingLevel(), parentListLeftIndent, counter);
		}
		void ExportEmptyParagraph(TextRunBase run) {
			Color foreColor = run.ForeColor;
			Color backColor = run.BackColor;
			if (foreColor == DXColor.Empty)
				foreColor = DXColor.Black;
			if (backColor == DXColor.Empty)
				backColor = DXColor.Transparent;
			ExportCore(CreateHtmlExporterTextProperties(run.FontCacheIndex, foreColor, backColor, false, false, CharacterFormattingScript.Normal, run.AllCaps, run.DoubleFontSize), "&nbsp;");
		}
		protected internal override void ExportTextRun(TextRun run) {
			string text = run.GetPlainText(PieceTable.TextBuffer);
			if (keepLeadingWhitespace) {
				if (text.StartsWith(" "))
					text = Characters.NonBreakingSpace + text.Substring(1);
				keepLeadingWhitespace = false;
			}
			text = ProcessRunText(run, text);
			if (!String.IsNullOrEmpty(text))
				ExportTextRunCore(run, text);
		}
		protected virtual string ProcessRunText(TextRun run, string text) {
			return ExportHelper.HtmlStyleHelper.PreprocessHtmlContentText(text, options.TabMarker);
		}
		protected internal virtual void ExportTextRunCore(TextRun run, string text) {
			ExportTextCore(run.FontCacheIndex, text, run.ForeColor, run.BackColor, run.FontUnderlineType, run.FontStrikeoutType, run.Script, run.AllCaps, run.DoubleFontSize);
		}
		protected internal void ExportTextCore(int fontCacheIndex, string text, Color foreColor, Color backColor, UnderlineType underlineType, StrikeoutType strikeoutType, CharacterFormattingScript script, bool allCaps, int doubleFontSize) {
			if (foreColor == DXColor.Empty)
				foreColor = DXColor.Black;
			if (backColor == DXColor.Empty)
				backColor = DXColor.Transparent;
			bool underline = underlineType != UnderlineType.None;
			bool strikeout = strikeoutType != StrikeoutType.None;
			ExportCore(CreateHtmlExporterTextProperties(fontCacheIndex, foreColor, backColor, underline, strikeout, script, allCaps, doubleFontSize), text);
		}
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			Color foreColor = run.ForeColor != DXColor.Empty ? run.ForeColor : DXColor.Black;
			Color backColor = run.BackColor != DXColor.Empty ? run.BackColor : DXColor.Transparent;
			bool underline = run.FontUnderlineType != UnderlineType.None;
			bool strikeout = run.FontStrikeoutType != StrikeoutType.None;
			HtmlExporterTextProperties textProperties = CreateHtmlExporterTextProperties(run.FontCacheIndex, foreColor, backColor, underline, strikeout, run.Script, run.AllCaps, run.DoubleFontSize);
			DXWebControlCollection parentControls = CurrentParent.Controls;
			int parentControlsCount = parentControls.Count;
			DXWebControlBase prevControl = parentControlsCount > 0 ? parentControls[parentControlsCount - 1] : null;
			if (prevControl != null && Object.ReferenceEquals(this.lastCreatedTextControl, prevControl) && textProperties.Equals(this.lastExportedTextProperties))
				AddControlToChild(prevControl, exportHelper.CreateImageControlInternal(run.Image, run.ActualSize, HtmlCssFloat.NotSet));
			else if (AreCharacterPropertiesDefault(run) || ((CurrentParent is DXHtmlAnchor) && textProperties.Equals(this.lastExportedHyperLinkTextProperties)))
				AddControlToChild(CurrentParent, exportHelper.CreateImageControl(run));
			else {
				DXHtmlControl textControl = (DXHtmlControl)exportHelper.CreateTextControl(String.Empty, textProperties);
				AddControlToChild(CurrentParent, textControl);
				AddControlToChild(textControl, exportHelper.CreateImageControlInternal(run.Image, run.ActualSize, HtmlCssFloat.NotSet));
				exportHelper.SetTextProperties(textControl, textProperties, false);
				this.lastCreatedTextControl = textControl;
				this.lastExportedTextProperties = textProperties;
			}
		}
		bool AreCharacterPropertiesDefault(InlinePictureRun run) {
			CharacterFormattingInfo characterProperties = HtmlExportHelper.CreateDefaultCharacterProperties(DocumentModel);
			return run.BackColor.Equals(characterProperties.BackColor) &&
				run.DoubleFontSize == characterProperties.DoubleFontSize &&
				run.FontName == characterProperties.FontName &&
				run.FontBold == characterProperties.FontBold &&
				run.FontItalic == characterProperties.FontItalic &&
				run.ForeColor.Equals(characterProperties.ForeColor);
		}
		protected internal override bool ShouldExportInlinePicture(InlinePictureRun run) {
			return ShouldExportRun(run);
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			if (PieceTable.IsMain) {
				base.ExportFootNoteRun(run);
				FootNoteExportInfo info = CreateFootNoteExportInfo(run);
				info.Id = FootNoteExportInfos.Count;
				FootNoteExportInfos.Add(info);
				ExportFootNoteRunReference(info, run, options.ActualFootNoteNumberStringFormat, options.ActualFootNoteNamePrefix);
			}
			else {
				FootNoteExportInfo info = FindFootNoteExportInfoByNote(FootNoteExportInfos, PieceTable);
				if (info != null)
					ExportFootNoteRunReference(info, run, options.ActualFootNoteNumberStringFormat, options.ActualFootNoteNamePrefix);
			}
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			if (PieceTable.IsMain) {
				base.ExportEndNoteRun(run);
				FootNoteExportInfo info = CreateFootNoteExportInfo(run);
				info.Id = EndNoteExportInfos.Count;
				EndNoteExportInfos.Add(info);
				ExportFootNoteRunReference(info, run, options.ActualEndNoteNumberStringFormat, options.ActualEndNoteNamePrefix);
			}
			else {
				FootNoteExportInfo info = FindFootNoteExportInfoByNote(EndNoteExportInfos, PieceTable);
				if (info != null)
					ExportFootNoteRunReference(info, run, options.ActualEndNoteNumberStringFormat, options.ActualEndNoteNamePrefix);
			}
		}
		protected internal virtual void ExportFootNoteRunReference<T>(FootNoteExportInfo info, FootNoteRunBase<T> run, string format, string prefix) where T : FootNoteBase<T> {
			DXHtmlAnchor control = new DXHtmlAnchor();
			string href = PieceTable.IsMain ? prefix : prefix + "_GoBack";
			string anchor = PieceTable.IsMain ? prefix + "_GoBack" : prefix;
			href += info.Id.ToString();
			anchor += info.Id.ToString();
			control.HRef = "#" + href;
			control.Name = anchor;
			try {
				AddControlToChild(CurrentParent, control);
				PushControl(control);
				string text = String.Format(format, info.NumberText, info.Number, info.Id);
				ExportTextRunCore(run, text);
			}
			finally {
				PopControl();
			}
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			DXHtmlGenericControl control = new DXHtmlGenericControl(DXHtmlTextWriterTag.Div);
			AddControlToChild(CurrentParent, control);
			PushControl(control);
			try {
				ExportFloatingObjectFrame(run, control);
				ExportFloatingObjectContent(run);
			}
			finally {
				PopControl();
			}
		}
		protected internal virtual void ExportFloatingObjectFrame(FloatingObjectAnchorRun run, DXHtmlGenericControl control) {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			DXCssStyleCollection style = control.Style;
			Shape shape = run.Shape;
			if (shape.OutlineWidth > 0 && !DXColor.IsTransparentOrEmpty(shape.OutlineColor)) {
				style.Add(DXHtmlTextWriterStyle.BorderWidth, String.Format(CultureInfo.InvariantCulture, "{0}pt", unitConverter.ModelUnitsToPointsF(shape.OutlineWidth)));
				style.Add(DXHtmlTextWriterStyle.BorderStyle, "solid");
				style.Add(DXHtmlTextWriterStyle.BorderColor, DXColor.ToHtml(shape.OutlineColor));
			}
			if (!DXColor.IsTransparentOrEmpty(shape.FillColor))
				style.Add(DXHtmlTextWriterStyle.BackgroundColor, DXColor.ToHtml(shape.FillColor));
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			if (floatingObjectProperties.LeftDistance > 0 || floatingObjectProperties.RightDistance > 0 || floatingObjectProperties.TopDistance > 0 || floatingObjectProperties.BottomDistance > 0) {
				style.Add(DXHtmlTextWriterStyle.Margin, String.Format(CultureInfo.InvariantCulture, "{0}pt {1}pt {2}pt {3}pt",
					unitConverter.ModelUnitsToPointsF(floatingObjectProperties.TopDistance),
					unitConverter.ModelUnitsToPointsF(floatingObjectProperties.RightDistance),
					unitConverter.ModelUnitsToPointsF(floatingObjectProperties.BottomDistance),
					unitConverter.ModelUnitsToPointsF(floatingObjectProperties.LeftDistance)));
			}
			int width = floatingObjectProperties.ActualSize.Width;
			int height = floatingObjectProperties.ActualSize.Height;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null) {
				TextBoxProperties properties = textBoxContent.TextBoxProperties;
				if (properties.LeftMargin > 0 || properties.RightMargin > 0 || properties.TopMargin > 0 || properties.BottomMargin > 0) {
					style.Add(DXHtmlTextWriterStyle.Padding, String.Format(CultureInfo.InvariantCulture, "{0}pt {1}pt {2}pt {3}pt",
						unitConverter.ModelUnitsToPointsF(properties.TopMargin),
						unitConverter.ModelUnitsToPointsF(properties.RightMargin),
						unitConverter.ModelUnitsToPointsF(properties.BottomMargin),
						unitConverter.ModelUnitsToPointsF(properties.LeftMargin)));
				}
				width -= 2 * shape.OutlineWidth + properties.LeftMargin + properties.RightMargin;
				height -= shape.OutlineWidth + properties.TopMargin + properties.BottomMargin;
			}
			style.Add(DXHtmlTextWriterStyle.Width, String.Format(CultureInfo.InvariantCulture, "{0}pt", unitConverter.ModelUnitsToPointsF(width)));
			style.Add(DXHtmlTextWriterStyle.Height, String.Format(CultureInfo.InvariantCulture, "{0}pt", unitConverter.ModelUnitsToPointsF(height)));
			HtmlCssFloat cssFloat = GetCssFloat(run);
			if (cssFloat != HtmlCssFloat.NotSet) {
				if (cssFloat == HtmlCssFloat.Left)
					control.Style.Add("float", "left");
				else if (cssFloat == HtmlCssFloat.Right)
					control.Style.Add("float", "right");
			}
		}
		protected internal virtual void ExportFloatingObjectContent(FloatingObjectAnchorRun run) {
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null) {
				ExportFloatingObjectTextBoxContent(textBoxContent, run);
				return;
			}
			PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
			if (pictureContent != null)
				ExportFloatingObjectPictureContent(pictureContent, run);
		}
		protected internal virtual void ExportFloatingObjectTextBoxContent(TextBoxFloatingObjectContent content, FloatingObjectAnchorRun run) {
			PerformExportPieceTable(content.TextBox.PieceTable, ExportPieceTable);
		}
		protected internal virtual void ExportFloatingObjectPictureContent(PictureFloatingObjectContent content, FloatingObjectAnchorRun run) {
			AddControlToChild(CurrentParent, CreateImageControl(content, run));
		}
		protected virtual DXWebControlBase CreateImageControl(PictureFloatingObjectContent content, FloatingObjectAnchorRun run) {
			return exportHelper.CreateImageControl(content.Image, run.FloatingObjectProperties.ActualSize, HtmlCssFloat.NotSet);
		}
		HtmlCssFloat GetCssFloat(FloatingObjectAnchorRun run) {
			FloatingObjectProperties properties = run.FloatingObjectProperties;
			if (properties.TextWrapType != FloatingObjectTextWrapType.Square && properties.TextWrapType == FloatingObjectTextWrapType.Tight)
				return HtmlCssFloat.NotSet;
			if (properties.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.Left)
				return HtmlCssFloat.Left;
			else if (properties.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.Right)
				return HtmlCssFloat.Right;
			return HtmlCssFloat.NotSet;
		}
	}
	#endregion
	class HtmlVisitableDocumentIntervalBasedObjectBoundaryIterator : VisitableDocumentIntervalBasedObjectBoundaryIterator {
		class VisitableDocumentIntervalBoundaryProxy : VisitableDocumentIntervalBoundary {
			readonly VisitableDocumentIntervalBoundary boundary;
			DocumentLogPosition position;
			public VisitableDocumentIntervalBoundaryProxy(VisitableDocumentIntervalBoundary boundary)
				: base(boundary.VisitableInterval) {
				this.boundary = boundary;
				this.position = boundary.Position.LogPosition;
			}
			public override DocumentModelPosition Position {
				get {
					return PositionConverter.ToDocumentModelPosition(boundary.VisitableInterval.PieceTable, position);
				}
			}
			public DocumentLogPosition LogPosition { get { return position; } }
			public override BookmarkBoundaryOrder Order { get { return boundary.Order; } }
			public override void Export(DocumentModelExporter exporter) {
				this.boundary.Export(exporter);
			}
			protected internal override Layout.VisitableDocumentIntervalBox CreateBox() {
				return this.boundary.CreateBox();
			}
			public void ChangePosition(DocumentLogPosition value) {
				this.position = value;
			}
		}
		public HtmlVisitableDocumentIntervalBasedObjectBoundaryIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void PopulateBoundariesCore<T>(List<T> intervals) {
			VisitableDocumentIntervalStartBoundaryFactory startBoundaryFactory = CreateVisitableDocumentIntervalStartBoundaryFactory();
			VisitableDocumentIntervalEndBoundaryFactory endBoundaryFactory = CreateVisitableDocumentIntervalEndBoundaryFactory();
			int count = intervals.Count;
			VisitableDocumentIntervalBoundaryProxy prevEndBoundary = null;
			for (int i = 0; i < count; i++) {
				VisitableDocumentInterval interval = intervals[i];
				if (!IsVisibleInterval(interval))
					continue;
				interval.Visit(startBoundaryFactory);
				VisitableDocumentIntervalBoundaryProxy startBoundary = new VisitableDocumentIntervalBoundaryProxy(startBoundaryFactory.Boundary);
				startBoundary.IntervalIndex = i;
				if (prevEndBoundary != null && startBoundary.Position < prevEndBoundary.Position)
					prevEndBoundary.ChangePosition(startBoundary.Position.LogPosition);
				Field hyperlink = PieceTable.GetHyperlinkField(startBoundary.Position.RunIndex);
				if (hyperlink != null)
					startBoundary.ChangePosition(DocumentModelPosition.FromRunEnd(PieceTable, hyperlink.LastRunIndex).LogPosition);
				Boundaries.Add(startBoundary);
				interval.Visit(endBoundaryFactory);
				VisitableDocumentIntervalBoundaryProxy endBoundary = new VisitableDocumentIntervalBoundaryProxy(endBoundaryFactory.Boundary);
				if (endBoundary.LogPosition > PieceTable.DocumentEndLogPosition)
					endBoundary.ChangePosition(PieceTable.DocumentEndLogPosition);
				endBoundary.IntervalIndex = i;
				if (endBoundary.Position < startBoundary.Position)
					endBoundary.ChangePosition(startBoundary.Position.LogPosition);
				else if (endBoundary.Position > startBoundary.Position) {
					hyperlink = PieceTable.GetHyperlinkField(endBoundary.Position.RunIndex);
					if (hyperlink != null)
						endBoundary.ChangePosition(DocumentModelPosition.FromRunStart(PieceTable, hyperlink.FirstRunIndex).LogPosition);
				}
				Boundaries.Add(endBoundary);
				prevEndBoundary = endBoundary;
			}
		}
	}
	#region HtmlExportHelper
	public class HtmlExportHelper {
		#region Fields
		readonly DocumentModel documentModel;
		readonly IScriptContainer scriptContainer;
		readonly IOfficeImageRepository imageRepository;
		readonly HtmlStyleHelper htmlStyleHelper;
		readonly HtmlParagraphStyleHelper paragraphStyleHelper;
		readonly HtmlTableStyleHelper tableStyleHelper;
		readonly HtmlParagraphTabsHelper paragraphTabsHelper;
		readonly OfficeHtmlImageHelper imageHelper;
		readonly HtmlDocumentExporterOptions options;
		#endregion
		public HtmlExportHelper(DocumentModel documentModel, IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlDocumentExporterOptions options) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(scriptContainer, "scriptContainer");
			Guard.ArgumentNotNull(imageRepository, "imageRepository");
			Guard.ArgumentNotNull(options, "options");
			this.documentModel = documentModel;
			this.scriptContainer = scriptContainer;
			this.imageRepository = imageRepository;
			this.options = options;
			this.htmlStyleHelper = new HtmlStyleHelper();
			this.tableStyleHelper = new HtmlTableStyleHelper(documentModel.UnitConverter, htmlStyleHelper);
			this.paragraphStyleHelper = new HtmlParagraphStyleHelper(documentModel.UnitConverter, htmlStyleHelper);
			this.paragraphTabsHelper = new HtmlParagraphTabsHelper(documentModel.UnitConverter, htmlStyleHelper);
			this.imageHelper = new OfficeHtmlImageHelper(documentModel, documentModel.UnitConverter);
			if (this.options.OverrideImageResolution != HtmlDocumentExporterOptions.DefaultOverrideImageResolution) { 
				this.imageHelper.HorizontalResolution = this.options.OverrideImageResolution;
				this.imageHelper.VerticalResolution = this.options.OverrideImageResolution;
			}
		}
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return documentModel.UnitConverter; } }
		protected internal bool IsExportInlineStyle { get { return options.CssPropertiesExportType == CssPropertiesExportType.Inline; } }
		protected internal HtmlStyleHelper HtmlStyleHelper { get { return htmlStyleHelper; } }
		protected internal virtual bool DisposeConvertedImagesImmediately { get { return options.DisposeConvertedImagesImmediately; } }
		public DXWebControlBase CreateTextControl(string text, HtmlExporterTextProperties textProperties) {
			Guard.ArgumentNonNegative(textProperties.FontCacheIndex, "fontCacheIndex");
			DXHtmlGenericControl control = new DXHtmlGenericControl(DXHtmlTextWriterTag.Span);
			AppendText(control, text);
			return control;
		}
		public void SetTextProperties(DXHtmlControl control, HtmlExporterTextProperties textProperties, bool forceExportCharacterProperties) {
			FontInfo fontInfo = documentModel.FontCache[textProperties.FontCacheIndex];
			bool useFontSizeInPixels = options.FontUnit == HtmlFontUnit.Pixel;
			bool isDXHtmlAnchor = control is DXHtmlAnchor;
			if (IsExportInlineStyle) {
				HtmlStyleRender.GetHtmlStyle(fontInfo.Name, textProperties.DoubleFontSize / 2f, GraphicsUnit.Point, fontInfo.Bold, fontInfo.Italic, textProperties.Strikeout, textProperties.Underline, textProperties.ForeColor, textProperties.BackColor, useFontSizeInPixels, control.Style);
				if (!options.DefaultCharacterPropertiesExportToCss && !forceExportCharacterProperties)
					RemoveDefaultProperties(textProperties, fontInfo, control.Style);
				if (textProperties.AllCaps)
					control.Style.Add("text-transform", "uppercase");
				if (isDXHtmlAnchor && !textProperties.Underline && !textProperties.Strikeout)
					control.Style.Add("text-decoration", "none");
			}
			else {
				DXCssStyleCollection styleCollection = new DXCssStyleCollection();
				HtmlStyleRender.GetHtmlStyle(fontInfo.Name, textProperties.DoubleFontSize / 2f, GraphicsUnit.Point, fontInfo.Bold, fontInfo.Italic, textProperties.Strikeout, textProperties.Underline, textProperties.ForeColor, textProperties.BackColor, useFontSizeInPixels, styleCollection);
				if (!options.DefaultCharacterPropertiesExportToCss && !forceExportCharacterProperties)
					RemoveDefaultProperties(textProperties, fontInfo, styleCollection);
				string style = styleCollection.Value;
				if (style == null)
					style = String.Empty;
				if (textProperties.AllCaps)
					style += "text-transform:uppercase;";
				if (isDXHtmlAnchor && !textProperties.Underline && !textProperties.Strikeout)
					style += "text-decoration: none;";
				if (!String.IsNullOrEmpty(style)) {
					string styleName = scriptContainer.RegisterCssClass(style);
					control.CssClass = styleName;
				}
			}
		}
		void RemoveDefaultProperties(HtmlExporterTextProperties textProperties, FontInfo fontInfo, DXCssStyleCollection dXCssStyleCollection) {
			CharacterFormattingInfo defaultCharacterProperties = CreateDefaultCharacterProperties(documentModel);
			RemoveDefaultProperties(defaultCharacterProperties, fontInfo.Name, fontInfo.Bold, fontInfo.Italic, textProperties.DoubleFontSize, textProperties.BackColor, textProperties.ForeColor, dXCssStyleCollection);
		}
		internal static CharacterFormattingInfo CreateDefaultCharacterProperties(DocumentModel documentModel) {
			CharacterPropertiesMerger merger;
			if (documentModel.CharacterStyles.Count > 0) {
				merger = new CharacterPropertiesMerger(documentModel.CharacterStyles[0].CharacterProperties);
				merger.Merge(documentModel.DefaultCharacterProperties);
			}
			else {
				merger = new CharacterPropertiesMerger(documentModel.DefaultCharacterProperties);
			}
			CharacterFormattingInfo defaultCharacterProperties = merger.MergedProperties.Info;
			return defaultCharacterProperties;
		}
		static void TryRemoveKey(DXCssStyleCollection dXCssStyleCollection, string key) {
			if(!String.IsNullOrEmpty(dXCssStyleCollection[key]))
				dXCssStyleCollection.Remove(key);
		}
		internal static void RemoveDefaultProperties(CharacterFormattingInfo characterProperties, string fontName, bool fontBold, bool fontItalic, int doubleFontSize, Color backColor, Color foreColor, DXCssStyleCollection style) {
			if (backColor.Equals(characterProperties.BackColor))
				TryRemoveKey(style, "background-color");
			if (doubleFontSize == characterProperties.DoubleFontSize)
				TryRemoveKey(style, "font-size");
			if (fontName == characterProperties.FontName)
				TryRemoveKey(style, "font-family");
			if (fontBold == characterProperties.FontBold)
				TryRemoveKey(style, "font-weight");
			if (fontItalic == characterProperties.FontItalic)
				TryRemoveKey(style, "font-style");
			if (foreColor.Equals(characterProperties.ForeColor))
				TryRemoveKey(style, "color");
		}
		public void AppendText(DXWebControlBase parent, string text) {
			parent.Controls.Add(new DXHtmlLiteralControl(ReplaceWhiteSpaceWithNonBreakingSpace(text)));
		}
		StringBuilder text = new StringBuilder();
		protected internal string ReplaceWhiteSpaceWithNonBreakingSpace(string rawText) {
			int doubleSpaceIndex = rawText.IndexOf("  ", StringComparison.Ordinal);
			if (doubleSpaceIndex < 0)
				return rawText;
			bool isPrevWhiteSpace = false;
			int count = rawText.Length;
			text.Capacity = Math.Max(text.Capacity, count + Math.Max(64, count / 20));
			if (doubleSpaceIndex > 0)
				text.Append(rawText.Substring(0, doubleSpaceIndex));
			for (int i = doubleSpaceIndex; i < count; i++) {
				if (rawText[i] == ' ')
					isPrevWhiteSpace = ProcessSpace(rawText[i], isPrevWhiteSpace);
				else {
					if (isPrevWhiteSpace)
						isPrevWhiteSpace = false;
					text.Append(rawText[i]);
				}
			}
			string result = text.ToString();
			text.Length = 0;
			return result;
		}
		internal bool ProcessSpace(char ch, bool isPrevWhiteSpace) {
			if (isPrevWhiteSpace)
				text.Append("&nbsp;");
			else {
				isPrevWhiteSpace = true;
				text.Append(ch);
			}
			return isPrevWhiteSpace;
		}
		public DXWebControlBase CreateTableControl(Table table) {
			Guard.ArgumentNotNull(table, "table");
			DXHtmlGenericControl tableControl = new DXHtmlGenericControl(DXHtmlTextWriterTag.Table);
			tableControl.Attributes.Add("border", "0");
			tableControl.Attributes.Add("cellspacing", ConvertWidthUnitToPixels(table.CellSpacing));
			tableControl.Attributes.Add("cellpadding", "0");
			if (table.PreferredWidth.Value != 0)
				tableControl.Attributes.Add("width", ConvertWidthUnitToPixels(table.PreferredWidth));
			if (table.CellSpacing.Value == 0)
				tableControl.Style.Add("border-collapse", "collapse");
			if (table.TableAlignment != TableRowAlignment.Left)
				tableControl.Attributes.Add("align", tableStyleHelper.GetHtmlTableAlignment(table.TableAlignment));
			if (IsExportInlineStyle)
				tableStyleHelper.GetHtmlTableStyle(table, tableControl.Style);
			else {
				string style = tableStyleHelper.GetHtmlTableStyle(table);
				tableControl.CssClass = scriptContainer.RegisterCssClass(style);
			}
			return tableControl;
		}
		public DXWebControlBase CreateTableRowControl(TableRow row) {
			Guard.ArgumentNotNull(row, "tableRow");
			DXHtmlGenericControl tableRowControl = new DXHtmlGenericControl(DXHtmlTextWriterTag.Tr);
			if (row.Properties.UseHeight) {
				string rowHeight = HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(row.Height.Value));
				tableRowControl.Style.Add("height", rowHeight);
			}
			return tableRowControl;
		}
		public DXWebControlBase CreateTableCellControl(TableCell cell, VerticalMergeCellProperties mergedCellProperties) {
			Guard.ArgumentNotNull(cell, "tableCell");
			DXHtmlGenericControl tableCellControl = new DXHtmlGenericControl(DXHtmlTextWriterTag.Td);
			if (cell.ColumnSpan > 1)
				tableCellControl.Attributes.Add("colspan", cell.ColumnSpan.ToString());
			int rowSpan = mergedCellProperties.RowSpan;
			if (rowSpan > 1)
				tableCellControl.Attributes.Add("rowspan", rowSpan.ToString());
			if (cell.VerticalAlignment != VerticalAlignment.Center)
				tableCellControl.Attributes.Add("valign", tableStyleHelper.GetHtmlVerticalAlignment(cell.VerticalAlignment));
			if (cell.PreferredWidth.Value != 0)
				tableCellControl.Attributes.Add("width", ConvertWidthUnitToPixels(cell.PreferredWidth));
			if (cell.NoWrap)
				tableCellControl.Attributes.Add("nowrap", String.Empty);
			if (IsExportInlineStyle)
				tableStyleHelper.GetHtmlTableCellStyle(cell, tableCellControl.Style, mergedCellProperties.ActualBottomBorder);
			else {
				string style = tableStyleHelper.GetHtmlTableCellStyle(cell, mergedCellProperties.ActualBottomBorder);
				tableCellControl.CssClass = scriptContainer.RegisterCssClass(style);
			}
			return tableCellControl;
		}
		string ConvertWidthUnitToPixels(WidthUnit widthUnit) {
			return tableStyleHelper.ConvertWidthUnitToPixels(widthUnit);
		}
		public virtual DXWebControlBase CreateParagraphControl(Paragraph paragraph, bool ignoreOutlineLevel, int listControlNestingLevel, int parentLevelOffset) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			DXHtmlGenericControl paragraphControl = new DXHtmlGenericControl(CalculateParagraphTag(paragraph, ignoreOutlineLevel));
			string tabStops = paragraphTabsHelper.CreateTabStops(paragraph);
			if (tabStops.Length > 0)
				paragraphControl.Style.Add("tab-stops", tabStops);
			if (IsExportInlineStyle)
				paragraphStyleHelper.GetHtmlParagraphStyle(paragraph, listControlNestingLevel, parentLevelOffset, paragraphControl.Style);
			else {
				string style = paragraphStyleHelper.GetHtmlParagraphStyle(paragraph, listControlNestingLevel, parentLevelOffset);
				paragraphControl.CssClass = scriptContainer.RegisterCssClass(style);
			}
			return paragraphControl;
		}
		protected internal virtual DXHtmlTextWriterTag CalculateParagraphTag(Paragraph paragraph, bool ignoreOutlineLevel) {
			if (ignoreOutlineLevel)
				return DXHtmlTextWriterTag.P;
			switch (paragraph.OutlineLevel) {
				case 1:
					return DXHtmlTextWriterTag.H1;
				case 2:
					return DXHtmlTextWriterTag.H2;
				case 3:
					return DXHtmlTextWriterTag.H3;
				case 4:
					return DXHtmlTextWriterTag.H4;
				case 5:
					return DXHtmlTextWriterTag.H5;
				case 6:
					return DXHtmlTextWriterTag.H6;
				default:
					return DXHtmlTextWriterTag.P;
			}
		}
		public DXWebControlBase CreateNumberingListControl(Paragraph paragraph, NumberingList numberingList, int counter) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			DXHtmlTextWriterTag htmlTextWriterTag;
			if (NumberingListHelper.GetListType(numberingList.AbstractNumberingList) == NumberingType.Bullet)
				htmlTextWriterTag = DXHtmlTextWriterTag.Ul;
			else
				htmlTextWriterTag = DXHtmlTextWriterTag.Ol;
			NumberingListWebControl control = new NumberingListWebControl(htmlTextWriterTag, numberingList.AbstractNumberingListIndex, paragraph.GetListLevelIndex());
			control.Style.Add(DXHtmlTextWriterStyle.MarginTop, "0");
			control.Style.Add(DXHtmlTextWriterStyle.MarginBottom, "0");
			if (ShouldAddNumberingListStartAttribute(paragraph, numberingList)) {
				control.Attributes.Add("start", counter.ToString());
			}
			return control;
		}
		bool ShouldAddNumberingListStartAttribute(Paragraph paragraph, NumberingList numberingList) {
			bool isFirstParagraph = paragraph.Index == ParagraphIndex.Zero;
			int curParagraphLevel = paragraph.GetListLevelIndex();
			if (isFirstParagraph) {
				return paragraph.IsInList()
					&& numberingList.AbstractNumberingList.Levels[curParagraphLevel].ListLevelProperties.Start != 1;
			}
			Paragraph previousParagraph = paragraph.PieceTable.Paragraphs[paragraph.Index - 1];
			int prevParagraphLevel = previousParagraph.GetListLevelIndex();
			bool differNumberingLists = previousParagraph.GetNumberingListIndex() != paragraph.GetNumberingListIndex();
			if (previousParagraph.IsInList() && prevParagraphLevel > curParagraphLevel
				&& differNumberingLists)
				return true;
			if (prevParagraphLevel == curParagraphLevel
				&& differNumberingLists)
				return true;
			return false;
		}
		public virtual DXWebControlBase CreateListLevelControl(Paragraph paragraph, NumberingList numberingList, int listControlNestingLevel, int parentLevelOffset, int counter) {
			DXHtmlGenericControl levelControl = new DXHtmlGenericControl(DXHtmlTextWriterTag.Li);
			string style;
			int listLevelIndex = paragraph.GetListLevelIndex();
			IListLevel listLevel = numberingList.Levels[listLevelIndex];
			if (IsExportInlineStyle)
				paragraphStyleHelper.GetHtmlParagraphInListStyle(paragraph, listLevel, listControlNestingLevel, parentLevelOffset, levelControl.Style, options.DefaultCharacterPropertiesExportToCss);
			else {
				style = paragraphStyleHelper.GetHtmlParagraphInListStyle(paragraph, listLevel, listControlNestingLevel, parentLevelOffset);
				levelControl.CssClass = scriptContainer.RegisterCssClass(style);
			}
			if (listLevel.ListLevelProperties.Start != 1 && listLevel.ListLevelProperties.Start == counter) 
				levelControl.Attributes.Add("value", listLevel.ListLevelProperties.Start.ToString());
			return levelControl;
		}
		internal int GetMostNestedCounter(int[] counters) {
			return counters[counters.Length - 1];
		}
		public DXWebControlBase CreateImageControl(OfficeImage image, Size actualSize, HtmlCssFloat cssFloat) {
			Guard.ArgumentNotNull(image, "image");
			WebImageControl imageControl = CreateImageControlInternal(image, actualSize, cssFloat);
			DXHtmlGenericControl control = new DXHtmlGenericControl(DXHtmlTextWriterTag.Span);
			control.Controls.Add(imageControl);
			return control;
		}
		internal WebImageControl CreateImageControlInternal(OfficeImage image, Size actualSize, HtmlCssFloat cssFloat) {
			WebImageControl imageControl = new WebImageControl();
			imageHelper.ApplyImageProperties(imageControl, image, actualSize, imageRepository, DisposeConvertedImagesImmediately, true, options.ExportImageSize == ExportImageSize.Always, options.KeepExternalImageSize);
			if (cssFloat != HtmlCssFloat.NotSet) {
				if (cssFloat == HtmlCssFloat.Left)
					imageControl.Style.Add("float", "left");
				else if (cssFloat == HtmlCssFloat.Right)
					imageControl.Style.Add("float", "right");
			}
			imageControl.GenerateEmptyAlternateText = true;
			return imageControl;
		}
		public DXWebControlBase CreateImageControl(InlinePictureRun run) {
			Guard.ArgumentNotNull(run, "run");
			return CreateImageControl(run.Image, run.ActualSize, HtmlCssFloat.NotSet);
		}
	}
	#endregion
	#region NumberingListWebControl
	public class NumberingListWebControl : DXHtmlGenericControl {
		readonly AbstractNumberingListIndex abstractListIndex;
		readonly int currentLevelIndex;
		public NumberingListWebControl(DXHtmlTextWriterTag htmlTextWriterTag, AbstractNumberingListIndex abstractListIndex, int currentLevelIndex)
			: base(htmlTextWriterTag) {
			this.abstractListIndex = abstractListIndex;
			this.currentLevelIndex = currentLevelIndex;
		}
		public AbstractNumberingListIndex AbstractListIndex { get { return abstractListIndex; } }
		public int CurrentLevelIndex { get { return currentLevelIndex; } }
	}
	#endregion
}
