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
using System;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class CircularDiagramMapping : IMapping {
		readonly CircularAxisMapping axisXMapping;
		readonly IAxisMapping axisYMapping;
		readonly Rect viewport;
		Point viewportCenter; 
		public Rect Viewport { get { return viewport; } }
		public CircularAxisMapping AxisXMapping { get { return axisXMapping; } }
		public IAxisMapping AxisYMapping { get { return axisYMapping; } }
		public CircularDiagramMapping(CircularDiagram2D diagram) : this (diagram, diagram.ActualViewport) {			
		}
		public CircularDiagramMapping(CircularDiagram2D diagram, Rect viewport) {
			this.viewport = viewport;
			axisXMapping = diagram.AxisXImpl.CreateMapping(viewport) as CircularAxisMapping;
			axisYMapping = diagram.AxisYImpl.CreateMapping(viewport);
			viewportCenter = viewport.CalcRelativeToLeftTopCenter();
		}
		#region IMapping implementation
		bool IMapping.Rotated { get { return false; } }
		bool IMapping.NavigationEnabled { get { return false; } }
		Point IMapping.GetRoundedDiagramPoint(double argument, double value) { return GetDiagramPoint(argument, value); }
		#endregion
		public bool IsLabelVisibleForResolveOverlapping(Point initialAnchorPoint) {
			Point center = viewport.CalcCenter();
			Point point = new Point(MathUtils.StrongRound(initialAnchorPoint.X), MathUtils.StrongRound(initialAnchorPoint.Y));
			double angle = GeometricUtils.CalcBetweenPointsAngle(new GRealPoint2D(center.X, center.Y), new GRealPoint2D(point.X, point.Y));
			double vectorLength = axisYMapping.Lenght * axisXMapping.GetValueScaleByAngle(angle);
			return MathUtils.CalcDistance(center, point) < vectorLength;			
		}
		public Point GetDiagramPoint(double argument, double value) {
			if (!axisXMapping.ValueInRange(argument))
				return new Point(0, 0);
			double angleRadian = axisXMapping.GetNormalizedAngleRadian(argument);
			double valueScale = axisXMapping.GetValueScale(argument);
			double positionVectorLength = axisYMapping.GetAxisValue(value);
			Vector directingVector = new Vector(Math.Cos(angleRadian), -Math.Sin(angleRadian));
			Vector positionVector = directingVector * positionVectorLength * valueScale;
			return Vector.Add(positionVector, viewportCenter);
		}
	}
}
