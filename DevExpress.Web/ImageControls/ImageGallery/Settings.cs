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

using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class ImageGalleryFolderSettings : PropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFolderSettingsImageSourceFolder"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue("")]
		public string ImageSourceFolder {
			get { return GetStringProperty("ImageSourceFolder", ""); }
			set {
				SetStringProperty("ImageSourceFolder", "", value);
				FolderChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFolderSettingsImageCacheFolder"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue("")]
		public string ImageCacheFolder {
			get { return GetStringProperty("ImageCacheFolder", string.Empty); }
			set {
				SetStringProperty("ImageCacheFolder", string.Empty, value);
				FolderChanged();
			}
		}
		public ImageGalleryFolderSettings(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageGalleryFolderSettings src = source as ImageGalleryFolderSettings;
				if(src != null) {
					ImageSourceFolder = src.ImageSourceFolder;
					ImageCacheFolder = src.ImageCacheFolder;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected void FolderChanged() {
			(Owner as ISupportsFolderBinding).ResetDigest();
		}
	}
	public class ImageGalleryFullscreenViewerSettings : PropertiesBase {
		internal const int DefaultInterval = 5000;
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsImageSizeMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(ImageSizeMode.ActualSizeOrFit)]
		public ImageSizeMode ImageSizeMode {
			get { return (ImageSizeMode)GetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit); }
			set { SetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsImageWidth"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultFullscreenViewerWidth)]
		public Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", new Unit(ImageGalleryContstants.DefaultFullscreenViewerWidth)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ImageWidth");
				SetUnitProperty("ImageWidth", new Unit(ImageGalleryContstants.DefaultFullscreenViewerWidth), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsImageHeight"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultFullscreenViewerHeight)]
		public Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", new Unit(ImageGalleryContstants.DefaultFullscreenViewerHeight)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ImageHeight");
				SetUnitProperty("ImageHeight", new Unit(ImageGalleryContstants.DefaultFullscreenViewerHeight), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsThumbnailWidth"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultFullscreenViewerThumbnailWidth)]
		public Unit ThumbnailWidth {
			get { return GetUnitProperty("ThumbnailWidth", new Unit(ImageGalleryContstants.DefaultFullscreenViewerThumbnailWidth)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ThumbnailWidth");
				SetUnitProperty("ThumbnailWidth", new Unit(ImageGalleryContstants.DefaultFullscreenViewerThumbnailWidth), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsThumbnailHeight"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), ImageGalleryContstants.DefaultFullscreenViewerThumbnailHeight)]
		public Unit ThumbnailHeight {
			get { return GetUnitProperty("ThumbnailHeight", new Unit(ImageGalleryContstants.DefaultFullscreenViewerThumbnailHeight)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ThumbnailHeight");
				SetUnitProperty("ThumbnailHeight", new Unit(ImageGalleryContstants.DefaultFullscreenViewerThumbnailHeight), value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsVisible"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsNavigationBarVisibility"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(ElementVisibilityMode.OnMouseOver)]
		public ElementVisibilityMode NavigationBarVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("NavigationBarVisibility", ElementVisibilityMode.OnMouseOver); }
			set {
				SetEnumProperty("NavigationBarVisibility", ElementVisibilityMode.OnMouseOver, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsNavigationButtonVisibility"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(ElementVisibilityMode.OnMouseOver)]
		public ElementVisibilityMode NavigationButtonVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("NavigationButtonVisibility", ElementVisibilityMode.OnMouseOver); }
			set {
				SetEnumProperty("NavigationButtonVisibility", ElementVisibilityMode.OnMouseOver, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsShowTextArea"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool ShowTextArea {
			get { return GetBoolProperty("ShowTextArea", true); }
			set {
				SetBoolProperty("ShowTextArea", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsKeyboardSupport"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool KeyboardSupport {
			get { return GetBoolProperty("KeyboardSupport", true); }
			set { SetBoolProperty("KeyboardSupport", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsShowCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool ShowCloseButton {
			get { return GetBoolProperty("ShowCloseButton", true); }
			set {
				SetBoolProperty("ShowCloseButton", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsEnablePagingGestures"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool EnablePagingGestures {
			get { return GetBoolProperty("EnablePagingGestures", true); }
			set { SetBoolProperty("EnablePagingGestures", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsAnimationType"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(AnimationType.Slide)]
		public AnimationType AnimationType {
			get { return (AnimationType)GetEnumProperty("AnimationType", AnimationType.Slide); }
			set { SetEnumProperty("AnimationType", AnimationType.Slide, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsImageLoadMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(ImageLoadMode.Auto)]
		public ImageLoadMode ImageLoadMode {
			get { return (ImageLoadMode)GetEnumProperty("ImageLoadMode", ImageLoadMode.Auto); }
			set { SetEnumProperty("ImageLoadMode", ImageLoadMode.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsEnablePagingByClick"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool EnablePagingByClick {
			get { return GetBoolProperty("EnablePagingByClick", true); }
			set { SetBoolProperty("EnablePagingByClick", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsShowPlayPauseButton"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool ShowPlayPauseButton {
			get { return GetBoolProperty("ShowPlayPauseButton", true); }
			set {
				SetBoolProperty("ShowPlayPauseButton", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerSettingsSlideShowInterval"),
#endif
		DefaultValue(ImageSliderSlideShowSettings.DefaultInterval), NotifyParentProperty(true), AutoFormatDisable]
		public int SlideShowInterval
		{
			get { return GetIntProperty("SlideShowInterval", DefaultInterval); }
			set
			{
				CommonUtils.CheckNegativeOrZeroValue(value, "SlideShowInterval");
				SetIntProperty("SlideShowInterval", DefaultInterval, value);
			}
		}
		public ImageGalleryFullscreenViewerSettings(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageGalleryFullscreenViewerSettings src = source as ImageGalleryFullscreenViewerSettings;
				if (src != null) {
					Visible = src.Visible;
					ShowTextArea = src.ShowTextArea;
					KeyboardSupport = src.KeyboardSupport;
					ShowCloseButton = src.ShowCloseButton;
					NavigationBarVisibility = src.NavigationBarVisibility;
					NavigationButtonVisibility = src.NavigationButtonVisibility;
					EnablePagingByClick = src.EnablePagingByClick;
					EnablePagingGestures = src.EnablePagingGestures;
					AnimationType = src.AnimationType;
					ImageLoadMode = src.ImageLoadMode;
					SlideShowInterval = src.SlideShowInterval;
					ShowPlayPauseButton = src.ShowPlayPauseButton;
					ImageWidth = src.ImageWidth;
					ImageHeight = src.ImageHeight;
					ThumbnailHeight = src.ThumbnailHeight;
					ThumbnailWidth = src.ThumbnailWidth;
					ImageSizeMode = src.ImageSizeMode;
				}
			} finally {
				EndUpdate();
			}
		}
		protected void LayoutChanged() {
			if(Owner != null)
				(Owner as ASPxImageGallery).LayoutChanged();
		}
	}
	public class ImageGalleryTableLayoutSettings : DataViewTableLayoutSettings {
		const int ColumnCountConstant = 4;
		protected override int DefaultColumnCount {
			get { return ColumnCountConstant; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryTableLayoutSettingsColumnCount"),
#endif
		DefaultValue(ColumnCountConstant)]
		public override int ColumnCount
		{
			get { return base.ColumnCount; }
			set { base.ColumnCount = value; }
		}
		public ImageGalleryTableLayoutSettings(ASPxImageGallery owner) :
			base(owner) { }
	}
	public class ImageGalleryFlowLayoutSettings : DataViewFlowLayoutSettings {
		const int ItemsPerPageConstant = 12;
		protected override int DefaultItemsPerPage {
			get { return ItemsPerPageConstant; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFlowLayoutSettingsItemsPerPage"),
#endif
		DefaultValue(ItemsPerPageConstant)]
		public override int ItemsPerPage
		{
			get { return base.ItemsPerPage; }
			set { base.ItemsPerPage = value; }
		}
		public ImageGalleryFlowLayoutSettings(ASPxImageGallery owner) :
			base(owner) { }
	}
	public class ImageGalleryPagerSettings : PagerSettingsEx, IDataViewEndlessPagingSettigns {
		protected internal ASPxDataViewBase DataView {
			get {
				if(Owner is ASPxDataViewBase)
					return Owner as ASPxDataViewBase;
				else if(Owner is DVPager)
					return (Owner as DVPager).DataView;
				return null;
			}
		}
		public ImageGalleryPagerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryPagerSettingsEndlessPagingMode"),
#endif
		AutoFormatDisable, DefaultValue(DataViewEndlessPagingMode.Disabled), NotifyParentProperty(true)]
		public DataViewEndlessPagingMode EndlessPagingMode
		{
			get { return (DataViewEndlessPagingMode)GetEnumProperty("EndlessPagingMode", DataViewEndlessPagingMode.Disabled); }
			set
			{
				if (EndlessPagingMode != value)
				{
					SetEnumProperty("EndlessPagingMode", DataViewEndlessPagingMode.Disabled, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryPagerSettingsShowMoreItemsText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string ShowMoreItemsText
		{
			get { return GetStringProperty("ShowMoreItemsText", ""); }
			set { SetStringProperty("ShowMoreItemsText", "", value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as ImageGalleryPagerSettings;
			if(settings != null) {
				EndlessPagingMode = settings.EndlessPagingMode;
				ShowMoreItemsText = settings.ShowMoreItemsText;
			}
		}
		protected override AllButtonProperties CreateAllButtonProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerAllButtonProperties(owner);
		}
		protected override NextButtonProperties CreateNextButtonProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerNextButtonProperties(owner);
		}
		protected override PrevButtonProperties CreatePrevButtonProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerPrevButtonProperties(owner);
		}
		protected override FirstButtonProperties CreateFirstButtonProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerFirstButtonProperties(owner);
		}
		protected override LastButtonProperties CreateLastButtonProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerLastButtonProperties(owner);
		}
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new ImageGalleryPagerPageSizeItemSettings(owner);
		}
		protected override SummaryProperties CreateSummaryProperties(IPropertiesOwner owner) {
			return new ImageGalleryPagerSummaryProperties(owner);
		}
	}
	public class ImageGalleryPagerAllButtonProperties : AllButtonProperties {
		public ImageGalleryPagerAllButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_All)) {
		}
	}
	public class ImageGalleryPagerNextButtonProperties : NextButtonProperties {
		public ImageGalleryPagerNextButtonProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ImageGalleryPagerPrevButtonProperties : PrevButtonProperties {
		public ImageGalleryPagerPrevButtonProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ImageGalleryPagerFirstButtonProperties : FirstButtonProperties {
		public ImageGalleryPagerFirstButtonProperties(IPropertiesOwner owner)
			: base(owner, true) {
		}
	}
	public class ImageGalleryPagerLastButtonProperties : LastButtonProperties {
		public ImageGalleryPagerLastButtonProperties(IPropertiesOwner owner)
			: base(owner, true) {
		}
	}
	public class ImageGalleryPagerPageSizeItemSettings : PageSizeItemSettings {
		protected internal new ImageGalleryPagerSettings PagerSettings {
			get { return Owner as ImageGalleryPagerSettings; }
		}
		public ImageGalleryPagerPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
				this.fDefaultVisible = true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryPagerPageSizeItemSettingsVisible"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryPagerPageSizeItemSettingsShowAllItem"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public override bool ShowAllItem {
			get { return GetBoolProperty("ShowAllItem", true); }
			set {
				SetBoolProperty("ShowAllItem", true, value);
				Changed();
			}
		}
		protected override string[] GetDefaultPageSizeItems() {
			return IsTableLayout() ? new string[] { "3", "5", "10", "20" } : new string[] { "12", "25", "50" };
		}
		protected internal override string GetDefaultCaption() {
			if(IsTableLayout())
				return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerRowPerPage);
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerPageSize);
		}
		protected internal virtual bool IsTableLayout() {
			if(PagerSettings.DataView != null)
				return PagerSettings.DataView.LayoutInternal == Layout.Table;
			return false;
		}
	}
	public class ImageGalleryPagerSummaryProperties : SummaryProperties {
		public ImageGalleryPagerSummaryProperties(IPropertiesOwner owner)
			: base(owner, false) {
		}
	}
}
