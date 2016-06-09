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
using System.Linq;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Export {
	#region SnapDocumentExporter
	public class SnapDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(SnapLocalizer.GetString(SnapStringId.FileFilterDescription_SnapFiles), "snx");
		#region IExporter<DocumentFormat,bool> Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return SnapDocumentFormat.Snap; } }
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			SnapDocumentModel model = (SnapDocumentModel)documentModel;
			model.RaiseBeforeExport(Format, options);
			if(((SnapDocumentExporterOptions)options).Format == SnapDocumentFormat.Snap)
				model.ActivePieceTable.UpdateTemplate(false);
			SnapExporter exporter = new SnapExporter(model, (SnapDocumentExporterOptions)options);
			exporter.Export(stream);
			return true;
		}
		public IExporterOptions SetupSaving() {
			return new SnapDocumentExporterOptions();
		}
		#endregion
	}
	#endregion
	#region PdfDocumentExporter
	public class PdfDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_PDFFiles), "pdf");
		#region IExporter<DocumentFormat,bool> Members
		public FileDialogFilter Filter {
			get { return filter; }
		}
		public DocumentFormat Format {
			get { return SnapDocumentFormat.Pdf; }
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			SnapDocumentModel model = (SnapDocumentModel)documentModel;
			model.RaiseBeforeExport(Format, options);
			PdfExporter exporter = new PdfExporter(model);
			exporter.Export(stream);
			return true;
		}
		public IExporterOptions SetupSaving() {
			return new PdfDocumentExporterOptions();
		}
		#endregion
	}
	#endregion
	#region SnapDocumentExporterOptions
	[ComVisible(true)]
	public class SnapDocumentExporterOptions : OpenXmlDocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return SnapDocumentFormat.Snap; } }
	}
	#endregion
	#region PdfDocumentExporterOptions
	[ComVisible(true)]
	public class PdfDocumentExporterOptions : DocumentExporterOptions {
		protected internal override DocumentFormat Format {
			get { return SnapDocumentFormat.Pdf; }
		}
	}
	#endregion
}
