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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class CardDashboardItemDataExporter : KpiDashboardItemDataExporter<CardExportColumn, CardExportRow, CardViewModel> {
		CardDashboardItemViewModel CardViewModel { get { return (CardDashboardItemViewModel)ViewModel; } }
		public CardDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
		}
		protected override void AddColumnsByElement(IList<CardExportColumn> columns, CardViewModel element) {
			base.AddColumnsByElement(columns, element);
			if(CardViewModel.IsSparklineMode && element.ShowSparkline) {
				DataAxis sparklineAxis = MDData.GetAxis(DashboardDataAxisNames.SparklineAxis);
				if(sparklineAxis != null) {
					IList<AxisPoint> sparklinePoints = sparklineAxis.GetPoints();
					if(sparklinePoints.Count > maxXlsxColumnCount) {
						columns.Add(new CardExportColumn() {
							IsIncorrectSparklineColumn = true,
							FormatSettings = new FormatSettings()
						});
					}
					else {
						MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(element.ID);
						columns.Add(new CardExportColumn() {
							Header = string.Format("{0} {1}", measure.Name, "Sparkline"),
							ColEditType = ColumnEditTypes.Sparkline,
							SparklineInfo = new SparklineInfo(element.SparklineOptions),
							Measure = measure,
							SparklinePoints = sparklinePoints,
							FormatSettings = new FormatSettings(),
							LogicalPosition = columns.Count
						});
					}
				}
			}
		}
		protected override IEnumerable<CardViewModel> GetElements() {
			return CardViewModel.Cards;
		}
		protected override CardViewModel GetElement() {
			return CardViewModel.Cards[0];
		}
		protected override object GetRowCellValue(CardExportRow row, CardExportColumn column) {
			if(column.IsIncorrectSparklineColumn)
				return sparklineColumnMessage;
			IList<AxisPoint> sparklinePoints = column.SparklinePoints;
			if(sparklinePoints == null)
				return base.GetRowCellValue(row, column);
			IList<object> values = new List<object>();
			foreach(AxisPoint point in column.SparklinePoints)
				values.Add(MDData.GetValue(row.AxisPoint, point, column.Measure).Value);
			return values;
		}
	}
}
