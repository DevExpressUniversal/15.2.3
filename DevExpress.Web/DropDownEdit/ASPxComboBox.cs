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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum DropDownStyle { DropDown, DropDownList }
	public enum IncrementalFilteringMode { Contains, StartsWith, None }
	public class ComboBoxListBoxProperties : ListBoxProperties, IListBoxRenderHelperOwner, IListBoxColumnsOwner, IListEditItemsRequester {
		public ComboBoxListBoxProperties()
			: base() {
		}
		public ComboBoxListBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal override bool CallbackModeEnabled {
			get { return EnableCallbackMode && CallbackModePossibleInernal; }
		}
		protected internal ASPxAutoCompleteBoxBase AutoCompleteBox {
			get { return AutoCompleteBoxProperties.AutoCompleteBox; }
		}
		protected internal AutoCompleteBoxPropertiesBase AutoCompleteBoxProperties {
			get { return Owner as AutoCompleteBoxPropertiesBase; }
		}
		protected internal ASPxComboBox ComboBox {
			get { return ComboBoxProperties == null ? null : ComboBoxProperties.ComboBox; }
		}
		protected internal ComboBoxProperties ComboBoxProperties {
			get { return Owner as ComboBoxProperties; }
		}
		protected override bool UseParentStylesOnly {
			get { return true; }
		}
		protected override bool UseParentImagesOnly {
			get { return true; }
		}
		internal override ASPxWebControl GetOwnerControl() {
			return (ASPxWebControl)AutoCompleteBox;
		}
		protected override bool IsClientSideEventsStoreToViewState() {
			return false;
		}
		protected override bool IsImagesStoreToViewState() {
			return false;
		}
		protected override bool IsStylesStoreToViewState() {
			return false;
		}
		protected internal override ListEditLoadOnDemandStrategyBase CreateLoadOnDemandStrategy() {
			if(ComboBox != null && ComboBox.DataRequestedEventHandlerAssigned)
				return new ListEditLoadOnDemandCustomEventStrategy(ComboBox.ItemRequestEventHelper);
			else if(CallbackModeEnabled && !IsDesignMode())
				return new ListEditLoadOnDemandInternalStrategy(AutoCompleteBoxProperties);
			else
				return new ListEditDisabledLoadOnDemandStrategy(this);
		}
		protected override void RaiseItemDeleting(ASPxDataDeletingEventArgs e) {
			AutoCompleteBox.RaiseItemDeleting(e);
		}
		protected override void RaiseItemDeleted(ASPxDataDeletedEventArgs e) {
			AutoCompleteBox.RaiseItemDeleted(e);
		}
		protected override void RaiseItemInserting(ASPxDataInsertingEventArgs e) {
			AutoCompleteBox.RaiseItemInserting(e);
		}
		protected override void RaiseItemInserted(ASPxDataInsertedEventArgs e) {
			AutoCompleteBox.RaiseItemInserted(e);
		}
		protected internal override bool? GetItemSelected(ListEditItem item) {
			if(ComboBox != null)
				return ComboBox.GetItemSelected(item);
			return null;
		}
		protected internal override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(ComboBox != null)
				ComboBox.OnItemSelectionChanged(item, selected);
		}
		int IListEditItemsRequester.SelectedIndex {
			get {
				if(ComboBox != null)
					return ComboBox.SelectedIndex;
				return -1;
			}
		}
		Page IListBoxRenderHelperOwner.Page {
			get {
				if(AutoCompleteBox != null)
					return AutoCompleteBox.Page;
				return null;
			}
		}
		bool IListBoxRenderHelperOwner.IsClientSideAPIEnabledInternal {
			get { return AutoCompleteBox.IsClientSideAPIEnabled(); }
		}
		protected internal override object ResolveClientUrl(string url) {
			if(AutoCompleteBox != null)
				return AutoCompleteBox.ResolveClientUrl(url);
			return url;
		}
	}
	public class ComboBoxProperties : AutoCompleteBoxPropertiesBase, IListBoxColumnsOwner, IListEditItemsRequester {
		private readonly object itemsRequestedByFilterEventLock = new object();
		private readonly object itemRequestedByValueEventLock = new object();
		private static readonly object itemsRequestedByFilterKeyObject = new object();
		private static readonly object itemRequestedByValueKeyObject = new object();
		ListEditItemsRequestedByFilterConditionEventHandler itemsRequestedByFilterEvent;
		ListEditItemRequestedByValueEventHandler itemRequestedByValueEvent;
		public ComboBoxProperties()
			: base() {
		}
		public ComboBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool AllowNull {
			get { return GetBoolProperty("AllowNull", false); }
			set { SetBoolProperty("AllowNull", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesColumns"),
#endif
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), NotifyParentProperty(true),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		Editor("DevExpress.Web.Design.ColumnsPropertiesCommonEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ListBoxColumnCollection Columns {
			get { return ColumnsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new ComboBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as ComboBoxClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesDropDownStyle"),
#endif
		Category("Styles"), DefaultValue(DropDownStyle.DropDownList), NotifyParentProperty(true), AutoFormatDisable]
		public DropDownStyle DropDownStyle {
			get { return DropDownStyleInternal; }
			set { DropDownStyleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesNative"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesShowImageInEditBox"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable, Themeable(false)]
		public bool ShowImageInEditBox {
			get { return GetBoolProperty("ShowImageInEditBox", false); }
			set {
				SetBoolProperty("ShowImageInEditBox", false, value);
				LayoutChanged(); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesValueType"),
#endif
		DefaultValue(typeof(String)), NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return ValueTypeInternal; }
			set { ValueTypeInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesItemsRequestedByFilterCondition"),
#endif
		Category("Action")]
		public event ListEditItemsRequestedByFilterConditionEventHandler ItemsRequestedByFilterCondition
		{
			add { lock (itemsRequestedByFilterEventLock) { itemsRequestedByFilterEvent += value; } }
			remove { lock (itemsRequestedByFilterEventLock) { itemsRequestedByFilterEvent -= value; } }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxPropertiesItemRequestedByValue"),
#endif
		Category("Action")]
		public event ListEditItemRequestedByValueEventHandler ItemRequestedByValue
		{
			add { lock (itemRequestedByValueEventLock) { itemRequestedByValueEvent += value; } }
			remove { lock (itemRequestedByValueEventLock) { itemRequestedByValueEvent -= value; } }
		}
		protected internal ASPxComboBox ComboBox {
			get { return AutoCompleteBox as ASPxComboBox; }
		}
		protected internal object ItemsRequestedByFilterConditionKeyObject {
			get { return itemsRequestedByFilterKeyObject; }
		}
		protected internal object ItemRequestedByValueKeyObject {
			get { return itemRequestedByValueKeyObject; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ComboBoxProperties src = source as ComboBoxProperties;
				if(src != null) {
					ShowImageInEditBox = src.ShowImageInEditBox;
					AllowNull = src.AllowNull;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ComboBoxClientSideEvents(this);
		}
		protected override void AssignEditorProperties(ASPxEditBase edit) {
			base.AssignEditorProperties(edit);
			((ASPxComboBox)edit).AssignRequestEventsFromProperties(itemsRequestedByFilterEvent, itemRequestedByValueEvent);
		}
		protected override void AssignInplaceProperties(CreateEditorArgs args) {
			base.AssignInplaceProperties(args);
			if(args.DataType != null && args.DataType != typeof(object))
				ValueType = args.DataType;
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxComboBox();
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			object convertedValue = ListEditPropertiesHelper.GetConvertedValue(args, ValueType);
			if(!string.IsNullOrEmpty(args.DisplayText) || convertedValue == null)
				return base.CreateDisplayControlInstance(args);
			else
				return ListEditPropertiesHelper.CreateDisplayControlInstance(args, convertedValue, ItemImage, DisplayImageSpacing);
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue))
				return base.GetDisplayTextCore(args, encode);
			else {
				CheckInplaceBound(args.DataType, args.Parent, args.DesignMode);
				RequestItemByValue(args);
				object convertedValue = ListEditPropertiesHelper.GetConvertedValue(args, ValueType);
				return ListEditPropertiesHelper.GetDisplayText(args, encode, convertedValue);
			}
		}
		protected override internal string OnSpecialCallback(AutoCompleteBoxCallbackArgumentsReader argumentsReader) {
			if(((ComboBoxCallbackArgumentsReader)argumentsReader).IsLoadDropDownOnDemandCallback) {
				return base.OnSpecialCallback(argumentsReader);
			} else
				return SerializingHelper.SerializeItemsRange(Items, ComboBox.SelectedIndex, ComboBox.SelectedIndex);
		}
		protected bool IsSelectingCallback(ComboBoxCallbackArgumentsReader argumentsReader) {
			return argumentsReader.IsCurrentSelectedItemCallback;
		}
		protected string OnSelectingCallback(ComboBoxCallbackArgumentsReader argumentsReader) {
			return SerializingHelper.SerializeItemsRange(Items, ComboBox.SelectedIndex, ComboBox.SelectedIndex, true);
		}
		protected internal override string OnCallback(string eventArgument) {
			ComboBoxCallbackArgumentsReader argumentsReader = new ComboBoxCallbackArgumentsReader(eventArgument);
			if(IsSpecialCallback(argumentsReader))
				return OnSpecialCallback(argumentsReader);
			else if(IsSelectingCallback(argumentsReader))
				return OnSelectingCallback(argumentsReader);
			else
				return OnRegularCallback(argumentsReader);
		}
		protected void RequestItemByValue(CreateDisplayControlArgs args) {
			if(args.Parent != null && itemRequestedByValueEvent != null && itemsRequestedByFilterEvent != null) {
				CreateEditControlArgs emptyArgs = new CreateEditControlArgs(null, ValueType, null, null, null, EditorInplaceMode.Inplace, true);
				ASPxComboBox comboBox = (ASPxComboBox)CreateEdit(emptyArgs, true);
				args.Parent.Controls.Add(comboBox);
				comboBox.AssignRequestEventsFromProperties(itemsRequestedByFilterEvent, itemRequestedByValueEvent);
				comboBox.ItemRequestEventHelper.RequestItemByValue(args.EditValue);
				args.Parent.Controls.Remove(comboBox);
				AssignItems(comboBox.Items);
			}
		}
		protected internal override bool IsClearButtonVisibleAuto() {
			return DropDownStyle != DropDownStyle.DropDownList && base.IsClearButtonVisibleAuto();
		}
		protected override bool IsNativeSupported() {
			return true;
		}
		int IListEditItemsRequester.SelectedIndex {
			get { return ((IListEditItemsRequester)ListBoxProperties).SelectedIndex; }
		}
		protected internal override ASPxEditBase CreateEdit(CreateEditControlArgs args, bool isInternal, Func<ASPxEditBase> createEditInstance) {
			ASPxEditBase control = base.CreateEdit(args, isInternal, createEditInstance);
			ASPxComboBox comboBox = control as ASPxComboBox;
			if(comboBox != null) {
				if(isInternal) {
					comboBox.DataSource = null;
					comboBox.DataSourceID = "";
				} else {
					if(comboBox.DataRequestedEventHandlerAssigned && comboBox.SelectedIndex < 0)
						comboBox.ItemRequestEventHelper.RequestItemByValue(comboBox.Value);
					if(comboBox.Value == null && comboBox.SelectedItem == null)
						comboBox.LoadFirstItemsPage();
				}
			}
			return control;
		}
	}
	public class AutoCompleteBoxCallbackArgumentsReader : ListBoxCallbackArgumentsReader {
		public const string LoadFilteringItemsCallbackPrefix = "CBLF";
		public const string CorrectFilterCallbackPrefix = "CBCF";
		public const string LoadDropDownOnDemandCallbackPrefix = "CBLD";
		public AutoCompleteBoxCallbackArgumentsReader(string arguments)
			: this(arguments, new string[] { }) {
		}
		public AutoCompleteBoxCallbackArgumentsReader(string arguments, string[] prefixes)
			: base(arguments, ListEditCallbackArgumentsReader.GetMergedStringArray(new string[] { LoadFilteringItemsCallbackPrefix, CorrectFilterCallbackPrefix, LoadDropDownOnDemandCallbackPrefix }, prefixes)) {
		}
		public bool IsLoadFilteringItemsCallback { get { return !string.IsNullOrEmpty(Filter); } }
		public bool IsFilterCorrectionCallback { get { return !string.IsNullOrEmpty(FilterForCorrection); } }
		public bool IsLoadDropDownOnDemandCallback { get { return LoadDropDownOnDemandRequest != null; } }
		public string Filter { get { return this[AutoCompleteBoxCallbackArgumentsReader.LoadFilteringItemsCallbackPrefix]; } }
		public string FilterForCorrection { get { return this[AutoCompleteBoxCallbackArgumentsReader.CorrectFilterCallbackPrefix]; } }
		public string LoadDropDownOnDemandRequest { get { return this[AutoCompleteBoxCallbackArgumentsReader.LoadDropDownOnDemandCallbackPrefix]; } }
	}
	public class ComboBoxCallbackArgumentsReader : AutoCompleteBoxCallbackArgumentsReader {
		public const string CurrentSelectedItemCallbackPrefix = "CBSI";
		public ComboBoxCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] { CurrentSelectedItemCallbackPrefix }) {
		}
		public bool IsCurrentSelectedItemCallback { get { return CurrentSelectedItemRequest != null; } }
		public string CurrentSelectedItemRequest { get { return this[ComboBoxCallbackArgumentsReader.CurrentSelectedItemCallbackPrefix]; } }
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("SelectedIndex"), DefaultEvent("SelectedIndexChanged"),
	ToolboxData("<{0}:ASPxComboBox runat=\"server\" ValueType=\"System.String\"></{0}:ASPxComboBox>"),
	Designer("DevExpress.Web.Design.ASPxComboBoxDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxComboBox.bmp")
	]
	public class ASPxComboBox : ASPxAutoCompleteBoxBase, IListBoxColumnsOwner, IValueTypeHolder, IControlDesigner {
		protected internal const string ComboBoxScriptResourceName = EditScriptsResourcePath + "ComboBox.js";
		private static readonly object EventSelectedIndexChanged = new object();
		private bool isInapplicableClientSelection = false;
		private bool selectionRestoringRequired = false;
		private int restoreSelectionLockCount = 0;
		ComboBoxItemRequestEventHelper itemRequestEventHelper;
		private ClientSelection clientSelection;
		class ClientSelection {
			object value;
			string stringValue;
			string text;
			public ClientSelection(object value, string stringValue, string text) {
				this.value = value;
				this.stringValue = stringValue;
				this.text = text;
			}
			public object Value { get { return value; } }
			public string StringValue { get { return stringValue; } }
			public string Text { get { return text; } }
		}
		internal const string ValueHiddenInputID = "VI";
		public ASPxComboBox()
			: base() {
		}
		protected ASPxComboBox(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected internal ASPxComboBox(ASPxWebControl ownerControl, ComboBoxProperties properties)
			: base(ownerControl, properties) {
		}
		[Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return Properties.AllowNull; }
			set { Properties.AllowNull = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new ComboBoxClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxDropDownStyle"),
#endif
		DefaultValue(DropDownStyle.DropDownList), AutoFormatDisable]
		public DropDownStyle DropDownStyle {
			get { return DropDownStyleInternal; }
			set { DropDownStyleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxColumns"),
#endif
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public ListBoxColumnCollection Columns {
			get { return ColumnsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxNative"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxSelectedIndex"),
#endif
		DefaultValue(-1), Bindable(true), AutoFormatDisable, Themeable(false)]
		public int SelectedIndex {
			get {
				TryToRestoreSelection();
				return IsInapplicableClientSelection ? -1 : ListEditHelper.GetSelectedIndex(ConvertEmptyStringToNull);
			}
			set {
				SelectionRestoringRequired = false;
				IsInapplicableClientSelection = false;
				ListEditHelper.SetSelectedIndex(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ListEditItem SelectedItem {
			get {
				TryToRestoreSelection();
				return IsInapplicableClientSelection ? null : ListEditHelper.GetSelectedItem(ConvertEmptyStringToNull);
			}
			set {
				SelectionRestoringRequired = false;
				IsInapplicableClientSelection = false;
				ListEditHelper.SetSelectedItem(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxShowImageInEditBox"),
#endif
		Category("Appearance"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public bool ShowImageInEditBox {
			get { return Properties.ShowImageInEditBox; }
			set { Properties.ShowImageInEditBox = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxValueType"),
#endif
		DefaultValue(typeof(String)), NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return Properties.ValueTypeInternal; }
			set { Properties.ValueTypeInternal = value; }
		}
		[DefaultValue(""), AutoFormatDisable, Themeable(false), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public override string Text {
			get {
				ListEditItem item = SelectedItem;
				if(SelectionRestoringRequired && clientSelection != null) {
					if(string.IsNullOrEmpty(clientSelection.StringValue) && !string.IsNullOrEmpty(NullText) && clientSelection.Text == NullText) 
						return string.Empty;
					return clientSelection.Text;
				}
				return (item != null) ? item.Text : base.Text;
			}
			set {
				ListEditItem item = FindItemByText(value);
				base.Value = item == null ? value : item.Value;
				ResetClientValueAndText();
				item = FindItemByClientValue(base.Value);
				IsInapplicableClientSelection = item != null && !string.Equals(item.Text, value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxComboBoxValue")]
#endif
		public override object Value {
			get {
				if(SelectionRestoringRequired && clientSelection != null)
					return GetClientValue();
				return base.Value;
			}
			set {
				bool LoadPostDataSetsNotSelectedValue = IsLoadingPostData && SelectionRestoringRequired &&
					CommonUtils.AreEqual(ClientValueInternal, value, Properties.ConvertEmptyStringToNull);
				if(!LoadPostDataSetsNotSelectedValue) {
					if(Items.FindByValue(value) != null)
						SelectionRestoringRequired = false;
					ResetClientValueAndText();
					IsInapplicableClientSelection = false;
				}
				base.Value = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable,
		Obsolete()]
		public object ClientValue { get { return ClientValueInternal; } }
		protected object ClientValueInternal { get { return clientSelection != null ? clientSelection.Value : null; } }
		protected internal bool AllowNullInternal {
			get {
				return DropDownStyle != DropDownStyle.DropDownList ||
					   Properties.IsClearButtonVisible() ||
					   DropDownStyle == DropDownStyle.DropDownList && AllowNull;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxSelectedIndexChanged"),
#endif
		Category("Action")]
		public event EventHandler SelectedIndexChanged
		{
			add { Events.AddHandler(EventSelectedIndexChanged, value); }
			remove { Events.RemoveHandler(EventSelectedIndexChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxItemsRequestedByFilterCondition"),
#endif
		Category("Action")]
		public event ListEditItemsRequestedByFilterConditionEventHandler ItemsRequestedByFilterCondition
		{
			add { Events.AddHandler(Properties.ItemsRequestedByFilterConditionKeyObject, value); }
			remove { Events.RemoveHandler(Properties.ItemsRequestedByFilterConditionKeyObject, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxComboBoxItemRequestedByValue"),
#endif
		Category("Action")]
		public event ListEditItemRequestedByValueEventHandler ItemRequestedByValue
		{
			add { Events.AddHandler(Properties.ItemRequestedByValueKeyObject, value); }
			remove { Events.RemoveHandler(Properties.ItemRequestedByValueKeyObject, value); }
		}
		protected new ComboBoxControl DropDownControl {
			get { return base.DropDownControl as ComboBoxControl; }
		}
		protected internal override bool IsNeedItemImageCell {
			get { return ShowImageInEditBox; }
		}
		protected internal bool IsSelectedItemHasImage {
			get { return SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.ImageUrl); }
		}
		protected bool SelectionRestoringRequired {
			get { return selectionRestoringRequired; }
			set { selectionRestoringRequired = value; }
		}
		protected bool IsInapplicableClientSelection {
			get { return isInapplicableClientSelection; }
			set { isInapplicableClientSelection = value; }
		}
		protected internal new ComboBoxProperties Properties {
			get { return (ComboBoxProperties)base.Properties; }
		}
		protected internal ComboBoxItemRequestEventHelper ItemRequestEventHelper {
			get {
				if(itemRequestEventHelper == null)
					itemRequestEventHelper = new ComboBoxItemRequestEventHelper(this);
				return itemRequestEventHelper;
			}
		}
		protected ListEditItem FindItemByText(string value) {
			ListEditItem item = Items.FindByText(value);
			if(item == null && Browser.IsIE && value != null)
				item = Items.FindByTextWithTrim(value);
			return item;
		}
		protected ListEditItem FindItemByClientValue(object value) {
			return Items.FindByValue(value);
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			if(InplaceMode == EditorInplaceMode.StandAlone)
				ListEditHelper.RestoreSelectedIndex();
		}
		protected internal bool IsImageVisible() {
			bool defaultImageHasSrc = !string.IsNullOrEmpty(ItemImage.Url);
			return IsSelectedItemHasImage || defaultImageHasSrc;
		}
		protected internal ImageProperties GetImage() {
			ImageProperties imageProperties = IsSelectedItemHasImage ?
				GetItemImage(SelectedItem, false) : new ImageProperties();
			imageProperties.MergeWith(ItemImage);
			if(!IsImageVisible())
				imageProperties.Url = EmptyImageProperties.GetGlobalEmptyImage(Page).Url;
			return imageProperties;
		}
		protected override void CreateDropDownControlHierarchy() {
			if(IsNativeRender())
				Controls.Add(new ComboBoxNativeControl(this));
			else
				base.CreateDropDownControlHierarchy();
		}
		protected override EditPropertiesBase CreatePropertiesInternal() {
			return new ComboBoxProperties(this);
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new ComboBoxControl(this);
		}
		protected internal override string GetInputText() {
			string text = base.GetInputText();
			if(SelectedItem != null && string.IsNullOrEmpty(SelectedItem.Text))
				return "";
			text = GetPlainText(text);
			return ListEditRenderUtils.ProtectTabs(text);
		}
		protected string GetPlainText(string text) {
			string plainText = text;
			plainText = plainText.Replace("\r\n", " ");
			plainText = plainText.Replace((char)160, (char)32);
			if(IsClientDecodingChangeSourceText(plainText))
				plainText = DecodeText(plainText);
			return plainText;
		}
		protected bool IsClientDecodingChangeSourceText(string text) {
			 if(!EncodeHtml) {
				 return text != DecodeText(text);
			 }
			return false;
		}
		protected string DecodeText(string text) {
			return System.Text.RegularExpressions.Regex.Replace(text.Replace("&nbsp;", string.Empty), "<.*?>", string.Empty);;
		}
		protected internal string GetValueHiddenInputClientID() {
			return ClientID + "_" + ValueHiddenInputID;
		}
		protected override string GetFormattedInputText() {
			if(String.IsNullOrEmpty(Text))
				return String.Empty;
			string format = CommonUtils.GetFormatString(DisplayFormatString);
			if(SelectedItem != null) {
				if(IsMultiColumn) {
					object[] texts = SelectedItem.GetVisibleColumnValues();
					for(int i = 0; i < texts.Length; i++) {
						string text = texts[i] == null ? "" : texts[i].ToString();
						texts[i] = AdjustArgTypeForFormatString(text);
					}
					return String.Format(format, texts);
				} else
					return String.Format(format, AdjustArgTypeForFormatString(Text));
			}
			return Text;
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPxClientNativeComboBox" : "ASPxClientComboBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxComboBox), ComboBoxScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(IsNativeRender() && SelectedIndex != -1)
				stb.AppendFormat("{0}.initSelectedIndex = {1};\n", localVarName, HtmlConvertor.ToScript(SelectedIndex));
			if(AllowNullInternal)
				stb.AppendFormat("{0}.allowNull = true;\n", localVarName);
		}
		protected override bool IsStandaloneScriptEnable() {
			return IsEncodingScriptRequired();
		}
		protected bool IsEncodingScriptRequired() {
			return !Enabled && !EncodeHtml && !Native && !HasClientInitialization();
		}
		protected override string CreateStandaloneScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendLine("var cbInputElement = document.getElementById('" + this.ID + "_" + ASPxTextEdit.InputControlSuffix + "');");
			stb.AppendLine("if (cbInputElement != null) {");
			stb.AppendLine("    var container = document.createElement('div');");
			stb.AppendLine("    container.innerHTML = cbInputElement.value;");
			stb.AppendLine("    cbInputElement.value = ASPx.GetInnerText(container);");
			stb.AppendLine("}");
			return stb.ToString();
		}
		protected override object GetPostBackValue(string key, NameValueCollection postCollection) {
			ListEditHelper.ResetStoredSelectedIndex();
			if(IsNativeRender()) {
				if(!string.IsNullOrEmpty(postCollection[UniqueID])) {
					int index;
					if(int.TryParse(postCollection[UniqueID], out index))
						return (0 <= index && index < Items.Count) ? Items[index].Value : null;
				}
				return null;
			} else {
				string text = postCollection[key];
				string encodedText = "";
				bool isNeedToEncode = false;
				if(!EncodeHtml) {
					encodedText = HttpUtility.HtmlEncode(text);
					isNeedToEncode = (FindItemByText(encodedText) != null);
				}
				string clientText = isNeedToEncode ? encodedText : text;
				string clientValue = postCollection[GetValueHiddenInputClientID()];
				TryToRestoreSelectionFromClientValueAndText(clientValue, clientText);
				if(DataSecurityMode == DataSecurityMode.Strict) {
					bool clientValueAndTextInvalid = Items.FindByValue(clientValue) == null;
					if(clientValueAndTextInvalid) {
						InvalidateClientValueAndText();
						TryToRestoreSelectionFromValue(Value);
					}
				}
				TryToRestoreSelection();
				return Value;
			}
		}
		protected void InvalidateClientValueAndText() {
			SelectionRestoringRequired = false;
			ResetClientValueAndText();
		}
		protected void TryToRestoreSelectionFromClientValueAndText(string clienValue, string clientText) {
			SaveClientValueAndText(clienValue, clientText);
			TryToRestoreSelectionFromClientValueCore();
			if(clientSelection != null) { 
				ListEditItem item = FindItemByClientText(clientSelection.Text);
				SelectionRestoringRequired = item == null;
			}
		}
		protected ListEditItem FindItemByClientText(string clientText) {
			ListEditItem item = FindItemByText(clientText);
			if(item != null) {
				string itemsValueString = item.Value != null ? item.Value.ToString() : string.Empty;
				if(!string.Equals(itemsValueString, clientSelection.StringValue))
					return null;
			}
			return item;
		}
		protected void SaveClientValueAndText(string clientValue, string clientText) {
			object convertedClientValue = clientText;
			try {
				convertedClientValue = CommonUtils.GetConvertedArgumentValue(clientValue, ValueType, "ClientValue");
			} catch {
			} finally {
				this.clientSelection = new ClientSelection(convertedClientValue, clientValue, clientText);
			}
		}
		protected void ResetClientValueAndText() {
			this.clientSelection = null;
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnSelectedIndexChanged(EventArgs.Empty);
		}
		protected object GetClientValue() {
			if(ConvertEmptyStringToNull && string.Equals(clientSelection.Value, string.Empty))
				return null;
			return clientSelection.Value;
		}
		protected void TryToRestoreSelection() {
			if(SelectionRestoringRequired && this.restoreSelectionLockCount == 0) {
				this.restoreSelectionLockCount++;
				TryToRestoreSelectionFromClientValue();
				TryToRestoreSelectionFromClientText();
				TryToRestoreSelectionFromText();
				if(SelectionRestoringRequired && SelectedItem != null)
					RestoreSelectedItem(SelectedItem);
				this.restoreSelectionLockCount--;
			}
		}
		protected void TryToRestoreSelectionFromClientValue() {
			if (SelectedItem != null || this.clientSelection == null)
				return;
			TryToRestoreSelectionFromClientValueCore();
		}
		protected void TryToRestoreSelectionFromClientText() {
			if (SelectedItem != null || this.clientSelection == null)
				return;
			ListEditItem item = null;
			item = FindItemByClientText(this.clientSelection.Text);
			RestoreSelectedItem(item);
		}
		protected void TryToRestoreSelectionFromText() {
			if (SelectedItem != null || this.clientSelection != null)
				return;
			ListEditItem item;
			item = FindItemByText(Text);
			RestoreSelectedItem(item);
		}
		protected void TryToRestoreSelectionFromClientValueCore() {
			TryToRestoreSelectionFromValue(ClientValueInternal);
		}
		protected void TryToRestoreSelectionFromValue(object value) {
			try {
				IsInapplicableClientSelection = false;
				ListEditItem item = FindItemByClientValue(value);
				if(item == null) {
					ItemRequestEventHelper.RequestItemByValueIfWasNotRequestedBefore(value);
					item = FindItemByClientValue(value);
				}
				if(item != null) {
					string itemText = "";
					if(!string.IsNullOrEmpty(DisplayFormatString)) {
						string format = CommonUtils.GetFormatString(DisplayFormatString);
						if(IsMultiColumn) {
							object[] texts = item.GetVisibleColumnValues();
							for(int i = 0; i < texts.Length; i++) {
								string text = texts[i] == null ? "" : texts[i].ToString();
								texts[i] = AdjustArgTypeForFormatString(text);
							}
							itemText = String.Format(format, texts);
						} else
							itemText = String.Format(format, AdjustArgTypeForFormatString(item.Text));
						if(IsCallback && string.Equals(System.Text.RegularExpressions.Regex.Replace(DisplayFormatString, @"{\d+}", ""), clientSelection.Text) ||
							string.Equals(item.Text, clientSelection.Text)) {
							itemText = clientSelection.Text;
						}
					} else
						itemText = item.Text;
					bool isNeedToDecode = FindItemByText(clientSelection.Text) == null;
					itemText = !EncodeHtml && isNeedToDecode ? HttpUtility.HtmlDecode(itemText) : itemText;
					itemText = GetPlainText(itemText);
					if(clientSelection != null && !string.Equals(itemText, clientSelection.Text)) {
						IsInapplicableClientSelection = true;
						item = null;
					}
				}
				RestoreSelectedItem(item);
			} catch { }
		}
		protected void RestoreSelectedItem(ListEditItem item) {
			if(item != null) {
				SelectionRestoringRequired = false;
				ResetClientValueAndText();
				base.Value = item.Value;
			}
		}
		protected override bool NotValueChangingRequest() {
			return IsCallback && IsLoadRangeItemsCallbackWithoutCustomProcessing();
		}
		private bool IsLoadRangeItemsCallbackWithoutCustomProcessing() {
			int callbackID = -1;
			string callbackArgument = PostDataCollection[GetCallbackParamName()];
			ProcessCallbackArgumentCore(ref callbackID, ref callbackArgument);
			ComboBoxCallbackArgumentsReader argumentsReader = new ComboBoxCallbackArgumentsReader(callbackArgument);
			return argumentsReader.IsLoadRangeItemsCallback && !argumentsReader.IsCustomCallback;
		}
		protected virtual string GetCallbackParamName() {
			return RenderUtils.CallbackControlParamParamName;
		}
		protected virtual void OnSelectedIndexChanged(EventArgs e) {
			EventHandler handler = Events[EventSelectedIndexChanged] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal bool DataRequestedEventHandlerAssigned {
			get { return Events[Properties.ItemsRequestedByFilterConditionKeyObject] != null && Events[Properties.ItemRequestedByValueKeyObject] != null; }
		}
		protected internal void RaiseItemsRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e) {
			ListEditItemsRequestedByFilterConditionEventHandler handler = (ListEditItemsRequestedByFilterConditionEventHandler)Events[Properties.ItemsRequestedByFilterConditionKeyObject];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemRequestedByValue(ListEditItemRequestedByValueEventArgs e) {
			ListEditItemRequestedByValueEventHandler handler = (ListEditItemRequestedByValueEventHandler)Events[Properties.ItemRequestedByValueKeyObject];
			if(handler != null)
				handler(this, e);
		}
		protected internal void AssignRequestEventsFromProperties(ListEditItemsRequestedByFilterConditionEventHandler itemsRequestedByFilterCondition,
			ListEditItemRequestedByValueEventHandler itemRequestedByValue) {
			ItemRequestEventHelper.AssignRequestEventsFromProperties(itemsRequestedByFilterCondition, itemRequestedByValue);
		}
		protected internal bool? GetItemSelected(ListEditItem item) {
			return IsInapplicableClientSelection ? null : ListEditHelper.GetItemSelected(item, ConvertEmptyStringToNull);
		}
		protected internal void OnItemSelectionChanged(ListEditItem item, bool selected) {
			ListEditHelper.OnItemSelectionChanged(item, selected);
		}
		public override void DataBind() {
			base.DataBind();
			if(Items.IsEmpty || (SelectedItem == null && Items.FindByValue(Value) == null))
				ItemRequestEventHelper.RequestItemByValue(Value);
		}
		protected override void EnsurePreRender() {
			if(!IsCallback) {
				if(Value != null && SelectedItem == null)
					ItemRequestEventHelper.RequestItemByValueIfWasNotRequestedBefore(Value);
				else if(!LoadDropDownOnDemand)
					LoadFirstItemsPageIfRequired();
			}
			base.EnsurePreRender();
		}
		protected void LoadFirstItemsPageIfRequired() {
			if(Items.Count == 0)
				LoadFirstItemsPage();
		}
		protected internal void LoadFirstItemsPage() {
			int endIndex = CallbackPageSize - 1;
			if(!EnableCallbackMode && LoadDropDownOnDemand)
				endIndex = -1;
			ItemRequestEventHelper.RequestItemsPageWithFilter(string.Empty, 0, endIndex);
		}
		protected internal override string GetLoadDropDownOnDemandRender(string args) {
			LoadFirstItemsPageIfRequired();
			return base.GetLoadDropDownOnDemandRender(args);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ComboBoxCommonFormDesigner"; } }
	}
}
namespace DevExpress.Web.Internal {
	public interface IListEditItemsRequester {
		ListEditItemCollection Items { get; }
		int SelectedIndex { get; }
		int CallbackPageSize { get; }
	}
	public interface IListEditItemsRequestEventOwner : IListEditItemsRequester {
		void RequestItemsPageWithFilter(string filter, int beginIndex, int endIndex);
		void RequestItemByValue(object value);
	}
	public class ComboBoxItemRequestEventHelper : IListEditItemsRequestEventOwner {
		ASPxComboBox comboBox;
		int requestEventsLockCount = 0;
		bool RequestEvensLocked { get { return requestEventsLockCount != 0; } }
		void LockRequestEvent() { this.requestEventsLockCount++; }
		void UnlockRequestEvent() { this.requestEventsLockCount--; }
		bool eventsHaveAlreadyAssigned = false;
		Hashtable cachedRequestedValues = new Hashtable();
		readonly object nothingKey = new object();
		public ComboBoxItemRequestEventHelper(ASPxComboBox comboBox) {
			this.comboBox = comboBox;
		}
		public ListEditItemCollection Items { get { return ComboBox.Items; } }
		public int SelectedIndex { get { return ComboBox.SelectedIndex; } }
		public int CallbackPageSize { get { return ComboBox.CallbackPageSize; } }
		ASPxComboBox ComboBox { get { return comboBox; } }
		public void RequestItemsPageWithFilter(string filter, int beginIndex, int endIndex) {
			if(!RequestEvensLocked) {
				LockRequestEvent();
				ListEditItemsRequestedByFilterConditionEventArgs e = new ListEditItemsRequestedByFilterConditionEventArgs(beginIndex, endIndex, filter);
				ComboBox.RaiseItemsRequestedByFilterCondition(e);
				UnlockRequestEvent();
			}
		}
		public void RequestItemByValue(object value) {
			if(!RequestEvensLocked) {
				MemorizeLastRequestedValue(value);
				LockRequestEvent();
				ListEditItemRequestedByValueEventArgs e = new ListEditItemRequestedByValueEventArgs(value);
				ComboBox.RaiseItemRequestedByValue(e);
				UnlockRequestEvent();
				MemorizeLastRequestedItem(value);
			}
		}
		public void RequestItemByValueIfWasNotRequestedBefore(object value) {
			if(!ValueHasBeenAlreadyRequested(value))
				RequestItemByValue(value);
			else {
				ListEditItem cachedItem = this.cachedRequestedValues[GetKeyForCache(value)]  as ListEditItem;
				if(Items.FindByValue(value) == null && cachedItem != null) {
					Items.Clear();
					Items.Add(cachedItem);
				}
			}
		}
		public void AssignRequestEventsFromProperties(ListEditItemsRequestedByFilterConditionEventHandler itemsRequestedByFilterCondition,
			ListEditItemRequestedByValueEventHandler itemRequestedByValue) {
			if(!eventsHaveAlreadyAssigned) {
				ComboBox.ItemsRequestedByFilterCondition += itemsRequestedByFilterCondition;
				ComboBox.ItemRequestedByValue += itemRequestedByValue;
				eventsHaveAlreadyAssigned = true;
			}
		}
		void MemorizeLastRequestedValue(object value) {
			if(!ValueHasBeenAlreadyRequested(value))
				this.cachedRequestedValues.Add(GetKeyForCache(value), null);
		}
		void MemorizeLastRequestedItem(object value) {
			if(ValueHasBeenAlreadyRequested(value))
				this.cachedRequestedValues[GetKeyForCache(value)] = Items.FindByValue(value);
		}
		bool ValueHasBeenAlreadyRequested(object value) {
			return this.cachedRequestedValues.ContainsKey(GetKeyForCache(value));
		}
		object GetKeyForCache(object key) {
			return key == null ? nothingKey : key;
		}
	}
	public abstract class ListEditLoadOnDemandStrategyBase {
		IListEditItemsRequester owner;
		public enum FilteringMode { OnlyFullCoincideWithFilterAllowed, FilterOutItemsWithMaxCoincide, None }
		public ListEditLoadOnDemandStrategyBase(IListEditItemsRequester owner) {
			this.owner = owner;
		}
		public abstract bool PossibleLoadItemsToTop { get; }
		public abstract bool PossibleLoadItemsToBottom { get; }
		public abstract int FirstVisibleItemIndex { get; }
		protected IListEditItemsRequester Owner { get { return owner; } }
		protected ListEditItemCollection Items { get { return Owner.Items; } }
		protected int CallbackPageSize { get { return Owner.CallbackPageSize; } }
		protected int SelectedIndex { get { return Owner.SelectedIndex; } }
		public string GetSerializedItems(IListEditItemSerializer itemsSerializer, string filter, FilteringMode filtrationMode, IncrementalFilteringMode filteringMode,
			int beginIndex, int endIndex) {
			return GetSerializedItems(itemsSerializer, filter, filtrationMode, filteringMode, beginIndex, endIndex, false);
		}
		public abstract string GetSerializedItems(IListEditItemSerializer itemsSerializer, string filter, FilteringMode filtrationMode, IncrementalFilteringMode filteringMode,
			int beginIndex, int endIndex, bool needSelection);
		public abstract ListEditItemCollection GetVisibleItems();
		protected string GetSerializedItemsCore(IListEditItemSerializer itemsSerializer,
			ListEditItemCollection items, int beginIndex, int endIndex, bool needSelection) {
			return itemsSerializer.SerializeItemsRange(items, beginIndex, endIndex, needSelection);
		}
	}
	public class ListEditDisabledLoadOnDemandStrategy : ListEditLoadOnDemandStrategyBase {
		public ListEditDisabledLoadOnDemandStrategy(IListEditItemsRequester owner)
			: base(owner) {
		}
		public override bool PossibleLoadItemsToTop { get { return false; } }
		public override bool PossibleLoadItemsToBottom { get { return false; } }
		public override int FirstVisibleItemIndex { get { return 0; } }
		public override string GetSerializedItems(IListEditItemSerializer itemsSerializer, string filter, FilteringMode filtrationMode, IncrementalFilteringMode filteringMode,
			int beginIndex, int endIndex, bool needSelection) {
			return GetSerializedItemsCore(itemsSerializer, Items, beginIndex, endIndex, needSelection);
		}
		public override ListEditItemCollection GetVisibleItems() {
			return Items;
		}
	}
	public class ListEditLoadOnDemandInternalStrategy : ListEditLoadOnDemandStrategyBase {
		public ListEditLoadOnDemandInternalStrategy(IListEditItemsRequester owner)
			: base(owner) {
		}
		public override bool PossibleLoadItemsToTop { get { return FirstVisibleItemIndex > 0; } }
		public override bool PossibleLoadItemsToBottom { get { return FirstVisibleItemIndex + CallbackPageSize < Items.Count; } }
		public override int FirstVisibleItemIndex {
			get {
				int serverIndexOfFirstItem = 0;
				if(SelectedIndex >= CallbackPageSize)
					serverIndexOfFirstItem = (SelectedIndex + CallbackPageSize < Items.Count) ? SelectedIndex : Items.Count - CallbackPageSize;
				return serverIndexOfFirstItem;
			}
		}
		public override string GetSerializedItems(IListEditItemSerializer itemsSerializer, string filter, FilteringMode filtrationMode, IncrementalFilteringMode filteringMode, int beginIndex, int endIndex, bool needSelection) {
			if(filtrationMode == FilteringMode.OnlyFullCoincideWithFilterAllowed)
				FilterOutItems(filter,  filteringMode);
			else if(filtrationMode == FilteringMode.FilterOutItemsWithMaxCoincide)
				FilterOutItemsWithMaxCoincide(filter, filteringMode);
			return GetSerializedItemsCore(itemsSerializer, Items, beginIndex, endIndex, needSelection);
		}
		public override ListEditItemCollection GetVisibleItems() {
			ListEditItemCollection vItems = Items.CreateEmptyClone();
			int firstVisibleItemIndex = FirstVisibleItemIndex;
			if(firstVisibleItemIndex < 0)
				firstVisibleItemIndex = 0;
			int lastVisibleItemIndex = firstVisibleItemIndex + CallbackPageSize - 1;
			if(lastVisibleItemIndex > Items.Count - 1)
				lastVisibleItemIndex = Items.Count - 1;
			ListEditItem currentItem = null;
			for(int i = firstVisibleItemIndex; i <= lastVisibleItemIndex; i++) {
				currentItem = Items[i].Clone();
				currentItem.LockValueConvertation();
				vItems.Add(currentItem);
				currentItem.UnlockValueConvertation();
			}
			return vItems;
		}
		protected bool IsSatisfy(string filter, string itemText, IncrementalFilteringMode filteringMode) {
			if(filteringMode == IncrementalFilteringMode.Contains)
				return itemText.ToLower().Contains(filter);
			else if(filteringMode == IncrementalFilteringMode.StartsWith)
				return itemText.ToLower().StartsWith(filter);
			throw new NotImplementedException();
		}
		protected void FilterOutItems(string filter, IncrementalFilteringMode filteringMode) {
			Items.BeginUpdate();
			filter = filter.ToLower();
			for(int i = Items.Count - 1; i >= 0; i--) {
				if(!IsSatisfy(filter, Items[i].Text, filteringMode))
					Items.RemoveAt(i);
			}
			Items.EndUpdate();
		}
		protected void FilterOutItemsWithMaxCoincide(string filter, IncrementalFilteringMode filteringMode) {
			Items.BeginUpdate();
			int[] coincide = new int[Items.Count];
			int maxCoincide = 0;
			filter = filter.ToLower();
			for(int i = Items.Count - 1; i >= 0; i--) {
				coincide[i] = GetCoincideCharCount(Items[i].Text, filter, filteringMode);
				if(coincide[i] > maxCoincide)
					maxCoincide = coincide[i];
			}
			for(int i = Items.Count - 1; i >= 0; i--) {
				if(coincide[i] < maxCoincide)
					Items.RemoveAt(i);
			}
			Items.EndUpdate();
		}
		protected int GetCoincideCharCount(string text, string filter, IncrementalFilteringMode filteringMode) {
			while(!IsSatisfy(filter, text, filteringMode)) {
				filter = filter.Remove(filter.Length - 1);
			}
			return filter.Length;
		}
	}
	public class ListEditLoadOnDemandCustomEventStrategy : ListEditLoadOnDemandStrategyBase {
		public ListEditLoadOnDemandCustomEventStrategy(IListEditItemsRequestEventOwner owner)
			: base(owner) {
		}
		public override bool PossibleLoadItemsToTop { get { return false; } }
		public override bool PossibleLoadItemsToBottom { get { return SelectedIndex == -1 || Items.Count == CallbackPageSize; } }
		public override int FirstVisibleItemIndex { get { return 0; } }
		protected new IListEditItemsRequestEventOwner Owner {
			get { return (IListEditItemsRequestEventOwner)base.Owner; }
		}
		public override string GetSerializedItems(IListEditItemSerializer itemsSerializer, string filter, FilteringMode filtrationMode, IncrementalFilteringMode filteringMode,
			int beginIndex, int endIndex, bool needSelection) {
			Owner.RequestItemsPageWithFilter(filter, beginIndex, endIndex);
			return GetSerializedItemsCore(itemsSerializer, Items, 0, endIndex - beginIndex, needSelection);
		}
		public override ListEditItemCollection GetVisibleItems() {
			return Items;
		}
	}
}
