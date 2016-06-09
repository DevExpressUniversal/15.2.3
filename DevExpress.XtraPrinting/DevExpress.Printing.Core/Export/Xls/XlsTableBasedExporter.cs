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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	using DevExpress.Utils.StructuredStorage.Writer;
	public class XlsDataAwareExporterOptions : IXlDocumentOptions {
		public XlDocumentFormat DocumentFormat { get { return XlDocumentFormat.Xls; } }
		public CultureInfo Culture { get; set; }
		public bool SupportsFormulas { get { return true; } }
		public bool SupportsDocumentParts { get { return true; } }
		public bool SupportsOutlineGrouping { get { return true; } }
		public int MaxColumnCount { get { return 256; } }
		public int MaxRowCount { get { return 65536; } }
	}
	public partial class XlsDataAwareExporter : IXlExport, IXlFormulaEngine, IXlExporter {
		#region Fields
		int currentRowIndex = 0;
		int currentColumnIndex = 0;
		BinaryWriter writer = null;
		Stream outputStream = null;
		CompositeMemoryStream workbookStream = null;
		BinaryWriter workbookWriter = null;
		BinaryWriter summaryInformationWriter = null;
		BinaryWriter docSummaryInformationWriter = null;
		readonly XlsContentEmpty contentEmpty = new XlsContentEmpty();
		readonly XlsContentBoolValue contentBoolValue = new XlsContentBoolValue();
		readonly XlsContentShortValue contentShortValue = new XlsContentShortValue();
		readonly XlsContentDoubleValue contentDoubleValue = new XlsContentDoubleValue();
		readonly XlsContentStringValue contentStringValue = new XlsContentStringValue();
		readonly XlsContentBeginOfSubstream contentBOF = new XlsContentBeginOfSubstream();
		readonly XlsDataAwareExporterOptions options = new XlsDataAwareExporterOptions();
		readonly IXlFormulaParser formulaParser;
		readonly XlExpressionContext expressionContext = new XlExpressionContext();
		readonly HashSet<XlCellPosition> sharedFormulaHostCells = new HashSet<XlCellPosition>();
		#endregion
		public XlsDataAwareExporter() 
			: this(null) {
		}
		public XlsDataAwareExporter(IXlFormulaParser formulaParser) {
			this.formulaParser = formulaParser;
			this.expressionContext.MaxColumnCount = XlsDefs.MaxColumnCount;
			this.expressionContext.MaxRowCount = XlsDefs.MaxRowCount;
		}
		#region IXlExporter implementation
		public IXlDocument CreateDocument(Stream stream) {
			return BeginExport(stream);
		}
		#endregion
		#region IXlExport Members
		public int CurrentRowIndex { get { return currentRow == null ? currentRowIndex : currentRow.RowIndex; } }
		public int CurrentColumnIndex { get { return currentColumnIndex; } }
		public IXlDocumentOptions DocumentOptions { get { return options; } }
		public XlDocumentProperties DocumentProperties { get { return documentProperties; } }
		internal XlCalculationOptions CalculationOptions { get { return calculationOptions; } }
		public bool ClipboardMode { get; set; }
		public IXlDocument BeginExport(Stream stream) {
			InitializeStyles();
			InitializeSharedStrings();
			outputStream = stream;
			workbookStream = new CompositeMemoryStream();
			workbookWriter = new BinaryWriter(workbookStream);
			ChunkedMemoryStream summaryStream = new ChunkedMemoryStream();
			this.summaryInformationWriter = new BinaryWriter(summaryStream);
			ChunkedMemoryStream docSummaryStream = new ChunkedMemoryStream();
			this.docSummaryInformationWriter = new BinaryWriter(docSummaryStream);
			return BeginDocument();
		}
		public void EndExport() {
			EndDocument();
			if(outputStream == null)
				throw new InvalidOperationException("BeginExport/EndExport calls consistency.");
			StructuredStorageWriter structuredStorageWriter = new StructuredStorageWriter();
			DevExpress.Utils.StructuredStorage.Internal.Writer.StorageDirectoryEntry directoryEntry = structuredStorageWriter.RootDirectoryEntry;
			directoryEntry.AddStreamDirectoryEntry("Workbook", workbookWriter.BaseStream);
			workbookWriter.Flush();
			if(summaryInformationWriter.BaseStream.Length > 0)
				directoryEntry.AddStreamDirectoryEntry("\x0005SummaryInformation", summaryInformationWriter.BaseStream);
			summaryInformationWriter.Flush();
			if(docSummaryInformationWriter.BaseStream.Length > 0)
				directoryEntry.AddStreamDirectoryEntry("\x0005DocumentSummaryInformation", docSummaryInformationWriter.BaseStream);
			docSummaryInformationWriter.Flush();
			structuredStorageWriter.Write(outputStream);
			outputStream = null;
			if(workbookWriter != null) {
				((IDisposable)workbookWriter).Dispose();
				workbookWriter = null;
			}
			if(workbookStream != null) {
				workbookStream.Dispose();
				workbookStream = null;
			}
			if(summaryInformationWriter != null) {
				((IDisposable)summaryInformationWriter).Dispose();
				summaryInformationWriter = null;
			}
			if(docSummaryInformationWriter != null) {
				((IDisposable)docSummaryInformationWriter).Dispose();
				docSummaryInformationWriter = null;
			}
			ClearSheets();
			this.sharedStrings.Clear();
			this.extendedSSTItems.Clear();
			this.drawingGroup.Clear();
		}
		public IXlFormulaEngine FormulaEngine { get { return this; } }
		CultureInfo CurrentCulture {
			get {
				if(options.Culture != null)
					return options.Culture;
				return CultureInfo.InvariantCulture;
			}
		}
		#endregion
		#region WriteContent
		protected void WriteContent(short recordType, IXlsContent content) {
			if(writer == null)
				return;
			if(content.RecordHeader != null)
				content.RecordHeader.RecordTypeId = recordType;
			writer.Write(recordType);
			writer.Write((short)content.GetSize());
			content.Write(writer);
		}
		protected void WriteContent(short recordType, bool content) {
			this.contentBoolValue.Value = content;
			WriteContent(recordType, this.contentBoolValue);
		}
		protected void WriteContent(short recordType, short content) {
			this.contentShortValue.Value = content;
			WriteContent(recordType, this.contentShortValue);
		}
		protected void WriteContent(short recordType, double content) {
			this.contentDoubleValue.Value = content;
			WriteContent(recordType, this.contentDoubleValue);
		}
		protected void WriteContent(short recordType, string content) {
			this.contentStringValue.Value = content;
			WriteContent(recordType, this.contentStringValue);
		}
		#endregion
		#region BOF/EOF
		protected void WriteBOF(XlsSubstreamType substreamType) {
			this.contentBOF.SubstreamType = substreamType;
			this.contentBOF.FileHistoryFlags = XlsFileHistory.Default;
			WriteContent(XlsRecordType.BOF, this.contentBOF);
		}
		protected void WriteEOF() {
			WriteContent(XlsRecordType.EOF, this.contentEmpty);
		}
		#endregion
		protected int RegisterString(string text) {
			this.sharedStringsRefCount++;
			return this.sharedStrings.RegisterString(text);
		}
		protected int RegisterString(XlRichTextString text) {
			this.sharedStringsRefCount++;
			int count = this.sharedStrings.Count;
			int index = this.sharedStrings.RegisterString(text);
			if(index == count) {
				foreach(XlRichTextRun textRun in text.Runs) {
					int fontId = Math.Max(0, RegisterFont(textRun.Font));
					if(fontId >= XlsDefs.UnusedFontIndex)
						fontId++;
					textRun.FontIndex = fontId;
				}
			}
			return index;
		}
	}
}
