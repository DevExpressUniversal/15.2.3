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
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Import {
	#region RichEditDocumentImportOptions
	public class RichEditDocumentImportOptions : RichEditNotificationOptions {
		#region Fields
		static readonly DocumentFormat fallbackFormatValue = DocumentFormat.PlainText;
		readonly RtfDocumentImporterOptions rtf;
		readonly PlainTextDocumentImporterOptions plainText;
		readonly HtmlDocumentImporterOptions html;
		readonly MhtDocumentImporterOptions mht;
		readonly OpenXmlDocumentImporterOptions openXml;
		readonly OpenDocumentImporterOptions openDocument;
		readonly WordMLDocumentImporterOptions wordML;
		readonly DocDocumentImporterOptions doc;
		DocumentFormat fallbackFormat;
		readonly Dictionary<DocumentFormat, DocumentImporterOptions> optionsTable;
		#endregion
		public RichEditDocumentImportOptions() {
			this.rtf = CreateRtfOptions();
			this.plainText = CreatePlainTextOptions();
			this.html = CreateHtmlOptions();
			this.mht = CreateMhtOptions();
			this.openXml = CreateOpenXmlOptions();
			this.openDocument = CreateOpenDocumentOptions();
			this.wordML = CreateWordMLOptions();
			this.doc = CreateDocOptions();
			this.optionsTable = new Dictionary<DocumentFormat, DocumentImporterOptions>();
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
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsRtf"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RtfDocumentImporterOptions Rtf { get { return rtf; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsPlainText"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PlainTextDocumentImporterOptions PlainText { get { return plainText; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsHtml"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlDocumentImporterOptions Html { get { return html; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsMht"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MhtDocumentImporterOptions Mht { get { return mht; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsOpenXml"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenXmlDocumentImporterOptions OpenXml { get { return openXml; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsOpenDocument"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OpenDocumentImporterOptions OpenDocument { get { return openDocument; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsWordML"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WordMLDocumentImporterOptions WordML { get { return wordML; } }
		[ NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocDocumentImporterOptions Doc { get { return doc; } }
		#region FallbackFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditDocumentImportOptionsFallbackFormat"),
#endif
 NotifyParentProperty(true)]
		public DocumentFormat FallbackFormat
		{
			get { return fallbackFormat; }
			set
			{
				if (fallbackFormat == value)
					return;
				DocumentFormat oldValue = fallbackFormat;
				fallbackFormat = value;
				OnChanged("FallbackFormat", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeFallbackFormat() {
			return FallbackFormat != fallbackFormatValue;
		}
		protected internal virtual void ResetFallbackFormat() {
			FallbackFormat = fallbackFormatValue;
		}
		#endregion
		protected internal Dictionary<DocumentFormat, DocumentImporterOptions> OptionsTable { get { return optionsTable; } }
		#endregion
		protected internal virtual void RegisterOptions(DocumentImporterOptions options) {
			optionsTable.Add(options.Format, options);
		}
		protected internal override void ResetCore() {
			if (optionsTable != null) {
				foreach (DocumentFormat key in optionsTable.Keys)
					optionsTable[key].Reset();
			}
			FallbackFormat = fallbackFormatValue;
		}
		protected internal virtual void CopyFrom(RichEditDocumentImportOptions options) {
			foreach (DocumentFormat key in optionsTable.Keys) {
				DocumentImporterOptions sourceOptions = options.GetOptions(key);
				if (sourceOptions != null)
					optionsTable[key].CopyFrom(sourceOptions);
			}
			FallbackFormat = options.FallbackFormat;
		}
		protected internal virtual DocumentImporterOptions GetOptions(DocumentFormat format) {
			DocumentImporterOptions result;
			if (optionsTable.TryGetValue(format, out result))
				return result;
			else
				return null;
		}
		protected internal virtual RtfDocumentImporterOptions CreateRtfOptions() {
			return new RtfDocumentImporterOptions();
		}
		protected internal virtual PlainTextDocumentImporterOptions CreatePlainTextOptions() {
			return new PlainTextDocumentImporterOptions();
		}
		protected internal virtual HtmlDocumentImporterOptions CreateHtmlOptions() {
			return new HtmlDocumentImporterOptions();
		}
		protected internal virtual MhtDocumentImporterOptions CreateMhtOptions() {
			return new MhtDocumentImporterOptions();
		}
		protected internal virtual OpenXmlDocumentImporterOptions CreateOpenXmlOptions() {
			return new OpenXmlDocumentImporterOptions();
		}
		protected internal virtual OpenDocumentImporterOptions CreateOpenDocumentOptions() {
			return new OpenDocumentImporterOptions();
		}
		protected internal virtual WordMLDocumentImporterOptions CreateWordMLOptions() {
			return new WordMLDocumentImporterOptions();
		}
		protected internal virtual DocDocumentImporterOptions CreateDocOptions() {
			return new DocDocumentImporterOptions();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region IDocumentImportManagerService
	public interface IDocumentImportManagerService : IImportManagerService<DocumentFormat, bool> {
	}
	#endregion
}
