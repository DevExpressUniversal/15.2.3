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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.XtraBars.Design {
	public partial class TileNavPaneDesignerForm : XtraDesignForm {
		public TileNavPaneDesignerForm() {
			InitializeComponent();
			pgCategory.CommandsVisibleIfAvailable = false;
			pgItem.CommandsVisibleIfAvailable = false;
			pgSubItem.CommandsVisibleIfAvailable = false;
			editKindCore = EditKinds.EditTileNavPane;
			this.FormClosed += TileNavPaneDesignerForm_FormClosed;
			tbCategories.MouseMove += tbCategories_MouseMove;
			tbItems.MouseMove += tbCategories_MouseMove;
			tbSubItems.MouseMove += tbCategories_MouseMove;
			SubscribeOnTilesControllers();
			DevExpress.Utils.Design.WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
		}
		private void SubscribeOnTilesControllers() {
			catTilesController.btnAdd.Click += btnAddCat_Click;
			catTilesController.btnRemove.Click += btnRemoveCat_Click;
			catTilesController.btnMoveLeft.Click += btnMoveLCat_Click;
			catTilesController.btnMoveRight.Click += btnMoveRCat_Click;
			itemsTilesController.btnAdd.Click += btnAddItem_Click;
			itemsTilesController.btnRemove.Click += btnRemoveItem_Click;
			itemsTilesController.btnMoveLeft.Click += btnMoveLItem_Click;
			itemsTilesController.btnMoveRight.Click += btnMoveRItem_Click;
			subitemsTilesController.btnAdd.Click += btnAddSubitem_Click;
			subitemsTilesController.btnRemove.Click += btnRemoveSubitem_Click;
			subitemsTilesController.btnMoveLeft.Click += btnMoveLSubItem_Click;
			subitemsTilesController.btnMoveRight.Click += btnMoveRSubItem_Click;
		}
		void TileNavPaneDesignerForm_FormClosed(object sender, FormClosedEventArgs e) {
			pgCategory.Site = null;
			pgItem.Site = null;
			pgSubItem.Site = null;
		}
		public enum EditKinds { EditTileNavPane, EditCategory }
		EditKinds editKindCore;
		EditKinds EditKind {
			get { return editKindCore; }
			set {
				if(editKindCore == value) return;
				editKindCore = value;
				OnEditKindChanged(editKindCore);
			}
		}
		void OnEditKindChanged(EditKinds editKindCore) {
			switch(editKindCore){
				case EditKinds.EditCategory:
					PrepareUIForEditCategory(); return;
				case EditKinds.EditTileNavPane:
					PrepareUIForEditTileNavPane(); return;
			}
		}
		void PrepareUIForEditTileNavPane() { }
		void PrepareUIForEditCategory() {
			layoutControlGroup4.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			tabCategory.PageVisible = false;
			SetManualSelectedCategory = true;
		}
		void tbCategories_MouseMove(object sender, MouseEventArgs e) {
			var hitInfo = (sender as TileBar).CalcHitInfo(e.Location);
			if(hitInfo.InItem)
				HoveredItem = hitInfo.ItemInfo as TileBarItemViewInfo;
			else
				HoveredItem = null;
		}
		internal void Assign(TileNavPane tileNavPane, TileNavCategory cat, TileNavPaneDesignTimeManager dtmanager) {
			this.tnpCore = tileNavPane;
			this.dtManagerCore = dtmanager;
			this.EditKind = EditKinds.EditCategory;
			this.selectedCategoryCore = cat;
			AssignCore(tileNavPane);
		}
		public void Assign(TileNavPane src, TileNavPaneDesignTimeManager dtmanager) {
			this.tnpCore = src;
			this.dtManagerCore = dtmanager;
			this.EditKind = EditKinds.EditTileNavPane;
			AssignCore(src);
		}
		void AssignCore(TileNavPane tnp) {
			OnControlChanged();
		}
		TileNavPaneDesignTimeManager dtManagerCore;
		public TileNavPaneDesignTimeManager DTManager {
			get { return dtManagerCore; }
		}
		bool SetManualSelectedCategory { get; set; }
		TileNavCategory selectedCategoryCore = null;
		TileNavCategory SelectedCategory {
			get {
				if(SetManualSelectedCategory) return selectedCategoryCore;
				if(tbCategories.SelectedItem == null) return null;
				return tbCategories.SelectedItem.Tag as TileNavCategory;
			}
		}
		TileNavPane tnpCore;
		public TileNavPane TileNavPane {
			get { return tnpCore; }
		}
		private void OnControlChanged() {
			if(TileNavPane == null) return;
			var lookAndFeel = TileNavPane.LookAndFeel.ActiveLookAndFeel;
			UpdateLAF(tbCategories, lookAndFeel);
			UpdateLAF(tbItems, lookAndFeel);
			UpdateLAF(tbSubItems, lookAndFeel);
			ClearTiles(tbCategories);
			ClearTiles(tbItems);
			FillCategories();
			SelectFirstCategory();
		}
		void UpdateLAF(DevExpress.LookAndFeel.ISupportLookAndFeel control, DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			if(control == null || lookAndFeel == null) return;
			control.LookAndFeel.UseDefaultLookAndFeel = false;
			control.LookAndFeel.Style = lookAndFeel.Style;
			control.LookAndFeel.SkinName = lookAndFeel.SkinName;
		}
		private void SelectFirstCategory() {
			if(EditKind == EditKinds.EditCategory) {
				tbCategories_SelectedItemChanged(null, null);
				return;
			}
			if(tbCategories.Groups.Count != 1) return;
			if(tbCategories.Groups[0].Items.Count == 0) return;
			tbCategories.SelectedItem = tbCategories.Groups[0].Items[0];
		}
		void FillCategories() {
			tbCategories.BeginUpdate();
			try {
				AssignCategoriesTileBar();
				TileBarGroup g = new TileBarGroup();
				tbCategories.Groups.Add(g);
				foreach(TileNavCategory cat in TileNavPane.Categories) {
					TileBarItem tile = new TileBarItem();
					AssignTile(tile, cat);
					g.Items.Add(tile);
				}
			}
			finally { tbCategories.EndUpdate(); }
		}
		void FillItems() {
			if(SelectedCategory == null) return;
			FillItems(SelectedCategory);
		}
		void FillItems(TileNavCategory cat) {
			if(cat == null) return;
			AssignItemsTileBar(cat);
			tbItems.BeginUpdate();
			try {
				TileBarGroup g = new TileBarGroup();
				tbItems.Groups.Add(g);
				foreach(TileNavItem item in cat.Items) {
					TileBarItem tile = new TileBarItem();
					AssignTile(tile, item);
					g.Items.Add(tile);
				}
			}
			finally { tbItems.EndUpdate(); }
		}
		void FillSubItems() {
			if(tbItems.SelectedItem == null) return;
			var item = tbItems.SelectedItem.Tag as TileNavItem;
			if(item == null) return;
			FillSubItems(item);
		}
		void FillSubItems(TileNavItem item) {
			if(item == null) return;
			tbSubItems.BeginUpdate();
			AssignSubItemsTileBar(item);
			try {
				TileBarGroup g = new TileBarGroup();
				tbSubItems.Groups.Add(g);
				foreach(TileNavSubItem subItem in item.SubItems) {
					TileBarItem tile = new TileBarItem();
					AssignTile(tile, subItem);
					g.Items.Add(tile);
				}
			}
			finally { tbSubItems.EndUpdate(); }
		}
		private void AssignSubItemsTileBar(TileNavItem item) {
			int itemSize = item.OptionsDropDown.ItemHeight > 0 ? item.OptionsDropDown.ItemHeight : TileNavPane.OptionsSecondaryDropDown.ItemHeight;
			if(itemSize > 0) tbSubItems.ItemSize = itemSize;
			int tileWidth = item.OptionsDropDown.WideItemWidth > 0 ? item.OptionsDropDown.WideItemWidth : TileNavPane.OptionsSecondaryDropDown.WideItemWidth;
			if(tileWidth > 0) tbSubItems.WideTileWidth = tileWidth;
			tbSubItems.ShowItemShadow = item.OptionsDropDown.ShowItemShadow != DefaultBoolean.Default ? item.OptionsDropDown.ShowItemShadow.ToBoolean(false) : TileNavPane.OptionsSecondaryDropDown.ShowItemShadow.ToBoolean(false);
			tbSubItems.AllowGlyphSkinning = item.AllowGlyphSkinning != DefaultBoolean.Default ? item.AllowGlyphSkinning.ToBoolean(false) : TileNavPane.OptionsSecondaryDropDown.AllowGlyphSkinning.ToBoolean(false);
			tbSubItems.SelectionColor = tbSubItems.BackColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
			tbSubItems.SelectionBorderWidth = 2;
		}
		void AssignCategoriesTileBar() {
			tbCategories.AppearanceItem.Assign(TileNavPane.OptionsPrimaryDropDown.AppearanceItem);
			tbCategories.AppearanceGroupText.Assign(TileNavPane.OptionsPrimaryDropDown.AppearanceGroupText);
			tbCategories.Height = TileNavPane.OptionsPrimaryDropDown.Height > 0 ? TileNavPane.OptionsPrimaryDropDown.Height : tbCategories.Height;
			tbCategories.ItemSize = TileNavPane.OptionsPrimaryDropDown.ItemHeight > 0 ? TileNavPane.OptionsPrimaryDropDown.ItemHeight : tbCategories.ItemSize;
			tbCategories.WideTileWidth = TileNavPane.OptionsPrimaryDropDown.WideItemWidth > 0 ? TileNavPane.OptionsPrimaryDropDown.WideItemWidth : tbCategories.WideTileWidth;
			tbCategories.ShowItemShadow = TileNavPane.OptionsPrimaryDropDown.ShowItemShadow.ToBoolean(false);
			tbCategories.BackColor = GetCategoriesBackColor(tbCategories);
			tbCategories.AllowGlyphSkinning = TileNavPane.OptionsPrimaryDropDown.AllowGlyphSkinning.ToBoolean(false);
			tbCategories.SelectionColor = tbCategories.BackColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
			tbCategories.SelectionBorderWidth = 2;
		}
		Color GetCategoriesBackColor(TileBar tbCategories) {
			Color c = TileNavPane.OptionsPrimaryDropDown.BackColor;
			if(!c.IsEmpty) return c;
			c = TileNavPane.AppearanceSelected.BackColor;
			if(!c.IsEmpty) return c;
			return tbCategories.BackColor;
		}
		void AssignItemsTileBar(TileNavCategory cat) {
			tbItems.AppearanceItem.Assign(GetCombinedItemsAppearances(cat));
			tbItems.AppearanceGroupText.Assign(CombineAppearances(cat.OptionsDropDown.AppearanceGroupText, TileNavPane.OptionsPrimaryDropDown.AppearanceGroupText));
			int h = cat.OptionsDropDown.Height > 0 ? cat.OptionsDropDown.Height : TileNavPane.OptionsPrimaryDropDown.Height;
			if(h > 0) tbItems.Height = h;
			int itemSize = cat.OptionsDropDown.ItemHeight > 0 ? cat.OptionsDropDown.ItemHeight : TileNavPane.OptionsPrimaryDropDown.ItemHeight;
			if(itemSize > 0) tbItems.ItemSize = itemSize;
			int tileWidth = cat.OptionsDropDown.WideItemWidth > 0 ? cat.OptionsDropDown.WideItemWidth : TileNavPane.OptionsPrimaryDropDown.WideItemWidth;
			if(tileWidth > 0) tbItems.WideTileWidth = tileWidth;
			tbItems.ShowItemShadow = cat.OptionsDropDown.ShowItemShadow != DefaultBoolean.Default ? cat.OptionsDropDown.ShowItemShadow.ToBoolean(false) : TileNavPane.OptionsPrimaryDropDown.ShowItemShadow.ToBoolean(false);
			tbItems.BackColor = GetItemsBackColor(cat, tbItems);
			tbItems.AllowGlyphSkinning = cat.AllowGlyphSkinning != DefaultBoolean.Default ? cat.AllowGlyphSkinning.ToBoolean(false) : TileNavPane.OptionsPrimaryDropDown.AllowGlyphSkinning.ToBoolean(false);
			tbItems.SelectionColor = tbCategories.BackColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
			tbItems.SelectionBorderWidth = 2;
		}
		Color GetItemsBackColor(TileNavElement element, TileBar tilebar) {
			Color c = element.OptionsDropDown.BackColor;
			if(!c.IsEmpty) return c;
			c = TileNavPane.OptionsPrimaryDropDown.BackColor;
			if(!c.IsEmpty) return c;
			c = element.AppearanceSelected.BackColor;
			if(!c.IsEmpty) return c;
			c = TileNavPane.AppearanceSelected.BackColor;
			if(!c.IsEmpty) return c;
			return tilebar.BackColor;
		}
		protected AppearanceObject CombineAppearances(AppearanceObject childAppearance, AppearanceObject rootAppearance) {
			AppearanceObject result = new AppearanceObject();
			AppearanceHelper.Combine(result, new AppearanceObject[] { childAppearance, rootAppearance });
			return result;
		}
		protected virtual TileItemAppearances GetCombinedItemsAppearances(TileNavCategory cat) {
			TileItemAppearances result = new TileItemAppearances();
			AppearanceObject norm = new AppearanceObject();
			AppearanceObject pressed = new AppearanceObject();
			AppearanceObject hovered = new AppearanceObject();
			AppearanceObject selected = new AppearanceObject();
			TileNavPaneDropDownOptions ownerOptions = TileNavPane.OptionsPrimaryDropDown;
			AppearanceHelper.Combine(norm, new AppearanceObject[] { cat.OptionsDropDown.AppearanceItem.Normal, ownerOptions.AppearanceItem.Normal });
			AppearanceHelper.Combine(pressed, new AppearanceObject[] { cat.OptionsDropDown.AppearanceItem.Pressed, ownerOptions.AppearanceItem.Pressed });
			AppearanceHelper.Combine(hovered, new AppearanceObject[] { cat.OptionsDropDown.AppearanceItem.Hovered, ownerOptions.AppearanceItem.Hovered });
			AppearanceHelper.Combine(selected, new AppearanceObject[] { cat.OptionsDropDown.AppearanceItem.Selected, ownerOptions.AppearanceItem.Selected });
			result.Normal.Assign(norm);
			result.Pressed.Assign(pressed);
			result.Hovered.Assign(hovered);
			result.Selected.Assign(selected);
			return result;
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			FreeHoverTimer();
			FreeFlyout();
			ClearTiles(tbCategories);
			ClearTiles(tbItems);
			ClearTiles(tbSubItems);
		}
		void FreeHoverTimer() {
			if(hoverTimer == null) return;
			hoverTimer.Stop();
			hoverTimer.Tick -= hoverPeekTimer_Tick;
			hoverTimer.Dispose();
			hoverTimer = null;
		}
		void FreeFlyout() {
			if(flyoutcore == null || flyoutcore.IsDisposed) return;
			flyoutcore.Dispose();
			flyoutcore = null;
		}
		void ClearTiles(TileBar tileBar) {
			tileBar.BeginUpdate();
			try {
				foreach(TileBarGroup g in tileBar.Groups) {
					g.Items.Clear();
				}
				tileBar.Groups.Clear();
			}
			finally { tileBar.EndUpdate(); }
		}
		private void tbCategories_SelectedItemChanged(object sender, TileItemEventArgs e) {
			if(EditKind == EditKinds.EditTileNavPane && tbCategories.SelectedItem == null) {
				pgCategory.SelectedObject = null;
				return;
			}
			pgCategory.SelectedObject = SelectedCategory;
			pgCategory.Site = SelectedCategory.Site;
			ClearTiles(tbItems);
			ClearTiles(tbSubItems);
			FillItems(SelectedCategory);
			tabControl.SelectedTabPage = tabCategory;
		}
		private void tbItems_SelectedItemChanged(object sender, TileItemEventArgs e) {
			if(tbItems.SelectedItem == null) {
				pgItem.SelectedObject = null;
				return;
			}
			pgItem.SelectedObject = tbItems.SelectedItem.Tag;
			pgItem.Site = (tbItems.SelectedItem.Tag as Component).Site;
			ClearTiles(tbSubItems);
			FillSubItems(tbItems.SelectedItem.Tag as TileNavItem);
			tabControl.SelectedTabPage = tabItem;
		}
		private void tbSubItems_SelectedItemChanged(object sender, TileItemEventArgs e) {
			if(tbSubItems.SelectedItem == null) {
				pgSubItem.SelectedObject = null;
				return;
			}
			pgSubItem.SelectedObject = tbSubItems.SelectedItem.Tag;
			pgSubItem.Site = (tbSubItems.SelectedItem.Tag as Component).Site;
			tabControl.SelectedTabPage = tabSubItems;
		}
		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			if(pgCategory.SelectedObject == null) return;
			if(pgCategory.SelectedObject is TileNavCategory) {
				AssignTile(tbCategories.SelectedItem as TileBarItem, pgCategory.SelectedObject as TileNavCategory);
			}
			Serialize();
		}
		private void pgItem_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			if(pgItem.SelectedObject == null) return;
			if(pgItem.SelectedObject is TileNavItem) {
				AssignTile(tbItems.SelectedItem as TileBarItem, pgItem.SelectedObject as TileNavItem);
			}
			Serialize();
		}
		private void pgSubItem_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			if(pgSubItem.SelectedObject == null) return;
			if(pgSubItem.SelectedObject is TileNavSubItem) {
				AssignTile(tbSubItems.SelectedItem as TileBarItem, pgSubItem.SelectedObject as TileNavSubItem);
			}
			Serialize();
		}
		void Serialize() {
			DTManager.ComponentChangeService.OnComponentChanged(TileNavPane, null, null, null);
		}
		private void btnAddCat_Click(object sender, EventArgs e) {
			var element = SelectedCategory;
			DTManager.OnAddCategoryClick();
			ClearTiles(tbCategories);
			FillCategories();
			tbCategories.SelectedItem = FindTileByElement(tbCategories, element);
		}
		private void btnRemoveCat_Click(object sender, EventArgs e) {
			if(SelectedCategory == null) return;
			DTManager.RemoveCategory(SelectedCategory, true);
			ClearTiles(tbItems);
			ClearTiles(tbSubItems);
			pgSubItem.SelectedObject = null;
			var prev = GetNextItem(tbCategories.SelectedItem as TileBarItem);
			tbCategories.SelectedItem.Group.Items.Remove(tbCategories.SelectedItem);
			tbCategories.SelectedItem = prev;
			tbItems.SelectedItem = null;
		}
		TileBarItem GetNextItem(TileBarItem item) {
			if(item == null) return null;
			var group = item.Group as TileBarGroup;
			var groups = group.Control.Groups;
			int groupCount = groups.Count;
			int itemsCount = group.Items.Count;
			int index = group.Items.IndexOf(item);
			int groupIndex = groups.IndexOf(group as TileGroup);
			index++;
			if(itemsCount == index) {
				groupIndex++;
				if(groupCount == groupIndex){
					index--;
					return group.Items.Count == 1 ? null : group.Items[index - 1] as TileBarItem;
				}
				return groups[groupIndex].Items.Count == 0 ? null : groups[groupIndex].Items[0] as TileBarItem;
			}
			else 
				return group.Items[index] as TileBarItem;
		}
		private void btnAddItem_Click(object sender, EventArgs e) {
			if(SelectedCategory == null) return;
			var element = tbItems.SelectedItem == null ? null : tbItems.SelectedItem.Tag as TileNavElement;
			DTManager.OnAddItemToCategoryClick(SelectedCategory);
			ClearTiles(tbItems);
			FillItems(SelectedCategory);
			tbItems.SelectedItem = FindTileByElement(tbItems, element);
		}
		private void btnRemoveItem_Click(object sender, EventArgs e) {
			if(tbItems.SelectedItem == null || SelectedCategory == null) return;
			DTManager.RemoveItem(tbItems.SelectedItem.Tag as TileNavItem, true);
			ClearTiles(tbSubItems);
			pgSubItem.SelectedObject = null;
			var prev = GetNextItem(tbItems.SelectedItem as TileBarItem);
			tbItems.SelectedItem.Group.Items.Remove(tbItems.SelectedItem);
			tbItems.SelectedItem = prev;
		}
		private void btnAddSubitem_Click(object sender, EventArgs e) {
			if(tbItems.SelectedItem == null) return;
			var element = tbSubItems.SelectedItem == null ? null : tbSubItems.SelectedItem.Tag as TileNavElement;
			DTManager.OnAddSubItemToItemClick(tbItems.SelectedItem.Tag as TileNavItem);
			ClearTiles(tbSubItems);
			FillSubItems(tbItems.SelectedItem.Tag as TileNavItem);
			tbSubItems.SelectedItem = FindTileByElement(tbSubItems, element);
		}
		private void btnRemoveSubitem_Click(object sender, EventArgs e) {
			if(tbSubItems.SelectedItem == null) return;
			DTManager.RemoveSubItem(tbSubItems.SelectedItem.Tag as TileNavSubItem);
			var prev = GetNextItem(tbSubItems.SelectedItem as TileBarItem);
			tbSubItems.SelectedItem.Group.Items.Remove(tbSubItems.SelectedItem);
			tbSubItems.SelectedItem = prev;
		}
		void AssignTile(TileBarItem tile, TileNavElement element) {
			tile.AppearanceItem.Assign(element.Tile.AppearanceItem);
			tile.ItemSize = element.Tile.ItemSize;
			tile.Text = element.TileText;
			tile.Image = element.TileImage;
			tile.AllowGlyphSkinning = element.Tile.AllowGlyphSkinning;
			tile.Tag = element;
			tile.Elements.Assign(element.Tile.Elements);
		}
		TileBarItemViewInfo hoveredItem = null;
		TileBarItemViewInfo HoveredItem {
			get { return hoveredItem; }
			set {
				if(hoveredItem == value)
					return;
				OnHoveredItemChanged(hoveredItem, value);
				hoveredItem = value;
			}
		}
		private void OnHoveredItemChanged(TileBarItemViewInfo prevItem, TileBarItemViewInfo item) {
			if(prevItem == null || item == null) {
				HoverTimer.Stop();
				HoverTimer.Tag = null;
			}
			if(item != null) {
				HoverTimer.Tag = item;
				HoverTimer.Start();
			}
		}
		internal void ShowItemSettingsPeekForm(Control content, TileBarItemViewInfo itemInfo) {
			if(Flyout.Tag == itemInfo.ItemCore) return;
			Flyout.HideBeakForm(true);
			Flyout.Controls.Clear();
			Flyout.OwnerControl = this;
			Flyout.ParentForm = this.FindForm();
			Flyout.Tag = itemInfo.ItemCore;
			Flyout.ClientSize = new Size(300, 100);
			content.Dock = DockStyle.Fill;
			content.Parent = Flyout;
			Point pt = itemInfo.Bounds.Location;
			pt.X += itemInfo.Bounds.Width / 2;
			var tileBar = itemInfo.ControlInfo.Owner.Control;
			if(tileBar == null || tileBar.IsDisposed) return;
			Flyout.ShowBeakForm(tileBar.PointToScreen(pt));
		}
		FlyoutPanel flyoutcore;
		FlyoutPanel Flyout {
			get {
				if(flyoutcore == null || flyoutcore.IsDisposed) {
					flyoutcore = new FlyoutPanel();
					flyoutcore.Hidden += (s, e) => { FlyoutHidded(flyoutcore); };
				}
				return flyoutcore;
			}
		}
		void FlyoutHidded(FlyoutPanel fp) {
			if(fp != null && !fp.IsDisposed)
				fp.Tag = null;
		}
		Timer hoverTimer;
		protected internal Timer HoverTimer {
			get {
				if(hoverTimer == null) {
					hoverTimer = new Timer();
					hoverTimer.Interval = 800;
					hoverTimer.Tick += hoverPeekTimer_Tick;
				}
				return hoverTimer;
			}
		}
		void hoverPeekTimer_Tick(object sender, EventArgs e) {
			HoverTimer.Stop();
			TileBarItemViewInfo item = HoverTimer.Tag as TileBarItemViewInfo;
			if(item == null) return;
			ItemWrapper iw = new ItemWrapper(item.ItemCore.Tag as TileNavElement, Flyout);
			PropertyGrid pg = new PropertyGrid();
			pg.ToolbarVisible = false;
			pg.HelpVisible = false;
			pg.SelectedObject = iw;
			pg.Site = iw.Element.Site;
			pg.PropertyValueChanged += (s, ea) => { AssignTile(item.ItemCore, item.ItemCore.Tag as TileNavElement); };
			ShowItemSettingsPeekForm(pg, item);
		}
		private void btnMoveLCat_Click(object sender, EventArgs e) {
			MoveSelectedElement(true, tbCategories, TileNavPane.Categories, new MethodInvoker(FillCategories));
		}
		private void btnMoveRCat_Click(object sender, EventArgs e) {
			MoveSelectedElement(false, tbCategories, TileNavPane.Categories, new MethodInvoker(FillCategories));
		}
		private void btnMoveLItem_Click(object sender, EventArgs e) {
			var cat = SelectedCategory;
			if(cat == null) return;
			MoveSelectedElement(true, tbItems, cat.Items, new MethodInvoker(FillItems));
		}
		private void btnMoveRItem_Click(object sender, EventArgs e) {
			var cat = SelectedCategory;
			if(cat == null) return;
			MoveSelectedElement(false, tbItems, cat.Items, new MethodInvoker(FillItems));
		}
		private void btnMoveLSubItem_Click(object sender, EventArgs e) {
			if(tbItems.SelectedItem == null) return;
			var item = tbItems.SelectedItem.Tag as TileNavItem;
			MoveSelectedElement(true, tbSubItems, item.SubItems, new MethodInvoker(FillSubItems));
		}
		private void btnMoveRSubItem_Click(object sender, EventArgs e) {
			if(tbItems.SelectedItem == null) return;
			var item = tbItems.SelectedItem.Tag as TileNavItem;
			MoveSelectedElement(false, tbSubItems, item.SubItems, new MethodInvoker(FillSubItems));
		}
		void MoveSelectedElement(bool moveLeft, TileBar tilebar, IList collection, MethodInvoker fill) {
			if(tilebar.SelectedItem == null) return;
			var element = tilebar.SelectedItem.Tag as TileNavElement;
			if(!collection.Contains(element)) return;
			int index = collection.IndexOf(element);
			collection.Remove(element);
			index = moveLeft ? index - 1 : index + 1;
			index = Math.Max(0, Math.Min(index, collection.Count));
			collection.Insert(index, element);
			ClearTiles(tilebar);
			fill();
			tilebar.SelectedItem = FindTileByElement(tilebar,element);
			Serialize();
		}
		TileBarItem FindTileByElement(TileBar tilebar, TileNavElement element) {
			foreach(TileBarGroup gr in tilebar.Groups) {
				foreach(TileBarItem item in gr.Items) {
					if(item.Tag.Equals(element))
						return item;
				}
			}
			return null;
		}
		void tbCategories_ItemClick(object sender, TileItemEventArgs e) {
			tabControl.SelectedTabPage = tabCategory;
		}
		void tbItems_ItemClick(object sender, TileItemEventArgs e) {
			tabControl.SelectedTabPage = tabItem;
		}
		void tbSubItems_ItemClick(object sender, TileItemEventArgs e) {
			tabControl.SelectedTabPage = tabSubItems;
		}
	}
}
class ItemWrapper  {
	public ItemWrapper(TileNavElement element, FlyoutPanel flyout) { 
		this.Element = element;
		this.Flyout = flyout;
	}
	[Browsable(false)]
	public TileNavElement Element { get; set; }
	[Browsable(false)]
	public FlyoutPanel Flyout { get; set; }
	[Category(CategoryName.Properties)]
	public string Text {
		get { return Element.TileText; }
		set { 
			Element.TileText = value;
			Element.Caption = value;
		}
	}
	[Category(CategoryName.Properties)]
	[Editor(typeof(FlyoutImageEditor), typeof(UITypeEditor))]
	public Image Image {
		get { return Element.TileImage; }
		set { Element.TileImage = value; }
	}
	[Category(CategoryName.Properties)]
	public Color BackColor {
		get { return Element.Tile.AppearanceItem.Normal.BackColor; }
		set { Element.Tile.AppearanceItem.Normal.BackColor = value; }
	}
	[Category(CategoryName.Properties)]
	public Color ForeColor {
		get { return Element.Tile.AppearanceItem.Normal.ForeColor; }
		set { Element.Tile.AppearanceItem.Normal.ForeColor = value; }
	}
	public void HideFlyout() {
		if(Flyout == null) return;
		Flyout.HideBeakForm(true);
	}
}
class FlyoutImageEditor : DXImageEditor {
	public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
		var itemWrapper = context.Instance as ItemWrapper;
		if(itemWrapper != null) itemWrapper.HideFlyout();
		return base.EditValue(context, provider, value);
	}
}
