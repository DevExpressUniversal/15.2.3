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
using System.Text;
using DevExpress.Office.Import;
using DevExpress.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Import {
	#region DocumentImporterOptions (abstract class)
	public abstract class DocumentImporterOptions : SpreadsheetNotificationOptions, IImporterOptions {
		#region Fields
		Encoding actualEncoding;
		string sourceUri;
		bool createEmptyDocumentOnLoadError;
		#endregion
		#region Properties
		#region ActualEncoding
		protected internal virtual Encoding ActualEncoding {
			get { return actualEncoding; }
			set {
				if (Object.Equals(actualEncoding, value))
					return;
				Encoding oldEncoding = actualEncoding;
				actualEncoding = value;
				OnChanged("ActualEncoding", oldEncoding, actualEncoding);
			}
		}
		#endregion
		#region SourceUri
		[Browsable(false)]
		public virtual string SourceUri { get { return sourceUri; } set { sourceUri = value; } }
		protected internal virtual bool ShouldSerializeSourceUri() {
			return false;
		}
		#endregion
		protected internal abstract DocumentFormat Format { get; }
		protected internal bool CreateEmptyDocumentOnLoadError { get { return createEmptyDocumentOnLoadError; } set { createEmptyDocumentOnLoadError = value; } }
		#endregion
		protected internal override void ResetCore() {
			this.CreateEmptyDocumentOnLoadError = true;
			this.actualEncoding = DXEncoding.Default;
			this.SourceUri = String.Empty;
		}
		public virtual void CopyFrom(IImporterOptions value) {
			DocumentImporterOptions options = value as DocumentImporterOptions;
			if (options != null) {
				this.ActualEncoding = options.ActualEncoding;
			}
		}
	}
	#endregion
	#region WorksheetNameValidationType
	public enum WorksheetNameValidationType {
		Check,
		AutoCorrect
	}
	#endregion
	#region WorkbookImportOptions
	public class WorkbookImportOptions : SpreadsheetNotificationOptions {
		#region Fields
		static readonly DocumentFormat fallbackFormatValue = DocumentFormat.OpenXml;
		static readonly WorksheetNameValidationType defaultWorksheetNameValidationType = WorksheetNameValidationType.Check;
		static bool defaultThrowExceptionOnInvalidDocument = false;
		readonly OpenXmlDocumentImporterOptions openXml;
		readonly XltxDocumentImporterOptions xltx;
		readonly XlsmDocumentImporterOptions xlsm;
		readonly XltmDocumentImporterOptions xltm;
#if OPENDOCUMENT
		readonly OpenDocumentImporterOptions openDocument;
#endif
		readonly XlsDocumentImporterOptions xls;
		readonly XltDocumentImporterOptions xlt;
		readonly CsvDocumentImporterOptions csv;
		readonly TxtDocumentImporterOptions text;
		DocumentFormat fallbackFormat;
		WorksheetNameValidationType worksheetNameValidationType;
		bool throwExceptionOnInvalidDocument;
		readonly Dictionary<DocumentFormat, DocumentImporterOptions> optionsTable;
		#endregion
		public WorkbookImportOptions() {
			this.openXml = CreateOpenXmlOptions();
			this.xltx = CreateXltxOptions();
			this.xlsm = CreateXlsmOptions();
			this.xltm = CreateXltmOptions();
#if OPENDOCUMENT
			this.openDocument = CreateOpenDocumentOptions();
#endif
			this.xls = CreateXlsOptions();
			this.xlt = CreateXltOptions();
			this.csv = CreateCsvOptions();
			this.text = CreateTxtOptions();
			this.optionsTable = new Dictionary<DocumentFormat, DocumentImporterOptions>();
			RegisterOptions(openXml);
			RegisterOptions(xltx);
			RegisterOptions(xlsm);
			RegisterOptions(xltm);
#if OPENDOCUMENT
			RegisterOptions(openDocument);
#endif
			RegisterOptions(xls);
			RegisterOptions(xlt);
			RegisterOptions(csv);
			RegisterOptions(text);
		}
		#region Properties
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsOpenXml"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenXmlDocumentImporterOptions OpenXml { get { return openXml; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXlsx"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public OpenXmlDocumentImporterOptions Xlsx { get { return openXml; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXltx"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltxDocumentImporterOptions Xltx { get { return xltx; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXlsm"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XlsmDocumentImporterOptions Xlsm { get { return xlsm; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXltm"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltmDocumentImporterOptions Xltm { get { return xltm; } }
#if OPENDOCUMENT
		[ NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenDocumentImporterOptions OpenDocument { get { return openDocument; } }
#endif
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXls"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XlsDocumentImporterOptions Xls { get { return xls; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsXlt"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltDocumentImporterOptions Xlt { get { return xlt; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsCsv"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CsvDocumentImporterOptions Csv { get { return csv; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsTxt"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TxtDocumentImporterOptions Txt { get { return text; } }
		#region FallbackFormat
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsFallbackFormat"),
#endif
 NotifyParentProperty(true)]
		public DocumentFormat FallbackFormat
		{
			get { return fallbackFormat; }
			set
			{
				if (fallbackFormat == value)
					return;
				DocumentFormat oldValue = fallbackFormat;
				fallbackFormat = value;
				OnChanged("FallbackFormat", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeFallbackFormat() {
			return FallbackFormat != fallbackFormatValue;
		}
		protected internal virtual void ResetFallbackFormat() {
			FallbackFormat = fallbackFormatValue;
		}
		#endregion
		#region WorksheetNameValidationType
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsWorksheetNameValidationType"),
#endif
 NotifyParentProperty(true)]
		public WorksheetNameValidationType WorksheetNameValidationType
		{
			get { return worksheetNameValidationType; }
			set
			{
				if (worksheetNameValidationType == value)
					return;
				WorksheetNameValidationType oldValue = worksheetNameValidationType;
				worksheetNameValidationType = value;
				OnChanged("WorksheetNameValidationType", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeWorksheetNameValidationType() {
			return WorksheetNameValidationType != defaultWorksheetNameValidationType;
		}
		protected internal virtual void ResetWorksheetNameValidationType() {
			WorksheetNameValidationType = defaultWorksheetNameValidationType;
		}
		#endregion
		#region ThrowExceptionOnInvalidDocument
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookImportOptionsThrowExceptionOnInvalidDocument"),
#endif
 NotifyParentProperty(true)]
		public bool ThrowExceptionOnInvalidDocument
		{
			get { return throwExceptionOnInvalidDocument; }
			set
			{
				if (throwExceptionOnInvalidDocument == value)
					return;
				throwExceptionOnInvalidDocument = value;
				OnChanged("ThrowExceptionOnInvalidDocument", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeThrowExceptionOnInvalidDocument() {
			return ThrowExceptionOnInvalidDocument != defaultThrowExceptionOnInvalidDocument;
		}
		protected internal virtual void ResetThrowExceptionOnInvalidDocument() {
			ThrowExceptionOnInvalidDocument = defaultThrowExceptionOnInvalidDocument;
		}
		#endregion
		protected internal Dictionary<DocumentFormat, DocumentImporterOptions> OptionsTable { get { return optionsTable; } }
		#endregion
		protected internal virtual void RegisterOptions(DocumentImporterOptions options) {
			optionsTable.Add(options.Format, options);
		}
		protected internal override void ResetCore() {
			if (optionsTable != null) {
				foreach (DocumentFormat key in optionsTable.Keys)
					optionsTable[key].Reset();
			}
			FallbackFormat = fallbackFormatValue;
			WorksheetNameValidationType = defaultWorksheetNameValidationType;
			ThrowExceptionOnInvalidDocument = defaultThrowExceptionOnInvalidDocument;
		}
		protected internal virtual void CopyFrom(WorkbookImportOptions options) {
			foreach (DocumentFormat key in optionsTable.Keys) {
				DocumentImporterOptions sourceOptions = options.GetOptions(key);
				if (sourceOptions != null)
					optionsTable[key].CopyFrom(sourceOptions);
			}
			FallbackFormat = options.FallbackFormat;
			WorksheetNameValidationType = options.WorksheetNameValidationType;
			ThrowExceptionOnInvalidDocument = options.ThrowExceptionOnInvalidDocument;
		}
		protected internal virtual DocumentImporterOptions GetOptions(DocumentFormat format) {
			DocumentImporterOptions result;
			if (optionsTable.TryGetValue(format, out result))
				return result;
			else
				return null;
		}
		protected internal virtual OpenXmlDocumentImporterOptions CreateOpenXmlOptions() {
			return new OpenXmlDocumentImporterOptions();
		}
		protected internal virtual XltxDocumentImporterOptions CreateXltxOptions() {
			return new XltxDocumentImporterOptions();
		}
		protected internal virtual XlsmDocumentImporterOptions CreateXlsmOptions() {
			return new XlsmDocumentImporterOptions();
		}
		protected internal virtual XltmDocumentImporterOptions CreateXltmOptions() {
			return new XltmDocumentImporterOptions();
		}
#if OPENDOCUMENT
		protected internal virtual OpenDocumentImporterOptions CreateOpenDocumentOptions() {
			return new OpenDocumentImporterOptions();
		}
#endif
		protected internal virtual XlsDocumentImporterOptions CreateXlsOptions() {
			return new XlsDocumentImporterOptions();
		}
		protected internal virtual XltDocumentImporterOptions CreateXltOptions() {
			return new XltDocumentImporterOptions();
		}
		protected internal virtual CsvDocumentImporterOptions CreateCsvOptions() {
			return new CsvDocumentImporterOptions();
		}
		protected internal virtual TxtDocumentImporterOptions CreateTxtOptions() {
			return new TxtDocumentImporterOptions();
		}
	}
	#endregion
}
