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

using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Linq;
using System.Windows.Forms.Design;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using System.Drawing.Design;
using DevExpress.Utils.Controls;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Win;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Serializing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Editors;
using DevExpress.XtraVerticalGrid.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraVerticalGrid {
	[Designer("DevExpress.XtraVerticalGrid.Design.PropertyGridDesigner, " + AssemblyInfo.SRAssemblyVertGridDesign, typeof(System.ComponentModel.Design.IDesigner)),
	 DefaultProperty("Rows"), DXToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabData),
	 Description("Displays properties of any object and allows them to be edited.")
]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "PropertyGridControl")]
	public class PropertyGridControl : VGridControlBase, IServiceProvider, DevExpress.Utils.Menu.IDXManagerPopupMenu, IPropertyDescriptorService, IPropertiesEditor {
		public const string FieldNameDelimiter = FieldNameHelper.FieldNameDelimiter;
		PGridDefaultEditors defaultEditors;
		AttributeCollection browsableAttributes;
		class SelectedObjectConverter : ReferenceConverter {
			public SelectedObjectConverter()
				: base(typeof(IComponent)) {
			}
		}
		object[] dataSource;
		bool fAutoGenerateRows;
		RepositoryItemComboBox fComboEditor;
		RepositoryItemButtonEdit fButtonEditor;
		RepositoryItemPopupContainerEdit fPopupEditor;
		RepositoryItemTextEdit fTextEditor;
		readonly WindowsFormsEditorService windowsFormsEditorService;
		readonly Dictionary<string, UITypeEditor> customUITypeEditors;
		int rowLoadingUnlockCount;
		public PropertyGridControl() {
			defaultEditors = new PGridDefaultEditors(this);
			this.LayoutStyle = LayoutViewStyle.SingleRecordView;
			this.windowsFormsEditorService = new WindowsFormsEditorService(this);
			this.customUITypeEditors = new Dictionary<string, UITypeEditor>();
		}
		protected override VGridDataManager CreateDataManager() {
			return new PGridDataManager(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new public CriteriaOperator FilterCriteria { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new public string FilterString { get; set; }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlScrollVisibility")]
#endif
		[DefaultValue(PGridScrollVisibility.Auto)]
		[Category("Layout")]
		[XtraSerializableProperty()]
		[Localizable(true)]
		new public PGridScrollVisibility ScrollVisibility {
			get {
				return Convert(base.ScrollVisibility);
			}
			set {
				 base.ScrollVisibility = ConvertBack(value);
			}
		}
		protected override VGridOptionsView CreateOptionsView() {
			return new PGridOptionsView();
		}
		protected override VGridOptionsBehavior CreateOptionsBehavior() {
			return new PGridOptionsBehavior();
		}
		protected override VGridRows CreateRows() {
			return new PGridVirtualRows(this);
		}
		protected override VGridMenuBase CreateMenu() {
			return new PGridMenu(this);
		}
		new protected internal PGridDataModeHelper DataModeHelper { get { return (PGridDataModeHelper)DataManager.DataModeHelper; } }
		new protected internal PGridEditorContainerHelper EditorHelper { get { return (PGridEditorContainerHelper)base.EditorHelper; } }
		#region IPropertiesEditor
		object IPropertiesEditor.SelectedObject {
			get { return SelectedObject; }
			set { SelectedObject = value; }
		}
		void IPropertiesEditor.InvalidateData() {
			InvalidateData();
		}
		void IPropertiesEditor.InvalidateRows() {
			UpdateRows();
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fGridDisposing = true;
				DisposeAutoEditors();
				ServiceProvider = null;
				SelectedObject = null;
			}
			base.Dispose(disposing);
		}
		void DisposeAutoEditors() {
			Dispose(fComboEditor);
			Dispose(fButtonEditor);
			Dispose(fPopupEditor);
			Dispose(fTextEditor);
		}
		void Dispose(RepositoryItem item) {
			if(item != null) {
				item.Dispose();
				item = null;
			}
		}
		protected override void ActivateDataSourceInternal() {
			if (DataManager == null)
				return;
			DataManager.SetGridDataSource(null, SelectedObjects, null);
			OnDataManager_Reset(DataManager, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlLayoutStyle"),
#endif
 Category("Layout"), Localizable(true),
		DefaultValue(LayoutViewStyle.SingleRecordView), XtraSerializableProperty()]
		public new LayoutViewStyle LayoutStyle {
			get { return base.LayoutStyle; }
			set { base.LayoutStyle = value; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlSelectedObject"),
#endif
 Category("Data"), DefaultValue((string)null), TypeConverter(typeof(SelectedObjectConverter))]
		public object SelectedObject {
			get {
				if(dataSource != null && dataSource.Length != 0)
					return dataSource[0];
				return null;
			}
			set {
				if(value == null)
					SelectedObjects = new object[0];
				else
					SelectedObjects = new object[] { value };
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public object[] SelectedObjects {
			get {
				if(dataSource == null)
					return new object[0];
				return (object[])dataSource.Clone();
			}
			set {
				if(AreEqual(dataSource, value))
					return;
				if(value != null) {
					dataSource = (object[])value.Clone();
				} else {
					dataSource = null;
				}
				if(!(IsLoading || GridDisposing)) {
					CloseEditor();
					if(Site != null && Site.DesignMode && Rows.Count > 0) {
						IUIService srv = (IUIService)Site.GetService(typeof(IUIService));
						if(srv == null || srv.ShowMessage(value == null ? "Do you want to clear the row collection?" : "Do you want to repopulate the row collection?\nWarning: the existing rows will be destroyed.", "Row Designer", MessageBoxButtons.YesNo) == DialogResult.Yes) {
							using(DevExpress.Utils.Design.UndoEngineHelper u = new DevExpress.Utils.Design.UndoEngineHelper(this)) {
								BeginUpdate();
								try {
									Rows.DestroyRows();
								} finally {
									CancelUpdate();
								}
							}
						}
					} else {
						if(AutoGenerateRows) {
							BeginUpdate();
							Rows.DestroyRows();
							CancelUpdate();
						}
					}
				}
				ActivateDataSource();
				RaiseDataSourceChanged();
			}
		}
		bool AreEqual(object[]  dataSource, object[] value) {
			bool equal = false;
			if(value != null && dataSource != null && dataSource.Length == value.Length) {
				int i;
				for(i = 0; i < value.Length; i++) {
					if(dataSource[i] != value[i])
						break;
				}
				if(i == value.Length)
					equal = true;
			}
			return equal;
		}
		void SubscribeComponentEvents(IServiceProvider provider) {
			IComponentChangeService service = PropertyHelper.GetComponentChangeService(provider);
			if(service == null)
				return;
			service.ComponentChanged += OnComponentChanged;
			service.ComponentRemoved += OnComponentRemoved;
		}
		void UnsubscribeComponentEvents(IServiceProvider provider) {
			IComponentChangeService service = PropertyHelper.GetComponentChangeService(provider);
			if(service == null)
				return;
			service.ComponentChanged -= OnComponentChanged;
			service.ComponentRemoved -= OnComponentRemoved;
		}
		void OnComponentRemoved(object sender, ComponentEventArgs e) {
			for(int i = 0; i < SelectedObjects.Length; i++) {
				if(SelectedObjects[i] == e.Component) {
					object[] destinationArray = new object[SelectedObjects.Length - 1];
					Array.Copy(SelectedObjects, 0, destinationArray, 0, i);
					if(i < destinationArray.Length) {
						Array.Copy(SelectedObjects, i + 1, destinationArray, i, destinationArray.Length - i);
					}
					SelectedObjects = destinationArray;
					break;
				}
			}
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			bool contains = false;
			int objectCount = SelectedObjects.Length;
			for(int i = 0; i < objectCount; i++)
				if(SelectedObjects[i] == e.Component) {
					contains = true;
					break;
				}
			if(contains)
				ComponentChanged();
		}
		protected virtual void ComponentChanged() {
			InvalidateData();
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlAutoGenerateRows"),
#endif
 Category("Data"), DefaultValue(false)]
		public bool AutoGenerateRows {
			get { return fAutoGenerateRows; }
			set {
				if(value == fAutoGenerateRows) return;
				fAutoGenerateRows = value;
			}
		}
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerializeCore(this); }
		[
		Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlOptionsMenu")
#else
	Description("")
#endif
		]
		public new PGridOptionsMenu OptionsMenu { get { return (PGridOptionsMenu)base.OptionsMenu; } }
		[Category("Options")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlOptionsBehavior")]
#endif
		public new PGridOptionsBehavior OptionsBehavior { get { return (PGridOptionsBehavior)base.OptionsBehavior; } }
		[Category("Options")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlOptionsView")]
#endif
		public new PGridOptionsView OptionsView { get { return (PGridOptionsView)base.OptionsView; } }
		IServiceProvider serviceProvider;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set {
				if(value == this.serviceProvider)
					return;
				if(this.serviceProvider != null) {
					UnsubscribeComponentEvents(this.serviceProvider);
					this.serviceProvider = null;
				}
				if(value != null) {
					SubscribeComponentEvents(value);
				}
				this.serviceProvider = value;
			}
		}
		internal WindowsFormsEditorService WindowsFormsEditorService { get { return windowsFormsEditorService; } }
		internal void ProcessUIEditing(ButtonPressedEventArgs args) {
			if(args != null && args.Button != null && !args.Button.IsDefaultButton)
				return;
			WindowsFormsEditorService.ProcessUIEditing();
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IWindowsFormsEditorService))
				return windowsFormsEditorService;
			if(serviceType == typeof(IPropertyDescriptorService))
				return this;
			return ServiceProvider != null ? ServiceProvider.GetService(serviceType) : null;
		}
		protected internal virtual bool IsUnlockedRowLoading { get { return rowLoadingUnlockCount != 0; } }
		protected internal virtual void UnlockRowLoading() { rowLoadingUnlockCount++; }
		protected internal virtual void LockRowLoading() { rowLoadingUnlockCount--; }
		protected override EditorContainerHelper CreateHelper() {
			return new PGridEditorContainerHelper(this);
		}
		internal RepositoryItemButtonEdit ButtonEditor {
			get {
				if(fButtonEditor == null)
					fButtonEditor = new PGRepositoryItemButtonEdit();
				return fButtonEditor;
			}
		}
		internal RepositoryItemComboBox ComboEditor {
			get {
				if(fComboEditor == null) {
					fComboEditor = new PGRepositoryItemComboBox();
					fComboEditor.ShowDropDown = ShowDropDown.Never;
					fComboEditor.CycleOnDblClick = true;
				}
				return fComboEditor;
			}
		}
		internal RepositoryItemPopupContainerEdit PopupEditor {
			get {
				if(fPopupEditor == null) {
					fPopupEditor = new PGRepositoryItemPopupContainerEdit();
				}
				return fPopupEditor;
			}
		}
		internal RepositoryItemTextEdit TextEditor {
			get {
				if(fTextEditor == null)
					fTextEditor = new PGRepositoryItemTextEdit();
				return fTextEditor;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public PGridDefaultEditors DefaultEditors { get { return defaultEditors; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AttributeCollection BrowsableAttributes {
			get {
				if(browsableAttributes == null) {
					browsableAttributes = new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes });
				}
				return browsableAttributes;
			}
			set {
				browsableAttributes = value;
				if(AutoGenerateRows) {
					Rows.DestroyRows();
				}
				ActivateDataSource();
			}
		}
		internal protected override bool IsDefault(RepositoryItem item) {
			if(item == null)
				return false;
			return this.defaultEditorMarkers.Contains(item);
		}
		Hashtable defaultEditorMarkers = new Hashtable();
		protected internal override void OnClearViewInfo() {
			base.OnClearViewInfo();
			this.defaultEditorMarkers.Clear();
		}
		protected RepositoryItem MakeDefault(RepositoryItem item) {
			if(item != null) {
				defaultEditorMarkers.Add(item, true);
			}
			return item;
		}
		protected internal override RepositoryItem CreateDefaultRowEdit(RowProperties properties) {
			DescriptorContext context = DataModeHelper.GetDescriptorContext(properties.FieldName);
			PropertyDescriptor propertyDescriptor = context.PropertyDescriptor;
			if(properties.Row == null || propertyDescriptor == null) {
				return MakeDefault((RepositoryItem)base.CreateDefaultRowEdit(properties).Clone());
			}
			if(OptionsBehavior.UseDefaultEditorsCollection) {
				DefaultEditor edit = DefaultEditors[propertyDescriptor.PropertyType];
				if(edit != null && edit.Edit != null)
					return MakeDefault((RepositoryItem)edit.Edit.Clone());
			}
			if (context.NeedsCustomEditorButton && !context.ShouldRenderReadOnly) {
					return MakeDefault((RepositoryItem)ButtonEditor.Clone());
			}
			if(context.NeedsDropDownButton) {
					return MakeDefault((RepositoryItem)PopupEditor.Clone());
			}
			if(PropertyHelper.GetStandardValuesSupported(context)) {
				return MakeDefault((RepositoryItem)ComboEditor.Clone());
			} else
				return MakeDefault((RepositoryItem)TextEditor.Clone());
		}
		protected override void ConfigureRowEdit(RepositoryItem item, RowProperties properties) {
			DescriptorContext context = DataModeHelper.GetDescriptorContext(properties.FieldName);
			if(properties.Row == null || context.PropertyDescriptor == null)
				return;
			item.BeginUpdate();
			bool isDefaultItem = IsDefault(item);
			if(isDefaultItem && !item.ReadOnly) {
				item.ReadOnly = properties.GetReadOnly();
				properties.RenderReadOnly = properties.ReadOnly.HasValue ? properties.ReadOnly.Value : context.ShouldRenderReadOnly;
			}
			RepositoryItemButtonEdit buttonEdit = item as RepositoryItemButtonEdit;
			if(isDefaultItem && buttonEdit != null) {
				buttonEdit.TextEditStyle = PropertyHelper.AllowTextEdit(context) ? TextEditStyles.Standard : TextEditStyles.DisableTextEditor;
			}
			if(IsAutoEditor(item) && PropertyHelper.IsPassword(context)) {
				RepositoryItemTextEdit textEdit = item as RepositoryItemTextEdit;
				if(textEdit != null)
					textEdit.PasswordChar = OptionsView.PasswordChar;
			}
			item.CancelUpdate();
		}
		protected internal virtual bool IsAutoEditor(RepositoryItem item) {
			IAutoEditor edit = item as IAutoEditor;
			if(edit != null)
				return true;
			return false;
		}
		protected override void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			if(e.Name == VGridOptionsBehavior.UseDefaultEditorsCollectionName) {
				LayoutChanged();
			}
			if(e.Name == PGridOptionsBehavior.PropertySortName) {
				ResetVisibleRows();
			}
		}
		protected override void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionsViewChanged(sender, e);
			if(e.Name == PGridOptionsView.PasswordCharName) {
				LayoutChanged();
			}
		}
		protected override void ProcessEditorAfterPost() {
			base.ProcessEditorAfterPost();
			if(FocusedRow == null)
				return;
			EditorHelper.UnmodifiedEditValue = GetCellValue(FocusedRow.GetRowProperties(FocusedRecordCellIndex), FocusedRecord);
		}
		protected internal void RetrieveFieldsCore(VGridRows rows, bool createCategories, string parentPropertyName, bool forceChildRowsCreating, bool autoGenerateRows) {
			if (rows.IsLoadedCore && !autoGenerateRows)
				return;
			rows.IsLoadedCore = true;
			object source = PropertyHelper.GetSingleValue(this, parentPropertyName);
			if(source == null || this.objects.ContainsKey(source))
				return;
			DescriptorContext parentContext = DataModeHelper.GetSingleDescriptorContext(parentPropertyName);
			if(!PropertyHelper.IsRoot(parentPropertyName) && !PropertyHelper.GetPropertiesSupported(source, parentContext))
				return;
			PropertyDescriptorCollection properties = parentContext.GetProperties(source, GetBrowsableAttributesArray());
			if(properties == null || properties.Count == 0)
				return;
			this.objects.Add(source, null);
			Dictionary<string, BaseRow> categories = new Dictionary<string, BaseRow>();
			int retrieveIndex = -1;
			foreach(PropertyDescriptor pd in properties) {
				retrieveIndex++;
				string propertyName = FieldNameHelper.GetFieldName(parentPropertyName, pd.Name);
				if(DataModeHelper.IsMultiSource && !ShouldIncludeMultiProperty(propertyName))
					continue;
				PGridEditorRow row = (PGridEditorRow)CreateEditorRow(propertyName);
				AssignRetrieveIndex(row, retrieveIndex, properties.Count, pd);
				row.Properties.Caption =  GetCaption(pd);
				TryExtractChildren(row, forceChildRowsCreating, propertyName, autoGenerateRows);
				TryAddToCategory(rows, createCategories, categories, pd, row);
			}
			if(createCategories) {
				rows.AddRange(GetCategories(categories));
			}
			this.objects.Remove(source);
		}
		protected override void UpdateDataOnRefresh() {
			UpdateData();
		}
		protected override void UpdateDataCore() {
			if (AutoGenerateRows) {
				UpdateRows();
			}
			else {
				DataModeHelper.Invalidate();
			}
			base.UpdateDataCore();
		}
		protected internal override IComparer VisibleRowsComparer {
			get {
				return new PGridRowComparer(OptionsBehavior.PropertySort == PropertySort.Alphabetical, !OptionsView.ShowRootCategories);
			}
		}
		void AssignRetrieveIndex(BaseRow row, int retrieveIndex, int rowCount, PropertyDescriptor pd) {
			IRetrievable retrievable = row as IRetrievable;
			if(retrievable == null)
				return;
			if(NeedParenthesize(pd)) {
				retrievable.RetrieveIndex = retrieveIndex - rowCount;
			} else
				retrievable.RetrieveIndex = retrieveIndex;
		}
		protected virtual BaseRow CreateEditorRow(string propertyName) {
			BaseRow row = CreateEditorRow();
			row.Properties.FieldName = propertyName;
			return row;
		}
		public override BaseRow CreateEditorRow() {
			return new PGridEditorRow();
		}
		string GetCaption(PropertyDescriptor pd) {
			if(NeedParenthesize(pd)) {
				return ParenthesizeCaption(pd.DisplayName);
			} else
				return pd.DisplayName;
		}
		void TryAddToCategory(VGridRows rows, bool createCategories, Dictionary<string, BaseRow> categories, PropertyDescriptor pd, BaseRow row) {
			if(createCategories) {
				string categoryName = pd.Category;
				BaseRow cat = null;
				if(!categories.TryGetValue(categoryName, out cat)) {
					cat = new CategoryRow(categoryName);
					categories[categoryName] = cat;
				}
				cat.ChildRows.Add(row);
			} else {
				rows.Add(row);
			}
		}
		void TryExtractChildren(PGridEditorRow row, bool forceChildRowsCreating, string propertyName, bool autoGenerateRows) {
			object value = PropertyHelper.GetSingleValue(this, propertyName);
			if(value == null || !PropertyHelper.GetPropertiesSupported(value, DataModeHelper.GetSingleDescriptorContext(propertyName))) {
				row.IsChildRowsLoaded = true;
				return;
			}
			if(forceChildRowsCreating) {
				RetrieveFieldsCore(row.ChildRows, false, propertyName, forceChildRowsCreating, autoGenerateRows);
			} else {
				row.IsChildRowsLoaded = false;
			}
			row.Expanded = false;
		}
		bool ShouldIncludeMultiProperty(string propertyName) {
			if(!DataModeHelper.IsMultiSource) return false;
			return DataModeHelper.GetDescriptorContext(propertyName).PropertyDescriptor != null;
		}
		bool NeedParenthesize(PropertyDescriptor pd) {
			return ((ParenthesizePropertyNameAttribute)pd.Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis;
		}
		BaseRow[] GetCategories(Dictionary<string, BaseRow> categories) {
			List<BaseRow> categoryRows = new List<BaseRow>(categories.Count);
			foreach(BaseRow category in categories.Values) {
				categoryRows.Add(category);
			}
			return categoryRows.ToArray();
		}
		internal Attribute[] GetBrowsableAttributesArray() {
			Attribute[] attributes = new Attribute[BrowsableAttributes.Count];
			BrowsableAttributes.CopyTo(attributes, 0);
			return attributes;
		}
		string ParenthesizeCaption(string caption) {
			return "(" + caption + ")";
		}
		protected internal override void OnRowChanged(BaseRow row, VGridRows rows, RowProperties rowProperties, RowChangeTypeEnum changeType) {
			base.OnRowChanged(row, rows, rowProperties, changeType);
			if(changeType == RowChangeTypeEnum.Value && !IsLoading)
				GetRefreshStrategy(rowProperties, GetCellValue(row, 0)).Refresh();
			DataModeHelper.ChangedImmutableFieldName = null;
		}
		public virtual void UpdateRows() {
			DataModeHelper.Invalidate();
			BaseRow row = GetFirstVisible();
			if (row == null || row.Properties == null)
				return;
			new RefreshAllStrategy(row.Properties) { AutoGenerateRows = true }.Refresh();
		}
		protected virtual RefreshStrategyBase GetRefreshStrategy(RowProperties props, object newValue) {
			if(NeedRefreshRefreshProperty(props)) {
				return new RefreshAllStrategy(props) { AutoGenerateRows = AutoGenerateRows };
			}
			if(PropertyHelper.GetPropertiesSupported(newValue, DataModeHelper.GetDescriptorContext(props.FieldName))) {
				return new RefreshChildrenStrategy(props);
			}
			if(!string.IsNullOrEmpty(DataModeHelper.ChangedImmutableFieldName)) {
				BaseRow row = GetRowByFieldName(DataModeHelper.ChangedImmutableFieldName);
				if(row == null)
					return new VoidRefreshStrategy();
				return new UpdateChildrenDataStrategy(row.Properties);
			}
			return new VoidRefreshStrategy();
		}
		bool NeedRefreshRefreshProperty(RowProperties properties) {
			return AutoGenerateRows && DataModeHelper.HasRefreshPropertiesAttribute(properties);
		}
		protected override bool GenerateRowsOnDataManagerReset() {
			return base.GenerateRowsOnDataManagerReset() && !DesignMode;
		}
		Hashtable objects = new Hashtable();
		public override void RetrieveFields() {
			RetrieveFields(!OptionsBehavior.AllowDynamicRowLoading);
		}
		public void RetrieveFields(bool forceChildRowsCreating) {
			if(SelectedObject == null) return;
			BeginUpdate();
			try {
				DataModeHelper.Invalidate();
				Rows.DestroyRows();
				Rows.IsLoadedCore = false;
				this.objects = new Hashtable();
				RetrieveFieldsCore(Rows, true, PGridDataModeHelper.RootPropertyName, forceChildRowsCreating, AutoGenerateRows);
			} finally {
				EndUpdate();
			}
		}
		internal BaseRow GetLoadedRowByFieldName(string fieldName) {
			return Rows.GetLoadedRowByFieldName(fieldName, true);
		}
		public PropertyDescriptor GetPropertyDescriptor(BaseRow row) {
			object targetObject;
			return GetPropertyDescriptor(row, out targetObject);
		}
		public PropertyDescriptor GetPropertyDescriptor(BaseRow row, out object targetObject) {
			if(row == null)
				throw new ArgumentNullException("row");
			DescriptorContext context = DataModeHelper.GetDescriptorContext(row.Properties.FieldName);
			targetObject = context.Instance;
			return context.PropertyDescriptor;
		}
		internal protected Dictionary<string, UITypeEditor> CustomUITypeEditors { get { return customUITypeEditors; } }
		internal void UpdateEditor() {
			if(ActiveEditor != null)
				ActiveEditor.Properties.Appearance.Assign(UpdateFocusedAppearance());
		}
		AppearanceObject UpdateFocusedAppearance() {
			RowValueInfo valueInfo = ViewInfo[FocusedRow].ValuesInfo[FocusedRecordCellIndex];
			valueInfo.CalcAppearance(ViewInfo);
			return valueInfo.Style;
		}
		protected internal override void RowDoubleClick(BaseRow row) {
			if(row != null && row.HasChildren)
				base.RowDoubleClick(row);
			else {
				if(ActiveEditor == null)
					ShowEditor();
				ComboBoxEdit comboEdit;
				if(ActiveEditor != null && !ActiveEditor.Properties.ReadOnly && (comboEdit = ActiveEditor as ComboBoxEdit) != null) {
					ScrollComboBox(comboEdit);
				}
			}
		}
		protected internal override void UpdateEditViewInfoData(BaseEditViewInfo editViewInfo, RowProperties properties, int recordIndex) {
			base.UpdateEditViewInfoData(editViewInfo, properties, recordIndex);
			if (!OverrideEditorDisplayText(editViewInfo.Item, properties))
				return;
			string displayText = DataModeHelper.GetTextData(properties, recordIndex);
			if (properties.Format.FormatType == FormatType.None && displayText != null)
				editViewInfo.SetDisplayText(displayText);
		}
		protected virtual bool OverrideEditorDisplayText(RepositoryItem item, RowProperties properties) {
			if(properties == null)
				return false;
			bool allow = EditorHelper.OverrideEditorDisplayText(item, properties.FieldName);
			RepositoryItemTextEdit textEdit = item as RepositoryItemTextEdit;
			if(textEdit != null && textEdit.PasswordChar != '\0')
				return false;
			return allow;
		}
		protected virtual void CheckPropertyDescriptors(PropertyDescriptorCollection properties) {
			if(properties == null)
				return;
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				if(propertyDescriptor == null)
					throw new ArgumentNullException("Properties");
			}
		}
		internal bool CanResetDefaultValue(RowProperties properties) {
			return DataModeHelper.CanResetDefaultValue(properties);
		}
		internal void ResetDefaultValue(BaseRow row) {
			DataModeHelper.Reset = true;
			try {
				SetCellValue(row, 0, null);
				if(ActiveEditor != null && row == FocusedRow)
					EditingValue = GetCellValue(row, 0);
			} finally {
				DataModeHelper.Reset = false;
			}
		}
		ScrollVisibility ConvertBack(PGridScrollVisibility scrollVisibility) {
			if (scrollVisibility == PGridScrollVisibility.Never)
				return XtraVerticalGrid.ScrollVisibility.Never;
			if (scrollVisibility == PGridScrollVisibility.Vertical)
				return XtraVerticalGrid.ScrollVisibility.Vertical;
			return XtraVerticalGrid.ScrollVisibility.Auto;
		}
		PGridScrollVisibility Convert(ScrollVisibility scrollVisibility) {
			if (scrollVisibility == XtraVerticalGrid.ScrollVisibility.Never)
				return PGridScrollVisibility.Never;
			if (scrollVisibility == XtraVerticalGrid.ScrollVisibility.Vertical)
				return PGridScrollVisibility.Vertical;
			return PGridScrollVisibility.Auto;
		}
		void ScrollComboBox(ComboBoxEdit comboEdit) {
			int itemsCount = comboEdit.Properties.Items.Count;
			if(itemsCount == 0)
				return;
			comboEdit.IsModified = true;
			comboEdit.SelectedIndex = comboEdit.SelectedIndex >= itemsCount - 1 ? 0 : comboEdit.SelectedIndex + 1;
			PostEditor();
		}
		#region Events raising
		internal protected override void RaiseShowMenu(PopupMenuShowingEventArgs e) {
			RaiseShowMenuCore(e);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			PropertyGridMenuEventHandler handler = (PropertyGridMenuEventHandler)this.Events[GS.showMenu];
			if (handler != null) {
				PropertyGridMenuEventArgs args = new PropertyGridMenuEventArgs(e.Menu, e.Row);
				handler(this, args);
			}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		protected virtual void RaiseCustomPropertyDescriptors(CustomPropertyDescriptorsEventArgs e) {
			CustomPropertyDescriptorsEventHandler handler = (CustomPropertyDescriptorsEventHandler)this.Events[GS.customPropertyDescriptors];
			if(handler != null)
				handler(this, e);
		}
		#endregion Events raising
		#region Events
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlShowMenu"),
#endif
		Category("Behavior")
		]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use 'PopupMenuShowing' instead", false)]
		new public event PropertyGridMenuEventHandler ShowMenu {
			add { Events.AddHandler(GS.showMenu, value); }
			remove { Events.RemoveHandler(GS.showMenu, value); }
		}
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("PropertyGridControlCustomPropertyDescriptors")]
#endif
		[Category("Data")]
		public event CustomPropertyDescriptorsEventHandler CustomPropertyDescriptors {
			add { Events.AddHandler(GS.customPropertyDescriptors, value); }
			remove { Events.RemoveHandler(GS.customPropertyDescriptors, value); }
		}
		#endregion Events
		internal protected virtual void ProcessUIEditingException(Exception e) {
			MessageBox.Show(e.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		internal UITypeEditor GetUITypeEditor(PropertyDescriptor propertyDescriptor) {
			UITypeEditor customEditor;
			if(CustomUITypeEditors.TryGetValue(propertyDescriptor.Name, out customEditor))
				return customEditor;
			try {
				return propertyDescriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			} catch {
				return null;
			}
		}
		protected internal override VGridViewInfoHelper CreateViewInfoHelper() {
			return new PGridViewInfoHelper(this);
		}
		protected override IEnumerable<string> GetExternalSeachColumns() {
			return GetFindColumnNames().Cast<IDataColumnInfo>().Select(p => p.FieldName);
		}
		#region Refresh classes
		protected abstract class RefreshStrategyBase {
			readonly RowProperties rowProperties;
			readonly PropertyGridControl grid;
			public RefreshStrategyBase(RowProperties rowProperties) {
				if(rowProperties == null) return;
				this.rowProperties = rowProperties;
				this.grid = (PropertyGridControl)rowProperties.Row.Grid;
			}
			public PropertyGridControl Grid { get { return grid; } }
			public BaseRow Row { get { return RowProperties.Row; } }
			public RowProperties RowProperties { get { return rowProperties; } }
			public string FieldName { get { return RowProperties.FieldName; } }
			public abstract void Refresh();
		}
		class VoidRefreshStrategy : RefreshStrategyBase {
			public VoidRefreshStrategy() : base(null) { }
			public override void Refresh() { }
		}
		class RefreshAllStrategy : RefreshStrategyBase {
			public RefreshAllStrategy(RowProperties props) : base(props) { }
			public bool AutoGenerateRows { get; set; }
			public override void Refresh() {
				Grid.BeginUpdate();
				if (AutoGenerateRows) {
					SaveExpandedStateOperation saveState = new SaveExpandedStateOperation();
					Grid.RowsIterator.DoOperation(saveState);
					Grid.Rows.DestroyRows();
					Grid.RetrieveFieldsCore(Grid.Rows, true, PropertyHelper.RootPropertyName, false, true);
					LoadExpandedStateOperation loadState = new LoadExpandedStateOperation(saveState.ExpandedRowStateStore);
					Grid.RowsIterator.DoOperation(loadState);
				}
				Grid.EndUpdate();
			}
		}
		class RefreshChildrenStrategy : RefreshStrategyBase {
			public RefreshChildrenStrategy(RowProperties props) : base(props) { }
			public override void Refresh() {
				if(!Row.ChildRows.IsLoaded)
					return;
				OneRowState rowState = new OneRowState(Row);
				Grid.LockEditor();
				Grid.BeginUpdate();
				if (Grid.AutoGenerateRows) {
					Row.ChildRows.DestroyRows();
				}
				Grid.RetrieveFieldsCore(Row.ChildRows, false, FieldName, true, Grid.AutoGenerateRows);
				Grid.EndUpdate();
				Grid.UnlockEditor();
				rowState.Restore(Grid);
			}
		}
		class UpdateChildrenDataStrategy : RefreshStrategyBase {
			public UpdateChildrenDataStrategy(RowProperties props) : base(props) { }
			public override void Refresh() {
				if(!Row.ChildRows.IsLoaded)
					return;
				Grid.BeginUpdate();
				Grid.RowsIterator.DoLocalOperation(new DelegateRowPropertiesOperation(InvalidateRow), Row);
				Grid.EndUpdate();
			}
			void InvalidateRow(RowProperties properties) {
				Grid.ViewInfo.UpdateCellData(properties.Row, 0, properties.CellIndex);
			}
		}
		#endregion refresh classes
		#region IPropertyDescriptorService Members
		PropertyDescriptorCollection IPropertyDescriptorService.GetProperties(object source, ITypeDescriptorContext context, Attribute[] attributes) {
			CustomPropertyDescriptorsEventArgs e = new CustomPropertyDescriptorsEventArgs(source, context, attributes);
			RaiseCustomPropertyDescriptors(e);
			CheckPropertyDescriptors(e.Properties);
			return e.Properties;
		}
		#endregion
		internal protected virtual void ConfigurePredefinedProperties(DefaultEditor edit) {
			RepositoryItemColorEdit colorEditItem = edit.Edit as RepositoryItemColorEdit;
			if(colorEditItem != null && edit.EditingType == typeof(Color)) {
				colorEditItem.TextEditStyle = TextEditStyles.Standard;
			}
		}
		protected internal override bool HighlightHeaders { get { return true; } }
		protected internal override bool CanSkipCategoryRow(CategoryRow categoryRow) {
			return true;
		}
	}
	abstract class RowsStateBase {
		string fieldName;
		bool isExpanded,
			isRestored;
		int topVisibleRowIndex;
		public RowsStateBase(BaseRow row) {
			if(row == null || row.Properties == null) return;
			this.fieldName = row.Properties.FieldName;
			this.isExpanded = row.Expanded;
			this.topVisibleRowIndex = row.Grid.TopVisibleRowIndex;
		}
		protected string FieldName { get { return fieldName; } }
		protected bool IsExpanded { get { return isExpanded; } }
		protected int TopVisibleRowIndex { get { return topVisibleRowIndex; } }
		bool IsRestored { get { return isRestored; } }
		public virtual void Restore(PropertyGridControl grid) {
			if(string.IsNullOrEmpty(FieldName) || IsRestored)
				return;
			if(grid.IsUpdateLocked)
				return;
			try {
				grid.BeginUpdate();
				BaseRow row = grid.Rows.GetRowByFieldName(FieldName, true);
				grid.MakeRowVisible(row);
				grid.TopVisibleRowIndex = GetTopVisibleRowIndex(grid, row);
				if(IsExpanded)
					grid.ExpandRow(row);
			} finally {
				this.isRestored = true;
				grid.EndUpdate();
			}
		}
		protected abstract int GetTopVisibleRowIndex(PropertyGridControl grid, BaseRow row);
	}
	class RowPositionKeeper : RowsStateBase {
		int screenVisibleIndex;
		public RowPositionKeeper(BaseRow row) : base(row) {
			if(row == null || !row.IsConnected) return;
			screenVisibleIndex = row.Grid.VisibleRows.IndexOf(row);
		}
		protected override int GetTopVisibleRowIndex(PropertyGridControl grid, BaseRow row) {
			int currentScreenVisibleIndex = grid.VisibleRows.IndexOf(row);
			return TopVisibleRowIndex + currentScreenVisibleIndex - this.screenVisibleIndex;
		}
	}
	class OneRowState : RowsStateBase {
		public OneRowState(BaseRow row) : base(row) { }
		protected override int GetTopVisibleRowIndex(PropertyGridControl grid, BaseRow row) {
			return TopVisibleRowIndex;
		}
	}
}
