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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using DevExpress.Utils.Paint;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
namespace DevExpress.Utils.Drawing {
	public class BackgroundPaintHelper {
		static Color defaultDisabledColorDefaultValue = Color.FromArgb(0xb0, 207, 207, 207);
		public static Color DefaultDisabledColor = defaultDisabledColorDefaultValue;
		public delegate void PaintInvoke(Control control, PaintEventArgs e);
		public delegate void BackColorChanged(Control control, EventArgs e);
		public static void PaintDisabledControl(Control control, PaintEventArgs e) {
			PaintDisabledControl(e, new Rectangle(Point.Empty, control.Size));
		}
		public static void PaintDisabledControl(PaintEventArgs e, Rectangle bounds) {
			using(Brush brush = new SolidBrush(DefaultDisabledColor)) {
				e.Graphics.FillRectangle(brush, bounds);
			}
		}
		public static void PaintDisabledControl(Control control, GraphicsCache cache) {
			PaintDisabledControl(cache, new Rectangle(Point.Empty, control.Size));
		}
		public static void PaintDisabledControl(GraphicsCache cache, Rectangle bounds) {
			Brush brush = cache.GetSolidBrush(DefaultDisabledColor);
			cache.FillRectangle(brush, bounds);
		}
		public static void PaintDisabledControl(UserLookAndFeel lookAndFeel, GraphicsCache cache, Rectangle bounds) {
			Brush brush = cache.GetSolidBrush(GetDisabledColor(lookAndFeel));
			cache.FillRectangle(brush, bounds);
		}
		public static void PaintDisabledControl(UserLookAndFeel lookAndFeel, PaintEventArgs e, Rectangle bounds) {
			using(Brush brush = new SolidBrush(GetDisabledColor(lookAndFeel))) {
				e.Graphics.FillRectangle(brush, bounds);
			}
		}
		static Color GetDisabledColor(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel == null) return DefaultDisabledColor;
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && DefaultDisabledColor == defaultDisabledColorDefaultValue) {
				Color res = CommonSkins.GetSkin(lookAndFeel).Colors[CommonColors.DisabledControl];
				return Color.FromArgb(200, res);
			}
			return DefaultDisabledColor;
		}
		public static bool PaintTransparentBackground(Control control, PaintEventArgs e, Rectangle bounds,
			PaintInvoke paint, PaintInvoke paintBackground) {
			Graphics g = e.Graphics;
			Control parent = control.Parent;
			if(parent == null) return false;
			NativeMethods.POINT p = new NativeMethods.POINT();
			p.X = p.Y = 0;
			NativeMethods.MapWindowPoints(control.Handle, parent.Handle, ref p, 1);
			bounds.Offset(p.X, p.Y);
			PaintEventArgs parentArgs = new PaintEventArgs(g, bounds);
			GraphicsState state = g.Save();
			try {
				g.TranslateTransform(-p.X, -p.Y);
				paintBackground(parent, parentArgs);
				g.Restore(state);
				state = g.Save();
				g.TranslateTransform(-p.X, -p.Y);
				paint(parent, parentArgs);
			}
			finally {
				g.Restore(state);
			}
			return true;
		}
		public static bool PaintTransparentBackground(Control control, PaintEventArgs e, Rectangle bounds) {
			return PaintTransparentBackground(control, e, bounds, invokePaint, invokePaintBackground);
		}
		public static void PerformBackColorChanged(Control control) {
			backColorChanged(control, EventArgs.Empty);
		}
		#region Initialization
		static PaintInvoke invokePaint;
		static PaintInvoke invokePaintBackground;
		static BackColorChanged backColorChanged;
		static BackgroundPaintHelper() {
			invokePaint = BuildPaintInvoke(typeof(Control).GetMethod("InvokePaint", BindingFlags.Instance | BindingFlags.NonPublic));
			invokePaintBackground = BuildPaintInvoke(typeof(Control).GetMethod("InvokePaintBackground", BindingFlags.Instance | BindingFlags.NonPublic));
			backColorChanged = BuildBackColorChanged(typeof(Control).GetMethod("OnBackColorChanged", BindingFlags.Instance | BindingFlags.NonPublic));
		}
		static PaintInvoke BuildPaintInvoke(MethodInfo method) {
			var c = System.Linq.Expressions.Expression.Parameter(typeof(Control), "c");
			var e = System.Linq.Expressions.Expression.Parameter(typeof(PaintEventArgs), "e");
			return System.Linq.Expressions.Expression.Lambda<PaintInvoke>(
						System.Linq.Expressions.Expression.Call(c, method, c, e),
						c, e
					).Compile();
		}
		static BackColorChanged BuildBackColorChanged(MethodInfo method) {
			var target = System.Linq.Expressions.Expression.Parameter(typeof(Control), "control");
			var e = System.Linq.Expressions.Expression.Parameter(typeof(EventArgs), "e");
			return System.Linq.Expressions.Expression.Lambda<BackColorChanged>(
						System.Linq.Expressions.Expression.Call(target, method, e),
						target, e
					).Compile();
		}
		#endregion Initialization
	}
	public class DXPaintEventArgs : EventArgs {
		Rectangle clipRectangle;
		Graphics graphics;
		PaintEventArgs paintArgs;
		IntPtr windowHandle;
		internal bool IsNativeDC { get { return paintArgs is ControlPaintHelper.XPaintEventArgs; } }
		public DXPaintEventArgs(PaintEventArgs paintArgs, IntPtr windowHandle)
			: this(paintArgs) {
			this.windowHandle = windowHandle;
		}
		public DXPaintEventArgs(PaintEventArgs paintArgs) {
			this.paintArgs = paintArgs;
			ControlPaintHelper.XPaintEventArgs xp = paintArgs as ControlPaintHelper.XPaintEventArgs;
			if(xp != null) clipRegion = xp.clipRegion;
		}
		public DXPaintEventArgs(Graphics graphics) {
			this.graphics = graphics;
		}
		public DXPaintEventArgs(Graphics graphics, Rectangle clipRectangle)
			: this(graphics) {
			this.clipRectangle = clipRectangle;
		}
		public DXPaintEventArgs() {
		}
		public PaintEventArgs PaintArgs { get { return paintArgs; }	}
		public Graphics Graphics {
			get { return PaintArgs == null ? graphics : PaintArgs.Graphics; }
		}
		public Rectangle ClipRectangle {
			get { return PaintArgs == null ? clipRectangle : PaintArgs.ClipRectangle; }
		}
		internal IntPtr WindowHandle { get { return windowHandle; } }
		internal Rectangle[] clipRegion = null;
	}
	public class ResourceCache : IDisposable {
		Dictionary<Color, Brush> solidBrushes;
		Dictionary<PenKey, Pen> pens;
		Dictionary<GradientKey, Brush> gradientBrushes;
		Hashtable fonts;
		class ColorComparer : IEqualityComparer<Color> {
			public bool Equals(Color x, Color y) {
				return x == y;
			}
			public int GetHashCode(Color obj) {
				return obj.GetHashCode();
			}
		}
		public ResourceCache() {
			this.solidBrushes = new Dictionary<Color, Brush>(new ColorComparer());
			this.pens = new Dictionary<PenKey, Pen>(new PenKeyComparer());
		}
		[ThreadStatic]
		static ResourceCache defaultCache;
		public static ResourceCache DefaultCache {
			get { 
				if(defaultCache == null) defaultCache = new ResourceCache();
				return defaultCache;
			}
		}
		const int MaxCount = 1000;
		public virtual void CheckCache() {
			if(Pens.Count > MaxCount) ClearHashtable(Pens);
			if(SolidBrushes.Count > MaxCount) ClearHashtable(SolidBrushes);
			if(this.gradientBrushes != null && gradientBrushes.Count > MaxCount) ClearHashtable(this.gradientBrushes);
			if(this.fonts != null && fonts.Count > MaxCount) ClearHashtable(this.fonts);
		}
		public virtual void Dispose() {
			Clear();
		}
		public virtual void Clear() {
			ClearHashtable(SolidBrushes);
			ClearHashtable(Pens);
			ClearHashtable(this.gradientBrushes);
			ClearHashtable(this.fonts);
		}
		public void ClearPartial() {
			ClearHashtable(SolidBrushes);
			ClearHashtable(Pens);
			ClearHashtable(this.gradientBrushes);
		}
		public Font GetFont(Font font, FontStyle style) {
			if(font.Style == style) return font;
			return GetFont(font, font.Size, style);
		}
		public virtual Font GetFont(Font font, float size, FontStyle style) {
			if(font.Style == style && font.Size == size) return font;
			string key = GetFontKey(font, style, size);
			object res = Fonts[key];
			if(res == null) {
				font = new Font(font.FontFamily, size, style, font.Unit);
				Fonts[key] = font;
				return font;
			}
			return res as Font;
		}
		protected virtual string GetFontKey(Font font, FontStyle style, float size) {
			return string.Format("{0}: {1} {2} {3}", new object[] {font.FontFamily, size, font.Unit, style});
		}
		protected virtual Brush GetSystemBrush(Color color) {
			return SystemBrushes.FromSystemColor(color);
		}
		protected virtual Pen GetSystemPen(Color color) {
			return SystemPens.FromSystemColor(color);
		}
		public virtual Brush GetSolidBrush(Color color) {
			Brush brush = null;
			if(color.IsSystemColor) brush = GetSystemBrush(color);
			if(brush != null) return brush;
			if(SolidBrushes.TryGetValue(color, out brush))
				return brush;
			brush = new SolidBrush(color);
			SolidBrushes.Add(color, brush);
			return brush;
		}
		public virtual Pen GetPen(Color color) {
			return GetPen(color, 1);
		}
		public virtual Pen GetPen(Color color, int width) {
			Pen pen = null;
			if(width == 1 && color.IsSystemColor) pen = GetSystemPen(color);
			if(pen != null) return pen;
			PenKey penHash = GetPenHash(color, width);
			if(Pens.TryGetValue(penHash, out pen)) return pen;
			pen = new Pen(color, width);
			Pens.Add(penHash, pen);
			return pen;
		}
		public Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode) {
			rect.Inflate(1, 1);
			return GetGradientBrush(rect, startColor, endColor, mode, 1);
		}
		public virtual Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount) {
			GradientKey key = GetGradientHash(rect, startColor, endColor, mode, blendCount);
			Brush brush;
			if(GradientBrushes.TryGetValue(key, out brush))
				return brush;
			if(startColor == endColor || endColor == Color.Empty || rect.Width < 1 || rect.Height < 1) 
				brush = new SolidBrush(startColor);
			else
				brush = new LinearGradientBrush(rect, startColor, endColor, mode);
			GradientBrushes.Add(key, brush);
			return brush;
		}
		class PenKeyComparer : IEqualityComparer<PenKey> {
			public bool Equals(PenKey x, PenKey y) {
				return x.Color == y.Color && x.Width == y.Width;
			}
			public int GetHashCode(PenKey obj) {
				return obj.Color.GetHashCode() ^ obj.Width;
			}
		}
		protected struct PenKey {
			public Color Color;
			public int Width;
			public PenKey(Color color, int width) {
				this.Color = color;
				this.Width = width;
			}
		}
		protected PenKey GetPenHash(Color color, int width) {
			return new PenKey(color, width);
		}
		class GradientKeyComparer : IEqualityComparer<GradientKey> {
			public bool Equals(GradientKey x, GradientKey y) {
				return x.Mode == y.Mode && x.StartColor == y.StartColor && x.EndColor == y.EndColor && x.BlendCount == y.BlendCount && x.Rect == y.Rect;
			}
			public int GetHashCode(GradientKey obj) {
				return obj.Rect.GetHashCode() ^ (int)obj.Mode ^ obj.StartColor.GetHashCode() ^ obj.EndColor.GetHashCode() ^ obj.BlendCount;
			}
		}
		protected struct GradientKey {
			public Rectangle Rect;
			public Color StartColor;
			public Color EndColor;
			public LinearGradientMode Mode;
			public int BlendCount;
			public GradientKey(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount) {
				this.Rect = rect;
				this.StartColor = startColor;
				this.EndColor = endColor;
				this.Mode = mode;
				this.BlendCount = blendCount;
			}
		}
		protected GradientKey GetGradientHash(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount) {
			return new GradientKey(rect, startColor, endColor, mode, blendCount);
		}
		protected Dictionary<Color, Brush> SolidBrushes {
			get { return solidBrushes; } 
		}
		protected Dictionary<PenKey, Pen> Pens {
			get { return pens; } 
		}
		protected Dictionary<GradientKey, Brush> GradientBrushes {
			get {
				if(gradientBrushes == null)
					this.gradientBrushes = new Dictionary<GradientKey, Brush>(new GradientKeyComparer());
				return gradientBrushes; 
			} 
		}
		protected Hashtable Fonts { 
			get { 
				if(fonts == null) fonts = new Hashtable();
				return fonts; 
			} 
		}
		protected void ClearHashtable(IDictionary hash) {
			if(hash == null) return;
			foreach(IDisposable obj in hash.Values) {
				obj.Dispose();
			}
			hash.Clear();
		}
		public int ResourceCount {
			get {
				return SolidBrushes.Count + GradientBrushes.Count + Pens.Count;
			}
		}
	}
	public class GraphicsCache : IGraphicsCache {
		XPaint paint;
		DXPaintEventArgs paintArgs;
		GraphicsClip clipInfo;
		ResourceCache cache;
		Matrix matrix;
		Point offset, offsetEx;
		bool matrixReady;
		public GraphicsCache(Graphics g) : this(new DXPaintEventArgs(g)) { }
		public GraphicsCache(PaintEventArgs e, XPaint paint) : this(new DXPaintEventArgs(e), paint) { }
		public GraphicsCache(DXPaintEventArgs e, XPaint paint) {
			this.paint = paint;
			this.clipInfo = new GraphicsClip();
			SetPaintArgs(e);
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheCache")]
#endif
		public ResourceCache Cache {
			get {
				if(cache == null) return ResourceCache.DefaultCache;
				return cache;
			}
			set {
				cache = value;
			}
		}
		public GraphicsCache(PaintEventArgs e) : this(new DXPaintEventArgs(e)) { }
		public GraphicsCache(DXPaintEventArgs e) : this(e, XPaint.Graphics) { }
		public virtual void Dispose() {
			Clear();
			if(this.cache != null) cache.Dispose();
			this.cache = null;
			if(this.clipInfo != null) this.clipInfo.Dispose();
		}
		public virtual void Clear() {
			this.paintArgs = null;
			this.matrix = null;
			this.matrixReady = false;
			this.offset = Point.Empty;
			this.offsetEx = Point.Empty;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheTransformMatrix")]
#endif
		public virtual Matrix TransformMatrix { get { return matrix; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheOffset")]
#endif
		public virtual Point Offset {
			get {
				if(!IsMatrixReady) UpdateMatrix();
				return offset;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheOffsetEx")]
#endif
		public virtual Point OffsetEx {
			get {
				if(!IsMatrixReady) UpdateMatrix();
				return offsetEx;
			}
		}
		public virtual Rectangle CalcRectangle(Rectangle r) {
			r.Offset(Offset); 
			return r;
		}
		public virtual Rectangle CalcClipRectangle(Rectangle r) {
			r.Offset(Offset); 
#if DXWhidbey
			r.Offset(OffsetEx); 
#endif
			return r;
		}
		public void ResetMatrix() {
			this.matrixReady = false;
		}
		protected virtual bool IsMatrixReady { get { return matrixReady; } }
		[System.Security.SecuritySafeCritical]
		protected virtual void UpdateMatrix() {
			this.matrix = null;
			this.offset = Point.Empty;
			this.offsetEx = Point.Empty;
			if(Graphics != null) {
				this.matrix = Graphics.Transform;
				IntPtr hdc = Graphics.GetHdc();
				try {
					NativeMethods.POINT pt = new NativeMethods.POINT();
					bool res = NativeMethods.GetViewportOrgEx(hdc, ref pt);
					this.offsetEx = new Point(pt.X, pt.Y);
				} finally {
					Graphics.ReleaseHdc(hdc);
				}
			}
			if(TransformMatrix != null) {
				offset = new Point((int)TransformMatrix.OffsetX, (int)TransformMatrix.OffsetY);
			}
			this.matrixReady = true;
		}
		internal void SetGraphics(Graphics g) { SetPaintArgs(new DXPaintEventArgs(g, Rectangle.Empty)); }
		internal void SetPaintArgs(DXPaintEventArgs e) {
			this.matrixReady = false;
			this.paintArgs = e;
			if(this.paintArgs == null) return;
			this.clipInfo.ReleaseGraphics();
			this.clipInfo.MaximumBounds = e.ClipRectangle;
		}
		public bool IsNeedDrawRectEx(Rectangle r) {
			if(!IsNeedDrawRect(r)) return false;
			if(PaintArgs == null) return true;
			if(!PaintArgs.IsNativeDC) return true; 
			if(PaintArgs.clipRegion == null) PrepareClipRegion();
			for(int n = 0; n < PaintArgs.clipRegion.Length; n++) {
				if(PaintArgs.clipRegion[n].IntersectsWith(r)) return true;
			}
			return false;
		}
		public void PrepareClipRegion() {
			PaintArgs.clipRegion = new Rectangle[0];
			if(PaintArgs.WindowHandle == IntPtr.Zero) return;
			IntPtr hdc = PaintArgs.Graphics.GetHdc();
			try {
				Rectangle[] rects = NativeMethods.GetClipRectsFromHDC(PaintArgs.WindowHandle, hdc, PaintArgs.IsNativeDC);
				if(rects != null) PaintArgs.clipRegion = rects;
			}
			finally {
				PaintArgs.Graphics.ReleaseHdc(hdc);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowDrawInvisibleRect { get; set; }
		public bool IsNeedDrawRect(Rectangle r) {
			if(r.IsEmpty) return false;
			if(PaintArgs == null) return true;
			if(AllowDrawInvisibleRect) return true;
			if(PaintArgs.ClipRectangle.IsEmpty) return true;
			if(PaintArgs.ClipRectangle.IntersectsWith(r))
				return Graphics.IsVisible(CalcRectangle(r));
			return false;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheClipInfo")]
#endif
		public GraphicsClip ClipInfo {
			get {
				if(clipInfo.Graphics != Graphics) clipInfo.Initialize(this);
				return clipInfo;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCachePaint")]
#endif
		public XPaint Paint {
			get { return paint; }
			set {
				if(value == null) value = XPaint.Graphics;
				paint = value;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCachePaintArgs")]
#endif
		public DXPaintEventArgs PaintArgs {
			get { return paintArgs; }
			set {
				if(PaintArgs == value) return;
				SetPaintArgs(value);
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("GraphicsCacheGraphics")]
#endif
		public Graphics Graphics { get { return PaintArgs == null ? null : PaintArgs.Graphics; } }
		public Font GetFont(Font font, FontStyle fontStyle) { return Cache.GetFont(font, fontStyle); }
		public Brush GetSolidBrush(Color color) { return Cache.GetSolidBrush(color); }
		public Pen GetPen(Color color) { return Cache.GetPen(color); }
		public Pen GetPen(Color color, int width) { return Cache.GetPen(color, width); }
		public Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode) {
			return Cache.GetGradientBrush(rect, startColor, endColor, mode);
		}
		public Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount) {
			return Cache.GetGradientBrush(rect, startColor, endColor, mode, blendCount);
		}
		public void FillRectangle(Brush brush, Rectangle rect) {
			Paint.FillRectangle(Graphics, brush, rect);
		}
		public void FillRectangle(Brush brush, RectangleF rect) {
			Paint.FillRectangle(Graphics, brush, rect);
		}
		public void FillRectangle(Color color, Rectangle rect) {
			FillRectangle(GetSolidBrush(color), rect);
		}
		public void DrawVString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat, int angle) {
			Paint.DrawVString(this, text, font, foreBrush, bounds, strFormat, angle);
		}
		public void DrawString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat) {
			Paint.DrawString(this, text, font, foreBrush, bounds, strFormat);
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth) {
			return Paint.CalcTextSize(Graphics, text, font, strFormat, maxWidth);
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			return Paint.CalcTextSize(Graphics, text, font, strFormat, maxWidth, maxHeight);
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			return Paint.CalcTextSize(Graphics, text, font, strFormat, maxWidth, maxHeight, out isCropped);
		}
		public void DrawRectangle(Pen pen, Rectangle r) {
			Paint.DrawRectangle(Graphics, pen, r);
		}
	}
	public class GraphicsInfoArgs {
		GraphicsCache cache;
		Rectangle bounds;
		Graphics graphics;
		public GraphicsInfoArgs(GraphicsInfoArgs info, Rectangle bounds) {
			this.graphics = info.graphics;
			this.cache = info.cache;
			this.bounds = bounds;
		}
		public GraphicsInfoArgs(GraphicsCache cache, Rectangle bounds) {
			this.cache = cache;
			this.bounds = bounds;
		}
		public virtual XPaint Paint { get { return Cache.Paint; } }
		public virtual GraphicsCache Cache { 
			get { return cache; } 
			set { 
				cache = value; 
			}
		}
		public Rectangle CalcRectangle(Rectangle bounds) {
			if(Cache != null) return Cache.CalcRectangle(bounds);
			return bounds;
		}
		public DXPaintEventArgs PaintArgs {
			get { return Cache != null ? Cache.PaintArgs : null; }
		}
		public virtual Graphics Graphics { 
			get { return (Cache != null ? Cache.Graphics : graphics); } 
			set { 
				if(value != null) {
					if(Cache == null) 
						Cache = new GraphicsCache(value);
					else 
						Cache.SetGraphics(value);
				}
				graphics = value; 
			}
		}
		public virtual Rectangle Bounds { 
			get { return bounds; }
			set { bounds = value; }
		}
	}
	public class GraphicsInfo {
		[ThreadStatic]
		static bool invalidSharedGraphics = false;
		[ThreadStatic]
		static Graphics sharedGraphics;
		static Graphics SharedGraphics { 
			get {
				if(sharedGraphics != null && invalidSharedGraphics && CanCreateCorrectGraphics()) {
					sharedGraphics.Dispose();
					sharedGraphics = null;
				}
				if(sharedGraphics == null) {
					invalidSharedGraphics = !CanCreateCorrectGraphics();
					sharedGraphics = CreateEmptyGraphics(null, true);
				}
				return sharedGraphics;
			}
		}
		static bool CanCreateCorrectGraphics() {
			if(GetControlHandle(mainControl) != IntPtr.Zero || GetOpenedFormHandle() != IntPtr.Zero) return true;
			return false;
		}
		GraphicsCache cache;
		int lockGraphics;
		[ThreadStatic]
		static Control mainControl;
		public static void SetMainControl(Control value) {
			if(mainControl == value) return;
			mainControl = value;
			if(mainControl != null && mainControl.IsHandleCreated) {
				if(sharedGraphics != null) sharedGraphics.Dispose();
			}
		}
		[ThreadStatic]
		static GraphicsInfo defaultInfo;
		public static GraphicsInfo Default {
			get { 
				if(defaultInfo == null) defaultInfo = new GraphicsInfo();
				return defaultInfo;
			}
		}
		public void CreateCache() {
			if(this.cache != null) return;
			this.cache = new GraphicsCache((Graphics)null);
		}
		protected virtual int LockGraphics { 
			get { return lockGraphics; }
			set { lockGraphics = value; }
		}
		internal static Graphics CreateTempEmptyGraphics() { return CreateEmptyGraphics(mainControl, false); }
		static IntPtr GetControlHandle(Control control) {
			if(control == null || !control.IsHandleCreated || control.InvokeRequired) return IntPtr.Zero;
			return control.Handle;
		}
		static IntPtr GetOpenedFormHandle() {
			if(Application.OpenForms.Count == 0) return IntPtr.Zero;
			for(int n = 0; n < Application.OpenForms.Count; n++) {
				IntPtr h = IntPtr.Zero;
				try {
					h = GetControlHandle(Application.OpenForms[n]);
				}
				catch(ArgumentOutOfRangeException) { }
				if(h != IntPtr.Zero) return h;
			}
			return IntPtr.Zero;
		}
		internal static Graphics CreateEmptyGraphics(Control defaultControl, bool trackControl) {
			Graphics g = null;
			try {
				IntPtr h = GetControlHandle(defaultControl);
				if(h == IntPtr.Zero) h = GetOpenedFormHandle();
				g = Graphics.FromHwnd(h);
				if(h != IntPtr.Zero) {
					var c = Control.FromHandle(h);
					if(c != null && trackControl)  {
						EventHandler c1 = null;
						c1 = (s, e) => {
							if(sharedGraphics == g) sharedGraphics = null;
							((Control)s).HandleDestroyed -= c1;
							((Control)s).Disposed -= c1;
						};
						c.Disposed += c1;
						c.HandleDestroyed += c1;
					}
				}
			}
			catch {
			}
			return g;
		}
		public virtual GraphicsCache Cache { get { return cache; } }
		public virtual Graphics Graphics { get { return Cache == null ? null : Cache.Graphics; } }
		public virtual Graphics AddGraphics(Graphics g) { return AddGraphics(g, null); }
		public virtual Graphics AddGraphics(Graphics g, Control defaultControl) {
			if(LockGraphics ++ != 0) return Graphics;
			if(defaultControl != null && !defaultControl.IsHandleCreated) defaultControl = mainControl;
			if(defaultControl != null && !defaultControl.IsHandleCreated) defaultControl = null;
			if(g == null) g = SharedGraphics;
			g = g == null ? CreateEmptyGraphics(defaultControl, true) : g;
			if(this.cache == null) 
				this.cache = new GraphicsCache(g);
			else {
				this.cache.SetGraphics(g);
			}
			return Graphics;
		}
		public virtual void ReleaseGraphics() {
			if(--LockGraphics == 0) {
				if(cache != null) {
					cache.Clear();
				}
			}
		}
	}
	public delegate void OnPaintMethod(PaintEventArgs e);
	public class ControlPaintHelper {
		Control owner;
		OnPaintMethod onPaint;
		OnPaintMethod onPaintBackground;
		bool allPaintInWmPaint; bool opaque;
		public ControlPaintHelper(Control owner, OnPaintMethod onPaint, OnPaintMethod onPaintBackground, bool allPaintInWmPaint, bool opaque) {
			this.owner = owner;
			this.onPaintBackground = onPaintBackground;
			this.onPaint = onPaint;
			this.opaque = opaque;
			this.allPaintInWmPaint = allPaintInWmPaint;
		}
		public void ProcessWMPaint(ref Message m) {
			IntPtr handle = owner.Handle;
			NativeMethods.PAINTSTRUCT paintStruct = default(NativeMethods.PAINTSTRUCT);
			bool beginPaint = false;
			try {
				IntPtr hdc;
				Rectangle rectangle;
				if(m.WParam == IntPtr.Zero) {
					hdc = NativeMethods.BeginPaint(handle, ref paintStruct);
					beginPaint = true;
					rectangle = paintStruct.rcPaint.ToRectangle();
				}
				else {
					hdc = m.WParam;
					rectangle = owner.ClientRectangle;
				}
				if(rectangle.Width > 0 && rectangle.Height > 0) {
					Rectangle[] clipBounds = NativeMethods.GetClipRectsFromHDC(handle, hdc, true);
					IntPtr prevPalette = IntPtr.Zero;
					BufferedGraphics bufferedGraphics = null;
					XPaintEventArgs paintEventArgs = null;
					GraphicsState graphicsState = null;
					try {
						prevPalette = SetUpPalette(hdc, false, false);
						if(beginPaint) {
						try {
							bufferedGraphics = BufferedGraphicsManager.Current.Allocate(hdc, owner.ClientRectangle);
							bufferedGraphics.Graphics.SetClip(rectangle);
						}
						catch(Exception e) {
							if(IsCriticalException(e)) throw e;
						}
						}
						if(bufferedGraphics == null) {
							paintEventArgs = new XPaintEventArgs(hdc, rectangle, clipBounds);
						}
						else {
							paintEventArgs = new XPaintEventArgs(bufferedGraphics.Graphics, rectangle, clipBounds);
							graphicsState = paintEventArgs.Graphics.Save();
						}
						using(paintEventArgs) {
							try {
									this.PaintWithErrorHandling(paintEventArgs, 1);
							}
							finally {
								if(graphicsState != null) {
									paintEventArgs.Graphics.Restore(graphicsState);
								}
								else {
									paintEventArgs.RestoreGraphicsState();
								}
							}
							this.PaintWithErrorHandling(paintEventArgs, 2);
							if(bufferedGraphics != null) {
								bufferedGraphics.Render();
							}
						}
					}
					finally {
						if(prevPalette != IntPtr.Zero) {
							NativeMethods.SelectPalette(hdc, prevPalette, false);
						}
						if(bufferedGraphics != null) {
							bufferedGraphics.Dispose();
						}
					}
				}
			}
			finally {
				if(beginPaint) {
					NativeMethods.EndPaint(handle, ref paintStruct);
				}
			}
		}
		public static bool IsCriticalException(Exception ex) {
			return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException || ex is System.Threading.ThreadAbortException || ex is IndexOutOfRangeException || ex is AccessViolationException;
		}
		private void PaintWithErrorHandling(PaintEventArgs paintEventArgs, int layer) {
			if(layer == 1 && !opaque) {
				onPaintBackground(paintEventArgs);
			}
			if(layer == 2) onPaint(paintEventArgs);
		}
		internal static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
			IntPtr halftonePalette = Graphics.GetHalftonePalette();
			IntPtr intPtr = NativeMethods.SelectPalette(dc, halftonePalette, force);
			if(intPtr != IntPtr.Zero && realizePalette) {
				NativeMethods.RealizePalette(dc);
			}
			return intPtr;
		}
		internal class XPaintEventArgs : PaintEventArgs {
			internal Rectangle[] clipRegion;
			internal XPaintEventArgs(IntPtr hdc, Rectangle clipRect, Rectangle[] clipRegion) : base(GraphicsInfo.CreateTempEmptyGraphics(), clipRect) {
				FieldInfo fiGraphics = typeof(PaintEventArgs).GetField("graphics", BindingFlags.NonPublic | BindingFlags.Instance);
				FieldInfo fiDC = typeof(PaintEventArgs).GetField("dc", BindingFlags.NonPublic | BindingFlags.Instance);
				if(fiGraphics == null || fiDC == null) throw new Exception("DXERR2014");
				Graphics.Dispose();
				fiGraphics.SetValue(this, null);
				fiDC.SetValue(this, hdc);
			}
			public XPaintEventArgs(Graphics graphics, Rectangle clipRect, Rectangle[] clipRegion) : base(graphics, clipRect) {
				this.clipRegion = clipRegion;
			}
			internal void RestoreGraphicsState() {
				MethodInfo mi = typeof(PaintEventArgs).GetMethod("ResetGraphics", BindingFlags.NonPublic | BindingFlags.Instance);
				if(mi != null) mi.Invoke(this, null);
			}
		}
	}
	public static class DXControlPaint {
		public static void DrawGrid(Graphics graphics, Rectangle r, Size pixelsBetweenDots, Color color) {
			using(SolidBrush br = new SolidBrush(color)) {
				for(int i = pixelsBetweenDots.Width / 2; i < r.Width; i += pixelsBetweenDots.Width) {
					for(int j = pixelsBetweenDots.Height / 2; j < r.Height; j += pixelsBetweenDots.Height) {
						graphics.FillEllipse(br, i, j, 2, 2);
					}
				}
			}
		}
		public static void DrawDashedBorder(Graphics graphics, Control control, Color backColor) {
			Rectangle r = control.ClientRectangle;
			Color clr = backColor;
			using(Pen pen = new Pen(clr.GetBrightness() < .5 ? ControlPaint.Light(clr) : ControlPaint.Dark(clr))) {
				pen.DashStyle = DashStyle.Dash;
				r.Width--; r.Height--;
				graphics.DrawRectangle(pen, r);
			}
		}
	}
	public static class ImageMetadataHelper {
		static readonly int CommentsId = 0x0320;
		public static void SaveTags(Image image, string path, string tags) {
			SaveTags(image, CommentsId, path, tags);
		}
		public static void SaveTags(Image image, int id, string path, string tags) {
			PropertyItem item = CreatePropertyItem(id, tags);
			image.SetPropertyItem(item);
			using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write)) {
				image.Save(fs, ImageFormat.Png);
			}
		}
		public static string LoadTags(Image image) {
			return LoadTags(image, CommentsId);
		}
		public static string LoadTags(Image image, int id) {
			string tags = string.Empty;
			try {
				PropertyItem item = image.GetPropertyItem(id);
				tags = Encoding.ASCII.GetString(item.Value);
			}
			catch {
			}
			return tags;
		}
		public static PropertyItem CreatePropertyItem(int id, string data) {
			ConstructorInfo ctor = typeof(PropertyItem).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
			PropertyItem item = (PropertyItem)ctor.Invoke(null);
			item.Id = id;
			item.Value = Encoding.ASCII.GetBytes(data + '\0');
			item.Len = item.Value.Length;
			item.Type = 2;
			return item;
		}
	}
}
namespace DevExpress.Utils.Helpers {
	public static class PaintHelper {
		public static void DrawImage(Graphics g, Image image, Rectangle bounds, System.Windows.Forms.Padding sizeMargins) {
			Rectangle[] src = new Rectangle[] { 
				new Rectangle(0, 0, sizeMargins.Left, sizeMargins.Top),
				new Rectangle(sizeMargins.Left, 0, image.Width- sizeMargins.Horizontal, sizeMargins.Top),
				new Rectangle(image.Width - sizeMargins.Right, 0, sizeMargins.Right, sizeMargins.Top),
				new Rectangle(0, sizeMargins.Top, sizeMargins.Left, image.Height-sizeMargins.Vertical),
				new Rectangle(sizeMargins.Left, sizeMargins.Top, image.Width- sizeMargins.Horizontal, image.Height-sizeMargins.Vertical),
				new Rectangle(image.Width-sizeMargins.Right, sizeMargins.Top, sizeMargins.Right, image.Height-sizeMargins.Vertical),
				new Rectangle(0, image.Height-sizeMargins.Bottom, sizeMargins.Left, sizeMargins.Bottom),
				new Rectangle(sizeMargins.Left, image.Height-sizeMargins.Bottom, image.Width- sizeMargins.Horizontal, sizeMargins.Bottom),
				new Rectangle(image.Width - sizeMargins.Right, image.Height-sizeMargins.Bottom, sizeMargins.Right, sizeMargins.Bottom)
			};
			Rectangle[] dest = new Rectangle[] { 
				new Rectangle(bounds.Left, bounds.Top, sizeMargins.Left, sizeMargins.Top),
				new Rectangle(bounds.Left+sizeMargins.Left, bounds.Top, bounds.Width- sizeMargins.Horizontal, sizeMargins.Top),
				new Rectangle(bounds.Right-sizeMargins.Right, bounds.Top, sizeMargins.Right, sizeMargins.Top),
				new Rectangle(bounds.Left, bounds.Top+ sizeMargins.Top, sizeMargins.Left, bounds.Height-sizeMargins.Vertical),
				new Rectangle(bounds.Left+sizeMargins.Left, bounds.Top+ sizeMargins.Top, bounds.Width- sizeMargins.Horizontal, bounds.Height-sizeMargins.Vertical),
				new Rectangle(bounds.Right-sizeMargins.Right, bounds.Top+ sizeMargins.Top, sizeMargins.Right, bounds.Height-sizeMargins.Vertical),
				new Rectangle(bounds.Left, bounds.Bottom-sizeMargins.Bottom, sizeMargins.Left, sizeMargins.Bottom),
				new Rectangle(bounds.Left+ sizeMargins.Left, bounds.Bottom-sizeMargins.Bottom, bounds.Width- sizeMargins.Horizontal, sizeMargins.Bottom),
				new Rectangle(bounds.Right - sizeMargins.Right, bounds.Bottom-sizeMargins.Bottom, sizeMargins.Right, sizeMargins.Bottom)
			};
			for(int i = 0; i < src.Length; i++)
				g.DrawImage(image, dest[i], src[i], GraphicsUnit.Pixel);
		}
		public static void DrawSkinnedDivider(DevExpress.Utils.Drawing.GraphicsCache cache, DevExpress.Skins.SkinElementInfo info) {
			DevExpress.Utils.Drawing.ObjectPainter.DrawObject(cache, DevExpress.Skins.SkinElementPainter.Default, info);
		}
		public static void DrawDivider(DevExpress.Utils.Drawing.GraphicsCache cache, Rectangle rect) {
			cache.Graphics.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			cache.Graphics.DrawLine(SystemPens.ControlLightLight, rect.Left, rect.Top, rect.Right, rect.Top);
		}
	}
	public static class ColoredImageHelper {
		[ThreadStatic]
		static ImageAttributes disabledImageAttr;
		public static ImageAttributes DisabledImageAttr {
			get {
				if(disabledImageAttr == null) {
					float[][] array = new float[5][];
					array[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
					array[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0, 0 };
					array[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
					array[3] = new float[5] { 0, 0, 0, 1, 0 };
					array[4] = new float[5] { 0.38f, 0.38f, 0.38f, -0.38f, 1 };
					ColorMatrix grayMatrix = new ColorMatrix(array);
					disabledImageAttr = new ImageAttributes();
					disabledImageAttr.ClearColorKey();
					disabledImageAttr.SetColorMatrix(grayMatrix);
				}
				return disabledImageAttr;
			}
		}
		public static int Convert(int value, int color) {
			byte alpha = (byte)((value >> 0x18) & 0xffL);
			byte red = (byte)((color >> 0x10) & 0xffL);
			byte green = (byte)((color >> 0x08) & 0xffL);
			byte blue = (byte)(color & 0xffL);
			return (int)(((uint)((((red << 0x10) | (green << 8)) | blue) | (alpha << 0x18))) & 0xffffffffL);
		}
		[System.Security.SecuritySafeCritical]
		public static Image GetColoredImage(Image sourceImage, Color color) {
			if(sourceImage == null)
				return null;
			int w = sourceImage.Width; int h = sourceImage.Height;
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			bmp.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
			using(Graphics g = Graphics.FromImage(bmp)) {
				g.DrawImageUnscaled(sourceImage, 0, 0);
			}
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bmp.PixelFormat);
			int argbColor = color.ToArgb();
			try {
				int argb; int offset = 0;
				for(int i = 0; i < w * h; i++) {
					argb = System.Runtime.InteropServices.Marshal.ReadInt32(bmpData.Scan0, offset);
					argb = Convert(argb, argbColor);
					System.Runtime.InteropServices.Marshal.WriteInt32(bmpData.Scan0, offset, argb);
					offset += sizeof(int);
				}
			}
			finally { bmp.UnlockBits(bmpData); }
			return bmp;
		}
	}
}
