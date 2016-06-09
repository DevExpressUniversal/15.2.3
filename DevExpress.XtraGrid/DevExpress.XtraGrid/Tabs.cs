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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraGrid.Tab {
	public class ViewTabPage : IXtraTabPage {
		ViewTab tabControl;
		string text;
		object tag;
		GridDetailInfo detailInfo;
		public ViewTabPage(ViewTab tabControl) {
			this.text = "page";
			this.tag = null;
			this.detailInfo = null;
			this.tabControl = tabControl;
		}
		public virtual GridDetailInfo DetailInfo { get { return detailInfo; } set { detailInfo = value; } }
		public virtual object Tag { get { return tag; } set { tag = value; } }
		public IXtraTab TabControl { get { return tabControl; } }
		DefaultBoolean IXtraTabPage.AllowGlyphSkinning { get { return  DefaultBoolean.Default; } }
		Image IXtraTabPage.Image { get { return null; } }
		int IXtraTabPage.ImageIndex { get { return -1; } }
		public virtual string Text { get { return text; } set { text = value; } }
		int IXtraTabPage.TabPageWidth { get { return 0; } }
		bool IXtraTabPage.PageEnabled { get { return true; } }
		bool IXtraTabPage.PageVisible { get { return true; } }
		void IXtraTabPage.Invalidate() {
			TabControl.Invalidate(TabControl.Bounds);
		}
		PageAppearance appearance;
		PageAppearance IXtraTabPage.Appearance {
			get {
				if(appearance == null) {
					appearance = new PageAppearance();
					appearance.Header.TextOptions.HotkeyPrefix = HKeyPrefix.None;
				}
				return appearance; 
			}
		}
		string IXtraTabPage.Tooltip { get { return string.Empty; } }
		string IXtraTabPage.TooltipTitle { get { return string.Empty; } }
		ToolTipIconType IXtraTabPage.TooltipIconType { get { return ToolTipIconType.None; } }
		SuperToolTip IXtraTabPage.SuperTip { get { return null; } }
		DefaultBoolean IXtraTabPage.ShowCloseButton { get { return DefaultBoolean.Default; } }
		Padding IXtraTabPage.ImagePadding { get { return new Padding(0); } }
	}
	[ListBindable(false)]
	public class ViewTabPageCollection : CollectionBase {
		public ViewTabPage this[int index] { get { return List[index] as ViewTabPage; } }
		public int Add(ViewTabPage page) {
			return List.Add(page);
		}
	}
	public class ViewTab : IXtraTab, IDisposable {
		bool populating;
		Rectangle bounds, cachedBounds, pageClientCachedBounds;
		BaseViewInfoRegistrator view;
		BaseTabControlViewInfo viewInfo;
		BaseTabPainter painter;
		BaseTabHandler handler;
		ViewTabPageCollection pages;
		BaseView ownerView;
		TabHeaderLocation headerLocation; 
		TabOrientation headerOrientation;
		public ViewTab(BaseView ownerView) {
			this.populating = false;
			this.ownerView = ownerView;
			this.pages = new ViewTabPageCollection();
			this.headerLocation = TabHeaderLocation.Top;
			this.headerOrientation = TabOrientation.Horizontal;
			this.pageClientCachedBounds = this.cachedBounds = this.bounds = Rectangle.Empty;
			CreateView();
		}
		public UserLookAndFeel LookAndFeel { get { return OwnerView == null ? UserLookAndFeel.Default : OwnerView.ElementsLookAndFeel; } }
		public virtual void Dispose() {
			this.ownerView = null;
			if(this.viewInfo != null) this.viewInfo.Dispose();
			this.viewInfo = null;
		}
		public virtual ViewTabPage SelectedPage {
			get { return ViewInfo.SelectedTabPage as ViewTabPage; }
			set {
				ViewInfo.SelectedTabPage = value;
			}
		}
		public virtual void Populate() {
			this.populating = true;
			try {
				Pages.Clear();
				ViewInfo.SetSelectedTabPageCore(null);
				OwnerView.PopulateTab();
			}
			finally {
				this.populating = false;
			}
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
			if(bounds == cachedBounds) return pageClientCachedBounds;
			this.cachedBounds = bounds;
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
			return DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.GetView(OwnerView.PaintStyle == null ? null : OwnerView.ElementsLookAndFeel, BaseViewInfoRegistrator.DefaultViewName);
		}
		protected virtual void CreateView() {
			this.view = GetView();
			IXtraTabPage prevPage = null;
			if(ViewInfo != null) {
				prevPage = ViewInfo.SelectedTabPage;
				ViewInfo.SelectedPageChanged -= new ViewInfoTabPageChangedEventHandler(OnSelectedPageChanged);
				ViewInfo.SelectedPageChanging -= new ViewInfoTabPageChangingEventHandler(OnSelectedPageChanging);
				ViewInfo.Dispose();
			}
			this.viewInfo = View.CreateViewInfo(this);
			this.painter = View.CreatePainter(this);
			this.handler = View.CreateHandler(this);
			UpdateViewInfo();
			if(ViewInfo != null) {
				if(prevPage != null) ViewInfo.SetSelectedTabPageCore(prevPage);
				ViewInfo.SelectedPageChanged += new ViewInfoTabPageChangedEventHandler(OnSelectedPageChanged);
				ViewInfo.SelectedPageChanging += new ViewInfoTabPageChangingEventHandler(OnSelectedPageChanging);
			}
		}
		protected virtual void UpdateViewInfo() {
			ViewInfo.FillPageClient = false;
		}
		protected virtual void OnSelectedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			if(populating) return;
			OwnerView.OnTabSelectedPageChanged(this, e);
		}
		protected virtual void OnSelectedPageChanging(object sender, ViewInfoTabPageChangingEventArgs e) {
			if(populating) return;
			OwnerView.OnTabSelectedPageChanging(this, e);
		}
		public virtual TabHeaderLocation HeaderLocation { 
			get { return headerLocation; }
			set {
				if(HeaderLocation == value) return;
				headerLocation = value;
				LayoutChanged();
			}
		}
		public virtual GridControl GridControl { get { return OwnerView == null ? null : OwnerView.GridControl; } }
		public virtual ViewTabPageCollection Pages { get { return pages; } }
		public BaseView OwnerView { get { return ownerView; } }
		protected virtual BaseViewInfoRegistrator View { get { return view; } }
		bool IXtraTab.RightToLeftLayout { get { return OwnerView != null && OwnerView.IsRightToLeft; } }
		int IXtraTab.PageCount { get { return Pages.Count; } }
		IXtraTabPage IXtraTab.GetTabPage(int index) { return Pages[index]; }
		BaseViewInfoRegistrator IXtraTab.View { get { return View; } }
		Rectangle IXtraTab.Bounds { get { return Bounds; } }
		TabHeaderLocation IXtraTab.HeaderLocation { get { return headerLocation; } }
		TabOrientation IXtraTab.HeaderOrientation { get { return headerOrientation; } }
		object IXtraTab.Images { get { return null; } }
		void IXtraTab.OnPageChanged(IXtraTabPage page) { 
			if(ViewInfo != null) ViewInfo.OnPageChanged(page);
			LayoutChanged();
		}
		BaseTabHitInfo IXtraTab.CreateHitInfo() { return new BaseTabHitInfo(); }
		BaseTabControlViewInfo IXtraTab.ViewInfo { 	get { return ViewInfo; } }
		Control IXtraTab.OwnerControl { get { return GridControl; } }
		public virtual void Invalidate(Rectangle rect) {
			if(GridControl != null) GridControl.Invalidate(rect);
		}
		public virtual void LayoutChanged() {
			ViewInfo.LayoutChanged();
		}
		public virtual void Draw(DXPaintEventArgs e, GraphicsCache cache) {
			TabDrawArgs dr = new TabDrawArgs(cache, ViewInfo, Bounds);
			Painter.Draw(dr);
		}
		protected internal virtual BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
		protected virtual BaseTabPainter Painter { get { return painter; } }
		public virtual BaseTabHandler Handler { get { return handler; } }
		Point IXtraTab.ScreenPointToControl(Point point) {
			return ((IXtraTab)this).OwnerControl.PointToClient(point);
		}
	}
}
