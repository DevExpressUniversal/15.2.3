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
using DevExpress.Xpf.Grid.Native;
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
using System.Windows.Input;
using DevExpress.Mvvm;
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
	public class GridCellValidationEventArgs : GridRowValidationEventArgs, IDataCellEventArgs {
		public GridCellValidationEventArgs(object source, object value, int rowHandle, DataViewBase view, ColumnBase column)
			: base(source, value, rowHandle, view) {
			Column = column;
		}
		public ColumnBase Column { get; private set; }
		public object CellValue { get { return DataControl.GetCellValueCore(RowHandle, Column); } }
		public CellValue Cell { get { return new CellValue(Row, Column.FieldName, CellValue); } }
	}
	public delegate void GridCellValidationEventHandler(object sender, GridCellValidationEventArgs e);
	public delegate void GridRowValidationEventHandler(object sender, GridRowValidationEventArgs e);	
	public class InvalidRowExceptionEventArgs : RowEventArgs, IInvalidRowExceptionEventArgs {
		public InvalidRowExceptionEventArgs(GridViewBase view, int rowHandle, string errorText, string windowCaption, Exception exception, ExceptionMode exceptionMode)
			: base(TableView.InvalidRowExceptionEvent, view, rowHandle) {
			ErrorText = errorText;
			WindowCaption = windowCaption;
			Exception = exception;
			ExceptionMode = exceptionMode;
		}
		public string ErrorText { get; set; }
		public string WindowCaption { get; set; }
		public ExceptionMode ExceptionMode { get; set; }
		public Exception Exception { get; private set; }
	}
	public class RowEventArgs : RoutedEventArgs, IDataRowEventArgs {
		protected GridViewBase view;
		public RowEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle)
			: base(routedEvent, view) {
			this.view = view;
			RowHandle = rowHandle;
		}
		protected object forcedRow;
		public RowEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle, object forcedRow)
			: this(routedEvent, view, rowHandle) {
			this.forcedRow = forcedRow;
		}
		protected GridControl Grid { get { return view.Grid; } }
		public int RowHandle { get; protected set; }
		public object Row { get { return forcedRow ?? Grid.GetRow(RowHandle); } }
		public new GridViewBase Source { get { return view; } }
	}
	public class RowAllowEventArgs : RowEventArgs {
		public RowAllowEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle)
			: base(routedEvent, view, rowHandle) {
			Allow = true;
		}
		public bool Allow { get; set; }
	}
	public class CellValueEventArgs : RowEventArgs, IDataCellEventArgs {
		public CellValueEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle, GridColumn column, object value)
			: base(routedEvent, view, rowHandle) {
			Column = column;
			Value = value;
		}
		public GridColumn Column { get; protected set; }
		public object Value { get; protected set; }
		public CellValue Cell { get { return new CellValue(Row, Column.FieldName, Value); } }
	}
	public class CellValueChangedEventArgs : CellValueEventArgs {
		public CellValueChangedEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle, GridColumn column, object value, object oldValue)
			: base(routedEvent, view, rowHandle, column, value) {
			OldValue = oldValue;
		}
		public object OldValue { get; private set; }
	}
	public class CustomGroupDisplayTextEventArgs : CellValueEventArgs {
		public CustomGroupDisplayTextEventArgs(GridViewBase view, int rowHandle, GridColumn column, object value, string displayText)
			: base(GridControl.CustomGroupDisplayTextEvent, view, rowHandle, column, value) {
			DisplayText = displayText;
		}
		internal void SetArgs(GridViewBase view, int rowHandle, GridColumn column, object value, string displayText) {
			this.view = view;
			this.RowHandle = rowHandle;
			this.Column = column;
			this.Value = value;
			this.DisplayText = displayText;
		}
		public string DisplayText { get; set; }
	}
	public class InitNewRowEventArgs : RoutedEventArgs {
		public InitNewRowEventArgs(RoutedEvent routedEvent, DataViewBase view, int rowHandle)
			: base(routedEvent, view) {
			RowHandle = rowHandle;
		}
		public int RowHandle { get; private set; }
	}
	public delegate void RowEventHandler(object sender, RowEventArgs e);
	public delegate void RowAllowEventHandler(object sender, RowAllowEventArgs e);
	public delegate void CellValueEventHandler(object sender, CellValueEventArgs e);
	public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);
	public delegate void InvalidRowExceptionEventHandler(object sender, InvalidRowExceptionEventArgs e);
	public delegate void CustomGroupDisplayTextEventHandler(object sender, CustomGroupDisplayTextEventArgs e);
	public delegate void InitNewRowEventHandler(object sender, InitNewRowEventArgs e);
	public enum NewItemRowPosition {
		None,
		Top,
		Bottom
	}
	public delegate void CopyingToClipboardEventHandler(object sender, CopyingToClipboardEventArgs e);
	public class CopyingToClipboardEventArgs : CopyingToClipboardEventArgsBase {
		IEnumerable<GridCell> gridCells;
		public CopyingToClipboardEventArgs(DataViewBase view, IEnumerable<int> rowHandles, bool copyHeader)
			: base(view, rowHandles, copyHeader) {
		}
		public CopyingToClipboardEventArgs(DataViewBase view, IEnumerable<GridCell> gridCells, bool copyHeader)
			: base(view, null, copyHeader) {
			this.gridCells = gridCells;
			RoutedEvent = GridViewBase.CopyingToClipboardEvent;
		}
		public IEnumerable<GridCell> GridCells { get { return gridCells; } }
	}
	public static class GridSerializationPropertiesNames {
		public const string GroupSummary = "GroupSummary";
		public const string GroupSummarySortInfo = "GroupSummarySortInfo";
	}
	public static class GridSerializationOrders {
		public const int GridControl_GroupSummarySortInfo = SerializationOrders.BaseValue;
	}
	public class GridSerializationOptions : DataControlSerializationOptions {
	}
	public class GridCell : CellBase {
		public GridCell(int rowHandle, GridColumn column) : base(rowHandle, column) { }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCellRowHandle")]
#endif
		public int RowHandle { get { return RowHandleCore; } } 
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCellColumn")]
#endif
		public GridColumn Column { get { return (GridColumn)ColumnCore; } }
	}
	public class RowDoubleClickEventArgs : RoutedEventArgs {
		public GridViewHitInfoBase HitInfo { get; private set; }
		public new DataViewBase Source { get; private set; }
		public RowDoubleClickEventArgs(GridViewHitInfoBase hitInfo
#if !SL
			, MouseButton changedButton
#endif
			, DataViewBase view) {
			HitInfo = hitInfo;
			Source = view;
#if !SL
			ChangedButton = changedButton;
#endif
		}
#if !SL
		public MouseButton ChangedButton { get; private set; }
#endif
	}
	public delegate void RowDoubleClickEventHandler(object sender, RowDoubleClickEventArgs e);
	public class DynamicLocalizationStringValueConvertor : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			LocalizationDescriptor descriptor = value as LocalizationDescriptor;
			if(descriptor == null || parameter == null) {
				return null;
			}
			return descriptor.GetValue(parameter.ToString());
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class GridCustomSummaryEventArgs : CustomSummaryEventArgs {
		public GridCustomSummaryEventArgs(GridControl source) {
			this.Source = source;
		}
		public GridControl Source { get; private set; }
	}
	public class GridCustomSummaryExistEventArgs : CustomSummaryExistEventArgs {
		public GridCustomSummaryExistEventArgs(GridControl source, GroupRowInfo groupRow, object item)
			: base(groupRow, item) {
			this.Source = source;
		}
		public GridControl Source { get; private set; }
	}
	public class ShowingGroupFooterEventArgs : RowAllowEventArgs {
		public ShowingGroupFooterEventArgs(RoutedEvent routedEvent, GridViewBase view, int rowHandle, int level)
			: base(routedEvent, view, rowHandle) {
				Level = level;
		}
		public int Level { get; private set; }
	}
	public delegate void ShowingGroupFooterEventHandler(object sender, ShowingGroupFooterEventArgs e);
}
