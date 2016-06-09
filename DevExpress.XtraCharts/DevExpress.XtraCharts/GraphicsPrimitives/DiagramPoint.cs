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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public struct DiagramPoint {
		public static DiagramPoint Zero { get { return new DiagramPoint(0.0, 0.0, 0.0); } }
		public static PointF[] Convert(IList<DiagramPoint> diagramPoints) {
			PointF[] points = new PointF[diagramPoints.Count];
			for (int i = 0; i < diagramPoints.Count; i++)
				points[i] = (PointF)diagramPoints[i];
			return points;
		}
		public static DiagramPoint Round(DiagramPoint point) {
			return new DiagramPoint(Math.Round(point.X), Math.Round(point.Y), Math.Round(point.Z));
		}
		public static DiagramPoint StrongRound(DiagramPoint point) {
			return new DiagramPoint(MathUtils.StrongRound(point.X), MathUtils.StrongRound(point.Y), MathUtils.StrongRound(point.Z));
		}
		public static DiagramPoint Offset(DiagramPoint point, double dx, double dy, double dz) {
			return new DiagramPoint(point.X + dx, point.Y + dy, point.Z + dz);
		}
		public static explicit operator Point(DiagramPoint point) {
			return new Point((int)MathUtils.StrongRound(point.X), (int)MathUtils.StrongRound(point.Y));
		}
		public static explicit operator PointF(DiagramPoint point) {
			return new PointF(System.Convert.ToSingle(point.X), System.Convert.ToSingle(point.Y));
		}
		public static explicit operator DiagramPoint(Point point) {
			return new DiagramPoint(point.X, point.Y);
		}
		public static explicit operator DiagramPoint(PointF point) {
			return new DiagramPoint(point.X, point.Y);
		}
		public static explicit operator GRealPoint2D(DiagramPoint point) {
			return new GRealPoint2D(point.X, point.Y);
		}
		public static explicit operator DiagramPoint(GRealPoint2D point) {
			return new DiagramPoint(point.X, point.Y);
		}
		public static bool operator ==(DiagramPoint left, DiagramPoint right) {
			return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
		}
		public static bool operator !=(DiagramPoint left, DiagramPoint right) {
			return !(left == right);
		}
		public static DiagramVector operator -(DiagramPoint p1, DiagramPoint p2) {
			return new DiagramVector(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
		}
		double x;
		double y;
		double z;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public double Z { get { return z; } set { z = value; } }
		public bool IsZero { get { return x == 0.0 && y == 0.0 && z == 0.0; } }
		public DiagramPoint(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public DiagramPoint(double x, double y) : this(x, y, 0.0) {
		}
		public override bool Equals(object obj) {
			if (!(obj is DiagramPoint))
				return false;
			DiagramPoint point = (DiagramPoint)obj;
			return point.X == x && point.Y == Y && point.Z == z;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return x + ";" + y + ";" + z;
		}
	}
}
