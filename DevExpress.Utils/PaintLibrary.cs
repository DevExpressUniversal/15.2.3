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
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Skins;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.Utils.Drawing {
	public enum TextGlyphDrawModeEnum { Text, TextGlyph, Glyph };
	public enum ImageLayoutMode { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight, Stretch, ZoomInside, ZoomOutside, StretchHorizontal, StretchVertical, Default, Squeeze }
	public enum ImageScaleMode { Clip, Stretch, ZoomInside, ZoomOutside, StretchHorizontal, StretchVertical, Squeeze }
	public enum ImageAlignmentMode { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight }
	[Flags]
	public enum ObjectState { Normal = 0, Hot = 1, Pressed = 2, Disabled = 4, Selected = 8};
	public class ObjectInfoArgs : GraphicsInfoArgs {
		ObjectState state;
		public ObjectInfoArgs() : this(null) { }
		public ObjectInfoArgs(GraphicsCache cache) : this(cache, Rectangle.Empty, ObjectState.Normal) { }
		public ObjectInfoArgs(GraphicsCache cache, Rectangle bounds, ObjectState state) : base(cache, bounds) {
			this.state = state;
		}
		public virtual void Assign(ObjectInfoArgs info) {
			this.state = info.state;
			this.Bounds = info.Bounds;
		}
		public virtual ObjectState State { 
			get { return state; } 
			set { state = value; }
		}
		public virtual void OffsetContent(int x, int y) {
			Rectangle r = Bounds;
			r.Offset(x, y);
			Bounds = r;
		}
		protected void OffsetRectangleRef(ref Rectangle bounds, int x, int y) {
			if(bounds.IsEmpty) return;
			bounds.Offset(x, y);
		}
		protected Rectangle OffsetRectangle(Rectangle bounds, int x, int y) {
			if(bounds.IsEmpty) return bounds;
			bounds.Offset(x, y);
			return bounds;
		}
	}
	public class StyleObjectInfoArgs : ObjectInfoArgs {
		AppearanceObject backAppearance, appearance;
		bool isDrawOnGlass;
		public StyleObjectInfoArgs() : this(null) { }
		public StyleObjectInfoArgs(GraphicsCache cache) : this(cache, Rectangle.Empty, null, ObjectState.Normal) { }
		public StyleObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance) : this(cache, bounds, appearance, null, ObjectState.Normal) { }
		public StyleObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState state) : this(cache, bounds, appearance, null, state) { }
		public StyleObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, AppearanceObject backAppearance, ObjectState state) : base(cache, bounds, state) {
			this.backAppearance = backAppearance;
			if(appearance == null) 
				this.appearance = (AppearanceObject)AppearanceObject.EmptyAppearance.Clone();
			else {
				this.appearance = appearance;
			}
		}
		public bool RightToLeft { get; set; }
		public void SetAppearance(AppearanceObject appearance) {
			this.appearance = appearance == null ? AppearanceObject.EmptyAppearance : appearance;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			StyleObjectInfoArgs styleInfo = info as StyleObjectInfoArgs;
			if(styleInfo == null) return;
			this.appearance = styleInfo.appearance;
			this.backAppearance = styleInfo.backAppearance;
			this.isDrawOnGlass = styleInfo.isDrawOnGlass;
		}
		public virtual AppearanceObject Appearance { 
			get { return appearance; } 
		}
		public virtual AppearanceObject BackAppearance { 
			get { 
				if(backAppearance == null) backAppearance = Appearance;
				return backAppearance; 
			} 
			set { backAppearance = value; } 
		}
		public bool IsDrawOnGlass {
			get { return isDrawOnGlass; }
			set { isDrawOnGlass = value; }
		}
	}
	public class OpacityProvider {
		static OpacityProvider() {
			defaultImageAttributes = GetImageAttributes(1.0f);
		}
		static float fOpacity = 0.0f;
		static ImageAttributes imageAttributes, defaultImageAttributes;
		static ImageAttributes GetImageAttributes(float opacity) {
			ImageAttributes attributes = new ImageAttributes();
			fOpacity = opacity;
			float[][] array = new float[5][];
			array[0] = new float[5] { 1.00f, 0.00f, 0.00f, 0.00f, 0 };
			array[1] = new float[5] { 0.00f, 1.00f, 0.00f, 0.00f, 0 };
			array[2] = new float[5] { 0.00f, 0.00f, 1.00f, 0.00f, 0 };
			array[3] = new float[5] { 0.00f, 0.00f, 0.00f, fOpacity, 0 };
			array[4] = new float[5] { 0.00f, 0.00f, 0.00f, 0.00f, 1.00f };
			ColorMatrix matrix = new ColorMatrix(array);
			attributes = new ImageAttributes();
			attributes.ClearColorKey();
			attributes.SetColorMatrix(matrix);
			return attributes;
		}
		public static ImageAttributes GetAttributes(float opacity) {
			if(opacity != fOpacity || imageAttributes == null)
				imageAttributes = GetImageAttributes(opacity);
			return imageAttributes;
		}
		public static ImageAttributes DefaultAttributes { get { return defaultImageAttributes; } }
	}
	public class ObjectPainter {
		static AppearanceDefault objectDefAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.Control);
		public virtual AppearanceDefault DefaultAppearance { get { return objectDefAppearance; } }
		Rectangle CalcDefaultMinBounds(ObjectInfoArgs info) {
			Graphics g = GraphicsInfo.Default.AddGraphics(null);
			try {
				return CalcObjectMinBounds(g, this, info);
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public Bitmap DrawElementIntoBitmap(ObjectInfoArgs info, ObjectState state) {
			ObjectState prev = info.State;
			try {
				info.State = state;
				return DrawElementInfoBitmap(info);
			}
			finally {
				info.State = prev;
			}
		}
		public Bitmap DrawElementInfoBitmap(ObjectInfoArgs info) {
			Rectangle minBounds = info.Bounds == Rectangle.Empty ? CalcDefaultMinBounds(info) : info.Bounds;
			if(minBounds.Width < 0 || minBounds.Height < 0) return null;
			info.Bounds = minBounds;
			Bitmap bitmap = new Bitmap(minBounds.Width, minBounds.Height);
			using(Graphics g = Graphics.FromImage(bitmap)) {
				g.Clear(Color.Transparent);
				CalcObjectBounds(g, this, info);
				using(GraphicsCache cache = new GraphicsCache(g)) {
					g.TranslateTransform(-minBounds.X, -minBounds.Y);
					DrawObject(cache, this, info);
				}
				g.Dispose();
			}
			return bitmap;
		}
		#region static helpers
		public static void DrawTextOnGlass(Graphics g, AppearanceObject obj, string text, Rectangle bounds) {
			DrawTextOnGlass(g, obj, text, bounds, null);
		}
		static TextFormatFlags GetTextFormatFlags(StringFormat sf) {
			TextFormatFlags f = TextFormatFlags.SingleLine;
			if(sf == null) f = TextFormatFlags.Default;
			else {
				if(sf.Alignment == StringAlignment.Center)
					f |= TextFormatFlags.HorizontalCenter;
				else if(sf.Alignment == StringAlignment.Near)
					f |= TextFormatFlags.Left;
				else f |= TextFormatFlags.Right;
				if(sf.LineAlignment == StringAlignment.Near)
					f |= TextFormatFlags.Bottom;
				else if(sf.LineAlignment == StringAlignment.Center)
					f |= TextFormatFlags.VerticalCenter;
				else
					f |= TextFormatFlags.Top;
				if(sf.HotkeyPrefix == System.Drawing.Text.HotkeyPrefix.Hide)
					f |= TextFormatFlags.HidePrefix;
				else if(sf.HotkeyPrefix == System.Drawing.Text.HotkeyPrefix.None)
					f |= TextFormatFlags.NoPrefix;
			}
			return f;
		}
		public static Size CalcTextSizeOnGlass(Graphics g, Font font, string text, Rectangle bounds, StringFormat sf) { 
			return NativeVista.CalcTextSizeOnGlass(g, text, font, bounds, GetTextFormatFlags(sf));
		}
		public static void DrawTextOnGlass(Graphics g, Font font, string text, Rectangle bounds, StringFormat sf, Color foreColor) {
			NativeVista.DrawTextOnGlass(g, text, font, bounds, foreColor, GetTextFormatFlags(sf));
		}
		public static void DrawTextOnGlass(Graphics g, AppearanceObject obj, string text, Rectangle bounds, StringFormat sf) {
			DrawTextOnGlass(g, obj.Font, text, bounds, sf, obj.ForeColor);
		}
		public static void DrawObject(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs e) {
			if(painter == null || e == null) return;
			GraphicsCache prev = e.Cache;
			e.Cache = cache;
			try {
				painter.DrawObject(e);
			}
			finally {
				e.Cache = prev;
			}
		}
		public static Rectangle CalcObjectMinBounds(Graphics graphics, ObjectPainter painter, ObjectInfoArgs e) {
			Graphics prev = e.Graphics;
			e.Graphics = graphics;
			Rectangle res = painter.CalcObjectMinBounds(e);
			e.Graphics = prev;
			return res;
		}
		public static Rectangle GetObjectClientRectangle(Graphics graphics, ObjectPainter painter, ObjectInfoArgs e) {
			Graphics prev = e.Graphics;
			e.Graphics = graphics;
			Rectangle res = painter.GetObjectClientRectangle(e);
			e.Graphics = prev;
			return res;
		}
		public static Rectangle CalcBoundsByClientRectangle(Graphics graphics, ObjectPainter painter, ObjectInfoArgs e, Rectangle client) {
			Graphics prev = e.Graphics;
			e.Graphics = graphics;
			Rectangle res = painter.CalcBoundsByClientRectangle(e, client);
			e.Graphics = prev;
			return res;
		}
		public static Rectangle CalcObjectBounds(Graphics graphics, ObjectPainter painter, ObjectInfoArgs e) {
			Graphics prev = e.Graphics;
			e.Graphics = graphics;
			Rectangle res = painter.CalcObjectBounds(e);
			e.Graphics = prev;
			return res;
		}
		#endregion
		public virtual void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
			e.Cache.DrawString(caption, font, brush, bounds, format);
		}
		public virtual Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public virtual Rectangle GetFocusRectangle(ObjectInfoArgs e) {
			return GetObjectClientRectangle(e);
		}
		public virtual Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		public Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e) { return CalcBoundsByClientRectangle(e, e.Bounds); }
		public virtual Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Empty;
		}
		public virtual Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		public virtual void DrawObject(ObjectInfoArgs e) {
		}
		protected virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject style, Rectangle bounds) {
			cache.Paint.FillRectangle(cache.Graphics, style.GetBackBrush(cache, bounds), bounds);
		}
		protected virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject style, Brush brush, Rectangle bounds) {
			cache.Paint.FillRectangle(cache.Graphics, brush, bounds);
		}
		protected virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject style, Color brushColor, Rectangle bounds) {
			cache.Paint.FillRectangle(cache.Graphics, cache.GetSolidBrush(brushColor), bounds);
		}
	}
	public class StyleObjectPainter : ObjectPainter {
		public virtual AppearanceObject GetStyle(ObjectInfoArgs e) {
			return (e as StyleObjectInfoArgs).Appearance;
		}
		protected Size CalcTextSize(ObjectInfoArgs e, string text, int width) {
			Size size = GetStyle(e).CalcTextSize(e.Graphics, text, width).ToSize();
			size.Height ++; size.Width ++;
			return size;
		}
	}
	public class EmptyObjectPainter : ObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds;	}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { return e.Bounds; }
	}
	public class BBrushes {
		public SolidBrush Light, LightLight, Dark, DarkDark;
		public BBrushes(GraphicsCache cache, Color lightColor, Color darkColor) {
			LightLight = Light = (SolidBrush)cache.GetSolidBrush(lightColor);
			DarkDark = Dark = (SolidBrush)cache.GetSolidBrush(darkColor);
		}
		public BBrushes(GraphicsCache cache, Color color) {
			Light = (SolidBrush)cache.GetSolidBrush(ControlPaint.Light(color, 0));
			LightLight = (SolidBrush)cache.GetSolidBrush(ControlPaint.LightLight(color));
			Dark = (SolidBrush)cache.GetSolidBrush(ControlPaint.Dark(color, 0));
			DarkDark = (SolidBrush)cache.GetSolidBrush(PPens.GetDarkDark(color));
		}
		public BBrushes(GraphicsCache cache, AppearanceObject style) : this(cache, style.BackColor) { }
	}
	public class PPens {
		public Pen Light, LightLight, Dark, DarkDark;
		public PPens(GraphicsCache cache, Color color) {
			Light = cache.GetPen(ControlPaint.Light(color, 0));
			LightLight = cache.GetPen(ControlPaint.LightLight(color));
			Dark = cache.GetPen(ControlPaint.Dark(color, 0));
			DarkDark = cache.GetPen(GetDarkDark(color));
		}
		public static Color GetDarkDark(Color color) {
			if(color == SystemColors.Control) return ControlPaint.DarkDark(color);
			return ControlPaint.Dark(color, 0.5f);
		}
	}
	public enum OverlappingMode { None, Force, Regular } ;
	public class DrawElementInfo {
		ObjectInfoArgs elementInfo;
		ObjectPainter elementPainter;
		StringAlignment alignment;
		bool requireTotalBounds;
		int elementInterval = 3;
		bool visible = true;
		OverlappingMode allowOverlapping = OverlappingMode.None;
		public DrawElementInfo(ObjectPainter elementPainter, ObjectInfoArgs elementInfo) : this(elementPainter, elementInfo, StringAlignment.Far) { }
		public DrawElementInfo(ObjectPainter elementPainter, ObjectInfoArgs elementInfo, StringAlignment alignment) {
			this.elementPainter = elementPainter;
			this.elementInfo = elementInfo;
			this.alignment = alignment;
		}
		public bool Visible { get { return visible; } set { visible = value; } }
		public OverlappingMode AllowOverlapping { get { return allowOverlapping; } set { allowOverlapping = value; } }
		public bool RequireTotalBounds { get { return requireTotalBounds; } set { requireTotalBounds = value; } }
		public int ElementInterval { get { return elementInterval; } set { elementInterval = value; } }
		public StringAlignment Alignment { get { return alignment; } set { alignment = value; } }
		public ObjectInfoArgs ElementInfo { get { return elementInfo; } set { elementInfo = value; } }
		public ObjectPainter ElementPainter { get { return elementPainter; } set { elementPainter = value; } }
	}
	public class GlyphElementInfoArgs : ObjectInfoArgs {
		Image glyph;
		object imageList;
		int imageIndex;
		public GlyphElementInfoArgs(object imageList, int imageIndex, Image glyph) {
			this.imageIndex = imageIndex;
			this.imageList = imageList;
			this.glyph = glyph;
		}
		public object ImageList { get { return imageList; } set { imageList = value; } }
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
		public Image Glyph { get { return glyph; } set { glyph = value; } }
		public Size GlyphSize { 
			get { 
				if(Glyph != null) return Glyph.Size;
				if(!ImageCollection.IsImageListImageExists(ImageList, ImageIndex)) return Size.Empty;
				return ImageCollection.GetImageListSize(ImageList);
			}
		}
	}
	public class SkinnedGlyphElementInfoArgs : GlyphElementInfoArgs, IParentAppearanceDependent {
		public ImageAttributes ImageAttributes {
			get { return ImageColorizer.GetColoredAttributes(ParentAppearance.GetForeColor()); }
		}
		public SkinnedGlyphElementInfoArgs(object imageList, int imageIndex, Image glyph)
			: base(imageList, imageIndex, glyph) {
		}
		public bool IsValid {
			get { return ParentAppearance != null; }
		}
		public AppearanceObject ParentAppearance { get; set; }
	}
	public class GlyphElementPainter : ObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds;	}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			GlyphElementInfoArgs ee = e as GlyphElementInfoArgs;
			return new Rectangle(Point.Empty, ee.GlyphSize);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GlyphElementInfoArgs ee = e as GlyphElementInfoArgs;
			if(ee.GlyphSize.IsEmpty) return;
			if(ee.Glyph != null)
				e.Cache.Paint.DrawImage(e.Graphics, ee.Glyph, e.Bounds, new Rectangle(Point.Empty, ee.GlyphSize), e.State != ObjectState.Disabled);
			else
				ImageCollection.DrawImageListImage(e, ee.ImageList, ee.ImageIndex, e.Bounds, e.State != ObjectState.Disabled);
		}
	}
	public class SkinnedGlyphElementPainter : GlyphElementPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			SkinnedGlyphElementInfoArgs ee = e as SkinnedGlyphElementInfoArgs;
			if(ee.IsValid) {
				if(ee.GlyphSize.IsEmpty) return;
				if(ee.Glyph != null)
					e.Cache.Paint.DrawImage(e.Graphics, ee.Glyph, e.Bounds, new Rectangle(Point.Empty, ee.GlyphSize), ee.ImageAttributes);
				else
					ImageCollection.DrawImageListImage(e.Cache, ee.ImageList, ee.ImageIndex, e.Bounds, ee.ImageAttributes);
			}
			else base.DrawObject(e);
		}
	}
	public class DrawElementInfoCollection : CollectionBase {
		public DrawElementInfo this[int index] { get { return List[index] as DrawElementInfo; } }
		public DrawElementInfo Find(Type type) {
			for(int n = 0; n < Count; n++) {
				DrawElementInfo info = this[n];
				if(info.ElementInfo.GetType().IsAssignableFrom(type)) return info;
			}
			return null;
		}
		public void UpdateRightToLeft(bool rightToLeft) {
			foreach(DrawElementInfo el in this) {
				StyleObjectInfoArgs si = el.ElementInfo as StyleObjectInfoArgs;
				if(si != null) si.RightToLeft = rightToLeft;
			}
		}
		public virtual int Add(DrawElementInfo info) { return List.Add(info); } 
		public virtual DrawElementInfo Add(ObjectPainter elementPainter, ObjectInfoArgs elementInfo) {
			DrawElementInfo info = new DrawElementInfo(elementPainter, elementInfo);
			Add(info);
			return info;
		}
		public virtual void Remove(DrawElementInfo info) {
			List.Remove(info);
		}
		public virtual Size CalcMinSize(Graphics graphics, ref bool canDrawMore) {
			Size size = Size.Empty;
			canDrawMore = true;
			for(int n = Count - 1; n >= 0; n--) {
				DrawElementInfo ei = this[n];
				if(!ei.Visible) continue;
				Size esize = ObjectPainter.CalcObjectMinBounds(graphics, ei.ElementPainter, ei.ElementInfo).Size;
				if(esize.IsEmpty) continue;
				size.Height = Math.Max(size.Height, esize.Height);
				size.Width += esize.Width;
				if(ei.Alignment == StringAlignment.Center) {
					canDrawMore = false;
					break;
				}
				size.Width += ei.ElementInterval;
			}
			return size;
		}
		protected virtual void ClearBounds() {
			for(int n = Count - 1; n >= 0; n--) {
				this[n].ElementInfo.Bounds = Rectangle.Empty;
			}
		}
		public virtual Rectangle CalcBounds(ObjectInfoArgs owner, GraphicsCache cache, Rectangle bounds, Rectangle totalBounds) {
			ClearBounds();
			bool rightToLeft = ((StyleObjectInfoArgs)owner).RightToLeft;
			for(int n = Count - 1; n >= 0; n--) {
				DrawElementInfo info = this[n];
				if(!info.Visible) continue;
				ISupportObjectInfo io = info.ElementInfo as ISupportObjectInfo;
				if(io != null) io.ParentObject = owner;
				info.ElementInfo.Cache = cache;
				info.ElementInfo.Bounds = Rectangle.Empty;
				Size elSize = info.ElementPainter.CalcObjectMinBounds(info.ElementInfo).Size;
				info.ElementInfo.Cache = null;
				if(elSize.IsEmpty) continue;
				if(bounds.Width < elSize.Width + (info.Alignment == StringAlignment.Center ? 0 : info.ElementInterval)) continue;
				Rectangle elRect = new Rectangle(bounds.X, bounds.Y + (bounds.Height - elSize.Height) / 2, elSize.Width, elSize.Height);
				switch(CalcAlignment(info.Alignment, rightToLeft)) {
					case StringAlignment.Center :
						elRect.X += (bounds.Width - elSize.Width) / 2;
						bounds.Width = 0;
						break;
					case StringAlignment.Far:
						elRect.X = bounds.Right - elRect.Width; 
						bounds.Width -= (elRect.Width + info.ElementInterval);
						break;
					case StringAlignment.Near :
						bounds.X += elRect.Width + info.ElementInterval;
						bounds.Width -= (elRect.Width + info.ElementInterval);
						break;
				}
				if(info.RequireTotalBounds) {
					info.ElementInfo.Bounds = totalBounds;
				} else {
					info.ElementInfo.Bounds = elRect;
				}
				ObjectPainter.CalcObjectBounds(cache == null ? null : cache.Graphics, info.ElementPainter, info.ElementInfo);
			}
			return bounds;
		}
		StringAlignment CalcAlignment(StringAlignment stringAlignment, bool rightToLeft) {
			if(!rightToLeft) return stringAlignment;
			switch(stringAlignment) {
				case StringAlignment.Far: return StringAlignment.Near;
				case StringAlignment.Near: return StringAlignment.Far;
			}
			return stringAlignment;
		}
		public virtual void DrawObjects(ObjectInfoArgs owner, GraphicsCache cache, Point offs) {
			for(int n = Count - 1; n >= 0; n--) {
				DrawElementInfo info = this[n];
				if(!info.Visible) continue;
				ISupportObjectInfo io = info.ElementInfo as ISupportObjectInfo;
				if(io != null) 
					io.ParentObject = owner;
				if(info.ElementInfo.Bounds.IsEmpty) continue;
				IParentAppearanceDependent pad = info.ElementInfo as IParentAppearanceDependent;
				if(pad != null && owner is StyleObjectInfoArgs)
					pad.ParentAppearance = ((StyleObjectInfoArgs)owner).Appearance;
				info.ElementInfo.Cache = cache;
				try {
					info.ElementInfo.OffsetContent(offs.X, offs.Y);
					info.ElementPainter.DrawObject(info.ElementInfo);
				}
				finally {
					info.ElementInfo.OffsetContent(-offs.X, -offs.Y);
				}
				info.ElementInfo.Cache = null;
			}
		}
		public virtual void SetBackStyle(AppearanceObject style) {
			foreach(DrawElementInfo info in this) {
				StyleObjectInfoArgs e = info.ElementInfo as StyleObjectInfoArgs;
				if(e != null) e.BackAppearance = style;
			}
		}
		public virtual void SetAppearance(AppearanceObject appearance) {
			foreach(DrawElementInfo info in this) {
				StyleObjectInfoArgs e = info.ElementInfo as StyleObjectInfoArgs;
				if(e != null) e.SetAppearance(appearance);
			}
		}
	}
	public class ButtonObjectPainter : StyleObjectPainter {
		public virtual Color GetBorderColor(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			Color color = appearance.GetBorderColor();
			return color.IsEmpty ? DefaultAppearance.BorderColor : color;
		}
		protected virtual BBrushes CreateBorderBrushes(ObjectInfoArgs e) {
			return new BBrushes(e.Cache, GetBorderColor(e));
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			if(IsPressed(e)) {
				r.X += 3;
				r.Width -= 4;
				r.Y += 3;
				r.Height -= 4;
			} else {
				r.Inflate(-2, -2);
			}
			return r;
		}
		protected virtual bool IsPressed(ObjectInfoArgs e) {
			return (e.State & ObjectState.Pressed) != 0;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(2, 2);
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 8, 8);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject style = GetStyle(e);
			bool drawn = false;
			if(IsPressed(e)) {
				DrawPressed(e, style);
				drawn = true;
			} 
			if(!drawn && (e.State & ObjectState.Hot) != 0) {
				DrawHot(e, style);
				drawn = true;
			}
			if(!drawn)
				DrawNormal(e, style);
			if((e.State & ObjectState.Selected) != 0) {
				DrawSelectedFrame(e);
			}
		}
		protected virtual void DrawSelectedFrame(ObjectInfoArgs e) {
			Brush brush = GetForeBrush(e);
			Rectangle r = e.Bounds;
			FlatButtonObjectPainter.DrawBounds(e, r, brush, brush);
		}
		public virtual Color GetForeColor(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) return SystemColors.GrayText;
			Color color = GetStyle(e).GetForeColor();
			if(color == Color.Empty) return DefaultAppearance.ForeColor;
			return color;
		}
		public Brush GetForeBrush(ObjectInfoArgs e) {
			return e.Cache.GetSolidBrush(GetForeColor(e));
		}
		protected virtual void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			DrawNormal(e, style);
		}
		protected virtual void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			Rectangle r = e.Bounds;
			BBrushes brushes = CreateBorderBrushes(e);
			DrawBounds(e, r, brushes.DarkDark, brushes.DarkDark);
			r.Inflate(-1, -1);
			DrawBounds(e, r, brushes.Light, brushes.Light);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
		protected virtual void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = e.Bounds;
			DrawBounds(e, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			DrawBounds(e, r, brushes.Light, brushes.Dark);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
		protected virtual void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			if(style.BackColor == Color.Empty) {
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(DefaultAppearance.BackColor), r);
			}
			else
				style.FillRectangle(e.Cache, r);
		}
		public static void DrawBounds(ObjectInfoArgs e, Rectangle r, Brush light, Brush dark) {
			e.Cache.Paint.FillRectangle(e.Graphics, light, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			e.Cache.Paint.FillRectangle(e.Graphics, light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			e.Cache.Paint.FillRectangle(e.Graphics, dark, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
		}
	}
	public class EmptyButtonObjectPainter : ButtonObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			Rectangle r = e.Bounds;
			BBrushes brushes = CreateBorderBrushes(e);
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, r);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			DrawButtonBackground(e, style, e.Bounds);
		}
	}
	public class SimpleButtonObjectPainter : ButtonObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			switch(e.State) {
				default:
					r.Inflate(-1, -1);
					break;
			}
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(1, 1);
			return r;
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			DrawPressed(e, e.Bounds, style);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			DrawNormal(e, e.Bounds, style);
		}
		protected virtual void DrawPressed(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			Rectangle r = rect;
			BBrushes brushes = CreateBorderBrushes(e);
			DrawBounds(e, r, brushes.DarkDark, brushes.DarkDark);
			r.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, r);
		}
		protected virtual void DrawNormal(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = rect;
			DrawBounds(e, r, brushes.DarkDark, brushes.DarkDark);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
	}
	public class ExpandButtonBackgroundFlatPainter : FlatButtonObjectPainter {
		public override AppearanceObject GetStyle(ObjectInfoArgs e) {
			return (e as BaseButtonInfo).PaintAppearance;
		}
		protected override void DrawSelectedFrame(ObjectInfoArgs e) { }
	} 
	public class FlatButtonObjectPainter : ButtonObjectPainter {
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			DrawPressed(e, e.Bounds, style);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			DrawNormal(e, e.Bounds, style);
		}
		protected virtual void DrawNormal(ObjectInfoArgs e, Rectangle r, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			DrawFlatBounds(e, r, brushes.LightLight, brushes.Dark);
			r.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.X, r.Y, 1, r.Height));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			r.X++;
			r.Y++;
			r.Width--;
			r.Height--;
			DrawButtonBackground(e, style, r);
		}
		protected virtual void DrawPressed(ObjectInfoArgs e, Rectangle r, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			DrawLines(e, r, brushes.DarkDark);
			r.X++; r.Y++; r.Width--; r.Height--;
			DrawLines(e, r, brushes.Dark);
			r.X++; r.Y++; r.Width--; r.Height--;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width--; r.Height--;
			DrawButtonBackground(e, style, r);
		}
		protected virtual bool AllowShiftDisabledCaption { get { return true; } }
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
			if(AllowShiftDisabledCaption && (e.State & ObjectState.Disabled) != 0) {
				Brush light = CreateBorderBrushes(e).LightLight;
				bounds.Offset(1, 1);
				base.DrawCaption(e, caption, font, light, bounds, format);
				bounds.Offset(-1, -1);
			}
			base.DrawCaption(e, caption, font, brush, bounds, format);
		}
		protected void DrawLines(ObjectInfoArgs e, Rectangle r, Brush brush) {
			e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
		}
		public static void DrawFlatBounds(ObjectInfoArgs e, Rectangle r, Brush light, Brush dark) {
			e.Cache.Paint.FillRectangle(e.Graphics, light, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			e.Cache.Paint.FillRectangle(e.Graphics, light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			e.Cache.Paint.FillRectangle(e.Graphics, dark, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
		}
	}
	public class WebFlatButtonObjectPainter : HotFlatButtonObjectPainter {
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			if(e.State == ObjectState.Normal || e.State == ObjectState.Disabled) return;
			base.DrawButtonBackground(e, style, r);
		}
		protected override BBrushes CreateBorderBrushes(ObjectInfoArgs e) {
			Color clr = GetBorderColor(e);
			return new BBrushes(e.Cache, clr, clr);
		}
		public override Color GetForeColor(ObjectInfoArgs e) {
			BBrushes brushes = base.CreateBorderBrushes(e);
			if(e.State == ObjectState.Disabled) return brushes.Dark.Color;
			if((e.State & (ObjectState.Pressed | ObjectState.Hot)) != 0) return brushes.LightLight.Color;
			return base.GetForeColor(e);
		}
	}
	public class WebIndicatorButtonObjectPainter : HotFlatButtonObjectPainter {
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			base.DrawButtonBackground(e, style, r);
		}
		protected override BBrushes CreateBorderBrushes(ObjectInfoArgs e) {
			Color clr = GetBorderColor(e);
			return new BBrushes(e.Cache, clr, clr);
		}
		public override Color GetForeColor(ObjectInfoArgs e) {
			BBrushes brushes = base.CreateBorderBrushes(e);
			if(e.State == ObjectState.Disabled) return brushes.Dark.Color;
			if((e.State & (ObjectState.Pressed | ObjectState.Hot)) != 0) return brushes.LightLight.Color;
			return base.GetForeColor(e);
		}
	}
	public class HotFlatButtonObjectPainter : ButtonObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			DrawPressed(e, e.Bounds, style);
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			DrawHot(e, e.Bounds, style);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			DrawNormal(e, e.Bounds, style);
		}
		protected virtual void DrawPressed(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = rect;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Dark, brushes.Dark);
			r.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.DarkDark, r);
		}
		protected virtual void DrawHot(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = rect;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Dark, r);
		}
		protected virtual void DrawNormal(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = rect;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Dark, brushes.Dark);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
		public override Color GetForeColor(ObjectInfoArgs e) {
			BBrushes brushes = CreateBorderBrushes(e);
			if(e.State == ObjectState.Disabled) return brushes.Dark.Color;
			if((e.State & (ObjectState.Pressed | ObjectState.Hot)) != 0) return brushes.LightLight.Color;
			return base.GetForeColor(e);
		}
	}
	public class Style3DButtonObjectPainter : ButtonObjectPainter {
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
			if((e.State & ObjectState.Disabled) != 0) {
				Brush light = CreateBorderBrushes(e).LightLight;
				bounds.Offset(1, 1);
				base.DrawCaption(e, caption, font, light, bounds, format);
				bounds.Offset(-1, -1);
			}
			base.DrawCaption(e, caption, font, brush, bounds, format);
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			DrawPressed(e, e.Bounds, style);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			DrawNormal(e, e.Bounds, style);
		}
		protected virtual void DrawNormal(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = rect;
			ButtonObjectPainter.DrawBounds(e, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			ButtonObjectPainter.DrawBounds(e, r, brushes.Light, brushes.Dark);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
		protected virtual void DrawPressed(ObjectInfoArgs e, Rectangle rect, AppearanceObject style) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = rect;
			ButtonObjectPainter.DrawBounds(e, r, brushes.Dark, brushes.LightLight);
			r.Inflate(-1, -1);
			ButtonObjectPainter.DrawBounds(e, r, brushes.DarkDark, brushes.Light);
			r.Inflate(-1, -1);
			DrawButtonBackground(e, style, r);
		}
	}
	public class Office1FlatButtonObjectPainter : FlatButtonObjectPainter {
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = e.Bounds;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			r.X ++;
			r.Y ++;
			r.Width --;
			r.Height --;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.X, r.Y, 1, r.Height - 2));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.DarkDark, new Rectangle(r.X - 1, r.Bottom - 1, r.Width, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.DarkDark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.X ++;
			r.Y ++;
			r.Width --;
			r.Height --;
			r.Width --;
			r.Height --;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Dark, new Rectangle(r.X - 1, r.Bottom - 1, r.Width, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width --;
			r.Height --;
			DrawButtonBackground(e, style, r);
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			Rectangle r = e.Bounds;
			BBrushes brushes = CreateBorderBrushes(e);
			DrawLines(e, r, brushes.DarkDark);
			r.X ++;	r.Y ++;	r.Width --; r.Height --;
			DrawLines(e, r, brushes.Dark);
			r.X ++;	r.Y ++;	r.Width --; r.Height --;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Light, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width --; r.Height --;
			DrawButtonBackground(e, style, r);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			BBrushes brushes = CreateBorderBrushes(e);
			Rectangle r = e.Bounds;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.LightLight, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			r.X ++;
			r.Y ++;
			r.Width --;
			r.Height --;
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Dark, new Rectangle(r.X - 1, r.Bottom - 1, r.Width, 1));
			e.Cache.Paint.FillRectangle(e.Graphics, brushes.Dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width --;
			r.Height --;
			DrawButtonBackground(e, style, r);
		}
	}
	public class Office2003ButtonObjectPainter : Office2003ButtonInBarsObjectPainter {
		protected override void DrawNormalBorder(StyleObjectInfoArgs e) {
			Brush borderPen = e.Cache.GetSolidBrush(GetHotBorderColor(e, false));
			e.Cache.Paint.DrawRectangle(e.Graphics, borderPen, e.Bounds);
		}
	}
	public class Office2003ButtonInBarsObjectPainter : UltraFlatButtonObjectPainter {
		protected override Brush GetHotBackBrush(ObjectInfoArgs e, bool pressed) {
			Color c1, c2;
			c1 = pressed ? Office2003Colors.Default[Office2003Color.Button1Pressed] : Office2003Colors.Default[Office2003Color.Button1Hot];
			c2 = pressed ? Office2003Colors.Default[Office2003Color.Button2Pressed] : Office2003Colors.Default[Office2003Color.Button2Hot];
			AppearanceObject obj = GetStyle(e);
			if(obj.GetBackColor() != Color.Empty && obj.GetBackColor2() != Color.Empty) {
				c1 = obj.GetBackColor2();
				c2 = obj.GetBackColor();
				if(pressed) {
					c1 = ControlPaint.Dark(c1);
				}
			}
			return e.Cache.GetGradientBrush(e.Bounds, c1, c2, LinearGradientMode.Vertical);
		}
		public override Color GetForeColor(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) {
				return Office2003Colors.Default[Office2003Color.TextDisabled];
			}
			Color color = GetStyle(e).ForeColor;
			if(color == Color.Empty || color == SystemColors.ControlText) color = Office2003Colors.Default[Office2003Color.Text];
			return color;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		protected override Color GetHotBorderColor(ObjectInfoArgs e, bool pressed) {
			Color clr = GetStyle(e).GetBorderColor();
			if(clr == SystemColors.Control) clr = Color.Empty;
			return clr != Color.Empty ? clr : Office2003Colors.Default[Office2003Color.Border];
		}
		protected override Brush GetNormalBackBrush(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) {
				return e.Cache.GetSolidBrush(SystemColors.Control);
			}
			Color c1, c2;
			c1 = Office2003Colors.Default[Office2003Color.Button1];
			c2 = Office2003Colors.Default[Office2003Color.Button2];
			AppearanceObject obj = GetStyle(e);
			if(obj.GetBackColor() != Color.Empty && obj.GetBackColor2() != Color.Empty) {
				c1 = obj.GetBackColor();
				c2 = obj.GetBackColor2();
			}
			return e.Cache.GetGradientBrush(e.Bounds, c1, c2, LinearGradientMode.Vertical);
		}
	}
	public class UltraFlatButtonObjectPainter : FlatButtonObjectPainter {
		protected override bool AllowShiftDisabledCaption { get { return false; } }
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			DrawHot(e, style, true);
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			DrawHot(e, style, false);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			Rectangle r = e.Bounds;
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			DrawNormalBorder(ee);
			r.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(e.Graphics, GetNormalBackBrush(e), r);
		}
		protected virtual void DrawNormalBorder(StyleObjectInfoArgs e) {
			Brush borderPen = e.BackAppearance.GetBackBrush(e.Cache);
			e.Cache.Paint.DrawRectangle(e.Graphics, borderPen, e.Bounds);
		}
		protected void DrawHot(ObjectInfoArgs e, AppearanceObject style, bool pressed) {
			Rectangle r = e.Bounds;
			DrawHot(e, r, style, pressed); 
		}
		protected void DrawHot(ObjectInfoArgs e, Rectangle rect, AppearanceObject style, bool pressed) {
			Rectangle r = rect;
			Brush brush;
			Rectangle buttonBack = r;
			buttonBack.Inflate(-1, -1);
			if(e.State == ObjectState.Selected) pressed = false;
			Brush borderPen = e.Cache.GetSolidBrush(GetHotBorderColor(e, pressed));
			brush = GetHotBackBrush(e, pressed);
			e.Cache.Paint.DrawRectangle(e.Graphics, borderPen, r);
			e.Graphics.FillRectangle(brush, buttonBack);
		}
		protected virtual Brush GetNormalBackBrush(ObjectInfoArgs e) {
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			return e.Cache.GetSolidBrush(ee.Appearance.BackColor == Color.Empty ? SystemColors.Control : ee.Appearance.BackColor);
		}
		protected virtual Brush GetHotBackBrush(ObjectInfoArgs e, bool pressed) {
			Color color = (!pressed ? DevExpress.Utils.ColorUtils.FlatBarItemHighLightBackColor : DevExpress.Utils.ColorUtils.FlatBarItemPressedBackColor);
			return e.Cache.GetSolidBrush(color);
		}
		protected virtual Color GetHotBorderColor(ObjectInfoArgs e, bool pressed) {
			return SystemColors.Highlight;
		}
	}
	public class BitmapRotate {
		[ThreadStatic]
		static Bitmap bufferBitmap;
		[ThreadStatic]
		static GraphicsCache bufferCache;
		public static Graphics BufferGraphics { get { return bufferCache.Graphics; } }
		public static GraphicsCache BufferCache { get { return bufferCache; } }
		public static Bitmap BufferBitmap { get { return bufferBitmap; } }
		public static Bitmap CreateBufferBitmap(Size size, bool alwaysCreate) {
			alwaysCreate = true;
			if(bufferBitmap != null && (alwaysCreate || bufferBitmap.Width < size.Width || bufferBitmap.Height < size.Height)) {
				if(!alwaysCreate) {
					size.Width = Math.Max(size.Width, bufferBitmap.Width);
					size.Height = Math.Max(size.Height, bufferBitmap.Height);
				}
				bufferBitmap.Dispose();
				bufferBitmap = null;
				if(bufferCache != null) bufferCache.Dispose();
				bufferCache = null;
			}
			if(bufferBitmap == null) {
				bufferBitmap = new Bitmap(size.Width, size.Height);
				bufferCache = new GraphicsCache(Graphics.FromImage(bufferBitmap));
			}
			return bufferBitmap;
		}
		public static void PrepareBitmap(GraphicsCache cache, Rectangle bounds, AppearanceObject backStyle) {
			BufferGraphics.FillRectangle(backStyle == null ? SystemBrushes.Control : backStyle.GetBackBrush(cache), bounds);
		}
		public static Bitmap RotateBitmap(RotateFlipType rotate) {
			lock(bufferBitmap) {
				if(rotate != RotateFlipType.RotateNoneFlipNone) {
					bufferBitmap.RotateFlip(rotate);
				}
			}
			return bufferBitmap;
		}
		internal static RotateFlipType GetMirroredRotateFlipType(RotateFlipType flip) {
			switch(flip) {
				case RotateFlipType.Rotate180FlipNone:
				case RotateFlipType.Rotate180FlipX:
				case RotateFlipType.Rotate180FlipXY:
				case RotateFlipType.Rotate180FlipY:
					return RotateFlipType.Rotate180FlipNone;
				case RotateFlipType.Rotate270FlipNone:
				case RotateFlipType.Rotate270FlipX:
				case RotateFlipType.Rotate270FlipXY:
				case RotateFlipType.Rotate270FlipY:
					return RotateFlipType.Rotate90FlipNone;
			}
			return RotateFlipType.RotateNoneFlipNone;
		}
		public static Bitmap RestoreBitmap(RotateFlipType rotate) {
			rotate = GetMirroredRotateFlipType(rotate);
			return RotateBitmap(rotate);
		}
		public static void ClearGraphics(Color color) {
			BufferGraphics.Clear(color);
		}
	}
	public enum SizeGripPosition { LeftTop, RightTop, LeftBottom, RightBottom};
	public class SizeGripObjectInfoArgs : StyleObjectInfoArgs {
		SizeGripPosition gripPosition;
		public SizeGripObjectInfoArgs(Rectangle bounds) : this() {	
			Bounds = bounds;
		}
		public SizeGripObjectInfoArgs() : this(SizeGripPosition.RightBottom, null) {	}
		public SizeGripObjectInfoArgs(SizeGripPosition gripPosition, AppearanceObject style) {
			this.gripPosition = gripPosition;
			SetAppearance(style);
		}
		public SizeGripPosition GripPosition { get { return gripPosition; } set { gripPosition = value; } }
	}
	public class SizeGripObjectPainter : ObjectPainter {
		public virtual Cursor CalcGripCursor(SizeGripPosition grip) {
			switch(grip) {
				case SizeGripPosition.LeftBottom : return Cursors.SizeNESW;
				case SizeGripPosition.RightTop : return Cursors.SizeNESW;
				case SizeGripPosition.LeftTop: return Cursors.SizeNWSE;
			}
			return Cursors.SizeNWSE;
		}
		public virtual int CalcGripHitTest(ObjectInfoArgs e) {
			int HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;
			SizeGripObjectInfoArgs ee = e as SizeGripObjectInfoArgs;
			switch(ee.GripPosition) {
				case SizeGripPosition.LeftBottom : return HTBOTTOMLEFT;
				case SizeGripPosition.RightTop : return HTTOPRIGHT;
				case SizeGripPosition.LeftTop: return HTTOPLEFT;
			}
			return HTBOTTOMRIGHT;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Size size = new Size(16, 16);
			return new Rectangle(Point.Empty, size);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SizeGripObjectInfoArgs ee = e as SizeGripObjectInfoArgs;
			Color backColor = ee.Appearance == null ? SystemColors.Control : ee.Appearance.BackColor;
			Bitmap bmp = BitmapRotate.CreateBufferBitmap(e.Bounds.Size, true);
			BitmapRotate.PrepareBitmap(e.Cache, new Rectangle(Point.Empty, e.Bounds.Size), ee.Appearance);
			DrawGrip(ee, backColor, BitmapRotate.BufferGraphics, new Rectangle(Point.Empty, e.Bounds.Size));
			RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
			switch(ee.GripPosition) {
				case SizeGripPosition.LeftBottom :
					rotate = RotateFlipType.Rotate90FlipNone;
					break;
				case SizeGripPosition.LeftTop:
					rotate = RotateFlipType.Rotate180FlipNone;
					break;
				case SizeGripPosition.RightTop:
					rotate = RotateFlipType.Rotate270FlipNone;
					break;
			}
			BitmapRotate.RotateBitmap(rotate);
			e.Paint.DrawImage(e.Graphics, bmp, e.Bounds, new Rectangle(Point.Empty, e.Bounds.Size), true);
		}
		protected virtual void DrawGrip(SizeGripObjectInfoArgs ee, Color backColor, Graphics g, Rectangle bounds) {
			ControlPaint.DrawSizeGrip(g, backColor, bounds);
		}
	}
	public class BorderObjectInfoArgs : StyleObjectInfoArgs {
		static AppearanceObject style;
		static BorderObjectInfoArgs() {
			CreateStyle();
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferencesChanged);
		}
		static void OnUserPreferencesChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			CreateStyle();
		}
		static void CreateStyle() {
			style = new AppearanceObject();
		}
		public BorderObjectInfoArgs() : this(null) { }
		public BorderObjectInfoArgs(GraphicsCache cache) : this(cache, null, Rectangle.Empty) { }
		public BorderObjectInfoArgs(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) : this(cache, appearance, bounds, ObjectState.Normal) {
		}
		public BorderObjectInfoArgs(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds, ObjectState state) : base(cache, bounds, null, state) { 
			if(appearance == null) 
				SetAppearance(BorderObjectInfoArgs.style);
			else {
				this.BackAppearance = appearance;
				SetAppearance(appearance);
			}
		}
		public BorderObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance) : this(cache, bounds, appearance, ObjectState.Normal) { }
		public BorderObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState state) : this(cache, appearance, bounds, state) { }
	}
	public class BorderPainter : StyleObjectPainter {
		public virtual Color GetBorderColor(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			Color color = appearance.GetBorderColor();
			return color.IsEmpty ? DefaultAppearance.BorderColor : color;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			e.Cache.Paint.DrawRectangle(e.Graphics, e.Cache.GetSolidBrush(GetBorderColor(e)), e.Bounds);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(1, 1);
			return r;
		}
	}
	public class EmptyBorderPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds; }
		public override void DrawObject(ObjectInfoArgs e) { }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; }
	}
	public class SimpleBorderPainter : BorderPainter {
		static AppearanceDefault simpleBorderDefAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark);
		public override AppearanceDefault DefaultAppearance { get { return simpleBorderDefAppearance; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(1, 1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			e.Cache.Paint.DrawRectangle(e.Graphics, e.Cache.GetSolidBrush(GetBorderColor(e)), r);
		}
	}
	public class FlatBorderPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Light, brushes.Dark);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(1, 1);
			return r;
		}
	}
	public class HotFlatBorderPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Dark, brushes.Dark);
			r.Inflate(-1, -1);
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Light, brushes.Light);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(2, 2);
			return r;
		}
	}
	public class TextFlatBorderPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Dark, brushes.LightLight);
			r.Inflate(-1, -1);
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Light, brushes.Light);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(2, 2);
			return r;
		}
	}
	public class FlatSunkenBorderPainter : BorderPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			FlatButtonObjectPainter.DrawFlatBounds(e, r, brushes.Dark, brushes.Light);
		}
	}
	public class Office1BorderPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			AppearanceObject vs = GetStyle(e);
			Brush dark = e.Cache.GetSolidBrush(ControlPaint.Dark(vs.BackColor));
			Brush light = e.Cache.GetSolidBrush(ControlPaint.LightLight(vs.BackColor));
			e.Graphics.FillRectangle(dark, new Rectangle(r.X, r.Y, r.Width - 1, 1));
			e.Graphics.FillRectangle(dark, new Rectangle(r.X, r.Y + 1, 1, r.Height - 2));
			e.Graphics.FillRectangle(light, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
			e.Graphics.FillRectangle(light, new Rectangle(r.Right - 1, r.Y, 1, r.Height - 1));
		}
	}
	public class Office2003BorderPainter : UltraFlatBorderPainter {
		public Office2003BorderPainter(bool alwaysDrawSelected) : base(alwaysDrawSelected) { }
		public Office2003BorderPainter() { }
		protected override void DrawSelected(ObjectInfoArgs e) {
			Brush brush = e.Cache.GetSolidBrush(Office2003Colors.Default[Office2003Color.Border]);
			e.Cache.Paint.DrawRectangle(e.Graphics, brush, e.Bounds);
		}
	}
	public class Office2003BorderPainterEx : Office2003BorderPainter {
		static Office2003BorderPainterEx def;
		public static Office2003BorderPainterEx Default {
			get { 
				if(def == null) def = new Office2003BorderPainterEx();
				return def;
			}
		}
		protected override void DrawSimple(ObjectInfoArgs e) {
			if(e.State == ObjectState.Normal) {
				Brush brush = e.Cache.GetSolidBrush(Office2003Colors.Default[Office2003Color.Button2]);
				e.Cache.Paint.DrawRectangle(e.Graphics, brush, e.Bounds);
				return;
			}
			base.DrawSimple(e);
		}
	}
	public class UltraFlatBorderPainter : BorderPainter {
		bool alwaysDrawSelected;
		public UltraFlatBorderPainter(bool alwaysDrawSelected) {
			this.alwaysDrawSelected = alwaysDrawSelected;
		}
		public UltraFlatBorderPainter() { }
		protected virtual bool AlwaysDrawSelected { get { return alwaysDrawSelected; } }
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) {
				DrawDisabled(e);
				return;
			}
			if(!AlwaysDrawSelected && (e.State == ObjectState.Normal || e.State == ObjectState.Disabled))
				DrawSimple(e);
			else
				DrawSelected(e);
		}
		protected virtual void DrawDisabled(ObjectInfoArgs e) {
			BorderObjectInfoArgs ee = e as BorderObjectInfoArgs;
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			FlatButtonObjectPainter.DrawFlatBounds(e, e.Bounds, brushes.Dark, brushes.Dark);
		}
		protected virtual void DrawSimple(ObjectInfoArgs e) {
			BorderObjectInfoArgs ee = e as BorderObjectInfoArgs;
			Brush pen = ee.BackAppearance.GetBackBrush(e.Cache);
			e.Cache.Paint.DrawRectangle(e.Graphics, pen, e.Bounds);
		}
		protected virtual void DrawSelected(ObjectInfoArgs e) {
			Brush pen = e.Cache.GetSolidBrush(SystemColors.Highlight);
			e.Cache.Paint.DrawRectangle(e.Graphics, pen, e.Bounds);
		}
	}
	public class RotatedBorderSidePainter : BorderSidePainter {
		protected virtual int GetDegree(ObjectInfoArgs e) { return 0; }
		protected override BorderSide GetBorderSides(ObjectInfoArgs e) {
			int degree = GetDegree(e);
			switch(degree) {
				case 90 : return BorderSide.Top | BorderSide.Bottom | BorderSide.Right;
				case 180 : return BorderSide.Left | BorderSide.Bottom | BorderSide.Right;
				case 270 : return BorderSide.Left | BorderSide.Bottom | BorderSide.Top;
			}
			return BorderSide.Left | BorderSide.Right | BorderSide.Top;
		}
	}
	public class BorderSidePainter : BorderPainter {
		protected virtual int GetBorderWidth(BorderSide side) {
			return 1;
		}
		protected virtual BorderSide GetBorderSides(ObjectInfoArgs e) {
			return BorderSide.None;
		}
		protected Rectangle GetRectangle(Rectangle bounds, BorderSide sides, bool client) {
			Rectangle res = bounds;
			if(sides == BorderSide.None) return res;
			int d = client ? 1 : -1;
			int left = (sides & BorderSide.Left) != 0 ? GetBorderWidth(BorderSide.Left) : 0, right = (sides & BorderSide.Right) != 0 ? GetBorderWidth(BorderSide.Right)  : 0,
				top = (sides & BorderSide.Top) != 0 ? GetBorderWidth(BorderSide.Top)  : 0, bottom = (sides & BorderSide.Bottom) != 0 ? GetBorderWidth(BorderSide.Bottom) : 0;
			res.X += left * d;
			res.Width -= (left + right) * d;
			res.Height -= (top + bottom) * d;
			res.Y += top * d;
			return res;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return GetRectangle(e.Bounds, GetBorderSides(e), true);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return GetRectangle(client, GetBorderSides(e), false);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BorderSide sides = GetBorderSides(e);
			int left = (sides & BorderSide.Left) != 0 ? GetBorderWidth(BorderSide.Left) : 0, right = (sides & BorderSide.Right) != 0 ? GetBorderWidth(BorderSide.Right)  : 0,
				top = (sides & BorderSide.Top) != 0 ? GetBorderWidth(BorderSide.Top)  : 0, bottom = (sides & BorderSide.Bottom) != 0 ? GetBorderWidth(BorderSide.Bottom) : 0;
			Rectangle r = e.Bounds;
			if(left != 0) DrawSide(e, BorderSide.Left, new Rectangle(r.X, r.Y, left, r.Height));
			if(right != 0) DrawSide(e, BorderSide.Right, new Rectangle(r.Right - right, r.Y, right, r.Height));
			if(top != 0) DrawSide(e, BorderSide.Top, new Rectangle(r.X, r.Y, r.Width, top));
			if(bottom != 0) DrawSide(e, BorderSide.Bottom, new Rectangle(r.X, r.Bottom - bottom, r.Width, bottom));
		}
		protected virtual void DrawSide(ObjectInfoArgs e, BorderSide side, Rectangle bounds) {
			e.Cache.Paint.FillRectangle(e.Graphics, GetBrush(e, side), bounds);
		}
		protected virtual Brush GetBrush(ObjectInfoArgs e, BorderSide side) {
			return GetStyle(e).GetBorderBrush(e.Cache);;
		}
	}
	public class Border3DRaisedPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			ButtonObjectPainter.DrawBounds(e, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			ButtonObjectPainter.DrawBounds(e, r, brushes.Light, brushes.Dark);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(2, 2);
			return r;
		}
	}
	public class Border3DSunkenPainter : BorderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BBrushes brushes = new BBrushes(e.Cache, GetBorderColor(e));
			Rectangle r = e.Bounds;
			ButtonObjectPainter.DrawBounds(e, r, brushes.Dark, brushes.LightLight);
			r.Inflate(-1, -1);
			ButtonObjectPainter.DrawBounds(e, r, brushes.DarkDark, brushes.Light);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(2, 2);
			return r;
		}
	}
	public enum TextOrientation { Default, Horizontal, VerticalUpwards, VerticalDownwards }
	public class ProgressBarObjectInfoArgs : StyleObjectInfoArgs { 
		bool isVertical;
		bool isBroken;
		bool reverseDraw, fillBackground;
		bool inDesign;
		bool flowAnimationEnabled;
		int flowAnimationSpeed;
		int flowAnimationDelay;
		Color startColor, endColor;
		float percent;
		Rectangle progressBounds;
		TextOrientation textOrientation;
		LinearSequenceGenerator sequenceSource;
		public ProgressBarObjectInfoArgs(AppearanceObject style) {
			SetAppearance(style);
			this.percent = 1;
			this.fillBackground = true;
			this.startColor = this.endColor = SystemColors.Highlight;
			this.progressBounds = Rectangle.Empty;
			this.textOrientation = TextOrientation.Default;
			this.flowAnimationEnabled = false;
			this.flowAnimationSpeed = 180;
			this.flowAnimationDelay = 1000;
			this.sequenceSource = new LinearSequenceGenerator();
			this.MarqueeWidth = 100;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			ProgressBarObjectInfoArgs pInfo = info as ProgressBarObjectInfoArgs;
			if(pInfo == null) return;
			this.isVertical = pInfo.isVertical;
			this.isBroken = pInfo.isBroken;
			this.reverseDraw = pInfo.reverseDraw;
			this.fillBackground = pInfo.fillBackground;
			this.startColor = pInfo.startColor;
			this.endColor = pInfo.endColor;
			this.percent = pInfo.percent;
			this.inDesign = pInfo.inDesign;
			this.progressBounds = pInfo.progressBounds;
			this.textOrientation = pInfo.textOrientation;
			this.ProgressPadding = ProgressPadding;
			this.flowAnimationEnabled = pInfo.flowAnimationEnabled;
			this.flowAnimationSpeed = pInfo.flowAnimationSpeed;
			this.flowAnimationDelay = pInfo.flowAnimationDelay;
			this.sequenceSource = pInfo.sequenceSource;
			this.MarqueeWidth = pInfo.MarqueeWidth;
		}
		public virtual Padding ProgressPadding { get; set; }
		public virtual float Percent { get { return percent; } set { percent = value; } }
		public virtual bool IsVertical { get { return isVertical; } set { isVertical = value; } }
		public virtual bool IsBroken { get { return isBroken; } set { isBroken = value; } }
		public virtual bool ReverseDraw { get { return reverseDraw; } set { reverseDraw = value; } }
		public bool GetReverseDraw() { return RightToLeft ? !ReverseDraw : ReverseDraw; }
		public virtual bool FillBackground { get { return fillBackground; } set { fillBackground = value; } }
		public virtual Color StartColor { get { return startColor; } set { startColor = value; } }
		public virtual Color EndColor { get { return endColor; } set { endColor = value; } }
		public virtual bool InDesign { get { return inDesign; } set { inDesign = value; } }
		public virtual Rectangle ProgressBounds { get { return progressBounds; } set { progressBounds = value; } }
		public virtual TextOrientation TextOrientation { get { return textOrientation; } set { textOrientation = value; } }
		public virtual bool FlowAnimationEnabled { get { return flowAnimationEnabled; } set { flowAnimationEnabled = value; } }
		public virtual int FlowAnimationSpeed { get { return flowAnimationSpeed; } set { flowAnimationSpeed = value; } }
		public virtual int FlowAnimationDelay { get { return flowAnimationDelay; } set { flowAnimationDelay = value; } }
		public virtual LinearSequenceGenerator SequenceSource { get { return sequenceSource; } set { sequenceSource = value; } }
		public int MarqueeWidth { get; set; }
	}
	internal interface ILinearSequenceGenerator {
		void Reset();
		long GetCurrentTicks();
		long GetCurrentPosition(int speed);
	}
	public class LinearSequenceGenerator : ILinearSequenceGenerator {
		long startTicks = DateTime.Now.Ticks;
		void ILinearSequenceGenerator.Reset() {
			startTicks = DateTime.Now.Ticks;
		}
		long ILinearSequenceGenerator.GetCurrentTicks() {
			return DateTime.Now.Ticks - startTicks;
		}
		long ILinearSequenceGenerator.GetCurrentPosition(int speed) {
			long ticksFromStart = DateTime.Now.Ticks - startTicks;
			long currPos = (ticksFromStart / 100) * speed / 100000;
			return currPos;
		}
	}
	public enum ProgressAnimationMode { Default, Cycle, PingPong }
	public class MarqueeProgressBarObjectInfoArgs : ProgressBarObjectInfoArgs {
		int marqueAnimationSpeed;
		bool animated;
		bool paused;
		long posBeforePause, deltaPos;
		ProgressAnimationMode animationMode;
		public MarqueeProgressBarObjectInfoArgs(AppearanceObject style) : base(style) {
			this.marqueAnimationSpeed = 100;
			this.animationMode = ProgressAnimationMode.Default;
			this.animated = true;
			this.paused = false;
			this.posBeforePause = this.deltaPos = 0;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			MarqueeProgressBarObjectInfoArgs pInfo = info as MarqueeProgressBarObjectInfoArgs;
			if(pInfo == null) return;
			this.marqueAnimationSpeed = pInfo.marqueAnimationSpeed;
			this.animationMode = pInfo.animationMode;
			this.animated = pInfo.animated;
			this.paused = pInfo.paused;
			this.posBeforePause = pInfo.posBeforePause;
			this.deltaPos = pInfo.deltaPos;
		}
		public long LastPosition { get; set; }
		public long Position { get; set; }
		public virtual int MarqueAnimationSpeed { get { return marqueAnimationSpeed; } set { marqueAnimationSpeed = value; } }
		public virtual ProgressAnimationMode AnimationMode { get { return animationMode; } set { animationMode = value; } }
		public virtual bool Animated { get { return animated; } set { animated = value; } }
		public virtual bool Paused { get { return paused; } set { paused = value; } }
		public virtual long PosBeforePause { get { return posBeforePause; } set { posBeforePause = value; } }
		public virtual long DeltaPos { get { return deltaPos; } set { deltaPos = value; } }
	}
	public class ProgressBarObjectPainter : ObjectPainter {
		protected virtual bool AllowBroken { get { return true; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Empty;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 4, 2);
		}
		public virtual void DrawBackground(ProgressBarObjectInfoArgs ee) {
			if(ee.FillBackground) ee.Cache.Paint.FillRectangle(ee.Graphics, ee.Appearance.GetBackBrush(ee.Cache, ee.Bounds), ee.Bounds);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ProgressBarObjectInfoArgs ee = e as ProgressBarObjectInfoArgs;
			DrawBackground(ee);
			DrawBar(ee);
		}
		protected virtual void DrawBar(ProgressBarObjectInfoArgs e) {
			if(e.IsBroken)
				DrawBroken(e);
			else	
				DrawSolid(e);
			if(e.FlowAnimationEnabled && !e.InDesign)
				DrawAnimation(e);
		}
		protected virtual void DrawAnimation(ProgressBarObjectInfoArgs e) {
			Rectangle bounds = CalcProgressBounds(e);
			RectangleF oldclip = e.Graphics.ClipBounds;
			e.Graphics.SetClip(bounds);
			try {
				if(bounds.Width > 0 && bounds.Height > 0)
					DrawAnimation(e, bounds);
			}
			finally {
				e.Graphics.SetClip(oldclip);
			}
		}
		protected virtual void DrawAnimation(ProgressBarObjectInfoArgs e, Rectangle pb) {
		}
		protected virtual Rectangle CalcAnimation(ProgressBarObjectInfoArgs e, Rectangle pb) {
			int length = CalcFlowIndicatorLength(e, pb);
			int startAnimation = 0;
			Rectangle shadowRect = Rectangle.Empty;
			if(!e.IsVertical) {
				shadowRect.Height = pb.Height;
				shadowRect.Y = pb.Y;
				startAnimation = pb.X;
				if(pb.Width > 0)
					startAnimation = CalcStartAnimationPosition(e, length, pb, pb.Width);
				shadowRect.X = startAnimation;
				shadowRect.Width = length;
			}
			else {
				shadowRect.Width = pb.Width;
				shadowRect.X = pb.X;
				startAnimation = pb.Bottom;
				if(pb.Height > 0)
					startAnimation = CalcStartAnimationPosition(e, length, pb, PatchBounds(e).Height);
				shadowRect.Y = startAnimation;
				shadowRect.Height = length;
			}
			return shadowRect;
		}
		protected virtual int CalcFlowIndicatorLength(ProgressBarObjectInfoArgs e, Rectangle pb) {
			return 40;
		}
		int AnimationSpeedDivider = 1000;
		protected int CalcStartAnimationPosition(ProgressBarObjectInfoArgs e, int width, Rectangle pb, int length) {
			int startPos = 0;
			long pos = ((ILinearSequenceGenerator)e.SequenceSource).GetCurrentPosition(e.FlowAnimationSpeed);
			if(pos > length + 2 * width + e.FlowAnimationDelay * e.FlowAnimationSpeed / AnimationSpeedDivider) {
				((ILinearSequenceGenerator)e.SequenceSource).Reset();
				pos = ((ILinearSequenceGenerator)e.SequenceSource).GetCurrentPosition(e.FlowAnimationSpeed);
			}
			if(!e.IsVertical)
				startPos = (int)pos + pb.X - width;
			else {
				startPos = pb.Bottom - (int)pos; 
			}
			return startPos;
		}
		protected virtual void DrawBroken(ProgressBarObjectInfoArgs e) { DrawBroken(e, CalcProgressBounds(e)); }
		const int BrokenIndent = 2;
		protected virtual void DrawBroken(ProgressBarObjectInfoArgs e, Rectangle pb) {
			Brush brush = CreateBrush(e);
			Rectangle r = pb;
			int step = GetChunkSize(e) - BrokenIndent;
			if(step <= 0)
				step = 1;
			int far;
			if(!e.IsVertical) {
				if(e.GetReverseDraw()) {
					far = r.Left;
					r.X = r.Right - step;
				} else
					far = r.Right;
				r.Width = step;
				while(e.GetReverseDraw() ? r.Left >= far : r.Right <= far) {
					e.Cache.Paint.FillGradientRectangle(e.Graphics, brush, r);
					if(e.GetReverseDraw()) {
						r.X -= step + BrokenIndent;
						if(r.X <= far && r.X + step > far) {
							r.Width = far - r.X - BrokenIndent;
							r.X = far;
							if(r.Width < 1 || e.Percent < 1) break;
						}
					} else {
						r.X += step + BrokenIndent;
						if(r.Right >= far) {
							r.Width = far - r.X;
							if(r.Width < 1 || e.Percent < 1) break;
						}
					}
				}
			} else {
				if(e.GetReverseDraw()) {
					far = r.Bottom;
				} else {
					r.Y = r.Bottom - step;
					far = pb.Y;
				}
				r.Height = step;
				while(e.GetReverseDraw() ? r.Bottom <= far : r.Top >= far) {
					e.Cache.Paint.FillGradientRectangle(e.Graphics, brush, r);
					if(e.GetReverseDraw()) {
						r.Y += step + BrokenIndent;
						if(r.Bottom >= far) {
							r.Height = far - r.Y;
							if(r.Height < 1 || e.Percent < 1) break;
						}
					} else {
						r.Y -= step + BrokenIndent;
						if(r.Y <= far && r.Y + step > far) {
							r.Height = far - r.Y - BrokenIndent;
							r.Y = far;
							if(r.Height < 1 || e.Percent < 1) break;
						}
					}
				}
			}
		}
		protected virtual void DrawSolid(ProgressBarObjectInfoArgs e) { DrawSolid(e, CalcProgressBounds(e)); }
		protected virtual void DrawSolid(ProgressBarObjectInfoArgs e, Rectangle rect) {
			Brush brush = CreateBrush(e);
			e.Cache.Paint.FillGradientRectangle(e.Graphics, brush, rect);
		}
		protected virtual Brush CreateBrush(ProgressBarObjectInfoArgs e) {
			if(e.StartColor == e.EndColor) 
				return e.Cache.GetSolidBrush(e.StartColor);
			if(e.IsVertical) {
				return e.Cache.GetGradientBrush(e.Bounds, e.EndColor, e.StartColor, LinearGradientMode.Vertical);
			}
			return e.Cache.GetGradientBrush(e.Bounds, e.StartColor, e.EndColor, LinearGradientMode.Horizontal);
		}
		protected Rectangle PatchBounds(ProgressBarObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			if(e.IsVertical)
				r = new Rectangle(r.X + e.ProgressPadding.Top, r.Y + e.ProgressPadding.Left, r.Width - e.ProgressPadding.Vertical, r.Height - e.ProgressPadding.Horizontal);
			else 
				r = new Rectangle(r.X + e.ProgressPadding.Left, r.Y + e.ProgressPadding.Top, r.Width - e.ProgressPadding.Horizontal, r.Height - e.ProgressPadding.Vertical);
			if(AllowBroken) {
				if(e.IsBroken) {
					if(r.Width > 7 && r.Height > 7)
						r.Inflate(-2, -2);
					else if(r.Width > 4 && r.Height > 4)
						r.Inflate(-1, -1);
				}
			}
			return r;
		}
		protected virtual Rectangle CalcProgressBounds(ProgressBarObjectInfoArgs e) {
			Rectangle r = PatchBounds(e);
			if(e.Percent < 1) {
				Size size = r.Size;
				if(!e.IsVertical) {
					r.Width = Math.Min(Convert.ToInt32(e.Percent * (float)(size.Width)), size.Width);
					if(e.GetReverseDraw()) r.X += size.Width - r.Width;
				}
				else {
					r.Height = Math.Min(System.Convert.ToInt32(e.Percent * (float)(size.Height)), size.Height);
					if(!e.GetReverseDraw()) r.Y += size.Height - r.Height;
				}
			}
			e.ProgressBounds = r;
			return r;
		}
		protected virtual int GetChunkSize(ProgressBarObjectInfoArgs e) {
			if(!e.IsBroken)
				return 0;
			int chunkSize = e.IsVertical ? e.Bounds.Width : e.Bounds.Height;
			chunkSize = Math.Max(1, (int)((float)chunkSize * 0.62F));
			chunkSize = Math.Min(chunkSize, 8);
			chunkSize += BrokenIndent;
			return chunkSize;
		}
		int GetMarqueLength(ProgressBarObjectInfoArgs e) {
			int chunkSize = GetChunkSize(e);
			if(chunkSize <= 0) {
				return e.MarqueeWidth;
			}
			int marqueeLength = chunkSize * 6;
			int boundsSize = e.IsVertical ? e.Bounds.Height : e.Bounds.Width;
			if(boundsSize > marqueeLength * 4) {
				marqueeLength = boundsSize / (chunkSize * 4) * chunkSize;
			}
			else if(boundsSize * 2 < marqueeLength * 3) {
				marqueeLength = boundsSize * 2 / (chunkSize * 3) * chunkSize;
				if(marqueeLength <= 0)
					marqueeLength = chunkSize;
			}
			return marqueeLength;
		}
		public const int MarqueAnimationSpeedDivider = 1000;
		int CalculateMarqueStartPos(int marqueLength, int totalLength, MarqueeProgressBarObjectInfoArgs e) {
			if(e.Animated == false) return totalLength;
			int animationSpeed = e.MarqueAnimationSpeed;
			if(animationSpeed < 1)
				animationSpeed = 1;
			long position = GetMarqueeProgressPos(animationSpeed, e);
			if(e.AnimationMode == ProgressAnimationMode.PingPong) {
				int bound = Math.Max(totalLength - marqueLength, 1);
				position = position % (bound * 2);
				if(position > bound)
					position = bound * 2 - position;
			}
			else {
				int bound = Math.Max(totalLength + marqueLength, 1);
				long pos = position - e.LastPosition;
				e.LastPosition = position;
				pos += e.Position;
				if(pos > bound)
					pos %= bound;
				e.Position = pos;
				pos -= marqueLength;
				int chunkSize = GetChunkSize(e);
				if(chunkSize > 0)
					pos = pos / chunkSize * chunkSize;
				position = pos;
			}
			return (int)position;
		}
		long GetMarqueeProgressPos(int animationSpeed, MarqueeProgressBarObjectInfoArgs e) {
			long pos = DateTime.Now.Ticks / animationSpeed / MarqueAnimationSpeedDivider;
			if(e.Paused) {
				if(e.PosBeforePause == 0) e.PosBeforePause = pos - e.DeltaPos;
				return e.PosBeforePause;
			}
			if(e.PosBeforePause > 0) {
				e.DeltaPos = pos - e.PosBeforePause;
				e.PosBeforePause = 0;
			}
			return pos - e.DeltaPos;
		}
		protected Rectangle CalcMarqueProgressBounds(MarqueeProgressBarObjectInfoArgs e) {
			int marqueLength = GetMarqueLength(e);
			Rectangle r = PatchBounds(e);
			if(e.IsVertical) {
				int chunkSize = GetChunkSize(e);
				if(chunkSize > 0) {
					int correctedHeight = r.Height / chunkSize * chunkSize;
					int corrector = r.Height - correctedHeight;
					r.Y += corrector;
					r.Height -= corrector;
				}
			}
			int totalLength = e.IsVertical ? r.Height : r.Width;
			int marqueStartPos;
			if(!e.InDesign) marqueStartPos = CalculateMarqueStartPos(marqueLength, totalLength, e);
			else marqueStartPos = (totalLength - marqueLength) / 2;
			Rectangle marque;
			if(!e.IsVertical) {
				marque = new Rectangle(r.X + marqueStartPos, r.Y, marqueLength, r.Height);
				if(e.RightToLeft) {
					marque = new Rectangle(r.Right  - marqueStartPos - marqueLength, r.Y, marqueLength, r.Height);
				}
			}
			else {
				marque = new Rectangle(r.X, r.Bottom - marqueStartPos - marqueLength, r.Width, marqueLength);
			}
			marque.Intersect(r);
			return marque;
		}
	}
	public class MarqueeProgressBarObjectPainter : ProgressBarObjectPainter {
		protected override Rectangle CalcProgressBounds(ProgressBarObjectInfoArgs e) {
			MarqueeProgressBarObjectInfoArgs me = e as MarqueeProgressBarObjectInfoArgs;
			if(me == null) return Rectangle.Empty;
			return CalcMarqueProgressBounds(me);
		}
	}
	public class OpenCloseButtonInfoArgs : StyleObjectInfoArgs {
		bool opened;
		public OpenCloseButtonInfoArgs(GraphicsCache cache) : this(cache, Rectangle.Empty, false, null, ObjectState.Normal) { }
		public OpenCloseButtonInfoArgs(GraphicsCache cache, Rectangle bounds, bool opened, AppearanceObject style, ObjectState state) : base(cache, bounds, style, state) {
			this.opened = opened;
		}
		public bool Opened { get { return opened; } set { opened = value; } }
	}
	public class OpenCloseButtonObjectPainter : ObjectPainter {
		protected ObjectPainter fButtonPainter;
		public OpenCloseButtonObjectPainter(ObjectPainter buttonPainter) {
			this.fButtonPainter = buttonPainter;
		}
		protected virtual ObjectPainter ButtonPainter { get { return fButtonPainter; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle saveBounds = e.Bounds, res;
			try {
				e.Bounds = new Rectangle(0, 0, 7, 7);
				res = ButtonPainter.CalcBoundsByClientRectangle(e);
			} finally {
				e.Bounds = saveBounds;
			}
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			OpenCloseButtonInfoArgs ee = e as OpenCloseButtonInfoArgs;
			ButtonPainter.DrawObject(e);
			Brush brush = ee.Appearance == null ? SystemBrushes.Control : e.Cache.GetSolidBrush(ee.Appearance.ForeColor);
			Rectangle r = ButtonPainter.GetObjectClientRectangle(e), r1;
			r1 = r;
			r1.Y = r.Y + r.Height / 2; r1.Height = 1; r1.Inflate(-1, 0);
			e.Cache.Paint.FillRectangle(e.Graphics, brush, r1);
			r1 = r;
			r1.X = r.X + r.Width / 2; r1.Width = 1; r1.Inflate(0, -1);
			if(!ee.Opened) 
				e.Cache.Paint.FillRectangle(e.Graphics, brush, r1);
		}
	}
	public class ExpandButtonGlyphPainter : OpenCloseButtonObjectPainter {
		public ExpandButtonGlyphPainter():base(null) { }
		public override void DrawObject(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			Brush brush = info.PaintAppearance == null ? SystemBrushes.Control : e.Cache.GetSolidBrush(info.PaintAppearance.ForeColor);
			Rectangle r = GetObjectClientRectangle(e);
			Rectangle r1 = r;
			r1.Y = r.Y + r.Height / 2; r1.Height = 1; r1.Inflate(-1, 0);
			e.Cache.Paint.FillRectangle(e.Graphics, brush, r1);
			r1 = r;
			r1.X = r.X + r.Width / 2; r1.Width = 1; r1.Inflate(0, -1);
			if(!info.Button.Properties.Checked)
				e.Cache.Paint.FillRectangle(e.Graphics, brush, r1);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			if(IsPressed(e)) {
				r.X += 3;
				r.Width -= 4;
				r.Y += 3;
				r.Height -= 4;
			}
			else {
				r.Inflate(-2, -2);
			}
			return r;
		}
		protected virtual bool IsPressed(ObjectInfoArgs e) {
			return (e.State & ObjectState.Pressed) != 0;
		}
	}
	public class Office2003OpenCloseButtonObjectPainter : OpenCloseButtonObjectPainter {
		[ThreadStatic]
		static Bitmap office2003GroupButtons;
		static Bitmap Office2003GroupButtons {
			get { 
				if(office2003GroupButtons == null) {
					office2003GroupButtons = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.GroupButtons.bmp", typeof(Office2003OpenCloseButtonObjectPainter).Assembly) as Bitmap;
					office2003GroupButtons.MakeTransparent();
				}
				return office2003GroupButtons;
			}
		}
		public Office2003OpenCloseButtonObjectPainter() : base(null) { 	}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 11, 11);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			OpenCloseButtonInfoArgs ee = e as OpenCloseButtonInfoArgs;
			Rectangle r = ee.Bounds, source;
			source = new Rectangle(0, 0, 11, 11);
			if(!ee.Opened) source.X += source.Width;
			e.Paint.DrawImage(e.Graphics, Office2003GroupButtons, r, source, true);
		}
	}
	public class ExplorerBarOpenCloseButtonInfoArgs : StyleObjectInfoArgs {
		bool expanded;
		public ExplorerBarOpenCloseButtonInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState state, bool expanded) : base(cache, bounds, appearance, state) {
			this.expanded = expanded;
		}
		public bool Expanded { get { return expanded; } set { expanded = value; } }
	}
	public class FlatButtonObjectPainterEx : FlatButtonObjectPainter {
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject appearance) {
			DrawHot(e, appearance);
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject appearance) {
			BBrushes brushes = new BBrushes(e.Cache, appearance);
			Rectangle r = e.Bounds;
			DrawFlatBounds(e, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			appearance.FillRectangle(e.Cache, r, true);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject appearance) {
			Rectangle r = e.Bounds;
			appearance.FillRectangle(e.Cache, r);
		}
	}
	public class ExpandButtonOffice2003GlyphPainter : ExplorerBarOpenCloseButtonObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = GetObjectClientRectangle(e);
			int x = r.X + (r.Width / 2) - 1;
			int y = r.Y + (r.Height / 2) - 2;
			bool expanded = IsExpanded(e);
			DrawMark(e, x, y - 2, expanded, 1);
			DrawMark(e, x, y - 2, expanded, 0);
			DrawMark(e, x, y + 2, expanded, 1);
			DrawMark(e, x, y + 2, expanded, 0);
		}
		public override AppearanceObject GetStyle(ObjectInfoArgs e) {
			return (e as BaseButtonInfo).PaintAppearance;
		}
		protected override void DrawSelectedFrame(ObjectInfoArgs e) { }
		protected override bool IsExpanded(ObjectInfoArgs e) {
			return (e as BaseButtonInfo).Button.Properties.Checked;
		}
	}
	public class ExplorerBarOpenCloseButtonObjectPainter : FlatButtonObjectPainterEx {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 16, 16);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject appearance) {
			Rectangle r = e.Bounds;
			appearance.FillRectangle(e.Cache, r, true);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate( -2, -2);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			Rectangle r = GetObjectClientRectangle(e);
			int x = r.X + (r.Width / 2) - 1;
			int y = r.Y + (r.Height / 2) - 2;
			bool expanded = IsExpanded(e);
			DrawMark(e, x, y - 2, expanded, 1);
			DrawMark(e, x, y - 2, expanded, 0);
			DrawMark(e, x, y + 2, expanded, 1);
			DrawMark(e, x, y + 2, expanded, 0);
		}
		protected virtual bool IsExpanded(ObjectInfoArgs e) {
			return ((ExplorerBarOpenCloseButtonInfoArgs)e).Expanded;
		}
		protected virtual Pen GetMarkPen(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			return appearance.GetForePen(e.Cache);
		}
		protected void DrawMark(ObjectInfoArgs e, int x, int startY, bool expanded, int delta) {
			int dx = 3, dy = 3;
			if(expanded) startY += dy;
			dx -= delta; dy -= delta;
			if(expanded) dy = -dy;
			Pen pen = GetMarkPen(e);
			e.Graphics.DrawLine(pen, new Point(x - dx, startY), new Point(x, startY + dy));
			e.Graphics.DrawLine(pen, new Point(x, startY + dy), new Point(x + dx, startY));
		}
	}
	public class AdvExplorerBarOpenCloseButtonObjectPainter : ExplorerBarOpenCloseButtonObjectPainter {
		public AdvExplorerBarOpenCloseButtonObjectPainter() {
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 18, 18);
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject appearance) {
			DrawNormal(e, appearance);
		}
		protected override Pen GetMarkPen(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			return e.Cache.GetPen(ExplorerBarColorHelper.CalcTextColor(e, appearance.GetForeColor(), true), 1);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject appearance) {
			SmoothingMode saved = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			ExplorerBarOpenCloseButtonInfoArgs args = e as ExplorerBarOpenCloseButtonInfoArgs;
			AppearanceObject backStyle = args.BackAppearance;
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			Brush shadowBrush = e.Cache.GetSolidBrush(Color.FromArgb(90, backStyle.GetBackColor()));
			r.Offset(2, 2);
			e.Graphics.FillEllipse(shadowBrush, r);
			r.Offset(-2, -2);
			e.Graphics.FillEllipse(SystemBrushes.Window, r);
			Color color = Office2003Colors.Default[Office2003Color.NavBarExpandButtonRoundColor];
			for(int n = 0; n < 2; n ++) {
				Pen pen = e.Cache.GetPen(color, 1);
				e.Graphics.DrawEllipse(pen, r);
				color = ControlPaint.Light(color, 0.5f);
				r.Inflate(-1, -1);
			}
			e.Graphics.SmoothingMode = saved;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			r.X += 1;
			r.Y += 1;
			return r;
		}
	}
	public class ExplorerBarColorHelper {
		public static Color CalcColor(int d) {
			Color clr = SystemColors.Highlight;
			int r = clr.R, g = clr.G, b = clr.B;
			int max = Math.Max(Math.Max(r, g), b);
			int delta = 0x23 + d;
			int maxDelta = (255 - (max + delta));
			if(maxDelta > 0) maxDelta = 0;
			r += (delta + maxDelta + 5);
			g += (delta + maxDelta);
			b += (delta + maxDelta);
			if(r > 255) r = 255;
			if(g > 255) g = 255;
			if(b > 255) b = 255;
			return Color.FromArgb(Math.Abs(r), Math.Abs(g), Math.Abs(b));
		}
		public static Brush CalcTextBrush(ObjectInfoArgs e, Color fromColor, bool enabled) {
			return e.Cache.GetSolidBrush(CalcTextColor(e, fromColor, enabled));
		}
		public static Color CalcDefaultTextColor(ObjectInfoArgs e, bool enabled) {
			ObjectState state = e.State;
			if(!enabled) state = ObjectState.Disabled;
			switch(state) {
				case ObjectState.Selected:
				case ObjectState.Pressed:
				case ObjectState.Hot: 
					return Office2003Colors.Default[Office2003Color.NavBarLinkHighlightedTextColor];
				case ObjectState.Disabled :
					return Office2003Colors.Default[Office2003Color.NavBarLinkDisabledTextColor];
			}
			return Office2003Colors.Default[Office2003Color.NavBarLinkTextColor];
		}
		public static Color CalcTextColor(ObjectInfoArgs e, Color fromColor, bool enabled) {
			bool isDefaultColor = fromColor == Color.Empty;
			if(isDefaultColor) return CalcDefaultTextColor(e, enabled);
			Color textColor = fromColor;
			ObjectState state = e.State;
			if(!enabled) state = ObjectState.Disabled;
			switch(state) {
				case ObjectState.Selected:
				case ObjectState.Pressed:
				case ObjectState.Hot: 
					textColor = ControlPaint.Light(textColor);
					break;
				case ObjectState.Disabled :
					if(textColor == Color.FromArgb(0x00,0x15, 0x5b)) return Color.Gray;
					textColor = ControlPaint.LightLight(textColor);
					break;
			}
			return textColor;
		}
	}
	public static class ImageLayoutHelper {
		public static RectangleF GetImageBounds(RectangleF bounds, SizeF imageSize, ImageScaleMode sizeMode, ImageAlignmentMode alignment) {
			RectangleF rect = sizeMode == ImageScaleMode.Clip? new RectangleF(PointF.Empty, imageSize): GetImageBounds(bounds, imageSize, GetImageLayoutMode(sizeMode));
			return GetImageBounds(bounds, rect.Size, GetImageLayoutMode(alignment));
		}
		private static ImageLayoutMode GetImageLayoutMode(ImageAlignmentMode alignment) {
			switch(alignment) { 
				case ImageAlignmentMode.BottomCenter:
					return ImageLayoutMode.BottomCenter;
				case ImageAlignmentMode.BottomLeft:
					return ImageLayoutMode.BottomLeft;
				case ImageAlignmentMode.BottomRight:
					return ImageLayoutMode.BottomRight;
				case ImageAlignmentMode.MiddleCenter:
					return ImageLayoutMode.MiddleCenter;
				case ImageAlignmentMode.MiddleLeft:
					return ImageLayoutMode.MiddleLeft;
				case ImageAlignmentMode.MiddleRight:
					return ImageLayoutMode.MiddleRight;
				case ImageAlignmentMode.TopCenter:
					return ImageLayoutMode.TopCenter;
				case ImageAlignmentMode.TopLeft:
					return ImageLayoutMode.TopLeft;
				case ImageAlignmentMode.TopRight:
					return ImageLayoutMode.TopRight;
			}
			return ImageLayoutMode.MiddleCenter;
		}
		private static ImageLayoutMode GetImageLayoutMode(ImageScaleMode sizeMode) {
			switch(sizeMode) { 
				case ImageScaleMode.Squeeze:
					return ImageLayoutMode.Squeeze;
				case ImageScaleMode.Stretch:
					return ImageLayoutMode.Stretch;
				case ImageScaleMode.StretchHorizontal:
					return ImageLayoutMode.StretchHorizontal;
				case ImageScaleMode.StretchVertical:
					return ImageLayoutMode.StretchVertical;
				case ImageScaleMode.ZoomInside:
					return ImageLayoutMode.ZoomInside;
				case ImageScaleMode.ZoomOutside:
					return ImageLayoutMode.ZoomOutside;
			}
			return ImageLayoutMode.Squeeze;
		}
		public static Rectangle GetImageBounds(Rectangle bounds, Size imageSize, ImageScaleMode scaleMode, ImageAlignmentMode layoutMode) {
			RectangleF boundsF = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			SizeF sizeF = new SizeF(imageSize.Width, imageSize.Height);
			RectangleF res = GetImageBounds(boundsF, sizeF, scaleMode, layoutMode);
			return new Rectangle((int)(res.X + 0.5f), (int)(res.Y + 0.5f), (int)(res.Width + 0.5f), (int)(res.Height + 0.5f));
		}
		public static RectangleF GetImageBounds(RectangleF bounds, SizeF imageSize, ImageLayoutMode imageLayout) {
			RectangleF res = bounds;
			if(imageLayout == ImageLayoutMode.Stretch)
				return bounds;
			if(imageLayout == ImageLayoutMode.Squeeze) {
				if(bounds.Width >= imageSize.Width && bounds.Height >= imageSize.Height)
					imageLayout = ImageLayoutMode.MiddleCenter;
				else
					imageLayout = ImageLayoutMode.ZoomInside;
			}
			if(imageLayout == ImageLayoutMode.ZoomInside || imageLayout == ImageLayoutMode.ZoomOutside) {
				float scaleX = imageSize.Width == 0.0f? 1.0f: bounds.Width / imageSize.Width;
				float scaleY = imageSize.Height == 0.0f? 1.0f: bounds.Height / imageSize.Height;
				float currScale;
				if(imageLayout == ImageLayoutMode.ZoomInside)
					currScale = scaleX > scaleY ? scaleY : scaleX;
				else
					currScale = scaleX > scaleY ? scaleX : scaleY;
				res.Width = imageSize.Width * currScale;
				res.Height = imageSize.Height * currScale;
				return CenterRectangle(bounds, res);
			}
			res.Width = imageSize.Width;
			res.Height = imageSize.Height;
			if(imageLayout == ImageLayoutMode.TopLeft) {
				return res;
			}
			if(imageLayout == ImageLayoutMode.TopCenter || imageLayout == ImageLayoutMode.MiddleCenter || imageLayout == ImageLayoutMode.BottomCenter) {
				res.X = AlignCenter(bounds.X, bounds.Width, res.Width);
			}
			if(imageLayout == ImageLayoutMode.TopRight || imageLayout == ImageLayoutMode.MiddleRight || imageLayout == ImageLayoutMode.BottomRight) {
				res.X = AlignRightBottom(bounds.X, bounds.Width, res.Width);
			}
			if(imageLayout == ImageLayoutMode.MiddleCenter || imageLayout == ImageLayoutMode.MiddleLeft || imageLayout == ImageLayoutMode.MiddleRight) {
				res.Y = AlignCenter(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.BottomLeft || imageLayout == ImageLayoutMode.BottomCenter || imageLayout == ImageLayoutMode.BottomRight) {
				res.Y = AlignRightBottom(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.StretchHorizontal) {
				res.Width = bounds.Width;
				res.X = bounds.X;
				res.Y = AlignCenter(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.StretchVertical) {
				res.Height = bounds.Height;
				res.Y = bounds.Y;
				res.X = AlignCenter(bounds.X, bounds.Width, res.Width);
			}
			return res;
		}
		public static Rectangle GetImageBounds(Rectangle bounds, Size imageSize, ImageLayoutMode imageLayout) {
			RectangleF boundsF = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			SizeF sizeF = new SizeF(imageSize.Width, imageSize.Height);
			RectangleF res = GetImageBounds(boundsF, sizeF, imageLayout);
			return new Rectangle((int)(res.X + 0.5f), (int)(res.Y + 0.5f), (int)(res.Width + 0.5f), (int)(res.Height + 0.5f));
		}
		static float AlignCenter(float boundStartValue, float boundValue, float val) {
			return boundStartValue + (boundValue - val) / 2;
		}
		static float AlignRightBottom(float boundStartValue, float boundValue, float val) {
			return boundStartValue + (boundValue - val);
		}
		static RectangleF CenterRectangle(RectangleF bounds, RectangleF rect) {
			RectangleF res = rect;
			res.X = bounds.X + (bounds.Width - rect.Width) / 2;
			res.Y = bounds.Y + (bounds.Height - rect.Height) / 2;
			return res;
		}
		static int AlignCenter(int boundStartValue, int boundValue, int val) {
			return boundStartValue + (boundValue - val) / 2;
		}
		static int AlignRightBottom(int boundStartValue, int boundValue, int val) {
			return boundStartValue + (boundValue - val);
		}
		static Rectangle CenterRectangle(Rectangle bounds, Rectangle rect) {
			Rectangle res = rect;
			res.X = bounds.X + (bounds.Width - rect.Width) / 2;
			res.Y = bounds.Y + (bounds.Height - rect.Height) / 2;
			return res;
		}
	}
	public enum ItemLocation { Default, Top, Left, Bottom, Right }
	public enum ItemVerticalAlignment { Default, Top, Center, Bottom, Stretch }
	public enum ItemHorizontalAlignment { Default, Left, Center, Right, Stretch }
	public class Items2Panel {
		Padding content1Padding;
		Padding content2Padding;
		Padding verticalPadding;
		Padding horizontalPadding;
		int verticalIndent;
		int horizontalIndent;
		ItemLocation content1Location;
		ItemHorizontalAlignment content1HorizontalAlignment;
		ItemHorizontalAlignment content2HorizontalAlignment;
		ItemVerticalAlignment content1VerticalAlignment;
		ItemVerticalAlignment content2VerticalAlignment;
		bool stretchContent1, stretchContent2;
		bool stretchContent1SecondarySize, stretchContent2SecondarySize;
		public Items2Panel() {
			this.content1Padding = new Padding(0);
			this.content2Padding = new Padding(0);
			this.verticalPadding = new Padding(0);
			this.horizontalPadding = new Padding(0);
			this.horizontalIndent = 0;
			this.verticalIndent = 0;
			this.content1Location = ItemLocation.Left;
			this.content1HorizontalAlignment = ItemHorizontalAlignment.Stretch;
			this.content1VerticalAlignment = ItemVerticalAlignment.Stretch;
			this.content2HorizontalAlignment = ItemHorizontalAlignment.Stretch;
			this.content2VerticalAlignment = ItemVerticalAlignment.Stretch;
		}
		public ItemHorizontalAlignment GetHorizontalAlignment(HorzAlignment align) {
			if(align == HorzAlignment.Center)
				return ItemHorizontalAlignment.Center;
			if(align == HorzAlignment.Far)
				return ItemHorizontalAlignment.Right;
			if(align == HorzAlignment.Near)
				return ItemHorizontalAlignment.Left;
			return ItemHorizontalAlignment.Center;
		}
		public ItemVerticalAlignment GetVerticalAlignment(VertAlignment align) {
			if(align == VertAlignment.Center)
				return ItemVerticalAlignment.Center;
			if(align == VertAlignment.Bottom)
				return ItemVerticalAlignment.Bottom;
			if(align == VertAlignment.Top)
				return ItemVerticalAlignment.Top;
			return ItemVerticalAlignment.Center;
		}
		public Padding Content1Padding {
			get { return content1Padding; }
			set { content1Padding = value; }
		}
		public Padding Content2Padding {
			get { return content2Padding; }
			set { content2Padding = value; }
		}
		public Padding VerticalPadding {
			get { return verticalPadding; }
			set { verticalPadding = value; }
		}
		public Padding HorizontalPadding {
			get { return horizontalPadding; }
			set { horizontalPadding = value; }
		}
		public int VerticalIndent {
			get { return verticalIndent; }
			set { verticalIndent = value; }
		}
		public int HorizontalIndent {
			get { return horizontalIndent; }
			set { horizontalIndent = value; }
		}
		public ItemLocation Content1Location {
			get { return content1Location; }
			set { content1Location = value; }
		}
		public ItemHorizontalAlignment Content1HorizontalAlignment {
			get { return content1HorizontalAlignment; }
			set { content1HorizontalAlignment = value; }
		}
		public ItemHorizontalAlignment Content2HorizontalAlignment {
			get { return content2HorizontalAlignment; }
			set { content2HorizontalAlignment = value; }
		}
		public ItemVerticalAlignment Content1VerticalAlignment {
			get { return content1VerticalAlignment; }
			set { content1VerticalAlignment = value; }
		}
		public ItemVerticalAlignment Content2VerticalAlignment {
			get { return content2VerticalAlignment; }
			set { content2VerticalAlignment = value; }
		}
		public bool StretchContent1 {
			get { return stretchContent1; }
			set { stretchContent1 = value; }
		}
		public bool StretchContent2 {
			get { return stretchContent2; }
			set { stretchContent2 = value; }
		}
		public bool StretchContent1SecondarySize {
			get { return stretchContent1SecondarySize; }
			set { stretchContent1SecondarySize = value; }
		}
		public bool StretchContent2SecondarySize {
			get { return stretchContent2SecondarySize; }
			set { stretchContent2SecondarySize = value; }
		}
		public Size CalcBestSize(Size content1Size, Size content2Size) {
			if(content1Size.IsEmpty && content2Size.IsEmpty)
				return Size.Empty;
			if(content1Size.IsEmpty) {
				return new Size(content2Size.Width + Content2Padding.Horizontal, content2Size.Height + Content2Padding.Vertical);	
			}
			else if(content2Size.IsEmpty) {
				return new Size(content1Size.Width + Content1Padding.Horizontal, content1Size.Height + Content1Padding.Vertical);
			}
			if(GetLocation(Content1Location) == ItemLocation.Left || GetLocation(Content1Location) == ItemLocation.Right) {
				return new Size(content1Size.Width + content2Size.Width + HorizontalPadding.Horizontal + HorizontalIndent, Math.Max(content1Size.Height, content2Size.Height) + HorizontalPadding.Vertical);
			}
			return new Size(Math.Max(content1Size.Width, content2Size.Width) + VerticalPadding.Vertical, content1Size.Height + content2Size.Height + VerticalIndent + VerticalPadding.Vertical);
		}
		ItemLocation GetLocation(ItemLocation loc) {
			if(loc == ItemLocation.Default)
				return ItemLocation.Left;
			return loc;
		}
		ItemHorizontalAlignment GetAlignment(ItemHorizontalAlignment horz) {
			if(horz == ItemHorizontalAlignment.Default)
				return ItemHorizontalAlignment.Stretch;
			return horz;
		}
		ItemVerticalAlignment GetAlignment(ItemVerticalAlignment vert) {
			if(vert == ItemVerticalAlignment.Default)
				return ItemVerticalAlignment.Stretch;
			return vert;
		}
		public void ArrangeItems(Rectangle bounds, Size content1Size, Size content2Size, ref Rectangle content1Bounds, ref Rectangle content2Bounds) {
			Rectangle content = bounds;
			content1Bounds = Rectangle.Empty;
			content2Bounds = Rectangle.Empty;
			if(content1Size.IsEmpty && content2Size.IsEmpty)
				return;
			if(content2Size.IsEmpty) {
				content = ApplyPadding(bounds, Content1Padding);
				if(StretchContent1)
					content1Bounds = content;
				else content1Bounds = ArrangeItem(content, content1Size, Content1HorizontalAlignment, Content1VerticalAlignment);
				return;
			}
			if(content1Size.IsEmpty) {
				content = ApplyPadding(bounds, Content2Padding);
				if(StretchContent2)
					content2Bounds = content;
				else content2Bounds = ArrangeItem(content, content2Size, Content2HorizontalAlignment, Content2VerticalAlignment);
				return;
			}
			if(GetLocation(Content1Location) == ItemLocation.Left || GetLocation(Content1Location) == ItemLocation.Right) {
				content = ApplyPadding(bounds, HorizontalPadding);
				content1Bounds.Height = content.Height;
				content2Bounds.Height = content.Height;
				if(StretchContent1) {
					if(StretchContent2) {
						content1Bounds.Width = Math.Max((content.Width - HorizontalIndent) / 2, content1Size.Width);
						content2Bounds.Width = Math.Max((content.Width - HorizontalIndent) / 2, content2Size.Width);
					}
					else {
						content1Bounds.Width = content.Width - content2Size.Width - HorizontalIndent;
						content2Bounds.Width = content2Size.Width;
					}
				}
				else {
					if(StretchContent2) {
						content1Bounds.Width = content1Size.Width;
						content2Bounds.Width = content.Width - content1Size.Width - HorizontalIndent;
					}
					else {
						content1Bounds.Width = content1Size.Width;
						content2Bounds.Width = content2Size.Width;
					}
				}
				if(StretchContent1SecondarySize) {
					content1Bounds.Height = content.Height;
					content1Size.Height = content.Height;
				}
				if(StretchContent2SecondarySize) {
					content2Bounds.Height = content.Height;
					content2Size.Height = content.Height;
				}
			}
			else {
				content = ApplyPadding(bounds, VerticalPadding);
				content1Bounds.Width = content.Width;
				content2Bounds.Width = content.Width;
				if(StretchContent1) {
					if(StretchContent2) {
						content1Bounds.Height = Math.Max((content.Height - VerticalIndent) / 2, content1Size.Height);
						content2Bounds.Height = Math.Max((content.Height - VerticalIndent) / 2, content2Size.Height);
					}
					else {
						content1Bounds.Height = content.Height - content2Size.Height - VerticalIndent;
						content2Bounds.Height = content2Size.Height;
					}
				}
				else {
					if(StretchContent2) {
						content1Bounds.Height = content1Size.Height;
						content2Bounds.Height = content.Height - content1Size.Height - VerticalIndent;
					}
					else {
						content1Bounds.Height = content1Size.Height;
						content2Bounds.Height = content2Size.Height;
					}
				}
				if(StretchContent1SecondarySize) {
					content1Bounds.Width = content.Width;
					content1Size.Width = content.Width;
				}
				if(StretchContent2SecondarySize) {
					content2Bounds.Width = content.Width;
					content2Size.Width = content.Width;
				}
			}
			if(GetLocation(Content1Location) == ItemLocation.Left) {
				content1Bounds.X = content.X;
				content1Bounds.Y = content.Y;
				content2Bounds.X = content1Bounds.Right + HorizontalIndent;
				content2Bounds.Y = content.Y;
			}
			else if(GetLocation(Content1Location) == ItemLocation.Right) {
				content2Bounds.X = content.X;
				content2Bounds.Y = content.Y;
				content1Bounds.X = content2Bounds.Right + HorizontalIndent;
				content1Bounds.Y = content.Y;
			}
			else if(GetLocation(Content1Location) == ItemLocation.Top) {
				content1Bounds.X = content.X;
				content1Bounds.Y = content.Y;
				content2Bounds.X = content.X;
				content2Bounds.Y = content1Bounds.Bottom + VerticalIndent;
			}
			else {
				content2Bounds.X = content.X;
				content2Bounds.Y = content.Y;
				content1Bounds.X = content.X;
				content1Bounds.Y = content2Bounds.Bottom + VerticalIndent;
			}
			content1Bounds = ArrangeItem(content1Bounds, content1Size, Content1HorizontalAlignment, Content1VerticalAlignment);
			content2Bounds = ArrangeItem(content2Bounds, content2Size, Content2HorizontalAlignment, Content2VerticalAlignment);
		}
		Rectangle ApplyPadding(Rectangle bounds, Padding padding) {
			bounds.Width -= padding.Horizontal;
			bounds.Height -= padding.Vertical;
			bounds.X += padding.Left;
			bounds.Y += padding.Top;
			return bounds;
		}
		Rectangle ArrangeItem(Rectangle bounds, Size itemSize, ItemHorizontalAlignment horz, ItemVerticalAlignment vert) { 
			Rectangle itemBounds = bounds;
			switch(GetAlignment(horz)) { 
				case ItemHorizontalAlignment.Left:
					itemBounds.Width = itemSize.Width;
					break;
				case ItemHorizontalAlignment.Center:
					itemBounds.X = bounds.X + (bounds.Width - itemSize.Width) / 2;
					itemBounds.Width = itemSize.Width;
					break;
				case ItemHorizontalAlignment.Right:
					itemBounds.X = bounds.Right - itemSize.Width;
					itemBounds.Width = itemSize.Width;
					break;
			}
			switch(GetAlignment(vert)) { 
				case ItemVerticalAlignment.Top:
					itemBounds.Height = itemSize.Height;
					break;
				case ItemVerticalAlignment.Center:
					itemBounds.Y = bounds.Y + (bounds.Height - itemSize.Height) / 2;
					itemBounds.Height = itemSize.Height;
					break;
				case ItemVerticalAlignment.Bottom:
					itemBounds.Y = bounds.Bottom - itemSize.Height;
					itemBounds.Height = itemSize.Height;
					break;
			}
			return itemBounds;
		}
	}
	public class SelectionPainter { 
		static SelectionPainter defaultPainter;
		public static SelectionPainter Default {
			get {
				if(defaultPainter == null)
					defaultPainter = new SelectionPainter();
				return defaultPainter;
			}
		}
		public void Draw(GraphicsCache cache, ISkinProvider provider, Rectangle rect) {
			Draw(cache, provider, rect, null);
		}
		public void Draw(GraphicsCache cache, ISkinProvider provider, Rectangle rect, SkinPaddingEdges borderThickness) {
			SkinElement elem = CommonSkins.GetSkin(provider)[CommonSkins.SkinSelection];
			object obj = elem.Properties[CommonSkins.SkinSelectionOpacity];
			int opactity = obj == null ? 40 : (int)obj;
			if(!elem.Color.BackColor.Equals(Color.Empty))
				elem.Color.BackColor = Color.FromArgb(opactity, elem.Color.BackColor);
			if(!elem.Color.BackColor2.Equals(Color.Empty))
				elem.Color.BackColor2 = Color.FromArgb(opactity, elem.Color.BackColor2);
			if(!elem.Border.Top.Equals(Color.Empty))
				elem.Border.Top = Color.FromArgb(opactity, elem.Border.Top);
			if(!elem.Border.Left.Equals(Color.Empty))
				elem.Border.Left = Color.FromArgb(opactity, elem.Border.Left);
			if(!elem.Border.Bottom.Equals(Color.Empty))
				elem.Border.Bottom = Color.FromArgb(opactity, elem.Border.Bottom);
			if(!elem.Border.Right.Equals(Color.Empty))
				elem.Border.Right = Color.FromArgb(opactity, elem.Border.Right);
			SkinPaddingEdges prevThickness = elem.Border.Thin;
			if(borderThickness != null)
				elem.Border.Thin = borderThickness;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, new SkinElementInfo(elem, rect));
			elem.Border.Thin = prevThickness;
		}
	}
	public static class ImageColorizer {
		[ThreadStatic]
		static ImageAttributes attributes;
		static float[][] matrix;
		static ColorMatrix GetColorMatrix(Color color) {
			if(matrix == null)
				matrix = new float[][] { 
					new float[] { - 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, - 1.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, - 1.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f  },
					new float[] { 1.0f, 1.0f, 1.0f, 0.0f, 1.0f }
			};
			matrix[0][0] = color.R / 255.0f;
			matrix[1][1] = color.G / 255.0f;
			matrix[2][2] = color.B / 255.0f;			
			matrix[4][0] = color.R / 255.0f;
			matrix[4][1] = color.G / 255.0f;
			matrix[4][2] = color.B / 255.0f;
			return new ColorMatrix(matrix);
		}
		public static ImageAttributes GetColoredAttributes(Color color, int opacity) {
			if(attributes == null)
				attributes = new ImageAttributes();
			ColorMatrix matrix = GetColorMatrix(color);
			matrix.Matrix33 = opacity / 255.0f;
			attributes.SetColorMatrix(matrix);
			return attributes;
		}
		public static ImageAttributes GetColoredAttributes(Color color) {
			if(attributes == null)
				attributes = new ImageAttributes();
			attributes.SetColorMatrix(GetColorMatrix(color));
			return attributes;
		}
		public static Image GetColoredImage(Image image, Color color) {
			if(image == null)
				return image;
			Bitmap img = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
			img.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using(Graphics g = Graphics.FromImage(img)) {
				g.DrawImage(image, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, GetColoredAttributes(color)); 
			}
			return img;
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public class DefaultSkinProvider : ISkinProvider {
		#region ISkinProvider Members
		static DefaultSkinProvider provider = new DefaultSkinProvider();
		public static DefaultSkinProvider Default { get { return provider; } }
		string ISkinProvider.SkinName {
			get { return "DevExpress Style"; }
		}
		#endregion
	}
}
