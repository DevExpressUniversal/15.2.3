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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class NavBarStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStyleGroupSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit GroupSpacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class NavBarGroupHeaderStyle : AppearanceStyle {
		[
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return Unit.Empty; }
			set { }
		}
	}
	public class NavBarGroupContentStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
		[
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual Unit ItemSpacing {
			get { return Spacing; }
			set { Spacing = value; }
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
			get { return VerticalAlign.NotSet; }
			set { }
		}
	}
	public class NavBarItemStyle : AppearanceItemStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class NavBarStyles : StylesBase {
		public const string GroupHeaderStyleName = "GroupHeader",
							GroupHeaderCollapsedStyleName = "GroupHeaderCollapsed",
							GroupContentStyleName = "GroupContent",
							ItemStyleName = "Item";
		internal const string
			NoHeadsCssMarker = "dxnb-noHeads",
			LastCssMarker = "dxnb-last",
			ControlSystemClassName = "dxnbSys",
			GroupCssClassName = "dxnb-gr",
			TextOnlyGroupCssClassName = "dxnb-t",
			TextAndImageGroupCssClassName = "dxnb-ti",
			ExpandButtonCssClassName = "dxnb-btn",
			ExpandButtonLeftCssClassName = "dxnb-btnLeft",
			ItemCssClassName = "dxnb-item",
			LargeCssClassName = "dxnb-large",
			BulletCssClassName = "dxnb-bullet",
			ImageCssClassName = "dxnb-img",
			LinkCssClassName = "dxnb-link",
			ItemTemplateCssClassName = "dxnb-tmpl",
			RtlHeaderCssClassName = "dxnb-rtlHeader",
			HeaderWithLeftExpandButtonCssClassName = "dxnb-header-left";
		public NavBarStyles(ASPxNavBar navBar)
			: base(navBar) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesGroupHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public NavBarGroupHeaderStyle GroupHeader {
			get { return (NavBarGroupHeaderStyle)GetStyle(GroupHeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesGroupHeaderCollapsed"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public NavBarGroupHeaderStyle GroupHeaderCollapsed {
			get { return (NavBarGroupHeaderStyle)GetStyle(GroupHeaderCollapsedStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesGroupContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public NavBarGroupContentStyle GroupContent {
			get { return (NavBarGroupContentStyle)GetStyle(GroupContentStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public NavBarItemStyle Item {
			get { return (NavBarItemStyle)GetStyle(ItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesLink"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinkStyle Link {
			get { return base.LinkInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarStylesLoadingPanel"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxnb";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(GroupHeaderStyleName, delegate() { return new NavBarGroupHeaderStyle(); }));
			list.Add(new StyleInfo(GroupHeaderCollapsedStyleName, delegate() { return new NavBarGroupHeaderStyle(); }));
			list.Add(new StyleInfo(GroupContentStyleName, delegate() { return new NavBarGroupContentStyle(); }));
			list.Add(new StyleInfo(ItemStyleName, delegate() { return new NavBarItemStyle(); } ));
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), "Lite");
			return style;
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), "LiteDisabled");
			return style;
		}
		protected internal virtual NavBarGroupHeaderStyle GetDefaultGroupHeaderStyle(bool expanded) {
			NavBarGroupHeaderStyle style = new NavBarGroupHeaderStyle();
			style.CssClass = "dxnb-header";
			if(!expanded)
				style.CssClass += "Collapsed";
			if(SkinOwner.IsRightToLeft()) {
				style.HorizontalAlign = HorizontalAlign.Right;
				style.CssClass += " " + RtlHeaderCssClassName;
			}
			return style;
		}
		protected internal virtual NavBarGroupHeaderStyle GetDefaultGroupHeaderHoverStyle(bool expanded) {
			NavBarGroupHeaderStyle style = GetDefaultGroupHeaderStyle(expanded);
			style.CssClass += "Hover";
			return style;
		}
		protected internal virtual NavBarGroupContentStyle GetDefaultGroupContentStyle(bool isBulletMode, bool isLargeItems) {
			NavBarGroupContentStyle style = new NavBarGroupContentStyle();
			style.CssClass = "dxnb-content";
			return style;
		}
		protected internal virtual NavBarItemStyle GetDefaultItemStyle(bool isBulletMode, bool isLargeItems) {
			NavBarItemStyle style = new NavBarItemStyle();
			style.CssClass = GetItemCssClassName(isBulletMode, isLargeItems);
			if(!isLargeItems && SkinOwner.IsRightToLeft())
				style.HorizontalAlign = HorizontalAlign.Right;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultItemDisabledStyle() {
			AppearanceStyleBase style = GetDefaultItemStyle(false, false);
			style.CssClass += "Disabled";
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, GetDefaultDisabledStyle().CssClass);
			return style;
		}
		protected internal virtual NavBarItemStyle GetDefaultItemHoverStyle(bool isBulletMode, bool isLargeItems) {
			NavBarItemStyle style = GetDefaultItemStyle(isBulletMode, isLargeItems);
			style.CssClass += "Hover";
			return style;
		}
		protected internal virtual NavBarItemStyle GetDefaultItemSelectedStyle(bool isBulletMode, bool isLargeItems) {
			NavBarItemStyle style = GetDefaultItemStyle(isBulletMode, isLargeItems);
			style.CssClass += "Selected";
			return style;
		}
		protected virtual string GetItemCssClassName(bool isBulletMode, bool isLargeItems) {
			if(isLargeItems) 
				return LargeCssClassName;
			if(isBulletMode) 
				return BulletCssClassName;
			return ItemCssClassName;
		}
		protected internal string GetLargeItemImageCssClassName(ImagePosition position) {
			if(position == ImagePosition.Top)
				return GetCssClassName(GetCssClassNamePrefix(), "LargeItemImgTop");
			else if(position == ImagePosition.Bottom)
				return GetCssClassName(GetCssClassNamePrefix(), "LargeItemImgBottom");
			return "";
		}
		internal static string GetGroupImagePositionCssMarker(ImagePosition value) {
			return "dxnb-" + value.ToString().ToLower();
		}
		protected override bool HideLoadingPanelBorder() {
			return true;
		}
	}
}
