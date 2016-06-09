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
	public static class AxisCoordCalculator {
		public static double GetCoord(IMinMaxValues range, double value, double dimension) {
			return (value - range.Min) / (range.Max - range.Min) * dimension;
		}
		public static double GetClampedCoord(IMinMaxValues range, double value, double dimension) {
			if (Double.IsNegativeInfinity(value) || value < range.Min)
				return 0.0;
			if (Double.IsPositiveInfinity(value) || value > range.Max)
				return dimension;
			return GetCoord(range, value, dimension);
		}
		public static double GetInternalValue(IMinMaxValues range, double ratio) {
			if (ratio < 1.0) {
				if (ratio > 0.0)
					return range.Min + (range.Max - range.Min) * ratio;
				else
					return range.Min; ;
			}
			else
				return range.Max;
		}
		public static double GetInternalValue(double min, double max, double ratio) {
			if (ratio < 1.0) {
				if (ratio > 0.0)
					return min + (max - min) * ratio;
				else
					return min; ;
			}
			else
				return max;
		}
		public static double GetCoord(IMinMaxValues range, double value, double dimension, bool clamped) {
			if (clamped)
				return GetClampedCoord(range, value, dimension);
			return GetCoord(range, value, dimension);
		}
	}
}
