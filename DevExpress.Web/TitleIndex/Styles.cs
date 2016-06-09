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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class IndexPanelItemStyle : IndexPanelStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override Unit LineSpacing {
			get { return base.Spacing; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelItemStyleDisabledForeColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), AutoFormatEnable,
		TypeConverter(typeof(WebColorConverter))]
		public Color DisabledForeColor {
			get { return DisabledStyle.ForeColor; }
			set { DisabledStyle.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelItemStyleCurrentStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceSelectedStyle CurrentStyle {
			get { return base.SelectedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelItemStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
	}
	public class IndexPanelStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelStylePaddings"),
#endif
		Category("Layout"), PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override DefaultBoolean Wrap {
			get { return (DefaultBoolean)ViewStateUtils.GetDefaultBooleanProperty(ReadOnlyViewState, "Wrap", DefaultBoolean.Default); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "Wrap", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelStyleLineSpacing"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit LineSpacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class IndexPanelSeparatorStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override string Cursor {
			get { return base.Cursor; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("IndexPanelSeparatorStyleHeight"),
#endif
		Category("Layout"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		new public FontInfo Font {
			get { return base.Font; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		new public Color ForeColor {
			get { return base.ForeColor; }
			set { }
		}
	}
	public class TitleIndexGroupHeaderStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return Unit.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
	}
	public class TitleIndexGroupContentStyle : AppearanceStyle {
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
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexGroupContentStyleItemSpacing"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ItemSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
	}
	public class TitleIndexItemStyle : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemStyleCurrentItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public AppearanceSelectedStyle CurrentItemStyle {
			get { return base.HoverStyle; }
		}
	}
	public class FilterBoxStyle : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
	}
	public class FilterBoxEditorStyle : FilterBoxStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxEditorStyleHeight"),
#endif
		PersistenceMode(PersistenceMode.Attribute), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterBoxEditorStyleWidth"),
#endif
		PersistenceMode(PersistenceMode.Attribute), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
	}
	public class FilterBoxInfoTextStyle : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
	}
	public class TitleIndexStyles : StylesBase {
		public const string BackToTopStyleName = "BackToTop";
		public const string ColumnSeparatorStyleName = "ColumnSeparator";
		public const string ColumnStyleName = "Column";
		public const string IndexPanelStyleName = "IndexPanel";
		public const string IndexPanelItemStyleName = "IndexPanelItem";
		public const string IndexPanelSeparatorStyleName = "IndexPanelSeparator";
		public const string FilterBoxEditStyleName = "FilterBoxEdit";
		public const string FilterBoxStyleName = "FilterBox";
		public const string FilterBoxInfoTextStyleName = "FilterBoxInfoText";
		public const string GroupHeaderStyleName = "GroupHeader";
		public const string GroupHeaderTextStyleName = "GroupHeaderText";
		public const string GroupContentStyleName = "GroupContent";
		public const string ItemStyleName = "Item";
		public const string ControlSystemStyleName = "dxtiControlSys";
		public const string IndexPanelSystemStyleName = "dxtiIndexPanelSys";
		public const string FilterBoxEditSystemStyleName = "dxtiFilterBoxEditSys";
		public static string GroupSystemStyleName  = "dxti-g";
		public static string ItemSystemStyleName  = "dxti-i";
		public static string LinkSystemStyleName  = "dxti-link";
		private LinkStyle indexPanelItemLink = new LinkStyle();
		public TitleIndexStyles(ASPxTitleIndex titleIndex)
			: base(titleIndex) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesBackToTop"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BackToTopStyle BackToTop {
			get { return (BackToTopStyle)GetStyle(BackToTopStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesColumnSeparator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ColumnSeparatorStyle ColumnSeparator {
			get { return (ColumnSeparatorStyle)GetStyle(ColumnSeparatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesColumn"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ColumnStyle Column {
			get { return (ColumnStyle)GetStyle(ColumnStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesIndexPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public IndexPanelStyle IndexPanel {
			get { return (IndexPanelStyle)GetStyle(IndexPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesIndexPanelItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public IndexPanelItemStyle IndexPanelItem {
			get { return (IndexPanelItemStyle)GetStyle(IndexPanelItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesIndexPanelSeparator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public IndexPanelSeparatorStyle IndexPanelSeparator {
			get { return (IndexPanelSeparatorStyle)GetStyle(IndexPanelSeparatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesFilterBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public FilterBoxStyle FilterBox {
			get { return (FilterBoxStyle)GetStyle(FilterBoxStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesFilterBoxEdit"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public FilterBoxEditorStyle FilterBoxEdit {
			get { return (FilterBoxEditorStyle)GetStyle(FilterBoxEditStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesFilterBoxInfoText"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public FilterBoxInfoTextStyle FilterBoxInfoText {
			get { return (FilterBoxInfoTextStyle)GetStyle(FilterBoxInfoTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesGroupHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TitleIndexGroupHeaderStyle GroupHeader {
			get { return (TitleIndexGroupHeaderStyle)GetStyle(GroupHeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesGroupHeaderText"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TitleIndexGroupHeaderStyle GroupHeaderText {
			get { return (TitleIndexGroupHeaderStyle)GetStyle(GroupHeaderTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesGroupContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TitleIndexGroupContentStyle GroupContent {
			get { return (TitleIndexGroupContentStyle)GetStyle(GroupContentStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TitleIndexItemStyle Item {
			get { return (TitleIndexItemStyle)GetStyle(ItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexStylesIndexPanelItemLink"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public LinkStyle IndexPanelItemLink {
			get { return indexPanelItemLink; }
		}
		#region CopyFrom, Reset
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			TitleIndexStyles src = source as TitleIndexStyles;
			if(src != null) {
				IndexPanelItemLink.CopyFrom(src.IndexPanelItemLink);
			}
		}
		public override void Reset() {
			base.Reset();
			IndexPanelItemLink.Reset();
		}
		#endregion
		protected override bool MakeLinkStyleAttributesImportant {
			get { return true; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxti";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(BackToTopStyleName, delegate() { return new BackToTopStyle(); } ));
			list.Add(new StyleInfo(ColumnSeparatorStyleName, delegate() { return new ColumnSeparatorStyle(); } ));
			list.Add(new StyleInfo(ColumnStyleName, delegate() { return new ColumnStyle(); } ));
			list.Add(new StyleInfo(IndexPanelStyleName, delegate() { return new IndexPanelStyle(); } ));
			list.Add(new StyleInfo(IndexPanelItemStyleName, delegate() { return new IndexPanelItemStyle(); } ));
			list.Add(new StyleInfo(IndexPanelSeparatorStyleName, delegate() { return new IndexPanelSeparatorStyle(); } ));
			list.Add(new StyleInfo(FilterBoxStyleName, delegate() { return new FilterBoxStyle(); } ));
			list.Add(new StyleInfo(FilterBoxEditStyleName, delegate() { return new FilterBoxEditorStyle(); } ));
			list.Add(new StyleInfo(FilterBoxInfoTextStyleName, delegate() { return new FilterBoxInfoTextStyle(); } ));
			list.Add(new StyleInfo(GroupHeaderStyleName, delegate() { return new TitleIndexGroupHeaderStyle(); }));
			list.Add(new StyleInfo(GroupHeaderTextStyleName, delegate() { return new TitleIndexGroupHeaderStyle(); }));
			list.Add(new StyleInfo(GroupContentStyleName, delegate() { return new TitleIndexGroupContentStyle(); }));
			list.Add(new StyleInfo(ItemStyleName, delegate() { return new TitleIndexItemStyle(); } ));
		}
		protected internal virtual BackToTopStyle GetDefaultBackToTopStyle() {
			BackToTopStyle style = new BackToTopStyle();
			style.CopyFrom(CreateStyleByName("BackToTopStyle"));
			if(SkinOwner.IsRightToLeft())
				style.CopyFrom(CreateStyleByName("BackToTopRtl"));
			style.HorizontalAlign = SkinOwner.IsRightToLeft() ? HorizontalAlign.Right : HorizontalAlign.Left;
			style.ImageSpacing = GetImageSpacing();
			return style;
		}
		protected internal virtual ColumnStyle GetDefaultColumnStyle() {
			ColumnStyle style = new ColumnStyle();
			if(SkinOwner.IsRightToLeft())
				style.Paddings.PaddingRight = 20;
			else
				style.Paddings.PaddingLeft = 20;
			return style;   
		}
		protected internal virtual ColumnSeparatorStyle GetDefaultColumnSeparatorStyle() {
			ColumnSeparatorStyle style = new ColumnSeparatorStyle();
			style.Paddings.Assign(GetColumnSeparatorPaddings());
			style.Width = Unit.Pixel(1);
			return style;
		}
		protected internal virtual FilterBoxStyle GetDefaultFilterBoxStyle() {
			FilterBoxStyle style = new FilterBoxStyle();
			style.CopyFrom(CreateStyleByName("FilterBoxStyle"));
			return style;
		}
		protected internal virtual FilterBoxEditorStyle GetDefaultFilterBoxEditStyle() {
			FilterBoxEditorStyle style = new FilterBoxEditorStyle();
			style.CopyFrom(CreateStyleByName("FilterBoxEditStyle"));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultFilterBoxInfoTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("FilterBoxInfoTextStyle"));
			return style;
		}
		protected internal virtual TitleIndexGroupContentStyle GetDefaultGroupContentStyle(bool isCategorized) {
			TitleIndexGroupContentStyle style = new TitleIndexGroupContentStyle();
			style.Paddings.Assign(GetGroupContentPaddings(isCategorized));
			return style;
		}
		protected internal virtual TitleIndexGroupHeaderStyle GetDefaultGroupHeaderStyle(bool isCategorized) {
			TitleIndexGroupHeaderStyle style = new TitleIndexGroupHeaderStyle();
			string cssNameStyle = "GroupHeaderStyle";
			cssNameStyle += isCategorized ? "Categorized" : "";
			style.CopyFrom(CreateStyleByName(cssNameStyle));
			return style;
		}
		protected internal TitleIndexGroupHeaderStyle GetDefaultGroupHeaderTextStyle(bool isCategorized) {
			TitleIndexGroupHeaderStyle style = new TitleIndexGroupHeaderStyle();
			string cssNameStyle = "GroupHeaderTextStyle";
			cssNameStyle += isCategorized ? "Categorized" : "";
			style.CopyFrom(CreateStyleByName(cssNameStyle));
			return style;
		}
		protected internal virtual TitleIndexItemStyle GetDefaultItemStyle() {
			TitleIndexItemStyle style = new TitleIndexItemStyle();
			style.CopyFrom(CreateStyleByName("ItemStyle"));
			style.Spacing = GetItemSpacing();
			return style;
		}
		protected internal IndexPanelStyle GetDefaultIndexPanelStyle(FilterBoxVerticalPosition vertPosition, bool isFilterBoxVisible) {
			IndexPanelStyle style = new IndexPanelStyle();
			style.CopyFrom(CreateStyleByName("IndexPanelStyle"));
			style.HorizontalAlign = HorizontalAlign.Center;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCurrentIndexPanelItemStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("CurrentIndexPanelItemStyle"));
			return style;
		}
		protected internal IndexPanelItemStyle GetDefaultIndexPanelItemStyle() {
			IndexPanelItemStyle style = new IndexPanelItemStyle();
			style.DisabledForeColor = Color.FromArgb(0xC0, 0xC0, 0xC0);
			style.CopyFrom(CreateStyleByName("IndexPanelItemStyle"));
			return style;
		}
		protected internal IndexPanelSeparatorStyle GetDefaultIndexPanelSeparatorStyle() {
			IndexPanelSeparatorStyle style = new IndexPanelSeparatorStyle();
			style.Height = Unit.Pixel(2);
			style.BackColor = Color.FromArgb(0xB4, 0xB4, 0xB4);
			return style;
		}
		public Paddings GetColumnSeparatorPaddings() {
			return new Paddings(Unit.Pixel(16), 0, Unit.Pixel(16), 0);
		}
		protected internal Unit GetFilterBoxSpacing() {
			return Unit.Pixel(25);
		}
		protected internal Unit GetGroupSpacing() {
			return Unit.Pixel(19);
		}
		protected Paddings GetGroupContentPaddings(bool isCategorized) {
			Unit sidePadding = isCategorized ? 2 : Unit.Empty;
			return new Paddings(
				SkinOwner.IsRightToLeft() ? Unit.Empty : sidePadding, 
				isCategorized ? 5 : 3,
				SkinOwner.IsRightToLeft() ? sidePadding : Unit.Empty, 
				8);
		}
		protected internal Unit GetIndexPanelSpacing(FilterBoxVerticalPosition vertPosition, bool isFilterBoxVisible) {
			if(!isFilterBoxVisible || (vertPosition == FilterBoxVerticalPosition.AboveIndexPanel))
				return Unit.Pixel(25);
			else
				return Unit.Empty;
		}
		protected Unit GetItemSpacing() {
			return Unit.Pixel(1);
		}
		public override Unit GetBulletIndent() {
			if(Browser.IsOpera) {
				if(Browser.MajorVersion == 8)
					return new Unit(12, UnitType.Pixel);
				else
					return new Unit(-29, UnitType.Pixel);
			} else
				if(Browser.Family.IsNetscape)
					return new Unit(-27, UnitType.Pixel);
				else
					if(Browser.Family.IsWebKit)
						return new Unit(-25, UnitType.Pixel);
					else
						return new Unit(16, UnitType.Pixel);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { IndexPanelItemLink });
		}
	}
}
