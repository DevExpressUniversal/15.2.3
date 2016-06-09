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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using System.Collections.Specialized;
using System.Collections;
using System.Web.Script.Serialization;
using DevExpress.Data.IO;
using DevExpress.Web.Data;
using DevExpress.Utils;
using DevExpress.Web.Internal.InternalCheckBox;
using System.Security;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum DataSecurityMode { Default, Strict }
	public enum ListEditSelectionMode { Single, Multiple, CheckColumn };
	public class ListBoxProperties : ListEditProperties, IMulticolumnListEditDataSettings, IListEditItemsRequester, IWebColumnsOwner, IListBoxColumnsOwner, IListBoxRenderHelperOwner {
		private static readonly object eventCallback = new object();
		private static readonly object eventItemDeleting = new object();
		private static readonly object eventItemDeleted = new object();
		private static readonly object eventItemInserting = new object();
		private static readonly object eventItemInserted = new object();
		private string customCallback = "";
		private ListEditItemCollection visibleItems;
		private ListBoxColumnCollection columns;
		private ListBoxItemsSerializingHelper serializingHelper;
		IWebColumnsOwner webColumnsOwnerImpl;
		private ListBoxRenderHelper renderHelper;
		ListEditLoadOnDemandStrategyBase loadOnDemandStrategy;
		protected internal const int CallbackPageSizeDefault = 100;
		public ListBoxProperties()
			: this(null) {
		}
		public ListBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesCallbackPageSize"),
#endif
		DefaultValue(ListBoxProperties.CallbackPageSizeDefault), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public int CallbackPageSize {
			get { return GetIntProperty("CallbackPageSize", ListBoxProperties.CallbackPageSizeDefault); }
			set { SetIntProperty("CallbackPageSize", ListBoxProperties.CallbackPageSizeDefault, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesColumns"),
#endif
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), NotifyParentProperty(true),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public ListBoxColumnCollection Columns {
			get {
				if(columns == null)
					columns = new ListBoxColumnCollection(this);
				return columns;
			}
		}
		protected IWebColumnsOwner WebColumnsOwnerImpl {
			get {
				if(webColumnsOwnerImpl == null)
					webColumnsOwnerImpl = new WebColumnsOwnerDefaultImplementation(this, Columns);
				return webColumnsOwnerImpl;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesEnableCallbackMode"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public bool EnableCallbackMode {
			get { return GetBoolProperty("EnableCallbackMode", false); }
			set { 
				SetBoolProperty("EnableCallbackMode", false, value); 
				ResetOnDemandStrategy();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesEnableSynchronization"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DefaultBoolean EnableSynchronization {
			get { return GetDefaultBooleanProperty("EnableSynchronization", DefaultBoolean.Default); }
			set { SetDefaultBooleanProperty("EnableSynchronization", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesDataSecurityMode"),
#endif
		DefaultValue(DataSecurityMode.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DataSecurityMode DataSecurityMode {
			get { return (DataSecurityMode)GetEnumProperty("DataSecurityMode", DataSecurityMode.Default); }
			set { SetEnumProperty("DataSecurityMode", DataSecurityMode.Default, value); ; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesNative"),
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
	DevExpressWebLocalizedDescription("ListBoxPropertiesRows"),
#endif
		DefaultValue(5), NotifyParentProperty(true), AutoFormatDisable]
		public int Rows {
			get { return GetIntProperty("Rows", 5); }
			set {
				CommonUtils.CheckMinimumValue(value, 2, "Rows");
				SetIntProperty("Rows", 5, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesItemStyle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBoxItemStyle ItemStyle {
			get { return Styles.ListBoxItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesSelectionMode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, Themeable(false),
		DefaultValue(ListEditSelectionMode.Single)]
		public ListEditSelectionMode SelectionMode {
			get { return (ListEditSelectionMode)GetEnumProperty("ListSelectionMode", ListEditSelectionMode.Single); }
			set { 
				SetEnumProperty("ListSelectionMode", ListEditSelectionMode.Single, value);
				ResetOnDemandStrategy();
				if(ListBox != null)
					ListBox.OnSelectionModeChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatDisable, MergableProperty(false), NotifyParentProperty(true)]
		public new ListBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as ListBoxClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesCheckBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle CheckBox {
			get { return Styles.CheckBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesCheckBoxFocused"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle CheckBoxFocused {
			get { return Styles.CheckBoxFocused; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesCheckBoxChecked"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxChecked {
			get { return Images.CheckBoxChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxPropertiesCheckBoxUnchecked"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxUnchecked {
			get { return Images.CheckBoxUnchecked; }
		}
		protected internal bool EnableSynchronizationOnPerformCallback {
			get { return GetBoolProperty("EnableSynchronizationOnPerformCallback ", false); }
			set { SetBoolProperty("EnableSynchronizationOnPerformCallback ", false, value); }
		}
		protected internal string TextFormatString {
			get { return GetStringProperty("TextFormatString", ""); }
			set { SetStringProperty("TextFormatString", "", value); }
		}
		protected internal string DefaultTextFormatString {
			get {
				if(IsMultiColumn)
					return CommonUtils.GetDefaultTextFormatString(VisibleColumns.Count, IsRightToLeft());
				return "";
			}
		}
		internal List<ListBoxColumn> VisibleColumns {
			get {
				List<WebColumnBase> baseColumns = (this as IWebColumnsOwner).GetVisibleColumns();
				return baseColumns.ConvertAll<ListBoxColumn>(delegate(WebColumnBase baseColumn) {
					return baseColumn as ListBoxColumn;
				});
			}
		}
		internal string[] VisibleColumnFieldNames {
			get { return (this as IMulticolumnListEditDataSettings).VisibleColumnFieldNames; }
		}
		protected internal string CustomCallback {
			get { return customCallback; }
			set { customCallback = value; }
		}
		protected internal object EventCallback {
			get { return eventCallback; }
		}
		protected internal object EventItemInserting {
			get { return eventItemInserting; }
		}
		protected internal object EventItemInserted {
			get { return eventItemInserted; }
		}
		protected internal object EventItemDeleting {
			get { return eventItemDeleting; }
		}
		protected internal object EventItemDeleted {
			get { return eventItemDeleted; }
		}
		protected internal ASPxListBox ListBox {
			get { return Owner as ASPxListBox; }
		}
		protected internal ListEditItemCollection VisibleItems {
			get {
				if(visibleItems == null)
					visibleItems = GetVisibleItems();
				return visibleItems;
			}
		}
		protected bool IsMultiColumn {
			get { return !Columns.IsEmpty; }
		}
		protected internal bool IsMultiSelect {
			get { return SelectionMode != ListEditSelectionMode.Single; }
		}
		protected override bool ItemCollectionPersistenceInViewStateRequired {
			get { return !IsMultiColumn; }
		}
		internal ListBoxItemsSerializingHelper SerializingHelper {
			get {
				if(serializingHelper == null)
					serializingHelper = new ListBoxItemsSerializingHelper(this);
				return serializingHelper;
			}
		}
		protected internal ListBoxRenderHelper RenderHelper {
			get {
				if(this.renderHelper == null)
					this.renderHelper = new ListBoxRenderHelper(this);
				return this.renderHelper;
			}
		}
		protected internal ImageProperties GetItemImage(ListEditItem item, bool sampleItem) {
			return RenderHelper.GetItemImage(item, sampleItem);
		}
		protected internal bool ImageColumnExists {
			get { return RenderHelper.ImageColumnExists; }
		}
		string[] IMulticolumnListEditDataSettings.VisibleColumnFieldNames {
			get {
				List<ListBoxColumn> visibleColumns = VisibleColumns;
				string[] fieldNames = new string[visibleColumns.Count];
				for(int i = 0; i < fieldNames.Length; i++)
					fieldNames[i] = visibleColumns[i].FieldName;
				return fieldNames;
			}
		}
		bool IMulticolumnListEditDataSettings.DesignMode {
			get { return GetOwnerControl().DesignMode; }
		}
		protected internal bool PossibleLoadItemsToTop {
			get { return LoadOnDemandStrategy.PossibleLoadItemsToTop; }
		}
		protected internal bool PossibleLoadItemsToBottom {
			get { return LoadOnDemandStrategy.PossibleLoadItemsToBottom; }
		}
		protected bool CallbackModePossibleInernal {
			get { return !IsDesignMode() && !IsMultiSelect; }
		}
		protected internal virtual bool CallbackModeEnabled {
			get { return EnableCallbackMode && CallbackModePossibleInernal; }
		}
		protected internal ListEditLoadOnDemandStrategyBase LoadOnDemandStrategy{
			get{
				if(loadOnDemandStrategy == null)
					loadOnDemandStrategy = CreateLoadOnDemandStrategy();
				return loadOnDemandStrategy;
			}
		}
		internal virtual ASPxWebControl GetOwnerControl() {
			return (ASPxWebControl)Owner;
		}
		protected internal virtual int GetFirstVisibleItemIndex() {
			return LoadOnDemandStrategy.FirstVisibleItemIndex;
		}
		protected ListEditItemCollection GetVisibleItems() {
			return LoadOnDemandStrategy.GetVisibleItems();
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				ListBoxProperties src = source as ListBoxProperties;
				if(src != null)
					SelectionMode = src.SelectionMode;
				base.Assign(source);
				if(src != null) {
					TextFormatString = src.TextFormatString;
					CustomCallback = src.CustomCallback;
					EnableCallbackMode = src.EnableCallbackMode;
					EnableSynchronization = src.EnableSynchronization;
					CallbackPageSize = src.CallbackPageSize;
					DataSecurityMode = src.DataSecurityMode;
					Rows = src.Rows;
					Columns.Assign(src.Columns);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Columns });
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ListBoxClientSideEvents(this);
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxListBox();
		}
		public override EditorType GetEditorType() {
			return EditorType.Lookup;
		}
		protected internal void SynchronizeItems(string serializedItems, bool isInsertingSyncType) {
			List<DeserializedListEditItem> deserializedItems = SerializingHelper.DeserializeItems(serializedItems, isInsertingSyncType);
			foreach(DeserializedListEditItem deserializedItem in deserializedItems) {
				if(isInsertingSyncType)
					SynchronizeItemInsert(deserializedItem.Index, deserializedItem.Item);
				else {
					ListEditItem item = deserializedItem.Item;
					ListEditItem itemToDelete = Items.FindByValue(item.Value);
					if(itemToDelete != null && AreItemsEqualWithoutValues(itemToDelete, item))
						SynchronizeItemRemove(itemToDelete);
				}
			}
		}
		protected void SynchronizeItemInsert(int intdex, ListEditItem item) {
			ASPxDataInsertingEventArgs insertingArgs = new ASPxDataInsertingEventArgs();
			InitializeInsertingDictionaries(item, insertingArgs);
			RaiseItemInserting(insertingArgs);
			if(insertingArgs.Cancel)
				return;
			Items.Insert(intdex, item);
			ASPxDataInsertedEventArgs insertedArgs = new ASPxDataInsertedEventArgs(1, null);
			InitializeInsertedDictionaries(item, insertedArgs);
			RaiseItemInserted(insertedArgs);
		}
		protected void SynchronizeItemRemove(ListEditItem item) {
			ASPxDataDeletingEventArgs deletingArgs = new ASPxDataDeletingEventArgs();
			InitializeDeletingDictionaries(item, deletingArgs);
			RaiseItemDeleting(deletingArgs);
			Items.Remove(item);
			ASPxDataDeletedEventArgs deletedArgs = new ASPxDataDeletedEventArgs(1, null);
			InitializeDeletedDictionaries(item, deletedArgs);
			RaiseItemDeleted(deletedArgs);
		}
		protected string GetItemValueString(ListEditItem item, string fieldName) {
			object itemValue = item.GetValue(fieldName);
			return itemValue == null ? (ConvertEmptyStringToNull ? string.Empty : null) : itemValue.ToString();   
		}
		protected bool AreItemsEqualWithoutValues(ListEditItem item1, ListEditItem item2) {
			if(item1.Value == null || item2.Value == null)
				return false;
			if(EncodeHtml && item1.Text != item2.Text) 
				return false;
			if(IsMultiColumn) {
				foreach(ListBoxColumn column in Columns) {
					if (GetItemValueString(item1, column.FieldName) != GetItemValueString(item2, column.FieldName))
						return false;
				}
			}
			return true;
		}
		protected internal string OnCallback(string eventArgument) {
			ListBoxCallbackArgumentsReader argumentsReader = new ListBoxCallbackArgumentsReader(eventArgument);
			if(argumentsReader.IsCustomCallback)
				RaiseCustomCallback(argumentsReader.CustomCallbackArg);
			IListEditItemSerializer itemsSerializer = SerializingHelper;
			return LoadOnDemandStrategy.GetSerializedItems(itemsSerializer, "", ListEditLoadOnDemandStrategyBase.FilteringMode.None, IncrementalFilteringMode.None, argumentsReader.BeginIndex, argumentsReader.EndIndex, argumentsReader.IsCustomCallback);
		}
		protected void RaiseCustomCallback(string arg) {
			if(ListBox != null)
				ListBox.OnCustomCallback(new CallbackEventArgsBase(arg));
		}
		protected internal string GetCustomCallbackSynchronizationArg(string customCallback) {
			if(!string.IsNullOrEmpty(customCallback)) {
				CustomCallback = customCallback;
				ListBoxCallbackArgumentsReader argumentsReader = new ListBoxCallbackArgumentsReader(CustomCallback);
				if(argumentsReader.IsCustomCallback)
					return argumentsReader.CustomCallbackArg;
			}
			return null;
		}
		protected internal CallbackEventArgsBase GetCustomCallbackSynchronizationEventArg(string customCallbackArgSerialized){
			string arg = GetCustomCallbackSynchronizationArg(customCallbackArgSerialized);
			if(arg != null)
				return new CallbackEventArgsBase(arg);
			return null;
		}
		protected internal virtual ListEditLoadOnDemandStrategyBase CreateLoadOnDemandStrategy(){
			if(CallbackModeEnabled)
				return new ListEditLoadOnDemandInternalStrategy(this);
			else
				return new ListEditDisabledLoadOnDemandStrategy(this);
		}
		protected void InitializeValuesDictionary(ListEditItem item, OrderedDictionary values) {
			if(IsMultiColumn) {
				foreach(ListBoxColumn column in Columns)
					values[column.FieldName] = item.GetValue(column.FieldName);
			} else
				values[ListEditHelper.GetActualTextFieldName(TextField)] = item.Text;
			values[ListEditHelper.GetActualValueFieldName(ValueField, TextField)] = item.Value;
			values[string.IsNullOrEmpty(ImageUrlField) ? "ImageUrl" : ImageUrlField] = item.ImageUrl;
		}
		protected void InitializeKeysDictionary(ListEditItem item, OrderedDictionary keys) {
			keys.Add(ListEditHelper.GetActualValueFieldName(ValueField, TextField), item.Value);
		}
		protected void InitializeInsertingDictionaries(ListEditItem item, ASPxDataInsertingEventArgs e) {
			InitializeValuesDictionary(item, e.NewValues);
		}
		protected void InitializeInsertedDictionaries(ListEditItem item, ASPxDataInsertedEventArgs e) {
			InitializeValuesDictionary(item, e.NewValues);
		}
		protected void InitializeDeletedDictionaries(ListEditItem item, ASPxDataDeletedEventArgs e) {
			InitializeValuesDictionary(item, e.Values);
			InitializeKeysDictionary(item, e.Keys);
		}
		protected void InitializeDeletingDictionaries(ListEditItem item, ASPxDataDeletingEventArgs e) {
			InitializeValuesDictionary(item, e.Values);
			InitializeKeysDictionary(item, e.Keys);
		}
		protected virtual void RaiseItemDeleting(ASPxDataDeletingEventArgs e) { 
			ListBox.RaiseItemDeleting(e);
		}
		protected virtual void RaiseItemDeleted(ASPxDataDeletedEventArgs e) { 
			ListBox.RaiseItemDeleted(e);
		}
		protected virtual void RaiseItemInserting(ASPxDataInsertingEventArgs e) { 
			ListBox.RaiseItemInserting(e);
		}
		protected virtual void RaiseItemInserted(ASPxDataInsertedEventArgs e) { 
			ListBox.RaiseItemInserted(e);
		}
		string IMulticolumnListEditDataSettings.GetTextFormatString() {
			if(!string.IsNullOrEmpty(TextFormatString))
				return TextFormatString;
			return DefaultTextFormatString;
		}
		protected override void LayoutChanged() {
			base.LayoutChanged();
			ResetVisibleItems();
		}
		protected internal void ResetVisibleItems() {
			this.visibleItems = null;
		}
		protected override bool IsNativeSupported() {
			return true;
		}
		WebColumnCollectionBase IWebColumnsOwner.Columns { get { return Columns; } }
		List<WebColumnBase> IWebColumnsOwner.GetVisibleColumns() { return WebColumnsOwnerImpl.GetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleColumns() { WebColumnsOwnerImpl.ResetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleIndices() { WebColumnsOwnerImpl.ResetVisibleIndices(); }
		void IWebColumnsOwner.EnsureVisibleIndices() { WebColumnsOwnerImpl.EnsureVisibleIndices(); }
		void IWebColumnsOwner.SetColumnVisible(WebColumnBase column, bool value) { WebColumnsOwnerImpl.SetColumnVisible(column, value); }
		void IWebColumnsOwner.SetColumnVisibleIndex(WebColumnBase column, int value) { WebColumnsOwnerImpl.SetColumnVisibleIndex(column, value); }
		void IWebColumnsOwner.OnColumnChanged(WebColumnBase column) {
			WebColumnsOwnerImpl.OnColumnChanged(column);
		}
		void IWebColumnsOwner.OnColumnCollectionChanged() {
			WebColumnsOwnerImpl.OnColumnCollectionChanged();
		}
		protected internal override bool? GetItemSelected(ListEditItem item) {
			ASPxListBox listBox = Owner as ASPxListBox;
			if(listBox != null)
				return listBox.GetItemSelected(item);
			return null;
		}
		protected internal override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(ListBox != null)
				ListBox.OnItemSelectionChanged(item, selected);
		}
		protected internal override void OnItemDeleting(ListEditItem item) {
			if(ListBox != null)
				ListBox.OnItemDeleting(item);
		}
		protected internal override void OnItemsCleared() {
			if(ListBox != null)
				ListBox.OnItemsCleared();
		}
		protected internal virtual void ResetOnDemandStrategy(){
			loadOnDemandStrategy = null;
		}
		Page IListBoxRenderHelperOwner.Page {
			get {
				if(ListBox != null)
					return ListBox.Page;
				return null;
			}
		}
		List<ListBoxColumn> IListBoxRenderHelperOwner.VisibleColumns {
			get { return VisibleColumns; }
		}
		EditorImages IListBoxRenderHelperOwner.RenderImages {
			get { return RenderImages; }
		}
		bool IListBoxRenderHelperOwner.DesignMode {
			get { return GetOwnerControl().DesignMode; }
		}
		bool IListBoxRenderHelperOwner.IsNativeRender {
			get { return GetOwnerControl().IsNativeRender(); }
		}
		bool IListBoxRenderHelperOwner.IsClientSideAPIEnabledInternal {
			get {
				return ListBox.IsClientSideAPIEnabledInternal();
			}
		}
		int IListEditItemsRequester.SelectedIndex{
			get { 
				if(ListBox != null)
					return ListBox.SelectedIndex;
				return -1;
			}
		}
		protected internal virtual object ResolveClientUrl(string url) {
			if(ListBox != null)
				return ListBox.ResolveClientUrl(url);
			return url;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free), 
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxData("<{0}:ASPxListBox runat=\"server\" ValueType=\"System.String\"></{0}:ASPxListBox>"),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxListBox.bmp"),
	Designer("DevExpress.Web.Design.ASPxListBoxDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public class ASPxListBox : ASPxListEdit, IListBoxColumnsOwner, IMultiSelectListEdit, IControlDesigner {
		protected internal enum SynchronizationType { DeletedItems, InsertedItems, CustomCallback };
		protected const string DefaultListBoxDivClassName = "dxlbd";
		protected const string ListBoxCheckColumnClassName = "dxlbcc";
		protected const string DefaultListBoxHeaderDivClassName = "dxeHD";
		protected const string ItemId = "LBI";
		protected internal const string HeaderDivID = "H";
		protected internal const string NativeItemDblClickHandlerName = "return ASPx.NLBIDClick(event)";
		protected static readonly string[] ItemIdPostfixes = new string[] { "I", "T" };
		private string callbackResult = "";
		private int baseDataBindCallLockCount = 0;
		private bool isComboBoxList = false;
		private bool isComboBoxClientSideAPIEnabled = false;
		private ListBoxProperties propertiesFromCtor;
		private SelectedIndexCollection selectedIndices;
		private SelectedItemCollection selectedItems;
		private SelectedValueCollection selectedValues;
		private bool selectedIndicesClientChanged = false;
		private bool internalDisableScrolling = false;
		public ASPxListBox()
			: base() {
		}
		protected ASPxListBox(ASPxWebControl ownerControl)
			: this(ownerControl, null) {
		}
		protected internal ASPxListBox(ASPxWebControl ownerControl, ListBoxProperties properties)
			: base(ownerControl) {
			this.propertiesFromCtor = properties;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCallbackPageSize"),
#endif
		Category("Behavior"), DefaultValue(ListBoxProperties.CallbackPageSizeDefault),
		AutoFormatDisable, Themeable(false)]
		public int CallbackPageSize {
			get { return Properties.CallbackPageSize; }
			set { Properties.CallbackPageSize = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxColumns"),
#endif
		Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public ListBoxColumnCollection Columns {
			get { return Properties.Columns; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxEnableCallbackMode"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Themeable(false)]
		public bool EnableCallbackMode {
			get { return Properties.EnableCallbackMode; }
			set { Properties.EnableCallbackMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxEnableSynchronization"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DefaultBoolean EnableSynchronization {
			get { return Properties.EnableSynchronization; }
			set { Properties.EnableSynchronization = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxDataSecurityMode"),
#endif
		DefaultValue(DataSecurityMode.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DataSecurityMode DataSecurityMode {
			get { return Properties.DataSecurityMode; }
			set { Properties.DataSecurityMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxEncodeHtml"),
#endif
		Category("Behavior"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		AutoFormatDisable, Themeable(false)]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ItemImage {
			get { return Properties.ItemImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBoxItemStyle ItemStyle {
			get { return Properties.ItemStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCheckBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle CheckBoxStyle {
			get { return Properties.CheckBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCheckBoxFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle CheckBoxFocusedStyle {
			get { return Properties.CheckBoxFocused; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCheckBoxCheckedImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxCheckedImage {
			get { return Properties.CheckBoxChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCheckBoxUncheckedImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxUncheckedImage {
			get { return Properties.CheckBoxUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxNative"),
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
	DevExpressWebLocalizedDescription("ASPxListBoxNativeCheckBoxes"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public bool NativeCheckBoxes
		{
			get { return GetBoolProperty("NativeCheckBoxes", false); }
			set
			{
				SetBoolProperty("NativeCheckBoxes", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxRows"),
#endif
		Category("Layout"), DefaultValue(5), AutoFormatDisable]
		public int Rows {
			get { return Properties.Rows; }
			set { Properties.Rows = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxSelectionMode"),
#endif
		Category("Behavior"), AutoFormatEnable, DefaultValue(ListEditSelectionMode.Single)]
		public ListEditSelectionMode SelectionMode {
			get { return Properties.SelectionMode; }
			set {
				if(Properties.SelectionMode != value) {
					Properties.SelectionMode = value;
					OnSelectionModeChanged();
				}
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxListBoxValue")]
#endif
		public override object Value {
			get {
				ListEditHelper.ValueDemanded(ConvertEmptyStringToNull);
				return base.Value;
			}
			set {
				ListEditHelper.ValueDemanded(ConvertEmptyStringToNull);
				if(!CommonUtils.AreEqual(base.Value, value, false)) {
					base.Value = value;
					ListEditHelper.OnValueChanged(value);
					Properties.ResetVisibleItems();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ListBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEventsInternal as ListBoxClientSideEvents; }
		}
		[DefaultValue(false), AutoFormatDisable,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool EnableSynchronizationOnPerformCallback {
			get { return Properties.EnableSynchronizationOnPerformCallback; }
			set { Properties.EnableSynchronizationOnPerformCallback = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxSelectedIndices"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		MergableProperty(false), AutoFormatDisable]
		public SelectedIndexCollection SelectedIndices {
			get {
				if(selectedIndices == null)
					selectedIndices = new SelectedIndexCollection(this);
				return selectedIndices;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxSelectedItems"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		MergableProperty(false), AutoFormatDisable]
		public SelectedItemCollection SelectedItems {
			get {
				if(selectedItems == null)
					selectedItems = new SelectedItemCollection(this);
				return selectedItems;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxListBoxSelectedValues")]
#endif
		public SelectedValueCollection SelectedValues {
			get {
				if(selectedValues == null) {
					selectedValues = new SelectedValueCollection(this);
					selectedValues.SetSortMethod(new SelectedValueCollection.SortMethod(SortSelectedValue));
				}
				return selectedValues;
			}
		}
		protected void SortSelectedValue() {
			List<object> sortedValues = new List<object>();
			foreach(ListEditItem item in Items) {
				if(item.Selected)
					sortedValues.Add(item.Value);
			}
			SelectedValues.ClearSelection();
			foreach(object value in sortedValues)
				SelectedValues.AddInternal(value);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback
		{
			add { Events.AddHandler(Properties.EventCallback, value); }
			remove { Events.RemoveHandler(Properties.EventCallback, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemDeleting"),
#endif
		Category("Action")]
		public event ASPxDataDeletingEventHandler ItemDeleting
		{
			add { Events.AddHandler(Properties.EventItemDeleting, value); }
			remove { Events.RemoveHandler(Properties.EventItemDeleting, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemDeleted"),
#endif
		Category("Action")]
		public event ASPxDataDeletedEventHandler ItemDeleted
		{
			add { Events.AddHandler(Properties.EventItemDeleted, value); }
			remove { Events.RemoveHandler(Properties.EventItemDeleted, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemInserting"),
#endif
		Category("Action")]
		public event ASPxDataInsertingEventHandler ItemInserting
		{
			add { Events.AddHandler(Properties.EventItemInserting, value); }
			remove { Events.RemoveHandler(Properties.EventItemInserting, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListBoxItemInserted"),
#endif
		Category("Action")]
		public event ASPxDataInsertedEventHandler ItemInserted
		{
			add { Events.AddHandler(Properties.EventItemInserted, value); }
			remove { Events.RemoveHandler(Properties.EventItemInserted, value); }
		}
		[Browsable(false)]
		public virtual bool HasSampleItem {
			get { return RenderHelper.HasSampleItem; }
		}
		protected internal virtual bool HasFakeItem {
			get { return Items.Count == 0 && !IsNativeRender(); }
		}
		protected internal bool IsComboBoxList {
			get { return isComboBoxList; }
			set { isComboBoxList = value; }
		}
		protected internal bool IsComboBoxClientSideAPIEnabled {
			get { return isComboBoxList && isComboBoxClientSideAPIEnabled; }
			set { isComboBoxClientSideAPIEnabled = value; }
		}
		protected internal bool IsMultiColumn {
			get { return RenderHelper.IsMultiColumn; }
		}
		protected internal bool IsMultiSelect {
			get { return Properties.IsMultiSelect; }
		}
		protected internal bool IsCheckColumnExists {
			get { return RenderHelper.IsCheckColumnExists; }
		}
		protected internal new ListBoxProperties Properties {
			get { return (ListBoxProperties)base.Properties; }
		}
		protected internal ListEditItem SampleItem {
			get { return RenderHelper.SampleItem; }
		}
		protected internal ListBoxRenderHelper RenderHelper {
			get { return Properties.RenderHelper; }
		}
		protected internal bool InternalDisableScrolling {
			get { return internalDisableScrolling; }
			set { internalDisableScrolling = value; }
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(IsMultiColumn && Items.IsEmpty && Page != null && Page.IsPostBack)
				RequireDataBinding();
		}
		public void SelectAll() {
			if(SelectionMode != ListEditSelectionMode.Single)
				ListEditHelper.SetAllSelection(true);
		}
		public void UnselectAll() {
			if(SelectionMode != ListEditSelectionMode.Single)
				ListEditHelper.SetAllSelection(false);
			else
				Value = null;
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return !NativeCheckBoxes;
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			if(!IsComboBoxList)
				Properties.ConvertItemTypes();
		}
		protected override ListEditHelper CreateListEditHelper() {
			return new ListEditHelper(new ListBoxMultiSelectHelperOwnerProxy(this));
		}
		protected internal void OnSelectionModeChanged() {
			ListEditHelper.OnSelectionModeChanged(SelectionMode);
			LayoutChanged();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { SelectedValues });
		}
		public override bool IsClientSideAPIEnabled() {
			return base.IsClientSideAPIEnabled() || (IsComboBoxList && IsComboBoxClientSideAPIEnabled);
		}
		protected internal bool IsClientSideAPIEnabledInternal() {
			return (IsComboBoxList ? IsComboBoxClientSideAPIEnabled : base.IsClientSideAPIEnabled()) || GetIsCallbackModeEnabled();
		}
		protected internal override bool IsCallBacksEnabled() {
			return (GetIsCallbackModeEnabled() || IsClientSideAPIEnabledInternal()) && !IsComboBoxList;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			RenderHelper.OnCreateControlHierarchy();
			if(IsNativeRender()) {
				if(!IsComboBoxList)
					Controls.Add(new ListBoxNativeControl(this));
			} 
			else {
				Controls.Add(CreateListBoxControl());
			}
		}
		protected virtual ListBoxControl CreateListBoxControl() {
			return new ListBoxControl(this);
		}
		protected override EditPropertiesBase CreateProperties() {
			return propertiesFromCtor != null ? propertiesFromCtor : new ListBoxProperties(this);
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPxClientNativeListBox" : "ASPxClientListBox";
		}
		protected override bool HasLoadingPanel() {
			return !IsNativeRender() && !IsComboBoxList && base.HasLoadingPanel();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected internal bool IsSynchronizationEnabled() {
			if(GetIsCallbackModeEnabled())
				return false;
			return EnableSynchronization != DefaultBoolean.False;
		}
		protected internal bool IsSampleItemIndex(int index) {
			return RenderHelper.IsSampleItemIndex(index);
		}
		protected internal bool IsHeaderRequired() {
			return IsMultiColumn; 
		}
		protected internal ListEditItem GetVisibleItem(int index) {
			if(IsSampleItemIndex(index))
				return SampleItem;
			return Properties.VisibleItems[index] as ListEditItem;
		}
		protected internal string GetVisibleItemText(int index) {
			return GetVisibleItemText(index, -1);
		}
		protected internal string GetVisibleItemText(int rowIndex, int visibleColumnIndex) {
			if(IsSampleItemIndex(rowIndex))
				return "&nbsp;";
			ListEditItem item = Properties.VisibleItems[rowIndex];
			string itemText = null;
			if(IsMultiColumn) {
				string fieldName = RenderHelper.VisibleColumns[visibleColumnIndex].FieldName;
				object itemTextObj = item.GetValue(fieldName);
				if(itemTextObj != null)
					itemText = itemTextObj.ToString();
			} else
				itemText = item.Text;
			return EncodeItemText(itemText, EncodeHtml);
		}
		protected internal static string EncodeItemText(string text, bool encodeHtml) {
			return encodeHtml ? HttpUtility.HtmlEncode(text) : text;
		}
		protected internal string GetTopSpacerId() {
			return "TS";
		}
		protected internal string GetBottomSpacerId() {
			return "BS";
		}
		protected internal string GetItemImageCellId() {
			return ItemId + ItemIdPostfixes[0];
		}
		protected internal string GetItemTextCellId() {
			return ItemId + ItemIdPostfixes[1];
		}
		protected internal string GetSampleItemId() {
			return ItemId + "-1";
		}
		protected internal string GetScrollDivId() {
			return "D";
		}
		protected internal string GetListTableId() {
			return "LBT";
		}
		protected internal override string GetAssociatedControlID() {
			return ClientID + "_" + (this.IsAccessibilityCompliantRender() ? AccessibilityUtils.AssistantID : GetKBSupportInputId());
		}
		protected internal override bool IsAccessibilityAssociatingSupported() {
			return RenderUtils.IsHtml5Mode(this) && IsAccessibilityCompliantRender() && !IsNativeRender();
		}
		protected override bool IsAccessibilityCompliant() {
			return IsAriaSupported() && base.IsAccessibilityCompliant();
		}
		protected internal bool HasImage(ListEditItem item, bool SampleItem) {
			return RenderHelper.HasImage(item, SampleItem);
		}
		protected internal ImageProperties GetItemImage(ListEditItem item, bool sampleItem) {
			return RenderHelper.GetItemImage(item, sampleItem);
		}
		protected override object GetPostBackValue(string controlName, NameValueCollection postCollection) {
			ListEditHelper.ResetStoredSelectedIndex();
			if(IsNativeRender()) {
				if(!IsComboBoxList) {
					if(!string.IsNullOrEmpty(postCollection[UniqueID])) {
						if(IsMultiSelect) {
							string[] clientIndices = postCollection[UniqueID].Split(new char[] { ',' });
							int index;
							Value = null;
							foreach(string clientIndex in clientIndices) {
								if(int.TryParse(clientIndex, out index)) {
									if(0 <= index && index < Items.Count)
										Items[index].Selected = true;
								}
							}
							return Value;
						} else {
							int index;
							if(int.TryParse(postCollection[UniqueID], out index))
								return (0 <= index && index < Items.Count) ? Items[index].Value : null;
						}
					}
				}
				return null;
			} else {
				if(IsMultiSelect) {
					string serializedClientValue = GetClientValue(UniqueID, postCollection);
					if(serializedClientValue != null) {
						string[] clientStringValues = Properties.SerializingHelper.DeserializeMultiSelectValues(serializedClientValue).ToArray();
						ArrayList clientValues = new ArrayList(Array.ConvertAll<string, object>(clientStringValues,
							new Converter<string, object>(ConvertClientValueToValueType)));
						List<object> newClientValuesToSelect = new List<object>();
						foreach(object clientValue in clientValues) {
							if(!SelectedValues.Contains(clientValue))
								newClientValuesToSelect.Add(clientValue);
						}
						List<object> valuesToUnselect = new List<object>();
						foreach(object value in SelectedValues) {
							if(!clientValues.Contains(value))
								valuesToUnselect.Add(value);
						}
						if(newClientValuesToSelect.Count != 0 || valuesToUnselect.Count != 0)
							selectedIndicesClientChanged = true;
						foreach(object value in valuesToUnselect) {
							SetItemSelectedByValue(value, false);
						}
						foreach(object value in newClientValuesToSelect) {
							SetItemSelectedByValue(value, true);
						}
					}
					return Value;
				} else {
					object clientValue = base.GetPostBackValue(UniqueID, postCollection);
					return clientValue != null ? clientValue : Value;
				}
			}
		}
		protected void SetItemSelectedByValue(object value, bool selected) {
			ListEditItem item = Items.FindByValue(value);
			if(item != null)
				item.Selected = selected;
		}
		protected internal virtual string GetOnNativeSelectedIndexChanged() {
			if(ClientSideEvents.ValueChanged != "" || ClientSideEvents.SelectedIndexChanged != "" || IsCustomValidationEnabled)
				return string.Format(ValueChangedHandlerName, ClientID);
			else if(AutoPostBack)
				return GetPostBackEventReference(false, false);
			return "";
		}
		protected internal virtual string GetOnNativeDblClick() {
			if(ClientSideEvents.ItemDoubleClick != String.Empty)
				return NativeItemDblClickHandlerName;
			return "";
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			EnsureHasSampleItemOnPreRender();
		}
		void EnsureHasSampleItemOnPreRender() {
			if(HasSampleItem != RenderHelper.SampleItemCreated)
				RecreateControlHierarchy();
		}
		protected override bool HasHoverScripts() {
			return !ReadOnly;
		}
		protected override bool HasSelectedScripts() {
			return true;
		}
		protected override bool IsStateScriptEnabled() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override void RegisterScriptBlocks() {
			if(IsMultiColumn && IsAccessibilityCompliantRender()) {
				string script = string.Format("ASPx.AccessibilitySR.TableItemFormatString = {0};",
					HtmlConvertor.ToScript(AccessibilityUtils.TableItemFormatString));
				RegisterScriptBlock("AccessibilitySR_ListBox", RenderUtils.GetScriptHtml(script));
			}
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(GetItemsValueArrayScript(localVarName));
			if(!IsSynchronizationEnabled())
				stb.AppendFormat("{0}.isSyncEnabled = false;\n", localVarName);
			if(IsComboBoxList)
				stb.AppendFormat("{0}.isComboBoxList = true;\n", localVarName);
			if(EnableSynchronizationOnPerformCallback)
				stb.AppendFormat("{0}.enableSyncOnPerfCallback = true;\n", localVarName);
			if(!IsNativeRender()) {
				if(InternalDisableScrolling)
					stb.AppendFormat("{0}.disableScrolling = true;\n", localVarName); 
				if(RenderHelper.ImageColumnExists) {
					stb.Append(localVarName + ".imageCellExists = true;\n");
					string defaultImageUrl = RenderHelper.GetDefaultItemImage().Url;
					if(!string.IsNullOrEmpty(defaultImageUrl))
						stb.AppendFormat("{0}.defaultImageUrl = '{1}';\n", localVarName, ResolveClientUrl(defaultImageUrl));
				}
				if(!Width.IsEmpty)
					stb.AppendFormat("{0}.width='{1}';\n", localVarName, Width.ToString());
				if(HasSampleItem)
					stb.AppendFormat("{0}.hasSampleItem = true;\n", localVarName);
				if(GetIsCallbackModeEnabled()) {
					stb.AppendFormat("{0}.isCallbackMode = true;\n", localVarName);
					stb.AppendFormat("{0}.callbackPageSize = {1};\n", localVarName, CallbackPageSize);
					if(Properties.PossibleLoadItemsToTop) {
						stb.AppendFormat("{0}.serverIndexOfFirstItem = {1};\n", localVarName, Properties.GetFirstVisibleItemIndex());
						stb.AppendFormat("{0}.isTopSpacerVisible = true;\n", localVarName);
					}
					if(Properties.PossibleLoadItemsToBottom)
						stb.AppendFormat("{0}.isBottomSpacerVisible = true;\n", localVarName);
				}
				if(HasFakeItem)
					stb.AppendFormat("{0}.isHasFakeRow = true;\n", localVarName);
				if(!ReadOnly && IsEnabled()) {
					GetStateStyleScript(stb, localVarName, GetItemHoverCssStyle(), "hoverClasses", "hoverCssArray");
					GetStateStyleScript(stb, localVarName, GetItemSelectedCssStyle(), "selectedClasses", "selectedCssArray");
					GetStateStyleScript(stb, localVarName, GetItemDisabledCssStyle(), "disabledClasses", "disabledCssArray");
				}
				if(IsMultiColumn) {
					stb.AppendFormat("{0}.columnFieldNames={1};\n",
						localVarName, HtmlConvertor.ToJSON(Properties.VisibleColumnFieldNames));
					stb.AppendFormat("{0}.textFormatString='{1}';\n", localVarName, (Properties as IMulticolumnListEditDataSettings).GetTextFormatString());
				}
				if(IsMultiSelect) {
					stb.AppendFormat("{0}.selectionMode={1};\n", localVarName, (int)SelectionMode);
					stb.Append(ListEditHelper.GetMultiSelectedIndicesArrayScript(localVarName));
				}
				HorizontalAlign itemHorizontalAlign = GetItemStyle(false).HorizontalAlign;
				if(itemHorizontalAlign != HorizontalAlign.NotSet)
					stb.AppendFormat("{0}.itemHorizontalAlign='{1}';\n", localVarName, itemHorizontalAlign.ToString().ToLower());
				stb.Append(GetEmptyTextRowCellIndices(localVarName));
				if(IsCheckColumnExists && !NativeCheckBoxes) {
					stb.AppendFormat("{0}.icbImageProperties = {1};\n", localVarName, ImagePropertiesSerializer.GetImageProperties(GetInternalCheckImages(), this));
					stb.AppendFormat("{0}.icbFocusedStyle = {1};\n", localVarName, GetSerializedInternalCheckBoxFocusedStyle());
				}
				if(AccessibilityCompliantInternal) {
					stb.AppendFormat("{0}.itemTotalCount = {1};\n", localVarName, Items.Count);
				}
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			if(IsClientSideAPIEnabledInternal()) 
				result.Add(SynchronizationType.CustomCallback.ToString(), Properties.CustomCallback);
			return result;
		}
		protected virtual string GetSerializedInternalCheckBoxFocusedStyle() {
			AppearanceStyleBase style = Properties.Styles.CreateStyleByName(string.Empty, InternalCheckboxControl.FocusedCheckBoxClassName);
			style.CopyFrom(CheckBoxFocusedStyle);
			return InternalCheckboxControl.SerializeFocusedStyle(style, this);
		}
		protected List<InternalCheckBoxImageProperties> GetInternalCheckImages() {
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] {
				GetCheckableImage(CheckState.Checked),
				GetCheckableImage(CheckState.Unchecked)
			});
		}
		protected string GetEmptyTextRowCellIndices(string localVarName) {
			bool listIsEmpty = true;
			List<object> emptyRowCellIndices = new List<object>();
			int textCellIndex = 0;
			for(int i = 0; i < Properties.VisibleItems.Count; i++) {
				List<int> emptyCellIndices = new List<int>();
				for(int j = RenderHelper.FirstTextCellIndex; j <= RenderHelper.LastTextCellIndex; j++) {
					textCellIndex = j - RenderHelper.FirstTextCellIndex;
					string text = IsMultiColumn ? GetVisibleItemText(i, textCellIndex) : GetVisibleItemText(i);
					if(string.IsNullOrEmpty(text)){
						emptyCellIndices.Add(j);
						listIsEmpty = false;
					}
				}
				if(emptyCellIndices.Count > 0)
					emptyRowCellIndices.Add(new object[]{i, emptyCellIndices} );
			}
			if(!listIsEmpty)
				return String.Format("{0}.emptyTextRowCellIndices={1};\n", localVarName, HtmlConvertor.ToJSON(emptyRowCellIndices));
			return string.Empty;
		}
		protected void GetStateStyleScript(StringBuilder stb, string localVarName,
			AppearanceStyleBase style, string classField, string cssField) {
			if(!string.IsNullOrEmpty(style.CssClass))
				stb.AppendLine(localVarName + "." + classField + "=[" + HtmlConvertor.ToScript(style.CssClass) + "];");
			string cssText = style.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(cssText))
				stb.AppendLine(localVarName + "." + cssField + "=[" + HtmlConvertor.ToScript(cssText) + "];");
		}
		protected string GetItemsValueArrayScript(string localVarName) {
			if(Properties.Items.Count > 0) {
				List<object> itemValues = new List<object>();
				for(int i = 0; i < Properties.VisibleItems.Count; i++)
					itemValues.Add(GetItemClientValue(Properties.VisibleItems[i]));
				return String.Format("{0}.itemsValue={1};\n", localVarName, HtmlConvertor.ToJSON(itemValues));
			}
			return "";
		}
		protected override int GetClientSavedSelectedIndex() {
			return SelectedIndex - Properties.GetFirstVisibleItemIndex();
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			return AppearanceStyle.Empty;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.ListBox);
			return style;
		}
		protected internal AppearanceStyleBase GetScrollingDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = DefaultListBoxDivClassName;
			return style;
		}
		protected override void PrepareControlStyleCore(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.GetDefaultListBoxStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(GetEditStyleFromStylesStorage());
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
			if(IsCheckColumnExists)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, ListBoxCheckColumnClassName);
		}
		protected override void PrepareControlHierarchy() {
			if(IsAriaSupported() && IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "role", "application");
			base.PrepareControlHierarchy();
		}
		static object itemStyleKey = new object();
		protected ListBoxItemStyle GetItemStyle(bool isSelected) {
			ListBoxItemStyle cachedItemStyle = (ListBoxItemStyle)CreateStyle(delegate() {
				ListBoxItemStyle style = new ListBoxItemStyle();
				style.CopyFrom(RenderStyles.GetDefaultListBoxItemStyle());
				style.CopyFrom(RenderStyles.ListBoxItem);
				if(isSelected)
					style.CopyFrom(style.SelectedStyle);
				MergeDisableStyle(style);
				return style;
			}, GetBoolParam(isSelected), itemStyleKey);
			ListBoxItemStyle clone = new ListBoxItemStyle();
			clone.Assign(cachedItemStyle);
			return clone;
		}
		protected internal AppearanceStyleBase GetItemRowStyle(bool isSelected) {
			ListBoxItemStyle itemStyle = GetItemStyle(isSelected);
			AppearanceStyleBase rowStyle = new AppearanceStyleBase();
			rowStyle.CopyFrom(RenderStyles.GetDefaultListBoxItemRowStyle());
			rowStyle.BackgroundImage.Assign(itemStyle.BackgroundImage);
			rowStyle.Cursor = itemStyle.Cursor;
			return rowStyle;
		}
		protected internal ListBoxItemStyle GetItemCellStyle(bool isSelected) {
			ListBoxItemStyle itemCellStyle = GetItemStyle(isSelected);
			itemCellStyle.BackgroundImage.Reset();
			itemCellStyle.Cursor = string.Empty;
			itemCellStyle.HorizontalAlign = HorizontalAlign.NotSet;
			return itemCellStyle;
		}
		protected internal AppearanceStyleBase GetHeaderDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = DefaultListBoxHeaderDivClassName;
			return style;
		}
		protected internal ListBoxItemStyle GetHeaderCellStyle() {
			ListBoxItemStyle headerCellStyle = new ListBoxItemStyle();
			ListBoxItemStyle itemCellStyle = GetItemCellStyle(false);
			headerCellStyle.CssClass = itemCellStyle.CssClass;
			headerCellStyle.CopyFontFrom(itemCellStyle);
			headerCellStyle.CopyBordersFrom(itemCellStyle);
			headerCellStyle.Paddings.CopyFrom(itemCellStyle.Paddings);
			return headerCellStyle;
		}
		protected internal bool GetIsItemAllowsSelectedStyle(int index, int selectedIndex) {
			bool indexSelected = (index == selectedIndex) || (IsMultiSelect && SelectedIndices.Contains(index));
			bool selectedStyleAllowed = DesignMode || ReadOnly || !IsEnabled();
			return selectedStyleAllowed && indexSelected;
		}
		protected AppearanceStyle GetItemHoverCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultListBoxItemStyle().HoverStyle);
			style.CopyFrom(RenderStyles.ListBoxItem.HoverStyle);
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(style));
			return style;
		}
		protected AppearanceStyle GetItemSelectedCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultListBoxItemStyle().SelectedStyle);
			style.CopyFrom(RenderStyles.ListBoxItem.SelectedStyle);
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(style));
			return style;
		}
		protected DisabledStyle GetItemDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledCssStyle());
			return style;
		}
		protected Paddings GetItemSelectedCssStylePaddings(AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetItemStyle(false);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, style.Paddings);
		}
		protected internal AppearanceStyleBase GetInternalCheckBoxStyle() {
			AppearanceStyleBase style = Properties.Styles.CreateStyleByName(string.Empty, InternalCheckboxControl.CheckBoxClassName);
			style.CopyFrom(CheckBoxStyle);
			return style;
		}
		protected internal InternalCheckBoxImageProperties GetCheckableImage(CheckState checkState) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch(checkState) {
				case CheckState.Checked:
					imageName = InternalCheckboxControl.CheckBoxCheckedImageName;
					result.MergeWith(CheckBoxCheckedImage);
					break;
				case CheckState.Unchecked:
					imageName = InternalCheckboxControl.CheckBoxUncheckedImageName;
					result.MergeWith(CheckBoxUncheckedImage);
					break;
			}
			result.MergeWith(Properties.Images.GetImageProperties(Page, imageName));
			Properties.Images.UpdateSpriteUrl(result, Page, InternalCheckboxControl.WebSpriteControlName, typeof(ASPxWebControl), InternalCheckboxControl.DesignModeSpriteImagePath);
			return result;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(!IsComboBoxList) {
				if(IsMultiColumn && Items.IsEmpty)
					RequireDataBinding();
				EnsureDataBound();
				SynchronizeCustomCallback(postCollection);
				if(DataSecurityMode == DataSecurityMode.Default)
					SynchronizeItems(postCollection);
			}
			bool valueChanged = base.LoadPostData(postCollection);
			return valueChanged || GetIsSelectionChangedByPostData();
		}
		protected override bool IsPostBackValueSecure(object value) {
			return base.IsPostBackValueSecure(value) && (DataSecurityMode == DataSecurityMode.Default || Items.FindByValue(value) != null);
		}
		protected bool GetIsSelectionChangedByPostData() { 
			return this.selectedIndicesClientChanged && IsMultiSelect;
		}
		protected void SynchronizeCustomCallback(NameValueCollection postCollection) {
			if(Page == null || !Page.IsCallback) {
				string customCallbackArgSerialized = GetClientObjectStateValueString(SynchronizationType.CustomCallback.ToString());
				CallbackEventArgsBase eventArg = Properties.GetCustomCallbackSynchronizationEventArg(customCallbackArgSerialized);
				if(eventArg != null)
					OnCustomCallback(eventArg);
			}
		}
		protected void SynchronizeItems(NameValueCollection postCollection) {
			string serializedDeletedItems = GetClientObjectStateValueString(SynchronizationType.DeletedItems.ToString());
			if(!string.IsNullOrEmpty(serializedDeletedItems))
				Properties.SynchronizeItems(serializedDeletedItems, false);
			string serializedInsertedItems = GetClientObjectStateValueString(SynchronizationType.InsertedItems.ToString());
			if(!string.IsNullOrEmpty(serializedInsertedItems))
				Properties.SynchronizeItems(serializedInsertedItems, true);
		}
		protected override object GetCallbackResult() {
			return callbackResult;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			callbackResult = Properties.OnCallback(eventArgument);
		}
		protected internal virtual void OnCustomCallback(CallbackEventArgsBase e) {
			LockBaseDataBindCall(); 
			try {
				CallbackEventHandlerBase handler = Events[Properties.EventCallback] as CallbackEventHandlerBase;
				if(handler != null)
					handler(this, e);
			} finally {
				UnlockBaseDataBindCall();
			}
		}
		protected internal void RaiseItemDeleting(ASPxDataDeletingEventArgs e) {
			ASPxDataDeletingEventHandler handler = (ASPxDataDeletingEventHandler)Events[Properties.EventItemDeleting];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemDeleted(ASPxDataDeletedEventArgs e) {
			ASPxDataDeletedEventHandler handler = (ASPxDataDeletedEventHandler)Events[Properties.EventItemDeleted];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemInserting(ASPxDataInsertingEventArgs e) {
			ASPxDataInsertingEventHandler handler = (ASPxDataInsertingEventHandler)Events[Properties.EventItemInserting];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemInserted(ASPxDataInsertedEventArgs e) {
			ASPxDataInsertedEventHandler handler = (ASPxDataInsertedEventHandler)Events[Properties.EventItemInserted];
			if(handler != null)
				handler(this, e);
		}
		protected internal void OnItemSelectionChanged(ListEditItem item, bool selected) {
			ListEditHelper.OnItemSelectionChanged(item, selected);
		}
		protected internal void OnItemDeleting(ListEditItem item) {
			ListEditHelper.OnItemDeleting(item);
		}
		protected internal void OnItemsCleared() {
			ListEditHelper.OnItemsCleared();
		}
		public virtual void DataBindItems() { 
			LockBaseDataBindCall();
			try {
				DataBind();
			} finally {
				UnlockBaseDataBindCall();
			}
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(IsComboBoxList)
				return;
			base.PerformDataBinding(dataHelperName, data);
		}
		protected override bool PerformDataBindingCore(string dataHelperName, IEnumerable data) {
			if(Columns.IsEmpty)
				return base.PerformDataBindingCore(dataHelperName, data);
			else
				return ListEditHelper.PerformDataBindingMulticolumn(data);
		}
		protected override void OnDataBinding(EventArgs e) {
			if(BaseDataBindCallIsLocked())
				return; 
			base.OnDataBinding(e);
		}
		protected internal bool? GetItemSelected(ListEditItem item) {
			return ListEditHelper.GetItemSelected(item, ConvertEmptyStringToNull);
		}
		protected bool GetIsCallbackModeEnabled() {
			return Properties.CallbackModeEnabled;
		}
		protected void LockBaseDataBindCall() {
			baseDataBindCallLockCount++;
		}
		protected void UnlockBaseDataBindCall() {
			baseDataBindCallLockCount--;
		}
		protected bool BaseDataBindCallIsLocked() {
			return baseDataBindCallLockCount > 0;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ListBoxCommonFormDesigner"; } }
	}
	internal class DeserializedListEditItem {
		private readonly ListEditItem item;
		private readonly int index;
		public DeserializedListEditItem(ListEditItem item, int index) {
			this.item = item;
			this.index = index;
		}
		public ListEditItem Item {
			get { return item; }
		}
		public int Index {
			get { return index; }
		}
	}
	internal class ListEditItemsSerializingHelper {
		ListEditProperties properties;
		protected static readonly char Separator = '|';
		protected static readonly int lengthOfSeparator = 1;
		public ListEditItemsSerializingHelper(ListEditProperties properties) {
			this.properties = properties;
		}
		protected ListEditProperties Properties {
			get { return properties; }
		}
		public List<int> DeserializeMultiSelectIndices(string serializedIndices) {
			List<int> deserializedIndices = new List<int>();
			int startPos = 0;
			while(startPos < serializedIndices.Length) {
				deserializedIndices.Add(ParseItemIndex(serializedIndices, ref startPos));
			}
			return deserializedIndices;
		}
		protected int ParseItemIndex(string serializedItem, ref int startPos) {
			return Int32.Parse(ParseString(serializedItem, ref startPos));
		}
		protected static string ParseString(string str, ref int startPos) {
			int indexOfSeparator = str.IndexOf(Separator, startPos);
			int length = indexOfSeparator - startPos;
			int strLength = int.Parse(str.Substring(startPos, length));
			startPos = indexOfSeparator + lengthOfSeparator + strLength;
			return str.Substring(indexOfSeparator + 1, strLength);
		}
	}
	internal class ListBoxItemsSerializingHelper : ListEditItemsSerializingHelper, IListEditItemSerializer {
		public ListBoxItemsSerializingHelper(ListBoxProperties properties) : base(properties) {
		}
		protected new ListBoxProperties Properties {
			get { return base.Properties as ListBoxProperties; }
		}
		protected List<ListBoxColumn> VisibleColumns {
			get { return Properties.VisibleColumns; }
		}
		protected Type ValueType {
			get { return Properties.ValueType; }
		}
		[SecuritySafeCritical] 
		public string SerializeItemsRange(ListEditItemCollection items, int beginIndex, int endIndex) {
			return SerializeItemsRange(items, beginIndex, endIndex, false);
		}
		[SecuritySafeCritical] 
		public string SerializeItemsRange(ListEditItemCollection items, int beginIndex, int endIndex, bool needSelection){
			StringBuilder sb = new StringBuilder();
			AdjustItemsRange(items, ref beginIndex, ref endIndex);
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			for(int i = beginIndex; i <= endIndex; i++)
				SerializeItem(sb, items[i], serializer, needSelection, Properties.Native, VisibleColumns.Count);
			return "[" + sb.ToString().TrimEnd(',') + "]";
		}
		[SecuritySafeCritical] 
		protected void SerializeItem(StringBuilder sb, ListEditItem item, JavaScriptSerializer serializer, bool needSelection, bool isNative, int vcCount) {
			if(needSelection && item.Selected) {
				sb.AppendFormat("[{0}],", serializer.Serialize(item.Value));
			} else {
				sb.AppendFormat("{0},", serializer.Serialize(item.Value));
			}
			if(vcCount != 0 && !isNative) {
				foreach(ListBoxColumn column in VisibleColumns) {
					object value = item.GetValue(column.FieldName);
					string text = value != null ? value.ToString() : "";
					sb.AppendFormat("{0},", serializer.Serialize(text));
				}
			} else {
				sb.AppendFormat("{0},", serializer.Serialize(item.Text != null ? item.Text : ""));
			}
			if(Properties.ImageColumnExists) {
				sb.AppendFormat("{0},", serializer.Serialize(item.ImageUrl != null ? item.ImageUrl : ""));
			}
		}
		protected void SerializeItemTexts(StringBuilder sb, ListEditItem item) {
			foreach(ListBoxColumn column in VisibleColumns) {
				object value = item.GetValue(column.FieldName);
				string text = value != null ? value.ToString() : String.Empty;
				SerializeValue(sb, text);
			}
		}
		protected void SerializeValue(StringBuilder sb, object value) {
			if(value == null) 
				value = "";
			string valueStr = value.ToString();
			sb.Append(valueStr.Length);
			sb.Append(Separator);
			sb.Append(valueStr);
		}
		protected void AdjustItemsRange(ListEditItemCollection items, ref int beginIndex, ref int endIndex) {
			if(beginIndex < 0)
				beginIndex = 0;
			if(endIndex < 0 || endIndex >= items.Count)
				endIndex = items.Count - 1;
		}
		public List<string> DeserializeMultiSelectValues(string serializedValues) {
			List<string> deserializedValues = new List<string>();
			int startPos = 0;
			while(startPos < serializedValues.Length) {
				deserializedValues.Add(ParseString(serializedValues, ref startPos));
			}
			return deserializedValues;
		}
		public List<DeserializedListEditItem> DeserializeItems(string serializedItems, bool sortRequired) {
			List<DeserializedListEditItem> deserializedItems = new List<DeserializedListEditItem>();
			int startPos = 0;
			while(startPos < serializedItems.Length) {
				ListEditItem item = CreateNewUnboundListItem();
				int index = ParseItemIndex(serializedItems, ref startPos);
				item.Value = ParseItemValue(serializedItems, ref startPos);
				item.ImageUrl = ParseString(serializedItems, ref startPos);
				if(VisibleColumns.Count == 0)
					item.Text = ParseString(serializedItems, ref startPos);
				else
					DeserializeItemTexts(item, serializedItems, ref startPos);
				DeserializedListEditItem deserializedItem = new DeserializedListEditItem(item, index);
				deserializedItems.Add(deserializedItem);
			}
			if(sortRequired)
				deserializedItems.Sort(new ClientItemStringComparer());
			return deserializedItems;
		}
		protected void DeserializeItemTexts(ListEditItem item, string serializedItem, ref int startPos) {
			string text;
			foreach(ListBoxColumn column in VisibleColumns) {
				text = ParseString(serializedItem, ref startPos);
				item.SetValue(column.FieldName, text);
			}
		}
		protected object ParseItemValue(string serializedItem, ref int startPos) {
			string valueString = ParseString(serializedItem, ref startPos);
			return CommonUtils.GetConvertedArgumentValue(valueString, ValueType, "value");
		}
		protected ListEditItem CreateNewUnboundListItem() {
			ListEditItem item = new ListEditItem();
			if(!Properties.Columns.IsEmpty) {
				ListEditUnboundDataItemWrapper dataItemWrapper = new ListEditUnboundDataItemWrapper(Properties);
				item.SetDataItemWrapper(dataItemWrapper);
			}
			return item;
		}
	}
	internal class ClientItemStringComparer : IComparer<DeserializedListEditItem> {
		public int Compare(DeserializedListEditItem item1, DeserializedListEditItem item2) {
			return item1.Index.CompareTo(item2.Index);
		}
	}
}
namespace DevExpress.Web.Internal {
	public interface IListEditItemSerializer{
		string SerializeItemsRange(ListEditItemCollection items, int beginIndex, int endIndex);
		string SerializeItemsRange(ListEditItemCollection items, int beginIndex, int endIndex, bool needSelection);
	}
	public interface IListBoxColumnsOwner {
		ListBoxColumnCollection Columns { get; }
	}
	public interface IMultiSelectListEdit {
		SelectedValueCollection SelectedValues { get; }
		ListEditItemCollection Items { get; }
	}
	public interface IListBoxRenderHelperOwner : IListBoxColumnsOwner {
		Page Page { get; }
		List<ListBoxColumn> VisibleColumns { get; }
		ListEditSelectionMode SelectionMode { get; }
		ListEditItemCollection Items { get; }
		EditorImages RenderImages { get;}
		bool DesignMode { get;}
		bool IsNativeRender { get;}
		bool IsClientSideAPIEnabledInternal { get;}
	}
	public class ListBoxRenderHelper {
		private const string ImageCellClassName = "dxeI";
		private const string TextCellClassName = "dxeT";
		private const string ImageMultiColumnCellClassName = "dxeIM";
		private const string FirstTextMultiColumnCellClassName = "dxeFTM";
		private const string TextMultiColumnCellClassName = "dxeTM";
		private const string LastTextMultiColumnCellClassName = "dxeLTM";
		private const string HeaderImageCellClassName = "dxeHIC";
		private const string HeaderFirstTextCellClassName = "dxeHFC";
		private const string HeaderTextCellClassName = "dxeHC";
		private const string HeaderLastTextCellClassName = "dxeHLC";
		private const string CheckCellClassName = "dxeC";
		private const string CheckMultiColumnCellClassName = "dxeCM";
		private const string HeaderCheckCellClassName = "dxeHCC";
		private const string MiddleImageCellClassName = "dxeMI";
		private const string MidleImageMultiColumnCellClassName = "dxeMIM";
		private const string HeaderMidleImageCellClassName = "dxeHMIC";
		private static Unit DefaultColumnWidth = new Unit(120);
		private IListBoxRenderHelperOwner owner;
		private Nullable<bool> imageColumnExists;
		public ListBoxRenderHelper(IListBoxRenderHelperOwner owner) {
			this.owner = owner;
		}
		public Unit CheckBoxWidth {
			get { return Unit.Empty; }
		}
		public Unit ItemImageWidth {
			get { return GetDefaultItemImage().Width; }
		}
		public List<ListBoxColumn> VisibleColumns {
			get { return Owner.VisibleColumns; }
		}
		public bool CheckColumnExists {
			get { return IsCheckColumnExists; }
		}
		public bool IsCheckColumnExists {
			get { return Owner.SelectionMode == ListEditSelectionMode.CheckColumn; }
		}
		public bool ImageColumnExists {
			get {
				if(!imageColumnExists.HasValue)
					imageColumnExists = IsAnyItemHasImage();
				return imageColumnExists.Value;
			}
		}
		public int CheckBoxCellIndex {
			get { return CheckColumnExists ? 0 : -1; }
		}
		public int ImageCellIndex {
			get {
				if(!ImageColumnExists)
					return -1;
				return CheckColumnExists ? 1 : 0;
			}
		}
		public int FirstTextCellIndex {
			get {
				int firstTextCellIndex = 0;
				if(CheckColumnExists)
					firstTextCellIndex++;
				if(ImageColumnExists)
					firstTextCellIndex++;
				return firstTextCellIndex++;
			}
		}
		public int LastTextCellIndex {
			get {
				if(IsMultiColumn)
					return FirstTextCellIndex + VisibleColumns.Count - 1;
				else
					return FirstTextCellIndex;
			}
		}
		protected IListBoxRenderHelperOwner Owner {
			get { return owner; }
		}
		public bool IsRightToLeft {
			get {
				ISkinOwner skinOwner = Owner as ISkinOwner;
				if(skinOwner != null)
					return skinOwner.IsRightToLeft();
				return false;
			}
		}
		public bool IsMultiColumn {
			get { return Owner.Columns.Count > 0; }
		}
		protected bool IsMultiSelect {
			get { return Owner.SelectionMode != ListEditSelectionMode.Single; }
		}
		protected Page Page {
			get { return Owner.Page; }
		}
		private ListEditItem sampleItem;
		protected internal ListEditItem SampleItem {
			get {
				if(sampleItem == null)
					sampleItem = new ListEditItem();
				return sampleItem;
			}
		}
		public string GetColumnCaption(ListBoxColumn column) {
			string caption = column.GetCaption();
			return !string.IsNullOrEmpty(caption) ? caption : "&nbsp;";
		}
		public Unit GetVisibleColumnWidth(int visibleColumnIndex) {
			ListBoxColumn column = VisibleColumns[visibleColumnIndex];
			return GetColumnWidth(column);
		}
		public Unit GetColumnWidth(ListBoxColumn column) {
			return !column.Width.IsEmpty ? column.Width : DefaultColumnWidth;
		}
		public string GetCheckCellClassName() {
			return (IsMultiColumn ? CheckMultiColumnCellClassName : CheckCellClassName) + SystemClassNameSuffix;
		}
		public string GetImageCellClassName() {
			if(IsMultiSelect)
				return (IsMultiColumn ? MidleImageMultiColumnCellClassName : MiddleImageCellClassName) + SystemClassNameSuffix;
			else
				return (IsMultiColumn ? ImageMultiColumnCellClassName : ImageCellClassName) + SystemClassNameSuffix;
		}
		public string GetTextCellClassName(int textCellIndex) {
			if(IsMultiColumn) {
				if(textCellIndex == VisibleColumns.Count - 1)
					return LastTextMultiColumnCellClassName + SystemClassNameSuffix;
				else if(textCellIndex == 0 && !ImageColumnExists && !CheckColumnExists)
					return FirstTextMultiColumnCellClassName + SystemClassNameSuffix;
				else
					return TextMultiColumnCellClassName + SystemClassNameSuffix;
			} else {
				if(ImageColumnExists || CheckColumnExists)
					return TextCellClassName + SystemClassNameSuffix;
				return string.Empty;
			}
		}
		public string GetHeaderCellClassName(int headerCellIndex) {
			if(headerCellIndex == CheckBoxCellIndex)
				return HeaderCheckCellClassName + SystemClassNameSuffix;
			else if(headerCellIndex == ImageCellIndex)
				return (CheckColumnExists ? HeaderMidleImageCellClassName : HeaderImageCellClassName) + SystemClassNameSuffix;
			else if(headerCellIndex == 0 && !CheckColumnExists && !ImageColumnExists)
				return HeaderFirstTextCellClassName + SystemClassNameSuffix;
			else if(headerCellIndex == LastTextCellIndex)
				return HeaderLastTextCellClassName + SystemClassNameSuffix;
			return HeaderTextCellClassName + SystemClassNameSuffix;
		}
		protected string SystemClassNameSuffix {
			get {
				if(IsRightToLeft)
					return "R";
				return String.Empty;
			}
		}
		public void OnCreateControlHierarchy() {
			this.imageColumnExists = null;
		}
		protected internal bool HasSampleItem {
			get { return Owner.IsClientSideAPIEnabledInternal && !Owner.DesignMode && !Owner.IsNativeRender; }
		}
		private bool sampleItemCreated;
		protected internal bool SampleItemCreated {
			get { return sampleItemCreated;}
			set { sampleItemCreated = value; }
		}
		protected internal bool IsSampleItemIndex(int index) {
			return (HasSampleItem && index == -1);
		}
		protected internal bool IsSampleItemHasImage() {
			ImageProperties defaultImage = GetDefaultItemImage();
			return defaultImage.Url != "" || !defaultImage.Height.IsEmpty || !defaultImage.Width.IsEmpty;
		}
		protected internal bool HasDefaultImage() {
			return !GetDefaultItemImage().IsEmpty;
		}
		protected internal bool HasImage(ListEditItem item, bool sampleItem) {
			bool isImageUrlExist = sampleItem ? IsSampleItemHasImage() : !string.IsNullOrEmpty(item.ImageUrl);
			return isImageUrlExist || HasDefaultImage();
		}
		protected bool IsAnyItemHasImage() {
			if(HasImage(SampleItem, true))
				return true;
			foreach(ListEditItem item in Owner.Items) {
				if(HasImage(item, false))
					return true;
			}
			return false;
		}
		internal ImageProperties GetItemImage(ListEditItem item, bool sampleItem) {
			ImageProperties imageProp = null;
			if(HasImage(item, sampleItem)) {
				imageProp = new ImageProperties();
				imageProp.CopyFrom(GetDefaultItemImage());
				if(item.ImageUrl != "")
					imageProp.Url = item.ImageUrl;
			}
			return imageProp;
		}
		internal ImageProperties GetDefaultItemImage() {
			return Owner.RenderImages.GetImageProperties(Page, EditorImages.ListEditItemImageName);
		}
	}
	public class ListEditCallbackArgumentsReader : CallbackArgumentsReader {
		public ListEditCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] { }) {
		}
		public ListEditCallbackArgumentsReader(string arguments, string[] prefixes)
			: base(arguments, prefixes) {
		}
		protected internal static string[] GetMergedStringArray(string[] array1, string[] array2) {
			string[] newArray = new string[array1.Length + array2.Length];
			array1.CopyTo(newArray, 0);
			array2.CopyTo(newArray, array1.Length);
			return newArray;
		}
	}
	public class ListBoxCallbackArgumentsReader : ListEditCallbackArgumentsReader {
		public const string CustomCallbackPrefix = "LECC";
		public const string LoadRangeItemsCallbackPrefix = "LBCRI";
		public ListBoxCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] { LoadRangeItemsCallbackPrefix, CustomCallbackPrefix }) {
		}
		public ListBoxCallbackArgumentsReader(string arguments, string[] prefixes)
			: base(arguments, ListEditCallbackArgumentsReader.GetMergedStringArray(new string[] { LoadRangeItemsCallbackPrefix, CustomCallbackPrefix }, prefixes)) {
		}
		public bool IsLoadRangeItemsCallback { get { return !string.IsNullOrEmpty(BeginEndIndex); } }
		public bool IsCustomCallback { get { return CustomCallbackArg != null; } }
		public string BeginEndIndex { get { return this[LoadRangeItemsCallbackPrefix]; } }
		public string CustomCallbackArg { get { return this[CustomCallbackPrefix]; } }
		public int BeginIndex { get { return int.Parse(BeginEndIndex.Split(':')[0]); } }
		public int EndIndex { get { return int.Parse(BeginEndIndex.Split(':')[1]); } }
	}
	public class ListBoxMultiSelectHelperOwnerProxy : IListEditMultiSelectHelperOwner {
		ASPxListBox listBox;
		public ListBoxMultiSelectHelperOwnerProxy(ASPxListBox listBox) {
			this.listBox = listBox;
		}
		ASPxListBox ListBox {
			get { return listBox; }
		}
		object IEditDataHelperOwner.DataSource { get { return ListBox.DataSource; } }
		string IEditDataHelperOwner.DataSourceID { get { return listBox.DataSourceID; } }
		bool IEditDataHelperOwner.DesignMode { get { return ListBox.DesignMode; } }
		ListEditItemCollection IEditDataHelperOwner.Items { get { return ListBox.Items; } }
		object IEditDataHelperOwner.Value {
			get { return ListBox.Value; }
			set { ListBox.Value = value; }
		}
		Type IEditDataHelperOwner.ValueType { get { return ListBox.ValueType; } }
		bool IEditDataHelperOwner.IsLoading() { return ListBox.IsLoading(); }
		public SelectedValueCollection SelectedValues { get { return ListBox.SelectedValues; } }
		public ListEditSelectionMode SelectionMode {
			get { return ListBox.SelectionMode; }
		}
	}
}
