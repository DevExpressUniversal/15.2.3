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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	public class ScatterChartViewBuilder : ModelViewBuilderBase {
		public ScatterChartViewBuilder(ModelChartBuilder modelBuilder, ScatterChartView view)
			: base(modelBuilder, view) {
		}
		protected new ScatterChartView View { get { return base.View as ScatterChartView; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			ScatterSeries scatterSeries = (ScatterSeries)series;
			ScatterChartStyle scatterStyle = scatterSeries.GetActualScatterStyle();
			if (scatterStyle == ScatterChartStyle.Marker)
				return new ChartsModel.PointSeries();
			return new ChartsModel.ScatterLineSeries();
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			SetupSeriesMarker(modelSeries, series);
		}
	}
}
