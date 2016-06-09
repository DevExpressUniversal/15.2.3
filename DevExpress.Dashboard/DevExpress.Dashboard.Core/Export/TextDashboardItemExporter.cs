#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Printing;
using System;
using System.Text;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.DashboardExport {
	public class TextDashboardItemExporter : DashboardItemExporter {
		readonly InternalRichEditDocumentServer server = new InternalRichEditDocumentServer(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		readonly RichEditPrinter printer;
		public TextDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
			string base64Rtf = ((TextBoxDashboardItemViewModel)ServerData.ViewModel).Base64Rtf;
			ASCIIEncoding enc = new ASCIIEncoding();
			string rtfText = enc.GetString(Convert.FromBase64String(base64Rtf));
			server.Document.RtfText = rtfText;
			this.printer = new RichEditPrinter(server.InnerServer);
		}
		public override IPrintable PrintableComponent {
			get { return printer; }
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				server.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
