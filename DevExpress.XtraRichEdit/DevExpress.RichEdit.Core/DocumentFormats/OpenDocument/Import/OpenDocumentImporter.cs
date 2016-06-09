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
using System.IO;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Compatibility.System.Drawing.Printing;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#else
using System.Drawing.Printing;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region OpenDocumentTextImporter
	public class OpenDocumentTextImporter : OpenDocumentImporterBase, IOpenDocumentTextImporter {
		#region Defaults
		readonly PageLayoutStyleInfo defaultPageStyle;
		readonly int defaultTab;
		readonly UnitsConverter unitsConverter;
		#endregion
		#region Fields
		readonly Stack<OpenDocumentPieceTableInfo> pieceTableInfos;
		FontTable fontTable;
		MasterPageStyles masterPageStyles;
		PageLayoutStyles pageLayoutStyles;
		CharacterAutoStyles characterAutoStyles;
		ParagraphAutoStyles paragraphAutoStyles;
		SectionAutoStyles sectionAutoStyles; 
		TableAutoStyles tableAutoStyles;
		FrameAutoStyles frameAutoStyles; 
		TableColumnAutoStyles tableColumnAutoStyles;
		TableRowsAutoStyles tableRowsAutoStyles;
		TableCellsAutoStyles tableCellsAutoStyles;
		OpenDocumentListInfoCollection listInfos;
		OpenDocumentLineNumbering lineNumberingInfo;
		readonly DocumentImporterOptions options;
		int tablesCount;
		#endregion
		public OpenDocumentTextImporter(DocumentModel documentModel, OpenDocumentImporterOptions options)
			: base(documentModel) {
			this.pieceTableInfos = new Stack<OpenDocumentPieceTableInfo>();
			PushCurrentPieceTable(documentModel.MainPieceTable);
			this.defaultTab = (int)documentModel.UnitConverter.CentimetersToModelUnitsF(2);
			this.unitsConverter = new UnitsConverter(UnitConverter);
			this.fontTable = new FontTable();
			this.masterPageStyles = new MasterPageStyles();
			this.pageLayoutStyles = new PageLayoutStyles();
			this.characterAutoStyles = new CharacterAutoStyles();
			this.paragraphAutoStyles = new ParagraphAutoStyles();
			this.sectionAutoStyles = new SectionAutoStyles();
			this.tableAutoStyles = new TableAutoStyles();
			this.tableColumnAutoStyles = new TableColumnAutoStyles();
			this.tableRowsAutoStyles = new TableRowsAutoStyles();
			this.tableCellsAutoStyles = new TableCellsAutoStyles();
			this.frameAutoStyles = new FrameAutoStyles();
			this.listInfos = new OpenDocumentListInfoCollection();
			this.lineNumberingInfo = new OpenDocumentLineNumbering();
			this.defaultPageStyle = CreateDefaultLayoutStyle();
			this.options = options;
		}
		#region Properties
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected internal DocumentImporterOptions InnerOptions { get { return options; } }
		internal PieceTable PieceTable { get { return PieceTableInfo.PieceTable; } }
		OpenDocumentPieceTableInfo PieceTableInfo { get { return pieceTableInfos.Peek(); } }
		public Dictionary<string, ImportRangePermissionInfo> RangePermissions { get { return PieceTableInfo.RangePermissions; } }
		public Stack<ImportFieldInfo> FieldInfoStack { get { return PieceTableInfo.FieldInfoStack; } }
		internal OpenDocumentInputPosition InputPosition { get { return PieceTableInfo.Position; } }
		internal Dictionary<string, ImportBookmarkInfo> Bookmarks { get { return PieceTableInfo.Bookmarks; } }
		internal Dictionary<string, ImportCommentInfo> Comments { get { return PieceTableInfo.Comments; } }
		internal OpenDocumentImporterOptions Options { get { return (OpenDocumentImporterOptions)InnerOptions; } }
		internal FontTable FontTable { get { return fontTable; }  }
		internal MasterPageStyles MasterPageStyles { get { return masterPageStyles; } }
		internal PageLayoutStyles PageLayoutStyles { get { return pageLayoutStyles; }  }
		internal CharacterAutoStyles CharacterAutoStyles { get { return characterAutoStyles; } }
		internal SectionAutoStyles SectionAutoStyles { get { return sectionAutoStyles; } }
		internal ParagraphAutoStyles ParagraphAutoStyles { get { return paragraphAutoStyles; } }
		internal TableAutoStyles TableAutoStyles { get { return tableAutoStyles; } }
		internal FrameAutoStyles FrameAutoStyles { get { return frameAutoStyles; } }
		internal TableColumnAutoStyles TableColumnAutoStyles { get { return tableColumnAutoStyles; } }
		internal TableRowsAutoStyles TableRowsAutoStyles { get { return tableRowsAutoStyles; } }
		internal TableCellsAutoStyles TableCellsAutoStyles { get { return tableCellsAutoStyles; } }
		internal OpenDocumentListInfoCollection ListInfos { get { return listInfos; } } 
		internal OpenDocumentLineNumbering LineNumbering { get { return lineNumberingInfo; } } 
		internal int DefaultTab { get { return defaultTab; } }
		internal PageLayoutStyleInfo DefaultPageStyle { get { return defaultPageStyle; } }
		protected internal UnitsConverter UnitsConverter { get { return unitsConverter; } }
		#endregion
		#region Defaults
		internal virtual PageLayoutStyleInfo CreateDefaultLayoutStyle() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			PageLayoutStyleInfo result = new PageLayoutStyleInfo(DocumentModel);
			result.Page.PaperKind = PaperKind.A4;
			result.Page.Width = (int)unitConverter.CentimetersToModelUnitsF(21);
			result.Page.Height = (int)unitConverter.CentimetersToModelUnitsF(29.7f);
			result.Page.Landscape = false;
			int defaultMargin = (int)unitConverter.CentimetersToModelUnitsF(2.0f);
			result.Margins.Left = defaultMargin;
			result.Margins.Right = defaultMargin;
			result.Margins.Top = defaultMargin;
			result.Margins.Bottom = defaultMargin;
			return result;
		}
		protected override void ApplyDefaultProperties() {
		}
		protected internal virtual void ApplyDefaultTabStop() {
		}
		protected internal virtual void ApplyDefaultSectionProperties() {
		}
		#endregion
		#region Import
		public void Import(Stream stream) {
			OpenPackage(stream);
			string fileName = "content.xml";
			XmlReader contentReader = GetPackageFileXmlReader(fileName);
			if (contentReader != null) {
				if (!ReadToRootElement(contentReader, "document-content", OpenDocumentHelper.OfficeNamespace))
					return;
				ImportMainDocument(contentReader, stream);
			}
		}
		protected override void BeforeImportMainDocument() {
			base.BeforeImportMainDocument();
			ApplyFirstSectionProperties();
		}
		protected override void AfterImportMainDocument() {
			DocumentModel.MainPieceTable.FixLastParagraph();
			InsertBookmarks();
			InsertRangePermissions();
			InsertComments();
			PieceTable.FixTables();
			DocumentModel.NormalizeZOrder();
			PieceTable.CheckIntegrity();
		}
		#endregion
		#region Helpers
		protected internal virtual void PushCurrentPieceTable(PieceTable pieceTable) {
			pieceTableInfos.Push(new OpenDocumentPieceTableInfo(pieceTable));
		}
		protected internal virtual PieceTable PopCurrentPieceTable() {
			return pieceTableInfos.Pop().PieceTable;
		}
		#endregion
		#region Styles
		protected override Destination CreateRootStyleDestination() {
			return new DocumentStylesDestination(this);
		}
		protected override void ImportStylesCore(XmlReader reader) {
			base.ImportStylesCore(reader);
			ClearLocalStyleAutoStyles();
			PreProcessMasterPageStyleInfos();
		}
		protected internal virtual void ClearLocalStyleAutoStyles() {
			paragraphAutoStyles.Clear();
			characterAutoStyles.Clear();
		}
		#endregion
		#region Bookmarks
		protected internal void InsertBookmarks() {
			if (!DocumentFormatsHelper.ShouldInsertBookmarks(DocumentModel))
				return;
			foreach (string id in Bookmarks.Keys) {
				ImportBookmarkInfo bookmark = Bookmarks[id];
				if (bookmark.Validate(PieceTable))
					PieceTable.CreateBookmarkCore(bookmark.Start, bookmark.End - bookmark.Start, bookmark.Name);
			}
		}
		#endregion
		#region Range Permissions
		protected internal void InsertRangePermissions() {
			Dictionary<string, ImportRangePermissionInfo> rangePermissions = CreateRangePermisions(RangePermissions, PieceTable.DocumentStartLogPosition, PieceTable.DocumentEndLogPosition);
			foreach (string id in rangePermissions.Keys) {
				ImportRangePermissionInfo rangePermission = rangePermissions[id];
				if (rangePermission.Validate(PieceTable))
					PieceTable.ApplyDocumentPermission(rangePermission.Start, rangePermission.End, rangePermission.PermissionInfo);
			}
		}
		protected Dictionary<string, ImportRangePermissionInfo> CreateRangePermisions(Dictionary<string, ImportRangePermissionInfo> protectionRanges, DocumentLogPosition documentStart, DocumentLogPosition documentEnd) {
			Dictionary<string, ImportRangePermissionInfo> result = new Dictionary<string,ImportRangePermissionInfo>();
			int count = protectionRanges.Count;
			if (count == 0) 
				return result;
			DocumentLogPosition currentPos = documentStart;
			List<string> keys = new List<string>(protectionRanges.Keys);
			for (int i = 0; i < count; i++) {
				ImportRangePermissionInfo protection = protectionRanges[keys[i]];
				if (currentPos < protection.Start)
					AddImportRangePermissionInfo(result, currentPos, protection.Start - 1);
				currentPos = GetValidNextLogPosition(protection.End, documentEnd);
			}
			if (currentPos < documentEnd) 
				AddImportRangePermissionInfo(result, currentPos, documentEnd);
			return result;
		}
		protected internal virtual DocumentLogPosition GetValidNextLogPosition(DocumentLogPosition val, DocumentLogPosition maxValue) {
			return Algorithms.Min<DocumentLogPosition>(val + 1, maxValue);
		}
		protected internal virtual void AddImportRangePermissionInfo(Dictionary<string, ImportRangePermissionInfo> ranges, DocumentLogPosition start, DocumentLogPosition end) {
			ImportRangePermissionInfo permission = new ImportRangePermissionInfo();
			permission.Start = start;
			permission.End = end;
			permission.PermissionInfo.UserName = "Everyone";
			string key = "rp" + ranges.Count.ToString();
			ranges.Add(key, permission);
		}
		#endregion
		#region Comments
		protected internal void InsertComments() {
			foreach (string name in Comments.Keys) {
				ImportCommentInfo comment = Comments[name];
				comment.CalculateCommentPosition();
				if (comment.Validate(PieceTable)) {
					PieceTable.CreateCommentCore(comment.Start, comment.End - comment.Start, comment.Name, comment.Author,  comment.Date,  null, comment.Content);
				}
			}
		}
		#endregion      
		#region SectionStyle
		public virtual PageLayoutStyleInfo GetPageLayoutStyle(string masterPageName) {
			if (String.IsNullOrEmpty(masterPageName))
				return null;
			MasterPageStyleInfo masterPageStyle;
			if (!MasterPageStyles.TryGetValue(masterPageName, out masterPageStyle)) {
				return null;
			}
			PageLayoutStyleInfo pageLayoutStyle;
			PageLayoutStyles.TryGetValue(masterPageStyle.PageLayoutName, out pageLayoutStyle);
			return pageLayoutStyle;
		}
		public SectionStyleInfo GetSectionStyle(string styleName) {
			if (String.IsNullOrEmpty(styleName))
				return null;
			SectionStyleInfo sectionStyle;
			if (!SectionAutoStyles.TryGetValue(styleName, out sectionStyle)) {
				Debug.Assert(false, "SectionAutoStyle not founded", styleName);
			}
			return sectionStyle;
		}
		public virtual void ApplySectionProperties(Section section, PageLayoutStyleInfo pageStyle, SectionStyleInfo sectionStyle) {
			if (!DocumentModel.DocumentCapabilities.SectionsAllowed)
				return;
			section.Reset();
			if (pageStyle != null)
				ApplySectionPageProperties(section, pageStyle);
			if (sectionStyle != null)
				ApplySectionStylePageProperties(section, sectionStyle);
			ColumnStyles columns = GetActualColumns(pageStyle, sectionStyle);
			ApplySectionColumns(section, columns);
			section.GeneralSettings.StartType = SectionStartType.NextPage;
			if (LineNumbering.ShowLineNumbering) 
				section.LineNumbering.CopyFrom(LineNumbering);
			ModifyMasterPageNameFirstPageHeaderFooter(section);
			if (InputPosition.CurrentSectionInfo.DifferentFirstPage)
				section.GeneralSettings.DifferentFirstPage = true;
		}
		void ModifyMasterPageNameFirstPageHeaderFooter(Section section) {
			string mpName = InputPosition.CurrentParagraphInfo.MasterPageName;
			if (InputPosition.CurrentParagraphInfo.HasMasterPage) {
				string nextMasterPageName = MasterPageStyles[mpName].NextMasterPageStyleName;
				if (!String.IsNullOrEmpty(nextMasterPageName)) {
					InputPosition.CurrentSectionInfo.DifferentFirstPage = true;
					InputPosition.NextMasterPageName = nextMasterPageName;
				}
			}
			else if (!String.IsNullOrEmpty(InputPosition.NextMasterPageName)) {
				if (InputPosition.NextMasterPageName != "Standard")
					mpName = InputPosition.NextMasterPageName;
				InputPosition.NextMasterPageName = String.Empty;
			}
		}
		internal virtual ColumnStyles GetActualColumns(PageLayoutStyleInfo pageStyle, SectionStyleInfo sectionStyle) {			
			if ((pageStyle != null) && IsNotEmpty(pageStyle.Columns))
				return pageStyle.Columns;
			if ((sectionStyle != null) && IsNotEmpty(sectionStyle.Columns))
				return sectionStyle.Columns;
			return new ColumnStyles();
		}
		internal virtual bool IsNotEmpty(ColumnStyles columns) {
			return columns.Count > 0 || columns.StyleColumnCount > 0;
		}
		internal virtual void ApplySectionPageProperties(Section section, PageLayoutStyleInfo pageStyle) {
			pageStyle.Page.ValidatePaperKind(DocumentModel.UnitConverter);
			section.Page.CopyFrom(pageStyle.Page);
			section.Margins.CopyFrom(pageStyle.Margins);
			section.DocumentModel.DocumentProperties.DisplayBackgroundShape = true;
			section.DocumentModel.DocumentProperties.PageBackColor = pageStyle.BackgroundColor;
		}
		internal virtual void ApplySectionStylePageProperties(Section section, SectionStyleInfo sectionStyle) {
			section.Margins.Left += sectionStyle.LeftMargin;
			section.Margins.Right += sectionStyle.RightMargin;
		}
		internal virtual void ApplySectionColumns(Section section, ColumnStyles columns) {
			section.Columns.DrawVerticalSeparator = columns.DrawVerticalSeparator;
			if (columns.Count == 0)
				ApplyEqualSectionColumns(section, columns);
			else
				ApplyNonEqualSectionColumns(section, columns);
		}
		internal virtual void ApplyEqualSectionColumns(Section section, ColumnStyles columns) {
			if (columns.StyleColumnCount != 0) {
				section.Columns.EqualWidthColumns = true;
				section.Columns.ColumnCount = columns.StyleColumnCount;
				section.Columns.Space = columns.StyleColumnGap;
			}
		}
		internal virtual void ApplyNonEqualSectionColumns(Section section, ColumnStyles columns) {
			ColumnsInfo info = CalculateColumnsInfo(section, columns);
			info.EqualWidthColumns = false;
			section.Columns.CopyFrom(info);
		}
		internal virtual ColumnsInfo CalculateColumnsInfo(Section section, ColumnStyles columnStyles) {
			ColumnsInfo columnsInfo = new ColumnsInfo();
			int count = columnStyles.Count;
			columnsInfo.ColumnCount = count;
			float pageWidth = CalculateSectionWidth(section);
			float columnsWidth = CalculateTotalColumnsWidth(columnStyles);
			Debug.Assert(columnsWidth != 0);
			if (columnsWidth == 0)
				return columnsInfo;
			for (int i = 0; i < count; i++) {
				ColumnInfo info = new ColumnInfo();
				info.Space = CalculateColumnSpace(columnStyles, i);
				info.Width = CalculateColumnWidth(columnStyles[i], columnsWidth, pageWidth);
				columnsInfo.Columns.Add(info);
			}
			return columnsInfo;
		}
		internal virtual int CalculateColumnWidth(ColumnStyleInfo style, float columnsWidth, float pageWidth) {
			float indents = style.StartIndent + style.EndIndent;
			return (int)Math.Max(0, style.RelativeWidth / columnsWidth * pageWidth - indents);
		}
		internal virtual int CalculateColumnSpace(ColumnStyles columns, int columnIndex) {
			bool isLastColumn = columnIndex == columns.Count - 1;
			float endIndent = columns[columnIndex].EndIndent;
			float nextStartIndent = isLastColumn ? 0 : columns[columnIndex + 1].StartIndent;
			return (int)(endIndent + nextStartIndent);
		}
		internal virtual float CalculateTotalColumnsWidth(ColumnStyles columns) {
			float result = 0;
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				result += columns[i].RelativeWidth;
			return result;
		}
		internal virtual float CalculateSectionWidth(Section section) {
			return section.Page.Width - section.Margins.Left - section.Margins.Right;
		}
		#endregion
		#region Headers Footers
		protected internal virtual void ApplyFirstSectionProperties() {
			string masterPageName = "Standard";
			PageLayoutStyleInfo pageStyle = GetPageLayoutStyle(masterPageName);
			if (pageStyle == null)
				pageStyle = DefaultPageStyle;
			Section section = DocumentModel.Sections.First; 
			ApplySectionProperties(section, pageStyle, null);
			ApplyHeaderAndFooterToSection(masterPageName, section, false);
		}
		protected internal virtual void ApplyHeaderAndFooterToSection(string masterPageName, Section section, bool isDestPieceTableFirstPage) {
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			MasterPageStyleInfo masterPageStyle;
			if (!MasterPageStyles.TryGetValue(masterPageName, out masterPageStyle))
				return;
			ApplyHeaderToSectionHelper headerHelper = new ApplyHeaderToSectionHelper(section, masterPageStyle);
			headerHelper.Process(isDestPieceTableFirstPage);
			ApplyFooterToSectionHelper footerHelper = new ApplyFooterToSectionHelper(section, masterPageStyle);
			footerHelper.Process(isDestPieceTableFirstPage);
		}
		#region PreProcessMasterPageStyleInfos
		protected internal virtual void PreProcessMasterPageStyleInfos() {
			bool isDifferent = false;
			foreach (KeyValuePair<string, MasterPageStyleInfo> keyPair in MasterPageStyles) {
				MasterPageStyleInfo styleInfo = keyPair.Value;
				if (styleInfo.IsHeadersDifferent || styleInfo.IsFootersDifferent) {
					isDifferent = true;
					break;
				}
			}
			DocumentModel.DocumentProperties.DifferentOddAndEvenPages = isDifferent;
			if (isDifferent) {
				foreach (KeyValuePair<string, MasterPageStyleInfo> keyPair in MasterPageStyles) {
					MasterPageStyleInfo styleInfo = keyPair.Value;
					CopyHeaderContentToEmptyHeaderLeft(styleInfo);
					CopyFooterContentToEmptyFooterLeft(styleInfo);
				}
			}
		}
		#endregion
		#region CopyHeaderContentToEmptyHeaderLeft
		void CopyHeaderContentToEmptyHeaderLeft(MasterPageStyleInfo styleInfo) {
			if (styleInfo.IsHeadersAvailable && !styleInfo.IsHeadersDifferent) {
				styleInfo.HeaderLeftIndex = styleInfo.CreateEmptyHeader();
				ApplyHeaderToSectionHelper headerHelper = new ApplyHeaderToSectionHelper(DocumentModel.Sections.First, styleInfo);
				headerHelper.CopyCore(styleInfo.Header, styleInfo.HeaderLeft);
			}
		}
		#endregion
		#region CopyFooterContentToEmptyFooterLeft
		void CopyFooterContentToEmptyFooterLeft(MasterPageStyleInfo styleInfo) {
			if (styleInfo.IsFootersAvailable && !styleInfo.IsFootersDifferent) {
				styleInfo.FooterLeftIndex = styleInfo.CreateEmptyFooter();				
				ApplyFooterToSectionHelper footerHelper = new ApplyFooterToSectionHelper(DocumentModel.Sections.First, styleInfo);
				footerHelper.CopyCore(styleInfo.Footer, styleInfo.FooterLeft);
			}
		}
		#endregion
		#endregion
		public void InsertParagraph() {
			if (DocumentFormatsHelper.ShouldInsertParagraph(DocumentModel)) {
				PieceTable.InsertParagraphCore(InputPosition);
			} else
				PieceTable.InsertTextCore(InputPosition, new String(' ', 1));
		}
		public void InsertSection() {
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ParagraphIndex paragraphIndex = InputPosition.ParagraphIndex;
				PieceTable.InsertSectionParagraphCore(InputPosition);
				DocumentModel.SafeEditor.PerformInsertSectionCore(paragraphIndex); 
				InputPosition.RegisterParagraphForTableCellDestination();
			}
		}
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this); 
		}
		public override void BeginSetMainDocumentContent() {
			DocumentModel.BeginSetContent();
		}
		public override void EndSetMainDocumentContent() {
			DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, true, Options.UpdateField.GetNativeOptions());
		}
		public override void SetMainDocumentEmptyContent() {
			DocumentModel.ClearDocumentCore(true, true);
		}
		protected internal void IncTablesCount() {
			this.tablesCount++;
		}
		protected internal void DecTablesCount() {
			this.tablesCount--;
			if (this.tablesCount < 0)
				Exceptions.ThrowInternalException();
		}
		public bool IsInTable() {
			return this.tablesCount > 0;
		}
	}
	#endregion
	#region InputPositionState
	public class CharacterFormattingState {
		CharacterFormattingBase characterFormatting;
		int characterStyleIndex;
		public CharacterFormattingState(CharacterFormattingBase characterFormatting, int styleIndex) {
			this.characterFormatting = characterFormatting;
			this.characterStyleIndex = styleIndex;
		}
		public CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } set { characterFormatting = value; } }
		public int CharacterStyleIndex { get { return characterStyleIndex; } set { characterStyleIndex = value; } }
	}
	#endregion
	#region OpenDocumentPieceTableInfo
	public class OpenDocumentPieceTableInfo {
		#region Fields
		readonly PieceTable pieceTable;
		readonly Dictionary<string, ImportBookmarkInfo> bookmarks;
		readonly OpenDocumentInputPosition position;
		readonly Dictionary<string, ImportRangePermissionInfo> rangePermissions;
		readonly Stack<ImportFieldInfo> fieldInfoStack;
		readonly Dictionary<string, ImportCommentInfo> comments;
		#endregion
		public OpenDocumentPieceTableInfo(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.position = new OpenDocumentInputPosition(pieceTable);
			this.fieldInfoStack = new Stack<ImportFieldInfo>();
			this.bookmarks = new Dictionary<string, ImportBookmarkInfo>();
			this.rangePermissions = new Dictionary<string, ImportRangePermissionInfo>();
			this.comments = new Dictionary<string, ImportCommentInfo>();
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public OpenDocumentInputPosition Position { get { return position; } }
		public Stack<ImportFieldInfo> FieldInfoStack { get { return fieldInfoStack; } }
		public Dictionary<string, ImportBookmarkInfo> Bookmarks { get { return bookmarks; } }
		public Dictionary<string, ImportRangePermissionInfo> RangePermissions { get { return rangePermissions; } }
		public Dictionary<string, ImportCommentInfo> Comments { get { return comments; } }
		#endregion
	}
	#endregion
	#region OpenDocumentInputPosition
	public class OpenDocumentInputPosition : InputPosition {
		#region Fields
		readonly ParagraphInfo currentParagraphInfo;
		readonly Stack<CharacterFormattingState> characterFormattingStack;
		readonly NumberingListReference currentListReference;
		readonly SectionInfo currentSectionInfo;
		readonly OpenDocumentTablesImportHelper tablesImportHelper;
		string nextMasterPageName = String.Empty;
		readonly OpenDocumentTableCellReference currentTableCellReference;
		#endregion
		public OpenDocumentInputPosition(PieceTable pieceTable)
			: base(pieceTable) {
			this.characterFormattingStack = new Stack<CharacterFormattingState>();
			CharacterFormattingBase defaultCharacterFormatting = new CharacterFormattingBase(pieceTable, pieceTable.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.characterFormattingStack.Push(new CharacterFormattingState(defaultCharacterFormatting, base.CharacterStyleIndex));
			this.currentSectionInfo = new SectionInfo();
			this.currentParagraphInfo = new ParagraphInfo(pieceTable);
			this.currentListReference = new NumberingListReference(-1, -1);
			this.currentTableCellReference = new OpenDocumentTableCellReference();
			this.tablesImportHelper = new OpenDocumentTablesImportHelper(PieceTable, this);
		}
		#region Properties
		protected internal Stack<CharacterFormattingState> CharacterFormattingStack { get { return characterFormattingStack; } }
		public override CharacterFormattingBase CharacterFormatting { get { return characterFormattingStack.Peek().CharacterFormatting; } }
		public override int CharacterStyleIndex { get { return characterFormattingStack.Peek().CharacterStyleIndex; } set { characterFormattingStack.Peek().CharacterStyleIndex = value; } }
		public NumberingListReference CurrentListReference { get { return currentListReference; } }
		public OpenDocumentTableCellReference CurrentTableCellReference { get { return currentTableCellReference; } }
		public ParagraphInfo CurrentParagraphInfo { get { return currentParagraphInfo; }  }
		public SectionInfo CurrentSectionInfo { get { return currentSectionInfo; } }
		public OpenDocumentTablesImportHelper TablesImportHelper { get { return tablesImportHelper; } }
		public string NextMasterPageName { get { return nextMasterPageName; } set { nextMasterPageName = value; } }
		#endregion
		public void SaveCharacterFormatting() {
			int styleIndex = CharacterFormattingStack.Peek().CharacterStyleIndex;
			CharacterFormattingState state = new CharacterFormattingState(CharacterFormatting.Clone(), styleIndex);
			CharacterFormattingStack.Push(state);
		}
		public void RestoreCharacterFormatting() {
			CharacterFormattingStack.Pop();
		}
		public void RegisterParagraphForTableCellDestination() {
			ITableCellDestination destination = CurrentTableCellReference.CurrentCell;
			if (destination == null)
				return;
			destination.EndParagraphIndex = ParagraphIndex;
			destination.IsEmptyCell = false;
		}
	}
	#endregion
	#region SectionInfo
	public class SectionInfo {
		#region Fields
		string name;
		string styleName;
		bool sectionTagOpened;
		bool sectionTagClosed;
		bool isFirstAtDocument;
		bool restartLineNumbering;
		bool differentFirstPage;
		#endregion
		public SectionInfo() {
			this.styleName = String.Empty;
			this.IsFirstAtDocument = true;
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public string StyleName { get { return styleName; } set { styleName = value; } }
		public bool SectionTagOpened { get { return sectionTagOpened; } set { sectionTagOpened = value; } }
		public bool SectionTagClosed { get { return sectionTagClosed; } set { sectionTagClosed = value; } }
		public bool IsFirstAtDocument { get { return isFirstAtDocument; } set { isFirstAtDocument = value; } }
		public bool RestartLineNumbering { get { return restartLineNumbering; } set { restartLineNumbering = value; } }
		public bool DifferentFirstPage { get { return differentFirstPage; } set { differentFirstPage = value; } }
		#endregion
	}
	#endregion
	#region ParagraphInfo
	public class ParagraphInfo {
		#region Fields
		readonly ParagraphFormattingBase paragraphFormatting;
		readonly TabFormattingInfo tabs;
		readonly ParagraphBreaksInfo breaks;
		int styleIndex;
		string masterPageName;
		bool isFirstAtDocument;
		#endregion
		public ParagraphInfo(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.paragraphFormatting = new ParagraphFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.tabs = new TabFormattingInfo();
			this.breaks = new ParagraphBreaksInfo();
			this.masterPageName = String.Empty;
			this.isFirstAtDocument = true;
		}
		#region Properties
		public TabFormattingInfo Tabs { get { return tabs; }  }
		public ParagraphBreaksInfo Breaks { get { return breaks; } }
		public ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		public int StyleIndex { get { return styleIndex; } set { styleIndex = value; } }
		public string MasterPageName { get { return masterPageName; } set { masterPageName = value; } }
		public bool IsFirstAtDocument { get { return isFirstAtDocument; } set { isFirstAtDocument = value; } }
		public bool HasMasterPage { get { return !String.IsNullOrEmpty(MasterPageName); } }
		#endregion
		public void Reset(DocumentModel documentModel) {
			StyleIndex = 0;
			MasterPageName = String.Empty;
			Tabs.Clear();
			Breaks.Clear();
			ParagraphFormatting.ReplaceInfo(documentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
		}
	}
	#endregion
	#region OpenDocumentLineNumbering
	public class OpenDocumentLineNumbering : LineNumberingInfo {
		bool useLineNumbering;
		public OpenDocumentLineNumbering() 
			: base() {
		}
		public LineNumberingInfo CloneInfo() {
			return base.Clone();
		}
		public bool ShowLineNumbering { get { return useLineNumbering; } set { useLineNumbering = value; } }
	}
	#endregion
}
