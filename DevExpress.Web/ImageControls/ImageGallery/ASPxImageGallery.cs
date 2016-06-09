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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum ImageGalleryImageLocation { FullscreenViewer, FullscreenViewerNavigationBar, Gallery };
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxImageGallery"),
	Designer("DevExpress.Web.Design.ASPxImageGalleryDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation), System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxImageGallery.bmp"),
	ToolboxData(@"<{0}:ASPxImageGallery runat=""server""><SettingsFolder ImageCacheFolder=""~\Thumb\"" /></{0}:ASPxImageGallery>")]
	public class ASPxImageGallery : ASPxDataViewBase, ISupportsFolderBinding, IControlDesigner {
		internal const string ScriptResourceName = WebScriptsResourcePath + "ImageGallery.js";
		private ImageGalleryDataHelper dataHelper = null;
		private FolderBindingHelper folderBindingHelper = null;
		private ImageGalleryFolderSettings settingsFolder = null;
		private ImageGalleryFullscreenViewerSettings settingsFullscreenViewer = null;
		private ITemplate itemTextTemplate = null;
		private ITemplate fullscreenViewerTextTemplate = null;
		private ITemplate fullscreenViewerItemTextTemplate = null;
		private ImageGalleryFullscreenViewerStyles stylesFullscreenViewer = null;
		private ImageGalleryFullscreenViewerImages imagesFullscreenViewer = null;
		private ImageGalleryFullscreenViewerNavigationBarStyles stylesFullscreenViewerNavigationBar = null;
		private ImageGalleryFullscreenViewerNavigationBarImages imagesFullscreenViewerNavigationBar = null;
		private static readonly object EventItemDataBound = new object();
		private static readonly object EventCustomImageProcessing = new object();
		public ASPxImageGallery()
			: base() {
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageGalleryThumbnailTemplateContainer))]
		public ITemplate ItemTextTemplate {
			get { return itemTextTemplate; }
			set {
				itemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageGalleryFullscreenViewerItemTemplateContainer))]
		public ITemplate FullscreenViewerItemTextTemplate {
			get { return fullscreenViewerItemTextTemplate; }
			set {
				fullscreenViewerItemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageGalleryFullscreenViewerItemTemplateContainer))]
		public ITemplate FullscreenViewerTextTemplate {
			get { return fullscreenViewerTextTemplate; }
			set {
				fullscreenViewerTextTemplate = value;
				TemplatesChanged();
			}
		}
		protected override object GetCallbackResult() {
			Hashtable callbackResult = base.GetCallbackResult() as Hashtable;
			if(IsCustomCallback() && SettingsFullscreenViewer.Visible) {
				var control = FindControlHelper.LookupControlRecursive(this, ImageGalleryContstants.FullscreenViewerPopupID);
				callbackResult["fv"] = control == null ? "" : RenderUtils.GetRenderResult(control);
			}
			callbackResult["items"] = CreateClientItems();
			return callbackResult;
		}
		protected internal virtual ImageGalleryDataHelper GalleryDataHelper {
			get {
				if(dataHelper == null)
					dataHelper = new ImageGalleryDataHelper(this);
				return dataHelper;
			}
		}
		protected virtual FolderBindingHelper FolderBindingHelper {
			get {
				if(folderBindingHelper == null)
					folderBindingHelper = new FolderBindingHelper(this);
				return folderBindingHelper;
			}
		}
		protected internal string FolderBindingDigest {
			get { return GetStringProperty("FolderBindingDigest", string.Empty); }
			set { SetStringProperty("FolderBindingDigest", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGallerySettingsFolder"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ImageGalleryFolderSettings SettingsFolder {
			get {
				if(settingsFolder == null)
					settingsFolder = new ImageGalleryFolderSettings(this);
				return settingsFolder;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGallerySettingsFullscreenViewer"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ImageGalleryFullscreenViewerSettings SettingsFullscreenViewer {
			get {
				if(settingsFullscreenViewer == null)
					settingsFullscreenViewer = new ImageGalleryFullscreenViewerSettings(this);
				return settingsFullscreenViewer;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryTarget"),
#endif
		Category("Misc"), DefaultValue(""), Localizable(false), TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryLayout"),
#endif
 Category("Layout"),
		DefaultValue(Layout.Table), AutoFormatDisable, RefreshProperties(RefreshProperties.All)]
		public Layout Layout {
			get { return LayoutInternal; }
			set { LayoutInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryThumbnailWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultThumbnailWidth), Localizable(false), AutoFormatDisable]
		public Unit ThumbnailWidth {
			get { return GetUnitProperty("ThumbnailWidth", new Unit(ImageGalleryContstants.DefaultThumbnailWidth)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ThumbnailWidth");
				SetUnitProperty("ThumbnailWidth", new Unit(ImageGalleryContstants.DefaultThumbnailWidth), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryThumbnailHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultThumbnailHeight), Localizable(false), AutoFormatDisable]
		public Unit ThumbnailHeight {
			get { return GetUnitProperty("ThumbnailHeight", new Unit(ImageGalleryContstants.DefaultThumbnailHeight)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ThumbnailHeight");
				SetUnitProperty("ThumbnailHeight", new Unit(ImageGalleryContstants.DefaultThumbnailHeight), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryAllowExpandText"),
#endif
		Category("Behavior"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool AllowExpandText {
			get { return GetBoolProperty("AllowExpandText", true); }
			set { SetBoolProperty("AllowExpandText", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryThumbnailImageSizeMode"),
#endif
		Category("Behavior"), DefaultValue(ImageSizeMode.FillAndCrop), Localizable(false), AutoFormatDisable]
		public ImageSizeMode ThumbnailImageSizeMode {
			get { return (ImageSizeMode)GetEnumProperty("ThumbnailImageSizeMode", ImageSizeMode.FillAndCrop); }
			set { SetEnumProperty("ThumbnailImageSizeMode", ImageSizeMode.FillAndCrop, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryTextVisibility"),
#endif
		Category("Behavior"), DefaultValue(ElementVisibilityMode.OnMouseOver), Localizable(false), AutoFormatDisable]
		public ElementVisibilityMode TextVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("TextVisibility", ElementVisibilityMode.OnMouseOver); }
			set {
				SetEnumProperty("TextVisibility", ElementVisibilityMode.OnMouseOver, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryUseHash"),
#endif
		Category("Behavior"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool UseHash {
			get { return GetBoolProperty("UseHash", true); }
			set { SetBoolProperty("UseHash", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryImagesFullscreenViewer"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryFullscreenViewerImages ImagesFullscreenViewer {
			get {
				if(imagesFullscreenViewer == null)
					imagesFullscreenViewer = new ImageGalleryFullscreenViewerImages(this);
				return imagesFullscreenViewer;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryImagesFullscreenViewerNavigationBar"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryFullscreenViewerNavigationBarImages ImagesFullscreenViewerNavigationBar {
			get {
				if(imagesFullscreenViewerNavigationBar == null)
					imagesFullscreenViewerNavigationBar = new ImageGalleryFullscreenViewerNavigationBarImages(this);
				return imagesFullscreenViewerNavigationBar;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryImages Images {
			get { return ImagesInternal as ImageGalleryImages; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		protected override ImagesBase CreateImages() {
			return new ImageGalleryImages(this);
		}
		protected virtual internal ButtonImageProperties GetNavigationBarMarkerImage() {
			ButtonImageProperties sprite = new ButtonImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.NavigationBarMarkerImageName));
			sprite.MergeWith(ImagesFullscreenViewer.NavigationBarMarker);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		protected virtual internal ButtonImageProperties GetCloseButtonImage() {
			ButtonImageProperties sprite = new ButtonImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.CloseButtonImageName));
			sprite.MergeWith(ImagesFullscreenViewer.CloseButton);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		protected virtual internal ButtonImageProperties GetPrevButtonImage() {
			ButtonImageProperties sprite = new ButtonImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.PrevButtonImageName));
			sprite.MergeWith(ImagesFullscreenViewer.PrevButton);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		protected virtual internal ButtonImageProperties GetNextButtonImage() {
			ButtonImageProperties sprite = new ButtonImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.NextButtonImageName));
			sprite.MergeWith(ImagesFullscreenViewer.NextButton);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		protected virtual internal ImageProperties GetPlayButtonImage() {
			ImageProperties sprite = new ImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.PlayButtonImageName));
			sprite.MergeWith(ImagesFullscreenViewer.PlayButton);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		protected virtual internal ImageProperties GetPauseButtonImage() {
			ImageProperties sprite = new ImageProperties();
			sprite.MergeWith(ImagesFullscreenViewer.GetImageProperties(Page, ImageGalleryFullscreenViewerImages.PauseButtonImageName));
			sprite.MergeWith(ImagesFullscreenViewer.PauseButton);
			sprite.Url = ResolveUrl(sprite.Url);
			sprite.UrlPressed = ResolveUrl(sprite.UrlPressed);
			sprite.UrlHottracked = ResolveUrl(sprite.UrlHottracked);
			sprite.UrlDisabled = ResolveUrl(sprite.UrlDisabled);
			return sprite;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryStylesFullscreenViewer"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryFullscreenViewerStyles StylesFullscreenViewer {
			get {
				if(stylesFullscreenViewer == null)
					stylesFullscreenViewer = new ImageGalleryFullscreenViewerStyles(this);
				return stylesFullscreenViewer;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryStylesFullscreenViewerNavigationBar"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryFullscreenViewerNavigationBarStyles StylesFullscreenViewerNavigationBar {
			get {
				if(stylesFullscreenViewerNavigationBar == null)
					stylesFullscreenViewerNavigationBar = new ImageGalleryFullscreenViewerNavigationBarStyles(this);
				return stylesFullscreenViewerNavigationBar;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageGalleryStyles Styles {
			get { return (ImageGalleryStyles)StylesInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerStyle PagerStyle {
			get { return Styles.Pager; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewContentStyle ContentStyle {
			get { return Styles.Content; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewItemStyle ItemStyle {
			get { return Styles.Item; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerButtonStyle PagerButtonStyle {
			get { return Styles.PagerButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerTextStyle PagerCurrentPageNumberStyle {
			get { return Styles.PagerCurrentPageNumber; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerButtonStyle PagerDisabledButtonStyle {
			get { return Styles.PagerDisabledButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerTextStyle PagerPageNumberStyle {
			get { return Styles.PagerPageNumber; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerTextStyle PagerSummaryStyle {
			get { return Styles.PagerSummary; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PagerPageSizeItemStyle PagerPageSizeItemStyle {
			get { return Styles.PagerPageSizeItem; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewContentStyle PagerPanelStyle {
			get { return Styles.PagerPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewPagerShowMoreItemsContainerStyle PagerShowMoreItemsContainerStyle {
			get { return Styles.PagerShowMoreItemsContainer; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewEmptyDataStyle EmptyDataStyle {
			get { return Styles.EmptyData; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		protected override StylesBase CreateStyles() {
			return new ImageGalleryStyles(this);
		}
		protected internal AppearanceStyle GetThumbnailTextAreaStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultThumbnailTextAreaStyle());
			style.CopyFrom(Styles.ThumbnailTextArea);
			return style;
		}
		protected internal AppearanceStyle GetOverflowPanelStyle() {
			return StylesFullscreenViewer.GetDefaultOverflowPanelStyle();
		}
		protected internal AppearanceStyle GetThumbnailBorderStyle() {
			AppearanceStyle style = Styles.GetDefaultThumbnailBorderStyle();
			style.Border.Assign(base.GetItemStyle().Border);
			return style;
		}
		protected internal AppearanceStyle GetThumbnailWrapperStyle() {
			return Styles.GetDefaultThumbnailWrapperStyle();
		}
		protected internal AppearanceStyle GetImageSliderWrapperStyle() {
			return StylesFullscreenViewer.GetDefaultImageSliderWrapperStyle();
		}
		protected internal AppearanceStyle GetBottomPanelStyle() {
			return StylesFullscreenViewer.GetDefaultBottomPanelStyle();
		}
		protected internal AppearanceStyle GetPlayPauseButtonWrapperStyle() {
			return StylesFullscreenViewer.GetDefaultPlayPauseButtonWrapperStyle();
		}
		protected internal AppearanceStyle GetNavigationBarMarkerStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultNavigationBarStyle());
			style.CopyFrom(StylesFullscreenViewer.NavigationBarMarker);
			return style;
		}
		protected internal AppearanceStyle GetPlayPauseButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultPlayPauseButtonStyle());
			style.CopyFrom(StylesFullscreenViewer.PlayPauseButton);
			return style;
		}
		protected internal AppearanceStyle GetFullscrenViewerTextAreaStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultTextAreaStyle());
			style.CopyFrom(StylesFullscreenViewer.TextArea);
			return style;
		}
		protected internal ImageGalleryButtonStyle GetCloseButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultCloseButtonStyle());
			style.CopyFrom(StylesFullscreenViewer.CloseButton);
			return style;
		}
		protected internal AppearanceStyle GetCloseButtonWrapperStyle() {
			return StylesFullscreenViewer.GetDefaultCloseButtonWrapperStyle();
		}
		protected internal ImageGalleryButtonStyle GetPrevButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultPrevButtonStyle());
			style.CopyFrom(StylesFullscreenViewer.PrevButton);
			return style;
		}
		protected internal ImageGalleryButtonStyle GetNextButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(StylesFullscreenViewer.GetDefaultNextButtonStyle());
			style.CopyFrom(StylesFullscreenViewer.NextButton);
			return style;
		}
		protected internal override DataViewItemStyle GetItemStyle() {
			DataViewItemStyle style = base.GetItemStyle();
			style.Border.Reset();
			style.Width = ThumbnailWidth;
			style.Height = ThumbnailHeight;
			return style;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			if(SettingsFullscreenViewer.ShowCloseButton)
				helper.AddStyle(GetCloseButtonStyle().HoverStyle, ImageGalleryContstants.GetFullscreenViewerCloseButtonID(), new string[0],
					GetCloseButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
			if(SettingsFullscreenViewer.NavigationButtonVisibility != ElementVisibilityMode.None) {
				helper.AddStyle(GetPrevButtonStyle().HoverStyle, ImageGalleryContstants.GetFullscreenViewerPrevButtonID(), new string[0],
					GetPrevButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
				helper.AddStyle(GetNextButtonStyle().HoverStyle, ImageGalleryContstants.GetFullscreenViewerNextButtonID(), new string[0],
					GetNextButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
			}
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			if(SettingsFullscreenViewer.ShowCloseButton)
				helper.AddStyle(GetCloseButtonStyle().PressedStyle, ImageGalleryContstants.GetFullscreenViewerCloseButtonID(), new string[0],
					GetCloseButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
			if(SettingsFullscreenViewer.NavigationButtonVisibility != ElementVisibilityMode.None) {
				helper.AddStyle(GetPrevButtonStyle().PressedStyle, ImageGalleryContstants.GetFullscreenViewerPrevButtonID(), new string[0],
					GetPrevButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
				helper.AddStyle(GetNextButtonStyle().PressedStyle, ImageGalleryContstants.GetFullscreenViewerNextButtonID(), new string[0],
					GetNextButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
			}
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			if(SettingsFullscreenViewer.ShowCloseButton)
				helper.AddStyle(GetCloseButtonStyle().DisabledStyle, ImageGalleryContstants.GetFullscreenViewerCloseButtonID(), new string[0],
					GetCloseButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
			if(SettingsFullscreenViewer.NavigationButtonVisibility != ElementVisibilityMode.None) {
				helper.AddStyle(GetPrevButtonStyle().DisabledStyle, ImageGalleryContstants.GetFullscreenViewerPrevButtonID(), new string[0],
					GetPrevButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
				helper.AddStyle(GetNextButtonStyle().DisabledStyle, ImageGalleryContstants.GetFullscreenViewerNextButtonID(), new string[0],
					GetNextButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
			}
		}
		IEnumerable GetPreparedData(IEnumerable enumerable) {
			var listServer = enumerable as IListServer;
			return listServer != null ? listServer.GetAllFilteredAndSortedRows() : enumerable;
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable enumerable) {
			if(enumerable == null)
				return;
			if(!DesignMode && SettingsFullscreenViewer.Visible) {
				Items.Clear();
				enumerable = GetPreparedData(enumerable);
				foreach(var item in enumerable)
					DataBindItem(item);
			} else
				base.PerformDataBinding(dataHelperName, enumerable);
		}
		protected internal override bool IsAllowDataSourcePaging() {
			if(SettingsFullscreenViewer.Visible)
				return false;
			return base.IsAllowDataSourcePaging();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryImageContentBytesField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageContentBytesField {
			get { return GetStringProperty("ImageContentBytesField", ""); }
			set {
				SetStringProperty("ImageContentBytesField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryFullscreenViewerThumbnailUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string FullscreenViewerThumbnailUrlField {
			get { return GetStringProperty("FullscreenViewerThumbnailUrlField", ""); }
			set {
				SetStringProperty("FullscreenViewerThumbnailUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryThumbnailUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ThumbnailUrlField {
			get { return GetStringProperty("ThumbnailUrlField", ""); }
			set {
				SetStringProperty("ThumbnailUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryImageUrlField"),
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
	DevExpressWebLocalizedDescription("ASPxImageGalleryTextField"),
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
	DevExpressWebLocalizedDescription("ASPxImageGalleryNameField"),
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
	DevExpressWebLocalizedDescription("ASPxImageGalleryFullscreenViewerTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string FullscreenViewerTextField {
			get { return GetStringProperty("FullscreenViewerTextField", ""); }
			set {
				SetStringProperty("FullscreenViewerTextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryNavigateUrlField"),
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
	DevExpressWebLocalizedDescription("ASPxImageGalleryImageUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string ImageUrlFormatString {
			get { return GetStringProperty("ImageUrlFormatString", "{0}"); }
			set { SetStringProperty("ImageUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryThumbnailUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string ThumbnailUrlFormatString {
			get { return GetStringProperty("ThumbnailUrlFormatString", "{0}"); }
			set { SetStringProperty("ThumbnailUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryFullscreenViewerThumbnailUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string FullscreenViewerThumbnailUrlFormatString {
			get { return GetStringProperty("FullscreenViewerThumbnailUrlFormatString", "{0}"); }
			set { SetStringProperty("FullscreenViewerThumbnailUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ImageGalleryItemCollection Items {
			get { return DataItems as ImageGalleryItemCollection; }
		}
		protected override DataViewItemCollection CreateItemCollection(ASPxDataViewBase control) {
			return new ImageGalleryItemCollection(control as ASPxImageGallery);
		}
		protected override void DataBindItem(object obj) {
			ImageGalleryItem item = new ImageGalleryItem();
			DataBindItemProperties(obj, item);
			OnItemDataBound(item);
			DataItems.Add(item);
		}
		protected virtual void DataBindItemProperties(object obj, ImageGalleryItem item) {
			string FullscreenViewerThumbnailFieldName = string.IsNullOrEmpty(FullscreenViewerThumbnailUrlField) ? "FullscreenViewerThumbnailUrl" : FullscreenViewerThumbnailUrlField;
			string thumbnailUrlFieldName = string.IsNullOrEmpty(ThumbnailUrlField) ? "ThumbnailUrl" : ThumbnailUrlField;
			string imageUrlFieldName = string.IsNullOrEmpty(ImageUrlField) ? "ImageUrl" : ImageUrlField;
			string textFieldName = string.IsNullOrEmpty(TextField) ? "Text" : TextField;
			string fullscreenViewerTextFieldName = string.IsNullOrEmpty(FullscreenViewerTextField) ? "FullscreenViewerText" : FullscreenViewerTextField;
			string navigateUrlFieldName = string.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			string imageContentBytesFieldName = string.IsNullOrEmpty(ImageContentBytesField) ? "ImageContentBytes" : ImageContentBytesField;
			string nameFieldName = string.IsNullOrEmpty(NameField) ? "Name" : NameField;
			item.DataItem = obj;
			item.FullscreenViewerThumbnailUrl = GetFieldValue(obj, FullscreenViewerThumbnailFieldName, false, string.Empty).ToString();
			item.ThumbnailUrl = GetFieldValue(obj, thumbnailUrlFieldName, false, string.Empty).ToString();
			item.ImageUrl = GetFieldValue(obj, imageUrlFieldName, false, string.Empty).ToString();
			item.Text = GetFieldValue(obj, textFieldName, false, string.Empty).ToString();
			item.FullscreenViewerText = GetFieldValue(obj, fullscreenViewerTextFieldName, false, string.Empty).ToString();
			item.NavigateUrl = GetFieldValue(obj, navigateUrlFieldName, false, string.Empty).ToString();
			item.ImageContentBytes = (byte[])GetFieldValue(obj, imageContentBytesFieldName, false, null);
			item.Name = GetFieldValue(obj, nameFieldName, false, string.Empty).ToString();
			if(item.ImageContentBytes != null && string.IsNullOrEmpty(item.BinaryImageUrl)) {
				AutogeneratedImageUrls images = FolderBindingHelper.CreateThumbnails(item.ImageContentBytes);
				item.BinaryFullscreenViewerThumbnailUrl = images[GetFullscreenViewerNavBarImageInfo()];
				item.BinaryImageUrl = images[GetFullscreenViewerImageInfo()];
				item.BinaryThumbnailUrl = images[GetThumbnailImageInfo()];
			}
		}
		protected internal override ItemContentInfo CreateItemContentInfo() {
			return new ItemContentInfo();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryCustomImageProcessing"),
#endif
		Category("Data")]
		public event ImageGalleryCustomImageProcessingEventHandler CustomImageProcessing {
			add { Events.AddHandler(EventCustomImageProcessing, value); }
			remove { Events.RemoveHandler(EventCustomImageProcessing, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryItemDataBound"),
#endif
		Category("Data")]
		public event ImageGalleryItemEventHandler ItemDataBound {
			add { Events.AddHandler(EventItemDataBound, value); }
			remove { Events.RemoveHandler(EventItemDataBound, value); }
		}
		protected virtual internal void OnItemDataBound(ImageGalleryItem item) {
			ImageGalleryItemEventHandler handler = (ImageGalleryItemEventHandler)Events[EventItemDataBound];
			if(handler != null)
				handler(this, new ImageGalleryItemEventArgs(item));
		}
		protected virtual internal void OnCustomImageProcessing(System.Drawing.Graphics graphics, System.Drawing.Bitmap thumbnail, ImageGalleryImageLocation location) {
			ImageGalleryCustomImageProcessingEventHandler handler = (ImageGalleryCustomImageProcessingEventHandler)Events[EventCustomImageProcessing];
			if(handler != null)
				handler(this, new ImageGalleryCustomImageProcessingEventArgs(graphics, thumbnail, location));
		}
		protected internal bool HasCustomImageProcessingEvent() {
			return Events[EventCustomImageProcessing] != null;
		}
		protected override DataViewTableLayoutSettings CreateTableLayoutSettings() {
			return new ImageGalleryTableLayoutSettings(this);
		}
		protected override DataViewFlowLayoutSettings CreateFlowLayoutSettings() {
			return new ImageGalleryFlowLayoutSettings(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryPagerAlign"),
#endif
		Category("Paging"), DefaultValue(PagerAlign.Justify), AutoFormatEnable]
		public override PagerAlign PagerAlign {
			get { return (PagerAlign)GetEnumProperty("PagerAlign", PagerAlign.Justify); }
			set {
				SetEnumProperty("PagerAlign", PagerAlign.Justify, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGallerySettingsFlowLayout"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryFlowLayoutSettings SettingsFlowLayout {
			get { return SettingsFlowLayoutInternal as ImageGalleryFlowLayoutSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGallerySettingsTableLayout"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryTableLayoutSettings SettingsTableLayout {
			get { return SettingsTableLayoutInternal as ImageGalleryTableLayoutSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryPagerSettings"),
#endif
		Category("Paging"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageGalleryPagerSettings PagerSettings {
			get { return base.PagerSettings as ImageGalleryPagerSettings; }
		}
		protected override PagerSettingsEx CreatePagerSettings(IPropertiesOwner owner) {
			return new ImageGalleryPagerSettings(owner);
		}
		protected internal override ASPxPagerBase CreatePager() {
			return new ImageGalleryPager(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageGalleryClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ImageGalleryClientSideEvents ClientSideEvents {
			get { return (ImageGalleryClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ImageGalleryClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientImageGallery";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterImageControlUtilsScript();
			RegisterIncludeScript(typeof(ASPxImageGallery), ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!SettingsFullscreenViewer.KeyboardSupport)
				stb.AppendFormat("{0}.keyboardSupport = false;\n", localVarName);
			if(GetNavigationBarVisibility() != ElementVisibilityMode.OnMouseOver)
				stb.AppendFormat("{0}.navBarVisibility = {1};\n", localVarName, (int)GetNavigationBarVisibility());
			if(GetNavigationButtonVisibility() != ElementVisibilityMode.OnMouseOver)
				stb.AppendFormat("{0}.navBtnVisibility = {1};\n", localVarName, (int)GetNavigationButtonVisibility());
			if(!UseHash)
				stb.AppendFormat("{0}.useHash = false;\n", localVarName);
			if(GalleryDataHelper.HasFullscreenViewerStaticTextTemplate)
				stb.AppendFormat("{0}.hasFVTextTemplate = true;\n", localVarName);
			else if(GalleryDataHelper.HasFullscreenViewerTextTemplate)
				stb.AppendFormat("{0}.hasItemFVTextTemplate = true;\n", localVarName);
			if(!SettingsFullscreenViewer.EnablePagingByClick)
				stb.AppendFormat("{0}.enablePagingByClick = false;\n", localVarName);
			ElementVisibilityMode thumbnailTextVisibility = GalleryDataHelper.GetThumbnailTextVisibility();
			if(thumbnailTextVisibility != ElementVisibilityMode.OnMouseOver)
				stb.AppendFormat("{0}.thumbTxtVisibility = {1};\n", localVarName, (int)thumbnailTextVisibility);
			if(ThumbnailImageSizeMode != ImageSizeMode.FillAndCrop)
				stb.AppendFormat("{0}.thumbImgSizeMode = {1};\n", localVarName, (int)ThumbnailImageSizeMode);
			if(!AllowExpandText)
				stb.AppendFormat("{0}.allowExpandText = false;\n", localVarName);
			if(Layout == Layout.Flow)
				stb.AppendFormat("{0}.isFlowLayout = true;\n", localVarName);
			GetClientDataItems(stb, localVarName);
		}
		protected virtual void GetClientDataItems(StringBuilder stb, string localVarName) {
			List<object> clientItems = CreateClientItems();
			if(clientItems.Count > 0)
				stb.AppendFormat("{0}.items = {1};\n", localVarName, HtmlConvertor.ToJSON(clientItems));
		}
		protected virtual List<object> CreateClientItems() {
			List<object> clientItems = new List<object>();
			foreach(ImageGalleryItem item in VisibleItemsList)
				clientItems.Add(CreateClientItem(item));
			return clientItems;
		}
		protected virtual object CreateClientItem(ImageGalleryItem item) {
			Hashtable result = new Hashtable();
			result["i"] = item.Index;
			if(!SettingsFullscreenViewer.Visible)
				AddNonEmptyString(result, "n", GalleryDataHelper.GetDataViewItemName(item));
			AddNonEmptyString(result, "tu", GalleryDataHelper.GetThumbnailUrl(item), true);
			return result;
		}
		void AddNonEmptyString(Hashtable ht, string key, string value, bool resolveUrl = false) {
			if(!string.IsNullOrEmpty(value)) {
				value = ImageUtilsHelper.EncodeImageUrl(value);
				ht.Add(key, resolveUrl ? ResolveClientUrl(value) : value);
			}
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override bool HasPressedScripts() {
			return true;
		}
		protected override bool HasDisabledScripts() {
			return true;
		}
		protected override bool HasSpriteCssFile() {
			return true;
		}
		protected override bool IsAnimationScriptNeeded() {
			return true;
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] {
					SettingsFolder,
					SettingsFullscreenViewer,
					StylesFullscreenViewer,
					ImagesFullscreenViewer,
					StylesFullscreenViewerNavigationBar,
					ImagesFullscreenViewerNavigationBar
				});
		}
		public override void DataBind() {
			base.DataBind();
			if(SettingsFullscreenViewer.Visible)
				ValidatePageIndex();
		}
		protected override void CreateControlHierarchy() {
			FolderBindingHelper.CreateItemsFromFolder();
			base.CreateControlHierarchy();
		}
		protected override DVMainControl CreateMainControl() {
			return new ImageGalleryMainControl(this);
		}
		protected internal override void CreateItemControl(ItemContentInfo contentInfo, Control parent) {
			parent.Controls.Add(CreateThumbnailItemControl(contentInfo.Item as ImageGalleryItem));
		}
		protected ThumbnailItemControlBase CreateThumbnailItemControl(ImageGalleryItem item) {
			return DesignMode ? new ThumbnailItemControlDesignMode(item) as ThumbnailItemControlBase : new ThumbnailItemControl(item);
		}
		protected internal ElementVisibilityMode GetNavigationBarVisibility() {
			if(SettingsFullscreenViewer.NavigationBarVisibility == ElementVisibilityMode.Faded)
				return ElementVisibilityMode.OnMouseOver;
			return SettingsFullscreenViewer.NavigationBarVisibility;
		}
		protected internal ElementVisibilityMode GetNavigationButtonVisibility() {
			if(SettingsFullscreenViewer.NavigationButtonVisibility == ElementVisibilityMode.Faded)
				return ElementVisibilityMode.OnMouseOver;
			return SettingsFullscreenViewer.NavigationButtonVisibility;
		}
		string ISupportsFolderBinding.ImageSourceFolder {
			get { return SettingsFolder.ImageSourceFolder; }
		}
		string ISupportsFolderBinding.ImageCacheFolder {
			get { return SettingsFolder.ImageCacheFolder; }
		}
		string ISupportsFolderBinding.ImageCacheFolderPropertyDisplayName {
			get { return "ImageGalleryFolderSettings.ImageCacheFolder"; }
		}
		string ISupportsFolderBinding.Digest {
			get { return FolderBindingDigest; }
			set { FolderBindingDigest = value; }
		}
		void ISupportsFolderBinding.ResetDigest() {
			(this as ISupportsFolderBinding).Digest = "";
			LayoutChanged();
		}
		IEnumerable<AutogeneratedImageInfo> ISupportsFolderBinding.GetOutputImagesInfo() {
			return new AutogeneratedImageInfo[]{
				GetThumbnailImageInfo(), GetFullscreenViewerImageInfo() , GetFullscreenViewerNavBarImageInfo()
			};
		}
		private AutogeneratedImageInfo GetFullscreenViewerNavBarImageInfo() {
			Size fullscrnThumbSize = new Size((int)SettingsFullscreenViewer.ThumbnailWidth.Value, (int)SettingsFullscreenViewer.ThumbnailHeight.Value);
			return new AutogeneratedImageInfo(fullscrnThumbSize, ImageSizeMode.FillAndCrop, ImageGalleryImageLocation.FullscreenViewerNavigationBar);
		}
		private AutogeneratedImageInfo GetFullscreenViewerImageInfo() {
			Size fullscrnImgSize = new Size((int)SettingsFullscreenViewer.ImageWidth.Value, (int)SettingsFullscreenViewer.ImageHeight.Value);
			return new AutogeneratedImageInfo(fullscrnImgSize, SettingsFullscreenViewer.ImageSizeMode, ImageGalleryImageLocation.FullscreenViewer);
		}
		private AutogeneratedImageInfo GetThumbnailImageInfo() {
			Size thumbSize = new Size((int)ThumbnailWidth.Value, (int)ThumbnailHeight.Value);
			return new AutogeneratedImageInfo(thumbSize, ThumbnailImageSizeMode, ImageGalleryImageLocation.Gallery);
		}
		CustomImageProcessingMethod ISupportsFolderBinding.GetCustomImageProcessingMethod(AutogeneratedImageInfo info) {
			ImageGalleryImageLocation location;
			if(info.CustomData != null && Enum.TryParse<ImageGalleryImageLocation>(info.CustomData.ToString(), out location)) {
				CustomImageProcessingMethod customImageProcessing =
					delegate(Graphics graphics, Bitmap thumbnail) {
						OnCustomImageProcessing(graphics, thumbnail, location);
					};
				return customImageProcessing;
			}
			return null;
		}
		void ISupportsFolderBinding.BeginItemsCreate() {
			Items.Clear();
		}
		void ISupportsFolderBinding.CreateAndInitializeItem(AutogeneratedImageUrls images) {
			ImageGalleryItem item = new ImageGalleryItem();
			item.FullscreenViewerThumbnailUrl = images[GetFullscreenViewerNavBarImageInfo()];
			item.ImageUrl = images[GetFullscreenViewerImageInfo()];
			item.ThumbnailUrl = images[GetThumbnailImageInfo()];
			Items.Add(item);
			OnItemDataBound(item);
		}
		void ISupportsFolderBinding.CompleteItemsCreate() {
			RaiseDataBound();
		}
		public void UpdateImageCacheFolder() {
			FolderBindingHelper.UpdateImageCacheFolder();
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ImageGalleryItemsOwner"; } }
		protected override DataHelperBase CreateDataHelper(string name) {
			return new ASPxImageGalleryDataHelper(this, name);
		}
	}
}
namespace DevExpress.Web.Internal {
	public class ASPxImageGalleryDataHelper : DataViewBaseDataHelper {
		protected ASPxImageGallery ImageGallery {
			get { return (ASPxImageGallery)Owner; }
		}
		public ASPxImageGalleryDataHelper(ASPxImageGallery imageGallery, string name)
			: base(imageGallery, name) {
		}
		public override bool CanServerPaging {
			get {
				return !(ImageGallery.DesignMode || ImageGallery.SettingsFullscreenViewer.Visible) && base.CanServerPaging;
			}
		}
	}
}
