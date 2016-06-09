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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class PieSeries2DPointLayout : PieSeries2DPointLayoutBase {
		readonly Point initialPieCenter;
		readonly double initialStartAngle;
		readonly double initialFinishAngle;
		readonly double initialRadius;
		readonly double holeRadiusPercent;
		readonly double rotation;
		readonly Point clipCenterPoint;
		double radius;
		Point pieCenter;
		double startAngle;
		double finishAngle;
		Point clipCenter;
		double HoleRadius {
			get {
				double holeRadius = radius * holeRadiusPercent / 100.0;
				if (MathUtils.StrongRound(holeRadius) == MathUtils.StrongRound(radius))
					holeRadius = radius > 1 ? radius - 1 : 0;
				return holeRadius;
			}
		}
		internal double InitialStartAngle { get { return initialStartAngle; } }
		internal double InitialFinishAngle { get { return initialFinishAngle; } }
		internal double InitialRadius { get { return initialRadius; } }
		internal Point InitialPieCenter { get { return initialPieCenter; } }
		internal double Rotation { get { return rotation; } }
		public override Rect Bounds {
			get { return new Rect(pieCenter.X - radius, pieCenter.Y - radius, 2 * radius, 2 * radius); }
		}
		public override Geometry ClipGeometry {
			get {
				DonutSegment segment = new DonutSegment(clipCenter, new Point(radius, radius), HoleRadius, radius + 1, startAngle, finishAngle);
				segment.RotateCCW(rotation);
				return CalculateClipGeometry(segment);
			}
		}
		public PieSeries2DPointLayout(Rect viewport, Point pieCenter, double radius, double holeRadiusPercent, double rotation, double startAngle, double finishAngle, Point clipCenterPoint)
			: base(viewport) {
			this.initialPieCenter = pieCenter;
			this.initialStartAngle = startAngle;
			this.initialFinishAngle = finishAngle;
			this.initialRadius = radius;
			this.holeRadiusPercent = holeRadiusPercent;
			this.rotation = rotation;
			this.clipCenterPoint = clipCenterPoint;
		}
		internal void Complete(Point pieCenter, double radius, double startAngle, double finishAngle) {
			this.pieCenter = pieCenter;
			this.radius = radius;
			this.startAngle = startAngle;
			this.finishAngle = finishAngle;
			clipCenter = new Point(radius + (clipCenterPoint.X - pieCenter.X), radius + (clipCenterPoint.Y - pieCenter.Y));
		}
	}
}
