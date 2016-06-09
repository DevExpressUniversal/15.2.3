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
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using DevExpress.Utils.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Design;
	using System.Drawing.Design;
	public enum GridLookupSelectionMode { Single, Multiple }
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.Design.ASPxLookupDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DXClientDocumentationProviderWeb("ASPxGridView"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGridLookup.bmp")
	]
	public class ASPxGridLookup : ASPxDropDownEditBase, IControlDesigner {
		protected internal const string GridLookupScriptResourceName = ASPxGridView.GridScriptResourcePath + "GridLookup.js";
		protected const string GridLookupKeyboardSupportHelperName = "GridLookupKeyboardSupportHelper";
		ASPxGridView gridView;
		GridViewProperties gridViewProperties;
		bool isInDataBind = false;
		protected bool IsInDataBind { get { return isInDataBind; } }
		protected void SetInDataBind(bool value) {
			this.isInDataBind = value;
			((GridViewWrapper)GridView).SetGLPInDataBind(value);
		}
		public ASPxGridLookup() {
			EnsureGridView();
		}
		#region GridView Api
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewProperties"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewProperties GridViewProperties {
			get {
				EnsureGridView();
				if(this.gridViewProperties == null)
					this.gridViewProperties = new GridViewProperties(this.gridView);
				return this.gridViewProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public GridViewClientSideEvents GridViewClientSideEvents {
			get { return GridView.ClientSideEvents; }
		}
		#region Data
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupKeyFieldName"),
#endif
		DefaultValue(""), Category("Data"), Localizable(false),
		TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable]
		public string KeyFieldName {
			get { return GridView.KeyFieldName; }
			set { GridView.KeyFieldName = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupColumns"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable,
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public GridViewColumnCollection Columns { get { return GridView.Columns; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupDataSource"),
#endif
		EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable]
		public override object DataSource {
			get { return GridView.DataSource; }
			set { GridView.DataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupDataSourceID"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Data"),
		AutoFormatDisable, Themeable(false)]
		public override string DataSourceID {
			get { return GridView.DataSourceID; }
			set { GridView.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupAutoGenerateColumns"),
#endif
		DefaultValue(true), Category("Data"), AutoFormatDisable]
		public bool AutoGenerateColumns {
			get { return GridView.AutoGenerateColumns; }
			set { GridView.AutoGenerateColumns = value; }
		}
		#endregion
		#region Images
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewImages GridViewImages { get { return GridView.Images; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditorImages GridViewImagesEditors { get { return GridView.ImagesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewImagesFilterControl"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImages GridViewImagesFilterControl { get { return GridView.ImagesFilterControl; } }
		#endregion
		#region Styles
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupDropDownWindowStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DropDownWindowStyle DropDownWindowStyle {
			get { return Properties.DropDownWindowStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStyles GridViewStyles { get { return GridView.Styles; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewStylesPager"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewPagerStyles GridViewStylesPager { get { return GridView.StylesPager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditorStyles GridViewStylesEditors { get { return GridView.StylesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewStylesFilterControl"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlStyles GridViewStylesFilterControl { get { return GridView.StylesFilterControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupGridViewStylesPopup"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewPopupControlStyles GridViewStylesPopup { get { return GridView.StylesPopup; } }
		#endregion
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Client-Side"),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new GridLookupClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupIncrementalFilteringDelay"),
#endif
		Category("Behavior"), DefaultValue(GridLookupProperties.DefaultIncrementalFilteringDelay), AutoFormatDisable]
		public int IncrementalFilteringDelay {
			get { return Properties.IncrementalFilteringDelay; }
			set { Properties.IncrementalFilteringDelay = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupIncrementalFilteringMode"),
#endif
		Category("Behavior"), DefaultValue(IncrementalFilteringMode.Contains), AutoFormatDisable]
		public IncrementalFilteringMode IncrementalFilteringMode {
			get { return Properties.IncrementalFilteringMode; }
			set { Properties.IncrementalFilteringMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupTextFormatString"),
#endif
		Category("Data"), DefaultValue(""), Localizable(true), AutoFormatEnable,
		Editor("DevExpress.Web.Design.LookupTextFormatStringUIEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TextFormatString {
			get { return Properties.TextFormatString; }
			set { Properties.TextFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupMultiTextSeparator"),
#endif
		Category("Data"), DefaultValue(GridLookupProperties.MultiTextSeparatorDefault),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public string MultiTextSeparator {
			get { return Properties.MultiTextSeparator; }
			set { Properties.MultiTextSeparator = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupSelectionMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Category("Behavior"),
		DefaultValue(GridLookupSelectionMode.Single)]
		public GridLookupSelectionMode SelectionMode {
			get { return Properties.SelectionMode; }
			set { Properties.SelectionMode = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridLookupValue")]
#endif
		public override object Value {
			get {
				return SelectionStrategy.Value;
			}
			set {
				SelectionStrategy.Value = value;
				ValueCore = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupText"),
#endif
		DefaultValue(""), AutoFormatDisable, Themeable(false), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public override string Text {
			get {
				return SelectionStrategy.Text;
			}
			set {
				SelectionStrategy.Text = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridLookupNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText
		{
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		protected internal string ClientText {
			get { return SelectionStrategy.ClientText; }
			set { SelectionStrategy.ClientText = value; }
		}
		protected object ValueCore {
			get { return base.Value; }
			set { base.Value = value; }
		}
		protected internal new GridLookupProperties Properties {
			get { return (GridLookupProperties)base.Properties; }
		}
		[Browsable(false)]
		public ASPxGridView GridView {
			get {
				EnsureGridView();
				return gridView;
			}
		}
		protected SelectionStrategyBase SelectionStrategy {
			get { return Properties.SelectionStrategy; }
		}
		protected internal bool MultiSelect {
			get { return Properties.MultiSelect; }
		}
		protected internal PreventDoubleBindHelper PreventDoubleBindHelper {
			get {
				EnsureGridView(); 
				return ((GridViewWrapper)GridView).PreventDoubleBindHelper; 
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ForceCreateControlHierarchy();
		}
		protected override EditPropertiesBase CreateProperties() {
			return new GridLookupProperties(this);
		}
		protected virtual ASPxGridView CreateGridView() {
			return new GridViewWrapper(Properties);
		}
		protected void EnsureGridView() {
			if(this.gridView != null)
				return;
			this.gridView = CreateGridView();
			PrepareGridView();
		}
		protected internal void PrepareGridView() {
			this.gridView.ID = "gv";
			this.gridView.KeyboardSupport = true;
			this.gridView.ParentSkinOwner = this;
			this.gridView.ParentImages = RenderImages;
			this.gridView.ParentStyles = RenderStyles;
			this.gridView.SettingsBehavior.AllowFocusedRow = true;
			this.gridView.FocusedRowIndex = -1;
			this.gridView.Border.BorderWidth = 0;
			this.gridView.CustomCallback += GridViewCustomCallback;
			this.gridView.AfterPerformCallback += GridViewAfterPerformCallback;
			this.gridView.DataBinding += GridViewDataBinding;
			this.gridView.DataBound += GridViewDataBound;
			SetGridViewSelectionMode(GridLookupSelectionMode.Single);
			AssingNewExpressionCreatorToGridView(this.gridView);
		}
		protected internal void AssingNewExpressionCreatorToGridView(ASPxGridView gridView) {
			FilterExpressionCreatorBase filterExpressionCreator = null;
			if(IncrementalFilteringMode == IncrementalFilteringMode.StartsWith)
				filterExpressionCreator = new StartWithFilterExpressionCreator();
			else if(IncrementalFilteringMode == IncrementalFilteringMode.Contains)
				filterExpressionCreator = new ContainsFilterExpressionCreator();
			else
				filterExpressionCreator = new DisabledFilterExpressionCreator();
			((IGridViewOwnedLookup)gridView).FilterExpressionCreator = filterExpressionCreator;
		}
		protected internal void SetGridViewSelectionMode(GridLookupSelectionMode selectionMode) {
			this.gridView.SettingsBehavior.AllowSelectSingleRowOnly = selectionMode == GridLookupSelectionMode.Single;
			this.gridView.SettingsBehavior.AllowSelectByRowClick = selectionMode == GridLookupSelectionMode.Multiple;
		}
		protected override bool ClearHeirarchyBefore_AddDropDownControl() {
			return true;
		}
		ControlCollection GridParentControls;
		protected internal void SetGridViewContainer(System.Web.UI.ControlCollection controls) {
			GridParentControls = controls;
			AddGridToHierarchy();
		}
		protected internal void AddGridToHierarchy() {
			if(!AddGridToHierarchyLocked() && Enabled)
				GridParentControls.Add(GetGridViewForRender());
		}
		int AddGridToHierarchyLockCount = 0;
		protected void SetLockAddGridToHierarchy(bool isLock) {
			AddGridToHierarchyLockCount += isLock ? 1 : -1;
		}
		protected internal bool AddGridToHierarchyLocked() {
			return AddGridToHierarchyLockCount > 0;
		}
		protected internal virtual void ForceCreateControlHierarchy() {
			SetLockAddGridToHierarchy(true);
			EnsureChildControlsRecursive(this);
			SetLockAddGridToHierarchy(false);
			AddGridToHierarchy();
			LayoutChanged();
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new GridLookupControl(this);
		}
		protected internal ASPxGridView GetGridViewForRender() {
			GridView.EnableCallBacks = !AutoPostBack;
			GridView.RightToLeft = RightToLeft;
			GridView.RenderHelper.CustomKbdHelperName = GridLookupKeyboardSupportHelperName;
			return GridView;
		}
		protected override bool NeedPopupControlInDesingMode {
			get { return true; }
		}
		protected internal override bool IsFormatterScriptRequired() {
			return true;
		}
		protected internal override string GetInputText() {
			string inputText = SelectionStrategy.Text;
			bool hasntSelectedItems = SelectionMode == GridLookupSelectionMode.Multiple ? GridView.Selection.Count == 0 : GridView.FocusedRowIndex == -1;
			return NullText != "" && hasntSelectedItems ? NullText : inputText;
		}
		protected List<string> GetFieldNames() {
			return GridView.GetFieldNames();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			ClientText = postCollection[UniqueID];
			if(GridView.EnableRowsCache && GridView.AutoGenerateColumns) 
				GridView.DataBind();
			return false;
		}
		protected override void DataBindChildren() {
			GridView.DataSource = DataSource;
			GridView.DataSourceID = DataSourceID;
			GridView.KeyFieldName = KeyFieldName;
			if(!Columns.IsEmpty)
				GridView.Columns.Assign(Columns);
			base.DataBindChildren();
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridScriptResourceName);
			RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridViewScriptResourceName);
			RegisterIncludeScript(typeof(ASPxGridLookup), GridLookupScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientGridLookup";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.textFormatString='{1}';\n", localVarName, Properties.GetTextFormatString());
			stb.AppendFormat("{0}.keyFieldName='{1}';\n", localVarName, KeyFieldName);
			IncrementalFilteringMode incrementalFilteringMode = GetIncrementalFilteringMode();
			if(incrementalFilteringMode != GridLookupProperties.DefaultIncrementalFilteringMode) {
				stb.AppendFormat("{0}.incrementalFilteringMode = \"{1}\";\n", localVarName, incrementalFilteringMode);
			}
			if(IncrementalFilteringDelay != GridLookupProperties.DefaultIncrementalFilteringDelay)
				stb.AppendFormat("{0}.filterTimer = {1};\n", localVarName, IncrementalFilteringDelay);
			if(MultiTextSeparator != GridLookupProperties.MultiTextSeparatorDefault)
				stb.AppendFormat("{0}.multiTextSeparator = '{1}';\n", localVarName, MultiTextSeparator);
			if(GridView.AccessibilityCompliant)
				stb.AppendFormat("{0}.isAccessibilityComplianceEnabled = true;\n", localVarName);
		}
		protected IncrementalFilteringMode GetIncrementalFilteringMode() {
			if(MultiSelect)
				return IncrementalFilteringMode.None;
			return IncrementalFilteringMode;
		}
		protected void OnGridViewCancelChanges(string[] keyFieldValues) {
			SelectionStrategy.CancelChanges(keyFieldValues);
		}
		protected void OnGridViewFilter(string filter) {
			SelectionStrategy.Filter(filter);
		}
		protected void OnGridViewApplyChanges(string appliedRowValue, bool filterExpressionChangingAllowed) {
			OnBeforeGridViewApply(filterExpressionChangingAllowed);
			SelectionStrategy.ApplyGridViewChanges(appliedRowValue);
			OnAferGridViewApply();
		}
		protected void OnGridViewApplyInputChanges() {
			OnBeforeGridViewApply();
			SelectionStrategy.ApplyInputChanges();
			OnAferGridViewApply();
		}
		protected void OnBeforeGridViewApply() {
			OnBeforeGridViewApply(true);
		}
		protected void OnBeforeGridViewApply(bool filterExpressionChangingAllowed) {
			if(filterExpressionChangingAllowed)
				GridView.FilterExpression = string.Empty;
		}
		protected void OnAferGridViewApply() {
			this.RaiseValueChanged();
		}
		protected void GridViewCustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e) {
			GridLookupCallbackArgumentsReader argument = new GridLookupCallbackArgumentsReader(e.Parameters);
			if(argument.IsFilteringCallback)
				OnGridViewFilter(argument.Filter);
			else if(argument.IsCancelChangesCallback)
				OnGridViewCancelChanges(argument.CancelChangesBySelectingRowsKeys);
			else if(argument.IsApplyChangesCallback)
				OnGridViewApplyChanges(argument.AppliedRowKey, true);
			else if(argument.IsApplyInputChangesCallback)
				OnGridViewApplyInputChanges();
		}
		protected void GridViewAfterPerformCallback(object sender, ASPxGridViewAfterPerformCallbackEventArgs e) {
			if(e.CallbackName == "SELECTROWSKEY" || e.CallbackName == "SELECTROWS")
				OnGridViewApplyChanges(null, false);
		}
		protected void GridViewDataBinding(object sender, EventArgs e) {
			if(!IsInDataBind) {
				try{ 
					RaiseDataBinding();
				} catch(InvalidOperationException){
				}
			}
		}
		protected void GridViewDataBound(object sender, EventArgs e) {
			if(!IsInDataBind)
				RaiseDataBound();
		}
		public override void DataBind() {
			SetInDataBind(true);
			try {
				base.DataBind();
				bool gvWasDataBinded = PreventDoubleBindHelper.CanGridViewDataBind;
				bool gvDataBindSuppressed = !gvWasDataBinded;
				if(gvDataBindSuppressed && Request != null)
					((IPostBackDataHandler)GridView).LoadPostData(string.Empty, Request.Params);
			} finally {
				SetInDataBind(false);
			}
		}
		protected internal AppearanceStyleBase GetDropDownWindowStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Properties.GetDefaultDropDownWindowStyle());
			style.CopyFrom(RenderStyles.DropDownWindow);
			return style;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.GridLookupCommonFormDesigner"; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridViewProperties {
		private ASPxGridView gridView = null;
		public GridViewProperties(ASPxGridView owner) {
			this.gridView = owner;
		}
		private ASPxGridView GridView { get { return gridView; } }
		#region Behavior
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnableRowsCache"),
#endif
		DefaultValue(true), AutoFormatDisable, Category("Behavior"), NotifyParentProperty(true)]
		public bool EnableRowsCache {
			get { return GridView.EnableRowsCache; }
			set { GridView.EnableRowsCache = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool EnableCallbackCompression {
			get { return GridView.EnableCallbackCompression; }
			set { GridView.EnableCallbackCompression = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnableCallBacks"),
#endif
		DefaultValue(true), AutoFormatDisable, Category("Behavior"), NotifyParentProperty(true)]
		public bool EnableCallBacks {
			get { return GridView.EnableCallBacks; }
			set { GridView.EnableCallBacks = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return GridView.EnableCallbackAnimation; }
			set { GridView.EnableCallbackAnimation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnablePagingCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnablePagingCallbackAnimation {
			get { return GridView.EnablePagingCallbackAnimation; }
			set { GridView.EnablePagingCallbackAnimation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesEnablePagingGestures"),
#endif
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnablePagingGestures {
			get { return GridView.EnablePagingGestures; }
			set { GridView.EnablePagingGestures = value; }
		}
		#endregion
		#region Templates
		[Browsable(false), AutoFormatEnable, Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewTemplates Templates { get { return GridView.Templates; } }
		#endregion
		#region Accessibility
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool AccessibilityCompliant {
			get { return GridView.AccessibilityCompliant; }
			set { GridView.AccessibilityCompliant = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesCaption"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable, NotifyParentProperty(true)]
		public string Caption {
			get { return GridView.Caption; }
			set { GridView.Caption = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSummaryText"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SummaryText {
			get { return GridView.SummaryText; }
			set { GridView.SummaryText = value; }
		}
		#endregion
		#region ClientSide
		[
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return GridView.JSProperties; }
		}
		#endregion
		#region Data
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesPreviewFieldName"),
#endif
		DefaultValue(""), Category("Data"), Localizable(false),
		TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable, NotifyParentProperty(true)]
		public string PreviewFieldName {
			get { return GridView.PreviewFieldName; }
			set { GridView.PreviewFieldName = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesTotalSummary"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public ASPxSummaryItemCollection TotalSummary { get { return GridView.TotalSummary; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesGroupSummary"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public ASPxSummaryItemCollection GroupSummary { get { return GridView.GroupSummary; } }
		#endregion
		#region Settings
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsBehavior"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewBehaviorSettings SettingsBehavior { get { return GridView.SettingsBehavior; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsPager"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewPagerSettings SettingsPager { get { return GridView.SettingsPager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsEditing"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewEditingSettings SettingsEditing { get { return GridView.SettingsEditing; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettings"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewSettings Settings { get { return GridView.Settings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsText"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewTextSettings SettingsText { get { return GridView.SettingsText; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsCustomizationWindow"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow { get { return GridView.SettingsCustomizationWindowInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return GridView.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsCookies"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewCookiesSettings SettingsCookies { get { return GridView.SettingsCookies; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsDetail"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewDetailSettings SettingsDetail { get { return GridView.SettingsDetail; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsPopup"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewPopupControlSettings SettingsPopup { get { return GridView.SettingsPopup; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsCommandButton"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewCommandButtonSettings SettingsCommandButton { get { return GridView.SettingsCommandButton; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsDataSecurity"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewDataSecuritySettings SettingsDataSecurity { get { return GridView.SettingsDataSecurity; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesSettingsSearchPanel"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ASPxGridViewSearchPanelSettings SettingsSearchPanel { get { return GridView.SettingsSearchPanel; } }
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPropertiesDataSourceForceStandardPaging"),
#endif
		DefaultValue(false), AutoFormatDisable]
		public bool DataSourceForceStandardPaging {
			get { return GridView.DataSourceForceStandardPaging; }
			set { GridView.DataSourceForceStandardPaging = value; }
		}
		public override string ToString() {
			return CommonUtils.GetObjectText(this);
		}
		public virtual void Assign(GridViewProperties source) {
			if(source == null)
				return;
			EnableRowsCache = source.EnableRowsCache;
			EnableCallbackCompression = source.EnableCallbackCompression;
			EnableCallBacks = source.EnableCallBacks;
			EnableCallbackAnimation = source.EnableCallbackAnimation;
			EnablePagingCallbackAnimation = source.EnablePagingCallbackAnimation;
			EnablePagingGestures = source.EnablePagingGestures;
			AccessibilityCompliant = source.AccessibilityCompliant;
			Caption = source.Caption;
			SummaryText = source.SummaryText;
			PreviewFieldName = source.PreviewFieldName;
			TotalSummary.Assign(source.TotalSummary);
			GroupSummary.Assign(source.GroupSummary);
			SettingsBehavior.Assign(source.SettingsBehavior);
			SettingsPager.Assign(source.SettingsPager);
			SettingsEditing.Assign(source.SettingsEditing);
			Settings.Assign(source.Settings);
			SettingsText.Assign(source.SettingsText);
			SettingsCustomizationWindow.Assign(source.SettingsCustomizationWindow);
			SettingsLoadingPanel.Assign(source.SettingsLoadingPanel);
			SettingsCookies.Assign(source.SettingsCookies);
			SettingsDetail.Assign(source.SettingsDetail);
			SettingsPopup.Assign(source.SettingsPopup);
			SettingsCommandButton.Assign(source.SettingsCommandButton);
			SettingsDataSecurity.Assign(source.SettingsDataSecurity);
			SettingsSearchPanel.Assign(source.SettingsSearchPanel);
			DataSourceForceStandardPaging = source.DataSourceForceStandardPaging;
		}
	}
	public class GridLookupProperties: DropDownEditPropertiesBase {
		internal const int DefaultIncrementalFilteringDelay = 500;
		internal const IncrementalFilteringMode DefaultIncrementalFilteringMode = IncrementalFilteringMode.Contains;
		internal const string MultiTextSeparatorDefault = ";";
		SelectionStrategyOwner selectionStrategyOwner;
		public GridLookupProperties(IPropertiesOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesDropDownWindowStyle")]
#endif
		public DropDownWindowStyle DropDownWindowStyle {
			get { return DropDownWindowStyleInternal; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesIncrementalFilteringDelay")]
#endif
		public int IncrementalFilteringDelay {
			get { return GetIntProperty("IncrementalFilteringDelay", DefaultIncrementalFilteringDelay); }
			set { SetIntProperty("IncrementalFilteringDelay", DefaultIncrementalFilteringDelay, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesIncrementalFilteringMode")]
#endif
		public IncrementalFilteringMode IncrementalFilteringMode {
			get { return (IncrementalFilteringMode)GetEnumProperty("IncrementalFilteringMode", IncrementalFilteringMode.Contains); }
			set {
				SetEnumProperty("IncrementalFilteringMode", IncrementalFilteringMode.Contains, value);
				OnIncrementalFilteringModeChanged();
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesTextFormatString")]
#endif
		public string TextFormatString {
			get { return GetStringProperty("TextFormatString", ""); }
			set { SetStringProperty("TextFormatString", "", value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesNullText")]
#endif
		public string NullText {
			get { return base.NullTextInternal; }
			set { base.NullTextInternal = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesSelectionMode")]
#endif
		public GridLookupSelectionMode SelectionMode {
			get { return GetSelectionMode(); }
			set { SetSelectionMode(value); }
		}
		protected virtual GridLookupSelectionMode GetSelectionMode() {
			return GridView.SettingsBehavior.AllowSelectSingleRowOnly ?
				GridLookupSelectionMode.Single : GridLookupSelectionMode.Multiple;
		}
		protected virtual void SetSelectionMode(GridLookupSelectionMode selectionMode) {
			GridLookup.SetGridViewSelectionMode(selectionMode);
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesClientSideEvents")]
#endif
		public new GridLookupClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as GridLookupClientSideEvents; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridLookupPropertiesMultiTextSeparator")]
#endif
		public string MultiTextSeparator {
			get { return GetStringProperty("MultiTextSeparator", MultiTextSeparatorDefault); }
			set { SetStringProperty("MultiTextSeparator", MultiTextSeparatorDefault, value); }
		}
		protected internal ASPxGridLookup GridLookup { get { return (ASPxGridLookup)Owner; } }
		protected internal ASPxGridView GridView { get { return GridLookup != null ? GridLookup.GridView : null; } }
		protected internal SelectionStrategyBase SelectionStrategy {
			get {
				if(selectionStrategyOwner == null)
					selectionStrategyOwner = CreateSelectionStrategyOwner();
				return selectionStrategyOwner.SelectionStrategy;
			}
		}
		protected internal bool MultiSelect {
			get { return SelectionMode == GridLookupSelectionMode.Multiple; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxGridLookup();
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new GridLookupClientSideEvents(this);
		}
		protected virtual SelectionStrategyOwner CreateSelectionStrategyOwner() {
			return new SelectionStrategyOwner(this);
		}
		protected void OnIncrementalFilteringModeChanged() {
			if(GridLookup != null)
				GridLookup.AssingNewExpressionCreatorToGridView(GridView);
		}
		protected internal string GetTextFormatString() {
			if(!string.IsNullOrEmpty(TextFormatString))
				return TextFormatString;
			return DefaultTextFormatString;
		}
		protected internal string DefaultTextFormatString {
			get { return CommonUtils.GetDefaultTextFormatString(GridView.DataColumns.Count, IsRightToLeft()); }
		}
		protected internal AppearanceStyleBase GetDefaultDropDownWindowStyle() {
			return base.GetDefaultDropDownWindowStyleInternal();
		}
		protected internal string GetSelectionStateJSON() {
			return SelectionStrategy.GetSelectionStateJSON();
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var properties = source as GridLookupProperties;
			if(properties != null) {
				IncrementalFilteringDelay = properties.IncrementalFilteringDelay;
				IncrementalFilteringMode = properties.IncrementalFilteringMode;
				TextFormatString = properties.TextFormatString;
				MultiTextSeparator = properties.MultiTextSeparator;
				NullText = properties.NullText;
				SelectionMode = properties.SelectionMode;
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	using DevExpress.Data.Filtering;
	using DevExpress.Data.Summary;
	using DevExpress.Web;
	using DevExpress.Web.Export;
	[ToolboxItem(false)]
	public class GridViewWrapper : ASPxGridView, IGridViewOwnedLookup {
		GridLookupProperties glpProperties;
		string savedSelectionState = string.Empty;
		bool allowFireFocusedOrSelectedRowChangedOnClient = false;
		public GridViewWrapper(GridLookupProperties glpProperties)
			: base() {
			this.glpProperties = glpProperties;
			ClientIDHelper.EnableClientIDGeneration(this);
		}
		protected internal ASPxGridLookup GridLookup { get { return ((IGridViewOwnedLookup)this).GLPProperties.GridLookup; } }
		protected override bool AllowFireFocusedOrSelectedRowChangedOnClient { get { return false; } }
		protected override void CreateControlHierarchy() {
			this.CheckUsingEndlessPaging();
			if(RenderHelper.UseEndlessPaging)
				throw new Exception("The ASPxGridLookup doesn't support the EndlessPaging functionality. For details, see http://documentation.devexpress.com/#AspNet/CustomDocument15467.");
			base.CreateControlHierarchy();
		}
		protected override void BeforeRender() {
			((IGridViewOwnedLookup)this).SaveSelectionState();
			base.BeforeRender();
		}
		protected override object GetCallbackResult() {
			((IGridViewOwnedLookup)this).SaveSelectionState();
			return base.GetCallbackResult();
		}
		protected override GridCallbackArgumentsReader GetCreateCallbackArgumentReader(string eventArgument) {
			var callbackArgumentReader = base.GetCreateCallbackArgumentReader(eventArgument);
			this.SaveCallbackCommand(callbackArgumentReader.CallbackArguments);
			return callbackArgumentReader;
		}
		protected override void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			this.InitializeAdditionalClientObjectScript(stb, localVarName, clientID);
		}
		protected override void EnsurePreRender() {
			this.SettingsBehavior.AllowSelectByRowClick = !SettingsBehavior.AllowSelectSingleRowOnly; 
			base.EnsurePreRender();
		}
		protected override void CBSelectRows(string[] args) {
			bool changedBySelectAllCheckBox = false;
			if(args.Length > 1 && bool.TryParse(args[1], out changedBySelectAllCheckBox))
				this.allowFireFocusedOrSelectedRowChangedOnClient = changedBySelectAllCheckBox;
			base.CBSelectRows(args);
		}
		protected override void LoadFocusedRowIndex() {
			string keyString = GetClientObjectStateValueString(GridClientStateProperties.FocusedKeyState);
			if(!string.IsNullOrEmpty(keyString)) {
				var keyObject = DataProxy.GetKeyValueFromScript(keyString);
				int visibleIndex = DataProxy.FindVisibleIndexByKey(keyObject, false, false);
				if(visibleIndex >= 0) {
					DataProxy.FocusedRowVisibleIndex = visibleIndex;
					return;
				}
			}
			base.LoadFocusedRowIndex();
		}
		#region IGridViewLookupOwner Members
		FilterExpressionCreatorBase IGridViewOwnedLookup.FilterExpressionCreator { get; set; }
		GridLookupProperties IGridViewOwnedLookup.GLPProperties { get { return glpProperties; } }
		string IGridViewOwnedLookup.CallbackCommand { get; set; }
		string[] IGridViewOwnedLookup.CallbackCommandArgs { get; set; }
		string IGridViewOwnedLookup.SavedSelectionState { get; set; }
		#endregion
		PreventDoubleBindHelper preventDoubleBindHelper = new PreventDoubleBindHelper();
		protected internal PreventDoubleBindHelper PreventDoubleBindHelper {
			get { return preventDoubleBindHelper; }
		}
		protected internal void SetGLPInDataBind(bool value) {
			PreventDoubleBindHelper.SetGLPInDataBind(value);
		}
		public override void DataBind() {
			if(PreventDoubleBindHelper.CanGridViewDataBind) {
				base.DataBind();
			}
		}
		protected override void OnDataBound(EventArgs e) {
			base.OnDataBound(e);
			PreventDoubleBindHelper.OnGridViewDataBound();
		}
	}
	public class PreventDoubleBindHelper {
		bool glpInDataBind = false;
		int gridViewBoundCount = 0;
		public bool CanGridViewDataBind {
			get {
				bool doubleDataBind = glpInDataBind && gridViewBoundCount > 0;
				return !doubleDataBind;
			}
		}
		public void SetGLPInDataBind(bool value) {
			this.glpInDataBind = value;
			this.gridViewBoundCount = 0;
		}
		public void OnGridViewDataBound() {
			gridViewBoundCount++;
		}
	}
	public static class GridViewWrapperHelper {
		public static void SaveSelectionState(this IGridViewOwnedLookup grid) {
			grid.SavedSelectionState = grid.GLPProperties.GetSelectionStateJSON();
		}
		public static void CheckUsingEndlessPaging(this ASPxGridView grid) {
			if(grid.RenderHelper.UseEndlessPaging)
				throw new Exception("The ASPxGridLookup doesn't support the EndlessPaging functionality. For details, see http://documentation.devexpress.com/#AspNet/CustomDocument15467.");
		}
		public static void SaveCallbackCommand(this ASPxGridView grid, string gridViewCallbackCommand) {
			string[] args = grid.GetCallBackPostBackArgs(ref gridViewCallbackCommand);
			IGridViewOwnedLookup gridOwnedLookup = (IGridViewOwnedLookup)grid;
			gridOwnedLookup.CallbackCommand = gridViewCallbackCommand;
			gridOwnedLookup.CallbackCommandArgs = args;
			if(gridViewCallbackCommand == "CUSTOMCALLBACK") {
				GridLookupCallbackArgumentsReader lookupArgumentReader = new GridLookupCallbackArgumentsReader(args[0]);
				if(lookupArgumentReader.IsCancelChangesCallback)
					gridOwnedLookup.CallbackCommand = GridLookupCallbackArgumentsReader.CancelChangesCallbackPrefix;
				else if(lookupArgumentReader.IsApplyChangesCallback)
					gridOwnedLookup.CallbackCommand = GridLookupCallbackArgumentsReader.ApplyChangesCallbackPrefix;
				else if(lookupArgumentReader.IsApplyInputChangesCallback)
					gridOwnedLookup.CallbackCommand = GridLookupCallbackArgumentsReader.ApplyInputChangesCallbackPrefix;
			}
		}
		public static bool SelectAllRowsCallback(this ASPxGridView grid) {
			var gridOwnedLookup = (IGridViewOwnedLookup)grid;
			return gridOwnedLookup.CallbackCommand == "SELECTROWS" && gridOwnedLookup.CallbackCommandArgs != null &&
				Array.Exists<string>(gridOwnedLookup.CallbackCommandArgs, delegate(string s) { return s == "all"; });
		}
		public static OrderedDictionary GetSelectedRowTexts(this ASPxGridView grid) {
			OrderedDictionary selectedRowTexts = new OrderedDictionary();
			string textFormatString = grid.GetTextFormatString();
			List<string> fieldNames = grid.GetFieldNames();
			fieldNames.Add(grid.KeyFieldName);
			List<Object> selectedRowValues = grid.GetSelectedFieldValues(fieldNames.ToArray());
			foreach(object rowValuesObject in selectedRowValues) {
				object[] rowValues = (object[])rowValuesObject;
				if(rowValues.Length > 0) {
					string keyValueForScript = grid.DataProxy.GetKeyValueForScript(rowValues[rowValues.Length - 1]);
					List<object> visibleRowValues = new List<object>(rowValues);
					visibleRowValues.RemoveAt(visibleRowValues.Count - 1);
					selectedRowTexts.Add(keyValueForScript, string.Format(textFormatString, visibleRowValues.ToArray()));
				}
			}
			return selectedRowTexts;
		}
		public static void InitializeAdditionalClientObjectScript(this ASPxGridView grid, StringBuilder stb, string localVarName, string clientID) {
			var gridViewOwnedLookup = (IGridViewOwnedLookup)grid;
			stb.AppendFormat("{0}.itemTexts={1};\n", localVarName, grid.GetVisibleOnPageRowTextsJSON());
			stb.AppendFormat("{0}.callbackCommand='{1}';\n", localVarName, gridViewOwnedLookup.CallbackCommand);
			stb.AppendFormat("{0}.currentSelectionState={1};\n", localVarName, gridViewOwnedLookup.SavedSelectionState);
		}
		public static string GetVisibleOnPageRowTextsJSON(this ASPxGridView grid) {
			var visibleRowTexts = GetVisibleOnPageRowTexts(grid);
			return HtmlConvertor.ToJSON(visibleRowTexts);
		}
		static List<string> GetVisibleOnPageRowTexts(ASPxGridView grid) {
			List<string> itemTexts = new List<string>();
			List<string> fieldNames = grid.GetFieldNames();
			int visibleStopIndex = grid.VisibleStartIndex + grid.DataProxy.VisibleRowCountOnPage;
			for(int i = grid.VisibleStartIndex; i < visibleStopIndex; i++)
				itemTexts.Add(GetVisibleRowTextCore(grid, i, fieldNames));
			return itemTexts;
		}
		public static string GetFocusedRowText(this ASPxGridView grid) {
			return grid.GetVisibleRowText(grid.FocusedRowIndex);
		}
		public static string GetVisibleRowText(this ASPxGridView grid, int rowIndex) {
			var fieldNames = grid.GetFieldNames();
			return GetVisibleRowTextCore(grid, rowIndex, fieldNames);
		}
		static string GetVisibleRowTextCore(ASPxGridView grid, int rowIndex, List<string> fieldNames) {
			if(rowIndex < 0 || grid.VisibleRowCount <= rowIndex)
				return string.Empty;
			string text;
			object valuesObject = grid.GetRowValues(rowIndex, fieldNames.ToArray());
			object[] values = valuesObject as object[];
			if(values != null)
				text = string.Format(grid.GetTextFormatString(), values);
			else
				text = string.Format(grid.GetTextFormatString(), valuesObject);
			return text;
		}
		public static void Filter(this ASPxGridView grid, string filter) {
			grid.PageIndex = 0;
			var textFormatString = grid.GetTextFormatString();
			var fieldNames = grid.GetFieldNames();
			grid.FilterExpression = ((IGridViewOwnedLookup)grid).FilterExpressionCreator.CreateFilterExpression(textFormatString, fieldNames, filter);
			grid.ExpandAll();
		}
		#region RowSelectionByText
		public static List<string> SelectRowsByTexts(this ASPxGridView grid, List<string> texts) {
			UnselectUnnecessaryRows(grid, texts);
			return SelectRowsByTextsCore(grid, texts);
		}
		static void UnselectUnnecessaryRows(ASPxGridView grid, List<string> textList) {
			var fieldNames = grid.GetFieldNames();
			fieldNames.Add(grid.KeyFieldName);
			List<object> rowsValues = grid.GetSelectedFieldValues(fieldNames.ToArray());
			for(int i = 0; i < rowsValues.Count; i++) {
				string rowText;
				object[] rowValues = rowsValues[i] as object[];
				if(rowValues != null) {
					object[] rowTexts = new object[rowValues.Length - 1];
					Array.Copy(rowValues, rowTexts, rowValues.Length - 1);
					rowText = string.Format(grid.GetTextFormatString(), rowTexts);
				}
				else
					rowText = rowsValues[i].ToString();
				if(!textList.Contains(rowText)) {
					object keyValue = rowValues[rowValues.Length - 1];
					grid.Selection.SetSelectionByKey(keyValue, false);
				}
			}
		}
		static List<string> SelectRowsByTextsCore(ASPxGridView grid, List<string> textList) {
			var filterExpressionCreator = new ExactMatchFilterExpressionCreator();
			string previousFilterExpression = grid.FilterExpression;
			for(int i = textList.Count - 1; i >= 0; i--) {
				grid.FilterExpression = filterExpressionCreator.CreateFilterExpression(grid.GetTextFormatString(), grid.GetFieldNames(), textList[i]);
				grid.ExpandAll();
				if(grid.VisibleRowCount > 0) {
					int index = FindFirstDataRowFrom(grid, 0);
					grid.Selection.SelectRow(index);
				}
				else
					textList.RemoveAt(i);
			}
			grid.FilterExpression = previousFilterExpression;
			return new List<string>(textList);
		}
		static int FindFirstDataRowFrom(ASPxGridView grid, int startIndex) {
			int index = startIndex;
			while(grid.IsGroupRow(index) && index < grid.VisibleRowCount)
				index++;
			if(index < grid.VisibleRowCount)
				return index;
			return -1;
		}
		#endregion
		public static List<string> GetFieldNames(this ASPxGridView grid) {
			return ((ISummaryItemsOwner)grid).GetFieldNames();
		}
		static string GetTextFormatString(this ASPxGridView grid) {
			return ((IGridViewOwnedLookup)grid).GLPProperties.GetTextFormatString();
		}
	}
	public interface IGridViewOwnedLookup {
		FilterExpressionCreatorBase FilterExpressionCreator { get; set; }
		GridLookupProperties GLPProperties { get; }
		string CallbackCommand { get; set; }
		string[] CallbackCommandArgs { get; set; }
		string SavedSelectionState { get; set; }
	}
	public class GridLookupCallbackArgumentsReader : CallbackArgumentsReader {
		public const string FilteringCallbackPrefix = "GLP_F";
		public const string CancelChangesCallbackPrefix = "GLP_CC";
		public const string ApplyChangesCallbackPrefix = "GLP_AC";
		public const string ApplyInputChangesCallbackPrefix = "GLP_AIC";
		string rawCallbackArgument;
		public GridLookupCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] { FilteringCallbackPrefix, CancelChangesCallbackPrefix, ApplyChangesCallbackPrefix, ApplyInputChangesCallbackPrefix }) {
			this.rawCallbackArgument = arguments;
		}
		public bool IsGridLookupCallback {
			get { return IsApplyInputChangesCallback || IsApplyChangesCallback || IsCancelChangesCallback || IsFilteringCallback; }
		}
		public bool IsApplyInputChangesCallback { get { return this[ApplyInputChangesCallbackPrefix] != null; } }
		public bool IsApplyChangesCallback { get { return AppliedRowKey != null; } }
		public string AppliedRowKey { get { return this[ApplyChangesCallbackPrefix]; } }
		public bool IsFilteringCallback { get { return Filter != null; } }
		public string Filter { get { return this[FilteringCallbackPrefix]; } }
		protected string CancelChangesBySelectingRowsKeysSerialized { get { return this[CancelChangesCallbackPrefix]; } }
		public bool IsCancelChangesCallback { get { return CancelChangesBySelectingRowsKeysSerialized != null; } }
		public string[] CancelChangesBySelectingRowsKeys {
			get {
				if(IsCancelChangesCallback) {
					List<string> rowKeyFieldValues = CommonUtils.DeserializeStringArray(CancelChangesBySelectingRowsKeysSerialized);
					return rowKeyFieldValues.ToArray();
				}
				return null;
			}
		}
	}
	public static class FormatStringHelper {
		public struct PlaceHolderTemplateStruct {
			int startIndex, length, index;
			string tempString;
			public int StartIndex { get { return startIndex; } }
			public int EndIndex { get { return startIndex + length; } }
			public int Length { get { return length; } }
			public int Index { get { return index; } }
			public string TempString { get { return tempString; } }
			public PlaceHolderTemplateStruct(int startIndex, int length, int index, string tempString) {
				this.startIndex = startIndex;
				this.length = length;
				this.index = index;
				this.tempString = tempString;
			}
		}
		public static List<PlaceHolderTemplateStruct> GetPlaceHolderTemplates(string formatString) {
			formatString = CollapseDoubleBrackets(formatString);
			List<PlaceHolderTemplateStruct> templates = CreatePlaceHolderTemplates(formatString);
			return templates;
		}
		static string CollapseDoubleBrackets(string formatString) {
			formatString = CollapseOpenDoubleBrackets(formatString);
			formatString = CollapseCloseDoubleBrackets(formatString);
			return formatString;
		}
		static string CollapseOpenDoubleBrackets(string formatString) {
			return formatString.Replace("{{", "_");
		}
		static string CollapseCloseDoubleBrackets(string formatString) {
			while(true) {
				int index = formatString.LastIndexOf("}}");
				if(index == -1)
					break;
				else
					formatString = formatString.Substring(0, index) + "_" + formatString.Substring(index + 2);
			}
			return formatString;
		}
		static List<PlaceHolderTemplateStruct> CreatePlaceHolderTemplates(string formatString) {
			List<PlaceHolderTemplateStruct> placeHolders = new List<PlaceHolderTemplateStruct>();
			Regex palceHoldersRegex = new Regex(@"{[^}]+}");
			Regex palceHolderNumberRegex = new Regex(@"\d+");
			MatchCollection matches = palceHoldersRegex.Matches(formatString);
			int pos = 0;
			foreach(Match match in matches) {
				string tempString = match.Value;
				int startIndex = formatString.IndexOf(tempString, pos);
				int length = tempString.Length;
				palceHolderNumberRegex.Matches(tempString);
				string indexString = palceHolderNumberRegex.Match(tempString).Value;
				int index = int.Parse(indexString);
				placeHolders.Add(new PlaceHolderTemplateStruct(startIndex, length, index, tempString));
				pos = startIndex + length;
			}
			return placeHolders;
		}
	}
	public class FilterExpressionCreatorBase {
		public string CreateFilterExpression(string formatString, List<string> fieldNames, string filter) {
			if(string.IsNullOrEmpty(formatString) || string.IsNullOrEmpty(filter))
				return string.Empty;
			return CreateFilterExpressionCoreCore(formatString, fieldNames, filter);
		}
		protected virtual string CreateFilterExpressionCoreCore(string formatString, List<string> fieldNames, string filter) {
			return "";
		}
	}
	public class DisabledFilterExpressionCreator : FilterExpressionCreatorBase {
		protected override string CreateFilterExpressionCoreCore(string formatString, List<string> fieldNames, string filter) {
			return string.Empty;
		}
	}
	public abstract class BaseFilterExpressionCreator : FilterExpressionCreatorBase {
		protected override string CreateFilterExpressionCoreCore(string formatString, List<string> fieldNames, string filter) {
			List<FormatStringHelper.PlaceHolderTemplateStruct> templates = FormatStringHelper.GetPlaceHolderTemplates(formatString);
			int currentPos = 0;
			List<CriteriaOperator> elements = new List<CriteriaOperator>();
			foreach(FormatStringHelper.PlaceHolderTemplateStruct template in templates) {
				string planeText = formatString.Substring(currentPos, template.StartIndex - currentPos);
				string fieldName = fieldNames[template.Index];
				if(!string.IsNullOrEmpty(planeText))
					elements.Add(new OperandValue(planeText));
				elements.Add(new OperandProperty(fieldName));
				currentPos = template.EndIndex;
			}
			int lastTemplatesSymbolPos = templates[templates.Count - 1].EndIndex;
			if(lastTemplatesSymbolPos < formatString.Length) {
				string planeText = formatString.Substring(lastTemplatesSymbolPos, formatString.Length - lastTemplatesSymbolPos);
				elements.Add(new OperandValue(planeText));
			}
			CriteriaOperator left;
			if(elements.Count == 0)
				return string.Empty;
			else if(elements.Count == 1)
				left = elements[0];
			else
				left = new FunctionOperator(FunctionOperatorType.Concat, elements);
			return CriteriaOperator.ToString(CreateFilterCriterion(left, new OperandValue(filter)));
		}
		protected abstract CriteriaOperator CreateFilterCriterion(CriteriaOperator left, CriteriaOperator right);
	}
	public class ExactMatchFilterExpressionCreator : BaseFilterExpressionCreator {
		protected override CriteriaOperator CreateFilterCriterion(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Equal);
		}
	}
	public class StartWithFilterExpressionCreator : BaseFilterExpressionCreator {
		protected override CriteriaOperator CreateFilterCriterion(CriteriaOperator left, CriteriaOperator right) {
			return new FunctionOperator(FunctionOperatorType.StartsWith, left, right);
		}
	}
	public class ContainsFilterExpressionCreator : FilterExpressionCreatorBase {
		const string UniversalSeparator = " ";
		protected override string CreateFilterExpressionCoreCore(string formatString, List<string> fieldNames, string filter) {
			List<FormatStringHelper.PlaceHolderTemplateStruct> templates = FormatStringHelper.GetPlaceHolderTemplates(formatString);
			List<string> constPartsFromFormatString = ExtractConstPartsFromFormatString(templates, formatString);
			string[] uniquePartsFromFilter = ExtractUniquePartsFromFilter(filter, constPartsFromFormatString);
			StringBuilder sb = new StringBuilder();
			bool firstCircle = true;
			foreach(string uniquePart in uniquePartsFromFilter) {
				if(firstCircle) {
					firstCircle = false;
					sb.Append("(");
				} else {
					sb.Append(" and (");
				}
				bool firstTemplate = true;
				foreach(FormatStringHelper.PlaceHolderTemplateStruct template in templates) {
					if(firstTemplate)
						firstTemplate = false;
					else
						sb.Append(" or ");
					sb.AppendFormat("Contains({0}, {1})", new OperandProperty(fieldNames[template.Index]), new OperandValue(uniquePart));
				}
				sb.Append(")");
			}
			return sb.ToString();
		}
		List<string> ExtractConstPartsFromFormatString(List<FormatStringHelper.PlaceHolderTemplateStruct> templates, string formatString) {
			List<string> constPartsFromFormatString = new List<string>();
			int currentPos = 0;
			foreach(FormatStringHelper.PlaceHolderTemplateStruct template in templates) {
				int length = template.StartIndex - currentPos;
				if(length > 0)
					constPartsFromFormatString.Add(formatString.Substring(currentPos, length));
				currentPos = template.EndIndex;
			}
			int lastTemplatesSymbolPos = templates[templates.Count - 1].EndIndex;
			if(lastTemplatesSymbolPos > formatString.Length)
				constPartsFromFormatString.Add(formatString.Substring(lastTemplatesSymbolPos, formatString.Length - lastTemplatesSymbolPos));
			return constPartsFromFormatString;
		}
		string[] ExtractUniquePartsFromFilter(string filter, List<string> constPartsFromFormatString) {
			foreach(string constPart in constPartsFromFormatString) {
				if(!string.IsNullOrEmpty(constPart))
					filter = filter.Replace(constPart, UniversalSeparator);
			}
			return filter.Split(new string[] { UniversalSeparator }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
	public class SelectionStrategyOwner {
		GridLookupProperties glpProperties;
		SelectionStrategyBase selectionStrategy;
		public SelectionStrategyOwner(GridLookupProperties glpProperties) {
			this.glpProperties = glpProperties;
		}
		public void Filter(string filter) {
			GridViewWrapper.Filter(filter);
		}
		public SelectionStrategyBase SelectionStrategy {
			get {
				if(!StrategyIsAppropriate)
					selectionStrategy = CreateSelectionStrategy();
				return selectionStrategy;
			}
		}
		public virtual bool HasDataSource { get { return (!string.IsNullOrEmpty(GridViewWrapper.DataSourceID) || (GridViewWrapper.DataSource != null)); } }
		protected bool StrategyIsAppropriate {
			get {
				if(selectionStrategy == null)
					return false;
				return (selectionStrategy.GetType() == typeof(SingleSelectionStrategy) && !MultiSelect) ||
					(selectionStrategy.GetType() == typeof(MulipleSelectionStrategy) && MultiSelect);
			}
		}
		protected bool MultiSelect { get { return GLPProperties.MultiSelect; } }
		protected GridLookupProperties GLPProperties { get { return glpProperties; } }
		protected virtual ASPxGridView GridViewWrapper { get { return GLPProperties.GridView; } }
		public SelectionStrategyBase CreateSelectionStrategy() {
			if(MultiSelect)
				return new MulipleSelectionStrategy(this);
			else
				return new SingleSelectionStrategy(this);
		}
		public string MultiTextSeparator {
			get {
				if(GLPProperties.GridLookup != null)
					return GLPProperties.GridLookup.MultiTextSeparator;
				return null;
			}
		}
		public virtual string TextFormatString { get { return GLPProperties.GetTextFormatString(); } }
		public string FilterExpression {
			get { return GridViewWrapper.FilterExpression; }
			set { GridViewWrapper.FilterExpression = value; }
		}
		public virtual string KeyFieldName { get { return GridViewWrapper.KeyFieldName; } }
		public virtual int FocusedRowIndex {
			get { return GridViewWrapper.FocusedRowIndex; }
			set { GridViewWrapper.FocusedRowIndex = value; }
		}
		public bool KeepClientTextsOrder{
			get { return !GridViewWrapper.SelectAllRowsCallback(); }
		}
		public virtual List<string> GetFieldNames() {
			return GridViewWrapper.GetFieldNames();
		}
		public virtual OrderedDictionary GetSelectedRowTexts() {
			return GridViewWrapper.GetSelectedRowTexts();
		}
		public virtual List<object> GetSelectedFieldValues(params string[] fields) {
			return GridViewWrapper.GetSelectedFieldValues(fields);
		}
		public virtual string GetFocusedRowText() {
			return GridViewWrapper.GetFocusedRowText();
		}
		public void UnselectAll() {
			GridViewWrapper.Selection.UnselectAll();
		}
		public virtual object GetRowValues(int visibleIndex, params string[] fieldNames) {
			return GridViewWrapper.GetRowValues(visibleIndex, fieldNames);
		}
		public int FindVisibleIndexByKeyValue(object keyValue) {
			return GridViewWrapper.FindVisibleIndexByKeyValue(keyValue);
		}
		public object GetKeyValueFromScript(string value) {
			return GridViewWrapper.DataProxy.GetKeyValueFromScript(value);
		}
		public void SelectRowByKey(object keyValue) {
			GridViewWrapper.Selection.SelectRowByKey(keyValue);
		}
		public void SetSelectionByKey(object keyValue, bool selected) {
			GridViewWrapper.Selection.SetSelectionByKey(keyValue, selected);
		}
		public List<string> SelectRowsByTexts(List<string> texts) {
			return GridViewWrapper.SelectRowsByTexts(texts);
		}
	}
	public abstract class SelectionStrategyBase {
		private SelectionStrategyOwner owner = null;
		private string clientText = null;
		public SelectionStrategyBase(SelectionStrategyOwner owner) {
			this.owner = owner;
		}
		public abstract string Text { get; set; }
		public abstract object Value { get; set; }
		protected internal string ClientText {
			get { return clientText; }
			set { clientText = value; }
		}
		protected SelectionStrategyOwner Owner { get { return owner; } }
		public virtual void ApplyGridViewChanges(string appliedRowValue) { }
		public virtual void ApplyInputChanges() { }
		public virtual void Filter(string filter) {
			Owner.Filter(filter);
		}
		public virtual void CancelChanges(string[] keyFieldValues) {
			Owner.UnselectAll();
			Owner.FilterExpression = string.Empty;
		}
		public virtual string MultiTextSeparator {
			get { return Owner.MultiTextSeparator; }
		}
		public abstract List<object> GetSelectedKeyValues();
		public abstract IDictionary GetSelectedRowTexts();
		public GridViewSelectionState GetSelectionState() {
			return new GridViewSelectionState(
			  Text, GetSelectedRowTexts());
		}
		public string GetSelectionStateJSON() {
			string selectionState = GetSelectionState().ToJSON();
			return string.IsNullOrEmpty(selectionState) ? "[]" : selectionState;
		}
	}
	public class SingleSelectionStrategy : SelectionStrategyBase {
		public SingleSelectionStrategy(SelectionStrategyOwner owner) : base(owner) { }
		public override string Text {
			get {
				ConvertSelectionToFocus(false);
				return Owner.GetFocusedRowText();
			}
			set { SetText(value); }
		}
 		public override void Filter(string filter) {
			base.Filter(filter);
			if(string.IsNullOrEmpty(filter))
				Owner.FocusedRowIndex = -1;
		}
		public override object Value {
			get {
				ConvertSelectionToFocus(false);
				object value = Owner.GetRowValues(Owner.FocusedRowIndex, Owner.KeyFieldName);
				object[] values = value as object[];
				if(values != null)
					return values[0];
				return value;
			}
			set {
				Owner.UnselectAll();
				FocusRowByKey(value);
			}
		}
		public override void ApplyGridViewChanges(string appliedRowValue) {
			if(appliedRowValue != null)
				SetSelectionFromClientKeyValue(appliedRowValue);
			else
				ConvertSelectionToFocus(true);
		}
		public override void ApplyInputChanges() {
			Owner.UnselectAll();
			SetText(ClientText);
		}
		public override void CancelChanges(string[] keyFieldValues) {
			base.CancelChanges(keyFieldValues);
			if(keyFieldValues.Length > 0)
				SetSelectionFromClientKeyValue(keyFieldValues[0]);
			else
				Owner.FocusedRowIndex = -1;
		}
		protected void ConvertSelectionToFocus(bool allowClearFocus) {
			List<object> keyValues = Owner.GetSelectedFieldValues(Owner.KeyFieldName);
			if(keyValues.Count != 0) {
				Owner.SetSelectionByKey(keyValues[0], false);
				Owner.FocusedRowIndex = Owner.FindVisibleIndexByKeyValue(keyValues[0]);
			} else if(allowClearFocus)
				Owner.FocusedRowIndex = -1;
		}
		protected void SetText(string text) {
			if(!string.IsNullOrEmpty(text))
				Owner.SelectRowsByTexts(new List<string>(new string[] { text }));
			else
				Owner.FocusedRowIndex = -1;
			ConvertSelectionToFocus(true);
		}
		protected void FocusRowByKey(object keyValue) {
			int visibleRowIndex = Owner.FindVisibleIndexByKeyValue(keyValue);
			Owner.FocusedRowIndex = visibleRowIndex < 0 ? -1 : visibleRowIndex;
		}
		protected void SetSelectionFromClientKeyValue(string clientKeyValue) {
			int selectedRowVisibleIndex = Owner.FindVisibleIndexByKeyValue(Owner.GetKeyValueFromScript(clientKeyValue));
			Owner.FocusedRowIndex = selectedRowVisibleIndex;
		}
		public override IDictionary GetSelectedRowTexts() {
			Dictionary<string, string> selectedRowTexts = new Dictionary<string, string>();
			if(Owner.HasDataSource) {
				object selectedKeyValue = Owner.FocusedRowIndex >= 0 ? Owner.GetRowValues(Owner.FocusedRowIndex, Owner.KeyFieldName) : null;
				if(selectedKeyValue != null)
					selectedRowTexts[selectedKeyValue.ToString()] = Text;
			}
			return selectedRowTexts;
		}
		public override List<object> GetSelectedKeyValues() {
			List<object> selectedKeyValues = new List<object>();
			selectedKeyValues.Add(Owner.GetRowValues(Owner.FocusedRowIndex, Owner.KeyFieldName));
			return selectedKeyValues;
		}
	}
	public class MulipleSelectionStrategy : SelectionStrategyBase {
		public MulipleSelectionStrategy(SelectionStrategyOwner owner) : base(owner) { }
		public override string Text {
			get { return GetText(); }
			set { SetText(value); }
		}
		public override object Value {
			get {
				List<object> values = GetSelectedKeyValues();
				if(values.Count > 0)
					return values[0];
				return null;
			}
			set {
				Owner.UnselectAll();
				Owner.SelectRowByKey(value);
			}
		}
		public override string MultiTextSeparator {
			get { return Owner.MultiTextSeparator; }
		}
		public override IDictionary GetSelectedRowTexts() {
			OrderedDictionary selectedRowTexts = Owner.GetSelectedRowTexts();
			return MultiSelectionStrategyUtils.OrderSelectedRowsSequence(selectedRowTexts, ClientText, MultiTextSeparator);
		}
		public override void ApplyInputChanges() {
			SetText(ClientText);
		}
		public override void CancelChanges(string[] keyFieldValues) {
			base.CancelChanges(keyFieldValues);
			foreach(string keyFieldValue in keyFieldValues)
				Owner.SelectRowByKey(Owner.GetKeyValueFromScript(keyFieldValue));
		}
		public override List<object> GetSelectedKeyValues() {
			return Owner.GetSelectedFieldValues(Owner.KeyFieldName);
		}
		protected string GetText() {
			List<string> fields = Owner.GetFieldNames();
			List<object> rowsValues = Owner.GetSelectedFieldValues(fields.ToArray());
			List<string> gridViewSelectedRowTexts = new List<string>();
			for(int i = 0; i < rowsValues.Count; i++) {
				object[] rowValues = rowsValues[i] as object[] ?? new object[] { rowsValues[i] };
				string rowText = string.Format(Owner.TextFormatString, rowValues);
				gridViewSelectedRowTexts.Add(rowText);
			}
			return MultiSelectionStrategyUtils.CorrectTextBasedOnGridSelection(ClientText, gridViewSelectedRowTexts, MultiTextSeparator, Owner.KeepClientTextsOrder);
		}
		protected void SetText(string text) {
			if(text != null) {
				List<string> textsForGridSelection = MultiSelectionStrategyUtils.GetTextsForGridSelection(text, MultiTextSeparator);
				List<string> gridSelectedTexts = Owner.SelectRowsByTexts(textsForGridSelection);
				ClientText = MultiSelectionStrategyUtils.GetFormattedText(gridSelectedTexts, MultiTextSeparator);
			} else
				Owner.UnselectAll();
		}
		#region Utils
		public static class MultiSelectionStrategyUtils {
			private static List<string> SplitText(string text, string separator) {
				if(string.IsNullOrEmpty(text))
					return new List<string>();
				string[] result = text.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
				return new List<string>(result);
			}
			public static bool IsSeparatorEmpty(string separator) {
				if(string.IsNullOrEmpty(separator))
					return true;
				for(int i = 0; i < separator.Length; i++)
					if(separator[i] != ' ')
						return false;
				return true;
			}
			public static List<string> OrderSelectedRowTextsBasedOnClientText(List<string> clientTexts, List<string> gridViewSelectedRowTexts) {
				if(clientTexts == null || clientTexts.Count == 0)
					return gridViewSelectedRowTexts;
				List<string> lowerGridViewSelectedRowTexts = new List<string>(gridViewSelectedRowTexts);
				clientTexts = TrimUtils.Trim(clientTexts, true);
				lowerGridViewSelectedRowTexts = lowerGridViewSelectedRowTexts.ConvertAll(new Converter<string, string>(
					delegate(string str) { return str.ToLower(); }
				));
				for(int i = 0; i < clientTexts.Count; i++) {
					if(!lowerGridViewSelectedRowTexts.Contains(clientTexts[i]))
						return gridViewSelectedRowTexts;
					else
						clientTexts[i] = gridViewSelectedRowTexts[lowerGridViewSelectedRowTexts.IndexOf(clientTexts[i])];
				}
				for(int i = 0; i < clientTexts.Count; i++) {
					if(!gridViewSelectedRowTexts.Contains(clientTexts[i]))
						clientTexts[i] = null;
				}
				clientTexts.RemoveAll(new Predicate<string>(
					delegate(string str) { return str == null; }
				));
				for(int i = 0; i < gridViewSelectedRowTexts.Count; i++) {
					if(!clientTexts.Contains(gridViewSelectedRowTexts[i]))
						clientTexts.Add(gridViewSelectedRowTexts[i]);
				}
				return clientTexts;
			}
			public static List<string> RemoveSimilarTexts(List<string> texts) {
				for(int i = texts.Count - 1; i > 0; i--) {
					if(texts.FindAll(new Predicate<string>(delegate(string str) { return TrimUtils.Trim(str, true) == TrimUtils.Trim(texts[i], true); })).Count > 1)
						texts.RemoveAt(i);
				}
				return texts;
			}
			public static List<string> GetTextsForGridSelection(string text, string separator) {
				separator = IsSeparatorEmpty(separator) ? " " : TrimUtils.Trim(separator);
				List<string> texts = MultiSelectionStrategyUtils.SplitText(text, separator);
				texts = RemoveSimilarTexts(texts);
				return TrimUtils.Trim(texts, true);
			}
			public static string CorrectTextBasedOnGridSelection(string clientText, List<string> gridViewSelectedRowTexts, string separator, bool keepClientTextsOrder) {
				string emptySeparator = IsSeparatorEmpty(separator) ? " " : TrimUtils.Trim(separator);
				List<string> clientTexts = SplitText(clientText, emptySeparator);
				List<string> validClientTexts = gridViewSelectedRowTexts;
				if(keepClientTextsOrder)
					validClientTexts = OrderSelectedRowTextsBasedOnClientText(clientTexts, gridViewSelectedRowTexts);
				return GetFormattedText(validClientTexts, separator);
			}
			public static string GetFormattedText(List<string> texts, string separator) {
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < texts.Count; i++) {
					sb.Append(texts[i]);
					if(i != texts.Count - 1)
						sb.Append(separator);
				}
				return sb.ToString();
			}
			public static OrderedDictionary OrderSelectedRowsSequence(OrderedDictionary selectedRows, string text, string separator) {
				try {
					return OrderSelectedRowsSequenceCore(selectedRows, text, separator);
				} catch {
					return selectedRows;
				}
			}
			private static OrderedDictionary OrderSelectedRowsSequenceCore(OrderedDictionary selectedRows, string text, string separator) {
				if(string.IsNullOrEmpty(text) || selectedRows.Count == 0)
					return selectedRows;
				List<string> texts = MultiSelectionStrategyUtils.SplitText(text, separator);
				texts = TrimUtils.Trim(texts, true);
				OrderedDictionary orderedSelectedRows = new OrderedDictionary();
				string[] keysMask = new string[texts.Count];
				foreach(string key in selectedRows.Keys) {
					int currentRowIndex = texts.IndexOf(TrimUtils.Trim((string)selectedRows[key], true));
					if(currentRowIndex < 0)
						throw new InvalidOperationException();
					keysMask[currentRowIndex] = key;
				}
				foreach(string keyMask in keysMask) {
					if(keyMask == null)
						throw new InvalidOperationException();
					orderedSelectedRows[keyMask] = selectedRows[keyMask];
				}
				return orderedSelectedRows;
			}
		}
		protected static class TrimUtils {
			public static string Trim(string text, bool toLower) {
				string trimmedText = text.Trim(' ');
				return toLower ? trimmedText.ToLower() : trimmedText;
			}
			public static string Trim(string text) { return Trim(text, false); }
			public static List<string> Trim(List<string> texts) { return Trim(texts, false); }
			public static List<string> Trim(List<string> texts, bool toLower) {
				List<string> result = new List<string>();
				foreach(string text in texts) {
					string trimmedText = Trim(text, toLower);
					if(!string.IsNullOrEmpty(trimmedText))
						result.Add(trimmedText);
				}
				return result;
			}
		}
		#endregion
	}
	public class GridViewSelectionState {
		string inputText;
		IDictionary selectedRowTexts;
		public GridViewSelectionState(string inputText, IDictionary selectedRowTexts) {
			this.inputText = inputText;
			this.selectedRowTexts = selectedRowTexts;
		}
		internal string InputText { get { return inputText; } }
		internal IDictionary SelectedRowTexts { get { return selectedRowTexts; } }
		internal string ToJSON() {
			return string.Format("{{InputText: {0}, SelectedRowTexts: {1}}}", HtmlConvertor.ToJSON(InputText), HtmlConvertor.ToJSON(SelectedRowTexts));
		}
	}
}
