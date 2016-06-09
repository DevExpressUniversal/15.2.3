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
using System.IO;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.Export;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
namespace DevExpress.DashboardExport {
	public class DashboardExporter : IDashboardExporter {
		static DashboardExporter instance;
		public static IDashboardExporter Instance {
			get {
				if(instance == null)
					instance = new DashboardExporter();
				return instance;
			}
		}
		static IDataAwareExportOptions CreateExportOptions(ExcelExportOptions exportOptions, string sheetName) {
			IDataAwareExportOptions options = null;
			switch(exportOptions.Format) {
				case ExcelFormat.Xls:
					options = new XlsExportOptionsEx();
					break;
				case ExcelFormat.Xlsx:
					options = new XlsxExportOptionsEx();
					break;
				case ExcelFormat.Csv:
					options = new CsvExportOptionsEx() { Separator = exportOptions.CsvValueSeparator };
					break;
				default:
					throw new NotSupportedException();
			}
			options.SheetName = sheetName;
			return options;
		}
		static void ExportData(Stream stream, DashboardExportData data, FormatOptions formatOptions) {
			DashboardItemExportData itemData = data.ItemsData[0];
			IDataAwareExportOptions options = CreateExportOptions(formatOptions.ExcelOptions, itemData.ServerData.ViewModel.Caption);
			switch(itemData.ServerData.Type) {
				case DashboardItemType.Grid:
					new GridDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.Pivot:
					new PivotDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.ChoroplethMap:
					new ChoroplethMapDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.GeoPointMap:
					new GeoPointMapDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.BubbleMap:
					new BubbleMapDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.PieMap:
					new PieMapDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.Card:
					new CardDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.Gauge:
					new GaugeDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.Pie:
					new PieDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.Chart:
					new ChartDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.ScatterChart:
					new ScatterChartDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				case DashboardItemType.RangeFilter:
					new RangeFilterDashboardItemDataAwareExporter(itemData, options).Export(stream);
					break;
				default:
					break;
			}
		}
		DashboardExporter() { }
		public IReportHolder CreateReportHolder(DashboardExportMode mode, DashboardExportData data, DashboardReportOptions opts) {
			return (mode == DashboardExportMode.EntireDashboard) ?
				 (IReportHolder)new EntireDashboardReportHolder(data, opts) :
				 (IReportHolder)new SingleDashboardItemReportHolder(data.ItemsData[0], opts);
		}
		public void Export(Stream stream, DashboardExportMode mode, DashboardExportData data, DashboardReportOptions opts) {
			FormatOptions formatOptions = opts.FormatOptions;
			if(formatOptions.Format == DashboardExportFormat.Excel)
				ExportData(stream, data, formatOptions);
			else
				using(IReportHolder holder = CreateReportHolder(mode, data, opts)) {
					IReport report = holder.Report;
					switch(formatOptions.Format) {
						case DashboardExportFormat.PDF:
							report.PrintingSystemBase.ExportToPdf(stream, formatOptions.PdfOptions);
							break;
						case DashboardExportFormat.Image:
							report.PrintingSystemBase.ExportToImage(stream, formatOptions.ImageOptions);
							break;
						default:
							throw new NotSupportedException();
					}
				}
		}
	}
}
