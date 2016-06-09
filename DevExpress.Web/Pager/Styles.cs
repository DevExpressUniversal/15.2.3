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
using DevExpress.Utils;
namespace DevExpress.Web {
	public class PagerStyle : AppearanceStyle {
		AppearanceStyle separatorStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleItemSpacing"),
#endif
 AutoFormatEnable,
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit ItemSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleSeparatorBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage SeparatorBackgroundImage {
			get { return SeparatorStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleSeparatorColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter))]
		public virtual Color SeparatorColor {
			get { return SeparatorStyle.BackColor; }
			set { SeparatorStyle.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleSeparatorHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit SeparatorHeight {
			get { return SeparatorStyle.Height; }
			set { SeparatorStyle.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleSeparatorPaddings"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings SeparatorPaddings {
			get { return SeparatorStyle.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStyleSeparatorWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public virtual Unit SeparatorWidth {
			get { return SeparatorStyle.Width; }
			set { SeparatorStyle.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return HorizontalAlign.NotSet; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return VerticalAlign.NotSet; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceStyle SeparatorStyle {
			get {
				return CreateObject(ref separatorStyle);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsEmpty {
			get { return base.IsEmpty && (separatorStyle == null || SeparatorStyle.IsEmpty); }
		}
		public override void CopyFrom(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.CopyFrom(style);
				PagerStyle pagerStyle = style as PagerStyle;
				if(pagerStyle != null && pagerStyle.separatorStyle != null) {
					SeparatorStyle.CopyFrom(pagerStyle.SeparatorStyle);
				}
			}
		}
		public override void MergeWith(Style style) {
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				PagerStyle pagerStyle = style as PagerStyle;
				if(pagerStyle != null && pagerStyle.separatorStyle != null) {
					SeparatorStyle.MergeWith(pagerStyle.SeparatorStyle);
				}
			}
		}
		public override void Reset() {
			base.Reset();
			if(separatorStyle != null)
				SeparatorStyle.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((PagerStyle)style).GetObject(ref ((PagerStyle)style).separatorStyle, create); ; });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class PagerItemStyle: AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerItemStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerItemStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class PagerButtonStyle: PagerItemStyle {
	}
	public class PagerPageSizeItemStyle : PagerItemStyle {
		private PagerComboBoxStyle comboBoxStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerPageSizeItemStyleCaptionSpacing"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(typeof(Unit), "")]
		public Unit CaptionSpacing
		{
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerPageSizeItemStyleComboBoxStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerComboBoxStyle ComboBoxStyle
		{
			get
			{
				return CreateObject(ref comboBoxStyle);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			PagerPageSizeItemStyle altStyle = style as PagerPageSizeItemStyle;
			if(altStyle != null) {
				ComboBoxStyle.CopyFrom(altStyle.ComboBoxStyle);
			}
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			PagerPageSizeItemStyle altStyle = style as PagerPageSizeItemStyle;
			if(altStyle != null) {
				ComboBoxStyle.MergeWith(altStyle.ComboBoxStyle);
			}
		}
		public override void Reset() {
			base.Reset();
			ComboBoxStyle.Reset();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((PagerPageSizeItemStyle)style).GetObject(ref ((PagerPageSizeItemStyle)style).comboBoxStyle, create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class PagerComboBoxStyle : AppearanceStyle {
		private PagerDropDownButtonStyle dropDownButtonStyle;
		private PagerDropDownWindowStyle dropDownWindowStyle;
		private PagerDropDownItemStyle itemStyle;
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStyleDropDownButtonSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit DropDownButtonSpacing {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "DropDownButtonSpacing", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownButtonSpacing");
				ViewStateUtils.SetUnitProperty(ViewState, "DropDownButtonSpacing", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStyleDropDownButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerDropDownButtonStyle DropDownButtonStyle {
			get {
				return CreateObject(ref dropDownButtonStyle);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStyleDropDownWindowStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerDropDownWindowStyle DropDownWindowStyle {
			get {
				return CreateObject(ref dropDownWindowStyle);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStyleItemStyle"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerDropDownItemStyle ItemStyle
		{
			get
			{
				return CreateObject(ref itemStyle);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStyleDisabledStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerComboBoxStylePressedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			PagerComboBoxStyle altStyle = style as PagerComboBoxStyle;
			if(altStyle != null) {
				DropDownButtonStyle.CopyFrom(altStyle.DropDownButtonStyle);
				DropDownWindowStyle.CopyFrom(altStyle.DropDownWindowStyle);
				ItemStyle.CopyFrom(altStyle.ItemStyle);
				if(!altStyle.DropDownButtonSpacing.IsEmpty)
					DropDownButtonSpacing = altStyle.DropDownButtonSpacing;
			}
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			PagerComboBoxStyle altStyle = style as PagerComboBoxStyle;
			if(altStyle != null) {
				DropDownButtonStyle.MergeWith(altStyle.DropDownButtonStyle);
				DropDownWindowStyle.MergeWith(altStyle.DropDownWindowStyle);
				ItemStyle.MergeWith(altStyle.ItemStyle);
				if(!altStyle.DropDownButtonSpacing.IsEmpty)
					DropDownButtonSpacing = altStyle.DropDownButtonSpacing;
			}
		}
		public override void Reset() {
			base.Reset();
			DropDownButtonStyle.Reset();
			DropDownWindowStyle.Reset();
			ItemStyle.Reset();
			DropDownButtonSpacing = Unit.Empty;
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((PagerComboBoxStyle)style).GetObject(ref ((PagerComboBoxStyle)style).dropDownButtonStyle, create); });
				list.Add(delegate(object style, bool create) { return ((PagerComboBoxStyle)style).GetObject(ref ((PagerComboBoxStyle)style).dropDownWindowStyle, create); });
				list.Add(delegate(object style, bool create) { return ((PagerComboBoxStyle)style).GetObject(ref ((PagerComboBoxStyle)style).itemStyle, create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
	}
	public class PagerDropDownButtonStyle : ButtonStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
	}
	public class PagerDropDownWindowStyle : MenuStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage GutterBackgroundImage {
			get { return base.GutterBackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit TextIndent {
			get { return base.TextIndent; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit GutterWidth {
			get { return base.GutterWidth; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage SeparatorBackgroundImage {
			get { return base.SeparatorBackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color SeparatorColor {
			get { return base.SeparatorColor; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit SeparatorHeight {
			get { return base.SeparatorHeight; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings SeparatorPaddings {
			get { return base.SeparatorPaddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit SeparatorWidth {
			get { return base.SeparatorWidth; }
			set { }
		}
	}
	public class PagerDropDownItemStyle : MenuItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit DropDownButtonSpacing {
			get { return base.DropDownButtonSpacing; }
			set { base.DropDownButtonSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override MenuItemDropDownButtonStyle DropDownButtonStyle {
			get { return base.DropDownButtonStyle; }
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
		public override Unit ToolbarDropDownButtonSpacing {
			get { return base.ToolbarDropDownButtonSpacing; }
			set { base.ToolbarDropDownButtonSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ToolbarPopOutImageSpacing {
			get { return base.ToolbarPopOutImageSpacing; }
			set { base.ToolbarPopOutImageSpacing = value; }
		}
	}
	public class PagerTextStyle: PagerItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class PagerStyles : StylesBase {
		public const string ButtonStyleName = "Button",
							DisabledButtonStyleName = "DisabledButton",
							CurrentPageNumberStyleName = "CurrentPageNumber",
							PageNumberStyleName = "PageNumber",
							PagerStyleName = "Pager",
							PageSizeItemStyleName = "PageSizeItem",
							SummaryStyleName = "Summary",
							EllipsisStyleName = "Ellipsis";
		internal const string LeadClassName = "dxp-lead",
							  DesignModePageSizeItemClassName = "dxp-pageSizeItemCell";
		private MenuStyles dropDownWindowStyles = null;
		public PagerStyles(ISkinOwner pager)
			: base(pager) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle Button {
			get { return (PagerButtonStyle)GetStyle(ButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesDisabledButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerButtonStyle DisabledButton {
			get { return (PagerButtonStyle)GetStyle(DisabledButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesCurrentPageNumber"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle CurrentPageNumber {
			get { return (PagerTextStyle)GetStyle(CurrentPageNumberStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesPageNumber"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle PageNumber {
			get { return (PagerTextStyle)GetStyle(PageNumberStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesPager"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerStyle Pager {
			get { return (PagerStyle)GetStyle(PagerStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesPageSizeItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerPageSizeItemStyle PageSizeItem
		{
			get { return (PagerPageSizeItemStyle)GetStyle(PageSizeItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesSummary"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle Summary {
			get { return (PagerTextStyle)GetStyle(SummaryStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerStylesEllipsis"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PagerTextStyle Ellipsis {
			get { return (PagerTextStyle)GetStyle(EllipsisStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxp";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new PagerButtonStyle(); } ));
			list.Add(new StyleInfo(DisabledButtonStyleName, delegate() { return new PagerButtonStyle(); } ));
			list.Add(new StyleInfo(CurrentPageNumberStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(PageNumberStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(PagerStyleName, delegate() { return new PagerStyle(); }));
			list.Add(new StyleInfo(PageSizeItemStyleName, delegate() { return new PagerPageSizeItemStyle(); }));
			list.Add(new StyleInfo(SummaryStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(EllipsisStyleName, delegate() { return new PagerTextStyle(); }));
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
		protected internal virtual PagerButtonStyle GetDefaultButtonStyle(bool hasText, bool hasImage) {
			PagerButtonStyle style = new PagerButtonStyle();
			style.ImageSpacing = GetButtonImageSpacing();
			style.CssClass = "dxp-button";
			if(hasImage && hasText)
				style.CssClass += " dxp-bti";
			else if(hasText)
				style.CssClass += " dxp-bt";
			else if(hasImage)
				style.CssClass += " dxp-bi";
			return style;
		}
		protected internal virtual PagerButtonStyle GetDefaultDisabledButtonStyle() {
			PagerButtonStyle style = new PagerButtonStyle();
			style.CssClass = "dxp-disabledButton";
			return style;
		}
		protected internal virtual PagerTextStyle GetDefaultPageNumberStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CssClass = "dxp-num";
			return style;
		}
		protected internal virtual PagerTextStyle GetDefaultCurrentPageNumberStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CssClass = "dxp-current";
			return style;
		}
		protected internal virtual PagerPageSizeItemStyle GetDefaultPageSizeItemStyle() {
			PagerPageSizeItemStyle style = new PagerPageSizeItemStyle();
			style.CssClass = "dxp-pageSizeItem";
			style.CaptionSpacing = GetPageSizeBoxSpacing();
			style.ComboBoxStyle.DropDownButtonSpacing = GetDropDownButtonSpacing();
			return style;
		}
		protected internal virtual PagerComboBoxStyle GetDefaultComboBoxStyle() {
			PagerComboBoxStyle style = new PagerComboBoxStyle();
			style.CssClass = "dxp-comboBox";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultDisabledComboBoxStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-disabledComboBox";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultHoverComboBoxStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-hoverComboBox";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultPressedComboBoxStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-pressedComboBox";
			return style;
		}
		protected internal virtual PagerDropDownButtonStyle GetDefaultDropDownButtonStyle() {
			PagerDropDownButtonStyle style = new PagerDropDownButtonStyle();
			style.CssClass = "dxp-dropDownButton";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultDisabledDropDownButtonStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-disabledDropDownButton";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultHoverDropDownButtonStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-hoverDropDownButton";
			return style;
		}
		protected internal virtual AppearanceSelectedStyle GetDefaultPressedDropDownButtonStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CssClass = "dxp-pressedDropDownButton";
			return style;
		}
		protected internal virtual MenuStyles GetDefaultDropDownWindowStyle() {
			if(this.dropDownWindowStyles == null)
				this.dropDownWindowStyles = new MenuStyles(base.SkinOwner);
			return this.dropDownWindowStyles;
		}
		protected internal virtual PagerTextStyle GetDefaultSummaryStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CssClass = "dxp-summary";
			return style;
		}
		protected internal virtual PagerTextStyle GetDefaultEllipsisStyle() {
			PagerTextStyle style = new PagerTextStyle();
			style.CssClass = "dxp-ellip";
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultSeparatorStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = "dxp-sep";
			return style;
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			PagerStyles styles = source as PagerStyles;
			if(styles != null) {
				Button.CopyFrom(styles.Button);
				DisabledButton.CopyFrom(styles.DisabledButton);
				CurrentPageNumber.CopyFrom(styles.CurrentPageNumber);
				PageNumber.CopyFrom(styles.PageNumber);
				PageSizeItem.CopyFrom(styles.PageSizeItem);
				Summary.CopyFrom(styles.Summary);
			}
		}
		public override void Reset() {
			base.Reset();
			Button.Reset();
			DisabledButton.Reset();
			CurrentPageNumber.Reset();
			PageNumber.Reset();
			PageSizeItem.Reset();
			Summary.Reset();
		}
		protected virtual Unit GetItemSpacing() {
			return 4;
		}
		protected virtual Paddings GetButtonPaddings() {
			return new Paddings(5, 0, 5, 0);
		}
		protected virtual Unit GetButtonImageSpacing() {
			return GetImageSpacing();
		}
		protected virtual Unit GetPageSizeBoxSpacing() {
			return 4;
		}
		protected virtual Unit GetDropDownButtonSpacing() {
			return 4;
		}
		protected virtual Paddings GetSeparatorPaddings() {
			return new Paddings(Unit.Empty, 2, Unit.Empty, 0);
		}
		protected virtual Unit GetSeparatorHeight() {
			return 11;
		}
		protected virtual Unit GetSeparatorWidth() {
			return 1;
		}
	}
}
