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
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import.EPub;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Import {
	#region EPubDocumentImporter
	public class EPubDocumentImporter : IImporter<DocumentFormat, bool> {
		#region IDocumentImporter Members
		public FileDialogFilter Filter { get { return EPubDocumentExporter.filter; } }
		public DocumentFormat Format { get { return DocumentFormat.ePub; } }
		public IImporterOptions SetupLoading() {
			return new EPubDocumentImporterOptions();
		}
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.RaiseBeforeImport(Format, options);
			EPubImporter importer = new EPubImporter(model, (EPubDocumentImporterOptions)options);
			importer.Import(stream);
			return true;
		}
		#endregion
		public static void Register(IServiceProvider provider) {
			if (provider == null)
				return;
			IDocumentImportManagerService service = provider.GetService(typeof(IDocumentImportManagerService)) as IDocumentImportManagerService;
			if (service != null)
				service.RegisterImporter(new EPubDocumentImporter());
		}
	}
	#endregion
	#region EPubDocumentImporterOptions
	public class EPubDocumentImporterOptions : XmlBasedDocumentImporterOptions {		
		protected internal override DocumentFormat Format { get { return DocumentFormat.ePub; } }
		#region UpdateField
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new UpdateFieldOptions UpdateField { get { return base.UpdateField; } }
		#endregion
	}
	#endregion
}
