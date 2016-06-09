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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.Utils.Menu;
using System.IO;
namespace DevExpress.XtraBars {
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent(null)]
	public class BarCustomContainerItem : BarItem, IAppearanceOwner {
		MenuAppearance menuAppearance;
		int menuBarWidth;
		bool showMenuCaption;
		string menuCaption;
		MenuDrawMode menuDrawMode;
		internal event EventHandler ItemLinksChanged;
		BarItemLinkCollection itemLinks; 
		internal BarCustomContainerItem(BarManager barManager) : base(barManager) {
			this.menuCaption = string.Empty;
			this.showMenuCaption = false;
			this.menuDrawMode = MenuDrawMode.Default;
			this.menuAppearance = new MenuAppearance(this);
			this.menuAppearance.Changed += new EventHandler(OnMenuAppearanceChanged);
			this.itemLinks = CreateLinkCollection();
			this.itemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
			this.menuBarWidth = 0;
			this.ShowNavigationHeader = DefaultBoolean.Default;
		}
		internal BarCustomContainerItem() : this(null) {
		}
		protected virtual BarItemLinkCollection CreateLinkCollection() {
			return new BarItemLinkCollection(this);
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowNavigationHeader {
			get;
			set;
		}
		protected internal virtual bool ShouldUpdateEditingLink { get { return false; } }
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(MenuAppearance != null) 
					MenuAppearance.Changed -= new EventHandler(OnMenuAppearanceChanged);
				if(ItemLinks != null) {
					ItemLinks.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
				}
			}
			base.Dispose(disposing);
		}
		bool ShouldSerializeMenuAppearance() { return MenuAppearance.ShouldSerialize(); }
		void ResetMenuAppearance() { MenuAppearance.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemMenuAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MenuAppearance MenuAppearance {
			get { return menuAppearance; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemMenuBarWidth"),
#endif
 DefaultValue(0), Category("Appearance")]
		public virtual int MenuBarWidth {
			get { return menuBarWidth; }
			set {
				if(value < 0) value = 0;
				if(MenuBarWidth == value) return;
				menuBarWidth = value;
			}
		}
		[Obsolete(BarsObsoleteText.SRObsoleteMenuAppearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color BackColor {
			get { return MenuAppearance.Menu.BackColor; }
			set { MenuAppearance.Menu.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteMenuAppearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Image BackgroundImage {
			get { return MenuAppearance.Menu.Image; }
			set { MenuAppearance.Menu.Image = value; }
		}
		[XtraSerializableProperty(true, false, true), Browsable(false)]
		public virtual BarItemLinkCollection ItemLinks { get { return itemLinks; } }
		public override bool ContainsItem(BarItem item) {
			foreach(BarItemLink link in ItemLinks) {
				if(link.Item == null) continue;
				if(link.Item == item) return true;
				if(link.Item.ContainsItem(item)) return true;
			}
			return false;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemMenuDrawMode"),
#endif
DefaultValue(MenuDrawMode.Default), Category("Appearance")]
		public virtual MenuDrawMode MenuDrawMode {
			get { return menuDrawMode; }
			set { menuDrawMode = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemShowMenuCaption"),
#endif
DefaultValue(false), Category("Appearance")]
		public bool ShowMenuCaption {
			get { return showMenuCaption; }
			set {
				showMenuCaption = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemMenuCaption"),
#endif
DefaultValue(""), Category("Appearance")]
		public string MenuCaption {
			get { return menuCaption; }
			set {
				if(value == null) value = string.Empty;
				menuCaption = value;
			}
		}
		protected internal virtual bool CanOpenMenu { get { return true; } }
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				if(link.IsLinkInMenu)
					ps = BarItemPaintStyle.CaptionGlyph;
				else
					ps = BarItemPaintStyle.Caption;
			}
			return ps;
		}
		protected virtual void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
		}
		protected void OnMenuAppearanceChanged(object sender, EventArgs e) {
			OnItemChanged();
		}
		private static object popup = new object();
		private static object closeUp = new object();
		private static object getItemData = new object();
		private static object paintMenuBar = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemPaintMenuBar"),
#endif
 Category("Events")]
		public event BarCustomDrawEventHandler PaintMenuBar {
			add { Events.AddHandler(paintMenuBar, value); }
			remove { Events.RemoveHandler(paintMenuBar, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemGetItemData"),
#endif
 Category("Events")]
		public event EventHandler GetItemData {
			add { Events.AddHandler(getItemData, value); }
			remove { Events.RemoveHandler(getItemData, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemCloseUp"),
#endif
 Category("Events")]
		public event EventHandler CloseUp {
			add { Events.AddHandler(closeUp, value); }
			remove { Events.RemoveHandler(closeUp, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemPopup"),
#endif
 Category("Events")]
		public event EventHandler Popup {
			add { Events.AddHandler(popup, value); }
			remove { Events.RemoveHandler(popup, value); }
		}
		protected internal virtual void OnCloseUp() {
			EventHandler handler = (EventHandler)Events[closeUp];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void OnPopup() {
			EventHandler handler = (EventHandler)Events[popup];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected bool IsOpened() {
			foreach(BarCustomContainerItemLink link in Links) {
				if(link.Opened) return true;
			}
			return false;
		}
		protected internal virtual void OnGetItemData() {
			if(IsOpened()) return;
			EventHandler handler = (EventHandler)Events[getItemData];
			if(handler == null) return;
			BeforeGetItemData();
			try {
				handler(this, EventArgs.Empty);
			}
			finally {
				AfterGetItemData();
			}
		}
		protected virtual void AfterGetItemData() {
			CancelUpdate();
		}
		protected virtual void BeforeGetItemData() {
			BeginUpdate();
		}
		protected virtual void ItemsCollectionChanged() { 
			FireChanged();
			if(ItemLinksChanged != null)
				ItemLinksChanged(this, EventArgs.Empty);
		}
		protected internal virtual void RaisePaintMenuBar(BarCustomDrawEventArgs e) {
			BarCustomDrawEventHandler handler = (BarCustomDrawEventHandler)Events[paintMenuBar];
			if(handler != null) handler(this, e);
		}
	}
	public class BarInListItem : BarButtonItem {
		BarListItem listItem;
		internal BarInListItem(BarListItem listItem) : base(null, true) {
			this.listItem = listItem;
		}
		public BarListItem ListItem { get { return listItem; } }
	}
	public class BarInListItemLink : BarButtonItemLink {
		protected BarInListItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override void BringParentToTopInRecentList() {
			if(OwnerListItemLink != null)
				OwnerListItemLink.BringToTopInRecentList(true);
		}
	}
	public class BarInMdiChildrenListItem : BarInListItem, Docking2010.Customization.ICommandItem {
		internal BarInMdiChildrenListItem(BarListItem listItem)
			: base(listItem) {
		}
		internal BarInMdiChildrenListItem(BarListItem listItem, string caption)
			: base(listItem) {
			Caption = caption;
		}
		bool Docking2010.Customization.ICommandItem.BeginGroup { get; set; }
		Image Docking2010.Customization.ICommandItem.Image {
			get { return Glyph; }
			set { Glyph = value; }
		}
		bool Docking2010.Customization.ICommandItem.Visible {
			get { return Visibility != BarItemVisibility.Never; }
			set { Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; }
		}
		bool Docking2010.Customization.ICommandItem.UseCommandGroupVisibility {
			get { return true; }
		}
	}
	public class BarInWorkspaceListItem : BarInListItem {
		protected WorkspaceManager workspaceManagerCore;
		internal BarInWorkspaceListItem(WorkspaceManager workspaceManager, BarListItem listItem)
			: base(listItem) {
			workspaceManagerCore = workspaceManager;
		}
		public bool BeginGroup { get; set; }
	}
	public class BarInWorkspaceListRestoreItem : BarInWorkspaceListItem {
		internal BarInWorkspaceListRestoreItem(WorkspaceManager workspaceManager, BarListItem listItem) :
			base(workspaceManager, listItem) {
			Caption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.LoadWorkspaceItemCaption); ;
			WorkspacesPath = string.Empty;
		}
		internal BarInWorkspaceListRestoreItem(WorkspaceManager workspaceManager, BarListItem listItem, string workspacesPath) :
			base(workspaceManager, listItem) {
			WorkspacesPath = workspacesPath;
			Caption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.LoadWorkspaceItemCaption); ;
		}
		Image glyphCore;
		string WorkspacesPath { get; set; }
		public override Image Glyph {
			get {
				if(glyphCore == null)
					glyphCore = ImageResourceLoader.GetImageFromResources("LoadFrom_16x16");;
				return glyphCore;
			}
			set { }
		}
		protected internal override void OnClick(BarItemLink link) {
			base.OnClick(link);
			using(FileDialog fileDialog = new OpenFileDialog()) {
				if(!string.IsNullOrEmpty(WorkspacesPath)) {
					string fullPath = Path.GetFullPath(WorkspacesPath);
					if(Directory.Exists(fullPath))
						fileDialog.InitialDirectory = fullPath;
				}
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = DocumentManagerLocalizer.GetString(DocumentManagerStringId.LoadWorkspaceFormCaption);
				fileDialog.CheckFileExists = true;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					string path = fileDialog.FileName;
					string fileName = Path.GetFileNameWithoutExtension(path);
					workspaceManagerCore.LoadWorkspace(fileName, path);
				}
			}
		}
	}
	public class BarInWorkspaceListCaptureItem : BarInWorkspaceListItem {
		internal BarInWorkspaceListCaptureItem(WorkspaceManager workspaceManager, BarListItem listItem) :
			base(workspaceManager, listItem) {
				Caption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.CaptureWorkspaceItemCaption);
		}
		Image glyphCore;
		public override Image Glyph {
			get {
				if(glyphCore == null)
					glyphCore = ImageResourceLoader.GetImageFromResources("AddFile_16x16");
				return glyphCore;
			}
			set { }
		}
		protected internal override void OnClick(BarItemLink link) {
			base.OnClick(link);
			using(WorkspaceNameForm form = new WorkspaceNameForm(workspaceManagerCore)) {
				var result = form.ShowDialog();
				if(result == DialogResult.OK)
					workspaceManagerCore.CaptureWorkspace(form.WorkspaceName);
			}
		}
	}
	public class BarInMdiChildrenListItemLink : BarInListItemLink {
		protected BarInMdiChildrenListItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }		
	}
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("ListItemClick")]
	public class BarListItem : BarCustomContainerItem {
		protected ArrayList subItems;
		bool showNumbers, showChecks;
		Strings strings;
		int itemIndex;
		int maxSubItemWidth = 0;
		public BarListItem(BarManager barManager, string caption) : this(barManager) {
			Caption = caption;
		}
		internal BarListItem(BarManager barManager) : base(barManager) {
			itemIndex = -1;
			this.maxSubItemWidth = 0;
			subItems = new ArrayList();
			strings = new Strings();
			strings.Changed += new EventHandler(StringsChanged);
			showNumbers = showChecks = false;
		}
		public BarListItem() : this(null) {
		}
		protected override void Dispose(bool disposing) {
			ClearItems();
			strings.Changed -= new EventHandler(StringsChanged);
			strings.Clear();
			base.Dispose(disposing);
		}
		protected internal override void AfterLoad() {
			base.AfterLoad();
			CreateItems();
		}
		[DefaultValue(0), Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemMaxSubItemTextWidth")
#else
	Description("")
#endif
]
		public int MaxSubItemTextWidth {
			get { return maxSubItemWidth; }
			set {
				value = Math.Max(0, value);
				if(MaxSubItemTextWidth == value) return;
				maxSubItemWidth = value;
				OnItemChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarListItemEnabled")]
#endif
public override bool Enabled {
			get { return base.Enabled; }
			set {
				if(Enabled == value) return;
 				base.Enabled = value;
				CreateItems();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemItemIndex"),
#endif
 DefaultValue(-1), RefreshProperties(RefreshProperties.All), Category("Behavior")]
		public virtual int ItemIndex {
			get { return itemIndex; }
			set {
				if(value < 0) value = -1;
				if(ItemIndex == value) return;
				itemIndex = value;
				CreateItems();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemDataIndex"),
#endif
 DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All), Category("Behavior")]
		public virtual int DataIndex {
			get {
				if(ItemIndex == -1 || Manager == null) return -1;
				if(ItemIndex < Strings.Count) return GetDataIndex(Strings[ItemIndex]);
				return -1;
			}
			set {
				if(value < 0) value = -1;
				if(value == DataIndex) return;
				ItemIndex = GetIndexByDataIndex(value);
			}
		}
		internal bool ShouldSerializeShowNumbers() { return ShowNumbers; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemShowNumbers"),
#endif
 Category("Appearance")]
		public virtual bool ShowNumbers {
			get { return showNumbers; }
			set {
				if(ShowNumbers == value) return;
				showNumbers = value;
				CreateItems();
				OnItemChanged();
			}
		}
		internal bool ShouldSerializeShowChecks() { return ShowChecks; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemShowChecks"),
#endif
 Category("Appearance")]
		public virtual bool ShowChecks {
			get { return showChecks; }
			set {
				if(ShowChecks == value) return;
				showChecks = value;
				CreateItems();
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemStrings"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Strings Strings {
			get { return strings; }
		}
		private static object listItemClick = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarListItemListItemClick"),
#endif
 Category("Events")]
		public event ListItemClickEventHandler ListItemClick {
			add { Events.AddHandler(listItemClick, value); }
			remove { Events.RemoveHandler(listItemClick, value); }
		}
		protected internal virtual void RaiseListItemClick(ListItemClickEventArgs e) {
			ListItemClickEventHandler handler = (ListItemClickEventHandler)Events[listItemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnListItemClick(ListItemClickEventArgs e) {
			RaiseListItemClick(e);
		}
		bool ShouldRecreateItems { get; set; }
		protected override void AfterGetItemData() {
			if(ShouldRecreateItems) {
				ShouldRecreateItems = false;
				CreateItems();
			}
			base.AfterGetItemData();
		}
		void StringsChanged(object sender, EventArgs args) {
			if(IsLockUpdate) {
				ShouldRecreateItems = true;
				return;
			}
			CreateItems();
			OnItemChanged();
		}
		public override void EndUpdate() {
			if(ShouldRecreateItems) {
				ShouldRecreateItems = false;
				CreateItems();
			}
			base.EndUpdate();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(subItems.Count != Strings.Count) { 
				StringsChanged(this, EventArgs.Empty);
			}
		}
		protected void ClearItems() {
			foreach(BarItem item in subItems) {
				item.ItemClick -= new ItemClickEventHandler(OnSubItemClick);
				item.Dispose();
			}
			subItems.Clear();
		}
		protected void OnSubItemClick(object sender, ItemClickEventArgs e) {
			int index = (int)e.Item.Tag;
			if(ShowChecks == true && ItemIndex == index) ItemIndex = -1;
			else ItemIndex = index;
			OnListItemClick(new ListItemClickEventArgs(this, index));
			OnClick(e.Link);
		}
		int GetIndexByDataIndex(int sourceDataIndex) {
			int dataIndex = 0;
			for(int n = 0; n < Strings.Count; n++) {
				if(Strings[n] == "-") continue;
				if(sourceDataIndex == dataIndex) return n;
				dataIndex ++;
			}
			return -1;
		}
		int GetDataIndex(string item) {
			int index = 0;
			foreach(string str in Strings) {
				if(str == "-") continue;
				if(str == item) return index;
				index ++;
			}
			return -1;
		}
		protected virtual BarInListItem CreateInListItem() {
			return new BarInListItem(this);
		}
		protected virtual void CreateItems() {
			if(Manager == null) return;
			ItemLinks.Clear();
			ClearItems();
			int dataIndex = 0;
			bool beginGroup = false;
			for(int index = 0; index < Strings.Count; index ++) {
				string str = Strings[index];
				string caption = str;
				if(caption == "-") {
					beginGroup = true; 
					continue;
				}
				BarInListItem listItem = CreateInListItem();
				if(ShowNumbers) {
					caption = (dataIndex < 10 ? "&" : "") + (++dataIndex).ToString() + " " + str;
				}
				listItem.Enabled = Enabled;
				listItem.Caption = caption;
				listItem.Tag = index;
				if(ShowChecks) {
					listItem.AllowAllUp = false;
					listItem.ButtonStyle = BarButtonStyle.Check;
					if(index == ItemIndex) listItem.Down = true;
				}
				subItems.Add(listItem);
				listItem.Manager = Manager;
				BarItemLink link = ItemLinks.Add(listItem);
				link.BeginGroup = beginGroup;
				beginGroup = false;
				listItem.ItemClick += new ItemClickEventHandler(OnSubItemClick);
				UpdateListItem(listItem);
			}
		}
		protected virtual void UpdateListItem(BarInListItem listItem) {
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	internal class BarDockPanelListItem : BarListItem {
		Strings panelList;
		protected BarDockPanelListItem(BarManager barManager) : base(barManager) {
			panelList = new Strings();
			ShowNumbers = ShowChecks = true;
		}
		public BarDockPanelListItem()
			: this(null) {
		}
		protected override void UpdateListItem(BarInListItem listItem) {
			base.UpdateListItem(listItem);
			int index = (int)listItem.Tag;
			if(index < 0 || index >= DockPanels.Length) return;
			listItem.Enabled = DockPanels[index].Enabled;
			if(Manager.DockManager == null) return;
			if(Manager.DockManager.Images == null) listItem.Glyph = null;
			listItem.Glyph = ImageCollection.GetImageListImage(Manager.DockManager.Images, DockPanels[index].ImageIndex);
		}
		protected override BarInListItem CreateInListItem() {
			return new BarInMdiChildrenListItem(this);
		}
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			panelList.Clear();
			if(DockPanels != null) {
				foreach(DockPanel panel in DockPanels) {
					panelList.Add(panel.Text);
				}
			}
			CreateItems();
		}
		internal new bool ShouldSerializeShowChecks() { return !ShowChecks; }
		[ Category("Appearance")]
		public override bool ShowChecks {
			get { return base.ShowChecks; }
			set { base.ShowChecks = value; }
		}
		internal new bool ShouldSerializeShowNumbers() { return !ShowNumbers; }
		[ Category("Appearance")]
		public override bool ShowNumbers {
			get { return base.ShowNumbers; }
			set { base.ShowNumbers = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Strings Strings {
			get { return panelList; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks { get { return base.ItemLinks; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ItemIndex { 
			get { 
				if(DockPanels == null) return -1;
				return Array.IndexOf(DockPanels, Manager.DockManager.ActivePanel);
			}
			set {
				if(value < 0) value = -1;
				if(ItemIndex == value) return;
				if(DockPanels == null || DockPanels.Length <= value) return;
				if(value != -1) {
					DockPanels[value].Show();
				}
				CreateItems();
			}
		}
		[DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All), Browsable(false)]
		public override int DataIndex {
			get { return ItemIndex; }
			set { ItemIndex = value;
			}
		}
		internal DockPanel[] DockPanels {
			get {
				if(Manager == null || Manager.DockManager == null) return null;
				ArrayList list = new ArrayList();
				ReadOnlyPanelCollection dockPanels = Manager.DockManager.Panels;
				foreach(DockPanel panel in dockPanels) {
					if(!panel.IsMdiDocument && panel.Count <= 1)
						list.Add(panel);
				}
				return list.ToArray(typeof(DockPanel)) as DockPanel[];
			}
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarDockingMenuItem : BarListItem {
		Strings documentListCore;
		IList<BarInMdiChildrenListItem> commandListCore;
		DocumentManager documentManagerCore;
		internal IList<BarInMdiChildrenListItem> CommandList {
			get { return commandListCore; }
		}
		protected BarDockingMenuItem(BarManager barManager) : base(barManager) {
			documentListCore = new Strings();
			commandListCore = new List<BarInMdiChildrenListItem>();
			ShowNumbers = ShowChecks = true;
			MaxDocuments = 10;
			visibleDocumentsCore = new List<BaseDocument>();
		}
		public BarDockingMenuItem()
			: this(null) {
		}
		protected override BarInListItem CreateInListItem() {
			return new BarInMdiChildrenListItem(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockingMenuItemMaxDocuments"),
#endif
 Category("Appearance"), DefaultValue(10)]
		public int MaxDocuments { get; set; }
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			documentListCore.Clear();
			commandListCore.Clear();
			var documents = GetDocuments();
			if(documents != null) {
				FillCommandList();
				for(int i = 0; i < documents.Length; i++) {
					if(i >= MaxDocuments) break;
					documentListCore.Add(documents[i].Caption);
				}
			}
			CreateItems();
		}
		void FillCommandList() {
			if(Manager.DockManager != null && Manager.DockManager.ActivePanel != null) {
				if(DockPanel.IsActive(Manager.DockManager.ActivePanel))
					((Docking.Controller.IDockControllerInternal)Manager.DockManager.DockController).CreateBarDockingMenuItemCommands(this);
			}
			if(DocumentManager != null && DocumentManager.View != null && !DocumentManager.View.IsDisposing)
				((IBaseViewControllerInternal)DocumentManager.View.Controller).CreateBarDockingMenuItemCommands(this);
		}
		internal new bool ShouldSerializeShowChecks() { return !ShowChecks; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockingMenuItemShowChecks"),
#endif
 Category("Appearance")]
		public override bool ShowChecks {
			get { return base.ShowChecks; }
			set { base.ShowChecks = value; }
		}
		internal new bool ShouldSerializeShowNumbers() { return !ShowNumbers; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockingMenuItemShowNumbers"),
#endif
 Category("Appearance")]
		public override bool ShowNumbers {
			get { return base.ShowNumbers; }
			set { base.ShowNumbers = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Strings Strings {
			get { return documentListCore; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks { get { return base.ItemLinks; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ItemIndex {
			get {
				if(GetDocuments() == null) return -1;
				return VisibleDocuments.IndexOf(DocumentManager.ActivationInfo.ActiveDocument);
			}
			set {
				if(value < 0) value = -1;
				if(ItemIndex == value) return;
				var documents = GetDocuments();
				if(documents == null || documents.Length <= value) return;
				if(value != -1) {
					var view = DocumentManager.View;
					BaseDocument nextDocument = documents[value];
					var prevDocument = DocumentManager.ActivationInfo.ActiveDocument;
					if(!view.RaiseNextDocument(prevDocument, ref nextDocument, true)) {
						view.Controller.Activate(nextDocument);
						if(prevDocument != null && nextDocument != null) {
							if(prevDocument.IsFloating && !nextDocument.IsFloating) {
								var form = Manager.GetForm();
								if(Form.ActiveForm != form)
									form.Activate();
							}
						}
					}
				}
				CreateItems();
			}
		}
		protected override void CreateItems() {
			if(Manager == null) return;
			ItemLinks.Clear();
			ClearItems();
			int dataIndex = 0;
			bool beginGroup = false;
			bool isDockPanelActive = DockPanel.IsActive();
			if(CommandList != null) {
				bool isDockingItem = true;
				for(int i = 0; i < CommandList.Count - 1; i++) {
					if(isDockingItem) {
						var dockingTag = CommandList[i].Tag as Docking.DockControllerMenu.DockControllerMenuTag;
						if(dockingTag == null) {
							isDockingItem = false;
							beginGroup = true;
						}
					}
					if(!isDockingItem && isDockPanelActive) {
						var documentTag = CommandList[i].Tag as Docking2010.Customization.BaseViewControllerMenu.BaseViewControllerMenuTag;
						if(documentTag != null) {
							if(documentTag.Args.Command is BaseViewControllerCommand.FloatCommand)
								continue;
							if(documentTag.Args.Command is BaseViewControllerCommand.DockCommand)
								continue;
						}
					}
					AddSubItemCore(beginGroup || ((Docking2010.Customization.ICommandItem)CommandList[i]).BeginGroup, CommandList[i]);
					if(!isDockingItem)
						beginGroup = false;
				}
			}
			beginGroup = true;
			for(int index = 0; index < Strings.Count; index++) {
				string str = Strings[index];
				string caption = str;
				if(ShowNumbers) {
					caption = (dataIndex < 10 ? "&" : "") + (++dataIndex).ToString() + " " + str;
				}
				AddSubItem(beginGroup, index, caption, null);
				beginGroup = false;
			}
			if(CommandList != null && CommandList.Count > 0) {
				AddSubItemCore(beginGroup || ((Docking2010.Customization.ICommandItem)CommandList[CommandList.Count - 1]).BeginGroup, CommandList[CommandList.Count - 1]);
				beginGroup = false;
			}
		}
		protected void AddSubItem(bool beginGroup, int index, string caption, Image image) {
			BarInListItem listItem = CreateInListItem();
			listItem.Enabled = Enabled;
			listItem.Caption = caption;
			listItem.Tag = index;
			listItem.Glyph = image;
			if(ShowChecks) {
				listItem.AllowAllUp = false;
				listItem.ButtonStyle = BarButtonStyle.Check;
				if(index == ItemIndex) listItem.Down = true;
			}
			AddSubItemCore(beginGroup, listItem);
		}
		void AddSubItemCore(bool beginGroup, BarInListItem listItem) {
			subItems.Add(listItem);
			listItem.Manager = Manager;
			BarItemLink link = ItemLinks.Add(listItem);
			link.BeginGroup = beginGroup;
			listItem.ItemClick += new ItemClickEventHandler(OnListItemClick);
			UpdateListItem(listItem);
		}
		protected void OnListItemClick(object sender, ItemClickEventArgs e) {
			int index = -1;
			Docking2010.Customization.BaseViewControllerMenu.BaseViewControllerMenuTag viewTag = e.Item.Tag as Docking2010.Customization.BaseViewControllerMenu.BaseViewControllerMenuTag;
			Docking.DockControllerMenu.DockControllerMenuTag panelTag = e.Item.Tag as Docking.DockControllerMenu.DockControllerMenuTag;
			if(e.Item.Tag is int)
				index = (int)e.Item.Tag;
			else if(viewTag != null) {
				BaseViewControllerCommand.Execute(viewTag.Controller, viewTag.Args);
			}
			else if(panelTag != null) {
				DevExpress.XtraBars.Docking.Controller.DockControllerCommand.Execute(panelTag.Controller, panelTag.Args);
			}
			if(ShowChecks == true && ItemIndex == index) ItemIndex = -1;
			else ItemIndex = index;
			OnListItemClick(new ListItemClickEventArgs(this, index));
			OnClick(e.Link);
		}
		[DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All), Browsable(false)]
		public override int DataIndex {
			get { return ItemIndex; }
			set { ItemIndex = value; }
		}
		protected internal DocumentManager DocumentManager {
			get {
				if(documentManagerCore == null) {
					if(Manager != null)
						documentManagerCore = DocumentManager.FromControl(Manager.Form);
				}
				return documentManagerCore;
			}
		}
		List<BaseDocument> visibleDocumentsCore;
		internal List<BaseDocument> VisibleDocuments { get { return visibleDocumentsCore; } }
		internal BaseDocument[] GetDocuments() {
			if(DocumentManager == null || DocumentManager.View == null) return null;
			if(DocumentManager.View.Documents == null) return null;
			VisibleDocuments.Clear();
			foreach(BaseDocument document in DocumentManager.ActivationInfo.DocumentActivationList) {
				if(document.IsVisible && !document.IsDisposing)
					visibleDocumentsCore.Add(document);
			}
			return visibleDocumentsCore.ToArray();
		}
	}
	public delegate void WorspaceManagerCustomSortItemsEventHandler(object sender, WorspaceManagerCustomSortItemsEventArgs e);
	public class WorspaceManagerCustomSortItemsEventArgs : EventArgs {
		public IComparer<IWorkspace> Comparer { get; set; }
	}
	public enum WorspacesSortMode { Default, Alphabetical, Usage, Custom };
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarWorkspaceMenuItem : BarListItem {
		Strings workspacesListCore;
		WorkspaceManager workspaceManagerCore;
		protected BarWorkspaceMenuItem(BarManager barManager)
			: base(barManager) {
			workspacesSortModeCore = WorspacesSortMode.Default;
			workspacesListCore = new Strings();
			ShowNumbers = ShowChecks = false;
			visibleWorkspacesCore = new List<IWorkspace>();
			WorkspacesPath = string.Empty;
		}
		public BarWorkspaceMenuItem()
			: this(null) {
		}
		protected override BarInListItem CreateInListItem() {
			return new BarInMdiChildrenListItem(this);
		}
		protected internal override void OnGetItemData() {
			if(IsOpened() || WorkspaceManager == null || WorkspaceManager.Workspaces == null) return;
			workspacesListCore.Clear();
			VisibleWorkspaces.Clear();
			List<IWorkspace> workspaces = new List<IWorkspace>();
			switch(WorkspacesSortMode) { 
				case WorspacesSortMode.Default:
					workspaces.AddRange(workspaceManagerCore.Workspaces.ToArray());
					break;
				case WorspacesSortMode.Alphabetical:
					workspaces.AddRange(workspaceManagerCore.Workspaces.ToArray());
					workspaces.Sort((x, y) => { return x.Name.CompareTo(y.Name); });
					break;
				case WorspacesSortMode.Custom:
					workspaces.AddRange(workspaceManagerCore.Workspaces.ToArray());
					var comparer = RaiseCustomComparsion();
					if(comparer != null && comparer is IComparer<IWorkspace>)
						workspaces.Sort(comparer);
					break;
				case WorspacesSortMode.Usage:
					workspaces.AddRange(workspaceManagerCore.RecentWorkspaces.ToArray());
					workspaces.AddRange(workspaceManagerCore.Workspaces.Select(x => x).Where(x => !workspaces.Contains(x)).ToArray());
					break;
			}
			VisibleWorkspaces.AddRange(workspaces);
			if(workspaces != null) {
				for(int i = 0; i < workspaces.Count; i++) {
					workspacesListCore.Add(workspaces[i].Name);
				}
			}
			CreateItems();
		}
		static readonly object queryCustomComparer = new object();
		[ Category("Behavior")]
		public event WorspaceManagerCustomSortItemsEventHandler QueryCustomComparer {
			add { Events.AddHandler(queryCustomComparer, value); }
			remove { Events.RemoveHandler(queryCustomComparer, value); }
		}
		IComparer<IWorkspace> RaiseCustomComparsion() {
			var handler = Events[queryCustomComparer] as WorspaceManagerCustomSortItemsEventHandler;
			var ea = new WorspaceManagerCustomSortItemsEventArgs();
			if(handler != null)
				handler(this, ea);
			return ea.Comparer;
		}
		protected internal override void AfterLoad() { }
		bool showSaveLoadCommandsCore;
		[ Category("Behavior"), DefaultValue(false)]
		public bool ShowSaveLoadCommands {
			get { return showSaveLoadCommandsCore; }
			set { showSaveLoadCommandsCore = value; }
		}
		[ Category("Behavior"), DefaultValue("")]
		public string WorkspacesPath { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Strings Strings {
			get { return workspacesListCore; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks { get { return base.ItemLinks; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ItemIndex {
			get { return -1; }
			set {
				if(value < 0) value = -1;
				if(ItemIndex == value) return;
				var workspaces = GetWorkspaces();
				if(workspaces == null || workspaces.Length <= value) return;
				if(value != -1) {
					IWorkspace workspace = workspaces[value];
					WorkspaceManager.ApplyWorkspace(workspace.Name);
				}
				CreateItems();
			}
		}
		WorspacesSortMode workspacesSortModeCore;
		[ Category("Behavior"), DefaultValue(WorspacesSortMode.Default)]
		public WorspacesSortMode WorkspacesSortMode {
			get { return workspacesSortModeCore; }
			set { workspacesSortModeCore = value; }
		}
		protected override void CreateItems() {
			if(Manager == null || Manager.IsLoading) return;
			ItemLinks.Clear();
			ClearItems();
			var captureCommand = new BarInWorkspaceListCaptureItem(WorkspaceManager, new BarListItem());
			AddSubItemCore(captureCommand.BeginGroup, captureCommand);
			if(ShowSaveLoadCommands) {
				var restoreCommand = new BarInWorkspaceListRestoreItem(WorkspaceManager, new BarListItem(), WorkspacesPath);
				AddSaveCommand();
				AddSubItemCore(restoreCommand.BeginGroup, restoreCommand);
			}
			AddWorkspacesCommand();
			int dataIndex = 0;
			for(int index = 0; index < Strings.Count; index++) {
				string caption = Strings[index];
				if(ShowNumbers) {
					caption = (dataIndex < 10 ? "&" : "") + (++dataIndex).ToString() + " " + caption;
				}
				AddSubItem(index == 0, index, caption, null);
			}
		}
		protected void AddSubItem(bool beginGroup, int index, string caption, Image image) {
			BarInListItem listItem = CreateInListItem();
			listItem.Enabled = Enabled;
			listItem.Caption = caption;
			listItem.Tag = index;
			listItem.Glyph = image;
			if(ShowChecks) {
				listItem.AllowAllUp = false;
				listItem.ButtonStyle = BarButtonStyle.Check;
				if(index == ItemIndex) listItem.Down = true;
			}
			AddSubItemCore(beginGroup, listItem);
		}
		void AddSaveCommand() {
			string SaveWorkspaceItemCaption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.SaveWorkspaceItemCaption);
			BarSubItem saveItem = new BarSubItem() { Caption = SaveWorkspaceItemCaption };
			saveItem.Glyph = ImageResourceLoader.GetImageFromResources("Save_16x16");
			var subItem = new BarButtonItem() { Caption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.SaveCurrentWorkspaceItemCaption) };
			subItem.ItemClick += (sender, e) => { OpenSaveFileDialog(string.Empty); };
			saveItem.AddItem(subItem);
			subItems.Add(saveItem);
			Manager.Items.Add(saveItem);
			Manager.Items.Add(subItem);
			BarItemLink link1 = ItemLinks.Add(saveItem);
			foreach(var item in Strings) {
				IWorkspace workspace = WorkspaceManager.GetWorkspace(item.ToString());
				bool hasPath = !string.IsNullOrEmpty(workspace.Path);
				var barItem = new BarButtonItem() { Caption = item.ToString() };
				if(!hasPath)
					barItem.Caption += "...";
				barItem.ItemClick += (sender, e) =>
				{
					if(!hasPath)
						OpenSaveFileDialog(barItem.Caption);
					else {
						WorkspaceManager.CaptureWorkspace(workspace.Name);
						WorkspaceManager.SaveWorkspace(workspace.Name, workspace.Path);
					}
				};
				saveItem.AddItem(barItem);
			}
		}
		void AddWorkspacesCommand() {
			string WorkspacesItemCaption = DocumentManagerLocalizer.GetString(DocumentManagerStringId.CommandWorkspacesDialog);
			BarButtonItem workspacesItem = new BarButtonItem() { Caption = WorkspacesItemCaption };
			subItems.Add(workspacesItem);
			Manager.Items.Add(workspacesItem);
			ItemLinks.Add(workspacesItem);
			workspacesItem.ItemClick += (sender, e) =>
			{
				using(DevExpress.XtraBars.Docking2010.Customization.WorkspacesDialog workspacesDialog = new DevExpress.XtraBars.Docking2010.Customization.WorkspacesDialog(WorkspaceManager)) {
					if(DialogResult.OK == workspacesDialog.ShowDialog()) {
						if(workspacesDialog.Result != null)
							WorkspaceManager.ApplyWorkspace(workspacesDialog.Result.Name);
					}
				}
			};
		}
		void OpenSaveFileDialog(string workspaceName) {
			using(FileDialog fileDialog = new SaveFileDialog()) {
				if(!string.IsNullOrEmpty(WorkspacesPath)) {
					string fullPath = Path.GetFullPath(WorkspacesPath);
					if(Directory.Exists(fullPath))
						fileDialog.InitialDirectory = fullPath;
				}
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = DocumentManagerLocalizer.GetString(DocumentManagerStringId.SaveWorkspaceFormCaption);
				workspaceName = workspaceName.TrimEnd(new char[] { '.' });
				fileDialog.FileName = workspaceName;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					string path = fileDialog.FileName;
					string fileName = Path.GetFileNameWithoutExtension(path);
					WorkspaceManager.SaveWorkspace(fileName, path, true);
				}
			}
		}
		void AddSubItemCore(bool beginGroup, BarInListItem listItem) {
			subItems.Add(listItem);
			listItem.Manager = Manager;
			BarItemLink link = ItemLinks.Add(listItem);
			link.BeginGroup = beginGroup;
			listItem.ItemClick += OnItemClick;
			listItem.Disposed += (sender, e) =>
			{
				listItem.ItemClick -= OnItemClick;
			};
			UpdateListItem(listItem);
		}
		void OnItemClick(object sender, ItemClickEventArgs e) {
			int index = -1;
			if(e.Item.Tag is int)
				index = (int)e.Item.Tag;
			if(ShowChecks == true && ItemIndex == index) ItemIndex = -1;
			else ItemIndex = index;
			OnListItemClick(new ListItemClickEventArgs(this, index));
			OnClick(e.Link);
		}
		[DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All), Browsable(false)]
		public override int DataIndex {
			get { return ItemIndex; }
			set { ItemIndex = value; }
		}
		[ Category("Behavior"), DefaultValue(null)]
		public  WorkspaceManager WorkspaceManager {
			get {
				if(workspaceManagerCore == null)
					if(Manager != null) {
						Control control = Manager.Form is RibbonControl ? Manager.Form.Parent : Manager.Form;
						workspaceManagerCore = WorkspaceManager.FromControl(control) ?? CreateWorkspaceManager(control);
					}
				return workspaceManagerCore;
			}
			set { workspaceManagerCore = value; }
		}
		WorkspaceManager CreateWorkspaceManager(Control control) {
			WorkspaceManager manager = null;
			if(control != null) {
				manager = new WorkspaceManager() { TargetControl = control };
				if(control.Container != null)
					control.Container.Add(manager);
			}
			return manager;
		}
		List<IWorkspace> visibleWorkspacesCore;
		internal List<IWorkspace> VisibleWorkspaces { get { return visibleWorkspacesCore; } }
		internal IWorkspace[] GetWorkspaces() {
			if(WorkspaceManager == null || WorkspaceManager.Workspaces.Count == 0) return null;
			return visibleWorkspacesCore.ToArray();
		}
		internal void EndInit() {
			if(Manager == null || Manager.IsLoading) return;
			CreateItems();
		}
	}
	static class ImageResourceLoader {
		public static Bitmap GetImageFromResources(string imageName) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraBars.Images.{0}.png", imageName), typeof(BarSubItem).Assembly) as Bitmap;
		}
	}
	public enum BarMdiChildrenListShowCheckMode { Check, Glyph }
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarMdiChildrenListItem : BarListItem {
		Strings mdiList;
		protected BarMdiChildrenListItem(BarManager barManager)
			: base(barManager) {
			mdiList = new Strings();
			ShowNumbers = ShowChecks = true;
			ShowCheckMode = BarMdiChildrenListShowCheckMode.Glyph;
		}
		public BarMdiChildrenListItem()
			: this(null) {
		}
		protected override void UpdateListItem(BarInListItem listItem) {
			base.UpdateListItem(listItem);
			int index = (int)listItem.Tag;
			if(index < 0 || index >= MdiChildren.Length) return;
			listItem.Enabled = MdiChildren[index].Enabled;
			bool dontUseIcon = ShowCheckMode == BarMdiChildrenListShowCheckMode.Check || !MdiChildren[index].ShowIcon;
			if(MdiChildren[index].Icon == null || dontUseIcon)
				listItem.Glyph = null;
			else {
				try { listItem.Glyph = new Icon(MdiChildren[index].Icon, SystemInformation.SmallIconSize).ToBitmap(); }
				catch(Exception) { }
			}
		}
		[DefaultValue(BarMdiChildrenListShowCheckMode.Glyph)]
		public BarMdiChildrenListShowCheckMode ShowCheckMode { get; set; }
		protected override BarInListItem CreateInListItem() {
			return new BarInMdiChildrenListItem(this);
		}
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			mdiList.Clear();
			if(MdiChildren != null) {
				foreach(Form frm in MdiChildren) {
					if(frm.Visible) 
						mdiList.Add(frm.Text);
				}
			}
			CreateItems();
		}
		internal new bool ShouldSerializeShowChecks() { return !ShowChecks; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarMdiChildrenListItemShowChecks"),
#endif
 Category("Appearance")]
		public override bool ShowChecks {
			get { return base.ShowChecks; }
			set { base.ShowChecks = value; }
		}
		internal new bool ShouldSerializeShowNumbers() { return !ShowNumbers; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarMdiChildrenListItemShowNumbers"),
#endif
 Category("Appearance")]
		public override bool ShowNumbers {
			get { return base.ShowNumbers; }
			set { base.ShowNumbers = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Strings Strings {
			get { return mdiList; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks { get { return base.ItemLinks; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ItemIndex {
			get {
				if(MdiChildren == null) return -1;
				return Array.IndexOf(MdiChildren, Form.ActiveMdiChild);
			}
			set {
				if(value < 0) value = -1;
				if(ItemIndex == value) return;
				if(Form == null || MdiChildren == null || MdiChildren.Length <= value) return;
				if(value != -1)
					ActivateMdiChild(MdiChildren[value]);
				CreateItems();
			}
		}
		void ActivateMdiChild(Form mdiChild) {
			var documentManager = DocumentManager.FromControl(Form);
			if(documentManager != null && Manager == documentManager.GetBarManager())
				documentManager.Activate(mdiChild);
			else
				mdiChild.Activate();
		}
		[DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All), Browsable(false)]
		public override int DataIndex {
			get { return ItemIndex; }
			set { ItemIndex = value; }
		}
		protected virtual Form Form {
			get { return Manager != null ? Manager.GetForm() : null; }
		}
		internal Form[] MdiChildren {
			get {
				Form[] mdiChildren = Form == null ? null : Form.MdiChildren;
				if(mdiChildren == null) return null;
				if(Form == null || Form.MdiChildren == null) return null;
				ArrayList list = new ArrayList();
				for(int n = 0; n < mdiChildren.Length; n++) {
					Form frm = mdiChildren[n];
					if(frm.Visible) list.Add(frm);
				}
				if(list.Count == mdiChildren.Length) return mdiChildren;
				return list.ToArray(typeof(Form)) as Form[];
			}
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarToolbarsListItem : BarLinkContainerItem {
		BarButtonItem item;
		bool showCustomizationItem, showToolbars, showDockPanels;
		internal BarToolbarsListItem(bool isPrivateItem, BarManager manager) : this() {
			this.fIsPrivateItem = true;
			this.Manager = manager;
			OnGetItemData();
		}
		public BarToolbarsListItem() {
			this.showDockPanels = false;
			this.showToolbars = true;
			this.showCustomizationItem = true;
			this.item = new BarButtonItem(null, true);
			this.item.ItemClick += new ItemClickEventHandler(onItemClick);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(item != null) {
					item.ItemClick -= new ItemClickEventHandler(onItemClick);
					item.Dispose();
				}
				item = null;
			}
			base.Dispose(disposing);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override LinksInfo LinksPersistInfo { 
			get { return null; } 
			set { 
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden), Browsable(false)]
		public override BarItemLinkCollection ItemLinks {
			get { return base.ItemLinks; }
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(item == null) return;
			item.Manager = Manager;
			if(Manager != null) AddItem(item);
		}
		protected override bool AllowFireChanging { get { return false; } }
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			BeginUpdate();
			try {
				ClearLinks();
				foreach(Bar bar in Manager.Bars) {
					string name = string.Empty;
					if(!ShowToolbars) continue;
					if(bar.IsMainMenu || bar.OptionsBar.DisableClose || (bar.OptionsBar.Hidden && !Manager.IsDesignMode)) continue;
					name = bar.Text;
					BarButtonItem btnItem = new BarButtonItem(Manager, true);
					btnItem.Caption = name;
					btnItem.Tag = bar;
					btnItem.ButtonStyle = BarButtonStyle.Check;
					btnItem.Down = (bar as IDockableObject).IsVisible;
					btnItem.ItemClick += new ItemClickEventHandler(onItemClick);
					AddItem(btnItem);
				}
				if(Manager.DockManager != null && ShowDockPanels) {
					foreach(DevExpress.XtraBars.Docking.DockPanel panel in Manager.DockManager.Panels) {
						if(panel.Count > 0) continue;
						if(panel.Visibility == DevExpress.XtraBars.Docking.DockVisibility.AutoHide || 
							(panel.RootPanel != null && panel.RootPanel.Visibility == DevExpress.XtraBars.Docking.DockVisibility.AutoHide)) {
							BarButtonItem btnItem = new BarButtonItem(Manager, true);
							btnItem.Caption = panel.TabText != "" ? panel.TabText : panel.Text; ;
							btnItem.Tag = panel;
							btnItem.ButtonStyle = BarButtonStyle.Default;
							btnItem.ItemClick += new ItemClickEventHandler(onItemClickAutohide);
							AddItem(btnItem);
						}
						else {
						BarButtonItem btnItem = new BarButtonItem(Manager, true);
						btnItem.Caption = panel.TabText != "" ? panel.TabText : panel.Text;;
						btnItem.Tag = panel;
						btnItem.ButtonStyle = BarButtonStyle.Check;
						btnItem.Down = panel.Visibility != DevExpress.XtraBars.Docking.DockVisibility.Hidden && (panel.RootPanel != null && panel.RootPanel.Visibility != DevExpress.XtraBars.Docking.DockVisibility.Hidden);
						btnItem.ItemClick += new ItemClickEventHandler(onItemClick);
						AddItem(btnItem);   
					}
				}
				}
				if(Manager.AllowCustomization && ShowCustomizationItem) {
					BarItemLink link = AddItem(item);
					link.UserCaption = Manager.GetString(BarString.CustomizeButton);
					link.BeginGroup = true;
				}
			}
			finally {
				CancelUpdate();
			}
			base.OnGetItemData();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarToolbarsListItemShowCustomizationItem"),
#endif
 DefaultValue(true), Category("Appearance")]
		public virtual bool ShowCustomizationItem {
			get { return showCustomizationItem; }
			set {
				showCustomizationItem = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarToolbarsListItemShowToolbars"),
#endif
 DefaultValue(true), Category("Appearance")]
		public virtual bool ShowToolbars {
			get { return showToolbars; }
			set {
				showToolbars = value;
			}
		}
		[Obsolete(BarsObsoleteText.SRObsoleteShowDockWindows), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ShowDockWindows {
			get { return false; }
			set { }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarToolbarsListItemShowDockPanels"),
#endif
 DefaultValue(false), Category("Appearance")]
		public virtual bool ShowDockPanels {
			get { return showDockPanels; }
			set {
				showDockPanels = value;
			}
		}
		void onItemClick(object sender, ItemClickEventArgs e) {
			if(e.Item == item) {
				Manager.Customize();
				return;
			}
			bool visible = ((BarButtonItem)e.Item).Down;
			DevExpress.XtraBars.Docking.DockPanel panel = e.Item.Tag as DevExpress.XtraBars.Docking.DockPanel;
			if(panel != null) {
				if(!visible) 
					panel.Hide();
				else
					panel.Show();
				return;
			}
			IDockableObject dockObject = e.Item.Tag as IDockableObject;
			if(dockObject == null) return;
			dockObject.IsVisible = visible;
		}
		void onItemClickAutohide(object sender, ItemClickEventArgs e) {
			DevExpress.XtraBars.Docking.DockPanel panel = e.Item.Tag as DevExpress.XtraBars.Docking.DockPanel;
			if(panel != null) {
				if(panel.DockManager != null) panel.DockManager.ActivePanel = panel;
				return;
			}
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarLinkContainerItem : BarCustomContainerItem, BarLinksHolder, IOptionsMultiColumnOwner, IVisualEffectsHolder {
		LinksInfo defaultLinks;
		int lockAllowFireEvents;
		protected BarLinkContainerItem(BarManager barManager) : base(barManager) {
			this.lockAllowFireEvents = 0;
			defaultLinks = null;
		}
		public BarLinkContainerItem() : this(null) {
		}
		public void Merge(BarLinkContainerItem item) {
			if(item == null || item.Manager == Manager || !item.GetType().Equals(GetType())) {
				throw new ArgumentException("Wrong argument", "item");
			}
			ItemLinks.Merge(item.ItemLinks);
		}
		public void UnMerge() {
			ItemLinks.UnMerge();
		}
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get {
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		DefaultBoolean multiColumn = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value)
					return;
				multiColumn = value;
				OnItemChanged();
			}
		}
		internal void XtraClearItemLinks(XtraItemEventArgs e) {
			ClearLinks();
		}
		internal object XtraCreateItemLinksItem(XtraItemEventArgs e) {
			return BarLinksHolderSerializer.CreateItemLink(Manager, e, this);
		}
		BarItemLink BarLinksHolder.AddItem(BarItem item, LinkPersistInfo info) {
			BarItemLink link = ItemLinks.Add(item, info);
			return link;
		}
		public virtual void AddItems(BarItem[] items) { ItemLinks.AddRange(items); 	}
		public virtual BarItemLink AddItem(BarItem item) { return ItemLinks.Add(item); }
		public virtual BarItemLink InsertItem(BarItemLink beforeLink, BarItem item) { return ItemLinks.Insert(beforeLink, item); }
		public virtual void RemoveLink(BarItemLink itemLink) { ItemLinks.Remove(itemLink); }
		public virtual void ClearLinks() { ItemLinks.Clear(); }
		protected override void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(Manager == null) return;
			bool isLoading = Manager.IsLoading;
			BarItemLink link = e.Element as BarItemLink;
			if(e.Action == CollectionChangeAction.Add) {
				ItemLinks.UpdateMostRecentlyUsed(link, !isLoading);
				RaiseLinkAdded(link);
			}
			if(e.Action == CollectionChangeAction.Remove) {
				ItemLinks.UpdateRecentIndex(link, -1, !isLoading, !isLoading);
				RaiseLinkDeleted(link);
			}
			UpdateRadialMenu(this);
			if(ItemLinks.IsLockUpdate != 0) return;
			ItemsCollectionChanged();
		}
		private void UpdateRadialMenu(BarLinksHolder holder) {
			BarItem item = holder as BarItem;
			if(item == null)
				return;
			foreach(BarItemLink link in item.Links) {
				if(link.Holder is RadialMenu) {
					((RadialMenu)link.Holder).OnMenuChanged();
				} else
					UpdateRadialMenu(link.Holder);
			}
		}
		protected override void ItemsCollectionChanged() {
			if(IsLockUpdate) return;
			foreach(BarCustomContainerItemLink link in Links) {
				if(link.SubControl != null) {
					link.UpdateVisibleLinks();
				}
			}
			base.ItemsCollectionChanged();
		}
		public override void Reset() {
			base.Reset();
			if(defaultLinks == null) return;
			BeginUpdate();
			try {
				ClearLinks();
				LinksPersistInfo = defaultLinks;
				AfterLoad();
				for(int n = 0; n < ItemLinks.Count; n++) {
					BarItemLink link = ItemLinks[n];
					link.Item.Reset();
				}
				ItemLinks.SetIsMergedState(false); 
			} finally {
				EndUpdate();
			}
		}
		protected virtual bool IsAllowFireEvents { get { return Manager != null && !Manager.IsLoading && lockAllowFireEvents == 0; } }
		protected internal override void AfterLoad() {
			base.AfterLoad();
			SetCurrentStateAsDefault();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetCurrentStateAsDefault() {
			if(LinksPersistInfo == null) return;
			BeginUpdate();
			lockAllowFireEvents++;
			try {
				defaultLinks = LinksPersistInfo;
				LinksPersistInfo = new LinksInfo();
				Manager.CreateLinks(this, defaultLinks);
				Manager.SynchronizeLinksInfo(LinksPersistInfo, defaultLinks);
				LinksInfo temp = defaultLinks;
				defaultLinks = LinksPersistInfo;
				LinksPersistInfo = temp;
			}
			finally {
				lockAllowFireEvents--;
				CancelUpdate();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)
#if DXWhidbey
		,DevExpress.Utils.Design.InheritableCollection
#endif
		]
		public virtual LinksInfo LinksPersistInfo { 
			get { return ItemLinks.LinksPersistInfo; } 
			set { ItemLinks.LinksPersistInfo = value; }
		}
		private static object linkAdded = new object();
		private static object linkDeleted = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLinkContainerItemLinkDeleted"),
#endif
 Category("Events")]
		public event LinkEventHandler LinkDeleted {
			add { Events.AddHandler(linkDeleted, value); }
			remove { Events.RemoveHandler(linkDeleted, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLinkContainerItemLinkAdded"),
#endif
 Category("Events")]
		public event LinkEventHandler LinkAdded {
			add { Events.AddHandler(linkAdded, value); }
			remove { Events.RemoveHandler(linkAdded, value); }
		}
		protected virtual void RaiseLinkAdded(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			LinkEventHandler handler = (LinkEventHandler)Events[linkAdded];
			if(handler != null) handler(this, new LinkEventArgs(link));
		}
		protected virtual void RaiseLinkDeleted(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			LinkEventHandler handler = (LinkEventHandler)Events[linkDeleted];
			if(handler != null) handler(this, new LinkEventArgs(link));
		}
		void IOptionsMultiColumnOwner.OnChanged() {
			OnItemChanged();
		}
		bool IVisualEffectsHolder.VisualEffectsVisible {
			get { return ((IVisualEffectsHolder)ItemLinks).VisualEffectsVisible; }
		}
		DevExpress.Utils.VisualEffects.ISupportAdornerUIManager IVisualEffectsHolder.VisualEffectsOwner {
			get { return ((IVisualEffectsHolder)ItemLinks).VisualEffectsOwner; }
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BarLinkContainerExItem : BarLinkContainerItem {
		protected override void ItemsCollectionChanged() {
			base.ItemsCollectionChanged();
			if(IsLockUpdate) return;
			OnItemChanged();
		}
		protected override void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			base.OnItemLinksCollectionChanged(sender, e);
			BarItemLink link = e.Element as BarItemLink;
			if(e.Action == CollectionChangeAction.Add) {
				link.InplaceHolder = this;	  
			}
			else if(e.Action == CollectionChangeAction.Remove) {
				link.InplaceHolder = null;
			}
		}
	}
	#region class BarSubItem
	[ToolboxItem(false), DesignTimeVisible(false), Designer("DevExpress.XtraBars.Design.BarSubItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))]
	public class BarSubItem : BarLinkContainerItem {
		internal BarSubItem(BarManager barManager, bool privateItem) : base() {
			this.fIsPrivateItem = privateItem;
			this.Manager = barManager;
		}
		public BarSubItem() {
			AutoFillEditorWidth = true;
		}
		public BarSubItem(BarManager manager, string caption) : this(manager, caption, null) {
		}
		public BarSubItem(BarManager manager, string caption, BarItem[] items) : this(manager, caption, -1, items) {
		}
		public BarSubItem(BarManager manager, string caption, int imageIndex) : this(manager, caption, imageIndex, null) {
		}
		public BarSubItem(BarManager manager, string caption, int imageIndex, BarItem[] items) {
			this.Caption = caption;
			this.ImageIndex = imageIndex;
			if(items != null) {
				AddItems(items);
			}
			Manager = manager;
			AutoFillEditorWidth = true;
		}
		[DefaultValue(0)]
		public int PopupMinWidth {
			get;
			set;
		}
		bool hideWhenEmpty = false;
		[DefaultValue(false)]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarSubItemHideWhenEmpty"),
#endif
 Category("Behavior")]
		public bool HideWhenEmpty {
			get { return hideWhenEmpty; }
			set {
				if(HideWhenEmpty == value)
					return;
				hideWhenEmpty = value;
				OnItemChanged();
			}
		}
		protected internal bool InplaceVisible {
			get { return !DesignMode && HideWhenEmpty && (ItemLinks.Count == 0 || !CheckVisibleItems()) ? false : true; }
		}
		private bool CheckVisibleItems() {
			foreach(BarItemLink link in ItemLinks) {
				if(link.CanVisible) {
					BarCustomContainerItemLink cInternalLink = link as BarCustomContainerItemLink;
					if(cInternalLink != null && (cInternalLink.VisibleLinks as BarItemLinkCollection).HasVisibleItems) return true;
				}
			}
			return false;
		}
		DefaultBoolean allowDrawArrrow = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarSubItemAllowDrawArrow"),
#endif
 Category("Behavior")]
		public DefaultBoolean AllowDrawArrow {
			get { return allowDrawArrrow; }
			set {
				if(AllowDrawArrow == value)
					return;
				allowDrawArrrow = value;
				OnItemChanged();
			}
		}
		internal virtual bool IsNeedOpenArrow { get {
			if((AllowDrawArrow == DefaultBoolean.Default) || (AllowDrawArrow ==  DefaultBoolean.True))
				return true;
			else
				return false;
		} }
		[DefaultValue(true)]
		public bool AutoFillEditorWidth { get; set; }
	}
	#endregion class BarSubItem
	public enum ButtonGroupsLayout { Default, Auto, TwoRows, ThreeRows }
	public class BarButtonGroup : BarLinkContainerExItem {
		public BarButtonGroup() { }
		public BarButtonGroup(BarManager manager) : this(manager, null) {
		}
		public BarButtonGroup(BarManager manager, BarBaseButtonItem[] items) {
			if(items != null) {
				AddItems(items);
			}
			Manager = manager;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override LinksInfo LinksPersistInfo {
			get { return null; }
			set { }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public override BarItemLinkCollection ItemLinks { get { return base.ItemLinks; } }
		protected override void ItemsCollectionChanged() {
			base.ItemsCollectionChanged();
			foreach(BarItemLink link in Links) {
				RibbonMiniToolbarItemLinkCollection toolbarCollection = link.Holder as RibbonMiniToolbarItemLinkCollection;
				if(toolbarCollection != null && toolbarCollection.Toolbar != null && toolbarCollection.Toolbar.ToolbarControl != null) {
					toolbarCollection.Toolbar.ToolbarControl.Refresh();
				}
			}
		}
		protected override BarItemLinkCollection CreateLinkCollection() {
			return new ButtonGroupItemLinkCollection(this);
		}
		[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
		protected class ButtonGroupItemLinkCollection : BarItemLinkCollection {
			public ButtonGroupItemLinkCollection(BarButtonGroup buttonGroup) : base(buttonGroup  ) { }
			BarButtonGroup Item { get { return (BarButtonGroup)Owner; } }
			protected override bool OnInsert(int index, object item) {
				if(!base.OnInsert(index, item)) return false;
				BarItemLink link = item as BarItemLink;
				if(link != null && link.Item != null && link.Item.Manager == null)
					link.Item.Manager = Manager;
				return true;
			}
		}
		protected internal virtual bool ShouldCreateViewInfo() {
			if(Visibility == BarItemVisibility.Never && !DesignMode) return false;
			for(int i = 0; i < ItemLinks.Count; i++) {
				if(!ItemLinks[i].CanVisible) continue;
				if(ItemLinks[i].Item.Visibility != BarItemVisibility.Never) return true;
			}
			return this.DesignMode;
		}
		ButtonGroupsLayout buttonGroupsLayout = ButtonGroupsLayout.Default;
		[DefaultValue(ButtonGroupsLayout.Default)]
		public ButtonGroupsLayout ButtonGroupsLayout {
			get { return buttonGroupsLayout; }
			set {
				if(ButtonGroupsLayout == value)
					return;
				buttonGroupsLayout = value;
				OnItemChanged();
			}
		}
	}
	#region class Strings
	[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
	public class Strings : IList {
		ArrayList data;
		public event EventHandler Changed;
		public Strings() {
			data = new ArrayList();
		}
		int InternalAdd(object item) {
			if(item is string) {
				int index = data.Add(item);
				DataChanged();
				return index;
			}
			return -1;
		}
		int IList.Add(object item) {
			return InternalAdd(item);
		}
		bool IList.Contains(object item) {
			return Contains(item as string);
		}
		int IList.IndexOf(object item) {
			return IndexOf(item as string); 
		}
		bool IList.IsFixedSize {
			get { return data.IsFixedSize; }
		}
		bool IList.IsReadOnly { 
			get { return data.IsReadOnly; } 
		}
		object IList.this[int index] {
			get { return this[index];}
			set { this[index] = value as string;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			data.CopyTo(array, index);
		}
		bool ICollection.IsSynchronized { 
			get { return data.IsSynchronized; }
		}
		object ICollection.SyncRoot { 
			get { return data.SyncRoot; } 
		}
		void IList.Remove(object item) {
			Remove(item as string);
		}
		void IList.Insert(int index, object item) {
			Insert(index, item as string);
		}
		void IList.RemoveAt(int index) {
			RemoveAt(index);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("StringsItem")]
#endif
		public virtual string this[int index] {
			get { return data[index] as string; }
			set {
				data[index] = value;
				DataChanged();
			}
		}
		public virtual IEnumerator GetEnumerator() {
			return data.GetEnumerator();
		}
		public virtual int Add(string item) {
			return InternalAdd(item);
		}
		public virtual void Remove(string item) {
			data.Remove(item);
			DataChanged();
		}
		public virtual void RemoveAt(int index) {
			data.RemoveAt(index);
			DataChanged();
		}
		public virtual bool Contains(string item) {
			return data.Contains(item);
		}
		public virtual int IndexOf(string item) {
			return data.IndexOf(item);
		}
		public virtual void Clear() {
			data.Clear();
			DataChanged();
		}
		public virtual void Insert(int index, string item) {
			data.Insert(index, item);
			DataChanged();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("StringsCount")]
#endif
		public virtual int Count {
			get { return data.Count; }
		}
		public virtual void AddRange(object[] items) {
			foreach(object item in items) {
				InternalAdd(item);
			}
		}
		protected void DataChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}
	#endregion
	#region class SkinBarSubItem
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class SkinBarSubItem : BarSubItem {
		public SkinBarSubItem() : base() {
		}
		public void Initialize() {
			OnInitialize();
		}
		protected internal virtual void OnInitialize() {
			SkinHelper.InitSkinPopupMenu(this);
		}
	}
#endregion
}
