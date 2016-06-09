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
using System.Data;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Text;
using System.Drawing.Design;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Localization;
using DevExpress.Utils;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxNewsControl"), 
	ToolboxData("<{0}:ASPxNewsControl Width=\"400px\" runat=\"server\"></{0}:ASPxNewsControl>"),
	Designer("DevExpress.Web.Design.ASPxNewsControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxNewsControl.bmp")
	]
	public class ASPxNewsControl : ASPxDataViewBase, IControlDesigner {
		protected internal const string NewsControlScriptResourceName = WebScriptsResourcePath + "NewsControl.js";
		protected const string TailOnClickHandlerName = "ASPx.HLTClick(event, '{0}', '{1}')";
		private NewsItemSettings fItemSettings = null;
		private static readonly object EventTailClick = new object();
		private static readonly object EventItemDataBound = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlDateField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string DateField {
			get { return GetStringProperty("DateField", ""); }
			set {
				SetStringProperty("DateField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set {
				SetStringProperty("ImageUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlHeaderTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string HeaderTextField {
			get { return GetStringProperty("HeaderTextField", ""); }
			set {
				SetStringProperty("HeaderTextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlNameField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlNavigateUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(false), AutoFormatDisable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public NewsItemCollection Items {
			get { return DataItems as NewsItemCollection; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public NewsItemSettings ItemSettings {
			get {
				if(fItemSettings == null)
					fItemSettings = new NewsItemSettings(this);
				return fItemSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public NewsControlClientSideEvents ClientSideEvents {
			get { return (NewsControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlPagerSettings"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new NewsControlPagerSettings PagerSettings
		{
			get { return (NewsControlPagerSettings)base.PagerSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlPagerAlign"),
#endif
		Category("Paging"), DefaultValue(PagerAlign.Left), AutoFormatEnable]
		public override PagerAlign PagerAlign {
			get { return (PagerAlign)GetEnumProperty("PagerAlign", PagerAlign.Left); }
			set {
				SetEnumProperty("PagerAlign", PagerAlign.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlRowPerPage"),
#endif
		Category("Paging"), DefaultValue(3), AutoFormatDisable]
		public int RowPerPage {
			get { return RowPerPageInternal; }
			set { RowPerPageInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlShowBackToTop"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowBackToTop {
			get { return GetBoolProperty("ShowBackToTop", false); }
			set {
				SetBoolProperty("ShowBackToTop", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlBackToTopText"),
#endif
		DefaultValue(StringResources.NewsControl_BackToTopText), AutoFormatDisable, Localizable(true)]
		public string BackToTopText {
			get { return GetStringProperty("BackToTopText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.NewsControl_BackToTop)); }
			set { SetStringProperty("BackToTopText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.NewsControl_BackToTop), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ItemImage {
			get { return Images.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlBackToTopImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties BackToTopImage {
			get { return Images.BackToTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlBackToTopStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackToTopStyle BackToTopStyle {
			get { return Styles.BackToTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemDateStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlineDateStyle ItemDateStyle {
			get { return Styles.ItemDate; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlineContentStyle ItemContentStyle {
			get { return Styles.ItemContent; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlineStyle ItemHeaderStyle {
			get { return Styles.ItemHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemTailStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlineStyle ItemTailStyle {
			get { return Styles.ItemTail; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemLeftPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlinePanelStyle ItemLeftPanelStyle {
			get { return Styles.ItemLeftPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemRightPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlinePanelStyle ItemRightPanelStyle {
			get { return Styles.ItemRightPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlTailClick"),
#endif
		Category("Action")]
		public event NewsItemEventHandler TailClick
		{
			add
			{
				Events.AddHandler(EventTailClick, value);
			}
			remove
			{
				Events.RemoveHandler(EventTailClick, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNewsControlItemDataBound"),
#endif
		Category("Data")]
		public event NewsItemEventHandler ItemDataBound
		{
			add { Events.AddHandler(EventItemDataBound, value); }
			remove { Events.RemoveHandler(EventItemDataBound, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<NewsItem> VisibleItems {
			get {
				NewsItem[] items = new NewsItem[VisibleItemsList.Count];
				VisibleItemsList.CopyTo(items);
				return new ReadOnlyCollection<NewsItem>(items);
			}
		}
		protected NewsControlImages Images {
			get { return (NewsControlImages)ImagesInternal; }
		}
		protected new NewsControlStyles Styles {
			get { return (NewsControlStyles)StylesInternal; }
		}
		protected internal override int ColumnCountInternal {
			get { return 1; }
		}
		public ASPxNewsControl() : base() {
		}
		protected override DataViewItemCollection CreateItemCollection(ASPxDataViewBase control) {
			return new NewsItemCollection(control as ASPxNewsControl);
		}
		protected override PagerSettingsEx CreatePagerSettings(IPropertiesOwner owner) {
			return new NewsControlPagerSettings(owner);
		}
		protected override void DataBindItem(object obj) {
			NewsItem newsItem = new NewsItem();
			DataBindItemProperties(obj, newsItem);
			newsItem.DataItem = obj;
			DataItems.Add(newsItem);
			OnItemDataBound(new NewsItemEventArgs(newsItem));
		}
		protected void DataBindItemProperties(object obj, NewsItem newsItem) {
			string headerTextFieldName = (HeaderTextField == "") ? "HeaderText" : HeaderTextField;
			string textFieldName = (TextField == "") ? "Text" : TextField;
			string urlFieldName = (NavigateUrlField == "") ? "NavigateUrl" : NavigateUrlField;
			string imageUrlFieldName = (ImageUrlField == "") ? "ImageUrl" : ImageUrlField;
			string dateFieldName = (DateField == "") ? "Date" : DateField;
			newsItem.HeaderText = GetFieldValue(obj, headerTextFieldName, HeaderTextField != "", "").ToString();
			newsItem.Text = GetFieldValue(obj, textFieldName, TextField != "", "").ToString();
			newsItem.Name = GetFieldValue(obj, NameField, NameField != "", "").ToString();
			newsItem.NavigateUrl = GetFieldValue(obj, urlFieldName, NavigateUrlField != "", "").ToString();
			newsItem.Image.Url = GetFieldValue(obj, imageUrlFieldName, ImageUrlField != "", "").ToString();
			DateTime date;
			if (DateTime.TryParse(GetFieldValue(obj, dateFieldName, DateField != "", "").ToString(), out date))
				newsItem.Date = date;
		}
		protected override ImagesBase CreateImages() {
			return new NewsControlImages(this);
		}
		protected internal ImageProperties GetItemImage(NewsItem item) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, NewsControlImages.ItemImageName));
			image.CopyFrom(item.Image);
			return image;
		}
		protected internal ImageProperties GetBackToTopImage() {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, NewsControlImages.BackToTopImageName));
			return image;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new NewsControlClientSideEvents();
		}
		protected override void CreateControlHierarchy() {
			WebControl topLink = RenderUtils.CreateAnchor(GetBackToTopLinkName());
			Controls.Add(topLink);
			base.CreateControlHierarchy();
		}
		protected internal override ItemContentInfo CreateItemContentInfo() {
			return new NewsItemContentInfo();
		}
		protected internal override ASPxPagerBase CreatePager() {
			return new NCPager(this);
		}
		protected internal override void CreateItemControl(ItemContentInfo contentInfo, Control parent) {
			NewsItemContentInfo newsItemContentInfo = contentInfo as NewsItemContentInfo;
			NewsItem newsItem = contentInfo.Item as NewsItem;
			bool isLastItem = (newsItem.Index == newsItem.Collection.Count - 1);
			newsItemContentInfo.Headline = new NCHeadline(this);
			newsItemContentInfo.Headline.RightToLeft = RightToLeft;
			parent.Controls.Add(newsItemContentInfo.Headline);
			newsItemContentInfo.Headline.ContentText = newsItem.Text;
			newsItemContentInfo.Headline.HeaderText = newsItem.HeaderText;
			newsItemContentInfo.Headline.NavigateUrl = GetItemNavigateUrl(newsItem);
			newsItemContentInfo.Headline.Date = newsItem.Date;
			newsItemContentInfo.Headline.Image.Assign(GetItemImage(newsItem));
			newsItemContentInfo.Headline.HeadlineSettings.Assign(ItemSettings);
			if (ShowBackToTop) {
				newsItemContentInfo.BackToTopControl = new BackToTopControl(GetBackToTopLink(), IsRightToLeft());
				contentInfo.Container.Controls.Add(newsItemContentInfo.BackToTopControl);
				newsItemContentInfo.BackToTopControl.Text = BackToTopText;
				newsItemContentInfo.BackToTopControl.BackToTopImage.Assign(GetBackToTopImage());
			}
		}
		protected internal override void PrepareItemControl(ItemContentInfo contentInfo) {
			NewsItemContentInfo newsItemContentInfo = contentInfo as NewsItemContentInfo;
			NCHeadline headline = newsItemContentInfo.Headline;
			if (headline != null) {
				headline.Enabled = IsEnabled();
				headline.EncodeHtml = EncodeHtml;
				headline.Height = Unit.Percentage(100);
				headline.Width = Unit.Percentage(100);
				AppearanceStyleBase headlineStyle = (AppearanceStyleBase)headline.ControlStyle;
				headlineStyle.CopyFontFrom(ItemStyle);
				headline.ContentStyle.CopyFrom(GetHeadlineContentStyle());
				headline.HeaderStyle.CopyFrom(GetHeadlineHeaderStyle());
				headline.TailStyle.CopyFrom(GetItemTailStyle());
				headline.DateStyle.CopyFrom(GetItemDateStyle());
				headline.DateStyle.Spacing = GetItemDataSpacing();
				headline.LeftPanelStyle.CopyFrom(GetHeadlineLeftPanelStyle());
				headline.RightPanelStyle.CopyFrom(GetHeadlineRightPanelStyle());
				if(HasTailOnClick())
					headline.HeadlineTailOnClick = GetTailOnClick((contentInfo.Item as NewsItem).Name);
				else headline.HeadlineTailOnClick = "";
			}
			BackToTopControl backToTopControl = newsItemContentInfo.BackToTopControl;
			if (backToTopControl != null) {
				backToTopControl.BackToTopStyle = GetBackToTopStyle();
				backToTopControl.BackToTopPaddings = GetBackToTopPaddings();
				backToTopControl.BackToTopSpacing = GetBackToTopSpacing();
				backToTopControl.BackToTopImageSpacing = GetBackToTopImageSpacing();
			}
		}
		protected internal override bool IsEmptyRowsVisible() {
			return false;
		}
		private string GetItemNavigateUrl(NewsItem newsItem) {
			string url = string.Format(NavigateUrlFormatString, newsItem.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected string GetBackToTopLink() {
			return "#" + GetBackToTopLinkName();
		}
		protected string GetBackToTopLinkName() {
			return ClientID + "_BT";
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasTailServerClickEventHandler();
		}
		protected bool HasTailServerClickEventHandler() {
			return HasEvents() && Events[EventTailClick] != null;
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterBackToTopScript();
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventTailClick] != null)
				eventNames.Add("TailClick");
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || HasTailOnClick();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxNewsControl), NewsControlScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientNewsControl";
		}
		protected internal bool HasTailOnClick() {
			return HasTailServerClickEventHandler() || ((ClientSideEvents as NewsControlClientSideEvents).TailClick != "") &&
				(ItemSettings.TailText != "" || ItemSettings.TailImage.Url != "");
		}
		protected internal string GetTailOnClick(string name) {
			return string.Format(TailOnClickHandlerName, ClientID, name);
		}
		protected override StylesBase CreateStyles() {
			return new NewsControlStyles(this);
		}
		protected internal Unit GetItemDataSpacing() {
			return GetItemDateStyle().Spacing;
		}
		protected internal Unit GetBackToTopSpacing() {
			return GetBackToTopStyle().Spacing;
		}
		protected internal Unit GetBackToTopImageSpacing() {
			return GetBackToTopStyle().ImageSpacing;
		}
		protected internal Paddings GetBackToTopPaddings() {
			return GetBackToTopStyle().Paddings;
		}
		protected internal BackToTopStyle GetBackToTopStyle() {
			BackToTopStyle style = new BackToTopStyle();
			style.CopyFrom(Styles.GetDefaultBackToTopStyle());
			style.CopyFrom(BackToTopStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineContentStyle GetHeadlineContentStyle() {
			HeadlineContentStyle style = new HeadlineContentStyle();
			style.CopyFrom(ItemContentStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineStyle GetHeadlineHeaderStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(ItemHeaderStyle);
			MergeDisableStyle(style);
			return style;				  
		}
		protected internal HeadlinePanelStyle GetHeadlineLeftPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(ItemLeftPanelStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlinePanelStyle GetHeadlineRightPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(ItemRightPanelStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineDateStyle GetItemDateStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(ItemDateStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineTailStyle GetItemTailStyle() {
			HeadlineTailStyle style = new HeadlineTailStyle();
			style.CopyFrom(ItemTailStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected virtual void OnItemDataBound(NewsItemEventArgs e) {
			NewsItemEventHandler handler = (NewsItemEventHandler)Events[EventItemDataBound];
			if (handler != null)
				handler(this, e);
		}
		private void OnItemClick(NewsItemEventArgs e) {
			NewsItemEventHandler handler = (NewsItemEventHandler)Events[EventTailClick];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "CLICK":
					string itemName = arguments[1];
					NewsItem item = Items.FindByName(itemName);
					NewsItemEventArgs args = item != null ? args = new NewsItemEventArgs(item) : new NewsItemEventArgs(null);
					OnItemClick(args);
					break;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ItemSettings });
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.NewsControlItemsOwner"; } }
	}
	public class NewsItemSettings : HeadlineSettings {
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemSettingsDateVerticalPosition"),
#endif
		DefaultValue(DateVerticalPosition.BelowHeader), NotifyParentProperty(true)]
		public override DateVerticalPosition DateVerticalPosition {
			get { return (DateVerticalPosition)GetEnumProperty("DateVerticalPosition", DateVerticalPosition.BelowHeader); }
			set {
				SetEnumProperty("DateVerticalPosition", DateVerticalPosition.BelowHeader, value);
				Changed();
			}
		}
		public NewsItemSettings()
			: base() {
		}
		public NewsItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
	}
}
