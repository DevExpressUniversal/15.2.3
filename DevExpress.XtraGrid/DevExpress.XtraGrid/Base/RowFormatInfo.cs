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
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraGrid.Views.Base.ViewInfo {
	public interface IRowConditionFormatProvider {
		ConditionInfo ConditionInfo { get; }
		RowFormatRuleInfo FormatInfo { get; }
	}
	public class RowFormatRuleInfo {
		public class CellFormatInfo {
			public GridColumn Column { get; set; }
			public GridFormatRule Format { get; set; }
			public object Value { get; set; }
		}
		public class RowFormatInfo {
			public GridFormatRule Format { get; set; }
			public object Value { get; set; }
		}
		Dictionary<GridColumn, AppearanceObjectEx> cellAppearances;
		Dictionary<GridColumn, bool> cellRulesFinalized;
		List<RowFormatInfo> rowFormat;
		List<CellFormatInfo> cellFormat;
		List<CellFormatInfo> drawFormat;
		List<CellFormatInfo> contextImageFormat;
		bool rowRulesFinalized;
		bool prepared = false;
		ColumnView view;
		public RowFormatRuleInfo(ColumnView view) {
			this.view = view;
		}
		public ColumnView View { get { return view; } }
		public void Clear() {
			this.prepared = false;
			this.rowRulesFinalized = false;
			if(rowFormat != null) rowFormat.Clear();
			if(cellFormat != null) cellFormat.Clear();
			if(contextImageFormat != null) contextImageFormat.Clear();
			if(drawFormat != null) drawFormat.Clear();
			if(cellAppearances != null) cellAppearances.Clear();
			if(cellRulesFinalized != null) cellRulesFinalized.Clear();
			RowAppearance = null;
		}
		public AppearanceObjectEx RowAppearance {
			get;
			set;
		}
		protected internal void SetIsReady() {
			this.prepared = true;
		}
		public void PrepareAppearances() {
			if(IsReady) return;
			PrepareRowAppearance();
			PrepareCellAppearance();
			this.prepared = true;
		}
		public bool IsReady { get { return prepared; } }
		void PrepareCellAppearance() {
			if(cellFormat == null || cellFormat.Count == 0) return;
			foreach(var cr in cellFormat) {
				CheckDrawRule(cr);
				var appearanceRule = cr.Format.Rule as IFormatRuleAppearance;
				if(appearanceRule == null) continue;
				if(cellAppearances == null) cellAppearances = new Dictionary<GridColumn, AppearanceObjectEx>();
				AppearanceObjectEx current;
				var appearance = appearanceRule.QueryAppearance(new FormatRuleAppearanceArgs(cr.Format.ValueProvider, cr.Value));
				if(cellAppearances.TryGetValue(cr.Column, out current)) {
					current.Combine(appearance);
				}
				else {
					cellAppearances[cr.Column] = appearance.Clone() as AppearanceObjectEx;
				}
			}
		}
		void PrepareRowAppearance() {
			if(rowFormat == null || rowFormat.Count == 0) return;
			foreach(var row in rowFormat) {
				var appearanceRule = row.Format.Rule as IFormatRuleAppearance;
				if(appearanceRule != null) {
					var appearance = appearanceRule.QueryAppearance(new FormatRuleAppearanceArgs(row.Format.ValueProvider, row.Value));
					if(RowAppearance == null) RowAppearance = appearance.Clone() as AppearanceObjectEx;
					else
						RowAppearance.Combine(appearance);
				}
				if(row is IFormatRuleDraw) CheckDrawRule(new CellFormatInfo() { Column = null, Format = row.Format });
			}
		}
		void CheckDrawRule(CellFormatInfo cell) {
			var drawRule = cell.Format.Rule as IFormatRuleDraw;
			if(drawRule != null) {
				if(drawFormat == null) drawFormat = new List<CellFormatInfo>();
				drawFormat.Add(cell);
			}
			var contextImageRule = cell.Format.Rule as IFormatRuleContextImage;
			if(contextImageRule != null) {
				if(contextImageFormat == null) contextImageFormat = new List<CellFormatInfo>();
				contextImageFormat.Add(cell);
			}
		}
		public virtual bool HasDrawFormat { get { return drawFormat != null && drawFormat.Count > 0; } }
		public virtual bool HasContextImageFormat { get { return contextImageFormat != null && contextImageFormat.Count > 0; } }
		public virtual bool ApplyContextImage(GraphicsCache cache, GridColumn column, Rectangle bounds, int rowHandle, BaseEditViewInfo viewInfo) {
			IFormatRuleSupportContextImage supportImage = viewInfo as IFormatRuleSupportContextImage;
			if(supportImage == null) return false;
			if(View.FormatRules.Count > 0 || View.FormatRules.Changed) supportImage.SetContextImage(null);
			if(!HasContextImageFormat) return false;
			foreach(var cf in contextImageFormat) {
				if(cf.Column != column) continue;
				var contextImage = cf.Format.Rule as IFormatRuleContextImage;
				cf.Format.ValueProvider.SetData(View.GetRowCellValue(rowHandle, cf.Format.Column), View.GetDataSourceRowIndex(rowHandle));
				var image = contextImage.GetContextImage(new FormatRuleDrawArgs(cache, bounds, cf.Format.ValueProvider));
				if(image != null) supportImage.SetContextImage(image);
			}
			return true;
		}
		public bool ApplyDrawFormat(GraphicsCache cache, GridColumn column, Rectangle bounds, int rowHandle, BaseEditViewInfo viewInfo) {
			return ApplyDrawFormat(cache, column, bounds, rowHandle, viewInfo, null, null);
		}
		public virtual bool ApplyDrawFormat(GraphicsCache cache, GridColumn column, Rectangle bounds, int rowHandle, BaseEditViewInfo viewInfo, DrawAppearanceMethod originalContentPainter, AppearanceObject originalContentAppearance) {
			if(!HasDrawFormat) return false;
			bool disableDrawCellContent = false;
			foreach(var cf in drawFormat) {
				if(cf.Column != column) continue;
				var drawRule = cf.Format.Rule as IFormatRuleDraw;
				if(cf.Format.Rule is IFormatRuleContextImage) {
					if(FormatRuleDrawArgs.IsSupportContextImage(viewInfo)) continue;
				}
				cf.Format.ValueProvider.SetData(View.GetRowCellValue(rowHandle, cf.Format.Column), View.GetDataSourceRowIndex(rowHandle));
				drawRule.DrawOverlay(new FormatRuleDrawArgs(cache, bounds, cf.Format.ValueProvider) { OriginalContentAppearance = originalContentAppearance, OriginalContentPainter = originalContentPainter });
				disableDrawCellContent |= !drawRule.AllowDrawValue;
			}
			return disableDrawCellContent;
		}
		public virtual AppearanceObjectEx GetCellAppearance(GridColumn column) {
			if(cellAppearances == null || cellAppearances.Count == 0) return null;
			AppearanceObjectEx res;
			if(!cellAppearances.TryGetValue(column, out res)) return null;
			return res;
		}
		public virtual void Check(GridFormatRule format, GridColumn column, int rowHandle, object value) {
			if(!format.IsValid || format.ValueProvider == null) return;
			if(rowRulesFinalized && format.ApplyToRow) return;
			if(!format.ApplyToRow && IsCellRulesFinalized(column)) return;
			ApplyProviderValues(format.ValueProvider, column, rowHandle, value);
			if(!format.IsFit()) return;
			if(format.ApplyToRow)
				ApplyToRow(format, value);
			else
				ApplyToCell(format, column, value);
		}
		bool IsCellRulesFinalized(GridColumn column) {
			if(cellRulesFinalized == null || cellRulesFinalized.Count == 0) return false;
			return cellRulesFinalized.ContainsKey(column);
		}
		void ApplyToCell(GridFormatRule format, GridColumn column, object value) {
			if(cellFormat == null) cellFormat = new List<CellFormatInfo>();
			cellFormat.Add(new CellFormatInfo() { Format = format, Column = column, Value = value });
			if(format.StopIfTrue) {
				if(cellRulesFinalized == null) cellRulesFinalized = new Dictionary<GridColumn, bool>();
				cellRulesFinalized[column] = true;
			}
		}
		void ApplyToRow(GridFormatRule format, object value) {
			if(rowRulesFinalized) return;
			if(rowFormat == null) rowFormat = new List<RowFormatInfo>();
			this.rowFormat.Add(new RowFormatInfo() { Format = format, Value = value });
			if(format.StopIfTrue) rowRulesFinalized = true;
		}
		void ApplyProviderValues(Helpers.GridFormatRuleValueProvider valueProvider, GridColumn column, int rowHandle, object cellValue) {
			valueProvider.SetData(cellValue, View.ViewRowHandleToDataSourceIndex(rowHandle));
		}
		internal void SetView(ColumnView view) {
			this.view = view;
		}
	}
}
