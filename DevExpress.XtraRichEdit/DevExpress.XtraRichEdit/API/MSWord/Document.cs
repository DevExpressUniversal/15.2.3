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
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Document : DocumentInner, DocumentEvents2_Event {
	}
	[GeneratedCode("Suppress FxCop check", "")]
	public interface DocumentInner : IWordObject {
		string Name { get; }
		object BuiltInDocumentProperties { get; }
		object CustomDocumentProperties { get; }
		string Path { get; }
		Bookmarks Bookmarks { get; }
		Tables Tables { get; }
		Footnotes Footnotes { get; }
		Endnotes Endnotes { get; }
		Comments Comments { get; }
		WdDocumentType Type { get; }
		bool AutoHyphenation { get; set; }
		bool HyphenateCaps { get; set; }
		int HyphenationZone { get; set; }
		int ConsecutiveHyphensLimit { get; set; }
		Sections Sections { get; }
		Paragraphs Paragraphs { get; }
		Words Words { get; }
		Sentences Sentences { get; }
		DevExpress.XtraRichEdit.API.Word.Characters Characters { get; }
		Fields Fields { get; }
		FormFields FormFields { get; }
		Styles Styles { get; }
		Frames Frames { get; }
		string FullName { get; }
		Revisions Revisions { get; }
		PageSetup PageSetup { get; set; }
		bool HasRoutingSlip { get; set; }
		bool Routed { get; }
		bool Saved { get; set; }
		Range Content { get; }
		WdDocumentKind Kind { get; set; }
		bool ReadOnly { get; }
		Subdocuments Subdocuments { get; }
		bool IsMasterDocument { get; }
		float DefaultTabStop { set; }
		bool EmbedTrueTypeFonts { set; }
		bool SaveFormsData { get; set; }
		bool ReadOnlyRecommended { get; set; }
		bool SaveSubsetFonts { get; set; }
		bool this[WdCompatibility Type] { get; set; }
		bool IsSubdocument { get; }
		int SaveFormat { get; }
		WdProtectionType ProtectionType { get; }
		Hyperlinks Hyperlinks { get; }
		Shapes Shapes { get; }
		ListTemplates ListTemplates { get; }
		Lists Lists { get; }
		bool UpdateStylesOnOpen { get; set; }
		object AttachedTemplate { get; set; }
		InlineShapes InlineShapes { get; }
		Shape Background { get; set; }
		bool GrammarChecked { get; set; }
		bool SpellingChecked { get; set; }
		bool ShowGrammaticalErrors { get; set; }
		bool ShowSpellingErrors { get; set; }
		bool ShowSummary { get; set; }
		WdSummaryMode SummaryViewMode { get; set; }
		int SummaryLength { get; set; }
		bool PrintFractionalWidths { get; set; }
		bool PrintPostScriptOverText { get; set; }
		object Container { get; }
		bool PrintFormsData { get; set; }
		ListParagraphs ListParagraphs { get; }
		string Password { set; }
		string WritePassword { set; }
		bool HasPassword { get; }
		bool WriteReserved { get; }
		string this[object LanguageID] { get; set; } 
		bool UserControl { get; set; }
		bool HasMailer { get; set; }
		ProofreadingErrors GrammaticalErrors { get; }
		ProofreadingErrors SpellingErrors { get; }
		bool FormsDesign { get; }
		string CodeName { get; }
		bool SnapToGrid { get; set; }
		bool SnapToShapes { get; set; }
		float GridDistanceHorizontal { get; set; }
		float GridDistanceVertical { get; set; }
		float GridOriginHorizontal { get; set; }
		float GridOriginVertical { get; set; }
		int GridSpaceBetweenHorizontalLines { get; set; }
		int GridSpaceBetweenVerticalLines { get; set; }
		bool GridOriginFromMargin { get; set; }
		bool KerningByAlgorithm { get; set; }
		WdJustificationMode JustificationMode { get; set; }
		WdFarEastLineBreakLevel FarEastLineBreakLevel { get; set; }
		string NoLineBreakBefore { get; set; }
		string NoLineBreakAfter { get; set; }
		bool TrackRevisions { get; set; }
		bool PrintRevisions { get; set; }
		bool ShowRevisions { get; set; }
		void Close(ref object SaveChanges, ref object OriginalFormat, ref object RouteDocument);
		void SaveAs2000(ref object FileName, ref object FileFormat, ref object LockComments, ref object Password, ref object AddToRecentFiles, ref object WritePassword, ref object ReadOnlyRecommended, ref object EmbedTrueTypeFonts, ref object SaveNativePictureFormat, ref object SaveFormsData, ref object SaveAsAOCELetter);
		void Repaginate();
		void FitToPages();
		void ManualHyphenation();
		void Select();
		void DataForm();
		void Route();
		void Save();
		void PrintOutOld(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object ActivePrinterMacGX, ref object ManualDuplexPrint);
		void SendMail();
		Range Range(ref object Start, ref object End);
		void RunAutoMacro(WdAutoMacros Which);
		void Activate();
		void PrintPreview();
		Range GoTo(ref object What, ref object Which, ref object Count, ref object Name);
		bool Undo(ref object Times);
		bool Redo(ref object Times);
		int ComputeStatistics(WdStatistic Statistic, ref object IncludeFootnotesAndEndnotes);
		void MakeCompatibilityDefault();
		void Protect2002(WdProtectionType Type, ref object NoReset, ref object Password);
		void Unprotect(ref object Password);
		void EditionOptions(WdEditionType Type, WdEditionOption Option, string Name, ref object Format);
		void RunLetterWizard(ref object LetterContent, ref object WizardMode);
		void CopyStylesFromTemplate(string Template);
		void UpdateStyles();
		void CheckGrammar();
		void CheckSpelling(ref object CustomDictionary, ref object IgnoreUppercase, ref object AlwaysSuggest, ref object CustomDictionary2, ref object CustomDictionary3, ref object CustomDictionary4, ref object CustomDictionary5, ref object CustomDictionary6, ref object CustomDictionary7, ref object CustomDictionary8, ref object CustomDictionary9, ref object CustomDictionary10);
		void FollowHyperlink(ref object Address, ref object SubAddress, ref object NewWindow, ref object AddHistory, ref object ExtraInfo, ref object Method, ref object HeaderInfo);
		void AddToFavorites();
		void Reload();
		Range AutoSummarize(ref object Length, ref object Mode, ref object UpdateProperties);
		void RemoveNumbers(ref object NumberType);
		void ConvertNumbersToText(ref object NumberType);
		int CountNumberedItems(ref object NumberType, ref object Level);
		void Post();
		void ToggleFormsDesign();
		void Compare2000(string Name);
		void UpdateSummaryProperties();
		object GetCrossReferenceItems(ref object ReferenceType);
		void AutoFormat();
		void ViewCode();
		void ViewPropertyBrowser();
		void ForwardMailer();
		void Reply();
		void ReplyAll();
		void SendMailer(ref object FileFormat, ref object Priority);
		void UndoClear();
		void PresentIt();
		void SendFax(string Address, ref object Subject);
		void Merge2000(string FileName);
		void ClosePrintPreview();
		void CheckConsistency();
		void AcceptAllRevisions();
		void RejectAllRevisions();
		void DetectLanguage();
		void ApplyTheme(string Name);
		void RemoveTheme();
		void WebPagePreview();
		void ReloadAs(MsoEncoding Encoding);
		string ActiveTheme { get; }
		string ActiveThemeDisplayName { get; }
		Scripts Scripts { get; }
		bool LanguageDetected { get; set; }
		WdFarEastLineBreakLanguageID FarEastLineBreakLanguage { get; set; }
		Frameset Frameset { get; }
		object ClickAndTypeParagraphStyle { get; set; }
		MsoEncoding OpenEncoding { get; }
		MsoEncoding SaveEncoding { get; set; }
		bool OptimizeForWord97 { get; set; }
		bool VBASigned { get; }
		void PrintOut2000(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object ActivePrinterMacGX, ref object ManualDuplexPrint, ref object PrintZoomColumn, ref object PrintZoomRow, ref object PrintZoomPaperWidth, ref object PrintZoomPaperHeight);
		void sblt(string s);
		void ConvertVietDoc(int CodePageOrigin);
		void PrintOut(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object ActivePrinterMacGX, ref object ManualDuplexPrint, ref object PrintZoomColumn, ref object PrintZoomRow, ref object PrintZoomPaperWidth, ref object PrintZoomPaperHeight);
		bool DisableFeatures { get; set; }
		bool DoNotEmbedSystemFonts { get; set; }
		string DefaultTargetFrame { get; set; }
		WdDisableFeaturesIntroducedAfter DisableFeaturesIntroducedAfter { get; set; }
		bool RemovePersonalInformation { get; set; }
		SmartTags SmartTags { get; }
		void Compare2002(string Name, ref object AuthorName, ref object CompareTarget, ref object DetectFormatChanges, ref object IgnoreAllComparisonWarnings, ref object AddToRecentFiles);
		void CheckIn(bool SaveChanges, ref object Comments, bool MakePublic);
		bool CanCheckin();
		void Merge(string FileName, ref object MergeTarget, ref object DetectFormatChanges, ref object UseFormattingFrom, ref object AddToRecentFiles);
		bool EmbedSmartTags { get; set; }
		bool SmartTagsAsXMLProps { get; set; }
		MsoEncoding TextEncoding { get; set; }
		WdLineEndingType TextLineEnding { get; set; }
		void SendForReview(ref object Recipients, ref object Subject, ref object ShowMessage, ref object IncludeAttachment);
		void ReplyWithChanges(ref object ShowMessage);
		void EndReview();
		object DefaultTableStyle { get; }
		string PasswordEncryptionProvider { get; }
		string PasswordEncryptionAlgorithm { get; }
		int PasswordEncryptionKeyLength { get; }
		bool PasswordEncryptionFileProperties { get; }
		void SetPasswordEncryptionOptions(string PasswordEncryptionProvider, string PasswordEncryptionAlgorithm, int PasswordEncryptionKeyLength, ref object PasswordEncryptionFileProperties);
		void RecheckSmartTags();
		void RemoveSmartTags();
		void SetDefaultTableStyle(ref object Style, bool SetInTemplate);
		void DeleteAllComments();
		void AcceptAllRevisionsShown();
		void RejectAllRevisionsShown();
		void DeleteAllCommentsShown();
		void ResetFormFields();
		void SaveAs(ref object FileName, ref object FileFormat, ref object LockComments, ref object Password, ref object AddToRecentFiles, ref object WritePassword, ref object ReadOnlyRecommended, ref object EmbedTrueTypeFonts, ref object SaveNativePictureFormat, ref object SaveFormsData, ref object SaveAsAOCELetter, ref object Encoding, ref object InsertLineBreaks, ref object AllowSubstitutions, ref object LineEnding, ref object AddBiDiMarks);
		bool EmbedLinguisticData { get; set; }
		bool FormattingShowFont { get; set; }
		bool FormattingShowClear { get; set; }
		bool FormattingShowParagraph { get; set; }
		bool FormattingShowNumbering { get; set; }
		WdShowFilter FormattingShowFilter { get; set; }
		void CheckNewSmartTags();
		XMLNodes XMLNodes { get; }
		bool EnforceStyle { get; set; }
		bool AutoFormatOverride { get; set; }
		bool XMLSaveDataOnly { get; set; }
		bool XMLHideNamespaces { get; set; }
		bool XMLShowAdvancedErrors { get; set; }
		bool XMLUseXSLTWhenSaving { get; set; }
		string XMLSaveThroughXSLT { get; set; }
		bool ReadingModeLayoutFrozen { get; set; }
		bool RemoveDateAndTime { get; set; }
		void SendFaxOverInternet(ref object Recipients, ref object Subject, ref object ShowMessage);
		void TransformDocument(string Path, bool DataOnly);
		void Protect(WdProtectionType Type, ref object NoReset, ref object Password, ref object UseIRM, ref object EnforceStyleLock);
		void SelectAllEditableRanges(ref object EditorID);
		void DeleteAllEditableRanges(ref object EditorID);
		void DeleteAllInkAnnotations();
		void AddDocumentWorkspaceHeader(bool RichFormat, string Url, string Title, string Description, string ID);
		void RemoveDocumentWorkspaceHeader(string ID);
		void Compare(string Name, ref object AuthorName, ref object CompareTarget, ref object DetectFormatChanges, ref object IgnoreAllComparisonWarnings, ref object AddToRecentFiles, ref object RemovePersonalInformation, ref object RemoveDateAndTime);
		void RemoveLockedStyles();
		XMLChildNodeSuggestions ChildNodeSuggestions { get; }
		XMLNode SelectSingleNode(string XPath, string PrefixMapping, bool FastSearchSkippingTextNodes);
		XMLNodes SelectNodes(string XPath, string PrefixMapping, bool FastSearchSkippingTextNodes);
		XMLNodes XMLSchemaViolations { get; }
		int ReadingLayoutSizeX { get; set; }
		int ReadingLayoutSizeY { get; set; }
		WdStyleSort StyleSortMethod { get; set; }
		bool TrackMoves { get; set; }
		bool TrackFormatting { get; set; }
		void Dummy1();
		void RemoveDocumentInformation(WdRemoveDocInfoType RemoveDocInfoType);
		void CheckInWithVersion(bool SaveChanges, ref object Comments, bool MakePublic, ref object VersionType);
		void Dummy2();
		void Dummy3();
		ContentControls ContentControls { get; }
		void LockServerFile();
		void Dummy4();
		void AddMeetingWorkspaceHeader(bool SkipIfAbsent, string Url, string Title, string Description, string ID);
		bool LockTheme { get; set; }
		bool LockQuickStyleSet { get; set; }
		string OriginalDocumentTitle { get; }
		string RevisedDocumentTitle { get; }
		bool FormattingShowNextLevel { get; set; }
		bool FormattingShowUserStyleName { get; set; }
		void SaveAsQuickStyleSet(string FileName);
		void ApplyQuickStyleSet(string Name);
		bool Final { get; set; }
		WdOMathBreakBin OMathBreakBin { get; set; }
		WdOMathBreakSub OMathBreakSub { get; set; }
		WdOMathJc OMathJc { get; set; }
		float OMathLeftMargin { get; set; }
		float OMathRightMargin { get; set; }
		float OMathWrap { get; set; }
		bool OMathIntSubSupLim { get; set; }
		bool OMathNarySupSubLim { get; set; }
		bool OMathSmallFrac { get; set; }
		string WordOpenXML { get; }
		void ApplyDocumentTheme(string FileName);
		bool HasVBProject { get; }
		ContentControls SelectContentControlsByTitle(string Title);
		void ExportAsFixedFormat(string OutputFileName, WdExportFormat ExportFormat, bool OpenAfterExport, WdExportOptimizeFor OptimizeFor, WdExportRange Range, int From, int To, WdExportItem Item, bool IncludeDocProps, bool KeepIRM, WdExportCreateBookmarks CreateBookmarks, bool DocStructureTags, bool BitmapMissingFonts, bool UseISO19005_1, ref object FixedFormatExtClassPtr);
		void FreezeLayout();
		void UnfreezeLayout();
		string OMathFontName { get; set; }
		void DowngradeDocument();
		string EncryptionProvider { get; set; }
		bool UseMathDefaults { get; set; }
		int CurrentRsid { get; }
		void Convert();
		ContentControls SelectContentControlsByTag(string Tag);
	}
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_BuildingBlockInsertEventHandler(Range Range, string Name, string Category, string BlockType, string Template);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_CloseEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlAfterAddEventHandler(ContentControl NewContentControl, bool InUndoRedo);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlBeforeContentUpdateEventHandler(ContentControl ContentControl, ref string Content);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlBeforeDeleteEventHandler(ContentControl OldContentControl, bool InUndoRedo);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlBeforeStoreUpdateEventHandler(ContentControl ContentControl, ref string Content);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlOnEnterEventHandler(ContentControl ContentControl);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_ContentControlOnExitEventHandler(ContentControl ContentControl, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_NewEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_OpenEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_SyncEventHandler(MsoSyncEventType SyncEventType);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_XMLAfterInsertEventHandler(XMLNode NewXMLNode, bool InUndoRedo);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void DocumentEvents2_XMLBeforeDeleteEventHandler(Range DeletedRange, XMLNode OldXMLNode, bool InUndoRedo);
	[GeneratedCode("Suppress FxCop check", "")]
	public interface DocumentEvents2_Event {
		event DocumentEvents2_BuildingBlockInsertEventHandler BuildingBlockInsert;
		event DocumentEvents2_CloseEventHandler Close;
		event DocumentEvents2_ContentControlAfterAddEventHandler ContentControlAfterAdd;
		event DocumentEvents2_ContentControlBeforeContentUpdateEventHandler ContentControlBeforeContentUpdate;
		event DocumentEvents2_ContentControlBeforeDeleteEventHandler ContentControlBeforeDelete;
		event DocumentEvents2_ContentControlBeforeStoreUpdateEventHandler ContentControlBeforeStoreUpdate;
		event DocumentEvents2_ContentControlOnEnterEventHandler ContentControlOnEnter;
		event DocumentEvents2_ContentControlOnExitEventHandler ContentControlOnExit;
		event DocumentEvents2_NewEventHandler New;
		event DocumentEvents2_OpenEventHandler Open;
		event DocumentEvents2_SyncEventHandler Sync;
		event DocumentEvents2_XMLAfterInsertEventHandler XMLAfterInsert;
		event DocumentEvents2_XMLBeforeDeleteEventHandler XMLBeforeDelete;
	}
	#region WdLineEndingType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLineEndingType {
		wdCRLF,
		wdCROnly,
		wdLFOnly,
		wdLFCR,
		wdLSPS
	}
	#endregion
	#region WdProtectionType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdProtectionType {
		wdAllowOnlyComments = 1,
		wdAllowOnlyFormFields = 2,
		wdAllowOnlyReading = 3,
		wdAllowOnlyRevisions = 0,
		wdNoProtection = -1
	}
	#endregion
	#region WdShowFilter
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdShowFilter {
		wdShowFilterStylesAvailable,
		wdShowFilterStylesInUse,
		wdShowFilterStylesAll,
		wdShowFilterFormattingInUse,
		wdShowFilterFormattingAvailable,
		wdShowFilterFormattingRecommended
	}
	#endregion
	#region WdDisableFeaturesIntroducedAfter
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdDisableFeaturesIntroducedAfter {
		wd70,
		wd70FE,
		wd80
	}
	#endregion
	#region WdFarEastLineBreakLevel
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFarEastLineBreakLevel {
		wdFarEastLineBreakLevelNormal,
		wdFarEastLineBreakLevelStrict,
		wdFarEastLineBreakLevelCustom
	}
	#endregion
	#region WdFarEastLineBreakLanguageID
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFarEastLineBreakLanguageID {
		wdLineBreakJapanese = 0x411,
		wdLineBreakKorean = 0x412,
		wdLineBreakSimplifiedChinese = 0x804,
		wdLineBreakTraditionalChinese = 0x404
	}
	#endregion
	#region WdLetterStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLetterStyle {
		wdFullBlock,
		wdModifiedBlock,
		wdSemiBlock
	}
	#endregion
	#region WdLetterheadLocation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLetterheadLocation {
		wdLetterTop,
		wdLetterBottom,
		wdLetterLeft,
		wdLetterRight
	}
	#endregion
	#region WdSalutationType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSalutationType {
		wdSalutationInformal,
		wdSalutationFormal,
		wdSalutationBusiness,
		wdSalutationOther
	}
	#endregion
	#region WdJustificationMode
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdJustificationMode {
		wdJustificationModeExpand,
		wdJustificationModeCompress,
		wdJustificationModeCompressKana
	}
	#endregion
	#region WdAutoMacros
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdAutoMacros {
		wdAutoExec,
		wdAutoNew,
		wdAutoOpen,
		wdAutoClose,
		wdAutoExit,
		wdAutoSync
	}
	#endregion
	#region WdStatistic
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdStatistic {
		wdStatisticWords,
		wdStatisticLines,
		wdStatisticPages,
		wdStatisticCharacters,
		wdStatisticParagraphs,
		wdStatisticCharactersWithSpaces,
		wdStatisticFarEastCharacters
	}
	#endregion
	#region WdEditionOption
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdEditionOption {
		wdCancelPublisher,
		wdSendPublisher,
		wdSelectPublisher,
		wdAutomaticUpdate,
		wdManualUpdate,
		wdChangeAttributes,
		wdUpdateSubscriber,
		wdOpenSource
	}
	#endregion
	#region WdEditionType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdEditionType {
		wdPublisher,
		wdSubscriber
	}
	#endregion
	#region WdDocumentKind
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdDocumentKind {
		wdDocumentNotSpecified,
		wdDocumentLetter,
		wdDocumentEmail
	}
	#endregion
	#region WdCompatibility
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCompatibility {
		wdAlignTablesRowByRow = 0x27,
		wdAllowSpaceOfSameStyleInTable = 0x36,
		wdApplyBreakingRules = 0x2e,
		wdAutofitLikeWW11 = 0x39,
		wdAutospaceLikeWW7 = 0x26,
		wdCachedColBalance = 0x41,
		wdConvMailMergeEsc = 6,
		wdDontAdjustLineHeightInTable = 0x24,
		wdDontAutofitConstrainedTables = 0x38,
		wdDontBalanceSingleByteDoubleByteWidth = 0x10,
		wdDontBreakConstrainedForcedTables = 0x3e,
		wdDontBreakWrappedTables = 0x2b,
		wdDontSnapTextToGridInTableWithObjects = 0x2c,
		wdDontULTrailSpace = 15,
		wdDontUseAsianBreakRulesInGrid = 0x30,
		wdDontUseHTMLParagraphAutoSpacing = 0x23,
		wdDontUseIndentAsNumberingTabStop = 0x34,
		wdDontVertAlignCellWithShape = 0x3d,
		wdDontVertAlignInTextbox = 0x3f,
		wdDontWrapTextWithPunctuation = 0x2f,
		wdExactOnTop = 0x1c,
		wdExpandShiftReturn = 14,
		wdFELineBreak11 = 0x35,
		wdFootnoteLayoutLikeWW8 = 0x22,
		wdForgetLastTabAlignment = 0x25,
		wdGrowAutofit = 50,
		wdHangulWidthLikeWW11 = 0x3b,
		wdLayoutRawTableWidth = 40,
		wdLayoutTableRowsApart = 0x29,
		wdLeaveBackslashAlone = 13,
		wdLineWrapLikeWord6 = 0x20,
		wdMWSmallCaps = 0x16,
		wdNoColumnBalance = 5,
		wdNoExtraLineSpacing = 0x17,
		wdNoLeading = 20,
		wdNoSpaceForUL = 0x15,
		wdNoSpaceRaiseLower = 2,
		wdNoTabHangIndent = 1,
		wdOrigWordTableRules = 9,
		wdPrintBodyTextBeforeHeader = 0x13,
		wdPrintColBlack = 3,
		wdSelectFieldWithFirstOrLastCharacter = 0x2d,
		wdShapeLayoutLikeWW8 = 0x21,
		wdShowBreaksInFrames = 11,
		wdSpacingInWholePoints = 0x12,
		wdSplitPgBreakAndParaMark = 60,
		wdSubFontBySize = 0x19,
		wdSuppressBottomSpacing = 0x1d,
		wdSuppressSpBfAfterPgBrk = 7,
		wdSuppressTopSpacing = 8,
		wdSuppressTopSpacingMac5 = 0x11,
		wdSwapBordersFacingPages = 12,
		wdTransparentMetafiles = 10,
		wdTruncateFontHeight = 0x18,
		wdUnderlineTabInNumList = 0x3a,
		wdUseNormalStyleForList = 0x33,
		wdUsePrinterMetrics = 0x1a,
		wdUseWord2002TableStyleRules = 0x31,
		wdUseWord97LineBreakingRules = 0x2a,
		wdWord11KerningPairs = 0x40,
		wdWPJustification = 0x1f,
		wdWPSpaceWidth = 30,
		wdWrapTrailSpaces = 4,
		wdWW11IndentRules = 0x37,
		wdWW6BorderRules = 0x1b
	}
	#endregion
	#region WdDocumentType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdDocumentType {
		wdTypeDocument,
		wdTypeTemplate,
		wdTypeFrameset
	}
	#endregion
	#region WdSummaryMode
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSummaryMode {
		wdSummaryModeHighlight,
		wdSummaryModeHideAllButSummary,
		wdSummaryModeInsert,
		wdSummaryModeCreateNew
	}
	#endregion
	#region WdStyleSort
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdStyleSort {
		wdStyleSortByName,
		wdStyleSortRecommended,
		wdStyleSortByFont,
		wdStyleSortByBasedOn,
		wdStyleSortByType
	}
	#endregion
	#region WdRemoveDocInfoType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRemoveDocInfoType {
		wdRDIAll = 0x63,
		wdRDIComments = 1,
		wdRDIContentType = 0x10,
		wdRDIDocumentManagementPolicy = 15,
		wdRDIDocumentProperties = 8,
		wdRDIDocumentServerProperties = 14,
		wdRDIDocumentWorkspace = 10,
		wdRDIEmailHeader = 5,
		wdRDIInkAnnotations = 11,
		wdRDIRemovePersonalInformation = 4,
		wdRDIRevisions = 2,
		wdRDIRoutingSlip = 6,
		wdRDISendForReview = 7,
		wdRDITemplate = 9,
		wdRDIVersions = 3
	}
	#endregion
	#region WdOMathBreakBin
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOMathBreakBin {
		wdOMathBreakBinBefore,
		wdOMathBreakBinAfter,
		wdOMathBreakBinRepeat
	}
	#endregion
	#region WdOMathBreakSub
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOMathBreakSub {
		wdOMathBreakSubMinusMinus,
		wdOMathBreakSubPlusMinus,
		wdOMathBreakSubMinusPlus
	}
	#endregion
	#region WdOMathJc
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOMathJc {
		wdOMathJcCenter = 2,
		wdOMathJcCenterGroup = 1,
		wdOMathJcInline = 7,
		wdOMathJcLeft = 3,
		wdOMathJcRight = 4
	}
	#endregion
}
