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

using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisInterval {
		readonly IMinMaxValues range;
		readonly IMinMaxValues wholeRange;
		public IMinMaxValues Range { get { return range; } }
		public IMinMaxValues WholeRange { get { return wholeRange; } }
		public double Delta { get { return range.Max - range.Min; } }
		public AxisInterval(IMinMaxValues range, IMinMaxValues wholeRange) {
			this.range = range;
			this.wholeRange = wholeRange;
		}
		public double GetInternalValue(double ratio, ITransformation transformation) {
			return AxisCoordCalculator.GetInternalValue(transformation.TransformForward(range.Min), transformation.TransformForward(range.Max), ratio);
		}
	}
	public class RangeWrapper : IMinMaxValues {
		double min = double.NaN;
		double max = double.NaN;
		IMinMaxValues range;
		public double Max {
			get {
				if (!double.IsNaN(max))
					return max;
				return range.Max;
			}
			set {
				max = value;
			}
		}
		public double Min {
			get {
				if (!double.IsNaN(min))
					return min;
				return range.Min;
			}
			set {
				min = value;
			}
		}
		public double Delta {
			get { return Max - Min; }
		}
		public RangeWrapper(IMinMaxValues range) {
			this.range = range;
		}
		public double CalculateCenter() {
			return (Max + Min) / 2;
		}
		public void Intersection(IMinMaxValues minMaxValues) {
			MinMaxValues intersection = MinMaxValues.Intersection(this, minMaxValues);
			this.Max = intersection.Max;
			this.Min = intersection.Min;
		}
		public override string ToString() {
			return Min + "; " + Max;
		}
	}
}
