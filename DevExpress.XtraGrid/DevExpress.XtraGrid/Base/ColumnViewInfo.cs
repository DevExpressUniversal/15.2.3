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
using System.Data;
using DevExpress.Utils;	
using DevExpress.Utils.Drawing;	
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Views.Base.ViewInfo {
	public class ConditionInfo {
		AppearanceObjectEx rowConditionAppearance;
		Hashtable cellAppearances, cellDescriptionAppearances;
		bool calculated, ignore;
		public ConditionInfo() {
			Clear();
		}
		public AppearanceObjectEx RowConditionAppearance {
			get { return ignore ? null : rowConditionAppearance; }
			set {
				rowConditionAppearance = value;
			}
		}
		public bool IsCalculated { 
			get { return calculated; } 
			set { calculated = value; }
		}
		public Hashtable CellAppearances {
			get { return cellAppearances; }
			set { cellAppearances = value; }
		}
		protected Hashtable CellDescriptionAppearances {
			get { return cellDescriptionAppearances; }
			set { cellDescriptionAppearances = value; }
		}
		internal void Obsolete() {
			Clear();
			calculated = true;
			this.ignore = true;
		}
		public virtual void Clear() {
			this.ignore = false;
			this.calculated = false;
			this.cellAppearances = this.cellDescriptionAppearances = null;
			this.rowConditionAppearance = null;
		}
		protected virtual AppearanceObjectEx GetCellAppearanceCore(GridColumn column) {
			if(CellAppearances == null || ignore) return null;
			return CellAppearances[column] as AppearanceObjectEx;
		}
		protected internal AppearanceObjectEx GetCellDescriptionAppearance(GridColumn column) {
			if(CellDescriptionAppearances == null || ignore) return null;
			AppearanceObjectEx descAppearance = CellDescriptionAppearances[column] as AppearanceObjectEx;
			return descAppearance == null ? RowConditionAppearance : descAppearance;
		}
		public AppearanceObjectEx GetCellAppearance(GridColumn column) {
			if(ignore) return null;
			AppearanceObjectEx cellAppearance = GetCellAppearanceCore(column);
			return cellAppearance == null ? RowConditionAppearance : cellAppearance;
		}
		public void CheckCondition(StyleFormatCondition cond, GridColumn column, object value, int listSourceRow) {
			if(cond.CheckValue(column, value, listSourceRow)) {
				ApplyCondition(cond, column);
			}
		}
		void ApplyCondition(StyleFormatCondition cond, GridColumn column) {
			AppearanceObjectEx appearance = cond.Appearance.Clone() as AppearanceObjectEx;
			AppearanceObjectEx descAppearance = cond.AppearanceDescription.Clone() as AppearanceObjectEx;
			if(cond.ApplyToRow) {
				if(RowConditionAppearance == null)
					RowConditionAppearance = appearance;
				else
					RowConditionAppearance.Combine(appearance);
			}
			else {
				if(CellAppearances == null) CellAppearances = new Hashtable();
				if(CellDescriptionAppearances == null) CellDescriptionAppearances = new Hashtable();
				ApplyColumnAppearance(column, appearance, descAppearance);
			}
		}
		void ApplyColumnAppearance(GridColumn column, AppearanceObject appearance, AppearanceObject descAppearance) {
			AppearanceObject current = CellAppearances[column] as AppearanceObject;
			CellAppearances[column] = GetColumnAppearance(current, appearance);
			AppearanceObject currentDesc = CellDescriptionAppearances[column] as AppearanceObject;
			CellDescriptionAppearances[column] = GetColumnAppearance(currentDesc, descAppearance);
		}
		AppearanceObject GetColumnAppearance(AppearanceObject current, AppearanceObject appearance) {
			if(current != null)
				current.Combine(appearance);
			else
				current = appearance;
			return current;
		}
	}
	public abstract class ColumnViewInfo : BaseViewInfo {
		GridFilterPanelInfoArgs filterPanel;
		public ColumnViewInfo(BaseView view) : base(view) {
			this.filterPanel = CreateFilterPanelInfo();
		}
		protected override void CalcViewInfo() {
			base.CalcViewInfo();
			View.FormatRules.TryUpdateStateValues();
		}
		public bool IsSkinned { get { return View != null && View.IsSkinned; } }
		public new ColumnViewAppearances PaintAppearance { get { return base.PaintAppearance as ColumnViewAppearances; } }
		public abstract ObjectPainter FilterPanelPainter { get; }
		public abstract ObjectPainter ViewCaptionPainter { get; }
		protected virtual GridFilterPanelInfoArgs CreateFilterPanelInfo() {
			GridFilterPanelInfoArgs args = new GridFilterPanelInfoArgs();
			return args;
		}
		protected virtual void UpdateFilterPanelInfo() {
			FilterPanel.CustomizeText = DevExpress.XtraGrid.Localization.GridLocalizer.Active.GetLocalizedString(DevExpress.XtraGrid.Localization.GridStringId.FilterPanelCustomizeButton);
			FilterPanel.DisplayText = View.FilterPanelText;
			FilterPanel.AllowMRU = View.IsAllowMRUFilterList;
			FilterPanel.SetAppearance(PaintAppearance.FilterPanel);
			FilterPanel.CloseButtonInfo.SetAppearance(PaintAppearance.FilterCloseButton);
			FilterPanel.ShowCustomizeButton = GridCriteriaHelper.IsAllowFilterEditor(View);
			FilterPanel.ShowCloseButton = View.OptionsView.ShowFilterPanelMode != ShowFilterPanelMode.ShowAlways;
			FilterPanel.RightToLeft = View.IsRightToLeft;
		}
		static int autoHeightCalculateMaxColumnCount = 30;
		public static int AutoHeightCalculateMaxColumnCount {
			get { return autoHeightCalculateMaxColumnCount; }
			set {
				if(value < 10) value = 10;
				autoHeightCalculateMaxColumnCount = value;
			}
		}
		protected virtual int GetFilterPanelHeight() {
			UpdateFilterPanelInfo();
			return CalcObjectSize(FilterPanelPainter, FilterPanel).Height;
		}
		protected virtual void CalcFilterDrawInfo() {
			if(FilterPanel.Bounds.IsEmpty) return;
			UpdateFilterPanelInfo();
			FilterPanel.Graphics = GInfo.AddGraphics(null);
			try {
				FilterPanelPainter.CalcObjectBounds(FilterPanel);
			}
			finally {
				FilterPanel.Graphics = null;
				GInfo.ReleaseGraphics();
			}
		}
		private Rectangle findPanelBounds = Rectangle.Empty;
		protected Rectangle FindPanelBounds { get { return findPanelBounds; } }
		protected void UpdateFindControlVisibility() {
			if(View.FindPanel != null) {
				if(FindPanelBounds.Height < 1) 
					View.FindPanel.Visible = false;
				else
					View.FindPanel.Bounds = FindPanelBounds;
			}
		}
		protected Rectangle UpdateFindControlVisibility(Rectangle client, bool setPosition) {
			if(View.IsFindPanelVisible) {
				View.RequestFindPanel();
			}
			else {
				View.DestroyFindPanel();
			}
			if(View.FindPanel != null) {
				bool prevVisible = View.FindPanel.Visible;
				Rectangle bounds = client;
				bounds.Height = View.FindPanel.Height;
				if(bounds.Height > client.Height / 2) {
					View.FindPanel.Visible = false;
					this.findPanelBounds = Rectangle.Empty;
				}
				else {
					this.findPanelBounds = bounds;
					if(setPosition) View.FindPanel.Bounds = bounds;
					View.FindPanel.Visible = true;
					bounds.Y = bounds.Bottom;
					bounds.Height = (client.Bottom - bounds.Y);
					client = bounds;
				}
				if(prevVisible != View.FindPanel.Visible) {
					if(!prevVisible)
						View.FindPanel.FocusFindEdit();
					else {
						GridControl.Focus();
					}
				}
			}
			return client;
		}
		protected virtual int CalcViewCaptionHeight(Rectangle client) {
			Graphics g = GInfo.AddGraphics(null);
			int h = 10;
			try {
				StyleObjectInfoArgs info = new StyleObjectInfoArgs(GInfo.Cache, client, PaintAppearance.ViewCaption);
				h = ViewCaptionPainter.CalcObjectMinBounds(info).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(h, View.ViewCaptionHeight);
		}
		public abstract void Calc(Graphics g, Rectangle bounds);
		public abstract Rectangle ViewCaptionBounds { get; }
		public GridFilterPanelInfoArgs FilterPanel { get { return filterPanel; } }
		public virtual new ColumnView View { get { return base.View as ColumnView; } }
		public virtual int CalcColumnBestWidth(GridColumn column) {
			return -1;
		}
		public virtual bool UpdateRowConditionAndFormat(int rowHandle, IRowConditionFormatProvider provider) {
			bool res = UpdateRowRules(rowHandle, provider.FormatInfo);
			if(View.FormatRules.Count == 0)
				res |= UpdateRowCondition2(rowHandle, provider.ConditionInfo);
			else provider.ConditionInfo.Obsolete();
			return res;
		}
		public virtual bool UpdateRowCondition2(int rowHandle, ConditionInfo condition) {
			if(rowHandle == DevExpress.Data.CurrencyDataController.NewItemRow) return false;
			if(condition.IsCalculated) return false;
			return CalcConditionsCore2(rowHandle, condition);
		}
		public virtual bool UpdateRowRules(int rowHandle, RowFormatRuleInfo formatInfo) {
			if(rowHandle == DevExpress.Data.CurrencyDataController.NewItemRow) return false;
			if(formatInfo.IsReady) return false;
			return CalcRulesCore(rowHandle, formatInfo);
		}
		protected virtual GridRowCellState CalcRowStateCore(int rowHandle) {
			GridRowCellState state = GridRowCellState.Default;
			if(View.IsFocusedView) state |= GridRowCellState.GridFocused;
			if(View.FocusedRowHandle == rowHandle) state |= GridRowCellState.Focused;
			if(View.IsRowSelected(rowHandle)) state |= GridRowCellState.Selected;
			return state;
		}
		protected virtual bool CalcRulesCore(int rowHandle, RowFormatRuleInfo formatInfo) {
			if(rowHandle == GridControl.NewItemRowHandle) return false;
			formatInfo.Clear();
			if(!View.FormatRules.HasValidRules) {
				formatInfo.SetIsReady();
				return true;
			}
			for(int n = 0; n < View.FormatRules.Count; n++) {
				var format = View.FormatRules[n];
				if(!format.IsValid) continue;
				if(format.GetApplyToColumn() != null && !format.GetApplyToColumn().Visible) {
					if(!format.ApplyToRow) continue;
					formatInfo.Check(format, format.GetApplyToColumn(), rowHandle, View.GetRowCellValue(rowHandle, format.Column));
					continue;
				}
				for(int c = 0; c < View.Columns.Count; c++ ) {
					GridColumn column = View.Columns[c];
					if(!format.ApplyToRow && !column.Visible) continue;
					if(format.GetApplyToColumn() != null && format.GetApplyToColumn() != column) continue;
					formatInfo.Check(format, format.GetApplyToColumn(), rowHandle, View.GetRowCellValue(rowHandle, format.Column));
				}
			}
			formatInfo.PrepareAppearances();
			return true;
		}
		protected virtual bool CalcConditionsCore2(int rowHandle, ConditionInfo condition) {
			condition.Clear();
			int cnt = View.FormatConditions.Count, colCount = View.Columns.Count;
			if(cnt == 0) return false;
			for(int n = 0; n < cnt; n++) {
				StyleFormatCondition cond = View.FormatConditions[n];
				if(!cond.IsValid) continue;
				if(cond.Column != null) {
					if(!cond.Column.Visible && !cond.ApplyToRow) continue;
					condition.CheckCondition(cond, cond.Column, View.GetRowCellValue(rowHandle, cond.Column), View.GetDataSourceRowIndex(rowHandle));
					continue;
				}
				for(int c = 0; c < colCount; c++) {
					GridColumn column = View.Columns[c];
					if(!cond.ApplyToRow && !column.Visible) continue;
					condition.CheckCondition(cond, column, View.GetRowCellValue(rowHandle, column), View.GetDataSourceRowIndex(rowHandle));
					if(cond.ApplyToRow && cond.Condition == FormatConditionEnum.Expression) break;
				}
			}
			condition.IsCalculated = true;
			return true;
		}
		protected internal virtual void UpdateRowIndexes(int newTopRowIndex, GridRowCollection rows) {
			if(rows == null) return;
			for(int n = 0; n < rows.Count; n++) {
				GridRow ri = rows[n];
				if(ri.RowHandle == GridControl.NewItemRowHandle || ri.RowHandle == GridControl.AutoFilterRowHandle) continue;
				ri.VisibleIndex = newTopRowIndex ++;
				ri.RowHandle = View.GetVisibleRowHandle(ri.VisibleIndex);
			}
		}
		protected virtual ShowButtonModeEnum GetDefaultShowButtonMode(int rowHandle, GridColumn column) {
			return ShowButtonModeEnum.ShowForFocusedCell;
		}
		protected internal DetailLevel CalcRowCellDetailLevel(int rowHandle, GridColumn column) {
			return CalcRowCellDetailLevel(rowHandle, column, false);
		}
		protected internal virtual DetailLevel CalcRowCellDetailLevel(int rowHandle, GridColumn column, bool ignoreFocused) {
			ShowButtonModeEnum show = column.ShowButtonMode;
			if(show == ShowButtonModeEnum.Default && rowHandle != GridControl.AutoFilterRowHandle) {
				if(!View.Editable || !column.OptionsColumn.AllowEdit) {
					return DevExpress.XtraEditors.Controls.DetailLevel.Minimum;
				}
			}
			if(show == ShowButtonModeEnum.Default) show = View.OptionsView.ShowButtonMode;
			if(show == ShowButtonModeEnum.Default) show = GetDefaultShowButtonMode(rowHandle, column);
			if(show == ShowButtonModeEnum.ShowAlways ||
				(!ignoreFocused && show == ShowButtonModeEnum.ShowForFocusedRow &&
				rowHandle == View.FocusedRowHandle) ||
				(!ignoreFocused && show == ShowButtonModeEnum.ShowForFocusedCell &&
				rowHandle == View.FocusedRowHandle &&
				column == View.FocusedColumn)) {
				return DevExpress.XtraEditors.Controls.DetailLevel.Full;
			}
			return DetailLevel.Minimum;
		}
		protected internal virtual void UpdateEditViewInfo(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi, GridColumn column, int rowHandle) {
			UpdateEditViewInfo(vi);
			if(column.DisplayFormat != null && !column.DisplayFormat.IsEmpty) {
				vi.Format = column.DisplayFormat;
			}
			vi.DetailLevel = CalcRowCellDetailLevel(rowHandle, column);
			vi.Editable = View.Editable && column.OptionsColumn.AllowEdit;
		}
		protected override BaseEditViewInfo HasItem( CellId id) {
			return null;
		}
		protected override void AddAnimatedItems() {
		}
	}
}
