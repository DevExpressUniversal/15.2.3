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
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Export {
	#region DocumentExporterOptions (abstract class)
	public abstract class DocumentExporterOptions : SpreadsheetNotificationOptions, IExporterOptions {
		#region Fields
		Encoding actualEncoding;
		string targetUri;
		#endregion
		protected DocumentExporterOptions() {
		}
		#region Properties
		#region ActualEncoding
		protected internal virtual Encoding ActualEncoding {
			get { return actualEncoding; }
			set {
				if (Object.Equals(actualEncoding, value))
					return;
				Encoding oldEncoding = actualEncoding;
				actualEncoding = value;
				OnChanged("Encoding", oldEncoding, actualEncoding);
			}
		}
		#endregion
		#region TargetUri
		[Browsable(false)]
		public string TargetUri { get { return targetUri; } set { targetUri = value; } }
		protected internal virtual bool ShouldSerializeTargetUri() {
			return false;
		}
		#endregion
		protected internal abstract DocumentFormat Format { get; }
		#endregion
		protected internal override void ResetCore() {
			this.actualEncoding = DXEncoding.Default;
			this.TargetUri = String.Empty;
		}
		public virtual void CopyFrom(IExporterOptions value) {
			DocumentExporterOptions options = value as DocumentExporterOptions;
			if (options != null) {
				this.ActualEncoding = options.ActualEncoding;
			}
		}
	}
	#endregion
	#region WorkbookExportOptions
	public class WorkbookExportOptions : SpreadsheetNotificationOptions {
		#region Fields
		readonly OpenXmlDocumentExporterOptions openXml;
		readonly XltxDocumentExporterOptions xltx;
		readonly XlsmDocumentExporterOptions xlsm;
		readonly XltmDocumentExporterOptions xltm;
		readonly XlsDocumentExporterOptions xls;
		readonly XltDocumentExporterOptions xlt;
		readonly CsvDocumentExporterOptions csv;
		readonly TxtDocumentExporterOptions txt;
		readonly HtmlDocumentExporterOptions html;
		readonly Dictionary<DocumentFormat, DocumentExporterOptions> optionsTable;
		CustomFunctionExportMode customFunctionExportMode;
		#endregion
		public WorkbookExportOptions() {
			this.openXml = CreateOpenXmlOptions();
			this.xltx = CreateXltxOptions();
			this.xlsm = CreateXlsmOptions();
			this.xltm = CreateXltmOptions();
			this.xls = CreateXlsOptions();
			this.xlt = CreateXltOptions();
			this.csv = CreateCsvOptions();
			this.txt = CreateTextOptions();
			this.html = CreateHtmlOptions();
			this.optionsTable = new Dictionary<DocumentFormat, DocumentExporterOptions>();
			RegisterOptions(openXml);
			RegisterOptions(xltx);
			RegisterOptions(xlsm);
			RegisterOptions(xltm);
			RegisterOptions(xls);
			RegisterOptions(xlt);
			RegisterOptions(csv);
			RegisterOptions(txt);
			RegisterOptions(html);
			customFunctionExportMode = CustomFunctionExportMode.Function;
		}
		#region Properties
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsOpenXml"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenXmlDocumentExporterOptions OpenXml { get { return openXml; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXlsx"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public OpenXmlDocumentExporterOptions Xlsx { get { return openXml; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXltx"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltxDocumentExporterOptions Xltx { get { return xltx; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXlsm"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XlsmDocumentExporterOptions Xlsm { get { return xlsm; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXltm"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltmDocumentExporterOptions Xltm { get { return xltm; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXls"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XlsDocumentExporterOptions Xls { get { return xls; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsXlt"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public XltDocumentExporterOptions Xlt { get { return xlt; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsCsv"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CsvDocumentExporterOptions Csv { get { return csv; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsTxt"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TxtDocumentExporterOptions Txt { get { return txt; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal HtmlDocumentExporterOptions Html { get { return html; } }
		protected internal Dictionary<DocumentFormat, DocumentExporterOptions> OptionsTable { get { return optionsTable; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookExportOptionsCustomFunctionExportMode"),
#endif
		DefaultValue(CustomFunctionExportMode.Function), NotifyParentProperty(true)]
		public CustomFunctionExportMode CustomFunctionExportMode
		{
			get { return customFunctionExportMode; }
			set
			{
				if (this.customFunctionExportMode == value)
					return;
				CustomFunctionExportMode oldValue = this.customFunctionExportMode;
				this.customFunctionExportMode = value;
				OnChanged("CustomFunctionExportMode", oldValue, value);
			}
		}
		#endregion
		protected internal virtual void RegisterOptions(DocumentExporterOptions options) {
			optionsTable.Add(options.Format, options);
		}
		protected internal override void ResetCore() {
			if (optionsTable != null) {
				foreach (DocumentFormat key in optionsTable.Keys)
					optionsTable[key].Reset();
			}
			customFunctionExportMode = Export.CustomFunctionExportMode.Function;
		}
		protected internal virtual void CopyFrom(WorkbookExportOptions options) {
			foreach (DocumentFormat key in optionsTable.Keys) {
				DocumentExporterOptions sourceOptions = options.GetOptions(key);
				if (sourceOptions != null)
					optionsTable[key].CopyFrom(sourceOptions);
			}
			customFunctionExportMode = options.customFunctionExportMode;
		}
		protected internal virtual OpenXmlDocumentExporterOptions CreateOpenXmlOptions() {
			return new OpenXmlDocumentExporterOptions();
		}
		protected internal virtual XltxDocumentExporterOptions CreateXltxOptions() {
			return new XltxDocumentExporterOptions();
		}
		protected internal virtual XlsmDocumentExporterOptions CreateXlsmOptions() {
			return new XlsmDocumentExporterOptions();
		}
		protected internal virtual XltmDocumentExporterOptions CreateXltmOptions() {
			return new XltmDocumentExporterOptions();
		}
		protected internal virtual XlsDocumentExporterOptions CreateXlsOptions() {
			return new XlsDocumentExporterOptions();
		}
		protected internal virtual XltDocumentExporterOptions CreateXltOptions() {
			return new XltDocumentExporterOptions();
		}
		protected internal virtual CsvDocumentExporterOptions CreateCsvOptions() {
			return new CsvDocumentExporterOptions();
		}
		protected internal virtual TxtDocumentExporterOptions CreateTextOptions() {
			TxtDocumentExporterOptions result = new TxtDocumentExporterOptions();
			result.Reset();
			return result;
		}
		protected internal virtual HtmlDocumentExporterOptions CreateHtmlOptions() {
			return new HtmlDocumentExporterOptions();
		}
		protected internal virtual DocumentExporterOptions GetOptions(DocumentFormat format) {
			DocumentExporterOptions result;
			if (optionsTable.TryGetValue(format, out result))
				return result;
			else
				return null;
		}
	}
	#endregion
	public enum CustomFunctionExportMode {
		CalculatedValue,
		Function
	}
}
