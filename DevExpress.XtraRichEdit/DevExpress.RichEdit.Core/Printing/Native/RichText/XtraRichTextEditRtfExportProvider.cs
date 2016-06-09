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
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Export;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native.RichText {
	public class XtraRichTextEditRtfExportProvider : RtfExportProviderBase {
		public static string CreateRtf(DocumentModel documentModel) {
			using (MemoryStream stream = new MemoryStream()) {
				CreateRtf(stream, documentModel);
				stream.Position = 0;
				StreamReader reader = new StreamReader(stream, EmptyEncoding.Instance);
				return reader.ReadToEnd();
			}
		}
		public static void CreateRtf(Stream stream, DocumentModel documentModel) {
			DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper rtfExportHelper = new DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper();
			RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
			options.WrapContentInGroup = true;
			RtfContentExporter rtfExporter = new RtfContentExporter(documentModel, options, rtfExportHelper);
			rtfExporter.Export();
			XtraRichTextEditRtfExportProvider provider = new XtraRichTextEditRtfExportProvider(stream, rtfExportHelper, rtfExporter.RtfBuilder.RtfContent.ToString());
			provider.Commit();
		}
		string rtfContent;
		public XtraRichTextEditRtfExportProvider(Stream stream, DevExpress.XtraPrinting.Export.Rtf.RtfExportHelper rtfExportHelper, string rtfContent)
			: base(stream, rtfExportHelper) {
			this.rtfContent = rtfContent;
		}
		protected override void WriteContent() {
			writer.WriteLine(rtfContent);
		}
	}
}
