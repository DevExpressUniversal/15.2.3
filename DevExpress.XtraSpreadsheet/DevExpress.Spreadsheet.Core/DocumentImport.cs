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
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region IDocumentImportManagerService
	public interface IDocumentImportManagerService : IImportManagerService<DocumentFormat, bool> {
	}
	#endregion
	#region DocumentImportManagerService
	public class DocumentImportManagerService : ImportManagerService<DocumentFormat, bool>, IDocumentImportManagerService {
		public DocumentImportManagerService()
			: base() {
		}
		protected override void RegisterNativeFormats() {
			RegisterImporter(new OpenXmlDocumentImporter());
			RegisterImporter(new XlsmDocumentImporter());
			RegisterImporter(new XlsDocumentImporter());
			RegisterImporter(new XltxDocumentImporter());
			RegisterImporter(new XltmDocumentImporter());
			RegisterImporter(new XltDocumentImporter());
			RegisterImporter(new TxtDocumentImporter());
			RegisterImporter(new CsvDocumentImporter());
#if OPENDOCUMENT
			RegisterImporter(new OpenDocumentImporter());
#endif
		}
	}
	#endregion
	#region DocumentImportHelper
	public class DocumentImportHelper : ImportHelper<DocumentFormat, bool> {
		public DocumentImportHelper(IDocumentModel documentModel)
			: base(documentModel) {
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)DocumentModelBase; } }
		protected IDocumentModel DocumentModelBase { get { return base.DocumentModel; } }
		protected override DocumentFormat UndefinedFormat { get { return DocumentFormat.Undefined; } }
		protected override DocumentFormat FallbackFormat { get { return DocumentFormat.OpenXml; } }
		protected override IImporterOptions GetPredefinedOptions(DocumentFormat format) {
			return DocumentModel.DocumentImportOptions.GetOptions(format);
		}
		public override void ThrowUnsupportedFormatException() {
			throw new SpreadsheetUnsupportedFormatException();
		}
		protected override void ApplyEncoding(IImporterOptions options, Encoding encoding) {
			DocumentImporterOptions documentImporterOptions = options as DocumentImporterOptions;
			if (documentImporterOptions != null)
				documentImporterOptions.ActualEncoding = encoding;
		}
	}
	#endregion
	#region SpreadsheetImageImportHelper
	public class SpreadsheetImageImportHelper : ImageImportHelper {
		public SpreadsheetImageImportHelper(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void ThrowUnsupportedFormatException() {
			throw new SpreadsheetUnsupportedFormatException();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet {
	public class SpreadsheetUnsupportedFormatException : Exception {
		public SpreadsheetUnsupportedFormatException()
			: base("unsupported") {
		}
	}
}
