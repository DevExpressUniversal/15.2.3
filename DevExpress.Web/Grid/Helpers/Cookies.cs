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
using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Cookies {
	public class GridColumnStateBase {
		public GridColumnStateBase(IWebGridColumn column) {
			Column = column;
		}
		public IWebGridColumn Column { get; private set; }
	}
	public class GridColumnSortState : GridColumnStateBase {
		public GridColumnSortState(IWebGridDataColumn column, ColumnSortOrder sortOrder)
			: base(column) {
			SortOrder = sortOrder;
		}
		public ColumnSortOrder SortOrder { get; private set; }
		public new IWebGridDataColumn Column { get { return base.Column as IWebGridDataColumn; } }
	}
	public class GridColumnState : GridColumnStateBase {
		public GridColumnState(IWebGridColumn column)
			: base(column) {
			Reset();
		}
		public new WebColumnBase Column { get { return base.Column as WebColumnBase; } }
		public Unit Width { get; set; }
		public bool Visible { get; set; }
		public int VisibleIndex { get; set; }
		public bool IsEmpty { get { return Width == Column.Width && Visible == Column.Visible && VisibleIndex == Column.VisibleIndex; } }
		public virtual void Apply() {
			if(IsEmpty) return;
			Column.Width = Width;
			Column.SetColVisible(Visible);
			Column.SetColVisibleIndex(VisibleIndex);
		}
		public void Reset() {
			ResetWidth();
			ResetVisibility();
		}
		public void ResetWidth() {
			Width = Column.Width;
		}
		public void ResetVisibility() {
			Visible = Column.Visible;
			VisibleIndex = Column.VisibleIndex;
		}
	}
	public class GridColumnsState {
		public GridColumnsState(ASPxGridBase grid) {
			Grid = grid;
			SortList = new List<GridColumnSortState>();
			ColumnStates = new List<GridColumnState>();
			EnsureColumnStates();
		}
		public ASPxGridBase Grid { get; private set; }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public List<GridColumnSortState> SortList { get; private set; }
		public List<GridColumnState> ColumnStates { get; private set; }
		protected virtual void EnsureColumnStates() {
			foreach(var column in ColumnHelper.AllColumns)
				ColumnStates.Add(new GridColumnState(column));
		}
		public virtual void Apply() {
			foreach(var state in ColumnStates)
				state.Apply();
			for(int i = 0; i < SortList.Count; i++) {
				var column = SortList[i].Column;
				column.Adapter.SetSortOrder(SortList[i].SortOrder);
				column.Adapter.SetSortIndex(i);
			}
		}
		public virtual void ResetColumnsWidth() {
			foreach(var state in ColumnStates)
				state.ResetWidth();
		}
		public virtual void ResetColumnsVisibility() {
			foreach(var state in ColumnStates)
				state.ResetVisibility();
		}
		public virtual bool IsEmpty {
			get {
				if(SortList.Count > 0)
					return false;
				if(ColumnStates.Any(s => !s.IsEmpty))
					return false;
				return true;
			}
		}
		public virtual void Save(TypedBinaryWriter writer) {
			SaveColumnsVisibleState(writer);
			SaveGroupingAndSorting(writer);
		}
		protected virtual void SaveColumnsVisibleState(TypedBinaryWriter writer) {
			writer.WriteObject(ColumnHelper.AllColumns.Count);
			foreach(var column in ColumnHelper.AllColumns) {
				writer.WriteObject(column.VisibleIndex);
				writer.WriteObject(column.Visible);
			}
		}
		protected virtual void SaveGroupingAndSorting(TypedBinaryWriter writer) {
			writer.WriteObject(Grid.SortedColumns.Count);
			foreach(var column in Grid.SortedColumns) {
				writer.WriteObject(ColumnHelper.GetColumnGlobalIndex(column));
				writer.WriteObject((int)column.SortOrder);
				writer.WriteObject((int)column.Adapter.UngroupedSortOrder);
			}
		}
		public virtual void Load(TypedBinaryReader reader) {
			LoadColumnsVisibleState(reader);
			LoadGroupingAndSorting(reader);
		}
		protected virtual void LoadColumnsVisibleState(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int visibleIndex = reader.ReadObject<int>();
				bool visible = reader.ReadObject<bool>();
				if(n >= ColumnStates.Count) continue;
				ColumnStates[n].Visible = visible;
				ColumnStates[n].VisibleIndex = visibleIndex;
			}
		}
		protected virtual void LoadGroupingAndSorting(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int index = reader.ReadObject<int>();
				ColumnSortOrder sortOrder = (ColumnSortOrder)reader.ReadObject(typeof(int));
				ColumnSortOrder ungroupedSortOrder = (ColumnSortOrder)reader.ReadObject(typeof(int));
				var column = index < ColumnHelper.AllColumns.Count ? ColumnHelper.AllColumns[index] as IWebGridDataColumn : null;
				if(column == null) continue;
				column.Adapter.UngroupedSortOrder = ungroupedSortOrder;
				SortList.Add(new GridColumnSortState(column, sortOrder));
			}
		}
	}
	public abstract class GridCookiesBase {
		const char Divider = '|';
		const int
			DefaultPageIndex = -2,
			DefaultPageSize = 0;
		protected const string
			VersionPrefix = "version",
			PagePrefix = "page",
			SizePrefix = "size",
			SortPrefix = "sort",
			FilterPrefix = "filter",
			FilterEnabledPrefix = "fltenabled",
			SearchPanelFilterPrefix = "spfilter";
		public GridCookiesBase(ASPxGridBase grid) {
			Grid = grid;
			StateBuilder = new StringBuilder(100);
			Clear();
		}
		public ASPxGridBase Grid { get; private set; }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public GridColumnsState ColumnsState { get; private set; }
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public string FilterExpression { get; private set; }
		public bool FilterEnabled { get; private set; }
		public string SearchPanelFilter { get; private set; }
		protected StringBuilder StateBuilder { get; private set; }
		protected string CurrentState { get; set; }
		protected internal void SetPageIndex() {
			if(!StorePaging) return;
			Grid.PageIndex = PageIndex;
		}
		protected internal void SetPageSize() {
			if(!StorePaging || !StoreSizePaging) return;
			Grid.PageSize = PageSize;
		}
		protected virtual bool StoreSizePaging { get { return Grid.SettingsPager.PageSizeItemSettings.Visible; } }
		protected abstract string Version { get; }
		protected abstract bool StorePaging { get; }
		protected abstract bool StoreGroupingAndSorting { get; }
		protected abstract bool StoreFiltering { get; }
		protected abstract bool StoreSearchPanelFiltering { get; }
		public string SaveState(int pageIndex) {
			return SaveState(pageIndex, 0);
		}
		public string SaveState(int pageIndex, int pageSize) {
			StateBuilder.Clear();
			SaveStateCore(pageIndex, pageSize);
			return StateBuilder.ToString();
		}
		protected virtual void SaveStateCore(int pageIndex, int pageSize) {
			SaveVersion();
			if(StorePaging)
				SavePaging(pageIndex, pageSize);
			if(StoreGroupingAndSorting)
				SaveGroupingAndSorting();
			if(StoreFiltering)
				SaveFilterState();
			SaveGridViewSpecificState();
			if(StoreSearchPanelFiltering)
				SaveSearchPanelFilter();
		}
		protected virtual void SaveGridViewSpecificState() { } 
		protected virtual void SaveVersion() {
			if(!string.IsNullOrEmpty(Version))
				AppendFormat("{0}{1}", VersionPrefix, Version);
		}
		protected virtual void SavePaging(int pageIndex, int pageSize) {
			AppendFormat("{0}{1}", PagePrefix, pageIndex + 1);
			if(StoreSizePaging)
				AppendFormat("{0}{1}", SizePrefix, pageSize);
		}
		protected virtual void SaveGroupingAndSorting() {
			if(Grid.SortCount > 0) {
				AppendFormat("{0}{1}", SortPrefix, Grid.SortCount);
				foreach(var column in Grid.SortedColumns)
					AppendFormat("{0}{1}", column.SortOrder == ColumnSortOrder.Ascending ? 'a' : 'd', Grid.GetColumnGlobalIndex(column));
			}
		}
		protected virtual void SaveFilterState() {
			if(!string.IsNullOrEmpty(Grid.FilterExpression))
				AppendFormat("{0}{1}|{2}", FilterPrefix, Grid.FilterExpression.Length, Grid.FilterExpression);
			if(!Grid.FilterEnabled)
				AppendFormat("{0}{1}", FilterEnabledPrefix, "false");
		}
		protected virtual void SaveSearchPanelFilter() {
			if(!string.IsNullOrEmpty(Grid.SearchPanelFilter))
				AppendFormat("{0}{1}|{2}", SearchPanelFilterPrefix, Grid.SearchPanelFilter.Length, Grid.SearchPanelFilter);
		}
		protected void AppendFormat(string format, params object[] args) {
			if(StateBuilder.Length > 0) 
				StateBuilder.Append(Divider);
			StateBuilder.Append(string.Format(format, args));
		}
		public bool LoadState(string state) {
			Clear();
			if(string.IsNullOrEmpty(state)) 
				return !IsEmpty;
			CurrentState = state;
			LoadStateCore();
			ClearUnsavedFields();
			return !IsEmpty;
		}
		protected virtual void LoadStateCore() {
			LoadPaging();
			LoadGroupingAndSorting();
			LoadFiltering();
			LoadGridViewSpecificState();
			LoadSearchPanelFilter();
		}
		protected virtual void LoadGridViewSpecificState() { } 
		protected virtual void LoadPaging() {
			PageIndex = !CurrentState.StartsWith(PagePrefix) ? Grid.PageIndex : ReadIndex(PagePrefix) - 1;
			if(!Grid.PagerIsValidPageIndex(PageIndex))
				PageIndex = Grid.PageIndex;
			if(!StoreSizePaging) 
				return;
			PageSize = !CurrentState.StartsWith(SizePrefix) ? Grid.PageSize : ReadIndex(SizePrefix);
			if(PageSize <= 0) {
				PageIndex = -1;
				PageSize = Grid.PageSize;
			}
			if(!Grid.PagerIsValidPageSize(PageSize))
				PageSize = Grid.PageSize;
		}
		protected virtual void LoadGroupingAndSorting() {
			int sortCount = ReadIndex(SortPrefix);
			if(sortCount == 0) return;
			for(int i = 0; i < sortCount; i++) {
				var sort = LoadSortInfo();
				if(sort == null) break;
				ColumnsState.SortList.Add(sort);
			}
		}
		protected virtual void LoadFiltering() {
			FilterExpression = LoadFilterString(FilterPrefix);
			bool enabled;
			if(bool.TryParse(ReadString(FilterEnabledPrefix), out enabled))
				FilterEnabled = enabled;
		}
		protected virtual void LoadSearchPanelFilter() {
			SearchPanelFilter = LoadFilterString(SearchPanelFilterPrefix);
		}
		GridColumnSortState LoadSortInfo() {
			if(string.IsNullOrEmpty(CurrentState)) return null;
			var sortOrder = CurrentState[0] == 'a' ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
			CurrentState = CurrentState.Remove(0, 1);
			int columnIndex = ReadIndex();
			if(columnIndex < 0) return null;
			if(columnIndex >= ColumnHelper.AllColumns.Count) return null;
			var column = ColumnHelper.AllColumns[columnIndex] as IWebGridDataColumn;
			if(column == null) return null;
			return new GridColumnSortState(column, sortOrder);
		}
		string LoadFilterString(string prefix) {
			string result = string.Empty;
			if(!CurrentState.StartsWith(prefix))
				return result;
			CurrentState = CurrentState.Remove(0, prefix.Length);
			int pos = CurrentState.IndexOf(Divider);
			int filterExpressionLength = Convert.ToInt32(CurrentState.Substring(0, pos));
			CurrentState = CurrentState.Remove(0, pos + 1);
			if(filterExpressionLength > 0) {
				result = CurrentState.Substring(0, filterExpressionLength);
				CurrentState = CurrentState.Remove(0, filterExpressionLength);
			}
			if(!string.IsNullOrEmpty(CurrentState) && CurrentState[0] == Divider)
				CurrentState = CurrentState.Remove(0, 1);
			return result;
		}
		protected virtual void ClearUnsavedFields() {
			if(!StorePaging) {
				PageIndex = DefaultPageIndex;
				if(!StoreSizePaging)
					PageSize = DefaultPageSize;
			}
			if(!StoreGroupingAndSorting)
				ColumnsState.SortList.Clear();
			if(!StoreFiltering)
				FilterExpression = string.Empty;
			if(!StoreSearchPanelFiltering)
				SearchPanelFilter = string.Empty;
		}
		public virtual bool IsEmpty { get { return PageIndex < -1 && PageSize < 1 && ColumnsState.IsEmpty && string.IsNullOrEmpty(FilterExpression) && string.IsNullOrEmpty(SearchPanelFilter); } }
		protected virtual void Clear() {
			ColumnsState = CreateColumnsState();
			PageIndex = DefaultPageIndex;
			PageSize = DefaultPageSize;
			FilterExpression = string.Empty;
			FilterEnabled = true;
			SearchPanelFilter = string.Empty;
		}
		protected virtual GridColumnsState CreateColumnsState() {
			return new GridColumnsState(Grid);
		}
		protected int ReadIndex(string prefixName) {
			if(!CurrentState.StartsWith(prefixName)) return 0;
			CurrentState = CurrentState.Remove(0, prefixName.Length);
			return ReadIndex();
		}
		protected int ReadIndex() {
			string res = ReadString();
			int index = -1;
			if(!int.TryParse(res, out index)) return -1;
			return index;
		}
		protected string ReadString(string prefixName) {
			if(!CurrentState.StartsWith(prefixName)) return string.Empty;
			CurrentState = CurrentState.Remove(0, prefixName.Length);
			return ReadString();
		}
		protected string ReadString() {
			int pos = CurrentState.IndexOf(Divider);
			if(pos < 0) pos = CurrentState.Length;
			string result = CurrentState.Substring(0, pos);
			CurrentState = CurrentState.Remove(0, pos);
			if(!string.IsNullOrEmpty(CurrentState) && CurrentState[0] == Divider) {
				CurrentState = CurrentState.Remove(0, 1);
			}
			return result;
		}
	}
}
