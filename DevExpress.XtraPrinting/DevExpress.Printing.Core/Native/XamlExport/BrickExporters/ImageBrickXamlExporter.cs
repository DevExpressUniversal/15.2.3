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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	class ImageBrickXamlExporter : VisualBrickXamlExporter {
		protected override void WriteBrickToXamlCore(XamlWriter writer, VisualBrick brick, XamlExportContext exportContext) {
			ImageBrick imageBrick = brick as ImageBrick;
			if(imageBrick == null)
				throw new ArgumentException("brick");
			if(!exportContext.ResourceMap.ImageResourcesDictionary.ContainsKey(imageBrick))
				return;
			writer.WriteStartElement(XamlTag.Image);
			writer.WriteAttribute(XamlAttribute.Source, exportContext.ResourceMap.ImageResourcesDictionary[imageBrick]);
			writer.WriteAttribute(XamlAttribute.VerticalAlignment, GetVerticalAlignment(imageBrick));
			writer.WriteAttribute(XamlAttribute.HorizontalAlignment, GetHorizontalAlignment(imageBrick));
#if SILVERLIGHT
			bool useImageEntryWidth = true;
#else
			bool useImageEntryWidth = imageBrick.Image != EmptyImage.Instance;
#endif
			if(useImageEntryWidth) {
				SizeF imageSize = CalculateImageSize(imageBrick, NeedSqueeze(imageBrick), exportContext.Page.Document.ScaleFactor);
				writer.WriteAttribute(XamlAttribute.Width, imageSize.Width);
				writer.WriteAttribute(XamlAttribute.Height, imageSize.Height);
				writer.WriteAttribute(XamlAttribute.Stretch, GetStretchModeName(imageBrick.SizeMode, exportContext.Page.Document.ScaleFactor));
			}
			if(exportContext.Compatibility == DevExpress.XtraPrinting.XamlExport.XamlCompatibility.WPF)
				writer.WriteAttribute(XamlAttribute.RenderOptionsBitmapScalingMode, XamlNames.Fant);
			writer.WriteEndElement();
		}
		public override bool RequiresImageResource() {
			return true;
		}
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.Content;
		}
		SizeF CalculateImageSize(ImageBrick imageBrick, bool needSqueeze, float scaleFactor) {
			if(imageBrick.SizeMode == ImageSizeMode.Normal || imageBrick.SizeMode == ImageSizeMode.CenterImage
				|| (!needSqueeze && imageBrick.SizeMode == ImageSizeMode.Squeeze)) {
				if(imageBrick.UseImageResolution) {
					float width = imageBrick.Image.Width;
					float height = imageBrick.Image.Height;
					width *= GraphicsDpi.DeviceIndependentPixel / imageBrick.Image.HorizontalResolution;
					height *= GraphicsDpi.DeviceIndependentPixel / imageBrick.Image.VerticalResolution;
					return MathMethods.Scale(new SizeF(width, height), scaleFactor);
				} else {
					float width = imageBrick.Width.DocToDip();
					float height = imageBrick.Height.DocToDip();
					return new SizeF(width, height);
				}
			}
			if(imageBrick.SizeMode == ImageSizeMode.AutoSize || imageBrick.SizeMode == ImageSizeMode.StretchImage || imageBrick.SizeMode == ImageSizeMode.ZoomImage
				|| (needSqueeze && imageBrick.SizeMode == ImageSizeMode.Squeeze) || imageBrick.SizeMode == ImageSizeMode.Tile) {
				RectangleF imageArea = GetBorderBoundsInPixels(imageBrick);
				return new SizeF(imageArea.Width, imageArea.Height);
			}
			return SizeF.Empty;
		}
		static bool NeedSqueeze(ImageBrick imageBrick) {
			SizeF brickSize = imageBrick.Size.DocToDip();
			return imageBrick.Image.Width > brickSize.Width || imageBrick.Image.Height > brickSize.Height;
		}
		static string GetVerticalAlignment(ImageBrick imageBrick) {
			switch(imageBrick.SizeMode) {
				case ImageSizeMode.AutoSize:
				case ImageSizeMode.Normal:
				case ImageSizeMode.StretchImage:
				case ImageSizeMode.Tile:
					return XamlNames.AlignmentTop;
				case ImageSizeMode.CenterImage:
				case ImageSizeMode.Squeeze:
				case ImageSizeMode.ZoomImage:
					return XamlNames.AlignmentCenter;
			}
			throw new ArgumentException("imageBrick");
		}
		static string GetHorizontalAlignment(ImageBrick imageBrick) {
			switch(imageBrick.SizeMode) {
				case ImageSizeMode.AutoSize:
				case ImageSizeMode.Normal:
				case ImageSizeMode.StretchImage:
				case ImageSizeMode.Tile:
					return XamlNames.AlignmentLeft;
				case ImageSizeMode.CenterImage:
				case ImageSizeMode.Squeeze:
				case ImageSizeMode.ZoomImage:
					return XamlNames.AlignmentCenter;
			}
			throw new ArgumentException("imageBrick");
		}
		static string GetStretchModeName(ImageSizeMode sizeMode, float scaleFactor) {
			switch(sizeMode) {
				case ImageSizeMode.ZoomImage:
				case ImageSizeMode.Squeeze:
					return XamlNames.StretchUniform;
				case ImageSizeMode.StretchImage:
					return XamlNames.StretchFill;
				default:
					return scaleFactor == 1 ? XamlNames.StretchNone : XamlNames.StretchUniformToFill;
			}
		}
	}
}
