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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Text;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemComboBox : RepositoryItemPopupBaseAutoSearchEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemComboBox Properties { get { return this; } }
		private static readonly object selectedIndexChanged = new object();
		private static readonly object selectedValueChanged = new object();
		private static readonly object dropDownCustomDrawItem = new object();
		private static readonly object measureItem = new object();
		private static readonly object customItemDisplayText = new object();
		protected bool fAutoComplete, fCaseSensitiveSearch, fUseCtrlScroll, fCycleOnDblClick;
		bool hotTrackItems;
		HighlightStyle highlightedItemStyle;
		int _dropDownRows, _dropDownItemHeight;
		DefaultBoolean showTooltipForTrimmedText;
		bool sorted;
		ComboBoxItemCollection fItems;
		public RepositoryItemComboBox() {
			this.sorted = false;
			this.fItems = CreateItemCollection();
			this.fItems.CollectionChanged += new CollectionChangeEventHandler(OnItems_CollectionChanged);
			this.hotTrackItems = true;
			this.showTooltipForTrimmedText = DefaultBoolean.Default;
			this.highlightedItemStyle = HighlightStyle.Default;
			this.fAutoComplete = true;
			this.fCaseSensitiveSearch = false;
			this.fUseCtrlScroll = true;
			this.fCycleOnDblClick = true;
			this._dropDownRows = 7;
			this._dropDownItemHeight = 0;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemComboBox source = item as RepositoryItemComboBox;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.sorted = source.Sorted;
				this.hotTrackItems = source.HotTrackItems;
				this.highlightedItemStyle = source.highlightedItemStyle;
				this.showTooltipForTrimmedText = source.ShowToolTipForTrimmedText;
				this._dropDownItemHeight = source.DropDownItemHeight;
				this.fAutoComplete = source.AutoComplete;
				this.fCaseSensitiveSearch = source.CaseSensitiveSearch;
				this.fCycleOnDblClick = source.CycleOnDblClick;
				this._dropDownRows = source.DropDownRows;
				this.fUseCtrlScroll = source.UseCtrlScroll;
				this.Items.Assign(source.Items);
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(selectedIndexChanged, source.Events[selectedIndexChanged]);
			Events.AddHandler(selectedValueChanged, source.Events[selectedValueChanged]);
			Events.AddHandler(dropDownCustomDrawItem, source.Events[dropDownCustomDrawItem]);
			Events.AddHandler(measureItem, source.Events[measureItem]);
			Events.AddHandler(customItemDisplayText, source.Events[customItemDisplayText]);
		}
		protected virtual ComboBoxItemCollection CreateItemCollection() {
			return new ComboBoxItemCollection(this);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "ComboBoxEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxCycleOnDblClick"),
#endif
 DefaultValue(true)]
		public bool CycleOnDblClick {
			get { return fCycleOnDblClick; }
			set {
				if(CycleOnDblClick == value) return;
				fCycleOnDblClick = value;
				OnPropertiesChanged();
			}
		}
		protected virtual bool ShouldSerializeDropDownRows() {
			return DropDownRows != 7;
		}
		protected virtual void ResetDropDownRows() {
			DropDownRows = 7;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxDropDownRows"),
#endif
 SmartTagProperty("DropDown Rows", "")]
		public int DropDownRows {
			get { return _dropDownRows; }
			set {
				if(value < 1) value = 1;
				if(DropDownRows == value) return;
				_dropDownRows = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxDropDownItemHeight"),
#endif
 DefaultValue(0)]
		public int DropDownItemHeight {
			get { return _dropDownItemHeight; }
			set {
				if(value < 1) value = 0;
				if(DropDownItemHeight == value) return;
				_dropDownItemHeight = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxAutoComplete"),
#endif
 DefaultValue(true), SmartTagProperty("Auto Complete", "", 0)]
		public virtual bool AutoComplete {
			get { return fAutoComplete; }
			set {
				if(AutoComplete == value) return;
				fAutoComplete = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteHotTrackItems)]
		public bool HotTrackDropDownItems {
			get { return HotTrackItems; }
			set { HotTrackItems = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxHotTrackItems"),
#endif
 DefaultValue(true)]
		public virtual bool HotTrackItems {
			get { return hotTrackItems; }
			set {
				if(HotTrackItems == value) return;
				hotTrackItems = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowToolTipForTrimmedText {
			get { return showTooltipForTrimmedText; }
			set {
				if(ShowToolTipForTrimmedText == value) return;
				showTooltipForTrimmedText = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxHighlightedItemStyle"),
#endif
 DefaultValue(HighlightStyle.Default)]
		public virtual HighlightStyle HighlightedItemStyle {
			get { return highlightedItemStyle; }
			set {
				if(HighlightedItemStyle == value) return;
				highlightedItemStyle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxSorted"),
#endif
 DefaultValue(false), SmartTagProperty("Sorted", "", 1)]
		public virtual bool Sorted {
			get { return sorted; }
			set {
				if(Sorted == value) return;
				sorted = value;
				Items.Sorted = Sorted;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxCaseSensitiveSearch"),
#endif
 DefaultValue(false)]
		public virtual bool CaseSensitiveSearch {
			get { return fCaseSensitiveSearch; }
			set {
				if(CaseSensitiveSearch == value) return;
				fCaseSensitiveSearch = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeUseCtrlScroll() {
			if(OwnerEdit == null) return !UseCtrlScroll;
			return UseCtrlScroll != ((ComboBoxEdit)OwnerEdit).DefaultUseCtrlScroll;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxUseCtrlScroll")
#else
	Description("")
#endif
]
		public virtual bool UseCtrlScroll {
			get { return fUseCtrlScroll; }
			set {
				if(UseCtrlScroll == value) return;
				fUseCtrlScroll = value;
				OnPropertiesChanged();
			}
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(key == GetKeyIncludeCtrlScroll(Keys.Up)) return true;
			if(key == GetKeyIncludeCtrlScroll(Keys.Down)) return true;
			if(key == GetKeyIncludeCtrlScroll(Keys.PageDown)) return true;
			if(key == GetKeyIncludeCtrlScroll(Keys.PageUp)) return true;
			if(key == GetKeyIncludeCtrlScroll(Keys.Home) && TextEditStyle != TextEditStyles.Standard) return true;
			if(key == GetKeyIncludeCtrlScroll(Keys.End) && TextEditStyle != TextEditStyles.Standard) return true;
			return base.NeededKeysContains(key);
		}
		protected internal virtual Keys GetKeyIncludeCtrlScroll(Keys key) {
			if(UseCtrlScroll) return key | Keys.Control;
			return key;
		}
		protected override ExportMode GetExportMode() {
			if(ExportMode == ExportMode.Default) return ExportMode.DisplayText;
			return ExportMode;
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(OwnerEdit != null) {
				editValue = OwnerEdit.RealEditValue;
			}
			return base.GetDisplayText(format, editValue);
		}
		[Browsable(false)]
		public new ComboBoxEdit OwnerEdit { get { return base.OwnerEdit as ComboBoxEdit; } }
		protected internal bool IsMeasureItemAssigned { get { return this.Events[measureItem] != null; } }
		[Localizable(true), DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual ComboBoxItemCollection Items { get { return fItems; } }
		protected internal virtual bool ShouldSerializeItems() {
			return Items.Count > 0;
		}
		protected virtual void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnPropertiesChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDrawItem)]
		public event ListBoxDrawItemEventHandler DropDownCustomDrawItem {
			add { DrawItem += value; }
			remove { DrawItem -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxDrawItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListBoxDrawItemEventHandler DrawItem {
			add { this.Events.AddHandler(dropDownCustomDrawItem, value); }
			remove { this.Events.RemoveHandler(dropDownCustomDrawItem, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxSelectedIndexChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedIndexChanged {
			add { this.Events.AddHandler(selectedIndexChanged, value); }
			remove { this.Events.RemoveHandler(selectedIndexChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxSelectedValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedValueChanged {
			add { this.Events.AddHandler(selectedValueChanged, value); }
			remove { this.Events.RemoveHandler(selectedValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxMeasureItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event MeasureItemEventHandler MeasureItem {
			add { this.Events.AddHandler(measureItem, value); }
			remove { this.Events.RemoveHandler(measureItem, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemComboBoxCustomItemDisplayText"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomItemDisplayTextEventHandler CustomItemDisplayText {
			add { this.Events.AddHandler(customItemDisplayText, value); }
			remove { this.Events.RemoveHandler(customItemDisplayText, value); }
		}
		protected internal virtual void RaiseSelectedIndexChanged(EventArgs e) {
			if(IsLockEvents)
				return;
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseSelectedValueChanged(EventArgs e) {
			if(IsLockEvents)
				return;
			EventHandler handler = (EventHandler)this.Events[selectedValueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseDropDownCustomDrawItem(ListBoxDrawItemEventArgs e) {
			if(OwnerEdit != null) OwnerEdit.RaiseDropDownCustomDrawItem(e);
			ListBoxDrawItemEventHandler handler = (ListBoxDrawItemEventHandler)this.Events[dropDownCustomDrawItem];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseMeasureItem(MeasureItemEventArgs e) {
			if(IsLockEvents)
				return;
			MeasureItemEventHandler handler = (MeasureItemEventHandler)this.Events[measureItem];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCustomItemDisplayText(CustomItemDisplayTextEventArgs e) {
			if(IsLockEvents)
				return;
			CustomItemDisplayTextEventHandler handler = (CustomItemDisplayTextEventHandler)this.Events[customItemDisplayText];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual bool UpdateEditValueFromPopup { get { return true; } }
		protected override bool NeededKeysPopupContains(Keys key) {
			if(IsListBoxKey(key))
				return true;
			return base.NeededKeysPopupContains(key);
		}
		protected override bool IsNeededKeyCore(Keys keyData) {
			bool shouldProcessUpDown = (OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Bars) && (!UseCtrlScroll || Control.ModifierKeys == Keys.Control);
			if(keyData == Keys.Up || keyData == Keys.Down)
				return shouldProcessUpDown;
			return base.IsNeededKeyCore(keyData);
		}
		protected internal virtual bool IsListBoxKey(Keys key) {
			switch(key) {
				case Keys.Up:
				case Keys.Down:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.Enter:
				case Keys.PageDown | Keys.Control:
				case Keys.PageUp | Keys.Control:
					return true;
				case Keys.Home:
				case Keys.End:
					if (TextEditStyle == TextEditStyles.Standard) return false;
					return true;
				default:
					return false;
			}
		}
		[Browsable(true)]
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get { return base.ContextButtonOptions; }
		}
		[Browsable(true)]
		public override ContextItemCollection ContextButtons {
			get { return base.ContextButtons; }
		}
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false), Obsolete(ObsoleteText.SRObsoleteComboBox)]
	public class ComboBox : ComboBoxEdit { }
	[DefaultEvent("SelectedIndexChanged"),
	 Designer("DevExpress.XtraEditors.Design.ComboBoxEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays a drop-down list of text items."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(ComboBoxEditActions), "Items", "Edit multi-line text", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ComboBoxEdit")
	]
	public class ComboBoxEdit : PopupBaseAutoSearchEdit {
		[Browsable(false)]
		public override string EditorTypeName { get { return "ComboBoxEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemComboBox Properties { get { return base.Properties as RepositoryItemComboBox; } }
		protected override void InitializeDefaultProperties() {
			base.InitializeDefaultProperties();
			Properties.UseCtrlScroll = DefaultUseCtrlScroll;
		}
		protected internal virtual bool DefaultUseCtrlScroll { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object SelectedItem {
			get { return EditValue; }
			set {
				if(value != null) {
					int index = Properties.Items.IndexOf(value);
					if(index != -1) EditValue = Properties.Items[index];
				}
				else 
					EditValue = null;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditSelectedIndex"),
#endif
 RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int SelectedIndex {
			get { return Properties.Items.IndexOf(SelectedItem); }
			set {
				if(value < 0 || value >= Properties.Items.Count)
					SelectedItem = null;
				else
					SelectedItem = Properties.Items[value];
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditEditValue"),
#endif
 RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Data)]
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if(IsLoading) {
					base.EditValue = value;
					return;
				}
				int prevIndex = SelectedIndex;
				base.EditValue = value;
				CheckAutoSearchText();
				UpdatePopupEditValueIndex(prevIndex);
				if(IsPopupOpen) UpdateDisplayText();
			}
		}
		public override void Reset() {
			base.Reset();
			if(PopupForm != null && PopupForm.ListBox != null)
				PopupForm.ListBox.TopIndex = 0;
		}
		protected virtual void UpdatePopupEditValueIndex(int prevIndex) {
			if(SelectedIndex != prevIndex || (IsPopupOpen && PopupForm.SelectedItemIndex != SelectedIndex)) {
				if(IsPopupOpen) {
					PopupForm.SelectedItemIndex = SelectedIndex;
				}
				Properties.RaiseSelectedIndexChanged(EventArgs.Empty);
				Properties.RaiseSelectedValueChanged(EventArgs.Empty);
			}
		}
		protected internal new PopupListBoxForm PopupForm { get { return base.PopupForm as PopupListBoxForm; } }
		protected override bool CanShowPopup { get { return base.CanShowPopup && Properties.Items.Count > 0; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new ComboBoxPopupListBoxForm(this);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseDown(ee);
				return;
			}
			EditHitInfo hitInfo = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(e.Button == MouseButtons.Left && !IsPopupOpen && !Properties.ReadOnly) {
				if(hitInfo.HitTest == EditHitTest.MaskBox) {
					if(Properties.CycleOnDblClick && e.Clicks == 2 && Properties.ShowDropDown == ShowDropDown.Never) {
						ScrollValue(1, true);
						SelectAll();
						ee.Handled = true;
					}
				}
			}
			base.OnMouseDown(ee);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(!e.Handled && !IsPopupOpen) {
				int delta = 0;
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.Up)) delta = -1;
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.Down)) delta = 1;
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.PageDown)) delta = Properties.DropDownRows;
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.PageUp)) delta = -Properties.DropDownRows;
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.Home) && Properties.TextEditStyle != TextEditStyles.Standard) ScrollIndex(0, e);
				if(e.KeyData == GetKeyIncludeCtrlScroll(Keys.End) && Properties.TextEditStyle != TextEditStyles.Standard) ScrollIndex(Properties.Items.Count - 1, e);
				if(delta != 0) {
					e.Handled = true;
					ScrollValue(delta, false);
				}
			}
		}
		Keys GetKeyIncludeCtrlScroll(Keys key) {
			return Properties.GetKeyIncludeCtrlScroll(key);
		}
		protected override void CreateMaskBox() {
			base.CreateMaskBox();
			MaskBox.SelectAllOnEnter = true;
		}
		protected virtual string GetItemDescription(object item) {
			return Properties.Items.GetItemDescription(item);
		}
		protected internal virtual void OnPopupSelectedIndexChanged() {
			bool selectAll = false;
			if(IsMaskBoxAvailable) {
				if(SelectionLength == 0) selectAll = true;
			}
			LayoutChanged();
			if(selectAll) SelectAll();
		}
		protected internal virtual object RealEditValue {
			get {
				if(IsPopupOpen && Properties.UpdateEditValueFromPopup) return PopupForm.ResultValue;
				return EditValue;
			}
		}
		protected override bool AllowAutoSearchSelectionLength { get { return Properties.AutoComplete; } }
		protected override void ProcessAutoSearchNavKey(KeyEventArgs e) {
			if(!Properties.AutoComplete) return;
			base.ProcessAutoSearchNavKey(e);
		}
		protected override int FindItem(string text, int startIndex) {
			return FindItem(text, Properties.AutoComplete, startIndex);
		}
		protected virtual int FindItem(string text, bool autoComplete, int startIndex) {
			if(text == null) return -1;
			startIndex = Math.Max(startIndex, 0);
			if(text.Length == 0) {
				for(int i = startIndex; i < Properties.Items.Count; ++i) {
					if(string.Empty == GetItemDescription(Properties.Items[i])) return i;
				}
			} else {
				if(!Properties.CaseSensitiveSearch) text = text.ToLower();
				for(int i = startIndex; i < Properties.Items.Count; i++) {
					string itemText = GetItemDescription(Properties.Items[i]);
					if(!Properties.CaseSensitiveSearch) itemText = itemText.ToLower();
					if(autoComplete)
						itemText = itemText.Substring(0, Math.Min(itemText.Length, text.Length));
					if(text == itemText) return i;
				}
			}
			return -1;
		}
		protected override void FindUpdatePopupSelectedItem(int itemIndex) {
			if(itemIndex == -1) {
				if(Properties.TextEditStyle != TextEditStyles.Standard) {
					if(AutoSearchText != "") return;
					itemIndex = FindCurrentText();
				}
			}
			PopupForm.SelectedItemIndex = itemIndex;
		}
		protected override void FindUpdateEditValue(int itemIndex, bool jopened) {
			if(!IsPopupOpen)
				EditValue = Properties.Items[itemIndex];
		}
		protected override void OnPopupFormValueChanged() {
			EditValue = PopupForm.ResultValue;
			SelectAll();
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			if(AllowSelectAllOnPopupClose) SelectAll();
			base.OnPopupClosed(closeMode);
		}
		protected virtual bool AllowSelectAllOnPopupClose { get { return true; } }
		protected internal override bool FireSpinRequest(DXMouseEventArgs e, bool isUp) {
			if(e != null && IsPopupOpen) {
				OnMouseWheel(e);
				return true;
			}
			return base.FireSpinRequest(e, isUp);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled || Properties.ReadOnly || !Properties.AllowMouseWheel) return;
				ee.Handled = true;
				int step = SystemInformation.MouseWheelScrollLines > 0 && IsPopupOpen ? SystemInformation.MouseWheelScrollLines : 1;
				if(e.Delta > 0) step *= -1;
				if(IsPopupOpen)
					PopupForm.ListBox.TopIndex += step;
				else {
					bool allowScroll = Properties.TextEditStyle == TextEditStyles.Standard ||
						(Properties.TextEditStyle != TextEditStyles.Standard && !DoSpin(step < 0));
					if(allowScroll)
						ScrollValue(step, false);
				}
			} finally {
				ee.Sync();
			}
		}
		protected virtual int GetNewScrollIndex(int delta) {
			return SelectedIndex + delta;
		}
		protected virtual void ScrollIndex(int index, KeyEventArgs e) {
			if(Properties.Items.Count == 0 || Properties.ReadOnly) return;
			if(index < 0) index = 0;
			if(index >= Properties.Items.Count) index = Properties.Items.Count - 1;
			SelectedIndex = index;
			SelectAll();
			if(e != null)
				e.Handled = true;
		}
		protected virtual void ScrollValue(int delta, bool cycle) {
			if(Properties.Items.Count == 0 || Properties.ReadOnly) return;
			int newIndex = GetNewScrollIndex(delta);
			if(newIndex >= Properties.Items.Count) {
				newIndex = cycle ? 0 : Properties.Items.Count - 1;
			}
			if(newIndex < 0) {
				newIndex = cycle ? Properties.Items.Count - 1 : 0;
			}
			SelectedIndex = newIndex;
			SelectAll();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditSelectedIndexChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedIndexChanged {
			add { Properties.SelectedIndexChanged += value; }
			remove { Properties.SelectedIndexChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditSelectedValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedValueChanged {
			add { Properties.SelectedValueChanged += value; }
			remove { Properties.SelectedValueChanged -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDrawItem)]
		public event ListBoxDrawItemEventHandler DropDownCustomDrawItem {
			add { DrawItem += value; }
			remove { DrawItem -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxEditDrawItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListBoxDrawItemEventHandler DrawItem {
			add { Properties.DrawItem += value; }
			remove { Properties.DrawItem -= value; }
		}
		protected void CheckHighlightPopupAutoSearchText(ListBoxDrawItemEventArgs e) {
			BaseListBoxViewInfo.ItemInfo vi = e.GetItemInfo() as BaseListBoxViewInfo.ItemInfo;
			if(vi == null) return;
			vi.HighlightCharsCount = 0;
			if(!string.IsNullOrEmpty(AutoSearchText)) {
				string lower = vi.Text.ToLower();
				if(lower.StartsWith(AutoSearchText) && (vi.State & DrawItemState.Selected) != 0) vi.HighlightCharsCount = AutoSearchText.Length;
			}
		}
		protected internal virtual void RaiseDropDownCustomDrawItem(ListBoxDrawItemEventArgs e) { }
		protected internal virtual void OnActionItemClick(ListItemActionInfo action) {
		}
		protected internal virtual bool HasItemActions { get { return false; } }
		protected internal virtual void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) { }
		protected virtual void SilentRemoveCore(object item) {
			Properties.Items.Remove(item);
		}
		protected bool SilentRemove(object item) {
			if(!Properties.Items.Contains(item)) return false;
			Properties.BeginUpdate();
			try {
				SilentRemoveCore(item);
			}
			finally {
				Properties.CancelUpdate();
			}
			return true;
		}
		protected internal override void RefreshPopup() {
			base.RefreshPopup();
			if(IsPopupOpen && PopupForm != null) PopupForm.ListBox.LayoutChanged();
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ComboBoxViewInfo : PopupBaseAutoSearchEditViewInfo {
		public ComboBoxViewInfo(RepositoryItem item) : base(item) { }
		public new ComboBoxEdit OwnerEdit { get { return base.OwnerEdit as ComboBoxEdit; } }
		public override void UpdateEditValue() {
			if(OwnerEdit != null && OwnerEdit.IsPopupOpen && Item.UpdateEditValueFromPopup) {
				EditValue = OwnerEdit.PopupForm.ResultValue;
				return;
			}
			base.UpdateEditValue();
		}
		public new RepositoryItemComboBox Item { get { return base.Item as RepositoryItemComboBox; } }
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class PopupListFormHelper {
		SimplePopupListBox listBox;
		public PopupListFormHelper(SimplePopupListBox listBox) {
			this.listBox = listBox;
		}
		public const int MinimumComboWidth = 10;
		public virtual int CalcFormHeight(int itemCount) {
			if(!ListBox.IsMeasureItemAssigned)
				return ListBox.ItemHeight * itemCount;
			int height = 0;
			for(int i = 0; i < itemCount; i++) {
				height += ListBox.ViewInfo.GetItemSize(i).Height;
			}
			return height;
		}
		public static int MaximumBestWidthItemCount = 3000;
		public virtual int CalcMinimumComboWidth(IList items, int dropDownRows) {
			GraphicsInfo gInfo = new GraphicsInfo();
			int itemWidth = 0;
			gInfo.AddGraphics(null);
			int recommendedWidth = ListBox.Appearance.FontHeight * 100;
			try {
				int count = items.Count;
				if(count > MaximumBestWidthItemCount) count = MaximumBestWidthItemCount;
				for(int i = 0; i < count; i++) {
					itemWidth = Math.Max(itemWidth, CalcComboItemWidth(gInfo, items[i], recommendedWidth));
				}
				if(items.Count > dropDownRows && ScrollBarBase.GetUIMode(ListBox.ScrollInfo.VScroll) != ScrollUIMode.Touch) itemWidth += SystemInformation.VerticalScrollBarWidth;
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return Math.Max(MinimumComboWidth, itemWidth);
		}
		public virtual int CalcComboItemWidth(GraphicsInfo gInfo, object item, int recommendedWidth) {
			int width = ListBox.CalcItemWidth(gInfo, item);
			return Math.Min(width, recommendedWidth);
		}
		public SimplePopupListBox ListBox { get { return listBox; } }
	}
	[ToolboxItem(false)]
	public class PopupListBoxForm : PopupBaseSizeableForm {
		PopupListBox listBox;
		int lockEvents, lastSelectedIndex;
		PopupListFormHelper popupListFormHelper;
		public PopupListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
			this.AllowSizing = OwnerEdit.Properties.PopupSizeable;
			ViewInfo.ShowSizeBar = this.AllowSizing;
			this.lastSelectedIndex = -1;
			this.lockEvents = 0;
			this.listBox = CreateListBox();
			this.popupListFormHelper = CreatePopupListFormHelper();
			this.ListBox.SelectedIndexChanged += new EventHandler(OnListBox_SelectedIndexChanged);
			if(OwnerEdit.StyleController != null)
				AppearanceHelper.Combine(ListBox.Appearance, Properties.AppearanceDropDown, OwnerEdit.StyleController.AppearanceDropDown);
			else
				this.ListBox.Appearance.Assign(Properties.AppearanceDropDown);
			this.ListBox.LookAndFeel.ParentLookAndFeel = Properties.LookAndFeel;
			this.ListBox.UpdateProperties();
			Controls.Add(ListBox);
		}
		protected virtual PopupListFormHelper CreatePopupListFormHelper() {
			return new PopupListFormHelper(ListBox);
		}
		protected PopupListFormHelper PopupListFormHelper { get { return popupListFormHelper; } }
		protected internal override void ResetResultValue() {
			this.lastSelectedIndex = -1;
		}
		protected override Size DefaultMinFormSize { get { return new Size(5, 5); } }
		[Browsable(false)]
		public new ComboBoxEdit OwnerEdit { get { return base.OwnerEdit as ComboBoxEdit; } }
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemComboBox Properties { get { return base.Properties as RepositoryItemComboBox; } }
		protected override Size CalcFormSizeCore() {
			Size blob = Size.Empty;
			object blobSize = OwnerEdit.Properties.PropertyStore["ComboPopupSize"];
			if(blobSize != null && OwnerEdit.Properties.PopupSizeable) {
				blob = (Size)blobSize;
				if(blob.Height > 0) return blob;
			}
			Size size = Size.Empty;
			int itemCount = Math.Min(ListBox.ItemCount, DropDownRows);
			if(itemCount == 0)
				itemCount = GetItemCountForEmptyList();
			if(itemCount == 0) return Size.Empty;
			size.Height = CalcFormHeight(itemCount);
			size.Width = CalcMinimumComboWidth();
			Size desiredSize = Properties.GetDesiredPopupFormSize(true);
			size.Width = CalcFormWidth(desiredSize, size);
			size = CalcFormSize(size);
			size.Height = Math.Max(desiredSize.Height, size.Height);
			if(blob.Width != 0) size.Width = blob.Width;
			return size;
		}
		protected virtual int DropDownRows { get { return Properties.DropDownRows; } }
		protected virtual int CalcFormWidth(Size desiredSize, Size minSize) {
			return Math.Max(desiredSize.Width == 0 ? OwnerEdit.Width - ViewInfo.CalcBorderSize().Width : desiredSize.Width, minSize.Width);
		}
		protected virtual int GetItemCountForEmptyList() {
			return 1;
		}
		protected virtual int CalcFormHeight(int itemCount) {
			return PopupListFormHelper.CalcFormHeight(itemCount);
		}
		protected internal virtual void OnVisibleRowCountChanged() {
		}
		public virtual int CalcMinimumComboWidth() {
			return PopupListFormHelper.CalcMinimumComboWidth(Properties.Items, DropDownRows);
		}
		[Browsable(false)]
		public virtual PopupListBox ListBox { get { return listBox; } }
		protected override Control EmbeddedControl { get { return ListBox; } }
		protected virtual PopupListBox CreateListBox() {
			return new PopupListBox(this);
		}
		protected internal int LastSelectedIndex {
			get { return lastSelectedIndex; }
			set { lastSelectedIndex = value; }
		}
		[DXCategory(CategoryName.Appearance)]
		public virtual int SelectedItemIndex {
			get { return ListBox.SelectedIndex; }
			set {
				lockEvents++;
				try {
					ListBox.SelectedIndex = value;
					if(ListBox.SelectedIndex >= 0) ListBox.MakeItemVisible(ListBox.SelectedIndex);
				}
				finally {
					lockEvents--;
				}
			}
		}
		public override void ShowPopupForm() {
			SetupListBoxOnShow();
			base.ShowPopupForm();
		}
		protected virtual void SetupListBoxOnShow() {
			ListBox.HotTrackItems = false;
			if(ListBox.TopIndex > ListBox.Model.GetMaxTopIndex()) ListBox.TopIndex = 0;
			if(AllowUpdateSelectedItem) SelectedItemIndex = OwnerEdit.SelectedIndex;
		}
		protected virtual bool AllowUpdateSelectedItem { get { return true; } }
		public override void HidePopupForm() {
			if(OwnerEdit.Properties.PopupSizeable && FormResized)
				OwnerEdit.Properties.PropertyStore["ComboPopupSize"] = Bounds.Size;
			base.HidePopupForm();
		}
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue {
			get {
				if(LastSelectedIndex < 0 || LastSelectedIndex >= ListBox.ItemCount) return OwnerEdit.EditValue;
				return ListBox.GetItem(LastSelectedIndex);
			}
		}
		protected virtual void OnListBox_SelectedIndexChanged(object sender, EventArgs e) {
			this.lastSelectedIndex = ListBox.SelectedIndex;
			if(lockEvents != 0) return;
			if(!Properties.ReadOnly)
				OwnerEdit.OnPopupSelectedIndexChanged();
		}
		public override void ProcessKeyPress(KeyPressEventArgs e) {
			base.ProcessKeyPress(e);
			if(e.Handled) return;
			if(!Properties.ReadOnly)
				OwnerEdit.ProcessChar(e);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyData == (Keys.Down | Keys.Alt)) {
				this.lastSelectedIndex = ListBox.SelectedIndex;
			}
			base.ProcessKeyDown(e);
			if(e.Handled) return;
			if(e.KeyData == Keys.Enter) {
				this.lastSelectedIndex = ListBox.SelectedIndex;
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			if(OwnerEdit.Properties.IsListBoxKey(e.KeyData)) {
				ListBox.ProcessKeyDown(e);
				e.Handled = true;
			}
		}
	}
	[ToolboxItem(false)]
	public class ComboBoxPopupListBoxForm : PopupListBoxForm {
		public ComboBoxPopupListBoxForm(ComboBoxEdit ownerEdit) : base(ownerEdit) { }
		protected override PopupListBox CreateListBox() {
			return new ComboBoxPopupListBox(this);
		}
		void ResetHotItemIndex() {
			if(ListBox == null) return;
			var singleSelectState = ListBox.Handler.ControlState as DevExpress.XtraEditors.ListBoxControlHandler.SingleSelectState;
			if(singleSelectState == null) return;
			singleSelectState.ResetFocusedIndex();
		}
		public override void ShowPopupForm() {
			ResetHotItemIndex();
			base.ShowPopupForm();
		}
	}
	[ToolboxItem(false)]
	public class ComboBoxPopupListBox : PopupListBox {
		public ComboBoxPopupListBox(ComboBoxPopupListBoxForm ownerForm) : base(ownerForm) { }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.ComboBoxListBoxAccessible(this);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			ViewInfo.FocusedItemIndex = -1;
		}
	}
	[ToolboxItem(false)]
	public abstract class SimplePopupListBox : ListBoxControl {
		bool sorted;
		SimplePopupBaseForm ownerForm;
		public SimplePopupListBox(SimplePopupBaseForm ownerForm) {
			this.ownerForm = ownerForm;
			this.sorted = false;
			SetStyle(ControlStyles.UserMouse, false);
			this.BorderStyle = BorderStyles.NoBorder;
			this.HotTrackItems = false;
			this.UseDisabledStatePainter = false;
			this.DataSource = GetDataSource();
		}
		public bool Sorted {
			get { return sorted; }
			set {
				if(Sorted == value) return;
				sorted = value;
				OnSortedChanged();
			}
		}
		protected virtual void OnSortedChanged() {
			DataAdapter.Sorted = Sorted;
		}
		public void SetFilter(string text) {
			SetFilter(text, "Column");
		}
		public virtual void SetFilter(string text, string column) {
			try {
				string filter = string.Empty;
				if(text != null && text.Length > 0) {
					filter = CreateFilterExpression(text, column);
				}
				DataAdapter.FilterExpression = filter;
			}
			catch {
				DataAdapter.FilterExpression = string.Empty;
			}
		}
		protected virtual string CreateFilterExpression(string text, string column) {
			return CriteriaOperator.ToString(new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(column), text));
		}
		protected internal override bool EnableAccessibleOnSelectedIndexChanged {
			get { return true; }
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Enter) {
				ClosePopup();
				return;
			}
			OnKeyDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			this.HotTrackItems = AllowHotTrackItems;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs de = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(de);
			if(e.Button == MouseButtons.Left) {
				if(!de.Handled) {
					SetLastSelectedIndex(SelectedIndex);
					Capture = false;
					ClosePopup();
					return;
				}
				Capture = false;
			}
		}
		public virtual int CalcItemWidth(GraphicsInfo gInfo, object item) {
			string text = GetItemText(item);
			int width = AllowHtmlDrawing ?
				StringPainter.Default.Calculate(gInfo.Graphics, PaintAppearance, text, 0).Bounds.Width + 1 :
				PaintAppearance.CalcTextSize(gInfo.Graphics, PaintAppearance.GetStringFormat(ViewInfo.DefaultTextOptions), text, 0).ToSize().Width + 1;
			width += ViewInfo.ListBoxItemPainter.GetHorzPadding(ViewInfo.ListBoxItemInfoArgs);
			return width;
		}
		protected virtual bool AllowHtmlDrawing { get { return OwnerForm.OwnerEdit.Properties.GetAllowHtmlDraw(); } }
		protected virtual string GetItemText(object item) {
			return item.ToString();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new SimplePopupListBoxViewInfo(this);
		}
		protected virtual AppearanceObject PaintAppearance { get { return ViewInfo == null ? Appearance : ViewInfo.PaintAppearance; } }
		protected override void OnVisibleRowCountChanged(object sender, EventArgs e) {
			base.OnVisibleRowCountChanged(sender, e);
			OnVisibleRowCountChanged();
		}
		protected internal virtual bool IsMeasureItemAssigned { get { return false; } }
		protected internal void UpdateProperties() {
			ViewInfo.UpdateAppearances();
			HighlightedItemStyle = GetHighlightedItemStyle();
			ShowToolTipForTrimmedText = ShouldShowToolTipForTrimmedText();
			ItemHeight = Math.Max(ViewInfo.CalcItemMinHeight(), ItemHeight);
			ViewInfo.UpdatePaintersCore();
		}
		protected virtual HighlightStyle GetHighlightedItemStyle() { return HighlightStyle.Default; }
		protected virtual DefaultBoolean ShouldShowToolTipForTrimmedText() { return DefaultBoolean.Default; }
		[Browsable(false)]
		public BaseEdit OwnerEdit { get { return OwnerForm == null ? null : OwnerForm.OwnerEdit as BaseEdit; } }
		protected virtual void OnVisibleRowCountChanged() {
		}
		protected override bool CanFocusListBox { get { return false; } }
		protected virtual bool AllowHotTrackItems { get { return true; } }
		protected virtual void ClosePopup() {
			OwnerForm.ClosePopup();
		}
		public SimplePopupBaseForm OwnerForm { get { return ownerForm; } }
		protected internal new SimplePopupListBoxViewInfo ViewInfo {
			get { return base.ViewInfo as SimplePopupListBoxViewInfo; }
		}
		protected abstract void SetLastSelectedIndex(int index);
		protected abstract object GetDataSource();
	}
	[ToolboxItem(false)]
	public class PopupListBox : SimplePopupListBox {
		public PopupListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
			this.Visible = false;
		}
		protected override object GetDataSource() {
			return Properties.Items;
		}
		protected internal override bool HasItemActions {
			get {
				return OwnerEdit != null && OwnerEdit.HasItemActions;
			}
		}
		protected internal override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
			if(OwnerEdit == null) return;
			OwnerEdit.CreateItemActions(itemInfo);
		}
		protected internal override void OnActionItemClick(ListItemActionInfo action) {
			if(OwnerEdit != null) OwnerEdit.OnActionItemClick(action);
		}
		protected override HighlightStyle GetHighlightedItemStyle() {
			return Properties.HighlightedItemStyle;
		}
		protected override DefaultBoolean ShouldShowToolTipForTrimmedText() { return Properties.ShowToolTipForTrimmedText; }
		protected override void UpdateBindingManager() {
		}
		[Browsable(false)]
		new public PopupListBoxForm OwnerForm { get { return base.OwnerForm as PopupListBoxForm; } }
		[Browsable(false)]
		new public ComboBoxEdit OwnerEdit { get { return OwnerForm == null ? null : OwnerForm.OwnerEdit as ComboBoxEdit; } }
		[DXCategory(CategoryName.Properties)]
		public RepositoryItemComboBox Properties { get { return OwnerEdit == null ? null : OwnerEdit.Properties; } }
		protected override string GetItemTextCore(int index) {
			object item = GetDisplayItemValue(index);
			return Properties.Items.GetItemDescription(item);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new PopupListBoxViewInfo(this);
		}
		internal new bool MeasureItemAssigned { get { return Properties.IsMeasureItemAssigned; } }
		protected internal new PopupListBoxViewInfo ViewInfo { get { return base.ViewInfo as PopupListBoxViewInfo; } }
		protected internal override bool IsMeasureItemAssigned { get { return MeasureItemAssigned; } }
		protected override bool AllowHtmlDrawing {
			get {
				return Properties.GetAllowHtmlDraw();
			}
		}
		protected override string GetItemText(object item) {
			return Properties.Items.GetItemDescription(item);
		}
		protected virtual bool RaiseCustomDraw(ListBoxDrawItemEventArgs e) {
			Properties.RaiseDropDownCustomDrawItem(e); 
			return e.Handled;
		}
		protected internal override void RaiseMeasureItem(MeasureItemEventArgs e) {
			Properties.RaiseMeasureItem(e);
		}
		protected override string GetDrawItemTextCore(int index) {
			CustomItemDisplayTextEventArgs e = new CustomItemDisplayTextEventArgs(this, index, GetItemTextCore(index));
			Properties.RaiseCustomItemDisplayText(e);
			return e.DisplayText;
		}
		protected internal override void RaiseDrawItem(ListBoxDrawItemEventArgs e) {
			base.RaiseDrawItem(e);
			if(e.Handled) return;
			e.Handled = RaiseCustomDraw(e);
		}
		protected override void OnVisibleRowCountChanged() {
			if(OwnerForm != null) OwnerForm.OnVisibleRowCountChanged();
		}
		protected override void SetLastSelectedIndex(int index) {
			OwnerForm.LastSelectedIndex = index;
		}
		protected override void ClosePopup() {
			OwnerEdit.ClosePopup();
		}
		protected override bool AllowHotTrackItems { get { return Properties.HotTrackItems; } }
		public override ContextItemCollection ContextButtons {
			get {
				return OwnerEdit.Properties.ContextButtons;
			}
		}
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get {
				return OwnerEdit.Properties.ContextButtonOptions;
			}
		}
		protected internal override void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			base.RaiseContextButtonClick(e);
			OwnerEdit.Properties.RaiseContextButtonClick(e);
		}
		protected internal override void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			base.RaiseContextButtonValueChanged(e);
			OwnerEdit.Properties.RaiseContextButtonValueChanged(e);
		}
		protected internal override void RaiseCustomizeContextItem(ListBoxControlContextButtonCustomizeEventArgs e) {
			base.RaiseCustomizeContextItem(e);
			OwnerEdit.Properties.RaiseCustomizeContextItem(e);
		}
	}
	public class SimplePopupListBoxViewInfo : BaseListBoxViewInfo {
		TextOptions popupListBoxTextOptions;
		public SimplePopupListBoxViewInfo(SimplePopupListBox owner) : base(owner) {
			this.popupListBoxTextOptions = new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.EllipsisCharacter);
		}
		protected internal Size GetItemSize(int index) {
			return CalcItemSize(index, false);
		}
		protected internal override int CalcItemMinHeight() {
			int h = base.CalcItemMinHeight();
			if(DropDownItemHeight != 0) h = DropDownItemHeight;
			return h;
		}
		public override TextOptions DefaultTextOptions { get { return popupListBoxTextOptions; } }
		protected virtual int DropDownItemHeight { get { return 0; } }
		protected internal override bool GetHtmlDrawText() {
			if(OwnerEdit == null || !OwnerEdit.ViewInfo.AllowHtmlString) return false;
			return true;
		}
		protected override ImageCollection GetHtmlImages() {
			return OwnerEdit == null ? null : OwnerEdit.Properties.HtmlImages;
		}
		protected BaseEdit OwnerEdit { get { return OwnerControl == null ? null : OwnerControl.OwnerEdit; } }
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled)
				return new ComboBoxSkinItemPainter();
			return new ComboBoxItemPainter();
		}
		protected new SimplePopupListBox OwnerControl { get { return (base.OwnerControl as SimplePopupListBox); } }
	}
	public class PopupListBoxViewInfo : SimplePopupListBoxViewInfo {
		public PopupListBoxViewInfo(PopupListBox owner)
			: base(owner) {
		}
		protected new PopupListBox OwnerControl { get { return (base.OwnerControl as PopupListBox); } }
		protected override int DropDownItemHeight {
			get { return OwnerControl.Properties.DropDownItemHeight; }
		}
		new protected ComboBoxEdit OwnerEdit { get { return base.OwnerEdit as ComboBoxEdit; } }
	}
	public class ComboBoxItemPainter : ListBoxItemPainter {
		public override int GetHorzPadding(ListBoxItemObjectInfoArgs e) { return 4; }
	}
	public class ComboBoxSkinItemPainter : ListBoxSkinItemPainter { }
}
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false)]
	public class ComboBoxItemCollection : CollectionBase {
		int lockUpdate;
		bool sorted;
		Hashtable indexes;
		RepositoryItemComboBox properties;
		public event CollectionChangeEventHandler CollectionChanged;
		public ComboBoxItemCollection(RepositoryItemComboBox properties) {
			this.sorted = false;
			this.properties = properties;
			this.indexes = new Hashtable();
			this.lockUpdate = 0;
		}
		protected internal virtual bool Sorted {
			get { return sorted; }
			set {
				if(Sorted == value) return;
				sorted = value;
				DoSort();
			}
		}
		protected virtual void DoSort() {
			if(IsLockUpdate) return;
			this.indexes.Clear();
			if(!Sorted) return;
			try {
				InnerList.Sort(CreateComparer());
			}
			catch {
			}
		}
		protected virtual IComparer CreateComparer() {
			return new DevExpress.Data.ValueComparer();
		}
		protected virtual RepositoryItemComboBox Properties { get { return properties; } }
		public virtual void Assign(ComboBoxItemCollection collection) {
			if(collection == null) return;
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					Add(CloneItem(collection[n]));
				}
				Sorted = collection.Sorted;
			}
			finally {
				EndUpdate();
			}
		}
		public virtual string GetItemDescription(object item) {
			if(item == null || item == DBNull.Value) return "";
			return string.Format("{0}", item);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("ComboBoxItemCollectionItem")]
#endif
		public object this[int index] {
			get { return List[index]; }
			set {
				object item = ExtractItem(value);
				if(!CanAddItem(item)) return;
				List[index] = item;
			}
		}
		protected virtual Hashtable Indexes { get { return indexes; } }
		protected virtual object CloneItem(object item) {
			return item;
		}
		protected virtual object ExtractItem(object item) {
			if(item is ComboBoxItem) {
				return (item as ComboBoxItem).Value;
			}
			return item;
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				DoSort();
				RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		internal void CancelUpdate() { --lockUpdate; }
		public virtual int Add(object item) {
			item = ExtractItem(item);
			if(!CanAddItem(item)) return -1;
			int index = List.Add(item);
			if(Sorted && !IsLockUpdate) {
				DoSort();
				index = IndexOf(item);
			}
			return index;
		}
		public virtual void AddRange(ICollection collection) {
			BeginUpdate();
			try {
				foreach(object item in collection) Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void AddRange(object[] items) {
			BeginUpdate();
			try {
				foreach(object item in items) Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Remove(object item) { 
			if(Contains(item))
				List.Remove(item); 
		}
		public virtual void Insert(int index, object item) {
			item = ExtractItem(item);
			if(!CanAddItem(item)) return;
			List.Insert(index, item);
		}
		public virtual int IndexOf(object item) {
			if(item != null && Indexes.Contains(item)) {
				return (int)Indexes[item];
			}
			int i = List.IndexOf(item);
			if(item != null && i > -1) Indexes[item] = i;
			return i;
		}
		public virtual bool Contains(object item) {
			return IndexOf(item) != -1;
		}
		protected virtual bool CanAddItem(object item) { return true; }
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			if(Sorted) {
				DoSort();
			}
			else {
				if(oldValue != null) Indexes.Remove(oldValue);
				if(newValue != null) Indexes[newValue] = index;
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, newValue));
		}
		protected override void OnInsertComplete(int index, object item) {
			if(Sorted)
				DoSort();
			else {
				if(index != Count - 1)
					Indexes.Clear();
				else {
					if(item != null) Indexes[item] = Count - 1;
				}
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			if(Sorted)
				DoSort();
			else {
				if(index != Count)
					Indexes.Clear();
				else
					if(item != null) Indexes.Remove(item);
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClearComplete() {
			Indexes.Clear();
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(IsLockUpdate) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
	}
	[DevExpress.Data.Access.DataPrimitive]
	public class ComboBoxItem {
		object value;
		public ComboBoxItem()
			: this(null) {
		}
		public ComboBoxItem(object val) {
			this.value = val;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ComboBoxItemValue"),
#endif
 DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public virtual object Value {
			get { return value; }
			set {
				if(Value == value) return;
				this.value = value;
				OnChanged();
			}
		}
		protected virtual void OnChanged() {
		}
	}
}
