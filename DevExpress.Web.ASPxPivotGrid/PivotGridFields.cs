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
using System.Drawing.Design;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.Design;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridField : PivotGridFieldBase, IWebControlObject, IDataSourceViewSchemaAccessor,
			IDisplayNameProvider, IFilterColumn, IDesignTimeCollectionItem {
		ITemplate headerTemplate;
		ITemplate valueTemplate;
		PivotHeaderStyle headerStyle;
		PivotFieldValueStyle valueStyle;
		PivotFieldValueStyle valueTotalStyle;
		PivotCellStyle cellStyle;
		string id;
		bool exportBestFit;
		static DevExpress.PivotGrid.IPivotCalculationCreator CalcCreator = new PivotCalculationCreator();
		static PivotGridField() {
			DevExpress.PivotGrid.ReflectionHelper.AddAssembly(System.Reflection.Assembly.GetExecutingAssembly());
		}
		public PivotGridField()
			: base() {
			Initialize();
		}
		public PivotGridField(PivotGridData data)
			: base(data) {
			Initialize();
		}
		public PivotGridField(string fieldName, PivotArea area)
			: base(fieldName, area) {
			Initialize();
		}
		void Initialize() {
			this.headerTemplate = null;
			this.valueTemplate = null;
			this.headerStyle = new PivotHeaderStyle();
			this.valueStyle = new PivotFieldValueStyle();
			this.valueTotalStyle = new PivotFieldValueStyle();
			this.cellStyle = new PivotCellStyle();
			this.id = string.Empty;
			this.exportBestFit = true;
			TrackViewState();
		}
		protected override PivotGridFieldOptions CreateOptions(PivotOptionsChangedEventHandler eventHandler, string name) {
			return new PivotGridWebFieldOptions(eventHandler, this, name);
		}
		PivotGridWebData WebData { get { return Data != null ? (PivotGridWebData)Data : null; } }
		internal ASPxPivotGrid PivotGrid { get { return WebData != null ? WebData.PivotGrid : null; } }
		protected new PivotGridFieldCollection Collection { get { return (PivotGridFieldCollection)base.Collection; } }
		internal string ClientID {
			get {
				if(!string.IsNullOrEmpty(ID)) return ID;
				return Index.ToString();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldFieldName"),
#endif
 Category("Data"),
		Localizable(true), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true),
		Editor("DevExpress.Utils.Design.EditorLoader, " + AssemblyInfo.SRAssemblyData, "System.Drawing.Design.UITypeEditor, System.Drawing"),
		EditorLoader("DevExpress.Data.Browsing.Design.WebColumnNameEditor", AssemblyInfo.SRAssemblyDesign)
		]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
#pragma warning disable 1691
#pragma warning disable 809
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("Use the ID property instead.")]
		public override string Name {
			get { return ID; }
			set { ID = value; }
		}
#pragma warning restore 809
#pragma warning restore 1691
		protected override string ComponentName {
			get { return ClientID; }
		}
		protected override PivotGridFieldBase GetFieldFromComponentName(string componentName) {
			return Collection.GetFieldByClientID(componentName);
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldID"),
#endif
		Category("Data"), DefaultValue(""), XtraSerializableProperty(), Localizable(false), NotifyParentProperty(true)]
		public string ID {
			get { return id; }
			set {
				if(value == null) 
					value = string.Empty;
				if(FieldName == value && value != string.Empty) {
					if(!IsDeserializing) 
						throw new Exception(FieldNameEqualsNameExceptionString);
					else 
						value = NamePrefix + value;
				}
				string oldId = id;
				id = value;
				if(Collection != null)
					Collection.OnNameChanged(this, oldId);
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldAllowedAreas"),
#endif
 Category("Behaviour"),
		DefaultValue(PivotGridAllowedAreas.All), XtraSerializableProperty(), NotifyParentProperty(true),
		Editor(typeof(EditorLoader), typeof(UITypeEditor)),
		EditorLoader("DevExpress.Utils.Editors.AttributesEditor", AssemblyInfo.SRAssemblyUtils),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Web.ASPxPivotGrid.PivotGridField.AllowedAreas")]
		public new PivotGridAllowedAreas AllowedAreas { get { return base.AllowedAreas; } set { base.AllowedAreas = value; } }
		void ResetOptions() { Options.Reset(); }
		bool ShouldSerializeOptions() { return Options.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldOptions"),
#endif
 Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new PivotGridWebFieldOptions Options { get { return (PivotGridWebFieldOptions)base.Options; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldCellStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(LayoutIds.LayoutIdViewState)]
		public PivotCellStyle CellStyle { get { return cellStyle; } }
		int groupIndex = -1;
		int innerGroupIndex = -1;
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldGroupIndex"),
#endif
		PersistenceMode(PersistenceMode.Attribute),
		DefaultValue(-1),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int GroupIndex {
			get {
				if(IsDataDeserializing && groupIndex != -1) return groupIndex;
				return base.GroupIndex;
			}
			set { groupIndex = value; }
		}
		internal int GroupIndexCore { get { return groupIndex; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldInnerGroupIndex"),
#endif
		PersistenceMode(PersistenceMode.Attribute),
		DefaultValue(-1),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int InnerGroupIndex {
			get {
				if(IsDataDeserializing && innerGroupIndex != -1) return innerGroupIndex;
				return base.InnerGroupIndex; 
			}
			set { innerGroupIndex = value; }
		}
		internal int InnerGroupIndexCore { get { return innerGroupIndex; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldHeaderStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(LayoutIds.LayoutIdViewState)]
		public PivotHeaderStyle HeaderStyle { get { return headerStyle; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(LayoutIds.LayoutIdViewState)]
		public PivotFieldValueStyle ValueStyle { get { return valueStyle; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTotalStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(LayoutIds.LayoutIdViewState)]
		public PivotFieldValueStyle ValueTotalStyle { get { return valueTotalStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsTextOnly { get { return !ShowFilterButton && !(Visible && ShowSortButton); } }
		protected override bool IsDataDeserializing {
			get {
				return base.IsDataDeserializing || (Data != null && Data.IsLoading);
			}
		}
		protected override bool CanFilterBySummary {
			get { return false; }
		}
		[
		Category("Data"), NotifyParentProperty(true),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new IList<PivotCalculationBase> Calculations {
			get { return base.Calculations; }
		}
		protected override DevExpress.PivotGrid.IPivotCalculationCreator CalculationCreator {
			get { return CalcCreator; }
		}
		bool ShouldSerializeCalculationSerializable() {
			return Calculations.Count > 0;
		}
	 	[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridHeaderTemplateContainer))]
		public ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {
				if(HeaderTemplate == value) return;
				headerTemplate = value;
				RaiseTemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridFieldValueTemplateContainer))]
		public ITemplate ValueTemplate {
			get { return valueTemplate; }
			set {
				if(ValueTemplate == value) return;
				valueTemplate = value;
				RaiseTemplatesChanged();
			}
		}
		void RaiseTemplatesChanged() {
			if(WebData != null) WebData.TemplatesChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties FilterButtonImage {
			get {
				if(WebData == null || PivotGrid == null || !ShowFilterButton) return null;
				if (Group != null && Group.IsFilterAllowed)
					return GetHeaderFilterImage(!Group.FilterValues.IsEmpty);
				return GetHeaderFilterImage(!FilterValues.IsEmpty);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties DeferFilterButtonImage {
			get {
				if(WebData == null || PivotGrid == null || !ShowFilterButton) return null;
				if(Group != null && Group.IsFilterAllowed)
					return GetHeaderFilterImage(HasDefereFilter || !Group.FilterValues.IsEmpty);
				return GetHeaderFilterImage(HasDefereFilter || !FilterValues.IsEmpty);
			}
		}
		ImageProperties GetHeaderFilterImage(bool isActive) {
			return isActive ? PivotGrid.RenderHelper.GetHeaderActiveFilterImage() : PivotGrid.RenderHelper.GetHeaderFilterImage();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageProperties SortImage {
			get {
				if(!(Visible && ShowSortButton) || Data == null || PivotGrid == null) return null;
				return PivotGrid.RenderHelper.GetHeaderSortImage(SortOrder);
			}
		}
		void ResetCustomTotals() { CustomTotals.Clear(); }
		bool ShouldSerializeCustomTotals() { return CustomTotals.Count > 0; }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldCustomTotals"),
#endif
 AutoFormatDisable,
		Category("Behaviour"), MergableProperty(false), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.DefaultValue),
		Editor("DevExpress.XtraPivotGrid.Design.CustomTotalsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridDesign,
			"System.Drawing.Design.UITypeEditor, System.Drawing")
		]
		public new PivotGridCustomTotalCollection CustomTotals {
			get { return (PivotGridCustomTotalCollection)base.CustomTotals; }
		}		
		protected override PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollection(this);
		}
		[
		Category("Export"), DefaultValue(100),
		NotifyParentProperty(true), Localizable(true),
		XtraSerializablePropertyId(LayoutIdAppearance), XtraSerializableProperty(100),
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldWidth"),
#endif
		]
		public override int Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
		Category("Export"), DefaultValue(20),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		NotifyParentProperty(true), Localizable(true),
		XtraSerializablePropertyId(LayoutIdAppearance), XtraSerializableProperty(10),
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldMinWidth")
#else
	Description("")
#endif
		]
		public override int MinWidth {
			get { return base.MinWidth; }
			set { base.MinWidth = value; }
		}
		[
		Category("Export"), DefaultValue(true),
		NotifyParentProperty(true), Localizable(false),
		XtraSerializablePropertyId(LayoutIdAppearance), XtraSerializableProperty(10),
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldExportBestFit")
#else
	Description("")
#endif
		]
		public bool ExportBestFit {
			get { return exportBestFit; }
			set { exportBestFit = value; }
		}
		public override void Assign(PivotGridFieldBase source) {
			base.Assign(source);
			PivotGridField field = source as PivotGridField;
			if (field != null) {
				GroupIndex = field.GroupIndex;
				InnerGroupIndex = field.InnerGroupIndex;
				CellStyle.CopyFrom(field.CellStyle);
				HeaderStyle.CopyFrom(field.HeaderStyle);
				ValueStyle.CopyFrom(field.ValueStyle);
				ValueTotalStyle.CopyFrom(field.ValueTotalStyle);
				CustomTotals.Capacity = field.CustomTotals.Capacity;
				ExportBestFit = field.ExportBestFit;
				GroupIntervalNumericRange = field.GroupIntervalNumericRange;
				UseDecimalValuesForMaxMinSummary = field.UseDecimalValuesForMaxMinSummary;
			}
		}
		protected override bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(WebData.IsInternalSerialization) {
				PivotGridWebOptionsLayout opts = options as PivotGridWebOptionsLayout;
				if(opts != null && id == LayoutIds.LayoutIdViewState)
					return opts.StoreViewStateOptions;
				return true;
			}
			return base.OnAllowSerializationProperty(options, propertyName, id);;
		}
		#region IWebControlObject Members
		bool IWebControlObject.IsDesignMode() {
			return WebData.IsDesignMode;
		}
		bool IWebControlObject.IsLoading() {
			return WebData.IsLoading;
		}
		bool IWebControlObject.IsRendering() {
			return WebData.IsRendering;
		}
		void IWebControlObject.LayoutChanged() {
			WebData.LayoutChanged();
		}
		void IWebControlObject.TemplatesChanged() {
			WebData.TemplatesChanged();
		}
		#endregion
		#region IDataSourceViewSchemaAccessor Members
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				if(PivotGrid != null && !String.IsNullOrEmpty(PivotGrid.OLAPConnectionString)) {
					WebData.OLAPDataProvider = PivotGrid.OLAPDataProvider;
					WebData.OLAPConnectionString = PivotGrid.OLAPConnectionString;
					return WebData.GetDesignOLAPDataSourceObject();
				} else {
					IDataSourceViewSchemaAccessor acessor = WebData.DataSourceViewSchemaAccessor;
					return acessor != null ? acessor.DataSourceViewSchema : null;
				}
			}
			set { ; }
		}
		#endregion
		internal void TrackViewState() {
			((IStateManager)CellStyle).TrackViewState();
			((IStateManager)HeaderStyle).TrackViewState();
			((IStateManager)ValueStyle).TrackViewState();
			((IStateManager)ValueTotalStyle).TrackViewState();
		}
		protected override void OnWidthChanged() { }
		#region IDisplayNameProvider Members
		string IDisplayNameProvider.GetDataSourceDisplayName() {
			if(WebData != null && WebData.PivotGrid != null)
				return WebData.PivotGrid.DataSourceID;
			else
				return string.Empty;
		}
		string IDisplayNameProvider.GetFieldDisplayName(string[] fieldAccessors) {
			return fieldAccessors[fieldAccessors.Length - 1];
		}
		#endregion
		#region IFilterColumn Members
		FilterColumnClauseClass IFilterColumn.ClauseClass {
			get {
				if(ActualDataType == typeof(string))
					return FilterColumnClauseClass.String;
				if(ActualDataType == typeof(DateTime))
					return FilterColumnClauseClass.DateTime;
				return FilterColumnClauseClass.Generic;
			}
		}
		string IFilterablePropertyInfo.DisplayName {
			get { return ToString(); }
		}
		EditPropertiesBase propertiesEdit;
		EditPropertiesBase IFilterColumn.PropertiesEdit {
			get {
				if(propertiesEdit == null) {
					if(ActualDataType == typeof(DateTime)) {
						propertiesEdit = new DateEditProperties();
					} else {
						ComboBoxProperties edit = new ComboBoxProperties();
						edit.ValueType = ActualDataType;
						object[] values = GetUniqueValues();
						foreach(object value in values) {
							edit.Items.Add(GetDisplayText(value), value);
						}
						edit.DropDownStyle = DropDownStyle.DropDown;
						edit.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
						if(MvcUtils.RenderMode == MvcRenderMode.None)
							edit.EnableCallbackMode = edit.Items.Count >= 1000;
						propertiesEdit = edit;
					}
				}
				return propertiesEdit;
			}
		}
		protected internal void ClearCaches() {
			propertiesEdit = null;
		}
		string IFilterablePropertyInfo.PropertyName {
			get { return PrefilterColumnName; }
		}
		Type IFilterablePropertyInfo.PropertyType {
			get { return ActualDataType; }
		}
		#endregion
		string IDesignTimeCollectionItem.Caption {
			get {
				var result = ToString();
				return !string.IsNullOrEmpty(result) ? result : GetType().Name;
			}
		}
		PropertiesBase IDesignTimeCollectionItem.EditorProperties { get { return this.propertiesEdit; } }
		IList IDesignTimeCollectionItem.Items { get { return null; } }
		IDesignTimeCollectionItem IDesignTimeCollectionItem.Parent { get { return Group as IDesignTimeCollectionItem; } }
		bool IDesignTimeCollectionItem.ReadOnly { get { return false; } }
		int IDesignTimeCollectionItem.VisibleIndex {
			get { return AreaIndex; }
			set { AreaIndex = value; }
		}
		void IDesignTimeCollectionItem.Assign(IDesignTimeCollectionItem item) {
			Assign((PivotGridFieldBase)item);
		}
		string[] IDesignTimeCollectionItem.GetHiddenPropertyNames() {
			return new string[] { };
		}
	}
	public class PivotGridFieldCollection : PivotGridFieldCollectionBase {
		public PivotGridFieldCollection(PivotGridData data)
			: base(data) {
		}
		public new PivotGridField this[int index] { get { return base[index] as PivotGridField; } }
		[Browsable(false)]
		public new PivotGridField this[string id_fieldName_Caption] {
			get {
				foreach(PivotGridField field in this) {
					if(field.ID == id_fieldName_Caption) return field;
				}
				foreach(PivotGridField field in this) {
					if(field.FieldName == id_fieldName_Caption) return field;
				}
				foreach(PivotGridField field in this) {
					if(field.Caption == id_fieldName_Caption) return field;
				}
				return null;
			}
		}
		public void AddField(PivotGridField field) {
			AddCore(field);
		}
		public void Add(PivotGridField field) {
			AddCore(field);
		}
		public new PivotGridField Add(string fieldName, PivotArea area) {
			return base.Add(fieldName, area) as PivotGridField;
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			PivotGridField field = new PivotGridField(fieldName, area);
			if(!Data.IsLoading && string.IsNullOrEmpty(field.Name))
				field.ID = GenerateName(field.FieldName);
			return field;
		}
		protected override bool IsNameOccupied(string name) {
			return this[name] != null;
		}
		protected internal void ClearCaches() {
			foreach(PivotGridField field in this) {
				field.ClearCaches();
			}
		}
		public PivotGridFieldBase GetFieldByClientID(string clientID) {
			for(int i = 0; i < Count; i++) {
				if(this[i].ClientID == clientID)
					return this[i];
			}
			return null;
		}
		internal void ForceUpdateAreaIndexes() {
			UpdateAreaIndexes(Data.DataField.Visible ? Data.DataField : null, true);
		}
	}
	public class PivotGridCustomTotal : PivotGridCustomTotalBase {
		public PivotGridCustomTotal() : base() { }
		public PivotGridCustomTotal(PivotSummaryType summaryType) : base(summaryType) { }
	}
	public class PivotGridCustomTotalCollection : PivotGridCustomTotalCollectionBase {
		public PivotGridCustomTotalCollection() { }
		public PivotGridCustomTotalCollection(PivotGridFieldBase field) : base(field) { }
		public PivotGridCustomTotalCollection(PivotGridCustomTotalBase[] totals)
			: base(totals) {}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCustomTotalCollectionItem")]
#endif
		public new PivotGridCustomTotal this[int index] {
			get { return (PivotGridCustomTotal)base[index]; }
		}
		protected override PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotal();
		}
	}
	public class PivotGridWebGroup : PivotGridGroup, IDesignTimeCollectionItem {
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridWebGroupFields"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, false, true)]
		public new IList Fields { get { return base.Fields; } }
		internal void SortFields() {
			Sort(new FieldGroupComparer());
		}
		internal PivotGridFieldBase GetFieldByName(string name) {
			foreach(PivotGridFieldBase field in this) {
				if(field.Name == name) return field;
			}
			return null;
		}
		class FieldGroupComparer : IComparer<PivotGridFieldBase> {
			public int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
				return Comparer.Default.Compare(((PivotGridField)x).InnerGroupIndexCore, ((PivotGridField)y).InnerGroupIndexCore);
			}
		}
		string IDesignTimeCollectionItem.Caption {
			get {
				var result = ToString();
				return !string.IsNullOrEmpty(result) ? result : GetType().Name;
			}
		}
		PropertiesBase IDesignTimeCollectionItem.EditorProperties { get { return null; } }
		string IDesignTimeCollectionItem.FieldName { get { return string.Empty; } set { } }
		IList IDesignTimeCollectionItem.Items { get { return Fields; } }
		IDesignTimeCollectionItem IDesignTimeCollectionItem.Parent { get { return null; } }
		bool IDesignTimeCollectionItem.ReadOnly { get { return false; } }
		bool IDesignTimeCollectionItem.Visible { get { return Visible; } set { } }
		int IDesignTimeCollectionItem.VisibleIndex {
			get {
				if(Fields.Count == 0)
					return -1 * ((Collection != null ? Collection.Count : 0) - Index);
				return ((IDesignTimeCollectionItem)Fields[0]).VisibleIndex;
			}
			set {
				if(Fields.Count != 0)
					((IDesignTimeCollectionItem)Fields[0]).VisibleIndex = value;
			}
		}
		void IDesignTimeCollectionItem.Assign(IDesignTimeCollectionItem item) { }
		string[] IDesignTimeCollectionItem.GetHiddenPropertyNames() { 
			return new string[] { }; 
		}
	}
	public class PivotGridWebGroupCollection : PivotGridGroupCollection {
		public PivotGridWebGroupCollection(PivotGridData data) : base(data) { }
		public new PivotGridWebGroup this[int index] { get { return (PivotGridWebGroup)InnerList[index]; } }
		protected override PivotGridGroup CreateGroup() {
			return new PivotGridWebGroup();
		}
	}
}
