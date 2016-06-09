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
using System.Windows;
using DevExpress.Charts.Native;
using System;
namespace DevExpress.Xpf.Charts.Native {
	public static class ResolveOverlappingHelper {
		public static void Process(IList<XYSeries> series, Rect bounds, int resolveOverlappingMinIndent) {
			if (!IsLabelsResolveOverlapping(series))
				return;
			List<IXYDiagramLabelLayout> labels = new List<IXYDiagramLabelLayout>();
			int index = series.Count / 2;
			for (int i = index, k = index - 1; i < series.Count || k >= 0; i++, k--) {
				if (i < series.Count)
					FillNonOverlappingSeriesLabels(labels, series[i]);
				if (k >= 0)
					FillNonOverlappingSeriesLabels(labels, series[k]);
			}
			XYDiagramResolveOverlappingAlgorithm.Process(labels, bounds.IsEmpty ? GRect2D.Empty : GraphicsUtils.ConvertRect(bounds), resolveOverlappingMinIndent);
		}
		public static void Process(PieLabelPosition position, List<IPieLabelLayout> labels, Rect labelBounds, GRealEllipse pie, int resolveOverlappingMinIndent, PieSweepDirection direction) {
			switch (position) {
				case PieLabelPosition.Outside:
					SimpleDiagrammResiolveOverlapping.ArrangeByEllipse(labels, pie, resolveOverlappingMinIndent, (PointsSweepDirection)direction, 
						new GRealRect2D(labelBounds.Left, labelBounds.Top, labelBounds.Width, labelBounds.Height));
					break;
				case PieLabelPosition.TwoColumns:
					SimpleDiagrammResiolveOverlapping.ArrangeByColumn(labels,
						new GRealRect2D(labelBounds.Left, labelBounds.Top, labelBounds.Width, labelBounds.Height), resolveOverlappingMinIndent);
					break;
			}
		}
		public static GRealPoint2D? CalculateSalientPoint(GRealPoint2D anchorPoint, GRect2D labelBounds, double startAngle) {
			GRealPoint2D center = MathUtils.CalcCenter(labelBounds);
			GRealPoint2D endPoint = new GRealPoint2D(center.X > anchorPoint.X ? labelBounds.Left : labelBounds.Right, center.Y);
			GRealPoint2D p = new GRealPoint2D(anchorPoint.X + Math.Cos(startAngle), anchorPoint.Y + Math.Sin(startAngle));
			GRealPoint2D? inersection = GeometricUtils.CalcLinesIntersection(anchorPoint, p, endPoint, new GRealPoint2D(endPoint.X + 5, endPoint.Y), false);
			if (inersection.HasValue) {
				GRealPoint2D point = inersection.Value;
				if ((point.X >= anchorPoint.X && point.X < endPoint.X) ||
					(point.X <= anchorPoint.X && point.X > endPoint.X))
					return point;
			}
			return null;
		} 
		static bool IsLabelsResolveOverlapping(IList<XYSeries> series) {
			foreach (XYSeries item in series)
				if (item.ActualLabel.ResolveOverlappingMode != ResolveOverlappingMode.None)
					return true;
			return false;
		}
		static void AddLayout(List<IXYDiagramLabelLayout> labels, SeriesLabelItem labelItem) {
			XYSeriesLabel2DLayout layout = labelItem.Layout as XYSeriesLabel2DLayout;
			if (layout != null)
				labels.Add(layout);
		}
		static void FillNonOverlappingSeriesLabels(List<IXYDiagramLabelLayout> labels, Series series) {
			IList<SeriesLabelItem> labelItems = series.ActualLabel.Items;
			if (labelItems != null) {
				int index = labelItems.Count / 2;
				if (series.ActualLabel.ResolveOverlappingMode != ResolveOverlappingMode.None &&
					series.ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.HideOverlapped)
					for (int i = 0, k = labelItems.Count - 1; i <= k; i++, k--) {
						AddLayout(labels, labelItems[i]);
						if (k != i)
							AddLayout(labels, labelItems[k]);
					}
				else
					for (int i = index, k = index - 1; i < labelItems.Count || k >= 0; i++, k--) {
						if (i < labelItems.Count)
							AddLayout(labels, labelItems[i]);
						if (k >= 0)
							AddLayout(labels, labelItems[k]);
					}
			}
		}
	}
}
