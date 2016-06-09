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
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Utils.Controls;
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Office.History;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using MergeFieldName = DevExpress.XtraRichEdit.API.Native.MergeFieldName;
using DevExpress.Compatibility.System.ComponentModel.Design;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Reflection;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentModel
	public class DocumentModel : DocumentModelBase<DocumentFormat>, ICharacterPropertiesContainer, IParagraphPropertiesContainer, ICellPropertiesOwner, IDocumentModelStructureChangedListener, IServiceContainer {
		#region Fields
		internal static readonly UnderlineRepository DefaultUnderlineRepository = new UnderlineRepository();
		internal static readonly StrikeoutRepository DefaultStrikeoutRepository = new StrikeoutRepository();
		internal int countIdForPeaceTables;
		TopBorder emptyTopBorder;
		BottomBorder emptyBottomBorder;
		ParagraphProperties defaultParagraphProperties;
		CharacterProperties defaultCharacterProperties;
		UnderlineRepository underlineRepository;
		BorderLineRepository borderLineRepository;
		StrikeoutRepository strikeoutRepository;
		ParagraphStyleCollection paragraphStyles;
		CharacterStyleCollection characterStyles;
		StyleLinkManager styleLinkManager;
		TableStyleCollection tableStyles;
		TableCellStyleCollection tableCellStyles;
		NumberingListStyleCollection numberingListStyles;
		TableProperties defaultTableProperties;
		TableRowProperties defaultTableRowProperties;
		TableCellProperties defaultTableCellProperties;
		string numberDecimalSeparator;
		string numberGroupSeparator;
		PieceTable pieceTable;
		PieceTable activePieceTable;
		SectionIndex activeSectionIndex;
		LineNumberCommonRun lineNumberRun;
		SectionCollection sections;
		HeaderCollection headers;
		FooterCollection footers;
		LastInsertedRunInfo lastInsertedRunInfo;
		LastInsertedInlinePictureRunInfo lastInsertedInlinePictureRunInfo;
		LastInsertedFloatingObjectAnchorRunInfo lastInsertedFloatingObjectAnchorRunInfo;
		LastInsertedSeparatorRunInfo lastInsertedSeparatorRunInfo;
		AbstractNumberingListIdProvider abstractNumberingListIdProvider;
		NumberingListIdProvider numberingListIdProvider;
		NumberingListCollection numberingLists;
		AbstractNumberingListCollection abstractNumberingLists;
		Selection selection;
		EditingOptions editingOptions;
		DocumentSaveOptions documentSaveOptions;
		DocumentCapabilitiesOptions documentCapabilities;
		RichEditLayoutOptions layoutOptions;
		FieldOptions fieldOptions;
		RichEditMailMergeOptions mailMergeOptions;
		RichEditDocumentImportOptions documentImportOptions;
		RichEditDocumentExportOptions documentExportOptions;
		BookmarkOptions bookmarkOptions;
		RangePermissionOptions rangePermissionOptions;
		CommentOptions commentOptions;
		DevExpress.XtraRichEdit.DocumentSearchOptions searchOptions;
		CopyPasteOptions copyPasteOptions;
		SpellCheckerOptions spellCheckerOptions;
		FormattingMarkVisibilityOptions formattingMarkVisibilityOptions;
		AuthenticationOptions authenticationOptions;
		TableOptions tableOptions;
		AutoCorrectOptions autoCorrectOptions;
		RichEditBehaviorOptions behaviorOptions;
		PrintingOptions printingOptions;
		DocumentCache cache;
		DocumentProperties documentProperties;
		ExtendedDocumentProperties extendedDocumentProperties;
		SafeDocumentModelEditor safeEditor;
		UnsafeDocumentModelEditor unsafeEditor;
		InternalAPI internalAPI;
		ICommandsCreationStrategy commandsCreationStrategy;
		DocumentModelDeferredChanges deferredChanges;
		MailMergeProperties mailMergeProperties;
		DocumentProtectionProperties protectionProperties;
		SearchParameters searchParameters;
		SearchContext searchContext;
		RichEditDataControllerAdapterBase mailMergeDataController;
		BorderInfoRepository tableBorderInfoRepository;
		BorderInfoRepository floatingObjectBorderInfoRepository;
		DocumentVariableCollection variables;
		FootNoteCollection footNotes;
		EndNoteCollection endNotes;
		int syntaxHighlightSuspendCount;
		bool suppressFieldsChangeNotification;
		bool isLastSelectionInEmptySpecialParagraph;
		RunIndex specialEmptyParagraphRunIndex;
		DocumentFormatsDependencies documentFormatsDependencies;
		WebSettings htmlSettings;
		CommentColorer commentColorer;
		string commentHeading;
		int suppressPerformLayout = 0;
		#endregion
		public DocumentModel(DocumentFormatsDependencies documentFormatsDependencies)
			: this(true, true, documentFormatsDependencies) {
		}
		protected internal DocumentModel(bool addDefaultsList, bool changeDefaultTableStyle, DocumentFormatsDependencies documentFormatsDependencies, float dpiX, float dpiY)
			: base(dpiX, dpiY) {
			Guard.ArgumentNotNull(documentFormatsDependencies, "documentFormatsDependencies");
			this.countIdForPeaceTables = 0;
			this.documentFormatsDependencies = documentFormatsDependencies;
			this.mailMergeDataController = CreateMailMergeDataController();
			SubscribeMailMergeDataControllerEvents();
			AddServices();
			Initialize(addDefaultsList, changeDefaultTableStyle);
			this.commentHeading = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment);
		}
		protected internal DocumentModel(bool addDefaultsList, bool changeDefaultTableStyle, DocumentFormatsDependencies documentFormatsDependencies)
			: this(addDefaultsList, changeDefaultTableStyle, documentFormatsDependencies, DocumentModelDpi.DpiX, DocumentModelDpi.DpiY) {
		}
		protected internal DocumentModel(DocumentFormatsDependencies documentFormatsDependencies, float dpiX, float dpiY)
			: this(true, true, documentFormatsDependencies, dpiX, dpiY) {
		}
		#region IDisposable implementation
		protected override void DisposeCore() {
			base.DisposeCore();
			if (mailMergeDataController != null) {
				UnubscribeMailMergeDataControllerEvents();
				mailMergeDataController = null;
			}
			UnsubscribeEventsForExport();
		}
		#endregion
		public static implicit operator DocumentModel(DocumentModelAccessor instance) {
			if (instance == null)
				return null;
			else
				return instance.DocumentModel;
		}
		public virtual void Reinitialize() {
			ClearCore();
			Initialize(true, true);
		}
		#region Initialize
		protected internal virtual void Initialize(bool addDefaultsList, bool changeDefaultTableStyle) {
			CreateOptions();
			UpdateFontCache();
			CreateDocumentObjects();
			ClearDocumentCore(addDefaultsList, changeDefaultTableStyle);
			SubscribeDocumentObjectsEvents();
			SubscribeOptionsEvents();
			SwitchToNormalHistory(true);
			SwitchToNormalSelection();
			this.internalAPI = CreateInternalAPI();
			this.commandsCreationStrategy = CreateCommandCreationStrategy();
			this.searchParameters = new SearchParameters();
			this.searchContext = new SearchContext(this.MainPieceTable);
		}
		#endregion
		void CreateOptions() {
			this.editingOptions = CreateEditingOptions();
			this.fieldOptions = CreateFieldOptions();
			this.mailMergeOptions = CreateRichEditMailMergeOptions();
			this.documentSaveOptions = CreateDocumentSaveOptions();
			this.documentCapabilities = CreateDocumentCapabilitiesOptions();
			this.layoutOptions = CreateRichEditLayoutOptions();
			this.documentImportOptions = CreateRichEditDocumentImportOptions();
			this.documentExportOptions = CreateRichEditDocumentExportOptions();
			this.bookmarkOptions = CreateBookmarkOptions();
			this.rangePermissionOptions = CreateRangePermissionOptions();
			this.commentOptions = CreateCommentOptions();
			this.searchOptions = CreateDocumentSearchOptions();
			this.formattingMarkVisibilityOptions = CreateFormattingMarkVisibilityOptions();
			this.authenticationOptions = CreateAuthenticationOptions();
			this.tableOptions = CreateTableOptions();
			this.autoCorrectOptions = CreateAutoCorrectOptions();
			this.behaviorOptions = CreateRichEditBehaviorOptions();
			this.printingOptions = CreatePrintingOptions();
			this.copyPasteOptions = CreateCopyPasteOptions();
			this.spellCheckerOptions = CreateSpellCheckerOptions();
		}
		protected internal virtual FieldOptions CreateFieldOptions() { return new FieldOptions(); }
		protected internal virtual DocumentSaveOptions CreateDocumentSaveOptions() { return new DocumentSaveOptions(); }
		protected internal virtual EditingOptions CreateEditingOptions() { return new EditingOptions(); }
		protected internal virtual RichEditMailMergeOptions CreateRichEditMailMergeOptions() { return new RichEditMailMergeOptions(); }
		protected internal virtual DocumentCapabilitiesOptions CreateDocumentCapabilitiesOptions() { return new DocumentCapabilitiesOptions(); }
		protected internal virtual RichEditLayoutOptions CreateRichEditLayoutOptions() { return new RichEditLayoutOptions(); }
		protected internal virtual RichEditDocumentImportOptions CreateRichEditDocumentImportOptions() { return new RichEditDocumentImportOptions(); }
		protected internal virtual RichEditDocumentExportOptions CreateRichEditDocumentExportOptions() { return new RichEditDocumentExportOptions(); }
		protected internal virtual BookmarkOptions CreateBookmarkOptions() { return new BookmarkOptions(); }
		protected internal virtual RangePermissionOptions CreateRangePermissionOptions() { return new RangePermissionOptions(); }
		protected internal virtual CommentOptions CreateCommentOptions() { return new CommentOptions(); }
		protected internal virtual DevExpress.XtraRichEdit.DocumentSearchOptions CreateDocumentSearchOptions() { return new DevExpress.XtraRichEdit.DocumentSearchOptions(); }
		protected internal virtual FormattingMarkVisibilityOptions CreateFormattingMarkVisibilityOptions() { return new FormattingMarkVisibilityOptions(); }
		protected internal virtual AuthenticationOptions CreateAuthenticationOptions() { return new AuthenticationOptions(); }
		protected internal virtual TableOptions CreateTableOptions() { return new TableOptions(); }
		protected internal virtual AutoCorrectOptions CreateAutoCorrectOptions() { return new AutoCorrectOptions(); }
		protected internal virtual RichEditBehaviorOptions CreateRichEditBehaviorOptions() { return new RichEditBehaviorOptions(); }
		protected internal virtual PrintingOptions CreatePrintingOptions() { return new PrintingOptions(); }
		protected internal virtual CopyPasteOptions CreateCopyPasteOptions() { return new CopyPasteOptions(); }
		protected internal virtual SpellCheckerOptions CreateSpellCheckerOptions() { return new SpellCheckerOptions(); }
		public bool ForceNotifyStructureChanged { get; set; }
		public DocumentFormatsDependencies DocumentFormatsDependencies { get { return documentFormatsDependencies; } }
		void SubscribeOptionsEvents() {
			SubscribeDocumentCapabilitiesOptionsEvents();
			SubscribeFormattingMarkVisibilityOptions();
			SubscribeRichEditLayoutOptions();
			SubscribeTableOptions();
			SubscribeCommentOptions();
		}
		void CreateDocumentObjects() {
			this.safeEditor = new SafeDocumentModelEditor(this);
			this.unsafeEditor = new UnsafeDocumentModelEditor(this);
			this.sections = new SectionCollection();
			this.headers = new HeaderCollection();
			this.footers = new FooterCollection();
			this.lastInsertedRunInfo = new LastInsertedRunInfo();
			this.lastInsertedInlinePictureRunInfo = new LastInsertedInlinePictureRunInfo();
			this.lastInsertedFloatingObjectAnchorRunInfo = new LastInsertedFloatingObjectAnchorRunInfo();
			this.lastInsertedSeparatorRunInfo = new LastInsertedSeparatorRunInfo();
			this.pieceTable = CreateMainContentType().PieceTable;
			this.activePieceTable = pieceTable;
			this.activeSectionIndex = new SectionIndex(-1);
			this.lastInsertedRunInfo.PieceTable = MainPieceTable;
			this.lastInsertedInlinePictureRunInfo.PieceTable = MainPieceTable;
			this.lastInsertedFloatingObjectAnchorRunInfo.PieceTable = MainPieceTable;
			this.lastInsertedSeparatorRunInfo.PieceTable = MainPieceTable;
			this.numberingLists = new NumberingListCollection();
			this.abstractNumberingLists = new AbstractNumberingListCollection();
			this.abstractNumberingListIdProvider = new AbstractNumberingListIdProvider(this);
			this.numberingListIdProvider = new NumberingListIdProvider(this);
			this.tableBorderInfoRepository = new BorderInfoRepository(UnitConverter);
			this.floatingObjectBorderInfoRepository = new BorderInfoRepository(UnitConverter);
			this.variables = new DocumentVariableCollection(this);
			this.footNotes = new FootNoteCollection();
			this.endNotes = new EndNoteCollection();
			this.mailMergeProperties = new MailMergeProperties();
		}
		void SubscribeDocumentObjectsEvents() {
			SubscribeMailMergePropertiesEvents();
		}
		protected internal virtual InternalAPI CreateInternalAPI() {
			return new InternalAPI(this, documentFormatsDependencies.ExportersFactory, documentFormatsDependencies.ImportersFactory);
		}
		protected internal virtual ICommandsCreationStrategy CreateCommandCreationStrategy() {
			return new RichEditCommandsCreationStrategy();
		}
		protected internal virtual  void ReplaceCommandCreationStrategy() {
			this.commandsCreationStrategy = new RichEditCommentCommandsCreationStrategy();
		}
		protected internal virtual RichEditDataControllerAdapterBase CreateMailMergeDataController() {
			return new RichEditDataControllerAdapter(new OfficeDataController());
		}
		public override void ClearCore() {
			base.ClearCore();
			if (cache != null) {
				cache.Dispose();
				cache = null;
			}
			if (selection != null) {
				UnsubscribeSelectionEvents();
				this.selection = null;
			}
			if (defaultCharacterProperties != null) {
				UnsubscribeCharacterPropertiesEvents();
				defaultCharacterProperties = null;
			}
			if (defaultParagraphProperties != null) {
				UnsubscribeParagraphPropertiesEvents();
				defaultParagraphProperties = null;
			}
			if (documentProperties != null) {
				UnsubscribeDocumentPropertiesEvents();
				documentProperties = null;
			}
			if (mailMergeProperties != null) {
				UnsubscribeMailMergePropertiesEvents();
				mailMergeProperties = null;
			}
			if (protectionProperties != null) {
				UnsubscribeProtectionPropertiesEvents();
				protectionProperties = null;
			}
			if (layoutOptions != null) {
				UnsubscribeRichEditLayoutOptions();
				layoutOptions = null;
			}
			if (documentCapabilities != null) {
				UnsubscribeDocumentCapabilitiesOptionsEvents();
				documentCapabilities = null;
			}
		}
		#region Properties
		public virtual bool SeparateModelForApiExport { get { return false; } }
		public bool ModelForExport { get; protected internal set; }
		public bool FieldResultModel { get; private set; }
		public bool IntermediateModel { get; protected internal set; }
		public DocumentCache Cache { get { return cache; } }
		public CharacterStyleCollection CharacterStyles { get { return characterStyles; } }
		public ParagraphStyleCollection ParagraphStyles { get { return paragraphStyles; } }
		public TableStyleCollection TableStyles { get { return tableStyles; } }
		public TableCellStyleCollection TableCellStyles { get { return tableCellStyles; } }
		public NumberingListStyleCollection NumberingListStyles { get { return numberingListStyles; } }
		public UnderlineRepository UnderlineRepository { get { return underlineRepository; } }
		public BorderLineRepository BorderLineRepository { get { return borderLineRepository; } }
		public StrikeoutRepository StrikeoutRepository { get { return strikeoutRepository; } }
		public override IDocumentModelPart MainPart { get { return MainPieceTable; } }
		public int LastExecutedCommandId { get; set; } 
		public int LastExecutedEditCommandId { get; set; } 
		public bool ShouldApplyAppearanceProperties { get; set; }
		[Obsolete("You should use either MainPieceTable or ActivePieceTable property instead (use TestPieceTable property in DEBUGTEST project configuration", true)]
		public PieceTable PieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return MainPieceTable; }
		}
#if DEBUGTEST
		public PieceTable TestPieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return MainPieceTable; }
		}
#endif
		public PieceTable MainPieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return pieceTable; }
		}
		public PieceTable ActivePieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return activePieceTable; }
		}
		public LineNumberCommonRun LineNumberRun { get { return lineNumberRun; } }
		#region Sections
		public SectionCollection Sections {
			[System.Diagnostics.DebuggerStepThrough]
			get { return sections; }
		}
		#endregion
		#region Headers
		public HeaderCollection Headers {
			[System.Diagnostics.DebuggerStepThrough]
			get { return headers; }
		}
		#endregion
		#region Footers
		public FooterCollection Footers {
			[System.Diagnostics.DebuggerStepThrough]
			get { return footers; }
		}
		#endregion
		public AbstractNumberingListIdProvider AbstractNumberingListIdProvider { get { return abstractNumberingListIdProvider; } }
		public NumberingListIdProvider NumberingListIdProvider { get { return numberingListIdProvider; } }
		#region NumberingLists
		public NumberingListCollection NumberingLists {
			[System.Diagnostics.DebuggerStepThrough]
			get { return numberingLists; }
		}
		#endregion
		#region AbstractNumberingLists
		public AbstractNumberingListCollection AbstractNumberingLists {
			[System.Diagnostics.DebuggerStepThrough]
			get { return abstractNumberingLists; }
		}
		#endregion
		#region InnerLastInsertedRunInfo
		protected internal LastInsertedRunInfo InnerLastInsertedRunInfo {
			[System.Diagnostics.DebuggerStepThrough]
			get { return lastInsertedRunInfo; }
		}
		#endregion
		public DocumentProperties DocumentProperties { get { return documentProperties; } }
		public ExtendedDocumentProperties ExtendedDocumentProperties { get { return extendedDocumentProperties; } }
		public string NumberDecimalSeparator { get { return numberDecimalSeparator; } set { numberDecimalSeparator = value; } }
		public string NumberGroupSeparator { get { return numberGroupSeparator; } set { numberGroupSeparator = value; } }
		public ParagraphProperties DefaultParagraphProperties { get { return defaultParagraphProperties; } }
		public CharacterProperties DefaultCharacterProperties { get { return defaultCharacterProperties; } }
		public TableProperties DefaultTableProperties { get { return defaultTableProperties; } }
		public TableRowProperties DefaultTableRowProperties { get { return defaultTableRowProperties; } }
		public TableCellProperties DefaultTableCellProperties { get { return defaultTableCellProperties; } }
		public Selection Selection { get { return selection; } }
		public EditingOptions EditingOptions { get { return editingOptions; } }
		public FieldOptions FieldOptions { get { return fieldOptions; } }
		public RichEditMailMergeOptions MailMergeOptions { get { return mailMergeOptions; } }
		public DocumentCapabilitiesOptions DocumentCapabilities { get { return documentCapabilities; } }
		public RichEditLayoutOptions LayoutOptions { get { return layoutOptions; } }
		public DocumentSaveOptions DocumentSaveOptions { get { return documentSaveOptions; } }
		public RichEditDocumentImportOptions DocumentImportOptions { get { return documentImportOptions; } }
		public RichEditDocumentExportOptions DocumentExportOptions { get { return documentExportOptions; } }
		public BookmarkOptions BookmarkOptions { get { return bookmarkOptions; } }
		public CopyPasteOptions CopyPasteOptions { get { return copyPasteOptions; } }
		public RangePermissionOptions RangePermissionOptions { get { return rangePermissionOptions; } }
		public CommentOptions CommentOptions { get { return commentOptions; } }
		public DevExpress.XtraRichEdit.DocumentSearchOptions SearchOptions { get { return searchOptions; } }
		public FormattingMarkVisibilityOptions FormattingMarkVisibilityOptions { get { return formattingMarkVisibilityOptions; } }
		public AuthenticationOptions AuthenticationOptions { get { return authenticationOptions; } }
		public TableOptions TableOptions { get { return tableOptions; } }
		public AutoCorrectOptions AutoCorrectOptions { get { return autoCorrectOptions; } }
		public RichEditBehaviorOptions BehaviorOptions { get { return behaviorOptions; } }
		public PrintingOptions PrintingOptions { get { return printingOptions; } }
		public SpellCheckerOptions SpellCheckerOptions { get { return spellCheckerOptions; } }
		public SafeDocumentModelEditor SafeEditor { get { return safeEditor; } }
		public UnsafeDocumentModelEditor UnsafeEditor { get { return unsafeEditor; } }
		public InternalAPI InternalAPI { get { return internalAPI; } }
		public ICommandsCreationStrategy CommandsCreationStrategy { get { return commandsCreationStrategy; } }
		protected internal DocumentModelDeferredChanges DeferredChanges { get { return deferredChanges; } }
		public bool Modified { get { return History.Modified; } set { History.Modified = value; } }
		public SearchParameters SearchParameters { get { return searchParameters; } }
		public SearchContext SearchContext { get { return searchContext; } }
		protected internal StyleLinkManager StyleLinkManager { get { return styleLinkManager; } }
		public MailMergeProperties MailMergeProperties { get { return mailMergeProperties; } }
		public DocumentProtectionProperties ProtectionProperties { get { return protectionProperties; } }
		public LangInfo ThemeFontLangInfo { get; set; }
		public bool StyleLockTheme { get; set; }
		protected internal virtual RichEditDataControllerAdapterBase MailMergeDataController { get { return mailMergeDataController; } }
		protected internal int CountIdForPeaceTables { get { return countIdForPeaceTables; } set { countIdForPeaceTables = value; } }
		public bool IsEmpty { get { return MainPieceTable.IsEmpty && Sections.Count <= 1 && !Sections.First.HasNonEmptyHeadersOrFooters; } }
		protected internal BorderInfoRepository TableBorderInfoRepository { get { return tableBorderInfoRepository; } }
		protected internal BorderInfoRepository FloatingObjectBorderInfoRepository { get { return floatingObjectBorderInfoRepository; } }
		protected internal bool IsDocumentProtectionEnabled { get { return ProtectionProperties.EnforceProtection && ProtectionProperties.ProtectionType == DocumentProtectionType.ReadOnly; } }
		protected bool SyntaxHighlightSuspended { get { return syntaxHighlightSuspendCount > 0; } }
		protected bool SuppressFieldsChangeNotification { get { return suppressFieldsChangeNotification; } }
		protected internal BottomBorder EmptyBottomBorder {
			get {
				if (emptyBottomBorder == null) {
					emptyBottomBorder = new BottomBorder(this.pieceTable);
					emptyBottomBorder.BeginInit();
					emptyBottomBorder.Style = BorderLineStyle.Disabled;
					emptyBottomBorder.EndInit();
				}
				return emptyBottomBorder;
			}
		}
		protected internal TopBorder EmptyTopBorder {
			get {
				if (emptyTopBorder == null) {
					emptyTopBorder = new TopBorder(this.pieceTable);
					emptyTopBorder.BeginInit();
					emptyTopBorder.Style = BorderLineStyle.Disabled;
					emptyTopBorder.EndInit();
				}
				return emptyTopBorder;
			}
		}
		public DocumentVariableCollection Variables { get { return variables; } }
		public FootNoteCollection FootNotes { get { return footNotes; } }
		public EndNoteCollection EndNotes { get { return endNotes; } }
		public WebSettings WebSettings { get { return htmlSettings; } }
		public CommentColorer CommentColorer { get { return commentColorer; } set { commentColorer = value; } }
		internal int SuppressPerformLayout { get { return suppressPerformLayout; } set { suppressPerformLayout = value; } }
		internal string CommentHeading { get { return commentHeading; } }
		#endregion
		#region Events
		#region BeginDocumentUpdate
		EventHandler onBeginDocumentUpdate;
		public event EventHandler BeginDocumentUpdate { add { onBeginDocumentUpdate += value; } remove { onBeginDocumentUpdate -= value; } }
		protected internal virtual void RaiseBeginDocumentUpdate() {
			if (onBeginDocumentUpdate != null)
				onBeginDocumentUpdate(this, EventArgs.Empty);
		}
		#endregion
		#region EndDocumentUpdate
		DocumentUpdateCompleteEventHandler onEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler EndDocumentUpdate { add { onEndDocumentUpdate += value; } remove { onEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseEndDocumentUpdate() {
			if (onEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region BeforeEndDocumentUpdate
		DocumentUpdateCompleteEventHandler onBeforeEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler BeforeEndDocumentUpdate { add { onBeforeEndDocumentUpdate += value; } remove { onBeforeEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseBeforeEndDocumentUpdate() {
			if (onBeforeEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onBeforeEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region AfterEndDocumentUpdate
		DocumentUpdateCompleteEventHandler onAfterEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler AfterEndDocumentUpdate { add { onAfterEndDocumentUpdate += value; } remove { onAfterEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseAfterEndDocumentUpdate() {
			if (onAfterEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onAfterEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region InnerSelectionChanged
		EventHandler onInnerSelectionChanged;
		internal event EventHandler InnerSelectionChanged { add { onInnerSelectionChanged += value; } remove { onInnerSelectionChanged -= value; } }
		protected internal virtual void RaiseInnerSelectionChanged() {
			if (onInnerSelectionChanged != null)
				onInnerSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
		EventHandler onSelectionChanged;
		internal event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionReseted
		EventHandler onSelectionReseted;
		protected internal event EventHandler SelectionReseted { add { onSelectionReseted += value; } remove { onSelectionReseted -= value; } }
		protected internal virtual void RaiseSelectionReseted() {
			if (onSelectionReseted != null)
				onSelectionReseted(this, EventArgs.Empty);
		}
		#endregion
		#region InnerContentChanged
		EventHandler onInnerContentChanged;
		internal event EventHandler InnerContentChanged { add { onInnerContentChanged += value; } remove { onInnerContentChanged -= value; } }
		protected internal virtual void RaiseInnerContentChanged() {
			if (onInnerContentChanged != null)
				onInnerContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		internal event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if (onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InvalidFormatException
		RichEditInvalidFormatExceptionEventHandler onInvalidFormatException;
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException { add { onInvalidFormatException += value; } remove { onInvalidFormatException -= value; } }
		public virtual void RaiseInvalidFormatException(Exception e) {
			if (onInvalidFormatException != null)
				onInvalidFormatException(this, new RichEditInvalidFormatExceptionEventArgs(e));
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		internal event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region CalculateDocumentVariable
		CalculateDocumentVariableEventHandler onCalculateDocumentVariable;
		internal event CalculateDocumentVariableEventHandler CalculateDocumentVariable {
			add {				
				onCalculateDocumentVariable += value;
			}
			remove { onCalculateDocumentVariable -= value; }
		}
		protected internal virtual bool RaiseCalculateDocumentVariable(CalculateDocumentVariableEventArgs args) {
			if (onCalculateDocumentVariable != null) {
				onCalculateDocumentVariable(this, args);
				return args.Handled;
			}
			else
				return false;
		}
		#endregion
		#region SectionInserted
		SectionEventHandler onSectionInserted;
		internal event SectionEventHandler SectionInserted { add { onSectionInserted += value; } remove { onSectionInserted -= value; } }
		protected internal virtual void RaiseSectionInserted(SectionIndex sectionIndex) {
			if (onSectionInserted != null) {
				SectionEventArgs args = new SectionEventArgs(sectionIndex);
				onSectionInserted(this, args);
			}
		}
		#endregion
		#region SectionRemoved
		SectionEventHandler onSectionRemoved;
		internal event SectionEventHandler SectionRemoved { add { onSectionRemoved += value; } remove { onSectionRemoved -= value; } }
		protected internal virtual void RaiseSectionRemoved(SectionIndex sectionIndex) {
			if (onSectionRemoved != null) {
				SectionEventArgs args = new SectionEventArgs(sectionIndex);
				onSectionRemoved(this, args);
			}
		}
		#endregion
		#region BeforeImport
		BeforeImportEventHandler onBeforeImport;
		public event BeforeImportEventHandler BeforeImport { add { onBeforeImport += value; } remove { onBeforeImport -= value; } }
		public virtual void RaiseBeforeImport(DocumentFormat format, IImporterOptions options) {
			if (onBeforeImport != null) {
				BeforeImportEventArgs args = new BeforeImportEventArgs(format, options);
				onBeforeImport(this, args);
			}
		}
		#endregion
		#region BeforeExport
		BeforeExportEventHandler onBeforeExport;
		public event BeforeExportEventHandler BeforeExport { add { onBeforeExport += value; } remove { onBeforeExport -= value; } }
		public virtual void RaiseBeforeExport(DocumentFormat format, IExporterOptions options) {
			if (onBeforeExport != null) {
				BeforeExportEventArgs args = new BeforeExportEventArgs(format, options);
				onBeforeExport(this, args);
			}
		}
		#endregion
		#region AfterExport
		EventHandler onAfterExport;
		public event EventHandler AfterExport { add { onAfterExport += value; } remove { onAfterExport -= value; } }
		protected internal virtual void RaiseAfterExport() {
			if (onAfterExport != null)
				onAfterExport(this, EventArgs.Empty);
		}
		#endregion
		#region InnerDocumentCleared
		EventHandler onInnerDocumentCleared;
		public event EventHandler InnerDocumentCleared { add { onInnerDocumentCleared += value; } remove { onInnerDocumentCleared -= value; } }
		protected internal virtual void RaiseInnerDocumentCleared() {
			if (onInnerDocumentCleared != null)
				onInnerDocumentCleared(this, EventArgs.Empty);
		}
		#endregion
		#region DocumentCleared
		EventHandler onDocumentCleared;
		public event EventHandler DocumentCleared { add { onDocumentCleared += value; } remove { onDocumentCleared -= value; } }
		protected internal virtual void RaiseDocumentCleared() {
			if (onDocumentCleared != null)
				onDocumentCleared(this, EventArgs.Empty);
		}
		#endregion
		#region HyperlinkInfoInserted
		HyperlinkInfoEventHandler onHyperlinkInfoInserted;
		internal event HyperlinkInfoEventHandler HyperlinkInfoInserted { add { onHyperlinkInfoInserted += value; } remove { onHyperlinkInfoInserted -= value; } }
		protected internal virtual void RaiseHyperlinkInfoInserted(PieceTable pieceTable, int fieldIndex) {
			if (onHyperlinkInfoInserted != null)
				onHyperlinkInfoInserted(this, new HyperlinkInfoEventArgs(pieceTable, fieldIndex));
		}
		#endregion
		#region HyperlinkInfoDeleted
		HyperlinkInfoEventHandler onHyperlinkInfoDeleted;
		internal event HyperlinkInfoEventHandler HyperlinkInfoDeleted { add { onHyperlinkInfoDeleted += value; } remove { onHyperlinkInfoDeleted -= value; } }
		protected internal virtual void RaiseHyperlinkInfoDeleted(PieceTable pieceTable, int fieldIndex) {
			if (onHyperlinkInfoDeleted != null)
				onHyperlinkInfoDeleted(this, new HyperlinkInfoEventArgs(pieceTable, fieldIndex));
		}
		#endregion
		#region CustomMarkInserted
		CustomMarkEventHandler onCustomMarkInserted;
		internal event CustomMarkEventHandler CustomMarkInserted { add { onCustomMarkInserted += value; } remove { onCustomMarkInserted -= value; } }
		protected internal virtual void RaiseCustomMarkInserted(PieceTable pieceTable, int customMarkIndex) {
			if (onCustomMarkInserted != null)
				onCustomMarkInserted(this, new CustomMarkEventArgs(pieceTable, customMarkIndex));
		}
		#endregion
		#region CustomMarkDeleted
		CustomMarkEventHandler onCustomMarkDeleted;
		internal event CustomMarkEventHandler CustomMarkDeleted { add { onCustomMarkDeleted += value; } remove { onCustomMarkDeleted -= value; } }
		protected internal virtual void RaiseCustomMarkDeleted(PieceTable pieceTable, int customMarkIndex) {
			if (onCustomMarkDeleted != null)
				onCustomMarkDeleted(this, new CustomMarkEventArgs(pieceTable, customMarkIndex));
		}
		#endregion
		#region CommentEditing
		public event CommentEditingEventHandler CommentEditing;
		protected internal virtual void RaiseCommentEditing(CommentEditingEventArgs e) {
			if (CommentEditing != null)
				CommentEditing(this, e);
		}
		#endregion
		#region CommentInserted
		internal event CommentEventHandler CommentInserted;
		protected internal virtual void RaiseCommentInserted(CommentEventArgs e) {
			if (CommentInserted != null)
				CommentInserted(this, e);
		}
		#endregion
		#region CommentDeleted
		internal event CommentEventHandler CommentDeleted;
		protected internal virtual void RaiseCommentDeleted(CommentEventArgs e) {
			if (IsUpdateLocked) { }
			if (CommentDeleted != null)
				CommentDeleted(this, e);
		}
		#endregion
		#region CommentChangedViaAPI
		internal event CommentEventHandler CommentChangedViaAPI;
		protected internal virtual void RaiseCommentChangedViaAPI(CommentEventArgs e) {
			if (CommentChangedViaAPI != null)
				CommentChangedViaAPI(this, e);
		}
		#endregion
		#region MailMergeStarted
		MailMergeStartedEventHandler onMailMergeStarted;
		public event MailMergeStartedEventHandler MailMergeStarted { add { onMailMergeStarted += value; } remove { onMailMergeStarted -= value; } }
		protected internal virtual bool RaiseMailMergeStarted(MailMergeStartedEventArgs args) {
			if (onMailMergeStarted != null) {
				onMailMergeStarted(this, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region MailMergeRecordStarted
		MailMergeRecordStartedEventHandler onMailMergeRecordStarted;
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted { add { onMailMergeRecordStarted += value; } remove { onMailMergeRecordStarted -= value; } }
		protected internal virtual bool RaiseMailMergeRecordStarted(MailMergeRecordStartedEventArgs args) {
			if (onMailMergeRecordStarted != null) {
				onMailMergeRecordStarted(this, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region MailMergeRecordFinished
		MailMergeRecordFinishedEventHandler onMailMergeRecordFinished;
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished { add { onMailMergeRecordFinished += value; } remove { onMailMergeRecordFinished -= value; } }
		protected internal virtual bool RaiseMailMergeRecordFinished(MailMergeRecordFinishedEventArgs args) {
			if (onMailMergeRecordFinished != null) {
				onMailMergeRecordFinished(this, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region MailMergeFinished
		MailMergeFinishedEventHandler onMailMergeFinished;
		public event MailMergeFinishedEventHandler MailMergeFinished { add { onMailMergeFinished += value; } remove { onMailMergeFinished -= value; } }
		protected internal virtual void RaiseMailMergeFinished(MailMergeFinishedEventArgs args) {
			if (onMailMergeFinished != null)
				onMailMergeFinished(this, args);
		}
		#endregion
		#region PageBackgroundChanged
		EventHandler onPageBackgroundChanged;
		internal event EventHandler PageBackgroundChanged { add { onPageBackgroundChanged += value; } remove { onPageBackgroundChanged -= value; } }
		protected internal virtual void RaisePageBackgroundChanged() {
			if (onPageBackgroundChanged != null)
				onPageBackgroundChanged(this, EventArgs.Empty);
		}
		#endregion
		public event EventHandler FieldsChanged;
		protected internal virtual void FireFieldsChanged() {
			if (FieldsChanged != null) {
				FieldsChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		protected internal virtual MainContentType CreateMainContentType() {
			return new MainContentType(this);
		}
		protected internal virtual PieceTable CreatePieceTable(ContentTypeBase contentType) {
			return new PieceTable(this, contentType);
		}
		public override void SetFontCacheManager(FontCacheManager fontCacheManager) {
			base.SetFontCacheManager(fontCacheManager);
			this.lineNumberRun.ResetFontCacheIndex();
			foreach (PieceTable pieceTable in GetPieceTables(true))
				pieceTable.ClearFontCacheIndices();
		}
		protected override void OnLayoutUnitChanged() {
			base.OnLayoutUnitChanged();
			ResetParagraphs();
		}
		protected internal List<PieceTable> GetPieceTables(bool includeUnreferenced) {
			List<PieceTable> result = new List<PieceTable>();
			MainPieceTable.AddPieceTables(result, includeUnreferenced);
			SectionIndex count = new SectionIndex(Sections.Count);
			for (SectionIndex i = new SectionIndex(0); i < count; i++)
				Sections[i].AddPieceTables(result, includeUnreferenced);
			AddNotes(FootNotes, result, includeUnreferenced);
			AddNotes(EndNotes, result, includeUnreferenced);
			return result;
		}
		protected internal virtual void AddNotes<T>(List<T> notes, List<PieceTable> result, bool includeUnreferenced) where T : FootNoteBase<T> {
			int count = notes.Count;
			for (int i = 0; i < count; i++)
				notes[i].PieceTable.AddPieceTables(result, includeUnreferenced);
		}
		protected internal virtual List<Bookmark> GetBookmarks() {
			return GetBookmarks(true);
		}
		protected internal virtual List<Bookmark> GetBookmarks(bool includeHiddenBookmarks) {
			List<Bookmark> result = new List<Bookmark>();
			foreach (PieceTable pieceTable in GetPieceTables(false))
				result.AddRange(pieceTable.GetBookmarks(includeHiddenBookmarks));
			return result;
		}
		protected internal Hyphenizer CreateHyphenizer() {
#if DEBUGTEST 
			return DevExpress.XtraRichEdit.Tests.TestHyphenizer.CreateHyphenizer();
#elif DEBUG
#if !DXPORTABLE
			System.IO.Stream patterns = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraRichEdit.Tests.hyphen.dic");
			System.IO.Stream exceptions = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraRichEdit.Tests.hyphen_exc.dic");
#endif
			return new Hyphenizer(patterns, exceptions);
#else
			return new EmptyHyphenizer();
#endif
		}
		object CreateHyphenizationService(IServiceContainer container, Type serviceType) {
			return new HyphenationService(CreateHyphenizer());
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IDocumentImportManagerService), documentFormatsDependencies.ImportManagerService);
			AddService(typeof(IDocumentExportManagerService), documentFormatsDependencies.ExportManagerService);
			AddService(typeof(IUriStreamService), new UriStreamService());
			AddService(typeof(DevExpress.Office.Services.IUriProviderService), new DevExpress.Office.Services.Implementation.UriProviderService());
			AddDataServices();
			AddService(typeof(IFieldCalculatorService), new FieldCalculatorService());
			AddService(typeof(IFieldFunctionCalculatorFactory), new FieldFunctionCalculatorFactory());
			AddService(typeof(IThreadPoolService), new OfficeThreadPoolService());
			AddService(typeof(IRichEditProgressIndicationService), new RichEditProgressIndicationService(this));
			AddService(typeof(IHyphenationService), CreateHyphenizationService);
			AddService(typeof(IUserGroupListService), new UserGroupListService());
			AddService(typeof(IHoverLayoutItemsService), new HoverLayoutItemsService());
		}
		protected virtual void AddDataServices() {
			MailMergeDataService service = new MailMergeDataService(MailMergeDataController);
			AddService(typeof(IMailMergeDataService), service);
			AddService(typeof(IFieldDataService), service);
		}
		protected internal virtual void SubscribeSelectionEvents() {
			if (Selection != null)
				Selection.Changed += OnSelectionChanged;
		}
		protected internal virtual void UnsubscribeSelectionEvents() {
			if (Selection != null)
				Selection.Changed -= OnSelectionChanged;
		}
		protected internal virtual void SubscribeDocumentPropertiesEvents() {
			if (documentProperties != null) {
				documentProperties.ObtainAffectedRange += OnDocumentPropertiesObtainAffectedRange;
				documentProperties.PageBackgroundChanged += OnDocumentPropertiesPageBackgroundChanged;
			}
		}
		protected internal virtual void UnsubscribeDocumentPropertiesEvents() {
			if (documentProperties != null) {
				documentProperties.ObtainAffectedRange -= OnDocumentPropertiesObtainAffectedRange;
				documentProperties.PageBackgroundChanged -= OnDocumentPropertiesPageBackgroundChanged;
			}
		}
		protected internal virtual void SubscribeProtectionPropertiesEvents() {
			if (protectionProperties != null)
				protectionProperties.ObtainAffectedRange += OnProtectionPropertiesObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeProtectionPropertiesEvents() {
			if (protectionProperties != null)
				protectionProperties.ObtainAffectedRange -= OnProtectionPropertiesObtainAffectedRange;
		}
		protected internal virtual void SubscribeCharacterPropertiesEvents() {
			if (defaultCharacterProperties != null)
				defaultCharacterProperties.ObtainAffectedRange += OnDefaultCharacterPropertiesObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeCharacterPropertiesEvents() {
			if (defaultCharacterProperties != null)
				defaultCharacterProperties.ObtainAffectedRange -= OnDefaultCharacterPropertiesObtainAffectedRange;
		}
		protected internal virtual void SubscribeParagraphPropertiesEvents() {
			if (defaultParagraphProperties != null)
				defaultParagraphProperties.ObtainAffectedRange += OnDefaultParagraphPropertiesObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeParagraphPropertiesEvents() {
			if (defaultCharacterProperties != null)
				defaultParagraphProperties.ObtainAffectedRange -= OnDefaultParagraphPropertiesObtainAffectedRange;
		}
		protected internal virtual void SubscribeMailMergePropertiesEvents() {
			this.mailMergeProperties.ActiveRecordChanged += OnMailMergeActiveRecordChanged;
			this.mailMergeProperties.ViewMergedDataChanged += OnMailMergeViewMergedDataChanged;
			this.mailMergeProperties.DataSourceChanged += OnMailMergeDataSourceChanged;
		}
		protected internal virtual void UnsubscribeMailMergePropertiesEvents() {
			this.mailMergeProperties.ActiveRecordChanged -= OnMailMergeActiveRecordChanged;
			this.mailMergeProperties.ViewMergedDataChanged -= OnMailMergeViewMergedDataChanged;
			this.mailMergeProperties.DataSourceChanged -= OnMailMergeDataSourceChanged;
		}
		protected internal void SubscribeMailMergeDataControllerEvents() {
			this.mailMergeDataController.CurrentRowChanged += OnMailMergeCurrentRowChanged;
			this.mailMergeDataController.DataSourceChanged += OnMailMergeDataSourceChanged;
		}
		protected internal void UnubscribeMailMergeDataControllerEvents() {
			this.mailMergeDataController.CurrentRowChanged -= OnMailMergeCurrentRowChanged;
			this.mailMergeDataController.DataSourceChanged -= OnMailMergeDataSourceChanged;
		}
		protected internal virtual void OnMailMergeCurrentRowChanged(object sender, EventArgs e) {
			MailMergeProperties.ActiveRecord = MailMergeDataController.CurrentControllerRow;
		}
		void OnMailMergeActiveRecordChanged(object sender, EventArgs e) {
			MailMergeDataController.CurrentControllerRow = MailMergeProperties.ActiveRecord;
			MailMergeOptions.ActiveRecord = MailMergeProperties.ActiveRecord;
			UpdateFields(UpdateFieldOperationType.Normal);
		}
		void OnMailMergeViewMergedDataChanged(object sender, EventArgs e) {
			MailMergeOptions.ViewMergedData = MailMergeProperties.ViewMergedData;
			UpdateFields(UpdateFieldOperationType.Normal);
		}
		protected virtual void OnMailMergeDataSourceChanged(object sender, EventArgs e) {
			MailMergeOptions.DataSource = MailMergeDataController.DataSource;
			MailMergeOptions.DataMember = MailMergeDataController.DataMember;
			UpdateFields(UpdateFieldOperationType.Normal);
		}
		public virtual void UpdateFields(UpdateFieldOperationType updateType) {
			UpdateFields(updateType, null);
		}
		public virtual void UpdateFields(UpdateFieldOperationType updateType, FieldUpdateOnLoadOptions options) {
			BeginUpdate();
			try {
				IFieldCalculatorService calculatorService = GetService<IFieldCalculatorService>();
				if (calculatorService != null && updateType == UpdateFieldOperationType.Load)
					calculatorService.BeginUpdateFieldsOnLoad(options);
				try {
					List<PieceTable> pieceTables = GetPieceTables(false);
					int count = pieceTables.Count;
					for (int i = 0; i < count; i++)
						pieceTables[i].FieldUpdater.UpdateFields(updateType);
				}
				finally {
					if (calculatorService != null && updateType == UpdateFieldOperationType.Load)
						calculatorService.EndUpdateFieldsOnLoad();
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SubscribeDocumentCapabilitiesOptionsEvents() {
			if (documentCapabilities != null)
				documentCapabilities.Changed += OnDocumentCapabilitiesChanged;
		}
		protected internal virtual void UnsubscribeDocumentCapabilitiesOptionsEvents() {
			if (documentCapabilities != null)
				documentCapabilities.Changed -= OnDocumentCapabilitiesChanged;
		}
		void OnDocumentCapabilitiesChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "Undo")
				SwitchToNormalHistory(false);
		}
		protected internal virtual void SubscribeFormattingMarkVisibilityOptions() {
			if (formattingMarkVisibilityOptions != null)
				formattingMarkVisibilityOptions.Changed += OnFormattingMarkVisibilityOptionsChanged;
		}
		protected internal virtual void SubscribeRichEditLayoutOptions() {
			if (layoutOptions != null)
				layoutOptions.Changed += OnLayoutOptionsChanged;
		}
		protected internal virtual void SubscribeTableOptions() {
			if (tableOptions != null)
				tableOptions.Changed += OnTableOptionsChanged;
		}
		protected internal virtual void SubscribeCommentOptions() {
			if (commentOptions != null)
				commentOptions.CommentOptionsChanged += OnCommentOptionChanged;
		}
		protected internal virtual void UnsubscribeCommentOptions() {
			if (commentOptions != null)
				commentOptions.CommentOptionsChanged -= OnCommentOptionChanged;
		}
		private void OnCommentOptionChanged(object sender, CommentOptions.CommentOptionsChangedEventArgs e) {
			BeginUpdate();
			if (e.VisibilityChanged || e.ShowAllAuthorsChanged)
				CaculateVisibleAuthors();
			if (e.VisibilityChanged || e.ShowAllAuthorsChanged || e.VisibleAuthorsChanged || e.HighlightCommentedRangeChanged)
				ApplyChanges(MainPieceTable, DocumentModelChangeType.CommentOptionsVisibilityChanged, RunIndex.MinValue, RunIndex.MaxValue);
			if (e.ColorChanged )
				ApplyChanges(MainPieceTable, DocumentModelChangeType.CommentColorPropertiesChanged, RunIndex.MinValue, RunIndex.MaxValue);
			EndUpdate();
		}
		protected internal virtual void UnsubscribeRichEditLayoutOptions() {
			if (layoutOptions != null)
				layoutOptions.Changed -= OnLayoutOptionsChanged;
		}
		void OnFormattingMarkVisibilityOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "ShowHiddenText" || e.Name == "HiddenText")
				OnShowHiddenTextOptionsChanged();
		}
		void OnTableOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "GridLines")
				OnGridLinesOptionsChanged();
		}
		void OnLayoutOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == ViewLayoutOptionsBase.AllowTablesToExtendIntoMarginsPropertyName)
				OnLayoutTablesToExtendIntoMarginsOptionsChanged();
			if (e.Name == ViewLayoutOptionsBase.MatchHorizontalTableIndentsToTextEdgePropertyName)
				OnMatchHorizontalTableIndentsToTextEdgeOptionsChanged();
		}
		protected internal virtual void OnDefaultCharacterPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		protected internal virtual void OnDefaultParagraphPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		void OnDocumentPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		void OnDocumentPropertiesPageBackgroundChanged(object sender, EventArgs e) {
			RaisePageBackgroundChanged();
		}
		void OnProtectionPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		protected override void OnHistoryOperationCompleted(object sender, EventArgs e) {
			RaiseInnerContentChanged();
			if (IsUpdateLockedOrOverlapped)
				ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged, RunIndex.DontCare, RunIndex.DontCare);
			else
				RaiseContentChanged();
		}
		protected override void OnHistoryModifiedChanged(object sender, EventArgs e) {
			if (IsUpdateLockedOrOverlapped)
				ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseModifiedChanged, RunIndex.DontCare, RunIndex.DontCare);
			else
				RaiseModifiedChanged();
		}
		protected internal virtual void OnSelectionChanged(object sender, EventArgs e) {
			RaiseInnerSelectionChanged();
			if (IsUpdateLocked) {
				DocumentModelChangeActions actions = DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				if (DeferredChanges.SelectionLengthChanged || Selection.Length > 0)
					actions |= DocumentModelChangeActions.Redraw;
				ActivePieceTable.ApplyChangesCore(actions, RunIndex.DontCare, RunIndex.DontCare);
			}
			else
				RaiseSelectionChanged();
		}
		bool ShouldFormatSpecialEmptyParagraph() {
			if (Selection.Items.Count == 1 && Selection.Length == 0) {
				ParagraphIndex paragraphIndex = Selection.Interval.Start.ParagraphIndex;
				if (paragraphIndex >= ParagraphIndex.Zero && paragraphIndex < new ParagraphIndex(Selection.PieceTable.Paragraphs.Count)) {
					Paragraph paragraph = Selection.PieceTable.Paragraphs[paragraphIndex];
					return (IsSpecialEmptyParagraphAfterInnerTable(paragraph, paragraph.GetCell()));
				}
			}
			return false;
		}
		public bool IsSpecialEmptyParagraphAfterInnerTable(Paragraph paragraph, TableCell cell) {
			if (!paragraph.BoxCollection.IsValid)
				return false;
			if (paragraph.BoxCollection.Count > 1 || cell == null)
				return false;
			ParagraphIndex paragraphIndex = paragraph.Index;
			if (paragraphIndex == new ParagraphIndex(0))
				return false;
			if (cell.EndParagraphIndex != paragraphIndex)
				return false;
			PieceTable pieceTable = paragraph.PieceTable;
			TableCell prevParagraphCell = pieceTable.Paragraphs[paragraphIndex - 1].GetCell();
			if (prevParagraphCell == null)
				return false;
			return Object.ReferenceEquals(prevParagraphCell.Table.ParentCell, cell);
		}
		public bool IsCursorInParagraph(Paragraph paragraph) {
			PieceTable pieceTable = paragraph.PieceTable;
			Selection selection = pieceTable.DocumentModel.Selection;
			if (Object.ReferenceEquals(pieceTable, selection.PieceTable) && selection.Items.Count == 1 && selection.Length == 0 && selection.Start >= paragraph.LogPosition && selection.Start <= paragraph.EndLogPosition)
				return true;
			return false;
		}
		internal bool IsTextSelectedOnly() {
			RunIndex startIndex = Selection.Interval.NormalizedStart.RunIndex;
			RunIndex endIndex = Selection.Interval.NormalizedEnd.RunIndex;
			PieceTable pieceTable = Selection.PieceTable;
			for (RunIndex i = startIndex; i <= endIndex; i++) {
				if (!pieceTable.VisibleTextFilter.IsRunVisible(i))
					continue;
				if (!(pieceTable.Runs[i] is TextRun))
					return false;
			}
			return true;
		}
		protected override DocumentHistory CreateDocumentHistory() {
			if (DocumentCapabilities.UndoAllowed)
				return new RichEditDocumentHistory(this);
			else
				return new DisabledHistory(this);
		}
		protected internal virtual void SwitchToNormalSelection() {
			SwitchToSelectionCore(CreateNormalSelection());
		}
		protected internal virtual Selection CreateNormalSelection() {
			return new Selection(ActivePieceTable);
		}
		protected internal virtual void SwitchToEmptySelection() {
			SwitchToSelectionCore(new EmptySelection(MainPieceTable));
		}
		protected internal virtual void SwitchToSelectionCore(Selection newSelection) {
			UnsubscribeSelectionEvents();
			this.selection = newSelection;
			SubscribeSelectionEvents();
			if (IsUpdateLocked)
				DeferredChanges.ResetSelectionChanged();
			RaiseSelectionReseted();
		}
		public virtual void BeginSetContent() {
			if (IsUpdateLocked && DeferredChanges.IsSetContentMode)
				Exceptions.ThrowInternalException();
			foreach (PieceTable pieceTable in GetPieceTables(true))
				pieceTable.OnBeginSetContent();
			BeginUpdate();
			DeferredChanges.IsSetContentMode = true;
			InternalAPI.ClearAnchors();
			ClearDocumentCore(true, true);
			SwitchToEmptyHistory(true);
			SetActivePieceTable(MainPieceTable, null);
			SwitchToEmptySelection();
			RaiseInnerDocumentCleared();
			RaiseDocumentCleared();
		}
		public virtual DocumentModel CreateDocumentModelForExport(Action<DocumentModel> initializeEmptyDocumentModel) {
			DocumentModel result = CreateNew();
			result.DisableCheckDocumentModelIntegrity = DisableCheckDocumentModelIntegrity;
			result.History.DisableHistory();
			result.LayoutUnit = LayoutUnit;
			if (result.LayoutUnit == DevExpress.Office.DocumentLayoutUnit.Pixel)
				result.LayoutUnit = DevExpress.Office.DocumentLayoutUnit.Document;
			initializeEmptyDocumentModel(result);
			SubscribeEventsForExport(result);
			result.LayoutOptions.PrintLayoutView.AllowTablesToExtendIntoMargins = LayoutOptions.PrintLayoutView.AllowTablesToExtendIntoMargins; 
			result.BookmarkOptions.AllowNameResolution = BookmarkOptions.AllowNameResolution;
			DocumentLogPosition startLogPosition = MainPieceTable.DocumentStartLogPosition;
			DocumentLogPosition endLogPosition = MainPieceTable.DocumentEndLogPosition;
			DocumentModelCopyOptions copyOptions = CreateDocumentModelCopyOptions(startLogPosition, endLogPosition - startLogPosition + 1);
			SetModelForExportCopyOptions(copyOptions);
			CopyDocumentModelOptions(result);
			result.BeginSetContentForExport();
			result.commentColorer = this.commentColorer;
			result.InheritServicesForExport(this);
			result.ExtendedDocumentProperties.SetPages(ExtendedDocumentProperties.Pages);
			IDocumentLayoutService service = GetService<IDocumentLayoutService>();
			if (service != null)
				service.CreateService(result);
			try {
				DocumentModelCopyCommand command = CreateDocumentModelCopyCommand(MainPieceTable, result, copyOptions);
				command.UpdateFieldOperationType = UpdateFieldOperationType.CreateModelForExport;
				command.UpdateIntervals = true;
				command.Execute();
				result.UpdateTableOfContents();
				result.MainPieceTable.FixLastParagraph();
			}
			finally {
				if (service != null)
					service.RemoveServise(result);
				result.EndSetContentForExport(DocumentModelChangeType.LoadNewDocument, false);
			}
			result.DocumentProperties.CopyFrom(DocumentProperties.Info);
			result.DocumentExportOptions.CopyFrom(DocumentExportOptions);
			result.CommentOptions.CopyFrom(this.CommentOptions);
			result.PrintingOptions.EnablePageBackgroundOnPrint = PrintingOptions.EnablePageBackgroundOnPrint; 
			result.PrintingOptions.EnableCommentBackgroundOnPrint = PrintingOptions.EnableCommentBackgroundOnPrint;
			result.PrintingOptions.EnableCommentFillOnPrint = PrintingOptions.EnableCommentFillOnPrint;
			result.History.EnableHistory();
			return result;
		}
		protected internal virtual void CopyDocumentModelOptions(DocumentModel result) {
			result.FieldOptions.CopyFrom(FieldOptions);
			result.PrintingOptions.UpdateDocVariablesBeforePrint = PrintingOptions.UpdateDocVariablesBeforePrint;
#if !SL
			result.PrintingOptions.DrawLayoutFromSilverlightRendering = PrintingOptions.DrawLayoutFromSilverlightRendering;
#endif
		}
		protected internal virtual void SubscribeEventsForExport(DocumentModel modelForExport) {
			modelForExport.CalculateDocumentVariable += RaiseCalculateDocumentVariableForExport;
			modelForExport.BeforeExport += RaiseBeforeExportFromExportModel;
			modelForExport.AfterExport += RaiseAfterExportFromExportModel;
		}
		protected internal virtual void UnsubscribeEventsForExport() {
			onCalculateDocumentVariable -= RaiseCalculateDocumentVariableForExport;
			onBeforeExport -= RaiseBeforeExportFromExportModel;
			onAfterExport -= RaiseAfterExportFromExportModel;
		}
		protected virtual void RaiseBeforeExportFromExportModel(object sender, BeforeExportEventArgs e) {
			RaiseBeforeExport(e.DocumentFormat, e.Options);
		}
		protected virtual void RaiseAfterExportFromExportModel(object sender, EventArgs e) {
			RaiseAfterExport();
		}
		protected virtual void RaiseCalculateDocumentVariableForExport(object sender, CalculateDocumentVariableEventArgs e) {
			RaiseCalculateDocumentVariable(e);
		}
		protected internal virtual void InheritServicesForExport(DocumentModel documentModel) {
			InheritDataServices(documentModel);
			InheritUriProviderService(documentModel);
		}
		protected internal virtual void InheritServicesForImport(DocumentModel documentModel) {
			InheritService<IUriStreamService>(documentModel);
		}
		protected internal virtual void UpdateTableOfContents() {
		}
		public virtual void BeginSetContentForExport() {
			ModelForExport = true;
			BeginSetContent();
		}
		public virtual void EndSetContentForExport(DocumentModelChangeType changeType, bool updateFields) {
			EndSetContent(changeType, updateFields, null);
		}
		public virtual void EndSetContent(DocumentModelChangeType changeType, bool updateFields, FieldUpdateOnLoadOptions updateOptions) {
			EndSetContent(changeType, updateFields, false, updateOptions);
		}
		public virtual void EndSetContent(DocumentModelChangeType changeType, bool updateFields, bool pasteFromIe, FieldUpdateOnLoadOptions updateOptions) {
			EndSetContent(changeType, updateFields, pasteFromIe, updateOptions, null);
		}
		public virtual void EndSetContent(DocumentModelChangeType changeType, bool updateFields, bool pasteFromIe, FieldUpdateOnLoadOptions updateOptions, Action afterPieceTablesEndSetContentAction) {
			foreach (PieceTable pieceTable in GetPieceTables(true)) {
				if (DocumentCapabilities.Fields == DocumentCapability.Disabled)
					pieceTable.ProcessFieldsRecursive(null, (DevExpress.XtraRichEdit.Model.Field field) => { return false; });
				pieceTable.OnEndSetContent();
			}
			if (afterPieceTablesEndSetContentAction != null)
				afterPieceTablesEndSetContentAction();
			TableConditionalFormattingController.ResetTablesCachedProperties(this);
			if (updateFields) {
				BeginFieldsUpdate();
				UpdateFields(pasteFromIe ? UpdateFieldOperationType.PasteFromIE : UpdateFieldOperationType.Load, updateOptions);
				EndFieldsUpdate();
			}
			if (CommentOptions.ShowAllAuthors) {
				CommentOptions.BeginUpdate();
				CaculateVisibleAuthors();
				CommentOptions.CancelUpdate();
			}
			SwitchToNormalHistory(true);
			SwitchToNormalSelection();
			MainPieceTable.ApplyChanges(changeType, RunIndex.Zero, new RunIndex(MainPieceTable.Runs.Count - 1));
			Selection.End = DocumentLogPosition.Zero;
			Selection.Start = DocumentLogPosition.Zero;
			Selection.SetStartCell(DocumentLogPosition.Zero);
			DeferredChanges.IsSetContentMode = false;
			EndUpdate();
		}
		internal void CaculateVisibleAuthors() {
			List<string> authors = GetAuthors();
			if (authors.Count > 0) {
				CommentOptions.BeginUpdate();
				CommentOptions.VisibleAuthors.Clear();
				foreach (string author in authors) {
					CommentOptions.VisibleAuthors.Add(author);
				}
				CommentOptions.EndUpdate();
			}
		}
		protected void BeginFieldsUpdate() {
			DeferredChanges.IsSetContentMode = false;
			this.suppressFieldsChangeNotification = true;
		}
		protected void EndFieldsUpdate() {
			this.suppressFieldsChangeNotification = false;
			DeferredChanges.IsSetContentMode = true;
		}
		protected internal virtual void PreprocessContentBeforeInsert(PieceTable destination, DocumentLogPosition pos) {
			this.InheritDataServices(destination.DocumentModel);
		}
		public override void PreprocessContentBeforeExport(DocumentFormat format) {
			List<PieceTable> pieceTables = GetPieceTables(false);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++) {
				if (this.DocumentCapabilities.Fields == DocumentCapability.Disabled)
					pieceTables[i].ProcessFieldsRecursive(null, (Field field) => { return false; });
				pieceTables[i].PreprocessContentBeforeExport(format);
			}
		}
		protected internal virtual void ClearDocumentCore(bool addDefaultsList, bool changeDefaultTableStyle) {
			UnsubscribeCharacterPropertiesEvents();
			UnsubscribeDocumentPropertiesEvents();
			UnsubscribeProtectionPropertiesEvents();
			IThreadPoolService threadPoolService = GetService<IThreadPoolService>();
			if (threadPoolService != null)
				threadPoolService.Reset();
			this.pieceTable.Clear();
			this.abstractNumberingListIdProvider.Reset();
			this.numberingListIdProvider.Reset();
			this.abstractNumberingLists.Clear();
			this.numberingLists.Clear();
			this.sections.Clear();
			this.headers.Clear();
			this.footers.Clear();
			this.variables.Clear();
			this.footNotes.Clear();
			this.endNotes.Clear();
			this.ResetMerging();
			if (ImageCache != null)
				ImageCache.Clear();
			this.emptyBottomBorder = null;
			this.emptyTopBorder = null;
			this.cache = new DocumentCache();
			this.cache.Initialize(this);
			this.underlineRepository = new UnderlineRepository();
			this.borderLineRepository = new BorderLineRepository();
			this.strikeoutRepository = new StrikeoutRepository();
			this.numberDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			this.numberGroupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
			this.characterStyles = new CharacterStyleCollection(this);
			this.paragraphStyles = new ParagraphStyleCollection(this);
			this.styleLinkManager = new StyleLinkManager(this);
			this.tableStyles = new TableStyleCollection(this, changeDefaultTableStyle);
			this.tableCellStyles = new TableCellStyleCollection(this, changeDefaultTableStyle);
			this.numberingListStyles = new NumberingListStyleCollection(this);
			InitializeDefaultStyles();
			this.defaultParagraphProperties = new ParagraphProperties(this);
			this.defaultCharacterProperties = new CharacterProperties(this);
			this.defaultTableProperties = new TableProperties(MainPieceTable);
			this.defaultTableRowProperties = new TableRowProperties(MainPieceTable);
			this.defaultTableCellProperties = new TableCellProperties(MainPieceTable, this);
			InitializeDefaultProperties();
			this.documentProperties = new DocumentProperties(this);
			this.protectionProperties = new DocumentProtectionProperties(this);
			this.extendedDocumentProperties = new ExtendedDocumentProperties();
			this.htmlSettings = new WebSettings();
			this.commentColorer = new CommentColorer(this.CommentOptions);
			UnsafeEditor.InsertFirstParagraph(MainPieceTable);
			UnsafeEditor.InsertFirstSection();
			this.lineNumberRun = new LineNumberCommonRun(MainPieceTable.Paragraphs.First);
			int styleIndex = CharacterStyles.GetStyleIndexByName(CharacterStyleCollection.LineNumberingStyleName);
			if (styleIndex < 0)
				styleIndex = CharacterStyleCollection.EmptyCharacterStyleIndex;
			this.lineNumberRun.SetCharacterStyleIndexCore(styleIndex);
			SubscribeCharacterPropertiesEvents();
			SubscribeParagraphPropertiesEvents();
			SubscribeDocumentPropertiesEvents();
			SubscribeProtectionPropertiesEvents();
		}
		protected virtual void InitializeDefaultProperties() {
			this.defaultParagraphProperties.SetIndexInitial(ParagraphFormattingCache.RootParagraphFormattingIndex);
			this.defaultCharacterProperties.SetIndexInitial(CharacterFormattingCache.RootCharacterFormattingIndex);
			this.defaultTableProperties.SetIndexInitial(TablePropertiesOptionsCache.RootTableFormattingOptionsItem);
			this.defaultTableRowProperties.SetIndexInitial(TableRowPropertiesOptionsCache.RootRowPropertiesOptionsItem);
			this.defaultTableCellProperties.SetIndexInitial(TableCellPropertiesOptionsCache.RootCellPropertiesOptionsItem);
		}
		protected internal virtual void InitializeDefaultStyles() {
		}
		#region IBatchUpdateHandler implementation
		public override void OnFirstBeginUpdate() {
			RaiseBeginDocumentUpdate();
			CreateDeferredChanges();
			History.BeginTransaction();
		}
		public override void OnBeginUpdate() {
			History.BeginTransaction();
			Selection.BeginUpdate();
		}
		public override void OnEndUpdate() {
			Selection.EndUpdate();
			History.EndTransaction();
		}
		public override void OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		public override void OnCancelUpdate() {
			Selection.CancelUpdate();
			History.EndTransaction();
		}
		public override void OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		protected internal virtual void CreateDeferredChanges() {
			this.deferredChanges = new DocumentModelDeferredChanges(this); ;
		}
		protected internal virtual void DeleteDeferredChanges() {
			this.deferredChanges = null;
		}
		protected internal virtual void OnLastEndUpdateCore() {
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0) {
				if (ShouldFormatSpecialEmptyParagraph()) {
					specialEmptyParagraphRunIndex = Selection.Interval.Start.RunIndex;
					ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, specialEmptyParagraphRunIndex, specialEmptyParagraphRunIndex);
					isLastSelectionInEmptySpecialParagraph = true;
				}
				else if (isLastSelectionInEmptySpecialParagraph) {
					ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, specialEmptyParagraphRunIndex, specialEmptyParagraphRunIndex);
					isLastSelectionInEmptySpecialParagraph = false;
				}
			}
			Dictionary<PieceTable, SortedRunIndexCollection> deferredChangesRunIndicesForSplit = DeferredChanges.RunIndicesForSplit;
			foreach (PieceTable pieceTable in deferredChangesRunIndicesForSplit.Keys)
				pieceTable.PerformTextRunSplit(deferredChangesRunIndicesForSplit[pieceTable]);
			bool explicitRaiseContentChanged = (DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseContentChanged) != 0;
			if (History.HasChangesInCurrentTransaction() && (DeferredChanges.ChangeActions & DocumentModelChangeActions.SuppressRaiseContentChangedCalculationByCurrentTransactionChanges) == 0)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.RaiseContentChanged;
			RaiseBeforeEndDocumentUpdate(); 
			if (!DeferredChanges.SuppressSyntaxHighlight && !SyntaxHighlightSuspended && (DeferredChanges.ChangeActions & (DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ForceSyntaxHighlight)) != 0) {
				if (explicitRaiseContentChanged || ShouldPerformSyntaxHighlight()) {
					try {
						History.BeginSyntaxHighlight();
						PerformSyntaxHighlight((DeferredChanges.ChangeActions & DocumentModelChangeActions.ForceSyntaxHighlight) != 0);
					}
					finally {
						History.EndSyntaxHighlight();
					}
				}
			}
			History.EndTransaction(); 
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.Fields) != 0) {
				FireFieldsChanged();
			}
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ApplyAutoCorrect) != 0)
				PerformAutoCorrect();
#if DEBUGTEST
			if (!DisableCheckIntegrityOnLastEndUpdate)
				CheckIntegrity();
#endif
			ResetParagraphsOnLastEndUpdate();
			NotifyContentChanged();
			RaiseEndDocumentUpdate();
			RaiseAfterEndDocumentUpdate();
			if (!DeferredChanges.SuppressClearOutdatedSelectionItems)
				Selection.ClearOutdatedItems();
			DeleteDeferredChanges();
		}
		bool ShouldPerformSyntaxHighlight() {
			if (History.Transaction == null || History.Transaction.Count == 0)
				if (((History is DisabledHistory) && History.HasChangesInCurrentTransaction()))
					return true;
				else
					return false;
			CompositeHistoryItem transaction = History.Transaction;
			for (int i = 0; i < transaction.Count; i++) {
				HistoryItem item = transaction[i];
				if (!(item is DeleteCustomMarkHistoryItem) && !(item is InsertCustomMarkHistoryItem))
					return true;
			}
			return false;
		}
		protected internal virtual void ResetParagraphsOnLastEndUpdate() {
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0)
				ResetParagraphs(); 
			else if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetPrimaryLayout) != 0 || (DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetSecondaryLayout) != 0) {
				MainPieceTable.ResetParagraphs(DeferredChanges.ChangeStart.ParagraphIndex, DeferredChanges.ChangeEnd.ParagraphIndex);
				IList<PieceTable> additionalChangedPieceTables = DeferredChanges.AdditionalChangedPieceTables;
				if (additionalChangedPieceTables != null)
					ResetAdditionalPieceTablesParagraphs(additionalChangedPieceTables);
			}
		}
		public virtual void ForceSyntaxHighlight() {
			BeginUpdate();
			DeferredChanges.ChangeActions |= DocumentModelChangeActions.ForceSyntaxHighlight;
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseContentChanged) == 0)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.SuppressRaiseContentChangedCalculationByCurrentTransactionChanges;
			EndUpdate();
		}
		protected internal virtual void NotifyContentChanged() {
			IContentChangedNotificationService service = GetService<IContentChangedNotificationService>();
			if (service != null)
				service.NotifyContentChanged();
		}
		protected internal virtual void PerformSyntaxHighlight(bool forced) {
			UnsubscribeSelectionEvents();
			try {
				ISyntaxHighlightService service = GetService<ISyntaxHighlightService>();
				if (service != null) {
					if (forced)
						service.ForceExecute();
					else
						service.Execute();
				}
			}
			finally {
				SubscribeSelectionEvents();
			}
		}
		protected internal virtual void PerformAutoCorrect() {
			IAutoCorrectService service = GetService<IAutoCorrectService>();
			if (service == null)
				return;
			AutoCorrectInfo info = service.CalculateAutoCorrectInfo();
			if (info != null)
				service.ApplyAutoCorrectInfo(info);
		}
		protected internal virtual void ApplyChanges(PieceTable pieceTable, DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			DocumentModelChangeActions actions = DocumentModelChangeActionsCalculator.CalculateChangeActions(changeType);
			ApplyChangesCore(pieceTable, actions, startRunIndex, endRunIndex);
		}
		protected internal virtual void ApplyChangesCore(PieceTable pieceTable, DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (actions == DocumentModelChangeActions.None)
				return;
			Debug.Assert(IsUpdateLocked || BatchUpdateHelper.OverlappedTransaction);
			this.DeferredChanges.ApplyChanges(pieceTable, actions, startRunIndex, endRunIndex);
		}
		protected internal virtual void InvalidateDocumentLayout() {
			Debug.Assert(IsUpdateLocked);
			DocumentModelChangeActions actions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
			ActivePieceTable.ApplyChangesCore(actions, RunIndex.Zero, RunIndex.MaxValue);
		}
		protected internal virtual void InvalidateDocumentLayoutFrom(RunIndex runIndex) {
			Debug.Assert(IsUpdateLocked);
			DocumentModelChangeActions actions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
			ActivePieceTable.ApplyChangesCore(actions, runIndex, RunIndex.MaxValue);
		}
		protected internal virtual void OnSectionInserted(SectionIndex sectionIndex) {
			Debug.Assert(IsUpdateLocked);
			RaiseSectionInserted(sectionIndex);
		}
		protected internal virtual void OnSectionRemoved(SectionIndex sectionIndex) {
			Debug.Assert(IsUpdateLocked);
			RaiseSectionRemoved(sectionIndex);
		}
		protected internal virtual void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool IsParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(DeferredChanges, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, IsParagraphMerged, actualParagraphIndex, historyNotificationId);
				if (!SuppressFieldsChangeNotification)
					DocumentModelStructureChangedNotifier.NotifyParagraphInserted(InternalAPI, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, IsParagraphMerged, actualParagraphIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(Selection, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, IsParagraphMerged, actualParagraphIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(SearchContext, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, IsParagraphMerged, actualParagraphIndex, historyNotificationId);
			}
			if (pieceTable.IsMain) {
				Debug.Assert(sectionIndex >= new SectionIndex(0));
				RecalcSectionIndices(sectionIndex, 1);
			}
		}
		protected internal virtual void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(DeferredChanges, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				if (!SuppressFieldsChangeNotification)
					DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(InternalAPI, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(Selection, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(SearchContext, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
			if (pieceTable.IsMain) {
				Debug.Assert(sectionIndex >= new SectionIndex(0));
				RecalcSectionIndices(sectionIndex, -1);
			}
		}
		protected internal virtual void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(DeferredChanges, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				if (!SuppressFieldsChangeNotification)
					DocumentModelStructureChangedNotifier.NotifyParagraphMerged(InternalAPI, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(Selection, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(SearchContext, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
			if (pieceTable.IsMain) {
				Debug.Assert(sectionIndex >= new SectionIndex(0));
				RecalcSectionIndices(sectionIndex, -1);
			}
		}
		protected internal virtual void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Debug.Assert(IsUpdateLocked);
			Debug.Assert(length > 0);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunInserted(DeferredChanges, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunInserted(InternalAPI, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunInserted(Selection, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunInserted(SearchContext, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			}
		}
		protected internal virtual void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			Debug.Assert(length > 0);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(DeferredChanges, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(InternalAPI, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(Selection, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(SearchContext, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			}
		}
		protected internal virtual void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunMerged(DeferredChanges, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunMerged(InternalAPI, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunMerged(Selection, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunMerged(SearchContext, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			}
		}
		protected internal virtual void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(DeferredChanges, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(InternalAPI, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(Selection, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(SearchContext, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			}
		}
		protected internal virtual void OnBeginMultipleRunSplit(PieceTable pieceTable) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyBeginMultipleRunSplit(Selection, pieceTable);
			}
		}
		protected internal virtual void OnEndMultipleRunSplit(PieceTable pieceTable) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyEndMultipleRunSplit(Selection, pieceTable);
			}
		}
		protected internal virtual void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			Debug.Assert(splitOffset > 0);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunSplit(DeferredChanges, pieceTable, paragraphIndex, runIndex, splitOffset);
				DocumentModelStructureChangedNotifier.NotifyRunSplit(InternalAPI, pieceTable, paragraphIndex, runIndex, splitOffset);
				DocumentModelStructureChangedNotifier.NotifyRunSplit(Selection, pieceTable, paragraphIndex, runIndex, splitOffset);
				DocumentModelStructureChangedNotifier.NotifyRunSplit(SearchContext, pieceTable, paragraphIndex, runIndex, splitOffset);
			}
		}
		protected internal virtual void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Debug.Assert(IsUpdateLockedOrOverlapped);
			Debug.Assert(splitOffset > 0);
			Debug.Assert(tailRunLength > 0);
			if (!DeferredChanges.IsSetContentMode) {
				DocumentModelStructureChangedNotifier.NotifyRunJoined(DeferredChanges, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunJoined(InternalAPI, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunJoined(Selection, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunJoined(SearchContext, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			}
		}
		protected internal virtual void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode && !SuppressFieldsChangeNotification) {
				DocumentModelStructureChangedNotifier.NotifyFieldInserted(InternalAPI, pieceTable, fieldIndex);
			}
		}
		protected internal virtual void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(IsUpdateLocked);
			if (!DeferredChanges.IsSetContentMode && !SuppressFieldsChangeNotification) {
				DocumentModelStructureChangedNotifier.NotifyFieldRemoved(InternalAPI, pieceTable, fieldIndex);
			}
		}
		internal void RecalcSectionIndices(SectionIndex from, int deltaIndex) {
			Sections[from].LastParagraphIndex += deltaIndex;
			RecalcSectionIndicesCore(from + 1, deltaIndex);
		}
		internal void RecalcSectionIndicesCore(SectionIndex from, int deltaIndex) {
			SectionIndex count = new SectionIndex(Sections.Count);
			for (SectionIndex i = from; i < count; i++) {
				Section section = Sections[i];
				section.FirstParagraphIndex += deltaIndex;
				section.LastParagraphIndex += deltaIndex;
			}
		}
		internal Section GetActiveSectionBySelectionEnd() {
			SectionHeaderFooterBase headerFooter = Selection.PieceTable.ContentType as SectionHeaderFooterBase;
			if (headerFooter != null) {
				Section section = GetActiveSection();
				Guard.ArgumentNotNull(section, "section");
				return section;
			}
			SectionIndex sectionIndex = FindSectionIndex(Selection.End);
			if (sectionIndex < new SectionIndex(0))
				return null;
			return Sections[sectionIndex];
		}
		internal SectionIndex FindSectionIndex(DocumentLogPosition logPosition) {
			return FindSectionIndex(logPosition, true);
		}
		internal SectionIndex FindSectionIndex(DocumentLogPosition logPosition, bool strictSearch) {
			SectionAndLogPositionComparable predicate = new SectionAndLogPositionComparable(logPosition);
			SectionIndex result = OfficeAlgorithms.BinarySearch(Sections, predicate);
			if (result < new SectionIndex(0)) {
				if (result == new SectionIndex(~Sections.Count))
					return new SectionIndex(Sections.Count - 1);
				else {
					Exceptions.ThrowArgumentException("logPosition", logPosition);
					return new SectionIndex(-1);
				}
			}
			else
				return result;
		}
		public void InsertSection(DocumentLogPosition logPosition) {
			InsertSection(logPosition, false);
		}
		public void InsertSection(DocumentLogPosition logPosition, bool forceVisible) {
			DocumentModelInsertSectionAtLogPositionCommand command = new DocumentModelInsertSectionAtLogPositionCommand(this, logPosition, forceVisible);
			command.Execute();
		}
		internal void ApplySectionFormatting(DocumentLogPosition logPositionStart, int length, SectionPropertyModifierBase modifier) {
			if (logPositionStart < DocumentLogPosition.MinValue)
				Exceptions.ThrowArgumentException("logPositionStart", logPositionStart);
			if (length <= 0)
				Exceptions.ThrowArgumentException("length", length);
			ApplySectionFormattingCore(logPositionStart, length, modifier);
		}
		protected internal void ApplySectionFormattingCore(DocumentLogPosition logPositionStart, int length, SectionPropertyModifierBase modifier) {
			using (HistoryTransaction transaction = new HistoryTransaction(History)) {
				SectionIndex lastSectionIndex = FindSectionIndex(logPositionStart + length - 1);
				for (SectionIndex i = FindSectionIndex(logPositionStart); i <= lastSectionIndex; i++)
					modifier.ModifySection(sections[i], i);
			}
		}
		internal Nullable<T> ObtainSectionsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, SectionPropertyModifier<T> modifier) where T : struct {
			SectionIndex firstSectionIndex = FindSectionIndex(logPositionStart);
			SectionIndex lastSectionIndex = FindSectionIndex(logPositionStart + length - 1);
			T value = modifier.GetSectionPropertyValue(Sections[firstSectionIndex]);
			for (SectionIndex i = firstSectionIndex + 1; i <= lastSectionIndex; i++) {
				T sectionValue = modifier.GetSectionPropertyValue(Sections[i]);
				if (!sectionValue.Equals(value))
					return null;
			}
			return value;
		}
		internal T ObtainMergedSectionsPropertyValue<T>(DocumentLogPosition logPositionStart, int length, MergedSectionPropertyModifier<T> modifier) {
			SectionIndex firstSectionIndex = FindSectionIndex(logPositionStart);
			SectionIndex lastSectionIndex = FindSectionIndex(logPositionStart + length - 1);
			T value = modifier.GetSectionPropertyValue(Sections[firstSectionIndex]);
			for (SectionIndex i = firstSectionIndex + 1; i <= lastSectionIndex; i++) {
				T sectionValue = modifier.GetSectionPropertyValue(Sections[i]);
				value = modifier.Merge(value, sectionValue);
			}
			return value;
		}
		protected internal virtual void DeleteDefaultNumberingList(NumberingListCollection numberingLists) {
			numberingLists.Remove(numberingLists.First);
		}
		protected internal virtual NumberingListIndex GetNumberingListIndex(DocumentModel target, NumberingListIndex sourceListIndex) {
			AbstractNumberingListIndex targetAbstractNumberingListIndex = GetAbstractNumberingListIndex(target, sourceListIndex);
			NumberingList sourceNumberingList = NumberingLists[sourceListIndex];
			for (NumberingListIndex i = new NumberingListIndex(0); i < new NumberingListIndex(target.NumberingLists.Count); i++) {
				NumberingList targetNumberingList = target.NumberingLists[i];
				if (AreNumberingListsIdentical(sourceNumberingList, targetNumberingList))
					return i;
			}
			return CreateNumberingList(target, sourceNumberingList, targetAbstractNumberingListIndex);
		}
		bool AreNumberingListsIdentical(NumberingList sourceNumberingList, NumberingList targetNumberingList) {
			int sourceListId = sourceNumberingList.AbstractNumberingList.Id;
			int targetListId = targetNumberingList.AbstractNumberingList.Id;
			bool result = sourceListId == targetListId && CheckIsOverrideEquals(targetNumberingList, sourceNumberingList);
			if (targetNumberingList.DocumentModel.ModelForExport)
				result = result && targetNumberingList.Id == sourceNumberingList.Id;
			return result;
		}
		NumberingListIndex CreateNumberingList(DocumentModel target, NumberingList sourceNumberingList, AbstractNumberingListIndex targetAbstractNumberingListIndex) {
			NumberingList numberingList = new NumberingList(target, targetAbstractNumberingListIndex);
			numberingList.CopyFrom(sourceNumberingList);
			target.AddNumberingListUsingHistory(numberingList);
			int numberingListId = target.ModelForExport ? sourceNumberingList.Id : target.NumberingLists.Count;
			numberingList.SetId(numberingListId);
			return new NumberingListIndex(target.NumberingLists.Count - 1);
		}
		protected virtual bool CheckIsOverrideEquals(NumberingList targetNumberingList, NumberingList sourceNumberingList) {
			ListLevelCollection<IOverrideListLevel> sourceLevels = sourceNumberingList.Levels;
			ListLevelCollection<IOverrideListLevel> targetLevels = targetNumberingList.Levels;
			if (sourceLevels.Count != targetLevels.Count)
				return false;
			int count = sourceNumberingList.Levels.Count;
			for (int i = 0; i < count; i++) {
				if ((sourceLevels[i].OverrideStart != targetLevels[i].OverrideStart) || (sourceLevels[i].NewStart != targetLevels[i].NewStart))
					return false;
			}
			return true;
		}
		protected internal virtual AbstractNumberingListIndex GetAbstractNumberingListIndex(DocumentModel target, NumberingListIndex sourceListIndex) {
#if DEBUGTEST || DEBUG
			Debug.Assert(sourceListIndex != NumberingListIndex.NoNumberingList);
#endif
			int numberingListId = NumberingLists[sourceListIndex].AbstractNumberingList.Id;
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < new AbstractNumberingListIndex(target.AbstractNumberingLists.Count); i++) {
				AbstractNumberingList targetList = target.AbstractNumberingLists[i];
				if (numberingListId == targetList.Id) {
					if (targetList.Deleted)
						CreateAbstractNumberingList(target, targetList, numberingListId, sourceListIndex);
					return i;
				}
			}
			CreateAbstractNumberingList(target, null, numberingListId, sourceListIndex);
			return new AbstractNumberingListIndex(target.AbstractNumberingLists.Count - 1);
		}
		protected internal virtual void CreateAbstractNumberingList(DocumentModel target, AbstractNumberingList targetList, int numberingListId, NumberingListIndex sourceListIndex) {
			if (targetList == null)
				targetList = new AbstractNumberingList(target);
			target.AddAbstractNumberingListUsingHistory(targetList);
			targetList.CopyFrom(NumberingLists[sourceListIndex].AbstractNumberingList);
			targetList.SetId(numberingListId);
		}
		protected internal virtual void AddAbstractNumberingListUsingHistory(AbstractNumberingList abstractList) {
			AddAbstractNumberingListHistoryItem historyItem = new AddAbstractNumberingListHistoryItem(ActivePieceTable);
			historyItem.AbstractList = abstractList;
			History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal virtual void AddNumberingListUsingHistory(NumberingList numberingList) {
			AddNumberingListHistoryItem historyItem = new AddNumberingListHistoryItem(ActivePieceTable);
			historyItem.NumberingList = numberingList;
			History.Add(historyItem);
			historyItem.Execute();
		}
		#region ReplaceText
		#endregion
		internal string GetSelectionText() {
			if (Selection.Length == 0)
				return String.Empty;
			DocumentModelPosition start = Selection.Interval.NormalizedStart;
			DocumentModelPosition end = Selection.Interval.NormalizedEnd;
			return Selection.PieceTable.GetFilteredPlainText(start, end, Selection.PieceTable.VisibleTextFilter.IsRunVisible);
		}
		#region ICharacterPropertiesContainer Members
		PieceTable ICharacterPropertiesContainer.PieceTable { get { return MainPieceTable; } }
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
			ResetDocumentFormattingCaches(ResetFormattingCacheType.Character);
		}
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(MainPieceTable, DefaultCharacterProperties);
		}
		#endregion
		#region IParagraphPropertiesContainer Members
		PieceTable IParagraphPropertiesContainer.PieceTable { get { return MainPieceTable; } }
		void IParagraphPropertiesContainer.OnParagraphPropertiesChanged() {
			ResetDocumentFormattingCaches(ResetFormattingCacheType.Paragraph);
		}
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IParagraphPropertiesContainer.CreateParagraphPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(MainPieceTable, DefaultParagraphProperties);
		}
		#endregion
		protected internal virtual void ResetDocumentFormattingCaches(ResetFormattingCacheType resetFromattingCacheType) {
			List<PieceTable> pieceTables = GetPieceTables(true);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++)
				pieceTables[i].ResetFormattingCaches(resetFromattingCacheType);
			ClearModelCachedIndices(resetFromattingCacheType);
		}
		void ClearModelCachedIndices(ResetFormattingCacheType resetFromattingCacheType) {
			if ((resetFromattingCacheType & ResetFormattingCacheType.Paragraph) != 0)
				ParagraphStyles.ResetCachedIndices(resetFromattingCacheType);
			if ((resetFromattingCacheType & ResetFormattingCacheType.Character) != 0)
				CharacterStyles.ResetCachedIndices(resetFromattingCacheType);
			TableStyles.ResetCachedIndices(resetFromattingCacheType);
			LineNumberRun.ResetCachedIndices(resetFromattingCacheType);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == DefaultTableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
			OnBeginMultipleRunSplit(pieceTable);
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
			OnEndMultipleRunSplit(pieceTable);
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			OnFieldInserted(pieceTable, fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			OnFieldRemoved(pieceTable, fieldIndex);
		}
		#endregion
		internal MergeFieldName[] GetDatabaseFieldNames() {
			MergeFieldName[] result = { };
			if (MailMergeDataController.IsReady) {
				result = MailMergeDataController.GetColumnNames();
				Array.Sort(result);
			}
			return result;
		}
		internal MergeFieldName[] GetAddressFieldNames() {
			List<string> names = new List<string>();
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapUniqueId));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapTitle));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapFirstName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapMiddleName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldLastName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapSuffix));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapNickName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapJobTitle));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapCompany));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapAddress1));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapAddress2));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapCity));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapState));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPostalCode));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapCountry));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapBusinessPhone));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapBusinessFax));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapHomePhone));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapHomeFax));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapEMailAddress));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapWebPage));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPartnerTitle));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPartnerFirstName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPartnerMiddleName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPartnerLastName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPartnerNickName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPhoneticGuideFirstName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapPhoneticGuideLastName));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapAddress3));
			names.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FieldMapDepartment));
			int count = names.Count;
			MergeFieldName[] result = new MergeFieldName[names.Count];
			for (int i = 0; i < count; i++)
				result[i] = new MergeFieldName(names[i]);
			return result;
		}	   
		protected internal virtual NumberingListCountersCalculator CreateNumberingListCountersCalculator(AbstractNumberingList list) {
			return new NumberingListCountersCalculator(list);
		}		
		public override void ResetMerging() {
			lastInsertedRunInfo.Reset(null);
		}
		public LastInsertedRunInfo GetLastInsertedRunInfo(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(pieceTable, lastInsertedRunInfo.PieceTable))
				lastInsertedRunInfo.Reset(pieceTable);
			return lastInsertedRunInfo;
		}
		public LastInsertedInlinePictureRunInfo GetLastInsertedInlinePictureRunInfo(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(pieceTable, lastInsertedInlinePictureRunInfo.PieceTable))
				lastInsertedInlinePictureRunInfo.Reset(pieceTable);
			return lastInsertedInlinePictureRunInfo;
		}
		public LastInsertedFloatingObjectAnchorRunInfo GetLastInsertedFloatingObjectAnchorRunInfo(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(pieceTable, lastInsertedFloatingObjectAnchorRunInfo.PieceTable))
				lastInsertedFloatingObjectAnchorRunInfo.Reset(pieceTable);
			return lastInsertedFloatingObjectAnchorRunInfo;
		}
		public LastInsertedSeparatorRunInfo GetLastInsertedSeparatorRunInfo(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(pieceTable, lastInsertedSeparatorRunInfo.PieceTable))
				lastInsertedSeparatorRunInfo.Reset(pieceTable);
			return lastInsertedSeparatorRunInfo;
		}
		internal int GetIdForNewPeaceTable() {
			return CountIdForPeaceTables++;
		}
		protected internal virtual void OnShowHiddenTextOptionsChanged() {
			BeginUpdate();
			try {
				bool showHiddenText = FormattingMarkVisibilityOptions.ShowHiddenText;
				foreach (PieceTable pieceTable in GetPieceTables(true))
					pieceTable.SetShowHiddenText(showHiddenText);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				MainPieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void RecreateVisibleTextFilter() {
			OnShowHiddenTextOptionsChanged();
		}
		protected internal virtual void OnGridLinesOptionsChanged() {
			OnLayoutTablesToExtendIntoMarginsOptionsChanged();
		}
		protected internal virtual void OnLayoutTablesToExtendIntoMarginsOptionsChanged() {
			BeginUpdate();
			try {
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				MainPieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnMatchHorizontalTableIndentsToTextEdgeOptionsChanged() {
			BeginUpdate();
			try {
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				MainPieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				EndUpdate();
			}
		}
		internal Section GetActiveSection() {
			if (activeSectionIndex < new SectionIndex(0))
				return null;
			else
				return Sections[activeSectionIndex];
		}
		internal SectionIndex GetActiveSectionIndex() {
			return activeSectionIndex;
		}
		public void SetActivePieceTable(PieceTable pieceTable, Section section) {
			BeginUpdate();
			try {
				SetActivePieceTableCore(pieceTable, section);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.ActivePieceTableChanged | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler | DocumentModelChangeActions.ValidateSelectionInterval | DocumentModelChangeActions.ResetSecondaryLayout;
				MainPieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetActivePieceTableCore(PieceTable pieceTable, Section section) {
			if (pieceTable.IsMain && section != null)
				Exceptions.ThrowArgumentException("section", section);
			if (!Object.ReferenceEquals(pieceTable, MainPieceTable) && pieceTable.IsHeaderFooter) {
				Guard.ArgumentNotNull(section, "section");
				ContentTypeBase contentType = pieceTable.ContentType;
				if (!Object.ReferenceEquals(section.InnerFirstPageHeader, contentType) &&
					!Object.ReferenceEquals(section.InnerOddPageHeader, contentType) &&
					!Object.ReferenceEquals(section.InnerEvenPageHeader, contentType) &&
					!Object.ReferenceEquals(section.InnerFirstPageFooter, contentType) &&
					!Object.ReferenceEquals(section.InnerOddPageFooter, contentType) &&
					!Object.ReferenceEquals(section.InnerEvenPageFooter, contentType))
					Exceptions.ThrowArgumentException("pieceTable", pieceTable);
			}
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.activePieceTable = pieceTable;
			if (section == null)
				this.activeSectionIndex = new SectionIndex(-1);
			else
				this.activeSectionIndex = Sections.IndexOf(section);
			SwitchToNormalSelection(); 
		}
		public bool ValidateActivePieceTable() {
			foreach (PieceTable pieceTable in GetPieceTables(false))
				if (Object.ReferenceEquals(ActivePieceTable, pieceTable))
					return true;
			return false;
		}
		protected internal bool CanEditSection(Section section) {
			DocumentLogPosition start = MainPieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition;
			Paragraph lastParagraph = MainPieceTable.Paragraphs[section.LastParagraphIndex];
			DocumentLogPosition end = lastParagraph.LogPosition + lastParagraph.Length;
			return MainPieceTable.CanEditRange(start, end);
		}
		public void EnforceDocumentProtection(string password) {
			BeginUpdate();
			try {
				PasswordHashCodeCalculator calculator = new PasswordHashCodeCalculator();
				ProtectionProperties.EnforceProtection = true;
				ProtectionProperties.ProtectionType = DocumentProtectionType.ReadOnly;
				if (String.IsNullOrEmpty(password)) {
					ProtectionProperties.HashAlgorithmType = HashAlgorithmType.None;
					ProtectionProperties.HashIterationCount = 0;
					ProtectionProperties.PasswordPrefix = null;
					ProtectionProperties.PasswordHash = null;
					ProtectionProperties.Word2003PasswordHash = null;
					ProtectionProperties.OpenOfficePasswordHash = null;
				}
				else {
					ProtectionProperties.HashAlgorithmType = HashAlgorithmType.Sha1;
					ProtectionProperties.HashIterationCount = 100000;
					ProtectionProperties.PasswordPrefix = calculator.GeneratePasswordPrefix(16);
					ProtectionProperties.PasswordHash = calculator.CalculatePasswordHash(password, ProtectionProperties.PasswordPrefix, ProtectionProperties.HashIterationCount, ProtectionProperties.HashAlgorithmType);
					ProtectionProperties.Word2003PasswordHash = calculator.CalculateLegacyPasswordHash(password);
					ProtectionProperties.OpenOfficePasswordHash = calculator.CalculateOpenOfficePasswordHash(password);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public bool RemoveDocumentProtection(string password) {
			if (!CheckDocumentProtectionPassword(password))
				return false;
			ForceRemoveDocumentProtection();
			return true;
		}
		public void ForceRemoveDocumentProtection() {
			BeginUpdate();
			try {
				ProtectionProperties.EnforceProtection = false;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual bool CheckDocumentProtectionPassword(string password) {
			PasswordHashCodeCalculator calculator = new PasswordHashCodeCalculator();
			DocumentFormat format = DocumentSaveOptions.CurrentFormat;
			if (format == DocumentFormat.Rtf)
				return CheckRtfDocumentProtectionPassword(calculator, password);
			else if (format == DocumentFormat.OpenDocument)
				return CheckOpenOfficeDocumentProtectionPassword(calculator, password);
			else if (format == DocumentFormat.OpenXml)
				return CheckOpenXmlDocumentProtectionPassword(calculator, password);
			else if (format == DocumentFormat.WordML)
				return CheckLegacyDocumentProtectionPassword(calculator, password);
			else
				return CheckDocumentProtectionPasswordCore(calculator, password);
		}
		protected internal virtual bool CheckPasswordHash(byte[] hash, byte[] expectedHash) {
			if (expectedHash == null || expectedHash.Length <= 0)
				return true;
			return PasswordHashCodeCalculator.CompareByteArrays(hash, expectedHash);
		}
		protected internal virtual bool CheckLegacyDocumentProtectionPassword(PasswordHashCodeCalculator calculator, string password) {
			byte[] hash = calculator.CalculateLegacyPasswordHash(password);
			return CheckPasswordHash(hash, ProtectionProperties.Word2003PasswordHash);
		}
		protected internal virtual bool CheckOpenXmlDocumentProtectionPassword(PasswordHashCodeCalculator calculator, string password) {
			byte[] hash = calculator.CalculatePasswordHash(password, ProtectionProperties.PasswordPrefix, ProtectionProperties.HashIterationCount, ProtectionProperties.HashAlgorithmType);
			return CheckPasswordHash(hash, ProtectionProperties.PasswordHash);
		}
		protected internal virtual bool CheckOpenOfficeDocumentProtectionPassword(PasswordHashCodeCalculator calculator, string password) {
			byte[] hash = calculator.CalculateOpenOfficePasswordHash(password);
			return CheckPasswordHash(hash, ProtectionProperties.OpenOfficePasswordHash);
		}
		protected internal virtual bool CheckRtfDocumentProtectionPassword(PasswordHashCodeCalculator calculator, string password) {
			if (ProtectionProperties.Word2003PasswordHash != null && ProtectionProperties.Word2003PasswordHash.Length > 0)
				return CheckLegacyDocumentProtectionPassword(calculator, password);
			else
				return CheckOpenXmlDocumentProtectionPassword(calculator, password);
		}
		protected internal virtual bool CheckDocumentProtectionPasswordCore(PasswordHashCodeCalculator calculator, string password) {
			if (ProtectionProperties.OpenOfficePasswordHash != null && ProtectionProperties.OpenOfficePasswordHash.Length > 0)
				return CheckOpenOfficeDocumentProtectionPassword(calculator, password);
			else
				return CheckRtfDocumentProtectionPassword(calculator, password);
		}
		protected internal virtual void ResetParagraphs() {
			foreach (PieceTable pieceTable in GetPieceTables(true))
				pieceTable.ResetParagraphs();
		}
		protected virtual void ResetAdditionalPieceTablesParagraphs(IList<PieceTable> pieceTable) {
			int count = pieceTable.Count;
			for (int i = 0; i < count; i++)
				pieceTable[i].ResetParagraphs(ParagraphIndex.Zero, ParagraphIndex.MaxValue);
		}
		public virtual void InheritDataServices(DocumentModel documentModel) {
			InheritService<IFieldDataService>(documentModel);
			InheritService<IMailMergeDataService>(documentModel);
			InheritService<IFieldCalculatorService>(documentModel);
			InheritService<IUriStreamService>(documentModel);
		}
		public virtual void InheritUriProviderService(DocumentModel documentModel) {
			InheritService<DevExpress.Office.Services.IUriProviderService>(documentModel);
			InheritService<IPdfLinkUpdater>(documentModel);
		}
		protected internal virtual void InheritService<T>(IServiceContainer sourceContainer) where T : class {
			RemoveService(typeof(T));
			T service = sourceContainer.GetService(typeof(T)) as T;
			if (service != null)
				AddService(typeof(T), service);
		}
#if DEBUGTEST
		bool disableCheckIntegrityOnLastEndUpdate;
		internal bool DisableCheckIntegrityOnLastEndUpdate { get { return disableCheckIntegrityOnLastEndUpdate; } set { disableCheckIntegrityOnLastEndUpdate = value; } }
#endif
		[System.Diagnostics.Conditional("DEBUG")]
		internal void CheckIntegrity() {
#if DEBUGTEST && !DXPORTABLE
			DevExpress.XtraRichEdit.Tests.DocumentModelIntegrityValidator.CheckDocumentModelIntegrity(this);
#endif
		}
#if DEBUGTEST
		public
#else
		protected internal 
#endif
		bool DisableCheckDocumentModelIntegrity { get; set; }
		#region GetTocStyle
		internal int GetTocStyle(int level) {
			return ParagraphStyles.GetStyleIndexByName(GetTocStyleName(level), true);
		}
		string GetTocStyleName(int level) {
			return String.Format("TOC {0}", level);
		}
		internal int CreateTocStyle(int level) {
			ParagraphStyle style = new ParagraphStyle(this, ParagraphStyles.DefaultItem, GetTocStyleName(level));
			int result = ParagraphStyles.Add(style);
			SetupTocStyle(style, level);
			return result;
		}
		void SetupTocStyle(ParagraphStyle style, int level) {
			style.ParagraphProperties.OutlineLevel = 0;
			style.ParagraphProperties.SpacingAfter = UnitConverter.PointsToModelUnits(5);
			style.ParagraphProperties.LeftIndent = 221 * (level - 1);
		}
		#endregion
		#region GetHeadingStyle
		internal int GetHeadingParagraphStyle(int level) {
			int paragraphStyleIndex = GetHeadingParagraphStyleCore(level);
			int characterStyleIndex = GetHeadingCharacterStyleCore(level);
			ParagraphStyle paragraphStyle = ParagraphStyles[paragraphStyleIndex];
			CharacterStyle characterStyle = CharacterStyles[characterStyleIndex];
			if (!paragraphStyle.HasLinkedStyle && !characterStyle.HasLinkedStyle)
				StyleLinkManager.CreateLink(paragraphStyle, characterStyle);
			return paragraphStyleIndex;
		}
		int GetHeadingParagraphStyleCore(int level) {
			string styleName = String.Format("Heading {0}", level);
			int index = ParagraphStyles.GetStyleIndexByName(styleName);
			if (index >= 0)
				return index;
			return CreateHeadingParagraphStyle(styleName, level);
		}
		int GetHeadingCharacterStyleCore(int level) {
			string styleName = String.Format("Heading {0} Char", level);
			int index = CharacterStyles.GetStyleIndexByName(styleName);
			if (index >= 0)
				return index;
			return CreateHeadingCharacterStyle(styleName, level);
		}
		int CreateHeadingParagraphStyle(string styleName, int level) {
			ParagraphStyle style = new ParagraphStyle(this, ParagraphStyles.DefaultItem, styleName);
			SetupHeadingParagraphStyle(style, level);
			style.NextParagraphStyle = ParagraphStyles.DefaultItem;
			return ParagraphStyles.Add(style);
		}
		int CreateHeadingCharacterStyle(string styleName, int level) {
			CharacterStyle style = new CharacterStyle(this, CharacterStyles.DefaultItem, styleName);
			SetupHeadingCharacterStyle(style, level);
			return CharacterStyles.Add(style);
		}
		void SetupHeadingParagraphStyle(ParagraphStyle style, int level) {
			style.ParagraphProperties.OutlineLevel = level;
			style.ParagraphProperties.SpacingAfter = 0;
			if (level == 1)
				style.ParagraphProperties.SpacingBefore = UnitConverter.PointsToModelUnits(24);
			else
				style.ParagraphProperties.SpacingBefore = UnitConverter.PointsToModelUnits(10);
			CharacterProperties characterProperties = style.CharacterProperties;
			switch (level) {
				case 1:
					characterProperties.DoubleFontSize = 28;
					characterProperties.FontBold = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x36, 0x5F, 0x91);
					break;
				case 2:
					characterProperties.DoubleFontSize = 26;
					characterProperties.FontBold = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x4F, 0x81, 0xBD);
					break;
				case 3:
					characterProperties.FontBold = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x4F, 0x81, 0xBD);
					break;
				case 4:
					characterProperties.FontBold = true;
					characterProperties.FontItalic = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x4F, 0x81, 0xBD);
					break;
				case 5:
					characterProperties.ForeColor = DXColor.FromArgb(0x24, 0x3F, 0x60);
					break;
				case 6:
					characterProperties.FontItalic = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x24, 0x3F, 0x60);
					break;
				case 7:
					characterProperties.FontItalic = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x40, 0x40, 0x40);
					break;
				case 8:
					characterProperties.DoubleFontSize = 20;
					characterProperties.ForeColor = DXColor.FromArgb(0x40, 0x40, 0x40);
					break;
				case 9:
					characterProperties.DoubleFontSize = 20;
					characterProperties.FontItalic = true;
					characterProperties.ForeColor = DXColor.FromArgb(0x40, 0x40, 0x40);
					break;
			}
		}
		void SetupHeadingCharacterStyle(CharacterStyle style, int level) {
			style.HiddenCore = true;
		}
		#endregion
		#region GetFootNoteReferenceCharacterStyle
		internal int GetFootNoteReferenceCharacterStyle() {
			string styleName = "footnote reference";
			int index = CharacterStyles.GetStyleIndexByName(styleName);
			if (index >= 0)
				return index;
			return CreateFootNoteReferenceCharacterStyle(styleName);
		}
		int CreateFootNoteReferenceCharacterStyle(string styleName) {
			CharacterStyle style = new CharacterStyle(this, CharacterStyles.DefaultItem, styleName);
			SetupFootNoteReferenceCharacterStyle(style);
			return CharacterStyles.Add(style);
		}
		void SetupFootNoteReferenceCharacterStyle(CharacterStyle style) {
			style.HiddenCore = true;
			style.CharacterProperties.Script = CharacterFormattingScript.Superscript;
		}
		#endregion
		internal void ResetTemporaryLayout() {
			IDocumentLayoutService service = GetService<IDocumentLayoutService>();
			if (service != null)
				service.ResetLayout();
		}
		protected internal virtual void ToggleAllFieldCodes(bool showCodes) {
			BeginUpdate();
			try {
				foreach (PieceTable pieceTable in GetPieceTables(false))
					pieceTable.ToggleAllFieldCodes(showCodes);
				ResetParagraphs();
			}
			finally {
				EndUpdate();
			}
		}
		public virtual DocumentModel CreateNew() {
			return new DocumentModel(documentFormatsDependencies, ScreenDpiX, ScreenDpiY);
		}
		public virtual DocumentModel CreateNew(bool modelForTextExport) {
			return CreateNew();
		}
		public virtual DocumentModel CreateNew(bool addDefaultLists, bool changeDefaultTableStyle) {
			return new DocumentModel(addDefaultLists, changeDefaultTableStyle, documentFormatsDependencies, ScreenDpiX, ScreenDpiY);
		}
		public virtual CopySectionOperation CreateCopySectionOperation(DocumentModelCopyManager copyManager) {
			return new CopySectionOperation(copyManager);
		}
		protected internal virtual DocumentModelCopyOptions CreateDocumentModelCopyOptions(DocumentLogPosition from, int length) {
			return new DocumentModelCopyOptions(from, length);
		}
		protected internal virtual void SetModelForExportCopyOptions(DocumentModelCopyOptions copyOptions) {
			copyOptions.DefaultPropertiesCopyOptions = DefaultPropertiesCopyOptions.Always;
			copyOptions.CopyDocumentVariables = true;
		}
		protected internal virtual DocumentModelCopyCommand CreateDocumentModelCopyCommand(PieceTable sourcePieceTable, DocumentModel target, DocumentModelCopyOptions options) {
			Debug.Assert(sourcePieceTable.DocumentModel == this);
			return CommandsCreationStrategy.CreateDocumentModelCopyCommand(sourcePieceTable, target, options);
		}
		protected internal virtual void EnsureImageLoadComplete() {
			List<PieceTable> pieceTables = GetPieceTables(false);
			pieceTables.ForEach(EnsureImageLoadComplete);
		}
		void EnsureImageLoadComplete(PieceTable pieceTable) {
			pieceTable.EnsureImageLoadComplete();
		}
		protected internal virtual void SuspendSyntaxHighlight() {
			syntaxHighlightSuspendCount++;
		}
		protected internal virtual void ResumeSyntaxHighlight() {
			if (syntaxHighlightSuspendCount > 0)
				syntaxHighlightSuspendCount--;
		}
		protected override ImportHelper<DocumentFormat, bool> CreateDocumentImportHelper() {
			return new DocumentImportHelper(this);
		}
		public override ExportHelper<DocumentFormat, bool> CreateDocumentExportHelper(DocumentFormat documentFormat) {
			return new DocumentExportHelper(this);
		}
		protected override IImportManagerService<DocumentFormat, bool> GetImportManagerService() {
			return GetService<IDocumentImportManagerService>();
		}
		protected override IExportManagerService<DocumentFormat, bool> GetExportManagerService() {
			return GetService<IDocumentExportManagerService>();
		}
		protected internal virtual void ResetUncheckedSpellIntervals() {
			List<PieceTable> pieceTables = GetPieceTables(false);
			foreach (PieceTable pieceTable in pieceTables)
				pieceTable.SpellCheckerManager.InitializeUncheckedInerval();
		}
		protected internal virtual void ResetSpellCheck(PieceTable pieceTable, RunIndex startRunIndex, RunIndex endRunIndex, bool formattingOnly) {
			DocumentModelChangeActions actions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSecondaryLayout;
			if (!formattingOnly)
				actions |= DocumentModelChangeActions.ResetUncheckedIntervals;
			BeginUpdate();
			try {
				pieceTable.ApplyChangesCore(actions, startRunIndex, endRunIndex);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void ResetSpellCheck(RunIndex startRunIndex, RunIndex endRunIndex, bool formattingOnly) {
			ResetSpellCheck(ActivePieceTable, startRunIndex, endRunIndex, formattingOnly);
		}
		protected internal virtual void ResetSpellCheck(RunIndex startRunIndex, RunIndex endRunIndex) {
			ResetSpellCheck(startRunIndex, endRunIndex, false);
		}
		protected internal virtual MailMergeDataMode GetMailMergeDataMode() {
			return MailMergeProperties.ViewMergedData ? MailMergeDataMode.ViewMergedData : MailMergeDataMode.None;
		}
		protected virtual DocumentModel GetFieldResultModel(Func<DocumentModel> documentModelCreator) {
			DocumentModel result = documentModelCreator();
			result.DisableCheckDocumentModelIntegrity = DisableCheckDocumentModelIntegrity;
			result.ModelForExport = ModelForExport;
			result.FieldOptions.CopyFrom(FieldOptions);
			result.FieldResultModel = true;
			return result;
		}
		public virtual DocumentModel GetFieldResultModel() {
			return GetFieldResultModel(CreateNew);
		}
		public int GetBoxEffectiveRotationAngle(Box box) {
			FloatingObjectBox floatingObjectBox = box as FloatingObjectBox;
			if (floatingObjectBox == null)
				return 0;
			return GetBoxEffectiveRotationAngle(floatingObjectBox);
		}
		public int GetBoxEffectiveRotationAngle(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null) {
				if (Object.ReferenceEquals(textBoxContent.TextBox.PieceTable, ActivePieceTable))
					return 0;
			}
			return run.Shape.Rotation;
		}
		public float GetBoxEffectiveRotationAngleInDegrees(Box box) {
			return UnitConverter.ModelUnitsToDegreeF(GetBoxEffectiveRotationAngle(box));
		}
		public float GetBoxEffectiveRotationAngleInDegrees(FloatingObjectBox box) {
			return UnitConverter.ModelUnitsToDegreeF(GetBoxEffectiveRotationAngle(box));
		}
		internal void NormalizeZOrder() {
			List<IZOrderedObject> floatingObjects = new List<IZOrderedObject>();
			GetPieceTables(false).ForEach(pieceTable => floatingObjects.AddRange(pieceTable.GetFloatingObjectList()));
			new ZOrderManager().Normalize(floatingObjects);
		}
		protected internal virtual void BeforeFloatingObjectDrop(DocumentLogPosition oldPosition, DocumentLogPosition newPosition, PieceTable pieceTable) { }
		protected internal virtual void ClearDataSources() { }
		internal List<string> GetAuthors() {
			List<string> result = new List<string>();
			int count = pieceTable.Comments.Count;
			for (int i = 0; i < count; i++) {
				string author = pieceTable.Comments[i].Author;
				if (!result.Contains(author))
					result.Add(author);
			}
			return result;
		}
		internal List<Comment> CreateVisibleComments() {
			List<Comment> result = new List<Comment>();
			CommentCollection allComments = MainPieceTable.Comments;
			int count = allComments.Count;
			for (int i = 0; i < count; i++) {
				Comment sourceComment = allComments[i];
				if (sourceComment.ParentComment == null) {
					if (CommentOptions.VisibleAuthors.Contains(sourceComment.Author))
						result.Add(sourceComment);
				}
				else {
					if (CommentOptions.VisibleAuthors.Contains(sourceComment.Author) && (CommentOptions.VisibleAuthors.Contains(sourceComment.ParentComment.Author)))
						result.Add(sourceComment);
				}
			}
			return result;
		}
	}
	#endregion
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	public enum ResetFormattingCacheType {
		Character = 1,
		Paragraph = 2,
		All = 0x7FFFFFFF
	}
	#region DocumentModelAccessor
	public class DocumentModelAccessor {
		readonly DocumentModel documentModel;
		internal DocumentModelAccessor(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		internal DocumentModel DocumentModel { get { return documentModel; } }
	}
	#endregion
	#region ResetDocumentLayoutType
	public enum DocumentLayoutResetType {
		None,
		SecondaryLayout, 
		AllPrimaryLayout, 
		PrimaryLayoutFormPosition 
	}
	#endregion
	#region HtmlSettings
	public class WebSettings {
		public int LeftMargin { get; set; }
		public int TopMargin { get; set; }
		public int RightMargin { get; set; }
		public int BottomMargin { get; set; }
		internal bool IsBodyMarginsSet() {
			return LeftMargin != 0 || TopMargin != 0 || RightMargin != 0 || BottomMargin != 0;
		}
	}
	#endregion
	public interface IDocumentExportersFactory {
		Export.Rtf.IRtfExporter CreateRtfExporter(DocumentModel modelForExport, RtfDocumentExporterOptions options);
		Export.PlainText.IPlainTextExporter CreatePlainTextExporter(DocumentModel modelForExport, PlainTextDocumentExporterOptions options);
		Export.Html.IHtmlExporter CreateHtmlExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options);
		Export.Mht.IMhtExporter CreateMhtExporter(DocumentModel modelForExport, MhtDocumentExporterOptions options);
		Export.OpenXml.IOpenXmlExporter CreateOpenXmlExporter(DocumentModel modelForExport, OpenXmlDocumentExporterOptions options);
		Export.OpenDocument.IOpenDocumentTextExporter CreateOpenDocumentTextExporter(DocumentModel DocumentModel, OpenDocumentExporterOptions options);
		Export.Xaml.IXamlExporter CreateXamlExporter(DocumentModel documentModel, XamlDocumentExporterOptions options);
		Export.WordML.IWordMLExporter CreateWordMLExporter(DocumentModel modelForExport, WordMLDocumentExporterOptions options);
		Export.Doc.IDocExporter CreateDocExporter(DocumentModel DocumentModel, DocDocumentExporterOptions options);
	}
	public interface IDocumentImportersFactory {
		IRtfImporter CreateRtfImporter(DocumentModel DocumentModel, RtfDocumentImporterOptions options);
		Import.Html.IHtmlImporter CreateHtmlImporter(DocumentModel DocumentModel, HtmlDocumentImporterOptions options);
		Import.Mht.IMhtImporter CreateMhtImporter(DocumentModel DocumentModel, MhtDocumentImporterOptions options);
		Import.OpenDocument.IOpenDocumentTextImporter CreateOpenDocumentTextImporter(DocumentModel DocumentModel, OpenDocumentImporterOptions options);
		Import.WordML.IWordMLImporter CreateWordMLImporter(DocumentModel DocumentModel, WordMLDocumentImporterOptions options);
		Import.Doc.IDocImporter CreateDocImporter(DocumentModel DocumentModel, DocDocumentImporterOptions options);
		Import.Xaml.IXamlImporter CreateXamlImporter(DocumentModel DocumentModel, XamlDocumentImporterOptions options);
		Import.OpenXml.IOpenXmlImporter CreateOpenXmlImporter(DocumentModel DocumentModel, OpenXmlDocumentImporterOptions options);
	}
}
namespace DevExpress.XtraRichEdit.Export.Rtf {
	public interface IRtfExporter {
		string Export();
		ChunkedStringBuilder ExportSaveMemory();
		bool LastParagraphRunNotSelected { get; set; }
		bool KeepFieldCodeViewState { get; set; }
	}
}
namespace DevExpress.XtraRichEdit.Export.Html {
	public interface IHtmlExporter {
		void Export(TextWriter writer);
		string Export();
	}
}
namespace DevExpress.XtraRichEdit.Export.Mht {
	public interface IMhtExporter {
		void Export(TextWriter writer);
		string Export();
	}
}
namespace DevExpress.XtraRichEdit.Export.WordML {
	public interface IWordMLExporter {
		string Export();
		void Export(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Export.OpenXml {
	public interface IOpenXmlExporter {
		void Export(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Export.PlainText {
	public interface IPlainTextExporter {
		string Export();
		string ExportPieceTableContent(PieceTable pieceTable);
		System.Text.StringBuilder ExportSaveMemory();
	}
}
namespace DevExpress.XtraRichEdit.Export.Xaml {
	public interface IXamlExporter {
		string Export();
		void Export(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Export.Doc {
	public interface IDocExporter {
		void Export(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	public interface IOpenDocumentTextExporter {
		void Export(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.Rtf {
	public interface IRtfImporter : IDisposable {
		void Import(Stream stream);
		bool UpdateFields { get; set; }
	}
}
namespace DevExpress.XtraRichEdit.Import.Html {
	public interface IHtmlImporter {
		HtmlDocumentImporterOptions Options { get; }
		System.Text.Encoding Encoding { get; set; }
		Model.DocumentLogPosition GetMarkPosition(string startFragmentCommentText);
		void RegisterCommentMarksToCollectPositions(params string[] marks);
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.Mht {
	public interface IMhtImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.WordML {
	public interface IWordMLImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	public interface IOpenXmlImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.PlainText {
	public interface IPlainTextImporter {
	}
}
namespace DevExpress.XtraRichEdit.Import.Xaml {
	public interface IXamlImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.Doc {
	public interface IDocImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	public interface IOpenDocumentTextImporter {
		void Import(Stream stream);
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	public class DocumentFormatsDependencies {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104")] 
		public static readonly DocumentFormatsDependencies Empty = new DocumentFormatsDependencies(null, null, new EmptyDocumentExportersFactory(), new EmptyDocumentImportersFactory());
		readonly IDocumentExportManagerService exportManagerService;
		readonly IDocumentImportManagerService importManagerService;
		readonly IDocumentExportersFactory exportersFactory;
		readonly IDocumentImportersFactory importersFactory;
		public DocumentFormatsDependencies(IDocumentExportManagerService exportManagerService, IDocumentImportManagerService importManagerService, IDocumentExportersFactory exportersFactory, IDocumentImportersFactory importersFactory) {
			this.exportManagerService = exportManagerService;
			this.importManagerService = importManagerService;
			this.exportersFactory = exportersFactory;
			this.importersFactory = importersFactory;
		}
		public IDocumentExportManagerService ExportManagerService { get { return exportManagerService; } }
		public IDocumentImportManagerService ImportManagerService { get { return importManagerService; } }
		public IDocumentExportersFactory ExportersFactory { get { return exportersFactory; } }
		public IDocumentImportersFactory ImportersFactory { get { return importersFactory; } }
	}
	public class EmptyDocumentExportersFactory : IDocumentExportersFactory {
		public Export.Rtf.IRtfExporter CreateRtfExporter(DocumentModel modelForExport, RtfDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.PlainText.IPlainTextExporter CreatePlainTextExporter(DocumentModel modelForExport, PlainTextDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.Html.IHtmlExporter CreateHtmlExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.Mht.IMhtExporter CreateMhtExporter(DocumentModel modelForExport, MhtDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.OpenXml.IOpenXmlExporter CreateOpenXmlExporter(DocumentModel modelForExport, OpenXmlDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.OpenDocument.IOpenDocumentTextExporter CreateOpenDocumentTextExporter(DocumentModel DocumentModel, OpenDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.Xaml.IXamlExporter CreateXamlExporter(DocumentModel documentModel, XamlDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.WordML.IWordMLExporter CreateWordMLExporter(DocumentModel modelForExport, WordMLDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
		public Export.Doc.IDocExporter CreateDocExporter(DocumentModel DocumentModel, DocDocumentExporterOptions options) {
			throw new NotImplementedException();
		}
	}
	public class EmptyDocumentImportersFactory : IDocumentImportersFactory {
		public IRtfImporter CreateRtfImporter(DocumentModel DocumentModel, RtfDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.Html.IHtmlImporter CreateHtmlImporter(DocumentModel DocumentModel, HtmlDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.Mht.IMhtImporter CreateMhtImporter(DocumentModel DocumentModel, MhtDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.OpenDocument.IOpenDocumentTextImporter CreateOpenDocumentTextImporter(DocumentModel DocumentModel, OpenDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.WordML.IWordMLImporter CreateWordMLImporter(DocumentModel DocumentModel, WordMLDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.Doc.IDocImporter CreateDocImporter(DocumentModel DocumentModel, DocDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.Xaml.IXamlImporter CreateXamlImporter(DocumentModel DocumentModel, XamlDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
		public Import.OpenXml.IOpenXmlImporter CreateOpenXmlImporter(DocumentModel DocumentModel, OpenXmlDocumentImporterOptions options) {
			throw new NotImplementedException();
		}
	}
}
