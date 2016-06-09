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
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit;
using System.Runtime.InteropServices;
using DevExpress.Office.Import;
using DevExpress.Snap.Core.Native;
using DevExpress.Office;
using DevExpress.Snap.Core.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Import {
	public class SnapDocumentImporter : IImporter<DocumentFormat, bool> {
		#region IImporter<DocumentFormat,bool> Members
		public FileDialogFilter Filter { get { return SnapDocumentExporter.filter; } }
		public DocumentFormat Format { get { return SnapDocumentFormat.Snap; } }
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			SnapDocumentModel model = (SnapDocumentModel)documentModel;
			model.RaiseBeforeImport(Format, options);
			SnapImporter importer = new SnapImporter(model, (SnapDocumentImporterOptions)options);
			importer.Import(stream);
			return true;
		}
		public IImporterOptions SetupLoading() {
			return new SnapDocumentImporterOptions();
		}
		#endregion
	}
	#region SnapDocumentImporterOptions
	[ComVisible(true)]
	public class SnapDocumentImporterOptions : OpenXmlDocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return SnapDocumentFormat.Snap; } }
	}
	#endregion
}
