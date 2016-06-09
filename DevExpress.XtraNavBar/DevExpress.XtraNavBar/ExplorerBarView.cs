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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class ExplorerBarNavBarViewInfo : NavBarViewInfo {
		int topY;
		public ExplorerBarNavBarViewInfo(NavBarControl navBar) : base(navBar) {
			this.topY = 0;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraNavBar.Accessibility.ExplorerNavBarAccessible(NavBar, this); 
		}
		protected override void DoGroupClick(NavBarHitInfo hitInfo) {
			NavBarGroup group = hitInfo.Group;
			if(hitInfo.InGroupButton) {
				NavBarGroupCancelEventArgs e = new NavBarGroupCancelEventArgs(group);
				group.RaiseGroupExpanding(e);
				if(e.Cancel) return;
				group.Expanded = !group.Expanded;
			}
			MakeGroupVisible(group);
		}
		public override void MakeGroupVisible(NavBarGroup group) {
			NavGroupInfoArgs info = GetGroupInfo(group);
			if(info == null || info.ClientInfo.Bounds.IsEmpty) return;
			Rectangle r = info.Bounds;
			if(!info.ImageBounds.IsEmpty) r.Y = info.ImageBounds.Y;
			if(!info.ClientInfo.Bounds.IsEmpty)
				r.Height = info.ClientInfo.Bounds.Bottom - r.Top;
			else
				r.Height = info.Bounds.Bottom - r.Top;
			int newTopDelta = 0;
			if(r.Bottom > Client.Bottom) {
				newTopDelta = (r.Bottom - Client.Bottom);
			}
			if((r.Y  - newTopDelta) < Client.Y) {
				newTopDelta -= (Client.Y - (r.Y - newTopDelta));
			}
			TopY += newTopDelta;
		}
		public override void MakeLinkVisible(NavBarItemLink link) {
			if(IsLinkVisible(link)) return;
			if(!link.Visible || link.Group == null || !link.Group.IsVisible || !link.Item.IsVisible) return;
			link.Group.Expanded = true;
			if(!link.Group.Expanded || IsLinkVisible(link)) return;
			NavGroupInfoArgs groupInfo = GetGroupInfo(link.Group);
			if(groupInfo == null) return;
			NavLinkInfoArgs linkInfo = GetLinkInfo(link);
			if(linkInfo == null) return;
			TopY += groupInfo.Bounds.Top;
			int linkVOffset = Math.Abs(groupInfo.Bounds.Top - linkInfo.Bounds.Top);
			if(linkVOffset > Client.Height) TopY += linkVOffset;
		}
		protected override bool IsScrollBarMode {
			get {
				return false;
			}
		}
		public override int ClientLinkOffsetY { get { return 0; } }
		public override bool IsExplorerBar { get { return true; } }
		public override int GetGroupTopVisibleLinkIndex(NavBarGroup group) { return 0; }
		protected override void OnScrollBarValueChanged(object sender, EventArgs e) {
			base.OnScrollBarValueChanged(sender, e);
		}
		public override int TopY {
			get { return topY; }
			set { 
				if(value > ScrollRange - ScrollBar.LargeChange + 2) value = ScrollRange - ScrollBar.LargeChange + 2;
				if(value < 0) value = 0;
				if(TopY == value) return;
				topY = value;
				this.fIsReady = false;
				SaveTextSizeCache = true;
				NavBar.Invalidate();
			}
		}
		protected override void OnBeforeDraw(GraphicsCache e) {
			ScrollBarVisible = !ScrollBarRectangle.IsEmpty;
			if(ScrollBar.Bounds != ScrollBarRectangle)
				ScrollBar.Bounds = ScrollBarRectangle;
			base.OnBeforeDraw(e);
		}
		protected override int CalcGroupVertIndent(NavGroupInfoArgs groupInfo) {
			int res = (NavBar.ExplorerBarGroupInterval == -1 ? 4 : 0);
			Size minSize = NavBar.GroupPainter.CalcObjectMinBounds(groupInfo).Size;
			Size imgSize = groupInfo.Group.GetPrefferedImageSize();
			if(imgSize.Height > minSize.Height) {
				res += (imgSize.Height - minSize.Height) + 1; 
			}
			res = Math.Max(res, NavBar.ExplorerBarGroupInterval);
			return res;
		}
		protected virtual int CalcGroupHorzIndent() { 
			if(NavBar.ExplorerBarGroupOuterIndent > -1) return NavBar.ExplorerBarGroupOuterIndent;
			return 10;
		}
		protected override void CalcButtonPositions(NavGroupInfoArgs activeGroupArgs, bool updateScrollbar) {
		}
		public override bool AllowSelectedLink { get { return true; } }
		public override bool AllowScrollBar { get { return true; } }
		public override bool AllowHideGroupCaptions { get { return false; } }
		public override bool AllowListViewMode { get { return false; } }
		public override bool AllowExpandCollapse { get { return true; } }
		protected virtual void UpdateScrollBar(int maxBottom, ref int savedY) {
			if(maxBottom > Client.Bottom) {
				Rectangle prevClient = scrollBarRectangle = Client;
				scrollBarRectangle.Width = SystemInformation.VerticalScrollBarWidth;
				scrollBarRectangle.X = IsRightToLeft ? Client.X : Client.Right - scrollBarRectangle.Width;
				if(!ScrollBar.IsOverlapScrollBar) {
					int clientX = IsRightToLeft ? scrollBarRectangle.Width + Client.X : Client.X;
					int clientWidth = IsRightToLeft ? Client.Right - scrollBarRectangle.Right : scrollBarRectangle.X - clientX;
					Client = new Rectangle(clientX, Client.Y, clientWidth, Client.Height);
					if(Client != prevClient) LinkSizesCache.Clear();
				}
			} else {
				savedY = 0;
				ScrollRange = 0;
			}
		}
		protected virtual void UpdateScrollRange(int newRange) {
			ScrollRange = newRange;
		}
		protected bool SaveTextSizeCache { get; set; }
		protected override void CalcViewInfo(Rectangle bounds) {
			UpdatePainters();
			int savedY = TopY;
			topY = 0;
			scrollBarRectangle = Rectangle.Empty;
			ScrollRange = 0;
			if(!SaveTextSizeCache)
				ResetLinkSizesCache();
			RemoveUnusedLinksFromCache();
			SaveTextSizeCache = false;
			Client  = CalcClientRectangle(bounds);
			int maxBottom = CalcGroupsViewInfo(Client);
			UpdateScrollBar(maxBottom, ref savedY);
			Groups.Clear();
			UpdateScrollRange(Math.Max(0, CalcGroupsViewInfo(Client)));
			if(savedY > ScrollRange) savedY = ScrollRange;
			topY = savedY;
			BeginScrollUpdate();
			try {
				ScrollBar.Maximum = ScrollRange + 1;
				ScrollBar.LargeChange = Math.Max(Client.Height - 1, 1);
				if(topY + ScrollBar.LargeChange > ScrollBar.Maximum) topY = Math.Max(0, 1 + (ScrollBar.Maximum - ScrollBar.LargeChange));
				ScrollBar.Value = TopY;
			}
			finally {
				EndScrollUpdate();
			}
			Groups.Clear();
			Rectangle r = Client;
			r.Offset(0, -topY);
			CalcGroupsViewInfo(r);
		}
		protected override int CalcGroupsViewInfo(Rectangle bounds) {
			Rectangle rect = bounds;
			BaseNavGroupPainter painter = NavBar.GroupPainter;
			rect.Inflate( -CalcGroupHorzIndent(), 0);
			rect.Height = 0;
			for(int n = 0; n < NavBar.Groups.Count; n++) {
				NavBarGroup group = NavBar.Groups[n];
				if(!group.IsVisible) {
					continue;
				}
				NavGroupInfoArgs groupArgs = new NavGroupInfoArgs(group, rect);
				groupArgs.Graphics = GInfo.Graphics;
				rect.Y += CalcGroupVertIndent(groupArgs);
				groupArgs.Bounds = rect;
				groupArgs.Bounds = new Rectangle(rect.X, rect.Y, rect.Width, painter.CalcObjectMinBounds(groupArgs).Height);
				painter.CalcObjectBounds(groupArgs);
				Groups.Add(groupArgs);
				rect.Y = groupArgs.Bounds.Bottom;
				Size buttonSize = Size.Empty;
				int footerHeight = painter.CalcFooterHeight(groupArgs, out buttonSize);
				if(group.Expanded) {
					if(NavBar.AllowStretchGroup(group)) {
						group.GroupClientHeight = Math.Max(1, bounds.Height - groupArgs.Bounds.Bottom - footerHeight);
					}
					rect.Y = CalcExpandedGroupInfo(groupArgs);
				}
				else {
					groupArgs.ClientInfo.Bounds = Rectangle.Empty;
				}
				if(footerHeight != 0) {
					Rectangle footerBounds = new Rectangle(rect.X, rect.Y, rect.Width, footerHeight);
					painter.CalcFooterBounds(groupArgs, footerBounds, buttonSize);
				}
				rect.Y += groupArgs.FooterBounds.Height;
				groupArgs.Graphics = null;
			}
			return rect.Y;
		}
		protected int CalcExpandedGroupInfo(NavGroupInfoArgs groupInfo) {
			NavBarGroup group = groupInfo.Group;
			Rectangle clientBounds = groupInfo.Bounds;
			clientBounds.Y = groupInfo.Bounds.Bottom;
			groupInfo.Graphics = GInfo.Graphics;
			if(group.GroupStyle != NavBarGroupStyle.ControlContainer && group.VisibleItemLinks.Count > 0) {
				groupInfo.ClientInfo.Bounds = clientBounds;
				int maxClientBottom = NavBar.GroupPainter.ClientPainter.CalcObjectBounds(groupInfo.ClientInfo).Bottom;
				clientBounds.Height = maxClientBottom - clientBounds.Y;
			} else clientBounds.Height = 0;
			int groupClientHeight = group.GroupClientHeight;
			if(groupClientHeight == -1 && group.ControlContainer != null)
				groupClientHeight = NavBar.GroupPainter.ClientPainter.CalcGroupClientHeightByClientSize(group.ControlContainer.ClientSize);
			if(groupClientHeight > -1) clientBounds.Height = Math.Max(groupClientHeight, clientBounds.Height);
			if(NavBar.AllowStretchGroup(group) && clientBounds.Height < groupClientHeight) clientBounds.Height = groupClientHeight;
			NavBarCalcGroupClientHeightEventArgs e = new NavBarCalcGroupClientHeightEventArgs(group, clientBounds.Height);
			group.RaiseCalcGroupClientHeight(e);
			clientBounds.Height = e.Height;
			groupInfo.ClientInfo.Bounds = clientBounds;
			NavBar.GroupPainter.ClientPainter.CalcObjectBounds(groupInfo.ClientInfo);
			groupInfo.Graphics = null;
			return clientBounds.Bottom;
		}
	}
	public class ExplorerBarNavLinkPainter : BaseNavLinkPainter {
		public ExplorerBarNavLinkPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
		}
		protected override Brush GetLinkCaptionDisabledBrush(ObjectInfoArgs e, Brush foreBrush) {
			return null;
		}
	}
	public class ExplorerBarNavGroupClientPainter : BaseNavGroupClientPainter {
		public ExplorerBarNavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override int CalcClientIndent(NavGroupClientInfoArgs e, IndentType indent) {
			if(e.Group.GroupStyle == NavBarGroupStyle.ControlContainer) return 0;
			return 5; 
		}
		protected internal override int CalcGroupClientHeightByClientSize(Size clientSize) {
			return clientSize.Height + 1;
		}
		protected override void CalcClientBounds(NavGroupClientInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, 0);
			r.Height --;
			e.ClientBounds = r;
		}
		protected override void DrawBorder(NavGroupClientInfoArgs e) {
			DrawBorder(e, GroupPainter.GetGroupAppearance(e).GetBorderBrush(e.Cache));
		}
		protected void DrawBorder(NavGroupClientInfoArgs e, Brush brush) {
			Rectangle r = e.Bounds;
			e.Graphics.FillRectangle(brush, new Rectangle(r.X, r.Y, 1, r.Height));
			e.Graphics.FillRectangle(brush, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			e.Graphics.FillRectangle(brush, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
		}
	}
	public class ExplorerBarNavGroupPainter : FlatNavGroupPainter {
		ObjectPainter openCloseButtonPainter;
		public ExplorerBarNavGroupPainter(NavBarControl navBar) : base(navBar) {
			this.openCloseButtonPainter = CreateOpenCloseButtonPainter();
		}
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return new ExplorerBarOpenCloseButtonObjectPainter(); }
		protected ObjectPainter OpenCloseButtonPainter { get { return openCloseButtonPainter; } }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new ExplorerBarNavGroupClientPainter(this); 
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		protected virtual bool AllowShowGroupButtonInCaption { get { return NavBar.ExplorerBarShowGroupButtons; } }
		protected virtual bool AllowShowGroupImageInCaption { get { return true; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			Rectangle savedBounds = e.Bounds, saved2 = groupInfo.CaptionClientBounds;
			Rectangle r = e.Bounds;
			try {
				e.Bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 40);
				CalcObjectBounds(e);
				int h = CalcTextSize(e, groupInfo.CaptionBounds.Width).Height + 8;
				r.Height = 0;
				if(AllowShowGroupButtonInCaption) {
					r.Height = CalcExpandCollapseButtonSize(e, h).Height;
				}
				r.Height = Math.Max(h, r.Height);
				r.Height = Math.Max(20, r.Height);
				r.Height += (CalcBoundsByClientRectangle(e, r).Height - r.Height);
			}
			finally {
				e.Bounds = savedBounds;
				groupInfo.CaptionClientBounds = saved2;
			}
			return r;
		}
		protected virtual Size CalcExpandCollapseButtonSize(ObjectInfoArgs e, int textHeight) {
			return OpenCloseButtonPainter.CalcObjectMinBounds(new ExplorerBarOpenCloseButtonInfoArgs(e.Cache, new Rectangle(0, 0, 10, textHeight), GetGroupAppearance(e), ObjectState.Normal, true)).Size;
		}
		protected override Rectangle CalcGroupCaptionImageBounds(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			if(!AllowShowGroupImageInCaption) return Rectangle.Empty;
			return base.CalcGroupCaptionImageBounds(e, custom);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			groupInfo.CaptionClientBounds = GetObjectClientRectangle(groupInfo);
			groupInfo.ButtonBounds = CalcGroupCaptionButtonBounds(groupInfo);
			groupInfo.ImageBounds = CalcGroupCaptionImageBounds(groupInfo, null); 
			Rectangle rect = new Rectangle(groupInfo.ImageBounds.Location, groupInfo.Group.GetPrefferedImageSize(groupInfo.ImageBounds.Size));
			groupInfo.CaptionBounds = CalcGroupCaptionBounds(groupInfo, rect);
			CheckGroupBounds(groupInfo, e.Bounds);
			return e.Bounds;
		}
		protected void CheckGroupBounds(NavGroupInfoArgs groupInfo, Rectangle contentBounds) {
			groupInfo.CaptionClientBounds = CheckBounds(groupInfo.CaptionClientBounds);
			groupInfo.ButtonBounds = CheckBounds(groupInfo.ButtonBounds);
			groupInfo.ImageBounds = CheckBounds(groupInfo.ImageBounds);
			groupInfo.CaptionBounds = CheckBounds(groupInfo.CaptionBounds);
		}
		protected virtual int GetImageToTextIndent() { return 4; }
		protected virtual int GetLeftTextIndent() { return 8; }
		protected virtual Rectangle CalcGroupCaptionBounds(NavGroupInfoArgs e, Rectangle imageBounds) {
			Rectangle buttonBounds = CalcGroupCaptionButtonBounds(e);
			Rectangle caption = e.CaptionClientBounds;
			caption.Inflate(0, -3);
			int right = caption.Right;
			if(imageBounds.IsEmpty) 
				caption.X += GetLeftTextIndent();
			else 
				caption.X = imageBounds.Right + GetImageToTextIndent();
			int width = (buttonBounds.IsEmpty ? right : (buttonBounds.X - 3)) - caption.X;
			e.CaptionMaxWidth = width;
			caption.Width = CalcTextSize(e, width).Width;
			caption = NavBarViewInfo.CheckElementCaptionLocation(e.PaintAppearance, caption, width);
			return caption;
		}
		protected virtual Rectangle CalcGroupCaptionButtonBounds(ObjectInfoArgs e) {
			if(!NavBar.ExplorerBarShowGroupButtons) return Rectangle.Empty;
			NavGroupInfoArgs ee = e as NavGroupInfoArgs;
			Size btnSize = OpenCloseButtonPainter.CalcObjectMinBounds(new ExplorerBarOpenCloseButtonInfoArgs(e.Cache, Rectangle.Empty, GetGroupAppearance(e), ObjectState.Normal, true)).Size;
			Rectangle res = ee.CaptionClientBounds;
			res.Size = btnSize;
			res.X = (ee.CaptionClientBounds.Right - res.Width) - 4;
			res.Y += (ee.CaptionClientBounds.Height - res.Height) / 2;
			return res;
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			NavBarGroup group = GetGroup(e);
			custom.Appearance.DrawBackground(e.Cache, e.Bounds, true);
			DrawGroupImage(e, custom);
			DrawGroupButton(e);
			if (e.Group.GetAllowHtmlString()) {
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else
				custom.Appearance.DrawString(e.Cache, custom.Caption, e.CaptionBounds);
		}
		protected virtual void DrawGroupButton(NavGroupInfoArgs e) {
			if(e.ButtonBounds.IsEmpty) return;
			NavBarGroup group = GetGroup(e);
			ExplorerBarOpenCloseButtonInfoArgs oa = new ExplorerBarOpenCloseButtonInfoArgs(e.Cache, e.ButtonBounds, GetGroupAppearance(e), e.State, group.Expanded);
			oa.BackAppearance = NavBar.PaintAppearance.Background;
			openCloseButtonPainter.DrawObject(oa);
		}
		protected override int CalcGroupX(Rectangle bounds) {
			int clientWidth = NavBar.Width;
			if(NavBar.ViewInfo != null && NavBar.ViewInfo.ScrollBar != null && !NavBar.ViewInfo.ScrollBar.IsOverlapScrollBar)
				clientWidth += NavBar.ViewInfo.ScrollBarRectangle.Width;
			return clientWidth - bounds.Right;
		}
	}
	public class UltraFlatExplorerBarNavBarViewInfo : ExplorerBarNavBarViewInfo {
		public UltraFlatExplorerBarNavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		public override bool AllowListViewMode { get { return true; } }
	}
	public class UltraFlatExplorerBarNavLinkPainter : ExplorerBarNavLinkPainter  {
		ButtonObjectPainter buttonPainter;
		public UltraFlatExplorerBarNavLinkPainter(NavBarControl navBar) : base(navBar) {
			buttonPainter = new UltraFlatButtonObjectPainter();
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			if(IsDrawLinkHotOrPressed(e)) {
				NavLinkInfoArgs li = e as NavLinkInfoArgs;
				StyleObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, li.ImageRectangle, GetLinkAppearance(e), e.State);
				buttonPainter.DrawObject(ia);
			}
		}
	}
}
