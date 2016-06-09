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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class SeriesData {
		readonly Series series;
		public Series Series { get { return series; } }
		public SeriesData(Series series) {
			this.series = series;
		}
		public void ValidateSeriesPointsCache(Diagram3DDomain domain, IRefinedSeries refinedSeries) {
			if (!series.Cache.IsPointsCacheDefinesFully)
				foreach (RefinedPoint refinedPoint in refinedSeries.Points)
					if (!refinedPoint.IsEmpty) {
						series.Cache.ValidateSeriesPointCache(refinedPoint, CreateSeriesPointGeometry(domain, refinedPoint));
					}
		}
		protected PointGeometry GetPointGeometry(RefinedPoint refinedPoint, Diagram3DDomain domain) {
			SeriesPointCache seriesPointCache = Series.Cache.GetSeriesPointCache(refinedPoint.SeriesPoint);
			if (seriesPointCache != null) {
				if (seriesPointCache.PointGeometry != null)
					return seriesPointCache.PointGeometry;
				else
					ChartDebug.Fail("PointGeometry cant't be null.");
			} else
				ChartDebug.Fail("SeriesPointCache cant't be null.");
			return CreateSeriesPointGeometry(domain, refinedPoint);
		}
		protected ContentPresenter CreateSeriesPointLabelContentPresenter(RefinedPoint refinedPoint) {
			SeriesLabel seriesLabel = series.ActualLabel;
			if (refinedPoint.IsEmpty || !series.LabelsVisibility)
				return null;
			string labelText = String.Empty;
			SeriesPointCache cache = series.Cache.GetSeriesPointCache(refinedPoint.SeriesPoint);
			if (cache != null)
				labelText = cache.LabelText;
			else {
				ChartDebug.Fail("SeriesPointCache cant't be null.");
				labelText = series.GetLabelsTexts(refinedPoint)[0];
			}
			ContentPresenter presenter = new ContentPresenter();
			presenter.ContentTemplate = TemplateHelper.GetSeriesLabelTemplate();
			presenter.Content = new SeriesLabelItem(seriesLabel, labelText, Series.GetPointLabelColor(refinedPoint));
			presenter.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			presenter.Arrange(new Rect() { Width = presenter.DesiredSize.Width, Height = presenter.DesiredSize.Height });
			return presenter;
		}
		protected virtual PointGeometry CreateSeriesPointGeometry(Diagram3DDomain domain, RefinedPoint refinedPoint) {
			return new PointGeometry(series.ActualLabel, CreateSeriesPointLabelContentPresenter(refinedPoint));
		}
		protected internal abstract Point3D CalculateToolTipPoint(SeriesPointCache pointCache, Diagram3DDomain domain);
	}
}
