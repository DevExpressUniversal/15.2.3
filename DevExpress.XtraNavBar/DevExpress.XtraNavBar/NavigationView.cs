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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Drawing;
using System.Reflection;
using DevExpress.XtraNavBar.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public enum NavigationPaneImage { Chevron, Splitter, SmallGroup, LargeGroup, MoreButtons, FewerButtons }
	public class NavigationPaneViewInfo : NavBarViewInfo {
		[ThreadStatic]
		static Hashtable paneImages = null;
		public static Image GetPaneImage(NavigationPaneImage image) {
			if(paneImages == null) LoadPaneImages();
			return paneImages[image] as Image;
		}
		static void LoadPaneImages() {
			paneImages = new Hashtable();
			paneImages[NavigationPaneImage.Chevron] = Load("NavigationPaneChevron.bmp");
			paneImages[NavigationPaneImage.Splitter] = Load("NavigationPaneSplitter.bmp");
			paneImages[NavigationPaneImage.SmallGroup] = Load("NavigationPaneGroup.bmp");
			paneImages[NavigationPaneImage.LargeGroup] = Load("NavigationPaneLargeGroup.bmp");
			paneImages[NavigationPaneImage.MoreButtons] = Load("NavigationPaneMoreButtons.bmp");
			paneImages[NavigationPaneImage.FewerButtons] = Load("NavigationPaneFewerButtons.bmp");
		}
		public static Image Load(string name) {
			Image res = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraNavBar." + name, typeof(NavigationPaneViewInfo).Assembly);
			Bitmap bmp = res as Bitmap;
			if(bmp != null) bmp.MakeTransparent(Color.Magenta);
			return res;
		}
		ArrayList overflowPanelGroups;
		NavigationPaneOverflowPanelInfo overflowInfo;
		NavigationPaneOverflowPanelPainter overflowPanelPainter;
		NavigationPaneSplitterPainter splitterPainter;
		NavigationPaneHeaderPainter headerPainter;
		NavigationPaneHeaderObjectInfo headerInfo;
		ObjectInfoArgs splitterInfo;
		Rectangle contentButton;
		public NavigationPaneViewInfo(NavBarControl navBar) : base(navBar) {
			this.overflowPanelGroups = new ArrayList();
			this.headerPainter = CreateNavPaneHeaderPainter();
			this.splitterPainter = CreateNavPaneSplitterPainter();
			this.overflowPanelPainter = CreateNavPaneOverflowPanelPainter();
			this.overflowInfo = null;
			this.headerInfo = null;
			this.splitterInfo = null;
		}
		protected internal override void OnSelectedLinkChanged(NavBarGroup group) {
			base.OnSelectedLinkChanged(group);
			if(NavBar.NavPaneForm != null) {
				NavBar.NavPaneForm.Refresh();
			}
		}
		protected override bool IsLinkVisible(NavBarItemLink link) {
			NavLinkInfoArgs linkInfo = GetLinkInfo(link);
			NavGroupInfoArgs groupInfo = GetGroupInfo(link.Group);
			if(linkInfo == null || groupInfo == null || !groupInfo.ClientInfoBounds.Contains(linkInfo.Bounds)) return false;
			return true;
		}
		protected override bool IsScrollBarMode {
			get {
				return base.IsScrollBarMode;
			}
		}
		protected override void CalcScrollBarPosition(NavGroupInfoArgs activeGroupArgs, bool updateScrollbar) {
			if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed) {
			   this.scrollBarRectangle = Rectangle.Empty;
			   this.TopY = 0;
			   return;
			}
			base.CalcScrollBarPosition(activeGroupArgs, updateScrollbar);
			if(updateScrollbar && ScrollBarRectangle.IsEmpty && TopY != 0) {
				TopY = 0;
			}
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			if(IsScrollBarMode) {
				base.OnMouseWheel(e);
				return;
			}
			bool isUp = e.Delta > 0;
			DoMoveTopLinkIndex(NavBar.ActiveGroup, isUp ? -1 : 1);
		}
		protected override bool ShouldMoveTopLinkIndex(NavBarGroup group, int delta) {
			if(!base.ShouldMoveTopLinkIndex(group, delta)) return false;
			NavGroupInfoArgs gInfo = GetGroupInfo(group);
			if(gInfo != null && gInfo.Links != null) {
				if(gInfo.Links.Count == 0) return false;
				NavLinkInfoArgs lInfo = gInfo.Links[gInfo.Links.Count - 1] as NavLinkInfoArgs;
				if(delta > 0 && lInfo.Bounds.Bottom <= gInfo.ClientInfoBounds.Bottom) return false;
			}
			return true;
		}
		public virtual int GetCollapsedWidth() {
			if(!IsReady) {
				CalcViewInfo(NavBar.ClientRectangle);
			}
			int maxWidth = 0;
			BaseNavigationPaneGroupPainter painter = NavBar.GroupPainter as BaseNavigationPaneGroupPainter;
			if(painter == null) return 20;
			maxWidth = GetCollapsedGroupsMaxWidth(maxWidth, painter);
			if(maxWidth == 0) return 32;
			return maxWidth;
		}
		protected int GetCollapsedGroupsMaxWidth(int maxWidth, BaseNavigationPaneGroupPainter painter) {
			bool isGraphicsAdded = false;
			if(NavBar.ViewInfo.GInfo.Graphics == null) {
				NavBar.ViewInfo.GInfo.AddGraphics(null);
				isGraphicsAdded = true;
			}
			try {
				foreach(NavGroupInfoArgs group in Groups) {
					group.Graphics = NavBar.ViewInfo.GInfo.Graphics;
					maxWidth = Math.Max(maxWidth, painter.CalcCollapsedGroupWidth(group));
					group.Graphics = null;
				}
			}
			finally {
				if(isGraphicsAdded) NavBar.ViewInfo.GInfo.ReleaseGraphics();
			}
			return maxWidth;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraNavBar.Accessibility.NavigationPaneAccessible(NavBar, this); 
		}
		public Rectangle ContentButton { get { return contentButton; } set { contentButton = value; } }
		protected virtual NavigationPaneHeaderPainter CreateNavPaneHeaderPainter() {
			return new NavigationPaneHeaderPainter(this);
		}
		protected virtual NavigationPaneSplitterPainter CreateNavPaneSplitterPainter() {
			return new NavigationPaneSplitterPainter(this);
		}
		protected virtual NavigationPaneOverflowPanelPainter CreateNavPaneOverflowPanelPainter() {
			return new NavigationPaneOverflowPanelPainter(NavBar);
		}
		protected virtual NavigationPaneHeaderObjectInfo CreateHeaderInfo(NavBarGroup group) {
			Image img = group.GetSmallImage() ?? group.GetLargeImage();
			string groupCaption = group.GetAllowHtmlString() ? StringPainter.Default.RemoveFormat(group.Caption, true) : group.Caption;
			return new NavigationPaneHeaderObjectInfo(group == null ? null : groupCaption, NavBar.PaintAppearance.NavigationPaneHeader, img);
		}
		public override NavBarGroupStyle GetDefaultGroupStyle() { return NavBarGroupStyle.SmallIconsText; }
		public ArrayList OverflowPanelGroups { get { return overflowPanelGroups; } }
		public NavigationPaneOverflowPanelInfo OverflowInfo { get { return overflowInfo; } }
		public ObjectInfoArgs SplitterInfo { get { return splitterInfo; } }
		public NavigationPaneHeaderPainter HeaderPainter { get { return headerPainter; } }
		public NavigationPaneOverflowPanelPainter OverflowPanelPainter { get { return overflowPanelPainter; } }
		public NavigationPaneSplitterPainter SplitterPainter { get { return splitterPainter; } }
		public NavigationPaneHeaderObjectInfo HeaderInfo { get { return headerInfo; } }
		public Rectangle SplitterBounds { get { return SplitterInfo == null ? Rectangle.Empty : SplitterInfo.Bounds; } }
		public Rectangle HeaderBounds { get { return HeaderInfo == null ? Rectangle.Empty : HeaderInfo.Bounds; } }
		public Rectangle OverflowBounds { get { return OverflowInfo == null ? Rectangle.Empty : OverflowInfo.Bounds; } }
		public virtual int GetGroupClientMinHeight() {
			return NavBar.NavigationPaneGroupClientHeight;
		}
		public bool GetOverflowPanelUseSmallImages() {
			return NavBar.NavigationPaneOverflowPanelUseSmallImages;
		}
		protected int GetVisibleGroupHeadersCount() {
			int count = 0;
			foreach(NavGroupInfoArgs groupInfo in Groups) {
				if(!groupInfo.Bounds.IsEmpty) count++;
			}
			return count;
		}
		public virtual bool GetCanShowFewerGroups() {
			return GetVisibleGroupHeadersCount() > 0;
		}
		public virtual bool GetCanShowMoreGroups() {
			return GetVisibleGroupHeadersCount() < GetNavigationVisibleGroupCount(); 
		}
		protected int GetNavigationVisibleGroupCount() {
			int res = 0;
			foreach(NavBarGroup group in NavBar.Groups) {
				if(group.IsVisible && group.NavigationPaneVisible) res ++;
			}
			return res;
		}
		public virtual Image GetOverflowPanelDefaultImage() {
			return GetPaneImage(GetOverflowPanelUseSmallImages() ? NavigationPaneImage.SmallGroup : NavigationPaneImage.LargeGroup);
		}
		protected virtual Size GetOverflowPanelImageSize() {
			Size size = GetOverflowPanelDefaultImage().Size;
			bool small = GetOverflowPanelUseSmallImages();
			foreach(NavBarGroup group in NavBar.Groups) {
				Size groupSize = small ? group.GetSmallImageSize() : group.GetLargeImageSize();
				size.Width = Math.Max(size.Width, groupSize.Width);
				size.Height = Math.Max(size.Height, groupSize.Height);
			}
			return size;
		}
		protected override BorderPainter CreateDefaultBorderPainter() {
			return new NavigationPaneBorderPainter();
		}
		protected internal override ObjectState CalcGroupState(NavGroupInfoArgs groupInfo) {
			ObjectState state = groupInfo.Group.State;
			if(GetExpandedGroup() == groupInfo.Group) {
				state |= ObjectState.Selected;
			}
			return state;
		}
		public override void Clear() {
			this.splitterInfo = null;
			this.overflowInfo = null;
			if(HeaderInfo != null) HeaderInfo.Bounds = Rectangle.Empty;
			if(OverflowPanelGroups != null) OverflowPanelGroups.Clear();
			base.Clear();
		}
		protected override int CalcGroupsViewInfo(Rectangle bounds) {
			Groups.Clear();
			NavBarGroup expandedGroup = GetExpandedGroup();
			int bottom = bounds.Top;
			int minOverflowHeight = CalcOverflowAndSplitterHeight();
			if(bounds.Height + 20 > minOverflowHeight) {
				bounds.Height -= minOverflowHeight;
			} else {
				minOverflowHeight = 0;
			}
			NavGroupInfoArgs expandedInfo = null;
			if(expandedGroup != null) {
				bottom = CalcExpandedGroupInfo(bounds, expandedGroup);
				expandedInfo = GetGroupInfo(expandedGroup);
				CalcButtonPositions(expandedInfo, false);
			}
			bounds.Height += minOverflowHeight;
			if(bottom == 0 || bottom > bounds.Bottom) {
				CalcButtonPositions(expandedInfo, true);
				return 0;
			}
			Rectangle groupBounds = new Rectangle(bounds.X, bottom, bounds.Width, bounds.Bottom - bottom);
			bottom = CalcSplitterInfo(new Rectangle(groupBounds.X, groupBounds.Y, groupBounds.Width, groupBounds.Height));
			if(bottom == 0) return 0; 
			groupBounds = new Rectangle(bounds.X, bottom, bounds.Width, bounds.Bottom - bottom);
			CalcNavGroupsViewInfo(bounds, groupBounds, expandedGroup);
			CreateOverflowGroupsList();
			if(expandedGroup != null) {
				int deltaIndex = CalcDeltaLinksTopVisibleIndex(expandedInfo);
				expandedGroup.TopVisibleLinkIndex -= deltaIndex;
			}
			CalcButtonPositions(expandedInfo, true);
			UpdateGroupPaintIndexes();
			return 0;
		}
		protected virtual int CalcDeltaLinksTopVisibleIndex(NavGroupInfoArgs groupInfo) {
			if(groupInfo == null || groupInfo.Links.Count == 0)
				return 0;
			int topVisibleIndex = groupInfo.ClientInfo.InForm ? 0 : Math.Min(groupInfo.Group.TopVisibleLinkIndex, groupInfo.Group.VisibleItemLinks.Count - 1);
			int delta = groupInfo.ClientInfo.ClientBounds.Bottom - groupInfo.LastLinkBounds.Bottom;
			int deltaIndex = 0, currIndex = groupInfo.ClientInfo.Links.Count - 1;
			if(delta > 0 && topVisibleIndex > 0) {
				while(currIndex >= 0 && delta > ((NavLinkInfoArgs)groupInfo.ClientInfo.Links[currIndex]).Bounds.Height) {
					delta -= ((NavLinkInfoArgs)groupInfo.ClientInfo.Links[currIndex]).Bounds.Height;
					currIndex--;
					deltaIndex++;
				}
			}
			return deltaIndex;
		}
		protected virtual void CreateOverflowGroupsList() {
			foreach(NavBarGroup group in NavBar.Groups) {
				if(!group.NavigationPaneVisible || !group.IsVisible) continue;
				NavGroupInfoArgs groupInfo = GetGroupInfo(group);
				if(groupInfo == null || groupInfo.Bounds.IsEmpty) 
					OverflowPanelGroups.Add(group);
			}
			if(OverflowInfo == null) return;
			OverflowInfo.OverflowGroups.Clear();
			OverflowInfo.OverflowGroups.AddRange(OverflowPanelGroups);
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, OverflowPanelPainter, OverflowInfo);
		}
		protected virtual void CalcNavGroupsViewInfo(Rectangle bounds, Rectangle groupBounds, NavBarGroup expandedGroup) {
			int maxGroupsHeight = groupBounds.Bottom - groupBounds.Top;
			NavGroupInfoArgs expandedInfo = GetGroupInfo(expandedGroup);
			ArrayList groupInfoList = CalcGroupsInfo(bounds, expandedGroup);
			int groupsHeight = 0;
			for(;;) {
				groupsHeight = CalcGroupsHeight(groupInfoList);
				if(groupsHeight <= maxGroupsHeight || groupInfoList.Count < 2) break;
				NavGroupInfoArgs groupInfo = groupInfoList[groupInfoList.Count - 2] as NavGroupInfoArgs;
				if(groupInfo == expandedInfo) groupInfo.Bounds = Rectangle.Empty;
				groupInfoList.RemoveAt(groupInfoList.Count - 2);
			}
			if(groupsHeight < maxGroupsHeight) {
				int delta = maxGroupsHeight - groupsHeight;
				if(expandedInfo != null) {
					Rectangle clientBounds = expandedInfo.ClientInfo.Bounds;
					clientBounds.Height += delta;
					expandedInfo.ClientInfo.Bounds = clientBounds;
					ObjectPainter.CalcObjectBounds(GInfo.Graphics, NavBar.GroupPainter.ClientPainter, expandedInfo.ClientInfo);
				}
				if(SplitterInfo != null)
					SplitterInfo.OffsetContent(0, delta + (groupsHeight > 0 && ShouldUseGroupsOffset ? 1 : 0));
				groupBounds.Y += delta; groupBounds.Height -= delta;
			} 
			int topY = groupBounds.Y;
			foreach(ObjectInfoArgs info in groupInfoList) {
				info.Bounds = new Rectangle(info.Bounds.X, topY, info.Bounds.Width, info.Bounds.Height);
				NavGroupInfoArgs groupInfo = info as NavGroupInfoArgs;
				ObjectPainter painter = NavBar.GroupPainter;
				if(groupInfo != null) {
					if(groupInfo.Group != expandedGroup) Groups.Add(info);
				} else {
					painter = OverflowPanelPainter;
					this.overflowInfo = info as NavigationPaneOverflowPanelInfo;
				}
				ObjectPainter.CalcObjectBounds(GInfo.Graphics, painter, info);
				topY += info.Bounds.Height;
				ProcessGroupBounds(info, groupInfoList, ref topY);
			}
			Groups.Sort(new GroupInfoComparer());
		}
		protected void ProcessGroupBounds(ObjectInfoArgs info, ArrayList groupInfoList, ref int topY) {
			if(!ShouldUseGroupsOffset) 
				return;
			topY--;
			if(info is NavigationPaneOverflowPanelInfo && !UseOnlyOverflowPanel(groupInfoList)) {
				Rectangle bounds = info.Bounds;
				bounds.Y++;
				info.Bounds = bounds;
			}
		}
		protected bool ShouldUseGroupsOffset {
			get {
				return NavPaneSkins.GetSkin(NavBar).Properties.GetBoolean(NavPaneSkins.OptGroupRequireOffset);
			}
		}
		class GroupInfoComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				NavGroupInfoArgs g1 = (NavGroupInfoArgs)a, g2 = (NavGroupInfoArgs)b;
				if(g1 == g2) return 0;
				return Comparer.Default.Compare(g1.Bounds.Top, g2.Bounds.Top);
			}
		}
		protected int CalcGroupsHeight(ArrayList groupInfoList) {
			int height = 0;
			foreach(ObjectInfoArgs info in groupInfoList) {
				height += info.Bounds.Height;
			}
			return height - CalcGroupsHeightCorrection(groupInfoList);
		}
		protected int CalcGroupsHeightCorrection(ArrayList groupInfoList) {
			int groupCount = CalcActiveGroupsCount(groupInfoList);
			if(!ShouldUseGroupsOffset || groupCount < 1)
				return 0;
			int res = groupCount - 1;
			if(NavBar.OptionsNavPane.ShowOverflowPanel && !UseOnlyOverflowPanel(groupInfoList)) res--;
			return res;
		}
		protected int CalcActiveGroupsCount(ArrayList groupInfoList) {
			int res = 0;
			foreach(ObjectInfoArgs info in groupInfoList) {
				if(info.Bounds.Height > 0) res++;
			}
			return res;
		}
		protected bool UseOnlyOverflowPanel(ArrayList groupInfoList) {
			if(!NavBar.OptionsNavPane.ShowOverflowPanel || NavBar.OptionsNavPane.ShowSplitter || groupInfoList.Count != 1)
				return false;
			return groupInfoList[0] is NavigationPaneOverflowPanelInfo;
		}
		protected ArrayList CalcGroupsInfo(Rectangle bounds, NavBarGroup expandedGroup) {
			ArrayList groups = new ArrayList();
			int top = bounds.Top;
			int groupCount = 0;
			foreach(NavBarGroup group in NavBar.Groups) {
				if(NavBar.NavigationPaneMaxVisibleGroups == 0 || IsAttachedToOfficeNavigationBar) break;
				if(!group.IsVisible || !group.NavigationPaneVisible) continue;
				NavGroupInfoArgs groupInfo;
				if(group == expandedGroup) {
					groupInfo = GetGroupInfo(expandedGroup);
					groupInfo.Bounds = bounds;
				}
				else 
					groupInfo = new NavGroupInfoArgs(group, bounds);
				int height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, NavBar.GroupPainter, groupInfo).Height; 				
				groupInfo.Bounds = new Rectangle(bounds.X, top, bounds.Width, height);
				groupInfo.CaptionClientBounds = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, NavBar.GroupPainter, groupInfo);
				groups.Add(groupInfo);
				top += height;
				groupCount ++;
				if(top > bounds.Bottom || groupCount == NavBar.NavigationPaneMaxVisibleGroups) break;
			}
			groups.Add(CalcOverflowPanelInfo(new Rectangle(bounds.X, top, bounds.Width, bounds.Height)));
			return groups;
		}
		protected virtual NavigationPaneOverflowPanelInfo CalcOverflowPanelInfo(Rectangle bounds) {
			NavigationPaneOverflowPanelInfo panelInfo = new NavigationPaneOverflowPanelInfo(NavBar);
			if(IsAttachedToOfficeNavigationBar) return panelInfo;
			panelInfo.ImageSize = GetOverflowPanelImageSize();
			panelInfo.Bounds = bounds;
			bounds.Height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, OverflowPanelPainter, panelInfo).Height;
			panelInfo.Bounds = bounds;
			return panelInfo;
		}
		protected virtual int CalcOverflowAndSplitterHeight() {
			int height = 0;
			if(IsAttachedToOfficeNavigationBar) return 0;
			if(NavBar.OptionsNavPane.ShowSplitter)
				height += ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SplitterPainter, new ObjectInfoArgs()).Height;
			if(NavBar.OptionsNavPane.ShowOverflowPanel)
				height += CalcOverflowPanelInfo(new Rectangle(0, 0, 100, 100)).Bounds.Height;
			return height;
		}
		protected internal virtual Size CalcExpandedGroupBestSize(NavGroupInfoArgs groupInfo) {
			NavBarGroup expandedGroup = groupInfo.Group;
			if(expandedGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
				if(expandedGroup.ControlContainer != null) 
					return new Size(expandedGroup.ControlContainer.Width, ContentButton.Height);
				return Size.Empty;
			}
			Size sz = Size.Empty;
			int width;
			groupInfo.ClientInfo.Bounds = new Rectangle(0, 0, NavBar.OptionsNavPane.ExpandedWidth, 10000);
			NavBar.GroupPainter.ClientPainter.CalcObjectBounds(groupInfo.ClientInfo);
			foreach(NavLinkInfoArgs link in groupInfo.Links) {
				width = groupInfo.ClientInfo.NavBar.LinkPainter.CalcBestWidth( link, groupInfo.ClientInfo.Bounds, groupInfo.Group );
				sz.Width = Math.Max(sz.Width, width);
			}
			if(sz.Width == 0) sz.Width = 100;
			sz.Height = 100;
			if(groupInfo.Links.Count > 0) {
				sz.Height = (groupInfo.Links[groupInfo.Links.Count - 1] as NavLinkInfoArgs).Bounds.Bottom - (groupInfo.Links[0] as NavLinkInfoArgs).Bounds.Top;
				sz.Height += ((groupInfo.Links[0] as NavLinkInfoArgs).Bounds.Top - groupInfo.Bounds.Top) * 2;
			}
			return sz;
		}
		protected virtual int CalcExpandedGroupInfo(Rectangle bounds, NavBarGroup expandedGroup) {
			CalcHeaderInfo(bounds, expandedGroup);
			int top = HeaderInfo.Bounds.Bottom;
			if(top > bounds.Bottom) return 0; 
			int clientHeight = Math.Max(GetGroupClientMinHeight(), expandedGroup.GroupClientHeight);
			NavBarCalcGroupClientHeightEventArgs e = new NavBarCalcGroupClientHeightEventArgs(expandedGroup, clientHeight);
			expandedGroup.RaiseCalcGroupClientHeight(e);
			clientHeight = e.Height;
			clientHeight = Math.Min(clientHeight, bounds.Bottom - top);
			if(clientHeight < 20) clientHeight = 20;
			NavGroupInfoArgs groupInfo = new NavGroupInfoArgs(expandedGroup, Rectangle.Empty);
			groupInfo.Graphics = GInfo.Graphics;
			groupInfo.ClientInfo.Bounds = new Rectangle(bounds.X, top, bounds.Width, clientHeight);
			NavBar.GroupPainter.ClientPainter.CalcObjectBounds(groupInfo.ClientInfo);
			groupInfo.Graphics = null;
			Groups.Add(groupInfo);
			return groupInfo.ClientInfo.Bounds.Bottom;
		}
		protected virtual int CalcSplitterInfo(Rectangle bounds) {
			if(bounds.Height < 1) return 0;
			this.splitterInfo = new ObjectInfoArgs();
			this.splitterInfo.Bounds = bounds;
			this.splitterInfo.Graphics = GInfo.Graphics;
			int divHeight = (NavBar.OptionsNavPane.ShowSplitter && !IsAttachedToOfficeNavigationBar) ? SplitterPainter.CalcObjectMinBounds(SplitterInfo).Height: 0;
			this.splitterInfo.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, divHeight);
			this.splitterInfo.Graphics = null;
			return SplitterInfo.Bounds.Bottom;
		}
		protected virtual void CalcHeaderInfo(Rectangle bounds, NavBarGroup expandedGroup) {
			this.headerInfo = CreateHeaderInfo(expandedGroup);
			this.headerInfo.Bounds = bounds;
			this.headerInfo.Graphics = GInfo.Graphics;
			this.headerInfo.OptionsNavPane = new OptionsNavPane(NavBar.OptionsNavPane);
			this.headerInfo.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 32);
			CalcHeaderButtonInfo(headerInfo);
			this.headerInfo.CaptionBounds = HeaderPainter.GetCaptionBounds(this.headerInfo);
			int headerHeight = HeaderPainter.CalcObjectMinBounds(HeaderInfo).Height;
			this.headerInfo.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, headerHeight);
			CalcHeaderButtonInfo(headerInfo);
			this.headerInfo.CaptionBounds = HeaderPainter.GetCaptionBounds(this.headerInfo);
			this.headerInfo.ImageBounds = HeaderPainter.GetHeaderImageBounds(this.headerInfo);
			CheckHeaderBounds(bounds);
		}
		protected void CheckHeaderBounds(Rectangle contentBounds) {
			HeaderInfo.ButtonBounds = CheckBounds(HeaderInfo.ButtonBounds);
			HeaderInfo.CaptionBounds = CheckBounds(HeaderInfo.CaptionBounds);
			HeaderInfo.ImageBounds = CheckBounds(HeaderInfo.ImageBounds);
		}
		protected override void CalcViewInfo(Rectangle bounds) {
			RemoveUnusedLinksFromCache();
			base.CalcViewInfo(bounds);
			OptionsNavPane options = NavBar.OptionsNavPane;
			if(options.NavPaneState == NavPaneState.Collapsed) {
				int bottom = SplitterBounds.Top;
				if(bottom == 0) bottom = Bounds.Bottom;
				ContentButton = new Rectangle(0, HeaderBounds.Bottom, Bounds.Width, bottom - HeaderBounds.Height);
			}
			else ContentButton = Rectangle.Empty;
			UpdateCollapsedNavPaneContentControl();
		}
		Control prevContentControl = null;
		protected virtual void UpdateCollapsedNavPaneContentControl() {
			if(NavBar == null || NavBar.IsDesignMode) return;
			Control control = GetCollapsedNavPaneContentControl();
			if(this.prevContentControl != null && this.prevContentControl != control) this.prevContentControl.Visible = false;
			if(this.prevContentControl != control)
				this.prevContentControl = control;
			if(control == null) return;
			Rectangle clientBounds = ContentButton;
			if(clientBounds.IsEmpty) {
				control.Visible = false;
			}
			else {
				if(!control.Visible) control.Visible = true;
				if(control.Dock != DockStyle.None) control.Dock = DockStyle.None;
				control.SetBounds(clientBounds.X, clientBounds.Y, clientBounds.Width, clientBounds.Height, BoundsSpecified.Size | BoundsSpecified.Location);
			}
		}
		protected internal override void EnsureGroupControls() {
			if(NavBar == null || NavBar.IsDesignMode || NavBar.IsLoading) return;
			foreach(NavBarGroup group in NavBar.Groups) {
				if(group.CollapsedNavPaneContentControl != null) NavBar.OnInitContentControl(group.CollapsedNavPaneContentControl);
			}
			Control control = NavBar.OptionsNavPane.CollapsedNavPaneContentControl;
			if(control != null) NavBar.OnInitContentControl(control);
		}
		protected virtual Control GetCollapsedNavPaneContentControl() {
			if(NavBar == null) return null;
			if(NavBar.ActiveGroup != null && NavBar.ActiveGroup.CollapsedNavPaneContentControl != null)
				return NavBar.ActiveGroup.CollapsedNavPaneContentControl;
			return NavBar.OptionsNavPane.CollapsedNavPaneContentControl;
		}
		protected virtual void CalcHeaderButtonInfo(NavigationPaneHeaderObjectInfo info) {
			Size bSize = this.HeaderPainter.GetButtonSize(info);
			int offset = (this.headerInfo.Bounds.Height - bSize.Height) / 2;
			int hzOffset = offset > bSize.Width / 2? bSize.Width / 2: offset;
			if(info.OptionsNavPane.ShowExpandButton)
				this.headerInfo.ButtonBounds = new Rectangle(this.headerInfo.Bounds.Right - hzOffset - bSize.Width, this.headerInfo.Bounds.Top + hzOffset, bSize.Width, bSize.Height);
			else
				this.headerInfo.ButtonBounds = Rectangle.Empty;
			UpdateHeaderButtonState();
			this.headerInfo.OptionsNavPane = new OptionsNavPane(this.NavBar.OptionsNavPane);
		}
		protected internal virtual void UpdateHeaderButtonState() {
			this.headerInfo.ButtonState = ObjectState.Normal;
			if(PressedInfo != null && PressedInfo.HitTest == NavBarHitTest.ExpandButton) this.headerInfo.ButtonState = ObjectState.Pressed;
			else if(HotInfo != null && HotInfo.HitTest == NavBarHitTest.ExpandButton) this.headerInfo.ButtonState = ObjectState.Hot;
		}
		protected virtual void DrawHeader(GraphicsCache e) {
			if(HeaderBounds.IsEmpty) return;
			ObjectPainter.DrawObject(e, HeaderPainter, HeaderInfo);
		}
		protected virtual void DrawSplitter(GraphicsCache e) {
			if(!NavBar.OptionsNavPane.ShowSplitter || SplitterBounds.IsEmpty) return;
			SplitterInfo.State = ObjectState.Normal;
			if(HotInfo.HitTest == NavBarHitTest.NavigationPaneSplitter || PressedInfo.HitTest == NavBarHitTest.NavigationPaneSplitter)
				SplitterInfo.State = ObjectState.Hot;
			ObjectPainter.DrawObject(e, SplitterPainter, SplitterInfo);
		}
		protected virtual void DrawOverflowHeader(GraphicsCache e) {
			if(OverflowBounds.IsEmpty) return;
			ObjectPainter.DrawObject(e, OverflowPanelPainter, OverflowInfo);
		}
		protected override void DrawGroups(GraphicsCache e) {
			DrawHeader(e);
			CheckGroupsBounds(e);
			base.DrawGroups(e);
			DrawOverflowHeader(e);
			DrawSplitter(e);
		}
		protected virtual bool ShouldCorrectGroupsBounds {
			get {
				for(int i = Groups.Count - 1; i >= 0; i--) {
					NavGroupInfoArgs groupInfo = Groups[i] as NavGroupInfoArgs;
					if(HeaderBounds.Height == groupInfo.ClientInfoBounds.Y && groupInfo.ClientInfoBounds.Y + groupInfo.ClientInfoBounds.Height == SplitterBounds.Y - 1)
						return true;
				}
				return false;
			}
		}
		protected virtual void CheckGroupsBounds(GraphicsCache e) {
			if(!ShouldCorrectGroupsBounds) return;
			SplitterInfo.Bounds = new Rectangle(SplitterBounds.X, SplitterBounds.Y - 1, SplitterBounds.Width, SplitterBounds.Height);
			for(int i = Groups.Count - 1; i >= 0; i--) {
				NavGroupInfoArgs groupInfo = Groups[i] as NavGroupInfoArgs;
				if(!groupInfo.Bounds.IsEmpty)
					groupInfo.Bounds = new Rectangle(groupInfo.Bounds.X, groupInfo.Bounds.Y - 1, groupInfo.Bounds.Width, groupInfo.Bounds.Height);
			}
			OverflowInfo.Bounds = new Rectangle(OverflowBounds.X, OverflowBounds.Y - 1, OverflowBounds.Width, OverflowBounds.Height + 1);
		}
		protected override void DrawButtons(GraphicsCache e) {
			if(NavBar.OptionsNavPane.ActualNavPaneState == NavPaneState.Collapsed) return;
			base.DrawButtons(e);
		}
		public override void InvalidateHitObject(NavBarHitInfo hitInfo) {
			if(hitInfo.HitTest == NavBarHitTest.NavigationPaneOverflowPanel || 
				hitInfo.HitTest == NavBarHitTest.NavigationPaneOverflowPanelButton) {
				Invalidate(OverflowBounds);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.ExpandButton) {
				UpdateHeaderButtonState();
				Invalidate(HeaderInfo.ButtonBounds);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.NavigationPaneHeader) {
				Invalidate(HeaderBounds);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.NavigationPaneSplitter) {
				Invalidate(SplitterBounds);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.ContentButton) {
				Invalidate(ContentButton);
			}
			base.InvalidateHitObject(hitInfo);
		}
		protected virtual DXPopupMenu CreateOverflowMenu() {
			DXPopupMenu menu = new DXPopupMenu(new EventHandler(OnOverflowMenu_BeforePopup));
			menu.Items.Add(new DXMenuItem(NavBarLocalizer.Active.GetLocalizedString(NavBarStringId.NavPaneMenuShowMoreButtons), 
				new EventHandler(OnOverflowMenu_ShowMoreButtons), GetPaneImage(NavigationPaneImage.MoreButtons)));
			menu.Items.Add(new DXMenuItem(NavBarLocalizer.Active.GetLocalizedString(NavBarStringId.NavPaneMenuShowFewerButtons), 
				new EventHandler(OnOverflowMenu_ShowFewerButtons), GetPaneImage(NavigationPaneImage.FewerButtons)));
			if(NavBar.OptionsNavPane.AllowOptionsMenuItem)
				menu.Items.Add(new DXMenuItem(NavBarLocalizer.Active.GetLocalizedString(NavBarStringId.NavPaneMenuPaneOptions), new EventHandler(OnOverflowMenu_PaneOptions)));
			menu.Items.Add(new DXSubMenuItem(NavBarLocalizer.Active.GetLocalizedString(NavBarStringId.NavPaneMenuAddRemoveButtons), new EventHandler(OnOverflowMenu_AddRemoveBeforePopup)));
			bool isFirst = false;
			foreach(NavBarGroup nbGroup in OverflowInfo.OverflowGroups) {
				if(!ShouldAddGroupToOverflowMenu(nbGroup)) continue;
				DXMenuItem item = new DXMenuItem(nbGroup.Caption);
				item.Click += new EventHandler(new EventHandler(OnOverflowGroupMenuItemClick));
				if(!isFirst) { isFirst = true; item.BeginGroup = true; }
				item.Image = nbGroup.GetSmallImage();
				item.Tag = nbGroup;
				menu.Items.Add(item);
			}
			return menu;
		}
		protected virtual bool ShouldAddGroupToOverflowMenu(NavBarGroup group) {
			if(group == null) return false;
			foreach(NavigationPaneOverflowPanelObjectInfo btnInfo in OverflowInfo.Buttons) {
				if(object.ReferenceEquals(btnInfo.Group, group))
					return false;
			}
			return true;
		}
		protected void OnOverflowGroupMenuItemClick(object sender, EventArgs ea) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			NavBarGroup group = item.Tag as NavBarGroup;
			if(group == null) return;
			group.Expanded = true;
		}
		protected virtual void DoChangeVisibleGroupCount(int delta) {
			int prevVisibleCount = GetVisibleGroupHeadersCount();
			if(prevVisibleCount + delta < 0) return;
			int prevCount = NavBar.NavigationPaneMaxVisibleGroups;
			NavBar.NavigationPaneMaxVisibleGroups = prevVisibleCount + delta;
			if(GetVisibleGroupHeadersCount() == prevVisibleCount)
				NavBar.NavigationPaneMaxVisibleGroups = prevCount;
		}
		protected virtual void OnOverflowMenu_ShowMoreButtons(object sender, EventArgs e) {
			if(NavBar.NavigationPaneMaxVisibleGroups == -1) return;
			DoChangeVisibleGroupCount(1);
		}
		protected virtual void OnOverflowMenu_ShowFewerButtons(object sender, EventArgs e) {
			if(NavBar.NavigationPaneMaxVisibleGroups == 0) return;
			DoChangeVisibleGroupCount(-1);
		}
		protected virtual void OnOverflowMenu_PaneOptions(object sender, EventArgs e) {
			PaneCustomizationHelper hr = NavBar.PaneCustomizationHelper;
			using(NavPaneOptionsForm frm = new NavPaneOptionsForm(NavBar, hr)) {
				DialogResult dlgRes = frm.ShowDialog();
				if(dlgRes == DialogResult.OK) hr.Customize(NavBar, frm.CustomizationInfo);
			}
		}
		protected virtual void OnOverflowMenu_AddRemoveBeforePopup(object sender, EventArgs e) {
			DXSubMenuItem item = sender as DXSubMenuItem;
			item.Items.Clear();
			foreach(NavBarGroup group in NavBar.Groups) {
				if(!group.IsVisible) continue;
				string groupCaption = group.GetAllowHtmlString() ? StringPainter.Default.RemoveFormat(group.Caption, true) : group.Caption;
				DXMenuCheckItem check = new DXMenuCheckItem(groupCaption, group.NavigationPaneVisible);
				Image image = group.GetSmallImage();
				check.Image = image == null ? NavigationPaneViewInfo.GetPaneImage(NavigationPaneImage.SmallGroup) : image;
				check.Tag = group;
				check.CheckedChanged += new EventHandler(OnOverflowMenu_CheckedChanged);
				item.Items.Add(check);
			}
			item.Enabled = item.Items.Count > 0;
		}
		protected virtual void OnOverflowMenu_CheckedChanged(object sender, EventArgs e) {
			DXMenuCheckItem checkItem = sender as DXMenuCheckItem;
			NavBarGroup group = checkItem.Tag as NavBarGroup;
			group.NavigationPaneVisible = checkItem.Checked;
		}
		protected virtual void OnOverflowMenu_BeforePopup(object sender, EventArgs e) {
			DXPopupMenu menu = sender as DXPopupMenu;
			menu.Items[0].Enabled = GetCanShowMoreGroups();
			menu.Items[1].Enabled = GetCanShowFewerGroups();
		}
		protected override bool AllowDesignClick(NavBarHitInfo hitInfo) {
			NavBarNavigationPaneHitInfo navHit = hitInfo as NavBarNavigationPaneHitInfo;
			if(navHit.OverflowPanelButton != null && navHit.OverflowPanelButton.Group != null) return true;
			return base.AllowDesignClick(hitInfo);
		}
		protected override int CalcInterval(int interval, NavBarHintInfo newHintInfo) {
			if(newHintInfo.ObjectType == NavBarHintObjectType.NavigationPaneOveflowGroup || newHintInfo.ObjectType == NavBarHintObjectType.NavigationPaneOveflowChevron) {
				return interval / 2;
			}
			return base.CalcInterval(interval, newHintInfo);
		}
		protected override bool GetCanShowHint(NavBarHitInfo hitInfo) {
			if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed) return true;
			if(hitInfo.HitTest == NavBarHitTest.NavigationPaneOverflowPanelButton) return true;
			return base.GetCanShowHint(hitInfo);
		}
		protected override NavBarHintInfo CalcHintInfo(NavBarHitInfo hitInfo) {
			NavBarNavigationPaneHitInfo navHit = hitInfo as NavBarNavigationPaneHitInfo;
			if(!NavBar.IsDesignMode) {
				if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed && navHit.HitTest == NavBarHitTest.GroupCaption) {
					NavBarGroup nbGroup = navHit.Group;
					if(nbGroup != null) {
						NavBarHintInfo hint = new NavBarHintInfo(PaintAppearance.Hint);
						hint.SetHint(NavBarHintObjectType.Group, nbGroup, nbGroup.Caption, hitInfo.Group.SuperTip);
						return hint;
					}
				}
				if(navHit.HitTest == NavBarHitTest.NavigationPaneOverflowPanelButton) {
					NavigationPaneOverflowPanelObjectInfo button = navHit.OverflowPanelButton;
					if(button != null) {
						NavBarHintInfo hint = new NavBarHintInfo(PaintAppearance.Hint);
						SuperToolTip superTip = button.IsChevron ? null : button.Group.SuperTip;
						hint.SetHint(button.Group == null ? NavBarHintObjectType.NavigationPaneOveflowChevron : NavBarHintObjectType.NavigationPaneOveflowGroup, button.Group, button.HintText, superTip);
						return hint;
					}
					return NavBarHintInfo.Empty;
				}
				if(navHit.HitTest == NavBarHitTest.ContentButton) {
					NavBarHintInfo hint = new NavBarHintInfo(PaintAppearance.Hint);
					SuperToolTip superTip = (hitInfo.Group == null) ? null : hitInfo.Group.SuperTip;
					hint.SetHint(NavBarHintObjectType.NavPaneContentButton, this, NavBar.ContentButtonHint, superTip);
					return hint;
				}
			}
			return base.CalcHintInfo(hitInfo);
		}
		public override void OnMouseMove(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseMove(e);
			if(e.Handled) return;
			if(e.Button == MouseButtons.Left && State == NavBarState.NavigationPaneSizing) {
				DoNavigationPaneSizing(e);
			}
		}
		NavGroupInfoArgs GetFirstVisibleGroupHeader() {
			int minY = -10000;
			NavGroupInfoArgs res = null;
			foreach(NavGroupInfoArgs groupInfo in Groups) {
				if(groupInfo.Bounds.IsEmpty) continue;
				if(minY == -10000 || groupInfo.Bounds.Top < minY) {
					res = groupInfo;
					minY = groupInfo.Bounds.Top;
				}
			}
			return res;
		}
		protected virtual void DoNavigationPaneSizing(MouseEventArgs e) {
			if(SplitterBounds.IsEmpty) return;
			int prevVisibleCount = GetVisibleGroupHeadersCount();
			int prevCount = NavBar.NavigationPaneMaxVisibleGroups;
			try {
				if(e.Y > SplitterBounds.Bottom) {
					if(GetVisibleGroupHeadersCount() > 0) {
						NavGroupInfoArgs groupInfo = GetFirstVisibleGroupHeader();
						if(groupInfo != null && e.Y > groupInfo.Bounds.Bottom)
							NavBar.NavigationPaneMaxVisibleGroups = GetVisibleGroupHeadersCount() - 1;
					}
				}
				if(e.Y < SplitterBounds.Top) {
					if(SplitterBounds.Top - e.Y > 20 && GetCanShowMoreGroups()) {
						NavBar.NavigationPaneMaxVisibleGroups = GetVisibleGroupHeadersCount() + 1;
					}
				}
			}
			finally {
				if(prevVisibleCount == GetVisibleGroupHeadersCount())
					NavBar.NavigationPaneMaxVisibleGroups = prevCount;
			}
		}
		protected internal override void DoClick(NavBarHitInfo hitInfo) {
			NavBarNavigationPaneHitInfo paneHitInfo = hitInfo as NavBarNavigationPaneHitInfo;
			if(paneHitInfo.HitTest == NavBarHitTest.NavigationPaneOverflowPanelButton) {
				DoOverflowPanelButtonClick(paneHitInfo.OverflowPanelButtonIndex);
				return;
			}
			base.DoClick(hitInfo);
		}
		protected override void DoContentButtonPress() {
			if(NavBar.NavPaneForm != null) {
				bool visible = NavBar.NavPaneForm.Visible;
				NavBar.HideNavPaneForm();
				if(visible)
					return;
			}
			ResetLinkSizesCache();
			NavPaneForm form = NavBar.View.CreateNavPaneForm(NavBar);
			NavBar.SetNavPaneForm(form);
			form.Size = new Size(NavBar.OptionsNavPane.ExpandedWidth, form.CalcBestSize().Height);
			Rectangle r = Screen.GetWorkingArea(NavBar);
			if(NavBar.IsRightToLeftDirection) {
				form.Location = NavBar.PointToScreen(new Point(ContentButton.Left - form.Width, ContentButton.Top));
			}
			else {
				form.Location = NavBar.PointToScreen(new Point(ContentButton.Right, ContentButton.Top));
			}
			if(form.Location.Y + form.Height > r.Bottom) {
				form.Location = new Point(form.Location.X, r.Bottom - form.Height);
			}
			form.Show();
			NavBar.RaiseNavPaneMinimizedGroupFormShowing(new NavPaneMinimizedGroupFormShowingEventArgs(NavBar, form));
		}
		protected virtual void DoOverflowPanelButtonClick(int buttonIndex) {
			if(OverflowInfo == null) return;
			NavigationPaneOverflowPanelObjectInfo button = OverflowInfo.GetButtonInfo(buttonIndex);
			if(button != null) {
				if(button.Group != null) {
					if(!RaiseGroupExpanding(button.Group)) return;
					button.Group.Expanded = true;
					return;
				}
				ShowMenu(CreateOverflowMenu());
			}
		}
		protected virtual void ShowMenu(DXPopupMenu menu) {
			MenuManagerHelper.Office2003.ShowPopupMenu(menu, NavBar, MousePosition);
		}
		protected override void UpdateStates() {
			base.UpdateStates();
			if(OverflowInfo == null) return;
			foreach(NavigationPaneOverflowPanelObjectInfo info in OverflowInfo.Buttons) {
				info.State = CalcPaneObjectState(info);
			}
		}
		protected int OverflowPanelPressedButtonIndex {
			get { return (PressedInfo as NavBarNavigationPaneHitInfo).OverflowPanelButtonIndex; }
		}
		protected int OverflowPanelHotButtonIndex {
			get { return (HotInfo as NavBarNavigationPaneHitInfo).OverflowPanelButtonIndex; }
		}
		protected int OverflowPanelSelectedButtonIndex {
			get {
				if(OverflowInfo == null) return -1;
				return OverflowInfo.GetButtonIndex(GetExpandedGroup());
			}
		}
		protected virtual ObjectState CalcPaneObjectState(NavigationPaneOverflowPanelObjectInfo info) {
			int index = OverflowInfo.Buttons.IndexOf(info);
			ObjectState state = ObjectState.Normal;
			if(OverflowPanelHotButtonIndex == index) state |= ObjectState.Hot;
			if(OverflowPanelPressedButtonIndex == index) state |= ObjectState.Pressed;
			if(OverflowPanelSelectedButtonIndex == index) state |= ObjectState.Selected;
			return state;
		}
		protected override NavBarHitTest[] CreateValidHotTracks() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, NavBarHitTest.NavigationPaneOverflowPanelButton,
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link, NavBarHitTest.NavigationPaneSplitter, NavBarHitTest.ExpandButton, NavBarHitTest.ContentButton };
		}
		protected override NavBarHitTest[] CreateValidPressedInfo() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, 
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link,
					NavBarHitTest.NavigationPaneOverflowPanelButton,
					NavBarHitTest.NavigationPaneSplitter, NavBarHitTest.ExpandButton, NavBarHitTest.ContentButton
			};
		}
		protected override NavBarState[] CreateValidPressedStateInfo() {
			return new NavBarState[] {
					NavBarState.UpButtonPressed, NavBarState.DownButtonPressed,
					NavBarState.GroupPressed, 
					NavBarState.LinkPressed, NavBarState.LinkPressed, NavBarState.LinkPressed,
					NavBarState.NavigationPaneOveflowButton,
					NavBarState.NavigationPaneSizing, NavBarState.ExpandButtonPressed, NavBarState.ContentButtonPressed
				};
		}
		protected override void UpdateCursor() {
			if(!CanUpdateCursor) return;
			NavBarNavigationPaneHitInfo hitInfo = CalcHitInfo(MousePosition) as NavBarNavigationPaneHitInfo;
			if(State == NavBarState.NavigationPaneSizing || hitInfo.HitTest == NavBarHitTest.NavigationPaneSplitter) {
				SetCursor(Cursors.SizeNS);
				return;
			}
			if(hitInfo.InGroupCaption) {
				if(NavBar.HotTrackedGroupCursor == Cursors.Default) 
					SetCursor(Cursors.Hand);
				else
					SetCursor(NavBar.HotTrackedGroupCursor);
				return;
			}
			base.UpdateCursor();
		}
		public override NavBarHitInfo CreateHitInfo() { return new NavBarNavigationPaneHitInfo(NavBar); }
		public virtual bool ShouldDrawGroupImage {
			get {
				var options = NavBar.OptionsNavPane;
				if(options == null || options.GroupImageShowMode == GroupImageShowMode.Always) return true;
				return NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed;
			}
		}
		public override void Dispose() {
			this.prevContentControl = null;
			base.Dispose();
		}
	}
	public class NavigationPaneSplitterPainter : ObjectPainter {
		NavigationPaneViewInfo viewInfo;
		public NavigationPaneSplitterPainter(NavigationPaneViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected NavigationPaneViewInfo ViewInfo { get { return viewInfo; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Size size = NavigationPaneViewInfo.GetPaneImage(NavigationPaneImage.Splitter).Size;
			size.Height += 3;
			return new Rectangle(Point.Empty, size);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Image image = NavigationPaneViewInfo.GetPaneImage(NavigationPaneImage.Splitter);
			Color color = ViewInfo.PaintAppearance.NavigationPaneHeader.BackColor; 
			Brush backBrush = e.Cache.GetGradientBrush(e.Bounds, color, ViewInfo.PaintAppearance.NavigationPaneHeader.BackColor2, LinearGradientMode.Vertical);
			Rectangle r = e.Bounds;
			e.Cache.Paint.FillRectangle(e.Graphics, backBrush, e.Bounds);
			r.Size = image.Size;
			r.X = e.Bounds.X + (e.Bounds.Width - r.Width) / 2;
			r.Y = e.Bounds.Y + 1;
			e.Cache.Paint.DrawImage(e.Graphics, image, r.X, r.Y, new Rectangle(Point.Empty, r.Size));
		}
	}
	public class NavigationPaneOverflowPanelObjectInfo : StyleObjectInfoArgs {
		NavBarGroup group;
		Image image;
		Size imageSize;
		NavBarControl navBar;
		public NavigationPaneOverflowPanelObjectInfo(NavBarControl navBar, NavBarGroup group, Size imageSize, AppearanceObject appearance) : base(null, Rectangle.Empty, appearance, ObjectState.Normal) {
			this.navBar = navBar;
			this.group = group;
			this.image = null;
			this.imageSize = imageSize;
			UpdateImage();
		}
		public override AppearanceObject Appearance {
			get {
				if((State & ObjectState.Selected) != 0) return NavBar.PaintAppearance.GroupHeaderActive;
				if((State & ObjectState.Pressed) != 0) return NavBar.PaintAppearance.GroupHeaderPressed;
				if((State & ObjectState.Hot) != 0) return NavBar.PaintAppearance.GroupHeaderHotTracked;
				return NavBar.PaintAppearance.GroupHeader;
			}
		}
		public Size ImageSize { get { return imageSize; } set { imageSize = value; } }
		public NavBarGroup Group { get { return group; } set { group = value; UpdateImage(); } }
		private bool useImage = false;
		public bool UseImage {
			get { return useImage; }
			set { useImage = value; }
		}
		public bool IsChevron { get { return Group == null && !useImage; } }
		public Image Image { 
			get { 
				return IsChevron ? NavigationPaneViewInfo.GetPaneImage(NavigationPaneImage.Chevron): image; 
			} 
			set { image = value; } 
		}
		protected NavBarControl NavBar { get { return navBar; } }
		NavigationPaneViewInfo ViewInfo {
			get { return (NavBar == null ? null : NavBar.ViewInfo) as NavigationPaneViewInfo;
			}
		}
		protected void UpdateImage() {
			if(Group == null) return;
			if(ViewInfo == null || ViewInfo.GetOverflowPanelUseSmallImages()) 
				this.image = Group.GetSmallImage();
			else
				this.image = Group.GetLargeImage();
			if(this.image == null) {
				this.image = ViewInfo != null ? ViewInfo.GetOverflowPanelDefaultImage() : null;
			}
		}
		public string HintText { 
			get { 
				if(IsChevron) return NavBarLocalizer.Active.GetLocalizedString(NavBarStringId.NavPaneChevronHint);
				return Group.Caption; 
			} 
		}
	}
	public class NavigationPaneOverflowPanelObjectPainter : StyleObjectPainter {
		ObjectPainter buttonPainter;
		NavBarControl navBar;
		public NavigationPaneOverflowPanelObjectPainter(NavBarControl navBar) {
			this.navBar = navBar;
			this.buttonPainter = CreateButtonPainter(navBar);
		}
		protected NavBarControl NavBar { get { return navBar; } }
		protected ObjectPainter ButtonPainter { get { return buttonPainter; } }
		protected virtual ObjectPainter CreateButtonPainter(NavBarControl navBar) {
			return new NavigationPaneButtonPainter(navBar, BorderSide.None, false);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelObjectInfo info = e as NavigationPaneOverflowPanelObjectInfo;
			Size size = info.ImageSize;
			size.Width += 4;
			size.Height += 8;
			return new Rectangle(Point.Empty, size);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelObjectInfo info = e as NavigationPaneOverflowPanelObjectInfo;
			ButtonPainter.DrawObject(info);
			Rectangle bounds = ButtonPainter.GetObjectClientRectangle(info);
			Image image = info.Image;
			if(image == null) return;
			if(info.IsChevron && NavBar != null && NavBar.IsRightToLeft) image = NavBarPaintHelper.GetRTLImage(image);
			Size imageSize = image.Size;
			Rectangle r = new Rectangle(
				bounds.X + (bounds.Width - imageSize.Width) / 2,
				bounds.Y + (bounds.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
			e.Cache.Paint.DrawImage(e.Graphics, image, r, new Rectangle(Point.Empty, image.Size), true);
		}
	}
	public class NavigationPaneOverflowPanelInfo : StyleObjectInfoArgs {
		Size imageSize;
		ArrayList overflowGroups;
		ArrayList buttons;
		Rectangle buttonBounds;
		NavBarControl navBar;
		public NavigationPaneOverflowPanelInfo(NavBarControl navBar) : base(null, Rectangle.Empty, null, ObjectState.Normal) {
			this.navBar = navBar;
			this.imageSize = new Size(16, 16);
			this.buttonBounds = Rectangle.Empty;
			this.buttons = new ArrayList();
			this.overflowGroups = new ArrayList();
		}
		public NavBarControl NavBar { get { return navBar; } }
		public override AppearanceObject Appearance { get { return NavBar.PaintAppearance.GroupHeader; } }
		public NavigationPaneOverflowPanelObjectInfo GetButtonInfo(int index) {
			if(index < 0 || Buttons.Count <= index) return null;
			return Buttons[index] as NavigationPaneOverflowPanelObjectInfo;
		}
		public NavigationPaneOverflowPanelObjectInfo GetButtonInfo(NavBarGroup group) {
			int index = GetButtonIndex(group);
			if(index > -1) return GetButtonInfo(index);
			return null;
		}
		public int GetButtonIndex(NavBarGroup group) {
			for(int n = 0; n < Buttons.Count; n++) {
				if(GetButtonInfo(n).Group == group) return n;
			}
			return -1;
		}
		public ArrayList OverflowGroups { get { return overflowGroups; } }
		public ArrayList Buttons { get { return buttons; } }
		public Size ImageSize { get { return imageSize; } set { imageSize = value; } }
		public Rectangle ButtonBounds { get { return buttonBounds; } set { buttonBounds = value; } }
		public virtual void Clear() {
			this.buttonBounds = Rectangle.Empty;
			this.buttons.Clear();
		}
	}
	public class NavigationPaneOverflowPanelPainter : StyleObjectPainter {
		NavigationPaneOverflowPanelObjectPainter buttonPainter;
		ObjectPainter panelButtonPainter;
		NavBarControl navBar;
		public NavigationPaneOverflowPanelPainter(NavBarControl navBar) {
			this.navBar = navBar;
			this.buttonPainter = CreateButtonPainter();
			this.panelButtonPainter = CreatePanelButtonPainter();
		}
		public NavBarControl NavBar { get { return navBar; } }
		protected virtual ObjectPainter CreatePanelButtonPainter() {
			return new NavigationPaneButtonPainter(NavBar);
		}
		protected virtual NavigationPaneOverflowPanelObjectPainter CreateButtonPainter() {
			return new NavigationPaneOverflowPanelObjectPainter(NavBar);
		}
		public NavigationPaneOverflowPanelObjectPainter ButtonPainter { get { return buttonPainter; } }
		public ObjectPainter PanelButtonPainter { get { return panelButtonPainter; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelInfo info = e as NavigationPaneOverflowPanelInfo;
			Size size = CalcButtonInfo(info, null).Bounds.Size;
			Rectangle save = e.Bounds;
			e.Bounds = new Rectangle(save.Location, size);
			if(NavBar.OptionsNavPane.ShowOverflowPanel)
				size = PanelButtonPainter.CalcBoundsByClientRectangle(e).Size;
			e.Bounds = save;
			return new Rectangle(Point.Empty, size);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelInfo info = e as NavigationPaneOverflowPanelInfo;
			info.Clear();
			info.ButtonBounds = PanelButtonPainter.GetObjectClientRectangle(e);
			CalcButtons(info);
			return e.Bounds;
		}
		protected virtual void CalcButtons(NavigationPaneOverflowPanelInfo e) {
			e.Buttons.Add(CalcButtonInfo(e, null));
			foreach(NavBarGroup group in e.OverflowGroups) {
				e.Buttons.Insert(e.Buttons.Count - 1, CalcButtonInfo(e, group));
				int buttonsWidth = CalcButtonsWidth(e);
				if(buttonsWidth > e.ButtonBounds.Width) {
					e.Buttons.RemoveAt(e.Buttons.Count - 2);
					break;
				}
			}
			UpdateButtonBounds(e);
		}
		protected virtual void UpdateButtonBounds(NavigationPaneOverflowPanelInfo e) {
			int right = e.ButtonBounds.Right;
			for(int n = e.Buttons.Count - 1; n >= 0; n--) {
				NavigationPaneOverflowPanelObjectInfo button = e.Buttons[n] as NavigationPaneOverflowPanelObjectInfo;
				if(!NavBar.OptionsNavPane.ShowOverflowPanel || !NavBar.OptionsNavPane.ShowOverflowButton && button.IsChevron) 
					button.Bounds = Rectangle.Empty;
				else {
					button.Bounds = new Rectangle(right - button.Bounds.Width, button.Bounds.Y, button.Bounds.Width, button.Bounds.Height);
					button.Bounds = CheckBounds(e.ButtonBounds, button.Bounds);
				}
				right -= button.Bounds.Width;
			}
		}
		protected int CalcButtonsWidth(NavigationPaneOverflowPanelInfo e) {
			int width = 0;
			foreach(NavigationPaneOverflowPanelObjectInfo button in e.Buttons) {
				if(button.IsChevron && NavBar != null && !NavBar.OptionsNavPane.ShowOverflowButton) continue;
				width += button.Bounds.Width;
			}
			return width;
		}
		protected virtual NavigationPaneOverflowPanelObjectInfo CalcButtonInfo(NavigationPaneOverflowPanelInfo e, NavBarGroup group) {
			NavigationPaneOverflowPanelObjectInfo button = new NavigationPaneOverflowPanelObjectInfo(NavBar, group, e.ImageSize, e.Appearance);
			if(!NavBar.OptionsNavPane.ShowOverflowPanel) button.Bounds = Rectangle.Empty;
			else {
				Size size = ObjectPainter.CalcObjectMinBounds(e.Graphics, ButtonPainter, button).Size;
				button.Bounds = new Rectangle(e.ButtonBounds.X, e.ButtonBounds.Y, size.Width, size.Height);
				button.Bounds = CheckBounds(e.ButtonBounds, button.Bounds);
			}
			return button;
		}
		protected Rectangle CheckBounds(Rectangle contentBounds, Rectangle bounds) {
			if(IsRightToleft) 
				return new Rectangle(contentBounds.Right - bounds.Right, bounds.Y, bounds.Width, bounds.Height);
			return bounds;
		}
		protected bool IsRightToleft {
			get {
				if(NavBar == null) return false;
				return NavBar.IsRightToLeft;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			PanelButtonPainter.DrawObject(e);
			DrawButtons(e);
		}
		protected virtual void DrawButtons(ObjectInfoArgs e) {
			if(!NavBar.OptionsNavPane.ShowOverflowPanel) return;
			NavigationPaneOverflowPanelInfo info = e as NavigationPaneOverflowPanelInfo;
			foreach(NavigationPaneOverflowPanelObjectInfo buttonInfo in info.Buttons) {
				if(NavBar.OptionsNavPane.ShowOverflowButton || !buttonInfo.IsChevron)
					ObjectPainter.DrawObject(e.Cache, ButtonPainter, buttonInfo);
			}
		}
	}
	public class NavigationPaneHeaderObjectInfo : StyleObjectInfoArgs {
		string caption;
		Image image;
		Rectangle buttonBounds;
		Rectangle captionBounds;
		Rectangle imageBounds;
		private OptionsNavPane optionsNavPane;
		private ObjectState buttonState;
		public NavigationPaneHeaderObjectInfo(string caption, AppearanceObject appearance, Image img) : base(null, Rectangle.Empty, appearance, ObjectState.Normal) {
			this.caption = caption;
			this.image = img;
			this.buttonBounds = Rectangle.Empty;
			this.captionBounds = Rectangle.Empty;
		}
		public string Caption { get { return caption; } set { caption = value; } }
		public Image Image { get { return image; } set { image = value; } }
		public Rectangle ButtonBounds { get { return buttonBounds; } set { buttonBounds = value; } }
		public Rectangle CaptionBounds { get { return captionBounds; } set { captionBounds = value; } }
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		public OptionsNavPane OptionsNavPane { get { return optionsNavPane; } set { optionsNavPane = value; } }
		public ObjectState ButtonState { get { return buttonState; } set { buttonState = value; } }
	}
	public class NavigationPaneHeaderPainter : StyleObjectPainter {
		protected NavigationPaneViewInfo parentVi;
		public NavigationPaneHeaderPainter(NavigationPaneViewInfo navPAneVi) {
			this.parentVi = navPAneVi;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavigationPaneHeaderObjectInfo info = e as NavigationPaneHeaderObjectInfo;
			if (info.OptionsNavPane.NavPaneState == NavPaneState.Collapsed) return e.Bounds;
			int textHeight = CalcTextSize(e, info.Caption == "" ? "Wg" : info.Caption, 0).Height;
			textHeight = Math.Max(textHeight, CalcTextSize(e, info.Caption == "" ? "Wg" : info.Caption, info.CaptionBounds.Width).Height);
			Rectangle r = new Rectangle(e.Bounds.Location, CalcTextSize(e, info.Caption == "" ? "Wg" : info.Caption, info.CaptionBounds.Width));
			r.Height += 4;
			return r;
		}
		protected internal virtual Rectangle GetCaptionBounds(NavigationPaneHeaderObjectInfo info) {
			Rectangle r = info.Bounds;
			if(info.OptionsNavPane.ShowExpandButton) {
				r.Width = info.ButtonBounds.Left - r.Left;
			}
			r.Inflate(-6, 0);
			if(info.OptionsNavPane.ShowGroupImageInHeader) r.X += info.OptionsNavPane.DefaultNavPaneHeaderImageSize.Width + r.X;
			r.Width = CalcTextSize(info, info.Caption, r.Width).Width;
			return r;
		}
		protected internal virtual Rectangle GetHeaderImageBounds(NavigationPaneHeaderObjectInfo info) {
			Rectangle r = new Rectangle();
			if(info.Image == null) return r;
			r.X = (info.CaptionBounds.X - info.OptionsNavPane.DefaultNavPaneHeaderImageSize.Width) / 2;
			r.Y = info.CaptionBounds.Y + info.CaptionBounds.Height / 2 - info.OptionsNavPane.DefaultNavPaneHeaderImageSize.Height / 2;
			r.Width = r.Height = info.OptionsNavPane.DefaultNavPaneHeaderImageSize.Width;
			return r;
		}
		public virtual Size GetButtonSize(NavigationPaneHeaderObjectInfo info) { return new Size(16, 16); }
		static Image chevronImageLeft, chevronImageRight;
		Image GetChevronImage(OptionsNavPane navPaneOptions) {
			NavPaneState state = navPaneOptions.NavPaneState;
			if(navPaneOptions.ExpandButtonMode == DevExpress.Utils.Controls.ExpandButtonMode.Inverted)
				state = NavPaneStateConverter.Invert(state);
			if(state == NavPaneState.Expanded) {
				if(chevronImageLeft == null)
					chevronImageLeft = NavigationPaneViewInfo.Load("left.bmp");
				return chevronImageLeft;
			}
			if(state == NavPaneState.Collapsed) {
				if(chevronImageRight == null)
					chevronImageRight = NavigationPaneViewInfo.Load("right.bmp");
				return chevronImageRight;
			}
			return null;
		}
		NavigationPaneOverflowPanelObjectPainter buttonPainter = null;
		public virtual void DrawButton(NavigationPaneHeaderObjectInfo info) {
			if(buttonPainter == null)
				buttonPainter = new NavigationPaneOverflowPanelObjectPainter(parentVi.NavBar);
			NavigationPaneOverflowPanelObjectInfo butonArgs = new NavigationPaneOverflowPanelObjectInfo(parentVi.NavBar, null, info.ButtonBounds.Size, info.Appearance);
			butonArgs.Bounds = info.ButtonBounds;
			butonArgs.Cache = info.Cache;
			butonArgs.State = info.ButtonState;
			butonArgs.UseImage = true;
			butonArgs.Image = GetChevronImage(info.OptionsNavPane);
			if(parentVi.IsRightToLeft) butonArgs.Image = NavBarPaintHelper.GetRTLImage(butonArgs.Image);
			buttonPainter.DrawObject(butonArgs);
		}
		protected virtual void DrawCaptionAndButton(NavigationPaneHeaderObjectInfo info, Brush foreBrush) {
			if(info.OptionsNavPane.ShowExpandButton) DrawButton(info);
			if(info.OptionsNavPane.NavPaneState != NavPaneState.Expanded)
				return;
			if(info.OptionsNavPane.ShowGroupImageInHeader && info.Image != null) {
				if(parentVi.NavBar.AllowGlyphSkinning)
					info.Cache.Paint.DrawImage(info.Cache.Graphics, info.Image, info.ImageBounds, new Rectangle(Point.Empty, info.Image.Size), ImageColorizer.GetColoredAttributes(info.Appearance.ForeColor));
				else
					info.Cache.Paint.DrawImage(info.Cache.Graphics, info.Image, info.ImageBounds);
			}
			if (info.OptionsNavPane.ShowHeaderText) {
				info.Appearance.DrawString(info.Cache, info.Caption, info.CaptionBounds, foreBrush);
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPaneHeaderObjectInfo info = e as NavigationPaneHeaderObjectInfo;
			Brush foreBrush;
			foreBrush = info.Appearance.GetForeBrush(e.Cache);
			Rectangle r = info.Bounds;
			info.Appearance.DrawBackground(e.Cache, info.Bounds, true);
			DrawCaptionAndButton(info, foreBrush);
		}
	}
	public class NavigationPaneBorderPainter : SimpleBorderPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			Color clr = GetStyle(e).BorderColor;
			if(clr == Color.Empty) clr = Office2003Colors.Default[Office2003Color.NavPaneBorderColor];
			e.Cache.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(clr), r);
		}
	}
	public class NavigationPaneLinkPainter : BaseNavLinkPainter {
		ObjectPainter buttonPainter;
		public NavigationPaneLinkPainter(NavBarControl navBar) : base(navBar) {
			this.buttonPainter = new NavigationPaneButtonPainter(navBar, BorderSide.All, false);
		}
		protected ObjectPainter ButtonPainter { get { return buttonPainter; } }
		public override ObjectState CalcLinkState(NavBarItemLink link, ObjectState state) {
			return state;
		}
		protected override Brush GetLinkCaptionDisabledBrush(ObjectInfoArgs e, Brush foreBrush) {
			return null;
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			AppearanceObject appearance = GetLinkAppearance(e);
			r = Rectangle.Inflate(e.Bounds, -1, 0);
			StyleObjectInfoArgs styleInfo = new StyleObjectInfoArgs(e.Cache, r, appearance, e.State);
			ButtonPainter.DrawObject(styleInfo);
		}
		protected override int CalcLargeVertIndent(ObjectInfoArgs e) {
			return 4;
		}
	}
	public class NavigationPaneButtonPainter : ButtonObjectPainter {
		bool allowDefaultFill;
		BorderSide borderSides;
		NavBarControl navBar;
		public NavigationPaneButtonPainter(NavBarControl navBar) : this(navBar, BorderSide.Top, true) { }
		public NavigationPaneButtonPainter(NavBarControl navBar, BorderSide borderSides, bool allowDefaultFill) {
			this.allowDefaultFill = allowDefaultFill;
			this.borderSides = borderSides;
			this.navBar = navBar;
		}
		protected NavBarControl NavBar { get { return navBar; } }
		protected BorderSide BorderSides { get { return borderSides; } }
		protected bool AllowDefaultFill { get { return allowDefaultFill; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r = TransformRectangle(r, BorderSides, 1);
			return r;
		}
		protected Rectangle TransformRectangle(Rectangle rect, BorderSide sides, int indent) {
			rect = Rectangle.FromLTRB(
				rect.Left + (((sides & BorderSide.Left) == BorderSide.Left) ? indent : 0),
				rect.Top + (((sides & BorderSide.Top) == BorderSide.Top) ? indent : 0),
				rect.Right - (((sides & BorderSide.Right) == BorderSide.Right) ? indent : 0),
				rect.Bottom - (((sides & BorderSide.Bottom) == BorderSide.Bottom) ? indent : 0));
			return rect;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r = TransformRectangle(r, BorderSides, -1);
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 16, 16);
		}
		protected virtual void DrawBorder(ObjectInfoArgs e) {
			if(BorderSides == BorderSide.None) return;
			Rectangle bounds = e.Bounds;
			Color clr = GetStyle(e).BorderColor;
			if(clr == Color.Empty) clr = Office2003Colors.Default[Office2003Color.NavPaneBorderColor];
			Brush brush = e.Cache.GetSolidBrush(clr);
			if((BorderSides & BorderSide.Top) == BorderSide.Top) {
				e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(bounds.Left, bounds.Top, bounds.Width, 1));
			}
			if((BorderSides & BorderSide.Left) == BorderSide.Left) {
				e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(bounds.Left, bounds.Top, 1, bounds.Height));
			}
			if((BorderSides & BorderSide.Bottom) == BorderSide.Bottom) {
				e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(bounds.Left, bounds.Bottom - 1, bounds.Width, 1));
			}
			if((BorderSides & BorderSide.Right) == BorderSide.Right) {
				e.Cache.Paint.FillRectangle(e.Graphics, brush, new Rectangle(bounds.Right - 1, bounds.Top, 1, bounds.Height));
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectState state = e.State;
			if((state & ObjectState.Selected) != 0) {
				if(state != ObjectState.Selected) state = ObjectState.Pressed;
			}
			if(state != ObjectState.Normal || AllowDefaultFill) DrawBorder(e);
			Rectangle bounds = GetObjectClientRectangle(e);
			AppearanceObject appearance = GetStyle(e); 
			Brush backBrush;
			if(e.State == (ObjectState.Selected | ObjectState.Hot))
				appearance = NavBar.PaintAppearance.GroupHeaderPressed;
			backBrush = appearance.GetBackBrush(e.Cache, bounds);
			if(state == ObjectState.Normal && !AllowDefaultFill) backBrush = null;
			if(backBrush != null) e.Cache.Paint.FillRectangle(e.Graphics, backBrush, bounds);
		}
		protected virtual Brush GetDefaultBackBrush(ObjectInfoArgs e, ObjectState state, Rectangle bounds) {
			Color c1, c2;
			c1 = Office2003Colors.Default[Office2003Color.Button1];
			c2 = Office2003Colors.Default[Office2003Color.Button2];
			switch(state) {
				case ObjectState.Hot : 
					c1 = Office2003Colors.Default[Office2003Color.Button1Hot];
					c2 = Office2003Colors.Default[Office2003Color.Button2Hot];
					break;
				case ObjectState.Pressed :
					c1 = Office2003Colors.Default[Office2003Color.Button1Pressed];
					c2 = Office2003Colors.Default[Office2003Color.Button2Pressed];
					break;
				case ObjectState.Selected :
					c1 = Office2003Colors.Default[Office2003Color.Button2Pressed];
					c2 = Office2003Colors.Default[Office2003Color.Button1Pressed];
					break;
				case ObjectState.Normal :
					if(!AllowDefaultFill) return null;
					break;
			}
			return e.Cache.GetGradientBrush(bounds, c1, c2, LinearGradientMode.Vertical);
		}
	}
	public class NavigationPaneGroupClientPainter : BaseNavGroupClientPainter {
		public NavigationPaneGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
			base.DrawCaption(e, caption, font, brush, bounds, format);
		}
	}
	public class NavigationPaneGroupPainter : BaseNavigationPaneGroupPainter {
		ButtonObjectPainter upDownButtonPainter;
		public NavigationPaneGroupPainter(NavBarControl navBar) : base(navBar) {
			this.upDownButtonPainter = new NavigationPaneButtonPainter(navBar, BorderSide.All, true);
		}
		public override ObjectPainter UpDownButtonPainter { get { return upDownButtonPainter; } }
	}
	public class BaseNavigationPaneGroupPainter : BaseNavGroupPainter {
		public BaseNavigationPaneGroupPainter(NavBarControl navBar) : base(navBar) { }
		protected override ObjectPainter CreateButtonPainter() { return new NavigationPaneButtonPainter(NavBar); }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new NavigationPaneGroupClientPainter(this); 
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			NavBarGroup group = GetGroup(e);
			DrawButton(e, custom);
			DrawGroupImage(e, custom);
			if(group.NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed) return;
			if (e.Group.GetAllowHtmlString()) {
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else
				custom.Appearance.DrawString(e.Cache, custom.Caption, e.CaptionBounds);
		}
		protected override void DrawGroupImage(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			if(!ShouldDrawGroupImage)
				return;
			base.DrawGroupImage(e, custom);
		}
		protected virtual void DrawButton(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			ButtonPainter.DrawObject(GetButtonArgs(e, e.Bounds, custom.Appearance, e.State));
		}
		protected virtual int TextHeightAdd { get { return 6; } }
		protected virtual int TextHeightAddExtra { get { return 4; } }
		protected virtual int TextHeightMin { get { return 20; } }
		protected virtual int HorzIndent { get { return 6; } }
		protected virtual bool ShouldDrawGroupImage {
			get { return NavPaneViewInfo == null || NavPaneViewInfo.ShouldDrawGroupImage; }
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			Rectangle savedBounds = e.Bounds;
			Rectangle r = e.Bounds;
			try {
				e.Bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 50);
				CalcObjectBounds(e);
				int h = Math.Max(CalcTextSize(e, groupInfo.CaptionBounds.Width).Height + TextHeightAdd, groupInfo.ImageBounds.Height + TextHeightAdd) + TextHeightAddExtra;
				h += HorzIndent;
				r.Height = Math.Max(TextHeightMin, h);
				e.Bounds = r;
				r = ButtonPainter.CalcBoundsByClientRectangle(e);
			}
			finally {
				e.Bounds = savedBounds;
			}
			return r;
		}
		protected override Rectangle CalcGroupCaptionImageBounds(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			Rectangle rect = base.CalcGroupCaptionImageBounds(e, custom);
			if(!rect.IsEmpty) rect.Offset(HorzIndent, 0);
			if(ShouldDrawGroupImage) return rect;
			return new Rectangle(0, 0, 0, rect.Height);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			groupInfo.CaptionClientBounds = GetObjectClientRectangle(groupInfo);
			groupInfo.ImageBounds = CalcGroupCaptionImageBounds(groupInfo, null); 
			groupInfo.CaptionBounds = CalcGroupCaptionBounds(groupInfo, groupInfo.ImageBounds);
			CheckGroupBounds(groupInfo, e.Bounds);
			return e.Bounds;
		}
		protected void CheckGroupBounds(NavGroupInfoArgs groupInfo, Rectangle contentBounds) {
			groupInfo.CaptionClientBounds = CheckBounds(groupInfo.CaptionClientBounds);
			groupInfo.ImageBounds = CheckBounds(groupInfo.ImageBounds);
			groupInfo.CaptionBounds = CheckBounds(groupInfo.CaptionBounds);
		}
		public virtual int CalcCollapsedGroupWidth(ObjectInfoArgs e) {
			CalcObjectBounds(e);
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			if(groupInfo.ImageBounds.Width == 0) return groupInfo.Bounds.Height;
			int x = IsRightToLeft ? groupInfo.Bounds.Width - groupInfo.ImageBounds.Right : groupInfo.ImageBounds.X;
			if(x < 0) x = HorzIndent;
			return groupInfo.ImageBounds.Width + x * 2;
		}
		protected Rectangle CalcGroupCaptionBounds(NavGroupInfoArgs e, Rectangle imageBounds) {
			Rectangle caption = e.CaptionClientBounds;
			caption.Inflate(0, -(HorzIndent / 2));
			int right = caption.Right;
			if(imageBounds.IsEmpty || !ShouldDrawGroupImage) 
				caption.X += HorzIndent;
			else 
				caption.X = imageBounds.Right + HorzIndent;
			int width = right - caption.X;
			e.CaptionMaxWidth = width;
			caption.Width = CalcTextSize(e, width).Width;
			caption = NavBarViewInfo.CheckElementCaptionLocation(e.PaintAppearance, caption, width);
			return caption;
		}
		protected NavigationPaneViewInfo NavPaneViewInfo { get { return NavBar.ViewInfo as NavigationPaneViewInfo; } }
	}
	public class NavBarNavigationPaneHitInfo : NavBarHitInfo {
		int overflowPanelButtonIndex = -1;
		NavigationPaneOverflowPanelObjectInfo overflowPanelButton;
		public NavBarNavigationPaneHitInfo(NavBarControl navBar)
			: base(navBar) {
		}
		protected override NavBarHitInfo CreateHitInfo() { return new NavBarNavigationPaneHitInfo(NavBar); }
		public override void Clear() {
			this.overflowPanelButton = null;
			this.overflowPanelButtonIndex = -1;
			base.Clear();
		}
		protected new NavigationPaneViewInfo ViewInfo { get { return base.ViewInfo as NavigationPaneViewInfo; } }
		protected internal NavigationPaneOverflowPanelObjectInfo OverflowPanelButton { get { return overflowPanelButton; } }
		public int OverflowPanelButtonIndex { get { return overflowPanelButtonIndex; } }
		public override void CalcHitInfo(Point p, NavBarHitTest[] validLinkHotTracks) {
			base.CalcHitInfo(p, validLinkHotTracks);
			if(CheckAndSetHitTest(ViewInfo.SplitterBounds, p, NavBarHitTest.NavigationPaneSplitter)) return;
			if(ViewInfo.HeaderInfo == null) return;
			if(CheckAndSetHitTest(ViewInfo.HeaderInfo.ButtonBounds, p, NavBarHitTest.ExpandButton)) {
				ExpandButtonBounds = ViewInfo.HeaderInfo.ButtonBounds;
				return;
			}
			if(CheckAndSetHitTest(ViewInfo.HeaderBounds, p, NavBarHitTest.NavigationPaneHeader)) return;
			if(CheckAndSetHitTest(ViewInfo.OverflowBounds, p, NavBarHitTest.NavigationPaneOverflowPanel)) {
				for(int n = 0; n < ViewInfo.OverflowInfo.Buttons.Count; n++) {
					NavigationPaneOverflowPanelObjectInfo info = ViewInfo.OverflowInfo.Buttons[n] as NavigationPaneOverflowPanelObjectInfo;
					if(CheckAndSetHitTest(info.Bounds, p, NavBarHitTest.NavigationPaneOverflowPanelButton)) {
						this.overflowPanelButton = info;
						this.overflowPanelButtonIndex = n;
						return;
					}
				}
				return;
			}
			if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed && CheckAndSetHitTest(ViewInfo.ContentButton, p, NavBarHitTest.ContentButton)) {
			}
		}
		public NavBarGroup OverlowPanelGroup {
			get { return OverflowPanelButton != null ? OverflowPanelButton.Group : null; }
		}
		public override bool IsEquals(object obj) {
			NavBarNavigationPaneHitInfo hi = obj as NavBarNavigationPaneHitInfo;
			if(hi == null) return false;
			return base.IsEquals(hi) && this.OverflowPanelButtonIndex == hi.OverflowPanelButtonIndex;
		}
	}
	public static class NavBarPaintHelper {
		public static Image GetRTLImage(Image image) {
			if(image == null) return null;
			Image i = image.Clone() as Image;
			i.RotateFlip(RotateFlipType.RotateNoneFlipX);
			return i;
		}
	}
}
