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
using System.Drawing.Imaging;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	public abstract class BandCaptionPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			BandCaptionViewInfo viewInfo = (BandCaptionViewInfo)e;
			DrawBackground(viewInfo);
			DrawBorder(viewInfo);
			DrawButton(viewInfo);
			DrawImage(viewInfo);
			DrawText(viewInfo);
		}
		protected virtual void DrawBorder(BandCaptionViewInfo viewInfo) {
		}
		protected virtual void DrawBackground(BandCaptionViewInfo viewInfo) {
		}
		protected virtual void DrawButton(BandCaptionViewInfo viewInfo) {
		}
		protected virtual void DrawImage(BandCaptionViewInfo viewInfo) {
		}
		protected virtual void DrawText(BandCaptionViewInfo viewInfo) {
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			BandCaptionViewInfo viewInfo = (BandCaptionViewInfo)e;
			return viewInfo.Bounds;
		}
	}
	public class BandCaptionPainterFlat : BandCaptionPainter {
		static Color backgrColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
		static Color selectBackgrColor = Color.FromArgb(0xdc, 0xea, 0xfb);
		static Color borderColor = Color.FromArgb(0xce, 0xce, 0xce);
		protected override void DrawBackground(BandCaptionViewInfo viewInfo) {
			viewInfo.Cache.FillRectangle(GetBackgroundBrush(viewInfo), viewInfo.Bounds);
		}
		protected virtual Brush GetBackgroundBrush(BandCaptionViewInfo viewInfo) {
			if(viewInfo.Locked)
				return Brushes.LightGray;
			return viewInfo.Cache.GetSolidBrush(GetBackgrColor(viewInfo.Selected));
		}
		static Color GetBackgrColor(bool selected) {
			return selected ? selectBackgrColor : backgrColor;
		}
		protected override void DrawBorder(BandCaptionViewInfo viewInfo) {			
			Brush brush = viewInfo.Cache.GetSolidBrush(borderColor);
			if(viewInfo.HasTopBorder)
				viewInfo.Graphics.FillRectangle(brush, new Rectangle(viewInfo.Bounds.X, viewInfo.Bounds.Y, viewInfo.Bounds.Width, 1));
			if(viewInfo.HasBottomBorder)
				viewInfo.Graphics.FillRectangle(brush, new Rectangle(viewInfo.Bounds.X, viewInfo.Bounds.Bottom - 1, viewInfo.Bounds.Width, 1));
		}
		protected override void DrawButton(BandCaptionViewInfo viewInfo) {
			RectangleF buttonBounds = RectF.Center(new RectangleF(0, 0, 11, 11), viewInfo.ButtonBounds);
			int level = Math.Min(Math.Max(viewInfo.Level, 0), 2);
			float alpha = 0f;
			switch(level) {
				case 0:
					alpha = 1f;
					break;
				case 1:
					alpha = 0.8f;
					break;
				case 2:
					alpha = 0.6f;
					break;
			}
			Bitmap bmp;
			PointF location = buttonBounds.Location;
			if(viewInfo.Expanded) {
				location.Y++;
				bmp = XRBitmaps.BandCaptionExpanded;
			} else
				bmp = XRBitmaps.BandCaptionCollapsed;
			DrawTransparentImage(viewInfo.Graphics, bmp, location, alpha);
		}
		static void DrawTransparentImage(Graphics gr, Bitmap bmp, PointF location, float alpha) {
			using(ImageAttributes attributes = new ImageAttributes()) {
				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix33 = alpha;
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				Point locationPx = Point.Round(location);
				gr.DrawImage(bmp,
					new Point[] {
						locationPx,
						new Point(locationPx.X + bmp.Width, locationPx.Y),
						new Point(locationPx.X, locationPx.Y + bmp.Height)
					},
					new Rectangle(Point.Empty, bmp.Size),
					GraphicsUnit.Pixel,
					attributes
				);
			}
		}
		protected override void DrawText(BandCaptionViewInfo viewInfo) {
			Brush foreBrush = viewInfo.Cache.GetSolidBrush(Color.FromArgb(0x75, 0x75, 0x75));
			viewInfo.Graphics.DrawString(viewInfo.Text, BandCaptionViewInfo.Font, foreBrush, viewInfo.TextBounds, BandCaptionViewInfo.StringFormat);
		}
		protected override void DrawImage(BandCaptionViewInfo viewInfo) {
			if(viewInfo.Image != null)
				viewInfo.Graphics.DrawImage(viewInfo.Image, viewInfo.ImageBounds);
		}
	}
}
