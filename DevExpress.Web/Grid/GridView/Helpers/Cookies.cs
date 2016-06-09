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

using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Cookies {
	public class GridViewColumnAutoFilterConditionState {
		public GridViewColumnAutoFilterConditionState(GridViewDataColumn column, AutoFilterCondition condition) {
			Column = column;
			Condition = condition;
		}
		public GridViewDataColumn Column { get; private set; }
		public AutoFilterCondition Condition { get; private set; }
		public void Apply() {
			Column.Settings.AutoFilterCondition = Condition;
		}
	}
	public class GridViewColumnsState : GridColumnsState {
		int groupCount;
		public GridViewColumnsState(ASPxGridView grid)
			: base(grid) {
			this.groupCount = 0;
			FilterConditionList = new List<GridViewColumnAutoFilterConditionState>();
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		public new GridViewColumnHelper ColumnHelper { get { return base.ColumnHelper as GridViewColumnHelper; } }
		public List<GridViewColumnAutoFilterConditionState> FilterConditionList { get; private set; }
		public int GroupCount { get { return Math.Min(this.groupCount, SortList.Count); } set { groupCount = value; } }
		public override void Apply() {
			base.Apply();
			for(int i = 0; i < SortList.Count; i++) {
				var column = SortList[i].Column;
				if(i < GroupCount)
					column.Adapter.SetGroupIndex(i);
			}
			foreach(var state in FilterConditionList)
				state.Apply();
		}
		public override bool IsEmpty {
			get {
				if(!base.IsEmpty)
					return false;
				if(FilterConditionList.Count > 0)
					return false;
				return true;
			}
		}
		public override void Save(TypedBinaryWriter writer) {
			base.Save(writer);
			SaveColumnsWidths(writer);
			SaveColumnAutoFilterConditions(writer);
		}
		protected override void SaveGroupingAndSorting(TypedBinaryWriter writer) {
			base.SaveGroupingAndSorting(writer);
			writer.WriteObject(Grid.GroupCount);
		}
		protected virtual void SaveColumnsWidths(TypedBinaryWriter writer) {
			int count = 0;
			if(RenderHelper.AllowColumnResizing) {
				foreach(var column in ColumnHelper.AllColumns) {
					if(!column.Width.IsEmpty)
						count++;
				}
			}
			writer.WriteObject(count);
			if(count == 0) return;
			for(int n = 0; n < ColumnHelper.AllColumns.Count; n++) {
				if(ColumnHelper.AllColumns[n].Width.IsEmpty)
					continue;
				writer.WriteObject(n);
				writer.WriteObject((int)ColumnHelper.AllColumns[n].Width.Type);
				writer.WriteObject(ColumnHelper.AllColumns[n].Width.Value);
			}
		}
		protected virtual void SaveColumnAutoFilterConditions(TypedBinaryWriter writer) {
			if(!RenderHelper.RequireRenderFilterRowMenu) {
				writer.WriteObject(0);
				return;
			}
			writer.WriteObject(ColumnHelper.AllDataColumns.Count);
			foreach(GridViewDataColumn column in ColumnHelper.AllDataColumns) {
				writer.WriteObject(ColumnHelper.GetColumnGlobalIndex(column));
				writer.WriteObject((int)column.Settings.AutoFilterCondition);
			}
		}
		public override void Load(TypedBinaryReader reader) {
			base.Load(reader);
			LoadColumnsWidths(reader);
			LoadColumnAutoFilterConditions(reader);
		}
		protected override void LoadGroupingAndSorting(TypedBinaryReader reader) {
			base.LoadGroupingAndSorting(reader);
			GroupCount = reader.ReadObject<int>();
		}
		protected virtual void LoadColumnsWidths(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				int index = reader.ReadObject<int>();
				UnitType type = (UnitType)reader.ReadObject<int>();
				double value = reader.ReadObject<double>();
				if(index >= 0 && index < ColumnStates.Count) {
					ColumnStates[index].Width = new Unit(value, type);
				}
			}
		}
		protected virtual void LoadColumnAutoFilterConditions(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				int index = reader.ReadObject<int>();
				AutoFilterCondition condition = (AutoFilterCondition)reader.ReadObject<int>();
				GridViewDataColumn column = index < ColumnHelper.AllColumns.Count ? ColumnHelper.AllColumns[index] as GridViewDataColumn : null;
				if(column != null)
					FilterConditionList.Add(new GridViewColumnAutoFilterConditionState(column, condition));
			}
		}
	}
	public abstract class GridViewCookiesBase : GridCookiesBase {
		protected const string
			GroupPrefix = "group",
			FilterConditionsPrefix = "conditions",
			ColumnWidthPrefix = "width",
			ControlWidthPrefix = "ctrlwidth",
			VisiblePrefix = "visible";
		public GridViewCookiesBase(ASPxGridView grid)
			: base(grid) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public Unit ControlWidth { get; private set; }
		public new GridViewColumnsState ColumnsState { get { return base.ColumnsState as GridViewColumnsState; } }
		protected abstract bool StoreVisibility { get; }
		protected abstract bool StoreColumnsWidth { get; }
		protected abstract bool StoreControlWidth { get; }
		protected override GridColumnsState CreateColumnsState() {
			return new GridViewColumnsState(Grid);
		}
		protected override void SaveGroupingAndSorting() {
			if(Grid.GroupCount > 0)
				AppendFormat("{0}{1}", GroupPrefix, Grid.GroupCount);
			base.SaveGroupingAndSorting();
		}
		protected override void SaveFilterState() {
			base.SaveFilterState();
			var columns = Grid.DataColumns.Where(c => c.Settings.AutoFilterCondition != AutoFilterCondition.Default).ToList();
			if(columns.Count > 0) {
				AppendFormat("{0}{1}", FilterConditionsPrefix, columns.Count);
				foreach(var column in columns)
					AppendFormat("{0}|{1}", Grid.GetColumnGlobalIndex(column), (int)column.Settings.AutoFilterCondition);
			}
		}
		protected override void SaveGridViewSpecificState() {
			if(StoreVisibility)
				SaveColumnsVisibility();
			if(StoreColumnsWidth)
				SaveColumnsWidth();
			if(StoreControlWidth)
				SaveControlWidth();
		}
		protected virtual void SaveColumnsVisibility() {
			AppendFormat("{0}{1}", VisiblePrefix, Grid.AllColumns.Count);
			foreach(var column in ColumnHelper.AllColumns)
				AppendFormat("{0}{1}", column.Visible ? 't' : 'f', column.VisibleIndex);
		}
		protected virtual void SaveColumnsWidth() {
			AppendFormat("{0}{1}", ColumnWidthPrefix, Grid.AllColumns.Count);
			foreach(GridViewColumn column in Grid.AllColumns)
				AppendFormat("{0}", column.Width.IsEmpty ? "e" : column.Width.ToString());
		}
		protected virtual void SaveControlWidth() {
			AppendFormat("{0}{1}", ControlWidthPrefix, Grid.Width);
		}
		protected override void LoadGroupingAndSorting() {
			ColumnsState.GroupCount = ReadIndex(GroupPrefix);
			base.LoadGroupingAndSorting();
		}
		protected override void LoadFiltering() {
			base.LoadFiltering();
			LoadFilterConditions();
		}
		protected override void LoadGridViewSpecificState() {
			LoadColumnsVisibility();
			LoadColumnsWidth();
			LoadControlWidth();
		}
		protected void LoadColumnsVisibility() {
			int count = Math.Min(ReadIndex(VisiblePrefix), ColumnsState.ColumnStates.Count);
			for(int i = 0; i < count; i++)
				LoadColumnVisibility(ColumnsState.ColumnStates[i]);
		}
		protected void LoadColumnsWidth() {
			int count = Math.Min(ReadIndex(ColumnWidthPrefix), ColumnsState.ColumnStates.Count);
			for(int i = 0; i < count; i++)
				LoadColumnWidth(ColumnsState.ColumnStates[i]);
		}
		protected void LoadControlWidth() {
			string controlWidthStr = ReadString(ControlWidthPrefix);
			if(!string.IsNullOrEmpty(controlWidthStr))
				ControlWidth = Unit.Parse(controlWidthStr);
		}
		void LoadColumnVisibility(GridColumnState columnState) {
			if(string.IsNullOrEmpty(CurrentState)) return;
			columnState.Visible = CurrentState[0] == 't';
			CurrentState = CurrentState.Remove(0, 1);
			columnState.VisibleIndex = ReadIndex();
		}
		void LoadColumnWidth(GridColumnState columnState) {
			string width = ReadString();
			if(!string.IsNullOrEmpty(width))
				columnState.Width = width == "e" ? Unit.Empty : Unit.Parse(width);
		}
		protected void LoadFilterConditions() {
			int count = ReadIndex(FilterConditionsPrefix);
			for(int i = 0; i < count; i++) {
				var conditionState = LoadFilterConditionState();
				if(conditionState != null)
					ColumnsState.FilterConditionList.Add(conditionState);
			}
		}
		GridViewColumnAutoFilterConditionState LoadFilterConditionState() {
			int columnIndex = ReadIndex();
			int value = ReadIndex();
			if(columnIndex >= Grid.AllColumns.Count)
				return null;
			GridViewDataColumn column = Grid.AllColumns[columnIndex] as GridViewDataColumn;
			if(column == null) return null;
			return new GridViewColumnAutoFilterConditionState(column, (AutoFilterCondition)value);
		}
		protected override void ClearUnsavedFields() {
			base.ClearUnsavedFields();
			if(!StoreGroupingAndSorting)
				ColumnsState.GroupCount = 0;
			if(!StoreFiltering)
				ColumnsState.FilterConditionList.Clear();
			if(!StoreVisibility)
				ColumnsState.ResetColumnsVisibility();
			if(!StoreColumnsWidth)
				ColumnsState.ResetColumnsWidth();
			if(!StoreControlWidth)
				ControlWidth = Unit.Empty;
		}
		protected override void Clear() {
			base.Clear();
			ControlWidth = Unit.Empty;
		}
	}
	public class GridViewSEOProcessing : GridViewCookiesBase {
		public GridViewSEOProcessing(ASPxGridView grid) : base(grid) { }
		protected override string Version { get { return string.Empty; } }
		protected override bool StorePaging { get { return true; } }
		protected override bool StoreGroupingAndSorting { get { return true; } }
		protected override bool StoreFiltering { get { return true; } }
		protected override bool StoreSearchPanelFiltering { get { return true; } }
		protected override bool StoreVisibility { get { return false; } }
		protected override bool StoreColumnsWidth { get { return false; } }
		protected override bool StoreControlWidth { get { return false; } }
	}
	public class GridViewCookies : GridViewCookiesBase {
		public GridViewCookies(ASPxGridView grid) : base(grid) { }
		protected ASPxGridViewCookiesSettings Settings { get { return Grid.SettingsCookies; } }
		protected override bool StorePaging { get { return Settings.StorePaging; } }
		protected override bool StoreGroupingAndSorting { get { return Settings.StoreGroupingAndSorting; } }
		protected override bool StoreFiltering { get { return Settings.StoreFiltering; } }
		protected override bool StoreSearchPanelFiltering { get { return Settings.StoreSearchPanelFiltering; } }
		protected override bool StoreColumnsWidth { get { return Settings.StoreColumnsWidth; } }
		protected override bool StoreControlWidth { get { return Settings.StoreControlWidth; } }
		protected override bool StoreVisibility { get { return Settings.StoreColumnsVisiblePosition; } }
		protected override string Version { get { return Settings.Version; } }
		protected override void LoadStateCore() {
			string oldVersion = ReadString(VersionPrefix);
			if(oldVersion != Version) return;
			base.LoadStateCore();
		}
	}
}
