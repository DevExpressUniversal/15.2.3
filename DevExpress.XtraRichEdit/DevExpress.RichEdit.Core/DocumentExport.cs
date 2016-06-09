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
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Export {
	#region RichEditDocumentExportOptions
	[ComVisible(true)]
	public class RichEditDocumentExportOptions : RichEditNotificationOptions {
		#region Fields
		readonly RtfDocumentExporterOptions rtf;
		readonly PlainTextDocumentExporterOptions plainText;
		readonly HtmlDocumentExporterOptions html;
		readonly MhtDocumentExporterOptions mht;
		readonly OpenXmlDocumentExporterOptions openXml;
		readonly OpenDocumentExporterOptions openDocument;
		readonly WordMLDocumentExporterOptions wordML;
		readonly DocDocumentExporterOptions doc;
		readonly Dictionary<DocumentFormat, DocumentExporterOptions> optionsTable;
		#endregion
		public RichEditDocumentExportOptions() {
			this.rtf = CreateRtfOptions();
			this.plainText = CreatePlainTextOptions();
			this.html = CreateHtmlOptions();
			this.mht = CreateMhtOptions();
			this.openXml = CreateOpenXmlOptions();
			this.openDocument = CreateOpenDocumentOptions();
			this.wordML = CreateWordMLOptions();
			this.doc = CreateDocOptions();
			this.optionsTable = new Dictionary<DocumentFormat, DocumentExporterOptions>();
			RegisterOptions(rtf);
			RegisterOptions(plainText);
			RegisterOptions(html);
			RegisterOptions(mht);
			RegisterOptions(openXml);
			RegisterOptions(openDocument);
			RegisterOptions(wordML);
			RegisterOptions(doc);
		}
		#region Properties
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsRtf"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RtfDocumentExporterOptions Rtf { get { return rtf; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsPlainText"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PlainTextDocumentExporterOptions PlainText { get { return plainText; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsHtml"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlDocumentExporterOptions Html { get { return html; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsMht"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MhtDocumentExporterOptions Mht { get { return mht; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsOpenXml"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenXmlDocumentExporterOptions OpenXml { get { return openXml; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsOpenDocument"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenDocumentExporterOptions OpenDocument { get { return openDocument; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsWordML"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WordMLDocumentExporterOptions WordML { get { return wordML; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentExportOptionsDoc"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocDocumentExporterOptions Doc { get { return doc; } }
		protected internal Dictionary<DocumentFormat, DocumentExporterOptions> OptionsTable { get { return optionsTable; } }
		#endregion
		protected internal virtual void RegisterOptions(DocumentExporterOptions options) {
			optionsTable.Add(options.Format, options);
		}
		protected internal override void ResetCore() {
			if (optionsTable != null) {
				foreach (DocumentFormat key in optionsTable.Keys)
					optionsTable[key].Reset();
			}
		}
		protected internal virtual void CopyFrom(RichEditDocumentExportOptions options) {
			foreach (DocumentFormat key in optionsTable.Keys) {
				DocumentExporterOptions sourceOptions = options.GetOptions(key);
				if (sourceOptions != null)
					optionsTable[key].CopyFrom(sourceOptions);
			}
		}
		protected internal virtual DocumentExporterOptions GetOptions(DocumentFormat format) {
			DocumentExporterOptions result;
			if (optionsTable.TryGetValue(format, out result))
				return result;
			else
				return null;
		}
		protected internal virtual RtfDocumentExporterOptions CreateRtfOptions() {
			return new RtfDocumentExporterOptions();
		}
		protected internal virtual PlainTextDocumentExporterOptions CreatePlainTextOptions() {
			return new PlainTextDocumentExporterOptions();
		}
		protected internal virtual HtmlDocumentExporterOptions CreateHtmlOptions() {
			return new HtmlDocumentExporterOptions();
		}
		protected internal virtual MhtDocumentExporterOptions CreateMhtOptions() {
			return new MhtDocumentExporterOptions();
		}
		protected internal virtual OpenXmlDocumentExporterOptions CreateOpenXmlOptions() {
			return new OpenXmlDocumentExporterOptions();
		}
		protected internal virtual OpenDocumentExporterOptions CreateOpenDocumentOptions() {
			return new OpenDocumentExporterOptions();
		}
		protected internal virtual WordMLDocumentExporterOptions CreateWordMLOptions() {
			return new WordMLDocumentExporterOptions();
		}
		protected internal virtual DocDocumentExporterOptions CreateDocOptions() {
			return new DocDocumentExporterOptions();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region IDocumentExportManagerService
	public interface IDocumentExportManagerService : IExportManagerService<DocumentFormat, bool> {
	}
	#endregion
}
