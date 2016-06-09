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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraNavBar;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class XPNavBarViewInfo : NavBarViewInfo {
		public XPNavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
	}
	public class XPNavGroupPainter : FlatNavGroupPainter {
		public XPNavGroupPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override Rectangle GetClientBounds(ObjectInfoArgs e) {
			Rectangle bounds = ButtonPainter.GetObjectClientRectangle(GetButtonArgs(e, e.Bounds));			
			bounds.Inflate(-2, 0);
			return bounds;
		}
	}
	public class XPNavLinkPainter : BaseNavLinkPainter  {
		ButtonObjectPainter linkButtonPainter;
		public XPNavLinkPainter(NavBarControl navBar) : base(navBar)  {
			linkButtonPainter = new XPObjectPainter("toolbar", 1, -1);
		}
		public override int GetImageIndent(ObjectInfoArgs e) {
			return 4;
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			ObjectState state = e.State;
			if(!li.Link.Enabled) state = ObjectState.Disabled;
			StyleObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, li.ImageRectangle, GetLinkAppearance(e), state);
			linkButtonPainter.DrawObject(ia);
		}
	}
	public class XPUpDownButtonObjectPainter : UpDownButtonObjectPainter  {
		XPButtonObjectPainter buttonPainter;
		public XPUpDownButtonObjectPainter() {
			buttonPainter = new XPButtonObjectPainter(ButtonPredefines.Up);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return buttonPainter.CalcObjectMinBounds(e);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			UpDownButtonObjectInfoArgs args = e as UpDownButtonObjectInfoArgs;
			buttonPainter.ButtonType = args.IsUpButton ? ButtonPredefines.Up : ButtonPredefines.Down;
			buttonPainter.DrawObject(e);
		}
	}
	public class XPObjectPainter : XPButtonObjectPainter {
		string themeName;
		int part, partState;
		bool fillBackground;
		public XPObjectPainter(string themeName) : this(themeName, -1, -1) {
		}
		public XPObjectPainter(string themeName, int part, int partState) : this(themeName, part, partState, false) {
		}
		public XPObjectPainter(string themeName, int part, int partState, bool fillBackground) {
			this.themeName = themeName;
			this.part = part;
			this.partState = partState;
			this.fillBackground = fillBackground;
		}
		public string ThemeName { get { return themeName; } }
		public override NativeControlPaintArgs CreateNativeArgs(ObjectInfoArgs e) {
			return new NativeControlAdvPaintArgs(e.Graphics, e.CalcRectangle(e.Bounds), ButtonType, CalcButtonState(e), GetStyle(e).GetBackBrush(e.Cache), e.State != ObjectState.Disabled, themeName, part, partState, fillBackground);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = base.CalcBoundsByClientRectangle(e, client);
			if(r.Height < client.Height) r.Height = client.Height;
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectMinBounds(e);
			if(r.Height < 8) r.Height = 8;
			return r;
		}
		public int Part { 
			get { return part; }
			set { part = value; }
		}
		public int PartState { 
			get { return partState; }
			set { partState = value; }
		}
	}
	public class XPButtonObjectPainter : ButtonObjectPainter {
		ButtonPredefines buttonType;
		public XPButtonObjectPainter() : this(ButtonPredefines.OK) {
		}
		public XPButtonObjectPainter(ButtonPredefines type) {
			this.buttonType = type;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public virtual ButtonPredefines ButtonType { 
			get { return buttonType; } 
			set { buttonType = value; }
		}
		protected virtual ButtonStates CalcButtonState(ObjectInfoArgs e) {
			switch(e.State) {
				case ObjectState.Hot : return ButtonStates.Hottrack;
				case ObjectState.Selected : 
				case ObjectState.Pressed : return ButtonStates.Push;
				case ObjectState.Disabled : return ButtonStates.Disabled;
			}
			return ButtonStates.None;
		}
		public virtual NativeControlPaintArgs CreateNativeArgs(ObjectInfoArgs e) {
			return new NativeControlAdvPaintArgs(e.Graphics, e.CalcRectangle(e.Bounds), ButtonType, CalcButtonState(e), GetStyle(e).GetBackBrush(e.Cache), e.State != ObjectState.Disabled, "", 0, 0, false);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			return DevExpress.Utils.WXPaint.Painter.CalcContentRect(native);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			native.Bounds = client;
			return DevExpress.Utils.WXPaint.Painter.CalcButtonBounds(native);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			return new Rectangle(Point.Empty, DevExpress.Utils.WXPaint.Painter.CalcButtonMinSize(native));
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			DevExpress.Utils.WXPaint.Painter.DrawButton(native);
		}
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject style) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			DevExpress.Utils.WXPaint.Painter.DrawButton(native);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			NativeControlPaintArgs native = CreateNativeArgs(e);
			DevExpress.Utils.WXPaint.Painter.DrawButton(native);
		}
	}
}
