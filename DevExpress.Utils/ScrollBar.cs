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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Internal;
namespace DevExpress.XtraEditors.ViewInfo {
	public enum ScrollBarState {Normal, IncButtonPressed, DecButtonPressed, ThumbPressed, 
		IncAreaPressed, DecAreaPressed, IncButtonHot, DecButtonHot, ThumbHot}
	public enum ScrollBarVisibility{
		Visible = 0,
		Hidden = 1,
	}
	public class ScrollBarArrowArgs: ObjectInfoArgs {
		Color normalColor, disabledColor;
		ScrollButton scrollButton;
		int pressedShift;
		public ScrollBarArrowArgs(GraphicsCache cache, 
			Rectangle bounds, ObjectState state, ScrollButton scrollButton): base(cache, bounds, state) {
			this.normalColor = SystemColors.ControlText;
			this.disabledColor = SystemColors.GrayText;
			this.scrollButton = scrollButton;
			this.pressedShift = 1;
		}
		public Color NormalColor { get { return this.normalColor; } set { this.normalColor = value; } }
		public Color DisabledColor { get { return this.disabledColor; } set { this.disabledColor = value; } }
		public ScrollButton ScrollButton { get { return this.scrollButton; } set { this.scrollButton = value; } }
		public int PressedShift { get { return this.pressedShift; } set { this.pressedShift = value; } }
		public Brush GetBrush() {
			return Cache.GetSolidBrush(ObjectState.Disabled == State ? DisabledColor : NormalColor);
		}
		public Pen GetPen() {
			return Cache.GetPen(ObjectState.Disabled == State ? DisabledColor : NormalColor);
		}
	}
	public class ScrollBarArrowPainter: ObjectPainter {
		static Pen lightLightPen = new Pen(SystemBrushes.ControlLightLight);
		public override void DrawObject (ObjectInfoArgs e) {
			ScrollBarArrowArgs sArgs = e as ScrollBarArrowArgs;
			Point[] points = CalculateArrowPoints(sArgs);
			if(ObjectState.Disabled == e.State) {
				for (int i = 0; i < points.Length; i ++)
					points[i].Offset(1, 1);
				DrawArrow(e, SystemBrushes.ControlLightLight, lightLightPen, points);
				for (int i = 0; i < points.Length; i ++)
					points[i].Offset(-1, -1);
			}
			DrawArrow(e, sArgs.GetBrush(), sArgs.GetPen(), points);
		}
		private void DrawArrow(ObjectInfoArgs e, Brush brush, Pen pen, Point[] points) {
			e.Graphics.DrawPolygon(pen, points);
			e.Graphics.FillPolygon(brush, points);
		}
		private Point[] CalculateArrowPoints(ScrollBarArrowArgs e) {
			Rectangle bounds = e.Bounds;
			Point[] points = {new Point(0), new Point(0), new Point(0)};
			if(ObjectState.Pressed == e.State)
				bounds.Offset(e.PressedShift, e.PressedShift);
			int delta = bounds.Width - bounds.Height;
			if(delta > 0) 
				bounds.Inflate(-delta / 2, 0);
			else bounds.Inflate(0, delta / 2);
			bounds = GetArrowBounds(bounds, e.ScrollButton);
			int middleLeftRight = bounds.Left + bounds.Width / 2;
			int middleTopBottom = bounds.Top + bounds.Height / 2;
			switch (e.ScrollButton) {
				case ScrollButton.Up: 
					points[0].X = middleLeftRight; points[0].Y = bounds.Top;
					points[1].X = bounds.Right; points[1].Y = bounds.Bottom;
					points[2].X = bounds.Left; points[2].Y = bounds.Bottom;
					break;
				case ScrollButton.Down:
					bounds.Offset(0, 1);
					points[0].X = middleLeftRight; points[0].Y = bounds.Bottom;
					points[1].X = bounds.Right; points[1].Y = bounds.Top;
					points[2].X = bounds.Left; points[2].Y = bounds.Top;
					break;
				case ScrollButton.Left:
					points[0].X = bounds.Left; points[0].Y = middleTopBottom;
					points[1].X = bounds.Right; points[1].Y = bounds.Top;
					points[2].X = bounds.Right; points[2].Y = bounds.Bottom;
					break;
				case ScrollButton.Right:
					bounds.Offset(1, 0);
					points[0].X = bounds.Right; points[0].Y = middleTopBottom;
					points[1].X = bounds.Left; points[1].Y = bounds.Top;
					points[2].X = bounds.Left; points[2].Y = bounds.Bottom;
					break;
			}
			return points;
		}
		private Rectangle GetArrowBounds(Rectangle bounds, ScrollButton button) {
			Rectangle r = bounds;
			Size size = GetArrowSize(bounds, button);
			r.X += (r.Width - size.Width) / 2 - 1;
			r.Y += (r.Height - size.Height) / 2 - 1;
			r.Width = size.Width; r.Height = size.Height;
			if((ScrollButton.Up == button) || (ScrollButton.Down == button)) {
				r.Width ++;
			} else {
				r.Height ++;
			}
			return r;
		}
		private Size GetArrowSize(Rectangle bounds, ScrollButton button) {
			Size size = new Size(0, 0);
			if((ScrollButton.Up == button) || (ScrollButton.Down == button)) {
				size.Width = (bounds.Width - 1) / 2 - 3;
				if(size.Width % 2 == 0) size.Width ++;
				size.Height = size.Width / 2 + 1;
			} else {
				size.Height = (bounds.Height - 1) / 2 - 3;
				if( size.Height % 2 == 0) size.Height ++;
				size.Width = size.Height / 2 + 1;
			}
			return size;
		}
	}
	public class ScrollBarButtonInfoArgs : StyleObjectInfoArgs {
		public ScrollButton ScrollButton;
		public ScrollBarButtonInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject style, AppearanceObject backstyle,
			ScrollButton scrollButton, ObjectState state): base(cache, bounds, style, backstyle, state) {
			this.ScrollButton = scrollButton;
		}
	}
	public abstract class ScrollBarButtonPainterBase: ObjectPainter {
		ObjectPainter buttonPainter;
		ScrollBarArrowPainter arrowPainter;
		public ScrollBarButtonPainterBase() {
			this.arrowPainter = new ScrollBarArrowPainter();
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ScrollBarButtonInfoArgs sbe = (e as ScrollBarButtonInfoArgs);
			ObjectInfoArgs bArgs = CreateInfoArgs(sbe);
			ButtonPainter.DrawObject(bArgs);
			if(NeedToDrawArrow) {
				ScrollBarArrowArgs aArgs = new ScrollBarArrowArgs(e.Cache, e.Bounds, e.State, sbe.ScrollButton);
				this.arrowPainter.DrawObject(aArgs);
			}
		}
		public ObjectPainter ButtonPainter { 
			get { 
				if(buttonPainter == null) buttonPainter = CreateButtonPainter();
				return buttonPainter;
			} 
		}
		protected abstract ObjectPainter CreateButtonPainter();
		protected virtual ObjectInfoArgs CreateInfoArgs(ScrollBarButtonInfoArgs e)  {
			return new StyleObjectInfoArgs(e.Cache, e.Bounds, e.Appearance, e.BackAppearance, e.State);
		}
		protected virtual bool NeedToDrawArrow { get { return true; } }
	}
	public class ScrollBarThumbInfoArgs: StyleObjectInfoArgs {
		bool isFlashing;
		ScrollBarType scrollBarType;
		public ScrollBarThumbInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject style, AppearanceObject backStyle,
			ObjectState state, bool isFlashing, ScrollBarType scrollBarType): base(cache, bounds, style, backStyle, state) {
			this.isFlashing = isFlashing;
			this.scrollBarType = scrollBarType;
		}
		public bool IsFlashing { get { return this.isFlashing; } set { this.isFlashing = value; } }
		public ScrollBarType ScrollBarType { get { return this.scrollBarType; } }
		public ScrollBarDrawMode ScrollBehavior { get; set; }
	}
	public class ScrollBarThumbButtonInfoArgs: StyleObjectInfoArgs {
		static Brush flashingBrush = new HatchBrush(HatchStyle.Percent50, Color.Black, Color.White);
		bool isFlashing;
		ScrollBarType scrollBarType;
		public ScrollBarThumbButtonInfoArgs(GraphicsCache cache, Rectangle bounds, 
			AppearanceObject style, AppearanceObject backStyle,
			ObjectState state, bool isFlashing, ScrollBarType scrollBarType): base(cache, bounds, style, backStyle, state) {
			this.isFlashing = isFlashing;
			this.scrollBarType = ScrollBarType;
		}
		public bool IsFlashing { get { return this.isFlashing; } set { this.isFlashing = value; } }
		public Brush FlashingBrush { get { return flashingBrush; } }
		public ScrollBarType ScrollBarType { get { return this.scrollBarType; } }
	}
	public abstract class ScrollBarThumbPainterBase: ObjectPainter {
		ObjectPainter buttonPainter = null;
		public ScrollBarThumbPainterBase() { }
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectInfoArgs bArgs = CreateInfoArgs(e);
			if((! CanBePressed) && (ObjectState.Pressed == bArgs.State))
				bArgs.State = ObjectState.Normal;
			ButtonPainter.DrawObject(bArgs);
		}
		public ObjectPainter ButtonPainter { 
			get { 
				if(buttonPainter == null) buttonPainter = CreateButtonPainter();
				return buttonPainter; 
			}
		}
		protected abstract ObjectPainter CreateButtonPainter();
		protected virtual ObjectInfoArgs CreateInfoArgs(ObjectInfoArgs e) {
			return new ScrollBarThumbButtonInfoArgs(e.Cache, e.Bounds, 
				(e as ScrollBarThumbInfoArgs).Appearance, (e as ScrollBarThumbInfoArgs).BackAppearance,
				e.State, (e as ScrollBarThumbInfoArgs).IsFlashing, 
				(e as ScrollBarThumbInfoArgs).ScrollBarType);
		}
		protected virtual bool CanBePressed { get { return false; } }
	}
	public class ScrollBarInfoArgs: StyleObjectInfoArgs {
		IScrollBar scrollBar;
		Rectangle thumbButtonBounds;
		public ScrollBarInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject style, AppearanceObject backStyle, IScrollBar scrollBar): 
			base(cache, bounds, style, backStyle, ObjectState.Normal) {
			this.scrollBar = scrollBar;
			this.thumbButtonBounds = ViewInfo.ThumbButtonBounds;
		}
		public ISkinProvider SkinProvider { get { return ScrollBar.LookAndFeel.ActiveLookAndFeel; } }
		public IScrollBar ScrollBar { get { return this.scrollBar; } }
		public ScrollBarViewInfo ViewInfo { get { return ScrollBar.ViewInfo; } }
		public ScrollBarState ScrollBarState { get { return ViewInfo.State; } }
		public Rectangle DecButtonBounds { get { return ViewInfo.DecButtonBounds; } }
		public Rectangle IncButtonBounds { get { return ViewInfo.IncButtonBounds; } }
		public Rectangle ThumbButtonBounds { get { return thumbButtonBounds; } set { thumbButtonBounds = value; }  }
	}
	public abstract class ScrollBarPainterBase: ObjectPainter {
		Brush scrollAreaBrush, pressedScrollAreaBrush;
		ObjectPainter _buttonPainter = null;
		ObjectPainter _thumbPainter = null;
		public ScrollBarPainterBase() {
			this.scrollAreaBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.Control, SystemColors.Window);
			this.pressedScrollAreaBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlDarkDark, SystemColors.WindowText);
		}
		public void DestroyBrushes() {
			if(scrollAreaBrush != null) scrollAreaBrush.Dispose();
			if(pressedScrollAreaBrush != null) pressedScrollAreaBrush.Dispose();
		}
		public virtual int ThumbMinWidth { get { return System.Windows.Forms.SystemInformation.HorizontalScrollBarThumbWidth; } }
		public override void DrawObject (ObjectInfoArgs e) {
			ScrollBarInfoArgs sbe = e as ScrollBarInfoArgs;
			if(sbe.ScrollBar.DrawMode == ScrollBarDrawMode.TouchMode)
				DrawScrollTouchBackground(sbe);
			if(! sbe.IncButtonBounds.IsEmpty)
				DrawArrowButtons(sbe);
			if(! sbe.ThumbButtonBounds.IsEmpty)
				DrawThumbButton(sbe);
			DrawScrollArea(sbe);
		}
		protected virtual void DrawScrollTouchBackground(ScrollBarInfoArgs e) {
			if((e.State & ObjectState.Selected) == 0) return;
			using(var brush = new SolidBrush(Color.FromArgb(50, Color.LightGray))) {
				e.Graphics.FillRectangle(brush, e.Bounds);
			}
		}
		protected ObjectPainter ThumbPainter {
			get {
				if(_thumbPainter == null) _thumbPainter = CreateScrollBarThumbPainter();
				return _thumbPainter;
			}
		}
		protected ObjectPainter ButtonPainter {
			get {
				if(_buttonPainter == null) _buttonPainter = CreateScrollBarButtonPainter();
				return _buttonPainter;
			}
		}
		protected abstract ObjectPainter CreateScrollBarButtonPainter();
		protected abstract ObjectPainter CreateScrollBarThumbPainter();
		protected virtual Color ButtonBackColor { get { return SystemColors.Control; } }
		protected virtual void DrawNormalScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			e.Cache.Paint.FillRectangle(e.Graphics, ScrollAreaBrush, bounds);
		}
		protected virtual void DrawPressedScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			e.Cache.Paint.FillRectangle(e.Graphics, PressedScrollAreaBrush, bounds);
		}
		protected virtual Brush ScrollAreaBrush { get {	return scrollAreaBrush;	} }
		protected virtual Brush PressedScrollAreaBrush { get {	return pressedScrollAreaBrush;	} }
		private void DrawArrowButtons(ScrollBarInfoArgs e) {
			ScrollBarButtonInfoArgs incArgs = new ScrollBarButtonInfoArgs(e.Cache, e.IncButtonBounds, 
				e.Appearance, e.BackAppearance, GetIncScrollButton(e), GetIncButtonState(e));
			e.Appearance.BackColor = ButtonBackColor;
			ScrollBarButtonInfoArgs decArgs = new ScrollBarButtonInfoArgs(e.Cache, e.DecButtonBounds, 
				e.Appearance, e.BackAppearance, GetDecScrollButton(e), GetDecButtonState(e));
			ButtonPainter.DrawObject(incArgs);
			ButtonPainter.DrawObject(decArgs);
		}
		protected ScrollBarThumbInfoArgs CreateThumbInfo(ScrollBarInfoArgs e) {
			ScrollBarThumbInfoArgs thumbArgs = new ScrollBarThumbInfoArgs(e.Cache, e.ThumbButtonBounds, e.Appearance, e.BackAppearance,
				GetThumbButtonState(e), e.ViewInfo.IsThumbFlashing, e.ScrollBar.ScrollBarType);
			thumbArgs.ScrollBehavior = e.ScrollBar.DrawMode;
			return thumbArgs;
		}
		protected void DrawThumbButton(ScrollBarInfoArgs e) {
			ThumbPainter.DrawObject(CreateThumbInfo(e));
		}
		private void DrawScrollAreaOld(ScrollBarInfoArgs e) {
			Rectangle decR, incR;
			Rectangle decButtonBounds = e.ViewInfo.VisibleDecButtonBounds;
			Rectangle incButtonBounds = e.ViewInfo.VisibleIncButtonBounds;
			if(e.ThumbButtonBounds.IsEmpty) {
				if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) 
					decR = new Rectangle(decButtonBounds.Right, 0, 
						incButtonBounds.X - decButtonBounds.Right, decButtonBounds.Height);
				else decR = new Rectangle(0, decButtonBounds.Bottom, 
						 decButtonBounds.Width, incButtonBounds.Y - decButtonBounds.Bottom);
				DrawNormalScrollArea(e, decR, true);
			} else {
				if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) 
					decR = new Rectangle(decButtonBounds.Right, 0, 
						e.ThumbButtonBounds.X - decButtonBounds.Right, decButtonBounds.Height);
				else decR = new Rectangle(0, decButtonBounds.Bottom, 
						 decButtonBounds.Width, e.ThumbButtonBounds.Y - decButtonBounds.Bottom);
				if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal)
					incR = new Rectangle(e.ThumbButtonBounds.Right, 0, 
						incButtonBounds.X - e.ThumbButtonBounds.Right, e.ThumbButtonBounds.Height);
				else incR = new Rectangle(0, e.ThumbButtonBounds.Bottom, 
						 e.ThumbButtonBounds.Width, incButtonBounds.Y - e.ThumbButtonBounds.Bottom);
				if(e.ViewInfo.IsRightToLeft){
					Rectangle r = decR;
					decR = incR;
					incR = r;
				}
				if(e.ScrollBarState == ScrollBarState.DecAreaPressed) 
					DrawPressedScrollArea(e, decR, false);
				else DrawNormalScrollArea(e, decR, false);
				if(e.ScrollBarState == ScrollBarState.IncAreaPressed) 
					DrawPressedScrollArea(e, incR, true);
				else DrawNormalScrollArea(e, incR, true);
			}
		}
		private void DrawScrollArea(ScrollBarInfoArgs e) {
			Rectangle decR, incR;
			Rectangle decButtonBounds = e.ViewInfo.VisibleDecButtonBounds;
			Rectangle incButtonBounds = e.ViewInfo.VisibleIncButtonBounds;
			if (e.ThumbButtonBounds.Width == 0 && e.ThumbButtonBounds.Height == 0) {
				if (e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal)
					decR = new Rectangle(decButtonBounds.Right, decButtonBounds.Y,
						incButtonBounds.X - decButtonBounds.Right, decButtonBounds.Height);
				else decR = new Rectangle(decButtonBounds.X, decButtonBounds.Bottom,
						 decButtonBounds.Width, incButtonBounds.Y - decButtonBounds.Bottom);
				DrawNormalScrollArea(e, decR, true);
			}
			else {
				if (e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal)
					decR = new Rectangle(decButtonBounds.Right, decButtonBounds.Y,
						e.ThumbButtonBounds.X - decButtonBounds.Right, decButtonBounds.Height);
				else decR = new Rectangle(decButtonBounds.X, decButtonBounds.Bottom,
						 decButtonBounds.Width, e.ThumbButtonBounds.Y - decButtonBounds.Bottom);
				if (e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal)
					incR = new Rectangle(e.ThumbButtonBounds.Right, decButtonBounds.Y,
						incButtonBounds.X - e.ThumbButtonBounds.Right, e.ThumbButtonBounds.Height);
				else incR = new Rectangle(decButtonBounds.X, e.ThumbButtonBounds.Bottom,
						 e.ThumbButtonBounds.Width, incButtonBounds.Y - e.ThumbButtonBounds.Bottom);
				if (e.ViewInfo.IsRightToLeft) {
					Rectangle r = decR;
					decR = incR;
					incR = r;
				}
				if (e.ScrollBarState == ScrollBarState.DecAreaPressed)
					DrawPressedScrollArea(e, decR, false);
				else DrawNormalScrollArea(e, decR, false);
				if (e.ScrollBarState == ScrollBarState.IncAreaPressed)
					DrawPressedScrollArea(e, incR, true);
				else DrawNormalScrollArea(e, incR, true);
			}
		}
		private ScrollButton GetIncScrollButton(ScrollBarInfoArgs e) {
			if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) {
				if( ! e.ViewInfo.IsRightToLeft)
					return ScrollButton.Right;
				else return ScrollButton.Left;
			}
			else return ScrollButton.Down;
		}
		private ScrollButton GetDecScrollButton(ScrollBarInfoArgs e) {
			if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) {
				if( ! e.ViewInfo.IsRightToLeft)
					return ScrollButton.Left;
				else return ScrollButton.Right;
			}
			else return ScrollButton.Up;
		}
		protected virtual ObjectState GetDecButtonState(ScrollBarInfoArgs e) {
			if( ! e.ViewInfo.Enabled)
				return ObjectState.Disabled;
			if(e.ScrollBarState == ScrollBarState.DecButtonHot)
				return ObjectState.Hot;
			if(e.ScrollBarState == ScrollBarState.DecButtonPressed)
				return ObjectState.Pressed;
			return ObjectState.Normal;
		}
		protected virtual ObjectState GetIncButtonState(ScrollBarInfoArgs e) {
			if( ! e.ViewInfo.Enabled)
				return ObjectState.Disabled;
			if(e.ScrollBarState == ScrollBarState.IncButtonHot)
				return ObjectState.Hot;
			if(e.ScrollBarState == ScrollBarState.IncButtonPressed)
				return ObjectState.Pressed;
			return ObjectState.Normal;
		}
		protected virtual ObjectState GetThumbButtonState(ScrollBarInfoArgs e) {
			if( ! e.ViewInfo.Enabled)
				return ObjectState.Disabled;
			if(e.ScrollBarState == ScrollBarState.ThumbHot) 
				return ObjectState.Hot;
			if(e.ScrollBarState == ScrollBarState.ThumbPressed)
				return ObjectState.Pressed;
			return ObjectState.Normal;
		}
		protected internal virtual int GetButtonCornerSize(ScrollBarType scrollType) {
			return 0;
		}
		protected internal virtual void UpdateThumbBounds(ScrollBarInfoArgs args) {
		}
	}
	public class Style3DScrollBarButtonPainter: ScrollBarButtonPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new Style3DButtonObjectPainter();
		}
	}
	public class Style3DScrollBarThumbButtonPainter: Style3DButtonObjectPainter {
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			base.DrawButtonBackground(e, style, r);
			if((e as ScrollBarThumbButtonInfoArgs).IsFlashing) 
				e.Cache.Paint.FillRectangle(e.Graphics, (e as ScrollBarThumbButtonInfoArgs).FlashingBrush, r);
		}
	}
	public class Style3DScrollBarThumbPainter: ScrollBarThumbPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new Style3DScrollBarThumbButtonPainter();
		}
	}
	public class Style3DScrollBarPainter: ScrollBarPainterBase {
		protected override ObjectPainter CreateScrollBarButtonPainter() {
			return new Style3DScrollBarButtonPainter();
		}
		protected override ObjectPainter CreateScrollBarThumbPainter() {
			return new Style3DScrollBarThumbPainter();
		}
	}
	public class FlatScrollBarButtonPainter: ScrollBarButtonPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new FlatButtonObjectPainter();
		}
	}
	public class FlatScrollBarThumbButtonPainter: FlatButtonObjectPainter {
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			base.DrawButtonBackground(e, style, r);
			if((e as ScrollBarThumbButtonInfoArgs).IsFlashing) 
				e.Cache.Paint.FillRectangle(e.Graphics, (e as ScrollBarThumbButtonInfoArgs).FlashingBrush, r);
		}
	}
	public class FlatScrollBarThumbPainter: ScrollBarThumbPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new FlatScrollBarThumbButtonPainter();
		}
	}
	public class FlatScrollBarPainter: ScrollBarPainterBase {
		protected override ObjectPainter CreateScrollBarButtonPainter() {
			return new FlatScrollBarButtonPainter();
		}
		protected override ObjectPainter CreateScrollBarThumbPainter() {
			return new FlatScrollBarThumbPainter();
		}
	}
	public class UltraFlatScrollBarButtonPainter: ScrollBarButtonPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new UltraFlatButtonObjectPainter();
		}
	}
	public class UltraFlatScrollBarThumbButtonPainter: UltraFlatButtonObjectPainter {
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			base.DrawButtonBackground(e, style, r);
			if((e as ScrollBarThumbButtonInfoArgs).IsFlashing) 
				e.Cache.Paint.FillRectangle(e.Graphics, (e as ScrollBarThumbButtonInfoArgs).FlashingBrush, r);
		}
	}
	public class UltraFlatScrollBarThumbPainter: ScrollBarThumbPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new UltraFlatScrollBarThumbButtonPainter();
		}
	}
	public class UltraFlatScrollBarPainter: ScrollBarPainterBase {
		protected override ObjectPainter CreateScrollBarButtonPainter() {
			return new UltraFlatScrollBarButtonPainter();
		}
		protected override ObjectPainter CreateScrollBarThumbPainter() {
			return new UltraFlatScrollBarThumbPainter();
		}
	}
	public class SkinScrollBarPainter : ScrollBarPainterBase {
		ISkinProvider provider;
		public SkinScrollBarPainter(ISkinProvider provider) { this.provider = provider; }
		public ISkinProvider Provider { get { return provider; } }
		public override int ThumbMinWidth { get { return 14; } }
		protected override void DrawScrollTouchBackground(ScrollBarInfoArgs e) {
			if((e.State & ObjectState.Selected) == 0) return;
			SkinElementInfo info = new SkinElementInfo(GetSkinElementTouchBackground(e), e.Bounds);
			if(info.Element == null) {
				base.DrawScrollTouchBackground(e);
				return;
			}
			info.State = e.State;
			info.ImageIndex = -1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		SkinElement GetSkinElementTouchBackground(ScrollBarInfoArgs e) {
			return CommonSkins.GetSkin(Provider)[e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal ?
				CommonSkins.SkinScrollTouchBackground : CommonSkins.SkinScrollTouchBackgroundVert];
		}
		protected override ObjectPainter CreateScrollBarButtonPainter() {
			return new SkinScrollBarButtonPainter(Provider);
		}
		protected override ObjectPainter CreateScrollBarThumbPainter() {
			return new SkinScrollBarThumbPainter(Provider);
		}
		SkinElement GetSkinElement(ScrollBarInfoArgs e) {
			return CommonSkins.GetSkin(Provider)[e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal ?
				CommonSkins.SkinScrollShaft : CommonSkins.SkinScrollShaftVert];
		}
		protected new SkinScrollBarThumbPainter ThumbPainter { get { return (SkinScrollBarThumbPainter)base.ThumbPainter; } } 
		protected internal override void UpdateThumbBounds(ScrollBarInfoArgs e) {
			if(e.ScrollBar.DrawMode != ScrollBarDrawMode.TouchMode) return;
			Rectangle bounds = e.Bounds;
			Rectangle thumbBounds = e.ViewInfo.ThumbButtonBounds;
			SkinElementInfo thumbInfo = ThumbPainter.GetSkinInfo(CreateThumbInfo(e));
			if(thumbInfo == null || thumbInfo.Element == null) return;
			thumbBounds = thumbInfo.Element.Offset.GetBounds(thumbBounds, thumbBounds.Size, SkinOffsetKind.Near);
			if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) {
				if(!thumbInfo.Element.Size.AllowVGrow) {
					thumbBounds.Height = thumbInfo.Element.Size.MinSize.Height;
					thumbBounds.Y += (bounds.Height - thumbBounds.Height) / 2;
				}
			}
			else {
				if(!thumbInfo.Element.Size.AllowHGrow) {
					thumbBounds.Width = thumbInfo.Element.Size.MinSize.Width;
					thumbBounds.X += (bounds.Width - thumbBounds.Width) / 2;
				}
			}
			e.ThumbButtonBounds = thumbBounds;
		}
		protected override void DrawNormalScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			SkinElementInfo info = new SkinElementInfo(GetSkinElement(e), bounds);
			info.State = e.State;
			info.ImageIndex = -1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawPressedScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			SkinElementInfo info = new SkinElementInfo(GetSkinElement(e), bounds);
			info.State = ObjectState.Pressed;
			info.ImageIndex = -1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected internal override int GetButtonCornerSize(ScrollBarType scrollType) {
			SkinElement skin = CommonSkins.GetSkin(Provider)[scrollType == ScrollBarType.Horizontal ? CommonSkins.SkinScrollButtonThumb : CommonSkins.SkinScrollButtonThumbVert];
			return skin.Properties.GetInteger(CommonSkins.OptScrollButtonCorner);
		}
	}
	public class SkinScrollBarButtonPainter : SkinButtonObjectPainter {
		public SkinScrollBarButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			ScrollBarButtonInfoArgs ee = e as ScrollBarButtonInfoArgs;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinScrollButton], e.Bounds);
			info.State = e.State;
			info.Cache = e.Cache;
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(info.Element.Image, e.State);
			switch(ee.ScrollButton) {
				case ScrollButton.Down : info.ImageIndex += 4; break;
				case ScrollButton.Left : info.ImageIndex += 8; break;
				case ScrollButton.Right : info.ImageIndex += 12; break;
			}
			return info;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.Control)), e.Bounds);
			base.DrawObject(e);
		}
	}
	public class SkinScrollBarThumbPainter : SkinButtonObjectPainter {
		public SkinScrollBarThumbPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			ScrollBarThumbInfoArgs ee = e as ScrollBarThumbInfoArgs;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[ee.ScrollBarType == ScrollBarType.Horizontal ? CommonSkins.SkinScrollButtonThumb : CommonSkins.SkinScrollButtonThumbVert], e.Bounds);
			if(ee.ScrollBehavior == ScrollBarDrawMode.TouchMode) {
				info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[ee.ScrollBarType == ScrollBarType.Horizontal ? CommonSkins.SkinScrollButtonTouchThumb : CommonSkins.SkinScrollButtonTouchThumbVert], e.Bounds);
				if(info.Element == null) return info;
			}
			info.State = e.State;
			info.Cache = e.Cache;
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(info.Element.Image, e.State);
			return info;
		}
		public SkinElementInfo GetSkinInfo(ObjectInfoArgs e) {
			return UpdateInfo(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ScrollBarThumbInfoArgs ee = e as ScrollBarThumbInfoArgs;
			SkinElementInfo se = UpdateInfo(e);
			if(se.Element == null)
				DrawTouchDefaultThumb(e);
			else {
				SkinElementPainter.Default.DrawObject(se);
			}
		}
		protected virtual void DrawTouchDefaultThumb(ObjectInfoArgs e) {
			ScrollBarThumbInfoArgs ee = e as ScrollBarThumbInfoArgs;
			Rectangle bounds = e.Bounds;
			if(bounds.Width == 0 || bounds.Height == 0) return;
			int size = ee.ScrollBarType == ScrollBarType.Horizontal ? (bounds.Height / 3) : (bounds.Width / 3);
			size++;
			if(ee.ScrollBarType == ScrollBarType.Horizontal) {
				bounds.Y += (bounds.Height - size) / 2;
				bounds.Height = size;
			}
			else {
				bounds.X += (bounds.Width - size) / 2;
				bounds.Width = size;
			}
			var dark = e.Cache.GetSolidBrush(Color.FromArgb(150, Color.DarkGray));
			var light = e.Cache.GetPen(Color.FromArgb(200, Color.White));
			e.Cache.DrawRectangle(light, bounds);
			bounds.Inflate(-1, -1);
			e.Cache.FillRectangle(dark, bounds);
		}
	}
	public class XPScrollBarButtonPainter : ScrollBarButtonPainterBase {
		protected override ObjectPainter CreateButtonPainter() {
			return new XPAdvButtonPainter();
		}
		protected override ObjectInfoArgs CreateInfoArgs(ScrollBarButtonInfoArgs e)  {
			XPAdvButtonInfoArgs args = new XPAdvButtonInfoArgs(GetButton(e.ScrollButton));
			args.Cache = e.Cache;
			args.Bounds = e.Bounds;
			args.State = e.State;
			return args;
		}
		protected override bool NeedToDrawArrow { get { return false; } }
		private ButtonPredefines GetButton(ScrollButton scrollButton) {
			switch(scrollButton) {
				case ScrollButton.Up: return ButtonPredefines.Up;
				case ScrollButton.Left: return ButtonPredefines.Left;
				case ScrollButton.Right: return ButtonPredefines.Right;
			}
			return ButtonPredefines.Down;
		}
	}
	public class XPScrollBarPartInfoArgs: ObjectInfoArgs {
		int partId;
		public XPScrollBarPartInfoArgs(GraphicsCache cache, Rectangle bounds, 
			ObjectState state, int partId): base(cache, bounds, state) {
			this.partId = partId;
		}
		public int PartId { get { return this.partId; } }
	}
	public class XPScrollBarPartPainter: XPObjectPainter {
		public XPScrollBarPartPainter() {
			DrawArgs = new WXPPainterArgs("scrollbar", 0, 0);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			DrawArgs.PartId = (e as XPScrollBarPartInfoArgs).PartId;
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			return CalcButtonStateId(e);
		}
	}
	public class XPScrollBarThumbPainter: ScrollBarThumbPainterBase {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			int partId = (e as ScrollBarThumbInfoArgs).ScrollBarType == ScrollBarType.Horizontal ?
				XPConstants.SBP_GRIPPERHORZ : XPConstants.SBP_GRIPPERVERT;
			XPScrollBarPartInfoArgs args = new XPScrollBarPartInfoArgs(e.Cache, e.Bounds, e.State, partId);
			Rectangle minBounds = ButtonPainter.CalcObjectMinBounds(args);
			if((e.Bounds.Width > minBounds.Width) && (e.Bounds.Height > minBounds.Height))
				ButtonPainter.DrawObject(args);
		}
		protected override ObjectPainter CreateButtonPainter() {
			return new XPScrollBarPartPainter();
		}
		protected override ObjectInfoArgs CreateInfoArgs(ObjectInfoArgs e) {
			int partId = (e as ScrollBarThumbInfoArgs).ScrollBarType == ScrollBarType.Horizontal ?
				XPConstants.SBP_THUMBBTNHORZ: XPConstants.SBP_THUMBBTNVERT;
			return new XPScrollBarPartInfoArgs(e.Cache, e.Bounds, e.State, partId);
		}
		protected override bool CanBePressed { get { return true; } }
	}
	public class XPScrollBarPainter: ScrollBarPainterBase {
		XPScrollBarPartPainter partPainter;
		public XPScrollBarPainter() {
			partPainter = new XPScrollBarPartPainter();
		}
		protected override ObjectPainter CreateScrollBarButtonPainter() {
			return new XPScrollBarButtonPainter();
		}
		protected override ObjectPainter CreateScrollBarThumbPainter() {
			return new XPScrollBarThumbPainter();
		}
		protected override void DrawNormalScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			XPScrollBarPartInfoArgs args = new XPScrollBarPartInfoArgs(e.Cache, bounds, 
				ObjectState.Normal, GetScrollBarPartId(e, isInc));
			partPainter.DrawObject(args);
		}
		protected override void DrawPressedScrollArea(ScrollBarInfoArgs e, Rectangle bounds, bool isInc) {
			XPScrollBarPartInfoArgs args = new XPScrollBarPartInfoArgs(e.Cache, bounds, 
				ObjectState.Pressed, GetScrollBarPartId(e, isInc));
			partPainter.DrawObject(args);
		}
		private int GetScrollBarPartId(ScrollBarInfoArgs e, bool isInc) {
			if(e.ScrollBar.ScrollBarType == ScrollBarType.Horizontal) {
				if(isInc) 
					return XPConstants.SBP_LOWERTRACKHORZ;
				else return XPConstants.SBP_UPPERTRACKHORZ;
			} else {
				if(isInc) 
					return XPConstants.SBP_LOWERTRACKVERT;
				else return XPConstants.SBP_UPPERTRACKVERT;
			}
		}
	}
	public class LookAndFeelScrollBarPainter {
		public static ScrollBarPainterBase GetPainter(UserLookAndFeel lookAndFeel) {
			switch(lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Style3D:
					return new Style3DScrollBarPainter();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new UltraFlatScrollBarPainter();
				case ActiveLookAndFeelStyle.Office2003:
					if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) return new XPScrollBarPainter();
					return new FlatScrollBarPainter();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new XPScrollBarPainter();
				case ActiveLookAndFeelStyle.Skin:
					return new SkinScrollBarPainter(lookAndFeel.ActiveLookAndFeel);
			} 
			return new FlatScrollBarPainter();
		}
	}
	public class LookAndFeelScrollBarButtonPainter {
		public static ObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			switch(lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Style3D:
					return new Style3DScrollBarButtonPainter();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new UltraFlatScrollBarButtonPainter();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new XPScrollBarButtonPainter();
				case ActiveLookAndFeelStyle.Office2003:
					if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) return new XPScrollBarButtonPainter();
					return new FlatScrollBarButtonPainter();
				case ActiveLookAndFeelStyle.Skin:
					return new SkinScrollBarButtonPainter(lookAndFeel.ActiveLookAndFeel);
			} 
			return new FlatScrollBarButtonPainter();
		}
	}
	public class LookAndFeelScrollBarThumbPainter {
		public static ScrollBarThumbPainterBase GetPainter(UserLookAndFeel lookAndFeel) {
			switch(lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Style3D:
					return new Style3DScrollBarThumbPainter();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new UltraFlatScrollBarThumbPainter();
				case ActiveLookAndFeelStyle.Office2003:
					if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) return new XPScrollBarThumbPainter();
					return new FlatScrollBarThumbPainter();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new XPScrollBarThumbPainter();
			} 
			return new FlatScrollBarThumbPainter();
		}
	}
	public enum ScrollBarHitTest {IncButton, DecButton, Thumb, IncArea, DecArea, None}
	public class ScrollBarViewInfo {
		IScrollBar scrollBar;
		ScrollBarState state;
		bool isReady;
		bool isThumbFlashing;
		Rectangle incButtonBounds, decButtonBounds, thumbButtonBounds;
		float thumbTrackValue;
		int oldTrackingPos;
		public ScrollBarViewInfo(IScrollBar scrollBar) {
			this.scrollBar = scrollBar;
			this.state = ScrollBarState.Normal;
			this.isThumbFlashing = false;
			this.thumbTrackValue = 0;
			this.oldTrackingPos = 0;
			Reset();
		}
		public ScrollBarPainterBase Painter { get { return ScrollBar.Painter; } }
		public IScrollBar ScrollBar { get { return this.scrollBar; } }
		public bool IsReady { 
			get	  { return this.isReady;  } 
			set {
				if(value == IsReady) return;
				if(! value) 
					Reset();
				else Calculate(); 
			}
		}
		public Rectangle IncButtonBounds { get { Calculate(); return this.incButtonBounds; } }
		public Rectangle DecButtonBounds { get { Calculate(); return this.decButtonBounds; } }
		public Rectangle ThumbButtonBounds { get { Calculate(); return this.thumbButtonBounds; } }
		public bool Enabled { get { return ScrollBar.GetEnabled() && (VisibleMaximum - Minimum > 0); }}
			public ScrollBarState State {
			get { return this.state; }
			set { this.state = value; }
		}
		public void Calculate() {
			if(IsReady) return;
			CalculateCore();
			this.isReady = true;
		}
		public void Reset() {
			this.isReady = false;
			ResetCore();
		}
		public bool IsRightToLeft { get { return (WindowsFormsSettings.GetIsRightToLeft(ScrollBar.RightToLeft)) && (ScrollBarType == ScrollBarType.Horizontal); } } 
		public bool IsThumbFlashing { get { return this.isThumbFlashing; } set { this.isThumbFlashing = value; } }
		protected void CalculateCore() {
			this.incButtonBounds = CalcIncButtonBounds;
			this.decButtonBounds = CalcDecButtonBounds;
			this.thumbButtonBounds = CalcThumbButtonBounds;
		}
		protected void ResetCore() {
			this.incButtonBounds = Rectangle.Empty;
			this.decButtonBounds = Rectangle.Empty;
			this.thumbButtonBounds = Rectangle.Empty;
		}
		protected Rectangle CalcIncButtonBounds {
			get {
				if(! IsRightToLeft)
					return VisibleIncButtonBounds;
				else return this.incButtonBounds = VisibleDecButtonBounds;
			}
		}
		protected Rectangle CalcDecButtonBounds {
			get {
				if(! IsRightToLeft)
					return VisibleDecButtonBounds;
				else return VisibleIncButtonBounds;
			}
		}
		public static Rectangle CalcThumbBounds(bool horz, int minimum, int maximum, int largeChange, int value, int scrollAreaSize, int thumbMinWidth, int buttonWidth, int buttonCornerSize, bool isRightToLeft) {
			if(thumbMinWidth == 0) thumbMinWidth = System.Windows.Forms.SystemInformation.HorizontalScrollBarThumbWidth;
			int visibleMaximum = Math.Max(maximum - largeChange + 1, 1);
			if(maximum - minimum + 1 == 0) return Rectangle.Empty;
			int thumbWidth = scrollAreaSize * largeChange / (maximum - minimum + 1);
			if(largeChange > 1 &&
			   (thumbWidth > scrollAreaSize - scrollAreaSize / largeChange) &&
			   largeChange <= maximum)
				thumbWidth = scrollAreaSize - scrollAreaSize / largeChange;
			if(thumbWidth < thumbMinWidth)
				thumbWidth = thumbMinWidth;
			if(thumbWidth > scrollAreaSize) thumbWidth = scrollAreaSize;
			Int64 startPos;
			startPos = (scrollAreaSize - thumbWidth) * (Int64)(value - minimum);
			startPos /= (visibleMaximum - minimum);
			if(isRightToLeft) {
				startPos = (scrollAreaSize - thumbWidth - startPos) + buttonWidth - buttonCornerSize;
			}
			else {
			startPos = Math.Min(startPos, scrollAreaSize - thumbWidth) + buttonWidth - buttonCornerSize;
			}
			if(horz)
				return new Rectangle((int)startPos, 0, thumbWidth, 0);
			else return new Rectangle(0, (int)startPos, 0, thumbWidth);
		}
		protected virtual Rectangle CalcThumbButtonBounds {
			get {
				if(!IsThumbVisible) return Rectangle.Empty;
				Rectangle bounds = CalcThumbBounds(ScrollBarType == XtraEditors.ScrollBarType.Horizontal,
					Minimum, Maximum,
					LargeChange,
					ScrollBar.Value, ScrollAreaWidth,
					ScrollBar.DrawMode == ScrollBarDrawMode.TouchMode ? 30 : 0, ButtonWidth, ButtonCornerSize, IsRightToLeft);
				if(!bounds.IsEmpty) {
					if(ScrollBarType == XtraEditors.ScrollBarType.Horizontal)
						bounds.Height = ScrollBarHeight;
					else
						bounds.Width = ScrollBarHeight;
				}
				return bounds;
			}
		}
		public void BeforeThumbTracking(int pos) {
			this.oldTrackingPos = pos;
			this.thumbTrackValue = ScrollBar.Value;
		}
		public int ThumbTrackIntValue { get { return (int)Math.Round(this.thumbTrackValue); } }
		internal int PointToValue(Point p) {
			int pos = 0;
			if(ScrollBarType == ScrollBarType.Horizontal) 
				pos = p.X;
			else 
				pos = p.Y;
			return (int)(Minimum + (float)(pos - ButtonWidth) / ScrollAreaWidth *(Maximum - Minimum));
		}
		public int GetValueByPos(int pos) {
			float oldThumbTrackValue = this.thumbTrackValue;
			if(pos < ButtonWidth)
				this.thumbTrackValue = (!IsRightToLeft) ? Minimum : VisibleMaximum;
			else {
				if(pos > ScrollBarWidth - ButtonWidth)
					this.thumbTrackValue = (!IsRightToLeft) ? VisibleMaximum : Minimum;
				else {
					int thumbWidth;
					if(ScrollBarType == ScrollBarType.Horizontal)
						thumbWidth = ThumbButtonBounds.Width;
					else thumbWidth = ThumbButtonBounds.Height;
					float t = ScrollAreaWidth != thumbWidth ? (float)(ScrollAreaWidth - thumbWidth) : 1f;
					float dif = (float)(pos - oldTrackingPos) * (float)(VisibleMaximum - Minimum) / t;
					if(!IsRightToLeft) {
						thumbTrackValue += dif;
						if(dif > 0 && ThumbTrackIntValue < VisibleValue)
							thumbTrackValue = VisibleValue;
						if(dif < 0 && ThumbTrackIntValue > VisibleValue)
							thumbTrackValue = VisibleValue;
					}
					else thumbTrackValue -= dif;
				}
			}
			if(this.thumbTrackValue < Minimum)
				this.thumbTrackValue = Minimum;
			if(this.thumbTrackValue > VisibleMaximum)
				this.thumbTrackValue = VisibleMaximum;
			if(oldThumbTrackValue != this.thumbTrackValue)
				this.oldTrackingPos = pos;
			return ThumbTrackIntValue;
		}
		public ScrollBarState GetScrollBarState(Point pt, bool pressed) {
			ScrollBarHitTest hTest = GetHitTest(pt);
			if(pressed && hTest == ScrollBarHitTest.IncButton) 
				return ScrollBarState.IncButtonPressed;
			if(pressed && hTest == ScrollBarHitTest.DecButton) 
				return ScrollBarState.DecButtonPressed;
			if(pressed && hTest == ScrollBarHitTest.IncArea) 
				return ScrollBarState.IncAreaPressed;
			if(pressed && hTest == ScrollBarHitTest.DecArea) 
				return ScrollBarState.DecAreaPressed;
			if(pressed && hTest == ScrollBarHitTest.Thumb) 
				return ScrollBarState.ThumbPressed;
			return ScrollBarState.Normal;
		}
		public virtual ScrollBarHitTest GetHitTest(Point pt) {
			if(ScrollBar.DrawMode == ScrollBarDrawMode.Desktop && IncButtonBounds.IsEmpty)
				return ScrollBarHitTest.None;
			if(IncButtonBounds.Contains(pt))
				return ScrollBarHitTest.IncButton;
			if(DecButtonBounds.Contains(pt))
				return ScrollBarHitTest.DecButton;
			Rectangle thumbR = ThumbButtonBounds;
			if(thumbR.IsEmpty)
				return ScrollBarHitTest.None;
			if(thumbR.Contains(pt)) 
				return ScrollBarHitTest.Thumb;
			if(ScrollBarType == ScrollBarType.Horizontal) {
				if(! IsRightToLeft) {
					if(pt.X < thumbR.X)
						return ScrollBarHitTest.DecArea;
					else return ScrollBarHitTest.IncArea;
				} else {
					if(pt.X > thumbR.Right)
						return ScrollBarHitTest.DecArea;
					else return ScrollBarHitTest.IncArea;
				}
			} else {
				if(pt.Y < thumbR.Y) 
					return ScrollBarHitTest.DecArea;
				else return ScrollBarHitTest.IncArea;
			}
		}
		public void UpdateHotState(Point pt) {
			if(IsNormalOrHotState(State)) {
				ScrollBarState oldState = State;
				ScrollBarHitTest hitTest = GetHitTest(pt);
				this.state = ScrollBarState.Normal;
				if(ScrollBarHitTest.IncButton == hitTest)
					this.state = ScrollBarState.IncButtonHot;
				if(ScrollBarHitTest.DecButton == hitTest)
					this.state = ScrollBarState.DecButtonHot;
				if(ScrollBarHitTest.Thumb == hitTest)
					this.state = ScrollBarState.ThumbHot;
				if(oldState != State) {
					InvalidateByState(oldState);
					InvalidateByState(State);
				}
			}
		}
		public bool IsNormalOrHotState(ScrollBarState state) {
			return (ScrollBarState.Normal == state) || (ScrollBarState.DecButtonHot == state) 
				|| (ScrollBarState.IncButtonHot == state) || (ScrollBarState.ThumbHot == state);
		}
		public int LargeChange { get { return ScrollBar.LargeChange; } }
		public int Maximum { get { return ScrollBar.Maximum; } }
		public int Minimum { get { return ScrollBar.Minimum; } }
		public ScrollBarType ScrollBarType { get { return ScrollBar.ScrollBarType; } } 
		public int VisibleValue { get { return ScrollBar.VisibleValue; } }
		public int VisibleMaximum { get { return Math.Max(Maximum - LargeChange + 1, 1); } }
		public Rectangle IncAreaBounds {
			get {
				return GetAreaBounds(!IsRightToLeft);
			}
		}
		public Rectangle DecAreaBounds {
			get {
				return GetAreaBounds(IsRightToLeft);
			}
		}
		Rectangle GetAreaBounds(bool far) {
			Calculate();
			Rectangle thumbR = ThumbButtonBounds;
			if(thumbR.IsEmpty) return Rectangle.Empty;
			Rectangle res = thumbR;
			if(ScrollBarType == ScrollBarType.Horizontal) {
				if(!far) {
					res.X = DecButtonBounds.Right;
					res.Width = thumbR.X - res.X;
				} else {
					res.X = thumbR.Right;
					res.Width = IncButtonBounds.Left - res.X;
				}
			} else {
				if(!far) {
					res.Y = DecButtonBounds.Bottom;
					res.Height = thumbR.Y - res.Y;
				} else {
					res.Y = thumbR.Bottom;
					res.Height = IncButtonBounds.Y - res.Y;
				}
			}
			if(res.Width == 0 || res.Height == 0) return Rectangle.Empty;
			return res;
		}
		public virtual Rectangle VisibleIncButtonBounds {
			get { 
				if(ButtonWidth == 0)
					return Rectangle.Empty;
				if(ScrollBarType == ScrollBarType.Horizontal)
					return new Rectangle(ScrollBarWidth - ButtonWidth, 0, ButtonWidth, ScrollBarHeight); 
				else return new Rectangle(0, ScrollBarWidth - ButtonWidth, ScrollBarHeight, ButtonWidth); 
			}
		}
		public virtual Rectangle VisibleDecButtonBounds {
			get {
				if(ButtonWidth == 0)
					return Rectangle.Empty;
				if(ScrollBarType == ScrollBarType.Horizontal)
					return new Rectangle(0, 0, ButtonWidth, ScrollBarHeight);
				else return new Rectangle (0, 0, ScrollBarHeight, ButtonWidth);
			}
		}
		public bool IsThumbVisible { get { return Enabled && (Maximum != Minimum) && (ScrollAreaWidth >= Painter.ThumbMinWidth + 2); } }
		private int ScrollBarHeight {
			get {
				if(ScrollBarType == ScrollBarType.Horizontal)
					return ScrollBar.GetHeight();
				else return ScrollBar.GetWidth();
			}
		}
		public int ScrollBarWidth {
			get {
				if(ScrollBarType == ScrollBarType.Horizontal)
					return ScrollBar.GetWidth();
				else return ScrollBar.GetHeight();
			}
		}
		public virtual int ButtonWidth {
			get {
				if(ScrollBar.DrawMode != ScrollBarDrawMode.Desktop)
					return 0;
				int buttonWidth;
				if(ScrollBarType == ScrollBarType.Horizontal)
					buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
				else buttonWidth = SystemInformation.VerticalScrollBarArrowHeight;
				if(buttonWidth > ScrollBarWidth * 2)
					return 0;
				if(buttonWidth > ScrollBarWidth / 2)
					buttonWidth = ScrollBarWidth / 2;
				return buttonWidth;
			}
		}
		public int ScrollAreaWidth { 
			get { 
				if(ScrollBar.DrawMode == ScrollBarDrawMode.Desktop && ButtonWidth == 0) return 0;
				return (ScrollBarWidth - ButtonWidth * 2) + ButtonCornerSize * 2; 
			}	
		}
		public int ButtonCornerSize { 
			get {
				if(ScrollBar.DrawMode == ScrollBarDrawMode.Desktop && ButtonWidth == 0) return 0;
				return Painter.GetButtonCornerSize(ScrollBarType); 
			} 
		}
		private void InvalidateByState(ScrollBarState state) {
			Rectangle bounds = Rectangle.Empty;
			if((ScrollBarState.DecButtonHot == state) || (ScrollBarState.DecButtonPressed == state))
				bounds = DecButtonBounds;
			if((ScrollBarState.IncAreaPressed == state) || (ScrollBarState.IncButtonHot == state) 
				|| (ScrollBarState.IncButtonPressed == state))
				bounds = IncButtonBounds;
			if((ScrollBarState.DecButtonHot == state) || (ScrollBarState.DecButtonPressed == state))
				bounds = DecButtonBounds;
			if((ScrollBarState.ThumbHot == state) || (ScrollBarState.ThumbPressed == state))
				bounds = ThumbButtonBounds;
			if(! bounds.IsEmpty)
				ScrollBar.Invalidate(bounds);
		}
	}
}
namespace DevExpress.Utils.Win {
	public interface IFormSubstitute {
		bool FormIsCreated { get; }
		bool FormIsHandleCreated { get; }
	}
	public static class FormExtensions {
		public static bool FormIsCreated(Control control) {
			return control is IFormSubstitute ?
				((IFormSubstitute)control).FormIsCreated :
				control.FindForm() != null;
		}
		public static bool FormIsHandleCreated(Control control) {
			if(control is IFormSubstitute)
				return ((IFormSubstitute)control).FormIsHandleCreated;
			Form form = control.FindForm();
			return form != null &&  form.IsHandleCreated;
		}
	}
}
namespace DevExpress.XtraEditors {
	using DevExpress.Utils.Win;
	public enum ScrollBarType {Horizontal, Vertical}
	public enum ScrollNotifyAction { MouseMove, Hide, Resize };
	public enum DragScrollThumbBeyondControlMode { Default, RestoreThumbPosition, KeepThumbAtTerminalPosition };
	public interface IScrollBar {
		bool GetEnabled();
		RightToLeft RightToLeft { get; }
		int LargeChange { get; }
		int Maximum { get; }
		int Minimum { get; }
		ScrollBarType ScrollBarType { get; }
		int GetWidth();
		int GetHeight();
		bool IsOverlapScrollBar { get; }
		bool ContainsMouse();
		ScrollBarPainterBase Painter { get; }
		int VisibleValue { get; }
		int Value { get; }
		void Invalidate(Rectangle rect);
		UserLookAndFeel LookAndFeel { get; }
		ScrollBarViewInfo ViewInfo { get; }
		void ChangeValueBasedByState(ScrollBarState state);
		ScrollBarDrawMode DrawMode { get; }
		Control Parent { get; }
		void ProcessMouseDown(MouseEventArgs e);
		void ProcessMouseMove(MouseEventArgs e);
		void ProcessMouseUp(MouseEventArgs e);
		void ProcessMouseEnter();
		void ProcessMouseLeave();
		void ProcessPaint(PaintEventArgs e);
	}
	public enum ScrollUIMode { Default, Desktop, Touch };
	public enum ScrollBarDrawMode { Desktop, Combined, TouchMode }
	public abstract class ScrollBarBase : Control, ISupportLookAndFeel, IScrollBar {
		protected internal const int DefaultLargeChangeCore = 10;
		protected internal const int DefaultMaximumCore = 100;
		protected internal const int DefaultMinimumCore = 0;
		protected internal const int DefaultSmallChangeCore = 1;
		const int DefaultFirstScrollInterval = 200;
		const int DefaultSecondScrollInterval = 50;
		ScrollBarPainterBase painter;
		ScrollBarViewInfo viewInfo;
		AppearanceObject style, backStyle;
		UserLookAndFeel lookAndFeel;
		int updateCount;
		int beforeThumbDraggingValue;
		Timer scrollTimer;
		Timer keyBoardFocusTimer;
		bool scrollBarAutoSize;
		public ScrollBarBase() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			base.TabStop = false;
			this.painter = null;
			this.viewInfo = CreateScrollBarViewInfo(); 
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			SetDefaultScrollBarHeight();
			this.updateCount = 0;
			this.scrollTimer = new Timer();
			this.scrollTimer.Enabled = false;
			this.scrollTimer.Tick += new EventHandler(ScrollTimerEventProcessor);
			this.keyBoardFocusTimer = new Timer();
			this.keyBoardFocusTimer.Enabled = false;
			this.keyBoardFocusTimer.Tick += new EventHandler(KeyBoardTimerEventProcessor);
			this.keyBoardFocusTimer.Interval = 500;
			this.scrollBarAutoSize = false;
			this.style = new AppearanceObject("ScrollBar");
			this.backStyle = new AppearanceObject("ScrollBar Back");
			this.backStyle.BackColor = SystemColors.WindowText;
		}
		const ScrollUIMode defaultUIMode = ScrollUIMode.Desktop;
		static ScrollUIMode uiMode = ScrollUIMode.Default;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ScrollUIMode GetUIMode(ScrollUIMode controlMode) {
			if(controlMode == ScrollUIMode.Default) controlMode = UIMode;
			if(controlMode == ScrollUIMode.Default) controlMode = defaultUIMode;
			return controlMode;
		}
		public int GetDefaultVerticalScrollBarWidth() { return GetVerticalScrollBarWidth(GetUIMode()); }
		public int GetDefaultHorizontalScrollBarHeight() { return GetHorizontalScrollBarHeight(GetUIMode()); }
		public static int GetVerticalScrollBarWidth() { return GetVerticalScrollBarWidth(ScrollUIMode.Default); }
		public static int GetVerticalScrollBarWidth(ScrollUIMode controlMode) {
			switch(GetUIMode(controlMode)) {
				case ScrollUIMode.Touch:
					return (int)DpiProvider.Default.DpiScaleFactor * 16;
			}
			return SystemInformation.VerticalScrollBarWidth;
		}
		public static int GetHorizontalScrollBarHeight() { return GetHorizontalScrollBarHeight(ScrollUIMode.Default); }
		public static int GetHorizontalScrollBarHeight(ScrollUIMode controlMode) {
			switch(GetUIMode(controlMode)) {
				case ScrollUIMode.Touch:
					return (int)DpiProvider.Default.DpiScaleFactor * 16;
			}
			return SystemInformation.HorizontalScrollBarHeight;
		}
		[Browsable(false)]
		public static ScrollUIMode UIMode { get { return uiMode; } set { uiMode = value; } }
		public static ScrollBarBase ApplyUIMode(ScrollBarBase scroll) { return ApplyUIMode(scroll, ScrollUIMode.Default); }
		public static ScrollBarBase ApplyUIMode(ScrollBarBase scroll, ScrollUIMode controlMode) {
			ScrollTouchBase ts = scroll as ScrollTouchBase;
			if(ts == null) return scroll;
			switch(GetUIMode(controlMode)) {
				case ScrollUIMode.Touch :
					ts.TouchMode = true;
					break;
				case ScrollUIMode.Desktop:
					ts.TouchMode = false;
					break;
			}
			return scroll;
		}
		public static ScrollUIMode GetUIMode(ScrollBarBase scroll) {
			ScrollTouchBase ts = scroll as ScrollTouchBase;
			if(ts == null) return ScrollUIMode.Desktop;
			if(ts.TouchMode) return ScrollUIMode.Touch;
			return ScrollUIMode.Desktop;
		}
		protected virtual ScrollUIMode GetUIMode() { return GetUIMode(this); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ScrollBarDrawMode DrawMode { get { return  ScrollBarDrawMode.Desktop; } }
		Point lastMouseActionPoint = Point.Empty;
		DateTime resizeActionTime = DateTime.MinValue;
		protected internal DateTime ResizeActionTime { get { return resizeActionTime; } set { resizeActionTime = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsOverlapScrollBar { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ActualVisible {
			get { return Visible; }
			protected set { Visible = value; }
		}
		public virtual void SetVisibility(bool visible) { Visible = visible; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnAction(ScrollNotifyAction action) {
			if(action == ScrollNotifyAction.MouseMove) {
				if(this.lastMouseActionPoint == Control.MousePosition) return;
				this.lastMouseActionPoint = Control.MousePosition;
			}
			if(action == ScrollNotifyAction.Resize) {
				this.resizeActionTime = DateTime.Now;
				DelayedOnActionCore(ScrollNotifyAction.Hide);
			}
			if(action == ScrollNotifyAction.Hide) {
				DelayedOnActionCore(action);
				return;
			}
			OnActionCore(action);
		}
		void DelayedOnActionCore(ScrollNotifyAction action) {
			if(IsHandleCreated) {
				BeginInvoke(new MethodInvoker(() => { OnActionCore(action); }));
			}
			else {
				OnActionCore(action);
			}
		}
		protected virtual void OnActionCore(ScrollNotifyAction action) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DevExpress.Accessibility.BaseAccessible GetAccessible() { return DXAccessible; }
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected virtual DevExpress.Accessibility.BaseAccessible DXAccessible { 
			get {
				if(dxAccessible == null) dxAccessible = new DevExpress.Accessibility.ScrollBarAccessible(this);
				return dxAccessible;
			}
		}
		protected virtual ScrollBarViewInfo CreateScrollBarViewInfo() {
			return new ScrollBarViewInfo(this);
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			return DXAccessible.Accessible;
		}
		protected override void Dispose(bool disposing){
			if(disposing) {
				DelayedTimerKillIfExists();
				if(this.scrollTimer != null) {
					this.scrollTimer.Enabled = false;
					this.scrollTimer.Dispose();
					this.scrollTimer = null;
				}
				if(this.keyBoardFocusTimer != null) {
					this.keyBoardFocusTimer.Enabled = false;
					this.keyBoardFocusTimer.Dispose();
					this.keyBoardFocusTimer = null;
				}
				if(this.lookAndFeel != null) {
					this.lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
					this.lookAndFeel.Dispose();
					this.lookAndFeel = null;
				}
				if(painter != null) {
					painter.DestroyBrushes();
					painter = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual bool IsCreated { get { return IsHandleCreated; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("Don't use the AutoSize property, use the ScrollBarAutoSize property instead")]
		public new bool AutoSize { 
			get { return this.ScrollBarAutoSize; }
			set { ScrollBarAutoSize = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseScrollBarAutoSize"),
#endif
 DefaultValue(false)]
		public bool ScrollBarAutoSize {
			get { return this.scrollBarAutoSize; }
			set {
				if(value == ScrollBarAutoSize) return;
				this.scrollBarAutoSize = value;
				SetDefaultScrollBarHeight();
				OnScrollBarScrollBarAutoSizeChanged(EventArgs.Empty);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseLargeChange"),
#endif
 DefaultValue(DefaultLargeChangeCore)]
		public int LargeChange {
			get { return LargeChangeCore; }
			set {
				if(!IsCreated) {
					LargeChangeCore = value;
					return;
				}
				if((value <= 0) || (value > Maximum - Minimum + 1) || (value == LargeChangeCore)) return;
				LargeChangeCore = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseMaximum"),
#endif
 DefaultValue(DefaultMaximumCore)]
		public int Maximum {
			get { return MaximumCore; }
			set {
				if(!IsCreated) {
					MaximumCore = value;
					if(MinimumCore > MaximumCore) MinimumCore = value;
					return;
				}
				if(value == Maximum) return;
				if(value < Minimum) Minimum = value;
				MaximumCore = value;
				if(Maximum < Value)
					Value = Maximum;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseMinimum"),
#endif
 DefaultValue(DefaultMinimumCore)]
		public int Minimum {
			get {
				return MinimumCore;
			}
			set {
				if(!IsCreated) {
					MinimumCore = value;
					if(MinimumCore > MaximumCore) MaximumCore = value;
					return;
				}
				if(value == Minimum) return;
				if(value > Maximum) Maximum = value;
				MinimumCore = value;
				if(Minimum > Value)
					Value = value;
				LayoutChanged();
			}
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[Browsable(false)]
		public abstract ScrollBarType ScrollBarType {get;}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseSmallChange"),
#endif
 DefaultValue(DefaultSmallChangeCore)]
		public int SmallChange {
			get { return SmallChangeCore; }
			set {
				if(!IsCreated) {
					SmallChangeCore = value;
					return;
				}
				if((value <= 0) && (value >= Maximum - Minimum)) return;
				SmallChangeCore = value;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseValue"),
#endif
 DefaultValue(DefaultMinimumCore)]
		public int Value {
			get { return ValueCore; }
			set {
				if(!IsCreated) {
					ValueCore = value;
					return;
				}
				if(value > Maximum) value = Maximum;
				if(value < Minimum) value = Minimum;
				if((value < Minimum) || (value > Maximum) || (value == Value)) return;
				int oldValue = Value;
				ValueCore = value;
				LayoutChanged();
				if(oldValue != Value)
					OnValueChanged(EventArgs.Empty);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font { get { return base.Font; } set { base.Font = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImeMode ImeMode { get { return base.ImeMode; } set { base.ImeMode = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ScrollBarBaseTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		public void BeginUpdate() { updateCount ++; }
		public void CancelUpdate() {
			updateCount --;
			if(updateCount < 0)
				updateCount = 0;
		}
		public void EndUpdate() {
			CancelUpdate();
			LayoutChanged(true);
		}
		private static readonly object scroll = new object();
		private static readonly object valueChanged = new object();
		private static readonly object scrollBarScrollBarAutoSizeChanged = new object();
#if !SL
	[DevExpressUtilsLocalizedDescription("ScrollBarBaseScroll")]
#endif
		public event ScrollEventHandler Scroll {
			add { Events.AddHandler(scroll, value); }
			remove { Events.RemoveHandler(scroll, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ScrollBarBaseValueChanged")]
#endif
		public event EventHandler ValueChanged {
			add { Events.AddHandler(valueChanged, value); }
			remove { Events.RemoveHandler(valueChanged, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ScrollBarBaseScrollBarAutoSizeChanged")]
#endif
		public event EventHandler ScrollBarAutoSizeChanged {
			add { Events.AddHandler(scrollBarScrollBarAutoSizeChanged, value); }
			remove { Events.RemoveHandler(scrollBarScrollBarAutoSizeChanged, value); }
		}
		protected override void OnGotFocus(EventArgs e) {
			KeyBoardTimerEnabled = true;
			base.OnGotFocus(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			KeyBoardTimerEnabled = false;
			base.OnLostFocus(e);
		}
		protected override void OnHandleCreated(EventArgs e) {
			myHandle = this.Handle;
			base.OnHandleCreated(e);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			myHandle = IntPtr.Zero;
			base.OnHandleDestroyed(e);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			LayoutChanged();
		}
		protected internal ScrollBarPainterBase Painter { 
			get { 
				if(painter == null)
					painter = CreateScrollBarPainter();
				return painter; 
			} 
		}
		bool IScrollBar.ContainsMouse() { return ContainsMouse; }
		internal bool containsMouse = false;
		protected bool ContainsMouse {
			get { return containsMouse; }
			set {
				if(ContainsMouse == value) return;
				containsMouse = value;
				Invalidate();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			ScrollBarInfoArgs args = new ScrollBarInfoArgs(cache, ClientRectangle, this.style, this.backStyle, this);
			args.State = (Enabled ? ObjectState.Normal : ObjectState.Disabled);
			if(containsMouse && Enabled) args.State |= ObjectState.Selected;
			Painter.UpdateThumbBounds(args);
			Painter.DrawObject(args);
		}
		protected override bool IsInputKey(Keys keyData) {
			return (keyData == Keys.Up) || (keyData == Keys.Left) || (keyData == Keys.Down)
				|| (keyData == Keys.Right) || (keyData == Keys.PageUp) || (keyData == Keys.PageDown)
				|| (keyData == Keys.Home) || (keyData == Keys.End);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.Handled) return;
			if((e.KeyData == Keys.Up) || (e.KeyData == Keys.Left))
				((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.DecButtonPressed);
			if((e.KeyData == Keys.Down) || (e.KeyData == Keys.Right))
				((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.IncButtonPressed);
			if(e.KeyData == Keys.PageUp)
				((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.DecAreaPressed);
			if(e.KeyData == Keys.PageDown)
				((IScrollBar)this).ChangeValueBasedByState(ScrollBarState.IncAreaPressed);
			if(e.KeyData == Keys.Home)
				Value = this.Minimum;
			if(e.KeyData == Keys.End)
				Value = ViewInfo.VisibleMaximum;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left) return;
			if(TabStop)
				Focus();
			State = ViewInfo.GetScrollBarState(new Point(e.X, e.Y), true);
			if(State == ScrollBarState.ThumbPressed) {
				this.beforeThumbDraggingValue = Value;
				ViewInfo.BeforeThumbTracking((ScrollBarType.Horizontal == ScrollBarType) ? e.X : e.Y);
			}
			if(Control.ModifierKeys == Keys.Shift)
				ChangeValueBasedByStateOnShiftPressed(State, new Point(e.X, e.Y));
			else
				((IScrollBar)this).ChangeValueBasedByState(State);
			this.scrollTimer.Enabled = IsScrollingState(State);
			if(this.scrollTimer.Enabled)
				this.scrollTimer.Interval = DefaultFirstScrollInterval;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(State == ScrollBarState.ThumbPressed) {
				int pos = (ScrollBarType.Horizontal == ScrollBarType) ? e.X: e.Y;
				if(((pos + WindowsFormsSettings.DragScrollThumbBeyondControlThreshold < -3 * ViewInfo.ButtonWidth) ||
					(pos - ViewInfo.ScrollBarWidth - WindowsFormsSettings.DragScrollThumbBeyondControlThreshold > 2 * ViewInfo.ButtonWidth)) 
					&& AllowRestoreThumbPosition) {
					SetScrollBarValue(ScrollEventType.ThumbTrack, this.beforeThumbDraggingValue);
				} else {
					int iPos = ViewInfo.GetValueByPos(pos);
					SetScrollBarValue(ScrollEventType.ThumbTrack, iPos);
					LayoutChanged();
				}
			}
			ViewInfo.UpdateHotState(new Point(e.X, e.Y));
		}
		bool AllowRestoreThumbPosition {
			get {
				if(WindowsFormsSettings.DragScrollThumbBeyondControlMode == DragScrollThumbBeyondControlMode.RestoreThumbPosition) return true;
				if(WindowsFormsSettings.DragScrollThumbBeyondControlMode == DragScrollThumbBeyondControlMode.KeepThumbAtTerminalPosition) return false;
				return DrawMode != ScrollBarDrawMode.TouchMode;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			this.scrollTimer.Enabled = false;
			SetNormalState();
			ViewInfo.UpdateHotState(new Point(e.X, e.Y));
			base.OnMouseUp(e);
		}
		protected virtual void SetNormalState() {
			if(State == ScrollBarState.Normal) return;
				if(IsScrollingState(State) || (State == ScrollBarState.ThumbPressed))  { 
					if(State == ScrollBarState.ThumbPressed)
						SetScrollBarValue(ScrollEventType.ThumbPosition, Value);
					State = ScrollBarState.Normal; 
					SetScrollBarValue(ScrollEventType.EndScroll, Value);
				}
				State = ScrollBarState.Normal;
				LayoutChanged();
			}
		const int WM_CAPTURECHANGED = 0x215;
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle) 
					OnGotCapture();
				else
					OnLostCapture();
			}
			base.WndProc(ref m);
			DoDelayedUpdateMessageProcessing();
			DevExpress.Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void OnLostCapture() {
			SetNormalState();
		}
		protected virtual void OnGotCapture() { }
		protected override void OnMouseLeave(EventArgs e) {
			ViewInfo.UpdateHotState(new Point(-10000, -10000));
			base.OnMouseLeave(e);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(ScrollBarAutoSize) 
				SetDefaultScrollBarHeight();
			LayoutChanged(true);
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			LayoutChanged();
		}
		internal protected ScrollBarViewInfo ViewInfo { get { return this.viewInfo; } }
		void IScrollBar.Invalidate(Rectangle bounds) {
			InvalidateCore(bounds);
		}
		protected virtual void InvalidateCore(Rectangle bounds) {
			Invalidate(bounds);
		}
		void IScrollBar.ProcessMouseLeave() {
			ContainsMouse = false;
		}
		void IScrollBar.ProcessMouseEnter() {
			ContainsMouse = IsTouchHitTestArea();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessMouseDown(MouseEventArgs e) {
			OnMouseDown(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessMouseUp(MouseEventArgs e) {
			OnMouseUp(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessMouseMove(MouseEventArgs e) {
			ContainsMouse = IsTouchHitTestArea();
			OnMouseMove(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessPaint(PaintEventArgs e) {
			Point p = PointToClientEx(Control.MousePosition);
			ViewInfo.UpdateHotState(new Point(p.X, p.Y));
			OnPaint(e);
		}
		Point PointToClientEx(Point screenPoint) {
			if(Parent != null && Parent.IsHandleCreated) {
				Point p = Parent.PointToClient(screenPoint);
				p.Offset(-Bounds.X, -Bounds.Y);
				return p;
			}
			return screenPoint;
		}
		bool IsTouchHitTestArea() {
			Point p = PointToClientEx(Control.MousePosition);
			return ClientRectangle.Contains(p);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessPaintBackground(PaintEventArgs e) {
			OnPaintBackground(e);
		}
		protected abstract int LargeChangeCore { get; set; }
		protected abstract int MaximumCore { get; set; }
		protected abstract int MinimumCore { get; set; }
		protected abstract int SmallChangeCore { get; set; }
		protected abstract int ValueCore { get; set; }
		protected virtual ScrollBarPainterBase CreateScrollBarPainter() {
			return LookAndFeelScrollBarPainter.GetPainter(LookAndFeel);
		}
		protected virtual void OnScrollBarScrollBarAutoSizeChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[scrollBarScrollBarAutoSizeChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnScroll(ScrollEventArgs e) {
			if(lockEvents) return;
			ScrollEventHandler handler = (ScrollEventHandler)this.Events[scroll];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnValueChanged(EventArgs e) {
			if(lockEvents) return;
			EventHandler handler = (EventHandler)this.Events[valueChanged];
			if(handler != null) handler(this, e);
			AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
		}
		protected ScrollBarState State {
			get { return ViewInfo.State; }
			set {
				if(value == State) return;
				ViewInfo.State = value;
				KeyBoardTimerEnabled = ViewInfo.IsNormalOrHotState(value) && Focused;
			}
		}
		protected internal int VisibleValue {
			get {
				if(ViewInfo.IsRightToLeft)
					return (ViewInfo.VisibleMaximum - Minimum) - Value;
				else return Value;
			}
			set {
				if(ViewInfo.IsRightToLeft)
					Value = (ViewInfo.VisibleMaximum - Minimum) - value;
				else Value = value;
			}
		}
		private bool IsScrollingState(ScrollBarState state) {
			return (state == ScrollBarState.IncButtonPressed) || 
				(state == ScrollBarState.DecButtonPressed) ||
				(state == ScrollBarState.IncAreaPressed) || 
				(state == ScrollBarState.DecAreaPressed);
		}
		private void SetScrollBarValue(ScrollEventType scrollType, int value) {
			value = GetScrollBarValue(value);
			ScrollEventArgs e = new ScrollEventArgs(scrollType, Value, value);
			OnScroll(e);
			Value = e.NewValue;
		}
		private int GetScrollBarValue(int value) {
			if(value < Minimum) return Minimum;
			if(value > ViewInfo.VisibleMaximum) return ViewInfo.VisibleMaximum;
			return value;
		}
		void IScrollBar.ChangeValueBasedByState(ScrollBarState state) {
			int newValue = Value;
			ScrollEventType scrollType = ScrollEventType.EndScroll;
			switch(state) {
				case ScrollBarState.IncButtonPressed:
					newValue += SmallChange; 
					scrollType = ScrollEventType.SmallIncrement;
					break;
				case ScrollBarState.DecButtonPressed:
					newValue -= SmallChange; 
					scrollType = ScrollEventType.SmallDecrement;
					break;
				case ScrollBarState.IncAreaPressed:
					newValue += LargeChange; 
					scrollType = ScrollEventType.LargeIncrement;
					break;
				case ScrollBarState.DecAreaPressed:
					newValue -= LargeChange; 
					scrollType = ScrollEventType.LargeDecrement;
					break;
			}
			if(scrollType != ScrollEventType.EndScroll)
				SetScrollBarValue(scrollType, newValue);
		}
		protected virtual void ChangeValueBasedByStateOnShiftPressed(ScrollBarState state, Point point) {
			if(state == ScrollBarState.DecAreaPressed || state == ScrollBarState.IncAreaPressed) {
				int newValue = ViewInfo.PointToValue(point);
				SetScrollBarValue(ScrollEventType.ThumbPosition, newValue);
			}
		}
		private void ScrollTimerEventProcessor(Object item, EventArgs e) {
			this.scrollTimer.Interval = DefaultSecondScrollInterval;
			ScrollBarState state = GetScrollBarState();
			if(state == State)
				((IScrollBar)this).ChangeValueBasedByState(state);
		}
		protected virtual ScrollBarState GetScrollBarState(){
			return ViewInfo.GetScrollBarState(PointToClient(MousePosition), true);
		}
		private bool KeyBoardTimerEnabled {
			get { return this.keyBoardFocusTimer.Enabled; }
			set {
				if (keyBoardFocusTimer == null) return;
				keyBoardFocusTimer.Enabled = value;
				if(! value) {
					bool oldIsThumbFlashing = ViewInfo.IsThumbFlashing;
					ViewInfo.IsThumbFlashing = false;
					if(oldIsThumbFlashing)
						Invalidate(ViewInfo.ThumbButtonBounds);
				}
			}
		}
		private void KeyBoardTimerEventProcessor(Object item, EventArgs e) {
			if(ViewInfo.IsThumbVisible) {
				ViewInfo.IsThumbFlashing = ! ViewInfo.IsThumbFlashing;
				Invalidate(ViewInfo.ThumbButtonBounds);
			}
		}
		protected void LayoutChanged() {
			LayoutChanged(false);
		}
		private void LayoutChanged(bool allowDelayed) {
			if(updateCount == 0) {
				ViewInfo.Reset();
				Invalidate();
				if(IsCreated) {
					if(allowDelayed)
						DelayedUpdate();
					else
						Update();
				}
			}
		}
		private void SetDefaultScrollBarHeight() {
			if(ScrollBarType == ScrollBarType.Horizontal)
				Height = GetDefaultHorizontalScrollBarHeight();
			else Width = GetDefaultVerticalScrollBarWidth();
		}
		private void OnLookAndFeelStyleChanged(object sender, System.EventArgs e) {
			RecreatePainter();
			ViewInfo.Reset();
			Invalidate();
		}
		protected void RecreatePainter() {
			if(painter != null) painter.DestroyBrushes();
			painter = CreateScrollBarPainter();
		}
		ScrollBarPainterBase IScrollBar.Painter { get { return this.Painter; } }
		int IScrollBar.VisibleValue { get { return this.VisibleValue; } }
		ScrollBarViewInfo IScrollBar.ViewInfo { get { return this.ViewInfo; } }
		#region IScrollBar Members
		public bool GetEnabled() {
			return Enabled;
		}
		public int GetWidth() {
			return Width;
		}
		public int GetHeight() {
			return Height;
		}
		#endregion
		bool lockEvents = false;
		internal void Assign(ScrollArgs scrollArgs) {
			BeginUpdate();
			try {
				this.lockEvents = !scrollArgs.FireEventsOnAssign;
				Enabled = scrollArgs.Enabled;
				MinimumCore = scrollArgs.Minimum;
				MaximumCore = scrollArgs.Maximum;
				LargeChangeCore = scrollArgs.LargeChange;
				SmallChangeCore = scrollArgs.SmallChange;
				Value = Math.Min(Maximum, scrollArgs.Value);
			}
			finally {
				this.lockEvents = false;
				EndUpdate();
			}
		}
		System.Threading.Timer delayedTimer;
		bool delayedUpdateFlag;
		IntPtr myHandle;
		const int DelayedInterval = 100;
		void DelayedUpdate() {
			delayedUpdateFlag = false;
			if(this.delayedTimer != null) {
				this.delayedTimer.Change(DelayedInterval, -1);
			} else {
				this.delayedTimer = new System.Threading.Timer(delayedTimerHandler, null, DelayedInterval, -1);
			}
		}
		[System.Security.SecuritySafeCritical]
		void delayedTimerHandler(object state) {
			try {
				IntPtr _myHandle = myHandle;
				if(_myHandle == IntPtr.Zero)
					return;
				if(!InvokeRequired)
					return;
				delayedUpdateFlag = true;
				DevExpress.Utils.Drawing.Helpers.NativeMethods.PostMessage(_myHandle, 0, IntPtr.Zero, IntPtr.Zero);
			} catch { }
		}
		static void OnDelayedUpdateStatic(object thisObjectWeakReference) {
			WeakReference weakRef = thisObjectWeakReference as WeakReference;
			if (weakRef == null)
				return;
			if (!weakRef.IsAlive)
				return;
			ScrollBarBase thisObject = weakRef.Target as ScrollBarBase;
			if (thisObject != null)
				thisObject.OnDelayedUpdate();
		}
		void OnDelayedUpdate() {
			if(delayedTimer == null)
				return;
			DelayedTimerKillIfExists();
			if(IsHandleCreated)
				Update();
			delayedUpdateFlag = false;
		}
		void DelayedTimerKillIfExists() {
			if(this.delayedTimer != null) {
				this.delayedTimer.Dispose();
				this.delayedTimer = null;
			}
		}
		void DoDelayedUpdateMessageProcessing() {
			if(delayedUpdateFlag) {
				delayedUpdateFlag = false;
				MethodInvoker call = delegate() { OnDelayedUpdateStatic(new WeakReference(this)); };
				BeginInvoke(call);
			}
		}
	}
	public abstract class ScrollTouchBase : ScrollBarBase {
		#region TouchBase
		FloatingScrollbar floatingScrollbar = null;
		bool touchMode;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TouchMode {
			get {
				return touchMode;
			}
			set {
				if(touchMode == value) return;
				touchMode = value;
				OnTouchModeChanged();
			}
		}
		protected override void InvalidateCore(Rectangle bounds) {
			base.InvalidateCore(bounds);
			if(FloatingScrollbar != null && FloatingScrollbar.Visible) {
				FloatingScrollbar.Invalidate();
			}
		}
		protected virtual bool IsAllowFloatingScrollbar {
			get { return TouchMode; }
		}
		protected override bool IsCreated { 
			get {
				if(base.IsCreated) return true;
				if(FloatingScrollbar != null && FloatingScrollbar.IsCreated) return true;
				return false;
			}
		}
		public override ScrollBarDrawMode DrawMode { get { return TouchMode ? ScrollBarDrawMode.TouchMode : ScrollBarDrawMode.Desktop; } }
		protected virtual void OnTouchModeChanged() {
			if(TouchMode) {
				Visible = false;
				LayoutChanged();
				if(ActualVisible) OnScrollChanged();
			}
			else {
				FloatingScrollbar.Hide();
				if(ActualVisible) {
					Visible = true;
					LayoutChanged();
					OnScrollChanged();
				}
			}
		}
		protected virtual void DestroyFloatingScrollbar() {
			if(floatingScrollbar != null) floatingScrollbar.CleanUp();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) DestroyFloatingScrollbar();
		}
		public override bool IsOverlapScrollBar { get { return TouchMode; } }
		protected FloatingScrollbar FloatingScrollbar { get { return floatingScrollbar; } }
		bool actualVisible = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActualVisible {
			get {
				if(IsAllowFloatingScrollbar) {
					if(Parent != null && Parent.Visible) return actualVisible;
					return false;
				}
				return Visible;
			}
			protected set { actualVisible = value; }
		}
		public override void SetVisibility(bool visible) {
			bool prevVisible = ActualVisible;
			ActualVisible = visible;
			Visible = visible && !IsAllowFloatingScrollbar;
			if(prevVisible != ActualVisible) OnActualVisibleChanged();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
			UpdateSize();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			UpdateSize();
		}
		protected virtual void UpdateSize() {
			if(Parent == null) return;
			if(!IsAllowFloatingScrollbar) return;
			if(!FormExtensions.FormIsHandleCreated(Parent) || !Parent.IsHandleCreated) return;
			if(ActualVisible) EnsureCreated();
			Rectangle bounds = new Rectangle(Parent.PointToScreen(Bounds.Location), Size);
			if(FloatingScrollbar != null) FloatingScrollbar.Bounds = bounds;
		}
		protected virtual void OnActualVisibleChanged() {
			if(ActualVisible && !IsDesignMode) {
				if(IsAllowFloatingScrollbar) {
					EnsureCreated();
					UpdateSize();
					if(!FloatingScrollbar.AllowAutoHide) FloatingScrollbar.Show();
					FloatingScrollbar.OnScrollChanged();
				}
			}
			else {
				HideFloatingScrollbar();
			}
		}
		protected override void OnValueChanged(EventArgs e) {
			base.OnValueChanged(e);
			OnScrollChanged();
		}
		protected override void OnScroll(ScrollEventArgs e) {
			base.OnScroll(e);
			OnScrollChanged();
		}
		protected override void OnActionCore(ScrollNotifyAction action) {
			if(action == ScrollNotifyAction.Hide) {
				HideFloatingScrollbar();
				return;
			}
			if(action == ScrollNotifyAction.MouseMove) {
				if(WindowsFormsSettings.ShowTouchScrollBarOnMouseMove) OnScrollChanged();
			}
		}
		protected void HideFloatingScrollbar() {
			if(FloatingScrollbar == null) return;
			FloatingScrollbar.Hide();
		}
		bool IsDesignMode { get { return DesignMode || (Parent != null && Parent.Site != null && Parent.Site.DesignMode); } }
		void OnScrollChanged() {
			if(DateTime.Now.Subtract(ResizeActionTime).TotalMilliseconds < 100) return;
			if(!IsAllowFloatingScrollbar) return;
			EnsureCreated();
			if(ActualVisible && !IsDesignMode) {
				UpdateSize();
				FloatingScrollbar.OnScrollChanged();
				FloatingScrollbar.Show();
			}
		}
		void EnsureCreated() {
			if(!IsAllowFloatingScrollbar) return;
			if(floatingScrollbar == null) floatingScrollbar = new FloatingScrollbar(this);
			if(Parent != null && Parent.IsHandleCreated && FormExtensions.FormIsCreated(Parent)) FloatingScrollbar.Create(Parent);
		}
		#endregion TouchBase
		internal void OnParentResized() {
			ResizeActionTime = DateTime.Now;
		}
	}
	public abstract class HScrollBarBase : ScrollTouchBase {
#if !SL
	[DevExpressUtilsLocalizedDescription("HScrollBarBaseScrollBarType")]
#endif
		public override ScrollBarType ScrollBarType { get { return ScrollBarType.Horizontal; } }
	}
	public abstract class VScrollBarBase : ScrollTouchBase {
#if !SL
	[DevExpressUtilsLocalizedDescription("VScrollBarBaseScrollBarType")]
#endif
		public override ScrollBarType ScrollBarType { get { return ScrollBarType.Vertical; } }
	}
	[Description("Allows you to implement horizontal scrolling in containers that do not provide their own scroll bars."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	  ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "HScrollBar")
	]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	public class HScrollBar : HScrollBarBase {
		int largeChangeCore, maximumCore, minimumCore, smallChangeCore, valueCore;
		public HScrollBar() {
			largeChangeCore = DefaultLargeChangeCore;
			maximumCore = DefaultMaximumCore;
			minimumCore = DefaultMinimumCore;
			smallChangeCore = DefaultSmallChangeCore;
			valueCore = minimumCore;
		}
		protected override int LargeChangeCore { get { return largeChangeCore; } set { largeChangeCore = value; } }
		protected override int MaximumCore { get { return maximumCore; } set { maximumCore = value; } }
		protected override int MinimumCore { get { return minimumCore; } set { minimumCore = value; } }
		protected override int SmallChangeCore { get { return smallChangeCore; } set { smallChangeCore = value; } }
		protected override int ValueCore { get { return valueCore; } set { valueCore = value; } }
		protected override Size DefaultSize { get { return new Size(80, 17); } }
	}
	[Description("Allows you to implement vertical scrolling in containers that do not provide their own scroll bars."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	  ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "VScrollBar")
	]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	public class  VScrollBar: VScrollBarBase {
		int largeChangeCore, maximumCore, minimumCore, smallChangeCore, valueCore;
		public VScrollBar() {
			largeChangeCore = DefaultLargeChangeCore;
			maximumCore = DefaultMaximumCore;
			minimumCore = DefaultMinimumCore;
			smallChangeCore = DefaultSmallChangeCore;
			valueCore = minimumCore;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		protected override int LargeChangeCore { get { return largeChangeCore; } set { largeChangeCore = value; } }
		protected override int MaximumCore { get { return maximumCore; } set { maximumCore = value; } }
		protected override int MinimumCore { get { return minimumCore; } set { minimumCore = value; } }
		protected override int SmallChangeCore { get { return smallChangeCore; } set { smallChangeCore = value; } }
		protected override int ValueCore { get { return valueCore; } set { valueCore = value; } }
		protected override Size DefaultSize { get { return new Size(17, 80); } }
	}
	public class ScrollArgs {
		int _largeChange, _smallChange, fValue, _maximum, _minimum;
		bool fEnabled, fireEventsOnAssign = true;
		public ScrollArgs(DevExpress.XtraEditors.ScrollBarBase scroll) {
			this.fEnabled = scroll.Enabled;
			this._maximum = scroll.Maximum;
			this._minimum = scroll.Minimum;
			this._largeChange = scroll.LargeChange;
			this._smallChange = scroll.SmallChange;
			this.fValue = scroll.Value;
		}
		public ScrollArgs() {
			this._largeChange = 1;
			this._smallChange = 1;
			this.fValue = 0;
			this._maximum = 0;
			this._minimum = 0;
			this.fEnabled = true;
		}
		public bool FireEventsOnAssign {
			get { return fireEventsOnAssign; }
			set { fireEventsOnAssign = value; }
		}
		public virtual bool IsEquals(ScrollArgs args) {
			if(this.Enabled == args.Enabled && this.LargeChange == args.LargeChange && 
				this.SmallChange == args.SmallChange && this.Maximum == args.Maximum && 
				this.Value == args.Value && this.Minimum == args.Minimum) return true;
			return false;
		}
		public int LargeChange { 
			get { return _largeChange; } 
			set { 
				if(value < 0) value = 0;
				_largeChange = value; 
			} 
		}
		public int SmallChange { 
			get { return _smallChange; } 
			set { 
				if(value < 0) value = 0;
				_smallChange = value; 
			} 
		}
		public int Value { get { return fValue; } set { fValue = value; } }
		public int Minimum { 
			get { return _minimum; } 
			set { 
				_minimum = value; 
			} 
		}
		public int Maximum { 
			get { return _maximum; } 
			set { 
				_maximum = value; 
			} 
		}
		public bool Enabled { get { return fEnabled; } set { fEnabled = value; } }
		public virtual void Check() {
			if(!Enabled) {
				Minimum = LargeChange = SmallChange = Value = Maximum = 0;
			} else {
				if(Value < Minimum) Value = Minimum;
				if(Maximum < Minimum) Maximum = Minimum;
				if(Value > Maximum) Value = Maximum;
				if(Value + LargeChange > Maximum)
					Value = Maximum - LargeChange;
			}
		}
		public virtual void AssignTo(DevExpress.XtraEditors.ScrollBarBase scroll) {
			scroll.Assign(this);
		}
	}
}
