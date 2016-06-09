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
	public interface IMinMaxValues {
		double Min { get; set; }
		double Max { get; set; }
		double Delta { get; }
		double CalculateCenter();
		void Intersection(IMinMaxValues minMaxValues);
	}
	public struct MinMaxValues : IMinMaxValues {
		public static MinMaxValues Empty { get { return new MinMaxValues(Double.NaN, Double.NaN); } }
		public static double CalculateCenter(IMinMaxValues minMaxValues) {
			if (double.IsNaN(minMaxValues.Min) || double.IsNaN(minMaxValues.Max))
				return double.NaN;
			return (minMaxValues.Max + minMaxValues.Min) / 2;
		}
		public static MinMaxValues Intersection(IMinMaxValues first, IMinMaxValues second) {
			if (!NotEmpty(first) && !NotEmpty(second))
				return Empty;
			if (!NotEmpty(first))
				return Clone(second);
			if (!NotEmpty(second))
				return Clone(first);
			double min = Math.Max(first.Min, second.Min);
			double max = Math.Min(first.Max, second.Max);
			MinMaxValues intersection = new MinMaxValues(min, max);
			if (intersection.Delta <= first.Delta && intersection.Delta <= second.Delta)
				return intersection;
			return Empty;
		}
		public static bool IsEmptyValue(IMinMaxValues value) {
			return double.IsNaN(value.Max) && double.IsNaN(value.Min);
		}
		static MinMaxValues Clone(IMinMaxValues value) {
			return new MinMaxValues(value.Min, value.Max);
		}
		static bool NotEmpty(IMinMaxValues value) {
			return !double.IsNaN(value.Min) && !double.IsNaN(value.Max);
		}
		double min;
		double max;
		public double Min {
			get { return min; }
			set { min = value; }
		}
		public double Max {
			get { return max; }
			set { max = value; }
		}
		public double Delta { get { return Max - Min; } }
		public bool IsEmpty { get { return double.IsNaN(max) && double.IsNaN(min); } }
		public bool HasValues { get { return !IsEmpty && min != double.MaxValue && max != double.MinValue && min != double.NegativeInfinity && max != double.PositiveInfinity; } }
		public static bool operator ==(MinMaxValues a, MinMaxValues b) {
			if (System.Object.ReferenceEquals(a, b))
				return true;
			if (a.IsEmpty ^ b.IsEmpty)
				return false;
			return a.Min == b.min && a.Max == b.Max;
		}
		public static bool operator !=(MinMaxValues a, MinMaxValues b) {
			return !(a == b);
		}
		public static MinMaxValues Union(MinMaxValues a, MinMaxValues b) {
			if (!a.HasValues)
				return b;
			if (!b.HasValues)
				return a;
			double min = Math.Min(a.min, b.min);
			double max = Math.Max(a.max, b.max);
			return new MinMaxValues(min, max);
		}
		public static MinMaxValues Union(MinMaxValues a, double b) {
			if (!a.HasValues)
				return a;
			if (double.IsNaN(b))
				return a;
			double min = Math.Min(a.min, b);
			double max = Math.Max(a.max, b);
			return new MinMaxValues(min, max);
		}
		public MinMaxValues(double min, double max) {
			this.min = min;
			this.max = max;
		}
		public MinMaxValues(double value) : this(value, value) { }
		public MinMaxValues(IMinMaxValues values) : this(values.Min, values.Max) { }
		public void Union(double value) {
			if (double.IsNaN(value))
				return;
			if (double.IsNaN(min))
				min = value;
			else
				min = Math.Min(min, value);
			if (double.IsNaN(max))
				max = value;
			else
				max = Math.Max(max, value);
		}
		public MinMaxValues Union(MinMaxValues value) {
			return Union(this, value);
		}
		public bool InRange(double value) {
			return min <= value && value <= max;
		}
		public double CalculateCenter() {
			return MinMaxValues.CalculateCenter(this);
		}
		public void Intersection(IMinMaxValues minMaxValues) {
			MinMaxValues intersection = MinMaxValues.Intersection(this, minMaxValues);
			this.max = intersection.max;
			this.min = intersection.min;
		}
		public MinMaxValues Clone() {
			return new MinMaxValues(min, max);
		}
		public override bool Equals(object obj) {
			if (!(obj is MinMaxValues))
				return false;
			MinMaxValues value = (MinMaxValues)obj;
			return Min == value.Min && Max == value.Max;
		}
		public override int GetHashCode() {
			return min.GetHashCode() ^ max.GetHashCode();
		}
	}
}
