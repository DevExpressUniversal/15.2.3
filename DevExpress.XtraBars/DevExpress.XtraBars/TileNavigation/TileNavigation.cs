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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Navigation;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.Utils.Controls;
using System.ComponentModel.Design;
namespace DevExpress.XtraBars.Navigation {
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "OfficeNavigationBar")]
	[ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[Description("An Outlook-inspired navigation bar, supporting integration with the NavBarControl.")]
	[Designer("DevExpress.XtraBars.Design.OfficeNavigationBarDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner))]
	[DXToolboxItem(true)]
	public class OfficeNavigationBar : Control, IButtonsPanelOwnerEx, IAppearanceOwner, INavigationBar, ISupportInitialize, IXtraResizableControl,
		IToolTipControlClient {
		public OfficeNavigationBar() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
			this.itemPadding = DefaultItemPadding;
			this.lookAndFeel = new UserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += OnLookAndFeelChanged;
			this.maxItemCount = maxItemCountDefault;
			this.ShowPeekFormOnItemHover = true;
			this.PeekFormShowDelay = defaultPeekFormShowDelay;
			this.animateItemPress = true;
			this.allowSelectedItemCore = true;
			base.AutoSize = true;
			this.showToolTips = true;
			this.itemsCore = CreateItems();
			Items.CollectionChanged += Items_CollectionChanged;
			this.clientItemsCore = CreateItems();
			ClientItems.CollectionChanged += Items_CollectionChanged;
			this.appearanceItem = new TileItemAppearances(this);
			AppearanceItem.Changed += OnAppearanceItemChanged;
			this.buttonsPanel = new NavigationBarButtonsPanel(this);
			EnsureDefaultGroup();
			this.buttonsCore = new ButtonCollection(ButtonsPanel);
			ButtonsPanel.Buttons.Add(new NavigationBarCustomizationButton(this));
			SubscribeButtonsPanel();
			ToolTipController.DefaultController.AddClientControl(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyItemMenu();
				RemoveTooltipControllers();
				Items.CollectionChanged -= Items_CollectionChanged;
				UnsubscribeButtonsPanel();
				if(IsNavigationClientAttached)
					UnsubscribeNavigationClient();
				if(itemsSourceItems != null)
					itemsSourceItems.Clear();
				navigationClientCore = null;
			}
			base.Dispose(disposing);
		}
		bool autoSizeInLayoutControl = false;
		[DefaultValue(false), DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All)]
		public virtual bool AutoSizeInLayoutControl {
			get { return autoSizeInLayoutControl; }
			set {
				if(autoSizeInLayoutControl == value) return;
				autoSizeInLayoutControl = value;
				RaiseResizableChanged();
			}
		}
		protected override Size DefaultSize {
			get { return new Size(250, 20); }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(true)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set {
				if(base.AutoSize == value) return;
				if(initLock == 0) Dock = GetDockByAutoSize(value);
				base.AutoSize = value;
				OnPropertiesChanged();
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(this.AutoSize && (specified & BoundsSpecified.Size) != 0) {
				Size size = GetLayoutMinSize();
				if(!size.IsEmpty) {
					if(Orientation == Orientation.Horizontal)
						height = size.Height;
					else
						width = size.Width;
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		DockStyle GetDockByAutoSize(bool autosize) {
			if(!autosize) return Dock;
			if(Dock == DockStyle.Fill) return DockStyle.None;
			if(Orientation == Orientation.Horizontal) {
				if(Dock == DockStyle.Left || Dock == DockStyle.Right) return DockStyle.None;
			}
			else {
				if(Dock == DockStyle.Top || Dock == DockStyle.Bottom) return DockStyle.None;
			}
			return Dock;
		}
		public override DockStyle Dock {
			get { return base.Dock; }
			set {
				if(base.Dock == value) return;
				if(initLock == 0) AutoSize = GetAutoSizeByDock(value);
				base.Dock = value;
			}
		}
		bool GetAutoSizeByDock(DockStyle dockStyle) {
			if(!AutoSize) return false;
			switch(dockStyle) {
				case DockStyle.Fill: return false;
				case DockStyle.Left:
				case DockStyle.Right: return Orientation == Orientation.Vertical;
				case DockStyle.Bottom:
				case DockStyle.Top: return Orientation == Orientation.Horizontal;
				default: return AutoSize;
			}
		}
		protected internal Size ContentBounds {
			get { return ControlCore.ViewInfoCore.ContentBounds.Size; }
		}
		bool allowSelectedItemCore;
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool AllowItemSelection {
			get { return allowSelectedItemCore; }
			set {
				if(allowSelectedItemCore == value) return;
				allowSelectedItemCore = value;
				OnPropertiesChanged();
			}
		}
		bool allowItemSkinnig;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool ItemSkinning {
			get { return allowItemSkinnig; }
			set {
				if(allowItemSkinnig == value) return;
				allowItemSkinnig = value;
				OnPropertiesChanged();
				if(value)
					OnLookAndFeelChanged(this, EventArgs.Empty);
				else
					OnPropertiesChanged();
			}
		}
		bool animateItemPress;
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool AnimateItemPressing {
			get { return animateItemPress; }
			set {
				if(animateItemPress == value) return;
				animateItemPress = value;
			}
		}
		NavigationBarItem selectedItem = null;
		[DefaultValue(null), Category(CategoryName.Properties)]
		public NavigationBarItem SelectedItem {
			get { return selectedItem; }
			set {
				if(!RaiseSelectedItemChanging(SelectedItem, value)) return;
				if(SelectedItem == value)
					return;
				NavigationBarItem oldItem = SelectedItem;
				selectedItem = value;
				if(IsNavigationClientAttached) {
					var prevClientItem = NavigationClient.SelectedItem;
					var newClientItem = GetItem(value);
					if(newClientItem != prevClientItem) {
						NavigationClient.SelectedItem = newClientItem;
						bool canceledByClient = NavigationClient.SelectedItem == prevClientItem;
						if(canceledByClient) {
							selectedItem = oldItem;
							return;
						}
					}
				}
				OnSelectedItemChanged(oldItem, value);
			}
		}
		void OnSelectedItemChanged(NavigationBarItem oldItem, NavigationBarItem newItem) {
			RaiseSelectedItemChanged(newItem);
			InvalidateSelectedItem(oldItem, newItem);
		}
		void InvalidateSelectedItem(NavigationBarItem oldItem, NavigationBarItem newItem) {
			if(oldItem != null && oldItem.Tile.ItemInfo != null && oldItem.Tile.Visible)
				oldItem.Tile.ItemInfo.ForceUpdateAppearanceColors();
			if(newItem != null && newItem.Tile.ItemInfo != null) {
				newItem.Tile.ItemInfo.ForceUpdateAppearanceColors();
				if(!newItem.Tile.ItemInfo.IsFullyVisible)
					ControlCore.ViewInfoCore.MakeVisible(newItem.Tile.ItemInfo);
			}
			Invalidate(ClientRectangle);
		}
		protected override void OnPaddingChanged(EventArgs e) {
			OnPropertiesChanged();
		}
		protected void SubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick += OnButtonClick;
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			CustomButtons.CollectionChanged += OnCustomButtonsCollectionChanged;
		}
		protected void UnsubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick -= OnButtonClick;
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			CustomButtons.CollectionChanged -= OnCustomButtonsCollectionChanged;
		}
		void OnCustomButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnPropertiesChanged();
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		void OnButtonClick(object sender, ButtonEventArgs e) {
			if(e.Button is NavigationBarCustomizationButton)
				OnCustomizeButtonClick();
			else
				OnButtonClick(e);
		}
		void OnButtonClick(ButtonEventArgs e) {
			ButtonEventHandler handler = Events[buttonClick] as ButtonEventHandler;
			if(handler != null) handler(this, e);
		}
		void OnCustomizeButtonClick() { }
		public void ShowNavigationOptionsDialog() {
			NavigationOptionsForm optionForm = new NavigationOptionsForm();
			optionForm.OfficeNavigationBar = this;
			if(optionForm.ShowDialog() == DialogResult.OK) {
				UpdateItems(optionForm.ResultItems, false);
				MaxItemCount = optionForm.ResultVisibleCount;
			}
		}
		void UpdateItems(List<NavigationBarItem> newCollection, bool keepInfosAlive) {
			Items.BeginUpdate();
			foreach(NavigationBarItem i in newCollection)
				Items.Remove(i);
			foreach(NavigationBarItem i in newCollection)
				Items.Add(i);
			KeepItemInfosAlive = keepInfosAlive;
			Items.EndUpdate();
			KeepItemInfosAlive = false;
		}
		protected internal void SyncItemsAndVisibleItems() {
			if(ControlCore.Groups.Count == 0) return;
			List<NavigationBarItem> reorderedItems = new List<NavigationBarItem>();
			foreach(NavigationBarItemCore itemInfo in ControlCore.Groups[0].Items)
				reorderedItems.Add(itemInfo.OwnerItem);
			UpdateItems(reorderedItems, true);
		}
		protected internal bool ShowCustomizationMenu(Point pt) {
			if(CustomizationMenu != null)
				CloseCustomizationMenu();
			this.customizationMenuCore = CreateCustomizationMenu();
			CustomizationMenu.CloseUp += CustomizationMenu_CloseUp;
			if(RaisePopupMenuShowing(CustomizationMenu)) {
				MenuManagerHelper.ShowMenu(CustomizationMenu, LookAndFeel, GetMenuManager(), this, pt);
				return true;
			}
			else DestroyCustomizationMenu();
			return false;
		}
		protected virtual NavigationBarMenu CreateCustomizationMenu() {
			var menu = new NavigationBarMenu();
			var optionsItem = new NavigationOptionsMenuItem(this);
			menu.Items.Add(optionsItem);
			var hiddenItems = GetHiddenItems();
			if(hiddenItems.Count > 0) {
				foreach(var hidden in hiddenItems) {
					var menuItem = new NavigationMenuItem(hidden);
					menuItem.BeginGroup = hiddenItems.IndexOf(hidden) == 0;
					menu.Items.Add(menuItem);
				}
			}
			return menu;
		}
		protected virtual List<NavigationBarItem> GetHiddenItems() {
			var res = new List<NavigationBarItem>();
			if(ControlCore.ViewInfoCore != null) {
				foreach(TileGroup group in ControlCore.Groups) {
					foreach(var item in group.Items) { 
						var itemCore = item as NavigationBarItemCore;
						if(itemCore == null || itemCore.OwnerItem == null) continue;
						if(!ControlCore.ViewInfoCore.CheckItemIsVisible(itemCore))
							res.Add(itemCore.OwnerItem);
					}
				}
			}
			return res;
		}
		public class NavigationMenuItem : DXMenuItem {
			public NavigationBarItem Item { get; private set; }
			public OfficeNavigationBar Owner {
				get { return Item != null && Item.Collection != null ? Item.Collection.Owner : null; }
			}
			public NavigationMenuItem(NavigationBarItem item) {
				this.Caption = item.Text;
				this.Item = item;
			}
			protected override void OnClick() {
				base.OnClick();
				if(this.Item != null)
					this.Item.Select();
			}
		}
		class NavigationOptionsMenuItem : DXMenuItem {
			OfficeNavigationBar Owner;
			public NavigationOptionsMenuItem(OfficeNavigationBar owner) {
				this.Caption = "Navigation Options";
				this.Owner = owner;
			}
			protected override void OnClick() {
				base.OnClick();
				if(Owner != null)
					Owner.ShowNavigationOptionsDialog();
			}
		}
		NavigationBarMenu customizationMenuCore;
		protected NavigationBarMenu CustomizationMenu {
			get { return customizationMenuCore; }
		}
		void CloseCustomizationMenu() {
			if(CustomizationMenu == null) return;
			CustomizationMenu.HidePopup();
			DestroyCustomizationMenu();
		}
		private void DestroyCustomizationMenu() {
			if(CustomizationMenu != null) {
				CustomizationMenu.CloseUp -= CustomizationMenu_CloseUp;
				CustomizationMenu.Dispose();
			}
			this.customizationMenuCore = null;
		}
		void CustomizationMenu_CloseUp(object sender, EventArgs e) {
			if(sender != CustomizationMenu) return;
			CloseCustomizationMenu();
		}
		NavigationBarItemCollection itemsCore;
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraBars.Design.NavigationBarItemCollectionEditor," + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor)),
		Category("Items")]
		public NavigationBarItemCollection Items {
			get { return itemsCore; }
		}
		NavigationBarItemCollection clientItemsCore;
		NavigationBarItemCollection ClientItems {
			get { return clientItemsCore; }
		}
		protected virtual NavigationBarItemCollection CreateItems() {
			return new NavigationBarItemCollection(this);
		}
		void Items_CollectionChanged(object sender, EventArgs e) {
			FireChanged();
			if(KeepItemInfosAlive) return;
			TileGroup group = EnsureDefaultGroup();
			group.Items.Clear();
			foreach(NavigationBarItem item in Items) {
				group.Items.Add(item.Tile);
			}
			foreach(NavigationBarItem clientItem in ClientItems) {
				group.Items.Add(clientItem.Tile);
			}
		}
		TileGroup EnsureDefaultGroup() {
			if(ControlCore.Groups.Count == 0)
				ControlCore.Groups.Add(new TileGroup());
			return ControlCore.Groups[0];
		}
		ButtonCollection buttonsCore;
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraBars.Docking.Design.CustomHeaderButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Items")]
		internal ButtonCollection CustomButtons {
			get { return buttonsCore; }
		}
		bool ShouldSerializeCustomButtons() {
			return (buttonsCore != null) && buttonsCore.Count > 0;
		}
		int maxItemCount;
		const int maxItemCountDefault = -1;
		[DefaultValue(maxItemCountDefault), Category(CategoryName.Behavior)]
		public int MaxItemCount {
			get { return maxItemCount; }
			set {
				if(maxItemCount == value)
					return;
				maxItemCount = value;
				OnPropertiesChanged();
			}
		}
		bool compact;
		[DefaultValue(false), Category(CategoryName.Behavior)]
		protected internal bool Compact {
			get { return compact; }
			set {
				if(compact == value) return;
				compact = value;
				OnCompactChanged();
			}
		}
		protected virtual void OnCompactChanged() {
			if(IsNavigationClientAttached) {
				var officeNavigationBarClient = NavigationClient as IOfficeNavigationBarClient;
				if(officeNavigationBarClient != null)
					officeNavigationBarClient.Compact = Compact;
			}
			OnPropertiesChanged();
		}
		NavigationBarButtonsPanel buttonsPanel;
		protected internal NavigationBarButtonsPanel ButtonsPanel {
			get { return buttonsPanel; }
		}
		ObjectPainter buttonsPanelPainter;
		protected internal ObjectPainter ButtonsPanelPainter {
			get {
				if(buttonsPanelPainter == null)
					buttonsPanelPainter = GetButtonsPanelPainter();
				return buttonsPanelPainter;
			}
		}
		protected virtual ObjectPainter GetButtonsPanelPainter() {
			return new NavigationBarButtonsPanelPainter(LookAndFeel);
		}
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool ShowPeekFormOnItemHover { get; set; }
		[Category(CategoryName.Behavior)]
		public Size PeekFormSize { get; set; }
		bool ShouldSerializePeekFormSize() {
			return PeekFormSize != Size.Empty;
		}
		void ResetPeekFormSize() {
			PeekFormSize = Size.Empty;
		}
		const int defaultPeekFormShowDelay = 1500;
		[DefaultValue(defaultPeekFormShowDelay), Category(CategoryName.Behavior)]
		public int PeekFormShowDelay { get; set; }
		CustomizationButtonVisibility buttonVisibility = CustomizationButtonVisibility.ShowAfterItems;
		public CustomizationButtonVisibility CustomizationButtonVisibility {
			get { return buttonVisibility; }
			set {
				if(buttonVisibility == value) return;
				buttonVisibility = value;
				OnPropertiesChanged();
			}
		}
		void ResetCustomizationButtonVisibility() {
			CustomizationButtonVisibility = CustomizationButtonVisibility.ShowAfterItems;
		}
		bool ShouldSerializeCustomizationButtonVisibility() {
			return CustomizationButtonVisibility != Navigation.CustomizationButtonVisibility.ShowAfterItems;
		}
		protected override void OnLocationChanged(EventArgs e) {
			base.OnLocationChanged(e);
			ControlCore.ViewInfoCore.UpdateVisualEffects();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			OnPropertiesChanged();
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			CheckParentColors();
			CheckViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ControlCore.Painter.Draw(new TileControlInfoArgs(cache, ControlCore.ViewInfoCore));
			}
			RaisePaintEvent(this, e);
		}
		protected virtual void CheckParentColors() {
			ControlCore.ViewInfoCore.CheckParentColors();
		}
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			buttonsPanelPainter = null;
			ControlCore.ViewInfoCore.ClearBorderPainter();
			ControlCore.ViewInfoCore.ForceUpdateItemsAppearances();
			OnPropertiesChanged();
		}
		public void OnPropertiesChanged() {
			if(IsLockUpdate)
				return;
			ControlCore.ViewInfoCore.SetDirty();
			Invalidate();
			RaiseResizableChanged();
		}
		void RaiseResizableChanged() {
			EventHandler handler = (EventHandler)Events[resizableChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		int lockUpdate;
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0)
				OnPropertiesChanged();
		}
		[Browsable(false)]
		public bool IsLockUpdate {
			get { return lockUpdate > 0; }
		}
		NavigationBarCore controlCore;
		internal NavigationBarCore ControlCore {
			get {
				if(controlCore == null)
					controlCore = new NavigationBarCore(this);
				return controlCore;
			}
		}
		void CheckViewInfo() {
			if(ControlCore.ViewInfoCore.IsReady) return;
			ControlCore.ViewInfoCore.CalcViewInfo(ClientRectangle);
		}
		bool isRightToLeft = false;
		[Browsable(false)]
		public bool IsRightToLeft { get { return isRightToLeft; } }
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnPropertiesChanged();
		}
		[DefaultValue(false), Category(CategoryName.Behavior)]
		public bool AllowDrag { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDesignMode { get; set; }
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		TileItemAppearances appearanceItem;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get { return appearanceItem; }
		}
		protected virtual void OnAppearanceItemChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		internal static Padding DefaultItemPadding { get { return new Padding(12, 8, 12, 8); } }
		void ResetItemPadding() { ItemPadding = DefaultItemPadding; }
		bool ShouldSerializeItemPadding() { return ItemPadding != DefaultItemPadding; }
		Padding itemPadding;
		[Category(CategoryName.Appearance)]
		public Padding ItemPadding {
			get { return itemPadding; }
			set {
				if(ItemPadding == value)
					return;
				itemPadding = value;
				OnPropertiesChanged();
			}
		}
		UserLookAndFeel lookAndFeel;
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
		}
		[DefaultValue(null), Category(XtraEditors.CategoryName.Appearance)]
		internal object Images { get; set; }
		bool allowHtmlDraw;
		[DefaultValue(false), Category(XtraEditors.CategoryName.Appearance)]
		public bool AllowHtmlDraw {
			get { return allowHtmlDraw; }
			set {
				if(allowHtmlDraw == value) return;
				allowHtmlDraw = value;
				OnPropertiesChanged();
			}
		}
		HorzAlignment horzContentAlignment = HorzAlignment.Default;
		[DefaultValue(HorzAlignment.Default), Category(XtraEditors.CategoryName.Appearance)]
		public HorzAlignment HorizontalContentAlignment {
			get { return horzContentAlignment; }
			set {
				if(horzContentAlignment == value) return;
				horzContentAlignment = value;
				OnPropertiesChanged();
			}
		}
		Orientation orientation;
		[DefaultValue(Orientation.Horizontal), Category(CategoryName.Appearance)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(Orientation == value)
					return;
				orientation = value;
				OnOrientationChanged();
			}
		}
		protected virtual void OnOrientationChanged() {
			if(initLock == 0) AutoSize = GetAutoSizeByOrientation(Orientation);
			ButtonsPanel.Orientation = Orientation;
			OnPropertiesChanged();
		}
		bool GetAutoSizeByOrientation(Orientation orient) {
			if(!AutoSize) return false;
			if(orient == Orientation.Horizontal)
				return Dock == DockStyle.Top || Dock == DockStyle.Bottom;
			else
				return Dock == DockStyle.Left || Dock == DockStyle.Right;
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			CheckViewInfo();
			ControlCore.Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			CheckViewInfo();
			ControlCore.Handler.OnMouseLeave(e);
			if(!Capture)
				ButtonsPanel.Handler.OnMouseLeave();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			CheckViewInfo();
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			ControlCore.Handler.OnMouseDown(ea);
			ButtonsPanel.Handler.OnMouseDown(ea);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			CheckViewInfo();
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			ControlCore.Handler.OnMouseMove(ea);
			if(!Capture)
				ButtonsPanel.Handler.OnMouseMove(ea);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			CheckViewInfo();
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			ControlCore.Handler.OnMouseUp(ea);
			ButtonsPanel.Handler.OnMouseUp(ea);
		}
		static readonly object itemClick = new object();
		static readonly object buttonClick = new object();
		static readonly object selectedItemChanged = new object();
		static readonly object selectedItemChanging = new object();
		static readonly object queryControl = new object();
		static readonly object registerItem = new object();
		static readonly object unregisterItem = new object();
		static readonly object popupMenuShowing = new object();
		static readonly object peekFormButtonClick = new object();
		static readonly object peekFormShown = new object();
		static readonly object peekFormHidden = new object();
		[Category(CategoryName.Behavior)]
		public event NavigationBarItemClickEventHandler ItemClick {
			add { this.Events.AddHandler(itemClick, value); }
			remove { this.Events.RemoveHandler(itemClick, value); }
		}
		[Category(CategoryName.Behavior)]
		internal event ButtonEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event NavigationBarItemClickEventHandler SelectedItemChanged {
			add { Events.AddHandler(selectedItemChanged, value); }
			remove { Events.RemoveHandler(selectedItemChanged, value); }
		}
		[Category(CategoryName.Behavior)]
		public event NavigationBarItemCancelEventHandler SelectedItemChanging {
			add { Events.AddHandler(selectedItemChanging, value); }
			remove { Events.RemoveHandler(selectedItemChanging, value); }
		}
		[Category(CategoryName.BarManager)]
		public event NavigationBarPopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(popupMenuShowing, value); }
			remove { Events.RemoveHandler(popupMenuShowing, value); }
		}
		[Category(CategoryName.Layout)]
		public event QueryPeekFormContentEventHandler QueryPeekFormContent {
			add { Events.AddHandler(queryControl, value); }
			remove { Events.RemoveHandler(queryControl, value); }
		}
		[Category("Navigation Client")]
		public event NavigationBarNavigationClientItemEventHandler RegisterItem {
			add { Events.AddHandler(registerItem, value); }
			remove { Events.RemoveHandler(registerItem, value); }
		}
		[Category("Navigation Client")]
		public event NavigationBarNavigationClientItemEventHandler UnregisterItem {
			add { Events.AddHandler(unregisterItem, value); }
			remove { Events.RemoveHandler(unregisterItem, value); }
		}
		[Category(CategoryName.Behavior)]
		public event EventHandler<NavigationPeekFormButtonClickEventArgs> PeekFormButtonClick {
			add { Events.AddHandler(peekFormButtonClick, value); }
			remove { Events.RemoveHandler(peekFormButtonClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event EventHandler<NavigationPeekFormEventArgs> PeekFormShown {
			add { Events.AddHandler(peekFormShown, value); }
			remove { Events.RemoveHandler(peekFormShown, value); }
		}
		[Category(CategoryName.Behavior)]
		public event EventHandler<NavigationPeekFormEventArgs> PeekFormHidden {
			add { Events.AddHandler(peekFormHidden, value); }
			remove { Events.RemoveHandler(peekFormHidden, value); }
		}
		protected internal void RaiseItemClick(NavigationBarItem item) {
			NavigationBarItemClickEventHandler handler = (NavigationBarItemClickEventHandler)Events[itemClick];
			if(handler != null)
				handler(this, new NavigationBarItemEventArgs(item));
		}
		protected internal bool RaiseSelectedItemChanging(NavigationBarItem oldItem, NavigationBarItem newItem) {
			NavigationBarItemCancelEventHandler handler = (NavigationBarItemCancelEventHandler)Events[selectedItemChanging];
			SelectedItemChangingEventArgs e = new SelectedItemChangingEventArgs() { PreviousItem = oldItem, Item = newItem };
			if(handler != null)
				handler(this, e);
			return !e.Cancel;
		}
		protected void RaiseSelectedItemChanged(NavigationBarItem item) {
			NavigationBarItemEventArgs e = new NavigationBarItemEventArgs(item);
			NavigationBarItemClickEventHandler handler = Events[selectedItemChanged] as NavigationBarItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal Control RaiseQueryControl(NavigationBarItem item) {
			QueryPeekFormContentEventHandler handler = (QueryPeekFormContentEventHandler)Events[queryControl];
			QueryPeekFormContentEventArgs e = new QueryPeekFormContentEventArgs(item);
			if(handler != null)
				handler(this, e);
			return e.Control;
		}
		protected internal bool RaisePopupMenuShowing(NavigationBarMenu menu) {
			var e = new NavigationBarPopupMenuShowingEventArgs((NavigationBarItem)null) { Menu = menu, MenuKind = NavigationBarMenuKind.CustomizationButton };
			NavigationBarPopupMenuShowingEventHandler handler = Events[popupMenuShowing] as NavigationBarPopupMenuShowingEventHandler;
			if(handler != null)
				handler(this, e);
			return !e.Cancel;
		}
		protected internal bool RaisePopupMenuShowing(NavigationBarMenu menu, NavigationBarItem item) {
			var e = new NavigationBarPopupMenuShowingEventArgs(item) { Menu = menu, MenuKind = NavigationBarMenuKind.Item };
			NavigationBarPopupMenuShowingEventHandler handler = Events[popupMenuShowing] as NavigationBarPopupMenuShowingEventHandler;
			if(handler != null)
				handler(this, e);
			return !e.Cancel;
		}
		protected internal void RaiseRegisterItem(NavigationBarItem item, INavigationItem navigationItem) {
			NavigationBarNavigationClientItemEventHandler handler = (NavigationBarNavigationClientItemEventHandler)Events[registerItem];
			if(handler != null)
				handler(this, new NavigationBarNavigationClientItemEventArgs(item, navigationItem));
		}
		protected internal void RaiseUnregisterItem(NavigationBarItem item, INavigationItem navigationItem) {
			NavigationBarNavigationClientItemEventHandler handler = (NavigationBarNavigationClientItemEventHandler)Events[unregisterItem];
			if(handler != null)
				handler(this, new NavigationBarNavigationClientItemEventArgs(item, navigationItem));
		}
		protected internal void RaisePeekFormButtonClick(NavigationPeekFormButtonClickEventArgs args) {
			EventHandler<NavigationPeekFormButtonClickEventArgs> handler = (EventHandler<NavigationPeekFormButtonClickEventArgs>)Events[peekFormButtonClick];
			if(handler != null) handler(this, args);
		}
		protected internal void RaisePeekFormShown(NavigationPeekFormEventArgs args) {
			EventHandler<NavigationPeekFormEventArgs> handler = (EventHandler<NavigationPeekFormEventArgs>)Events[peekFormShown];
			if(handler != null) handler(this, args);
		}
		protected internal void RaisePeekFormHidden(NavigationPeekFormEventArgs args) {
			EventHandler<NavigationPeekFormEventArgs> handler = (EventHandler<NavigationPeekFormEventArgs>)Events[peekFormHidden];
			if(handler != null) handler(this, args);
		}
		#region Popups
		IDXMenuManager menuManagerCore;
		[Category(CategoryName.Appearance), DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManagerCore; }
			set { menuManagerCore = value; }
		}
		IDXMenuManager GetMenuManager() {
			return MenuManager ?? MenuManagerHelper.GetMenuManager(LookAndFeel);
		}
		NavigationBarMenu itemMenuCore;
		protected NavigationBarMenu ItemMenu {
			get { return itemMenuCore; }
		}
		protected internal bool IsMenuOpen {
			get { return itemMenuCore != null || customizationMenuCore != null; }
		}
		protected internal bool ShowItemMenu(NavigationBarItem item, Point pt) {
			if(ItemMenu != null)
				CloseItemMenu();
			this.itemMenuCore = CreateItemMenu(item);
			ItemMenu.CloseUp += OnItemMenuCloseUp;
			if(RaisePopupMenuShowing(ItemMenu, item)) {
				if(ItemMenu.Items.Count == 0) {
					DestroyItemMenu();
					return false;
				}
				MenuManagerHelper.ShowMenu(ItemMenu, LookAndFeel, GetMenuManager(), this, pt);
				return true;
			}
			else DestroyItemMenu();
			return false;
		}
		protected virtual NavigationBarMenu CreateItemMenu(NavigationBarItem item) {
			return new NavigationBarMenu();
		}
		void DestroyItemMenu() {
			if(ItemMenu != null) {
				ItemMenu.CloseUp -= OnItemMenuCloseUp;
				ItemMenu.Dispose();
			}
			this.itemMenuCore = null;
		}
		void CloseItemMenu() {
			if(ItemMenu == null) return;
			ItemMenu.HidePopup();
			DestroyItemMenu();
		}
		void OnItemMenuCloseUp(object sender, EventArgs e) {
			if(sender != ItemMenu)
				return;
			CloseItemMenu();
		}
		#endregion
		#region IButtonsPanelOwner
		Padding IButtonsPanelOwnerEx.ButtonBackgroundImageMargin { get { return new Padding(0); } set { } }
		bool IButtonsPanelOwnerEx.CanPerformClick(IBaseButton button) { return true; }
		bool IButtonsPanelOwner.AllowGlyphSkinning { get { return true; } }
		bool IButtonsPanelOwner.AllowHtmlDraw { get { return this.AllowHtmlDraw; } }
		XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton { get { return null; } }
		object IButtonsPanelOwner.ButtonBackgroundImages { get { return null; } }
		bool IButtonsPanelOwner.Enabled { get { return true; } }
		ObjectPainter IButtonsPanelOwner.GetPainter() { return ButtonsPanelPainter; }
		object IButtonsPanelOwner.Images { get { return null; } }
		void IButtonsPanelOwner.Invalidate() {
			if(ButtonsPanel != null && ButtonsPanel.ViewInfo != null) {
				Invalidate(ButtonsPanel.ViewInfo.Bounds);
			}
		}
		bool IButtonsPanelOwner.IsSelected { get { return false; } }
		#endregion
		bool IAppearanceOwner.IsLoading { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavigationBarItem PeekItem {
			get { return (flyout != null) ? Flyout.GetItem() : null; }
		}
		public void HidePeekForm() {
			Flyout.HideBeakForm(true);
			Flyout.Tag = null;
		}
		public void ShowPeekForm(NavigationBarItem item) {
			if(item != null && item.Tile != null && controlCore != null)
				item.Tile.ShowPeekForm(ControlCore);
		}
		internal void ShowPeekForm(Control content, Rectangle itemRect, NavigationBarItem item) {
			if(Flyout.Tag == item) return;
			Flyout.HideBeakForm(true);
			Flyout.Controls.Clear();
			Flyout.OwnerControl = this;
			Flyout.ParentForm = this.FindForm();
			Flyout.Tag = item;
			var clientSize = Size.Empty;
			if(!PeekFormSize.IsEmpty)
				clientSize = PeekFormSize;
			else
				clientSize = content.Size;
			if(OptionsPeekFormButtonPanel.ShowButtonPanel)
				clientSize = new Size(clientSize.Width, clientSize.Height + OptionsPeekFormButtonPanel.ButtonPanelHeight);
			Flyout.ClientSize = clientSize;
			content.Dock = DockStyle.Fill;
			content.Parent = Flyout;
			Flyout.ShowBeakForm(GetPeekLocation(itemRect, this.Orientation, this.Dock));
		}
		Point GetPeekLocation(Rectangle itemRect, Orientation orientation, DockStyle dockStyle) {
			int centerX = itemRect.X + (itemRect.Width / 2);
			int centerY = itemRect.Y + (itemRect.Height / 2);
			Point defaultPoint = new Point(centerX, itemRect.Y);
			switch(dockStyle) {
				case DockStyle.Bottom:
					return defaultPoint;
				case DockStyle.Top:
					return new Point(centerX, itemRect.Bottom);
				case DockStyle.Left:
					return orientation == Orientation.Horizontal ?
						defaultPoint : new Point(itemRect.Right, centerY);
				case DockStyle.Right:
					return orientation == Orientation.Horizontal ?
						defaultPoint : new Point(itemRect.Left, centerY);
				default: return defaultPoint;
			}
		}
		NavigationFlyoutPanel flyout;
		NavigationFlyoutPanel Flyout {
			get {
				if(flyout == null || flyout.IsDisposed)
					flyout = CreateFlyoutPanel();
				return flyout;
			}
		}
		protected virtual NavigationFlyoutPanel CreateFlyoutPanel() {
			return new NavigationFlyoutPanel(this);
		}
		protected class NavigationFlyoutPanel : FlyoutPanel {
			OfficeNavigationBar owner;
			public NavigationFlyoutPanel(OfficeNavigationBar owner) {
				this.owner = owner;
			}
			protected override void OnShown(FlyoutPanelEventArgs e) {
				base.OnShown(e);
				owner.RaisePeekFormShown((NavigationPeekFormEventArgs)e);
			}
			protected override void OnHidden(FlyoutPanelEventArgs e) {
				base.OnHidden(e);
				owner.RaisePeekFormHidden((NavigationPeekFormEventArgs)e);
				Tag = null;
			}
			protected override void OnButtonClick(FlyoutPanelButtonClickEventArgs e) {
				base.OnButtonClick(e);
				owner.RaisePeekFormButtonClick((NavigationPeekFormButtonClickEventArgs)e);
			}
			protected internal NavigationBarItem GetItem() {
				return Tag as NavigationBarItem;
			}
			protected internal Control GetControl() {
				if(Controls.Count == 0) return null;
				return Controls[0];
			}
			protected override FlyoutPanelEventArgs CreateFlyoutPanelEventArgs() {
				return new NavigationPeekFormEventArgs(GetItem(), GetControl());
			}
			protected override FlyoutPanelButtonClickEventArgs CreateFlyoutPanelButtonClickEventArgs(PeekFormButton button) {
				return new NavigationPeekFormButtonClickEventArgs(button, Tag as NavigationBarItem);
			}
		}
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FlyoutPanelButtonOptions OptionsPeekFormButtonPanel {
			get { return Flyout.OptionsButtonPanel; }
		}
		bool ShouldSerializeOptionsPeekFormButtonPanel() { return OptionsPeekFormButtonPanel.ShouldSerialize(); }
		void ResetOptionsPeekFormButtonPanel() { OptionsPeekFormButtonPanel.Reset(); }
		#region INavigationBarClient integration
		INavigationBarClient navigationClientCore;
		[DefaultValue(null), Category("Navigation Client")]
		public INavigationBarClient NavigationClient {
			get { return navigationClientCore; }
			set {
				if(navigationClientCore == value) return;
				if(IsNavigationClientAttached) {
					OnNavigationClientDetached();
					UnsubscribeNavigationClient();
				}
				this.navigationClientCore = value;
				if(IsNavigationClientAttached) {
					SubscribeNavigationClient();
					OnNavigationClientAttached();
				}
			}
		}
		protected bool IsNavigationClientAttached {
			get { return navigationClientCore != null; }
		}
		protected virtual void OnNavigationClientAttached() {
			UpdateItems(NavigationClient.ItemsSource);
		}
		protected virtual void OnNavigationClientDetached() {
			ClearItems(NavigationClient.ItemsSource);
		}
		void SubscribeNavigationClient() {
			NavigationClient.ItemsSourceChanged += NavigationClient_ItemsSourceChanged;
			NavigationClient.SelectedItemChanged += NavigationClient_SelectedItemChanged;
		}
		void UnsubscribeNavigationClient() {
			NavigationClient.ItemsSourceChanged -= NavigationClient_ItemsSourceChanged;
			NavigationClient.SelectedItemChanged -= NavigationClient_SelectedItemChanged;
		}
		void NavigationClient_ItemsSourceChanged(object sender, EventArgs e) {
			UpdateItems(NavigationClient.ItemsSource);
		}
		void NavigationClient_SelectedItemChanged(object sender, EventArgs e) {
			if(itemsSourceItems.Count == 0) return;
			if(NavigationClient.SelectedItem != null)
				SelectedItem = itemsSourceItems[NavigationClient.SelectedItem];
			else
				SelectedItem = null;
		}
		IDictionary<INavigationItem, NavigationBarItem> itemsSourceItems;
		INavigationItem GetItem(NavigationBarItem item) {
			foreach(var pair in itemsSourceItems) {
				if(pair.Value != item) continue;
				return pair.Key;
			}
			return null;
		}
		void ClearItems(IEnumerable<INavigationItem> itemsSource) {
			if(itemsSourceItems == null) return;
			var oldSelecteItem = SelectedItem;
			var items = GetClientItemsCollection();
			items.BeginUpdate();
			foreach(INavigationItem item in itemsSource) {
				var barItem = itemsSourceItems[item];
				items.Remove(barItem);
				RaiseUnregisterItem(barItem, item);
			}
			items.EndUpdate();
			if(oldSelecteItem != SelectedItem)
				InvalidateSelectedItem(oldSelecteItem, SelectedItem);
		}
		void UpdateItems(IEnumerable<INavigationItem> itemsSource) {
			if(itemsSourceItems == null)
				itemsSourceItems = new Dictionary<INavigationItem, NavigationBarItem>();
			var oldSelecteItem = SelectedItem;
			var items = GetClientItemsCollection();
			items.BeginUpdate();
			foreach(INavigationItem item in itemsSource) {
				bool reaiseRegisterItem = false;
				NavigationBarItem barItem;
				if(!itemsSourceItems.TryGetValue(item, out barItem)) {
					barItem = new ClientNavigationBarItem(item);
					itemsSourceItems.Add(item, barItem);
					reaiseRegisterItem = true;
				}
				else {
					barItem.Visible = item.Visible;
					barItem.Name = item.Name;
					barItem.Text = item.Text;
					barItem.Image = item.Image;
				}
				if(!items.Contains(barItem))
					items.Add(barItem);
				if(reaiseRegisterItem)
					RaiseRegisterItem(barItem, item);
			}
			RemoveUnusedClientItems(itemsSource, items);
			items.EndUpdate();
			if(oldSelecteItem != SelectedItem)
				InvalidateSelectedItem(oldSelecteItem, SelectedItem);
		}
		void RemoveUnusedClientItems(IEnumerable<INavigationItem> itemsSource, NavigationBarItemCollection items) {
			var actualItems = new List<INavigationItem>();
			var unusedItems = new List<INavigationItem>();
			actualItems.AddRange(itemsSource);
			foreach(var pair in itemsSourceItems) {
				if(actualItems.Contains(pair.Key)) continue;
				unusedItems.Add(pair.Key);
			}
			foreach(INavigationItem item in unusedItems) {
				var barItem = itemsSourceItems[item];
				items.Remove(barItem);
				RaiseUnregisterItem(barItem, item);
				itemsSourceItems.Remove(item);
			}
		}
		NavigationBarItemCollection GetClientItemsCollection() {
			if(DesignMode) return ClientItems;
			return Items;
		}
		#endregion
		int initLock;
		#region ISupportInitialize Members
		public void BeginInit() { initLock++; }
		public void EndInit() {
			if(IsNavigationClientAttached) {
				UpdateItems(NavigationClient.ItemsSource);
				if(itemsSourceItems.Count > 0 && NavigationClient.SelectedItem != null) {
					var selectedNavigationBarItem = itemsSourceItems[NavigationClient.SelectedItem];
					if(SelectedItem != selectedNavigationBarItem)
						SelectedItem = selectedNavigationBarItem;
				}
			}
			EnsureAutoSize();
			initLock = 0;
		}
		#endregion
		private void EnsureAutoSize() {
			if(!AutoSize) return;
			AutoSize = GetAutoSize();
		}
		bool GetAutoSize() {
			if(Dock == DockStyle.None) return true;
			if(Orientation == Orientation.Horizontal)
				return Dock == DockStyle.Top || Dock == DockStyle.Bottom;
			else
				return Dock == DockStyle.Left || Dock == DockStyle.Right;
		}
		bool IXtraResizableControl.IsCaptionVisible { get { return false; } }
		Size IXtraResizableControl.MaxSize {
			get { return GetResizableSize(); }
		}
		Size IXtraResizableControl.MinSize {
			get { return GetResizableSize(); }
		}
		Size GetResizableSize() {
			return AutoSizeInLayoutControl ? GetLayoutMinSize() : new Size(0, 0);
		}
		Size GetLayoutMinSize() {
			CheckViewInfo();
			Size max = ControlCore.ViewInfoCore.MaxSize;
			if(Orientation == Orientation.Horizontal)
				return new Size(0, max.Height);
			return new Size(max.Width, 0);
		}
		protected static object resizableChanged = new object();
		event EventHandler IXtraResizableControl.Changed {
			add { Events.AddHandler(resizableChanged, value); }
			remove { Events.RemoveHandler(resizableChanged, value); }
		}
		bool KeepItemInfosAlive { get; set; }
		IComponentChangeService componentChangeService;
		protected internal IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = GetICCS();
					SubcribeOnComponentChangeService(componentChangeService);
				}
				return componentChangeService;
			}
		}
		IComponentChangeService GetICCS() {
			if(Site == null) return null;
			return Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
		}
		protected internal void FireChanged() {
			if(ComponentChangeService == null) return;
			ComponentChangeService.OnComponentChanged(this, null, null, null);
		}
		void SubcribeOnComponentChangeService(IComponentChangeService srv) {
			if(srv == null) return;
			srv.ComponentRemoved += srv_ComponentRemoved;
		}
		void srv_ComponentRemoved(object sender, ComponentEventArgs e) {
			var control = e.Component as OfficeNavigationBar;
			if(control == null || control != this) return;
			RemoveItemsFromContainter();
		}
		protected internal void AddItemToContainer(object component) {
			if(component is ClientNavigationBarItem) return;
			NavigationBarItem item = component as NavigationBarItem;
			if(item == null || Container == null) return;
			Container.Add(item);
		}
		protected internal void RemoveItemFromContainer(object component) {
			NavigationBarItem item = component as NavigationBarItem;
			if(item == null || Container == null) return;
			Container.Remove(item);
		}
		protected internal void RemoveItemsFromContainter() {
			foreach(var item in Items) {
				RemoveItemFromContainer(item);
			}
			FireChanged();
		}
		#region ToolTip
		bool showToolTips;
		[DXCategory(CategoryName.ToolTip),  DefaultValue(true)]
		public virtual bool ShowToolTips { get { return showToolTips; } set { showToolTips = value; } }
		ToolTipController toolTipController;
		[DXCategory(CategoryName.ToolTip),  DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(CanShowToolTip) {
				object obj = this;
				SuperToolTip superTip = null;
				ToolTipControlInfo res = new ToolTipControlInfo();
				var hitInfo = ControlCore.ViewInfoCore.CalcHitInfo(point);
				if(hitInfo.InItem && hitInfo.ItemInfo != null && hitInfo.ItemInfo.Item != null) {
					var tile = hitInfo.ItemInfo.Item as NavigationBarItemCore;
					NavigationBarItem navBarItem = tile.OwnerItem;
					obj = navBarItem;
					superTip = navBarItem.SuperTip;
				}
				res.Object = obj;
				res.SuperTip = superTip;
				return res;
			}
			return null;
		}
		protected virtual bool CanShowToolTip {
			get { return ControlCore.Handler.State == TileControlHandlerState.Normal && ShowToolTips && !IsDesignMode; }
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return this.ShowToolTips; }
		}
		void RemoveTooltipControllers() {
			ToolTipController.DefaultController.RemoveClientControl(this);
			ToolTipController = null;
		}
		#endregion ToolTip
	}
	public enum CustomizationButtonVisibility { ShowBeforeItems, ShowAfterItems, Hidden }
	class NavigationBarCore : ITileControl, ITileControlProperties, ISupportXtraAnimation {
		public NavigationBarCore(OfficeNavigationBar owner) {
			this.ownerCore = owner;
		}
		OfficeNavigationBar ownerCore;
		public bool AutoSize { get { return Owner.AutoSize; } }
		internal OfficeNavigationBar Owner { get { return ownerCore; } }
		internal ObjectPainter GetButtonsPanelPainter() { return (Owner as IButtonsPanelOwner).GetPainter(); }
		public bool AnimateItemPress { get { return Owner.AnimateItemPressing; } }
		public bool AllowItemSkinning { get { return Owner.ItemSkinning; } }
		public NavigationBarButtonsPanel ButtonsPanel { get { return Owner.ButtonsPanel; } }
		public ButtonCollection CustomButtons { get { return Owner.CustomButtons; } }
		public bool ShowItemMenu(NavigationBarItem item, Point pt) { return Owner.ShowItemMenu(item, pt); }
		public bool ShowCustomizationMenu(Point point) { return Owner.ShowCustomizationMenu(point); }
		public void SyncItemsAndVisibleItems() { Owner.SyncItemsAndVisibleItems(); }
		public bool ShowCustomizationButton { get { return Owner.CustomizationButtonVisibility != CustomizationButtonVisibility.Hidden; } }
		public bool ShowPeekFormOnItemHover { get { return Owner.ShowPeekFormOnItemHover; } }
		public bool ShowButtonsBeforeItems { get { return Owner.CustomizationButtonVisibility == CustomizationButtonVisibility.ShowBeforeItems; } }
		public int PeekFormShowDelay { get { return Owner.PeekFormShowDelay; } }
		public int MaxItemCount { get { return Owner.MaxItemCount; } }
		public Control RaiseQueryControl(NavigationBarItem item) { return Owner.RaiseQueryControl(item); }
		bool ITileControl.Capture { get; set; }
		TileGroupCollection groupsCore;
		internal TileGroupCollection Groups {
			get {
				if(groupsCore == null) groupsCore = new TileGroupCollection(this);
				return groupsCore;
			}
		}
		ContextItemCollection ITileControl.ContextButtons { get { return null; } }
		ContextItemCollectionOptions ITileControl.ContextButtonOptions { get { return null; } }
		void ITileControl.RaiseContextItemClick(ContextItemClickEventArgs e) { }
		void ITileControl.RaiseContextButtonCustomize(ITileItem tileItem, ContextItem item) { }
		void ITileControl.RaiseCustomContextButtonToolTip(ITileItem tileItem, ContextButtonToolTipEventArgs e) { }
		TileGroupCollection ITileControl.Groups { get { return Groups; } }
		TileControlHandler ITileControl.Handler { get { return Handler; } }
		TileControlViewInfo ITileControl.ViewInfo { get { return ViewInfoCore; } }
		TileControlPainter ITileControl.SourcePainter { get { return Painter; } }
		TileControlNavigator ITileControl.Navigator { get { return Navigator; } }
		void ITileControl.Invalidate(Rectangle rect) { Owner.Invalidate(rect); }
		void ITileControl.OnPropertiesChanged() { Owner.OnPropertiesChanged(); }
		void ITileControl.BeginUpdate() { Owner.BeginUpdate(); }
		void ITileControl.EndUpdate() { Owner.EndUpdate(); }
		void ITileControl.AddControl(Control control) { }
		Color ITileControl.BackColor { get { return Owner.BackColor; } }
		Color ITileControl.SelectionColor { get { return Color.Empty; } }
		TileItemAppearances ITileControl.AppearanceItem { get { return Owner.AppearanceItem; } }
		AppearanceObject ITileControl.AppearanceText { get { return null; } }
		AppearanceObject ITileControl.AppearanceGroupText { get { return null; } }
		Rectangle ITileControl.Bounds { get { return Owner.Bounds; } }
		Rectangle ITileControl.ClientRectangle { get { return Owner.ClientRectangle; } }
		Point ITileControl.PointToClient(Point pt) { return Owner.PointToClient(pt); }
		Point ITileControl.PointToScreen(Point pt) { return Owner.PointToScreen(pt); }
		SizeF ITileControl.ScaleFactor { get { return new SizeF(1, 1); } }
		Size ITileControl.DragSize { get { return Size.Empty; } set { } }
		bool ITileControl.AllowGlyphSkinning { get { return false; } set { } }
		bool ITileControl.AnimateArrival { get { return false; } set { } }
		bool ITileControl.ContainsControl(Control control) { return false; }
		bool ITileControl.EnableItemDoubleClickEvent { get { return false; } set { } }
		bool ITileControl.IsLockUpdate { get { return Owner.IsLockUpdate; } }
		bool ITileControl.Focus() { return false; }
		bool ITileControl.AllowDrag { get { return Owner.AllowDrag; } }
		bool ITileControl.IsDesignMode { get { return Owner.IsDesignMode; } }
		bool ITileControl.IsHandleCreated { get { return Owner.IsHandleCreated; } }
		bool ITileControl.AutoSelectFocusedItem { get; set; }
		bool ITileControl.AllowItemContentAnimation { get { return false; } }
		bool ITileControl.AllowSelectedItem { get { return true; } }
		bool ITileControl.AllowDisabledStateIndication { get { return false; } set { } }
		bool ITileControl.AllowDragTilesBetweenGroups { get { return true; } set { } }
		int ITileControl.Position { get { return 0; } set { } }
		int ITileControl.ScrollButtonFadeAnimationTime { get; set; }
		Image ITileControl.BackgroundImage { get; set; }
		ScrollBarBase ITileControl.ScrollBar { get { return null; } set { } }
		TileControlScrollMode ITileControl.ScrollMode { get { return TileControlScrollMode.None; } set { } }
		ISite ITileControl.Site { get { return Owner.Site; } }
		IContainer ITileControl.Container { get { return Owner.Container; } }
		Control ITileControl.Control { get { return Owner; } }
		XtraEditors.Controls.BorderStyles ITileControl.BorderStyle { get; set; }
		LookAndFeel.UserLookAndFeel ITileControl.LookAndFeel { get { return Owner.LookAndFeel; } }
		IntPtr ITileControl.Handle { get { return Owner.Handle; } }
		ITileControlProperties ITileControl.Properties { get { return this; } }
		ISupportXtraAnimation ITileControl.AnimationHost { get { return this; } }
		OfficeNavigationBarViewInfo viewInfoCore;
		internal OfficeNavigationBarViewInfo ViewInfoCore {
			get {
				if(viewInfoCore == null) viewInfoCore = new OfficeNavigationBarViewInfo(this);
				return viewInfoCore;
			}
		}
		OfficeNavigationBarPainter painterCore;
		internal OfficeNavigationBarPainter Painter {
			get {
				if(painterCore == null) painterCore = new OfficeNavigationBarPainter();
				return painterCore;
			}
		}
		OfficeNavigationBarNavigator navigatorCore;
		OfficeNavigationBarNavigator Navigator {
			get {
				if(navigatorCore == null) navigatorCore = new OfficeNavigationBarNavigator(this);
				return navigatorCore;
			}
		}
		OfficeNavigationBarHandler handlerCore;
		internal OfficeNavigationBarHandler Handler {
			get {
				if(handlerCore == null) handlerCore = new OfficeNavigationBarHandler(this);
				return handlerCore;
			}
		}
		void ITileControl.OnItemPress(TileItem tileItem) { }
		void ITileControl.OnItemDoubleClick(TileItem tileItem) { }
		void ITileControl.OnRightItemClick(TileItem tileItem) { }
		void ITileControl.OnItemCheckedChanged(TileItem tileItem) { }
		void ITileControl.OnItemClick(TileItem tileItem) {
			var item = tileItem as NavigationBarItemCore;
			if(item == null || item.OwnerItem == null) return;
			Owner.RaiseItemClick(item.OwnerItem);
		}
		event TileItemClickEventHandler ITileControl.ItemClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemDoubleClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.RightItemClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemPress { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemCheckedChanged { add { } remove { } }
		event TileItemClickEventHandler ITileControl.SelectedItemChanged { add { } remove { } }
		event TileItemDragEventHandler ITileControl.StartItemDragging { add { } remove { } }
		event TileItemDragEventHandler ITileControl.EndItemDragging { add { } remove { } }
		void ITileControl.SuspendAnimation() { }
		void ITileControl.ResumeAnimation() { }
		bool ITileControl.IsAnimationSuspended {
			get { return false; }
		}
		TileItem ITileControl.SelectedItem {
			get { return Owner.SelectedItem == null ? null : Owner.SelectedItem.Tile; }
			set {
				var item = value as NavigationBarItemCore;
				if(item != null && item.OwnerItem != null)
					item.OwnerItem.Select();
				else
					ownerCore.SelectedItem = null;
			}
		}
		TileItemDragEventArgs ITileControl.RaiseStartItemDragging(TileItem item) {
			TileItemDragEventArgs ea = new TileItemDragEventArgs();
			return ea;
		}
		TileItemDragEventArgs ITileControl.RaiseEndItemDragging(TileItem item, TileGroup targetGroup) {
			TileItemDragEventArgs ea = new TileItemDragEventArgs();
			return ea;
		}
		object ITileControl.Images {
			get { return Owner.Images; }
			set { Owner.Images = value; }
		}
		TileControlHitInfo ITileControl.CalcHitInfo(Point pt) {
			return ViewInfoCore.CalcHitInfo(pt);
		}
		string ITileControl.Text { get; set; }
		bool ITileControl.Enabled {
			get { return Owner.Enabled; }
			set { Owner.Enabled = value; }
		}
		bool ITileControl.DebuggingState { get { return false; } }
		void ITileControl.UpdateSmartTag() { }
		bool ISupportXtraAnimation.CanAnimate {
			get { return !Owner.Disposing; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return Owner; }
		}
		#region PROPERTIES
		public bool Compact { get { return Owner.Compact; } }
		int ITileControlProperties.RowCount { get { return 1; } set { } }
		int ITileControlProperties.ColumnCount { get { return 0; } set { } }
		int ITileControlProperties.ItemSize { get { return 120; } set { } }
		int ITileControlProperties.LargeItemWidth { get { return 0; } set { } }
		int ITileControlProperties.IndentBetweenItems { get { return 0; } set { } }
		int ITileControlProperties.IndentBetweenGroups { get { return 0; } set { } }
		public int Width { get { return Owner.Width; } set { Owner.Width = value; } }
		public int Height { get { return Owner.Height; } set { Owner.Height = value; } }
		bool ITileControlProperties.AllowItemHover { get { return true; } set { } }
		bool ITileControlProperties.AllowSelectedItem { get { return Owner.AllowItemSelection; } set { } }
		bool ITileControlProperties.ShowText { get { return false; } set { } }
		bool ITileControlProperties.AllowGroupHighlighting { get { return false; } set { } }
		public Orientation Orientation {
			get { return Owner.Orientation; }
			set { Owner.Orientation = value; }
		}
		HorzAlignment ITileControlProperties.HorizontalContentAlignment {
			get { return Owner.HorizontalContentAlignment; }
			set { Owner.HorizontalContentAlignment = value; }
		}
		VertAlignment ITileControlProperties.VerticalContentAlignment {
			get { return VertAlignment.Top; }
			set { }
		}
		Padding ITileControlProperties.Padding {
			get { return Owner.Padding; }
			set { }
		}
		Padding ITileControlProperties.ItemPadding {
			get { return Owner.ItemPadding; }
			set { }
		}
		ImageLayout ITileControlProperties.BackgroundImageLayout {
			get { return ImageLayout.None; }
			set { }
		}
		TileItemContentAlignment ITileControlProperties.ItemImageAlignment {
			get { return TileItemContentAlignment.Default; }
			set { }
		}
		TileItemContentAlignment ITileControlProperties.ItemBackgroundImageAlignment {
			get { return TileItemContentAlignment.Default; }
			set { }
		}
		TileItemImageScaleMode ITileControlProperties.ItemImageScaleMode {
			get { return TileItemImageScaleMode.Default; }
			set { }
		}
		TileItemImageScaleMode ITileControlProperties.ItemBackgroundImageScaleMode {
			get { return TileItemImageScaleMode.Default; }
			set { }
		}
		TileItemContentAnimationType ITileControlProperties.ItemContentAnimation {
			get { return TileItemContentAnimationType.Default; }
			set { }
		}
		TileItemContentShowMode ITileControlProperties.ItemTextShowMode {
			get { return TileItemContentShowMode.Default; }
			set { }
		}
		TileItemCheckMode ITileControlProperties.ItemCheckMode {
			get { return TileItemCheckMode.None; }
			set { }
		}
		bool ITileControlProperties.AllowHtmlDraw {
			get { return Owner.AllowHtmlDraw; }
			set { Owner.AllowHtmlDraw = value; }
		}
		bool ITileControlProperties.AllowDrag {
			get { return Owner.AllowDrag; }
			set { Owner.AllowDrag = value; }
		}
		GroupHighlightingProperties ITileControlProperties.AppearanceGroupHighlighting {
			get { return new GroupHighlightingProperties(); }
			set { }
		}
		bool ITileControlProperties.ShowGroupText {
			get { return false; }
			set { }
		}
		void ITileControlProperties.Assign(ITileControlProperties source) {
			throw new NotImplementedException();
		}
		TileItemBorderVisibility ITileControlProperties.ItemBorderVisibility {
			get { return TileItemBorderVisibility.Auto; }
			set { }
		}
		#endregion PROPERTIES
	}
}
