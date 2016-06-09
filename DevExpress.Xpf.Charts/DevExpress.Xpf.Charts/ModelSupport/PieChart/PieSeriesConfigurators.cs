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
using System.Diagnostics;
using System.Reflection;
using DevExpress.Utils;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public abstract class PieSeriesPropertiesConfiguratorBase : GeneralSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			PieSeries pieSeries = (PieSeries)series;
			ConfigureExplodeMode(pieSeries, (Model.PieSeriesBase)seriesModel);
			ConfigureHoleRadiusPercent(pieSeries, (Model.PieSeriesBase)seriesModel);
		}
		void ConfigureExplodeMode(PieSeries series, Model.PieSeriesBase seriesModel) {
			switch (seriesModel.ExplodeMode) {
				case Model.PieExplodeMode.All:
					foreach (SeriesPoint point in series.Points)
						PieSeries.SetExplodedDistance(point, seriesModel.ExplodedDistancePercentage / 100.0);
					break;
				case Model.PieExplodeMode.UsePoints:
					foreach (SeriesPoint point in series.Points)
						PieSeries.SetExplodedDistance(point, 0.0);
					foreach (int pointIndex in seriesModel.ExplodedPointsIndexes)
						PieSeries.SetExplodedDistance(series.Points[pointIndex], seriesModel.ExplodedDistancePercentage / 100.0);
					break;
				default:
					foreach (SeriesPoint point in series.Points)
						PieSeries.SetExplodedDistance(point, 0.0);
					break;
			}
		}
		void ConfigureHoleRadiusPercent(PieSeries series, Model.PieSeriesBase seriesModel) {
			if (seriesModel is Model.PieSeries)
				series.HoleRadiusPercent = 0.0;
			if (seriesModel is Model.DonutSeries)
				series.HoleRadiusPercent = ((Model.DonutSeries)seriesModel).HoleRadiusPercent;
		}
	}
	public class PieSeries2DPropertiesConfigurator : PieSeriesPropertiesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			PieSeries2D pieSeries = (PieSeries2D)series;
			Model.PieSeriesBase pieSeriesModel = (Model.PieSeriesBase)seriesModel;
			pieSeries.Rotation = pieSeriesModel.RotationAngle;
			pieSeries.SweepDirection = (PieSweepDirection)pieSeriesModel.SweepDirection;
			if (seriesModel.Label != null) {
				SeriesLabel label = series.ActualLabel;
				switch (seriesModel.Label.Position) {
					case Model.SeriesLabelPosition.OutsideEnd:
						PieSeries2D.SetLabelPosition(label, PieLabelPosition.Outside);
						break;
					case Model.SeriesLabelPosition.Center:
						PieSeries2D.SetLabelPosition(label, PieLabelPosition.Inside);
						break;
					case Model.SeriesLabelPosition.BestFit:
						PieSeries2D.SetLabelPosition(label, PieLabelPosition.TwoColumns);
						label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
						pieSeries.LabelsResolveOverlappingMinIndent = 3;
						break;
					default:
						break;
				}
			}
		}
	}
	public class NestedDonutPropertiesConfigurator : PieSeries2DPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			((NestedDonutSeries2D)series).InnerIndent = 0;
		}
	}
}
