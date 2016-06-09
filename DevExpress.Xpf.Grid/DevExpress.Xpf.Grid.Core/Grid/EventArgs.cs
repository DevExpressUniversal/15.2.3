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
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Core;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Data.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Media.Animation;
#if SILVERLIGHT
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public delegate void UpdateRowDataDelegate(RowData rowData);
	public interface INotifyCurrentViewChanged {
		void OnCurrentViewChanged(DependencyObject d);
	}
	public interface INotifyNavigationIndexChanged {
		void OnNavigationIndexChanged();
	}
	public class GridUIPropertyAttribute : XtraSerializablePropertyId {
		public const int IdUI = 1;
		public GridUIPropertyAttribute()
			: base(IdUI) {
		}
	}
	public class GridSerializeAlwaysPropertyAttribute : XtraSerializablePropertyId {
		public const int IdSerializeAlways = 2;
		public GridSerializeAlwaysPropertyAttribute()
			: base(IdSerializeAlways) {
		}
	}
	public class GridStoreAlwaysPropertyAttribute : XtraSerializablePropertyId {
		public GridStoreAlwaysPropertyAttribute()
			: base(-1) {
		}
	}
	public static class GridColumnTypeHelper {
		public static BaseEditSettings CreateEditSettings(Type type) {
			switch(GetTypeCode(type)) {
				case TypeCode.Boolean:
					return new CheckEditSettings();
				case TypeCode.DateTime:
					return new DateEditSettings();
				default:
					return new TextEditSettings();
			}
		}
		public static TypeCode GetTypeCode(Type type) {
			return Type.GetTypeCode(ResolveType(type));
		}
		static Type ResolveType(Type type) {
			if(type == null)
				return type;
			return Nullable.GetUnderlyingType(type) ?? type;
		}
		public static bool IsNumericType(Type type) {
			return IsNumericTypeCode(GetTypeCode(type));
		}
		public static bool IsNumericTypeCode(TypeCode typeCode) {
			return typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal;
		}
	}
	public static class SerializationPropertiesNames {
		public const string TotalSummary = "TotalSummary";
		public const string SortInfo = "SortInfo";
		public const string Columns = "Columns";
		public const string MRUFilters = "MRUFilters";
		public const string Bands = "Bands";
		public const string UseFieldNameForSerialization = "UseFieldNameForSerialization";
	}
	public static class SerializationOrders {
		public const int BaseValue = int.MaxValue;
		public const int GridControl_FilterString = BaseValue - 2;
		public const int GridControl_IsFilterEnabled = BaseValue - 1;
		public const int GridColumn_ActualWidth = BaseValue;
		public const int GridColumn_Width = BaseValue - 1;
		public const int GridControl_MRUFilters = BaseValue - 2;
		public const int GridControl_UseFieldNameForSerialization = -1;
	}
	public class DataControlSerializationOptions {
		public static readonly DependencyProperty RemoveOldColumnsProperty =
			DependencyPropertyManager.RegisterAttached("RemoveOldColumns", typeof(bool), typeof(DataControlSerializationOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty AddNewColumnsProperty =
			DependencyPropertyManager.RegisterAttached("AddNewColumns", typeof(bool), typeof(DataControlSerializationOptions), new PropertyMetadata(true));
		public static bool GetAddNewColumns(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewColumnsProperty);
		}
		public static void SetAddNewColumns(DependencyObject obj, bool value) {
			obj.SetValue(AddNewColumnsProperty, value);
		}
		public static bool GetRemoveOldColumns(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldColumnsProperty);
		}
		public static void SetRemoveOldColumns(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldColumnsProperty, value);
		}
	}
	[Serializable]
	public class InfiniteGridSizeException : ApplicationException {
		internal const string InfiniteWidthMessage = "By default, an infinite grid width is not allowed since all grid cards will be rendered and, hence, the grid will work very slowly. To fix this issue, place the grid into a container that will give a finite width to the grid, or manually specify the grid's Width or MaxWidth. Note that you can also avoid this exception by setting the {0}.AllowInfiniteGridSize static property to True, but in that case, the grid will run slowly.";
		internal const string InfiniteHeightMessage = "By default, an infinite grid height is not allowed since all grid rows will be rendered and, hence, the grid will work very slowly. To fix this issue, place the grid into a container that will give a finite height to the grid, or manually specify the grid's Height or MaxHeight. Note that you can also avoid this exception by setting the {0}.AllowInfiniteGridSize static property to True, but in that case, the grid will run slowly.";
		internal static void ValidateDefineSize(double defineSize, Orientation orientation, string DataControlTypeName) {
			if(!DataControlBase.AllowInfiniteGridSize && double.IsPositiveInfinity(defineSize))
				throw new InfiniteGridSizeException(
					String.Format(orientation == Orientation.Horizontal ? InfiniteWidthMessage : InfiniteHeightMessage, DataControlTypeName)
					);
		}
		public InfiniteGridSizeException(string message)
			: base(message) {
		}
	}
	public class GridRowValidationEventArgs : ValidationEventArgs, IDataRowEventArgs {
		protected readonly DataViewBase view;
		protected readonly int fRowHandle;
		public GridRowValidationEventArgs(object source, object value, int rowHandle, DataViewBase view) :
			base(BaseEdit.ValidateEvent, source, value, CultureInfo.CurrentCulture) {
			this.view = view;
			this.fRowHandle = rowHandle;
		}
		protected DataControlBase DataControl { get { return view.DataControl; } }
		public virtual int RowHandle { get { return fRowHandle; } }
		public virtual object Row { get { return DataControl.GetRow(RowHandle); } }
	}
	public abstract class RowValidationError : BaseValidationError {
		public RowValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, bool isCellError)
			: base(errorContent, exception, errorType) {
			this.fRowHandle = rowHandle;
			this.IsCellError = isCellError;
		}
		protected readonly int fRowHandle;
		internal bool IsCellError { get; private set; }
	}
	public class CellBase {
		int rowHandle;
		ColumnBase column;
		public CellBase(int rowHandle, ColumnBase column) {
			this.column = column;
			this.rowHandle = rowHandle;
		}
		protected internal int RowHandleCore { get { return rowHandle; } }
		protected internal ColumnBase ColumnCore { get { return column; } }
		public bool Equals(CellBase gridCell) {
			return gridCell != null && gridCell.RowHandleCore == rowHandle && gridCell.ColumnCore == column;
		}
	}
	public delegate void PastingFromClipboardEventHandler(object sender, PastingFromClipboardEventArgs e);
	public class PastingFromClipboardEventArgs : GridEventArgs {
		public PastingFromClipboardEventArgs(DataControlBase source, RoutedEvent routedEvent) : base(source, routedEvent) { }
	}
	public enum TableViewSelectMode { None, Row, Cell };
	public class UnboundExpressionEditorEventArgs : RoutedEventArgs {
		public UnboundExpressionEditorEventArgs(ExpressionEditorControl control, ColumnBase column) {
			this.ExpressionEditorControl = control;
			this.Column = column;
		}
		public ExpressionEditorControl ExpressionEditorControl { get; private set; }
		public ColumnBase Column { get; private set; }
		public new DataViewBase Source { get { return Column.View; } }
	}
	public delegate void UnboundExpressionEditorEventHandler(object sender, UnboundExpressionEditorEventArgs e);
	public class FilterPopupEventArgs : RoutedEventArgs {
		public FilterPopupEventArgs(ColumnBase column, PopupBaseEdit popupBaseEdit) {
			this.Column = column;
			this.PopupBaseEdit = popupBaseEdit;
		}
		public ColumnBase Column { get; private set; }
		public ComboBoxEdit ComboBoxEdit { get { return PopupBaseEdit as ComboBoxEdit; } }
		public PopupBaseEdit PopupBaseEdit { get; private set; }
		public new DataViewBase Source { get { return Column.View; } }
	}
	public delegate void FilterPopupEventHandler(object sender, FilterPopupEventArgs e);
	public class CustomFilterDisplayTextEventArgs : RoutedEventArgs {
		public CustomFilterDisplayTextEventArgs(DataViewBase source, object value) {
			Value = value;
			this.Source = source;
		}
		public object Value { get; set; }
		public new DataViewBase Source { get; private set; }
	}
	public delegate void CustomFilterDisplayTextEventHandler(object sender, CustomFilterDisplayTextEventArgs e);
	public class FilterEditorEventArgs : RoutedEventArgs {
		public FilterEditorEventArgs(DataViewBase source, FilterControl control) {
			this.FilterControl = control;
			this.Source = source;
		}
		public FilterControl FilterControl { get; private set; }
		public new DataViewBase Source { get; private set; }
	}
	public delegate void FilterEditorEventHandler(object sender, FilterEditorEventArgs e);
	public interface IInvalidRowExceptionEventArgs {
		string ErrorText { get; }
		string WindowCaption { get; }
		ExceptionMode ExceptionMode { get; set; }
		Exception Exception { get; }
	}
	public enum ExceptionMode { DisplayError, ThrowException, NoAction, Ignore } ;
	public class FocusedRowHandleChangedEventArgs : RoutedEventArgs {
		public FocusedRowHandleChangedEventArgs(RowData rowData) {
			RowData = rowData;
		}
		public RowData RowData { get; private set; }
	}
	public delegate void FocusedRowHandleChangedEventHandler(object sender, FocusedRowHandleChangedEventArgs e);
	public class FocusedRowChangedEventArgs : RoutedEventArgs {
		public FocusedRowChangedEventArgs(DataViewBase source, object oldRow, object newRow) {
			OldRow = oldRow;
			NewRow = newRow;
			Source = source;
		}
		public object OldRow { get; private set; }
		public object NewRow { get; private set; }
		public new DataViewBase Source { get; private set; }
	}
	public delegate void FocusedRowChangedEventHandler(object sender, FocusedRowChangedEventArgs e);
	public class FocusedViewChangedEventArgs : RoutedEventArgs {
		public DataViewBase OldView { get; private set; }
		public DataViewBase NewView { get; private set; }
		public FocusedViewChangedEventArgs(DataViewBase oldView, DataViewBase newView) {
			OldView = oldView;
			NewView = newView;
		}
	}
	public delegate void FocusedViewChangedEventHandler(object sender, FocusedViewChangedEventArgs e);
	public class FocusedColumnChangedEventArgs : RoutedEventArgs {
		public GridColumnBase OldColumn { get; private set; }
		public GridColumnBase NewColumn { get; private set; }
		public new DataViewBase Source { get; private set; }
		public FocusedColumnChangedEventArgs(DataViewBase source, GridColumnBase oldColumn, GridColumnBase newColumn) {
			OldColumn = oldColumn;
			NewColumn = newColumn;
			Source = source;
		}
	}
	public delegate void FocusedColumnChangedEventHandler(object sender, FocusedColumnChangedEventArgs e);
	public class ItemsSourceChangedEventArgs : RoutedEventArgs {
		[Obsolete("Instead use the OldItemsSource property.")]
		public object OldDataSource { get { return OldItemsSource; } }
		[Obsolete("Instead use the NewItemsSource property.")]
		public object NewDataSource { get { return NewItemsSource; } }
		public object OldItemsSource { get; private set; }
		public object NewItemsSource { get; private set; }
		public new DataControlBase Source { get; private set; }
		public ItemsSourceChangedEventArgs(DataControlBase source, object oldDataSource, object newDataSource) {
			OldItemsSource = oldDataSource;
			NewItemsSource = newDataSource;
			Source = source;
		}
	}
	public delegate void ItemsSourceChangedEventHandler(object sender, ItemsSourceChangedEventArgs e);
	public abstract class EditorEventArgsBase : RoutedEventArgs {
		protected EditorEventArgsBase(RoutedEvent routedEvent, DataViewBase view, int rowHandle, ColumnBase column)
			: base(routedEvent, view) {
			RowHandle = rowHandle;
			Column = column;
			this.view = view;
		}
		protected DataViewBase view;
		DataControlBase DataControl { get { return view.DataControl; } }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("EditorEventArgsBaseRowHandle")]
#endif
		public int RowHandle { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("EditorEventArgsBaseColumn")]
#endif
		public ColumnBase Column { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("EditorEventArgsBaseValue")]
#endif
		public object Value { get { return DataControl.GetCellValueCore(RowHandle, Column); } }
		public new DataViewBase Source { get { return view; } }
	}
	public class ShowingEditorEventArgsBase : EditorEventArgsBase {
		public ShowingEditorEventArgsBase(RoutedEvent routedEvent, DataViewBase view, int rowHandle, ColumnBase column)
			: base(routedEvent, view, rowHandle, column) {
		}
		public bool Cancel { get; set; }
	}
	public class ColumnDataEventArgsBase : EventArgs {
		ColumnBase column;
		object _value = null;
		bool isGetAction = true;
		protected internal ColumnDataEventArgsBase(ColumnBase column, object _value, bool isGetAction) {
			this.column = column;
			this._value = _value;
			this.isGetAction = isGetAction;
		}
		public ColumnBase Column { get { return column; } }
		public bool IsGetData { get { return isGetAction; } }
		public bool IsSetData { get { return !IsGetData; } }
		public object Value { get { return _value; } set { _value = value; } }
	}
	public class CustomScrollAnimationEventArgs : RoutedEventArgs {
		public CustomScrollAnimationEventArgs(double oldOffset, double newOffset) {
			OldOffset = oldOffset;
			NewOffset = newOffset;
		}
		public Storyboard Storyboard { get; set; }
		public double OldOffset { get; private set; }
		public double NewOffset { get; private set; }
	}
	public delegate void CustomScrollAnimationEventHandler(object sender, CustomScrollAnimationEventArgs e);
	public class CopyingToClipboardEventArgsBase : RoutedEventArgs {
		readonly IEnumerable<int> rowHandles;
		readonly bool copyHeader;
		readonly DataViewBase view;
		public CopyingToClipboardEventArgsBase(DataViewBase view, IEnumerable<int> rowHandles, bool copyHeader) {
			this.rowHandles = rowHandles;
			this.copyHeader = copyHeader;
			this.view = view;
		}
		public IEnumerable<int> RowHandles { get { return rowHandles; } }
		public bool CopyHeader { get { return copyHeader; } }
		public new DataViewBase Source { get { return view; } }
	}
	public class CustomUniqueValuesEventArgs : RoutedEventArgs {
		public new DataControlBase Source { get { return Column.OwnerControl; } }
		public bool IncludeFilteredOut { get; private set; }
		public bool RoundDateTime { get; private set; }
		public ColumnBase Column { get; private set; }
		public object[] UniqueValues { get; set; }
		public OperationCompleted AsyncCompleted { get; private set; }
		public CustomUniqueValuesEventArgs(ColumnBase column, bool includeFilteredOut, bool roundDateTime, OperationCompleted asyncCompleted) {
			Column = column;
			IncludeFilteredOut = includeFilteredOut;
			RoundDateTime = roundDateTime;
			AsyncCompleted = asyncCompleted;
		}
	}
	public delegate void CustomUniqueValuesEventHandler(object sender, CustomUniqueValuesEventArgs e);
	public class CustomBestFitEventArgsBase : RoutedEventArgs {
		BestFitMode bestFitMode;
		public CustomBestFitEventArgsBase(ColumnBase column, BestFitMode bestFitMode) {
			this.BestFitMode = bestFitMode;
			this.ColumnCore = column;
		}
		public BestFitMode BestFitMode {
			get { return bestFitMode; }
			set {
				if(value == BestFitMode.Smart || value == BestFitMode.Default)
					throw new IncorrectBestFitModeException();
				bestFitMode = value;
			}
		}
		public IEnumerable<int> BestFitRows { get; set; }
		protected ColumnBase ColumnCore { get; private set; }
	}
	public class IncorrectBestFitModeException : ApplicationException { }
	public class GridEventArgs : RoutedEventArgs {
		public GridEventArgs(DataControlBase dataControl, RoutedEvent routedEvent)
			:base(routedEvent) {
			this.Source = dataControl;
		}
		public new DataControlBase Source { get; private set; }
	}
	public class GridCancelRoutedEventArgs : CancelRoutedEventArgs {
		public GridCancelRoutedEventArgs(DataControlBase dataControl, RoutedEvent routedEvent)
			: base(routedEvent) {
			this.Source = dataControl;
		}
		public new DataControlBase Source { get; private set; }
	}
	public delegate void ColumnHeaderClickEventHandler(object sender, ColumnHeaderClickEventArgs e);
	public class ColumnHeaderClickEventArgs : RoutedEventArgs {
		public ColumnHeaderClickEventArgs(ColumnBase column, bool isShift, bool isCtrl) {
			Column = column;
			IsShift = isShift;
			IsCtrl = isCtrl;
		}
		public ColumnBase Column { get; private set; }
		public bool IsShift { get; set; }
		public bool IsCtrl { get; set; }
		public bool AllowSorting { get; set; }
	}
	public delegate void AutoGeneratingColumnEventHandler(object sender, AutoGeneratingColumnEventArgs e);
	public class AutoGeneratingColumnEventArgs : CancelRoutedEventArgs {
		public AutoGeneratingColumnEventArgs(ColumnBase column) {
			Column = column;
		}
		public ColumnBase Column { get; private set; }
	}
	public delegate void CurrentItemChangedEventHandler(object sender, CurrentItemChangedEventArgs e);
	public class CurrentItemChangedEventArgs : RoutedEventArgs {
		public CurrentItemChangedEventArgs(DataControlBase source, object oldItem, object newItem) {
			OldItem = oldItem;
			NewItem = newItem;
			Source = source;
		}
		public object OldItem { get; private set; }
		public object NewItem { get; private set; }
		public new DataControlBase Source { get; private set; }
	}
	public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);
	public class SelectedItemChangedEventArgs : CurrentItemChangedEventArgs {
		public SelectedItemChangedEventArgs(DataControlBase source, object oldItem, object newItem) : base(source, oldItem, newItem) {  }
	}
	public class CurrentColumnChangedEventArgs : RoutedEventArgs {
		public GridColumnBase OldColumn { get; private set; }
		public GridColumnBase NewColumn { get; private set; }
		public new DataControlBase Source { get; private set; }
		public CurrentColumnChangedEventArgs(DataControlBase source, GridColumnBase oldColumn, GridColumnBase newColumn) {
			OldColumn = oldColumn;
			NewColumn = newColumn;
			Source = source;
		}
	}
	public delegate void CurrentColumnChangedEventHandler(object sender, CurrentColumnChangedEventArgs e);
	public delegate void GridSelectionChangedEventHandler(object sender, GridSelectionChangedEventArgs e);
	public class GridSelectionChangedEventArgs : RoutedEventArgs {
		readonly CollectionChangeAction action;
		readonly int controllerRow;
		readonly DataViewBase view;
		public GridSelectionChangedEventArgs(DataViewBase view, CollectionChangeAction action, int controllerRow) {
			this.action = action;
			this.controllerRow = controllerRow;
			this.view = view;
		}
		public CollectionChangeAction Action { get { return action; } }
		public int ControllerRow { get { return controllerRow; } }
		public new DataViewBase Source { get { return view; } }
	}
	public delegate void CustomGetParentEventEventHandler(object sender, CustomGetParentEventArgs e);
	public class CustomGetParentEventArgs : EventArgs {
		readonly object child;
		public object Parent { get; set; }
		public object Child { get { return child; } }
		public bool Handled { get; set; }
		public CustomGetParentEventArgs(object child) {
			this.child = child;
		}
	}
}
