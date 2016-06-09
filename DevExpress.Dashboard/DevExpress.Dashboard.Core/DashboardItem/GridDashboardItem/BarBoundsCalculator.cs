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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public static class BarBoundsCalculator {
		public static Rectangle CalculateBounds(Rectangle targetBounds, decimal normalizedValue, decimal zeroValue, bool allowNegativeAxis) {
			int width = Convert.ToInt32(targetBounds.Width * Math.Abs(normalizedValue));
			int zeroXCoordanate = targetBounds.X + Convert.ToInt32(zeroValue * targetBounds.Width);
			int x = allowNegativeAxis && normalizedValue < 0 ? zeroXCoordanate - width : zeroXCoordanate;
			return new Rectangle(x, targetBounds.Y, width, targetBounds.Height);
		}
		public static Rectangle CalculateBounds(IBarStyleSettingsInfo barInfo, Rectangle cellBounds, object value) {
			bool isNegativeNumber = Convert.ToDecimal(value) < 0;
			Rectangle barBounds = CalculateBounds(cellBounds, barInfo.NormalizedValue, barInfo.ZeroPosition, barInfo.AllowNegativeAxis);
			if(barInfo.AllowNegativeAxis && barInfo.DrawAxis && isNegativeNumber && barInfo.ZeroPosition > 0)
				barBounds.Width += 1;
			return barBounds;
		}
		public static void CalculateAxisPoints(IBarStyleSettingsInfo barInfo, Rectangle cellBounds, out Point topPoint, out Point bottomPoint) {
			int xCord = cellBounds.X + Convert.ToInt32(barInfo.ZeroPosition * cellBounds.Width);
			topPoint = new Point(xCord, cellBounds.Top + 2);
			bottomPoint = new Point(xCord, cellBounds.Bottom - 3);
		}
	}
}
