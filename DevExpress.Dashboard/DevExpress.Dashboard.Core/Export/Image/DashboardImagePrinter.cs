#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraPrinting;
using System.Drawing;
using System;
namespace DevExpress.DashboardExport {
	public class DashboardImagePrinter : DashboardExportPrinter {
		readonly Image image;
		readonly ImageSizeMode sizeMode;
		readonly Size size;
		readonly DevExpress.DashboardCommon.ImageHorizontalAlignment horizontalAligment;
		readonly DevExpress.DashboardCommon.ImageVerticalAlignment verticalAligment;
		bool ImageIsSmallerThenClientSize { get { return image.Width < size.Width && image.Height < size.Height; } }
		bool IsPassiveSqueezeImageMode { get { return sizeMode == ImageSizeMode.Squeeze && ImageIsSmallerThenClientSize; } }
		bool IsActiveSqueezeImageMode { get { return sizeMode == ImageSizeMode.Squeeze && !ImageIsSmallerThenClientSize; } }
		public DashboardImagePrinter(Image image, string url, ImageSizeMode sizeMode, Size size,
			DevExpress.DashboardCommon.ImageHorizontalAlignment horizontalAligment, DevExpress.DashboardCommon.ImageVerticalAlignment verticalAligment) {
			this.image = image;
			if(image == null && !string.IsNullOrEmpty(url))
				this.image = ExportHelper.DownloadImage(url);
			this.sizeMode = sizeMode;
			this.size = size;
			this.horizontalAligment = horizontalAligment;
			this.verticalAligment = verticalAligment;
		}
		protected override void CreateDetail(IBrickGraphics graph) {
			IImageBrick imageBrick = PS.CreateImageBrick();
			imageBrick.Image = image;
			imageBrick.SizeMode = sizeMode;
			imageBrick.Sides = BorderSide.None;
			imageBrick.Rect = PrepareImageAligmentBounds();
			IPanelBrick panel = PS.CreatePanelBrick();
			panel.Sides = BorderSide.None;
			panel.Bricks.Add(imageBrick);
			Rectangle parentBounds = new Rectangle(new Point(0, 0), size);
			graph.DrawBrick(panel, parentBounds);
		}
		protected Rectangle PrepareImageAligmentBounds() {
			if(image == null)
				return new Rectangle(new Point(0, 0), size);
			int x = 0;
			int y = 0;
			Size brickSize = size;
			if(sizeMode == ImageSizeMode.AutoSize || IsPassiveSqueezeImageMode) {
				x = ApplyAligmentToX(image.Width);
				y = ApplyAligmentToY(image.Height);
				brickSize = image.Size;
			}
			if(sizeMode == ImageSizeMode.ZoomImage || IsActiveSqueezeImageMode) {
				float widthProp = (float)image.Width / (float)size.Width;
				float heightProp = (float)image.Height / (float)size.Height;
				if(widthProp > heightProp) {
					int zoomImageHeight = (int)(image.Height / widthProp);
					y = ApplyAligmentToY(zoomImageHeight);
					brickSize = new Size(size.Width, zoomImageHeight);
				}
				else {
					int zoomImageWidth = (int)(image.Width / heightProp);
					x = ApplyAligmentToX(zoomImageWidth);
					brickSize = new Size(zoomImageWidth, size.Height);
				}
			}
			return new Rectangle(new Point(x, y), brickSize);
		}
		protected int ApplyAligmentToX(int actualImageWidth) {
			int x = 0;
			if(horizontalAligment == DashboardCommon.ImageHorizontalAlignment.Center)
				x = -(actualImageWidth - size.Width) / 2;
			if(horizontalAligment == DashboardCommon.ImageHorizontalAlignment.Right)
				x = -(actualImageWidth - size.Width);
			return x;
		}
		protected int ApplyAligmentToY(int actualImageHeight) {
			int y = 0;
			if(verticalAligment == DashboardCommon.ImageVerticalAlignment.Center)
				y = -(actualImageHeight - size.Height) / 2;
			if(verticalAligment == DashboardCommon.ImageVerticalAlignment.Bottom)
				y = -(actualImageHeight - size.Height);
			return y;
		}
	}
}
