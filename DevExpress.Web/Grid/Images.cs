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
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public abstract class GridImages : ImagesBase {
		protected internal const string
			LoadingPanelOnStatusBarName = "gvLoadingOnStatusBar",
			HeaderFilterName = "gvHeaderFilter",
			HeaderFilterActiveName = "gvHeaderFilterActive",
			HeaderSortDownName = "gvHeaderSortDown",
			HeaderSortUpName = "gvHeaderSortUp",
			CustomizationWindowCloseName = "gvCustomizationWindowClose",
			PopupEditFormWindowCloseName = "gvPopupEditFormWindowClose",
			FilterRowButtonName = "gvFilterRowButton",
			FilterBuilderCloseName = "FilterBuilderClose",
			BatchEditErrorCellName = "gvCellError",
			FormatConditionIconSetSpriteName = "FCISprite";
		public GridImages(ISkinOwner owner)
			: base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(LoadingPanelOnStatusBarName));
			list.Add(new ImageInfo(HeaderFilterName, ImageFlags.Empty,
				() => ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_HeaderFilterButton), HeaderFilterName));
			list.Add(new ImageInfo(HeaderFilterActiveName, ImageFlags.Empty,
				() => ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_HeaderFilterButtonActive), HeaderFilterActiveName));
			list.Add(new ImageInfo(HeaderSortDownName, ImageFlags.Empty,
				() => ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_SortedDescending), HeaderSortDownName));
			list.Add(new ImageInfo(HeaderSortUpName, ImageFlags.Empty,
				() => ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_SortedAscending), HeaderSortUpName));
			list.Add(new ImageInfo(FilterRowButtonName, ImageFlags.IsPng, 13, 13,
				() => ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_FilterRowButton), FilterRowButtonName));
			list.Add(new ImageInfo(CustomizationWindowCloseName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(PopupEditFormWindowCloseName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(FilterBuilderCloseName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(BatchEditErrorCellName, ImageFlags.Empty, BatchEditErrorCellName));
		}
		protected internal ImageProperties LoadingPanelOnStatusBar { get { return GetImage(LoadingPanelOnStatusBarName); } }
		protected internal ImageProperties HeaderFilter { get { return GetImage(HeaderFilterName); } }
		protected internal ImageProperties HeaderActiveFilter { get { return GetImage(HeaderFilterActiveName); } }
		protected internal ImageProperties HeaderSortDown { get { return GetImage(HeaderSortDownName); } }
		protected internal ImageProperties HeaderSortUp { get { return GetImage(HeaderSortUpName); } }
		protected internal ImageProperties CustomizationWindowClose { get { return GetImage(CustomizationWindowCloseName); } }
		protected internal ImageProperties PopupEditFormWindowClose { get { return GetImage(PopupEditFormWindowCloseName); } }
		protected internal ImageProperties FilterRowButton { get { return GetImage(FilterRowButtonName); } }
		protected internal ImageProperties FilterBuilderClose { get { return GetImage(FilterBuilderCloseName); } }
		protected internal ImageProperties CellError { get { return GetImage(BatchEditErrorCellName); } }
	}
	public class GridViewImages : GridImages {
		protected internal const string ElementName_ArrowDragDownImage = "IADD";
		protected internal const string ElementName_ArrowDragUpImage = "IADU";
		protected internal const string ElementName_DragHideColumnImage = "IDHF";
		protected internal const string
			CollapsedButtonName = "gvCollapsedButton",
			CollapsedButtonRtlName = "gvCollapsedButtonRtl",
			ExpandedButtonName = "gvExpandedButton",
			ExpandedButtonRtlName = "gvExpandedButtonRtl",
			DetailCollapsedButtonName = "gvDetailCollapsedButton",
			DetailCollapsedButtonRtlName = "gvDetailCollapsedButtonRtl",
			DetailExpandedButtonName = "gvDetailExpandedButton",
			DetailExpandedButtonRtlName = "gvDetailExpandedButtonRtl",
			ShowAdaptiveDetailButtonName = "gvShowAdaptiveDetailButton",
			HideAdaptiveDetailButtonName = "gvHideAdaptiveDetailButton",
			DragAndDropArrowDownName = "gvDragAndDropArrowDown",
			DragAndDropArrowUpName = "gvDragAndDropArrowUp",
			DragAndDropHideColumnName = "gvDragAndDropHideColumn",
			ParentGroupRowsName = "gvParentGroupRows",
			FixedGroupRowName = "gvFixedGroupRow",
			ContextMenuFullExpandName = "gvCMFullExpand",
			ContextMenuFullExpandDisabledName = "gvCMFullExpandDisabled",
			ContextMenuFullCollapseName = "gvCMFullCollapse",
			ContextMenuFullCollapseDisabledName = "gvCMFullCollapseDisabled",
			ContextMenuSortAscendingName = "gvCMSortAscending",
			ContextMenuSortAscendingDisabledName = "gvCMSortAscendingDisabled",
			ContextMenuSortDescendingName = "gvCMSortDescending",
			ContextMenuSortDescendingDisabledName = "gvCMSortDescendingDisabled",
			ContextMenuClearSortingName = "gvCMClearSorting",
			ContextMenuClearSortingDisabledName = "gvCMClearSortingDisabled",
			ContextMenuShowFilterEditorName = "gvCMShowFilterEditor",
			ContextMenuShowFilterEditorDisabledName = "gvCMShowFilterEditorDisabled",
			ContextMenuShowFilterRowName = "gvCMShowFilterRow",
			ContextMenuShowFilterRowDisabledName = "gvCMShowFilterRowDisabled",
			ContextMenuShowFilterRowMenuName = "gvCMShowFilterRowMenu",
			ContextMenuShowFilterRowMenuDisabledName = "gvCMShowFilterRowMenuDisabled",
			ContextMenuGroupByColumnName = "gvCMGroupByColumn",
			ContextMenuGroupByColumnDisabledName = "gvCMGroupByColumnDisabled",
			ContextMenuUngroupColumnName = "gvCMGroupByColumn",
			ContextMenuUngroupColumnDisabledName = "gvCMGroupByColumnDisabled",
			ContextMenuClearGroupingName = "gvCMClearGrouping",
			ContextMenuClearGroupingDisabledName = "gvCMClearGroupingDisabled",
			ContextMenuShowGroupPanelName = "gvCMShowGroupPanel",
			ContextMenuShowGroupPanelDisabledName = "gvCMShowGroupPanelDisabled",
			ContextMenuShowSearchPanelName = "gvCMShowSearchPanel",
			ContextMenuShowSearchPanelDisabledName = "gvCMShowSearchPanelDisabled",
			ContextMenuShowColumnName = "gvCMShowColumn",
			ContextMenuShowColumnDisabledName = "gvCMShowColumnDisabled",
			ContextMenuHideColumnName = "gvCMHideColumn",
			ContextMenuHideColumnDisabledName = "gvCMHideColumnDisabled",
			ContextMenuShowCustomizationWindowName = "gvCMShowCustomizationWindow",
			ContextMenuShowCustomizationWindowDisabledName = "gvCMShowCustomizationWindowDisabled",
			ContextMenuExpandRowName = "gvCMExpandRow",
			ContextMenuExpandRowDisabledName = "gvCMExpandRowDisabled",
			ContextMenuCollapseRowName = "gvCMCollapseRow",
			ContextMenuCollapseRowDisabledName = "gvCMCollapseRowDisabled",
			ContextMenuExpandDetailRowName = "gvCMExpandDetailRow",
			ContextMenuExpandDetailRowDisabledName = "gvCMExpandDetailRowDisabled",
			ContextMenuCollapseDetailRowName = "gvCMCollapseDetailRow",
			ContextMenuCollapseDetailRowDisabledName = "gvCMCollapseDetailRowDisabled",
			ContextMenuNewRowName = "gvCMNewRow",
			ContextMenuNewRowDisabledName = "gvCMNewRowDisabled",
			ContextMenuEditRowName = "gvCMEditRow",
			ContextMenuEditRowDisabledName = "gvCMEditRowDisabled",
			ContextMenuDeleteRowName = "gvCMDeleteRow",
			ContextMenuDeleteRowDisabledName = "gvCMDeleteRowDisabled",
			ContextMenuRefreshName = "gvCMRefresh",
			ContextMenuRefreshDisabledName = "gvCMRefreshDisabled",
			ContextMenuClearFilterName = "gvCMClearFilter",
			ContextMenuClearFilterDisabledName = "gvCMClearFilterDisabled",
			ContextMenuSummarySumName = "gvCMSummarySum",
			ContextMenuSummarySumDisabledName = "gvCMSummarySumDisabled",
			ContextMenuSummaryMinName = "gvCMSummaryMin",
			ContextMenuSummaryMinDisabledName = "gvCMSummaryMinDisabled",
			ContextMenuSummaryMaxName = "gvCMSummaryMax",
			ContextMenuSummaryMaxDisabledName = "gvCMSummaryMaxDisabled",
			ContextMenuSummaryAverageName = "gvCMSummaryAverage",
			ContextMenuSummaryAverageDisabledName = "gvCMSummaryAverageDisabled",
			ContextMenuSummaryCountName = "gvCMSummaryCount",
			ContextMenuSummaryCountDisabledName = "gvCMSummaryCountDisabled",
			ContextMenuSummaryNoneName = "gvCMSummaryNone",
			ContextMenuSummaryNoneDisabledName = "gvCMSummaryNoneDisabled",
			ContextMenuShowFooterName = "gvCMShowFooter",
			ContextMenuShowFooterDisabledName = "gvCMShowFooterDisabled";
		public GridViewImages(ISkinOwner owner) : base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CollapsedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }, CollapsedButtonName));
			list.Add(new ImageInfo(CollapsedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }, CollapsedButtonRtlName));
			list.Add(new ImageInfo(ExpandedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }, ExpandedButtonName));
			list.Add(new ImageInfo(ExpandedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }, ExpandedButtonRtlName));
			list.Add(new ImageInfo(DetailCollapsedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }, DetailCollapsedButtonName));
			list.Add(new ImageInfo(DetailCollapsedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }, DetailCollapsedButtonRtlName));
			list.Add(new ImageInfo(DetailExpandedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }, DetailExpandedButtonName));
			list.Add(new ImageInfo(DetailExpandedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }, DetailExpandedButtonRtlName));
			list.Add(new ImageInfo(DragAndDropArrowDownName, ImageFlags.Empty, "|", DragAndDropArrowDownName));
			list.Add(new ImageInfo(DragAndDropArrowUpName, ImageFlags.Empty, "|", DragAndDropArrowUpName));
			list.Add(new ImageInfo(DragAndDropHideColumnName, ImageFlags.Empty, 
				delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_DragAndDropHideColumnIcon); }, DragAndDropHideColumnName));
			list.Add(new ImageInfo(ParentGroupRowsName, ImageFlags.Empty, "...", ParentGroupRowsName));
			list.Add(new ImageInfo(FixedGroupRowName, ImageFlags.Empty, "...", FixedGroupRowName));
			list.Add(new ImageInfo(ShowAdaptiveDetailButtonName, ImageFlags.Empty, "...", ShowAdaptiveDetailButtonName));
			list.Add(new ImageInfo(HideAdaptiveDetailButtonName, ImageFlags.Empty, "X", HideAdaptiveDetailButtonName));
			list.Add(new ImageInfo(WindowResizerImageName, ImageFlags.Empty, "", WindowResizerImageName));
			list.Add(new ImageInfo(WindowResizerRtlImageName, ImageFlags.Empty, "", WindowResizerRtlImageName));
			list.Add(new ImageInfo(ContextMenuFullExpandName, ImageFlags.IsPng, 16, ContextMenuFullExpandName));
			list.Add(new ImageInfo(ContextMenuFullExpandDisabledName, ImageFlags.IsPng, 16, ContextMenuFullExpandDisabledName));
			list.Add(new ImageInfo(ContextMenuFullCollapseName, ImageFlags.IsPng, 16, ContextMenuFullCollapseName));
			list.Add(new ImageInfo(ContextMenuFullCollapseDisabledName, ImageFlags.IsPng, 16, ContextMenuFullCollapseDisabledName));
			list.Add(new ImageInfo(ContextMenuSortAscendingName, ImageFlags.IsPng, 16, ContextMenuSortAscendingName));
			list.Add(new ImageInfo(ContextMenuSortAscendingDisabledName, ImageFlags.IsPng, 16, ContextMenuSortAscendingDisabledName));
			list.Add(new ImageInfo(ContextMenuSortDescendingName, ImageFlags.IsPng, 16, ContextMenuSortDescendingName));
			list.Add(new ImageInfo(ContextMenuSortDescendingDisabledName, ImageFlags.IsPng, 16, ContextMenuSortDescendingDisabledName));
			list.Add(new ImageInfo(ContextMenuClearSortingName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuClearSortingDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuClearFilterName, ImageFlags.IsPng, 16, ContextMenuClearFilterName));
			list.Add(new ImageInfo(ContextMenuClearFilterDisabledName, ImageFlags.IsPng, 16, ContextMenuClearFilterDisabledName));
			list.Add(new ImageInfo(ContextMenuShowFilterEditorName, ImageFlags.IsPng, 16, ContextMenuShowFilterEditorName));
			list.Add(new ImageInfo(ContextMenuShowFilterEditorDisabledName, ImageFlags.IsPng, 16, ContextMenuShowFilterEditorDisabledName));
			list.Add(new ImageInfo(ContextMenuShowFilterRowName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowFilterRowDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowFilterRowMenuName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowFilterRowMenuDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuGroupByColumnName, ImageFlags.IsPng, 16, ContextMenuGroupByColumnName));
			list.Add(new ImageInfo(ContextMenuGroupByColumnDisabledName, ImageFlags.IsPng, 16, ContextMenuGroupByColumnName));
			list.Add(new ImageInfo(ContextMenuUngroupColumnName, ImageFlags.IsPng, 16, ContextMenuUngroupColumnName));
			list.Add(new ImageInfo(ContextMenuUngroupColumnDisabledName, ImageFlags.IsPng, 16, ContextMenuUngroupColumnDisabledName));
			list.Add(new ImageInfo(ContextMenuClearGroupingName, ImageFlags.IsPng, 16, ContextMenuClearGroupingName));
			list.Add(new ImageInfo(ContextMenuClearGroupingDisabledName, ImageFlags.IsPng, 16, ContextMenuClearGroupingDisabledName));
			list.Add(new ImageInfo(ContextMenuShowGroupPanelName, ImageFlags.IsPng, 16, ContextMenuShowGroupPanelName));
			list.Add(new ImageInfo(ContextMenuShowGroupPanelDisabledName, ImageFlags.IsPng, 16, ContextMenuShowGroupPanelDisabledName));
			list.Add(new ImageInfo(ContextMenuShowSearchPanelName, ImageFlags.IsPng, 16, ContextMenuShowSearchPanelName));
			list.Add(new ImageInfo(ContextMenuShowSearchPanelDisabledName, ImageFlags.IsPng, 16, ContextMenuShowSearchPanelDisabledName));
			list.Add(new ImageInfo(ContextMenuShowColumnName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowColumnDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuHideColumnName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuHideColumnDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowCustomizationWindowName, ImageFlags.IsPng, 16, ContextMenuShowCustomizationWindowName));
			list.Add(new ImageInfo(ContextMenuShowCustomizationWindowDisabledName, ImageFlags.IsPng, 16, ContextMenuShowCustomizationWindowDisabledName));
			list.Add(new ImageInfo(ContextMenuExpandRowName, ImageFlags.IsPng, 16, ContextMenuExpandRowName));
			list.Add(new ImageInfo(ContextMenuExpandRowDisabledName, ImageFlags.IsPng, 16, ContextMenuExpandRowDisabledName));
			list.Add(new ImageInfo(ContextMenuCollapseRowName, ImageFlags.IsPng, 16, ContextMenuCollapseRowName));
			list.Add(new ImageInfo(ContextMenuCollapseRowDisabledName, ImageFlags.IsPng, 16, ContextMenuCollapseRowDisabledName));
			list.Add(new ImageInfo(ContextMenuExpandDetailRowName, ImageFlags.IsPng, 16, ContextMenuExpandDetailRowName));
			list.Add(new ImageInfo(ContextMenuExpandDetailRowDisabledName, ImageFlags.IsPng, 16, ContextMenuExpandDetailRowDisabledName));
			list.Add(new ImageInfo(ContextMenuCollapseDetailRowName, ImageFlags.IsPng, 16, ContextMenuCollapseDetailRowName));
			list.Add(new ImageInfo(ContextMenuCollapseDetailRowDisabledName, ImageFlags.IsPng, 16, ContextMenuCollapseDetailRowDisabledName));
			list.Add(new ImageInfo(ContextMenuNewRowName, ImageFlags.IsPng, 16, ContextMenuNewRowName));
			list.Add(new ImageInfo(ContextMenuNewRowDisabledName, ImageFlags.IsPng, 16, ContextMenuNewRowDisabledName));
			list.Add(new ImageInfo(ContextMenuEditRowName, ImageFlags.IsPng, 16, ContextMenuEditRowName));
			list.Add(new ImageInfo(ContextMenuEditRowDisabledName, ImageFlags.IsPng, 16, ContextMenuEditRowDisabledName));
			list.Add(new ImageInfo(ContextMenuDeleteRowName, ImageFlags.IsPng, 16, ContextMenuDeleteRowName));
			list.Add(new ImageInfo(ContextMenuDeleteRowDisabledName, ImageFlags.IsPng, 16, ContextMenuDeleteRowDisabledName));
			list.Add(new ImageInfo(ContextMenuRefreshName, ImageFlags.IsPng, 16, ContextMenuRefreshName));
			list.Add(new ImageInfo(ContextMenuRefreshDisabledName, ImageFlags.IsPng, 16, ContextMenuRefreshDisabledName));
			list.Add(new ImageInfo(ContextMenuSummarySumName, ImageFlags.IsPng, 16, ContextMenuSummarySumName));
			list.Add(new ImageInfo(ContextMenuSummarySumDisabledName, ImageFlags.IsPng, 16, ContextMenuSummarySumDisabledName));
			list.Add(new ImageInfo(ContextMenuSummaryMinName, ImageFlags.IsPng, 16, ContextMenuSummaryMinName));
			list.Add(new ImageInfo(ContextMenuSummaryMinDisabledName, ImageFlags.IsPng, 16, ContextMenuSummaryMinDisabledName));
			list.Add(new ImageInfo(ContextMenuSummaryMaxName, ImageFlags.IsPng, 16, ContextMenuSummaryMaxName));
			list.Add(new ImageInfo(ContextMenuSummaryMaxDisabledName, ImageFlags.IsPng, 16, ContextMenuSummaryMaxDisabledName));
			list.Add(new ImageInfo(ContextMenuSummaryAverageName, ImageFlags.IsPng, 16, ContextMenuSummaryAverageName));
			list.Add(new ImageInfo(ContextMenuSummaryAverageDisabledName, ImageFlags.IsPng, 16, ContextMenuSummaryAverageDisabledName));
			list.Add(new ImageInfo(ContextMenuSummaryCountName, ImageFlags.IsPng, 16, ContextMenuSummaryCountName));
			list.Add(new ImageInfo(ContextMenuSummaryCountDisabledName, ImageFlags.IsPng, 16, ContextMenuSummaryCountDisabledName));
			list.Add(new ImageInfo(ContextMenuSummaryNoneName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuSummaryNoneDisabledName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowFooterName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ContextMenuShowFooterDisabledName, ImageFlags.HasNoResourceImage));
		}
		public override string ToString() { return string.Empty; }
		protected override Type GetResourceType() {
			return typeof(ASPxGridView);
		}
		protected override string GetResourceImagePath() {
			return ASPxGridView.GridViewResourceImagePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxGridView.GridViewResourceImagePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxGridView.GridViewSpriteCssResourceName;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesLoadingPanelOnStatusBar"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties LoadingPanelOnStatusBar { get { return base.LoadingPanelOnStatusBar; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesCollapsedButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties CollapsedButton { get { return GetImage(CollapsedButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesCollapsedButtonRtl"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties CollapsedButtonRtl { get { return GetImage(CollapsedButtonRtlName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesExpandedButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ExpandedButton { get { return GetImage(ExpandedButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesExpandedButtonRtl"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ExpandedButtonRtl { get { return GetImage(ExpandedButtonRtlName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDetailCollapsedButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailCollapsedButton { get { return GetImage(DetailCollapsedButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDetailCollapsedButtonRtl"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailCollapsedButtonRtl { get { return GetImage(DetailCollapsedButtonRtlName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDetailExpandedButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailExpandedButton { get { return GetImage(DetailExpandedButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDetailExpandedButtonRtl"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailExpandedButtonRtl { get { return GetImage(DetailExpandedButtonRtlName); } }
		[ Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ShowAdaptiveDetailButton { get { return GetImage(ShowAdaptiveDetailButtonName); } }
		[ Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties HideAdaptiveDetailButton { get { return GetImage(HideAdaptiveDetailButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesHeaderFilter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderFilter { get { return base.HeaderFilter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesHeaderActiveFilter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderActiveFilter { get { return base.HeaderActiveFilter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesHeaderSortDown"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderSortDown { get { return base.HeaderSortDown; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesHeaderSortUp"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties HeaderSortUp { get { return base.HeaderSortUp; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDragAndDropArrowDown"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropArrowDown { get { return GetImage(DragAndDropArrowDownName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDragAndDropArrowUp"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropArrowUp { get { return GetImage(DragAndDropArrowUpName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesDragAndDropColumnHide"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropColumnHide { get { return GetImage(DragAndDropHideColumnName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesParentGroupRows"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ParentGroupRows { get { return GetImage(ParentGroupRowsName); } }
		[ Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties FixedGroupRow { get { return GetImage(FixedGroupRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesFilterRowButton"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties FilterRowButton { get { return base.FilterRowButton; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesCustomizationWindowClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties CustomizationWindowClose { get { return base.CustomizationWindowClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesPopupEditFormWindowClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties PopupEditFormWindowClose { get { return base.PopupEditFormWindowClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesWindowResizer"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties WindowResizer { get { return WindowResizerInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesWindowResizerRtl"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties WindowResizerRtl { get { return WindowResizerRtlInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesFilterBuilderClose"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties FilterBuilderClose { get { return base.FilterBuilderClose; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesCellError"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties CellError { get { return base.CellError; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuFullExpand"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuFullExpand { get { return GetImage(ContextMenuFullExpandName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuFullExpandDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuFullExpandDisabled { get { return GetImage(ContextMenuFullExpandDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuFullCollapse"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuFullCollapse { get { return GetImage(ContextMenuFullCollapseName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuFullCollapseDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuFullCollapseDisabled { get { return GetImage(ContextMenuFullCollapseDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSortAscending"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSortAscending { get { return GetImage(ContextMenuSortAscendingName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSortAscendingDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSortAscendingDisabled { get { return GetImage(ContextMenuSortAscendingDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSortDescending"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSortDescending { get { return GetImage(ContextMenuSortDescendingName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSortDescendingDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSortDescendingDisabled { get { return GetImage(ContextMenuSortDescendingDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearSorting"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearSorting { get { return GetImage(ContextMenuClearSortingName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearSortingDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearSortingDisabled { get { return GetImage(ContextMenuClearSortingDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterEditor"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterEditor { get { return GetImage(ContextMenuShowFilterEditorName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterEditorDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterEditorDisabled { get { return GetImage(ContextMenuShowFilterEditorDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterRow { get { return GetImage(ContextMenuShowFilterRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterRowDisabled { get { return GetImage(ContextMenuShowFilterRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterRowMenu"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterRowMenu { get { return GetImage(ContextMenuShowFilterRowMenuName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFilterRowMenuDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFilterRowMenuDisabled { get { return GetImage(ContextMenuShowFilterRowMenuDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuGroupByColumn"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuGroupByColumn { get { return GetImage(ContextMenuGroupByColumnName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuGroupByColumnDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuGroupByColumnDisabled { get { return GetImage(ContextMenuGroupByColumnDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuUngroupColumn"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuUngroupColumn { get { return GetImage(ContextMenuUngroupColumnName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuUngroupColumnDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuUngroupColumnDisabled { get { return GetImage(ContextMenuUngroupColumnDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearGrouping"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearGrouping { get { return GetImage(ContextMenuClearGroupingName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearGroupingDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearGroupingDisabled { get { return GetImage(ContextMenuClearGroupingDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowGroupPanel"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowGroupPanel { get { return GetImage(ContextMenuShowGroupPanelName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowGroupPanelDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowGroupPanelDisabled { get { return GetImage(ContextMenuShowGroupPanelDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowSearchPanel"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowSearchPanel { get { return GetImage(ContextMenuShowSearchPanelName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowSearchPanelDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowSearchPanelDisabled { get { return GetImage(ContextMenuShowSearchPanelDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowColumn"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowColumn { get { return GetImage(ContextMenuShowColumnName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowColumnDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowColumnDisabled { get { return GetImage(ContextMenuShowColumnDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuHideColumn"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuHideColumn { get { return GetImage(ContextMenuHideColumnName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuHideColumnDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuHideColumnDisabled { get { return GetImage(ContextMenuHideColumnDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowCustomizationWindow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowCustomizationWindow { get { return GetImage(ContextMenuShowCustomizationWindowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowCustomizationWindowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowCustomizationWindowDisabled { get { return GetImage(ContextMenuShowCustomizationWindowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuExpandRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuExpandRow { get { return GetImage(ContextMenuExpandRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuExpandRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuExpandRowDisabled { get { return GetImage(ContextMenuExpandRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuCollapseRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuCollapseRow { get { return GetImage(ContextMenuCollapseRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuCollapseRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuCollapseRowDisabled { get { return GetImage(ContextMenuCollapseRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuExpandDetailRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuExpandDetailRow { get { return GetImage(ContextMenuExpandDetailRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuExpandDetailRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuExpandDetailRowDisabled { get { return GetImage(ContextMenuExpandDetailRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuCollapseDetailRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuCollapseDetailRow { get { return GetImage(ContextMenuCollapseDetailRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuCollapseDetailRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuCollapseDetailRowDisabled { get { return GetImage(ContextMenuCollapseDetailRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuNewRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuNewRow { get { return GetImage(ContextMenuNewRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuNewRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuNewRowDisabled { get { return GetImage(ContextMenuNewRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuEditRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuEditRow { get { return GetImage(ContextMenuEditRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuEditRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuEditRowDisabled { get { return GetImage(ContextMenuEditRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuDeleteRow"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuDeleteRow { get { return GetImage(ContextMenuDeleteRowName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuDeleteRowDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuDeleteRowDisabled { get { return GetImage(ContextMenuDeleteRowDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuRefresh"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuRefresh { get { return GetImage(ContextMenuRefreshName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuRefreshDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuRefreshDisabled { get { return GetImage(ContextMenuRefreshDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearFilter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearFilter { get { return GetImage(ContextMenuClearFilterName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuClearFilterDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuClearFilterDisabled { get { return GetImage(ContextMenuClearFilterDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummarySum"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummarySum { get { return GetImage(ContextMenuSummarySumName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummarySumDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummarySumDisabled { get { return GetImage(ContextMenuSummarySumDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryMin"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryMin { get { return GetImage(ContextMenuSummaryMinName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryMinDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryMinDisabled { get { return GetImage(ContextMenuSummaryMinDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryMax"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryMax { get { return GetImage(ContextMenuSummaryMaxName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryMaxDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryMaxDisabled { get { return GetImage(ContextMenuSummaryMaxDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryAverage"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryAverage { get { return GetImage(ContextMenuSummaryAverageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryAverageDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryAverageDisabled { get { return GetImage(ContextMenuSummaryAverageDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryCount"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryCount { get { return GetImage(ContextMenuSummaryCountName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryCountDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryCountDisabled { get { return GetImage(ContextMenuSummaryCountDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryNone"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryNone { get { return GetImage(ContextMenuSummaryNoneName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuSummaryNoneDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuSummaryNoneDisabled { get { return GetImage(ContextMenuSummaryNoneDisabledName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFooter"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFooter { get { return GetImage(ContextMenuShowFooterName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewImagesContextMenuShowFooterDisabled"),
#endif
 Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ContextMenuShowFooterDisabled { get { return GetImage(ContextMenuShowFooterDisabledName); } }
	}
	public class GridViewEditorImages : EditorImages {
		public GridViewEditorImages(ISkinOwner skinOwner) : base(skinOwner) { }
	}
}
