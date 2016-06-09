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
using System.ComponentModel;
namespace DevExpress.Map {
	public interface IRangeDistribution {
		double ConvertRangeValue(double min, double max, double value);
	}
	public abstract class RangeDistributionBase : IRangeDistribution {
		public abstract double ConvertRangeValue(double min, double max, double value);
	}
}
namespace DevExpress.Map.Native {
	public static class RangeDistributionHelper {
		const double Epsilon = 0.000001;
		static bool IsValidValuesRange(double val) {
			return (!Double.IsInfinity(val)) && (Math.Abs(val) > Epsilon);
		}
		public static double CalcLinearRangeValue(double min, double max, double value) {
			double valuesRange = max - min;
			return IsValidValuesRange(valuesRange) ? (value - min) / valuesRange : 0.5;
		}
		public static double CalcLogarithmicRangeValue(double min, double max, double value, double factor) {
			return 1 - CalcExponentialRangeValue(min, max, max + min - value, factor);
		}
		public static double CalcExponentialRangeValue(double min, double max, double value, double factor) {
			double normValue = CalcLinearRangeValue(min, max, value);
			return Math.Pow(normValue, factor);
		}
	}
}
