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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Data;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration.Creators {
	abstract class BandTuner {
		protected XRTable bandTable;
		protected XtraReport report;
		protected Band band;
		protected int startUpRow = 0;
		public BandTuner(Band band){
			this.band = band;
			if(band != null){
				this.report = (XtraReport) band.Report;
				if(band.Controls.Count != 0){
					this.bandTable = (XRTable) band.Controls[0];
				}
			}
		}
		public void Tune(){
			if(band == null || bandTable == null) return;
			SetBandStyle();
			SetBandSortOrder();
			for(int rInd = startUpRow; rInd < bandTable.Controls.Count; rInd++){
				var tableRow = bandTable.Rows[rInd];
				for(int i = 0; i < tableRow.Cells.Count; i++){
					var cell = tableRow.Cells[i];
					SetCellBindings(cell);
					SetCellBorders(cell);
					SetCellAppearance(cell);
				}
			}
		}
		protected virtual void SetBandSortOrder() {
		}
		protected virtual void SetBandStyle(){
		}
		protected virtual void SetCellBindings(XRTableCell cell){
		}
		protected virtual void SetCellAppearance(XRTableCell cell){
		}
		protected virtual void SetCellBorders(XRTableCell cell){
		}
	}
	class HeaderBandTuner<TCol, TRow> : BandTuner where TRow : IRowBase where TCol : IColumn {
		ReportGenerationModel<TCol, TRow> model;
		public HeaderBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band){
			this.model = model;
		}
		protected override void SetBandStyle(){
			this.band.Styles.Style = model.Styles.FirstOrDefault(s => s.Name == "ReportHeaderBandStyle");
		}
		protected override void SetCellBindings(XRTableCell cell){
			int index = cell.Index;
			cell.Text = model.DataColumns[index].Header;
		}
		protected override void SetCellAppearance(XRTableCell cell){
			var column = model.DataColumns[cell.Index];
			if(!StylesExtensions<TCol, TRow>.Equals(column.AppearanceHeader, StylesExtensions<TCol, TRow>.GetDefaultFormat())) {
				GeneratorUtils.SetCellStyle(cell, column.AppearanceHeader);
			}
		}
		protected override void SetCellBorders(XRTableCell cell) {
			if(model.PrintHorizontalLines) cell.Borders |= BorderSide.Bottom | BorderSide.Top;
			if(model.PrintVerticalLines) cell.Borders |= BorderSide.Left;
			if(cell.Index == model.DataColumns.Count - 1 && model.PrintVerticalLines)
				cell.Borders |= BorderSide.Right;
		}
	}
	class GroupHeaderBandTuner<TCol, TRow> : BandTuner where TRow : IRowBase where TCol : IColumn {
		enum HeaderCell {
			GroupHeaderCell,
			GroupHeaderValueCell,
			GroupHeaderSummaryCell
		}
		ReportGenerationModel<TCol, TRow> model;
		TCol column;
		public GroupHeaderBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band){
			this.model = model;
			var groupedCoolumnsCount = model.GroupedColumns.Count;
			column = model.GroupedColumns.FirstOrDefault(x => x.GroupIndex == groupedCoolumnsCount - 1 - band.LevelInternal);
			if(column != null){
				((GroupHeaderBand) this.band).GroupFields.Add(new GroupField(column.FieldName));
				((GroupHeaderBand) this.band).GroupUnion = GroupUnion.WithFirstDetail;
			}
			bandTable.EndInit();
		}
		protected override void SetBandStyle(){
			this.band.Styles.Style = model.Styles.FirstOrDefault(s => s.Name == "ReportGroupHeaderBandStyle");
		}
		public void SetBestFit(XRControl control){
			float actualWidth = report.PrintingSystem.Graph.MeasureString(control.Text).Width;
			report.PrintingSystem.Graph.Font = control.GetEffectiveFont();
			SizeF size = report.PrintingSystem.Graph.MeasureString(control.Text);
			switch(report.ReportUnit){
				case ReportUnit.HundredthsOfAnInch:
					actualWidth = GraphicsUnitConverter.Convert(size.Width, GraphicsDpi.Pixel, GraphicsDpi.HundredthsOfAnInch);
					break;
				case ReportUnit.TenthsOfAMillimeter:
					actualWidth = GraphicsUnitConverter.Convert(size.Width, GraphicsDpi.Pixel, GraphicsDpi.TenthsOfAMillimeter);
					break;
				default:
					break;
			}
			control.WidthF = actualWidth + Converter.PaddingLeft;
		}
		HeaderCell fl = HeaderCell.GroupHeaderCell;
		int itemIndex;
		protected override void SetCellBindings(XRTableCell cell){
			if (column != null){
				switch (fl){
					case HeaderCell.GroupHeaderCell:
						cell.Text = column.Header + ":";
						SetBestFit(cell);
						fl = HeaderCell.GroupHeaderValueCell;
						break;
					case HeaderCell.GroupHeaderValueCell:
						string formatString = GeneratorUtils.NormalizeFormatString(column.FormatSettings.FormatString);
						SetCellDataMethods<TCol>.AddCellData(report, cell, column, formatString);
						fl = HeaderCell.GroupHeaderSummaryCell;
						break;
					case HeaderCell.GroupHeaderSummaryCell:
						var item = model.GroupHeaderSummary[itemIndex];
						if(item != null){
							if(string.IsNullOrEmpty(item.DisplayFormat))
								item.DisplayFormat = SummaryMethods<TCol>.GetDisplayFormatByFunction(Converter.GetXRSummaryFunc(item.SummaryType).ToString());
							var sourceColumn = model.DataColumns.FirstOrDefault(c => c.FieldName == item.FieldName);
							if(sourceColumn == null) sourceColumn = model.GroupedColumns.FirstOrDefault(x => x.FieldName == item.FieldName);
							if (sourceColumn != null) {
								SummaryMethods<TCol>.SetSummaryDataBindings(report, cell, sourceColumn, column, string.Empty);
								cell.Summary = SummaryMethods<TCol>.GetSummary(item, SummaryRunning.Group, sourceColumn.FormatSettings.FormatString);
							}
							itemIndex++;
						}
						break;
				}
			}
		}
		protected override void SetCellAppearance(XRTableCell cell) {
			if (!StylesExtensions<TCol,TRow>.Equals(model.AppearanceGroupRow, StylesExtensions<TCol, TRow>.GetDefaultFormat())){
				GeneratorUtils.SetCellStyle(cell, model.AppearanceGroupRow);
			}
		}
		protected override void SetCellBorders(XRTableCell cell){
			if(model.PrintHorizontalLines) cell.Borders |= BorderSide.Bottom;
			if(model.PrintVerticalLines && cell.Index == 0) cell.Borders |= BorderSide.Left;
			if(model.PrintVerticalLines && cell.Index == model.GroupHeaderCellsCount - 1)
				cell.Borders |= BorderSide.Right;
		}
		protected override void SetBandSortOrder() {
			var sortFields = report.Bands[BandKind.GroupHeader].Band.SortFieldsInternal;
			if(sortFields.Count > 0) {
				var sortField = sortFields.FirstOrDefault(x => x.FieldName == GeneratorUtils.GetDataMember(report, column.FieldName));
				if(sortField != null) {
					sortField.SortOrder = (XRColumnSortOrder) (int) column.SortOrder;
				}
			}
		}
	}
	class DetailBandTuner<TCol, TRow> : BandTuner where TRow : IRowBase where TCol : IColumn {
		ReportGenerationModel<TCol, TRow> model;
		public DetailBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band){
			this.model = model;
		}
		protected override void SetBandStyle(){
			this.band.Styles.Style = model.Styles.FirstOrDefault(s => s.Name == "ReportDetailBandStyle");
			this.band.Styles.EvenStyle = model.Styles.FirstOrDefault(s => s.Name == "ReportEvenStyle");
			this.band.Styles.OddStyle = model.Styles.FirstOrDefault(s => s.Name == "ReportOddStyle");
		}
		protected override void SetCellBindings(XRTableCell cell){
			var column = model.DataColumns[cell.Index];
			string formatString = GeneratorUtils.NormalizeFormatString(column.FormatSettings.FormatString);
			SetCellDataMethods<TCol>.AddCellData(report,cell, column, formatString);
			AssignFormatRule(column, cell);
		}
		protected override void SetBandSortOrder() {
			var columns = model.DataColumns.Where(c => c.SortOrder != ColumnSortOrder.None);
			foreach(var col in columns) {
				this.band.SortFieldsInternal.Add(new GroupField(GeneratorUtils.GetDataMember(report, col.FieldName), (XRColumnSortOrder) (int) col.SortOrder));
			}
		}
		void AssignFormatRule(TCol col,XRTableCell cell){
			List<FormattingRule> rules;
			if(model.FormattingRules.TryGetValue(col.FieldName, out rules)){
				cell.FormattingRules.AddRange(rules);
			}
			if(model.FormattingRules.TryGetValue(string.Empty, out rules)){
				cell.FormattingRules.AddRange(rules);
			}
		}
		protected override void SetCellAppearance(XRTableCell cell){
			var column = model.DataColumns[cell.Index];
			if(!StylesExtensions<TCol, TRow>.Equals(column.Appearance, StylesExtensions<TCol, TRow>.GetDefaultFormat())) {
				GeneratorUtils.SetCellStyle(cell, column.Appearance);
			} else {
				GeneratorUtils.CorrectCellTextAlingment(cell, column);
			}
		}
		protected override void SetCellBorders(XRTableCell cell) {
			if(model.PrintVerticalLines) cell.Borders |= BorderSide.Left;
			if(model.PrintHorizontalLines) cell.Borders |= BorderSide.Bottom;
			if(cell.Index == model.DataColumnsCount - 1 && model.PrintVerticalLines)
				cell.Borders |= BorderSide.Right;
		}
	}
	abstract class FooterBandTuner<TCol, TRow> : BandTuner where TRow : IRowBase where TCol : IColumn {
		protected ReportGenerationModel<TCol, TRow> model;
		protected SummaryRunning running;
		protected List<ISummaryItemEx> summarySet;
		public FooterBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band){
			this.model = model;
			Init();
		}
		protected abstract void Init();
		protected override void SetCellBindings(XRTableCell cell){
			var column = model.DataColumns[cell.Index];
			var summaryItem = summarySet.FirstOrDefault(x => x.ShowInColumnFooterName == column.FieldName);
			if(summaryItem == null) return;
			if(summaryItem.SummaryType != SummaryItemType.Custom){
				var source = model.DataColumns.FirstOrDefault(x => x.FieldName == summaryItem.FieldName);
				if(source == null) source = model.GroupedColumns.FirstOrDefault(x => x.FieldName == summaryItem.FieldName);
				if(source != null){
					string dataMember;
					model.CalculatedFields.TryGetValue(source, out dataMember);
					SummaryMethods<TCol>.SetSummaryDataBindings(report, cell, source, column, GeneratorUtils.GetDataMember(report, source.FieldName));
					cell.Summary = SummaryMethods<TCol>.GetSummary(summaryItem, running, source.FormatSettings.FormatString);
				}
			}
			summarySet.Remove(summaryItem);
		}
		protected override void SetCellBorders(XRTableCell cell) {
			if(model.PrintVerticalLines) cell.Borders |= BorderSide.Left;
			if(model.PrintHorizontalLines) cell.Borders |= BorderSide.Bottom;
			if(model.PrintVerticalLines && cell.Index == model.DataColumnsCount - 1)
				cell.Borders |= BorderSide.Right;
		}
		protected override void SetCellAppearance(XRTableCell cell) {
			cell.Padding = Converter.CalcPaddingInfo(this.report.ReportUnit, false);
			var targetDetailTable = report.Bands[BandKind.Detail].Controls[0];
			var targetDetailTableRow = targetDetailTable.Controls[0];
			var targetDetailTableCell = targetDetailTableRow.Controls[cell.Index];
			cell.TextAlignment = targetDetailTableCell.TextAlignment;
		}
	}
	class ReportFooterBandTuner<TCol, TRow> : FooterBandTuner<TCol,TRow> where TRow : IRowBase where TCol : IColumn {
		public ReportFooterBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band,model){
		}
		protected override void Init(){
			running = SummaryRunning.Report;
			summarySet = model.TotalSummary;
		}
		protected override void SetBandStyle(){
			this.band.Styles.Style = model.Styles.FirstOrDefault(s => s.Name == "ReportFooterBandStyle");
		}
	}
	class GroupFooterBandTuner<TCol, TRow> : FooterBandTuner<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		public GroupFooterBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band,model){
		}
		protected override void Init(){
			running = SummaryRunning.Group;
			summarySet = model.GroupSummary;
		}
		protected override void SetBandStyle(){
			this.band.Styles.Style = model.Styles.FirstOrDefault(s => s.Name == "ReportGroupFooterBandStyle");
		}
	}
	class FixedTotalFooterBandTuner<TCol, TRow>: FooterBandTuner<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		public FixedTotalFooterBandTuner(Band band, ReportGenerationModel<TCol, TRow> model) : base(band, model){
			startUpRow = model.TotalFooterRows;
		}
		protected override void Init(){
			running = SummaryRunning.Report;
			summarySet = model.FixedTotalSummary;
		}
		protected override void SetCellBorders(XRTableCell cell){
			if(model.PrintHorizontalLines) cell.Borders = BorderSide.Bottom;
			if(model.PrintVerticalLines && cell.Index == 0)
				cell.Borders |= BorderSide.Left;
			if(model.PrintVerticalLines && cell.Index == model.DataColumnsCount - 1)
				cell.Borders |= BorderSide.Right;
		}
	}
	class AdditionalSettingsProvider<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		ReportGenerationModel<TCol, TRow> model;
		XtraReport report;
		public AdditionalSettingsProvider(XtraReport report, ReportGenerationModel<TCol, TRow> model){
			this.model = model;
			this.report = report;
		}
		public void Execute(){
			AddFormatRules();
			AddStyles();
		}
		void AddFormatRules(){
			foreach(var item in model.FormatRules){
				FormattingRule rule = null;
				if(item.Rule is IFormatConditionRuleExpression){
					rule = GetFormattingRule((IFormatConditionRuleExpression) item.Rule);
				}
				if(item.Rule is IFormatConditionRuleValue){
					rule = GetFormattingRule((IFormatConditionRuleValue) item.Rule);
				}
				this.report.FormattingRuleSheet.Add(rule);
				var positionCol = item.ColumnApplyTo ?? item.Column;
				var key = item.ApplyToRow ? string.Empty : positionCol.FieldName;
				if(model.FormattingRules.ContainsKey(key)){
					model.FormattingRules[key].Add(rule);
				} else{
					var values = new List<FormattingRule>();
					values.Add(rule);
					model.FormattingRules.Add(key, values);
				}
			}
		}
		FormattingRule GetFormattingRule(IFormatConditionRuleExpression itemRule){
			FormattingRule rule = new FormattingRule();
			rule.Condition = itemRule.Expression;
			GetDataSource(rule);
			GetFormat(rule, itemRule.Appearance);
			return rule;
		}
		FormattingRule GetFormattingRule(IFormatConditionRuleValue itemRule){
			FormattingRule rule = new FormattingRule();
			rule.Condition = itemRule.Expression;
			GetDataSource(rule);
			GetFormat(rule, itemRule.Appearance);
			return rule;
		}
		void GetDataSource(FormattingRule rule){
			rule.DataMember = report.DataMember;
			rule.DataSource = report.DataSource;
		}
		public void GetFormat(FormattingRule rule, XlDifferentialFormatting appearance){
			if(appearance.Fill != null)
				rule.Formatting.BackColor = appearance.Fill.BackColor;
			if(appearance.Border != null){
				rule.Formatting.BorderColor = appearance.Border.BottomColor;
				rule.Formatting.BorderDashStyle = Converter.ConvertBorderStyle(appearance.Border.BottomLineStyle);
			}
			var font = appearance.Font;
			if(font != null){
				rule.Formatting.Font = new Font(font.Name, (float) font.Size, Converter.GetFontStyle(font));
				rule.Formatting.ForeColor = appearance.Font.Color;
			}
		}
		void AddStyles(){
			foreach(var style in model.Styles){
				this.report.StyleSheet.Add(style);
			}
		}
	}
	static class SummaryMethods<TCol> where TCol : IColumn {
		public static void SetSummaryDataBindings(XtraReport report, XRTableCell cell, TCol sourceColumn, TCol column,string dataMember){
			if (sourceColumn == null) sourceColumn = column;
			if(string.IsNullOrEmpty(dataMember))
				dataMember = GeneratorUtils.GetDataMember(report, sourceColumn.FieldName);
			cell.DataBindings.Add("Text", null, dataMember);
		}
		public static XRSummary GetSummary(ISummaryItemEx item, SummaryRunning running, string columnFormatString){
			var summaryType = Converter.GetXRSummaryFunc(item.SummaryType);
			var value = new XRSummary();
			value.Func = summaryType;
			if(string.IsNullOrEmpty(item.DisplayFormat)) {
				if(summaryType != SummaryFunc.Count)
					value.FormatString = GeneratorUtils.NormalizeFormatString(columnFormatString);
			} else value.FormatString = item.DisplayFormat;
			value.Running = running;
			value.IgnoreNullValues = true;
			return value;
		}
		public static void SetCustomSummary(XRTableCell cell, ISummaryItemEx item){
			if(item.SummaryValue != null){
				var summaryValue = item.SummaryValue.ToString();
				cell.Text = summaryValue;
			}
		}
		public static string GetDisplayFormatByFunction(string func){
			return "(" + func + "= {0}" + ")";
		}
	}
	static class SetCellDataMethods<TCol> where TCol : IColumn {
		public static void AddCellData(XtraReport report, XRTableCell cell, TCol column, string formatString) {
			if(column.UnboundInfo != null && !string.IsNullOrEmpty(column.UnboundInfo.UnboundExpression)) {
				var calculatedField = new CalculatedField();
				report.CalculatedFields.Add(calculatedField);
				calculatedField.DataSource = report.DataSource;
				calculatedField.DataMember = report.DataMember;
				calculatedField.Name = GeneratorUtils.GetDataMember(report, column.FieldName); 
				calculatedField.FieldType = Converter.GetUnboundFieldType(column.UnboundInfo.UnboundType);
				calculatedField.Expression = column.UnboundInfo.UnboundExpression;
				cell.DataBindings.Add("Text", null, calculatedField.Name, formatString); 
			} else {
				cell.DataBindings.Add("Text", null, GeneratorUtils.GetDataMember(report, column.FieldName), formatString);
			}
		}
	}
}
