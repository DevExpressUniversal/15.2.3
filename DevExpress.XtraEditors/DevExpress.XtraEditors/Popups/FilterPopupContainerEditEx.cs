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

namespace DevExpress.XtraPivotGrid {
	using System;
	[Flags]
	public enum FilterPopupToolbarButtons {
		None = 0x00,
		ShowOnlyAvailableItems = 0x01,
		ShowNewValues = 0x02,
		IncrementalSearch = 0x04,
		MultiSelection = 0x08,
		RadioMode = 0x10,
		InvertFilter = 0x20,
		All = ShowOnlyAvailableItems | ShowNewValues | IncrementalSearch | MultiSelection | RadioMode | InvertFilter,
	}
}
namespace DevExpress.XtraEditors.Filtering {
	using DevExpress.Skins;
	using DevExpress.Utils.Drawing;
	using DevExpress.Utils.Menu;
	using DevExpress.XtraEditors;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.Popup;
	using DevExpress.XtraEditors.Repository;
	using DevExpress.XtraPivotGrid;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;
	using DevExpress.Utils.Controls;
	using System.Collections;
	using DevExpress.XtraEditors.ViewInfo;
	using System.ComponentModel;
	using DevExpress.XtraReports.Native;
	using DevExpress.Utils;
	using DevExpress.LookAndFeel;
	public abstract class DropDownFilterEditBase {
		BlobBaseEdit containerEdit;
		Control parentControl;
		public DropDownFilterEditBase(Control parentControl, Rectangle bounds) {
			this.Bounds = bounds;
			this.parentControl = parentControl;
		}
		public Rectangle Bounds { get; private set; }
		public Control ParentControl { 
			get { return parentControl; } 
			protected set { parentControl = value; } 
		}
		public void Show() {
			if (CanShowPopup)
				ContainerEdit.ShowPopup();
		}
		protected virtual bool CanShowPopup {
			get { return ParentControl != null; }
		}
		public bool HasContainerEdit { get { return containerEdit != null; } }
		protected internal BlobBaseEdit ContainerEdit {
			get {
				if (!HasContainerEdit)
					SetupContainerEdit();
				return containerEdit;
			}
		}
		protected virtual void SetupContainerEdit() {
			this.containerEdit = CreateContainerEdit();
			ContainerEdit.Text = string.Empty;
			ContainerEdit.Properties.AutoHeight = false;
			ContainerEdit.Properties.Appearance.BackColor = Color.Transparent;
			ContainerEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			ContainerEdit.Properties.Buttons.Clear();
			ContainerEdit.Closed += OnClosed;
			ContainerEdit.Bounds = Bounds;
			ContainerEdit.Parent = ParentControl;
		}
		protected abstract BlobBaseEdit CreateContainerEdit();
		protected virtual void OnClosed(object sender, ClosedEventArgs e) {
			if (HasContainerEdit) {
				ContainerEdit.Dispose();
				this.containerEdit = null;
			}
		}
		public void Close() {
			if (HasContainerEdit && ContainerEdit.IsPopupOpen)
				ContainerEdit.ClosePopup();
		}
	}
	public class FilterPopupContainerEditEx : FilterPopupContainerEdit {
		public FilterPopupContainerEditEx(IFilterItems filterItems)
			: base(filterItems) { }
		public FilterPopupContainerEditEx() : base(null) {
		}
		public new IFilterItems FilterItems { get { return (IFilterItems)base.FilterItems; } set { base.FilterItems = value; } }
		protected override PopupBaseForm CreatePopupForm() {
			 return new FilterPopupContainerFormBase(this);
		}
		protected internal override void ClosePopup(PopupCloseMode closeMode) {
			if(closeMode == XtraEditors.PopupCloseMode.Normal && PopupForm != null) {
				SelectionCheckedListBoxControl lb = ((FilterPopupContainerFormBase)PopupForm).CheckListBox as SelectionCheckedListBoxControl;
				if(lb != null)
					lb.SetCheckedElement();
			}
			base.ClosePopup(closeMode);
		}
	}
	public class FilterPopupContainerFormBase : FilterPopupContainerForm {
		PivotFilterPopupToolbar toolbar;
		public FilterPopupContainerFormBase(FilterPopupContainerEditEx ownerEdit)
			: base(ownerEdit, ownerEdit.FilterItems) {
			CreateSeparatorLine();
			if(ShowToolbar) {
				this.toolbar = CreateFilterPopupToolbar();
				InitializeToolbarButtons();
			}
		}
		protected virtual bool ShowToolbar {
			get { return true; }
		}
		protected virtual bool AllowMultiSelectOption {
			get;
			set;
		}
		protected virtual bool AllowIncrementalSearchOption {
			get;
			set;
		}
		protected virtual bool IsRadioModeOption { 
			get; 
			set; 
		}
		protected virtual FilterPopupToolbarButtons ToolbarButtons {
			get { return FilterPopupToolbarButtons.MultiSelection | FilterPopupToolbarButtons.RadioMode | FilterPopupToolbarButtons.IncrementalSearch; }
		}
		protected virtual void OnCheckListBoxMouseClick(object sender, MouseEventArgs e) {
			if(!IsRadioMode || e.Button != MouseButtons.Left) return;
			OwnerEdit.ClosePopup();
		}
		protected PivotFilterPopupToolbar Toolbar { get { return toolbar; } }
		protected new FilterPopupContainerEditEx OwnerEdit { get { return (FilterPopupContainerEditEx)base.OwnerEdit; } }
		protected virtual bool IsRadioMode { get { return IsRadioModeOption; } }
		protected CheckedListBoxItem ShowAllItem { get { return CheckListBox.Items[0]; } }
		protected override string GetShowAllItemCaption() { return "(Show All)"; }
		PivotFilterPopupToolbar CreateFilterPopupToolbar() {
			PivotFilterPopupToolbar toolbar = new PivotFilterPopupToolbar(this);
			toolbar.BorderColor = new SkinElementInfo(CommonSkins.GetSkin(CheckListBox.LookAndFeel)[CommonSkins.SkinTextBorder]).Element.Border.Top;
			return toolbar;
		}
		protected virtual void InitializeToolbarButtons() {
			InitializeToolbarButtonsCore(ToolbarButtons);
		}
		protected virtual void InitializeToolbarButtonsCore(FilterPopupToolbarButtons buttons) {
			Toolbar.InitializeToolbarButtons(buttons);
			PivotToolbarCheckButton btnChangeIncrementalSearch = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.IncrementalSearch];
			btnChangeIncrementalSearch.IsChecked = AllowIncrementalSearchOption;
			btnChangeIncrementalSearch.CheckedChanged += btnChangeIncrementalSearch_CheckedChanged;
			PivotToolbarCheckButton btnChangeSelectionMode = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.MultiSelection];
			btnChangeSelectionMode.IsChecked = AllowMultiSelectOption;
			btnChangeSelectionMode.CheckedChanged += btnChangeSelectionMode_CheckedChanged;
			PivotToolbarCheckButton btnRadioMode = (PivotToolbarCheckButton)Toolbar[FilterPopupToolbarButtons.RadioMode];
			btnRadioMode.IsChecked = IsRadioModeOption;
			btnRadioMode.CheckedChanged += btnRadioModeChanged_CheckedChanged;
		}
		#region Toolbar button event handlers
		void btnRadioModeChanged_CheckedChanged(object sender, CheckedChangedEventArgs e) {
			SetRadioModeOptions(e.IsChecked);
			FillFilterItems();
			CheckListBox.Focus();
		}
		protected virtual void SetRadioModeOptions(bool isChecked) {
			IsRadioModeOption = isChecked;
			Toolbar[FilterPopupToolbarButtons.InvertFilter].Enabled = !IsRadioModeOption;
			UnsubscribeListEvents();
			RecreateCheckedListBox();
		}
		protected virtual void FillFilterItems() {
			FillList();
		}
		void btnChangeIncrementalSearch_CheckedChanged(object sender, CheckedChangedEventArgs e) {
			CheckListBox.IncrementalSearch = AllowIncrementalSearchOption = e.IsChecked;
		}
		void btnChangeSelectionMode_CheckedChanged(object sender, CheckedChangedEventArgs e) {
			AllowMultiSelectOption = e.IsChecked;
			SetCheckListBoxSelectionMode(CheckListBox);
		}
		#endregion
		void RecreateCheckedListBox() {
			Controls.Remove(CheckListBox);
			CheckListBox.Dispose();
			CheckListBox = CreateCheckListBox();
			Controls.Add(CheckListBox);
			SubscribeListEvents();
			UpdateControlPositions();
		}
		void SetCheckListBoxSelectionMode(CheckedListBoxControl checkedListBox) {
			checkedListBox.SelectionMode = AllowMultiSelectOption && !IsRadioMode ? SelectionMode.MultiExtended : SelectionMode.One;
		}
		protected override void InitializeCheckListBox(CheckedListBoxControl checkedListBox) {
			base.InitializeCheckListBox(checkedListBox);
			checkedListBox.BorderStyle = BorderStyles.NoBorder;
			checkedListBox.IncrementalSearch = AllowIncrementalSearchOption;
			SetCheckListBoxSelectionMode(checkedListBox);
		}
		protected override void FillList() {
			CheckListBox.BeginUpdate();
			CheckListBox.Items.Clear();
			CheckListBox.Items.BeginUpdate();
			try {
				AddShowAllItem();
				AddItems();
				if(!IsRadioMode)
					UnselectIfNoCheckedItems();
				else
					ChangeSelectionForRadioMode();
			} finally {
				CheckListBox.Items.EndUpdate();
				CheckListBox.EndUpdate();
			}
		}
		protected override void AddItems() {
			CheckListBox.BeginUpdate();
			try {
				foreach(object item in VisibleItems) {
					CheckListBox.Items.Add(item, ((IFilterItem)item).IsChecked);
				}
			} finally {
				CheckListBox.EndUpdate();
			}
		}
		protected virtual IEnumerable VisibleItems {
			get { return FilterItems; }
		}
		protected override void CheckAllItems(CheckState state) {
			if(state == CheckState.Indeterminate) return;
			CheckItems(state);
			UnsubscribeListEvents();
			try {
				if(state == CheckState.Checked)
					CheckListBox.CheckAll();
				else
					CheckListBox.UnCheckAll();
			} finally {
				SubscribeListEvents();
			}
		}
		protected virtual void CheckItems(CheckState state) {
			FilterItems.CheckAllItems(state == CheckState.Checked);
		}
		protected virtual void UnselectIfNoCheckedItems() {
			if(CheckListBox.CheckedItemsCount == 0)
				CheckListBox.SelectedIndex = -1;
		}
		void ChangeSelectionForRadioMode() {
			if(CheckListBox.CheckedItemsCount == CheckListBox.ItemCount)
				CheckListBox.SelectedIndex = 0;
			else
				CheckListBox.SelectedIndex = CheckListBox.CheckedItemsCount == 1 ? CheckListBox.CheckedIndices[0] : -1;
		}
		protected override CheckedListBoxControl CreateCheckListBox() {
			CheckedListBoxControl control;
			if(IsRadioMode)
				control = new SelectionCheckedListBoxControl();
			else
				control = new FilterCheckedListBoxControl();
			InitializeCheckListBox(control);
			return control;
		}
		protected override void SubscribeListEvents() {
			base.SubscribeListEvents();
			CheckListBox.MouseClick += OnCheckListBoxMouseClick;
		}
		protected override void UnsubscribeListEvents() {
			base.UnsubscribeListEvents();
			CheckListBox.MouseClick -= OnCheckListBoxMouseClick;
		}
		protected virtual void CollapseAllItems() { }
		protected virtual void ExpandAllItems() { }
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			if(ShowToolbar && Toolbar != null) {
				Toolbar.Width = SeparatorControl.Width;
				Toolbar.Top = EmbeddedControl.Top;
				EmbeddedControl.Top += Toolbar.Height;
				EmbeddedControl.Height -= Toolbar.Height;
			}
		}
	}
	[ToolboxItem(false)]
	public class SelectionCheckedListBoxControl : CheckedListBoxControl {
		#region inner classes
		class SelectionCheckedListBoxViewInfo : CheckedListBoxViewInfo {
			public SelectionCheckedListBoxViewInfo(SelectionCheckedListBoxControl selectionCheckedListBox)
				: base(selectionCheckedListBox) {
			}
			protected new SelectionCheckedListBoxControl OwnerControl { get { return (SelectionCheckedListBoxControl)base.OwnerControl; } }
			protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
				return new CheckedItemInfo(OwnerControl, bounds, item, text, index, OwnerControl.GetItemCheckState(index), true);
			}
		}
		#endregion 
		public SelectionCheckedListBoxControl()
			: base() {
			SelectionMode = SelectionMode.One;
			this.Click += OnClick;
		}
#if DEBUGTEST
		public 
#endif
		void OnClick(object sender, EventArgs e) {
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}
#if DEBUGTEST
		public
#else
		internal
#endif
		 void SetCheckedElement() {
			if (SelectedIndex == -1) return;
			UnCheckAll(-1);
			Items[SelectedIndex].CheckState = CheckState.Checked;
		}
		protected override void SetItemCore(object item) {
			SelectedIndex = Items.IndexOf(item);
		}
		protected int CheckedIndex {
			get {
				if (CheckedItemsCount != 1) return -1;
				for (int i = 0; i < Items.Count; ++i) {
					if (Items[i].CheckState == CheckState.Checked)
						return i;
				}
				return -1;
			}
		}
		protected void UnCheckAll(int ignoreIndex) {
			for (int i = 0; i < Items.Count; ++i) {
				if (i == ignoreIndex) continue;
				if (Items[i].CheckState != CheckState.Unchecked) {
					SetItemCheckStateCore(i, CheckState.Unchecked);
					if (i != 0)
						((IFilterItem)Items[i].Value).IsChecked = false;
				}
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new SelectionCheckedListBoxViewInfo(this);
		}
		public override void InvertCheckState() {
			throw new Exception("Check state inverting is not supported.");
		}
	}
	public static class FilterPopupToolbarButtonExtensions {
		public static bool Include(this FilterPopupToolbarButtons buttons, FilterPopupToolbarButtons button) {
			return (buttons & button) == button;
		}
		public static string ToButtonName(this FilterPopupToolbarButtons button) {
			if(button == FilterPopupToolbarButtons.ShowOnlyAvailableItems) return "btnShowOnlyAvailableItems";
			if(button == FilterPopupToolbarButtons.ShowNewValues) return "btnShowNewValues";
			if(button == FilterPopupToolbarButtons.IncrementalSearch) return "btnIncrementalSearch";
			if(button == FilterPopupToolbarButtons.MultiSelection) return "btnMultiSelection";
			if(button == FilterPopupToolbarButtons.RadioMode) return "btnRadioMode";
			if(button == FilterPopupToolbarButtons.InvertFilter) return "btnInvertFilter";
			throw new ArgumentException("The button parameter must represent only one FilterPopupToolbarButtons value");
		}
		public static int ToImageIndex(this FilterPopupToolbarButtons button) {
			if(button == FilterPopupToolbarButtons.ShowOnlyAvailableItems) return 0;
			if(button == FilterPopupToolbarButtons.ShowNewValues) return 1;
			if(button == FilterPopupToolbarButtons.IncrementalSearch) return 2;
			if(button == FilterPopupToolbarButtons.MultiSelection) return 3;
			if(button == FilterPopupToolbarButtons.RadioMode) return 4;
			if(button == FilterPopupToolbarButtons.InvertFilter) return 5;
			throw new ArgumentException("The button parameter must represent only one FilterPopupToolbarButtons value");
		}
	}
	public class PivotToolbarButton : LayoutItemButton { }
	public class PivotToolbarCheckButton : PivotToolbarButton {
		static readonly ObjectState? PressedState = (ObjectState?)(ObjectState.Pressed | ObjectState.Hot);
		#region internal classes
		class PivotToolbarButtonViewInfo : SimpleButtonViewInfo {
			public PivotToolbarButtonViewInfo(SimpleButton owner)
				: base(owner) {
				OverrideState = null;
			}
			public ObjectState? OverrideState { get; set; }
			public override ObjectState State {
				get { return OverrideState ?? base.State; }
				set { base.State = value; }
			}
		}
		#endregion
		public event CheckedChangedEventHandler CheckedChanged;
		bool isChecked;
		public bool IsChecked {
			get { return isChecked; }
			set {
				if(isChecked == value) return;
				isChecked = value;
				((PivotToolbarButtonViewInfo)ViewInfo).OverrideState = isChecked ? PressedState : null;
				OnCheckedChanged();
			}
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new PivotToolbarButtonViewInfo(this);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			IsChecked = !IsChecked;
		}
		void OnCheckedChanged() {
			if(CheckedChanged != null)
				CheckedChanged(this, new CheckedChangedEventArgs(IsChecked));
		}
	}
	public class CheckedChangedEventArgs : EventArgs {
		public CheckedChangedEventArgs(bool isChecked) {
			IsChecked = isChecked;
		}
		public bool IsChecked { get; set; }
	}
	public delegate void CheckedChangedEventHandler(object sender, CheckedChangedEventArgs e);
	[ToolboxItem(false)]
	public class PivotFilterPopupToolbarPanel : PanelControl {
		PopupSeparatorLine toolbarBottomBorder;
		public PivotFilterPopupToolbarPanel() {
			toolbarBottomBorder = new PopupSeparatorLine(LookAndFeel);
			toolbarBottomBorder.Dock = DockStyle.Bottom;
			Controls.Add(toolbarBottomBorder);
		}
		public Color BorderColor { get { return toolbarBottomBorder.Color; } set { toolbarBottomBorder.Color = value; } }
		Control LastControl { get; set; }
		int LastControlRight { get { return LastControl.Right; } }
		int LastControlRightMargin { get { return LastControl.Margin.Right; } }
		public void AddControl(Control control) {
			if(control.Visible) {
				control.Top = control.Margin.Top;
				control.Left = LastControl == null ? 0 : LastControl.Right;
				control.Left += Math.Max(control.Margin.Left, LastControl == null ? 0 : LastControlRightMargin);
				Height = Math.Max(Height, control.Height + control.Margin.Top + control.Margin.Bottom + 1);
				LastControl = control;
			}
			Controls.Add(control);
		}
	}
	public class PivotFilterPopupToolbar {
		const string ImagesResourceName = "DevExpress.XtraEditors.Images.FilterPopupToolbarButtonImages.png";
		static readonly Size ButtonSize = new Size(20, 20);
		PivotFilterPopupToolbarPanel toolbar = new PivotFilterPopupToolbarPanel();
		ImageCollection images;
		public PivotFilterPopupToolbar(FilterPopupContainerFormBase filterPopup) {
			InitializeToolbar(filterPopup);
		}
		public int Left { get { return Toolbar.Left; } }
		public int Top { get { return Toolbar.Top; } set { Toolbar.Top = value; } }
		public int Width { get { return Toolbar.Width; } set { Toolbar.Width = value; } }
		public int Height { get { return Toolbar.Height; } }
		public Color BorderColor { get { return toolbar.BorderColor; } set { toolbar.BorderColor = value; } }
		protected PivotFilterPopupToolbarPanel Toolbar { get { return toolbar; } }
		Images Images { get { return images.Images; } }
		public PivotToolbarButton this[FilterPopupToolbarButtons button] { get { return (PivotToolbarButton)Toolbar.Controls[button.ToButtonName()]; } }
		protected void InitializeToolbar(Control form) {
			this.images = ImageHelper.CreateImageCollectionFromResources(ImagesResourceName, System.Reflection.Assembly.GetExecutingAssembly(), new Size(16, 16));
			toolbar.Bounds = new Rectangle(1, 1, 0, 0);
			toolbar.BorderStyle = BorderStyles.NoBorder;
			form.Controls.Add(toolbar);
		}
		public void InitializeToolbarButtons(FilterPopupToolbarButtons buttons) {
			AddToolbarButton(FilterPopupToolbarButtons.ShowOnlyAvailableItems, buttons.Include(FilterPopupToolbarButtons.ShowOnlyAvailableItems),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarShowOnlyAvailableItems), true);
			AddToolbarButton(FilterPopupToolbarButtons.ShowNewValues, buttons.Include(FilterPopupToolbarButtons.ShowNewValues),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarShowNewValues), true);
			AddToolbarButton(FilterPopupToolbarButtons.IncrementalSearch, buttons.Include(FilterPopupToolbarButtons.IncrementalSearch),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarIncrementalSearch), true);
			AddToolbarButton(FilterPopupToolbarButtons.MultiSelection, buttons.Include(FilterPopupToolbarButtons.MultiSelection),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarMultiSelection), true);
			AddToolbarButton(FilterPopupToolbarButtons.RadioMode, buttons.Include(FilterPopupToolbarButtons.RadioMode),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarRadioMode), true);
			AddToolbarButton(FilterPopupToolbarButtons.InvertFilter, buttons.Include(FilterPopupToolbarButtons.InvertFilter),
				Localizer.Active.GetLocalizedString(StringId.FilterPopupToolbarInvertFilter), false);
		}
		PivotToolbarButton AddToolbarButton(FilterPopupToolbarButtons button, bool isVisible, string tooltip, bool isCheckButton) {
			PivotToolbarButton btn = isCheckButton ? new PivotToolbarCheckButton() : new PivotToolbarButton();
			btn.Name = button.ToButtonName();
			btn.Image = Images[button.ToImageIndex()];
			btn.ToolTip = tooltip;
			btn.Size = ButtonSize;
			btn.Margin = new Padding(2);
			btn.Visible = isVisible;
			Toolbar.AddControl(btn);
			return btn;
		}
		public bool IsChecked(FilterPopupToolbarButtons button) {
			return ((PivotToolbarCheckButton)this[button]).IsChecked;
		}
	}
}
