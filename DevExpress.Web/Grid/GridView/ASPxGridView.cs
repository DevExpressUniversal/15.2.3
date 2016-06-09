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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.IO;
using DevExpress.Data.Summary;
using DevExpress.Utils;
using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Design;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxGridView"),
	Designer("DevExpress.Web.Design.GridViewDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGridView.bmp")
]
	public class ASPxGridView : ASPxGridBase, ISummaryItemsOwner {
		public const string
			GridViewScriptResourceName = GridScriptResourcePath + "GridView.js",
			GridViewTableColumnResizingResourceName = GridScriptResourcePath + "TableColumnResizing.js",
			GridViewResourceImagePath = "DevExpress.Web.Images.GridView.",
			GridViewCssResourcePath = "DevExpress.Web.Css.GridView.",
			GridViewDefaultCssResourceName = GridViewCssResourcePath + "default.css",
			GridViewSpriteCssResourceName = GridViewCssResourcePath + "sprite.css",
			SkinControlName = "GridView";
		public const int InvalidRowIndex = DataController.InvalidRow;
		static readonly object 
			customColumnDisplayText = new object(),
			customUnboundColumnData = new object(),
			beforeHeaderFilterFillItems = new object(),
			headerFilterFillItems = new object(),
			commandButtonInitialize = new object(),
			customButtonInitialize = new object(),
			cellEditorInitialize = new object(),
			searchPanelEditorCreate = new object(),
			searchPanelEditorInitialize = new object(),
			beforeColumnSortingGrouping = new object(),
			customCallback = new object(),
			customDataCallback = new object(),
			customButtonCallback = new object(),
			afterPerformCallback = new object(),
			customColumnSort = new object(),
			customErrorText = new object(),
			substituteFilter = new object(),
			substituteSortInfo = new object(),
			pageIndexChanged = new object(),
			pageSizeChanged = new object(),
			focusedRowChanged = new object(),
			selectionChanged = new object(),
			startRowEditing = new object(),
			cancelRowEditing = new object(),
			customFilterExpressionDisplayText = new object(),
			rowInserting = new object(),
			rowInserted = new object(),
			rowUpdating = new object(),
			rowUpdated = new object(),
			rowDeleting = new object(),
			rowDeleted = new object(),
			parseValue = new object(),
			initNewRow = new object(),
			rowValidating = new object(),
			htmlRowCreated = new object(),
			htmlRowPrepared = new object(),
			htmlDataCellPrepared = new object(),
			htmlCommandCellPrepared = new object(),
			rowCommand = new object(),
			autoFilterCellEditorCreate = new object(),
			autoFilterCellEditorInitialize = new object(),
			summaryDisplayText = new object(),
			detailRowGetButtonVisibility = new object(),
			processColumnAutoFilter = new object(),
			processMultiColumnAutoFilter = new object(),
			beforePerformDataSelect = new object(),
			customColumnGroup = new object(),
			htmlEditFormCreated = new object(),
			htmlFooterCellPrepared = new object(),
			detailsChanged = new object(),
			detailRowExpandedChanged = new object(),
			fillContextMenuItems = new object(),
			contextMenuItemVisibility = new object(),
			contextMenuItemClick = new object(),
			addSummaryItemViaContextMenu = new object(),
			batchUpdate = new object(),
			customSummaryCalculate = new object(),
			customGroupDisplayText = new object();
		public static object GetDetailRowKeyValue(Control control) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			return container != null ? container.KeyValue : null;
		}
		public static object GetDetailRowValues(Control control, params string[] fieldNames) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			if(container == null) return null;
			return container.Grid.GetRowValues(container.VisibleIndex, fieldNames);
		}
		protected static ASPxGridView FindParentGrid(Control control) {
			GridViewBaseRowTemplateContainer container = FindParentGridTemplateContainer(control);
			return container != null ? container.Grid : null;
		}
		public static GridViewBaseRowTemplateContainer FindParentGridTemplateContainer(Control control) {
			if(control == null) return null;
			GridViewBaseRowTemplateContainer container = null;
			while(control.Parent != null) {
				container = control.Parent as GridViewBaseRowTemplateContainer;
				if(container != null) break;
				control = control.Parent;
			}
			return container;
		}
		public ASPxGridView()
			: base() {
			TotalSummary.SummaryChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			GroupSummary.SummaryChanged += new CollectionChangeEventHandler(OnSummaryChanged);
			GroupSummarySortInfo.SummaryChanged += new CollectionChangeEventHandler(OnGroupSummaryChanged);
			SettingsDetail = CreateSettingsDetail();
			SettingsContextMenu = new ASPxGridViewContextMenuSettings(this);
			SettingsAdaptivity = new ASPxGridViewAdaptivitySettings(this);
			Templates = new GridViewTemplates(this);
			StylesContextMenu = new GridViewContextMenuStyles(this);
			SettingsCustomizationWindowInternal = new ASPxGridViewCustomizationWindowSettings(this); 
		}
		protected override string GetSkinControlName() { return SkinControlName; }
		protected override string DefaultCssResourceName { get { return GridViewDefaultCssResourceName; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewClientInstanceName"),
#endif
 Category("Client-Side"), AutoFormatDisable, DefaultValue(""), Localizable(false)]
		public string ClientInstanceName { get { return base.ClientInstanceNameInternal; } set { base.ClientInstanceNameInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewClientVisible"),
#endif
 Category("Client-Side"), AutoFormatDisable, DefaultValue(true), Localizable(false)]
		public new bool ClientVisible { get { return base.ClientVisible; } set { base.ClientVisible = value; } }
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor)), 
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable, MergableProperty(false)]
		public new GridViewClientSideEvents ClientSideEvents { get { return base.ClientSideEvents as GridViewClientSideEvents; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewJSProperties"),
#endif
 Category("Client-Side"), Browsable(false), AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties { get { return JSPropertiesInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnableCallBacks"),
#endif
 DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public new bool EnableCallBacks { get { return base.EnableCallBacks; } set { base.EnableCallBacks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnableCallbackAnimation"),
#endif
 Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public new bool EnableCallbackAnimation { get { return base.EnableCallbackAnimation; } set { base.EnableCallbackAnimation = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnablePagingCallbackAnimation"),
#endif
 Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public new bool EnablePagingCallbackAnimation { get { return base.EnablePagingCallbackAnimation; } set { base.EnablePagingCallbackAnimation = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnablePagingGestures"),
#endif
 Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public new AutoBoolean EnablePagingGestures { get { return base.EnablePagingGestures; } set { base.EnablePagingGestures = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnableCallbackCompression"),
#endif
 Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public new bool EnableCallbackCompression { get { return base.EnableCallbackCompression; } set { base.EnableCallbackCompression = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsDetail"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewDetailSettings SettingsDetail { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsContextMenu"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewContextMenuSettings SettingsContextMenu { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsAdaptivity"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewAdaptivitySettings SettingsAdaptivity { get; private set; }
		[Browsable(false), AutoFormatEnable, Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewTemplates Templates { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStylesContextMenu"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyles StylesContextMenu { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsPager"),
#endif
 Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewPagerSettings SettingsPager { get { return base.SettingsPager as ASPxGridViewPagerSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsEditing"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewEditingSettings SettingsEditing { get { return base.SettingsEditing as ASPxGridViewEditingSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettings"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewSettings Settings { get { return base.Settings as ASPxGridViewSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsBehavior"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewBehaviorSettings SettingsBehavior { get { return base.SettingsBehavior as ASPxGridViewBehaviorSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsCookies"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewCookiesSettings SettingsCookies { get { return base.SettingsCookies as ASPxGridViewCookiesSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsCommandButton"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewCommandButtonSettings SettingsCommandButton { get { return base.SettingsCommandButton as ASPxGridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsDataSecurity"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewDataSecuritySettings SettingsDataSecurity { get { return base.SettingsDataSecurity as ASPxGridViewDataSecuritySettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsPopup"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewPopupControlSettings SettingsPopup { get { return base.SettingsPopup as ASPxGridViewPopupControlSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsSearchPanel"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewSearchPanelSettings SettingsSearchPanel { get { return base.SettingsSearchPanel as ASPxGridViewSearchPanelSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsFilterControl"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewFilterControlSettings SettingsFilterControl { get { return base.SettingsFilterControl as ASPxGridViewFilterControlSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsLoadingPanel"),
#endif
 Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return (ASPxGridViewLoadingPanelSettings)base.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsText"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxGridViewTextSettings SettingsText { get { return base.SettingsText as ASPxGridViewTextSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStylesPopup"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPopupControlStyles StylesPopup { get { return base.StylesPopup as GridViewPopupControlStyles; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAccessibilityCompliant"),
#endif
 Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSummaryText"),
#endif
 Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable, Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SummaryText {
			get { return GetStringProperty("SummaryText", ""); }
			set {
				if(value == SummaryText)
					return;
				if(value != null)
					value = value.Replace('\n', ' ').Replace("\r", "");
				SetStringProperty("SummaryText", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCaption"),
#endif
 Category("Accessibility"), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public new string Caption { get { return base.Caption; } set { base.Caption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewKeyboardSupport"),
#endif
 Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public new bool KeyboardSupport { get { return base.KeyboardSupport; } set { base.KeyboardSupport = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRightToLeft"),
#endif
 Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public new DefaultBoolean RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Settings"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue(null), AutoFormatDisable]
		public new GridViewFormLayoutProperties EditFormLayoutProperties { get { return (GridViewFormLayoutProperties)base.EditFormLayoutProperties; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewImagesEditors"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewEditorImages ImagesEditors { get { return base.ImagesEditors as GridViewEditorImages; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStylesEditors"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewEditorStyles StylesEditors { get { return base.StylesEditors as GridViewEditorStyles; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStylesPager"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPagerStyles StylesPager { get { return base.StylesPager as GridViewPagerStyles; } }
		#region Obsolete
		[Obsolete("Use the SettingsPopup.CustomizationWindow property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsCustomizationWindow"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow { get { return SettingsCustomizationWindowInternal; } }
		protected internal virtual ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindowInternal { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("This method is deprecated and should not be used")]
		public List<GridViewColumn> GetColumnsShownInHeaders() { return ColumnHelper.Leafs.ConvertAll<GridViewColumn>((n) => { return (n as GridViewColumnVisualTreeNode).Column; }); }
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDataSourceForceStandardPaging"),
#endif
 DefaultValue(false), AutoFormatDisable]
		public new bool DataSourceForceStandardPaging { get { return base.DataSourceForceStandardPaging; } set { base.DataSourceForceStandardPaging = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewKeyFieldName"),
#endif
 DefaultValue(""), Category("Data"), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable]
		public new string KeyFieldName { get { return base.KeyFieldName; } set { base.KeyFieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPreviewFieldName"),
#endif
 DefaultValue(""), Category("Data"), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable]
		public new string PreviewFieldName { get { return base.PreviewFieldName; } set { base.PreviewFieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAutoGenerateColumns"),
#endif
 DefaultValue(true), Category("Data"), AutoFormatDisable]
		public new bool AutoGenerateColumns { get { return base.AutoGenerateColumns; } set { base.AutoGenerateColumns = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEnableRowsCache"),
#endif
 DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public new bool EnableRowsCache { get { return base.EnableRowsCache; } set { base.EnableRowsCache = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewColumns"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public new GridViewColumnCollection Columns { get { return base.Columns as GridViewColumnCollection; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTotalSummary"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new ASPxSummaryItemCollection TotalSummary { get { return base.TotalSummary as ASPxSummaryItemCollection; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxGroupSummarySortInfoCollection GroupSummarySortInfo { get { return DataProxy.GroupSummarySortInfo; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewGroupSummary"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public ASPxSummaryItemCollection GroupSummary { get { return DataProxy.GroupSummary as ASPxSummaryItemCollection; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new GridViewFormatConditionCollection FormatConditions { get { return (GridViewFormatConditionCollection)base.FormatConditions; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStyles"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewStyles Styles { get { return base.Styles as GridViewStyles; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewImages"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewImages Images { get { return (GridViewImages)base.Images; } }
		[Browsable(false)]
		public int FixedColumnCount { get { return ColumnHelper.FixedColumns.Count; } }
		[Browsable(false)]
		public int GroupCount { get { return SortedColumns.Where(c => c.Adapter.GroupIndex >= 0).Count(); } }
		[Browsable(false)]
		public WebDataDetailRows DetailRows { get { return DataProxy.DetailRows; } }
		[Browsable(false)]
		public ReadOnlyGridColumnCollection<GridViewColumn> AllColumns { get { return ColumnHelper.AllColumns; } }
		[Browsable(false)]
		public ReadOnlyGridColumnCollection<GridViewDataColumn> DataColumns { get { return ColumnHelper.AllDataColumns; } }
		[Browsable(false)]
		public ReadOnlyGridColumnCollection<GridViewColumn> VisibleColumns { get { return ColumnHelper.AllVisibleColumns; } }
		#region Events
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomColumnDisplayText"),
#endif
 Category("Rendering")]
		public event ASPxGridViewColumnDisplayTextEventHandler CustomColumnDisplayText { add { Events.AddHandler(customColumnDisplayText, value); } remove { Events.RemoveHandler(customColumnDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomUnboundColumnData"),
#endif
 Category("Data")]
		public event ASPxGridViewColumnDataEventHandler CustomUnboundColumnData { add { Events.AddHandler(customUnboundColumnData, value); } remove { Events.RemoveHandler(customUnboundColumnData, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBeforeHeaderFilterFillItems"),
#endif
 Category("Data")]
		public event ASPxGridViewBeforeHeaderFilterFillItemsEventHandler BeforeHeaderFilterFillItems { add { Events.AddHandler(beforeHeaderFilterFillItems, value); } remove { Events.RemoveHandler(beforeHeaderFilterFillItems, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHeaderFilterFillItems"),
#endif
 Category("Data")]
		public event ASPxGridViewHeaderFilterEventHandler HeaderFilterFillItems { add { Events.AddHandler(headerFilterFillItems, value); } remove { Events.RemoveHandler(headerFilterFillItems, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonInitialize"),
#endif
 Category("Rendering")]
		public event ASPxGridViewCommandButtonEventHandler CommandButtonInitialize { add { Events.AddHandler(commandButtonInitialize, value); } remove { Events.RemoveHandler(commandButtonInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomButtonInitialize"),
#endif
 Category("Rendering")]
		public event ASPxGridViewCustomButtonEventHandler CustomButtonInitialize { add { Events.AddHandler(customButtonInitialize, value); } remove { Events.RemoveHandler(customButtonInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCellEditorInitialize"),
#endif
 Category("Rendering")]
		public event ASPxGridViewEditorEventHandler CellEditorInitialize { add { Events.AddHandler(cellEditorInitialize, value); } remove { Events.RemoveHandler(cellEditorInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelEditorCreate"),
#endif
 Category("Rendering")]
		public event ASPxGridViewSearchPanelEditorCreateEventHandler SearchPanelEditorCreate { add { Events.AddHandler(searchPanelEditorCreate, value); } remove { Events.RemoveHandler(searchPanelEditorCreate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelEditorInitialize"),
#endif
 Category("Rendering")]
		public event ASPxGridViewSearchPanelEditorEventHandler SearchPanelEditorInitialize { add { Events.AddHandler(searchPanelEditorInitialize, value); } remove { Events.RemoveHandler(searchPanelEditorInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBeforeColumnSortingGrouping"),
#endif
 Category("Data")]
		public event ASPxGridViewBeforeColumnGroupingSortingEventHandler BeforeColumnSortingGrouping { add { Events.AddHandler(beforeColumnSortingGrouping, value); } remove { Events.RemoveHandler(beforeColumnSortingGrouping, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomCallback"),
#endif
 Category("Action")]
		public event ASPxGridViewCustomCallbackEventHandler CustomCallback { add { Events.AddHandler(customCallback, value); } remove { Events.RemoveHandler(customCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomDataCallback"),
#endif
 Category("Data")]
		public event ASPxGridViewCustomDataCallbackEventHandler CustomDataCallback { add { Events.AddHandler(customDataCallback, value); } remove { Events.RemoveHandler(customDataCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomButtonCallback"),
#endif
 Category("Action")]
		public event ASPxGridViewCustomButtonCallbackEventHandler CustomButtonCallback { add { Events.AddHandler(customButtonCallback, value); } remove { Events.RemoveHandler(customButtonCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAfterPerformCallback"),
#endif
 Category("Action")]
		public event ASPxGridViewAfterPerformCallbackEventHandler AfterPerformCallback { add { Events.AddHandler(afterPerformCallback, value); } remove { Events.RemoveHandler(afterPerformCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomColumnSort"),
#endif
 Category("Data")]
		public event ASPxGridViewCustomColumnSortEventHandler CustomColumnSort { add { Events.AddHandler(customColumnSort, value); } remove { Events.RemoveHandler(customColumnSort, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomColumnGroup"),
#endif
 Category("Data")]
		public event ASPxGridViewCustomColumnSortEventHandler CustomColumnGroup { add { Events.AddHandler(customColumnGroup, value); } remove { Events.RemoveHandler(customColumnGroup, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomJSProperties"),
#endif
 Category("Client-Side")]
		public event ASPxGridViewClientJSPropertiesEventHandler CustomJSProperties { add { Events.AddHandler(EventCustomJsProperties, value); } remove { Events.RemoveHandler(EventCustomJsProperties, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomErrorText"),
#endif
 Category("Rendering")]
		public event ASPxGridViewCustomErrorTextEventHandler CustomErrorText { add { Events.AddHandler(customErrorText, value); } remove { Events.RemoveHandler(customErrorText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSubstituteFilter"),
#endif
 Category("Data")]
		public event EventHandler<SubstituteFilterEventArgs> SubstituteFilter { add { Events.AddHandler(substituteFilter, value); } remove { Events.RemoveHandler(substituteFilter, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSubstituteSortInfo"),
#endif
 Category("Data")]
		public event EventHandler<SubstituteSortInfoEventArgs> SubstituteSortInfo { add { Events.AddHandler(substituteSortInfo, value); } remove { Events.RemoveHandler(substituteSortInfo, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFocusedRowChanged"),
#endif
 Category("Action")]
		public event EventHandler FocusedRowChanged { add { Events.AddHandler(focusedRowChanged, value); } remove { Events.RemoveHandler(focusedRowChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSelectionChanged"),
#endif
 Category("Action")]
		public event EventHandler SelectionChanged { add { Events.AddHandler(selectionChanged, value); } remove { Events.RemoveHandler(selectionChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPageIndexChanged"),
#endif
 Category("Action")]
		public event EventHandler PageIndexChanged { add { Events.AddHandler(pageIndexChanged, value); } remove { Events.RemoveHandler(pageIndexChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPageSizeChanged"),
#endif
 Category("Action")]
		public event EventHandler PageSizeChanged { add { Events.AddHandler(pageSizeChanged, value); } remove { Events.RemoveHandler(pageSizeChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStartRowEditing"),
#endif
 Category("Action")]
		public event ASPxStartRowEditingEventHandler StartRowEditing { add { Events.AddHandler(startRowEditing, value); } remove { Events.RemoveHandler(startRowEditing, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCancelRowEditing"),
#endif
 Category("Action")]
		public event ASPxStartRowEditingEventHandler CancelRowEditing { add { Events.AddHandler(cancelRowEditing, value); } remove { Events.RemoveHandler(cancelRowEditing, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomFilterExpressionDisplayText"),
#endif
 Category("Rendering")]
		public event CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText { add { Events.AddHandler(customFilterExpressionDisplayText, value); } remove { Events.RemoveHandler(customFilterExpressionDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlOperationVisibility"),
#endif
 Category("Rendering")]
		public new event FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility { add { base.FilterControlOperationVisibility += value; } remove { base.FilterControlOperationVisibility -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlParseValue"),
#endif
 Category("Rendering")]
		public new event FilterControlParseValueEventHandler FilterControlParseValue { add { base.FilterControlParseValue += value; } remove { base.FilterControlParseValue -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlCustomValueDisplayText"),
#endif
 Category("Rendering")]
		public new event FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText { add { base.FilterControlCustomValueDisplayText += value; } remove { base.FilterControlCustomValueDisplayText -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlColumnsCreated"),
#endif
 Category("Rendering")]
		public new event FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated { add { base.FilterControlColumnsCreated += value; } remove { base.FilterControlColumnsCreated -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlCriteriaValueEditorInitialize"),
#endif
 Category("Rendering")]
		public new event FilterControlCriteriaValueEditorInitializeEventHandler FilterControlCriteriaValueEditorInitialize { add { base.FilterControlCriteriaValueEditorInitialize += value; } remove { base.FilterControlCriteriaValueEditorInitialize -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlCriteriaValueEditorCreate"),
#endif
 Category("Rendering")]
		public new event FilterControlCriteriaValueEditorCreateEventHandler FilterControlCriteriaValueEditorCreate { add { base.FilterControlCriteriaValueEditorCreate += value; } remove { base.FilterControlCriteriaValueEditorCreate -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowInserting"),
#endif
 Category("Action")]
		public event ASPxDataInsertingEventHandler RowInserting { add { Events.AddHandler(rowInserting, value); } remove { Events.RemoveHandler(rowInserting, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowInserted"),
#endif
 Category("Action")]
		public event ASPxDataInsertedEventHandler RowInserted { add { Events.AddHandler(rowInserted, value); } remove { Events.RemoveHandler(rowInserted, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowUpdating"),
#endif
 Category("Action")]
		public event ASPxDataUpdatingEventHandler RowUpdating { add { Events.AddHandler(rowUpdating, value); } remove { Events.RemoveHandler(rowUpdating, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowUpdated"),
#endif
 Category("Action")]
		public event ASPxDataUpdatedEventHandler RowUpdated { add { Events.AddHandler(rowUpdated, value); } remove { Events.RemoveHandler(rowUpdated, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowDeleting"),
#endif
 Category("Action")]
		public event ASPxDataDeletingEventHandler RowDeleting { add { Events.AddHandler(rowDeleting, value); } remove { Events.RemoveHandler(rowDeleting, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowDeleted"),
#endif
 Category("Action")]
		public event ASPxDataDeletedEventHandler RowDeleted { add { Events.AddHandler(rowDeleted, value); } remove { Events.RemoveHandler(rowDeleted, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewParseValue"),
#endif
 Category("Data")]
		public event ASPxParseValueEventHandler ParseValue { add { Events.AddHandler(parseValue, value); } remove { Events.RemoveHandler(parseValue, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewInitNewRow"),
#endif
 Category("Data")]
		public event ASPxDataInitNewRowEventHandler InitNewRow { add { Events.AddHandler(initNewRow, value); } remove { Events.RemoveHandler(initNewRow, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowValidating"),
#endif
 Category("Action")]
		public event ASPxDataValidationEventHandler RowValidating { add { Events.AddHandler(rowValidating, value); } remove { Events.RemoveHandler(rowValidating, value); } }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridViewClientLayout")]
#endif
		public new event ASPxClientLayoutHandler ClientLayout { add { base.ClientLayout += value; } remove { base.ClientLayout -= value; } }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGridViewBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult { add { Events.AddHandler(EventBeforeGetCallbackResult, value); } remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAutoFilterCellEditorCreate"),
#endif
 Category("Rendering")]
		public event ASPxGridViewEditorCreateEventHandler AutoFilterCellEditorCreate { add { Events.AddHandler(autoFilterCellEditorCreate, value); } remove { Events.RemoveHandler(autoFilterCellEditorCreate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAutoFilterCellEditorInitialize"),
#endif
 Category("Rendering")]
		public event ASPxGridViewEditorEventHandler AutoFilterCellEditorInitialize { add { Events.AddHandler(autoFilterCellEditorInitialize, value); } remove { Events.RemoveHandler(autoFilterCellEditorInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSummaryDisplayText"),
#endif
 Category("Rendering")]
		public event ASPxGridViewSummaryDisplayTextEventHandler SummaryDisplayText { add { Events.AddHandler(summaryDisplayText, value); } remove { Events.RemoveHandler(summaryDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailRowGetButtonVisibility"),
#endif
 Category("Rendering")]
		public event ASPxGridViewDetailRowButtonEventHandler DetailRowGetButtonVisibility { add { Events.AddHandler(detailRowGetButtonVisibility, value); } remove { Events.RemoveHandler(detailRowGetButtonVisibility, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBeforePerformDataSelect"),
#endif
 Category("Data")]
		public event EventHandler BeforePerformDataSelect { add { Events.AddHandler(beforePerformDataSelect, value); } remove { Events.RemoveHandler(beforePerformDataSelect, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewProcessColumnAutoFilter"),
#endif
 Category("Data")]
		public event ASPxGridViewAutoFilterEventHandler ProcessColumnAutoFilter { add { Events.AddHandler(processColumnAutoFilter, value); } remove { Events.RemoveHandler(processColumnAutoFilter, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewProcessOnClickRowFilter"),
#endif
 Category("Data")]
		public event ASPxGridViewOnClickRowFilterEventHandler ProcessOnClickRowFilter { add { Events.AddHandler(processMultiColumnAutoFilter, value); } remove { Events.RemoveHandler(processMultiColumnAutoFilter, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBatchUpdate"),
#endif
 Category("Action")]
		public event ASPxDataBatchUpdateEventHandler BatchUpdate { add { Events.AddHandler(batchUpdate, value); } remove { Events.RemoveHandler(batchUpdate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomGroupDisplayText"),
#endif
 Category("Rendering")]
		public event ASPxGridViewColumnDisplayTextEventHandler CustomGroupDisplayText { add { Events.AddHandler(customGroupDisplayText, value); } remove { Events.RemoveHandler(customGroupDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomSummaryCalculate"),
#endif
 Category("Data")]
		public event CustomSummaryEventHandler CustomSummaryCalculate { add { Events.AddHandler(customSummaryCalculate, value); } remove { Events.RemoveHandler(customSummaryCalculate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlRowCreated"),
#endif
 Category("Rendering")]
		public event ASPxGridViewTableRowEventHandler HtmlRowCreated { add { Events.AddHandler(htmlRowCreated, value); } remove { Events.RemoveHandler(htmlRowCreated, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlRowPrepared"),
#endif
 Category("Rendering")]
		public event ASPxGridViewTableRowEventHandler HtmlRowPrepared { add { Events.AddHandler(htmlRowPrepared, value); } remove { Events.RemoveHandler(htmlRowPrepared, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlDataCellPrepared"),
#endif
 Category("Rendering")]
		public event ASPxGridViewTableDataCellEventHandler HtmlDataCellPrepared { add { Events.AddHandler(htmlDataCellPrepared, value); } remove { Events.RemoveHandler(htmlDataCellPrepared, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlCommandCellPrepared"),
#endif
 Category("Rendering")]
		public event ASPxGridViewTableCommandCellEventHandler HtmlCommandCellPrepared { add { Events.AddHandler(htmlCommandCellPrepared, value); } remove { Events.RemoveHandler(htmlCommandCellPrepared, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewRowCommand"),
#endif
 Category("Action")]
		public event ASPxGridViewRowCommandEventHandler RowCommand { add { Events.AddHandler(rowCommand, value); } remove { Events.RemoveHandler(rowCommand, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlEditFormCreated"),
#endif
 Category("Rendering")]
		public event ASPxGridViewEditFormEventHandler HtmlEditFormCreated { add { Events.AddHandler(htmlEditFormCreated, value); } remove { Events.RemoveHandler(htmlEditFormCreated, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewHtmlFooterCellPrepared"),
#endif
 Category("Rendering")]
		public event ASPxGridViewTableFooterCellEventHandler HtmlFooterCellPrepared { add { Events.AddHandler(htmlFooterCellPrepared, value); } remove { Events.RemoveHandler(htmlFooterCellPrepared, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailsChanged"),
#endif
 Category("Action")]
		public event EventHandler DetailsChanged { add { Events.AddHandler(detailsChanged, value); } remove { Events.RemoveHandler(detailsChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailRowExpandedChanged"),
#endif
 Category("Action")]
		public event ASPxGridViewDetailRowEventHandler DetailRowExpandedChanged { add { Events.AddHandler(detailRowExpandedChanged, value); } remove { Events.RemoveHandler(detailRowExpandedChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFillContextMenuItems"),
#endif
 Category("Rendering")]
		public event ASPxGridViewFillContextMenuItemsEventHandler FillContextMenuItems { add { Events.AddHandler(fillContextMenuItems, value); } remove { Events.RemoveHandler(fillContextMenuItems, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuItemVisibility"),
#endif
 Category("Rendering")]
		public event ASPxGridViewContextMenuItemVisibilityEventHandler ContextMenuItemVisibility { add { Events.AddHandler(contextMenuItemVisibility, value); } remove { Events.RemoveHandler(contextMenuItemVisibility, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuItemClick"),
#endif
 Category("Rendering")]
		public event ASPxGridViewContextMenuItemClickEventHandler ContextMenuItemClick { add { Events.AddHandler(contextMenuItemClick, value); } remove { Events.RemoveHandler(contextMenuItemClick, value); } }
		[ Category("Rendering")]
		public event ASPxGridViewAddSummaryItemViaContextMenuEventHandler AddSummaryItemViaContextMenu { add { Events.AddHandler(addSummaryItemViaContextMenu, value); } remove { Events.RemoveHandler(addSummaryItemViaContextMenu, value); } }
		protected internal virtual void RaiseAutoFilterEditorCreate(ASPxGridViewEditorCreateEventArgs e) {
			var handler = (ASPxGridViewEditorCreateEventHandler)Events[autoFilterCellEditorCreate];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseAutoFilterEditorInitialize(ASPxGridViewEditorEventArgs e) {
			var handler = (ASPxGridViewEditorEventHandler)Events[autoFilterCellEditorInitialize];
			if(handler != null) handler(this, e);
		}
		protected internal override string RaiseSummaryDisplayText(ASPxGridSummaryDisplayTextEventArgs e) {
			var handler = (ASPxGridViewSummaryDisplayTextEventHandler)Events[summaryDisplayText];
			if(handler != null) handler(this, e as ASPxGridViewSummaryDisplayTextEventArgs);
			return e.Text;
		}
		protected internal virtual void RaiseDetailRowGetButtonVisibility(ASPxGridViewDetailRowButtonEventArgs e) {
			var handler = (ASPxGridViewDetailRowButtonEventHandler)Events[detailRowGetButtonVisibility];
			if(handler != null) handler(this, e);
		}
		protected internal override void RaiseBeforePerformDataSelect() {
			var handler = (EventHandler)Events[beforePerformDataSelect];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseProcessColumnAutoFilter(ASPxGridViewAutoFilterEventArgs e) {
			var handler = (ASPxGridViewAutoFilterEventHandler)Events[processColumnAutoFilter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseProcessOnClickRowFilter(ASPxGridViewOnClickRowFilterEventArgs e) {
			var handler = (ASPxGridViewOnClickRowFilterEventHandler)Events[processMultiColumnAutoFilter];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlEditFormCreated(WebControl container) {
			var handler = (ASPxGridViewEditFormEventHandler)Events[htmlEditFormCreated];
			if(handler == null) return;
			RenderUtils.EnsureChildControlsRecursive(container, false);
			handler(this, new ASPxGridViewEditFormEventArgs(container));
		}
		protected internal virtual void RaiseHtmlFooterCellPrepared(GridViewColumn column, int visibleIndex, TableCell cell) {
			var handler = (ASPxGridViewTableFooterCellEventHandler)Events[htmlFooterCellPrepared];
			if(handler == null) return;
			var e = new ASPxGridViewTableFooterCellEventArgs(this, column, visibleIndex, cell);
			handler(this, e);
		}
		protected override void RaiseDetailRowsChanged() {
			var handler = (EventHandler)Events[detailsChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseDetailRowExpandedChanged(ASPxGridViewDetailRowEventArgs e) {
			var handler = (ASPxGridViewDetailRowEventHandler)Events[detailRowExpandedChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseFillContextMenuItems(ASPxGridViewContextMenuEventArgs e) {
			var handler = (ASPxGridViewFillContextMenuItemsEventHandler)Events[fillContextMenuItems];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextMenuItemVisibility(ASPxGridViewContextMenuItemVisibilityEventArgs e) {
			var handler = (ASPxGridViewContextMenuItemVisibilityEventHandler)Events[contextMenuItemVisibility];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextMenuItemClick(ASPxGridViewContextMenuItemClickEventArgs e) {
			var handler = (ASPxGridViewContextMenuItemClickEventHandler)Events[contextMenuItemClick];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseAddSummaryItemViaContextMenu(ASPxGridViewAddSummaryItemViaContextMenuEventArgs e) {
			var handler = (ASPxGridViewAddSummaryItemViaContextMenuEventHandler)Events[addSummaryItemViaContextMenu];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseBatchUpdate(ASPxDataBatchUpdateEventArgs e) {
			var handler = (ASPxDataBatchUpdateEventHandler)Events[batchUpdate];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomGroupDisplayText(ASPxGridViewColumnDisplayTextEventArgs e) {
			var handler = (ASPxGridViewColumnDisplayTextEventHandler)Events[customGroupDisplayText];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseCustomSummaryCalculate(CustomSummaryEventArgs e) {
			var handler = (CustomSummaryEventHandler)Events[customSummaryCalculate];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlRowCreated(GridViewTableRow row) {
			var handler = (ASPxGridViewTableRowEventHandler)Events[htmlRowCreated];
			if(handler == null) return;
			RenderUtils.EnsureChildControlsRecursive(row, false);
			var e = new ASPxGridViewTableRowEventArgs(row);
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlRowPrepared(GridViewTableRow row) {
			var handler = (ASPxGridViewTableRowEventHandler)Events[htmlRowPrepared];
			if(handler == null) return;
			var e = new ASPxGridViewTableRowEventArgs(row);
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlDataCellPrepared(GridViewTableDataCell cell) {
			var handler = (ASPxGridViewTableDataCellEventHandler)Events[htmlDataCellPrepared];
			if(handler == null) return;
			var e = new ASPxGridViewTableDataCellEventArgs(cell, DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(cell.VisibleIndex) : null);
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseHtmlCommandCellPrepared(GridViewTableBaseCommandCell cell) {
			var handler = (ASPxGridViewTableCommandCellEventHandler)Events[htmlCommandCellPrepared];
			if(handler == null) return;
			object keyValue = cell.VisibleIndex > -1 && DataProxy.HasCorrectKeyFieldName ? DataProxy.GetRowKeyValue(cell.VisibleIndex) : null;
			var e = new ASPxGridViewTableCommandCellEventArgs(cell, keyValue);
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRowCommand(ASPxGridViewRowCommandEventArgs e) {
			var handler = (ASPxGridViewRowCommandEventHandler)Events[rowCommand];
			if(handler != null) handler(this, e);
		}
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			var rowCommand = args as ASPxGridViewRowCommandEventArgs;
			if(rowCommand != null) {
				RaiseRowCommand(rowCommand);
				return true;
			}
			return base.OnBubbleEvent(source, args);
		}
		protected internal override void RaiseCustomColumnDisplayText(ASPxGridColumnDisplayTextEventArgs e) {
			var handler = (ASPxGridViewColumnDisplayTextEventHandler)Events[customColumnDisplayText];
			if(handler != null)
				handler(this, (ASPxGridViewColumnDisplayTextEventArgs)e);
		}
		protected override void RaiseCustomUnboundColumnData(ASPxGridColumnDataEventArgs e) {
			var handler = Events[customUnboundColumnData] as ASPxGridViewColumnDataEventHandler;
			if(handler != null) handler(this, e as ASPxGridViewColumnDataEventArgs);
		}
		protected internal override void RaiseBeforeHeaderFilterFillItems(ASPxGridBeforeHeaderFilterFillItemsEventArgs e) {
			var handler = (ASPxGridViewBeforeHeaderFilterFillItemsEventHandler)Events[beforeHeaderFilterFillItems];
			if(handler != null) handler(this, e as ASPxGridViewBeforeHeaderFilterFillItemsEventArgs);
		}
		protected internal override void RaiseHeaderFilterFillItems(ASPxGridHeaderFilterEventArgs e) {
			var handler = (ASPxGridViewHeaderFilterEventHandler)Events[headerFilterFillItems];
			if(handler != null) handler(this, e as ASPxGridViewHeaderFilterEventArgs);
		}
		protected internal override void RaiseCommandButtonInitialize(ASPxGridCommandButtonEventArgs e) {
			var handler = (ASPxGridViewCommandButtonEventHandler)Events[commandButtonInitialize];
			if(handler != null) handler(this, e as ASPxGridViewCommandButtonEventArgs);
		}
		protected internal override void RaiseCustomButtonInitialize(ASPxGridCustomCommandButtonEventArgs e) {
			var handler = (ASPxGridViewCustomButtonEventHandler)Events[customButtonInitialize];
			if(handler != null) handler(this, e as ASPxGridViewCustomButtonEventArgs);
		}
		protected internal override void RaiseEditorInitialize(ASPxGridEditorEventArgs e) {
			var handler = (ASPxGridViewEditorEventHandler)Events[cellEditorInitialize];
			if(handler != null) handler(this, e as ASPxGridViewEditorEventArgs);
		}
		protected internal override void RaiseSearchPanelEditorCreate(ASPxGridEditorCreateEventArgs e) {
			var handler = (ASPxGridViewSearchPanelEditorCreateEventHandler)Events[searchPanelEditorCreate];
			if(handler != null) handler(this, e as ASPxGridViewSearchPanelEditorCreateEventArgs);
		}
		protected internal override void RaiseSearchPanelEditorInitialize(ASPxGridEditorEventArgs e) {
			var handler = (ASPxGridViewSearchPanelEditorEventHandler)Events[searchPanelEditorInitialize];
			if(handler != null) handler(this, e as ASPxGridViewSearchPanelEditorEventArgs);
		}
		protected internal override void RaiseBeforeColumnSortingGrouping(ASPxGridBeforeColumnGroupingSortingEventArgs e) {
			var handler = (ASPxGridViewBeforeColumnGroupingSortingEventHandler)Events[beforeColumnSortingGrouping];
			if(handler != null) handler(this, e as ASPxGridViewBeforeColumnGroupingSortingEventArgs);
		}
		protected override void RaiseCustomCallback(ASPxGridCustomCallbackEventArgs e) {
			var handler = (ASPxGridViewCustomCallbackEventHandler)Events[customCallback];
			if(handler != null) handler(this, e as ASPxGridViewCustomCallbackEventArgs);
		}
		protected override void RaiseCustomDataCallback(ASPxGridCustomCallbackEventArgs e) {
			var handler = (ASPxGridViewCustomDataCallbackEventHandler)Events[customDataCallback];
			if(handler != null) handler(this, e as ASPxGridViewCustomDataCallbackEventArgs);
		}
		protected override void RaiseCustomButtonCallback(ASPxGridCustomButtonCallbackEventArgs e) {
			var handler = (ASPxGridViewCustomButtonCallbackEventHandler)Events[customButtonCallback];
			if(handler != null) handler(this, e as ASPxGridViewCustomButtonCallbackEventArgs);
		}
		protected override void RaiseAfterPerformCallback(ASPxGridAfterPerformCallbackEventArgs e) {
			var handler = (ASPxGridViewAfterPerformCallbackEventHandler)Events[afterPerformCallback];
			if(handler != null) handler(this, e as ASPxGridViewAfterPerformCallbackEventArgs);
		}
		protected internal override void RaiseCustomColumnSort(GridCustomColumnSortEventArgs e) {
			var handler = (ASPxGridViewCustomColumnSortEventHandler)Events[customColumnSort];
			if(handler != null) handler(this, e as CustomColumnSortEventArgs);
		}
		protected internal override void RaiseCustomColumnGroup(GridCustomColumnSortEventArgs e) {
			var handler = (ASPxGridViewCustomColumnSortEventHandler)Events[customColumnGroup];
			if(handler != null) handler(this, e as CustomColumnSortEventArgs);
		}
		protected override void RaiseGridCustomJSProperties(CustomJSPropertiesEventArgs e) {
			var handler = Events[EventCustomJsProperties] as ASPxGridViewClientJSPropertiesEventHandler;
			if(handler != null) handler(this, e as ASPxGridViewClientJSPropertiesEventArgs);
		}
		protected internal override string RaiseCustomErrorText(ASPxGridCustomErrorTextEventArgs e) {
			var handler = (ASPxGridViewCustomErrorTextEventHandler)Events[customErrorText];
			if(handler != null) handler(this, e as ASPxGridViewCustomErrorTextEventArgs);
			return e.ErrorText;
		}
		protected override void RaiseSubstituteFilter(SubstituteFilterEventArgs e) {
			var handler = (EventHandler<SubstituteFilterEventArgs>)Events[substituteFilter];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseSubstituteSortInfo(SubstituteSortInfoEventArgs e) {
			var handler = (EventHandler<SubstituteSortInfoEventArgs>)Events[substituteSortInfo];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaisePageIndexChanged() {
			base.RaisePageIndexChanged();
			var handler = (EventHandler)Events[pageIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaisePageSizeChanged() {
			var handler = (EventHandler)Events[pageSizeChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseSelectionChanged() {
			var handler = (EventHandler)Events[selectionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseFocusedRowChanged() {
			var handler = (EventHandler)Events[focusedRowChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseStartEditingRow(ASPxStartItemEditingEventArgs e) {
			var handler = (ASPxStartRowEditingEventHandler)Events[startRowEditing];
			if(handler != null) handler(this, e as ASPxStartRowEditingEventArgs);
		}
		protected override void RaiseCancelEditingRow(ASPxStartItemEditingEventArgs e) {
			var handler = (ASPxStartRowEditingEventHandler)Events[cancelRowEditing];
			if(handler != null) handler(this, e as ASPxStartRowEditingEventArgs);
		}
		protected override void RaiseFilterControlCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			var handler = Events[customFilterExpressionDisplayText] as CustomFilterExpressionDisplayTextEventHandler;
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowInserting(ASPxDataInsertingEventArgs e) {
			var handler = (ASPxDataInsertingEventHandler)Events[rowInserting];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowInserted(ASPxDataInsertedEventArgs e) {
			var handler = (ASPxDataInsertedEventHandler)Events[rowInserted];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowUpdating(ASPxDataUpdatingEventArgs e) {
			var handler = (ASPxDataUpdatingEventHandler)Events[rowUpdating];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowUpdated(ASPxDataUpdatedEventArgs e) {
			var handler = (ASPxDataUpdatedEventHandler)Events[rowUpdated];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowDeleting(ASPxDataDeletingEventArgs e) {
			var handler = (ASPxDataDeletingEventHandler)Events[rowDeleting];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowDeleted(ASPxDataDeletedEventArgs e) {
			var handler = (ASPxDataDeletedEventHandler)Events[rowDeleted];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseParseValue(ASPxParseValueEventArgs e) {
			var handler = (ASPxParseValueEventHandler)Events[parseValue];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseInitNewRow(ASPxDataInitNewRowEventArgs e) {
			var handler = (ASPxDataInitNewRowEventHandler)Events[initNewRow];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowValidating(ASPxGridDataValidationEventArgs e) {
			var handler = (ASPxDataValidationEventHandler)Events[rowValidating];
			if(handler != null) handler(this, e as ASPxDataValidationEventArgs);
		}
		protected internal override ASPxGridColumnDisplayTextEventArgs CreateColumnDisplayTextEventArgs(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, object value) {
			return new ASPxGridViewColumnDisplayTextEventArgs((GridViewDataColumn)column, visibleIndex, value, provider);
		}
		protected override ASPxGridColumnDataEventArgs CreateColumnDataEventArgs(IWebGridDataColumn column, int listSourceRowIndex, object value, bool isGetAction) {
			return new ASPxGridViewColumnDataEventArgs(column as GridViewDataColumn, listSourceRowIndex, value, isGetAction);
		}
		protected internal override ASPxGridBeforeHeaderFilterFillItemsEventArgs CreateBeforeHeaderFilterFillItemsEventArgs(IWebGridDataColumn column) {
			return new ASPxGridViewBeforeHeaderFilterFillItemsEventArgs(column as GridViewDataColumn);
		}
		protected internal override ASPxGridHeaderFilterEventArgs CreateHeaderFilterFillItemsEventArgs(IWebGridDataColumn column, GridHeaderFilterValues values) {
			return new ASPxGridViewHeaderFilterEventArgs(column as GridViewDataColumn, values);
		}
		protected internal override ASPxGridEditorEventArgs CreateCellEditorInitializeEventArgs(IWebGridDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value) {
			return new ASPxGridViewEditorEventArgs(column as GridViewDataColumn, visibleIndex, editor, keyValue, value);
		}
		protected internal override ASPxGridEditorCreateEventArgs CreateSearchPanelEditorCreateEventArgs(EditPropertiesBase editorProperties, object value) {
			return new ASPxGridViewSearchPanelEditorCreateEventArgs(editorProperties, value);
		}
		protected internal override ASPxGridEditorEventArgs CreateSearchPanelEditorInitializeEventArgs(ASPxEditBase editor, object value) {
			return new ASPxGridViewSearchPanelEditorEventArgs(editor, value);
		}
		protected internal override ASPxGridBeforeColumnGroupingSortingEventArgs CreateBeforeColumnSortingGroupingEventArgs(IWebGridDataColumn column, ColumnSortOrder sortOrder, int sortIndex, int groupIndex) {
			return new ASPxGridViewBeforeColumnGroupingSortingEventArgs(column as GridViewDataColumn, sortOrder, sortIndex, groupIndex);
		}
		protected internal override ASPxGridCustomCallbackEventArgs CreateCustomCallbackEventArgs(string parameters) {
			return new ASPxGridViewCustomCallbackEventArgs(parameters);
		}
		protected internal override ASPxGridCustomCallbackEventArgs CreateCustomDataCallbackEventArgs(string parameters) {
			return new ASPxGridViewCustomDataCallbackEventArgs(parameters);
		}
		protected internal override ASPxGridCustomButtonCallbackEventArgs CreateCustomButtonCallbackEventArgs(string buttonID, int visibleIndex) {
			return new ASPxGridViewCustomButtonCallbackEventArgs(buttonID, visibleIndex);
		}
		protected internal override ASPxGridAfterPerformCallbackEventArgs CreateAfterPerformCallbackEventArgs(string callbackName, string[] args) {
			return new ASPxGridViewAfterPerformCallbackEventArgs(callbackName, args);
		}
		protected internal override GridCustomColumnSortEventArgs CreateCustomColumnSortEventArgs(IWebGridDataColumn column, object value1, object value2, ColumnSortOrder sortOrder) {
			return new CustomColumnSortEventArgs(column as GridViewDataColumn, value1, value2, sortOrder);
		}
		protected internal override CustomJSPropertiesEventArgs CreateCustomJSPropertiesEventArgs(Dictionary<string, object> properties) {
			return new ASPxGridViewClientJSPropertiesEventArgs(properties);
		}
		protected internal override ASPxGridCustomErrorTextEventArgs CreateCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText) {
			return new ASPxGridViewCustomErrorTextEventArgs(exception, errorTextKind, errorText);
		}
		protected internal override ASPxStartItemEditingEventArgs CreateStartEditingEventArgs(object editingKeyValue) {
			return new ASPxStartRowEditingEventArgs(editingKeyValue);
		}
		protected internal override ASPxGridDataValidationEventArgs CreateItemValidatingEventArgs(int visibleIndex, bool isNew) {
			return new ASPxDataValidationEventArgs(visibleIndex, isNew);
		}
		#endregion
		[Browsable(false)]
		public new int VisibleRowCount { get { return base.VisibleRowCount; } }
		[Browsable(false)]
		public new int VisibleStartIndex { get { return base.VisibleStartIndex; } }
		[Browsable(false)]
		public new GridViewSelection Selection { get { return base.Selection as GridViewSelection; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int FocusedRowIndex { get { return base.FocusedRowIndex; } set { base.FocusedRowIndex = value; } }
		public new object GetRow(int visibleIndex) { return base.GetRow(visibleIndex); }
		public new DataRow GetDataRow(int visibleIndex) { return base.GetDataRow(visibleIndex); }
		public new List<object> GetSelectedFieldValues(params string[] fieldNames) { return base.GetSelectedFieldValues(fieldNames); }
		public new List<object> GetFilteredSelectedValues(params string[] fieldNames) { return base.GetFilteredSelectedValues(fieldNames); }
		public new object GetRowValues(int visibleIndex, params string[] fieldNames) { return base.GetRowValues(visibleIndex, fieldNames); }
		public new object GetRowValuesByKeyValue(object keyValue, params string[] fieldNames) { return base.GetRowValuesByKeyValue(keyValue, fieldNames); }
		public new int FindVisibleIndexByKeyValue(object keyValue) { return base.FindVisibleIndexByKeyValue(keyValue); }
		public new List<object> GetCurrentPageRowValues(params string[] fieldNames) { return base.GetCurrentPageRowValues(fieldNames); }
		public new bool MakeRowVisible(object keyValue) { return base.MakeRowVisible(keyValue); }
		[Browsable(false), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int PageIndex { get { return base.PageIndex; } set { base.PageIndex = value; } }
		[Browsable(false), AutoFormatDisable]
		public new int PageCount { get { return base.PageCount; } }
		[Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		[Browsable(false), DefaultValue(true), AutoFormatDisable]
		public new bool FilterEnabled { get { return base.FilterEnabled; } set { base.FilterEnabled = value; } }
		[Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SearchPanelFilter { get { return base.SearchPanelFilter; } set { base.SearchPanelFilter = value; } }
		[Browsable(false)]
		public new int SortCount { get { return base.SortCount; } }
		public new void DoRowValidation() {
			base.DoRowValidation();
		}
		public new void StartEdit(int visibleIndex) {
			base.StartEdit(visibleIndex);
		}
		public new void UpdateEdit() {
			base.UpdateEdit();
		}
		public new void CancelEdit() {
			base.CancelEdit();
		}
		public new void AddNewRow() {
			base.AddNewRow();
		}
		public new void DeleteRow(int visibleIndex) {
			base.DeleteRow(visibleIndex);
		}
		public new void ShowFilterControl() { base.ShowFilterControl(); }
		public new void HideFilterControl() { base.HideFilterControl(); }
		[Browsable(false)]
		public new bool IsFilterControlVisible { get { return base.IsFilterControlVisible; } }
		[AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new int ScrollToVisibleIndexOnClient { get { return base.ScrollToVisibleIndexOnClient; } set { base.ScrollToVisibleIndexOnClient = value; } }
		[Browsable(false)]
		public new bool IsEditing { get { return base.IsEditing; } }
		[Browsable(false)]
		public new bool IsNewRowEditing { get { return base.IsNewRowEditing; } }
		[Browsable(false)]
		public new int EditingRowVisibleIndex { get { return base.EditingRowVisibleIndex; } }
		public new void BeginUpdate() { base.BeginUpdate(); }
		public new void EndUpdate() { base.EndUpdate(); }
		[Browsable(false)]
		public new bool IsLockUpdate { get { return base.IsLockUpdate; } }
		public new void ClearSort() { base.ClearSort(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewImagesFilterControl"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlImages ImagesFilterControl { get { return base.ImagesFilterControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewStylesFilterControl"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlStyles StylesFilterControl { get { return base.StylesFilterControl; } }
		public new string SaveClientLayout() { return base.SaveClientState(); }
		public new void LoadClientLayout(string layoutData) { base.LoadClientLayout(layoutData); }
		public int GetChildRowCount(int groupRowVisibleIndex) {
			return DataProxy.GetChildRowCount(groupRowVisibleIndex);
		}
		public object GetChildRow(int groupRowVisibleIndex, int childIndex) {
			return DataProxy.GetChildRow(groupRowVisibleIndex, childIndex);
		}
		public DataRow GetChildDataRow(int groupRowVisibleIndex, int childIndex) {
			DataRowView rowView = DataProxy.GetChildRow(groupRowVisibleIndex, childIndex) as DataRowView;
			return rowView != null ? rowView.Row : null;
		}
		public object GetChildRowValues(int groupRowVisibleIndex, int childIndex, params string[] fieldNames) {
			return DataProxy.GetChildRowValues(groupRowVisibleIndex, childIndex, fieldNames);
		}
		public bool IsGroupRow(int visibleIndex) {
			return DataProxy.GetRowType(visibleIndex) == WebRowType.Group;
		}
		public int GetRowLevel(int visibleIndex) { return DataProxy.GetRowLevel(visibleIndex); }
		public void CollapseRow(int visibleIndex) { CollapseRow(visibleIndex, false); }
		public void CollapseRow(int visibleIndex, bool recursive) {
			DataBoundProxy.CollapseRow(visibleIndex, recursive);
			LayoutChanged();
		}
		public void ExpandRow(int visibleIndex) { ExpandRow(visibleIndex, false); }
		public void ExpandRow(int visibleIndex, bool recursive) {
			DataBoundProxy.ExpandRow(visibleIndex, recursive);
			LayoutChanged();
		}
		public bool IsRowExpanded(int visibleIndex) {
			return DataProxy.IsRowExpanded(visibleIndex);
		}
		public void ExpandAll() {
			DataBoundProxy.ExpandAll();
			LayoutChanged();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		public void CollapseAll() {
			DataBoundProxy.CollapseAll();
			LayoutChanged();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		public void UnGroup(GridViewColumn column) {
			GroupBy(column, -1);
		}
		public int GroupBy(GridViewColumn column) {
			return GroupBy(column, GroupCount);
		}
		public int GroupBy(GridViewColumn column, int value) {
			return GroupBy(column as IWebGridColumn, value);
		}
		public int GroupBy(IWebGridColumn column) {
			return GroupBy(column, GroupCount);
		}
		public int GroupBy(IWebGridColumn column, int value) {
			if(column == null) throw new ArgumentNullException("column");
			var dc = column as IWebGridDataColumn;
			if(dc == null) throw new ArgumentException("Column should be DataColumn", "column");
			ColumnSortOrder order;
			if(value == -1)
				order = dc.Adapter.UngroupedSortOrder;
			else {
				order = dc.Adapter.SortOrder;
				if(order == ColumnSortOrder.None)
					order = ColumnSortOrder.Ascending;
			}
			return SortedColumnsChanged(dc, value, true, order);
		}
		public virtual string GetPreviewText(int visibleIndex) {
			return DataProxy.GetRowDisplayText(visibleIndex, PreviewFieldName);
		}
		public string GetGroupRowSummaryText(int visibleIndex) {
			StringBuilder sb = new StringBuilder();
			GetGroupRowSummaryTextCore(visibleIndex, sb);
			return sb.ToString();
		}
		protected internal virtual void GetGroupRowSummaryTextCore(int visibleIndex, StringBuilder sb) {
			int ac = 0;
			foreach(ASPxSummaryItem item in GroupSummary.GetGroupRowItems()) {
				if(!DataProxy.IsGroupSummaryExists(visibleIndex, item)) continue;
				if(ac++ == 0) {
					sb.Append(' ');
					if(!IsRightToLeft())
						sb.Append('(');
				} else {
					sb.Append(Settings.GroupSummaryTextSeparator);
				}
				object value = DataProxy.GetGroupSummaryValue(visibleIndex, item);
				string text = item.GetGroupRowDisplayText(ColumnHelper.FindColumnByString(item.FieldName), value);
				text = RaiseSummaryDisplayText(new ASPxGridViewSummaryDisplayTextEventArgs(item, value, text, visibleIndex, true));
				sb.Append(text);
			}
			if(ac > 0 && !IsRightToLeft())
				sb.Append(")");
		}
		public object GetTotalSummaryValue(ASPxSummaryItem item) {
			return base.GetTotalSummaryValue(item);
		}
		public object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			if(item == null) throw new ArgumentNullException("item");
			return DataProxy.GetGroupSummaryValue(visibleIndex, item);
		}
		public void AutoFilterByColumn(GridViewColumn column, string value) {
			AutoFilterByColumn(column as IWebGridColumn, value);
		}
		protected internal void AutoFilterByColumn(IWebGridColumn column, string value) {
			if(column == null) throw new ArgumentNullException("column");
			var dcColumn = column as IWebGridDataColumn;
			if(dcColumn == null) new ArgumentException("Column should be DataColumn", "column");
			var filter = FilterHelper.CreateAutoFilter(dcColumn, value);
			ApplyFilterToColumn(dcColumn, filter);
		}
		public virtual void ApplyFilterToColumn(GridViewDataColumn column, CriteriaOperator criteria) {
			base.ApplyFilterToColumn(column, criteria);
		}
		public bool IsAllowSort(GridViewColumn column) {
			return base.IsAllowSort(column);
		}
		public bool IsAllowGroup(GridViewColumn column) {
			return base.IsAllowGroup(column);
		}
		public virtual bool IsReadOnly(GridViewDataColumn column) {
			return base.IsReadOnly(column);
		}
		public int SortBy(GridViewColumn column, int value) {
			return base.SortBy(column, value);
		}
		public ColumnSortOrder SortBy(GridViewColumn column, ColumnSortOrder value) {
			return base.SortBy(column, value);
		}
		public object GetMasterRowKeyValue() {
			return ASPxGridView.GetDetailRowKeyValue(this);
		}
		public object GetMasterRowFieldValues(params string[] fieldNames) {
			return ASPxGridView.GetDetailRowValues(this, fieldNames);
		}
		protected override GridSortData CreateSortData() {
			return new GridViewSortData(this);
		}
		protected override bool ProcessContextMenuItemClick(string values) { 
			if(values == null)
				return false;
			var arguments = values.Split(',');
			var menuType = (GridViewContextMenuType)int.Parse(arguments[0]);
			var menu = RenderHelper.GetContextMenu(menuType);
			if(menu == null)
				return false;
			var item = (GridViewContextMenuItem)menu.GetItemByIndexPath(arguments[1]);
			int elementIndex = -1;
			int.TryParse(arguments[2], out elementIndex);
			var args = new ASPxGridViewContextMenuItemClickEventArgs(menuType, item, elementIndex);
			RaiseContextMenuItemClick(args);
			return args.Handled;
		}
		protected override GridFormLayoutProperties CreateEditFormLayoutProperties() { return new GridViewFormLayoutProperties(this); }
		protected override IWebGridDataColumn CreateEditColumn(Type dataType) {
			return GridViewEditDataColumn.CreateColumn(dataType);
		}
		protected override WebColumnsOwnerDefaultImplementation CreateWebColumnsOwnerImpl() {
			return new WebColumnsOwnerGridViewImplementation(this, Columns);
		}
		protected override GridCookiesBase CreateControlCookies() {
			return new GridViewCookies(this);
		}
		protected override void ProcessCookies(GridCookiesBase cookies, string state) {
			if(string.IsNullOrEmpty(state))
				return;
			if(AutoGenerateColumns && (SettingsCookies.StoreColumnsVisiblePosition || SettingsCookies.StoreColumnsWidth))
				BindAndSynchronizeDataProxy();
			base.ProcessCookies(cookies, state);
			var gridCookies = cookies as GridViewCookies;
			if(SettingsCookies.StoreControlWidth)
				Width = gridCookies.ControlWidth;
		}
		protected override GridColumnsState CreateColumnsState() {
			return new GridViewColumnsState(this);
		}
		protected override void LoadGridControlStateCore(TypedBinaryReader reader, Dictionary<string, object> clientState) {
			base.LoadGridControlStateCore(reader, clientState);
			ApplyColumnResizingResult((string)clientState["ResizedColumnWidths"]);
		}
		protected void ApplyColumnResizingResult(string columnsResizingResult) {
			if(String.IsNullOrEmpty(columnsResizingResult))
				return;
			IDictionary widths = HtmlConvertor.FromJSON<IDictionary>(columnsResizingResult);
			foreach(object key in widths.Keys) {
				if(key.ToString() == "ctrlWidth") {
					Width = Unit.Parse(widths[key].ToString());
					continue;
				}
				var columnIndex = Convert.ToInt32(key);
				if(columnIndex < AllColumns.Count)
					AllColumns[columnIndex].Width = Unit.Parse(widths[key].ToString());
			}
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxGridView), GridViewScriptResourceName);
			RegisterIncludeScript(typeof(ASPxGridView), GridViewTableColumnResizingResourceName, RenderHelper.RequireTablesHelperScripts);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientGridView";
		}
		protected override void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			stb.AppendFormat("{0}.editMode={1};\n", localVarName, (int)SettingsEditing.Mode);
			stb.AppendFormat("{0}.indentColumnCount={1};\n", localVarName, RenderHelper.IndentColumnCount);
			stb.AppendFormat("{0}.allowMultiColumnAutoFilter={1};\n", localVarName, HtmlConvertor.ToScript(RenderHelper.AllowMultiColumnAutoFilter));
			if(RenderHelper.UseFixedGroups)
				stb.AppendFormat("{0}.allowFixedGroups=true;\n", localVarName);
			if(SettingsBehavior.ColumnResizeMode != ColumnResizeMode.Disabled)
				stb.AppendFormat("{0}.columnResizeMode={1};\n", localVarName, HtmlConvertor.ToScript((int)SettingsBehavior.ColumnResizeMode));
			if(RenderHelper.HasFixedColumns)
				stb.AppendFormat("{0}.fixedColumnCount={1};\n", localVarName, FixedColumnCount);
			if(Settings.ShowFilterRow)
				stb.AppendFormat("{0}.autoFilterDelay='{1}';\n", localVarName, SettingsBehavior.AutoFilterRowInputDelay);
			if(Templates.FooterRow != null)
				stb.AppendFormat("{0}.hasFooterRowTemplate = true;\n", localVarName);
			if(IsAdaptivityEnabled())
				CreateSetAdaptiveModeScript(stb, localVarName);
			if(!Settings.ShowColumnHeaders)
				stb.Append(localVarName + ".visibleColumnIndices=" + HtmlConvertor.ToJSON(ColumnHelper.Leafs.Select(n => GetColumnGlobalIndex(n.Column)).ToList()) + ";\n");
			if(RenderHelper.RequireRenderFilterRowMenu) {
				Hashtable filterRowConditions = new Hashtable();
				foreach(var column in DataColumns) {
					var condition = column.Settings.AutoFilterCondition;
					if(condition == AutoFilterCondition.Default)
						condition = FilterHelper.GetDefaultAutoFilterCondition(column);
					filterRowConditions.Add(GetColumnGlobalIndex(column), (int)condition);
				}
				stb.AppendFormat("{0}.filterRowConditions={1};\n", localVarName, HtmlConvertor.ToJSON(filterRowConditions));
			}
			if(RenderHelper.HasAnySelectAllCheckbox) {
				CheckState? isSelectedAllRowsWithoutPage = Selection.IsSelectedAllRowsWithoutPage();
				object selectAllBtnStateWithoutPage = isSelectedAllRowsWithoutPage != null ? InternalCheckboxControl.GetCheckStateKey(isSelectedAllRowsWithoutPage.Value) : null;
				stb.AppendFormat("{0}.selectAllBtnStateWithoutPage={1};\n", localVarName, HtmlConvertor.ToScript(selectAllBtnStateWithoutPage));
				stb.AppendFormat("{0}.selectAllSettings={1};\n", localVarName, HtmlConvertor.ToJSON(GetCommandColumnsSelectAllSettings()));
			}
		}
		protected void CreateSetAdaptiveModeScript(StringBuilder stb, string localVarName) {
			Hashtable data = new Hashtable();
			data.Add("adaptivityMode", (int)SettingsAdaptivity.AdaptivityMode);
			data.Add("allowOnlyOneAdaptiveDetailExpanded", SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded);
			data.Add("hideDataCellsWindowInnerWidth", SettingsAdaptivity.HideDataCellsAtWindowInnerWidth);
			data.Add("adaptiveDetailColumnCount", SettingsAdaptivity.AdaptiveDetailColumnCount);
			data.Add("adaptiveColumnsOrder", ColumnHelper.AdaptiveColumns.Select(c => ColumnHelper.GetColumnGlobalIndex(c)).ToList());
			stb.AppendFormat("{0}.SetAdaptiveMode({1});\n", localVarName, HtmlConvertor.ToJSON(data));
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = base.GetClientObjectState();
			if(!RenderHelper.RequireEndlessPagingPartialLoad) {
				if(RenderHelper.AllowColumnResizing)
					result.Add(GridClientStateProperties.ResizingState, "");
			}
			return result;
		}
		protected internal bool IsAdaptivityEnabled() {
			if(Width.IsEmpty || Settings.HorizontalScrollBarMode != ScrollBarMode.Hidden)
				return false;
			return SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCells && SettingsBehavior.ColumnResizeMode == ColumnResizeMode.Disabled ||
				SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCellsWindowLimit;
		}
		protected override string ClientColumnName { get { return "ASPxClientGridViewColumn"; } }
		protected override object[] GetClientColumnArgs(IWebGridColumn column) {
			var col = column as GridViewColumn;
			var dataCol = column as GridViewDataColumn;
			string filterRowTypeKind = string.Empty;
			bool showFilterRowMenuLikeItem = false;
			if(dataCol != null && RenderHelper.RequireRenderFilterRowMenu) {
				FilterRowTypeKind kind = FilterHelper.GetFilterRowTypeKind(dataCol);
				filterRowTypeKind = BaseFilterHelper.GetFilterRowTypeKindSymbol(kind);
				showFilterRowMenuLikeItem = kind == FilterRowTypeKind.String && RenderHelper.IsFilterRowMenuLikeItemVisible(dataCol);
			}
			bool inCustWindow = ColumnHelper.LeafsForCustWindow.Contains(col) || ColumnHelper.BandsForCustWindow.Contains(col);
			return new object[] { 
				col.Name,
				GetColumnGlobalIndex(col),
				col.ParentBand != null ? GetColumnGlobalIndex(col.ParentBand) : -1,
				dataCol != null ? dataCol.FieldName : null,
				col.Visible,
				filterRowTypeKind,
				showFilterRowMenuLikeItem,
				col.GetAllowGroup(),
				col.GetAllowSort() || dataCol != null && dataCol.GroupIndex > -1 && col.GetAllowGroup() && !inCustWindow,
				col.GetAllowDragDrop(),
				dataCol != null ? dataCol.IsMultiSelectHeaderFilter : false,
				inCustWindow,
				col.GetColumnMinWidth(),
				col is GridViewCommandColumn
			};
		}
		internal IEnumerable GetCommandColumnsSelectAllSettings() {
			return AllColumns.OfType<GridViewCommandColumn>().Where(c => c.SelectAllCheckboxMode != GridViewSelectAllCheckBoxMode.None).Select(c => new {
				index = GetColumnGlobalIndex(c),
				mode = (int)c.SelectAllCheckboxMode,
				selectText = SettingsText.GetSelectAllCheckboxTooltip(c.SelectAllCheckboxMode),
				unselectText = SettingsText.GetUnselectAllCheckboxTooltip(c.SelectAllCheckboxMode)
			});
		}
		protected override void OnSummaryExists(CustomSummaryExistEventArgs e) {
			if(e.IsGroupSummary) {
				ASPxSummaryItem item = e.Item as ASPxSummaryItem;
				if(item != null && !string.IsNullOrEmpty(item.ShowInColumn)) {
					if(e.GroupLevel < SortedColumns.Count && !(SortedColumns[e.GroupLevel] as GridViewDataColumn).IsEquals(item.ShowInColumn)) { 
						e.Exists = false;
					}
				}
			}
		}
		protected internal List<ASPxSummaryItem> GetTotalSummaryItems(GridViewColumn column) {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem item in TotalSummary) {
				if(item.SummaryType == SummaryItemType.None)
					continue;
				if(!string.IsNullOrEmpty(item.ShowInColumn)) {
					if(!ShowSummaryItemInColumn(item.ShowInColumn, column))
						continue;
				} else {
					GridViewDataColumn dataColumn = column as GridViewDataColumn;
					if(dataColumn == null || item.FieldName != dataColumn.FieldName)
						continue;
				}
				res.Add(item);
			}
			return res;
		}
		protected internal List<ASPxSummaryItem> GetVisibleTotalSummaryItems(GridViewColumn column) {
			return GetTotalSummaryItems(column).Where(i => i.Visible).ToList();
		}
		protected internal List<ASPxSummaryItem> GetGroupFooterSummaryItems(GridViewColumn column) {
			List<ASPxSummaryItem> res = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem item in GroupSummary) {
				if(ShowSummaryItemInColumn(item.ShowInGroupFooterColumn, column))
					res.Add(item);
			}
			return res;
		}
		static bool ShowSummaryItemInColumn(string id, GridViewColumn column) {
			if(string.IsNullOrEmpty(id))
				return false;
			if(!string.IsNullOrEmpty(column.Name))
				return id == column.Name;
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(dataColumn != null && id == dataColumn.FieldName)
				return true;
			return id == column.Caption || id == column.ToString();
		}
		#region ISummaryItemsOwner Members
		ISummaryItem ISummaryItemsOwner.CreateItem(string fieldName, SummaryItemType summaryType) {
			return new ASPxSummaryItem(fieldName, summaryType);
		}
		string ISummaryItemsOwner.GetCaptionByFieldName(string fieldName) {
			return ColumnHelper.FindColumnByString(fieldName).ToString();
		}
		List<string> ISummaryItemsOwner.GetFieldNames() {
			List<string> result = new List<string>();
			foreach(GridViewDataColumn column in DataColumns) {
				result.Add(column.FieldName);
			}
			return result;
		}
		List<ISummaryItem> ISummaryItemsOwner.GetItems() {
			List<ISummaryItem> result = new List<ISummaryItem>();
			foreach(ASPxSummaryItem item in GroupSummary) {
				result.Add(item);
			}
			return result;
		}
		Type ISummaryItemsOwner.GetTypeByFieldName(string fieldName) {
			return DataProxy.GetFieldType(fieldName);
		}
		void ISummaryItemsOwner.SetItems(List<ISummaryItem> items) {
			BeginUpdate();
			try {
				GroupSummary.Clear();
				foreach(ISummaryItem item in items) {
					GroupSummary.Add(item as ASPxSummaryItem);
				}
			} finally {
				EndUpdate();
			}
		}
		#endregion
		protected override void OnKeyFieldNameChanged() {
			DetailRows.CollapseAllRows();
			base.OnKeyFieldNameChanged();
		}
		protected override bool AutoExpandAllGroupsInternal { get { return SettingsBehavior.AutoExpandAllGroups; } } 
		protected internal override int GroupCountInternal { get { return GroupCount; } } 
		protected override void ExpandAllInternal() { ExpandAll(); } 
		protected override bool AllowOnlyOneMasterRowExpandedInternal { get { return SettingsDetail.AllowOnlyOneMasterRowExpanded; } } 
		protected internal new GridViewEndlessPagingHelper EndlessPagingHelper { get { return base.EndlessPagingHelper as GridViewEndlessPagingHelper; } }
		protected internal new GridViewBatchEditHelper BatchEditHelper { get { return base.BatchEditHelper as GridViewBatchEditHelper; } }
		protected internal new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected internal new GridViewColumnHelper ColumnHelper { get { return base.ColumnHelper as GridViewColumnHelper; } }
		protected internal new GridViewFilterHelper FilterHelper { get { return base.FilterHelper as GridViewFilterHelper; } }
		protected internal new GridViewUpdatableContainer ContainerControl { get { return base.ContainerControl as GridViewUpdatableContainer; } }
		protected override ClientSideEventsBase CreateClientSideEvents() { return new GridViewClientSideEvents(); }
		protected virtual ASPxGridViewDetailSettings CreateSettingsDetail() { return new ASPxGridViewDetailSettings(this); }
		protected override ASPxGridTextSettings CreateSettingsText() { return new ASPxGridViewTextSettings(this); }
		protected override GridPopupControlStyles CreatePopupControlStyles() { return new GridViewPopupControlStyles(this); }
		protected override EditorImages CreateEditorImages() { return new GridViewEditorImages(this); }
		protected override EditorStyles CreateEditorStyles() { return new GridViewEditorStyles(this); }
		protected override PagerStyles CreatePagerStyles() { return new GridViewPagerStyles(this); }
		protected override ASPxGridPagerSettings CreateSettingsPager() { return new ASPxGridViewPagerSettings(this); }
		protected override ASPxGridEditingSettings CreateSettingsEditing() { return new ASPxGridViewEditingSettings(this); }
		protected override ASPxGridSettings CreateGridSettings() { return new ASPxGridViewSettings(this); }
		protected override ASPxGridBehaviorSettings CreateBehaviorSettings() { return new ASPxGridViewBehaviorSettings(this); }
		protected override ASPxGridCookiesSettings CreateCookiesSettings() { return new ASPxGridViewCookiesSettings(this); }
		protected override ASPxGridCommandButtonSettings CreateCommandButtonSettings() { return new ASPxGridViewCommandButtonSettings(this); }
		protected override ASPxGridPopupControlSettings CreatePopupSettings() { return new ASPxGridViewPopupControlSettings(this); }
		protected override ASPxGridSearchPanelSettings CreateSearchPanelSettings() { return new ASPxGridViewSearchPanelSettings(this); }
		protected override ASPxGridFilterControlSettings CreateFilterControlSettings() {return new ASPxGridViewFilterControlSettings(this); }
		protected override IList CreateSummaryItemCollection() { return new ASPxSummaryItemCollection(this); }
		protected override ASPxSummaryItemBase CreateTotalSummaryItem() { return new ASPxSummaryItem(); }
		protected override GridFormatConditionCollection CreateFormatConditions() { return new GridViewFormatConditionCollection(this, OnFormatConditionSummaryChanged); }
		protected override GridColumnCollection CreateColumnCollection() { return new GridViewColumnCollection(this); }
		protected override GridEndlessPagingHelper CreateEndlessPagingHelper() { return new GridViewEndlessPagingHelper(this); }
		protected override GridBatchEditHelper CreateBatchEditHelper() { return new GridViewBatchEditHelper(this); }
		protected override GridRenderHelper CreateRenderHelper() { return new GridViewRenderHelper(this); }
		protected override GridColumnHelper CreateColumnHelper() { return new GridViewColumnHelper(this); }
		protected override GridFilterHelper CreateFilterHelper() { return new GridViewFilterHelper(this); }
		protected override GridClientStylesInfo CreateClientStylesInfo() { return new GridViewClientStylesInfo(this); }
		protected override WebDataSelection CreateSelection(WebDataProxy proxy) { return new GridViewSelection(proxy); }
		protected override ASPxGridDataSecuritySettings CreateDataSecuritySettings() { return new ASPxGridViewDataSecuritySettings(this); }
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() { return new ASPxGridViewLoadingPanelSettings(this); }
		protected override ImagesBase CreateImages() { return new GridViewImages(this); }
		protected override StylesBase CreateStyles() { return new GridViewStyles(this); }
		protected override GridUpdatableContainer CreateContainerControl() { return new GridViewUpdatableContainer(this); }
		protected internal override ASPxGridPager CreatePagerControl(string id) { return new ASPxGridViewPager(this) { ID = id }; }
		protected internal virtual void OnGroupSummaryChanged(object sender, CollectionChangeEventArgs e) {
			OnSummaryChanged(sender, e);
		}
		protected override IStateManager[] GetStateManagedObjects() { 
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(GroupSummary);
			list.Add(SettingsCustomizationWindowInternal);
			list.Add(SettingsDetail);
			list.Add(SettingsText);
			list.Add(SettingsContextMenu);
			list.Add(SettingsAdaptivity);
			list.Add(StylesPopup);
			list.Add(StylesContextMenu);
			return list.ToArray();
		}
		public ReadOnlyCollection<GridViewDataColumn> GetSortedColumns() { return SortedColumns.OfType<GridViewDataColumn>().ToList().AsReadOnly(); }
		public ReadOnlyCollection<GridViewDataColumn> GetGroupedColumns() {
			int count = Math.Min(GroupCount, SortedColumns.Count);
			return GetSortedColumns().Take(count).ToList().AsReadOnly();
		}
		protected internal override IWebGridColumn FindColumnByName(string columnName) {
			return ColumnHelper.AllColumns[columnName];
		}
		protected internal override FormLayoutProperties GenerateDefaultLayout(bool fromControlDesigner) {
			var prop = new GridViewFormLayoutProperties();
			prop.ColCount = SettingsEditing.EditFormColumnCount;
			var layout = GridViewEditFormLayout.CreateLayout(this);
			var editItems = layout.SelectMany(l => l).Where(i => i.Type == GridViewEditFormLayoutItemType.Editor);
			GridUniqueColumnInfo info = fromControlDesigner ? new GridUniqueColumnInfo(editItems.Select(i => i.Column)) : null;
			foreach(var item in editItems) {
				GridViewColumnLayoutItem layoutItem;
				var column = item.Column;
				if(fromControlDesigner)
					layoutItem = new GridViewColumnLayoutItem() { ColumnName = info.GetUniqueColumnName(column) };
				else
					layoutItem = new GridViewColumnLayoutItem(column);
				prop.Items.Add(layoutItem);
				AssignColumnEditFormSettingsToLayoutItem(layoutItem, item, prop.ColCount);
			}
			prop.Items.AddCommandItem(new EditModeCommandLayoutItem() { ColSpan = prop.ColCount, HorizontalAlign = CommandLayoutItemDefaultHorizontalAlign });
			return prop;
		}
		protected internal FormLayoutProperties GenerateAdaptiveDefaultLayout(bool fromControlDesigner) {
			var prop = new GridViewFormLayoutProperties();
			prop.ColCount = SettingsAdaptivity.AdaptiveDetailColumnCount;
			GridUniqueColumnInfo info = fromControlDesigner ? new GridUniqueColumnInfo(ColumnHelper.Leafs.Select(i => i.Column)) : null;
			foreach(var leaf in ColumnHelper.Leafs) {
				GridViewColumnLayoutItem layoutItem;
				if(fromControlDesigner)
					layoutItem = new GridViewColumnLayoutItem() { ColumnName = info.GetUniqueColumnName(leaf.Column) };
				else
					layoutItem = new GridViewColumnLayoutItem(leaf.Column);
				prop.Items.Add(layoutItem);
			}
			return prop;
		}
		protected void AssignColumnEditFormSettingsToLayoutItem(ColumnLayoutItem layoutItem, GridViewEditFormLayoutItem item, int colCount) {
			var editSettings = item.Column.EditFormSettings;
			var captionLocation = item.CaptionLocation;
			if(!string.IsNullOrEmpty(editSettings.Caption))
				layoutItem.Caption = editSettings.Caption;
			if(captionLocation == ASPxColumnCaptionLocation.Top)
				layoutItem.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			if(captionLocation == ASPxColumnCaptionLocation.None)
				layoutItem.ShowCaption = Utils.DefaultBoolean.False;
			if(editSettings.ColumnSpan > 0)
				layoutItem.ColSpan = Math.Min(colCount, editSettings.ColumnSpan);
			if(editSettings.RowSpan > 0)
				layoutItem.RowSpan = editSettings.RowSpan;
		}
		#region Callbacks
		protected override void RegisterCallBacks(Dictionary<string, ASPxGridCallBackMethod> callBacks) {
			base.RegisterCallBacks(callBacks);
			callBacks[GridViewCallbackCommand.Group] = new ASPxGridCallBackMethod(CBGroup);
			callBacks[GridViewCallbackCommand.ColumnMove] = new ASPxGridCallBackMethod(CBMove); 
			callBacks[GridViewCallbackCommand.CollapseAll] = new ASPxGridCallBackMethod(CBCollapseAll);
			callBacks[GridViewCallbackCommand.ExpandAll] = new ASPxGridCallBackMethod(CBExpandAll);
			callBacks[GridViewCallbackCommand.ExpandRow] = new ASPxGridCallBackMethod(CBExpandRow);
			callBacks[GridViewCallbackCommand.CollapseRow] = new ASPxGridCallBackMethod(CBCollapseRow);
			callBacks[GridViewCallbackCommand.HideAllDetail] = new ASPxGridCallBackMethod(CBHideAllDetailRows);
			callBacks[GridViewCallbackCommand.ShowAllDetail] = new ASPxGridCallBackMethod(CBShowAllDetailRows);
			callBacks[GridViewCallbackCommand.ShowDetailRow] = new ASPxGridCallBackMethod(CBShowDetailRow);
			callBacks[GridViewCallbackCommand.HideDetailRow] = new ASPxGridCallBackMethod(CBHideDetailRow);
			callBacks[GridViewCallbackCommand.ApplyColumnFilter] = new ASPxGridCallBackMethod(CBApplyColumnFilter);
			callBacks[GridViewCallbackCommand.ApplyMultiColumnFilter] = new ASPxGridCallBackMethod(CBApplyMultiColumnAutoFilter);
			callBacks[GridViewCallbackCommand.FilterRowMenu] = new ASPxGridCallBackMethod(CBFilterRowMenu);
			callBacks[GridViewCallbackCommand.ContextMenu] = new ASPxGridCallBackMethod(CBContextMenu);
		}
		protected virtual void CBGroup(string[] args) {
			GridViewDataColumn column = ColumnHelper.FindColumnByKey(args[0]) as GridViewDataColumn;
			if(column == null) return;
			int groupIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			string order = args[2];
			if(groupIndex == -1) {
				column.UnGroup();
			} else {
				if(groupIndex == -2) groupIndex = column.GroupIndex < 0 ? GroupCount : column.GroupIndex;
				GroupBy(column, groupIndex);
				column.SortOrder = GetSortOrder(column, order);
			}
		}
		protected void CBMove(string[] args) {
			GridViewColumn column = ColumnHelper.FindColumnByKey(args[0]);
			GridViewColumn moveToColumn = ColumnHelper.FindColumnByKey(args[1]);
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			GridViewDataColumn moveToDataColumn = moveToColumn as GridViewDataColumn;
			if(column == null) return;
			bool moveBefore = GetBoolArg(GetArg(args, 2), true),
				 moveToGroup = GetBoolArg(GetArg(args, 3), false),
				 moveFromGroup = GetBoolArg(GetArg(args, 4), false);
			if(moveToColumn != null && Object.Equals(column.ParentBand, moveToColumn)) {
				GridColumnVisualTreeNode bandNode = ColumnHelper.FindVisualTreeNode(moveToColumn);
				if(bandNode.Children.Count > 0) {
					int index = moveBefore ? 0 : bandNode.Children.Count - 1;
					moveToColumn = (GridViewColumn)bandNode.Children[index].Column;
				} else {
					moveToColumn = (moveToColumn as GridViewBandColumn).Columns[0];
				}
				moveToDataColumn = null;
			}
			ColumnMoveCore(column, moveToColumn, dataColumn, moveToDataColumn, moveBefore, moveToGroup, moveFromGroup);
			ResetVisibleColumnsRecursive(this);
		}
		protected virtual void ColumnMoveCore(GridViewColumn column, GridViewColumn moveToColumn, GridViewDataColumn dataColumn, GridViewDataColumn moveToDataColumn, bool moveBefore, bool moveToGroup, bool moveFromGroup) {
			if(moveToGroup && dataColumn != null) {
				ColumnMoveToGroupBy(dataColumn, moveToDataColumn, moveBefore);
			} else {
				ColumnMoveTo(column, moveToColumn, dataColumn, moveBefore, moveFromGroup);
			}
		}
		void ColumnMoveTo(GridViewColumn column, GridViewColumn moveToColumn, GridViewDataColumn dataColumn, bool moveBefore, bool moveFromGroup) {
			if(dataColumn != null && dataColumn.GroupIndex > -1 && moveFromGroup)
				dataColumn.UnGroup();
			column.VisibleIndex = GetMovedColumnNewVisibleIndex(column, moveToColumn, moveBefore);
		}
		protected int GetMovedColumnNewVisibleIndex(GridViewColumn sourceColumn, GridViewColumn targetColumn, bool moveBefore) {
			if(targetColumn == null)
				return -1;
			int result = targetColumn.VisibleIndex;
			if(RenderHelper.HasFixedColumns && sourceColumn.ParentBand == null && targetColumn.ParentBand == null) {
				bool sourceFixed = sourceColumn.FixedStyle == GridViewColumnFixedStyle.Left;
				bool targetFixed = targetColumn.FixedStyle == GridViewColumnFixedStyle.Left;
				if(sourceFixed && !targetFixed) {
					GridViewColumn column = GetLastVisibleFixedColumn();
					result = column != null ? column.VisibleIndex : -1;
					moveBefore = false;
				}
				if(!sourceFixed && targetFixed) {
					GridViewColumn column = GetFirstVisibleUnFixedColumn();
					result = column != null ? column.VisibleIndex : -1;
					moveBefore = true;
				}
			}
			if(sourceColumn.VisibleIndex < result && sourceColumn.VisibleIndex > -1)
				result--;
			if(!moveBefore) result++;
			if(result < 0) result = 0;
			return result;
		}
		GridViewColumn GetLastVisibleFixedColumn() {
			GridViewColumn result = null;
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Layout[0]) {
				if(node.Column.FixedStyle == GridViewColumnFixedStyle.Left)
					result = node.Column;
				else
					break;
			}
			return result;
		}
		GridViewColumn GetFirstVisibleUnFixedColumn() {
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Layout[0])
				if(node.Column.FixedStyle != GridViewColumnFixedStyle.Left)
					return node.Column;
			return null;
		}
		void ColumnMoveToGroupBy(GridViewDataColumn dataColumn, GridViewDataColumn moveToDataColumn, bool moveBefore) {
			int groupIndex = GroupCount;
			if(moveToDataColumn != null && moveToDataColumn.GroupIndex > -1) {
				groupIndex = moveToDataColumn.GroupIndex;
				if(!moveBefore) groupIndex++;
			}
			if(!dataColumn.Visible)
				dataColumn.Visible = true;
			dataColumn.GroupIndex = groupIndex;
		}
		protected virtual void CBExpandAll(string[] args) {
			ExpandAll();
		}
		protected virtual void CBCollapseAll(string[] args) {
			CollapseAll();
		}
		protected virtual void CBExpandRow(string[] args) {
			int visibleIndex;
			if(!Int32.TryParse(args[0], out visibleIndex)) return;
			bool recursive = GetBoolArg(GetArg(args, 1), false);
			ChangeGroupRowExpandedState(visibleIndex, recursive, true);
		}
		protected virtual void CBCollapseRow(string[] args) {
			int visibleIndex;
			if(!Int32.TryParse(args[0], out visibleIndex)) return;
			bool recursive = GetBoolArg(GetArg(args, 1), false);
			ChangeGroupRowExpandedState(visibleIndex, recursive, false);
		}
		void ChangeGroupRowExpandedState(int visibleIndex, bool recursive, bool expand) {
			int offset = 0;
			var command = expand ? GridViewCallbackCommand.ExpandRow : GridViewCallbackCommand.CollapseRow;
			if(RenderHelper.UseEndlessPaging)
				visibleIndex = EndlessPagingHelper.ValidateVisibleIndex(command, visibleIndex, ref offset);
			if(expand)
				ExpandRow(visibleIndex, recursive);
			else
				CollapseRow(visibleIndex, recursive);
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(command, visibleIndex, offset);
		}
		protected void CBShowAllDetailRows(string[] args) {
			DetailRows.ExpandAllRows();
		}
		protected void CBHideAllDetailRows(string[] args) {
			DetailRows.CollapseAllRows();
		}
		protected void CBShowDetailRow(string[] args) {
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if(key == null) return;
			ChangeDetailRowExpandedState(FindVisibleIndexByKeyValue(key), true);
		}
		protected void CBHideDetailRow(string[] args) {
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if(key == null) return;
			ChangeDetailRowExpandedState(FindVisibleIndexByKeyValue(key), false);
		}
		void ChangeDetailRowExpandedState(int visibleIndex, bool expanded) {
			int offset = 0;
			var command = expanded ? GridViewCallbackCommand.ShowDetailRow : GridViewCallbackCommand.HideDetailRow;
			if(RenderHelper.UseEndlessPaging)
				visibleIndex = EndlessPagingHelper.ValidateVisibleIndex(command, visibleIndex, ref offset);
			bool oldExpanded = DetailRows.IsVisible(visibleIndex);
			if(expanded)
				DetailRows.ExpandRow(visibleIndex);
			else
				DetailRows.CollapseRow(visibleIndex);
			bool newState = DetailRows.IsVisible(visibleIndex);
			if(oldExpanded != newState) {
				ASPxGridViewDetailRowEventArgs e = new ASPxGridViewDetailRowEventArgs(visibleIndex, newState);
				RaiseDetailRowExpandedChanged(e);
			}
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(command, visibleIndex, offset);
		}
		protected virtual void CBApplyColumnFilter(string[] args) {
			GridViewDataColumn column = ColumnHelper.FindColumnByKey(args[0]) as GridViewDataColumn;
			if(column == null) return;
			column.AutoFilterBy(string.Join("|", args, 1, args.Length - 1));
		}
		protected virtual void CBApplyMultiColumnAutoFilter(string[] args) {
			Dictionary<GridViewDataColumn, string> filterValues = new Dictionary<GridViewDataColumn, string>();
			for(int i = 0; i < args.Length; i += 3) {
				GridViewDataColumn column = ColumnHelper.FindColumnByKey(args[i]) as GridViewDataColumn;
				if(column == null)
					continue;
				filterValues.Add(column, args[i + 1]);
				if(!string.IsNullOrEmpty(args[i + 2]))
					column.Settings.AutoFilterCondition = (AutoFilterCondition)Int32.Parse(args[i + 2]);
			}
			var filterCriterias = FilterHelper.CreateMultiColumnAutoFilter(filterValues);
			foreach(string fieldName in filterCriterias.Keys)
				ColumnFilterInfo[new OperandProperty(fieldName)] = filterCriterias[fieldName];
			FilterExpression = CriteriaOperator.ToString(GroupOperator.And(ColumnFilterInfo.Values));
		}
		protected virtual void CBFilterRowMenu(string[] args) {
			GridViewDataColumn column = ColumnHelper.FindColumnByKey(args[0]) as GridViewDataColumn;
			if(column == null) return;
			string filterValue = RenderHelper.GetColumnAutoFilterText(column);
			column.Settings.AutoFilterCondition = (AutoFilterCondition)Int32.Parse(args[1]);
			if(!RenderHelper.AllowMultiColumnAutoFilter) {
				column.AutoFilterBy(filterValue);
			} else if(args.Length > 2) {
				string[] arguments = new string[args.Length - 2];
				for(int i = 2; i < args.Length; ++i)
					arguments[i - 2] = args[i];
				CBApplyMultiColumnAutoFilter(arguments);
			}
		}
		protected void CBContextMenu(string[] args) {
			switch(args[0]) {
				case "ItemClick":
					ContextMenuItemClickHandler((GridViewContextMenuType)int.Parse(args[1]), args[2], int.Parse(args[3]));
					break;
				case "ShowGroupPanel":
					Settings.ShowGroupPanel = int.Parse(args[1]) != 0;
					break;
				case "ShowSearchPanel":
					SettingsSearchPanel.Visible = int.Parse(args[1]) != 0;
					break;
				case "ShowFilterRow":
					Settings.ShowFilterRow = int.Parse(args[1]) != 0;
					break;
				case "ShowFilterRowMenu":
					Settings.ShowFilterRowMenu = int.Parse(args[1]) != 0;
					break;
				case "ClearGrouping":
					ClearGrouping();
					break;
				case "ShowFooter":
					Settings.ShowFooter = int.Parse(args[1]) != 0;
					break;
				case "SetSummary":
					CheckContextMenuSummaryItem(args);
					break;
				case "ClearSummary":
					UncheckAllContextMenuSummaryItems(args);
					break;
			}
		}
		void ClearGrouping() {
			BeginUpdate();
			foreach(var column in DataColumns)
				column.UnGroup();
			EndUpdate();
		}
		void CheckContextMenuSummaryItem(string[] args) {
			if(args.Length != 4) return;
			var column = ColumnHelper.FindColumnByKey(args[1]) as GridViewDataColumn;
			if(column == null) return;
			var summaryType = (SummaryItemType)int.Parse(args[2]);
			var check = int.Parse(args[3]) != 0;
			var summaryItems = GetTotalSummaryItems(column);
			var item = summaryItems.FirstOrDefault(i => i.SummaryType == summaryType);
			if(check) {
				if(item == null)
					CreateSummaryItemViaContextMenu(column, summaryType);
				else
					item.Visible = true;
			} else if(item != null) {
				UncheckContextMenuSummaryItem(item);
			}
		}
		void UncheckContextMenuSummaryItem(ASPxSummaryItem item) {
			if(item.CreatedByClient)
				item.Collection.Remove(item);
			else
				item.Visible = false;
		}
		void UncheckAllContextMenuSummaryItems(string[] args) {
			if(args.Length != 2) return;
			var column = ColumnHelper.FindColumnByKey(args[1]) as GridViewDataColumn;
			if(column == null) return;
			foreach(var item in GetTotalSummaryItems(column))
				UncheckContextMenuSummaryItem(item);
		}
		protected virtual void CreateSummaryItemViaContextMenu(GridViewDataColumn column, SummaryItemType summaryType) {
			var item = new ASPxSummaryItem(column.FieldName, summaryType) { CreatedByClient = true };
			TotalSummary.Add(item);
			RaiseAddSummaryItemViaContextMenu(new ASPxGridViewAddSummaryItemViaContextMenuEventArgs(item, column));
		}
		void ContextMenuItemClickHandler(GridViewContextMenuType menuType, string itemPath, int elementIndex) { 
			var menu = RenderHelper.GetContextMenu(menuType);
			if(menu == null)
				return;
			var item = (GridViewContextMenuItem)menu.GetItemByIndexPath(itemPath);
			if(item == null)
				return;
			RaiseContextMenuItemClick(new ASPxGridViewContextMenuItemClickEventArgs(menuType, item, elementIndex));
		}
		#endregion
		#region Templates
		protected override Dictionary<string, object> GetEditTemplateValuesCore() {
			if(!IsEditing) return null;
			Dictionary<object, ITemplate> columnEditItems = GetColumnEditItems();
			Dictionary<object, ITemplate> formLayoutEditItems = GetFormLayoutEditItems();
			Dictionary<string, object> cellValues = RenderHelper.EditRowCellTemplates.FindTwoWayBindings(columnEditItems, new TemplateContainerFinder(FindEditRowCellTemplate));
			Dictionary<string, object> formValues = RenderHelper.EditFormTemplates.FindTwoWayBindings(Templates.EditForm);
			Dictionary<string, object> formLayoutItemValues = RenderHelper.FormLayoutItemTemplates.FindTwoWayBindings(formLayoutEditItems, new TemplateContainerFinder(FindFormLayoutItemTemplate));
			return MergeDictionaries(cellValues, formValues, formLayoutItemValues);
		}
		Dictionary<object, ITemplate> GetColumnEditItems() {
			Dictionary<object, ITemplate> editItems = new Dictionary<object, ITemplate>();
			foreach(GridViewDataColumn column in DataColumns) {
				if(column.EditItemTemplate == null) continue;
				editItems[column] = column.EditItemTemplate;
			}
			return editItems;
		}
		public Control FindDetailRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.DetailRowTemplates.FindChild(new TemplateContainerFinder(FindDetailRowTemplate), visibleIndex, id);
		}
		public Control FindPreviewRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.PreviewRowTemplates.FindChild(new TemplateContainerFinder(FindPreviewRowTemplate), visibleIndex, id);
		}
		public Control FindGroupRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.GroupRowTemplates.FindChild(new TemplateContainerFinder(FindGroupRowTemplate), visibleIndex, id);
		}
		public Control FindRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.DataRowTemplates.FindChild(new TemplateContainerFinder(FindDataRowTemplateByVIndex), visibleIndex, id);
		}
		public Control FindRowTemplateControlByKey(object rowKey, string id) {
			return RenderHelper.DataRowTemplates.FindChild(new TemplateContainerFinder(FindDataRowTemplateByKey), rowKey, id);
		}
		public Control FindRowCellTemplateControl(int visibleIndex, GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.RowCellTemplates.FindChild(new TemplateContainerFinder(FindRowTemplateByVIndex), new RowFindParams(gridViewDataColumn, visibleIndex), id);
		}
		public Control FindRowCellTemplateControlByKey(object rowKey, GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.RowCellTemplates.FindChild(new TemplateContainerFinder(FindRowTemplateByKey), new RowFindParams(gridViewDataColumn, rowKey), id);
		}
		public Control FindEmptyDataRowTemplateControl(string id) {
			return RenderHelper.EmptyDataRowTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindFilterCellTemplateControl(GridViewColumn column, string id) {
			return RenderHelper.FilterTemplates.FindChild(new TemplateContainerFinder(FindFilterCellTemplate), column, id);
		}
		public Control FindFilterRowTemplateControl(string id) {
			return RenderHelper.FilterRowTemplates.FindChild(new TemplateContainerFinder(FindFilterRowTemplate), null, id);
		}
		public Control FindFooterRowTemplateControl(string id) {
			return RenderHelper.FooterRowTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindFooterCellTemplateControl(GridViewColumn column, string id) {
			return RenderHelper.FooterCellTemplates.FindChild(new TemplateContainerFinder(FindFooterCellTemplate), column, id);
		}
		public Control FindGroupFooterCellTemplateControl(int visibleIndex, GridViewColumn column, string id) {
			return RenderHelper.GroupFooterCellTemplates.FindChild(new TemplateContainerFinder(FindGroupFooterCellTemplate), new RowFindParams(column, visibleIndex), id);
		}
		public Control FindGroupFooterRowTemplateControl(int visibleIndex, string id) {
			return RenderHelper.GroupFooterRowTemplates.FindChild(new TemplateContainerFinder(FindGroupFooterRowTemplate), visibleIndex, id);
		}
		public Control FindEditRowCellTemplateControl(GridViewDataColumn gridViewDataColumn, string id) {
			return RenderHelper.EditRowCellTemplates.FindChild(new TemplateContainerFinder(FindEditRowCellTemplate), gridViewDataColumn, id);
		}
		public Control FindHeaderTemplateControl(GridViewColumn column, string id) {
			return RenderHelper.HeaderTemplates.FindChild(new TemplateContainerFinder(FindHeaderTemplate), column, id);
		}
		public Control FindTitleTemplateControl(string id) {
			return RenderHelper.TitleTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindStatusBarTemplateControl(string id) {
			return RenderHelper.StatusBarTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindPagerBarTemplateControl(string id, GridViewPagerBarPosition position) {
			return RenderHelper.PagerBarTemplates.FindChild(new TemplateContainerFinder(FindPagerBarTemplate), position, id);
		}
		public Control FindEditFormTemplateControl(string id) {
			return RenderHelper.EditFormTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		public Control FindEditFormLayoutItemTemplateControl(string id) {
			return RenderHelper.FormLayoutItemTemplates.FindChild(new TemplateContainerFinder(FindSingleControlTemplate), null, id);
		}
		Control FindDataRowTemplateByVIndex(Control container, object parameters, string id) {
			GridViewDataRowTemplateContainer row = (GridViewDataRowTemplateContainer)container;
			if((row.VisibleIndex != (int)parameters)) return null;
			return row.FindControl(id);
		}
		Control FindDataRowTemplateByKey(Control container, object parameters, string id) {
			GridViewDataRowTemplateContainer row = (GridViewDataRowTemplateContainer)container;
			if(!object.Equals(row.KeyValue, parameters)) return null;
			return row.FindControl(id);
		}
		Control FindDetailRowTemplate(Control container, object parameters, string id) {
			GridViewDetailRowTemplateContainer row = (GridViewDetailRowTemplateContainer)container;
			if(row.VisibleIndex != (int)parameters) return null;
			return row.FindControl(id);
		}
		Control FindPreviewRowTemplate(Control container, object parameters, string id) {
			GridViewPreviewRowTemplateContainer row = (GridViewPreviewRowTemplateContainer)container;
			if(row.VisibleIndex != (int)parameters) return null;
			return row.FindControl(id);
		}
		Control FindGroupRowTemplate(Control container, object parameters, string id) {
			GridViewGroupRowTemplateContainer group = (GridViewGroupRowTemplateContainer)container;
			if(group.VisibleIndex != (int)parameters) return null;
			return group.FindControl(id);
		}
		class RowFindParams {
			public object Key;
			public GridViewColumn Column;
			public RowFindParams(GridViewColumn column, object key) {
				this.Column = column;
				this.Key = key;
			}
		}
		Control FindRowTemplateByVIndex(Control container, object parameters, string id) {
			RowFindParams findParams = (RowFindParams)parameters;
			GridViewDataItemTemplateContainer item = (GridViewDataItemTemplateContainer)container;
			if((findParams.Column == null || findParams.Column == item.Column) && item.VisibleIndex == (int)findParams.Key) return item.FindControl(id);
			return null;
		}
		Control FindRowTemplateByKey(Control container, object parameters, string id) {
			RowFindParams findParams = (RowFindParams)parameters;
			GridViewDataItemTemplateContainer item = (GridViewDataItemTemplateContainer)container;
			if((findParams.Column == null || findParams.Column == item.Column) && object.Equals(item.KeyValue, findParams.Key)) return item.FindControl(id);
			return null;
		}
		Control FindEditRowCellTemplate(Control container, object parameters, string id) {
			GridViewEditItemTemplateContainer item = (GridViewEditItemTemplateContainer)container;
			if(parameters != null && item.Column != parameters) return null;
			if(id == null) return item;
			return item.FindControl(id);
		}
		Control FindFormLayoutItemTemplate(Control container, object parameters, string id) {
			GridViewEditFormLayoutItemTemplateContainer item = (GridViewEditFormLayoutItemTemplateContainer)container;
			return item.LayoutItem.Path == parameters.ToString() ? item : null;
		}
		Control FindHeaderTemplate(Control container, object parameters, string id) {
			GridViewHeaderTemplateContainer header = (GridViewHeaderTemplateContainer)container;
			if(header.Column != parameters) return null;
			return header.FindControl(id);
		}
		Control FindFilterCellTemplate(Control container, object parameters, string id) {
			GridViewFilterCellTemplateContainer filterCell = (GridViewFilterCellTemplateContainer)container;
			if(filterCell.Column != parameters)
				return null;
			return filterCell.FindControl(id);
		}
		Control FindFilterRowTemplate(Control container, object parameters, string id) {
			return container.FindControl(id);
		}
		Control FindFooterCellTemplate(Control container, object parameters, string id) {
			GridViewFooterCellTemplateContainer footerCell = (GridViewFooterCellTemplateContainer)container;
			if(parameters != null && footerCell.Column != parameters) return null;
			return footerCell.FindControl(id);
		}
		Control FindGroupFooterCellTemplate(Control container, object parameters, string id) {
			RowFindParams findParams = (RowFindParams)parameters;
			GridViewGroupFooterCellTemplateContainer item = (GridViewGroupFooterCellTemplateContainer)container;
			if((findParams.Column == null || findParams.Column == item.Column) && item.VisibleIndex == (int)findParams.Key)
				return item.FindControl(id);
			return null;
		}
		Control FindGroupFooterRowTemplate(Control container, object parameters, string id) {
			GridViewGroupFooterRowTemplateContainer row = (GridViewGroupFooterRowTemplateContainer)container;
			if(row.VisibleIndex == (int)parameters)
				return row.FindControl(id);
			return null;
		}
		Control FindPagerBarTemplate(Control container, object parameters, string id) {
			GridViewPagerBarTemplateContainer pagerBar = (GridViewPagerBarTemplateContainer)container;
			if(pagerBar.Position != (GridViewPagerBarPosition)parameters) return null;
			return pagerBar.FindControl(id);
		}
		Control FindSingleControlTemplate(Control container, object parameters, string id) {
			return container.FindControl(id);
		}
		#endregion
	}
}
