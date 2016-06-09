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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using System.Web.UI;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.Web {
	public class ImageSliderStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
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
	public class ImageSliderPassePartoutStyle : ImageSliderStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border {
			get { return base.Border; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
	}
	public class ImageSliderImageAreaStyle : ImageSliderStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height
		{
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImageAreaStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class ImageSliderNavigationBarThumbnailStyle : ImageSliderStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarThumbnailStyleSelectedStyle"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle SelectedStyle
		{
			get { return base.SelectedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarThumbnailStyleImageWidth"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit ImageWidth {
			get { return Width; }
			set { Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarThumbnailStyleImageHeight"),
#endif
		Browsable(true), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit ImageHeight {
			get { return Height; }
			set { Height = value; }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			ImageSliderNavigationBarThumbnailStyle itemNbStyle = style as ImageSliderNavigationBarThumbnailStyle;
			if(itemNbStyle != null) {
				ImageWidth = itemNbStyle.ImageWidth;
				ImageHeight = itemNbStyle.ImageHeight;
			}
		}
	}
	public class ImageSliderNavigationBarDotStyle : ImageSliderNavigationBarThumbnailStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarDotStyleHoverStyle"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceSelectedStyle HoverStyle
		{
			get { return base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarDotStylePressedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		new public AppearanceSelectedStyle PressedStyle
		{
			get { return base.PressedStyle; }
		}
	}
	public class ImageSliderNavigationBarStyle : ImageSliderStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarStyleMargins"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new Margins Margins {
			get { return base.Margins; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationBarStylePaddings"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public override Paddings Paddings
		{
			get { return base.Paddings; }
		}
	}
	public class ImageSliderNavigationButtonStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderNavigationButtonStyleWidth"),
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
	DevExpressWebLocalizedDescription("ImageSliderNavigationButtonStyleDisabledStyle"),
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
	DevExpressWebLocalizedDescription("ImageSliderNavigationButtonStylePressedStyle"),
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
	public class ImageSliderStylesBase : StylesBase {
		public const string ItemStyleName = "item",
							ItemTextAreaStyleName = "itemTextArea",
							ImageAreaStyleName = "imageArea",
							PassePartoutStyleName = "passePartout",
							PlayPauseButtonStyleName = "playPauseBtn",
							PrevButtonHorizontalStyleName = "prevBtnHor",
							PrevButtonHorizontalPressedStyleName = "prevBtnHorPressed",
							PrevButtonHorizontalHoverStyleName = "prevBtnHorHover",
							PrevButtonHorizontalDisabledStyleName = "prevBtnHorDisabled",
							NextButtonHorizontalStyleName = "nextBtnHor",
							NextButtonHorizontalPressedStyleName = "nextBtnHorPressed",
							NextButtonHorizontalHoverStyleName = "nextBtnHorHover",
							NextButtonHorizontalDisabledStyleName = "nextBtnHorDisabled",
							PrevButtonVerticalStyleName = "prevBtnVert",
							PrevButtonVerticalPressedStyleName = "prevBtnVertPressed",
							PrevButtonVerticalHoverStyleName = "prevBtnVertHover",
							PrevButtonVerticalDisabledStyleName = "prevBtnVertDisabled",
							NextButtonVerticalStyleName = "nextBtnVert",
							NextButtonVerticalPressedStyleName = "nextBtnVertPressed",
							NextButtonVerticalHoverStyleName = "nextBtnVertHover",
							NextButtonVerticalDisabledStyleName = "nextBtnVertDisabled",
							PrevPageButtonHorizontalStyleName = "prevPageBtnHor",
							PrevPageButtonHorizontalPressedStyleName = "prevPageBtnHorPressed",
							PrevPageButtonHorizontalHoverStyleName = "prevPageBtnHorHover",
							PrevPageButtonHorizontalDisabledStyleName = "prevPageBtnHorDisabled",
							NextPageButtonHorizontalStyleName = "nextPageBtnHor",
							NextPageButtonHorizontalPressedStyleName = "nextPageBtnHorPressed",
							NextPageButtonHorizontalHoverStyleName = "nextPageBtnHorHover",
							NextPageButtonHorizontalDisabledStyleName = "nextPageBtnHorDisabled",
							PrevPageButtonVerticalStyleName = "prevPageBtnVert",
							PrevPageButtonVerticalPressedStyleName = "prevPageBtnVertPressed",
							PrevPageButtonVerticalHoverStyleName = "prevPageBtnVertHover",
							PrevPageButtonVerticalDisabledStyleName = "prevPageBtnVertDisabled",
							NextPageButtonVerticalStyleName = "nextPageBtnVert",
							NextPageButtonVerticalPressedStyleName = "nextPageBtnVertPressed",
							NextPageButtonVerticalHoverStyleName = "nextPageBtnVertHover",
							NextPageButtonVerticalDisabledStyleName = "nextPageBtnVertDisabled",
							PrevPageButtonHorizontalOutsideStyleName = "prevPageBtnHorOutside",
							PrevPageButtonHorizontalOutsidePressedStyleName = "prevPageBtnHorOutsidePressed",
							PrevPageButtonHorizontalOutsideHoverStyleName = "prevPageBtnHorOutsideHover",
							PrevPageButtonHorizontalOutsideDisabledStyleName = "prevPageBtnHorOutsideDisabled",
							NextPageButtonHorizontalOutsideStyleName = "nextPageBtnHorOutside",
							NextPageButtonHorizontalOutsidePressedStyleName = "nextPageBtnHorOutsidePressed",
							NextPageButtonHorizontalOutsideHoverStyleName = "nextPageBtnHorOutsideHover",
							NextPageButtonHorizontalOutsideDisabledStyleName = "nextPageBtnHorOutsideDisabled",
							PrevPageButtonVerticalOutsideStyleName = "prevPageBtnVertOutside",
							PrevPageButtonVerticalOutsidePressedStyleName = "prevPageBtnVertOutsidePressed",
							PrevPageButtonVerticalOutsideHoverStyleName = "prevPageBtnVertOutsideHover",
							PrevPageButtonVerticalOutsideDisabledStyleName = "prevPageBtnVertOutsideDisabled",
							NextPageButtonVerticalOutsideStyleName = "nextPageBtnVertOutside",
							NextPageButtonVerticalOutsidePressedStyleName = "nextPageBtnVertOutsidePressed",
							NextPageButtonVerticalOutsideHoverStyleName = "nextPageBtnVertOutsideHover",
							NextPageButtonVerticalOutsideDisabledStyleName = "nextPageBtnVertOutsideDisabled",
							NavigationBarThumbnailsModeTopStyleName = "nbTop",
							NavigationBarThumbnailsModeBottomStyleName = "nbBottom",
							NavigationBarThumbnailsModeLeftStyleName = "nbLeft",
							NavigationBarThumbnailsModeRightStyleName = "nbRight",
							NavigationBarDotsModeTopStyleName = "nbDotsTop",
							NavigationBarDotsModeBottomStyleName = "nbDotsBottom",
							NavigationBarDotsModeLeftStyleName = "nbDotsLeft",
							NavigationBarDotsModeRightStyleName = "nbDotsRight",
							ThumbnailStyleName = "nbItem",
							ThumbnailSelectedStyleName = "nbSelectedItem",
							DotStyleName = "nbDotItem",
							DotPressedStyleName = "nbDotItemPressed",
							DotHoverStyleName = "nbDotItemHover",
							DotSelectedStyleName = "nbDotItemSelected",
							DotDisabledStyleName = "nbDotItemDisabled",
							RightToLeftClassName = "dxisRtl";
		private const string PressedSuffix = "Pressed",
							 HoverSuffix = "Hover",
							 SelectedSuffix = "Selected",
							 DisabledSuffix = "Disabled";
		protected string GetPressedStyleName(string styleName) {
			return styleName + PressedSuffix;
		}
		protected string GetHoverStyleName(string styleName) {
			return styleName + HoverSuffix;
		}
		protected string GetSelectedStyleName(string styleName) {
			return styleName + SelectedSuffix;
		}
		protected string GetDisabledStyleName(string styleName) {
			return styleName + DisabledSuffix;
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = string.Format("{0}-{1}", GetCssClassNamePrefix(), cssClassName);
			return style;
		}
		protected T GetDefaultStyleWithAddedStateStyles<T>(string styleName, bool addSelected = false,
			bool onlySelected = false, string selectedStyleName = null) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CopyFrom(GetDefaultStyle<T>(styleName));
			if(!onlySelected) {
				style.HoverStyle.CopyFrom(GetDefaultStyle<AppearanceSelectedStyle>(GetHoverStyleName(styleName)));
				style.PressedStyle.CopyFrom(GetDefaultStyle<AppearanceSelectedStyle>(GetPressedStyleName(styleName)));
				style.DisabledStyle.CopyFrom(GetDefaultStyle<DisabledStyle>(GetDisabledStyleName(styleName)));
			}
			if(addSelected || onlySelected)
				style.SelectedStyle.CopyFrom(
					GetDefaultStyle<AppearanceSelectedStyle>(selectedStyleName ?? GetSelectedStyleName(styleName)));
			return style;
		}
		protected internal CreateStyleHandler GetStyleHandler<T>() where T : AppearanceStyleBase, new() {
			return delegate() { return new T(); };
		}
		#region Properties
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesBaseDisabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled {
			get { return base.DisabledInternal; }
		}
		protected internal ImageSliderPassePartoutStyle PassePartoutInternal {
			get { return (ImageSliderPassePartoutStyle)GetStyle(PassePartoutStyleName); }
		}
		protected internal ImageSliderImageAreaStyle ImageAreaInternal {
			get { return (ImageSliderImageAreaStyle)GetStyle(ImageAreaStyleName); }
		}
		protected internal ImageSliderStyle PlayPauseButtonInternal {
			get { return (ImageSliderStyle)GetStyle(PlayPauseButtonStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevButtonHorizontalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevButtonHorizontalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextButtonHorizontalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextButtonHorizontalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevButtonVerticalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevButtonVerticalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextButtonVerticalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextButtonVerticalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevPageButtonHorizontalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevPageButtonHorizontalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextPageButtonHorizontalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextPageButtonHorizontalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevPageButtonVerticalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevPageButtonVerticalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextPageButtonVerticalInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextPageButtonVerticalStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevPageButtonHorizontalOutsideInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevPageButtonHorizontalOutsideStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextPageButtonHorizontalOutsideInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextPageButtonHorizontalOutsideStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle PrevPageButtonVerticalOutsideInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(PrevPageButtonVerticalOutsideStyleName); }
		}
		protected internal ImageSliderNavigationButtonStyle NextPageButtonVerticalOutsideInternal {
			get { return (ImageSliderNavigationButtonStyle)GetStyle(NextPageButtonVerticalOutsideStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarThumbnailsModeTopInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeTopStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarThumbnailsModeBottomInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeBottomStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarThumbnailsModeLeftInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeLeftStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarThumbnailsModeRightInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarThumbnailsModeRightStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarDotsModeTopInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeTopStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarDotsModeBottomInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeBottomStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarDotsModeLeftInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeLeftStyleName); }
		}
		protected internal ImageSliderNavigationBarStyle NavigationBarDotsModeRightInternal {
			get { return (ImageSliderNavigationBarStyle)GetStyle(NavigationBarDotsModeRightStyleName); }
		}
		protected internal ImageSliderStyle ItemInternal {
			get { return (ImageSliderStyle)GetStyle(ItemStyleName); }
		}
		protected internal ImageSliderNavigationBarThumbnailStyle ThumbnailInternal {
			get { return (ImageSliderNavigationBarThumbnailStyle)GetStyle(ThumbnailStyleName); }
		}
		protected internal ImageSliderNavigationBarDotStyle DotInternal {
			get { return (ImageSliderNavigationBarDotStyle)GetStyle(DotStyleName); }
		}
		protected internal ImageSliderStyle ItemTextAreaInternal {
			get { return (ImageSliderStyle)GetStyle(ItemTextAreaStyleName); }
		}
		#endregion
		public ImageSliderStylesBase(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxis";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ImageAreaStyleName, GetStyleHandler<ImageSliderImageAreaStyle>()));
			list.Add(new StyleInfo(PlayPauseButtonStyleName, GetStyleHandler<ImageSliderStyle>()));
			list.Add(new StyleInfo(PassePartoutStyleName, GetStyleHandler<ImageSliderPassePartoutStyle>()));
			list.Add(new StyleInfo(PrevButtonHorizontalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextButtonHorizontalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(PrevButtonVerticalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextButtonVerticalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(PrevPageButtonHorizontalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextPageButtonHorizontalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(PrevPageButtonVerticalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextPageButtonVerticalStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(PrevPageButtonHorizontalOutsideStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextPageButtonHorizontalOutsideStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(PrevPageButtonVerticalOutsideStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NextPageButtonVerticalOutsideStyleName, GetStyleHandler<ImageSliderNavigationButtonStyle>()));
			list.Add(new StyleInfo(NavigationBarThumbnailsModeTopStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarThumbnailsModeBottomStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarThumbnailsModeLeftStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarThumbnailsModeRightStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarDotsModeTopStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarDotsModeBottomStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarDotsModeLeftStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(NavigationBarDotsModeRightStyleName, GetStyleHandler<ImageSliderNavigationBarStyle>()));
			list.Add(new StyleInfo(ItemStyleName, GetStyleHandler<ImageSliderStyle>()));
			list.Add(new StyleInfo(ItemTextAreaStyleName, GetStyleHandler<ImageSliderStyle>()));
			list.Add(new StyleInfo(ThumbnailStyleName, GetStyleHandler<ImageSliderNavigationBarThumbnailStyle>()));
			list.Add(new StyleInfo(DotStyleName, GetStyleHandler<ImageSliderNavigationBarDotStyle>()));
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarThumbnailsModeTopStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarThumbnailsModeTopStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarThumbnailsModeBottomStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarThumbnailsModeBottomStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarThumbnailsModeLeftStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarThumbnailsModeLeftStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarThumbnailsModeRightStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarThumbnailsModeRightStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarDotsModeTopStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarDotsModeTopStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarDotsModeBottomStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarDotsModeBottomStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarDotsModeLeftStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarDotsModeLeftStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultNavigationBarDotsModeRightStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(NavigationBarDotsModeRightStyleName);
		}
		protected internal virtual ImageSliderPassePartoutStyle GetDefaultPassePartoutStyle() {
			return GetDefaultStyle<ImageSliderPassePartoutStyle>(PassePartoutStyleName);
		}
		protected internal virtual ImageSliderNavigationBarStyle GetDefaultImageAreaStyle() {
			return GetDefaultStyle<ImageSliderNavigationBarStyle>(ImageAreaStyleName);
		}
		protected internal virtual ImageSliderStyle GetDefaultPlayPauseButtonStyle() {
			return GetDefaultStyle<ImageSliderStyle>(PlayPauseButtonStyleName);
		}
		protected internal virtual ImageSliderStyle GetDefaultItemStyle() {
			return GetDefaultStyle<ImageSliderStyle>(ItemStyleName);
		}
		protected internal virtual ImageSliderStyle GetDefaultItemTextAreaStyle() {
			return GetDefaultStyle<ImageSliderStyle>(ItemTextAreaStyleName);
		}
		protected internal virtual ImageSliderNavigationBarThumbnailStyle GetDefaultThumbnailStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationBarThumbnailStyle>(ThumbnailStyleName,
				onlySelected: true, selectedStyleName: ThumbnailSelectedStyleName);
		}
		protected internal virtual ImageSliderNavigationBarDotStyle GetDefaultDotStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationBarDotStyle>(DotStyleName, addSelected: true);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevButtonVerticalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevButtonVerticalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextButtonVerticalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextButtonVerticalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevButtonHorizontalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevButtonHorizontalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextButtonHorizontalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextButtonHorizontalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevPageButtonVerticalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevPageButtonVerticalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextPageButtonVerticalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextPageButtonVerticalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevPageButtonHorizontalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevPageButtonHorizontalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextPageButtonHorizontalStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextPageButtonHorizontalStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevPageButtonVerticalOutsideStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevPageButtonVerticalOutsideStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextPageButtonVerticalOutsideStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextPageButtonVerticalOutsideStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultPrevPageButtonHorizontalOutsideStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(PrevPageButtonHorizontalOutsideStyleName);
		}
		protected internal virtual ImageSliderNavigationButtonStyle GetDefaultNextPageButtonHorizontalOutsideStyle() {
			return GetDefaultStyleWithAddedStateStyles<ImageSliderNavigationButtonStyle>(NextPageButtonHorizontalOutsideStyleName);
		}
	}
	public class ImageSliderStyles : ImageSliderStylesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPassePartout"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderPassePartoutStyle PassePartout {
			get { return PassePartoutInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesImageArea"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderImageAreaStyle ImageArea {
			get { return ImageAreaInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPlayPauseButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderStyle PlayPauseButton {
			get { return PlayPauseButtonInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevButtonHorizontal"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevButtonHorizontal {
			get { return PrevButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextButtonHorizontal"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextButtonHorizontal {
			get { return NextButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevButtonVertical"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevButtonVertical {
			get { return PrevButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextButtonVertical"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextButtonVertical {
			get { return NextButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevPageButtonHorizontal"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonHorizontal {
			get { return PrevPageButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextPageButtonHorizontal"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonHorizontal {
			get { return NextPageButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevPageButtonVertical"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonVertical {
			get { return PrevPageButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextPageButtonVertical"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonVertical {
			get { return NextPageButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevPageButtonHorizontalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonHorizontalOutside {
			get { return PrevPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextPageButtonHorizontalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonHorizontalOutside {
			get { return NextPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesPrevPageButtonVerticalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonVerticalOutside {
			get { return PrevPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNextPageButtonVerticalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonVerticalOutside {
			get { return NextPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarThumbnailsModeTop"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarThumbnailsModeTop {
			get { return NavigationBarThumbnailsModeTopInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarThumbnailsModeBottom"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarThumbnailsModeBottom {
			get { return NavigationBarThumbnailsModeBottomInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarThumbnailsModeLeft"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarThumbnailsModeLeft {
			get { return NavigationBarThumbnailsModeLeftInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarThumbnailsModeRight"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarThumbnailsModeRight {
			get { return NavigationBarThumbnailsModeRightInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarDotsModeTop"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarDotsModeTop {
			get { return NavigationBarDotsModeTopInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarDotsModeBottom"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarDotsModeBottom {
			get { return NavigationBarDotsModeBottomInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarDotsModeLeft"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarDotsModeLeft {
			get { return NavigationBarDotsModeLeftInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesNavigationBarDotsModeRight"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigationBarDotsModeRight {
			get { return NavigationBarDotsModeRightInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesItem"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderStyle Item {
			get { return ItemInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesThumbnail"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarThumbnailStyle Thumbnail {
			get { return ThumbnailInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesDot"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarDotStyle Dot {
			get { return DotInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderStylesItemTextArea"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderStyle ItemTextArea {
			get { return ItemTextAreaInternal; }
		}
		public ImageSliderStyles(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageSliderImagesBase : ImagesBase {
		protected internal const string LoadingImageName = "isLoading";
		public const string DesignTimeSpriteImageName = "isDesignTimeSprite";
		public const string PrevButtonHorizontalImageName = "isPrevBtnHor";
		public const string NextButtonHorizontalImageName = "isNextBtnHor";
		public const string PrevButtonVerticalImageName = "isPrevBtnVert";
		public const string NextButtonVerticalImageName = "isNextBtnVert";
		public const string PrevPageButtonHorizontalImageName = "isPrevPageBtnHor";
		public const string NextPageButtonHorizontalImageName = "isNextPageBtnHor";
		public const string PrevPageButtonVerticalImageName = "isPrevPageBtnVert";
		public const string NextPageButtonVerticalImageName = "isNextPageBtnVert";
		public const string PrevPageButtonHorizontalOutsideImageName = "isPrevPageBtnHorOutside";
		public const string NextPageButtonHorizontalOutsideImageName = "isNextPageBtnHorOutside";
		public const string PrevPageButtonVerticalOutsideImageName = "isPrevPageBtnVertOutside";
		public const string NextPageButtonVerticalOutsideImageName = "isNextPageBtnVertOutside";
		public const string PlayButtonImageName = "isPlayBtn";
		public const string PauseButtonImageName = "isPauseBtn";
		public const string DotImageName = "isDot";
		public ImageSliderImagesBase(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		#region
		protected internal ImageProperties PlayButtonInternal {
			get { return (ImageProperties)GetImageBase(PlayButtonImageName); }
		}
		protected internal ImageProperties PauseButtonInternal {
			get { return (ImageProperties)GetImageBase(PauseButtonImageName); }
		}
		protected internal ItemImageProperties DotInternal {
			get { return (ItemImageProperties)GetImageBase(DotImageName); }
		}
		protected internal ButtonImageProperties PrevButtonVerticalInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevButtonVerticalImageName); }
		}
		protected internal ButtonImageProperties NextButtonHorizontalInternal {
			get { return (ButtonImageProperties)GetImageBase(NextButtonHorizontalImageName); }
		}
		protected internal ButtonImageProperties PrevButtonHorizontalInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevButtonHorizontalImageName); }
		}
		protected internal ButtonImageProperties NextButtonVerticalInternal {
			get { return (ButtonImageProperties)GetImageBase(NextButtonVerticalImageName); }
		}
		protected internal ButtonImageProperties PrevPageButtonVerticalInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevPageButtonVerticalImageName); }
		}
		protected internal ButtonImageProperties NextPageButtonVerticalInternal {
			get { return (ButtonImageProperties)GetImageBase(NextPageButtonVerticalImageName); }
		}
		protected internal ButtonImageProperties PrevPageButtonHorizontalInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevPageButtonHorizontalImageName); }
		}
		protected internal ButtonImageProperties NextPageButtonHorizontalInternal {
			get { return (ButtonImageProperties)GetImageBase(NextPageButtonHorizontalImageName); }
		}
		protected internal ButtonImageProperties PrevPageButtonVerticalOutsideInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevPageButtonVerticalOutsideImageName); }
		}
		protected internal ButtonImageProperties NextPageButtonVerticalOutsideInternal {
			get { return (ButtonImageProperties)GetImageBase(NextPageButtonVerticalOutsideImageName); }
		}
		protected internal ButtonImageProperties PrevPageButtonHorizontalOutsideInternal {
			get { return (ButtonImageProperties)GetImageBase(PrevPageButtonHorizontalOutsideImageName); }
		}
		protected internal ButtonImageProperties NextPageButtonHorizontalOutsideInternal {
			get { return (ButtonImageProperties)GetImageBase(NextPageButtonHorizontalOutsideImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel {
			get { return base.LoadingPanel; }
		}
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(DesignTimeSpriteImageName, ImageFlags.IsPng));
			list.Add(new ImageInfo(PlayButtonImageName, string.Empty, typeof(ImageProperties), PlayButtonImageName));
			list.Add(new ImageInfo(PauseButtonImageName, string.Empty, typeof(ImageProperties), PauseButtonImageName));
			list.Add(new ImageInfo(DotImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState | ImageFlags.HasSelectedState, string.Empty, typeof(ItemImageProperties), DotImageName));
			AddImageInfo(list, PrevButtonVerticalImageName);
			AddImageInfo(list, NextButtonVerticalImageName);
			AddImageInfo(list, PrevButtonHorizontalImageName);
			AddImageInfo(list, NextButtonHorizontalImageName);
			AddImageInfo(list, PrevPageButtonVerticalImageName);
			AddImageInfo(list, NextPageButtonVerticalImageName);
			AddImageInfo(list, PrevPageButtonHorizontalImageName);
			AddImageInfo(list, NextPageButtonHorizontalImageName);
			AddImageInfo(list, PrevPageButtonVerticalOutsideImageName);
			AddImageInfo(list, NextPageButtonVerticalOutsideImageName);
			AddImageInfo(list, PrevPageButtonHorizontalOutsideImageName);
			AddImageInfo(list, NextPageButtonHorizontalOutsideImageName);
		}
		private void AddImageInfo(List<ImageInfo> list, string imageName) {
			list.Add(new ImageInfo(imageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), imageName));
		}
	}
	public class ImageSliderImages : ImageSliderImagesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPlayButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties PlayButton {
			get { return PlayButtonInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPauseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties PauseButton {
			get { return PauseButtonInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesDot"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties Dot {
			get { return DotInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevButtonVertical"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevButtonVertical {
			get { return PrevButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextButtonHorizontal"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextButtonHorizontal {
			get { return NextButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevButtonHorizontal"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevButtonHorizontal {
			get { return PrevButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextButtonVertical"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextButtonVertical {
			get { return NextButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevPageButtonVertical"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonVertical {
			get { return PrevPageButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextPageButtonVertical"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonVertical {
			get { return NextPageButtonVerticalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevPageButtonHorizontal"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonHorizontal {
			get { return PrevPageButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextPageButtonHorizontal"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonHorizontal {
			get { return NextPageButtonHorizontalInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevPageButtonVerticalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonVerticalOutside {
			get { return PrevPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextPageButtonVerticalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonVerticalOutside {
			get { return NextPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesPrevPageButtonHorizontalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonHorizontalOutside {
			get { return PrevPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderImagesNextPageButtonHorizontalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonHorizontalOutside {
			get { return NextPageButtonHorizontalOutsideInternal; }
		}
		public ImageSliderImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
	}
}
