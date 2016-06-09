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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Drawing;
namespace DevExpress.XtraPrinting.Export.Web {
	public class MhtDocumentBuilder : HtmlDocumentBuilder {
		string partId = String.Empty;
		DXHtmlTextWriter htmlWriter;
		protected override bool ShouldCreateCompressedWriter { get { return false; } }
		public MhtDocumentBuilder(MhtExportOptions options)
			: this(options, defaultBgColor) {
		}
		public MhtDocumentBuilder(MhtExportOptions options, Color bgColor)
			: base(options, bgColor) {
			partId = Guid.NewGuid().ToString();
		}
		void WriteMhtHeader(DXHtmlTextWriter writer) {
			writer.WriteLine("From: <Saved by " + AssemblyInfo.SRAssemblyPrinting + '>');
			writer.WriteLine("Subject: " + Title);
			writer.WriteLine("MIME-Version: 1.0");
			writer.WriteLine("Content-Type: multipart/related;");
			writer.WriteLine("    boundary=\"----=_NextPart_" + partId + "\";");
			writer.WriteLine("    type=\"text/html\"");
			writer.WriteLine();
			writer.WriteLine("This is a multi-part message in MIME format.");
			writer.WriteLine();
			writer.WriteLine("------=_NextPart_" + partId);
			writer.WriteLine("Content-Type: text/html;");
			writer.WriteLine("    charset=\"" + CharSet + "\"");
			writer.WriteLine("Content-Transfer-Encoding: base64");
			writer.WriteLine("Content-Location: file://");
			writer.WriteLine();
		}
		void WriteImage(DXHtmlTextWriter writer, ImageInfo imageInfo) {
			writer.WriteLine("------=_NextPart_" + partId);
			writer.WriteLine("Content-Type: image/" + HtmlImageHelper.GetMimeType(imageInfo.Image));
			writer.WriteLine("Content-Transfer-Encoding: base64");
			writer.WriteLine("Content-ID: <" + imageInfo.ContentId + ">");
			writer.WriteLine();
			writer.Write(Convert.ToBase64String(HtmlImageHelper.ImageToArray(imageInfo.Image)));
			writer.WriteLine();
			writer.WriteLine();
		}
		void WriteImages(DXHtmlTextWriter writer, MhtImageRepository imageRepository) {
			writer.WriteLine();
			writer.WriteLine();
			Dictionary<long, ImageInfo> images = imageRepository.ImagesTable;
			foreach(long key in images.Keys)
				WriteImage(writer, images[key]);
			writer.WriteLine("------=_NextPart_" + partId + "--");
			writer.WriteLine();
		}
		protected internal override IImageRepository CreateImageRepository(Stream stream) {
			return new MhtImageRepository();
		}
		public override void CreateDocumentCore(DXHtmlTextWriter writer, DXWebControlBase webControl, IImageRepository imageRepository) {
			WriteMhtHeader(writer);
			TextWriter streamWriter = new Base64Writer(writer);
			htmlWriter = Compressed ? new CompressedHtmlTextWriter(streamWriter) : new DXHtmlTextWriter(streamWriter);
			base.CreateDocumentCore(htmlWriter, webControl, imageRepository);
			htmlWriter.Close();
			htmlWriter = null;
			WriteImages(writer, (MhtImageRepository)imageRepository);
			writer.Flush();
		}
	}
}
