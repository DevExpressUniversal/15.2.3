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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class CardViewStyleBase : GridStyleBase { }
	public class CardViewHeaderStyle : GridHeaderStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderStyleFilterImageSpacing"),
#endif
 DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit FilterImageSpacing { get { return base.FilterImageSpacing; } set { base.FilterImageSpacing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderStyleSortingImageSpacing"),
#endif
 DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit SortingImageSpacing { get { return base.SortingImageSpacing; } set { base.SortingImageSpacing = value; } }
	}
	public class CardViewCardStyle : CardViewStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCardStyleHeight"),
#endif
 Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCardStyleWidth"),
#endif
 Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
		public override void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment, bool exceptOpacity) {
			base.AssignToControl(control, range, exceptTextDecoration, useBlockAlignment, exceptOpacity);
			if(!Height.IsEmpty && control.ControlStyle.Height != Height)
				control.ControlStyle.Height = Height;
			if(!Width.IsEmpty && control.ControlStyle.Width != Width)
				control.ControlStyle.Width = Width;
		}
	}
	public class CardViewStyles : GridStyles {
		public const string
			CardViewClassNamePrefix = "dxcv",
			FlowTableClassNamePostfix = "FT",
			EndlessPagingMoreButtonContainerPostfix = "EMBC",
			EditCardErrorLayoutContainerPostfix = "ECELC",
			EditCardErrorLayoutContainerErrorCellPostfix = "ECELCEC",
			FlowLayoutEmptyCardWrapperPostfix = "FLECW",
			SeparatorStyleName = "Separator",
			CardStyleName = "Card",
			FlowCardStyleName = "FlowCard",
			EmptyCardStyleName = "EmptyCard",
			EmptyHiddenCardStyleName = "EmptyHiddenCard",
			FocusedCardStyleName = "FocusedCard",
			SelectedCardStyleName = "SelectedCard",
			CardHoverStyleName = "CardHover",
			EditFormCardStyleName = "EditForm",
			CardErrorStyleName = "CardError",
			HeaderPanelStyleName = "HeaderPanel",
			CommandItemStyleName = "CommandItem",
			SummaryItemStyleName = "SummaryItem",
			SummaryPanelStyleName = "SummaryPanel";
		public CardViewStyles(ISkinOwner grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesHeader"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewHeaderStyle Header { get { return (CardViewHeaderStyle)base.Header; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesCard"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle Card { get { return (CardViewCardStyle)GetStyle(CardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesFlowCard"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle FlowCard { get { return (CardViewCardStyle)GetStyle(FlowCardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesEmptyCard"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle EmptyCard { get { return (CardViewCardStyle)GetStyle(EmptyCardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesFocusedCard"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle FocusedCard { get { return (CardViewCardStyle)GetStyle(FocusedCardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesSelectedCard"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle SelectedCard { get { return (CardViewCardStyle)GetStyle(SelectedCardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesCardHotTrack"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle CardHotTrack { get { return (CardViewCardStyle)GetStyle(CardHoverStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesCardError"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewStyleBase CardError { get { return (CardViewStyleBase)GetStyle(CardErrorStyleName); } }
		[ PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCardStyle EditFormCard { get { return (CardViewCardStyle)GetStyle(EditFormCardStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesCommandItem"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewStyleBase CommandItem { get { return (CardViewStyleBase)GetStyle(CommandItemStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesSummaryItem"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewStyleBase SummaryItem { get { return (CardViewStyleBase)GetStyle(SummaryItemStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesSummaryPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewStyleBase SummaryPanel { get { return (CardViewStyleBase)GetStyle(SummaryPanelStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesBatchEditCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase BatchEditCell { get { return (CardViewStyleBase)base.BatchEditCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesBatchEditModifiedCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase BatchEditModifiedCell { get { return (CardViewStyleBase)base.BatchEditModifiedCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesTitlePanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase TitlePanel { get { return (CardViewStyleBase)base.TitlePanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesSearchPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase SearchPanel { get { return (CardViewStyleBase)base.SearchPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesPagerTopPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase PagerTopPanel { get { return (CardViewStyleBase)base.PagerTopPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesPagerBottomPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase PagerBottomPanel { get { return (CardViewStyleBase)base.PagerBottomPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesStatusBar"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase StatusBar { get { return (CardViewStyleBase)base.StatusBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBar"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBar { get { return (CardViewStyleBase)base.FilterBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarLink"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBarLink { get { return (CardViewStyleBase)base.FilterBarLink; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarCheckBoxCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBarCheckBoxCell { get { return (CardViewStyleBase)base.FilterBarCheckBoxCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarImageCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBarImageCell { get { return (CardViewStyleBase)base.FilterBarImageCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarExpressionCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBarExpressionCell { get { return (CardViewStyleBase)base.FilterBarExpressionCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesFilterBarClearButtonCell"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase FilterBarClearButtonCell { get { return (CardViewStyleBase)base.FilterBarClearButtonCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesLoadingPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel { get { return base.LoadingPanelInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesLoadingDiv"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv { get { return base.LoadingDivInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesDisabled"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled { get { return base.DisabledInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewStylesHeaderPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewStyleBase HeaderPanel { get { return (CardViewStyleBase)GetStyle(HeaderPanelStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewStylesTable"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyleBase Table { get { return (CardViewStyleBase)base.Table; } }
		public override string ToString() { return string.Empty; }
		protected internal override string GetCssClassNamePrefix() { return CardViewClassNamePrefix; }
		protected internal string FlowTableClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), FlowTableClassNamePostfix); } }
		protected internal string EndlessPagingMoreButtonContainerClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), EndlessPagingMoreButtonContainerPostfix); } }
		protected internal string EditCardErrorLayoutContainerClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), EditCardErrorLayoutContainerPostfix); } }
		protected internal string EditCardErrorLayoutContainerErrorCellClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), EditCardErrorLayoutContainerErrorCellPostfix); } }
		protected internal string FlowLayoutEmptyCardWrapperClassName { get { return string.Format(GridClassNameFormat, GetCssClassNamePrefix(), FlowLayoutEmptyCardWrapperPostfix); } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(HeaderStyleName, () => new CardViewHeaderStyle()));
			list.Add(new StyleInfo(TitlePanelStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(StatusBarStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(PagerTopPanelStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(PagerBottomPanelStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(SearchPanelStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarLinkStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarCheckBoxCellStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarImageCellStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarExpressionCellStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(FilterBarClearButtonCellStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(TableStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(BatchEditCellStyleName, delegate() { return new CardViewStyleBase(); }));
			list.Add(new StyleInfo(BatchEditModifiedCellStyleName, delegate() { return new CardViewStyleBase(); }));
			list.Add(new StyleInfo(CardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(FlowCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(EmptyHiddenCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(EmptyCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(FocusedCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(SelectedCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(CardHoverStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(CardErrorStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(EditFormCardStyleName, () => new CardViewCardStyle()));
			list.Add(new StyleInfo(HeaderPanelStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(CommandItemStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(SummaryItemStyleName, () => new CardViewStyleBase()));
			list.Add(new StyleInfo(SummaryPanelStyleName, () => new CardViewStyleBase()));
		}
	}
	public class CardViewPopupControlStyles : GridPopupControlStyles {
		public CardViewPopupControlStyles(ASPxCardView grid)
			: base(grid) {
		}
		protected new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesCommon"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewPopupControlStyle Common { get { return base.Common as CardViewPopupControlStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesEditForm"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewEditFormPopupStyle EditForm { get { return base.EditForm as CardViewEditFormPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesCustomizationWindow"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCustomizationWindowPopupStyle CustomizationWindow { get { return base.CustomizationWindow as CardViewCustomizationWindowPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesFilterBuilder"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewFilterBuilderPopupStyle FilterBuilder { get { return base.FilterBuilder as CardViewFilterBuilderPopupStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewPopupControlStylesHeaderFilter"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewHeaderFilterPopupStyle HeaderFilter { get { return base.HeaderFilter as CardViewHeaderFilterPopupStyle; } }
		protected override GridPopupControlStyle CreateCommonStyle() { return new CardViewPopupControlStyle(Grid); }
		protected override GridPopupControlStyle CreateEditFormStyle() { return new CardViewEditFormPopupStyle(Grid); }
		protected override GridPopupControlStyle CreateCustomizationWindowStyle() { return new CardViewCustomizationWindowPopupStyle(Grid); }
		protected override GridFilterBuilderPopupStyle CreateFilterBuilderStyle() { return new CardViewFilterBuilderPopupStyle(Grid); }
		protected override GridHeaderFilterPopupStyle CreateHeaderFilterStyle() { return new CardViewHeaderFilterPopupStyle(Grid); }
	}
	public class CardViewPopupControlStyle : GridPopupControlStyle {
		public CardViewPopupControlStyle(ASPxCardView grid)
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
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new CardViewStyleBase(); }));
		}
	}
	public class CardViewEditFormPopupStyle : GridPopupControlStyle {
		public CardViewEditFormPopupStyle(ASPxCardView grid)
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
		public new CardViewStyleBase MainArea { get { return base.MainArea as CardViewStyleBase; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new CardViewStyleBase(); }));
		}
	}
	public class CardViewCustomizationWindowPopupStyle : GridPopupControlStyle {
		public CardViewCustomizationWindowPopupStyle(ASPxCardView grid)
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
		public new CardViewStyleBase MainArea { get { return base.MainArea as CardViewStyleBase; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowFooterStyle Footer { get { return base.Footer; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupControlModalBackgroundStyle ModalBackground { get { return base.ModalBackground; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new CardViewStyleBase(); }));
		}
	}
	public class CardViewFilterBuilderPopupStyle : GridFilterBuilderPopupStyle {
		public CardViewFilterBuilderPopupStyle(ASPxCardView grid)
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
		public new CardViewStyleBase MainArea { get { return base.MainArea as CardViewStyleBase; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFilterBuilderPopupStyleButtonPanel"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppearanceStyle ButtonPanel { get { return base.ButtonPanel; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new CardViewStyleBase(); }));
		}
	}
	public class CardViewHeaderFilterPopupStyle : GridHeaderFilterPopupStyle {
		public CardViewHeaderFilterPopupStyle(ASPxCardView grid)
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
			list.Add(new StyleInfo(MainAreaStyleName, delegate() { return new CardViewStyleBase(); }));
		}
	}
	public class CardViewImages : GridImages {
		public CardViewImages(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesLoadingPanelOnStatusBar"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties LoadingPanelOnStatusBar { get { return base.LoadingPanelOnStatusBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesHeaderFilter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderFilter { get { return base.HeaderFilter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesHeaderActiveFilter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderActiveFilter { get { return base.HeaderActiveFilter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesHeaderSortDown"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderSortDown { get { return base.HeaderSortDown; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesHeaderSortUp"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderSortUp { get { return base.HeaderSortUp; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesCustomizationWindowClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties CustomizationWindowClose { get { return base.CustomizationWindowClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesPopupEditFormWindowClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties PopupEditFormWindowClose { get { return base.PopupEditFormWindowClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesFilterRowButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties FilterRowButton { get { return base.FilterRowButton; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesFilterBuilderClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties FilterBuilderClose { get { return base.FilterBuilderClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewImagesCellError"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties CellError { get { return base.CellError; } }
		protected override Type GetResourceType() {
			return typeof(ASPxCardView);
		}
		protected override string GetResourceImagePath() {
			return ASPxCardView.CardViewResourceImagePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxCardView.CardViewResourceImagePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxCardView.CardViewSpriteCssResourceName;
		}
	}
}
