#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Layout;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.XtraBars;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.HitInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		readonly DashboardViewerCommandBarItemsContainer barItemsContainer = new DashboardViewerCommandBarItemsContainer();
		DashboardPopupMenu viewerPopupMenu;
		internal DashboardViewerCommandBarItemsContainer BarItemsContainer { get { return barItemsContainer; } }
		internal IDashboardLayoutControlItem SelectedLayoutItem {
			get {
				IDashboardLayoutSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
				return selectionService.SelectedItem;
			}
			set {
				IDashboardLayoutSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
				selectionService.SelectedItem = value;
			}
		}
		DashboardPopupMenu DesignerPopupMenu {
			get {
				if(designerPopupMenu == null) {
					if(designer != null) {
						designerPopupMenu = designer.PopupMenu;
						if(designerPopupMenu != null)
							designerPopupMenu.CloseUp += OnPopupMenuCloseUp;
					}
				}
				return designerPopupMenu;
			}
		}
		IDashboardLayoutAccessService LayoutAccessService {
			get { return ServiceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>(); }
		}
		IDashboardLayoutUpdateService LayoutUpdateService {
			get { return ServiceProvider.RequestServiceStrictly<IDashboardLayoutUpdateService>(); }
		}
		public event DashboardPopupMenuShowingEventHandler PopupMenuShowing;
		void SubscribeLayoutControlEvents() {
			layoutControl.PopupMenuShowing += OnLayoutControlPopupMenuShowing;
			layoutControl.CustomContextMenu += OnLayoutControlCustomContextMenu;
			layoutControl.DashboardLayoutGetCaptionImageToolTip += OnLayoutControlDashboardLayoutGetCaptionImageToolTip;
			layoutControl.Paint += OnLayoutControlPaint;
		}
		void UnsubscribeLayoutControlEvents() {
			layoutControl.PopupMenuShowing -= OnLayoutControlPopupMenuShowing;
			layoutControl.CustomContextMenu -= OnLayoutControlCustomContextMenu;
			layoutControl.DashboardLayoutGetCaptionImageToolTip -= OnLayoutControlDashboardLayoutGetCaptionImageToolTip;
			layoutControl.Paint -= OnLayoutControlPaint;
		}		
		void OnPopupMenuCloseUp(object sender, EventArgs e) {
			barItemsContainer.Clear();
			if(designer == null)
				SelectedLayoutItem = null;
			viewerBarManager.Items.Clear();
		}
		void OnLayoutChanged(object sender, EventArgs e) {
			if (LayoutChanged != null)
				LayoutChanged(this, e);
		}
		void OnLayoutControlDashboardLayoutGetCaptionImageToolTip(object sender, DashboardCustomContextMenuEventArgs e) {
			if(e.LayoutGroupHitTypes == LayoutGroupHitTypes.CaptionImage) {
				IDashboardLayoutControlItem item = e.Item as IDashboardLayoutControlItem;
				if(item.ItemViewer.ViewModel.ShouldIgnoreUpdate)
					toolTipController.ShowHint(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerDataObsoleteMessage), e.Point);
				else
				toolTipController.ShowHint(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerDataReducedMessage), e.Point);
			} else
				toolTipController.HideHint();
		}
		void OnLayoutControlPaint(object sender, PaintEventArgs e) {
			Rectangle bounds = new Rectangle(0, 0, layoutControl.Width, layoutControl.Height);
			string message = null;
			int offset = 0;
			Dashboard dashboard = Dashboard;
			if(dashboard != null) {
				bool isDashboardEmpty = dashboard.Items.Count == 0 && dashboard.Groups.Count == 0;
				if(isDashboardEmpty) {
					if(IsDashboardVSDesignMode)
						message = "To add dashboard items to your dashboard, drag them from the toolbox.\n\rTo load the existing dashboard, select Load Dashboard in the DASHBOARD menu.";
					else {
						if(IsDesignMode && !IsVSDesignMode && (designer != null && !designer.IsVSDesignMode))
							message = DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerEmptyMessage);
					}
				}
			} else if(DesignMode) {
				offset = title.Visible ? title.Height : 0;
				if(DashboardSourceType != null)
					message = string.Format("The '{0}' class is used to initialize the dashboard.", DashboardSourceType.Name);
				else if(DashboardSourceUri != null && DashboardSourceUri.IsAbsoluteUri)
					message = string.Format("The '{0}' file is used to initialize the dashboard.", DashboardSourceUri.AbsolutePath);
				else if(DashboardSource != null && !object.Equals(string.Empty, DashboardSource))
					message = string.Format("A {0} string specifies an incorrect dashboard supplier for the Dashboard Viewer.", DashboardSource);
				else
					message = DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardViewerEmptyDesignerMessage);
			}
			if(message != null)
				WarningRenderer.MessageRenderer(e, bounds, message, LookAndFeel, 700, offset);
		}
		void OnLayoutControlCustomContextMenu(object sender, CustomContextMenuEventArgs e) {
			BaseLayoutItemHitInfo info = layoutControl.CalcHitInfo(e.Point);
			if(info != null) {
				IDashboardLayoutControlItem layoutItem = info.Item as IDashboardLayoutControlItem;
				if(layoutItem != null)
					ForceRaisePopupMenuShowing(layoutItem, layoutControl.PointToScreen(e.Point));
			}
		}
		void OnLayoutControlPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			e.Allow = false;
		}
		[System.Security.SecuritySafeCritical]
		void SuspendDrawing() {
			if (Parent != null)
				SendMessage(Parent.Handle, WM_SETREDRAW, new UIntPtr(0), IntPtr.Zero);
		}
		[System.Security.SecuritySafeCritical]
		void ResumeDrawing() {
			if (Parent != null)
				SendMessage(Parent.Handle, WM_SETREDRAW, new UIntPtr(1), IntPtr.Zero);
		}
		void EnlargeLayoutControl() {
			SuspendDrawing();
			RemoveControls();
			layoutControl.Size = new Size(2000, 2000);
		}
		void RestoreLayoutControl() {
			layoutControl.Dock = DockStyle.Fill;
			AddControls();
			ResumeDrawing();
		}
		void CreateItemLinks(PopupMenu popupMenu, IList<DashboardItemViewerPopupMenuCreator> creators, bool overAll) {
			int overAllIndex = 0;
			for(int i = 0; i < creators.Count; i++) {
				DashboardItemViewerPopupMenuCreator creator = creators[i];
				List<BarItem> barItems = creator.GetBarItems();
				if(creator.AllowViewerCommands)
					barItemsContainer.AddBarItems(barItems);
				if(i == 0)
					for(int z = 0; z < popupMenu.ItemLinks.Count; z++) {
						BarItemLink link = popupMenu.ItemLinks[z];
						if(link.Visible) {
							link.BeginGroup = true;
							break;
						}
					}
				for(int j = 0; j < barItems.Count; j++) {
					BarItemLink link = overAll ? popupMenu.ItemLinks.Insert(overAllIndex++, barItems[j]) : popupMenu.AddItem(barItems[j]);
					BarSubItemLink subLink = link as BarSubItemLink;
					if(subLink != null)
						SetBarManager(subLink);
					if(j == 0 && i != 0)
						link.BeginGroup = true;
				}
			}
		}
		void SetBarManager(BarSubItemLink subLink) {
			foreach(BarItemLink link in subLink.Item.ItemLinks)
				if(link.Item.Manager == null)
					viewerBarManager.Items.Add(link.Item);
		}
		void DisposeViewerPopupMenu() {
			if(viewerPopupMenu != null) {
				viewerPopupMenu.CloseUp -= OnPopupMenuCloseUp;
				viewerPopupMenu.Dispose();
				viewerPopupMenu = null;
			}
		}
		void UpdateLayoutControlByDashboardLayout(DashboardPane rootPane) {
			EnlargeLayoutControl();
			try {
				IDashboardPaneAdapter paneAdapter = ServiceProvider.RequestServiceStrictly<IDashboardPaneAdapter>();
				paneAdapter.SetRootPane(rootPane);
			}
			finally {
				RestoreLayoutControl();
			}
		}	   
		void RefreshLayoutControl(IList<DashboardPaneContent> paneContentList) {
			List<IDashboardLayoutControlItem> layoutItems = LayoutAccessService.VisibleLayoutControlItems.Where(item => !item.IsGroup).ToList();
			List<IDashboardLayoutControlItem> layoutGroups = LayoutAccessService.VisibleLayoutControlItems.Where(item => item.IsGroup).ToList();
			foreach (IDashboardLayoutControlItem layoutItem in layoutItems) {
				DashboardPaneContent itemContent = FindPaneContent(paneContentList, layoutItem);
				if (itemContent == null)
					LayoutUpdateService.RemoveLayoutItem(layoutItem);
				else {
					IDashboardLayoutControlItem parentGroup = layoutItem.ParentGroup;
					if (parentGroup != null) {
						DashboardPaneContent parentGroupContent = FindPaneContent(paneContentList, parentGroup);
						if (parentGroupContent == null)
							LayoutUpdateService.MoveLayoutItemToRoot(layoutItem);
					}
				}
			}
			foreach (IDashboardLayoutControlItem layoutGroup in layoutGroups) {
				DashboardPaneContent groupContent = FindPaneContent(paneContentList, layoutGroup);
				if(groupContent == null)
					LayoutUpdateService.RemoveLayoutItem(layoutGroup);
			}
			foreach (DashboardPaneContent paneContent in paneContentList) {
				IDashboardLayoutControlItem layoutItem = LayoutAccessService.FindLayoutItem(paneContent.Name);
				if(layoutItem == null)
					LayoutUpdateService.CreateLayoutItem(paneContent.Name, paneContent.Type);
			}
			foreach(DashboardPaneContent paneContent in paneContentList)
				RefreshDashboardItemViewer(paneContent);
		}
		DashboardPaneContent FindPaneContent(IList<DashboardPaneContent> paneContentList, IDashboardLayoutControlItem layoutItem) {
			return paneContentList.FirstOrDefault(content => {
				return content.Name == layoutItem.Name &&
					content.Type == layoutItem.Type;
			});
		}
		void SubscribeDashboardItemViewerEvents(DashboardItemViewer itemViewer) {
			itemViewer.ItemClick += OnDashboardItemClick;
			itemViewer.ItemDoubleClick += OnDashboardItemDoubleClick;
			itemViewer.ItemMouseMove += OnDashboardItemMouseMove;
			itemViewer.ItemMouseEnter += OnDashboardItemMouseEnter;
			itemViewer.ItemMouseLeave += OnDashboardItemMouseLeave;
			itemViewer.ItemMouseUp += OnDashboardItemMouseUp;
			itemViewer.ItemMouseDown += OnDashboardItemMouseDown;
			itemViewer.ItemMouseHover += OnDashboardItemMouseHover;
			itemViewer.ItemMouseWheel += OnDashboardItemMouseWheel;
			itemViewer.ControlUpdated += OnDashboardItemControlUpdated;
			itemViewer.ControlCreating += OnDashboardItemControlCreating;
			itemViewer.BeforeControlDisposed += OnDashboardItemBeforeControlDisposed;
			ISupportVisualInteractivity interactivity = itemViewer as ISupportVisualInteractivity;
			if(interactivity != null) {
				interactivity.SelectionChanged += OnDashboardItemSelectionChanged;
				interactivity.VisualInteractivity += OnDashboardItemVisualInteractivity;
			}
			ISupportColoring coloring = itemViewer as ISupportColoring;
			if(coloring != null) {
				coloring.ElementCustomColor += OnDashboardItemElementCustomColor;
			}
		}
		void RefreshDashboardItemViewer(DashboardPaneContent paneContent) {
			if (paneContent.ContentType == ContentType.Empty)
				return;
			DashboardItemViewer itemViewer = FindDashboardItemViewer(paneContent.Name);
			if (itemViewer != null)
				itemViewer.RefreshByPaneContent(paneContent);
		}
		void ClearLayout() {
			layoutControl.Clear(true, false);
		}
		void BeginUpdateLayout() {
			LayoutUpdateService.BeginUpdate();
			LayoutUpdateService.LockLayoutControlUpdate();
		}
		void EndUpdateLayout(bool performLayout) {
			LayoutUpdateService.UnlockLayoutControlUpdate(performLayout);
			LayoutUpdateService.EndUpdate();
		}
		internal void ForceRaisePopupMenuShowing(IDashboardLayoutControlItem layoutItem, Point point) {
			if(viewerPopupMenu != null && viewerPopupMenu.Opened) {
				viewerPopupMenu.HidePopup();
				DisposeViewerPopupMenu();
			}
			DashboardItemViewer viewer = layoutItem.ItemViewer;
			if(viewer == null)
				return;
			DashboardPopupMenu menu = DesignerPopupMenu;
			PopupMenuCreatorsData data = viewer.CreatePopupMenuCreatorsData(point);
			if(menu == null || data.UseViewerPopup) {
				viewerPopupMenu = new DashboardPopupMenu(viewerBarManager);
				viewerPopupMenu.CloseUp += OnPopupMenuCloseUp;
				menu = viewerPopupMenu;
			}
			barItemsContainer.Viewer = this;
			barItemsContainer.ItemViewer = viewer;
			CreateItemLinks(menu, data.ServiceCreators, true);
			CreateItemLinks(menu, data.Creators, false);
			DashboardPopupMenuShowingEventArgs args = new DashboardPopupMenuShowingEventArgs(viewer, viewer.PointToClient(point)) {
				Menu = menu,
				DashboardItemArea = data.DashboardItemArea,
				DashboardArea = DashboardArea.DashboardItem
			};
			RaisePopupMenuShowing(args);
			PopupMenu popupMenu = args.Menu;
			if(popupMenu != menu || !args.Allow)
				barItemsContainer.Clear();
			if (args.Allow && popupMenu != null && popupMenu.ItemLinks.Count > 0) {
				if (IsDesignMode) {
					IDashboardLayoutSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
					selectionService.SelectedItem = layoutItem;
				}
				IPopupMenuShowingService popupMenuService = ServiceProvider.RequestServiceStrictly<IPopupMenuShowingService>();
				popupMenuService.ShowPopupMenu(layoutItem, popupMenu, point);
			}
		}
		internal void RaisePopupMenuShowing(DashboardPopupMenuShowingEventArgs args) {
			if(designer != null)
				designer.RaisePopupMenuShowing(args);
			else if(PopupMenuShowing != null)
				PopupMenuShowing(this, args);
		}
		internal DashboardItemViewer CreateDashboardItemViewer(string name, string type, IDashboardLayoutControlItem layoutItem) {
			if (!string.IsNullOrEmpty(type)) {
				Type viewerType;
				if (DashboardItemViewer.Repository.TryGetValue(type, out viewerType)) {
					DashboardItemViewer itemViewer = (DashboardItemViewer)Activator.CreateInstance(viewerType);
					itemViewer.Initialize(name, this, layoutItem, layoutItem);
					layoutItem.ItemViewer = itemViewer;
					IDashboardItemDesignerFactory itemDesignerFactory = ServiceProvider.RequestService<IDashboardItemDesignerFactory>();
					if (itemDesignerFactory != null)
						layoutItem.ItemDesigner = itemDesignerFactory.CreateDashboardItemDesigner(itemViewer);
					SubscribeDashboardItemViewerEvents(itemViewer);
					itemViewer.InitializeViewControl();
					return itemViewer;
				}
			}
			return null;
		}
	}
}
