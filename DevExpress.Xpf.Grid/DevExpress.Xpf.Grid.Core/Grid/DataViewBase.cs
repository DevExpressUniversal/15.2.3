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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.ExpressionEditor.Native;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Grid.Automation;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Data.Linq;
#if !SL
using DXDialog = DevExpress.Xpf.Core.FloatingContainer;
using System.Windows.Documents;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Utils.Design;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Grid.Printing;
using System.Printing;
using DataController = DevExpress.Data.DataController;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export;
using System.IO;
namespace DevExpress.Xpf.Grid {
	public enum ScrollAnimationMode {
		EaseOut,
		Linear,
		EaseInOut,
		Custom
	}
	public enum ItemsSourceErrorInfoShowMode { 
		RowAndCell = 3,
		Row = 1,
		Cell = 2,
		None = 0,
	}
	public abstract class GridDataViewBase : DataViewBase {
		internal GridDataViewBase(MasterNodeContainer masterRootNode, MasterRowsContainer masterRootDataItem, DataControlDetailDescriptor detailDescriptor)
			: base(masterRootNode, masterRootDataItem, detailDescriptor) {
		}
	}
	public abstract partial class DataViewBase : Control, ILogicalOwner, IColumnOwnerBase, IConvertClonePropertyValue, IPrintableControl, INotifyPropertyChanged
#if SL
, IPopupContainer
#endif
 {
		internal static int CoerceBestFitMaxRowCount(int baseValue) {
			return Math.Max(baseValue, -1);
		}
		public static readonly DependencyProperty FadeSelectionOnLostFocusProperty;
		protected static readonly DependencyPropertyKey ActualFadeSelectionOnLostFocusPropertyKey;
		public static readonly DependencyProperty ActualFadeSelectionOnLostFocusProperty;
		public static readonly DependencyProperty RowAnimationKindProperty;
		public static readonly RoutedEvent RowAnimationBeginEvent;
		internal static readonly DependencyPropertyKey HasValidationErrorPropertyKey;
		public static readonly DependencyProperty HasValidationErrorProperty;
		internal static readonly DependencyPropertyKey ValidationErrorPropertyKey;
		public static readonly DependencyProperty ValidationErrorProperty;
		public static readonly DependencyProperty AllowCommitOnValidationAttributeErrorProperty;
		public static readonly DependencyProperty RowHandleProperty;
		public static readonly DependencyProperty ScrollingModeProperty;
		public static readonly DependencyProperty IsDeferredScrollingProperty;
		public static readonly DependencyProperty EditorButtonShowModeProperty;
		public static readonly DependencyProperty AllowMoveColumnToDropAreaProperty;
		public static readonly DependencyProperty ColumnChooserTemplateProperty;
		protected static readonly DependencyPropertyKey ActualColumnChooserTemplatePropertyKey;
		public static readonly DependencyProperty ActualColumnChooserTemplateProperty;
		public static readonly DependencyProperty IsColumnChooserVisibleProperty;
		public static readonly DependencyProperty ColumnHeaderDragIndicatorTemplateProperty;
		internal const string ColumnHeaderDragIndicatorTemplatePropertyName = "ColumnHeaderDragIndicatorTemplate";
		static readonly DependencyPropertyKey ActiveEditorPropertyKey;
		public static readonly DependencyProperty ActiveEditorProperty;
		public static readonly DependencyProperty EditorShowModeProperty;
		public static readonly DependencyProperty IsFocusedRowProperty;
		public static readonly DependencyProperty IsFocusedCellProperty;
		static readonly DependencyPropertyKey IsKeyboardFocusWithinViewPropertyKey;
		public static readonly DependencyProperty IsKeyboardFocusWithinViewProperty;
		static readonly DependencyPropertyKey IsHorizontalScrollBarVisiblePropertyKey;
		public static readonly DependencyProperty IsHorizontalScrollBarVisibleProperty;
		static readonly DependencyPropertyKey IsTouchScrollBarsModePropertyKey;
		public static readonly DependencyProperty IsTouchScrollBarsModeProperty;
		public static readonly DependencyProperty ColumnHeaderDragIndicatorSizeProperty;
		public static readonly DependencyProperty NavigationStyleProperty;
		public static readonly DependencyProperty ScrollStepProperty;
		static readonly DependencyPropertyKey ColumnChooserColumnsPropertyKey;
		public static readonly DependencyProperty ColumnChooserColumnsProperty;
		public static readonly DependencyProperty ShowFocusedRectangleProperty;
		public static readonly DependencyProperty FocusedCellBorderTemplateProperty;
		public static readonly DependencyProperty FocusedGroupRowBorderTemplateProperty;
		public static readonly DependencyProperty ClipboardCopyWithHeadersProperty;
		public static readonly DependencyProperty ClipboardCopyAllowedProperty;
		public static readonly DependencyProperty FocusedRowProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		static readonly DependencyPropertyKey FocusedRowDataPropertyKey;
		public static readonly DependencyProperty FocusedRowDataProperty;
		public static readonly DependencyProperty ColumnChooserFactoryProperty;
		public static readonly DependencyProperty ColumnChooserStateProperty;
		public static readonly DependencyProperty AllowSortingProperty;
		[Obsolete("Instead use the AllowColumnMoving property.")]
		public static readonly DependencyProperty AllowMovingProperty;
		public static readonly DependencyProperty AllowColumnMovingProperty;
		public static readonly DependencyProperty AllowEditingProperty;
		public static readonly DependencyProperty AllowColumnFilteringProperty;
		public static readonly DependencyProperty AllowFilterEditorProperty;
		static readonly DependencyPropertyKey ShowEditFilterButtonPropertyKey;
		public static readonly DependencyProperty ShowEditFilterButtonProperty;
		static Duration defaultAnimationDuration = new Duration(TimeSpan.FromMilliseconds(350));
		public static readonly DependencyProperty ColumnHeaderTemplateProperty;
		public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty ColumnHeaderCustomizationAreaTemplateProperty;
		public static readonly DependencyProperty ColumnHeaderCustomizationAreaTemplateSelectorProperty;
		public static readonly DependencyProperty ShowTotalSummaryProperty, ShowColumnHeadersProperty;
		public static readonly DependencyProperty ShowFilterPanelModeProperty;
		static readonly DependencyPropertyKey ActualShowFilterPanelPropertyKey;
		public static readonly DependencyProperty ActualShowFilterPanelProperty;
		static readonly DependencyPropertyKey FilterPanelTextPropertyKey;
		public static readonly DependencyProperty FilterPanelTextProperty;
		public static readonly DependencyProperty ShowValidationAttributeErrorsProperty;
		const string FilterEditorShowOperandTypeIconPropertyName = "FilterEditorShowOperandTypeIcon";
		public static readonly DependencyProperty FilterEditorShowOperandTypeIconProperty;
		static readonly DependencyPropertyKey IsEditingPropertyKey;
		public static readonly DependencyProperty IsEditingProperty;
		static readonly DependencyPropertyKey IsFocusedRowModifiedPropertyKey;
		public static readonly DependencyProperty IsFocusedRowModifiedProperty;
		public static readonly DependencyProperty ColumnHeaderContentStyleProperty;
		public static readonly DependencyProperty CellStyleProperty;
		public static readonly DependencyProperty TotalSummaryContentStyleProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty FooterTemplateProperty;
		public static readonly DependencyProperty TotalSummaryItemTemplateProperty;
		public static readonly DependencyProperty TotalSummaryItemTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualTotalSummaryItemTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualTotalSummaryItemTemplateSelectorProperty;
		public static readonly DependencyProperty IsColumnMenuEnabledProperty;
		public static readonly DependencyProperty IsTotalSummaryMenuEnabledProperty;
		public static readonly DependencyProperty IsRowCellMenuEnabledProperty;
		public static readonly DependencyProperty ColumnHeaderToolTipTemplateProperty;
		public static readonly DependencyProperty RowOpacityAnimationDurationProperty;
		public static readonly DependencyProperty WaitIndicatorTypeProperty;
		public static readonly DependencyProperty WaitIndicatorStyleProperty;
		public static readonly DependencyProperty IsWaitIndicatorVisibleProperty;
		static readonly DependencyPropertyKey IsWaitIndicatorVisiblePropertyKey;
		public static readonly DependencyProperty ScrollableAreaMinWidthProperty;
		static readonly DependencyPropertyKey ScrollableAreaMinWidthPropertyKey;
		public static readonly DependencyProperty TopRowIndexProperty;
		public static readonly DependencyProperty AllowLeaveFocusOnTabProperty;
		public static readonly DependencyProperty WheelScrollLinesProperty;
		public static readonly DependencyProperty TouchScrollThresholdProperty;
		public static readonly RoutedEvent FilterEditorCreatedEvent;
		public static readonly RoutedEvent ShownColumnChooserEvent;
		public static readonly RoutedEvent HiddenColumnChooserEvent;
		public static readonly RoutedEvent CustomFilterDisplayTextEvent;
		public static readonly RoutedEvent BeforeLayoutRefreshEvent;
		public static readonly DependencyProperty AutoScrollOnSortingProperty;
		public static readonly RoutedEvent ShowFilterPopupEvent;
		public static readonly RoutedEvent UnboundExpressionEditorCreatedEvent;
		public static readonly RoutedEvent PastingFromClipboardEvent;
		public static readonly DependencyProperty FocusedRowHandleProperty;
		public static readonly DependencyProperty AllowScrollToFocusedRowProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly RoutedEvent FocusedColumnChangedEvent;
		public static readonly RoutedEvent FocusedRowHandleChangedEvent;
		public static readonly RoutedEvent FocusedRowChangedEvent;
		public static readonly RoutedEvent FocusedViewChangedEvent;
		public static readonly RoutedEvent ShowGridMenuEvent;
		public static readonly RoutedEvent ColumnHeaderClickEvent;
		public static readonly DependencyProperty SummariesIgnoreNullValuesProperty;
		public static readonly DependencyProperty EnterMoveNextColumnProperty;
		public static readonly DependencyProperty RuntimeLocalizationStringsProperty;
		static readonly DependencyPropertyKey LocalizationDescriptorPropertyKey;
		public static readonly DependencyProperty LocalizationDescriptorProperty;
		public static readonly DependencyProperty ColumnChooserColumnsSortOrderComparerProperty;
		public static readonly DependencyProperty DetailHeaderContentProperty;
		public static readonly DependencyProperty ItemsSourceErrorInfoShowModeProperty;
		public static readonly DependencyProperty SelectedRowsSourceProperty;
		public static readonly DependencyProperty AllItemsSelectedProperty;
		static readonly DependencyPropertyKey AllItemsSelectedPropertyKey;
		public static readonly DependencyProperty UseExtendedMouseScrollingProperty;
		public static readonly DependencyProperty EnableImmediatePostingProperty;
		public static readonly DependencyProperty AllowLeaveInvalidEditorProperty;
		#region Printing
		public static readonly DependencyProperty PrintHeaderTemplateProperty;
		public static readonly DependencyProperty PrintCellStyleProperty;
		public static readonly DependencyProperty PrintRowIndentStyleProperty;
		public static readonly DependencyProperty PrintSelectedRowsOnlyProperty;
		public static readonly DependencyProperty PrintTotalSummaryProperty;
		public static readonly DependencyProperty PrintFixedTotalSummaryProperty;
		public static readonly DependencyProperty PrintTotalSummaryStyleProperty;
		public static readonly DependencyProperty PrintFixedTotalSummaryStyleProperty;
		public static readonly DependencyProperty PrintFooterTemplateProperty;
		public static readonly DependencyProperty PrintFixedFooterTemplateProperty;
		#endregion
		public static readonly DependencyProperty DataNavigatorButtonsProperty;
		public static readonly DependencyProperty FilterRowDelayProperty;
		public static readonly DependencyProperty ClipboardCopyOptionsProperty;
		public static readonly DependencyProperty ClipboardModeProperty;
		public static readonly DependencyProperty SelectionRectangleStyleProperty;
		public static readonly DependencyProperty ShowSelectionRectangleProperty;
		internal readonly Locker ScrollAnimationLocker = new Locker();
		internal readonly Locker ScrollIntoViewLocker = new Locker();
		internal readonly Locker CommitEditingLocker = new Locker();
		static DataViewBase() {
			Type ownerType = typeof(DataViewBase);
			FadeSelectionOnLostFocusProperty = DependencyPropertyManager.Register("FadeSelectionOnLostFocus", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnFadeSelectionOnLostFocusChanged));
			ActualFadeSelectionOnLostFocusPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualFadeSelectionOnLostFocus", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).UpdateRowDataFocusWithinState()));
			ActualFadeSelectionOnLostFocusProperty = ActualFadeSelectionOnLostFocusPropertyKey.DependencyProperty;
			RowAnimationKindProperty = DependencyPropertyManager.Register("RowAnimationKind", typeof(RowAnimationKind), ownerType, new FrameworkPropertyMetadata(RowAnimationKind.Opacity));
			RowAnimationBeginEvent = EventManager.RegisterRoutedEvent("RowAnimationBegin", RoutingStrategy.Direct, typeof(RowAnimationEventHandler), ownerType);
			RowHandleProperty = DependencyPropertyManager.RegisterAttached("RowHandle", typeof(RowHandle), ownerType, new FrameworkPropertyMetadata(null));
			ValidationErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ValidationError", typeof(BaseValidationError), ownerType,
				new FrameworkPropertyMetadata(null, OnValidationErrorPropertyChanged));
			ValidationErrorProperty = ValidationErrorPropertyKey.DependencyProperty;
			HasValidationErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasValidationError", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			HasValidationErrorProperty = HasValidationErrorPropertyKey.DependencyProperty;
			AllowCommitOnValidationAttributeErrorProperty = DependencyPropertyManager.Register("AllowCommitOnValidationAttributeError", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ScrollingModeProperty = DependencyPropertyManager.Register("ScrollingMode", typeof(ScrollingMode), ownerType, new FrameworkPropertyMetadata(ScrollingMode.Smart));
			IsDeferredScrollingProperty = DependencyPropertyManager.Register("IsDeferredScrolling", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			EditorButtonShowModeProperty = DependencyPropertyManager.Register("EditorButtonShowMode", typeof(EditorButtonShowMode), ownerType, new FrameworkPropertyMetadata(EditorButtonShowMode.ShowOnlyInEditor, OnEditorShowModeChanged));
			AllowMoveColumnToDropAreaProperty = DependencyPropertyManager.Register("AllowMoveColumnToDropArea", typeof(bool), ownerType, new UIPropertyMetadata(true));
			ColumnChooserTemplateProperty = DependencyPropertyManager.Register("ColumnChooserTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateActualColumnChooserTemplate()));
			ActualColumnChooserTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualColumnChooserTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualColumnChooserTemplateProperty = ActualColumnChooserTemplatePropertyKey.DependencyProperty;
			IsColumnChooserVisibleProperty = DependencyPropertyManager.Register("IsColumnChooserVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DataViewBase)d).OnIsColumnChooserVisibleChanged()));
			ColumnHeaderDragIndicatorTemplateProperty = DependencyPropertyManager.Register(ColumnHeaderDragIndicatorTemplatePropertyName, typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ColumnHeaderDragIndicatorSizeProperty = DependencyPropertyManager.RegisterAttached("ColumnHeaderDragIndicatorSize", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None));
			ActiveEditorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActiveEditor", typeof(BaseEdit), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).OnActiveEditorChanged()));
			ActiveEditorProperty = ActiveEditorPropertyKey.DependencyProperty;
			EditorShowModeProperty = DependencyPropertyManager.Register("EditorShowMode", typeof(EditorShowMode), ownerType, new FrameworkPropertyMetadata(EditorShowMode.Default));
			IsFocusedRowProperty = DependencyPropertyManager.RegisterAttached("IsFocusedRow", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			IsFocusedCellProperty = DependencyPropertyManager.RegisterAttached("IsFocusedCell", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			IsKeyboardFocusWithinViewPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsKeyboardFocusWithinView", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((DataViewBase)d).OnIsKeyboardFocusWithinViewChanged()));
			IsKeyboardFocusWithinViewProperty = IsKeyboardFocusWithinViewPropertyKey.DependencyProperty;
			IsHorizontalScrollBarVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsHorizontalScrollBarVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			IsHorizontalScrollBarVisibleProperty = IsHorizontalScrollBarVisiblePropertyKey.DependencyProperty;
			IsTouchScrollBarsModePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTouchScrollBarsMode", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			IsTouchScrollBarsModeProperty = IsTouchScrollBarsModePropertyKey.DependencyProperty;
			NavigationStyleProperty = DependencyPropertyManager.Register("NavigationStyle", typeof(GridViewNavigationStyle), ownerType, new FrameworkPropertyMetadata(GridViewNavigationStyle.Cell, new PropertyChangedCallback(OnNavigationStyleChanged)));
			ScrollStepProperty = DependencyPropertyManager.Register("ScrollStep", typeof(int), ownerType, new FrameworkPropertyMetadata(10, (d, e) => { d.CoerceValue(ScrollStepProperty); }, CoerceScrollStep));
			ColumnChooserColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ColumnChooserColumns", typeof(ReadOnlyCollection<ColumnBase>), ownerType, new FrameworkPropertyMetadata(null));
			ColumnChooserColumnsProperty = ColumnChooserColumnsPropertyKey.DependencyProperty;
			ShowFocusedRectangleProperty = DependencyPropertyManager.Register("ShowFocusedRectangle", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DataViewBase)d).OnShowFocusedRectangleChanged()));
			FocusedCellBorderTemplateProperty = DependencyPropertyManager.Register("FocusedCellBorderTemplate", typeof(ControlTemplate), ownerType);
			FocusedGroupRowBorderTemplateProperty = DependencyPropertyManager.Register("FocusedGroupRowBorderTemplate", typeof(ControlTemplate), ownerType);
			ClipboardCopyWithHeadersProperty = DependencyPropertyManager.Register("ClipboardCopyWithHeaders", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ClipboardCopyAllowedProperty = DependencyPropertyManager.Register("ClipboardCopyAllowed", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			FocusedRowProperty = DependencyPropertyManager.Register("FocusedRow", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((DataViewBase)d).OnFocusedRowChanged(e.OldValue, e.NewValue)));
			IsSynchronizedWithCurrentItemProperty = DependencyPropertyManager.Register("IsSynchronizedWithCurrentItem", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).OnIsSynchronizedWithCurrentItemChanged((bool)e.OldValue, (bool)e.NewValue)));
			FocusedRowDataPropertyKey = DependencyPropertyManager.RegisterReadOnly("FocusedRowData", typeof(RowData), ownerType, new FrameworkPropertyMetadata(null));
			FocusedRowDataProperty = FocusedRowDataPropertyKey.DependencyProperty;
			FocusedRowHandleProperty = DependencyPropertyManager.Register("FocusedRowHandle", typeof(int), ownerType, new FrameworkPropertyMetadata(DataControlBase.InvalidRowHandle, OnFocusedRowHandleChanged, CoerceFocusedRowHandle));
			AllowScrollToFocusedRowProperty = DependencyPropertyManager.Register("AllowScrollToFocusedRow", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ColumnChooserFactoryProperty = DependencyPropertyManager.Register("ColumnChooserFactory", typeof(IColumnChooserFactory), ownerType, new FrameworkPropertyMetadata(DefaultColumnChooserFactory.Instance, (d, e) => ((DataViewBase)d).OnColumnChooserFactoryChanged(), (d, baseValue) => ((DataViewBase)d).CoerceColumnChooserFactory((IColumnChooserFactory)baseValue)));
			ColumnChooserStateProperty = DependencyPropertyManager.Register("ColumnChooserState", typeof(IColumnChooserState), ownerType, new UIPropertyMetadata(null, OnColumnChooserStateChanged));
			CellTemplateProperty = DependencyPropertyManager.Register("CellTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualCellTemplateSelector()));
			CellTemplateSelectorProperty = DependencyPropertyManager.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualCellTemplateSelector()));
			AllowSortingProperty = DependencyPropertyManager.Register("AllowSorting", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnUpdateColumnsViewInfo));
#pragma warning disable 618
			AllowMovingProperty = DependencyPropertyManager.Register("AllowMoving", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).AllowColumnMoving = (bool)e.NewValue));
#pragma warning restore 618
			AllowColumnMovingProperty = DependencyPropertyManager.Register("AllowColumnMoving", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnUpdateColumnsViewInfo));
			AllowEditingProperty = DependencyPropertyManager.Register("AllowEditing", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).UpdateEditorButtonVisibilities()));
			AllowColumnFilteringProperty = DependencyPropertyManager.Register("AllowColumnFiltering", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnUpdateColumnsViewInfo));
			AllowFilterEditorProperty = DependencyPropertyManager.Register("AllowFilterEditor", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataViewBase)d).AllowFilterEditorChanged()));
			ShowEditFilterButtonPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowEditFilterButton", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowEditFilterButtonProperty = ShowEditFilterButtonPropertyKey.DependencyProperty;
			ColumnHeaderTemplateProperty = DependencyPropertyManager.Register("ColumnHeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualHeaderTemplateSelector()));
			ColumnHeaderTemplateSelectorProperty = DependencyPropertyManager.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualHeaderTemplateSelector()));
			ColumnHeaderCustomizationAreaTemplateProperty = DependencyPropertyManager.Register("ColumnHeaderCustomizationAreaTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualHeaderCustomizationAreaTemplateSelector()));
			ColumnHeaderCustomizationAreaTemplateSelectorProperty = DependencyPropertyManager.Register("ColumnHeaderCustomizationAreaTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnsActualHeaderCustomizationAreaTemplateSelector()));
			ShowTotalSummaryProperty = DependencyPropertyManager.Register("ShowTotalSummary", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((DataViewBase)d).OnShowTotalSummaryChanged()));
			ShowColumnHeadersProperty = DependencyPropertyManager.Register("ShowColumnHeaders", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((DataViewBase)d).InvalidateParentTree()));
			ShowFilterPanelModeProperty = DependencyPropertyManager.Register("ShowFilterPanelMode", typeof(ShowFilterPanelMode), ownerType, new PropertyMetadata(ShowFilterPanelMode.Default, (d, e) => ((DataViewBase)d).UpdateFilterPanel()));
			ActualShowFilterPanelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowFilterPanel", typeof(bool), ownerType, new PropertyMetadata(false));
			ActualShowFilterPanelProperty = ActualShowFilterPanelPropertyKey.DependencyProperty;
			FilterPanelTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("FilterPanelText", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			FilterPanelTextProperty = FilterPanelTextPropertyKey.DependencyProperty;
			ShowValidationAttributeErrorsProperty = DependencyPropertyManager.Register("ShowValidationAttributeErrors", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnUpdateShowValidationAttributeError));
			FilterEditorShowOperandTypeIconProperty = DependencyPropertyManager.Register(FilterEditorShowOperandTypeIconPropertyName, typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsEditingPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsEditing", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DataViewBase)d).IsEditingCore = (bool)e.NewValue));
			IsEditingProperty = IsEditingPropertyKey.DependencyProperty;
			IsFocusedRowModifiedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFocusedRowModified", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsFocusedRowModifiedProperty = IsFocusedRowModifiedPropertyKey.DependencyProperty;
			TotalSummaryItemTemplateProperty = DependencyPropertyManager.Register("TotalSummaryItemTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateActualTotalSummaryItemTemplateSelector()));
			TotalSummaryItemTemplateSelectorProperty = DependencyPropertyManager.Register("TotalSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateActualTotalSummaryItemTemplateSelector()));
			ActualTotalSummaryItemTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualTotalSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualTotalSummaryItemTemplateSelectorProperty = ActualTotalSummaryItemTemplateSelectorPropertyKey.DependencyProperty;
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			FooterTemplateProperty = DependencyPropertyManager.Register("FooterTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ColumnHeaderContentStyleProperty = DependencyPropertyManager.Register("ColumnHeaderContentStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			CellStyleProperty = DependencyPropertyManager.Register("CellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			TotalSummaryContentStyleProperty = DependencyPropertyManager.Register("TotalSummaryContentStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			RuntimeLocalizationStringsProperty = DependencyPropertyManager.Register("RuntimeLocalizationStrings", typeof(GridRuntimeStringCollection), ownerType, new FrameworkPropertyMetadata(null, OnRuntimeLocalizationStringsChanged));
			LocalizationDescriptorPropertyKey = DependencyPropertyManager.RegisterReadOnly("LocalizationDescriptor", typeof(LocalizationDescriptor), ownerType, new FrameworkPropertyMetadata(null));
			LocalizationDescriptorProperty = LocalizationDescriptorPropertyKey.DependencyProperty;
			WaitIndicatorTypeProperty = DependencyPropertyManager.Register("WaitIndicatorType", typeof(WaitIndicatorType), ownerType, new FrameworkPropertyMetadata(WaitIndicatorType.Default));
			WaitIndicatorStyleProperty = DependencyPropertyManager.Register("WaitIndicatorStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			IsWaitIndicatorVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsWaitIndicatorVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsWaitIndicatorVisibleProperty = IsWaitIndicatorVisiblePropertyKey.DependencyProperty;
			ScrollableAreaMinWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ScrollableAreaMinWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d));
			ScrollableAreaMinWidthProperty = ScrollableAreaMinWidthPropertyKey.DependencyProperty;
			TopRowIndexProperty = DependencyPropertyManager.Register("TopRowIndex", typeof(int), ownerType, new PropertyMetadata(0, (d,e) => ((DataViewBase)d).OnTopRowIndexChanged(), CoerceTopRowIndex));
			AllowLeaveFocusOnTabProperty = DependencyPropertyManager.Register("AllowLeaveFocusOnTab", typeof(bool), ownerType, new PropertyMetadata(false));
			WheelScrollLinesProperty = DependencyPropertyManager.Register("WheelScrollLines", typeof(double), ownerType, new FrameworkPropertyMetadata((double)System.Windows.SystemParameters.WheelScrollLines, (d, e) => { d.CoerceValue(WheelScrollLinesProperty); }, CoerceWheelScrollLines));
			TouchScrollThresholdProperty = DependencyPropertyManager.Register("TouchScrollThreshold", typeof(double), ownerType, new PropertyMetadata(DataPresenterBase.DefaultTouchScrollThreshold));
			SummariesIgnoreNullValuesProperty = DependencyPropertyManager.Register("SummariesIgnoreNullValues", typeof(bool), typeof(DataViewBase), new FrameworkPropertyMetadata(false, (d, e) => ((DataViewBase)d).UpdateSummariesIgnoreNullValues()));
			EnterMoveNextColumnProperty = DependencyPropertyManager.Register("EnterMoveNextColumn", typeof(bool), ownerType, new PropertyMetadata(false));
			ColumnChooserColumnsSortOrderComparerProperty = DependencyPropertyManager.Register("ColumnChooserColumnsSortOrderComparer", typeof(IComparer<ColumnBase>), ownerType, new PropertyMetadata(DefaultColumnChooserColumnsSortOrderComparer.Instance, (d, e) => ((DataViewBase)d).RebuildColumnChooserColumns()));
			ShownColumnChooserEvent = EventManager.RegisterRoutedEvent("ShownColumnChooser", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			HiddenColumnChooserEvent = EventManager.RegisterRoutedEvent("HiddenColumnChooser", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			BeforeLayoutRefreshEvent = EventManager.RegisterRoutedEvent("BeforeLayoutRefresh", RoutingStrategy.Bubble, typeof(CancelRoutedEventHandler), ownerType);
			CustomFilterDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomFilterDisplayText", RoutingStrategy.Direct, typeof(CustomFilterDisplayTextEventHandler), ownerType);
			AutoScrollOnSortingProperty = DependencyPropertyManager.Register("AutoScrollOnSorting", typeof(bool), ownerType, new PropertyMetadata(true));
			IsColumnMenuEnabledProperty = DependencyPropertyManager.Register("IsColumnMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsTotalSummaryMenuEnabledProperty = DependencyPropertyManager.Register("IsTotalSummaryMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsRowCellMenuEnabledProperty = DependencyPropertyManager.Register("IsRowCellMenuEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ColumnHeaderToolTipTemplateProperty = DependencyPropertyManager.Register("ColumnHeaderToolTipTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnHeadersToolTipTemplate()));
			FilterEditorCreatedEvent = EventManager.RegisterRoutedEvent("FilterEditorCreated", RoutingStrategy.Direct, typeof(FilterEditorEventHandler), ownerType);
			RowOpacityAnimationDurationProperty = DependencyPropertyManager.Register("RowOpacityAnimationDuration", typeof(Duration), ownerType, new PropertyMetadata(defaultAnimationDuration));
			ShowFilterPopupEvent = EventManager.RegisterRoutedEvent("ShowFilterPopup", RoutingStrategy.Direct, typeof(FilterPopupEventHandler), ownerType);
			PastingFromClipboardEvent = EventManager.RegisterRoutedEvent("PastingFromClipboard", RoutingStrategy.Direct, typeof(PastingFromClipboardEventHandler), ownerType);
			UnboundExpressionEditorCreatedEvent = EventManager.RegisterRoutedEvent("UnboundExpressionEditorCreated", RoutingStrategy.Direct, typeof(UnboundExpressionEditorEventHandler), ownerType);
			FocusedColumnChangedEvent = EventManager.RegisterRoutedEvent("FocusedColumnChanged", RoutingStrategy.Direct, typeof(FocusedColumnChangedEventHandler), ownerType);
			FocusedRowHandleChangedEvent = EventManager.RegisterRoutedEvent("FocusedRowHandleChanged", RoutingStrategy.Direct, typeof(FocusedRowHandleChangedEventHandler), ownerType);
			FocusedRowChangedEvent = EventManager.RegisterRoutedEvent("FocusedRowChanged", RoutingStrategy.Direct, typeof(FocusedRowChangedEventHandler), ownerType);
			FocusedViewChangedEvent = EventManager.RegisterRoutedEvent("FocusedViewChanged", RoutingStrategy.Direct, typeof(FocusedViewChangedEventHandler), ownerType);
			ShowGridMenuEvent = EventManager.RegisterRoutedEvent("ShowGridMenu", RoutingStrategy.Direct, typeof(GridMenuEventHandler), ownerType);
			ColumnHeaderClickEvent = EventManager.RegisterRoutedEvent("ColumnHeaderClick", RoutingStrategy.Direct, typeof(ColumnHeaderClickEventHandler), ownerType);
			ShowFixedTotalSummaryProperty = DependencyPropertyManager.Register("ShowFixedTotalSummary", typeof(bool), typeof(DataViewBase), new PropertyMetadata(false, (d, e) => ((DataViewBase)d).InvalidateParentTree()));
			DetailHeaderContentProperty = DependencyPropertyManager.Register("DetailHeaderContent", typeof(object), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateColumnChooserCaption()));
			ItemsSourceErrorInfoShowModeProperty = DependencyPropertyManager.Register("ItemsSourceErrorInfoShowMode", typeof(ItemsSourceErrorInfoShowMode), ownerType, new FrameworkPropertyMetadata(ItemsSourceErrorInfoShowMode.RowAndCell, (d, e) => ((DataViewBase)d).OnItemsSourceErrorInfoShowModeChanged()));
			SelectedRowsSourceProperty = DependencyPropertyManager.Register("SelectedRowsSource", typeof(IList), ownerType, new PropertyMetadata(null, (d, e) => ((DataViewBase)d).OnSelectedRowsSourceChanged()));
			AllItemsSelectedPropertyKey = DependencyProperty.RegisterReadOnly("AllItemsSelected", typeof(bool?), ownerType, new PropertyMetadata(false));
			AllItemsSelectedProperty = AllItemsSelectedPropertyKey.DependencyProperty;
			UseExtendedMouseScrollingProperty = DependencyPropertyManager.Register("UseExtendedMouseScrolling", typeof(bool), ownerType, new PropertyMetadata(true));
			EnableImmediatePostingProperty = DependencyPropertyManager.RegisterAttached("EnableImmediatePosting", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			DataNavigatorButtonsProperty = DependencyPropertyManager.RegisterAttached("DataNavigatorButtons", typeof(NavigatorButtonType), ownerType, new FrameworkPropertyMetadata(NavigatorButtonType.All));
			FilterRowDelayProperty = DependencyProperty.RegisterAttached("FilterRowDelay", typeof(int), ownerType, new PropertyMetadata(0));
			ClipboardCopyOptionsProperty = DependencyPropertyManager.RegisterAttached("ClipboardCopyOptions", typeof(ClipboardCopyOptions), ownerType, new FrameworkPropertyMetadata(ClipboardCopyOptions.All));
			ClipboardModeProperty = DependencyPropertyManager.RegisterAttached("ClipboardMode", typeof(ClipboardMode), ownerType, new FrameworkPropertyMetadata(ClipboardMode.PlainText));
			SelectionRectangleStyleProperty = DependencyPropertyManager.Register("SelectionRectangleStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateSelectionRectanle));
			ShowSelectionRectangleProperty = DependencyPropertyManager.Register("ShowSelectionRectangle", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			#region SearchPanel
			SearchPanelFindFilterProperty = DependencyPropertyManager.Register("SearchPanelFindFilter", typeof(FilterCondition), typeof(DataViewBase), new PropertyMetadata(FilterCondition.Default));
			SearchStringProperty = DependencyPropertyManager.Register("SearchString", typeof(string), typeof(DataViewBase), new PropertyMetadata(null, (d, e) => ((DataViewBase)d).UpdateSearchPanelText()));
			SearchPanelHighlightResultsProperty = DependencyPropertyManager.Register("SearchPanelHighlightResults", typeof(bool), typeof(DataViewBase),
new FrameworkPropertyMetadata(true));
			AllowLeaveInvalidEditorProperty = DependencyPropertyManager.RegisterAttached("AllowLeaveInvalidEditor", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d ,e) =>((DataViewBase)d).UpdateCellDataErrors()));
			#region Printing
			PrintHeaderTemplateProperty = DependencyProperty.Register("PrintHeaderTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCellStyleProperty = DependencyProperty.Register("PrintCellStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			PrintRowIndentStyleProperty = DependencyProperty.Register("PrintRowIndentStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			PrintSelectedRowsOnlyProperty = DependencyPropertyManager.Register("PrintSelectedRowsOnly", typeof(bool), ownerType, new UIPropertyMetadata(false));
			PrintTotalSummaryProperty = DependencyPropertyManager.Register("PrintTotalSummary", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintFixedTotalSummaryProperty = DependencyPropertyManager.Register("PrintFixedTotalSummary", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintTotalSummaryStyleProperty = DependencyPropertyManager.Register("PrintTotalSummaryStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			PrintFixedTotalSummaryStyleProperty = DependencyPropertyManager.Register("PrintFixedTotalSummaryStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			PrintFooterTemplateProperty = DependencyPropertyManager.Register("PrintFooterTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintFixedFooterTemplateProperty = DependencyPropertyManager.Register("PrintFixedFooterTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			#endregion
			ShowSearchPanelCloseButtonProperty = DependencyPropertyManager.Register("ShowSearchPanelCloseButton", typeof(bool), typeof(DataViewBase), new PropertyMetadata(true));
			SearchPanelFindModeProperty = DependencyPropertyManager.Register("SearchPanelFindMode", typeof(FindMode), typeof(DataViewBase),
	new PropertyMetadata(FindMode.Always));
			ShowSearchPanelFindButtonProperty = DependencyPropertyManager.Register("ShowSearchPanelFindButton", typeof(bool), typeof(DataViewBase), new PropertyMetadata(false));
			ShowSearchPanelMRUButtonProperty = DependencyPropertyManager.Register("ShowSearchPanelMRUButton", typeof(bool), typeof(DataViewBase), new PropertyMetadata(false));
			SearchPanelAllowFilterProperty = DependencyPropertyManager.Register("SearchPanelAllowFilter", typeof(bool), typeof(DataViewBase),
				new FrameworkPropertyMetadata(true));
			SearchPanelCriteriaOperatorTypeProperty = DependencyPropertyManager.Register("SearchPanelCriteriaOperatorType", typeof(CriteriaOperatorType), typeof(DataViewBase), new FrameworkPropertyMetadata(CriteriaOperatorType.Or));
			SearchColumnsProperty = DependencyPropertyManager.Register("SearchColumns", typeof(string), typeof(DataViewBase), new FrameworkPropertyMetadata("*", (d, e) => ((DataViewBase)d).SearchColumnsChanged((string)e.NewValue)));
			SearchPanelClearOnCloseProperty = DependencyPropertyManager.Register("SearchPanelClearOnClose", typeof(bool), typeof(DataViewBase),
		new FrameworkPropertyMetadata(true));
			ShowSearchPanelModeProperty = DependencyPropertyManager.Register("ShowSearchPanelMode", typeof(ShowSearchPanelMode), ownerType, new FrameworkPropertyMetadata(ShowSearchPanelMode.Default, (d, e) => ((DataViewBase)d).UpdateSearchPanelVisibility()));
			ActualShowSearchPanelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowSearchPanel", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((DataViewBase)d).OnActualShowSearchPanelChanged()));
			ActualShowSearchPanelProperty = ActualShowSearchPanelPropertyKey.DependencyProperty;
			SearchDelayProperty = DependencyPropertyManager.Register("SearchDelay", typeof(int), ownerType, new PropertyMetadata(1000));
			SearchPanelImmediateMRUPopupProperty = DependencyPropertyManager.Register("SearchPanelImmediateMRUPopup", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			SearchPanelHorizontalAlignmentProperty = DependencyPropertyManager.Register("SearchPanelHorizontalAlignment", typeof(HorizontalAlignment), ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			SearchControlProperty = DependencyPropertyManager.Register("SearchControl", typeof(SearchControl), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataViewBase)d).OnSearchControlChanged((SearchControl)e.OldValue, (SearchControl)e.NewValue)));
			SearchPanelNullTextProperty = DependencyPropertyManager.Register("SearchPanelNullText", typeof(string), ownerType, new FrameworkPropertyMetadata(null));
			#endregion
			RegisterClassCommandBindings();
			EventManager.RegisterClassHandler(ownerType, DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler(OnDeserializeAllowProperty));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.StartSerializingEvent, new RoutedEventHandler(OnSerializeStart));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.StartDeserializingEvent, new StartDeserializingEventHandler(OnDeserializeStart));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.EndDeserializingEvent, new EndDeserializingEventHandler(OnDeserializeEnd));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.DeserializePropertyEvent, new XtraPropertyInfoEventHandler(OnDeserializeProperty));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CustomShouldSerializePropertyEvent, new CustomShouldSerializePropertyEventHandler(OnCustomShouldSerializeProperty));
			CloneDetailHelper.RegisterKnownPropertyKeys(ownerType, ActualFadeSelectionOnLostFocusPropertyKey);
		}
		static void OnFadeSelectionOnLostFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).UpdateActualFadeSelectionOnLostFocus(d, e);
		}
		internal virtual void UpdateActualFadeSelectionOnLostFocus(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataViewBase view = (DataViewBase)d;
			view.ActualFadeSelectionOnLostFocus = view.FadeSelectionOnLostFocus;
		}
		static void OnDeserializeAllowProperty(object sender, AllowPropertyEventArgs e) {
			((DataViewBase)sender).OnDeserializeAllowPropertyInternal(e);
		}
		static void OnSerializeStart(object sender, RoutedEventArgs e) {
			((DataViewBase)sender).OnSerializeStart();
		}
		static void OnDeserializeStart(object sender, StartDeserializingEventArgs e) {
			((DataViewBase)sender).OnDeserializeStart(e);
		}
		static void OnDeserializeEnd(object sender, EndDeserializingEventArgs e) {
			((DataViewBase)sender).OnDeserializeEnd(e);
		}
		static void OnDeserializeProperty(object sender, XtraPropertyInfoEventArgs e) {
			((DataViewBase)sender).OnDeserializeProperty(e);
		}
		static void OnCustomShouldSerializeProperty(object sender, CustomShouldSerializePropertyEventArgs e) {
			((DataViewBase)sender).OnCustomShouldSerializeProperty(e);
		}
		static void OnRuntimeLocalizationStringsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).OnRuntimeLocalizationStringsChanged(e.OldValue as GridRuntimeStringCollection);
		}
		protected static void OnFocusedColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).OnFocusedColumnChanged((GridColumnBase)e.OldValue, (GridColumnBase)e.NewValue);
		}
		static partial void RegisterClassCommandBindings();
		protected static void OnUpdateShowValidationAttributeError(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).UpdateShowValidationAttributeError();
		}
		protected static void OnUpdateColumnsAppearance(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).UpdateColumnsAppearance();
		}
		static void OnColumnChooserStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).OnColumnChooserStateChanged();
		}
		static void OnFocusedRowHandleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).OnFocusedRowHandleChanged((int)e.OldValue);
		}
		static object CoerceFocusedRowHandle(DependencyObject d, object value) {
			return ((DataViewBase)d).CoerceFocusedRowHandle((int)value);
		}
		internal static void OnUpdateColumnsViewInfo(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).UpdateColumnsViewInfo();
		}
		static object CoerceTopRowIndex(DependencyObject d, object value) {
			return ((DataViewBase)d).CoerceTopRowIndex((int)value);
		}
		static void OnUpdateSelectionRectanle(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).OnUpdateSelectionRectanleChanged();
		}
		internal abstract int GroupCount { get; }
		internal virtual bool PrintAllGroupsCore { get { return true; } }
		static object CoerceScrollStep(DependencyObject d, object value) {
			if((int)value < 1) {
				return 1;
			}
			return value;
		}
		static object CoerceWheelScrollLines(DependencyObject d, object value) {
			double wheelScrollLines = (double)value;
			if(wheelScrollLines < 0 && (wheelScrollLines != DataPresenterBase.WheelScrollLinesPerPage))
				return System.Windows.SystemParameters.WheelScrollLines;
			return value;
		}
		static void OnEditorShowModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).UpdateEditorButtonVisibilities();
		}
		static void OnValidationErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataViewBase)d).ValidationErrorPropertyChanged((BaseValidationError)e.NewValue);
		}
		public static void SetRowHandle(DependencyObject element, RowHandle value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RowHandleProperty, value);
		}
		public static RowHandle GetRowHandle(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (RowHandle)element.GetValue(RowHandleProperty);
		}
		class DefaultColumnChooserColumnsSortOrderComparer : IComparer<ColumnBase> {
			public static readonly IComparer<ColumnBase> Instance = new DefaultColumnChooserColumnsSortOrderComparer();
			DefaultColumnChooserColumnsSortOrderComparer() { }
			int IComparer<ColumnBase>.Compare(ColumnBase x, ColumnBase y) {
				return Comparer<string>.Default.Compare(GetColumnChooserSortableCaption(x), GetColumnChooserSortableCaption(y));
			}
		}
		internal static string GetColumnChooserSortableCaption(BaseColumn column) {
			return column.HeaderCaption != null ? column.HeaderCaption.ToString() : string.Empty;
		}
		internal static DependencyObject GetStartHitTestObject(DependencyObject d, DataViewBase view) {
			if(!view.IsRootView)
				throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.HitTestInfoCanBeCalculatedOnlyOnTheMasterViewLevel);
			DependencyObject treeElement = d;
			while(treeElement != null && treeElement != view) {
				if(treeElement is DataViewBase)
					d = treeElement;
				treeElement = LayoutHelper.GetParent(treeElement);
			}
			return d;
		}
		[Browsable(false)]
		public void ChangeVerticalScrollOffsetBy(double offset) {
			ViewBehavior.ChangeVerticalOffsetBy(offset);
		}
		[Browsable(false)]
		public void ChangeHorizontalScrollOffsetBy(double offset) {
			ViewBehavior.ChangeHorizontalOffsetBy(offset);
		}
		#region hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new FontFamily FontFamily { get { return base.FontFamily; } set { base.FontFamily = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new double FontSize { get { return base.FontSize; } set { base.FontSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new FontStretch FontStretch { get { return base.FontStretch; } set { base.FontStretch = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new FontWeight FontWeight { get { return base.FontWeight; } set { base.FontWeight = value; } }
		#endregion
		#region public properties
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool? AllItemsSelected {
			get { return (bool?)GetValue(AllItemsSelectedProperty); }
			private set { SetValue(AllItemsSelectedPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseUseExtendedMouseScrolling"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool UseExtendedMouseScrolling {
			get { return (bool)GetValue(UseExtendedMouseScrollingProperty); }
			set { SetValue(UseExtendedMouseScrollingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFadeSelectionOnLostFocus"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool FadeSelectionOnLostFocus {
			get { return (bool)GetValue(FadeSelectionOnLostFocusProperty); }
			set { SetValue(FadeSelectionOnLostFocusProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Force)]
		public bool ActualFadeSelectionOnLostFocus {
			get { return (bool)GetValue(ActualFadeSelectionOnLostFocusProperty); }
			protected set { this.SetValue(ActualFadeSelectionOnLostFocusPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseRowAnimationKind")]
#endif
		public RowAnimationKind RowAnimationKind {
			get { return (RowAnimationKind)GetValue(RowAnimationKindProperty); }
			set { SetValue(RowAnimationKindProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseWaitIndicatorType"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty]
		public WaitIndicatorType WaitIndicatorType {
			get { return (WaitIndicatorType)GetValue(WaitIndicatorTypeProperty); }
			set { SetValue(WaitIndicatorTypeProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsWaitIndicatorVisible")]
#endif
		public bool IsWaitIndicatorVisible {
			get { return (bool)GetValue(IsWaitIndicatorVisibleProperty); }
			internal set { this.SetValue(IsWaitIndicatorVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseWaitIndicatorStyle")]
#endif
		public Style WaitIndicatorStyle {
			get { return (Style)GetValue(WaitIndicatorStyleProperty); }
			set { SetValue(WaitIndicatorStyleProperty, value); }
		}
		public Style SelectionRectangleStyle {
			get { return (Style)GetValue(SelectionRectangleStyleProperty); }
			set { SetValue(SelectionRectangleStyleProperty, value); }
		}
		public bool ShowSelectionRectangle {
			get { return (bool)GetValue(ShowSelectionRectangleProperty); }
			set { SetValue(ShowSelectionRectangleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseRuntimeLocalizationStrings"),
#endif
 Category(Categories.Appearance)]
		public GridRuntimeStringCollection RuntimeLocalizationStrings {
			get { return (GridRuntimeStringCollection)GetValue(RuntimeLocalizationStringsProperty); }
			set { SetValue(RuntimeLocalizationStringsProperty, value); }
		}
		[Browsable(false)]
		public LocalizationDescriptor LocalizationDescriptor {
			get { return (LocalizationDescriptor)GetValue(LocalizationDescriptorProperty); }
			internal set { this.SetValue(LocalizationDescriptorPropertyKey, value); }
		}
		[Obsolete("Use the DataControlBase.CurrentColumnChanged event instead"), Category("Behavior")]
		public event FocusedColumnChangedEventHandler FocusedColumnChanged {
			add { AddHandler(FocusedColumnChangedEvent, value); }
			remove { RemoveHandler(FocusedColumnChangedEvent, value); }
		}
		[Category("Behavior")]
		public event FocusedRowHandleChangedEventHandler FocusedRowHandleChanged {
			add { AddHandler(FocusedRowHandleChangedEvent, value); }
			remove { RemoveHandler(FocusedRowHandleChangedEvent, value); }
		}
		[Obsolete("Use the DataControlBase.CurrentItemChanged event instead"), Category("Behavior")]
		public event FocusedRowChangedEventHandler FocusedRowChanged {
			add { AddHandler(FocusedRowChangedEvent, value); }
			remove { RemoveHandler(FocusedRowChangedEvent, value); }
		}
		[Category("Behavior")]
		public event FocusedViewChangedEventHandler FocusedViewChanged {
			add { AddHandler(FocusedViewChangedEvent, value); }
			remove { RemoveHandler(FocusedViewChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowColumnFiltering"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool AllowColumnFiltering {
			get { return (bool)GetValue(AllowColumnFilteringProperty); }
			set { SetValue(AllowColumnFilteringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowFilterEditor"),
#endif
 Category(Categories.OptionsFilter)]
		public bool AllowFilterEditor {
			get { return (bool)GetValue(AllowFilterEditorProperty); }
			set { SetValue(AllowFilterEditorProperty, value); }
		}
		[Browsable(false)]
		public bool ShowEditFilterButton {
			get { return (bool)GetValue(ShowEditFilterButtonProperty); }
			protected set { this.SetValue(ShowEditFilterButtonPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowSorting"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowSorting {
			get { return (bool)GetValue(AllowSortingProperty); }
			set { SetValue(AllowSortingProperty, value); }
		}
		[Obsolete("Instead use the AllowColumnMoving property.")]
		[XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never), Browsable(false), 
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowMoving"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.OptionsBehavior)]
#pragma warning disable 618
		public bool AllowMoving {
			get { return (bool)GetValue(AllowMovingProperty); }
			set { SetValue(AllowMovingProperty, value); }
		}
#pragma warning restore 618
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowColumnMoving"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowColumnMoving {
			get { return (bool)GetValue(AllowColumnMovingProperty); }
			set { SetValue(AllowColumnMovingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowEditing"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowEditing {
			get { return (bool)GetValue(AllowEditingProperty); }
			set { SetValue(AllowEditingProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public int FocusedRowHandle {
			get { return (int)DependencyObjectHelper.GetCoerceValue(this, FocusedRowHandleProperty); }
			set { SetValue(FocusedRowHandleProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowScrollToFocusedRow {
			get { return (bool)GetValue(AllowScrollToFocusedRowProperty); }
			set { SetValue(AllowScrollToFocusedRowProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetIsFocusedRow(DependencyObject dependencyObject, bool focused) {
			dependencyObject.SetValue(IsFocusedRowProperty, focused);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static bool GetIsFocusedRow(DependencyObject dependencyObject) {
			return (bool)dependencyObject.GetValue(IsFocusedRowProperty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetIsFocusedCell(DependencyObject dependencyObject, bool focused) {
			dependencyObject.SetValue(IsFocusedCellProperty, focused);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static bool GetIsFocusedCell(DependencyObject dependencyObject) {
			return (bool)dependencyObject.GetValue(IsFocusedCellProperty);
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseScrollStep"),
#endif
 Category(Categories.OptionsBehavior), CloneDetailMode(CloneDetailMode.Skip)]
		public int ScrollStep {
			get { return (int)GetValue(ScrollStepProperty); }
			set { SetValue(ScrollStepProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseHasValidationError")]
#endif
		public bool HasValidationError {
			get { return (bool)GetValue(HasValidationErrorProperty); }
			internal set { this.SetValue(HasValidationErrorPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseValidationError")]
#endif
		public BaseValidationError ValidationError {
			get { return (BaseValidationError)GetValue(ValidationErrorProperty); }
			internal set { this.SetValue(ValidationErrorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowCommitOnValidationAttributeError"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowCommitOnValidationAttributeError {
			get { return (bool)GetValue(AllowCommitOnValidationAttributeErrorProperty); }
			set { SetValue(AllowCommitOnValidationAttributeErrorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseScrollingMode"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public ScrollingMode ScrollingMode {
			get { return (ScrollingMode)GetValue(ScrollingModeProperty); }
			set { SetValue(ScrollingModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsDeferredScrolling"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool IsDeferredScrolling {
			get { return (bool)GetValue(IsDeferredScrollingProperty); }
			set { SetValue(IsDeferredScrollingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseEditorButtonShowMode"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public EditorButtonShowMode EditorButtonShowMode {
			get { return (EditorButtonShowMode)GetValue(EditorButtonShowModeProperty); }
			set { SetValue(EditorButtonShowModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowMoveColumnToDropArea"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowMoveColumnToDropArea {
			get { return (bool)GetValue(AllowMoveColumnToDropAreaProperty); }
			set { SetValue(AllowMoveColumnToDropAreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnChooserTemplate"),
#endif
 Category(Categories.Appearance), CloneDetailMode(CloneDetailMode.Skip)]
		public ControlTemplate ColumnChooserTemplate {
			get { return (ControlTemplate)GetValue(ColumnChooserTemplateProperty); }
			set { SetValue(ColumnChooserTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseActualColumnChooserTemplate")]
#endif
		public ControlTemplate ActualColumnChooserTemplate {
			get { return (ControlTemplate)GetValue(ActualColumnChooserTemplateProperty); }
			protected set { this.SetValue(ActualColumnChooserTemplatePropertyKey, value); }
		}
		[Browsable(false)]
		public bool IsColumnChooserVisible {
			get { return (bool)GetValue(IsColumnChooserVisibleProperty); }
			set { SetValue(IsColumnChooserVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderDragIndicatorTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate ColumnHeaderDragIndicatorTemplate {
			get { return (DataTemplate)base.GetValue(ColumnHeaderDragIndicatorTemplateProperty); }
			set { SetValue(ColumnHeaderDragIndicatorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnChooserColumns"),
#endif
 Browsable(false)]
		public ReadOnlyCollection<ColumnBase> ColumnChooserColumns {
			get { return (ReadOnlyCollection<ColumnBase>)GetValue(ColumnChooserColumnsProperty); }
			protected internal set { this.SetValue(ColumnChooserColumnsPropertyKey, value); }
		}
		[Browsable(false)]
		public double ScrollableAreaMinWidth {
			get { return (double)GetValue(ScrollableAreaMinWidthProperty); }
			protected set { this.SetValue(ScrollableAreaMinWidthPropertyKey, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public int TopRowIndex {
			get { return (int)GetValue(TopRowIndexProperty); }
			set { SetValue(TopRowIndexProperty, value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowLeaveFocusOnTab {
			get { return (bool)GetValue(AllowLeaveFocusOnTabProperty); }
			set { SetValue(AllowLeaveFocusOnTabProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseWheelScrollLines"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public double WheelScrollLines {
			get { return (double)GetValue(WheelScrollLinesProperty); }
			set { SetValue(WheelScrollLinesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseTouchScrollThreshold"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public double TouchScrollThreshold {
			get { return (double)GetValue(TouchScrollThresholdProperty); }
			set { SetValue(TouchScrollThresholdProperty, value); }
		}
		public object DetailHeaderContent {
			get { return GetValue(DetailHeaderContentProperty); }
			set { SetValue(DetailHeaderContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseItemsSourceErrorInfoShowMode"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public ItemsSourceErrorInfoShowMode ItemsSourceErrorInfoShowMode {
			get { return (ItemsSourceErrorInfoShowMode)GetValue(ItemsSourceErrorInfoShowModeProperty); }
			set { SetValue(ItemsSourceErrorInfoShowModeProperty, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserColumns(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserColumnsSortOrderComparer(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		public event RowAnimationEventHandler RowAnimationBegin {
			add { AddHandler(RowAnimationBeginEvent, value); }
			remove { RemoveHandler(RowAnimationBeginEvent, value); }
		}
		public static void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ColumnHeaderDragIndicatorSizeProperty, value);
		}
		public static double GetColumnHeaderDragIndicatorSize(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (double)element.GetValue(ColumnHeaderDragIndicatorSizeProperty);
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseActiveEditor")]
#endif
		public BaseEdit ActiveEditor {
			get { return (BaseEdit)GetValue(ActiveEditorProperty); }
			private set { this.SetValue(ActiveEditorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseEditorShowMode"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public EditorShowMode EditorShowMode {
			get { return (EditorShowMode)GetValue(EditorShowModeProperty); }
			set { SetValue(EditorShowModeProperty, value); }
		}
		internal void SetActiveEditor() {
			SetActiveEditor((BaseEdit)BaseEditHelper.GetBaseEdit(InplaceEditorOwner.ActiveEditor));
		}
		internal void SetActiveEditor(BaseEdit baseEdit) {
			IsEditing = baseEdit != null;
			ActiveEditor = baseEdit;
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseNavigationStyle"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public GridViewNavigationStyle NavigationStyle {
			get { return (GridViewNavigationStyle)GetValue(NavigationStyleProperty); }
			set { SetValue(NavigationStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowFocusedRectangle"),
#endif
 Category(Categories.Appearance)]
		public bool ShowFocusedRectangle {
			get { return (bool)GetValue(ShowFocusedRectangleProperty); }
			set { SetValue(ShowFocusedRectangleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFocusedCellBorderTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate FocusedCellBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedCellBorderTemplateProperty); }
			set { SetValue(FocusedCellBorderTemplateProperty, value); }
		}
		public ControlTemplate FocusedGroupRowBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedGroupRowBorderTemplateProperty); }
			set { SetValue(FocusedGroupRowBorderTemplateProperty, value); }
		}
		[Obsolete("Use the DataControlBase.ClipboardCopyMode property instead"), Category(Categories.OptionsCopy), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ClipboardCopyWithHeaders {
			get { return (bool)GetValue(ClipboardCopyWithHeadersProperty); }
			set { SetValue(ClipboardCopyWithHeadersProperty, value); }
		}
		protected internal virtual bool ActualClipboardCopyWithHeaders {
		   get { return (DataControl == null || DataControl.ClipboardCopyMode == ClipboardCopyMode.Default) ? (bool)GetValue(ClipboardCopyWithHeadersProperty) : DataControl.ClipboardCopyMode == ClipboardCopyMode.IncludeHeader; }
		}
		[Obsolete("Use the DataControlBase.ClipboardCopyMode property instead"), Category(Categories.OptionsCopy), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ClipboardCopyAllowed {
			get { return (bool)GetValue(ClipboardCopyAllowedProperty); }
			set { SetValue(ClipboardCopyAllowedProperty, value); }
		}
		protected internal virtual bool ActualClipboardCopyAllowed {
			get { return (DataControl == null || DataControl.ClipboardCopyMode == ClipboardCopyMode.Default) ? (bool)GetValue(ClipboardCopyAllowedProperty) : DataControl.ClipboardCopyMode != ClipboardCopyMode.None; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsKeyboardFocusWithinView {
			get { return (bool)GetValue(IsKeyboardFocusWithinViewProperty); }
			private set { this.SetValue(IsKeyboardFocusWithinViewPropertyKey, value); }
		}
		[Obsolete("Use the DataControlBase.SelectedItem/DataControlBase.CurrentItem property instead"), Category(Categories.Data), CloneDetailMode(CloneDetailMode.Skip)]
		public object FocusedRow {
			get { return (object)GetValue(FocusedRowProperty); }
			set { SetValue(FocusedRowProperty, value); }
		}
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFocusedRowData")]
#endif
		public RowData FocusedRowData {
			get { return (RowData)GetValue(FocusedRowDataProperty); }
			internal set { this.SetValue(FocusedRowDataPropertyKey, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public IColumnChooserFactory ColumnChooserFactory {
			get { return (IColumnChooserFactory)GetValue(ColumnChooserFactoryProperty); }
			set { SetValue(ColumnChooserFactoryProperty, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public bool IsHorizontalScrollBarVisible {
			get { return (bool)GetValue(IsHorizontalScrollBarVisibleProperty); }
			internal set { this.SetValue(IsHorizontalScrollBarVisiblePropertyKey, value); } 
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public bool IsTouchScrollBarsMode {
			get { return (bool)GetValue(IsTouchScrollBarsModeProperty); }
			internal set { this.SetValue(IsTouchScrollBarsModePropertyKey, value); } 
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		GridUIProperty, CloneDetailMode(CloneDetailMode.Skip)
		]
		public IColumnChooserState ColumnChooserState {
			get { return (IColumnChooserState)GetValue(ColumnChooserStateProperty); }
			set { SetValue(ColumnChooserStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseCellTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseCellTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		[Category(Categories.OptionsFilter)]
		public event FilterPopupEventHandler ShowFilterPopup {
			add { AddHandler(ShowFilterPopupEvent, value); }
			remove { RemoveHandler(ShowFilterPopupEvent, value); }
		}
		[Obsolete("Use the DataControlBase.PastingFromClipboard event"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event PastingFromClipboardEventHandler PastingFromClipboard {
			add { AddHandler(PastingFromClipboardEvent, value); }
			remove { RemoveHandler(PastingFromClipboardEvent, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public event UnboundExpressionEditorEventHandler UnboundExpressionEditorCreated {
			add { AddHandler(UnboundExpressionEditorCreatedEvent, value); }
			remove { RemoveHandler(UnboundExpressionEditorCreatedEvent, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseRowOpacityAnimationDuration")]
#endif
		public Duration RowOpacityAnimationDuration {
			get { return (Duration)GetValue(RowOpacityAnimationDurationProperty); }
			set { SetValue(RowOpacityAnimationDurationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAutoScrollOnSorting"),
#endif
 Category(Categories.OptionsView)]
		public bool AutoScrollOnSorting {
			get { return (bool)GetValue(AutoScrollOnSortingProperty); }
			set { SetValue(AutoScrollOnSortingProperty, value); }
		}
		internal bool IsEditingCore { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsEditing")]
#endif
		public bool IsEditing {
			get { return (bool)GetValue(IsEditingProperty); }
			internal set { this.SetValue(IsEditingPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsFocusedRowModified")]
#endif
		public bool IsFocusedRowModified {
			get { return (bool)GetValue(IsFocusedRowModifiedProperty); }
			internal set { this.SetValue(IsFocusedRowModifiedPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFilterEditorShowOperandTypeIcon"),
#endif
 Category(Categories.OptionsFilter)]
		public bool FilterEditorShowOperandTypeIcon {
			get { return (bool)GetValue(FilterEditorShowOperandTypeIconProperty); }
			set { SetValue(FilterEditorShowOperandTypeIconProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowValidationAttributeErrors"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool ShowValidationAttributeErrors {
			get { return (bool)GetValue(ShowValidationAttributeErrorsProperty); }
			set { SetValue(ShowValidationAttributeErrorsProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseActualShowFilterPanel")]
#endif
		public bool ActualShowFilterPanel {
			get { return (bool)GetValue(ActualShowFilterPanelProperty); }
			private set { this.SetValue(ActualShowFilterPanelPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFilterPanelText")]
#endif
		public string FilterPanelText {
			get { return (string)GetValue(FilterPanelTextProperty); }
			private set { this.SetValue(FilterPanelTextPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowFilterPanelMode"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public ShowFilterPanelMode ShowFilterPanelMode {
			get { return (ShowFilterPanelMode)GetValue(ShowFilterPanelModeProperty); }
			set { SetValue(ShowFilterPanelModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowTotalSummary"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowTotalSummary {
			get { return (bool)GetValue(ShowTotalSummaryProperty); }
			set { SetValue(ShowTotalSummaryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowColumnHeaders"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowColumnHeaders {
			get { return (bool)GetValue(ShowColumnHeadersProperty); }
			set { SetValue(ShowColumnHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseTotalSummaryContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style TotalSummaryContentStyle {
			get { return (Style)GetValue(TotalSummaryContentStyleProperty); }
			set { SetValue(TotalSummaryContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style ColumnHeaderContentStyle {
			get { return (Style)GetValue(ColumnHeaderContentStyleProperty); }
			set { SetValue(ColumnHeaderContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseTotalSummaryItemTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate TotalSummaryItemTemplate {
			get { return (DataTemplate)GetValue(TotalSummaryItemTemplateProperty); }
			set { SetValue(TotalSummaryItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseTotalSummaryItemTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector TotalSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(TotalSummaryItemTemplateSelectorProperty); }
			set { SetValue(TotalSummaryItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseActualTotalSummaryItemTemplateSelector")]
#endif
		public DataTemplateSelector ActualTotalSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualTotalSummaryItemTemplateSelectorProperty); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseHeaderTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseFooterTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate FooterTemplate {
			get { return (DataTemplate)GetValue(FooterTemplateProperty); }
			set { SetValue(FooterTemplateProperty, value); }
		}
		[Category(Categories.OptionsCustomization)]
		public event RoutedEventHandler ShownColumnChooser {
			add { AddHandler(ShownColumnChooserEvent, value); }
			remove { RemoveHandler(ShownColumnChooserEvent, value); }
		}
		[Category(Categories.OptionsCustomization)]
		public event RoutedEventHandler HiddenColumnChooser {
			add { AddHandler(HiddenColumnChooserEvent, value); }
			remove { RemoveHandler(HiddenColumnChooserEvent, value); }
		}
		public event CancelRoutedEventHandler BeforeLayoutRefresh {
			add { AddHandler(BeforeLayoutRefreshEvent, value); }
			remove { RemoveHandler(BeforeLayoutRefreshEvent, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public event CustomFilterDisplayTextEventHandler CustomFilterDisplayText {
			add { AddHandler(CustomFilterDisplayTextEvent, value); }
			remove { RemoveHandler(CustomFilterDisplayTextEvent, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public event FilterEditorEventHandler FilterEditorCreated {
			add { AddHandler(FilterEditorCreatedEvent, value); }
			remove { RemoveHandler(FilterEditorCreatedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate ColumnHeaderTemplate {
			get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
			set { SetValue(ColumnHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector ColumnHeaderTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(ColumnHeaderTemplateSelectorProperty); }
			set { SetValue(ColumnHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderCustomizationAreaTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate ColumnHeaderCustomizationAreaTemplate {
			get { return (DataTemplate)GetValue(ColumnHeaderCustomizationAreaTemplateProperty); }
			set { SetValue(ColumnHeaderCustomizationAreaTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderCustomizationAreaTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector ColumnHeaderCustomizationAreaTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(ColumnHeaderCustomizationAreaTemplateSelectorProperty); }
			set { SetValue(ColumnHeaderCustomizationAreaTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsColumnMenuEnabled"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsColumnMenuEnabled {
			get { return (bool)GetValue(IsColumnMenuEnabledProperty); }
			set { SetValue(IsColumnMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsTotalSummaryMenuEnabled"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsTotalSummaryMenuEnabled {
			get { return (bool)GetValue(IsTotalSummaryMenuEnabledProperty); }
			set { SetValue(IsTotalSummaryMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseIsRowCellMenuEnabled"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsRowCellMenuEnabled {
			get { return (bool)GetValue(IsRowCellMenuEnabledProperty); }
			set { SetValue(IsRowCellMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseColumnHeaderToolTipTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate ColumnHeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(ColumnHeaderToolTipTemplateProperty); }
			set { SetValue(ColumnHeaderToolTipTemplateProperty, value); }
		}
		public event GridMenuEventHandler ShowGridMenu {
			add { AddHandler(ShowGridMenuEvent, value); }
			remove { RemoveHandler(ShowGridMenuEvent, value); }
		}
		public event ColumnHeaderClickEventHandler ColumnHeaderClick {
			add { this.AddHandler(ColumnHeaderClickEvent, value); }
			remove { this.RemoveHandler(ColumnHeaderClickEvent, value); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseCommands")]
#endif
		public DataViewCommandsBase Commands { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSummariesIgnoreNullValues")]
#endif
		public bool SummariesIgnoreNullValues {
			get { return (bool)GetValue(SummariesIgnoreNullValuesProperty); }
			set { SetValue(SummariesIgnoreNullValuesProperty, value); }
		}
		public bool EnterMoveNextColumn {
			get { return (bool)GetValue(EnterMoveNextColumnProperty); }
			set { SetValue(EnterMoveNextColumnProperty, value); }
		}
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip)]
		public IComparer<ColumnBase> ColumnChooserColumnsSortOrderComparer {
			get { return (IComparer<ColumnBase>)GetValue(ColumnChooserColumnsSortOrderComparerProperty); }
			set { SetValue(ColumnChooserColumnsSortOrderComparerProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowFixedTotalSummary"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowFixedTotalSummary {
			get { return (bool)GetValue(ShowFixedTotalSummaryProperty); }
			set { SetValue(ShowFixedTotalSummaryProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseEnableImmediatePosting")]
#endif
		public bool EnableImmediatePosting {
			get { return (bool)GetValue(EnableImmediatePostingProperty); }
			set { SetValue(EnableImmediatePostingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseAllowLeaveInvalidEditor")]
#endif
		public bool AllowLeaveInvalidEditor {
			get { return (bool)GetValue(AllowLeaveInvalidEditorProperty); }
			set { SetValue(AllowLeaveInvalidEditorProperty, value); }
		}
		#region Printing proeprties
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintHeaderTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintHeaderTemplate {
			get { return (DataTemplate)GetValue(PrintHeaderTemplateProperty); }
			set { SetValue(PrintHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintCellStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintCellStyle {
			get { return (Style)GetValue(PrintCellStyleProperty); }
			set { SetValue(PrintCellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintRowIndentStyle"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public Style PrintRowIndentStyle {
			get { return (Style)GetValue(PrintRowIndentStyleProperty); }
			set { SetValue(PrintRowIndentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintSelectedRowsOnly"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintSelectedRowsOnly {
			get { return (bool)GetValue(PrintSelectedRowsOnlyProperty); }
			set { SetValue(PrintSelectedRowsOnlyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintTotalSummary"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintTotalSummary {
			get { return (bool)GetValue(PrintTotalSummaryProperty); }
			set { SetValue(PrintTotalSummaryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintFixedTotalSummary"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintFixedTotalSummary {
			get { return (bool)GetValue(PrintFixedTotalSummaryProperty); }
			set { SetValue(PrintFixedTotalSummaryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintTotalSummaryStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintTotalSummaryStyle {
			get { return (Style)GetValue(PrintTotalSummaryStyleProperty); }
			set { SetValue(PrintTotalSummaryStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintFixedTotalSummaryStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintFixedTotalSummaryStyle {
			get { return (Style)GetValue(PrintFixedTotalSummaryStyleProperty); }
			set { SetValue(PrintFixedTotalSummaryStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintFooterTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintFooterTemplate {
			get { return (DataTemplate)GetValue(PrintFooterTemplateProperty); }
			set { SetValue(PrintFooterTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBasePrintFixedFooterTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintFixedFooterTemplate {
			get { return (DataTemplate)GetValue(PrintFixedFooterTemplateProperty); }
			set { SetValue(PrintFixedFooterTemplateProperty, value); }
		}
		#endregion
#if DEBUGTEST
		public
#else
		internal
#endif
			FixedSummariesHelper FixedSummariesHelper {
			get { return fixedSummariesHelper; }
		}
		public IList<GridTotalSummaryData> FixedSummariesLeft {
			get { return fixedSummariesLeft; }
		}
		public IList<GridTotalSummaryData> FixedSummariesRight {
			get { return fixedSummariesRight; }
		}
		protected internal virtual bool ForceShowTotalSummaryColumnName { get { return false; } }
		[Obsolete("Use the DataControlBase.SelectedItems property instead"), Browsable(false)]
		public IList SelectedRows { get { return DataControl == null ? null : DataControl.SelectedItems; } }
		[Obsolete("Use the DataControlBase.SelectedItems property instead"), CloneDetailMode(CloneDetailMode.Skip)]
		public IList SelectedRowsSource {
			get { return (IList)GetValue(SelectedRowsSourceProperty); }
			set { SetValue(SelectedRowsSourceProperty, value); }
		}
		[Obsolete("Use the DataControlBase.GetSelectedRowHandles method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int[] GetSelectedRowHandles() {
			return GetSelectedRowHandlesCore();
		}
		internal int[] GetSelectedRowHandlesCore() {
			return SelectionStrategy.GetSelectedRows();
		}
		#region SearchPanel
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowSearchPanelCloseButton"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowSearchPanelCloseButton {
			get { return (bool)GetValue(ShowSearchPanelCloseButtonProperty); }
			set { SetValue(ShowSearchPanelCloseButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelFindFilter"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public FilterCondition SearchPanelFindFilter {
			get { return (FilterCondition)GetValue(SearchPanelFindFilterProperty); }
			set { SetValue(SearchPanelFindFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchString"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, GridUIProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public string SearchString {
			get { return (string)GetValue(SearchStringProperty); }
			set { SetValue(SearchStringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelHighlightResults"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool SearchPanelHighlightResults {
			get { return (bool)GetValue(SearchPanelHighlightResultsProperty); }
			set { SetValue(SearchPanelHighlightResultsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowSearchPanelFindButton"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowSearchPanelFindButton {
			get { return (bool)GetValue(ShowSearchPanelFindButtonProperty); }
			set { SetValue(ShowSearchPanelFindButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelFindMode"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public FindMode SearchPanelFindMode {
			get { return (FindMode)GetValue(SearchPanelFindModeProperty); }
			set { SetValue(SearchPanelFindModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowSearchPanelMRUButton"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowSearchPanelMRUButton {
			get { return (bool)GetValue(ShowSearchPanelMRUButtonProperty); }
			set { SetValue(ShowSearchPanelMRUButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelAllowFilter"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool SearchPanelAllowFilter {
			get { return (bool)GetValue(SearchPanelAllowFilterProperty); }
			set { SetValue(SearchPanelAllowFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelCriteriaOperatorType"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public CriteriaOperatorType SearchPanelCriteriaOperatorType {
			get { return (CriteriaOperatorType)GetValue(SearchPanelCriteriaOperatorTypeProperty); }
			set { SetValue(SearchPanelCriteriaOperatorTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchColumns"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public string SearchColumns {
			get { return (string)GetValue(SearchColumnsProperty); }
			set { SetValue(SearchColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelClearOnClose"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool SearchPanelClearOnClose {
			get { return (bool)GetValue(SearchPanelClearOnCloseProperty); }
			set { SetValue(SearchPanelClearOnCloseProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseShowSearchPanelMode"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public ShowSearchPanelMode ShowSearchPanelMode {
			get { return (ShowSearchPanelMode)GetValue(ShowSearchPanelModeProperty); }
			set { SetValue(ShowSearchPanelModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseActualShowSearchPanel"),
#endif
 XtraSerializableProperty, XtraResetProperty(ResetPropertyMode.None)]
		public bool ActualShowSearchPanel {
			get { return (bool)GetValue(ActualShowSearchPanelProperty); }
			private set { this.SetValue(ActualShowSearchPanelPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchDelay"),
#endif
 Category(Categories.SearchPanel), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public int SearchDelay {
			get { return (int)GetValue(SearchDelayProperty); }
			set { SetValue(SearchDelayProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelImmediateMRUPopup"),
#endif
 XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool? SearchPanelImmediateMRUPopup {
			get { return (bool?)GetValue(SearchPanelImmediateMRUPopupProperty); }
			set { SetValue(SearchPanelImmediateMRUPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelHorizontalAlignment"),
#endif
 XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public HorizontalAlignment SearchPanelHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(SearchPanelHorizontalAlignmentProperty); }
			set { SetValue(SearchPanelHorizontalAlignmentProperty, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeSearchPanelImmediateMRUPopup(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		#endregion
		public NavigatorButtonType DataNavigatorButtons {
			get { return (NavigatorButtonType)GetValue(DataNavigatorButtonsProperty); }
			set { SetValue(DataNavigatorButtonsProperty, value); }
		}
		public int FilterRowDelay {
			get { return (int)GetValue(FilterRowDelayProperty); }
			set { SetValue(FilterRowDelayProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseClipboardCopyOptions")]
#endif
		public ClipboardCopyOptions ClipboardCopyOptions {
			get { return (ClipboardCopyOptions)GetValue(ClipboardCopyOptionsProperty); }
			set { SetValue(ClipboardCopyOptionsProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseClipboardMode")]
#endif
		public ClipboardMode ClipboardMode {
			get { return (ClipboardMode)GetValue(ClipboardModeProperty); }
			set { SetValue(ClipboardModeProperty, value); }
		}	 
		#endregion
		#region clipboard
		RowsClipboardController clipboardController;
		protected internal RowsClipboardController ClipboardController {
			get {
				if(clipboardController == null) 
					clipboardController = CreateClipboardController();
				return clipboardController;
			}
		}
		protected abstract RowsClipboardController CreateClipboardController();
		[Obsolete("Use the DataControlBase.CopySelectedItemsToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopySelectedRowsToClipboard() {
			CopySelectedRowsToClipboardCore();
		}
		internal void CopySelectedRowsToClipboardCore() {
			ClipboardController.CopyRowsToClipboard(GetSelectedRowHandlesCore());
		}
		[Obsolete("Use the DataControlBase.CopyCurrentItemToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public virtual void CopyFocusedRowToClipboard() {
			CopyFocusedRowToClipboardCore();
		}
		internal void CopyFocusedRowToClipboardCore() {
			List<int> list = new List<int>();
			if(!IsInvalidFocusedRowHandle)
				list.Add(FocusedRowHandle);
			ClipboardController.CopyRowsToClipboard(list);
		}
		[Obsolete("Use the DataControlBase.CopyRowsToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopyRowsToClipboard(IEnumerable<int> rows) {
			CopyRowsToClipboardCore(rows);
		}
		internal void CopyRowsToClipboardCore(IEnumerable<int> rows) {
			ClipboardController.CopyRowsToClipboard(rows); 
		}
		[Obsolete("Use the DataControlBase.CopyRangeToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopyRangeToClipboard(int startRowHandle, int endRowHandle) {
			CopyRangeToClipboardCore(startRowHandle, endRowHandle);
		}
		internal void CopyRangeToClipboardCore(int startRowHandle, int endRowHandle) {
			ClipboardController.CopyRangeToClipboard(startRowHandle, endRowHandle);
		}
		internal protected virtual void GetHeadersText(StringBuilder sb) {
			GetDataRowText(sb, DataControlBase.InvalidRowHandle);
		}
		internal void CopyAllRowsToClipboardCore(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			ClipboardController.CopyRowsToClipboard(rows);
		}
		#region ClipboardFormat
		protected void SetActualClipboardOptions(ClipboardOptions options) {
			options.AllowCsvFormat = ClipboardCopyOptions.HasFlag(ClipboardCopyOptions.Csv) ? DefaultBoolean.True : DefaultBoolean.False;
			options.AllowExcelFormat = ClipboardCopyOptions.HasFlag(ClipboardCopyOptions.Excel) ? DefaultBoolean.True : DefaultBoolean.False;
			options.AllowHtmlFormat = ClipboardCopyOptions.HasFlag(ClipboardCopyOptions.Html) ? DefaultBoolean.True : DefaultBoolean.False;
			options.AllowRtfFormat = ClipboardCopyOptions.HasFlag(ClipboardCopyOptions.Rtf) ? DefaultBoolean.True : DefaultBoolean.False;
			options.AllowTxtFormat = ClipboardCopyOptions.HasFlag(ClipboardCopyOptions.Txt) ? DefaultBoolean.True : DefaultBoolean.False;
			options.ClipboardMode = this.ClipboardMode == DevExpress.Xpf.Grid.ClipboardMode.Formatted ? Export.ClipboardMode.Formatted : Export.ClipboardMode.PlainText;
			options.CopyColumnHeaders = ActualClipboardCopyWithHeaders ? DefaultBoolean.True : DefaultBoolean.False;
		}
		ClipboardOptions _optionsClipboard;
		protected internal ClipboardOptions OptionsClipboard {
			get {
				if(_optionsClipboard == null)
					_optionsClipboard = CreateOptionsClipboard();
				return _optionsClipboard;
			}
		}
		ClipboardOptions CreateOptionsClipboard() {
			ClipboardOptions opt = new ClipboardOptions();
			SetActualClipboardOptions(opt);
			return opt;
		}
		protected internal virtual bool SetDataAwareClipboardData() { return false; }
		#endregion
		#endregion
		#region Export
		internal bool ShouldPrintTotalSummary { get { return ShowTotalSummary && PrintTotalSummary; } }
		internal bool ShouldPrintFixedTotalSummary { get { return ShowFixedTotalSummary && PrintFixedTotalSummary; } }
		protected internal virtual object GetExportValue(int rowHandle, ColumnBase column) {
			object editValue = DataControl.GetCellValueCore(rowHandle, column);
			if(IsInLookUpMode(column) || editValue is Enum || column.ActualEditSettings.DisplayTextConverter != null)
				return DataControl.GetCellDisplayText(rowHandle, column.FieldName);
			return editValue;
		}
		protected internal virtual object GetExportValueFromItem(object item, ColumnBase column) {
			if(item is Enum)
				return GetColumnDisplayText(item, column);
			if(IsInLookUpMode(column) || column.ActualEditSettings.DisplayTextConverter != null) {
				object editValue = ((LookUpEditSettingsBase)column.ActualEditSettings).GetValueFromItem(item);
				return GetColumnDisplayText(editValue, column);
			}
			return item;
		}
		internal static bool IsInLookUpMode(ColumnBase column) {
			return column.EditSettings is LookUpEditSettingsBase && DevExpress.Xpf.Editors.Native.LookUpEditHelper.IsInLookUpMode((LookUpEditSettingsBase)column.EditSettings);
		}
		#endregion
		protected internal void RaiseRowAnimationBegin(LoadingAnimationHelper loadingAnimationHelper, bool isGroupRow) {
			RowAnimationBeginEventArgs eArgs = new RowAnimationBeginEventArgs(this) {
				RoutedEvent = DataViewBase.RowAnimationBeginEvent,
				IsGroupRow = isGroupRow
			};
			RaiseEventInOriginationView(eArgs);
			loadingAnimationHelper.CustomAnimation = eArgs.AnimationTimeline;
			loadingAnimationHelper.CustomPropertyPath = eArgs.PropertyPath;
			loadingAnimationHelper.AddedEffect = eArgs.AddedEffect;
		}
		protected internal virtual CriteriaOperator CreateAutoFilterCriteria(string fieldName, AutoFilterCondition condition, object value) {
			if(condition == AutoFilterCondition.Equals) {
				ColumnBase column = DataControl.ColumnsCore[fieldName];
				if(column.ActualEditSettings is CheckEditSettings && column.ColumnFilterMode == ColumnFilterMode.DisplayText)
					value = GetDisplayObject(value, column);
				if(!column.RoundDateTimeForColumnFilter)
					return new BinaryOperator(fieldName, value, BinaryOperatorType.Equal);
				else
					return DataProviderBase.CalcColumnFilterCriteriaByValue(column, value);
			}
			string strValue = value.ToString();
			if(string.IsNullOrEmpty(strValue))
				return null;
			if(condition == AutoFilterCondition.Contains)
				return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(fieldName), strValue);
			else if(strValue[0] == '_' || strValue[0] == '%')
				return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(fieldName), strValue.Substring(1));
			else
				return new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(fieldName), strValue);
		}
		protected internal virtual object GetAutoFilterValue(ColumnBase column, CriteriaOperator op) {
			return column.GetAutoFilterValue(op);
		}
		protected internal virtual bool CanStartDrag(GridColumnHeaderBase header) {
			return true;
		}
		protected internal bool IsLastVisibleColumn(BaseColumn column) {
			return column.Fixed == FixedStyle.None ? FixedNoneColumnsCount <= 1 : VisibleColumnsCore.Count <= 1;
		}
		internal bool CanBecameFixed(BaseColumn column) {
			int fixedNoneColumnsCount = FixedNoneColumnsCount;
			if(DataControl.BandsCore.Count != 0)
				fixedNoneColumnsCount = DataControl.BandsLayoutCore.FixedNoneVisibleBands.Count;
			return !column.Visible || column.Fixed != FixedStyle.None || fixedNoneColumnsCount != 1;
		}
		protected virtual int FixedNoneColumnsCount { get { return VisibleColumnsCore.Count; } }
		GridViewNavigationBase navigation;
		GridViewNavigationBase additionalRowNavigation;
		internal GridViewNavigationBase AdditionalRowNavigation { get { return additionalRowNavigation; } }
		protected internal bool AllowMouseMoveSelection { get; set; }
		protected internal GridViewNavigationBase Navigation {
			get {
				if(navigation == null)
					navigation = CreateNavigation();
				return navigation;
			}
			protected set { navigation = value; }
		}
		SelectionStrategyBase selectionStrategy;
		internal SelectionStrategyBase SelectionStrategy {
			get {
				if(selectionStrategy == null)
					selectionStrategy = CreateSelectionStrategy();
				return selectionStrategy;
			}
			set { selectionStrategy = value; }
		}
		internal bool IsFirstRow { get { return DataProviderBase != null && DataProviderBase.CurrentIndex == 0; } }
		internal bool IsLastRow { get { return DataProviderBase != null && DataProviderBase.CurrentIndex == DataControl.VisibleRowCount - 1 && !IsTopNewItemRowFocused; } }
		protected abstract SelectionStrategyBase CreateSelectionStrategy();
		protected internal Locker UpdateVisibleIndexesLocker = new Locker();
		protected Locker applyColumnChooserStateLocker = new Locker();
		internal Locker layoutUpdatedLocker = new Locker();
		internal DataViewBehavior ViewBehavior { get; private set; }
		IColumnCollection emptyColumns;
		protected virtual bool IsLoading { get { return DataControl == null || DataControl.IsLoading; } }
		[Browsable(false)]
		public FrameworkElement ScrollContentPresenter { get; protected internal set; }
		protected internal virtual bool UseMouseUpFocusedEditorShowModeStrategy { get { return false; } }
		internal bool HasCellEditorError { get { return HasValidationError && ValidationError is RowValidationError && ((RowValidationError)ValidationError).IsCellError; } }
		IList<ColumnBase> visibleColumnsCore;
		internal IList<ColumnBase> VisibleColumnsCore {
			get { return visibleColumnsCore; }
			set {
				visibleColumnsCore = value;
				SetVisibleColumns(visibleColumnsCore);
				if(DataControl != null) {
					if(DataControl.CurrentColumn != null && !visibleColumnsCore.Contains(DataControl.CurrentColumn)) {
						DataControl.CurrentColumn = null;
					}
					DataControl.InitializeCurrentColumn();
				}
			}
		}
		internal IEnumerable<ColumnBase> PrintableColumns { get { return VisibleColumnsCore.Where(column => column.AllowPrinting); } }
		protected internal bool IsDesignTime { get { return DesignerProperties.GetIsInDesignMode(this); } }
		internal bool IsInvalidFocusedRowHandle { get { return FocusedRowHandle == DataControlBase.InvalidRowHandle; } }
		protected internal virtual DataProviderBase DataProviderBase { get { return DataControl != null ? DataControl.DataProviderBase : null; } }
		protected internal virtual Orientation OrientationCore { get { return Orientation.Vertical; } }
		internal IScrollInfoOwner ScrollInfoOwner { get; set; }
		internal DataPresenterBase DataPresenter { get { return ScrollInfoOwner as DataPresenterBase; } }
		internal DataPresenterBase RootDataPresenter { get { return RootView.DataPresenter; } }
		internal virtual bool AllowFixedGroupsCore { get { return false; } }
		internal virtual bool AllowPartialGroupingCore { get { return false; } }
		internal bool LockEditorClose { get; set; }
		protected readonly VisualDataTreeBuilder visualDataTreeBuilder;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseHeadersData")]
#endif
		public HeadersData HeadersData { get { return visualDataTreeBuilder.HeadersData; } }
		internal VisualDataTreeBuilder VisualDataTreeBuilder { get { return visualDataTreeBuilder; } }
		protected internal DetailNodeContainer RootNodeContainer { get { return visualDataTreeBuilder.RootNodeContainer; } }
		internal MasterNodeContainer MasterRootNodeContainer { get { return visualDataTreeBuilder.MasterRootNodeContainer; } }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseRootRowsContainer")]
#endif
		public DetailRowsContainer RootRowsContainer { get { return visualDataTreeBuilder.RootRowsContainer; } }
		public MasterRowsContainer MasterRootRowsContainer { get { return visualDataTreeBuilder.MasterRootRowsContainer; } }
		protected internal Dictionary<int, DataRowNode> Nodes { get { return visualDataTreeBuilder.Nodes; } }
		public bool IsFocusedView { get { return MasterRootRowsContainer.FocusedView == this; } }
		internal IColumnCollection ColumnsCore { get { return DataControl != null ? DataControl.ColumnsCore : emptyColumns; } }
		internal InplaceEditorOwnerBase InplaceEditorOwner { get; set; }
		protected internal bool EditorSetInactiveAfterClick { get; set; }
		SelectionAnchorCell selectionAnchorCore = null;
		internal SelectionAnchorCell SelectionAnchor {
			get { return RootView.selectionAnchorCore; }
			set { RootView.selectionAnchorCore = value; }
		}
		SelectionAnchorCell selectionOldCellCore = null;
		internal SelectionAnchorCell SelectionOldCell {
			get { return RootView.selectionOldCellCore; }
			set { RootView.selectionOldCellCore = value; }
		}
		internal FrameworkElement RootBandsContainer { get; set; }
		internal FrameworkElement HeadersPanel { get; set; }
		ImmediateActionsManager immediateActionsManager;
		internal ImmediateActionsManager ImmediateActionsManager { get { return RootView.immediateActionsManager; } }
		bool isColumnFilterOpened = false;
		protected internal bool IsColumnFilterOpened {
			get { return isColumnFilterOpened; }
			set {
				if(isColumnFilterOpened == value)
					return;
				isColumnFilterOpened = value;
				OnIsColumnFilterOpenedChanged();
			}
		}
		protected internal bool IsColumnFilterLoaded { get; set; }
		internal virtual bool IsRowMarginControlVisible { get { return false; } }
		protected internal Locker FocusedRowHandleChangedLocker = new Locker();
		protected internal Locker KeyboardLocker { get { return InplaceEditorOwner.KeyboardLocker; } }
		readonly DataIteratorBase dataIterator;
		protected internal DataIteratorBase DataIterator { get { return dataIterator; } }
		bool rowsStateDirty = true;
		internal bool RowsStateDirty { get { return rowsStateDirty; } set { rowsStateDirty = value; } }
		protected internal Guid CacheVersion { get; protected set; }
		IColumnChooser actualColumnChooser;
		internal IColumnChooser ActualColumnChooser {
			get {
				if(actualColumnChooser == null)
					ActualColumnChooser = CreateColumnChooser();
				return actualColumnChooser;
			}
			set {
				if(actualColumnChooser == value)
					return;
				if(actualColumnChooser != null) actualColumnChooser.Destroy();
				actualColumnChooser = value;
			}
		}
		protected internal bool IsActualColumnChooserCreated { get { return actualColumnChooser != null; } }
		DataControlBase dataControl;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseDataControl")]
#endif
		public DataControlBase DataControl {
			get { return dataControl; }
			internal set { SetDataControl(value); }
		}
		internal SortInfoCollectionBase SortInfoCore { get { return DataControl.SortInfoCore; } }
		internal Locker SupressCacheCleanCountLocker { get; private set; }
		protected internal FrameworkElement FocusedRowElement {
			get {
				if(ViewBehavior.IsAdditionalRow(FocusedRowHandle)) {
					return ViewBehavior.GetAdditionalRowElement(FocusedRowHandle);
				}
				return GetRowElementByRowHandle(FocusedRowHandle);
			}
		}
#if !SL //TODO SL
		protected override bool HandlesScrolling { get { return true; } }
#endif
		protected internal InplaceEditorBase CurrentCellEditor { get { return (CellEditorBase)InplaceEditorOwner.CurrentCellEditor; } set { InplaceEditorOwner.CurrentCellEditor = value; } }
		internal int NavigationIndex {
			get { return (DataControl == null || DataControl.CurrentColumn == null) ? Constants.InvalidNavigationIndex : DataControl.CurrentColumn.VisibleIndex; }
			set {
				int navigationIndex = (value > -1 && value < VisibleColumnsCore.Count) ? value : 0;
				if(NavigationIndex != value || DataControl.CurrentColumn != VisibleColumnsCore[navigationIndex]) {
					DataControl.CurrentColumn = VisibleColumnsCore[navigationIndex];
				}
			}
		}
		internal virtual bool IsDesignTimeAdornerPanelLeftAligned { get { return false; } }
		protected internal IDesignTimeAdornerBase DesignTimeAdorner { get { return DataControl != null ? DataControl.DesignTimeAdorner : EmptyDesignTimeAdornerBase.Instance; ; } }
		#region menu
		Lazy<BarManagerMenuController> columnMenuControllerValue;
		internal BarManagerMenuController ColumnMenuController { get { return columnMenuControllerValue.Value; } }
		Lazy<BarManagerMenuController> totalSummaryMenuControllerValue;
		internal BarManagerMenuController TotalSummaryMenuController { get { return totalSummaryMenuControllerValue.Value; } }
		Lazy<BarManagerMenuController> rowCellMenuControllerValue;
		internal BarManagerMenuController RowCellMenuController { get { return rowCellMenuControllerValue.Value; } }
		[ Browsable(false)]
		public BarManagerActionCollection ColumnMenuCustomizations { get { return ColumnMenuController.ActionContainer.Actions; } }
		[ Browsable(false)]
		public BarManagerActionCollection TotalSummaryMenuCustomizations { get { return TotalSummaryMenuController.ActionContainer.Actions; } }
		[ Browsable(false)]
		public BarManagerActionCollection RowCellMenuCustomizations { get { return RowCellMenuController.ActionContainer.Actions; } }
		#region RowCellMenuItemsSource
		#endregion
		bool initDataControlMenuWhenCreated = false;
#if DEBUGTEST
		internal
#endif
 DataControlPopupMenu dataControlMenu;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataViewBaseDataControlMenu")]
#endif
		public DataControlPopupMenu DataControlMenu {
			get {
				if(dataControlMenu == null) {
					dataControlMenu = CreatePopupMenu();
					if(initDataControlMenuWhenCreated)
						dataControlMenu.Init();
				}
				return dataControlMenu;
			}
		}
		#endregion
		readonly NamePropertyChangeListener namePropertyChangeListener;
		internal int PageVisibleDataRowCount { get { return RootNodeContainer.CurrentLevelItemCount; } }
		internal int PageVisibleTopRowIndex { get { return RootNodeContainer.StartScrollIndex; } }
		internal DataViewBase(MasterNodeContainer masterRootNode, MasterRowsContainer masterRootDataItem, DataControlDetailDescriptor detailDescriptor) {
			namePropertyChangeListener = NamePropertyChangeListener.CreateDesignTimeOnly(this, () => DesignTimeAdorner.UpdateDesignTimeInfo());
			if(!DesignerProperties.GetIsInDesignMode(this))
				BarNameScope.SetIsScopeOwner(this, true);
			if(detailDescriptor != null)
				OriginationView = detailDescriptor.DataControl.DataView;
			visualDataTreeBuilder = new VisualDataTreeBuilder(this, masterRootNode, masterRootDataItem,
				detailDescriptor != null ? detailDescriptor.SynchronizationQueues : new SynchronizationQueues());
			ViewBehavior = CreateViewBehavior();
			emptyColumns = CreateEmptyColumnCollection();
			immediateActionsManager = new ImmediateActionsManager(this);
			VisibleColumnsCore = new ObservableCollection<ColumnBase>();
			ColumnChooserColumns = new ReadOnlyCollection<ColumnBase>(new ColumnBase[0]);
			additionalRowNavigation = ViewBehavior.CreateAdditionalRowNavigation();
			dataIterator = CreateDataIterator();
			Commands = CreateCommandsContainer();
			SupressCacheCleanCountLocker = new Locker();
			RecreateLocalizationDescriptor();
			InplaceEditorOwner = new GridViewInplaceEditorOwner(this);
			EditorSetInactiveAfterClick = false;
			AllowMouseMoveSelection = true;
			columnMenuControllerValue = CreateMenuControllerLazyValue();
			totalSummaryMenuControllerValue = CreateMenuControllerLazyValue();
			rowCellMenuControllerValue = CreateMenuControllerLazyValue();
			DataControlBase.SetActiveView(this, this);
			RequestBringIntoView += DataViewBase_RequestBringIntoView;
		}
		void DataViewBase_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			IInputElement focusedElement = Keyboard.FocusedElement;
			if(focusedElement == this || focusedElement == DataPresenter)
				e.Handled = true;
		}
		protected virtual void SetDataControl(DataControlBase newValue) {
			if(dataControl != newValue) {
				DataControlBase oldValue = dataControl;
				dataControl = newValue;
				OnDataControlChanged(oldValue);
			}
		}
		protected virtual void OnDataControlChanged(DataControlBase oldValue) {
			if(oldValue != null)
				oldValue.IsVisibleChanged -= OnDataControlIsVisibleChanged;
			if(dataControl != null) {
				dataControl.IsVisibleChanged += OnDataControlIsVisibleChanged;
				if(dataControl.BandsLayoutCore != null && DataControl.ColumnsCore.Count == 0 && DesignerProperties.GetIsInDesignMode(dataControl)) {
					dataControl.BandsLayoutCore.FillColumns();
				}
				ValidateSelectionStrategy();
				if(dataControl.isSync && dataControl.CurrentItem != null)
					UpdateFocusedRowHandleCore();
				OnDataReset();
				SetFocusedRowHandle(dataControl.DataProviderBase.CurrentControllerRow);
				SelectionStrategy.OnAssignedToGrid();
				UpdateBorderForFocusedUIElement();
				UpdateMasterDetailViewProperties();
				UpdateSummariesIgnoreNullValues();
				UpdateActualAllowCellMergeCore();
			}
			else {
				HideColumnChooser();
			}
			UpdateIsKeyboardFocusWithinView();
		}
		protected abstract DataViewCommandsBase CreateCommandsContainer();
		protected abstract void SetVisibleColumns(IList<ColumnBase> columns);
		protected abstract DataViewBehavior CreateViewBehavior();
		internal abstract RowValidationError CreateCellValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, ColumnBase column);
		internal abstract IColumnCollection CreateEmptyColumnCollection();
		internal abstract void RaiseCellValueChanging(int rowHandle, ColumnBase column, object value, object oldValue);
		internal abstract bool RaiseShowingEditor(int rowHanlde, ColumnBase columnBase);
		internal abstract void RaiseHiddenEditor(int rowHandle, ColumnBase column, IBaseEdit editCore);
		internal abstract void RaiseShownEditor(int rowHandle, ColumnBase column, IBaseEdit editCore);
		internal abstract IDataViewHitInfo CalcHitInfoCore(DependencyObject source);
		internal virtual bool CanShowEditor(int rowHandle, ColumnBase column) {
			if(rowHandle != DataControlBase.AutoFilterRowHandle)
				if(!column.GetAllowEditing() || DataProviderBase.IsAsyncOperationInProgress || EditFormManager.AllowEditForm)
					return false;
			return !IsKeyboardFocusInSearchPanel() && RaiseShowingEditor(rowHandle, column);
		}
		internal void ResetMenu() {
			DataControlMenu.Reset();
		}
		internal void InitMenu() {
			if(dataControlMenu != null)
				dataControlMenu.Init();
			else
				initDataControlMenuWhenCreated = true;
		}
		internal abstract DataControlPopupMenu CreatePopupMenu();
		protected Lazy<BarManagerMenuController> CreateMenuControllerLazyValue() {
			return new Lazy<BarManagerMenuController>(() => GridMenuInfoBase.CreateMenuController(DataControlMenu));
		}
		protected internal virtual void RaiseShowGridMenu(GridMenuEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		protected internal virtual void RaiseColumnHeaderClick(ColumnHeaderClickEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		internal string GetLocalizedString(string stringId) {
			return LocalizationDescriptor.GetValue(stringId);
		}
		internal string GetLocalizedString(GridControlStringId id) {
			string stringId = Enum.GetName(typeof(GridControlStringId), id);
			return GetLocalizedString(stringId);
		}
		protected internal virtual void UpdateGroupSummary() { }
		internal ColumnBase GetColumnBySortLevel(int level) {
			GridSortInfo sortInfo = GetSortInfoBySortLevel(level);
			return ColumnsCore[sortInfo.FieldName];
		}
		internal GridSortInfo GetSortInfoBySortLevel(int level) {
			return DataControl.ActualSortInfo[level];
		}
		internal IEnumerable<UIElement> GetTopLevelDropContainers() {
			if(!IsRootView)
				throw new InvalidOperationException();
			IColumnChooser visibleColumnChooser = null;
			UpdateAllOriginationViews(view => {
				if(view.IsColumnChooserVisible)
					visibleColumnChooser = view.ActualColumnChooser;
			});
			if(visibleColumnChooser != null)
				yield return visibleColumnChooser.TopContainer;
#if !SL
			yield return LayoutHelper.GetTopContainerWithAdornerLayer(this);
#else
			yield return LayoutHelper.GetTopLevelVisual(this) as UIElement;
#endif
		}
		void OnDataControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(!(bool)e.NewValue) {
				IsColumnChooserVisible = false;
			}
		}
		protected internal virtual void OnDataReset() {
			CacheVersion = Guid.NewGuid();
			Nodes.Clear();
			VisualDataTreeBuilder.GroupSummaryNodes.Clear();
			RootNodeContainer.OnMasterRooDataChanged();
			UpdateDataObjects();
		}
		void ValidationErrorPropertyChanged(BaseValidationError error) {
			HasValidationError = error != null;
		}
		internal void RaiseCommandsCanExecuteChanged() {
			Commands.RaiseCanExecutedChanged();
		}
		protected internal virtual bool CanAdjustScrollbar() {
			return ViewBehavior.CanAdjustScrollbarCore();
		}
		int lockUpdateColumnsLayout = 0;
		protected internal bool IsLockUpdateColumnsLayout { get { return lockUpdateColumnsLayout > 0; } }
		internal virtual void BeginUpdateColumnsLayout() { lockUpdateColumnsLayout++; }
		internal void EndUpdateColumnsLayout() {
			EndUpdateColumnsLayout(true);
		}
		internal virtual void EndUpdateColumnsLayout(bool calcLayout) {
			if(lockUpdateColumnsLayout > 0)
				lockUpdateColumnsLayout--;
			if(lockUpdateColumnsLayout == 0 && calcLayout)
				((IColumnOwnerBase)this).CalcColumnsLayout();
		}
		internal abstract GridRowValidationEventArgs CreateCellValidationEventArgs(object source, object value, int rowHandle, ColumnBase column);
		protected static void OnNavigationStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataViewBase view = (DataViewBase)d;
			if(view == null)
				return;
			if(view.DataControl == null)
				view.OnNavigationStyleChanged();
			else {
				Action<DataControlBase> updateNavigation = dataControl => {
					if(dataControl.DataView != null)
						dataControl.DataView.OnNavigationStyleChanged();
				};
				view.DataControl.UpdateAllDetailDataControls(updateNavigation, updateNavigation);
			}
		}
		void OnShowTotalSummaryChanged() {
			InvalidateParentTree();
			if(DataControl == null || DataControl.AutomationPeer == null) return;
			DataControl.AutomationPeer.ResetChildrenCachePlatformIndependent();
		}
		internal virtual bool CanStartDragSingleColumn() {
			return false;
		}
		internal void ApplyResize(BaseColumn column, double value, double maxWidth) {
			ViewBehavior.ApplyResize(column, value, maxWidth);
		}
		protected internal virtual IDragElement CreateDragElement(BaseGridHeader columnHeader, Point offset) {
			return new ColumnHeaderDragElement(columnHeader, offset);
		}
		protected internal virtual IDropTarget CreateEmptyDropTarget() {
			return new RemoveColumnDropTarget();
		}
		protected internal virtual IDropTarget CreateEmptyBandDropTarget() {
			return new RemoveBandDropTarget();
		}
		protected internal virtual Point CorrectDragIndicatorLocation(UIElement panel, Point point) {
			return ViewBehavior.CorrectDragIndicatorLocation(panel, point);
		}
		protected internal virtual double CalcVerticalDragIndicatorSize(UIElement panel, Point point, double width) {
			return ViewBehavior.CalcVerticalDragIndicatorSize(panel, point, width);
		}
		protected internal string GetDefaultFilterItemLocalizedString(DefaultFilterItem item) {
			return GetLocalizedString(item.ToString());
		}
		protected internal virtual void BeforeMoveColumnToChooser(BaseColumn column, HeaderPresenterType sourceType) { }
#if DEBUGTEST
		internal
#endif
		List<object> logicalChildren = new List<object>();
		internal void AddChild(object child) {
#if !SL //TODO SL
			AddLogicalChild(child);
#endif
			logicalChildren.Add(child);
		}
		internal void RemoveChild(object child) {
#if !SL //TODO SL
			RemoveLogicalChild(child);
#endif
			logicalChildren.Remove(child);
		}
#if !SL
		protected override IEnumerator LogicalChildren { get { return logicalChildren.GetEnumerator(); } }
#endif
		void ILogicalOwner.AddChild(object child) {
			AddChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveChild(child);
		}
#if SL //TODO SL
		bool ILogicalOwner.IsLoaded { get { return true; } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add { } remove { } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
#endif
		internal int GetRowParentIndex(int visibleIndex, int level) {
			int finalRowHandle = DataProviderBase.GetControllerRow(visibleIndex);
			while(DataProviderBase.GetRowLevelByControllerRow(finalRowHandle) > level) {
				finalRowHandle = DataProviderBase.GetParentRowHandle(finalRowHandle);
			}
			return DataProviderBase.GetRowVisibleIndexByHandle(finalRowHandle);
		}
		internal virtual bool IsNewItemRowHandle(int rowHandle) { return false; }
		internal bool IsNewItemRowVisible { get { return ViewBehavior.IsNewItemRowVisible; } }
		internal FrameworkElement GetCellElementByRowHandleAndColumnCore(int rowHandle, ColumnBase column) {
			FrameworkElement rowElement = GetRowElementByRowHandle(rowHandle);
			if(rowElement == null)
				return null;
			return LayoutHelper.FindElement(rowElement, element => element is IGridCellEditorOwner && ((IGridCellEditorOwner)element).AssociatedColumn == column && element.Visibility == Visibility.Visible);
		}
		protected internal virtual DispatcherTimer ScrollTimer { get { return ViewBehavior.ScrollTimer; } }
		protected internal virtual bool IsAdditionalRowFocused { get { return ViewBehavior.IsAdditionalRow(FocusedRowHandle); } }
		protected internal virtual bool IsAutoFilterRowFocused { get { return ViewBehavior.IsAutoFilterRowFocused; } }
		protected internal bool IsContextMenuOpened { get { return dataControlMenu != null && dataControlMenu.IsOpen; } }
		protected internal virtual void OnAllowPerPixelScrollingChanged() {
			if(DataControl != null)
				DataControl.InvalidateDetailScrollInfoCache();
			if(DataPresenter != null && UIElementHelper.IsVisibleInTree(DataPresenter, true) && DataControl != null) {
				DataPresenter.ClearScrollInfoAndUpdate();
			}
		}
		internal virtual KeyValuePair<DataViewBase, int> GetViewAndVisibleIndex(double verticalOffset) { return ViewBehavior.GetViewAndVisibleIndex(verticalOffset); }
		internal bool CollapseFocusedRowCore() {
			if(IsInvalidFocusedRowHandle || !IsExpanded(FocusedRowHandle))
				return false;
			return ChangeVisibleRowExpand(FocusedRowHandle);
		}
		internal bool ExpandFocusedRowCore() {
			if(IsInvalidFocusedRowHandle || IsExpanded(FocusedRowHandle))
				return false;
			return ChangeVisibleRowExpand(FocusedRowHandle);
		}
		protected bool ChangeVisibleRowExpand(int rowHandle) {
			if(rowHandle == DataControlBase.InvalidRowHandle) return false;
			return ChangeVisibleRowExpandCore(rowHandle);
		}
		protected abstract bool ChangeVisibleRowExpandCore(int rowHandle);
		protected internal void UpdateCellData(ColumnsRowDataBase rowData) {
			ViewBehavior.UpdateCellData(rowData);
		}
		internal virtual Type GetColumnType(ColumnBase column, DataProviderBase dataProvider = null) {
			return GetColumnType(column.FieldName, dataProvider);
		}
		internal virtual Type GetColumnType(string fieldName, DataProviderBase dataProvider = null) {
			if(dataProvider == null) dataProvider = DataProviderBase;
			if(dataProvider == null) return null;
			DataColumnInfo columnInfo = dataProvider.Columns[fieldName];
			if(columnInfo == null)
				return null;
			return columnInfo.Type;
		}
		protected internal string GetColumnDisplayText(object value, ColumnBase column, int? rowHandle = null) {
			object displayObject = GetDisplayObject(value, column);
			string displayText = displayObject != null ? displayObject.ToString() : null;
			return RaiseCustomDisplayText(rowHandle ?? DataControlBase.InvalidRowHandle, null, column, value, displayText);
		}
		internal abstract string RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string displayText);
		internal abstract bool? RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string originalDisplayText, out string displayText);
		internal object GetDisplayObject(object value, ColumnBase column) {
			return GetDisplayObject(value, column, true);
		}
		internal object GetDisplayObject(object value, ColumnBase column, bool applyFormat) {
			if(applyFormat)
				return column.ActualEditSettingsCore.GetDisplayTextFromEditor(value);
			return column.ActualEditSettingsCore.GetDisplayText(value, applyFormat);
		}
		internal bool UpdateActionEnqueued { get; set; }
		protected internal void EnqueueImmediateAction(IAction action) {
			ImmediateActionsManager.EnqueueAction(action);
		}
		protected internal void EnqueueImmediateAction(Action action) {
			ImmediateActionsManager.EnqueueAction(action);
		}
		protected internal virtual void RaiseFilterPopupEvent(ColumnBase column, PopupBaseEdit popupBaseEdit) {
			RaiseEventInOriginationView(new FilterPopupEventArgs(column, popupBaseEdit) { RoutedEvent = ShowFilterPopupEvent });
		}
		public bool IsRowSelected(int rowHandle) {
			return SelectionStrategy.IsRowSelected(rowHandle);
		}
		protected internal SelectionState GetRowSelectionState(int rowHandle) {
			return SelectionStrategy.GetRowSelectionState(rowHandle);
		}
		protected internal SelectionState GetCellSelectionState(int rowHandle, bool isFocused, bool isSelected) {
			return SelectionStrategy.GetCellSelectionState(rowHandle, isFocused, isSelected);
		}
		protected internal virtual void StopSelection() {
			ViewBehavior.StopSelection();
		}
		protected internal virtual bool NeedCellsWidthUpdateOnScrolling { get { return false; } }
		internal bool OnBeforeChangePixelScrollOffset() {
			return ViewBehavior.NavigationStrategyBase.OnBeforeChangePixelScrollOffset(this);
		}
		internal WeakEventHandler<EventArgs, EventHandler> IsKeyboardFocusWithinViewChanged = new WeakEventHandler<EventArgs, EventHandler>();
		void OnIsKeyboardFocusWithinViewChanged() {
			IsKeyboardFocusWithinViewChanged.SafeRaise(this, new EventArgs());
			UpdateRowDataFocusWithinState();
		}
		void UpdateRowDataFocusWithinState() {
			if(DataControl != null)
				DataControl.UpdateAllDetailDataControls(dataControl => dataControl.DataView.ViewBehavior.UpdateViewRowData(x => x.UpdateClientFocusWithinState()));
		}
		protected void OnIsSynchronizedWithCurrentItemChanged(bool oldValue, bool newValue) {
		}
		protected void OnFocusedRowChanged(object oldValue, object newValue) {
			if(DataControl != null && !ReferenceEquals(DataControl.CurrentItem, GetValue(FocusedRowProperty)))
				DataControl.SetCurrentItemCore(GetValue(FocusedRowProperty));
			RaiseFocusedRowChanged(oldValue, newValue);
		}
		internal void UpdateFocusedRowHandleCore() {
			if(object.Equals(DataProviderBase.GetRowValue(FocusedRowHandle), DataControl.CurrentItem) && !DataProviderBase.IsGroupRowHandle(FocusedRowHandle))
				return;
			int rowHandle = DataProviderBase.FindRowByRowValue(DataControl.CurrentItem);
			if(rowHandle == DataControlBase.InvalidRowHandle && DataControl.HasCurrentItemBinding && !DataControl.AllowUpdateTwoWayBoundPropertiesOnSynchronization)
				return;
			SetFocusedRowHandle(rowHandle);
		}
		internal void SetFocusOnCurrentItem() {
			if(DataControl.CurrentItem != null)
				UpdateFocusedRowHandleCore();
			else
				SetFocusedRowHandle(DataControlBase.InvalidRowHandle);
		}
		internal void OnDataSourceReset() {
			SetFocusOnCurrentControllerRow();
			SelectionStrategy.OnDataSourceReset();
		}
		protected internal void SetFocusOnCurrentControllerRow() {
			if(DataControl != null && !SelectionStrategy.IsFocusedRowHandleLocked)
				SetFocusedRowHandle(DataProviderBase.CurrentControllerRow);
		}
		internal void BeginSelectionCore() {
			SelectionStrategy.BeginSelection();
		}
		internal void SelectRowCore(int rowHandle) {
			SelectionStrategy.SelectRow(rowHandle);
		}
		internal void UnselectRowCore(int rowHandle) {
			SelectionStrategy.UnselectRow(rowHandle);
		}
		internal void EndSelectionCore() { 
			SelectionStrategy.EndSelection();
		}
		internal void ClearSelectionCore() {
			SelectionStrategy.ClearSelection();
		}
		internal void SelectRangeCore(int startRowHandle, int endRowHandle) {
			SelectionStrategy.SelectRange(startRowHandle, endRowHandle);
		}
		protected internal virtual void OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			SelectionStrategy.OnSelectionChanged(e);
			AllItemsSelected = SelectionStrategy.GetAllItemsSelected();
		}
		protected internal void RaiseSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			RaiseSelectionChanged(CreateSelectionChangedEventArgs(e));
		}
		protected internal abstract GridSelectionChangedEventArgs CreateSelectionChangedEventArgs(DevExpress.Data.SelectionChangedEventArgs e);
		protected internal virtual void RaiseSelectionChanged(GridSelectionChangedEventArgs e) { }
		void OnColumnChooserStateChanged() {
			if(!applyColumnChooserStateLocker.IsLocked)
				ActualColumnChooser.ApplyState(ColumnChooserState);
		}
		IColumnChooser CreateColumnChooser() {
			IColumnChooser result = ColumnChooserFactory.Create(this);
			NullColumnChooserException.CheckColumnChooserNotNull(result);
			result.ApplyState(RootView.ColumnChooserState);
			return result;
		}
		void OnColumnChooserFactoryChanged() {
			IsColumnChooserVisible = false;
			ActualColumnChooser = CreateColumnChooser();
		}
		private void OnIsColumnFilterOpenedChanged() {
			DataPresenter.Do(presenter => presenter.SetManipulation(IsColumnFilterOpened));
		}
		IColumnChooserFactory CoerceColumnChooserFactory(IColumnChooserFactory baseValue) {
			return baseValue ?? DefaultColumnChooserFactory.Instance;
		}
#if DEBUGTEST
		internal int UpdateColumnVisibleIndexCount = 0;
#endif
		internal void ApplyColumnVisibleIndex(BaseColumn column) {
			if(UpdateVisibleIndexesLocker.IsLocked || IsLoading || DataControl.IsDeserializing || ColumnsCore.IsLockUpdate || !column.Visible)
				return;
#if DEBUGTEST
			UpdateColumnVisibleIndexCount++;
#endif
			UpdateVisibleIndexesLocker.Lock();
			try {
				if(DataControl != null && DataControl.BandsLayoutCore != null) {
					DataControl.BandsLayoutCore.ApplyColumnVisibleIndex(column);
				} else {
					int lastValidIndex = VisibleColumnsCore.Contains(column) ? VisibleColumnsCore.Count - 1 : VisibleColumnsCore.Count;
					column.VisibleIndex = Math.Max(Math.Min(column.VisibleIndex, lastValidIndex), 0);
					MoveColumnToCore((ColumnBase)column, column.VisibleIndex, HeaderPresenterType.Headers, HeaderPresenterType.Headers);
				}
			} finally {
				UpdateVisibleIndexesLocker.Unlock();
			}
		}
		protected internal virtual void MoveColumnToCore(ColumnBase source, int newVisibleIndex, HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			if(moveFrom == HeaderPresenterType.ColumnChooser)
				source.Visible = true;
			HandleGroupMoveAction(source, newVisibleIndex, moveFrom, moveTo);
			if(moveTo == HeaderPresenterType.Headers) {
				ObservableCollection<ColumnBase> visibleColumnsList = new ObservableCollection<ColumnBase>(VisibleColumnsCore);
				visibleColumnsList.Remove(source);
				newVisibleIndex = AdjustVisibleIndex(source, newVisibleIndex);
				visibleColumnsList.Insert(newVisibleIndex, source);
				AssignVisibleColumns(visibleColumnsList);
			}
		}
		internal virtual int AdjustVisibleIndex(ColumnBase column, int visibleIndex) {
			bool alreadyInVisibleColumns = VisibleColumnsCore.Contains(column);
			if(alreadyInVisibleColumns && ColumnBase.GetVisibleIndex(column) < visibleIndex && ColumnBase.GetVisibleIndex(column) > -1)
				return visibleIndex - 1;
			return visibleIndex;
		}
		protected virtual void HandleGroupMoveAction(ColumnBase source, int newVisibleIndex, HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
		}
		void AllowFilterEditorChanged() {
			CommandManager.InvalidateRequerySuggested();
			UpdateShowEditFilterButtonCore();
		}
		internal void UpdateShowEditFilterButtonCore() {
			ShowEditFilterButton = GetShowEditFilterButton();
		}
		protected virtual bool GetShowEditFilterButton() {
			bool allowColumnsFiltering = true;
			if(DataControl != null) {
				allowColumnsFiltering = AllowColumnFiltering ? ((DataControl.countColumnFilteringTrue != 0) || (DataControl.countColumnFilteringDefault != 0)) : (DataControl.countColumnFilteringTrue != 0);
			}
			return allowColumnsFiltering && AllowFilterEditor;
		}
		internal void OnColumnHeaderClick(ColumnBase column) {
#if SL
			DependencyObject topVisual = LayoutHelper.GetTopLevelVisual(this);
			if(topVisual != null && (bool)topVisual.GetValue(DragManager.IsDraggingProperty))
				return;
#endif
			if(column != null && DataControl.DataControlOwner.CanSortColumn(column) && CommitEditing()) {
				OnColumnHeaderClick(column, (Keyboard.Modifiers & ModifierKeys.Shift) != 0, (Keyboard.Modifiers & ModifierKeys.Control) != 0);
				DesignTimeAdorner.OnColumnHeaderClick();
			}
		}
		internal virtual void OnSummaryDataChanged() {
			UpdateColumnsTotalSummary();
		}
		protected virtual internal void OnDataChanged(bool rebuildVisibleColumns) {
			if(rebuildVisibleColumns && !IsLockUpdateColumnsLayout) {
				RebuildVisibleColumns();
				UpdateColumnsViewInfo();
			}
			UpdateFilterPanel();
			if(SearchPanelColumnProvider != null)
				SearchPanelColumnProvider.UpdateColumns();
		}
		internal bool LockDataColumnsChanged;
		public void MoveColumnTo(ColumnBase source, int newVisibleIndex, HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			if(source == null)
				return;
			ColumnsCore.BeginUpdate();
			try {
				MoveColumnToCore(source, newVisibleIndex, moveFrom, moveTo);
			}
			finally {
				if(moveFrom != HeaderPresenterType.GroupPanel && moveTo != HeaderPresenterType.GroupPanel)
					LockDataColumnsChanged = true;
				DataControl.syncronizationLocker.DoLockedAction(ColumnsCore.EndUpdate);
				LockDataColumnsChanged = false;
			}
			NotifyDesignTimeAdornerOnColumnMoved(moveFrom, moveTo);
		}
		protected virtual void NotifyDesignTimeAdornerOnColumnMoved(HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			DesignTimeAdorner.OnColumnMoved();
		}
		internal virtual bool IsColumnVisibleInHeaders(BaseColumn col) {
			return true;
		}
		protected internal abstract bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e);
		#region methods implemented in GridViewBase only
		internal abstract DataController GetDataControllerForUnboundColumnsCore();
		internal void OnValidation(ColumnBase column, GridRowValidationEventArgs e) {
			column.OnValidation(e);
			e.Source = this;
			EventTargetView.RaiseValidateCell(e);
		}
		protected internal abstract void RaiseValidateCell(GridRowValidationEventArgs e);
		internal abstract FrameworkElement CreateRowElement(RowData rowData);
		internal abstract bool IsExpandableRowFocused();
		protected internal abstract object GetGroupDisplayValue(int rowHandle);
		protected internal abstract string GetGroupRowDisplayText(int rowHandle);
		protected abstract DataIteratorBase CreateDataIterator();
		internal abstract BaseValidationError CreateCellValidationError(object errorContent, ErrorType errorType, int rowHandle, ColumnBase column);
		internal abstract BaseValidationError CreateRowValidationError(object errorContent, ErrorType errorType, int rowHandle);
		#endregion
		#region methods implemented in TableViewBase && (||) CardViewBase
		protected internal virtual void OnOpeningEditor() {
		}
		protected internal virtual void OnShowEditor(CellEditorBase editor) {
			ViewBehavior.OnShowEditor(editor);
		}
		protected internal virtual void OnHideEditor(CellEditorBase editor, bool closeEditor) {
			ViewBehavior.OnHideEditor(editor);
		}
		#endregion
		protected internal virtual GridFilterColumn CreateFilterColumn(ColumnBase column, bool useDomainDataSourceRestrictions, bool useWcfSource) {
			return new GridFilterColumn(column, useDomainDataSourceRestrictions, useWcfSource);
		}
		protected internal bool IsColumnNavigatable(ColumnBase column, bool isTabNavigation) {
			bool isTabNavigatable = isTabNavigation ? column.TabStop : true;
			return isTabNavigatable && (column.AllowFocus || FocusedRowHandle == DataControlBase.AutoFilterRowHandle);
		}
		protected internal virtual void UpdateCellMergingPanels() {
		}
		protected internal virtual void UpdateRowData(UpdateRowDataDelegate updateMethod, bool updateInvisibleRows = true, bool updateFocusedRow = true) {
			ViewBehavior.UpdateRowData(updateMethod, updateInvisibleRows, updateFocusedRow);
		}
		protected internal string GetTextForClipboard(int rowHandle, int visibleColumnIndex) {
			string cellText = rowHandle == DataControlBase.InvalidRowHandle ? VisibleColumnsCore[visibleColumnIndex].HeaderCaption.ToString() : GetRowCellDisplayText(rowHandle, visibleColumnIndex);
			if(cellText == null)
				return String.Empty;
			if(cellText.Contains(Environment.NewLine))
				cellText = "\"" + cellText.Replace("\"", "\"\"").Replace(Environment.NewLine, "\n") + "\"";
			cellText = cellText.Replace("\t", " ");
			return cellText;
		}
		protected internal string GetRowCellDisplayText(int rowHandle, int visibleColumnIndex) {
			if(DataControl.IsGroupRowHandleCore(rowHandle)) {
				return GetGroupRowDisplayText(rowHandle);
			}
			else {
				if(VisibleColumnsCore[visibleColumnIndex].CopyValueAsDisplayText) {
					return GetColumnDisplayText(DataControl.GetCellValueCore(rowHandle, VisibleColumnsCore[visibleColumnIndex]), VisibleColumnsCore[visibleColumnIndex], rowHandle);
				} else {
					object value = DataControl.GetCellValueCore(rowHandle, VisibleColumnsCore[visibleColumnIndex]);
					return value != null ? value.ToString() : string.Empty;
				}
			}
		}
		internal int VisibleComparison(BaseColumn x, BaseColumn y) {
			if(ColumnBase.GetVisibleIndex(x) == ColumnBase.GetVisibleIndex(y))
				return Comparer<int>.Default.Compare(x.index, y.index);
			int? coreResult = ViewBehavior.VisibleComparisonCore(x, y);
			if(coreResult.HasValue)
				return coreResult.Value;
			coreResult = CompareGroupedColumns(x, y);
			if(coreResult.HasValue)
				return coreResult.Value;
			if(ColumnBase.GetVisibleIndex(x) < 0)
				return 1;
			if(ColumnBase.GetVisibleIndex(y) < 0)
				return -1;
			return Comparer<int>.Default.Compare(ColumnBase.GetVisibleIndex(x), ColumnBase.GetVisibleIndex(y));
		}
		protected virtual int? CompareGroupedColumns(BaseColumn x, BaseColumn y) {
			return null;
		}
		protected internal virtual void ResetHeadersChildrenCache() { }
#if DEBUGTEST
		internal int RebuildVisibleColumnsCount;
#endif
		internal void RebuildVisibleColumns() {
			if(DataControl == null || DataControl.DataView != this || DataControl.IsLoading || DataControl.IsDeserializing || IsLockUpdateColumnsLayout || ColumnsCore.IsLockUpdate)
				return;
#if DEBUGTEST
			RebuildVisibleColumnsCount++;
#endif
			AssignVisibleColumns(ViewBehavior.RebuildVisibleColumns());
			RebuildColumnChooserColumns();
			DataControl.UpdateAllDetailViewIndents();
		}
		internal virtual void AssignVisibleColumns(ObservableCollection<ColumnBase> visibleColumnsList) {
			UpdateVisibleIndexesLocker.Lock();
			try {
				ViewBehavior.UpdateColumnsPosition(visibleColumnsList);
			}
			finally {
				UpdateVisibleIndexesLocker.Unlock();
			}
			bool changed = false;
			if(!ListHelper.AreEqual(VisibleColumnsCore, visibleColumnsList)) {
				VisibleColumnsCore = visibleColumnsList;
				changed = true;
				DesignTimeAdorner.OnColumnsLayoutChanged();
			}
			changed = OnVisibleColumnsAssigned(changed);
			if(changed) {
				if(DataPresenter != null) {
					ImmediateActionsManager.EnqueueAction(UpdateCellData);
				}
				else {
					UpdateCellData();
				}
			}
			ViewBehavior.UpdateColumnsLayout();
			ViewBehavior.UpdateViewportVisibleColumns();
		}
#if DEBUGTEST
		public static int RebuildColumnChooserColumnsCount;
#endif
		protected internal void RebuildColumnChooserColumns() {
			if(ColumnsCore.IsLockUpdate) return;
#if DEBUGTEST
			RebuildColumnChooserColumnsCount++;
#endif
			ViewBehavior.RebuildColumnChooserColumns();
		}
		protected virtual bool OnVisibleColumnsAssigned(bool changed) {
			return ViewBehavior.OnVisibleColumnsAssigned(changed);
		}
		protected internal virtual void UpdateCellData() {
			ViewBehavior.UpdateCellData();
		}
		internal void UpdateRowDataByRowHandle(int rowHandle, UpdateRowDataDelegate updateMethod) {
			RowData focusedRowData = FocusedRowData;
			if(focusedRowData != null && focusedRowData.RowHandle.Value == rowHandle)
				updateMethod(focusedRowData);
			RowData rowDataByRowHandle = GetRowData(rowHandle);
			if(rowDataByRowHandle != null)
				updateMethod(rowDataByRowHandle);
		}
		object CoerceTopRowIndex(int value) {
			if(value < 0) return 0;
			if(DataControl == null || ScrollInfoOwner == null)
				return value;
			if(value > DataControl.VisibleRowCount)
				switch(ScrollingMode) {
					case ScrollingMode.Smart: return DataControl.VisibleRowCount - ScrollInfoOwner.ItemsOnPage;
					case ScrollingMode.Normal: return DataControl.VisibleRowCount - 1;
				}
			return value;
		}
		void OnTopRowIndexChanged() {
			ViewBehavior.OnTopRowIndexChanged();
		}
		internal virtual void SetFocusedRectangleOnRow() {
			SetFocusedRectangleOnRowData(GetRowFocusedRectangleTemplate());
		}
		internal virtual void SetFocusedRectangleOnCell() {
			SetFocusedRectangleOnElement(FocusedView.CurrentCellEditor != null ? FocusedView.CurrentCell as FrameworkElement : null, GetCellFocusedRectangleTemplate());
		}
		internal virtual void SetFocusedRectangleOnGroupRow() {
			SetFocusedRectangleOnRowData(GetGroupRowFocusedRectangleTemplate());
		}
		protected virtual ControlTemplate GetCellFocusedRectangleTemplate() {
			return FocusedCellBorderTemplate;
		}
		abstract protected ControlTemplate GetRowFocusedRectangleTemplate();
		protected virtual ControlTemplate GetGroupRowFocusedRectangleTemplate() {
			return FocusedGroupRowBorderTemplate;
		}
		internal RowData GetRowData(int rowHandle) {
			return ViewBehavior.GetRowData(rowHandle);
		}
		protected void SetFocusedRectangleOnRowData(ControlTemplate template) {
			RowData row = GetRowData(FocusedRowHandle);
			if(row != null) {
				IFocusedRowBorderObject rowBorderObject = row.RowElement as IFocusedRowBorderObject;
				FrameworkElement element = row.RowElement;
				double leftIndent = 0d;
				if(rowBorderObject != null) {
					element = rowBorderObject.RowDataContent;
					leftIndent = rowBorderObject.LeftIndent;
				}
				SetFocusedRectangleOnElement(element, template, leftIndent);
			}
			else {
				ClearFocusedRectangle();
			}
		}
		internal FocusRectPresenter FocusRectPresenter { get; set; }
		protected void SetFocusedRectangleOnElement(FrameworkElement element, ControlTemplate template, double leftIndent = 0d) {
			if(element != null && UIElementHelper.IsVisibleInTree(element, true)) {
				if(RootView.FocusRectPresenter == null || RootDataPresenter == null)
					return;
				RootView.FocusRectPresenter.Owner = element;
				RootView.FocusRectPresenter.ChildTemplate = template;
				RootView.FocusRectPresenter.UpdateRendering(leftIndent);
				RootView.FocusRectPresenter.Visibility = System.Windows.Visibility.Visible;
			}
			else {
				ClearFocusedRectangle();
			}
		}
		internal void ClearFocusedRectangle() {
			if(RootView.FocusRectPresenter != null)
				RootView.FocusRectPresenter.Visibility = System.Windows.Visibility.Collapsed;
		}
		protected virtual internal bool CanSelectCellInRow(int rowHandle) {
			return GetNavigation(rowHandle).CanSelectCell;
		}
		protected internal virtual GridViewNavigationBase GetNavigation(int rowHandle) {
			if(ViewBehavior.IsAdditionalRow(rowHandle))
				return AdditionalRowNavigation;
			return Navigation;
		}
		protected internal ColumnBase GetColumnByCommandParameter(object commandParameter) {
			if(commandParameter is string)
				return DataControl.ColumnsCore[(string)commandParameter];
			return commandParameter as ColumnBase;
		}
		protected GridViewNavigationBase CreateNavigation() {
			switch(RootView.NavigationStyle) {
				case GridViewNavigationStyle.None:
					return new DummyNavigation(this);
				case GridViewNavigationStyle.Row:
					return ViewBehavior.CreateRowNavigation();
				case GridViewNavigationStyle.Cell:
					return ViewBehavior.CreateCellNavigation();
			}
			return null;
		}
		protected internal virtual bool RaisePastingFromClipboard() {
			PastingFromClipboardEventArgs e = new PastingFromClipboardEventArgs(DataControl, DataViewBase.PastingFromClipboardEvent);
			RaiseEventInOriginationView(e);
			return e.Handled;
		}
		internal virtual bool CanCopyRows() {
			return false;
		}
		internal bool IsMultiSelection { get { return GetActualSelectionMode() != MultiSelectMode.None; } }
		internal bool IsMultiCellSelection { get { return GetActualSelectionMode() == MultiSelectMode.Cell; } }
		internal bool IsMultiRowSelection { get { return GetActualSelectionMode() == MultiSelectMode.Row || GetActualSelectionMode() == MultiSelectMode.MultipleRow; } }
		internal virtual bool GetAllowGroupSummaryCascadeUpdate { get { return false; } }
		bool actualAllowCellMerge;
		protected internal bool ActualAllowCellMerge {
			get { return actualAllowCellMerge; }
			set {
				if(actualAllowCellMerge == value) return;
				actualAllowCellMerge = value;
				OnActualAllowCellMergeChanged();
			}
		}
		protected virtual void OnActualAllowCellMergeChanged() { }
		[Obsolete("Use the DataControlBase.CopyToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopyToClipboard() {
			SelectionStrategy.CopyToClipboard();
		}
		[Obsolete("Use the DataControlBase.SelectAll method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectAll() {
			SelectAllCore();
		}
		internal void SelectAllCore() {
			SelectionStrategy.SelectAll();
		}
		void OnFocusedRowHandleChanged(int oldRowHandle) {
			if(FocusedRowHandleChangedLocker.IsLocked)
				return;
			OnFocusedRowHandleChangedCore(oldRowHandle);
		}
		protected virtual void OnFocusedRowHandleChangedCore(int oldRowHandle) {
			if(FocusedRowHandle != DataControlBase.InvalidRowHandle)
				MasterRootRowsContainer.FocusedView = this;
			UpdateFullRowState(oldRowHandle);
			UpdateIsFocusedCellIfNeeded(oldRowHandle);
			UpdateFullRowState(FocusedRowHandle);
			UpdateIsFocusedCellIfNeeded(FocusedRowHandle);
			UpdateEditorButtonVisibilities(oldRowHandle);
			UpdateEditorButtonVisibilities(FocusedRowHandle);
			SelectionStrategy.OnFocusedRowHandleChanged(oldRowHandle);
			if(IsAdditionalRowFocused) {
				ViewBehavior.UpdateAdditionalFocusedRowData();
				ForceUpdateRowsState();
				DataControl.UpdateCurrentItem();
				SelectionStrategy.OnFocusedRowDataChanged();
				ScrollToFocusedRowIfNeeded();
				return;
			}
			DataProviderBase.MakeRowVisible(FocusedRowHandle);
			if(DataProviderBase.AllowUpdateFocusedRowData)
				UpdateFocusedRowData();
			else
				DataControl.UpdateCurrentItem();
			int prevIndex = DataProviderBase.CurrentIndex;
			DataProviderBase.CurrentControllerRow = FocusedRowHandle;
			ForceUpdateRowsState();
			if(prevIndex == DataProviderBase.CurrentIndex) {
				RowsStateDirty = true;
			}
			UpdateBorderForFocusedUIElement();
			ScrollToFocusedRowIfNeeded();
		}
		void ScrollToFocusedRowIfNeeded() {
			if(RootView.AllowScrollToFocusedRow && IsFocusedView) {
				ScrollIntoView(FocusedRowHandle);
			}
		}
		internal void ProcessFocusedElement() {
			FrameworkElement focusedRowElement = FocusedRowElement;
			if(focusedRowElement == null) return;
			bool isKeyboardFocusWithinFocusedRowElement = LayoutHelper.IsChildElementEx(FocusedRowElement, KeyboardHelper.FocusedElement);
#if !SL
			if(!FloatingContainer.IsModalContainerOpened)
#endif
				if (IsKeyboardFocusWithinView && !isKeyboardFocusWithinFocusedRowElement && !IsColumnFilterOpened && !IsKeyboardFocusInSearchPanel()) {
					if (CurrentCellEditor != null) {
						if((SearchControl == null || !SearchControl.GetIsKeyboardFocusWithin()) && !IsKeyboardFocusInHeadersPanel()) {
							CurrentCellEditor.Edit.SetKeyboardFocus();
						}
					}
					else {
						KeyboardHelper.Focus(focusedRowElement);
					}
				}
			if(GetNavigation(FocusedRowHandle).ShouldRaiseRowAutomationEvents && ((NavigationStyle == GridViewNavigationStyle.Row) || DataProviderBase.IsGroupRowHandle(FocusedRowHandle))) {
				RaiseItemSelectAutomationEvents(focusedRowElement);
			}
		}
		void UpdateFullRowState(int rowHandle) {
			UpdateRowDataByRowHandle(rowHandle, (rowData) => rowData.UpdateFullState());
		}
		void UpdateFullRowState() {
			UpdateRowData(rowData => rowData.UpdateFullState(), false, true);
		}
		void UpdateIsFocused() {
			UpdateRowData(rowData => rowData.UpdateIsFocused(), false, true);
		}
		void UpdateIsSelected() {
			UpdateRowData(rowData => rowData.UpdateIsSelected(), false, true);
		}
		void UpdateEditorButtonVisibilities(int rowHandle) {
			UpdateRowDataByRowHandle(rowHandle, (rowData) => rowData.UpdateEditorButtonVisibilities());
		}
		internal void UpdateEditorButtonVisibilities() {
			UpdateRowData((rowData) => rowData.UpdateEditorButtonVisibilities());
		}
		internal void UpdateEditorHighlightingText() {
			UpdateRowData((rowData) => rowData.UpdateEditorHighlightingText(), false, false);			
		}
		internal void UpdateCellDataLanguage() {
			UpdateRowData((rowData) => rowData.UpdateCellDataLanguage());
		}
		internal void ProcessFocusedViewChange() {
			if(!IsFocusedView)
				FocusedRowHandle = DataControlBase.InvalidRowHandle;
			UpdateIsKeyboardFocusWithinView();
			UpdateFullRowState();
			ForceUpdateRowsState();
			if(IsFocusedView) {
				ScrollIntoView(FocusedRowHandle);
			}
		}
		internal void UpdateIsKeyboardFocusWithinView() {
			if(DataControl == null) {
				IsKeyboardFocusWithinView = false;
			} else {
				IsKeyboardFocusWithinView = IsFocusedView && DataControl.GetRootDataControl().IsKeyboardFocusWithin;
			}
		}
		internal void UpdateFocusedRowData() {
			if(FocusedRowHandleChangedLocker.IsLocked)
				return;
			if((FocusedRowData == null) || ViewBehavior.IsAdditionalRowData(FocusedRowData))
				FocusedRowData = CreateFocusedRowData();
			RowHandle oldRowHandle = FocusedRowData.RowHandle;
			object oldRow = FocusedRowData.Row;
			FocusedRowData.AssignFrom(FocusedRowHandle);
			if(dataControl != null) {
				dataControl.UpdateCurrentItem();
				if(!object.Equals(oldRowHandle, FocusedRowData.RowHandle) || !object.Equals(oldRow, FocusedRowData.Row)) 
					DataControl.UpdateFocusedRowDataposponedAction.PerformIfNotInProgress(RaiseFocusedRowHandleChanged);
				SelectionStrategy.OnFocusedRowDataChanged();
			}
		}
		protected virtual RowData CreateFocusedRowData() {
			return new StandaloneRowData(VisualDataTreeBuilder, true);
		}
		internal void UpdateBorderForFocusedUIElement() {
			if(DataControl == null) return;
			SelectionStrategy.UpdateBorderForFocusedElement();
		}
		protected void OnShowFocusedRectangleChanged() {
			UpdateBorderForFocusedUIElement();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			ViewBehavior.OnViewMouseLeave();
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			ViewBehavior.OnViewMouseMove(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			ViewBehavior.OnMouseLeftButtonUp();
		}
		internal virtual void OnMultiSelectModeChanged() {
			EditorSetInactiveAfterClick = false;
			if(DataControl != null) {
				DataControl.ValidateMasterDetailConsistency();
				ClearSelectionCore(); 
			}
			if(DataControl == null)
				return;
			ResetSelectionStrategy();
		}
		internal void ResetSelectionStrategy() {
			SelectionStrategy = null;
			if(DataControl == null)
				return;
			SelectionStrategy.OnAssignedToGrid();
			UpdateBorderForFocusedUIElement();
			UpdateIsSelected();
		}
		protected internal void GetDataRowText(StringBuilder sb, int rowHandle) {
			ViewBehavior.GetDataRowText(sb, rowHandle);
		}
		void UpdateActualTotalSummaryItemTemplateSelector() {
			UpdateActualTemplateSelector(ActualTotalSummaryItemTemplateSelectorPropertyKey, TotalSummaryItemTemplateSelector, TotalSummaryItemTemplate);
		}
		void UpdateColumnsActualHeaderTemplateSelector() {
			UpdateColumns(column => column.UpdateActualHeaderTemplateSelector());
		}
		void UpdateColumnsActualHeaderCustomizationAreaTemplateSelector() {
			UpdateColumns(column => column.UpdateActualHeaderCustomizationAreaTemplateSelector());
		}
		internal void UpdateActualColumnChooserTemplate() {
			ActualColumnChooserTemplate = ViewBehavior.GetActualColumnChooserTemplate();
		}
		internal void UpdateFilterPanel() {
			if(DataControl == null || DataControl.IsLoading) return;
			ActualShowFilterPanel = GetActualShowFilterPanel();
			FilterPanelText = GetFilterOperatorCustomText(DataControl.FilterCriteria);
			DataControl.UpdateActiveFilterInfo();
		}
		protected internal virtual void UpdateAlternateRowBackground() { }
		internal string GetFilterOperatorCustomText(CriteriaOperator filterCriteria) {
			CriteriaOperator op = DisplayCriteriaGenerator.Process(new DisplayCriteriaHelper(this), filterCriteria);
			CustomFilterDisplayTextEventArgs e = new CustomFilterDisplayTextEventArgs(this, op) { RoutedEvent = DataViewBase.CustomFilterDisplayTextEvent };
			RaiseCustomFilterDisplayText(e);
			return GetFilterPanelText(e.Value);
		}
		protected virtual void RaiseCustomFilterDisplayText(CustomFilterDisplayTextEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		static internal string GetFilterPanelText(object op) {
			if(op == null)
				return string.Empty;
			if(op is CriteriaOperator)
				return LocalaizableCriteriaToStringProcessor.Process(op as CriteriaOperator);
			return op.ToString();
		}
		bool GetActualShowFilterPanel() {
			if(ShowFilterPanelMode == ShowFilterPanelMode.Never)
				return false;
			if(ShowFilterPanelMode == ShowFilterPanelMode.ShowAlways)
				return true;
			return !object.ReferenceEquals(DataControl.FilterCriteria, null);
		}
		void UpdateColumnHeadersToolTipTemplate() {
			foreach(ColumnBase column in ColumnsCore)
				column.UpdateActualHeaderToolTipTemplate();
		}
#if DEBUGTEST
		internal int UpdateDataObjectsFireCount;
#endif
		internal void UpdateDataObjects(bool updateColumnsViewInfo = true, bool updateDataObjects = true) {
			if(updateColumnsViewInfo)
				UpdateColumnsViewInfo(true);
			UpdateFocusedRowData();
			if(updateDataObjects) {
#if DEBUGTEST
				UpdateDataObjectsFireCount++;
#endif
				ViewBehavior.UpdateAdditionalRowsData();
			}
		}
		internal void UpdateShowValidationAttributeError() {
			UpdateColumns((column) => column.UpdateActualShowValidationAttributeErrors());
			UpdateCellDataErrors();
		}
		internal void UpdateCellDataErrors() {
			if(DataControl == null) return;
			UpdateRowData((rowData) => rowData.UpdateDataErrors(), false);
		}
#if DEBUGTEST
		internal int UpdateColumnsViewInfoCount;
#endif
		internal void UpdateColumnsViewInfo(bool updateDataPropertiesOnly = false) {
			if(DataControl == null || DataControl.DataView != this || DataControl.IsLoading || DataControl.IsDeserializing || IsLockUpdateColumnsLayout || ColumnsCore.IsLockUpdate)
				return;
#if DEBUGTEST
			UpdateColumnsViewInfoCount++;
#endif
			if (DataControl != null && DataControl.DataProviderBase != null && DataControl.DataProviderBase.IsICollectionView) {
				ICollectionViewHelper helper = DataProviderBase.DataController.DataSource as ICollectionViewHelper;
				if (helper != null)
					helper.AllowSyncSortingAndGrouping = AllowSorting;
			}
			ViewBehavior.UpdateColumnsViewInfo(updateDataPropertiesOnly);
			UpdateTotalSummaries();
			UpdateShowEditFilterButtonCore();
			CommandManager.InvalidateRequerySuggested();
		}
		internal void UpdateColumnsTotalSummary() {
			UpdateColumns((column) => column.UpdateTotalSummaries());
			UpdateTotalSummaries();
		}
		internal void UpdateColumnsAppearance() {
			UpdateColumns((column) => column.UpdateAppearance());
		}
		internal virtual void UpdateColumns(Action<ColumnBase> updateColumnDelegate) {
			foreach(ColumnBase column in ColumnsCore) {
				updateColumnDelegate(column);
			}
		}
		internal void PerformNavigationOnLeftButtonDownCore(MouseButtonEventArgs e) {
			DependencyObject originalSource = e.OriginalSource as DependencyObject;
			IDataViewHitInfo hitInfo = RootView.CalcHitInfoCore(originalSource);
			SelectionStrategy.OnBeforeMouseLeftButtonDown(e);
			this.GetNavigation(this.GetRowHandleByTreeElement(originalSource)).ProcessMouse(originalSource);
			SelectionStrategy.OnAfterMouseLeftButtonDown(hitInfo, e.StylusDevice, e.ClickCount);
			ViewBehavior.OnAfterMouseLeftButtonDown(hitInfo);
		}
		internal void OnInvalidateHorizontalScrolling() {
			ViewBehavior.NavigationStrategyBase.OnInvalidateHorizontalScrolling(this);
		}
		internal ColumnBase CoerceFocusedColumn(ColumnBase newValue) {
			if(RequestUIUpdate())
				return newValue;
			return (ColumnBase)this.GetCoerceOldValue(GetFocusedColumnProperty());
		}
		internal abstract DependencyProperty GetFocusedColumnProperty();
		internal void CurrentColumnChanged(ColumnBase oldColumn) {
			SelectionStrategy.OnFocusedColumnChanged();
			UpdateIsFocusedCellIfNeeded(FocusedRowHandle, oldColumn);
			UpdateIsFocusedCellIfNeeded(FocusedRowHandle, DataControl.CurrentColumn);
			UpdateFullRowState(FocusedRowHandle);
			ForceUpdateRowsState();
			ViewBehavior.NavigationStrategyBase.OnNavigationIndexChanged(this);
			EnqueueMakeCellVisible();
		}
		void OnFocusedColumnChanged(GridColumnBase oldValue, GridColumnBase newValue) {
			if(DataControl != null && !ReferenceEquals(DataControl.CurrentColumn, (ColumnBase)GetValue(GetFocusedColumnProperty())))
				DataControl.CurrentColumn = (ColumnBase)GetValue(GetFocusedColumnProperty());
			RaiseFocusedColumnChanged(oldValue, newValue);
		}
		internal void RaiseFocusedColumnChanged(GridColumnBase oldValue, GridColumnBase newValue) {
			RaiseEventInOriginationView(new FocusedColumnChangedEventArgs(this, oldValue, newValue) { RoutedEvent = FocusedColumnChangedEvent });
		}
		void EnqueueMakeCellVisible() {
			if(RootDataPresenter != null)
				ImmediateActionsManager.EnqueueAction(ViewBehavior.MakeCellVisible);
		}
		protected virtual void ClearAllStates() {
			Navigation.ClearAllStates();
			AdditionalRowNavigation.ClearAllStates();
			if(DataProviderBase != null) {
				ClearSelectionCore();
				SetFocusedRowHandle(DataControlBase.InvalidRowHandle);
				if(DataControl != null)
					DataControl.ReInitializeCurrentColumn();
			}
		}
		protected internal void ForceUpdateRowsState() {
			RowsStateDirty = true;
			UpdateRowsState();
		}
		protected internal virtual void ForceLayout() {
			InvalidateMeasure();
		}
		internal GridRowsEnumerator CreateVisibleRowsEnumerator() {
			return new SkipInvisibleGridRowsEnumerator(this, RootNodeContainer);
		}
		internal VirtualItemsEnumerator CreateAllRowsEnumerator() {
			return new SkipCollapsedGroupVirtualItemsEnumerator(RootNodeContainer);
		}
		protected internal virtual bool UpdateRowsState() {
			FocusedView.UpdateBorderForFocusedUIElement();
			if(!RowsStateDirty)
				return false;
			CurrentCell = null;
			Navigation.UpdateRowsState();
			AdditionalRowNavigation.UpdateRowsState();
			RowsStateDirty = false;
			return true;
		}
		internal bool RequestUIUpdate() {
			return EnumerateViewsForCommitEditingAndRequestUIUpdate(view => view.RequestUIUpdateCore());
		}
		bool RequestUIUpdateCore(bool cleanError = false) {
			if(RootDataPresenter != null) RootDataPresenter.ForceCompleteCurrentAction();
			if(!RootView.LockEditorClose) CloseEditor(Navigation.NavigationMouseLocker.IsLocked, cleanError);
			if(!EditFormManager.RequestUIUpdate())
				return false;
			return !HasCellEditorError;
		}
		bool EnumerateViewsForCommitEditingAndRequestUIUpdate(Func<DataViewBase, bool> getResult) {
			bool result = getResult(this);
			if(this != MasterRootRowsContainer.FocusedView)
				result = result & getResult(MasterRootRowsContainer.FocusedView);
			return result;
		}
		protected internal virtual int FindRowHandle(DependencyObject element) {
			DependencyObject row = FindParentRow(element);
			if(row == null) return DataControlBase.InvalidRowHandle;
			return DataViewBase.GetRowHandle(row).Value;
		}
		protected internal static DependencyObject FindParentRow(DependencyObject obj) {
			DependencyObject row = obj;
			while(row != null && GetRowHandle(row) == null) {
				if(row is DataViewBase)
					return null;
				row = LayoutHelper.GetParent(row);
			}
			return row;
		}
		protected internal static DependencyObject FindParentCell(DependencyObject obj) {
			DependencyObject cell = obj;
			while(cell != null && ColumnBase.GetNavigationIndex(cell) == Constants.InvalidNavigationIndex) {
				if(cell is DataViewBase)
					return null;
				cell = LayoutHelper.GetParent(cell);
			}
			return cell;
		}
		internal abstract void RaiseCellValueChanged(int rowHandle, ColumnBase column, object newValue, object oldValue);
		internal void RaiseFocusedRowHandleChanged() {
			RaiseEvent(new FocusedRowHandleChangedEventArgs(FocusedRowData) { RoutedEvent = FocusedRowHandleChangedEvent });
		}
		internal void RaiseFocusedRowChanged(object oldValue, object newValue) {
			RaiseEventInOriginationView(new FocusedRowChangedEventArgs(this, oldValue, newValue) { RoutedEvent = FocusedRowChangedEvent });
		}
		internal void RaiseFocusedViewChanged(DataViewBase oldView, DataViewBase newView) {
			RaiseEvent(new FocusedViewChangedEventArgs(oldView, newView) { RoutedEvent = FocusedViewChangedEvent });
			RaisePropertyChanged("FocusedView");
		}
		internal object GetWpfRow(RowHandle rowHandle, int listSourceRowIndex) {
			return DataProviderBase.GetWpfRow(rowHandle, listSourceRowIndex);
		}
		internal object GetRowValue(RowHandle rowHandle) {
			return DataProviderBase.GetRowValue(rowHandle.Value);
		}
		internal void PerformDataResetAction() {
			if(!SupressCacheCleanCountLocker.IsLocked) {
				DataControl.UpdateRowsCore();
			}
			else {
				RootNodeContainer.ReGenerateMasterRootItems();
			}
		}
		protected internal virtual void SetSummariesIgnoreNullValues(bool value) { }
		internal void UpdateSummariesIgnoreNullValues() {
			SetSummariesIgnoreNullValues(SummariesIgnoreNullValues);
		}
		protected virtual void OnSelectedRowsSourceChanged() {
			if(DataControl != null)
				DataControl.SelectedItems = (IList)GetValue(SelectedRowsSourceProperty);
		}
		protected internal abstract MultiSelectMode GetSelectionMode();
		internal MultiSelectMode GetActualSelectionMode() {
			return (DataControl == null || DataControl.SelectionMode == MultiSelectMode.None) ? GetSelectionMode() : DataControl.SelectionMode;
		}
		internal void ValidateSelectionStrategy() {
			if(selectionStrategy != null && selectionStrategy.GetType() != CreateSelectionStrategy().GetType())
				ResetSelectionStrategy();
		}
		#region localization
		void OnRuntimeLocalizationStringsChanged(GridRuntimeStringCollection oldValue) {
			if(RuntimeLocalizationStrings != null) {
				RuntimeLocalizationStrings.CollectionChanged += RuntimeLocalizationStringsCollectionChanged;
			}
			if(oldValue != null) {
				oldValue.CollectionChanged -= RuntimeLocalizationStringsCollectionChanged;
			}
			RecreateLocalizationDescriptor();
		}
		void RecreateLocalizationDescriptor() {
			LocalizationDescriptor = new LocalizationDescriptor(RuntimeLocalizationStrings);
		}
		void RuntimeLocalizationStringsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RecreateLocalizationDescriptor();
		}
		#endregion
		#region validation
		protected virtual string GetInvalidRowErrorText(string errorText) {
			string text;
			if(errorText.EndsWith("\n") || errorText.EndsWith("\r")) {
				text = errorText + GetLocalizedString(GridControlStringId.InvalidRowExceptionMessage);
			}
			else {
				text = errorText + "\r\n" + GetLocalizedString(GridControlStringId.InvalidRowExceptionMessage);
			}
			return text;
		}
		protected virtual MessageBoxResult DisplayInvalidRowError(IInvalidRowExceptionEventArgs e) {
			string messageBoxText = GetInvalidRowErrorText(e.ErrorText);
			string caption = e.WindowCaption;
#if !SL
			Window window = DataControl.Window;
			const MessageBoxButton button = MessageBoxButton.YesNo;
			const MessageBoxImage image = MessageBoxImage.Error;
			if(window != null)
				return MessageBox.Show(window, messageBoxText, caption, button, image);
			else
				return MessageBox.Show(messageBoxText, caption, button, image);
#else
			return MessageBox.Show(messageBoxText, caption, MessageBoxButton.OKCancel);
#endif
		}
		protected void HandleInvalidRowExceptionEventArgs(ControllerRowExceptionEventArgs e, IInvalidRowExceptionEventArgs eventArgs) {
			if(eventArgs.ExceptionMode == ExceptionMode.DisplayError) {
				MessageBoxResult result = DisplayInvalidRowError(eventArgs);
				if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
					eventArgs.ExceptionMode = ExceptionMode.Ignore;
				AllowMouseMoveSelection = false;
			}
			if(eventArgs.ExceptionMode == ExceptionMode.Ignore) {
				e.Action = ExceptionAction.CancelAction;
			}
			if(eventArgs.ExceptionMode == ExceptionMode.ThrowException) {
				throw e.Exception;
			}
			if(e.Action == ExceptionAction.CancelAction)
				DataControl.SetRowStateError(e.RowHandle, null);
		}
		#endregion
		#region navigation
		internal bool IsExpanded(int rowHandle) {
			RowNode rowNode = Nodes.ContainsKey(rowHandle) ? Nodes[rowHandle] : null;
			if(rowNode == null)
				return false;
			return rowNode.IsRowExpandedForNavigation();
		}
		internal abstract bool IsDataRowNodeExpanded(DataRowNode rowNode);
#if !SL
		protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e) {
			base.OnPreviewMouseDoubleClick(e);
			DataViewBase targetView = DataControl.FindTargetView(e.OriginalSource);
			if(targetView == this || targetView.OriginationView != null)
				targetView.ViewBehavior.OnDoubleClick(e);
		}
#endif
		internal void ProcessStylusUp(DependencyObject source) {
			DataControl.FindTargetView(source).InplaceEditorOwner.ProcessStylusUpCore(source);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			MasterRootRowsContainer.FocusedView.InplaceEditorOwner.ProcessKeyUp(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			MasterRootRowsContainer.FocusedView.InplaceEditorOwner.ProcessKeyDown(e);
		}
		internal void ProcessMouseLeftButtonDown(MouseButtonEventArgs e) {
			DataViewBase targetView = DataControl.FindTargetView(e.OriginalSource);
			if(targetView != this && targetView.IsRootView)
				return;
			targetView.UpdateAllowMouseMoveSelection(e);
			if(SearchControl != null && e.OriginalSource is UIElement && LayoutHelper.IsChildElement(SearchControl, (UIElement)e.OriginalSource)) {
				SetSearchPanelFocus(true);
				return;
			}
			else
				SetSearchPanelFocus();
			targetView.InplaceEditorOwner.ProcessMouseLeftButtonDown(e);
		}
		void UpdateAllowMouseMoveSelection(MouseButtonEventArgs e) {
			Func<MouseEventArgs, bool> allowMouseSelection = null;
			if(this.DataControl != null)
				allowMouseSelection = DragManager.GetAllowMouseMoveSelectionFunc(this.DataControl);
			AllowMouseMoveSelection = allowMouseSelection != null ? allowMouseSelection(e) : true;
		}
		internal void ProcessMouseRightButtonDown(MouseButtonEventArgs e) {
			DataControl.FindTargetView(e.OriginalSource).InplaceEditorOwner.ProcessMouseRightButtonDown(e);
		}
		internal void ProcessPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			InplaceEditorOwner.ProcessPreviewLostKeyboardFocus(e);
		}
		internal void ProcessIsKeyboardFocusWithinChanged() {
			if(!IsKeyboardFocusWithin)
				AllowMouseMoveSelection = false;
			if(!IsKeyboardFocusInSearchPanel() && !IsKeyboardFocusInHeadersPanel() && !IsKeyboardFocusWithinContentView())
				InplaceEditorOwner.ProcessIsKeyboardFocusWithinChanged();
			UpdateIsKeyboardFocusWithinView();
			UpdateBorderForFocusedUIElement();
			if(IsKeyboardFocusWithinView) ViewBehavior.OnGotKeyboardFocus();
		}
		bool IsKeyboardFocusWithinContentView() {
			var focusedElement = FocusHelper.GetFocusedElement() as DependencyObject;
			if(focusedElement == null)
				return false;
			DataViewBase contentView = LayoutHelper.FindLayoutOrVisualParentObject<DataViewBase>(focusedElement);
			return contentView != null && contentView != this && contentView.IsRootView;
		}
#if DEBUGTEST && !SL
		internal void ProcessMouse(DependencyObject originalSource) {
			ProcessMouseLeftButtonDown(new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left) { RoutedEvent = MouseLeftButtonDownEvent, Source = originalSource });
		}
#endif
		protected virtual void OnNavigationStyleChanged() {
			ClearAllStates();
			Navigation = null;
			ValidateSelectionStrategy();
			UpdateActualAllowCellMergeCore();
		}
		protected internal bool GetIsCellFocused(int rowHandle, ColumnBase column) {
			return GetNavigation(rowHandle).GetIsFocusedCell(rowHandle, column);
		}
		internal int CoerceFocusedRowHandle(int newValue) {
			FocusedRowHandleChangedLocker.Lock();
			try {
				if(IsAutoFilterRowFocused && DataControl.IsDataResetLocked) {
					return FocusedRowHandle;
				}
				if(newValue == FocusedRowHandle) {
					return FocusedRowHandle;
				}
				if(!FocusedView.CommitEditing()) {
					SelectionStrategy.OnNavigationCanceled();
					return FocusedRowHandle;
				}
				if(ViewBehavior.CheckNavigationStyle(newValue)) {
					return DataControlBase.InvalidRowHandle;
				}
				if(DataControl != null && !DataControl.DataSourceChangingLocker.IsLocked && IsSynchronizedWithCurrentItem && DataProviderBase.CollectionViewSource != null
					&& !IsNewItemRowHandle(newValue) && !DataProviderBase.IsGroupRowHandle(newValue)) {
					if(dataControl.isSync) {
						object val = GetRowValue(new RowHandle(newValue));
						DataProviderBase.CollectionViewSource.MoveCurrentTo(val);
						return val == DataProviderBase.CollectionViewSource.CurrentItem ? newValue : FocusedRowHandle;
					}
				}
				return newValue;
			}
			finally {
				FocusedRowHandleChangedLocker.Unlock();
			}
		}
		protected internal DependencyObject FindNavigationIndex(int minIndex, int maxIndex, bool findMin, bool isTabNavigation) {
			DependencyObject current = null;
			int navIndex, currIndex = findMin ? Int32.MaxValue : Int32.MinValue;
			DataViewBase rootView = RootView;
			VisualTreeEnumerator ve = new VisualTreeEnumerator(FocusedRowElement);
			while(ve.MoveNext()) {
				DependencyObject dobj = ve.Current as DependencyObject;
				if(dobj == null)
					break;
				if(!ReferenceEquals(rootView, LayoutHelper.FindLayoutOrVisualParentObject(dobj, GetType())))
					break;
				navIndex = ColumnBase.GetNavigationIndex(dobj);
				bool isElementVisible = dobj is FrameworkElement && UIElementHelper.IsVisibleInTree((FrameworkElement)dobj);
				if(navIndex == Constants.InvalidNavigationIndex || navIndex < minIndex || navIndex > maxIndex || !isElementVisible ||
					(navIndex < VisibleColumnsCore.Count && !IsColumnNavigatable(VisibleColumnsCore[navIndex], isTabNavigation))) 
					continue;
				if((currIndex > navIndex && findMin) || (currIndex < navIndex && !findMin)) {
					current = dobj;
					currIndex = navIndex;
					if((findMin && currIndex == minIndex) || (!findMin && currIndex == maxIndex))
						break;
				}
			}
			return current;
		}
		protected internal DependencyObject FindNearLeftNavigationIndex(int currIndex, bool isTabNavigation) {
			return FindNavigationIndex(0, currIndex - 1, false, isTabNavigation);
		}
		protected internal DependencyObject FindNearRightNavigationIndex(int currIndex, bool isTabNavigation) {
			return FindNavigationIndex(currIndex + 1, int.MaxValue, true, isTabNavigation);
		}
		DependencyObject currentCell;
		internal DependencyObject CurrentCell {
			get { return currentCell; }
			set {
				if(CurrentCell == value)
					return;
				currentCell = value;
				OnCurrentCellChanged();
			}
		}
		internal virtual void OnCurrentCellChanged() {
			if(CurrentCell == null) return;
			if(NavigationStyle == GridViewNavigationStyle.Cell)
				RaiseItemSelectAutomationEvents(CurrentCell);
			UpdateBorderForFocusedUIElement();
		}
		internal void OnPostponedNavigationComplete() {
			SelectionStrategy.OnNavigationComplete(false);
			PostponedNavigationInProgress = false;
			MasterRootRowsContainer.UpdatePostponedData(true, false);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			ViewBehavior.ProcessPreviewKeyDown(e);
		}
#if DEBUGTEST
		internal void ProcessPreviewKeyDown(KeyEventArgs e) {
			ViewBehavior.ProcessPreviewKeyDown(e);
		}
#endif
		internal void ProcessKeyDown(KeyEventArgs e) {
			if(ViewBehavior.IsNavigationLocked) {
				e.Handled = true;
				return;
			}
			ProcessKeyDownCore(e);
			if(!e.Handled)
				base.OnKeyDown(e);
		}
		private void ProcessKeyDownCore(KeyEventArgs e) {
			if(IsColumnFilterOpened || IsContextMenuOpened) return;
			SelectionStrategy.OnBeforeProcessKeyDown(e);
			if((RootDataPresenter != null) && RootDataPresenter.IsInAction)
				RootDataPresenter.ForceCompleteContinuousActions();
			if(e.Key == Key.Tab && (ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) || AllowLeaveFocusOnTab)) {
#if !SL //TODO SL
				InplaceEditorOwner.MoveFocus(e);
				e.Handled = true;
#endif
			}
			SearchControlKeyDownProcessing(e);
			GetNavigation(FocusedRowHandle).ProcessKey(e);
			SelectionStrategy.OnAfterProcessKeyDown(e);
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchControl"),
#endif
 Category(Categories.SearchPanel), CloneDetailMode(CloneDetailMode.Skip)]
		public SearchControl SearchControl {
			get { return (SearchControl)GetValue(SearchControlProperty); }
			set { SetValue(SearchControlProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataViewBaseSearchPanelNullText"),
#endif
 Category(Categories.SearchPanel), CloneDetailMode(CloneDetailMode.Skip)]
		public string SearchPanelNullText {
			get { return (string)GetValue(SearchPanelNullTextProperty); }
			set { SetValue(SearchPanelNullTextProperty, value); }
		}
		internal void SelectRowForce() {
			SelectionStrategy.SelectRowForce();
		}
		protected virtual void OnColumnHeaderClick(ColumnBase column, bool isShift, bool isCtrl) {
			ColumnHeaderClickEventArgs args = new ColumnHeaderClickEventArgs(column, isShift, isCtrl) { RoutedEvent = ColumnHeaderClickEvent };
			RaiseColumnHeaderClick(args);
			if(args.Handled) {
				isShift = args.IsShift;
				isCtrl = args.IsCtrl;
			}
			if(!args.Handled || args.AllowSorting) {
				SortInfoCore.OnColumnHeaderClick(column.FieldName, isShift, isCtrl);
			}
		}
		protected virtual bool CanNavigateToNewItemRow { get { return false; } }
		protected internal void MovePrevCell(bool isTabNavigation) {
			if(ViewBehavior.NavigationStrategyBase.IsBeginNavigationIndex(this, isTabNavigation) || ShouldChangeRowByTab) {
				if(IsAutoFilterRowFocused || !ViewBehavior.AutoMoveRowFocusCore)
					return;
				if(IsTopNewItemRowFocused && IsRootView)
					return;
				bool isFirstRow = (IsFirstRow && !CanNavigateToNewItemRow) || IsTopNewItemRowFocused;
				if(isFirstRow) {
					DataControl.NavigateToMasterCell(isTabNavigation);
					return;
				}
				if(DataControl.NavigateToPreviousInnerDetailCell(isTabNavigation))
					return;
				MovePrevRow();
				MoveLastNavigationIndex(isTabNavigation);
				return;
			}
			MovePrevCellCore(isTabNavigation);
		}
		protected internal void MoveNextCell(bool isTabNavigation, bool isEnterNavigation = false) {
			if(ViewBehavior.NavigationStrategyBase.IsEndNavigationIndex(this, isTabNavigation) || ShouldChangeRowByTab) {
				bool moveNextTopNewItemRowCell = IsTopNewItemRowFocused && (IsRootView || ViewBehavior.IsNewItemRowEditing || isEnterNavigation);
				bool moveNextBottomNewItemRowCell = IsBottomNewItemRowFocused && (ViewBehavior.IsNewItemRowEditing || (!IsRootView && isEnterNavigation));
				if(moveNextTopNewItemRowCell || moveNextBottomNewItemRowCell) {
					MoveNextNewItemRowCell();
					return;
				}
				if(IsAutoFilterRowFocused || !ViewBehavior.AutoMoveRowFocusCore)
					return;
				if(DataControl.NavigateToFirstChildDetailCell(isTabNavigation))
					return;
				if(IsLastRow) {
					DataControl.NavigateToNextOuterMasterCell(isTabNavigation);
					return;
				}
				MoveNextRow();
				MoveFirstNavigationIndex(isTabNavigation);
				return;
			}
			MoveNextCellCore(isTabNavigation);
		}
		protected virtual bool ShouldChangeRowByTab { get { return IsExpandableRowFocused(); } }
		void EnqueueScrollIntoView(int rowHandle) {
			ScrollRowIntoViewAction currentAction = ImmediateActionsManager.FindActionOfType(typeof(ScrollRowIntoViewAction)) as ScrollRowIntoViewAction;
			if(currentAction != null) {
				currentAction.Reassign(this, rowHandle);
			} else {
				EnqueueImmediateAction(new ScrollRowIntoViewAction(this, rowHandle, 0));
			}
		}
		public void ScrollIntoView(int rowHandle) {
			ScrollIntoViewLocker.DoIfNotLocked(delegate {
				DataProviderBase.MakeRowVisible(rowHandle);
				EnqueueScrollIntoView(rowHandle);
			});
		}
		public void ScrollIntoView(object row) {
			ScrollIntoView(DataProviderBase.FindRowByRowValue(row));
		}
		public virtual void MovePrevCell() {
			MovePrevCell(false);
		}
		public virtual void MoveNextCell() {
			MoveNextCell(false);
		}
		protected internal void MoveFirstNavigationIndex() {
			MoveFirstNavigationIndex(false);
		}
		protected internal void MoveLastNavigationIndex() {
			MoveLastNavigationIndex(false);
		}
		internal void MovePrevCellCore(bool isTabNavigation = false) {
			ViewBehavior.NavigationStrategyBase.MovePrevNavigationIndex(this, isTabNavigation);
		}
		internal void MoveNextCellCore(bool isTabNavigation = false) {
			ViewBehavior.NavigationStrategyBase.MoveNextNavigationIndex(this, isTabNavigation);
		}
		internal void MoveFirstNavigationIndex(bool isTabNavigation) {
			ViewBehavior.NavigationStrategyBase.MoveFirstNavigationIndex(this, isTabNavigation);
		}
		internal void MoveFirstNavigationIndexCore(bool isTabNavigation) {
			ViewBehavior.NavigationStrategyBase.MoveFirstNavigationIndexCore(this, isTabNavigation);
		}
		internal void MoveLastNavigationIndex(bool isTabNavigation) {
			ViewBehavior.NavigationStrategyBase.MoveLastNavigationIndex(this, isTabNavigation);
		}
#if SL
		internal virtual bool NeedsKey(Key key, ModifierKeys modifiers) {
			return false;
		}
#endif
		public virtual void MovePrevRow() {
			ViewBehavior.MovePrevRow();
		}
		public virtual void MoveNextRow() {
			ViewBehavior.MoveNextRow();
		}
		public virtual void MovePrevPage() {
			if(DataControl.VisibleRowCount == 0 && !IsNewItemRowVisible)
				return;
			if(IsAdditionalRowFocused && IsRootView) 
				return;
			MovePrevPageCore();
		}
		protected virtual void MovePrevPageCore() {
			if(ShouldScrollOnePageBack()) {
				if(RootDataPresenter.CanScrollWithAnimation) {
					ScrollOnePageUpWithAnimationAction scrollOnePageUpAction = new ScrollOnePageUpWithAnimationAction(this);
					scrollOnePageUpAction.Execute();
				} else {
					ScrollOnePageUpAction scrollOnePageUpAction = new ScrollOnePageUpAction(this);
					scrollOnePageUpAction.Execute();
				}
			} else {
				MoveFocusedRowToFirstScrollRow();
				SelectionStrategy.OnNavigationComplete(false);
			}
		}
		public virtual void MoveNextPage() {
			if(DataControl.VisibleRowCount == 0 && !IsNewItemRowVisible)
				return;
			if(!CanMoveFromFocusedRow()) 
				return;
			MoveNextPageCore();
		}
		protected virtual void MoveNextPageCore() {
			if(ShouldScrollOnePageForward()) {
				if(RootDataPresenter.CanScrollWithAnimation) {
					ScrollOnePageDownWithAnimationAction scrollOnePageDownAction = new ScrollOnePageDownWithAnimationAction(this);
					scrollOnePageDownAction.Execute();
				} else {
					ScrollOnePageDownAction scrollOnePageDownAction = new ScrollOnePageDownAction(this);
					scrollOnePageDownAction.Execute();
				}
			} else {
				MoveFocusedRowToLastScrollRow();
				SelectionStrategy.OnNavigationComplete(false);
			}
		}
		internal bool CanMoveFromFocusedRow() {
			if(IsAutoFilterRowFocused && NavigationStyle == GridViewNavigationStyle.Cell)
				return DataControl.CurrentColumn != null && DataControl.CurrentColumn.AllowFocus;
			return true;
		}
		bool ShouldScrollOnePageBack() {
			KeyValuePair<DataViewBase, int> commonCurrentItem = new KeyValuePair<DataViewBase, int>();
			KeyValuePair<DataViewBase, int> commonTargetItem = new KeyValuePair<DataViewBase, int>();
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			GetFirstScrollRowViewAndVisibleIndex(out targetView, out targetVisibleIndex);
			FindCommonViewVisibleIndexes(targetView, targetVisibleIndex, out commonCurrentItem, out commonTargetItem);
			int commonVisibleIndexDifference = commonCurrentItem.Value - commonTargetItem.Value;
			return commonVisibleIndexDifference <= 0;
		}
		bool ShouldScrollOnePageForward() {
			if(IsTopNewItemRowFocused) return false;
			KeyValuePair<DataViewBase, int> commonCurrentItem = new KeyValuePair<DataViewBase, int>();
			KeyValuePair<DataViewBase, int> commonTargetItem = new KeyValuePair<DataViewBase, int>();
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			GetLastScrollRowViewAndVisibleIndex(out targetView, out targetVisibleIndex);
			if(targetView == null)
				return false;
			FindCommonViewVisibleIndexes(targetView, targetVisibleIndex, out commonCurrentItem, out commonTargetItem);
			int commonVisibleIndexDifference = commonCurrentItem.Value - commonTargetItem.Value;
			if(commonVisibleIndexDifference == 0) {
				int currentScrollIndex = ConvertVisibleIndexToScrollIndex(DataProviderBase.CurrentIndex);
				int lastScrollRowScrollIndex = targetView.ConvertVisibleIndexToScrollIndex(targetVisibleIndex);
				if(currentScrollIndex > lastScrollRowScrollIndex) {
					List<int> fixedRowsScrollIndexes = targetView.DataControl.GetParentFixedRowsScrollIndexes(targetVisibleIndex);
					if(fixedRowsScrollIndexes.Contains(currentScrollIndex))
						return false;
				}
			}
			return commonVisibleIndexDifference >= 0;
		}
		void FindCommonViewVisibleIndexes(DataViewBase targetView, int targetVisibleIndex,
			out KeyValuePair<DataViewBase, int> commonCurrentItem, out KeyValuePair<DataViewBase, int> commonTargetItem) {
			List<KeyValuePair<DataViewBase, int>> currentViewChain = DataControl.GetViewVisibleIndexChain(DataProviderBase.CurrentIndex);
			if(currentViewChain.Count == 0)
				currentViewChain = DataControl.GetViewVisibleIndexChain(targetVisibleIndex);
			if(targetView == null) {
				commonCurrentItem = commonTargetItem = currentViewChain[0];
				return;
			}		 
			List<KeyValuePair<DataViewBase, int>> targetViewChain = targetView.DataControl.GetViewVisibleIndexChain(targetVisibleIndex);
			commonCurrentItem = currentViewChain[0];
			commonTargetItem = targetViewChain[0];
			foreach(KeyValuePair<DataViewBase, int> currentItem in currentViewChain) {
				foreach(KeyValuePair<DataViewBase, int> targetItem in targetViewChain) {
					if(currentItem.Key == targetItem.Key) {
						commonCurrentItem = currentItem;
						commonTargetItem = targetItem;
						break;
					}
				}
			}
		}
		internal int CalcFirstScrollRowScrollIndex() {
			return Math.Min((int)Math.Ceiling(RootDataPresenter.ActualScrollOffset), RootDataPresenter.ItemCount - 1);
		}
		internal void GetFirstScrollRowViewAndVisibleIndex(out DataViewBase view, out int visibleIndex) {
			if(!DataControl.FindViewAndVisibleIndexByScrollIndex(CalcFirstScrollRowScrollIndex(), true, out view, out visibleIndex))
				DataControl.FindViewAndVisibleIndexByScrollIndex(CalcFirstScrollRowScrollIndex(), false, out view, out visibleIndex);
			DataViewBase targetView = view;
			DataControlBase targetDataControl = view.DataControl;
			int targetVisibleIndex = visibleIndex;
			int targetRowHandle = targetDataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			if(targetDataControl.IsExpandedFixedRow(targetVisibleIndex)) {
				GridRowsEnumerator gridRowsEnumerator = RootView.CreateVisibleRowsEnumerator();
				while(gridRowsEnumerator.MoveNext()) {
					RowData rowData = gridRowsEnumerator.CurrentRowData as RowData;
					if((rowData != null) && (rowData.View == targetView) && (rowData.RowHandle.Value == targetRowHandle)) {
						break;
					}
				}
				if(gridRowsEnumerator.MoveNext()) {
					RowData rowData = gridRowsEnumerator.CurrentRowData as RowData;
					if(rowData != null) {
						view = rowData.View;
						visibleIndex = targetDataControl.GetRowVisibleIndexByHandleCore(rowData.RowHandle.Value);
					}
				}
			}
		}
		internal int CalcLastScrollRowScrollIndex() {
			DataViewBase lastDataRowView = null;
			int lastDataRowVisibleIndex = 0;
			GetLastScrollRowViewAndVisibleIndex(out lastDataRowView, out lastDataRowVisibleIndex);
			if(lastDataRowView == null) {
				return 0;
			} else {
				return lastDataRowView.ConvertVisibleIndexToScrollIndex(lastDataRowVisibleIndex);
			}
		}
		internal void GetLastScrollRowViewAndVisibleIndex(out DataViewBase view, out int visibleIndex) {
			RowData lastRowData = null;
			int i = 0;
			Point prevElementBottomRight = new Point();
			GridRowsEnumerator gridRowsEnumerator = RootView.CreateVisibleRowsEnumerator();
			while(gridRowsEnumerator.MoveNext()) {
				Rect elementRect = LayoutHelper.GetRelativeElementRect(gridRowsEnumerator.CurrentRowData.WholeRowElement, RootDataPresenter);
				Point elementPoint = elementRect.TopLeft();
				if(elementPoint.X >= prevElementBottomRight.X || elementPoint.Y >= prevElementBottomRight.Y) {
					if(SizeHelper.GetDefinePoint(elementPoint) >= 0)
						i++;
				}
				prevElementBottomRight = elementRect.BottomRight();
				RowData rowData = gridRowsEnumerator.CurrentRowData as RowData;
				if(rowData != null && (!(rowData.MatchKey is GroupSummaryRowKey))) {
					lastRowData = rowData;
				}
				if(i >= RootDataPresenter.FullyVisibleItemsCount) {
					break;
				}
			}
			if(lastRowData != null) {
				view = lastRowData.View;
				visibleIndex = lastRowData.View.DataControl.GetRowVisibleIndexByHandleCore(lastRowData.RowHandle.Value);
			} else {
				view = null;
				visibleIndex = -1;
			}
		}
		protected internal virtual void MoveFocusedRowToFirstScrollRow() {
			DataViewBase targetView = null;
			int targetVisibleIndex = 0;
			GetFirstScrollRowViewAndVisibleIndex(out targetView, out targetVisibleIndex);
			MoveFocusedRowToScrollIndexForPageUpPageDown(targetView.ConvertVisibleIndexToScrollIndex(targetVisibleIndex), true);
		}
		protected internal virtual void MoveFocusedRowToLastScrollRow() {
			MoveFocusedRowToScrollIndexForPageUpPageDown(CalcLastScrollRowScrollIndex(), false);
		}
		internal void MoveFocusedRowToScrollIndexForPageUpPageDown(int scrollIndex, bool pageUp) {
			DataViewBase targetView = null;
			int targetVisibleIndex = 0;
			if((scrollIndex == 0) && pageUp) {
				targetView = RootView;
				targetVisibleIndex = 0;
			} else {
				DataControl.FindViewAndVisibleIndexByScrollIndex(scrollIndex, pageUp, out targetView, out targetVisibleIndex);
			}
			int targetRowHandle = targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex);
			if(targetView.DataControl.VisibleRowCount == 0 && targetView.IsNewItemRowVisible)
				targetRowHandle = DataControlBase.NewItemRowHandle;
			if(RootDataPresenter.CanScrollWithAnimation && targetView == FocusedView && targetRowHandle == targetView.FocusedRowHandle)
				ScrollIntoView(targetRowHandle);
			FocusViewAndRow(targetView, targetRowHandle);
		}
		bool postponedNavigationInProgress;
		internal bool PostponedNavigationInProgress {
			get { return postponedNavigationInProgress; }
			private set { postponedNavigationInProgress = value; }
		}
		internal void AddScrollOneItemAfterPageUpAction(DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount) {
			PostponedNavigationInProgress = true;
			EnqueueImmediateAction(new ScrollOneItemAfterPageUpAction(this, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount));
		}
		internal void AddScrollOneItemAfterPageDownAction(DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount) {
			PostponedNavigationInProgress = true;
			IAction action = null;
			if(RootView.ViewBehavior.AllowPerPixelScrolling)
				action = new ScrollOneItemAfterPageDownPerPixelAction(this, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount);
			else
				action = new ScrollOneItemAfterPageDownAction(this, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount);
			EnqueueImmediateAction(action);
		}
		protected internal virtual int ConvertVisibleIndexToScrollIndex(int visibleIndex) {
			int scrollIndexWithDetails = 0;
			DataControl.EnumerateThisAndParentDataControls((dataControl, index) => {
				int scrollIndex = dataControl.DataProviderBase.ConvertVisibleIndexToScrollIndex(index, dataControl.DataView.AllowFixedGroupsCore);
				scrollIndexWithDetails += scrollIndex;
				scrollIndexWithDetails += dataControl.MasterDetailProvider.CalcVisibleDetailRowsCountBeforeRow(scrollIndex);
			}, visibleIndex);
			int rowHandle = DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			scrollIndexWithDetails += DataControl.MasterDetailProvider.CalcVisibleDetailRowsCountForRow(rowHandle);
			return scrollIndexWithDetails;
		}
		protected internal SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(OrientationCore); } }
		protected internal virtual bool ShouldChangeForwardIndex(int rowHandle) {
			DataViewBase lastScrollRowView = null;
			int lastScrollRowVisibleIndex = -1;
			GetLastScrollRowViewAndVisibleIndex(out lastScrollRowView, out lastScrollRowVisibleIndex);
			int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			if(visibleIndex <= lastScrollRowVisibleIndex + 1 && lastScrollRowView == this) {
				if(ScrollActionsHelper.IsRowElementVisible(this, rowHandle))
					return true;
				else if(ScrollActionsHelper.GetGroupSummaryRowCountBeforeRow(this, visibleIndex, true) > 0)
					return true;
			}
			return false;
		}
		protected internal virtual double CalcOffsetForward(int rowHandle, bool perPixelScrolling) {
			FrameworkElement elem = null;
			if(rowHandle == DataControlBase.NewItemRowHandle && IsNewItemRowVisible)
				elem = GetRowElementByRowHandle(rowHandle);
			else
				elem = GetRowElementByVisibleIndex(dataControl.GetRowVisibleIndexByHandleCore(rowHandle));
			if(elem == null || !elem.IsInVisualTree()) return 1.0;
			return CalcOffsetByHeight(GetItemInvisibleSize(elem), perPixelScrolling);
		}
		internal double CalcOffsetForBackwardScrolling(int firstInvisibleIndex) {
			FrameworkElement elem = GetRowElementByVisibleIndex(firstInvisibleIndex);
			if(elem == null || !elem.IsInVisualTree()) return 0;
			double visibleSize = GetItemVisibleSize(elem);
			if(visibleSize == 0)
				return 0;
			return CalcOffsetByHeight(visibleSize, true);
		}
		FrameworkElement GetRowElementByVisibleIndex(int index) {
			RowData rowData = GetRowData(DataControl.GetRowHandleByVisibleIndexCore(index));
			return rowData != null ? GetRowVisibleElement(rowData) : null;
		}
		protected virtual double GetItemInvisibleSize(FrameworkElement elem) {
			Rect elemRect = LayoutHelper.GetRelativeElementRect(elem, RootDataPresenter);
			return SizeHelper.GetDefineSize(elemRect.Size()) - GetItemVisibleSize(elem);
		}
		protected virtual double GetItemVisibleSize(FrameworkElement elem) {
			Rect elemRect = LayoutHelper.GetRelativeElementRect(elem, RootDataPresenter);
			return SizeHelper.GetDefineSize(RootDataPresenter.LastConstraint) - SizeHelper.GetDefinePoint(elemRect.Location());
		}
		double CalcOffsetByHeight(double invisibleHeight, bool perPixelScrolling) {
			double offset = 0;
			GridRowsEnumerator en = RootView.CreateVisibleRowsEnumerator();
			RowDataBase rowDataToScroll = RootDataPresenter.GetRowDataToScroll();
			bool rowDataToScrollFound = false;
			double prevElementBottom = 0;
			while(en.MoveNext()) {
				FrameworkElement row = GetRowVisibleElement(en.CurrentRowData);
				Rect rowRect = LayoutHelper.GetRelativeElementRect(row, RootDataPresenter);
				if(!rowDataToScrollFound) {
					if(rowDataToScroll == null || rowDataToScroll.node == en.CurrentNode) {
						rowDataToScrollFound = true;
					} else {
						prevElementBottom = rowRect.Bottom;
						continue;
					}
				}
				double currentRowHeight = SizeHelper.GetDefineSize(rowRect.Size());
				if(SizeHelper.GetDefinePoint(rowRect.Location()) < prevElementBottom) {
					invisibleHeight -= SizeHelper.GetDefinePoint(rowRect.Location()) - prevElementBottom;
				}
				if(currentRowHeight >= invisibleHeight) {
					offset += perPixelScrolling ? invisibleHeight / currentRowHeight : 1;
					break;
				} else {
					invisibleHeight -= currentRowHeight;
					offset++;
				}
				prevElementBottom = rowRect.Bottom;
			}
			return offset;
		}
		public void MoveFocusedRow(int visibleIndex) {
			FocusViewAndRow(this, DataControl.GetRowHandleByVisibleIndexCore(visibleIndex));
		}
		internal void FocusViewAndRow(DataViewBase view, int rowHandle) {
			if(MasterRootRowsContainer.FocusedView == view || MasterRootRowsContainer.FocusedView.CommitEditing()) {
				MasterRootRowsContainer.FocusedView = view;
				view.SetFocusedRowHandle(rowHandle);
			}
		}
		public virtual void MoveFirstRow() {
			MoveFocusedRow(0);
		}
		internal void NavigateToFirstRow() {
			if(!IsNewItemRowVisible) {
				MoveFirstRow();
			}
			else {
				FocusViewAndRow(this, DataControlBase.NewItemRowHandle);
			}
		}
		public virtual void MoveLastRow() {
			if(CanMoveFromFocusedRow())
				MoveFocusedRow(DataControl.VisibleRowCount - 1);
		}
		internal void NavigateToLastRow() {
			if(DataControl.VisibleRowCount > 0) {
				MoveFocusedRow(DataControl.VisibleRowCount - 1);
			}
			else if(IsNewItemRowVisible) {
				FocusViewAndRow(this, DataControlBase.NewItemRowHandle);
			}
		}
		internal virtual void MoveFirstMasterRow() {
			DataControl.NavigateToFirstMasterRow();
		}
		internal virtual void MoveLastMasterRow() {
			DataControl.NavigateToLastMasterRow();
		}
		internal void MoveFirstOrFirstMasterRow() {
			if(DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle) != 0) {
				MoveFirstRow();
			} else {
				MoveFirstMasterRow();
			}
		}
		internal void MoveLastOrLastMasterRow() {
			if(DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle) != DataProviderBase.VisibleCount - 1) {
				MoveLastRow();
			} else {
				MoveLastMasterRow();
			}
		}
		protected bool HasParentRow(int visibleRowIndex) {
			int rowHandle = DataControl.GetRowHandleByVisibleIndexCore(visibleRowIndex);
			return DataProviderBase.GetParentRowHandle(rowHandle) != DevExpress.Data.DataController.InvalidRow;
		}
		internal void MoveParentRow() {
			if(HasParentRow(DataProviderBase.CurrentIndex)) {
				int parentIndex = DataProviderBase.GetParentRowIndex(DataProviderBase.CurrentIndex);
				MoveFocusedRow(parentIndex);
			}
		}
		#endregion
		#region column chooser
#if SL
		Decorator IPopupContainer.PopupContainer { get; set; }
#endif
		protected void OnIsColumnChooserVisibleChanged() {
			if(OriginationView != null)
				return;
			if(IsColumnChooserVisible) {
				RootView.UpdateAllOriginationViews(view => {
					if(view != this)
						view.HideColumnChooser();
				});
				RootView.ForceCreateColumnChooserStateInternal();
				ActualColumnChooser.ApplyState(RootView.ColumnChooserState);
				ActualColumnChooser.Show();
				RaiseShownColumnChooser(new RoutedEventArgs(ShownColumnChooserEvent));
			}
			else {
				ActualColumnChooser.Hide();
				ActualColumnChooser.SaveState(RootView.ColumnChooserState);
				RaiseHiddenColumnChooser(new RoutedEventArgs(HiddenColumnChooserEvent));
			}
		}
		void OnActiveEditorChanged() {
			if(CurrentCellEditor as CellEditorBase != null)
				((CellEditorBase)CurrentCellEditor).CellData.UpdateSelectionState();
		}
		protected void ForceCreateColumnChooserStateInternal() {
			if(ColumnChooserState == null) {
				applyColumnChooserStateLocker.Lock();
				ColumnChooserState = ForceCreateColumnChooserState();
				applyColumnChooserStateLocker.Unlock();
			}
		}
		protected virtual DefaultColumnChooserState ForceCreateColumnChooserState() {
			return new DefaultColumnChooserState();
		}
		public void ShowColumnChooser() {
			IsColumnChooserVisible = true;
		}
		public void HideColumnChooser() {
			IsColumnChooserVisible = false;
		}
		protected virtual void RaiseShownColumnChooser(RoutedEventArgs e) {
			RaiseEvent(e);
		}
		protected virtual void RaiseHiddenColumnChooser(RoutedEventArgs e) {
			RaiseEvent(e);
		}
		#endregion
		#region row hit test API
		public int GetRowHandleByTreeElement(DependencyObject d) {
			DependencyObject parentRow = FindParentRow(d);
			if(parentRow == null) return GridDataController.InvalidRow;
			RowHandle rowHandle = GetRowHandle(parentRow);
			return rowHandle.Value;
		}
		public int GetRowHandleByMouseEventArgs(MouseEventArgs e) {
			return GetRowHandleByTreeElement((DependencyObject)e.OriginalSource);
		}
		public FrameworkElement GetRowElementByTreeElement(DependencyObject d) {
			return FindParentRow(d) as FrameworkElement;
		}
		public FrameworkElement GetRowElementByMouseEventArgs(MouseEventArgs e) {
			return GetRowElementByTreeElement((DependencyObject)e.OriginalSource);
		}
		public FrameworkElement GetRowElementByRowHandle(int rowHandle) {
			DataRowNode node;
			Nodes.TryGetValue(rowHandle, out node);
			if(node != null && IsInvisibleGroupRow(node))
				return null;
			if(rowHandle == DataControlBase.AutoFilterRowHandle || (IsNewItemRowHandle(rowHandle) && IsNewItemRowVisible))
				return ViewBehavior.GetAdditionalRowElement(rowHandle);
			RowData rowData = GetRowData(rowHandle);
			if(rowData != null && ((ISupportVisibleIndex)rowData).VisibleIndex >= 0)
				return rowData.RowElement;
			return null;
		}
		internal virtual FrameworkElement GetRowVisibleElement(RowDataBase rowData) {
			return rowData.WholeRowElement;
		}
		internal virtual bool IsInvisibleGroupRow(RowNode node) {
			return false;
		}
		public ColumnBase GetColumnByTreeElement(DependencyObject d) {
			IGridCellEditorOwner cellControl = GetCellElementByTreeElement(d) as IGridCellEditorOwner;
			return cellControl != null ? cellControl.AssociatedColumn : null;
		}
		public ColumnBase GetColumnByMouseEventArgs(MouseEventArgs e) {
			return GetColumnByTreeElement((DependencyObject)e.OriginalSource);
		}
		public FrameworkElement GetCellElementByTreeElement(DependencyObject d) {
			return (FrameworkElement)LayoutHelper.FindLayoutOrVisualParentObject<IGridCellEditorOwner>(d);
		}
		public FrameworkElement GetCellElementByMouseEventArgs(MouseEventArgs e) {
			return GetCellElementByTreeElement((DependencyObject)e.OriginalSource);
		}
		public FrameworkElement GetCellElementByRowHandleAndColumn(int rowHandle, ColumnBase column) {
			return GetCellElementByRowHandleAndColumnCore(rowHandle, column);
		}
		#endregion
		#region commands
		internal DXDialog SummaryEditorContainer = null;
		public void ShowTotalSummaryEditor(ColumnBase column) {
			GridTotalSummaryHelper helper = new GridTotalSummaryHelper(this, () => column);
			helper.ShowSummaryEditor();
		}
		public void ShowFixedTotalSummaryEditor() {
			GridTotalSummaryPanelHelper helper = new GridTotalSummaryPanelHelper(this);
			helper.ShowSummaryEditor();
		}
		internal void ShowTotalSummaryEditor(object parameter) {
			ColumnBase column = GetColumnByCommandParameter(parameter);
			if(column != null)
				ShowTotalSummaryEditor(column);
		}
		internal bool CanShowTotalSummaryEditor(object parameter) {
			ColumnBase column = GetColumnByCommandParameter(parameter);
			return column != null;
		}
		bool CheckNavigationIndex() {
			if(NavigationStyle != GridViewNavigationStyle.Cell)
				return true;
			return NavigationIndex != Constants.InvalidNavigationIndex;
		}
		bool CheckNavigationIndexAndFocusedRowHandle() {
			return CheckNavigationIndex() && !IsInvalidFocusedRowHandle;
		}
		protected virtual bool IsFirstNewRow() {
			return false;
		}
		protected internal bool CanMovePrevRow() {
			if(FocusedRowHandle == DataControlBase.AutoFilterRowHandle)
				return false;
			if(IsFirstNewRow())
				return false;
			return !IsFirstRow && !IsInvalidFocusedRowHandle;
		}
		protected internal bool CanMoveNextRow() {
			return !IsLastRow && !IsInvalidFocusedRowHandle;
		}
		internal bool CanPrevRow() {
			if(IsRootView)
				return CanMovePrevRow();
			return true;
		}
		internal bool CanNextRow() {
			if(IsAdditionalRowFocused)
				if(!IsNewItemRowHandle(FocusedRowHandle) && FocusedRowHandle != DataControlBase.AutoFilterRowHandle)
					return false;
			if(DataControl == null || DataProviderBase==null)
				return false;
			if(DataControl.MasterDetailProvider.FindFirstDetailView(DataProviderBase.CurrentIndex) != null)
				return true;
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControl.DataControlParent.FindNextOuterMasterRow(out targetView, out targetVisibleIndex))
				return true;
			int current = FocusedRowHandle == DataControlBase.InvalidRowHandle ? -1 : DataControl.CurrentIndex;
			if(DataControl.IsLast(current))
				return false;
			if(IsRootView)
				return CanMoveNextRow();
			return true;
		}
		internal bool CanMovePrevCell() {
			return ((!ViewBehavior.NavigationStrategyBase.IsBeginNavigationIndex(this) && CheckNavigationIndex()) || !IsFirstRow) && !IsInvalidFocusedRowHandle;
		}
		internal bool CanMoveNextCell() {
			return ((!ViewBehavior.NavigationStrategyBase.IsEndNavigationIndex(this) && CheckNavigationIndex()) || !IsLastRow) && !IsInvalidFocusedRowHandle;
		}
		internal bool CanMoveFirstRow() {
			return !IsFirstRow && CheckNavigationIndex() && !IsInvalidFocusedRowHandle;
		}
		internal bool CanMoveLastRow() {
			return !IsLastRow && CheckNavigationIndex() && !IsInvalidFocusedRowHandle;
		}
		internal bool CanMovePrevPage() {
			return !IsFirstRow && CheckNavigationIndexAndFocusedRowHandle();
		}
		internal bool CanMoveNextPage() {
			return !IsLastRow && CheckNavigationIndexAndFocusedRowHandle();
		}
		internal bool CanMoveFirstCell() {
			return DataControl != null && !ViewBehavior.NavigationStrategyBase.IsBeginNavigationIndex(this) && CheckNavigationIndexAndFocusedRowHandle() && !DataControl.IsGroupRowHandleCore(FocusedRowHandle);
		}
		internal bool CanMoveLastCell() {
			return (DataControl != null && !ViewBehavior.NavigationStrategyBase.IsEndNavigationIndex(this)) && !IsInvalidFocusedRowHandle && !DataControl.IsGroupRowHandleCore(FocusedRowHandle);
		}
		internal void MoveFirstCell() {
			MoveFirstNavigationIndex();
		}
		internal void MoveLastCell() {
			MoveLastNavigationIndex();
		}
		internal void ClearFilter() {
			DataControl.FilterCriteria = null;
		}
		internal void ChangeColumnsSortOrder(ChangeColumnsSortOrderMode mode) {
			foreach(var column in ColumnsCore.Cast<ColumnBase>().Where(x => {
				if(mode.HasFlag(ChangeColumnsSortOrderMode.NotSortedColumns) && x.SortOrder == ColumnSortOrder.None)
					return true;
				if(mode.HasFlag(ChangeColumnsSortOrderMode.SortedColumns) && x.SortOrder != ColumnSortOrder.None && x.GroupIndexCore < 0)
					return true;
				if(mode.HasFlag(ChangeColumnsSortOrderMode.GroupedColumns) && x.SortOrder != ColumnSortOrder.None && x.GroupIndexCore >= 0)
					return true;
				return false;
			})) {
				OnColumnHeaderClick(column, true, false);
			}
		}
		internal DXDialog FilterControlContainer = null;
		public void ShowFilterEditor(ColumnBase defaultColumn) {
			ShowFilterEditorInternal(DataControl, DataControlBase.FilterCriteriaProperty.GetName(), defaultColumn);
		}
		protected void ShowFilterEditorInternal(object filterCriteriaSource, string filterCriteriaPropertyName, ColumnBase defaultColumn) {
			if(IsFilterControlOpened)
				return;
			FilterControl filterControl = new FilterControl() { ShowBorder = false, SupportDomainDataSource = DataControl.SupportDomainDataSource };
			filterControl.DefaultColumn = DataControl.GetFilterColumnFromGridColumn(defaultColumn);
			filterControl.SourceControl = DataControl.FilteredComponent;
			Binding bindingFilterCriteria = new Binding() { Source = filterCriteriaSource, Path = new PropertyPath(filterCriteriaPropertyName), Mode = BindingMode.TwoWay };
			filterControl.SetBinding(FilterControl.FilterCriteriaProperty, bindingFilterCriteria);
			Binding bindingFilterEditorShowOperandTypeIcon = new Binding() { Source = this, Path = new PropertyPath(FilterEditorShowOperandTypeIconProperty.GetName()) };
			filterControl.SetBinding(FilterControl.ShowOperandTypeIconProperty, bindingFilterEditorShowOperandTypeIcon);
			RoutedEventArgs args = new FilterEditorEventArgs(this, filterControl) { RoutedEvent = FilterEditorCreatedEvent };
			RaiseEventInOriginationView(args);
			if(args.Handled)
				return;
			string titleText = GetLocalizedString(GridControlStringId.FilterEditorTitle);
			filterControl.FlowDirection = FlowDirection;
			FilterControlContainer = ShowDialogContent(filterControl, new Size(500, 350), new FloatingContainerParameters() {
					Title = titleText,
					AllowSizing = true,
					ShowApplyButton = !IsDesignTime,
					CloseOnEscape = false,
#if !SL
					Icon = DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromCoreEmbeddedResource("Editors.Images.FilterControl.filter.png")
#endif
				});
			if(FilterControlContainer != null) {
#if SL
				FilterControlContainer.Closed += filterControlContainer_Closed;
#else
				FilterControlContainer.Hidden += filterControlContainer_Hidden;
#endif
			}
		}
		[Obsolete]
		public void ShowFilterEditor(object filterCriteriaSource, string filterCriteriaPropertyName, ColumnBase defaultColumn) {
			ShowFilterEditorInternal(filterCriteriaSource, filterCriteriaPropertyName, defaultColumn);
		}
		internal DXDialog ShowDialogContent(FrameworkElement dialogContent, Size size, FloatingContainerParameters parameters) {
			var popupBaseEdit = BaseEdit.GetOwnerEdit(this) as PopupBaseEdit;
			bool? staysPopupOpen = false;
			popupBaseEdit.Do(edit => {
				staysPopupOpen = edit.StaysPopupOpen;
				edit.StaysPopupOpen = true; 
			});
			var floatingContainer = FloatingContainer.ShowDialogContent(dialogContent, RootView, size, parameters);
			RoutedEventHandler floatingContainerHidden = null;
			floatingContainerHidden = (d, e) => {
				floatingContainer.Hidden -= floatingContainerHidden;
				DataControl.Focus();
				popupBaseEdit.Do(edit => edit.StaysPopupOpen = staysPopupOpen);
			};
			if(floatingContainer != null)
				floatingContainer.Hidden += floatingContainerHidden;
			return floatingContainer;
		}
		internal bool IsFilterControlOpened {
			get { return FilterControlContainer != null; }
		}
		void filterControlContainer_Hidden(object sender, RoutedEventArgs e) {
			FilterControlContainer.Hidden -= filterControlContainer_Hidden;
			FilterControlContainer = null;
		}
		internal void ShowFilterEditor(object commandParameter) {
			ColumnBase defaultColumn = (ColumnBase)GetColumnByCommandParameter(commandParameter);
			ShowFilterEditor(defaultColumn);
		}
		internal void DeleteFocusedRow() {
			if(!CanDeleteFocusedRow())
				return;
			DeleteRowCore(FocusedRowHandle);
		}
		internal void DeleteRowCore(int rowHandle) {
			if(IsLastRow)
				MovePrevRow();
			DataProviderBase.DeleteRow(new RowHandle(rowHandle));
			if(DataControl.VisibleRowCount == 0)
				SetFocusedRowHandle(DataControlBase.InvalidRowHandle);
		}
		internal void EditFocusedRow() {
			if(CanEditFocusedRow())
				ShowEditor();
		}
		internal void EndEditFocusedRow() {
			if(CanEndEditFocusedRow())
				CommitEditing();
		}
		internal void CancelEditFocusedRow() {
			if(!CanCancelEditFocusedRow())
				return;
			CancelRowEdit();
		}
		internal bool CanClearFilter() {
			return DataControl != null && !object.ReferenceEquals(DataControl.FilterCriteria, null);
		}
		internal bool CanShowFilterEditor(object commandParameter) {
			bool allowFiltering = true;
			if(commandParameter is ColumnBase)
				allowFiltering = (commandParameter as ColumnBase).ActualAllowColumnFiltering;
			return ((DataControl != null) && allowFiltering && ShowEditFilterButton);
		}
		internal bool CanShowColumnChooser() {
			return !IsColumnChooserVisible;
		}
		internal bool CanHideColumnChooser() {
			return IsColumnChooserVisible;
		}
		protected bool IsAddDeleteInSource() {
			return DataProviderBase != null && (DataProviderBase.DataSource is IList || DataProviderBase.DataSource is IEditableCollectionView || DataProviderBase.DataSource is IListSource);			
		}
		protected internal virtual bool CanEndEditFocusedRow() {
			return !IsAutoFilterRowFocused && IsEditing;
		}
		protected internal virtual bool CanCancelEditFocusedRow() {
			return !IsAutoFilterRowFocused && (DataProviderBase.IsCurrentRowEditing || ActiveEditor != null);
		}
		protected internal virtual bool CanDeleteFocusedRow() {
			return DataProviderBase!= null && !IsAutoFilterRowFocused && !IsEditing && !IsInvalidFocusedRowHandle && IsFocusedView && DataProviderBase.DataRowCount > 0 && IsAddDeleteInSource();
		}
		protected internal virtual bool CanAddNewRow() {
			return !IsEditing && IsAddDeleteInSource();
		}
		protected internal virtual bool CanEditFocusedRow() {
			if(IsAutoFilterRowFocused || NavigationStyle != GridViewNavigationStyle.Cell || DataControl ==null || DataControl.CurrentColumn == null)
				return false;
			return ActiveEditor == null && CurrentCellEditor != null && CurrentCellEditor.CanShowEditor();
		}
		void OnCanEndEditFocusedRow(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanEndEditFocusedRow();
		}
		void OnCanCancelEditFocusedRow(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanCancelEditFocusedRow();
		}
		#endregion
		#region editing API
		public void ShowEditor() {
			ShowEditor(false);
		}
		public void ShowEditor(bool selectAll) {
			if(CurrentCellEditor != null)
				CurrentCellEditor.ShowEditorIfNotVisible(selectAll);
		}
		public void HideEditor() {
			HideEditor(true);
		}
		internal void HideEditor(bool closeEditor) {
			if(CurrentCellEditor != null) {
				if(closeEditor)
					CurrentCellEditor.CancelEditInVisibleEditor();
				else
					CurrentCellEditor.HideEditor(false);
			}
		}
		public void CloseEditor() {
			CloseEditor(true);
		}
		internal void CloseEditor(bool closeEditor, bool cleanError = false) {
			if(CurrentCellEditor != null) {
				CurrentCellEditor.CommitEditor(closeEditor);
				if(cleanError) {
					CurrentCellEditor.ClearError();
					if(this.HasCellEditorError)
						CurrentCellEditor.CancelEditInVisibleEditor();
				}
			}
		}
		public void ValidateEditor() {
			if(CurrentCellEditor != null)
				CurrentCellEditor.ValidateEditor();
		}
		public void PostEditor() {
			if(CurrentCellEditor != null)
				CurrentCellEditor.PostEditor();
		}
		public bool CommitEditing() {
			return CommitEditing(false);
		}
		internal void CommitAndCleanEditor() {
			CommitEditing(true, true);
		}
		public bool CommitEditing(bool forceCommit, bool cleanError = false) {
			return EnumerateViewsForCommitEditingAndRequestUIUpdate(view => view.CommitEditingCore(forceCommit, cleanError));
		}
		private bool CommitEditingCore(bool forceCommit, bool cleanError = false) {
			bool result = RequestUIUpdateCore(cleanError);
			if(!result && !forceCommit)
				return result;
			if(!result)
				HideEditor(false);
			if(!AutoScrollOnSorting)
				ScrollIntoViewLocker.Lock();
			CommitEditingLocker.DoLockedAction(() => {
				result = result && ViewBehavior.EndRowEdit();
			});
			if(!AutoScrollOnSorting)
				ScrollIntoViewLocker.Unlock();
			if(!result)
				return false;
			CancelRoutedEventArgs args = new GridCancelRoutedEventArgs(DataControl, BeforeLayoutRefreshEvent);
			RaiseBeforeLayoutRefresh(args);
			return !args.Cancel;
		}
		protected virtual void RaiseBeforeLayoutRefresh(CancelRoutedEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		protected internal virtual void RaiseCustomScrollAnimation(CustomScrollAnimationEventArgs e) {
			RaiseEvent(e);
		}
		public void CancelRowEdit() {
			if(CurrentCellEditor != null)
				CurrentCellEditor.CancelEditInVisibleEditor();
			if(DataProviderBase.IsCurrentRowEditing)
				DataProviderBase.CancelCurrentRowEdit();
			ViewBehavior.OnCancelRowEdit();
			DataControl.SetRowStateError(FocusedRowHandle, null);
		}
		#endregion
		#region IFixedExtentProvider Members
		internal double FixedExtent { get { return ViewBehavior.GetFixedExtent(); } }
		internal double FixedViewport { get { return ViewBehavior.HorizontalViewportCore; } }
		#endregion
		#region unbound expression editor
		internal DXDialog UnboundExpressionEditorContainer = null;
		public virtual void ShowUnboundExpressionEditor(ColumnBase column) {
			ExpressionEditorControl expressionEditorControl = new ExpressionEditorControl(column);
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true)
					column.UnboundExpression = expressionEditorControl.Expression;
			};
			RoutedEventArgs args = new UnboundExpressionEditorEventArgs(expressionEditorControl, column) { RoutedEvent = UnboundExpressionEditorCreatedEvent };
			RaiseEventInOriginationView(args);
			if(args.Handled)
				return;
			UnboundExpressionEditorContainer = ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, RootView, closedHandler);
		}
		internal void ShowUnboundExpressionEditor(object commandParameter) {
			ColumnBase column = GetColumnByCommandParameter(commandParameter);
			if(CanShowUnboundExpressionEditor(column))
				ShowUnboundExpressionEditor(column);
		}
		internal bool CanShowUnboundExpressionEditor(object commandParameter) {
			ColumnBase column = GetColumnByCommandParameter(commandParameter);
			return column != null && column.AllowUnboundExpressionEditor;
		}
		#endregion
		#region IColumnOwnerBase Members
		bool IColumnOwnerBase.AllowGrouping { get { return AllowGroupingCore; } }
		internal virtual bool AllowGroupingCore { get { return false; } }
		ActualTemplateSelectorWrapper IColumnOwnerBase.ActualGroupValueTemplateSelector { get { return ActualGroupValueTemplateSelectorCore; } }
		internal virtual ActualTemplateSelectorWrapper ActualGroupValueTemplateSelectorCore { get { return null; } }
		Style IColumnOwnerBase.AutoFilterRowCellStyle { get { return ViewBehavior.AutoFilterRowCellStyle; } }
		Style IColumnOwnerBase.NewItemRowCellStyle { get { return GetNewItemRowCellStyle; } }
		internal virtual Style GetNewItemRowCellStyle { get { return null; } }
		bool IColumnOwnerBase.AllowColumnsResizing { get { return ViewBehavior.AllowColumnResizingCore; } }
		void IColumnOwnerBase.RebuildVisibleColumns() {
			RebuildVisibleColumns();
		}
		void IColumnOwnerBase.RebuildColumnChooserColumns() {
			RebuildColumnChooserColumns();
		}
		void IColumnOwnerBase.CalcColumnsLayout() {
			if(IsLockUpdateColumnsLayout)
				return;
			RebuildVisibleColumns();
			UpdateColumnsViewInfo();
		}
		IList<DevExpress.Xpf.Grid.SummaryItemBase> IColumnOwnerBase.GetTotalSummaryItems(ColumnBase column) {
			return column.TotalSummariesCore;
		}
		object IColumnOwnerBase.GetTotalSummaryValue(DevExpress.Xpf.Grid.SummaryItemBase item) {
			return DataControl.GetTotalSummaryValue(item);
		}
		HorizontalAlignment IColumnOwnerBase.GetDefaultColumnAlignment(ColumnBase column) {
			return DefaultColumnAlignmentHelper.IsColumnFarAlignedByDefault(GetColumnType(column)) ? HorizontalAlignment.Right : HorizontalAlignment.Left;
		}
		BaseEditSettings IColumnOwnerBase.CreateDefaultEditSettings(IDataColumnInfo column) {
			return GridColumnTypeHelper.CreateEditSettings(column.FieldType);
		}
		bool IColumnOwnerBase.AllowSortColumn(ColumnBase column) {
			if(column == null)
				return false;
			return CanSortColumn(column, column.FieldName);
		}
		protected internal virtual bool CanSortColumn(string fieldName) {
			return CanSortColumnCore(ColumnsCore[fieldName], fieldName, true);
		}
		protected internal virtual bool CanSortColumn(ColumnBase column, string fieldName) {
			return CanSortColumnCore(column, fieldName, false);
		}
		protected internal virtual bool CanSortColumnCore(ColumnBase column, string fieldName, bool prohibitColumnProperty) {
			if(!DataControl.DataProviderBase.CanColumnSortCore(fieldName))
				return false;
			if(column == null || column.AllowSorting == DefaultBoolean.Default) {
				DataColumnInfo columnInfo = DataProviderBase.Columns[fieldName];
				return columnInfo != null && CanSortDataColumnInfo(columnInfo);
			}
			return column != null && (prohibitColumnProperty || column.AllowSorting != DefaultBoolean.False);
		}
		protected virtual bool CanSortDataColumnInfo(DataColumnInfo columnInfo) {
			return columnInfo.AllowSort;
		}
		void IColumnOwnerBase.UpdateCellDataValues() {
			UpdateRowData((rowData) => rowData.UpdateCellDataValues());
		}
		void IColumnOwnerBase.ClearBindingValues(ColumnBase column) {
			UpdateRowData((rowData) => rowData.ClearBindingValues(column));
		}
		bool IColumnOwnerBase.AllowSorting { get { return AllowSorting && !IsEditFormVisible; ; } }
		bool IColumnOwnerBase.AllowColumnMoving { get { return AllowColumnMoving; } }
		bool IColumnOwnerBase.AllowEditing { get { return AllowEditing; } }
		bool IColumnOwnerBase.AllowColumnFiltering { get { return AllowColumnFiltering && !IsEditFormVisible; } }
		bool IColumnOwnerBase.AllowResizing { get { return ViewBehavior.AllowResizingCore; } }
		bool IColumnOwnerBase.UpdateAllowResizingOnWidthChanging { get { return ViewBehavior.UpdateAllowResizingOnWidthChanging; } }
		bool IColumnOwnerBase.AutoWidth { get { return ViewBehavior.AutoWidthCore; } }
		DataTemplate IColumnOwnerBase.GetActualCellTemplate() {
			return CellTemplate;
		}
		ColumnBase IColumnOwnerBase.GetColumn(string fieldName) {
			return ColumnsCore[fieldName];
		}
		void IColumnOwnerBase.UpdateContentLayout() {
			UpdateContentLayout();
		}
		protected internal void UpdateContentLayout() {
			if(DataControl != null)
				((INotificationManager)DataControl).AcceptNotification(this, NotificationType.Layout);
		}
		IList<ColumnBase> IColumnOwnerBase.VisibleColumns { get { return VisibleColumnsCore; } }
		void IColumnOwnerBase.ApplyColumnVisibleIndex(BaseColumn column) {
			ApplyColumnVisibleIndex(column);
		}
		void IColumnOwnerBase.ChangeColumnSortOrder(ColumnBase column) {
			OnColumnHeaderClick(column);
		}
		void IColumnOwnerBase.ClearColumnFilter(ColumnBase column) {
			DataControl.ClearColumnFilter(column);
		}
		bool IColumnOwnerBase.CanClearColumnFilter(ColumnBase column) {
			return CanClearColumnFilter(column);
		}
		Type IColumnOwnerBase.GetColumnType(ColumnBase column, DataProviderBase dataProvider) {
			return GetColumnType(column, dataProvider);
		}
		bool IColumnOwnerBase.LockEditorClose { get { return LockEditorClose; } set { LockEditorClose = value; } }
		void IColumnOwnerBase.UpdateShowEditFilterButton(DefaultBoolean newAllowColumnFiltering, DefaultBoolean oldAllowColumnFiltering) {
			if(DataControl != null) {
				DataControl.UpdateColumnFilteringCounters(newAllowColumnFiltering, oldAllowColumnFiltering);
				UpdateShowEditFilterButtonCore();
			}
		}
		protected internal virtual void UpdateActualAllowCellMergeCore() {
		}
		internal void UpdateColumnsActualCellTemplateSelector() {
			if(DataControl == null)
				return;
			foreach(ColumnBase column in DataControl.ColumnsCore) {
				column.UpdateActualCellTemplateSelector();
			}
		}
		internal bool CanClearColumnFilter(object commandParameter) {
			if(commandParameter is ColumnBase)
				return ((DataControl != null) && (commandParameter as ColumnBase).IsFiltered);
			return false;
		}
		void IColumnOwnerBase.GroupColumn(string fieldName, int index, ColumnSortOrder sortOrder) {
			GroupColumn(fieldName, index, sortOrder);
		}
		void IColumnOwnerBase.UngroupColumn(string fieldName) {
			UngroupColumn(fieldName);
		}
		protected virtual void GroupColumn(string fieldName, int index, ColumnSortOrder sortOrder) { }
		protected virtual void UngroupColumn(string fieldName) { }
		bool IColumnOwnerBase.ShowAllTableValuesInFilterPopup { get { return DataControl == null ? false : DataControl.ShowAllTableValuesInFilterPopup; } }
		bool IColumnOwnerBase.ShowAllTableValuesInCheckedFilterPopup { get { return DataControl == null ? true : DataControl.ShowAllTableValuesInCheckedFilterPopup; } }
		Style IColumnOwnerBase.GetActualCellStyle(ColumnBase column) {
			return ViewBehavior.GetActualCellStyle(column);
		}
		#endregion
		#region MasterDetail
		internal virtual bool AllowMasterDetailCore { get { return false; } }
		internal DataViewBase OriginationView { get; private set; }
		internal DataViewBase EventTargetView { get { return OriginationView ?? this; } }
		internal DataViewBase RootView { get { return DataControl != null ? DataControl.GetRootDataControl().DataView : this; } }
		internal bool HasClonedDetails { get { return !IsRootView && DataControl != null && DataControl.DetailClones.Any(); } }
		internal bool HasClonedExpandedDetails { 
			get {
				if(IsRootView || DataControl == null)
					return false;
				bool result = false;
				DataControlOriginationElementHelper.EnumerateDependentElemets<DataControlBase>(DataControl, grid => grid, grid => result = true);
				return result;
			} 
		}
		public bool IsRootView { get { return RootView == this; } }
		public DataViewBase FocusedView { get { return MasterRootRowsContainer.FocusedView; } }
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(DataControl != null) {
				DataControl.GetDataControlOriginationElement().NotifyPropertyChanged(DataControl, e.Property, dataControl => dataControl.DataView, typeof(DataViewBase));
			}
		}
		object IConvertClonePropertyValue.ConvertClonePropertyValue(string propertyName, object sourceValue, DependencyObject destinationObject) {
			if(propertyName != "FocusedColumn")
				return sourceValue;
			if(sourceValue == null)
				return null;
			ColumnBase sourceColumn = (ColumnBase)sourceValue;
			DataViewBase destinationView = (DataViewBase)destinationObject;
			if(destinationView.DataControl == null)
				return null;
			return CloneDetailHelper.SafeGetDependentCollectionItem<ColumnBase>(sourceColumn, sourceColumn.View.ColumnsCore, destinationView.ColumnsCore);
		}
		internal void UpdateAllOriginationViews(Action<DataViewBase> updateMethod) {
			UpdateAllDependentViewsCore(updateMethod,
				method => DataControl.UpdateAllOriginationDataControls(dataControl => {
					if(dataControl.DataView != null)
						method(dataControl.DataView);
					})
			);
		}
		internal void UpdateAllDependentViews(Action<DataViewBase> updateMethod) {
			UpdateAllDependentViewsCore(updateMethod,
				method => DataControlOriginationElementHelper.EnumerateDependentElemetsIncludingSource<DataViewBase>(DataControl, dataControl => dataControl.DataView, method)
			);
		}
		void UpdateAllDependentViewsCore(Action<DataViewBase> updateMethod, Action<Action<DataViewBase>> iterateMethod) {
			if(DataControl == null) {
				updateMethod(this);
				return;
			}
			iterateMethod(updateMethod);
		}
		internal virtual void UpdateMasterDetailViewProperties() {
			ViewBehavior.UpdateActualProperties();
		}
		protected void InvalidateParentTree() {
			if(DataControl != null) {
				DataControl.DataControlParent.InvalidateTree();
			}
		}
		internal void FinalizeClonedDetail() {
			SelectionStrategy = null;
			Navigation = null;
		}
		internal void UpdateActualTemplateSelector(DependencyPropertyKey propertyKey, DataTemplateSelector selector, DataTemplate template, Func<DataTemplateSelector, DataTemplate, ActualTemplateSelectorWrapper> createWrapper = null) {
			DataControlOriginationElementHelper.UpdateActualTemplateSelector(this, OriginationView, propertyKey, selector, template, createWrapper);
		}
		protected internal virtual void UpdateUseLightweightTemplates() { }
		protected void RaiseEventInOriginationView(RoutedEventArgs e) {
			EventTargetView.RaiseEvent(e);
		}
		internal virtual void BindDetailContainerNextLevelItemsControl(ItemsControl itemsControl) { }
		internal void UpdateColumnChooserCaption() {
			if(!IsActualColumnChooserCreated)
				return;
			DefaultColumnChooser chooser = ActualColumnChooser as DefaultColumnChooser;
			if(chooser != null)
				chooser.UpdateCaption();
		}
		internal virtual void ThrowNotSupportedInMasterDetailException() {
			throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.OnlyTableViewSupported);
		}
		internal virtual void ThrowNotSupportedInDetailException() { }
		#endregion
		#region TotalSummaryPanel
		public static readonly DependencyProperty ShowFixedTotalSummaryProperty;   
		IList<GridTotalSummaryData> fixedSummariesLeft = new ObservableCollection<GridTotalSummaryData>();
		IList<GridTotalSummaryData> fixedSummariesRight = new ObservableCollection<GridTotalSummaryData>();
		FixedSummariesHelper fixedSummariesHelper = new FixedSummariesHelper();
		void UpdateTotalSummaries() {
			if(dataControl == null)
				return;
			FixedTotalSummaryHelper.GenerateTotalSummaries(fixedSummariesHelper.FixedSummariesLeftCore, ColumnsCore, dataControl.GetTotalSummaryValue, FixedSummariesLeft);
			FixedTotalSummaryHelper.GenerateTotalSummaries(fixedSummariesHelper.FixedSummariesRightCore, ColumnsCore, dataControl.GetTotalSummaryValue, FixedSummariesRight);
		}
		internal string GetFixedSummariesLeftString() {
			return FixedTotalSummaryHelper.GetFixedSummariesString(FixedSummariesLeft);
		}
		internal string GetFixedSummariesRightString() {
			return FixedTotalSummaryHelper.GetFixedSummariesString(FixedSummariesRight);
		}
		#endregion
		#region SearchPanel
		public static readonly DependencyProperty SearchPanelFindFilterProperty;
		public static readonly DependencyProperty SearchPanelHighlightResultsProperty;
		public static readonly DependencyProperty SearchStringProperty;
		public static readonly DependencyProperty ShowSearchPanelCloseButtonProperty;
		public static readonly DependencyProperty SearchPanelFindModeProperty;
		public static readonly DependencyProperty ShowSearchPanelMRUButtonProperty;
		public static readonly DependencyProperty SearchPanelAllowFilterProperty;
		public static readonly DependencyProperty SearchPanelCriteriaOperatorTypeProperty;
		public static readonly DependencyProperty SearchColumnsProperty;
		public static readonly DependencyProperty ShowSearchPanelFindButtonProperty;
		public static readonly DependencyProperty SearchPanelClearOnCloseProperty;
		public static readonly DependencyProperty ShowSearchPanelModeProperty;
		public static readonly DependencyProperty ActualShowSearchPanelProperty;
		static readonly DependencyPropertyKey ActualShowSearchPanelPropertyKey;
		public static readonly DependencyProperty SearchDelayProperty;
		public static readonly DependencyProperty SearchPanelImmediateMRUPopupProperty;
		public static readonly DependencyProperty SearchPanelHorizontalAlignmentProperty;
		public static readonly DependencyProperty SearchControlProperty;
		public static readonly DependencyProperty SearchPanelNullTextProperty;
		internal GridControlColumnProviderBase SearchPanelColumnProvider { get { return GridControlColumnProviderBase.GetColumnProvider(DataControl); } }
		void UpdateSearchPanelColumnProviderBindings() {
			if(DataControl == null || SearchControl == null)
				return;
			if(SearchPanelColumnProvider == null)
				UpdateGridControlColumnProvider();
			if(SearchPanelColumnProvider.DataControlBase != null && SearchPanelColumnProvider.DataControlBase != DataControl)
				SearchPanelColumnProvider.DataControlBase = DataControl;
			Binding bindingFilterByColumnsMode = new Binding() { Source = SearchPanelColumnProvider, Path = new PropertyPath(GridControlColumnProviderBase.FilterByColumnsModeProperty.GetName()), Mode = BindingMode.OneWay };
			SearchControl.SetBinding(SearchControl.FilterByColumnsModeProperty, bindingFilterByColumnsMode);
			SearchControl.ColumnProvider = SearchPanelColumnProvider;
			Binding bindingAllowTextHighlighting = new Binding() { Source = this, Path = new PropertyPath(DataViewBase.SearchPanelHighlightResultsProperty.GetName()), Mode = BindingMode.OneWay };
			SearchPanelColumnProvider.SetBinding(GridControlColumnProviderBase.AllowTextHighlightingProperty, bindingAllowTextHighlighting);
			Binding bindingAllowExtraFilter = new Binding() { Source = this, Path = new PropertyPath(DataViewBase.SearchPanelAllowFilterProperty.GetName()), Mode = BindingMode.OneWay };
			SearchPanelColumnProvider.SetBinding(GridControlColumnProviderBase.AllowGridExtraFilterProperty, bindingAllowExtraFilter);
			ApplySearchColumns();
		}
		protected virtual void UpdateGridControlColumnProvider() {
			if(DataControl != null) {
				GridControlColumnProviderBase columnProvider = new GridControlColumnProviderBase() { FilterByColumnsMode = FilterByColumnsMode.Default, AllowColumnsHighlighting = false };
				GridControlColumnProviderBase.SetColumnProvider(DataControl, columnProvider);
			}
		}
		void UpdateSearchPanelText() {
			UpdateSearchPanelVisibility();
			if(SearchControl == null && ShowSearchPanelMode == Grid.ShowSearchPanelMode.Never)
				SearchControl = new GridSearchControlBase() { View = this };
		}
		void UpdateSearchPanelVisibility() {
			switch(ShowSearchPanelMode){
				case ShowSearchPanelMode.Never:
					ActualShowSearchPanel = false;
					break;
				case ShowSearchPanelMode.Always:
					ActualShowSearchPanel = true;
					break;
				case ShowSearchPanelMode.Default:
					if(!ActualShowSearchPanel)
						ActualShowSearchPanel = !String.IsNullOrEmpty(SearchString);
					break;
			}
		}
		public void ShowSearchPanel(bool moveFocusInSearchPanel){
			if(ShowSearchPanelMode == Grid.ShowSearchPanelMode.Never)
				return;
			if(ShowSearchPanelMode == ShowSearchPanelMode.Default && !ActualShowSearchPanel)
				ActualShowSearchPanel = true;
			if(moveFocusInSearchPanel)
				FocusSearchPanel();
		}
		protected internal bool ConvertCommandParameterToBool(object commandParameter) {
			bool result = true;
			try {
				result = Convert.ToBoolean(commandParameter);
			}
			catch { }
			return result;
		}
		void OnActualShowSearchPanelChanged() {
			if(ActualShowSearchPanel)
				ShowSearchPanel(false);
			else
				HideSearchPanel();
		}
		void SearchColumnsChanged(string columnsString) {
			ApplySearchColumns();
		}
		internal void ApplySearchColumns() {
			if(String.IsNullOrEmpty(SearchColumns) || SearchPanelColumnProvider == null)
				return;
			List<ColumnBase> columns = new List<ColumnBase>();
			foreach(ColumnBase column in ColumnsCore) {
				columns.Add(column);
			}
			ObservableCollection<string> customSearchColumns = GridColumnListParser.GetSearchColumns(columns, SearchColumns);
			if(customSearchColumns != null) {
				SearchPanelColumnProvider.FilterByColumnsMode = FilterByColumnsMode.Custom;
				SearchPanelColumnProvider.CustomColumns = customSearchColumns;
			}
			else {
				SearchPanelColumnProvider.FilterByColumnsMode = FilterByColumnsMode.Default;
			}
			if(SearchControl == null)
				SearchPanelColumnProvider.UpdateColumns();
			else
				SearchControl.UpdateColumnProvider();
		}
		public void HideSearchPanel() {
			if(ShowSearchPanelMode != ShowSearchPanelMode.Default)
				return;
			ActualShowSearchPanel = false;
		}
		internal void UpdateColumnAllowSearchPanel(BaseColumn columnBase) {
			if(SearchPanelColumnProvider != null && SearchControl != null)
				SearchControl.UpdateColumnProvider();
		}
		internal bool IsKeyboardFocusInSearchPanel() {
			return isKeyboardFocusInSearchPanel;
		}
		internal bool IsKeyboardFocusInHeadersPanel() {
			return HeadersPanel != null && HeadersPanel.GetIsKeyboardFocusWithin();
		}
		bool isKeyboardFocusInSearchPanel = false;
		bool postponedSearchControlFocus = false;
		internal bool PostponedSearchControlFocus { 
			get { return postponedSearchControlFocus; } 
			set { postponedSearchControlFocus = value; } 
		}
		internal void SetSearchPanelFocus(bool isKeyboardFocusInSearchPanel = false) {
			this.isKeyboardFocusInSearchPanel = isKeyboardFocusInSearchPanel;
		}
		protected void SearchControlKeyDownProcessing(KeyEventArgs e) {
			if(!IsRootView && RootView != null) {
				RootView.SearchControlKeyDownProcessing(e);
				return;
			}
			if(e.Key == Key.F && ModifierKeysHelper.IsOnlyCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				if(ShowSearchPanelMode != ShowSearchPanelMode.Never && !ActualShowSearchPanel) {
					FocusSearchPanel();
					ActualShowSearchPanel = true;
				}
				FocusSearchPanel();
			}
			SearchControlKeyButtonUpProcessing(e);
		}
		protected virtual void SearchControlKeyButtonUpProcessing(KeyEventArgs e) { }
		protected void FocusSearchPanel() {
			if(SearchControl == null) {
				if(ShowSearchPanelMode != ShowSearchPanelMode.Never)
					PostponedSearchControlFocus = true;
				return;
			}
			if(!SearchControl.GetIsKeyboardFocusWithin())
				SearchControl.Focus();
		}
		IList<string> searchControlMru = null;
		internal IList<string> SearchControlMru { 
			get { return searchControlMru; }
			set { searchControlMru = value; }
		}
		protected virtual void OnSearchControlChanged(SearchControl oldValue, SearchControl newValue) {
			UpdateSearchControlMRU(oldValue, newValue);
			UpdateSearchPanelColumnProviderBindings();
		}
		void UpdateSearchControlMRU(SearchControl oldValue, SearchControl newValue) {
			if(oldValue == null && newValue == null)
				return;
			if(oldValue != null)
				searchControlMru = oldValue.MRU;
			if(newValue != null && searchControlMru != null)
				newValue.MRU = searchControlMru;
		}
		protected internal TextHighlightingProperties GetTextHighlightingProperties(ColumnBase column) {
			if(SearchPanelColumnProvider == null)
				return null;
			return SearchPanelColumnProvider.GetTextHighlightingProperties(column);
		}
		#endregion
		#region ShowErrorInfoError
		void OnItemsSourceErrorInfoShowModeChanged() {
			UpdateCellDataErrors();
		}
		#endregion
		#region Serialization
		void OnDeserializeAllowPropertyInternal(AllowPropertyEventArgs e) {
			e.Allow = OnDeserializeAllowProperty(e);
		}
		protected virtual bool OnDeserializeAllowProperty(AllowPropertyEventArgs e) {
			return DataControl != null ? DataControl.OnDeserializeAllowProperty(e) : false;
		}
		protected virtual void OnSerializeStart() {
			ForceCreateColumnChooserStateInternal();
			bool isActualColumnChooserCreated = IsActualColumnChooserCreated;
			ActualColumnChooser.SaveState(ColumnChooserState);
			if(!isActualColumnChooserCreated)
				ActualColumnChooser = null;
		}
		bool columnChooserStateDeserialized;
		protected virtual void OnDeserializeStart(StartDeserializingEventArgs e) {
			ForceCreateColumnChooserStateInternal();
			columnChooserStateDeserialized = false;
		}
		protected virtual void OnDeserializeEnd(EndDeserializingEventArgs e) {
			if(columnChooserStateDeserialized) {
				bool isActualColumnChooserCreated = IsActualColumnChooserCreated;
				ActualColumnChooser.ApplyState(ColumnChooserState);
				if(!isActualColumnChooserCreated)
					ActualColumnChooser = null;
			}
		}
		void OnDeserializeProperty(XtraPropertyInfoEventArgs e) {
			if(e.DependencyProperty == ColumnChooserStateProperty) {
				columnChooserStateDeserialized = true;
				return;
			}
			if(e.DependencyProperty == ActualShowSearchPanelProperty) {
				e.Handled = true;
				ActualShowSearchPanel = Convert.ToBoolean(e.Info.Value);
				return;
			}
		}
		protected virtual void OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) {
#pragma warning disable 618
			if(e.DependencyProperty == AllowMovingProperty) {
				e.CustomShouldSerialize = false;
			}
#pragma warning restore 618
		}
		#endregion
		#region Automation
#if DEBUGTEST
		internal static EventLog AutomationEventLog = new EventLog();
#endif
		DependencyObject previousAutomationObject;
		protected bool CanGetAutomationPeer(DependencyObject obj) {
			return obj != null && DataControl.AutomationPeer != null && previousAutomationObject != obj;
		}
		protected virtual AutomationPeer GetAutomationPeer(DependencyObject obj) {
			if(!CanGetAutomationPeer(obj)) return null;
			previousAutomationObject = obj;
			if(NavigationStyle == GridViewNavigationStyle.Row)
				return DataControl.AutomationPeer.GetRowPeer(FocusedRowHandle);
			if(NavigationStyle == GridViewNavigationStyle.Cell && dataControl.CurrentColumn != null)
				return DataControl.AutomationPeer.GetCellPeer(FocusedRowHandle, DataControl.CurrentColumn);
			return null;
		}
		void RaiseItemSelectAutomationEvents(DependencyObject obj) {
			AutomationPeer peer = GetAutomationPeer(obj);
			if(peer != null) {
				peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
				peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
			}
#if DEBUGTEST
			AutomationEventLog.AddEvent(new AutomationEventSnapshot(AutomationEvents.AutomationFocusChanged, peer));
#endif
		}
		#endregion
		protected internal virtual bool ShowGroupSummaryFooter { get { return false; } }
		protected internal virtual bool ShouldDisplayBottomRow { get { return false; } }
		protected internal bool IsEditFormVisible { get { return EditFormManager.IsEditFormVisible; } }
		protected internal bool IsNewItemRowFocused { get { return IsNewItemRowHandle(FocusedRowHandle); } }
		protected internal bool IsTopNewItemRowFocused { get { return IsNewItemRowFocused && !ShouldDisplayBottomRow; } }
		protected internal bool IsBottomNewItemRowFocused { get { return IsNewItemRowFocused && ShouldDisplayBottomRow; } }
		protected internal virtual void MoveNextNewItemRowCell() { }
		protected virtual internal void ResetDataProvider() {
		}
		protected internal virtual void OnUpdateRowsCore() {
			SelectionStrategy.UpdateCachedSelection();
		}
		EditForm.IEditFormManager editFormManagerCore;
		internal EditForm.IEditFormManager EditFormManager {
			get {
				if(editFormManagerCore == null)
					editFormManagerCore = CreateEditFormManager();
				return editFormManagerCore;
			}
		}
		internal protected virtual EditForm.IEditFormManager CreateEditFormManager() {
			return EditForm.EmptyEditFormManager.Instance;
		}
		internal protected virtual EditForm.IEditFormOwner CreateEditFormOwner() {
			return null;
		}
		protected internal virtual int CalcGroupSummaryVisibleRowCount() {
			return 0;
		}
		protected internal virtual bool CanShowColumnInSummaryEditor(ColumnBase column) {
			return !string.IsNullOrEmpty(column.FieldName) && DataProviderBase.Columns[column.FieldName] != null;
		}
		protected internal virtual string FormatSummaryItemCaptionInSummaryEditor(DevExpress.Data.Summary.ISummaryItem item, string defaultCaption) {
			return defaultCaption;
		}
		protected virtual internal void OnColumnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {  }
		internal void SetFocusedRowHandle(int rowHandle) {
#if SL
			FocusedRowHandle = rowHandle;
#else
			SetCurrentValue(FocusedRowHandleProperty, rowHandle);		   
#endif
		}
		internal virtual void PatchVisibleColumns(IList<ColumnBase> visibleColumns, bool hasFixedLeftColumns) { }
		bool IsGroupRowHandleCore(int handle) {
			return DataControl.IsGroupRowHandleCore(handle);
		}
		object GetRowValue(int handle) {
			return DataProviderBase.GetRowValue(handle);
		}
		internal void UpdateCellDataValues(ColumnBase column) {
			UpdateRowData(rowData => rowData.GetCellDataByColumn(column).UpdateValue());
		}
		protected internal virtual bool ShouldUpdateCellData { get { return DataControl != null && !DataControl.DataSourceChangingLocker.IsLocked; } }
		internal virtual ICommand GetColumnCommand(ColumnBase column) {
			return column.Commands.ChangeColumnSortOrder;
		}
		internal bool IsNextRowCellMerged(int visibleIndex, ColumnBase column, bool checkRowData) {
			return IsCellMerged(visibleIndex, visibleIndex + 1, column, checkRowData, visibleIndex);
		}
		internal bool IsPrevRowCellMerged(int visibleIndex, ColumnBase column, bool checkRowData) {
			return IsCellMerged(visibleIndex, visibleIndex - 1, column, checkRowData, visibleIndex - 1);
		}
		internal bool IsMergedCell(int rowHandle, ColumnBase column) {
			if(!DataControl.IsValidRowHandleCore(rowHandle) || ViewBehavior.IsAdditionalRowCore(rowHandle)) return false;
			int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			return IsPrevRowCellMerged(visibleIndex, column, false) || IsNextRowCellMerged(visibleIndex, column, false);
		}
		protected virtual bool IsCellMerged(int visibleIndex1, int visibleIndex2, ColumnBase column, bool checkRowData, int checkMDIndex) {
			return false;
		}
		void UpdateIsFocusedCellIfNeeded(int rowHandle, ColumnBase column = null) {
			if(!ActualAllowCellMerge) return;
			if(column == null) {
				if(DataControl == null) return;
				column = DataControl.CurrentColumn;
			}
			int actualRowHandle = CalcActualRowHandle(rowHandle, column);
			if(actualRowHandle == rowHandle) return;
			RowData rowData = GetRowData(actualRowHandle);
			if(rowData != null)
				rowData.UpdateIsFocusedCell();
		}
		int CalcActualRowHandle(int rowHandle, ColumnBase column) {
			if(!ActualAllowCellMerge || column == null || GetRowData(rowHandle) == null)
				return rowHandle;
			int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			while(IsNextRowCellMerged(visibleIndex, column, true)) {
				visibleIndex++;
			}
			return DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
		}
		internal int CalcActualFocusedRowHandle() {
			return CalcActualRowHandle(FocusedRowHandle, DataControl.CurrentColumn);
		}
		internal void ClearCurrentCellIfNeeded() {
			if(CurrentCell == null)
				return;
			if(GetRowElementByRowHandle(FocusedRowHandle) == null)
				CurrentCell = null;
		}
		internal void SelectRowByValue(string fieldName, object value) {
			SelectRowByValueCore(fieldName, value, true);
		}
		internal void SelectRowByValue(ColumnBase column, object value) {
			SelectRowByValueCore(column.FieldName, value, true);
		}
		void SelectRowByValueCore(string fieldName, object value, bool first) {
			if(FocusedRowHandle == DevExpress.Data.DataController.OperationInProgress && first)
				return;		  
			int rHandle = DataProviderBase.DataController.FindRowByValue(fieldName, value, arg => {
				if (DataControl.VisibleRowCount == 0)
					return;
				int handle = (int)arg;
				if(handle == DataControlBase.InvalidRowHandle) {
					if(first)
						SelectRowByValueCore(fieldName, value, false);
					else
						return;
				} else {
					FocusedRowHandle = handle;
					DataControl.SelectItem(handle);
				}
			});
			if(rHandle != DataControlBase.InvalidRowHandle)
				FocusedRowHandle = rHandle;			
		}
		internal void SelectRowsByValues(string fieldName, IEnumerable<object> values) {
			SelectRowsByValuesCore(fieldName, values);
		}
		internal void SelectRowsByValues(ColumnBase column, IEnumerable<object> values) {
			SelectRowsByValuesCore(column.FieldName, values);
		}
		void SelectRowsByValuesCore(string fieldName, IEnumerable<object> values) {
			if(!IsMultiSelection || values == null || FocusedRowHandle == DevExpress.Data.DataController.OperationInProgress)
				return;
			foreach(var value in values) {
				int rHandle = DataProviderBase.DataController.FindRowByValue(fieldName, value, arg => {
					if(DataControl.VisibleRowCount == 0)
						return;
					int handle = (int)arg;
					if(handle == DataControlBase.InvalidRowHandle)
						SelectItemRowsByValue(fieldName, value);					 
					else
						DataControl.SelectItem(handle);
				});
				if(rHandle != DataControlBase.InvalidRowHandle)
					DataControl.SelectItem(rHandle);
				if(rHandle == DevExpress.Data.DataController.OperationInProgress)
					FocusedRowHandle = rHandle;
			}
		}
		void SelectItemRowsByValue(string fieldName, object value) {
			int rHandle = DataProviderBase.DataController.FindRowByValue(fieldName, value, arg => {
				int handle = (int)arg;
				if(handle == DataControlBase.InvalidRowHandle)
					return;
				else
					DataControl.SelectItem(handle);
			});
			if(rHandle != DataControlBase.InvalidRowHandle)
				DataControl.SelectItem(rHandle);
		}
		protected internal virtual CriteriaOperator GetCheckedFilterPopupFilterCriteria(ColumnBase column, List<object> selectedItems) {
			return ((CheckedListColumnFilterInfo)column.ColumnFilterInfo).GetFilterCriteriaCore();
		}
		protected internal virtual IEnumerable GetCheckedFilterPopupSelectedItems(ColumnBase column, ComboBoxEdit comboBox, CriteriaOperator filterCriteria) {
			return ((CheckedListColumnFilterInfo)column.ColumnFilterInfo).GetSelectedItemsCore(comboBox, filterCriteria);
		}
		#region Printing
		bool IPrintableControl.CanCreateRootNodeAsync {
			get { return GetCanCreateRootNodeAsync(); }
		}
		protected abstract bool GetCanCreateRootNodeAsync();
		IRootDataNode IPrintableControl.CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return CreateRootNode(usablePageSize, reportHeaderSize, reportFooterSize, pageHeaderSize, pageFooterSize);
		}
		protected abstract IRootDataNode CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize);
		void IPrintableControl.CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			CreateRootNodeAsync(usablePageSize, reportHeaderSize, reportFooterSize, pageHeaderSize, pageFooterSize);
		}
		protected abstract void CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize);
		public event EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> CreateRootNodeCompleted {
			add { AddCreateRootNodeCompletedEvent(value); }
			remove { RemoveCreateRootNodeCompletedEvent(value); }
		}
		protected abstract void AddCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler);
		protected abstract void RemoveCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler);
		IVisualTreeWalker IPrintableControl.GetCustomVisualTreeWalker() {
			return null;
		}
		protected virtual IVisualTreeWalker GetCustomVisualTreeWalker() { return null; }
		void IPrintableControl.PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
			PagePrintedCallback(pageBrickEnumerator, brickUpdaters);
		}
		protected abstract void PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters);
		protected internal abstract PrintingDataTreeBuilderBase CreatePrintingDataTreeBuilder(double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy, MasterDetailPrintInfo masterDetailPrintInfo, BandsLayoutBase bandsLayout);
		public void Print() {
			PrintHelper.Print(this);
		}
		public void PrintDirect() {
			PrintHelper.PrintDirect(this);
		}
		public void PrintDirect(PrintQueue queue) {
			PrintHelper.PrintDirect(this, queue);
		}
		public void ShowPrintPreview(FrameworkElement owner) {
			PrintHelper.ShowPrintPreview(owner, this);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName) {
			PrintHelper.ShowPrintPreview(owner, this, documentName);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName, string title) {
			PrintHelper.ShowPrintPreview(owner, this, documentName, title);
		}
		public void ShowPrintPreview(Window owner) {
			PrintHelper.ShowPrintPreview(owner, this);
		}
		public void ShowPrintPreview(Window owner, string documentName) {
			PrintHelper.ShowPrintPreview(owner, this, documentName);
		}
		public void ShowPrintPreview(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreview(owner, this, documentName, title);
		}
		public void ShowPrintPreviewDialog(Window owner) {
			PrintHelper.ShowPrintPreviewDialog(owner, this);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, documentName);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, string title) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, documentName, title);
		}
		protected internal abstract DataTemplate GetPrintRowTemplate();
		#endregion
		#region Export
		#region HTML
		public void ExportToHtml(Stream stream) {
			PrintHelper.ExportToHtml(this, stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, stream, options);
		}
		public void ExportToHtml(string filePath) {
			PrintHelper.ExportToHtml(this, filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			PrintHelper.ExportToHtml(this, filePath, options);
		}
		#endregion
		#region Image
		public void ExportToImage(Stream stream) {
			PrintHelper.ExportToImage(this, stream);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, stream, options);
		}
		public void ExportToImage(string filePath) {
			PrintHelper.ExportToImage(this, filePath);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			PrintHelper.ExportToImage(this, filePath, options);
		}
		#endregion
		#region MHT
		public void ExportToMht(Stream stream) {
			PrintHelper.ExportToMht(this, stream);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, stream, options);
		}
		public void ExportToMht(string filePath) {
			PrintHelper.ExportToMht(this, filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			PrintHelper.ExportToMht(this, filePath, options);
		}
		#endregion
		#region PDF
		public void ExportToPdf(Stream stream) {
			PrintHelper.ExportToPdf(this, stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, stream, options);
		}
		public void ExportToPdf(string filePath) {
			PrintHelper.ExportToPdf(this, filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			PrintHelper.ExportToPdf(this, filePath, options);
		}
		#endregion
		#region RTF
		public void ExportToRtf(Stream stream) {
			PrintHelper.ExportToRtf(this, stream);
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, stream, options);
		}
		public void ExportToRtf(string filePath) {
			PrintHelper.ExportToRtf(this, filePath);
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			PrintHelper.ExportToRtf(this, filePath, options);
		}
		#endregion
		#region Text
		public void ExportToText(Stream stream) {
			PrintHelper.ExportToText(this, stream);
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			PrintHelper.ExportToText(this, stream, options);
		}
		public void ExportToText(string filePath) {
			PrintHelper.ExportToText(this, filePath);
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			PrintHelper.ExportToText(this, filePath, options);
		}
		#endregion
		#region XPS
		public void ExportToXps(Stream stream) {
			PrintHelper.ExportToXps(this, stream);
		}
		public void ExportToXps(Stream stream, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, stream, options);
		}
		public void ExportToXps(string filePath) {
			PrintHelper.ExportToXps(this, filePath);
		}
		public void ExportToXps(string filePath, XpsExportOptions options) {
			PrintHelper.ExportToXps(this, filePath, options);
		}
		#endregion
		#endregion
		Border selectionRectangle;
		internal Border SelectionFrame{get{
			if(selectionRectangle == null && DataControl != null) {
				selectionRectangle = DataControl.SelectionRectangle;
			}
			return selectionRectangle;
		}}
		void OnUpdateSelectionRectanleChanged() {
			selectionRectangle = null;
		}
	}
	public abstract class RowsClipboardController {
		public class RowsClipboardDataProvider : IClipboardDataProvider {
			public RowsClipboardDataProvider(IEnumerable<KeyValuePair<DataControlBase, int>> rows, RowsClipboardController owner) {
				this.rows = rows;
				this.owner = owner;
			}
			IEnumerable<KeyValuePair<DataControlBase, int>> rows;
			RowsClipboardController owner;
			public object GetObjectFromClipboard() {
				return owner.GetSelectedData(rows);
			}
			public string GetTextFromClipboard() {
				return owner.GetTextInRows(rows);
			}
		}
		DataViewBase view;
		protected RowsClipboardController(DataViewBase view) {
			this.view = view;
		}
		protected DataViewBase View { get { return view; } }
		#region public API
		public void CopyRowsToClipboard(IEnumerable<int> rows) {
			IEnumerable<int> sortedHandles = SortRowHandlesByVisibleIndex(rows);
			IEnumerable<KeyValuePair<DataControlBase, int>> sortedRows = sortedHandles.Select(x => new KeyValuePair<DataControlBase, int>(View.DataControl, x));
			CopyToClipboard(() => { return CreateRowsCopyingToClipboardEventArgs(rows); }, new RowsClipboardDataProvider(sortedRows, this));
		}
		internal void CopyRowsToClipboard(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			List<int> rowHandles = null;
			if(!IsMasterDetail())
				rowHandles = rows.Select(row => row.Value).ToList();
			CopyToClipboard(() => { return CreateRowsCopyingToClipboardEventArgs(rowHandles); }, new RowsClipboardDataProvider(rows, this));
		}
		bool IsMasterDetail() {
			return View.RootView.DataControl.DetailDescriptorCore != null && View.RootView.AllowMasterDetailCore;
		}
		public void CopyRangeToClipboard(int startRowHandle, int endRowHandle) {
			if(startRowHandle == DataControlBase.InvalidRowHandle || endRowHandle == DataControlBase.InvalidRowHandle)
				return;
			int startIndex = view.DataProviderBase.GetRowVisibleIndexByHandle(startRowHandle),
				endIndex = view.DataProviderBase.GetRowVisibleIndexByHandle(endRowHandle);
			if(startIndex < 0 || endIndex < 0)
				return;
			if(startIndex > endIndex) {
				int a = endIndex;
				endIndex = startIndex;
				startIndex = a;
			}
			int[] rows = new int[endIndex - startIndex + 1];
			for(int i = startIndex; i <= endIndex; i++) {
				rows[i - startIndex] = view.DataControl.GetRowHandleByVisibleIndexCore(i);
			}
			CopyRowsToClipboard(rows);
		}
		#endregion
		protected string GetTextInRows(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			StringBuilder sb = new StringBuilder();
			int countCopyRows = GetCountCopyRows(rows);
			List<DataControlBase> printableDataControls = rows.Select(row => row.Key).Distinct().ToList();
			var actualRows = GetDataAndHeaderRows(rows.Take(countCopyRows), printableDataControls);
			foreach(var row in actualRows) {
				if(sb.Length > 0) sb.Append(Environment.NewLine);
				AppendIndent(sb, row, printableDataControls);
				row.Key.DataView.GetDataRowText(sb, row.Value);
			}
			if(countCopyRows == 0 && View.ActualClipboardCopyWithHeaders)
				GetHeadersText(sb);
			return sb.ToString();
		}
		IEnumerable<KeyValuePair<DataControlBase, int>> GetDataAndHeaderRows(IEnumerable<KeyValuePair<DataControlBase, int>> rows, List<DataControlBase> printableDataControls) {
			DataControlBase currentDataControl = null;
			foreach(var row in rows) {
				if(GetOriginationDataControl(currentDataControl) != GetOriginationDataControl(row.Key) 
					&& View.ActualClipboardCopyWithHeaders) {
					var printedDataControls = GetThisAndParentDataControls(currentDataControl);
					foreach(var header in GetHeadersForRow(row, printableDataControls.Except(printedDataControls)))
						yield return header;
				}
				currentDataControl = row.Key;
				yield return row;
			}
		}
		DataControlBase GetOriginationDataControl(DataControlBase dataControl) {
			if(dataControl != null)
				return dataControl.GetOriginationDataControl();
			return null;
		}
		static List<DataControlBase> GetThisAndParentDataControls(DataControlBase dataControl) {
			List<DataControlBase> parents = new List<DataControlBase>();
			if(dataControl != null)
				dataControl.EnumerateThisAndParentDataControls(parent => parents.Add(parent));
			return parents;
		}
		static IEnumerable<KeyValuePair<DataControlBase, int>> GetHeadersForRow(KeyValuePair<DataControlBase, int> row, IEnumerable<DataControlBase> printableDataControls) {
			var parents = GetThisAndParentDataControls(row.Key);
			foreach(var dataControl in parents.Reverse<DataControlBase>())
				if(printableDataControls.Contains(dataControl))
					yield return GetHeaderRow(dataControl);
		}
		static KeyValuePair<DataControlBase, int> GetHeaderRow(DataControlBase dataControl) {
			return new KeyValuePair<DataControlBase, int>(dataControl, DataControlBase.InvalidRowHandle);
		}
		static void AppendIndent(StringBuilder sb, KeyValuePair<DataControlBase, int> row, List<DataControlBase> printableDataControls) {
			int level = printableDataControls.Intersect(GetThisAndParentDataControls(row.Key)).Count();
			sb.Append('\t', level - 1);
		}
		void GetHeadersText(StringBuilder sb) {
			view.GetDataRowText(sb, DataControlBase.InvalidRowHandle);
		}
		protected object GetSelectedData(IEnumerable<int> rows) {
			if(rows == null) return null;
			return GetSelectedData(rows.Select(row => new KeyValuePair<DataControlBase, int>(View.DataControl, row)));
		}
		protected virtual object GetSelectedData(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			if(rows == null) return null;
			ArrayList list = new ArrayList();
			int countCopyRows = GetCountCopyRows(rows);
			foreach(var pair in rows) {
				if(countCopyRows-- <= 0)
					break;
				if(CanAddRowToSelectedData(pair.Key, pair.Value))
					list.Add(pair.Key.DataProviderBase.GetRowValue(pair.Value));
			}
			return list;
		}
		protected virtual bool CanAddRowToSelectedData(DataControlBase dataControl, int rowHandle) {
			return true;
		}
		protected virtual int GetCountCopyRows(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			return rows.Count();
		}
		List<int> GetListVisibleIndexFromArrayRowsHandle(IEnumerable<int> rows) {
			List<int> list = new List<int>();
			foreach(int i in rows) {
				int visibleIndex = view.DataProviderBase.GetRowVisibleIndexByHandle(i);
				if(!(visibleIndex < 0))
					list.Add(visibleIndex);
			}
			list.Sort();
			return list;
		}
		IEnumerable<int> SortRowHandlesByVisibleIndex(IEnumerable<int> rows) {
			var sortedVisibleIndicesList = GetListVisibleIndexFromArrayRowsHandle(rows);
			return sortedVisibleIndicesList.Select(x => View.DataControl.GetRowHandleByVisibleIndexCore(x));
		}
		protected void CopyToClipboard(Func<CopyingToClipboardEventArgsBase> createClipboardCopyingEventArgs, IClipboardDataProvider clipboardDataProvider) {
			if(!view.ActualClipboardCopyAllowed)
				return;
			if(view.DataControl.RaiseCopyingToClipboard(createClipboardCopyingEventArgs()))
				return;
			if(!view.RaiseCopyingToClipboard(createClipboardCopyingEventArgs())) {
				if(view.SetDataAwareClipboardData())
					return;
				DXClipboard.SetDataFromClipboardDataProvider(clipboardDataProvider);
			}
		}
		protected abstract CopyingToClipboardEventArgsBase CreateRowsCopyingToClipboardEventArgs(IEnumerable<int> rows);
	}
#if DEBUGTEST
	internal class AutomationEventSnapshot {
		public AutomationEvents Event { get; private set; }
		public AutomationPeer Peer { get; private set; }
		public AutomationEventSnapshot(AutomationEvents raisedEvent, AutomationPeer peer) {
			Event = raisedEvent;
			Peer = peer;
		}
	}
#endif
	[Flags]
	public enum ChangeColumnsSortOrderMode {
		AllColumns = SortedAndGroupedColumns | NotSortedColumns,
		SortedAndGroupedColumns = SortedColumns | GroupedColumns,
		SortedColumns = 4,
		GroupedColumns = 2,
		NotSortedColumns = 1,
	}
}
