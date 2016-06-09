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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	class PageXamlExporter : BrickXamlExporterBase {
		public override void WriteBrickToXaml(XamlWriter writer, BrickBase brick, XamlExportContext exportContext, RectangleF clipRect, Action<XamlWriter> declareNamespaces, Action<XamlWriter, object> writeCustomProperties) {
			Page page = brick as Page;
			if(page == null)
				throw new ArgumentException("brick");
			writer.WriteStartElement(XamlTag.Canvas, XamlNs.Default);
			writer.WriteNamespace(XamlNsPrefix.x, XamlNs.x);
			writer.WriteNamespace(XamlNsPrefix.system, XamlNs.system);
			writer.WriteNamespace(XamlNsPrefix.dxpn, XamlNs.PrintingCoreNative);
			if(!XamlExporter.InheritParentFlowDirection) {
				writer.WriteAttribute(XamlAttribute.FlowDirection, XamlNames.FlowDirectionLeftToRight);
			}
			declareNamespaces(writer);
			if(page.Document != null)
				writer.WriteAttribute(XamlAttribute.Background, page.Document.PrintingSystem.Graph.PageBackColor);
			float width = page.PageSize.Width.DocToDip();
			float height = page.PageSize.Height.DocToDip();
			writer.WriteAttribute(XamlAttribute.Width, width);
			writer.WriteAttribute(XamlAttribute.Height, height);
			if(exportContext.Compatibility == XamlCompatibility.WPF)
				writer.WriteAttribute(XamlAttribute.SnapsToDevicePixels, true.ToString());
			writer.WriteAttribute(XamlAttribute.UseLayoutRounding, false.ToString());
			writer.WriteStartElement(XamlTag.CanvasClip);
			writer.WriteStartElement(XamlTag.RectangleGeometry);
			writer.WriteAttribute(XamlAttribute.Rect, new RectangleF(0f, 0f, width, height));
			writer.WriteEndElement();
			writer.WriteEndElement();
			WriteResources(writer, exportContext);
			WatermarkXamlExporter.WriteToXaml(writer, page, exportContext);
		}
		public override void WriteEndTags(XamlWriter writer) {
			writer.WriteEndElement(); 
		}
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.ChildElements;
		}
		static void WriteResources(XamlWriter writer, XamlExportContext exportContext) {
			writer.WriteStartElement(XamlTag.CanvasResources);
			writer.WriteStartElement(XamlTag.ResourceDictionary);
			WriteBorderStyles(writer, exportContext.ResourceCache.BorderStyles, exportContext);
			WriteBorderDashStyles(writer, exportContext.ResourceCache.BorderDashStyles);
			WriteTextBlockStyles(writer, exportContext.ResourceCache.TextBlockStyles, exportContext.Compatibility, exportContext.TextMeasurementSystem);
			WriteLineStyles(writer, exportContext.ResourceCache.LineStyles);
			WriteImageResources(writer, exportContext.ResourceCache.ImageResources, exportContext.ResourceMap.ImageResourcesDictionary.Count != 0, exportContext.Compatibility, exportContext.EmbedImagesToXaml);
			if(exportContext.ResourceMap.ShouldAddCheckBoxTemplates) {
				WriteCheckBoxTemplates(writer);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		static void WriteBorderStyles(XamlWriter writer, IEnumerable<XamlBorderStyle> borderStyles, XamlExportContext exportContext) {
			foreach(XamlBorderStyle borderStyle in borderStyles) {
				writer.WriteStartElement(XamlTag.Style);
				writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, borderStyle.Name);
				writer.WriteAttribute(XamlAttribute.TargetType, XamlNames.TargetTypeBorder);
				writer.WriteSetter(XamlAttribute.BorderThickness, new float[] {
					GetBorderWidth(BorderSide.Left, borderStyle),
					GetBorderWidth(BorderSide.Top, borderStyle),
					GetBorderWidth(BorderSide.Right, borderStyle),
					GetBorderWidth(BorderSide.Bottom, borderStyle)
				});
				writer.WriteSetter(XamlAttribute.BorderBrush, borderStyle.BorderBrush);
				writer.WriteSetter(XamlAttribute.Background, borderStyle.BackColor);
				PaddingInfo paddingInPixels = borderStyle.Padding.Scale(GraphicsDpi.DeviceIndependentPixel / borderStyle.Padding.Dpi);
				writer.WriteSetter(XamlAttribute.Padding,
					new float[] { paddingInPixels.Left, paddingInPixels.Top, paddingInPixels.Right, paddingInPixels.Bottom });
				writer.WriteEndElement();
			}
		}
		static float GetBorderWidth(BorderSide borderSide, XamlBorderStyle borderStyle) {
			return (borderStyle.Sides & borderSide) > 0 ? borderStyle.BorderWidth : 0;
		}
		static void WriteBorderDashStyles(XamlWriter writer, IEnumerable<XamlLineStyle> lineStyles) {
			foreach(XamlLineStyle lineStyle in lineStyles) {
				WriteLineStart(writer, lineStyle);
				writer.WriteSetter(XamlAttribute.StrokeStartLineCap, "Square");
				writer.WriteSetter(XamlAttribute.StrokeEndLineCap, "Square");
				writer.WriteEndElement();
			}
		}
		static void WriteLineStart(XamlWriter writer, XamlLineStyle lineStyle) {
			writer.WriteStartElement(XamlTag.Style);
			writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, lineStyle.Name);
			writer.WriteAttribute(XamlAttribute.TargetType, XamlNames.TargetTypeLine);
			writer.WriteSetter(XamlAttribute.Stroke, lineStyle.Stroke);
			writer.WriteSetter(XamlAttribute.StrokeThickness, lineStyle.StrokeThickness);
		}
		static void WriteTextBlockStyles(XamlWriter writer, IEnumerable<XamlTextBlockStyle> textBlockStyles, XamlCompatibility compatibility, TextMeasurementSystem textMeasurementSystem) {
			foreach(XamlTextBlockStyle textBlockStyle in textBlockStyles) {
				writer.WriteStartElement(XamlTag.Style);
				writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, textBlockStyle.Name);
				writer.WriteAttribute(XamlAttribute.TargetType, XamlNames.TargetTypeTextBlock);
				writer.WriteSetter(XamlAttribute.FontFamily, ResolveFontFamilyName(textBlockStyle.Font));
				WriteFontSetters(writer, textBlockStyle);
				writer.WriteSetter(XamlAttribute.Foreground, textBlockStyle.Foreground);
				writer.WriteSetter(XamlAttribute.TextAlignment, GetTextHorizontalAlignment(textBlockStyle.TextAlignment, compatibility));
				writer.WriteSetter(XamlAttribute.VerticalAlignment, GetVerticalAlignment(textBlockStyle.TextAlignment));
				if(textMeasurementSystem == TextMeasurementSystem.NativeXpf) {
					writer.WriteSetter(XamlAttribute.TextWrapping, textBlockStyle.WrapText ? XamlNames.TextWrappingWrap : XamlNames.TextWrappingNoWrap);
				}
				if(textBlockStyle.StringTrimming != StringTrimming.None) {
					string textTrimming = GetTextTrimming(textBlockStyle.StringTrimming);
					if(textTrimming != XamlNames.TextTrimmingNone) {
						writer.WriteSetter(XamlAttribute.TextTrimming, textTrimming);
					}
				}
				writer.WriteEndElement();
			}
		}
		static string ResolveFontFamilyName(Font font) {
			return FontResolver.ResolveEnglishFamilyName(font);
		}
		internal static Stream GetCheckBoxTemplatesResourceStream() {
			Stream stream = ResourceStreamHelper.GetStream("Native.CheckBoxTemplates.xaml", typeof(DevExpress.Printing.ResFinder));
			return stream;
		}
		static void WriteCheckBoxTemplates(XamlWriter writer) {
			writer.WriteStartElement(XamlTag.ResourceDictionaryMergedDictionaries);
			using(Stream stream = GetCheckBoxTemplatesResourceStream())
			using(StreamReader reader = new StreamReader(stream)) {
				writer.WriteRaw(reader.ReadToEnd());
			}
			writer.WriteEndElement();
		}
		static void WriteLineStyles(XamlWriter writer, IEnumerable<XamlLineStyle> lineStyles) {
			foreach(XamlLineStyle lineStyle in lineStyles) {
				WriteLineStyle(writer, lineStyle);
				writer.WriteEndElement();
			}
		}
		static void WriteLineStyle(XamlWriter writer, XamlLineStyle lineStyle) {
			writer.WriteStartElement(XamlTag.Style);
			writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, lineStyle.Name);
			writer.WriteAttribute(XamlAttribute.TargetType, XamlNames.TargetTypeLine);
			writer.WriteSetter(XamlAttribute.Stroke, lineStyle.Stroke);
			writer.WriteSetter(XamlAttribute.StrokeThickness, lineStyle.StrokeThickness);
		}
		static void WriteImageResources(XamlWriter writer, IEnumerable<ImageResource> imageResources, bool shouldWriteConverterResource, XamlCompatibility compatibility, bool embedImagesToXaml) {
			foreach(ImageResource imageResource in imageResources) {
				writer.WriteStartElement(XamlNsPrefix.system, XamlTag.String, XamlNs.system);
				writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, imageResource.Name);
				writer.WriteValue(GetBase64StringForImage(imageResource.Image));
				writer.WriteEndElement();
			}
			if(shouldWriteConverterResource) {
				if(embedImagesToXaml) {
					writer.WriteStartElement(XamlNsPrefix.dxpn, XamlTag.Base64StringImageConverter, XamlNs.PrintingCoreNativePresentation);
					writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, XamlExporter.EmbeddedImageConverterName);
					writer.WriteEndElement();
				}
#if !SILVERLIGHT
				if(compatibility == XamlCompatibility.WPF) {
					writer.WriteStartElement(XamlNsPrefix.dxpn, XamlTag.RepositoryImageConverter, XamlNs.PrintingCoreNativePresentation);
					writer.WriteAttribute(XamlNsPrefix.x, XamlAttribute.Key, XamlExporter.RepositoryImageConverterName);
					writer.WriteEndElement();
				}
#endif
			}
		}
		static string GetBase64StringForImage(Image image) {
			return image != null ? Convert.ToBase64String(PSConvert.ImageToArray(image, ImageFormat.Png)) : string.Empty;
		}
		static void WriteFontSetters(XamlWriter writer, XamlTextBlockStyle textBlockStyle) {
			float fontSizeInDocs = textBlockStyle.Font.Size.ToDocFrom(textBlockStyle.Font.Unit);
			writer.WriteSetter(XamlAttribute.FontSize, fontSizeInDocs.DocToDip());
			if(textBlockStyle.Font.Bold) {
				writer.WriteSetter(XamlAttribute.FontWeight, XamlNames.FontBold);
			} else {
				writer.WriteSetter(XamlAttribute.FontWeight, XamlNames.FontNormal);
			}
			if(textBlockStyle.Font.Italic) {
				writer.WriteSetter(XamlAttribute.FontStyle, XamlNames.FontItalic);
			} else {
				writer.WriteSetter(XamlAttribute.FontStyle, XamlNames.FontNormal);
			}
			if(textBlockStyle.Font.Strikeout && textBlockStyle.Font.Underline) {
				writer.WriteSetter(XamlAttribute.TextDecorations, String.Format("{0},{1}", XamlNames.FontStrikethrough, XamlNames.FontUnderline));
			} else {
				if(textBlockStyle.Font.Strikeout) {
					writer.WriteSetter(XamlAttribute.TextDecorations, XamlNames.FontStrikethrough);
				}
				if(textBlockStyle.Font.Underline) {
					writer.WriteSetter(XamlAttribute.TextDecorations, XamlNames.FontUnderline);
				}
			}
		}
		static string GetTextTrimming(StringTrimming stringTrimming) {
			switch(stringTrimming) {
				case StringTrimming.EllipsisCharacter:
					return XamlNames.TextTrimmingCharacterEllipsis;
				case StringTrimming.EllipsisWord:
					return XamlNames.TextTrimmingWordEllipsis;
				default:
					return XamlNames.TextTrimmingNone;
			}
		}
		static string GetTextHorizontalAlignment(TextAlignment alignment, XamlCompatibility compatibility) {
			switch(alignment) {
				case TextAlignment.TopLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.BottomLeft:
					return XamlNames.AlignmentLeft;
				case TextAlignment.TopCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.BottomCenter:
					return XamlNames.AlignmentCenter;
				case TextAlignment.TopRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.BottomRight:
					return XamlNames.AlignmentRight;
				case TextAlignment.TopJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.BottomJustify:
					return compatibility == XamlCompatibility.WPF ? XamlNames.AlignmentJustify : XamlNames.AlignmentLeft;
			}
			throw new ArgumentException("alignment");
		}
		static string GetVerticalAlignment(TextAlignment alignment) {
			switch(alignment) {
				case TextAlignment.TopCenter:
				case TextAlignment.TopLeft:
				case TextAlignment.TopRight:
				case TextAlignment.TopJustify:
					return XamlNames.AlignmentTop;
				case TextAlignment.MiddleCenter:
				case TextAlignment.MiddleLeft:
				case TextAlignment.MiddleRight:
				case TextAlignment.MiddleJustify:
					return XamlNames.AlignmentCenter;
				case TextAlignment.BottomCenter:
				case TextAlignment.BottomRight:
				case TextAlignment.BottomLeft:
				case TextAlignment.BottomJustify:
					return XamlNames.AlignmentBottom;
			}
			throw new ArgumentException("alignment");
		}
	}
}
