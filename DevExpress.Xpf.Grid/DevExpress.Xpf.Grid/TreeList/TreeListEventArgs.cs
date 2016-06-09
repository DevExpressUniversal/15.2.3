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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
#if SL
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListNodeAllowEventArgs : TreeListNodeEventArgs {
		public bool Allow { get; set; }
		public TreeListNodeAllowEventArgs(TreeListNode node)
			: base(node) {
				Allow = true;
		}
	}
	public class TreeListNodeEventArgs : RoutedEventArgs, IDataRowEventArgs {
		public TreeListNode Node { get; protected set; }
		public TreeListNodeEventArgs(TreeListNode node) {
			Node = node;
		}
		public object Row { get { return Node.Content; } }
	}
	public class TreeListNodeChangedEventArgs : EventArgs {
		public NodeChangeType ChangeType { get; protected set; }
		public TreeListNode Node { get; protected set; }
		public TreeListNodeChangedEventArgs(TreeListNode node, NodeChangeType changeType) {
			Node = node;
			ChangeType = changeType;
		}
	}
	public class TreeListUnboundColumnDataEventArgs : ColumnDataEventArgsBase {
		public TreeListNode Node { get; private set; }
		protected internal TreeListUnboundColumnDataEventArgs(ColumnBase column, TreeListNode node, object _value, bool isGetAction)
			: base(column, _value, isGetAction) {
				Node = node;
		}
	}
	public class TreeListNodeFilterEventArgs : EventArgs {
		public TreeListNodeFilterEventArgs(TreeListNode node) {
			Node = node;
			Handled = false;
			Visible = true;
		}
		public bool Handled { get; set;  }
		public bool Visible { get; set; }
		public TreeListNode Node { get; private set; } 
	}
	public class TreeListCustomColumnSortEventArgs : EventArgs {
		public TreeListCustomColumnSortEventArgs(ColumnBase column, TreeListNode node1, TreeListNode node2, object value1, object value2)
			: base() {
			Node1 = node1;
			Node2 = node2;
			SetArgs(column, value1, value2);
		}
		public ColumnBase Column { get; private set; }
		public ColumnSortOrder SortOrder { get { return Column.SortOrder; } }
		public TreeListNode Node1 { get; private set; }
		public TreeListNode Node2 { get; private set; }
		public object Value1 { get; private set; }
		public object Value2 { get; private set; }
		public bool Handled { get; set; }
		public int Result { get; set; }
		protected internal void SetArgs(ColumnBase column, object value1, object value2) {
			Column = column;
			Value1 = value1;
			Value2 = value2;
			Handled = false;
			Result = 0;
		}
	}
	public class TreeListCustomColumnDisplayTextEventArgs : EventArgs {
		public TreeListCustomColumnDisplayTextEventArgs(TreeListNode node, ColumnBase column, object value, string displayText) {
			SetArgs(node, column, value, displayText);
		}
		public TreeListNode Node { get; private set; }
		public ColumnBase Column { get; private set; }
		public object Value { get; private set; }
		public string DisplayText { get; set; }
		public bool ShowAsNullText { get; set; }
		protected internal void SetArgs(TreeListNode node, ColumnBase column, object value, string displayText) {
			Node = node;
			Column = column;
			Value = value;
			DisplayText = displayText;
			ShowAsNullText = false;
		}
		internal void Clear() {
			Node = null;
			Column = null;
			Value = null;
		}
	}
	public class TreeListCustomSummaryEventArgs : EventArgs {
		public TreeListCustomSummaryEventArgs(TreeListNode node, SummaryItemBase summaryItem, CustomSummaryProcess summaryProcess, object fieldValue) {
			Node = node;
			SummaryItem = summaryItem;
			SummaryProcess = summaryProcess;
			FieldValue = fieldValue;
		}
		public object TotalValue { get; set; }
		public object FieldValue { get; private set; }
		public TreeListNode Node { get; private set;}
		public SummaryItemBase SummaryItem { get; private set; }
		public CustomSummaryProcess SummaryProcess { get; private set; }
	}
	public class TreeListInvalidNodeExceptionEventArgs : TreeListNodeEventArgs, IInvalidRowExceptionEventArgs {
		public TreeListInvalidNodeExceptionEventArgs(TreeListNode node, string errorText, string windowCaption, Exception exception, ExceptionMode exceptionMode)
			: base(node) {
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
	public class TreeListNodeValidationEventArgs : GridRowValidationEventArgs, IDataRowEventArgs {
		public TreeListNodeValidationEventArgs(object value, int rowHandle, TreeListView view) : base(view, value, rowHandle, view) {  }
		public TreeListNode Node { get { return ((TreeListView)view).GetNodeByRowHandle(RowHandle); } }
	}
	public class TreeListCellValidationEventArgs : GridCellValidationEventArgs, IDataCellEventArgs {
		public TreeListCellValidationEventArgs(object source, object value, int rowHandle, TreeListView view, ColumnBase column)
			: base(source, value, rowHandle, view, column) {
		}
		public TreeListNode Node { get { return ((TreeListView)view).GetNodeByRowHandle(RowHandle); } }
	}
	public class CustomColumnFilterListEventArgs : EventArgs {
		public CustomColumnFilterListEventArgs(TreeListNode node, TreeListColumn column) {
			Node = node;
			Column = column;
			Visible = true;
		}
		public bool Visible { get; set; }
		public TreeListColumn Column { get; set; }
		public TreeListNode Node { get; set; }
	}
	public class TreeListCellValueEventArgs : TreeListNodeEventArgs , IDataCellEventArgs{
		public TreeListCellValueEventArgs(TreeListNode node, ColumnBase column, object value)
			: base(node) {
			Column = column;
			Value = value;
		}
		public ColumnBase Column { get; protected set; }
		public object Value { get; protected set; }
		public CellValue Cell { get { return new CellValue(Row, Column.FieldName, Value); } }
	}
	public class TreeListCellValueChangedEventArgs : TreeListCellValueEventArgs {
		public TreeListCellValueChangedEventArgs(TreeListNode node, ColumnBase column, object value, object oldValue)
			: base(node, column, value) {
			OldValue = oldValue;
		}
		public object OldValue { get; private set; }
	}
	public class TreeListSelectionChangedEventArgs : GridSelectionChangedEventArgs {
		public TreeListSelectionChangedEventArgs(TreeListView view, CollectionChangeAction action, int controllerRow) : base(view, action, controllerRow)  {  }
		public TreeListNode Node { get { return ((TreeListView)Source).GetNodeByRowHandle(ControllerRow); } }
	}
	public class TreeListCopyingToClipboardEventArgs : CopyingToClipboardEventArgsBase {
		IEnumerable<TreeListNode> nodes;
		public TreeListCopyingToClipboardEventArgs(TreeListView view, IEnumerable<int> rowHandles, bool copyHeader)
			: base(view, rowHandles, copyHeader) {
		}
		public TreeListCopyingToClipboardEventArgs(TreeListView view, IEnumerable<TreeListCell> cells, bool copyHeader) : base(view, null, copyHeader) {
			Cells = cells;
		}
		public IEnumerable<TreeListNode> Nodes {
			get {
				if(nodes == null)
					nodes = ((TreeListView)Source).GetNodesFromRowHandles(RowHandles);
				return nodes;
			}
		}
		public IEnumerable<TreeListCell> Cells { get; private set; }
	}
	public class TreeListCustomBestFitEventArgs : CustomBestFitEventArgsBase {
		public TreeListCustomBestFitEventArgs(ColumnBase column, BestFitMode bestFitMode)
			: base(column, bestFitMode) {
				RoutedEvent = TreeListView.CustomBestFitEvent;
		}
		public ColumnBase Column { get { return ColumnCore; } }
	}
	public class TreeListEditorEventArgs : EditorEventArgsBase {
		public TreeListEditorEventArgs(TreeListView view, int rowHandle, ColumnBase column, IBaseEdit editor)
			: base(TreeListView.ShownEditorEvent, view, rowHandle, column) {
			Editor = editor;
		}
		public IBaseEdit Editor { get; private set; }
		public TreeListNode Node { get { return ((TreeListView)view).TreeListDataProvider.GetNodeByRowHandle(RowHandle); } }
	}
	public class TreeListShowingEditorEventArgs : ShowingEditorEventArgsBase {
		public TreeListShowingEditorEventArgs(TreeListView view, int rowHandle, ColumnBase column)
			: base(TreeListView.ShowingEditorEvent, view, rowHandle, column) {
		}
		public TreeListNode Node { get { return ((TreeListView)view).TreeListDataProvider.GetNodeByRowHandle(RowHandle); } }
	}
	public class TreeListSerializationOptions : DataControlSerializationOptions {
	}
	public delegate void TreeListCellValueChangedEventHandler(object sender, TreeListCellValueChangedEventArgs e);
	public delegate void TreeListCellValidationEventHandler(object sender, TreeListCellValidationEventArgs e);
	public delegate void TreeListNodeValidationEventHandler(object sender, TreeListNodeValidationEventArgs e);
	public delegate void TreeListInvalidNodeExceptionEventHandler(object sender, TreeListInvalidNodeExceptionEventArgs e);
	public delegate void TreeListCustomSummaryEventHandler(object sender, TreeListCustomSummaryEventArgs e);
	public delegate void TreeListCustomColumnDisplayTextEventHandler(object sender, TreeListCustomColumnDisplayTextEventArgs e);
	public delegate void TreeListCustomColumnSortEventHandler(object sender, TreeListCustomColumnSortEventArgs e);
	public delegate void TreeListNodeFilterEventHandler(object sender, TreeListNodeFilterEventArgs e);
	public delegate void CustomColumnFilterListEventHandler(object sender, CustomColumnFilterListEventArgs e);
	public delegate void TreeListNodeAllowEventHandler(object sender, TreeListNodeAllowEventArgs e);
	public delegate void TreeListNodeEventHandler(object sender, TreeListNodeEventArgs e);
	public delegate void TreeListNodeChangedEventHandler(object sender, TreeListNodeChangedEventArgs e);
	public delegate void TreeListUnboundColumnDataEventHandler(object sender, TreeListUnboundColumnDataEventArgs e);
	public delegate void TreeListSelectionChangedEventHandler(object sender, TreeListSelectionChangedEventArgs e);
	public delegate void TreeListCopyingToClipboardEventHandler(object sender, TreeListCopyingToClipboardEventArgs e);
	public delegate void TreeListCustomBestFitEventHandler(object sender, TreeListCustomBestFitEventArgs e);
	public delegate void TreeListShowingEditorEventHandler(object sender, TreeListShowingEditorEventArgs e);
	public delegate void TreeListEditorEventHandler(object sender, TreeListEditorEventArgs e);
}
