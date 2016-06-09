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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Linq;
using DevExpress.Web.Internal;
using DevExpress.Web.FilterControl;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using System.IO;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Web.Design;
using DevExpress.Data;
namespace DevExpress.Web {
	[DefaultProperty("FilterExpression"), 
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxFilterControl")
	]
	public abstract class ASPxFilterControlBase : ASPxWebControl, IFilterControlOwner, IFilterControlOperationsOwner {
		private class CriteriaValidatorInternal : EvaluatorCriteriaValidator {
			private bool isCriteriaOperatorValid = true;
			private CriteriaValidatorInternal() : base(null) { }
			public static bool IsCriteriaOperatorValid(CriteriaOperator criteria) {
				CriteriaValidatorInternal validator = new CriteriaValidatorInternal();
				validator.Validate(criteria);
				return validator.isCriteriaOperatorValid;
			}
			public override void Visit(OperandValue theOperand) {
				if(theOperand.Value == null)
					isCriteriaOperatorValid = false;
			}
			public override void Visit(JoinOperand theOperand) {
			}
			public override void Visit(OperandProperty theOperand) {
			}
			public override void Visit(AggregateOperand theOperand) {
			}
		}
		protected internal const string FilterControlScriptsResourcePath = "DevExpress.Web.Scripts.Editors.";
		protected internal const string FilterControlScriptResourceName = FilterControlScriptsResourcePath + "FilterControl.js";
		protected const string FilterExpressionStateKey = "filterExpression";
		protected const string AppliedFilterExpressionStateKey = "appliedFilterExpression";
		protected const string FilterValueStateKey = "filterValue";
		protected const string IsFilterExpressionValidStateKey = "isFilterExpressionValid";
		protected const string TextTabExpressionValidStateKey = "textTabExpressionValid";
		WebFilterControlRenderHelper renderHelper;
		WebControl contentControl;
		Image buttonAddHotImage, buttonRemoveHotImage;
		Table rootTable;
		FilterControlPopupMenu operationPopup, groupPopup, aggregatePopup;
		ASPxWebControl fieldNamePopup;
		WebFilterTreeModel model;
		static readonly object
			EventOperationVisibility = new object(),
			EventParseValue = new object(),
			EventCustomValueDisplayText = new object(),
			EventCriteriaValueEditorInitialize = new object(),
			EventCriteriaValueEditorCreate = new object();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public WebFilterTreeModel Model { get { return model; } }
		WebFilterTreeModel CreateTreeModel() {
			WebFilterTreeModel m = new WebFilterTreeModel(this);
			m.ShowIsNullOperatorsForStrings = true;
			return m;
		}
		public ASPxFilterControlBase() {
			this.renderHelper = CreateRenderHelper();
			EnableCallBacks = true;
			EnableClientSideAPIInternal = true;
			this.model = CreateTreeModel();
			GroupOperationsVisibility = new FilterControlGroupOperationsVisibility(this);
		}
		protected override StylesBase CreateStyles() { return new FilterControlStyles(this); }
		protected override ImagesBase CreateImages() { return new FilterControlImages(this); }
		protected internal FilterControlImages RenderImages {
			get { return (FilterControlImages)RenderImagesInternal; }
		}
		protected internal FilterControlStyles RenderStyles {
			get { return (FilterControlStyles)RenderStylesInternal; }
		}
		protected override string GetSkinControlName() {
			return "Editors";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Web" };
		}
		protected override bool HasLoadingDiv() {
			return true;
		}
		[Browsable(false)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AppliedFilterExpression {
			get { return GetStringProperty("AppliedFilterExpression", string.Empty); }
		}
		protected void SetAppliedFilterExpression(string value) {
			SetStringProperty("AppliedFilterExpression", string.Empty, value);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseFilterExpression"),
#endif
 DefaultValue("")]
		public string FilterExpression {
			get { return Model.FilterString; }
			set {
				Model.FilterString = value;
				FilterValue = FilterTreeCloner.ToString(Model.RootNode);				
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FilterValue {
			get { return GetStringProperty("FilterValue", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(FilterValue == value) return;
				SetStringProperty("FilterValue", string.Empty, value);				
				Model.RootNode = (GroupNode)FilterTreeCloner.FromString(value, new FilterControlNodesFactory(Model));				
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseEnableCallBacks"),
#endif
		DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseEnablePopupMenuScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnablePopupMenuScrolling {
			get { return GetBoolProperty("EnablePopupMenuScrolling", false); }
			set {
				SetBoolProperty("EnablePopupMenuScrolling", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseViewMode"),
#endif
		Category("Behavior"), DefaultValue(FilterControlViewMode.Visual), AutoFormatDisable]
		public FilterControlViewMode ViewMode {
			get { return (FilterControlViewMode)GetEnumProperty("ViewMode", FilterControlViewMode.Visual); }
			set {
				if(value == ViewMode) return;
				SetEnumProperty("ViewMode", FilterControlViewMode.Visual, value);
				LayoutChanged();
			}
		}
		[
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowOperandTypeButton {
			get { return GetBoolProperty("ShowOperandTypeButton", false); }
			set {
				if(value == ShowOperandTypeButton) return;
				SetBoolProperty("ShowOperandTypeButton", false, value);
				LayoutChanged();
			}
		}
		[ Category("Settings"), AutoFormatDisable, 
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlGroupOperationsVisibility GroupOperationsVisibility { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		bool isApplyingFilter = false;
		public virtual void ApplyFilter() {
			this.isApplyingFilter = true;
			try {
				SetAppliedFilterExpression(FilterExpression);				
			} finally {
				this.isApplyingFilter = false;
			}
		}
		public void ResetFilter() {
			if(this.isApplyingFilter) return;
			FilterExpression = AppliedFilterExpression;			
		}
		public void ClearFilter() {
			if(this.isApplyingFilter) return;
			FilterExpression = string.Empty;
			ApplyFilter();
		}
		public void BindToSource(object obj, bool allowHierarchicalColumns = true, int maxHierarchyDepth = FilterControlSettings.DefaultMaxHierarchyDepth) {
			GenerateColumns(obj, allowHierarchicalColumns, maxHierarchyDepth, true, new List<IFilterColumn>());
		}
		public void BindToSource(Type type, bool allowHierarchicalColumns = true, int maxHierarchyDepth = FilterControlSettings.DefaultMaxHierarchyDepth) {
			GenerateColumns(type, allowHierarchicalColumns, maxHierarchyDepth, true, new List<IFilterColumn>());
		}
		protected internal void GenerateColumns(object type, bool allowHierarchicalColumns, int maxHierarchyDepth, bool showAllDataSourceColumns, List<IFilterColumn> externalColumns) {
			Columns.Clear();
			var columnBuilder = new FilterControlColumnBuilder();
			var columns = columnBuilder.GenerateColumns(type, allowHierarchicalColumns, maxHierarchyDepth, showAllDataSourceColumns, externalColumns).OfType<FilterControlColumn>();
			Columns.AddRange(columns.ToArray());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WebFilterControlRenderHelper RenderHelper {
			get { return renderHelper; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterControlColumnCollection Columns { get { return (FilterControlColumnCollection)Model.FilterProperties; } }
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), ASPxEditBase.EditDefaultCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), ASPxEditBase.EditSystemCssResourceName);
		}
		protected override void ClearControlFields() {
			this.rootTable = null;
			this.contentControl = null;
			this.buttonAddHotImage = null;
			this.buttonRemoveHotImage = null;
			base.ClearControlFields();
			RenderHelper.ExtraHandlers.Clear();
		}
		protected delegate string GetWhereExpressionDelegate(CriteriaOperator op);
		protected string GetFilterExpressionForDB(GetWhereExpressionDelegate getWhereFunction) {
			CriteriaOperator op = Model.CriteriaParse(FilterExpression);
			if(!Object.ReferenceEquals(op, null))
				return getWhereFunction(op);
			return string.Empty;
		}
		protected WebControl ContentControl { get { return contentControl; } }
		protected Table RootTable { get { return rootTable; } }
		protected TableCell RootCell {
			get {
				if(RootTable == null) return null;
				return RootTable.Rows[0].Cells[0];
			}
		}
		protected Image ButtonAddHotImage { get { return buttonAddHotImage; } }
		protected Image ButtonRemoveHotImage { get { return buttonRemoveHotImage; } }
		protected override bool HasContent() {
			return DesignMode || ColumnCount > 0;
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(CreateFieldNamePopup());
			Controls.Add(CreateOperationPopup());
			Controls.Add(CreateGroupPopup());
			Controls.Add(CreateAggregatePopup());
			CreateRootTable();
			Controls.Add(RootTable);
			if(ViewMode == FilterControlViewMode.VisualAndText) 
				this.contentControl = CreatePageControl();
			else  
				this.contentControl = CreateExpressionTreeContainerControl();
			RootCell.Controls.Add(ContentControl);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			if(ButtonAddHotImage != null)
				RenderHelper.AssignImageToControl(FilterControlImages.AddButtonHotName, ButtonAddHotImage);
			if(ButtonRemoveHotImage != null)
				RenderHelper.AssignImageToControl(FilterControlImages.RemoveButtonHotName, ButtonRemoveHotImage);
			if(RootTable != null) {
				GetControlStyle().AssignToControl(RootCell, true);
				RootCell.HorizontalAlign = HorizontalAlign.Left;
				RootCell.VerticalAlign = VerticalAlign.Top;
				RenderUtils.AssignAttributes(this, RootTable);
				RenderUtils.SetVisibility(RootTable, ClientVisibleInternal, true);
				RootTable.CellPadding = 0;
				RootTable.CellSpacing = 0;
			}
			if(OperationPopup != null)
				OperationPopup.PrepareImages();
			if(GroupPopup != null)
				GroupPopup.PrepareImages();
			if(AggregatePopup != null)
				AggregatePopup.PrepareImages();
			base.PrepareControlHierarchy();
		}
		protected void CreateRootTable() {
			this.rootTable = RenderUtils.CreateTable(true);
			Controls.Add(RootTable);
			RootTable.Rows.Add(RenderUtils.CreateTableRow());
			RootTable.Rows[0].Cells.Add(RenderUtils.CreateTableCell());
		}
		protected internal WebControl FieldNamePopup { get { return fieldNamePopup; } }
		FilterControlPopupMenu OperationPopup { get { return operationPopup; } }
		FilterControlPopupMenu GroupPopup { get { return groupPopup; } }
		FilterControlPopupMenu AggregatePopup { get { return aggregatePopup; } }
		protected int ColumnCount { get { return Columns.Count; }}
		protected IFilterColumn GetColumn(int index) { return Columns[index]; }
		protected internal abstract FilterControlImages GetImages();
		protected internal abstract FilterControlStyles GetStyles();
		protected internal abstract EditorImages GetImagesEditors();
		protected internal abstract EditorStyles GetStylesEditors();		
		protected virtual WebFilterOperationsBase CreateOperations(string text) {
			return new WebFilterOperations(this, text, Model);
		}
		protected virtual WebFilterControlRenderHelper CreateRenderHelper() {
			return new WebFilterControlRenderHelper(this);
		}
		protected WebControl CreateExpressionTreeContainerControl() {
			return new FilterExpressionTreeContainerControl(RenderHelper, Model);
		}
		protected FilterPageControl CreatePageControl() {
			return new FilterPageControl(this);
		}
		protected virtual ASPxWebControl CreateFieldNamePopup() {
			if(EnableColumnsTreeView)
				this.fieldNamePopup = new FilterControlPropertiesPopupTreeView(RenderHelper);
			else
				this.fieldNamePopup = new FilterControlPropertiesPopupMenu(RenderHelper);
			return this.fieldNamePopup;
		}
		protected virtual ASPxPopupMenu CreateOperationPopup() {
			this.operationPopup =  new FilterControlOperationPopupMenu(RenderHelper);
			return this.operationPopup;
		}
		protected virtual ASPxPopupMenu CreateAggregatePopup() {
			this.aggregatePopup = new FilterControlAggregatePopupMenu(RenderHelper);
			return this.aggregatePopup;
		}
		protected virtual ASPxPopupMenu CreateGroupPopup() {
			this.groupPopup =  new FilterControlGroupPopupMenu(RenderHelper);
			return this.groupPopup;
		}
		protected virtual Image CreateHotImage(string id) {
			Image image = RenderUtils.CreateImage();
			image.ID = id;
			image.Style[HtmlTextWriterStyle.Display] = "none";
			return image;
		}
		protected virtual void OnAfterFilterApply(bool isClosing) {
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientFilterControl";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxFilterControlBase), FilterControlScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.callBacksEnabled={1};\n", localVarName, EnableCallBacks ? "true" : "false");
			Hashtable userHiddenOperations = GetUserHiddenOperations();
			if(userHiddenOperations.Count > 0)
				stb.AppendFormat("{0}.userHiddenOperations={1};\n", localVarName, HtmlConvertor.ToJSON(userHiddenOperations));
			if(ViewMode == FilterControlViewMode.VisualAndText)
				stb.AppendFormat("{0}.enableTextTab = true;", localVarName);
			if(EnableColumnsTreeView) { 
				stb.AppendFormat("{0}.enableColumnsTreeView = true;", localVarName);
				stb.AppendFormat("{0}.maxHierarchyDepth = {1};", localVarName, GetMaxHierarchyDepth());
			}
			foreach(FilterControlEditorExtraHandler handler in RenderHelper.ExtraHandlers) {
				stb.AppendFormat("ASPx.GetControlCollection().Get({0}).{1}.AddHandler({2});\n", 
					HtmlConvertor.ToScript(handler.Editor.ClientID), 
					handler.EventName, handler.HandlerScript);
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(FilterExpressionStateKey, RenderHelper.FilterExpression);
			result.Add(AppliedFilterExpressionStateKey, RenderHelper.AppliedFilterExpression);
			result.Add(FilterValueStateKey, RenderHelper.FilterValue);
			result.Add(IsFilterExpressionValidStateKey, RenderHelper.IsFilterExpressionValid());
			if(ViewMode == FilterControlViewMode.VisualAndText)
				result.Add(TextTabExpressionValidStateKey, Model.IsTextTabExpressionValid());
			return result;
		}
		protected Hashtable GetUserHiddenOperations() {
			Hashtable result = new Hashtable();
			Columns.IterateFilterColumnsHierarchycally(column => {
				return CheckColumnOperationsVisible(column as IFilterColumn, result, this);
			});
			return result;
		}
		bool CheckColumnOperationsVisible(IFilterColumn column, Hashtable result, IFilterControlOwner filterOwner) {
			List<int> operations = new List<int>();
			foreach(ClauseType clause in Enum.GetValues(typeof(ClauseType))) {
				if(Model.IsValidClause(clause, column.ClauseClass)
					&& filterOwner.IsOperationHiddenByUser(column, clause)) {
					operations.Add((int)clause);
				}
			}
			if(operations.Count > 0)
				result.Add((column as IBoundProperty).GetFullNameWithLists(), operations);
			return false;
		}
		public bool IsFilterExpressionValid() {
			CriteriaOperator criteriaOperator = Model.CriteriaParse(FilterExpression);
			return CriteriaValidatorInternal.IsCriteriaOperatorValid(criteriaOperator);
		}
		public string GetFilterExpressionForAccess() {
			return GetFilterExpressionForDB(CriteriaToWhereClauseHelper.GetAccessWhere);
		}
		public string GetFilterExpressionForMsSql() {
			return GetFilterExpressionForDB(CriteriaToWhereClauseHelper.GetMsSqlWhere);
		}
		public string GetFilterExpressionForOracle() {
			return GetFilterExpressionForDB(CriteriaToWhereClauseHelper.GetOracleWhere);
		}
		public string GetFilterExpressionForDataSet() {
			return GetFilterExpressionForDB(CriteriaToWhereClauseHelper.GetDataSetWhere);
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseOperationVisibility")]
#endif
		public event FilterControlOperationVisibilityEventHandler OperationVisibility {
			add { Events.AddHandler(EventOperationVisibility, value); }
			remove { Events.RemoveHandler(EventOperationVisibility, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseParseValue")]
#endif
		public event FilterControlParseValueEventHandler ParseValue {
			add { Events.AddHandler(EventParseValue, value); }
			remove { Events.RemoveHandler(EventParseValue, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseCustomValueDisplayText")]
#endif
		public event FilterControlCustomValueDisplayTextEventHandler CustomValueDisplayText {
			add { Events.AddHandler(EventCustomValueDisplayText, value); }
			remove { Events.RemoveHandler(EventCustomValueDisplayText, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseCriteriaValueEditorInitialize")]
#endif
		public event FilterControlCriteriaValueEditorInitializeEventHandler CriteriaValueEditorInitialize {
			add { Events.AddHandler(EventCriteriaValueEditorInitialize, value); }
			remove { Events.RemoveHandler(EventCriteriaValueEditorInitialize, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseCriteriaValueEditorCreate")]
#endif
		public event FilterControlCriteriaValueEditorCreateEventHandler CriteriaValueEditorCreate {
			add { Events.AddHandler(EventCriteriaValueEditorCreate, value); }
			remove { Events.RemoveHandler(EventCriteriaValueEditorCreate, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFilterControlBaseCustomJSProperties")]
#endif
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		protected internal void RaiseOperationVisibility(FilterControlOperationVisibilityEventArgs e) {
			FilterControlOperationVisibilityEventHandler handler = (FilterControlOperationVisibilityEventHandler)Events[EventOperationVisibility];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseParseValue(FilterControlParseValueEventArgs e) {
			FilterControlParseValueEventHandler handler = (FilterControlParseValueEventHandler)Events[EventParseValue];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e) {
			FilterControlCustomValueDisplayTextEventHandler handler = (FilterControlCustomValueDisplayTextEventHandler)Events[EventCustomValueDisplayText];
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			FilterControlCriteriaValueEditorInitializeEventHandler handler = (FilterControlCriteriaValueEditorInitializeEventHandler)Events[EventCriteriaValueEditorInitialize];
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e) {
			FilterControlCriteriaValueEditorCreateEventHandler handler = (FilterControlCriteriaValueEditorCreateEventHandler)Events[EventCriteriaValueEditorCreate];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			RaisePostBackEventCore(eventArgument);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			RaisePostBackEventCore(eventArgument);
		}
		protected virtual void RaisePostBackEventCore(string arg) {
			new WebFilterControlOperations(this).Perform(arg);
			ResetControlHierarchy();
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			if(ClientObjectState != null) {
				string filterExpression = GetClientObjectStateValueString(FilterExpressionStateKey);
				if(filterExpression != null && filterExpression != FilterExpression)
					FilterExpression = filterExpression;
				string appliedFilterExpression = GetClientObjectStateValueString(AppliedFilterExpressionStateKey);
				if(appliedFilterExpression != null && appliedFilterExpression != AppliedFilterExpression)
					SetAppliedFilterExpression(appliedFilterExpression);
				string filterValue = GetClientObjectStateValueString(FilterValueStateKey);
				if(!string.IsNullOrEmpty(filterValue))
					FilterValue = filterValue;
			}
			return base.LoadPostData(postCollection);
		}
		protected override object GetCallbackResult() {
			Hashtable res = new Hashtable();
			EnsureChildControls();
			BeginRendering();
			try {
				res[CallbackResultProperties.Html] = RenderUtils.GetRenderResult(ContentControl);
				res[CallbackResultProperties.StateObject] = GetClientObjectState();
			}
			finally {
				EndRendering();
			}
			return res;
		}
		protected abstract bool TryGetSpecialValueDisplayTextImpl(IFilterColumn column, object value, bool encodeValue, out string displayText);
		protected virtual bool TryConvertValueImpl(IFilterablePropertyInfo propertyInfo, string text, out object value) {
			FilterControlParseValueEventArgs e = new FilterControlParseValueEventArgs(propertyInfo, text);
			RaiseParseValue(e);
			value = e.Handled ? e.Value : null;
			return e.Handled;
		}
		protected virtual bool IsOperationHiddenByUserImpl(IFilterablePropertyInfo propertyInfo, ClauseType operation) {
			FilterControlOperationVisibilityEventArgs e = new FilterControlOperationVisibilityEventArgs(propertyInfo, operation);
			RaiseOperationVisibility(e);
			return !e.Visible;
		}
		protected virtual void RaiseCustomValueDisplayTextImpl(FilterControlCustomValueDisplayTextEventArgs e) {
			RaiseCustomValueDisplayText(e);
		}
		protected virtual void RaiseCriteriaValueEditorInitializeImpl(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			RaiseCriteriaValueEditorInitialize(e);
		}
		protected virtual void RaiseCriteriaValueEditorCreateImpl(FilterControlCriteriaValueEditorCreateEventArgs e) {
			RaiseCriteriaValueEditorCreate(e);
		}
		protected internal virtual bool SuppressEditorValidation {
			get { return false; }
		}
		protected internal bool EnableColumnsTreeView { 
			get {  return Columns.OfType<FilterControlComplexTypeColumn>().Where(c => c.Columns.Count > 0).Any(); } 
		}
		int GetMaxHierarchyDepth() {
			int maxDepth = 0;
			Columns.IterateFilterColumnsHierarchycally(col => {
				var colDepth = col.GetLevel();
				if(colDepth > maxDepth)
					maxDepth = colDepth;
				return false;
			});
			return maxDepth;
		}
		#region IFilterControlOwner Members
		bool IFilterControlOwner.IsRightToLeft { get { return IsRightToLeft(); } }
		bool IFilterControlOwner.TryGetSpecialValueDisplayText(IFilterColumn column, object value, bool encodeValue, out string displayText) {
			return TryGetSpecialValueDisplayTextImpl(column, value, encodeValue, out displayText);
		}
		bool IFilterControlOwner.IsOperationHiddenByUser(IFilterablePropertyInfo propertyInfo, ClauseType operation) {
			return IsOperationHiddenByUserImpl(propertyInfo, operation);
		}
		FilterControlViewMode IFilterControlOwner.ViewMode { get { return ViewMode; } }
		bool IFilterControlOwner.ShowOperandTypeButton { get { return ShowOperandTypeButton; } }
		FilterControlGroupOperationsVisibility IFilterControlOwner.GroupOperationsVisibility { get { return GroupOperationsVisibility; } }
		FilterControlColumnCollection IFilterControlOwner.GetFilterColumns() { return Columns; }
		bool IFilterControlOwner.TryConvertValue(IFilterablePropertyInfo propertyInfo, string text, out object value) {
			return TryConvertValueImpl(propertyInfo, text, out value);
		}
		void IFilterControlOwner.RaiseCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e) { 
			RaiseCustomValueDisplayTextImpl(e);
		}
		void IFilterControlOwner.RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			RaiseCriteriaValueEditorInitializeImpl(e);
		}
		void IFilterControlOwner.RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e) {
			RaiseCriteriaValueEditorCreateImpl(e);
		}
		#endregion
		#region IFilterControlOperationsOwner Members
		WebFilterOperationsBase IFilterControlOperationsOwner.CreateOperations(string filterValue) {
			return CreateOperations(filterValue);
		}
		void IFilterControlOperationsOwner.OnAfterFilterApply(bool isClosing) {
			OnAfterFilterApply(isClosing);
		}
		#endregion
	}
	[DXWebToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxFilterControl.bmp"),
	Designer("DevExpress.Web.Design.ASPxFilterControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public class ASPxFilterControl : ASPxFilterControlBase, IControlDesigner {
		EditorImages imagesEditors;
		EditorStyles stylesEditors;
		public ASPxFilterControl() {
			this.imagesEditors = new EditorImages(this);
			this.stylesEditors = new EditorStyles(this);			
		}
		string rememberedExpression = null;
		internal void RememberCurrentExpression() {
			rememberedExpression = FilterExpression;
		}
		internal void RevertToLastRememberedExpression() {
			if (rememberedExpression != null)
				FilterExpression = rememberedExpression;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlColumns"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable,
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public new FilterControlColumnCollection Columns { get { return base.Columns; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel { get { return base.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorImages ImagesEditors { get { return imagesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImages Images { get { return (FilterControlImages)ImagesInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlStyles Styles { get { return StylesInternal as FilterControlStyles; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public FilterControlClientSideEvents ClientSideEvents {
			get { return (FilterControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlClientInstanceName"),
#endif
		Category("Client-Side"), AutoFormatDisable, DefaultValue(""), Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlClientVisible"),
#endif
		Category("Client-Side"), AutoFormatDisable, DefaultValue(true), Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new FilterControlClientSideEvents();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFilterControlJSProperties"),
#endif
		AutoFormatDisable, Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		protected internal override FilterControlImages GetImages() {  return Images; }
		protected internal override FilterControlStyles GetStyles() { return Styles; }
		protected internal override EditorImages GetImagesEditors() { return ImagesEditors; }
		protected internal override EditorStyles GetStylesEditors() { return StylesEditors; }
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.standAlone=true;\n", localVarName);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			list.Add(StylesEditors);
			list.Add(ImagesEditors);
			list.Add(GroupOperationsVisibility);
			return list.ToArray();
		}
		protected internal void OnColumnCollectionChanged() {
			LayoutChanged();
		}
		protected override bool TryGetSpecialValueDisplayTextImpl(IFilterColumn column, object value, bool encodeValue, out string displayText) {
			displayText = string.Empty;
			return false;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.FilterControlColumnsOwner"; } }
	}
}
