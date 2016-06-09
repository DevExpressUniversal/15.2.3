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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.Compatibility.System;
namespace DevExpress.Office.Export.Html {
	public abstract class HtmlExporterBase {
		protected HtmlExporterBase(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.ServiceProvider = serviceProvider;
			this.StyleControl = new StyleWebControl();
		}
		#region Properties
		protected IServiceProvider ServiceProvider { get; private set; }
		protected StyleWebControl StyleControl { get; private set; }
		protected IScriptContainer ScriptContainer { get; private set; }
		protected IOfficeImageRepository ImageRepository { get; private set; }
		protected string FilesPath { get; private set; }
		protected string RelativeUri { get; private set; }
		protected abstract bool EmbedImages { get; }
		protected abstract bool ExportToBodyTag { get; }
		protected abstract bool ExportStylesAsStyleTag { get; }
		protected abstract bool ExportStylesAsLink { get; }
		protected abstract Encoding Encoding { get; }
		protected virtual bool UseHtml5 { get { return false; } }
		#endregion
		protected void Initialize(string targetUri, bool useAbsolutePath) {
			if (String.IsNullOrEmpty(targetUri))
				this.FilesPath = "_files/";
			else
				this.FilesPath = Path.GetDirectoryName(targetUri) + "/" + Path.GetFileNameWithoutExtension(targetUri) + "_files/";
			if (useAbsolutePath)
				this.RelativeUri = String.Empty;
			else
				this.RelativeUri = Path.GetFileNameWithoutExtension(targetUri) + "_files/";
			this.ScriptContainer = GetScriptContainer();
			this.ImageRepository = new ServiceBasedImageRepository(ServiceProvider, FilesPath, RelativeUri);
		}
		protected internal virtual IScriptContainer GetScriptContainer() {
			IScriptContainer scriptContainer = ServiceProvider.GetService(typeof(IScriptContainer)) as IScriptContainer;
			if (scriptContainer != null)
				return scriptContainer;
			else
				return StyleControl;
		}
		public string Export() {
			ChunkedStringBuilder result = new ChunkedStringBuilder();
			using (ChunkedStringBuilderWriter writer = new ChunkedStringBuilderWriter(result)) {
				Export(writer);
			}
			return result.ToString();
		}
		public virtual void Export(TextWriter writer) {
			if (EmbedImages) {
				IUriProvider uriProvider = new DataStringUriProvider();
				IUriProviderService service = ServiceProvider.GetService(typeof(IUriProviderService)) as IUriProviderService;
				if (service != null)
					service.RegisterProvider(uriProvider);
				try {
					ExportCore(writer);
				}
				finally {
					writer.Flush();
					if (service != null)
						service.UnregisterProvider(uriProvider);
				}
			}
			else
				ExportCore(writer);
			writer.Flush();
		}
		protected internal virtual void ExportCore(TextWriter writer) {
			using (DXHtmlTextWriter htmlWriter = new DXHtmlTextWriter(writer)) {
				DXWebControlBase body = ExportBodyControl();
				if (ExportToBodyTag)
					body.RenderControl(htmlWriter);
				else
					CreateHtmlDocument(htmlWriter, body);
			}
		}
		protected internal virtual void CreateHtmlDocument(DXHtmlTextWriter writer, DXWebControlBase body) {
			DXHtmlGenericControl html = new DXHtmlGenericControl(DXHtmlTextWriterTag.Html);
			if (!UseHtml5)
				html.Attributes.Add("xmlns", "http://www.w3.org/1999/xhtml");
			DXHtmlGenericControl head = new DXHtmlGenericControl(DXHtmlTextWriterTag.Head);
			html.Controls.Add(head);
			DXHtmlGenericControl meta = new DXHtmlGenericControl(DXHtmlTextWriterTag.Meta);
			meta.Attributes.Add("http-equiv", "Content-Type");
			meta.Attributes.Add("content", String.Format("text/html; charset={0}", Encoding.WebName));
			head.Controls.Add(meta);
			DXWebControlBase title = new DXHtmlGenericControl(DXHtmlTextWriterTag.Title);
			head.Controls.Add(title);
			if (ShouldWriteStyles())
				WriteStyles(head);
			html.Controls.Add(body);
			WriteHtmlDocumentPreamble(writer);
			writer.WriteLine();
			html.RenderControl(writer);
		}
		protected internal virtual void WriteHtmlDocumentPreamble(DXHtmlTextWriter writer) {
			if (UseHtml5)
				writer.Write("<!DOCTYPE html>");
			else
				writer.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
		}
		protected internal bool ShouldWriteStyles() {
			return StyleControl.Styles.Count > 0 || !Object.ReferenceEquals(ScriptContainer, StyleControl);
		}
		protected internal void WriteStyles(DXWebControlBase control) {
			if (ExportStylesAsStyleTag)
				control.Controls.Add(StyleControl);
			if (ExportStylesAsLink)
				WriteStyleLink(control);
		}
		private void WriteStyleLink(DXWebControlBase control) {
			string result = ExportCssProperiesToSeparateFile(StyleControl);
			DXHtmlGenericControl link = new DXHtmlGenericControl(DXHtmlTextWriterTag.Link);
			link.Attributes.Add("rel", "stylesheet");
			link.Attributes.Add("type", "text/css");
			link.Attributes.Add("href", result);
			control.Controls.Add(link);
		}
		protected internal string ExportCssProperiesToSeparateFile(StyleWebControl style) {
			StringBuilder sb = new StringBuilder();
			using (StringWriter writer = new StringWriter(sb)) {
				style.RenderStyles(writer);
				writer.Flush();
			}
			string styleText = sb.ToString();
			IUriProviderService service = ServiceProvider.GetService(typeof(IUriProviderService)) as IUriProviderService;
			if (service != null)
				return service.CreateCssUri(FilesPath, styleText, RelativeUri);
			return String.Empty;
		}
		protected virtual DXWebControlBase ExportBodyControl() {
			EmptyWebControl root = new EmptyWebControl();
			ExportBodyContent(root);
			DXWebControlBase body = new DXHtmlGenericControl(DXHtmlTextWriterTag.Body);
			SetupBodyTag(body);
			if (ExportToBodyTag && ShouldWriteStyles())
				WriteStyles(body);
			body.Controls.Add(root);
			return body;
		}
		protected virtual void SetupBodyTag(DXWebControlBase body) {
		}
		protected abstract void ExportBodyContent(DXWebControlBase root);
	}
}
