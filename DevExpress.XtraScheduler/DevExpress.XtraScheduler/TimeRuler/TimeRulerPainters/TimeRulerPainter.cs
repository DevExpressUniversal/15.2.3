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

using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeRulerPainter : BorderObjectPainter, IViewInfoItemPainter {
		#region Properties
		public virtual int HorizontalOverlap { get { return 0; } }
		public virtual int VerticalOverlap { get { return 0; } }
		public virtual int ContentSpan { get { return 4; } }
		public virtual int HourSeparatorLineOffset { get { return -4; } }
		public virtual int CurrentTimeAreaHeight { get { return 7; } }
		public virtual int CurrentTimelineHeight { get { return 1; } }
		public virtual int GapBetweenHourAndMinutes { get { return 1; } }
		public virtual int TopLineWidth { get { return 1; } }
		public virtual int SeparatorLineWidth { get { return 1; } }
		#endregion
		public virtual void DrawTimeRulers(GraphicsCache cache, TimeRulerViewInfoCollection rulers, ISupportCustomDraw provider) {
			int count = rulers.Count;
			for (int i = 0; i < count; i++)
				DrawTimeRuler(cache, rulers[i], provider);
		}
		protected internal virtual bool RaiseTimeRulerCustomDrawEvent(GraphicsCache cache, TimeRulerViewInfo ruler, ISupportCustomDraw provider, DefaultDrawDelegate defaultDrawDelegate) {
			ruler.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(ruler, ruler.Bounds, defaultDrawDelegate);
				provider.RaiseCustomDrawDayViewTimeRuler(args);
				return args.Handled;
			} finally {
				ruler.Cache = null;
			}
		}
		protected virtual void DrawTimeRuler(GraphicsCache cache, TimeRulerViewInfo ruler, ISupportCustomDraw provider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawTimeRulerCore(cache, ruler); };
			if (RaiseTimeRulerCustomDrawEvent(cache, ruler, provider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawTimeRulerCore(GraphicsCache cache, TimeRulerViewInfo ruler) {
			DrawBackground(cache, ruler);
			using (IntersectClipper clipper = new IntersectClipper(cache, ruler.ContentBounds)) {
				DrawCurrentTimeArea(cache, ruler);
				DrawItems(cache, ruler.Items);
				DrawCurrentTimeline(cache, ruler);
			}
			DrawBorders(cache, ruler);
			DrawHeader(cache, ruler);
			if (ruler.HeaderCaptionItem != null)
				ruler.HeaderCaptionItem.Draw(cache, this);
		}
		protected virtual void DrawCurrentTimeArea(GraphicsCache cache, TimeRulerViewInfo ruler) {
			if (ruler.CurrentTimeItems.Count == 2)
				ruler.CurrentTimeItems[0].Draw(cache, this);
		}
		protected virtual void DrawCurrentTimeline(GraphicsCache cache, TimeRulerViewInfo ruler) {
			if (ruler.CurrentTimeItems.Count == 2)
				ruler.CurrentTimeItems[1].Draw(cache, this);
		}
		protected virtual void DrawItems(GraphicsCache cache, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].Draw(cache, this);
		}
		protected internal virtual void DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
			item.Appearance.DrawString(cache, item.Text, item.Bounds);
		}
		protected internal virtual void DrawSeparatorLineItem(GraphicsCache cache, TimeRulerSeparatorLineItem item) {
			item.Appearance.FillRectangle(cache, item.Bounds);
		}
		protected internal virtual void DrawCurrentTimelineItem(GraphicsCache cache, TimeRulerCurrentTimelineBaseItem item) {
			DrawSeparatorLineItem(cache, item);
		}
		protected virtual void DrawBackground(GraphicsCache cache, TimeRulerViewInfo ruler) {
			ruler.BackgroundAppearance.FillRectangle(cache, ruler.ContentBounds);
		}
		protected virtual void DrawHeader(GraphicsCache cache, TimeRulerViewInfo ruler) {
			ruler.BackgroundAppearance.FillRectangle(cache, ruler.HeaderBounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			TimeRulerViewInfo ruler = (TimeRulerViewInfo)viewInfo;
			cache.FillRectangle(ruler.BackgroundAppearance.GetBorderBrush(cache), viewInfo.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			TimeRulerViewInfo ruler = (TimeRulerViewInfo)viewInfo;
			cache.FillRectangle(ruler.BackgroundAppearance.GetBorderBrush(cache), viewInfo.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected internal virtual Rectangle CalcClientBounds(GraphicsCache cache, Rectangle bounds) {
			return bounds;
		}
		protected internal virtual int GetFullWidthByContentWidth(GraphicsCache cache, int contentWidth) {
			return contentWidth;
		}
		protected internal virtual Rectangle CalcHeaderContentBounds(GraphicsCache cache, TimeRulerViewInfo ruler) {
			Rectangle result = ruler.HeaderBounds;
			result.Inflate(-2, -2);
			return result;
		}
		#region IViewInfoItemPainter implementation
		void IViewInfoItemPainter.DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
			this.DrawTextItem(cache, item);
		}
		void IViewInfoItemPainter.DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
		}
		void IViewInfoItemPainter.DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
		}
		void IViewInfoItemPainter.DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
		}
		#endregion
	}
}
