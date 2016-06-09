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
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Windows.Forms;
using DevExpress.Office.Utils;
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	#region DocumentExportHelper
	public class DocumentExportHelper : ExportHelper<DocumentFormat, bool> {
		public DocumentExportHelper(DocumentModel documentModel)
			: base(documentModel) {
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		protected override IExporterOptions GetPredefinedOptions(DocumentFormat format) {
			return DocumentModel.DocumentExportOptions.GetOptions(format);
		}
		protected override void PreprocessContentBeforeExport(DocumentFormat format) {
			DocumentModel.PreprocessContentBeforeExport(format);
		}
		protected override string GetFileNameForSaving() {
			return GetFileNameForSavingCore(DocumentModel.DocumentSaveOptions);
		}
		protected override string GetFileNameForSaving(IDocumentSaveOptions<DocumentFormat> options) {
			return GetFileNameForSavingCore(options);
		}
		string GetFileNameForSavingCore(IDocumentSaveOptions<DocumentFormat> options) {
			DocumentSaveOptions documentSaveOptions = (DocumentSaveOptions)options;
			if (!documentSaveOptions.CanSaveToCurrentFileName || String.IsNullOrEmpty(documentSaveOptions.CurrentFileName))
				return documentSaveOptions.DefaultFileName;
			else
				return documentSaveOptions.CurrentFileName;
		}
		protected override DocumentFormat GetCurrentDocumentFormat() {
			return GetCurrentDocumentFormatCore(DocumentModel.DocumentSaveOptions);
		}
		protected override DocumentFormat GetCurrentDocumentFormat(IDocumentSaveOptions<DocumentFormat> options) {
			return GetCurrentDocumentFormatCore(options);
		}
		DocumentFormat GetCurrentDocumentFormatCore(IDocumentSaveOptions<DocumentFormat> options) {
			DocumentSaveOptions documentSaveOptions = (DocumentSaveOptions)options;
			DocumentFormat documentFormat = documentSaveOptions.CurrentFormat;
			if (documentFormat == DocumentFormat.Undefined)
				return documentSaveOptions.DefaultFormat;
			else
				return documentFormat;
		}
		public override void ThrowUnsupportedFormatException() {
			throw new RichEditUnsupportedFormatException();
		}
		protected override void ApplyEncoding(IExporterOptions options, Encoding encoding) {
			DocumentExporterOptions documentExporterOptions = options as DocumentExporterOptions;
			if (documentExporterOptions != null)
				documentExporterOptions.ActualEncoding = encoding;
		}
	}
	#endregion
}
