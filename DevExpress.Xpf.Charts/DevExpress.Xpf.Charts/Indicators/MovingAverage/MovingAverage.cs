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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum MovingAverageKind {
		MovingAverage = MovingAverageKindInternal.MovingAverage,
		Envelope = MovingAverageKindInternal.Envelope,
		MovingAverageAndEnvelope = MovingAverageKindInternal.MovingAverageAndEnvelope
	}
	public abstract class MovingAverage : Indicator {
		public static readonly DependencyProperty ValueLevelProperty = DependencyPropertyManager.Register("ValueLevel", typeof(ValueLevel), typeof(MovingAverage), new PropertyMetadata(ValueLevel.Value, ChartElementHelper.Update));
		public static readonly DependencyProperty MovingAverageKindProperty = DependencyPropertyManager.Register("MovingAverageKind", typeof(MovingAverageKind), typeof(MovingAverage), new PropertyMetadata(MovingAverageKind.MovingAverage, ChartElementHelper.Update));
		public static readonly DependencyProperty EnvelopePercentProperty = DependencyPropertyManager.Register("EnvelopePercent", typeof(double), typeof(MovingAverage), new PropertyMetadata(3.0, ChartElementHelper.Update), new ValidateValueCallback(ValidateEnvelopePercentProperty));
		public static readonly DependencyProperty PointsCountProperty = DependencyPropertyManager.Register("PointsCount", typeof(int), typeof(MovingAverage), new PropertyMetadata(10, ChartElementHelper.Update), new ValidateValueCallback(ValidatePointsCountProperty));
		protected List<GRealPoint2D> movingAverageData;
		protected List<GRealPoint2D> upperEnvelopeData;
		protected List<GRealPoint2D> lowerEnvelopeData;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel {
			get { return (ValueLevel)GetValue(ValueLevelProperty); }
			set { SetValue(ValueLevelProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public MovingAverageKind MovingAverageKind {
			get { return (MovingAverageKind)GetValue(MovingAverageKindProperty); }
			set { SetValue(MovingAverageKindProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double EnvelopePercent {
			get { return (double)GetValue(EnvelopePercentProperty); }
			set { SetValue(EnvelopePercentProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int PointsCount {
			get { return (int)GetValue(PointsCountProperty); }
			set { SetValue(PointsCountProperty, value); }
		}
		public MovingAverage() {
			DefaultStyleKey = typeof(MovingAverage);
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			MovingAverage movingAverage = indicator as MovingAverage;
			if (movingAverage != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, movingAverage, ValueLevelProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, movingAverage, MovingAverageKindProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, movingAverage, EnvelopePercentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, movingAverage, PointsCountProperty);
			}
		}
		public override Geometry CreateGeometry() {
			if (movingAverageData == null || movingAverageData.Count < 2)
				return null;
			PathGeometry pathGeometry = new PathGeometry();
			PaneMapping mapping = new PaneMapping(Pane, XYSeries);
			if (MovingAverageKind == MovingAverageKind.MovingAverage || MovingAverageKind == MovingAverageKind.MovingAverageAndEnvelope) {
				PathFigure pathFigure = new PathFigure();
				pathFigure.StartPoint = mapping.GetDiagramPoint(movingAverageData[0]);
				foreach (GRealPoint2D diagramPoint in movingAverageData) {
					Point screenPoint = mapping.GetDiagramPoint(diagramPoint);
					pathFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(pathFigure);
			}
			if (MovingAverageKind == MovingAverageKind.Envelope || MovingAverageKind == MovingAverageKind.MovingAverageAndEnvelope) {
				PathFigure upperEnvelopePathFigure = new PathFigure();
				upperEnvelopePathFigure.StartPoint = mapping.GetDiagramPoint(upperEnvelopeData[0]);
				foreach (GRealPoint2D diagramPoint in upperEnvelopeData) {
					Point screenPoint = mapping.GetDiagramPoint(diagramPoint);
					upperEnvelopePathFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(upperEnvelopePathFigure);
				PathFigure loverEnvelopePathFigure = new PathFigure();
				loverEnvelopePathFigure.StartPoint = mapping.GetDiagramPoint(lowerEnvelopeData[0]);
				foreach (GRealPoint2D diagramPoint in lowerEnvelopeData) {
					Point screenPoint = mapping.GetDiagramPoint(diagramPoint);
					loverEnvelopePathFigure.Segments.Add(new LineSegment() { Point = screenPoint });
				}
				pathGeometry.Figures.Add(loverEnvelopePathFigure);
			}
			return pathGeometry;
		}
		static bool ValidateEnvelopePercentProperty(object value){
			if (! (value is double))
				return false;
			double d = (double)value;
			if (d < 0)
				return false;
			return true;
		}
		static bool ValidatePointsCountProperty(object value) {
			if (!(value is int))
				return false;
			int i = (int)value;
			if (i <= 0)
				return false;
			return true;
		}
	}
}
