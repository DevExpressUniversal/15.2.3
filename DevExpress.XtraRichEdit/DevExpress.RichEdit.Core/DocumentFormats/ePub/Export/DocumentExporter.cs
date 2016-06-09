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
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Export.EPub;
namespace DevExpress.XtraRichEdit.Export {
	#region EPubDocumentExporter
	public class EPubDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_ePubFiles), "epub");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.ePub; } }
		public IExporterOptions SetupSaving() {
			return new EPubDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.RaiseBeforeExport(Format, options);
			EPubExporter exporter = new EPubExporter(model, (EPubDocumentExporterOptions)options);
			exporter.Export(stream);
			model.RaiseAfterExport();
			return true;
		}
		#endregion
		public static void Register(IServiceProvider provider) {
			if (provider == null)
				return;
			IDocumentExportManagerService service = provider.GetService(typeof(IDocumentExportManagerService)) as IDocumentExportManagerService;
			if (service != null)
				service.RegisterExporter(new EPubDocumentExporter());
		}
	}
	#endregion
	#region EPubDocumentExporterOptions
	public class EPubDocumentExporterOptions : DocumentExporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.ePub; } }
	}
	#endregion
}
