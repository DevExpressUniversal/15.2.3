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
using System.Text;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using System.ComponentModel;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using System.Threading;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using System.Reflection;
using System.Linq;
using System.Collections;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public interface IDisplayTextEvaluatorOwner { 
		bool IsFlags { get; }
		Type FlagsType { get; }
		Array GetValues();
		char SeparatorChar { get; }
		IEnumerable GetItems(object editValue);
		bool IsItemChecked(object editValue, object item);
		string GetItemDescription(object item);
		object GetItemValue(object item);
		bool HasNegativeFlagsElements { get; }
		EditValueTypeCollection EditValueType { get; }
	}
	public class DisplayTextEvaluator {
		bool calcDisplayTextValue = true;
		string displayText = string.Empty;
		IDisplayTextEvaluatorOwner owner;
		public DisplayTextEvaluator(IDisplayTextEvaluatorOwner owner) {
			calcDisplayTextValue = true;
			displayText = string.Empty;
			this.owner = owner;
		}
		public void UpdateValues() { calcDisplayTextValue = true; }
		public void CalcDisplayText(QueryDisplayTextEventArgs e) {
			if(owner.IsFlags) {
				e.DisplayText = GetFlagDisplayText(e.EditValue, e.DisplayText);
			} else {
				string ret = string.Empty;
				if(!calcDisplayTextValue)
					ret = displayText;
				else {
					ret = string.Format("{0}", GetCheckedDescriptions(e.EditValue));
					displayText = ret;
					calcDisplayTextValue = false;
				}
				if(ret != string.Empty)
					e.DisplayText = ret;
			}
		}
		string GetFlagDisplayText(object editValue, string displayText) {
			if(editValue != null && editValue != DBNull.Value) {
				if(displayText == string.Format("{0}", editValue)) {
					long values = Convert.ToInt64(editValue);
					displayText = string.Empty;
					Array itemValues = values == 0 ? Enum.GetValues(owner.FlagsType) : owner.GetValues();
					foreach(object item in itemValues) {
						long val = Convert.ToInt64(item);
						if((values & val) > 0 || (val == 0 && values == 0))
							displayText += string.Format("{0}{1} ", EnumDisplayTextHelper.GetDisplayText(item), owner.SeparatorChar);
					}
					if(displayText.Length > 2) displayText = displayText.Substring(0, displayText.Length - 2);
				}
			}
			return displayText;
		}
		IEnumerable GetCheckItemsCollection(object editValue) {
			return owner.GetItems(editValue).Cast<object>().Where(item => owner.IsItemChecked(editValue, item));
		}
		protected virtual object GetCheckedDescriptions(object editValue) {
			if(!owner.IsFlags) {
				bool emptyResult = true;
				StringBuilder sb = new StringBuilder();
				foreach(object item in GetCheckItemsCollection(editValue)) {
					sb.AppendFormat("{0}{1} ", owner.GetItemDescription(item), owner.SeparatorChar);
					if(emptyResult && owner.GetItemDescription(item) != string.Empty) emptyResult = false;
				}
				string ret = sb.ToString();
				if(ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
				if(emptyResult) return string.Empty;
				return ret;
			} else {
				return GetCheckedItems(editValue);
			}
		}
		public object GetCheckedItems(object editValue) {
			if(!owner.IsFlags) {
				if(owner.EditValueType == EditValueTypeCollection.List) {
					IList<object> ret = CreateList();
					foreach(object item in GetCheckItemsCollection(editValue))
						ret.Add(owner.GetItemValue(item));
					return ret;
				} else {
					StringBuilder sb = new StringBuilder();
					foreach(object item in GetCheckItemsCollection(editValue))
						sb.AppendFormat("{0}{1} ", owner.GetItemValue(item), owner.SeparatorChar);
					string ret = sb.ToString();
					if(ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
					return ret;
				}
			} else {
				return Enum.ToObject(owner.FlagsType, GetCheckedFlagItems(editValue));
			}
		}
		protected virtual IList<object> CreateList() { return new List<object>(); }
		object GetCheckedFlagItems(object editValue) {
			if(owner.HasNegativeFlagsElements)
				return GetCheckedFlagItemsInt64(editValue);
			else return GetCheckedFlagItemsUInt64(editValue);
		}
		ulong GetCheckedFlagItemsUInt64(object editValue) {
			ulong ret = 0;
			foreach(object item in GetCheckItemsCollection(editValue))
				ret += Convert.ToUInt64(owner.GetItemValue(item));
			return ret;
		}
		long GetCheckedFlagItemsInt64(object editValue) {
			long ret = 0;
			foreach(object item in GetCheckItemsCollection(editValue))
				ret += Convert.ToInt64(owner.GetItemValue(item));
			return ret;
		}
	}
	public enum EditValueTypeCollection { CSV, List };
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[CheckedComboBoxCustomBindingProperties]
	public class RepositoryItemCheckedComboBoxEdit : RepositoryItemPopupContainerEdit, IDisplayTextEvaluatorOwner {
		static readonly object getItemEnabled = new object();
		static readonly object measureListBoxItem = new object();
		CheckedListBoxItemCollection items;
		internal bool collectionChanged = true;
		internal bool boundCollectionChanged = true;
		bool showAllItemVisible = true;
		bool showButtons = true;
		char separatorChar = ',';
		bool synchronizeEditValueWithCheckedItems = true;
		Type flags = null;
		bool itemSynchronizing = false;
		bool incrementalSearch = false, itemAutoHeight = false;
		bool allowMultiSelect = false;
		int lockItemsUpdate = 0;
		HighlightStyle highlightedItemStyle;
		DefaultBoolean showToolTipForTrimmedText = DefaultBoolean.Default;
		string showAllItemCaption;
		int dropDownRows = 7;
		int maxDisplayTextLength = 255;
		bool isItemsCreating = false;
		bool forceClearItems = false;
		DefaultBoolean forceUpdateEditValue = DefaultBoolean.Default;
		DisplayTextEvaluator displayTextEvaluator;
		EditValueTypeCollection editValueType = EditValueTypeCollection.CSV;
		public RepositoryItemCheckedComboBoxEdit() {
			items = CreateItemCollection();
			showAllItemCaption = Localizer.Active.GetLocalizedString(StringId.FilterShowAll);
			displayTextEvaluator = CreateDisplayTextEvaluator();
			items.ListChanged += new ListChangedEventHandler(OnChangeList);
		}
		protected virtual DisplayTextEvaluator CreateDisplayTextEvaluator() { return new DisplayTextEvaluator(this); }
		public object GetCheckedItems() {
			return GetCheckedItems(OwnerEdit == null? null: OwnerEdit.EditValue);
		}
		public object GetCheckedItems(object editValue) {
			return displayTextEvaluator.GetCheckedItems(editValue);
		}
		internal Type FlagsType { get { return flags; } }
		protected override void OnLoaded() {
			base.OnLoaded();
			if(IsLoading) return;
			ActivateDataSource();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && items != null) {
				items.ListChanged -= new ListChangedEventHandler(OnChangeList);
			}
			DisposeDataAdapter(disposing);
			base.Dispose(disposing);
		}
		void OnChangeList(object sender, ListChangedEventArgs e) {
			displayTextEvaluator.UpdateValues();
			if(!itemSynchronizing) {
				SynchronizeEditValue();
				SetCollectionChanged();
			}
		}
		internal void BeginItemsUpdate() { lockItemsUpdate++; }
		internal void EndItemsUpdate() {
			lockItemsUpdate--;
			if(lockItemsUpdate == 0) SynchronizeEditValue();
		}
		internal void ClearCollectionChanged() { collectionChanged = false; }
		internal void SetCollectionChanged() { collectionChanged = true; }
		void ClearBoundCollectionChanged() { boundCollectionChanged = false; }
		void SetBoundCollectionChanged() { boundCollectionChanged = true; }
		CheckedListBoxItemCollection CreateItemCollection() {
			return new CheckedListBoxItemCollection();
		}
		public override string EditorTypeName { get { return "CheckedComboBoxEdit"; } }
		public override void Assign(RepositoryItem item) {
			Assign(item, true);
		}
		public void ClearDataAdapter() {
			if(dataAdapter == null) return;
			dataAdapter.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
			dataAdapter.AdapterListChanged -= new ListChangedEventHandler(OnListChanged);
			dataAdapter = null;
		}
		public void Assign(RepositoryItem item, bool assignData) {
			RepositoryItemCheckedComboBoxEdit source = item as RepositoryItemCheckedComboBoxEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.showAllItemVisible = source.SelectAllItemVisible;
				this.showButtons = source.ShowButtons;
				this.highlightedItemStyle = source.highlightedItemStyle;
				this.separatorChar = source.SeparatorChar;
				this.synchronizeEditValueWithCheckedItems = source.SynchronizeEditValueWithCheckedItems;
				this.showAllItemCaption = source.SelectAllItemCaption;
				this.displayMember = source.DisplayMember;
				this.valueMember = source.ValueMember;
				this.dropDownRows = source.DropDownRows;
				this.incrementalSearch = source.IncrementalSearch;
				this.itemAutoHeight = source.ItemAutoHeight;
				this.allowMultiSelect = source.AllowMultiSelect;
				this.showToolTipForTrimmedText = source.ShowToolTipForTrimmedText;
				this.editValueType = source.EditValueType;
				this.maxDisplayTextLength = source.MaxDisplayTextLength;
				if(assignData) {
					this.dataAdapter = source.DataAdapter;
					this.dataSource = source.DataSource;
					this.items = source.Items;
					if(this.OwnerEdit != null && this.OwnerEdit.InplaceType == InplaceType.Bars) { } else {
						source.Items.BeginUpdate();
						try {
							foreach(CheckedListBoxItem element in source.Items)
								element.SetCheckStateCore(CheckState.Unchecked, false);
						} finally {
							source.Items.EndUpdate();
						}
						this.displayTextEvaluator = source.displayTextEvaluator;
					}
					this.flags = source.flags;
					this.collectionChanged = source.collectionChanged;
					this.boundCollectionChanged = source.boundCollectionChanged;
					this.forceUpdateEditValue = source.ForceUpdateEditValue;
				}
				Events.AddHandler(getItemEnabled, source.Events[getItemEnabled]);
				Events.AddHandler(measureListBoxItem, source.Events[measureListBoxItem]);
			} finally {
				EndUpdate();
			}
		}
		internal GetCheckedComboBoxItemEnabledEventHandler GetItemEnabledHandler() {
			return (GetCheckedComboBoxItemEnabledEventHandler)Events[getItemEnabled];
		}
		internal MeasureItemEventHandler MeasureListBoxItemHandler() {
			return (MeasureItemEventHandler)Events[measureListBoxItem];
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditGetItemEnabled"),
#endif
 DXCategory(CategoryName.Events)]
		public event GetCheckedComboBoxItemEnabledEventHandler GetItemEnabled {
			add { Events.AddHandler(getItemEnabled, value); }
			remove { Events.RemoveHandler(getItemEnabled, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditMeasureListBoxItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event MeasureItemEventHandler MeasureListBoxItem {
			add { Events.AddHandler(measureListBoxItem, value); }
			remove { Events.RemoveHandler(measureListBoxItem, value); }
		}
		protected internal virtual bool RaiseGetItemEnabled(GetCheckedComboBoxItemEnabledEventArgs e) {
			GetCheckedComboBoxItemEnabledEventHandler handler = GetItemEnabledHandler();
			if(handler != null) {
				e.SetListSourceRowIndex(e.Index - CustomItemsShift);
				handler(GetEventSender(), e);
			}
			return e.Enabled;
		}
		protected internal virtual void RaiseMeasureListBoxItem(object sender, MeasureItemEventArgs e) {
			MeasureItemEventHandler handler = MeasureListBoxItemHandler();
			if(handler != null) {
				handler(sender, e);
			}
		}
		protected internal virtual bool IgnoreSeparatorSpace { get { return false; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditEditValueType"),
#endif
 DefaultValue(EditValueTypeCollection.CSV)]
		public EditValueTypeCollection EditValueType {
			get { return editValueType; }
			set {
				if(EditValueType == value) return;
				editValueType = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditAllowMultiSelect"),
#endif
 DefaultValue(false), SmartTagProperty("Allow MultiSelect", "", 0)]
		public bool AllowMultiSelect {
			get { return allowMultiSelect; }
			set {
				if(AllowMultiSelect == value) return;
				allowMultiSelect = value;
				OnPropertiesChanged();
				if(OwnerCheckedComboBoxEdit != null) OwnerCheckedComboBoxEdit.UpdateListBoxProperties();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditIncrementalSearch"),
#endif
 DefaultValue(false)]
		public bool IncrementalSearch {
			get { return incrementalSearch; }
			set {
				if(IncrementalSearch == value) return;
				incrementalSearch = value;
				OnPropertiesChanged();
				if(OwnerCheckedComboBoxEdit != null) OwnerCheckedComboBoxEdit.UpdateListBoxProperties();
			}
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(false)]
		public bool ItemAutoHeight {
			get { return itemAutoHeight; }
			set {
				if(ItemAutoHeight == value) return;
				itemAutoHeight = value;
				OnPropertiesChanged();
				if(OwnerCheckedComboBoxEdit != null) OwnerCheckedComboBoxEdit.UpdateListBoxProperties();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditDropDownRows"),
#endif
 DefaultValue(7), SmartTagProperty("Drop Down Rows", "")]
		public int DropDownRows {
			get { return dropDownRows; }
			set {
				if(value < 1) value = 1;
				if(DropDownRows == value && OwnerCheckedComboBoxEdit == null) return;
				dropDownRows = value;
				OnPropertiesChanged();
				if(OwnerCheckedComboBoxEdit != null) OwnerCheckedComboBoxEdit.CoreCalcPopupHeight();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditMaxDisplayTextLength"),
#endif
 DefaultValue(255), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MaxDisplayTextLength {
			get { return maxDisplayTextLength; }
			set {
				if(value < 10) value = 10;
				if(MaxDisplayTextLength == value) return;
				maxDisplayTextLength = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditSelectAllItemVisible"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(true), SmartTagProperty("Select All Item Visible", "", 0)]
		public virtual bool SelectAllItemVisible {
			get { return this.showAllItemVisible; }
			set {
				if(SelectAllItemVisible == value) return;
				this.showAllItemVisible = value;
				OnPropertiesChanged();
				SetCollectionChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditForceUpdateEditValue"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ForceUpdateEditValue {
			get { return this.forceUpdateEditValue; }
			set {
				if(ForceUpdateEditValue == value) return;
				this.forceUpdateEditValue = value;
				OnPropertiesChanged();
				SetCollectionChanged();
			}
		}
		public override DefaultBoolean AllowHtmlDraw {
			get { return base.AllowHtmlDraw; }
			set {
				base.AllowHtmlDraw = value;
				if(OwnerCheckedComboBoxEdit != null) OwnerCheckedComboBoxEdit.UpdateListBoxProperties();
			}
		}
		[Obsolete(ObsoleteText.SRObsoleteShowAllItemVisible), DefaultValue(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ShowAllItemVisible {
			get { return this.SelectAllItemVisible; }
			set { this.SelectAllItemVisible = value; }
		}
		protected virtual void ResetSelectAllItemCaption() { SelectAllItemCaption = Localizer.Active.GetLocalizedString(StringId.FilterShowAll); }
		protected virtual bool ShouldSerializeSelectAllItemCaption() { return (SelectAllItemCaption != Localizer.Active.GetLocalizedString(StringId.FilterShowAll)); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditSelectAllItemCaption"),
#endif
 DXCategory(CategoryName.Appearance), Localizable(true)]
		public virtual string SelectAllItemCaption {
			get { return this.showAllItemCaption; }
			set {
				if(SelectAllItemCaption == value) return;
				this.showAllItemCaption = value;
				OnPropertiesChanged();
				SetCollectionChanged();
			}
		}
		protected bool ShouldSerializeShowAllItemCaption() { return false; }
		[Obsolete(ObsoleteText.SRObsoleteShowAllItemCaption), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ShowAllItemCaption {
			get { return this.SelectAllItemCaption; }
			set { this.SelectAllItemCaption = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditHighlightedItemStyle"),
#endif
 DefaultValue(HighlightStyle.Default)]
		public virtual HighlightStyle HighlightedItemStyle {
			get { return highlightedItemStyle; }
			set {
				if(HighlightedItemStyle == value) return;
				highlightedItemStyle = value;
				OnPropertiesChanged();
				SetCollectionChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditShowToolTipForTrimmedText"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowToolTipForTrimmedText {
			get { return showToolTipForTrimmedText; }
			set {
				if(ShowToolTipForTrimmedText == value) return;
				showToolTipForTrimmedText = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditSeparatorChar"),
#endif
 DXCategory(CategoryName.Format), DefaultValue(',')]
		public virtual char SeparatorChar {
			get {
				return this.separatorChar;
			}
			set {
				if(SeparatorChar == value) return;
				this.separatorChar = value;
				OnPropertiesChanged();
				SynchronizeEditValue();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditShowButtons"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(true)]
		public virtual bool ShowButtons {
			get { return this.showButtons; }
			set {
				if(ShowButtons == value) return;
				this.showButtons = value;
				UpdatePopupButtons();
				OnPropertiesChanged();
			}
		}
		void UpdatePopupButtons() {
			if(OwnerCheckedComboBoxEdit != null && OwnerCheckedComboBoxEdit.CheckedPopupForm != null)
				OwnerCheckedComboBoxEdit.CheckedPopupForm.UpdatePopupButtons();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditSynchronizeEditValueWithCheckedItems"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool SynchronizeEditValueWithCheckedItems {
			get { return this.synchronizeEditValueWithCheckedItems; }
			set {
				if(SynchronizeEditValueWithCheckedItems == value) return;
				this.synchronizeEditValueWithCheckedItems = value;
				OnPropertiesChanged();
				SynchronizeEditValue();
			}
		}
		protected internal virtual void SynchronizeItemsWithEditValue() {
			if(!SynchronizeEditValueWithCheckedItems) return;
			if(OwnerEdit == null) return;
			CheckedComboBoxEdit.DoSynchronizeEditValueWithCheckedItems(Items, this, OwnerEdit.EditValue);
		}
		protected internal virtual void SynchronizeEditValue() {
			if(lockItemsUpdate != 0) return;
			if(!SynchronizeEditValueWithCheckedItems) return;
			if(OwnerEdit != null)
				OwnerEdit.EditValue = displayTextEvaluator.GetCheckedItems(OwnerEdit.EditValue);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override PopupContainerControl PopupControl {
			get { return base.PopupControl; }
			set { base.PopupControl = value; }
		}
		[Localizable(true), DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public CheckedListBoxItemCollection Items { get { return items; } }
		public CheckedListBoxItemCollection GetItems() {
			CreateDataItems();
			return Items;
		}
		internal void SynchronizingItems(CheckedListBoxItemCollection checkedListBoxItemCollection) {
			itemSynchronizing = true;
			for(int i = 0; i < Items.Count; i++)
				Items[i].SetCheckStateCore(checkedListBoxItemCollection[i + CustomItemsShift].CheckState, false);
			Items.UpdateItemsChanged();
			itemSynchronizing = false;
		}
		int CustomItemsShift {
			get {
				return SelectAllItemVisible ? 1 : 0;
			}
		}
		protected internal override void PreQueryDisplayText(QueryDisplayTextEventArgs e) {
			base.PreQueryDisplayText(e);
			if(EditValueType == EditValueTypeCollection.List && Count(e.EditValue) == 0)
				e.DisplayText = String.Empty;
			displayTextEvaluator.CalcDisplayText(e);
		}
		public void AddEnum(Type enumType, bool addEnumeratorIntegerValues) {
			BeginUpdate();
			try {
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(object obj in values) {
					object value = EnumDisplayTextHelper.GetEnumValue(addEnumeratorIntegerValues, obj, enumType);
					Items.Add(value, EnumDisplayTextHelper.GetDisplayText(obj));
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum(Type enumType) {
			AddEnum(enumType, false);
		}
		public void AddEnum<TEnum>() {
			AddEnum<TEnum>(null);
		}
		public void AddEnum<TEnum>(Converter<TEnum, string> displayTextConverter) {
			if(displayTextConverter == null)
				displayTextConverter = (v) => EnumDisplayTextHelper.GetDisplayText(v);
			BeginUpdate();
			try {
				var values = EnumDisplayTextHelper.GetEnumValues<TEnum>();
				foreach(TEnum value in values)
					Items.Add(value, displayTextConverter(value));
			}
			finally { EndUpdate(); }
		}
		internal static int Count(object editValue) {
			ICollection collection = editValue as ICollection;
			return (collection != null) ? collection.Count : 0;
		}
		internal static bool Contains(object editValue, object value) {
			IList list = editValue as IList;
			return (list != null) && list.Contains(value);
		}
		protected internal override void PreQueryResultValue(QueryResultValueEventArgs e) {
			base.PreQueryResultValue(e);
			e.Value = displayTextEvaluator.GetCheckedItems(e.Value);
		}
		public void SetFlags(Type enumType) {
			flags = enumType;
			CreateFlagsItems();
		}
		void CreateFlagsItems() {
			if(!IsFlags) return;
			this.items.Clear();
			foreach(object val in Enum.GetValues(flags))
				if(!IsZero(val))
					this.items.Add(val, EnumDisplayTextHelper.GetDisplayText(val));
		}
		bool IsZero(object val) {
			int ret = -1;
			try {
				ret = Convert.ToInt32(val);
			} catch { }
			return 0.Equals(ret);
		}
		internal bool IsFlags {
			get { return flags != null; }
		}
		internal bool HasNegativeFlagsElements {
			get {
				if(!IsFlags) return false;
				foreach(object val in Enum.GetValues(flags)) {
					if(IsNegativeElement(val)) return true;
				}
				return false;
			}
		}
		bool IsNegativeElement(object val) {
			bool ret = false;
			try {
				ret = Convert.ToInt64(val) < 0;
			} catch { }
			return ret;
		}
		internal CheckedComboBoxEdit OwnerCheckedComboBoxEdit { get { return OwnerEdit as CheckedComboBoxEdit; } }
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			if(OwnerCheckedComboBoxEdit != null)
				OwnerCheckedComboBoxEdit.SyncLookAndFeel();
		}
		#region DataBinding
		object dataSource;
		string displayMember;
		string valueMember;
		ListDataAdapter dataAdapter;
		static object dataSourceChanged = new object();
		static object valueMemberChanged = new object();
		static object displayMemberChanged = new object();
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditDisplayMemberChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler DisplayMemberChanged {
			add { this.Events.AddHandler(displayMemberChanged, value); }
			remove { this.Events.RemoveHandler(displayMemberChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditValueMemberChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler ValueMemberChanged {
			add { this.Events.AddHandler(valueMemberChanged, value); }
			remove { this.Events.RemoveHandler(valueMemberChanged, value); }
		}
		private void InitDataBindingElements() {
			this.dataSource = null;
			this.displayMember = string.Empty;
			this.valueMember = string.Empty;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditDataSource"),
#endif
 AttributeProvider(typeof(IListSource)), DefaultValue(null), RefreshProperties(RefreshProperties.Repaint), DXCategory(CategoryName.Data)]
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(DataSource == null && value != null) forceClearItems = false;
				if(DataSource != null && value == null) forceClearItems = true;
				dataSource = value;
				ActivateDataSource();
				RaiseDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditDisplayMember"),
#endif
 DefaultValue(""), DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string DisplayMember {
			get {
				if(displayMember == null) displayMember = string.Empty;
				return displayMember;
			}
			set {
				if(value == null) value = string.Empty;
				if(value == DisplayMember) return;
				displayMember = value;
				ActivateDataSource();
				RaiseDisplayMemberChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckedComboBoxEditValueMember"),
#endif
 DefaultValue(""), DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string ValueMember {
			get {
				if(valueMember == null) valueMember = string.Empty;
				return valueMember;
			}
			set {
				if(value == null) value = string.Empty;
				if(value == ValueMember) return;
				valueMember = value;
				ActivateDataSource();
				RaiseValueMemberChanged();
			}
		}
		protected virtual void ActivateDataSource() {
			if(IsLoading) return;
			DataAdapter.SetDataSource(DataSource, DisplayMember, ValueMember);
		}
		protected void RaiseDataSourceChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseDisplayMemberChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[displayMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseValueMemberChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[valueMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected ListDataAdapter DataAdapter {
			get {
				if(dataAdapter == null) {
					dataAdapter = CreateDataAdapter();
					dataAdapter.DataSourceChanged += new EventHandler(OnDataSourceChanged);
					dataAdapter.AdapterListChanged += new ListChangedEventHandler(OnListChanged);
				}
				return dataAdapter;
			}
		}
		void DisposeDataAdapter(bool disposing) {
			if(OwnerEdit == null || OwnerEdit.InplaceType == InplaceType.Standalone) {
				if(!Cloned && dataAdapter != null) {
					dataAdapter.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
					dataAdapter.AdapterListChanged -= new ListChangedEventHandler(OnListChanged);
					dataAdapter.Dispose();
				}
			}
		}
		public void RefreshDataSource() {
			SetCollectionChanged();
			SetBoundCollectionChanged();
			if(OwnerCheckedComboBoxEdit != null)
				OwnerCheckedComboBoxEdit.SynchronizingItems();
		}
		protected virtual void OnDataSourceChanged(object sender, EventArgs e) {
			SetCollectionChanged();
			SetBoundCollectionChanged();
		}
		protected virtual void OnListChanged(object sender, ListChangedEventArgs e) {
			SetCollectionChanged();
			SetBoundCollectionChanged();
		}
		protected virtual ListDataAdapter CreateDataAdapter() { return new ListDataAdapter(); }
		protected internal bool IsBoundMode { get { return dataAdapter != null && DataAdapter.ListSource != null; } }
		internal bool CreateDataItems() {
			if(forceClearItems && this.items.Count > 0) {
				this.items.Clear();
				forceClearItems = false;
				return true;
			}
			if(!IsBoundMode) return false;
			if(!boundCollectionChanged) return false;
			if(isItemsCreating) return false;
			isItemsCreating = true;
			this.items.Clear();
			this.items.BeginUpdate();
			try {
				for(int i = 0; i < DataAdapter.ListSource.Count; i++) {
					if(DisplayMember == string.Empty) {
						this.items.Add(DataAdapter.GetValueAtIndex(ValueViewMember, i));
					} else {
						this.items.Add(DataAdapter.GetValueAtIndex(ValueViewMember, i), string.Format("{0}", DataAdapter.GetValueAtIndex(DisplayMember, i)));
					}
				}
			} finally {
				this.items.EndUpdate();
			}
			ClearBoundCollectionChanged();
			isItemsCreating = false;
			return true;
		}
		protected string ValueViewMember { get { return ((ValueMember == string.Empty) ? DisplayMember : ValueMember); } }
		#endregion
		#region GridDisplayText
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			if(!IsFlags && (this.OwnerEdit == null || this.OwnerEdit.InplaceType != InplaceType.Standalone)) {
				string displayText = string.Format("{0}", editValue);
				if(IsBoundMode) displayText = GetDisplayText(editValue, true);
				else if(HasUnboundDifferentItemMembers) {
					displayText = GetDisplayText(editValue, false);
				}
				CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(editValue, displayText);
				if(format != EditFormat) RaiseCustomDisplayText(e);
				return e.DisplayText;
			}
			CustomDisplayTextEventArgs args = new CustomDisplayTextEventArgs(editValue, base.GetDisplayText(format, editValue)); 
			if(format != EditFormat) RaiseCustomDisplayText(args);
			return args.DisplayText;
		}
		bool HasUnboundDifferentItemMembers {
			get {
				if(IsFlags || IsBoundMode) return false;
				int num = Math.Min(2, Items.Count);
				for(int i = 0; i < num; i++)
					if(Items[i].Description != null && Items[i].Description != string.Empty)
						return true;
				return false;
			}
		}
		private string GetDisplayText(object editValue, bool bound) {
			string displayText = string.Format("{0}", editValue);
			if(displayText == string.Empty || (DisplayMember == string.Empty && bound)) return displayText;
			string[] values = displayText.Split(SeparatorChar);
			if(EditValueType == EditValueTypeCollection.List && editValue is IEnumerable) 
				values = ((IEnumerable)editValue).Cast<object>().Select(val => val.ToString()).ToArray();
			string ret = string.Empty;
			bool drawEllipsis = false;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < values.Length; i++) {
				string val = values[i].TrimStart(null);
				if(ret.Length > MaxDisplayTextLength) {
					drawEllipsis = true;
					break;
				}
				sb.AppendFormat("{0}{1} ", bound ? GetBoundDisplayTextByValue(val) : GetDisplayTextByValue(val), SeparatorChar);
			}
			ret = sb.ToString();
			if(ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
			if(drawEllipsis) ret += "...";
			return ret;
		}
		private string GetDisplayTextByValue(string val) {
			for(int i = 0; i < Items.Count; i++) {
				if(val.Equals(string.Format("{0}", Items[i].Value))) {
					return string.Format("{0}", Items[i].Description);
				}
			}
			return val;
		}
		string GetBoundDisplayTextByValue(string val) {
			for(int i = 0; i < DataAdapter.ListSource.Count; i++) {
				if(val.Equals(string.Format("{0}", DataAdapter.GetValueAtIndex(ValueViewMember, i)))) {
					return string.Format("{0}", DataAdapter.GetValueAtIndex(DisplayMember, i));
				}
			}
			return val;
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event QueryDisplayTextEventHandler QueryDisplayText {
			add { base.QueryDisplayText += value; }
			remove { base.QueryDisplayText -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event QueryResultValueEventHandler QueryResultValue {
			add { base.QueryResultValue += value; }
			remove { base.QueryResultValue -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int MaxLength {
			get { return base.MaxLength; }
			set { base.MaxLength = value; }
		}
		public override bool IsFilterLookUp { get { return true; } }
		#region IDisplayTextEvaluatorOwner Members
		bool IDisplayTextEvaluatorOwner.IsFlags {
			get { return IsFlags; }
		}
		Type IDisplayTextEvaluatorOwner.FlagsType {
			get { return FlagsType; }
		}
		Array IDisplayTextEvaluatorOwner.GetValues() {
			return Items.Cast<CheckedListBoxItem>().Select(item => item.Value).ToArray();
		}
		char IDisplayTextEvaluatorOwner.SeparatorChar {
			get { return SeparatorChar; }
		}
		IEnumerable IDisplayTextEvaluatorOwner.GetItems(object editValue) {
			return Items;
		}
		bool IDisplayTextEvaluatorOwner.IsItemChecked(object editValue, object item) {
			return ((CheckedListBoxItem)item).CheckState == CheckState.Checked;
		}
		string IDisplayTextEvaluatorOwner.GetItemDescription(object item) {
			string ret = ((CheckedListBoxItem)item).Description;
			if(EditValueType == EditValueTypeCollection.List && string.IsNullOrEmpty(ret))
				ret = string.Format("{0}", ((CheckedListBoxItem)item).Value);
			return ret;
		}
		object IDisplayTextEvaluatorOwner.GetItemValue(object item) {
			return ((CheckedListBoxItem)item).Value;
		}
		bool IDisplayTextEvaluatorOwner.HasNegativeFlagsElements {
			get { return HasNegativeFlagsElements; }
		}
		#endregion
		protected class CheckedComboBoxCustomBindingPropertiesAttribute : DevExpress.Utils.Design.DataAccess.CustomBindingPropertiesAttribute {
			public override IEnumerable<DevExpress.Utils.Design.DataAccess.ICustomBindingProperty> GetCustomBindingProperties() {
				return new DevExpress.Utils.Design.DataAccess.ICustomBindingProperty[] {
						new CustomBindingPropertyAttribute("DisplayMember", "Display Member", GetDisplayMemberDescription()),
						new CustomBindingPropertyAttribute("ValueMember", "Value Member", GetValueMemberDescription())
					};
			}
			protected virtual string GetValueMemberDescription() {
				return "Gets or sets the field name in the bound data source whose contents are assigned to item values.";
			}
			protected virtual string GetDisplayMemberDescription() {
				return "Gets or sets a field name in the bound data source whose contents are to be displayed by the control's check items.";
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
namespace DevExpress.XtraEditors.Controls {
	class ShowAllElement {
		CheckedListBoxItemCollection items;
		CheckedListBoxControl control;
		string caption = string.Empty;
		public ShowAllElement(CheckedListBoxControl control, string caption) {
			this.items = control.Items;
			this.control = control;
			this.caption = caption;
		}
		public override string ToString() {
			if(caption == null || caption == string.Empty)
				return Localizer.Active.GetLocalizedString(StringId.FilterShowAll);
			return caption;
		}
		public void UpdateCheckStateElement() {
			items[0].CheckState = ItemsCheckStyle();
		}
		public void UpdateCheckStateItems() {
			CheckState state = items[0].CheckState;
			control.BeginUpdate();
			for(int i = 1; i < items.Count; i++) {
				if(items[i].Enabled) 
					items[i].SetCheckStateCore(state, false);
			}
			items.UpdateItemsChanged();
			control.EndUpdate();
		}
		internal CheckState ItemsCheckStyle() {
			if(items.Count < 2) return CheckState.Unchecked;
			CheckState state = items[1].CheckState;
			for(int i = 2; i < items.Count; i++) {
				if(!items[i].Enabled) continue;
				if(items[i].CheckState != state)
					return CheckState.Indeterminate;
			}
			return state;
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class CheckedPopupContainerForm : PopupContainerForm {
		public CheckedPopupContainerForm(PopupContainerEdit ownerEdit)
			: base(ownerEdit) {
			CreateSeparatorLine();
		}
		protected override void SetupButtons() {
			UpdatePopupButtons();
		}
		new RepositoryItemCheckedComboBoxEdit Properties {
			get {
				CheckedComboBoxEdit edit = OwnerEdit as CheckedComboBoxEdit;
				if(edit == null) return null;
				return edit.Properties;
			}
		}
		internal void UpdatePopupButtons() {
			if(Properties == null) return;
			this.fShowOkButton = Properties.ShowButtons;
			if(Properties.ShowPopupCloseButton)
				this.fCloseButtonStyle = Properties.ShowButtons ? BlobCloseButtonStyle.Caption : BlobCloseButtonStyle.Glyph;
			else
				this.fCloseButtonStyle = BlobCloseButtonStyle.None;
			this.AllowSizing = Properties.PopupSizeable;
			if(!AllowSizing && !fShowOkButton && fCloseButtonStyle == BlobCloseButtonStyle.None)
				ViewInfo.ShowSizeBar = false;
		}
	}
	internal class PopupCheckedListBoxControl : CheckedListBoxControl {
		bool readOnly = false;
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(readOnly == value) return;
				readOnly = value;
			}
		}
		protected internal override void OnChangeCheck(int index) {
			if(!ReadOnly)
				base.OnChangeCheck(index);
		}
		public override ContextItemCollection ContextButtons {
			get {
				PopupContainerControl parent = Parent as PopupContainerControl;
				if(parent != null) return parent.OwnerItem != null ? parent.OwnerItem.ContextButtons : base.ContextButtons;
				return base.ContextButtons;
			}
		}
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get {
				PopupContainerControl parent = Parent as PopupContainerControl;
				if(parent != null) return parent.OwnerItem != null ? parent.OwnerItem.ContextButtonOptions : base.ContextButtonOptions;
				return base.ContextButtonOptions;
			}
		}
		protected internal override void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			base.RaiseContextButtonClick(e);
			PopupContainerControl parent = Parent as PopupContainerControl;
			if(parent != null && parent.OwnerItem != null) parent.OwnerItem.RaiseContextButtonClick(e);
		}
		protected internal override void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			base.RaiseContextButtonValueChanged(e);
			PopupContainerControl parent = Parent as PopupContainerControl;
			if(parent != null && parent.OwnerItem != null) parent.OwnerItem.RaiseContextButtonValueChanged(e);
		}
		protected internal override void RaiseCustomizeContextItem(ListBoxControlContextButtonCustomizeEventArgs e) {
			base.RaiseCustomizeContextItem(e);
			PopupContainerControl parent = Parent as PopupContainerControl;
			if(parent != null && parent.OwnerItem != null) parent.OwnerItem.RaiseCustomizeContextItem(e);
		}
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraEditors.Design.CheckedComboBoxEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays a check item list in a drop-down window."),
	 DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(CheckedComboBoxEditFilter)), SmartTagAction(typeof(CheckedComboBoxEditActions), "Items", "Edit Items", SmartTagActionType.CloseAfterExecute),
		ToolboxBitmap(typeof(ToolboxIconsRootNS), "CheckedComboBoxEdit")
	]
	public class CheckedComboBoxEdit : PopupContainerEdit {
		public CheckedComboBoxEdit() { }
		[Obsolete(ObsoleteText.SRObsoleteDefaultPopupHeight)]
		protected virtual int DefaultPopupHeight { get { return 139; } }
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(!IsDesignMode && EditValue == null && InplaceType == InplaceType.Standalone)
				Properties.SynchronizeEditValue();
		}
		internal CheckedPopupContainerForm CheckedPopupForm { get { return PopupForm as CheckedPopupContainerForm; } }
		protected override PopupBaseForm CreatePopupForm() {
			if(Properties.PopupControl == null) return null;
			return new CheckedPopupContainerForm(this);
		}
		PopupCheckedListBoxControl listBox = null;
		CheckedListBoxItem showElement = null;
		protected internal override void RefreshPopup() {
			SynchronizingItems();
			if(Properties.PopupControl != null)
				Properties.PopupControl.Height = GetPopupHeight();
			CalcPopupHeight();
			base.RefreshPopup();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckedComboBoxEditGetItemEnabled"),
#endif
 DXCategory(CategoryName.Events)]
		public event GetCheckedComboBoxItemEnabledEventHandler GetItemEnabled {
			add { Properties.GetItemEnabled += value; }
			remove { Properties.GetItemEnabled -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckedComboBoxEditMeasureListBoxItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event MeasureItemEventHandler MeasureListBoxItem {
			add { Properties.MeasureListBoxItem += value; }
			remove { Properties.MeasureListBoxItem -= value; }
		}
		void CheckPopupControl() {
			if(Properties.PopupControl == null) {
				this.Properties.PopupControl = CreatePopupCheckListControl();
				Size size = Properties.GetDesiredPopupFormSize(true);
				if(size.Width <= 0) Properties.PopupControl.Width = Math.Max(Properties.PopupControl.Width, this.Width);
				if(size.Height <= 0) Properties.PopupControl.Height = GetPopupHeight();
			}
			UpdateStyleProperties();
			SynchronizingItems();
			if(Properties.SynchronizeEditValueWithCheckedItems)
				DoSynchronizeEditValueWithCheckedItems();
			listBox.SelectedIndex = 0;
		}
		protected override void DoShowPopup() {
			CheckPopupControl();
			CalcPopupHeight();
			if(Properties.GetItemEnabledHandler() != null) {
				for(int i = 0; i < listBox.Items.Count; i++)
					listBox.Items[i].Enabled = Properties.RaiseGetItemEnabled(new GetCheckedComboBoxItemEnabledEventArgs(i, listBox.Items[i].Enabled, listBox));
			}
			listBox.ReadOnly = Properties.ReadOnly;
			base.DoShowPopup();
		}
		public override bool DoValidate(PopupCloseMode closeMode) {
			DoClosePopup(closeMode);
			return base.DoValidate(closeMode);
		}
		int CalcBorderHeight(CheckedListBoxViewInfo vInfo) {
			int res = 0;
			int rectLength = 300;
			vInfo.GInfo.AddGraphics(null);
			try {
				res = vInfo.BorderPainter.CalcBoundsByClientRectangle(new DevExpress.Utils.Drawing.BorderObjectInfoArgs(vInfo.GInfo.Cache, new Rectangle(0, 0, rectLength, rectLength), null)).Height;
			} finally {
				vInfo.GInfo.ReleaseGraphics();
			}
			return res - rectLength;
		}
		int CalcListBoxHeignt(int rowCount) {
			return rowCount * listBox.ViewInfo.ItemHeight + CalcBorderHeight(listBox.ViewInfo);
		}
		int GetPopupHeight() {
			return CalcListBoxHeignt(Properties.DropDownRows);
		}
		internal void CoreCalcPopupHeight() {
			if(Properties.PopupControl != null) {
				Properties.PopupControl.Height = 0;
				Properties.PopupControl.Height = GetPopupHeight();
			}
		}
		void CalcPopupHeight() {
			int height = CalcListBoxHeignt(listBox.ItemCount);
			int heightMin = CalcListBoxHeignt(Math.Min(listBox.ItemCount, 2));
			if(Properties.PopupControl.Height > height)
				Properties.PopupControl.Height = height;
			if(Properties.PopupControl.Height < heightMin)
				Properties.PopupControl.Height = heightMin;
		}
		static bool UpdateCheckItem(CheckedListBoxItem item, bool val) {
			bool ret = false;
			if(val && item.CheckState != CheckState.Checked) {
				item.CheckState = CheckState.Checked;
				ret = true;
			}
			if(!val && item.CheckState != CheckState.Unchecked) {
				item.CheckState = CheckState.Unchecked;
				ret = true;
			}
			return ret;
		}
		static string GetStringValue(string value, bool first, bool ignoreSeparatorSpace) {
			if(!ignoreSeparatorSpace && !first && value.IndexOf((char)32) == 0)
				return value.Substring(1);
			return value;
		}
		internal static bool DoSynchronizeEditValueWithCheckedItems(CheckedListBoxItemCollection lbItems, RepositoryItemCheckedComboBoxEdit p, object value) {
			lbItems.BeginUpdate();
			bool update = false;
			try {
				if(!p.IsFlags) {
					string[] synchString = string.Format("{0}", value).Split(p.SeparatorChar);
					if(synchString.Length > 0 && !p.IgnoreSeparatorSpace) synchString[0] = synchString[0].Insert(0, " ");
					var vals = synchString.Distinct().Aggregate(new HashSet<object>(), (set, v) => { set.Add(GetStringValue(v, false, p.IgnoreSeparatorSpace)); return set; });
					foreach(CheckedListBoxItem item in lbItems) {
						if(item.Value == null) continue;
						if(UpdateCheckItem(item,
								(p.EditValueType == EditValueTypeCollection.List) ?
								RepositoryItemCheckedComboBoxEdit.Contains(value, item.Value) :
								vals.Contains(string.Format("{0}", item.Value))))
							update = true;
					}
				} else {
					if(p.HasNegativeFlagsElements)
						update = DoSynchronizeFlagEditValueWithCheckedItemsInt64(lbItems, value);
					else update = DoSynchronizeFlagEditValueWithCheckedItemsUInt64(lbItems, value);
				}
			} finally {
				lbItems.EndUpdate();
			}
			return update;
		}
		static bool DoSynchronizeFlagEditValueWithCheckedItemsInt64(CheckedListBoxItemCollection lbItems, object value) {
			bool ret = false;
			long val = 0;
			try {
				if(value != null && !string.Empty.Equals(value) && value != DBNull.Value)
					val = Convert.ToInt64(value);
			} catch { }
			foreach(CheckedListBoxItem item in lbItems) {
				if(item.Value is ShowAllElement) continue;
				if(UpdateCheckItem(item,
			(val & Convert.ToInt64(item.Value)) != 0))
					ret = true;
			}
			return ret;
		}
		static bool DoSynchronizeFlagEditValueWithCheckedItemsUInt64(CheckedListBoxItemCollection lbItems, object value) {
			bool ret = false;
			ulong val = 0;
			try {
				if(value != null && !string.Empty.Equals(value) && value != DBNull.Value)
					val = Convert.ToUInt64(value);
			} catch { }
			foreach(CheckedListBoxItem item in lbItems) {
				if(item.Value is ShowAllElement) continue;
				if(UpdateCheckItem(item,
					(val & Convert.ToUInt64(item.Value)) != 0))
					ret = true;
			}
			return ret;
		}
		protected internal bool IsAllSelectedItemsChecked() {
			if(listBox == null) return false;
			var selectAllItem = listBox.Items.FirstOrDefault(item => item.Value is ShowAllElement);
			return (selectAllItem != null) && selectAllItem.CheckState == CheckState.Checked;
		}
		protected virtual void DoSynchronizeEditValueWithCheckedItems() {
			if(DoSynchronizeEditValueWithCheckedItems(listBox.Items, Properties, EditValue))
				UpdateShowAll(-1);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(listBox != null) {
					listBox.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(OnItemCheck);
					listBox.KeyDown -= new KeyEventHandler(OnListBoxKeyDown);
					listBox.KeyUp -= new KeyEventHandler(OnListBoxKeyUp);
					listBox.MeasureItem -= new MeasureItemEventHandler(OnListBoxMeasureItem);
				}
			}
			base.Dispose(disposing);
		}
		internal void SynchronizingItems() {
			if(!Properties.collectionChanged) return;
			object value = EditValue;
			if(Properties.CreateDataItems()) {
				if(value != null && !string.Empty.Equals(value))
					SetEditValue(value);
			}
			listBox.Items.BeginUpdate();
			listBox.Items.Clear();
			showElement = null;
			if(Properties.SelectAllItemVisible) {
				showElement = new CheckedListBoxItem(new ShowAllElement(listBox, Properties.SelectAllItemCaption));
				listBox.Items.Add(showElement);
			}
			foreach(CheckedListBoxItem item in Properties.Items)
				listBox.Items.Add(new CheckedListBoxItem(item.Value, item.Description, item.CheckState, item.Enabled));
			listBox.Items.EndUpdate();
			UpdateShowAll();
			Properties.ClearCollectionChanged();
		}
		void UpdateShowAll() {
			UpdateShowAll(-1);
		}
		bool lockUpdateShowAllOnCheckItemEvent = false;
		void UpdateShowAll(int index) {
			if(showElement == null) return;
			lockUpdateShowAllOnCheckItemEvent = true;
			try {
				ShowAllElement element = showElement.Value as ShowAllElement;
				if(element == null) return;
				if(index != 0)
					element.UpdateCheckStateElement();
				else
					element.UpdateCheckStateItems();
			} finally {
				lockUpdateShowAllOnCheckItemEvent = false;
			}
		}
		public override string EditorTypeName { get { return "CheckedComboBoxEdit"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemCheckedComboBoxEdit Properties {
			get { return base.Properties as RepositoryItemCheckedComboBoxEdit; }
		}
		internal void UpdateListBoxProperties() {
			if(listBox == null) return;
			listBox.IncrementalSearch = Properties.IncrementalSearch;
			listBox.ItemAutoHeight = Properties.ItemAutoHeight;
			listBox.SelectionMode = Properties.AllowMultiSelect ? SelectionMode.MultiExtended : SelectionMode.One;
			listBox.AllowHtmlDraw = Properties.AllowHtmlDraw;
			listBox.HtmlImages = Properties.HtmlImages;
			listBox.ShowToolTipForTrimmedText = Properties.ShowToolTipForTrimmedText;
		}
		protected virtual PopupContainerControl CreatePopupCheckListControl() {
			listBox = new PopupCheckedListBoxControl();
			PopupContainerControl control = new PopupContainerControl();
			listBox.Parent = control;
			listBox.BorderStyle = BorderStyles.NoBorder;
			listBox.Dock = DockStyle.Fill;
			listBox.TabIndex = 0;
			listBox.CheckOnClick = true;
			listBox.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(OnItemCheck);
			listBox.KeyDown += new KeyEventHandler(OnListBoxKeyDown);
			listBox.KeyUp += new KeyEventHandler(OnListBoxKeyUp);
			listBox.MeasureItem += new MeasureItemEventHandler(OnListBoxMeasureItem);
			listBox.UseDisabledStatePainter = false;
			UpdateListBoxProperties();
			UpdateStyleProperties();
			SyncLookAndFeel();
			return control;
		}
		void OnListBoxMeasureItem(object sender, MeasureItemEventArgs e) {
			if(Properties.MeasureListBoxItemHandler() != null) {
				Properties.RaiseMeasureListBoxItem(sender, e);
			}
		}
		void UpdateStyleProperties() {
			listBox.HighlightedItemStyle = Properties.HighlightedItemStyle;
			if(StyleController != null)
				AppearanceHelper.Combine(listBox.Appearance, Properties.AppearanceDropDown, StyleController.AppearanceDropDown);
			else
				listBox.Appearance.Assign(Properties.AppearanceDropDown);
			listBox.ViewInfo.UpdateAppearances();
		}
		internal void SyncLookAndFeel() {
			if(listBox == null) return;
			listBox.LookAndFeel.Assign(this.Properties.LookAndFeel);
		}
		protected virtual void OnItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if(lockUpdateShowAllOnCheckItemEvent) return;
			string search = listBox.Handler.ControlState.CurrentSearch;
			UpdateShowAll(e.Index);
			listBox.Handler.ControlState.SetCurrentSearch(search);
		}
		bool keyDown = false;
		protected virtual void OnListBoxKeyDown(object sender, KeyEventArgs e) {
			keyDown = true;
			if(e.KeyCode == Keys.Enter) {
				if(Properties.AllowMultiSelect && listBox.Handler.CurrentSearch != null && listBox.SelectedIndex >= 0)
					listBox.ToggleItem(listBox.SelectedIndex);
			}
		}
		void OnListBoxKeyUp(object sender, KeyEventArgs e) {
			if(!keyDown) return;
			keyDown = false;
			if(e.KeyCode == Keys.Enter) {
				ClosePopup(PopupCloseMode.ButtonClick);
			}
		}
		protected override void UpdateEditValueOnClose(PopupCloseMode closeMode, bool acceptValue, object newValue, object oldValue) {
			base.UpdateEditValueOnClose(closeMode, acceptValue, newValue, oldValue);
			if(!acceptValue) Properties.SynchronizeItemsWithEditValue();
		}
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			if(IsAcceptCloseMode(closeMode)) {
				if(listBox != null)
					Properties.SynchronizingItems(listBox.Items);
			} else {
				Properties.SetCollectionChanged();
			}
			base.DoClosePopup(closeMode);
		}
		void FillDataItems() {
			if(Properties.IsBoundMode && (Properties.Items.Count == 0 || Properties.boundCollectionChanged)) {
				Properties.CreateDataItems();
			}
		}
		public void CheckAll() {
			FillDataItems();
			try {
				Properties.BeginItemsUpdate();
				foreach(CheckedListBoxItem item in Properties.Items)
					item.SetCheckStateCore(CheckState.Checked, false);
			} finally {
				Properties.Items.UpdateItemsChanged();
				Properties.EndItemsUpdate();
			}
		}
		public virtual void SetEditValue(object value) {
			FillDataItems();
			this.EditValue = value;
			Properties.SynchronizeItemsWithEditValue();
			this.LayoutChanged();
		}
		public override void RefreshEditValue() {
			SetEditValue(this.EditValue);
		}
		bool AllowForceUpdateEditValue {
			get {
				if(Properties.ForceUpdateEditValue == DefaultBoolean.Default)
					return IsBoundToEditValue;
				return Properties.ForceUpdateEditValue == DefaultBoolean.True ? true : false;
			}
		}
		bool UpdateEditValue { get { return AllowForceUpdateEditValue && updateEditValue; } }
		bool updateEditValue = false;
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if(base.EditValue == value || UpdateEditValue) return;
				updateEditValue = true;
				try {
					if(AllowForceUpdateEditValue)
						FillDataItems();
					base.EditValue = value;
					if(AllowForceUpdateEditValue) {
						Properties.SynchronizeItemsWithEditValue();
						this.LayoutChanged();
					}
				} finally {
					updateEditValue = false;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event QueryDisplayTextEventHandler QueryDisplayText {
			add { base.QueryDisplayText += value; }
			remove { base.QueryDisplayText -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event QueryResultValueEventHandler QueryResultValue {
			add { base.QueryResultValue += value; }
			remove { base.QueryResultValue -= value; }
		}
		protected override void OnCancelEditValueChanging() {
			base.OnCancelEditValueChanging();
			Properties.SynchronizeItemsWithEditValue();
		}
		#region FindString
		public int FindString(string s, int startIndex, bool updown) {
			s = s.ToLower();
			return FindItem(startIndex, updown, delegate(ListBoxFindItemArgs e) {
				string itemText = e.DisplayText.ToLower();
				e.IsFound = itemText.StartsWith(s) && ((s != string.Empty) || (itemText == string.Empty));
			});
		}
		public int FindString(string s, int startIndex) { return FindString(s, startIndex, true); }
		public int FindString(string s) { return FindString(s, -1); }
		public int FindStringExact(string s, int startIndex) {
			s = s.ToLower();
			return FindItem(startIndex, true, delegate(ListBoxFindItemArgs e) {
				string itemText = e.DisplayText.ToLower();
				e.IsFound = s.Equals(itemText);
			});
		}
		public int FindStringExact(string s) { return FindStringExact(s, -1); }
		public int FindItem(int startIndex, bool updown, ListBoxFindItemDelegate predicate) {
			CheckPopupControl();
			return listBox.FindItem(startIndex, updown, predicate);
		}
		#endregion
#if DEBUGTEST
		internal CheckedListBoxControl ListBox { get { return listBox; } }
#endif
	}
}
namespace DevExpress.XtraEditors.Filtering {
	public class FilterRepositoryItemCheckedComboBoxEdit : RepositoryItemCheckedComboBoxEdit {
		static FilterRepositoryItemCheckedComboBoxEdit() { RegisterFilterCheckedComboBoxEdit(); }
		internal static void RegisterFilterCheckedComboBoxEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("FilterCheckedComboBoxEdit", typeof(FilterCheckedComboBoxEdit), typeof(FilterRepositoryItemCheckedComboBoxEdit), typeof(DevExpress.XtraEditors.ViewInfo.PopupContainerEditViewInfo), new ButtonEditPainter(), true, null, typeof(DevExpress.Accessibility.ComboBoxEditAccessible)));
		}
		public override string EditorTypeName { get { return "FilterCheckedComboBoxEdit"; } }
		protected internal override void SynchronizeItemsWithEditValue() { }
		protected internal override void SynchronizeEditValue() { }
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			string ret = string.Empty;
			foreach(CheckedListBoxItem item in Items) {
				if(item.CheckState == CheckState.Checked)
					ret += string.Format("{0}{1} ", item.Description, SeparatorChar);
				if(ret.Length > 100) break; 
			}
			if(ret.Length > 2) ret = ret.Substring(0, ret.Length - 2);
			return ret;
		}
	}
	[ToolboxItem(false)]
	public class FilterCheckedComboBoxEdit : CheckedComboBoxEdit {
		public override string EditorTypeName { get { return "FilterCheckedComboBoxEdit"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterRepositoryItemCheckedComboBoxEdit Properties {
			get { return base.Properties as FilterRepositoryItemCheckedComboBoxEdit; }
		}
		internal void SetValues(IList<object> list) {
			Properties.Items.BeginUpdate();
			try {
				foreach(CheckedListBoxItem item in Properties.Items)
					SetCheckState(item, list);
			} finally {
				Properties.Items.EndUpdate();
			}
			IsModified = true;
		}
		void SetCheckState(CheckedListBoxItem item, IList<object> list) {
			item.CheckState = CheckState.Unchecked;
			for(int i = 0; i < list.Count; i++)
				if(object.Equals(item.Value, list[i])) {
					item.CheckState = CheckState.Checked;
					break;
				}
		}
		protected override void DoSynchronizeEditValueWithCheckedItems() { }
		public override void SetEditValue(object value) { }
		public override object EditValue {
			get { return string.Empty; }
			set { }
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public class GetCheckedComboBoxItemEnabledEventArgs : GetItemEnabledEventArgs {
		CheckedListBoxControl listBox;
		int listSourceRowIndex;
		public GetCheckedComboBoxItemEnabledEventArgs(int index, bool enabled, CheckedListBoxControl listBox)
			: base(index, enabled) {
			this.listBox = listBox;
		}
		internal void SetListSourceRowIndex(int index) { listSourceRowIndex = index; }
		public CheckedListBoxControl ListBox { get { return listBox; } }
		public int ListSourceRowIndex { get { return listSourceRowIndex; } }
	}
	public delegate void GetCheckedComboBoxItemEnabledEventHandler(object sender, GetCheckedComboBoxItemEnabledEventArgs e);
}
