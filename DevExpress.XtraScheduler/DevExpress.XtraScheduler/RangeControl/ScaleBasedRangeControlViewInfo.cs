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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Skins;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace DevExpress.XtraScheduler.Drawing {
	public enum ScaleBasedRangeControlHitTest { Unknown, Content, RulerHeader };
	public class ScaleBasedRangeControlViewInfo : BaseRangeControlViewInfo {
		const int DefaultNumberFontSize = 16;
		const int DefaultThumbnailRowHeight = 7;
		const int DefaultThumbnailRowIndent = 1;
		bool shouldReleaseGraphics = false;
		public ScaleBasedRangeControlViewInfo(ScaleBasedRangeControlClient client, RangeControl rangeControl)
			: base(rangeControl) {
			Guard.ArgumentNotNull(client, "client");
			ScaleBasedClient = client;
		}
		public bool ForceRequestData { get; set; }
		protected TimeIntervalCollection LastRequestedIntervals { get; set; }
		public ScaleBasedRangeControlClient ScaleBasedClient { get; private set; }
		public int RulerCount { get { return Controller.RulerCount; } }
		public List<DataItemThumbnailList> ThumbnailData { get; private set; }
		public virtual AppearanceObject CellAppearance { get { return ScaleBasedClient.CellAppearance; } }
		public Color DefaultElementColor { get { return GetColorProperty(EditorsSkins.OptRangeControlDefaultElementColor); } }
		public Color ElementBaseColor { get { return GetColorProperty(EditorsSkins.OptRangeControlElementBaseColor); } }
		public Color NumberForeColor { get { return GetColorProperty(EditorsSkins.OptRangeControlElementForeColor); } }
		public Color ContentBackColor { get { return BackColor; } }
		public int NumberFontSize { 
			get { 
				int size = GetIntProperty(EditorsSkins.OptRangeControlElementFontSize);
				return size > 0 ? size : DefaultNumberFontSize;
			} 
		}
		ScaleBasedRangeControlController Controller { get { return ScaleBasedClient.Controller; } }
		public SkinElementInfo GetSkinElementInfo(string skinElementName, Rectangle bounds) {
			SkinElement skinEl = GetRangeControlSkinElement(skinElementName);
			return new SkinElementInfo(skinEl, bounds);
		}
		protected SkinElement GetRangeControlSkinElement(string skinElementName) {
			Skin skin = EditorsSkins.GetSkin(RangeControl.LookAndFeel);
			return skin[skinElementName];
		}
		protected internal SkinElement GetRulerHeaderElement() {
			return GetRangeControlSkinElement(EditorsSkins.SkinRangeControlRulerHeader);
		}
		public virtual SkinPaddingEdges GetRulerHeaderMargins() {
			return GetRulerHeaderElement().ContentMargins;
		}
		public virtual SkinBorder GetRulerHeaderBorder() {
		   return GetRulerHeaderElement().Border;
		}
		public virtual Color GetActualAppointmentColor(Color appointmentBackColor) {
			ColorMatrix m = DevExpress.Utils.Paint.XPaint.GetColorMatrix(DefaultElementColor, appointmentBackColor);
			return SkinElementColorer.ApplyColorTransform(ElementBaseColor, m);
		}
		public bool ShowThumbnailZeroNumber { get; set; }
		public int MaxThumbnailRowCount { get; set; }
		public int ThumbnailHeight { get; set; }
		public RangeControlDataDisplayType DataDisplayType { get; set; }
		public virtual int ThumbnailRowHeight { 
			get {
				return ThumbnailHeight > 0 ? ThumbnailHeight : DefaultThumbnailRowHeight;
			}
		}
		public virtual int ThumbnailRowIndent { get { return DefaultThumbnailRowIndent; } }
		public int RulerAreaHeight { get; set; }
		public Rectangle RulerAreaBounds { get; set; }
		public Rectangle[] RulersBounds { get; set; }
		public Rectangle ContentBounds {  get { return GetContentBounds(); } }
		public RangeControlRulerViewInfo[] RulerViewInfos { get; set; }
		public override void Dispose() {
			try {
				ReleaseGraphics();
			} finally {
				base.Dispose();
			}
		}
		protected virtual void EnsureGraphics() {
			if (GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				shouldReleaseGraphics = true;
			}
		}
		protected virtual void ReleaseGraphics() {
			if (shouldReleaseGraphics) {
				if (GInfo != null)
					GInfo.ReleaseGraphics();
				shouldReleaseGraphics = false;
			}
		}
		public void Recalculate(Rectangle contentRect) {
			EnsureGraphics();
			InitializeCalculator();
			CalcHorizontalRects(contentRect);
			Calculator.ScrollBounds = Calculator.CalcScrollBounds(contentRect);
			CalcRulerViewInfos();
			PrepareThumbnailData();
	   }
		void InitializeCalculator() {
			Calculator.Orientation = RangeControl.Orientation;
			Calculator.Bounds = RangeControl.Bounds;
			Calculator.ClientRect = RangeControl.ClientRectangle;
			Calculator.Graphics = GInfo.Graphics;
			Calculator.VisibleRangeStartPosition = RangeControl.VisibleRangeStartPosition;
			Calculator.VisibleRangeWidth = RangeControl.VisibleRangeWidth;
			Calculator.VisibleRangeScaleFactor = RangeControl.VisibleRangeScaleFactor;			
		}
		public int GetRangeBoxTopIndent() {
			EnsureGraphics();
			UpdatePaintAppearance();
			CalcRulersAreaHeight();
			return RulerAreaHeight;
		}
		protected virtual Rectangle GetContentBounds() {
			return RectUtils.CutFromTop(base.ScrollBoundsCore, RulerAreaHeight);
		}
		protected virtual void PrepareThumbnailData() {
			TimeIntervalCollection intervals = GetBaseRulerIntervals();
			if (ShouldRequestThumbnailData(intervals)) {
			ThumbnailData = Controller.CreateThumbnailData(intervals);
				LastRequestedIntervals = intervals;
			}
		}
		protected virtual bool ShouldRequestThumbnailData(TimeIntervalCollection intervals) {
			if (ForceRequestData) {
				ForceRequestData = false;
				return true;
			}
			if (LastRequestedIntervals == null)
				return true;
			int count = intervals.Count;
			if (LastRequestedIntervals.Count != count)
				return true;
			if (!LastRequestedIntervals.Interval.Equals(intervals.Interval))
				return true;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				if (!interval.Equals(LastRequestedIntervals[i]))
					return true;
			}
			return false;
		}
		private TimeIntervalCollection GetBaseRulerIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			RangeControlRulerViewInfo rulerViewInfo = RulerViewInfos[Controller.RulerCount - 1];
			result.AddRange(rulerViewInfo.HeaderIntervals);
			return result;
		}
		protected internal virtual void CalcRulersAreaHeight() {
			RulerAreaHeight = CalculateRulersAreaHeight();
		}
		internal void CalcHorizontalRects(Rectangle contentBounds) {
			RulerAreaBounds = CalculateTotalRulersBounds(contentBounds);
			RulersBounds = SplitVertically(RulerAreaBounds, Controller.Rulers);
			MaxThumbnailRowCount = ContentBounds.Height / (ThumbnailRowHeight + ThumbnailRowIndent);
		}
		protected internal Rectangle[] SplitVertically(Rectangle bounds, List<ISchedulerRangeControlRuler> rulers) {
			int totalRulerCount = rulers.Count;
			int visibleRulerCount = CalcVisibleRulerCount(rulers);
			Rectangle[] visibleBounds = RectUtils.SplitVertically(bounds, visibleRulerCount);
			Rectangle[] totalRulerBounds = new Rectangle[totalRulerCount];
			int nextVisibleBoundsIndex = 0;
			Rectangle currentVisibleBounds = visibleBounds[nextVisibleBoundsIndex];
			for (int i = 0; i < totalRulerCount; i++) {
				ISchedulerRangeControlRuler ruler = rulers[i];
				if (ruler.Scale.Visible) {					
					totalRulerBounds[i] = currentVisibleBounds;
					nextVisibleBoundsIndex++;
					if (nextVisibleBoundsIndex < visibleRulerCount)
						currentVisibleBounds = visibleBounds[nextVisibleBoundsIndex];
				} else {
					totalRulerBounds[i] =  new Rectangle(currentVisibleBounds.X, currentVisibleBounds.Y, currentVisibleBounds.Width, 0);
				}
			}
			return totalRulerBounds;
		}
		int CalcVisibleRulerCount(List<ISchedulerRangeControlRuler> rulers) {
			int count = rulers.Count;
			int resultCount = 0;
			for (int i = 0; i < count; i++) {
				if (rulers[i].Scale.Visible)
					resultCount++;
			}
			return resultCount;
		}
		protected virtual void CalcRulerViewInfos() {
			int count = RulersBounds.Length;
			RulerViewInfos =  new RangeControlRulerViewInfo[count];
			for (int i = 0; i < count; i++) {
				RangeControlRulerViewInfo rulerViewInfo = CreateRulerViewInfo(i);
				ISchedulerRangeControlRuler ruler = Controller.Rulers[i];
				CalcRulerViewInfo(ruler, rulerViewInfo, GInfo.Graphics);
				if (!ruler.Scale.Visible)
					continue;
				if (ScaleBasedClient.Options.AutoFormatScaleCaptions) {
					string optimalFormat = CalculateRulerHeaderOptimalFormat(ruler.Scale, rulerViewInfo, GInfo.Graphics, PaintAppearance.Font);
					UpdateRulerViewInfoCaptions(ruler.Scale, rulerViewInfo, optimalFormat);
				}
			}
		}
		private void UpdateRulerViewInfoCaptions(TimeScale scale, RangeControlRulerViewInfo rulerViewInfo, string optimalFormat) {
			if (String.IsNullOrEmpty(optimalFormat))
				return;
			ITimeScaleDateTimeFormatter formatter = TimeScaleDateTimeFormatterFactory.Default.CreateFormatter(scale);
			int count = rulerViewInfo.HeaderCaptions.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = rulerViewInfo.HeaderIntervals[i];
				rulerViewInfo.HeaderCaptions[i] = formatter.Format(optimalFormat, interval.Start, interval.End);
			}
		}
		protected virtual RangeControlRulerViewInfo CreateRulerViewInfo(int index) {
			RangeControlRulerViewInfo result = new RangeControlRulerViewInfo();
			result.RulerIndex = index;
			result.Bounds = RulersBounds[index];
			RulerViewInfos[index] = result;
			return result;
		}
		protected virtual void CalcRulerViewInfo(ISchedulerRangeControlRuler ruler, RangeControlRulerViewInfo rulerViewInfo, Graphics gr) {
			ScaleBasedRangeControlClient client = ScaleBasedClient;
			Rectangle bounds = rulerViewInfo.Bounds;
			DateTime start = client.GetFirstVisibleDate();
			DateTime end = client.GetLastVisibleDate();
			DateTime current = start;
			double normaizedX = ((IRangeControlClient)client).GetNormalizedValue(current);
			int x = Value2Pixel(normaizedX);
			Rectangle prevHeaderBounds = new Rectangle(x, bounds.Y, x, bounds.Height);
			int rulerIndex = rulerViewInfo.RulerIndex;
			 SkinPaddingEdges margins = GetRulerHeaderMargins();
			 SkinBorder border = GetRulerHeaderBorder();
			while (current < end) {
				DateTime intervalStart = current;
				DateTime intervalEnd = client.GetNextVisibleDate(rulerIndex, current);
				rulerViewInfo.HeaderIntervals.Add(new TimeInterval(intervalStart, intervalEnd));
				double normalizedPos = RangeControl.Client.GetNormalizedValue(intervalEnd);
				int xCoor = Value2Pixel(normalizedPos);
				current = intervalEnd;
				string labelText = client.FormatRulerFixedFormatCaption(rulerIndex, intervalStart, intervalEnd);
				Rectangle headerBounds = CalcNextHeaderRect(prevHeaderBounds, xCoor);
				Rectangle headerContentBounds = CalculateRulerHeaderContentBounds(border, headerBounds);
				Rectangle headerTextBounds = CalculateRulerHeaderTextBounds(margins, headerContentBounds);
				if (prevHeaderBounds.IntersectsWith(headerBounds))
					continue;
				rulerViewInfo.HeaderRects.Add(headerBounds);
				rulerViewInfo.ContentRects.Add(headerContentBounds);
				rulerViewInfo.TextRects.Add(headerTextBounds);
				rulerViewInfo.HeaderCaptions.Add(labelText);
				prevHeaderBounds = headerBounds;
			}
		}
		string  CalculateRulerHeaderOptimalFormat(TimeScale scale, RangeControlRulerViewInfo rulerViewInfo, Graphics gr, Font font) {
			DateTime[] dates = ConvertTimeIntervalToDates(rulerViewInfo.HeaderIntervals);
			int minWidth = GetHeaderMinTextWidth(rulerViewInfo.Bounds, rulerViewInfo.ContentRects,  rulerViewInfo.TextRects);
			return ScaleBasedClient.FindOptimalDateTimeFormat(scale, dates, gr, font, minWidth);
		}
		protected internal int GetHeaderMinTextWidth(Rectangle bounds, List<Rectangle> contentRects, List<Rectangle> textRects) {
			int count = textRects.Count;
			if (count < 1)
				return int.MaxValue;
			if (count < 3)
				return Math.Max(textRects[0].Width, textRects[count - 1].Width);
			int minWidth = int.MaxValue;
			for (int i = 1; i < count - 1; i++) {
				if (contentRects[i].X < bounds.X || contentRects[i].Right > bounds.Right)
					continue;
				int width = textRects[i].Width;
				if (minWidth > width)
					minWidth = width;
			}
			return minWidth;
		}
		private DateTime[] ConvertTimeIntervalToDates(List<TimeInterval> intervals) {
			int count = intervals.Count;
			DateTime[] result = new DateTime[count];
			for (int i = 0; i < count; i++) {
				result[i] = intervals[i].Start;
			}
			return result;
		}
		protected virtual Rectangle CalculateRulerHeaderContentBounds(SkinBorder border, Rectangle bounds) {
			SkinPaddingEdges borderThin = border.Thin;
			return new Rectangle(bounds.X + borderThin.Left, bounds.Y + borderThin.Top, bounds.Width - borderThin.Bottom, bounds.Height - borderThin.Right);
		}
		protected virtual Rectangle CalculateRulerHeaderTextBounds(SkinPaddingEdges margins, Rectangle contentBounds) {
			return Rectangle.FromLTRB(contentBounds.X + margins.Left, contentBounds.Y + margins.Top,
				contentBounds.Right - margins.Right, contentBounds.Bottom - margins.Bottom);
		}
		protected virtual int CalculateRulersAreaHeight() {
			SkinBorder skinBorder = GetRulerHeaderBorder();
			int borderHeight = skinBorder.Thin.Top + skinBorder.Thin.Bottom;
			SkinPaddingEdges margins = GetRulerHeaderMargins();
			int marginHeight = margins.Top + margins.Bottom;
			int textHeight = PaintAppearance.CalcDefaultTextSize(GInfo.Graphics).Height;
			return (textHeight + borderHeight + marginHeight) * CalcVisibleRulerCount(Controller.Rulers);
		}
		protected virtual Rectangle CalculateTotalRulersBounds(Rectangle bounds) {
			return new Rectangle(bounds.X, bounds.Y, bounds.Width, RulerAreaHeight);
		}
		protected internal virtual Rectangle CalcNextHeaderRect(Rectangle prevBounds, int xCoor) {
			Rectangle result = prevBounds;
			result.X = prevBounds.Right;
			result.Width = xCoor - result.X;
			return result;
		}
		public bool SelectedRangeContainsInterval(TimeInterval interval) {
			DateTime rangeStart = Controller.GetValue(RangeMinimum);
			DateTime rangeEnd = Controller.GetValue(RangeMaximum);
			TimeInterval activeInterval = new TimeInterval(rangeStart, rangeEnd);
			return activeInterval.Contains(interval);
		}
		protected internal void UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			UpdateHitInfoCore(hitInfo);
		}
		protected internal void UpdateHotInfo(RangeControlHitInfo hitInfo) {
			UpdateHitInfoCore(hitInfo);
		}
		private void UpdateHitInfoCore(RangeControlHitInfo hitInfo) {
			Point pt = hitInfo.HitPoint;
			ScaleBasedRangeControlHitTest hitTest = CalculateHitTest(pt);
			if (hitTest == ScaleBasedRangeControlHitTest.RulerHeader) {
				RangeControlRulerHeaderHitInfo headerHitInfo = CalculateRulerHeaderHitInfo(pt);
				if (headerHitInfo == null)
					return;
				hitInfo.HitTest = RangeControlHitTest.Client;
				hitInfo.ClientHitTest = hitTest;
				hitInfo.ObjectBounds = headerHitInfo.Bounds;
				hitInfo.HitObject = headerHitInfo.Interval;
			}
		}
		protected virtual ScaleBasedRangeControlHitTest CalculateHitTest(Point pt) {
			if (ContentBounds.Contains(pt))
				return ScaleBasedRangeControlHitTest.Content;
			if (RulerAreaBounds.Contains(pt))
				return ScaleBasedRangeControlHitTest.RulerHeader;
			return ScaleBasedRangeControlHitTest.Unknown;
		}
		protected virtual RangeControlRulerHeaderHitInfo CalculateRulerHeaderHitInfo(Point pt) { 
			int count = RulersBounds.Length;
			for (int i = 0; i < count; i++) {
				Rectangle rulerRect = RulersBounds[i];
				if (!rulerRect.Contains(pt))
					continue;
				RangeControlRulerViewInfo ruler = RulerViewInfos[i];
				int headerIndex = FindHotRulerHeaderIndex(ruler, pt);
				if (headerIndex < 0)
					return null;
				RangeControlRulerHeaderHitInfo result = new RangeControlRulerHeaderHitInfo();
				result.Bounds = ruler.HeaderRects[headerIndex];
				TimeInterval interval = ruler.HeaderIntervals[headerIndex];
				result.Interval = ScaleBasedClient.Controller.RoundToWholeInterval(ruler.RulerIndex, interval.Start);
				return result;
			}
			return null;
		 }
		protected int FindHotRulerHeaderIndex(RangeControlRulerViewInfo ruler, Point pt) {
			int count = ruler.HeaderRects.Count;
			for (int i = 0; i < count; i++) {
				Rectangle headerRect = ruler.HeaderRects[i];
				if (headerRect.Contains(pt))
					return i;
			}
			return -1;
		}
		protected internal TimeInterval CalculateHitInterval(RangeControlHitInfo hitInfo) {
			if (hitInfo.HitTest != RangeControlHitTest.Client)
				return null;
			ScaleBasedRangeControlHitTest clientHitTest = (ScaleBasedRangeControlHitTest)hitInfo.ClientHitTest;
			if (clientHitTest == ScaleBasedRangeControlHitTest.RulerHeader) 
				return hitInfo.HitObject as TimeInterval;
			return null;
		}
	}
	public class RangeControlRulerHeaderHitInfo {
		public Rectangle Bounds { get; set; }
		public TimeInterval Interval { get; set; }
		public ScaleBasedRangeControlHitTest HitTest { get { return ScaleBasedRangeControlHitTest.RulerHeader;  } }
	}
	public class RangeControlRulerViewInfo {
		public RangeControlRulerViewInfo() { 
			TextRects = new List<Rectangle>();
			HeaderCaptions = new List<string>();
			HeaderRects = new List<Rectangle>();
			ContentRects = new List<Rectangle>();
			HeaderIntervals = new List<TimeInterval>();
		}
		public Rectangle Bounds { get; set; }
		public int RulerIndex { get; set; }
		public List<string> HeaderCaptions { get; private set; }
		public List<Rectangle> TextRects { get; private set; }
		public List<Rectangle> HeaderRects { get; private set; }
		public List<Rectangle> ContentRects { get; private set; }
		public List<TimeInterval> HeaderIntervals { get; private set; }
		protected internal Rectangle GetTextOutRect(int index) {
			Rectangle rect = TextRects[index];
			int left = Math.Max(Bounds.X, rect.X);
			int right = Math.Min(Bounds.Right, rect.Right);
			return Rectangle.FromLTRB(left, rect.Top, right, rect.Bottom);
		}
	}
}
