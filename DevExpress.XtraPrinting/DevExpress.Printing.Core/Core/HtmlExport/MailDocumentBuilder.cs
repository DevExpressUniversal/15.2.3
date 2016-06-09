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
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;
namespace DevExpress.XtraPrinting.Export.Web {
	public class MailDocumentBuilder : HtmlDocumentBuilder {
		#region static
		string partId = String.Empty;
		#endregion
		MailMessageExportOptions options;
		public MailDocumentBuilder(MailMessageExportOptions options)
			: this(options, defaultBgColor) {
		}
		public MailDocumentBuilder(MailMessageExportOptions options, Color bgColor)
			: base(options, bgColor) {
			partId = Guid.NewGuid().ToString();
			this.options = options;
		}
		public AlternateView CreateDocument(DevExpress.XtraPrinting.Document document, IImageRepository imageRepository, HtmlExportMode exportMode) {
			PSWebControlBase webControl = new PSWebControlMail(document, imageRepository, HtmlExportMode.SingleFilePageByPage, options.TableLayout);
			if(webControl != null) {
				StringWriter sw = new StringWriter();
				DXHtmlTextWriter writer = new DXHtmlTextWriter(sw);
				base.CreateDocumentCore(writer, webControl, imageRepository);
				AlternateView alternateView = AlternateView.CreateAlternateViewFromString(sw.ToString(), null, MediaTypeNames.Text.Html);
				Dictionary<long, ImageInfo> images = ((MailImageRepository)imageRepository).ImagesTable;
				foreach(long key in images.Keys)
					WriteImage(alternateView, writer, images[key]);
				return alternateView;
			}
			return null;
		}
		protected internal override IImageRepository CreateImageRepository(Stream stream) {
			return new MailImageRepository();
		}
		void WriteImage(AlternateView alternateView, DXHtmlTextWriter writer, ImageInfo imageInfo) {
			var ms = new MemoryStream(HtmlImageHelper.ImageToArray(imageInfo.Image));
			alternateView.LinkedResources.Add(new LinkedResource(ms,
				"image/" + HtmlImageHelper.GetMimeType(imageInfo.Image)) { ContentId = imageInfo.ContentId });
		}
	}
}
