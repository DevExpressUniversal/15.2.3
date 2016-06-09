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
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraVerticalGrid.ViewInfo;
using System.Collections.Generic;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraVerticalGrid.Internal;
namespace DevExpress.XtraVerticalGrid.Rows {
	public class VGridRowsIterator {
		protected VGridControlBase grid;
		internal VGridRowsIterator(VGridControlBase grid, bool inner) : this(grid) {}
		protected VGridRowsIterator(VGridControlBase grid) {
			if(grid == null) throw new NullReferenceException("Can't create VGridRowsIterator instance.");
			this.grid = grid;
		}
		public virtual void DoOperation(RowOperation operation) {
			DoLocalOperation(operation, grid.Rows);
		}
		public virtual void DoLocalOperation(RowOperation operation, VGridRows rows) {
			if(rows == null) return;
			if(operation == null) return;
			if(rows.Grid != grid && rows.Grid != null) return;
			try {
				operation.Init();
				if(operation.NeedsFullIteration)
					VisitAllNodes(rows, operation);
				else
					VisitParentNodes(rows, operation);
			}
			finally {
				operation.Release();
			}
		}
		public virtual void DoLocalOperation(RowOperation operation, BaseRow row) {
			if(row == null) return;
			if(operation == null) return;
			if(row.Grid != grid && row.IsConnected) return;
			try {
				operation.Init();
				if(!operation.CanContinueIteration(row)) return;
				operation.Execute(row);
				if(NeedsVisitChildren(row, operation))
					DoLocalOperation(operation, row.ChildRows);
			} finally {
				operation.Release();
			}
		}
		protected virtual void VisitAllNodes(VGridRows rows, RowOperation operation) {
			foreach(BaseRow row in rows) {
				if(!operation.CanContinueIteration(row))
					return;
				operation.Execute(row);
				if(!operation.CanContinueIteration(row))
					return;
				if(row.HasChildren) {
					if(NeedsVisitChildren(row, operation))
						VisitAllNodes(row.ChildRows, operation);
				}
			}
		}
		protected virtual void VisitParentNodes(VGridRows rows, RowOperation operation) {
			foreach(BaseRow row in rows) {
				if(!operation.CanContinueIteration(row))
					return;
				if(row.HasChildren) {
					operation.Execute(row);
					if(!operation.CanContinueIteration(row))
						return;
					if(NeedsVisitChildren(row, operation))
						VisitParentNodes(row.ChildRows, operation);
				}
			}
		}
		bool NeedsVisitChildren(BaseRow row, RowOperation operation) {
			return operation.NeedsVisitChildren(row) && (row.ChildRows.IsLoaded || (!row.ChildRows.IsLoaded && operation.NeedsVisitUnloadedRows));
		}
	}
	public abstract class RowOperation {
		public abstract void Execute(BaseRow row);
		public virtual bool CanContinueIteration(BaseRow row) { return true; }
		public virtual bool NeedsVisitChildren(BaseRow row) { return true; }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("RowOperationNeedsFullIteration")]
#endif
		public virtual bool NeedsFullIteration { get { return true; } }
		public virtual void Init() {}
		public virtual void Release() {}
		internal virtual bool NeedsVisitUnloadedRows { get { return false; } }
	}
	public abstract class RowPropertiesOperation : RowOperation {
		public override void Execute(BaseRow row) {
			for(int i = 0; i < row.RowPropertiesCount; i++) {
				ExecuteForProperty(row.GetRowProperties(i));
				if(!CanContinueIteration(row))
					break;
			}
		}
		protected abstract void ExecuteForProperty(RowProperties properties);
	}
	public class GetVisibleRowsRowOperation : RowOperation {
		GridRowReadOnlyCollection rows = new GridRowReadOnlyCollection();
		public VGridControlBase Grid { get; protected set; }
		public GetVisibleRowsRowOperation(VGridControlBase owner) {
			Grid = owner;
		}
		public override void Execute(BaseRow row) {
			if (IsVisible(row))
				Rows.Add(row);
		}
		public virtual bool IsVisible(BaseRow row) {
			if (!row.Visible || row.Grid != Grid)
				return false;
			if (SkipCategory(row))
				return false;
			return true;
		}
		public bool ShowCategories { get; set; }
		public GridRowReadOnlyCollection Rows { get { return rows; } }
		public IComparer<BaseRow> Comparer { get; set; }
		public override bool NeedsVisitChildren(BaseRow row) {
			return row.Visible && (row.Expanded || SkipCategory(row));
		}
		bool SkipCategory(BaseRow row) {
			if (ShowCategories)
				return false;
			CategoryRow categoryRow = row as CategoryRow;
			return categoryRow != null && Grid.CanSkipCategoryRow(categoryRow);
		}
	}
	internal class SortCustomizationRows : RowOperation {
		protected class AddChildCustomizationRow : RowOperation {
			private SortCustomizationRows op;
			public AddChildCustomizationRow(SortCustomizationRows op) {
				this.op = op;
			}
			public override void Execute(BaseRow row) {
				if(row.OptionsRow.ShowInCustomizationForm) {
					op.AddRow(row);
				}
			}
		}
		protected VGridCustomizationForm.RowsListBox lbRows, lbCats;
		protected int hr, hc;
		protected VGridControlBase grid;
		internal SortCustomizationRows(VGridControlBase grid, VGridCustomizationForm.RowsListBox lbRows, VGridCustomizationForm.RowsListBox lbCats) {
			this.grid = grid;
			this.lbRows = lbRows;
			this.lbCats = lbCats;
			this.hr = this.hc = 0;
		}
		public override void Init() {
			hr = lbRows.ClientRectangle.Y;
			hc = lbCats.ClientRectangle.Y;
			lbRows.Items.BeginUpdate();
			lbCats.Items.BeginUpdate();
			lbRows.Items.Clear();
			lbCats.Items.Clear();
		}
		public override void Execute(BaseRow row) {
			if(!row.Visible && row.OptionsRow.ShowInCustomizationForm) {
				AddRow(row);
			}
		}
		public override bool NeedsVisitChildren(BaseRow row) { 
			if(row.Visible) return base.NeedsVisitChildren(row);
			AddAllChilds(row);
			return false; 
		}
		protected void AddRow(BaseRow row) {
			int h = row.Grid.ViewInfo.GetVisibleRowHeight(row);
			if(row is CategoryRow) {
				BaseRowHeaderInfo rh = CreateRowInfo(row, new Size(lbCats.ClientRectangle.Width, h));
				lbCats.Items.Add(rh);
				hc += h;
			}
			else {
				BaseRowHeaderInfo rh = CreateRowInfo(row, new Size(lbRows.ClientRectangle.Width, h));
				lbRows.Items.Add(rh);
				hr += h;
			}
		}
		private BaseRowHeaderInfo CreateRowInfo(BaseRow row, Size size) {
			BaseRowHeaderInfo rh = row.CreateHeaderInfo();
			rh.Calc(new Rectangle(Point.Empty, size), row.Grid.ViewInfo, null, false, null);
			return rh;
		}
		private void AddAllChilds(BaseRow row) {
			grid.RowsIterator.DoLocalOperation(new AddChildCustomizationRow(this), row.ChildRows);
		}
		public override void Release() {
			lbRows.Items.EndUpdate();
			lbCats.Items.EndUpdate();
		}
	}
	internal class CalcRowHeight : RowOperation {
		protected int sumHeight, horzLineHeight;
		protected int rowCount;
		internal CalcRowHeight(int horzLineHeight) {
			this.sumHeight = 0;
			this.horzLineHeight = horzLineHeight;
			this.rowCount = 0;
		}
		public override void Execute(BaseRow row) {
			if(row.Visible) {
				sumHeight += row.Grid.ViewInfo.GetVisibleRowHeight(row) + horzLineHeight;
				rowCount++;
			}
		}
		public override bool NeedsVisitChildren(BaseRow row) {
			return row.Expanded;
		}
		public int SumHeight { get { return sumHeight; } }
		public int RowCount { get { return rowCount; } }
	}
	internal class ExpandCollapse : RowOperation {
		protected bool expand;
		protected VGridControlBase grid;
		internal ExpandCollapse(VGridControlBase grid, bool expand) {
			this.grid = grid;
			this.expand = expand;
		}
		public override void Execute(BaseRow row) {
			row.Expanded = expand;
		}
		public override void Init() { grid.BeginUpdate(); }
		public override void Release() { grid.EndUpdate(); }
	}
	internal class AddToContainer : RowOperation {
		static char[] ExceptionChars = "!. :#@$%^&*()[]{}/\\|*-+?,\'\"`~".ToCharArray();
		static char ReplaceChar = '_';
		protected IContainer container;
		IContainer Container { get { return container; } }
		internal AddToContainer(IContainer container) {
			this.container = container;
		}
		public override void Execute(BaseRow row) {
			string supposedName = GetSupposedName(row);
			if(Container == null) {
				row.Name = row.Grid.UniqueNames.AddName(supposedName);
			}
			else {
				if(row.Site != null)
					return;
				string uniqueName = GetContainerName(supposedName);
				try {
					container.Add(row, uniqueName);
				}
				catch {
					container.Add(row);
				}
				AddMultiEditorRowProperties(row);
			}
		}
		string GetSupposedName(BaseRow row) {
			if(row.Name != string.Empty)
				return row.Name;
			if(row.Properties == null)
				return RowProperties.BaseRowName;
			StringBuilder supposedName = new StringBuilder(row.Properties.GetBaseName());
			foreach(char c in ExceptionChars) {
				supposedName = supposedName.Replace(c, ReplaceChar);
			}
			return supposedName.ToString();
		}
		string GetContainerName(string name) {
			string res = name;
			string final = name;
			int n = 1;
			while(true) {
				if(Container == null)
					break;
				if(Container.Components[final] == null)
					break;
				final = res + n++.ToString();
			}
			return final;
		}
		void AddMultiEditorRowProperties(BaseRow row) {
			MultiEditorRow multiEditorRow = row as MultiEditorRow;
			if(multiEditorRow == null || Container == null)
				return;
			for(int i = 0; i < multiEditorRow.RowPropertiesCount; i++) {
				IComponent properties = (IComponent)multiEditorRow.GetRowProperties(i);
				if(properties.Site != null)
					continue;
				Container.Add(properties);
			}
		}
	}
	internal class FindLastChild : RowOperation {
		protected BaseRow result;
		public FindLastChild() { this.result = null; }
		public override bool NeedsVisitChildren(BaseRow row) {
			return (row.Index == row.Rows.Count - 1);
		}
		public override void Execute(BaseRow row) {
			if(!(row.HasChildren && row.ChildRows.IsLoaded))
				result = row;
		}
		public BaseRow Row { get { return result; } }
	}
	public class FindRowByNameOperation : RowOperation {
		BaseRow result;
		string name;
		public FindRowByNameOperation(string name) {
			this.name = name;
			this.result = null;
		}
		public override void Execute(BaseRow row) {
			if(row.Name == this.name)
				result = row;
		}
		public override bool CanContinueIteration(BaseRow row) { return (result == null); }
		public BaseRow Row { get { return result; } }
	}
	internal class FindRowByHandle : RowOperation {
		BaseRow result;
		int handle;
		public FindRowByHandle(int handle) {
			this.handle = handle;
			this.result = null;
		}
		public override void Execute(BaseRow row) {
			if(row.Properties.RowHandle == handle)
				result = row;
		}
		public override bool CanContinueIteration(BaseRow row) { return (result == null); }
		public BaseRow Row { get { return result; } }
	}
	internal class FindRowByFieldName : RowPropertiesOperation {
		BaseRow result;
		string fieldName;
		bool recursive;
		public FindRowByFieldName(string fieldName, bool recursive) {
			this.fieldName = fieldName;
			this.recursive = recursive;
			this.result = null;
		}
		protected override void ExecuteForProperty(RowProperties properties) {
			if(properties.FieldName == fieldName)
				this.result = properties.Row;
		}
		public override bool NeedsVisitChildren(BaseRow row) {
			if(!recursive)
				return false;
			if(!row.ChildRows.IsLoaded) {
				return row.Properties != null && fieldName.StartsWith(row.Properties.FieldName);
			}
			return true;
		}
		public override bool CanContinueIteration(BaseRow row) { return (result == null); }
		public BaseRow Row { get { return result; } }
		internal override bool NeedsVisitUnloadedRows { get { return true; } }
	}
	public class FindRowByCaption : RowPropertiesOperation {
		string caption;
		BaseRow result;
		public FindRowByCaption(string caption) {
			this.caption = caption;
		}
		protected override void ExecuteForProperty(RowProperties properties) {
			if(properties.Caption == this.caption)
				this.result = properties.Row;
		}
		public override bool CanContinueIteration(BaseRow row) { return (result == null); }
		public BaseRow Row { get { return result; } }
	}
	internal class RowFireChanged : RowOperation {
		public override void Execute(BaseRow row) { row.FireChanged();  }
	}
	internal class RemoveHeight : RowOperation {
		AutoHeightsStore autoHeights;
		public RemoveHeight(AutoHeightsStore autoHeights) {
			this.autoHeights = autoHeights;
		}
		public override void Execute(BaseRow row) {
			if(AutoHeights.ContainsKey(row))
				AutoHeights.Remove(row);
		}
		protected AutoHeightsStore AutoHeights { get { return autoHeights; } }
	}
	internal class CreateUnboundColumnInfoesOperation : RowPropertiesOperation {
		DevExpress.Data.UnboundColumnInfoCollection unboundColumns;
		UnboundRowPropertiesCache unboundRowPropertiesCache;
		public DevExpress.Data.UnboundColumnInfoCollection UnboundColumns { get { return unboundColumns; } }
		public CreateUnboundColumnInfoesOperation(UnboundRowPropertiesCache unboundRowPropertiesCache) {
			this.unboundColumns = new DevExpress.Data.UnboundColumnInfoCollection();
			this.unboundRowPropertiesCache = unboundRowPropertiesCache;
			this.unboundRowPropertiesCache.Clear();
		}
		protected override void ExecuteForProperty(RowProperties properties) {
			if(properties != null && properties.UnboundType != DevExpress.Data.UnboundColumnType.Bound && properties.Bindable) {
				if(string.IsNullOrEmpty(properties.FieldName)) {
					properties.FieldName = properties.GetName();
					if(string.IsNullOrEmpty(properties.FieldName))
						return;
				}
				UnboundColumns.Add(new DevExpress.Data.UnboundColumnInfo(properties.FieldName, properties.UnboundType, properties.GetReadOnly(), properties.UnboundExpression));
				this.unboundRowPropertiesCache.AddProperties(properties.FieldName, properties);
			}
		}
	}
	internal class RowChangedOperation : RowOperation {
		RowChangeTypeEnum changeType;
		public RowChangedOperation(RowChangeTypeEnum changeType) {
			this.changeType = changeType;
		}
		public override void Execute(BaseRow row) {
			row.Rows.Changed(row, row.Rows, null, this.changeType);
		}
		public override bool CanContinueIteration(BaseRow row) {
			if(row == null || row.Rows == null) return false;
			return true;
		}
	}
	internal class SaveExpandedStateOperation : RowOperation {
		Dictionary<string, bool> expandedRowStateStore = new Dictionary<string, bool>();
		public Dictionary<string, bool> ExpandedRowStateStore { get { return expandedRowStateStore; } }
		public override void Execute(BaseRow row) {
			if(string.IsNullOrEmpty(row.Name))
				return;
			ExpandedRowStateStore[row.Name] = row.Expanded;
		}
	}
	internal class LoadExpandedStateOperation : RowOperation {
		Dictionary<string, bool> rowExpandedStateStore;
		public LoadExpandedStateOperation(Dictionary<string, bool> rowExpandedStateStore) {
			this.rowExpandedStateStore = rowExpandedStateStore;
		}
		public Dictionary<string, bool> RowExpandedStateStore { get { return rowExpandedStateStore; } }
		public override void Execute(BaseRow row) {
			if(string.IsNullOrEmpty(row.Name))
				return;
			bool expanded;
			if(RowExpandedStateStore.TryGetValue(row.Name, out expanded)) {
				row.Expanded = expanded;
			}
		}
	}
	internal class CalculateBestWidth : RowOperation {
		float maxHeaderWidth = 0f;
		public float MaxHeaderWidth { get { return maxHeaderWidth; } set { maxHeaderWidth = value; } }
		public override void Execute(BaseRow row) {
			MaxHeaderWidth = Math.Max(MaxHeaderWidth, row.HeaderInfo.CalculateBestFit());
		}
	}
	internal class CountOperation : RowOperation {
		int rowCount;
		public int RowCount { get { return rowCount; } }
		public override void Execute(BaseRow row) {
			rowCount++;
		}
	}
	public class GetRowPropertiesOperation : RowPropertiesOperation {
		List<RowProperties> propertyList = new List<RowProperties>();
		RowProperties excluded;
		public GetRowPropertiesOperation(RowProperties excluded) {
			this.excluded = excluded;
		}
		public List<RowProperties> Properties { get { return propertyList; } }
		protected override void ExecuteForProperty(RowProperties properties) {
			if(properties == excluded || !properties.Bindable)
				return;
			propertyList.Add(properties);
		}
	}
	public class DelegateRowOperation : RowOperation {
		public delegate void ExecuteDelegate(BaseRow row);
		ExecuteDelegate executeDelegate;
		public DelegateRowOperation(ExecuteDelegate executeDelegate) {
			this.executeDelegate = executeDelegate;
		}
		public override void Execute(BaseRow row) {
			this.executeDelegate(row);
		}
	}
	public class DelegateRowPropertiesOperation : RowPropertiesOperation {
		public delegate void ExecuteDelegate(RowProperties properties);
		ExecuteDelegate executeDelegate;
		public DelegateRowPropertiesOperation(ExecuteDelegate executeDelegate) {
			this.executeDelegate = executeDelegate;
		}
		protected override void ExecuteForProperty(RowProperties properties) {
			this.executeDelegate(properties);
		}
	}
	public class CreateItemsPoolOperation : RowPropertiesOperation {
		public List<RowInfo> Rows { get; private set; }
		public List<RowProperties> RowProperties { get; private set; }
		public CreateItemsPoolOperation() {
			Rows = new List<RowInfo>();
			RowProperties = new List<RowProperties>();
		}
		public override void Execute(BaseRow row) {
			Rows.Add(new RowInfo { Row = row, ParentRowName = row.ParentRow != null ? row.ParentRow.Name : null });
			base.Execute(row);
		}
		protected override void ExecuteForProperty(RowProperties properties) {
			if (properties is MultiEditorRowProperties)
				RowProperties.Add(properties);
		}
	}
	public class FilterRowPropertiesOperation : GetVisibleRowsRowOperation {
		Dictionary<BaseRow, IEnumerable<BaseRow>> filterCache;
		Dictionary<BaseRow, IEnumerable<BaseRow>> FilterCache { get { return filterCache; } }
		public FilterRowPropertiesOperation(VGridControlBase vGrid) : base(vGrid)  {
			filterCache = new Dictionary<BaseRow, IEnumerable<BaseRow>>();
		}
		protected ExpressionEvaluator ExpressionEvaluator {
			get {
				return new ExpressionEvaluator(
						GetFilterProperties(),
						GetFilterCriteria(),
						false);
			}
		}
		protected virtual IEnumerable<BaseRow> GetFilteredChildren(BaseRow handle) {
			IEnumerable<BaseRow> handles;
			if (!FilterCache.TryGetValue(handle, out handles)) {
				handles = GetFilteredChildrenInternal(handle);
				FilterCache[handle] = handles;
			}
			return handles;
		}
		IEnumerable<BaseRow> GetFilteredChildrenInternal(BaseRow handle) {
			IEnumerable<BaseRow> children = GetChildrenInternal(handle) ?? EmptyEnumerable<BaseRow>.Instance;
			foreach (BaseRow child in children) {
				if (IsVisible(child)) {
					yield return child;
					continue;
				}
				if (HasVisibleChildren(child)) {
					yield return child;
					continue;
				}
			}
		}
		IEnumerable<BaseRow> GetChildrenInternal(BaseRow handle) {
			return handle.ChildRows.Cast<BaseRow>();
		}
		bool HasVisibleChildren(BaseRow row) {
			PGridEditorRow pRow = row as PGridEditorRow;
			if (pRow != null && !pRow.IsChildRowsLoaded)
				return false;
			return GetFilteredChildren(row).Any();
		}
		public override bool IsVisible(BaseRow row) {
			bool baseIsVisible = base.IsVisible(row);
			if (!baseIsVisible)
				return false;
			if (CriteriaOperator.Equals(GetFilterCriteria(), null))
				return true;
			return row.GetRowProperties().Any(p => {
				bool result = (bool)ExpressionEvaluator.Evaluate(FilterObject.Instance.Init(p));
				FilterObject.Instance.Release();
				return result;
			}) || HasVisibleChildren(row);
		}
		CriteriaOperator GetFilterCriteria() {
			return Grid.FindFilterCriteria;
		}
		PropertyDescriptorCollection GetFilterProperties() {
			return TypeDescriptor.GetProperties(FilterObject.Instance, null, false);
		}
		public class FilterObject {
			public static FilterObject Instance = new FilterObject();
			RowProperties properties;
			protected FilterObject() {
			}
			public string Caption { get { return properties.Caption; } }
			public FilterObject Init(RowProperties properties) {
				this.properties = properties;
				return this;
			}
			public void Release() {
				this.properties = null;
			}
		}
	}
}
