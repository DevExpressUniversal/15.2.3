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

using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public enum PagerAlign { Left, Center, Right, Justify }
	public enum PagerPanelPosition { Top, Bottom }
	public enum PagerPanelTemplatePosition { Left, Right }
	public enum Layout { Table, Flow }
	[DXWebToolboxItem(true), DefaultProperty("DataSourceID"), DefaultEvent("PageIndexChanged"),
	Designer("DevExpress.Web.Design.ASPxDataViewControlDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public abstract class ASPxDataViewBase : ASPxDataWebControl, IRequiresLoadPostDataControl, IPagerOwner {
		protected internal const string DataViewScriptResourceName = WebScriptsResourcePath + "DataView.js";
		protected internal const string PagerClickHandlerName = "ASPx.DVPagerClick('{0}', '{1}')";
		protected internal const string PagerPageSizeChangeHandlerName = "function(s, e) {{ ASPx.DVPagerClick('{0}', e.value); }}";
		protected internal const string PagerPanelSpacingClassAffix = "PPSpacing";
		protected internal const string EndlessPagingHtmlKey = "epHtml";
		protected internal const string PageIndexStateKey = "pageIndex";
		protected internal const string PageSizeStateKey = "pageSize";
		protected internal const string PageCountStateKey = "pageCount";
		protected internal const string LayoutStateKey = "layout";
		protected internal const string EndlessPagingModeStateKey = "endlessPagingMode";
		protected const string AllPagesSavedPageIndexFieldName = "aspi";
		protected const string PageCountFieldName = "pc";
		protected const string PageSizeFieldName = "ps";
		protected const string DataItemCountFieldName = "ic";
		protected const string IsItemsBoundFieldName = "b";
		protected static Dictionary<PagerPanelPosition, string> fPagerPanelPositionIDSuffixes = new Dictionary<PagerPanelPosition, string>();
		protected static Dictionary<PagerPanelTemplatePosition, string> fPagerPanelTemplatePositionIDSuffixes = new Dictionary<PagerPanelTemplatePosition, string>();
		protected static Dictionary<PagerAlign, HorizontalAlign> fPagerAlignHorizontalAlign = new Dictionary<PagerAlign, HorizontalAlign>();
		private int fAllPagesSavedPageIndex = 0;
		private int initialRowPerPage;
		private int fPageIndex = 0;
		private int fPageSize = 0;
		private int fPageCount = 0;
		private int fDataItemCount = 0;
		private bool isItemsBound = false;
		private enum DataViewCallbackType { Custom, PagerClick };
		private DataViewCallbackType dataViewCallbackType;
		bool requireItemCountRecheck;
		DataViewEndlessPagingHelper endlessPagingHelper;
		private DataViewItemCollection dataItems = null;
		private ArrayList visibleItemsList = null;
		protected PagerSettingsEx fPagerSettings = null;
		protected DataViewFlowLayoutSettings fFlowLayoutSettings = null;
		protected DataViewTableLayoutSettings fTableLayoutSettings = null;
		internal bool fDataBound = false;
		private ITemplate fPagerPanelLeftTemplate = null;
		private ITemplate fPagerPanelRightTemplate = null;
		private ITemplate emptyDataTemplate;
		private DVMainControl fMainControl = null;
		private static readonly object EventPagerPanelCommand = new object();
		private static readonly object EventPageIndexChanging = new object();
		private static readonly object EventPageIndexChanged = new object();
		private static readonly object EventPageSizeChanging = new object();
		private static readonly object EventPageSizeChanged = new object();
		private static readonly object EventCallback = new object();
		static ASPxDataViewBase() {
			fPagerPanelPositionIDSuffixes.Add(PagerPanelPosition.Top, "T");
			fPagerPanelPositionIDSuffixes.Add(PagerPanelPosition.Bottom, "B");
			fPagerPanelTemplatePositionIDSuffixes.Add(PagerPanelTemplatePosition.Left, "L");
			fPagerPanelTemplatePositionIDSuffixes.Add(PagerPanelTemplatePosition.Right, "R");
			fPagerAlignHorizontalAlign.Add(PagerAlign.Left, HorizontalAlign.Left);
			fPagerAlignHorizontalAlign.Add(PagerAlign.Center, HorizontalAlign.Center);
			fPagerAlignHorizontalAlign.Add(PagerAlign.Right, HorizontalAlign.Right);
			fPagerAlignHorizontalAlign.Add(PagerAlign.Justify, HorizontalAlign.Justify);
		}
		public ASPxDataViewBase()
			: base() {
			EnableCallBacks = true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseAllButtonPageCount"),
#endif
		Category("Paging"), DefaultValue(0), AutoFormatDisable]
		public int AllButtonPageCount {
			get { return GetIntProperty("AllButtonPageCount", 0); }
			set {
				SetIntProperty("AllButtonPageCount", 0, value);
				DataLayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseAllowPaging"),
#endif
		Category("Paging"), DefaultValue(true), AutoFormatDisable]
		public bool AllowPaging {
			get { return GetBoolProperty("AllowPaging", true); }
			set { SetBoolProperty("AllowPaging", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEnableCallBacks"),
#endif
		DefaultValue(true), Category("Behavior"), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set {
				EnableCallBacksInternal = value;
				base.AutoPostBackInternal = !EnableCallBacks;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEnablePagingCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public bool EnablePagingCallbackAnimation {
			get { return EnableSlideCallbackAnimationInternal; }
			set { EnableSlideCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEnablePagingGestures"),
#endif
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnablePagingGestures {
			get { return EnableSwipeGesturesInternal; }
			set { EnableSwipeGesturesInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseItemSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ItemSpacing {
			get { return ((DataViewStyle)ControlStyle).ItemSpacing; }
			set {
				UnitUtils.CheckNegativeUnit(value, "ItemSpacing");
				((DataViewStyle)ControlStyle).ItemSpacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerPanelSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit PagerPanelSpacing {
			get { return ((DataViewStyle)ControlStyle).PagerPanelSpacing; }
			set {
				UnitUtils.CheckNegativeUnit(value, "PagerPanelSpacing");
				((DataViewStyle)ControlStyle).PagerPanelSpacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerAlign"),
#endif
		Category("Paging"), DefaultValue(PagerAlign.Center), AutoFormatEnable]
		public virtual PagerAlign PagerAlign {
			get { return (PagerAlign)GetEnumProperty("PagerAlign", PagerAlign.Center); }
			set {
				SetEnumProperty("PagerAlign", PagerAlign.Center, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePageIndex"),
#endif
		Category("Paging"), DefaultValue(0), AutoFormatDisable]
		public int PageIndex {
			get { return fPageIndex; }
			set {
				CommonUtils.CheckMinimumValue(value, -1, "PageIndex");
				SetPageIndex(value);
				DataLayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseAlwaysShowPager"),
#endif
		Category("Paging"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool AlwaysShowPager {
			get { return GetBoolProperty("AlwaysShowPager", false); }
			set { SetBoolProperty("AlwaysShowPager", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseAccessibilityCompliant"),
#endif
		 Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEmptyDataText"),
#endif
		Category("Misc"), DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string EmptyDataText {
			get { return GetStringProperty("EmptyDataText", ""); }
			set { SetStringProperty("EmptyDataText", "", value); }
		}
		[
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
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
	DevExpressWebLocalizedDescription("ASPxDataViewBaseImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewContentStyle ContentStyle {
			get { return Styles.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewItemStyle ItemStyle {
			get { return Styles.Item; }
		}
		[
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected DataViewEmptyItemStyle EmptyItemStyle {
			get { return Styles.EmptyItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerStyle PagerStyle {
			get { return Styles.Pager; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle PagerButtonStyle {
			get { return Styles.PagerButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerCurrentPageNumberStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle PagerCurrentPageNumberStyle {
			get { return Styles.PagerCurrentPageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerDisabledButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle PagerDisabledButtonStyle {
			get { return Styles.PagerDisabledButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerPageNumberStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle PagerPageNumberStyle {
			get { return Styles.PagerPageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerSummaryStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle PagerSummaryStyle {
			get { return Styles.PagerSummary; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerPageSizeItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerPageSizeItemStyle PagerPageSizeItemStyle {
			get { return Styles.PagerPageSizeItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewContentStyle PagerPanelStyle {
			get { return Styles.PagerPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerShowMoreItemsContainerStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewPagerShowMoreItemsContainerStyle PagerShowMoreItemsContainerStyle {
			get { return Styles.PagerShowMoreItemsContainer; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseEmptyDataStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewEmptyDataStyle EmptyDataStyle {
			get { return Styles.EmptyData; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DataViewPagerPanelTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PagerPanelLeftTemplate {
			get { return fPagerPanelLeftTemplate; }
			set {
				fPagerPanelLeftTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DataViewPagerPanelTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate PagerPanelRightTemplate {
			get { return fPagerPanelRightTemplate; }
			set {
				fPagerPanelRightTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DataViewTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EmptyDataTemplate {
			get { return emptyDataTemplate; }
			set {
				emptyDataTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePagerPanelCommand"),
#endif
		Category("Action")]
		public event DataViewPagerPanelCommandEventHandler PagerPanelCommand
		{
			add { Events.AddHandler(EventPagerPanelCommand, value); }
			remove { Events.RemoveHandler(EventPagerPanelCommand, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePageIndexChanging"),
#endif
		Category("Action")]
		public event DataViewPageEventHandler PageIndexChanging
		{
			add { Events.AddHandler(EventPageIndexChanging, value); }
			remove { Events.RemoveHandler(EventPageIndexChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePageIndexChanged"),
#endif
		Category("Action")]
		public event EventHandler PageIndexChanged
		{
			add { Events.AddHandler(EventPageIndexChanged, value); }
			remove { Events.RemoveHandler(EventPageIndexChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePageSizeChanging"),
#endif
		Category("Action")]
		public event DataViewPageSizeEventHandler PageSizeChanging
		{
			add { Events.AddHandler(EventPageSizeChanging, value); }
			remove { Events.RemoveHandler(EventPageSizeChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBasePageSizeChanged"),
#endif
		Category("Action")]
		public event EventHandler PageSizeChanged
		{
			add { Events.AddHandler(EventPageSizeChanged, value); }
			remove { Events.RemoveHandler(EventPageSizeChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewBaseCustomCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase CustomCallback
		{
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataViewBaseBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PageCount {
			get {
				if (IsItemsBound)
					return fPageCount;
				if (!AllowPaging)
					return 1;
				return DataItemCount / ItemsPerPage + (DataItemCount % ItemsPerPage > 0 ? 1 : 0);
			}
		}
		protected int RowPerPageInternal {
			get { return SettingsTableLayoutInternal.RowsPerPage; }
			set { SettingsTableLayoutInternal.RowsPerPage = value; }
		}
		protected internal DVMainControl MainControl { get { return fMainControl; } }
		protected DataViewStyles Styles {
			get { return StylesInternal as DataViewStyles; }
		}
		protected internal Layout LayoutInternal {
			get { return (Layout)GetEnumProperty("LayoutInternal", Layout.Table); }
			set {
				if(LayoutInternal == value)
					return;
				SetEnumProperty("LayoutInternal", Layout.Table, value);
				DataLayoutChanged();
			}
		}
		protected internal DataViewFlowLayoutSettings SettingsFlowLayoutInternal {
			get {
				if(fFlowLayoutSettings == null)
					fFlowLayoutSettings = CreateFlowLayoutSettings();
				return fFlowLayoutSettings;
			}
		}
		protected internal DataViewTableLayoutSettings SettingsTableLayoutInternal {
			get {
				if(fTableLayoutSettings == null)
					fTableLayoutSettings = CreateTableLayoutSettings();
				return fTableLayoutSettings;
			}
		}
		protected internal PagerSettingsEx PagerSettings {
			get {
				if(fPagerSettings == null)
					fPagerSettings = CreatePagerSettings(this);
				return fPagerSettings;
			}
		}
		protected internal IDataViewEndlessPagingSettigns EndlessPagingSettings {
			get { return PagerSettings as IDataViewEndlessPagingSettigns; }
		}
		protected internal int AllPagesSavedPageIndex {
			get { return fAllPagesSavedPageIndex; }
		}
		protected internal virtual int ColumnCountInternal {
			get { return SettingsTableLayoutInternal.ColumnCount; }
		}
		protected internal virtual int PageSizeInternal {
			get { return fPageSize; }
			set { fPageSize = value; }
		}
		protected internal int DataItemCount {
			get { return IsItemsBound ? fDataItemCount : DataItems.Count; }
		}
		protected internal DataViewItemCollection DataItems {
			get {
				if (dataItems == null)
					dataItems = CreateItemCollection(this);
				return dataItems;
			}
		}
		protected bool IsItemsBound {
			get { return isItemsBound; }
			set { isItemsBound = value; }
		}
		protected internal int ItemsPerPage {
			get { return GetItemsPerPage(); }
			set {
				if(PagerSettings.PageSizeItemSettings.Visible) {
					PageSizeInternal = value;
					DataLayoutChanged();
				}
			}
		}
		protected virtual internal int RowCount {
			get { return VisibleItemsList.Count / ColumnCountInternal + (VisibleItemsList.Count % ColumnCountInternal > 0 ? 1 : 0); }
		}
		protected internal int InitialPageSize {
			get { return initialRowPerPage; }
			set { initialRowPerPage = value; }
		}
		protected internal string SEOTargetName {
			get { return "seo" + ClientID; }
		}
		protected internal ArrayList VisibleItemsList {
			get {
				if (visibleItemsList == null)
					visibleItemsList = CreateVisibleItemsList();
				return visibleItemsList;
			}
		}
		protected internal int VisibleEndIndex {
			get {
				if (!AllowPaging)
					return DataItemCount - 1;
				else {
					if (PageIndex > -1)
						return ItemsPerPage * (PageIndex + 1) - 1;
					else
						if (AllButtonPageCount > 0 && ItemsPerPage * AllButtonPageCount < DataItemCount)
							return ItemsPerPage * AllButtonPageCount - 1;
						else
							return DataItemCount - 1;
				}
			}
		}
		protected internal int VisibleStartIndex {
			get {
				if (!AllowPaging)
					return 0;
				else {
					if (PageIndex > -1)
						return ItemsPerPage * PageIndex;
					else
						return 0;
				}
			}
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is DataViewPagerPanelCommandEventArgs) {
				OnPagerPanelCommand(e as DataViewPagerPanelCommandEventArgs);
				return true;
			}
			return false;
		}
		protected override void OnInit(EventArgs e) {
			ProcessSEOPaging();
			InitialPageSize = GetPageSize();
			base.OnInit(e);
		}
		private void ProcessSEOPaging() {
			if((Page != null  || IsMvcRender()) && PagerSettings.IsSEOEnabled) {
				int newPageIndex = ASPxPagerBase.GetPageIndexBySeoTargetName(SEOTargetName, Request, PageIndex); 
				ChangePageIndex(newPageIndex);
				if(PagerSettings.PageSizeItemSettings.Visible) {
					int newPageSize = ASPxPagerBase.GetPageSizeBySeoTargetName(SEOTargetName, Request, GetPageSize());
					ChangePageSize(newPageSize);
				}
			}
		}
		public Control FindItemControl(string id, DataViewItem item) {
			return TemplateContainerBase.FindTemplateControl(this, GetItemTemplateContainerID(item), id);
		}
		public Control FindPagerPanelControl(string id, PagerPanelPosition pagerPanelPosition, PagerPanelTemplatePosition templatePosition) {
			return TemplateContainerBase.FindTemplateControl(this, GetPagerPanelTemplateContainerID(pagerPanelPosition, templatePosition), id);
		}
		protected virtual DataViewItemCollection CreateItemCollection(ASPxDataViewBase control) {
			return null;
		}
		protected virtual DataViewTableLayoutSettings CreateTableLayoutSettings() {
			return new DataViewTableLayoutSettings(this);
		}
		protected virtual DataViewFlowLayoutSettings CreateFlowLayoutSettings() {
			return new DataViewFlowLayoutSettings(this);
		}
		protected virtual PagerSettingsEx CreatePagerSettings(IPropertiesOwner owner) {
			return null;
		}
		public bool HasItems() {
			return DataItems.Count > 0;
		}
		public bool HasVisibleItems() {
			return VisibleItemsList.Count > 0;
		}
		protected ArrayList CreateVisibleItemsList() {
			ArrayList list = new ArrayList();
			if (IsItemsBound) {
				for (int i = 0; i < DataItems.Count; i++)
					list.Add(DataItems[i]);
			}
			else {
				int visibleStartIndex = VisibleStartIndex;
				int visibleEndIndex = VisibleEndIndex;
				for (int i = visibleStartIndex; i <= visibleEndIndex; i++) {
					if (0 <= i && i < DataItems.Count)
						list.Add(DataItems[i]);
				}
			}
			return list;
		}
		protected void ResetVisibleItems() {
			this.visibleItemsList = null;
		}
		protected internal void ChangePageIndex(int newPageIndex) {
			CheckDataBound();
			DataViewPageEventArgs args = new DataViewPageEventArgs(newPageIndex);
			OnPageIndexChanging(args);
			if (!args.Cancel && args.NewPageIndex != PageIndex && PagerIsValidPageIndex(args.NewPageIndex)) {
				bool requireCallSizeChanged = ASPxPagerBase.IsAnyShowAll(args.NewPageIndex, PageIndex);
				SetPageIndex(args.NewPageIndex);
				DataLayoutChanged();
				OnPageIndexChanged(EventArgs.Empty);
				if(requireCallSizeChanged)
					OnPageSizeChanged(EventArgs.Empty);
			}
		}
		protected internal void SetPageIndex(int value) {
			if (PageIndex != value) {
				fPageIndex = value;
				SetAllPagesSavedPageIndex(value);
			}
		}
		protected internal void SetAllPagesSavedPageIndex(int value) {
			if (value > -1)
				fAllPagesSavedPageIndex = value;
		}
		protected internal void ValidatePageIndex() {
			if (fPageIndex < -1)
				fPageIndex = -1;
			if (!DesignMode && DataItemCount > 0 && fPageIndex >= PageCount)
				fPageIndex = PageCount - 1;
		}
		protected internal void ChangePageSize(int newPageSize) {
			CheckDataBound();
			DataViewPageSizeEventArgs args = new DataViewPageSizeEventArgs(newPageSize);
			OnPageSizeChanging(args);
			if(!args.Cancel && GetPageSize() != args.NewPageSize && PagerIsValidPageSize(args.NewPageSize)) {
				bool requireCallIndexChanged = ASPxPagerBase.IsAnyShowAll(args.NewPageSize, GetPageSize());
				SetPageSize(args.NewPageSize);
				DataLayoutChanged();
				OnPageSizeChanged(EventArgs.Empty);
				if(requireCallIndexChanged)
					OnPageIndexChanged(EventArgs.Empty);
			}
		}
		protected internal int GetPageSize() {
			if(PageIndex == -1)
				return -1;
			return LayoutInternal == Layout.Table ? SettingsTableLayoutInternal.RowsPerPage : ItemsPerPage;
		}
		protected internal void SetPageSize(int value) {
			if(value == -1)
				SetPageIndex(-1);
			if(value > 0) {
				if(PageIndex == -1)
					SetPageIndex(0);
				if(LayoutInternal == Layout.Table) {
					if(SettingsTableLayoutInternal.RowsPerPage != value)
						SettingsTableLayoutInternal.RowsPerPage = value;
				}
				else {
					if(SettingsFlowLayoutInternal.ItemsPerPage != value)
						SettingsFlowLayoutInternal.ItemsPerPage = value;
					if(ItemsPerPage != value)
						ItemsPerPage = value;
				}
			}
		}
		protected int GetItemsPerPage() {
			if(PagerSettings.PageSizeItemSettings.Visible && PageSizeInternal > 0)
				return PageSizeInternal;
			if(LayoutInternal == Layout.Table)
				return SettingsTableLayoutInternal.RowsPerPage * ColumnCountInternal;
			return SettingsFlowLayoutInternal.ItemsPerPage;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || IsCallBacksEnabled();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxPagerBase), ASPxPagerBase.PagerBaseScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDataViewBase), DataViewScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDataView";
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result[PageIndexStateKey] = PageIndex;
			result[AllPagesSavedPageIndexFieldName] = fAllPagesSavedPageIndex;
			result[PageCountFieldName] = fPageCount;
			result[PageSizeFieldName] = fPageSize;
			result[DataItemCountFieldName] = fDataItemCount;
			result[IsItemsBoundFieldName] = this.isItemsBound;
			if(IsCallBacksEnabled()) {
				result.Add(PageSizeStateKey, GetPageSize());
				result.Add(PageCountStateKey, PageCount);
				result.Add(LayoutStateKey, (int)LayoutInternal);
				if(EndlessPagingSettings != null)
					result.Add(EndlessPagingModeStateKey, (int)EndlessPagingSettings.EndlessPagingMode);
			}
			return result;
		}
		protected virtual void LoadDataViewState() {
			fPageIndex = GetClientObjectStateValue<int>(PageIndexStateKey);
			fAllPagesSavedPageIndex = GetClientObjectStateValue<int>(AllPagesSavedPageIndexFieldName);
			fPageCount = GetClientObjectStateValue<int>(PageCountFieldName);
			fPageSize = GetClientObjectStateValue<int>(PageSizeFieldName);
			fDataItemCount = GetClientObjectStateValue<int>(DataItemCountFieldName);
			this.isItemsBound = GetClientObjectStateValue<bool>(IsItemsBoundFieldName);
			DataLayoutChanged(CanBindOnLoadPostData());
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override void ClearControlFields() {
			fMainControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			fMainControl = CreateMainControl();
			Controls.Add(fMainControl);
			base.CreateControlHierarchy();
		}
		protected virtual DVMainControl CreateMainControl() {
			return new DVMainControl(this);
		}
		protected override void CreateChildControls() {
			EnsureDataBound();
			base.CreateChildControls();
		}
		protected internal virtual ItemContentInfo CreateItemContentInfo() {
			return null;
		}
		protected internal virtual void CreateItemControl(ItemContentInfo contentInfo, Control parent) {
		}
		protected internal virtual void CreateEmptyItemControl(ItemContentInfo contentInfo, Control parent) {
		}
		protected internal virtual void PrepareItemControl(ItemContentInfo contentInfo) {
		}
		protected internal virtual void PrepareEmptyItemControl(ItemContentInfo contentInfo) {
		}
		protected internal virtual ASPxPagerBase CreatePager() {
			return new DVPager(this);
		}
		protected internal string GetPagerOnClickFunction(string id) {
			return string.Format(PagerClickHandlerName, ClientID, id);
		}
		protected internal string GetPageSizeChangedHandler() {
			return string.Format(PagerPageSizeChangeHandlerName, this.ClientID);
		}
		protected internal string GetContentControlID() {
			return "CC";
		}
		protected internal string GetContentCellID() {
			return "CCell";
		}
		protected internal string GetItemsCellID() {
			return "ICell";
		}
		protected internal string GetItemsScrollerID() {
			return "IScroller";
		}
		protected internal string GetItemTemplateContainerID(DataViewItem item) {
			var clientItemIndex = item.Index;
			if(UseEndlessPaging)
				clientItemIndex += PageIndex * ItemsPerPage;
			return "IT" + clientItemIndex;
		}
		protected internal string GetPagerID(PagerPanelPosition pagerPanelPosition) {
			return "PG" + fPagerPanelPositionIDSuffixes[pagerPanelPosition];
		}
		protected internal string GetPagerPanelTemplateContainerID(PagerPanelPosition pagerPanelPosition, PagerPanelTemplatePosition templatePosition) {
			return string.Format("PPT{0}{1}", fPagerPanelPositionIDSuffixes[pagerPanelPosition],
				fPagerPanelTemplatePositionIDSuffixes[templatePosition]);
		}
		protected internal bool IsPageSizeAllItemVisible() {
			return PagerSettings.PageSizeItemSettings.Visible && PagerSettings.PageSizeItemSettings.ShowAllItem;
		}
		protected internal bool IsPageSizeItemVisible() {
			return PagerSettings.PageSizeItemSettings.Visible;
		}
		protected virtual bool IsPagerNeeded() {
			return !UseEndlessPaging && ( DesignMode || AlwaysShowPager || (PageCount > 1) || IsPageSizeItemVisible());
		}
		protected internal bool HasTopPager() {
			return AllowPaging && PagerSettings.Visible && IsPagerNeeded() &&
							(PagerSettings.Position == PagerPosition.Top ||
							PagerSettings.Position == PagerPosition.TopAndBottom);
		}
		protected internal bool HasBottomPager() {
			return AllowPaging && PagerSettings.Visible && IsPagerNeeded() &&
							(PagerSettings.Position == PagerPosition.Bottom ||
							PagerSettings.Position == PagerPosition.TopAndBottom);
		}
		protected internal bool HasBottomPagerPanel() {
			return HasBottomPager() || PagerPanelLeftTemplate != null || PagerPanelRightTemplate != null;
		}
		protected internal bool HasTopPagerPanel() {
			return HasTopPager() || PagerPanelLeftTemplate != null || PagerPanelRightTemplate != null;
		}
		protected internal virtual bool IsEmptyRowsVisible() {
			return true;
		}
		protected internal virtual bool IsScrollingEnabled() {
			return false;
		}
		protected internal override bool IsSwipeGesturesEnabled() {
			return base.IsSwipeGesturesEnabled() && !UseEndlessPaging;
		}
		protected internal void ItemsChanged() {
			if (!IsLoading()) {
				ResetViewStateStoringFlag();
				ResetVisibleItems();
				ResetControlHierarchy();
			}
		}
		protected internal virtual void DataLayoutChanged(bool canPerformBind) {
			if(!IsLoading()) {
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
				ResetVisibleItems();
				if(canPerformBind) {
					if(IsBoundUsingDataSourceID())
						RequireDataBinding();
					else if(DataSource != null)
						DataBind();
					else
						ValidatePageIndex();
				} else
					ValidatePageIndex();
			}
		}
		protected internal virtual void DataLayoutChanged() {
			DataLayoutChanged(true);
		}
		public bool IsCustomCallback() {
			return this.dataViewCallbackType == DataViewCallbackType.Custom;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			ProcessCallbackEventArgs(ref eventArgument);
			if(!IsCustomCallback()) {
				string[] arguments = eventArgument.Split(RenderUtils.CallBackSeparator);
				SetPageIndex(int.Parse(arguments[0]));
				SetPageSize(int.Parse(arguments[1]));
				string command = arguments[2];
				this.requireItemCountRecheck = false;
				if(ASPxPagerBase.IsChangePageSizeCommand(command)) {
					int newPageSize = ASPxPagerBase.GetNewPageSize(command, GetPageSize());
					ChangePageSize(newPageSize);
				}
				else {
					int newPageIndex = ASPxPagerBase.GetNewPageIndex(command, PageIndex, PagerPageCountEvaluator, false);
					ChangePageIndex(newPageIndex);
				}
				if(UseEndlessPaging)
					EndlessPagingHelper.ProcessCallback();
			}
			else {
				CustomCallBackPostBack(eventArgument);
				if(UseEndlessPaging)
					EndlessPagingHelper.ForceLoadFirstPage();
			}
		}
		protected internal int PagerPageCountEvaluator() {
			this.requireItemCountRecheck = true;
			return PageCount;
		}
		protected internal bool PagerIsValidPageIndex(int newIndex) {
			if(newIndex == -1) {
				if(PagerSettings.Visible)
					return PagerSettings.AllButton.Visible || IsPageSizeAllItemVisible();
				return false;
			}
			return true;
		}
		protected internal bool PagerIsValidPageSize(int newSize) {
			if(newSize == -1)
				return PagerIsValidPageIndex(-1);
			if(PagerSettings.PageSizeItemSettings.Visible)
				return newSize == InitialPageSize || 
					   Array.Exists<string>(PagerSettings.PageSizeItemSettings.Items, delegate(string item) { return item == newSize.ToString(); });
			return false;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			ProcessCallbackEventArgs(ref eventArgument);
			base.RaisePostBackEvent(eventArgument);
			if(IsCustomCallback())
				CustomCallBackPostBack(eventArgument);
		}
		protected override object GetCallbackResult() {
			if(UseEndlessPaging) {
				EndlessPagingHelper.Validate();
				EnsureChildControls();
			}
			var callbackResult = new Hashtable();
			BeginRendering();
			try {
				callbackResult[CallbackResultProperties.Html] = GetCallbackContentControlResult();
				callbackResult[CallbackResultProperties.StateObject] = GetClientObjectState();
				if(IsEndlessPagingCallback)
					EndlessPagingHelper.AddParametersToCallbackResult(callbackResult);
			}
			finally {
				EndRendering();
			}
			return callbackResult;
		}
		protected virtual string GetCallbackContentControlResult() {
			if(IsEndlessPagingCallback)
				return EndlessPagingHelper.GetCallbackContentControlResult();
			var contentControl = GetCallbackResultControl();
			return contentControl != null ? RenderUtils.GetRenderResult(contentControl) : string.Empty;
		}
		protected virtual DVContentControl GetCallbackResultControl() {
			return FindControl(GetContentControlID()) as DVContentControl;
		}
		private void ProcessCallbackEventArgs(ref string eventArgument) {
			char prefix = eventArgument[0];
			eventArgument = eventArgument.Substring(1);
			this.dataViewCallbackType = prefix == 'p' ? DataViewCallbackType.PagerClick : DataViewCallbackType.Custom;
		}
		protected internal void CheckDataBound() {
			if (IsItemsBound && !IsBoundUsingDataSourceID() && DataSource == null)
				throw new InvalidOperationException(StringResources.DataView_DataSourceUnassigned);
		}
		protected IEnumerable CreatePagedDataSource(IEnumerable data) {
			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = DataUtils.ConvertEnumerableToCollection(data);
			if(pds.DataSource == null) 
				return null;
			else {
				pds.AllowPaging = false;
				int prevItemCount = fDataItemCount;
				fDataItemCount = pds.Count;
				pds.AllowPaging = AllowPaging;
				if(!AllowPaging) {
					pds.PageSize = fDataItemCount;
					fPageCount = 1;
				} else {
					pds.PageSize = ItemsPerPage;
					if(fPageIndex > pds.PageCount - 1)
						fPageIndex = pds.PageCount - 1;
					if(requireItemCountRecheck && prevItemCount < fDataItemCount)
						fPageIndex = pds.PageCount - 1;
					pds.CurrentPageIndex = fPageIndex;
					fPageCount = pds.PageCount;
					if(PageIndex == -1) { 
						pds.CurrentPageIndex = 0;
						pds.PageSize = ItemsPerPage * AllButtonPageCount;
					}
				}
				return pds;
			}
		}
		protected void CorrectPageCountForServerPaging() {
			fDataItemCount = GetServerPagingTotalRowCount();
			if(!AllowPaging) {
				fPageCount = 1;
			} else {
				if(fDataItemCount > 0) { 
					int pageCount = fDataItemCount / ItemsPerPage + (fDataItemCount % ItemsPerPage > 0 ? 1 : 0);
					if(PageIndex > pageCount - 1)
						PageIndex = pageCount - 1 < 0 ? 0 : pageCount - 1;
					fPageCount = pageCount;
				}
			}
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable enumerable) {
			IEnumerable data = null;
			if(!IsAllowDataSourcePaging()) {
				data = CreatePagedDataSource(enumerable);
			} else {
				data = enumerable;
				CorrectPageCountForServerPaging();
			}
			if (!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null)) {
				IsItemsBound = false;
				DataItems.Clear();
			}
			else {
				if (!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
					IsItemsBound = true;
					DataBindItems(data);
					ResetControlHierarchy();
				}
			}
		}
		protected virtual void DataBindItems(IEnumerable enumerable) {
			DataItems.Clear();
			if (enumerable != null) {
				foreach (object obj in enumerable)
					DataBindItem(obj);
			}
		}
		protected virtual void DataBindItem(object obj) {
			DataViewItem item = new DataViewItem();
			DataItems.Add(item);
			item.DataItem = obj;
		}
		protected override Style CreateControlStyle() {
			return new DataViewStyle();
		}
		protected override StylesBase CreateStyles() {
			return new DataViewStyles(this);
		}
		protected internal Unit GetItemHeight() {
			DataViewItemStyle style = GetItemStyle();
			Unit height = style.Height;
			if (Browser.IsIE && LayoutInternal != Layout.Flow)
				height = UnitUtils.GetCorrectedHeight(height, style, GetItemPaddings());
			return height;
		}
		protected internal Unit GetItemWidth() {
			DataViewItemStyle style = GetItemStyle();
			Unit width = style.Width;
			width = UnitUtils.GetCorrectedWidth(width, style, GetItemPaddings());
			return width;
		}
		protected internal Unit GetItemSpacing() {
			return ((DataViewStyle)GetControlStyle()).ItemSpacing;
		}
		protected internal Margins GetFlowItemSpacing() {
			Unit spacing = ((DataViewStyle)GetControlStyle()).ItemSpacing;
			return new Margins(spacing, spacing, Unit.Empty, Unit.Empty);
		}
		protected internal Margins GetFlowItemsContainerSpacing() {
			Unit spacing = ((DataViewStyle)GetControlStyle()).ItemSpacing;
			return new Margins(new Unit(-spacing.Value, spacing.Type), new Unit(-spacing.Value, spacing.Type), Unit.Empty, Unit.Empty);
		}
		protected internal Unit GetPagerPanelSpacing() {
			return ((DataViewStyle)GetControlStyle()).PagerPanelSpacing;
		}
		protected internal Paddings GetContentPaddings() {
			return GetContentStyle().Paddings;
		}
		protected internal Paddings GetPagerPanelPaddings() {
			return GetPagerPanelStyle().Paddings;
		}
		protected internal Paddings GetItemPaddings() {
			return GetItemStyle().Paddings;
		}
		protected internal Paddings GetEmptyItemPaddings() {
			return GetEmptyItemStyle().Paddings;
		}
		protected internal AppearanceStyleBase GetFlowItemsContainerStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultFlowItemsContainerStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetFlowItemStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultFlowItemStyle());
			style.CopyFrom(GetItemStyle());
			return style;
		}
		protected internal DataViewContentStyle GetContentStyle() {
			DataViewContentStyle style = new DataViewContentStyle();
			style.CopyFrom(Styles.GetDefaultContentStyle());
			style.CopyFrom(ContentStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected virtual internal DataViewItemStyle GetItemStyle() {
			DataViewItemStyle style = new DataViewItemStyle();
			style.CopyFrom(Styles.GetDefaultItemStyle());
			style.CopyFrom(ItemStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal DataViewEmptyItemStyle GetEmptyItemStyle() {
			DataViewEmptyItemStyle style = new DataViewEmptyItemStyle();
			style.CopyFrom(Styles.GetDefaultEmptyItemStyle());
			style.CopyFrom(EmptyItemStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal DataViewContentStyle GetPagerPanelStyle() {
			DataViewContentStyle style = new DataViewContentStyle();
			style.CopyFrom(Styles.GetDefaultPagerPanelStyle());
			style.CopyFrom(PagerPanelStyle);
			return style;
		}
		protected internal DataViewEmptyDataStyle GetEmptyDataStyle() {
			DataViewEmptyDataStyle style = new DataViewEmptyDataStyle();
			style.CopyFrom(Styles.GetDefaultEmptyDataStyle());
			style.CopyFrom(EmptyDataStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal PagerStyle GetPagerStyle() {
			PagerStyle style = new PagerStyle();
			style.CopyFrom(PagerStyle);
			return style;
		}
		protected internal DisabledStyle GetPagerDisabledStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(DisabledStyle);
			return style;
		}
		protected internal PagerButtonStyle GetPagerButtonStyle() {
			return PagerButtonStyle;
		}
		protected internal PagerTextStyle GetPagerCurrentPageNumberStyle() {
			return PagerCurrentPageNumberStyle;
		}
		protected internal PagerButtonStyle GetPagerDisabledButtonStyle() {
			return PagerDisabledButtonStyle;
		}
		protected internal PagerTextStyle GetPagerPageNumberStyle() {
			return PagerPageNumberStyle;
		}
		protected internal AppearanceStyleBase GetPagerSeparatorStyle() {
			return GetPagerStyle().SeparatorStyle;
		}
		protected internal PagerTextStyle GetPagerSummaryStyle() {
			return PagerSummaryStyle;
		}
		protected internal PagerPageSizeItemStyle GetPagerPageSizeItemStyle() {
			return PagerPageSizeItemStyle;
		}
		protected internal HorizontalAlign GetPagerAlignHorizontalAlign() {
			PagerAlign align = PagerAlign;
			if(IsRightToLeft()) {
				if(align == PagerAlign.Left)
					align = PagerAlign.Right;
				else if(align == PagerAlign.Right)
					align = PagerAlign.Left;
			}
			return fPagerAlignHorizontalAlign[align];
		}
		protected internal DataViewPagerShowMoreItemsContainerStyle GetPagerShowMoreItemsContainerStyle() {
			DataViewPagerShowMoreItemsContainerStyle style = new DataViewPagerShowMoreItemsContainerStyle();
			style.CopyFrom(Styles.GetDefaultPagerShowMoreItemsContainerStyle());
			style.CopyFrom(PagerShowMoreItemsContainerStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal string GetEmptyDataText() {
			return !string.IsNullOrEmpty(EmptyDataText) ? EmptyDataText :
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataViewBase_EmptyDataText);
		}
		protected internal string GetPagerPanelSpacingClassName() {
			return Styles.GetCssClassNamePrefix() + PagerPanelSpacingClassAffix;
		}
		protected virtual void OnPagerPanelCommand(DataViewPagerPanelCommandEventArgs e) {
			DataViewPagerPanelCommandEventHandler handler = (DataViewPagerPanelCommandEventHandler)Events[EventPagerPanelCommand];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPageIndexChanging(DataViewPageEventArgs e) {
			DataViewPageEventHandler handler = (DataViewPageEventHandler)Events[EventPageIndexChanging];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPageIndexChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventPageIndexChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPageSizeChanging(DataViewPageSizeEventArgs e) {
			DataViewPageSizeEventHandler handler = (DataViewPageSizeEventHandler)Events[EventPageSizeChanging];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnPageSizeChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventPageSizeChanged];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void CustomCallBackPostBack(string eventArgument) {
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			CallbackEventHandlerBase handler = (CallbackEventHandlerBase)Events[EventCallback];
			if(handler != null)
				handler(this, args);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(UseEndlessPaging && EndlessPagingHelper != null)
				EndlessPagingHelper.ForceLoadFirstPage();
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
			this.ResetVisibleItems(); 
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DataItems, PagerSettings, SettingsFlowLayoutInternal, SettingsTableLayoutInternal });
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState != null) {
				LoadDataViewState();
				int newPageIndex = GetClientObjectStateValue<int>(PageIndexStateKey);
				int newPageSize = GetClientObjectStateValue<int>(PageSizeStateKey);
				int pageCount = GetClientObjectStateValue<int>(PageCountStateKey);
				LayoutInternal = (Layout)GetClientObjectStateValue<int>(LayoutStateKey);
				bool dataChanged = false;
				if(IsItemsBound && pageCount > -1) {
					if(fPageCount != pageCount) {
						fPageCount = pageCount;
						dataChanged = true;
					}
				}
				if(newPageIndex != PageIndex) {
					SetPageIndex(newPageIndex);
					dataChanged = true;
				}
				if(newPageSize != GetPageSize()) {
					SetPageSize(newPageSize);
					dataChanged = true;
				}
				if(EndlessPagingSettings != null) {
					EndlessPagingSettings.EndlessPagingMode = (DataViewEndlessPagingMode)GetClientObjectStateValue<int>(EndlessPagingModeStateKey);
					EndlessPagingHelper.LoadClientState(this);
				}
				if(dataChanged)
					DataLayoutChanged(CanBindOnLoadPostData());
			}
			return false;
		}
		protected virtual bool CanBindOnLoadPostData() {
			return true;
		}
		int IPagerOwner.InitialPageSize {
			get { return InitialPageSize; }
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new DataViewBaseDataHelper(this, name);
		}
		protected DataViewBaseDataHelper DataHelper {
			get { return DataContainer[DefaultDataHelperName] as DataViewBaseDataHelper; }
		}
		protected virtual internal bool IsAllowDataSourcePaging() {
			if(DesignMode)
				return false;
			return DataHelper.CanServerPaging;
		}
		protected int GetServerPagingTotalRowCount() {
			return DataHelper.SelectArgumentsInternal.TotalRowCount;
		}
		protected internal DataViewEndlessPagingHelper EndlessPagingHelper {
			get {
				if(EndlessPagingSettings == null)
					return null;
				if(endlessPagingHelper == null)
					endlessPagingHelper = new DataViewEndlessPagingHelper(this);
				return endlessPagingHelper;
			}
		}
		protected internal bool UseEndlessPaging { 
			get { return EndlessPagingSettings != null && EndlessPagingSettings.EndlessPagingMode != DataViewEndlessPagingMode.Disabled; } 
		}
		protected bool IsEndlessPagingCallback { get { return UseEndlessPaging && EndlessPagingHelper.PartialLoad; } }
	}
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxDataView"),
	Designer("DevExpress.Web.Design.ASPxDataViewControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDataView.bmp")
	]
	public class ASPxDataView : ASPxDataViewBase {
		ITemplate fItemTemplate;
		ITemplate fEmptyItemTemplate;
		private static readonly object EventItemCommand = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public CallbackClientSideEventsBase ClientSideEvents {
			get { return (CallbackClientSideEventsBase)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewLayout"),
#endif
		Category("Layout"), DefaultValue(Layout.Table), AutoFormatDisable,
		RefreshProperties(RefreshProperties.All)]
		public Layout Layout {
			get { return LayoutInternal; }
			set { LayoutInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft
		{
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[Obsolete("Use the SettingsTableLayout.RowsPerPage property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewRowPerPage"),
#endif
		Category("Paging"), DefaultValue(3), AutoFormatDisable]
		public int RowPerPage {
			get {
				if(DesignMode) return 3;
				return RowPerPageInternal;
			}
			set {
				RowPerPageInternal = value;
				ChangeFlowLayoutItemsPerPage();
			}
		}
		[Obsolete("Use the SettingsTableLayout.ColumnCount property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewColumnCount"),
#endif
		DefaultValue(3), AutoFormatDisable]
		public int ColumnCount {
			get {
				if(DesignMode) return 3;
				return SettingsTableLayout.ColumnCount;
			}
			set { 
				SettingsTableLayout.ColumnCount = value;
				ChangeFlowLayoutItemsPerPage();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewHideEmptyRows"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool HideEmptyRows {
			get { return GetBoolProperty("HideEmptyRows", true); }
			set {
				SetBoolProperty("HideEmptyRows", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewEnableScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableScrolling {
			get { return GetBoolProperty("EnableScrolling", false); }
			set {
				SetBoolProperty("EnableScrolling", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewSettingsFlowLayout"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewFlowLayoutSettings SettingsFlowLayout {
			get { return SettingsFlowLayoutInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewSettingsTableLayout"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewTableLayoutSettings SettingsTableLayout {
			get { return SettingsTableLayoutInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewPagerSettings"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewPagerSettings PagerSettings {
			get { return (DataViewPagerSettings)base.PagerSettings; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		MergableProperty(false), AutoFormatDisable]
		public DataViewItemCollection Items {
			get { return DataItems; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewEmptyItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewEmptyItemStyle EmptyItemStyle {
			get { return base.EmptyItemStyle; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DataViewItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return fItemTemplate; }
			set {
				fItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
	   TemplateContainer(typeof(DataViewTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EmptyItemTemplate {
			get { return fEmptyItemTemplate; }
			set {
				fEmptyItemTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDataViewItemCommand"),
#endif
		Category("Action")]
		public event DataViewItemCommandEventHandler ItemCommand
		{
			add { Events.AddHandler(EventItemCommand, value); }
			remove { Events.RemoveHandler(EventItemCommand, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<DataViewItem> VisibleItems {
			get {
				DataViewItem[] items = new DataViewItem[VisibleItemsList.Count];
				VisibleItemsList.CopyTo(items);
				return new ReadOnlyCollection<DataViewItem>(items);
			}
		}
		public ASPxDataView()
			: base() {
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is DataViewItemCommandEventArgs) {
				OnItemCommand(e as DataViewItemCommandEventArgs);
				return true;
			}
			return base.OnBubbleEvent(source, e);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if (Events[EventItemCommand] != null)
				RequireDataBinding();
		}
		protected override DataViewItemCollection CreateItemCollection(ASPxDataViewBase control) {
			return new DataViewItemCollection(control as ASPxDataView);
		}
		protected override PagerSettingsEx CreatePagerSettings(IPropertiesOwner owner) {
			return new DataViewPagerSettings(owner);
		}
		protected internal override bool IsScrollingEnabled() {
			return EnableScrolling && !Height.IsEmpty && EndlessPagingSettings.EndlessPagingMode == DataViewEndlessPagingMode.Disabled;
		}
		protected void ChangeFlowLayoutItemsPerPage() {
			SettingsFlowLayout.ItemsPerPage = SettingsTableLayout.RowsPerPage * SettingsTableLayout.ColumnCount;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackClientSideEventsBase();
		}
		protected internal override ItemContentInfo CreateItemContentInfo() {
			return new ItemContentInfo();
		}
		protected internal override void CreateItemControl(ItemContentInfo contentInfo, Control parent) {
			if (ItemTemplate != null) {
				DataViewItemTemplateContainer container = new DataViewItemTemplateContainer(contentInfo.Item);
				container.AddToHierarchy(parent, GetItemTemplateContainerID(contentInfo.Item));
				ItemTemplate.InstantiateIn(container);
			}
			else {
				LiteralControl spaceText = RenderUtils.CreateLiteralControl("&nbsp;");
				parent.Controls.Add(spaceText);
			}
		}
		protected internal override void CreateEmptyItemControl(ItemContentInfo contentInfo, Control parent) {
			if (EmptyItemTemplate != null) {
				DataViewTemplateContainer container = new DataViewTemplateContainer(this);
				EmptyItemTemplate.InstantiateIn(container);
				container.AddToHierarchy(parent, null);
			}
			else {
				LiteralControl spaceText = RenderUtils.CreateLiteralControl("&nbsp;");
				parent.Controls.Add(spaceText);
			}
		}
		protected internal override bool IsEmptyRowsVisible() {
			return !HideEmptyRows;
		}
		protected internal override int RowCount {
			get { return IsEmptyRowsVisible() ? Math.Max(SettingsTableLayout.RowsPerPage, base.RowCount) : base.RowCount; }
		}
		protected virtual void OnItemCommand(DataViewItemCommandEventArgs e) {
			DataViewItemCommandEventHandler handler = (DataViewItemCommandEventHandler)Events[EventItemCommand];
			if (handler != null)
				handler(this, e);
		}
	}
	public class DataViewBaseDataHelper : DataHelper {
		public DataViewBaseDataHelper(ASPxDataViewBase dataView, string name)
			: base(dataView, name) {
		}
		public ASPxDataViewBase Owner {
			get { return (ASPxDataViewBase)Control; }
		}
		public DataSourceSelectArguments SelectArgumentsInternal {
			get { return SelectArguments; }
		}
		public override void PerformSelect() {
			ResetSelectArguments();
			base.PerformSelect();
		}
		protected override DataSourceSelectArguments CreateDataSourceSelectArguments() {
			if(!CanServerPaging)
				return base.CreateDataSourceSelectArguments();
			int startIndex = 0;
			int count = -1;
			if(Owner.ItemsPerPage > -1) {
				if(Owner.AllowPaging && Owner.PageIndex > -1) {
					count = Owner.ItemsPerPage;
					startIndex = Owner.PageIndex * Owner.ItemsPerPage;
				} else {
					count = RetrieveTotalCount();
				}
			}
			DataSourceSelectArguments selectArguments = new DataSourceSelectArguments(startIndex, count);
			selectArguments.RetrieveTotalRowCount = true;
			return selectArguments;
		}
		public int RetrieveTotalCount() {
			DataSourceSelectArguments selectArguments = new DataSourceSelectArguments();
			selectArguments.RetrieveTotalRowCount = true;
			selectArguments.MaximumRows = 0;
			PerformSelectCore(selectArguments, null);
			return selectArguments.TotalRowCount;
		}
	}
}
