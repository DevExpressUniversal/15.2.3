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
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public struct DiagramVector {		
		public static DiagramVector Zero { get { return new DiagramVector(0.0, 0.0, 0.0); } }
		public static DiagramVector operator +(DiagramVector v1, DiagramVector v2) {
			return new DiagramVector(v1.dx + v2.dx, v1.dy + v2.dy, v1.dz + v2.dz);
		}
		public static DiagramVector operator *(DiagramVector v1, DiagramVector v2) {
			return new DiagramVector(v1.dy * v2.dz - v1.dz * v2.dy, v1.dz * v2.dx - v1.dx * v2.dz, v1.dx * v2.dy - v1.dy * v2.dx);
		}
		public static DiagramVector CreateNormalized(double dx, double dy, double dz) {
			DiagramVector vector = new DiagramVector(dx, dy, dz);
			vector.Normalize();
			return vector;
		}
		double dx;
		double dy;
		double dz;
		public double DX { get { return dx; } set { dx = value; } }
		public double DY { get { return dy; } set { dy = value; } }
		public double DZ { get { return dz; } set { dz = value; } }
		public bool IsZero { get { return dx == 0.0 && dy == 0.0 && dz == 0.0; } }
		public double SquaredLength { get { return dx * dx + dy * dy + dz * dz; } }
		public double Length { get { return Math.Sqrt(SquaredLength); } }
		public DiagramVector(double dx, double dy, double dz) {
			this.dx = dx;
			this.dy = dy;
			this.dz = dz;
		}
		public void Normalize() {
			double length = Length;
			if (ComparingUtils.CompareDoubles(length, 0.0, 1e-5) == 0) {
				dx = 0;
				dy = 0;
				dz = 0;
			}
			else {
				dx /= length;
				dy /= length;
				dz /= length;
			}
		}
		public void Revert() {
			dx = -dx;
			dy = -dy;
			dz = -dz;
		}
		public override bool Equals(object obj) {
			if (!(obj is DiagramVector))
				return false;
			DiagramVector vector = (DiagramVector)obj;
			return vector.dx == dx && vector.dy == dy && vector.dz == dz;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
