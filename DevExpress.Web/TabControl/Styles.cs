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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class TabControlStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStyleTabSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit TabSpacing
		{
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStyleScrollButtonSpacing"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ScrollButtonSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ScrollButtonSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "ScrollButtonSpacing", Unit.Empty, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStyleScrollButtonsIndent"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonsIndent {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ScrollButtonsIndent", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "ScrollButtonsIndent");
				ViewStateUtils.SetUnitProperty(ViewState, "ScrollButtonsIndent", Unit.Empty, value); 
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get { return base.IsEmpty && ScrollButtonSpacing.IsEmpty && ScrollButtonsIndent.IsEmpty; }
		}
		public override void CopyFrom(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				TabControlStyle tabControlStyle = style as TabControlStyle;
				if(tabControlStyle != null) {
					if(!tabControlStyle.ScrollButtonSpacing.IsEmpty)
						ScrollButtonSpacing = tabControlStyle.ScrollButtonSpacing;
					if(!tabControlStyle.ScrollButtonsIndent.IsEmpty)
						ScrollButtonsIndent = tabControlStyle.ScrollButtonsIndent;
				}
			}
		}
		public override void MergeWith(Style style) {
			if((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				TabControlStyle tabControlStyle = style as TabControlStyle;
				if(tabControlStyle != null) {
					if(!tabControlStyle.ScrollButtonSpacing.IsEmpty && ScrollButtonSpacing.IsEmpty)
						ScrollButtonSpacing = tabControlStyle.ScrollButtonSpacing;
					if(!tabControlStyle.ScrollButtonsIndent.IsEmpty && ScrollButtonsIndent.IsEmpty)
						ScrollButtonsIndent = tabControlStyle.ScrollButtonsIndent;
				}
			}
		}
		public override void Reset() {
			ScrollButtonSpacing = Unit.Empty;
			ScrollButtonsIndent = Unit.Empty;
			base.Reset();
		}
	}
	public class TabStyle : AppearanceStyle{
		[
#if !SL
	DevExpressWebLocalizedDescription("TabStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class ContentStyle: AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return Unit.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return Unit.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
	}
	public class TabControlStyles : StylesBase {
		public const string
			ActiveTabStyleName = "ActiveTab",
			TabStyleName = "Tab",
			ContentStyleName = "Content",
			SpaceBeforeTabsTemplateStyleName = "SpaceBeforeTabsTemplate",
			SpaceAfterTabsTemplateStyleName = "SpaceAfterTabsTemplate",
			ScrollButtonStyleName = "ScrollButton";
		internal const string
			SystemCssClassName = "dxtcSys",
			InitializationCssClassName = "dxtc-init",
			RightToLeftCssMarker = "dxtc-rtl",
			NoTabSpacingCssMarker = "dxtc-noSpacing",
			TabStripContainerCssMarker = "dxtc-stripContainer",
			TabStripCssClassName = "dxtc-strip",
			TabStripWrapperCssClassName = "dxtc-wrapper",
			TabStripHolderCssClassName = "dxtc-stripHolder",
			TabStripLeftIndentCssClassName = "dxtc-leftIndent",
			TabStripRightIndentCssClassName = "dxtc-rightIndent",
			TabStripIndentTemplateCssClassName = "dxtc-it",
			FirstTabCssMarker = "dxtc-lead",
			TabItemLinkCssClassName = "dxtc-link",
			TabItemImageCssClassName = "dxtc-img",
			TabItemSpacerCssClassName = "dxtc-spacer",
			LineBrakeCssClassName = "dxtc-lineBreak",
			TabStartsNewLineCssMarker = "dxtc-n",
			NoTabsCssMarker = "dxtc-noTabs",
			MultiRowModeCssMarker = "dxtc-multiRow";
		public TabControlStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesActiveTab"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle ActiveTab {
			get { return (TabStyle)GetStyle(ActiveTabStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesTab"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle Tab {
			get { return (TabStyle)GetStyle(TabStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContentStyle Content {
			get { return (ContentStyle)GetStyle(ContentStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesSpaceBeforeTabsTemplate"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceBeforeTabsTemplate {
			get { return (SpaceTabTemplateStyle)GetStyle(SpaceBeforeTabsTemplateStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesSpaceAfterTabsTemplate"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceAfterTabsTemplate {
			get { return (SpaceTabTemplateStyle)GetStyle(SpaceAfterTabsTemplateStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabControlStylesScrollButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle ScrollButton {
			get { return (ButtonStyle)GetStyle(ScrollButtonStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxtc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(CreateActiveTabStyleInfo());
			list.Add(CreateTabStyleInfo());
			list.Add(CreateContentStyleInfo());
			list.Add(CreateSpaceBeforeTabsTemplateStyleInfo());
			list.Add(CreateSpaceAfterTabsTemplateStyleInfo());
			list.Add(CreateScrollButtonStyleInfo());
		}
		protected virtual StyleInfo CreateActiveTabStyleInfo() {
			return new StyleInfo(ActiveTabStyleName, delegate() { return new TabStyle(); });
		}
		protected virtual StyleInfo CreateTabStyleInfo() {
			return new StyleInfo(TabStyleName, delegate() { return new TabStyle(); });
		}
		protected virtual StyleInfo CreateContentStyleInfo() {
			return new StyleInfo(ContentStyleName, delegate() { return new ContentStyle(); });
		}
		protected virtual StyleInfo CreateScrollButtonStyleInfo() {
			return new StyleInfo(ScrollButtonStyleName, delegate() { return new ButtonStyle(); });
		}
		protected virtual StyleInfo CreateSpaceBeforeTabsTemplateStyleInfo() {
			return new StyleInfo(SpaceBeforeTabsTemplateStyleName, delegate() { return new SpaceTabTemplateStyle(); });
		}
		protected virtual StyleInfo CreateSpaceAfterTabsTemplateStyleInfo() {
			return new StyleInfo(SpaceAfterTabsTemplateStyleName, delegate() { return new SpaceTabTemplateStyle(); });
		}
		internal static string GetTabPositionCssMarker(TabPosition tabPosition) {
			return "dxtc-" + tabPosition.ToString().ToLower();
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle style = base.GetDefaultControlStyle();
			style.Spacing = GetTabSpacing();
			style.Paddings.Assign(GetTabsPaddings());
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), "Lite");
			return style;
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			AppearanceStyleBase result = base.GetDefaultDisabledStyle();
			result.CssClass = GetCssClassName(GetCssClassNamePrefix(), "LiteDisabled");
			return result;
		}
		protected internal virtual TabStyle GetDefaultActiveTabStyle(TabPosition position) {
			return CreateCssStyle<TabStyle>("dxtc-activeTab");
		}
		protected internal virtual AppearanceStyleBase GetDefaultActiveTabHoverStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-activeTabHover");
		}
		protected internal virtual TabStyle GetDefaultTabStyle(TabPosition position) {
			return CreateCssStyle<TabStyle>("dxtc-tab");
		}
		protected internal virtual AppearanceStyleBase GetDefaultTabHoverStyle(TabPosition position) {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-tabHover");
		}
		protected internal virtual ContentStyle GetDefaultContentStyle() {
			return CreateCssStyle<ContentStyle>("dxtc-content");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonCellStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbWrapper");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonSeparatorStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbSpacer");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonIndentStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbIndent");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sb");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonHoverStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbHover");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonPressedStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbPressed");
		}
		protected internal virtual AppearanceStyleBase GetDefaultScrollButtonDisabledStyle() {
			return CreateCssStyle<AppearanceStyleBase>("dxtc-sbDisabled");
		}
		protected T CreateCssStyle<T>(string cssClassName) where T : AppearanceStyleBase, new() {
			T result = new T();
			result.CssClass = cssClassName;
			return result;
		}
		protected virtual Unit GetTabSpacing() {
			return Unit.Empty;
		}
		protected virtual Unit GetTabImageSpacing() {
			return GetImageSpacing();
		}
		public virtual Paddings GetTabsPaddings() {
			return new Paddings();
		}
	}
	public class SpaceTabTemplateStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("SpaceTabTemplateStyleHorizontalAlign"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpaceTabTemplateStyleVerticalAlign"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor { get { return base.Cursor; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor { get { return base.BackColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssClass { get { return base.CssClass; } }
	}  
}
