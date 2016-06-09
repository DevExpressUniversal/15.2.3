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
using System.Text;
using System.Xml;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.Utils.Crypt;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsExporter
	public class XlsExporter : DocumentModelExporter, IDisposable {
		Stream outputStream;
		BinaryWriter mainStreamWriter;
		BinaryWriter xmlStreamWriter;
		BinaryWriter summaryInformationWriter;
		BinaryWriter docSummaryInformationWriter;
		List<VbaProjectEntry> vbaProjectEntries;
		XlsDocumentExporterOptions options;
		ExportXlsStyleSheet exportStyleSheet;
		XlsGlobalsExporter globalsExporter;
		XlsRC4EncryptionHeader encryptionHeader = null;
		ARC4KeyGen keygen = null;
		ARC4Cipher cipher = null;
		List<CustomXmlDataEntry> customXmlDataEntries;
		List<PivotCacheEntry> pivotCacheEntries;
		List<PivotCache> excludedPivotCaches;
		List<PivotCache> includedPivotCaches;
		public XlsExporter(DocumentModel workbook, XlsDocumentExporterOptions options)
			: base(workbook) {
			this.options = options;
			OleObjectRange = null;
		}
		protected BinaryWriter MainStreamWriter { get { return mainStreamWriter; } }
		protected BinaryWriter XmlStreamWriter { get { return xmlStreamWriter; } }
		protected BinaryWriter SummaryInformationWriter { get { return summaryInformationWriter; } }
		protected BinaryWriter DocSummaryInformationWriter { get { return docSummaryInformationWriter; } }
		public XlsDocumentExporterOptions Options { get { return options; } }
		public Stream OutputStream { get { return outputStream; } }
		protected internal ExportXlsStyleSheet ExportStyleSheet { get { return exportStyleSheet; } }
		protected internal XlsGlobalsExporter GlobalsExporter { get { return globalsExporter; } }
		protected internal bool IsTemplate { get { return options is XltDocumentExporterOptions; } }
		protected internal CellRangeInfo OleObjectRange { get; set; }
		protected internal List<PivotCache> ExcludedPivotCaches { get { return excludedPivotCaches; } }
		protected internal List<PivotCache> IncludedPivotCaches { get { return includedPivotCaches; } }
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			Export();
		}
		public override void Export() {
			if (this.outputStream == null)
				throw new InvalidOperationException();
			Initialize();
			ILogService logService = Workbook.GetService<ILogService>();
			if(logService != null)
				logService.Clear();
			Workbook.DataContext.SetImportExportSettings();
			Workbook.DataContext.PushCurrentLimits(XlsDefs.MaxColumnCount, XlsDefs.MaxRowCount);
			try {
				ExportStyleSheet.RegisterSharedStrings();
				ExportStyleSheet.RegisterStyles();
				ExportStyleSheet.RegisterSheetDefinitions();
				ExportStyleSheet.RegisterObjects();
				GlobalsExporter.WriteContent();
				base.Export();
				if(logService != null) {
					if(ExportStyleSheet.DefaultStyleXFUsed) {
						string message = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefaultStyleXFUsed);
						logService.LogMessage(LogCategory.Warning, message);
					}
					if(ExportStyleSheet.DefaultCellXFUsed) {
						string message = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefaultCellXFUsed);
						logService.LogMessage(LogCategory.Warning, message);
					}
				}
				ExportCustomXmlMappings();
				ExportVbaProjectContent();
				ExportSummaryInformation();
				ExportDocumentSummaryInformation();
				ExportCustomXmlData();
			}
			finally {
				Workbook.DataContext.SetWorkbookDefinedSettings();
				Workbook.DataContext.PopCurrentLimits();
			}
			Encrypt();
			Write();
		}
		internal void Initialize() {
			ChunkedMemoryStream mainStream = new ChunkedMemoryStream();
			this.mainStreamWriter = new BinaryWriter(mainStream);
			ChunkedMemoryStream xmlStream = new ChunkedMemoryStream();
			this.xmlStreamWriter = new BinaryWriter(xmlStream);
			this.vbaProjectEntries = new List<VbaProjectEntry>();
			ChunkedMemoryStream summaryStream = new ChunkedMemoryStream();
			this.summaryInformationWriter = new BinaryWriter(summaryStream);
			ChunkedMemoryStream docSummaryStream = new ChunkedMemoryStream();
			this.docSummaryInformationWriter = new BinaryWriter(docSummaryStream);
			this.exportStyleSheet = new ExportXlsStyleSheet(Workbook);
			this.exportStyleSheet.ClipboardMode = Options.ClipboardMode;
			this.globalsExporter = CreateGlobalsExporter();
			this.customXmlDataEntries = new List<CustomXmlDataEntry>();
			this.pivotCacheEntries = new List<PivotCacheEntry>();
			this.excludedPivotCaches = new List<PivotCache>();
			this.includedPivotCaches = new List<PivotCache>();
		}
		protected virtual XlsGlobalsExporter CreateGlobalsExporter() {
			return new XlsGlobalsExporter(MainStreamWriter, Workbook, ExportStyleSheet) { MainExporter = this };
		}
		public BinaryWriter CreatePivotCacheWriter(int pivotCacheId) {
			Guard.ArgumentPositive(pivotCacheId, "pivotCacheId");
			PivotCacheEntry entry = PivotCacheEntry.Create(pivotCacheId);
			ChunkedMemoryStream stream = new ChunkedMemoryStream();
			entry.Writer = new BinaryWriter(stream);
			this.pivotCacheEntries.Add(entry);
			return entry.Writer;
		}
		void Encrypt() {
			if (cipher != null) {
				XlsRC4Encryptor encryptor = new XlsRC4Encryptor(keygen, cipher, mainStreamWriter.BaseStream);
				encryptor.Execute();
				foreach (PivotCacheEntry entry in pivotCacheEntries) {
					encryptor = new XlsRC4Encryptor(keygen, cipher, entry.Writer.BaseStream);
					encryptor.Execute(true);
				}
			}
		}
		void Write() {
			StructuredStorageWriter structuredStorageWriter = new StructuredStorageWriter();
			StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.WorkbookStreamName, MainStreamWriter);
			if(XmlStreamWriter.BaseStream.Length > 0)
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.XmlStreamName, XmlStreamWriter);
			foreach(VbaProjectEntry entry in vbaProjectEntries)
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, entry.StreamName, entry.Writer);
			if (summaryInformationWriter.BaseStream.Length > 0)
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.SummaryStreamName, summaryInformationWriter);
			if (docSummaryInformationWriter.BaseStream.Length > 0)
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.DocSummaryStreamName, docSummaryInformationWriter);
			foreach(CustomXmlDataEntry customXmlDataEntry in customXmlDataEntries) {
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.CustomXMlStreamName + "\\" + customXmlDataEntry.StreamName + "\\Item", customXmlDataEntry.ItemData);
				StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, XlsStreams.CustomXMlStreamName + "\\" + customXmlDataEntry.StreamName + "\\Properties", customXmlDataEntry.ItemData);
			}
			foreach (PivotCacheEntry entry in pivotCacheEntries)
				if(entry.Writer.BaseStream.Length > 0)
					StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, entry.StreamName, entry.Writer);
			structuredStorageWriter.Write(outputStream);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.globalsExporter != null) {
					this.globalsExporter.Dispose();
					this.globalsExporter = null;
				}
				if (this.mainStreamWriter != null) {
					this.mainStreamWriter.Dispose();
					this.mainStreamWriter = null;
				}
				if (this.xmlStreamWriter != null) {
					this.xmlStreamWriter.Dispose();
					this.xmlStreamWriter = null;
				}
				foreach(VbaProjectEntry entry in vbaProjectEntries) {
					entry.Writer.Dispose();
					entry.Writer = null;
				}
				foreach(CustomXmlDataEntry customXmlDataEntry in customXmlDataEntries) {
					if(customXmlDataEntry.ItemData != null)
						customXmlDataEntry.ItemData.Dispose();
					customXmlDataEntry.ItemData = null;
					if(customXmlDataEntry.Properties != null)
						customXmlDataEntry.Properties.Dispose();
					customXmlDataEntry.Properties = null;
				}
				foreach (PivotCacheEntry entry in pivotCacheEntries) {
					entry.Writer.Dispose();
					entry.Writer = null;
				}
				this.outputStream = null;
				this.exportStyleSheet = null;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal override void ExportSheet(Worksheet sheet) {
			WriteBoundSheetStartPosition();
			using(XlsWorksheetExporter exporter = new XlsWorksheetExporter(MainStreamWriter, Workbook, ExportStyleSheet, sheet, this)) {
				exporter.WriteContent();
			}
		}
		protected internal override void ExportSheet(Chartsheet sheet) {
		}
		void WriteBoundSheetStartPosition() {
			long boundPos = GlobalsExporter.BoundPositions.Dequeue();
			long currentPos = MainStreamWriter.BaseStream.Position;
			MainStreamWriter.BaseStream.Seek(boundPos, SeekOrigin.Begin);
			MainStreamWriter.Write((uint)currentPos);
			MainStreamWriter.BaseStream.Seek(currentPos, SeekOrigin.Begin);
		}
		void ExportCustomXmlMappings() {
		}
		void ExportVbaProjectContent() {
			if(Workbook.VbaProjectContent.IsEmpty)
				return;
			foreach(VbaProjectItem item in Workbook.VbaProjectContent.Items) {
				string name = XlsStreams.VbaProjectRootStorageName + item.StreamName;
				VbaProjectExportHelper.ExportVbaProjectItem(this.vbaProjectEntries, name, item.Data);
			}
		}
		void ExportCustomXmlData() {
			ExportMailMergeDataSources();
		}
		void ExportMailMergeDataSources() {
			OpenXmlExporter exporter = new OpenXmlExporter(this.Workbook, new OpenXmlDocumentExporterOptions());
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, exporter.CreateXmlDataSourceWriterSettings());
			exporter.GenerateMailMergeCustomXmlContent(xmlWriter);
			xmlWriter.Flush();
			CustomXmlExportHelper.ExportCustomXmlData(customXmlDataEntries, "DataSources", memoryStream.ToArray());
		}
		#region Document properties
		void ExportSummaryInformation() {
			OlePropertyStreamContent content = new OlePropertyStreamContent();
			GenerateSummary(content);
			content.Write(this.summaryInformationWriter);
		}
		void ExportDocumentSummaryInformation() {
			OlePropertyStreamContent content = new OlePropertyStreamContent();
			GenerateDocSummary(content);
			GenerateUserDefined(content);
			content.Write(this.docSummaryInformationWriter);
		}
		void GenerateSummary(OlePropertyStreamContent content) {
			OlePropertySetSummary propertySet = new OlePropertySetSummary();
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			ModelDocumentCoreProperties coreProps = Workbook.DocumentCoreProperties;
			if (!string.IsNullOrEmpty(coreProps.Title))
				propertySet.Properties.Add(new OlePropertyTitle(OlePropDefs.VT_LPWSTR) { Value = coreProps.Title });
			if (!string.IsNullOrEmpty(coreProps.Subject))
				propertySet.Properties.Add(new OlePropertySubject(OlePropDefs.VT_LPWSTR) { Value = coreProps.Subject });
			if (!string.IsNullOrEmpty(coreProps.Creator))
				propertySet.Properties.Add(new OlePropertyAuthor(OlePropDefs.VT_LPWSTR) { Value = coreProps.Creator });
			if (!string.IsNullOrEmpty(coreProps.Keywords))
				propertySet.Properties.Add(new OlePropertyKeywords(OlePropDefs.VT_LPWSTR) { Value = coreProps.Keywords });
			if (!string.IsNullOrEmpty(coreProps.Description))
				propertySet.Properties.Add(new OlePropertyComments(OlePropDefs.VT_LPWSTR) { Value = coreProps.Description });
			if (!string.IsNullOrEmpty(coreProps.LastModifiedBy))
				propertySet.Properties.Add(new OlePropertyLastAuthor(OlePropDefs.VT_LPWSTR) { Value = coreProps.LastModifiedBy });
			if (coreProps.Created != DateTime.MinValue)
				propertySet.Properties.Add(new OlePropertyCreated() { Value = coreProps.Created });
			if (coreProps.Modified != DateTime.MinValue)
				propertySet.Properties.Add(new OlePropertyModified() { Value = coreProps.Modified });
			if (coreProps.LastPrinted != DateTime.MinValue)
				propertySet.Properties.Add(new OlePropertyLastPrinted() { Value = coreProps.LastPrinted });
			ModelDocumentApplicationProperties applProps = Workbook.DocumentApplicationProperties;
			if (!string.IsNullOrEmpty(applProps.Application))
				propertySet.Properties.Add(new OlePropertyApplication(OlePropDefs.VT_LPWSTR) { Value = applProps.Application });
			propertySet.Properties.Add(new OlePropertyDocSecurity() { Value = (int)applProps.Security });
			content.PropertySets.Add(propertySet);
		}
		void GenerateDocSummary(OlePropertyStreamContent content) {
			OlePropertySetDocSummary propertySet = new OlePropertySetDocSummary();
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			ModelDocumentCoreProperties coreProps = Workbook.DocumentCoreProperties;
			if (!string.IsNullOrEmpty(coreProps.Category))
				propertySet.Properties.Add(new OlePropertyCategory(OlePropDefs.VT_LPWSTR) { Value = coreProps.Category });
			ModelDocumentApplicationProperties applProps = Workbook.DocumentApplicationProperties;
			if (!string.IsNullOrEmpty(applProps.Manager))
				propertySet.Properties.Add(new OlePropertyManager(OlePropDefs.VT_LPWSTR) { Value = applProps.Manager });
			if (!string.IsNullOrEmpty(applProps.Company))
				propertySet.Properties.Add(new OlePropertyCompany(OlePropDefs.VT_LPWSTR) { Value = applProps.Company });
			if (!string.IsNullOrEmpty(applProps.Version)) {
				string[] parts = applProps.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				try {
					int version = Convert.ToInt32(parts[0]) << 16;
					propertySet.Properties.Add(new OlePropertyVersion() { Value = version });
				}
				catch { }
			}
			content.PropertySets.Add(propertySet);
		}
		void GenerateUserDefined(OlePropertyStreamContent content) {
			ModelDocumentCustomProperties props = Workbook.DocumentCustomProperties;
			if (props.Count == 0)
				return;
			OlePropertySetUserDefined propertySet = new OlePropertySetUserDefined();
			OlePropertyDictionary dictionary = new OlePropertyDictionary();
			propertySet.Properties.Add(dictionary);
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			int propertyId = OlePropDefs.PID_NORMAL_MIN;
			foreach (string name in props.Names) {
				DevExpress.Spreadsheet.CellValue value = props[name];
				if (value.IsText)
					propertySet.Properties.Add(new OlePropertyString(propertyId, OlePropDefs.VT_LPWSTR) { Value = value.TextValue });
				else if (value.IsDateTime)
					propertySet.Properties.Add(new OlePropertyFileTime(propertyId) { Value = value.DateTimeValue });
				else if (value.IsBoolean)
					propertySet.Properties.Add(new OlePropertyBool(propertyId) { Value = value.BooleanValue });
				else if (value.IsNumeric) {
					double doubleValue = value.NumericValue;
					int intValue = (int)doubleValue;
					if (intValue == doubleValue)
						propertySet.Properties.Add(new OlePropertyInt32(propertyId) { Value = intValue });
					else
						propertySet.Properties.Add(new OlePropertyDouble(propertyId) { Value = doubleValue });
				}
				else
					continue;
				dictionary.Entries.Add(propertyId, name);
				propertyId++;
			}
			if (dictionary.Entries.Count > 0)
				content.PropertySets.Add(propertySet);
		}
#endregion
#region Encryption
		protected internal bool ShouldEncryptWorkbook {
			get {
				if (!string.IsNullOrEmpty(Options.Password))
					return true;
				return Workbook.IsProtected && (GetWorkbookPasswordVerifier() != ProtectionByPasswordVerifier.PasswordNotSetted);
			}
		}
		protected internal XlsRC4EncryptionHeader EncryptionHeader {
			get {
				if (encryptionHeader == null)
					CreateEncryptionHeader();
				return encryptionHeader;
			}
		}
		void CreateEncryptionHeader() {
			this.encryptionHeader = new XlsRC4EncryptionHeader();
			string password = Options.Password;
			if (string.IsNullOrEmpty(password))
				password = XlsCommandFilePassword.MagicMSPassword;
			byte[] salt = new byte[16];
			byte[] verifier = new byte[16];
			Random random = new Random();
			random.NextBytes(salt);
			random.NextBytes(verifier);
			byte[] verifierHash = MD5Hash.ComputeHash(verifier);
			this.keygen = new ARC4KeyGen(password, salt);
			this.cipher = new ARC4Cipher(keygen.DeriveKey(0));
			this.encryptionHeader.Salt = salt;
			this.encryptionHeader.EncryptedVerifier = cipher.Encrypt(verifier);
			this.encryptionHeader.EncryptedVerifierHash = cipher.Encrypt(verifierHash);
		}
		ushort GetWorkbookPasswordVerifier() {
			ushort passwordVerifier = ProtectionByPasswordVerifier.PasswordNotSetted;
			ProtectionCredentials protection = Workbook.Properties.Protection.Credentials;
			if (!protection.IsEmpty) {
				ProtectionCredentials enforcedProtection = protection as ProtectionCredentials;
				if (enforcedProtection != null) {
					ProtectionByPasswordVerifier byPasswordVerifier = enforcedProtection.PasswordVerifier;
					if (byPasswordVerifier != null)
						passwordVerifier = byPasswordVerifier.Value;
				}
			}
			return passwordVerifier;
		}
#endregion
	}
#endregion
#region XlsExporterBase
	public abstract class XlsExporterBase : IDisposable {
		BinaryWriter streamWriter;
		DocumentModel documentModel;
		ExportXlsStyleSheet exportStyleSheet;
		protected XlsExporterBase(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet) {
			Guard.ArgumentNotNull(streamWriter, "streamWriter");
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(exportStyleSheet, "exportStyleSheet");
			this.streamWriter = streamWriter;
			this.documentModel = documentModel;
			this.exportStyleSheet = exportStyleSheet;
		}
#region Properties
		public BinaryWriter StreamWriter { get { return streamWriter; } }
		protected DocumentModel DocumentModel { get { return documentModel; } }
		protected internal ExportXlsStyleSheet ExportStyleSheet { get { return exportStyleSheet; } }
#endregion
		public abstract void WriteContent();
#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				this.streamWriter = null;
				this.documentModel = null;
				this.exportStyleSheet = null;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
#endregion
		public void LogMessage(LogCategory category, string message) {
			DocumentModel.LogMessage(category, message);
		}
	}
#endregion
#region XlsSubstreamExporter
	public abstract class XlsSubstreamExporter : XlsExporterBase {
		protected XlsSubstreamExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet)
			: base(streamWriter, documentModel, exportStyleSheet) {
		}
		protected void WriteBOF(XlsSubstreamType dataType) {
			XlsCommandBeginOfSubstream command = new XlsCommandBeginOfSubstream();
			command.SubstreamType = dataType;
			command.FileHistoryFlags = XlsFileHistory.Default;
			command.Write(StreamWriter);
		}
		protected void WriteEOF() {
			XlsCommandEndOfSubstream command = new XlsCommandEndOfSubstream();
			command.Write(StreamWriter);
		}
	}
#endregion
#region XlsWorksheetExporterBase
	public abstract class XlsWorksheetExporterBase : XlsSubstreamExporter {
		Worksheet sheet;
		protected XlsWorksheetExporterBase(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, Worksheet sheet)
			: base(streamWriter, documentModel, exportStyleSheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				this.sheet = null;
			}
		}
#endregion
	}
#endregion
#region XlsRC4Encryptor
	class XlsRC4Encryptor {
#region Fields
		const int blockSize = 1024;
		const short BOF = 2057;
		const short FilePass = 47;
		const short FileLock = 405;
		const short BoundSheet8 = 133;
		const short InterfaceHdr = 225;
		const short RRDHead = 312;
		const short RRDInfo = 406;
		const short UsrExcl = 404;
		ARC4KeyGen keygen;
		ARC4Cipher cipher;
		int blockCount;
		int bytesCount;
		Stream stream;
		short typeCode;
		short size;
		byte[] dataBuffer;
		byte[] shortBuffer;
#endregion
		public XlsRC4Encryptor(ARC4KeyGen keygen, ARC4Cipher cipher, Stream stream) {
			this.keygen = keygen;
			this.cipher = cipher;
			this.stream = stream;
			this.dataBuffer = new byte[XlsDefs.MaxRecordDataSize];
			this.shortBuffer = new byte[2];
		}
		public void Execute() {
			Execute(false);
		}
		public void Execute(bool hasFilePass) {
			stream.Position = 0;
			long streamLength = stream.Length;
			if(hasFilePass)
				ResetCipher(stream.Position);
			while (stream.Position < streamLength) {
				typeCode = ReadInt16();
				size = ReadInt16();
				long dataPosition = stream.Position;
				if (size > 0)
					stream.Read(dataBuffer, 0, size);
				if (hasFilePass) {
					SeekKeyStream(4);
					if (ShouldNotEncrypt(typeCode))
						SeekKeyStream(size);
					else if (typeCode == BoundSheet8) {
						byte[] boundSheetPos = new byte[4];
						Array.Copy(dataBuffer, boundSheetPos, 4);
						Encrypt(dataBuffer, size);
						Array.Copy(boundSheetPos, dataBuffer, 4);
						stream.Position = dataPosition;
						stream.Write(dataBuffer, 0, size);
					}
					else {
						Encrypt(dataBuffer, size);
						stream.Position = dataPosition;
						stream.Write(dataBuffer, 0, size);
					}
				}
				else if (typeCode == FilePass) {
					hasFilePass = true;
					ResetCipher(stream.Position);
				}
			}
		}
#region Internals
		short ReadInt16() {
			stream.Read(shortBuffer, 0, 2);
			return BitConverter.ToInt16(shortBuffer, 0);
		}
		void ResetCipher(long position) {
			this.blockCount = (int)(position / blockSize);
			this.bytesCount = (int)(position % blockSize);
			this.cipher.UpdateKey(this.keygen.DeriveKey(this.blockCount));
			this.cipher.Reset(this.bytesCount);
		}
		byte Encrypt(byte input) {
			byte output = this.cipher.Encrypt(input);
			this.bytesCount++;
			if (this.bytesCount == blockSize) {
				this.bytesCount = 0;
				this.blockCount++;
				this.cipher.UpdateKey(this.keygen.DeriveKey(blockCount));
			}
			return output;
		}
		byte[] Encrypt(byte[] input, int count) {
			for (int i = 0; i < count; i++) {
				input[i] = this.cipher.Encrypt(input[i]);
				this.bytesCount++;
				if (this.bytesCount == blockSize) {
					this.bytesCount = 0;
					this.blockCount++;
					this.cipher.UpdateKey(this.keygen.DeriveKey(blockCount));
				}
			}
			return input;
		}
		void SeekKeyStream(int offset) {
			for (int i = 0; i < offset; i++)
				Encrypt((byte)0);
		}
		bool ShouldNotEncrypt(short typeCode) {
			return typeCode == BOF || typeCode == FileLock || typeCode == InterfaceHdr || 
				typeCode == RRDHead || typeCode == RRDInfo || typeCode == UsrExcl || 
				typeCode == FilePass;
		}
#endregion
	}
#endregion
}
