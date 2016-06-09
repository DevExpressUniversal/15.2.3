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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public static class ValueMapScaleHelper {
		public static Color GetGradientColor(Color color1, Color color2, int index, int colorCount) {
			int stepCount = colorCount - 1;
			if(stepCount == 0)
				return color1;
			else
				return Color.FromArgb(
					AverageValue(color1.A, color2.A, index, stepCount),
					AverageValue(color1.R, color2.R, index, stepCount),
					AverageValue(color1.G, color2.G, index, stepCount),
					AverageValue(color1.B, color2.B, index, stepCount)
				);
		}
		static int AverageValue(int value1, int value2, int step, int stepCount) {
			return value1 + (value2 - value1) * step / stepCount;
		}
		public static List<double> GetPercentRangeStops(int rangeStopsCount) {
			var marks = new List<double>(rangeStopsCount);
			for (int i = 0; i < rangeStopsCount; i++)
				marks.Add(i * 100 / rangeStopsCount);
			return marks;
		}
	}
}
