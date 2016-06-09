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
namespace DevExpress.Charts.Native {
	public abstract class RefinedPointComparerBase : Comparer<RefinedPoint> {
		public virtual bool IsSupportedPoint(RefinedPoint point) { return true; }
	}
	public class SeriesPointSettingsComparer : RefinedPointComparerBase {
		readonly bool isAsceding;
		readonly SeriesPointKeyNative sortingPointKey;
		Scale valueScaleType;
		Scale argumentScaleType;
		public SeriesPointSettingsComparer(bool isAsceding, SeriesPointKeyNative sortingKey, Scale valueScaleType, Scale argumentScaleType) {
			this.isAsceding = isAsceding;
			this.sortingPointKey = sortingKey;
			this.valueScaleType = valueScaleType;
			this.argumentScaleType = argumentScaleType;
		}
		Scale GetScale() {
			if (sortingPointKey == SeriesPointKeyNative.Argument)
				return argumentScaleType;
			else
				return valueScaleType;
		}
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				return -1;
			if (pointInArray == null)
				return 1;
			int result;
			switch (GetScale()) {
				case Scale.Qualitative: {
						string value1 = GetQualitativeValue(pointInArray);
						string value2 = GetQualitativeValue(newРoint);
						result = isAsceding ? value1.CompareTo(value2) : value2.CompareTo(value1);
					}
					break;
				case Scale.Numerical: {
						double value1 = GetValue(pointInArray);
						double value2 = GetValue(newРoint);
						result = isAsceding ? value1.CompareTo(value2) : value2.CompareTo(value1);
					}
					break;
				case Scale.DateTime: {
						DateTime value1 = GetDateTimeValue(pointInArray);
						DateTime value2 = GetDateTimeValue(newРoint);
						result = isAsceding ? value1.CompareTo(value2) : value2.CompareTo(value1);
					}
					break;
				default :
					result = 0;
					break;
			}
			return  result == 0 ? pointInArray.Index.CompareTo(newРoint.Index) : result;
		}
		double GetValue(RefinedPoint refinedPoint) {
			switch (sortingPointKey) {
				case SeriesPointKeyNative.Argument:
					return refinedPoint.SeriesPoint.NumericalArgument;
				case SeriesPointKeyNative.Value_1:
					return refinedPoint.SeriesPoint.UserValues[0];
				case SeriesPointKeyNative.Value_2:
					return refinedPoint.SeriesPoint.UserValues[1];
				case SeriesPointKeyNative.Value_3:
					return refinedPoint.SeriesPoint.UserValues[2];
				case SeriesPointKeyNative.Value_4:
					return refinedPoint.SeriesPoint.UserValues[3];
				default:
					return refinedPoint.SeriesPoint.NumericalArgument;
			}
		}
		string GetQualitativeValue(RefinedPoint refinedPoint) {
			return refinedPoint.SeriesPoint.QualitativeArgument;
		}
		DateTime GetDateTimeValue(RefinedPoint refinedPoint) {
			switch (sortingPointKey) {
				case SeriesPointKeyNative.Argument:
					return refinedPoint.SeriesPoint.DateTimeArgument;
				case SeriesPointKeyNative.Value_1:
					return refinedPoint.SeriesPoint.DateTimeValues[0];
				case SeriesPointKeyNative.Value_2:
					return refinedPoint.SeriesPoint.DateTimeValues[1];
				case SeriesPointKeyNative.Value_3:
					return refinedPoint.SeriesPoint.DateTimeValues[2];
				case SeriesPointKeyNative.Value_4:
					return refinedPoint.SeriesPoint.DateTimeValues[3];
				default:
					return refinedPoint.SeriesPoint.DateTimeValues[0];
			}
		}
	}
	public class RefinedPointsArgumentAndIndexComparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(point.Argument); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				return -1;
			if (pointInArray == null)
				return 1;
			if (double.IsNaN(pointInArray.Argument) || double.IsNaN(newРoint.Argument)) 
				if(double.IsNaN(pointInArray.Argument) && double.IsNaN(newРoint.Argument))
					return 0;
				else 
					return double.IsNaN(pointInArray.Argument) ? 1 : -1;
			int res = pointInArray.Argument.CompareTo(newРoint.Argument);
			return res == 0 ? pointInArray.Index.CompareTo(newРoint.Index) : res;
		}
	}
	public class RefinedPointsArgumentComparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(point.Argument); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				return - 1;
			if (pointInArray == null)
				return 1;
			if (double.IsNaN(pointInArray.Argument) ^ double.IsNaN(newРoint.Argument))
				return double.IsNaN(pointInArray.Argument) ? 1 : -1;
			return pointInArray.Argument.CompareTo(newРoint.Argument);
		}
	}
	public class RefinedPointsValueComparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(((IValuePoint)point).Value); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				throw new ArgumentException();
			if (pointInArray == null)
				return 1;
			return ((IValuePoint)pointInArray).Value.CompareTo(((IValuePoint)newРoint).Value);
		}
	}
	public class RefinedPointsRangeValue1Comparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(((IRangePoint)point).Value1); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				throw new ArgumentException();
			if (pointInArray == null)
				return 1;
			return ((IRangePoint)pointInArray).Value1.CompareTo(((IRangePoint)newРoint).Value1);
		}
	}
	public class RefinedPointsRangeValue2Comparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(((IRangePoint)point).Value2); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				throw new ArgumentException();
			if (pointInArray == null)
				return 1;
			return ((IRangePoint)pointInArray).Value2.CompareTo(((IRangePoint)newРoint).Value2);
		}
	}
	public class RefinedPointsWeightComparer : RefinedPointComparerBase {
		public override bool IsSupportedPoint(RefinedPoint point) { return !double.IsNaN(((IXYWPoint)point).Weight); }
		public override int Compare(RefinedPoint pointInArray, RefinedPoint newРoint) {
			if (newРoint == null)
				throw new ArgumentException();
			if (pointInArray == null)
				return 1;
			return ((IXYWPoint)pointInArray).Weight.CompareTo(((IXYWPoint)newРoint).Weight);
		}
	}
}
