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
using DevExpress.Snap.Core;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.Snap.Core.Native;
using System.IO;
using DevExpress.Snap.Core.Export;
using DevExpress.Snap.Core.Import;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.API.Internal {
	public class SnapInternalAPI : InternalAPI {
		#region Fields
		byte[] cachedSnxContent;
		#endregion
		public SnapInternalAPI(SnapDocumentModel documentModel, IDocumentExportersFactory exportersFactory, IDocumentImportersFactory importersFactory)
			: base(documentModel, exportersFactory, importersFactory) {
		}
		#region Properties
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		#region Snx
		public byte[] SnxBytes {
			get {
				if (cachedSnxContent == null)
					cachedSnxContent = GetDocumentSnxContent();
				return cachedSnxContent;
			}
			set {
				if (cachedSnxContent != null && cachedSnxContent == value)
					return;
				if (value == null)
					Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentSnxContent);
			}
		}
		#endregion
		#endregion
		#region Snx
		public virtual void LoadDocumentSnxContent(Stream stream, SnapDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(SnapDocumentFormat.Snap, options);
			try {
				SnapImporter importer = new SnapImporter(DocumentModel, options);
				importer.Import(stream);
			} catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentSnxContent(Stream stream, SnapDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(SnapDocumentFormat.Snap, options);
			SnapExporter exporter = new SnapExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public byte[] GetDocumentSnxContent() {
			return GetDocumentSnxContent(null);
		}
		protected internal byte[] GetDocumentSnxContent(SnapDocumentExporterOptions options) {
			if (options == null) {
				options = new SnapDocumentExporterOptions();
				ApplyDefaultOptions(options);
				DocumentModel.RaiseBeforeExport(SnapDocumentFormat.Snap, options);
			}
			DocumentModel.PreprocessContentBeforeExport(SnapDocumentFormat.Snap);
			SnapExporter exporter = new SnapExporter(DocumentModel, options);
			using (MemoryStream stream = new MemoryStream()) {
				exporter.Export(stream);
				return stream.ToArray();
			}
		}
		public void SetDocumentSnxContent(byte[] content) {
			SnapDocumentImporterOptions options = new SnapDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(SnapDocumentFormat.Snap, options);
			SetDocumentSnxContent(content, options);
		}
		public void SetDocumentSnxContent(byte[] content, SnapDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentSnxContent(stream, options);
			}
		}
		#endregion
	}
}
