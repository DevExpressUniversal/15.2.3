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
using System.Reflection;
using System.Collections;
using DevExpress.XtraLayout.Localization;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Helpers;
using System.Collections.Generic;
using DevExpress.XtraLayout.Customization.Templates;
using System.IO;
using DevExpress.XtraLayout.Customization.Controls;
namespace DevExpress.XtraLayout.Customization {
	public class MenuManagerException : LayoutControlInternalException {
		public static string CurrentGroupNotSet = "CurrentGroup property is not set";
		public static string CurrentGroupSelectionError = "No item is selected in current group";
		public static string LayoutItemInvalidOperation = "Cannot perform this operation on item";
		public MenuManagerException(string reason) : base(reason) { }
	}
	public class CurrentGroupChecker {
		[ThreadStatic]
		static CurrentGroupChecker _default = null;
		private CurrentGroupChecker() { }
		public static CurrentGroupChecker Default {
			get {
				if(_default==null) {
					_default = new CurrentGroupChecker();
				}
				return _default;
			}
		}
		public void CheckIsNotNull(LayoutGroup currentGroup) {
			if(currentGroup==null) {
				throw new MenuManagerException(MenuManagerException.CurrentGroupNotSet);
			}
		}
		public void CheckIsSelected(LayoutGroup currentGroup) {
			if(!(currentGroup.SelectedItems.Count>0 || currentGroup.Selected)) {
				throw new MenuManagerException(MenuManagerException.CurrentGroupSelectionError);
			}
		}
		public void CheckIsTabbed(LayoutGroup currentGroup) {
			if((currentGroup.SelectedItems[0] as TabbedGroup)==null) {
				throw new MenuManagerException(MenuManagerException.LayoutItemInvalidOperation);
			}
		}
		public void CheckCustom(LayoutGroup currentGroup, bool fNullCkeck, bool fSelectedCheck, bool fTabbedCheck) {
			if(fNullCkeck) CheckIsNotNull(currentGroup);
			if(fSelectedCheck) CheckIsSelected(currentGroup);
			if(fTabbedCheck) CheckIsTabbed(currentGroup);
		}
	}
	public class RightButtonMenuManager {
		ILayoutControl Owner;
		RightButtonMenuBase menu;
		LayoutGroup _currentGroup;
		public LayoutGroup CurrentGroup {
			get { return _currentGroup; }
			set { _currentGroup = value; }
		}
		public RightButtonMenuManager(ILayoutControl control) {
			Owner = control;
		}
		public virtual void ShowPopUpMenu(HitInfo.BaseLayoutItemHitInfo hi, LayoutGroup lg, Point showPopupPoint) {
			ShowPopUpMenu(hi, lg, showPopupPoint, false);
		}
		internal virtual void ShowPopUpMenuForTempalte(TemplateItemsList templateList, Point showPopupPoint) {
			if(Owner.CustomizationMode == CustomizationModes.Quick && !Owner.DesignMode) return;
			CreatePopMenuMenuForTemplateManager(templateList);
			menu.ShowMenu(showPopupPoint);
		}
		private void CreatePopMenuMenuForTemplateManager(TemplateItemsList templateList) {
			ArrayList menuItemInfoList = new ArrayList();
			menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DeleteTemplateText), GetRemoveImage(), new EventHandler(templateList.DeleteTemplate), false, false));
			menu = CreatePopMenuMenuCore(Owner, menuItemInfoList);
		}
		public virtual void ShowPopUpMenu(HitInfo.BaseLayoutItemHitInfo hi, LayoutGroup lg, Point showPopupPoint, bool isLayoutTreeView) {
			if(Owner.CustomizationMode == CustomizationModes.Quick && !Owner.DesignMode) return;
			if(lg != null && lg.Owner != null) {
				CurrentGroup = lg;
				if(hi.Item != null & (lg.Owner.EnableCustomizationMode || Owner.DesignMode)) {
					BaseLayoutItem item = hi.Item;
					if(item.Parent != null) {
						if(item.Parent.SelectedItems.Count < 2 && !item.Selected) {
							item.Owner.RootGroup.ClearSelection();
							HitInfo.TabbedGroupHitInfo thi = hi as HitInfo.TabbedGroupHitInfo;
							if(thi != null && thi.TabbedGroupHotPageIndex >= 0){
								TabbedGroup tg = (TabbedGroup)item;
								item = (tg).VisibleTabPages[thi.TabbedGroupHotPageIndex];
							}
							item.Selected = true;
						}
					}
				}
				CreatePopMenuMenu();
				PopupMenuShowingEventArgs ma = new PopupMenuShowingEventArgs(menu.Menu, hi);
				Owner.RaiseShowCustomizationMenu(ma);
				if(ma.Menu == null || !ma.Allow || ma.Menu.Items.Count == 0) return;
				if(isLayoutTreeView) {
					Owner.RaiseShowLayoutTreeViewContextMenu(ma);
					if(ma.Menu == null || !ma.Allow || ma.Menu.Items.Count == 0) return;
				}
				if((!this.Owner.DesignMode && (Owner.MenuManager != null))) {
					Owner.MenuManager.ShowPopupMenu(menu.Menu, Owner.Control, showPopupPoint);
				} else {
					menu.ShowMenu(showPopupPoint);
				}
			}
		}
		public static ICollection GetSelectedObjects(LayoutGroup group) {
			if(group == null) return null;
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> list = sh.GetItemsList(group);
			ArrayList selectionCore = new ArrayList();
			for(int i = 0; i < list.Count; i++) {
				if(list[i].IsDisposing) continue;
				selectionCore.Add(list[i]);
			}
			if(group.Selected) ((ArrayList)selectionCore).Add(group);
			return selectionCore;
		}
		public static BaseLayoutItem GetLocalSelectedParent(ILayoutControl owner) {
			if(owner == null || owner.RootGroup == null) return null;
			BaseLayoutItem bli1, bli2, localParent = null;
			ArrayList localSelObjectes = new ArrayList(GetSelectedObjects(owner.RootGroup));
			if(localSelObjectes.Count > 1) {
				foreach(object selObj1 in localSelObjectes) {
					foreach(object selObj2 in localSelObjectes) {
						if(selObj1 == selObj2) continue;
						bli1 = selObj1 as BaseLayoutItem;
						bli2 = selObj2 as BaseLayoutItem;
						if(bli1 != null && bli2 != null) {
							if(bli1.Parent != bli2.Parent && (bli2.Parent != bli1 || bli1.Parent != bli2)) return null;
							if(bli1.Parent == bli2.Parent) {
								localParent = bli1.Parent;
							}
							if(localParent != null) {
								bool isLocalSelection = false;
								if(bli2.Parent == localParent && bli1.Parent == localParent) isLocalSelection = true;
								if(bli2 == localParent && bli1.Parent == localParent) isLocalSelection = true;
								if(bli1 == localParent && bli2.Parent == localParent) isLocalSelection = true;
								if(!isLocalSelection) return null;
							}
						}
					}
				}
			}
			return localParent;
		}
		public virtual void ShowPopUpMenu(Point hitPoint, Point showPopupPoint) {
			LayoutGroup lg = GetLocalSelectedParent(Owner) as LayoutGroup;
			if(lg == null) lg = Owner.GetGroupAtPoint(hitPoint);
			HitInfo.BaseLayoutItemHitInfo hi = lg.CalcHitInfo(hitPoint, false);
			ShowPopUpMenu(hi, lg, showPopupPoint);
		}
		public void ShowPopUpMenu(Point p) {
			ShowPopUpMenu(p, p);
		}
		void CreatePopMenuMenu() {
			ArrayList menuItemInfoList = new ArrayList();
			if(Owner.DesignMode) CreateDisignModePopUpData(menuItemInfoList);
			else CreateRuntimeModePopUpData(menuItemInfoList);
			menu = CreatePopMenuMenuCore(Owner, menuItemInfoList);
		}
		protected virtual RightButtonMenuBase CreatePopMenuMenuCore(ILayoutControl owner, ArrayList menuItemInfoList) {
		   return new RightButtonMenuBase(Owner, menuItemInfoList);
		}
		public virtual void OnGroup(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				CurrentGroup.CreateGroupForSelectedItems();
			}
		}
		public virtual void OnShowText(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				if(CurrentGroup.Selected) {
					CurrentGroup.TextVisible = !CurrentGroup.TextVisible;
				}
				foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
					if(item is TabbedControlGroup)
						continue;
					if(!item.TextVisible) {
						LayoutControlItem controlItem = item as LayoutControlItem;
						if(controlItem != null && controlItem.TextAlignMode == TextAlignModeItem.CustomSize && controlItem.TextSize.IsEmpty) {
							controlItem.TextAlignMode = TextAlignModeItem.UseParentOptions;
						}
					}
					item.TextVisible = !item.TextVisible;
				}
			}
		}
		public virtual void OnHideSpace(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
					LayoutControlItem controlItem = item as LayoutControlItem;
					if(controlItem != null && !controlItem.TextVisible) {
						if(controlItem.TextAlignMode != TextAlignModeItem.CustomSize) {
							controlItem.TextAlignMode = TextAlignModeItem.CustomSize;
							controlItem.TextSize = Size.Empty;
						}
						else controlItem.TextAlignMode = TextAlignModeItem.UseParentOptions;
					}
				}
			}
		}
		public virtual void OnHideItem(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				ArrayList list = new ArrayList(CurrentGroup.SelectedItems);
				foreach(BaseLayoutItem item in list) {
					item.HideToCustomization();
				}
			}
		}
		public virtual void OnSetTextLocationTop(object sender, EventArgs eventArgs) {
			SetTextLocationCore(Locations.Top);
		}
		public virtual void OnSetTextLocationBottom(object sender, EventArgs eventArgs) {
			SetTextLocationCore(Locations.Bottom);
		}
		public virtual void OnSetTextLocationLeft(object sender, EventArgs eventArgs) {
			SetTextLocationCore(Locations.Left);
		}
		public virtual void OnSetTextLocationRight(object sender, EventArgs eventArgs) {
			SetTextLocationCore(Locations.Right);
		}
		void SetTextLocationCore(Locations location) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				if(CurrentGroup.Selected) {
					CurrentGroup.TextLocation = location;
				}
				foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
					item.TextLocation = location;
				}
			}
		}
		public virtual void OnResetConstraintsToDefault(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
					LayoutControlItem citem = item as LayoutControlItem;
					if(citem != null) {
						citem.SizeConstraintsType = SizeConstraintsType.Default;
					}
				}
			}
		}
		public virtual void OnFreeSizing(object sender, EventArgs eventArgs) {
			LockCore(false, false);
		}
		public virtual void OnLockItemWidth(object sender, EventArgs eventArgs) {
			LockCore(true, false);
		}
		public virtual void OnLockItemHeight(object sender, EventArgs eventArgs) {
			LockCore(false, true);
		}
		public virtual void OnLockItemSize(object sender, EventArgs eventArgs) {
			LockCore(true, true);
		}
		void LockCore(bool lockW, bool lockH) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
					LayoutControlItem citem = item as LayoutControlItem;
					if(citem != null) {
						Size sz = citem.Size;
						Size minSize = citem.MinSize;
						Size maxSize = citem.MaxSize;
						citem.BeginInit();
						if(citem.SizeConstraintsType == SizeConstraintsType.Default) {
							citem.SizeConstraintsType = SizeConstraintsType.Custom;
						}
						if(lockW && !lockH) {
							citem.MaxSize = new Size(sz.Width, maxSize.Height);
							citem.MinSize = new Size(sz.Width, minSize.Height);
						}
						if(lockH && !lockW) {
							citem.MaxSize = new Size(maxSize.Width, sz.Height);
							citem.MinSize = new Size(minSize.Width, sz.Height);
						}
						if(lockH && lockW) {
							citem.MaxSize = sz;
							citem.MinSize = sz;
						}
						if(!lockH && !lockW) {
							citem.MaxSize = Size.Empty;
							citem.MinSize = citem.CalcDefaultMinMaxSize(true);
						}
						citem.EndInit();
						citem.Invalidate();
					}
				}
			}
			if(CurrentGroup != null && CurrentGroup.Owner != null) CurrentGroup.Owner.SetIsModified(true);
		}
		public virtual void OnUngroup(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			if(CurrentGroup.Parent != null) {
				CurrentGroup.Selected = true;
				CurrentGroup = (LayoutControlGroup)CurrentGroup.Parent;
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					CurrentGroup.UngroupSelected();
				}
			}
		}
		public virtual void OnChangeLayoutModeToFlow(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			if(CurrentGroup.Parent != null) {
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					CurrentGroup.LayoutMode = LayoutMode.Flow;
				}
			}
		}
		public virtual void OnChangeLayoutModeToRegular(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			if(CurrentGroup.Parent != null) {
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					CurrentGroup.LayoutMode = LayoutMode.Regular;
				}
			}
		}
		public virtual void OnChangeLayoutModeToTable(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			if(CurrentGroup.Parent != null) {
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					CurrentGroup.LayoutMode = LayoutMode.Table;
				}
			}
		}
		public virtual void OnCreateEmptySpaceItem(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			EmptySpaceItem item = CurrentGroup.Owner.CreateEmptySpaceItem(null);
			CurrentGroup.Add(item);
		}
		public virtual void OnResetLayout(object sender, EventArgs eventArgs) {
			string text = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LayoutResetConfirmationText);
			string caption = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LayoutResetConfirmationDialogCaption);
			if(XtraMessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
				Owner.RestoreDefaultLayout();
			}
		}
		public virtual void OnCreateTabbedGroupForGroup(object sender, EventArgs eventArgs) {
			if(!(Owner as ISupportImplementor).Implementor.AllowUseTabbedGroups) return;
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
			if(CurrentGroup.Parent != null) {
				CurrentGroup = (LayoutControlGroup)CurrentGroup.Parent;
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					CurrentGroup.CreateTabbedGroupForSelectedGroup();
				}
			}
		}
		public virtual void OnAddTab(object sender, EventArgs eevent) {
			if(!(Owner as ISupportImplementor).Implementor.AllowUseTabbedGroups) return;
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, true);
			TabbedGroup group = CurrentGroup.SelectedItems[0] as TabbedGroup;
			using(new SafeBaseLayoutItemChanger(group)) {
				group.AddTabPage();
			}
		}
		public virtual void OnBestFit(object sender, EventArgs eevent) {
			Owner.BestFit();
		}
		public virtual void OnUngroupTabbedGroup(object sender, EventArgs eventArgs) {
			CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, true);
			CurrentGroup.UngroupTabbedGroup();
		}
		protected virtual void CreateDisignModePopUpData(ArrayList menuItemInfoList) {
			CreateShowCustomizeMenuPopUpData(menuItemInfoList);
			CreateTextPopUpMenuData(menuItemInfoList);
			CreateGroupPopUpMenuData(menuItemInfoList);
			CreateTabPopUpMenuData(menuItemInfoList);
			CreateLockMenuData(menuItemInfoList);
			CreateConvertToMenuData(menuItemInfoList);
			CreateTemplateMenuData(menuItemInfoList);
		}
		void CreateConvertToMenuData(ArrayList menuItemInfoList) {
			if(CanChangeLayoutModeToRootOrWithNestedGroups()) {
				switch(CurrentGroup.LayoutMode) {
					case LayoutMode.Regular:
						menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ConvertToTableLayoutText), GetTableImage(), new EventHandler(OnChangeLayoutModeToTableRoot), false, false));
						break;
					case LayoutMode.Table:
						menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ConvertToRegularLayoutText), GetConvertRegularImage(), new EventHandler(OnChangeLayoutModeToRegularRoot), false, false));
						break;
				}
				return;
			}
			if(CanChangeLayoutModeForChildGroup()) {
				ParentMenuItemInfo parentMI = new ParentMenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ConvertToText), null, null, false, false);
				menuItemInfoList.Add(parentMI);
				switch(CurrentGroup.LayoutMode) {
					case LayoutMode.Regular:
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.FlowLayoutText), GetConvertFlowImage(), new EventHandler(OnChangeLayoutModeToFlow), false, false));
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TableLayoutText), GetTableImage(), new EventHandler(OnChangeLayoutModeToTable), false, false));
						break;
					case LayoutMode.Flow:
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.RegularLayoutText), GetConvertRegularImage(), new EventHandler(OnChangeLayoutModeToRegular), false, false));
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TableLayoutText), GetTableImage(), new EventHandler(OnChangeLayoutModeToTable), false, false));
						break;
					case LayoutMode.Table:
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.FlowLayoutText), GetConvertFlowImage(), new EventHandler(OnChangeLayoutModeToFlow), false, false));
						parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.RegularLayoutText), GetConvertRegularImage(), new EventHandler(OnChangeLayoutModeToRegular), false, false));
						break;
				}
			}
		}
		private void OnChangeLayoutModeToRegularRoot(object sender, EventArgs e) {
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				CurrentGroup.LayoutMode = LayoutMode.Regular;
			}
		}
		private void OnChangeLayoutModeToTableRoot(object sender, EventArgs e) {
			using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
				CurrentGroup.LayoutMode = LayoutMode.Table;
			}
		}
		private bool CanChangeLayoutModeToRootOrWithNestedGroups() {
			if(CurrentGroup != null && CurrentGroup.SelectedItems.Count == 0 && CurrentGroup.IsRoot && CurrentGroup.Selected) {
				return true;
			}
			if(CurrentGroup != null && CurrentGroup.Parent != null && CurrentGroup.LayoutMode != LayoutMode.Flow && CurrentGroup.Selected && CurrentGroup.Parent.SelectedItems.Count == 1) {
				foreach(BaseLayoutItem bli in CurrentGroup.Items) if(bli is LayoutItemContainer) return true;
			}
			return false;
		}
		protected BaseLayoutItem GetActiveItem() {
			BaseLayoutItem activeItem = null;
			if(CurrentGroup.Selected) activeItem = CurrentGroup;
			if(CurrentGroup.SelectedItems.Count != 0 && !(CurrentGroup.SelectedItems[0] is TabbedControlGroup))
				activeItem = CurrentGroup.SelectedItems[0];
			return activeItem;
		}
		protected string GetShowTextMenuText() {
			string textShowText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ShowTextMenuItem);
			BaseLayoutItem activeItem = GetActiveItem();
			if(activeItem != null && activeItem.TextVisible) {
				textShowText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HideTextMenuItem);
			}
			return textShowText;
		}
		protected string GetHideSpaceMenuText() {
			string textHideSpace = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HideSpaceMenuItem);
			LayoutControlItem activeItem = GetActiveItem() as LayoutControlItem;
			if(activeItem != null && activeItem.TextAlignMode == TextAlignModeItem.CustomSize) {
				textHideSpace = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ShowSpaceMenuItem);
			}
			return textHideSpace;
		}
		void OnClick(object Sender, System.EventArgs e) {
			if(!Owner.EnableCustomizationMode) {
				Owner.ShowCustomizationForm();
			} else {
				Owner.HideCustomizationForm();
			}
		}
		protected bool IsSelectionEmpty {
			get {
				return !(CurrentGroup.SelectedItems.Count > 0);
			}
		}
		protected LayoutControlItem GetSelectedItem {
			get {
				if(IsSelectionEmpty) return null;
				return CurrentGroup.SelectedItems[0] as LayoutControlItem;
			}
		}
		protected LayoutControlGroup GetSelectedGroup {
			get {
				if(CurrentGroup.Selected) return (LayoutControlGroup)CurrentGroup;
				if(IsSelectionEmpty) return null;
				return CurrentGroup.SelectedItems[0] as LayoutControlGroup;
			}
		}
		protected TabbedControlGroup GetSelectedTabbedGroup {
			get {
				if(IsSelectionEmpty) return null;
				return CurrentGroup.SelectedItems[0] as TabbedControlGroup;
			}
		}
		protected bool CanUnLock() {
			LayoutControlItem item = GetSelectedItem;
			if(item == null) return false;
			if(item.SizeConstraintsType == SizeConstraintsType.Custom && item.MinSize == item.MaxSize) return true;
			return false;
		}
		protected bool CanLock() {
			LayoutControlItem item = GetSelectedItem;
			if(item == null) return false;
			if(item.SizeConstraintsType == SizeConstraintsType.Default) return true;
			if(item.SizeConstraintsType == SizeConstraintsType.Custom && (item.Size != item.MinSize && item.Size == item.MaxSize)) return true;
			return false;
		}
		protected bool CanResetLayout() {
			return (Owner as ISupportImplementor).Implementor.CanRestoreDefaultLayout;
		}
		protected bool CanRename() {
			LayoutControlItem item = GetSelectedItem;
			LayoutControlGroup group = GetSelectedGroup;
			if(CurrentGroup.SelectedItems.Count > 1) return false;
			if(item != null) { return item.TextVisible; }
			if(group != null && group.ParentTabbedGroup != null) { return true; }
			if(group != null) { return group.TextVisible && group.GroupBordersVisible; }
			return false;
		}
		protected bool CanGroupItems() {
			if(CurrentGroup == null) return false;
			return CurrentGroup.CanGroupSelectedItems;
		}
		protected bool CanHideItem() {
			if(CurrentGroup == null) return false;
			if(CurrentGroup.SelectedItems.Count == 0) return false;
			bool allowHide = false;
			foreach(BaseLayoutItem item in CurrentGroup.SelectedItems) {
				allowHide |= item.AllowHide;
			}
			return allowHide;
		}
		protected bool CanCreateEmptySpaceItem() {
			if(CurrentGroup != null && (CurrentGroup.Selected || CurrentGroup.SelectedItems.Count > 0)) {
				return true;
			}
			return false;
		}
		protected bool CanUngroupItems() {
			if(CurrentGroup != null && CurrentGroup.Parent != null) {
				return CurrentGroup.Parent.CanUngroupSelectedGroup;
			}
			return false;
		}
		protected bool CanChangeLayoutModeForChildGroup() {
			if(CurrentGroup != null && CurrentGroup.Parent != null) {
				return CurrentGroup.Parent.CanChangeLayoutModeForChildSelectedGroup;
			}
			return false;
		}
		protected bool CanCreateTabbedGroupForGroup() {
			if(CurrentGroup == null || CurrentGroup.Parent == null) return false;
			return CurrentGroup.Parent.CanCreateTabbedGroupForSelectedGroup;
		}
		protected bool CanDestroyTabbedGroup() {
			if(CurrentGroup == null) return false;
			return CurrentGroup.CanUngroupTabbedGroup;
		}
		protected bool CanAddTab() {
			if(CurrentGroup == null) return false;
			return CurrentGroup.CanAddTab;
		}
		protected bool CanShowTextLocationMenu() {
			LayoutControlItem item = GetSelectedItem;
			LayoutControlGroup group = GetSelectedGroup;
			TabbedControlGroup tgroup = GetSelectedTabbedGroup;
			bool fixedCanShowItemTextLocation = true;
			bool fixedCanShowGroupTextLocation = true;
			bool fixedCanShowTabbedGroupTextLocation = true;
			if(item is IFixedLayoutControlItem) {
				fixedCanShowItemTextLocation = (item as IFixedLayoutControlItem).AllowChangeTextLocation;
			}
			if(group is IFixedLayoutControlItem) {
				fixedCanShowGroupTextLocation = (group as IFixedLayoutControlItem).AllowChangeTextLocation;
			}
			if(tgroup is IFixedLayoutControlItem) {
				fixedCanShowTabbedGroupTextLocation = (tgroup as IFixedLayoutControlItem).AllowChangeTextLocation;
			}
			if(item != null && item.TextVisible) { return fixedCanShowItemTextLocation; }
			if(group != null && group.TextVisible && group.GroupBordersVisible) { return fixedCanShowGroupTextLocation; }
			if(tgroup != null && tgroup.TextVisible) { return fixedCanShowTabbedGroupTextLocation; }
			return false;
		}
		protected bool CanHideSpace() {
			bool canHide = false;
			LayoutControlItem item = GetSelectedItem;
			if(item != null) {
				canHide = !item.TextVisible;
			}
			return Owner.OptionsItemText.AlignControlsWithHiddenText && canHide;
		}
		protected bool CanShowText() {
			LayoutControlItem item = GetSelectedItem;
			LayoutControlGroup group = GetSelectedGroup;
			bool fixedCanShowItemText = true;
			bool fixedCanShowGroupText = true;
			if(item is IFixedLayoutControlItem) {
				fixedCanShowItemText = (item as IFixedLayoutControlItem).AllowChangeTextVisibility;
			}
			if(group is IFixedLayoutControlItem) {
				fixedCanShowGroupText = (group as IFixedLayoutControlItem).AllowChangeTextVisibility;
			}
			if(item != null) { return fixedCanShowItemText; }
			if(group != null) { return fixedCanShowGroupText; }
			return false;
		}
		protected bool CanBestFit() {
		   if(Owner.EnableCustomizationMode) return true;	   
		   return false;
		}
		protected BaseLayoutItem GetSelectedBaseLayoutItem {
			get {
				if(CurrentGroup.Selected) return CurrentGroup;
				if(IsSelectionEmpty) return null;
				return CurrentGroup.SelectedItems[0] as BaseLayoutItem;
			}
		}
		private event EventHandler MakeTemplate;
		void CreateTemplate(object sender, EventArgs eventArgs) {
			if(MakeTemplate != null) MakeTemplate(this, null);
			TemplateManager templateManager = new TemplateManager();
			templateManager.CreateTemplate(CurrentGroup.Owner as LayoutControl);
		}
		void OnRename(object sender, EventArgs eventArgs) {
			BaseLayoutItem selectedItem = GetSelectedBaseLayoutItem;
			Rectangle rect = Rectangle.Empty;
			if(selectedItem is LayoutControlItem) {
				rect = selectedItem.ViewInfo.TextAreaRelativeToControl;
				rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
			}
			LayoutGroup group = selectedItem as LayoutGroup;
			if(group != null) {
				if(group.ParentTabbedGroup == null) {
					rect = group.ViewInfo.BorderInfo.TextBounds;
					rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
				} else {
					int tabIndex = group.ParentTabbedGroup.VisibleTabPages.IndexOf(group);
					rect = group.ParentTabbedGroup.ViewInfo.GetScreenTabCaptionRect(tabIndex);
					rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
				}
			}
			(Owner as ISupportImplementor).Implementor.RenameItemManager.Rename(selectedItem, rect);
		}
		protected virtual Image GetShowTextImage() {
			Image img = GetShowItemTextImage();
			BaseLayoutItem activeItem = GetActiveItem();
			if(activeItem != null && activeItem.TextVisible) {
				img = GetHideItemTextImage();
			}
			return img;
		}
		protected virtual Image GetHideSpaceImage() {
			Image img = GetHideItemSpaceImage();
			LayoutControlItem activeItem = GetActiveItem() as LayoutControlItem;
			if(activeItem != null && activeItem.TextAlignMode == TextAlignModeItem.CustomSize) {
				img = GetShowItemSpaceImage();
			}
			return img;
		}
		protected virtual Image GetShowHideCustomizationFormImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[0];
		}
		protected virtual Image GetRenameImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[1];
		}
		protected virtual Image GetTextPositionImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[2];
		}
		protected virtual Image GetHideItemImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[3];
		}
		protected virtual Image GetGroupItemsImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[4];
		}
		protected virtual Image GetUnGroupItemsImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[5];
		}
		protected virtual Image GetCreateTabbedGroupImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[6];
		}
		protected virtual Image GetUnGroupTabbedGroupImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[7];
		}
		protected virtual Image GetAddTabImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[8];
		}
		protected virtual Image GetLockSizeImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[9];
		}
		protected virtual Image GetLockWidthImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[10];
		}
		protected virtual Image GetLockHeightImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[11];
		}
		protected virtual Image GetShowItemTextImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[12];
		}
		protected virtual Image GetShowItemSpaceImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[22];
		}
		protected virtual Image GetLockMenuGroupImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[13];
		}
		protected virtual Image GetResetConstraintsToDefaultsImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[14];
		}
		protected virtual Image GetFreeSizingImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[15];
		}
		protected virtual Image GetHideItemTextImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[16];
		}
		protected virtual Image GetHideItemSpaceImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[23];
		}
		protected virtual Image GetResetLayoutImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[17];
		}
		protected virtual Image GetCreateEmptySpaceItemImage() {
			return LayoutControlImageStorage.Default.CustomizationFormMenu.Images[21];
		}
		protected virtual Image GetBestFitImage() {
			return LayoutControlImageStorage.Default.BestFit;
		}
		protected virtual Image GetTableImage() {
			return LayoutControlImageStorage.Default.Table;
		}
		protected virtual Image GetRemoveImage() {
			return LayoutControlImageStorage.Default.Remove;
		}
		protected virtual Image GetConvertRegularImage() {
			return LayoutControlImageStorage.Default.ConvertRegular;
		}
		protected virtual Image GetConvertFlowImage() {
			return LayoutControlImageStorage.Default.ConvertFlow;
		}
		protected virtual Image GetCreateTemplateImage() {
			return LayoutControlImageStorage.Default.CreateTemplate;
		}
		protected void CreateShowCustomizeMenuPopUpData(ArrayList menuItemInfoList) {
			String name = !Owner.EnableCustomizationMode ? LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ShowCustomizationFormMenuText) : LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HideCustomizationFormMenuText);
			menuItemInfoList.Add(new MenuItemInfo(name, GetShowHideCustomizationFormImage(), new EventHandler(OnClick), false, false));
		}
		protected void CreateTextPopUpMenuData(ArrayList menuItemInfoList) {
			if(CanRename()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.RenameMenuText), GetRenameImage(), new EventHandler(OnRename), false, false));
			if(CanShowText()) {
				menuItemInfoList.Add(new MenuItemInfo(GetShowTextMenuText(), GetShowTextImage(), new EventHandler(OnShowText), false, true));
				if(CanHideSpace())
					menuItemInfoList.Add(new MenuItemInfo(GetHideSpaceMenuText(), GetHideSpaceImage(), new EventHandler(OnHideSpace), false, true));
			}
			CreateTextLocationMenuData(menuItemInfoList);
		}
		protected void CreateTemplateMenuData(ArrayList menuItemInfoList) {
			if(CanMakeTemplate()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CreateTemplate), GetCreateTemplateImage(), new EventHandler(CreateTemplate), false, false));
		}
		protected bool CanMakeTemplate() {
			if(Owner == null) return false;
			if(Owner.RootGroup == null) return false;
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> list = sh.GetItemsList(Owner.RootGroup);
			if(list != null && CurrentGroup != null && CurrentGroup.Owner.DesignMode && CurrentGroup.Owner is LayoutControl && TemplateManager.CanMakeTemplate(list)) return true;
			return false;
		}
		protected void CreateGroupPopUpMenuData(ArrayList menuItemInfoList) {
			if(CanHideItem()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HideItemMenutext), GetHideItemImage(), new EventHandler(OnHideItem), false, false));
			if(CanGroupItems()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.GroupItemsMenuText), GetGroupItemsImage(), new EventHandler(OnGroup), false, false));
			if(CanUngroupItems()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.UnGroupItemsMenuText), GetUnGroupItemsImage(), new EventHandler(OnUngroup), false, false));
			if(CanCreateEmptySpaceItem()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CreateEmptySpaceItem), GetCreateEmptySpaceItemImage(), new EventHandler(OnCreateEmptySpaceItem), false, false));
		}
		protected void CreateLockMenuData(ArrayList menuItemInfoList) {
			LayoutControlItem citem = GetSelectedItem;
			if (citem == null) return;
			ParentMenuItemInfo parentMI = new ParentMenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LockMenuGroup), GetLockMenuGroupImage(), null, false, false);
			menuItemInfoList.Add(parentMI);
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ResetConstraintsToDefaultsMenuItem), GetResetConstraintsToDefaultsImage(), new EventHandler(OnResetConstraintsToDefault), citem.SizeConstraintsType == SizeConstraintsType.Default, true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.FreeSizingMenuItem), GetFreeSizingImage(), new EventHandler(OnFreeSizing), citem.SizeConstraintsType != SizeConstraintsType.Default && (citem.MaxSize == Size.Empty), true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LockSizeMenuItem), GetLockSizeImage(), new EventHandler(OnLockItemSize), citem.SizeConstraintsType != SizeConstraintsType.Default && (citem.MinSize == citem.MaxSize), true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LockWidthMenuItem), GetLockWidthImage(), new EventHandler(OnLockItemWidth), citem.SizeConstraintsType != SizeConstraintsType.Default && (citem.MinSize.Width == citem.MaxSize.Width) && (citem.MaxSize.Height != citem.MinSize.Height), true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LockHeightMenuItem), GetLockHeightImage(), new EventHandler(OnLockItemHeight), citem.SizeConstraintsType != SizeConstraintsType.Default && (citem.MinSize.Height == citem.MaxSize.Height) && (citem.MaxSize.Width != citem.MinSize.Width), true));
		}
		protected void CreateTextLocationMenuData(ArrayList menuItemInfoList) {
			if(!CanShowTextLocationMenu()) return;
			ParentMenuItemInfo parentMI = new ParentMenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TextPositionMenuText), GetTextPositionImage(), null, false, false);
			menuItemInfoList.Add(parentMI);
			BaseLayoutItem baseLayoutItem = GetSelectedBaseLayoutItem;
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TextPositionTopMenuText), null, new EventHandler(OnSetTextLocationTop), baseLayoutItem.TextLocation == Locations.Top, true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TextPositionBottomMenuText), null, new EventHandler(OnSetTextLocationBottom), baseLayoutItem.TextLocation == Locations.Bottom, true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TextPositionLeftMenuText), null, new EventHandler(OnSetTextLocationLeft), baseLayoutItem.TextLocation == Locations.Left, true));
			parentMI.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TextPositionRightMenuText), null, new EventHandler(OnSetTextLocationRight), baseLayoutItem.TextLocation == Locations.Right, true));
		}
		protected void CreateTabPopUpMenuData(ArrayList menuItemInfoList) {
			if(!(Owner as ISupportImplementor).Implementor.AllowUseTabbedGroups) return;
			if(CanCreateTabbedGroupForGroup())
				menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CreateTabbedGroupMenuText), GetCreateTabbedGroupImage(), new EventHandler(OnCreateTabbedGroupForGroup), false, false));
			else
				if(CanCreateTabbedGroupForItems())
					menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CreateTabbedGroupMenuText), GetCreateTabbedGroupImage(), new EventHandler(OnCreateTabbedGroupForItems), false, false));
			if(CanDestroyTabbedGroup()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.UnGroupTabbedGroupMenuText), GetUnGroupTabbedGroupImage(), new EventHandler(OnUngroupTabbedGroup), false, false));
			if(CanAddTab()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.AddTabMenuText), GetAddTabImage(), new EventHandler(OnAddTab), false, false));
		}
		protected bool CanCreateTabbedGroupForItems() {
			if(!CanGroupItems()) return false;
			foreach(BaseLayoutItem bli in CurrentGroup.SelectedItems) {
				if(bli is TabbedGroup) return false;
			}
			return true;
		}
		internal bool LockUpdate;
		public virtual void OnCreateTabbedGroupForItems(object sender, EventArgs e) {
			LockUpdate = true;
			try {
				CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
				LayoutGroup group = null;
				using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
					group = CurrentGroup.CreateGroupForSelectedItems();
				}
				group.Owner.RootGroup.ClearSelection();
				group.Selected = true;
				CurrentGroup = group;
				if(!(Owner as ISupportImplementor).Implementor.AllowUseTabbedGroups) return;
				if(Owner.UndoManager != null) Owner.UndoManager.LockUndo();
				CurrentGroupChecker.Default.CheckCustom(CurrentGroup, true, true, false);
				if(CurrentGroup.Parent != null) {
					CurrentGroup = (LayoutControlGroup)CurrentGroup.Parent;
					using(new SafeBaseLayoutItemChanger(CurrentGroup)) {
						CurrentGroup.CreateTabbedGroupForSelectedGroup();
					}
				}
				if(Owner.UndoManager != null) Owner.UndoManager.UnlockUndo();
			} finally {
				LockUpdate = false;
			}
		}
		protected void CreateMenuSeparator(ArrayList menuItemInfoList) {
			menuItemInfoList.Add(new MenuItemInfo(RightButtonMenuBase.MenuSeparator, null, null, false, false));
		}
		protected virtual void CreateRuntimeModePopUpData(ArrayList menuItemInfoList) {
			CreateShowCustomizeMenuPopUpData(menuItemInfoList);
			if(CanResetLayout()) {
				Image menuImage = GetResetLayoutImage();
				menuItemInfoList.Add(new MenuItemInfo(
					LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ResetLayoutMenuText),
					menuImage, new EventHandler(OnResetLayout), false, false));
			}
			if(CanBestFit()) menuItemInfoList.Add(new MenuItemInfo(LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.BestFitMenuText), GetBestFitImage(), new EventHandler(OnBestFit), false, false));
			CreateTextPopUpMenuData(menuItemInfoList);
			CreateConvertToMenuData(menuItemInfoList);
			CreateGroupPopUpMenuData(menuItemInfoList);
			CreateTabPopUpMenuData(menuItemInfoList);
			CreateLockMenuData(menuItemInfoList);
		}
	}
}
