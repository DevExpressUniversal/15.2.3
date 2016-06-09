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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class StripLimitsUtils {
		public static bool CheckLimits(Axis2D axis, object minAxisValue, object maxAxisValue) {
			if (axis == null)
				return true;
			try {
				if (axis.ScaleType == ActualScaleType.DateTime) {
					double diff = DateTimeUtils.GetDifference((DateTimeMeasureUnitNative)axis.DateTimeScaleOptions.GetActualMeasureUnit(),
						(DateTimeMeasureUnitNative)axis.DateTimeScaleOptions.GetActualGridAlignment(), Convert.ToDateTime(minAxisValue),
						Convert.ToDateTime(maxAxisValue), axis.DateTimeScaleOptions.WorkdaysOptions);
					return diff >= 0;
				}
				else if (axis.ScaleType == ActualScaleType.Numerical)
					return Convert.ToDouble(minAxisValue) <= Convert.ToDouble(maxAxisValue);
				else if (axis.ScaleType == ActualScaleType.Qualitative) {
					double minIndex = axis.ScaleTypeMap.NativeToInternal(minAxisValue);
					double maxIndex = axis.ScaleTypeMap.NativeToInternal(maxAxisValue);
					if (double.IsNaN(minIndex) && double.IsNaN(maxIndex))
						return String.Compare(minAxisValue.ToString(), maxAxisValue.ToString()) != 0;
					else
						return double.IsNaN(maxIndex) || minIndex < 0 || double.IsNaN(minIndex) || maxIndex < 0 || minIndex <= maxIndex;
				}
			}
			catch {
			}
			return true;
		}
	}
}
