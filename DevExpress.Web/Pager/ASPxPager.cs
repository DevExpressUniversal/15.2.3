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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Utils;
using System.Collections.Specialized;
namespace DevExpress.Web {
	public enum PagerEllipsisMode { None, InsideNumeric, OutsideNumeric }
	public enum SEOFriendlyMode { Disabled, Enabled, CrawlerOnly }
	public abstract class ASPxPagerBase : ASPxWebControl {
		protected internal const string PagerBaseScriptResourceName = WebScriptsResourcePath + "Pager.js";
		protected internal const string PageSizeChangeValueCommand = "PSP";
		protected internal const string PageSizeItemCssClass = "dxpPSI";
		protected internal const string PagerHiddenFieldId = "State";
		protected const string AllPagesSavedPageIndexFieldName = "a";
		protected const string ItemCountFieldName = "c";
		protected const string PageIndexFieldName = "p";
		private int allPagesSavedPageIndex = 0;
		private int itemCount = 0;
		private int pageIndex = 0;
		protected PagerSettingsEx fPagerSettings = null;
		private static readonly object EventPageIndexChanging = new object();
		private static readonly object EventPageIndexChanged = new object();
		private static readonly object EventPageSizeChanging = new object();
		private static readonly object EventPageSizeChanged = new object();
		protected internal static string CellNavigationClickHandlerName = "ASPx.PGNav(event);";
		protected internal static string PageSizeClickHandlerName = "ASPx.POnPageSizeClick('{0}', event)";
		protected internal static string PageSizeKeyDownHandlerName = "ASPx.POnPageSizeKeyDown('{0}', event)";
		protected internal static string PageSizeInputBlurHandlerName = "ASPx.POnPageSizeBlur('{0}', event)";
		protected internal static string PageSizePopupItemClickHandlerName = "function(s,e) {{ ASPx.POnPageSizePopupItemClick('{0}', e.item); }}";
		protected internal static string PagePrefix = "page";
		protected internal static string SizePrefix = "size";
		protected const char Divider = '|';
		public ASPxPagerBase()
			: this(null) {
			AutoPostBackInternal = true;
		}
		protected ASPxPagerBase(ASPxWebControl ownedControl)
			: base(ownedControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseItemCount"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public virtual int ItemCount {
			get { return itemCount; }
			set {
				CommonUtils.CheckNegativeValue(value, "ItemCount");
				itemCount = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseItemsPerPage"),
#endif
		DefaultValue(10), AutoFormatDisable]
		public virtual int ItemsPerPage {
			get { return GetIntProperty("ItemsPerPage", 10); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ItemsPerPage");
				SetIntProperty("ItemsPerPage", 10, value);
				ValidatePageIndex();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageIndex"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public virtual int PageIndex {
			get { return pageIndex; }
			set {
				if(!IsLoading())
					CommonUtils.CheckValueRange(value, -1, PageCount - 1, "PageIndex");
				SetPageIndex(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseAllButton"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AllButtonProperties AllButton {
			get { return PagerSettings.AllButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseCurrentPageNumberFormat"),
#endif
		Category("Settings"), DefaultValue(PagerSettingsEx.DefaultCurrentPageNumberFormat),
		Localizable(true), AutoFormatEnable]
		public virtual string CurrentPageNumberFormat {
			get { return PagerSettings.CurrentPageNumberFormat; }
			set {
				PagerSettings.CurrentPageNumberFormat = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseEllipsisMode"),
#endif
		Category("Settings"), AutoFormatEnable, DefaultValue(PagerEllipsisMode.InsideNumeric)]
		public virtual PagerEllipsisMode EllipsisMode {
			get { return PagerSettings.EllipsisMode; }
			set { PagerSettings.EllipsisMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseEnableAdaptivity"),
#endif
		Category("Settings"), DefaultValue(false), AutoFormatDisable]
		public virtual bool EnableAdaptivity {
			get { return PagerSettings.EnableAdaptivity; }
			set { PagerSettings.EnableAdaptivity = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseFirstPageButton"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual FirstButtonProperties FirstPageButton {
			get { return PagerSettings.FirstPageButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseLastPageButton"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LastButtonProperties LastPageButton {
			get { return PagerSettings.LastPageButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseNextPageButton"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NextButtonProperties NextPageButton {
			get { return PagerSettings.NextPageButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseNumericButtonCount"),
#endif
		Category("Settings"), DefaultValue(10), AutoFormatDisable]
		public virtual int NumericButtonCount {
			get { return PagerSettings.NumericButtonCount; }
			set { PagerSettings.NumericButtonCount = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageNumberFormat"),
#endif
		Category("Settings"), AutoFormatEnable,
		DefaultValue(PagerSettingsEx.DefaultPageNumberFormat), Localizable(true)]
		public virtual string PageNumberFormat {
			get { return PagerSettings.PageNumberFormat; }
			set {
				PagerSettings.PageNumberFormat = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePrevPageButton"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PrevButtonProperties PrevPageButton {
			get { return PagerSettings.PrevPageButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseShowDisabledButtons"),
#endif
		Category("Settings"), DefaultValue(true), AutoFormatEnable]
		public bool ShowDisabledButtons {
			get { return PagerSettings.ShowDisabledButtons; }
			set { PagerSettings.ShowDisabledButtons = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseShowNumericButtons"),
#endif
		Category("Settings"), DefaultValue(true), AutoFormatDisable]
		public virtual bool ShowNumericButtons {
			get { return PagerSettings.ShowNumericButtons; }
			set { PagerSettings.ShowNumericButtons = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSummary"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SummaryProperties Summary {
			get { return PagerSettings.Summary; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageSizeItemSettings"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PageSizeItemSettings PageSizeItemSettings
		{
			get { return PagerSettings.PageSizeItemSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseItemSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit ItemSpacing {
			get { return (ControlStyle as PagerStyle).ItemSpacing; }
			set { (ControlStyle as PagerStyle).ItemSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSeparatorColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public virtual Color SeparatorColor {
			get { return (ControlStyle as PagerStyle).SeparatorColor; }
			set { (ControlStyle as PagerStyle).SeparatorColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSeparatorBackgroundImage"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackgroundImage SeparatorBackgroundImage {
			get { return (ControlStyle as PagerStyle).SeparatorBackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSeparatorHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit SeparatorHeight {
			get { return (ControlStyle as PagerStyle).SeparatorHeight; }
			set { (ControlStyle as PagerStyle).SeparatorHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSeparatorPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings SeparatorPaddings {
			get { return (ControlStyle as PagerStyle).SeparatorPaddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSeparatorWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit SeparatorWidth {
			get { return (ControlStyle as PagerStyle).SeparatorWidth; }
			set { (ControlStyle as PagerStyle).SeparatorWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseShowDefaultImages"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowDefaultImages {
			get { return PagerSettings.ShowDefaultImages; }
			set { PagerSettings.ShowDefaultImages = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseShowSeparators"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public virtual bool ShowSeparators {
			get { return PagerSettings.ShowSeparators; }
			set { PagerSettings.ShowSeparators = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseVisible"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public override bool Visible {
			get { return PagerSettings.Visible; }
			set { PagerSettings.Visible = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxPagerBaseSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxPagerBaseButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle ButtonStyle {
			get { return Styles.Button; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseDisabledButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle DisabledButtonStyle {
			get { return Styles.DisabledButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseCurrentPageNumberStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle CurrentPageNumberStyle {
			get { return Styles.CurrentPageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageNumberStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle PageNumberStyle {
			get { return Styles.PageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageSizeItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerPageSizeItemStyle PageSizeItemStyle
		{
			get { return Styles.PageSizeItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseSummaryStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle SummaryStyle {
			get { return Styles.Summary; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBaseEllipsisStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle EllipsisStyle {
			get { return Styles.Ellipsis; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageIndexChanging"),
#endif
		Category("Action")]
		public event PagerPageEventHandler PageIndexChanging
		{
			add { Events.AddHandler(EventPageIndexChanging, value); }
			remove { Events.RemoveHandler(EventPageIndexChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageIndexChanged"),
#endif
		Category("Action")]
		public event EventHandler PageIndexChanged
		{
			add { Events.AddHandler(EventPageIndexChanged, value); }
			remove { Events.RemoveHandler(EventPageIndexChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageSizeChanging"),
#endif
		Category("Action")]
		public event PagerPageSizeEventHandler PageSizeChanging
		{
			add { Events.AddHandler(EventPageSizeChanging, value); }
			remove { Events.RemoveHandler(EventPageSizeChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerBasePageSizeChanged"),
#endif
		Category("Action")]
		public event EventHandler PageSizeChanged
		{
			add { Events.AddHandler(EventPageSizeChanged, value); }
			remove { Events.RemoveHandler(EventPageSizeChanged, value); }
		}
		[Browsable(false)]
		public virtual int PageCount {
			get {
				if(PageIndex == -1 && IsPageSizeVisible())
					return 1;
				return ItemCount / ItemsPerPage + ((ItemCount % ItemsPerPage != 0) ? 1 : 0);
			}
		}
		protected virtual int ItemCountInSummaryText { get { return ItemCount; } }
		protected internal int AllPagesSavedPageIndex {
			get { return allPagesSavedPageIndex; }
		}
		protected PagerImages Images {
			get { return (PagerImages)ImagesInternal; }
		}
		protected internal PagerSettingsEx PagerSettings {
			get {
				if(fPagerSettings == null)
					fPagerSettings = CreatePagerSettings(this);
				return fPagerSettings;
			}
		}
		protected int InitialPageSize { get; set; }
		protected internal string[] PageSizeItems {
			get {
				var result = new SortedSet<int>(PageSizeItemSettings.Items.Select(int.Parse));
				if(OwnerControl as IPagerOwner != null)
					result.UnionWith(PageSizeItemSettings.Items.Select(int.Parse));
				if(InitialPageSize > 0)
					result.Add(InitialPageSize);
				return result.Select(x => x.ToString()).ToArray();
			}
		}
		protected PagerStyles Styles {
			get { return (PagerStyles)StylesInternal; }
		}
		protected internal bool IsSEOEnabled { get { return PagerSettings.IsSEOEnabled; } }
		protected internal bool IsNeedNoDecorationButtonStyle() {
			return IsSEOEnabled && Browser.Family.IsWebKit;
		}
		protected internal string GetSeoTargetValue(params object[] args) {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}", PagePrefix, (int)args[0] + 1);
			if(args.Length > 1)
				sb.AppendFormat("{0}{1}{2}", Divider, SizePrefix, (int)args[1]);
			return sb.ToString();
		}
		protected virtual SEOTarget GetSEOTarget(params object[] args) {
			return SEOTarget.Empty;
		}
		private string GetSeoNavigateUrl(params object[] args) {
			if((Page == null && !IsMvcRender()) || DesignMode || GetSEOTarget(args).IsEmpty)
				return string.Empty;
			var url = string.Format("{0}?{1}", !MvcUtils.IsCallback() ? Request.Path : Request.UrlReferrer.AbsolutePath, CreateSeoQueryString(args));
			return Uri.EscapeUriString(url);
		}
		private void AppendQueryString(StringBuilder query, string name, string value) {
			if(query.Length > 0)
				query.Append("&");
			query.AppendFormat("{0}={1}", name, value);
		}
		private string CreateSeoQueryString(params object[] args) {
			SEOTarget seoTarget = GetSEOTarget(args);
			StringBuilder query = new StringBuilder();
			foreach(string name in Request.QueryString) {
				if(!string.IsNullOrEmpty(name) && !name.ToLower().Equals(seoTarget.Name.ToLower()))
					AppendQueryString(query, name, Request.QueryString[name]);
			}
			AppendQueryString(query, seoTarget.Name, seoTarget.Value);
			return query.ToString();
		}
		protected virtual PagerSettingsEx CreatePagerSettings(ASPxPagerBase pager) {
			return new PagerSettingsEx(pager);
		}
		protected internal int GetNumericButtonsCurrentPageIndex() {
			return (PageIndex != -1) ? PageIndex : AllPagesSavedPageIndex;
		}
		protected internal void SetPageIndex(int value) {
			if(PageIndex != value) {
				if(value < -1)
					value = -1;
				if(ItemCount > 0 && value >= PageCount)
					value = PageCount - 1;
				this.pageIndex = value;
				SetAllPagesSavedPageIndex(value);
				LayoutChanged();
			}
		}
		protected internal virtual string GetPageSizeText() {
			return GetPageSize() == -1 ? PageSizeItemSettings.AllItemText : GetPageSize().ToString();
		}
		protected internal virtual int GetPageSize() {
			if(PageIndex == -1)
				return -1;
			return ItemsPerPage;
		}
		protected internal void SetPageSize(int value) {
			if(PageIndex == -1 && value > 0)
				SetPageIndex(0);
			if(ItemsPerPage != value) {
				if(value == -1)
					SetPageIndex(-1);
				if(value > 0)
					ItemsPerPage = value;
			}
		}
		protected internal void SetAllPagesSavedPageIndex(int value) {
			if(value > -1)
				this.allPagesSavedPageIndex = value;
		}
		protected internal List<MenuItem> GetPageSizeMenuItems() {
			List<MenuItem> items = new List<MenuItem>();
			int count = PageSizeItems.Length;
			for(int i = 0; i < count; i++) {
				int size = int.Parse(PageSizeItems[i]);
				string navigateUrl = (ItemsPerPage != size || PageIndex == -1) ? GetPageSizeNavigateUrl(size) : string.Empty;
				items.Add(new MenuItem(PageSizeItems[i], PageSizeItems[i], string.Empty, navigateUrl));
			}
			if(PageSizeItemSettings.ShowAllItem) {
				string navigateUrl = (PageIndex != -1) ? GetPageSizeNavigateUrl(0) : string.Empty;
				items.Add(new MenuItem(PageSizeItemSettings.AllItemText, "-1", string.Empty, navigateUrl));
			}
			return items;
		}
		protected internal List<Hashtable> GetPageSizeItemsScriptObject() {
			List<Hashtable> items = new List<Hashtable>();
			Hashtable item;
			int count = PageSizeItems.Length;
			for(int i = 0; i < count; i++) {
				item = new Hashtable();
				item["text"] = PageSizeItems[i];
				item["value"] = PageSizeItems[i];
				items.Add(item);
			}
			if(IsPageSizeAllItemVisible()) {
				item = new Hashtable();
				item["text"] = PageSizeItemSettings.AllItemText;
				item["value"] = "-1";
				items.Add(item);
			}
			return items;
		}
		protected internal Hashtable GetPageSizeSelectedItemScriptObject() {
			Hashtable item = new Hashtable();
			item["text"] = GetPageSizeText();
			item["value"] = GetPageSize();
			return item;
		}
		protected virtual bool RequireInlineLayout { get { return false; } }
		protected override bool HasContent() {
			return (ItemCount > 0) && (Summary.Visible || ShowNumericButtons || AllButton.Visible ||
				FirstPageButton.Visible || LastPageButton.Visible || NextPageButton.Visible || PrevPageButton.Visible);
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(CreatePagerMainControlLite());
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsAriaSupported() && IsAccessibilityCompliantRender()) {
				RenderUtils.SetStringAttribute(this, "role", "application");
				RenderUtils.SetStringAttribute(this, "aria-label", AccessibilityUtils.PagerDescriptionText);
			}
		}
		protected virtual PagerMainControlLite CreatePagerMainControlLite() {
			return DesignMode ? new PagerMainControlLiteDesignMode(this) : new PagerMainControlLite(this);
		}
		protected internal string GetSummaryText() {
			if(PageCount == 0)
				return Summary.EmptyText;
			if(PageIndex > -1)
				return string.Format(HtmlEncode(Summary.GetText()), PageIndex + 1, PageCount, ItemCountInSummaryText);
			return string.Format(HtmlEncode(Summary.AllPagesText), 1, PageCount, ItemCountInSummaryText);
		}
		protected internal string GetPageNumberButtonText(bool isCurrent, int pageIndex) {
			string format = isCurrent ? CurrentPageNumberFormat : PageNumberFormat;
			return string.Format(HtmlEncode(format), pageIndex + 1);
		}
		protected virtual string GetItemElementOnClick(string id) {
			return RenderUtils.GetPostBackEventReference(Page, this, id);
		}
		protected internal string GetItemElementOnClickInternal(string id) {
			return IsSEOEnabled ? CellNavigationClickHandlerName : GetItemElementOnClick(id);
		}
		protected virtual string GetPageSizeElementOnClick() {
			return string.Format(PageSizeClickHandlerName, this.ClientID);
		}
		protected internal string GetPageSizeClickHandler() {
			return GetPageSizeElementOnClick();
		}
		protected virtual string GetPageSizeChangedHandler() {
			return "ASPx.POnPageSizeChanged";
		}
		protected virtual string GetPageSizeChangedHandlerInternal() {
			return IsSEOEnabled ? "ASPx.POnSeoPageSizeChanged" : GetPageSizeChangedHandler();
		}
		protected internal string GetPageSizeKeyDownHandler() {
			return string.Format(PageSizeKeyDownHandlerName, this.ClientID);
		}
		protected internal string GetPageSizeInputBlurHandler() {
			return string.Format(PageSizeInputBlurHandlerName, this.ClientID);
		}
		protected virtual string GetPageSizePopupItemElementOnClick() {
			return string.Format(PageSizePopupItemClickHandlerName, this.ClientID);
		}
		protected internal string GetPageSizePopupItemElementOnClickInternal() {
			return GetPageSizePopupItemElementOnClick();
		}
		protected internal string GetPageSizeBoxID() {
			return "PSB";
		}
		protected internal string GetPageSizeInputID() {
			return "PSI";
		}
		protected internal string GetPageSizeDropDownButtonID() {
			return "DDB";
		}
		protected internal string GetPageSizeDropDownButtonImageID() {
			return string.Format("{0}{1}", GetPageSizeDropDownButtonID(), PagerImages.ButtonImageIdPostfix);
		}
		protected internal string GetPageSizePopupControlID() {
			return "PSP";
		}
		protected internal string GetButtonID(PagerButtonProperties properties) {
			return "PB" + properties.GetButtonIDSuffix();
		}
		protected internal string GetNumberButtonID(int index) {
			return "PN" + index.ToString();
		}
		protected internal bool IsPageSizeVisible() {
			return PageSizeItemSettings.Visible ;
		}
		protected internal bool IsPageSizeAllItemVisible() {
			return PageSizeItemSettings.Visible && PageSizeItemSettings.ShowAllItem;
		}
		protected internal virtual string GetButtonNavigateUrl(int pageIndex) {
			return GetButtonNavigateUrl(pageIndex, ItemsPerPage);
		}
		protected string GetButtonNavigateUrl(int pageIndex, int pageSize) {
			if(IsSEOEnabled) {
				if(IsPageSizeVisible())
					return GetSeoNavigateUrl(pageIndex, pageSize);
				return GetSeoNavigateUrl(pageIndex);
			}
			if(IsAccessibilityCompliantRender(true))
				return RenderUtils.AccessibilityEmptyUrl;
			return String.Empty;
		}
		protected internal virtual string GetPageSizeNavigateUrl(int pageSize) {
			if(IsSEOEnabled)
				return GetSeoNavigateUrl(PageIndex != -1 ? PageIndex : 0, pageSize);
			if(IsAccessibilityCompliantRender(true))
				return RenderUtils.AccessibilityEmptyUrl;
			return String.Empty;
		}
		protected internal static int GetPageIndexBySeoTargetName(string seoTargetName, HttpRequest request, int pageIndex){
			int newPageIndex = pageIndex;
			if(request != null && !string.IsNullOrEmpty(request.QueryString[seoTargetName])) {
				string[] seoTargetValues = request.QueryString[seoTargetName].Split(Divider);
				foreach(string seoTargetValue in seoTargetValues) {
					if(!string.IsNullOrEmpty(seoTargetValue) && seoTargetValue.StartsWith(PagePrefix)) {
						string newPageIndexString = seoTargetValue.Substring(PagePrefix.Length);
						if(int.TryParse(newPageIndexString, out newPageIndex))
							newPageIndex = newPageIndex - 1;
						break;
					}
				}
			}
			return newPageIndex;
		}
		protected internal static int GetPageSizeBySeoTargetName(string seoTargetName, HttpRequest request, int pageSize) {
			int newPageSize = pageSize;
			if(request != null && !string.IsNullOrEmpty(request.QueryString[seoTargetName])) {
				string[] seoTargetValues = request.QueryString[seoTargetName].Split(Divider);
				foreach(string seoTargetValue in seoTargetValues) {
					if(!string.IsNullOrEmpty(seoTargetValue) && seoTargetValue.StartsWith(SizePrefix)) {
						string newPageSizeString = seoTargetValue.Substring(SizePrefix.Length);
						int.TryParse(newPageSizeString, out newPageSize);
						break;
					}
				}
			}
			return newPageSize;
		}
		protected override ImagesBase CreateImages() {
			return new PagerImages(this);
		}
		protected internal ImageProperties GetPageSizeDropDownImage() {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(GetDefaultPageSizeDropDownImage());
			image.CopyFrom(PageSizeItemSettings.DropDownImage);
			return image;
		}
		protected internal ImagePropertiesEx GetAllButtonImage() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			if(ShowDefaultImages)
				image.CopyFrom(GetDefaultAllButtonImage());
			image.CopyFrom(AllButton.Image);
			return image;
		}
		protected internal ImagePropertiesEx GetPrevButtonImage() {
			ImagePropertiesEx result = GetPrevButtonImageCore();
			if(IsRightToLeft()) {
				string alt = result.AlternateText;
				result = GetNextButtonImageCore();
				result.AlternateText = alt;
			}
			return result;
		}
		protected internal ImagePropertiesEx GetNextButtonImage() {
			ImagePropertiesEx result = GetNextButtonImageCore();
			if(IsRightToLeft()) {
				string alt = result.AlternateText;
				result = GetPrevButtonImageCore();
				result.AlternateText = alt;
			}
			return result;
		}
		protected internal ImagePropertiesEx GetFirstButtonImage() {
			ImagePropertiesEx result = GetFirstButtonImageCore();
			if(IsRightToLeft()) {
				string alt = result.AlternateText;
				result = GetLastButtonImageCore();
				result.AlternateText = alt;
			}
			return result;
		}
		protected internal ImagePropertiesEx GetLastButtonImage() {
			ImagePropertiesEx result = GetLastButtonImageCore();
			if(IsRightToLeft()) {
				string alt = result.AlternateText;
				result = GetFirstButtonImageCore();
				result.AlternateText = alt;
			}
			return result;
		}
		ImagePropertiesEx GetPrevButtonImageCore() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			if(ShowDefaultImages)
				image.CopyFrom(GetDefaultPrevButtonImage());
			image.CopyFrom(PrevPageButton.Image);
			return image;
		}
		ImagePropertiesEx GetNextButtonImageCore() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			if(ShowDefaultImages)
				image.CopyFrom(GetDefaultNextButtonImage());
			image.CopyFrom(NextPageButton.Image);
			return image;
		}
		ImagePropertiesEx GetFirstButtonImageCore() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			if(ShowDefaultImages)
				image.CopyFrom(GetDefaultFirstButtonImage());
			image.CopyFrom(FirstPageButton.Image);
			return image;
		}
		ImagePropertiesEx GetLastButtonImageCore() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			if(ShowDefaultImages)
				image.CopyFrom(GetDefaultLastButtonImage());
			image.CopyFrom(LastPageButton.Image);
			return image;
		}
		protected ImageProperties GetDefaultPageSizeDropDownImage() {
			return Images.GetImageProperties(Page, PagerImages.PopOutImageName);
		}
		protected ImageProperties GetDefaultAllButtonImage() {
			return Images.GetImageProperties(Page, PagerImages.AllButtonImageName);
		}
		protected ImageProperties GetDefaultNextButtonImage() {
			return Images.GetImageProperties(Page, PagerImages.NextButtonImageName);
		}
		protected ImageProperties GetDefaultPrevButtonImage() {
			return Images.GetImageProperties(Page, PagerImages.PrevButtonImageName);
		}
		protected ImageProperties GetDefaultFirstButtonImage() {
			return Images.GetImageProperties(Page, PagerImages.FirstButtonImageName);
		}
		protected ImageProperties GetDefaultLastButtonImage() {
			return Images.GetImageProperties(Page, PagerImages.LastButtonImageName);
		}
		protected override Style CreateControlStyle() {
			return new PagerStyle();
		}
		protected override StylesBase CreateStyles() {
			return new PagerStyles(this);
		}
		protected internal Unit GetButtonImageSpacing(bool isButtonDisabled, bool hasText, bool hasImage) {
			return GetButtonStyle(isButtonDisabled, hasText, hasImage).ImageSpacing;
		}
		protected internal Unit GetItemSpacing() {
			return GetControlStyle().Spacing;
		}
		public override Paddings GetPaddings() {
			return new Paddings();
		}
		protected internal Paddings GetButtonPaddings(bool isButtonDisabled, bool hasText, bool hasImage) {
			return GetButtonStyle(isButtonDisabled, hasText, hasImage).Paddings;
		}
		protected internal Paddings GetPageNumberButtonPaddings(bool isCurrentPage) {
			return GetPageNumberStyle(isCurrentPage).Paddings;
		}
		protected internal Paddings GetPageSizeItemPaddings() {
			return GetPageSizeItemStyle().Paddings;
		}
		protected internal Paddings GetPageSizeBoxPaddings(bool isDisabled) {
			return GetComboBoxStyle(isDisabled).Paddings;
		}
		protected internal Paddings GetPageSizeDropDownButtonPaddings(bool isDisabled) {
			return GetPageSizeDropDownButtonStyle(isDisabled).Paddings;
		}
		protected internal Paddings GetSummaryPaddings() {
			return GetSummaryStyle().Paddings;
		}
		protected internal Paddings GetSeparatorPaddings() {
			return GetSeparatorStyle().Paddings;
		}
		protected internal Unit GetSeparatorHeight() {
			return GetSeparatorStyle().Height;
		}
		protected internal Unit GetSeparatorWidth() {
			return GetSeparatorStyle().Width;
		}
		protected internal bool HasSeparators() {
			return ShowSeparators && GetSeparatorHeight().Value != 0 && GetSeparatorWidth().Value != 0;
		}
		protected internal Unit GetButtonHeight(bool isButtonDisabled, bool hasText, bool hasImage) {
			AppearanceStyle style = GetButtonStyle(isButtonDisabled, hasText, hasImage);
			Unit height = style.Height;
			if(Browser.IsIE)
				height = UnitUtils.GetCorrectedHeight(height, style, GetButtonPaddings(isButtonDisabled, hasText, hasImage));
			return height;
		}
		protected internal Unit GetButtonWidth(bool isButtonDisabled, bool hasText, bool hasImage) {
			AppearanceStyle style = GetButtonStyle(isButtonDisabled, hasText, hasImage);
			Unit width = style.Width;
			if(Browser.IsIE)
				width = UnitUtils.GetCorrectedHeight(width, style, GetButtonPaddings(isButtonDisabled, hasText, hasImage));
			return width;
		}
		protected internal Unit GetPageSizeItemHeight() {
			PagerPageSizeItemStyle style = GetPageSizeItemStyle();
			Unit height = style.Height;
			if(Browser.IsIE)
				height = UnitUtils.GetCorrectedHeight(height, style, GetPageSizeItemPaddings());
			return height;
		}
		protected internal Unit GetPageSizeItemWidth() {
			PagerPageSizeItemStyle style = GetPageSizeItemStyle();
			Unit width = style.Width;
			if(Browser.IsIE)
				width = UnitUtils.GetCorrectedWidth(width, style, GetPageSizeItemPaddings());
			return width;
		}
		protected internal Unit GetPageNumberButtonHeight(bool isCurrentPage) {
			AppearanceStyle style = GetPageNumberStyle(isCurrentPage);
			Unit height = style.Height;
			if(Browser.IsIE)
				height = UnitUtils.GetCorrectedHeight(height, style, GetPageNumberButtonPaddings(isCurrentPage));
			return height;
		}
		protected internal Unit GetPageNumberButtonWidth(bool isCurrentPage) {
			AppearanceStyle style = GetPageNumberStyle(isCurrentPage);
			Unit width = style.Width;
			if(Browser.IsIE)
				width = UnitUtils.GetCorrectedHeight(width, style, GetPageNumberButtonPaddings(isCurrentPage));
			return width;
		}
		protected internal Unit GetSummaryHeight() {
			AppearanceStyle style = GetSummaryStyle();
			Unit height = style.Height;
			if(Browser.IsIE)
				height = UnitUtils.GetCorrectedHeight(height, style, GetSummaryPaddings());
			return height;
		}
		protected internal Unit GetSummaryWidth() {
			AppearanceStyle style = GetSummaryStyle();
			Unit width = style.Width;
			if(Browser.IsIE)
				width = UnitUtils.GetCorrectedHeight(width, style, GetSummaryPaddings());
			return width;
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			base.PrepareControlStyle(style);
			style.CopyFrom(Styles.Pager);
		}
		protected DisabledStyle GetChildDisabledStyle() {
			return DisabledStyle;
		}
		protected internal PagerButtonStyle GetButtonStyle(bool isButtonDisabled, bool hasText, bool hasImage) {
			PagerButtonStyle style = new PagerButtonStyle();
			style.CopyFrom(Styles.GetDefaultButtonStyle(hasText, hasImage));
			if(isButtonDisabled)
				style.CopyFrom(Styles.GetDefaultDisabledButtonStyle());
			style.CopyFrom(ButtonStyle);
			if(isButtonDisabled) {
				MergeDisableStyle(style, false, GetChildDisabledStyle());
				style.CopyFrom(DisabledButtonStyle);
			}			
			MergeDisableStyle(style, GetChildDisabledStyle());
			return style;
		}
		protected internal PagerTextStyle GetPageNumberStyle(bool isCurrentPage) {
			PagerTextStyle style = new PagerTextStyle();
			style.CopyFrom(Styles.GetDefaultPageNumberStyle());
			if(isCurrentPage)
				style.CopyFrom(Styles.GetDefaultCurrentPageNumberStyle());
			style.CopyFrom(PageNumberStyle);
			if(isCurrentPage)
				style.CopyFrom(CurrentPageNumberStyle);
			MergeDisableStyle(style, GetChildDisabledStyle());
			return style;
		}
		protected internal PagerPageSizeItemStyle GetPageSizeItemStyle() {
			PagerPageSizeItemStyle style = new PagerPageSizeItemStyle();
			style.CopyFrom(Styles.GetDefaultPageSizeItemStyle());
			style.CopyFrom(PageSizeItemStyle);
			MergeDisableStyle(style, IsEnabled(), GetChildDisabledStyle(), false);
			return style;
		}
		protected internal PagerComboBoxStyle GetComboBoxStyle(bool isDisabled) {
			PagerComboBoxStyle style = new PagerComboBoxStyle();
			style.CopyFrom(Styles.GetDefaultComboBoxStyle());
			if(!IsEnabled() && isDisabled)
				style.CopyFrom(Styles.GetDefaultDisabledComboBoxStyle());
			style.CopyFrom(PageSizeItemStyle.ComboBoxStyle);
			if(!IsEnabled() && isDisabled) {
				MergeDisableStyle(style, GetChildDisabledStyle());
				style.CopyFrom(PageSizeItemStyle.ComboBoxStyle.DisabledStyle);
			}
			return style;
		}
		protected internal AppearanceStyleBase GetComboBoxHoverStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultHoverComboBoxStyle();
			ret.CopyFrom(PageSizeItemStyle.ComboBoxStyle.HoverStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetComboBoxPressedStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultPressedComboBoxStyle();
			ret.CopyFrom(PageSizeItemStyle.ComboBoxStyle.PressedStyle);
			return ret;
		}
		protected internal AppearanceSelectedStyle GetPageSizeBoxHoverCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetComboBoxHoverStyle());
			return style;
		}
		protected internal AppearanceSelectedStyle GetPageSizeBoxPressedCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetComboBoxPressedStyle());
			return style;
		}
		protected internal PagerDropDownButtonStyle GetPageSizeDropDownButtonStyle(bool isDisabled) {
			PagerDropDownButtonStyle style = new PagerDropDownButtonStyle();
			style.CopyFrom(Styles.GetDefaultDropDownButtonStyle());
			if(!IsEnabled() || isDisabled)
				style.CopyFrom(Styles.GetDefaultDisabledDropDownButtonStyle());
			style.CopyFrom(GetComboBoxStyle(isDisabled).DropDownButtonStyle);
			if(!IsEnabled() || isDisabled) {
				MergeDisableStyle(style, GetChildDisabledStyle());
				style.CopyFrom(GetComboBoxStyle(isDisabled).DropDownButtonStyle.DisabledStyle);
			}
			return style;
		}
		protected internal AppearanceStyleBase GetPageSizeDropDownButtonHoverStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultHoverDropDownButtonStyle();
			ret.CopyFrom(PageSizeItemStyle.ComboBoxStyle.DropDownButtonStyle.HoverStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetPageSizeDropDownButtonPressedStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultPressedDropDownButtonStyle();
			ret.CopyFrom(PageSizeItemStyle.ComboBoxStyle.DropDownButtonStyle.PressedStyle);
			return ret;
		}
		protected internal AppearanceSelectedStyle GetPageSizeDropDownButtonHoverCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetPageSizeDropDownButtonHoverStyle());
			return style;
		}
		protected internal AppearanceSelectedStyle GetPageSizeDropDownButtonPressedCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetPageSizeDropDownButtonPressedStyle());
			return style;
		}
		protected internal MenuStyles GetPageSizeDropDownWindowStyle() {
			MenuStyles styles = Styles.GetDefaultDropDownWindowStyle();
			styles.Style.CopyFrom(GetComboBoxStyle(false).DropDownWindowStyle);
			styles.Item.CopyFrom(GetComboBoxStyle(false).ItemStyle);
			return styles;
		}
		protected internal PagerTextStyle GetSummaryStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CopyFrom(Styles.GetDefaultSummaryStyle());
			style.CopyFrom(SummaryStyle);
			MergeDisableStyle(style, GetChildDisabledStyle());
			return style;
		}
		protected internal PagerTextStyle GetEllipsisStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CopyFrom(Styles.GetDefaultEllipsisStyle());
			style.CopyFrom(EllipsisStyle);
			MergeDisableStyle(style, GetChildDisabledStyle());
			return style;
		}
		protected internal AppearanceStyle GetSeparatorStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultSeparatorStyle());
			style.CopyFrom(((PagerStyle)GetControlStyle()).SeparatorStyle);
			return style;
		}
		protected internal virtual bool CanStoreIndexes() {
			return false;
		}
		protected override void OnInit(EventArgs e) {
			var pagerOwner = OwnerControl as IPagerOwner;
			InitialPageSize = pagerOwner != null ? pagerOwner.InitialPageSize : ItemsPerPage;
			base.OnInit(e);
		}
		protected override string GetClientObjectStateInputID() {
			return base.GetClientObjectStateInputID() + "$State";
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null)
				return false;
			if(CanStoreIndexes()) {
				this.allPagesSavedPageIndex = GetClientObjectStateValue<int>(ASPxPagerBase.AllPagesSavedPageIndexFieldName);
				this.itemCount = GetClientObjectStateValue<int>(ASPxPagerBase.ItemCountFieldName);
				this.pageIndex = GetClientObjectStateValue<int>(ASPxPagerBase.PageIndexFieldName);
			}
			return true;
		}
		protected internal Hashtable GetPagerStateObject() {
			Hashtable result = new Hashtable();
			result[ASPxPagerBase.AllPagesSavedPageIndexFieldName] = AllPagesSavedPageIndex;
			result[ASPxPagerBase.ItemCountFieldName] = ItemCount;
			result[ASPxPagerBase.PageIndexFieldName] = PageIndex;
			return result;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PagerSettings });
		}
		protected virtual void OnPageIndexChanging(PagerPageEventArgs e) {
			PagerPageEventHandler handler = (PagerPageEventHandler)Events[EventPageIndexChanging];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnPageIndexChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventPageIndexChanged];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnPageSizeChanging(PagerPageSizeEventArgs e) {
			PagerPageSizeEventHandler handler = (PagerPageSizeEventHandler)Events[EventPageSizeChanging];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnPageSizeChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventPageSizeChanged];
			if(handler != null)
				handler(this, e);
		}
		public static int GetNewPageIndex(string eventArgument, int pageIndex, int pageCount) {
			return GetNewPageIndex(eventArgument, pageIndex, delegate() { return pageCount; }, true);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int GetNewPageIndex(string eventArgument, int pageIndex, ASPxPagerGetCountCallback countEvaluator, bool correctForNextPage) {
			if(eventArgument.Contains("PN"))
				return int.Parse(eventArgument.Substring(2));
			else {
				if(eventArgument.Contains("PB")) {
					switch(eventArgument.Substring(2)) {
						case "A":   
							return -1;
						case "L":   
							return countEvaluator() - 1;
						case "N":   
							if(correctForNextPage) {
								int count = countEvaluator();
								return (pageIndex < count - 1) ? pageIndex + 1 : count - 1;
							}
							return pageIndex + 1;
						case "F":   
							return 0;
						case "P":   
							return (pageIndex > 0) ? pageIndex - 1 : 0;
					}
				}
			}
			return 0;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int GetNewPageSize(string eventArgument, int pageSize) {
			int newPageSize = pageSize;
			if(IsChangePageSizeCommand(eventArgument)) {
				int value;
				if(int.TryParse(eventArgument.Substring(PageSizeChangeValueCommand.Length), out value))
					newPageSize = value;
			}
			return newPageSize;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsChangePageSizeCommand(string command) {
			return command.Contains("PSP");
		}
		protected virtual void ProcessPagerEvent(string eventArgument) {
			var postponedEvents = new List<Action<EventArgs>>();
			if(IsChangePageSizeCommand(eventArgument)) {
				int newPageSize = GetNewPageSize(eventArgument);
				PagerPageSizeEventArgs args = new PagerPageSizeEventArgs(newPageSize);
				OnPageSizeChanging(args);
				if(!args.Cancel && GetPageSize() != args.NewPageSize && IsValidPageSize(args.NewPageSize)) {
					if(IsAnyShowAll(args.NewPageSize, GetPageSize()))
						postponedEvents.Add(OnPageIndexChanged);
					SetPageSize(args.NewPageSize);
					postponedEvents.Add(OnPageSizeChanged);
				}
			}
			else {
				int newPageIndex = GetNewPageIndex(eventArgument);
				PagerPageEventArgs args = new PagerPageEventArgs(newPageIndex);
				OnPageIndexChanging(args);
				if(!args.Cancel && PageIndex != args.NewPageIndex && IsValidPageIndex(args.NewPageIndex)) {
					if(IsAnyShowAll(args.NewPageIndex, PageIndex))
						postponedEvents.Add(OnPageSizeChanged);
					SetPageIndex(args.NewPageIndex);
					postponedEvents.Add(OnPageIndexChanged);
				}
			}
			postponedEvents.ForEach(x => x(EventArgs.Empty));
		}
		protected static internal bool IsAnyShowAll(params int[] values) {
			return values.Any(x => x == -1);
		}
		protected virtual int GetNewPageIndex(string eventArgument) {
			return GetNewPageIndex(eventArgument, PageIndex, PageCount);
		}
		protected virtual int GetNewPageSize(string eventArgument) {
			return GetNewPageSize(eventArgument, ItemsPerPage);
		}
		protected bool IsValidPageIndex(int pageIndex) {
			if(pageIndex == -1)
				return AllButton.Visible || IsPageSizeAllItemVisible();
			return true;
		}
		protected bool IsValidPageSize(int pageSize) {
			if(pageSize == -1)
				return IsValidPageIndex(-1);
			return Array.Exists<string>(PageSizeItems, delegate(string item) { return item == pageSize.ToString(); });
		}
		protected void ValidatePageIndex() {
			if(this.pageIndex < -1)
				this.pageIndex = -1;
			if(PageCount > 0 && this.pageIndex >= PageCount)
				this.pageIndex = PageCount - 1;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			ProcessPagerEvent(eventArgument);
		}
		protected override bool HasFunctionalityScripts() {
			return (IsSEOEnabled || ItemsAdjustmentNeeded());
		}
		protected override bool HasClientInitialization() {
			return ItemsAdjustmentNeeded();
		}
		protected virtual bool IsAdaptivityEnebled() {
			return EnableAdaptivity;
		}
		private bool ItemsAdjustmentNeeded() {
			return IsPageSizeVisible() || !Width.IsEmpty || OwnerControl != null || IsAdaptivityEnebled();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPager";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(OwnerControl != null)
				stb.Append(localVarName + ".hasOwnerControl=true;\n");
			if(RequireInlineLayout)
				stb.Append(localVarName + ".requireInlineLayout=true;\n");
			if(IsAdaptivityEnebled())
				stb.Append(localVarName + ".enableAdaptivity=true;\n");
			stb.AppendFormat("{0}.pageSizeItems = {1};\n", localVarName, HtmlConvertor.ToJSON(GetPageSizeItemsScriptObject()));
			stb.AppendFormat("{0}.pageSizeSelectedItem = {1};\n", localVarName, HtmlConvertor.ToJSON(GetPageSizeSelectedItemScriptObject()));
			stb.AppendFormat("{0}.{1}.AddHandler({2});\n", localVarName, "pageSizeChangedHandler", GetPageSizeChangedHandlerInternal());
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxPagerBase), PagerBaseScriptResourceName);
		}
		protected override bool HasHoverScripts() {
			return IsEnabled() && IsPageSizeVisible();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			base.AddHoverItems(helper);
			helper.AddStyle(GetPageSizeBoxHoverCssStyle(), GetPageSizeBoxID(), new string[0],
				null, String.Empty, IsEnabled());
			helper.AddStyle(GetPageSizeDropDownButtonHoverCssStyle(), GetPageSizeDropDownButtonID(), new string[0],
				GetPageSizeDropDownImage().GetHottrackedScriptObject(Page), PagerImages.ButtonImageIdPostfix, IsEnabled());
		}
		protected override bool HasPressedScripts() {
			return IsEnabled() && IsPageSizeVisible();
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			base.AddPressedItems(helper);
			helper.AddStyle(GetPageSizeBoxPressedCssStyle(), GetPageSizeBoxID(), new string[0],
				null, String.Empty, IsEnabled());
			helper.AddStyle(GetPageSizeDropDownButtonPressedCssStyle(), GetPageSizeDropDownButtonID(), new string[0],
				GetPageSizeDropDownImage().GetPressedScriptObject(Page), PagerImages.ButtonImageIdPostfix, IsEnabled());
		}
	}
	[DXWebToolboxItem(true), DefaultProperty("ItemCount"), DefaultEvent("PageIndexChanged"),
	ToolboxData("<{0}:ASPxPager runat=\"server\"></{0}:ASPxPager>"),
	Designer("DevExpress.Web.Design.ASPxPagerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxPager.bmp")
	]
	public class ASPxPager : ASPxPagerBase {
		public ASPxPager()
			: base() {
		}
		protected ASPxPager(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxPagerWidth")]
#endif
		public override Unit Width {
			get { return base.Width; }
			set {
				base.Width = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerRenderMode"),
#endif
 Category("Layout"),
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, DefaultValue(ControlRenderMode.Lightweight)]
		public ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerSeoFriendly"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable,
		Category("Settings"), DefaultValue(SEOFriendlyMode.Disabled), Localizable(false)]
		public SEOFriendlyMode SeoFriendly {
			get { return PagerSettings.SEOFriendly; }
			set { PagerSettings.SEOFriendly = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerSeoNavigateUrlFormatString"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable,
		Category("Settings"), DefaultValue(""), Localizable(false)]
		public string SeoNavigateUrlFormatString {
			get { return PagerSettings.SeoNavigateUrlFormatString; }
			set { PagerSettings.SeoNavigateUrlFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPagerRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		protected internal override bool CanStoreIndexes() {
			return true;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ProcessSeoPaging();
		}
		protected override SEOTarget GetSEOTarget(params object[] args) {
			return new SEOTarget(SEOTargetName, GetSeoTargetValue(args));
		}
		protected internal string SEOTargetName {
			get { return "seo" + ClientID; }
		}
		protected internal override string GetButtonNavigateUrl(int pageIndex) {
			if(!string.IsNullOrEmpty(SeoNavigateUrlFormatString)) {
				if(IsPageSizeVisible())
					return GetSeoNavigateUrlFormatString(pageIndex, ItemsPerPage);
				return GetSeoNavigateUrlFormatString(pageIndex);
			}
			return base.GetButtonNavigateUrl(pageIndex);
		}
		protected internal override string GetPageSizeNavigateUrl(int pageSize) {
			return string.IsNullOrEmpty(SeoNavigateUrlFormatString) ? base.GetPageSizeNavigateUrl(pageSize) :
				GetSeoNavigateUrlFormatString(PageIndex, pageSize);
		}
		private string GetSeoNavigateUrlFormatString(int pageIndex) {
			return string.Format(SeoNavigateUrlFormatString, pageIndex + 1);
		}
		private string GetSeoNavigateUrlFormatString(int pageIndex, int pageSize) {
			pageIndex = PageIndex != -1 ? pageIndex : 0;
			return string.Format(SeoNavigateUrlFormatString, (pageIndex + 1) + Divider.ToString() + pageSize);
		}
		private void ProcessSeoPaging() {
			if(!IsSEOEnabled) return;
			bool allowNegative = true;
			bool allowSizing = IsPageSizeVisible();
			int newPageIndex = -1;
			int newPageSize = ItemsPerPage;
			if(!string.IsNullOrEmpty(SeoNavigateUrlFormatString)) {
				if(allowSizing) {
					allowNegative = false;
					int pageIndex, pageSize;
					if(SearchSeoSizePaging(out pageIndex, out pageSize)) {
						newPageIndex = pageIndex;
						newPageSize = pageSize;
					} else
						newPageIndex = 0;
				} else {
					int pageIndex;
					if(SearchSeoPaging(out pageIndex))
						newPageIndex = pageIndex;
				}
			}
			else {
				newPageIndex = GetPageIndexBySeoTargetName(SEOTargetName, Request, PageIndex);
				newPageSize = GetPageSizeBySeoTargetName(SEOTargetName, Request, ItemsPerPage);
			}
			if(newPageSize <= 0) {
				newPageIndex = -1;
				newPageSize = ItemsPerPage;
			}
			if(IsValidPageIndex(newPageIndex) || (allowNegative && newPageIndex == -1))
				SetPageIndex(newPageIndex);
			if(allowSizing && PageIndex != -1 && IsValidPageSize(newPageSize))
				SetPageSize(newPageSize);
		}
		private bool SearchSeoPaging(out int pageIndex) {
			pageIndex = 0;
			for(int i = -1; i < PageCount; i++) {
				if(UrlUtils.IsCurrentUrl(ResolveUrl(GetSeoNavigateUrlFormatString(i)))) {
					pageIndex = i;
					return true;
				}
			}
			return false;
		}
		private bool SearchSeoSizePaging(out int pageIndex, out int pageSize) {
			pageIndex = 0;
			pageSize = ItemsPerPage;
			string[] sizes = PageSizeItems;
			if(!Array.Exists<string>(sizes, delegate(string item) { return item == ItemsPerPage.ToString(); })) {
				Array.Resize<string>(ref sizes, sizes.Length + 1);
				sizes[sizes.Length - 1] = ItemsPerPage.ToString();
			}
			if(IsValidPageSize(-1)) {
				Array.Resize<string>(ref sizes, sizes.Length + 1);
				sizes[sizes.Length - 1] = "0";
			}
			for(int i = -1; i < PageCount; i++) {
				for(int j = 0; j < sizes.Length; j++) {
					int size = int.Parse(sizes[j]);
					if(UrlUtils.IsCurrentUrl(ResolveUrl(GetSeoNavigateUrlFormatString(i, size)))) {
						pageIndex = i;
						pageSize = size;
						return true;
					}
				}
			}
			return false;
		}
		public override Paddings GetPaddings() {
			return GetControlStyle().Paddings;
		}
	}
}
namespace DevExpress.Web.Internal {
	public class SEOTarget {
		private string name;
		private string value;
		public static readonly SEOTarget Empty = new SEOTarget(null, null);
		public SEOTarget(string name, string value) {
			this.name = name;
			this.value = value;
		}
		public bool IsEmpty { get { return string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value); } }
		public string Name { get { return name; } }
		public string Value { get { return value; } }
	}
	public delegate int ASPxPagerGetCountCallback();
}
