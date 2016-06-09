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
	public enum PointViewKind {
		Argument,
		Values,
		ArgumentAndValues,
		SeriesName,
		Undefined
	}
	public static class PointOptionsHelper {
		public const string DefaultPattern = PatternUtils.ValuesPattern;
		public const PointViewKind DefaultPointView = PointViewKind.Values;
		public static string ConvertToPattern(PointViewKind pointView) {
			switch (pointView) {
				case PointViewKind.Argument:
					return PatternUtils.ArgumentPattern;
				case PointViewKind.Values:
					return PatternUtils.ValuesPattern;
				case PointViewKind.ArgumentAndValues:
					return PatternUtils.ArgumentAndValuesPattern;
				case PointViewKind.SeriesName:
					return PatternUtils.SeriesNamePattern;
				default:
					return String.Empty;
			}
		}
		public static PointViewKind ConvertToPointView(string pattern) {
			if (String.IsNullOrEmpty(pattern))
				return DefaultPointView;
			bool argumentPatternFound = pattern.IndexOf(PatternUtils.ArgumentPattern) >= 0 || 
										pattern.IndexOf(PatternUtils.ArgumentPatternLowercase) >= 0;
			bool valuesPatternFound = pattern.IndexOf(PatternUtils.ValuesPattern) >= 0 || 
									  pattern.IndexOf(PatternUtils.ValuesPatternLowercase) >= 0;
			bool seriesNamePatternFound = pattern.IndexOf(PatternUtils.SeriesNamePattern) >= 0 || 
										  pattern.IndexOf(PatternUtils.SeriesNamePatternLowercase) >= 0;
			if (seriesNamePatternFound)
				return (argumentPatternFound || valuesPatternFound) ? PointViewKind.Undefined : PointViewKind.SeriesName;
			if (argumentPatternFound)
				return valuesPatternFound ? PointViewKind.ArgumentAndValues : PointViewKind.Argument;
			return valuesPatternFound ? PointViewKind.Values : PointViewKind.Undefined;
		}
		public static string ConstructArgumentText(ISeries series, ISeriesPoint point, INumericOptions numericOptions, IDateTimeOptions dateTimeOptions) {
			if (point.UserArgument == null)
				return string.Empty;
			IXYSeriesView xyView = series.SeriesView as IXYSeriesView;
			switch (series.ArgumentScaleType) {
				case Scale.Qualitative:
					return point.QualitativeArgument;
				case Scale.Numerical:
					return NumericOptionsHelper.GetValueText(point.NumericalArgument, numericOptions);
				case Scale.DateTime:
					return DateTimeOptionsHelper.GetValueText(point.DateTimeArgument, dateTimeOptions);
				default:
					throw new Exception("Incorrect argument scale type.");
			}
		}
	}
}
