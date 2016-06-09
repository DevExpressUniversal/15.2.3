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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Views.Grid.ViewInfo {
	public class AutoWidthObjectInfo {
		object _obj;
		int minWidth, width, visibleWidth, maxWidth;
		bool fixedWidth, maxFixedWidth;
		AutoWidthObjectInfoCollection children;
		AutoWidthObjectInfo parent, realParent;
		public const int NonFixedWidth = -999999;
		public AutoWidthObjectInfo(object obj, int minWidth, int width, int visibleWidth) : this(obj, minWidth, 0, width, visibleWidth, false) {
		}
		public AutoWidthObjectInfo(object obj, int minWidth, int maxWidth, int width, int visibleWidth, bool fixedWidth) {
			if(width < 1) width = 1;
			if(minWidth < 1) minWidth = 1;
			this.minWidth = minWidth;
			this.maxWidth = maxWidth;
			this.width = CheckWidth(width);
			this.fixedWidth = fixedWidth;
			this.visibleWidth = CheckWidth(visibleWidth);
			this.realParent = null;
			this.parent = null;
			this._obj = obj;
			this.children = null;
			this.maxFixedWidth = false;
		}
		public object Obj { get { return _obj; } }
		public AutoWidthObjectInfo Parent { get { return parent; } set { parent = value; } }
		public AutoWidthObjectInfo RealParent { get { return realParent; } set { realParent = value; } }
		public bool IsFixedWidth { get { return fixedWidth; } set { fixedWidth = value; } }
		public bool IsMaxFixedWidth { get { return maxFixedWidth; } set { maxFixedWidth = value; } }
		public int Width { get { return width; } set { width = value; } }
		public int MinWidth { 
			get { return minWidth; }
			set {
				minWidth = value;
				width = CheckWidth(Width);
			}
		}
		public int MaxWidth {
			get { return maxWidth; }
			set {
				maxWidth = value;
				width = CheckWidth(Width);
			}
		}
		public int CheckWidth(int width) {
			if(width > MaxWidth && MaxWidth > 0) width = MaxWidth;
			if(width < MinWidth) width = MinWidth;
			return width;
		}
		public int VisibleWidth { get { return visibleWidth; } set { visibleWidth = value; } }
		public AutoWidthObjectInfoCollection Children { get { return children; } }
		public void AddChild(AutoWidthObjectInfo info) {
			if(info == null) return;
			if(Children == null) children = new AutoWidthObjectInfoCollection(this);
			Children.Add(info);
			info.Parent = this;
			info.RealParent = this;
		}
		public bool HasChildren { get { return Children != null && Children.Count > 0; } }
		public override string ToString() {
			return string.Format("{0} Min={1},Max={2},Width={3},V={4}", Obj, MinWidth, MaxWidth, Width, VisibleWidth);
		}
	}
	public class MultiRowAutoWidthObjectInfo : AutoWidthObjectInfo {
		AutoWidthObjectRowCollection rows;
		public MultiRowAutoWidthObjectInfo(object obj, int minWidth, int width, int visibleWidth) : this(obj, minWidth, width, visibleWidth, false) {
		}
		public MultiRowAutoWidthObjectInfo(object obj, int minWidth, int width, int visibleWidth, bool fixedWidth) : base(obj, minWidth, width, visibleWidth) {
			this.rows = new AutoWidthObjectRowCollection(this);
		}
		public AutoWidthObjectRowCollection Rows { get { return rows; } }
		public virtual void UpdateValues() {
			Rows.UpdateTotals();
			this.IsFixedWidth = Rows.IsFixedWidth;
			this.MinWidth = Rows.MaxRowMinWidth;
			this.Width = Rows.MaxRowWidth;
			this.VisibleWidth = Rows.MaxRowVisibleWidth;
		}
		public AutoWidthObjectRow FindRowByObject(object obj) {
			foreach(AutoWidthObjectRow row in Rows) {
				if(row.Objects.FindObject(obj) != null) return row;
			}
			return null;
		}
	}	
	public class AutoWidthObjectRow {
		AutoWidthObjectInfoCollection objects;
		AutoWidthObjectInfo owner;
		public AutoWidthObjectRow(AutoWidthObjectInfo owner) {
			this.owner = owner;
			this.objects = new AutoWidthObjectInfoCollection(owner);
		}
		public AutoWidthObjectInfo Owner { get { return owner; } }
		public AutoWidthObjectInfoCollection Objects { get { return objects; } }
		public virtual bool IsAllFixedWidth {
			get {
				foreach(AutoWidthObjectInfo obj in Objects) {
					if(!obj.IsFixedWidth) return false;
				}
				return true;
			}
		}
	}
	public class AutoWidthObjectRowCollection : CollectionBase {
		int maxRowWidth, maxRowMinWidth, maxRowVisibleWidth;
		bool isFixedWidth;
		AutoWidthObjectInfo owner;
		public AutoWidthObjectRowCollection(AutoWidthObjectInfo owner) {
			this.owner = owner;
			ClearConstants();
		}
		public AutoWidthObjectInfo Owner { get { return owner; } }
		public AutoWidthObjectRow this[int index] { get { return List[index] as AutoWidthObjectRow; } }
		public virtual AutoWidthObjectRow Add() {
			AutoWidthObjectRow row = new AutoWidthObjectRow(Owner);
			List.Add(row);
			return row;
		}
		public int MaxRowVisibleWidth { get { return maxRowVisibleWidth; } set { maxRowVisibleWidth = value; } }
		public int MaxRowWidth { get { return maxRowWidth; } set { maxRowWidth = value; } }
		public int MaxRowMinWidth { get { return maxRowMinWidth; } set { maxRowMinWidth = value; } }
		public bool IsFixedWidth { get { return isFixedWidth; } set { isFixedWidth = value; } }
		public virtual void ClearConstants() {
			this.MaxRowVisibleWidth = this.maxRowMinWidth = this.maxRowWidth = 0;
			this.isFixedWidth = false;
		}
		public virtual void UpdateTotals() {
			ClearConstants();
			this.isFixedWidth = true;
			foreach(AutoWidthObjectRow row in this) {
				row.Objects.UpdateTotals(0);
				isFixedWidth = isFixedWidth && row.IsAllFixedWidth;
				if(MaxRowWidth < row.Objects.TotalWidth) {
					MaxRowWidth = row.Objects.TotalWidth;
				}
				if(MaxRowVisibleWidth < row.Objects.TotalVisibleWidth) {
					MaxRowVisibleWidth = row.Objects.TotalVisibleWidth;
				}
				if(MaxRowMinWidth < row.Objects.TotalMinWidth)
					MaxRowMinWidth = row.Objects.TotalMinWidth;
			}
		}
	}
	public class AutoWidthObjectInfoCollection : CollectionBase {
		int totalMinWidth, totalFixedWidth, totalWidth, totalVisibleWidth, isAllFixed = -1;
		AutoWidthObjectInfo owner;
		public AutoWidthObjectInfoCollection(AutoWidthObjectInfo owner) {
			this.owner = owner;
			ClearConstants();
		}
		public AutoWidthObjectInfo Owner { get { return owner; } }
		public virtual void ClearConstants() {
			this.totalMinWidth = this.totalFixedWidth = this.totalVisibleWidth = this.totalWidth = 0;
		}
		public int TotalMinWidth { get { return totalMinWidth; } set { totalMinWidth = value; } }
		public int TotalVisibleWidth { get { return totalVisibleWidth; } set { totalVisibleWidth = value; } }
		public int TotalFixedWidth { get { return totalFixedWidth; } set { totalFixedWidth = value; } }
		public int TotalWidth { get { return totalWidth; } set { totalWidth = value; } }
		public AutoWidthObjectInfo this[int index] { get { return List[index] as AutoWidthObjectInfo; } }
		public virtual void UpdateTotals(int startFrom) {
			ClearConstants();
			if(startFrom < 0) startFrom = 0;
			for(int n = startFrom; n < Count; n++) {
				AutoWidthObjectInfo info = this[n];
				if(info.IsFixedWidth) TotalFixedWidth += info.Width;
				TotalWidth += info.Width;
				TotalVisibleWidth += info.VisibleWidth;
				TotalMinWidth += info.MinWidth;
			}
		}
		public virtual AutoWidthObjectInfo FindObject(object obj) {
			foreach(AutoWidthObjectInfo info in this) {
				if(info.Obj == obj) return info;
				if(info.HasChildren) {
					AutoWidthObjectInfo res = info.Children.FindObject(obj);
					if(res != null) return res;
				}
				MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
				if(mr != null) {
					foreach(AutoWidthObjectRow row in mr.Rows) {
						AutoWidthObjectInfo res = row.Objects.FindObject(obj);
						if(res != null) return res;
					}
				}
			}
			return null;
		}
		public virtual int NonFixedCount {
			get { 
				int res = 0;
				for(int n = 0; n < Count; n++) {
					if(!this[n].IsFixedWidth) res ++;
				}
				return res;
			}
		}
		public virtual bool IsAllFixed {
			get {
				if(isAllFixed != -1) return isAllFixed == 1;
				if(Count == 0) return true;
				foreach(AutoWidthObjectInfo info in this) {
					if(!info.IsFixedWidth) {
						this.isAllFixed = 0;
						return false;
					}
				}
				this.isAllFixed = 1;
				return true;
			}
		}
		public void UpdateTotals() {
			UpdateTotals(0);
		}
		public AutoWidthObjectInfo Add(AutoWidthObjectInfo info) { 
			this.isAllFixed = -1;
			if(info.IsFixedWidth) TotalFixedWidth += info.Width;
			TotalWidth += info.Width;
			TotalVisibleWidth += info.VisibleWidth;
			List.Add(info); 
			return info;
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ClearConstants();
		}
		public int IndexOf(AutoWidthObjectInfo info) { return List.IndexOf(info); }
		public AutoWidthObjectInfo Last {
			get { return Count == 0 ? null : this[Count - 1]; }
		}
		public AutoWidthObjectInfo LastNonFixed {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					AutoWidthObjectInfo info = this[n];
					if(!info.IsFixedWidth) return info;
				}
				return null;
			}
		}
	}
	public class AutoWidthCalculatorArgs {
		int maxVisibleWidth;
		AutoWidthObjectInfoCollection objects, lastChildren;
		bool isAutoWidth, readyToCalc;
		public AutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth) {
			this.readyToCalc = false;
			this.lastChildren = null;
			this.maxVisibleWidth = maxVisibleWidth;
			this.objects = objects;
			this.isAutoWidth = isAutoWidth;
		}
		public bool IsAutoWidth { get { return isAutoWidth; } }
		public int MaxVisibleWidth { get { return maxVisibleWidth; } set { maxVisibleWidth = value; } }
		public AutoWidthObjectInfoCollection Objects { get { return objects; } set { this.objects = value; } }
		public AutoWidthObjectInfoCollection LastChildren { get { return lastChildren; } set { this.lastChildren = value; } }
		public bool ReadyToCalc { get { return readyToCalc; } set { this.readyToCalc = value; } }
	}
	public class GridAutoWidthCalculatorArgs : AutoWidthCalculatorArgs {
		int startColumn, endColumn;
		public GridAutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth) : this(objects, isAutoWidth, maxVisibleWidth, -1, -1) {
		}
		public GridAutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth, int startColumn, int endColumn) : base(objects, isAutoWidth, maxVisibleWidth) {
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}
		public int StartColumn { get { return startColumn; } }
		public int EndColumn { get { return endColumn; } }
		public List<GridColumn> VisibleColumns { get; set; }
	}
	public class AutoWidthCalculator {
		AutoWidthObjectInfoCollection objects;
		public AutoWidthCalculator() {
			this.objects = new AutoWidthObjectInfoCollection(null);
		}
		public virtual AutoWidthObjectInfoCollection Objects { get { return objects; } }
		protected virtual void CalcAutoWidth(AutoWidthCalculatorArgs e) {
			bool ignoreFixedWidth = false;
			if(e.Objects.IsAllFixed) {
				ignoreFixedWidth = true;
			}
			CalcAutoWidthCore(e, ignoreFixedWidth);
			List<AutoWidthObjectInfo> changedObjects = UpdateFixedWidthForMaximizedItems(e);
			CalcAutoWidthCore(e, ignoreFixedWidth);
			foreach(AutoWidthObjectInfo info in changedObjects) info.IsFixedWidth = false;
			foreach(AutoWidthObjectInfo info in e.Objects) {
				if(info.HasChildren) {
					bool isAutoWidthChildren = e.IsAutoWidth;
					if(!isAutoWidthChildren) {
						info.Children.UpdateTotals();
						if(info.Children.TotalWidth < info.MinWidth) isAutoWidthChildren = true;
					}
					CalcAutoWidth(new AutoWidthCalculatorArgs(info.Children, isAutoWidthChildren, info.VisibleWidth));
				} 
				MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
				if(mr != null) {
					foreach(AutoWidthObjectRow row in mr.Rows) {
						CalcAutoWidth(new AutoWidthCalculatorArgs(row.Objects, true, info.VisibleWidth));
					}
				}
			}
		}
		List<AutoWidthObjectInfo> UpdateFixedWidthForMaximizedItems(AutoWidthCalculatorArgs e) {
			List<AutoWidthObjectInfo> changedObjects = new List<AutoWidthObjectInfo>();
			foreach(AutoWidthObjectInfo info in e.Objects) {
				if(info.VisibleWidth >= info.MaxWidth && info.MaxWidth > 0 && !info.IsFixedWidth) {
					info.IsMaxFixedWidth = true;
					changedObjects.Add(info);
				}
			}
			return changedObjects;
		}
		void CalcAutoWidthCore(AutoWidthCalculatorArgs e, bool ignoreFixedWidth) {
			e.Objects.UpdateTotals();
			int maxWidth = e.MaxVisibleWidth;
			int totalWidth = e.Objects.TotalWidth;
			if(!ignoreFixedWidth) {
				maxWidth -= e.Objects.TotalFixedWidth;
				totalWidth -= e.Objects.TotalFixedWidth;
			}
			if(maxWidth < 1) maxWidth = 1;
			int restWidth = maxWidth;
			for(int n = 0; n < e.Objects.Count; n++) {
				AutoWidthObjectInfo info = e.Objects[n];
				int finalWidth = info.Width;
				if(!e.IsAutoWidth) {
					info.VisibleWidth = info.Width;
				}
				else {
					if(info.IsFixedWidth && !ignoreFixedWidth) {
						info.VisibleWidth = info.Width;
					}
					else {
						if(info.IsMaxFixedWidth)
							finalWidth = info.MaxWidth;
						else {
							finalWidth = (info.Width * maxWidth) / totalWidth;
							if(finalWidth > restWidth) finalWidth = restWidth;
						}
						finalWidth = info.CheckWidth(finalWidth);
						info.VisibleWidth = finalWidth;
						restWidth -= finalWidth;
					}
				}
			}
			if(!ignoreFixedWidth)
				AutoWidthRecovery(e, restWidth);
		}
		int AutoWidthRecovery(AutoWidthCalculatorArgs e, int restWidth) {
			if(!e.IsAutoWidth) return restWidth;
			if(restWidth > 0) {
				for(int i = e.Objects.Count - 1; i >= 0 && restWidth > 0; i--) {
					AutoWidthObjectInfo info = e.Objects[i];
					if(info.IsFixedWidth || info.IsMaxFixedWidth) continue;
					if(restWidth > 0) {
						int delta = info.CheckWidth(info.VisibleWidth + restWidth) - info.VisibleWidth;
						info.VisibleWidth += delta;
						restWidth -= delta;
					}
				}
			}
			else {
				for(int i = e.Objects.Count - 1; i >= 0 && restWidth < 0; i--) {
					AutoWidthObjectInfo info = e.Objects[i];
					if(info.IsFixedWidth || info.IsMaxFixedWidth) continue;
					if(restWidth < 0) {
						int delta = info.VisibleWidth - info.CheckWidth(info.VisibleWidth + restWidth);
						info.VisibleWidth -= delta;
						restWidth += delta;
					}
				}
			}
			return restWidth;
		}
		protected virtual void CheckArgs(AutoWidthCalculatorArgs e) {
			if(e.Objects == null) e.Objects = Objects;
		}
		protected virtual void FixObject(AutoWidthObjectInfo info) {
			if(info == null) return;
			AutoWidthObjectInfoCollection coll = info.Parent == null ? Objects : info.Parent.Children;
			MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
			if(mr == null) mr = info.RealParent as MultiRowAutoWidthObjectInfo;
			if(mr != null) {
				foreach(AutoWidthObjectRow row in mr.Rows) {
					FixObjectCollection(info, row.Objects);
				}
				FixObject(info.RealParent);
				return;
			}
			FixObjectCollection(info, coll);
		}
		protected virtual void FixObjectCollection(AutoWidthObjectInfo info, AutoWidthObjectInfoCollection coll) {
			int lastNFIndex = coll.IndexOf(coll.LastNonFixed);
			int curIndex = coll.IndexOf(info);
			for(int n = 0; n < coll.Count; n++) {
				AutoWidthObjectInfo ooo = coll[n];
				FixObject(ooo.Parent);
				if(curIndex < lastNFIndex && coll.NonFixedCount > 1)
					ooo.IsFixedWidth = true;
				if(ooo.Parent != null) FixObject(ooo.Parent);
				if(ooo == info) break;
			}
		}
		public virtual void ChangeObjectWidth(AutoWidthCalculatorArgs e, object obj) {
			CreateList(e);
			AutoWidthObjectInfo info = Objects.FindObject(obj);
			if(info == null) return;
			AutoWidthObjectInfoCollection coll = info.Parent == null ? Objects : info.Parent.Children;
			if(!e.IsAutoWidth) {
				ChangeObjectNonAutoWidth(e, info);
			} else {
				ChangeObjectWithAutoWidth(e, info);
			}
		}
		void ChangeObjectWithAutoWidth(AutoWidthCalculatorArgs e, AutoWidthObjectInfo info) {
			FixObject(info);
			UpdateRealObjects(e, true);
			if(info.HasChildren) {
				e.Objects = Objects;
				CalcAutoWidth(new AutoWidthCalculatorArgs(info.Children, true, info.VisibleWidth));
				UpdateRealObjects(new AutoWidthCalculatorArgs(info.Children, true, info.VisibleWidth), true);
				UpdateWidthes(info.Children);
				UpdateRealObjects(e, true);
			}
			else {
				MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
				if(mr == null) mr = info.RealParent as MultiRowAutoWidthObjectInfo;
				if(mr != null) {
					int maxWidth = mr.Parent != null ? mr.Parent.VisibleWidth : mr.VisibleWidth;
					foreach(AutoWidthObjectRow row in mr.Rows) {
						CalcAutoWidth(new AutoWidthCalculatorArgs(row.Objects, true, maxWidth));
						UpdateRealObjects(new AutoWidthCalculatorArgs(row.Objects, true, maxWidth), true);
						UpdateWidthes(row.Objects);
						e.Objects = Objects;
						UpdateRealObjects(e, true);
					}
					return;
				}
			}
			Calc(e);
			e.Objects = Objects;
			UpdateRealObjects(e, true);
		}
		void ChangeObjectNonAutoWidth(AutoWidthCalculatorArgs e, AutoWidthObjectInfo info) {
			if(info.HasChildren) {
				e.Objects = Objects;
				UpdateRealObjects(e, true);
				CalcAutoWidth(new AutoWidthCalculatorArgs(info.Children, true, info.Width));
				UpdateRealObjects(new AutoWidthCalculatorArgs(info.Children, true, info.Width), true);
				UpdateWidthes(info.Children);
			}
			else {
				if(info.Parent != null) {
					if(info.Parent.MinWidth > info.Parent.Children.TotalWidth) {
						info.Width += (info.Parent.MinWidth - info.Parent.Children.TotalWidth);
					}
				}
				MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
				if(mr == null) mr = info.RealParent as MultiRowAutoWidthObjectInfo;
				if(mr == null) return;
				AutoWidthObjectRow row = mr.FindRowByObject(info.Obj);
				if(row != null) {
					ChangeObjectWithAutoWidth(new AutoWidthCalculatorArgs(row.Objects, true, mr.VisibleWidth), info);
				}
			}
		}
		public virtual void CreateList(AutoWidthCalculatorArgs e) {
		}
		public virtual void PrepareCalc(AutoWidthCalculatorArgs e, bool updateWidthes) {
			if(e.Objects == null) {
				CreateList(e);
				e.Objects = Objects;
			}
			if(updateWidthes) {
				e.LastChildren = GetLastChildren(Objects, null);
				UpdateWidthes(e.LastChildren);
			}
		}
		public virtual void Calc(AutoWidthCalculatorArgs e) {
			if(!e.ReadyToCalc) PrepareCalc(e, true);
			DoCalc(e);
			UpdateWidthes(e.LastChildren);
		}
		public virtual Size CalcMinMaxWidth(AutoWidthCalculatorArgs e, int index) {
			if(!e.ReadyToCalc) PrepareCalc(e, true);
			if(index < 0) index = 0;
			if(index > e.Objects.Count - 1) index =  e.Objects.Count - 1;
			AutoWidthObjectInfo info = e.Objects[index];
			if(!e.IsAutoWidth) return new Size(info.MinWidth, -1);
			Size res = new Size(info.MinWidth, info.MinWidth);
			int m = 0, w = 0;
			for(int n = index; n < e.Objects.Count; n++) {
				m += e.Objects[n].MinWidth;
				w += e.Objects[n].VisibleWidth;
			}
			if(m > 0) res.Height = Math.Max(res.Height, w - m);
			return res;
		}
		public void UpdateRealObjects(AutoWidthCalculatorArgs e) {
			UpdateRealObjects(e, false);
		}
		public virtual void UpdateRealObjects(AutoWidthCalculatorArgs e, bool updateBothToVisibleWidth) {
			CheckArgs(e);
			DoUpdateRealObjects(e.Objects, updateBothToVisibleWidth);
		}
		protected virtual void DoUpdateRealObjects(AutoWidthObjectInfoCollection objs, bool updateBothToVisibleWidth) {
			if(objs == null )return;
			for(int n = 0; n < objs.Count; n++) {
				AutoWidthObjectInfo info = objs[n];
				DoUpdateRealObject(info, updateBothToVisibleWidth);
				if(info.HasChildren) DoUpdateRealObjects(info.Children, updateBothToVisibleWidth);
				MultiRowAutoWidthObjectInfo mr = info as MultiRowAutoWidthObjectInfo;
				if(mr != null) {
					foreach(AutoWidthObjectRow row in mr.Rows) {
						DoUpdateRealObjects(row.Objects, updateBothToVisibleWidth);
					}
				}
			}
		}
		protected virtual void DoUpdateRealObject(AutoWidthObjectInfo info, bool setBothToVisibleWidth) {
		}
		protected virtual void DoCalc(AutoWidthCalculatorArgs e) {
		}
		public virtual bool CheckMinWidth(AutoWidthCalculatorArgs e) {
			AutoWidthObjectInfoCollection lastCh = GetLastChildren(Objects, null);
			return UpdateWidthes(lastCh);
		}
		protected virtual AutoWidthObjectInfoCollection GetLastChildren(AutoWidthObjectInfoCollection start, AutoWidthObjectInfoCollection res) {
			if(res == null) res = new AutoWidthObjectInfoCollection(null);
			for(int n = 0; n < start.Count; n++) {
				AutoWidthObjectInfo info = start[n];
				if(!info.HasChildren) 
					res.Add(info);
				else 
					GetLastChildren(info.Children, res);
			}
			return res;
		}
		protected bool UpdateWidthes(AutoWidthObjectInfoCollection lastChildren) {
			if(lastChildren == null) return false;
			bool anyChanges = false;
			for(int pass = 0; pass < 2; pass++) {
				for(int n = 0; n < lastChildren.Count; n++) {
					AutoWidthObjectInfo info = lastChildren[n];
					if(info.Width < info.MinWidth) {
						anyChanges = true;
						info.Width = info.MinWidth;
					}
					if(pass == 0) continue;
					if(info.Parent != null) {
						anyChanges |= UpdateWidthes(info.Parent);
					}
				}
			}
			return anyChanges;
		}
		protected bool UpdateWidthes(AutoWidthObjectInfo info) {
			if(!info.HasChildren) return false;
			bool anyChanges = false;
			int mw = 0, w = 0, vw = 0;
			for(int n = 0; n < info.Children.Count; n++) {
				AutoWidthObjectInfo i = info.Children[n];
				w += i.Width;
				mw += i.MinWidth;
				vw += i.VisibleWidth;
			}
			if(info.MinWidth < mw) info.MinWidth = mw;
			w = Math.Max(w, info.MinWidth);
			if(info.Width != w) { 
				anyChanges = true;
				info.Width = w;
			}
			if(info.VisibleWidth != vw)  
				info.VisibleWidth = vw;
			if(info.Parent != null) {
				anyChanges |= UpdateWidthes(info.Parent.Children);
			}
			return anyChanges;
		}
	}
	public class GridAutoWidthCalculator : AutoWidthCalculator {
		GridView view;
		public GridAutoWidthCalculator(GridView view) {
			this.view = view;
		}
		public GridViewInfo ViewInfo { get { return View.ViewInfo as GridViewInfo; } }
		public GridView View { get { return view; } }
		public override void CreateList(AutoWidthCalculatorArgs e) {
			GridAutoWidthCalculatorArgs ee = e as GridAutoWidthCalculatorArgs;
			Objects.Clear();
			IList visibleColumns = ee.VisibleColumns == null ? (IList)View.VisibleColumns.ToIList() : (IList)ee.VisibleColumns;
			int count = visibleColumns.Count;
			for(int i = ee.StartColumn < 0 ? 0 : ee.StartColumn; i < count; i++) {
				GridColumn col = (GridColumn)visibleColumns[i];
				bool isFixedWidth = false;
				if(col.OptionsColumn.FixedWidth || col.Fixed != FixedStyle.None) {
					isFixedWidth = true;
					if(col == ViewInfo.FixedLeftColumn || col == ViewInfo.FixedRightColumn) Objects.TotalFixedWidth += View.FixedLineWidth;
				} 
				Objects.Add(new AutoWidthObjectInfo(col, GetObjectMinWidth(col), GetObjectMaxWidth(col), col.Width, col.VisibleWidth, isFixedWidth));
			}
		}
		protected override void DoCalc(AutoWidthCalculatorArgs e) {
			GridAutoWidthCalculatorArgs ee = e as GridAutoWidthCalculatorArgs;
			if(ee.StartColumn != -1) {
				for(int n = 0; n < Math.Min(ee.StartColumn, View.VisibleColumns.Count); n++) {
					ee.MaxVisibleWidth -= View.VisibleColumns[n].VisibleWidth;
				}
			}
			CalcAutoWidth(ee);
		}
		protected virtual int GetObjectMinWidth(object obj) {
			GridColumn col = obj as GridColumn;
			return View.GetColumnMinWidth(col);
		}
		protected virtual int GetObjectMaxWidth(object obj) {
			GridColumn col = obj as GridColumn;
			return View.GetColumnMaxWidth(col);
		}
		protected override void DoUpdateRealObject(AutoWidthObjectInfo info, bool setBothToVisibleWidth) {
			GridColumn col = info.Obj as GridColumn;
			if(col == null) return;
			if(setBothToVisibleWidth) info.Width = info.VisibleWidth;
			col.InternalSetWidth(info.Width);
			col.visibleWidth = info.VisibleWidth;
		}
	}
	public class BandedGridAutoWidthCalculatorArgs : AutoWidthCalculatorArgs {
		int startBand;
		GridBandCollection bands;
		GridBand singleBand;
		public BandedGridAutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth) : this(objects, isAutoWidth, maxVisibleWidth, null, -1) {
		}
		public BandedGridAutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth, GridBand singleBand) : this(objects, isAutoWidth, maxVisibleWidth, null, -1) {
			this.singleBand = singleBand;
			if(SingleBand != null) {
				this.bands = SingleBand.Collection;
				this.startBand = Bands.IndexOf(SingleBand);
			}
		}
		public BandedGridAutoWidthCalculatorArgs(AutoWidthObjectInfoCollection objects, bool isAutoWidth, int maxVisibleWidth, GridBandCollection bands, int startBand) : base(objects, isAutoWidth, maxVisibleWidth) {
			this.bands = bands;
			this.startBand = startBand;
		}
		public int StartBand { get { return startBand; } }
		public GridBand SingleBand { get { return singleBand; } }
		public GridBandCollection Bands { get { return bands; } set { bands = value; } }
	}
	public class BandAutoWidthCalculator : AutoWidthCalculator {
		BandedGridView view;
		public BandAutoWidthCalculator(BandedGridView view) {
			this.view = view;
		}
		public BandedGridViewInfo ViewInfo { get { return View.ViewInfo as BandedGridViewInfo; } }
		public BandedGridView View { get { return view; } }
		public override void CreateList(AutoWidthCalculatorArgs e) {
			BandedGridAutoWidthCalculatorArgs ee = e as BandedGridAutoWidthCalculatorArgs;
			if(ee.Bands == null) ee.Bands = View.Bands;
			Objects.Clear();
			CreateListCore(null, ee.SingleBand, ee.StartBand, ee.Bands);
		}
		protected void CreateListCore(AutoWidthObjectInfo parent, GridBand singleBand, int start, GridBandCollection bands) {
			int count = bands.Count;
			if(start < 0) start = 0;
			if(singleBand != null) count = 1 + start;
			for(int i = start; i < count; i++) {
				GridBand band = bands[i];
				if(!band.Visible) continue;
				bool isFixedWidth = false;
				if(band.OptionsBand.FixedWidth || band.Fixed != FixedStyle.None) {
					isFixedWidth = true;
					if(band == ViewInfo.FixedLeftBand || band == ViewInfo.FixedRightBand) Objects.TotalFixedWidth += View.FixedLineWidth;
				} 
				AutoWidthObjectInfo info = new AutoWidthObjectInfo(band, GetObjectMinWidth(band), 0, band.Width, band.VisibleWidth, isFixedWidth);
				if(parent == null) 
					Objects.Add(info);
				else
					parent.AddChild(info);
				if(band.HasChildren) {
					CreateListCore(info, null, 0, band.Children);
				} else {
					if(band.Columns.Count > 0) {
						CreateColumnList(info, band);
					}
				}
			}
		}
		protected virtual void CreateColumnList(AutoWidthObjectInfo parent, GridBand band) {
			for(int n = 0; n < band.Columns.Count; n++) {
				GridColumn col = band.Columns[n];
				if(col.VisibleIndex < 0 || (View.IsPrinting && (col.OptionsColumn.Printable == DevExpress.Utils.DefaultBoolean.False))) continue;
				bool isFixedWidth = false;
				if(col.OptionsColumn.FixedWidth) {
					isFixedWidth = true;
				} 
				parent.AddChild(new AutoWidthObjectInfo(col, GetObjectMinWidth(col), GetObjectMaxWidth(col), col.Width, col.VisibleWidth, isFixedWidth));
			}
		}
		protected override void DoCalc(AutoWidthCalculatorArgs e) {
			BandedGridAutoWidthCalculatorArgs ee = e as BandedGridAutoWidthCalculatorArgs;
			if(ee.StartBand != -1) {
				for(int n = 0; n < ee.StartBand; n++) {
					if(!ee.Bands[n].Visible) continue;
					ee.MaxVisibleWidth -= ee.Bands[n].VisibleWidth;
				}
			}
			CalcAutoWidth(ee);
		}
		protected virtual int GetObjectMaxWidth(object obj) {
			return 0;
		}
		protected virtual int GetObjectMinWidth(object obj) {
			GridBand band = obj as GridBand;
			if(band != null) {
				return View.GetBandMinWidth(band);
			}
			GridColumn col = obj as GridColumn;
			if(col != null) {
				return View.GetColumnMinWidth(col);
			}
			return 1;
		}
		protected override void DoUpdateRealObject(AutoWidthObjectInfo info, bool setBothToVisibleWidth) {
			GridBand band = info.Obj as GridBand;
			if(setBothToVisibleWidth) info.Width = info.VisibleWidth;
			if(band != null) {
				band.SetWidthCore(info.Width);
				band.SetVisibleWidthCore(info.VisibleWidth);
			}
			GridColumn col = info.Obj as GridColumn;
			if(col != null) {
				col.InternalSetWidth(info.Width);
				col.visibleWidth = info.VisibleWidth;
			}
		}
	}
	public class AdvBandAutoWidthCalculator : BandAutoWidthCalculator {
		public AdvBandAutoWidthCalculator(AdvBandedGridView view) : base(view) {
		}
		public new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as AdvBandedGridViewInfo; } }
		public new BandedGridView View { get { return base.View as AdvBandedGridView; } }
		protected override void CreateColumnList(AutoWidthObjectInfo parent, GridBand band) {
			if(band.Columns.Count == 0) return;
			GridBandRowCollection rows = View.GetBandRows(band);
			MultiRowAutoWidthObjectInfo mr = new MultiRowAutoWidthObjectInfo(null, 0, 0, 0);
			foreach(GridBandRow row in rows) {
				if(row.Columns.Count == 0) continue;
				AutoWidthObjectRow r = mr.Rows.Add();
				for(int n = 0; n < row.Columns.Count; n++) {
					GridColumn col = row.Columns[n];
					bool isFixedWidth = false;
					if(col.OptionsColumn.FixedWidth) {
						isFixedWidth = true;
					} 
					AutoWidthObjectInfo av = new AutoWidthObjectInfo(col, GetObjectMinWidth(col), 0, col.Width, col.VisibleWidth, isFixedWidth);
					av.RealParent = mr;
					r.Objects.Add(av);
				}
			}
			if(rows.Count == 0) return;
			mr.UpdateValues();
			parent.AddChild(mr);
		}
	}
}
