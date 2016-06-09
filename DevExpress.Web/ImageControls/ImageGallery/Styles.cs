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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ImageGalleryButtonStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryButtonStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatEnable]
		public new Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryButtonStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle
		{
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryButtonStylePressedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle
		{
			get { return base.PressedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class ImageGalleryStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class ImageGalleryStyles : DataViewStyles {
		private const string ThumbnailBorderClassName = "thumbnailBorder",
							 ThumbnailWrapperClassName = "thumbnailWrapper";
		public const string ThumbnailTextAreaStyleName = "thumbnailTextArea";
		public ImageGalleryStyles(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesLoadingPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel {
			get { return LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesThumbnailTextArea"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle ThumbnailTextArea {
			get { return (AppearanceStyle)GetStyle(ThumbnailTextAreaStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataViewEmptyItemStyle EmptyItem {
			get { return base.EmptyItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesContent"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewContentStyle Content {
			get { return base.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewItemStyle Item {
			get { return base.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPager"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerStyle Pager {
			get { return base.Pager; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerButtonStyle PagerButton {
			get { return base.PagerButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerCurrentPageNumber"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerTextStyle PagerCurrentPageNumber {
			get { return base.PagerCurrentPageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerDisabledButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerButtonStyle PagerDisabledButton {
			get { return base.PagerDisabledButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerPageNumber"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerTextStyle PagerPageNumber {
			get { return base.PagerPageNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerSummary"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerTextStyle PagerSummary {
			get { return base.PagerSummary; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerPageSizeItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerPageSizeItemStyle PagerPageSizeItem {
			get { return base.PagerPageSizeItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewContentStyle PagerPanel {
			get { return base.PagerPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesPagerShowMoreItemsContainer"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewPagerShowMoreItemsContainerStyle PagerShowMoreItemsContainer {
			get { return base.PagerShowMoreItemsContainer; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryStylesEmptyData"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DataViewEmptyDataStyle EmptyData {
			get { return base.EmptyData; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxig";
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = string.Format("{0}-{1}", GetCssClassNamePrefix(), cssClassName);
			return style;
		}
		protected internal CreateStyleHandler GetStyleHandler<T>() where T : AppearanceStyleBase, new() {
			return delegate() { return new T(); };
		}
		internal AppearanceStyle GetDefaultThumbnailBorderStyle() {
			return GetDefaultStyle<AppearanceStyle>(ThumbnailBorderClassName);
		}
		internal AppearanceStyle GetDefaultThumbnailWrapperStyle() {
			return GetDefaultStyle<AppearanceStyle>(ThumbnailWrapperClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultThumbnailTextAreaStyle() {
			return GetDefaultStyle<AppearanceStyle>(ThumbnailTextAreaStyleName);
		}
		protected internal override DataViewItemStyle GetDefaultItemStyle() {
			DataViewItemStyle style = base.GetDefaultItemStyle();
			style.Height = GetItemHeight();
			return style;
		}
		protected internal override AppearanceStyleBase GetDefaultFlowItemStyle() {
			AppearanceStyleBase style = base.GetDefaultFlowItemStyle();
			style.Height = GetItemHeight();
			return style;
		}
		protected override Unit GetPagerPanelSpacing() {
			return ImageGalleryContstants.DefaultPagerPanelSpacing;
		}
		protected override Unit GetItemSpacing() {
			return ImageGalleryContstants.DefaultItemSpacing;
		}
		protected override Unit GetItemWidth() {
			return new Unit(ImageGalleryContstants.DefaultThumbnailWidth);
		}
		protected virtual Unit GetItemHeight() {
			return new Unit(ImageGalleryContstants.DefaultThumbnailHeight);
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ThumbnailTextAreaStyleName, GetStyleHandler<AppearanceStyle>()));
		}
	}
	public class ImageGalleryFullscreenViewerNavigationBarStyles : ImageSliderStyles {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerNavigationBarStylesNextPageButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageSliderNavigationButtonStyle NextPageButton {
			get { return base.PrevPageButtonHorizontal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerNavigationBarStylesPrevPageButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageSliderNavigationButtonStyle PrevPageButton {
			get { return base.PrevPageButtonVertical; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerNavigationBarStylesNavigationBar"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageSliderNavigationBarStyle NavigationBar {
			get { return base.NavigationBarThumbnailsModeBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonHorizontal {
			get { return base.PrevPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonHorizontal {
			get { return base.NextPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeBottom {
			get { return base.NavigationBarThumbnailsModeBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderImageAreaStyle ImageArea {
			get { return (ImageSliderImageAreaStyle)GetStyle(ImageAreaStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderStyle PlayPauseButton {
			get { return (ImageSliderStyle)GetStyle(PlayPauseButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevButtonHorizontal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevButtonHorizontalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextButtonHorizontal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextButtonHorizontalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevButtonVertical {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevButtonVerticalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextButtonVertical {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextButtonVerticalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonVertical {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevPageButtonVerticalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonVertical {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextPageButtonVerticalStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeTop {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeTopStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeLeft {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeLeftStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeRight {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeRightStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeTop {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeTopStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeBottom {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeBottomStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeLeft {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeLeftStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeRight {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeRightStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderStyle Item {
			get { return (ImageSliderStyle)GetStyle(ItemStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarDotStyle Dot {
			get { return (ImageSliderNavigationBarDotStyle)GetStyle(DotStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderStyle ItemTextArea {
			get { return (ImageSliderStyle)GetStyle(ItemTextAreaStyleName); }
		}
		public ImageGalleryFullscreenViewerNavigationBarStyles(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageGalleryFullscreenViewerStyles : ImageSliderStyles {
		private const string BottomPanelClassName = "bottomPanel",
							 ImageSliderWrapperClassName = "imageSliderWrapper",
							 PlayPauseButtonWrapperClassName = "playPauseButtonWrapper",
							 OverflowPanelClassName = "overflowPanel";
		public new const string PlayPauseButtonStyleName = "playPauseButton";
		public const string TextAreaStyleName = "fullscreenViewerTextArea",
							CloseButtonStyleName = "closeButton",
							CloseButtonHoverStyleName = "closeButtonHover",
							CloseButtonPressedStyleName = "closeButtonPressed",
							CloseButtonDisabledStyleName = "closeButtonDisabled",
							CloseButtonWrapperStyleName = "closeButtonWrapper",
							PrevButtonStyleName = "prevButton",
							PrevButtonHoverStyleName = "prevButtonHover",
							PrevButtonPressedStyleName = "prevButtonPressed",
							PrevButtonDisabledStyleName = "prevButtonDisabled",
							NextButtonStyleName = "nextButton",
							NextButtonHoverStyleName = "nextButtonHover",
							NextButtonPressedStyleName = "nextButtonPressed",
							NextButtonDisabledStyleName = "nextButtonDisabled",
							NavigationBarMarkerStyleName = "navigationBarMarker";
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesNavigationBarMarker"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryStyle NavigationBarMarker {
			get { return (ImageGalleryStyle)GetStyle(NavigationBarMarkerStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesPlayPauseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageGalleryStyle PlayPauseButton {
			get { return (ImageGalleryStyle)GetStyle(PlayPauseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesTextArea"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle TextArea {
			get { return (AppearanceStyle)GetStyle(TextAreaStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryButtonStyle CloseButton {
			get { return (ImageGalleryButtonStyle)GetStyle(CloseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesPrevButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryButtonStyle PrevButton {
			get { return (ImageGalleryButtonStyle)GetStyle(PrevButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerStylesNextButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageGalleryButtonStyle NextButton {
			get { return (ImageGalleryButtonStyle)GetStyle(NextButtonStyleName); }
		}
		internal AppearanceStyle GetDefaultOverflowPanelStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(OverflowPanelClassName);
		}
		internal AppearanceStyle GetDefaultPlayPauseButtonWrapperStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(PlayPauseButtonWrapperClassName);
		}
		internal AppearanceStyle GetDefaultImageSliderWrapperStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(ImageSliderWrapperClassName);
		}
		internal AppearanceStyle GetDefaultBottomPanelStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(BottomPanelClassName);
		}
		protected internal virtual AppearanceStyle GetDefaultTextAreaStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(TextAreaStyleName);
		}
		protected internal new ImageGalleryStyle GetDefaultPlayPauseButtonStyle() {
			return GetDefaultStyleWidthIGPrefix<ImageGalleryStyle>(PlayPauseButtonStyleName);
		}
		protected internal virtual ImageGalleryStyle GetDefaultNavigationBarStyle() {
			return GetDefaultStyleWidthIGPrefix<ImageGalleryStyle>(NavigationBarMarkerStyleName);
		}
		protected internal virtual ImageGalleryButtonStyle GetDefaultCloseButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(GetDefaultStyleWidthIGPrefix<ImageGalleryButtonStyle>(CloseButtonStyleName));
			style.PressedStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(CloseButtonPressedStyleName));
			style.HoverStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(CloseButtonHoverStyleName));
			style.DisabledStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<DisabledStyle>(CloseButtonDisabledStyleName));
			return style;
		}
		protected internal virtual ImageGalleryButtonStyle GetDefaultPrevButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(GetDefaultStyleWidthIGPrefix<ImageGalleryButtonStyle>(PrevButtonStyleName));
			style.PressedStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(PrevButtonPressedStyleName));
			style.HoverStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(PrevButtonHoverStyleName));
			style.DisabledStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<DisabledStyle>(PrevButtonDisabledStyleName));
			return style;
		}
		protected internal virtual ImageGalleryButtonStyle GetDefaultNextButtonStyle() {
			ImageGalleryButtonStyle style = new ImageGalleryButtonStyle();
			style.CopyFrom(GetDefaultStyleWidthIGPrefix<ImageGalleryButtonStyle>(NextButtonStyleName));
			style.PressedStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(NextButtonPressedStyleName));
			style.HoverStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<AppearanceSelectedStyle>(NextButtonHoverStyleName));
			style.DisabledStyle.CopyFrom(GetDefaultStyleWidthIGPrefix<DisabledStyle>(NextButtonDisabledStyleName));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultCloseButtonWrapperStyle() {
			return GetDefaultStyleWidthIGPrefix<AppearanceStyle>(CloseButtonWrapperStyleName);
		}
		protected T GetDefaultStyleWidthIGPrefix<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = string.Format("dxig-{0}", cssClassName);
			return style;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderStyle ItemTextArea {
			get { return base.ItemTextArea; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevButtonHorizontal {
			get { return base.PrevButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextButtonHorizontal {
			get { return base.NextButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevButtonVertical {
			get { return base.PrevButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextButtonVertical {
			get { return base.NextButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonHorizontal {
			get { return base.PrevPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonHorizontal {
			get { return base.NextPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonVertical {
			get { return base.PrevPageButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonVertical {
			get { return base.NextPageButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonHorizontalOutside {
			get { return base.PrevPageButtonHorizontalOutside; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonHorizontalOutside {
			get { return base.NextPageButtonHorizontalOutside; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle PrevPageButtonVerticalOutside {
			get { return base.PrevPageButtonVerticalOutside; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationButtonStyle NextPageButtonVerticalOutside {
			get { return base.NextPageButtonVerticalOutside; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeTop {
			get { return base.NavigationBarThumbnailsModeTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeBottom {
			get { return base.NavigationBarThumbnailsModeBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeLeft {
			get { return base.NavigationBarThumbnailsModeLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarThumbnailsModeRight {
			get { return base.NavigationBarThumbnailsModeRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeTop {
			get { return base.NavigationBarDotsModeTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeBottom {
			get { return base.NavigationBarDotsModeBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeLeft {
			get { return base.NavigationBarDotsModeLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarStyle NavigationBarDotsModeRight {
			get { return base.NavigationBarDotsModeRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarThumbnailStyle Thumbnail {
			get { return base.Thumbnail; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSliderNavigationBarDotStyle Dot {
			get { return base.Dot; }
		}
		public ImageGalleryFullscreenViewerStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(NavigationBarMarkerStyleName, GetStyleHandler<ImageGalleryStyle>()));
			list.Add(new StyleInfo(PlayPauseButtonStyleName, GetStyleHandler<ImageGalleryStyle>()));
			list.Add(new StyleInfo(TextAreaStyleName, GetStyleHandler<AppearanceStyle>()));
			list.Add(new StyleInfo(CloseButtonStyleName, GetStyleHandler<ImageGalleryButtonStyle>()));
			list.Add(new StyleInfo(PrevButtonStyleName, GetStyleHandler<ImageGalleryButtonStyle>()));
			list.Add(new StyleInfo(NextButtonStyleName, GetStyleHandler<ImageGalleryButtonStyle>()));
		}
	}
	public class ImageGalleryImages : ImageControlsImagesBase {
		protected internal const string LoadingImageName = "igLoading",
										NavigationButtonsBack = "igNavBtnsBack";
		public ImageGalleryImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
	}
	public class ImageGalleryFullscreenViewerNavigationBarImages : ImageSliderImages {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerNavigationBarImagesPrevPageButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButton {
			get { return base.PrevPageButtonHorizontal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerNavigationBarImagesNextPageButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButton {
			get { return base.NextPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties PlayButton {
			get { return base.PlayButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties PauseButton {
			get { return base.PauseButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ItemImageProperties Dot {
			get { return base.Dot; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties PrevButtonVertical {
			get { return base.PrevButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties NextButtonVertical {
			get { return base.NextButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties NextButtonHorizontal {
			get { return base.NextButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties PrevButtonHorizontal {
			get { return base.PrevButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties PrevPageButtonVertical {
			get { return base.PrevPageButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties NextPageButtonVertical {
			get { return base.NextPageButtonVertical; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties PrevPageButtonHorizontal {
			get { return base.PrevPageButtonHorizontal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties NextPageButtonHorizontal {
			get { return base.NextPageButtonHorizontal; }
		}
		public ImageGalleryFullscreenViewerNavigationBarImages(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageGalleryFullscreenViewerImages : ImagesBase {
		public const string CloseButtonImageName = "igCloseButton",
							PrevButtonImageName = "igPrevButton",
							NextButtonImageName = "igNextButton",
							PlayButtonImageName = "igPlayButton",
							PauseButtonImageName = "igPauseButton",
							NavigationBarMarkerImageName = "igNavigationBarMarker";
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesNavigationBarMarker"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties NavigationBarMarker {
			get { return (ImageProperties)GetImageBase(NavigationBarMarkerImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesPlayButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties PlayButton {
			get { return (ImageProperties)GetImageBase(PlayButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesPauseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties PauseButton {
			get { return (ImageProperties)GetImageBase(PauseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties CloseButton {
			get { return (ButtonImageProperties)GetImageBase(CloseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesPrevButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevButton {
			get { return (ButtonImageProperties)GetImageBase(PrevButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryFullscreenViewerImagesNextButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextButton {
			get { return (ButtonImageProperties)GetImageBase(NextButtonImageName); }
		}
		public ImageGalleryFullscreenViewerImages(ISkinOwner owner)
			: base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(PlayButtonImageName, string.Empty, typeof(ImageProperties), PlayButtonImageName));
			list.Add(new ImageInfo(PauseButtonImageName, string.Empty, typeof(ImageProperties), PauseButtonImageName));
			list.Add(new ImageInfo(NavigationBarMarkerImageName, string.Empty, typeof(ImageProperties), NavigationBarMarkerImageName));
			list.Add(new ImageInfo(CloseButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), CloseButtonImageName));
			list.Add(new ImageInfo(PrevButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), PrevButtonImageName));
			list.Add(new ImageInfo(NextButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), NextButtonImageName));
		}
	}
}
