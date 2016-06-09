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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import {
	#region PlainTextDocumentImporter
	public class PlainTextDocumentImporter : IImporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_TextFiles), "txt");
		#region IDocumentImporter Members
		public virtual DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		public virtual FileDialogFilter Filter { get { return filter; } }
		public virtual IImporterOptions SetupLoading() {
			return new PlainTextDocumentImporterOptions();
		}
		public virtual bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentPlainTextContent(stream, (PlainTextDocumentImporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
}
