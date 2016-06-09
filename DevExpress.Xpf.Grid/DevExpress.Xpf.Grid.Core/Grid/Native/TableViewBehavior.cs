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
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Linq;
using System.Collections;
using System.Windows.Media;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using DialogService = DevExpress.Xpf.Core.DialogService;
using IDialogService = DevExpress.Mvvm.IDialogService;
using DataViewAssignableDialogServiceHelper = DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<DevExpress.Xpf.Grid.DataViewBase, DevExpress.Mvvm.IDialogService>;
using DevExpress.Xpf.Grid.EditForm;
#if !SL
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
#endif
namespace DevExpress.Xpf.Grid {
	public class TableViewProperties {
		public static readonly DependencyProperty FixedAreaStyleProperty;
		static TableViewProperties() {
			FixedAreaStyleProperty = DependencyPropertyManager.RegisterAttached("FixedAreaStyle", typeof(FixedStyle), typeof(TableViewProperties), new FrameworkPropertyMetadata(FixedStyle.None, FrameworkPropertyMetadataOptions.Inherits));
		}
		public static void SetFixedAreaStyle(DependencyObject element, FixedStyle value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(FixedAreaStyleProperty, value);
		}
		public static FixedStyle GetFixedAreaStyle(DependencyObject element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (FixedStyle)element.GetValue(FixedAreaStyleProperty);
		}
	}
}
namespace DevExpress.Xpf.Grid.Native {
	internal class RowStateObject : DependencyObject { }
	public abstract partial class TableViewBehavior : DataViewBehavior {
		#region static
		internal static DependencyProperty RegisterAllowFixedColumnMenuProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowFixedColumnMenu", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		internal static DependencyProperty RegisterAllowScrollHeadersProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowScrollHeaders", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		internal static DependencyProperty RegisterUseIndicatorForSelectionProperty(Type ownerType) {
			return DependencyPropertyManager.Register("UseIndicatorForSelection", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		internal static DependencyProperty RegisterMultiSelectModeProperty(Type ownerType) {
			return DependencyPropertyManager.Register("MultiSelectMode", typeof(TableViewSelectMode), ownerType, new FrameworkPropertyMetadata(TableViewSelectMode.None, TableViewBehavior.OnMultiSelectModeChanged));
		}
		internal static DependencyPropertyKey RegisterIndicatorHeaderWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("IndicatorHeaderWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d));
		}
		internal static DependencyProperty RegisterIndicatorWidthProperty(Type ownerType) {
			return DependencyPropertyManager.Register("IndicatorWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(16d, TableViewBehavior.OnIndicatorWidthChanged));
		}
		internal static DependencyPropertyKey RegisterActualIndicatorWidthPropertyKey(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ActualIndicatorWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(16d, (d, _) => ((ITableView)d).TableViewBehavior.OnActualIndicatorWidthChanged()));
		}
		internal static DependencyPropertyKey RegisterShowTotalSummaryIndicatorIndentPropertyKey(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ShowTotalSummaryIndicatorIndent", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		internal static DependencyProperty RegisterShowIndicatorProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowIndicator", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, TableViewBehavior.OnShowIndicatorChanged));
		}
		internal static DependencyPropertyKey RegisterActualShowIndicatorProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ActualShowIndicator", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, _) => ((ITableView)d).TableViewBehavior.OnActualShowIndicatorChanged()));
		}
		internal static DependencyProperty RegisterAllowBestFitProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowBestFit", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		internal static DependencyProperty RegisterAutoMoveRowFocusProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AutoMoveRowFocus", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		internal static DependencyProperty RegisterRowMinHeightProperty(Type ownerType) {
			return DependencyPropertyManager.Register("RowMinHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(20d, (d, _) => ((ITableView)d).TableViewBehavior.OnRowMinHeightChanged()));
		}
		internal static DependencyProperty RegisterHeaderPanelMinHeightProperty(Type ownerType) {
			return DependencyPropertyManager.Register("HeaderPanelMinHeight", typeof(double), ownerType, new FrameworkPropertyMetadata(20d));
		}
		internal static DependencyPropertyKey RegisterFixedNoneContentWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("FixedNoneContentWidth", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((ITableView)d).TableViewBehavior.OnFixedNoneContentWidthChanged()));
		}
		internal static DependencyPropertyKey RegisterTotalSummaryFixedNoneContentWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("TotalSummaryFixedNoneContentWidth", typeof(double), ownerType, new PropertyMetadata(0d));
		}
		internal static DependencyPropertyKey RegisterVerticalScrollBarWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("VerticalScrollBarWidth", typeof(double), ownerType, new PropertyMetadata(0d));
		}
		internal static DependencyPropertyKey RegisterFixedLeftContentWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("FixedLeftContentWidth", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((ITableView)d).TableViewBehavior.OnFixedLeftContentWidthChanged()));
		}
		internal static DependencyPropertyKey RegisterFixedRightContentWidthProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("FixedRightContentWidth", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((ITableView)d).TableViewBehavior.OnFixedRightContentWidthChanged()));
		}
		internal static DependencyPropertyKey RegisterTotalGroupAreaIndentProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("TotalGroupAreaIndent", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((ITableView)d).TableViewBehavior.OnTotalGroupAreaIndentChanged()));
		}
		internal static DependencyPropertyKey RegisterScrollingHeaderVirtualizationMarginProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ScrollingHeaderVirtualizationMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((ITableView)d).TableViewBehavior.OnScrollingVirtualizationMarginChanged()));
		}
		internal static DependencyPropertyKey RegisterScrollingVirtualizationMarginProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ScrollingVirtualizationMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((ITableView)d).TableViewBehavior.OnScrollingVirtualizationMarginChanged()));
		}
		internal static DependencyProperty RegisterRowStyleProperty(Type ownerType) {
			return DependencyPropertyManager.Register("RowStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnRowStyleChanged((Style)e.NewValue)));
		}
		internal static DependencyProperty RegisterAllowHorizontalScrollingVirtualizationProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowHorizontalScrollingVirtualization", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.OnAllowHorizontalScrollingVirtualizationChanged()));
		}
		internal static DependencyProperty RegisterAllowResizingProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowResizing", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, DataViewBase.OnUpdateColumnsViewInfo));
		}
		internal static DependencyProperty RegisterRowIndicatorContentTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("RowIndicatorContentTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnRowIndicatorContentTemplateChanged()));
		}
		internal static DependencyPropertyKey RegisterActualDataRowTemplateSelectorProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ActualDataRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnActualDataRowTemplateSelectorChanged()));
		}
		internal static DependencyProperty RegisterDataRowTemplateSelectorProperty(Type ownerType) {
			return DependencyPropertyManager.Register("DataRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateActualDataRowTemplateSelector()));
		}
		internal static DependencyProperty RegisterDataRowTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("DataRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateActualDataRowTemplateSelector()));
		}
		internal static DependencyProperty RegisterDefaultDataRowTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("DefaultDataRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
		}
		internal static DependencyProperty RegisterRowDecorationTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("RowDecorationTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnRowDecorationTemplateChanged()));
		}
		internal static DependencyProperty RegisterShowHorizontalLinesProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowHorizontalLines", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.OnShowHorizontalLinesChanged()));
		}
		internal static DependencyProperty RegisterShowVerticalLinesProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowVerticalLines", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.OnShowVerticalLinesChanged()));
		}
		internal static DependencyProperty RegisterFixedLineWidthProperty(Type ownerType) {
			return DependencyPropertyManager.Register("FixedLineWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(2d, (d, e) => ((ITableView)d).TableViewBehavior.OnFixedLineWidthChanged(), (d, value) => Math.Max(0, (double)value)));
		}
		internal static DependencyPropertyKey RegisterHorizontalViewportProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("HorizontalViewport", typeof(double), ownerType, new FrameworkPropertyMetadata(0d));
		}
		internal static DependencyPropertyKey RegisterFixedNoneVisibleColumnsProperty<T>(Type ownerType) where T : ColumnBase {
			return DependencyPropertyManager.RegisterReadOnly("FixedNoneVisibleColumns", typeof(IList<T>), ownerType, new FrameworkPropertyMetadata(null));
		}
		internal static DependencyPropertyKey RegisterFixedRightVisibleColumnsProperty<T>(Type ownerType) where T : ColumnBase {
			return DependencyPropertyManager.RegisterReadOnly("FixedRightVisibleColumns", typeof(IList<T>), ownerType, new FrameworkPropertyMetadata(null, OnFixedVisibleColumnsChanged));
		}
		internal static DependencyPropertyKey RegisterFixedLeftVisibleColumnsProperty<T>(Type ownerType) where T : ColumnBase {
			return DependencyPropertyManager.RegisterReadOnly("FixedLeftVisibleColumns", typeof(IList<T>), ownerType, new FrameworkPropertyMetadata(null, OnFixedVisibleColumnsChanged));
		}
		static void OnFixedVisibleColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var behavior = ((ITableView)d).TableViewBehavior;
			if(behavior != null)
				behavior.OnFixedVisibleColumnsChanged();
		}
		internal static DependencyProperty RegisterShowAutoFilterRowProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowAutoFilterRow", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ITableView)d).TableViewBehavior.OnShowAutoFilterRowChanged()));
		}
		internal static DependencyProperty RegisterAllowCascadeUpdateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowCascadeUpdate", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		internal static DependencyProperty RegisterAllowPerPixelScrollingProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowPerPixelScrolling", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((DataViewBase)d).OnAllowPerPixelScrollingChanged()));
		}
		internal static DependencyProperty RegisterScrollAnimationDurationProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ScrollAnimationDuration", typeof(double), ownerType, new PropertyMetadata(350d));
		}
		internal static DependencyProperty RegisterScrollAnimationModeProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ScrollAnimationMode", typeof(ScrollAnimationMode), ownerType, new PropertyMetadata(ScrollAnimationMode.EaseOut));
		}
		internal static DependencyProperty RegisterAllowScrollAnimationProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowScrollAnimation", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		internal static DependencyProperty RegisterExtendScrollBarToFixedColumnsProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ExtendScrollBarToFixedColumns", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		internal static RoutedEvent RegisterCustomScrollAnimationEvent(Type ownerType) {
			return EventManager.RegisterRoutedEvent("CustomScrollAnimation", RoutingStrategy.Direct, typeof(CustomScrollAnimationEventHandler), ownerType);
		}
		internal static DependencyProperty RegisterRightDataAreaIndentProperty(Type ownerType) {
			return DependencyPropertyManager.Register("RightDataAreaIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((DataViewBase)d).RebuildVisibleColumns()));
		}
		internal static DependencyProperty RegisterUseGroupShadowIndentProperty(Type ownerType) {
			return DependencyPropertyManager.Register("UseGroupShadowIndent", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).RebuildVisibleColumns()));
		}
		internal static DependencyProperty RegisterLeftDataAreaIndentProperty(Type ownerType) {
			return DependencyPropertyManager.Register("LeftDataAreaIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((DataViewBase)d).RebuildVisibleColumns()));
		}
		internal static DependencyProperty RegisterAutoWidthProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AutoWidth", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((ITableView)d).TableViewBehavior.OnAutoWidthChanged()));
		}
		internal static DependencyProperty RegisterFocusedRowBorderTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("FocusedRowBorderTemplate", typeof(ControlTemplate), ownerType);
		}
		internal static DependencyProperty RegisterColumnBandChooserTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ColumnBandChooserTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, (d,e) => ((DataViewBase)d).UpdateActualColumnChooserTemplate()));
		}
		internal static DependencyProperty RegisterAlternateRowBackgroundProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AlternateRowBackground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnAlternateRowPropertiesChanged()));
		}
		internal static DependencyPropertyKey RegisterActualAlternateRowBackgroundProperty(Type ownerType) {
			return DependencyPropertyManager.RegisterReadOnly("ActualAlternateRowBackground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnActualAlternateRowBackgroundChanged()));
		}
		internal static DependencyProperty RegisterEvenRowBackgroundProperty(Type ownerType) {
			return DependencyPropertyManager.Register("EvenRowBackground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnAlternateRowPropertiesChanged()));
		}
		internal static DependencyProperty RegisterUseEvenRowBackgroundProperty(Type ownerType) {
			return DependencyPropertyManager.Register("UseEvenRowBackground", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ITableView)d).TableViewBehavior.OnAlternateRowPropertiesChanged()));
		}
		internal static DependencyProperty RegisterAlternationCountProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AlternationCount", typeof(int), ownerType, new PropertyMetadata(2, (d, e) => ((ITableView)d).TableViewBehavior.OnActualAlternateRowBackgroundChanged()));
		}
		internal static DependencyProperty RegisterShowBandsPanelProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowBandsPanel", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowChangeColumnParentProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowChangeColumnParent", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowChangeBandParentProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowChangeBandParent", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterShowBandsInCustomizationFormProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ShowBandsInCustomizationForm", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowBandMovingProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowBandMoving", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowBandResizingProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowBandResizing", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowAdvancedVerticalNavigationProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowAdvancedVerticalNavigation", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowAdvancedHorizontalNavigationProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowAdvancedHorizontalNavigation", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterColumnChooserBandsSortOrderComparerProperty(Type ownerType) {
			return DependencyPropertyManager.Register("ColumnChooserBandsSortOrderComparer", typeof(IComparer<BandBase>), ownerType, new PropertyMetadata(DefaultColumnChooserBandsSortOrderComparer.Instance, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterBandHeaderTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("BandHeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterBandHeaderTemplateSelectorProperty(Type ownerType) {
			return DependencyPropertyManager.Register("BandHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterBandHeaderToolTipTemplateProperty(Type ownerType) {
			return DependencyPropertyManager.Register("BandHeaderToolTipTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterPrintBandHeaderStyleProperty(Type ownerType) {
			return DependencyPropertyManager.Register("PrintBandHeaderStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateBandsLayoutProperties()));
		}
		internal static DependencyProperty RegisterAllowConditionalFormattingMenuProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowConditionalFormattingMenu", typeof(bool), ownerType, new PropertyMetadata(null));
		}
		internal static DependencyProperty RegisterAllowConditionalFormattingManagerProperty(Type ownerType) {
			return DependencyPropertyManager.Register("AllowConditionalFormattingManager", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		internal static DependencyProperty RegisterPredefinedFormatsProperty(Type ownerType) {
			return DependencyPropertyManager.Register("PredefinedFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
		}
		internal static DependencyProperty RegisterPredefinedColorScaleFormatsProperty(Type ownerType) {
			return DependencyPropertyManager.Register("PredefinedColorScaleFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
		}
		internal static DependencyProperty RegisterPredefinedDataBarFormatsProperty(Type ownerType) {
			return DependencyPropertyManager.Register("PredefinedDataBarFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
		}
		internal static DependencyProperty RegisterPredefinedIconSetFormatsProperty(Type ownerType) {
			return DependencyPropertyManager.Register("PredefinedIconSetFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
		}
		protected internal override void UpdateBandsLayoutProperties() {
			if(DataControl == null || DataControl.BandsLayoutCore == null) return;
			DataControl.BandsLayoutCore.ShowBandsPanel = TableView.ShowBandsPanel;
			DataControl.BandsLayoutCore.AllowChangeColumnParent = TableView.AllowChangeColumnParent;
			DataControl.BandsLayoutCore.AllowChangeBandParent = TableView.AllowChangeBandParent;
			DataControl.BandsLayoutCore.ShowBandsInCustomizationForm = TableView.ShowBandsInCustomizationForm;
			DataControl.BandsLayoutCore.AllowBandMoving = TableView.AllowBandMoving;
			DataControl.BandsLayoutCore.AllowBandResizing = TableView.AllowBandResizing;
			DataControl.BandsLayoutCore.AllowAdvancedVerticalNavigation = TableView.AllowAdvancedVerticalNavigation;
			DataControl.BandsLayoutCore.AllowAdvancedHorizontalNavigation = TableView.AllowAdvancedHorizontalNavigation;
			DataControl.BandsLayoutCore.ColumnChooserBandsSortOrderComparer = TableView.ColumnChooserBandsSortOrderComparer;
			DataControl.BandsLayoutCore.BandHeaderTemplate = TableView.BandHeaderTemplate;
			DataControl.BandsLayoutCore.BandHeaderTemplateSelector = TableView.BandHeaderTemplateSelector;
			DataControl.BandsLayoutCore.BandHeaderToolTipTemplate = TableView.BandHeaderToolTipTemplate;
			DataControl.BandsLayoutCore.PrintBandHeaderStyle = TableView.PrintBandHeaderStyle;
			UpdateBandsIndicator();
		}
		internal override void NotifyBandsLayoutChanged() {
			UpdateViewRowData(rowData => rowData.UpdateCellsPanel());
		}
		internal override void NotifyFixedNoneBandsChanged() {
			UpdateViewRowData(rowData => rowData.UpdateClientFixedNoneBands());
		}
		internal override void NotifyFixedLeftBandsChanged() {
			UpdateViewRowData(rowData => rowData.UpdateClientFixedLeftBands());
		}
		internal override void NotifyFixedRightBandsChanged() {
			UpdateViewRowData(rowData => rowData.UpdateClientFixedRightBands());
		}
		static void AssignColumnPosition(ColumnBase column, int index, IList<ColumnBase> visibleColumns, bool showIndicator, bool isRootView, bool showDetailButtons) {
			column.ColumnPosition = GetColumnPosition(index, visibleColumns.Count, showIndicator, isRootView, showDetailButtons);
			column.IsLast = (index == visibleColumns.Count - 1);
			column.IsFirst = (index == 0);
		}
		static ColumnPosition GetColumnPosition(int index, int count, bool showIndicator, bool isRootView, bool showDetailButtons) {
			if(index == 0 && isRootView && !showIndicator && !showDetailButtons)
				return ColumnPosition.Left;
			return ColumnPosition.Middle;
		}
		internal static void OnIndicatorWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ITableView)d).TableViewBehavior.UpdateActualIndicatorWidth();
			((ITableView)d).ViewBase.RebuildVisibleColumns();
		}
		internal static void OnMultiSelectModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ITableView)d).ViewBase.OnMultiSelectModeChanged();
		}
		internal static void OnShowIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ITableView)d).TableViewBehavior.UpdateActualShowIndicator();
			((ITableView)d).TableViewBehavior.UpdateShowTotalSummaryIndicatorIndent((ITableView)d);
			((ITableView)d).TableViewBehavior.UpdateExpandColumnPosition();
			((ITableView)d).ViewBase.RebuildVisibleColumns();
		}
		internal static void OnFadeSelectionOnLostFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ITableView)d).TableViewBehavior.UpdateActualFadeSelectionOnLostFocus();
		}
		#endregion
		MouseMoveSelectionBase mouseMoveSelection;
		GridViewInfo viewInfo;
		DispatcherTimer scrollTimer;
		IList<ColumnBase> fixedLeftVisibleColumns;
		IList<ColumnBase> fixedRightVisibleColumns;
		IList<ColumnBase> fixedNoneVisibleColumns;
		internal double HorizontalOffset { get; private set; }
		protected TableViewBehavior(DataViewBase view) 
			: base(view) {
#if SL
			LastMousePosition = new Point(double.NaN, double.NaN);
			view.AddHandler(Control.MouseLeftButtonDownEvent, new MouseButtonEventHandler((d, e) => {
				if(MouseHelper.IsDoubleClick(e)) {
					OnDoubleClick(e);
				}
			}), true);
#endif
			FixedLeftVisibleColumns = new ObservableCollection<ColumnBase>();
			FixedRightVisibleColumns = new ObservableCollection<ColumnBase>();
			FixedNoneVisibleColumns = new ObservableCollection<ColumnBase>();
			scrollTimer = new DispatcherTimer();
			scrollTimer.Interval = TimeSpan.FromMilliseconds(10);
			scrollTimer.Tick += new EventHandler(OnScrollTimer_Tick);
			AutoFilterRowData = new AdditionalRowData(view.VisualDataTreeBuilder) { RowHandle = new RowHandle(DataControlBase.AutoFilterRowHandle) };
			BestFitCalculator = new DataControlBestFitCalculator(View);
		}
		internal override bool IsNavigationLocked { get { return !MouseMoveSelection.AllowNavigation; } }
		internal override bool IsAutoFilterRowFocused { get { return View.FocusedRowHandle == DataControlBase.AutoFilterRowHandle; } }
		internal override bool CanShowFixedColumnMenu { get { return TableView.AllowFixedColumnMenu; } }
		internal override bool AllowResizingCore { get { return TableView.AllowResizing; } }
		internal override bool UpdateAllowResizingOnWidthChanging { get { return DataControl == null || DataControl.BandsLayoutCore == null; } }
		internal override bool AutoWidthCore { get { return TableView.AutoWidth; } }
		internal override bool AllowColumnResizingCore { get { return true; } }
		internal override double HorizontalViewportCore { get { return TableView.HorizontalViewport; } }
		internal override bool AutoMoveRowFocusCore { get { return TableView.AutoMoveRowFocus; } }
		protected internal override FrameworkElement GetAdditionalRowElement(int rowHandle) {
			AdditionalRowItemsControl additionalRowItemsControl = AdditionalRowItemsControl;
			if(additionalRowItemsControl == null) return null;
			foreach(AdditionalRowContainerControlBase control in additionalRowItemsControl.Items) {
				if(control.RowHandle == rowHandle) {
					return control;
				}
			}
			return null;
		}
		internal double HorizontalVirtualizationOffset { get; private set; }
		internal double HorizontalHeaderVirtualizationOffset { get; private set; }
		internal void ResetHorizontalVirtualizationOffset() {
			HorizontalVirtualizationOffset = 0;
			HorizontalHeaderVirtualizationOffset = 0;
		}			
#if DEBUGTEST
		public void SetLastMousePositionDebugTest(Point point) {
			LastMousePosition = point;
		}
#endif
		protected internal virtual bool IsNewItemRowFocused { get { return false; } }
		public RowData AutoFilterRowData { get; private set; }
		internal NormalHorizontalNavigationStrategy HorizontalNavigationStrategy {
			get { return (NormalHorizontalNavigationStrategy)NavigationStrategyBase; }
		}
		internal override HorizontalNavigationStrategyBase NavigationStrategyBase {
			get {
				return TableView.AllowHorizontalScrollingVirtualization && !TableView.AutoWidth ?
					VirtualizedHorizontalNavigationStrategy.VirtualizedHorizontalNavigationStrategyInstance :
					NormalHorizontalNavigationStrategy.NormalHorizontalNavigationStrategyInstance;
			}
		}
		internal bool HasFixedLeftElements { get { return UpdateColumnsStrategy.HasFixedLeftElements(TableView); } }
		internal IList<ColumnBase> FixedLeftVisibleColumns { 
			get { return fixedLeftVisibleColumns; } 
			set { 
				fixedLeftVisibleColumns = value;
				TableView.SetFixedLeftVisibleColumns(fixedLeftVisibleColumns);
			} 
		}
		internal bool HasFixedRightElements { get { return UpdateColumnsStrategy.HasFixedRightElements(TableView); } }
		internal IList<ColumnBase> FixedRightVisibleColumns { 
			get { return fixedRightVisibleColumns; } 
			set { 
				fixedRightVisibleColumns = value;
				TableView.SetFixedRightVisibleColumns(fixedRightVisibleColumns);
			} 
		}
		internal IList<ColumnBase> FixedNoneVisibleColumns { 
			get { return fixedNoneVisibleColumns; } 
			set { 
				fixedNoneVisibleColumns = value;
				TableView.SetFixedNoneVisibleColumns(fixedNoneVisibleColumns);
			} 
		}
		internal double HorizontalExtent { get; set; }
		internal AdditionalRowItemsControl AdditionalRowItemsControl { get; set; }		
		internal double LeftIndent { get; set; }
		internal double RightIndent { get; set; }
		internal MouseMoveSelectionBase MouseMoveSelection {
			get {
				TableViewBehavior rootBehavior = View.RootView.ViewBehavior as TableViewBehavior;
				if(rootBehavior.mouseMoveSelection == null)
					return MouseMoveSelectionNone.Instance;
				return rootBehavior.mouseMoveSelection;
			}
		}
		public GridViewInfo ViewInfo {
			get {
				if (viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		internal ITableView TableView { get { return (ITableView)View; } }
		internal DataPresenterBase DataPresenter { get { return View.DataPresenter; } }
		internal DataPresenterBase RootDataPresenter { get { return View.RootDataPresenter; } }
		protected internal override DispatcherTimer ScrollTimer {
			get {
				return scrollTimer;
			}
		}
		protected internal override bool AllowCascadeUpdate { get { return TableView.AllowCascadeUpdate; } }
		protected internal override bool AllowPerPixelScrolling { get { return TableView.AllowPerPixelScrolling; } }
		protected internal override double ScrollAnimationDuration { get { return TableView.ScrollAnimationDuration; } }
		protected internal override ScrollAnimationMode ScrollAnimationMode { get { return TableView.ScrollAnimationMode; } }
		protected internal override bool AllowScrollAnimation { get { return TableView.AllowScrollAnimation; } }
		protected double TotalVisibleIndent { get { return ViewInfo.TotalGroupAreaIndent; } }
		protected internal virtual GridViewInfo CreateViewInfo() {
			return new GridViewInfo(View);
		}
		protected internal virtual GridViewInfo CreatePrintViewInfo() {
			return CreatePrintViewInfo(null);
		}
		protected internal virtual GridViewInfo CreatePrintViewInfo(BandsLayoutBase bandsLayout) {
			return new GridPrintViewInfo(View, bandsLayout);
		}
		internal override bool CheckNavigationStyle(int newValue) {
			return base.CheckNavigationStyle(newValue) && (newValue != DataControlBase.AutoFilterRowHandle);
		}
		internal override bool CanAdjustScrollbarCore() {
			return View.ScrollingMode == Xpf.Grid.ScrollingMode.Smart;
		}
		internal override void OnColumnResizerDoubleClick(ColumnBase column) {
			base.OnColumnResizerDoubleClick(column);
			BestFitColumnIfAllowed(column, DataControl.BandsCore.Count == 0);
		}
		void BestFitColumnIfAllowed(ColumnBase column, bool updateWidths) {
			if(CanBestFitColumn(column))
				BestFitColumnCore(column, updateWidths);
		}
		internal bool CanBestFitColumn(ColumnBase column) {
			return column.AllowResizing.GetValue(TableView.AllowResizing) && CanBestFitColumnCore(column);
		}
		internal override bool CanBestFitColumnCore(ColumnBase column) {
			return column.AllowBestFit.GetValue(TableView.AllowBestFit);
		}
		internal void BestFitColumn(object commandParameter) {
			ColumnBase column = View.GetColumnByCommandParameter(commandParameter);
			if(column != null)
				BestFitColumn(column);
		}
		internal void BestFitColumn(ColumnBase column) {
			BestFitColumnCore(column, DataControl.BandsCore.Count == 0);
		}
		bool IsBestFitControlDecoratorLoaded { get { return BestFitControlDecorator != null && UIElementHelper.IsVisibleInTree(BestFitControlDecorator); } }
		void BestFitColumnCore(ColumnBase column, bool updateWidths) {
			if(!IsBestFitControlDecoratorLoaded)
				return;
			ViewInfo.CreateColumnsLayoutCalculator().ApplyResize(column, CalcColumnBestFitWidthCore(column), double.MaxValue, 0, updateWidths);
		}
		internal Locker BestFitLocker = new Locker();
		internal void BestFitColumns() {
			if(!IsBestFitControlDecoratorLoaded)
				return;
			Dictionary<ColumnBase, double> widthsCache = new Dictionary<ColumnBase, double>();
			foreach(ColumnBase column in View.VisibleColumnsCore) {
				if(CanBestFitColumn(column))
					widthsCache[column] = CalcColumnBestFitWidthCore(column);
			}
			BestFitLocker.DoLockedAction(() => {
				View.BeginUpdateColumnsLayout();
				foreach(ColumnBase column in widthsCache.Keys) {
					ViewInfo.CreateColumnsLayoutCalculator().ApplyResize(column, widthsCache[column], double.MaxValue, 0, false);
				}
				View.EndUpdateColumnsLayout();
			});
		}
		protected internal override IndicatorState GetIndicatorState(RowData rowData) {
			int rowHandle = rowData.RowHandle.Value;
			if(rowHandle == DataControlBase.AutoFilterRowHandle)
				return IndicatorState.AutoFilterRow;
			if(BaseEditHelper.GetValidationError(rowData) != null) {
				return rowHandle == View.FocusedRowHandle && View.NavigationStyle != GridViewNavigationStyle.None ? IndicatorState.FocusedError : IndicatorState.Error;
			}
			bool isFocusedRow = rowHandle == View.FocusedRowHandle && (View.NavigationStyle != GridViewNavigationStyle.None || View.IsNewItemRowHandle(View.FocusedRowHandle));
			if(rowData.View.IsFocusedView && isFocusedRow) {
				if(View.IsFocusedRowModified)
					return IndicatorState.Changed;
				if(View.IsEditing)
					return IndicatorState.Editing;
				return IndicatorState.Focused;
			}
			if(View.IsNewItemRowHandle(rowHandle))
				return IndicatorState.NewItemRow;
			return IndicatorState.None;
		}
		public virtual void MakeColumnVisible(BaseColumn column) {
			if(RootDataPresenter == null)
				return;
			double left = -HorizontalOffset;
			left += GetActualColumnOffset(column);
			MakeRangeVisible(left, left + column.ActualHeaderWidth, UpdateColumnsStrategy.GetFixedStyle(column, this));
		}
		double GetActualColumnOffset(BaseColumn column) {
			if(DataControl.BandsLayoutCore != null)
				return GetBandOffset(column);
			else
				return GetColumnOffset((ColumnBase)column);
		}
		double GetColumnOffset(ColumnBase column) {
			double offset = 0;
			int columnIndex = 0;
			for(; columnIndex < View.VisibleColumnsCore.Count; columnIndex++) {
				if(View.VisibleColumnsCore[columnIndex] == column) {
					break;
				}
				offset += View.VisibleColumnsCore[columnIndex].ActualDataWidth;
			}
			if(FixedLeftVisibleColumns.Count > 0 && columnIndex >= FixedLeftVisibleColumns.Count)
				offset += TableView.FixedLineWidth;
			offset += TotalVisibleIndent;
			return offset;
		}
		double GetBandOffset(BaseColumn column) {
			double offset = 0;
			foreach(BandBase band in DataControl.BandsLayoutCore.GetBands(column.ParentBandInternal, true)) {
				offset += band.ActualHeaderWidth;
			}
			if(!column.IsBand) {
				for(int i = 0; i < column.BandRow.Columns.Count; i++) {
					if(column.BandRow.Columns[i] == column) break;
					offset += column.BandRow.Columns[i].ActualHeaderWidth;
				}
			}
			if(DataControl.BandsLayoutCore.FixedLeftVisibleBands.Count > 0)
				offset += TableView.FixedLineWidth;
			return offset;
		}
		public void MakeCurrentCellVisible() {
			DependencyObject currentCell = View.CurrentCell;
			if (currentCell == null || DataControlBase.FindCurrentView(currentCell) == null || !LayoutHelper.IsChildElement(View.RootView, currentCell))
				return;
			Rect rect = LayoutHelper.GetRelativeElementRect((UIElement)currentCell, RootDataPresenter);
			double indicatorWidth = GetTotalLeftIndent(true, false);
			MakeRangeVisible(rect.Left - indicatorWidth, rect.Right - indicatorWidth, TableViewProperties.GetFixedAreaStyle(currentCell));
		}
		void MakeRangeVisible(double left, double right, FixedStyle fixedStyle) {
			if (fixedStyle != FixedStyle.None)
				return;
			double leftInvisiblePart = left - LeftIndent;
			if (FixedLeftVisibleColumns.Count != 0)
				leftInvisiblePart -= TableView.FixedLineWidth;
			if (leftInvisiblePart < 0) {
				RootDataPresenter.SetHorizontalOffsetForce(HorizontalOffset + leftInvisiblePart);
				return;
			}
			double rightInvisiblePart = right - LeftIndent - HorizontalViewportCore;
			if(rightInvisiblePart > 0) {
				if((FixedRightVisibleColumns.Count > 0) || (FixedLeftVisibleColumns.Count != 0))
					rightInvisiblePart -= TableView.FixedLineWidth;
				double leftInvisiblePartAfterSetOffset = leftInvisiblePart - rightInvisiblePart;
				if (leftInvisiblePartAfterSetOffset < 0)
					rightInvisiblePart += leftInvisiblePartAfterSetOffset;
				RootDataPresenter.SetHorizontalOffsetForce(HorizontalOffset + rightInvisiblePart);
			}
		}
		internal double GetTotalLeftIndent(bool includeIndicator, bool includeCurrentGroupAreaIndentWhenDetailButtonVisible) {
			double indent = (includeIndicator && TableView.ShowIndicator ? TableView.IndicatorWidth : 0);
			DataControlBase originationDataControl = View.DataControl.GetOriginationDataControl();
			originationDataControl.EnumerateThisAndOwnerDataControls(dataControl => {
				ITableView tableView = (ITableView)dataControl.DataView;
				indent += dataControl != originationDataControl ? tableView.ActualDetailMargin.Left : 0;
				if((includeCurrentGroupAreaIndentWhenDetailButtonVisible && tableView.ActualShowDetailButtons) || dataControl != originationDataControl)
					indent += tableView.TableViewBehavior.ViewInfo.TotalGroupAreaIndent;
			});
			if(TableView.ActualShowDetailButtons)
				indent += TableView.ActualExpandDetailButtonWidth;
			return indent;
		}
		internal double GetTotalRightIndent() {
			double indent = 0;
			DataControlBase originationDataControl = View.DataControl.GetOriginationDataControl();
			int level = -1;
			originationDataControl.EnumerateThisAndOwnerDataControls(dataControl => {
				level++;
				ITableView tableView = (ITableView)dataControl.DataView;
				indent += dataControl != originationDataControl && tableView.ActualShowDetailButtons ? tableView.ActualDetailMargin.Right : 0;
			});
			return indent;
		}
		protected internal override bool OnVisibleColumnsAssigned(bool changed) {
			changed = UpdateFixedColumns(changed);
			if(DataPresenter != null)
				DataPresenter.UpdateSecondarySizeScrollInfo();
			View.DataControl.ResetGridChildPeersIfNeeded();
			return changed;
		}
		bool UpdateFixedColumns(bool changed) {
			IList<ColumnBase> fixedLeftVisibleColumnsList = GetFixedLeftVisibleColumns();
			if(!ListHelper.AreEqual(FixedLeftVisibleColumns, fixedLeftVisibleColumnsList)) {
				FixedLeftVisibleColumns = fixedLeftVisibleColumnsList;
				changed = true;
			}
			IList<ColumnBase> fixedRightVisibleColumnsList = GetFixedRightVisibleColumns();
			if(!ListHelper.AreEqual(FixedRightVisibleColumns, fixedRightVisibleColumnsList)) {
				FixedRightVisibleColumns = fixedRightVisibleColumnsList;
				changed = true;
			}
			IList<ColumnBase> fixedNoneVisibleColumnsList = GetFixedNoneVisibleColumns();
			if(!ListHelper.AreEqual(FixedNoneVisibleColumns, fixedNoneVisibleColumnsList)) {
				FixedNoneVisibleColumns = fixedNoneVisibleColumnsList;
				changed = true;
			}
			return changed;
		}
		IList<ColumnBase> GetFixedLeftVisibleColumns() {
			return GetFixedVisibleColumns(FixedStyle.Left);
		}
		IList<ColumnBase> GetFixedRightVisibleColumns() {
			return GetFixedVisibleColumns(FixedStyle.Right);
		}
		IList<ColumnBase> GetFixedNoneVisibleColumns() {
			return GetFixedVisibleColumns(FixedStyle.None);
		}
		UpdateColumnsStrategyBase UpdateColumnsStrategy {
			get {
				if(DataControl != null && DataControl.BandsLayoutCore != null) return BandedViewUpdateColumnsStrategy.Instance;
				return TableViewUpdateColumnsStrategy.Instance;
			}
		}
		IList<ColumnBase> GetFixedVisibleColumns(FixedStyle fixedStyle) {
			return UpdateColumnsStrategy.GetFixedVisibleColumns(fixedStyle, View);
		}
		internal void UpdateColumnDataWidths(out double totalFixedNoneSize, out double totalFixedLeftSize, out double totalFixedRightSize) {
			UpdateColumnsStrategy.UpdateColumnDataWidths(View, ViewInfo, out totalFixedNoneSize, out totalFixedLeftSize, out totalFixedRightSize);
		}
		internal void FillByLastFixedColumn(double arrangeWidth) {
			UpdateColumnsStrategy.FillByLastFixedColumn(this, arrangeWidth);
		}
		internal void UpdateViewportVisibleColumnsCore() {
			if(RootDataPresenter == null)
				return;
			List<ColumnBase> viewportColumns = UpdateColumnsStrategy.GetViewportVisibleColumns(this);
			if (TableView.ViewportVisibleColumns == null || !ListHelper.AreEqual(TableView.ViewportVisibleColumns, viewportColumns)) {
				TableView.ViewportVisibleColumns = viewportColumns;
				UpdateVirtualizedData();
			}
		}
		internal override void UpdateViewportVisibleColumns() {
			HorizontalNavigationStrategy.UpdateViewportVisibleColumns(this);
			TableView.ScrollingVirtualizationMargin = UpdateColumnsStrategy.GetScrollingMargin(TableView, -HorizontalOffset, HorizontalVirtualizationOffset);
			TableView.ScrollingHeaderVirtualizationMargin = UpdateColumnsStrategy.GetScrollingMargin(TableView, -HorizontalOffset, HorizontalHeaderVirtualizationOffset);
		}
		protected internal void UpdateVirtualizedData() {
			View.HeadersData.UpdateFixedNoneCellData(true);
			UpdateRowData(rowData => rowData.UpdateFixedNoneCellData(true));
			UpdateServiceRowData(rowData => rowData.UpdateFixedNoneCellData(true));
		}
		protected internal override KeyValuePair<DataViewBase, int> GetViewAndVisibleIndex(double verticalOffset) {
			FrameworkElement firstVisibleRow = RootDataPresenter.GetFirstVisibleRow();
			if(firstVisibleRow != null)
				verticalOffset += RootDataPresenter.ActualScrollOffset % 1 * firstVisibleRow.DesiredSize.Height;
			double offset = 0;
			GridRowsEnumerator en = View.RootView.CreateVisibleRowsEnumerator();
			RowDataBase rowData = null;
			RowNode rowNode = null;
			while(en.MoveNext()) {
				rowNode = en.CurrentNode;
				rowData = en.CurrentRowData;
				if(rowNode == null)
					continue;
				FrameworkElement rowElement = en.CurrentRow;
				if(rowElement == null)
					continue;
				offset += rowElement.ActualHeight;
				bool isAboveViewport = verticalOffset < 0;
				bool isOverRow = verticalOffset < offset;
				bool isBelowViewport = offset > View.RootView.ScrollContentPresenter.ActualHeight;
				if(isBelowViewport || isAboveViewport || isOverRow) {
					if(rowNode.MatchKey is GroupSummaryRowKey)
						return new KeyValuePair<DataViewBase, int>(View, -1);
					break;
				}
			}
			if(rowData == null)
				return new KeyValuePair<DataViewBase, int>(View, -1);
			DataViewBase view = rowData.View;
			int visibleIndex = -1;
			if(rowNode is DataRowNode)
				visibleIndex = view.DataControl.GetRowVisibleIndexByHandleCore(((DataRowNode)rowNode).RowHandle.Value);
			return new KeyValuePair<DataViewBase, int>(view, visibleIndex);
		}
		internal ColumnBase GetColumn(double offset) {
			if(TableView.ShowIndicator)
				offset -= TableView.IndicatorWidth;
			if (offset < -DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset)
				return View.VisibleColumnsCore[0];			
			double current = ViewInfo.TotalGroupAreaIndent;
			foreach (ColumnBase column in FixedLeftVisibleColumns) {
				if (offset <= current + column.ActualDataWidth)
					return column;
				current += column.ActualDataWidth;
			}
			if (FixedLeftVisibleColumns.Count != 0)
				current += TableView.FixedLineWidth;
			double nonFixedOffset = current - DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset;
			foreach (ColumnBase column in FixedNoneVisibleColumns) {
				bool outOfBounds = offset - nonFixedOffset - DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset > DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Viewport + TableView.FixedLineWidth;
				if (FixedRightVisibleColumns.Count != 0 && outOfBounds)
					break;
				if (offset <= nonFixedOffset + column.ActualDataWidth)
					return column;
				nonFixedOffset += column.ActualDataWidth;
			}
			current += DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Viewport + TableView.FixedLineWidth;
			foreach (ColumnBase column in FixedRightVisibleColumns) {
				if (offset <= current + column.ActualDataWidth)
					return column;
				current += column.ActualDataWidth;
			}
			return View.VisibleColumnsCore[View.VisibleColumnsCore.Count - 1];
		}
		protected internal override Size CorrectMeasureResult(double scrollOffset, Size constraint, Size result) {
			if ((scrollOffset > 0 || View.ScrollingMode == ScrollingMode.Normal) && !double.IsPositiveInfinity(constraint.Height) && result.Height < constraint.Height)
				result.Height = constraint.Height;
			result.Width = Math.Min(constraint.Width, Math.Max(result.Width, HorizontalExtent + TableView.TotalGroupAreaIndent + (TableView.ActualShowIndicator ? TableView.IndicatorWidth : 0)));
			return result;
		}
		protected internal override void UpdateCellData(ColumnsRowDataBase rowData) {
			rowData.CellData = rowData.CreateCellDataList();
			rowData.UpdateFixedLeftCellData();
			this.HorizontalNavigationStrategy.UpdateFixedNoneCellData(rowData, this);
			rowData.UpdateFixedRightCellData();
		}
		internal void OnAutoWidthChanged() {
			View.RebuildVisibleColumns();
			if(View.IsLockUpdateColumnsLayout)
				return;
			UpdateCellData();
			if (View.DataPresenter != null)
				View.DataPresenter.UpdateAutoSize();
		}
		internal virtual MouseMoveSelectionBase GetMouseMoveSelection(IDataViewHitInfo hitInfo) {
			if(!View.AllowMouseMoveSelection || DataControl.SelectionMode == MultiSelectMode.MultipleRow)
				return MouseMoveSelectionNone.Instance;
			ITableViewHitInfo tableViewHitInfo = (ITableViewHitInfo)hitInfo;
			if(View.ShowSelectionRectangle && TableView.ViewBase.IsMultiSelection && (hitInfo.IsRowCell || (hitInfo.InRow && !tableViewHitInfo.IsRowIndicator && TableView.ViewBase.IsMultiRowSelection)) && !View.ViewBehavior.IsAdditionalRow(hitInfo.RowHandle))
				if(TableView.ViewBase.IsMultiCellSelection)
					return MouseMoveSelectionRectangleGridCell.Instance;
				else
					return MouseMoveSelectionRectangleRowIndicator.Instance;
			if(tableViewHitInfo.IsRowIndicator && TableView.ViewBase.IsMultiSelection && TableView.UseIndicatorForSelection && !View.ViewBehavior.IsAdditionalRow(hitInfo.RowHandle)
				&& !ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers) && !ModifierKeysHelper.IsShiftPressed(Keyboard.Modifiers))
				return MouseMoveSelectionRowIndicator.Instance;
			if (hitInfo.IsRowCell && TableView.ViewBase.IsMultiCellSelection && !View.ViewBehavior.IsAdditionalRow(hitInfo.RowHandle))
				return MouseMoveStrategyGridCell.Instance;
			return null;
		}
		internal override bool IsRowIndicator(DependencyObject originalSource) {
			ITableViewHitInfo hitInfo = TableView.CalcHitInfo(originalSource);
			return hitInfo.IsRowIndicator;
		}
		protected internal override void StopSelection() {
			MouseMoveSelection.OnMouseUp(View);
			SetMouseMoveSelectionStrategy(null);
			MouseMoveSelection.ReleaseMouseCapture(View);
		}
		public virtual void OnScrollTimer_Tick(object sender, EventArgs e) {
			if(View == null || View.ScrollContentPresenter == null)
				return;
			if(LastMousePosition.X == double.NaN || LastMousePosition.Y == double.NaN) return;
			int vScrollDelta = 0, hScrollDelta = 0;
			DragScroll();
			if(MouseMoveSelection.CanScrollVertically) {
				if(LastMousePosition.Y < 0)
					vScrollDelta = -1;
				if(LastMousePosition.Y > View.ScrollContentPresenter.ActualHeight)
					vScrollDelta = 1;
			}
			if(MouseMoveSelection.CanScrollHorizontally) {
				double indicatorWidth = TableView.ShowIndicator ? TableView.IndicatorWidth : 0;
				if(LastMousePosition.X - indicatorWidth < 0)
					hScrollDelta = -10;
				if(LastMousePosition.X > View.ScrollContentPresenter.ActualWidth)
					hScrollDelta = 10;
			}
			if(hScrollDelta != 0)
				ChangeHorizontalOffsetBy(hScrollDelta);
			if(vScrollDelta != 0)
				ChangeVerticalOffsetBy(vScrollDelta);
			if(vScrollDelta != 0 || hScrollDelta != 0)
				View.EnqueueImmediateAction(() => InvalidateSelection());
		}
		void DragScroll() {
			if(!DragDropScroller.IsDragging(View)) return;
			if(LastMousePosition.X - TableView.IndicatorWidth < 0)
				ChangeHorizontalOffsetBy(-10);
			if(LastMousePosition.X > View.ScrollContentPresenter.ActualWidth)
				ChangeHorizontalOffsetBy(10);
		}
		internal void InvalidateSelection() {
			MouseMoveSelection.UpdateSelection(TableView);
		}
		protected internal override int? VisibleComparisonCore(BaseColumn x, BaseColumn y) {
			if(x.Fixed != y.Fixed) {
				if(x.Fixed == FixedStyle.Left)
					return -1;
				if(y.Fixed == FixedStyle.Left)
					return 1;
				if(x.Fixed == FixedStyle.Right)
					return 1;
				if(y.Fixed == FixedStyle.Right)
					return -1;
			}
			return null;
		}
		protected internal override void OnEditorActivated() {
			base.OnEditorActivated();
			UpdateFocusedRowIndicator();
		}
		protected internal override void OnHideEditor(CellEditorBase editor) {
			base.OnHideEditor(editor);
			UpdateFocusedRowIndicator();
		}
		protected internal override void OnCancelRowEdit() {
			base.OnCancelRowEdit();
			UpdateFocusedRowIndicator();
		}
		protected internal override void OnFocusedRowCellModified() {
			base.OnFocusedRowCellModified();
			UpdateFocusedRowIndicator();
		}
		protected internal override void OnTopRowIndexChangedCore() {
			DataPresenter.SetVerticalOffsetForce(View.TopRowIndex);
		}
		protected internal override bool EndRowEdit() {
			bool result = base.EndRowEdit();
			UpdateFocusedRowIndicator();
			return result;
		}
		void UpdateFocusedRowIndicator() {
			RowData rowData = View.ViewBehavior.GetRowData(View.FocusedRowHandle);
			if(rowData != null) rowData.UpdateIndicatorState();
		}
		internal override void AssignColumnPosition(ColumnBase column, int index, IList<ColumnBase> visibleColumns) {
			AssignColumnPosition(column, index, visibleColumns, TableView.ShowIndicator, View.IsRootView, TableView.ActualShowDetailButtons);
		}
		internal override void UpdateColumnsPosition(ObservableCollection<ColumnBase> visibleColumnsList) {
			bool hasBands = DataControl.BandsLayoutCore != null;
			for(int i = 0; i < visibleColumnsList.Count; i++) {
				ColumnBase col = visibleColumnsList[i];
				ColumnBase.SetVisibleIndex(col, i);
				if(!hasBands)
					AssignColumnPosition(col, i, visibleColumnsList);
			}
		}
		protected internal override IList<ColumnBase> RebuildVisibleColumnsCore() {
			if(DataControl.BandsLayoutCore != null)
				return DataControl.BandsLayoutCore.RebuildBandRows();
			return base.RebuildVisibleColumnsCore();
		}
		internal override int GetValueForSelectionAnchorRowHandle(int value) {
			if(value == DataControlBase.AutoFilterRowHandle) {
				return DataControl.GetRowHandleByVisibleIndexCore(0);
			}
			return base.GetValueForSelectionAnchorRowHandle(value);
		}
		protected internal override void UpdateLastPostition(Xpf.Core.IndependentMouseEventArgs e) {
			if(View.ScrollContentPresenter == null) return;
			LastMousePosition = e.GetPosition(View.ScrollContentPresenter);
		}
		internal override void UpdateSecondaryScrollInfoCore(double secondaryOffset, bool allowUpdateViewportVisibleColumns) {
			double newValue = -secondaryOffset;
			if(HorizontalOffset != newValue) {
				HorizontalOffset = newValue;
				if(allowUpdateViewportVisibleColumns)
					UpdateViewportVisibleColumns();
			}
		}
		void OnScrollingVirtualizationMarginChanged() {
			UpdateViewRowData(x => x.UpdateClientScrollingMargin());
		}
		void OnShowHorizontalLinesChanged() {
			View.UpdateContentLayout();
			UpdateViewRowData(x => x.UpdateClientHorizontalLineVisibility());
		}
		void OnShowVerticalLinesChanged() {
			View.UpdateContentLayout();
			UpdateViewRowData(x => x.UpdateClientVerticalLineVisibility());
		}
		void OnFixedLineWidthChanged() {
			View.RebuildVisibleColumns();
			UpdateViewRowData(x => x.UpdateClientFixedLineWidth());
		}
		void OnFixedVisibleColumnsChanged() {
			UpdateViewRowData(x => x.UpdateClientFixedLineVisibility());
		}
		void OnActualIndicatorWidthChanged() {
			UpdateViewRowData(x => x.UpdateClientIndicatorWidth());
		}
		void OnActualShowIndicatorChanged() {
			UpdateViewRowData(x => x.UpdateClientShowIndicator());
			UpdateBandsIndicator();
		}
		void UpdateBandsIndicator() {
			if(DataControl != null && DataControl.BandsLayoutCore != null)
				DataControl.BandsLayoutCore.UpdateShowIndicator(TableView.ActualShowIndicator);
		}
		void OnRowMinHeightChanged() {
			UpdateViewRowData(x => x.UpdateClientMinHeight());
		}
		void OnRowIndicatorContentTemplateChanged() {
			UpdateViewRowData(x => x.UpdateIndicatorContentTemplate());
		}
		void OnActualDataRowTemplateSelectorChanged() {
			UpdateViewRowData(x => x.UpdateContent());
		}
		protected internal override void UpdateAdditionalRowsData() {
			if(TableView.ShowAutoFilterRow) AutoFilterRowData.UpdateData();
		}
		protected internal override void UpdateColumnsViewInfo(bool updateDataPropertiesOnly) {
			base.UpdateColumnsViewInfo(updateDataPropertiesOnly);
			if(DataControl != null && DataControl.BandsLayoutCore != null)
				DataControl.BandsLayoutCore.ForeachBand(b => b.UpdateViewInfo(updateDataPropertiesOnly));
		}
		DependencyObject autoFilterRowState = new RowStateObject();
		protected internal override DependencyObject GetRowState(int rowHandle) {
			if(rowHandle == DataControlBase.AutoFilterRowHandle)
				return autoFilterRowState;
			return null;
		}
		internal void OnAllowHorizontalScrollingVirtualizationChanged() {
			View.RebuildVisibleColumns();
			View.UpdateCellData();
		}
		internal override void ProcessPreviewKeyDown(KeyEventArgs e) {
			base.ProcessPreviewKeyDown(e);
			View.EditFormManager.OnPreviewKeyDown(e);
			if(e.Key == Key.Delete && ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && View.IsAutoFilterRowFocused && DataControl.CurrentColumn != null) {
				DataControl.ClearColumnFilter(DataControl.CurrentColumn);
			}
		}
		protected internal override void OnGotKeyboardFocus() {
			base.OnGotKeyboardFocus();
			if(View.DataProviderBase.DataRowCount == 0 && View.FocusedRowHandle == DataControlBase.InvalidRowHandle) {
				if(TableView.ShowAutoFilterRow) {
					View.SetFocusedRowHandle(DataControlBase.AutoFilterRowHandle);
				}
				else if(IsNewItemRowVisible || View.ShouldDisplayBottomRow) {
					View.SetFocusedRowHandle(DataControlBase.NewItemRowHandle);
				}
			}
		}
		internal override GridViewNavigationBase CreateAdditionalRowNavigation() {
			return new AdditionalRowNavigation(View);
		}
		internal override GridViewNavigationBase CreateRowNavigation() {
			return new GridViewRowNavigation(View);
		}
		internal override GridViewNavigationBase CreateCellNavigation() {
			return new GridViewCellNavigation(View);
		}
		protected internal override void UpdateRowData(UpdateRowDataDelegate updateMethod, bool updateInvisibleRows = true, bool updateFocusedRow = true) {
			base.UpdateRowData(updateMethod, updateInvisibleRows, updateFocusedRow);
			if(updateInvisibleRows || TableView.ShowAutoFilterRow) updateMethod(AutoFilterRowData);
		}
		protected internal override void UpdateServiceRowData(Action<ColumnsRowDataBase> updateMethod) {
			if(View.DataControl == null)
				return;
			foreach(ColumnsRowDataBase item in View.DataControl.DataControlParent.GetColumnsRowDataEnumerator()) {
				updateMethod(item);
			}
		}
		protected internal override double CalcColumnMaxWidth(ColumnBase column) {
			return ViewInfo.CreateColumnsLayoutCalculator().CalcColumnMaxWidth(column);
		}
		protected internal override void ResetHeadersChildrenCache() {
			View.ResetHeadersChildrenCache();
		}
		internal override void OnResizingComplete() {
			View.DesignTimeAdorner.OnColumnResized();
		}
		protected internal override void ApplyResize(BaseColumn column, double value, double maxWidth) {
			ViewInfo.CreateColumnsLayoutCalculator().ApplyResize(column, value, maxWidth);
		}
		protected internal override Point CorrectDragIndicatorLocation(UIElement panel, Point point) {
			ITableView rootView = (ITableView)View.RootView;
			Rect rect = LayoutHelper.GetRelativeElementRect(panel, rootView.ViewBase);
			double leftIndent = GetTotalLeftIndent(true, true);
			if(point.X < leftIndent - rect.Left)
				return new Point(leftIndent - rect.Left, point.Y);
			if(rootView.ViewBase.ActualWidth - GetVerticalScrollBarWidth() < rect.Left + point.X)
				return new Point(rootView.ViewBase.ActualWidth - GetVerticalScrollBarWidth() - rect.Left, point.Y);
			return point;
		}
		protected internal override double CalcVerticalDragIndicatorSize(UIElement panel, Point point, double width) {
			ITableView rootView = (ITableView)View.RootView;
			double correctedWidth = width;
			Rect rect = LayoutHelper.GetRelativeElementRect(panel, rootView.ViewBase);
			double leftIndent = GetTotalLeftIndent(true, true);
			if(point.X < leftIndent - rect.Left) {
				correctedWidth -= leftIndent - rect.Left - point.X;
			}
			if(rootView.ViewBase.ActualWidth - GetVerticalScrollBarWidth() - rect.Left < point.X + width) {
				correctedWidth -= point.X + width - rootView.ViewBase.ActualWidth + GetVerticalScrollBarWidth() + rect.Left;
			}
			return correctedWidth;
		}
		internal virtual void UpdateActualDataRowTemplateSelector() {
			DataTemplate template = TableView.DataRowTemplate;
#if !SL
			if(UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row) && template is DefaultDataTemplate)
				template = null;
#endif
			View.UpdateActualTemplateSelector(TableView.ActualDataRowTemplateSelectorPropertyKey, TableView.DataRowTemplateSelector, template);
		}
		protected internal virtual void OnRowDecorationTemplateChanged() {
		}
		protected internal override RowData GetRowData(int rowHandle) {
			if(rowHandle == DataControlBase.AutoFilterRowHandle)
				return AutoFilterRowData;
			return base.GetRowData(rowHandle);
		}
		void OnFixedNoneContentWidthChanged() {
			UpdateServiceRowData(rowData => UpdateFixedNoneContentWidth(rowData));
			UpdateFixedNoneContentWidth(View.HeadersData);
			View.UpdateRowData(rowData => UpdateFixedNoneContentWidth(rowData));
		}
		void OnFixedLeftContentWidthChanged() { }
		void OnFixedRightContentWidthChanged() { }
		void OnTotalGroupAreaIndentChanged() { }
		protected internal override void UpdateFixedNoneContentWidth(ColumnsRowDataBase rowData) {
			rowData.FixedNoneContentWidth = Math.Max(0, rowData.GetFixedNoneContentWidth(TableView.FixedNoneContentWidth));
		}
		internal override void UpdateAdditionalFocusedRowData() {
			UpdateAdditionalFocusedRowDataCore();
			View.RaiseFocusedRowHandleChanged();
		}
		protected virtual void UpdateAdditionalFocusedRowDataCore() {
			if(View.IsAutoFilterRowFocused)
				View.FocusedRowData = AutoFilterRowData;
		}
		internal override void UpdateColumnsLayout() {
			if(DataControl.BandsLayoutCore == null) {
				UpdateHasLeftRightSibling();
			}
			ViewInfo.CalcColumnsLayout();
		}
		void UpdateHasLeftRightSibling() {
			ViewInfo.CreateColumnsLayoutCalculator().UpdateHasLeftRightSibling(FixedLeftVisibleColumns);
			ViewInfo.CreateColumnsLayoutCalculator().UpdateHasLeftRightSibling(FixedRightVisibleColumns);
			ViewInfo.CreateColumnsLayoutCalculator().UpdateHasLeftRightSibling(FixedNoneVisibleColumns);
		}
		internal void OnShowAutoFilterRowChanged() {
			if(View.DataControl != null)
				View.DataControl.ValidateMasterDetailConsistency();
			if(TableView.ShowAutoFilterRow && View.DataControl != null)
				AutoFilterRowData.UpdateData();
			else if(View.IsAutoFilterRowFocused)
				View.SetFocusedRowHandle(DataControl.GetRowHandleByVisibleIndexCore(0));
		}
		internal override void MakeCellVisible() {
			HorizontalNavigationStrategy.MakeCellVisible(this);
		}
		internal override void MovePrevRow() {
			MovePrevRow(allowNavigateToAutoFilterRow: false);
		}
		internal void MovePrevRow(bool allowNavigateToAutoFilterRow) {
			if(MovePrevRowCore()) return;
			if(View.DataProviderBase.CurrentIndex == 0 && CanNavigateToAdditionalRow(allowNavigateToAutoFilterRow) && !View.IsAdditionalRowFocused) {
				View.SetFocusedRowHandle(GetAdditionalRowHandle());
				return;
			}
			if(IsNewItemRowFocused && CanNavigateToAutoFilterRow(allowNavigateToAutoFilterRow)) {
				View.SetFocusedRowHandle(DataControlBase.AutoFilterRowHandle);
				return;
			}
			DataControl.NavigateToMasterRow();
		}
		internal override void MoveNextRow() {
			if(MoveNextRowCore() || !View.CanMoveFromFocusedRow()) return;
			if(View.IsAdditionalRowFocused) {
				if(IsAutoFilterRowFocused && IsNewItemRowVisible) {
					View.SetFocusedRowHandle(DataControlBase.NewItemRowHandle);
					return;
				}
				if(View.IsTopNewItemRowFocused)
					View.CommitEditing();
				if(DataControl.VisibleRowCount > 0) {
					View.SetFocusedRowHandle(DataControl.GetRowHandleByVisibleIndexCore(0));
					return;
				}
			}
			if(View.IsBottomNewItemRowFocused) {
				if(IsNewItemRowEditing && View.CommitEditing()) {
					View.ScrollIntoView(View.FocusedRowHandle);
					return;
				}
			}
			DataControl.NavigateToNextOuterMasterRow();
		}
		bool CanNavigateToAutoFilterRow(bool allowNavigateToAutoFilterRow) {
			return allowNavigateToAutoFilterRow && TableView.ShowAutoFilterRow;
		}
		protected virtual bool CanNavigateToAdditionalRow(bool allowNavigateToAutoFilterRow) {
			return CanNavigateToAutoFilterRow(allowNavigateToAutoFilterRow);
		}
		protected virtual int GetAdditionalRowHandle() {
			return DataControlBase.AutoFilterRowHandle;
		}
		internal override void OnViewMouseLeave() {
			MouseMoveSelection.CaptureMouse(View);
		}
		internal override void OnViewMouseMove(MouseEventArgs e) {
			LastMousePosition = e.GetPosition(View.ScrollContentPresenter);
			InvalidateSelection();
		}
		internal void UpdateSelectionRectCore(int rowHandle, ColumnBase column) {
			View.SelectionStrategy.UpdateSelectionRect(rowHandle, column);
		}
		internal override void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonUp(e);
			StopSelection();
		}
		internal override void OnMouseLeftButtonUp() {
			StopSelection();
		}
		internal override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			base.OnAfterMouseLeftButtonDown(hitInfo);
			if(!TableView.IsEditing && Mouse.RightButton == MouseButtonState.Released) {
				SetMouseMoveSelectionStrategy(GetMouseMoveSelection(hitInfo));
				MouseMoveSelection.OnMouseDown(View, hitInfo);
			}
		}
		internal override void EnsureSurroundingsActualSize(Size finalSize) {
			ViewInfo.ColumnsLayoutSize = finalSize;
		}
		double GetVerticalScrollBarWidth() {
			return View.IsTouchScrollBarsMode ? 0 : ViewInfo.VerticalScrollBarWidth;
		}
		internal override void SetVerticalScrollBarWidth(double width) {
			ViewInfo.VerticalScrollBarWidth = width;
		}
		protected internal override double GetFixedExtent() { return HorizontalExtent; }
		internal override void OnCellContentPresenterRowChanged(FrameworkElement presenter) {
			DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(presenter, View.DataControlMenu);
			DataControlPopupMenu.SetGridMenuType(presenter, GridMenuType.RowCell);
		}
		internal void SetHorizontalViewport(double value) {
			if(TableView.HorizontalViewport == value)
				return;
			TableView.SetHorizontalViewport(value);
			UpdateViewportVisibleColumns();
		}
		internal override void OnDoubleClick(MouseButtonEventArgs e) {
			base.OnDoubleClick(e);
			View.EditFormManager.OnDoubleClick(e);
			ITableViewHitInfo hitInfo = ((ITableView)View.RootView).CalcHitInfo(e.OriginalSource as DependencyObject);
			if(hitInfo.InRow)
				TableView.RaiseRowDoubleClickEvent(hitInfo
#if !SL
					, e.ChangedButton
#endif
					);
		}
		protected internal override double FirstColumnIndent { get { return ViewInfo.FirstColumnIndent; } }
		protected internal override double NewItemRowIndent { get { return ViewInfo.NewItemRowIndent; } }
		#region best fit
		Decorator bestFitControlDecorator;
		public Decorator BestFitControlDecorator { get { return ((ITableView)View.RootView).TableViewBehavior.bestFitControlDecorator; } set { bestFitControlDecorator = value; } }
		internal DataControlBestFitCalculator BestFitCalculator { get; private set; }
		internal override bool CanBestFitAllColumns() {
			return TableView.AllowBestFit && TableView.AllowResizing;
		}
		internal bool CanBestFitColumns() {
			return TableView.AllowBestFit;
		}
		internal double CalcColumnBestFitWidthCore(ColumnBase column) {
			InplaceEditorBase oldCellEditor = View.CurrentCellEditor;
			try {
				return BestFitCalculator.CalcColumnBestFitWidth(column);
			} finally {
				View.CurrentCellEditor = oldCellEditor;
			}
		}
		internal abstract GridColumnHeaderBase CreateGridColumnHeader();
		internal abstract BestFitControlBase CreateBestFitControl(ColumnBase column);
		internal abstract FrameworkElement CreateGridTotalSummaryControl();
		internal abstract FrameworkElement CreateGroupFooterSummaryControl();
		internal abstract GridColumnData GetGroupSummaryColumnData(int rowHandle, IBestFitColumn column);
		internal abstract CustomBestFitEventArgsBase RaiseCustomBestFit(ColumnBase column, BestFitMode bestFitMode);
		internal abstract int GetTopRow(int pageVisibleTopRowIndex);
		#endregion
		#region CellSelection
		protected internal void SelectCell(int rowHandle, ColumnBase column) {
			if (!IsValidRowHandleAndColumn(rowHandle, column))
				return;
			View.SelectionStrategy.SelectCell(rowHandle, column);
		}
		protected internal void UnselectCell(int rowHandle, ColumnBase column) {
			if (!IsValidRowHandleAndColumn(rowHandle, column))
				return;
			View.SelectionStrategy.UnselectCell(rowHandle, column);
		}
		protected internal void SelectCells(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			if (!IsValidRowHandleAndColumn(startRowHandle, startColumn) || !IsValidRowHandleAndColumn(endRowHandle, endColumn))
				return;
			View.SelectionStrategy.SetCellsSelection(startRowHandle, startColumn, endRowHandle, endColumn, true);
		}
		protected internal void UnselectCells(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			if (!IsValidRowHandleAndColumn(startRowHandle, startColumn) || !IsValidRowHandleAndColumn(endRowHandle, endColumn))
				return;
			View.SelectionStrategy.SetCellsSelection(startRowHandle, startColumn, endRowHandle, endColumn, false);
		}
		public bool IsCellSelected(int rowHandle, ColumnBase column) {
			if (!IsValidRowHandleAndColumn(rowHandle, column))
				return false;
			return View.SelectionStrategy.IsCellSelected(rowHandle, column);
		}
		protected internal override bool GetIsCellSelected(int rowHandle, ColumnBase column) {
			return View.SelectionStrategy.IsCellSelected(rowHandle, column);
		}
		public IList<CellBase> GetSelectedCells() {
			return View.SelectionStrategy.GetSelectedCells();
		}
		#endregion
		#region CopyRows
		internal void CopyCellsToClipboard(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			if (!IsValidRowHandleAndColumn(startRowHandle, startColumn) || !IsValidRowHandleAndColumn(endRowHandle, endColumn))
				return;
			List<CellBase> gridCells = new List<CellBase>();
			IterateCells(startRowHandle, startColumn.VisibleIndex, endRowHandle, endColumn.VisibleIndex,
				delegate(int rowHandle, ColumnBase column) {
					gridCells.Add(TableView.CreateGridCell(rowHandle, column));
				});
		   TableView.CopyCellsToClipboard(gridCells);
		}	  
		internal bool IsValidRowHandleAndColumn(int rowHandle, ColumnBase column) {
			return DataControl.IsValidRowHandleCore(rowHandle) && column != null;
		}
		protected internal override void GetDataRowText(StringBuilder sb, int rowHandle) {
			for (int i = 0; i < View.VisibleColumnsCore.Count; i++) {
				sb.Append(View.GetTextForClipboard(rowHandle, i));
				if (DataControl.IsGroupRowHandleCore(rowHandle)) {
					return;
				}
				if (i != View.VisibleColumnsCore.Count - 1)
					sb.Append("\t");
			}
		}
#endregion
		#region master detail
		internal override MasterDetailProviderBase CreateMasterDetailProvider() {
			return new MasterDetailProvider(this);
		}
		delegate void SetPropertyIntoView(ITableView view);
		internal override void UpdateActualProperties() {
			UpdateShowTotalSummaryIndicatorIndent(TableView);
			UpdateActualShowIndicator();
			UpdateActualIndicatorWidth();
			UpdateActualExpandDetailButtonWidth();
			UpdateActualDetailMargin();
			UpdateActualFadeSelectionOnLostFocus();
			UpdateExpandColumnPosition();
		}
		void UpdateShowTotalSummaryIndicatorIndent(ITableView view) {
			if(!view.ViewBase.IsRootView) {
				view.SetShowTotalSummaryIndicatorIndent(false);
			} else {
				view.SetShowTotalSummaryIndicatorIndent(view.ShowIndicator);
			}
		}
		void UpdateActualRootProperty(SetPropertyIntoView setProperty) {
			View.UpdateAllOriginationViews(view => {
				setProperty((ITableView)view);
			});
		}
		void UpdateActualShowIndicator() {
			UpdateActualRootProperty(view => view.SetActualShowIndicator(((ITableView)View.RootView).ShowIndicator));
		}
		void UpdateActualIndicatorWidth() {
			UpdateActualRootProperty(view => view.SetActualIndicatorWidth(((ITableView)View.RootView).IndicatorWidth));
		}
		internal virtual void UpdateActualExpandDetailButtonWidth() { }
		internal virtual void UpdateActualDetailMargin() { }
		void UpdateActualFadeSelectionOnLostFocus() {
			UpdateActualRootProperty(view => view.SetActualFadeSelectionOnLostFocus(View.RootView.FadeSelectionOnLostFocus));
		}
		void UpdateExpandColumnPosition() {
			if(View.IsRootView && !TableView.ShowIndicator) {
				TableView.SetExpandColumnPosition(ColumnPosition.Left);
				return;
			}
			TableView.SetExpandColumnPosition(ColumnPosition.Middle);
		}
		internal abstract DetailHeaderControlBase CreateDetailHeaderElement();
		internal abstract DetailHeaderControlBase CreateDetailContentElement();
		internal abstract DetailTabHeadersControlBase CreateDetailTabHeadersElement();
		internal abstract DetailRowControlBase CreateDetailColumnHeadersElement();
		internal abstract DetailRowControlBase CreateDetailTotalSummaryElement();
		internal abstract DetailRowControlBase CreateDetailFixedTotalSummaryElement();
		internal abstract DetailRowControlBase CreateDetailNewItemRowElement();
		#endregion
		#region Format Conditions
		protected internal virtual void AddFormatConditionCore(FormatConditionBase formatCondition) {
			if(formatCondition == null)
				return;
			IModelItem dataControl = GetDataControlModelItem();
			using(IModelEditingScope scope = dataControl.BeginEdit("Add format condition")) {
				IModelItemCollection formatConditions = GetFormatConditionsModelItemCollection(dataControl);
				if(formatCondition is IndicatorFormatConditionBase) {
					var formatConditionsToRemove = formatConditions
						.Where(x => x.ItemType == formatCondition.GetType() && (x.Properties[FormatConditionBase.FieldNameProperty.Name].ComputedValue as string) == formatCondition.FieldName)
						.ToArray();
					foreach(var condition in formatConditionsToRemove) {
						formatConditions.Remove(condition);
					}
				}
				formatConditions.Add(View.DesignTimeAdorner.CreateModelItem(formatCondition, dataControl));
				scope.Complete();
			}
		}
		protected internal virtual void ShowFormatConditionDialogCore(ColumnBase column, FormatConditionDialogType dialogKind) {
			DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<FrameworkElement, IDialogService>.DoServiceAction(View, TableView.FormatConditionDialogServiceTemplate, service =>
			{
				var viewModel = GetViewModelFactory(dialogKind)(TableView);
				viewModel.Initialize(new DataControlDialogContext(column));
				var commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new DXDialogWindowMessageBoxButtonLocalizer());
				commands[0].Command = new DelegateCommand<CancelEventArgs>(x => x.Cancel = !viewModel.TryClose());
				var result = service.ShowDialog(commands, viewModel.Title, viewModel);
				if(result == commands[0]) {
					IModelItem dataControl = GetDataControlModelItem();
					IModelItemCollection formatConditions = GetFormatConditionsModelItemCollection(dataControl);
					IModelItem formatCondition = viewModel.CreateCondition(dataControl.Context, column.FieldName);
					formatCondition.Properties[FormatCondition.FieldNameProperty.Name].SetValue(column.FieldName);
					viewModel.SetFormatProperty(formatCondition);
					formatConditions.Add(formatCondition);
				}
			});
		}
		Func<IFormatsOwner, ConditionalFormattingDialogViewModel> GetViewModelFactory(FormatConditionDialogType dialogKind) {
			switch(dialogKind) {
				case FormatConditionDialogType.GreaterThan:
					return GreaterThanConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.LessThan:
					return LessThanConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.Between:
					return BetweenConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.EqualTo:
					return EqualToConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.TextThatContains:
					return TextThatContainsConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.ADateOccurring:
					return DateOccurringConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.CustomCondition:
					return CustomConditionConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.Top10Items:
					return Top10ItemsConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.Bottom10Items:
					return Bottom10ItemsConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.Top10Percent:
					return Top10PercentConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.Bottom10Percent:
					return Bottom10PercentConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.AboveAverage:
					return AboveAverageConditionalFormattingDialogViewModel.Factory;
				case FormatConditionDialogType.BelowAverage:
					return BelowAverageConditionalFormattingDialogViewModel.Factory;
				default:
					throw new InvalidOperationException();
			}
		}
		protected internal virtual void ClearFormatConditionsFromAllColumnsCore() {
			GetFormatConditionsModelItemCollection(GetDataControlModelItem()).Clear();
		}
		protected internal virtual void ClearFormatConditionsFromColumnCore(ColumnBase column) {
			if(column == null)
				return;
			IModelItemCollection formatConditions = GetFormatConditionsModelItemCollection(GetDataControlModelItem());
			var columnConditions = formatConditions.Where(x => ((FormatConditionBase)x.GetCurrentValue()).FieldName == column.FieldName).ToArray();
			if(columnConditions.Length == 0)
				return;
			FormatConditions.BeginUpdate();
			try {
				foreach(var item in columnConditions) {
					formatConditions.Remove(item);
				}
			}
			finally {
				FormatConditions.EndUpdate();
			}
		}
		protected internal virtual void ShowConditionalFormattingManagerCore(ColumnBase column) {
			DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<FrameworkElement, IDialogService>.DoServiceAction(View, TableView.ConditionalFormattingManagerServiceTemplate, service =>
			{
				var viewModel = DevExpress.Xpf.Core.ConditionalFormattingManager.ManagerViewModel.Factory(new DataControlDialogContext(column));
				List<UICommand> commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new DXDialogWindowMessageBoxButtonLocalizer());
				UICommand applyCommand = new UICommand()
				{
					Caption = ConditionalFormattingLocalizer.Active.GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Apply),
					IsCancel = false,
					IsDefault = false,
					Command = new DelegateCommand<CancelEventArgs>(e =>
					{
						e.Cancel = true;
						viewModel.ApplyChanges();
					},
					x => viewModel.CanApply),
				};
				commands.Add(applyCommand);
				UICommand result = service.ShowDialog(commands, viewModel.Description, viewModel);
				if(result == commands[0])
					viewModel.ApplyChanges();
			});
		}
		IModelItemCollection GetFormatConditionsModelItemCollection(IModelItem dataControl) {
			IModelItem view = dataControl.Properties["View"].Value;
			return view.Properties["FormatConditions"].Collection;
		}
		IModelItem GetDataControlModelItem() {
			return View.DesignTimeAdorner.GetDataControlModelItem(DataControl);
		}
		#endregion
		void OnRowStyleChanged(Style newStyle) {
			ValidateRowStyle(newStyle);
			UpdateViewRowData(rowData => rowData.UpdateClientRowStyle());
		}
		internal virtual void ValidateRowStyle(Style newStyle) { 
		}
		void OnAlternateRowPropertiesChanged() {
			View.Do(view => view.UpdateAlternateRowBackground());
		}
		void OnActualAlternateRowBackgroundChanged() {
			if(DataControl != null)
				DataControl.UpdateRowsCore();
			UpdateViewRowData(x => x.UpdateClientAlternateBackground());
		}
		protected internal override bool IsFirstColumn(BaseColumn column) {
			if(DataControl.BandsLayoutCore == null)
				return column.VisibleIndex == 0;
			return !column.HasLeftSibling;
		}
		protected internal override ControlTemplate GetActualColumnChooserTemplate() {
			return DataControl != null && DataControl.BandsLayoutCore != null && DataControl.BandsLayoutCore.ShowBandsInCustomizationForm ? TableView.ColumnBandChooserTemplate : View.ColumnChooserTemplate;
		}
		protected override void RebuildColumnChooserColumnsCore() {
			if(DataControl.BandsLayoutCore == null)
				base.RebuildColumnChooserColumnsCore();
			else
				DataControl.BandsLayoutCore.RebuildColumnChooserColumns();
		}
		void SetMouseMoveSelectionStrategy(MouseMoveSelectionBase mouseMoveSelectionStrategy) {
			TableViewBehavior rootBehavior = View.RootView.ViewBehavior as TableViewBehavior;
			rootBehavior.mouseMoveSelection = mouseMoveSelectionStrategy;
		}
		#region Inner classes
		class DefaultColumnChooserBandsSortOrderComparer : IComparer<BandBase> {
			public static readonly IComparer<BandBase> Instance = new DefaultColumnChooserBandsSortOrderComparer();
			DefaultColumnChooserBandsSortOrderComparer() { }
			int IComparer<BandBase>.Compare(BandBase x, BandBase y) {
				return Comparer<string>.Default.Compare(DataViewBase.GetColumnChooserSortableCaption(x), DataViewBase.GetColumnChooserSortableCaption(y));
			}
		}
		abstract class UpdateColumnsStrategyBase {
			internal abstract IList<ColumnBase> GetFixedVisibleColumns(FixedStyle fixedStyle, DataViewBase view);
			internal abstract bool HasFixedLeftElements(ITableView view);
			internal abstract bool HasFixedRightElements(ITableView view);
			internal abstract void UpdateColumnDataWidths(DataViewBase view, GridViewInfo viewInfo, out double totalFixedNoneSize, out double totalFixedLeftSize, out double totalFixedRightSize);
			internal Thickness GetScrollingMargin(ITableView tableView, double secondaryOffset, double virtualizationOffset) {
				return GetScrollingMarginCore(secondaryOffset, virtualizationOffset);
			}
			protected abstract Thickness GetScrollingMarginCore(double secondaryOffset, double virtualizationOffset);
			internal abstract List<ColumnBase> GetViewportVisibleColumns(TableViewBehavior viewBehavior);
			internal abstract void FillByLastFixedColumn(TableViewBehavior viewBehavior, double arrangeWidth);
			internal abstract FixedStyle GetFixedStyle(BaseColumn column, TableViewBehavior viewBehavior);
			protected void GetColumnVisibleBounds(TableViewBehavior viewBehavior, out double leftVisibleBound, out double rightVisibleBound) {
				if(!IsTotalSummaryVisible(viewBehavior) || (viewBehavior.HasFixedLeftElements && viewBehavior.HasFixedRightElements)) {
					leftVisibleBound = viewBehavior.HorizontalOffset - viewBehavior.ViewInfo.TotalGroupAreaIndent;
					rightVisibleBound = viewBehavior.HorizontalOffset + viewBehavior.TableView.HorizontalViewport;
					return;
				}
				leftVisibleBound = viewBehavior.HorizontalOffset;
				if(!viewBehavior.HasFixedLeftElements) {
					if(viewBehavior.TableView.ActualShowDetailButtons)
						leftVisibleBound -= viewBehavior.TableView.ActualExpandDetailButtonWidth;
					leftVisibleBound -= viewBehavior.ViewInfo.TotalGroupAreaIndent;
					if(viewBehavior.TableView.ViewBase.IsRootView && viewBehavior.TableView.ActualShowIndicator)
						leftVisibleBound -= viewBehavior.TableView.ActualIndicatorWidth;
				}
				double width = 0;
				width = viewBehavior.TableView.TotalSummaryFixedNoneContentWidth;
				rightVisibleBound = leftVisibleBound + width;
			}
			bool IsTotalSummaryVisible(TableViewBehavior viewBehavior) {
				List<DataControlBase> dataControls = new List<DataControlBase>();
				viewBehavior.View.DataControl.EnumerateThisAndParentDataControls((dataControl) => dataControls.Add(dataControl));
				foreach(DataControlBase dataControl in dataControls)
					if(dataControl.viewCore.ShowTotalSummary)
						return true;
				return false;
			}
		}
		class TableViewUpdateColumnsStrategy :  UpdateColumnsStrategyBase {
			public static readonly UpdateColumnsStrategyBase Instance = new TableViewUpdateColumnsStrategy();
			internal override IList<ColumnBase> GetFixedVisibleColumns(FixedStyle fixedStyle, DataViewBase view) {
				IList<ColumnBase> result = new ObservableCollection<ColumnBase>();
				for(int i = 0; i < view.VisibleColumnsCore.Count; i++) {
					if(view.VisibleColumnsCore[i].Fixed == fixedStyle)
						result.Add(view.VisibleColumnsCore[i]);
				}
				return result;
			}
			internal override bool HasFixedLeftElements(ITableView view) {
				return view.TableViewBehavior.FixedLeftVisibleColumns.Count > 0;
			}
			internal override bool HasFixedRightElements(ITableView view) {
				return view.TableViewBehavior.fixedRightVisibleColumns.Count > 0;
			}
			internal override void UpdateColumnDataWidths(DataViewBase view, GridViewInfo viewInfo, out double totalFixedNoneSize, out double totalFixedLeftSize, out double totalFixedRightSize) {
				totalFixedNoneSize = 0;
				totalFixedLeftSize = 0;
				totalFixedRightSize = 0;
				for(int i = 0; i < view.VisibleColumnsCore.Count; i++) {
					ColumnBase column = view.VisibleColumnsCore[i];
					if(column.Fixed == FixedStyle.None)
						totalFixedNoneSize += column.ActualDataWidth;
					else if(column.Fixed == FixedStyle.Left)
						totalFixedLeftSize += column.ActualDataWidth;
					else
						totalFixedRightSize += column.ActualDataWidth;
				}
			}
			protected override Thickness GetScrollingMarginCore(double secondaryOffset, double virtualizationOffset) {
				return new Thickness(secondaryOffset + virtualizationOffset, 0, 0, 0);
			}
			internal override List<ColumnBase> GetViewportVisibleColumns(TableViewBehavior viewBehavior) {
				List<ColumnBase> viewportColumns = new List<ColumnBase>();
				double totalWidth = 0;
				double totalHeaderWidth = 0;
				int columnIndex = 0;
				viewBehavior.ResetHorizontalVirtualizationOffset();
				double leftVisibleBound, rightVisibleBound;
				GetColumnVisibleBounds(viewBehavior, out leftVisibleBound, out rightVisibleBound);
				do {
					if(columnIndex >= viewBehavior.FixedNoneVisibleColumns.Count)
						break;
					if(totalWidth + viewBehavior.FixedNoneVisibleColumns[columnIndex].ActualDataWidth > leftVisibleBound)
						viewportColumns.Add(viewBehavior.FixedNoneVisibleColumns[columnIndex]);
					if(totalWidth <= leftVisibleBound) {
						viewBehavior.HorizontalVirtualizationOffset = totalWidth;
						viewBehavior.HorizontalHeaderVirtualizationOffset = totalHeaderWidth;
					}
					totalWidth += viewBehavior.FixedNoneVisibleColumns[columnIndex].ActualDataWidth;
					totalHeaderWidth += viewBehavior.FixedNoneVisibleColumns[columnIndex].ActualHeaderWidth;
					columnIndex++;
				} while(totalWidth <= rightVisibleBound);
				return viewportColumns;
			}
			internal override void FillByLastFixedColumn(TableViewBehavior viewBehavior, double arrangeWidth) {
				if(viewBehavior.TableView.AutoWidth || viewBehavior.View.VisibleColumnsCore.Count <= 1 || viewBehavior.FixedRightVisibleColumns.Count == 0)
					return;
				double totalWidth = viewBehavior.ViewInfo.GetDesiredColumnsWidth(viewBehavior.View.VisibleColumnsCore);
				double delta = Math.Max(0, arrangeWidth - totalWidth);
				if(delta <= 0)
					return;
				ColumnBase column = viewBehavior.View.VisibleColumnsCore[viewBehavior.View.VisibleColumnsCore.Count - 1];
				column.ActualDataWidth = column.ActualDataWidth + delta;
				column.ActualHeaderWidth = column.ActualHeaderWidth + delta;
			}
			internal override FixedStyle GetFixedStyle(BaseColumn column, TableViewBehavior viewBehavior) {
				return column.Fixed;
			}
		}
		class BandedViewUpdateColumnsStrategy : UpdateColumnsStrategyBase {
			public static readonly UpdateColumnsStrategyBase Instance = new BandedViewUpdateColumnsStrategy();
			internal override IList<ColumnBase> GetFixedVisibleColumns(FixedStyle fixedStyle, DataViewBase view) {
				IList<ColumnBase> result = new ObservableCollection<ColumnBase>();
				BandsLayoutBase.ForeachVisibleBand(GetFixedBands(fixedStyle, view), (band) => {
					foreach(BandRow row in band.ActualRows)
						foreach(ColumnBase column in row.Columns)
							result.Add(column);
				});
				return result;
			}
			IEnumerable GetFixedBands(FixedStyle fixedStyle, DataViewBase view) {
				if(fixedStyle == FixedStyle.Left)
					return view.DataControl.BandsLayoutCore.FixedLeftVisibleBands;
				else if(fixedStyle == FixedStyle.Right)
					return view.DataControl.BandsLayoutCore.FixedRightVisibleBands;
				else
					return view.DataControl.BandsLayoutCore.FixedNoneVisibleBands;
			}
			internal override bool HasFixedLeftElements(ITableView view) {
				return view.TableViewBehavior.DataControl.BandsLayoutCore.FixedLeftVisibleBands.Count > 0;
			}
			internal override bool HasFixedRightElements(ITableView view) {
				return view.TableViewBehavior.DataControl.BandsLayoutCore.FixedRightVisibleBands.Count > 0;
			}
			internal override void UpdateColumnDataWidths(DataViewBase view, GridViewInfo viewInfo, out double totalFixedNoneSize, out double totalFixedLeftSize, out double totalFixedRightSize) {
				totalFixedNoneSize = 0;
				totalFixedLeftSize = 0;
				totalFixedRightSize = 0;
				for(int i = 0; i < view.DataControl.BandsLayoutCore.VisibleBands.Count; i++) {
					BandBase band = view.DataControl.BandsLayoutCore.VisibleBands[i];
					double cellWidth = band.ActualHeaderWidth;
					if(i == 0)
						cellWidth -= viewInfo.FirstColumnIndent;
					if(band.Fixed == FixedStyle.None)
						totalFixedNoneSize += cellWidth;
					else if(band.Fixed == FixedStyle.Left)
						totalFixedLeftSize += cellWidth;
					else
						totalFixedRightSize += cellWidth;
				}
			}
			protected override Thickness GetScrollingMarginCore(double secondaryOffset, double virtualizationOffset) {
				return new Thickness(secondaryOffset, 0, 0, 0);
			}
			internal override List<ColumnBase> GetViewportVisibleColumns(TableViewBehavior viewBehavior) {
				List<ColumnBase> viewportColumns = new List<ColumnBase>();
				double currentWidth = 0;
				double leftVisibleBound, rightVisibleBound;
				GetColumnVisibleBounds(viewBehavior, out leftVisibleBound, out rightVisibleBound);
				BandsLayoutBase.ForeachVisibleBand(viewBehavior.DataControl.BandsLayoutCore.FixedNoneVisibleBands, (band) => {
					currentWidth = GetBandViewportVisibleColumns(band, currentWidth, leftVisibleBound, rightVisibleBound, viewBehavior, viewportColumns);
				});
				return viewportColumns;
			}
			double GetBandViewportVisibleColumns(BandBase band, double currentWidth, double leftVisibleBound, double rightVisibleBound, TableViewBehavior viewBehavior, List<ColumnBase> viewportColumns) {
				if(band.ActualRows.Count != 0) {
					foreach(BandRow bandRow in band.ActualRows) {
						double width = currentWidth;
						foreach(ColumnBase column in bandRow.Columns) {
							if(width > rightVisibleBound + viewBehavior.viewInfo.TotalGroupAreaIndent) break;
							if(width + column.ActualHeaderWidth > leftVisibleBound)
								viewportColumns.Add(column);
							width += column.ActualHeaderWidth;
						}
					}
				}
				return band.VisibleBands.Count != 0 ? currentWidth : currentWidth + band.ActualHeaderWidth;
			 }
			internal override void FillByLastFixedColumn(TableViewBehavior viewBehavior, double arrangeWidth) {
			}
			internal override FixedStyle GetFixedStyle(BaseColumn column, TableViewBehavior viewBehavior) {
				return viewBehavior.DataControl.BandsLayoutCore.GetRootBand(column.ParentBandInternal).Fixed;
			}
		}
		#endregion
	}
}
