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
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public struct DonutSegment {
		public Point Center { get; private set; }
		public Point RelativeCenter { get; private set; }
		public double InnerRadius { get; private set; }
		public double OuterRadius { get; private set; }
		public double StartAngleDeg { get; private set; }
		public double EndAngleDeg { get; private set; }
		public double ArcAngleDeg { get; private set; }
		public double MedianAngleDeg { get; private set; }
		public DonutSegment(Point center, Point relativeCenter, double innerRadius, double outerRadius, Slice startEndAnglesDeg, PieSweepDirection sweepDirection)
			: this() {
			if (innerRadius < 0)
				throw new ArgumentException("Value must be greater than or equal to 0.", "innerRadius");
			if (outerRadius < 0)
				throw new ArgumentException("Value must be greater than or equal to 0.", "outerRadius");
			Center = center;
			RelativeCenter = relativeCenter;
			InnerRadius = innerRadius;
			OuterRadius = outerRadius;
			if (sweepDirection == PieSweepDirection.Counterclockwise) {
				StartAngleDeg = startEndAnglesDeg.StartAngle;
				EndAngleDeg = startEndAnglesDeg.FinishAngle;
			}
			else {
				StartAngleDeg = startEndAnglesDeg.FinishAngle;
				EndAngleDeg = startEndAnglesDeg.StartAngle;
			}
			ArcAngleDeg = Math.Abs(EndAngleDeg - StartAngleDeg);
			MedianAngleDeg = 0.5 * (StartAngleDeg + EndAngleDeg);
		}
		public DonutSegment(Point center, Point relativeCenter, double innerRadius, double outerRadius, double startAngleDeg, double endAngleDeg)
			: this() {
			if (innerRadius < 0)
				throw new ArgumentException("Value must be greater than or equal to 0.", "innerRadius");
			if (outerRadius < 0)
				throw new ArgumentException("Value must be greater than or equal to 0.", "outerRadius");
			Center = center;
			RelativeCenter = relativeCenter;
			InnerRadius = innerRadius;
			OuterRadius = outerRadius;
			StartAngleDeg = startAngleDeg;
			EndAngleDeg = endAngleDeg;
			ArcAngleDeg = Math.Abs(EndAngleDeg - StartAngleDeg);
			MedianAngleDeg = 0.5 * (StartAngleDeg + EndAngleDeg);
		}
		public void RotateCCW(double angleDeg) {
			double diff = EndAngleDeg - StartAngleDeg;
			StartAngleDeg = MathUtils.NormalizeDegree(StartAngleDeg - angleDeg);
			EndAngleDeg = StartAngleDeg + diff;
		}
	}
}
