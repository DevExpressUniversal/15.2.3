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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.Drawing;
using System.Drawing.Printing;
namespace DevExpress.XtraPrinting.Export.Web {
	public abstract class PSWebControlBase : DXHtmlGenericControl {
		#region static
		protected static bool PageHasWatermark(Document document, int pageIndex) {
			return (document.Pages[pageIndex].ActualWatermark.Image != null || !string.IsNullOrEmpty(document.Pages[pageIndex].ActualWatermark.Text)) &&
					document.PrintingSystem.Watermark.NeedDraw(pageIndex, document.PageCount);
		}
		protected static void AddPageControls(PSWebControlBase dest, HtmlExportOptionsBase options) {
			XtraPrinting.Document document = dest.document;
			int[] pageIndices = ExportOptionsHelper.GetPageIndices(options, document.PageCount);
			for(int i = 0; i < pageIndices.Length; i++) {
				DXHtmlDivision divControl = new DXHtmlDivision();
				Page page = document.Pages[pageIndices[i]];
				float widthPreviousPage = i == 0 ? 0 : document.Pages[pageIndices[i - 1]].PageSize.Width;
				float widthNextPage = i == pageIndices.Length - 1 ? 0 : document.Pages[pageIndices[i + 1]].PageSize.Width;
				BorderSide borderSide = BorderSide.Left | BorderSide.Right;
				if(widthPreviousPage <= page.PageSize.Width)
					borderSide |= BorderSide.Top;
				if(widthNextPage < page.PageSize.Width)
					borderSide |= BorderSide.Bottom;
				string style = PSHtmlStyleRender.GetBorderHtml(options.PageBorderColor, Color.Transparent, borderSide, options.PageBorderWidth);
				style += "background-color: " + HtmlConvert.ToHtml(page.Document.PrintingSystem.Graph.PageBackColor);
				divControl.Attributes["style"] = style;
				Size pageSize = GraphicsUnitConverter.Convert(Size.Round(page.PageSize), GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
				HtmlHelper.SetStyleSize(divControl.Style, pageSize);
				if(options.ExportWatermarks && PageHasWatermark(document, i))
					divControl.Controls.Add(CreateWatermark(page, dest.htmlExportContext, 0, pageSize.Height, false));
				using(HtmlPageLayoutBuilder builder = new HtmlPageLayoutBuilder(page, dest.htmlExportContext)) {
					DXHtmlTable table = dest.CreateHtmlLayoutTable(builder, options.TableLayout);
					if(table != null)
						divControl.Controls.Add(table);
				}
				dest.AddChildrenControl(divControl);
			}
		}
		protected static DXHtmlDivision CreateWatermark(Page page, HtmlExportContext context, int topOffset, int pageHeight, bool needClipMargins) {
			DXHtmlDivision watermark = new DXHtmlDivision();
			Size pageSize = GraphicsUnitConverter.Convert(Size.Round(page.PageSize), GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
			int pageWidth = needClipMargins ? pageSize.Width - GraphicsUnitConverter.Convert(page.Margins.Left + page.Margins.Right, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.DeviceIndependentPixel) : 
				pageSize.Width; 
			watermark.Attributes["style"] = String.Format("width:{0}px;height:{1}px;position:absolute;overflow:hidden;z-index:{2};",
				pageWidth, pageHeight, page.ActualWatermark.ShowBehind ? "0" : "1");
			if(!page.ActualWatermark.ShowBehind) {
				watermark.Attributes.Add("onmousedown", HtmlHelper.WatermarkMouseDownScript);
			}
			Point offset = new Point(GraphicsUnitConverter.Convert(page.Margins.Left, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.DeviceIndependentPixel), topOffset);
			if(page.ActualWatermark.Image != null)
				watermark.Controls.Add(CreateImageWatermark(page, pageSize, context, offset, needClipMargins));
			if(!string.IsNullOrEmpty(page.ActualWatermark.Text))
				watermark.Controls.Add(CreateTextWatermark(page, pageSize, context, offset, needClipMargins));
			return watermark;
		}
		protected static DXHtmlDivision CreateTextWatermark(Page page, Size pageSize, HtmlExportContext htmlExportContext, Point offset, bool needClipMargins) {
			PageWatermark pageWatermark = page.ActualWatermark;
			Size textSize = Size.Round(htmlExportContext.Measurer.MeasureString(pageWatermark.Text, pageWatermark.Font, GraphicsUnit.Pixel));
			string watermarkStyle = PSHtmlStyleRender.GetHtmlWatermarkTextStyle(pageSize, offset, needClipMargins, textSize, pageWatermark);
			string watermarkTextCss = htmlExportContext.ScriptContainer.RegisterCssClass(watermarkStyle);
			DXHtmlDivision textDivControl = new DXHtmlDivision();
			textDivControl.Attributes["class"] = watermarkTextCss;
			textDivControl.Controls.Add(new DXHtmlLiteralControl(pageWatermark.Text));
			return textDivControl;
		}
		protected static DXHtmlDivision CreateImageWatermark(Page page, Size pageSize, HtmlExportContext htmlExportContext, Point offset, bool needClipMargins) {
			PageWatermark pageWatermark = page.ActualWatermark;
			string imageSrc = htmlExportContext.HtmlCellImageContentCreator.GetWatermarkImageSrc(pageWatermark.Image);
			string watermarkStyle = PSHtmlStyleRender.GetHtmlWatermarkImageStyle(pageSize, offset, needClipMargins, imageSrc, pageWatermark);
			string watermarkImageCss = htmlExportContext.ScriptContainer.RegisterCssClass(watermarkStyle);
			DXHtmlDivision imageDivControl = new DXHtmlDivision();
			imageDivControl.Attributes["class"] = watermarkImageCss;
			return imageDivControl;
		}
		const string cssClass = "pagebreak";
		public static DXHtmlControl CreatePageBreaker(WebStyleControl styles) {
			DXHtmlDivision ctl = new DXHtmlDivision();
			styles.AddStyle("page-break-after:always;height:0px;width:0px;overflow:hidden;font-size:0px;line-height:0px;", cssClass);
			ctl.CssClass = cssClass;
			return ctl;
		}
		#endregion
		protected Document document;
		protected WebStyleControl styleControl;
		protected ScriptBlockControl scriptControl;
		protected HtmlExportContext htmlExportContext;
		protected ProgressReflector ProgressReflector {
			get { return document.ProgressReflector; }
		}
		public WebStyleControl Styles {
			get { return styleControl; }
		}
		protected PSWebControlBase(Document document, IImageRepository imageRepository, HtmlExportMode exportMode)
			: base(DXHtmlTextWriterTag.Unknown) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(imageRepository, "imageRepository");
			this.document = document;
			styleControl = new WebStyleControl();
			scriptControl = CreateScriptControl(styleControl);
			if(imageRepository is CssImageRepository)
				((CssImageRepository)imageRepository).ScriptContainer = scriptControl;
			htmlExportContext = CreateExportContext(document.PrintingSystem, scriptControl, imageRepository, exportMode);
		}
		protected internal override void Render(DXHtmlTextWriter writer) {
			CreateChildControls();
			base.RenderContents(writer);
		}
		protected internal override void CreateChildControls() {
			if(ChildControlsCreated)
				return;
			ChildControlsCreated = true;
			CreateChildControlsCore();
		}
		protected virtual void CreateChildControlsCore() {
			Controls.Clear();
			if(document == null)
				return;
			styleControl.ClearContent();
			scriptControl.ClearContent();
			AddControlsBeforeCreatePages();
			CreatePages();
			AddControlsAfterCreatePages();
		}
		protected virtual void AddControlsBeforeCreatePages() {
			Controls.Add(scriptControl);
		}
		protected virtual void AddControlsAfterCreatePages() {
		}
		protected abstract void CreatePages();
		protected virtual HtmlExportContext CreateExportContext(PrintingSystemBase printingSystem, IScriptContainer scriptContainer, IImageRepository imageRepository, HtmlExportMode exportMode) {
			return new HtmlExportContext(printingSystem, scriptControl, imageRepository, exportMode);
		}
		protected virtual ScriptBlockControl CreateScriptControl(WebStyleControl styleControl) {
			return new ScriptBlockControl(styleControl);
		}
		protected DXHtmlTable CreateHtmlLayoutTable(ILayoutBuilder layoutBuilder, bool tableLayout) {
			LayoutControlCollection layoutControls = layoutBuilder.BuildLayoutControls();
			ProgressReflector.RangeValue++;
			DXHtmlTable contentTable = CreateContentTable(layoutControls, tableLayout);
			if(htmlExportContext.MainExportMode != HtmlExportMode.SingleFile)
				ProgressReflector.RangeValue++;
			return contentTable;
		}
		protected virtual HtmlBuilderBase GetHtmlBuilder(bool tableLayout) {
			return tableLayout ? (HtmlBuilderBase)new HtmlTableBuilder() : (HtmlBuilderBase)new HtmlDivBuilder();
		}
		protected DXHtmlTable CreateContentTable(LayoutControlCollection layoutControls, bool tableLayout) {
			return GetHtmlBuilder(tableLayout).BuildTable(layoutControls, document.CorrectImportBrickBounds, htmlExportContext);
		}
		protected void AddChildrenControl(DXWebControlBase control) {
			if(control != null)
				Controls.Add(control);
		}
	}
}
