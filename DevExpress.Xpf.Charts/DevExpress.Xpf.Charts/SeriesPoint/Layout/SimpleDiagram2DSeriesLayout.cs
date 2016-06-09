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

using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class PieSeries2DPointLayoutBase : SeriesPointLayout {
		public PieSeries2DPointLayoutBase(Rect viewport)
			: base(viewport) { }
		public override Transform Transform {
			get { return null; }
		}
		protected Geometry CalculateClipGeometry(DonutSegment segment) {
			if (segment.ArcAngleDeg < 360.0) {
				bool isLargeArc = segment.ArcAngleDeg > 180.0;
				PathFigure figure = new PathFigure();
				figure.StartPoint = segment.RelativeCenter.MoveByAngle(segment.InnerRadius, segment.StartAngleDeg);
				LineSegment line1 = new LineSegment() {
					Point = segment.RelativeCenter.MoveByAngle(segment.OuterRadius, segment.StartAngleDeg)
				};
				figure.Segments.Add(line1);
				ArcSegment outerArc = new ArcSegment() {
					Point = segment.RelativeCenter.MoveByAngle(segment.OuterRadius, segment.EndAngleDeg),
					Size = new Size(segment.OuterRadius, segment.OuterRadius),
					IsLargeArc = isLargeArc,
					SweepDirection = SweepDirection.Counterclockwise
				};
				figure.Segments.Add(outerArc);
				LineSegment line2 = new LineSegment() {
					Point = segment.RelativeCenter.MoveByAngle(segment.InnerRadius, segment.EndAngleDeg)
				};
				figure.Segments.Add(line2);
				ArcSegment innerArc = new ArcSegment() {
					Point = figure.StartPoint,
					Size = new Size(segment.InnerRadius, segment.InnerRadius),
					IsLargeArc = isLargeArc,
					SweepDirection = SweepDirection.Clockwise
				};
				figure.Segments.Add(innerArc);
				PathGeometry geometry = new PathGeometry();
				geometry.Figures.Add(figure);
				return geometry;
			}
			else {
				GeometryGroup geometry = new GeometryGroup() { FillRule = FillRule.EvenOdd };
				geometry.Children.Add(new EllipseGeometry() { 
					Center = segment.RelativeCenter, 
					RadiusX = segment.OuterRadius, 
					RadiusY = segment.OuterRadius 
				});
				geometry.Children.Add(new EllipseGeometry() { 
					Center = segment.RelativeCenter, 
					RadiusX = segment.InnerRadius, 
					RadiusY = segment.InnerRadius 
				});
				return geometry; 
			}
		}
	}
}
