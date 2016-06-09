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
using System.Linq;
using System.Text;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraReports.ReportGeneration.Creators;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration {
	abstract class XRBuilderBase {
		public abstract void CustomizeHeaderBand();
		public abstract void CustomizeGroupHeaderBand();
		public abstract void CustomizeDetailBand();
		public abstract void CustomizeGroupFooterBand();
		public abstract void CustomizeReportFooterBand();
		public abstract void AddReportAdditionalSettings();
	}
	class XRBuilderDirector {
		XRBuilderBase builder;
		public XRBuilderDirector(XRBuilderBase builder) {
			this.builder = builder;
		}
		public void Costruct() {
			builder.AddReportAdditionalSettings();
			builder.CustomizeHeaderBand();
			builder.CustomizeGroupHeaderBand();
			builder.CustomizeDetailBand();
			builder.CustomizeGroupFooterBand();
			builder.CustomizeReportFooterBand();
		}
	}
	class XRBuilder<TCol, TRow> : XRBuilderBase where TRow : IRowBase where TCol : IColumn {
		XtraReport report;
		ReportGenerationModel<TCol, TRow> model;
		public XRBuilder(XtraReport report, ReportGenerationModel<TCol, TRow> model) {
			this.report = report;
			this.model = model;
		}
		public override void CustomizeHeaderBand() {
			var band = report.Bands[BandKind.PageHeader];
			var headerBandTuner = new HeaderBandTuner<TCol, TRow>(band, model);
			headerBandTuner.Tune();
		}
		public override void CustomizeGroupHeaderBand() {
			if(!model.ShowGroupHeader || model.DataColumnsCount == 0) return;
			for(int i = 0; i < model.GroupedColumns.Count; i++) {
				var band = report.Bands.FirstOrDefault(x => x.LevelInternal == i);
				var headerBandTuner = new GroupHeaderBandTuner<TCol, TRow>(band, model);
				headerBandTuner.Tune();
			}
		}
		public override void CustomizeDetailBand() {
			var band = report.Bands[BandKind.Detail];
			var headerBandTuner = new DetailBandTuner<TCol, TRow>(band, model);
			headerBandTuner.Tune();
		}
		public override void CustomizeGroupFooterBand() {
			if(!model.ShowGroupFooter || model.DataColumnsCount == 0) return;
			for(int i = 0; i < model.GroupedColumns.Count; i++) {
				var band = report.Bands.FirstOrDefault(x => x.LevelInternal == i && (x as GroupFooterBand) != null);
				var footerBandTuner = new GroupFooterBandTuner<TCol, TRow>(band, model);
				footerBandTuner.Tune();
			}
		}
		public override void CustomizeReportFooterBand() {
			var band = report.Bands[BandKind.ReportFooter];
			if(band == null) return;
			if(model.ShowReportFooter) {
				var footerBandTuner = new ReportFooterBandTuner<TCol, TRow>(band, model);
				footerBandTuner.Tune();
			}
			if(!model.ShowFixedTotalFooter) return;
			var fixedTotalBandTuner = new FixedTotalFooterBandTuner<TCol, TRow>(band, model);
			fixedTotalBandTuner.Tune();
		}
		public override void AddReportAdditionalSettings() {
			var additionalSettingsProvider = new AdditionalSettingsProvider<TCol, TRow>(report, model);
			additionalSettingsProvider.Execute();
		}
	}
	class ReportGenerationModel<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		IGridView<TCol, TRow> view;
		ReportGenerationOptions options;
		public ReportGenerationModel(IGridView<TCol, TRow> view, ReportGenerationOptions options) {
			this.view = view;
			this.options = options ?? new ReportGenerationOptions();
		}
		bool GetBoolValue(DefaultBoolean value, bool defaultValue) {
			if(value == DefaultBoolean.True) return true;
			if(value == DefaultBoolean.Default) return defaultValue;
			return false;
		}
		static Func<ISummaryItemEx, bool> SearchPredicate() {
			return x => !string.IsNullOrEmpty(x.ShowInColumnFooterName);
		}
		static Func<ISummaryItemEx, bool> SearchPredicateNot() {
			return x => string.IsNullOrEmpty(x.ShowInColumnFooterName);
		}
		Func<ISummaryItemEx, bool> ItemSearchPredicate(IColumn column) {
			return x => x.ShowInColumnFooterName == column.FieldName;
		}
		Dictionary<TCol, string> calculatedFieldsSet;
		public Dictionary<TCol, string> CalculatedFields {
			get {
				if(calculatedFieldsSet == null)
					calculatedFieldsSet = CreateSet();
				return calculatedFieldsSet;
			}
		}
		Dictionary<TCol, string> CreateSet() {
			var result = new Dictionary<TCol, string>();
			var unboundColumns = DataColumns.Where(x => !string.IsNullOrEmpty(x.UnboundInfo.UnboundExpression));
			int calcFieldIndex = 1;
			foreach(var column in unboundColumns) {
				if(!result.ContainsKey(column)) {
					result.Add(column, "calculatedField" + calcFieldIndex);
				}
				calcFieldIndex++;
			}
			return result;
		}
		public XlCellFormatting AppearanceGroupRow {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null)
					return view.AppearancePrint.AppearanceGroupRow;
				return view.AppearanceGroupRow;
			}
		}
		public XlCellFormatting AppearanceEvenRow {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null)
					return view.AppearancePrint.AppearanceEvenRow;
				return view.AppearanceEvenRow;
			}
		}
		public XlCellFormatting AppearanceOddRow {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null)
					return view.AppearancePrint.AppearanceOddRow;
				return view.AppearanceOddRow;
			}
		}
		public XlCellFormatting AppearanceGroupFooter {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null)
					return view.AppearancePrint.AppearanceGroupFooter;
				return view.AppearanceGroupFooter;
			}
		}
		public XlCellFormatting AppearanceFooter {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null) {
					return view.AppearancePrint.AppearanceFooterPanel;
				}
				return view.AppearanceFooter;
			}
		}
		public XlCellFormatting AppearanceRow {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null) {
					return view.AppearancePrint.AppearanceRow;
				}
				return view.AppearanceRow;
			}
		}
		public XlCellFormatting AppearanceHeader {
			get {
				if(this.UsePrintStyles && view.AppearancePrint != null) {
					return view.AppearancePrint.AppearanceHeader;
				}
				return view.AppearanceHeader;
			}
		}
		List<TCol> columns;
		public List<TCol> DataColumns {
			get {
				if(columns == null) {
					if(view.ShowGroupedColumns) columns = view.GetAllColumns().ToList();
					else columns = view.GetAllColumns().Where(x => x.GroupIndex == -1).ToList();
				}
				return columns;
			}
		}
		List<TCol> groupedColumns;
		public List<TCol> GroupedColumns {
			get {
				if(groupedColumns == null) {
					groupedColumns = view.GetGroupedColumns().ToList();
				}
				return groupedColumns;
			}
		}
		Dictionary<string, List<FormattingRule>> formattingRules;
		public Dictionary<string, List<FormattingRule>> FormattingRules {
			get {
				if(formattingRules == null) formattingRules = Create();
				return formattingRules;
			}
		}
		Dictionary<string, List<FormattingRule>> Create() {
			return new Dictionary<string, List<FormattingRule>>();
		}
		IEnumerable<XRControlStyle> styles;
		public IEnumerable<XRControlStyle> Styles {
			get {
				if(styles == null) {
					var se = new StylesExtensions<TCol, TRow>(this);
					styles = se.GetStyles();
				}
				return styles;
			}
		}
		public IEnumerable<IFormatRuleBase> FormatRules {
			get {
				return view.FormatRulesCollection.Where(x => x.Rule is IFormatConditionRuleExpression || x.Rule is IFormatConditionRuleValue);
			}
		}
		public List<ISummaryItemEx> GroupHeaderSummary { get { return view.GroupHeaderSummaryItems.ToList(); } }
		public List<ISummaryItemEx> FixedTotalSummary { get { return view.FixedSummaryItems.ToList(); } }
		public List<ISummaryItemEx> GroupSummary { get { return view.GridGroupSummaryItemCollection.Where(SearchPredicate()).ToList(); } }
		public List<ISummaryItemEx> TotalSummary {
			get {
				var items = view.GridTotalSummaryItemCollection.Where(SearchPredicate()).Reverse().ToList();
				for(int i = 0; i < FixedTotalSummary.Count; i++)
					items.Remove(items.First());
				items.Reverse();
				return items.ToList();
			}
		}
		public int GroupHeaderCellsCount { get { return GroupHeaderSummary.Count + 2; } }
		public int TotalFooterRows { get { return CalcFooterRows(this.TotalSummary); } }
		public int FixedTotalFooterRows { get { return CalcFooterRows(this.FixedTotalSummary); } }
		public int GroupFooterRows { get { return CalcFooterRows(this.GroupSummary); } }
		int CalcFooterRows(IEnumerable<ISummaryItemEx> items) {
			SortedSet<int> orderStats = new SortedSet<int>();
			for(int i = 0; i < DataColumnsCount; i++) {
				int orderStat = items != null ? items.Count(ItemSearchPredicate(columns[i])) : 0;
				orderStats.Add(orderStat);
			}
			return orderStats.Count > 0 ? orderStats.Last() : 0;
		}
		public int DataColumnsCount { get { return DataColumns.Count; } }
		#region options
		public bool ShowReportHeader { get { return GetBoolValue(options.PrintColumnHeaders, true); } }
		public bool ShowReportFooter { get { return view.ShowFooter && GetBoolValue(options.PrintTotalSummaryFooter, true); } }
		public bool ShowGroupHeader { get { return GetBoolValue(options.PrintGroupRows, true); } }
		public bool ShowFixedTotalFooter { get { return view.ShowFooter && FixedTotalSummary.Count > 0; } }
		public bool ShowGroupFooter { get { return view.ShowGroupFooter && GetBoolValue(options.PrintGroupSummaryFooter, true) && GroupSummary.Any(); } }
		public bool EnableOddAppearance { get { return GetBoolValue(options.EnablePrintAppearanceOddRow, true); } }
		public bool EnableEvenAppearance { get { return GetBoolValue(options.EnablePrintAppearanceEvenRow, true); } }
		public bool UsePrintStyles { get { return GetBoolValue(options.UsePrintAppearances, false); } }
		public bool PrintVerticalLines { get { return GetBoolValue(options.PrintVerticalLines, true); } }
		public bool PrintHorizontalLines { get { return GetBoolValue(options.PrintHorizontalLines, true); } }
		#endregion
	}
}
