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

using DevExpress.Printing.Native;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
namespace DevExpress.XtraPrinting.XamlExport {
	static class WatermarkXamlExporter {
		const int showOnTopZIndex = 1;
		public static void WriteToXaml(XamlWriter writer, Page page, XamlExportContext exportContext) {
			WriteWatermarkToXaml(writer, page, exportContext, page.ActualWatermark, GetPageRange(page));
		}
		static string GetPageRange(Page page) {
			return GetPageRange(page.ActualWatermark) ?? GetPageRange(page.Document);
		}
		static string GetPageRange(PageWatermark pageWatermark) {
			Watermark watermark = pageWatermark as Watermark;
			return watermark != null ? watermark.PageRange : null;
		}
		static string GetPageRange(Document document) {
			return document != null && document.PrintingSystem != null ? GetPageRange(document.PrintingSystem.Watermark) : null;
		}
		static void WriteWatermarkToXaml(XamlWriter writer, Page page, XamlExportContext exportContext, PageWatermark wm, string pageRange) {
			if(wm == null)
				return;
			if(!string.IsNullOrEmpty(pageRange)) {
				int[] items = PageRangeParser.GetIndices(pageRange, page.Document.PageCount);
				if(Array.IndexOf(items, page.Index) < 0)
					return;
			}
			if(ContainsImage(wm)) {
				WriteImageWatermark(writer, page, exportContext, wm);
			}
			if(ContainsText(wm)) {
				WriteTextWatermark(writer, page, wm, exportContext);
			}
		}
		static void WriteImageWatermark(XamlWriter writer, Page page, XamlExportContext exportContext, PageWatermark wm) {
			if(!exportContext.ResourceMap.ImageResourcesDictionary.ContainsKey(page))
				return;
			if(wm.ImageTiling && exportContext.IsPartialTrustMode)
				return;
			writer.WriteStartElement(XamlTag.Border);
			if(!wm.ShowBehind)
				writer.WriteAttribute(XamlAttribute.CanvasZIndex, showOnTopZIndex);
			SizeF watermarkAreaSize = GetWatermarkAreaSize(page, exportContext);
			writer.WriteAttribute(XamlAttribute.Width, watermarkAreaSize.Width);
			writer.WriteAttribute(XamlAttribute.Height, watermarkAreaSize.Height);
			writer.WriteStartElement(XamlTag.Image);
			SizeF watermarkImageAreaSize = wm.ImageViewMode == ImageViewMode.Clip && !wm.ImageTiling ? new SizeF(wm.Image.Width, wm.Image.Height) : watermarkAreaSize;
			writer.WriteAttribute(XamlAttribute.Width, watermarkImageAreaSize.Width);
			writer.WriteAttribute(XamlAttribute.Height, watermarkImageAreaSize.Height);
			if(exportContext.Compatibility == DevExpress.XtraPrinting.XamlExport.XamlCompatibility.WPF)
				writer.WriteAttribute(XamlAttribute.RenderOptionsBitmapScalingMode, XamlNames.Fant);
			string imageSource = exportContext.ResourceMap.ImageResourcesDictionary[page];
			writer.WriteAttribute(XamlAttribute.Source, imageSource);
			writer.WriteAttribute(XamlAttribute.Margin,
				page.PageData.MinMarginsF.Left.DocToDip(),
				page.PageData.MinMarginsF.Top.DocToDip(),
				page.PageData.MinMarginsF.Right.DocToDip(),
				page.PageData.MinMarginsF.Bottom.DocToDip());
			if(wm.ImageTransparency != 0 && !wm.ImageTiling) {
				writer.WriteAttribute(XamlAttribute.Opacity, MapTransparencyToOpacity(wm.ImageTransparency));
			}
			if(!(wm.ImageViewMode == ImageViewMode.Zoom)) {
				writer.WriteAttribute(XamlAttribute.VerticalAlignment, MapContentAlignmentToVerticalAlignment(wm.ImageAlign));
				writer.WriteAttribute(XamlAttribute.HorizontalAlignment, MapContentAlignmentToHorizontalAlignment(wm.ImageAlign));
			}
			if(wm.ImageTiling) {
				writer.WriteAttribute(XamlAttribute.Stretch, MapImageViewModeToStretch(ImageViewMode.Stretch));
				writer.WriteStartElement(XamlTag.ImageEffect);
				writer.WriteStartElement(XamlNsPrefix.dxpn, XamlTag.TileEffect, XamlNs.PrintingCoreNativePresentation);
				SizeF watermarkImageSize = GetWatermarkImageSize(page, exportContext);
				writer.WriteAttribute(XamlAttribute.TileCount,
					watermarkAreaSize.Width / watermarkImageSize.Width,
					watermarkAreaSize.Height / watermarkImageSize.Height);
				writer.WriteAttribute(XamlAttribute.Opacity, MapTransparencyToOpacity(wm.ImageTransparency));
				writer.WriteEndElement(); 
				writer.WriteEndElement(); 
			} else {
				writer.WriteAttribute(XamlAttribute.Stretch, MapImageViewModeToStretch(wm.ImageViewMode));
			}
			writer.WriteEndElement(); 
			writer.WriteEndElement(); 
		}
		static void WriteTextWatermark(XamlWriter writer, Page page, PageWatermark wm, XamlExportContext exportContext) {
			writer.WriteStartElement(XamlTag.Border);
			SizeF watermarkSize = GetWatermarkAreaSize(page, exportContext);
			writer.WriteAttribute(XamlAttribute.MinWidth, watermarkSize.Width);
			writer.WriteAttribute(XamlAttribute.MinHeight, watermarkSize.Height);
			writer.WriteAttribute(XamlAttribute.MaxWidth, watermarkSize.Width);
			writer.WriteAttribute(XamlAttribute.MaxHeight, watermarkSize.Height);
			writer.WriteAttribute(XamlAttribute.Margin,
				page.PageData.MinMarginsF.Left.DocToDip(),
				page.PageData.MinMarginsF.Top.DocToDip(),
				page.PageData.MinMarginsF.Right.DocToDip(),
				page.PageData.MinMarginsF.Bottom.DocToDip());
			if(!wm.ShowBehind)
				writer.WriteAttribute(XamlAttribute.CanvasZIndex, showOnTopZIndex);
			writer.WriteStartElement(XamlTag.TextBlock);
			if(wm.TextTransparency != 0) {
				writer.WriteAttribute(XamlAttribute.Opacity, MapTransparencyToOpacity(wm.TextTransparency));
			}
			writer.WriteAttribute(XamlAttribute.Text, wm.Text);
			WriteFontAttributes(writer, wm);
			writer.WriteAttribute(XamlAttribute.FontFamily, FontResolver.ResolveFamilyName(wm.Font));
			writer.WriteAttribute(XamlAttribute.Foreground, wm.ForeColor);
			writer.WriteAttribute(XamlAttribute.VerticalAlignment, MapContentAlignmentToVerticalAlignment(ContentAlignment.MiddleCenter));
			writer.WriteAttribute(XamlAttribute.HorizontalAlignment, MapContentAlignmentToHorizontalAlignment(ContentAlignment.MiddleCenter));
			writer.WriteAttribute(XamlAttribute.RenderTransformOrigin, 0.5f, 0.5f);
			WriteRotateTransform(writer, wm);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		static void WriteRotateTransform(XamlWriter writer, PageWatermark wm) {
			if(wm.TextDirection == DirectionMode.Horizontal)
				return;
			writer.WriteStartElement(XamlTag.TextBlockRenderTransform);
			writer.WriteStartElement(XamlTag.TransformGroup);
			writer.WriteStartElement(XamlTag.RotateTransform);
			writer.WriteAttribute(XamlAttribute.Angle, GetRotationAngle(wm.TextDirection));
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		static int GetRotationAngle(DirectionMode directionMode) {
			switch(directionMode) {
				case DirectionMode.BackwardDiagonal:
					return 50;
				case DirectionMode.ForwardDiagonal:
					return 310;
				case DirectionMode.Horizontal:
					return 0;
				case DirectionMode.Vertical:
					return 270;
			}
			throw new ArgumentException("directionMode");
		}
		static bool ContainsImage(PageWatermark wm) {
			return wm.Image != null;
		}
		static bool ContainsText(PageWatermark wm) {
			return wm.Text != null && wm.Text != string.Empty;
		}
		static SizeF GetWatermarkAreaSize(Page page, XamlExportContext exportContext) {
			SizeF size = page.PageSize.DocToDip();
			size.Width -= page.PageData.MinMarginsF.Left.DocToDip() + page.PageData.MinMarginsF.Right.DocToDip();
			size.Height -= page.PageData.MinMarginsF.Top.DocToDip() + page.PageData.MinMarginsF.Bottom.DocToDip();
			return size;
		}
		static string MapImageViewModeToStretch(ImageViewMode imageWatermarkMode) {
			switch(imageWatermarkMode) {
				case ImageViewMode.Clip:
					return XamlNames.StretchFill;
				case ImageViewMode.Zoom:
					return XamlNames.StretchUniform;
				case ImageViewMode.Stretch:
					return XamlNames.StretchFill;
			}
			throw new ArgumentException("imageWatermarkMode");
		}
		static string MapContentAlignmentToVerticalAlignment(ContentAlignment alignment) {
			switch(alignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopRight:
					return XamlNames.AlignmentTop;
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
					return XamlNames.AlignmentCenter;
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
				case ContentAlignment.BottomLeft:
					return XamlNames.AlignmentBottom;
			}
			throw new ArgumentException("alignment");
		}
		static string MapContentAlignmentToHorizontalAlignment(ContentAlignment alignment) {
			switch(alignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
					return XamlNames.AlignmentLeft;
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					return XamlNames.AlignmentCenter;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					return XamlNames.AlignmentRight;
			}
			throw new ArgumentException("alignment");
		}
		static SizeF GetWatermarkImageSize(Page page, XamlExportContext exportContext) {
			PageWatermark wm = page.ActualWatermark;
			switch(wm.ImageViewMode) {
				case ImageViewMode.Clip:
					return new SizeF(wm.Image.Width, wm.Image.Height);
				case ImageViewMode.Stretch:
					return GetWatermarkAreaSize(page, exportContext);
				case ImageViewMode.Zoom:
					return CalculateWatermarkImageSizeInZoomMode(page, exportContext);
			}
			throw new ArgumentException("page");
		}
		static SizeF CalculateWatermarkImageSizeInZoomMode(Page page, XamlExportContext exportContext) {
			SizeF watermarkAreaSize = GetWatermarkAreaSize(page, exportContext);
			Size watermarkImageSize = new Size(page.ActualWatermark.Image.Width, page.ActualWatermark.Image.Height);
			float horizontalInflateCoeff = watermarkAreaSize.Width / watermarkImageSize.Width;
			float verticalInflateCoeff = watermarkAreaSize.Height / watermarkImageSize.Height;
			if(horizontalInflateCoeff > verticalInflateCoeff) {
				return new SizeF(watermarkImageSize.Width * verticalInflateCoeff, watermarkAreaSize.Height);
			} else {
				return new SizeF(watermarkAreaSize.Width, watermarkImageSize.Height * horizontalInflateCoeff);
			}
		}
		static void WriteFontAttributes(XamlWriter writer, PageWatermark wm) {
			float fontSizeInDocs = wm.Font.Size.ToDocFrom(wm.Font.Unit);
			writer.WriteAttribute(XamlAttribute.FontSize, fontSizeInDocs.DocToDip());
			if(wm.Font.Bold) {
				writer.WriteAttribute(XamlAttribute.FontWeight, XamlNames.FontBold);
			}
			if(wm.Font.Italic) {
				writer.WriteAttribute(XamlAttribute.FontStyle, XamlNames.FontItalic);
			}
			if(wm.Font.Strikeout && wm.Font.Underline) {
				writer.WriteAttribute(XamlAttribute.TextDecorations, XamlNames.FontStrikethrough + ',' + XamlNames.FontUnderline);
			} else {
				if(wm.Font.Strikeout) {
					writer.WriteAttribute(XamlAttribute.TextDecorations, XamlNames.FontStrikethrough);
				}
				if(wm.Font.Underline) {
					writer.WriteAttribute(XamlAttribute.TextDecorations, XamlNames.FontUnderline);
				}
			}
		}
		static float MapTransparencyToOpacity(int transparency) {
			const int maxImageTransparency = 255;
			return 1 - (float)transparency / (float)maxImageTransparency;
		}
	}
}
