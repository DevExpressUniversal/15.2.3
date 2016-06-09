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
using DevExpress.XtraEditors.ViewInfo;
using System.Drawing;
using DevExpress.Skins;
using System.Windows.Forms;
using System.Threading;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.ComponentModel;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraEditors {
	public class BaseRangeControlViewInfo : BaseStyleControlViewInfo {
		public BaseRangeControlViewInfo(RangeControl rangeControl) : base(rangeControl) { }
		public RangeControl RangeControl { get { return (RangeControl)base.OwnerControl; } }
		public double RangeMinimum { get { return RangeControl.OverriddenRange.Minimum >= 0.0 ? RangeControl.OverriddenRange.Minimum : RangeControl.NormalizedSelectedRange.Minimum; } }
		public double RangeMaximum { get { return RangeControl.OverriddenRange.Maximum >= 0.0 ? RangeControl.OverriddenRange.Maximum : RangeControl.NormalizedSelectedRange.Maximum; } }
		protected internal bool IsHorizontal { get { return RangeControl.Orientation == Orientation.Horizontal; } }
		protected internal bool IsVertical { get { return !IsHorizontal; } }
		public virtual int MinIndentBetweenTicks { get { return Calculator.MinIndentBetweenTicks; } }
		public virtual Size CalcTextSize(Graphics g, string text) {
			return Calculator.CalcTextSize(g, text, PaintAppearance);
		}
		protected internal Color BackColor { get { return Calculator.BackColor; } }
		protected internal Color BorderColor { get { return Calculator.BorderColor; } }
		protected internal Color RulerColor { get { return Calculator.RulerColor; } }
		protected internal Color LabelColor { get { return Calculator.LabelColor; } }
		protected internal Color OutOfRangeMaskColor {
			get {
				Color res = Calculator.GetColorProperty(EditorsSkins.OptRangeControlOutOfRangeColorMask);
				if(res.A == 0)
					return Color.FromArgb(0xc0, 0xff, 0xff, 0xff);
				return res;
			}
		}
		public SkinElementInfo GetBorderInfo() {
			return Calculator.GetBorderInfo();
		}
		public Color GetColorProperty(string propertyName) {
			return Calculator.GetColorProperty(propertyName);
		}
		protected internal int GetIntProperty(string propertyName) {
			return Calculator.GetIntProperty(propertyName);
		}
		protected internal Color ScrollAreaColor { get { return Calculator.GetColorProperty(EditorsSkins.OptRangeControlScrollAreaColor); } }
		protected internal Color ViewPortPreviewColor { get { return Calculator.GetColorProperty(EditorsSkins.OptRangeControlViewPortPreviewColor); } }
		protected internal Color RangePreviewColor { get { return Calculator.GetColorProperty(EditorsSkins.OptRangeControlRangePreviewColor); } }
		public virtual int RuleTextTopIndent { get { return Calculator.RuleTextTopIndent; } }
		public virtual int RuleTextBottomIndent { get { return Calculator.RuleTextBottomIndent; } }
		protected Size GetElementSizeCore(SkinElement elem) {
			if(elem != null)
				return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(elem)).Size;
			return new Size(10, 32);
		}
		public virtual int RangeBoxTopIndent {
			get {
				if(RangeControl.Client != null)
					return RangeControl.Client.RangeBoxTopIndent;
				return 0;
			}
		}
		public virtual int RangeBoxBottomIndent {
			get {
				if(RangeControl.Client != null)
					return RangeControl.Client.RangeBoxBottomIndent;
				return 0;
			}
		}
		protected virtual Rectangle CalcRectInViewPort(Rectangle scrollBounds, double start, double end) {
			if(double.IsNaN(start) || double.IsNaN(end)) return scrollBounds;
			int rangeRight = (int)((end - RangeControl.VisibleRangeStartPosition) * RangeControl.VisibleRangeScaleFactor * scrollBounds.Width + 0.5);
			int rangeLeft = (int)((start - RangeControl.VisibleRangeStartPosition) * RangeControl.VisibleRangeScaleFactor * scrollBounds.Width + 0.5);
			int rangeWidth = rangeRight - rangeLeft;
			return new Rectangle(scrollBounds.X + rangeLeft, scrollBounds.Y + RangeBoxTopIndent, rangeWidth, scrollBounds.Height - RangeBoxTopIndent - RangeBoxBottomIndent);
		}
		public Rectangle ScrollBoundsCore { get { return RangeControl.RangeViewInfo.ScrollBounds; } }
		public Rectangle ScrollBarAreaBoundsCore { get { return RangeControl.RangeViewInfo.ScrollBarAreaBounds; } }
		public virtual int GetDeltaX(Point pt) {
			if(RangeControl.Orientation == Orientation.Horizontal) {
				if(RangeControl.IsRightToLeft) return ScrollBoundsCore.Right - pt.X;
				else return pt.X - ScrollBoundsCore.X;
			}
			return pt.Y - ScrollBoundsCore.Y;
		}
		public virtual int GetPreviewDeltaX(Point pt) {
			if(RangeControl.Orientation == Orientation.Horizontal) {
				if(RangeControl.IsRightToLeft) return ScrollBarAreaBoundsCore.Right - pt.X;
				else return pt.X - ScrollBarAreaBoundsCore.X;
			}
			return pt.Y - ScrollBarAreaBoundsCore.Y;
		}
		public virtual double Point2Value(Point pt) {
			int deltaX = GetDeltaX(pt);
			double deltaValue = (double)deltaX / RangeControl.RangeViewInfo.GetScrollWidth() * RangeControl.VisibleRangeWidth;
			return RangeControl.VisibleRangeStartPosition + deltaValue;
		}
		public virtual double PreviewPoint2Value(Point pt) {
			int deltaX = GetPreviewDeltaX(pt);
			double deltaValue = (double)deltaX / RangeControl.RangeViewInfo.GetPreviewScrollWidth();
			return deltaValue;
		}
		public virtual int Delta2Pixel(double value) {
			return Calculator.Delta2Pixel(value);
		}
		public virtual int Value2Pixel(double value) {
			return Calculator.Value2Pixel(value);
		}
		public virtual double Pixel2Delta(int pixel) {
			return Calculator.Pixel2Delta(pixel);
		}
		RangeControlViewInfoCalc calculator;
		protected internal RangeControlViewInfoCalc Calculator {
			get {
				if(calculator == null)
					calculator = CreateCalculator();
				return calculator;
			}
		}
		RangeControlViewInfoCalc CreateCalculator() {
			return new RangeControlViewInfoCalc(Bounds, ClientRect, RangeControl.Orientation, RangeControl.VisibleRangeScaleFactor, false, LookAndFeel, RangeControl.Appearance);
		}
	}
	public class RangeControlViewInfo : BaseRangeControlViewInfo {
		static int RangeAnimationLength = 500;
		public RangeControlViewInfo(RangeControl rangeControl)
			: base(rangeControl) {
			SelectionEnd = SelectionStart = 0.0;
		}
		protected internal bool IsRightToLeft { get { return RangeControl.IsRightToLeft; } }
		protected Rectangle GetThumbThouchBounds(Rectangle thumb) {
			if(IsHorizontal) {
				thumb.Y = Calculator.RangeBounds.Y;
				thumb.Height = Calculator.RangeBounds.Height;
				thumb.Inflate(10, 0);
			}
			else {
				thumb.X = Calculator.RangeBounds.X;
				thumb.Width = Calculator.RangeBounds.Width;
				thumb.Inflate(0, 10);
			}
			return thumb;
		}
		public virtual Rectangle MinThumbTouchBounds {
			get {
				return GetThumbThouchBounds(Calculator.MinRangeThumbBounds);
			}
		}
		public virtual Rectangle MaxThumbTouchBounds {
			get {
				return GetThumbThouchBounds(Calculator.MaxRangeThumbBounds);
			}
		}
		public virtual double SelectionStart { get; internal set; }
		public virtual double SelectionEnd { get; internal set; }
		public bool HasSelection { get { return SelectionEnd != 0.0 || SelectionStart != 0.0; } }
		protected void SafeCalcViewInfo() {
			bool shouldReleaseGraphics = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
			}
			CalcViewInfo(GInfo.Graphics);
			if(shouldReleaseGraphics)
				GInfo.ReleaseGraphics();
		}
		protected internal virtual int GetPreviewScrollWidth() {
			if(!IsReady) {
				SafeCalcViewInfo();
			}
			return GetRectWidth(Bounds);
		}
		protected internal virtual int GetScrollWidth() {
			if(!IsReady) {
				SafeCalcViewInfo();
			}
			return GetRectWidth(ScrollBoundsCore);
		}
		protected internal virtual int GetRectWidth(Rectangle rect) {
			if(RangeControl.Orientation == Orientation.Horizontal)
				return rect.Width;
			return rect.Height;
		}
		RangeControlHitInfo hotInfo;
		RangeControlHitInfo pressedInfo;
		public RangeControlHitInfo HotInfo {
			get {
				if(hotInfo == null)
					hotInfo = new RangeControlHitInfo();
				return hotInfo;
			}
			set {
				RangeControlHitInfo prevInfo = HotInfo;
				if(prevInfo.Equals(value))
					return;
				hotInfo = value;
				OnHotInfoChanged(prevInfo);
			}
		}
		protected virtual void OnHotInfoChanged(RangeControlHitInfo prevInfo) {
			if(prevInfo != null)
				RangeControl.Invalidate(prevInfo.ObjectBounds);
			if(HotInfo != null)
				RangeControl.Invalidate(HotInfo.ObjectBounds);
		}
		public RangeControlHitInfo PressedInfo {
			get {
				if(pressedInfo == null)
					pressedInfo = new RangeControlHitInfo();
				return pressedInfo;
			}
			set {
				RangeControlHitInfo prevInfo = PressedInfo;
				if(prevInfo.Equals(value))
					return;
				pressedInfo = value;
				OnPressedInfoChanged(prevInfo);
			}
		}
		protected virtual void OnPressedInfoChanged(RangeControlHitInfo prevInfo) {
			if(prevInfo != null)
				RangeControl.Invalidate(prevInfo.ObjectBounds);
			if(PressedInfo != null)
				RangeControl.Invalidate(PressedInfo.ObjectBounds);
		}
		public Rectangle RangeClientBounds {
			get {
				Rectangle rect = ScrollBounds;
				if(RangeControl.Orientation == Orientation.Horizontal) {
					rect.X = Math.Min(ContentRect.X, rect.X);
					rect.Width = Math.Max(rect.Width, ContentRect.Width);
				} else {
					rect.Y = Math.Min(rect.Y, ContentRect.Y);
					rect.Height = Math.Max(rect.Height, ContentRect.Height);
				}
				return rect;
			}
		}
		protected Rectangle ScreenBounds2ImageBounds(Rectangle rect) {
			if(IsVertical) 
				return new Rectangle(rect.X - ScrollBounds.X, rect.Y, rect.Width, rect.Height);
			return new Rectangle(rect.X, rect.Y - ScrollBounds.Y, rect.Width, rect.Height);
		}
		protected internal virtual Rectangle RangeBoxImageBounds { 
			get {
				return ScreenBounds2ImageBounds(RangeBounds);
			} 
		}
		protected internal virtual Rectangle LeftOutOfRangeImageBounds {
			get {
				return ScreenBounds2ImageBounds(LeftOutOfRangeBounds);
			}
		}
		protected internal virtual Rectangle RightOutOfRangeImageBounds {
			get {
				return ScreenBounds2ImageBounds(RightOutOfRangeBounds);
			}
		}
		protected internal virtual bool IsContentImageReady { get; set; }
		protected internal Image ContentImage { get; set; }
		protected internal Image RotatedImage { get; set; }
		protected internal void CreateRotatedImage() {
			if(ContentImage == null) return;
			RotatedImage = (Image)ContentImage.Clone();
			RotatedImage.RotateFlip(GetRotateType());
		}
		RotateFlipType GetRotateType() {
			if(IsHorizontal) {
				if(IsRightToLeft) return RotateFlipType.RotateNoneFlipX;
			}
			else return IsRightToLeft ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate90FlipNone;
			return RotateFlipType.RotateNoneFlipNone;
		}
		protected virtual void UpdateContentImage() {
			Size size = new Size(Bounds.Width, ScrollBounds.Height);
			if(IsVertical) {
				size = new Size(Bounds.Height, ScrollBounds.Width);
			}
			if(ContentImage != null && (ContentImage.Width != size.Width || ContentImage.Height != size.Height)) {
				ContentGraphicsCache.Dispose();
				contentGraphicsCache = null;
				ContentImage.Dispose();
				ContentImage = null;
				if(RotatedImage != null) {
					RotatedImage.Dispose();
					RotatedImage = null;
				}
			}
			if(ContentImage == null && size.Width > 0 && size.Height > 0) {
				ContentImage = new Bitmap(size.Width, size.Height);
				IsContentImageReady = false;
			}
		}
		GraphicsCache contentGraphicsCache;
		protected internal virtual GraphicsCache ContentGraphicsCache {
			get {
				if(contentGraphicsCache == null) {
					contentGraphicsCache = new GraphicsCache(Graphics.FromImage(ContentImage));
				}
				return contentGraphicsCache;
			}
		}
		public Rectangle ScrollBounds { get { return Calculator.ScrollBounds; } private set { Calculator.ScrollBounds = value; } }
		public Rectangle LeftOutOfRangeBounds { get { return Calculator.LeftOutOfRangeBounds; } private set { Calculator.LeftOutOfRangeBounds = value; } }
		public Rectangle RightOutOfRangeBounds { get { return Calculator.RightOutOfRangeBounds; } private set { Calculator.RightOutOfRangeBounds = value; } }
		public Rectangle RangeBounds { get { return Calculator.RangeBounds; } private set { Calculator.RangeBounds = value; } }
		public Rectangle MinRangeThumbBounds { get { return Calculator.MinRangeThumbBounds; } private set { Calculator.MinRangeThumbBounds = value; } }
		public Rectangle MaxRangeThumbBounds { get { return Calculator.MaxRangeThumbBounds; } private set { Calculator.MaxRangeThumbBounds = value; } }
		public Rectangle MinRangeFlagBounds { get { return Calculator.MinRangeFlagBounds; } private set { Calculator.MinRangeFlagBounds = value; } }
		public Rectangle MaxRangeFlagBounds { get { return Calculator.MaxRangeFlagBounds; } private set { Calculator.MaxRangeFlagBounds = value; } }
		public Rectangle RulerBounds { get { return Calculator.RulerBounds; } set { Calculator.RulerBounds = value; } }
		public Rectangle ScrollBarAreaBounds { get { return Calculator.ScrollBarAreaBounds; } private set { Calculator.ScrollBarAreaBounds = value; } }
		public Rectangle ScrollBarThumbBounds { get { return Calculator.ScrollBarThumbBounds; } set { Calculator.ScrollBarThumbBounds = value; } }
		public Rectangle RangeIndicatorBounds { get { return Calculator.RangeIndicatorBounds; } set { Calculator.RangeIndicatorBounds = value; } }
		public Rectangle SelectionBounds { get { return Calculator.SelectionBounds; } private set { Calculator.SelectionBounds = value; } }
		public Rectangle LeftScaleThumbBounds { get { return Calculator.LeftScaleThumbBounds; } private set { Calculator.LeftScaleThumbBounds = value; } }
		public Rectangle RightScaleThumbBounds { get { return Calculator.RightScaleThumbBounds; } private set { Calculator.RightScaleThumbBounds = value; } }
		public Rectangle MinFlagLineBounds { get { return Calculator.MinRangeFlagLineBounds; } private set { Calculator.MinRangeFlagLineBounds = value; } }
		public Rectangle MaxFlagLineBounds { get { return Calculator.MaxRangeFlagLineBounds; } private set { Calculator.MaxRangeFlagLineBounds = value; } }
		public Rectangle MinFlagTextBounds { get { return Calculator.MinFlagTextBounds; } private set { Calculator.MinFlagTextBounds = value; } }
		public Rectangle MaxFlagTextBounds { get { return Calculator.MaxFlagTextBounds; } private set { Calculator.MaxFlagTextBounds = value; } }
		protected virtual Size GetSizingGlyphSize() {
			SkinElement elem = GetSizingGlyphElement();
			if(elem == null)
				return Size.Empty;
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(elem)).Size;
		}
		protected internal virtual SkinElement GetSizingGlyphElement() {
			return EditorsSkins.GetSkin(RangeControl.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeControlSizingGlyph];
		}
		protected internal virtual SkinElementInfo GetLeftSizingGlyphInfo() { 
			SkinElement elem = GetSizingGlyphElement();
			if(elem == null)
				return null;
			Size size = GetSizingGlyphSize();
			return new SkinElementInfo(GetSizingGlyphElement(), new Rectangle(ScrollBarThumbBounds.X + 1, ScrollBarThumbBounds.Y + (ScrollBarThumbBounds.Height - size.Height) / 2, size.Width, size.Height));
		}
		protected internal virtual SkinElementInfo GetRightSizingGlyphInfo() {
			SkinElementInfo info = GetLeftSizingGlyphInfo();
			if(info != null)
				info.Bounds = new Rectangle(ScrollBarThumbBounds.Right - info.Bounds.Width, info.Bounds.Y, info.Bounds.Width, info.Bounds.Height);
			return info;
		}
		protected internal virtual SkinElementInfo GetMinRangeThumbInfo() {
			SkinElement elem = GetMinRangeThumbElement();
			if(elem == null)
				return null;
			return new SkinElementInfo(elem, MinRangeThumbBounds);
		}
		protected internal virtual SkinElementInfo GetMaxRangeThumbInfo() {
			SkinElement elem = GetMaxRangeThumbElement();
			if(elem == null)
				return null;
			return new SkinElementInfo(elem, MaxRangeThumbBounds);
		}
		protected internal void CalcRectsCore() { CalcRects(); }
		protected override void CalcRects() {
			base.CalcRects();
			bool releaseGraphics = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				releaseGraphics = true;
			}
			try {
				if(RangeControl.Orientation == Orientation.Horizontal)
					CalcHorizontalRects();
				else
					CalcVerticalRects();
				if(RangeControl.IsRightToLeft)
					MirrorRectsHorizontal();
				UpdateContentImage();
			} finally {
				if(releaseGraphics)
					GInfo.ReleaseGraphics();
			}
			if (RangeControl.Client != null) {
				RangeControl.Client.Calculate(Calculator.ClientRect);
			}
			RangeControl.UpdateRuler();
		}
		void MirrorRectsHorizontal() {
			Calculator.MirrorRectsHorizontal(); 
		}
		protected virtual void CalcVerticalRects() {
			CalcHorizontalRects(new Rectangle(Bounds.X, Bounds.Y, Bounds.Height, Bounds.Width), new Rectangle(ContentRect.X, ContentRect.Y, ContentRect.Height, ContentRect.Width));
			ReInitializeCalculator(Bounds);
			RotateRects();
		}
		protected virtual void RotateRects() {
			Calculator.RotateRects();
		}
		protected void ReInitializeCalculator(Rectangle bounds) {
			bounds = CorrectBounds(bounds);
			Calculator.Bounds = bounds;
			Calculator.ClientRect = bounds;
		}
		protected internal virtual Rectangle Vertical2Horizontal(Rectangle rect) {
			return new Rectangle(rect.Y, Bounds.Right - rect.Right, rect.Height, rect.Width);
		}
		protected virtual Rectangle Horizontal2Vertical(Rectangle rect) {
			return Calculator.Horizontal2VerticalCore(rect);
		}
		protected internal virtual Rectangle Left2Right(Rectangle rect) {
			return Calculator.HorizontalRotateCore(rect);
		}
		protected internal virtual Rectangle Right2Left(Rectangle rect) {
			return new Rectangle(Bounds.Right - rect.Right, rect.Y, rect.Width, rect.Height);
		}
		protected virtual void CalcHorizontalRects() {
			CalcHorizontalRects(Bounds, ContentRect);
		}
		protected internal virtual List<object> Ruler { get { return RangeControl.Ruler; } }
		protected int MinRangeThumbWidth { get { return GetMinRangeThumbSize().Width; } }
		protected int MinRangeThumbOffset {
			get {
				SkinElement elem = GetMinRangeThumbElement();
				if(elem != null)
					return elem.Offset.Offset.X;
				return -10;
			}
		}
		protected int MaxRangeThumbWidth { get { return GetMaxRangeThumbSize().Width; } }
		protected int MaxRangeThumbOffset {
			get {
				SkinElement elem = GetMaxRangeThumbElement();
				if(elem != null)
					return elem.Offset.Offset.X;
				return 0;
			}
		}
		protected virtual Rectangle CorrectBounds(Rectangle bounds) {
			if(RangeControl.BorderStyle != DevExpress.XtraEditors.Controls.BorderStyles.NoBorder)
				bounds.Width -= 1;
			else {
				bounds.X -= 1;
				bounds.Width += 1;
			}
			return bounds;
		}
		protected virtual void CalcHorizontalRects(Rectangle bounds, Rectangle clientBounds) {
			bounds = CorrectBounds(bounds);
			InitializeCalculator(bounds, bounds);
			ScrollBarAreaBounds = CalcScrollBarAreaBounds(bounds);
			ScrollBarThumbBounds = CalcScrollBarThumbBounds(ScrollBarAreaBounds);
			LeftScaleThumbBounds = CalcLeftScaleThumbBounds(ScrollBarThumbBounds);
			RightScaleThumbBounds = CalcRightScaleThumbBounds(ScrollBarThumbBounds);
			RangeIndicatorBounds = CalcRangeIndicatorBounds(ScrollBarAreaBounds);
			RulerBounds = CalcRulerBounds(clientBounds);
			ScrollBounds = CalcScrollBounds(bounds);
			RangeBounds = CalcRangeBounds(ScrollBounds);
			LeftOutOfRangeBounds = CalcLeftOutOfRangeAreaBounds(bounds, RangeBounds);
			RightOutOfRangeBounds = CalcRightOutOfRangeAreaBounds(bounds, RangeBounds);
			if(RangeControl.Client != null) {
				MinRangeFlagBounds = CalcMinRangeFlagBounds(RangeBounds);
				MaxRangeFlagBounds = CalcMaxRangeFlagBounds(RangeBounds);
				MinRangeThumbBounds = CalcMinRangeThumbBounds(RangeBounds);
				MaxRangeThumbBounds = CalcMaxRangeThumbBounds(RangeBounds);
			}
			SelectionBounds = CalcSelectionBounds(ScrollBounds);
			MinFlagLineBounds = CalcMinFlagLineBounds();
			MaxFlagLineBounds = CalcMaxFlagLineBounds();
		}
		private void InitializeCalculator(Rectangle bounds, Rectangle clientRect) {
			Calculator.Reset();
			Calculator.Orientation = RangeControl.Orientation;
			Calculator.Bounds = bounds;
			Calculator.ClientRect = clientRect;
			Calculator.Graphics = GInfo.Graphics;
			Calculator.IsRightToLeft = RangeControl.IsRightToLeft;
			Calculator.VisibleRangeStartPosition = RangeControl.VisibleRangeStartPosition;
			Calculator.VisibleRangeWidth = RangeControl.VisibleRangeWidth;
			Calculator.VisibleRangeScaleFactor = RangeControl.VisibleRangeScaleFactor;
		}
		protected internal virtual int GetRulerIndexBeforeValue(double value) {
			return Calculator.GetRulerIndexBeforeValue(value, RangeControl.Client, Ruler);
		}
		protected bool IsCustomRuler() {
			IRangeControlClient client = RangeControl.Client;
			return client != null && client.IsCustomRuler;
		}
		protected virtual Rectangle CalcRulerBounds(Rectangle bounds) {
			if(!RangeControl.ShowLabels) 
				return new Rectangle(bounds.X,bounds.Bottom - ScrollBarAreaBounds.Height, bounds.Width, 0);
			return Calculator.GetRulerBounds(bounds, PaintAppearance, IsCustomRuler(), RangeControl.ShowZoomScrollBar);
		}
		protected virtual Rectangle CalcRightScaleThumbBounds(Rectangle thumb) {
			if(thumb == Rectangle.Empty) return Rectangle.Empty;
			Rectangle rect = thumb;
			rect.Width = GetSizingGlyphSize().Width;
			rect.X = (thumb.Right - rect.Width) < 0 ? thumb.Right : thumb.Right - rect.Width;
			return rect;
		}
		protected virtual Rectangle CalcLeftScaleThumbBounds(Rectangle thumb) {
			if(thumb == Rectangle.Empty) return Rectangle.Empty;
			Rectangle rect = thumb;
			rect.Width = GetSizingGlyphSize().Width;
			rect.X = (thumb.X + rect.Width) > ScrollBarAreaBounds.Right ? thumb.X-rect.Width : thumb.X;
			return rect;
		}
		protected virtual Rectangle CalcRectInScrollBarArea(Rectangle scrollAreaBounds, double position, double range) {
			Rectangle area = scrollAreaBounds;
			Rectangle res = area;
			if(double.IsNaN(range) || double.IsNaN(position)) return res; 
			res.Width = (int)(area.Width * range + 0.5);
			res.X = res.X + (int)(position * area.Width + 0.5);
			return res;
		}
		protected virtual Rectangle CalcRangeIndicatorBounds(Rectangle bounds) {
			if(!RangeControl.AllowSelection || !RangeControl.ShowZoomScrollBar) return Rectangle.Empty;
			return CalcRectInScrollBarArea(bounds, RangeMinimum, RangeMaximum - RangeMinimum);
		}
		protected virtual Rectangle CalcScrollBarThumbBounds(Rectangle bounds) {
			if(!RangeControl.ShowZoomScrollBar) return Rectangle.Empty;
			return CalcRectInScrollBarArea(bounds, RangeControl.VisibleRangeStartPosition, RangeControl.VisibleRangeWidth);
		}
		protected virtual SkinElement GetScrollBarElement() {
			return CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinScrollButtonThumb];
		}
		protected int ScrollBarHeight { get { return Calculator.ScrollBarHeight; } }
		protected virtual Rectangle CalcScrollBarAreaBounds(Rectangle bounds) {
			if(!RangeControl.ShowZoomScrollBar) return Rectangle.Empty;
			return Calculator.CalcScrollBarAreaBounds(bounds);
		}
		protected virtual Rectangle CalcRightOutOfRangeAreaBounds(Rectangle bounds, Rectangle rangeBounds) {
			return Calculator.CalcRightOutOfRangeAreaBounds(bounds, rangeBounds);
		}
		protected virtual Rectangle CalcLeftOutOfRangeAreaBounds(Rectangle bounds, Rectangle rangeBounds) {
			return Calculator.CalcLeftOutOfRangeAreaBounds(bounds, rangeBounds);
		}
		private Rectangle CalcMaxRangeThumbBounds(Rectangle rangeBounds) {
			if(RangeControl.SelectionType == RangeControlSelectionType.Flag) {
				return Rectangle.Empty;
			}
			Size thumbSize = GetMaxRangeThumbSize();
			int thumbY = rangeBounds.Y + (rangeBounds.Height - thumbSize.Height) / 2;
			if(IsRangePositionThumbVerticallyStretched) {
				thumbSize.Height = rangeBounds.Height;
			}
			return new Rectangle(new Point(rangeBounds.Right + MaxRangeThumbOffset, thumbY), thumbSize);
		}
		protected virtual Rectangle CalcMinRangeThumbBounds(Rectangle rangeBounds) {
			if(RangeControl.SelectionType == RangeControlSelectionType.Flag) {
				return Rectangle.Empty;
			}
			Size thumbSize = GetMinRangeThumbSize();
			int thumbY = rangeBounds.Y + (rangeBounds.Height - thumbSize.Height) / 2;
			if(IsRangePositionThumbVerticallyStretched) {
				thumbSize.Height = rangeBounds.Height;
			}
			return new Rectangle(new Point(rangeBounds.X + MinRangeThumbOffset, thumbY), thumbSize);
		}
		private Rectangle CalcMinRangeFlagBounds(Rectangle rangeBounds) {
			if(RangeControl.SelectionType == RangeControlSelectionType.Thumb) return Rectangle.Empty;
			string text = RangeControl.Client.ValueToString(this.RangeMinimum);
			if(RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchUI())
				return Calculator.CalcMinRangeFlagBounds(GInfo.Graphics, text, PaintAppearance, RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchScaleFactor());
			return Calculator.CalcMinRangeFlagBounds(GInfo.Graphics, text, PaintAppearance);
		}
		private Rectangle CalcMaxRangeFlagBounds(Rectangle rangeBounds) {
			if(RangeControl.SelectionType == RangeControlSelectionType.Thumb) return Rectangle.Empty;
			string text = RangeControl.Client.ValueToString(this.RangeMaximum);
			if(RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchUI())
				return Calculator.CalcMaxRangeFlagBounds(GInfo.Graphics, text, PaintAppearance, RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchScaleFactor());
			return Calculator.CalcMaxRangeFlagBounds(GInfo.Graphics, text, PaintAppearance);
		}
		private Rectangle CalcMinFlagLineBounds() {
			if(RangeControl.SelectionType != RangeControlSelectionType.Flag) return Rectangle.Empty;
			if(RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchUI()) 
				return new Rectangle(LeftOutOfRangeBounds.Right-5, RangeBounds.Y, 10, RangeBounds.Height);
			return new Rectangle(LeftOutOfRangeBounds.Right-1, RangeBounds.Y, 2, RangeBounds.Height);
		}
		private Rectangle CalcMaxFlagLineBounds() {
			if(RangeControl.SelectionType != RangeControlSelectionType.Flag) return Rectangle.Empty;
			if(RangeControl.LookAndFeel.ActiveLookAndFeel.GetTouchUI())
				return new Rectangle(RightOutOfRangeBounds.X - 5, RangeBounds.Y, 10, RangeBounds.Height);
			return new Rectangle(RightOutOfRangeBounds.X - 1, RangeBounds.Y, 2, RangeBounds.Height);
		}
		protected virtual bool IsRangePositionThumbVerticallyStretched {
			get {
				SkinElement elem = GetMinRangeThumbElement();
				if(elem == null)
					return false;
				return elem.Size.AllowVGrow;
			}
		}
		protected virtual SkinElement GetMaxRangeThumbElement() {
			return EditorsSkins.GetSkin(RangeControl.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeControlRightThumb];
		}
		protected virtual SkinElement GetMinRangeThumbElement() {
			return EditorsSkins.GetSkin(RangeControl.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeControlLeftThumb];
		}
		protected virtual Size GetMinRangeThumbSize() {
			return GetElementSizeCore(GetMinRangeThumbElement());
		}
		protected virtual Size GetMaxRangeThumbSize() {
			return GetElementSizeCore(GetMaxRangeThumbElement());
		}
		protected virtual Rectangle CalcSelectionBounds(Rectangle scrollBounds) {
			if(!HasSelection)
				return Rectangle.Empty;
			double min = Math.Min(SelectionStart, SelectionEnd);
			double max = Math.Max(SelectionStart, SelectionEnd);
			return CalcRectInViewPort(scrollBounds, min, max);
		}
		protected virtual Rectangle CalcRangeBounds(Rectangle scrollBounds) {
			RangeAnimationInfo info = XtraAnimator.Current.Get(RangeControl, RangeControl.AnimationId) as RangeAnimationInfo;
			if(info == null)
				return CalcRectInViewPort(scrollBounds, RangeMinimum, RangeMaximum);
			return CalcRectInViewPort(scrollBounds, info.CurrentMin, info.CurrentMax);
		}
		protected virtual Rectangle CalcScrollBounds(Rectangle bounds) {
			return Calculator.CalcScrollBounds(bounds);
		}
		public virtual void UpdateHotInfo(Point point) {
			RangeControlHitInfo hitInfo = CalcHitInfo(point);
			if(RangeControl.Client != null)
				RangeControl.Client.UpdateHotInfo(hitInfo);
			HotInfo = hitInfo;
		}
		public virtual RangeControlHitInfo CalcHitInfo(Point point) {
			RangeControlHitInfo info = new RangeControlHitInfo(point, RangeControl.AllowSelection);
			if(info.ContainsSet(RightScaleThumbBounds, RangeControlHitTest.RightScaleThumb))
				return info;
			if(info.ContainsSet(LeftScaleThumbBounds, RangeControlHitTest.LeftScaleThumb))
				return info;
			if(info.ContainsSet(RangeIndicatorBounds, RangeControlHitTest.RangeIndicator))
				return info;
			if(info.ContainsSet(ScrollBarThumbBounds, RangeControlHitTest.ScrollBarThumb))
				return info;
			if(info.ContainsSet(ScrollBarAreaBounds, RangeControlHitTest.ScrollBarArea))
				return info;
			if(info.ContainsSet(MaxRangeThumbBounds, RangeControlHitTest.MaxRangeThumb))
				return info;
			if(info.ContainsSet(MinRangeThumbBounds, RangeControlHitTest.MinRangeThumb))
				return info;
			if(info.ContainsSet(MaxFlagLineBounds, RangeControlHitTest.MaxRangeThumb))
				return info;
			if(info.ContainsSet(MinFlagLineBounds, RangeControlHitTest.MinRangeThumb))
				return info;
			if(info.ContainsSet(RangeBounds, RangeControlHitTest.RangeBox))
				return info;
			if(info.ContainsSet(ScrollBounds, RangeControlHitTest.ViewPort))
				return info;
			return info;
		}
		public virtual void UpdatePressedInfo(Point point) {
			RangeControlHitInfo hitInfo = CalcHitInfo(point);
			if(RangeControl.Client != null)
				RangeControl.Client.UpdatePressedInfo(hitInfo);
			PressedInfo = hitInfo;
		}
		protected internal virtual SkinElementInfo GetSelectionInfo() {
			return new SkinElementInfo(CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinSelection], SelectionBounds);
		}
		protected internal void ResetSelection() {
			SelectionStart = SelectionEnd = 0.0;
		}
		protected internal virtual void UpdateSelection(double start, double end) {
			NormalizedRangeInfo info = RangeControl.GetValidatedInfo(start, end, RangeControlValidationType.Selection);
			if(start > end) {
				SelectionStart = info.Range.Maximum;
				SelectionEnd = info.Range.Minimum;
			} else {
				SelectionStart = info.Range.Minimum;
				SelectionEnd = info.Range.Maximum;
			}
			if(IsHorizontal) {
				SelectionBounds = CalcSelectionBounds(ScrollBounds);
				if(RangeControl.IsRightToLeft) SelectionBounds = Left2Right(SelectionBounds);
			}
			else
				SelectionBounds = Horizontal2Vertical(CalcSelectionBounds(Vertical2Horizontal(ScrollBounds)));
			RangeControl.Invalidate();
		}
		protected internal virtual void UpdateAnimatedRangeBounds() {
			if(RangeControl.IsRightToLeft && IsHorizontal){
					RangeBounds = Left2Right(CalcRangeBounds((ScrollBounds)));
					MinRangeThumbBounds = Left2Right(CalcMinRangeThumbBounds(Right2Left(RangeBounds)));
					MaxRangeThumbBounds = Left2Right(CalcMaxRangeThumbBounds(Right2Left(RangeBounds)));
					LeftOutOfRangeBounds = Left2Right(CalcLeftOutOfRangeAreaBounds(Right2Left(ScrollBounds), Right2Left(RangeBounds)));
					RightOutOfRangeBounds = Left2Right(CalcRightOutOfRangeAreaBounds(Right2Left(ScrollBounds), Right2Left(RangeBounds)));
			}
			else if(IsHorizontal) {
				RangeBounds = CalcRangeBounds(ScrollBounds);
				MinRangeThumbBounds = CalcMinRangeThumbBounds(RangeBounds);
				MaxRangeThumbBounds = CalcMaxRangeThumbBounds(RangeBounds);
				LeftOutOfRangeBounds = CalcLeftOutOfRangeAreaBounds(Bounds, RangeBounds);
				RightOutOfRangeBounds = CalcRightOutOfRangeAreaBounds(Bounds, RangeBounds);
			} else {
				RangeBounds = Horizontal2Vertical(CalcRangeBounds(Vertical2Horizontal(ScrollBounds)));
				MinRangeThumbBounds = Horizontal2Vertical(CalcMinRangeThumbBounds(Vertical2Horizontal(RangeBounds)));
				MaxRangeThumbBounds = Horizontal2Vertical(CalcMaxRangeThumbBounds(Vertical2Horizontal(RangeBounds)));
				LeftOutOfRangeBounds = Horizontal2Vertical(CalcLeftOutOfRangeAreaBounds(Vertical2Horizontal(ScrollBounds), Vertical2Horizontal(RangeBounds)));
				RightOutOfRangeBounds = Horizontal2Vertical(CalcRightOutOfRangeAreaBounds(Vertical2Horizontal(ScrollBounds), Vertical2Horizontal(RangeBounds)));
			}
		}
		protected internal virtual void AddRangeAnimation(double newMin, double newMax) {
			double min = Math.Min(newMin, newMax);
			double max = Math.Max(newMin, newMax);
			if(XtraAnimator.Current.Get(RangeControl, RangeControl.AnimationId) != null)
				XtraAnimator.Current.Animations.Remove(RangeControl, RangeControl.AnimationId);
			if(!RangeControl.AllowAnimation) {
				RangeControl.UpdateRange(newMin, newMax);
				return;
			}		 
			RangeAnimationInfo info = new RangeAnimationInfo(RangeControl, RangeMinimum, RangeMaximum, min, max, RangeControlViewInfo.RangeAnimationLength);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected internal virtual void AddViewPortAnimation(double newValue) {
			if(XtraAnimator.Current.Get(RangeControl, RangeControl.AnimationId) != null)
				XtraAnimator.Current.Animations.Remove(RangeControl, RangeControl.AnimationId);
			if(!RangeControl.AllowAnimation) {
				RangeControl.VisibleRangeStartPosition = newValue;
				return;
			}
			RangeControlDoubleAnimationInfo info = new RangeControlDoubleAnimationInfo(RangeControl, RangeControl.VisibleRangeStartPosition, newValue, RangeControlViewInfo.RangeAnimationLength);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected internal virtual void UpdateAnimatedViewPort(double value) {
			IsContentImageReady = false;
			RangeControl.IneternalSetViewPortPosition(value);
			CalcRects();
		}
		protected override ObjectInfoArgs GetBorderArgs(Rectangle bounds) {
			if(RangeControl.BorderStyle == DevExpress.XtraEditors.Controls.BorderStyles.Default)
				return GetBorderInfo();
			return base.GetBorderArgs(bounds);
		}
		protected override BorderPainter GetBorderPainterCore() {
			if(LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin && RangeControl.BorderStyle == DevExpress.XtraEditors.Controls.BorderStyles.Default)
				return new RangeControlBorderPainter(LookAndFeel.ActiveLookAndFeel);
			return base.GetBorderPainterCore();
		}
	}
	public class RangeControlBorderPainter : SkinTextBorderPainter {
		public RangeControlBorderPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinRangeControlBorder], e.Bounds);
		}
	}
	public class RangeAnimationInfo : BaseAnimationInfo {
		public RangeAnimationInfo(RangeControl rangeControl, double startMin, double startMax, double endMin, double endMax, int animLength)
			: base(rangeControl, rangeControl.AnimationId, (int)(TimeSpan.TicksPerMillisecond * animLength / 300), 300) {
			RangeControl = rangeControl;
			RangeControl.IsAnimating = true;
			NormalizedRangeInfo info = new NormalizedRangeInfo(endMin, endMax);
			if(RangeControl.Client != null) {
				RangeControl.Client.ValidateRange(info);
			}
			StartMin = startMin;
			StartMax = startMax;
			EndMin = endMin;
			EndMax = endMax;
			Min = new SplineAnimationHelper();
			Min.Init((float)StartMin, (float)EndMin, 1.0);
			Max = new SplineAnimationHelper();
			Max.Init((float)StartMax, (float)EndMax, 1.0);
		}
		public RangeControl RangeControl { get; private set; }
		public double StartMin { get; private set; }
		public double EndMin { get; private set; }
		public double StartMax { get; private set; }
		public double EndMax { get; private set; }
		public double CurrentMin { get; private set; }
		public double CurrentMax { get; private set; }
		SplineAnimationHelper Min { get; set; }
		SplineAnimationHelper Max { get; set; }
		public override void FrameStep() {
			double k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0;
			CurrentMin = Min.CalcSpline(k);
			CurrentMax = Max.CalcSpline(k);
			RangeControl.RangeViewInfo.UpdateAnimatedRangeBounds();
			RangeControl.Invalidate();
			if(IsFinalFrame) {
				RangeControl.UpdateRange(EndMin, EndMax);
				RangeControl.IsAnimating = false;
			}
		}
	}
	public class RangeControlDoubleAnimationInfo : BaseAnimationInfo {
		public RangeControlDoubleAnimationInfo(RangeControl rangeControl, double start, double end, int animLength)
			: base(rangeControl, rangeControl.AnimationId, (int)(TimeSpan.TicksPerMillisecond * animLength / 300), 300) {
			RangeControl = rangeControl;
			Start = start;
			End = end;
			ValueHelper = new SplineAnimationHelper();
			ValueHelper.Init((float)Start, (float)End, 1.0);
		}
		public RangeControl RangeControl { get; private set; }
		public double Start { get; private set; }
		public double End { get; private set; }
		public double Value { get; private set; }
		SplineAnimationHelper ValueHelper { get; set; }
		public override void FrameStep() {
			double k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0;
			Value = ValueHelper.CalcSpline(k);
			RangeControl.RangeViewInfo.UpdateAnimatedViewPort(Value);
			RangeControl.Invalidate();
		}
	}
}
