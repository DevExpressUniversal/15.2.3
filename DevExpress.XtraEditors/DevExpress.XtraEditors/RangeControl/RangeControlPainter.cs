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
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.Drawing;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace DevExpress.XtraEditors {
	public class RangeControlPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			DrawBackground(info, viewInfo);
			if(viewInfo.RangeControl.Client == null) {
				DrawNoClientText(info);
				return;
			}
			if(!viewInfo.RangeControl.IsClientValid) {
				DrawInvalidText(info);
				return;
			}
			if(viewInfo.RangeControl.ShowZoomScrollBar)
				DrawScrollBar(info);
			DrawClient(info);
			if(viewInfo.RangeControl.AllowSelection)
				DrawOutOfRangeMask(info);
			if(viewInfo.RangeControl.ShowLabels)
				DrawRuler(info);
			if(viewInfo.RangeControl.AllowSelection){
				DrawSelection(info);
				DrawRangeBox(info);
				if(viewInfo.RangeControl.SelectionType != RangeControlSelectionType.Thumb)
					DrawFlags(info);
				if(viewInfo.RangeControl.SelectionType != RangeControlSelectionType.Flag) {
					DrawMinRangeThumb(info);
					DrawMaxRangeThumb(info);
				}
			}
			if(!viewInfo.RangeControl.Enabled) {
				DrawDisabledMask(info);
			}
		}
		protected virtual void DrawBackground(ControlGraphicsInfoArgs info, RangeControlViewInfo viewInfo) {
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.BackColor), viewInfo.Bounds);
		}
		protected virtual void DrawDisabledMask(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(Color.FromArgb(10, 0, 0, 0)), viewInfo.Bounds);
		}
		protected virtual void DrawNoClientText(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			info.ViewInfo.PaintAppearance.DrawString(info.Cache, Properties.Resources.RangeControlWarning, viewInfo.ContentRect, info.Cache.GetSolidBrush(viewInfo.LabelColor), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
		}
		protected virtual void DrawInvalidText(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			info.ViewInfo.PaintAppearance.DrawString(info.Cache, viewInfo.RangeControl.Client.InvalidText, viewInfo.ContentRect, info.Cache.GetSolidBrush(viewInfo.LabelColor), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
		}
		protected virtual RangeControlPaintEventArgs CreatePaintArgs(GraphicsCache cache, ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			RangeControlPaintEventArgs res = new RangeControlPaintEventArgs()
			{
				RangeControl = viewInfo.RangeControl,
				Cache = cache,
				ContentBounds = viewInfo.RangeClientBounds,
				HotInfo = viewInfo.HotInfo,
				PressedInfo = viewInfo.PressedInfo,
				ActualRangeMaximum = viewInfo.RangeMaximum,
				ActualRangeMinimum = viewInfo.RangeMinimum
			};
			if(viewInfo.IsVertical) {
				res.ContentBounds = new Rectangle(res.ContentBounds.Y, 0, res.ContentBounds.Height, res.ContentBounds.Width);
			}
			return res;
		}
		protected virtual void DrawRuler(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			viewInfo.Calculator.DrawRuler(info, viewInfo.PaintAppearance, viewInfo.RangeControl.Client, CreatePaintArgs(info.Cache, info), viewInfo.Ruler);
		}
		protected virtual void DrawSelection(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			if(!viewInfo.HasSelection)
				return;
			SkinPaddingEdges edges = viewInfo.IsHorizontal? new SkinPaddingEdges(1,0,1,0): new SkinPaddingEdges(0,1,0,1);
			SelectionPainter.Default.Draw(info.Cache, viewInfo.LookAndFeel.ActiveLookAndFeel, viewInfo.SelectionBounds, edges);
		}
		bool IsValueBetween(int value, int min, int max) {
			return min < value && max > value;
		}
		bool IsValueInRectangle(Point value, Rectangle rect, bool isVertical) {
			return (!isVertical && rect.X < value.X && rect.Right > value.X) || 
				(isVertical && rect.Y < value.Y && rect.Bottom > value.Y);
		}
		protected virtual void DrawScrollBar(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			Pen bp = info.Cache.GetPen(viewInfo.BorderColor);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.ScrollAreaColor), viewInfo.ScrollBarAreaBounds);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.ViewPortPreviewColor), viewInfo.ScrollBarThumbBounds);
			SkinElementInfo leftSizeInfo = viewInfo.GetLeftSizingGlyphInfo();
			if(leftSizeInfo != null)
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, leftSizeInfo);
			SkinElementInfo rightSizeInfo = viewInfo.GetRightSizingGlyphInfo();
			if(rightSizeInfo != null)
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, rightSizeInfo);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.RangePreviewColor), viewInfo.RangeIndicatorBounds);
			bp.DashStyle = DashStyle.Solid;
			if(IsValueInRectangle(viewInfo.ScrollBarThumbBounds.Location, viewInfo.RangeIndicatorBounds, viewInfo.IsVertical))
				bp.DashStyle = DashStyle.Dot;
			if(viewInfo.IsHorizontal)
				info.Graphics.DrawLine(bp, viewInfo.ScrollBarThumbBounds.Location, new Point(viewInfo.ScrollBarThumbBounds.X, viewInfo.ScrollBarThumbBounds.Bottom));
			else
				info.Graphics.DrawLine(bp, viewInfo.ScrollBarThumbBounds.Location, new Point(viewInfo.ScrollBarThumbBounds.Right, viewInfo.ScrollBarThumbBounds.Y));
			bp.DashStyle = DashStyle.Solid;
			if(IsValueInRectangle(new Point(viewInfo.ScrollBarThumbBounds.Right, viewInfo.ScrollBarThumbBounds.Bottom), viewInfo.RangeIndicatorBounds, viewInfo.IsVertical))
				bp.DashStyle = DashStyle.Dot;
			if(viewInfo.IsHorizontal)
				info.Graphics.DrawLine(bp, new Point(viewInfo.ScrollBarThumbBounds.Right, viewInfo.ScrollBarThumbBounds.Y), new Point(viewInfo.ScrollBarThumbBounds.Right, viewInfo.ScrollBarThumbBounds.Bottom));
			else
				info.Graphics.DrawLine(bp, new Point(viewInfo.ScrollBarThumbBounds.X, viewInfo.ScrollBarThumbBounds.Bottom), new Point(viewInfo.ScrollBarThumbBounds.Right, viewInfo.ScrollBarThumbBounds.Bottom));
			bp.DashStyle = DashStyle.Solid;
			if(viewInfo.IsHorizontal) {
				info.Graphics.DrawLine(bp, viewInfo.RangeIndicatorBounds.Location, new Point(viewInfo.RangeIndicatorBounds.X, viewInfo.RangeIndicatorBounds.Bottom));
				info.Graphics.DrawLine(bp, new Point(viewInfo.RangeIndicatorBounds.Right, viewInfo.RangeIndicatorBounds.Y), new Point(viewInfo.RangeIndicatorBounds.Right, viewInfo.RangeIndicatorBounds.Bottom));
			}
			else {
				info.Graphics.DrawLine(bp, viewInfo.RangeIndicatorBounds.Location, new Point(viewInfo.RangeIndicatorBounds.Right, viewInfo.RangeIndicatorBounds.Y));
				info.Graphics.DrawLine(bp, new Point(viewInfo.RangeIndicatorBounds.X, viewInfo.RangeIndicatorBounds.Bottom), new Point(viewInfo.RangeIndicatorBounds.Right, viewInfo.RangeIndicatorBounds.Bottom));
			}
			if(viewInfo.IsHorizontal)
				info.Graphics.DrawLine(bp, new Point(viewInfo.ClientRect.X, viewInfo.ScrollBarAreaBounds.Y), new Point(viewInfo.ClientRect.Right, viewInfo.ScrollBarAreaBounds.Y));
			else
				if(viewInfo.IsRightToLeft)
					info.Graphics.DrawLine(bp, new Point(viewInfo.ScrollBarAreaBounds.X, viewInfo.ClientRect.Y), new Point(viewInfo.ScrollBarAreaBounds.X, viewInfo.ClientRect.Bottom));
				else info.Graphics.DrawLine(bp, new Point(viewInfo.ScrollBarAreaBounds.Right, viewInfo.ClientRect.Y), new Point(viewInfo.ScrollBarAreaBounds.Right, viewInfo.ClientRect.Bottom));
		}
		static ImageAttributes grayAttributes;
		static ImageAttributes GrayAttributes {
			get {
				if(grayAttributes == null) {
					grayAttributes = new ImageAttributes();
					#region matrices
					float[][] array = new float[][]{   
									new float[]{0.30f,0.30f,0.30f,0.00f,0},
									new float[]{0.59f,0.59f,0.59f,0.00f,0},
									new float[]{0.11f,0.11f,0.11f,0.00f,0},
									new float[]{0.00f,0.00f,0.00f,0.35f,0},
									new float[]{0.00f,0.00f,0.00f,0.00f,1}};
					#endregion
					grayAttributes = new ImageAttributes();
					grayAttributes.ClearColorKey();
					grayAttributes.SetColorMatrix(new ColorMatrix(array));
				}
				return grayAttributes;
			}
		}
		protected void DrawClient(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			if(viewInfo.RangeControl.Client == null || viewInfo.ContentImage == null)
				return;
			if(!viewInfo.IsContentImageReady) {
				viewInfo.ContentGraphicsCache.Graphics.Clear(Color.Transparent);
				viewInfo.RangeControl.Client.DrawContent(CreatePaintArgs(viewInfo.ContentGraphicsCache, info));
				viewInfo.CreateRotatedImage();
			}
			viewInfo.IsContentImageReady = true;
			if(new Rectangle(0, 0, viewInfo.RotatedImage.Width, viewInfo.RotatedImage.Height).IntersectsWith(viewInfo.RangeBounds)) {
				info.Graphics.DrawImage(viewInfo.RotatedImage, viewInfo.RangeBounds, viewInfo.RangeBoxImageBounds, GraphicsUnit.Pixel);
			}
		}
		protected virtual void DrawOutOfRangeMask(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			if(viewInfo.ContentImage == null)
				return;
			Rectangle left = viewInfo.LeftOutOfRangeBounds;
			if(left.Width > 0) {
				Rectangle r = viewInfo.LeftOutOfRangeImageBounds;
				info.Graphics.DrawImage(viewInfo.RotatedImage, left, r.X, r.Y, r.Width, r.Height, GraphicsUnit.Pixel, GrayAttributes, null);
				info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.OutOfRangeMaskColor), viewInfo.LeftOutOfRangeBounds);
			}
			Rectangle right = viewInfo.RightOutOfRangeBounds;
			if(right.Width > 0) {
				Rectangle r = viewInfo.RightOutOfRangeImageBounds;
				info.Graphics.DrawImage(viewInfo.RotatedImage, right, r.X, r.Y, r.Width, r.Height, GraphicsUnit.Pixel, GrayAttributes, null);
				info.Graphics.FillRectangle(info.Cache.GetSolidBrush(viewInfo.OutOfRangeMaskColor), viewInfo.RightOutOfRangeBounds);
			}
		}
		protected virtual void DrawMaxRangeThumb(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			SkinElementInfo sinfo = viewInfo.GetMaxRangeThumbInfo();
			if(sinfo != null) {
				if(viewInfo.IsVertical)
					new RotateObjectPaintHelper().DrawRotated(info.Cache, sinfo, SkinElementPainter.Default, RotateFlipType.Rotate90FlipNone);
				else
					ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, sinfo);
				return;
			}
			Brush brush = Brushes.Green;
			if(viewInfo.PressedInfo.HitTest == RangeControlHitTest.MaxRangeThumb)
				brush = Brushes.DarkGreen;
			else if(viewInfo.HotInfo.HitTest == RangeControlHitTest.MaxRangeThumb)
				brush = Brushes.LightGreen;
			info.Graphics.FillRectangle(brush, viewInfo.MaxRangeThumbBounds);
		}
		protected virtual void DrawMinRangeThumb(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			SkinElementInfo sinfo = viewInfo.GetMinRangeThumbInfo();
			if(sinfo != null) {
				if(viewInfo.IsVertical)
					new RotateObjectPaintHelper().DrawRotated(info.Cache, sinfo, SkinElementPainter.Default, RotateFlipType.Rotate90FlipNone);
				else
					ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, sinfo);
				return;
			}
			Brush brush = Brushes.Green;
			if(viewInfo.PressedInfo.HitTest == RangeControlHitTest.MinRangeThumb)
				brush = Brushes.DarkGreen;
			else if(viewInfo.HotInfo.HitTest == RangeControlHitTest.MinRangeThumb)
				brush = Brushes.LightGreen;
			info.Graphics.FillRectangle(brush, viewInfo.MinRangeThumbBounds);
		}
		protected virtual void DrawFlags(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			viewInfo.Calculator.DrawFlags(info, viewInfo.RangeControl.Client, viewInfo.PaintAppearance, viewInfo.RangeMinimum, viewInfo.RangeMaximum);
		}
		protected virtual void DrawRangeBox(ControlGraphicsInfoArgs info) {
			RangeControlViewInfo viewInfo = (RangeControlViewInfo)info.ViewInfo;
			Pen pen = info.Cache.GetPen(viewInfo.BorderColor);
			if(viewInfo.IsHorizontal) {
				info.Graphics.DrawLine(pen, viewInfo.RangeBounds.X, viewInfo.RangeBounds.Y, viewInfo.RangeBounds.X, viewInfo.RangeBounds.Bottom);
				info.Graphics.DrawLine(pen, viewInfo.RangeBounds.Right, viewInfo.RangeBounds.Y, viewInfo.RangeBounds.Right, viewInfo.RangeBounds.Bottom);
			}
			else {
				info.Graphics.DrawLine(pen, viewInfo.RangeBounds.X, viewInfo.RangeBounds.Y, viewInfo.RangeBounds.Right, viewInfo.RangeBounds.Y);
				info.Graphics.DrawLine(pen, viewInfo.RangeBounds.X, viewInfo.RangeBounds.Bottom, viewInfo.RangeBounds.Right, viewInfo.RangeBounds.Bottom);
			}
		}
	}
}
