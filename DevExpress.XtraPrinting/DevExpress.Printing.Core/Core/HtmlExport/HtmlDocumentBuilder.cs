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
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Export.Web {
	public class HtmlDocumentBuilderBase {
		protected SortedList<string, string> startupList = new SortedList<string, string>();
		public void RegisterStartupScript(string key, string script) {
			if(key.Length > 0 && script.Length > 0)
				startupList.Add(key, script);
		}
		public virtual void CreateDocumentCore(DXHtmlTextWriter writer, DXWebControlBase webControl, IImageRepository imageRepository) {
			RenderControl(webControl, writer);
			WriteStartupScript(writer);
			writer.Flush();
		}
		protected virtual void WriteStartupScript(TextWriter writer) {
			foreach(string val in startupList.Values)
				writer.WriteLine(val);
		}
		protected virtual void RenderControl(DXWebControlBase control, DXHtmlTextWriter writer) {
			control.RenderControl(writer);
		}
	}
	public class HtmlDocumentBuilder : HtmlDocumentBuilderBase {
		#region static
		const string FilesDirSuffix = "_files";
		internal static ImageRepositoryBase CreatePSImageRepository(string filePath, bool embedImagesInHTML) {
			if(embedImagesInHTML)
				return new CssImageRepository();
			string rootPath = string.IsNullOrEmpty(filePath) ? string.Empty : Path.GetDirectoryName(filePath);
			string imagePath = Path.GetFileNameWithoutExtension(filePath) + FilesDirSuffix;
			DeleteExistingDirectory(Path.Combine(rootPath, imagePath));
			return new HtmlImageRepository(rootPath, imagePath);
		}
		static void DeleteExistingDirectory(string path) {
			if(Directory.Exists(path))
				Directory.Delete(path, true);
		}
		#endregion
		protected static readonly Color defaultBgColor = Color.White;
		HtmlExportOptionsBase options;
		Color bgColor = defaultBgColor;
		Margins bodyMargins;
		public Margins BodyMargins {
			get { return bodyMargins; }
			set { bodyMargins = value; }
		}
		protected string CharSet { get { return options.CharacterSet; } }
		protected string Title { get { return string.IsNullOrEmpty(options.Title) ? "XtraExport" : options.Title; } }
		protected bool Compressed { get { return options.RemoveSecondarySymbols; } }
		protected virtual bool ShouldCreateCompressedWriter { get { return Compressed; } }
		internal HtmlExportOptionsBase Options { get { return options; } }
		public HtmlDocumentBuilder(HtmlExportOptionsBase options)
			: this(options, defaultBgColor) {
		}
		public HtmlDocumentBuilder(HtmlExportOptionsBase options, Color bgColor) {
			this.options = options;
			this.bgColor = bgColor;
			bodyMargins = new Margins(10, 10, 10, 10);
		}
		public void CreateDocument(DevExpress.XtraPrinting.Document document, Stream stream, IImageRepository imageRepository, HtmlExportMode exportMode) {
			PSWebControlBase webControl = CreateWebControl(document, imageRepository, exportMode);
			if(webControl != null) {
				StreamWriter sw = new StreamWriter(stream);
				DXHtmlTextWriter writer = ShouldCreateCompressedWriter ? new CompressedHtmlTextWriter(sw) : new DXHtmlTextWriter(sw);
				CreateDocumentCore(writer, webControl, imageRepository);
			}
		}
		PSWebControlBase CreateWebControl(DevExpress.XtraPrinting.Document document, IImageRepository imageRepository, HtmlExportMode exportMode) {
			switch(options.ExportMode) {
				case HtmlExportMode.SingleFile:
					return new PSWebControl(document, imageRepository, exportMode, options.TableLayout);
				case HtmlExportMode.SingleFilePageByPage:
					return new PSMultiplePageWebControl(document, imageRepository, options, exportMode);
				default:
					return null;
			}
		}
		public override void CreateDocumentCore(DXHtmlTextWriter writer, DXWebControlBase webControl, IImageRepository imageRepository) {
			TextWriter innerWriter = writer.InnerWriter;
			string documentBody;
			using(StringWriter sw = new StringWriter()) {
				writer.InnerWriter = sw;
				base.CreateDocumentCore(writer, webControl, imageRepository);
				writer.Flush();
				documentBody = sw.ToString();
			}
			writer.InnerWriter = innerWriter;
			WriteHeader(writer, webControl as PSWebControlBase);
			writer.Write(documentBody);
			WriteFooter(writer);
			writer.Flush();
		}
		protected virtual void WriteHeader(DXHtmlTextWriter writer, PSWebControlBase psWebControl) {
			writer.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
			writer.WriteLine();
			writer.Write("<!-- saved from url=(0014)about:internet -->");
			writer.WriteLine();
			writer.WriteFullBeginTag("html");
			writer.WriteLine();
			writer.WriteFullBeginTag("head");
			writer.WriteLine();
			writer.Indent++;
			writer.WriteFullBeginTag("title");
			writer.Write(Title);
			writer.WriteEndTag("title");
			writer.WriteLine();
			if(!string.IsNullOrEmpty(CharSet)) {
				writer.Write(string.Format("<meta HTTP-EQUIV='Content-Type' CONTENT='text/html; charset={0}'/>", CharSet));
				writer.WriteLine();
			}
			if(psWebControl != null)
				psWebControl.Styles.RenderControl(writer);
			writer.Indent--;
			writer.WriteEndTag("head");
			writer.WriteLine();
			string bodyStyle = String.Format("body leftMargin={0} topMargin={1} rightMargin={2} bottomMargin={3}",
				bodyMargins.Left, bodyMargins.Top, bodyMargins.Right, bodyMargins.Bottom);
			if(options.ExportMode == HtmlExportMode.SingleFile)
				bodyStyle += String.Format(" style=\"background-color:{0}\"", HtmlConvert.ToHtml(bgColor));
			writer.WriteFullBeginTag(bodyStyle);
			writer.WriteLine();
		}
		protected virtual void WriteFooter(DXHtmlTextWriter writer) {
			writer.WriteEndTag("body");
			writer.WriteLine();
			writer.WriteEndTag("html");
		}
		protected internal virtual IImageRepository CreateImageRepository(Stream stream) {
			if(options.EmbedImagesInHTML || stream is FileStream) {
				string filePath = string.Empty;
				if(stream is FileStream)
					filePath = ((FileStream)stream).Name;
				return CreatePSImageRepository(filePath, options.EmbedImagesInHTML);
			}
			return new InMemoryHtmlImageRepository(FilesDirSuffix);
		}
	}
}
