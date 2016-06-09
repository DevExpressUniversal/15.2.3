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
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraReports.ReportGeneration.Creators;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration {
	interface ILayoutContextStrategy {
		XtraReport Report { get; }
		void ConstructReportHeader();
		void ConstructReportGroupHeader();
		void ConstructReportDetail();
		void ConstructReportGroupFooter();
		void ConstructReportPageFooter();
	}
	class GridViewLayoutStrategy<TCol, TRow> : ILayoutContextStrategy where TRow : IRowBase where TCol : IColumn {
		protected ReportGenerationModel<TCol, TRow> model;
		protected XtraReport report;
		protected const int GroupDivider = 26;
		protected int GroupingStep;
		protected int x;
		protected int groupOffset;
		public GridViewLayoutStrategy(XtraReport report, ReportGenerationModel<TCol, TRow> model) {
			this.model = model;
			this.report = report;
			this.GroupingStep = (report.PageWidth - (report.Margins.Left + report.Margins.Right))/GroupDivider;
			if(model.ShowGroupHeader)
				this.groupOffset = GroupingStep*model.GroupedColumns.Count;
		}
		public XtraReport Report { get { return report; } }
		public void ConstructReportHeader() {
			if(!model.ShowReportHeader) return;
			var creator = new TableCreator(report, new PageHeaderBand(), 0, groupOffset);
			creator.InitTable(1, model.DataColumnsCount);
			creator.EndInit();
		}
		public void ConstructReportGroupHeader() {
			if(!model.ShowGroupHeader || model.DataColumnsCount == 0) return;
			this.x = model.GroupedColumns.Count*GroupingStep;
			int local = Math.Abs(model.GroupedColumns.Count - 1)*GroupingStep;
			for(int i = 0; i < model.GroupedColumns.Count; i++) {
				var band = new GroupHeaderBand();
				var creator = new TableCreator(report, band, local,0);
				creator.InitTable(1, model.GroupHeaderCellsCount);
				local -= GroupingStep;
			}
		}
		public void ConstructReportDetail() {
			var creator = new TableCreator(report, new DetailBand(), x, groupOffset);
			creator.InitTable(1, model.DataColumnsCount);
			creator.EndInit();
		}
		public void ConstructReportGroupFooter() {
			if(!model.ShowGroupFooter || model.DataColumnsCount == 0) return;
			int local = Math.Abs(model.GroupedColumns.Count - 1)*GroupingStep;
			for (int i = 0; i < model.GroupedColumns.Count; i++) {
				var creator = new TableCreator(report, new GroupFooterBand(), local, groupOffset);
				creator.InitTable(1, model.DataColumnsCount);
				creator.EndInit();
				local -= GroupingStep;
			}
		}
		public void ConstructReportPageFooter() {
			if(!model.ShowReportFooter) return;
			var creator = new TableCreator(report, new ReportFooterBand(), 0, groupOffset);
			creator.InitTable(model.TotalFooterRows + model.FixedTotalFooterRows, model.DataColumnsCount);
			creator.EndInit();
		}
	}
	class BandedViewLayoutStrategy<TCol, TRow> : GridViewLayoutStrategy<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		public BandedViewLayoutStrategy(XtraReport report, ReportGenerationModel<TCol, TRow> model) : base(report, model) {
		}
	}
	class ReportLayoutContext<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		protected XtraReport report;
		protected IGridView<TCol, TRow> view;
		protected ReportGenerationOptions options;
		ILayoutContextStrategy strategy;
		public ReportLayoutContext(ILayoutContextStrategy strategy) {
			this.strategy = strategy;
		}
		public void Execute() {
			strategy.ConstructReportHeader();
			strategy.ConstructReportGroupHeader();
			strategy.ConstructReportDetail();
			strategy.ConstructReportGroupFooter();
			strategy.ConstructReportPageFooter();
		}
	}
}
