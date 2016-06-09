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
using System.ComponentModel;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Export;
using DevExpress.Office;
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.Spreadsheet;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Export {
	#region XlsDocumentExporterOptions
	public class XlsDocumentExporterOptions : DocumentExporterOptions {
		#region Fields
		string password = string.Empty;
		#endregion
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xls; } }
		protected internal bool ClipboardMode { get; set; }
		[Browsable(false)]
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("XlsDocumentExporterOptionsPassword"),
#endif
 DefaultValue("")]
		public string Password {
			get { return password; }
			set {
				if (value == null)
					value = string.Empty;
				if (password != value) {
					string oldPassword = password;
					password = value;
					OnChanged("Password", oldPassword, password);
				}
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			Password = string.Empty;
			ClipboardMode = false;
		}
		public override void CopyFrom(IExporterOptions value) {
			base.CopyFrom(value);
			XlsDocumentExporterOptions other = value as XlsDocumentExporterOptions;
			if (other != null) {
				Password = other.Password;
				ClipboardMode = other.ClipboardMode;
			}
		}
	}
	#endregion
	#region XlsDocumentExporter
	public class XlsDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_DocFiles), "xls");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xls; } } 
		public IExporterOptions SetupSaving() {
			return new XlsDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentXlsContent(stream, (XlsDocumentExporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
	#region XltDocumentExporterOptions
	public class XltDocumentExporterOptions : XlsDocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xlt; } }
	}
	#endregion
	#region XltDocumentExporter
	public class XltDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_XltFiles), "xlt");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xlt; } }
		public IExporterOptions SetupSaving() {
			return new XltDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.SaveDocumentXltContent(stream, (XltDocumentExporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
}
