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
using System.Collections;
using System.Drawing;
using System.IO;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlExporter {
		public const string EmbeddedImageConverterName = "embeddedImageConverter";
		public const string RepositoryImageConverterName = "repositoryImageConverter";
		public const string BindingFormatString = "{{Binding Source={{StaticResource {0}}}, Converter={{StaticResource {1}}}}}";
		public const string BindingWithConverterFormatString = "{{Binding RelativeSource={{RelativeSource Self}}, Converter={{StaticResource {0}}}, ConverterParameter={1}}}";
		public event EventHandler<ImageResolveEventArgs> ImageResolve;
		public event EventHandler<DeclareNamespacesEventArgs> DeclareNamespaces;
		public event EventHandler<WriteCustomPropertiesEventArgs> WriteCustomProperties;
		static readonly PageWatermark DefaultWatermark = new Watermark();
		PrintingSystemBase currentPrintingSystem;
		public bool IsPartialTrustMode { get; set; }
		public bool EmbedImagesToXaml { get; set; }
		public static bool InheritParentFlowDirection { get; set; }
		public XamlExporter() {
			EmbedImagesToXaml = true;
		}
		public void Export(Stream stream, Page page, int pageNumber, int pageCount, XamlCompatibility compatibility, TextMeasurementSystem textMeasurementSystem) {
#if SL
			if(textMeasurementSystem == TextMeasurementSystem.GdiPlus)
				throw new NotSupportedException();
#endif
			using(var writer = new XamlWriter(stream)) {
				var exportContext = new XamlExportContext(
					page,
					pageNumber,
					pageCount,
					new ResourceCache(),
					new ResourceMap(),
					compatibility,
					textMeasurementSystem,
					IsPartialTrustMode,
					EmbedImagesToXaml
				);
				if(page.Document != null)
					currentPrintingSystem = page.Document.PrintingSystem;
				CollectStyles(page, exportContext);
				var pageClipRect = new RectangleF(0f, 0f, page.Rect.X + page.Rect.Width, page.PageData.PageSize.Height);
				WriteBricks(writer, page, exportContext, page.Location, pageClipRect, RectangleF.Empty);
				writer.Flush();
			}
			if(EmbedImagesToXaml && currentPrintingSystem != null)
				currentPrintingSystem.GarbageImages.Clear();
			currentPrintingSystem = null;
		}
		string ResolveImage(Image image) {
			if(ImageResolve != null && image != null) {
				var e = new ImageResolveEventArgs(image);
				ImageResolve(this, e);
				return e.Uri;
			}
			return null;
		}
		void RaiseDeclareNamespaces(XamlWriter writer) {
			if(DeclareNamespaces != null) {
				DeclareNamespaces(this, new DeclareNamespacesEventArgs(writer));
			}
		}
		void RaiseWriteCustomProperties(XamlWriter writer, object obj) {
			if(WriteCustomProperties != null) {
				WriteCustomProperties(this, new WriteCustomPropertiesEventArgs(writer, obj));
			}
		}
		void CollectStyles(BrickBase brick, XamlExportContext exportContext) {
			RegisterBorderStyle(brick, exportContext);
			RegisterTextBlockStyle(brick, exportContext);
			RegisterLineStyle(brick, exportContext);
			RegisterImage(brick, exportContext);
			exportContext.ResourceMap.ShouldAddCheckBoxTemplates = exportContext.ResourceMap.ShouldAddCheckBoxTemplates || brick is CheckBoxBrick;
			foreach(BrickBase innerBrick in GetInnerBricks(brick))
				CollectStyles(innerBrick, exportContext);
		}
		void WriteBricks(XamlWriter writer, BrickBase brickBase, XamlExportContext exportContext, PointF offset, RectangleF clipRect, RectangleF exportBrickClipRect) {
			Brick brick = brickBase as Brick;
			if(brick != null && !brick.IsVisible)
				return;
			BrickXamlExporterBase exporter = BrickXamlExporterFactory.CreateExporter(brickBase);
			if(exporter != null) {
				exporter.WriteBrickToXaml(writer, brickBase, exportContext, exportBrickClipRect, RaiseDeclareNamespaces, RaiseWriteCustomProperties);
			}
			BrickIterator iterator = new BrickIterator(GetInnerBricks(brickBase), offset, clipRect);
			while(iterator.MoveNext()) {
				var newOffset = new PointF(iterator.Offset.X + iterator.CurrentBrick.X + iterator.CurrentBrick.InnerBrickListOffset.X,
										   iterator.Offset.Y + iterator.CurrentBrick.Y + iterator.CurrentBrick.InnerBrickListOffset.Y);
				RectangleF newClipRect = iterator.CurrentBrickRectangle;
				if(!iterator.CurrentClipRectangle.IsEmpty)
					clipRect.Intersect(iterator.CurrentClipRectangle);
				RectangleF newExportBrickClipRect = newClipRect;
				newExportBrickClipRect.Offset(-newOffset.X, -newOffset.Y);
				WriteBricks(writer, iterator.CurrentBrick, exportContext, newOffset, newClipRect, newExportBrickClipRect);
			}
			if(exporter != null)
				exporter.WriteEndTags(writer);
		}
		static IList GetInnerBricks(BrickBase brickBase) {
			IRichTextBrick richTextBrick = brickBase as IRichTextBrick;
			if(richTextBrick != null)
				return richTextBrick.GetChildBricks();
			if(!(brickBase is BrickContainerBase)) {
				Brick brick = brickBase as Brick;
				if(brick != null && brick.Bricks != EmptyBrickCollection.Instance)
					return brick.Bricks;
			}
			return brickBase.InnerBrickList;
		}
		static void RegisterBorderStyle(BrickBase brick, XamlExportContext exportContext) {
			if(XamlResourceHelper.ExporterRequiresBorderStyle(brick)) {
				var visualBrick = brick as VisualBrick;
				if(visualBrick != null) {
					var brickStyle = visualBrick.Style.Clone() as BrickStyle;
					if(BrickIsBarCodeOrZip(brick)) {
						brickStyle.Padding = PaddingInfo.Empty;
					}
					XamlBorderStyle borderStyle = CreateXamlBorderStyle(brickStyle);
					string styleName = exportContext.ResourceCache.RegisterBorderStyle(borderStyle);
					exportContext.ResourceMap.BorderStylesDictionary[visualBrick] = styleName;
					if(visualBrick.Style.BorderDashStyle.IsDashedOrDottedLineStyle()) {
						var lineStyle = new XamlLineStyle(visualBrick.Style.BorderColor, visualBrick.BorderWidth, VisualBrick.GetDashPattern(visualBrick.BorderDashStyle));
						styleName = exportContext.ResourceCache.RegisterBorderDashStyle(lineStyle);
						exportContext.ResourceMap.BorderDashStylesDictionary[visualBrick] = styleName;
					}
				}
			}
		}
		internal static XamlBorderStyle CreateXamlBorderStyle(BrickStyle brickStyle) {
			Color borderColor = brickStyle.BorderDashStyle.IsSolidLineStyle() ? brickStyle.BorderColor : brickStyle.BackColor;
			if(brickStyle.BorderWidth == 0 || brickStyle.Sides == BorderSide.None) {
				borderColor = Color.FromArgb(0, borderColor);
			}
			return new XamlBorderStyle(brickStyle.BorderWidth, borderColor, brickStyle.Sides, brickStyle.BackColor, brickStyle.Padding);
		}
		static void RegisterTextBlockStyle(BrickBase brick, XamlExportContext exportContext) {
			var textBrick = brick as TextBrick;
			if(textBrick != null) {
				var textBlockStyle = new XamlTextBlockStyle(
					textBrick.Font,
					textBrick.Style.TextAlignment,
					(textBrick.Style.StringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0 ? true : false,
					textBrick.Style.ForeColor,
					textBrick.StringFormat.Trimming);
				string styleName = exportContext.ResourceCache.RegisterTextBlockStyle(textBlockStyle);
				exportContext.ResourceMap.TextBlockStylesDictionary[textBrick] = styleName;
			}
		}
		static void RegisterLineStyle(BrickBase brick, XamlExportContext exportContext) {
			var lineBrick = brick as LineBrick;
			if(lineBrick != null) {
				var lineStyle = new XamlLineStyle(lineBrick.Style.ForeColor, lineBrick.LineWidth, VisualBrick.GetDashPattern(lineBrick.LineStyle));
				string styleName = exportContext.ResourceCache.RegisterLineStyle(lineStyle);
				exportContext.ResourceMap.LineStylesDictionary[lineBrick] = styleName;
			}
		}
		void RegisterImage(BrickBase brick, XamlExportContext exportContext) {
			if(brick is ImageBrick && ((ImageBrick)brick).Image == EmptyImage.Instance) {
				ImageBrick imageBrick = (ImageBrick)brick;
				string bindingString = string.Format(BindingWithConverterFormatString,
					RepositoryImageConverterName,
					string.Format("{0}_{1}", RepositoryImageHelper.GetDocumentId(currentPrintingSystem), string.Empty));
				exportContext.ResourceMap.ImageResourcesDictionary[brick] = bindingString;
				return;
			}
			ImageResource watermarkImageResource = CreateWatermarkImageResource(brick as Page);
			RegisterImageResource(watermarkImageResource, exportContext.ResourceCache.RegisterWatermarkImageResource, exportContext, brick);
			if(exportContext.Page.Document == null)
				return;
			float scaleFactor = exportContext.Page.Document.ScaleFactor;
			ImageResource imageResource = CreateImageResource(brick, scaleFactor);
			RegisterImageResource(imageResource, exportContext.ResourceCache.RegisterImageResource, exportContext, brick);
		}
		void RegisterImageResource(ImageResource imageResource, Func<ImageResource, string> addToResourceCache, XamlExportContext exportContext, BrickBase brick) {
			if(imageResource == null)
				return;
			string imageResourceName = EmbedImagesToXaml ? addToResourceCache(imageResource) : ResolveImage(imageResource.Image);
			AddToResourceMap(imageResourceName, exportContext, brick);
		}
		void AddToResourceMap(string imageResourceKey, XamlExportContext context, BrickBase brick) {
			string value;
			if(EmbedImagesToXaml)
				value = string.Format(BindingFormatString, imageResourceKey, EmbeddedImageConverterName);
			else
				value = imageResourceKey;
			context.ResourceMap.ImageResourcesDictionary[brick] = value;
		}
		static ImageResource CreateWatermarkImageResource(Page page) {
			if(page != null && page.ActualWatermark != null && !PageWatermark.Equals(page.ActualWatermark, DefaultWatermark) && page.ActualWatermark.Image != null) {
				return new ImageResource(page.ActualWatermark.Image);
			}
			return null;
		}
		ImageResource CreateImageResource(BrickBase brick, float scaleFactor) {
			if(brick is ImageBrick) {
				Image image = ((ImageBrick)brick).Image;
				return image != null ? new ImageResource(image) : null;
			}
			if(XamlResourceHelper.ExporterRequiresImageResource(brick)) {
				VisualBrick visualBrick = brick as VisualBrick;
				double ratio = 1d / scaleFactor;
				SizeF scaledVisualBrickSize = MathMethods.Scale(visualBrick.Size, ratio);
				RectangleF brickRectInPixels = new RectangleF(PointF.Empty, scaledVisualBrickSize).DocToPixel();
				VisualBrickExporter exporter = currentPrintingSystem.ExportersFactory.GetExporter(visualBrick) as VisualBrickExporter;
				float dpi = XamlResourceHelper.GetExporterGraphicsDpi(brick);
				Image image = exporter.DrawContentToImage(currentPrintingSystem, currentPrintingSystem.GarbageImages, brickRectInPixels, false, dpi);
				return new ImageResource(image);
			}
			return null;
		}
		static bool BrickIsBarCodeOrZip(BrickBase brick) {
			return brick is BarCodeBrick || brick is ZipCodeBrick;
		}
	}
}
