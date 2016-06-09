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
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.Utils.Drawing {
	public class XPObjectInfoArgs : ObjectInfoArgs {
		WXPPainterArgs drawArgs;
		public XPObjectInfoArgs() {
		}
		public WXPPainterArgs DrawArgs { get { return drawArgs; } set { drawArgs = value; } }
	}
	public class ExpandButtonWindowsXpPainter : XPObjectPainter {
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			if(info == null) return 0;
			return info.Button.Properties.Checked ? XPConstants.GLPS_OPENED : XPConstants.GLPS_CLOSED;
		}
	}
	public class XPObjectPainter : ObjectPainter {
		WXPPainterArgs drawArgs;
		public virtual WXPPainterArgs DrawArgs { get { return drawArgs; } set { drawArgs = value; } }
		protected virtual void UpdateDrawArgs(ObjectInfoArgs e) {
			if(drawArgs == null) return;
			this.drawArgs.ThemeHandle = IntPtr.Zero;
			this.drawArgs.Bounds = e.Bounds;
			this.drawArgs.StateId = CalcThemeStateId(e);
		}
		protected virtual int CalcButtonStateId(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) return XPConstants.PBS_DISABLED;
			if((e.State & ObjectState.Pressed) != 0) return XPConstants.PBS_PRESSED;
			if((e.State & ObjectState.Hot) != 0) return XPConstants.PBS_HOT; 
			if(e.State == ObjectState.Selected && AllowDefaultedStateId(e)) return XPConstants.PBS_DEFAULTED;
			return XPConstants.PBS_NORMAL;
		}
		protected virtual bool AllowDefaultedStateId(ObjectInfoArgs e) {
			return false; 
		}
		protected virtual int CalcThemeStateId(ObjectInfoArgs e) { return 0; } 
		public Rectangle GetObjectClientRectangle(ObjectInfoArgs e, int margin) {
			UpdateDrawArgs(e);
			Rectangle m = WXPPainter.Default.GetThemeMargins(DrawArgs, margin);
			Rectangle res = e.Bounds;
			res.X += m.X; res.Width -= (m.X + m.Width);
			res.Y += m.Y; res.Height -= (m.Y + m.Height);
			return res;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			UpdateDrawArgs(e);
			return WXPPainter.Default.GetThemeContentRect(DrawArgs);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			UpdateDrawArgs(e);
			return WXPPainter.Default.GetThemeBounds(DrawArgs, client);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			UpdateDrawArgs(e);
			return new Rectangle(Point.Empty, WXPPainter.Default.GetThemeSize(DrawArgs, true));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			UpdateDrawArgs(e);
			Rectangle saveBounds = DrawArgs.Bounds;
			DrawArgs.Bounds = e.CalcRectangle(DrawArgs.Bounds);
			try {
				WXPPainter.Default.DrawTheme(DrawArgs, e.Graphics, null);
			}
			finally {
				DrawArgs.Bounds = saveBounds;
			}
		}
		[Obsolete("Use DrawObjectEdge"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DrawObjectEdje(ObjectInfoArgs e) {
			DrawObjectEdge(e);
		}
		public void DrawObjectEdge(ObjectInfoArgs e) {
			UpdateDrawArgs(e);
			Rectangle saveBounds = DrawArgs.Bounds;
			DrawArgs.Bounds = e.CalcRectangle(DrawArgs.Bounds);
			try {
				WXPPainter.Default.DrawThemeEdge(DrawArgs, e.Graphics, null);
			}
			finally {
				DrawArgs.Bounds = saveBounds;
			}
		}
	}
	public class XPToolTipInfoArgs : XPObjectInfoArgs {
		bool isStandard;
		public XPToolTipInfoArgs(bool isStandard) {
			this.isStandard = isStandard;
		}
		public bool IsStandard { get { return this.isStandard; } }
	}
	public class XPToolTipPainter : XPObjectPainter {
		public XPToolTipPainter() {
			DrawArgs = new WXPPainterArgs("tooltip", XPConstants.TTP_STANDARD, XPConstants.TTSS_NORMAL);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			XPToolTipInfoArgs ee = e as XPToolTipInfoArgs;
			DrawArgs.PartId = ee.IsStandard ? XPConstants.TTP_STANDARD : XPConstants.TTP_BALLOON;
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			XPToolTipInfoArgs ee = e as XPToolTipInfoArgs;
			return ee.IsStandard ? XPConstants.TTSS_NORMAL : XPConstants.TTBS_NORMAL;
		}
	}
	public class XPCheckBoxInfoArgs : ObjectInfoArgs {
		bool isRadioButton;
		CheckState checkState;
		public XPCheckBoxInfoArgs(bool isRadio, CheckState state) {
			this.isRadioButton = isRadio;
			this.checkState = state;
		}
		public CheckState CheckState { get { return checkState;  } set { checkState = value; } }
		public bool IsRadioButton { get { return isRadioButton; } set { isRadioButton = value; } }
	}
	public class XPCheckBoxPainter : XPObjectPainter {
		public XPCheckBoxPainter() {
			DrawArgs = new WXPPainterArgs("button", XPConstants.BP_CHECKBOX, 0);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			XPCheckBoxInfoArgs ee = e as XPCheckBoxInfoArgs;
			DrawArgs.PartId = ee.IsRadioButton ? XPConstants.BP_RADIOBUTTON : XPConstants.BP_CHECKBOX;
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			XPCheckBoxInfoArgs ee = e as XPCheckBoxInfoArgs;
			int state = CalcButtonStateId(e);
			switch(ee.CheckState) {
				case CheckState.Checked : state += 4; break;
				case CheckState.Indeterminate : state += 8; break;
			}
			return state;
		}
	}
	public enum XPTabPageLocation { Left, Top, Right, Bottom }
	public enum XPTabPagePosition { Default, Single, Left, Center, Right }
	public class XPTabPageObjectInfoArgs : StyleObjectInfoArgs {
		XPTabPageLocation location;
		XPTabPagePosition position;
		public XPTabPageObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject style, ObjectState state, XPTabPageLocation location) : this(cache, bounds, style, state, location, XPTabPagePosition.Default) { }
		public XPTabPageObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject style, ObjectState state, XPTabPageLocation location, XPTabPagePosition position) : base(cache, bounds, style, state) {
			this.location = location;
			this.position = position;
		}
		public XPTabPageLocation Location { get { return location; } set { location = value; } }
		public XPTabPagePosition Position { get { return position; } set { position = value; } }
	}
	public class XPTabPagePainter : XPObjectPainter {
		public XPTabPagePainter() {
			DrawArgs = new WXPPainterArgs("tab", XPConstants.TABP_TABITEMRIGHTEDGE, 0);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			DrawArgs.PartId = CalcThemePartId(e);
		}
		protected int CalcThemePartId(ObjectInfoArgs e) {
			int res = XPConstants.TABP_TABITEM;
			XPTabPageObjectInfoArgs ee = e as XPTabPageObjectInfoArgs;
			switch(ee.Position) {
				case XPTabPagePosition.Center: res = XPConstants.TABP_TABITEM; break;
				case XPTabPagePosition.Left: res = XPConstants.TABP_TABITEMLEFTEDGE; break;
				case XPTabPagePosition.Right: res = XPConstants.TABP_TABITEMRIGHTEDGE; break;
				case XPTabPagePosition.Single: res = XPConstants.TABP_TABITEMBOTHEDGE; break;
			}
			return res;
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) return XPConstants.TIS_DISABLED;
			if((e.State & ObjectState.Selected) != 0) return XPConstants.TIS_SELECTED;
			if((e.State & ObjectState.Hot) != 0) return XPConstants.TIS_HOT;
			return XPConstants.TIS_NORMAL;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e), res;
			res = r;
			XPTabPageObjectInfoArgs ee = e as XPTabPageObjectInfoArgs;
			switch(ee.Location) {
				case XPTabPageLocation.Right :
					res = Rectangle.FromLTRB(e.Bounds.Left + (r.Bottom - e.Bounds.Bottom),  e.Bounds.Top + (r.Left - e.Bounds.Left), e.Bounds.Right + (e.Bounds.Top - r.Top), e.Bounds.Bottom + (r.Right - e.Bounds.Right));
					break;
				case XPTabPageLocation.Bottom:
					res = Rectangle.FromLTRB(r.X, e.Bounds.Top + (r.Bottom - e.Bounds.Bottom), r.Right, e.Bounds.Bottom + (r.Top - e.Bounds.Top));
					break;
				case XPTabPageLocation.Left:
					res = Rectangle.FromLTRB(e.Bounds.Left + (r.Top - e.Bounds.Top),  e.Bounds.Top + (r.Right - e.Bounds.Right), e.Bounds.Right + (r.Bottom - e.Bounds.Bottom), e.Bounds.Bottom + (r.Left - e.Bounds.Left));
					break;
			}
			return r;
		}
		protected virtual void DrawTopTab(ObjectInfoArgs e) {
			XPTabPageObjectInfoArgs ee = e as XPTabPageObjectInfoArgs;
			Size topSize = e.Bounds.Size;
			if(ee.Location == XPTabPageLocation.Left || ee.Location == XPTabPageLocation.Right) {
				topSize.Width = e.Bounds.Height;
				topSize.Height = e.Bounds.Width;
			}
			Bitmap bmp = BitmapRotate.CreateBufferBitmap(topSize, true);
			BitmapRotate.PrepareBitmap(ee.Cache, new Rectangle(Point.Empty, topSize), ee.Appearance);
			e.Bounds = new Rectangle(0, 0, topSize.Width, topSize.Height + 1 + (ee.State == ObjectState.Selected ? 0 : 0));
			e.Cache = BitmapRotate.BufferCache;
			base.DrawObject(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			XPTabPageObjectInfoArgs ee = e as XPTabPageObjectInfoArgs;
			GraphicsCache save = e.Cache;
			Rectangle saveBounds = e.Bounds;
			try {
				DrawTopTab(e);
			} finally {
				e.Bounds = saveBounds;
				e.Cache = save;
			}
			RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
			switch(ee.Location) {
				case XPTabPageLocation.Left:
					rotate = RotateFlipType.Rotate270FlipNone;
					break;
				case XPTabPageLocation.Right :
					rotate = RotateFlipType.Rotate90FlipNone;
					break;
				case XPTabPageLocation.Bottom:
					rotate = RotateFlipType.RotateNoneFlipY;
					break;
			}
			BitmapRotate.RotateBitmap(rotate);
			e.Paint.DrawImage(e.Graphics, BitmapRotate.BufferBitmap, e.Bounds, new Rectangle(Point.Empty, e.Bounds.Size), true);
		}
	}
	public class XPTabPageClientPainter : XPObjectPainter {
		public XPTabPageClientPainter() {
			DrawArgs = new WXPPainterArgs("tab", XPConstants.TABP_PANE, 0);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = GetObjectClientRectangle(e);
			ObjectInfoArgs body = new ObjectInfoArgs(e.Cache, r, ObjectState.Normal);
			try {
				base.DrawObject(e);
			}
			catch {
			}
		}
	}
	public class XPButtonEditBorderPainter : XPTextBorderPainter {
		public XPButtonEditBorderPainter() {
			DrawArgs = new WXPPainterArgs("listview", XPConstants.LVP_EMPTYTEXT, 0);
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			return 0;
		}
	}
	public class XPTextBorderPainter : XPObjectPainter {
		public XPTextBorderPainter() {
			DrawArgs = new WXPPainterArgs("edit", XPConstants.EP_EDITBORDER_NOSCROLL, 0);
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			BorderObjectInfoArgs args = (BorderObjectInfoArgs)e;
			if(args.State == ObjectState.Hot)
				return XPConstants.EPSH_HOT;
			return 0;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			try {
				IntPtr hdc = e.Cache.Graphics.GetHdc();
				e.Cache.Graphics.ReleaseHdc(hdc);
				Rectangle r = e.Bounds;
				r.Inflate(-1, -1);
				int savedDC = NativeMethods.SaveDC(hdc);
				NativeMethods.ExcludeClipRect(hdc, r.X, r.Y, r.Right, r.Bottom);
				base.DrawObject(e);
				NativeMethods.RestoreDC(hdc, savedDC);
			}
			catch {
			}
		}
	}
	public class XPHeaderPainter : XPObjectPainter {
		public XPHeaderPainter() {
			DrawArgs = new WXPPainterArgs("header", XPConstants.HP_HEADERITEM, 0);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			DrawArgs.PartId = XPConstants.HP_HEADERITEM; 
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			if((e.State & ObjectState.Pressed) != 0) return XPConstants.HIS_PRESSED;
			if((e.State & ObjectState.Hot) != 0) return XPConstants.HIS_HOT;
			return XPConstants.HIS_NORMAL;
		}
	}
	public class WindowsXPSortedShapeObjectPainter : FlatSortedShapeObjectPainter{
	}
	public class WindowsXPTextBorderPainterBase : BorderPainter {
		XPObjectPainter xpPainter;
		public WindowsXPTextBorderPainterBase() {
			xpPainter = CreateXPPainter();
		}
		protected virtual XPObjectPainter CreateXPPainter() {
			return new XPTextBorderPainter();
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		protected XPObjectPainter XPPainter { get { return xpPainter; } }
		public override void DrawObject(ObjectInfoArgs e) {
			XPPainter.DrawObject(e);
		}
	}
	public class WindowsXPTextBorderPainter : WindowsXPTextBorderPainterBase {
		protected override XPObjectPainter CreateXPPainter() {
			return new XPTextBorderPainter();
		}
	}
	public class WindowsXPButtonEditBorderPainter : WindowsXPTextBorderPainterBase {
		protected override XPObjectPainter CreateXPPainter() {
			return new XPButtonEditBorderPainter();
		}
	}
	public class XPSizeGripPainter : XPObjectPainter {
		public XPSizeGripPainter() {
			DrawArgs = new WXPPainterArgs("status", XPConstants.SP_GRIPPER, 0);
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			return 0;
		}
	}
	public class XPProgressBarPainter : XPObjectPainter {
		bool isBorderPainter;
		public XPProgressBarPainter(bool isBorderPainter) {
			this.isBorderPainter = isBorderPainter;
			DrawArgs = new WXPPainterArgs("PROGRESS", 1, 0);
		}
		protected virtual bool IsBorderPainter { get { return isBorderPainter; } }
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			ProgressBarObjectInfoArgs ee = e as ProgressBarObjectInfoArgs;
			if(NativeVista.IsWindows7) {
				if(IsBorderPainter)
					DrawArgs.PartId = ee.IsVertical ? XPConstants.PP_TRANSPARENTBARVERT : XPConstants.PP_TRANSPARENTBAR;
				else
					DrawArgs.PartId = ee.IsVertical ? XPConstants.PP_FILLVERT : XPConstants.PP_FILL;
			}
			else {
				if(IsBorderPainter)
					DrawArgs.PartId = ee.IsVertical ? XPConstants.PP_BARVERT : XPConstants.PP_BAR;
				else
					DrawArgs.PartId = ee.IsVertical ? XPConstants.PP_CHUNKVERT : XPConstants.PP_CHUNK;
			}
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) { return 0; }
	}
	public class WindowsXPProgressBarObjectPainter : ProgressBarObjectPainter {
		XPProgressBarPainter barPainter;
		public WindowsXPProgressBarObjectPainter() {
			this.barPainter = new XPProgressBarPainter(false);
		}
		protected override bool AllowBroken { get { return false; } }
		public XPProgressBarPainter BarPainter { get { return barPainter; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return BarPainter.CalcObjectMinBounds(e);
		}
		protected override void DrawSolid(ProgressBarObjectInfoArgs e, Rectangle pb) {
			DrawBroken(e, pb);
		}
		protected override void DrawBroken(ProgressBarObjectInfoArgs e, Rectangle pb) {
			Rectangle savedBounds = e.Bounds;
			try {
				e.Bounds = pb;
				BarPainter.DrawObject(e);
			}
			finally {
				e.Bounds = savedBounds;
			}
		}
		protected override int GetChunkSize(ProgressBarObjectInfoArgs e) {
			Rectangle minBounds = CalcObjectMinBounds(e);
			return e.IsVertical ? minBounds.Height : minBounds.Width;
		}
	}
	public class WindowsXPMarqueeProgressBarObjectPainter : WindowsXPProgressBarObjectPainter {
		protected override Rectangle CalcProgressBounds(ProgressBarObjectInfoArgs e) {
			MarqueeProgressBarObjectInfoArgs me = e as MarqueeProgressBarObjectInfoArgs;
			if(me == null) return Rectangle.Empty;
			return CalcMarqueProgressBounds(me);
		}
	}
	public class WindowsXPProgressBarBorderPainter : BorderPainter {
		XPProgressBarPainter barPainter;
		public WindowsXPProgressBarBorderPainter() {
			this.barPainter = new XPProgressBarPainter(true);
		}
		public XPProgressBarPainter BarPainter { get { return barPainter; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return BarPainter.GetObjectClientRectangle(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BarPainter.DrawObject(e);
		}
	}
	public class WindowsXPSizeGripObjectPainter : SizeGripObjectPainter {
		XPSizeGripPainter xpPainter;
		public WindowsXPSizeGripObjectPainter() {
			this.xpPainter = new XPSizeGripPainter();
		}
		protected XPSizeGripPainter XPPainter { get { return xpPainter; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return XPPainter.CalcObjectMinBounds(e);
		}
		protected override void DrawGrip(SizeGripObjectInfoArgs ee, Color backColor, Graphics g, Rectangle bounds) {
			XPPainter.DrawObject(new StyleObjectInfoArgs(new GraphicsCache(g), bounds, null, ObjectState.Normal));
		}
	}
	public class XPButtonPainter : XPObjectPainter {
		public XPButtonPainter() {
			DrawArgs = new WXPPainterArgs("button", XPConstants.BP_PUSHBUTTON, 0);
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			return CalcButtonStateId(e);
		}
		public Brush GetForeBrush(ObjectInfoArgs e) {
			return e.Cache.GetSolidBrush(GetForeColor(e));
		}
		public virtual Color GetForeColor(ObjectInfoArgs e) {
			UpdateDrawArgs(e);
			if((e.State & ObjectState.Disabled) != 0) return SystemColors.GrayText;
			return WXPPainter.Default.GetThemeColor(DrawArgs, XPConstants.TMT_TEXTCOLOR);
		}
	}
	public class XPAdvButtonInfoArgs : ObjectInfoArgs {
		ButtonPredefines button;
		public XPAdvButtonInfoArgs(ButtonPredefines button) {
			this.button = button;
		}
		public ButtonPredefines Button { get { return button; } set { button = value; } }
	}
	public class XPAdvButtonPainterMH : XPAdvButtonPainter {		
		protected virtual Size MinSize { get { return new Size(16, 22); } }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			Rectangle res = base.CalcBoundsByClientRectangle(e, client);
			if((ee.Button == ButtonPredefines.SpinDown ||
			   ee.Button == ButtonPredefines.SpinUp)) {
				return res;
			}
			else {
				res.Width = Math.Max(res.Width, MinSize.Width);
				res.Height = Math.Max(res.Height, MinSize.Height);
			}
			return res;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			Rectangle res = base.CalcObjectMinBounds(e);
			if((ee.Button != ButtonPredefines.SpinDown &&
			   ee.Button != ButtonPredefines.SpinUp)) {
				res.Width = Math.Max(res.Width, MinSize.Width);
				res.Height = Math.Max(res.Height, MinSize.Height);
			}
			return res;
		}
	}
	public class XPAdvButtonPainter : XPButtonPainter {
		public XPAdvButtonPainter() {
			DrawArgs = new WXPPainterArgs("button", XPConstants.BP_PUSHBUTTON, 0);
		}
		protected override bool AllowDefaultedStateId(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			return ee.Button == ButtonPredefines.Glyph;
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			UpdateThemeNamePart(e);
			base.UpdateDrawArgs(e);
		}
		public virtual bool IsCustomContentButton(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			int style = (int)ee.Button;
			if(style >= 0) return true;
			return false;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			Rectangle res = base.CalcObjectMinBounds(e);
			if((ee.Button == ButtonPredefines.SpinDown ||
			   ee.Button == ButtonPredefines.SpinUp)) {
				res.Width = 16;
			}
			return res;
		}
		protected virtual void UpdateThemeNamePart(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			switch(ee.Button) {
				case ButtonPredefines.Down : 
				case ButtonPredefines.Up : 
				case ButtonPredefines.Left : 
				case ButtonPredefines.Right : 
					DrawArgs.ThemeName = "scrollbar";
					DrawArgs.PartId = XPConstants.SBP_ARROWBTN;
					break;
				case ButtonPredefines.Combo : 
					DrawArgs.ThemeName = "combobox";
					DrawArgs.PartId = XPConstants.CP_DROPDOWNBUTTON;
					break;
				case ButtonPredefines.Close : 
					DrawArgs.ThemeName = "window";
					DrawArgs.PartId = XPConstants.WP_SMALLCLOSEBUTTON;
					break;
				case ButtonPredefines.SpinUp : 
					DrawArgs.ThemeName = "spin";
					DrawArgs.PartId = XPConstants.SPNP_UP;
					break;
				case ButtonPredefines.SpinDown : 
					DrawArgs.ThemeName = "spin";
					DrawArgs.PartId = XPConstants.SPNP_DOWN;
					break;
				case ButtonPredefines.SpinLeft : 
					DrawArgs.ThemeName = "spin";
					DrawArgs.PartId = XPConstants.SPNP_DOWNHORZ;
					break;
				case ButtonPredefines.SpinRight : 
					DrawArgs.ThemeName = "spin";
					DrawArgs.PartId = XPConstants.SPNP_UPHORZ;
					break;
				default:
					DrawArgs.ThemeName = "button";
					DrawArgs.PartId = XPConstants.BP_PUSHBUTTON;
					break;
			}
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			XPAdvButtonInfoArgs ee = e as XPAdvButtonInfoArgs;
			int res = CalcButtonStateId(e);
			if(DrawArgs.ThemeName != "scrollbar") return res;
			switch(ee.Button) {
				case ButtonPredefines.Up : break;
				case ButtonPredefines.Down : 
					res += (XPConstants.ABS_DOWNNORMAL - XPConstants.ABS_UPNORMAL);
					break;
				case ButtonPredefines.Left : 
					res += (XPConstants.ABS_LEFTNORMAL - XPConstants.ABS_UPNORMAL);
					break;
				case ButtonPredefines.Right : 
					res += (XPConstants.ABS_RIGHTNORMAL - XPConstants.ABS_UPNORMAL);
					break;
			}
			return res;
		}
	}
	public class WindowsXPHeaderObjectPainter : HeaderObjectPainter {
		public WindowsXPHeaderObjectPainter() : base(new XPHeaderPainter()) {
		}
		public new XPHeaderPainter ButtonPainter { get { return base.ButtonPainter as XPHeaderPainter; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			r.Inflate(0, -2);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = base.CalcBoundsByClientRectangle(e, client);
			r.Inflate(0, 2);
			return r;
		}
	}
	public class WindowsXPOpenCloseButtonObjectPainter : XPObjectPainter {
		public WindowsXPOpenCloseButtonObjectPainter() {
			DrawArgs = new WXPPainterArgs("treeview", XPConstants.TVP_GLYPH, 0);
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			OpenCloseButtonInfoArgs ee = e as OpenCloseButtonInfoArgs;
			if(ee.Opened) return XPConstants.GLPS_OPENED;
			return XPConstants.GLPS_CLOSED;
		}
	}
	public enum Office2003Appearance { Reserved }; 
	public enum Office2003Color { Border, Button1, Button2, Button1Hot, Button2Hot, 
		Button1Pressed, Button2Pressed, ButtonDisabled, Text, TextDisabled, Header, Header2, GroupRow,
		TabPageForeColor, TabBackColor1, TabBackColor2, TabPageClient, TabPageBackColor1, TabPageBackColor2, TabPageBorderColor,
		NavBarBackColor1, NavBarBackColor2, NavBarLinkTextColor, NavBarLinkHighlightedTextColor, NavBarLinkDisabledTextColor, NavBarGroupClientBackColor,
		NavBarGroupCaptionBackColor1, NavBarGroupCaptionBackColor2, NavBarExpandButtonRoundColor, NavPaneBorderColor,
		NavBarNavPaneHeaderBackColor, LinkBorder, GroupRowSeparatorEx, GroupRowEx
	};
	public enum Office2003GridAppearance {
		Header, 
		HeaderHotLine, 
		HeaderPressedLine,
		GroupRow,
		GroupRowIndent,
		GroupRowSeparator,
		GridLine,
		GroupPanel,
		FooterPanel,
		FooterCell,
		FilterPanel
	};
	public class Office2003Colors {
		static AppearanceDefault[][] Office2003Grid = new AppearanceDefault[][] {
			  new AppearanceDefault[] {
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xaecaf0), FromRgb(0x81a9e2), FromRgb(172, 200, 227)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Empty, FromRgb(0xe3a846)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Gray), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xbedafb)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xfdeec9)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xBDD0F8)), 
				  new AppearanceDefault(FromRgb(210, 230, 254), FromRgb(210, 230, 254)),
				  new AppearanceDefault(Color.White, FromRgb(0x2b63b7)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xaecaf0), FromRgb(0x2b63b7)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xddecfe), FromRgb(0x2b63b7)), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xaecaf0), FromRgb(0x2b63b7)) 
			  },
			  new AppearanceDefault[] {
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(226, 225, 192), FromRgb(0xbebd70), FromRgb(221, 220, 179)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Empty, FromRgb(0xe3a846)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Gray), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xafba91)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xfdeec9)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xdfe8c6)), 
				  new AppearanceDefault(Color.Gray, Color.Gray),
				  new AppearanceDefault(Color.White, FromRgb(0xd5deb7)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xd5deb7), FromRgb(0x7b8e4a)), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf4f7de), FromRgb(0x7b8e4a)), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xd5deb7), FromRgb(0x7b8e4a)) 
			  },
			  new AppearanceDefault[] {
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xe1e4e5), FromRgb(0xbdbece), FromRgb(0xdbdae4)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Empty, FromRgb(0xe3a846)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf8e3af), Color.Gray), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xe5e5eb)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xfdeec9)), 
				  new AppearanceDefault(Color.Empty, FromRgb(0xe8e8f0)), 
				  new AppearanceDefault(Color.Gray, Color.Gray), 
				  new AppearanceDefault(Color.White, FromRgb(0xd5deb7)), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xc5c5d7), FromRgb(0x5a577b)),
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xf3f4fa), FromRgb(0x5a577b)), 
				  new AppearanceDefault(SystemColors.ControlText, FromRgb(0xc5c5d7), FromRgb(0x5a577b)) 
			  }
		};
		static int[][] Office11Colors = new int[][] {  
			  new int[] {0xddecfe, 0x81a9e2, 0x9ebef5, 0xc3daf9, 0x3b619c, 0x274176, 0xFFFFFF, 0x6a8ccb, 0xf1f9ff,
			 0x000080, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0x75a6f1, 0x053995, 
			  0xF6F6F6, 0x002d96, 
	   0x2a66c9, 0xc4dbf9,
		0xbedafb, 
	  		 0x7ba2e7, 0x6375d6, 
						 0xd6dff7,		   
						 0x215dc6, 0x428eff, 
						 0xffffff, 0xc6d3f7, 
						 0xb1b9d8,			
						 0xd3d3d3, 
						 0xfdeec9,
						 0xBDD0F8 
													}, 
			  new int[] {0xf4f7de, 0xb7c691, 0xd9d9a7, 0xf2f0e4, 0x608058, 0x515e33, 0xFFFFFF, 0x608058, 0xf4f7de,
			 0x3f5d38, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0xb0c28c, 0x60776b,
			  0xF4F4F4, 0x758d5e, 
	   0x74865e, 0xe1dead,
		0xafba91,
	  		 0xccd9ad, 0xa5bd84, 
						 0xf6f6ec,		   
						 0x56662d, 0x72921d, 
						 0xfffcec, 0xe0e7b8, 
						 0xcad5be,		   
						 0xd3d3d3, 
						 0xfdeec9, 
						 0xdfe8c6 
	},
			  new int[] {0xf3f4fa, 0x9997b5, 0xd7d7e5, 0xf3f3f7, 0x7c7c94, 0x545475, 0xFFFFFF, 0x6e6d8f, 0xFFFFFF,
			 0x000080, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0xb3b2c8, 0x767492, 
			  0xfdfaff, 0x7c7c94, 
	   0x7a7999, 0xdbdae4,
		0xe5e5eb,
	  		 0xc4c8d4, 0xb1b3c8, 
						 0xf0f1f5,		   
						 0x3f3d3d, 0x7e7c7c, 
						 0xffffff, 0xd6d7e0, 
						 0xc4c6d0,		   
						 0xd3d3d3, 
						 0xfdeec9, 
						 0xe8e8f0 
	},
			  };
		Hashtable colors, appearances;
		public Office2003Colors() {
			this.colors = new Hashtable();
			this.appearances = new Hashtable();
			Init();
		}
		static Office2003Colors defaultColors;
		static Office2003Colors() {
			defaultColors = new Office2003Colors();
		}
		public static Office2003Colors Default {
			get { return defaultColors; }
		}
		public XPThemeType ThemeType {
			get { return DevExpress.Utils.WXPaint.WXPPainter.Default.GetXPThemeType(); }
		}
		public virtual void Init() {
			XPThemeType themeType = ThemeType;
			if(themeType != XPThemeType.Unknown) {
				int id = ((int)themeType) - 1;
				InitOfficeColors(id);
			} else {
				InitStandardColors();
			}
			colors[Office2003Color.TabPageForeColor] = colors[Office2003Color.Text];
			colors[Office2003Color.TabBackColor1] = colors[Office2003Color.Button1];
			colors[Office2003Color.TabBackColor2] = colors[Office2003Color.Button2];
			colors[Office2003Color.TabPageBackColor1] = colors[Office2003Color.Button1Pressed];
			colors[Office2003Color.TabPageBackColor2] = colors[Office2003Color.Button2Pressed];
			colors[Office2003Color.TabPageBorderColor] = colors[Office2003Color.Border];
			colors[Office2003Color.GroupRowSeparatorEx] = colors[Office2003Color.Button2];
		}
		protected virtual void InitOfficeColors(int id) {
			colors[Office2003Color.Button1] = FromRgb(Office11Colors[id][0]);
			colors[Office2003Color.Button2] = FromRgb(Office11Colors[id][1]);
			colors[Office2003Color.Border] = FromRgb(Office11Colors[id][9]);
			colors[Office2003Color.Button1Hot] = FromRgb(Office11Colors[id][10]);
			colors[Office2003Color.Button2Hot] = FromRgb(Office11Colors[id][11]);
			colors[Office2003Color.Button1Pressed] = FromRgb(Office11Colors[id][12]);
			colors[Office2003Color.Button2Pressed] = FromRgb(Office11Colors[id][13]);
			colors[Office2003Color.Text] = FromRgb(Office11Colors[id][14]);
			colors[Office2003Color.TextDisabled] = FromRgb(Office11Colors[id][15]);
			colors[Office2003Color.GroupRow] = FromRgb(Office11Colors[id][22]);
			colors[Office2003Color.Header] = GetMiddleRGB(this[Office2003Color.Button1],  this[Office2003Color.Button2], 50);
			colors[Office2003Color.Header2] = this[Office2003Color.Button2]; 
			colors[Office2003Color.Header2] = ControlPaint.Dark(this[Office2003Color.Button2], 0.05f);
			colors[Office2003Color.LinkBorder] = FromRgb(Office11Colors[id][31]);
			colors[Office2003Color.NavBarBackColor1] = FromRgb(Office11Colors[id][23]);
			colors[Office2003Color.NavBarBackColor2] = FromRgb(Office11Colors[id][24]);
			colors[Office2003Color.NavBarGroupClientBackColor] = FromRgb(Office11Colors[id][25]);
			colors[Office2003Color.NavBarLinkTextColor] = FromRgb(Office11Colors[id][26]);
			colors[Office2003Color.NavBarLinkHighlightedTextColor] = FromRgb(Office11Colors[id][27]);
			colors[Office2003Color.NavBarLinkDisabledTextColor] = ControlPaint.Light(this[Office2003Color.NavBarLinkHighlightedTextColor], 0.5f);
			colors[Office2003Color.NavBarGroupCaptionBackColor1] = FromRgb(Office11Colors[id][28]);
			colors[Office2003Color.NavBarGroupCaptionBackColor2] = FromRgb(Office11Colors[id][29]);
			colors[Office2003Color.NavBarExpandButtonRoundColor] = FromRgb(Office11Colors[id][30]);
			colors[Office2003Color.GroupRowEx] = FromRgb(Office11Colors[id][32]);
			colors[Office2003Color.NavPaneBorderColor] = ControlPaint.Dark(SystemColors.Highlight, 0.05f);
			colors[Office2003Color.NavBarNavPaneHeaderBackColor] = SystemColors.Highlight;
			colors[Office2003Color.TabPageClient] = FromRgb(Office11Colors[id][33]);
			appearances[Office2003GridAppearance.Header] = Office2003Grid[id][0];
			appearances[Office2003GridAppearance.HeaderHotLine] = Office2003Grid[id][1];
			appearances[Office2003GridAppearance.HeaderPressedLine] = Office2003Grid[id][2];
			appearances[Office2003GridAppearance.GroupRow] = Office2003Grid[id][3];
			appearances[Office2003GridAppearance.GroupRowIndent] = Office2003Grid[id][4];
			appearances[Office2003GridAppearance.GroupRowSeparator] = Office2003Grid[id][5];
			appearances[Office2003GridAppearance.GridLine] = Office2003Grid[id][6];
			appearances[Office2003GridAppearance.GroupPanel] = Office2003Grid[id][7];
			appearances[Office2003GridAppearance.FooterPanel] = Office2003Grid[id][8];
			appearances[Office2003GridAppearance.FooterCell] = Office2003Grid[id][9];
			appearances[Office2003GridAppearance.FilterPanel] = Office2003Grid[id][10];
		}
		protected virtual Hashtable Colors { get { return colors; } }
		protected virtual Hashtable Appearances { get { return appearances; } }
		public AppearanceDefault this[object appearanceKey] {
			get { 
				AppearanceDefault res = Appearances[appearanceKey] as AppearanceDefault; 
				if(res == null) res = AppearanceDefault.Control;
				return res;
			}
		}
		public Color this[Office2003Color color] {
			get { 
				object val = Colors[color];
				if(val == null) return SystemColors.Control;
				return (Color)val;
			}
		}
		protected virtual void InitStandardColors() {
			colors[Office2003Color.Button1] = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 22);
			colors[Office2003Color.Button2] = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 96);
			colors[Office2003Color.Border] = SystemColors.Highlight;
			colors[Office2003Color.Button2Hot] = colors[Office2003Color.Button1Hot] = GetRealColor(GetLightColor(-2, 30, 72));
			colors[Office2003Color.Button1Pressed] = colors[Office2003Color.Button2Pressed] = GetRealColor(GetLightColor(14, 44, 40));
			colors[Office2003Color.Text] = SystemColors.ControlText;
			colors[Office2003Color.TextDisabled] = SystemColors.GrayText;
			colors[Office2003Color.Header] = GetMiddleRGB(this[Office2003Color.Button1],  this[Office2003Color.Button2], 50);
			colors[Office2003Color.Header2] = ControlPaint.Dark(this[Office2003Color.Button2], 0.5f);
			colors[Office2003Color.GroupRow] = SystemColors.Control;
			colors[Office2003Color.LinkBorder] = CalcColor(SystemColors.Window, .29f, SystemColors.Control, .72f);
			colors[Office2003Color.NavBarBackColor1] = CalcNavColor(-10);
			colors[Office2003Color.NavBarBackColor2] = CalcNavColor(-29);
			colors[Office2003Color.NavBarGroupClientBackColor] = ControlPaint.LightLight(SystemColors.InactiveCaption);
			colors[Office2003Color.NavBarLinkTextColor] = CalcNavColor(-50);
			colors[Office2003Color.NavBarLinkHighlightedTextColor] = ControlPaint.Light(CalcNavColor(-50));
			if(this[Office2003Color.NavBarLinkTextColor] == Color.FromArgb(0, 0x15, 0x5b))
				colors[Office2003Color.NavBarLinkDisabledTextColor] = Color.Gray;
			else
				colors[Office2003Color.NavBarLinkDisabledTextColor] = ControlPaint.LightLight(CalcNavColor(-50));
			colors[Office2003Color.NavBarGroupCaptionBackColor1] = SystemColors.Window;
			colors[Office2003Color.NavBarGroupCaptionBackColor2] = ControlPaint.LightLight(SystemColors.Highlight);
			colors[Office2003Color.NavBarExpandButtonRoundColor] = SystemColors.ControlDark;
			colors[Office2003Color.NavPaneBorderColor] = ControlPaint.Dark(GetLightColor(40, 0, 0), 0.05f);
			colors[Office2003Color.NavBarNavPaneHeaderBackColor] = GetLightColor(40, 0, 0);
			colors[Office2003Color.GroupRowEx] = SystemColors.Control;
			colors[Office2003Color.TabPageClient] = colors[Office2003Color.Header];
			appearances[Office2003GridAppearance.Header] = 
				new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control);
			appearances[Office2003GridAppearance.HeaderHotLine] = 
				new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlLightLight, Color.Empty, SystemColors.ControlLight);
			appearances[Office2003GridAppearance.HeaderPressedLine] = 
				new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlLightLight, SystemColors.ControlDark);
			appearances[Office2003GridAppearance.GroupRow] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[Office2003GridAppearance.GroupRowIndent] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[Office2003GridAppearance.GroupRowSeparator] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[Office2003GridAppearance.GridLine] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			appearances[Office2003GridAppearance.GroupPanel] = new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark);
			appearances[Office2003GridAppearance.FooterPanel] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark);
			appearances[Office2003GridAppearance.FooterCell] = new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlLight, SystemColors.ControlDark);
			appearances[Office2003GridAppearance.FilterPanel] = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark);
		}
		public static Color CalcNavColor(int d) {
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
		public static Color CalcColor(Color color1, float percent1, Color color2, float percent2) {
			return CalcColor(color1, percent1, color2, percent2, Color.Empty, 0f);
		}
		public static Color CalcColor(Color color1, float percent1, Color color2, float percent2, Color color3, float percent3 ) {
			percent1 = Math.Max(0, Math.Min(1, percent1));
			percent2 = Math.Max(0, Math.Min(1, percent2));
			percent3 = Math.Max(0, Math.Min(1, percent3));
			int r = (int)(color1.R * percent1 + color2.R * percent2 + color3.R * percent3);
			int g = (int)(color1.G * percent1 + color2.G * percent2 + color3.G * percent3);
			int b = (int)(color1.B * percent1+ color2.B * percent2 + color3.B * percent3);
			r = Math.Min(r, 255);
			g = Math.Min(g, 255);
			b = Math.Min(b, 255);
			return Color.FromArgb(r, g, b);
		}
		public static Color FromRgb(int r, int g, int b) { return Color.FromArgb(r, g, b); }
		public static Color FromRgb(int rgb) { return Color.FromArgb((int)(rgb + (uint)0xff000000)); }
		Color GetRealColor(Color clr) {
			return DevExpress.Utils.ColorUtils.GetRealColor(clr);
		}
		Color GetLightColor(int btnFaceColorPart, int highlightColorPart, int windowColorPart) {
			return DevExpress.Utils.ColorUtils.GetLightColor(btnFaceColorPart, highlightColorPart, windowColorPart);
		}
		int CalcValue(int v1, int v2, int percent) {
			int i;
			i = (v1 * percent) / 100 + (v2 * (100 - percent)) / 100;
			if(i > 255) i = 255;
			return i;
		}
		Color GetMiddleRGB(Color clr1, Color clr2, int percent) {
			Color r = Color.FromArgb(
				CalcValue(clr1.R, clr2.R, percent),CalcValue(clr1.G, clr2.G, percent),CalcValue(clr1.B, clr2.B, percent));
			return r;
		}
	}
}
