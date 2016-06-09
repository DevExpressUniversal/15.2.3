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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ImageSliderImageAreaSettings : PropertiesBase {
		internal const int DefaultDuration = 400;
		[Obsolete("Use the ItemTextVisibilityMode property instead.")]
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsShowItemText"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowItemText {
			get { return ItemTextVisibility != ElementVisibilityMode.None ? true : false; }
			set { ItemTextVisibility = value ? ElementVisibilityMode.Always : ElementVisibilityMode.None; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsItemTextVisibility"),
#endif
		DefaultValue(ElementVisibilityMode.OnMouseOver), NotifyParentProperty(true), AutoFormatDisable]
		public ElementVisibilityMode ItemTextVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("ItemTextVisibility", ElementVisibilityMode.OnMouseOver); }
			set { SetEnumProperty("ItemTextVisibility", ElementVisibilityMode.OnMouseOver, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsNavigationButtonVisibility"),
#endif
		DefaultValue(ElementVisibilityMode.OnMouseOver), NotifyParentProperty(true), AutoFormatDisable]
		public ElementVisibilityMode NavigationButtonVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("NavigationButtonVisibility", ElementVisibilityMode.OnMouseOver); }
			set { SetEnumProperty("NavigationButtonVisibility", ElementVisibilityMode.OnMouseOver, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsImageSizeMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(ImageSizeMode.ActualSizeOrFit)]
		public ImageSizeMode ImageSizeMode {
			get { return (ImageSizeMode)GetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit); }
			set { SetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsAnimationType"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(AnimationType.Auto)]
		public AnimationType AnimationType {
			get { return (AnimationType)GetEnumProperty("AnimationType", AnimationType.Auto); }
			set { SetEnumProperty("AnimationType", AnimationType.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsNavigationDirection"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(NavigationDirection.Horizontal)]
		public NavigationDirection NavigationDirection {
			get { return (NavigationDirection)GetEnumProperty("NavigationDirection", NavigationDirection.Horizontal); }
			set { SetEnumProperty("NavigationDirection", NavigationDirection.Horizontal, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsAnimationDuration"),
#endif
		DefaultValue(DefaultDuration), NotifyParentProperty(true), AutoFormatDisable]
		public int AnimationDuration {
			get { return GetIntProperty("AnimationDuration", DefaultDuration); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "AnimationDuration");
				SetIntProperty("AnimationDuration", DefaultDuration, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaSettingsEnableLoopNavigation"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableLoopNavigation {
			get { return GetBoolProperty("EnableLoopNavigation", false); }
			set { SetBoolProperty("EnableLoopNavigation", false, value); }
		}
		public ImageSliderImageAreaSettings(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageSliderImageAreaSettings src = source as ImageSliderImageAreaSettings;
				if(src != null) {
					ItemTextVisibility = src.ItemTextVisibility;
					ImageSizeMode = src.ImageSizeMode;
					NavigationButtonVisibility = src.NavigationButtonVisibility;
					AnimationType = src.AnimationType;
					AnimationDuration = src.AnimationDuration;
					NavigationDirection = src.NavigationDirection;
					EnableLoopNavigation = src.EnableLoopNavigation;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ImageSliderNavigationBarSettings : PropertiesBase {
		internal const string DefaultItemSpacing = "5";
		internal const int DefaultVisibleItemsCount = 0;
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsMode"),
#endif
		DefaultValue(NavigationBarMode.Thumbnails), NotifyParentProperty(true), AutoFormatDisable]
		public NavigationBarMode Mode {
			get { return (NavigationBarMode)GetEnumProperty("Mode", NavigationBarMode.Thumbnails); }
			set { SetEnumProperty("Mode", NavigationBarMode.Thumbnails, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsPosition"),
#endif
		DefaultValue(NavigationBarPosition.Bottom), NotifyParentProperty(true), AutoFormatDisable]
		public NavigationBarPosition Position {
			get { return (NavigationBarPosition)GetEnumProperty("Position", NavigationBarPosition.Bottom); }
			set { SetEnumProperty("Position", NavigationBarPosition.Bottom, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsThumbnailsNavigationButtonPosition"),
#endif
		DefaultValue(ThumbnailNavigationBarButtonPosition.Inside), NotifyParentProperty(true), AutoFormatDisable]
		public ThumbnailNavigationBarButtonPosition ThumbnailsNavigationButtonPosition {
			get { return (ThumbnailNavigationBarButtonPosition)GetEnumProperty("ThumbnailsNavigationButtonPosition", ThumbnailNavigationBarButtonPosition.Inside); }
			set { SetEnumProperty("ThumbnailsNavigationButtonPosition", ThumbnailNavigationBarButtonPosition.Inside, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsThumbnailsModeNavigationButtonVisibility"),
#endif
		DefaultValue(ElementVisibilityMode.OnMouseOver), NotifyParentProperty(true), AutoFormatDisable]
		public ElementVisibilityMode ThumbnailsModeNavigationButtonVisibility
		{
			get { return (ElementVisibilityMode)GetEnumProperty("ThumbnailsModeNavigationButtonVisibility", ElementVisibilityMode.OnMouseOver); }
			set { SetEnumProperty("ThumbnailsModeNavigationButtonVisibility", ElementVisibilityMode.OnMouseOver, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsItemSpacing"),
#endif
		DefaultValue(typeof(Unit), DefaultItemSpacing), NotifyParentProperty(true), AutoFormatEnable]
		public Unit ItemSpacing {
			get { return GetUnitProperty("ItemSpacing", new Unit(DefaultItemSpacing)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ItemSpacing");
				SetUnitProperty("ItemSpacing", new Unit(DefaultItemSpacing), value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsPagingMode"),
#endif
		DefaultValue(NavigationBarPagingMode.Page), NotifyParentProperty(true), AutoFormatEnable]
		public NavigationBarPagingMode PagingMode {
			get { return (NavigationBarPagingMode)GetEnumProperty("PagingMode", NavigationBarPagingMode.Page); }
			set { SetEnumProperty("PagingMode", NavigationBarPagingMode.Page, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarSettingsVisibleItemsCount"),
#endif
		DefaultValue(DefaultVisibleItemsCount), NotifyParentProperty(true), AutoFormatEnable]
		public int VisibleItemsCount {
			get { return GetIntProperty("VisibleItemsCount", DefaultVisibleItemsCount); }
			set {
				UnitUtils.CheckNegativeUnit(value, "VisibleItemsCount");
				SetIntProperty("VisibleItemsCount", DefaultVisibleItemsCount, value);
			}
		}
		public ImageSliderNavigationBarSettings(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageSliderNavigationBarSettings src = source as ImageSliderNavigationBarSettings;
				if(src != null) {
					ItemSpacing = src.ItemSpacing;
					Mode = src.Mode;
					Position = src.Position;
					PagingMode = src.PagingMode;
					VisibleItemsCount = src.VisibleItemsCount;
					ThumbnailsModeNavigationButtonVisibility = src.ThumbnailsModeNavigationButtonVisibility;
					ThumbnailsNavigationButtonPosition = src.ThumbnailsNavigationButtonPosition;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ImageSliderBehaviorSettings : PropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderBehaviorSettingsImageLoadMode"),
#endif
		DefaultValue(ImageLoadMode.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public ImageLoadMode ImageLoadMode {
			get { return (ImageLoadMode)GetEnumProperty("ImageLoadMode", ImageLoadMode.Auto); }
			set { SetEnumProperty("ImageLoadMode", ImageLoadMode.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderBehaviorSettingsEnablePagingGestures"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnablePagingGestures {
			get { return GetBoolProperty("EnablePagingGestures", true); }
			set { SetBoolProperty("EnablePagingGestures", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderBehaviorSettingsEnablePagingByClick"),
#endif
		DefaultValue(AutoBoolean.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public AutoBoolean EnablePagingByClick {
			get { return (AutoBoolean)GetEnumProperty("EnablePagingByClick", AutoBoolean.Auto); }
			set { SetEnumProperty("EnablePagingByClick", AutoBoolean.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderBehaviorSettingsExtremeItemClickMode"),
#endif
		DefaultValue(ExtremeItemClickMode.SelectAndSlide), NotifyParentProperty(true), AutoFormatDisable]
		public ExtremeItemClickMode ExtremeItemClickMode
		{
			get { return (ExtremeItemClickMode)GetEnumProperty("ExtremeItemClickMode", ExtremeItemClickMode.SelectAndSlide); }
			set { SetEnumProperty("ExtremeItemClickMode", ExtremeItemClickMode.SelectAndSlide, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderBehaviorSettingsAllowMouseWheel"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool AllowMouseWheel {
			get { return GetBoolProperty("AllowMouseWheel", false); }
			set { SetBoolProperty("AllowMouseWheel", false, value); }
		}
		public ImageSliderBehaviorSettings(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageSliderBehaviorSettings src = source as ImageSliderBehaviorSettings;
				if(src != null) {
					ImageLoadMode = src.ImageLoadMode;
					EnablePagingGestures = src.EnablePagingGestures;
					EnablePagingByClick = src.EnablePagingByClick;
					ExtremeItemClickMode = src.ExtremeItemClickMode;
					AllowMouseWheel = src.AllowMouseWheel;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ImageSliderSlideShowSettings : PropertiesBase {
		internal const int DefaultInterval = 5000;
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderSlideShowSettingsAutoPlay"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool AutoPlay {
			get { return GetBoolProperty("AutoPlay", false); }
			set { SetBoolProperty("AutoPlay", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderSlideShowSettingsInterval"),
#endif
		DefaultValue(DefaultInterval), NotifyParentProperty(true), AutoFormatDisable]
		public int Interval {
			get { return GetIntProperty("Interval", DefaultInterval); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "Interval");
				SetIntProperty("Interval", DefaultInterval, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderSlideShowSettingsPlayPauseButtonVisibility"),
#endif
		DefaultValue(ElementVisibilityMode.None), NotifyParentProperty(true), AutoFormatDisable]
		public ElementVisibilityMode PlayPauseButtonVisibility {
			get { return (ElementVisibilityMode)GetEnumProperty("PlayPauseButtonVisibility", ElementVisibilityMode.None); }
			set { SetEnumProperty("PlayPauseButtonVisibility", ElementVisibilityMode.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderSlideShowSettingsStopPlayingWhenPaging"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool StopPlayingWhenPaging {
			get { return GetBoolProperty("StopPlayingWhenPaging", false); }
			set { SetBoolProperty("StopPlayingWhenPaging", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderSlideShowSettingsPausePlayingWhenMouseOver"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool PausePlayingWhenMouseOver {
			get { return GetBoolProperty("PausePlayingWhenMouseOver", false); }
			set { SetBoolProperty("PausePlayingWhenMouseOver", false, value); }
		}
		public ImageSliderSlideShowSettings(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageSliderSlideShowSettings src = source as ImageSliderSlideShowSettings;
				if(src != null) {
					AutoPlay = src.AutoPlay;
					Interval = src.Interval;
					StopPlayingWhenPaging = src.StopPlayingWhenPaging;
					PlayPauseButtonVisibility = src.PlayPauseButtonVisibility;
					PausePlayingWhenMouseOver = src.PausePlayingWhenMouseOver;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public abstract class AutoGeneratedImagesSettingsBase : PropertiesBase {
		protected string DefaultImageWidth = "660",
					 DefaultImageHeight = "440",
					 DefaultThumbnailWidth = "90",
					 DefaultThumbnailHeight = "60";
		public AutoGeneratedImagesSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue("")]
		public string ImageCacheFolder {
			get { return GetStringProperty("ImageCacheFolder", string.Empty); }
			set { SetStringProperty("ImageCacheFolder", string.Empty, value); }
		}
		[
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), "660")]
		public virtual Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", new Unit(DefaultImageWidth)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FolderSettingsBase.ImageWidth");
				SetUnitProperty("ImageWidth", new Unit(DefaultImageWidth), value);
				LayoutChanged();
			}
		}
		[
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), "440")]
		public virtual Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", new Unit(DefaultImageHeight)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FolderSettingsBase.ImageHeight");
				SetUnitProperty("ImageHeight", new Unit(DefaultImageHeight), value);
				LayoutChanged();
			}
		}
		[
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), "90")]
		public virtual Unit ThumbnailWidth {
			get { return GetUnitProperty("ThumbnailWidth", new Unit(DefaultThumbnailWidth)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FolderSettingsBase.ThumbnailWidth");
				SetUnitProperty("ThumbnailWidth", new Unit(DefaultThumbnailWidth), value);
				LayoutChanged();
			}
		}
		[
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(Unit), "60")]
		public virtual Unit ThumbnailHeight {
			get { return GetUnitProperty("ThumbnailHeight", new Unit(DefaultThumbnailHeight)); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FolderSettingsBase.ThumbnailHeight");
				SetUnitProperty("ThumbnailHeight", new Unit(DefaultThumbnailHeight), value);
				LayoutChanged();
			}
		}
		protected void LayoutChanged() {
			if(Owner != null)
				(Owner as ASPxWebControl).LayoutChanged();
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				AutoGeneratedImagesSettingsBase src = source as AutoGeneratedImagesSettingsBase;
				if(src != null) {
					ImageCacheFolder = src.ImageCacheFolder;
					ImageWidth = src.ImageWidth;
					ImageHeight = src.ImageHeight;
					ThumbnailHeight = src.ThumbnailHeight;
					ThumbnailWidth = src.ThumbnailWidth;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ImageSliderAutoGeneratedImagesSettings : AutoGeneratedImagesSettingsBase {
		public ImageSliderAutoGeneratedImagesSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderAutoGeneratedImagesSettingsEnableImageAutoGeneration"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableImageAutoGeneration {
			get { return GetBoolProperty("EnableImageAutoGeneration", false); }
			set { SetBoolProperty("EnableImageAutoGeneration", false, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageSliderAutoGeneratedImagesSettings src = source as ImageSliderAutoGeneratedImagesSettings;
				if(src != null) {
					EnableImageAutoGeneration = src.EnableImageAutoGeneration;
				}
			} finally {
				EndUpdate();
			}
		}
	}
}
