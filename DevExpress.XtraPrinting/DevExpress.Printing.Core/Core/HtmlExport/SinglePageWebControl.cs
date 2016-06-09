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

using System.Diagnostics;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
namespace DevExpress.Printing.Core.HtmlExport {
	public class SinglePageWebControl : PSWebControlBase {
		readonly int pageIndex;
		bool tableLayout;
		public SinglePageWebControl(Document document, IImageRepository imageRepository, int pageIndex)
			: this(document, imageRepository, pageIndex, true) {
		}
		public SinglePageWebControl(Document document, IImageRepository imageRepository, int pageIndex, bool tableLayout)
			: base(document, imageRepository, HtmlExportMode.SingleFile) {
			Debug.Assert(pageIndex >= 0 && pageIndex < document.Pages.Count);
			this.pageIndex = pageIndex;
			this.tableLayout = tableLayout;
		}
		protected virtual bool NeedClipMargins { get { return false; } }
		protected override void CreatePages() {
			using(var builder = CreateHtmlPageLayoutBuilder(document.Pages[pageIndex], htmlExportContext)) {
				DXHtmlDivision mainDiv = new DXHtmlDivision();
				DXHtmlTable table = CreateHtmlLayoutTable(builder, tableLayout) ?? new DXHtmlTable();
				PSPage page = document.Pages[pageIndex] as PSPage;
				if(PageHasWatermark(document, pageIndex)) {
					int topOffset = !NeedClipMargins ? 0 :
						System.Convert.ToInt32(GraphicsUnitConverter.Convert(page.MarginsF.Top - page.GetTopMarginOffset(), GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel));
					int watermarkHeight = System.Convert.ToInt32(GraphicsUnitConverter.Convert(NeedClipMargins ? page.GetClippedPageHeight() :
						document.Pages[pageIndex].PageSize.Height, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel));
					var watermark = CreateWatermark(document.Pages[pageIndex], htmlExportContext, topOffset, watermarkHeight, NeedClipMargins);
					watermark.CssClass = "page-watermark-container";
					mainDiv.Style["position"] = "relative";
					if(watermark != null)
						mainDiv.Controls.Add(watermark);
				}
				mainDiv.CssClass = "page-background-color-holder";
				mainDiv.Style.Add("background-color", HtmlConvert.ToHtml(htmlExportContext.PrintingSystem.Graph.PageBackColor));
				if(table != null)
					mainDiv.Controls.Add(table);
				AddChildrenControl(mainDiv);
			}
		}
		protected override void AddControlsBeforeCreatePages() {
		}
		protected override void AddControlsAfterCreatePages() {
			Controls.Add(styleControl);
			Controls.Add(scriptControl);
		}
		protected virtual HtmlPageLayoutBuilder CreateHtmlPageLayoutBuilder(Page page, HtmlExportContext htmlExportContext) {
			return new HtmlPageLayoutBuilder(page, htmlExportContext);
		}
	}
}
