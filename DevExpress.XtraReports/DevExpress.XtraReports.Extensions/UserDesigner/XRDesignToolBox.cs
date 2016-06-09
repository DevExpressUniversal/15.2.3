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
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraNavBar;
using DevExpress.XtraReports.UserDesigner.Native;
using System.Drawing;
using DevExpress.XtraReports.Localization;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraReports.Design;
namespace DevExpress.XtraReports.UserDesigner {
	[
	ToolboxItem(false),
	Designer(typeof(System.Windows.Forms.Design.ControlDesigner)),
	]
	public class XRDesignToolBox : DevExpress.XtraNavBar.NavBarControl, IDesignControl {
		#region inner classes
		interface IMouseEventsFilter {
			void RefreshToolBox();
			void HandleDoubleClick();
			void HandleMouseMove();
		}
		class InactiveMouseEventsFilter : IMouseEventsFilter {
			protected XRDesignToolBox toolBox;
			public InactiveMouseEventsFilter(XRDesignToolBox toolBox) {
				this.toolBox = toolBox;
			}
			public virtual void RefreshToolBox() {
				toolBox.RefreshToolBox();
			}
			public virtual void HandleDoubleClick() {
			}
			public virtual void HandleMouseMove() {
			}
		}
		class ActiveMouseEventsFilter : InactiveMouseEventsFilter {
			public ActiveMouseEventsFilter(XRDesignToolBox toolBox)
				: base(toolBox) {
			}
			public override void RefreshToolBox() {
				base.RefreshToolBox();
			}
			public override void HandleDoubleClick() {
				toolBox.HandleDoubleClick();
			}
			public override void HandleMouseMove() {
				toolBox.HandleMouseMove();
			}
		}
		#endregion
		XRDesignPanel xrDesignPanel;
		NavBarGroupStyle groupsStyle = NavBarGroupStyle.SmallIconsText;
		IMouseEventsFilter mouseEventsFilter;
		Dictionary<NavBarItem, ToolboxItem> toolboxItems = new Dictionary<NavBarItem, ToolboxItem>(); 
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignToolBoxSmallImages"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new object SmallImages { get { return base.SmallImages; } set { base.SmallImages = value; } }
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignToolBoxXRDesignPanel"),
#endif
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get { return xrDesignPanel; }
			set {
				if(xrDesignPanel != null)
					UnSubscribeDesignPanelEvents();
				xrDesignPanel = value;
				if(xrDesignPanel != null)
					SubscribeDesignPanelEvents();
			}
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignToolBoxGroupsStyle")]
#endif
		public NavBarGroupStyle GroupsStyle {
			get { return groupsStyle; }
			set {
				if(groupsStyle == value)
					return;
				groupsStyle = value;
				foreach(NavBarGroup group in Groups) {
					group.GroupStyle = groupsStyle;
				}
			}
		}
		public XRDesignToolBox() {
			InitializeComponent();
			Deactivate();
			this.SelectedLinkChanged += new DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventHandler(OnSelectedLinkChanged);
			DisableNativeNavBarDrarDrop();
		}
		void DisableNativeNavBarDrarDrop() {
			DragDropFlags = NavBarDragDrop.None;
		}
		private XtraNavBar.NavBarItemLink PointerLink {
			get { return SelectedLink != null ? SelectedLink.Group.ItemLinks[0] : Items[0].Links[0]; }
		}
		DevExpress.XtraNavBar.NavBarItem pointerItem;
		NavBarItem PointerItem { get { return pointerItem; } }
		#region Component Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			AllowDrop = true;
			LinkSelectionMode = LinkSelectionModeType.OneInControl;
			HotTrackedItemCursor = System.Windows.Forms.Cursors.Arrow;
			ImageCollection imageCollection = new ImageCollection();
			imageCollection.ImageSize = new Size(24, 24);
			SmallImages = imageCollection;
			Appearance.ItemDisabled.ForeColor = SystemColors.Control;
			View = new DevExpress.XtraNavBar.ViewInfo.VSToolBoxViewInfoRegistrator();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
		}
		#endregion
		NavBarGroup AddGroup(string caption) {
			if(GetGroupByCaption(caption) == null) {
				NavBarGroup group = new NavBarGroup(caption);
				return Groups.Add(group);
			}
			return GetGroupByCaption(caption);
		}
		NavBarGroup GetGroupByCaption(string caption) {
			foreach(NavBarGroup group in Groups)
				if(group.Caption.Equals(caption))
					return group;
			return null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Deactivate();
				SelectedLinkChanged -= new DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventHandler(OnSelectedLinkChanged);
				if(xrDesignPanel != null)
					UnSubscribeDesignPanelEvents();
			}
			base.Dispose(disposing);
		}
		protected override void OnDoubleClick(System.EventArgs e) {
			base.OnDoubleClick(e);
			mouseEventsFilter.HandleDoubleClick();
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			mouseEventsFilter.HandleMouseMove();
		}
		void HandleDoubleClick() {
			ToolboxItem toolboxItem = GetToolboxItem(SelectedLink);
			xrDesignPanel.ToolPicked(toolboxItem);
		}
		void HandleMouseMove() {
			if(Control.MouseButtons.IsLeft()) {
				ToolboxItem toolboxItem = GetToolboxItem(SelectedLink);
				if(toolboxItem != null) {
					DoDragDrop(new DataObject(toolboxItem), DragDropEffects.Copy);
					SelectedLink = PointerLink;
					xrDesignPanel.SelectToolboxItem(GetToolboxItem(SelectedLink));
				}
			}
		}
#if DEBUGTEST
		public ToolboxItem Test_GetToolboxItem(NavBarItemLink link) {
			return GetToolboxItem(link);
		}
#endif
		ToolboxItem GetToolboxItem(NavBarItemLink link) {
			if(link == null || link.Item == null) return null;
			ToolboxItem item;
			return toolboxItems.TryGetValue(link.Item, out item) ? item : null;
		}
		void Activate() {
			mouseEventsFilter = new ActiveMouseEventsFilter(this);
		}
		void Deactivate() {
			mouseEventsFilter = new InactiveMouseEventsFilter(this);
		}
		void OnSelectedLinkChanged(object sender, DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventArgs e) {
			xrDesignPanel.SelectToolboxItem(GetToolboxItem(e.Link));
		}
		private void SubscribeDesignPanelEvents() {
			xrDesignPanel.SelectedToolboxItemUsed += new EventHandler(OnSelectedToolboxItemUsed);
			xrDesignPanel.Activated += new EventHandler(OnDesignerActivated);
			xrDesignPanel.Deactivated += new EventHandler(OnDesignerDeactivated);
		}
		private void UnSubscribeDesignPanelEvents() {
			xrDesignPanel.SelectedToolboxItemUsed -= new EventHandler(OnSelectedToolboxItemUsed);
			xrDesignPanel.Activated -= new EventHandler(OnDesignerActivated);
			xrDesignPanel.Deactivated -= new EventHandler(OnDesignerDeactivated);
		}
		private void OnSelectedToolboxItemUsed(object sender, EventArgs e) {
			SelectedLink = PointerLink;
		}
		private void OnDesignerActivated(object sender, EventArgs e) {
			mouseEventsFilter.RefreshToolBox();
			Activate();
		}
		private void OnDesignerDeactivated(object sender, EventArgs e) {
			ClearToolBox();
			Deactivate();
		}
		void ClearToolBox() {
			Items.Clear();
			Groups.Clear();
		}
		protected void RefreshToolBox() {
			BeginUpdate();
			ClearToolBox();
			InitPointerItem();
			toolboxItems.Clear();
			try {
				XRToolboxService toolboxService = (XRToolboxService)xrDesignPanel.GetService(typeof(IToolboxService));
				CategoryNameCollection nameCollection = toolboxService.CategoryNames;
				foreach(string name in nameCollection) {
					NavBarGroup group = AddGroup(name);
					CreatePointerNavBarItem(group);
					group.GroupStyle = groupsStyle;
					FillToolBoxGroup(group, toolboxService.GetToolboxItems(name));
				}
				ActiveGroup = GetGroupByCaption(toolboxService.DefaultCategoryName);
				ActiveGroup.Expanded = true;
				SelectedLink = ActiveGroup.ItemLinks[0];
			} finally {
				EndUpdate();
			}
		}
		void InitPointerItem() {
			if(pointerItem != null)
				pointerItem.Dispose();
			pointerItem = new DevExpress.XtraNavBar.NavBarItem();
			pointerItem.Caption = ReportLocalizer.GetString(ReportStringId.UD_XtraReportsPointerItemCaption);
			pointerItem.Name = "navBarItem1";
			XRToolboxService toolboxService = (XRToolboxService)xrDesignPanel.GetService(typeof(IToolboxService));
			if(toolboxService != null)
				SetImageIndex(pointerItem, toolboxService.GetImageByType(typeof(object)));
			Items.AddRange(new NavBarItem[] { pointerItem });
		}
		void FillToolBoxGroup(NavBarGroup group, ToolboxItemCollection items) {
			IDesignerHost host = (IDesignerHost)xrDesignPanel.GetService(typeof(IDesignerHost));
			ToolboxItem[][] subcategorizedItems = XRToolboxService.GroupItemsBySubCategory(items, host);
			foreach(ToolboxItem[] subcategory in subcategorizedItems)
				for(int i = 0; i < subcategory.Length; i++) {
					NavBarItem item = new NavBarItem();
					ToolboxItem toolboxItem = subcategory[i];
					SetImageIndex(item, toolboxItem);
					item.Caption = toolboxItem is LocalizableToolboxItem ? ((LocalizableToolboxItem)toolboxItem).DisplayName : ((ToolboxItem)toolboxItem).DisplayName;
					toolboxItems[item] = toolboxItem;
					Items.Add(item);
					group.ItemLinks.Add(new NavBarItemLink(item));
				}
		}
		void SetImageIndex(NavBarItem item, ToolboxItem toolboxItem) {
			XRToolboxService toolboxService = (XRToolboxService)xrDesignPanel.GetService(typeof(IToolboxService));
			if(toolboxService == null)
				return;
			SetImageIndex(item, toolboxService.GetImage(toolboxItem));
		}
		void SetImageIndex(NavBarItem item, Image image) {
			if(image == null)
				throw new ArgumentException();
			ImageCollection imageCollection = (ImageCollection)this.SmallImages;
			if(!imageCollection.Images.Contains(image))
				item.SmallImageIndex = imageCollection.Images.Add(image);
			else
				item.SmallImageIndex = imageCollection.Images.IndexOf(image);
		}
		void CreatePointerNavBarItem(NavBarGroup group) {
			group.ItemLinks.AddRange(new NavBarItemLink[] { new NavBarItemLink(PointerItem) });
		}
	}
}
