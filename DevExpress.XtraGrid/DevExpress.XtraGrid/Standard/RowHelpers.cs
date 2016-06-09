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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Views.Grid.ViewInfo {
	public abstract class GridPixelPositionCalculatorBase {
		GridViewInfo viewInfo;
		public GridPixelPositionCalculatorBase(GridViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public GridViewInfo ViewInfo { get { return viewInfo; } }
		public GridView View { get { return viewInfo.View; } }
		public virtual void Check() { }
		public virtual void Reset() { }
		public virtual int VisibleRowsHeight {
			get {
				return CalcVisibleRowsHeight();
			}
		}
		public abstract int CalcVisibleRowsHeight();
		public abstract int CalcPixelPositionByRow(int row);
		public int CalcVisibleRowByPixel(int pixelPosition) { return CalcVisibleRowByPixel(0, 1, pixelPosition, View.AllowFixedGroups); }
		public abstract int CalcVisibleRowByPixel(int startIndex, int direction, int pixelPosition, bool allowFixedGroups);
		public virtual void ResetRow(int handle) { }
	}
	public abstract class GridPixelPositionCalculatorBaseEx : GridPixelPositionCalculatorBase {
		public GridPixelPositionCalculatorBaseEx(GridViewInfo viewInfo) : base(viewInfo) { }
		protected int GetRowHeightInternal(int handle, int vindex) {
			GroupRowInfo group = handle < 0 ? View.DataController.GroupInfo.GetGroupRowInfoByHandle(handle) : null;
			int rowHeight = ViewInfo.CalcRowHeight(ViewInfo.GInfo.Graphics, handle, ViewInfo.MinRowHeight, group == null ? 0 : group.Level, true, null);
			rowHeight = ViewInfo.CalcTotalRowHeight(ViewInfo.GInfo.Graphics, rowHeight, handle, vindex, group == null ? 0 : group.Level, group == null ? (bool?)true : (bool?)group.Expanded);
			return rowHeight;
		}
		protected int GetRowHeight(bool allowFixedGroups, int handle, int vindex) {
			if(allowFixedGroups) {
				VisibleIndexHeightInfo info = View.DataController.GetVisibleIndexes().ScrollHeightInfo;
				if(info.Map != null && vindex < info.Map.Length) {
					int[] map = info.Map[vindex];
					if(info.IsZeroHeight(map)) return 0;
					if(info.IsSelfHeight(map)) return GetRowHeightInternal(handle, vindex);
					int rowHeight = 0;
					for(int n = 0; n < map.Length; n++) {
						rowHeight += GetRowHeightInternal(map[n], View.DataController.GetVisibleIndex(map[n]));
					}
					return rowHeight;
				}
			}
			return GetRowHeightInternal(handle, vindex);
		}
	}
	public class GridPixelPositionCalculator : GridPixelPositionCalculatorBaseEx {
		public GridPixelPositionCalculator(GridViewInfo viewInfo) : base(viewInfo) { }
		public override int CalcVisibleRowsHeight() {
			if(View.GroupCount == 0) return CalcPixelPositionByRow(View.RowCount);
			return CalcPixelPositionByRow(View.RowCount);
		}
		public override int CalcPixelPositionByRow(int row) {
			bool isGrouped = View.GroupCount > 0;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = isGrouped ? vi.Count : View.RowCount;
			int res = 0;
			bool allowFixedGroups = View.AllowFixedGroups;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(int n = 0; n < Math.Min(viCount, row); n++) {
					int index = isGrouped ? vi[n] : n;
					res += GetRowHeight(allowFixedGroups, index, n);
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return res;
		}
		public override int CalcVisibleRowByPixel(int startIndex, int direction, int pixelPosition, bool allowFixedGroups) {
			bool isGrouped = View.GroupCount > 0;
			int res = 0, n = 0;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = isGrouped ? vi.Count : View.RowCount;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(n = startIndex; n < viCount && n >= 0; n += direction) {
					int index = isGrouped ? vi[n] : n;
					res += GetRowHeight(allowFixedGroups, index, n);
					if(res > pixelPosition) return Math.Max(0, n);
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return Math.Max(0, n - 1);
		}
	}
	public class GridPixelPositionCalculatorSimple : GridPixelPositionCalculatorBase {
		public GridPixelPositionCalculatorSimple(GridViewInfo viewInfo) : base(viewInfo) { }
		public override int CalcVisibleRowsHeight() {
			if(View.GroupCount == 0) return CalcPixelPositionByRow(View.RowCount);
			return CalcPixelPositionByRow(View.RowCount);
		}
		public override int CalcPixelPositionByRow(int row) {
			bool isGrouped = View.GroupCount > 0;
			if(!isGrouped) return row * ViewInfo.ActualDataRowMinRowHeight;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = isGrouped ? vi.Count : View.RowCount;
			int res = 0;
			bool allowFixedGroups = View.AllowFixedGroups;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(int n = 0; n < Math.Min(viCount, row); n++) {
					int index = isGrouped ? vi[n] : n;
					GroupRowInfo group = index < 0 ? View.DataController.GroupInfo.GetGroupRowInfoByHandle(index) : null;
					int rowHeight = ViewInfo.ActualDataRowMinRowHeight;
					if(group != null) {
						rowHeight = ViewInfo.CalcRowHeight(null, group.Handle, group.Level);
					}
					res += rowHeight;
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return res;
		}
		public override int CalcVisibleRowByPixel(int startIndex, int direction, int pixelPosition, bool allowFixedGroups) {
			bool isGrouped = View.GroupCount > 0;
			if(!isGrouped && startIndex == 0) return pixelPosition / ViewInfo.ActualDataRowMinRowHeight;
			int res = 0, n = 0;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = isGrouped ? vi.Count : View.RowCount;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(n = startIndex; n < viCount && n >= 0; n += direction) {
					int index = isGrouped ? vi[n] : n;
					GroupRowInfo group = index < 0 ? View.DataController.GroupInfo.GetGroupRowInfoByHandle(index) : null;
					int rowHeight = ViewInfo.ActualDataRowMinRowHeight;
					if(group != null) {
						rowHeight = ViewInfo.CalcRowHeight(null, group.Handle, group.Level);
					}
					res += rowHeight;
					if(res > pixelPosition) return Math.Max(0, n);
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return Math.Max(0, n - 1);
		}
	}
	public class GridPixelPositionCalculatorCached : GridPixelPositionCalculatorBaseEx {
		internal List<int> rowsCache = new List<int>(); 
		internal List<int> rowsCacheHandle = new List<int>(); 
		internal List<int> rowsCachePixelHeight = new List<int>(); 
		internal List<int> rowsCacheMap = new List<int>();
		int cacheCreatedVisibleCount = 0;
		int cacheCreateGroupRowCount = 0;
		int cacheCreateGroupCount = 0;
		int cacheCreateRowHeight = 0, cacheGroupRowMinHeight = 0;
		int totalRowsHeight = -1;
		Rectangle viewRect, viewRowsRect;
		string cachePaintStyle;
		public GridPixelPositionCalculatorCached(GridViewInfo viewInfo) : base(viewInfo) { }
		public override int CalcVisibleRowsHeight() {
			if(totalRowsHeight > -1) return totalRowsHeight;
			if(View.GroupCount == 0) 
				this.totalRowsHeight = CalcPixelPositionByRow(View.RowCount);
			else
				this.totalRowsHeight = CalcPixelPositionByRow(View.RowCount);
			return totalRowsHeight;
		}
		public override void Check() {
			if(cacheCreateGroupCount != View.GroupCount || cacheCreatedVisibleCount != View.SafeVisibleCount || cacheCreateRowHeight != ViewInfo.ActualDataRowMinRowHeight ||
				viewRect != View.ViewRect || viewRowsRect != View.ViewInfo.ViewRects.Rows || GetPaintStyle() != cachePaintStyle || cacheGroupRowMinHeight != ViewInfo.GroupRowMinHeight) {
				Reset();
				return;
			}
			if(View.DataControllerCore != null && View.DataControllerCore.GroupRowCount != cacheCreateGroupRowCount) {
				Reset();
				return;
			}
		}
		public override void Reset() {
			base.Reset();
			rowsCache.Clear();
			rowsCacheHandle.Clear();
			rowsCacheMap.Clear();
			rowsCachePixelHeight.Clear();
			this.totalRowsHeight = -1;
			this.viewRect = View.ViewRect;
			this.viewRowsRect = View.ViewInfo.ViewRects.Rows;
			this.cacheCreateGroupCount = View.GroupCount;
			this.cacheCreateGroupRowCount = View.DataControllerCore == null ? 0 : View.DataControllerCore.GroupRowCount;
			this.cacheCreatedVisibleCount = View.SafeVisibleCount;
			this.cacheCreateRowHeight = ViewInfo.ActualDataRowMinRowHeight;
			this.cacheGroupRowMinHeight = ViewInfo.GroupRowMinHeight;
			this.cachePaintStyle = GetPaintStyle();
		}
		string GetPaintStyle() { return View.PaintStyleName + ((ISkinProvider)View).SkinName; }
		int GetCachedRowHeight(bool allowFixedGroups, int handle, int vindex, bool allowAddCache) {
			int rowHeight = 0;
			var scrollHeightInfo = View.DataController.GetVisibleIndexes().ScrollHeightInfo;
			int currentMapInfo = GetMapInfo(scrollHeightInfo, vindex);
			if(vindex < rowsCache.Count) {
				rowHeight = rowsCache[vindex];
				if(!CheckVIndexValid(scrollHeightInfo, vindex, handle, currentMapInfo)) {
					rowHeight = -1;
					rowsCachePixelHeight.Clear();
				}
				if(!allowFixedGroups && rowHeight == 0) rowHeight = -1;
				if(rowHeight < 0) {
					this.totalRowsHeight = -1;
					rowHeight = GetRowHeight(allowFixedGroups, handle, vindex);
					if(allowAddCache) {
						rowsCache[vindex] = rowHeight;
						rowsCacheHandle[vindex] = handle;
						rowsCacheMap[vindex] = currentMapInfo;
					}
				}
			}
			else {
				rowHeight = GetRowHeight(allowFixedGroups, handle, vindex);
				if(allowAddCache) {
					this.totalRowsHeight = -1;
					rowsCache.Add(rowHeight);
					rowsCacheHandle.Add(handle);
					rowsCacheMap.Add(currentMapInfo);
				}
			}
			return rowHeight;
		}
		bool CheckVIndexValid(VisibleIndexHeightInfo scrollHeightInfo, int vindex, int? handle, int? currentMapInfo) {
			if(vindex >= rowsCacheHandle.Count) return false;
			if(scrollHeightInfo == null) scrollHeightInfo = View.DataController.GetVisibleIndexes().ScrollHeightInfo;
			int handleCache = rowsCacheHandle[vindex];
			int mapCache = rowsCacheMap[vindex];
			if(handle == null) handle = View.GetVisibleRowHandle(vindex);
			if(currentMapInfo == null) currentMapInfo = GetMapInfo(scrollHeightInfo, vindex);
			if(handleCache != handle || mapCache != currentMapInfo) {
				return false;
			}
			return true;
		}
		int GetMapInfo(VisibleIndexHeightInfo scrollHeightInfo, int vindex) {
			if(scrollHeightInfo.Map == null || vindex >= scrollHeightInfo.Map.Length) return int.MinValue;
			var map = scrollHeightInfo.Map[vindex];
			if(scrollHeightInfo.IsZeroHeight(map)) return -1;
			if(scrollHeightInfo.IsSelfHeight(map)) return -2;
			try {
				int res = 0;
				foreach(int i in map) {
					res += i * 1234;
				}
				res += map.Length * 1000000;
				return res;
			}
			catch {
				return map.Length * 1000;
			}
		}
		public override void ResetRow(int handle) {
			for(int n = 0; n < rowsCacheHandle.Count; n++) {
				if(rowsCacheHandle[n] == handle) {
					rowsCache[n] = -1;
					rowsCachePixelHeight.Clear();
					totalRowsHeight = -1;
					break;
				}
			}
		}
		public override int CalcPixelPositionByRow(int row) {
			if(row < 0) return 0;
			if(row <= rowsCachePixelHeight.Count) {
				if(!CheckVIndexValid(null, row, null, null)) {
					rowsCachePixelHeight.Clear();
				} else
					return CalcPixelPositionByRowCached(row);
			}
			bool isGrouped = View.GroupCount > 0;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = View.RowCount; 
			int viMaxCount = vi.Count;
			int res = 0;
			bool allowFixedGroups = View.AllowFixedGroups;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(int n = 0; n < Math.Min(viCount, row); n++) {
					int index = isGrouped && n < viMaxCount ? vi[n] : n;
					res += GetCachedRowHeight(allowFixedGroups, index, n, true);
					if(n == rowsCachePixelHeight.Count) rowsCachePixelHeight.Add(res);
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return res;
		}
		int CalcPixelPositionByRowCached(int row) {
			if(row < 1) return 0;
			return rowsCachePixelHeight[row - 1];
		}
		int CalcVisibleRowByPixelCached(int startIndex, int pixelPosition) {
			if(rowsCachePixelHeight.Count <= startIndex) return -1;
			int res = SearchHelper(startIndex, pixelPosition);
			if(res >= 0) return res + 1;
			res = ~res;
			return res;
		}
		int SearchHelper(int startIndex, int value) {
			int length = rowsCachePixelHeight.Count;
			int i = startIndex;
			int last = startIndex + length - 1;
			while(i <= last) {
				int current = i + (last - i >> 1);
				int compareRes = Comparer<int>.Default.Compare(rowsCachePixelHeight[current], value);
				if(compareRes == 0) {
					return current;
				}
				if(compareRes < 0) {
					i = current + 1;
				}
				else {
					last = current - 1;
				}
			}
			return ~i;
		}		
		public override int CalcVisibleRowByPixel(int startIndex, int direction, int pixelPosition, bool allowFixedGroups) {
			int row = -1;
			if(direction == 1 && allowFixedGroups == View.AllowFixedGroups) {
				row = CalcVisibleRowByPixelCached(startIndex, pixelPosition);
				if(row >= 0 && row < this.rowsCachePixelHeight.Count) return row;
			}
			bool isGrouped = View.GroupCount > 0;
			int res = 0, n = 0;
			VisibleIndexCollection vi = View.DataController.GetVisibleIndexes();
			int viCount = isGrouped ? vi.Count : View.RowCount;
			if(row >= viCount) return viCount - 1;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(n = startIndex; n < viCount && n >= 0; n += direction) {
					int index = isGrouped ? vi[n] : n;
					res += GetCachedRowHeight(allowFixedGroups, index, n, startIndex == 0 && View.AllowFixedGroups == allowFixedGroups);
					if(res > pixelPosition) return Math.Max(0, n);
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
			return Math.Max(0, n - 1);
		}
	}
}
