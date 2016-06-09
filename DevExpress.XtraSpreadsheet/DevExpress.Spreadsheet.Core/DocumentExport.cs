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
using System.Text;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.Spreadsheet;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region IDocumentExportManagerService
	public interface IDocumentExportManagerService : IExportManagerService<DocumentFormat, bool> {
	}
	#endregion
	#region DocumentExportManagerService
	public class DocumentExportManagerService : ExportManagerService<DocumentFormat, bool>, IDocumentExportManagerService {
		public DocumentExportManagerService()
			: base() {
		}
		protected override void RegisterNativeFormats() {
			RegisterExporter(new OpenXmlDocumentExporter());
			RegisterExporter(new XlsmDocumentExporter());
			RegisterExporter(new XlsDocumentExporter());
			RegisterExporter(new XltxDocumentExporter());
			RegisterExporter(new XltmDocumentExporter());
			RegisterExporter(new XltDocumentExporter());
			RegisterExporter(new TxtDocumentExporter());
			RegisterExporter(new CsvDocumentExporter());
			RegisterExporter(new HtmlDocumentExporter());
#if OPENDOCUMENT
			RegisterExporter(new OpenDocumentDocumentExporter());
#endif
		}
	}
	#endregion
	#region DocumentExportHelper
	public class DocumentExportHelper : ExportHelper<DocumentFormat, bool> {
		public DocumentExportHelper(IDocumentModel documentModel)
			: base(documentModel) {
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)DocumentModelBase; } }
		protected IDocumentModel DocumentModelBase { get { return base.DocumentModel; } }
		protected override IExporterOptions GetPredefinedOptions(DocumentFormat format) {
			return DocumentModel.DocumentExportOptions.GetOptions(format);
		}
		protected override void PreprocessContentBeforeExport(DocumentFormat format) {
		}
		protected override string GetFileNameForSaving() {
			WorkbookSaveOptions documentSaveOptions = DocumentModel.DocumentSaveOptions;
			if (!documentSaveOptions.CanSaveToCurrentFileName || String.IsNullOrEmpty(documentSaveOptions.CurrentFileName))
				return documentSaveOptions.DefaultFileName;
			else
				return documentSaveOptions.CurrentFileName;
		}
		protected override DocumentFormat GetCurrentDocumentFormat() {
			WorkbookSaveOptions documentSaveOptions = DocumentModel.DocumentSaveOptions;
			DocumentFormat documentFormat = documentSaveOptions.CurrentFormat;
			if (documentFormat == DocumentFormat.Undefined)
				return documentSaveOptions.DefaultFormat;
			else
				return documentFormat;
		}
		public override void ThrowUnsupportedFormatException() {
			throw new SpreadsheetUnsupportedFormatException();
		}
		protected override void ApplyEncoding(IExporterOptions options, Encoding encoding) {
			DocumentExporterOptions documentExporterOptions = options as DocumentExporterOptions;
			if (documentExporterOptions != null)
				documentExporterOptions.ActualEncoding = encoding;
		}
		protected override FileDialogFilterCollection CreateExportFilters(List<IExporter<DocumentFormat, bool>> exporters) {
			List<IExporter<DocumentFormat, bool>> cloneWithoutHtmlExporter = new List<IExporter<DocumentFormat, bool>>(exporters.ToArray());
			cloneWithoutHtmlExporter.RemoveAll(item => item.Format == DocumentFormat.Html);
			return base.CreateExportFilters(cloneWithoutHtmlExporter);
		}
	}
	#endregion
}
