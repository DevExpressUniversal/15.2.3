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
	public class GridViewStyleBase : GridStyleBase { }
	public class GridStyleBase : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class GridViewTableStyle : GridViewStyleBase { }
	public class GridViewCustomizationStyle : GridViewStyleBase { }
	public class GridViewPopupEditFormStyle : GridViewStyleBase { }
	public class GridViewHeaderPanelStyle : GridViewStyleBase { }
	public class GridViewCellStyle : GridViewStyleBase { }
	public class GridViewFooterStyle : GridViewStyleBase { }
	public class GridViewGroupFooterStyle : GridViewStyleBase { }
	public class GridViewEditCellStyle : GridViewStyleBase { }
	public class GridViewFilterCellStyle : GridViewStyleBase { }
	public class GridViewInlineEditRowStyle : GridViewStyleBase { }
	public class GridViewEditFormStyle : GridViewStyleBase { }
	public class GridViewEditFormCaptionStyle : GridViewStyleBase { }
	public class GridViewTitleStyle : GridViewStyleBase { }
	public class GridViewStatusBarStyle : GridViewStyleBase { }
	public class GridViewFilterBarStyle : GridViewStyleBase { }
	public class GridViewEditFormTableStyle : GridViewStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings { get { return base.Paddings; } }
	}
	public class GridViewGroupPanelStyle : GridViewStyleBase {
	}
	public class GridViewCommandColumnStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class GridViewHeaderStyle : GridHeaderStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderStyleFilterImageSpacing"),
#endif
 DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit FilterImageSpacing { get { return base.FilterImageSpacing; } set { base.FilterImageSpacing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderStyleSortingImageSpacing"),
#endif
 DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit SortingImageSpacing { get { return base.SortingImageSpacing; } set { base.SortingImageSpacing = value; } }
	}
	public abstract class GridHeaderStyle : AppearanceStyle {
		protected internal Unit FilterImageSpacing { get { return Spacing; } set { Spacing = value; } }
		protected internal Unit SortingImageSpacing { get { return ImageSpacing; } set { ImageSpacing = value; } }
		protected internal Unit GetFilterImageSpacing() {
			return FilterImageSpacing.IsEmpty ? 5 : FilterImageSpacing;
		}
		protected internal Unit GetSortingImageSpacing() {
			return SortingImageSpacing.IsEmpty ? 5 : SortingImageSpacing;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class GridViewAlternatingRowStyle : GridViewDataRowStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewAlternatingRowStyleEnabled"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatEnable]
		public DefaultBoolean Enabled {
			get { return (DefaultBoolean)ViewStateUtils.GetIntProperty(ViewState, "Enabled", (int)DefaultBoolean.Default); }
			set {
				ViewStateUtils.SetIntProperty(ViewState, "Enabled", (int)DefaultBoolean.Default, (int)value);
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			GridViewAlternatingRowStyle altStyle = style as GridViewAlternatingRowStyle;
			if(altStyle != null) Enabled = altStyle.Enabled;
		}
	}
	public class GridViewRowStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings { get { return base.Paddings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
	}
	public class GridViewGroupRowStyle : GridViewRowStyle {}
	public class GridViewDataRowStyle : GridViewRowStyle { }
	public class GridViewPopupMainAreaStyle : GridViewStyleBase { }
	public class GridViewSearchPanelStyle : GridViewStyleBase { }
	public class GridViewStyles : GridStyles {
		public const string AdaptiveHeaderPanelStyleName = "AdaptiveHeaderPanel";
		public const string AdaptiveFooterPanelStyleName = "AdaptiveFooterPanel";
		public const string GroupRowStyleName = "GroupRow";
		public const string FocusedGroupRowStyleName = "FocusedGroupRow";
		public const string DetailRowStyleName = "DetailRow";
		public const string DetailCellStyleName = "DetailCell";
		public const string PreviewRowStyleName = "PreviewRow";
		public const string EmptyDataRowStyleName = "EmptyDataRow";
		public const string DataRowStyleName = "DataRow";
		public const string DataRowHoverStyleName = "DataRowHover";
		public const string DataRowAltStyleName = "DataRowAlt";
		public const string SelectedRowStyleName = "SelectedRow";
		public const string FocusedRowStyleName = "FocusedRow";
		public const string FilterRowStyleName = "FilterRow";
		public const string CellStyleName = "Cell";
		public const string FooterStyleName = "Footer";
		public const string GroupFooterStyleName = "GroupFooter";
		public const string GroupPanelStyleName = "GroupPanel";
		public const string HeaderPanelStyleName = "HeaderPanel";
		public const string DetailButtonStyleName = "DetailButton";
		public const string CommandColumnStyleName = "CommandColumn";
		public const string CommandColumnItemStyleName = "CommandColumnItem";
		public const string FixedColumnStyleName = "FixedColumn";
		public const string FilterCellStyleName = "FilterCell";
		public const string InlineEditRowStyleName = "InlineEditRow";
		public const string EditFormStyleName = "EditForm";
		public const string EditFormDisplayRowStyleName = "EditFormDisplayRow";
		public const string EditingErrorRowStyleName = "EditingErrorRow";
		public const string EditFormTableStyleName = "EditFormTable";
		public const string EditFormCaptionStyleName = "EditFormCaption";
		public const string InlineEditCellStyleName = "InlineEditCell";
		public const string EditFormCellStyleName = "EditFormCell";
		public const string ContextMenuStyleName = "ContextMenu";
		public const string
			FilterRowMenuStyleName = "FilterRowMenu",
			FilterRowMenuItemStyleName = "FilterRowMenuItem";
		protected internal const string GridPrefix = "dxgv";		
		protected internal const string GridIndentCellCssClass = GridPrefix + "IndentCell";
		protected internal const string GridDetailIndentCellCssClass = GridPrefix + "DIC";
		protected internal const string GridAdaptiveIndentCellCssClass = GridPrefix + "AIC";
		protected internal const string GridAdaptiveDetailRowCssClass = GridPrefix + "ADR";
		protected internal const string GridAdaptiveDetailCaptionCellCssClass = GridPrefix + "ADCC";
		protected internal const string GridAdaptiveDetailDataCellCssClass = GridPrefix + "ADDC";
		protected internal const string GridAdaptiveDetailCommandCellCssClass = GridPrefix + "ADCMDC";
		protected internal const string GridAdaptiveDetailLayoutItemContentCssClass = GridPrefix + "ADLIC";
		protected internal const string GridAdaptiveHeaderCssClass = GridPrefix + "ADH";
		protected internal const string GridAdaptiveHeaderTableRowCssClass = GridPrefix + "ADHTR";
		protected internal const string GridAdaptiveFooterSummaryDivCssClass = GridPrefix + "ADFSD";
		public GridViewStyles(ISkinOwner grid)
			: base(grid) {
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(HeaderStyleName, () => new GridViewHeaderStyle()));
			list.Add(new StyleInfo(TitlePanelStyleName, () => new GridViewTitleStyle()));
			list.Add(new StyleInfo(StatusBarStyleName, () => new GridViewStatusBarStyle()));
			list.Add(new StyleInfo(FilterBarStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(PagerTopPanelStyleName, () => new GridViewCellStyle()));
			list.Add(new StyleInfo(PagerBottomPanelStyleName, () => new GridViewCellStyle()));
			list.Add(new StyleInfo(SearchPanelStyleName, () => new GridViewSearchPanelStyle()));
			list.Add(new StyleInfo(FilterBarLinkStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(FilterBarCheckBoxCellStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(FilterBarImageCellStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(FilterBarExpressionCellStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(FilterBarClearButtonCellStyleName, () => new GridViewFilterBarStyle()));
			list.Add(new StyleInfo(TableStyleName, () => new GridViewTableStyle()));
			list.Add(new StyleInfo(GroupRowStyleName, delegate() { return new GridViewGroupRowStyle(); } ));
			list.Add(new StyleInfo(FocusedGroupRowStyleName, delegate() { return new GridViewGroupRowStyle(); } ));
			list.Add(new StyleInfo(DataRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(DataRowHoverStyleName, delegate() { return new GridViewDataRowStyle(); }));
			list.Add(new StyleInfo(DetailRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(DetailCellStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(PreviewRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(EmptyDataRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(DataRowAltStyleName, delegate() { return new GridViewAlternatingRowStyle(); } ));
			list.Add(new StyleInfo(SelectedRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(FocusedRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(FilterRowStyleName, delegate() { return new GridViewRowStyle(); } ));
			list.Add(new StyleInfo(CellStyleName, delegate() { return new GridViewCellStyle(); } ));
			list.Add(new StyleInfo(FooterStyleName, delegate() { return new GridViewFooterStyle(); } ));
			list.Add(new StyleInfo(GroupFooterStyleName, delegate() { return new GridViewGroupFooterStyle(); } ));
			list.Add(new StyleInfo(GroupPanelStyleName, delegate() { return new GridViewGroupPanelStyle(); } ));
			list.Add(new StyleInfo(HeaderPanelStyleName, delegate() { return new GridViewHeaderPanelStyle(); } ));
			list.Add(new StyleInfo(DetailButtonStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(CommandColumnStyleName, delegate() { return new GridViewCommandColumnStyle(); } ));
			list.Add(new StyleInfo(CommandColumnItemStyleName, delegate() { return new GridViewCommandColumnStyle(); } ));
			list.Add(new StyleInfo(FixedColumnStyleName, delegate() { return new GridViewStyleBase(); }));
			list.Add(new StyleInfo(InlineEditCellStyleName, delegate() { return new GridViewEditCellStyle(); } ));
			list.Add(new StyleInfo(FilterCellStyleName, delegate() { return new GridViewFilterCellStyle(); } ));
			list.Add(new StyleInfo(InlineEditRowStyleName, delegate() { return new GridViewInlineEditRowStyle(); } ));
			list.Add(new StyleInfo(EditFormDisplayRowStyleName, delegate() { return new GridViewDataRowStyle(); } ));
			list.Add(new StyleInfo(EditingErrorRowStyleName, delegate() { return new GridViewRowStyle(); } ));
			list.Add(new StyleInfo(EditFormStyleName, delegate() { return new GridViewEditFormStyle(); } ));
			list.Add(new StyleInfo(EditFormCellStyleName, delegate() { return new GridViewEditCellStyle(); } ));
			list.Add(new StyleInfo(EditFormTableStyleName, delegate() { return new GridViewEditFormTableStyle(); } ));
			list.Add(new StyleInfo(EditFormCaptionStyleName, delegate() { return new GridViewEditFormCaptionStyle(); } ));
			list.Add(new StyleInfo(FilterRowMenuStyleName, delegate() { return new DevExpress.Web.MenuStyle(); }));
			list.Add(new StyleInfo(FilterRowMenuItemStyleName, delegate() { return new DevExpress.Web.MenuItemStyle(); }));
			list.Add(new StyleInfo(BatchEditCellStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(BatchEditModifiedCellStyleName, delegate() { return new GridViewCellStyle(); }));
			list.Add(new StyleInfo(AdaptiveHeaderPanelStyleName, delegate() { return new GridViewStyleBase(); }));
			list.Add(new StyleInfo(AdaptiveFooterPanelStyleName, delegate() { return new GridViewStyleBase(); }));
		}
		protected internal override string GetCssClassNamePrefix() {
			return GridPrefix;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesGroupButtonWidth"),
#endif
		DefaultValue(15), NotifyParentProperty(true), AutoFormatEnable]
		public int GroupButtonWidth {
			get { return GetIntProperty("GroupButtonWidth", 15); }
			set { SetIntProperty("GroupButtonWidth", 15, value); }
		}
		[
		DefaultValue(20), NotifyParentProperty(true), AutoFormatEnable]
		public int AdaptiveDetailButtonWidth {
			get { return GetIntProperty("AdaptiveDetailButtonWidth", 22); }
			set { SetIntProperty("AdaptiveDetailButtonWidth", 22, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesDisabled"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled { get { return base.DisabledInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesTable"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewTableStyle Table { get { return (GridViewTableStyle)base.Table; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewHeaderStyle Header {
			get { return (GridViewHeaderStyle)base.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesGroupRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupRowStyle GroupRow {
			get { return (GridViewGroupRowStyle)GetStyle(GroupRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFocusedGroupRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupRowStyle FocusedGroupRow {
			get { return (GridViewGroupRowStyle)GetStyle(FocusedGroupRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle Row {
			get { return (GridViewDataRowStyle)GetStyle(DataRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesRowHotTrack"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle RowHotTrack {
			get { return (GridViewDataRowStyle)GetStyle(DataRowHoverStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesDetailRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle DetailRow {
			get { return (GridViewDataRowStyle)GetStyle(DetailRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesDetailCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle DetailCell {
			get { return (GridViewCellStyle)GetStyle(DetailCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesPreviewRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle PreviewRow {
			get { return (GridViewDataRowStyle)GetStyle(PreviewRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEmptyDataRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle EmptyDataRow {
			get { return (GridViewDataRowStyle)GetStyle(EmptyDataRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesAlternatingRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewAlternatingRowStyle AlternatingRow {
			get { return (GridViewAlternatingRowStyle)GetStyle(DataRowAltStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesSelectedRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle SelectedRow {
			get { return (GridViewDataRowStyle)GetStyle(SelectedRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFocusedRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle FocusedRow {
			get { return (GridViewDataRowStyle)GetStyle(FocusedRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle FilterRow {
			get { return (GridViewRowStyle)GetStyle(FilterRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle Cell {
			get { return (GridViewCellStyle)GetStyle(CellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFooterStyle Footer {
			get { return (GridViewFooterStyle)GetStyle(FooterStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesGroupFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupFooterStyle GroupFooter {
			get { return (GridViewGroupFooterStyle)GetStyle(GroupFooterStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesGroupPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupPanelStyle GroupPanel {
			get { return (GridViewGroupPanelStyle)GetStyle(GroupPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesHeaderPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewHeaderPanelStyle HeaderPanel {
			get { return (GridViewHeaderPanelStyle)GetStyle(HeaderPanelStyleName); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStyleBase AdaptiveHeaderPanel {
			get { return (GridViewStyleBase)GetStyle(AdaptiveHeaderPanelStyleName); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStyleBase AdaptiveFooterPanel {
			get { return (GridViewStyleBase)GetStyle(AdaptiveFooterPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesPagerTopPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCellStyle PagerTopPanel { get { return (GridViewCellStyle)base.PagerTopPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesPagerBottomPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCellStyle PagerBottomPanel { get { return (GridViewCellStyle)base.PagerBottomPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesDetailButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle DetailButton {
			get { return (GridViewCellStyle)GetStyle(DetailButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesLoadingPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel { get { return base.LoadingPanelInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesLoadingDiv"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv { get { return base.LoadingDivInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesCommandColumn"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandColumnStyle CommandColumn {
			get { return (GridViewCommandColumnStyle)GetStyle(CommandColumnStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesCommandColumnItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandColumnStyle CommandColumnItem {
			get { return (GridViewCommandColumnStyle)GetStyle(CommandColumnItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFixedColumn"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewStyleBase FixedColumn {
			get { return (GridViewStyleBase)GetStyle(FixedColumnStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesInlineEditCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditCellStyle InlineEditCell {
			get { return (GridViewEditCellStyle)GetStyle(InlineEditCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFilterCellStyle FilterCell {
			get { return (GridViewFilterCellStyle)GetStyle(FilterCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesInlineEditRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewInlineEditRowStyle InlineEditRow {
			get { return (GridViewInlineEditRowStyle)GetStyle(InlineEditRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditFormDisplayRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewDataRowStyle EditFormDisplayRow {
			get { return (GridViewDataRowStyle)GetStyle(EditFormDisplayRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditingErrorRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle EditingErrorRow {
			get { return (GridViewRowStyle)GetStyle(EditingErrorRowStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditForm"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormStyle EditForm {
			get { return (GridViewEditFormStyle)GetStyle(EditFormStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditFormCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditCellStyle EditFormCell {
			get { return (GridViewEditCellStyle)GetStyle(EditFormCellStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditFormTable"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormTableStyle EditFormTable {
			get { return (GridViewEditFormTableStyle)GetStyle(EditFormTableStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesEditFormColumnCaption"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewEditFormCaptionStyle EditFormColumnCaption {
			get { return (GridViewEditFormCaptionStyle)GetStyle(EditFormCaptionStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesTitlePanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewTitleStyle TitlePanel { get { return (GridViewTitleStyle)base.TitlePanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesStatusBar"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewStatusBarStyle StatusBar { get { return (GridViewStatusBarStyle)base.StatusBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBar"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBar { get { return (GridViewFilterBarStyle)base.FilterBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarLink"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBarLink { get { return (GridViewFilterBarStyle)base.FilterBarLink; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarCheckBoxCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBarCheckBoxCell { get { return (GridViewFilterBarStyle)base.FilterBarCheckBoxCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarImageCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBarImageCell { get { return (GridViewFilterBarStyle)base.FilterBarImageCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarExpressionCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBarExpressionCell { get { return (GridViewFilterBarStyle)base.FilterBarExpressionCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarClearButtonCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBarStyle FilterBarClearButtonCell { get { return (GridViewFilterBarStyle)base.FilterBarClearButtonCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesHeaderFilterItem"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ListBoxItemStyle HeaderFilterItem { get { return base.HeaderFilterItem; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterRowMenu"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Web.MenuStyle FilterRowMenu {
			get { return (DevExpress.Web.MenuStyle)GetStyle(FilterRowMenuStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterRowMenuItem"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Web.MenuItemStyle FilterRowMenuItem {
			get { return (DevExpress.Web.MenuItemStyle)GetStyle(FilterRowMenuItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesBatchEditCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCellStyle BatchEditCell { get { return (GridViewCellStyle)base.BatchEditCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesBatchEditModifiedCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCellStyle BatchEditModifiedCell { get { return (GridViewCellStyle)base.BatchEditModifiedCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesSearchPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewSearchPanelStyle SearchPanel { get { return (GridViewSearchPanelStyle)base.SearchPanel; } }
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			GridViewStyles styles = source as GridViewStyles;
			if(styles != null) {
				GroupButtonWidth = styles.GroupButtonWidth;
				AdaptiveDetailButtonWidth = styles.AdaptiveDetailButtonWidth;
			}
		}
		internal AppearanceStyleBase GetStyleInternal(string styleName) {
			return GetStyle(styleName);
		}
	}
	public abstract class GridStyles : StylesBase {
		protected const string
			GridClassNameFormat = "{0}{1}",
			HFSelectAllCellClassNamePostfix = "HFSAC",
			HFSeparatorCellClassNamePostfix = "HFSC",
			HFListBoxCellClassNamePostfix = "HFLC",
			HFDRCalendarClassNamePostfix = "HFDRC",
			HFDRDateRangePickerClassNamePostfix = "HFDRP",
			MSTouchDraggableMarkerCssClassNamePostfix = "MSDraggable",
			BatchEditErrorCellClassNamePostfix = "ErrorCell",
			SearchPanelHighlightTextClassNamePostfix = "HL",
			DisabledCheckboxClassNamePostfix = "_cd";
		public const string
			ControlStyleName = "Control",
			TableStyleName = "Table",
			TitlePanelStyleName = "TitlePanel",
			SearchPanelStyleName = "SearchPanel",
			StatusBarStyleName = "StatusBar",
			FilterBarStyleName = "FilterBar",
			PagerTopPanelStyleName = "PagerTopPanel",
			PagerBottomPanelStyleName = "PagerBottomPanel",
			FilterBarLinkStyleName = "FilterBarLink",
			FilterBarCheckBoxCellStyleName = "FilterBarCheckBoxCell",
			FilterBarImageCellStyleName = "FilterBarImageCell",
			FilterBarExpressionCellStyleName = "FilterBarExpressionCell",
			FilterBarClearButtonCellStyleName = "FilterBarClearButtonCell",
			FilterBuilderMainAreaStyleName = "FilterBuilderMainArea",
			FilterBuilderButtonAreaStyleName = "FilterBuilderButtonArea",
			CustomizationStyleName = "Customization",
			PopupEditFormStyleName = "PopupEditForm",
			BatchEditCellStyleName = "BatchEditCell",
			BatchEditModifiedCellStyleName = "BatchEditModifiedCell",
			StatusBarLoadingPanelStyleName = "LoadingPanelStatusBar",
			HeaderStyleName = "Header",
			HeaderFilterItemStyleName = "HeaderFilterItemStyle";
		public GridStyles(ISkinOwner grid)
			: base(grid) {
		}
		protected internal string HFSelectAllCellClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), HFSelectAllCellClassNamePostfix); } }
		protected internal string HFSeparatorCellClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), HFSeparatorCellClassNamePostfix); } }
		protected internal string HFListBoxCellClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), HFListBoxCellClassNamePostfix); } }
		protected internal string HFDRCalendarClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), HFDRCalendarClassNamePostfix); } }
		protected internal string HFDRDateRangePickerClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), HFDRDateRangePickerClassNamePostfix); } }
		protected internal string MSTouchDraggableMarkerCssClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), MSTouchDraggableMarkerCssClassNamePostfix); } }
		protected internal string BatchEditErrorCellClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), BatchEditErrorCellClassNamePostfix); } }
		protected internal string SearchPanelHighlightTextClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), SearchPanelHighlightTextClassNamePostfix); } }
		protected internal string DisabledCheckboxClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), DisabledCheckboxClassNamePostfix); } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(HeaderFilterItemStyleName, delegate() { return new ListBoxItemStyle(); }));
		}
		protected internal AppearanceStyle Header { get { return (AppearanceStyle)GetStyle(HeaderStyleName); } }
		protected internal AppearanceStyle SearchPanel { get { return (AppearanceStyle)GetStyle(SearchPanelStyleName); } }
		protected internal AppearanceStyle TitlePanel { get { return (AppearanceStyle)GetStyle(TitlePanelStyleName); } }
		protected internal AppearanceStyle StatusBar { get { return (AppearanceStyle)GetStyle(StatusBarStyleName); } }
		protected internal AppearanceStyle FilterBar { get { return (AppearanceStyle)GetStyle(FilterBarStyleName); } }
		protected internal AppearanceStyle PagerTopPanel { get { return (AppearanceStyle)GetStyle(PagerTopPanelStyleName); } }
		protected internal AppearanceStyle PagerBottomPanel { get { return (AppearanceStyle)GetStyle(PagerBottomPanelStyleName); } }
		protected internal AppearanceStyle Table { get { return (AppearanceStyle)GetStyle(TableStyleName); } }
		protected internal AppearanceStyle FilterBarLink { get { return (AppearanceStyle)GetStyle(FilterBarLinkStyleName); } }
		protected internal AppearanceStyle FilterBarCheckBoxCell { get { return (AppearanceStyle)GetStyle(FilterBarCheckBoxCellStyleName); } }
		protected internal AppearanceStyle FilterBarImageCell { get { return (AppearanceStyle)GetStyle(FilterBarImageCellStyleName); } }
		protected internal AppearanceStyle FilterBarExpressionCell { get { return (AppearanceStyle)GetStyle(FilterBarExpressionCellStyleName); } }
		protected internal AppearanceStyle FilterBarClearButtonCell { get { return (AppearanceStyle)GetStyle(FilterBarClearButtonCellStyleName); } }
		protected internal ListBoxItemStyle HeaderFilterItem { get { return (ListBoxItemStyle)GetStyle(HeaderFilterItemStyleName); } }
		protected internal AppearanceStyle BatchEditCell { get { return (AppearanceStyle)GetStyle(BatchEditCellStyleName); } }
		protected internal AppearanceStyle BatchEditModifiedCell { get { return (AppearanceStyle)GetStyle(BatchEditModifiedCellStyleName); } }
	}
	public class GridViewPagerStyles : PagerStyles {
		public GridViewPagerStyles(ASPxGridView grid) : base(grid) { }
		public override string ToString() { return string.Empty; }
	}
	public class GridViewEditorStyles : EditorStyles {
		public GridViewEditorStyles(ASPxGridView grid) : base(grid) { }
		public override string ToString() { return string.Empty; }
	}
	public class GridPopupControlStyle : PopupControlStyles {
		protected const string MainAreaStyleName = "MainArea";
		public GridPopupControlStyle(ASPxGridBase grid)
			: base(grid) {
		}
		protected AppearanceStyle PopupControl { get { return Style; } }
		protected internal AppearanceStyle MainArea { get { return (AppearanceStyle)GetStyle(MainAreaStyleName); } }
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LinkStyle Link { get { return base.Link; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingDivStyle LoadingDiv { get { return base.LoadingDiv; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingPanelStyle LoadingPanel { get { return base.LoadingPanel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DisabledStyle Disabled { get { return base.Disabled; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyle Style { get { return base.Style; } }
		#endregion
	}
	public class GridFilterBuilderPopupStyle : GridPopupControlStyle {
		const string ButtonPanelStyleName = "ButtonPanel";
		public GridFilterBuilderPopupStyle(ASPxGridBase grid)
			: base(grid) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowFooterStyle Footer { get { return base.Footer; } }
		protected internal AppearanceStyle ButtonPanel { get { return (AppearanceStyle)GetStyle(ButtonPanelStyleName); } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonPanelStyleName, delegate() { return new AppearanceStyle(); }));
		}
	}
	public class GridHeaderFilterPopupStyle : GridPopupControlStyle {
		HeaderFilterButtonPanelStyles buttonPanel;
		public GridHeaderFilterPopupStyle(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal HeaderFilterButtonPanelStyles ButtonPanel {
			get {
				if(buttonPanel == null)
					buttonPanel = new HeaderFilterButtonPanelStyles(SkinOwner);
				return buttonPanel;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowStyle Header { get { return base.Header; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle CloseButton { get { return base.CloseButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupControlModalBackgroundStyle ModalBackground { get { return base.ModalBackground; } }
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			GridHeaderFilterPopupStyle style = source as GridHeaderFilterPopupStyle;
			if(style != null)
				ButtonPanel.CopyFrom(style.ButtonPanel);
		}
		public override void Reset() {
			base.Reset();
			ButtonPanel.Reset();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(ButtonPanel);
			return list.ToArray();
		}
	}
	public class GridViewPopupControlStyle : GridPopupControlStyle {
		public GridViewPopupControlStyle(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylePopupControl"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle PopupControl { get { return base.PopupControl; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new GridViewPopupMainAreaStyle(); }));
		}
	}
	public class GridViewEditFormPopupStyle : GridPopupControlStyle {
		public GridViewEditFormPopupStyle(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylePopupControl"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle PopupControl { get { return base.PopupControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupStyleMainArea"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPopupMainAreaStyle MainArea { get { return base.MainArea as GridViewPopupMainAreaStyle; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new GridViewPopupMainAreaStyle(); }));
		}
	}
	public class GridViewCustomizationWindowPopupStyle : GridPopupControlStyle {
		public GridViewCustomizationWindowPopupStyle(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylePopupControl"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle PopupControl { get { return base.PopupControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupStyleMainArea"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPopupMainAreaStyle MainArea { get { return base.MainArea as GridViewPopupMainAreaStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowFooterStyle Footer { get { return base.Footer; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupControlModalBackgroundStyle ModalBackground { get { return base.ModalBackground; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new GridViewPopupMainAreaStyle(); }));
		}
	}
	public class GridViewFilterBuilderPopupStyle : GridFilterBuilderPopupStyle {
		public GridViewFilterBuilderPopupStyle(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylePopupControl"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle PopupControl { get { return base.PopupControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFilterBuilderPopupStyleMainArea"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPopupMainAreaStyle MainArea { get { return base.MainArea as GridViewPopupMainAreaStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFilterBuilderPopupStyleButtonPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle ButtonPanel { get { return base.ButtonPanel; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new GridViewPopupMainAreaStyle(); }));
		}
	}
	public class GridViewHeaderFilterPopupStyle : GridHeaderFilterPopupStyle {
		public GridViewHeaderFilterPopupStyle(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylePopupControl"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle PopupControl { get { return base.PopupControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupStyleButtonPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new HeaderFilterButtonPanelStyles ButtonPanel { get { return base.ButtonPanel; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new GridViewPopupMainAreaStyle(); }));
		}
	}
	public class GridViewContextMenuStyles : PropertiesBase {
		GridViewContextMenuStyle common;
		GridViewContextMenuStyle groupPanel;
		GridViewContextMenuStyle column;
		GridViewContextMenuStyle row;
		GridViewContextMenuStyle footer;
		public GridViewContextMenuStyles(ASPxGridView grid) 
			:base(grid) {
		}
		protected ASPxGridView Grid { get { return (ASPxGridView)Owner; } }
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyle Common {
			get {
				if(common == null)
					common = new GridViewContextMenuStyle(Grid);
				return common; 
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyle GroupPanel {
			get {
				if(groupPanel == null)
					groupPanel = new GridViewContextMenuStyle(Grid);
				return groupPanel;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyle Column {
			get {
				if(column == null)
					column = new GridViewContextMenuStyle(Grid);
				return column;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyle Row {
			get {
				if(row == null)
					row = new GridViewContextMenuStyle(Grid);
				return row;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewContextMenuStyle Footer {
			get {
				if(footer == null)
					footer = new GridViewContextMenuStyle(Grid);
				return footer;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Common);
			list.Add(GroupPanel);
			list.Add(Column);
			list.Add(Row);
			list.Add(Footer);
			return list.ToArray();
		}
		public void Assign(GridViewContextMenuStyles source) {
			Reset();
			CopyFrom(source);
		}
		public virtual void CopyFrom(GridViewContextMenuStyles source) {
			Common.CopyFrom(source.Common);
			GroupPanel.CopyFrom(source.GroupPanel);
			Column.CopyFrom(source.Column);
			Row.CopyFrom(source.Row);
			Footer.CopyFrom(source.Footer);
		}
		public virtual void Reset() {
			Common.Reset();
			GroupPanel.Reset();
			Column.Reset();
			Row.Reset();
			Footer.Reset();
		}
	}
	public class GridViewContextMenuStyle : MenuStyles {
		public GridViewContextMenuStyle(ASPxGridView grid)
			: base(grid) {
		}
	}
	public abstract class GridPopupControlStyles : PropertiesBase {
		GridPopupControlStyle common;
		GridPopupControlStyle editForm;
		GridPopupControlStyle customizationWindow;
		GridFilterBuilderPopupStyle filterBuilder;
		GridHeaderFilterPopupStyle headerFilter;
		public GridPopupControlStyles(ASPxGridBase grid)
			: base(grid) {
		}
		protected ASPxGridBase Grid { get { return (ASPxGridBase)Owner; } }
		protected internal GridPopupControlStyle Common {
			get {
				if(common == null)
					common = CreateCommonStyle();
				return common;
			}
		}
		protected internal GridPopupControlStyle EditForm {
			get {
				if(editForm == null)
					editForm = CreateEditFormStyle();
				return editForm;
			}
		}
		protected internal GridPopupControlStyle CustomizationWindow {
			get {
				if(customizationWindow == null)
					customizationWindow = CreateCustomizationWindowStyle();
				return customizationWindow;
			}
		}
		protected internal GridFilterBuilderPopupStyle FilterBuilder {
			get {
				if(filterBuilder == null)
					filterBuilder = CreateFilterBuilderStyle();
				return filterBuilder;
			}
		}
		protected internal GridHeaderFilterPopupStyle HeaderFilter {
			get {
				if(headerFilter == null)
					headerFilter = CreateHeaderFilterStyle();
				return headerFilter;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Common);
			list.Add(EditForm);
			list.Add(CustomizationWindow);
			list.Add(FilterBuilder);
			list.Add(HeaderFilter);
			return list.ToArray();
		}
		protected abstract GridPopupControlStyle CreateCommonStyle();
		protected abstract GridPopupControlStyle CreateEditFormStyle();
		protected abstract GridPopupControlStyle CreateCustomizationWindowStyle();
		protected abstract GridFilterBuilderPopupStyle CreateFilterBuilderStyle();
		protected abstract GridHeaderFilterPopupStyle CreateHeaderFilterStyle();
		public void Assign(GridPopupControlStyles source) {
			Reset();
			CopyFrom(source);
		}
		public virtual void CopyFrom(GridPopupControlStyles source) {
			Common.CopyFrom(source.Common);
			EditForm.CopyFrom(source.EditForm);
			CustomizationWindow.CopyFrom(source.CustomizationWindow);
			FilterBuilder.CopyFrom(source.FilterBuilder);
			HeaderFilter.CopyFrom(source.HeaderFilter);
		}
		public virtual void Reset() {
			Common.Reset();
			EditForm.Reset();
			CustomizationWindow.Reset();
			FilterBuilder.Reset();
			HeaderFilter.Reset();
		}
	}
	public class GridViewPopupControlStyles : GridPopupControlStyles {
		public GridViewPopupControlStyles(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesCommon"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewPopupControlStyle Common { get { return base.Common as GridViewPopupControlStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesEditForm"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewEditFormPopupStyle EditForm { get { return base.EditForm as GridViewEditFormPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesCustomizationWindow"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCustomizationWindowPopupStyle CustomizationWindow { get { return base.CustomizationWindow as GridViewCustomizationWindowPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesFilterBuilder"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewFilterBuilderPopupStyle FilterBuilder { get { return base.FilterBuilder as GridViewFilterBuilderPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesHeaderFilter"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewHeaderFilterPopupStyle HeaderFilter { get { return base.HeaderFilter as GridViewHeaderFilterPopupStyle; } }
		protected override GridPopupControlStyle CreateCommonStyle() { return new GridViewPopupControlStyle(Grid); }
		protected override GridPopupControlStyle CreateEditFormStyle() { return new GridViewEditFormPopupStyle(Grid); }
		protected override GridPopupControlStyle CreateCustomizationWindowStyle() { return new GridViewCustomizationWindowPopupStyle(Grid); }
		protected override GridFilterBuilderPopupStyle CreateFilterBuilderStyle() { return new GridViewFilterBuilderPopupStyle(Grid); }
		protected override GridHeaderFilterPopupStyle CreateHeaderFilterStyle() { return new GridViewHeaderFilterPopupStyle(Grid); }
	}
}
