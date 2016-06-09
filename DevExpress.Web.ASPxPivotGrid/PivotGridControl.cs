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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.Printing;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.PivotGrid.ServerMode.Queryable;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.ASPxPivotGrid.Internal;
using DevExpress.Web.Design;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid {
	[
	DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxPivotGrid"),
	Designer("DevExpress.Web.ASPxPivotGrid.Design.PivotGridControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ASPxPivotGrid), "ASPxPivotGrid.bmp"), 
	DisplayName("PivotGrid (DevExpress)")
]
	public class ASPxPivotGrid : ASPxDataWebControl, IPivotGridEventsImplementor, IViewBagOwner,
		IXtraSerializable, IRequiresLoadPostDataControl, IDataSource, IPopupFilterControlOwner,
		IPopupFilterControlStyleOwner, IXtraSerializableLayoutEx, IMasterControl, ISupportsCallbackResult,
		ISupportsFieldsCustomization, IXtraSupportDeserializeCollection, IXtraSupportDeserializeCollectionItem,
		IXtraSerializableLayout, IHeaderFilterPopupOwner, IPagerOwner, IControlDesigner {
		protected const string fieldsPropertyName = "Fields", groupsPropertyName = "Groups";
		string olapColumnsState, olapFieldValueState, olapSessionID;
		int initialRowsPerPage; 
		PivotGridWebData data;
		ScriptHelper scriptHelper;
		readonly CallbackState callbackState;
		bool enableCallBacks;
		bool isPrefilterPopupVisible;
		PivotCollapsedStateStoreMode collapsedStateStoreMode;
		bool enableRowsCache;
		bool addCustomJSPropertiesScript = false;
		bool canUnlockRefreshAfterDataBind = false;
		IXtraPropertyCollection firstSnapshot;
		TableCell mainTD;
		Table tableContainer;
		PivotGridHtmlTable mainTable;
		Table captionTable;
		PivotGridHtmlCustomizationFieldsPopup customizationFields;
		PivotWebFilterControlPopup prefilterPopup;
		ASPxPivotGridPopupMenu headerMenu;
		ASPxPivotGridPopupMenu fieldValueMenu;
		ASPxPivotGridPopupMenu fieldListMenu;
		Image arrowUpImage, arrowDownImage, arrowRightImage, arrowLeftImage;
		Image dragHideFieldImage;
		Image groupSeparatorImage;
		PivotGridImages images;
		PivotGridPagerStyles pagerStyles;
		WebPrintAppearance printStyles;
		EditorStyles stylesEditors;
		FilterControlStyles stylesFilterControl;
		PivotKPIImages kpiImages;
		FilterControlImages imagesPrefilterControl;
		EditorImages imagesEditors;
		ASPxPivotGridRenderHelper renderHelper;
		PivotGridHtmlFilterPopupContentBase filterPopupContentControl;
		PivotGridMasterControlDefaultImplementation masterControlImplementation;
		PivotGridPostBackActionBase postbackAction;
		bool isPivotDataCanDoRefresh, requireDataUpdate,
			isOurCallbackPostback, isFilterPopupCallBack, isDefereFilterCallback, isVirtualScrollingCallback;
		GridLayoutEventHelper gridLayoutEventHelper;
		private static readonly object customUnboundFieldData = new object();
		private static readonly object customSummary = new object();
		private static readonly object customFieldSort = new object();
		private static readonly object customServerModeSort = new object();
		private static readonly object customGroupInterval = new object();
		private static readonly object fieldAreaChanged = new object();
		private static readonly object fieldAreaIndexChanged = new object();
		private static readonly object fieldVisibleChanged = new object();
		private static readonly object fieldFilterChanged = new object();
		private static readonly object fieldFilterChanging = new object();
		private static readonly object fieldPropertyChanged = new object();
		private static readonly object pageIndexChanged = new object();
		private static readonly object fieldValueCollapsed = new object();
		private static readonly object fieldValueExpanded = new object();
		private static readonly object fieldValueNotExpanded = new object();
		private static readonly object fieldValueCollapsing = new object();
		private static readonly object fieldValueExpanding = new object();
		private static readonly object fieldExpandedInFieldGroupChanged = new object();
		private static readonly object fieldValueDisplayText = new object();
		private static readonly object customChartDataSourceData = new object();
		private static readonly object customChartDataSourceRows = new object();
		private static readonly object controlHierarchyCreated = new object();
		private static readonly object customCellDisplayText = new object();
		private static readonly object customCellValue = new object();
		private static readonly object customFilterPopupItems = new object();
		private static readonly object customFieldValueCells = new object();
		private static readonly object customCellStyle = new object();
		private static readonly object addPopupMenuItem = new object();
		private static readonly object popupMenuCreated = new object();
		private static readonly object dataAreaPopupCreated = new object();
		private static readonly object olapQueryTimeout = new object();
		private static readonly object olapException = new object();
		private static readonly object queryException = new object();
		private static readonly object customCallback = new object();
		private static readonly object afterPerformCallback = new object();
		private static readonly object layoutChanged = new object();
		private static readonly object beforePerformDataSelect = new object();
		private static readonly object customFilterExpressionDisplayText = new object();
		private static readonly object filterControlOperationVisibility = new object();
		private static readonly object filterControlParseValue = new object();
		private static readonly object filterControlCustomValueDisplayText = new object();
		private static readonly object prefilterCriteriaChanged = new object();
		private static readonly object htmlCellPrepared = new object();
		private static readonly object htmlFieldValuePrepared = new object();
		private static readonly object customSaveCallbackState = new object();
		private static readonly object customLoadCallbackState = new object();
		private static readonly object beforeLoadLayout = new object();
		private static readonly object beginRefresh = new object();
		private static readonly object endRefresh = new object();
		private static readonly object dataSourceChanged = new object();
		private static readonly object fieldAreaChanging = new object();
		private static readonly object fieldUnboundExpressionChanged = new object();
		private static readonly object groupFilterChanged = new object();
		private static readonly object layoutUpgrade = new object();
		public ASPxPivotGrid() {
			this.callbackState = new CallbackState();
			this.EnableCallBacksInternal = true;
			this.enableCallBacks = true;
			this.enableRowsCache = true;
			this.collapsedStateStoreMode = PivotCollapsedStateStoreMode.Indexes;
			this.customizationFields = null;
			this.masterControlImplementation = new PivotGridMasterControlDefaultImplementation(this);
			this.images = new PivotGridImages(this);
			this.imagesPrefilterControl = new FilterControlImages(this);
			this.imagesEditors = new EditorImages(this);
			this.pagerStyles = new PivotGridPagerStyles(this);
			this.printStyles = CreatePrintStyles();
			this.stylesEditors = new EditorStyles(this);
			this.stylesFilterControl = new FilterControlStyles(this);
			FilterControlCachedColumns = new FilterControlColumnCollection(this);
			CreateCustomizationFormImages();
			OptionsChartDataSource.OptionsChanged += OnOptionsChartDataSourceChanged;
			PivotGridLocalizer.ActiveChanged += OnActiveLocalizerChanged;
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		protected virtual WebPrintAppearance CreatePrintStyles() {
			return new WebPrintAppearance();
		}
		protected internal CallbackState CallbackState { get { return callbackState; } }
		protected internal bool RequireDataUpdate {
			get { return requireDataUpdate; }
			set {
				if(Data.IsDeserializing || requireDataUpdate == value) return;
				requireDataUpdate = value;
				if(value)
					ResetControlHierarchy();
			}
		}
		internal PivotGridMasterControlDefaultImplementation MasterControlImplementation {
			get { return masterControlImplementation; }
		}
		protected virtual bool IsPostBack {
			get {
				return Page != null && Page.IsPostBack;
			}
		}
		internal new bool IsTrackingViewState { get { return base.IsTrackingViewState; } }
#if DEBUGTEST
		internal
#endif
 protected string OLAPColumnsState {
			get { return olapColumnsState; }
#if DEBUGTEST
			internal
#else
			private
#endif
 set { olapColumnsState = value; }
		}
#if DEBUGTEST
		internal
#endif
 protected string OLAPFieldValueState {
			get { return olapFieldValueState; }
#if DEBUGTEST
			internal
#else
			private
#endif
 set { olapFieldValueState = value; }
		}
		internal protected string OLAPSessionID {
			get { return olapSessionID; }
#if DEBUGTEST
			internal
#else
			private
#endif
 set { olapSessionID = value; }
		}
		public override void Dispose() {
			PivotGridLocalizer.ActiveChanged -= OnActiveLocalizerChanged;
			OptionsChartDataSource.OptionsChanged -= OnOptionsChartDataSourceChanged;
			data.Dispose();
			this.masterControlImplementation = null;
			base.Dispose();
		}
		static ASPxPivotGrid() {
			SetLocalizer();
		}
		static void SetLocalizer() {
			DefaultActiveLocalizerProvider<PivotGridStringId> defaultProvider = PivotGridLocalizer.GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<PivotGridStringId>;
			if(defaultProvider == null || defaultProvider.GetActiveLocalizer().GetType() == typeof(PivotGridResLocalizer))
				PivotGridLocalizer.SetActiveLocalizerProvider(CreateResLocalizerProvider());
		}
		internal static ASPxActiveLocalizerProvider<PivotGridStringId> CreateResLocalizerProvider() {
			return new ASPxActiveLocalizerProvider<PivotGridStringId>(CreateResLocalizerInstance);
		}
		static XtraLocalizer<PivotGridStringId> CreateResLocalizerInstance() {
			return new ASPxPivotGridResLocalizer();
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridFields"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), DefaultValue(null), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIds.LayoutIdColumns)]
		public PivotGridFieldCollection Fields { get { return Data != null ? Data.Fields : null; } }
		protected void XtraClearFields(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				Fields.ClearAndDispose();
				return;
			}
			OptionsLayoutGrid gridOptions = e.Options as OptionsLayoutGrid;
			bool addNewColumns = (gridOptions != null && gridOptions.Columns.AddNewColumns);
			List<PivotGridField> commonFields = GetCommonFields(e.Item.ChildProperties);
			List<PivotGridField> newFields = GetNewFields(commonFields);
			if(!addNewColumns) {
				RemoveNewFields(newFields);
			} else {
				SetNew(newFields);
			}
		}
		List<PivotGridField> GetNewFields(List<PivotGridField> savingFields) {
			List<PivotGridField> newFields = new List<PivotGridField>();
			for(int i = Fields.Count - 1; i >= 0; i--) {
				PivotGridField field = Fields[i];
				if(!savingFields.Contains(field))
					newFields.Add(field);
			}
			return newFields;
		}
		void SetNew(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				field.IsNew = true;
		}
		List<PivotGridField> GetCommonFields(IXtraPropertyCollection layoutItems) {
			List<PivotGridField> commonFields = new List<PivotGridField>();
			foreach(XtraPropertyInfo pInfo in layoutItems) {
				PivotGridField commonField = XtraFindFieldsItem(new XtraItemEventArgs(this, Fields, pInfo)) as PivotGridField;
				if(commonField != null)
					commonFields.Add(commonField);
			}
			return commonFields;
		}
		void RemoveNewFields(List<PivotGridField> newFields) {
			foreach(PivotGridField field in newFields)
				Fields.Remove(field);
		}
		protected object XtraCreateFieldsItem(XtraItemEventArgs e) {
			OptionsLayoutGrid gridOptions = e.Options as OptionsLayoutGrid;
			if(gridOptions != null && gridOptions.Columns.RemoveOldColumns)
				return null;
			return Fields.Add();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraFindFieldsItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["ID"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(string.IsNullOrEmpty(name)) return null;
			PivotGridField field = Fields[name];
			return field;
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			EnsureRefreshData();
			return Data.GetDrillDownDataSource(-1, -1, 0);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex) {
			EnsureRefreshData();
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, dataIndex, false);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, false);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			EnsureRefreshData();
			return VisualItems.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns, false);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.CreateSummaryDataSource(columnIndex, rowIndex, false);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			EnsureRefreshData();
			return Data.CreateSummaryDataSource();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridWebData Data {
			get {
				if(data == null)
					data = CreateData();
				return data;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ScriptHelper ScriptHelper {
			get {
				if(scriptHelper == null)
					scriptHelper = CreateScriptHelper();
				return scriptHelper;
			}
		}
		protected PivotWebVisualItems VisualItems {
			get { return Data.VisualItems; }
		}
		protected internal PivotGridPostBackActionBase PostBackAction {
			get { return postbackAction; }
			set {
				if(postbackAction == value) return;
				postbackAction = value;
				if(postbackAction != null && postbackAction.RequireDataUpdate) {
					RequireDataUpdate = true;
					UpdateDataBoundAndChildControls(false);
				}
			}
		}
		protected internal ASPxPivotGridRenderHelper RenderHelper {
			get {
				if(renderHelper == null)
					renderHelper = CreateRenderHelper();
				return renderHelper;
			}
		}
		protected FilterControlColumnCollection FilterControlCachedColumns { get; private set; }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridIsDataBound"),
#endif
 Category("Data"), Browsable(false)]
		public bool IsDataBound { get { return Data.IsDataBound; } }
		protected internal PivotGridHtmlTable MainTable { get { return mainTable; } }
		protected WebControl MainTD { get { return mainTD; } }
		internal Table TableContainer { get { return tableContainer; } }
		Table CaptionTable { get { return captionTable; } }
		internal WebControl CallbackContainer { get { return MainTD; } }
		protected internal PivotGridHtmlCustomizationFieldsPopup CustomizationFields {
			get {
				if(customizationFields == null) {
					customizationFields = CreateCustomizationFieldsPopup();
					if(fieldListState != null) {
						customizationFields.ApplyLayoutState(fieldListState);
						fieldListState = null;
					}
				}
				return customizationFields;
			}
		}
		void ResetRelatedCustomizationFormHierarchy() {
			if(!MasterControlImplementation.ContainsRelatedCustomizationForm)
				return;
			Data.ResetFieldListFields();
			MasterControlImplementation.CustomizationForm.ResetContentControlHierarchy();
		}
		internal PivotGridHtmlCustomizationFields CustomizationFieldsInternal {
			get {
				if(MasterControlImplementation.ContainsRelatedCustomizationForm)
					return MasterControlImplementation.CustomizationForm.CustomizationFields;
				return CustomizationFields.CustomizationFields;
			}
		}
		protected virtual PivotGridHtmlCustomizationFieldsPopup CreateCustomizationFieldsPopup() {
			return new PivotGridHtmlCustomizationFieldsPopup(this);
		}
		protected internal PivotGridHtmlFilterPopupContentBase FilterPopupContentControl { get { return filterPopupContentControl; } }
		protected Image ArrowUpImage { get { return arrowUpImage; } }
		protected Image ArrowDownImage { get { return arrowDownImage; } }
		protected Image ArrowRightImage { get { return arrowRightImage; } }
		protected Image ArrowLeftImage { get { return arrowLeftImage; } }
		protected Image DragHideFieldImage { get { return dragHideFieldImage; } }
		protected Image GroupSeparatorImage { get { return groupSeparatorImage; } }
		protected PivotWebFilterControlPopup PrefilterPopup { get { return prefilterPopup; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOLAPConnectionString"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"), Browsable(true), AutoFormatDisable(), Localizable(false),
		Editor("DevExpress.Web.ASPxPivotGrid.Design.OLAPConnectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string OLAPConnectionString {
			get { return GetStringProperty("OLAPConnectionString", ""); }
			set {
				string oldValue = OLAPConnectionString;
				if(OLAPConnectionString == value) return;
				SetStringProperty("OLAPConnectionString", "", value);
				if(!string.IsNullOrEmpty(value)) {
					DataSource = null;
					DataSourceID = string.Empty;
					OLAPColumnsState = null;
					if(!string.IsNullOrEmpty(oldValue))
						OLAPFieldValueState = null;
				}
				OnDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOLAPDataProvider"),
#endif
		DefaultValue(OLAPDataProvider.Default), Themeable(false), Category("Data"), Browsable(true), AutoFormatDisable(), Localizable(false),
		]
		public OLAPDataProvider OLAPDataProvider {
			get { return (OLAPDataProvider)GetEnumProperty("OLAPDataProvider", OLAPDataProvider.Default); }
			set {
				if(OLAPDataProvider == value)
					return;
				SetEnumProperty("OLAPDataProvider", OLAPDataProvider.Default, value);
				OnDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridDataSource"),
#endif
		Bindable(false), Category("Data"), Themeable(false),
		DefaultValue(null), DesignerSerializationVisibility(0), AutoFormatDisable
		]
		public override object DataSource {
			get { return base.DataSource; }
			set {
				if(DataSource == value) return;
				base.DataSource = value;
				if(value != null) {
					OLAPConnectionString = "";
				}
				if(value is IQueryDataSource || IsQueryableDataSource(value)) {
					Data.PivotDataSource = null;
					Data.ListSource = FakeData;
				}
				else if(value != null && (!string.IsNullOrEmpty(OLAPColumnsState) || !string.IsNullOrEmpty(OLAPFieldValueState))) {
					OLAPColumnsState = null;
					OLAPFieldValueState = null;
					Data.WebFieldValuesStateCache = null;
				}
				OnDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridDataSourceID"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"),
		AutoFormatDisable, Localizable(false)
		]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set {
				if(DataSourceID == value) return;
				string oldValue = DataSourceID;
				base.DataSourceID = value;
				if(!string.IsNullOrEmpty(value)) {
					OLAPConnectionString = "";
					if(!string.IsNullOrEmpty(oldValue)) { 
						Data.WebFieldValuesStateCache = null;
					}
					OnDataSourceChanged();
				}
			}
		}
		protected bool IsOLAP { get { return !String.IsNullOrEmpty(OLAPConnectionString); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridIsPrefilterPopupVisible"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable,
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue)
		]
		public bool IsPrefilterPopupVisible {
			get { return isPrefilterPopupVisible; }
			set {
				if(IsPrefilterPopupVisible == value) return;
				isPrefilterPopupVisible = value;
				ResetControlHierarchy();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCaption"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridSummaryText"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable,
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SummaryText {
			get { return GetStringProperty("SummaryText", ""); }
			set {
				if(value == SummaryText) return;
				if(value != null)
					value = value.Replace('\n', ' ').Replace("\r", "");
				SetStringProperty("SummaryText", "", value);
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridClientInstanceName"),
#endif
 AutoFormatDisable,
		Category("Client-Side"), DefaultValue(""), Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnableCallBacks"),
#endif
		DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableCallBacks {
			get { return enableCallBacks; }
			set { enableCallBacks = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnablePagingCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public bool EnablePagingCallbackAnimation {
			get { return EnableSlideCallbackAnimationInternal; }
			set { EnableSlideCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnablePagingGestures"),
#endif
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnablePagingGestures {
			get { return EnableSwipeGesturesInternal; }
			set { EnableSwipeGesturesInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEnableRowsCache"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableRowsCache {
			get { return enableRowsCache; }
			set { enableRowsCache = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCollapsedStateStoreMode"),
#endif
		Category("Behavior"), DefaultValue(PivotCollapsedStateStoreMode.Indexes), AutoFormatDisable
		]
		public PivotCollapsedStateStoreMode CollapsedStateStoreMode {
			get { return collapsedStateStoreMode; }
			set { collapsedStateStoreMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false), XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(LayoutIds.ClientSideEvents)
		]
		public PivotGridClientSideEvents ClientSideEvents {
			get { return (PivotGridClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PivotGridClientSideEvents();
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridSaveStateToCookies"),
#endif
 Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridSaveStateToCookiesID"),
#endif
 Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public override bool EncodeHtml {
			get { return GetBoolProperty("EncodeHtml", false); }
			set { SetBoolProperty("EncodeHtml", false, value); }
		}
		protected override void LoadClientState(string state) {
			LoadLayoutFromString(state, PivotGridWebOptionsLayout.DefaultLayout);
		}
		protected override string SaveClientState() {
			return SaveLayoutToString(PivotGridWebOptionsLayout.DefaultLayout);
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new ASPxPivotGridDataHelper(this, name);
		}
		protected virtual ASPxPivotGridRenderHelper CreateRenderHelper() {
			return new ASPxPivotGridRenderHelper(this);
		}
		protected virtual PivotGridWebData CreateData() {
			return new PivotGridWebData(this);
		}
		protected virtual ScriptHelper CreateScriptHelper() {
			return new ScriptHelper(this);
		}
		object[] fieldListState = null;
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainTable = null;
			if(this.customizationFields != null)
				fieldListState = CustomizationFields.GetLayoutState();
			this.customizationFields = null;
			this.mainTD = null;
			this.prefilterPopup = null;
			this.headerMenu = null;
			this.fieldValueMenu = null;
			this.arrowUpImage = null;
			this.arrowDownImage = null;
			this.arrowRightImage = null;
			this.arrowLeftImage = null;
			this.dragHideFieldImage = null;
		}
		public override bool PreRendered {
			get { return false; }
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			InitialPageSize = OptionsPager.RowsPerPage;
			if(Page != null)
				Page.PreRenderComplete += new EventHandler(OnPreRenderComplete);
			RegisterRequiresViewStateEncryption();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			RegisterRequiresViewStateEncryption();
		}
		bool ShouldEncryptViewState() {
			return !String.IsNullOrEmpty(OLAPConnectionString);
		}
		void RegisterRequiresViewStateEncryption() {
			if(ShouldEncryptViewState()) {
				if(Page != null) {
					if(Page.ViewStateEncryptionMode == ViewStateEncryptionMode.Never)
						throw new Exception("The ASPxPivotGrid.OLAPConnectionString property cannot be specified when the ViewStateEncryptionMode property is set to ViewStateEncryptionMode.Never.");
					Page.RegisterRequiresViewStateEncryption();
				}
			}
		}
		new void OnPreRenderComplete(object sender, EventArgs e) {
			EnsureFieldListCreated();
		}
		protected override void FinalizeCreateControlHierarchy() {
			base.FinalizeCreateControlHierarchy();
			EnsureFieldListCreated();
		}
		void EnsureFieldListCreated() {
			if(DesignMode) return;
			if(!ContainsCustomizationFieldsForm) {
				if(MainTD != null && MainTD.Controls.IndexOf(CustomizationFields) < 0)
					MainTD.Controls.Add(CustomizationFields);
			}
		}
		protected override void CreateControlHierarchy() {
			if(!IsFilterPopupCallBack && IsPrefilterPopupVisible && IsPostBack && !IsCallback)
				DataBindIfNecessary();
			RefreshData();
			if(IsFilterPopupCallBack) {
				base.CreateControlHierarchy();
				return;
			}
			this.mainTable = new PivotGridHtmlTable(this);
			MainTable.ID = ElementNames.MainTable;
			this.mainTD = RenderUtils.CreateTableCell();
			MainTD.ID = ElementNames.MainTD;
			MainTD.Controls.Add(MainTable);
			this.tableContainer = RenderUtils.CreateTable();
			RenderUtils.SetStyleStringAttribute(TableContainer, "border-collapse", "separate");
			TableContainer.Rows.Add(CreateCaptionRow());
			TableContainer.Rows.Add(RenderUtils.CreateTableRow());
			TableContainer.Rows[1].Cells.Add(this.mainTD);
			TableContainer.Rows[1].Style.Add(HtmlTextWriterStyle.Height, "100%");
			Controls.Add(TableContainer);
			base.CreateControlHierarchy();
			if(this.Enabled && !DesignMode)
				AddFilterPopup();
			PreparePrefilterPopup();
			if(!DesignMode && IsEnabled()) {
				if(OptionsView.ShowContextMenus) {
					this.headerMenu = CreateHeaderMenu();
					this.fieldValueMenu = CreateFieldValueMenu();
					MainTD.Controls.Add(this.headerMenu);
					MainTD.Controls.Add(this.fieldValueMenu);
				}
				this.fieldListMenu = CreateFieldListMenu();
				MainTD.Controls.Add(this.fieldListMenu);
			}
			ResetRelatedCustomizationFormHierarchy();
			if(Page == null || IsRendering)
				EnsureFieldListCreated();
			this.arrowUpImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragDownImage, PivotGridImages.DragArrowDownName);
			this.arrowDownImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragUpImage, PivotGridImages.DragArrowUpName);
			this.arrowRightImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragRightImage, PivotGridImages.DragArrowRightName);
			this.arrowLeftImage = CreateHiddenImage(PivotGridImages.ElementName_ArrowDragLeftImage, PivotGridImages.DragArrowLeftName);
			this.dragHideFieldImage = CreateHiddenImage(PivotGridImages.ElementName_DragHideFieldImage, PivotGridImages.DragHideFieldName);
			this.groupSeparatorImage = CreateHiddenImage(PivotGridImages.ElementName_GroupSeparatorImage, PivotGridImages.GroupSeparatorName);
			Controls.Add(ArrowUpImage);
			Controls.Add(ArrowDownImage);
			Controls.Add(ArrowRightImage);
			Controls.Add(ArrowLeftImage);
			Controls.Add(DragHideFieldImage);
			Controls.Add(GroupSeparatorImage);
		}
		TableRow CreateCaptionRow() {
			TableRow captionRow = RenderUtils.CreateTableRow();
			this.captionTable = RenderUtils.CreateTable();
			TableCell captionCell = RenderUtils.CreateTableCell();
			captionCell.Controls.Add(this.captionTable);
			captionRow.Cells.Add(captionCell);
			return captionRow;
		}
		WebControl CreateHorzScrollMainDiv() {
			WebControl mainDiv = RenderUtils.CreateDiv();
			mainDiv.ID = ElementNames.ScrollMainDiv;
			mainDiv.Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
			return mainDiv;
		}
		HeaderFilterPopup AddFilterPopup() {
			HeaderFilterPopup fp = new HeaderFilterPopup(this);
			this.Controls.Add(fp);
			return fp;
		}
		void PreparePrefilterPopup() {
			if(!IsPrefilterPopupVisible || DesignMode) return;
			this.prefilterPopup = CreatePrefilterPopup();
			MainTD.Controls.Add(PrefilterPopup);
			PrefilterPopup.EnableViewState = false;
		}
		protected virtual PivotWebFilterControlPopup CreatePrefilterPopup() {
			return new PivotWebFilterControlPopup(this);
		}
		bool isDataBinding;
		internal bool IsDataBinding { get { return isDataBinding; } }
		bool IPivotGridEventsImplementor.IsDataBindNecessary {
			get { return IsDataBindNecessaryInternal; }
		}
		bool IsDataBindNecessaryInternal {
			get { return !Data.IsDataBound && !isDataBinding; }
		}
		protected void DataBindIfNecessary() {
			if(IsDataBindNecessaryInternal) {
				isDataBinding = true;
				DataBind();
				isDataBinding = false;
				if(Data.IsDataBound && !RequireDataUpdate && !string.IsNullOrEmpty(Data.WebFieldValuesStateCache)) {
					Data.RestoreWebCollapsedState(Data.WebFieldValuesStateCache);
					Data.WebFieldValuesStateCache = null;
					ResetControlHierarchy();
				}
				CorrectPageIndex();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void EnsureRefreshData() {
			DataBindIfNecessary();
			bool changeIsLoading = !Initialized && canUnlockRefreshAfterDataBind;
			if(changeIsLoading)
				Data.SetIsLoading(false);
			bool needUpdate = RequireDataUpdate;
			RefreshData(true);
			if(changeIsLoading)
				Data.SetIsLoading(null);
			if(needUpdate != RequireDataUpdate) {
				ResetControlHierarchy();
				canUnlockRefreshAfterDataBind = false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDataSourceEmpty {
			get {
				return DataSource == null && string.IsNullOrEmpty(DataSourceID)
					  && string.IsNullOrEmpty(OLAPConnectionString);
			}
		}
		IList fakeData;
		protected IList FakeData {
			get {
				if(fakeData == null)
					fakeData = new ArrayList();
				return fakeData;
			}
		}
		protected bool IsFakeData {
			get { return Data.ListSource == FakeData; }
		}
		protected internal void BindFakeDataIfNecessary() {
			if(!Data.IsDataBound && IsDataSourceEmpty) {
				Data.ListSource = FakeData;
			}
		}
		protected override void OnPreRender(EventArgs e) {
			EnsureDataBoundCore();
			base.OnPreRender(e);
			RegisterRequiresViewStateEncryption();
		}
		protected override void BeforeRender() {
			if(!PreRendered && !DesignMode)
				EnsureDataBoundCore();
			base.BeforeRender();
		}
		void EnsureDataBoundCore() {
			if(!IsPostBack && !IsCallback) {
				EnsureRefreshDataInternal();
			} else {
				if(!IsOurCallbackPostback) {
					if(!IsDataSourceEmpty) {
						if(RequireDataUpdate || !EnableRowsCache)
							EnsureRefreshData();
						HasContentInternal = true;
					} else {
						BindFakeDataIfNecessary();
						ResetControlHierarchy();
					}
				}
			}
		}
		void EnsureRefreshDataInternal() {
			BindFakeDataIfNecessary();
			EnsureRefreshData();
			HasContentInternal = true;
		}
		protected override void EnsureChildControlsRecursive(Control control, bool skipContentContainers) {
			base.EnsureChildControlsRecursive(control, skipContentContainers);
			RaiseControlHierarchyCreated(new EventArgs());
		}
		protected override void PrepareControlHierarchy() {
			if(HasContentInternal) {
				PrepareTableContainer();
				RenderHelper.GetDragArrowUpImage().AssignToControl(ArrowDownImage, DesignMode);
				RenderHelper.GetDragArrowDownImage().AssignToControl(ArrowUpImage, DesignMode);
				RenderHelper.GetDragArrowRightImage().AssignToControl(ArrowRightImage, DesignMode);
				RenderHelper.GetDragArrowLeftImage().AssignToControl(ArrowLeftImage, DesignMode);
				RenderHelper.GetDragHideFieldImage().AssignToControl(DragHideFieldImage, DesignMode);
				RenderHelper.GetGroupSeparatorImage().AssignToControl(GroupSeparatorImage, DesignMode);
			}
			base.PrepareControlHierarchy();
		}
		protected virtual void PrepareTableContainer() {
			RenderUtils.AssignAttributes(this, TableContainer);
			TableContainer.CellPadding = 0;
			TableContainer.CellSpacing = 0;
			TableContainer.BorderWidth = 0;
			GetControlStyle().AssignToControl(TableContainer);
			PrepareCaptionTable();
			Styles.ApplyContainerCellStyle(MainTD);
		}
		void PrepareCaptionTable() {
			Data.GetTableStyle().AssignToControl(CaptionTable);
			CaptionTable.Caption = Caption;
			if(string.IsNullOrEmpty(Caption))
				CaptionTable.Visible = false;
			CaptionTable.Width = Unit.Percentage(100);
			CaptionTable.Height = Unit.Percentage(100);
		}
		protected override void PrepareLoadingDiv(WebControl loadingDiv) {
			base.PrepareLoadingDiv(loadingDiv);
			if(LoadingDiv != null && IsEnabled()) {
				RenderUtils.SetAttribute(LoadingDiv, "onmousemove", "ASPx.pivotGrid_ClearSelection();", "");
			}
		}
		protected override void ResetControlHierarchy() {
			RenderHelper.ResetMenus();
			FilterControlCachedColumns.Clear();
			this.masterControlImplementation.RemoveInnerRelatedControls();
			base.ResetControlHierarchy();
		}
		protected virtual string GetCallbackStateFieldValue() {
			string result;
			if(RaiseLoadCallbackState(out result)) return result;
			return GetClientObjectStateValueString(ClientStateProperties.CallbackState);
		}
		protected internal virtual string SaveCallbackState(bool internalSaving, bool saveCachedState) {
			callbackState.Clear();
			IXtraPropertyCollection toSerialize = GetCallbackStateProperties(internalSaving);
			callbackState[CallbackState.SerializedLayoutName] = SerializeSnapshot(toSerialize);
			callbackState[CallbackState.SavedFilterValuesIsEmptyName] = Data.SaveFilterValuesIsEmptyState();
			callbackState[CallbackState.CollapsedStateName] = Data.SaveWebCollapsedState();
			callbackState[CallbackState.SavedDenyExpandStateName] = Data.SaveDenyExpandState();
			callbackState[CallbackState.SavedOLAPSessionID] = Data.OLAPSessionID;
			if(saveCachedState) {
				callbackState[CallbackState.SavedFieldValuesName] = VisualItems.SaveFieldValueItemsState();
				callbackState[CallbackState.SavedDataCellsName] = VisualItems.SaveDataCellsState();
				callbackState[CallbackState.SavedFieldDataTypesName] = Data.SaveFieldDataTypes();
				if(IsOLAP || HasQueryDataSource || HasQueryPivotDataSource) {
					callbackState[CallbackState.SavedOLAPColumnsName] = Data.SaveQueryDataSourceColumns();
					callbackState[CallbackState.SavedOLAPFieldValuesName] = Data.SaveQueryDataSourceState();
				}
			}
			return callbackState.ToString();
		}
		protected IXtraPropertyCollection GetCallbackStateProperties(bool internalSaving) {
			if(!internalSaving)
				return GetSnapshot(true, false);
			if(FirstSnapshot == null)
				return GetSnapshot(true, true);
			else
				return SerializationDiffCalculator.CalculateDiffCore(FirstSnapshot, GetSnapshot(true, true));
		}
		protected string SaveCallbackState() {
			string state = SaveCallbackState(true, EnableRowsCache);
			string newState = state;
			if(RaiseSaveCallbackState(ref newState))
				state = newState;
			if(this.gridLayoutEventHelper != null && this.gridLayoutEventHelper.IsRaiseNecessary)
				RaiseLayoutChanged();
			callbackState.Clear();
			return state;
		}
		protected internal void LoadCallbackState(string state, bool resetProperties, bool loadCachedState) {
			if(string.IsNullOrEmpty(state))
				return;
			callbackState.FromString(state);
			LoadLayoutFromCallbackState(resetProperties, loadCachedState);
			ReadCustomizationFormPostData();
		}
		void LoadLayoutFromCallbackState(bool resetProperties, bool loadCachedState) {
			DeserializeSnapshot(callbackState[CallbackState.SerializedLayoutName], resetProperties);
			Data.EnsureFieldCollections();
			Data.RestoreFieldDataTypes(callbackState[CallbackState.SavedFieldDataTypesName]);
			Data.RestoreFilterValuesIsEmptyState(callbackState[CallbackState.SavedFilterValuesIsEmptyName]);
			Data.WebFieldValuesStateCache = callbackState[CallbackState.CollapsedStateName];
			Data.RestoreDenyExpandState(callbackState[CallbackState.SavedDenyExpandStateName]);
			OLAPSessionID = callbackState[CallbackState.SavedOLAPSessionID];
			if(loadCachedState)
				Data.LoadVisualItemsState(callbackState[CallbackState.SavedFieldValuesName], callbackState[CallbackState.SavedDataCellsName]);
			if(IsOLAP || HasOLAPSessionID) {
				OLAPColumnsState = callbackState[CallbackState.SavedOLAPColumnsName];
				OLAPFieldValueState = callbackState[CallbackState.SavedOLAPFieldValuesName];
			}
		}
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		protected override bool CanLoadPostDataOnCreateControls() {
			return true;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			LoadCallbackState(GetCallbackStateFieldValue(), false, true);
			this.gridLayoutEventHelper = new GridLayoutEventHelper(this);
			return false;
		}
		internal string SerializeSnapshot(IXtraPropertyCollection snapshot) {
			return Data.CreateBase64XtraSerializer().Serialize(snapshot);
		}
		protected void DeserializeSnapshot(string base64Snapshot, bool resetProperties) {
			Data.CreateBase64XtraSerializer().Deserialize(this, base64Snapshot, resetProperties);
		}
		internal void DeserializeSnapshot(string base64Snapshot) {
			DeserializeSnapshot(base64Snapshot, true);
		}
		internal new StateBag GetViewState() {
			return ViewState;
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			MakeFirstSnapshot();
		}
		protected IXtraPropertyCollection FirstSnapshot { get { return firstSnapshot; } }
		protected internal void MakeFirstSnapshot() {
			if(firstSnapshot == null)
				firstSnapshot = GetSnapshot(false, true);
		}
		internal protected IXtraPropertyCollection GetSnapshot(bool fullLayout) {
			return GetSnapshot(true, fullLayout);
		}
		IXtraPropertyCollection GetSnapshot(bool callbackState, bool fullLayout) {
			PivotGridWebOptionsLayout options;
			if(callbackState)
				options = fullLayout ? PivotGridWebOptionsLayout.CallbackStateFullLayout : PivotGridWebOptionsLayout.CallbackStateLayout;
			else
				options = fullLayout ? PivotGridWebOptionsLayout.FullLayout : new PivotGridWebOptionsLayout();
			return GetXtraSerializationProperties(options);
		}
		IXtraPropertyCollection GetXtraSerializationProperties(PivotGridWebOptionsLayout options) {
			return new SnapshotSerializeHelper().SerializeObject(this, options);
		}
		void LoadLayoutCore(string layoutState, PivotGridWebOptionsLayout optionsLayout) {
			if(string.IsNullOrEmpty(layoutState)) return;
			bool isPreviousLayoutVersion = layoutState.StartsWith(CallbackState.StartConstSign);
			if(isPreviousLayoutVersion) {
				LoadCallbackState(layoutState, true, false);
				RequireDataUpdate = true;
			} else {
				using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(layoutState))) {
					LoadLayoutFromStream(stream, optionsLayout);
				}
			}
		}
		public string SaveLayoutToString() {
			return SaveLayoutToString(OptionsLayout);
		}
		public string SaveLayoutToString(PivotGridWebOptionsLayout optionsLayout) {
			using(MemoryStream stream = new MemoryStream()) {
				SaveLayoutToStream(stream, optionsLayout);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public void LoadLayoutFromString(string layoutState) {
			LoadLayoutCore(layoutState, OptionsLayout);
		}
		public void LoadLayoutFromString(string layoutState, PivotGridWebOptionsLayout optionsLayout) {
			LoadLayoutCore(layoutState, optionsLayout);
		}
		public void SaveLayoutToStream(Stream stream) {
			SaveLayoutToStream(stream, OptionsLayout);
		}
		public void SaveLayoutToStream(Stream stream, PivotGridWebOptionsLayout optionsLayout) {
			EnsureRefreshData();
			Data.SetIsInternalSerialization(false);
			try {
				Data.CreatePivotXtraSerializer().Serialize(this, stream, optionsLayout != null ? optionsLayout : PivotGridWebOptionsLayout.FullLayout);
			} finally {
				Data.SetIsInternalSerialization(true);
			}
		}
		public void LoadLayoutFromStream(Stream stream) {
			LoadLayoutFromStream(stream, OptionsLayout);
		}
		public void LoadLayoutFromStream(Stream stream, PivotGridWebOptionsLayout optionsLayout) {
			EnsureRefreshData();
			Data.SetIsInternalSerialization(false);
			try {
				Data.CreatePivotXtraSerializer().Deserialize(this, stream, optionsLayout != null ? optionsLayout : PivotGridWebOptionsLayout.FullLayout);
			} finally {
				RequireDataUpdate = true;
				Data.SetIsInternalSerialization(true);
			}
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			EnsureRefreshData();
			Data.SaveCollapsedStateToStream(stream);
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			EnsureRefreshData();
			Data.LoadCollapsedStateFromStream(stream);
		}
		public string SaveCollapsedStateToString() {
			using(MemoryStream stream = new MemoryStream()) {
				SaveCollapsedStateToStream(stream);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public void LoadCollapsedStateFromString(string collapsedState) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(collapsedState))) {
				LoadCollapsedStateFromStream(stream);
			}
		}
		protected ASPxPivotGridPopupMenu HeaderMenu {
			get { return headerMenu; }
		}
		protected internal ASPxPivotGridPopupMenu FieldListMenu {
			get { return fieldListMenu; }
		}
		protected internal virtual PivotGridDataAreaPopup CreateDataAreaPopup() {
			PivotGridDataAreaPopup dataAreaPopup = new PivotGridDataAreaPopup(Data.PivotGrid);
			RaiseDataAreaPopupCreated(dataAreaPopup);
			return dataAreaPopup;
		}
		protected internal virtual ASPxPivotGridPopupMenu CreateHeaderMenu() {
			ASPxPivotGridPopupMenu menu = new ASPxPivotGridPopupMenu(this, PivotGridPopupMenuType.HeaderMenu);
			menu.ID = ElementNames.HeaderMenu;
			menu.ClientSideEvents.ItemClick = "ASPx.pivotGrid_OnHeaderMenuClick";
			menu.EnableViewState = false;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderSortAscending))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuSortAscending), ASPxPivotGridPopupMenu.SortAZID).GroupName = ASPxPivotGridPopupMenu.SortGroupID;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderSortDescending))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuSortDescending), ASPxPivotGridPopupMenu.SortZAID).GroupName = ASPxPivotGridPopupMenu.SortGroupID;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderClearSorting))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuClearSorting), ASPxPivotGridPopupMenu.ClearSortID);
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderRefresh))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuRefreshData), ASPxPivotGridPopupMenu.RefreshID).BeginGroup = true;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderHide))
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuHideField), ASPxPivotGridPopupMenu.HideFieldID).BeginGroup = true;
			if(RaiseAddPopupMenuItem(MenuItemEnum.HeaderShowList) && !ContainsCustomizationFieldsForm && OptionsCustomization.AllowCustomizationForm) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuHideFieldList), ASPxPivotGridPopupMenu.HideFieldListID);
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuShowFieldList), ASPxPivotGridPopupMenu.ShowFieldListID);
			}
			if(OptionsCustomization.AllowPrefilter && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter)
					&& GetColumnCount() > 0
					&& RaiseAddPopupMenuItem(MenuItemEnum.HeaderShowPrefilter)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuShowPrefilter),
					ASPxPivotGridPopupMenu.ShowPrefilterID);
			}
			RaisePopupMenuCreated(menu);
			return menu;
		}
		protected internal virtual ASPxPivotGridPopupMenu CreateFieldValueMenu() {
			ASPxPivotGridPopupMenu menu = new ASPxPivotGridPopupMenu(this, PivotGridPopupMenuType.FieldValueMenu);
			menu.EnableViewState = false;
			menu.ID = ElementNames.FieldValueMenu;
			menu.ClientSideEvents.ItemClick = "ASPx.pivotGrid_OnFieldValueMenuClick";
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueExpand)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuExpand), ASPxPivotGridPopupMenu.ExpandValueID);
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuCollapse), ASPxPivotGridPopupMenu.CollapseValueID);
			}
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueExpandAll)) {
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuExpandAll), ASPxPivotGridPopupMenu.ExpandAllID).BeginGroup = true;
				menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuCollapseAll), ASPxPivotGridPopupMenu.CollapseAllID);
			}
			if(RaiseAddPopupMenuItem(MenuItemEnum.FieldValueSortBySummaryFields)) {
				CreateCrossAreaMenuItems(menu, PivotArea.ColumnArea);
				CreateCrossAreaMenuItems(menu, PivotArea.RowArea);
			}
			RaisePopupMenuCreated(menu);
			return menu;
		}
		protected internal virtual ASPxPivotGridPopupMenu CreateFieldListMenu() {
			ASPxPivotGridPopupMenu menu = new ASPxPivotGridPopupMenu(this, PivotGridPopupMenuType.FieldListMenu);
			menu.ID = ElementNames.FieldListMenu;
			menu.ClientSideEvents.ItemClick = "ASPx.pivotGrid_OnFieldListMenuClick";
			menu.EnableViewState = false;
			return menu;
		}
		protected internal virtual void CreateCrossAreaMenuItems(ASPxPivotGridPopupMenu menu, PivotArea area) {
			if(area != PivotArea.ColumnArea && area != PivotArea.RowArea)
				throw new ArgumentException("area");
			if(Data.DataFieldCount == 0)
				return;
			PivotArea crossArea = area == PivotArea.ColumnArea ? PivotArea.RowArea : PivotArea.ColumnArea;
			List<PivotGridFieldBase> crossAreaFields = Data.GetFieldsByArea(crossArea, false),
									 dataFields = Data.GetFieldsByArea(PivotArea.DataArea, false);
			for(int i = 0; i < crossAreaFields.Count; i++) {
				PivotGridFieldBase field = crossAreaFields[i];
				if(!field.CanSortBySummary) continue;
				CreateSortByWithDataMenuItems(area, menu, field, dataFields, i == 0);
			}
			menu.Items.Add(GetLocalizedString(PivotGridStringId.PopupMenuRemoveAllSortByColumn),
				ASPxPivotGridPopupMenu.SortByID + area.ToString() + "_RemoveAll").BeginGroup = true;
		}
		protected void CreateSortByWithDataMenuItems(PivotArea area, ASPxPivotGridPopupMenu menu, PivotGridFieldBase field, List<PivotGridFieldBase> dataFields, bool beginGroup) {
			for(int i = 0; i < dataFields.Count; i++) {
				CreateSortByMenuItem(area, menu, field, dataFields[i], beginGroup && i == 0);
			}
		}
		protected void CreateSortByMenuItem(PivotArea area, ASPxPivotGridPopupMenu menu, PivotGridFieldBase field,
									PivotGridFieldBase dataField, bool beginGroup) {
			bool showDataCaption = Data.DataField.Area != area && Data.DataFieldCount > 1;
			string caption = GetCaption(field, dataField, area, showDataCaption),
				menuId = ASPxPivotGridPopupMenu.SortByID + area.ToString() + "_" + field.Index.ToString();
			if(dataField != null)
				menuId += "_" + dataField.Index.ToString();
			DevExpress.Web.MenuItem menuItem = menu.Items.Add(caption, menuId);
			menuItem.GroupName = menuId;
			menuItem.BeginGroup = beginGroup;
		}
		protected internal string GetCaption(PivotGridFieldBase field, PivotGridFieldBase dataField, PivotArea area, bool showDataFieldCaption) {
			PivotGridStringId stringId = area == PivotArea.ColumnArea ? PivotGridStringId.PopupMenuSortFieldByColumn : PivotGridStringId.PopupMenuSortFieldByRow;
			string captionTemplate = GetLocalizedString(stringId),
				fieldCaption = field.HeaderDisplayText;
			if(showDataFieldCaption)
				fieldCaption += " - " + dataField.HeaderDisplayText;
			return string.Format(captionTemplate, fieldCaption);
		}
		protected string GetLocalizedString(PivotGridStringId id) {
			return PivotGridLocalizer.GetString(id);
		}
		protected virtual Image CreateHiddenImage(string id, string src) {
			Image image = RenderUtils.CreateImage();
			image.ID = id;
			image.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			image.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
			image.Style.Add(HtmlTextWriterStyle.Display, "none");
			image.ImageUrl = src;
			return image;
		}
		protected override bool HasLoadingPanel() {
			return true;
		}
		protected override bool HasLoadingDiv() {
			return IsCallBacksEnabled();
		}
		int fLockRefreshDataCount = 0;
		void LockRefreshData() {
			fLockRefreshDataCount++;
		}
		void UnlockRefreshData() {
			fLockRefreshDataCount--;
		}
		internal bool IsLockRefreshData { get { return fLockRefreshDataCount != 0; } }
		protected internal void RefreshData(bool force = false) {
			if((!this.RequireDataUpdate && !DesignMode) || IsLockRefreshData || (!force && !Initialized)) return;
			PerformDataRefresh();
			if(PostBackAction != null) {
				PostBackAction.RaiseEventHandlers();
				PostBackAction = null;
				if((IsPostBack || IsCallback))
					RaiseAfterPerformCallback();
				if(RequireDataUpdate)
					PerformDataRefresh();
			}
		}
		void PerformDataRefresh() {
			PivotGridEventsRecorder eventImplementor = new PivotGridEventsRecorder(this);
			eventImplementor.StartRecording();
			LockRefreshData();
			Data.EventsImplementor = eventImplementor;
			if(PostBackAction != null)
				PostBackAction.ApplyBefore();
			RefreshDataCore();
			this.RequireDataUpdate = false;
			if(PostBackAction != null)
				PostBackAction.ApplyAfter();
			RefreshCustomizationFieldList();
			if(!isDataBinding)
				CorrectPageIndex();
			InvalidateChartData();
			UnlockRefreshData();
			if(RequireDataUpdate)
				throw new Exception("Unexpected data update required");
			 Data.EventsImplementor = this;
			eventImplementor.FinishRecordingAndRaiseEvents();
		}
		bool? containsCustomizationFieldsForm;
		internal bool ContainsCustomizationFieldsForm {
			get {
				if(containsCustomizationFieldsForm.HasValue)
					return containsCustomizationFieldsForm.Value;
				return MasterControlImplementation.ContainsRelatedCustomizationForm;
			}
			set {
				if(ContainsCustomizationFieldsForm && value)
					throw new MoreThanOneCustFieldsFormsException();
				containsCustomizationFieldsForm = value;
				ResetControlHierarchy();
				if(!value)
					MasterControlImplementation.RemoveInnerRelatedControls();
			}
		}
		void RefreshCustomizationFieldList() {
			if(ContainsCustomizationFieldsForm)
				MasterControlImplementation.RefreshCustomizationForm();
			else
				CustomizationFields.Refresh();
		}
		internal void PreparePopupMenu(PivotGridPopupMenuType menuType) {
			if(menuType == PivotGridPopupMenuType.FieldListMenu)
				PrepareCustomizationFieldsMenu();
		}
		void PrepareCustomizationFieldsMenu() {
			if(ContainsCustomizationFieldsForm && MasterControlImplementation.ContainsRelatedCustomizationForm)
				MasterControlImplementation.PrepareCustomizationFieldsMenu();
			else
				CustomizationFields.PrepareLayoutMenu();
		}
		void CorrectPageIndex() {
			if(Data.ListSource == null && !IsOLAP && !HasQueryDataSource && !HasQueryPivotDataSource)
				return;
			OptionsPager.PageIndex = GetCorrectedPageIndex(false, OptionsPager.PageIndex);
			OptionsPager.ColumnPageIndex = GetCorrectedPageIndex(true, OptionsPager.ColumnPageIndex);
		}
		int GetCorrectedPageIndex(bool isColumn, int pageIndex) {
			PivotWebVisualItems items = Data.VisualItems;
			if(items.HasPaging(isColumn)) {
				int pageCount = items.GetPageCount(isColumn);
				pageIndex = (pageIndex < pageCount) ? pageIndex : pageCount - 1;
			}
			return pageIndex;
		}
		protected internal int InitialPageSize {
			get { return initialRowsPerPage; }
			set { initialRowsPerPage = value; }
		}
		protected internal virtual void RefreshDataCore() {
			MemoryStream collapsedStateStream = null;
			this.isPivotDataCanDoRefresh = true;
			try {
				if(string.IsNullOrEmpty(Data.WebFieldValuesStateCache) && Data.IsDataBound && !isDataBinding) {
					collapsedStateStream = new MemoryStream();
					Data.SaveCollapsedStateToStream(collapsedStateStream);
					collapsedStateStream.Position = 0;
				}
				Data.DoRefresh();
				canUnlockRefreshAfterDataBind = false;
				if(Data.IsDataBound && !isDataBinding) {
					if(!string.IsNullOrEmpty(Data.WebFieldValuesStateCache) && !IsFakeData) {
						Data.RestoreWebCollapsedState(Data.WebFieldValuesStateCache);
						Data.WebFieldValuesStateCache = null;
					}
					if(collapsedStateStream != null)
						Data.LoadCollapsedStateFromStream(collapsedStateStream);
				}
			} finally {
				this.isPivotDataCanDoRefresh = false;
				if(collapsedStateStream != null) {
					collapsedStateStream.Dispose();
					collapsedStateStream = null;
				}
			}
		}
		bool hasContentInternal;
		protected internal bool HasContentInternal {
			get { return hasContentInternal; }
			set {
				if(hasContentInternal == value) return;
				hasContentInternal = value;
				ResetControlHierarchy();
			}
		}
		protected override bool HasContent() {
			return HasContentInternal || IsPrefilterPopupVisible;
		}
		protected override bool IsSwipeGesturesEnabled() {
			return base.IsSwipeGesturesEnabled() && !Data.OptionsView.ShowHorizontalScrollBarInternal;
		}
		protected internal bool ShowHorzScrollBar {
			get { return Data.OptionsView.ShowHorizontalScrollBarInternal && !Width.IsEmpty; }
		}
		protected internal bool HasHorzScrollContainer {
			get { return (ShowHorzScrollBar || HasCallbackAnimation) && !Data.IsDesignMode; }
		}
		internal bool HasCallbackAnimation {
			get { return IsCallbackAnimationEnabled() || IsSlideCallbackAnimationEnabled(); }
		}
		bool HasOLAPDataSource { get { return DataSource is IPivotOLAPDataSource; } }
		bool HasQueryDataSource { get { return DataSource is IQueryDataSource; } }
		bool HasQueryPivotDataSource { get { return Data.PivotDataSource is IQueryDataSource; } }
		bool HasOLAPSessionID { get { return !string.IsNullOrEmpty(OLAPSessionID); } }
		protected override void PerformDataBinding(string dataHelperName, IEnumerable dataSource) {
			bool isOlap = IsOLAP;
			bool hasQueryDataSource = HasQueryDataSource;
			bool isQueryableDataSource = IsQueryableDataSourceFrontEnd(dataSource);
			if(isOlap || hasQueryDataSource || isQueryableDataSource) {
				bool isStateInvalid = !(isOlap || HasOLAPDataSource) && RequireDataUpdate && (hasQueryDataSource || isQueryableDataSource);
				if(hasQueryDataSource) {
					if(HasOLAPSessionID && HasOLAPDataSource)
						((IOLAPHelpersOwner)DataSource).Metadata.SessionID = OLAPSessionID;
					Data.PivotDataSource = (IQueryDataSource)DataSource;
					OLAPSessionID = null;
				}
				else if(isOlap){
					Data.OLAPDataProvider = OLAPDataProvider;
					Data.OLAPConnectionString = OLAPConnectionString;
				}
				else if(isQueryableDataSource) {
					Data.PivotDataSource = new ServerModeDataSource(new QueryableQueryExecutor(GetQueryable(dataSource)));
					OLAPSessionID = null;
				}
				if(!string.IsNullOrEmpty(OLAPColumnsState) && !isStateInvalid) {
					Data.RestoreQueryDataSourceColumns(OLAPColumnsState);
					OLAPColumnsState = null;
				}
				if(!string.IsNullOrEmpty(OLAPFieldValueState) && !isStateInvalid) {
					Data.RestoreQueryDataSourceState(OLAPFieldValueState);
					OLAPFieldValueState = null;
				}
				RequireDataUpdate = true;
			}
			else {
				IList list = dataSource as System.Collections.IList;
				if(list == null && dataSource != null) {
					list = new ArrayList();
					foreach(object o in dataSource)
						list.Add(o);
				}
				if((list != null || !IsFakeData) && Data.ListSource != list) {
					LockRefreshData();
					Data.BeginUpdate();
					Data.ListSource = list;
					UnlockRefreshData();
					Data.CancelUpdate();
					RequireDataUpdate = true;
					canUnlockRefreshAfterDataBind = true;
				}
			}
			Fields.ForceUpdateAreaIndexes();
		}
		bool IsQueryableDataSourceFrontEnd(object dataSource) {
			return dataSource is LinqServerModeFrontEnd || dataSource is EntityServerModeFrontEnd;
		}
		bool IsQueryableDataSource(object dataSource) {
			if(dataSource is LinqServerModeDataSource)
				return true;
			if(dataSource is EntityServerModeDataSource)
				return true;
			IListSource listSource = dataSource as IListSource;
			if(listSource == null)
				return false;
			return IsQueryableDataSourceFrontEnd(listSource.GetList());
		}
		IQueryable GetQueryable(object dataSource) {
			LinqServerModeFrontEnd linqServerModeFrontEnd = dataSource as LinqServerModeFrontEnd;
			if(linqServerModeFrontEnd != null)
				return linqServerModeFrontEnd.Owner.QueryableSource;
			EntityServerModeFrontEnd entityServerModeFrontEnd = dataSource as EntityServerModeFrontEnd;
			if(entityServerModeFrontEnd != null)
				return entityServerModeFrontEnd.Owner.QueryableSource;
			return null;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxPivotGrid), PivotGridWebData.PivotGridSystemCssResourceName);
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDragAndDropUtilsScripts();
			RegisterRelatedControlManagerScripts();
			RegisterIncludeScript(typeof(ASPxPivotGrid), PivotGridWebData.PivotTableWrapperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxPivotGrid), PivotGridWebData.AdjustingManagerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxPivotGrid), PivotGridWebData.PivotGridScriptResourceName);
		}
		protected override void RegisterDragAndDropUtilsScripts() {
			base.RegisterDragAndDropUtilsScripts();
			RegisterIncludeScript(typeof(ASPxPivotGrid), PivotGridWebData.DragAndDropScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPivotGrid";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return base.GetStateManagedObjects();
		}
		protected virtual void UpdateDataBoundAndChildControls(bool boundImmediate) {
			if((!IsMvcRender() || Page == null) && IsLoading()) return;
			if(!Data.IsDataBound) {
				ResetControlHierarchy();
				if(boundImmediate)
					DataBind();
				else {
					PerformSelect();
					DataBindChildren();
					ResetControlHierarchy();
				}
			} else {
				RequireDataUpdate = true;
			}
		}
		protected bool IsOurCallbackPostback {
			get { return isOurCallbackPostback; }
		}
		protected virtual void PerformCallbackOrPostBack(string eventArgument) {
			if(string.IsNullOrEmpty(eventArgument))
				return;
			this.isOurCallbackPostback = true;
			string postbackId = CallbackCommands.GetPostBackId(eventArgument);
			bool isOurFilterAction = CallbackCommands.GetIsFilterShowWindowAction(postbackId)
				|| CallbackCommands.GetIsFilterChildrenRetrievedAction(postbackId);
			this.isVirtualScrollingCallback = CallbackCommands.GetIsVirtualScrollingAction(postbackId);
			EnsureRefreshDataInternal();
			isDefereFilterCallback = CallbackCommands.GetIsDefereFilterAction(postbackId);
			if(isOurFilterAction) {
				PrepareFilterShowCallBack(eventArgument, CallbackCommands.GetIsFilterChildrenRetrievedAction(postbackId));
			} else {
				PreparePostBackAction(postbackId, eventArgument);
			}
		}
		void PrepareFilterShowCallBack(string eventArgument, bool isGroupFilterCallback) {
			isFilterPopupCallBack = true;
			FilterManager filterManager = FilterManager.Create(eventArgument, isGroupFilterCallback);
			if(filterManager == null)
				return;
			if(IsOLAP) Data.DoRefresh();
			UpdateDataBoundAndChildControls(true);
			HeaderFilterPopup fp = AddFilterPopup();
			EnsureFilterPopupContentControl(filterManager);
			fp.Controls.Add(FilterPopupContentControl);
			fp.EnsureChildControls();
			FilterPopupContentControl.PrepareCallbackResultObject(filterManager.CallbackArgs);
		}
		void PreparePostBackAction(string postbackId, string eventArgument) {
			PostBackAction = CallbackCommands.CreatePostBackAction(this, postbackId, eventArgument);
			EnsureRefreshData();
		}
		bool IsFilterPopupCallBack { get { return isFilterPopupCallBack; } }
		bool IsDefereFilterCallback { get { return isDefereFilterCallback; } }
		void EnsureFilterPopupContentControl(FilterManager filterManager) {
			PivotGridField filterField = Data.GetFieldByIndex(filterManager.FieldIndex);
			if(Data.GetIsGroupFilter(filterField)) {
				this.filterPopupContentControl = new PivotGridHtmlGroupFilterPopupContent(Data, filterField, filterManager.State, filterManager.Values, filterManager.DeferredFilter, filterManager.IsGroupCallback);
			}
			else {
				this.filterPopupContentControl = new PivotGridHtmlFilterPopupContent(Data, filterField, filterManager.DeferredFilter);
			}
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			if(string.IsNullOrEmpty(eventArgument)) return;
			PerformCallbackOrPostBack(eventArgument);
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			RegisterRequiresViewStateEncryption();
			if(IsFilterPopupCallBack && FilterPopupContentControl != null)
				result[CallbackResultProperties.Result] = FilterPopupContentControl.GetCallbackResult();
			else if(IsDefereFilterCallback) {
				EnsureChildControls();
				result[CallbackResultProperties.Result] = new object[] { "DF" };
			}
			else {
				EnsureChildControls();
				EnsureFieldListCreated();
				StringBuilder sb = new StringBuilder("G|");
				sb.Append(GetRelatedCallbackResult());
				sb.Append(GetMasterCallbackResult());
				result[CallbackResultProperties.Result] = new object[] { sb.ToString(), null };
			}
			result[CallbackResultProperties.StateObject] = GetClientObjectState();
			return result;
		}
		string GetRelatedCallbackResult() {
			return (this as IMasterControl).CalcRelatedControlsCallbackResult();
		}
		string GetMasterCallbackResult() {
			List<ISupportsCallbackResult> list = new List<ISupportsCallbackResult>();
			if(isVirtualScrollingCallback) {
				list.AddRange(MainTable.GetPartialCallbackControls());
			} else {
				list.Add(this);
			}
			return masterControlImplementation.CalcRelatedControlsCallbackResult(list);
		}
		internal string GetMasterCallbackResultString() {
			StringBuilder sb = new StringBuilder();
			foreach(Control control in mainTD.Controls)
				sb.Append(RenderUtils.GetRenderResult(control));
			sb.Append(GetCallbackClientObjectScript());
			return sb.ToString();
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			int callbackStart = eventArgument.IndexOf("|CB|");
			if(callbackStart > -1) {
				EnsureChildControls();
				PerformCallbackOrPostBack(eventArgument.Substring(0, callbackStart));
			} else
				PerformCallbackOrPostBack(eventArgument);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName).Append(".callBacksEnabled = ").Append(EnableCallBacks ? "true" : "false").AppendLine(";");
			InitializeClientObjectScript(stb, localVarName, clientName);
		}
		protected virtual void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			if(this.addCustomJSPropertiesScript)
				GetCustomJSPropertiesScript(stb, ShortClientLocalVariableName);
			if(OptionsPager.HasPager(false) && (Data.ListSource != null || !string.IsNullOrEmpty(Data.OLAPConnectionString))) {
				stb.Append(ShortClientLocalVariableName).Append(".pageIndex = ").Append(OptionsPager.PageIndex).AppendLine(";");
				stb.Append(ShortClientLocalVariableName).Append(".pageCount = ").Append(Data.VisualItems.GetPageCount(false)).AppendLine(";");
			}
			stb.AppendLine(RenderHelper.GetHoverScript());
			stb.AppendLine(RenderHelper.GetContextMenuScript(ShortClientLocalVariableName));
			stb.AppendLine(RenderHelper.GetAllowedAreaIdsScript(ShortClientLocalVariableName));
			stb.AppendLine(RenderHelper.GetGroupsScript(ShortClientLocalVariableName));
			stb.AppendLine(RenderHelper.GetFilterPopupScript());
			stb.AppendLine(RenderHelper.GetAfterCallBackInitializeScript());
			stb.AppendLine(RenderHelper.GetRowTreeIE8LayoutFixScript());
		}
		internal string GetCallbackClientObjectScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("var {0} = ASPx.GetControlCollection().Get({1});\r\n", ShortClientLocalVariableName, HtmlConvertor.ToScript(ClientID));
			InitializeClientObjectScript(stb, ShortClientLocalVariableName, ClientID);
			return RenderUtils.GetScriptHtml(stb.ToString());
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ClientStateProperties.CallbackState, SaveCallbackState());
			return result;
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCustomizationFieldsVisible"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable()]
		public bool CustomizationFieldsVisible {
			get { return CustomizationFields.ShowOnPageLoad; }
			set {
				CustomizationFields.ShowOnPageLoad = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCustomizationFieldsLeft"),
#endif
		Category("Behavior"), DefaultValue(100), AutoFormatDisable()]
		public int CustomizationFieldsLeft {
			get { return CustomizationFields.Left; }
			set {
				CustomizationFields.Left = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCustomizationFieldsTop"),
#endif
		Category("Behavior"), DefaultValue(100), AutoFormatDisable()]
		public int CustomizationFieldsTop {
			get { return CustomizationFields.Top; }
			set {
				CustomizationFields.Top = value;
			}
		}
		void ResetPrefilter() { Prefilter.Reset(); }
		bool ShouldSerializePrefilter() { return Prefilter.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPrefilter"),
#endif
		Category("Data"), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public WebPrefilter Prefilter {
			get { return Data.Prefilter; }
		}
		void ResetOptionsView() { OptionsView.Reset(); }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsView"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable, XtraSerializablePropertyId(LayoutIds.LayoutIdOptionsView)]
		public PivotGridWebOptionsView OptionsView { get { return Data.OptionsView; } }
		void ResetOptionsOLAP() { OptionsOLAP.Reset(); }
		bool ShouldSerializeOptionsOLAP() { return OptionsOLAP.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsOLAP"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable()
		]
		public PivotGridOptionsOLAP OptionsOLAP { get { return Data.OptionsOLAP; } }
		void ResetOptionsCustomization() { OptionsCustomization.Reset(); }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsCustomization"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable]
		public PivotGridWebOptionsCustomization OptionsCustomization { get { return Data.OptionsCustomization as PivotGridWebOptionsCustomization; } }
		void ResetOptionsChartDataSource() { OptionsChartDataSource.Reset(); }
		bool ShouldSerializeOptionsChartDataSource() { return OptionsChartDataSource.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsChartDataSource"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable]
		public PivotGridWebOptionsChartDataSource OptionsChartDataSource { get { return Data.OptionsChartDataSource; } }
		void ResetOptionsDataField() { OptionsDataField.Reset(); }
		bool ShouldSerializeOptionsDataField() { return OptionsDataField.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsDataField"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		AutoFormatDisable()]
		public PivotGridWebOptionsDataField OptionsDataField { get { return Data.OptionsDataField as PivotGridWebOptionsDataField; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsPager"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true), AutoFormatEnable()]
		public PivotGridWebOptionsPager OptionsPager { get { return Data.OptionsPager; } }
		void ResetOptionsLoadingPanel() { OptionsLoadingPanel.Reset(); }
		bool ShouldSerializeOptionsLoadingPanel() { return OptionsLoadingPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsLoadingPanel"),
#endif
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true), AutoFormatEnable()
		]
		public PivotGridWebOptionsLoadingPanel OptionsLoadingPanel { get { return Data.OptionsLoadingPanel; } }
		void ResetOptionsData() { OptionsData.Reset(); }
		bool ShouldSerializeOptionsData() { return OptionsData.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsData"),
#endif
		Category("Options"),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true),
		AutoFormatEnable()
		]
		public PivotGridWebOptionsData OptionsData { get { return Data.OptionsData; } }
		void ResetOptionsBehavior() { OptionsBehavior.Reset(); }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridOptionsBehavior"),
#endif
		Category("Options"),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true),
		AutoFormatEnable()
		]
		public PivotGridWebOptionsBehavior OptionsBehavior { get { return Data.OptionsBehavior as PivotGridWebOptionsBehavior; } }
		void ResetOptionsFilter() { OptionsFilter.Reset(); }
		bool ShouldSerializeOptionsFilter() { return OptionsFilter.ShouldSerialize(); }
		[
		Category("Options"),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		Localizable(true),
		AutoFormatEnable()
		]
		public PivotGridWebOptionsFilter OptionsFilter { get { return Data.OptionsFilter; } }
		PivotGridWebOptionsLayout optionsLayout;
		protected virtual PivotGridWebOptionsLayout CreateOptionsLayout() {
			return new PivotGridWebOptionsLayout();
		}
		void ResetOptionsLayout() { OptionsLayout.Reset(); }
		[
		Category("Options"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable]
		public PivotGridWebOptionsLayout OptionsLayout {
			get {
				if(optionsLayout == null)
					optionsLayout = CreateOptionsLayout();
				return optionsLayout;
			}
		}
		protected override StylesBase CreateStyles() {
			return new PivotGridStyles(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxPivotGrid), PivotGridWebData.PivotGridDefaultCssResourceName);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCssClass"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatEnable()]
		public override string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridImages"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImages Images { get { return (PivotGridImages)ImagesInternal; } }
		protected override ImagesBase CreateImages() {
			return new PivotGridImages(this);
		}
		void CreateCustomizationFormImages() {
			if(customizationFormImages == null)
				customizationFormImages = new PivotCustomizationFormImages(this);
		}
		PivotKPIImages IPivotGridEventsImplementor.KPIImages {
			get {
				if(kpiImages == null)
					kpiImages = new PivotKPIImages(this);
				return kpiImages;
			}
		}
		PivotCustomizationFormImages customizationFormImages;
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridCustomizationFormImages"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public PivotCustomizationFormImages CustomizationFormImages { get { return customizationFormImages; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorImages ImagesEditors { get { return imagesEditors; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridImagesPrefilterControl"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImages ImagesPrefilterControl { get { return imagesPrefilterControl; } }
		internal AppearanceStyle[] GetStyles() {
			return new AppearanceStyle[] { 
				LoadingPanelStyle
			};
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridStyles"),
#endif
		Category("Styles"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()]
		public PivotGridStyles Styles { get { return StylesInternal as PivotGridStyles; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridStylesPager"),
#endif
		Category("Styles"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()]
		public PivotGridPagerStyles StylesPager { get { return pagerStyles; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridStylesPrint"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatEnable()]
		public WebPrintAppearance StylesPrint { get { return printStyles; } }
		void ResetStylesPrint() { StylesPrint.Reset(); }
		bool ShouldSerializeStylesPrint() { return StylesPrint.ShouldSerialize(); }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridStylesFilterControl"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlStyles StylesFilterControl { get { return stylesFilterControl; } }
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridHeaderTemplateContainer)), AutoFormatDisable()]
		public virtual ITemplate HeaderTemplate {
			get { return Data.HeaderTemplate; }
			set {
				Data.HeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridEmptyAreaTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate EmptyAreaTemplate {
			get { return Data.EmptyAreaTemplate; }
			set {
				Data.EmptyAreaTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridFieldValueTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate FieldValueTemplate {
			get { return Data.FieldValueTemplate; }
			set {
				Data.FieldValueTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridToolTip"),
#endif
		Localizable(false), AutoFormatDisable()]
		public override string ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		[Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(PivotGridCellTemplateContainer)), AutoFormatEnable()]
		public virtual ITemplate CellTemplate {
			get { return Data.CellTemplate; }
			set {
				Data.CellTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		public override Unit Width {
			set {
				if(value == base.Width) return;
				base.Width = value;
				if(ShowHorzScrollBar)
					ResetControlHierarchy();
			}
			get { return base.Width; }
		}
		[Category("Data")]
		public event CustomFieldDataEventHandler CustomUnboundFieldData {
			add {
				this.Events.AddHandler(customUnboundFieldData, value);
				RequireDataUpdate = true;
			}
			remove {
				this.Events.RemoveHandler(customUnboundFieldData, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomSummaryEventHandler CustomSummary {
			add {
				this.Events.AddHandler(customSummary, value);
				RequireDataUpdate = true;
			}
			remove {
				this.Events.RemoveHandler(customSummary, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomFieldSortEventHandler CustomFieldSort {
			add {
				this.Events.AddHandler(customFieldSort, value);
				RequireDataUpdate = true;
			}
			remove {
				this.Events.RemoveHandler(customFieldSort, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event EventHandler<CustomServerModeSortEventArgs> CustomServerModeSort {
			add {
				this.Events.AddHandler(customServerModeSort, value);
				RequireDataUpdate = true;
			}
			remove {
				this.Events.RemoveHandler(customServerModeSort, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Data")]
		public event PivotGridCustomGroupIntervalEventHandler CustomGroupInterval {
			add {
				this.Events.AddHandler(customGroupInterval, value);
				RequireDataUpdate = true;
			}
			remove {
				this.Events.RemoveHandler(customGroupInterval, value);
				RequireDataUpdate = true;
			}
		}
		[Category("Layout")]
		public event PivotAreaChangingEventHandler FieldAreaChanging {
			add { this.Events.AddHandler(fieldAreaChanging, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanging, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldAreaChanged {
			add { this.Events.AddHandler(fieldAreaChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldAreaIndexChanged {
			add { this.Events.AddHandler(fieldAreaIndexChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaIndexChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldVisibleChanged {
			add { this.Events.AddHandler(fieldVisibleChanged, value); }
			remove { this.Events.RemoveHandler(fieldVisibleChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldEventHandler FieldFilterChanged {
			add { this.Events.AddHandler(fieldFilterChanged, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanged, value); }
		}
		[Category("Layout")]
		public event PivotFieldFilterChangingEventHandler FieldFilterChanging {
			add { this.Events.AddHandler(fieldFilterChanging, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanging, value); }
		}
		[Category("Layout")]
		public event PivotGroupEventHandler GroupFilterChanged {
			add { this.Events.AddHandler(groupFilterChanged, value); }
			remove { this.Events.RemoveHandler(groupFilterChanged, value); }
		}
		[Category("Layout")]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Category("Layout")]
		public event ASPxClientLayoutHandler ClientLayout {
			add { this.Events.AddHandler(EventClientLayout, value); }
			remove { this.Events.RemoveHandler(EventClientLayout, value); }
		}
		[Category("Rendering")]
		public event PivotHtmlCellPreparedEventHandler HtmlCellPrepared {
			add { this.Events.AddHandler(htmlCellPrepared, value); }
			remove { this.Events.RemoveHandler(htmlCellPrepared, value); }
		}
		[Category("Rendering")]
		public event PivotHtmlFieldValuePreparedEventHandler HtmlFieldValuePrepared {
			add { this.Events.AddHandler(htmlFieldValuePrepared, value); }
			remove { this.Events.RemoveHandler(htmlFieldValuePrepared, value); }
		}
		[Category("Pager")]
		public event EventHandler PageIndexChanged {
			add { Events.AddHandler(pageIndexChanged, value); }
			remove { Events.RemoveHandler(pageIndexChanged, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedEventHandler FieldValueCollapsed {
			add { this.Events.AddHandler(fieldValueCollapsed, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsed, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedEventHandler FieldValueExpanded {
			add { this.Events.AddHandler(fieldValueExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanded, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedCancelEventHandler FieldValueCollapsing {
			add { this.Events.AddHandler(fieldValueCollapsing, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsing, value); }
		}
		[Category("Behavior")]
		public event PivotFieldEventHandler FieldExpandedInFieldGroupChanged {
			add { this.Events.AddHandler(fieldExpandedInFieldGroupChanged, value); }
			remove { this.Events.RemoveHandler(fieldExpandedInFieldGroupChanged, value); }
		}
		[Category("Data")]
		public event PivotFieldPropertyChangedEventHandler FieldPropertyChanged {
			add { this.Events.AddHandler(fieldPropertyChanged, value); }
			remove { this.Events.RemoveHandler(fieldPropertyChanged, value); }
		}
		[Category("Data")]
		public event PivotFieldEventHandler FieldUnboundExpressionChanged {
			add { this.Events.AddHandler(fieldUnboundExpressionChanged, value); }
			remove { this.Events.RemoveHandler(fieldUnboundExpressionChanged, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedCancelEventHandler FieldValueExpanding {
			add { this.Events.AddHandler(fieldValueExpanding, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanding, value); }
		}
		[Category("Behavior")]
		public event PivotFieldStateChangedEventHandler FieldValueNotExpanded {
			add { this.Events.AddHandler(fieldValueNotExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueNotExpanded, value); }
		}
		[Category("Behavior")]
		public event EventHandler ControlHierarchyCreated {
			add { this.Events.AddHandler(controlHierarchyCreated, value); }
			remove { this.Events.RemoveHandler(controlHierarchyCreated, value); }
		}
		[Category("Data")]
		public event EventHandler PrefilterCriteriaChanged {
			add { this.Events.AddHandler(prefilterCriteriaChanged, value); }
			remove { this.Events.RemoveHandler(prefilterCriteriaChanged, value); }
		}
		[Category("Data")]
		public event PivotFieldDisplayTextEventHandler FieldValueDisplayText {
			add { this.Events.AddHandler(fieldValueDisplayText, value); }
			remove { this.Events.RemoveHandler(fieldValueDisplayText, value); }
		}
		[Category("Data")]
		public event PivotCustomChartDataSourceDataEventHandler CustomChartDataSourceData {
			add { this.Events.AddHandler(customChartDataSourceData, value); }
			remove { this.Events.RemoveHandler(customChartDataSourceData, value); }
		}
		[Category("Data")]
		public event PivotCustomChartDataSourceRowsEventHandler CustomChartDataSourceRows {
			add { this.Events.AddHandler(customChartDataSourceRows, value); }
			remove { this.Events.RemoveHandler(customChartDataSourceRows, value); }
		}
		[Category("Data")]
		public event PivotCellDisplayTextEventHandler CustomCellDisplayText {
			add { this.Events.AddHandler(customCellDisplayText, value); }
			remove { this.Events.RemoveHandler(customCellDisplayText, value); }
		}
		[Category("Data")]
		public event EventHandler<PivotCellValueEventArgs> CustomCellValue {
			add { this.Events.AddHandler(customCellValue, value); }
			remove { this.Events.RemoveHandler(customCellValue, value); }
		}
		[Category("Data")]
		public event EventHandler<PivotCustomFilterPopupItemsEventArgs> CustomFilterPopupItems {
			add { this.Events.AddHandler(customFilterPopupItems, value); }
			remove { this.Events.RemoveHandler(customFilterPopupItems, value); }
		}
		[Category("Data")]
		public event EventHandler<PivotCustomFieldValueCellsEventArgs> CustomFieldValueCells {
			add { this.Events.AddHandler(customFieldValueCells, value); }
			remove { this.Events.RemoveHandler(customFieldValueCells, value); }
		}
		[Category("Data")]
		public event EventHandler BeginRefresh {
			add { this.Events.AddHandler(beginRefresh, value); }
			remove { this.Events.RemoveHandler(beginRefresh, value); }
		}
		[Category("Data")]
		public event EventHandler EndRefresh {
			add { this.Events.AddHandler(endRefresh, value); }
			remove { this.Events.RemoveHandler(endRefresh, value); }
		}
		[Category("Data")]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[Category("Layout")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[Category("Styles")]
		public event PivotCustomCellStyleEventHandler CustomCellStyle {
			add { this.Events.AddHandler(customCellStyle, value); }
			remove { this.Events.RemoveHandler(customCellStyle, value); }
		}
		[Category("Menu")]
		public event PivotAddPopupMenuItemEventHandler AddPopupMenuItem {
			add { this.Events.AddHandler(addPopupMenuItem, value); }
			remove { this.Events.RemoveHandler(addPopupMenuItem, value); }
		}
		[Category("Menu")]
		public event PivotPopupMenuCreatedEventHandler PopupMenuCreated {
			add { this.Events.AddHandler(popupMenuCreated, value); }
			remove { this.Events.RemoveHandler(popupMenuCreated, value); }
		}
		[Category("Behavior")]
		public event EventHandler<PivotDataAreaPopupCreatedEventArgs> DataAreaPopupCreated {
			add { this.Events.AddHandler(dataAreaPopupCreated, value); }
			remove { this.Events.RemoveHandler(dataAreaPopupCreated, value); }
		}
		[Category("Behavior")]
		public event EventHandler OLAPQueryTimeout {
			add { this.Events.AddHandler(olapQueryTimeout, value); }
			remove { this.Events.RemoveHandler(olapQueryTimeout, value); }
		}
		[Category("Behavior"), Browsable(false), Obsolete("The OLAPException event is obsolete now. Use the QueryException event instead.")]
		public event PivotOlapExceptionEventHandler OLAPException {
			add { this.Events.AddHandler(olapException, value); }
			remove { this.Events.RemoveHandler(olapException, value); }
		}
		[Category("Behavior")]
		public event PivotOlapExceptionEventHandler QueryException {
			add { this.Events.AddHandler(queryException, value); }
			remove { this.Events.RemoveHandler(queryException, value); }
		}
		[Category("Behavior")]
		public event PivotCustomCallbackEventHandler CustomCallback {
			add { this.Events.AddHandler(customCallback, value); }
			remove { this.Events.RemoveHandler(customCallback, value); }
		}
		static bool lockDataRefreshOnCustomCallback = false;
		public static bool LockDataRefreshOnCustomCallback {
			get { return lockDataRefreshOnCustomCallback; }
			set { lockDataRefreshOnCustomCallback = value; }
		}
		[Category("Behavior")]
		public event EventHandler AfterPerformCallback {
			add { this.Events.AddHandler(afterPerformCallback, value); }
			remove { this.Events.RemoveHandler(afterPerformCallback, value); }
		}
		[Category("Behavior")]
		public event PivotGridCallbackStateEventHandler CustomSaveCallbackState {
			add { Events.AddHandler(customSaveCallbackState, value); }
			remove { Events.RemoveHandler(customSaveCallbackState, value); }
		}
		[Category("Behavior")]
		public event PivotGridCallbackStateEventHandler CustomLoadCallbackState {
			add { Events.AddHandler(customLoadCallbackState, value); }
			remove { Events.RemoveHandler(customLoadCallbackState, value); }
		}
		[Category("Layout")]
		public event EventHandler GridLayout {
			add { this.Events.AddHandler(layoutChanged, value); }
			remove { this.Events.RemoveHandler(layoutChanged, value); }
		}
		[Category("Data")]
		public event EventHandler BeforePerformDataSelect {
			add { this.Events.AddHandler(beforePerformDataSelect, value); }
			remove { this.Events.RemoveHandler(beforePerformDataSelect, value); }
		}
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJsProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		public event CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText {
			add { Events.AddHandler(customFilterExpressionDisplayText, value); }
			remove { Events.RemoveHandler(customFilterExpressionDisplayText, value); }
		}
		public event FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility {
			add { Events.AddHandler(filterControlOperationVisibility, value); }
			remove { Events.RemoveHandler(filterControlOperationVisibility, value); }
		}
		public event FilterControlParseValueEventHandler FilterControlParseValue {
			add { Events.AddHandler(filterControlParseValue, value); }
			remove { Events.RemoveHandler(filterControlParseValue, value); }
		}
		public event FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText {
			add { Events.AddHandler(filterControlCustomValueDisplayText, value); }
			remove { Events.RemoveHandler(filterControlCustomValueDisplayText, value); }
		}
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		public void BeginUpdate() {
			Data.BeginUpdate();
		}
		public void EndUpdate() {
			Data.EndUpdate();
		}
		public bool IsUpdateLocked { get { return Data.IsLockUpdate; } }
		public void RetrieveFields() {
			EnsureRefreshData();
			Data.RetrieveFields();
			if(IsMvcRender())
				EnsureChildControls();
		}
		public void RetrieveFields(PivotArea area, bool visible) {
			EnsureRefreshData();
			Data.RetrieveFields(area, visible);
			if(IsMvcRender())
				EnsureChildControls();
		}
		public string[] GetFieldList() {
			EnsureRefreshData();
			return Data.GetFieldList();
		}
		public List<PivotGridField> GetFieldsByArea(PivotArea area) {
			EnsureRefreshData();
			List<PivotGridFieldBase> baseList = Data.GetFieldsByArea(area, false);
			List<PivotGridField> res = new List<PivotGridField>(baseList.Count);
			for(int i = 0; i < baseList.Count; i++)
				res.Add((PivotGridField)baseList[i]);
			return res;
		}
		public PivotGridField GetFieldByArea(PivotArea area, int areaIndex) {
			EnsureRefreshData();
			return (PivotGridField)Data.GetFieldByArea(area, areaIndex);
		}
		public int GetFieldCountByArea(PivotArea area) {
			EnsureRefreshData();
			return Data.GetFieldCountByArea(area);
		}
		public List<string> GetOLAPKPIList() {
			EnsureRefreshData();
			return Data.GetOLAPKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			EnsureRefreshData();
			return Data.GetOLAPKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			EnsureRefreshData();
			return Data.GetOLAPKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			EnsureRefreshData();
			return Data.GetOLAPKPIServerGraphic(kpiName, kpiType);
		}
		public ImageProperties GetKPIImage(PivotKPIGraphic graphic, PivotKPIType kpiType, int value) {
			return Data.KPIImages.GetImage(graphic, kpiType, value);
		}
		public void ReloadData() {
			Data.ReloadData();
		}
		public new void LayoutChanged() {
			Data.LayoutChanged();
		}
		protected override void InitInternal() {
			base.InitInternal();
			Data.RestoreFieldsInGroups();
		}
		void ReadCustomizationFormPostData() {
			if(!ContainsCustomizationFieldsForm)
				CustomizationFields.ReadPostData();
		}
		protected void OnDataSourceChanged() {
			if(Data.IsLoading)
				return;
			RequireDataUpdate = true;
			Data.OLAPDataProvider = XtraPivotGrid.OLAPDataProvider.Default;
			Data.OLAPConnectionString = null;
			Data.ListSource = null;
			Fields.ClearCaches();
		}
		void IPivotGridEventsImplementor.CustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle) {
			RaiseCustomCellStyle(cellItem, cellStyle);
		}
		void IPivotGridEventsImplementor.PageIndexChanged() {
			if(!IsLoading()) RaisePageIndexChanged();
		}
		void IPivotGridEventsImplementor.ElementTemplatesChanged() {
			Data.ElementTemplatesChanged();
			TemplatesChanged();
		}
		void IPivotGridEventsImplementor.EnsureRefreshData() {
			EnsureRefreshData();
		}
		bool IPivotGridEventsImplementor.IsPivotDataCanDoRefresh { get { return isPivotDataCanDoRefresh; } }
		T IViewBagOwner.GetViewBagProperty<T>(string objectPath, string propertyName, T value) {
			return (T)GetObjectProperty(GetBagKeyName(objectPath, propertyName), value);
		}
		void IPivotGridEventsImplementor.ResetControlHierarchy() {
			ResetControlHierarchy();
		}
		bool IPivotGridEventsImplementor.RequireUpdateData {
			get { return RequireDataUpdate; }
			set {
				RequireDataUpdate = true;
				if(IsOLAP)
					OLAPFieldValueState = string.Empty;
			}
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value) {
			SetObjectProperty(GetBagKeyName(objectPath, propertyName), defaultValue, value);
		}
		Dictionary<string, Dictionary<string, string>> names = new Dictionary<string, Dictionary<string, string>>();
		string GetBagKeyName(string objectName, string propertyName) {
			Dictionary<string, string> list;
			if(!names.TryGetValue(objectName, out list)) {
				list = new Dictionary<string, string>();
				names.Add(objectName, list);
			}
			string name;
			if(!list.TryGetValue(propertyName, out name)) {
				name = objectName + "." + propertyName;
				list.Add(propertyName, name);
			}
			return name;
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridGroups"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 100),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, XtraSerializablePropertyId(LayoutIds.LayoutIdLayout),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PivotGridWebGroupCollection Groups {
			get { return Data.Groups; }
		}
		protected void XtraClearGroups(XtraItemEventArgs e) {
			if(IsGroupsClearingRequired(e.Options)) {
				Groups.Clear();
			}
		}
		protected object XtraCreateGroupsItem(XtraItemEventArgs e) {
			return Groups.Add();
		}
		protected object XtraFindGroupsItem(XtraItemEventArgs e) {
			if(!IsGroupsClearingRequired(e.Options))
				return null;
			XtraPropertyInfo itemGroup = e.Item;
			PivotGridWebGroupCollection groups = Groups;
			if(groups.Count > 0 && itemGroup.ChildProperties != null) {
				XtraPropertyInfo fields = itemGroup.ChildProperties["Fields"];
				if(fields != null && fields.ChildProperties != null) {
					foreach(PivotGridWebGroup group in groups) {
						if(group.Count == fields.ChildProperties.Count) {
							bool isFound = true;
							for(int i = 0; i < fields.ChildProperties.Count; i++) {
								XtraPropertyInfo fieldInfo = fields.ChildProperties[i];
								if(fieldInfo.Value == null || (fieldInfo.Value != null && group.GetFieldByName(fieldInfo.Value.ToString()) == null)) {
									isFound = false;
									break;
								}
							}
							if(isFound)
								return group;
						}
					}
				}
			}
			return null;
		}
		bool IsGroupsClearingRequired(OptionsLayoutBase optionsLayout) {
			PivotGridOptionsLayout pivotOptionsLayout = optionsLayout as PivotGridOptionsLayout;
			return (pivotOptionsLayout == null) ? true : !pivotOptionsLayout.AddNewGroups;
		}
		protected virtual object RaiseCustomUnboundColumnData(PivotGridFieldBase field, int listSourceRowIndex, object value) {
			CustomFieldDataEventHandler handler = (CustomFieldDataEventHandler)this.Events[customUnboundFieldData];
			if(handler != null) {
				CustomFieldDataEventArgs e = new CustomFieldDataEventArgs(Data, field as PivotGridField, listSourceRowIndex, value);
				handler(this, e);
				return e.Value;
			} else
				return value;
		}
		protected virtual void RaiseCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			PivotGridCustomSummaryEventHandler handler = (PivotGridCustomSummaryEventHandler)this.Events[customSummary];
			if(handler != null) {
				PivotGridCustomSummaryEventArgs e = new PivotGridCustomSummaryEventArgs(Data, field as PivotGridField, customSummaryInfo);
				handler(this, e);
			}
		}
		PivotGridCustomFieldSortEventArgs fieldSortEventArgs = null;
		protected virtual int? RaiseCustomFieldSort(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			PivotGridCustomFieldSortEventHandler handler = (PivotGridCustomFieldSortEventHandler)this.Events[customFieldSort];
			if(handler != null) {
				if(fieldSortEventArgs != null && (fieldSortEventArgs.Field != field || fieldSortEventArgs.Data != Data)) {
					fieldSortEventArgs = null;
				}
				if(fieldSortEventArgs == null) {
					fieldSortEventArgs = new PivotGridCustomFieldSortEventArgs(Data, field as PivotGridField);
				}
				fieldSortEventArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
				handler(this, fieldSortEventArgs);
				return fieldSortEventArgs.GetSortResult();
			} else
				return null;
		}
		CustomServerModeSortEventArgs customServerModeSortEventArgs = null;
		protected virtual int? RaiseQuerySorting(PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			EventHandler<CustomServerModeSortEventArgs> handler = (EventHandler<CustomServerModeSortEventArgs>)this.Events[customServerModeSort];
			if(handler != null) {
				if(customServerModeSortEventArgs != null && customServerModeSortEventArgs.Field != field)
					customServerModeSortEventArgs = null;
				if(customServerModeSortEventArgs == null)
					customServerModeSortEventArgs = new CustomServerModeSortEventArgs((PivotGridField)field);
				customServerModeSortEventArgs.SetArgs(value0, value1, helper);
				handler(this, customServerModeSortEventArgs);
				return customServerModeSortEventArgs.Result;
			}
			else
				return null;
		}
		protected virtual object RaiseCustomGroupInterval(PivotGridFieldBase field, object value) {
			PivotGridCustomGroupIntervalEventHandler handler = (PivotGridCustomGroupIntervalEventHandler)this.Events[customGroupInterval];
			if(handler != null) {
				PivotCustomGroupIntervalEventArgs e = new PivotCustomGroupIntervalEventArgs(field as PivotGridField, value);
				handler(this, e);
				return e.GroupValue;
			} else
				return value;
		}
		protected virtual bool RaiseFieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			PivotAreaChangingEventHandler handler = (PivotAreaChangingEventHandler)this.Events[fieldAreaChanging];
			if(handler != null) {
				PivotAreaChangingEventArgs e = new PivotAreaChangingEventArgs(field as PivotGridField, newArea, newAreaIndex);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected virtual void RaiseFieldAreaIndexChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaIndexChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected virtual void RaiseFieldVisibleChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldVisibleChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected virtual void RaiseFieldFilterChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldFilterChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected virtual bool RaiseFieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			PivotFieldFilterChangingEventHandler handler = (PivotFieldFilterChangingEventHandler)this.Events[fieldFilterChanging];
			if(handler != null) {
				PivotFieldFilterChangingEventArgs e = new PivotFieldFilterChangingEventArgs(field as PivotGridField, filterType, showBlanks, values);
				handler(this, e);
				return e.Cancel;
			}
			return false;
		}
		protected virtual void RaiseGroupFilterChanged(PivotGridGroup group) {
			PivotGroupEventHandler handler = (PivotGroupEventHandler)this.Events[groupFilterChanged];
			if(handler != null) handler(this, new PivotGroupEventArgs(group));
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaisePageIndexChanged() {
			EventHandler handler = (EventHandler)Events[pageIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseFieldValueCollapsed(PivotFieldValueItem item) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueCollapsed];
			RaiseFieldValueCollapsedExpandedCore(item, null, handler);
		}
		protected virtual void RaiseFieldValueExpanded(PivotFieldValueItem item) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueExpanded];
			RaiseFieldValueCollapsedExpandedCore(item, null, handler);
		}
		protected virtual void RaiseFieldValueNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
			PivotFieldStateChangedEventHandler handler = (PivotFieldStateChangedEventHandler)this.Events[fieldValueNotExpanded];
			RaiseFieldValueCollapsedExpandedCore(item, field, handler);
		}
		protected void RaiseFieldValueCollapsedExpandedCore(PivotFieldValueItem item, PivotGridFieldBase field,
				PivotFieldStateChangedEventHandler handler) {
			if(handler != null) {
				PivotFieldStateChangedEventArgs e = item != null ? new PivotFieldStateChangedEventArgs(item) :
					new PivotFieldStateChangedEventArgs((PivotGridField)field);
				handler(this, e);
			}
		}
		protected virtual void RaiseControlHierarchyCreated(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[controlHierarchyCreated];
			if(handler != null) handler(this, e);
		}
		protected virtual bool RaiseFieldValueCollapsing(PivotFieldValueItem item) {
			PivotFieldStateChangedCancelEventHandler handler = (PivotFieldStateChangedCancelEventHandler)this.Events[fieldValueCollapsing];
			return RaiseFieldValueExpandingCollapsingCore(item, handler);
		}
		protected virtual bool RaiseFieldValueExpanding(PivotFieldValueItem item) {
			PivotFieldStateChangedCancelEventHandler handler = (PivotFieldStateChangedCancelEventHandler)this.Events[fieldValueExpanding];
			return RaiseFieldValueExpandingCollapsingCore(item, handler);
		}
		private bool RaiseFieldValueExpandingCollapsingCore(PivotFieldValueItem item,
				PivotFieldStateChangedCancelEventHandler handler) {
			if(handler != null) {
				PivotFieldStateChangedCancelEventArgs e = new PivotFieldStateChangedCancelEventArgs(item);
				handler(this, e);
				return !e.Cancel;
			} else
				return true;
		}
		protected void RaiseFieldExpandedInFieldGroupChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldExpandedInFieldGroupChanged];
			if(handler != null)
				handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected void RaiseFieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) {
			PivotFieldPropertyChangedEventHandler handler = (PivotFieldPropertyChangedEventHandler)this.Events[fieldPropertyChanged];
			if(handler != null)
				handler(this, new PivotFieldPropertyChangedEventArgs(field as PivotGridField, propertyName));
		}
		protected virtual void RaiseFieldUnboundExpressionChanged(PivotGridFieldBase field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldUnboundExpressionChanged];
			if(handler != null) handler(this, new PivotFieldEventArgs(field as PivotGridField));
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridFieldBase field, object value) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(field as PivotGridField, value);
				handler(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(value);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridFieldBase field, IOLAPMember member) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(field as PivotGridField, member);
				handler(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(member);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(item, defaultText);
				handler(this, e);
				return e.DisplayText;
			} else
				return defaultText;
		}
		protected virtual object RaiseCustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			PivotCustomChartDataSourceDataEventHandler handler = (PivotCustomChartDataSourceDataEventHandler)this.Events[customChartDataSourceData];
			if(handler != null) {
				PivotCustomChartDataSourceDataEventArgs e = new PivotCustomChartDataSourceDataEventArgs(itemType, itemDataMember, fieldValueItem, cellItem, value);
				handler(this, e);
				return e.Value;
			}
			return value;
		}
		void RaiseCustomChartDataSourceRows(PivotChartDataSource ds, IList<PivotChartDataSourceRowBase> rows) {
			PivotCustomChartDataSourceRowsEventHandler handler = (PivotCustomChartDataSourceRowsEventHandler)this.Events[customChartDataSourceRows];
			if(handler != null) {
				PivotCustomChartDataSourceRowsEventArgs e = new PivotCustomChartDataSourceRowsEventArgs(ds, rows);
				handler(this, e);
			}
		}
		protected virtual string RaiseCellDisplayText(PivotGridCellItem cellItem) {
			PivotCellDisplayTextEventHandler handler = (PivotCellDisplayTextEventHandler)this.Events[customCellDisplayText];
			if(handler != null) {
				PivotCellDisplayTextEventArgs e = new PivotCellDisplayTextEventArgs(cellItem);
				handler(this, e);
				return e.DisplayText;
			} else
				return cellItem.Text;
		}
		protected virtual object RaiseCellValue(PivotGridCellItem cellItem) {
			EventHandler<PivotCellValueEventArgs> handler = (EventHandler<PivotCellValueEventArgs>)this.Events[customCellValue];
			if(handler != null) {
				PivotCellValueEventArgs e = new PivotCellValueEventArgs(cellItem);
				handler(this, e);
				return e.Value;
			} else
				return cellItem.Value;
		}
		protected virtual bool RaiseCustomFilterPopupItems(PivotGridFilterItems items) {
			EventHandler<PivotCustomFilterPopupItemsEventArgs> handler = (EventHandler<PivotCustomFilterPopupItemsEventArgs>)this.Events[customFilterPopupItems];
			if(handler != null) {
				handler(this, new PivotCustomFilterPopupItemsEventArgs(items));
				return true;
			} else
				return false;
		}
		protected virtual bool RaiseCustomFieldValueCells(PivotVisualItemsBase items) {
			EventHandler<PivotCustomFieldValueCellsEventArgs> handler = (EventHandler<PivotCustomFieldValueCellsEventArgs>)this.Events[customFieldValueCells];
			if(handler != null) {
				PivotCustomFieldValueCellsEventArgs e = new PivotCustomFieldValueCellsEventArgs(items);
				handler(this, e);
				return e.IsUpdateRequired;
			} else
				return false;
		}
		protected internal void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseBeginRefresh() {
			EventHandler handler = (EventHandler)this.Events[beginRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseEndRefresh() {
			EventHandler handler = (EventHandler)this.Events[endRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseDataSourceChanged() {
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		PivotCellDisplayTextEventArgs CreateCellDisplayTextEventArgs(PivotGridCellItem cellItem) {
			return new PivotCellDisplayTextEventArgs(cellItem);
		}
		protected virtual void RaiseCustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle) {
			PivotCustomCellStyleEventHandler handler = (PivotCustomCellStyleEventHandler)this.Events[customCellStyle];
			if(handler != null)
				handler(this, new PivotCustomCellStyleEventArgs(cellItem, cellStyle));
		}
		protected bool RaiseAddPopupMenuItem(MenuItemEnum item) {
			PivotAddPopupMenuItemEventHandler handler = (PivotAddPopupMenuItemEventHandler)this.Events[addPopupMenuItem];
			if(handler != null) {
				PivotAddPopupMenuItemEventArgs args = new PivotAddPopupMenuItemEventArgs(item);
				handler(this, args);
				return args.Add;
			}
			return true;
		}
		protected internal void RaisePopupMenuCreated(ASPxPivotGridPopupMenu menu) {
			PivotPopupMenuCreatedEventHandler handler = (PivotPopupMenuCreatedEventHandler)this.Events[popupMenuCreated];
			if(handler != null) {
				PivotPopupMenuCreatedEventArgs args = new PivotPopupMenuCreatedEventArgs(menu);
				handler(this, args);
			}
		}
		protected void RaiseDataAreaPopupCreated(PivotGridDataAreaPopup popup) {
			EventHandler<PivotDataAreaPopupCreatedEventArgs> handler = (EventHandler<PivotDataAreaPopupCreatedEventArgs>)this.Events[dataAreaPopupCreated];
			if(handler != null) {
				PivotDataAreaPopupCreatedEventArgs args = new PivotDataAreaPopupCreatedEventArgs(popup);
				handler(this, args);
			}
		}
		protected void RaiseOLAPQueryTimeout() {
			EventHandler handler = (EventHandler)this.Events[olapQueryTimeout];
			if(handler != null) handler(this, new EventArgs());
		}
		protected bool RaiseQueryException(Exception ex) {
			  PivotOlapExceptionEventArgs e = null;
			PivotOlapExceptionEventHandler handler = (PivotOlapExceptionEventHandler)this.Events[olapException];
			if(handler != null) {
				e = new PivotOlapExceptionEventArgs(ex);
				handler(this, e);
			}
			PivotQueryExceptionEventHandler handler2 = (PivotQueryExceptionEventHandler)this.Events[queryException];
			if(handler2 != null) {
				if(e == null)
					e = new PivotOlapExceptionEventArgs(ex);
				handler2(this, e);
			}
			return e == null ? false : e.Handled;
		}
		protected void RaiseLayoutChanged() {
			EventHandler handler = (EventHandler)this.Events[layoutChanged];
			if(handler != null) handler(this, new EventArgs());
		}
		protected internal void RaiseCustomCallback(string parameters) {
			PivotCustomCallbackEventHandler handler = (PivotCustomCallbackEventHandler)this.Events[customCallback];
			if(handler != null) handler(this, new PivotGridCustomCallbackEventArgs(parameters));
		}
		bool raiseAfterPerformCallbackCalled;
		protected internal void RaiseAfterPerformCallback() {
			if(this.raiseAfterPerformCallbackCalled) return;
			RaiseAfterPerformCallbackCore();
			this.raiseAfterPerformCallbackCalled = true;
		}
		protected internal void RaiseAfterPerformCallbackCore() {
			EventHandler handler = (EventHandler)this.Events[afterPerformCallback];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseBeforePerformDataSelect() {
			EventHandler handler = (EventHandler)this.Events[beforePerformDataSelect];
			if(handler != null) handler(this, new EventArgs());
		}
		protected internal void RaisePrefilterCriteriaChanged() {
			EventHandler handler = (EventHandler)this.Events[prefilterCriteriaChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseHtmlCellPrepared(PivotGridHtmlDataCell cell) {
			PivotHtmlCellPreparedEventHandler handler = (PivotHtmlCellPreparedEventHandler)this.Events[htmlCellPrepared];
			if(handler != null) handler(this, new PivotHtmlCellPreparedEventArgs(cell));
		}
		protected internal void RaiseHtmlFieldValuePrepared(PivotGridHtmlFieldValueCellBase cell) {
			PivotHtmlFieldValuePreparedEventHandler handler = (PivotHtmlFieldValuePreparedEventHandler)this.Events[htmlFieldValuePrepared];
			if(handler != null) handler(this, new PivotHtmlFieldValuePreparedEventArgs(cell));
		}
		protected bool RaiseSaveCallbackState(ref string callbackState) {
			PivotGridCallbackStateEventHandler handler = (PivotGridCallbackStateEventHandler)Events[customSaveCallbackState];
			if(handler != null) {
				PivotGridCallbackStateEventArgs e = new PivotGridCallbackStateEventArgs(callbackState);
				handler(this, e);
				callbackState = e.CallbackState;
				return e.Handled;
			}
			return false;
		}
		protected bool RaiseLoadCallbackState(out string callbackState) {
			callbackState = null;
			PivotGridCallbackStateEventHandler handler = (PivotGridCallbackStateEventHandler)Events[customLoadCallbackState];
			if(handler != null) {
				PivotGridCallbackStateEventArgs e = new PivotGridCallbackStateEventArgs(callbackState);
				handler(this, e);
				callbackState = e.CallbackState;
				return e.Handled;
			}
			return false;
		}
		public void CollapseAll() {
			Data.ChangeExpandedAll(false);
		}
		public void CollapseAllRows() {
			Data.ChangeExpandedAll(false, false);
		}
		public void CollapseAllColumns() {
			Data.ChangeExpandedAll(true, false);
		}
		public void ExpandAll() {
			Data.ChangeExpandedAll(true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public void CollapseValue(bool isColumn, object[] values) {
			Data.ChangeExpanded(isColumn, values, false);
		}
		public void ExpandValue(bool isColumn, object[] values) {
			Data.ChangeExpanded(isColumn, values, true);
		}
		public void ExpandValue(bool isColumn, int lastLevelIndex) {
			ChangeValueExpanded(isColumn, lastLevelIndex, true);
		}
		protected void ChangeValueExpanded(bool isColumn, int lastLevelIndex, bool expand) {
			PivotFieldValueItem valueItem = VisualItems.GetLastLevelUnpagedItem(isColumn, lastLevelIndex);
			if(valueItem.IsCollapsed != expand) 
				return;
			Data.ChangeExpanded(valueItem, false);
		}
		public bool IsObjectCollapsed(PivotGridField field, int lastLevelIndex) {
			EnsureRefreshData();
			return VisualItems.IsObjectCollapsed(field, lastLevelIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedCellValue(columnIndex, rowIndex);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			EnsureRefreshData();
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		[Browsable(false), 
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridColumnCount")
#else
	Description("")
#endif
]
		public int ColumnCount {
			get {
				EnsureRefreshData();
				return VisualItems.GetLastLevelUnpagedItemCount(true);
			}
		}
		[Browsable(false), 
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridRowCount")
#else
	Description("")
#endif
]
		public int RowCount {
			get {
				EnsureRefreshData();
				return VisualItems.UnpagedRowCount;
			}
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridVisibleRowsOnPage")]
#endif
		public int VisibleRowsOnPage {
			get {
				EnsureRefreshData();
				return VisualItems.RowCount;
			}
		}
		public PivotCellBaseEventArgs GetCellInfo(int columnIndex, int rowIndex) {
			EnsureRefreshData();
			return new PivotCellBaseEventArgs(VisualItems.CreateCellItem(columnIndex, rowIndex));
		}
		public int GetAbsoluteRowIndex(int pageRowVisibleIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedRowIndex(pageRowVisibleIndex);
		}
		public int GetColumnIndex(object[] values) {
			EnsureRefreshData();
			return Data.GetFieldValueIndex(true, values);
		}
		public int GetRowIndex(object[] values) {
			EnsureRefreshData();
			return Data.GetFieldValueIndex(false, values);
		}
		public int GetColumnIndex(object[] values, PivotGridField field) {
			EnsureRefreshData();
			return Data.GetFieldValueIndex(true, values, field);
		}
		public int GetRowIndex(object[] values, PivotGridField field) {
			EnsureRefreshData();
			return Data.GetFieldValueIndex(false, values, field);
		}
		public bool IsFieldValueCollapsed(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.IsUnpagedObjectCollapsed(field, cellIndex);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedFieldValue(field, cellIndex);
		}
		public PivotGridValueType GetFieldValueType(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedItem(field, cellIndex).ValueType;
		}
		public PivotGridValueType GetFieldValueType(bool isColumn, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetLastLevelItem(isColumn, cellIndex, false).ValueType;
		}
		public IOLAPMember GetFieldValueOLAPMember(PivotGridField field, int cellIndex) {
			EnsureRefreshData();
			return VisualItems.GetUnpagedOLAPMember(field, cellIndex);
		}
		public object GetFieldValueByIndex(PivotGridField field, int fieldValueIndex) {
			EnsureRefreshData();
			return Data.GetFieldValue(field, fieldValueIndex, fieldValueIndex);
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			if(!Data.IsInternalSerialization) {
				RaiseBeforeLoadLayout(e);
				if(!e.Allow) return;
			}
			e.Allow = true;
			Data.SetIsDeserializing(true);
			Data.BeginUpdate();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			if(!Data.IsInternalSerialization && restoredVersion != OptionsLayout.LayoutVersion) {
				RaiseClientLayout(new ASPxClientLayoutArgs(ClientLayoutMode.Loading, restoredVersion));
				RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion)); 
			}
			Data.OnDeserializationComplete();
			Data.EndUpdate();
			Data.SetIsDeserializing(false);
			ResetControlHierarchy();
		}
		void IXtraSerializable.OnStartSerializing() { }
		void IXtraSerializable.OnEndSerializing() { }
		#endregion
		public override bool IsLoading() {
			return base.IsLoading() || Data.IsDeserializing;
		}
		#region Skins
		protected override string GetSkinControlName() {
			return "PivotGrid";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Web", "Editors" };
		}
		#endregion
		#region Loading Panel Members
		internal new ImageProperties LoadingPanelImage { get { return base.LoadingPanelImage; } }
		internal new LoadingPanelStyle LoadingPanelStyle { get { return base.LoadingPanelStyle; } }
		internal new SettingsLoadingPanel SettingsLoadingPanel { get { return base.SettingsLoadingPanel; } }
		#endregion
		#region ChartDataSource Members
		PivotChartDataSourceView chartDataSourceView;
		protected virtual PivotChartDataSourceView ChartDataSourceView {
			get {
				if(!Data.IsDataBound) DataBind();
				if(chartDataSourceView == null)
					chartDataSourceView = CreateChartDataSourceView();
				return chartDataSourceView;
			}
		}
		protected EventHandler ChartDataSourceChanged;
		protected virtual void RaiseChartDataSourceChanged() {
			if(Data.ChartDataSource.IsListChangedLocked)
				return;
			if(ChartDataSourceChanged != null)
				ChartDataSourceChanged(this, EventArgs.Empty);
		}
		protected virtual void InvalidateChartData() {
			if(chartDataSourceView != null)
				chartDataSourceView.InvalidateChartData();
			RaiseChartDataSourceChanged();
		}
		protected virtual PivotChartDataSourceView CreateChartDataSourceView() {
			return new PivotChartDataSourceView(this);
		}
		event EventHandler IDataSource.DataSourceChanged { add { ChartDataSourceChanged += value; } remove { ChartDataSourceChanged -= value; } }
		DataSourceView IDataSource.GetView(string viewName) {
			if(viewName != string.Empty) return null;
			return ChartDataSourceView;
		}
		ICollection IDataSource.GetViewNames() {
			return new string[] { string.Empty };
		}
		protected virtual void OnOptionsChartDataSourceChanged(object sender, EventArgs e) {
			InvalidateChartData();
		}
		protected virtual void OnActiveLocalizerChanged(object sender, EventArgs e) {
			ResetControlHierarchy();
			RefreshCustomizationFieldList();
		}
		#endregion
		#region IPopupFilterControlOwner Members
		void IPopupFilterControlOwner.CloseFilterControl() {
			IsPrefilterPopupVisible = false;
		}
		string IPopupFilterControlOwner.FilterPopupHeaderText {
			get { return PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.PrefilterFormCaption); }
		}
		object IPopupFilterControlOwner.GetControlCallbackResult() {
			addCustomJSPropertiesScript = true;
			return GetCallbackResult();
		}
		FilterControlImages IPopupFilterControlOwner.GetImages() {
			return ImagesPrefilterControl;
		}
		EditorImages IPopupFilterControlOwner.GetImagesEditors() {
			return ImagesEditors;
		}
		string IPopupFilterControlOwner.GetJavaScriptForApplyFilterControl() {
			return String.Format("ASPx.pivotGrid_ApplyPrefilter('{0}');", ClientID);
		}
		string IPopupFilterControlOwner.GetJavaScriptForCloseFilterControl() {
			return String.Format("ASPx.pivotGrid_HidePrefilter('{0}');", ClientID);
		}
		FilterControlStyles IPopupFilterControlOwner.GetStyles() {
			return StylesFilterControl;
		}
		EditorStyles IPopupFilterControlOwner.GetStylesEditors() {
			return StylesEditors;
		}
		string IPopupFilterControlOwner.MainElementID {
			get { return ElementNames.MainTD; }
		}
		bool IPopupFilterControlOwner.EnablePopupMenuScrolling {
			get { return OptionsView.EnableFilterControlPopupMenuScrolling; }
		}
		ASPxWebControl IPopupFilterControlOwner.OwnerControl {
			get { return this; }
		}
		SettingsLoadingPanel IPopupFilterControlOwner.SettingsLoadingPanel {
			get { return SettingsLoadingPanel; }
		}
		#endregion
		#region IFilterControlRowOwner Members
		void IFilterControlRowOwner.AppendDefaultDXClassName(WebControl control) {
			Styles.AppendDefaultDXClassName(control);
		}
		void IFilterControlRowOwner.AssignCheckBoxCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelCheckBoxCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignClearButtonCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelClearButtonCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignExpressionCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelExpressionCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignFilterStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignImageCellStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelImageCellStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignLinkStyleToControl(WebControl control) {
			Styles.GetPrefilterPanelLinkStyle().AssignToControl(control);
		}
		string IFilterControlRowOwner.ClearButtonText {
			get { return string.Empty; }   
		}
		ImageProperties IFilterControlRowOwner.CreateFilterImage {
			get { return RenderHelper.GetPrefilterImage(); }
		}
		string IFilterControlRowOwner.GetJavaScriptForClearFilter() {
			return String.Format("ASPx.pivotGrid_ClearPrefilter('{0}');", ClientID);
		}
		string IFilterControlRowOwner.GetJavaScriptForSetFilterEnabledForCheckbox() {
			return String.Format("ASPx.pivotGrid_ChangePrefilterEnabled('{0}');", ClientID);
		}
		string IFilterControlRowOwner.GetJavaScriptForShowFilterControl() {
			return String.Format("ASPx.pivotGrid_ShowPrefilter('{0}');", ClientID);
		}
		bool IFilterControlRowOwner.IsFilterEnabled {
			get { return Prefilter.Enabled; }
		}
		bool IFilterControlRowOwner.IsFilterEnabledSupported {
			get { return true; }
		}
		string IFilterControlRowOwner.ShowFilterBuilderText {
			get { return PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.PopupMenuShowPrefilter); }
		}
		void IFilterControlRowOwner.RaiseCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			if(data.Prefilter.State == PrefilterState.Invalid) e.DisplayText = PrefilterBase.InvalidCriteriaDisplayText;
			CustomFilterExpressionDisplayTextEventHandler handler = Events[customFilterExpressionDisplayText] as CustomFilterExpressionDisplayTextEventHandler;
			if(handler == null) return;
			handler(this, e);
		}
		#endregion
		#region IFilterControlOwner Members
		bool IFilterControlOwner.IsRightToLeft {
			get { return IsRightToLeft(); }
		}
		string IFilterControlOwner.FilterExpression {
			get {
				return Data.Prefilter.CriteriaString;
			}
			set {
				PostBackAction = new PivotGridPostBackPrefilterAction(this, PivotGridPrefilterCommand.Set, value);
			}
		}
		IFilterColumn GetFilterColumn(int index) {
			EnsureRefreshData();
			IList visibleFields = Fields.GetPrefilterFields(Prefilter.PrefilterColumnNames);
			return index >= 0 && index < visibleFields.Count ? (IFilterColumn)visibleFields[index] : null;
		}
		int GetColumnCount() {
			return Fields.GetPrefilterFields(Prefilter.PrefilterColumnNames).Count;
		}
		FilterControlColumnCollection IFilterControlOwner.GetFilterColumns() {
			if(FilterControlCachedColumns.IsEmpty)
				FilterControlCachedColumns = GenerateFilterColumns();
			return FilterControlCachedColumns;
		}
		FilterControlColumnCollection GenerateFilterColumns() {
			var externalColumns = new List<IFilterColumn>();
			for(int i = 0; i < GetColumnCount(); i++)
				externalColumns.Add(GetFilterColumn(i));
			FilterControlColumnBuilder columnBuilder = new FilterControlColumnBuilder();
			return columnBuilder.GenerateColumns(null, false, 0, false, externalColumns);
		}
		bool IFilterControlOwner.TryGetSpecialValueDisplayText(IFilterColumn column, object value, bool encodeValue, out string displayText) {
			displayText = ((PivotGridField)column).GetDisplayText(value);
			if(encodeValue)
				displayText = HtmlEncode(displayText);
			return true;
		}
		bool IFilterControlOwner.IsOperationHiddenByUser(IFilterablePropertyInfo propertyInfo, DevExpress.Data.Filtering.Helpers.ClauseType operation) {
			FilterControlOperationVisibilityEventHandler handler = (FilterControlOperationVisibilityEventHandler)Events[filterControlOperationVisibility];
			if(handler == null)
				return false;
			FilterControlOperationVisibilityEventArgs e = new FilterControlOperationVisibilityEventArgs(propertyInfo, operation);
			handler(this, e);
			return !e.Visible;
		}
		bool IFilterControlOwner.TryConvertValue(IFilterablePropertyInfo propertyInfo, string text, out object value) {
			value = null;
			FilterControlParseValueEventHandler handler = (FilterControlParseValueEventHandler)Events[filterControlParseValue];
			if(handler == null)
				return false;
			FilterControlParseValueEventArgs e = new FilterControlParseValueEventArgs(propertyInfo, text);
			handler(this, e);
			if(e.Handled)
				value = e.Value;
			return e.Handled;
		}
		void IFilterControlOwner.RaiseCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e) {
			var handler = (FilterControlCustomValueDisplayTextEventHandler)Events[filterControlCustomValueDisplayText];
			if(handler != null)
				handler(this, e);
		}
		FilterControlViewMode IFilterControlOwner.ViewMode { get { return FilterControlViewMode.Visual; } }
		bool IFilterControlOwner.ShowOperandTypeButton { get { return false; } }
		FilterControlGroupOperationsVisibility IFilterControlOwner.GroupOperationsVisibility { get { 
			return new FilterControlGroupOperationsVisibility(this); } 
		}
		void IFilterControlOwner.RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e) {}
		void IFilterControlOwner.RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e) { }
		#endregion
		#region IPopupFilterControlStyleOwner Members
		AppearanceStyle IPopupFilterControlStyleOwner.ButtonAreaStyle {
			get { return Styles.GetPrefilterBuilderButtonAreaStyle(); }
		}
		ImageProperties IPopupFilterControlStyleOwner.CloseButtonImage {
			get { return RenderHelper.GetCustomizationFieldsCloseImage(); }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.CloseButtonStyle {
			get { return Styles.PrefilterBuilderCloseButtonStyle; }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.HeaderStyle {
			get { return Styles.PrefilterBuilderHeaderStyle; }
		}
		AppearanceStyle IPopupFilterControlStyleOwner.MainAreaStyle {
			get { return Styles.GetPrefilterBuilderMainAreaStyle(); }
		}
		AppearanceStyleBase IPopupFilterControlStyleOwner.ModalBackgroundStyle {
			get { return Styles.PrefilterBuilderModalBackgroundStyle; }
		}
		#endregion
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			PivotGridWebOptionsLayout opts = options as PivotGridWebOptionsLayout;
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIds.ClientSideEvents:
					return opts.StoreClientSideEvents;
				case LayoutIds.LayoutIdAppearance:
					return opts.StoreAppearance || opts.Columns.StoreAppearance;
				case LayoutIds.LayoutIdData:
					return opts.StoreDataSettings;
				case LayoutIds.LayoutIdOptionsView:
					return opts.StoreVisualOptions;
				case LayoutIds.LayoutIdLayout:
					return opts.Columns.StoreLayout;
			}
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			PivotGridWebOptionsLayout opts = options as PivotGridWebOptionsLayout;
			ResetPrefilter();
			IsPrefilterPopupVisible = false;
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsChartDataSource) != 0)
				OptionsChartDataSource.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsCustomization) != 0)
				OptionsCustomization.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsData) != 0)
				OptionsData.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsDataField) != 0)
				OptionsDataField.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsLoadingPanel) != 0)
				OptionsLoadingPanel.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsPager) != 0)
				OptionsPager.Reset();
			if(opts == null || opts.StoreVisualOptions || opts.StoreAllOptions)
				OptionsView.Reset();
			if(opts == null) return;
			if((opts.ResetOptions & PivotGridResetOptions.OptionsOLAP) != 0)
				OptionsOLAP.Reset();
		}
		#endregion
		#region IMasterControl Members
		string IMasterControl.CalcRelatedControlsCallbackResult() {
			return masterControlImplementation.CalcRelatedControlsCallbackResult();
		}
		void IMasterControl.RegisterRelatedControl(IRelatedControl control) {
			if(control.MasterControlID != ID) return;
			masterControlImplementation.RegisterRelatedControl(control);
			if(customizationFields != null)
				customizationFields.Visible = false;
			if(headerMenu != null) {
				DevExpress.Web.MenuItem item = headerMenu.Items.FindByName(ASPxPivotGridPopupMenu.HideFieldListID);
				if(item != null)
					item.Visible = false;
				item = headerMenu.Items.FindByName(ASPxPivotGridPopupMenu.ShowFieldListID);
				if(item != null)
					item.Visible = false;
			}
			ResetControlHierarchy();
		}
		#endregion
		#region ISupportsCallbackResult Members
		public CallbackResult CalcCallbackResult() {
			RelatedControlDefaultImplementation impl = new PivotGridRelatedControlDefaultImplementation(this);
			return impl.CalcCallbackResult();
		}
		#endregion
		#region ISupportsFieldsCustomization Members
		string ISupportsFieldsCustomization.UniqueID { get { return UniqueID; } }
		string ISupportsFieldsCustomization.ClientID { get { return ClientID; } }
		PivotGridWebData ISupportsFieldsCustomization.Data { get { return Data; } }
		PivotGridStyles ISupportsFieldsCustomization.Styles { get { return Styles; } }
		CustomizationFormStyle ISupportsFieldsCustomization.FormStyle {
			get { return Data.OptionsCustomization.CustomizationFormStyle; }
		}
		CustomizationFormLayout ISupportsFieldsCustomization.FormLayout {
			get { return Data.OptionsCustomization.CustomizationFormLayout; }
			set { Data.OptionsCustomization.CustomizationFormLayout = value; }
		}
		bool ISupportsFieldsCustomization.DeferredUpdates {
			get { return Data.OptionsCustomization.DeferredUpdates; }
			set { Data.OptionsCustomization.DeferredUpdates = value; }
		}
		bool ISupportsFieldsCustomization.AllowSortInForm {
			get { return Data.OptionsCustomization.AllowSortInCustomizationForm; }
		}
		bool ISupportsFieldsCustomization.AllowFilterInForm {
			get { return Data.OptionsCustomization.AllowFilterInCustomizationForm; }
		}
		ASPxPivotGridPopupMenu ISupportsFieldsCustomization.LayoutMenu {
			get { return FieldListMenu; }
		}
		PivotCustomizationFormImages ISupportsFieldsCustomization.Images {
			get { return CustomizationFormImages; }
		}
		CustomizationFormAllowedLayouts ISupportsFieldsCustomization.AllowedFormLayouts {
			get { return OptionsCustomization.CustomizationFormAllowedLayouts; }
		}
		#endregion
		#region IPivotGridEventsImplementorBase Members
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(Data.IsObjectCollapsed(item.IsColumn, item.VisibleIndex))
				RaiseFieldValueCollapsed(item);
			else
				RaiseFieldValueExpanded(item);
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
			RaiseFieldValueNotExpanded(item, field);
		}
		bool IPivotGridEventsImplementorBase.BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(item.IsCollapsed)
				return RaiseFieldValueExpanding(item);
			else
				return RaiseFieldValueCollapsing(item);
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
			RaiseBeginRefresh();
		}
		void IPivotGridEventsImplementorBase.CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			RaiseCustomSummary(field, customSummaryInfo);
		}
		string IPivotGridEventsImplementorBase.CustomCellDisplayText(PivotGridCellItem cellItem) {
			return RaiseCellDisplayText(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomCellValue(PivotGridCellItem cellItem) {
			return RaiseCellValue(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return RaiseCustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		void IPivotGridEventsImplementorBase.CustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows) {
			RaiseCustomChartDataSourceRows((Data.ChartDataSource as PivotChartDataSource), rows);
		}
		bool IPivotGridEventsImplementorBase.CustomFieldValueCells(PivotVisualItemsBase items) {
			return RaiseCustomFieldValueCells(items);
		}
		bool IPivotGridEventsImplementorBase.CustomFilterPopupItems(PivotGridFilterItems items) {
			return RaiseCustomFilterPopupItems(items);
		}
		object IPivotGridEventsImplementorBase.CustomGroupInterval(PivotGridFieldBase field, object value) {
			return RaiseCustomGroupInterval(field, value);
		}
		void IPivotGridEventsImplementorBase.DataSourceChanged() {
			if(Data.IsDataBound)
				HasContentInternal = true;
			RaiseDataSourceChanged();
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
			RaiseEndRefresh();
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldAreaChanged(field);
		}
		bool IPivotGridEventsImplementorBase.FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			return RaiseFieldAreaChanging(field, newArea, newAreaIndex);
		}
		void IPivotGridEventsImplementorBase.FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldExpandedInFieldGroupChanged(field);
		}
		void IPivotGridEventsImplementorBase.FieldFilterChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldFilterChanged(field);
		}
		bool IPivotGridEventsImplementorBase.FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return RaiseFieldFilterChanging(field, filterType, showBlanks, values);
		}
		void IPivotGridEventsImplementorBase.FieldAreaIndexChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldAreaIndexChanged(field);
		}
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldVisibleChanged(field);
		}
		void IPivotGridEventsImplementorBase.FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) {
			if(IsLoading()) return;
			RaiseFieldPropertyChanged(field, propertyName);
		}
		void IPivotGridEventsImplementorBase.FieldUnboundExpressionChanged(PivotGridFieldBase field) {
			if(IsLoading()) return;
			RaiseFieldUnboundExpressionChanged(field);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			return RaiseFieldValueDisplayText(item, defaultText);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, object value) {
			return RaiseFieldValueDisplayText(field, value);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember value) {
			return RaiseFieldValueDisplayText(field, value);
		}
		void IPivotGridEventsImplementorBase.FieldWidthChanged(PivotGridFieldBase field) {
		}
		int? IPivotGridEventsImplementorBase.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		int? IPivotGridEventsImplementorBase.QuerySorting(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			return RaiseQuerySorting(value0, value1, field, helper);
		}
		object IPivotGridEventsImplementorBase.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return RaiseCustomUnboundColumnData(field, listSourceRowIndex, expValue);
		}
		void IPivotGridEventsImplementorBase.GroupFilterChanged(PivotGridGroup group) {
			if(IsLoading()) return;
			RaiseGroupFilterChanged(group);
		}
		void IPivotGridEventsImplementorBase.OLAPQueryTimeout() {
			if(!IsLoading()) RaiseOLAPQueryTimeout();
		}
		bool IPivotGridEventsImplementorBase.QueryException(Exception ex) {
			if(IsLoading()) return false;
			return RaiseQueryException(ex);
		}
		void IPivotGridEventsImplementorBase.PrefilterCriteriaChanged() {
			if(IsLoading()) return;
			RaisePrefilterCriteriaChanged();
		}
		void IPivotGridEventsImplementorBase.LayoutChanged() {
			if(IsLoading()) return;
			if(!IsLockRefreshData)
				RequireDataUpdate = true;
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == fieldsPropertyName)
				return XtraCreateFieldsItem(e);
			if(propertyName == groupsPropertyName)
				return XtraCreateGroupsItem(e);
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) { }
		#endregion
		#region IXtraSupportDeserializeCollection Members
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName == fieldsPropertyName) {
				XtraClearFields(e);
				return true;
			}
			if(propertyName == groupsPropertyName) {
				XtraClearGroups(e);
				return true;
			}
			return false;
		}
		#endregion
		#region IXtraSerializableLayout Members
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		#endregion
		#region IHeaderFilterPopupOwner
		bool IHeaderFilterPopupOwner.ShowButtonPanel { get { return true; } }
		Unit IHeaderFilterPopupOwner.PopupHeight { get { return RenderHelper.GetHeaderFilterPopupHeight(); } }
		Unit IHeaderFilterPopupOwner.PopupWidth { get { return RenderHelper.GetHeaderFilterPopupWidth(); } }
		Unit IHeaderFilterPopupOwner.PopupMinHeight { get { return Data.OptionsCustomization.FilterPopupWindowMinHeight; } }
		Unit IHeaderFilterPopupOwner.PopupMinWidth { get { return Data.OptionsCustomization.FilterPopupWindowMinWidth; } }
		ResizingMode IHeaderFilterPopupOwner.PopupResizeMode { get { return Data.OptionsCustomization.FilterPopupWindowResizeMode; } }
		bool IHeaderFilterPopupOwner.PopupCloseOnEscape { get { return false; } }
		AppearanceStyleBase IHeaderFilterPopupOwner.ControlStyle { get { return Data.GetFilterWindowStyle(); } }
		AppearanceStyleBase IHeaderFilterPopupOwner.ContentStyle { get { return Data.GetFilterItemsAreaStyle(); } }
		AppearanceStyleBase IHeaderFilterPopupOwner.FooterStyle { get { return Data.GetFilterButtonPanelStyle(); } }
		ImageProperties IHeaderFilterPopupOwner.SizeGrip { get { return RenderHelper.GetFilterWindowSizeGripImage(); } }
		ImageProperties IHeaderFilterPopupOwner.SizeGripRtl { get { return null; } }
		string IHeaderFilterPopupOwner.OkButtonText { get { return PivotGridLocalizer.GetString(PivotGridStringId.FilterOk); } }
		string IHeaderFilterPopupOwner.CancelButtonText { get { return PivotGridLocalizer.GetString(PivotGridStringId.FilterCancel); } }
		string IHeaderFilterPopupOwner.OkButtonClickScript { get { return ScriptHelper.FilterPopupOkButtonClick; } }
		string IHeaderFilterPopupOwner.CancelButtonClickScript { get { return ScriptHelper.FilterPopupCancelButtonClick; } }
		HeaderFilterButtonPanelStyles IHeaderFilterPopupOwner.ButtonPanelStyles {
			get {
				HeaderFilterButtonPanelStyles style = new HeaderFilterButtonPanelStyles(this);
				style.OkButton.CopyFrom(Data.GetFilterButtonStyle());
				style.CancelButton.CopyFrom(Data.GetFilterButtonStyle());
				style.ButtonSpacing = Data.GetFilterButtonPanelSpacing();
				return style;
			}
		}
		#endregion
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		int IPagerOwner.InitialPageSize {
			get { return InitialPageSize; }
		}
		protected internal bool GetCanDragHeaders(){
			foreach(PivotGridField field in Fields) {
				if(field.CanDragInCustomizationForm || field.CanDrag)
					return true;
			}
			return false;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxPivotGrid.Design.PivotGridCommonFormDesigner"; } }
	}
	class FilterManager {
		public static FilterManager CreateEmpty() {
			return new FilterManager(-1, null, null, null, false, false);
		}
		public static FilterManager Create(string separatedArgs, bool isGroup) {
			int index;
			string[] parts = separatedArgs.Split(CallbackCommands.ArgumentsSeparator);
			if(!int.TryParse(parts[1], out index))
				return null;
			bool defereFilter = !isGroup && parts.Length > 2 && parts[2] == "D";
			string state = isGroup ? parts[2] : null,
				values = isGroup ? parts[3] : null, arg = null;
			if(isGroup) {
				int subIndex = 0;
				for(int i = 0; i <= 3; i++) {
					subIndex += parts[i].Length + 1;
				}
				arg = separatedArgs.Substring(subIndex);
			}
			return new FilterManager(index, state, values, arg, defereFilter, isGroup);
		}
		int fieldIndex;
		string callbackArgs;
		string state;
		string values;
		bool deferredFilter = false;
		bool isGroupCallback = false;
		protected FilterManager(int fieldIndex, string state, string values, string arg, bool deferredFilter, bool isGroupCallback) {
			this.fieldIndex = fieldIndex;
			this.state = state;
			this.values = values;
			this.callbackArgs = arg;
			this.deferredFilter = deferredFilter;
			this.isGroupCallback = isGroupCallback;
		}
		public bool IsGroupCallback { get { return isGroupCallback; } }
		public bool DeferredFilter { get { return deferredFilter; } }
		public int FieldIndex { get { return fieldIndex; } }
		public string CallbackArgs { get { return callbackArgs; } }
		public string State { get { return state; } }
		public string Values { get { return values; } }
	}
	class MoreThanOneCustFieldsFormsException : Exception {
		public MoreThanOneCustFieldsFormsException() : base() { }
		public override string Message {
			get { return "Cannot add more than one customization form to the same ASPxPivotGrid control"; }
		}
	}
	internal class PivotGridMasterControlDefaultImplementation : MasterControlDefaultImplementation {
		ASPxPivotGrid pivotGrid;
		public PivotGridMasterControlDefaultImplementation(ASPxPivotGrid control) {
			if(control == null)
				throw new ArgumentNullException("control");
			this.pivotGrid = control;
		}
		public ASPxPivotGrid Control { get { return pivotGrid; } }
		protected override bool ShouldCalcCallbackResult(ISupportsCallbackResult control) {
			return base.ShouldCalcCallbackResult(control);
		}
		public bool ContainsRelatedCustomizationForm { get { return (CustomizationForm != null); } }
		public ASPxPivotCustomizationControl CustomizationForm {
			get {
				if(RelatedControls.Count > 1)
					throw new MoreThanOneCustFieldsFormsException();
				for(int i = 0; i < RelatedControls.Count; i++)
					if(RelatedControls[i].GetType() == typeof(ASPxPivotCustomizationControl) || RelatedControls[i].GetType().IsSubclassOf(typeof(ASPxPivotCustomizationControl)))
						return (ASPxPivotCustomizationControl)RelatedControls[i];
				return null;
			}
		}
		public void RefreshCustomizationForm() {
			if(CustomizationForm != null)
				CustomizationForm.Refresh();
		}
		public void PrepareCustomizationFieldsMenu() {
			CustomizationForm.PrepareLayoutMenu();
		}
	}
	class PivotGridRelatedControlDefaultImplementation : RelatedControlDefaultImplementation {
		public PivotGridRelatedControlDefaultImplementation(ASPxPivotGrid control)
			: base(control, new FakeIRelatedControl()) {
			if(control == null)
				throw new ArgumentNullException("control");
		}
		ASPxPivotGrid PivotGrid { get { return Control as ASPxPivotGrid; } }
		protected override string GenerateInnerHtml() {
			return PivotGrid.GetMasterCallbackResultString();
		}
	}
	public enum PivotCollapsedStateStoreMode { Indexes, Values };
	public class ASPxPivotGridDataHelper : DataHelper {
		public ASPxPivotGridDataHelper(ASPxPivotGrid pivot, string name) : base(pivot, name) { }
		public ASPxPivotGrid Pivot { get { return (ASPxPivotGrid)Control; } }
		public override void PerformSelect() {
			if(Pivot.DesignMode && !string.IsNullOrEmpty(Pivot.DataSourceID)) return;
			Pivot.RaiseBeforePerformDataSelect();
			ResetSelectArguments();
			base.PerformSelect();
		}
		protected override void ValidateDataSource(object dataSource) {
			if(dataSource is IQueryDataSource)
				return;
			base.ValidateDataSource(dataSource);
		}
	}
}
