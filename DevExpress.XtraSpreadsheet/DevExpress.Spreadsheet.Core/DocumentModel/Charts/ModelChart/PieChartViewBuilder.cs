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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	#region PieChartViewBuilderBase
	public abstract class PieChartViewBuilderBase : ModelViewBuilderBase {
		protected PieChartViewBuilderBase(ModelChartBuilder modelBuilder, IChartView view)
			: base(modelBuilder, view) {
		}
		protected virtual void SetupExplosion(ChartsModel.PieSeriesBase modelSeries, PieSeries series) {
			modelSeries.ExplodeMode = GetPieExplodeMode(series);
			modelSeries.ExplodedDistancePercentage = GetPieExplosionDistance(series);
			if (modelSeries.ExplodeMode == ChartsModel.PieExplodeMode.UsePoints)
				modelSeries.ExplodedPointsIndexes.AddRange(GetExplodedPointsIndicies(series));
		}
		protected virtual int TranslateAngle(int angle) {
			int result = (angle - 90) % 360;
			if (result < 0)
				result += 360;
			return result;
		}
		ChartsModel.PieExplodeMode GetPieExplodeMode(PieSeries series) {
			foreach (DataPoint point in series.DataPoints) {
				if (point.HasExplosion && (point.Explosion != series.Explosion))
					return ChartsModel.PieExplodeMode.UsePoints;
			}
			return series.Explosion == 0 ? ChartsModel.PieExplodeMode.None : ChartsModel.PieExplodeMode.All;
		}
		int GetPieExplosionDistance(PieSeries series) {
			int result = series.Explosion;
			foreach (DataPoint point in series.DataPoints) {
				if (point.HasExplosion)
					result = Math.Max(point.Explosion, result);
			}
			return result;
		}
		IList<int> GetExplodedPointsIndicies(PieSeries series) {
			List<int> result = new List<int>();
			if (series.Explosion == 0) {
				foreach (DataPoint point in series.DataPoints) {
					if (point.HasExplosion && point.Explosion > 0)
						result.Add(point.Index);
				}
			}
			else {
				int count = (int)series.Values.ValuesCount;
				for (int i = 0; i < count; i++) {
					DataPoint point = series.DataPoints.FindByIndex(i);
					if (point == null || point.Explosion > 0)
						result.Add(i);
				}
			}
			return result;
		}
	}
	#endregion
	#region PieChartViewBuilder
	public class PieChartViewBuilder : PieChartViewBuilderBase {
		public PieChartViewBuilder(ModelChartBuilder modelBuilder, PieChartView view)
			: base(modelBuilder, view) {
		}
		protected new PieChartView View { get { return (PieChartView)base.View; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			if(!IsFirstSeries)
				return null;
			return new ChartsModel.PieSeries();
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			modelSeries.LegendPointPattern = "{A}";
			ChartsModel.PieSeries pieModelSeries = (ChartsModel.PieSeries)modelSeries;
			SetupExplosion(pieModelSeries, (PieSeries)series);
			pieModelSeries.RotationAngle = TranslateAngle(View.FirstSliceAngle);
			pieModelSeries.SweepDirection = ChartsModel.PieSweepDirection.Clockwise;
		}
		protected override void SetupSeriesShowInLegend(ChartsModel.SeriesModel modelSeries) {
			modelSeries.ShowInLegend = true;
		}
	}
	#endregion
	#region Pie3DChartViewBuilder
	public class Pie3DChartViewBuilder : PieChartViewBuilderBase {
		public Pie3DChartViewBuilder(ModelChartBuilder modelBuilder, Pie3DChartView view)
			: base(modelBuilder, view) {
		}
		protected new Pie3DChartView View { get { return (Pie3DChartView)base.View; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			if (!IsFirstSeries)
				return null;
			return new ChartsModel.PieSeries();
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			modelSeries.LegendPointPattern = "{A}";
			ChartsModel.PieSeries pieModelSeries = (ChartsModel.PieSeries)modelSeries;
			SetupExplosion(pieModelSeries, (PieSeries)series);
			pieModelSeries.RotationAngle = 0;
			pieModelSeries.DepthPercent = 15;
			pieModelSeries.SweepDirection = ChartsModel.PieSweepDirection.Clockwise;
		}
		protected override void SetupSeriesShowInLegend(ChartsModel.SeriesModel modelSeries) {
			modelSeries.ShowInLegend = true;
		}
	}
	#endregion
}
