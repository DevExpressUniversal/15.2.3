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

using System.Collections.Generic;
using System.Windows;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Utils;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
using DevExpress.XtraPrinting.Native;
#if SL
using GdiImage = DevExpress.Xpf.Drawing.Image;
#else
using System.Windows.Interop;
using System.Windows.Forms;
using GdiImage = System.Drawing.Image;
#endif
namespace DevExpress.Xpf.Printing.BrickCollection {
	class ImageBrickCreator : BrickCreator {
		public ImageBrickCreator(PrintingSystemBase ps, Dictionary<BrickStyleKey, BrickStyle> brickStyles, Dictionary<IVisualBrick, IOnPageUpdater> onPageUpdaters)
			: base(ps, brickStyles, onPageUpdaters) {
		}
		public override VisualBrick Create(UIElement source, UIElement parent) {
			Guard.ArgumentNotNull(source, "source");
			Guard.ArgumentNotNull(parent, "parent");
			IImageExportSettings exportSettings = new EffectiveImageExportSettings(source);
			ImageBrick brick = new ImageBrick();
			InitializeBrickCore(source, parent, brick, exportSettings);
			FrameworkElement sourceElement = exportSettings.SourceElement ?? (FrameworkElement)source;
			switch(exportSettings.ImageRenderMode) {
				case ImageRenderMode.MakeScreenshot:
					object key = exportSettings.ImageKey != null ? new MultiKey(new object[] { exportSettings.ImageKey, sourceElement.ActualWidth, sourceElement.ActualHeight }) : null;
					brick.Image = GetImageFromCache(ps.Images, key, () => CreateImage(sourceElement));
					brick.SizeMode = exportSettings.ForceCenterImageMode ? ImageSizeMode.CenterImage : ImageSizeMode.Normal;
					break;
				case ImageRenderMode.UseImageSource:
					Image image = exportSettings.SourceElement as Image;
					if(image == null) {
						throw new InvalidOperationException();
					}					
					BitmapSource bitmapSource = image.Source as BitmapSource;
					if(bitmapSource != null) {
						key = exportSettings.ImageKey ?? bitmapSource;
						brick.Image = GetImageFromCache(ps.Images, key, () => DrawingConverter.FromBitmapSource(bitmapSource));
					}
					brick.SizeMode = exportSettings.ForceCenterImageMode ? ImageSizeMode.CenterImage : GetImageSizeModeFromStretch(image.Stretch);
					break;
			}
#if !SILVERLIGHT
			brick.UseImageResolution = true;
#endif
			return brick;
		}
		static GdiImage GetImageFromCache(ImagesContainer container, object key, Func<GdiImage> createIfNotExists) {
			if(key != null) {
				GdiImage gdiImage = container.GetImageByKey(key);
				if(gdiImage == null) {
					gdiImage = createIfNotExists();
					if(gdiImage != null) {
						container.Add(key, gdiImage);
					}
				}
				return gdiImage;
			} else {
				GdiImage gdiImage = createIfNotExists();
				return container.GetImage(gdiImage);
			}
		}
		static ImageSizeMode GetImageSizeModeFromStretch(Stretch stretch) {
			ImageSizeMode sizeMode = ImageSizeMode.Normal;
			switch(stretch) {
				case Stretch.None: sizeMode = ImageSizeMode.Normal; break;
				case Stretch.Fill: sizeMode = ImageSizeMode.StretchImage; break;
				case Stretch.Uniform: sizeMode = ImageSizeMode.ZoomImage; break;
				case Stretch.UniformToFill: throw new NotSupportedException("UniformToFill mode is not supported");
			}
			return sizeMode;
		}
		GdiImage CreateImage(FrameworkElement source) {
			return DrawingConverter.CreateGdiImage(source);
		}
	}
}
