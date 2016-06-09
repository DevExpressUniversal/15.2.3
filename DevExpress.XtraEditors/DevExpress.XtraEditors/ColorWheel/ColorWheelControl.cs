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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Colors;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.ColorWheel {
	public class ColorWheelControlViewInfo : BaseControlViewInfo {
		public ColorWheelControlViewInfo(object owner)
			: base(owner) {
			this.cursorPoisition = new PointF(0.5f, 0.5f);
		}
		public ColorWheelControl ColorWheel { get { return ((ColorWheelControl)OwnerControl); } }
		public Image CursorImage {
			get { return ((ColorWheelControl)OwnerControl).CursorImage; }
		}
		Image cachedImage;
		protected internal Image CachedImage {
			get {
				if(cachedImage == null)
					cachedImage = CreateCachedImage(Math.Min(OwnerControl.Width, OwnerControl.Height));
				return cachedImage;
			}
		}
		protected internal void ResetCache() {
			if(this.cachedImage != null)
				this.cachedImage.Dispose();
			this.cachedImage = null;
		}
		protected virtual Image CreateCachedImage(int size) {
			Image img = new Bitmap(size, size);
			size--;
			double length = Math.PI * size;
			using(Graphics g = Graphics.FromImage(img)) {
				g.SmoothingMode = SmoothingMode.AntiAlias;
				GraphicsCache cache = new GraphicsCache(g);
				for(double pos = 0.0; pos <= length; pos += 0.5) {
					double angle = 2.0 * Math.PI * pos / length;
					float x = (float)(0.5 * size * Math.Cos(angle));
					float y = (float)(0.5 * size * Math.Sin(angle));
					HueSatBright hsb = new HueSatBright(pos / length, 1.0, 1.0);
					g.DrawLine(cache.GetPen(hsb.AsRGB), new PointF(x + size * 0.5f, y + size * 0.5f), new PointF(size * 0.5f, size * 0.5f));
				}
				Image img2 = new Bitmap(img.Width, img.Height);
				using(Graphics g2 = Graphics.FromImage(img2)) {
					g2.Clear(Color.Transparent);
					GraphicsPath path = new GraphicsPath();
					path.AddEllipse(0, 0, size, size);
					PathGradientBrush pthGrBrush = new PathGradientBrush(path);
					pthGrBrush.CenterColor = Color.FromArgb(255, 255, 255, 255);
					Color[] colors = { Color.FromArgb(0, 255, 255, 255) };
					pthGrBrush.SurroundColors = colors;
					g2.FillEllipse(pthGrBrush, 0, 0, size, size);
				}
				g.DrawImageUnscaled(img2, Point.Empty);
				cache.Clear();
			}
			return img;
		}
		PointF cursorPoisition;
		protected internal PointF CursorPosition {
			get { return cursorPoisition; }
			set {
				if(cursorPoisition == value)
					return;
				cursorPoisition = value;
				((ColorWheelControl)OwnerControl).OnColorChanged();
			}
		}
		protected internal PointF DefaultCursorPosition {
			get { return new PointF(0.5f, 0.5f); }
		}
		protected internal Rectangle InnerCursorBounds {
			get {
				PointF pt = ColorWheel.Color == Color.Empty ? DefaultCursorPosition : CursorPosition;
				int x = (int)(pt.X * CachedImage.Width);
				int y = (int)(pt.Y * CachedImage.Height);
				return new Rectangle(x - 5, y - 5, 11, 11);
			}
		}
		protected internal Rectangle OuterCursorBounds {
			get {
				PointF pt = ColorWheel.Color == Color.Empty ? DefaultCursorPosition : CursorPosition;
				if(CursorImage != null) {
					return new Rectangle((int)(pt.X * CachedImage.Width) - CursorImage.Width / 2, (int)(pt.Y * CachedImage.Height) - CursorImage.Height / 2, CursorImage.Width, CursorImage.Height);
				}
				Rectangle res = InnerCursorBounds;
				res.Inflate(2, 2);
				return res;
			}
		}
		protected internal Color ColorFromPosition(PointF cursor) {
			if(cursor.X == -1.0f && cursor.Y == -1.0f)
				return Color.Empty;
			cursor.X -= 0.5f;
			cursor.Y -= 0.5f;
			double radius = 1.0 - 2.0 * Math.Sqrt(cursor.X * cursor.X + cursor.Y * cursor.Y);
			double rad = Math.Atan2(cursor.Y, cursor.X);
			if(rad < 0.0) rad += Math.PI * 2;
			rad /= Math.PI * 2;
			return (new HueSatBright(rad, 1.0 - radius, ((ColorWheelControl)OwnerControl).Brightness)).AsRGB;
		}
		protected internal PointF ColorToPosition(Color value) {
			if(value == Color.Empty)
				return new PointF(-1.0f, -1.0f);
			HueSatBright res = new HueSatBright(value);
			double radius = res.Saturation * 0.5f;
			double rad = 2.0 * Math.PI * res.Hue;
			double x = 0.5 + radius * Math.Cos(rad);
			double y = 0.5 + radius * Math.Sin(rad);
			return new PointF((float)x, (float)y);
		}
	}
	public class ColorWheelPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			DrawColorWheelImage(info);
			GraphicsPath p = new GraphicsPath();
			ColorWheelControlViewInfo vi = (ColorWheelControlViewInfo)info.ViewInfo;
			p.AddEllipse(vi.Bounds);
			info.Graphics.SetClip(p);
			DrawCursor(info);
			info.Graphics.ResetClip();
			p.Dispose();
		}
		protected virtual void DrawCursor(ControlGraphicsInfoArgs info) {
			ColorWheelControlViewInfo vi = (ColorWheelControlViewInfo)info.ViewInfo;
			if(!vi.ColorWheel.Enabled)
				return;
			if(vi.CursorImage != null) {
				info.Graphics.DrawImage(vi.CursorImage, vi.OuterCursorBounds);
				return;
			}
			info.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			info.Graphics.DrawEllipse(info.Cache.GetPen(Color.White, 2), vi.InnerCursorBounds);
			info.Graphics.DrawEllipse(info.Cache.GetPen(Color.FromArgb(52, 127, 126), 2), vi.OuterCursorBounds);
		}
		protected virtual void DrawColorWheelImage(ControlGraphicsInfoArgs info) {
			ColorWheelControlViewInfo vi = (ColorWheelControlViewInfo)info.ViewInfo;
			if(!vi.ColorWheel.Enabled)
				info.Graphics.DrawImage(vi.CachedImage, new Rectangle(0, 0, vi.CachedImage.Width, vi.CachedImage.Height), 0, 0, vi.CachedImage.Width, vi.CachedImage.Height, GraphicsUnit.Pixel, DisabledAttributes);
			else
				info.Graphics.DrawImageUnscaled(vi.CachedImage, Point.Empty);
		}
		internal static ImageAttributes DisabledAttributes {
			get {
				if(disabledAttributes == null)
					InitDisabledAttributes();
				return disabledAttributes;
			}
		}
		static ImageAttributes disabledAttributes;
		static void InitDisabledAttributes() {
			float[][] array = new float[5][];
			array[0] = new float[5] { 0.10801f, 0.10801f, 0.10801f, 0, 0 };
			array[1] = new float[5] { 0.21329f, 0.21329f, 0.21329f, 0, 0 };
			array[2] = new float[5] { 0.0287f, 0.0287f, 0.0287f, 0, 0 };
			array[3] = new float[5] { 0, 0, 0, 0.9f, 0 };
			array[4] = new float[5] { 0.5f, 0.5f, 0.5f, 0, 1f };
			ColorMatrix cm = new ColorMatrix(array);
			disabledAttributes = new ImageAttributes();
			disabledAttributes.SetColorMatrix(cm);
		}
	}
	[DXToolboxItem(false)]
	public class ColorWheelControl : BaseControl {
		private static readonly object colorChanged = new object();
		public ColorWheelControl() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
		}
		ColorWheelPainter painter;
		protected internal override DevExpress.XtraEditors.Drawing.BaseControlPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual ColorWheelPainter CreatePainter() {
			return new ColorWheelPainter();
		}
		double brightness = 1.0;
		[DefaultValue(1.0)]
		public double Brightness {
			get {
				return brightness; 
			}
			set {
				if(Brightness == value)
					return;
				brightness = value;
				OnColorChanged();
			}
		}
		[DefaultValue(null)]
		public Image CursorImage {
			get;
			set;
		}
		ColorWheelControlViewInfo viewInfo;
		protected internal override DevExpress.XtraEditors.ViewInfo.BaseControlViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected virtual ColorWheelControlViewInfo CreateViewInfo() {
			return new ColorWheelControlViewInfo(this);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			((ColorWheelControlViewInfo)ViewInfo).ResetCache();
		}
		ColorWheelControlViewInfo ColorWheelViewInfo { get { return (ColorWheelControlViewInfo)ViewInfo; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			SuppressRaiseColorChanged = true;
			ColorWheelViewInfo.CursorPosition = ConstrainPosition(new PointF((float)e.Location.X / ColorWheelViewInfo.CachedImage.Width, (float)e.Location.Y / ColorWheelViewInfo.CachedImage.Height));
		}
		bool SuppressRaiseColorChanged { get; set; }
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(e.Button != System.Windows.Forms.MouseButtons.Left)
				return;
			SuppressRaiseColorChanged = true;
			ColorWheelViewInfo.CursorPosition = ConstrainPosition(new PointF((float)e.Location.X / ColorWheelViewInfo.CachedImage.Width, (float)e.Location.Y / ColorWheelViewInfo.CachedImage.Height));
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			SuppressRaiseColorChanged = false;
			RaiseOnColorChanged();
		}
		protected virtual PointF ConstrainPosition(PointF pt) {
			double radius = Math.Sqrt((pt.X - 0.5) * (pt.X - 0.5) + (pt.Y - 0.5) * (pt.Y - 0.5));
			if(radius <= 0.5)
				return pt;
			radius = 0.5;
			double rad = Math.Atan2(pt.Y - 0.5, pt.X - 0.5);
			pt.X = (float)(0.5 + radius * Math.Cos(rad));
			pt.Y = (float)(0.5 + radius * Math.Sin(rad));
			return pt;
		}
		protected internal virtual void OnColorChanged() {
			Invalidate();
			RaiseOnColorChanged();
		}
		public event EventHandler ColorChanged {
			add { Events.AddHandler(colorChanged, value); }
			remove { Events.RemoveHandler(colorChanged, value); }
		}
		protected virtual void RaiseOnColorChanged() {
			if(SuppressRaiseColorChanged)
				return;
			EventHandler handler = Events[colorChanged] as EventHandler;
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		public Color Color {
			get { return ColorWheelViewInfo.ColorFromPosition(ColorWheelViewInfo.CursorPosition); }
			set {  
				ColorWheelViewInfo.CursorPosition = ColorWheelViewInfo.ColorToPosition(value);
				UpdateBrightness(value);
			}
		}
		private void UpdateBrightness(System.Drawing.Color value) {
			if(value == Color.Empty)
				Brightness = 1.0;
			else 
				Brightness = new HueSatBright(value).Brightness;
		}
	}
	[DXToolboxItem(false)]
	public class ColorIndicator : Control {
		public ColorIndicator() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			BackColor = Color.Transparent;
		}
		Color color;
		public Color Color {
			get { return color; }
			set {
				if(Color == value)
					return;
				color = value;
				Invalidate();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			SolidBrush br = new SolidBrush(Color);
			Rectangle rect = ClientRectangle;
			rect.Width--; rect.Height--;
			e.Graphics.FillEllipse(br, rect);
		}
	}
	public class ColorSliderPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			ColorSliderViewInfo vi = (ColorSliderViewInfo)info.ViewInfo;
			if(vi.SliderImage != null)
				info.Graphics.DrawImage(vi.SliderImage, vi.SliderBounds);
		}
	}
	public class ColorSliderViewInfo : BaseControlViewInfo {
		public ColorSliderViewInfo(object owner) : base(owner) { }
		public ColorSlider SliderControl { get { return ((ColorSlider)OwnerControl); } }
		public Image SliderImage { get { return SliderControl.SliderImage; } }
		public Rectangle ScrollArea {
			get {
				if(SliderImage == null)
					return Rectangle.Empty;
				int startPos = Bounds.X + SliderImage.Width / 2;
				int width = Bounds.Width - SliderImage.Width;
				return new Rectangle(startPos, Bounds.Y + Bounds.Height / 2, width, 1);
			}
		}
		[Obsolete("Use SliderPosition"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Point SliderPoisition {
			get {
				return SliderPosition;
			}
		}
		public Point SliderPosition {
			get {
				return new Point(ScrollArea.X + (int)(ScrollArea.Width * SliderControl.Value), ScrollArea.Y);
			}
		}
		public Rectangle SliderBounds {
			get {
				if(SliderImage == null)
					return Rectangle.Empty;
				return new Rectangle(SliderPosition.X - SliderImage.Width / 2, SliderPosition.Y - SliderImage.Height / 2, SliderImage.Width, SliderImage.Height);
			}
		}
		public double ValueFromPoint(Point pt) {
			return Math.Max(0.0, Math.Min(1.0, ((float)(pt.X - ScrollArea.X)) / ScrollArea.Width));
		}
	}
	[DXToolboxItem(false)]
	public class ColorSlider : BaseControl {
		private static readonly object valueChanged = new object();
		public ColorSlider() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
		}
		[DefaultValue(null)]
		public Image SliderImage {
			get;
			set;
		}
		double sliderValue;
		public double Value {
			get { return sliderValue; }
			set {
				if(Value == value)
					return;
				sliderValue = value;
				OnValueChanged();
			}
		}
		protected virtual void OnValueChanged() {
			RaiseValueChanged();
			Invalidate();
		}
		protected virtual void RaiseValueChanged() {
			if(SuppressRaiseValueChanged)
				return;
			EventHandler handler = Events[valueChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public event EventHandler ValueChanged {
			add { Events.AddHandler(valueChanged, value); }
			remove { Events.RemoveHandler(valueChanged, value); }
		}
		ColorSliderPainter painter;
		protected internal override BaseControlPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual ColorSliderPainter CreatePainter() {
			return new ColorSliderPainter();
		}
		ColorSliderViewInfo viewInfo;
		protected internal override BaseControlViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected virtual ColorSliderViewInfo CreateViewInfo() {
			return new ColorSliderViewInfo(this);
		}
		ColorSliderViewInfo SliderViewInfo { get { return (ColorSliderViewInfo)ViewInfo; } }
		Point DownPoint { get; set; }
		Point InvalidPoint = new Point(-1000, -1000);
		int Offset { get; set; }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != System.Windows.Forms.MouseButtons.Left)
				return;
			DownPoint = InvalidPoint;
			if(!SliderViewInfo.SliderBounds.Contains(e.Location)) {
				SuppressRaiseValueChanged = true;
				Value = SliderViewInfo.ValueFromPoint(e.Location);
			}
			else {
				DownPoint = e.Location;
				Offset = DownPoint.X - SliderViewInfo.SliderPosition.X;
			}
		}
		protected bool SuppressRaiseValueChanged { get; set; }
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(e.Button != System.Windows.Forms.MouseButtons.Left || DownPoint == InvalidPoint)
				return;
			SuppressRaiseValueChanged = true;
			Value = SliderViewInfo.ValueFromPoint(new Point(e.Location.X - Offset, e.Location.Y));
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			SuppressRaiseValueChanged = false;
			RaiseValueChanged();
			DownPoint = InvalidPoint;
		}
	}
}
