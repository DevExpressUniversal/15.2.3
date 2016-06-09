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
	public class GeometricLine {
		public static Point CalculateIntersectionPoint(GeometricLine line1, GeometricLine line2) {
			double x = (line1.b * line2.c - line2.b * line1.c) / (line1.a * line2.b - line2.a * line1.b);
			double y = (line1.c * line2.a - line2.c * line1.a) / (line1.a * line2.b - line2.a * line1.b);
			return new Point(x, y);
		}
		double a;
		double b;
		double c;
		public GeometricLine(double a, double b, double c) {
			if (a==0 && b==0)
				throw new ArgumentException("At least one of parameters 'a' or 'b' must be non equal to 0");
			this.a = a;
			this.b = b;
			this.c = c;
		}
		public GeometricLine(Point start, Point end) {
			if (start == end)
				throw new ArgumentException("Points must be non equal");
			this.a = start.Y - end.Y;
			this.b = end.X - start.X;
			this.c = start.X * end.Y - end.X * start.Y;
		}
		public GeometricLine(GRealPoint2D start, GRealPoint2D end) {
			if (start == end)
				throw new ArgumentException("Points must be non equal");
			this.a = start.Y - end.Y;
			this.b = end.X - start.X;
			this.c = start.X * end.Y - end.X * start.Y;
		}
		public Vector GetDirectingVector() { 
			return new Vector(-a, b); 
		}
		public Vector GetNormalizedDirectingVector() {
			Vector normalized = GetDirectingVector();
			normalized.Normalize();
			return normalized;
		}
		public Vector GetNormalVector() {
			return new Vector(a, b);
		}
		public Vector GetNormalizedNormalVector() {
			Vector normalized = GetNormalVector();
			normalized.Normalize();
			return normalized;
		}
		public double DistanceToPoint(Point point) {
			return (a * point.X + b * point.Y + c) / Math.Sqrt(a * a + b * b);
		}
	}
}
