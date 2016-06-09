#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Viewer {
	public class GaugeRangeCalculator {
		double min;
		double max;
		double? customMin;
		double? customMax;
		IEnumerable<double> values;
		int minTickCount;
		int maxTickCount;
		bool EqualSign { get { return Math.Sign(min) == Math.Sign(max); } }
		bool ScaleReversed { get { return min > max; } }
		public GaugeRangeCalculator(IEnumerable<double> values, GaugeViewType viewType, double? customMin, double? customMax) {
			this.values = values;
			this.customMin = customMin;
			this.customMax = customMax;
			SetMinMaxTicks(viewType);
		}
		public GaugeRangeModel GetGaugeRangeModel() {
			DefineMinMax();
			SetRangeStart();
			ExtendRange();
			double left = Math.Min(min, max);
			double right = Math.Max(max, min);
			int majorTickCount;
			int minorTickCount;
			double rangeLength = right - left;
			if(rangeLength == 0) {
				majorTickCount = 1;
				minorTickCount = 0;
			}
			else {
				int stepCount = minTickCount - 1;
				double step = MultiplierChooser.ChooseMultiplier(rangeLength / stepCount);
				double delta = step * stepCount - rangeLength;
				bool fit = IsFit(left, right, step, stepCount);
				for(int i = stepCount + 1; i < maxTickCount; i++) {
					double currentStep = MultiplierChooser.ChooseMultiplier(rangeLength / i);
					double currentDelta = currentStep * i - rangeLength;
					bool currentFit = IsFit(left, right, currentStep, i);
					if(currentFit && (currentDelta < delta || !fit)) {
						delta = currentDelta;
						step = currentStep;
						fit = currentFit;
						stepCount = i;
					}
				}
				left = GetLeft(left, step);
				right = GetRight(right, step);
				min = !ScaleReversed ? left : right;
				max = !ScaleReversed ? right : left;
				majorTickCount = stepCount + 1;
				if(step % 5 == 0)
					minorTickCount = 4;
				else if(step % 3 == 0)
					minorTickCount = 2;
				else
					minorTickCount = 3;
			}
			return new GaugeRangeModel {
				MinRangeValue = min,
				MaxRangeValue = max,
				MinorTickCount = minorTickCount,
				MajorTickCount = majorTickCount
			};
		}
		double GetLeft(double left, double step) {
			if(EqualSign && left > 0)
				return Math.Floor(Math.Abs(left) / step) * step * Math.Sign(left);
			else
				return Math.Ceiling(Math.Abs(left) / step) * step * Math.Sign(left);
		}
		double GetRight(double right, double step) {
			if(EqualSign && right < 0)
				return Math.Floor(Math.Abs(right) / step) * step * Math.Sign(right);
			else
				return Math.Ceiling(Math.Abs(right) / step) * step * Math.Sign(right);
		}
		bool IsFit(double left, double right, double step, int tickCount) {
			double leftAbs = Math.Abs(left);
			double rigthAbs = Math.Abs(right);
			bool isFit = false;
			if(Math.Sign(left) != Math.Sign(right)) {
				isFit = (Math.Ceiling(leftAbs / step) + Math.Ceiling(rigthAbs / step)) <= tickCount;
			}
			else {
				double minAbs = Math.Min(leftAbs, rigthAbs);
				double maxAbs = Math.Max(leftAbs, rigthAbs);
				isFit = Math.Ceiling(maxAbs / step) - Math.Floor(minAbs / step) <= tickCount;
			}
			return isFit;
		}
		void DefineMinMax() {
			if(customMin.HasValue)
				min = customMin.Value;
			else
				min = values.Count() > 0 ? values.Min() : 0d;
			if(customMax.HasValue)
				max = customMax.Value;
			else
				max = values.Count() > 0 ? values.Max() : 1d;
		}
		void SetRangeStart() {
			if(EqualSign) {
				if(Double.Equals(min, max)) {
					if(min > 0)
						SetMinAsStart();
					else
						SetMaxAsStart();
				}
				if(Math.Abs(min) <= Math.Abs(max))
					SetMinAsStart();
				else
					SetMaxAsStart();
			}
		}
		void SetMinAsStart() {
			if(!customMin.HasValue)
				min = 0d;
		}
		void SetMaxAsStart() {
			if(!customMax.HasValue)
				max = 0d;
		}
		void ExtendRange() {
			if(EqualSign) {
				if(Math.Abs(min) < Math.Abs(max)) {
					ExtendMin(0.95d);
					ExtendMax(1.05d);
				}
				else {
					ExtendMin(1.05d);
					ExtendMax(0.95d);
				}
			}
			else {
				ExtendMin(1.05d);
				ExtendMax(1.05d);
			}
			if(min == max)
				max *= 1.4d;
		}
		void ExtendMin(double factor) {
			if(!customMin.HasValue)
				min *= factor;
		}
		void ExtendMax(double factor) {
			if(!customMax.HasValue)
				max *= factor;
		}
		void SetMinMaxTicks(GaugeViewType viewType) {
			switch(viewType) {
				case GaugeViewType.CircularFull:
					minTickCount = 6;
					maxTickCount = 9;
					break;
				case GaugeViewType.CircularHalf:
				case GaugeViewType.CircularThreeFourth:
					minTickCount = 4;
					maxTickCount = 6;
					break;
				case GaugeViewType.LinearHorizontal:
					minTickCount = 3;
					maxTickCount = 3;
					break;
				default:
					minTickCount = 4;
					maxTickCount = 5;
					break;
			}
		}
	}
}
