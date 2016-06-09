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
namespace DevExpress.Charts.Native {
	public class MissingArgumentsCalculator {
		const double eps = 0.000001;
		const double internalArgumentStep = 1.0;
		readonly AxisScaleTypeMap axisXScaleMap;
		readonly ISeries series;
		public MissingArgumentsCalculator(AxisScaleTypeMap axisXScaleMap, ISeries series) {
			this.axisXScaleMap = axisXScaleMap;
			this.series = series;
		}
		RefinedPoint CreatePointForMissingArgument(double internalArgument, ProcessMissingPointsModeNative processMissingPoints) {
			object nativeArgument = axisXScaleMap.InternalToNative(internalArgument);
			double refinedArgument = axisXScaleMap.InternalToRefined(internalArgument);
			ISeriesPoint seriesPoint = series.CreateSeriesPoint(nativeArgument);
			RefinedPoint refinedPoint = new RefinedPoint(seriesPoint, refinedArgument, 0.0);
			refinedPoint.IsEmpty = processMissingPoints == ProcessMissingPointsModeNative.InsertEmptyPoints || series.ValueScaleType == Scale.DateTime;
			return refinedPoint;
		}
		bool EqualValues(double value1, double value2, double epsilon) {
			return Math.Abs(value1 - value2) <= epsilon;
		}
		public IList<RefinedPoint> FillMissingArguments(IList<RefinedPoint> sourcePoints, ProcessMissingPointsModeNative processMissingPoints) {
			if (processMissingPoints == ProcessMissingPointsModeNative.Skip || sourcePoints.Count == 0)
				return sourcePoints;
			IList<RefinedPoint> points = new List<RefinedPoint>();
			double previousArgument = double.NaN;
			double currentArgument = axisXScaleMap.RefinedToInternal(sourcePoints[0].Argument);
			int pointIndex = 0;
			while (pointIndex < sourcePoints.Count) {
				RefinedPoint point = sourcePoints[pointIndex];
				double currentPointArgument = axisXScaleMap.RefinedToInternal(point.Argument);
				if (EqualValues(previousArgument, currentPointArgument, eps))
					pointIndex++;
				else {
					if (EqualValues(currentArgument, currentPointArgument, eps))
						pointIndex++;
					else
						point = CreatePointForMissingArgument(currentArgument, processMissingPoints);
					previousArgument = currentArgument;
					currentArgument += internalArgumentStep;
				}
				points.Add(point);
			}
			return points;
		}
	}
}
