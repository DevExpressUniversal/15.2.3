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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using ModelUnit = System.Int32;
using LayoutUnit = System.Int32;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	#region DocumentDataRegistrator
	public partial class DocumentDataRegistrator : DocumentModelExporter {
		#region fields
		List<Section> sectionPages;
		Section currentSection;
		List<string> fontNames;
		PictureStyleInfoTable pictureAutoStyles;
		FloatingObjectStyleInfoTable floatingObjectAutoStyles;
		CharacterStyleInfoTable characterAutoStyles;
		ParagraphStyleInfoTable paragraphAutoStyles;
		TableStyleInfoTable tableStyleInfoTable;
		TableCellStyleInfoTable tableCellStyleInfoTable;
		TableRowStyleInfoTable tableRowStyleInfoTable;
		FieldRegistrator fieldRegistrator;
		NumberingListRegistrator listRegistrator;
		ParagraphsLineNumberInfo paragraphsLineNumberInfo;
		int lineNumbersIncrement;
		TableColumnStyleInfoTable tableColumnsInfoTable;
		List<Section> editableSections;
		DocumentLayout documentLayout;
		#endregion
		public DocumentDataRegistrator(DocumentModel documentModel)
			: base(documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			sectionPages = new List<Section>();
			fontNames = new List<string>();
			pictureAutoStyles = new PictureStyleInfoTable();
			floatingObjectAutoStyles = new FloatingObjectStyleInfoTable();
			characterAutoStyles = new CharacterStyleInfoTable();
			paragraphAutoStyles = new ParagraphStyleInfoTable();
			fieldRegistrator = new FieldRegistrator(CharacterAutoStyles);
			listRegistrator = new NumberingListRegistrator();
			this.paragraphsLineNumberInfo = new ParagraphsLineNumberInfo();
			tableColumnsInfoTable = new TableColumnStyleInfoTable();
			tableStyleInfoTable = new TableStyleInfoTable();
			tableCellStyleInfoTable = new TableCellStyleInfoTable();
			tableRowStyleInfoTable = new TableRowStyleInfoTable();
			editableSections = new List<Section>();
			documentLayout = null;
		}
		#region Properties
		public List<string> FontNames { get { return fontNames; } }
		public CharacterStyleInfoTable CharacterAutoStyles { get { return characterAutoStyles; } }
		public FloatingObjectStyleInfoTable FloatingObjectAutoStyles { get { return floatingObjectAutoStyles; } }
		public PictureStyleInfoTable PictureAutoStyles { get { return pictureAutoStyles; } }
		public ParagraphStyleInfoTable ParagraphAutoStyles { get { return paragraphAutoStyles; } }
		public TableStyleInfoTable TableStyleInfoTable { get { return tableStyleInfoTable; } }
		public TableCellStyleInfoTable TableCellStyleInfoTable { get { return tableCellStyleInfoTable; } }
		public TableColumnStyleInfoTable TableColumnsInfoTable { get { return tableColumnsInfoTable; } }
		public TableRowStyleInfoTable TableRowStyleInfoTable { get { return tableRowStyleInfoTable; } }
		public List<Section> SectionPages { get { return sectionPages; } }
		public Dictionary<FieldCodeStartRun, string> FieldStyles { get { return FieldRegistrator.FieldStyles; } }
		protected internal FieldRegistrator FieldRegistrator { get { return fieldRegistrator; } }
		protected internal NumberingListRegistrator ListRegistrator { get { return listRegistrator; } }
		internal Section CurrentSection { get { return currentSection; } set { currentSection = value; } }
		public int LineNumbersIncrement { get { return lineNumbersIncrement; } set { lineNumbersIncrement = value; } }
		public ParagraphsLineNumberInfo ParagraphsLineNumber { get { return paragraphsLineNumberInfo; } }
		public List<Section> EditableSections { get { return editableSections; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		#endregion
		protected internal override void ExportDocument() {
			PushVisitableDocumentIntervalBoundaryIterator();
			RegisterDocumentFonts();
			RegisterNumberingListsStyles();
			RegisterRangePermissions();
			base.ExportDocument();
			PopVisitableDocumentIntervalBoundaryIterator();
		}
		protected internal virtual void RegisterRangePermissions() {
			EditableSections.Clear();
			RangePermissionCollection permissions = DocumentModel.MainPieceTable.RangePermissions;
			int count = permissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission permission = permissions[i];
				List<Section> sections = FindSectionsByPermission(permission);
				AddEditableSections(sections);
			}
		}
		private List<Section> FindSectionsByPermission(RangePermission permission) {
			SectionIndex startIndex = DocumentModel.FindSectionIndex(permission.Start);
			SectionIndex endIndex = DocumentModel.FindSectionIndex(permission.End);
			return DocumentModel.Sections.GetRange(startIndex, endIndex - startIndex + 1);
		}
		protected void AddEditableSections(List<Section> sections) {
			int count = sections.Count;
			for (int i = 0; i < count; i++) {
				Section section = sections[i];
				if (!EditableSections.Contains(section))
					EditableSections.Add(section);
			}
		}
		private void RegisterNumberingListsStyles() {
			NumberingListCollection lists = DocumentModel.NumberingLists;
			int count = lists.Count;
			for (int i = 0; i < count; i++) {
				NumberingList list = lists[new NumberingListIndex(i)];
				RegisterNumberingListLevelStyle(list.Levels);
			}
		}
		private void RegisterNumberingListLevelStyle(IReadOnlyIListLevelCollection levels) {
			int count = levels.Count;
			for (int i = 0; i < count; i++) {
				IListLevel level = levels[i];
				CharacterAutoStyles.RegisterStyle(level.CharacterProperties, StyleInfoBase.EmptyStyleIndex, true);
				RegisterFontName(level.CharacterProperties);
			}
		}
		protected internal void RegisterDocumentFonts() {
			RegisterDocumentParagraphFonts();
			RegisterDocumentCharacterFonts();
		}
		protected internal virtual void RegisterDocumentParagraphFonts() {
			ParagraphStyleCollection styles = DocumentModel.ParagraphStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				RegisterFontName(styles[i].CharacterProperties);
		}
		protected internal virtual void RegisterDocumentCharacterFonts() {
			CharacterStyleCollection styles = DocumentModel.CharacterStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				RegisterFontName(styles[i].CharacterProperties);
		}
		protected void RegisterFontName(CharacterProperties properties) {
			if (properties.UseFontName && !FontNames.Contains(properties.FontName))
				FontNames.Add(properties.FontName);
		}
		protected internal virtual void RegisterCharacterStyle(CharacterProperties properties, int styleIndex, bool isUsedInMainPieceTable) {
			if (!ShouldExportCharacterProperties(properties))
				return;
			CharacterAutoStyles.RegisterStyle(properties, styleIndex, isUsedInMainPieceTable);
		}
		protected internal override void ExportTextRun(TextRun run) {
			RegisterCharacterStyle(run.CharacterProperties, run.CharacterStyleIndex, run.Paragraph.PieceTable.IsMain);
			RegisterFontName(run.CharacterProperties); 
			fieldRegistrator.RegisterTextRun(run);
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			ExportTextRun(run);
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			ExportTextRun(run);
		}
		public bool ShouldExportCharacterProperties(CharacterProperties characterProperties) {
			return
				characterProperties.UseFontName ||
				characterProperties.UseDoubleFontSize ||
				characterProperties.UseFontBold ||
				characterProperties.UseFontItalic ||
				characterProperties.UseFontUnderlineType || characterProperties.UseUnderlineColor || characterProperties.UseUnderlineWordsOnly ||
				characterProperties.UseFontStrikeoutType || characterProperties.UseStrikeoutColor || characterProperties.UseStrikeoutWordsOnly ||
				characterProperties.UseAllCaps ||
				characterProperties.UseForeColor ||
				characterProperties.UseScript ||
				characterProperties.UseBackColor ||
				characterProperties.UseHidden;
		}
		internal bool ShouldExportParagraphPropertiesCore(ParagraphProperties properties) {
			return
				properties.UseAlignment ||
				properties.UseFirstLineIndent ||
				properties.UseFirstLineIndentType ||
				properties.UseLeftIndent ||
				properties.UseRightIndent ||
				properties.UseLineSpacing ||
				properties.UseLineSpacingType ||
				properties.UseSpacingAfter ||
				properties.UseSpacingBefore ||
				properties.UseSuppressLineNumbers ||
				properties.UsePageBreakBefore ||
				properties.UseKeepWithNext ||
				properties.UseKeepLinesTogether ||
				properties.UseWidowOrphanControl ||
				properties.UseBackColor;
		}
		public bool ShouldExportParagraphProperties(Paragraph paragraph) {
			return ShouldExportParagraphPropertiesCore(paragraph.ParagraphProperties) || ShouldExportTabProperties(paragraph.GetOwnTabs());
		}
		public static bool ShouldExportTabProperties(TabFormattingInfo tabs) {
			int nonDefaultTabCount = 0;
			int count = tabs.Count;
			for (int i = 0; i < count; i++)
				if (!tabs[i].IsDefault)
					nonDefaultTabCount++;
			return nonDefaultTabCount > 0;
		}
		#region Paragraphs
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			ExportParagraphRegisterParagraphForListRegistrator(paragraph);
			string masterPageName =  string.Empty;
			if (ShouldAddPageReference(paragraph) && Object.ReferenceEquals(paragraph.PieceTable, DocumentModel.MainPieceTable)) {
				masterPageName = GetMasterPageName(paragraph.Index);
			}
			ParagraphBreakType breakAfterType = CalculateBreakAfterType(paragraph);
			bool isStyleForTextBox = paragraph.PieceTable.IsTextBox;
			TextBoxContentType contentType = paragraph.PieceTable.ContentType as TextBoxContentType;
			if (isStyleForTextBox && contentType != null) 
				isStyleForTextBox &= contentType.AnchorRun.PieceTable.IsMain;
			ParagraphAutoStyles.RegisterStyle(paragraph, masterPageName, breakAfterType, paragraph.PieceTable.IsMain || isStyleForTextBox);
			return base.ExportParagraph(paragraph);
		}
		string GetMasterPageName(ParagraphIndex paragraphIndex) {			
			int sectionIndex = SectionPages.IndexOf(currentSection);
			if (ShouldAddFirstPageHeaderFooterReference(paragraphIndex))
				return NameResolver.CalculateMasterPageNameForFirstPageHeaderFooter(sectionIndex);
			else
				return NameResolver.CalculateMasterPageName(sectionIndex);
		}
		void ExportParagraphRegisterParagraphForListRegistrator(Paragraph paragraph) {
			TableCell tableCell = paragraph.GetCell();
			bool isCoveredCell = tableCell != null && tableCell.VerticalMerging == MergingState.Continue;
			int listsCount = DocumentModel.NumberingLists.Count;
			if (!isCoveredCell && listsCount > 0 && paragraph.IsInList() && paragraph.ShouldExportNumbering())
				ListRegistrator.RegisterParagraph(paragraph);
			else {
				bool isFirstParagraphInCell = tableCell != null && tableCell.StartParagraphIndex == paragraph.Index;
				if (listsCount > 0 && (!paragraph.IsInList() || isFirstParagraphInCell))
					ListRegistrator.RegisterParagraphAfterClosedList(CurrentSection, paragraph);
			}
		}
		private ParagraphBreakType CalculateBreakAfterType(Paragraph paragraph) {
			char lastCharacterInContent = new char();
			string paragraphContent = String.Empty;
			Debug.Assert(PieceTable.Runs[paragraph.LastRunIndex] is ParagraphRun);
			if (paragraph.LastRunIndex <= paragraph.FirstRunIndex)
				return ParagraphBreakType.None;
			if (paragraph.LastRunIndex - 1 == paragraph.FirstRunIndex) {
				paragraphContent = PieceTable.Runs[paragraph.FirstRunIndex].GetTextFast(PieceTable.TextBuffer);
			}
			else if (paragraph.LastRunIndex - 1 > paragraph.FirstRunIndex) { 
				paragraphContent = PieceTable.Runs[paragraph.LastRunIndex - 1].GetTextFast(PieceTable.TextBuffer);
			}
			if (String.IsNullOrEmpty(paragraphContent))
				return ParagraphBreakType.None;
			lastCharacterInContent = paragraphContent[paragraphContent.Length - 1];
			if (lastCharacterInContent == Characters.ColumnBreak)
				return ParagraphBreakType.Column;
			else if (lastCharacterInContent == Characters.PageBreak)
				return ParagraphBreakType.Page;
			return ParagraphBreakType.None;
		}
		public bool ShouldExportParagraphProperties(ParagraphStyle style) {
			return ShouldExportParagraphPropertiesCore(style.ParagraphProperties) || ShouldExportTabProperties(style.Tabs.GetTabs());
		}
		protected internal virtual bool IsNewPageSection() {
			SectionStartType startType = CurrentSection.GeneralSettings.StartType;
			return startType == SectionStartType.NextPage || startType == SectionStartType.EvenPage || startType == SectionStartType.OddPage;
		}
		protected internal virtual bool ShouldAddPageReference(Paragraph paragraph) {
			bool firstParagraph = CurrentSection.FirstParagraphIndex == paragraph.Index;
			return firstParagraph && IsNewPageSection() && paragraph.PieceTable.IsMain && !paragraph.IsInCell();
		}
		protected internal virtual bool ShouldAddPageReference(Table table) {
			bool firstParagraph = CurrentSection.FirstParagraphIndex == table.FirstRow.FirstCell.StartParagraphIndex;
			return firstParagraph && IsNewPageSection() && table.PieceTable.IsMain && table.NestedLevel == 0;
		}
		#endregion
		#region Section
		protected internal override void ExportSection(Section section) {
			if (CurrentSection != section)
				CurrentSection = section;
			ListRegistrator.RegisterSection(section);
			if (!SectionPages.Contains(section)) {
				SectionPages.Add(section); 
				if (section.LineNumbering.Step != 0)
					ExportSectionTurnOnLineNumbering(section);
				if (section.LineNumbering.Step == 0) {
					ExportSectionExcludeLineNumbering(section);
					base.ExportSection(section);
					ListRegistrator.FinalizeSection(section);
					return;
				}
				if (section.LineNumbering.NumberingRestartType == LineNumberingRestart.NewSection) {
					ExportSectionRestartNumbering(section);
				}
			}
			base.ExportSection(section);
			ListRegistrator.FinalizeSection(section);
		}
		protected internal virtual void ExportSectionTurnOnLineNumbering(Section section) {
			LineNumbersIncrement = section.LineNumbering.Step;
		}
		protected internal virtual void ExportSectionRestartNumbering(Section section) {
			OpenDocumentLineNumberingInfo info = new OpenDocumentLineNumberingInfo();
			info.NumberingRestartType = LineNumberingRestart.NewSection;
			info.StartingLineNumber = Math.Max(section.LineNumbering.StartingLineNumber, 1);
			ParagraphsLineNumber.Add(section.FirstParagraphIndex, info);
		}
		protected internal virtual void ExportSectionExcludeLineNumbering(Section section) {
			OpenDocumentLineNumberingInfo info = new OpenDocumentLineNumberingInfo();
			info.SectionExclude = true;
			ParagraphsLineNumber.Add(section.FirstParagraphIndex, info);
		}
		private bool ShouldAddFirstPageHeaderFooterReference(ParagraphIndex paragraphIndex) {
			return CurrentSection.GeneralSettings.DifferentFirstPage && currentSection.FirstParagraphIndex == paragraphIndex;			
		}
		#endregion
		#region Tables
		protected internal override ParagraphIndex ExportTable(TableInfo tableInfo) {
			Table table = tableInfo.Table;
			if (DocumentLayout == null && ShouldApplyRealWidthsToTable(table))
				CalculateDocumentLayoutOnce();
			int parentWidth = tableInfo.GetParentSectionWidth();
			if (IsEveryCellHasRealWidth(table) && table.PreferredWidth.Type != WidthUnitType.ModelUnits && table.PreferredWidth.Type != WidthUnitType.FiftiethsOfPercent) {
				parentWidth = GetTotalTableWidthConsiderMaxCellsWidth(table);
			}
			string masterPageName = String.Empty;
			if (ShouldAddPageReference(table)) {
				masterPageName = GetMasterPageName(table.FirstRow.FirstCell.StartParagraphIndex);
			}
			TableStyleInfoTable.RegisterStyle(table, parentWidth, masterPageName);
			TableGrid grid = tableInfo.GetTableGrid(parentWidth);
			OpenDocumentTableColumnsInfo newColumnsInfo = new OpenDocumentTableColumnsInfo(grid, tableInfo.Table.TableLayout == TableLayoutType.Autofit);
			newColumnsInfo.GenerateColumnsInfo(DocumentModel, TableStyleInfoTable[table].Name);
			TableColumnsInfoTable.RegisterStyle(table, newColumnsInfo);
			Paragraph firstTableParagraph = PieceTable.Paragraphs[table.FirstRow.FirstCell.StartParagraphIndex];
			if (DocumentModel.NumberingLists.Count > 0 && firstTableParagraph.IsInList())
				ListRegistrator.CloseNumberingListBeforeTableStart();
			return base.ExportTable(tableInfo);
		}
		void ApplyRealWidthsToTables(DocumentLayout documentLayout) {
#if DEBUGTEST
			Debug.Assert(documentLayout != null);
#endif
			int pagesCount = DocumentLayout.Pages.Count;
			for (int pageId = 0; pageId < pagesCount; pageId++) {
				Page page = DocumentLayout.Pages[pageId];
				int areasCount = page.Areas.Count;
				for (int areaId = 0; areaId < areasCount; areaId++) {
					PageArea pageArea = page.Areas[areaId];
					int columnsCount = pageArea.Columns.Count;
					for (int columnId = 0; columnId < columnsCount; columnId++)
						ApplyRealWidthsToColumnTables(pageArea.Columns[columnId]);
				}
			}
		}
		protected internal void ApplyRealWidthsToColumnTables(Column column) {
			TableViewInfoCollection tables = column.InnerTables;
			if (tables == null)
				return;
			int tablesInColumn = tables.Count;
			for (int tableId = 0; tableId < tablesInColumn; tableId++) {
				TableViewInfo tableViewInfo = tables[tableId];
				if (tableViewInfo.PrevTableViewInfo != null)
					continue;
				FromAutoToRealWidthsTableCalculator.ApplyRealWidths(tableViewInfo);
				StoreParentWidthForTable(column.Bounds, tableViewInfo);
			}
		}
		protected internal virtual void StoreParentWidthForTable(Rectangle columnBounds, TableViewInfo tableViewInfo) {
			Table modelTable = tableViewInfo.Table;
			if (!tableStyleInfoTable.ContainsKey(modelTable)) {
				TableStyleInfoTable.RegisterStyle(modelTable, Int32.MinValue, ShouldAddPageReference(modelTable) ? GetMasterPageName(modelTable.FirstRow.FirstCell.StartParagraphIndex) : String.Empty);
			}
			TableStyleInfo tableStyle = tableStyleInfoTable[modelTable];
			ModelUnit rightCellMargin = GetActualCellRightMargin(tableViewInfo.Table.Rows.First.Cells.First);
			LayoutUnit layoutRightCellMaring = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(rightCellMargin);
			LayoutUnit percentBaseWidth = tableViewInfo.Column.Bounds.Width;
			if (tableViewInfo.Table.NestedLevel == 0)
				percentBaseWidth += layoutRightCellMaring - tableViewInfo.LeftOffset;
			tableStyle.ParentWidth = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(percentBaseWidth);
		}
		ModelUnit GetActualCellRightMargin(TableCell cell) {
			WidthUnit margin = cell.GetActualRightMargin();
			if (margin.Type == WidthUnitType.ModelUnits)
				return margin.Value;
			return 0;
		}
		bool ShouldApplyRealWidthsToTable(Table table) {
			return !IsEveryCellHasRealWidth(table);
		}
		bool IsEveryCellHasRealWidth(Table table) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableCellCollection cells = rows[i].Cells;
				int cellCount = cells.Count; ;
				for (int j = 0; j < cellCount; j++) {
					if (cells[j].PreferredWidth.Type != WidthUnitType.ModelUnits)
						return false;
				}
			}
			return true;
		}
		void CalculateDocumentLayoutOnce() {
			if (documentLayout != null)
				return;
			IDocumentLayoutService documentLayoutService = DocumentModel.GetService<IDocumentLayoutService>();
			if (documentLayoutService == null)
				return;
			documentLayout = documentLayoutService.CalculateDocumentLayout();
			ApplyRealWidthsToTables(documentLayout);
		}
		int GetTotalTableWidthConsiderMaxCellsWidth(Table table) {
			int result = 0;
			int rowsCount = table.Rows.Count;
			for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++) {
				TableRow row = table.Rows[rowIndex];
				int cellsCount = row.Cells.Count;
				int cellsWidth = 0;
				for (int cellIndex = 0; cellIndex < cellsCount; cellIndex++) {
					Debug.Assert(row.Cells[cellIndex].PreferredWidth.Type == WidthUnitType.ModelUnits);
					cellsWidth += row.Cells[cellIndex].PreferredWidth.Value;
				}
				result = Math.Max(result, cellsWidth);
			}
			return result;
		}
		protected internal override void ExportRow(TableRow row, TableInfo tableInfo) {
			RegisterTableRow(row);
			base.ExportRow(row, tableInfo);
		}
		protected internal virtual void RegisterTableRow(TableRow row) {
			string tableStyleName = TableStyleInfoTable.GetName(row.Table);
			TableRowStyleInfoTable.RegisterStyle(row, tableStyleName);
		}
		public static bool ShouldExportTableProperties(Table table) {
			return true;
		}
		protected internal override void ExportCell(TableCell cell, TableInfo tableInfo) {
			if (cell.VerticalMerging != MergingState.Continue)
				RegisterTableCell(cell);
			base.ExportCell(cell, tableInfo);
			Paragraph lastParagraph = cell.PieceTable.Paragraphs[cell.EndParagraphIndex];
			if (DocumentModel.NumberingLists.Count > 0 && lastParagraph.IsInList())
				ListRegistrator.RegisterTableCellEnd();
		}
		protected internal virtual void RegisterTableCell(TableCell cell) {
			string tableRowName = TableRowStyleInfoTable.GetName(cell.Row);
			TableCellStyleInfoTable.RegisterStyle(cell, tableRowName);
		}
		#endregion
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			PictureAutoStyles.RegisterStyle(run.PictureProperties, run.Paragraph.PieceTable.IsMain);
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			FloatingObjectAutoStyles.RegisterStyle(run, run.Paragraph.PieceTable.IsMain);
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if(!Object.ReferenceEquals(textBoxContent, null))
				PerformExportPieceTable(textBoxContent.TextBox.PieceTable, ExportPieceTable);
		}
		protected internal override ProgressIndication CreateProgressIndication() {
			return new EmptyProgressIndication(DocumentModel);
		}
		#region DocumentFields
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			fieldRegistrator.RegisterCodeStartRun(run);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			fieldRegistrator.RegisterCodeEndRun();
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			fieldRegistrator.RegisterEndResultRun();
		}
		#endregion
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportFirstPageHeader(sectionHeader, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionHeader.PieceTable);
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportFirstPageFooter(sectionFooter, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionFooter.PieceTable);
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportEvenPageHeader(sectionHeader, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionHeader.PieceTable);
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportEvenPageFooter(sectionFooter, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionFooter.PieceTable);
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportOddPageHeader(sectionHeader, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionHeader.PieceTable);
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			base.ExportOddPageFooter(sectionFooter, linkedToPrevious);
			ListRegistrator.FinalizeHeaderFooter(sectionFooter.PieceTable);
		}
	}
	#endregion
	#region ParagraphsLineNumberInfo
	public class ParagraphsLineNumberInfo : Dictionary<ParagraphIndex, OpenDocumentLineNumberingInfo> {
	}
	#endregion
	#region OpenDocumentLineNumberingInfo
	public class OpenDocumentLineNumberingInfo {
		int startingLineNumber;
		bool exclude;
		LineNumberingRestart numberingRestartType;
		public int StartingLineNumber { get { return startingLineNumber; } set { startingLineNumber = value; } }
		public LineNumberingRestart NumberingRestartType { get { return numberingRestartType; } set { numberingRestartType = value; } }
		public bool SectionExclude { get { return exclude; } set { exclude = value; } }
	}
	#endregion
	#region NameResolver
	public static class NameResolver {
		public static string CalculateMasterPageName(int pageIndex) {
			if (pageIndex == 0)
				return "Standard";
			return String.Format("Standard{0}", pageIndex);
		}
		public static string CalculateMasterPageNameForFirstPageHeaderFooter(int pageIndex) {
			return String.Format("StandardF{0}", pageIndex);
		}
		public static string CalculatePageLayoutName(int pageIndex) {
			return string.Format("Mpm{0}", pageIndex);
		}
		internal static string CalculateNumberingListStyleName(int index) {
			return String.Format("L{0}", index);
		}
		internal static string CalculateSectionAutoStyleName(int index) {
			return String.Format("S{0}", index);
		}
		internal static string CalculateSectionName(int index) {
			return String.Format("Section{0}", index);
		}
		public static string CalculateParagraphAutoStyleName(int index) {
			return String.Format("P{0}", index);
		}
		public static string CalculateCharacterAutoStyleName(int index) {
			return String.Format("T{0}", index);
		}
		public static string CalculatePictureAutoStyleName(int index) {
			return String.Format("Gr{0}", index);
		}
		public static string CalculateFloatingObjectAutoStyleName(int index) {
			return String.Format("fr{0}", index);
		}
		public static string CalculatePictureFrameName(string imageID) {
			return "Gr" + imageID;
		}
		public static string CalculateFloatingObjectFrameName(string name, string imageID) {
			return name + imageID;
		}
		public static string CalculateNumberingListReferenceName(Paragraph paragraph) {
			if (!paragraph.IsInList())
				return String.Empty;
			NumberingListIndex numListIndex = paragraph.GetNumberingListIndex();
			int index = ((IConvertToInt<NumberingListIndex>)numListIndex).ToInt();
			return CalculateNumberingListStyleName(index);
		}
		public static string GetTextRunStyleName(CharacterStyleInfoTable characterAutoStyles, TextRun run) {
			string autoStyleName = characterAutoStyles.GetStyleName(run.CharacterProperties, run.CharacterStyleIndex);
			if (String.IsNullOrEmpty(autoStyleName))
				return run.CharacterStyle.StyleName;
			return autoStyleName;
		}
		public static string CalculateNumberingListId(int hashCode) {
			return String.Format("list{0}", hashCode);
		}
		public static String CalculateTableStyleName(int index) {
			return String.Format("Table{0}", index);
		}
		public static String CalculateTableRowStyleName(string tableStyleName, int index) {
			return String.Format("{0}.{1}", tableStyleName, index.ToString());
		}
		public static String CalculateTableCellStyleName(string tableRowStyleName, int index) {
			return String.Format("{0}.{1}", tableRowStyleName, index.ToString());
		}
		public static String CalculateTableColumnsStyleName(string tableStyleName, int columnIndex) {
			return String.Format("{0}.c{1}", tableStyleName, columnIndex.ToString());
		}
	}
	#endregion
}
