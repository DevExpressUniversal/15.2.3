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
using DevExpress.XtraNavBar;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class XP2NavBarViewInfo : NavBarViewInfo {
		public XP2NavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		protected override NavBarHitTest[] CreateValidHotTracks() {
			return new NavBarHitTest[] {
										   NavBarHitTest.UpButton, NavBarHitTest.DownButton,
										   NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton,
										   NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link };
		}
		protected override NavBarHitTest[] CreateValidPressedInfo() {
			return new NavBarHitTest[] {
										   NavBarHitTest.UpButton, NavBarHitTest.DownButton,
										   NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton,
										   NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link };
		}
		protected override NavBarState[] CreateValidPressedStateInfo() {
			return new NavBarState[] {
										 NavBarState.UpButtonPressed, NavBarState.DownButtonPressed,
										 NavBarState.GroupPressed, NavBarState.GroupPressed,
										 NavBarState.LinkPressed, NavBarState.LinkPressed, NavBarState.LinkPressed };
		}
		protected override BorderPainter CreateBorderPainter() {
			return new XP2ViewBorderPainter();
		}
	}
	public class XP2ViewBorderPainter : BorderPainter {
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return new Rectangle(client.X, client.Y, client.Width, client.Height + 1);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
		}
		protected Color GetBorderColor() {
			const int LBCP_BORDER_NOSCROLL = 3;
			const int NORMAL_STATE = 1;
			const int TMT_BORDERCOLOR = 3801;
			return DevExpress.Utils.WXPaint.WXPPainter.Default.GetThemeColor(new DevExpress.Utils.WXPaint.WXPPainterArgs("listView", LBCP_BORDER_NOSCROLL, NORMAL_STATE), TMT_BORDERCOLOR);
		}
		Color borderColor = Color.Empty;
		public Color Color { 
			get {
				if(borderColor == Color.Empty) 
					borderColor = GetBorderColor();
				return borderColor;
			} 
		}
		protected virtual void DrawBorderLine(ObjectInfoArgs e) {
			e.Graphics.DrawLine(new Pen(Color), new Point(e.Bounds.X, e.Bounds.Bottom - 1), new Point(e.Bounds.Right - 2, e.Bounds.Bottom - 1));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawBorderLine(e);
		}
	}
	public class XP2NavGroupClientPainter : BaseNavGroupClientPainter {
		public XP2NavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override void DrawBackgroundCore(NavGroupClientInfoArgs e, CustomDrawObjectEventArgs custom) {
			XPObjectPainter bp = (XPObjectPainter)GroupPainter.ButtonPainter;
			bp.Part = 8;
			bp.PartState = 1;
			StyleObjectInfoArgs ba = new StyleObjectInfoArgs(e.Cache, e.ClientBounds, GroupPainter.GetGroupAppearance(e), e.State);
			bp.DrawObject(ba);
		}
	}
	public class XP2NavGroupPainter : FlatNavGroupPainter {
		public XP2NavGroupPainter(NavBarControl navBar) : base(navBar) {
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle res = base.CalcObjectMinBounds(e);
			res.Height += 6;
			return res;
		}
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new XP2NavGroupClientPainter(this); 
		}
		protected override ObjectPainter CreateButtonPainter() {
			return new XPObjectPainter("tab", 1, -1, false);
		}
		protected override Rectangle GetClientBounds(ObjectInfoArgs e) {
			Rectangle bounds = ButtonPainter.GetObjectClientRectangle(GetButtonArgs(e, e.Bounds));			
			bounds.Inflate(-2, 0);
			return bounds;
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			XPObjectPainter bp = (XPObjectPainter)ButtonPainter;
			bp.Part = 1;
			switch(e.State) {
				case ObjectState.Pressed : bp.PartState = 4; break;
				case ObjectState.Hot : bp.PartState = 2; break;
				default : bp.PartState = 1; break;
			}
			base.DrawGroupCaptionCore(e, custom);
		}
	}
	public class XP2NavLinkPainter : BaseNavLinkPainter  {
		ButtonObjectPainter linkButtonPainter;
		public XP2NavLinkPainter(NavBarControl navBar) : base(navBar)  {
			linkButtonPainter = new XPObjectPainter("toolbar", 1, -1);
		}
		public override int GetImageIndent(ObjectInfoArgs e) {
			return 4; 
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			ObjectState state = e.State;
			if(!li.Link.Enabled) state = ObjectState.Disabled;
			Rectangle lb = li.ImageRectangle;
			if(li.Link.Group.GetLinksUseSmallImage()) {
				lb = li.Bounds;
				lb.Inflate(-2, 0);
				lb.Width -= 2;
			}
			StyleObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, lb, GetLinkAppearance(e), state);
			linkButtonPainter.DrawObject(ia);
		}
	}
	public class XP2GroupButtonObjectPainter : FlatButtonObjectPainter {
		ButtonObjectPainter groupButtonPainter;
		public XP2GroupButtonObjectPainter()  {
			groupButtonPainter = new XPObjectPainter("button", 3, -1);
		}
		protected override void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject appearance, Rectangle r) {
			ObjectState state = e.State;
			StyleObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, r, appearance, state);
			groupButtonPainter.DrawObject(ia);
		}
	}
}
