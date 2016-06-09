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
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class PopupWindowStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class PopupWindowContentStyle : PopupWindowStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
	}
	public class PopupWindowFooterStyle : PopupWindowStyle {
		Paddings fSizeGripPaddings;
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowFooterStyleSizeGripPaddings"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings SizeGripPaddings {
			get {
				return CreateObject(ref fSizeGripPaddings);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowFooterStyleSizeGripSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public virtual Unit SizeGripSpacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("PopupWindowFooterStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get { return base.IsEmpty && (fSizeGripPaddings == null || SizeGripPaddings.IsEmpty); }
		}
		public override void CopyFrom(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				PopupWindowFooterStyle popupWindowFooterStyle = style as PopupWindowFooterStyle;
				if(popupWindowFooterStyle != null && popupWindowFooterStyle.fSizeGripPaddings != null) {
					SizeGripPaddings.CopyFrom(popupWindowFooterStyle.SizeGripPaddings);
				}
			}
		}
		public override void MergeWith(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				PopupWindowFooterStyle popupWindowFooterStyle = style as PopupWindowFooterStyle;
				if(popupWindowFooterStyle != null && popupWindowFooterStyle.fSizeGripPaddings != null) {
					SizeGripPaddings.MergeWith(popupWindowFooterStyle.SizeGripPaddings);
				}
			}
		}
		public override void Reset() {
			base.Reset();
			if(fSizeGripPaddings != null)
				SizeGripPaddings.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((PopupWindowFooterStyle)style).GetObject(ref ((PopupWindowFooterStyle)style).fSizeGripPaddings, create); ; });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class PopupWindowButtonStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowButtonStyleCheckedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual AppearanceSelectedStyle CheckedStyle
		{
			get { return base.SelectedStyle; }
		}
	}
	public class PopupControlModalBackgroundStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlModalBackgroundStyleOpacity"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(DefaultOpacity), AutoFormatEnable]
		public new virtual int Opacity {
			get { return base.Opacity; }
			set { base.Opacity = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border {
			get { return base.Border; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return Color.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class PopupControlStyles : StylesBase {
		public const string StyleStyleName = "Style",
							CloseButtonStyleName = "CloseButton",
							PinButtonStyleName = "PinButton",
							RefreshButtonStyleName = "RefreshButton",
							CollapseButtonStyleName = "CollapseButton",
							MaximizeButtonStyleName = "MaximizeButton",
							ContentStyleName = "Content",
							HeaderStyleName = "Header",
							FooterStyleName = "Footer",
							ModalBackgroundStyleName = "ModalBackground",
							RtlMarkerCssClassName = "dxRtl",
							HeaderButtonCellSystemClassName = "dxpcHBCellSys";
		internal const string MainDivCssClassName = "dxpc-mainDiv",
							AnimationWrapperOldIEClassName = "dxpc-animationWrapper",
							ExpandableDivCssClassName = "dxpc-expandableDiv",
							HeaderWithCloseButtonCssMarker = "dxpc-withBtn",
							HeaderTextCssClassName = "dxpc-headerText",
							HeaderImageCssClassName = "dxpc-headerImg",
							ContentWrapperCssClassName = "dxpc-contentWrapper",
							ContentCssClassName = "dxpc-content",
							FooterContentContainerCssClassName = "dxpc-footerContent",
							HeaderContentContainerCssClassName = "dxpc-headerContent",
							FooterTextCssClassName = "dxpc-footerText",
							FooterImageCssClassName = "dxpc-footerImg",
							SizeGripCssClassName = "dxpc-sizeGrip",
							InternalHyperLinkCssClassName = "dxpc-link",
							ShadowCssClassName = "dxpc-shadow";
		public PopupControlStyles(ISkinOwner popupControl)
			: base(popupControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesCloseButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowButtonStyle CloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CloseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesPinButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowButtonStyle PinButton {
			get { return (PopupWindowButtonStyle)GetStyle(PinButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesRefreshButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowButtonStyle RefreshButton {
			get { return (PopupWindowButtonStyle)GetStyle(RefreshButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesCollapseButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowButtonStyle CollapseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CollapseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesMaximizeButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowButtonStyle MaximizeButton {
			get { return (PopupWindowButtonStyle)GetStyle(MaximizeButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesContent"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowContentStyle Content {
			get { return (PopupWindowContentStyle)GetStyle(ContentStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Style property instead."),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle ControlStyle {
			get { return Style; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual AppearanceStyle Style {
			get { return (AppearanceStyle)GetStyle(StyleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesFooter"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowFooterStyle Footer {
			get { return (PopupWindowFooterStyle)GetStyle(FooterStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesHeader"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public PopupWindowStyle Header {
			get { return (PopupWindowStyle)GetStyle(HeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesModalBackground"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public virtual PopupControlModalBackgroundStyle ModalBackground {
			get { return (PopupControlModalBackgroundStyle)GetStyle(ModalBackgroundStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesLink"),
#endif
NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinkStyle Link
		{
			get { return base.LinkInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesLoadingDiv"),
#endif
NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv
		{
			get { return base.LoadingDivInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesLoadingPanel"),
#endif
NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel
		{
			get { return base.LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlStylesDisabled"),
#endif
NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled
		{
			get { return base.DisabledInternal; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxpc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StyleStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(CloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(PinButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(RefreshButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(CollapseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(MaximizeButtonStyleName, delegate() { return new PopupWindowButtonStyle(); }));
			list.Add(new StyleInfo(ContentStyleName, delegate() { return new PopupWindowContentStyle(); }));
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new PopupWindowStyle(); }));
			list.Add(new StyleInfo(FooterStyleName, delegate() { return new PopupWindowFooterStyle(); }));
			list.Add(new StyleInfo(ModalBackgroundStyleName, delegate() { return new PopupControlModalBackgroundStyle(); } ));
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			return GetDefaultStyleWithThemePostfix<AppearanceStyle>("Lite");
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			return GetDefaultStyleWithThemePostfix<AppearanceStyleBase>("LiteDisabled"); ;
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultHeaderButtonCellStyle() {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(CreateStyleByName("HBCellStyle"));
			return style;
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultCloseButtonStyle() {
			return GetDefaultStyle<PopupWindowButtonStyle>("dxpc-closeBtn");
		}
		protected internal virtual AppearanceStyleBase GetDefaultCloseButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-closeBtnHover");
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultPinButtonStyle() {
			return GetDefaultStyle<PopupWindowButtonStyle>("dxpc-pinBtn");
		}
		protected internal virtual AppearanceStyleBase GetDefaultPinButtonCheckedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-pinBtnChecked");
		}
		protected internal virtual AppearanceStyleBase GetDefaultPinButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-pinBtnHover");
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultRefreshButtonStyle() {
			return GetDefaultStyle<PopupWindowButtonStyle>("dxpc-refreshBtn");
		}
		protected internal virtual AppearanceStyleBase GetDefaultRefreshButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-refreshBtnHover");
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultCollapseButtonStyle() {
			return GetDefaultStyle<PopupWindowButtonStyle>("dxpc-collapseBtn");
		}
		protected internal virtual AppearanceStyleBase GetDefaultCollapseButtonCheckedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-collapseBtnChecked");
		}
		protected internal virtual AppearanceStyleBase GetDefaultCollapseButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-collapseBtnHover");
		}
		protected internal virtual PopupWindowButtonStyle GetDefaultMaximizeButtonStyle() {
			return GetDefaultStyle<PopupWindowButtonStyle>("dxpc-maximizeBtn");
		}
		protected internal virtual AppearanceStyleBase GetDefaultMaximizeButtonCheckedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-maximizeBtnChecked");
		}
		protected internal virtual AppearanceStyleBase GetDefaultMaximizeButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("dxpc-maximizeBtnHover");
		}
		protected internal virtual PopupWindowContentStyle GetDefaultContentStyle() {
			return new PopupWindowContentStyle();
		}
		protected internal virtual string GetDefaultContentPaddingsCssClass() {
			return GetCssClassName(GetCssClassNamePrefix(), "ContentPaddings");
		}
		protected internal virtual PopupWindowFooterStyle GetDefaultFooterStyle() {
			return GetDefaultStyle<PopupWindowFooterStyle>("dxpc-footer");
		}
		protected internal virtual PopupWindowStyle GetDefaultHeaderStyle() {
			return GetDefaultStyle<PopupWindowStyle>("dxpc-header");
		}
		protected internal virtual PopupControlModalBackgroundStyle GetDefaultModalBackgroundStyle() {
			return GetDefaultStyleWithThemePostfix<PopupControlModalBackgroundStyle>("ModalBackLite");
		}
		protected virtual Paddings GetHeaderPaddings(bool hasHeaderButton) {
			if(SkinOwner.IsRightToLeft())
				return new Paddings(hasHeaderButton ? 2 : 12, 2, 12, 2);
			return new Paddings(12, 2, hasHeaderButton ? 2 : 12, 2);
		}
		protected virtual Paddings GetSizeGripPaddings() {
			return new Paddings(0);
		}
		protected virtual Unit GetFooterImageSpacing() {
			return GetImageSpacing();
		}
		protected virtual Unit GetHeaderImageSpacing() {
			return GetImageSpacing();
		}
		protected virtual Unit GetSizeGripSpacing() {
			return GetImageSpacing();
		}
		protected T GetDefaultStyle<T>(string cssClassName) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = cssClassName;
			return style;
		}
		protected T GetDefaultStyleWithThemePostfix<T>(string cssClassName) where T : AppearanceStyleBase, new() {
			return GetDefaultStyle<T>(GetCssClassName(GetCssClassNamePrefix(), cssClassName));
		}
	}
}
