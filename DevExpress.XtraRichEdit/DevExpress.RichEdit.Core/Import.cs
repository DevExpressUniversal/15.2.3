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
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import {
}
namespace DevExpress.XtraRichEdit.Internal {
	#region DocumentImportHelper
	public class DocumentImportHelper : ImportHelper<DocumentFormat, bool> {
		public DocumentImportHelper(DocumentModel documentModel)
			: base(documentModel) {
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		protected override DocumentFormat UndefinedFormat { get { return DocumentFormat.Undefined; } }
		protected override DocumentFormat FallbackFormat { get { return DocumentFormat.PlainText; } }
		protected override IImporterOptions GetPredefinedOptions(DocumentFormat format) {
			return DocumentModel.DocumentImportOptions.GetOptions(format);
		}
		public override void ThrowUnsupportedFormatException() {
			throw new RichEditUnsupportedFormatException();
		}
		protected override void ApplyEncoding(IImporterOptions options, Encoding encoding) {
			DocumentImporterOptions documentImporterOptions = options as DocumentImporterOptions;
			if (documentImporterOptions != null)
				documentImporterOptions.ActualEncoding = encoding;
		}
	}
	#endregion
	#region RichEditImageImportHelper
	public class RichEditImageImportHelper : ImageImportHelper {
		public RichEditImageImportHelper(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void ThrowUnsupportedFormatException() {
			throw new RichEditUnsupportedFormatException();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	public class RichEditUnsupportedFormatException : Exception {
		public RichEditUnsupportedFormatException()
			: base(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_UnsupportedFormatException)) {
		}
	}
}
