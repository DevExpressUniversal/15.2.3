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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System.Linq;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotFieldsAreaViewInfo : PivotFieldsAreaViewInfoBase {
		public PivotFieldsAreaViewInfo(PivotGridViewInfo viewInfo, bool isColumn)
			: base(viewInfo, isColumn) { }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotFieldsAreaCellViewInfo this[int index] { get { return (PivotFieldsAreaCellViewInfo)base[index]; } }
		protected override PivotFieldsAreaCellViewInfoBase CreateFieldsAreaCellViewInfo(PivotFieldValueItem item) {
			return new PivotFieldsAreaCellViewInfo(ViewInfo, item);
		}
		public object[] GetValues(PivotFieldsAreaCellViewInfo CellViewInfo) {
			object[] result = new object[CellViewInfo.StartLevel + 1];
			PivotFieldsAreaCellViewInfo curViewInfo = CellViewInfo;
			result[curViewInfo.StartLevel] = GetCellValue(curViewInfo);
			if(result.Length == 1) return result;
			for(int i = CellViewInfo.Item.Index - 1; i >= 0; i--)
				if(this[i].StartLevel < curViewInfo.StartLevel) {
					curViewInfo = this[i];
					result[curViewInfo.StartLevel] = GetCellValue(curViewInfo);
					if(curViewInfo.StartLevel == 0)
						return result;
				}
			return null;
		}
		public PivotFieldsAreaCellViewInfo GetCellByValues(object[] values) {
			return GetCellByValues(values, 0, 0);
		}
		public PivotFieldsAreaCellViewInfo GetCellByValues(object[] values, int startIndex, int level) {
			for(int i = startIndex; i < ChildCount; i++) {
				if(this[i].StartLevel == level && CheckCellValue(this[i], values[level])) {
					if(this[i].StartLevel == values.Length - 1) return this[i];
					else return GetCellByValues(values, i + 1, level + 1);
				}
			}
			return null;
		}
		object GetCellValue(PivotFieldsAreaCellViewInfo cellInfo) {
			if(cellInfo.Item.ItemType == PivotFieldValueItemType.DataCell ||
					cellInfo.Item.ItemType == PivotFieldValueItemType.TopDataCell) {
				return cellInfo.Text;
			} else
				return cellInfo.Value;
		}
		bool CheckCellValue(PivotFieldsAreaCellViewInfo cellInfo, object value) {
			object cellValue = cellInfo.Item.ItemType == PivotFieldValueItemType.DataCell ||
				cellInfo.Item.ItemType == PivotFieldValueItemType.TopDataCell ? cellInfo.Text : cellInfo.Value;
			return object.Equals(value, cellValue);
		}
		public PivotFieldsAreaCellViewInfo GetValueFromTotal(PivotFieldsAreaCellViewInfo cellViewInfo) {
			if(cellViewInfo.ValueType == PivotGridValueType.Value)
				return cellViewInfo;
			if(cellViewInfo.ValueType != PivotGridValueType.Total && cellViewInfo.ValueType != PivotGridValueType.CustomTotal)
				return null;
			PivotFieldsAreaCellViewInfo curCellViewInfo = cellViewInfo;
			do {
				int newIndex = curCellViewInfo.Item.Index + (Data.OptionsView.IsTotalsFar(IsColumn, PivotGridValueType.Total) ? -1 : 1);
				if(newIndex < 0 || newIndex == ChildCount) 
					return null;
				curCellViewInfo = this[newIndex];
			}
			while(curCellViewInfo.ValueType != PivotGridValueType.Value && curCellViewInfo.StartLevel == cellViewInfo.StartLevel);
			return curCellViewInfo;
		}
		public int GetDataFieldsBeforeCount(PivotFieldsAreaCellViewInfo cellViewInfo) {
			int result = 0;
			for(int i = cellViewInfo.Item.Index; i >= 0; i--)
				if(this[i].ResizingField != null && this[i].ResizingField.IsDataField)
					result++;
			return result;
		}
	}
	public class PivotFieldsAreaViewInfoBase : PivotViewInfo, IFieldValueAreaBestFitProvider {
		readonly List<int> firstLevelIndexes;
		readonly bool isColumn;
		public PivotFieldsAreaViewInfoBase(PivotGridViewInfoBase viewInfo, bool isColumn)
			: base(viewInfo) {
			this.isColumn = isColumn;
			this.firstLevelIndexes = new List<int>();
		}
		public bool IsColumn { get { return isColumn; } }
		public int LevelCount { get { return VisualItems.GetLevelCount(IsColumn); } }
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public new PivotFieldsAreaCellViewInfoBase this[int index] { get { return (PivotFieldsAreaCellViewInfoBase)base[index]; } }
		public int LastLevelItemCount { get { return VisualItems.GetLastLevelItemCount(IsColumn); } }
		public PivotFieldsAreaCellViewInfoBase GetViewInfoByItem(PivotFieldValueItem item) {
			return (PivotFieldsAreaCellViewInfoBase)this[item.UniqueIndex];
		}
		protected PivotFieldValueItem GetLastLevelItem(int index) {
			return VisualItems.GetLastLevelItem(IsColumn, index);
		}
		protected PivotFieldValueItem GetItem(int index) {
			return VisualItems.GetItem(IsColumn, index);
		}
		protected PivotFieldValueItem GetParentItem(PivotFieldValueItem item) {
			return VisualItems.GetParentItem(IsColumn, item);
		}
		protected int ItemCount { get { return VisualItems.GetItemCount(IsColumn); } }		
		public PivotFieldsAreaCellViewInfoBase GetLastLevelViewInfo(int index) {
			PivotFieldValueItem item = GetLastLevelItem(index);
			return item != null ? GetViewInfoByItem(item) : null;
		}
		public PivotFieldsAreaCellViewInfoBase GetItem(int lastLevelIndex, int level) {
			PivotFieldValueItem item = GetLastLevelItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = GetParentItem(item);
			return item != null ? GetViewInfoByItem(item) : null;
		}
		public string GetChartText(int index) {
			PivotFieldValueItem item = GetLastLevelItem(index);
			string result = GetItemDisplayText(item);
			if(item.StartLevel == 0) return result;
			while((item = GetParentItem(item)) != null)
				result = GetItemDisplayText(item) + " | " + result;
			return result;
		}
		protected string GetItemDisplayText(PivotFieldValueItem item) {
			return GetViewInfoByItem(item).DisplayText;
		}
		public int GetVisibleColumnCount(int width) {
			if(IsColumn) throw new Exception("Method is not implemented");
			List<PivotFieldItemBase> fields = FieldItems.GetFieldItemsByArea(Area, true);
			int count = 0, widthLeft = width;
			for(int i = 0; i < fields.Count; i++) {
				widthLeft -= fields[i].Width;
				if(widthLeft < 0) break;
				count++;
			}
			return count;
		}
		public int GetLastVisibleColumnCount(ref int width) {
			if(IsColumn) throw new Exception("Method is not implemented");
			List<PivotFieldItemBase> fields = FieldItems.GetFieldItemsByArea(Area, true);
			int count = 0, widthLeft = width;
			for(int i = fields.Count - 1; i >= 0; i--) {
				widthLeft -= fields[i].Width;
				if(widthLeft < 0) break;
				count++;
			}
			return count;
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			for(int i = 0; i < ChildCount; i ++)
				if(this[i].PaintBounds.Contains(pt))
					return this[i].GetToolTipObjectInfo(pt);
			return null;
		}
		int CalcDistanceToNearBorder(int x, Rectangle rect) {
			return Math.Abs(IsRightToLeft ?
				x - rect.Right : 
				x - rect.Left);
		}
		int CalcDistanceToFarBorder(int x, Rectangle rect) {
			return Math.Abs(IsRightToLeft ?
			x  - rect.Left :
			x - rect.Right);
		}
		int GetXDirection() {
			return IsRightToLeft ? -1 : 1;
		}
		public PivotFieldItem GetSizingField(Point pt) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(pt, false);
			if(viewInfo == null) return null;
			if(CalcDistanceToNearBorder(pt.X, viewInfo.PaintBounds) <= ViewInfo.FrameBorderWidth) {
				pt.X -= ViewInfo.FrameBorderWidth * GetXDirection();
				viewInfo = GetViewInfoAtPoint(pt, false);
			}
			PivotFieldsAreaCellViewInfoBase cellViewInfo = viewInfo as PivotFieldsAreaCellViewInfoBase;
			if(cellViewInfo == null) return null;
			if(CalcDistanceToFarBorder(pt.X, viewInfo.PaintBounds) <= ViewInfo.FrameBorderWidth 
				&& (!IsColumn || (cellViewInfo.EndLevel == LevelCount - 1))) 
				return cellViewInfo.ResizingField;
			return null;
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(hitPoint, false);
			PivotFieldsAreaCellViewInfo cellViewInfo = viewInfo as PivotFieldsAreaCellViewInfo;
			if(cellViewInfo == null)
				return new PivotGridHitInfo(hitPoint);
			return cellViewInfo.CalcHitInfo(hitPoint);
		}
		public int ScrollColumnCount { 
			get { 
				int count = FieldItems.GetFieldCountByArea(Area);
				if(count == 0) count ++;
				return count;
			}
		}
		public int ScrollOffset {
			get {
				int leftTop = IsColumn ? ViewInfo.LeftTopCoord.Y : ViewInfo.LeftTopCoord.X;
				if(leftTop >= ScrollColumnCount)
					return IsColumn ? Bounds.Height : Bounds.Width;
				int offset = 0;
				for(int i = 0; i < leftTop; i ++)
					offset += IsColumn ? FieldMeasures.DefaultFieldValueHeight : FieldItems.GetFieldItemByArea(Area, i).Width;
				return offset;
			}
		}
		protected override void OnBeforeCalculating() {
			this.firstLevelIndexes.Clear();
			int itemCount = ItemCount;
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = GetItem(i);
				PivotFieldsAreaCellViewInfoBase valueViewInfo = CreateFieldsAreaCellViewInfo(item);
				AddChild(valueViewInfo);
				if(valueViewInfo.StartLevel == 0)
					this.firstLevelIndexes.Add(i);
			}
		}
		protected virtual PivotFieldsAreaCellViewInfoBase CreateFieldsAreaCellViewInfo(PivotFieldValueItem item) {
			return new PivotFieldsAreaCellViewInfoBase(ViewInfo, item);
		}
		protected override void OnAfterCalculated() {
			SetParents();
			int levelCount = LevelCount;
			int cellLeft = 0;
			int nextCellTop = 0;
			PivotFieldsAreaCellViewInfoBase lastTopCell = null;
			for(int curLevel = 0; curLevel < levelCount; curLevel++) {
				cellLeft = 0;
				for(int i = 0; i < ChildCount; i++) {
					PivotFieldsAreaCellViewInfoBase child = this[i];
					if(child.Level == curLevel) {
						if(IsColumn) {
							PivotFieldsAreaCellViewInfoBase parentCell = GetParentCell(i);
							child.Y = (parentCell != null) ? parentCell.Bounds.Bottom : 0;
						} else {
							PivotFieldsAreaCellViewInfoBase parentCell = GetParentCell(i);
							child.X = (parentCell != null) ? parentCell.Bounds.Right : 0;
						}
						child.IsTopMost = (IsColumn && curLevel == 0) || (!IsColumn && cellLeft == 0);
						if((IsColumn && curLevel == 0) || (!IsColumn && curLevel == levelCount - 1 && cellLeft == 0))
							lastTopCell = child;
					}
					if(child.ContainsLevel(curLevel)) {
						cellLeft += FieldMeasures.GetFieldValueSeparator(child.Item);
						if(IsColumn) {
							child.X = cellLeft;
							cellLeft += child.Bounds.Width;
							if(child.EndLevel == curLevel)
								nextCellTop = Math.Max(nextCellTop, child.Bounds.Bottom);
						} else {
							child.Y = cellLeft;
							cellLeft += child.Bounds.Height;
							if(child.EndLevel == curLevel)
								nextCellTop = Math.Max(nextCellTop, child.Bounds.Right);
						}
						if(child.Bounds.X == 0)
							child.HeaderPosition = HeaderPositionKind.Left;
					}
				}
			}
			if(lastTopCell != null && lastTopCell.Bounds.X > 0)
				lastTopCell.HeaderPosition = HeaderPositionKind.Right;
			if(ChildCount == 1)
				Size = this[0].Bounds.Size;
			else
				Size = IsColumn ? new Size(cellLeft, nextCellTop) : new Size(nextCellTop, cellLeft);
		}
		void SetParents() {
			PivotFieldsAreaCellViewInfoBase[] parents = new PivotFieldsAreaCellViewInfoBase[LevelCount];
			for(int i = 0; i < ChildCount; i++) {
				PivotFieldsAreaCellViewInfoBase cell = this[i];
				int parentLevel = cell.StartLevel - 1;
				if(parentLevel >= 0 && parentLevel < parents.Length)
					cell.ParentCache = parents[parentLevel];
				for(int j = cell.StartLevel; j <= cell.EndLevel; j++) {
					parents[j] = cell;
				}
			}
		}
		PivotFieldsAreaCellViewInfoBase GetParentCell(int cellIndex) {
			PivotFieldsAreaCellViewInfoBase cell = this[cellIndex];
			if(cell.ParentCache != null)
				return cell.ParentCache;
			PivotFieldsAreaCellViewInfoBase result = null;			
			int parentEndLevel = cell.StartLevel - 1;
			if(parentEndLevel == -1)
				return result;
			for(int i = cellIndex - 1; i >= 0; i--) {
				if(this[i].EndLevel == parentEndLevel) {
					result = this[i];
					break;
				}
			}
			cell.ParentCache = result;
			return result;
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.GetFieldSortConditions(IsColumn, cellViewInfo.Item.Index);			
		}
		public bool IsFieldSortedBySummary(PivotFieldItemBase field, PivotFieldItemBase dataField, PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.IsFieldSortedBySummary(IsColumn, field, dataField, cellViewInfo.Item.Index);
		}
		public bool GetIsAnyFieldSortedBySummary(PivotFieldsAreaCellViewInfoBase cellViewInfo) {
			return VisualItems.GetIsAnyFieldSortedBySummary(IsColumn, cellViewInfo.Item.Index);
		}
		#region IFieldValueAreaBestFitProvider
		internal GraphicsCache bestFitGraphicsCache = null;
		void IFieldValueAreaBestFitProvider.BeginBestFitCalculcations() {
			Graphics bestFitGraphics = GraphicsInfo.Default.AddGraphics(null);
			bestFitGraphicsCache = new GraphicsCache(bestFitGraphics);
		}
		void IFieldValueAreaBestFitProvider.EndBestFitCalculcations() {
			bestFitGraphicsCache.Dispose();
			bestFitGraphicsCache = null;
			GraphicsInfo.Default.ReleaseGraphics();
		}
		IPivotFieldValueItem IFieldValueAreaBestFitProvider.this[int index] {
			get {
				return this[index];
			}
		}
		#endregion
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			if(!IsColumn) {
				Color borderColor = Data.Appearance.FieldValueTopBorderColor;
				if(borderColor != null) {
					e.GraphicsCache.Paint.DrawLine(e.Graphics, e.GraphicsCache.GetPen(borderColor),
						new Point(PaintBounds.Left, PaintBounds.Top), new Point(PaintBounds.Right - 1, PaintBounds.Top));
				}
			}
			base.InternalPaint(e);
		}
	}
	public class PivotFieldsAreaCellViewInfo : PivotFieldsAreaCellViewInfoBase {
		public PivotFieldsAreaCellViewInfo(PivotGridViewInfo viewInfo, PivotFieldValueItem item)
			: base(viewInfo, item) {
		}
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		protected override string DefaultValueToolTip {
			get {
				if(Field != null && (ValueType == PivotGridValueType.Value || Field.Area == PivotArea.DataArea))
					return Field.ToolTips.GetValueText(Value);
				return string.Empty;
			}
		}
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override IDXMenuManager GetMenuManager() {
			return Data.MenuManager;
		}
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
			return Data.EventArgsHelper.CreateMenuEventArgs(menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		protected override PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return Data.EventArgsHelper.CreateGridMenuItemClickEventArgs(this.menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation, menuItem);
		}
		public override string DisplayText {
			get { return Item.DisplayText; } 
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			string defaultToolTip = DefaultValueToolTip;
			if(defaultToolTip != string.Empty) {
				return new ToolTipControlInfo(this, defaultToolTip);
			}
			if(!Data.OptionsHint.ShowValueHints) return null;
			return !IsCaptionFit ? new ToolTipControlInfo(this, DisplayText) : null; 
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(OpenCloseButtonInfoArgs != null && IsMouseOnCollapseButton(e.Location)) {
				DoChangeExpandedStateAsync();
			} else if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableFieldValueMenu)
				ShowPopupMenu(e);
			return this;
		}
		public override void DoubleClick(MouseEventArgs e) {
			if(Data.OptionsCustomization.AllowExpandOnDoubleClick && !IsMouseOnCollapseButton(e.Location)) {
				VisualItems.ChangeExpanded(IsColumn, Item);
			}
			base.DoubleClick(e);
		}
		public override void ChangeExpanded() {
			Data.ChangeExpanded(Item);
		}
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			AddExpandCollapseMenuItems(menu);
			AddSortBySummaryMenuItems(menu);
			AddExpressionMenuItems(menu);
		}
		internal void DoChangeExpandedStateAsync() {
			PivotFieldsAreaViewInfo parent = IsColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
			PivotFieldsAreaCellViewInfo oldCellViewInfo = parent.GetValueFromTotal(this);
			int oldCellCount = oldCellViewInfo.Item.CellCount + oldCellViewInfo.Item.TotalsCount,
				dataFieldsBefore = parent.GetDataFieldsBeforeCount(oldCellViewInfo);
			if(dataFieldsBefore > 0)
				dataFieldsBefore--;
			object[] values = parent.GetValues(oldCellViewInfo);
			Point oldLeftTopCoord = ViewInfo.LeftTopCoord;
			Data.ChangeExpandedAsync(Item, false, delegate(AsyncOperationResult result) {
				ViewInfo.EnsureIsCalculated();
				parent = IsColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
				if(Data.OptionsDataField.DataFieldVisible &&
						(Data.OptionsDataField.Area == PivotDataArea.ColumnArea && IsColumn ||
						Data.OptionsDataField.Area == PivotDataArea.RowArea && !IsColumn) &&
						Data.OptionsDataField.AreaIndex < Item.StartLevel) {
					PivotFieldsAreaCellViewInfo newCellViewInfo = parent.GetCellByValues(values);
					if(newCellViewInfo == null) return;
					newCellViewInfo = parent.GetValueFromTotal(newCellViewInfo);
					int newCellCount = newCellViewInfo.Item.CellCount + newCellViewInfo.Item.TotalsCount,
						deltaIndex = (newCellCount - oldCellCount + (newCellViewInfo.IsCollapsed ? 1 : -1)) * dataFieldsBefore;
					if(IsColumn)
						ViewInfo.LeftTopCoord = new Point(oldLeftTopCoord.X + deltaIndex, oldLeftTopCoord.Y);
					else
						ViewInfo.LeftTopCoord = new Point(oldLeftTopCoord.X, oldLeftTopCoord.Y + deltaIndex);
				}
			});
		}
		void AddExpandCollapseMenuItems(DXPopupMenu menu) {
			if(Item.ShowCollapsedButton)
				CreateExpandCollapseMenuItems(menu);
		}
		void AddSortBySummaryMenuItems(DXPopupMenu menu) {
			if(Item.CanShowSortBySummary)
				CreateSortBySummaryMenuItems(menu);
		}
		void AddExpressionMenuItems(DXPopupMenu menu) {
			if(Field != null && Field.CanShowUnboundExpressionMenu)
				CreateExpressionMenuItems(menu);
		}
		void CreateExpandCollapseMenuItems(DXPopupMenu menu) {
			menu.Items.Add(CreateMenuItem(Item.IsCollapsed ? GetLocalizedString(PivotGridStringId.PopupMenuExpand) : GetLocalizedString(PivotGridStringId.PopupMenuCollapse), PivotContextMenuIds.ChangeExpandedMenuID));
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuExpandAll), PivotContextMenuIds.ExpandAllMenuID));
				SetBeginGrouptoLastMenuItem();
				menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuCollapseAll), PivotContextMenuIds.CollapseAllMenuID));
		}
		void CreateExpressionMenuItems(DXPopupMenu menu) {
			DXMenuItem item = CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuShowExpression), PivotContextMenuIds.ShowValueExpressionMenuID);
			item.BeginGroup = true;
			menu.Items.Add(item);
		}
		protected virtual void CreateSortBySummaryMenuItems(DXPopupMenu menu) {
			List<PivotFieldItemBase> crossAreaFields = Item.GetCrossAreaFields();
			bool showRemoveAllSortingItem = false;
			string captionTemplate = GetLocalizedString(Item.Area == PivotArea.ColumnArea ? PivotGridStringId.PopupMenuSortFieldByColumn : PivotGridStringId.PopupMenuSortFieldByRow);
			List<PivotFieldItemBase> dataFields = IsDataLocatedInThisArea || FieldItems.DataFieldCount == 1 ? null : FieldItems.GetFieldItemsByArea(PivotArea.DataArea, false);
			for(int i = 0; i < crossAreaFields.Count; i++) {
				PivotFieldItem field = crossAreaFields[i] as PivotFieldItem;
				if(!field.CanSortBySummary) continue;
				if(IsDataLocatedInThisArea || FieldItems.DataFieldCount == 1) {
					showRemoveAllSortingItem |= CreateSortByMenuItem(menu, captionTemplate, field, Item.DataField, i == 0);
				} else {
					showRemoveAllSortingItem |= CreateSortByWithDataMenuItems(menu, captionTemplate, field, dataFields, i == 0);
				}
			}
			if(showRemoveAllSortingItem) {
				menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuRemoveAllSortByColumn),
					PivotContextMenuIds.RemoveSortBySummaryMenuID, true, -1, true));
			}
		}
		bool CreateSortByMenuItem(DXPopupMenu menu, string captionTemplate, PivotFieldItemBase field, PivotFieldItemBase dataField, bool beginGroup) {
			string caption = string.Format(captionTemplate, field.HeaderDisplayText);
			return CreateSortByMenuItemCore(menu, field, dataField, beginGroup, caption);
		}
		bool CreateSortByMenuItemCore(DXPopupMenu menu, PivotFieldItemBase field, PivotFieldItemBase dataField, bool beginGroup, string caption) {
			bool isChecked = Parent.IsFieldSortedBySummary(field, dataField, this);
			menu.Items.Add(CreateMenuCheckItem(caption, isChecked, new PivotGridFieldPair(Data, field, dataField), beginGroup && menu.Items.Count > 0));
			return isChecked;
		}
		bool CreateSortByWithDataMenuItems(DXPopupMenu menu, string captionTemplate, PivotFieldItemBase field, List<PivotFieldItemBase> dataFields, bool beginGroup) {
			bool result = false;
			for(int i = 0; i < dataFields.Count; i++) {
				PivotFieldItem dataField = dataFields[i] as PivotFieldItem;
				string caption = string.Format(captionTemplate, field.HeaderDisplayText + " - " + dataField.HeaderDisplayText);
				result |= CreateSortByMenuItemCore(menu, field, dataField, beginGroup && i == 0, caption);
			}
			return result;
		}
		protected override void OnMenuItemClick(DXMenuItem menuItem) {
			if(menuItem.Tag is int) {
				switch((int)menuItem.Tag) {
					case PivotContextMenuIds.ChangeExpandedMenuID:
						Data.ChangeExpandedAsync(Item, false);
						break;
					case PivotContextMenuIds.ExpandAllMenuID:
						Data.ChangeFieldExpandedAsync(Data.GetField(Field), true, false);
						break;
					case PivotContextMenuIds.CollapseAllMenuID:
						Data.ChangeFieldExpandedAsync(Data.GetField(Field), false, false);
						break;
					case PivotContextMenuIds.RemoveSortBySummaryMenuID:
						RemoveSortBySummaryAsync();
						break;
					case PivotContextMenuIds.ShowValueExpressionMenuID:
						Data.PivotGrid.ShowUnboundExpressionEditor(Data.GetField(Field) as PivotGridField);
						break;
				}
			} else {
				PivotGridFieldPair pair = menuItem.Tag as PivotGridFieldPair;
				DXMenuCheckItem checkMenuItem = menuItem as DXMenuCheckItem;
				if(checkMenuItem == null || pair == null) return;
				SetFieldSortBySummaryAsync(pair.FieldItem, pair.DataFieldItem, checkMenuItem.Checked);
			}
		}
		void SetFieldSortBySummaryAsync(PivotFieldItemBase field, PivotFieldItemBase dataField, bool sort) {
			if(!field.CanSortBySummary) return;
			Data.BeginUpdate();
			try {
				SetFieldSortBySummaryCore(field, dataField, sort);
			} finally {
				Data.EndUpdateAsync(false);
			}
		}
		void RemoveSortBySummaryAsync() {
			List<PivotFieldItemBase> crossAreaFields = Item.GetCrossAreaFields();
			Data.BeginUpdate();
			try {
				foreach(PivotFieldItemBase field in crossAreaFields) {
					if(field.CanSortBySummary && Parent.IsFieldSortedBySummary(field, null, this))
						SetFieldSortBySummaryCore(field, null, false);
				}
			} finally {
				Data.EndUpdateAsync(false);
			}
		}
		protected internal List<PivotGridFieldSortCondition> GetFieldSortConditions() {
			return Parent.GetFieldSortConditions(this);
		}
		void SetFieldSortBySummaryCore(PivotFieldItemBase field, PivotFieldItemBase dataField, bool sort) {
			PivotGridField origField = Data.GetField(field) as PivotGridField,
				origDataField = Data.GetField(dataField) as PivotGridField;
			origField.SetSortBySummary(origDataField, GetFieldSortConditions(), CustomSummaryType, sort);
		}
		PivotSummaryType? CustomSummaryType {
			get {
				return Item.CustomTotal != null ? (PivotSummaryType?)Item.CustomTotal.SummaryType : null;
			}
		}
		protected override int GetImageIndex() {
			return Data.GetPivotFieldImageIndex(Item);
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotGridHitInfo hitInfo = new PivotGridHitInfo(this, IsMouseOnCollapseButton(hitPoint) ? PivotGridValueHitTest.ExpandButton : PivotGridValueHitTest.None, hitPoint);
			return hitInfo;
		}
		public bool IsMouseOnCollapseButton(Point pt) {
			return OpenCloseButtonInfoArgs != null && OpenCloseButtonInfoArgs.Bounds.Contains(pt);
		}
	}	
	public class PivotFieldsAreaCellViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner, IPivotFieldValueItem {
		 public const int HeaderTextOffset = CellSizeProvider.FieldValueTextOffset;
		readonly PivotFieldValueItem item;
		PivotFieldsAreaCellViewInfoBase parentCache;
		HeaderPositionKind headerPosition;
		bool isTopMost;
		Nullable<bool> isAnyFieldSortedBySummary;
		AppearanceObject appearance;
		internal PivotHeaderObjectInfoArgs info;
		bool isCaptionFit = false;
		public PivotFieldsAreaCellViewInfoBase(PivotGridViewInfoBase viewInfo, PivotFieldValueItem item)
			: base(viewInfo) {
			this.item = item;
			this.headerPosition = HeaderPositionKind.Center;
			this.isTopMost = false;
		}
		protected new PivotFieldsAreaViewInfoBase Parent { get { return (PivotFieldsAreaViewInfoBase)base.Parent; } }
		public PivotFieldValueItem Item { get { return item; } }
		public bool IsColumn { get { return Item.IsColumn; } }
		public int CellCount { get { return item.CellCount; } }
		public int VisibleIndex { get { return Item.VisibleIndex; } }
		public int Level {	get { return Item.Level; } }
		internal PivotFieldsAreaCellViewInfoBase ParentCache {
			get { return parentCache; }
			set { parentCache = value; }
		}
		public int CellLevelCount { get { return Item.CellLevelCount; } }
		public int StartLevel { get { return Item.StartLevel; }  }
		public int EndLevel { get { return Item.EndLevel; }  }
		public bool IsLastFieldLevel { get { return Item.IsLastFieldLevel; } }
		public bool ContainsLevel(int level) { return Item.ContainsLevel(level); }		
		public int DataIndex { get { return Item.DataIndex; } }
		public PivotSummaryType SummaryType { get { return Item.SummaryType; } }
		public PivotGridCustomTotal CustomTotal { get { return (PivotGridCustomTotal)Item.CustomTotal; } }
		public string Text { get { return Item.Text; } }
		public bool IsOthersValue { get { return Item.IsOthersRow; } }
		public PivotGridValueType ValueType { get {	return Item.ValueType; } }
		public object Value { get { return Item.Value; } }
		public PivotFieldItem Field { get { return (PivotFieldItem)Item.Field; } }
		public PivotFieldItem ColumnField { get { return (PivotFieldItem)Item.ColumnField; } }
		public PivotFieldItem RowField { get { return (PivotFieldItem)Item.RowField; } }
		public bool IsRowTree { get { return Item.IsRowTree; } }
		public PivotFieldItem ResizingField { get { return (PivotFieldItem)Item.ResizingField; } }
		public virtual string DisplayText { get { return Text; } }
		public bool IsCustomDisplayText { get { return Item.IsCustomDisplayText; } }
		public int MinLastLevelIndex { get { return Item.MinLastLevelIndex; } }
		public int MaxLastLevelIndex { get { return Item.MaxLastLevelIndex; } }
		protected bool IsDataLocatedInThisArea { get { return Item.IsDataFieldsVisible; } }
		protected bool IsCaptionFit { get { return isCaptionFit; } }
		public HeaderPositionKind HeaderPosition { get { return headerPosition; } set { headerPosition = value; } }
		public bool IsTopMost { get { return isTopMost; } set { isTopMost = value; } }
		public virtual int GetBestWidth(GraphicsCache graphicsCache) {
			HeaderObjectInfoArgs info = CreateHeaderInfoArgs(graphicsCache);
			return GetHeaderPainter().CalcObjectMinBounds(info).Width;
		}
		protected virtual string DefaultValueToolTip {
			get { return string.Empty; }
		}
		public int Separator {
			get {
				PivotFieldsAreaViewInfoBase fieldsArea = Parent;
				return fieldsArea != null ? FieldMeasures.GetFieldValueSeparator(this.Item) : 0;
			}
		}
		internal OpenCloseButtonInfoArgs OpenCloseButtonInfoArgs { get; set; }
		public AppearanceObject Appearance {
			get { 
				if(appearance == null)
					appearance = GetViewInfoAppearances().GetActualFieldValueAppearance(ValueType, Field);
				return appearance; 
			} 
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		bool EnableWrapping {
			get {
				return Appearance.Options.UseTextOptions && Appearance.TextOptions.WordWrap == WordWrap.Wrap;
			}
		}
		protected virtual PivotGridAppearancesBase GetViewInfoAppearances() {
			return ViewInfo.PrintAndPaintAppearance;
		}
		protected override void OnBeforeCalculating() {
			base.OnBeforeCalculating();
			ForEachCell(cell => cell.EnsureIsCalculated());
		}
		protected override void OnAfterCalculated() {
			Size = CalculateCellSize();
		}
		protected Size CalculateCellSize() {
			if(!IsColumn)
				return new Size(
								FieldMeasures.GetWidthDifference(false, item.StartLevel, item.EndLevel + 1),
								FieldMeasures.GetHeightDifference(false, item.MinLastLevelIndex, item.MaxLastLevelIndex + 1));
			return new Size(
							FieldMeasures.GetWidthDifference(true, item.MinLastLevelIndex, item.MaxLastLevelIndex + 1),
							FieldMeasures.GetHeightDifference(true, item.StartLevel, item.EndLevel + 1));
		}
		void ForEachCell(Action<PivotFieldsAreaCellViewInfoBase> action) {
			for(int i = 0; i < CellCount; i++) {
				PivotFieldsAreaCellViewInfoBase cell = Parent.GetViewInfoByItem(item.GetCell(i));
				if(cell == null) continue;
				action(cell);
			}
		}
		void CreateOpenCloseButton(PivotHeaderObjectInfoArgs info, GraphicsCache cache) {
			if(Item.ShowCollapsedButton) {
				OpenCloseButtonInfoArgs = new OpenCloseButtonInfoArgs(cache);
				OpenCloseButtonInfoArgs.Opened = !IsCollapsed;
				OpenCloseButtonInfoArgs.State = IsActive ? ObjectState.Pressed : ObjectState.Normal;
				info.OpenCloseButtonInfoArgs = OpenCloseButtonInfoArgs;
				info.InnerElements.Add(new DrawElementInfo(Data.ActiveLookAndFeel.Painter.OpenCloseButton, OpenCloseButtonInfoArgs, StringAlignment.Near));
			}
		}
		void CleateGlyphInfoArgs(PivotHeaderObjectInfoArgs info, GraphicsCache cache) {
			if(Data.ValueImages != null) {
				int imageIndex = GetImageIndex();
				if(imageIndex > -1) {
					bool allowGlyphSkinning = Data.OptionsView.AllowGlyphSkinning;
					GlyphElementPainter painter = PivotViewInfo.CreateGlyphPainter(allowGlyphSkinning);
					GlyphElementInfoArgs glyphElementInfoArgs = PivotViewInfo.CreateGlyphInfoArgs(Data.ValueImages, imageIndex, allowGlyphSkinning);
					glyphElementInfoArgs.Cache = cache;
					info.GlyphInfo = glyphElementInfoArgs;
					info.InnerElements.Add(new DrawElementInfo(painter, glyphElementInfoArgs, StringAlignment.Near));
				}
			}
		}
		void CleateIndicator(PivotHeaderObjectInfoArgs info, GraphicsCache cache) {
			if(IsAnyFieldSortedByThisSummary && IsLastFieldLevel) {
				Image image = null;
				if(Data.PaintAppearance.SortByColumnIndicatorImage != null)
					image = Data.PaintAppearance.SortByColumnIndicatorImage;
				else
					image = GetIndicatorPainter().ImageList.Images[9];
				if(image != null) {
					GlyphElementInfoArgs indicatorInfoArgs = new GlyphElementInfoArgs(null, 0, image);
					indicatorInfoArgs.Cache = cache;
					info.IndicatorInfoArgs = indicatorInfoArgs;
					info.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), indicatorInfoArgs, StringAlignment.Far));
				}
			}
		}
		protected PivotHeaderObjectInfoArgs CreateHeaderInfoArgs(GraphicsCache graphicsCache) {
			PivotHeaderObjectInfoArgs info = new PivotHeaderObjectInfoArgs();
			info.Cache = graphicsCache;
			info.Caption = DisplayText;
			Rectangle paintBounds = Rectangle.Intersect(PaintBounds, ViewInfo.PaintBoundsWithScroll);
			int minimumHeight = FieldMeasures.GetDefaultFieldValueHeight(Item) + GetNextRowTreeTotalItemHeight(false);
			bool isPaintBoundsCorrected = false;
			if(!paintBounds.IsEmpty && paintBounds.Height < minimumHeight && PaintBounds.Y == Parent.PaintBounds.Y) {
				isPaintBoundsCorrected = true;
				paintBounds = new Rectangle(PaintBounds.X, PaintBounds.Bottom - minimumHeight, PaintBounds.Width, minimumHeight);
			}
			info.Bounds = info.CaptionRect = paintBounds;
			info.AutoHeight = false;
			if(Appearance.TextOptions.VAlignment == VertAlignment.Default)
				Appearance.TextOptions.VAlignment = VertAlignment.Center;
			info.SetAppearance(Appearance);
			info.FieldValueTopBorderColor = Data.Appearance.FieldValueTopBorderColor;
			info.FieldValueLeftRightBorderColor = Data.Appearance.FieldValueLeftRightBorderColor;
			CleateIndicator(info, graphicsCache);
			CleateGlyphInfoArgs(info, graphicsCache);
			if(ControlBounds.Size == Bounds.Size)
				CreateOpenCloseButton(info, graphicsCache);
			if(HeaderPosition == HeaderPositionKind.Right && (!IsColumn || ViewInfo.IsVScrollBarVisible))
				info.HeaderPosition = HeaderPositionKind.Center;
			else info.HeaderPosition = HeaderPosition;
			info.IsTopMost = IsTopMost || (Parent.PaintBounds.Top == info.Bounds.Top);
			info.NeedsRowTreeBorder = IsRowTree && MinLastLevelIndex > 0;
			info.IsAfterTopRowTree = info.NeedsRowTreeBorder && VisualItems.GetRowItem(MinLastLevelIndex - 1).ValueType == PivotGridValueType.Total;
			info.IsLeftMost = IsRightToLeft ? Parent.PaintBounds.Right == info.Bounds.Right : Parent.PaintBounds.Left == info.Bounds.Left;
			info.RightToLeft = IsRightToLeft;
			info.IsRowTree = IsRowTree;
			info.MinimumHeight = minimumHeight;
			info.NextRowTreeTotalItemHeight = IsRowTree ? GetNextRowTreeTotalItemHeight(!isPaintBoundsCorrected) : 0;
			GetHeaderPainter().CalcObjectBounds(info);
			isCaptionFit = GetHeaderPainter().IsCaptionFit(graphicsCache, info);
			return info;
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			this.info = CreateHeaderInfoArgs(e.GraphicsCache);
			GraphicsInfoState clipState = e.GraphicsCache.ClipInfo.SaveState();
			if(!IsColumn) {
				int offset = info.FieldValueTopBorderColor != null ? 0 : 1;
				 Rectangle clipRectangle = new Rectangle {
					X = Parent.PaintBounds.X,
					Y = Parent.PaintBounds.Y - offset,
					Width = Parent.PaintBounds.Width,
					Height = Parent.PaintBounds.Height
				};
				e.GraphicsCache.ClipInfo.SetClip(clipRectangle);
			}
			MethodInvoker defaultDraw = () => {
				GetHeaderPainter().DrawObject(info);
			};
			if(!ViewInfo.CustomDrawFieldValue(e, this, info, GetHeaderPainter(), defaultDraw))
				defaultDraw();
			e.GraphicsCache.ClipInfo.RestoreState(clipState);
		}
		protected Rectangle BaseCalculatePaintBounds() {
			return base.CalculatePaintBounds();
		}
		protected override Rectangle CalculatePaintBounds() {
			if(!Item.IsVisible)
				return Rectangle.Empty;
			Rectangle oldBounds = Bounds;
			try {
				Height += GetNextRowTreeTotalItemHeight(false);
				return base.CalculatePaintBounds();
			} finally {
				Bounds = oldBounds;
			}
		}
		internal int GetNextRowTreeTotalItemHeight(bool useOnlyVisibleHeight) {
			int res = 0;
			if(IsRowTree && ValueType == PivotGridValueType.Total) {
				PivotFieldsAreaCellViewInfoBase nextItem = GetNextItem(Item);
				if(nextItem != null) {
					Rectangle paintBounds = useOnlyVisibleHeight ? Rectangle.Intersect(nextItem.BaseCalculatePaintBounds(), ViewInfo.PaintBounds) : nextItem.Bounds;
					res = paintBounds.Height;
				}
			}
			return res;
		}
		internal PivotFieldsAreaCellViewInfoBase GetNextItem(PivotFieldValueItem Item) {
			for(int i = Item.Index + 1; i < Parent.ChildCount; i++) {
				if(Parent[i].Level < Item.Level)
					break;
				if(Parent[i].Level > Item.Level) 
					continue;
				if(Parent[i].ValueType == PivotGridValueType.Value)
					return Parent[i];
				else
					break;
			}
			return null;
		}
		protected virtual int GetImageIndex() {
			return -1;
		}
		protected PivotHeaderObjectPainter GetHeaderPainter() {
			return new PivotHeaderObjectPainter(Data.ActiveLookAndFeel.Painter.Header);
		}
		protected IndicatorObjectPainter GetIndicatorPainter() {
			return Data.ActiveLookAndFeel.Painter.Indicator;
		}
		protected Rectangle HeaderPaintBounds { get {return PaintBounds; } }
		protected override PivotGridMenuType MenuType { get { return PivotGridMenuType.FieldValue; } }
		protected override PivotArea MenuArea { get { return Item.Area; } }		
		public bool IsCollapsed { get { return Item.IsCollapsed; } }
		public virtual void ChangeExpanded() {}
		protected internal bool IsAnyFieldSortedByThisSummary { 
			get {
				if(isAnyFieldSortedBySummary == null)
					isAnyFieldSortedBySummary = Parent.GetIsAnyFieldSortedBySummary(this);
				return isAnyFieldSortedBySummary.Value; 
			} 
		}
		#region IPivotFieldValueItem
		int IPivotFieldValueItem.GetBestWidth() {
			return GetBestWidth(Parent.bestFitGraphicsCache);
		}
		PivotFieldValueItem IPivotFieldValueItem.Item {
			get { return Item; }
		}
		#endregion
	}
	public class PivotHeaderObjectInfoArgs : HeaderObjectInfoArgs {
		public GlyphElementInfoArgs GlyphInfo { get; set; }
		public Color FieldValueTopBorderColor { get; set; }
		public Color FieldValueLeftRightBorderColor { get; set; }
		public bool NeedsRowTreeBorder { get; set; }
		public bool IsAfterTopRowTree { get; set; }
		public OpenCloseButtonInfoArgs OpenCloseButtonInfoArgs { get; set; }
		public GlyphElementInfoArgs IndicatorInfoArgs { get; set; }
		public bool IsLeftMost { get; set; }
		public int NextRowTreeTotalItemHeight { get; set; }
		public bool IsRowTree { get; set; }
		public int MinimumHeight { get; set; }
	}
	public class PivotHeaderObjectPainter : HeaderObjectPainter {
		readonly HeaderObjectPainter painter;
		public PivotHeaderObjectPainter(HeaderObjectPainter painter)
			: base(painter.ButtonPainter) {
			this.painter = painter;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			this.painter.DrawObject(e);
			PivotHeaderObjectInfoArgs info = (PivotHeaderObjectInfoArgs)e;
			DrawTopBorder(info, info.FieldValueTopBorderColor);
			DrawLeftRightBorder(info, info.FieldValueLeftRightBorderColor);
		}
		public void DrawGlyph(PivotHeaderObjectInfoArgs info) {
			if (info.GlyphInfo != null) {
				info.InnerElements[0].ElementInfo.Cache = info.Cache;
				info.InnerElements[0].ElementPainter.DrawObject(info.GlyphInfo);
				info.InnerElements[0].ElementInfo.Cache = null;
			}
		}
		public void DrawIndicator(PivotHeaderObjectInfoArgs info) {
			if (info.IndicatorInfoArgs != null) {
				info.InnerElements[0].ElementInfo.Cache = info.Cache;
				info.InnerElements[0].ElementPainter.DrawObject(info.IndicatorInfoArgs);
				info.InnerElements[0].ElementInfo.Cache = null;
			}
		}
		void DrawLeftRightBorder(PivotHeaderObjectInfoArgs info, Color borderColor) {
			if(borderColor.IsEmpty) return;
			Rectangle bounds = info.Bounds;
			int near = info.RightToLeft ? bounds.Right :  bounds.Left - 1;
			if(info.NeedsRowTreeBorder || info.HeaderPosition == HeaderPositionKind.Left || info.IsLeftMost)
				DrawBorder(info, borderColor, near, near, bounds.Top - 1, bounds.Bottom - 1);
		}
		void DrawTopBorder(PivotHeaderObjectInfoArgs info, Color borderColor) {
			if(borderColor.IsEmpty) return;
			Rectangle bounds = info.Bounds;
			if(info.IsTopMost || info.IsAfterTopRowTree)
				DrawBorder(info, borderColor, bounds.Left, bounds.Right - 1, bounds.Top - 1, bounds.Top - 1);
		}
		void DrawBorder(PivotHeaderObjectInfoArgs info, Color borderColor, int x1, int x2, int y1, int y2) {
			info.Cache.Paint.DrawLine(info.Cache.Graphics, info.Cache.GetPen(borderColor), new Point(x1, y1), new Point(x2, y2));
		}
		#region Wrappers
		public override AppearanceDefault DefaultAppearance { get { return this.painter.DefaultAppearance; } }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return this.painter.CalcBoundsByClientRectangle(e, client);
		}
		Rectangle SetRectangleTop(Rectangle rect, int top) {
			return new Rectangle(rect.Left, top, rect.Width, rect.Height);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			Rectangle rect = this.painter.CalcObjectBounds(e);
			Rectangle minBounds = this.painter.CalcObjectMinBounds(e);
			PivotHeaderObjectInfoArgs pivotHeaderInfoArgs = e as PivotHeaderObjectInfoArgs;
			if(pivotHeaderInfoArgs != null) {
				int captionTop = pivotHeaderInfoArgs.CaptionRect.Top + 1;
				if(pivotHeaderInfoArgs.OpenCloseButtonInfoArgs != null)
					pivotHeaderInfoArgs.OpenCloseButtonInfoArgs.Bounds = SetRectangleTop(pivotHeaderInfoArgs.OpenCloseButtonInfoArgs.Bounds, captionTop);
				if(pivotHeaderInfoArgs.GlyphInfo != null)
					pivotHeaderInfoArgs.GlyphInfo.Bounds = SetRectangleTop(pivotHeaderInfoArgs.GlyphInfo.Bounds, captionTop);
				if(pivotHeaderInfoArgs.IndicatorInfoArgs != null)
					pivotHeaderInfoArgs.IndicatorInfoArgs.Bounds = SetRectangleTop(pivotHeaderInfoArgs.IndicatorInfoArgs.Bounds, captionTop + 2); 
			}
			CorrectCaptionRect(pivotHeaderInfoArgs);
			return rect;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return this.painter.CalcObjectMinBounds(e);
		}
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
			this.painter.DrawCaption(e, caption, font, brush, bounds, format);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return this.painter.GetObjectClientRectangle(e);
		}
		public override AppearanceObject GetStyle(ObjectInfoArgs e) {
			return this.painter.GetStyle(e);
		}
		public override bool IsCaptionFit(GraphicsCache cache, ObjectInfoArgs e) {
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			if(ee.Caption.Length == 0)
				return true;
			if(ee.CaptionRect.Width == 0)
				return false;
			else {
				Size captionSize = CalcCaptionTextSize(cache, ee, ee.Caption);
				return captionSize.Width <= ee.CaptionRect.Width && captionSize.Height <= ee.CaptionRect.Height;
			}
		}
		void CorrectCaptionRect(PivotHeaderObjectInfoArgs info) {
			int leftOffset = 2;
			int topOffset = 1;
			info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + leftOffset, info.CaptionRect.Top + topOffset), info.CaptionRect.Size);
			if(info.IsRowTree) {
				info.CaptionRect = new Rectangle(
					info.CaptionRect.Left,
					info.CaptionRect.Top,
					info.CaptionRect.Width,
					info.CaptionRect.Height - info.NextRowTreeTotalItemHeight
					);
			} else if(info.Bounds.Height > info.MinimumHeight) {
				info.Appearance.TextOptions.VAlignment = VertAlignment.Top;
				int expectedBaselineY = info.Bounds.Y + info.MinimumHeight / 2;
				Font font = info.Appearance.Font;
				FontFamily fontFamily = font.FontFamily;
				int lineHeight = (int)Math.Round(font.GetHeight(info.Cache.Graphics) / 2);
				info.CaptionRect = new Rectangle(
					info.CaptionRect.Left,
					expectedBaselineY - lineHeight,
					info.CaptionRect.Width,
					info.CaptionRect.Height
					);
			}
		}
		#endregion
	}
}
