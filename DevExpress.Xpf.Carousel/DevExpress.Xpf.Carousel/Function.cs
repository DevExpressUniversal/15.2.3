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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
namespace DevExpress.Xpf.Carousel {
	public abstract class FunctionBase {
		public double GetValue(double x) {
			return GetValueOverride(x);
		}
		protected abstract double GetValueOverride(double x);
	}
	public class EqualFunction : FunctionBase {
		protected override double GetValueOverride(double x) {
			return x;
		}
	}
	public class LinearFunction : FunctionBase {
		public double K { get; set; }
		public double B { get; set; }
		public LinearFunction() { }
		public LinearFunction(double k, double b) {
			K = k;
			B = b;
		}
		public LinearFunction(Point p1, Point p2) {
			K = (p2.Y - p1.Y) / (p2.X - p1.X);
			B = p1.Y - K * p1.X;
		}
		protected override double GetValueOverride(double x) {
			return K * x + B;
		}
	}
	public class PointCollection : List<Point> {
	}
	public class PieceLinearFunction : FunctionBase {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PointCollection Points { get; set; }
		public PieceLinearFunction() {
			this.Points = new PointCollection();
		}
		public PieceLinearFunction(params Point[] points) : this() {
			Points.AddRange(points);
		}
		protected override double GetValueOverride(double x) {
			for(int i = 0; i < Points.Count - 1; i++) {
				Point p1 = Points[i];
				Point p2 = Points[i + 1];
				if(p2.X == p1.X) 
					return p2.Y;
				if(p1.X <= x && x < p2.X) {
					return p1.Y + (p2.Y - p1.Y) * (x - p1.X) / (p2.X - p1.X);
				}
			}
			int n = 0;
			if(x >= Points[Points.Count - 1].X)
				n = Points.Count - 1;
			return Points[n].Y;
		}
	}
	public class SquareFunction : FunctionBase {
		protected override double GetValueOverride(double x) {
			return x * x;
		}
	}
	public class SineFunction : FunctionBase {
		public double A { get; set; }
		public double W { get; set; }
		public SineFunction() {
			W = 2 * Math.PI;
		}
		public SineFunction(double a, double w) {
			this.A = a;
			this.W = w;
		}
		protected override double GetValueOverride(double x) {
			return A * Math.Sin(W * x);
		}
	}
	public class ZeroFunction : FunctionBase {
		protected override double GetValueOverride(double x) {
			return 0;
		}
	}
	public class ConstantFunction : FunctionBase {
		public double Constant { get; set; }
		public ConstantFunction() { }
		public ConstantFunction(double c) {
			Constant = c;
		}
		protected override double GetValueOverride(double x) {
			return Constant;
		}
	}
}
