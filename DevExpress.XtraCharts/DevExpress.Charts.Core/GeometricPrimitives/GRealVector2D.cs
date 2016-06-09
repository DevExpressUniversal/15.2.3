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
namespace DevExpress.Charts.Native {
	public struct GRealVector2D {
		public static bool operator ==(GRealVector2D v1, GRealVector2D v2) {
			return v1.X == v2.X && v1.Y == v2.Y;
		}
		public static bool operator !=(GRealVector2D v1, GRealVector2D v2) {
			return !(v1 == v2);
		}
		public static GRealVector2D operator +(GRealVector2D v1, GRealVector2D v2) {
			GRealVector2D result = new GRealVector2D();
			result.x = v1.X + v2.X;
			result.y = v1.Y + v2.Y;
			return result;
		}
		public static GRealVector2D operator -(GRealVector2D v1, GRealVector2D v2) {
			GRealVector2D result = new GRealVector2D();
			result.x = v1.X - v2.X;
			result.y = v1.Y - v2.Y;
			return result;
		}
		public static GRealPoint2D operator +(GRealPoint2D point, GRealVector2D vector) {
			GRealPoint2D result = new GRealPoint2D();
			result.X = point.X + vector.X;
			result.Y = point.Y + vector.Y;
			return result;
		}
		public static GRealPoint2D operator -(GRealPoint2D point, GRealVector2D vector) {
			GRealPoint2D result = new GRealPoint2D();
			result.X = point.X - vector.X;
			result.Y = point.Y - vector.Y;
			return result;
		}
		public static GRealVector2D operator *(GRealVector2D vector, double scalar) {
			return new GRealVector2D(vector.X * scalar, vector.Y * scalar);
		}
		public static GRealVector2D operator *(double scalar, GRealVector2D vector) {
			return new GRealVector2D(vector.X * scalar, vector.Y * scalar);
		}
		double x;
		double y;
		public double X { get { return x; } }
		public double Y { get { return y; } }
		public double SquaredLength { get { return x * x + y * y; } }
		public double Length { get { return Math.Sqrt(SquaredLength); } }
		public GRealVector2D(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public GRealVector2D(GRealPoint2D start, GRealPoint2D end) {
			this.x = end.X - start.X;
			this.y = end.Y - start.Y;
		}
		public void Normalize() {
			double length = Length;
			if (length > 0) {
				x /= length;
				y /= length;
			}
			else {
				x = 0;
				y = 0;
			}
		}
		public override bool Equals(object obj) {
			return (obj is GRealVector2D) && this == (GRealVector2D)obj;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
