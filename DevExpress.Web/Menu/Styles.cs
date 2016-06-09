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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class MenuStyle: AppearanceStyle {
		private AppearanceStyle gutterStyle;
		private AppearanceStyle separatorStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleGutterBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage GutterBackgroundImage {
			get { return GutterStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleGutterColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		Obsolete("Use the GutterBackgroundImage property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Color GutterColor {
			get { return Color.Empty; }
			set {  }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleGutterCssClass"),
#endif
		Category("Appearance"), DefaultValue(""), AutoFormatEnable,
		NotifyParentProperty(true), Localizable(false)]
		public virtual string GutterCssClass {
			get { return GutterStyle.CssClass; }
			set { GutterStyle.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleGutterImageSpacing"),
#endif
		Category("Appearance"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true),
		Obsolete("Use the ImageSpacing and TextIndent properties instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Unit GutterImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleTextIndent"),
#endif
		Category("Appearance"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit TextIndent {
			get { return GutterStyle.ImageSpacing; }
			set { GutterStyle.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleGutterWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit GutterWidth {
			get { return GutterStyle.Width; }
			set { GutterStyle.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value;  }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleItemSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit ItemSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage SeparatorBackgroundImage {
			get { return SeparatorStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter))]
		public virtual Color SeparatorColor {
			get { return SeparatorStyle.BackColor; }
			set { SeparatorStyle.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorCssClass"),
#endif
		Category("Appearance"), DefaultValue(""), AutoFormatEnable,
		NotifyParentProperty(true), Localizable(false)]
		public virtual string SeparatorCssClass {
			get { return SeparatorStyle.CssClass; }
			set { SeparatorStyle.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit SeparatorHeight {
			get { return SeparatorStyle.Height; }
			set { SeparatorStyle.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorPaddings"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings SeparatorPaddings {
			get { return SeparatorStyle.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStyleSeparatorWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit SeparatorWidth {
			get { return SeparatorStyle.Width; }
			set { SeparatorStyle.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceStyle GutterStyle {
			get { return CreateObject(ref gutterStyle); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceStyle SeparatorStyle {
			get { return CreateObject(ref separatorStyle); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get { return base.IsEmpty && (gutterStyle == null || GutterStyle.IsEmpty) && (separatorStyle == null || SeparatorStyle.IsEmpty); }
		}
		public override void CopyFrom(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				MenuStyle menuStyle = style as MenuStyle;
				if(menuStyle != null) {
					if(menuStyle.gutterStyle != null && !menuStyle.gutterStyle.IsEmpty)
						GutterStyle.CopyFrom(menuStyle.GutterStyle);
					if(menuStyle.separatorStyle != null && !menuStyle.separatorStyle.IsEmpty)
						SeparatorStyle.CopyFrom(menuStyle.SeparatorStyle);
				}
			}
		}
		public override void MergeWith(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				MenuStyle menuStyle = style as MenuStyle;
				if(menuStyle != null) {
					if(menuStyle.gutterStyle != null)
						GutterStyle.MergeWith(menuStyle.GutterStyle);
					if(menuStyle.separatorStyle != null)
						SeparatorStyle.MergeWith(menuStyle.SeparatorStyle);
				}
			}
		}
		public override void Reset() {
			gutterStyle = null;
			separatorStyle = null;
			base.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((MenuStyle)style).GetObject(ref ((MenuStyle)style).gutterStyle, create); });
				list.Add(delegate(object style, bool create) { return ((MenuStyle)style).GetObject(ref ((MenuStyle)style).separatorStyle, create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class MenuItemStyleBase: AppearanceItemStyle {
		private AppearanceSelectedStyle checkedStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleBaseCheckedStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceSelectedStyle CheckedStyle {
			get {
				return CreateCheckedStyle(true);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleBasePopOutImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit PopOutImageSpacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleBaseHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleBaseWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get { return base.IsEmpty && (checkedStyle == null || CheckedStyle.IsEmpty); }
		}
		public override void CopyFrom(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				MenuItemStyleBase itemStyle = style as MenuItemStyleBase;
				if(itemStyle != null && itemStyle.checkedStyle != null)
					CheckedStyle.CopyFrom(itemStyle.CheckedStyle);
			}
		}
		public override void MergeWith(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				MenuItemStyleBase itemStyle = style as MenuItemStyleBase;
				if(itemStyle != null && itemStyle.checkedStyle != null)
					CheckedStyle.MergeWith(itemStyle.CheckedStyle);
			}
		}
		public override void Reset() {
			checkedStyle = null;
			base.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((MenuItemStyleBase)style).CreateCheckedStyle(create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
		AppearanceSelectedStyle CreateCheckedStyle(bool create) {
			if(checkedStyle == null && create)
				TrackViewState(checkedStyle = CreateCheckedStyle());
			return checkedStyle;
		}
		protected virtual AppearanceSelectedStyle CreateCheckedStyle() {
			return new AppearanceSelectedStyle();
		}
	}
	public class MenuItemStyle: MenuItemStyleBase {
		private MenuItemDropDownButtonStyle dropDownButtonStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleDropDownButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual MenuItemDropDownButtonStyle DropDownButtonStyle {
			get {
				return CreateObject(ref dropDownButtonStyle);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleDropDownButtonSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit DropDownButtonSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "DropDownButtonSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownButtonSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "DropDownButtonSpacing", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleToolbarDropDownButtonSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit ToolbarDropDownButtonSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ToolbarDropDownButtonSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ToolbarDropDownButtonSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "ToolbarDropDownButtonSpacing", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemStyleToolbarPopOutImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit ToolbarPopOutImageSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ToolbarPopOutImageSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ToolbarPopOutImageSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "ToolbarPopOutImageSpacing", Unit.Empty, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get { return base.IsEmpty && (dropDownButtonStyle == null || DropDownButtonStyle.IsEmpty)
				&& DropDownButtonSpacing.IsEmpty && ToolbarDropDownButtonSpacing.IsEmpty && ToolbarPopOutImageSpacing.IsEmpty;
			}
		}
		public override void CopyFrom(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				MenuItemStyle itemStyle = style as MenuItemStyle;
				if(itemStyle != null) {
					if(itemStyle.dropDownButtonStyle != null)
						DropDownButtonStyle.CopyFrom(itemStyle.DropDownButtonStyle);
					if(!itemStyle.DropDownButtonSpacing.IsEmpty)
						DropDownButtonSpacing = itemStyle.DropDownButtonSpacing;
					if(!itemStyle.ToolbarDropDownButtonSpacing.IsEmpty)
						ToolbarDropDownButtonSpacing = itemStyle.ToolbarDropDownButtonSpacing;
					if(!itemStyle.ToolbarPopOutImageSpacing.IsEmpty)
						ToolbarPopOutImageSpacing = itemStyle.ToolbarPopOutImageSpacing;
				}
			}
		}
		public override void MergeWith(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				MenuItemStyle itemStyle = style as MenuItemStyle;
				if(itemStyle != null) {
					if(itemStyle.dropDownButtonStyle != null)
						DropDownButtonStyle.MergeWith(itemStyle.DropDownButtonStyle);
					if(!itemStyle.DropDownButtonSpacing.IsEmpty && DropDownButtonSpacing.IsEmpty)
						DropDownButtonSpacing = itemStyle.DropDownButtonSpacing;
					if(!itemStyle.ToolbarDropDownButtonSpacing.IsEmpty && ToolbarDropDownButtonSpacing.IsEmpty)
						ToolbarDropDownButtonSpacing = itemStyle.ToolbarDropDownButtonSpacing;
					if(!itemStyle.ToolbarPopOutImageSpacing.IsEmpty && ToolbarPopOutImageSpacing.IsEmpty)
						ToolbarPopOutImageSpacing = itemStyle.ToolbarPopOutImageSpacing;
				}
			}
		}
		public override void Reset() {
			dropDownButtonStyle = null;
			DropDownButtonSpacing = Unit.Empty;
			ToolbarDropDownButtonSpacing = Unit.Empty;
			ToolbarPopOutImageSpacing = Unit.Empty;
			base.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((MenuItemStyle)style).GetObject(ref ((MenuItemStyle)style).dropDownButtonStyle, create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class MenuItemDropDownButtonStyle: MenuItemStyleBase {
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
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit PopOutImageSpacing {
			get { return base.PopOutImageSpacing; }
			set { base.PopOutImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new MenuItemDropDownButtonSelectedStyle();
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return new MenuItemDropDownButtonSelectedStyle();
		}
		protected override AppearanceSelectedStyle CreateCheckedStyle() {
			return new MenuItemDropDownButtonSelectedStyle();
		}
	}
	public class MenuItemDropDownButtonSelectedStyle: AppearanceSelectedStyle {
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
	public class MenuScrollButtonStyle: ButtonStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
	}
	public class MenuStyles : StylesBase {
		public const string ItemStyleName = "Item",
							ScrollButtonStyleName = "ScrollButton",
							SubMenuItemStyleName = "SubMenuItem",
							SubMenuStyleName = "SubMenu",
							StyleStyleName = "Style";
		protected internal const string prefix = "dxm-",
					MenuCssClass = "Lite",
					BorderCorrectorCssClass = "BrdCor",
					DisabledCssClass = prefix + "disabled",
					MainMenuCssClass = prefix + "main",
					MainPopupMenuCssClass = prefix + "popupMain",
					PopupMenuCssClass = prefix + "popup",
					PopupMenuShadowCssClass = prefix + "shadow",
					TextOnlyMenuCssClass = prefix + "t",
					TextAndImageMenuCssClass = prefix + "ti",
					HorizontalMenuCssClass = prefix + "horizontal",
					VerticalMenuCssClass = prefix + "vertical",
					DXCssClass = "dx",
					SeparatorCssClass = prefix + "separator",
					SpacingCssClass = prefix + "spacing",
					GutterCssClass = prefix + "gutter",
					WithoutImagesCssClass = prefix + "noImages",
					ItemCssClass = prefix + "item",
					ItemHoveredCssClass = prefix + "hovered",
					ItemSelectedCssClass = prefix + "selected",
					ItemCheckedCssClass = prefix + "checked",
					ItemWithoutImageCssClass = prefix + "noImage",
					ItemHasTextCssClass = prefix + "hasText",
					ItemWithSubMenuCssClass = prefix + "subMenu",
					ItemDropDownModeCssClass = prefix + "dropDownMode",
					ItemWithoutSubMenuCssClass = prefix + "noSubMenu",
					ItemTemplateCssClassName = prefix + "tmpl",
					AdaptiveMenuCssClass = prefix + "am",
					AdaptiveMenuItemCssClass = prefix + "ami",
					AdaptiveMenuItemSpacingCssClass = prefix + "amis",
					AdaptiveItemTextCssClass = prefix + "ait",
					AdaptiveItemRegularTextCssClass = prefix + "airt",
					ContentContainerCssClass = prefix + "content",
					ImageCssClass = prefix + "image",
					PopOutContainerCssClass = prefix + "popOut",
					PopOutImageCssClass = prefix + "pImage",
					ScrollAreaCssClass = prefix + "scrollArea",
					ScrollUpButtonCssClass = prefix + "scrollUpBtn",
					ScrollDownButtonCssClass = prefix + "scrollDownBtn",
					ScrollButtonHoveredCssClass = prefix + "scrollBtnHovered",
					ScrollButtonPressedCssClass = prefix + "scrollBtnPressed",
					ScrollButtonDisabledCssClass = prefix + "scrollBtnDisabled",
					AutoWidthCssClass = prefix + "autoWidth",
					NoWrapCssClass = prefix + "noWrap",
					RtlCssClass = prefix + "rtl",
					LtrCssClass = prefix + "ltr";
		public const string ToolbarCssClassPrefix = "dxtb",
			ToolbarComboBoxCssClass = ToolbarCssClassPrefix + "-comboBoxMenuItem",
			ToolbarLabelCssClass = ToolbarCssClassPrefix + "-labelMenuItem",
			ToolbarColorButtonItemCssClass = ToolbarCssClassPrefix + "-cb",
			ToolbarCustomDDImageItemCssClass = ToolbarCssClassPrefix + "-cddi",
			ToolbarCustomDDTextItemCssClass = ToolbarCssClassPrefix + "-cddt";
		protected internal const string ImageCssClassFormat = prefix + "image-{0}";
		protected bool DesignMode {
			get {
				ASPxWebControlBase control = Owner as ASPxWebControlBase;
				if(control != null)
					return control.DesignMode;
				return false;
			}
		}
		public MenuStyles(ISkinOwner menu)
			: base(menu) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Style property instead."),
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public MenuStyle Control {
			get { return Style; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesStyle"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public MenuStyle Style {
			get { return (MenuStyle)GetStyle(StyleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesItem"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public MenuItemStyle Item {
			get { return (MenuItemStyle)GetStyle(ItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesScrollButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public MenuScrollButtonStyle ScrollButton {
			get { return (MenuScrollButtonStyle)GetStyle(ScrollButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesSubMenuItem"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public MenuItemStyle SubMenuItem {
			get { return (MenuItemStyle)GetStyle(SubMenuItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesSubMenu"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public MenuStyle SubMenu {
			get { return (MenuStyle)GetStyle(SubMenuStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuStylesLink"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinkStyle Link {
			get { return base.LinkInternal; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxm";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StyleStyleName, delegate() { return new MenuStyle(); }));
			list.Add(new StyleInfo(ItemStyleName, delegate() { return new MenuItemStyle(); } ));
			list.Add(new StyleInfo(ScrollButtonStyleName, delegate() { return new MenuScrollButtonStyle(); }));
			list.Add(new StyleInfo(SubMenuItemStyleName, delegate() { return new MenuItemStyle(); } ));
			list.Add(new StyleInfo(SubMenuStyleName, delegate() { return new MenuStyle(); } ));
		}
		protected internal virtual MenuStyle GetDefaultMainMenuStyle(bool isVertical, bool isAutoWidth, bool isNoWrap) {
			string orientationCssClass = isVertical ? VerticalMenuCssClass : HorizontalMenuCssClass;
			if(!DesignMode) 
				return GetStyleWithCssClass<MenuStyle>(MainMenuCssClass, orientationCssClass, 
					isAutoWidth ? AutoWidthCssClass : string.Empty,
					isNoWrap ? NoWrapCssClass : string.Empty);
			return GetStyleWithCssClass<MenuStyle>(MainMenuCssClass, orientationCssClass);
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			AppearanceStyleBase result = base.GetDefaultDisabledStyle();
			result.CssClass = DisabledCssClass;
			return result;
		}
		protected internal virtual MenuItemStyle GetDefaultMainMenuItemStyle() {
			return GetStyleWithCssClass<MenuItemStyle>(ItemCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMainMenuItemHoverStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ItemHoveredCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMainMenuItemSelectedStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ItemSelectedCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMainMenuItemCheckedStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ItemCheckedCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMainMenuGutterStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(GutterCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMainMenuSeparatorStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(SeparatorCssClass);
		}
		protected internal virtual MenuStyle GetDefaultMenuStyle() {
			return GetStyleWithCssClass<MenuStyle>(PopupMenuCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultMenuBorderCorrectorStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(GetCssClassName(GetCssClassNamePrefix(), BorderCorrectorCssClass));
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollUpButtonStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollUpButtonCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollDownButtonStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollDownButtonCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonHoverStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollButtonHoveredCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonPressedStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollButtonPressedCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonDisabledStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollButtonDisabledCssClass);
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollAreaStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ScrollAreaCssClass);
		}
		protected internal AppearanceStyleBase CreateStyleCopyByName(string styleName) {
			return CreateStyleCopyByName<AppearanceStyleBase>(styleName);
		}
		protected string GetStylePrefix(bool isVertical) {
			return isVertical ? "Vertical" : "";
		}
		protected internal virtual AppearanceStyleBase GetMenuItemSpacingStyle(bool isMainMenu, bool isLargeItem, bool isVertical, bool hasSeparator) {
			string prefix;
			if(isMainMenu) {
				prefix = GetStylePrefix(isVertical) + "Menu";
				if(isLargeItem)
					prefix += "Large";
			}
			else
				prefix = "SubMenu";
			string separator = hasSeparator ? "Separator" : "";
			return CreateCachedStyle<AppearanceStyleBase>(string.Concat(prefix, "Item", separator, "Spacing"), null);
		}
		const string MenuItemLeftImageSpacingStyleName = "MenuItemLeftImageSpacing";
		const string MenuItemRightImageSpacingStyleName = "MenuItemRightImageSpacing";
		const string MenuItemTopImageSpacingStyleName = "MenuItemTopImageSpacing";
		const string MenuItemBottomImageSpacingStyleName = "MenuItemBottomImageSpacing";
		const string SubMenuItemImageSpacingStyleName = "SubMenuItemImageSpacing";
		protected internal virtual AppearanceStyleBase GetMenuItemImageSpacingStyle(bool isMainMenu, ImagePosition imagePosition, bool isVertical) {
			string styleName;
			if(isMainMenu) {
				styleName = GetStylePrefix(isVertical);
				if(imagePosition == ImagePosition.Left)
					styleName += SkinOwner.IsRightToLeft() ? MenuItemRightImageSpacingStyleName : MenuItemLeftImageSpacingStyleName;
				else if(imagePosition == ImagePosition.Right)
					styleName += SkinOwner.IsRightToLeft() ? MenuItemLeftImageSpacingStyleName : MenuItemRightImageSpacingStyleName;
				else if(imagePosition == ImagePosition.Top)
					styleName += MenuItemTopImageSpacingStyleName;
				else if(imagePosition == ImagePosition.Bottom)
					styleName += MenuItemBottomImageSpacingStyleName;
			}
			else
				styleName = SubMenuItemImageSpacingStyleName;
			return CreateStyleCopyByName(styleName);
		}
		protected virtual Unit GetMainMenuItemPopOutImageSpacing(bool isVertical) {
			return isVertical ? 18 : GetImageSpacing();
		}
		protected virtual Unit GetMainMenuItemDropDownButtonSpacing(bool isVertical) {
			return isVertical ? 18 : GetImageSpacing();
		}
		protected virtual Unit GetMainMenuItemToolbarPopOutImageSpacing(bool isVertical) {
			return isVertical ? 18 : GetImageSpacing();
		}
		protected virtual Unit GetMainMenuItemToolbarDropDownButtonSpacing(bool isVertical) {
			return isVertical ? 18 : GetImageSpacing();
		}
		protected virtual Unit GetMainMenuLargeItemPopOutImageSpacing(bool isVertical) {
			return GetImageSpacing();
		}
		protected virtual Unit GetMainMenuLargeItemDropDownButtonSpacing(bool isVertical) {
			return 8;
		}
		protected virtual Unit GetMenuItemPopOutImageSpacing() {
			return 18;
		}
		protected virtual Unit GetMenuItemDropDownButtonSpacing() {
			return 18;
		}
		protected virtual Unit GetMenuItemToolbarPopOutImageSpacing() {
			return 8;
		}
		protected virtual Unit GetMenuItemToolbarDropDownButtonSpacing() {
			return 8;
		}
		protected string GetRtlSuffix() {
			if(SkinOwner.IsRightToLeft())
				return "Rtl";
			return String.Empty;
		}
		protected internal virtual string GetImageCssClass(ImagePosition position) {
			if(SkinOwner.IsRightToLeft()) {
				if(position == ImagePosition.Left)
					position = ImagePosition.Right;
				else if(position == ImagePosition.Right)
					position = ImagePosition.Left;
			}
			return string.Format(ImageCssClassFormat, position.ToString().ToLower().Substring(0, 1));
		}
		protected internal string GetRootContainerCssClass() {
			return RenderUtils.CombineCssClasses(GetCssClassName(GetCssClassNamePrefix(), MenuCssClass), SkinOwner.IsRightToLeft() ? MenuStyles.RtlCssClass : MenuStyles.LtrCssClass);
		}
		protected T GetStyleWithCssClass<T>(params string[] cssClasses) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses);
			return style;
		}
	}
}
