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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public class SchedulerRangeControlPainter : ViewInfoPainterBase {
		public const int NormalHeaderState = 0;
		public const int HotTrackHeaderState = 1;
		public SchedulerRangeControlPainter() {
			CellPainter = new SchedulerRangeControlCellPainter<DataItemThumbnail>();
		}
		public SchedulerRangeControlCellPainter<DataItemThumbnail> CellPainter { get; private set; }
		public virtual void DrawRuler(RangeControlPaintEventArgs e, ScaleBasedRangeControlViewInfo viewInfo) {
			Rectangle hoveredHeaderRect = GetHoveredHeaderRect(e);
			int count = viewInfo.RulerCount;
			for (int i = 0; i < count; i++) {
				DrawHorizontalRuler(e, viewInfo.RulersBounds[i], viewInfo, i, hoveredHeaderRect);
			}
		}
		protected virtual Rectangle GetHoveredHeaderRect(RangeControlPaintEventArgs e) {
			if (e.HotInfo.HitTest == RangeControlHitTest.Client) {
				ScaleBasedRangeControlHitTest clientHitTest = (ScaleBasedRangeControlHitTest)e.HotInfo.ClientHitTest;
				if (clientHitTest == ScaleBasedRangeControlHitTest.RulerHeader) {
					return e.HotInfo.ObjectBounds;
				}
			}
			return Rectangle.Empty;
		}
		protected void DrawHorizontalRuler(RangeControlPaintEventArgs e, Rectangle bounds, ScaleBasedRangeControlViewInfo viewInfo, int rulerIndex, Rectangle hoveredHeaderRect) {
			RangeControlRulerViewInfo rulerViewInfo = viewInfo.RulerViewInfos[rulerIndex];
			SkinBorder headerBorder = viewInfo.GetRulerHeaderBorder();
			int count = rulerViewInfo.HeaderRects.Count;
			for (int i = 0; i < count; i++) {
				Rectangle contentBounds = rulerViewInfo.ContentRects[i];
				DrawHeaderBackground(e, viewInfo, contentBounds, hoveredHeaderRect);
				DrawHeaderText(e, viewInfo.PaintAppearance, rulerViewInfo.HeaderCaptions[i], rulerViewInfo.GetTextOutRect(i));
				DrawHeaderBorders(e, headerBorder, rulerViewInfo.HeaderRects[i]);
			}
			DrawHeaderBottomBorders(e, headerBorder, bounds);
		}
		protected virtual void DrawHeaderBackground(RangeControlPaintEventArgs e, ScaleBasedRangeControlViewInfo viewInfo, Rectangle contentBounds, Rectangle hoveredHeaderRect) {
			SkinElementInfo el = viewInfo.GetSkinElementInfo(EditorsSkins.SkinRangeControlRulerHeader, contentBounds);
			int imageIndex = (hoveredHeaderRect != Rectangle.Empty && contentBounds == hoveredHeaderRect) ? HotTrackHeaderState : NormalHeaderState;
			el.ImageIndex = imageIndex;
			ObjectPainter.DrawObject((GraphicsCache)e.Cache, SkinElementPainter.Default, el);
		}
		protected virtual void DrawHeaderBottomBorders(RangeControlPaintEventArgs e, SkinBorder headerBorder, Rectangle bounds) {
			int thin = headerBorder.Thin.Bottom;
			if (thin <= 0)
				return;
			Brush bb = e.Cache.GetSolidBrush(headerBorder.Bottom);
			e.Graphics.FillRectangle(bb, new Rectangle(bounds.X, bounds.Bottom - thin, bounds.Right, thin));
		}
		protected virtual void DrawHeaderText(RangeControlPaintEventArgs e, AppearanceObject appearance, string caption, Rectangle textBounds) {
			appearance.DrawString((GraphicsCache)e.Cache, caption, textBounds, e.Cache.GetSolidBrush(e.LabelColor), appearance.GetStringFormat(TextOptions.DefaultOptionsCenteredWithEllipsis));
		}
		protected virtual void DrawHeaderBorders(RangeControlPaintEventArgs e, SkinBorder headerBorder, Rectangle bounds) {
			int right = headerBorder.Thin.Right;
			if (right > 0) {
				Brush rbb = e.Cache.GetSolidBrush(headerBorder.Right);
				e.Graphics.FillRectangle(rbb, new Rectangle(bounds.Right - right, bounds.Top, right, bounds.Height));
			}
			int left = headerBorder.Thin.Left;
			if (left > 0) {
				Brush lbb = e.Cache.GetSolidBrush(headerBorder.Left);
				e.Graphics.FillRectangle(lbb, new Rectangle(bounds.Left, bounds.Top, left, bounds.Height));
			}
		}
		public virtual void DrawContent(RangeControlPaintEventArgs e, ScaleBasedRangeControlViewInfo viewInfo) {
			DrawBackground(e, viewInfo);
				DrawData(e, viewInfo);
		}
		protected virtual void DrawBackground(RangeControlPaintEventArgs e, ScaleBasedRangeControlViewInfo viewInfo) {
			e.Cache.FillRectangle(viewInfo.ContentBackColor, viewInfo.ContentBounds);
		}
		protected virtual void DrawData(RangeControlPaintEventArgs e, ScaleBasedRangeControlViewInfo viewInfo) {
			List<DataItemThumbnailList> thumbnailData = viewInfo.ThumbnailData;
			int count = thumbnailData.Count;
			if (count == 0)
				return;
			CellPainter.Appearance = viewInfo.CellAppearance;
			RangeControlRulerViewInfo rulerViewInfo = viewInfo.RulerViewInfos[viewInfo.RulerCount - 1];
			Rectangle contentBounds = viewInfo.ContentBounds;
			XtraSchedulerDebug.Assert(rulerViewInfo.HeaderRects.Count == (thumbnailData.Count));
			for (int i = 0; i < count; i++) {
				Rectangle headerRect = rulerViewInfo.ContentRects[i];
				Rectangle cellRect = new Rectangle(headerRect.X, contentBounds.Y, headerRect.Width, contentBounds.Height);
				CellPainter.DrawCell(e, cellRect, thumbnailData[i], viewInfo);
			}
		}
	}
	public class SchedulerRangeControlCellPainter<T> : ViewInfoPainterBase where T : IDataItemThumbnail {
		public SchedulerRangeControlCellPainter() {
		}
		public AppearanceObject Appearance { get; set; }
		public virtual void DrawCell(RangeControlPaintEventArgs e, Rectangle bounds, DataItemThumbnailList thumbnailData, ScaleBasedRangeControlViewInfo viewInfo) {
			RangeControlDataDisplayType displayType = viewInfo.DataDisplayType;
			if (displayType == RangeControlDataDisplayType.Thumbnail) {
				DrawThumbnail(e, bounds, thumbnailData.Interval, thumbnailData.Thumbnails, viewInfo);
				return;
			}
			int count = thumbnailData.Thumbnails.Count;
			bool showAsNumber = (displayType == RangeControlDataDisplayType.Number) || (count >= viewInfo.MaxThumbnailRowCount);
			if (showAsNumber) {
				bool canShow = count > 0 || (count == 0 && viewInfo.ShowThumbnailZeroNumber);
				if (canShow) {
					DrawNumber(e, bounds, count.ToString());
				}
			} else
				DrawThumbnail(e, bounds, thumbnailData.Interval, thumbnailData.Thumbnails, viewInfo);
		}
		protected virtual void DrawThumbnail(RangeControlPaintEventArgs e, Rectangle contentBounds, TimeInterval interval, List<IDataItemThumbnail> thumbnails, ScaleBasedRangeControlViewInfo viewInfo) {
			int height = viewInfo.ThumbnailRowHeight;
			int count = thumbnails.Count;
			contentBounds.X += 1;
			contentBounds.Width -= 1;
			Rectangle rect = new Rectangle(contentBounds.X, contentBounds.Bottom - height, contentBounds.Width, height);
			int delta = height + viewInfo.ThumbnailRowIndent;
			for ( int i = 0; i < count; i++ ) {
				if ( rect.Top <= contentBounds.Top )
					break;
				DataItemThumbnail tn = thumbnails[i] as DataItemThumbnail;
				if ( tn == null )
					continue;
				Color color = CalculateThumbnailColor(tn, interval, viewInfo);
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(color), rect);
				rect.Offset(0, -delta);
			}
		}
		private Color CalculateThumbnailColor(DataItemThumbnail thumbnail, TimeInterval interval, ScaleBasedRangeControlViewInfo viewInfo) {
			return (thumbnail.Color == SystemColors.Window) ? viewInfo.DefaultElementColor : viewInfo.GetActualAppointmentColor(thumbnail.Color);
		}
		protected virtual void DrawNumber(RangeControlPaintEventArgs e, Rectangle bounds, string numberText) {
			StringFormat format = Appearance.GetStringFormat(Appearance.TextOptions);
			e.Graphics.DrawString(numberText, Appearance.Font, Appearance.GetForeBrush((GraphicsCache)e.Cache), bounds, format);
		}
	}
}
