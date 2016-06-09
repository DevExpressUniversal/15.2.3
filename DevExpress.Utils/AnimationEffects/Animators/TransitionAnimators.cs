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
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Animation {
	public enum TransitionDirection { Vertical, Horizontal, LeftToRightDiagonal, RightToLeftDiagonal }
	public class PushTransition : SlideTransition {
		public PushTransition() : base() { }
		public PushTransition(Image from, Image to, IPushAnimationParameters parameters)
			: base(from, to, parameters) { }
		protected override string GetTitle() { return "Push"; }
		protected override TransitionDirection TransitionDirection {
			get {
				if(Parameters.EffectOptions == PushEffectOptions.FromBottom || Parameters.EffectOptions == PushEffectOptions.FromTop)
					return TransitionDirection.Vertical;
				return base.TransitionDirection;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IPushAnimationParameters Parameters { get { return base.Parameters as IPushAnimationParameters; } }
		protected override bool ForwardDirection {
			get {
				return Parameters.EffectOptions == PushEffectOptions.FromRight || Parameters.EffectOptions == PushEffectOptions.FromBottom;
			}
		}
		protected override IAnimationParameters CreateAnimationParameters() {
			return new PushAnimationParameters();
		}
	}
	public class ClockTransition : SlideTransition {
		public ClockTransition() : base() { }
		public ClockTransition(Image from, Image to, IClockAnimationParameters parameters)
			: base(from, to, parameters) { }
		protected virtual GraphicsPath CalcClipBoundsFrom(Rectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			Rectangle realBounds = Calc(bounds);
			float sweepAngle = (float)(360 * (Progress - 1));
			switch(Parameters.EffectOptions) {
				case ClockEffectOptions.Counterclockwise:
					sweepAngle = (float)(360 * (1 - Progress));
					break;
				case ClockEffectOptions.Wedge:
					path.AddPie(realBounds, 270, (float)(360 * (1 - Progress / 2)));
					sweepAngle = (float)(360 * Progress / 2);
					break;
			}
			path.AddPie(realBounds, 270, sweepAngle);
			return path;
		}
		Rectangle Calc(Rectangle bounds) {
			int length = (int)Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height);
			int x = (int)(length - bounds.Width) / 2;
			int y = (int)(length - bounds.Height) / 2;
			return new Rectangle(bounds.X - x, bounds.Y - y, length, length);
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			using(Region region = new Region(CalcClipBoundsFrom(bounds))) {
				cache.Graphics.Clip = region;
				cache.Graphics.DrawImageUnscaled(From, bounds.Location);
			}
		}
		protected override IAnimationParameters CreateAnimationParameters() {
			return new ClockAnimationParameters();
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IClockAnimationParameters Parameters { get { return base.Parameters as IClockAnimationParameters; } }
		protected override string GetTitle() { return "Clock"; }
	}
	public class ShapeTransition : SlideTransition {
		public ShapeTransition() : base() { }
		public ShapeTransition(Image from, Image to, IShapeAnimationParameters parameters)
			: base(from, to, parameters) { }
		Rectangle Calc(Rectangle bounds, double progress, bool isCircle) {
			Size size = isCircle ? CalcCircleSize(bounds, progress) : CalcSize(bounds, progress);
			return CalcBounds(size, bounds);
		}
		Rectangle CalcBounds(Size size, Rectangle bounds) {
			int x = (int)(size.Width - bounds.Width) / 2;
			int y = (int)(size.Height - bounds.Height) / 2;
			return new Rectangle(new Point(bounds.X - x, bounds.Y - y), size);
		}
		Size CalcSize(Rectangle bounds, double progress) {
			int width = (int)(bounds.Width * progress);
			int height = (int)(bounds.Height * progress);
			return new Size(width, height);
		}
		Size CalcCircleSize(Rectangle bounds, double progress) {
			int length = (int)Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height);
			int newlength = (int)(length * progress);
			return new Size(newlength, newlength);
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IShapeAnimationParameters Parameters { get { return base.Parameters as IShapeAnimationParameters; } }
		protected override bool ForwardDirection {
			get {
				return Parameters.EffectOptions == ShapeEffectOptions.Out || Parameters.EffectOptions == ShapeEffectOptions.CircleOut;
			}
		}
		protected virtual GraphicsPath CalcClipBoundsFrom(Rectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			switch(Parameters.EffectOptions) {
				case ShapeEffectOptions.In:
					path.AddRectangle(Calc(bounds, Progress, false));
					break;
				case ShapeEffectOptions.Out:
					path.AddRectangle(Calc(bounds, 1 - Progress, false));
					break;
				case ShapeEffectOptions.CircleOut:
					path.AddEllipse(Calc(bounds, 1 - Progress, true));
					break;
				default:
					path.AddEllipse(Calc(bounds, Progress, true));
					break;
			}
			return path;
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			Image from = ForwardDirection ? To : From;
			Image to = ForwardDirection ? From : To;
			cache.Graphics.DrawImageUnscaled(from, bounds.Location);
			using(Region region = new Region(CalcClipBoundsFrom(bounds))) {
				cache.Graphics.Clip = region;
				cache.Graphics.DrawImageUnscaled(to, bounds.Location);
			}
		}
		protected override string GetTitle() { return "Shape"; }
		protected override IAnimationParameters CreateAnimationParameters() {
			return new ShapeAnimationParameters();
		}
	}
	public class SlideTransition : BaseTransition {
		double progress;
		public SlideTransition() : base() { }
		public SlideTransition(Image from, Image to, IAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected virtual TransitionDirection TransitionDirection { get { return TransitionDirection.Horizontal; } }
		protected virtual bool ForwardDirection { get { return false; } }
		protected virtual Point CalculateLocationFrom(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(TransitionDirection == TransitionDirection.Vertical) return new Point(bounds.Left, bounds.Top + forward * (int)(progress * bounds.Height));
			return new Point(bounds.Left + forward * (int)(progress * bounds.Width), bounds.Top);
		}
		protected virtual Point CalculateLocationTo(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(TransitionDirection == TransitionDirection.Vertical) return new Point(bounds.Left, bounds.Top + forward * (int)((progress - 1) * bounds.Height));
			return new Point(bounds.Left + forward * (int)((progress - 1) * bounds.Width), bounds.Top);
		}
		protected override void OnAnimationCore(BaseAnimationInfo info, double progress) {
			this.progress = progress;
		}
		protected override void OnStopAnimation() {
			base.OnStopAnimation();
			progress = 0;
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			IntPtr hDC = cache.Graphics.GetHdc();
			try {
				Rectangle target = new Rectangle(CalculateLocationFrom(bounds), bounds.Size);
				Rectangle source = new Rectangle(Point.Empty, bounds.Size);
				SlideFadeTransition.ValidateBounds(bounds, ref target, ref source);
				DrawImage_BitBlt(hDC, From, target, source);
				target = new Rectangle(CalculateLocationTo(bounds), bounds.Size);
				source = new Rectangle(Point.Empty, bounds.Size);
				SlideFadeTransition.ValidateBounds(bounds, ref target, ref source);
				DrawImage_BitBlt(hDC, To, target, source);
			}
			finally { cache.Graphics.ReleaseHdc(hDC); }
		}
		protected double Progress { get { return progress; } }
	}
	public class FadeTransition : BaseTransition {
		public FadeTransition() : base() { }
		public FadeTransition(Image from, Image to, IAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		byte CalcOpacity(double progress) {
			if(progress <= 0) return (byte)0;
			if(progress >= 1) return (byte)255.0;
			return (byte)(int)(255.0 * progress);
		}
		byte opacity;
		protected override void OnAnimationCore(BaseAnimationInfo info, double progress) {
			opacity = CalcOpacity(progress);
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			IntPtr hDC = cache.Graphics.GetHdc();
			try {
				Rectangle source = new Rectangle(Point.Empty, bounds.Size);
				DrawImage_BitBlt(hDC, From, bounds, source);
				DrawImage_AlphaBlend(hDC, To, bounds, source, opacity);
			}
			finally { cache.Graphics.ReleaseHdc(hDC); }
		}
		protected override void OnStopAnimation() {
			base.OnStopAnimation();
			opacity = 0;
		}
		protected override string GetTitle() { return "Fade"; }
	}
	public class SegmentedFadeTransition : BaseTransition {
		protected float RenderImageOpacity { get; set; }
		static int SegmentCount = 50;
		protected int SegmentSize {
			get {
				if(From == null) return 1;
				if(From.Size.Height <= SegmentCount) return 1;
				return From.Size.Height / SegmentCount;
			}
		}
		int segmentRowCountCore = 0;
		protected int SegmentRow {
			get {
				if(segmentRowCountCore == 0)
					segmentRowCountCore = GetCount(From.Size.Height, SegmentSize);
				return segmentRowCountCore;
			}
		}
		int segmentColumnCountCore = 0;
		protected int SegmentColumn {
			get {
				if(segmentColumnCountCore == 0)
					segmentColumnCountCore = GetCount(From.Size.Width, SegmentSize);
				return segmentColumnCountCore;
			}
		}
		public SegmentedFadeTransition() : base() { }
		public SegmentedFadeTransition(Image from, Image to, IAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		int GetCount(int size, int itemSize) {
			int res = size / itemSize;
			if(size % itemSize != 0)
				res += 1;
			return res;
		}
		protected override void OnAnimationCore(BaseAnimationInfo info, double progress) {
			RenderImageOpacity = (float)progress;
		}
		Bitmap bufferBitmapCore;
		Bitmap BufferBitmap {
			get {
				if(From == null) return null;
				if(bufferBitmapCore == null || bufferBitmapCore.Width != From.Width || bufferBitmapCore.Height != From.Height) {
					Ref.Dispose(ref bufferBitmapCore);
					bufferBitmapCore = new Bitmap(From.Width, From.Height);
				}
				return bufferBitmapCore;
			}
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			if(RenderImageOpacity >= 0.01f) {
				cache.Graphics.DrawImageUnscaled(To, bounds.Location);
				using(Graphics graphic = Graphics.FromImage(BufferBitmap)) {
					graphic.DrawImageUnscaled(From, Point.Empty);
					IntPtr sourceDC = cache.Graphics.GetHdc();
					IntPtr targetDC = graphic.GetHdc();
					DrawImageCore(cache, sourceDC, targetDC, bounds);
					cache.Graphics.ReleaseHdc(sourceDC);
					graphic.ReleaseHdc(targetDC);
				}
				if(!AnimationInProgress) return;
				cache.Graphics.DrawImageUnscaled(BufferBitmap, bounds.Location);
			}
			else
				cache.Graphics.DrawImageUnscaled(From, bounds.Location);
		}
		protected virtual void DrawImageCore(GraphicsCache cache, IntPtr sourceDC, IntPtr targetDC, Rectangle bounds) {
			for(int j = 0; j < SegmentColumn; j++) {
				for(int i = 0; i < SegmentRow; i++) {
					DrawSegment(cache, sourceDC, targetDC, bounds, j, i, i * j);
				}
			}
		}
		protected virtual void DrawSegment(GraphicsCache cache, IntPtr sourceDC, IntPtr targetDC, Rectangle bounds, int x, int y, int index) {
			float opSize = 1.0f / (SegmentColumn * SegmentRow + 10);
			float start = index * opSize;
			float opacity = 0.0f;
			if(RenderImageOpacity > start) {
				opacity = RenderImageOpacity > start + opSize * 10 ? 1.0f : (RenderImageOpacity - start) / (opSize * 10);
			}
			if(!AnimationInProgress) return;
			if(opacity == 1.0f)
				DevExpress.Utils.Drawing.Helpers.NativeMethods.BitBlt(targetDC, x * SegmentSize, y * SegmentSize, SegmentSize,
					SegmentSize, sourceDC, x * SegmentSize + bounds.X, y * SegmentSize + bounds.Y, 0xCC0020);
			return;
		}
		protected override string GetTitle() { return "SegmentedFade"; }
		protected override void OnDispose() {
			base.OnDispose();
			Ref.Dispose(ref bufferBitmapCore);
		}
	}
	public class DissolveTransition : SegmentedFadeTransition {
		public DissolveTransition() : base() { }
		public DissolveTransition(Image from, Image to, IAnimationParameters parameters)
			: base(from, to, parameters) { }
		internal List<Point> RandomSegments { get; set; }
		protected internal virtual void GenerateRandomSegments() {
			List<Point> list = new List<Point>();
			RandomSegments = new List<Point>();
			for(int i = 0; i < SegmentColumn; i++) {
				for(int j = 0; j < SegmentRow; j++) {
					list.Add(new Point(i, j));
				}
			}
			Random r = new Random();
			while(list.Count > 0) {
				int t = r.Next(list.Count);
				RandomSegments.Add(list[t]);
				list.RemoveAt(t);
			}
		}
		protected override void DrawImageCore(GraphicsCache cache, IntPtr sourceDC, IntPtr targetDC, Rectangle bounds) {
			for(int i = 0; i < RandomSegments.Count; i++) {
				DrawSegment(cache, sourceDC, targetDC, bounds, RandomSegments[i].X, RandomSegments[i].Y, i);
			}
		}
		protected override string GetTitle() { return "Dissolve"; }
		protected override void OnStartAnimation() {
			GenerateRandomSegments();
		}
	}
	public class SlideFadeTransition : PushTransition {
		public SlideFadeTransition() : base() { }
		public SlideFadeTransition(Image from, Image to, IPushAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		[
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ISlideFadeAnimationParameters Parameters { get { return base.Parameters as ISlideFadeAnimationParameters; } }
		protected override Data.Utils.IEasingFunction DefaultEasingFunction { get { return null; } }
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			cache.Graphics.Clear(GetBackground(Parameters.Background));
			IntPtr hDC = cache.Graphics.GetHdc();
			try {
				byte opacity = CalculateOpacity();
				Point location = CalculateLocationFrom(bounds);
				Rectangle target = new Rectangle(location, bounds.Size);
				Rectangle source = new Rectangle(0, 0, bounds.Width, bounds.Height);
				ValidateBounds(bounds, ref target, ref source);
				DrawImage_AlphaBlend(hDC, From, target, source, opacity);
				location = CalculateLocationTo(bounds);
				target = new Rectangle(location, bounds.Size);
				source = new Rectangle(0, 0, bounds.Width, bounds.Height);
				ValidateBounds(bounds, ref target, ref source);
				DrawImage_AlphaBlend(hDC, To, target, source, (byte)(255.0 - opacity));
			}
			finally { cache.Graphics.ReleaseHdc(hDC); }
		}
		internal static void ValidateBounds(Rectangle bounds, ref Rectangle boundsToPaint, ref Rectangle sourceImage) {
			int deltaX = 0, deltaWidth = 0;
			if(boundsToPaint.X < bounds.X) {
				deltaX = bounds.X - boundsToPaint.X;
				deltaWidth = deltaX;
				boundsToPaint.X = bounds.X;
				boundsToPaint.Width -= deltaWidth;
			}
			if(boundsToPaint.Right > bounds.Right) {
				deltaWidth += (boundsToPaint.Right - bounds.Right);
				boundsToPaint.Width -= boundsToPaint.Right - bounds.Right;
			}
			sourceImage.X += deltaX;
			sourceImage.Width -= deltaWidth;
		}
		protected override Point CalculateLocationTo(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(TransitionDirection == TransitionDirection.Vertical)
				return new Point(bounds.Left, bounds.Top + forward * (int)((Progress - 1.0) * (double)bounds.Height * 0.2));
			return new Point(bounds.Left + forward * (int)((Progress - 1.0) * (double)bounds.Width * 0.2), bounds.Top);
		}
		protected override Point CalculateLocationFrom(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(TransitionDirection == TransitionDirection.Vertical) return new Point(bounds.Left, bounds.Top + forward * (int)(Progress * (double)bounds.Height * 0.2));
			return new Point(bounds.Left + forward * (int)(Progress * (double)bounds.Width * 0.2), bounds.Top);
		}
		protected virtual byte CalculateOpacity() {
			if(Progress <= 0) return (byte)255.0;
			if(Progress >= 1) return (byte)0;
			return (byte)(255 * (1.0 - Progress));
		}
		protected override string GetTitle() {
			return "SlideFade";
		}
		protected override IAnimationParameters CreateAnimationParameters() {
			return new SlideFadeAnimationParameters();
		}
	}
	public class CoverTransition : SlideTransition {
		public CoverTransition() : base() { }
		public CoverTransition(Image from, Image to, ICoverAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ICoverAnimationParameters Parameters { get { return base.Parameters as ICoverAnimationParameters; } }
		protected override Point CalculateLocationFrom(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(!Parameters.UnCover) return Point.Empty;
			if(TransitionDirection == TransitionDirection.LeftToRightDiagonal)
				return new Point(bounds.Left + forward * (int)((Progress) * bounds.Width), bounds.Top + forward * (int)((Progress) * bounds.Height));
			if(TransitionDirection == TransitionDirection.RightToLeftDiagonal)
				return new Point(bounds.Left - forward * (int)((Progress) * bounds.Width), bounds.Top + forward * (int)((Progress) * bounds.Height));
			return base.CalculateLocationFrom(bounds);
		}
		protected override Point CalculateLocationTo(Rectangle bounds) {
			int forward = ForwardDirection ? -1 : 1;
			if(Parameters.UnCover) return Point.Empty;
			if(TransitionDirection == TransitionDirection.LeftToRightDiagonal)
				return new Point(bounds.Left + forward * (int)((Progress - 1) * bounds.Width), bounds.Top + forward * (int)((Progress - 1) * bounds.Height));
			if(TransitionDirection == TransitionDirection.RightToLeftDiagonal)
				return new Point(bounds.Left + forward * (int)((Progress - 1) * bounds.Width), bounds.Top - forward * (int)((Progress - 1) * bounds.Height));
			return base.CalculateLocationTo(bounds);
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			if(Parameters.UnCover) {
				cache.Graphics.DrawImageUnscaled(To, CalculateLocationTo(bounds));
				cache.Graphics.DrawImageUnscaled(From, CalculateLocationFrom(bounds));
				return;
			}
			base.DrawCore(cache, bounds);
		}
		protected override bool ForwardDirection {
			get {
				return Parameters.EffectOptions == CoverEffectOptions.FromRight || Parameters.EffectOptions == CoverEffectOptions.FromBottom || Parameters.EffectOptions == CoverEffectOptions.FromTopRight || Parameters.EffectOptions == CoverEffectOptions.FromBottomRight;
			}
		}
		protected override TransitionDirection TransitionDirection {
			get {
				if(Parameters.EffectOptions == CoverEffectOptions.FromTopLeft || Parameters.EffectOptions == CoverEffectOptions.FromBottomRight)
					return TransitionDirection.LeftToRightDiagonal;
				if(Parameters.EffectOptions == CoverEffectOptions.FromTopRight || Parameters.EffectOptions == CoverEffectOptions.FromBottomLeft)
					return Animation.TransitionDirection.RightToLeftDiagonal;
				if(Parameters.EffectOptions == CoverEffectOptions.FromBottom || Parameters.EffectOptions == CoverEffectOptions.FromTop)
					return Animation.TransitionDirection.Vertical;
				return base.TransitionDirection;
			}
		}
		protected override IAnimationParameters CreateAnimationParameters() {
			return new CoverAnimationParameters();
		}
		protected override string GetTitle() { return "Cover"; }
	}
	public class CombTransition : BaseTransition {
		double progress;
		public CombTransition() : base() { }
		public CombTransition(Image from, Image to, ICombAnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected override void OnAnimationCore(BaseAnimationInfo info, double progress) {
			this.progress = progress;
		}
		protected override void OnStopAnimation() {
			base.OnStopAnimation();
			progress = 0;
		}
		protected virtual Point OffsetLocationFrom(Rectangle bounds) {
			if(Parameters.Inward) return Point.Empty;
			if(Parameters.Vertical) return new Point(0, (int)(progress * bounds.Height));
			return new Point((int)(progress * bounds.Width), 0);
		}
		protected virtual Point OffsetLocationTo(Rectangle bounds) {
			if(!Parameters.Inward) return Point.Empty;
			if(Parameters.Vertical) return new Point(0, (int)((progress - 1) * bounds.Height));
			return new Point((int)((progress - 1) * bounds.Width), 0);
		}
		protected override void DrawCore(GraphicsCache cache, Rectangle bounds) {
			if(Parameters.Inward) {
				DrawParts(cache.Graphics, bounds, (Bitmap)From, (Bitmap)To, OffsetLocationTo(bounds));
				return;
			}
			DrawParts(cache.Graphics, bounds, (Bitmap)To, (Bitmap)From, OffsetLocationFrom(bounds));
		}
		Point CalcLocationPartImage(Rectangle bounds, int index, Bitmap part) {
			int x = Parameters.Vertical ? part.Width * index : 0;
			int y = Parameters.Vertical ? 0 : part.Height * index;
			return new Point(bounds.X + x, bounds.Top + y);
		}
		void DrawParts(Graphics g, Rectangle bounds, Bitmap source, Bitmap combImage, Point animationOffset) {
			g.DrawImageUnscaled(source, bounds.Location);
			Bitmap[] images = CutImageHelper.CutImage(combImage, Parameters.StripeCount, bounds, Parameters.Vertical);
			for(int i = 0; i < images.Length; i++) {
				int forward = i % 2 == 0 ? -1 : 1;
				Bitmap image = images[i];
				Point locationPart = CalcLocationPartImage(bounds, i, image);
				g.DrawImageUnscaled(image, new Point(locationPart.X + forward * animationOffset.X, locationPart.Y + forward * animationOffset.Y));
				Ref.Dispose(ref image);
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ICombAnimationParameters Parameters { get { return base.Parameters as ICombAnimationParameters; } }
		protected override IAnimationParameters CreateAnimationParameters() {
			return new CombAnimationParameters();
		}
		protected override string GetTitle() {
			return "Comb";
		}
	}
}
