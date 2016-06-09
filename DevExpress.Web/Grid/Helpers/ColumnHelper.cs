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

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.Web.Internal {
	public abstract class GridColumnHelper {
		ReadOnlyCollection<IWebGridColumn> allColumns;
		ReadOnlyCollection<IWebGridColumn> allVisibleColumns;
		ReadOnlyCollection<IWebGridDataColumn> allDataColumns;
		ReadOnlyCollection<IWebGridDataColumn> allVisibleDataColumns;
		ReadOnlyCollection<IWebGridDataColumn> filterControlColumns;
		FilterControlColumnCollection filterControlCachedColumns;
		ReadOnlyCollection<IWebGridDataColumn> editColumns;
		ReadOnlyCollection<IWebGridDataColumn> batchEditTemplateColumns;
		ReadOnlyCollection<IWebGridColumn> bandsForCustWindow;
		ReadOnlyCollection<IWebGridColumn> leafsForCustWindow;
		ReadOnlyCollection<IWebGridDataColumn> searchPanelColumns;
		ReadOnlyCollection<DataColumnInfo> searchPanelColumnInfos;
		GridColumnVisualTreeNode visualTree;
		List<GridColumnVisualTreeNode> leafs;
		List<List<GridColumnVisualTreeNode>> layout;
		Dictionary<IWebGridColumn, int> globalIndexTable;
		public GridColumnHelper(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		public ReadOnlyCollection<IWebGridColumn> AllColumns {
			get {
				if(allColumns == null) {
					var list = new List<IWebGridColumn>();
					PopulateAllColumnsList(Grid.Columns, list);
					allColumns = list.AsReadOnly();
				}
				return allColumns;
			}
		}
		public ReadOnlyCollection<IWebGridColumn> AllVisibleColumns {
			get {
				if(allVisibleColumns == null) {
					var list = new List<IWebGridColumn>();
					PopulateAllVisibleColumnsList(Grid, list);
					allVisibleColumns = list.AsReadOnly();
				}
				return allVisibleColumns;
			}
		}
		public ReadOnlyCollection<IWebGridDataColumn> AllDataColumns {
			get {
				if(allDataColumns == null)
					allDataColumns = CreateAllDataColumnsList().AsReadOnly();
				return allDataColumns;
			}
		}
		public ReadOnlyCollection<IWebGridDataColumn> AllVisibleDataColumns {
			get {
				if(allVisibleDataColumns == null) {
					var list = AllVisibleColumns.Intersect(AllDataColumns).OfType<IWebGridDataColumn>().ToList();
					allVisibleDataColumns = list.AsReadOnly();
				}
				return allVisibleDataColumns;
			}
		}
		public ReadOnlyCollection<IWebGridDataColumn> FilterControlColumns {
			get {
				if(filterControlColumns == null)
					filterControlColumns = CreateFilterControlsColumnsList().AsReadOnly();
				return filterControlColumns;
			}
		}
		public FilterControlColumnCollection FilterControlCachedColumns {
			get {
				if(filterControlCachedColumns == null)
					filterControlCachedColumns = Grid.CreateFilterControlColumnCollection();
				return filterControlCachedColumns;
			}
		}
		public ReadOnlyCollection<IWebGridDataColumn> EditColumns {
			get {
				if(editColumns == null)
					editColumns = CreateEditColumnsList().AsReadOnly();
				return editColumns;
			}
		}
		public ReadOnlyCollection<IWebGridDataColumn> BatchEditTemplateColumns {
			get {
				if(batchEditTemplateColumns == null)
					batchEditTemplateColumns = CreateBatchEditTemplateColumnsList().AsReadOnly();
				return batchEditTemplateColumns;
			}
		}
		public virtual ReadOnlyCollection<IWebGridColumn> BandsForCustWindow {
			get {
				if(bandsForCustWindow == null)
					PopulateCustWindowColumns();
				return bandsForCustWindow;
			}
		}
		public virtual ReadOnlyCollection<IWebGridColumn> LeafsForCustWindow {
			get {
				if(leafsForCustWindow == null)
					PopulateCustWindowColumns();
				return leafsForCustWindow;
			}
		}
		public List<IWebGridColumn> CustWindowColumns { get { return BandsForCustWindow.Concat(LeafsForCustWindow).ToList(); } }
		public ReadOnlyCollection<IWebGridDataColumn> SearchPanelColumns {
			get {
				if(searchPanelColumns == null)
					searchPanelColumns = CreateSearchPanelColumnList().AsReadOnly();
				return searchPanelColumns;
			}
		}
		public ReadOnlyCollection<DataColumnInfo> SearchPanelColumnInfos {
			get {
				if(searchPanelColumnInfos == null)
					searchPanelColumnInfos = SearchPanelColumns.Select(c => c.Adapter.GetColumnInfo()).Where(c => c != null).ToList().AsReadOnly();
				return searchPanelColumnInfos;
			}
		}
		protected internal GridColumnVisualTreeNode VisualTree {
			get {
				if(visualTree == null)
					visualTree = CreateVisualTree();
				return visualTree;
			}
		}
		public List<GridColumnVisualTreeNode> Leafs {
			get {
				if(leafs == null)
					leafs = FindLeafNodes(VisualTree);
				return leafs;
			}
		}
		public List<List<GridColumnVisualTreeNode>> Layout {
			get {
				if(layout == null) {
					layout = new List<List<GridColumnVisualTreeNode>>();
					PopulateLayout(VisualTree, 0, layout);
				}
				return layout;
			}
		}
		public virtual void Invalidate() {
			this.allColumns = null;
			this.globalIndexTable = null;
			this.allVisibleColumns = null;
			this.allDataColumns = null;
			this.allVisibleDataColumns = null;
			this.filterControlColumns = null;
			this.filterControlCachedColumns = null;
			this.editColumns = null;
			this.batchEditTemplateColumns = null;
			this.bandsForCustWindow = null;
			this.leafsForCustWindow = null;
			this.searchPanelColumns = null;
			this.searchPanelColumnInfos = null;
			this.visualTree = null;
			this.leafs = null;
			this.layout = null;
		}
		public IWebGridColumn FindColumnByKey(string key) {
			int index = 0;
			if(Int32.TryParse(key, out index)) {
				if(index > -1 && index < AllColumns.Count)
					return AllColumns[index];
			}
			return FindColumnByString(key);
		}
		public IWebGridColumn FindColumnByString(string caption) {
			return FindColumnByStringRecursive(Grid.Columns, caption);
		}
		public static IWebGridColumn FindColumnByStringRecursive(IEnumerable collection, string caption) {
			return FindColumnByStringRecursiveInternal(collection, caption);
		}
		protected internal static IWebGridColumn FindColumnByStringRecursiveInternal(IEnumerable collection, string caption) {
			foreach(IWebGridColumn column in collection) {
				if(column.Name == caption)
					return column;
			}
			foreach(IWebGridColumn column in collection) {
				var dataColumn = column as IWebGridDataColumn;
				if(dataColumn != null && dataColumn.FieldName == caption)
					return column;
			}
			foreach(IWebGridColumn column in collection) {
				var dataColumn = column as IWebGridDataColumn;
				if(column.Caption == caption || dataColumn != null && dataColumn.Adapter.GetDisplayName() == caption)
					return column;
			}
			foreach(IWebGridColumn column in collection) {
				var band = column as IWebColumnsOwner;
				if(band == null)
					continue;
				var innerResult = FindColumnByStringRecursiveInternal(band.Columns, caption);
				if(innerResult != null)
					return innerResult;
			}
			return null;
		}
		public int GetColumnGlobalIndex(IWebGridColumn column) {
			if(AllColumns.Count < 5)
				return AllColumns.IndexOf(column);
			if(this.globalIndexTable == null) {
				this.globalIndexTable = new Dictionary<IWebGridColumn, int>(AllColumns.Count);
				for(int i = 0; i < AllColumns.Count; i++)
					this.globalIndexTable.Add(AllColumns[i], i);
			}
			if(!this.globalIndexTable.ContainsKey(column))
				return -1;
			return this.globalIndexTable[column];
		}
		public GridColumnVisualTreeNode FindVisualTreeNode(IWebGridColumn column) {
			return FindVisualTreeNodeRecursive(VisualTree, column);
		}
		public virtual bool IsLeaf(IWebGridColumn column) {
			foreach(var node in Leafs) {
				if(node.Column == column)
					return true;
			}
			return false;
		}
		static GridColumnVisualTreeNode FindVisualTreeNodeRecursive(GridColumnVisualTreeNode root, IWebGridColumn column) {
			foreach(var child in root.Children) {
				if(child.Column == column)
					return child;
				var result = FindVisualTreeNodeRecursive(child, column);
				if(result != null)
					return result;
			}
			return null;
		}
		static void PopulateAllColumnsList(WebColumnCollectionBase collection, List<IWebGridColumn> result) {
			foreach(IWebGridColumn item in collection) {
				result.Add(item);
				var band = item as IWebColumnsOwner;
				if(band != null)
					PopulateAllColumnsList(band.Columns, result);
			}
		}
		static void PopulateAllVisibleColumnsList(IWebColumnsOwner columnsOwner, List<IWebGridColumn> result) {
			foreach(IWebGridColumn item in columnsOwner.GetVisibleColumns()) {
				result.Add(item);
				var band = item as IWebColumnsOwner;
				if(band != null)
					PopulateAllVisibleColumnsList(band, result);
			}
		}
		List<IWebGridDataColumn> CreateAllDataColumnsList() {
			var result = new List<IWebGridDataColumn>();
			foreach(IWebGridColumn column in AllColumns) {
				var dataColumn = column as IWebGridDataColumn;
				if(dataColumn != null)
					result.Add(dataColumn);
			}
			return result;
		}
		List<IWebGridDataColumn> CreateFilterControlsColumnsList() {
			return AllDataColumns.Where(c => c.Adapter.ShowInFilterControl).ToList();
		}
		List<IWebGridDataColumn> CreateEditColumnsList() {
			var result = new List<IWebGridDataColumn>();
			foreach(var leaf in Leafs) {
				var column = leaf.Column as IWebGridDataColumn;
				if(column != null && CanEditColumn(column))
					result.Add(column);
			}
			return result;
		}
		protected virtual bool CanEditColumn(IWebGridDataColumn column) {
			var prop = Grid.RenderHelper.GetColumnEdit(column);
			if(prop is ImageEditProperties && !HasEditTemplate(column))
				return false;
			return true; 
		}
		protected List<IWebGridDataColumn> CreateBatchEditTemplateColumnsList() {
			return EditColumns.Where(c => HasEditTemplate(c)).ToList();
		}
		protected abstract bool HasEditTemplate(IWebGridDataColumn column);
		List<IWebGridDataColumn> CreateSearchPanelColumnList() {
			var result = new List<IWebGridDataColumn>();
			var names = CalcSearchPanelColumnNames();
			foreach(var column in AllVisibleDataColumns.Where(c => c.Adapter.AllowSearchPanelFilter)) {
				var allow = column.Adapter.Settings.AllowFilterBySearchPanel;
				var nameAssigned = names.Contains(column.FieldName) || names.Contains(column.Caption) || names.Contains(column.Name);
				if (IsAllowSearchByColumn(allow, nameAssigned))
					result.Add(column);
			}
			return result;
		}
		public static bool IsAllowSearchByColumn(DefaultBoolean allowFilterBySearchPanel, bool nameAssigned) {
			return allowFilterBySearchPanel == DefaultBoolean.Default && nameAssigned || allowFilterBySearchPanel == DefaultBoolean.True;
		}
		internal List<string> CalcSearchPanelColumnNames() {
			return CalcSearchPanelColumnNames(AllVisibleDataColumns.OfType<IWebColumnInfo>(), Grid.SettingsSearchPanel.ColumnNames);
		}
		public static List<string> CalcSearchPanelColumnNames(IEnumerable<IWebColumnInfo> visibleColumns, string columnNames) {
			var nameString = columnNames.Trim();
			if (string.IsNullOrEmpty(nameString))
				return new List<string>();
			if (nameString == "*")
				return visibleColumns.Select(c => c.FieldName).ToList();
			return nameString.Split(';').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToList();
		}
		void PopulateCustWindowColumns() {
			Hashtable hash = new Hashtable(AllVisibleColumns.Count);
			foreach(var column in AllVisibleColumns)
				hash[column] = true;
			var bands = new List<IWebGridColumn>();
			var leafs = new List<IWebGridColumn>();
			foreach(var column in AllColumns) {
				if(hash.ContainsKey(column))
					continue;
				if(column.Visible)
					continue;
				if(!(column as WebColumnBase).ShowInCustomizationForm)
					continue;
				var band = column as IWebColumnsOwner;
				if(band != null) {
					bands.Add(column);
				} else {
					var dataColumn = column as IWebGridDataColumn;
					if(dataColumn == null || dataColumn.Adapter.GroupIndex < 0 || ShowGroupedColumns)
						leafs.Add(column);
				}
			}
			leafs.Sort(CompareColumnsForCustWindow);
			bands.Sort(CompareColumnsForCustWindow);
			this.leafsForCustWindow = leafs.AsReadOnly();
			this.bandsForCustWindow = bands.AsReadOnly();
		}
		protected virtual bool ShowGroupedColumns { get { return false; } } 
		int CompareColumnsForCustWindow(IWebGridColumn x, IWebGridColumn y) {
			if(object.Equals(x, y))
				return 0;
			int result = Comparer.Default.Compare(x.ToString(), y.ToString());
			if(result == 0)
				result = Comparer.Default.Compare(AllColumns.IndexOf(x), AllColumns.IndexOf(y));
			return result;
		}
		protected internal virtual bool UseColumnInVisualTree(IWebGridColumn column) {
			return true;
		}
		protected GridColumnVisualTreeNode CreateVisualTree() {
			var root = CreateTreeNode(null, null);
			PopulateVisualNodeChildren(root, Grid);
			PopulateVisualNodeSpans(root);
			return root;
		}
		static void PopulateVisualNodeSpans(GridColumnVisualTreeNode root) {
			var leafs = FindLeafNodes(root);
			int height = 0;
			foreach(GridColumnVisualTreeNode leaf in leafs) {
				leaf.ColSpan = 1;
				var current = leaf.Parent;
				while(current != null) {
					current.ColSpan++;
					current.RowSpan = 1;
					leaf.RowSpan++;
					current = current.Parent;
				}
				height = Math.Max(height, leaf.RowSpan);
			}
			foreach(GridColumnVisualTreeNode leaf in leafs)
				leaf.RowSpan = height - leaf.RowSpan + 1;
		}
		void PopulateVisualNodeChildren(GridColumnVisualTreeNode node, IWebColumnsOwner columnsOwner) {
			foreach(IWebGridColumn column in columnsOwner.GetVisibleColumns()) {
				if(!UseColumnInVisualTree(column))
					continue;
				var child = CreateTreeNode(column, node);
				node.Children.Add(child);
				var band = column as IWebColumnsOwner;
				if(band != null)
					PopulateVisualNodeChildren(child, band);
			}
		}
		protected virtual GridColumnVisualTreeNode CreateTreeNode(IWebGridColumn column, GridColumnVisualTreeNode parent) {
			return new GridColumnVisualTreeNode(column, parent);
		}
		static void PopulateLayout(GridColumnVisualTreeNode node, int currentLevel, List<List<GridColumnVisualTreeNode>> layout) {
			foreach(var child in node.Children) {
				while(layout.Count < currentLevel + 1)
					layout.Add(new List<GridColumnVisualTreeNode>());
				layout[currentLevel].Add(child);
				PopulateLayout(child, 1 + currentLevel, layout);
			}
		}
		protected static List<GridColumnVisualTreeNode> FindLeafNodes(GridColumnVisualTreeNode root) {
			var result = new List<GridColumnVisualTreeNode>();
			PopulateLeafNodes(root, result);
			return result;
		}
		static void PopulateLeafNodes(GridColumnVisualTreeNode node, List<GridColumnVisualTreeNode> result) {
			foreach(GridColumnVisualTreeNode child in node.Children) {
				if(child.Children.Count < 1)
					result.Add(child);
				else
					PopulateLeafNodes(child, result);
			}
		}
	}
	public class GridColumnVisualTreeNode {
		public GridColumnVisualTreeNode(IWebGridColumn column, GridColumnVisualTreeNode parent) {
			Column = column;
			Parent = parent;
			Children = new List<GridColumnVisualTreeNode>();
		}
		public IWebGridColumn Column { get; private set; }
		public GridColumnVisualTreeNode Parent { get; private set; }
		public List<GridColumnVisualTreeNode> Children { get; private set; }
		public int ColSpan { get; set; }
		public int RowSpan { get; set; }
	}
	public class GridUniqueColumnInfo {
		public GridUniqueColumnInfo(IEnumerable<IWebGridColumn> columns) {
			Columns = columns;
			NotUniqueNames = new HashSet<string>();
			NotUniqueFieldNames = new HashSet<string>();
			NotUniqueCaptions = new HashSet<string>();
			Build();
		}
		protected IEnumerable<IWebGridColumn> Columns { get; private set; }
		protected HashSet<string> NotUniqueNames { get; private set; }
		protected HashSet<string> NotUniqueFieldNames { get; private set; }
		protected HashSet<string> NotUniqueCaptions { get; private set; }
		public string GetUniqueColumnName(IWebGridColumn column) {
			IWebGridDataColumn dataColumn = column as IWebGridDataColumn;
			if(!string.IsNullOrEmpty(column.Name) && !NotUniqueNames.Contains(column.Name))
				return column.Name;
			if(dataColumn != null && !string.IsNullOrEmpty(dataColumn.FieldName) && !NotUniqueFieldNames.Contains(dataColumn.FieldName))
				return dataColumn.FieldName;
			if(!string.IsNullOrEmpty(column.Caption) && !NotUniqueCaptions.Contains(column.Caption))
				return column.Caption;
			return string.Empty;
		}
		protected void Build() {
			var names = new HashSet<string>();
			var fieldNames = new HashSet<string>();
			var captions = new HashSet<string>();
			foreach(var column in Columns) {
				AddNotUniqueName(names, NotUniqueNames, column.Name);
				if(column is IWebGridDataColumn)
					AddNotUniqueName(fieldNames, NotUniqueFieldNames, ((IWebGridDataColumn)column).FieldName);
				AddNotUniqueName(captions, NotUniqueCaptions, column.Caption);
			}
		}
		protected void AddNotUniqueName(HashSet<string> usedNames, HashSet<string> notUniqueNames, string name) {
			if(string.IsNullOrEmpty(name))
				return;
			if(usedNames.Contains(name))
				notUniqueNames.Add(name);
			usedNames.Add(name);
		}
	}
}
