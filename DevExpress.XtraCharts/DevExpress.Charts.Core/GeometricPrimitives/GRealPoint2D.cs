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
	public struct GRealPoint2D {
		public static bool operator ==(GRealPoint2D p1, GRealPoint2D p2) {
			return p1.X == p2.X && p1.Y == p2.Y;
		}
		public static bool operator !=(GRealPoint2D p1, GRealPoint2D p2) {
			return !(p1 == p2);
		}
		public static explicit operator GRealPoint2D(GPoint2D point) {
			return new GRealPoint2D(point.X, point.Y);
		}
		public static double CalculateDistance (GRealPoint2D point1, GRealPoint2D point2){
			return Math.Sqrt(Math.Pow((point1.x - point2.x), 2) + Math.Pow((point1.y - point2.y), 2));
		}
		double x;
		double y;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public GRealPoint2D(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public override bool Equals(object obj) {
			return (obj is GRealPoint2D) && this == (GRealPoint2D)obj;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return "(" + x + "; " + y + ")"; 
		}
	}
}
