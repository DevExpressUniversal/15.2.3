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

using DevExpress.XtraRichEdit.Internal;
using DevExpress.Snap.Core.Export;
using System.Collections.Generic;
using DevExpress.Office.Internal;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Native.Services {
	public class SnapDocumentExportManagerService : DocumentExportManagerService {
		public IExporter<XtraRichEdit.DocumentFormat, bool> SnapExporter { get; private set; }
		protected override void RegisterNativeFormats() {
			base.RegisterNativeFormats();
#if !SL
			RegisterExporter(new PdfDocumentExporter());
#endif
			SnapExporter = CreateSaveExporter();
		}
		protected virtual IExporter<XtraRichEdit.DocumentFormat, bool> CreateSaveExporter() {
			return new SnapDocumentExporter();
		}
		public override IExporter<XtraRichEdit.DocumentFormat, bool> GetExporter(XtraRichEdit.DocumentFormat format) {
			if (format == SnapDocumentFormat.Snap)
				return SnapExporter;
			return base.GetExporter(format);
		}
		public override List<IExporter<XtraRichEdit.DocumentFormat, bool>> GetExporters() {
			return new List<IExporter<XtraRichEdit.DocumentFormat, bool>>() { SnapExporter };
		}
		public virtual List<IExporter<XtraRichEdit.DocumentFormat, bool>> GetDefaultExporters() {
			return base.GetExporters();
		}
	}
}
namespace DevExpress.Snap.Core.Native {
	public static class SnapDocumentFormatsDependecies {
		public static DocumentFormatsDependencies CreateDocumentFormatsDependencies() {
			IDocumentExportManagerService exportManagerService = new SnapDocumentExportManagerService();
			IDocumentImportManagerService importManagerService = new SnapDocumentImportManagerService();
			IDocumentExportersFactory exportersFactory = new DocumentExportersFactory();
			IDocumentImportersFactory importersFactory = new DocumentImportersFactory();
			return new DocumentFormatsDependencies(exportManagerService, importManagerService, exportersFactory, importersFactory);
		}
	}
}
