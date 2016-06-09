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
using System.CodeDom.Compiler;
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Selection
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Selection {
		string Text { get; set; }
		Range FormattedText { get; set; }
		int Start { get; set; }
		int End { get; set; }
		Font Font { get; set; }
		WdSelectionType Type { get; }
		WdStoryType StoryType { get; }
		object Style { get; set; }
		Tables Tables { get; }
		Words Words { get; }
		Sentences Sentences { get; }
		Characters Characters { get; }
		Footnotes Footnotes { get; }
		Endnotes Endnotes { get; }
		Comments Comments { get; }
		Cells Cells { get; }
		Sections Sections { get; }
		Paragraphs Paragraphs { get; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		Fields Fields { get; }
		FormFields FormFields { get; }
		Frames Frames { get; }
		ParagraphFormat ParagraphFormat { get; set; }
		PageSetup PageSetup { get; set; }
		Bookmarks Bookmarks { get; }
		int StoryLength { get; }
		WdLanguageID LanguageID { get; set; }
		WdLanguageID LanguageIDFarEast { get; set; }
		WdLanguageID LanguageIDOther { get; set; }
		Hyperlinks Hyperlinks { get; }
		Columns Columns { get; }
		Rows Rows { get; }
		HeaderFooter HeaderFooter { get; }
		bool IsEndOfRowMark { get; }
		int BookmarkID { get; }
		int PreviousBookmarkID { get; }
		Find Find { get; }
		Range Range { get; }
		object this[WdInformation Type] { get; }
		WdSelectionFlags Flags { get; set; }
		bool Active { get; }
		bool StartIsActive { get; set; }
		bool IPAtEndOfLine { get; }
		bool ExtendMode { get; set; }
		bool ColumnSelectMode { get; set; }
		WdTextOrientation Orientation { get; set; }
		InlineShapes InlineShapes { get; }
		Application Application { get; }
		int Creator { get; }
		object Parent { get; }
		Document Document { get; }
		ShapeRange ShapeRange { get; }
		void Select();
		void SetRange(int Start, int End);
		void Collapse(ref object Direction);
		void InsertBefore(string Text);
		void InsertAfter(string Text);
		Range Next(ref object Unit, ref object Count);
		Range Previous(ref object Unit, ref object Count);
		int StartOf(ref object Unit, ref object Extend);
		int EndOf(ref object Unit, ref object Extend);
		int Move(ref object Unit, ref object Count);
		int MoveStart(ref object Unit, ref object Count);
		int MoveEnd(ref object Unit, ref object Count);
		int MoveWhile(ref object Cset, ref object Count);
		int MoveStartWhile(ref object Cset, ref object Count);
		int MoveEndWhile(ref object Cset, ref object Count);
		int MoveUntil(ref object Cset, ref object Count);
		int MoveStartUntil(ref object Cset, ref object Count);
		int MoveEndUntil(ref object Cset, ref object Count);
		void Cut();
		void Copy();
		void Paste();
		void InsertBreak(ref object Type);
		void InsertFile(string FileName, ref object Range, ref object ConfirmConversions, ref object Link, ref object Attachment);
		bool InStory(Range Range);
		bool InRange(Range Range);
		int Delete(ref object Unit, ref object Count);
		int Expand(ref object Unit);
		void InsertParagraph();
		void InsertParagraphAfter();
		Table ConvertToTableOld(ref object Separator, ref object NumRows, ref object NumColumns, ref object InitialColumnWidth, ref object Format, ref object ApplyBorders, ref object ApplyShading, ref object ApplyFont, ref object ApplyColor, ref object ApplyHeadingRows, ref object ApplyLastRow, ref object ApplyFirstColumn, ref object ApplyLastColumn, ref object AutoFit);
		void InsertDateTimeOld(ref object DateTimeFormat, ref object InsertAsField, ref object InsertAsFullWidth);
		void InsertSymbol(int CharacterNumber, ref object Font, ref object Unicode, ref object Bias);
		void InsertCrossReference_2002(ref object ReferenceType, WdReferenceKind ReferenceKind, ref object ReferenceItem, ref object InsertAsHyperlink, ref object IncludePosition);
		void InsertCaptionXP(ref object Label, ref object Title, ref object TitleAutoText, ref object Position);
		void CopyAsPicture();
		void SortOld(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object SortColumn, ref object Separator, ref object CaseSensitive, ref object LanguageID);
		void SortAscending();
		void SortDescending();
		bool IsEqual(Range Range);
		float Calculate();
		Range GoTo(ref object What, ref object Which, ref object Count, ref object Name);
		Range GoToNext(WdGoToItem What);
		Range GoToPrevious(WdGoToItem What);
		void PasteSpecial(ref object IconIndex, ref object Link, ref object Placement, ref object DisplayAsIcon, ref object DataType, ref object IconFileName, ref object IconLabel);
		Field PreviousField();
		Field NextField();
		void InsertParagraphBefore();
		void InsertCells(ref object ShiftCells);
		void Extend(ref object Character);
		void Shrink();
		int MoveLeft(ref object Unit, ref object Count, ref object Extend);
		int MoveRight(ref object Unit, ref object Count, ref object Extend);
		int MoveUp(ref object Unit, ref object Count, ref object Extend);
		int MoveDown(ref object Unit, ref object Count, ref object Extend);
		int HomeKey(ref object Unit, ref object Extend);
		int EndKey(ref object Unit, ref object Extend);
		void EscapeKey();
		void TypeText(string Text);
		void CopyFormat();
		void PasteFormat();
		void TypeParagraph();
		void TypeBackspace();
		void NextSubdocument();
		void PreviousSubdocument();
		void SelectColumn();
		void SelectCurrentFont();
		void SelectCurrentAlignment();
		void SelectCurrentSpacing();
		void SelectCurrentIndent();
		void SelectCurrentTabs();
		void SelectCurrentColor();
		void CreateTextbox();
		void WholeStory();
		void SelectRow();
		void SplitTable();
		void InsertRows(ref object NumRows);
		void InsertColumns();
		void InsertFormula(ref object Formula, ref object NumberFormat);
		Revision NextRevision(ref object Wrap);
		Revision PreviousRevision(ref object Wrap);
		void PasteAsNestedTable();
		void DetectLanguage();
		void SelectCell();
		void InsertRowsBelow(ref object NumRows);
		void InsertColumnsRight();
		void InsertRowsAbove(ref object NumRows);
		void RtlRun();
		void LtrRun();
		void BoldRun();
		void ItalicRun();
		void RtlPara();
		void LtrPara();
		void InsertDateTime(ref object DateTimeFormat, ref object InsertAsField, ref object InsertAsFullWidth, ref object DateLanguage, ref object CalendarType);
		void Sort2000(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object SortColumn, ref object Separator, ref object CaseSensitive, ref object BidiSort, ref object IgnoreThe, ref object IgnoreKashida, ref object IgnoreDiacritics, ref object IgnoreHe, ref object LanguageID);
		Table ConvertToTable(ref object Separator, ref object NumRows, ref object NumColumns, ref object InitialColumnWidth, ref object Format, ref object ApplyBorders, ref object ApplyShading, ref object ApplyFont, ref object ApplyColor, ref object ApplyHeadingRows, ref object ApplyLastRow, ref object ApplyFirstColumn, ref object ApplyLastColumn, ref object AutoFit, ref object AutoFitBehavior, ref object DefaultTableBehavior);
		int NoProofing { get; set; }
		Tables TopLevelTables { get; }
		bool LanguageDetected { get; set; }
		float FitTextWidth { get; set; }
		void ClearFormatting();
		void PasteAppendTable();
		SmartTags SmartTags { get; }
		ShapeRange ChildShapeRange { get; }
		bool HasChildShapeRange { get; }
		FootnoteOptions FootnoteOptions { get; }
		EndnoteOptions EndnoteOptions { get; }
		void ToggleCharacterCode();
		void PasteAndFormat(WdRecoveryType Type);
		void PasteExcelTable(bool LinkedToExcel, bool WordFormatting, bool RTF);
		void ShrinkDiscontiguousSelection();
		void InsertStyleSeparator();
		void Sort(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object SortColumn, ref object Separator, ref object CaseSensitive, ref object BidiSort, ref object IgnoreThe, ref object IgnoreKashida, ref object IgnoreDiacritics, ref object IgnoreHe, ref object LanguageID, ref object SubFieldNumber, ref object SubFieldNumber2, ref object SubFieldNumber3);
		XMLNodes XMLNodes { get; }
		XMLNode XMLParentNode { get; }
		string this[bool DataOnly] { get; }
		object EnhMetaFileBits { get; }
		Range GoToEditableRange(ref object EditorID);
		void InsertXML(string XML, ref object Transform);
		void InsertCaption(ref object Label, ref object Title, ref object TitleAutoText, ref object Position, ref object ExcludeLabel);
		void InsertCrossReference(ref object ReferenceType, WdReferenceKind ReferenceKind, ref object ReferenceItem, ref object InsertAsHyperlink, ref object IncludePosition, ref object SeparateNumbers, ref object SeparatorString);
		string WordOpenXML { get; }
		void ClearParagraphStyle();
		void ClearCharacterAllFormatting();
		void ClearCharacterStyle();
		void ClearCharacterDirectFormatting();
		ContentControls ContentControls { get; }
		ContentControl ParentContentControl { get; }
		void ExportAsFixedFormat(string OutputFileName, WdExportFormat ExportFormat, bool OpenAfterExport, WdExportOptimizeFor OptimizeFor, bool ExportCurrentPage, WdExportItem Item, bool IncludeDocProps, bool KeepIRM, WdExportCreateBookmarks CreateBookmarks, bool DocStructureTags, bool BitmapMissingFonts, bool UseISO19005_1, ref object FixedFormatExtClassPtr);
		void ReadingModeGrowFont();
		void ReadingModeShrinkFont();
		void ClearParagraphAllFormatting();
		void ClearParagraphDirectFormatting();
		void InsertNewPage();
	}
	#endregion
	#region WdSelectionType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSelectionType {
		wdNoSelection,
		wdSelectionIP,
		wdSelectionNormal,
		wdSelectionFrame,
		wdSelectionColumn,
		wdSelectionRow,
		wdSelectionBlock,
		wdSelectionInlineShape,
		wdSelectionShape
	}
	#endregion
	#region WdSelectionFlags
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSelectionFlags {
		wdSelActive = 8,
		wdSelAtEOL = 2,
		wdSelOvertype = 4,
		wdSelReplace = 0x10,
		wdSelStartActive = 1
	}
	#endregion
}
