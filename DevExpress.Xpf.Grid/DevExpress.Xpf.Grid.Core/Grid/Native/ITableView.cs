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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using System.Windows.Media;
using DevExpress.Xpf.Grid.EditForm;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid {
	public enum GroupSummaryDisplayMode { Default, AlignByColumns }
	public interface IGroupSummaryDisplayMode {
		GroupSummaryDisplayMode GroupSummaryDisplayMode { get; set; }
	}
	public partial interface ITableView {
		bool AllowResizing { get; }
		bool AllowBestFit { get; }
		bool AutoWidth { get; }
		bool IsEditing { get; }
		bool ShowAutoFilterRow { get; }
		Style AutoFilterRowCellStyle { get; }
		bool AllowCascadeUpdate { get; }
		bool AllowPerPixelScrolling { get; }
		bool AllowScrollHeaders { get; }
		double ScrollAnimationDuration { get; }
		ScrollAnimationMode ScrollAnimationMode { get; }
		bool AllowScrollAnimation { get; }
		bool ShowIndicator { get; set; }
		bool ActualShowIndicator { get; }
		double ActualIndicatorWidth { get; }
		bool UseIndicatorForSelection { get; }
		bool AllowHorizontalScrollingVirtualization { get; }
		bool AutoMoveRowFocus { get; }
		bool AllowFixedColumnMenu { get; set; }
		bool ShowVerticalLines { get; set; }
		bool ShowHorizontalLines { get; set; }
		bool ActualShowDetailButtons { get; }
		bool IsCheckBoxSelectorColumnVisible { get; }
		double LeftGroupAreaIndent { get; }
		double RightGroupAreaIndent { get; }
		double LeftDataAreaIndent { get; }
		double RightDataAreaIndent { get; }
		double FixedNoneContentWidth { get; set; }
		double TotalSummaryFixedNoneContentWidth { get; set; }
		double VerticalScrollBarWidth { get; set; }
		double FixedLeftContentWidth { get; set; }
		double FixedRightContentWidth { get; set; }
		double TotalGroupAreaIndent { get; set; }
		double IndicatorWidth { get; set; }
		double IndicatorHeaderWidth { get; set; }
		double FixedLineWidth { get; set; }
		double HorizontalViewport { get; }
		double ActualExpandDetailButtonWidth { get; }
		Thickness ActualDetailMargin { get; }
		double ActualExpandDetailHeaderWidth { get; }
		Thickness ScrollingVirtualizationMargin { get; set; }
		Thickness ScrollingHeaderVirtualizationMargin { get; set; }
		TableViewBehavior TableViewBehavior { get; }
		IList<ColumnBase> ViewportVisibleColumns { get; set; }
		DataViewBase ViewBase { get; }
		CellBase CreateGridCell(int rowHandle, ColumnBase column);
		ITableViewHitInfo CalcHitInfo(DependencyObject d);
		ControlTemplate FocusedRowBorderTemplate { get; }
		ControlTemplate ColumnBandChooserTemplate { get; }
		DependencyPropertyKey ActualDataRowTemplateSelectorPropertyKey { get; }
		DataTemplateSelector DataRowTemplateSelector { get; }
		DataTemplate DataRowTemplate { get; set; }
		ControlTemplate RowDecorationTemplate { get; set; }
		DataTemplateSelector ActualDataRowTemplateSelector { get; }
		bool PrintAutoWidth { get; }
		bool PrintColumnHeaders { get; }
		bool PrintBandHeaders { get; }
		bool PrintTotalSummary { get; }
		bool PrintFixedTotalSummary { get; }
		Style PrintColumnHeaderStyle { get; }
		Style PrintCellStyle { get; }
		Style PrintTotalSummaryStyle { get; }
		Style PrintFixedTotalSummaryStyle { get; }
		Style PrintRowIndentStyle { get; }
		int BestFitMaxRowCount { get; }
		Xpf.Core.BestFitMode BestFitMode { get; }
		double RowMinHeight { get; set; }
		BestFitArea BestFitArea { get; }
		void SetHorizontalViewport(double value);
		void SetFixedLeftVisibleColumns(IList<ColumnBase> columns);
		void SetFixedRightVisibleColumns(IList<ColumnBase> columns);
		void SetFixedNoneVisibleColumns(IList<ColumnBase> columns);
		void CopyCellsToClipboard(IEnumerable<CellBase> cells);
		void SetActualShowIndicator(bool showIndicator);
		void SetActualIndicatorWidth(double indicatorWidth);
		void SetActualExpandDetailHeaderWidth(double expandDetailButtonWidth);
		void SetActualDetailMargin(Thickness detailMargin);
		void SetShowTotalSummaryIndicatorIndent(bool showTotalSummaryIndicatorIndent);
		void SetActualFadeSelectionOnLostFocus(bool fadeSelectionOnLostFocus);
		void RaiseRowDoubleClickEvent(ITableViewHitInfo hitInfo, MouseButton changedButton);
		void SetExpandColumnPosition(ColumnPosition position);
		bool ShowBandsPanel { get; }
		bool AllowChangeColumnParent { get; set; }
		bool AllowChangeBandParent { get; }
		bool ShowBandsInCustomizationForm { get; }
		bool AllowBandMoving { get; }
		bool AllowBandResizing { get; }
		bool AllowAdvancedVerticalNavigation { get; }
		bool AllowAdvancedHorizontalNavigation { get; }
		IComparer<BandBase> ColumnChooserBandsSortOrderComparer { get; }
		DataTemplate BandHeaderTemplate { get; }
		DataTemplateSelector BandHeaderTemplateSelector { get; }
		DataTemplate BandHeaderToolTipTemplate { get; }
		Style PrintBandHeaderStyle { get; }
		int AlternationCount { get; set; }
		Brush AlternateRowBackground { get; set; }
		Brush EvenRowBackground { get; set; }
		bool UseEvenRowBackground { get; set; }
		DataTemplate RowIndicatorContentTemplate { get; set; }
		Style RowStyle { get; set; }
		bool ActualAllowTreeIndentScrolling { get; }
		int EditFormColumnCount { get; set; }
		EditFormPostMode EditFormPostMode { get; set; }
		EditFormShowMode EditFormShowMode { get; set; }
		DataTemplate EditFormDialogServiceTemplate { get; set; }
		DataTemplate EditFormTemplate { get; set; }
		bool ShowEditFormUpdateCancelButtons { get; set; }
		bool ShowEditFormOnF2Key { get; set; }
		bool ShowEditFormOnEnterKey { get; set; }
		bool ShowEditFormOnDoubleClick { get; set; }
		BindingBase EditFormCaptionBinding { get; set; }
		PostConfirmationMode EditFormPostConfirmation { get; set; }
		void ShowDialogEditForm();
		void ShowInlineEditForm();
		void ShowEditForm();
		void HideEditForm();
		void CloseEditForm();
	}
	public class MasterDetailPrintInfo {
		public MasterDetailPrintInfo(DefaultBoolean allowPrintDetails, DefaultBoolean allowPrintEmptyDetails, DefaultBoolean printAllDetails, ISupportMasterDetailPrinting rootPrintingDataTreeBuilder, PrintDetailType printDetailType = PrintDetailType.None, int detailGroupLevel = 0) {
			AllowPrintDetails = allowPrintDetails;
			AllowPrintEmptyDetails = allowPrintEmptyDetails;
			PrintAllDetails = printAllDetails;
			PrintDetailType = printDetailType;
			DetailGroupLevel = detailGroupLevel;
			RootPrintingDataTreeBuilder = rootPrintingDataTreeBuilder;
		}
		public DefaultBoolean AllowPrintDetails { get; private set; }
		public DefaultBoolean AllowPrintEmptyDetails { get; private set; }
		public DefaultBoolean PrintAllDetails { get; private set; }
		public PrintDetailType PrintDetailType { get; private set; }
		public int DetailGroupLevel { get; private set; }
		public ISupportMasterDetailPrinting RootPrintingDataTreeBuilder { get; private set; }
	}
	public enum PrintDetailType { First, None, Last }
}
