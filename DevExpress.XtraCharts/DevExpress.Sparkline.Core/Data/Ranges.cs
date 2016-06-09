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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.ComponentModel;
using DevExpress.Utils.Design;
namespace DevExpress.Sparkline {
	public sealed class SparklineRange {
		const double DefaultLimit1 = 0;
		const double DefaultLimit2 = 1;
		const bool DefaultAuto = true;
		double limit1 = DefaultLimit1;
		double limit2 = DefaultLimit2;
		bool isAuto = DefaultAuto;
		[Browsable(false)]
		public event EventHandler PropertiesChanged;
		internal double Min {
			get { return Math.Min(limit1, limit2); }
		}
		internal double Max {
			get { return Math.Max(limit1, limit2); }
		}
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("SparklineRangeLimit1")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.Sparkline.SparklineRange.Limit1")]
		[DefaultValue(DefaultLimit1)]
		public double Limit1 {
			get { return limit1; }
			set {
				if (limit1 != value) {
					limit1 = value;
					if (!isAuto)
						OnPropertiesChanged();
				}
			}
		}
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("SparklineRangeLimit2")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.Sparkline.SparklineRange.Limit2")]
		[DefaultValue(DefaultLimit2)]
		public double Limit2 {
			get { return limit2; }
			set {
				if (limit2 != value) {
					limit2 = value;
					if (!isAuto)
						OnPropertiesChanged();
				}
			}
		}
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("SparklineRangeIsAuto")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.Sparkline.SparklineRange.IsAuto")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		[DefaultValue(DefaultAuto)]
		public bool IsAuto {
			get { return isAuto; }
			set {
				if (isAuto != value) {
					isAuto = value;
					OnPropertiesChanged();
				}
			}
		}
		public SparklineRange() {
		}
		public SparklineRange(double min, double max) {
			this.limit1 = min;
			this.limit2 = max;
			this.isAuto = false;
		}
		void OnPropertiesChanged() {
			if (PropertiesChanged != null)
				PropertiesChanged(this, EventArgs.Empty);
		}
		public override string ToString() {
			return string.Format("SparklineRange [{0}:{1}]", Min, Max);
		}
	}
}
namespace DevExpress.Sparkline.Core {
	public sealed class SparklineRangeData {
		double min;
		double max;
		public double Min {
			get { return min; }
		}
		public double Max {
			get { return max; }
		}
		public double Delta {
			get { return Max - Min; }
		}
		public bool IsValid {
			get { return !double.IsNaN(Min) && !double.IsNaN(Max); }
		}
		public SparklineRangeData() {
			Invalidate();
		}
		public SparklineRangeData(double min, double max) {
			this.min = min;
			this.max = max;
		}
		internal void Extend(double value) {
			if (SparklineMathUtils.IsValidDouble(value)) {
				if (double.IsNaN(min))
					min = value;
				else if (min > value)
					min = value;
				if (double.IsNaN(max))
					max = value;
				else if (max < value)
					max = value;
			}
		}
		internal void Invalidate() {
			min = double.NaN;
			max = double.NaN;
		}
		public void Set(double min, double max) {
			this.min = min;
			this.max = max;
		}
		public bool InRange(double value) {
			return (value >= min) && (value <= max);
		}
		public override string ToString() {
			if (IsValid)
				return string.Format("SparklineRange [{0}:{1}]", min, max);
			return "Invalid SparklineRange";
		}
	}
	public sealed class SparklineInteractionRanges {
		readonly SparklineRangeData dataArgumentRange;
		readonly SparklineRangeData dataValueRange;
		readonly SparklineRangeData visualArgumentRange;
		readonly SparklineRangeData customDataArgumentRange;
		double minPointDistance;
		public SparklineRangeData ActualDataArgumentRange {
			get { return customDataArgumentRange.IsValid ? customDataArgumentRange : dataArgumentRange; }
		}
		public SparklineRangeData DataArgumentRange {
			get { return dataArgumentRange; }
		}
		public SparklineRangeData DataValueRange {
			get { return dataValueRange; }
		}
		public SparklineRangeData VisualArgumentRange {
			get { return visualArgumentRange; }
		}
		public SparklineRangeData CustomDataArgumentRange {
			get { return customDataArgumentRange; }
		}
		public double MinPointDistance {
			get { return minPointDistance; }
		}
		internal SparklineInteractionRanges() {
			this.dataArgumentRange = new SparklineRangeData();
			this.dataValueRange = new SparklineRangeData();
			this.customDataArgumentRange = new SparklineRangeData();
			this.visualArgumentRange = new SparklineRangeData(0.0, 1.0);
			this.minPointDistance = double.NaN;
		}
		internal void Invalidate() {
			dataArgumentRange.Invalidate();
			dataValueRange.Invalidate();
			minPointDistance = double.NaN;
		}
		internal void UpdateMinPointDistance(double value) {
			if (SparklineMathUtils.IsValidDouble(value)) { 
				if (double.IsNaN(minPointDistance) || (minPointDistance > value))
					minPointDistance = value;
			}
		}
	}
	public sealed class SparklineIndexRange {
		int min;
		int max;
		public int Min {
			get { return min; }
			set {
				if (value < 0)
					throw new ArgumentException("Index must be greater than or equal to 0.", "MinMaxIndexes.Max");
				min = value;
			}
		}
		public int Max {
			get { return max; }
			set {
				if (value < 0)
					throw new ArgumentException("Index must be greater than or equal to 0.", "MinMaxIndexes.Max");
				max = value;
			}
		}
		public bool IsValid {
			get { return (min >= 0) && (max >= 0); }
		}
		internal SparklineIndexRange() : this(-1, -1) { }
		internal SparklineIndexRange(int start, int end) {
			this.min = start;
			this.max = end;
		}
		public override string ToString() {
			return string.Format("IndexRange [{0}:{1}]", min, max);
		}
	}
}
