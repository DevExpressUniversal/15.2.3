#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Printing;
using DevExpress.Export;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class GridDashboardItemDataAwareExporter : DashboardItemExcelExporter<GridExportColumn, GridExportRow> {
		public GridDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new GridDashboardItemDataExporter(data, options), options) {
		}
	}
	public class PivotDashboardItemDataAwareExporter : DashboardItemExcelExporter<PivotExportColumn, PivotExportRow> {
		public PivotDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new PivotDashboardItemDataExporter(data, options), options) {
		}
	}
	public class ChoroplethMapDashboardItemDataAwareExporter : GridViewExcelExporter<ChoroplethMapExportColumn, ChoroplethMapExportRow> {
		public ChoroplethMapDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new ChoroplethMapDashboardItemDataExporter(data, options), options) {
		}
	}
	public class GeoPointMapDashboardItemDataAwareExporter : DashboardItemExcelExporter<GeoPointMapExportColumn, GeoPointMapExportRow> {
		public GeoPointMapDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new GeoPointMapDashboardItemDataExporter(data, options), options) {
		}
	}
	public class BubbleMapDashboardItemDataAwareExporter : DashboardItemExcelExporter<GeoPointMapExportColumn, GeoPointMapExportRow> {
		public BubbleMapDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new BubbleMapDashboardItemDataExporter(data, options), options) {
		}
	}
	public class PieMapDashboardItemDataAwareExporter : DashboardItemExcelExporter<GeoPointMapExportColumn, GeoPointMapExportRow> {
		public PieMapDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new PieMapDashboardItemDataExporter(data, options), options) {
		}
	}
	public class CardDashboardItemDataAwareExporter : GridViewExcelExporter<CardExportColumn, CardExportRow> {
		public CardDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new CardDashboardItemDataExporter(data, options), options) {
		}
	}
	public class GaugeDashboardItemDataAwareExporter : GridViewExcelExporter<GaugeExportColumn, GaugeExportRow> {
		public GaugeDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new GaugeDashboardItemDataExporter(data, options), options) {
		}
	}
	public class PieDashboardItemDataAwareExporter : GridViewExcelExporter<PieExportColumn, PieExportRow> {
		public PieDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new PieDashboardItemDataExporter(data, options), options) {
		}
	}
	public class ChartDashboardItemDataAwareExporter : DashboardItemExcelExporter<ChartExportColumn, ChartExportRow> {
		public ChartDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new ChartDashboardItemDataExporter(data, options), options) {
		}
	}
	public class ScatterChartDashboardItemDataAwareExporter : GridViewExcelExporter<ScatterChartExportColumn, ScatterChartExportRow> {
		public ScatterChartDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new ScatterChartDashboardItemDataExporter(data, options), options) {
		}
	}
	public class RangeFilterDashboardItemDataAwareExporter : GridViewExcelExporter<RangeFilterExportColumn, RangeFilterExportRow> {
		public RangeFilterDashboardItemDataAwareExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(new RangeFilterDashboardItemDataExporter(data, options), options) {
		}
	}
}
