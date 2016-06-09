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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web.ASPxTreeList.Internal;
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListNodeEventArgs : EventArgs {
		TreeListNode node;
		public TreeListNodeEventArgs(TreeListNode node) {
			this.node = node;
		}
		public TreeListNode Node { get { return node; } }
	}
	public delegate void TreeListNodeEventHandler(object sender, TreeListNodeEventArgs e);
	public class TreeListNodeCancelEventArgs : TreeListNodeEventArgs {
		bool cancel;
		public TreeListNodeCancelEventArgs(TreeListNode node)
			: base(node) {
			this.cancel = false;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void TreeListNodeCancelEventHandler(object sender, TreeListNodeCancelEventArgs e);
	public class TreeListCustomCallbackEventArgs : EventArgs {
		string argument;
		public TreeListCustomCallbackEventArgs(string argument)
			: base() {
			this.argument = argument;
		}
		public string Argument { get { return argument; } }
	}
	public delegate void TreeListCustomCallbackEventHandler(object sender, TreeListCustomCallbackEventArgs e);
	public class TreeListCustomDataCallbackEventArgs : TreeListCustomCallbackEventArgs {		
		object result;
		public TreeListCustomDataCallbackEventArgs(string argument) 
			: base(argument) {			
			this.result = String.Empty;
		}
		public object Result { get { return result; } set { result = value; } }
	}
	public delegate void TreeListCustomDataCallbackEventHandler(object sender, TreeListCustomDataCallbackEventArgs e);
	public class TreeListCustomSummaryEventArgs : TreeListNodeEventArgs {
		TreeListSummaryItem summaryItem;
		CustomSummaryProcess summaryProcess;
		object value;
		public TreeListCustomSummaryEventArgs(TreeListNode node, TreeListSummaryItem summaryItem, CustomSummaryProcess summaryProcess)
			: base(node) {
			this.summaryItem = summaryItem;
			this.summaryProcess = summaryProcess;
		}
		public object Value { get { return value; } set { this.value = value; } }
		public TreeListSummaryItem SummaryItem { get { return summaryItem; } }
		public CustomSummaryProcess SummaryProcess { get { return summaryProcess; } }
	}
	public delegate void TreeListCustomSummaryEventHandler(object sender, TreeListCustomSummaryEventArgs e);
	public class TreeListHtmlRowEventArgs : EventArgs {
		TreeListDataRowBase row;
		internal TreeListHtmlRowEventArgs(TreeListDataRowBase row) {
			this.row = row;			
		}
		public TableRow Row { get { return row; } }
		public TreeListRowKind RowKind { get { return row.Kind; } }
		public string NodeKey { get { return RowInfo.NodeKey; } }
		public int Level { get { return this.row.Level; } }
		protected TreeListRowInfo RowInfo { get { return row.RowInfo; } }		
		public object GetValue(string fieldName) {
			return RowInfo.GetValue(fieldName);			
		}
	}
	public delegate void TreeListHtmlRowEventHandler(object sender, TreeListHtmlRowEventArgs e);
	public class TreeListHtmlDataCellEventArgs : EventArgs {
		TreeListDataCell cell;
		internal TreeListHtmlDataCellEventArgs(TreeListDataCell cell) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
		public TreeListDataColumn Column { get { return cell.Column; } }
		public object CellValue { get { return RowInfo.GetValue(Column.FieldName); } }
		public string NodeKey { get { return RowInfo.NodeKey; } }
		public int Level { get { return this.cell.Level; } }
		protected TreeListRowInfo RowInfo { get { return cell.RowInfo; } }
		public object GetValue(string fieldName) {
			return RowInfo.GetValue(fieldName);
		}
	}
	public delegate void TreeListHtmlDataCellEventHandler(object sender, TreeListHtmlDataCellEventArgs e);
	public class TreeListHtmlCommandCellEventArgs : EventArgs {
		TreeListCommandCell cell;
		internal TreeListHtmlCommandCellEventArgs(TreeListCommandCell cell) 
			: base() {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
		public TreeListCommandColumn Column { get { return cell.Column; } }
		public string NodeKey { get { return cell.RowInfo.NodeKey; } }
		public int Level { get { return this.cell.Level; } }
	}
	public delegate void TreeListHtmlCommandCellEventHandler(object sender, TreeListHtmlCommandCellEventArgs e);
	public class TreeListVirtualModeCreateChildrenEventArgs : EventArgs {
		object nodeObject;
		IList children;
		public TreeListVirtualModeCreateChildrenEventArgs(object nodeObject) {
			this.nodeObject = nodeObject;
		}
		public object NodeObject { get { return nodeObject; } }
		public IList Children { get { return children; } set { children = value; } }
	}
	public delegate void TreeListVirtualModeCreateChildrenEventHandler(object sender, TreeListVirtualModeCreateChildrenEventArgs e);
	public class TreeListVirtualModeNodeCreatingEventArgs : EventArgs {
		object keyValue;
		object nodeObject;
		TreeListUnboundNodeDataItem dataItem;
		bool isLeaf;
		public TreeListVirtualModeNodeCreatingEventArgs(IWebTreeListData treeListData, object nodeObject) {
			this.nodeObject = nodeObject;
			this.keyValue = null;
			this.dataItem = new TreeListUnboundNodeDataItem(treeListData);
			this.isLeaf = false;
		}
		public object NodeKeyValue { get { return keyValue; } set { keyValue = value; } }
		public object NodeObject { get { return nodeObject; } }
		public bool IsLeaf { get { return isLeaf; } set { isLeaf = value; } }
		protected internal TreeListUnboundNodeDataItem DataItem { get { return dataItem; } }
		public void SetNodeValue(string fieldName, object value) {
			DataItem.SetValue(fieldName, value);			
		}
	}
	public delegate void TreeListVirtualModeNodeCreatingEventHandler(object sender, TreeListVirtualModeNodeCreatingEventArgs e);
	public class TreeListVirtualNodeEventArgs : TreeListNodeEventArgs {
		object nodeObject;
		public TreeListVirtualNodeEventArgs(TreeListNode node, object nodeObject) 
			: base(node) {
			this.nodeObject = nodeObject;
		}
		public object NodeObject { get { return nodeObject; } }
	}
	public delegate void TreeListVirtualNodeEventHandler(object sender, TreeListVirtualNodeEventArgs e);
	public class TreeListCustomJSPropertiesEventArgs : CustomJSPropertiesEventArgs {
		public TreeListCustomJSPropertiesEventArgs()
			: base() {
		}
		public TreeListCustomJSPropertiesEventArgs(Dictionary<string, object> properties)
			: base(properties) {
		}
	}
	public delegate void TreeListCustomJSPropertiesEventHandler(object sender, TreeListCustomJSPropertiesEventArgs e);
	public class TreeListNodeEditingEventArgs : CancelEventArgs {
		string nodeKey;
		public TreeListNodeEditingEventArgs(string nodeKey)
			: base() {
			this.nodeKey = nodeKey;
		}
		public string NodeKey { get { return nodeKey; } }
	}
	public delegate void TreeListNodeEditingEventHandler(object sender, TreeListNodeEditingEventArgs e);
	public class TreeListNodeValidationEventArgs : EventArgs {
		bool isNew;
		OrderedDictionary keys, oldValues, values;
		string nodeError;
		Dictionary<string, string> errors;
		public TreeListNodeValidationEventArgs(bool isNew) {
			this.isNew = isNew;
			this.keys = new OrderedDictionary();
			this.oldValues = new OrderedDictionary();
			this.values = new OrderedDictionary();
			this.nodeError = String.Empty;
			this.errors = new Dictionary<string, string>();			
		}
		public bool IsNewNode { get { return isNew; } }
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary OldValues { get { return oldValues; } }
		public OrderedDictionary NewValues { get { return values; } }
		public string NodeError {
			get { return nodeError; }
			set { nodeError = value; }
		}
		public Dictionary<string, string> Errors { get { return errors; } }
		public bool HasErrors { get { return !String.IsNullOrEmpty(NodeError) || Errors.Count > 0; } }
	}
	public delegate void TreeListNodeValidationEventHandler(object sender, TreeListNodeValidationEventArgs e);
	public class TreeListColumnEditorEventArgs : EventArgs {
		string nodeKey;
		object value;
		ASPxEditBase editor;
		TreeListDataColumn column;
		public TreeListColumnEditorEventArgs(string nodeKey, TreeListDataColumn column, ASPxEditBase editor, object value ) 
			: base() {
			this.nodeKey = nodeKey;
			this.column = column;
			this.editor = editor;
			this.value = value;
		}
		public string NodeKey { get { return nodeKey; } }
		public TreeListDataColumn Column { get { return column; } }
		public ASPxEditBase Editor { get { return editor; } }
		public object Value { get { return value; } }
	}
	public delegate void TreeListColumnEditorEventHandler(object sender, TreeListColumnEditorEventArgs e);
	public class TreeListNodeDragEventArgs : TreeListNodeCancelEventArgs {
		TreeListNode newParentNode;
		bool handled;
		public TreeListNodeDragEventArgs(TreeListNode node, TreeListNode newParentNode) 
			: base(node) {
			this.newParentNode = newParentNode;
			this.handled = false;
		}
		public TreeListNode NewParentNode { get { return newParentNode; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}		
	}
	public delegate void TreeListNodeDragEventHandler(object sender, TreeListNodeDragEventArgs e);
	public class TreeListCustomErrorTextEventArgs : EventArgs {
		Exception exception;
		string errorText;
		public TreeListCustomErrorTextEventArgs(Exception exception, string errorText) 
			: base() {
			this.exception = exception;
			this.errorText = errorText;
		}
		public Exception Exception { get { return exception; } }
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public delegate void TreeListCustomErrorTextEventHandler(object sender, TreeListCustomErrorTextEventArgs e);
	public class TreeListCustomNodeSortEventArgs : EventArgs {
		TreeListNode node1, node2;
		TreeListDataColumn column;
		bool handled;
		int result;
		public TreeListCustomNodeSortEventArgs(TreeListDataColumn column, TreeListNode node1, TreeListNode node2)
			: base() {
			this.node1 = node1;
			this.node2 = node2;
			SetColumn(column);
		}
		public TreeListDataColumn Column { get { return column; } }
		public ColumnSortOrder SortOrder { get { return Column.SortOrder; } }
		public TreeListNode Node1 { get { return node1; } }
		public TreeListNode Node2 { get { return node2; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public int Result {
			get { return result; }
			set { result = value; }
		}
		protected internal void SetColumn(TreeListDataColumn column) {
			this.column = column;
			this.handled = false;
			this.result = 0;
		}
	}
	public delegate void TreeListCustomNodeSortEventHandler(object sender, TreeListCustomNodeSortEventArgs e);
	public class TreeListEditingOperationEventArgs : EventArgs {		
		TreeListEditingOperation operation;
		public TreeListEditingOperationEventArgs(TreeListEditingOperation operation) {			
			this.operation = operation;
		}
		public TreeListEditingOperation Operation { get { return operation; } }
	}
	public delegate void TreeListEditingOperationEventHandler(object sender, TreeListEditingOperationEventArgs e);
	public class TreeListCommandColumnButtonEventArgs : EventArgs {
		TreeListCommandColumn column;
		string nodeKey;
		TreeListCommandColumnButtonType buttonType;
		int customButtonIndex;
		DefaultBoolean visible;
		DefaultBoolean enabled;
		internal TreeListCommandColumnButtonEventArgs(TreeListCommandColumn column, string nodeKey, TreeListCommandColumnButtonType buttonType, int customButtonIndex) {
			this.column = column;
			this.nodeKey = nodeKey;
			this.buttonType = buttonType;
			this.customButtonIndex = customButtonIndex;
			this.visible = DefaultBoolean.Default;
			this.enabled = DefaultBoolean.Default;
		}
		public TreeListCommandColumn CommandColumn { get { return column; } }
		public string NodeKey { get { return nodeKey; } }
		public TreeListCommandColumnButtonType ButtonType { get { return buttonType; } }
		public int CustomButtonIndex { get { return customButtonIndex; } }
		public DefaultBoolean Visible {
			get { return visible; }
			set { visible = value; }
		}
		public DefaultBoolean Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
	}
	public delegate void TreeListCommandColumnButtonEventHandler(object sender, TreeListCommandColumnButtonEventArgs e);
}
