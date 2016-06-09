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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxTreeList.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Data;
using DevExpress.Utils;
using System.Web;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxTreeList {
	[DXWebToolboxItem(true), 
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxTreeList"), 
	DefaultProperty("Columns"),
	Designer("DevExpress.Web.ASPxTreeList.Design.ASPxTreeListDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ASPxTreeList), "Bitmaps256.ASPxTreeList.bmp")
	]
	public class ASPxTreeList : ASPxDataWebControl, IWebColumnsOwner, IPagerOwner, IControlDesigner {
		internal const string
			ResourcePath = "DevExpress.Web.ASPxTreeList.",
			ResourceImagesPath = ResourcePath + "Images.",
			ResourceScriptsPath = ResourcePath + "Scripts.";
		internal const string
			ScriptName = ResourceScriptsPath + "TreeList.js",
			ScrollUtilsScriptName = ResourceScriptsPath + "TreeListScrollUtils.js",
			DefaultCssName = ResourcePath + "Css.Default.css",
			SpriteCssName = ResourcePath + "Css.Sprite.css",
			SystemCssName = ResourcePath + "Css.System.css";
		internal readonly string[] ChildControlNames = new string[] { "Web", "Editors" };
		internal const string SkinControlName = "TreeList";
		#region Event keys
		static readonly object
			EventNodeExpanding = new object(),
			EventNodeCollapsing = new object(),
			EventNodeExpanded = new object(),
			EventNodeCollapsed = new object(),
			EventCustomSummaryCalculate = new object(),
			EventCustomCallback = new object(),
			EventHtmlRowPrepared = new object(),
			EventHtmlDataCellPrepared = new object(),
			EventVirtualModeCreateChildren = new object(),
			EventVirtualModeNodeCreating = new object(),
			EventVirtualModeNodeCreated = new object(),
			EventSelectionChanged = new object(),
			EventFocusedNodeChanged = new object(),
			EventPageIndexChanged = new object(),
			EventPageSizeChanged = new object(),
			EventStartNodeEditing = new object(),
			EventCancelNodeEditing = new object(),
			EventNodeUpdating = new object(),
			EventNodeUpdated = new object(),
			EventNodeDeleting = new object(),
			EventNodeDeleted = new object(),
			EventInitNewNode = new object(),
			EventNodeInserting = new object(),
			EventNodeInserted = new object(),
			EventNodeValidating = new object(),
			EventCellEditorInitialize = new object(),
			EventHtmlCommandCellPrepared = new object(),
			EventProcessDragNode = new object(),
			EventParseValue = new object(),
			EventCustomErrorText = new object(),
			EventCustomNodeSort = new object(),
			EventEditingOperationCompleted = new object(),
			EventCommandColumnButtonInitialize = new object();
		#endregion
		#region Fields
		bool nodesCreated;
		bool nodesCreating;		
		WebColumnsOwnerDefaultImplementation webColumnsOwnerImpl;
		TreeListDataHelper treeDataHelper;		
		TreeListRenderHelper renderHelper;
		TreeListTemplateHelper templateHelper;
		TreeListCommandButtonsHelper commandButtonsHelper;
		TreeListCommand commandToExecute;
		TreeListPendingChange pendingChange;
		bool pendingChangesReleased;
		TreeListColumnCollection columns;		
		TreeListNode rootNode;
		List<TreeListColumn> visibleColumnsStorage;
		bool sortedColumnsCreated;
		int sortLockCount;
		List<TreeListDataColumn> sortedColumns;
		TreeListNodeComparer nodeComparer;
		TreeListSettings settings;
		TreeListSettingsBehavior settingsBehavior;
		TreeListSettingsPager settingsPager;
		TreeListSettingsCustomizationWindow settingsCustomizationWindow;
		TreeListSettingsSelection settingsSelection;
		TreeListSettingsCookies settingsCookies;
		TreeListSettingsEditing settingsEditing;
		TreeListSettingsPopupEditForm settingsPopupEditForm;
		TreeListSettingsDataSecurity settingsDataSecurity;
		TreeListSettingsText settingsText;
		TreeListMainTable mainTable;
		TreeListEditorRegistrator editorRegistrator;
		TreeListSummaryCollection summary;
		TreeListTemplates templates;
		PagerStyles stylesPager;
		int initialPageSize;
		EditorStyles stylesEditors;
		EditorImages imagesEditors;
		string editorValuesState;
		bool stateExists;
		bool requireApplyPostData;
		static object EmptyRootValue = new object();
		object rootValue;
		#endregion
		public ASPxTreeList() {
			this.nodesCreated = false;
			this.nodesCreating = false;
			this.treeDataHelper = CreateDataHelper();
			this.renderHelper = CreateRenderHelper();		
			this.commandToExecute = null;
			this.pendingChange = TreeListPendingChange.Empty;
			this.pendingChangesReleased = false;
			this.columns = new TreeListColumnCollection(this);
			this.webColumnsOwnerImpl = new WebColumnsOwnerDefaultImplementation(this, Columns);
			this.rootNode = new TreeListRootNode(TreeDataHelper);
			this.visibleColumnsStorage = new List<TreeListColumn>();
			this.sortedColumnsCreated = false;
			this.sortLockCount = 0;
			this.sortedColumns = new List<TreeListDataColumn>();
			this.nodeComparer = new TreeListNodeComparer(this);
			this.settings = CreateSettings();
			this.settingsBehavior = CreateBehaviorSettings();
			this.settingsPager = new TreeListSettingsPager(this);
			this.settingsCustomizationWindow = CreateCustomizationWindowSettings();
			this.settingsSelection = CreateSelectionSettings();
			this.settingsCookies = CreateCookiesSettings();
			this.settingsEditing = CreateEditingSettings();
			this.settingsPopupEditForm = CreatePopupEditFormSettings();
			this.settingsDataSecurity = CreateDataSecuritySettings();
			this.settingsText = CreateTextSettings();
			EnableCallBacksInternal = true;
			this.summary = new TreeListSummaryCollection(this);	
			this.templates = new TreeListTemplates(this);
			this.stylesPager = new PagerStyles(this);
			this.stylesEditors = new EditorStyles(this);
			this.imagesEditors = new EditorImages(this);
			this.editorValuesState = String.Empty;
			this.stateExists = false;
			this.requireApplyPostData = true;
			this.rootValue = EmptyRootValue;
		}
		#region Public Properties
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCaption"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSummaryText"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true),
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
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListKeyboardSupport"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool KeyboardSupport {
			get { return GetBoolProperty("KeyboardSupport", false); }
			set {
				if(value == KeyboardSupport) return;
				SetBoolProperty("KeyboardSupport", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEnableCallbacks"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEnablePagingCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public bool EnablePagingCallbackAnimation {
			get { return EnableSlideCallbackAnimationInternal; }
			set { EnableSlideCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEnablePagingGestures"),
#endif
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnablePagingGestures
		{
			get { return EnableSwipeGesturesInternal; }
			set { EnableSwipeGesturesInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListColumns"),
#endif
		Category("Data"), MergableProperty(false),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public TreeListColumnCollection Columns { get { return columns; } }
		bool ShouldSerializeColumns() {
			return Columns.Count > 0 && !AutoGenerateColumns;
		}
		[Browsable(false)]
		public ReadOnlyTreeListColumnCollection<TreeListDataColumn> DataColumns { get { return RenderHelper.AllDataColumns; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListKeyFieldName"),
#endif
		Category("Data"), DefaultValue(""), AutoFormatDisable, TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), Localizable(false)]
		public string KeyFieldName {
			get { return GetStringProperty("KeyFieldName", String.Empty); }
			set { SetStringProperty("KeyFieldName", String.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListParentFieldName"),
#endif
		Category("Data"), DefaultValue(""), AutoFormatDisable, TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), Localizable(false)]
		public string ParentFieldName {
			get { return GetStringProperty("ParentFieldName", String.Empty); }
			set { SetStringProperty("ParentFieldName", String.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListPreviewFieldName"),
#endif
		Category("Data"), DefaultValue(""), AutoFormatDisable, TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter)), Localizable(false)]
		public string PreviewFieldName {
			get { return GetStringProperty("PreviewFieldName", string.Empty); }
			set { SetStringProperty("PreviewFieldName", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListAutoGenerateColumns"),
#endif
		Category("Data"), DefaultValue(true), AutoFormatDisable]
		public bool AutoGenerateColumns {
			get { return GetBoolProperty("AutoGenerateColumns", true); }
			set { SetBoolProperty("AutoGenerateColumns", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListAutoGenerateServiceColumns"),
#endif
		Category("Data"), DefaultValue(false), AutoFormatDisable]
		public bool AutoGenerateServiceColumns {
			get { return GetBoolProperty("AutoGenerateServiceColumns", false); }
			set { SetBoolProperty("AutoGenerateServiceColumns", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListDataCacheMode"),
#endif
		Category("Data"), DefaultValue(TreeListDataCacheMode.Auto), AutoFormatDisable]
		public TreeListDataCacheMode DataCacheMode {
			get { return (TreeListDataCacheMode)GetEnumProperty("DataCacheMode", TreeListDataCacheMode.Auto); }
			set { SetEnumProperty("DataCacheMode", TreeListDataCacheMode.Auto, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSummary"),
#endif
		Category("Data"), AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public TreeListSummaryCollection Summary {
			get { return summary; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettings"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettings Settings { get { return settings; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsBehavior"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsBehavior SettingsBehavior { get { return settingsBehavior; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsPager"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsPager SettingsPager { get { return settingsPager; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsCustomizationWindow"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsCustomizationWindow SettingsCustomizationWindow { get { return settingsCustomizationWindow; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsSelection"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsSelection SettingsSelection { get { return settingsSelection; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsCookies"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsCookies SettingsCookies { get { return settingsCookies; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new TreeListSettingsLoadingPanel SettingsLoadingPanel { get { return (TreeListSettingsLoadingPanel)base.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsEditing"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsEditing SettingsEditing { get { return settingsEditing; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsPopupEditForm"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsPopupEditForm SettingsPopupEditForm { get { return settingsPopupEditForm; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsDataSecurity"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsDataSecurity SettingsDataSecurity { get { return settingsDataSecurity; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSettingsText"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListSettingsText SettingsText { get { return settingsText; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListImages Images { get { return ImagesInternal as TreeListImages; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorImages ImagesEditors { get { return imagesEditors; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListStyles Styles { get { return StylesInternal as TreeListStyles; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListStylesPager"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerStyles StylesPager { get { return stylesPager; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings { get { return (ControlStyle as AppearanceStyle).Paddings; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable, MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListClientSideEvents ClientSideEvents { get { return ClientSideEventsInternal as TreeListClientSideEvents; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListClientInstanceName"),
#endif
		Category("Client-Side"), AutoFormatDisable, DefaultValue(""), Localizable(false)]
		public string ClientInstanceName { get { return ClientInstanceNameInternal; } set { ClientInstanceNameInternal = value; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListClientVisible"),
#endif
		Category("Client-Side"), AutoFormatDisable, DefaultValue(true)]
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[Browsable(false), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListTemplates Templates { get { return templates; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListNode RootNode {
			get {
				EnsureNodesCreated();
				return rootNode;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<TreeListColumn> VisibleColumns {
			get {
				this.visibleColumnsStorage.Clear();
				foreach(WebColumnBase column in WebColumnsOwnerImpl.GetVisibleColumns()) {
					TreeListColumn treeColumn = column as TreeListColumn;
					if(treeColumn == null)
						throw new InvalidCastException();
					this.visibleColumnsStorage.Add(treeColumn);
				}
				return this.visibleColumnsStorage.AsReadOnly();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object RootValue {
			get {
				if(!IsRootValueAssigned)
					return null;
				return rootValue;
			}
			set { rootValue = value; }
		}
		#endregion
		#region Internal Properties
		protected internal TreeListDataHelper TreeDataHelper { get { return treeDataHelper; } }		
		protected internal TreeListRenderHelper RenderHelper { get { return renderHelper; } }
		protected WebColumnsOwnerDefaultImplementation WebColumnsOwnerImpl { get { return webColumnsOwnerImpl; } }
		protected internal TreeListTemplateHelper TemplateHelper {
			get {
				if(templateHelper == null)
					templateHelper = new TreeListTemplateHelper(this);
				return templateHelper;
			}
		}
		protected internal TreeListCommandButtonsHelper CommandButtonsHelper {
			get {
				if(commandButtonsHelper == null)
					commandButtonsHelper = new TreeListCommandButtonsHelper();
				return commandButtonsHelper;
			}
		}
		protected internal List<TreeListDataColumn> SortedColumns {
			get {
				EnsureSortedColumns();
				return sortedColumns;
			}
		}
		protected bool Sorting { get { return sortLockCount > 0; } }
		protected TreeListNodeComparer NodeComparer { get { return nodeComparer; } }
		protected internal TreeListCommand CommandToExecute { get { return commandToExecute; } set { commandToExecute = value; } }
		protected TreeListPendingChange PendingChange { get { return pendingChange; } set { pendingChange = value; } }		
		protected TreeListMainTable MainTable { get { return mainTable; } }
		protected internal Control EditorRegistrator { get { return editorRegistrator; } }
		protected internal bool NodesCreated { get { return nodesCreated; } }
		protected bool NodesCreating { get { return nodesCreating; } }
		protected override bool EnableClientSideAPIInternal { get { return true; } set { } }
		protected override bool AutoPostBackInternal { get { return !EnableCallbacks; } set { } } 
		protected internal bool IsRootValueAssigned {
			get { return !Object.ReferenceEquals(rootValue, EmptyRootValue); }
		}
		protected internal int InitialPageSize {
			get { return initialPageSize; }
			set { initialPageSize = value; }
		}
		#endregion
		#region Events
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeExpanding"),
#endif
		Category("Action")]
		public event TreeListNodeCancelEventHandler NodeExpanding
		{
			add { Events.AddHandler(EventNodeExpanding, value); }
			remove { Events.RemoveHandler(EventNodeExpanding, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeCollapsing"),
#endif
		Category("Action")]
		public event TreeListNodeCancelEventHandler NodeCollapsing
		{
			add { Events.AddHandler(EventNodeCollapsing, value); }
			remove { Events.RemoveHandler(EventNodeCollapsing, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeExpanded"),
#endif
		Category("Action")]
		public event TreeListNodeEventHandler NodeExpanded
		{
			add { Events.AddHandler(EventNodeExpanded, value); }
			remove { Events.RemoveHandler(EventNodeExpanded, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeCollapsed"),
#endif
		Category("Action")]
		public event TreeListNodeEventHandler NodeCollapsed
		{
			add { Events.AddHandler(EventNodeCollapsed, value); }
			remove { Events.RemoveHandler(EventNodeCollapsed, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListPageIndexChanged"),
#endif
		Category("Action")]
		public event EventHandler PageIndexChanged
		{
			add { Events.AddHandler(EventPageIndexChanged, value); }
			remove { Events.RemoveHandler(EventPageIndexChanged, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListPageSizeChanged"),
#endif
		Category("Action")]
		public event EventHandler PageSizeChanged
		{
			add { Events.AddHandler(EventPageSizeChanged, value); }
			remove { Events.RemoveHandler(EventPageSizeChanged, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListFocusedNodeChanged"),
#endif
		Category("Action")]
		public event EventHandler FocusedNodeChanged
		{
			add { Events.AddHandler(EventFocusedNodeChanged, value); }
			remove { Events.RemoveHandler(EventFocusedNodeChanged, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListSelectionChanged"),
#endif
		Category("Action")]
		public event EventHandler SelectionChanged
		{
			add { Events.AddHandler(EventSelectionChanged, value); }
			remove { Events.RemoveHandler(EventSelectionChanged, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomCallback"),
#endif
		Category("Action")]
		public event TreeListCustomCallbackEventHandler CustomCallback
		{
			add { Events.AddHandler(EventCustomCallback, value); }
			remove { Events.RemoveHandler(EventCustomCallback, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListStartNodeEditing"),
#endif
		Category("Action")]
		public event TreeListNodeEditingEventHandler StartNodeEditing
		{
			add { Events.AddHandler(EventStartNodeEditing, value); }
			remove { Events.RemoveHandler(EventStartNodeEditing, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCancelNodeEditing"),
#endif
		Category("Action")]
		public event TreeListNodeEditingEventHandler CancelNodeEditing
		{
			add { Events.AddHandler(EventCancelNodeEditing, value); }
			remove { Events.RemoveHandler(EventCancelNodeEditing, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeUpdating"),
#endif
		Category("Action")]
		public event ASPxDataUpdatingEventHandler NodeUpdating
		{
			add { Events.AddHandler(EventNodeUpdating, value); }
			remove { Events.RemoveHandler(EventNodeUpdating, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeUpdated"),
#endif
		Category("Action")]
		public event ASPxDataUpdatedEventHandler NodeUpdated
		{
			add { Events.AddHandler(EventNodeUpdated, value); }
			remove { Events.RemoveHandler(EventNodeUpdated, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeDeleting"),
#endif
		Category("Action")]
		public event ASPxDataDeletingEventHandler NodeDeleting
		{
			add { Events.AddHandler(EventNodeDeleting, value); }
			remove { Events.RemoveHandler(EventNodeDeleting, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeDeleted"),
#endif
		Category("Action")]
		public event ASPxDataDeletedEventHandler NodeDeleted
		{
			add { Events.AddHandler(EventNodeDeleted, value); }
			remove { Events.RemoveHandler(EventNodeDeleted, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeInserting"),
#endif
		Category("Action")]
		public event ASPxDataInsertingEventHandler NodeInserting
		{
			add { Events.AddHandler(EventNodeInserting, value); }
			remove { Events.RemoveHandler(EventNodeInserting, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeInserted"),
#endif
		Category("Action")]
		public event ASPxDataInsertedEventHandler NodeInserted
		{
			add { Events.AddHandler(EventNodeInserted, value); }
			remove { Events.RemoveHandler(EventNodeInserted, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListNodeValidating"),
#endif
		Category("Action")]
		public event TreeListNodeValidationEventHandler NodeValidating
		{
			add { Events.AddHandler(EventNodeValidating, value); }
			remove { Events.RemoveHandler(EventNodeValidating, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListProcessDragNode"),
#endif
		Category("Action")]
		public event TreeListNodeDragEventHandler ProcessDragNode
		{
			add { Events.AddHandler(EventProcessDragNode, value); }
			remove { Events.RemoveHandler(EventProcessDragNode, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListEditingOperationCompleted"),
#endif
		Category("Action")]
		public event TreeListEditingOperationEventHandler EditingOperationCompleted
		{
			add { Events.AddHandler(EventEditingOperationCompleted, value); }
			remove { Events.RemoveHandler(EventEditingOperationCompleted, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCellEditorInitialize"),
#endif
		Category("Behavior")]
		public event TreeListColumnEditorEventHandler CellEditorInitialize
		{
			add { Events.AddHandler(EventCellEditorInitialize, value); }
			remove { Events.RemoveHandler(EventCellEditorInitialize, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomSummaryCalculate"),
#endif
		Category("Data")]
		public event TreeListCustomSummaryEventHandler CustomSummaryCalculate
		{
			add { Events.AddHandler(EventCustomSummaryCalculate, value); }
			remove { Events.RemoveHandler(EventCustomSummaryCalculate, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomDataCallback"),
#endif
		Category("Data")]
		public event TreeListCustomDataCallbackEventHandler CustomDataCallback
		{
			add { Events.AddHandler(EventCustomDataCallback, value); }
			remove { Events.RemoveHandler(EventCustomDataCallback, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomNodeSort"),
#endif
		Category("Data")]
		public event TreeListCustomNodeSortEventHandler CustomNodeSort
		{
			add { Events.AddHandler(EventCustomNodeSort, value); }
			remove { Events.RemoveHandler(EventCustomNodeSort, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListInitNewNode"),
#endif
		Category("Data")]
		public event ASPxDataInitNewRowEventHandler InitNewNode
		{
			add { Events.AddHandler(EventInitNewNode, value); }
			remove { Events.RemoveHandler(EventInitNewNode, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListParseValue"),
#endif
		Category("Data")]
		public event ASPxParseValueEventHandler ParseValue
		{
			add { Events.AddHandler(EventParseValue, value); }
			remove { Events.RemoveHandler(EventParseValue, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event TreeListCustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListHtmlRowPrepared"),
#endif
		Category("Rendering")]
		public event TreeListHtmlRowEventHandler HtmlRowPrepared
		{
			add
			{
				Events.AddHandler(EventHtmlRowPrepared, value);
				LayoutChanged();
			}
			remove
			{
				Events.RemoveHandler(EventHtmlRowPrepared, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListHtmlDataCellPrepared"),
#endif
		Category("Rendering")]
		public event TreeListHtmlDataCellEventHandler HtmlDataCellPrepared
		{
			add
			{
				Events.AddHandler(EventHtmlDataCellPrepared, value);
				LayoutChanged();
			}
			remove
			{
				Events.RemoveHandler(EventHtmlDataCellPrepared, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListHtmlCommandCellPrepared"),
#endif
		Category("Rendering")]
		public event TreeListHtmlCommandCellEventHandler HtmlCommandCellPrepared
		{
			add { Events.AddHandler(EventHtmlCommandCellPrepared, value); }
			remove { Events.RemoveHandler(EventHtmlCommandCellPrepared, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCustomErrorText"),
#endif
		Category("Rendering")]
		public event TreeListCustomErrorTextEventHandler CustomErrorText
		{
			add { Events.AddHandler(EventCustomErrorText, value); }
			remove { Events.RemoveHandler(EventCustomErrorText, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListCommandColumnButtonInitialize"),
#endif
		Category("Rendering")]
		public event TreeListCommandColumnButtonEventHandler CommandColumnButtonInitialize
		{
			add { Events.AddHandler(EventCommandColumnButtonInitialize, value); }
			remove { Events.RemoveHandler(EventCommandColumnButtonInitialize, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListVirtualModeCreateChildren"),
#endif
		Category("Virtual Mode")]
		public event TreeListVirtualModeCreateChildrenEventHandler VirtualModeCreateChildren
		{
			add { Events.AddHandler(EventVirtualModeCreateChildren, value); }
			remove { Events.RemoveHandler(EventVirtualModeCreateChildren, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListVirtualModeNodeCreating"),
#endif
		Category("Virtual Mode")]
		public event TreeListVirtualModeNodeCreatingEventHandler VirtualModeNodeCreating
		{
			add { Events.AddHandler(EventVirtualModeNodeCreating, value); }
			remove { Events.RemoveHandler(EventVirtualModeNodeCreating, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListVirtualModeNodeCreated"),
#endif
		Category("Virtual Mode")]
		public event TreeListVirtualNodeEventHandler VirtualModeNodeCreated
		{
			add { Events.AddHandler(EventVirtualModeNodeCreated, value); }
			remove { Events.RemoveHandler(EventVirtualModeNodeCreated, value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		#endregion
		#region Data
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListDataSource")]
#endif
		public override object DataSource {
			get { return base.DataSource; }
			set {
				if(value != DataSource) {
					this.requireApplyPostData = true;
					base.DataSource = value;
				}
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			InitialPageSize = SettingsPager.PageSize;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(!IsPostBack() || PostDataLoaded) 
				FinalizeLoading(false);
		}
		protected void FinalizeLoading(bool onLoadPostData) {
			if(onLoadPostData || PostDataLoaded)
				ReleasePendingChanges();
			if(!HasDataSource() && !IsVirtualMode())
				EndCreateNodes();
		}
		protected bool IsPostBack() {
			return Page != null && Page.IsPostBack;
		}
		protected internal bool HasDataSource() {
			return DataSource != null || !String.IsNullOrEmpty(DataSourceID);
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new HybridDataHelper(this, name);
		}
		protected virtual TreeListDataHelper CreateDataHelper() {
			return new TreeListDataHelper(this);
		}
		protected virtual TreeListRenderHelper CreateRenderHelper() {
			return new TreeListRenderHelper(this);
		}
		protected void BeginCreateNodes() {
			this.nodesCreated = false;
			this.nodesCreating = true;
		}
		protected void EndCreateNodes() {
			if(!IsVirtualMode())
				TreeDataHelper.AddFakeNewNode();
			this.nodesCreating = false;
			this.nodesCreated = true;
			DoSort();
			if(SettingsBehavior.AutoExpandAllNodes && !this.stateExists)
				ExpandAll();			
		}
		protected internal void EnsureNodesCreated() {
			EnsureNodesCreated(false);
		}
		protected internal void EnsureNodesCreated(bool allowFirstLoad) {
			if(NodesCreated || NodesCreating) return;
			if(IsVirtualMode()) {
				PerformVirtualLoad();
			} else {
				if(HasDataSource() && (IsPostBack() || allowFirstLoad))
					PerformSelect();
			}
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(IsVirtualMode() || !HasDataSource()) return; 
			ExtractFieldsInfo(data);
			BeginCreateNodes();
			try {
				IHierarchicalEnumerable hierData = data as IHierarchicalEnumerable;
				if(hierData != null)
					treeDataHelper.LoadHierarchicalData(hierData);
				else
					TreeDataHelper.LoadLinearData(data);
			} finally {
				EndCreateNodes();
			}			
		}
		protected void PerformVirtualLoad() {
			BeginCreateNodes();
			try {
				TreeDataHelper.LoadVirtualData();
			} finally {
				EndCreateNodes();
			}			
		}
		protected void ExtractFieldsInfo(IEnumerable data) {
			if(data == null) return;
			PropertyDescriptorCollection fields = TreeListUtils.GetDataSourceItemProperties(data);
			if(fields == null)
				return;
			foreach(PropertyDescriptor field in fields)
				TreeDataHelper.RegisterFieldType(field.Name, field.PropertyType);
			if(!AutoGenerateColumns || Columns.Count > 0)
				return;
			foreach(PropertyDescriptor field in fields) {
				string name = field.Name;
				if(!AutoGenerateServiceColumns && IsServiceFieldName(name))
					continue;
				TreeListDataColumn column = TreeListDataColumn.CreateInstance(field.PropertyType);
				column.FieldName = name;
				column.AutoGenerated = true;
				Columns.Add(column);
			}
		}
		protected internal bool IsServiceFieldName(string fieldName) {
			return fieldName == KeyFieldName || fieldName == ParentFieldName;
		}
		protected internal bool IsDataCacheEnabled() {
			if(HierarchyRequiresNodes())
				return false;
			if(DataCacheMode == TreeListDataCacheMode.Disabled)
				return false;
			if(DataCacheMode == TreeListDataCacheMode.Enabled)
				return true;
			if(DataSource != null)
				return false;
			if(SettingsPager.Mode == TreeListPagerMode.ShowPager)
				return true;
			int count = VisibleColumns.Count;
			if(count == 0 || TreeDataHelper.Rows.Count / count < 1000)
				return true;
			return false;
		}
		protected internal DataSourceView GetDataSourceView() {
			if(!HasDataSource())
				return null;
			return GetData();
		}
		protected internal Dictionary<string, object> GetTemplateEditValues() {
			Dictionary<string, object> result = null;
			foreach(IBindableTemplate template in GetBindingTemplates()) {
				foreach(Control container in RenderHelper.EditTemplateContainers) {
					IOrderedDictionary values = template.ExtractValues(container);
					if(values == null || values.Count < 1)
						continue;
					if(result == null)
						result = new Dictionary<string, object>();
					foreach(DictionaryEntry entry in values)
						result[entry.Key.ToString()] = entry.Value;
				}
			}
			return result;
		}
		List<IBindableTemplate> GetBindingTemplates() {
			IBindableTemplate template = null;
			List<IBindableTemplate> result = new List<IBindableTemplate>();
			template = Templates.EditForm as IBindableTemplate;
			if(template != null) result.Add(template);
			foreach(TreeListColumn column in Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn == null)
					continue;
				template = dataColumn.EditCellTemplate as IBindableTemplate;
				if(template == null)
					continue;
				result.Add(template);
			}
			return result;
		}
		void EnsureHasDataSource() {
			if(!HasDataSource())
				throw new NotSupportedException(StringResources.TreeList_UnboundModeInvalidOperation);
		}
		#endregion
		#region API
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListNodeCollection Nodes {
			get { return RootNode.ChildNodes; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PageIndex {
			get { return TreeDataHelper.PageIndex; }
			set { TreeDataHelper.PageIndex = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SortCount { 
			get { return SortedColumns.Count; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TotalNodeCount { 
			get {
				EnsureNodesCreated(); 
				return TreeDataHelper.GetRegisteredNodeCount();
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListNode FocusedNode {
			get {
				string key = TreeDataHelper.FocusedNodeKey;
				if(String.IsNullOrEmpty(key))
					return null;
				return FindNodeByKeyValue(key);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionCount {
			get { return TreeDataHelper.SelectionCount; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEditing { get { return TreeDataHelper.IsEditing; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EditingNodeKey { get { return TreeDataHelper.EditingKey; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsNewNodeEditing { get { return TreeDataHelper.IsNewNodeEditing; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string NewNodeParentKey { get { return TreeDataHelper.NewNodeParentKey; } }
		public bool IsVirtualMode() {
			return Events[EventVirtualModeCreateChildren] as TreeListVirtualModeCreateChildrenEventHandler != null;
		}
		public TreeListNode AppendNode(object keyObject) {
			return AppendNode(keyObject, null);
		}
		public TreeListNode AppendNode(object keyObject, TreeListNode parentNode) {
			TreeListNode node = TreeDataHelper.CreateNode(keyObject);
			node.DataItemInternal = new TreeListUnboundNodeDataItem(TreeDataHelper);
			if(parentNode == null)
				RootNode.AppendChild(node);
			else
				parentNode.AppendChild(node);
			TreeDataHelper.ResetVisibleData(); 
			return node;
		}
		public void RefreshVirtualTree() {
			RefreshVirtualTree(RootNode);
		}
		public void RefreshVirtualTree(TreeListNode startNode) {
			TreeDataHelper.LoadVirtualData(startNode);
		}
		public void ClearNodes() {
			TreeDataHelper.ClearNodes();
		}
		public object GetVirtualNodeObject(TreeListNode node) {
			TreeListVirtualNode virtualNode = node as TreeListVirtualNode;
			if(virtualNode == null) return null;
			return virtualNode.NodeObject;
		}
		public void ExpandAll() {
			ExpandAllCore(-1);
		}
		public void CollapseAll() {
			TreeDataHelper.ClearExpandedKeys();
		}
		public void ExpandToLevel(int level) {
			ExpandAllCore(level);
		}
		protected void ExpandAllCore(int maxLevel) {
			TreeListNodeIterator iterator = CreateNodeIterator();
			TreeListNode node;
			while((node = iterator.GetNext()) != null) {
				if(maxLevel > -1 && node.Level > maxLevel)
					continue;
				TreeListVirtualNode vNode = node as TreeListVirtualNode;
				if(node.HasChildren || vNode != null && !vNode.IsLeaf)
					node.Expanded = true;
			}
		}
		public void SelectAll() {
			if(!RenderHelper.IsSelectionEnabled)
				return;
			TreeListNodeIterator iterator = CreateNodeIterator();
			TreeListNode node;
			while((node = iterator.GetNext()) != null)
				node.Selected = true;				
		}
		public void UnselectAll() {
			TreeDataHelper.ClearSelectedKeys();
		}
		public List<TreeListNode> GetSelectedNodes() {
			List<TreeListNode> result = new List<TreeListNode>();
			if(RenderHelper.IsSelectionEnabled) {
				TreeListNodeIterator iterator = CreateNodeIterator();
				TreeListNode node;
				while((node = iterator.GetNext()) != null) {
					if(node.Selected)
						result.Add(node);
				}				
			}
			return result;
		}
		public List<TreeListNode> GetSelectedNodes(bool visibleOnly) {
			if(!visibleOnly)
				return GetSelectedNodes();
			List<TreeListNode> result = new List<TreeListNode>();
			foreach(TreeListRowInfo row in RenderHelper.Rows) {
				if(row.Selected)
					result.Add(TreeDataHelper.GetNodeByKey(row.NodeKey));
			}
			return result;
		}
		public TreeListNodeIterator CreateNodeIterator() {
			return new TreeListNodeIterator(RootNode);
		}
		public TreeListNodeIterator CreateNodeIterator(TreeListNode startNode) {
			if(startNode == null)
				throw new ArgumentNullException("startNode");
			return new TreeListNodeIterator(startNode);
		}
		public TreeListNode FindNodeByFieldValue(string fieldName, object value) {
			TreeListNodeIterator iterator = CreateNodeIterator();
			TreeListNode node;
			while((node = iterator.GetNext()) != null) {
				if(Object.Equals(node.GetValue(fieldName), value))
					return node;
			}
			return null;
		}
		public List<TreeListNode> FindNodesByFieldValue(string fieldName, object value) {
			List<TreeListNode> result = new List<TreeListNode>();
			TreeListNodeIterator iterator = CreateNodeIterator();
			TreeListNode node;
			while((node = iterator.GetNext()) != null) {
				if(Object.Equals(node.GetValue(fieldName), value))
					result.Add(node);
			}
			return result;
		}
		public TreeListNode FindNodeByKeyValue(string key) {
			if(key == null)
				throw new ArgumentNullException("key");				
			return TreeDataHelper.GetNodeByKey(key);
		}
		public List<TreeListNode> GetVisibleNodes() {
			List<TreeListNode> result = new List<TreeListNode>();
			foreach(TreeListRowInfo row in RenderHelper.Rows) {
				if(row.NodeKey == TreeListRenderHelper.NewNodeKey) continue;
				result.Add(TreeDataHelper.GetNodeByKey(row.NodeKey));
			}
			return result;
		}
		public ICollection<TreeListNode> GetAllNodes() {
			return TreeDataHelper.GetAllNodes();
		}
		public void ClearSort() {
			KillSortingInfo();
			TreeDataHelper.ResetVisibleData();
		}
		public int SortBy(TreeListDataColumn column, int sortIndex) {
			if(column == null)
				throw new ArgumentNullException("column");
			UpdateDataColumnSortIndex(column, sortIndex);			
			return column.SortIndex;
		}
		public ColumnSortOrder SortBy(TreeListDataColumn column, ColumnSortOrder sortOrder) {
			if(column == null)
				throw new ArgumentNullException("column");
			UpdateDataColumnSortOrder(column, sortOrder);			
			return column.SortOrder;
		}
		public object GetSummaryValue(TreeListNode node, TreeListSummaryItem item) {
			if(node == null)
				throw new ArgumentNullException("node");			
			return TreeDataHelper.GetSummaryValue(node.Key, item);
		}
		public Control FindHeaderCaptionTemplateControl(TreeListColumn column, string id) {
			return FindTemplateControl(TemplateHelper.HeaderCaptionContainers, column, null, id);			
		}
		public Control FindDataCellTemplateControl(string nodeKey, TreeListDataColumn column, string id) {
			return FindTemplateControl(TemplateHelper.DataCellContainers, column, nodeKey, id);			
		}
		public Control FindPreviewTemplateControl(string nodeKey, string id) {
			return FindTemplateControl(TemplateHelper.PreviewContainers, null, nodeKey, id);			
		}
		public Control FindGroupFooterTemplateControl(string nodeKey, TreeListColumn column, string id) {
			return FindTemplateControl(TemplateHelper.GroupFooterContainers, column, nodeKey, id);						
		}
		public Control FindFooterTemplateControl(TreeListColumn column, string id) {
			return FindTemplateControl(TemplateHelper.FooterContainers, column, null, id);			
		}
		[Obsolete("Use the FindEditCellTemplateControl method instead")]
		public Control FindInlineEditCellTemplateControl(TreeListDataColumn column, string id) {
			return FindEditCellTemplateControl(column, id);
		}
		public Control FindEditCellTemplateControl(TreeListDataColumn column, string id) {
			return FindTemplateControl(TemplateHelper.EditCellContainers, column, null, id);
		}
		public Control FindEditFormTemplateControl(string id) {
			return FindTemplateControl(TemplateHelper.EditFormContainers, null, null, id);			
		}
		Control FindTemplateControl(List<TreeListTemplateRegistration> containers, TreeListColumn column, string nodeKey, string id) {
			RenderUtils.EnsureChildControlsRecursive(this, false);
			return TreeListTemplateHelper.Find(containers, column, nodeKey, id);
		}
		public virtual byte[] SaveClientLayout() {
			return new TreeListClientLayoutHelper(this).SaveState();
		}
		public virtual bool LoadClientLayout(byte[] layoutData) {
			return new TreeListClientLayoutHelper(this).RestoreState(layoutData);
		}
		public void StartEdit(string nodeKey) {
			if(!SettingsDataSecurity.AllowEdit)
				return;
			TreeDataHelper.StartEdit(nodeKey);
			RecreateNodes();			
		}
		public void StartEditNewNode(string parentNodeKey) {
			if(!SettingsDataSecurity.AllowInsert)
				return;
			TreeDataHelper.StartEditNewNode(parentNodeKey);
			RecreateNodes();
			TreeDataHelper.EnsureNewNodeVisibility();
		}
		public void DoNodeValidation() {
			TreeDataHelper.DoNodeValidation();
		}
		public void UpdateEdit() {
			if(TreeDataHelper.CommitEdit()) {
				RecreateNodes();
			} else {
				if(TreeDataHelper.IsNewNodeEditing)
					TreeDataHelper.RefreshFakeNewNode();
				LayoutChanged();
		}
		}
		public void CancelEdit() {			
			TreeDataHelper.CancelEdit();
			RecreateNodes();
		}
		public void MoveNode(string nodeKey, string parentNodeKey) {
			EnsureHasDataSource();
			MoveNodeInternal(nodeKey, parentNodeKey);
		}
		public void DeleteNode(string nodeKey) {
			EnsureHasDataSource();
			DeleteNodeInternal(nodeKey);
		}
		protected internal void MoveNodeInternal(string nodeKey, string parentNodeKey) {
			if(!SettingsDataSecurity.AllowEdit)
				return;
			if(TreeDataHelper.MoveNode(nodeKey, parentNodeKey))
				RecreateNodes();
			else
				LayoutChanged();
		}
		protected internal virtual void DeleteNodeInternal(string nodeKey) {
			if(!SettingsDataSecurity.AllowDelete)
				return;
			TreeDataHelper.DeleteNode(nodeKey, SettingsEditing.AllowRecursiveDelete);
			RecreateNodes();
		}
		protected virtual void RecreateNodes() {
			if(IsVirtualMode()) {
				PerformVirtualLoad();
			} else if(HasDataSource()) {
				PerformSelect();
			} else {
				EndCreateNodes();
			}
		}
		#endregion
		protected internal TreeListUpdatableContainer GetUpdateableContainer() {
			EnsureChildControls();
			if(MainTable == null)
				return null;
			return MainTable.ContainerControl;
		}
		protected internal TreeListDataTable GetDataTable() {
			EnsureChildControls();
			TreeListUpdatableContainer container = GetUpdateableContainer();
			if(container == null)
				return null;
			return container.DataTable;
		}
		protected internal AppearanceStyleBase InternalCreateStyle(CreateStyleHandler handler, params object[] keys) {
			return base.CreateStyle(handler, keys);
		}
		protected internal bool InternalIsCallbacksEnabled() {
			return IsCallBacksEnabled();
		}
		protected internal InternalCheckBoxImageProperties GetCheckBoxImage(CheckState checkState) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch(checkState) {
				case CheckState.Checked:
					imageName = InternalCheckboxControl.CheckBoxCheckedImageName;
					result.MergeWith(ImagesEditors.CheckBoxChecked);
					break;
				case CheckState.Unchecked:
					imageName = InternalCheckboxControl.CheckBoxUncheckedImageName;
					result.MergeWith(ImagesEditors.CheckBoxUnchecked);
					break;
				default:
					imageName = InternalCheckboxControl.CheckBoxGrayedImageName;
					result.MergeWith(ImagesEditors.CheckBoxGrayed);
					break;
			}
			result.MergeWith(ImagesEditors.GetImageProperties(Page, imageName));
			ImagesEditors.UpdateSpriteUrl(result, Page, InternalCheckboxControl.WebSpriteControlName, typeof(ASPxWebControl), InternalCheckboxControl.DesignModeSpriteImagePath);
			return result;
		}
		protected List<InternalCheckBoxImageProperties> GetCheckBoxImages() {
			List<InternalCheckBoxImageProperties> result = new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] { GetCheckBoxImage(CheckState.Checked), 
				GetCheckBoxImage(CheckState.Unchecked), GetCheckBoxImage(CheckState.Indeterminate) });
			return result;
		}
		#region Event helpers
		protected internal void RaiseNodeExpanding(TreeListNodeCancelEventArgs e) {
			TreeListNodeCancelEventHandler handler = Events[EventNodeExpanding] as TreeListNodeCancelEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeExpanded(TreeListNodeEventArgs e) {
			TreeListNodeEventHandler handler = Events[EventNodeExpanded] as TreeListNodeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeCollapsing(TreeListNodeCancelEventArgs e) {
			TreeListNodeCancelEventHandler handler = Events[EventNodeCollapsing] as TreeListNodeCancelEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeCollapsed(TreeListNodeEventArgs e) {
			TreeListNodeEventHandler handler = Events[EventNodeCollapsed] as TreeListNodeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCustomDataCallback(TreeListCustomDataCallbackEventArgs e) {
			TreeListCustomDataCallbackEventHandler handler = Events[EventCustomDataCallback] as TreeListCustomDataCallbackEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseHtmlRowPrepared(TreeListDataRowBase row)  {
			TreeListHtmlRowEventHandler handler = Events[EventHtmlRowPrepared] as TreeListHtmlRowEventHandler;
			if(handler != null) {
				TreeListHtmlRowEventArgs e = new TreeListHtmlRowEventArgs(row);
				handler(this, e);
			}
		}
		protected internal void RaiseHtmlDataCellPrepared(TreeListDataCell cell) {
			TreeListHtmlDataCellEventHandler handler = Events[EventHtmlDataCellPrepared] as TreeListHtmlDataCellEventHandler;
			if(handler != null) {
				TreeListHtmlDataCellEventArgs e = new TreeListHtmlDataCellEventArgs(cell);
				handler(this, e);
			}
		}
		protected internal void RaiseSelectionChanged() {
			EventHandler handler = Events[EventSelectionChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseFocusedNodeChanged() {
			EventHandler handler = Events[EventFocusedNodeChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaisePageIndexChanged() {
			EventHandler handler = Events[EventPageIndexChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaisePageSizeChanged() {
			EventHandler handler = Events[EventPageSizeChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseVirtualModeCreateChildren(TreeListVirtualModeCreateChildrenEventArgs e) {
			TreeListVirtualModeCreateChildrenEventHandler handler = Events[EventVirtualModeCreateChildren] as TreeListVirtualModeCreateChildrenEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseVirtualModeNodeCreating(TreeListVirtualModeNodeCreatingEventArgs e) {
			TreeListVirtualModeNodeCreatingEventHandler handler = Events[EventVirtualModeNodeCreating] as TreeListVirtualModeNodeCreatingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseVirtualModeNodeCreated(TreeListNode node, object nodeObject) {
			TreeListVirtualNodeEventHandler handler = Events[EventVirtualModeNodeCreated] as TreeListVirtualNodeEventHandler;
			if(handler != null)
				handler(this, new TreeListVirtualNodeEventArgs(node, nodeObject));
		}
		protected internal void RaiseCustomSummaryCalculate(TreeListCustomSummaryEventArgs e) {
			TreeListCustomSummaryEventHandler handler = Events[EventCustomSummaryCalculate] as TreeListCustomSummaryEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCustomCallback(TreeListCustomCallbackEventArgs e) {
			TreeListCustomCallbackEventHandler handler = Events[EventCustomCallback] as TreeListCustomCallbackEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override IDictionary<string, object> GetCustomJSProperties() {
			TreeListCustomJSPropertiesEventArgs e = new TreeListCustomJSPropertiesEventArgs(JSPropertiesInternal);
			RaiseCustomJSProperties(e);
			if(e.Properties.Count > 0)
				return e.Properties;
			return null;
		}
		protected void RaiseCustomJSProperties(TreeListCustomJSPropertiesEventArgs e) {
			TreeListCustomJSPropertiesEventHandler handler = Events[EventCustomJsProperties] as TreeListCustomJSPropertiesEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseStartNodeEditing(TreeListNodeEditingEventArgs e) {
			TreeListNodeEditingEventHandler handler = Events[EventStartNodeEditing] as TreeListNodeEditingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCancelNodeEditing(TreeListNodeEditingEventArgs e) {
			TreeListNodeEditingEventHandler handler = Events[EventCancelNodeEditing] as TreeListNodeEditingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeUpdating(ASPxDataUpdatingEventArgs e) {
			ASPxDataUpdatingEventHandler handler = Events[EventNodeUpdating] as ASPxDataUpdatingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeUpdated(ASPxDataUpdatedEventArgs e) {
			ASPxDataUpdatedEventHandler handler = Events[EventNodeUpdated] as ASPxDataUpdatedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeDeleting(ASPxDataDeletingEventArgs e) {
			ASPxDataDeletingEventHandler handler = Events[EventNodeDeleting] as ASPxDataDeletingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeDeleted(ASPxDataDeletedEventArgs e) {
			ASPxDataDeletedEventHandler handler = Events[EventNodeDeleted] as ASPxDataDeletedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseInitNewNode(ASPxDataInitNewRowEventArgs e) {
			ASPxDataInitNewRowEventHandler handler = Events[EventInitNewNode] as ASPxDataInitNewRowEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeInserting(ASPxDataInsertingEventArgs e) {
			ASPxDataInsertingEventHandler handler = Events[EventNodeInserting] as ASPxDataInsertingEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeInserted(ASPxDataInsertedEventArgs e) {
			ASPxDataInsertedEventHandler handler = Events[EventNodeInserted] as ASPxDataInsertedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNodeValidating(TreeListNodeValidationEventArgs e) {
			TreeListNodeValidationEventHandler handler = Events[EventNodeValidating] as TreeListNodeValidationEventHandler;
			if(handler != null)
				handler(this, e);
		}		
		protected internal void RaiseCellEditorInitialize(TreeListColumnEditorEventArgs e) {
			TreeListColumnEditorEventHandler handler = Events[EventCellEditorInitialize] as TreeListColumnEditorEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseHtmlCommandCellPrepared(TreeListCommandCell cell) {
			TreeListHtmlCommandCellEventHandler handler = Events[EventHtmlCommandCellPrepared] as TreeListHtmlCommandCellEventHandler;
			if(handler != null)
				handler(this, new TreeListHtmlCommandCellEventArgs(cell));			
		}
		protected internal void RaiseProcessDragNode(TreeListNodeDragEventArgs e) {
			TreeListNodeDragEventHandler handler = Events[EventProcessDragNode] as TreeListNodeDragEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseParseValue(ASPxParseValueEventArgs e) {
			ASPxParseValueEventHandler handler = Events[EventParseValue] as ASPxParseValueEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCustomNodeSort(TreeListCustomNodeSortEventArgs e) {
			TreeListCustomNodeSortEventHandler handler = Events[EventCustomNodeSort] as TreeListCustomNodeSortEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseCustomErrorText(TreeListCustomErrorTextEventArgs e) {
			TreeListCustomErrorTextEventHandler handler = Events[EventCustomErrorText] as TreeListCustomErrorTextEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseEditingOperationCompleted(TreeListEditingOperation operation) {
			TreeListEditingOperationEventHandler handler = Events[EventEditingOperationCompleted] as TreeListEditingOperationEventHandler;
			if(handler == null) return;
			handler(this, new TreeListEditingOperationEventArgs(operation));
		}
		protected internal void RaiseCommandColumnButtonInitialize(TreeListCommandColumnButtonEventArgs e) {
			TreeListCommandColumnButtonEventHandler handler = Events[EventCommandColumnButtonInitialize] as TreeListCommandColumnButtonEventHandler;
			if(handler == null) return;
			handler(this, e);
		}
		protected internal bool IsHtmlRowPreparedEventAssigned() {
			return Events[EventHtmlRowPrepared] as TreeListHtmlRowEventHandler != null;				
		}		
		protected internal bool IsCustomNodeSortEventAssigned() {
			return Events[EventCustomNodeSort] as TreeListCustomNodeSortEventHandler != null;
		}
		#endregion
		#region Visible columns
		protected internal void ResetVisibleColumns() {
			WebColumnsOwnerImpl.ResetVisibleColumns();
		}
		#endregion
		#region Sorted columns, sorting
		protected void EnsureSortedColumns() {
			if(!this.sortedColumnsCreated)
				CreateSortedColumns();
		}
		protected void CreateSortedColumns() {
			this.sortedColumnsCreated = true;
			this.sortedColumns = GetSortableColumns();
			SortedColumns.Sort(CompareColumnsBySortIndex);
			FixSortedColumnsIndices();
		}
		int CompareColumnsBySortIndex(TreeListDataColumn x, TreeListDataColumn y) {
			return Comparer.Default.Compare(x.SortIndex, y.SortIndex);
		}
		protected internal List<TreeListDataColumn> GetSortableColumns() {
			List<TreeListDataColumn> result = new List<TreeListDataColumn>();
			foreach(TreeListColumn column in Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn == null) continue;
				if(dataColumn.SortIndex > -1 && dataColumn.SortOrder != ColumnSortOrder.None)
					result.Add(dataColumn);
			}
			return result;
		}
		protected internal void ResetSortedColumns() {
			this.sortedColumnsCreated = false;
		}
		protected internal void KillSortingInfo() {
			foreach(TreeListColumn col in Columns) {
				TreeListDataColumn dataColumn = col as TreeListDataColumn;
				if(dataColumn == null) continue;
				dataColumn.Unsort();
			}
			ResetSortedColumns();
		}
		protected internal void UpdateDataColumnSortOrder(TreeListDataColumn column, ColumnSortOrder value) {
			ResetSortedColumns();
			if(column.AllowSort == DefaultBoolean.False) {
				value = ColumnSortOrder.None;
			} else {
				int index = value == ColumnSortOrder.None ? -1 : GetSortableColumns().Count;
				column.SetSortIndex(index);
			}
			column.SetSortOrder(value);
			DoSort();
		}
		protected internal void UpdateDataColumnSortIndex(TreeListDataColumn column, int value) {
			ResetSortedColumns();
			if(column.AllowSort == DefaultBoolean.False) {
				value = -1;
			} else {
				if(value < -1)
					value = -1;
				if(value > -1) {
					if(column.SortOrder == ColumnSortOrder.None)
						column.SetSortOrder(ColumnSortOrder.Ascending);
					column.SetSortIndex(-1);
					foreach(TreeListDataColumn dataColumn in GetSortableColumns()) {						
						int oldIndex = dataColumn.SortIndex;
						if(oldIndex >= value)
							dataColumn.SetSortIndex(1 + oldIndex);
					}					
				}
			}
			column.SetSortIndex(value);
			DoSort();
		}
		protected void FixSortedColumnsIndices() {
			for(int i = 0; i < SortedColumns.Count; i++)
				SortedColumns[i].SetSortIndex(i);
		}
		protected void BeginSort() {
			this.sortLockCount++;
		}
		protected void EndSort() {
			this.sortLockCount--;
		}
		protected internal void DoSort() {
			if(Sorting)
				return;
			BeginSort();
			try {				
				DoSortSubtree(RootNode);
				TreeDataHelper.ResetVisibleData();
			} finally {
				EndSort();
			}
		}
		protected void DoSortCore(TreeListNodeCollection nodes, IComparer<TreeListNode> comparer) {
			nodes.Sort(comparer);
			foreach(TreeListNode node in nodes) {
				if(!node.HasChildren)
					continue;
				DoSortCore(node.ChildNodes, comparer);
			}
		}
		protected internal void DoSortSubtree(TreeListNode startNode) {			
			DoSortCore(startNode.ChildNodes, NodeComparer);
		}
		#endregion
		#region ASPxWebControl
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.editorRegistrator = null;			
			RenderHelper.EditTemplateContainers.Clear();
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			if(HierarchyRequiresNodes())
				EnsureNodesCreated(); 
			this.editorRegistrator = new TreeListEditorRegistrator();
			RenderHelper.PerformEditorRegistration();
			Controls.Add(EditorRegistrator);
			this.mainTable = new TreeListMainTable(RenderHelper);
			Controls.Add(MainTable);
			if(!DesignMode)
				Controls.Add(new TreeListServiceControl(RenderHelper));
			base.CreateControlHierarchy();
		}
		bool HierarchyRequiresNodes() {
			if(IsEditing && !IsNewNodeEditing && SettingsEditing.Mode != TreeListEditMode.Inline)
				return true;
			if(SettingsSelection.Enabled && SettingsSelection.Recursive)
				return true;
			return false;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TreeListClientSideEvents();
		}
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new TreeListSettingsLoadingPanel(this);
		}
		protected virtual TreeListSettings CreateSettings() {
			return new TreeListSettings(this);
		}
		protected virtual TreeListSettingsBehavior CreateBehaviorSettings() {
			return new TreeListSettingsBehavior(this);
		}
		protected virtual TreeListSettingsCustomizationWindow CreateCustomizationWindowSettings() {
			return new TreeListSettingsCustomizationWindow(this);
		}
		protected virtual TreeListSettingsSelection CreateSelectionSettings() {
			return new TreeListSettingsSelection(this);
		}
		protected virtual TreeListSettingsCookies CreateCookiesSettings() {
			return new TreeListSettingsCookies(this);
		}
		protected virtual TreeListSettingsEditing CreateEditingSettings() {
			return new TreeListSettingsEditing(this);
		}
		protected virtual TreeListSettingsPopupEditForm CreatePopupEditFormSettings() {
			return new TreeListSettingsPopupEditForm(this);
		}
		protected virtual TreeListSettingsDataSecurity CreateDataSecuritySettings() {
			return new TreeListSettingsDataSecurity(this);
		}
		protected virtual TreeListSettingsText CreateTextSettings() {
			return new TreeListSettingsText(this);
		}
		protected override ImagesBase CreateImages() {
			return new TreeListImages(this);
		}
		protected override StylesBase CreateStyles() {
			return new TreeListStyles(this);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override string GetSkinControlName() {
			return SkinControlName;
		}
		protected override string[] GetChildControlNames() {
			return ChildControlNames;
		}
		protected internal new void PropertyChanged(string name) {
			base.PropertyChanged(name);
		}
		[Browsable(false)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false)] 
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		protected internal override bool IsSwipeGesturesEnabled() {
			return base.IsSwipeGesturesEnabled() && !RenderHelper.HasHorizontalScrollBar;
		}
		protected internal bool IsSwipeGesturesEnabledInternal { get { return IsSwipeGesturesEnabled(); } }
		protected override bool HasLoadingDiv() {
			return RenderHelper.NeedRenderLoadingPanel;
		}
		protected override bool HasLoadingPanel() {
			return RenderHelper.NeedRenderLoadingPanel;
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxTreeList), DefaultCssName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxTreeList), SystemCssName);
		}
		public override void RegisterStyleSheets() {
			base.RegisterStyleSheets();
			if(EditorRegistrator != null) {
				foreach(Control control in EditorRegistrator.Controls) {
					ASPxEditBase editor = control as ASPxEditBase;
					if(editor != null)
						editor.RegisterStyleSheets();
				}
			}
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return true;
		}
		protected override bool IsStateSavedToCookies {
			get { return !DesignMode && SettingsCookies.Enabled; }
		}
		protected internal override string SaveClientState() {
			return Convert.ToBase64String(SaveClientLayout());
		}
		protected internal override void LoadClientState(string state) {
			LoadClientLayout(Convert.FromBase64String(state));
			ResetVisibleColumns();
		}
		protected override string GetStateCookieName() {
			return base.GetStateCookieName(SettingsCookies.CookiesID);
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDragAndDropUtilsScripts();
			RegisterIncludeScript(typeof(ASPxTreeList), ScriptName);
			if(RenderHelper.RequireFixedTableLayout) {
				RegisterTableScrollUtilsScript();
				RegisterIncludeScript(typeof(ASPxTreeList), ScrollUtilsScriptName);
			}
		}
		protected override bool HasFunctionalityScripts() {
			return IsEnabled();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTreeList";
		}
		protected internal virtual AppearanceStyleBase GetICBFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.CreateStyleByName(string.Empty, InternalCheckboxControl.FocusedCheckBoxClassName));
			style.CopyFrom(StylesEditors.CheckBoxFocused);
			return style;
		}
		protected internal virtual AppearanceStyleBase GetICBStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.CreateStyleByName(string.Empty, InternalCheckboxControl.CheckBoxClassName));
			style.CopyFrom(StylesEditors.CheckBox);
			return style;
		}
		protected virtual string GetICBSerializedFocusedStyle() {
			return InternalCheckboxControl.SerializeFocusedStyle(GetICBFocusedStyle(), this);
		}
		protected virtual string GetICBSerializedStyle() {
			return InternalCheckboxControl.SerializeFocusedStyle(GetICBStyle(), this);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.maxVisibleLevel = {1};\n", localVarName, TreeDataHelper.MaxVisibleLevel);
			stb.AppendFormat("{0}.visibleColumnCount = {1};\n", localVarName, VisibleColumns.Count);
			stb.AppendFormat("{0}.rowCount = {1};\n", localVarName, TreeDataHelper.Rows.Count);
			if(RenderHelper.IsFocusedNodeEnabled) {
				stb.AppendFormat("{0}.enableFocusedNode = true;\n", localVarName);
				if(SettingsBehavior.ProcessFocusedNodeChangedOnServer)
					stb.AppendFormat("{0}.focusSendsCallback = true;\n", localVarName);
			}
			if(!SettingsBehavior.FocusNodeOnExpandButtonClick)
				stb.AppendFormat("{0}.focusOnExpandCollapse = false;\n", localVarName);
			if(RenderHelper.IsSelectionEnabled || RenderHelper.IsSelectAllCheckVisible) {
				stb.AppendFormat("{0}.checkBoxImageProperties = {1}", localVarName, ImagePropertiesSerializer.GetImageProperties(GetCheckBoxImages(), this) + ";\n");
				stb.AppendFormat("{0}.icbFocusedStyle = {1};\n", localVarName, GetICBSerializedFocusedStyle());
			 }
			if(RenderHelper.IsSelectionEnabled) {
			 	if(RenderHelper.NeedProcessSelectionChangedOnServer) {
					stb.AppendFormat("{0}.selectionSendsCallback = true;\n", localVarName);
				}
				if(SettingsSelection.Recursive)
					stb.AppendFormat("{0}.recursiveSelection = true;\n", localVarName);
			}
			foreach(TreeListColumn column in Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				string columnName = dataColumn != null ? dataColumn.Name : "";
				string columnFieldName = dataColumn != null ? dataColumn.FieldName : "";
				bool canSort = dataColumn != null ? RenderHelper.IsColumnSortable(dataColumn) : false;
				stb.AppendFormat("{0}.CreateColumn({1},{2},{3},{4},{5},{6});\n", localVarName,
					column.Index, HtmlConvertor.ToScript(columnName), 
					HtmlConvertor.ToScript(columnFieldName), 
					HtmlConvertor.ToScript(canSort), 
					HtmlConvertor.ToScript(column.ShowInCustomizationForm),
					HtmlConvertor.ToScript(RenderHelper.GetColumnMinWidth(column))
				);
			}
			if(SettingsBehavior.ExpandCollapseAction != TreeListExpandCollapseAction.Button)
				stb.AppendFormat("{0}.expandCollapseAction = {1};\n", localVarName, (int)SettingsBehavior.ExpandCollapseAction);
			if(SettingsEditing.ConfirmDelete)
				stb.AppendFormat("{0}.confirmDeleteMsg = {1};\n", localVarName, HtmlConvertor.ToScript(SettingsText.ConfirmDelete));
			if(EditingNodeKey != null)
				stb.AppendFormat("{0}.editingKey = {1};\n", localVarName, HtmlConvertor.ToScript(EditingNodeKey));
			if(IsNewNodeEditing)
				stb.AppendFormat("{0}.isNewNodeEditing = true;\n", localVarName);
			if(RenderHelper.IsNodeDisplayedInEditMode)
				stb.AppendFormat("{0}.allowStylizeEditingNode = true;\n", localVarName);
			if(Settings.ShowRoot)
				stb.AppendFormat("{0}.showRoot = true;\n", localVarName);
			if(KeyboardSupport) {
				stb.AppendFormat("{0}.enableKeyboard = true;\n", localVarName);
				if(!String.IsNullOrEmpty(AccessKey))
					stb.AppendFormat("{0}.accessKey = {1};\n", localVarName, HtmlConvertor.ToScript(AccessKey));
			}
			if(SettingsPager.Mode != TreeListPagerMode.ShowAllNodes) {
				stb.AppendFormat("{0}.pageIndex={1};\n", localVarName, TreeDataHelper.PageIndex);
				stb.AppendFormat("{0}.pageSize={1};\n", localVarName, TreeDataHelper.PageSize);
				stb.AppendFormat("{0}.pageCount={1};\n", localVarName, TreeDataHelper.PageCount);
			}
			if(RenderHelper.HasHorizontalScrollBar)
				stb.AppendFormat("{0}.horzScroll = {1};\n", localVarName, (int)RenderHelper.HorizontalScrollBarMode);
			if(RenderHelper.HasVerticalScrollBar)
				stb.AppendFormat("{0}.vertScroll = {1};\n", localVarName, (int)RenderHelper.VerticalScrollBarMode);
			if(RenderHelper.AllowColumnResizing)
				stb.AppendFormat("{0}.columnResizeMode = {1};\n", localVarName, (int)RenderHelper.ColumnResizeMode);
			if(CommandButtonsHelper.HasButtons)
				stb.AppendFormat("{0}.cButtonIDs = {1};\n", localVarName, HtmlConvertor.ToJSON(CommandButtonsHelper.GetClientIDs()));
			if(SettingsBehavior.AllowEllipsisInText)
				stb.AppendFormat("{0}.enableEllipsis = true;\n", localVarName);
		}
		protected internal Hashtable GetClientObjectStateForCallback() {
			return GetClientObjectState();
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ClientStateProperties.CallbackState, GetCallbackStateString());
			if(!IsCallback) {
				if(RenderHelper.IsFocusedNodeEnabled)
					result.Add(TreeListClientStateProperties.FocusedKey, TreeDataHelper.FocusedNodeKey);
				if(RenderHelper.IsSelectionEnabled)
					result.Add(TreeListClientStateProperties.Selection, "");
				if(RenderHelper.IsSelectAllCheckVisible)
					result.Add(TreeListClientStateProperties.SelectAllMark, "");
				if(RenderHelper.HasScrolling)
					result.Add(TreeListClientStateProperties.ScrollState, GetClientObjectStateValue<ArrayList>(TreeListClientStateProperties.ScrollState));
				if(RenderHelper.AllowColumnResizing)
					result.Add(TreeListClientStateProperties.ResizingState, "");
				result.Add(TreeListClientStateProperties.EditorValues, "");
			}
			return result;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			list.Add(Settings);
			list.Add(SettingsBehavior);
			list.Add(SettingsPager);
			list.Add(SettingsCustomizationWindow);
			list.Add(SettingsSelection);
			list.Add(SettingsCookies);
			list.Add(SettingsEditing);
			list.Add(SettingsText);
			list.Add(SettingsPopupEditForm);
			list.Add(SettingsDataSecurity);
			list.Add(Summary);
			list.Add(StylesPager);
			list.Add(StylesEditors);
			list.Add(ImagesEditors);
			return list.ToArray();
		}
		#endregion
		protected internal override void ResetControlHierarchy() {
			base.ResetControlHierarchy();
			if(this.templateHelper != null)
				TemplateHelper.Reset();
			if(this.commandButtonsHelper != null)
				CommandButtonsHelper.Invalidate();
		}
		#region PostBacks / Callbacks
		protected internal bool IsPartialCallbackPossible() {
			if(SettingsBehavior.DisablePartialUpdate) 
				return false;
			if(SettingsPager.Mode != TreeListPagerMode.ShowAllNodes) 
				return false;
			if(RenderHelper.NeedRenderCustomizationWindow) 
				return false;
			if(RenderHelper.IsAlternatingNodeStyleEnabled) 
				return false;
			if(RenderHelper.NeedRenderStyleTable && RenderHelper.NeedExactStyleTable) 
				return false;
			if(RenderHelper.IsSelectionEnabled) 
				return false;
			return true;
		}
		protected override bool CanLoadPostDataOnCreateControls() { return true; }
		protected override bool CanLoadPostDataOnLoad() { return false; }
		protected override bool LoadPostData(NameValueCollection postCollection) {
			ApplyPostData();
			return true;
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			this.requireApplyPostData = true;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			RaisePostBackEventCore(eventArgument);			
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			ApplyPostData();
			ResetControlHierarchy();
			RaisePostBackEventCore(eventArgument);
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			BeginRendering();
			try {
				return CommandToExecute.GetCallbackResult();
			} finally {
				EndRendering();
			}
		}
		protected override string OnCallbackException(Exception e) {
			TreeListCustomErrorTextEventArgs args = new TreeListCustomErrorTextEventArgs(e, base.OnCallbackException(e));
			RaiseCustomErrorText(args);
			return args.ErrorText;
		}
		protected void RaisePostBackEventCore(string arg) {
			int separatorPos = arg.IndexOf(TreeListRenderHelper.SeparatorToken);
			TreeListCommandId cmdId;
			try {
				cmdId = (TreeListCommandId)Int32.Parse(separatorPos < 0 ? arg : arg.Substring(0, separatorPos));
			} catch {
				throw new ArgumentException();
			}
			string cmdArgs = arg.Substring(1 + separatorPos);
			CommandToExecute = TreeListCommandFactory.CreateInstance(this, cmdId, cmdArgs);
			if(CommandToExecute == null)
				throw new ArgumentException("Unknown command");
			new TreeListEditorValuesReader(this).Read(this.editorValuesState, CanIgnoreInvalidEditorValues());
			CommandToExecute.Execute();
		}
		protected void ReleasePendingChanges() {
			if(this.pendingChangesReleased)
				return; 
			if(HasPendingChange(TreeListPendingChange.FocusedNode))
				RaiseFocusedNodeChanged();
			if(HasPendingChange(TreeListPendingChange.Selection))
				RaiseSelectionChanged();
			PendingChange = TreeListPendingChange.Empty;
			this.pendingChangesReleased = true;
		}
		protected internal string GetCallbackStateString() {
			return TreeDataHelper.GetStateString(IsDataCacheEnabled(), TreeListRenderHelper.UseStateCompression);
		}
		void ApplyPostData() {
			if(!this.requireApplyPostData) return;
			if(ClientObjectState == null) return;
			ApplyCallbackState(GetClientObjectStateValue<string>(ClientStateProperties.CallbackState));
			if(RenderHelper.AllowColumnResizing)
				ApplyColumnResizingClientState(GetClientObjectStateValue<string>(TreeListClientStateProperties.ResizingState));
			if(IsVirtualMode() && NodesCreated)
				RefreshVirtualTree();
			if(IsNewNodeEditing && NodesCreated && !IsVirtualMode())
				TreeDataHelper.RefreshFakeNewNode();
			if(RenderHelper.IsFocusedNodeEnabled)
				ApplyFocusedKey(GetClientObjectStateValue<string>(TreeListClientStateProperties.FocusedKey));
			if(RenderHelper.IsSelectionEnabled)
				ApplySelectionResult(GetClientObjectStateValue<string>(TreeListClientStateProperties.Selection), GetClientObjectStateValue<string>(TreeListClientStateProperties.SelectAllMark));
			this.editorValuesState = GetClientObjectStateValue<string>(TreeListClientStateProperties.EditorValues);
			this.requireApplyPostData = false;
			if(Loaded)
				FinalizeLoading(true);
		}
		void ApplyCallbackState(string state) {
			if(String.IsNullOrEmpty(state)) return;
			this.stateExists = true;
			TreeDataHelper.SetStateString(state, TreeListRenderHelper.UseStateCompression);			
		}
		void ApplyColumnResizingClientState(string state) {
			if(String.IsNullOrEmpty(state)) return;
			foreach(DictionaryEntry entry in HtmlConvertor.FromJSON<IDictionary>(state)) {
				if(entry.Key.ToString() == "ctrlWidth")
					continue;
				Columns[Convert.ToInt32(entry.Key)].Width = Unit.Parse(entry.Value.ToString());
			}
		}
		void ApplyFocusedKey(string value) {
			if(value == TreeDataHelper.FocusedNodeKey) return;
			if(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(TreeDataHelper.FocusedNodeKey)) return;
			TreeDataHelper.FocusedNodeKey = value;
			PendingChange |= TreeListPendingChange.FocusedNode;
		}
		void ApplySelectionResult(string value, string selectAllMark) {
			if(!String.IsNullOrEmpty(selectAllMark)) {
				if(selectAllMark == "A" && !RootNode.Selected) {
					TreeDataHelper.SetNodeSelected(TreeListRenderHelper.RootNodeKey, true);
					PendingChange |= TreeListPendingChange.Selection;
				}
				if(selectAllMark == "N" && RootNode.Selected) {
					TreeDataHelper.SetNodeSelected(TreeListRenderHelper.RootNodeKey, false);
					PendingChange |= TreeListPendingChange.Selection;
				}
				return;
			}
			if(String.IsNullOrEmpty(value))
				return;
			List<string> keys = new List<string>();
			foreach(string rawKey in value.Split(TreeListRenderHelper.SeparatorToken))
				keys.Add(TreeListUtils.UnescapeNodeKey(rawKey));			
			if(keys.Count < 1)
				return;
			Dictionary<string, bool> currentStates = new Dictionary<string, bool>(keys.Count);
			foreach(string key in keys)
				currentStates[key] = TreeDataHelper.IsNodeSelected(key);
			foreach(string key in keys) {
				if(TreeDataHelper.IsNodeSelected(key) != currentStates[key])
					continue;
				TreeDataHelper.ToggleNodeSelection(key);
				currentStates[key] = TreeDataHelper.IsNodeSelected(key);
			}
			PendingChange |= TreeListPendingChange.Selection;
		}
		bool HasPendingChange(TreeListPendingChange change) {
			return (PendingChange & change) == change;
		}
		bool CanIgnoreInvalidEditorValues() {		
			if(!IsEditing)
				return false;
			if(!EnableCallbacks)
				return true;
			return CommandToExecute is TreeListCancelEditCommand
				|| CommandToExecute is TreeListStartEditCommand				
				|| CommandToExecute is TreeListCustomDataCallbackCommand;
		}
		#endregion		
		#region IWebColumnsOwner
		WebColumnCollectionBase IWebColumnsOwner.Columns { get { return WebColumnsOwnerImpl.Columns; } }
		List<WebColumnBase> IWebColumnsOwner.GetVisibleColumns() { return WebColumnsOwnerImpl.GetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleColumns() { WebColumnsOwnerImpl.ResetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleIndices() { WebColumnsOwnerImpl.ResetVisibleIndices(); }
		void IWebColumnsOwner.EnsureVisibleIndices() { WebColumnsOwnerImpl.EnsureVisibleIndices(); }
		void IWebColumnsOwner.SetColumnVisible(WebColumnBase column, bool value) { WebColumnsOwnerImpl.SetColumnVisible(column, value); }
		void IWebColumnsOwner.SetColumnVisibleIndex(WebColumnBase column, int value) { WebColumnsOwnerImpl.SetColumnVisibleIndex(column, value); }
		void IWebColumnsOwner.OnColumnChanged(WebColumnBase column) { 
			WebColumnsOwnerImpl.OnColumnChanged(column);
			ResetSortedColumns();
			TreeDataHelper.ResetVisibleData();
		}
		void IWebColumnsOwner.OnColumnCollectionChanged() { 
			WebColumnsOwnerImpl.OnColumnCollectionChanged();
			ResetSortedColumns();
			TreeDataHelper.ResetVisibleData();
		}
		#endregion
		int IPagerOwner.InitialPageSize {
			get { return InitialPageSize; }
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxTreeList.Design.TreeListCommonFormDesigner"; } }
	}
}
namespace DevExpress.Web.ASPxTreeList.Internal {
	public static class TreeListClientStateProperties {
		public const string
			FocusedKey = "focusedKey",
			Selection = "selection",
			SelectAllMark = "selectAllMark",
			ScrollState = "scrollState",
			ResizingState = "resizingState",
			EditorValues = "editorValues";
	}
}
