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
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using System.Collections;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.XtraPivotGrid.FilterDropDown {
	public interface IPivotGridDropDownFilterEditOwner {
		CustomizationFormFields CustomizationFormFields { get; }
		void CloseFilter();
	}
	public abstract class PivotDropDownFilterEditBase : DropDownFilterEditBase {
		public PivotDropDownFilterEditBase(IPivotGridDropDownFilterEditOwner owner,
				PivotGridViewInfoData data, PivotGridField field, Rectangle bounds)
			: this(owner, null, data, field, bounds) { }
		public PivotDropDownFilterEditBase(IPivotGridDropDownFilterEditOwner owner, Control parentControl,
				PivotGridViewInfoData data, PivotGridField field, Rectangle bounds)
			: base(parentControl, bounds) {
			this.Field = field;
			this.Data = data;
			this.Owner = owner;
			if(parentControl == null) {
				IViewInfoControl viewInfoControl = data as IViewInfoControl;
				this.ParentControl = viewInfoControl != null ? viewInfoControl.ControlOwner : null;
			}
		}
		public PivotGridViewInfoData Data { get; private set; }
		public PivotGridField Field { get; private set; }
		public IPivotGridDropDownFilterEditOwner Owner { get; private set; }
		protected override bool CanShowPopup {
			get { return base.CanShowPopup && Data != null && Field != null; }
		}
		protected override void SetupContainerEdit() {
			base.SetupContainerEdit();
			ContainerEdit.Properties.LookAndFeel.ParentLookAndFeel = Data.ActiveLookAndFeel;
			ContainerEdit.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(OnCloseUp);
			if(!Field.DropDownFilterListSize.IsEmpty)
				ContainerEdit.Properties.PopupFormSize = Field.DropDownFilterListSize;
		}
		void OnCloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
			if(!HasContainerEdit) return;
			Field.DropDownFilterListSize = PopupFormContentSize;
		}
		protected abstract Size PopupFormContentSize { get; }
		protected override void OnClosed(object sender, ClosedEventArgs e) {
			if(Owner != null)
				Owner.CloseFilter();
			base.OnClosed(sender, e);
		}
	}
	public class PivotGridDropDownFilterEdit : PivotDropDownFilterEditBase {
		bool deferUpdates;
		protected internal new PivotFilterPopupContainerEdit ContainerEdit {
			get { return (PivotFilterPopupContainerEdit)base.ContainerEdit; }
		}
		public PivotGridDropDownFilterEdit(IPivotGridDropDownFilterEditOwner owner, 
				PivotGridViewInfoData data, PivotGridField field, Rectangle bounds)
			: this(owner, null, data, field, bounds, false) { }
		public PivotGridDropDownFilterEdit(IPivotGridDropDownFilterEditOwner owner, Control parentControl,
					PivotGridViewInfoData data, PivotGridField field, Rectangle bounds, bool deferUpdates)
			: base(owner, parentControl, data, field, bounds) {
				this.deferUpdates = deferUpdates;
		}
		protected override Size PopupFormContentSize {
			get { return ContainerEdit.PopupForm.ContentSize;  }
		}
		protected override BlobBaseEdit CreateContainerEdit() {
			PivotFilterItemsBase filterItems = CreateFilterItems();
			if(!Data.OptionsBehavior.UseAsyncMode)
				if(filterItems.Count == 0)
					filterItems.CreateItems();
				else
					filterItems.EnsureAvailableItems();
			return CreateFilterPopupContainerEdit(filterItems);
		}
		protected virtual PivotFilterPopupContainerEdit CreateFilterPopupContainerEdit(PivotFilterItemsBase filterItems) {
			return new PivotFilterPopupContainerEdit(filterItems);
		}
		protected virtual PivotFilterItemsBase CreateFilterItems() {
			PivotFilterItemsBase filter = null;
			if(Field.Group != null && Field.Group.IsFilterAllowed) {
				if(Owner != null && Owner.CustomizationFormFields != null)
					filter = Owner.CustomizationFormFields.GetGroupFilter(Field);
				if(filter == null)
					return new PivotGroupFilterItems(Data, Field, Field.CanFilterRadioMode, deferUpdates);
			} else {
				if(Owner != null && Owner.CustomizationFormFields != null)
					filter = Owner.CustomizationFormFields.GetFieldFilter(Field);
				if(filter == null)
					return new PivotGridFilterItems(Data, Field, Field.CanFilterRadioMode, Data.OptionsFilterPopup.ShowOnlyAvailableItems, deferUpdates);
			}
			return filter;
		}
	}
	public class PivotFilterPopupContainerEdit : FilterPopupContainerEditEx {
		public PivotFilterPopupContainerEdit(PivotFilterItemsBase filterItems)
			: base(filterItems) { }
		public new PivotFilterItemsBase FilterItems { get { return (PivotFilterItemsBase)base.FilterItems; } }
		public PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)FilterItems.Data; } }
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			if(Data.IsLocked) return;
			base.DoClosePopup(closeMode);
		}
		protected override PopupBaseForm CreatePopupForm() {
			if(FilterItems.Group != null && FilterItems.Group.IsFilterAllowed)
				return CreateGroupFilterPopupContainerForm();
			else
				return CreateFilterPopupContainerForm();
		}
		protected virtual PivotGroupFilterPopupContainerForm CreateGroupFilterPopupContainerForm() {
			return new PivotGroupFilterPopupContainerForm(this);
		}
		protected virtual PivotFilterPopupContainerForm CreateFilterPopupContainerForm() {
			return new PivotFilterPopupContainerForm(this);
		}
		public new FilterPopupContainerFormBase PopupForm { get { return (FilterPopupContainerFormBase)base.PopupForm; } }
	}
	public class PivotFilterPopupContainerForm : FilterPopupContainerFormBase {
		protected enum ContextMenuItemType { CollapseAll, ExpandAll };
		DXPopupMenu contextMenu;
		LoadingAnimator loadingAnimator;
		public new PivotFilterItemsBase FilterItems { get { return (PivotFilterItemsBase)base.FilterItems; } }
		protected new DXPopupMenu ContextMenu { get { return contextMenu; } }
		protected new PivotFilterPopupContainerEdit OwnerEdit { get { return (PivotFilterPopupContainerEdit)base.OwnerEdit; } }
		protected PivotGridViewInfoData Data { get { return OwnerEdit.Data; } }
		protected PivotGridOptionsFilterPopup Options { get { return Data.OptionsFilterPopup; } }
		protected bool IsDeferredFillingCore { get { return Data.OptionsBehavior.UseAsyncMode; } }
		protected override bool IsDeferredFilling { get { return IsDeferredFillingCore && FilterItems.Count == 0; } }
		protected LoadingAnimator LoadingAnimator { get { return loadingAnimator ?? (loadingAnimator = new LoadingAnimator(CheckListBox, LoadingAnimator.LoadingImageLine)); } }
		ILoadingPanelOwner LoadingPanelOwner {
			get { return Data as ILoadingPanelOwner; }
		}
		protected PivotGridField Field { get { return (PivotGridField)FilterItems.Field; } }
		protected PivotGridGroup Group { get { return FilterItems.Group; } }
		protected override bool IsRadioMode { get { return Field.CanFilterRadioMode; } }
		protected override CheckState ShowAllCheckState { get { return CheckedListBoxItem.GetCheckState(FilterItems.VisibleCheckState); } }
		protected override string GetShowAllItemCaption() { return FilterItems.ShowAllItemCaption; }
		protected override FilterPopupToolbarButtons ToolbarButtons {
			get { return Options.ToolbarButtons; }
		}
		protected override bool ShowToolbar {
			get { return Options.ShowToolbar; }
		}
		protected override bool AllowMultiSelectOption {
			get { return Options.AllowMultiSelect; }
		}
		protected override bool AllowIncrementalSearchOption {
			get { return Options.AllowIncrementalSearch; }
			set { Options.AllowIncrementalSearch = value; }
		}
		protected override bool IsRadioModeOption {
			get { return Options.IsRadioMode; }
			set { Options.IsRadioMode = value; }
		}
		public PivotFilterPopupContainerForm(PivotFilterPopupContainerEdit ownerEdit)
			: base(ownerEdit) {
			if (Options.AllowContextMenu) {
				this.contextMenu = CreateContextMenu();
				ContextMenu.CloseUp += OnContextMenuCloseUp;
			}
		}
		protected override IEnumerable VisibleItems {
			get { return FilterItems.VisibleItems; }
		}
		protected override void CheckItems(CheckState state) {
			FilterItems.CheckVisibleItems(state == CheckState.Checked);
		}
		public override void ShowPopupForm() {
			base.ShowPopupForm();
			if(IsDeferredFillingCore)
				CreateAndLoadItemsAsync();
		}
		void CreateAndLoadItemsAsync() {
			CreateAndLoadItemsAsync(null);
		}
		void CreateItemsCompleted(AsyncOperationResult asyncResult, AsyncCompletedHandler callback) {
			this.Enabled = true;
			if(CheckListBox == null)
				return;
			FillList();
			if(callback != null)
				callback(asyncResult);
		}
		void RecreateFilterItemsAsync(AsyncCompletedHandler callback) {
			this.Enabled = false;
			FilterItems.CreateItemsAsync(asyncResult => CreateItemsCompleted(asyncResult, callback));
		}
		void CreateAndLoadItemsAsync(AsyncCompletedHandler callback) {
			this.Enabled = false;
			if(IsDeferredFilling)
				FilterItems.CreateItemsAsync(asyncResult => CreateItemsCompleted(asyncResult, callback));
			else
				FilterItems.EnsureAvailableItemsAsync(asyncResult => CreateItemsCompleted(asyncResult, callback));
		}
		void OnContextMenuCloseUp(object sender, EventArgs e) {
			if (IsRadioMode)
				CheckListBox.ToggleItem(CheckListBox.SelectedIndex);
		}
		protected virtual void OnCheckListBoxMouseUp(object sender, MouseEventArgs e) {
			if (ContextMenu == null || ContextMenu.Items == null || ContextMenu.Items.Count == 0 || e.Button != MouseButtons.Right) return;
			ShowContextMenu(e.Location);
		}
		protected virtual void ShowContextMenu(Point pos) {
			MenuManagerHelper.ShowMenu(ContextMenu, Data.ActiveLookAndFeel, Data.MenuManager, CheckListBox, pos);
		}
		protected virtual DXPopupMenu CreateContextMenu() {
			return new DXPopupMenu();
		}
		protected virtual DXMenuItem CreateContextMenuItem(PivotGridStringId caption, ContextMenuItemType itemType) {
			DXMenuItem item = new DXMenuItem(PivotGridLocalizer.GetString(caption), OnContextMenuClick);
			item.Tag = itemType;
			return item;
		}
		protected override void Dispose(bool disposing) {
			DisposeContextMenu();
			base.Dispose(disposing);
		}
		protected virtual void DisposeContextMenu() {
			if (ContextMenu != null) {
				ContextMenu.HidePopup();
				ContextMenu.CloseUp -= OnContextMenuCloseUp;
				ContextMenu.Dispose();
			}
		}
		protected override void SubscribeListEvents() {
			base.SubscribeListEvents();
			CheckListBox.MouseUp += OnCheckListBoxMouseUp;
		}
		protected override void UnsubscribeListEvents() {
			base.UnsubscribeListEvents();
			CheckListBox.MouseUp -= OnCheckListBoxMouseUp;
		}
		protected virtual void OnContextMenuClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if (item == null || CheckListBox == null) return;
			ContextMenuItemType type = (ContextMenuItemType)item.Tag;
			switch (type) {
				case ContextMenuItemType.CollapseAll:
					CollapseAllItems();
					break;
				case ContextMenuItemType.ExpandAll:
					ExpandAllItems();
					break;
			}
		}
		protected override void SubscribeFormEvents() {
			base.SubscribeFormEvents();
			CheckListBox.Paint += checkedListBox_Paint;
			LoadingPanelOwner.ShowFilterPopupLoadingPanel += LoadingPanelOwner_ShowFilterPopupLoadingPanel;
		}
		void checkedListBox_Paint(object sender, PaintEventArgs e) {
			if (!LoadingPanelOwner.IsFilterPopupLoadingPanelVisible) {
				if (loadingAnimator != null)
					loadingAnimator.StopAnimation();
				return;
			}
			using (GraphicsCache cache = new GraphicsCache(e)) {
				LoadingAnimator.DrawAnimatedItem(cache, CheckListBox.ClientRectangle);
			}
		}
		void LoadingPanelOwner_ShowFilterPopupLoadingPanel(object sender, EventArgs e) {
			if (CheckListBox != null)
				CheckListBox.Invalidate();
		}
		protected override void UnsubscribeFormEvents() {
			base.UnsubscribeFormEvents();
			LoadingPanelOwner.ShowFilterPopupLoadingPanel -= LoadingPanelOwner_ShowFilterPopupLoadingPanel;
			CheckListBox.Paint -= checkedListBox_Paint;
			if (loadingAnimator != null)
				loadingAnimator.StopAnimation();
		}
		protected override void InitializeToolbarButtonsCore(FilterPopupToolbarButtons buttons) {
			base.InitializeToolbarButtonsCore(buttons);
			PivotToolbarCheckButton btnRadioMode = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.RadioMode];
			PivotToolbarCheckButton btnShowOnlyAvailableItems = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.ShowOnlyAvailableItems];
			btnShowOnlyAvailableItems.IsChecked = Options.ShowOnlyAvailableItems;
			btnShowOnlyAvailableItems.CheckedChanged += btnShowOnlyAvailableItems_CheckedChanged;
			PivotToolbarCheckButton btnShowNewValues = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.ShowNewValues];
			InitializeToolbarButtonShowNewValues(btnShowNewValues);
			btnShowNewValues.Enabled = !Options.IsRadioMode;
			PivotToolbarButton btnInvertFilter = Toolbar[FilterPopupToolbarButtons.InvertFilter];
			btnInvertFilter.Click += btnInvertFilter_Click;
			btnInvertFilter.Enabled = !Options.IsRadioMode;
		}
		protected virtual void InitializeToolbarButtonShowNewValues(PivotToolbarCheckButton btnShowNewValues) {
			FilterItems.ShowNewValues = Field.ShowNewValues;
			btnShowNewValues.IsChecked = Field.ShowNewValues;
			btnShowNewValues.CheckedChanged += (s, e) => FilterItems.ShowNewValues = e.IsChecked;
		}
		protected override void SetRadioModeOptions(bool isChecked) {
			base.SetRadioModeOptions(isChecked);
			FilterItems.RadioMode = isChecked;
		}
		protected override void FillFilterItems() {
			RecreateFilterItems(asyncResult => {
				UpdateShowAllItem();
			});
		}
		void UpdateShowAllItem() {
			ShowAllItem.CheckState = ShowAllCheckState;
			ShowAllItem.Value = GetShowAllItemCaption();
		}
		void RecreateFilterItems(AsyncCompletedHandler callback) {
			if(IsDeferredFillingCore)
				RecreateFilterItemsAsync(callback);
			else {
				CreateAndLoadItems();
				if (callback != null)
					callback(null);
			}
		}
		void CreateAndLoadItems() {
			FilterItems.CreateItems();
			FillList();
		}
		void btnInvertFilter_Click(object sender, EventArgs e) {
			InvertFilter();
		}
		public virtual void InvertFilter() {
			if (IsRadioMode) return;
			UnsubscribeListEvents();
			CheckListBox.BeginUpdate();
			try {
				FilterItems.InvertVisibleCheckState();
				CheckListBox.InvertCheckState();
				UpdateShowAllItem();
			}
			finally {
				CheckListBox.EndUpdate();
				SubscribeListEvents();
			}
			UpdateOKButtonEnabled();
		}
		void btnShowOnlyAvailableItems_CheckedChanged(object sender, CheckedChangedEventArgs e) {
			FilterItems.ShowOnlyAvailableItems = Options.ShowOnlyAvailableItems = e.IsChecked;
			RecreateFilterItems(asyncResult => {
				ChangeFilterType(Toolbar.IsChecked(FilterPopupToolbarButtons.ShowNewValues));
			});
		}
		void ChangeFilterType(bool showNewValues) {
			PivotFilterType newFilterType = showNewValues ? PivotFilterType.Excluded : PivotFilterType.Included;
			if(Field.FilterValues.FilterType == newFilterType) return;
			Field.FilterValues.FilterType = newFilterType;
		}
	}
}
