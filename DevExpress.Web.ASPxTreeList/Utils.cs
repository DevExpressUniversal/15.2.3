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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public static class TreeListUtils {
		public static void CalcRowIndent(TreeListNode node, List<TreeListRowIndentType> indents, bool showRootIndent) {
			indents.Clear();
			int level = node.Level;
			int min = showRootIndent ? 0 : 1;
			TreeListRowIndentType indent;
			TreeListNode parent = node.ParentNode;
			for(int i = level; i > min; i--) {
				if(i == level) {
					if(level == 1 && node.IsFirst() && node.IsLast())
						indent = TreeListRowIndentType.Last;
					else if(level == 1 && node.IsFirst())
						indent = TreeListRowIndentType.First;
					else if(node.IsLast())
						indent = TreeListRowIndentType.Last;
					else
						indent = TreeListRowIndentType.Middle;
				} else {
					indent = parent.IsLast() ? TreeListRowIndentType.None : TreeListRowIndentType.Root;
					parent = parent.ParentNode;
				}
				indents.Add(indent);
			}
			indents.Reverse();
		}	
		public static List<TableRow> GetSubtreeHtmlRows(Table table, string rootNodeKey) {			
			List<TableRow> result = new List<TableRow>();
			bool inside = false;
			int rootLevel = -1;
			TreeListRowInfo row;
			foreach(TableRow htmlRow in table.Rows) {
				TreeListDataRowBase dataRow = htmlRow as TreeListDataRowBase;
				if(dataRow == null)
					continue;
				row = dataRow.RowInfo;
				int level = row.Indents.Count;
				if(dataRow.Kind == TreeListRowKind.Footer || dataRow.Kind == TreeListRowKind.GroupFooter)
					level++;
				if(inside && level <= rootLevel && !AuxRowKinds.Contains(dataRow.Kind)) {
					result.Add(htmlRow);
					break;
				}
				if(!inside && row.NodeKey == rootNodeKey) {
					inside = true;
					rootLevel = level;
				}
				if(inside)
					result.Add(htmlRow);
			}
			return result;
		}
		static readonly List<TreeListRowKind> AuxRowKinds = new List<TreeListRowKind>(
			new TreeListRowKind[] { TreeListRowKind.Preview, TreeListRowKind.Error, TreeListRowKind.EditForm });
		public static object GetPropertyValue(object owner, string name) {
			DataRowView view = owner as DataRowView;
			if(view != null) {
				try {
					return view.Row[name];
				} catch(ArgumentException) {
					return null;
				}
			}
			object value = null;
			if(ReflectionUtils.TryToGetPropertyValue(owner, name, out value))
				return value;
			return null;
		}
		public static PropertyDescriptorCollection GetDataSourceItemProperties(IEnumerable data) {
			ITypedList typedList = data as ITypedList;
			if(typedList != null)
				return typedList.GetItemProperties(null);
			Type itemType = GetDataSourceItemType(data);
			if(itemType == null)
				return null;
			return TypeDescriptor.GetProperties(itemType);			
		}
		internal static Type GetDataSourceItemType(IEnumerable data) {
			Type type = data.GetType();
			if(type.IsArray)
				return type.GetElementType();
			System.Reflection.PropertyInfo[] props = type.GetProperties();
			for(int i = 0; i < props.Length; i++) {
				if(props[i].Name == "Item" && props[i].PropertyType != typeof(Object))
					return props[i].PropertyType;
			}			
			IEnumerator enumerator = data.GetEnumerator();
			if(enumerator.MoveNext() && enumerator.Current != null)
				return enumerator.Current.GetType();
			return null;
		}
		public static bool TestParentChildRelationship(TreeListNode parent, TreeListNode child) {
			TreeListNode node = child;
			while(node != null) {
				if(node == parent)
					return true;
				node = node.ParentNode;
			}
			return false;
		}		
		public static string EscapeNodeKey(string value) {
			if(value == null) return null;
			return Regex.Replace(value, "[^0-9A-Za-z]", EscapeNodeKeyMatchEvaluator);
		}
		static string EscapeNodeKeyMatchEvaluator(Match match) {
			return "_" + (int)match.Value[0] + "_";
		}
		public static string UnescapeNodeKey(string value) {
			if(value == null) return null;
			return Regex.Replace(value, "_\\d+_", delegate(Match match) {				
				int code = int.Parse(match.Value.Substring(1, match.Value.Length - 2));
				return ((char)code).ToString();
			});
		}
		public static TreeListCommandColumn FindCommandColumnForEditForm(ASPxTreeList treeList) {
			TreeListCommandColumn weakMatch = null;
			foreach(TreeListColumn column in treeList.Columns) {
				TreeListCommandColumn commandColumn = column as TreeListCommandColumn;
				if(commandColumn == null) continue;
				if(commandColumn.UpdateButton.Visible)
					return commandColumn;
				weakMatch = commandColumn;
			}
			return weakMatch;
		}
		public static bool IsSummaryVisible(TreeListColumn column, string showInColumnValue) {
			if(string.IsNullOrEmpty(showInColumnValue))
				return false;
			if(!string.IsNullOrEmpty(column.Name))
				return showInColumnValue == column.Name;
			if(column.Caption == showInColumnValue)
				return true;
			TreeListDataColumn dataCol = column as TreeListDataColumn;
			if(dataCol != null && dataCol.FieldName == showInColumnValue)
				return true;
			return showInColumnValue == column.ToString();
		}
	}
	public class TreeListEditorValuesReader {
		int readerPos;
		string serializedValues;
		ASPxTreeList treeList;
		public TreeListEditorValuesReader(ASPxTreeList treeList) {
			this.treeList = treeList;
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected TreeListDataHelper DataHelper { get { return TreeList.TreeDataHelper; } }
		protected string SerializedValues { get { return serializedValues; } }
		public void Read(string state, bool ignoreInvalidValues) {			
			if(String.IsNullOrEmpty(state)) return;
			this.readerPos = 0;
			this.serializedValues = state;
			int count = Int32.Parse(ReadToSeparator());
			while(count-- > 0) {
				int columnIndex = Int32.Parse(ReadToSeparator());
				int length = Int32.Parse(ReadToSeparator());
				object value = null;
				if(length > -1)
					value = Read(length);
				else
					Read(0);
				TreeListDataColumn column = TreeList.Columns[columnIndex] as TreeListDataColumn;
				if(column == null)
					continue;
				try {
					DataHelper.SetEditingValue(column.FieldName, value);
				} catch {
					if(!ignoreInvalidValues)
						throw;
				}
			}			
		}
		string ReadToSeparator() {
			int pos = SerializedValues.IndexOf(TreeListRenderHelper.SeparatorToken, this.readerPos);
			string result = pos < 0 
				? SerializedValues.Substring(this.readerPos)
				: SerializedValues.Substring(this.readerPos, pos - this.readerPos);
			this.readerPos = 1 + pos;
			return result;
		}
		string Read(int length) {
			string result = SerializedValues.Substring(this.readerPos, length);
			this.readerPos += 1 + length;
			return result;
		}
	}
	public class TreeListExportHelper {
		ASPxTreeList treeList;
		public TreeListExportHelper(ASPxTreeList treeList) {
			this.treeList = treeList;
		}
		public TreeListRenderHelper RenderHelper { get { return treeList.RenderHelper; } }
		public TreeListRowsCreationFlags RowsCreationFlags {
			get { return treeList.TreeDataHelper.RowsCreationFlags; }
			set { treeList.TreeDataHelper.RowsCreationFlags = value; }
		}
		public static string GetColumnCaption(TreeListColumn column) {
			return column.GetCaption();
		}
	}
	public static class TreeListBuilderHelper {
		public static void Build(ITreeListBuilder builder) {
			Build(builder, TreeListDataTableRenderPart.All);
		}
		public static void Build(ITreeListBuilder builder, TreeListDataTableRenderPart part) {
			TreeListRenderHelper helper = builder.RenderHelper;
			switch(part) {
				case TreeListDataTableRenderPart.Header:
					CreateHeader(builder, helper);
					break;
				case TreeListDataTableRenderPart.Content:
					CreateContent(builder, helper);
					break;
				case TreeListDataTableRenderPart.Footer:
					CreateFooter(builder, helper);
					break;
				case TreeListDataTableRenderPart.All:
					CreateHeader(builder, helper);
					CreateContent(builder, helper);
					CreateFooter(builder, helper);
					break;
			}
		}
		static void CreateHeader(ITreeListBuilder builder, TreeListRenderHelper helper) {
			if(helper.IsHeaderRowVisible)
				builder.CreateHeader();
		}
		static void CreateFooter(ITreeListBuilder builder, TreeListRenderHelper helper) {
			if(helper.IsTotalFooterVisible)
				builder.CreateFooter(TreeListRootRowInfo.RowIndex);
		}
		static void CreateContent(ITreeListBuilder builder, TreeListRenderHelper helper) {
			Stack<int> stack = new Stack<int>();
			int count = helper.Rows.Count;
			for(int i = 0; i < count; i++) {
				TreeListRowInfo row = helper.Rows[i];
				if(helper.IsGroupFooterVisible) {
					while(stack.Count > 0 && row.Indents.Count <= helper.Rows[stack.Peek()].Indents.Count)
						builder.CreateGroupFooter(stack.Pop());
					if(row.HasButton && row.Expanded && i < count - 1)
						stack.Push(i);
				}
				if(helper.IsEditingKey(row.NodeKey))
					CreateEditRows(builder, i);
				else {
					builder.CreateDataRow(i);
					if(helper.IsPreviewRowVisible)
						builder.CreatePreview(i);
				}
			}
			if(helper.IsGroupFooterVisible) {
				while(stack.Count > 0)
					builder.CreateGroupFooter(stack.Pop());
			}
		}
		static void CreateEditRows(ITreeListBuilder builder, int rowIndex) {
			TreeListRenderHelper helper = builder.RenderHelper;
			switch(helper.EditMode) {
				case TreeListEditMode.Inline:
					builder.CreateInlineEditRow(rowIndex);
					break;
				case TreeListEditMode.EditForm:
					builder.CreateEditFormRow(rowIndex, false);
					break;
				case TreeListEditMode.EditFormAndDisplayNode:
					if(helper.IsNewNodeEditing) {
						builder.CreateEditFormRow(rowIndex, false);
					} else {
						builder.CreateDataRow(rowIndex);
						builder.CreateEditFormRow(rowIndex, true);
					}
					break;
				case TreeListEditMode.PopupEditForm:
					builder.CreateDataRow(rowIndex);
					break;
				default:
					throw new NotImplementedException();
			}
			if(helper.EditMode != TreeListEditMode.PopupEditForm)
				builder.CreateErrorRow(rowIndex);
		}
		public static int GetAuxRowCount(TreeListRenderHelper helper, string nodeKey) {
			return GetAuxRowCount(helper, nodeKey, false);
		}
		public static int GetAuxRowCount(TreeListRenderHelper helper, string nodeKey, bool skipErrorRow) {
			if(nodeKey == TreeListRenderHelper.NewNodeKey && !helper.IsNewNodeEditing)
				throw new InvalidOperationException();
			int count = 0;
			if(helper.IsEditingKey(nodeKey)) {
				if(!skipErrorRow)
					count++; 
				if(nodeKey != null && helper.EditMode == TreeListEditMode.EditFormAndDisplayNode)
					count++; 
			} else {
				if(helper.IsPreviewRowVisible)
					count++; 
			}
			return count;
		}
	}
	public interface ITreeListBuilder {
		TreeListRenderHelper RenderHelper { get; }
		void CreateHeader();
		void CreateDataRow(int rowIndex);
		void CreatePreview(int rowIndex);
		void CreateGroupFooter(int rowIndex);
		void CreateFooter(int rowIndex);
		void CreateInlineEditRow(int rowIndex);
		void CreateEditFormRow(int rowIndex, bool isAuxRow);
		void CreateErrorRow(int rowIndex);
	}
	public class TreeListEditFormLayout {
		public const int
			EmptyCell = -1,
			SkipCell = -2;
		protected class Row : List<int> {
			public Row(int columnCount)
				: base(columnCount) {
				for(int i = 0; i < columnCount; i++)
					Add(EmptyCell);
			}
		}
		int columnCount;
		List<Row> rows;
		public TreeListEditFormLayout(int columnCount) {
			this.columnCount = columnCount;
			this.rows = new List<Row>();
		}
		public int ColumnCount { get { return columnCount; } }
		public int RowCount { get { return Rows.Count; } }
		public int this[int rowIndex, int cellIndex] {
			get {
				if(rowIndex > RowCount - 1)
					return EmptyCell;
				return Rows[rowIndex][cellIndex];
			}
			set {
				EnsureRowCapacity(1 + rowIndex);
				Rows[rowIndex][cellIndex] = value;
			}
		}
		protected List<Row> Rows { get { return rows; } }
		public void Allocate(int colSpan, int rowSpan, int value) {
			int rowIndex, cellIndex;
			FindEmptyCell(out rowIndex, out cellIndex);						
			for(int y = rowIndex; y < rowIndex + rowSpan; y++) {
				for(int x = cellIndex; x < cellIndex + colSpan; x++) {
					if(x > ColumnCount - 1)
						continue;
					this[y, x] = x > cellIndex|| y > rowIndex ? SkipCell : value;
				}
			}
		}
		void FindEmptyCell(out int rowIndex, out int cellIndex) {
			for(int y = 0; y < RowCount; y++) {
				for(int x = 0; x < ColumnCount; x++) {
					if(this[y, x] == EmptyCell) {
						rowIndex = y;
						cellIndex = x;
						return;
					}
				}
			}
			rowIndex = RowCount;
			cellIndex = 0;
			return;
		}
		void EnsureRowCapacity(int requiredCount) {
			int deficit = requiredCount - RowCount;
			if(deficit < 1)
				return;
			for(int i = 0; i < deficit; i++)
				Rows.Add(new Row(ColumnCount));
		}
	}
	public class TreeListEditFormLayoutCalculator {
		ASPxTreeList treeList;
		List<TreeListDataColumn> editableColumns;
		public TreeListEditFormLayoutCalculator(ASPxTreeList treeList) {
			this.treeList = treeList;
			this.editableColumns = new List<TreeListDataColumn>();			
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected List<TreeListDataColumn> EditableColumns { get { return editableColumns; } }
		public TreeListEditFormLayout Calculate() {
			BuildEditableColumnList();
			int columnCount = Math.Min(TreeList.SettingsEditing.EditFormColumnCount, EditableColumns.Count);
			TreeListEditFormLayout layout = new TreeListEditFormLayout(columnCount);
			foreach(TreeListDataColumn column in EditableColumns)
				layout.Allocate(column.EditFormSettings.ColumnSpan, column.EditFormSettings.RowSpan, column.Index);
			return layout;
		}
		void BuildEditableColumnList() {
			EditableColumns.Clear();
			foreach(TreeListColumn column in TreeList.Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn == null) continue;
				if(IsColumnEditable(dataColumn))
					EditableColumns.Add(dataColumn);
			}
			EditableColumns.Sort(CompareByVisibleIndex);
		}
		int CompareByVisibleIndex(TreeListDataColumn x, TreeListDataColumn y) {
			if(x == y) return 0;
			int index1 = x.EditFormSettings.VisibleIndex;
			int index2 = y.EditFormSettings.VisibleIndex;
			if(index1 < 0) index1 = x.VisibleIndex;
			if(index2 < 0) index2 = y.VisibleIndex;
			return Comparer.Default.Compare(index1, index2);
		}
		bool IsColumnEditable(TreeListDataColumn column) {
			switch(column.EditFormSettings.Visible) {
				case DefaultBoolean.True:
					return true;
				case DefaultBoolean.False:
					return false;
				default:
					return column.Visible;
			}
		}	
	}	
	public struct TreeListTemplateRegistration {
		public Control Container;
		public TreeListColumn Column;
		public string NodeKey;
		public TreeListTemplateRegistration(Control container)
			: this(container, null, null) {
		}
		public TreeListTemplateRegistration(Control container, string nodeKey)
			: this(container, null, nodeKey) {
		}
		public TreeListTemplateRegistration(Control container, TreeListColumn column)
			: this(container, column, null) {
		}
		public TreeListTemplateRegistration(Control container, TreeListColumn column, string nodeKey) {
			Container = container;
			Column = column;
			NodeKey = nodeKey;
		}
	}
	public class TreeListTemplateHelper {
		ASPxTreeList treeList;		
		public TreeListTemplateHelper(ASPxTreeList treeList) {
			this.treeList = treeList;
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		List<TreeListTemplateRegistration> headerCaptionContainers;
		public List<TreeListTemplateRegistration> HeaderCaptionContainers {
			get {
				if(headerCaptionContainers == null)
					headerCaptionContainers = new List<TreeListTemplateRegistration>();
				return headerCaptionContainers;
			}
		}
		public string GetHeaderCaptionContainerId(TreeListColumn column) {
			return String.Format("{0}{1}", TreeListRenderHelper.HeaderCaptionTemplatePrefix, column.Index);
		}
		List<TreeListTemplateRegistration> dataCellContainers;
		public List<TreeListTemplateRegistration> DataCellContainers {
			get {
				if(dataCellContainers == null)
					dataCellContainers = new List<TreeListTemplateRegistration>();
				return dataCellContainers;
			}
		}
		public string GetDataCellContainerId(TreeListColumn column, string nodeKey) {
			return String.Format("{0}{1}_{2}", TreeListRenderHelper.DataCellTemplatePrefix, column.Index, TreeListUtils.EscapeNodeKey(nodeKey));
		}
		List<TreeListTemplateRegistration> previewContainers;
		public List<TreeListTemplateRegistration> PreviewContainers {
			get {
				if(previewContainers == null)
					previewContainers = new List<TreeListTemplateRegistration>();
				return previewContainers;
			}
		}
		public string GetPreviewContainerId(string nodeKey) {
			return TreeListRenderHelper.PreviewCellTemplatePrefix + TreeListUtils.EscapeNodeKey(nodeKey);
		}
		List<TreeListTemplateRegistration> groupFooterContainers;
		public List<TreeListTemplateRegistration> GroupFooterContainers {
			get {
				if(groupFooterContainers == null)
					groupFooterContainers = new List<TreeListTemplateRegistration>();
				return groupFooterContainers;
			}
		}
		public string GetGroupFooterContainerId(TreeListColumn column, string nodeKey) {
			return String.Format("{0}{1}_{2}", TreeListRenderHelper.GroupFooterCellTemplatePrefix, column.Index, TreeListUtils.EscapeNodeKey(nodeKey));
		}
		List<TreeListTemplateRegistration> footerContainers;
		public List<TreeListTemplateRegistration> FooterContainers {
			get {
				if(footerContainers == null)
					footerContainers = new List<TreeListTemplateRegistration>();
				return footerContainers;
			}
		}
		public string GetFooterContainerId(TreeListColumn column) {
			return TreeListRenderHelper.FooterCellTemplatePrefix + column.Index;
		}
		List<TreeListTemplateRegistration> editCellContainers;
		public List<TreeListTemplateRegistration> EditCellContainers {
			get {
				if(editCellContainers == null)
					editCellContainers = new List<TreeListTemplateRegistration>();
				return editCellContainers;
			}
		}
		public string GetEditCellContainerId(TreeListColumn column, string nodeKey) {
			return String.Format("{0}{1}_{2}", TreeListRenderHelper.EditCellTemplatePrefix, column.Index, TreeListUtils.EscapeNodeKey(nodeKey));
		}
		List<TreeListTemplateRegistration> editFormContainers;
		public List<TreeListTemplateRegistration> EditFormContainers {
			get {
				if(editFormContainers == null)
					editFormContainers = new List<TreeListTemplateRegistration>();
				return editFormContainers;
			}
		}
		public string GetEditFormContainerId(string nodeKey) {
			return String.Format("{0}_{1}", TreeListRenderHelper.EditFormTemplateID, TreeListUtils.EscapeNodeKey(nodeKey));
		}
		public void Reset() {
			this.headerCaptionContainers = null;
			this.dataCellContainers = null;
			this.previewContainers = null;
			this.groupFooterContainers = null;
			this.footerContainers = null;
			this.editCellContainers = null;
			this.editFormContainers = null;
		}
		public static Control Find(List<TreeListTemplateRegistration> containers, TreeListColumn column, string nodeKey, string id) {
			foreach(TreeListTemplateRegistration item in containers) {
				if(column != null && item.Column != column)
					continue;
				if(item.NodeKey != nodeKey)
					continue;
				Control result = item.Container.FindControl(id);
				if(result != null)
					return result;
			}
			return null;
		}
	}
	public class TreeListCommandButtonsHelper {
		public TreeListCommandButtonsHelper() {
			CommandButtonList = new List<ASPxButton>();
		}
		public bool HasButtons { get { return CommandButtonList.Count > 0; } }
		protected List<ASPxButton> CommandButtonList { get; private set; }
		protected ASPxTreeList TreeList { get; private set; }
		public void Register(ASPxButton button) {
			CommandButtonList.Add(button);
		}
		public void Invalidate() {
			CommandButtonList.Clear();
		}
		public object GetClientIDs() {
			return CommandButtonList.Select(b => b.ClientID);
		}
	}
}
