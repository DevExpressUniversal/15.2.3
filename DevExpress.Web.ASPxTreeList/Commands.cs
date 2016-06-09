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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public static class TreeListCallbackResultProperties {
		public const string
			Data = "data",
			FocusedNodeKey = "fkey",
			MaxVisibleLevel = "level",
			VisibleColumnCount = "visColCount",
			VisibleRowCount = "rows",
			PartialUpdateNodeKey = "partial",
			RowCountToRemove = "remove",
			IsCustomData = "customData",
			CustomCallbackArgument = "arg",
			CustomDataHandlerIndex = "handler",
			EditingNodeKey = "editingKey",
			NewNodeEditingMark = "newNode",
			PageIndex = "pi",
			PageCount = "pc",
			CommandButtonIDList = "cButtonIDs";
	}
	public static class TreeListCommandFactory {
		public static TreeListCommand CreateInstance(ASPxTreeList treeList, TreeListCommandId id, string args) {
			switch(id) {
				case TreeListCommandId.Expand:
					return new TreeListExpandCommand(treeList, args);
				case TreeListCommandId.Collapse:
					return new TreeListCollapseCommand(treeList, args);
				case TreeListCommandId.Pager:
					return new TreeListPagerCommand(treeList, args);
				case TreeListCommandId.CustomDataCallback:
					return new TreeListCustomDataCallbackCommand(treeList, args);
				case TreeListCommandId.MoveColumn:
					return new TreeListMoveColumnCommand(treeList, args);
				case TreeListCommandId.Sort:
					return new TreeListSortCommand(treeList, args);
				case TreeListCommandId.Dummy:
					return new TreeListDummyCommand(treeList);
				case TreeListCommandId.ExpandAll:
					return new TreeListExpandAllCommand(treeList);
				case TreeListCommandId.CollapseAll:
					return new TreeListCollapseAllCommand(treeList);
				case TreeListCommandId.CustomCallback:
					return new TreeListCustomCallbackCommand(treeList, args);
				case TreeListCommandId.StartEdit:
					return new TreeListStartEditCommand(treeList, args);
				case TreeListCommandId.UpdateEdit:
					return new TreeListUpdateEditCommand(treeList);
				case TreeListCommandId.CancelEdit:
					return new TreeListCancelEditCommand(treeList);
				case TreeListCommandId.MoveNode:
					return new TreeListMoveNodeCommand(treeList, args);
				case TreeListCommandId.DeleteNode:
					return new TreeListDeleteNodeCommand(treeList, args);
				case TreeListCommandId.StartEditNewNode:
					return new TreeListStartEditNewNodeCommand(treeList, args);
				case TreeListCommandId.GetNodeValues:
					return new TreeListGetNodeValuesCommand(treeList, args);
				default: 
					return null;
			}
		}
	}
	public abstract class TreeListCommand {		
		ASPxTreeList treeList;
		protected TreeListCommand(ASPxTreeList treeList, string args) {
			this.treeList = treeList;
			ReadArguments(args);
		}
		protected TreeListCommand(ASPxTreeList treeList) 
			: this(treeList, String.Empty) {
		}
		public virtual bool IsPartialUpdateNodeSingle { get { return false; } }
		public virtual bool PartialUpdatePossible { get { return false; } }
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected TreeListDataHelper TreeDataHelper { get { return TreeList.TreeDataHelper; } }
		protected TreeListRenderHelper RenderHelper { get { return TreeList.RenderHelper; } }
		public abstract void Execute();
		protected abstract void ReadArguments(string args);
		public virtual IDictionary GetCallbackResult() {
			IDictionary result = CreateBasicCallbackResult();
			result[TreeListCallbackResultProperties.Data] = RenderHelper.GetFullRenderResult();
			return result;			
		}
		public virtual string GetPartialUpdateNodeKey() {
			return string.Empty;
		}
		protected IDictionary CreateBasicCallbackResult() {
			IDictionary result = new Hashtable();
			result[CallbackResultProperties.StateObject] = TreeList.GetClientObjectStateForCallback();
			if(RenderHelper.IsFocusedNodeEnabled)
				result[TreeListCallbackResultProperties.FocusedNodeKey] = TreeDataHelper.FocusedNodeKey ?? String.Empty;
			result[TreeListCallbackResultProperties.MaxVisibleLevel] = TreeDataHelper.MaxVisibleLevel;
			result[TreeListCallbackResultProperties.VisibleRowCount] = TreeDataHelper.Rows.Count;
			result[TreeListCallbackResultProperties.VisibleColumnCount] = TreeList.VisibleColumns.Count;
			if(TreeDataHelper.EditingKey != null)
				result[TreeListCallbackResultProperties.EditingNodeKey] = TreeDataHelper.EditingKey;
			if(TreeDataHelper.IsNewNodeEditing)
				result[TreeListCallbackResultProperties.NewNodeEditingMark] = 1;
			if(TreeList.SettingsPager.Mode != TreeListPagerMode.ShowAllNodes) {
				result[TreeListCallbackResultProperties.PageIndex] = TreeDataHelper.PageIndex;
				result[TreeListCallbackResultProperties.PageCount] = TreeDataHelper.PageCount;
			}
			if(TreeList.CommandButtonsHelper.HasButtons)
				result[TreeListCallbackResultProperties.CommandButtonIDList] = TreeList.CommandButtonsHelper.GetClientIDs();
			return result;
		}
		protected IDictionary CreatePartialCallbackResult(string nodeKey, string html, int removeCount) {
			IDictionary result = CreateBasicCallbackResult();
			result[TreeListCallbackResultProperties.PartialUpdateNodeKey] = nodeKey;
			result[TreeListCallbackResultProperties.Data] = html;
			result[TreeListCallbackResultProperties.RowCountToRemove] = removeCount;
			return result;
		}
		protected IDictionary CreateCustomDataCallbackResult(int clientHandlerIndex, string arg, object data) {
			IDictionary result = new Hashtable();
			result[TreeListCallbackResultProperties.IsCustomData] = 1;
			result[TreeListCallbackResultProperties.CustomCallbackArgument] = arg;
			result[TreeListCallbackResultProperties.Data] = data;
			if(clientHandlerIndex > -1)
				result[TreeListCallbackResultProperties.CustomDataHandlerIndex] = clientHandlerIndex;
			return result;
		}
	}
	public abstract class TreeListNodeCommand : TreeListCommand {
		string nodeKey = null;		
		public TreeListNodeCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {			
		}
		protected string NodeKey { get { return nodeKey; } }
		protected override sealed void ReadArguments(string args) {
			this.nodeKey = args;
		}
		public override string GetPartialUpdateNodeKey() {
			return NodeKey;
		}
	}
	public class TreeListExpandCommand : TreeListNodeCommand {
		int removeCount;
		bool requireFullUpdate;
		public TreeListExpandCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
			this.requireFullUpdate = false;
		}
		public override bool PartialUpdatePossible { get { return !requireFullUpdate; } }
		public override void Execute() {
			this.removeCount = 1 + TreeListBuilderHelper.GetAuxRowCount(RenderHelper, NodeKey);
			if(TreeDataHelper.IsNodeExpanded(NodeKey)) {
				this.requireFullUpdate = true;
				return;
			}
			TreeListNode node = TreeDataHelper.GetNodeByKey(NodeKey);
			if(node == null) {
				this.requireFullUpdate = true;
				return;
			}
			TreeListNodeCancelEventArgs expandingArgs = new TreeListNodeCancelEventArgs(node);
			TreeList.RaiseNodeExpanding(expandingArgs);
			if(!expandingArgs.Cancel) {
				TreeDataHelper.SetNodeExpanded(NodeKey, true);
				TreeListNodeEventArgs expandedArgs = new TreeListNodeEventArgs(node);
				TreeList.RaiseNodeExpanded(expandedArgs);
			}
			if(RenderHelper.RenderPopupEditForm)
				this.requireFullUpdate = true;
		}
		public override IDictionary GetCallbackResult() {
			if(this.requireFullUpdate || !TreeList.IsPartialCallbackPossible())
				return base.GetCallbackResult();
			string html = RenderHelper.GetPartialRenderResult(NodeKey);
			if(String.IsNullOrEmpty(html))
				return base.GetCallbackResult();
			return CreatePartialCallbackResult(NodeKey, html, this.removeCount);
		}
	}
	public class TreeListCollapseCommand : TreeListNodeCommand {
		int rowCountToRemove;
		bool requireFullUpdate;
		bool canceled;
		public TreeListCollapseCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
			this.rowCountToRemove = 0;
			this.requireFullUpdate = false;
			this.canceled = false;
		}
		public override bool IsPartialUpdateNodeSingle { get { return this.canceled; } }
		public override bool PartialUpdatePossible { get { return !requireFullUpdate && TreeList.IsPartialCallbackPossible(); } }
		protected int RowCountToRemove { get { return rowCountToRemove; }  }
		public override void Execute() {
			if(RenderHelper.RenderPopupEditForm)
				this.requireFullUpdate = true;
			if(!TreeDataHelper.IsNodeExpanded(NodeKey)) {
				this.requireFullUpdate = true;
				return;
			}
			TreeListNode node = TreeDataHelper.GetNodeByKey(NodeKey);
			if(node == null) {
				this.requireFullUpdate = true;
				TreeDataHelper.SetNodeExpanded(NodeKey, false); 
				return;
			}
			TreeListNodeCancelEventArgs collapsingArgs = new TreeListNodeCancelEventArgs(node);
			TreeList.RaiseNodeCollapsing(collapsingArgs);
			if(!collapsingArgs.Cancel) {
				if(PartialUpdatePossible)
					this.rowCountToRemove = CalcRowCountToRemove();
				TreeDataHelper.SetNodeExpanded(NodeKey, false);
				TreeListNodeEventArgs collapsedArgs = new TreeListNodeEventArgs(node);
				TreeList.RaiseNodeCollapsed(collapsedArgs);
			} else {
				this.canceled = true;
			}
		}
		public override IDictionary GetCallbackResult() {
			if(!PartialUpdatePossible)
				return base.GetCallbackResult();
			string html = RenderHelper.GetPartialRenderResult(NodeKey);
			if(String.IsNullOrEmpty(html))
				return base.GetCallbackResult();
			return CreatePartialCallbackResult(NodeKey, html, RowCountToRemove);
		}
		int CalcRowCountToRemove() {
			List<TableRow> rows = TreeListUtils.GetSubtreeHtmlRows(TreeList.GetDataTable(), NodeKey);
			return rows.Count - 1;
		}
	}
	public class TreeListPagerCommand : TreeListCommand {
		string pagerArgs;
		bool fromKbd;
		public TreeListPagerCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
		}		
		protected override void ReadArguments(string args) {
			string[] parts = args.Split(TreeListRenderHelper.SeparatorToken);
			this.pagerArgs = parts[0];
			if(parts.Length > 1)
				this.fromKbd = true;			
		}
		public override void Execute() {
			int oldIndex = TreeDataHelper.PageIndex;
			int oldSize = TreeDataHelper.PageSize;
			int newIndex = oldIndex;
			int newSize = oldSize;
			var postponedEvents = new List<Action>();
			if(ASPxPagerBase.IsChangePageSizeCommand(this.pagerArgs)) {
				newSize = ASPxPagerBase.GetNewPageSize(this.pagerArgs, oldSize);
				if(newSize <= 0) {
					TreeDataHelper.PageIndex = -1;
					newSize = oldSize;
				} else {
					if(oldIndex == -1)
						TreeDataHelper.PageIndex = 0;
				}
				bool switchShowAll = new[] { oldIndex, TreeDataHelper.PageIndex }.Any(x => x == -1);
				if(oldSize != newSize && TreeDataHelper.PagerIsValidPageSize(newSize) || switchShowAll) {
					TreeDataHelper.PageSize = newSize;
					postponedEvents.Add(TreeList.RaisePageSizeChanged);
					if(switchShowAll)
						postponedEvents.Add(TreeList.RaisePageIndexChanged);
				}
			} else {
				newIndex = ASPxPagerBase.GetNewPageIndex(this.pagerArgs, TreeDataHelper.PageIndex, TreeDataHelper.PageCount);
				if(oldIndex != newIndex && TreeDataHelper.PagerIsValidPageIndex(newIndex)) {
					TreeDataHelper.PageIndex = newIndex;
					postponedEvents.Add(TreeList.RaisePageIndexChanged);
					if(new[]{oldIndex, newIndex}.Any(x => x == -1))
						postponedEvents.Add(TreeList.RaisePageSizeChanged);
					if(this.fromKbd && newIndex < oldIndex)
						TreeDataHelper.FocusedNodeKey = RenderHelper.Rows[RenderHelper.Rows.Count - 1].NodeKey;
				}
			}
			postponedEvents.AsParallel().ForAll(x => x());
		}
	}
	public class TreeListCustomCallbackCommand : TreeListCommand {
		string argument;
		public TreeListCustomCallbackCommand(ASPxTreeList treeList, string args) 
			: base(treeList, args) {
		}
		protected string Argument { get { return argument; } }
		protected override void ReadArguments(string args) {
			this.argument = args;
		}
		public override void Execute() {
			TreeListCustomCallbackEventArgs e = new TreeListCustomCallbackEventArgs(Argument);
			TreeList.RaiseCustomCallback(e);
		}
	}
	public class TreeListCustomDataCallbackCommand : TreeListCommand {
		TreeListCustomDataCallbackEventArgs eventArgs = null;
		public TreeListCustomDataCallbackCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {			
		}
		protected TreeListCustomDataCallbackEventArgs EventArgs { get { return eventArgs; } }
		protected override void ReadArguments(string args) {			
			this.eventArgs = new TreeListCustomDataCallbackEventArgs(args);			
		}
		public override void Execute() {
			TreeList.RaiseCustomDataCallback(EventArgs);
		}
		public override IDictionary GetCallbackResult() {
			return CreateCustomDataCallbackResult(-1, EventArgs.Argument, EventArgs.Result);
		}
	}
	public class TreeListMoveColumnCommand : TreeListCommand {
		int sourceIndex, targetIndex;
		bool before;
		public TreeListMoveColumnCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
		}
		protected int SourceIndex { get { return sourceIndex; } }
		protected int TargetIndex { get { return targetIndex; } }
		protected bool Before { get { return before; } }
		protected override void ReadArguments(string args) {
			string[] data = args.Split(TreeListRenderHelper.SeparatorToken);
			if(data.Length != 3)
				throw new ArgumentException();
			this.sourceIndex = Int32.Parse(data[0]);
			this.targetIndex = Int32.Parse(data[1]);
			this.before = Boolean.Parse(data[2]);
		}
		public override void Execute() {
			TreeListColumn column = TreeList.Columns[SourceIndex];
			if(TargetIndex < 0) {
				if(column.Visible)
					HideColumn(column);
			} else {
				int index = PrepareIndex(column);
				if(!column.Visible)
					ShowColumn(column, index);
				else
					MoveColumn(column, index);
			}
		}
		int PrepareIndex(TreeListColumn column) {
			int index = TreeList.Columns[TargetIndex].VisibleIndex;
			if(column.VisibleIndex > -1 && column.VisibleIndex < index)
				--index;
			if(!Before)
				++index;
			return index;
		}
		void MoveColumn(TreeListColumn column, int index) {
			column.VisibleIndex = index;
			TreeList.ResetVisibleColumns();
		}
		void ShowColumn(TreeListColumn column, int visibleIndex) {
			column.Visible = true;
			column.VisibleIndex = visibleIndex;
			TreeList.ResetVisibleColumns();
			TreeDataHelper.ResetVisibleData();			
		}
		void HideColumn(TreeListColumn column) {
			column.Visible = false;
			TreeList.ResetVisibleColumns();
		}
	}
	public class TreeListSortCommand : TreeListCommand {
		TreeListDataColumn column;
		ColumnSortOrder sortOrder;
		bool reset;
		public TreeListSortCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
		}
		protected internal TreeListDataColumn Column { get { return column; } }
		protected internal ColumnSortOrder SortOrder { get { return sortOrder; } }
		protected internal bool Reset { get { return reset; } }
		protected override void ReadArguments(string args) {
			string[] data = args.Split(TreeListRenderHelper.SeparatorToken);
			if(data.Length != 3)
				throw new ArgumentException();
			ReadColumn(data[0]);
			ReadSortOrder(data[1]);
			ReadReset(data[2]);
		}
		public override void Execute() {
			if(!RenderHelper.IsColumnSortable(Column)) return;
			if(Reset)
				TreeList.KillSortingInfo();
			Column.SortOrder = SortOrder;
		}
		void ReadColumn(string value) {
			int index;
			if(Int32.TryParse(value, out index))
				this.column = TreeList.Columns[index] as TreeListDataColumn;
			else
				this.column = TreeList.Columns[value] as TreeListDataColumn;
			if(Column == null)
				throw new ArgumentException();
		}
		void ReadSortOrder(string value) {
			switch(value.ToLower()) {
				case "desc": 
					this.sortOrder = ColumnSortOrder.Descending; 
					break;
				case "none": 
					this.sortOrder = ColumnSortOrder.None; 
					break;
				case "asc": 
					this.sortOrder = ColumnSortOrder.Ascending; 
					break;
				default:
					this.sortOrder = Column.SortOrder == ColumnSortOrder.Ascending
						? ColumnSortOrder.Descending
						: ColumnSortOrder.Ascending;
					break;
			}
		}
		void ReadReset(string value) {
			this.reset = false;
			if(!String.IsNullOrEmpty(value))
				Boolean.TryParse(value, out this.reset);
		}
	}
	public abstract class TreeListNoArgsCommand : TreeListCommand {
		public TreeListNoArgsCommand(ASPxTreeList treeList) 
			: base(treeList)  {
		}
		protected override sealed void ReadArguments(string args) {
		}
	}
	public class TreeListDummyCommand : TreeListNoArgsCommand {
		public TreeListDummyCommand(ASPxTreeList treeList)
			: base(treeList) {
		}
		public override void Execute() {
		}
	}
	public class TreeListExpandAllCommand : TreeListNoArgsCommand {
		public TreeListExpandAllCommand(ASPxTreeList treeList)
			: base(treeList) {
		}
		public override void Execute() {
			TreeList.ExpandAll();
		}
	}
	public class TreeListCollapseAllCommand : TreeListNoArgsCommand {
		public TreeListCollapseAllCommand(ASPxTreeList treeList)
			: base(treeList) {
		}
		public override void Execute() {
			TreeList.CollapseAll();
		}
	}
	public class TreeListGetNodeValuesCommand : TreeListCommand {
		int clientHandlerIndex;
		TreeListGetNodeValuesCommandMode mode;
		string nodeKey;
		List<string> fieldNames;
		object result;
		public TreeListGetNodeValuesCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
		}
		protected int ClientHandlerIndex { get { return clientHandlerIndex; } }
		protected TreeListGetNodeValuesCommandMode Mode { get { return mode; } }
		protected string NodeKey { get { return nodeKey; } }
		protected List<string> FieldNames { get { return fieldNames; } }
		protected object Result { get { return result; } }
		#region Read arguments
		protected override void ReadArguments(string args) {
			string[] data = args.Split(TreeListRenderHelper.SeparatorToken);			
			ReadClientHandlerIndex(data[0]);
			ReadMode(data[1]);
			ReadNodeKey(data[2]);
			this.fieldNames = new List<string>();
			for(int i = 3; i < data.Length; i++)
				ReadFieldName(data[i]);			
		}
		void ReadClientHandlerIndex(string value) {
			this.clientHandlerIndex = Int32.Parse(value);
			if(ClientHandlerIndex < 0)
				throw new ArgumentException();
		}
		void ReadMode(string value) {
			this.mode = (TreeListGetNodeValuesCommandMode)Int32.Parse(value);
		}
		void ReadNodeKey(string value) {
			this.nodeKey = TreeListUtils.UnescapeNodeKey(value);
		}
		void ReadFieldName(string value) {
			if(value.Contains(";")) {
				string[] names = value.Split(';');
				foreach(string name in names)
					AddFieldNameCore(name);
			} else {
				AddFieldNameCore(value);
			}
		}
		void AddFieldNameCore(string name) {
			if(!String.IsNullOrEmpty(name))
				FieldNames.Add(name);
		}
		#endregion
		public override void Execute() {
			PrepareResultObject();
			switch(Mode) {
				case TreeListGetNodeValuesCommandMode.ByKey:
					AddNodeTuplet(NodeKey);
					break;
				case TreeListGetNodeValuesCommandMode.Visible:
				case TreeListGetNodeValuesCommandMode.SelectedVisible:
					TreeDataHelper.ResetVisibleData(); 
					foreach(TreeListRowInfo info in RenderHelper.Rows) {
						if(info.NodeKey == TreeListRenderHelper.NewNodeKey) continue;
						if(!info.Selected && mode == TreeListGetNodeValuesCommandMode.SelectedVisible)
							continue;
						AddNodeTuplet(info.NodeKey);
					}
					break;
				case TreeListGetNodeValuesCommandMode.SelectedAll:
					foreach(TreeListNode node in TreeList.GetSelectedNodes())
						AddNodeTuplet(node);
					break;
				default:
					throw new NotImplementedException();
			}
		}
		public override IDictionary GetCallbackResult() {
			return CreateCustomDataCallbackResult(ClientHandlerIndex, String.Empty, Result);
		}
		void AddNodeTuplet(TreeListNode node) {
			object tuplet = null;
			int count = FieldNames.Count;
			if(count > 0) {
				if(count < 2) {
					tuplet = node[FieldNames[0]];
				} else {
					object[] list = new object[count];
					for(int i = 0; i < count; i++)
						list[i] = node[FieldNames[i]];
					tuplet = list;
				}
			}
			IList resultList = Result as IList;
			if(resultList != null)
				resultList.Add(tuplet);
			else
				this.result = tuplet;
		}
		void AddNodeTuplet(string key) {
			AddNodeTuplet(TreeList.FindNodeByKeyValue(key));
		}
		void PrepareResultObject() {
			if(Mode != TreeListGetNodeValuesCommandMode.ByKey)
				this.result = new ArrayList();
			else
				this.result = null;
		}
	}
	public class TreeListStartEditCommand : TreeListNodeCommand {
		bool requireFullUpdate;
		int removeCount;
		public TreeListStartEditCommand(ASPxTreeList treeList, string args) 
			: base(treeList, args) {
			this.requireFullUpdate = false;
		}
		public override bool IsPartialUpdateNodeSingle { get { return true; } }
		public override bool PartialUpdatePossible { get { return !requireFullUpdate; } }
		public override void Execute() {
			if(TreeDataHelper.IsEditing || TreeList.SettingsEditing.IsPopupEditForm)
				this.requireFullUpdate = true;
			else
				this.removeCount = TreeListBuilderHelper.GetAuxRowCount(RenderHelper, NodeKey);
			TreeList.StartEdit(NodeKey);
		}
		public override IDictionary GetCallbackResult() {
			if(this.requireFullUpdate || !TreeList.IsPartialCallbackPossible())
				return base.GetCallbackResult();
			string html = RenderHelper.GetPartialRenderResult(NodeKey);
			return CreatePartialCallbackResult(NodeKey, html, this.removeCount);
		}
	}
 	public class TreeListUpdateEditCommand : TreeListNoArgsCommand {
		bool requireFullUpdate;
		string partialUpdateKey;
		int removeCount;
		public TreeListUpdateEditCommand(ASPxTreeList treeList) 
			: base(treeList) {
			this.partialUpdateKey = null;
			this.requireFullUpdate = false;
		}
		public override bool IsPartialUpdateNodeSingle { get { return true; } }
		public override bool PartialUpdatePossible { get { return !requireFullUpdate; } }
		public override void Execute() {
			string key = TreeList.EditingNodeKey;
			TreeList.UpdateEdit();
			if(TreeList.IsEditing && TreeList.EditingNodeKey == key) {
				this.partialUpdateKey = key;
				this.removeCount = TreeListBuilderHelper.GetAuxRowCount(RenderHelper, key);
			}
			if(String.IsNullOrEmpty(this.partialUpdateKey) || TreeList.SettingsEditing.IsPopupEditForm)
				this.requireFullUpdate = true;
		}
		public override IDictionary GetCallbackResult() {
			if(this.requireFullUpdate || !TreeList.IsPartialCallbackPossible())
				return base.GetCallbackResult();
			string html = RenderHelper.GetPartialRenderResult(this.partialUpdateKey);
			return CreatePartialCallbackResult(this.partialUpdateKey, html, this.removeCount);
		}
		public override string GetPartialUpdateNodeKey() {
			return partialUpdateKey;
		}
	}
	public class TreeListCancelEditCommand : TreeListNoArgsCommand {
		string editedKey;
		int removeCount;
		bool requireFullUpdate;
		public TreeListCancelEditCommand(ASPxTreeList treeList) 
			: base(treeList) {
			this.requireFullUpdate = false;
		}
		public override bool IsPartialUpdateNodeSingle { get { return true; } }
		public override bool PartialUpdatePossible { get { return !requireFullUpdate; } }
		protected string EditedKey { get { return editedKey; } }
		public override void Execute() {
			string key = TreeDataHelper.EditingKey;
			this.editedKey = key;
			if(!String.IsNullOrEmpty(key))
				this.removeCount = TreeListBuilderHelper.GetAuxRowCount(RenderHelper, key);
			TreeList.CancelEdit();
			if(String.IsNullOrEmpty(EditedKey) || TreeList.SettingsEditing.IsPopupEditForm)
				this.requireFullUpdate = true;
		}
		public override IDictionary GetCallbackResult() {
			if(this.requireFullUpdate || !TreeList.IsPartialCallbackPossible())
				return base.GetCallbackResult();
			string html = RenderHelper.GetPartialRenderResult(EditedKey);
			return CreatePartialCallbackResult(EditedKey, html, this.removeCount);
		}
		public override string GetPartialUpdateNodeKey() {
			return EditedKey;
		}
	}
	public class TreeListMoveNodeCommand : TreeListCommand {
		string nodeKey, parentNodeKey;
		public TreeListMoveNodeCommand(ASPxTreeList treeList, string args) 
			: base(treeList, args) {
		}
		protected string NodeKey { get { return nodeKey; } }
		protected string ParentNodeKey { get { return parentNodeKey; } }
		protected override void ReadArguments(string args) {
			string[] list = args.Split(TreeListRenderHelper.SeparatorToken);
			if(list.Length != 2)
				throw new ArgumentException();
			this.nodeKey = TreeListUtils.UnescapeNodeKey(list[0]);
			this.parentNodeKey = TreeListUtils.UnescapeNodeKey(list[1]);
		}
		public override void Execute() {
			TreeList.MoveNodeInternal(NodeKey, ParentNodeKey);
		}
	}
	public class TreeListDeleteNodeCommand : TreeListNodeCommand {
		public TreeListDeleteNodeCommand(ASPxTreeList treeList, string args) 
			: base(treeList, args) {
		}
		public override void Execute() {
			TreeList.DeleteNodeInternal(NodeKey);
		}
	}
	public class TreeListStartEditNewNodeCommand : TreeListNodeCommand {
		public TreeListStartEditNewNodeCommand(ASPxTreeList treeList, string args)
			: base(treeList, args) {
		}
		public override void Execute() {
			TreeList.StartEditNewNode(NodeKey);
		}
	}
}
