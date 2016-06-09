﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Office.Internal;
using DevExpress.Office.Export;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Export {
	#region XlsmDocumentExporterOptions
	public class XlsmDocumentExporterOptions : OpenXmlDocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xlsm; } }
	}
	#endregion
	#region XlsmDocumentExporter
	public class XlsmDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_XlsmFiles), "xlsm");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xlsm; } }
		public IExporterOptions SetupSaving() {
			return new XlsmDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentXlsmContent(stream, (XlsmDocumentExporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
	#region XltmDocumentExporterOptions
	public class XltmDocumentExporterOptions : XlsmDocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xltm; } }
	}
	#endregion
	#region XltmDocumentExporter
	public class XltmDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_XltmFiles), "xltm");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xltm; } }
		public IExporterOptions SetupSaving() {
			return new XltmDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentXltmContent(stream, (XltmDocumentExporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
}
