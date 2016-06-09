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

#define EXPORT_CHARTS
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
using DevExpress.Office.Utils;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Runtime.InteropServices;
using DevExpress.Office.Model;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsGlobalsExporter : XlsSubstreamExporter {
		#region Fields
		XlsStylesExporter stylesExporter;
		XlsSharedStringsExporter sharedStringsExporter;
		Queue<long> boundPositions = new Queue<long>();
		string rtdPrefix;
		#endregion
		public XlsGlobalsExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet)
			: base(streamWriter, documentModel, exportStyleSheet) {
			this.stylesExporter = CreateStylesExporter();
			this.sharedStringsExporter = new XlsSharedStringsExporter(StreamWriter, DocumentModel, ExportStyleSheet);
			MainExporter = null;
		}
		public Queue<long> BoundPositions { get { return boundPositions; } }
		protected internal XlsStylesExporter StylesExporter { get { return stylesExporter; } }
		protected internal XlsExporter MainExporter { get; set; }
		public override void WriteContent() {
			WriteBOF(XlsSubstreamType.WorkbookGlobals);
			WriteFilePass();
			WriteTemplate();
			WriterInterface();
			WriteWriteAcess();
			WriteCodePage();
			WriteDSF();
			WriteExcel9File();
			WriteSheetIdTable();
			WriteVBAProjectRecords();
			WriteCodeName();
			WriteOleObjectSize();
			WriteProtection();
			WriteWindow1();
			WriteBackup();
			WriteHideObj();
			WriteDate1904();
			WriteCalcPrecision();
			WriteRefreshAll();
			WriteBookBool();
			WriteFormatting();
			WritePivotCacheDefinition(); 
			WriteUsesELFs();
			WriteBundleSheet();
			WriteMetaData();
			WriteCountry();
			WriteSupportingLinks();
			WriteDefinedNames();
			WriteRealTimeData();
			WriteRecalcId();
			WriteDrawingGroup();
			WriteSharedStrings();
			WriteExtendedSST();
			WriteBookExt();
			WriteOfficeTheme();
			WriteEOF();
		}
		protected void WriteFilePass() {
			if (MainExporter == null || !MainExporter.ShouldEncryptWorkbook)
				return;
			XlsCommandFilePassword command = new XlsCommandFilePassword();
			command.RC4EncryptionHeader = MainExporter.EncryptionHeader;
			command.Write(StreamWriter);
		}
		protected void WriteTemplate() {
			if (MainExporter == null || !MainExporter.IsTemplate)
				return;
			XlsCommandTemplate command = new XlsCommandTemplate();
			command.Write(StreamWriter);
		}
		protected void WriterInterface() {
			XlsCommandInterfaceHeader headerCommand = new XlsCommandInterfaceHeader();
			headerCommand.Encoding = Encoding.Unicode;
			headerCommand.Write(StreamWriter);
			XlsCommandAddDelMenuItems mmsCommand = new XlsCommandAddDelMenuItems();
			mmsCommand.Write(StreamWriter);
			XlsCommandInterfaceEnd endCommand = new XlsCommandInterfaceEnd();
			endCommand.Write(StreamWriter);
		}
		protected void WriteWriteAcess() {
			XlsCommandWriteAccess command = new XlsCommandWriteAccess();
			command.Write(StreamWriter);
		}
		protected void WriteCodePage() {
			XlsCommandEncoding command = new XlsCommandEncoding();
			command.Encoding = Encoding.Unicode;
			command.Write(StreamWriter);
		}
		protected void WriteDSF() {
			XlsCommandDSF command = new XlsCommandDSF();
			command.Write(StreamWriter);
		}
		protected void WriteExcel9File() {
			XlsCommandExcel9File command = new XlsCommandExcel9File();
			command.Write(StreamWriter);
		}
		protected void WriteSheetIdTable() {
			XlsCommandSheetIdTable command = new XlsCommandSheetIdTable();
			for(int i = 0; i < DocumentModel.Sheets.Count; i++)
				command.SheetIdTable.Add(i + 1);
			command.Write(StreamWriter);
		}
		protected void WriteVBAProjectRecords() {
			if(DocumentModel.VbaProjectContent.IsEmpty)
				return;
			XlsCommandVBAProject command = new XlsCommandVBAProject();
			command.Write(StreamWriter);
			if(DocumentModel.VbaProjectContent.HasNoMacros) {
				XlsCommandVBAProjectHasNoMacros commandHasNoMacros = new XlsCommandVBAProjectHasNoMacros();
				commandHasNoMacros.Write(StreamWriter);
			}
		}
		protected void WriteCodeName() {
			if(DocumentModel.VbaProjectContent.IsEmpty || string.IsNullOrEmpty(DocumentModel.Properties.CodeName))
				return;
			XlsCommandCodeName command = new XlsCommandCodeName();
			command.Name = DocumentModel.Properties.CodeName;
			command.Write(StreamWriter);
		}
		void WriteOleObjectSize() {
			if (MainExporter == null || MainExporter.OleObjectRange == null)
				return;
			XlsCommandOleObjectSize command = new XlsCommandOleObjectSize();
			command.Range = MainExporter.OleObjectRange;
			command.Write(StreamWriter);
		}
		protected void WriteProtection() {
			XlsCommandWindowsProtected winProtCommand = new XlsCommandWindowsProtected();
			winProtCommand.Value = DocumentModel.Properties.Protection.LockWindows;
			winProtCommand.Write(StreamWriter);
			XlsCommandProtected protCommand = new XlsCommandProtected();
			protCommand.Value = DocumentModel.Properties.Protection.LockStructure;
			protCommand.Write(StreamWriter);
			ushort passwordVerifier = ProtectionByPasswordVerifier.PasswordNotSetted;
			ushort revisionsPasswordVerifier = ProtectionByWorkbookRevisions.RevisionsPasswordNotSetted;
			ProtectionCredentials protection = DocumentModel.Properties.Protection.Credentials;
			if (!protection.IsEmpty) {
				ProtectionCredentials enforcedProtection = protection as ProtectionCredentials;
				if (enforcedProtection != null) {
					ProtectionByPasswordVerifier byPasswordVerifier = enforcedProtection.PasswordVerifier;
					if (byPasswordVerifier != null)
						passwordVerifier = byPasswordVerifier.Value;
					ProtectionByWorkbookRevisions byRevisionPassword = enforcedProtection.RevisionProtection;
					if (byRevisionPassword != null)
						revisionsPasswordVerifier = byRevisionPassword.Value;
				}
			}
			XlsCommandPasswordVerifier passCommand = new XlsCommandPasswordVerifier();
			passCommand.Value = (short)passwordVerifier;
			passCommand.Write(StreamWriter);
			XlsCommandProtectForRevisions prot4RevCommand = new XlsCommandProtectForRevisions();
			prot4RevCommand.Value = DocumentModel.Properties.Protection.LockRevisions;
			prot4RevCommand.Write(StreamWriter);
			XlsCommandProtectForRevisionsPasswordVerifier prot4RevPassCommand = new XlsCommandProtectForRevisionsPasswordVerifier();
			prot4RevPassCommand.Value = (short)revisionsPasswordVerifier;
			prot4RevPassCommand.Write(StreamWriter);
		}
		protected void WriteWindow1() {
			XlsCommandWorkbookWindowInformation command = new XlsCommandWorkbookWindowInformation();
			XlsContentWorkbookWindow content = command.Content;
			List<WorkbookWindowProperties> list = DocumentModel.Properties.WorkbookWindowPropertiesList;
			int count = list.Count;
			for (int i = 0; i < count; i++) {
				WorkbookWindowProperties item = list[i];
				content.HorizontalPosition = item.HorizontalPosition;
				content.VerticalPosition = item.VerticalPosition;
				content.WidthInTwips = item.WidhtInTwips;
				content.HeightInTwips = item.HeightInTwips;
				content.IsHidden = item.Visibility == SheetVisibleState.Hidden;
				content.IsMinimized = item.Minimized;
				content.IsVeryHidden = item.Visibility == SheetVisibleState.VeryHidden;
				content.HorizontalScrollDisplayed = item.ShowHorizontalScroll;
				content.VerticalScrollDisplayed = item.ShowVerticalScroll;
				content.SheetTabsDisplayed = item.ShowSheetTabs;
				content.NoAutoFilterDateGrouping = !item.AutoFilterDateGrouping;
				content.SelectedTabIndex = item.SelectedTabIndex;
				content.FirstDisplayedTabIndex = item.FirstDisplayedTabIndex;
				content.SelectedTabsCount = item.SelectedTabsCount;
				content.TabRatio = item.TabRatio;
				command.Write(StreamWriter);
			}
		}
		protected void WriteBackup() {
			XlsCommandShouldSaveBackup command = new XlsCommandShouldSaveBackup();
			command.Value = false;
			command.Write(StreamWriter);
		}
		protected void WriteHideObj() {
			XlsCommandDisplayObjectsOptions command = new XlsCommandDisplayObjectsOptions();
			command.DisplayObjects = DocumentModel.Properties.DisplayObjects;
			command.Write(StreamWriter);
		}
		protected void WriteDate1904() {
			XlsCommandIs1904DateSystemUsed command = new XlsCommandIs1904DateSystemUsed();
			command.Value = DocumentModel.Properties.CalculationOptions.DateSystem == DateSystem.Date1904;
			command.Write(StreamWriter);
		}
		protected void WriteCalcPrecision() {
			XlsCommandPrecisionAsDisplayed command = new XlsCommandPrecisionAsDisplayed();
			command.Value = DocumentModel.Properties.CalculationOptions.PrecisionAsDisplayed;
			command.Write(StreamWriter);
		}
		protected void WriteRefreshAll() {
			XlsCommandRefreshAllOnLoading command = new XlsCommandRefreshAllOnLoading();
			if (MainExporter == null || !MainExporter.IsTemplate)
				command.Value = false;
			else
				command.Value = DocumentModel.Properties.RefreshAllOnLoading;
			command.Write(StreamWriter);
		}
		protected void WriteBookBool() {
			XlsCommandWorkbookBoolProperties command = new XlsCommandWorkbookBoolProperties();
			command.SaveExternalLinksValues = DocumentModel.Properties.CalculationOptions.SaveExternalLinkValues;
			command.ShowBordersOfUnselectedTables = DocumentModel.Properties.ShowBordersOfUnselectedTables;
			command.EnvelopeInitDone = DocumentModel.MailOptions.EnvelopeInitDone;
			command.EnvelopeVisible = DocumentModel.MailOptions.EnvelopeVisible;
			command.HasEnvelope = DocumentModel.MailOptions.HasEnvelope;
			command.Write(StreamWriter);
		}
		protected void WriteFormatting() {
			this.stylesExporter.WriteContent();
		}
		protected void WritePivotCacheDefinition() {
			new XlsPivotCacheDefinitionExporter(StreamWriter, DocumentModel, ExportStyleSheet, MainExporter).WriteContent();
		}
		protected void WriteUsesELFs() {
			XlsCommandSupportNaturalLanguagesFormulaInput command = new XlsCommandSupportNaturalLanguagesFormulaInput();
			command.Value = DocumentModel.Properties.SupportNaturalLanguagesFormulaInput;
			command.Write(StreamWriter);
		}
		protected void WriteBundleSheet() {
			XlsCommandSheetInformation command = new XlsCommandSheetInformation();
			WorksheetCollection sheets = DocumentModel.Sheets;
			foreach (Worksheet sheet in sheets) {
				SaveBoundSheetStartPosition();
				command.Type = SheetType.RegularWorksheet;
				command.VisibleState = sheet.VisibleState;
				command.Name = sheet.Name;
				command.Write(StreamWriter);
			}
		}
		protected void WriteMetaData() {
		}
		protected void WriteSupportingLinks() {
			List<XlsSupBookInfo> supBooks = ExportStyleSheet.RPNContext.SupBooks;
			if(supBooks.Count == 0) return;
			for(int i = 0; i < supBooks.Count; i++) {
				XlsSupBookInfo supBook = supBooks[i];
				WriteSubBook(supBook);
				WriteExternalNames(supBook);
				WriteExternalCache(supBook);
				WriteOleDdeItems(supBook);
				WriteAddInNames(supBook);
			}
			WriteExternSheet(ExportStyleSheet.RPNContext.ExternSheets);
		}
		void WriteSubBook(XlsSupBookInfo info) {
			XlsCommandSupBook firstCommand = new XlsCommandSupBook();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using(XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				info.Write(writer);
			}
		}
		#region WriteExternalNames
		void WriteExternalNames(XlsSupBookInfo info) {
			if(info.LinkType != XlsSupportingLinkType.ExternalWorkbook || info.ExternalIndex == 0) return;
			ExternalLink link = DocumentModel.ExternalLinks[info.ExternalIndex - 1];
			ExternalWorkbook workbook = link.Workbook;
			foreach(XlsDefinedNameInfo defNameInfo in info.ExternalNames) {
				ExternalWorksheet sheet = null;
				ExternalDefinedNameCollection definedNames = workbook.DefinedNames;
				if(defNameInfo.Scope != XlsDefs.NoScope) {
					sheet = workbook.Sheets[defNameInfo.Scope];
					definedNames = sheet.DefinedNames;
				}
				string name = defNameInfo.Name;
				string description = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_DefinedName) + " \"{0}\" ({1} \"{2}\")", 
					name, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_ExternalWorkbook) , workbook.FilePath);
				DefinedNameBase definedName = definedNames[name];
				XlsExternNameInfo extNameInfo = new XlsExternNameInfo();
				extNameInfo.LinkType = XlsSupportingLinkType.ExternalWorkbook;
				XlsExternDocName content = new XlsExternDocName();
				content.Name = definedName.Name;
				content.SheetIndex = definedName.ScopedSheetId + 1;
				ParsedExpression expression = new ParsedExpression();
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentWorksheet(sheet);
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentWorkbook(workbook);
				ExportStyleSheet.RPNContext.WorkbookContext.PushDefinedNameProcessing(definedName);
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(0, 0);
				ExportStyleSheet.RPNContext.PushCurrentSubject(description);
				try {
					if(definedName.Expression != null && definedName.Expression.Count > 0) {
						expression = definedName.Expression;
						content.Formula = ExportStyleSheet.RPNContext.ExtNameExpressionToBinary(XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext));
					}
				}
				finally {
					ExportStyleSheet.RPNContext.PopCurrentSubject();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
					ExportStyleSheet.RPNContext.WorkbookContext.PopDefinedNameProcessing();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentWorkbook();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentWorksheet();
				}
				extNameInfo.Content = content;
				WriteExternalName(extNameInfo);
			}
		}
		void WriteExternalName(XlsExternNameInfo info) {
			XlsCommandExternName firstCommand = new XlsCommandExternName();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using(XlsChunkWriter commandWriter = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				info.Write(commandWriter);
			}
		}
		#endregion
		#region WriteExternalCache
		void WriteExternalCache(XlsSupBookInfo info) {
			if(info.LinkType != XlsSupportingLinkType.ExternalWorkbook || info.ExternalIndex == 0) return;
			ExternalLink link = DocumentModel.ExternalLinks[info.ExternalIndex - 1];
			ExternalWorkbook workbook = link.Workbook;
			for(int i = 0; i < workbook.SheetCount; i++)
				WriteExternalCacheSheet(workbook.Sheets[i], i, workbook.Sheets[i].RefreshFailed);
		}
		void WriteExternalCacheSheet(ExternalWorksheet sheet, int sheetIndex, bool refreshFailed) {
			long initialPosition = StreamWriter.BaseStream.Position;
			XlsCommandExternCacheStart command = new XlsCommandExternCacheStart();
			command.SheetIndex = sheetIndex;
			command.Write(StreamWriter);
			int count = 0;
			foreach(ExternalRow row in sheet.Rows.GetExistingRows(0, XlsDefs.MaxRowCount - 1, false)) {
				count += WriteExternalCacheRow(row);
			}
			if(count > 0) {
				StreamWriter.BaseStream.Seek(initialPosition, SeekOrigin.Begin);
				command.Count = refreshFailed ? -count : count;
				command.Write(StreamWriter);
				StreamWriter.BaseStream.Seek(0, SeekOrigin.End);
			}
		}
		int WriteExternalCacheRow(ExternalRow row) {
			int result = 0;
			XlsExternCacheData data = new XlsExternCacheData();
			foreach(ExternalCell cell in row.Cells.GetExistingCells(0, XlsDefs.MaxColumnCount - 1, false)) {
				IPtgExtraArrayValue item = PtgExtraArrayFactory.CreateArrayValue(cell.Value);
				item.Value = cell.Value;
				if((data.GetSize() + item.GetSize()) > XlsDefs.MaxRecordDataSize) {
					WriteExternalCacheData(data);
					data.Values.Clear();
					result++;
				}
				if(data.Values.Count == 0) {
					data.Row = cell.RowIndex;
					data.FirstColumn = cell.ColumnIndex;
				}
				data.Values.Add(item);
			}
			if(data.Values.Count > 0) {
				WriteExternalCacheData(data);
				result++;
			}
			return result;
		}
		void WriteExternalCacheData(XlsExternCacheData data) {
			XlsCommandExternCacheItem firstCommand = new XlsCommandExternCacheItem();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using(XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				data.Write(writer);
			}
		}
		#endregion
		#region WriteOleDdeItems
		void WriteOleDdeItems(XlsSupBookInfo info) {
			if(info.LinkType != XlsSupportingLinkType.DataSource || info.ExternalIndex == 0) return;
			ExternalLink link = DocumentModel.ExternalLinks[info.ExternalIndex - 1];
			DdeExternalWorkbook connection = link.Workbook as DdeExternalWorkbook;
			foreach(DdeExternalWorksheet item in connection.Sheets)
				WriteDdeItem(item);
		}
		void WriteDdeItem(DdeExternalWorksheet item) {
			XlsExternNameInfo info = new XlsExternNameInfo();
			info.LinkType = XlsSupportingLinkType.DataSource;
			info.WantAdvise = item.Advise;
			info.WantPictureFormat = item.IsDataImage;
			info.IsOle = item.IsUsesOLE;
			info.ClipboardFormat = XlsExternClipboardFormat.None;
			if(info.IsOle) {
				XlsExternDdeLink content = new XlsExternDdeLink();
				content.Name = item.Name;
				info.Content = content;
			}
			else {
				XlsExternOleDdeLink content = new XlsExternOleDdeLink();
				content.Name = item.Name;
				int columns = item.ColumnCount;
				int rows = item.RowCount;
				int count = rows * columns;
				if(count > 0) {
					content.LastColumn = columns - 1;
					content.LastRow = rows - 1;
					for(int i = 0; i < count; i++) {
						int rowIndex = i / columns;
						int columnIndex = i % columns;
						VariantValue value = VariantValue.Empty;
						ExternalCell cell = item.TryGetCell(columnIndex, rowIndex) as ExternalCell;
						if(cell != null)
							value = cell.Value;
						IPtgExtraArrayValue valueItem = PtgExtraArrayFactory.CreateArrayValue(value);
						valueItem.Value = value;
						content.Values.Add(valueItem);
					}
				}
				info.Content = content;
			}
			WriteExternalName(info);
		}
		#endregion
		#region WriteAddInNames
		void WriteAddInNames(XlsSupBookInfo info) {
			if(info.LinkType != XlsSupportingLinkType.AddIn) return;
			foreach(XlsDefinedNameInfo item in info.ExternalNames) {
				XlsExternNameInfo extNameInfo = new XlsExternNameInfo();
				extNameInfo.LinkType = XlsSupportingLinkType.AddIn;
				XlsAddInUdf content = new XlsAddInUdf();
				content.Name = item.Name;
				extNameInfo.Content = content;
				WriteExternalName(extNameInfo);
			}
		}
		#endregion
		void WriteExternSheet(List<XlsExternInfo> list) {
			XlsCommandExternSheet firstCommand = new XlsCommandExternSheet();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using(XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				list.Write(writer);
			}
		}
		#region DefinedNames
		protected void WriteDefinedNames() {
			foreach(XlsDefinedNameInfo info in ExportStyleSheet.RPNContext.DefinedNames) {
				if(info.Scope == XlsDefs.NoScope)
					WriteDefinedNameError(info.Name, -1);
				else
					WriteDefinedName(info.Name, info.Scope);
			}
		}
		void WriteDefinedNameError(string name, int scope) {
			XlsCommandDefinedName command = new XlsCommandDefinedName();
			command.Content.Name = name;
			command.Content.SheetIndex = DocumentModel.Sheets.GetIndexById(scope) + 1;
			ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(0, 0);
			try {
				ParsedExpression expression = new ParsedExpression();
				command.SetParsedExpression(expression, ExportStyleSheet.RPNContext);
			}
			finally {
				ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
			}
			command.Write(StreamWriter);
		}
		void WriteDefinedName(string name, int scope) {
			int sheetIndex = -1;
			DefinedNameCollectionBase collection = DocumentModel.DefinedNames;
			if(scope >= 0) {
				sheetIndex = DocumentModel.Sheets.GetIndexById(scope);
				collection = DocumentModel.GetSheetByIndex(sheetIndex).DefinedNames;
			}
			if(collection.Contains(name)) {
				DefinedNameBase definedName = collection[name];
				XlsCommandDefinedName command = new XlsCommandDefinedName();
				command.Content.Name = definedName.Name;
				command.Content.SheetIndex = sheetIndex + 1;
				DefinedName defName = definedName as DefinedName;
				if(defName != null) {
					command.Content.IsHidden = defName.IsHidden;
					command.Content.Comment = defName.Comment;
					command.Content.IsXlmMacro = defName.IsXlmMacro;
					command.Content.IsVbaMacro = defName.IsVbaMacro;
					command.Content.IsMacro = defName.IsMacro;
					command.Content.FunctionCategory = defName.FunctionGroupId;
				}
				else {
					command.Content.IsHidden = false;
					command.Content.Comment = string.Empty;
					command.Content.IsXlmMacro = false;
					command.Content.IsVbaMacro = false;
					command.Content.IsMacro = false;
					command.Content.FunctionCategory = 0;
				}
				ExportStyleSheet.RPNContext.WorkbookContext.PushDefinedNameProcessing(definedName);
				ExportStyleSheet.RPNContext.WorkbookContext.PushCurrentCell(0, 0);
				ExportStyleSheet.RPNContext.PushCurrentSubject(definedName.GetDescription());
				try {
					ParsedExpression expression = definedName.Expression;
					expression = XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext);
					command.SetParsedExpression(expression, ExportStyleSheet.RPNContext);
				}
				finally {
					ExportStyleSheet.RPNContext.WorkbookContext.PopDefinedNameProcessing();
					ExportStyleSheet.RPNContext.WorkbookContext.PopCurrentCell();
					ExportStyleSheet.RPNContext.PopCurrentSubject();
				}
				command.Write(StreamWriter);
				WriteDefinedNameComment(command.Content);
			}
			else
				WriteDefinedNameError(name, scope);
		}
		protected void WriteDefinedNameComment(XlsContentDefinedNameExt content) {
			if(string.IsNullOrEmpty(content.Comment)) return;
			XlsCommandDefinedNameComment command = new XlsCommandDefinedNameComment();
			command.Name = content.InternalName;
			command.Comment = content.Comment;
			command.Write(StreamWriter);
		}
		#endregion
		#region RealTimeData
		protected void WriteRealTimeData() {
			List<RealTimeDataApplication> applications = GetRTDApplications();
			foreach (RealTimeDataApplication application in applications)
				WriteRealTimeData(application);
		}
		void WriteRealTimeData(RealTimeDataApplication application) {
			if (!application.HasTopics)
				return;
			rtdPrefix = string.Empty;
			foreach (RealTimeTopic topic in application.Topics.Values)
				WriteRealTimeData(topic);
		}
		void WriteRealTimeData(RealTimeTopic topic) {
			XlsRealTimeData data = new XlsRealTimeData();
			data.SamePrefix = rtdPrefix.Length;
			data.ApplicationId = topic.Application.ApplicationId;
			data.ServerName = topic.Application.ServerName;
			rtdPrefix = GetRTDPrefix(data.ApplicationId, data.ServerName);
			data.Parameters.AddRange(topic.Parameters);
			GetRTDValue(data.Value, topic.CachedValue);
			foreach (ICell cell in topic.ReferencedCells) {
				if(cell.RowIndex >= XlsDefs.MaxRowCount)
					continue;
				if(cell.ColumnIndex >= XlsDefs.MaxColumnCount)
					continue;
				XlsRTDCell rtdCell = new XlsRTDCell();
				rtdCell.RowIndex = cell.RowIndex;
				rtdCell.ColumnIndex = cell.ColumnIndex;
				rtdCell.SheetIndex = DocumentModel.Sheets.IndexOf(cell.Worksheet);
				data.Cells.Add(rtdCell);
			}
			if (data.Parameters.Count == 0 || data.Cells.Count == 0)
				return;
			XlsCommandRealTimeData firstCommand = new XlsCommandRealTimeData();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using (XlsChunkWriter rtdWriter = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				FutureRecordHeader frtHeader = new FutureRecordHeader();
				frtHeader.RecordTypeId = 0x0813;
				frtHeader.Write(rtdWriter);
				data.Write(rtdWriter);
			}
		}
		void GetRTDValue(XlsRTDValue rtdValue, VariantValue value) {
			if (value.IsBoolean)
				rtdValue.BooleanValue = value.BooleanValue;
			else if (value.IsError)
				rtdValue.ErrorValue = ErrorConverter.ValueToErrorCode(value);
			else if (value.IsNumeric)
				rtdValue.DoubleValue = value.NumericValue;
			else if (value.IsInlineText)
				rtdValue.StringValue = value.InlineTextValue;
			else if (value.IsSharedString)
				rtdValue.StringValue = value.GetTextValue(DocumentModel.SharedStringTable);
		}
		string GetRTDPrefix(string applicationId, string serverName) {
			StringBuilder sb = new StringBuilder();
			sb.Append((char)applicationId.Length);
			sb.Append(applicationId);
			sb.Append((char)serverName.Length);
			sb.Append(serverName);
			return sb.ToString();
		}
		List<RealTimeDataApplication> GetRTDApplications() {
			List<RealTimeDataApplication> result = new List<RealTimeDataApplication>();
			result.AddRange(DocumentModel.RealTimeDataManager.Applications.Values);
			result.Sort(CompareRTDApplication);
			return result;
		}
		int CompareRTDApplication(RealTimeDataApplication x, RealTimeDataApplication y) {
			if (x == null) {
				if (y == null)
					return 0;
				else
					return -1;
			}
			else {
				if (y == null)
					return 1;
				if (x.ApplicationId.CompareTo(y.ApplicationId) == -1)
					return -1;
				if (x.ApplicationId.CompareTo(y.ApplicationId) == 1)
					return 1;
				return x.ServerName.CompareTo(y.ServerName);
			}
		}
		#endregion
		protected void WriteRecalcId() {
		}
		protected void WriteCountry() {
			XlsCommandCountry command = new XlsCommandCountry();
			command.DefaultCountryIndex = XlsCountryCodes.GetCountryCode(CultureInfo.CurrentUICulture);
			command.CountryIndex = XlsCountryCodes.GetCountryCode(DocumentModel.Culture);
			command.Write(StreamWriter);
		}
		#region DrawingGroup
		protected void WriteDrawingGroup() {
			if(ShouldWriteDrawingGroup()) {
				OfficeArtDrawingGroupContainer drawingGroup = new OfficeArtDrawingGroupContainer();
				FillFileDrawingBlock(drawingGroup);
				FillBlipContainer(drawingGroup);
				FillArtProperties(drawingGroup);
				XlsCommandMsoDrawingGroup firstCommand = new XlsCommandMsoDrawingGroup();
				XlsCommandContinue continueCommand = new XlsCommandContinue();
				using(XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
					drawingGroup.Write(writer, writer);
				}
			}
		}
		bool ShouldWriteDrawingGroup() {
			return ExportStyleSheet.ObjectsCount > 0;
		}
#if EXPORT_CHARTS
		void FillFileDrawingBlock(OfficeArtDrawingGroupContainer drawingGroup) {
			int maxShapedId = 0;
			int totalShapes = 0;
			int totalDrawings = 0;
			WorksheetCollection sheets = DocumentModel.Sheets;
			for (int i = 0; i < sheets.Count; i++) {
				Worksheet sheet = sheets[i];
				int count = sheet.Comments.CountInXlsCellRange();
				if(sheet.Workbook.DocumentCapabilities.PicturesAllowed)
					count += sheet.DrawingObjects.GetPictureCount();
				if (sheet.Workbook.DocumentCapabilities.ChartsAllowed)
					count += sheet.DrawingObjects.GetChartCount();
				if(sheet.Workbook.DocumentCapabilities.ShapesAllowed)
					count += sheet.DrawingObjects.GetShapesCount();
				if (count > 0) {
					totalShapes += count + 1;
					totalDrawings++;
				}
				int clusterMaxShapeId = count + 1;
				maxShapedId = Math.Max(maxShapedId, sheet.GetTopmostShapeId() + clusterMaxShapeId);
				OfficeArtFileIdCluster cluster = new OfficeArtFileIdCluster();
				cluster.ClusterId = sheet.SheetId;
				cluster.LargestShapeId = clusterMaxShapeId;
				drawingGroup.FileDrawingBlock.Clusters.Add(cluster);
			}
			drawingGroup.FileDrawingBlock.MaxShapeId = maxShapedId;
			drawingGroup.FileDrawingBlock.TotalShapes = totalShapes;
			drawingGroup.FileDrawingBlock.TotalDrawings = totalDrawings;
		}
#else
		void FillFileDrawingBlock(OfficeArtDrawingGroupContainer drawingGroup) {
			int maxShapedId = 0;
			int totalShapes = 0;
			int totalDrawings = 0;
			WorksheetCollection sheets = DocumentModel.Sheets;
			for(int i = 0; i < sheets.Count; i++) {
				Worksheet sheet = sheets[i];
				int count = sheet.DrawingObjects.Count + sheet.Comments.CountInXlsCellRange();
				if(count > 0) {
					totalShapes += count + 1;
					totalDrawings++;
				}
				int clusterMaxShapeId = count + 1;
				maxShapedId = Math.Max(maxShapedId, sheet.GetTopmostShapeId() + clusterMaxShapeId);
				OfficeArtFileIdCluster cluster = new OfficeArtFileIdCluster();
				cluster.ClusterId = sheet.SheetId;
				cluster.LargestShapeId = clusterMaxShapeId;
				drawingGroup.FileDrawingBlock.Clusters.Add(cluster);
			}
			drawingGroup.FileDrawingBlock.MaxShapeId = maxShapedId;
			drawingGroup.FileDrawingBlock.TotalShapes = totalShapes;
			drawingGroup.FileDrawingBlock.TotalDrawings = totalDrawings;
		}
#endif
		void FillBlipContainer(OfficeArtDrawingGroupContainer drawingGroup) {
			foreach(KeyValuePair<OfficeImage, DrawingTableInfo> item in ExportStyleSheet.DrawingTable) {
				FileBlipStoreEntry entry = new FileBlipStoreEntry(item.Key, false);
				entry.ReferenceCount = item.Value.RefCount;
				drawingGroup.BlipContainer.Blips.Add(entry);
			}
		}
		void FillArtProperties(OfficeArtDrawingGroupContainer drawingGroup) {
			OfficeArtProperties artProperties = drawingGroup.ArtProperties;
			artProperties.Properties.Add(new DrawingTextBooleanProperties());
			DrawingFillColor fillColor = new DrawingFillColor();
			fillColor.ColorRecord.ColorSchemeIndex = Palette.DefaultBackgroundColorIndex;
			artProperties.Properties.Add(fillColor);
			DrawingLineColor lineColor = new DrawingLineColor();
			lineColor.ColorRecord.ColorSchemeIndex = Palette.DefaultForegroundColorIndex;
			artProperties.Properties.Add(lineColor);
		}
		#endregion
		protected void WriteSharedStrings() {
			this.sharedStringsExporter.WriteContent();
		}
		protected void WriteExtendedSST() {
			XlsCommandExtendedSST command = new XlsCommandExtendedSST();
			command.StringsInBucket = this.sharedStringsExporter.StringsInBucket;
			foreach (XlsSSTInfo item in this.sharedStringsExporter.ExtendedSSTItems) {
				command.Items.Add(item);
			}
			command.Write(StreamWriter);
		}
		protected void WriteBookExt() {
			XlsCommandWorkbookExtendedProperties command = new XlsCommandWorkbookExtendedProperties();
			command.IsHidePivotList = this.DocumentModel.Properties.HidePivotFieldList;
			command.IsShowInkAnnotation = true; 
			command.Write(StreamWriter);
		}
		protected void WriteOfficeTheme() {
			if (ExportStyleSheet.ClipboardMode)
				return;
			XlsThemesExporter exporter = new XlsThemesExporter(StreamWriter, DocumentModel);
			exporter.Export();
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				if (this.stylesExporter != null) {
					this.stylesExporter.Dispose();
					this.stylesExporter = null;
				}
				if (this.sharedStringsExporter != null) {
					this.sharedStringsExporter.Dispose();
					this.sharedStringsExporter = null;
				}
				MainExporter = null;
			}
		}
		#endregion
		void SaveBoundSheetStartPosition() {
			long startPositionOffset = StreamWriter.BaseStream.Position + 4; 
			BoundPositions.Enqueue(startPositionOffset);
		}
		protected virtual XlsStylesExporter CreateStylesExporter() {
			return new XlsStylesExporter(StreamWriter, DocumentModel, ExportStyleSheet);
		}
	}
}
