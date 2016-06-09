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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class VSToolBoxNavBarViewInfo : NavBarViewInfo {
		public VSToolBoxNavBarViewInfo(NavBarControl navBar) : base(navBar) { }
		protected override int CalcGroupVertIndent(NavGroupInfoArgs ia) { return 1;	}
		public override bool AllowSelectDisabledLink { get { return true; } }
		public override bool AllowOnlySmallImages { get { return true; } }
		public override bool AllowHideGroupCaptions { get { return false; } }
		public override bool AllowTooltipController { get { return false; } }
		public override void OnMouseWheel(MouseEventArgs e) {
			if(GetFocusedControl() is NavBarGroupControlContainer) {
				return;
			}
			if(NavBar.ActiveGroup != null) {
				if(e.Delta < 0 && CalcButtonState(NavBarState.Normal, NavBarHitTest.DownButton) == ObjectState.Disabled) return;
				DoMoveTopLinkIndex(NavBar.ActiveGroup, e.Delta > 0 ? -1 : 1);
			}
		}
		const int MaxButtonWidth = 18;
		protected void CalcButtonBounds(NavGroupInfoArgs group, ref Rectangle buttonBounds) {
			Rectangle r = group.Bounds;
			Size size = r.Size;
			size.Width = size.Height;
			if(size.Width > MaxButtonWidth) size.Width = MaxButtonWidth;
			buttonBounds = r;
			buttonBounds.X = r.Right - (size.Width + CalcButtonIndent(IndentType.Right));
			buttonBounds.Size = size;
			r.Width = (buttonBounds.X - group.Bounds.X) - 2;
			group.Bounds = r;
			NavBar.GroupPainter.CalcObjectBounds(group);
		}
		protected override void CalcButtonPositions(NavGroupInfoArgs activeGroupArgs, bool updateScrollbar) {
			if(activeGroupArgs == null) {
				base.CalcButtonPositions(activeGroupArgs, updateScrollbar);
				return;
			}
			Rectangle buttonBounds = Rectangle.Empty;
			CalcButtonBounds(activeGroupArgs, ref buttonBounds);
			DownButtonBounds = UpButtonBounds = buttonBounds;
			int index = Groups.IndexOf(activeGroupArgs);
			if(index > -1 && index < Groups.Count - 1) {
				NavGroupInfoArgs nextGroup = Groups[index + 1] as NavGroupInfoArgs;
				Rectangle button = DownButtonBounds;
				CalcButtonBounds(nextGroup, ref button);
				DownButtonBounds = button;
			} else {
				buttonBounds.Y = Client.Bottom - buttonBounds.Height;
				DownButtonBounds = buttonBounds;
			}
			DownButtonBounds = CheckBounds(DownButtonBounds);
			UpButtonBounds = CheckBounds(UpButtonBounds);
		}
		protected override int CalcButtonIndent(IndentType indent) { return 0; }
		protected override int CalcInterval(int interval,NavBarHintInfo newHintInfo) {
			NavBarItemLink link = newHintInfo.HintObject as NavBarItemLink;
			if(link != null && !link.Group.GetShowAsIconsView()) return 50;
			return base.CalcInterval(interval, newHintInfo);
		}
		protected override bool GetCanShowHint(NavBarHitInfo hitInfo) {
			if(hitInfo == null) return false;
			NavLinkInfoArgs linkArgs = GetLinkInfo(hitInfo.Link);
			if(linkArgs != null && !linkArgs.Link.Group.GetShowAsIconsView()) {
				NavLinkInfoArgs li = CreateHintLinkArgs(linkArgs, null);
				bool show = true;
				try {
					Size size = li.Link.AllowHtmlString ? NavBar.LinkPainter.CalcHtmlStringSize(li,0) : NavBar.LinkPainter.CalcTextSize(li, 0);
					Size sizeActual = li.Link.AllowHtmlString ? NavBar.LinkPainter.CalcHtmlStringSize(li, linkArgs.CaptionRectangle.Width) : NavBar.LinkPainter.CalcTextSize(li, linkArgs.CaptionRectangle.Width);
					if(size.Height < sizeActual.Height) return false;
					show = size.Width > linkArgs.CaptionRectangle.Width;
				}
				finally {
					DisposeHintLinkArgs(li);
				}
				return show;
			}
			return base.GetCanShowHint(hitInfo);
		}
		NavLinkInfoArgs CreateHintLinkArgs(NavLinkInfoArgs linkArgs, PaintEventArgs e) {
			if(e == null) e = new PaintEventArgs(NavBar.CreateGraphics(), new Rectangle(-9999, 0, 0, 0));
			NavLinkInfoArgs li = new NavLinkInfoArgs(new GraphicsCache(e), linkArgs.Link, new Rectangle(Point.Empty, linkArgs.Bounds.Size), linkArgs.State);
			return li;
		}
		void CalcHintLinkSize(NavLinkInfoArgs li) {
			Size size = li.Bounds.Size;
			Size textSize = NavBar.LinkPainter.CalcTextSize(li, 0);
			li.Bounds = new Rectangle(Point.Empty, size);
			NavBar.LinkPainter.CalcObjectBounds(li);
			size.Width += (textSize.Width - li.CaptionRectangle.Width);
			li.Bounds = new Rectangle(Point.Empty, size);
			NavBar.LinkPainter.CalcObjectBounds(li);
		}
		void DisposeHintLinkArgs(NavLinkInfoArgs li) {
			if(li.Cache == null) return;
			if(li.Cache.PaintArgs.ClipRectangle.X == -9999) li.Graphics.Dispose();
			li.Cache.Dispose();
			li.Cache = null;
		}
		public override void HideHint() {
			NavBarItemLink lastLink = LastHintInfo != null ? LastHintInfo.HintObject as NavBarItemLink : null;
			base.HideHint();
			if(lastLink != null) {
				Application.DoEvents();
				InvalidateLink(lastLink);
			}
		}
		protected override string GetLinkHint(NavBarItemLink link) {
			if(link != null) return link.Caption;
			return base.GetLinkHint(link);
		}
		protected override void OnCustomDrawToolTip(object sender, DevExpress.Utils.Win.ToolTipCustomDrawEventArgs e) {
			NavLinkInfoArgs linkArgs = GetLinkInfo(LastHintInfo.HintObject as NavBarItemLink);
			if(linkArgs == null || (linkArgs != null && linkArgs.Link.Group.GetShowAsIconsView())) {
				base.OnCustomDrawToolTip(sender, e);
				return;
			}
			NavBarCustomDrawHintEventArgs args = new NavBarCustomDrawHintEventArgs(LastHintInfo,  e.PaintArgs, ((Control)sender).ClientRectangle);
			NavBar.RaiseCusomDrawHint(args);
			e.Handled = args.Handled;
			if(args.Handled) return;
			Size size = new Size(linkArgs.Bounds.Width, linkArgs.Bounds.Height);
			NavLinkInfoArgs li = CreateHintLinkArgs(linkArgs, e.PaintArgs);
			CalcHintLinkSize(li);
			li.Link.Group.AppearanceBackground.FillRectangle(li.Cache, li.Bounds);
			NavBar.LinkPainter.DrawObject(li);
			DisposeHintLinkArgs(li);
			e.Handled = true;
		}
		protected override void OnCalcToolTipSize(object sender, DevExpress.Utils.Win.ToolTipCalcSizeEventArgs e) {
			NavBarItemLink link = LastHintInfo.HintObject as NavBarItemLink;
			if(link != null && !link.Group.GetShowAsIconsView()) {
				NavLinkInfoArgs args = GetLinkInfo(link);
				if(args != null) {
					NavLinkInfoArgs li = CreateHintLinkArgs(args, null);
					CalcHintLinkSize(li);
					e.Size = li.Bounds.Size;
					DisposeHintLinkArgs(li);
				}
			}
			base.OnCalcToolTipSize(sender, e);
		}
		protected override Point CalcHintPosition() {
			NavBarItemLink link = LastHintInfo.HintObject as NavBarItemLink;
			if(link != null && !link.Group.GetShowAsIconsView()) {
				NavLinkInfoArgs args = GetLinkInfo(link);
				if(args != null) {
					return NavBar.PointToScreen(args.Bounds.Location);
				}
			}
			return base.CalcHintPosition();
		}
		protected override ObjectState CalcButtonState(NavBarState pressedState, NavBarHitTest btnHitTest) {
			NavBarHitInfo hitInfo = CalcHitInfo(MousePosition);
			NavGroupInfoArgs ga = GetGroupInfo(NavBar.ActiveGroup);
			if(NavBar.ActiveGroup == null) return ObjectState.Disabled;
			if(btnHitTest == NavBarHitTest.UpButton) {
				if(NavBar.ActiveGroup.TopVisibleLinkIndex < 1) return ObjectState.Disabled;
				return base.CalcButtonState(pressedState, btnHitTest);
			}
			if(ga == null) return ObjectState.Disabled;
			if(ga.LastLinkBounds.Bottom <= ga.ClientInfo.ClientBounds.Bottom) return ObjectState.Disabled;
			return base.CalcButtonState(pressedState, btnHitTest);
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
	}
	public class VSToolBoxNavLinkPainter : BaseNavLinkPainter {
		VSToolBoxLinkButtonObjectPainter buttonPainter;
		public VSToolBoxNavLinkPainter(NavBarControl navBar) : base(navBar)  {
			buttonPainter = new VSToolBoxLinkButtonObjectPainter();
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
		}
		protected override Brush GetLinkCaptionDisabledBrush(ObjectInfoArgs e, Brush foreBrush) {
			if(foreBrush is SolidBrush) {
				Color color = ((SolidBrush)foreBrush).Color;
				return e.Cache.GetSolidBrush(ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(color))));
			} 
			return null;
		}
		protected override void DrawSmall(CustomDrawNavBarElementEventArgs custom) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			if(li.Link.Item.IsSeparator()) {
				DrawLinkSeparator(custom);
				return;
			}
			DrawLinkBorder(custom.ObjectInfo);
			DrawLinkImage(custom, true);
			DrawLinkCaption(custom, true);
		}
		protected override void DrawListView(CustomDrawNavBarElementEventArgs custom) {
			DrawLinkBorder(custom.ObjectInfo);
			DrawLinkImage(custom, true);
		}
		protected virtual void DrawLinkBorder(ObjectInfoArgs e) {
			NavBarControl navBar = GetNavBar(e);
			AppearanceObject appearance = GetLinkAppearance(e);
			StyleObjectInfoArgs ba = new StyleObjectInfoArgs(e.Cache, e.Bounds, appearance, ObjectState.Normal);
			switch(e.State) {
				case ObjectState.Hot : navBar.GroupPainter.ButtonPainter.DrawObject(ba); break;
				case ObjectState.Selected : 
				case ObjectState.Pressed : 
					ba.State = ObjectState.Pressed;
					buttonPainter.DrawObject(ba); break;
			}
		}
	}
	public class VSToolBoxNavGroupPainter : FlatNavGroupPainter {
		public VSToolBoxNavGroupPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new VSToolBoxNavGroupClientPainter(this); 
		}
		protected override Rectangle CalcClientBounds(ObjectInfoArgs e, NavGroupInfoArgs groupInfo) {
			Rectangle bounds = base.CalcClientBounds(e, groupInfo);
			bounds.Width = CalcTextSize(e, groupInfo.CaptionBounds.Width).Width;
			return bounds;
		}
	}
	public class VSToolBoxNavGroupClientPainter : BaseNavGroupClientPainter {
		public VSToolBoxNavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override int CalcClientIndent(NavGroupClientInfoArgs e, IndentType indent) {
			if(e.Group.GroupStyle != NavBarGroupStyle.ControlContainer) {
				if(indent == IndentType.Top) return 2;
			}
			return base.CalcClientIndent(e, indent);
		}
	}
	public class VSToolBoxLinkButtonObjectPainter : FlatButtonObjectPainter {
		protected override void DrawPressed(ObjectInfoArgs e, AppearanceObject appearance) {
			Rectangle r = e.Bounds;
			BBrushes brushes = new BBrushes(e.Cache, appearance);
			DrawLines(e, r, brushes.Dark);
			r.X ++;	r.Y ++;	r.Width --; r.Height --;
			e.Graphics.FillRectangle(brushes.Light, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
			e.Graphics.FillRectangle(brushes.Light, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width --; r.Height --;
			appearance.DrawBackground(e.Cache, r, true);
		}
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject appearance) {
			BBrushes brushes = new BBrushes(e.Cache, appearance);
			Rectangle r = e.Bounds;
			DrawFlatBounds(e, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			e.Graphics.FillRectangle(brushes.Light, new Rectangle(r.X, r.Y, 1, r.Height));
			e.Graphics.FillRectangle(brushes.Light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			r.X ++;
			r.Y ++;
			r.Width --;
			r.Height --;
			appearance.DrawBackground(e.Cache, r, true);
		}
	}
}
