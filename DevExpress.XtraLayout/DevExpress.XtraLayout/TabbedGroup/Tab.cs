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
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using System.Drawing.Text;
namespace DevExpress.XtraLayout.Tab {
	public class TabObjectInfo : ObjectInfoArgs {
		LayoutTab tab;
		public TabObjectInfo(TabbedGroup group) {
			this.tab = new LayoutTab(group);
		}
		public LayoutTab Tab { get { return tab; } }
	}
	public class TabObjectPainter : ObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			if(e != null) {
				TabObjectInfo vi = e as TabObjectInfo;
				return vi.Tab.CalcPageClient(e.Bounds);
			}
			else
				throw new NullReferenceException("error in parameters");
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(e != null) {
				TabObjectInfo vi = e as TabObjectInfo;
				vi.Tab.Draw(new DXPaintEventArgs(e.Graphics, e.Cache.PaintArgs.ClipRectangle));
			}
			else
				throw new NullReferenceException("e is null");
		}
	}
	public class LayoutTabPage :IXtraTabPageExt {
		LayoutTab tabControl;
		string text;
		LayoutGroup group;
		public LayoutTabPage(LayoutTab tabControl, LayoutGroup group) {
			this.group = group;
			this.text = Group.TextVisible ? Group.Text : String.Empty;
			this.tabControl = tabControl;
		}
		public LayoutGroup Group { get { return group; } }
		public IXtraTab TabControl { get { return tabControl; } }
		DefaultBoolean IXtraTabPage.AllowGlyphSkinning {
			get { return group.GetAllowGlyphSkinning() ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		Image IXtraTabPage.Image {
			get { return (group.CaptionImage != null && group.CaptionImageVisible) ? group.CaptionImage : null; }
		}
		int IXtraTabPage.ImageIndex { get { return -1; } }
		int IXtraTabPage.TabPageWidth { get { return group.TabPageWidth == 0 ? (tabControl as IXtraTabProperties).TabPageWidth : group.TabPageWidth; } }
		string IXtraTabPage.Text { get { return text; } }
		bool IXtraTabPage.PageEnabled {
			get { return (group.Owner != null && !group.Owner.EnableCustomizationMode) ? group.PageEnabled : true; }
		}
		bool IXtraTabPage.PageVisible { get { return true; } }
		void IXtraTabPage.Invalidate() {
			TabControl.Invalidate(TabControl.Bounds);
		}
		PageAppearance IXtraTabPage.Appearance {
			get { return Group.PaintAppearanceGroup; }
		}
		string IXtraTabPage.Tooltip { get { return string.Empty; } }
		string IXtraTabPage.TooltipTitle { get { return string.Empty; } }
		ToolTipIconType IXtraTabPage.TooltipIconType { get { return ToolTipIconType.None; } }
		SuperToolTip IXtraTabPage.SuperTip { get { return null; } }
		DefaultBoolean IXtraTabPage.ShowCloseButton { get { return FromBoolToDefaultBoolean(group.ShowTabPageCloseButton); } }
		protected DefaultBoolean FromBoolToDefaultBoolean(bool val) {
			return val ? DefaultBoolean.True : DefaultBoolean.False;
		}
		Padding IXtraTabPage.ImagePadding {
			get {
				if(group.ParentTabbedGroup != null && group.IsDefaultCaptionImagePadding()) {
					return GetPadding(group.ParentTabbedGroup.CaptionImagePadding);
				}
				return GetPadding(group.CaptionImagePadding);
			}
		}
		public HotkeyPrefix HotkeyPrefixOverride { get { return HotkeyPrefix.Show; } }
		public int MaxTabPageWidth { get { return 0; } }
		public bool Pinned { get { return false; } set { } }
		public bool UsePinnedTab { get { return false; } }
		public DefaultBoolean ShowPinButton { get { return DefaultBoolean.False; } }
		static Padding GetPadding(XtraLayout.Utils.Padding padding) {
			return new Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}
	}
	[ListBindable(false)]
	public class LayoutTabPageCollection : CollectionBase {
		public LayoutTabPage this[int index] { get { return List[index] as LayoutTabPage; } }
		public void Insert(int insertIndex, LayoutTabPage page) {
			this.InnerList.Insert(insertIndex, page);	 
		}
		public int Add(LayoutTabPage page) {
			return List.Add(page);
		}
	}
	public class LayoutTab : IXtraTab, IXtraTabProperties, IDisposable {
		bool populating;
		BaseViewInfoRegistrator view;
		Rectangle bounds, cachedBounds, pageClientCachedBounds;
		int cachedPageCount = 0;
		BaseTabControlViewInfo viewInfo;
		BaseTabPainter painter;
		BaseTabHandler handler;
		TabHeaderLocation headerLocation;
		TabOrientation headerOrientation;
		TabbedGroup owner;
		LayoutTabPageCollection pages;
		public event EventHandler SelectedPageChanged;
		public event EventHandler CloseButtonClick;
		public LayoutTab(TabbedGroup owner) {
			this.owner = owner;
			this.populating = false;
			this.pages = new LayoutTabPageCollection();
			this.pageClientCachedBounds = this.cachedBounds = this.bounds = Rectangle.Empty;
			CreateView();
			Populate();
		}
		public TabbedGroup Owner { get { return owner; } }
		public UserLookAndFeel LookAndFeel { get { return Owner.PaintStyle.LookAndFeel; } }
		public virtual void Dispose() {
			if(this.viewInfo != null) this.viewInfo.Dispose();
			this.viewInfo = null;
		}
		public virtual LayoutTabPage SelectedPage {
			get { return ViewInfo.SelectedTabPage as LayoutTabPage; }
			set {
				ViewInfo.SelectedTabPage = value;
			}
		}
		public virtual void Populate() {
				PopulateFromOwnerTabs(); 
		}
		public LayoutTabPage GetPageByGroup(LayoutGroup group) {
			foreach(LayoutTabPage page in Pages) {
				if(page.Group == group) return page;
			}
			return null;
		}
		public virtual void PopulateFromOwnerTabs() {
			this.populating = true;
			ViewInfo.BeginUpdate();
			try {
				Pages.Clear();
				LayoutTabPage activePage = null;
				foreach(LayoutGroup group in Owner.VisibleTabPages) {
					LayoutTabPage page = new LayoutTabPage(this, group);
					if(group == Owner.SelectedTabPage) activePage = page;
					Pages.Add(page);
				}
				if(activePage != null) ViewInfo.SelectedTabPage = activePage;
			}
			finally {
				this.populating = false;
			}
			ViewInfo.EndUpdate();
		}
		public virtual Rectangle Bounds { 
			get { return bounds; } 
			set { 
				if(Bounds == value) return;
				bounds = value; 
				ViewInfo.Resize();
			}
		}
		public virtual Rectangle CalcPageClient(Rectangle bounds) {
			if(bounds == cachedBounds && Pages.Count == cachedPageCount) return pageClientCachedBounds;
			this.cachedBounds = bounds;
			this.cachedPageCount = Pages.Count;
			BaseTabControlViewInfo vInfo = View.CreateViewInfo(this);
			vInfo.CalcViewInfo(bounds, null);
			this.pageClientCachedBounds = vInfo.PageClientBounds;
			vInfo.Dispose();
			return pageClientCachedBounds;
		}
		public virtual void CheckInfo() {
			if(View != GetView()) CreateView();
		}
		protected virtual BaseViewInfoRegistrator GetView() { 
			return Owner.PaintStyle.CreateTabInfo();
		}
		protected virtual void CreateView() {
			this.view = GetView();
			IXtraTabPage prevPage = null;
			if(ViewInfo != null) {
				prevPage = ViewInfo.SelectedTabPage;
				ViewInfo.SelectedPageChanged -= new ViewInfoTabPageChangedEventHandler(OnSelectedPageChanged);
				ViewInfo.CloseButtonClick -= new EventHandler(OnCloseButtonClick);
				ViewInfo.Dispose();
			}
			this.viewInfo = View.CreateViewInfo(this);
			this.painter =  View.CreatePainter(this);
			this.handler = View.CreateHandler(this);
			UpdateViewInfo();
			if(ViewInfo != null) {
				if(prevPage != null) ViewInfo.SetSelectedTabPageCore(prevPage);
				ViewInfo.SelectedPageChanged += new ViewInfoTabPageChangedEventHandler(OnSelectedPageChanged);
				ViewInfo.CloseButtonClick += new EventHandler(OnCloseButtonClick);
			}
		}
		protected virtual void UpdateViewInfo() {
			ViewInfo.FillPageClient = true;
		}
		protected virtual void OnCloseButtonClick(object sender, EventArgs e) {
			if (populating) return;
			if (CloseButtonClick != null) CloseButtonClick(sender, e);
		}
		protected virtual void OnSelectedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			if (populating) return;
			if (SelectedPageChanged != null) SelectedPageChanged(this, EventArgs.Empty);
		}
		public virtual LayoutTabPageCollection Pages { get { return pages; } }
		protected virtual BaseViewInfoRegistrator View { get { return view; } }
		int IXtraTabProperties.TabPageWidth { get { return Owner.TabPageWidth; } }
		DefaultBoolean IXtraTabProperties.AllowHotTrack { get { return DefaultBoolean.Default; } }
		DefaultBoolean IXtraTabProperties.AllowGlyphSkinning { get { return DefaultBoolean.Default; } }
		DefaultBoolean IXtraTabProperties.ShowTabHeader { get { return Owner.ShowTabHeader; } }
		DefaultBoolean IXtraTabProperties.ShowToolTips { get { return DefaultBoolean.Default; } }
		DefaultBoolean IXtraTabProperties.MultiLine { get { return Owner.MultiLine; } }
		DefaultBoolean IXtraTabProperties.HeaderAutoFill { get { return Owner.HeaderAutoFill; } }
		DefaultBoolean IXtraTabProperties.ShowHeaderFocus { get { return DefaultBoolean.Default; } }
		TabPageImagePosition IXtraTabProperties.PageImagePosition { get { return Owner.PageImagePosition; } }
		AppearanceObject IXtraTabProperties.Appearance { get { return Owner.PaintAppearanceGroup.AppearanceGroup; } }
		PageAppearance IXtraTabProperties.AppearancePage { get { return Owner.PaintAppearanceGroup; } }
		BorderStyles IXtraTabProperties.BorderStyle { get { return BorderStyles.Default; } }
		BorderStyles IXtraTabProperties.BorderStylePage { get { return BorderStyles.Default; } }
		TabButtonShowMode IXtraTabProperties.HeaderButtonsShowMode { get { return TabButtonShowMode.WhenNeeded; } }
		TabButtons IXtraTabProperties.HeaderButtons { get { return TabButtons.Default; } }
		ClosePageButtonShowMode IXtraTabProperties.ClosePageButtonShowMode { get { return ClosePageButtonShowMode.InAllTabPageHeaders; } }
		PinPageButtonShowMode IXtraTabProperties.PinPageButtonShowMode { get { return PinPageButtonShowMode.Default; } }
		bool IXtraTab.RightToLeftLayout {
			get {
				return Owner.IsRTL;
			}
		}
		int IXtraTab.PageCount { get { return Pages.Count; } }
		IXtraTabPage IXtraTab.GetTabPage(int index) { return Pages[index]; }
		BaseViewInfoRegistrator IXtraTab.View { get { return View; } }
		Rectangle IXtraTab.Bounds { get { return Bounds; } }
		protected internal void SetHeaderLocationAndOrientation(Locations location, TabOrientation orientation) {
			switch(location) {
				case Locations.Left:
					headerLocation = TabHeaderLocation.Left;
					headerOrientation  = TabOrientation.Vertical;
					break;
				case Locations.Right:
					headerLocation = TabHeaderLocation.Right;
					headerOrientation  = TabOrientation.Vertical;
					break;
				case Locations.Top:
				case Locations.Default:
					headerLocation = TabHeaderLocation.Top;
					headerOrientation  = TabOrientation.Horizontal;
					break;
				case Locations.Bottom:
					headerLocation = TabHeaderLocation.Bottom;
					headerOrientation  = TabOrientation.Horizontal;
					break;
			}
			if(orientation != TabOrientation.Default) headerOrientation = orientation;
			this.cachedBounds = Rectangle.Empty;
		}
		Point IXtraTab.ScreenPointToControl(Point point) { return Point.Empty; }
		TabHeaderLocation IXtraTab.HeaderLocation { get { return headerLocation; } }
		TabOrientation IXtraTab.HeaderOrientation { get { return headerOrientation; } }
		object IXtraTab.Images { get { return null; } }
		void IXtraTab.OnPageChanged(IXtraTabPage page) { 
			if(ViewInfo != null) ViewInfo.OnPageChanged(page);
			LayoutChanged();
		}
		BaseTabHitInfo IXtraTab.CreateHitInfo() { return new BaseTabHitInfo(); }
		BaseTabControlViewInfo IXtraTab.ViewInfo { 	get { return ViewInfo; } }
		Control IXtraTab.OwnerControl { 
			get {
				if(Owner.Owner!=null) return Owner.Owner.Control;
				else return null;
			} 
		} 
		void IXtraTab.Invalidate(Rectangle rect) {
			Invalidate(rect); 
		}
		public virtual void Invalidate(Rectangle rect) {
			if(rect != Rectangle.Empty && 
				((IXtraTab)this).OwnerControl != null)
				((IXtraTab)this).OwnerControl.Invalidate(rect);
		}
		public virtual void LayoutChanged() {
			ViewInfo.LayoutChanged();
		}
		public virtual void Draw(DXPaintEventArgs e) {
			TabDrawArgs dr = new TabDrawArgs(new GraphicsCache(e), ViewInfo, Bounds);
			Painter.Draw(dr);
			dr.Cache.Dispose();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
		protected virtual BaseTabPainter Painter { get { return painter; } }
		public virtual BaseTabHandler Handler { get { return handler; } }
	}
}
