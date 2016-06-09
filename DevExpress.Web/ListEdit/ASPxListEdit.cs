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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public abstract class ListEditProperties : EditProperties {
		private ListEditItemCollection items = null;
		private object dataSource = null;
		private ListEditPropertiesHelper listEditPropertiesHelper = null;
		public ListEditProperties()
			: this(null) {
		}
		public ListEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Bindable(false), Browsable(false), Category("Data"), Themeable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesDataSourceID"),
#endif
		DefaultValue(""), Localizable(false), Themeable(false), Category("Data"), AutoFormatDisable,
		IDReferenceProperty(typeof(DataSourceControl)), NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string DataSourceID {
			get { return GetStringProperty("DataSourceID", ""); }
			set { SetStringProperty("DataSourceID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesDataMember"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, Themeable(false), NotifyParentProperty(true)]
		public string DataMember {
			get { return GetStringProperty("DataMember", ""); }
			set { SetStringProperty("DataMember", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesImageUrlField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set { SetStringProperty("ImageUrlField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesTextField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set { SetStringProperty("TextField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesValueField"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ValueField {
			get { return GetStringProperty("ValueField", ""); }
			set { SetStringProperty("ValueField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesValueType"),
#endif
		DefaultValue(typeof(String)), NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return (Type)GetObjectProperty("ValueType", typeof(String)); }
			set {
				if(ValueType != value) {
					SetObjectProperty("ValueType", typeof(String), value);
					RefreshValues();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public ListEditItemCollection Items {
			get {
				if(items == null)
					items = CreateListEditItemCollection(true);
				return items;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditPropertiesItemImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ItemImage {
			get { return Images.ListEditItem; }
		}
		protected ListEditPropertiesHelper ListEditPropertiesHelper {
			get {
				if(listEditPropertiesHelper == null)
					listEditPropertiesHelper = new ListEditPropertiesHelper(this, Items);
				return listEditPropertiesHelper;
			}
		}
		protected virtual ListEditItem CreateListEditItem() {
			return new ListEditItem();
		}
		protected virtual ListEditItemCollection CreateListEditItemCollection(bool withOwner) {
			return withOwner ? new ListEditItemCollection(this) : new ListEditItemCollection();
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ListEditProperties src = source as ListEditProperties;
				if(src != null) {
					DataSource = src.DataSource;
					DataSourceID = src.DataSourceID;
					DataMember = src.DataMember;
					ImageUrlField = src.ImageUrlField;
					TextField = src.TextField;
					ValueField = src.ValueField;
					Items.Assign(src.Items);
					ValueType = src.ValueType;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override bool IsRequireInplaceBound {
			get { return IsRequireInplaceBoundInternal; }
		}
		protected internal bool IsRequireInplaceBoundInternal {
			get { return !string.IsNullOrEmpty(DataSourceID) || DataSource != null; }
		}
		protected virtual bool ItemCollectionPersistenceInViewStateRequired {
			get { return true; }
		}
		protected override void AssignEditorProperties(ASPxEditBase edit) {
			edit.DataSource = DataSource;
			edit.DataSourceID = DataSourceID;
			edit.DataMember = DataMember;
		}
		protected override void AssignInplaceBoundProperties(ASPxEditBase edit) {
			ASPxListEdit listEdit = (ASPxListEdit)edit;
			Items.Assign(listEdit.Items);
		}
		protected override void AssignInplaceProperties(CreateEditorArgs args) {
			base.AssignInplaceProperties(args);
			if(args.DataType != null && args.DataType != typeof(object))
				ValueType = args.DataType;
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			object convertedValue = ListEditPropertiesHelper.GetConvertedValue(args, ValueType);
			if(!string.IsNullOrEmpty(args.DisplayText) || convertedValue == null)
				return base.CreateDisplayControlInstance(args);
			else
				return ListEditPropertiesHelper.CreateDisplayControlInstance(args, convertedValue, ItemImage, Unit.Empty);
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue))
				return base.GetDisplayTextCore(args, encode);
			else {
				CheckInplaceBound(args.DataType, args.Parent, args.DesignMode);
				object convertedValue = ListEditPropertiesHelper.GetConvertedValue(args, ValueType);
				return ListEditPropertiesHelper.GetDisplayText(args, encode, convertedValue);
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ListEditClientSideEvents(this);
		}
		public override object GetExportValue(CreateDisplayControlArgs args) {
			return null;
		}
		static StateManager emptyStateManager = new StateManager();
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> stateManagesObjects = new List<IStateManager>(base.GetStateManagedObjects());
			if(ItemCollectionPersistenceInViewStateRequired)
				stateManagesObjects.Add(Items);
			else
				stateManagesObjects.Add(emptyStateManager);
			return stateManagesObjects.ToArray();
		}
		protected internal virtual void OnItemsChanged() {
			LayoutChanged();
		}
		protected internal virtual bool? GetItemSelected(ListEditItem item) {
			return null;
		}
		protected internal virtual void OnItemSelectionChanged(ListEditItem item, bool selected) {
		}
		protected internal virtual void OnItemDeleting(ListEditItem item) {
		}
		protected internal virtual void OnItemsCleared() {
		}
		protected internal void ConvertItemTypes() {
			foreach(ListEditItem item in Items)
				item.ConvertValue();
		}
		protected void RefreshValues() {
			foreach(ListEditItem item in Items) {
				if(item.Value != null && item.Value.GetType() != ValueType) 
					item.Value = CommonUtils.ConvertToType(item.Value, ValueType, false);
			}
		}
	}
	[ValidationProperty("Value"), DefaultProperty("SelectedIndex"),
	DefaultEvent("SelectedIndexChanged"), ControlValueProperty("Value"),
	Designer("DevExpress.Web.Design.ASPxListEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxListEdit : ASPxEdit, IEditDataHelperOwner, IValueTypeHolder, IControlDesigner {
		protected internal const string ListEditScriptResourceName = EditScriptsResourcePath + "ListEdit.js";
		private static readonly object EventSelectedIndexChanged = new object();
		private ListEditHelper listEditHelper = null;
		public ASPxListEdit()
			: base() {
		}
		protected ASPxListEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditDataSourceID"),
#endif
		Browsable(true), AutoFormatDisable, Themeable(false),
		EditorBrowsable(EditorBrowsableState.Always)]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditDataSource"),
#endif
		Browsable(true), AutoFormatDisable,
		EditorBrowsable(EditorBrowsableState.Always)]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string ImageUrlField {
			get { return Properties.ImageUrlField; }
			set {
				Properties.ImageUrlField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string TextField {
			get { return Properties.TextField; }
			set {
				Properties.TextField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditValueField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)),
		AutoFormatDisable, Themeable(false)]
		public string ValueField {
			get { return Properties.ValueField; }
			set {
				Properties.ValueField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditValueType"),
#endif
		DefaultValue(typeof(String)), TypeConverter(typeof(ListEditValueTypeTypeConverter)),
		AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return Properties.ValueType; }
			set { Properties.ValueType = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ListEditItemCollection Items {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditSelectedIndex"),
#endif
		DefaultValue(-1), Bindable(true), AutoFormatDisable, Themeable(false)]
		public int SelectedIndex {
			get { return ListEditHelper.GetSelectedIndex(ConvertEmptyStringToNull); }
			set { ListEditHelper.SetSelectedIndex(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ListEditItem SelectedItem {
			get { return ListEditHelper.GetSelectedItem(ConvertEmptyStringToNull); }
			set { ListEditHelper.SetSelectedItem(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxListEditSelectedIndexChanged"),
#endif
		Category("Action")]
		public event EventHandler SelectedIndexChanged
		{
			add { Events.AddHandler(EventSelectedIndexChanged, value); }
			remove { Events.RemoveHandler(EventSelectedIndexChanged, value); }
		}
		protected ListEditHelper ListEditHelper {
			get {
				if(listEditHelper == null)
					listEditHelper = CreateListEditHelper();
				return listEditHelper;
			}
		}
		protected internal new ListEditProperties Properties {
			get { return (ListEditProperties)base.Properties; }
		}
		protected virtual ListEditHelper CreateListEditHelper() {
			return new ListEditHelper(this);
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			if(InplaceMode == EditorInplaceMode.StandAlone)
				ListEditHelper.RestoreSelectedIndex();
		}
		void BeforeDataBinding() {
			ListEditHelper.BeforeDataBinding();
		}
		void AfterDataBinding() {
			ListEditHelper.AfterDataBinding();
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			BeforeDataBinding();
			if(PerformDataBindingCore(dataHelperName, data))
				ResetControlHierarchy();
			AfterDataBinding();
		}
		protected virtual bool PerformDataBindingCore(string dataHelperName, IEnumerable data) {
			return ListEditHelper.PerformDataBinding(data, ValueField, TextField, ImageUrlField);
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected object GetItemClientValue(ListEditItem item) {
			object value = item.Value;
			if(ValueType == typeof(String) && value != null)
				value = HttpUtility.HtmlAttributeEncode(value.ToString());
			return value;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxListEdit), ListEditScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(SelectedIndex != -1)
				stb.Append(localVarName + ".savedSelectedIndex = " + HtmlConvertor.ToScript(GetClientSavedSelectedIndex()) + ";\n");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientListEdit";
		}
		protected virtual int GetClientSavedSelectedIndex() {
			return SelectedIndex;
		}
		protected virtual void OnSelectedIndexChanged(EventArgs e) {
			EventHandler handler = Events[EventSelectedIndexChanged] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override object GetPostBackValue(string key, System.Collections.Specialized.NameValueCollection postCollection) {
			string clientValue = GetClientValue(key, postCollection);
			return ConvertClientValueToValueType(clientValue);
		}
		protected string GetClientValue(string key, System.Collections.Specialized.NameValueCollection postCollection) {
			return GetClientValue(postCollection[key], ValueType);
		}
		protected internal static string GetClientValue(string valueFromRequest, Type valueType) {
			string clientValue = valueFromRequest;
			if(valueType == typeof(string) && !string.IsNullOrEmpty(valueFromRequest))
				clientValue = HttpUtility.HtmlDecode(valueFromRequest);
			return clientValue;
		}
		protected internal object ConvertClientValueToValueType(string clientValue) {
			if(ValueType == typeof(string) && !string.IsNullOrEmpty(clientValue))
				clientValue = HttpUtility.HtmlDecode(clientValue);
			return CommonUtils.GetConvertedArgumentValue(clientValue, ValueType, "clientValue");
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnSelectedIndexChanged(EventArgs.Empty);
		}
		protected internal void ItemsChanged() {
			LayoutChanged();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			ListEditHelper.OnViewStateLoaded();
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ListEditItemsOwner"; } }
	}
}
namespace DevExpress.Web.Internal {
	public class ListEditItemBuilder : ControlBuilder {
		private void InitializeEditorProperties(Type type, IDictionary attribs) {
			object value;
			value = attribs["Value"];
			if(value != null) {
				attribs["ValueString"] = value;
				attribs.Remove("Value");
			}
		}
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			InitializeEditorProperties(type, attribs);
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
	public class ListEditPropertiesHelper {
		private EditProperties properties = null;
		private ListEditItemCollection items = null;
		public ListEditPropertiesHelper(EditProperties properties, ListEditItemCollection items) {
			this.properties = properties;
			this.items = items;
		}
		protected EditProperties Properties {
			get { return properties; }
		}
		protected ListEditItemCollection Items {
			get { return items; }
		}
		public Control CreateDisplayControlInstance(CreateDisplayControlArgs args, object convertedValue,
			ImageProperties itemImage, Unit displayImageSpacing) {
			ListEditDisplayControl control = new ListEditDisplayControl();
			control.Text = RenderUtils.CheckEmptyRenderText(Properties.GetDisplayText(args));
			control.ImageSpacing = displayImageSpacing;
			PrepareDisplayImage(control.ImageProperties, convertedValue, itemImage);
			return control;
		}
		public string GetDisplayText(CreateDisplayControlArgs args, bool encode, object convertedValue) {
			ListEditItem item = Items.FindByValue(convertedValue);
			string text = (item != null) ? item.Text : convertedValue.ToString();
			return encode ? HttpUtility.HtmlEncode(text) : text;
		}
		public object GetConvertedValue(CreateDisplayControlArgs args, Type valueType) {
			object value = null;
			if(args.EditValue != null && !(args.EditValue is DBNull))
				value = CommonUtils.GetConvertedArgumentValue(args.EditValue, valueType, "EditValue");
			return value;
		}
		public void PrepareDisplayImage(ImageProperties image, object value, ImageProperties itemImage) {
			ListEditItem item = Items.FindByValue(value);
			if(item != null) {
				image.Assign(itemImage);
				if(item.ImageUrl != "")
					image.Url = item.ImageUrl;
			}
		}
	}
	public abstract class ListEditSelectionStrategyBase {
		IEditDataHelperOwner dataSettings;
		ListEditHelper listEditHelper;
		int valueDemandedLockCount = 0;
		public ListEditSelectionStrategyBase(IEditDataHelperOwner dataSettings, ListEditHelper listEditHelper) {
			this.dataSettings = dataSettings;
			this.listEditHelper = listEditHelper;
		}
		protected IEditDataHelperOwner DataSettings {
			get { return dataSettings; }
		}
		protected ListEditItemCollection Items {
			get { return DataSettings.Items; }
		}
		protected ListEditHelper ListEditHelper {
			get { return listEditHelper; }
		}
		public abstract void ResetStoredSelectedIndex();
		public abstract void RestoreSelectedIndex();
		public abstract bool? GetItemSelected(ListEditItem item, bool convertEmptyStringToNull);
		public abstract int GetSelectedIndex(bool convertEmptyStringToNull);
		public abstract void SetSelectedIndex(int index);
		public abstract ListEditItem GetSelectedItem(bool convertEmptyStringToNull);
		public abstract void SetSelectedItem(ListEditItem item);
		public abstract void SetAllSelection(bool selected);
		public virtual void OnValueChanged(object value) { }
		public void ValueDemanded(bool convertEmptyStringToNull) {
			if(!IsValueDemandedLocked) {
				LockValueDemanded();
				ValueDemandedCore(convertEmptyStringToNull);
				UnlockValueDemanded();
			}
		}
		public virtual void ValueDemandedCore(bool convertEmptyStringToNull) { }
		public abstract void OnItemSelectionChanged(ListEditItem item, bool selected);
		public virtual void OnItemDeleting(ListEditItem item) { }
		public virtual void OnItemsCleared() { }
		public abstract bool SupportsListEditSelectionMode(ListEditSelectionMode mode);
		public virtual void BeforeDataBinding() { }
		public virtual void AfterDataBinding() { }
		protected void AdjustItemIndexToBeInRange(ref int itemIndex) {
			if(itemIndex >= Items.Count)
				itemIndex = Items.Count - 1;
			else if(itemIndex < -1)
				itemIndex = -1;
		}
		protected bool IsItemIndexInRange(int itemIndex) {
			return itemIndex >= 0 && itemIndex < Items.Count;
		}
		protected bool IsValueDemandedLocked { get { return valueDemandedLockCount > 0; } }
		protected void LockValueDemanded() { valueDemandedLockCount++; }
		protected void UnlockValueDemanded() { valueDemandedLockCount--; }
	}
	public class ListEditSingleSelectStrategy : ListEditSelectionStrategyBase {
		private const int DefaultStoredSelectedIndex = -1;
		private int storedSelectedIndex = DefaultStoredSelectedIndex;
		public ListEditSingleSelectStrategy(IEditDataHelperOwner dataSettings, ListEditHelper listEditHelper)
			: base(dataSettings, listEditHelper) {
		}
		protected int StoredSelectedIndex {
			get { return storedSelectedIndex; }
			set { storedSelectedIndex = value; }
		}
		public override bool? GetItemSelected(ListEditItem item, bool convertEmptyStringToNull) {
			object selectedValue = GetValueCore(convertEmptyStringToNull);
			return CommonUtils.AreEqual(item.Value, selectedValue, convertEmptyStringToNull);
		}
		public override int GetSelectedIndex(bool convertEmptyStringToNull) {
			if(!ListEditHelper.ItemsAreFinal)
				return StoredSelectedIndex;
			ListEditItem item = Items.FindByValue(GetValueCore(convertEmptyStringToNull));
			return item != null ? item.Index : -1;
		}
		public override void SetSelectedIndex(int index) {
			StoredSelectedIndex = index;
			if(ListEditHelper.ItemsAreFinal) {
				if(!ListEditHelper.IsLoading)
					AdjustItemIndexToBeInRange(ref index);
				bool isItemIndexInRange = IsItemIndexInRange(index);
				object value = isItemIndexInRange ? Items[index].Value : null;
				SetValue(value);
				SetValueToMultiSelectArray(value, isItemIndexInRange);
			}
		}
		public override void SetAllSelection(bool selected) {
			throw new NotSupportedException();
		}
		public override void ResetStoredSelectedIndex() {
			StoredSelectedIndex = DefaultStoredSelectedIndex;
		}
		public override void RestoreSelectedIndex() {
			if(StoredSelectedIndex != DefaultStoredSelectedIndex)
				SetSelectedIndex(StoredSelectedIndex);
		}
		public override ListEditItem GetSelectedItem(bool convertEmptyStringToNull) {
			int selectedIndex = GetSelectedIndex(convertEmptyStringToNull);
			return IsItemIndexInRange(selectedIndex) ? Items[selectedIndex] : null;
		}
		public override void SetSelectedItem(ListEditItem item) {
			int index = item != null ? item.Index : -1;
			SetSelectedIndex(index);
		}
		public override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(selected)
				SetValue(item.Value);
			else if(DataSettings.Value == item.Value)
				SetValue(null);
		}
		public override bool SupportsListEditSelectionMode(ListEditSelectionMode mode) {
			return mode == ListEditSelectionMode.Single;
		}
		protected object GetValueCore(bool convertEmptyStringToNull) {
			object value = DataSettings.Value;
			if(value != null && value.ToString() == string.Empty && convertEmptyStringToNull)
				value = null;
			return value;
		}
		public override void OnValueChanged(object value) {
			ListEditItem item = Items.FindByValue(value);
			StoredSelectedIndex = item != null ? item.Index : -1;
			SetValueToMultiSelectArray(value, item != null);
		}
		protected void SetValue(object value) {
			DataSettings.Value = value;
		}
		protected void SetValueToMultiSelectArray(object value, bool itemExists) {
			IListEditMultiSelectHelperOwner multiSelectHelperOwner = DataSettings as IListEditMultiSelectHelperOwner;
			if(multiSelectHelperOwner != null) {
				multiSelectHelperOwner.SelectedValues.ClearSelection();
				if(itemExists)
					multiSelectHelperOwner.SelectedValues.AddInternal(value);
			}
		}
	}
	public class ListEditMultiSelectStrategy : ListEditSelectionStrategyBase {
		int onItemSelectionChangedLockCount = 0;
		int valueLockerCount = 0;
		int? firstSelectedItemIndex = null;
		bool dataBinding;
		public ListEditMultiSelectStrategy(IEditDataHelperOwner dataSettings, ListEditHelper listEditHelper)
			: base(dataSettings, listEditHelper) {
		}
		protected new IListEditMultiSelectHelperOwner DataSettings {
			get { return (IListEditMultiSelectHelperOwner)base.DataSettings; }
		}
		protected SelectedValueCollection SelectedValues {
			get { return DataSettings.SelectedValues; }
		}
		public override bool? GetItemSelected(ListEditItem item, bool convertEmptyStringToNull) {
			object itemValue = item.Value;
			bool itemValueSelected = SelectedValues.Contains(itemValue);
			if(itemValueSelected)
				return true;
			else if(convertEmptyStringToNull) {
				return (itemValue == null && SelectedValues.Contains(""))
					|| (itemValue is string && (string)itemValue == "" && SelectedValues.Contains(null));
			}
			return false;
		}
		public override int GetSelectedIndex(bool convertEmptyStringToNull) {
			ListEditItem firstSelectedItem = GetFirstSelectedItem(convertEmptyStringToNull);
			return firstSelectedItem != null ? firstSelectedItem.Index : -1;
		}
		public override void SetSelectedIndex(int index) {
			ClearSelection();
			if(!ListEditHelper.IsLoading)
				AdjustItemIndexToBeInRange(ref index);
			if(DataSettings.Items.IsIndexValid(index))
				DataSettings.Items[index].Selected = true;
			SetFirstSelectedItem(index);
		}
		public override void ResetStoredSelectedIndex() {
		}
		public override void RestoreSelectedIndex() {
		}
		public override ListEditItem GetSelectedItem(bool convertEmptyStringToNull) {
			return GetFirstSelectedItem(convertEmptyStringToNull);
		}
		public override void SetSelectedItem(ListEditItem item) {
			SetSelectedIndex(Items.IndexOf(item));
		}
		public override void SetAllSelection(bool selected) {
			LockOnItemSelectionChanged();
			ReserFirstSelectedItem();
			SelectedValues.ClearSelection();
			if(selected) {
				foreach(ListEditItem item in DataSettings.Items)
					SelectedValues.AddInternal(item.Value);
			}
			UnlockOnItemSelectionChanged();
		}
		public override void ValueDemandedCore(bool convertEmptyStringToNull) {
			ListEditItem item = GetFirstSelectedItem(convertEmptyStringToNull);
			SetValueCore(item != null ? item.Value : null);
		}
		public override void OnValueChanged(object value) {
			if(!IsValueLocked) {
				SetSelectedIndex(Items.IndexOfValue(value));
				SetValueCore(value);
			}
		}
		public override bool SupportsListEditSelectionMode(ListEditSelectionMode mode) {
			return mode == ListEditSelectionMode.Multiple || mode == ListEditSelectionMode.CheckColumn;
		}
		public override void BeforeDataBinding() {
			dataBinding = true;
		}
		public override void AfterDataBinding() {
			dataBinding = false;
			PurgeSelection();
		}
		protected void PurgeSelection() {
			for(int i = SelectedValues.Count - 1; i >= 0; i--) {
				if(Items.FindByValue(SelectedValues[i]) == null)
					SelectedValues.RemoveInternal(SelectedValues[i]);
			}
		}
		protected void SetValueCore(object value) {
			if(!IsValueLocked) {
				LockValue();
				DataSettings.Value = value;
				UnlockValue();
			}
		}
		protected void ClearSelection() {
			SetAllSelection(false);
		}
		protected bool IsItemSelectionChangedLocked { get { return onItemSelectionChangedLockCount > 0; } }
		protected void LockOnItemSelectionChanged() { onItemSelectionChangedLockCount++; }
		protected void UnlockOnItemSelectionChanged() { onItemSelectionChangedLockCount--; }
		protected bool IsValueLocked { get { return valueLockerCount > 0; } }
		protected void LockValue() { valueLockerCount++; }
		protected void UnlockValue() { valueLockerCount--; }
		protected void SetFirstSelectedItem(int index) {
			firstSelectedItemIndex = index;
		}
		protected void ReserFirstSelectedItem() {
			firstSelectedItemIndex = null;
		}
		protected ListEditItem GetFirstSelectedItem(bool convertEmptyStringToNull) { 
			if(firstSelectedItemIndex != null) {
				return DataSettings.Items.IsIndexValid(firstSelectedItemIndex.Value) ?
					DataSettings.Items[(int)firstSelectedItemIndex] : null;
			} else {
				foreach(ListEditItem item in DataSettings.Items) {
					if(item.Selected)
						return item;
				}
			}
			return null;
		}
		public override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(!IsItemSelectionChangedLocked) {
				bool itemWasSelected = ItemIsSelected(item);
				if(selected && !itemWasSelected)
					SelectedValues.AddInternal(item.Value);
				else if(!selected && itemWasSelected)
					SelectedValues.RemoveInternal(item.Value);
				ReserFirstSelectedItem();
			}
		}
		public override void OnItemDeleting(ListEditItem item) {
			bool itemWasSelected = ItemIsSelected(item);
			if(itemWasSelected)
				SelectedValues.RemoveInternal(item.Value);
		}
		public override void OnItemsCleared() {
			if(!dataBinding)
				SelectedValues.ClearSelection();
		}
		protected bool ItemIsSelected(ListEditItem item) {
			return SelectedValues.Contains(item.Value);
		}
	}
	public interface IEditDataHelperOwner {
		object DataSource { get; }
		string DataSourceID { get; }
		bool DesignMode { get; }
		ListEditItemCollection Items { get; }
		object Value { get; set; }
		Type ValueType { get; }
		bool IsLoading();
	}
	public interface IListEditMultiSelectHelperOwner : IEditDataHelperOwner {
		SelectedValueCollection SelectedValues { get; }
		ListEditSelectionMode SelectionMode { get; }
	}
	public class EditDataHelper {
		private const string
			DefaultValueField = "Value",
			DefaultTextField = "Text";
		private bool
			itemsAsserted = false,
			itemsBound = false,
			itemsLoadedFromViewState = false;
		private IEditDataHelperOwner owner;
		public EditDataHelper(IEditDataHelperOwner owner) {
			this.owner = owner;
		}
		#region Properties
		protected bool DataSourceAssigned {
			get { return !string.IsNullOrEmpty(Owner.DataSourceID) || Owner.DataSource != null; }
		}
		protected bool DesignMode {
			get { return Owner.DesignMode; }
		}
		protected internal bool IsLoading {
			get { return Owner.IsLoading(); }
		}
		protected ListEditItemCollection Items {
			get { return Owner.Items; }
		}
		internal bool ItemsAreFinal {
			get { return !Owner.IsLoading() && !ItemsWillBeBoundLater; }
		}
		protected bool ItemsAsserted {
			get { return itemsAsserted; }
			set { itemsAsserted = value; }
		}
		protected bool ItemsAssigned {
			get { return ItemsBound || ItemsLoadedFromViewState || ItemsAsserted; }
		}
		protected bool ItemsBound {
			get { return itemsBound; }
			set { itemsBound = value; }
		}
		protected bool ItemsLoadedFromViewState {
			get { return itemsLoadedFromViewState; }
			set { itemsLoadedFromViewState = value; }
		}
		protected bool ItemsWillBeBoundLater {
			get { return DataSourceAssigned && !ItemsAssigned; }
		}
		protected IEditDataHelperOwner Owner {
			get { return owner; }
		}
		#endregion
		public void OnItemsAssigned() {
			ItemsAsserted = true;
		}
		public virtual void OnViewStateLoaded() {
			if(DataSourceAssigned)
				ItemsLoadedFromViewState = true;
		}
		public bool PerformDataBindingMulticolumn(IEnumerable data) {
			return PerformDataBinding(data, null, null, null, true);
		}
		public bool PerformDataBinding(IEnumerable data, string valueField, string textField, string imageUrlField) {
			return PerformDataBinding(data, valueField, textField, imageUrlField, false);
		}
		public virtual void RestoreSelectedIndex() {
		}
		private bool PerformDataBinding(IEnumerable data, string valueField, string textField, string imageUrlField,
			bool isMultiColumn) {
			bool bound = false;
			if(data != null) {
				if(!DesignMode && ItemsLoadedFromViewState && !DataSourceAssigned) {
					Items.Clear();
					ItemsBound = false;
				}
				else if(DataSourceAssigned) {
					Items.DataBinding = true;
					if(isMultiColumn)
						DataBindItems(data);
					else
						DataBindItems(data, valueField, textField, imageUrlField);
					ItemsBound = true;
					Items.DataBinding = false;
					RestoreSelectedIndex();
					bound = true;
				}
			}
			return bound;
		}
		public static string GetActualValueFieldName(string valueFieldName, string textFieldName) {
			string actualValueFieldName = String.IsNullOrEmpty(valueFieldName) ? textFieldName : valueFieldName;
			if(string.IsNullOrEmpty(actualValueFieldName))
				actualValueFieldName = DefaultValueField;
			return actualValueFieldName;
		}
		public static string GetActualTextFieldName(string textFieldName) {
			return string.IsNullOrEmpty(textFieldName) ? DefaultTextField : textFieldName;
		}
		public static object GetDataItemValue(object dataItem, string actualValueFieldName, string valueFieldName, string textFieldName, bool designMode) {
			bool fieldFound;
			object value = DataUtils.GetFieldValue(dataItem, actualValueFieldName, !designMode && valueFieldName != "", designMode, out fieldFound);
			if(!fieldFound)
				value = textFieldName == "" && !designMode ? dataItem.ToString() : null;
			return value;
		}
		public static string GetDataItemText(object dataItem, string actualTextFieldName, string textFieldName, bool designMode) {
			bool fieldFound;
			object textObject = DataUtils.GetFieldValue(dataItem, actualTextFieldName, designMode || textFieldName != "", designMode, out fieldFound);
			string text = textObject != null ? textObject.ToString() : "";
			if(!fieldFound)
				text = dataItem.ToString();
			else if(text == null)
				text = "";
			return text;
		}
		public static string GetDataItemImageUrl(object dataItem, string actualImageUrlFieldName, string imageUrlFieldName, bool designMode) {
			return DataUtils.GetFieldValue(dataItem, actualImageUrlFieldName, imageUrlFieldName != "", designMode, "").ToString();
		}
		private void DataBindItems(IEnumerable data, string valueFieldName, string textFieldName, string imageUrlFieldName) {
			string actualValueFieldName = GetActualValueFieldName(valueFieldName, textFieldName);
			string actualTextFieldName = GetActualTextFieldName(textFieldName);
			Items.BeginUpdate();
			Items.Clear();
			foreach(object dataItem in data) {
				AddNewItemToCollection(dataItem,
					GetDataItemText(dataItem, actualTextFieldName, textFieldName, DesignMode),
					GetDataItemValue(dataItem, actualValueFieldName, valueFieldName, textFieldName, DesignMode),
					GetDataItemImageUrl(dataItem, imageUrlFieldName, imageUrlFieldName, DesignMode)
				);
			}
			Items.EndUpdate();
		}
		protected virtual void AddNewItemToCollection(object dataItem, string text, object value, string imageUrl) {
			Items.Add(text, value, imageUrl);
		}
		private void DataBindItems(IEnumerable data) {
			Items.BeginUpdate();
			Items.Clear();
			foreach(object item in data) {
				ListEditItem listItem = Items.Add();
				DataBindItem(listItem, item);
			}
			Items.EndUpdate();
		}
		private void DataBindItem(ListEditItem listItem, object dataItem) {
			ListBoxProperties properties = (ListBoxProperties)Items.Owner;
			ListEditBoundDataItemWrapper dataItemWrapper = new ListEditBoundDataItemWrapper((IMulticolumnListEditDataSettings)properties, dataItem);
			listItem.SetDataItemWrapper(dataItemWrapper);
		}
	}
	public class CheckBoxListHelper : ListEditHelper {
		public CheckBoxListHelper(IEditDataHelperOwner owner)
			: base(owner) {
		}
		public override ListEditSelectionMode DefaultSelectionStrategy {
			get { return ListEditSelectionMode.CheckColumn; }
		}
	}
	public class ListEditHelper : EditDataHelper {
		private ListEditSelectionStrategyBase selectionStrategy = null;
		public ListEditHelper(IEditDataHelperOwner owner)
			: base(owner) {
		}
		public override void OnViewStateLoaded() {
			base.OnViewStateLoaded();
			EnsureSelectionStrategy();
		}
		public virtual ListEditSelectionMode DefaultSelectionStrategy {
			get { return ListEditSelectionMode.Single; }
		}
		protected IListEditMultiSelectHelperOwner MultiSelectSupportsOwner {
			get { return Owner as IListEditMultiSelectHelperOwner; }
		}
		protected ListEditSelectionStrategyBase SelectionStrategy {
			get {
				if(selectionStrategy == null)
					CreateSelectionStrategy(DefaultSelectionStrategy);
				return selectionStrategy;
			}
		}
		public string GetMultiSelectedIndicesArrayScript(string localVarName) {
			List<int> selectedIndices = new List<int>();
			foreach(ListEditItem item in Items) {
				if(item.Selected)
					selectedIndices.Add(item.Index);
			}
			return string.Format("{0}.initSelectedIndices={1};\n", localVarName, HtmlConvertor.ToJSON(selectedIndices));
		}
		#region Selected properties
		public void ValueDemanded(bool convertEmptyStringToNull) {
			SelectionStrategy.ValueDemanded(convertEmptyStringToNull);
		}
		public void OnValueChanged(object value) {
			SelectionStrategy.OnValueChanged(value);
		}
		public int GetSelectedIndex(bool convertEmptyStringToNull) {
			return SelectionStrategy.GetSelectedIndex(convertEmptyStringToNull);
		}
		public void SetSelectedIndex(int index) {
			SelectionStrategy.SetSelectedIndex(index);
		}
		public void ResetStoredSelectedIndex() {
			SelectionStrategy.ResetStoredSelectedIndex();
		}
		public override void RestoreSelectedIndex() {
			SelectionStrategy.RestoreSelectedIndex();
		}
		public bool? GetItemSelected(ListEditItem item, bool convertEmptyStringToNull) {
			return SelectionStrategy.GetItemSelected(item, convertEmptyStringToNull);
		}
		public ListEditItem GetSelectedItem(bool convertEmptyStringToNull) {
			return SelectionStrategy.GetSelectedItem(convertEmptyStringToNull);
		}
		public void SetSelectedItem(ListEditItem item) {
			SelectionStrategy.SetSelectedItem(item);
		}
		public void SetAllSelection(bool selected) {
			SelectionStrategy.SetAllSelection(selected);
		}
		public void OnItemSelectionChanged(ListEditItem item, bool selected) {
			SelectionStrategy.OnItemSelectionChanged(item, selected);
		}
		public void OnItemDeleting(ListEditItem item) {
			SelectionStrategy.OnItemDeleting(item);
		}
		public void OnItemsCleared() {
			SelectionStrategy.OnItemsCleared();
		}
		public void OnSelectionModeChanged(ListEditSelectionMode mode) {
			CreateSelectionStrategy(mode);
		}
		public void BeforeDataBinding() {
			SelectionStrategy.BeforeDataBinding();
		}
		public void AfterDataBinding() {
			SelectionStrategy.AfterDataBinding();
		}
		#endregion
		protected void CreateSelectionStrategy(ListEditSelectionMode mode) {
			if(mode == ListEditSelectionMode.Single)
				this.selectionStrategy = new ListEditSingleSelectStrategy(Owner, this);
			else
				this.selectionStrategy = new ListEditMultiSelectStrategy(Owner, this);
		}
		protected void EnsureSelectionStrategy() {
			IListEditMultiSelectHelperOwner multiSelectSupportsOwner = Owner as IListEditMultiSelectHelperOwner;
			if(multiSelectSupportsOwner != null) {
				if(!SelectionStrategy.SupportsListEditSelectionMode(multiSelectSupportsOwner.SelectionMode))
					CreateSelectionStrategy(multiSelectSupportsOwner.SelectionMode);
			}
		}
	}
}
