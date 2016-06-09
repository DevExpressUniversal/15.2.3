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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Services;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Export.Html;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlExporter
	public class HtmlExporter : HtmlExporterBase, IHtmlExporter {
		#region Fields
		readonly HtmlContentExporter contentExporter;
		readonly HtmlDocumentExporterOptions options;
		readonly DocumentModel documentModel;
		#endregion
		public HtmlExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.options = options;
			Initialize(options.TargetUri, options.UriExportType == UriExportType.Absolute);
			this.contentExporter = CreateContentExporter(documentModel, options);
		}
		#region Properties
		protected override bool EmbedImages { get { return options.EmbedImages; } }
		protected override bool ExportToBodyTag { get { return options.ExportRootTag == ExportRootTag.Body; } }
		protected override bool ExportStylesAsStyleTag { get { return options.CssPropertiesExportType == CssPropertiesExportType.Style; } }
		protected override bool ExportStylesAsLink { get { return options.CssPropertiesExportType == CssPropertiesExportType.Link; } }
		protected override Encoding Encoding { get { return options.Encoding; } }
		protected HtmlContentExporter ContentExporter { get { return contentExporter; } }
		protected override bool UseHtml5 { get { return options.UseHtml5; } }
		#endregion
		protected internal virtual HtmlContentExporter CreateContentExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options) {
			return new HtmlContentExporter(documentModel, ScriptContainer, ImageRepository, options);
		}
		protected override void ExportBodyContent(DXWebControlBase root) {
			contentExporter.Export(root);
		}
		protected override void SetupBodyTag(DXWebControlBase body) {
			DXHtmlGenericControl bodyControl = (DXHtmlGenericControl)body;
			Color pageBackColor = documentModel.DocumentProperties.PageBackColor;
			if (!DXColor.IsEmpty(pageBackColor))
				bodyControl.Attributes.Add("bgcolor", HtmlConvert.ToHtml(pageBackColor));
			WebSettings webSettings = this.documentModel.WebSettings;
			if (webSettings.IsBodyMarginsSet()) {
				DXCssStyleCollection style = bodyControl.Style;
				Office.DocumentModelUnitConverter unitConverter = this.documentModel.UnitConverter;
				if (webSettings.LeftMargin != 0)
					style.Add(DXHtmlTextWriterStyle.MarginLeft, unitConverter.ModelUnitsToPixels(webSettings.LeftMargin) + "px");
				if (webSettings.TopMargin != 0)
					style.Add(DXHtmlTextWriterStyle.MarginTop, unitConverter.ModelUnitsToPixels(webSettings.TopMargin) + "px");
				if (webSettings.RightMargin != 0)
					style.Add(DXHtmlTextWriterStyle.MarginRight, unitConverter.ModelUnitsToPixels(webSettings.RightMargin) + "px");
				if (webSettings.BottomMargin != 0)
					style.Add(DXHtmlTextWriterStyle.MarginBottom, unitConverter.ModelUnitsToPixels(webSettings.BottomMargin) + "px");
			}
		}
	}
	#endregion
}
